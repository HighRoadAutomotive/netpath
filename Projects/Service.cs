using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;

namespace RestForge.Projects
{
	public interface IService : IType
	{
		string Name { get; }
		List<IDataElement> IElements { get; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class ServiceBase<P, N, S, SM, SMP> : IService where P : ProjectBase<N> where N : NamespaceBase<ProjectBase<N>, N> where S : ServiceBase<P, N, S, SM, SMP> where SM : ServiceMethodBase<P, N, S, SM, SMP> where SMP : ServiceMethodParameterBase
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
		public List<SM> Elements { get; private set; }

		[JsonIgnore]
		List<IDataElement> IService.IElements { get { return ((List<IDataElement>)Elements.AsEnumerable()).ToList(); } }

		[JsonIgnore]
		TypeMode IType.Mode { get { return TypeMode.Object; } }

		[JsonIgnore]
		TypePrimitive IType.Primitive { get { return TypePrimitive.None; } }
	}

	public interface IServiceMethod
	{
		string Name { get; }
		TypeBase ReturnType { get; }
		bool IsHidden { get; }
		List<ServiceMethodParameterBase> IParameters { get; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class ServiceMethodBase<P, N, S, SM, SMP> : IServiceMethod where P : ProjectBase<N> where N : NamespaceBase<ProjectBase<N>, N> where S : ServiceBase<P, N, S, SM, SMP> where SM : ServiceMethodBase<P, N, S, SM, SMP> where SMP : ServiceMethodParameterBase
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("type")]
		public TypeBase ReturnType { get; set; }

		[JsonProperty("hidden")]
		public bool IsHidden { get; set; }

		[JsonProperty("parent", IsReference = true)]
		public S Parent { get; private set; }

		[JsonProperty("parameters", IsReference = true)]
		List<SMP> Parameters { get; }

		[JsonIgnore]
		List<ServiceMethodParameterBase> IServiceMethod.IParameters { get { return ((List<ServiceMethodParameterBase>)Parameters.AsEnumerable()).ToList(); } }
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
	}
}
