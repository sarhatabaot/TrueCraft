using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace TrueCraft.Server
{
	public class AccessConfiguration : Configuration, IAccessConfiguration
	{
		public AccessConfiguration()
		{
			Blacklist = new List<string>();
			Whitelist = new List<string>();
			Oplist = new List<string>();
		}

		[YamlMember(Alias = "blacklist")] public IList<string> Blacklist { get; }

		[YamlMember(Alias = "whitelist")] public IList<string> Whitelist { get; }

		[YamlMember(Alias = "ops")] public IList<string> Oplist { get; }
	}
}