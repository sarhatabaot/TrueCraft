using System;

namespace TrueCraft.Serialization.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class IgnoreOnNullAttribute : Attribute
	{
	}
}