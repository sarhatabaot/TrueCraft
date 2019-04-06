using System;

namespace VisualTests
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var game = new VisualTestGame(args))
			{
				game.Run();
			}
		}
	}
}