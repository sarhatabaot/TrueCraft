using System;
using System.Collections.Generic;

namespace TrueCraft.Server
{
	public interface IEventScheduler
	{
		HashSet<string> DisabledEvents { get; }
		void ScheduleEvent(string name, IEventSubject subject, TimeSpan when, Action<IMultiPlayerServer> action, string source = null);
		void Update();
	}
}