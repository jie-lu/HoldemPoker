using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoldemPoker
{
	public class Deck
	{
		private readonly Random _random = new Random();
		private readonly string[] _cards;
		private int _cardTop = 0;

		public Deck()
		{
			var ranks = Enumerable.Range((int)PlayingCard.RankEnum.Deuce, 13);
			_cards = ranks.Select(r => $"{r}s") // Spades
				.Union(ranks.Select(r => $"{r}h")) // Hearts
				.Union(ranks.Select(r => $"{r}c")) // Clubs
				.Union(ranks.Select(r => $"{r}d")) // Diamonds
				.ToArray();
		}

		public void Reset()
		{
			_cardTop = 0;
		}

		// http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
		public void Shuffle()
		{
			for (int n = _cards.Length - 1; n > 0; --n)
			{
				var k = _random.Next(n + 1);
				var temp = _cards[n];
				_cards[n] = _cards[k];
				_cards[k] = temp;
			}
		}

		public string[] Draw(int count)
		{
			if (_cardTop >= _cards.Length) return new string[0];

			var ret = _cards.Skip(_cardTop).Take(count).ToArray();
			_cardTop += count;

			return ret;
		}
	}
}
