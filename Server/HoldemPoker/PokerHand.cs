using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HoldemPoker
{
	public class PokerHand
	{
		public enum FormationEnum
		{
			HighCard = 0,
			Pair,
			TwoPairs,
			ThreeOfAKind,
			Straight,
			Flush,
			FullHouse,
			FourOfAKind,
			StraightFlush,
			RoyalFlush
		}

		public List<PlayingCard> Cards { get; set; }

		public FormationEnum Formation { get; set; }

		public bool IsAceToFive { get; set; }

		public PokerHand()
		{
			Cards = new List<PlayingCard>();
			IsAceToFive = false;
		}
	}
}
