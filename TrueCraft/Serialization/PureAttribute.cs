using System;

namespace TrueCraft.Serialization
{
	/// <summary> Indicates that method doesn't contain observable side effects. </summary>
	[AttributeUsage(AttributeTargets.Method)]
	internal sealed class PureAttribute : Attribute
	{
	}
}