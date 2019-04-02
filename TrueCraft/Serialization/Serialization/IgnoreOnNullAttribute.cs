using System;

namespace fNbt.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class IgnoreOnNullAttribute : Attribute
	{
	}
}