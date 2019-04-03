﻿using System.Collections.Generic;
using System.Net.Sockets;

namespace TrueCraft.Networking
{
	public class BufferManager
	{
		private readonly Stack<int> availableBuffers;
		private readonly object bufferLocker = new object();

		private readonly List<byte[]> buffers;

		private readonly int bufferSize;

		public BufferManager(int bufferSize)
		{
			this.bufferSize = bufferSize;
			buffers = new List<byte[]>();
			availableBuffers = new Stack<int>();
		}

		public void SetBuffer(SocketAsyncEventArgs args)
		{
			if (availableBuffers.Count > 0)
			{
				var index = availableBuffers.Pop();

				byte[] buffer;
				lock (bufferLocker) buffer = buffers[index];

				args.SetBuffer(buffer, 0, buffer.Length);
			}
			else
			{
				var buffer = new byte[bufferSize];

				lock (bufferLocker) buffers.Add(buffer);

				args.SetBuffer(buffer, 0, buffer.Length);
			}
		}

		public void ClearBuffer(SocketAsyncEventArgs args)
		{
			int index;
			lock (bufferLocker) index = buffers.IndexOf(args.Buffer);

			if (index >= 0)
				availableBuffers.Push(index);

			args.SetBuffer(null, 0, 0);
		}
	}
}