using System;
using System.Collections.Generic;
using Xunit;

namespace HoldemPoker.UnitTests
{
	public class PokerHandComparerTests
	{
		PokerHandComparer _target = new PokerHandComparer();

		[Theory]
		[InlineData(new string[] { "2d", "3d", "4d", "5d", "6d" }, PokerHand.FormationEnum.StraightFlush, false,
						new string[] { "Ad", "2d", "3d", "4d", "5d" }, PokerHand.FormationEnum.StraightFlush, true)]
		[InlineData(new string[] { "Qd", "Qc", "3d", "3s", "3h" }, PokerHand.FormationEnum.FullHouse, false,
						new string[] { "2d", "2c", "3d", "3s", "3h" }, PokerHand.FormationEnum.FullHouse, false)]
		[InlineData(new string[] { "2d", "3c", "4d", "5d", "6d" }, PokerHand.FormationEnum.Straight, false,
						new string[] { "Ad", "2d", "3d", "4s", "5c" }, PokerHand.FormationEnum.Straight, true)]
		[InlineData(new string[] { "2d", "3d", "6c", "7d", "10d" }, PokerHand.FormationEnum.HighCard, false,
						new string[] { "2d", "3d", "5c", "7d", "10d" }, PokerHand.FormationEnum.HighCard, false)]
		[InlineData(new string[] { "Ad", "Ac", "Kd", "Kc", "6d" }, PokerHand.FormationEnum.TwoPairs, false,
						new string[] { "Ad", "Ad", "Ks", "Kh", "5d" }, PokerHand.FormationEnum.TwoPairs, false)]
		[InlineData(new string[] { "Ks", "Kc", "10d", "4c", "Qd" }, PokerHand.FormationEnum.Pair, false,
						new string[] { "Kd", "Kh", "9c", "2h", "5d" }, PokerHand.FormationEnum.Pair, false)]
		[InlineData(new string[] { "2d", "3d", "7d", "8d", "Jd" }, PokerHand.FormationEnum.Flush, false,
						new string[] { "2c", "3c", "4c", "7c", "Jc" }, PokerHand.FormationEnum.Flush, false)]
		public void Compare_Two_Hands_Same_Formations(string[] xCards, PokerHand.FormationEnum xFormation, bool xIsAceToFive,
												string[] yCards, PokerHand.FormationEnum yFormation, bool yIsAceToFive)
		{
			var x = new PokerHand();
			foreach (string c in xCards) x.Cards.Add(c);
			x.Formation = xFormation;
			x.IsAceToFive = xIsAceToFive;

			var y = new PokerHand();
			foreach (string c in yCards) y.Cards.Add(c);
			y.Formation = yFormation;
			y.IsAceToFive = yIsAceToFive;

			var ret = _target.Compare(x, y);

			Assert.True(ret > 0);
		}

		[Theory]
		[InlineData(new string[] { "2d", "3d", "4d", "5d", "6d" }, PokerHand.FormationEnum.StraightFlush, false,
						new string[] { "Ad", "2d", "3d", "4d", "5d" }, PokerHand.FormationEnum.FourOfAKind, false)]
		[InlineData(new string[] { "Qd", "Qc", "3d", "3s", "3h" }, PokerHand.FormationEnum.FourOfAKind, false,
						new string[] { "2d", "2c", "3d", "3s", "3h" }, PokerHand.FormationEnum.FullHouse, false)]
		[InlineData(new string[] { "2d", "3c", "4d", "5d", "6d" }, PokerHand.FormationEnum.FullHouse, false,
						new string[] { "Ad", "2d", "3d", "4s", "5c" }, PokerHand.FormationEnum.ThreeOfAKind, false)]
		[InlineData(new string[] { "2d", "3d", "6c", "7d", "10d" }, PokerHand.FormationEnum.Flush, false,
						new string[] { "2d", "3d", "5c", "7d", "10d" }, PokerHand.FormationEnum.ThreeOfAKind, false)]
		[InlineData(new string[] { "2d", "2c", "4d", "4c", "6d" }, PokerHand.FormationEnum.ThreeOfAKind, false,
						new string[] { "Ad", "Ad", "2s", "2h", "5d" }, PokerHand.FormationEnum.TwoPairs, false)]
		[InlineData(new string[] { "2d", "2c", "4d", "4c", "6d" }, PokerHand.FormationEnum.TwoPairs, false,
						new string[] { "Ad", "Ad", "2s", "2h", "5d" }, PokerHand.FormationEnum.Pair, false)]
		[InlineData(new string[] { "2d", "2c", "4d", "4c", "6d" }, PokerHand.FormationEnum.Pair, false,
						new string[] { "Ad", "Ad", "2s", "2h", "5d" }, PokerHand.FormationEnum.HighCard, false)]
		public void Compare_Two_Hands_Different_Formations(string[] xCards, PokerHand.FormationEnum xFormation, bool xIsAceToFive,
												string[] yCards, PokerHand.FormationEnum yFormation, bool yIsAceToFive)
		{
			var x = new PokerHand();
			foreach (string c in xCards) x.Cards.Add(c);
			x.Formation = xFormation;
			x.IsAceToFive = xIsAceToFive;

			var y = new PokerHand();
			foreach (string c in yCards) y.Cards.Add(c);
			y.Formation = yFormation;
			y.IsAceToFive = yIsAceToFive;

			var ret = _target.Compare(x, y);

			Assert.True(ret > 0);
		}
	}
}
