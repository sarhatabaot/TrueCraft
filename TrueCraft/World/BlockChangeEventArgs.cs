﻿using System;
using TrueCraft.Logic;

namespace TrueCraft.World
{
	public class BlockChangeEventArgs : EventArgs
	{
		public BlockDescriptor NewBlock;
		public BlockDescriptor OldBlock;

		public Coordinates3D Position;

		public BlockChangeEventArgs(Coordinates3D position, BlockDescriptor oldBlock, BlockDescriptor newBlock)
		{
			Position = position;
			OldBlock = oldBlock;
			NewBlock = newBlock;
		}
	}
}