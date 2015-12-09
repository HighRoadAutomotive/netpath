using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;

namespace RestForge.Projects
{
	public interface IService : IType
	{
		string Name { get; }
		bool Collapsed { get; }
		string Path { get; }
		ObservableCollection<IDataElement> IElements { get; }
		IProject IProject { get; }
		INamespace IParent { get; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class ServiceBase<P, N, E, EE, D, DE, S, SM, SMP> : IService
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

		[JsonProperty("path")]
		public string Path { get; set; }

		[JsonProperty("methods")]
		public ObservableCollection<SM> Elements { get; private set; }

		[JsonIgnore]
		ObservableCollection<IDataElement> IService.IElements { get { return ((ObservableCollection<IDataElement>)Elements.AsEnumerable()); } }

		[JsonIgnore]
		TypeMode IType.Mode { get { return TypeMode.Object; } }

		[JsonIgnore]
		TypePrimitive IType.Primitive { get { return TypePrimitive.None; } }

		[JsonIgnore]
		IProject IService.IProject { get { return Project; } }

		[JsonIgnore]
		INamespace IService.IParent { get { return Parent; } }

		public override string ToString()
		{
			return Name;
		}
	}

	public interface IServiceMethod
	{
		string Name { get; }
		TypeBase ReturnType { get; }
		bool IsHidden { get; }
		IService IParent { get; }
		ObservableCollection<ServiceMethodParameterBase> IParameters { get; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class ServiceMethodBase<P, N, E, EE, D, DE, S, SM, SMP> : IServiceMethod
		where P : ProjectBase<P, N, E, EE, D, DE, S, SM, SMP> where N : NamespaceBase<P, N, E, EE, D, DE, S, SM, SMP>
		where E : EnumBase<P, N, E, EE, D, DE, S, SM, SMP> where EE : EnumElementBase<P, N, E, EE, D, DE, S, SM, SMP>
		where D : DataBase<P, N, E, EE, D, DE, S, SM, SMP> where DE : DataElementBase<P, N, E, EE, D, DE, S, SM, SMP>
		where S : ServiceBase<P, N, E, EE, D, DE, S, SM, SMP> where SM : ServiceMethodBase<P, N, E, EE, D, DE, S, SM, SMP> where SMP : ServiceMethodParameterBase
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("type")]
		public TypeBase ReturnType { get; set; }

		[JsonProperty("hidden")]
		public bool IsHidden { get; set; }

		[JsonProperty("parent", IsReference = true)]
		public S Parent { get; private set; }

		[JsonIgnore]
		IService IServiceMethod.IParent { get { return Parent; } }

		[JsonProperty("parameters", IsReference = true)]
		ObservableCollection<SMP> Parameters { get; }

		[JsonIgnore]
		ObservableCollection<ServiceMethodParameterBase> IServiceMethod.IParameters { get { return ((ObservableCollection<ServiceMethodParameterBase>)Parameters.AsEnumerable()); } }

		public override string ToString()
		{
			return Name;
		}
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class ServiceMethodParameterBase
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("type")]
		public TypeBase DataType { get; set; }

		[JsonProperty("hidden")]
		public bool IsHidden { get; set; }

		[JsonProperty("default")]
		public string Default { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
