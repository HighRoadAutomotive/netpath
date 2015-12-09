using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestForge.Projects
{
	public interface IData : IType
	{
		string Name { get; }
		bool Collapsed { get; }
		IProject IProject { get; }
		INamespace IParent { get; }
		ObservableCollection<IDataElement> IElements { get; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class DataBase<P, N, E, EE, D, DE, S, SM, SMP> : IData
		where P : ProjectBase<P, N, E, EE, D, DE, S, SM, SMP> where N : NamespaceBase<P, N, E, EE, D, DE, S, SM, SMP>
		where E : EnumBase<P, N, E, EE, D, DE, S, SM, SMP> where EE : EnumElementBase<P, N, E, EE, D, DE, S, SM, SMP>
		where D : DataBase<P, N, E, EE, D, DE, S, SM, SMP> where DE : DataElementBase<P, N, E, EE, D, DE, S, SM, SMP>
		where S : ServiceBase<P, N, E, EE, D, DE, S, SM, SMP> where SM : ServiceMethodBase<P, N, E, EE, D, DE, S, SM, SMP> where SMP : ServiceMethodParameterBase
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("collapsed")]
		public bool Collapsed { get; set; }

		[JsonProperty("project", IsReference = true)]
		public P Project { get; private set; }

		[JsonProperty("parent", IsReference = true)]
		public N Parent { get; private set; }

		[JsonProperty("members")]
		public ObservableCollection<DE> Elements { get; private set; }

		[JsonIgnore]
		ObservableCollection<IDataElement> IData.IElements { get { return ((ObservableCollection<IDataElement>)Elements.AsEnumerable()); } }
		[JsonIgnore]
		IProject IData.IProject { get { return Project; } }

		[JsonIgnore]
		INamespace IData.IParent { get { return Parent; } }


		[JsonIgnore]
		TypeMode IType.Mode { get { return TypeMode.Object; } }

		[JsonIgnore]
		TypePrimitive IType.Primitive { get { return TypePrimitive.None; } }

		public override string ToString()
		{
			return Name;
		}
	}

	public interface IDataElement
	{
		string Name { get; }
		string TransportName { get; }
		TypeBase DataType { get; }
		bool IsHidden { get; }
		bool IsReadOnly { get; }
		bool EnableUpdates { get; }
		bool IsUpdateLookup { get; }
		IData IParent { get; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class DataElementBase<P, N, E, EE, D, DE, S, SM, SMP> : IDataElement
		where P : ProjectBase<P, N, E, EE, D, DE, S, SM, SMP> where N : NamespaceBase<P, N, E, EE, D, DE, S, SM, SMP>
		where E : EnumBase<P, N, E, EE, D, DE, S, SM, SMP> where EE : EnumElementBase<P, N, E, EE, D, DE, S, SM, SMP>
		where D : DataBase<P, N, E, EE, D, DE, S, SM, SMP> where DE : DataElementBase<P, N, E, EE, D, DE, S, SM, SMP>
		where S : ServiceBase<P, N, E, EE, D, DE, S, SM, SMP> where SM : ServiceMethodBase<P, N, E, EE, D, DE, S, SM, SMP> where SMP : ServiceMethodParameterBase
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("transport")]
		public string TransportName { get; set; }

		[JsonProperty("type")]
		public TypeBase DataType { get; set; }

		[JsonProperty("hidden")]
		public bool IsHidden { get; set; }

		[JsonProperty("readonly")]
		public bool IsReadOnly { get; set; }

		[JsonProperty("parent", IsReference = true)]
		public D Parent { get; private set; }

		[JsonProperty("enableupdates")]
		public bool EnableUpdates { get; set; }

		[JsonProperty("updatelookup")]
		public bool IsUpdateLookup { get; set; }

		[JsonIgnore]
		IData IDataElement.IParent { get { return Parent; } }

		public override string ToString()
		{
			return Name;
		}
	}
}
