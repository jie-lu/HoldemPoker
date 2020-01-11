using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoldemPoker.Hubs
{
	public interface IGameClient
	{
		Task Update(GameState game);
	}
}
