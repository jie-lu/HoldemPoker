using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoldemPoker
{
	public class GameState
	{
		public List<Player> Players { get; set; }
		public int DealerIndex { get; set; }
		public int CurrentPlayerIndex { get; set; }
		public List<string> SharedCards { get; set; }
		public int Stage { get; set; }
		public int MaxBetAtThisStage { get; set; }
		public List<string> WinnerIds { get; set; }
		public GameEvent GameEvent { get; set; }
		public int Blind { get; set; }

		public GameState(GameEvent gameEvent = null)
		{
			GameEvent = gameEvent;
			Players = new List<Player>();
			SharedCards = new List<string>();
			WinnerIds = new List<string>();
		}
	}
}
