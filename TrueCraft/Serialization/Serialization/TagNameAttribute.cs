using System;

namespace fNbt.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class)]
	public class TagNameAttribute : Attribute
	{
		/// <summary>
		///  Decorates the given property or field with the specified
		///  NBT tag name.
		/// </summary>
		public TagNameAttribute(string name) => Name = name;

		public string Name { get; set; }
	}
}