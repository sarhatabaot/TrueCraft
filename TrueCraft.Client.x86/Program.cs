using System;

namespace TrueCraft.Client
{
	public static class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Bootstrap.Start(args);
		}
	}
}