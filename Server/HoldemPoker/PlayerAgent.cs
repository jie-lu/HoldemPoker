using HoldemPoker.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace HoldemPoker
{
	/// <summary>
	/// Make moves for offline players.
	/// </summary>
	public class PlayerAgent : IDisposable
	{
		private readonly Game _game;
		private IHubContext<GameHub, IGameClient> _gameHubContext;
		private Timer _timer = new Timer();

		public PlayerAgent(Game game, IHubContext<GameHub, IGameClient> gameHubContext)
		{
			_game = game;
			_game.CurrentPlayerChanged += OnCurrentPlayerChanged;

			_gameHubContext = gameHubContext;

			_timer.Interval = game.PlayerTimeout * 1000;
			_timer.AutoReset = false;
			_timer.Elapsed += OnPlayerActionTimeout;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_timer.Dispose();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~PlayerAgent()
		{
			Dispose(false);
		}

		private async Task OnCurrentPlayerChanged(int lastPlayerIndex)
		{
			_timer.Stop();

			if (lastPlayerIndex >= 0)
			{
				_game.Players[lastPlayerIndex].CountDown = 0;
			}

			if (_game.CurrentPlayer != null)
			{
				if (_game.CurrentPlayer.IsConnected)
				{
					// Start timing if the current player is connected.
					_timer.Start();
				}
				else
				{
					// Otherwise, a disconnected player will call every bet.
					await _game.Call(_game.CurrentPlayer.Id);
				}
			}
		}

		private async void OnPlayerActionTimeout(object sender, ElapsedEventArgs e)
		{
			// If a connected player didn't do anything within a timeout, call be bet automatically.
			await _game.Call(_game.CurrentPlayer.Id);
		}
	}
}
