using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoldemPoker
{
	public class PokerHandComparer : IComparer<PokerHand>
	{
		private static int CompareStraight(PokerHand x, PokerHand y)
		{
			if (x.IsAceToFive && y.IsAceToFive) return 0;
			else if (x.IsAceToFive && !y.IsAceToFive) return -1;
			else if (!x.IsAceToFive && y.IsAceToFive) return 1;
			else return x.Cards.Max(c => c.Rank) - y.Cards.Max(c => c.Rank);
		}

		private static int CompareHighCard(PokerHand x, PokerHand y)
		{
			for (int i = x.Cards.Count - 1; i >= 0; i--)
			{
				var diff = x.Cards[i].Rank - y.Cards[i].Rank;

				if (diff == 0) continue;
				else return diff;
			}

			return 0;
		}

		private static int CompareFullHouse(PokerHand x, PokerHand y)
		{
			return CompareRankGroups(x, y, 2);
		}

		private static int CompareRankGroups(PokerHand x, PokerHand y, int groupCount)
		{
			var xRankGroups = x.Cards.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ThenByDescending(g => g.Key);
			var yRankGroups = y.Cards.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ThenByDescending(g => g.Key);
			int diff = 0;
			for (int i = 0; i < groupCount; i++)
			{
				diff = xRankGroups.ElementAt(i).Key - yRankGroups.ElementAt(i).Key;

				if (diff != 0) return diff;
			}

			return diff;
		}

		private static int CompareThreeOfAKind(PokerHand x, PokerHand y)
		{
			return CompareRankGroups(x, y, 3);
		}

		private static int CompareFourOfAKind(PokerHand x, PokerHand y)
		{
			return CompareRankGroups(x, y, 2);
		}

		private static int CompareTwoPairs(PokerHand x, PokerHand y)
		{
			return CompareRankGroups(x, y, 3);
		}

		private static int ComparePair(PokerHand x, PokerHand y)
		{
			return CompareRankGroups(x, y, 4);
		}

		private static Dictionary<PokerHand.FormationEnum, Func<PokerHand, PokerHand, int>> _comparers
			= new Dictionary<PokerHand.FormationEnum, Func<PokerHand, PokerHand, int>>()
			{
				{ PokerHand.FormationEnum.StraightFlush, CompareStraight },
				{ PokerHand.FormationEnum.Straight, CompareStraight },
				{ PokerHand.FormationEnum.FourOfAKind, CompareFourOfAKind },
				{ PokerHand.FormationEnum.FullHouse, CompareFullHouse },
				{ PokerHand.FormationEnum.Flush, CompareHighCard },
				{ PokerHand.FormationEnum.ThreeOfAKind, CompareThreeOfAKind },
				{ PokerHand.FormationEnum.TwoPairs, CompareTwoPairs },
				{ PokerHand.FormationEnum.Pair, ComparePair },
				{ PokerHand.FormationEnum.HighCard, CompareHighCard }
			};

		public int Compare(PokerHand x, PokerHand y)
		{
			if (x.Formation == y.Formation)
			{
				return _comparers[x.Formation](x, y);
			}
			else
			{
				return x.Formation - y.Formation;
			}
		}
	}
}
