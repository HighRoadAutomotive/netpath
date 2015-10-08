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
		bool IsHidden { get; }
		bool IsAuto { get; }
		bool IsCustom { get; }
		bool IsAggregate { get; }
		long ServerValue { get; }
		long? ClientValue { get; }
		List<IEnumElement> IAggregateValues { get; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class EnumBase<P, N, E, EE> : IEnum where P : ProjectBase<N> where N : NamespaceBase<ProjectBase<N>, N> where E : EnumBase<P, N, E, EE> where EE : EnumElementBase<P, N, E, EE>
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("collapsed")]
		public bool Collapsed { get; set; }

		[JsonProperty("packed")]
		public bool IsPacked { get; set; }

		[JsonProperty("project", IsReference = true)]
		public P Project { get; private set; }

		[JsonProperty("parent", IsReference = true)]
		public N Parent { get; private set; }

		[JsonProperty("values")]
		public List<E> Elements { get; private set; }

		[JsonIgnore]
		List<IEnumElement> IEnum.IElements { get { return ((List<IEnumElement>)Elements.AsEnumerable()).ToList(); } }

		[JsonIgnore]
		TypeMode IType.Mode { get { return TypeMode.Enum; } }

		[JsonProperty("primitive")]
		public TypePrimitive Primitive { get { return TypePrimitive.Int64; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class EnumElementBase<P, N, E, EE> : IEnumElement where E : EnumBase<P, N, E, EE> where P : ProjectBase<N> where N : NamespaceBase<ProjectBase<N>, N> where EE : EnumElementBase<P, N, E, EE>
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("parent", IsReference = true)]
		public E Parent { get; private set; }

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
		public List<EnumElementBase<P, N, E, EE>> AggregateValues { get; private set; }

		[JsonIgnore]
		List<IEnumElement> IEnumElement.IAggregateValues { get { return ((List<IEnumElement>)AggregateValues.AsEnumerable()).ToList(); } }

		public EnumElementBase(E parent, string name)
		{
			Name = name;
			Parent = parent;
		}
	}
}
