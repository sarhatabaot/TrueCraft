using System;

namespace TrueCraft.Server.Host
{
	public class Program
	{
		public static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
			{
				if (Environment.UserInteractive)
				{
					Console.WriteLine("Press any key to quit.");
					Console.ReadKey();
				}
			};

			Bootstrap.Start(args);
		}
	}
}