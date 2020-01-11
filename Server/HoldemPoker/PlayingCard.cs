using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoldemPoker
{
	public class PlayingCard
	{
		public enum SuitEnum
		{
			Spades = 0,
			Hearts,
			Clubs,
			Diamonds
		}

		public enum RankEnum
		{
			Deuce = 2,
			Trey,
			Four,
			Five,
			Six,
			Seven,
			Eight,
			Nine,
			Ten,
			Jack,
			Queen,
			King,
			Ace
		}

		static readonly Dictionary<string, int> _rankValues = new Dictionary<string, int>()
		{
			{ "2", 2 },
			{ "3", 3 },
			{ "4", 4 },
			{ "5", 5 },
			{ "6", 6 },
			{ "7", 7 },
			{ "8", 8 },
			{ "9", 9 },
			{ "10", 10 },
			{ "11", 11 },
			{ "12", 12 },
			{ "13", 13 },
			{ "14", 14 },
			{ "J", 11 },
			{ "Q", 12 },
			{ "K", 13 },
			{ "A", 14 }
		};

		static readonly Dictionary<char, SuitEnum> _suitValues = new Dictionary<char, SuitEnum>()
		{
			{ 's', SuitEnum.Spades },
			{ 'h', SuitEnum.Hearts },
			{ 'c', SuitEnum.Clubs },
			{ 'd', SuitEnum.Diamonds },
		};

		public PlayingCard(int rank, SuitEnum suit)
		{
			Rank = rank;
			Suit = suit;
		}

		public PlayingCard(string cardStr)
		{
			if (cardStr == null) throw new ArgumentNullException("cardStr");
			if (cardStr.Length < 2) throw new ArgumentException("'cardStr' cannot be less than 2", "cardStr");

			Suit = _suitValues[cardStr.Last()];
			Rank = _rankValues[cardStr.Substring(0, cardStr.Length - 1)];
		}

		public PlayingCard()
		{ 
		}

		public SuitEnum Suit { get; set; }

		public int Rank { get; set; }

		public static implicit operator PlayingCard(string cardStr) => new PlayingCard(cardStr);

		public override string ToString()
		{
			return $"{Rank} of {Suit}";
		}
	}
}
