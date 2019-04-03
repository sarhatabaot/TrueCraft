using System;
using System.Collections.ObjectModel;

namespace TrueCraft
{
	public class ReadOnlyNibbleArray
	{
		public ReadOnlyNibbleArray(NibbleSlice array) => NibbleArray = array;

		private NibbleSlice NibbleArray { get; }

		public byte this[int index] => NibbleArray[index];

		public ReadOnlyCollection<byte> Data => Array.AsReadOnly(NibbleArray.Data);
	}
}