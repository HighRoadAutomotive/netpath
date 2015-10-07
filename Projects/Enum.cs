using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestForge.Projects
{
	public interface IEnum : IType
	{
		string Name { get; }
		bool IsPacked { get; }
		List<IEnumElement> IElements { get; }
	}

	public interface IEnumElement
	{
		string Name { get; }
		bool IsExcluded { get; }
		bool IsHidden { get; }
		bool IsAuto { get; }
		bool IsCustom { get; }
		bool IsAggregate { get; }
		long ServerValue { get; }
		long? ClientValue { get; }
		List<IEnumElement> IAggregateValues { get; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class EnumBase<P, N, E> : IEnum where P : ProjectBase<N> where N : NamespaceBase<ProjectBase<N>, N>
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("collapsed")]
		public bool Collapsed { get; set; }

		[JsonProperty("packed")]
		public bool IsPacked { get; set; }

		[JsonProperty("project", ItemIsReference = true)]
		public P Project { get; private set; }

		[JsonProperty("parent", ItemIsReference = true)]
		public N Parent { get; private set; }

		[JsonProperty("children")]
		public List<E> Elements { get; private set; }

		[JsonIgnore]
		List<IEnumElement> IEnum.IElements { get { return ((List<IEnumElement>)Elements.AsEnumerable()).ToList(); } }

		TypeMode IType.Mode { get { return TypeMode.Enum; } }

		TypePrimitive IType.Primitive { get { return TypePrimitive.Int64; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class EnumElementBase<P, N, E> : IEnumElement where E : EnumBase<P, N, EnumElementBase<P, N, E>> where P : ProjectBase<N> where N : NamespaceBase<ProjectBase<N>, N>
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("parent", ItemIsReference = true)]
		public P Parent { get; private set; }

		[JsonProperty("excluded")]
		public bool IsExcluded { get; set; }

		[JsonProperty("hidden")]
		public bool IsHidden { get; set; }

		[JsonProperty("auto")]
		public bool IsAuto { get; set; }

		[JsonProperty("custom")]
		public bool IsCustom { get; set; }

		[JsonProperty("aggregate")]
		public bool IsAggregate { get; set; }

		[JsonProperty("servervalue")]
		public long ServerValue { get; set; }

		[JsonProperty("clientvalue")]
		public long? ClientValue { get; set; }

		[JsonProperty("aggregates", ItemIsReference = true)]
		public List<E> AggregateValues { get; private set; }

		[JsonIgnore]
		List<IEnumElement> IEnumElement.IAggregateValues { get { return ((List<IEnumElement>)AggregateValues.AsEnumerable()).ToList(); } }

		public EnumElementBase(P parent, string name)
		{
			Name = name;
			Parent = parent;
		}
	}
}
