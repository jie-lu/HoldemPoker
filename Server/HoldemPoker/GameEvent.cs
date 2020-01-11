using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoldemPoker
{
	public class GameEvent
	{
		public enum EventTypeEnum
		{
			Start = 0,
			End,
			Reset,
			Join,
			Leave,
			Call,
			Raise,
			Check,
			Fold,
			Wait,
			RoundChanged,
		}

		public EventTypeEnum EventType { get; set; }

		public string PlayerId { get; set; }

		public int Amount { get; set; }

		public Game.StageEnum Stage { get; set; }
	}
}
