using System;

namespace TrueCraft.Networking
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public class MessageTargetAttribute : Attribute
	{
		public MessageTargetAttribute(MessageTarget target)
		{
			Target = target;
		}

		public MessageTarget Target { get; }
	}
}