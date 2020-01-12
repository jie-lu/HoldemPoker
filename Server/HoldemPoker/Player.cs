using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoldemPoker
{
	public class Player
	{
		public string Id { get; set; }

		public bool IsConnected { get; set; }

		public bool HasFolded { get; set; }

		public int Chips { get; set; }

		public PokerHand BestHand { get; set; }

		/// <summary>
		/// Index	Round
		/// 0			Pre-flop
		/// 1			Flop
		/// 2			Turn
		/// 3			River
		/// </summary>
		public int[] Bets { get; private set; }

		public List<string> Cards { get; private set; }

		public int CountDown { get; set; }

		public GameEvent LastActionInCurrentStage { get; set; }

		public Player()
		{
			Reset();
		}

		public void Reset()
		{
			this.Bets = new int[] { 0, 0, 0, 0 };
			this.Cards = new List<string>();
			this.HasFolded = false;
			this.BestHand = null;
			this.LastActionInCurrentStage = null;
		}
	}
}
