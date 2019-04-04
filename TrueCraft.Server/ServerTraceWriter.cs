using System;
using System.Diagnostics;
using System.Text;

namespace TrueCraft.Server
{
	internal class ServerTraceWriter : TraceListener
	{
		#region Overrides 

		public override void Write(string message)
		{
			WriteLine(message);
		}

		public override void Write(object o)
		{
			WriteLine(Convert.ToString(o));
		}

		public override void Write(object o, string category)
		{
			WriteLine(Convert.ToString(o), category);
		}

		public override void Write(string message, string category)
		{
			WriteLine(message, category);
		}

		public override void WriteLine(string message)
		{
			WriteLine(message, null);
		}

		public override void WriteLine(object o)
		{
			WriteLine(Convert.ToString(o), null);
		}

		public override void WriteLine(object o, string category)
		{
			base.WriteLine(Convert.ToString(o), category);
		}

		public override void WriteLine(string message, string category)
		{
			if (string.IsNullOrEmpty(message))
				return;

			var value = $"{GetTimestamp()} [{category}] {string.Format(message)}";

			var foreground = Console.ForegroundColor;
			Console.ForegroundColor = GetConsoleColor(category);
			Console.WriteLine(value);
			Console.ForegroundColor = foreground;
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			if ((Filter != null) && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
				return;
			var message = data.ToString();
			WriteLine(message, eventType.ToString());
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
		{
			if ((Filter != null) && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
				return;

			var stringBuilder = new StringBuilder();
			for (var i = 0; i < data.Length; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(", ");
				}
				if (data[i] != null)
				{
					stringBuilder.Append(data[i]);
				}
			}

			WriteLine(stringBuilder.ToString(), eventType.ToString());
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			WriteLine(message, eventType.ToString());
		}

		#endregion

		private static string GetTimestamp(bool utc = true, string timeFormat = "yyyy-MM-dd H:mm:ss")
		{
			return (utc ? DateTime.UtcNow : DateTime.Now).ToString(timeFormat);
		}

		private static ConsoleColor GetConsoleColor(string category)
		{
			if (Enum.TryParse(category, true, out TraceEventType type))
			{
				switch (type)
				{
					case TraceEventType.Critical:
						return ConsoleColor.Red;
					case TraceEventType.Error:
						return ConsoleColor.DarkRed;
					case TraceEventType.Information:
						return ConsoleColor.White;
					case TraceEventType.Verbose:
						return ConsoleColor.Cyan;
					case TraceEventType.Warning:
						return ConsoleColor.Yellow;

					case TraceEventType.Resume:
					case TraceEventType.Start:
					case TraceEventType.Stop:
					case TraceEventType.Suspend:
					case TraceEventType.Transfer:
					default:
						return ConsoleColor.Gray;
				}
			}

			return ConsoleColor.Gray;
		}
	}
}
