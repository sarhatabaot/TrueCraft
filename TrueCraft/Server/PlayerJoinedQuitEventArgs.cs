using System;
using TrueCraft.Networking;

namespace TrueCraft.Server
{
	public class PlayerJoinedQuitEventArgs : EventArgs
	{
		public PlayerJoinedQuitEventArgs(IRemoteClient client) => Client = client;

		public IRemoteClient Client { get; set; }
	}
}