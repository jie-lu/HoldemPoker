using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoldemPoker.Hubs
{
	[Authorize]
	public class GameHub : Hub<IGameClient>
	{
		private readonly Game _game;

		public GameHub(Game game)
		{
			_game = game;
		}

		private async Task Try(Func<Task> action)
		{
			try
			{
				await action.Invoke();
			}
			catch (Exception ex)
			{
				if (!(ex is HubException)) throw new HubException(ex.Message, ex);
				else throw;
			}
		}

		public override async Task OnConnectedAsync()
		{
			await Try(async () =>
			{
				await _game.OnPlayerConnected(Context.UserIdentifier);
			});
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			await Try(async () =>
			{
				await _game.OnPlayerDisconnected(Context.UserIdentifier);
			});
		}

		public async Task StartGame()
		{
			await Try(async () =>
			{
				await _game.Start(Context.UserIdentifier);
			});
		}

		public async Task ResetGame()
		{
			await Try(async () =>
			{
				await _game.Reset(Context.UserIdentifier);
			});
		}

		public async Task Bet(int amount)
		{
			await Try(async () =>
			{
				await _game.Bet(Context.UserIdentifier, amount);
			});
		}

		public async Task Check()
		{
			await Try(async () =>
			{
				await _game.Check(Context.UserIdentifier);
			});
		}

		public async Task Call()
		{
			await Try(async () =>
			{
				await _game.Call(Context.UserIdentifier);
			});
		}

		public async Task Fold()
		{
			await Try(async () =>
			{
				await _game.Fold(Context.UserIdentifier);
			});
		}
	}
}
