using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RestForge.Solutions
{
	[JsonObject(MemberSerialization.OptIn)]
	public sealed class UserProfile
	{
		[JsonProperty("collapsed")]
		public ConcurrentDictionary<Guid, bool> CollapsedItems { get; }

		[JsonProperty("open")]
		public List<Guid> OpenDocuments { get; }

		[JsonConstructor]
		public UserProfile()
		{
			CollapsedItems = new ConcurrentDictionary<Guid, bool>();
			OpenDocuments = new List<Guid>();
		}
	}
}
