using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HoldemPoker
{
	public class PokerHandEqualityComparer : IEqualityComparer<PokerHand>
	{
		private PokerHandComparer _comparer = new PokerHandComparer();

		public bool Equals([AllowNull] PokerHand x, [AllowNull] PokerHand y)
		{
			return _comparer.Compare(x, y) == 0;
		}

		public int GetHashCode([DisallowNull] PokerHand obj)
		{
			return base.GetHashCode();
		}
	}
}
