using System;

namespace TrueCraft.Server.Exceptions
{
	public class PlayerDisconnectException : Exception
	{
		public PlayerDisconnectException(bool playerInitiated) => PlayerInitiated = playerInitiated;

		/// <summary>
		///  True if the disconnection was the result of player actions.
		/// </summary>
		public bool PlayerInitiated { get; set; }
	}
}