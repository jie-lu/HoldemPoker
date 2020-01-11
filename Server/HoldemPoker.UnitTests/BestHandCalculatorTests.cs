using System;
using System.Collections.Generic;
using Xunit;

namespace HoldemPoker.UnitTests
{
	public class BestHandCalculatorTests
	{
		BestHandCalculator _target = new BestHandCalculator();

		[Fact]
		public void Detect_Straight()
		{
			var cards = new List<PlayingCard>
			{
				"2d", "3d", "Ac", "4d", "3h", "5s", "6d"
			};
			var bestHand = _target.CalculateBestHand(cards);

			Assert.True(bestHand.Formation == PokerHand.FormationEnum.Straight);
		}

		[Fact]
		public void Detect_FlushStraight()
		{
			var cards = new List<PlayingCard>
			{
				"2d", "3d", "3c", "4d", "3h", "5d", "6d"
			};
			var bestHand = _target.CalculateBestHand(cards);

			Assert.True(bestHand.Formation == PokerHand.FormationEnum.StraightFlush);
		}

		[Fact]
		public void Detect_FourOfAKind()
		{
			var cards = new List<PlayingCard>
			{
				"2d", "3d", "3c", "3s", "3h", "5d", "6d"
			};
			var bestHand = _target.CalculateBestHand(cards);

			Assert.True(bestHand.Formation == PokerHand.FormationEnum.FourOfAKind);
		}

		[Fact]
		public void Detect_FullHouse()
		{
			var cards = new List<PlayingCard>
			{
				"2d", "3d", "3c", "3s", "Ah", "5d", "Ad"
			};
			var bestHand = _target.CalculateBestHand(cards);

			Assert.True(bestHand.Formation == PokerHand.FormationEnum.FullHouse);
		}

		[Fact]
		public void Detect_ThreeOfAKind()
		{
			var cards = new List<PlayingCard>
			{
				"2d", "3d", "3c", "3s", "Ah", "5d", "Qd"
			};
			var bestHand = _target.CalculateBestHand(cards);

			Assert.True(bestHand.Formation == PokerHand.FormationEnum.ThreeOfAKind);
		}

		[Fact]
		public void Detect_AceToFive()
		{
			var cards = new List<PlayingCard>
			{
				"2d", "3d", "4c", "3s", "Ah", "5d", "Qd"
			};
			var bestHand = _target.CalculateBestHand(cards);

			Assert.True(bestHand.Formation == PokerHand.FormationEnum.Straight);
		}

		[Fact]
		public void Detect_TwoPairs()
		{
			var cards = new List<PlayingCard>
			{
				"2d", "3d", "4c", "3s", "Kh", "5d", "5h"
			};
			var bestHand = _target.CalculateBestHand(cards);

			Assert.True(bestHand.Formation == PokerHand.FormationEnum.TwoPairs);
		}

		[Fact]
		public void Detect_Pair()
		{
			var cards = new List<PlayingCard>
			{
				"2d", "3d", "4c", "3s", "Kh", "Jd", "5h"
			};
			var bestHand = _target.CalculateBestHand(cards);

			Assert.True(bestHand.Formation == PokerHand.FormationEnum.Pair);
		}

		[Fact]
		public void Detect_HighCard()
		{
			var cards = new List<PlayingCard>
			{
				"2d", "3d", "4c", "9s", "Kh", "Jd", "5h"
			};
			var bestHand = _target.CalculateBestHand(cards);

			Assert.True(bestHand.Formation == PokerHand.FormationEnum.HighCard);
		}

		[Fact]
		public void Detect_ThreeOfAKind2()
		{
			var cards = new List<PlayingCard>
			{
				"6h", "Jh", "8c", "Ad", "8h", "8s", "10d"
			};
			var bestHand = _target.CalculateBestHand(cards);

			Assert.True(bestHand.Formation == PokerHand.FormationEnum.ThreeOfAKind);
		}
	}
}
