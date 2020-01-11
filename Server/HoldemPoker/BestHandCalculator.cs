using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HoldemPoker
{
	public class BestHandCalculator
	{
		private IDictionary<int, List<int>> FindConsecutiveSequences(IEnumerable<int> numbers)
		{
			var consecutiveSequences = new Dictionary<int, List<int>>(); // key: start number, value: card ranks
			int sequenceStart = -1;
			int lastNumber = -1;
			foreach (int n in numbers)
			{
				if (sequenceStart == -1)
				{
					sequenceStart = n;
					lastNumber = n;
					continue;
				}

				if (n - lastNumber == 1)
				{
					if (consecutiveSequences.ContainsKey(sequenceStart))
					{
						consecutiveSequences[sequenceStart].Add(n);
					}
					else
					{
						var consecutiveNumbers = new List<int>();
						consecutiveNumbers.Add(lastNumber);
						consecutiveNumbers.Add(n);
						consecutiveSequences.Add(sequenceStart, consecutiveNumbers);
					}
				}
				else
				{
					if (!consecutiveSequences.ContainsKey(sequenceStart))
					{
						consecutiveSequences.Add(sequenceStart, new List<int> { sequenceStart });
					}
					sequenceStart = n;
				}

				lastNumber = n;
			}

			return consecutiveSequences;
		}

		private PokerHand GetFourOfAKind(IEnumerable<IGrouping<int, PlayingCard>> orderedCardRankGroups)
		{
			var firstGroup = orderedCardRankGroups.ElementAt(0);
			if (firstGroup.Count() >= 4)
			{
				var bestHand = new PokerHand();
				bestHand.Cards.AddRange(firstGroup.Take(4));
				bestHand.Cards.AddRange(
					orderedCardRankGroups.Where(g => g.Key != firstGroup.Key)
					.OrderByDescending(g => g.Key).First());
				bestHand.Formation = PokerHand.FormationEnum.FourOfAKind;

				return bestHand;
			}

			return null;
		}

		private PokerHand GetFullHouse(IEnumerable<IGrouping<int, PlayingCard>> orderedCardRankGroups)
		{
			var firstGroup = orderedCardRankGroups.ElementAt(0);
			if (firstGroup.Count() >= 3)
			{
				var secondGroup = orderedCardRankGroups.ElementAt(1);
				if (secondGroup.Count() >= 2)
				{
					var bestHand = new PokerHand();
					bestHand.Cards.AddRange(firstGroup.Take(3));
					bestHand.Cards.AddRange(secondGroup.Take(2));
					bestHand.Formation = PokerHand.FormationEnum.FullHouse;

					return bestHand;
				}
			}

			return null;
		}

		private PokerHand GetStraight<TSource>(IEnumerable<TSource> cardSource, 
			Func<TSource, int> rankSelector, Func<TSource, PlayingCard> cardSelector, bool isFlush)
		{
			var orderedCardRanks = cardSource.Select(rankSelector).OrderBy(r => r);
			var longestSequence = FindConsecutiveSequences(orderedCardRanks)
				.OrderByDescending(s => s.Value.Count).First().Value;
			if (longestSequence.Count >= 5)
			{
				var bestHand = new PokerHand();
				bestHand.Cards.AddRange(
					cardSource.Where(g => longestSequence.Contains(rankSelector(g)))
						.OrderBy(rankSelector).Select(cardSelector).Take(5));
				bestHand.Formation = 
					isFlush ? PokerHand.FormationEnum.StraightFlush : PokerHand.FormationEnum.Straight;

				return bestHand;
			}
			else if (cardSource.Where(c => rankSelector(c) == (int)PlayingCard.RankEnum.Ace).Any())
			{
				// Because A's rank is 14 so we need to detect the straight of 1 to 5 sepearately.
				var aceToFiveRanks = new int[] { (int)PlayingCard.RankEnum.Ace, 2, 3, 4, 5 };
				var aceToFive = cardSource.Where(c => aceToFiveRanks.Contains(rankSelector(c)))
					.OrderBy(rankSelector).Select(cardSelector);
				if (aceToFive.Count() == 5)
				{
					var bestHand = new PokerHand();
					bestHand.Cards.AddRange(aceToFive);
					bestHand.Formation =
						isFlush ? PokerHand.FormationEnum.StraightFlush : PokerHand.FormationEnum.Straight;
					bestHand.IsAceToFive = true;

					return bestHand;
				}
			}

			return null;
		}

		private PokerHand GetThreeOfAKind(IEnumerable<IGrouping<int, PlayingCard>> orderedCardRankGroups)
		{
			var firstGroup = orderedCardRankGroups.ElementAt(0);
			if (firstGroup.Count() >= 3)
			{
				var bestHand = new PokerHand();
				bestHand.Cards.AddRange(firstGroup);
				bestHand.Cards.Add(orderedCardRankGroups.ElementAt(1).First());
				bestHand.Cards.Add(orderedCardRankGroups.ElementAt(2).First());
				bestHand.Formation = PokerHand.FormationEnum.ThreeOfAKind;
				return bestHand;
			}

			return null;
		}

		private PokerHand GetTwoPairs(IEnumerable<IGrouping<int, PlayingCard>> orderedCardRankGroups)
		{
			var firstGroup = orderedCardRankGroups.ElementAt(0);
			if (firstGroup.Count() >= 2)
			{
				var secondGroup = orderedCardRankGroups.ElementAt(1);
				if (secondGroup.Count() >= 2)
				{
					var bestHand = new PokerHand();
					bestHand.Cards.AddRange(firstGroup.Take(2));
					bestHand.Cards.AddRange(secondGroup.Take(2));
					bestHand.Cards.Add(
						orderedCardRankGroups.Where(g => g.Key != firstGroup.Key && g.Key != secondGroup.Key)
						.OrderByDescending(g => g.Key).First().First());
					bestHand.Formation = PokerHand.FormationEnum.TwoPairs;

					return bestHand;
				}
			}

			return null;
		}

		private PokerHand GetPair(IEnumerable<IGrouping<int, PlayingCard>> orderedCardRankGroups)
		{
			var firstGroup = orderedCardRankGroups.ElementAt(0);
			if (firstGroup.Count() >= 2)
			{
				var bestHand = new PokerHand();
				bestHand.Cards.AddRange(firstGroup.Take(2));
				bestHand.Cards.AddRange(
						orderedCardRankGroups.Where(g => g.Key != firstGroup.Key)
						.OrderByDescending(g => g.Key).Select(g => g.First()).Take(3));
				bestHand.Formation = PokerHand.FormationEnum.Pair;

				return bestHand;
			}

			return null;
		}

		private PokerHand GetHighCard(IEnumerable<IGrouping<int, PlayingCard>> orderedCardRankGroups)
		{
			var bestHand = new PokerHand();
			bestHand.Cards.AddRange(
				orderedCardRankGroups.OrderByDescending(g => g.Key).Select(g => g.First()).Take(5));
			bestHand.Formation = PokerHand.FormationEnum.HighCard;

			return bestHand;
		}

		private PokerHand GetFlushOrFlushStraight(IEnumerable<PlayingCard> cards)
		{
			var cardSuitGroups = cards.GroupBy(c => c.Suit);
			var flush = cardSuitGroups.Where(g => g.Count() >= 5).FirstOrDefault();
			if (flush != null)
			{
				// When a flush exists, the only better hand is a straight flush or royal flush,
				// because in 7 cards, a flush cannot coexist with a full house or four of a kind.
				var bestHand = GetStraight(flush, c => c.Rank, c => c, true);

				if (bestHand == null)
				{
					bestHand = new PokerHand();
					bestHand.Cards.AddRange(flush.OrderByDescending(c => c.Rank).Take(5));
					bestHand.Formation = PokerHand.FormationEnum.Flush;
				}

				return bestHand;
			}

			return null;
		}

		public PokerHand CalculateBestHand(IEnumerable<string> cards)
		{
			return CalculateBestHand(cards.Select(c => new PlayingCard(c)));
		}

		public PokerHand CalculateBestHand(IEnumerable<PlayingCard> cards)
		{
			if (cards.Count() < 5) throw new InvalidOperationException("The number of cards must be equal to or greater than 5.");

			var bestHand = GetFlushOrFlushStraight(cards);
			if(bestHand == null)
			{
				var orderedCardRankGroups = cards.GroupBy(c => c.Rank)
															.OrderByDescending(g => g.Count())
															.ThenByDescending(g => g.Key);
				bestHand = GetFourOfAKind(orderedCardRankGroups);
				if (bestHand == null) bestHand = GetFullHouse(orderedCardRankGroups);
				if (bestHand == null) bestHand = GetStraight(orderedCardRankGroups, g => g.Key, g => g.First(), false);
				if (bestHand == null) bestHand = GetThreeOfAKind(orderedCardRankGroups);
				if (bestHand == null) bestHand = GetTwoPairs(orderedCardRankGroups);
				if (bestHand == null) bestHand = GetPair(orderedCardRankGroups);
				if (bestHand == null) bestHand = GetHighCard(orderedCardRankGroups);
			}

			return bestHand;
		}
	}
}
