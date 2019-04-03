using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using TrueCraft.Profiling;

namespace TrueCraft.Server
{
	public class EventScheduler : IEventScheduler
	{
		private readonly object EventLock = new object();

		public EventScheduler(IMultiplayerServer server)
		{
			Events = new List<ScheduledEvent>();
			ImmediateEventQueue = new ConcurrentQueue<ScheduledEvent>();
			LaterEventQueue = new ConcurrentQueue<ScheduledEvent>();
			DisposedSubjects = new ConcurrentQueue<IEventSubject>();
			Server = server;
			Subjects = new HashSet<IEventSubject>();
			Stopwatch = new Stopwatch();
			DisabledEvents = new HashSet<string>();
			Stopwatch.Start();
		}

		private IList<ScheduledEvent> Events { get; } // Sorted
		private IMultiplayerServer Server { get; }
		private HashSet<IEventSubject> Subjects { get; }
		private Stopwatch Stopwatch { get; }
		private ConcurrentQueue<ScheduledEvent> ImmediateEventQueue { get; }
		private ConcurrentQueue<ScheduledEvent> LaterEventQueue { get; }
		private ConcurrentQueue<IEventSubject> DisposedSubjects { get; }
		public HashSet<string> DisabledEvents { get; }

		public void ScheduleEvent(string name, IEventSubject subject, TimeSpan when, Action<IMultiplayerServer> action)
		{
			if (DisabledEvents.Contains(name))
				return;
			var _when = Stopwatch.ElapsedTicks + when.Ticks;
			if (subject != null && !Subjects.Contains(subject))
			{
				Subjects.Add(subject);
				subject.Disposed += Subject_Disposed;
			}

			var queue = when.TotalSeconds > 3 ? LaterEventQueue : ImmediateEventQueue;
			queue.Enqueue(new ScheduledEvent
			{
				Name = name,
				Subject = subject,
				When = _when,
				Action = action
			});
		}

		public void Update()
		{
			Profiler.Start("scheduler");
			Profiler.Start("scheduler.receive-events");
			var start = Stopwatch.ElapsedTicks;
			var limit = Stopwatch.ElapsedMilliseconds + 10;
			while (ImmediateEventQueue.Count > 0 && Stopwatch.ElapsedMilliseconds < limit)
			{
				ScheduledEvent e;
				var dequeued = false;
				while (!(dequeued = ImmediateEventQueue.TryDequeue(out e))
				       && Stopwatch.ElapsedMilliseconds < limit) ;
				if (dequeued)
					ScheduleEvent(e);
			}

			while (LaterEventQueue.Count > 0 && Stopwatch.ElapsedMilliseconds < limit)
			{
				ScheduledEvent e;
				var dequeued = false;
				while (!(dequeued = LaterEventQueue.TryDequeue(out e))
				       && Stopwatch.ElapsedMilliseconds < limit) ;
				if (dequeued)
					ScheduleEvent(e);
			}

			Profiler.Done();
			Profiler.Start("scheduler.dispose-subjects");
			while (DisposedSubjects.Count > 0 && Stopwatch.ElapsedMilliseconds < limit)
			{
				IEventSubject subject;
				var dequeued = false;
				while (!(dequeued = DisposedSubjects.TryDequeue(out subject))
				       && Stopwatch.ElapsedMilliseconds < limit) ;
				if (dequeued)
				{
					// Cancel all events with this subject
					for (var i = 0; i < Events.Count; i++)
						if (Events[i].Subject == subject)
						{
							Events.RemoveAt(i);
							i--;
						}

					Subjects.Remove(subject);
				}
			}

			limit = Stopwatch.ElapsedMilliseconds + 10;
			Profiler.Done();
			for (var i = 0; i < Events.Count && Stopwatch.ElapsedMilliseconds < limit; i++)
			{
				var e = Events[i];
				if (e.When <= start)
				{
					Profiler.Start("scheduler." + e.Name);
					e.Action(Server);
					Events.RemoveAt(i);
					i--;
					Profiler.Done();
				}

				if (e.When > start)
					break; // List is sorted, we can exit early
			}

			Profiler.Done(20);
		}

		private void ScheduleEvent(ScheduledEvent e)
		{
			int i;
			for (i = 0; i < Events.Count; i++)
				if (Events[i].When > e.When)
					break;
			Events.Insert(i, e);
		}

		private void Subject_Disposed(object sender, EventArgs e)
		{
			DisposedSubjects.Enqueue((IEventSubject) sender);
		}

		private struct ScheduledEvent
		{
			public long When;
			public Action<IMultiplayerServer> Action;
			public IEventSubject Subject;
			public string Name;
		}
	}
}