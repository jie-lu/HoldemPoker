using HoldemPoker.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace HoldemPoker
{
	public class Game
	{
		public enum StageEnum
		{
			NotStarted = -1,
			Preflop,
			Flop,
			Turn,
			River,
			End
		}

		private Deck _deck = new Deck();
		private List<Player> _audiencePlayers = new List<Player>();
		private List<Player> _players = new List<Player>();
		private List<string> _sharedCards = new List<string>();
		private List<string> _winnerIds = new List<string>();
		private List<GameEvent> _events = new List<GameEvent>();
		private IHubContext<GameHub, IGameClient> _gameHubContext;

		public Game(IHubContext<GameHub, IGameClient> gameHubContext)
		{
			_gameHubContext = gameHubContext;

			Stage = StageEnum.NotStarted;
			DealerIndex = -1;
			CurrentPlayerIndex = -1;
			StartingChips = 500;
			Blind = 10;
			PlayerTimeout = 10;
		}

		public IReadOnlyList<Player> AudiencePlayers { get { return _audiencePlayers; } }

		public IReadOnlyList<Player> Players { get { return _players; } }

		public int DealerIndex { get; private set; }

		public int CurrentPlayerIndex { get; private set; }

		public Player CurrentPlayer 
		{
			get
			{
				if (CurrentPlayerIndex < 0) return null;

				return this.Players[this.CurrentPlayerIndex];
			}
			private set 
			{
				CurrentPlayerIndex = -1;
			}
		}

		public IReadOnlyList<string> SharedCards { get { return _sharedCards; } }

		public StageEnum Stage { get; private set; }

		public bool IsInprocess
		{
			get { return Stage != StageEnum.NotStarted && Stage != StageEnum.End; }
		}

		public int MaxBetInCurrentStage { get; private set; }

		public int Blind { get; set; }

		public IReadOnlyList<string> WinnerIds { get { return _winnerIds; } }

		public int StartingChips { get; set; }

		public int PlayerTimeout { get; private set; }

		public event Func<int, Task> CurrentPlayerChanged;

		public async Task PublishEvent(GameEvent gameEvent)
		{
			_events.Add(gameEvent);
			await _gameHubContext.Clients.All.Update(GetSnapshot(gameEvent));
		}

		public GameState GetSnapshot(GameEvent gameEvent)
		{
			var state = new GameState(gameEvent);
			state.Players.AddRange(_players);
			state.SharedCards.AddRange(_sharedCards);
			state.CurrentPlayerIndex = CurrentPlayerIndex;
			state.DealerIndex = DealerIndex;
			state.Stage = (int)Stage;
			state.MaxBetAtThisStage = MaxBetInCurrentStage;
			state.WinnerIds.AddRange(_winnerIds);
			state.Blind = Blind;

			return state;
		}

		private void BlindBets()
		{
			var remainingPlayers = _players.Where(p => p.Chips > 0);

			// When only two players remain, the dealer button posts the small blind, 
			// while his / her opponent places the big blind.
			if (remainingPlayers.Count() == 2)
			{
				PlayerBet(DealerIndex, Blind / 2);

				CurrentPlayerIndex = GetNextActivePlayerIndex(DealerIndex);
				PlayerBet(CurrentPlayerIndex, Blind);
			}
			else
			{
				CurrentPlayerIndex = GetNextActivePlayerIndex(DealerIndex);
				PlayerBet(CurrentPlayerIndex, Blind / 2);

				CurrentPlayerIndex = GetNextActivePlayerIndex(CurrentPlayerIndex);
				PlayerBet(CurrentPlayerIndex, Blind);
			}
		}

		private async Task SetCurrentPlayer(int playerIndex)
		{
			var lastPlayerIndex = CurrentPlayerIndex;
			CurrentPlayerIndex = playerIndex;

			if (CurrentPlayer != null)
			{
				CurrentPlayer.CountDown = PlayerTimeout;

				await PublishEvent(new GameEvent
				{
					PlayerId = CurrentPlayer.Id,
					EventType = GameEvent.EventTypeEnum.Wait
				});
			}
			await CurrentPlayerChanged?.Invoke(lastPlayerIndex);
		}

		private void CalculateWinners()
		{
			var inGamePlayers = _players.Where(p => p.Cards.Any());
			var showdownPlayers = inGamePlayers.Where(p => !p.HasFolded);

			// All players have folded except one.
			if (showdownPlayers.Count() == 1)
			{
				var winner = showdownPlayers.First();
				winner.Chips += inGamePlayers.Sum(p => p.Bets.Sum());
				_winnerIds.Add(winner.Id);
			}
			else if (showdownPlayers.Count() > 1)
			{
				var calc = new BestHandCalculator();
				var playerBets = new Dictionary<Player, int>();
				foreach (Player p in inGamePlayers)
				{
					playerBets[p] = p.Bets.Sum();
					if (!p.HasFolded)
					{
						p.BestHand = calc.CalculateBestHand(p.Cards.Union(SharedCards));
					}
				}
				var orderedShowdownPlayerGroups = showdownPlayers
					.GroupBy(p => p.BestHand, new PokerHandEqualityComparer())
					.OrderByDescending(g => g.Key, new PokerHandComparer());
				foreach (IGrouping<PokerHand, Player> winnerGroups in orderedShowdownPlayerGroups)
				{
					var orderedShowdownPlayers = winnerGroups.OrderBy(p => p.Bets.Sum());
					var winnerCountForSplittingPot = winnerGroups.Count();
					var splittedBetFromPrevsousWinners = 0;
					foreach (Player winner in winnerGroups)
					{
						// Returns the winner's bet to himself.
						var winnerBet = playerBets[winner];
						var winnedChips = winnerBet;
						playerBets.Remove(winner);

						// Add the splitted bet from previsous winners to the current winner.
						winnedChips += splittedBetFromPrevsousWinners;

						// Calcuate how many chips should this winner get from other players.
						foreach (Player p in playerBets.Keys.ToList())
						{
							// Winners won't lose chips.
							if (winnerGroups.Contains(p)) continue;

							// Cut the loser's bet according to the winner's bet.
							var lost = Math.Min(winnerBet, playerBets[p]);
							playerBets[p] -= lost;

							// Split the loser's bet to winners
							var splittedBet = lost / winnerCountForSplittingPot;
							winnedChips += splittedBet;
							splittedBetFromPrevsousWinners += splittedBet;
						}

						winner.Chips += winnedChips;
						if (winnedChips > winnerBet)
						{
							_winnerIds.Add(winner.Id);
						}

						// Decrease the winner count after the current winner has been calculated.
						winnerCountForSplittingPot--;
					}

					// The pot is clear.
					if (playerBets.Sum(p => p.Value) == 0) break;
				}
			}
			else
			{
				throw new InvalidOperationException("Internal error: All players have folded.");
			}
		}

		public async Task OnPlayerConnected(string playerId)
		{
			var newPlayer = new Player
			{
				Id = playerId,
				IsConnected = true,
				Chips = StartingChips
			};

			var player = _players.Where(p => p.Id == playerId).FirstOrDefault();
			if (player != null)
			{
				player.IsConnected = true;
			}
			else
			{
				if (IsInprocess)
				{
					if (!_audiencePlayers.Any(p => p.Id == playerId))
					{
						_audiencePlayers.Add(newPlayer);
					}
				}
				else
				{
					_players.Add(newPlayer);
				}
			}

			var gameEvent = new GameEvent
			{
				PlayerId = playerId,
				EventType = GameEvent.EventTypeEnum.Join
			};
			await PublishEvent(gameEvent);
		}

		public async Task OnPlayerDisconnected(string playerId)
		{
			var player = _players.Where(p => p.Id == playerId).FirstOrDefault();
			if (player != null)
			{
				if (IsInprocess)
				{
					player.IsConnected = false;
				}
				else
				{
					_players.Remove(player);
				}
			}
			else 
			{
				player = _audiencePlayers.Where(p => p.Id == playerId).FirstOrDefault();
				if (player != null)
				{
					_audiencePlayers.Remove(player);
				}
			}

			var gameEvent = new GameEvent
			{
				PlayerId = playerId,
				EventType = GameEvent.EventTypeEnum.Leave
			};
			await PublishEvent(gameEvent);
		}

		private int PlayerBet(int playerIndex, int amount)
		{
			var player = _players[playerIndex];

			// Put the bet to the pot
			player.Bets[(int)this.Stage] += amount;
			player.Chips -= amount;

			// Update the max bet.
			if (player.Bets[(int)this.Stage] > MaxBetInCurrentStage)
			{
				MaxBetInCurrentStage = player.Bets[(int)this.Stage];
			}

			return amount;
		}

		private void PlayerFold(int playerIndex)
		{
			var player = _players[playerIndex];

			player.HasFolded = true;
		}

		/// <summary>
		/// Returns the next player's index who can post a bet or fold.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private int GetNextActivePlayerIndex(int index, bool checkBet = true)
		{
			var nextPlayerIndex = (index + 1) % _players.Count;
			var nextPlayer = _players[nextPlayerIndex];
			while (nextPlayerIndex != index)
			{
				if (!nextPlayer.HasFolded && nextPlayer.Chips > 0)
				{
					if (checkBet && nextPlayer.Cards.Any())
					{
						if (nextPlayer.LastActionInCurrentStage == null) break;
						else if (nextPlayer.Bets[(int)Stage] < MaxBetInCurrentStage) break;
						//else if (MaxBetInCurrentStage == 0 && !nextPlayer.HasChecked) break;
						//else if (Stage == StageEnum.Preflop 
						//	&& nextPlayer == _bigBlindPlayer 
						//	&& nextPlayer.Bets[(int)Stage] == MaxBetInCurrentStage
						//	&& !nextPlayer.HasChecked) break;
					}
					else if (!checkBet) // Calucating deal index doesn't need to check bet.
					{
						break;
					}
				}

				nextPlayerIndex = (nextPlayerIndex + 1) % _players.Count;
				nextPlayer = _players[nextPlayerIndex];
			}

			return nextPlayerIndex;
		}

		private async Task MoveToStage(StageEnum stage)
		{
			Stage = stage;
			MaxBetInCurrentStage = 0;
			foreach (Player p in Players)
			{
				p.LastActionInCurrentStage = null;
			}

			switch (stage)
			{
				case StageEnum.Flop:
					_deck.Draw(1); // Burn one card
					_sharedCards.AddRange(_deck.Draw(3));
					break;
				case StageEnum.Turn:
					_deck.Draw(1); // Burn one card
					_sharedCards.AddRange(_deck.Draw(1));
					break;
				case StageEnum.River:
					_deck.Draw(1); // Burn one card
					_sharedCards.AddRange(_deck.Draw(1));
					break;
				case StageEnum.End:
					CalculateWinners();
					break;
			}

			await PublishEvent(new GameEvent
			{
				PlayerId = CurrentPlayer.Id,
				EventType = GameEvent.EventTypeEnum.RoundChanged,
				Stage = stage
			});

			if (Stage != StageEnum.End)
			{
				// When a new stage is started, the next player should be recalculated based on the dealer button.
				var nextPlayerIndex = GetNextActivePlayerIndex(DealerIndex);
				await SetCurrentPlayer(nextPlayerIndex);
			}
			else
			{
				await PublishEvent(new GameEvent
				{
					PlayerId = CurrentPlayer.Id,
					EventType = GameEvent.EventTypeEnum.End
				});
				await SetCurrentPlayer(-1);
			}
		}

		private async Task MoveToNextStage()
		{
			if (Stage < StageEnum.Preflop || Stage >= StageEnum.End)
				throw new InvalidOperationException($"Cannot move to the next stage when the current stage is {Stage}.");
			
			await MoveToStage(Stage + 1);
		}

		public async Task OnPlayerActed()
		{
			// Try to find the player who can perform an action.
			var nextPlayerIndex = GetNextActivePlayerIndex(CurrentPlayerIndex);

			// If no more players can act at this stage, try to move to the next stage.
			if (nextPlayerIndex == CurrentPlayerIndex)
			{
				var inGamePlayers = _players.Where(p => p.Cards.Any());
				if (inGamePlayers.Where(p => p.Chips > 0 && !p.HasFolded).Count() >= 2)
				{
					// If there are more than one players who have chips and haven't folded,
					// then move the game to the next stage.
					await MoveToNextStage();
				}
				else if (inGamePlayers.Where(p => p.Chips > 0).Count() <= 1
					|| inGamePlayers.Where(p => !p.HasFolded).Count() == 1)
				{
					// If no one or only one player has chips,
					// or if only one player hasn't folded,
					// the game should end.
					await MoveToStage(StageEnum.End);
				}
				else
				{
					throw new InvalidOperationException("Internal error: unhandled condition after a player's action.");
				}
			}
			else
			{
				await SetCurrentPlayer(nextPlayerIndex);
			}
		}

		private void ValidateBeforePlayerAction(string playerId)
		{
			if (!IsInprocess)
			{
				throw new InvalidOperationException("Game has not stared or has ended.");
			}
			if (CurrentPlayer == null)
			{
				throw new InvalidOperationException("The current player has not been set.");
			}
			if (CurrentPlayer.Id != playerId)
			{
				throw new InvalidOperationException("Only the current player can perform action.");
			}
		}

		private async Task ResetBase()
		{
			await SetCurrentPlayer(-1);

			// Add the audience players to the table before start
			Players.Concat(_audiencePlayers);
			_audiencePlayers.Clear();

			// Reset the game state
			_sharedCards.Clear();
			_winnerIds.Clear();
			_deck.Reset();
			Stage = StageEnum.NotStarted;
			MaxBetInCurrentStage = 0;
			_events.Clear();

			foreach (Player p in _players)
			{
				p.Reset();
			}
		}

		public async Task Reset(string playerId)
		{
			await ResetBase();

			foreach (Player p in _players)
			{
				p.Chips = StartingChips;
			}

			var gameEvent = new GameEvent
			{
				PlayerId = playerId,
				EventType = GameEvent.EventTypeEnum.Reset
			};
			await PublishEvent(gameEvent);
		}

		public async Task Start(string playerId)
		{
			ResetBase();

			if (IsInprocess) 
				throw new InvalidOperationException("Game cannot start when it's in process");

			if (_players.Where(p => p.Chips > 0).Count() < 2)
				throw new InvalidOperationException("Game cannot start with less than 2 players.");

			_deck.Shuffle();

			// Move the dealer position
			DealerIndex = GetNextActivePlayerIndex(DealerIndex, false);

			// Deal the initinal two cards for each player
			foreach (Player p in _players)
			{
				p.Reset();
				if (p.Chips > 0) p.Cards.AddRange(_deck.Draw(2));
			}

			Stage = StageEnum.Preflop;

			// Blind bets
			BlindBets();

			var gameEvent = new GameEvent
			{
				PlayerId = playerId,
				EventType = GameEvent.EventTypeEnum.Start
			};
			await PublishEvent(gameEvent);
			await OnPlayerActed();
		}

		public async Task Call(string playerId)
		{
			await Bet(playerId, 0);
		}

		public async Task Check(string playerId)
		{
			await Bet(playerId, 0);
		}

		public async Task Bet(string playerId, int amount)
		{
			if (amount < 0) throw new InvalidOperationException("Negative bet is not allowed.");

			ValidateBeforePlayerAction(playerId);

			var minBet = MaxBetInCurrentStage - CurrentPlayer.Bets[(int)Stage];
			if (minBet < 0)
			{
				throw new InvalidOperationException($"Internal error: the player {CurrentPlayer.Id} cannot post a negative bet.");
			}

			// Check or call
			if (amount == 0) amount = minBet;

			if (amount < minBet)
			{
				if (CurrentPlayer.Chips >= minBet)
				{
					throw new InvalidOperationException($"The player {CurrentPlayer.Id}'s bet {amount} is less than the minimum bet {minBet}.");
				}
				else
				{
					// Have to all in because the remaining chips are not enough.
					amount = CurrentPlayer.Chips;
				}
			}

			PlayerBet(CurrentPlayerIndex, amount);

			var gameEvent = new GameEvent
			{
				PlayerId = CurrentPlayer.Id,
				Amount = amount,
				EventType = amount == 0 ? GameEvent.EventTypeEnum.Check :
												  (amount > minBet ? GameEvent.EventTypeEnum.Raise : GameEvent.EventTypeEnum.Call)
			};
			CurrentPlayer.LastActionInCurrentStage = gameEvent;
			await PublishEvent(gameEvent);
			await OnPlayerActed();
		}

		public async Task Fold(string playerId)
		{
			ValidateBeforePlayerAction(playerId);

			var activePlayerCount = _players.Where(p => p.Cards.Any() && !p.HasFolded).Count();
			if (activePlayerCount == 1)
			{
				throw new InvalidOperationException($"The last player {CurrentPlayer.Id} cannot fold.");
			}

			PlayerFold(CurrentPlayerIndex);

			var gameEvent = new GameEvent
			{
				PlayerId = CurrentPlayer.Id,
				EventType = GameEvent.EventTypeEnum.Fold
			};
			CurrentPlayer.LastActionInCurrentStage = gameEvent;
			await PublishEvent(gameEvent);

			activePlayerCount = _players.Where(p => p.Cards.Any() && !p.HasFolded).Count();
			if (activePlayerCount == 1)
			{
				await MoveToStage(StageEnum.End);
			}
			else
			{
				await OnPlayerActed();
			}
		}
	}
}
