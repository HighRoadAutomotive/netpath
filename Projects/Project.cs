using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace RestForge.Projects
{
	public interface IProject
	{
		Guid ID { get; }
		string Name { get; }
		string SolutionPath { get; }
		List<Guid> Dependencies { get; }
		INamespace IRoot { get; }

		Task Build();
		Task Save();
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class ProjectBase<P, N, E, EE, D, DE, S, SM, SMP> : IProject
		where P : ProjectBase<P, N, E, EE, D, DE, S, SM, SMP> where N : NamespaceBase<P, N, E, EE, D, DE, S, SM, SMP>
		where E : EnumBase<P, N, E, EE, D, DE, S, SM, SMP> where EE : EnumElementBase<P, N, E, EE, D, DE, S, SM, SMP>
		where D : DataBase<P, N, E, EE, D, DE, S, SM, SMP> where DE : DataElementBase<P, N, E, EE, D, DE, S, SM, SMP>
		where S : ServiceBase<P, N, E, EE, D, DE, S, SM, SMP> where SM : ServiceMethodBase<P, N, E, EE, D, DE, S, SM, SMP> where SMP : ServiceMethodParameterBase
	{
		[JsonProperty("id")]
		public Guid ID { get; private set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("solutionPath")]
		public string SolutionPath { get; set; }

		[JsonProperty("dependencies")]
		public List<Guid> Dependencies { get; private set; }

		[JsonProperty("root")]
		public N Root { get; private set; }

		[JsonIgnore]
		INamespace IProject.IRoot { get { return Root; } }

		protected ProjectBase()
		{
			ID = Guid.NewGuid();
			Dependencies = new List<Guid>();
		}

		public abstract Task Build();
		public abstract Task Save();
	}

	public interface INamespace
	{
		string Name { get; }
		bool Collapsed { get; }
		ObservableCollection<INamespace> IChildren { get; }
		ObservableCollection<IEnum> IEnums { get; }
		ObservableCollection<IData> IData { get; }
		ObservableCollection<IService> IService { get; }
		IProject IProject { get; }
		INamespace IParent { get; }

		void AddNamespace();
		void AddEnum();
		void AddData();
		void AddService();
		void RemoveNamespace(INamespace remove);
		void RemoveEnum(IEnum remove);
		void RemoveData(IData remove);
		void RemoveService(IService remove);
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class NamespaceBase<P, N, E, EE, D, DE, S, SM, SMP> : INamespace
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

		[JsonProperty("children")]
		public ObservableCollection<N> Children { get; private set; }

		[JsonProperty("enums")]
		public ObservableCollection<E> Enumerations { get; private set; }

		[JsonProperty("data")]
		public ObservableCollection<D> Data { get; private set; }

		[JsonProperty("services")]
		public ObservableCollection<S> Services { get; private set; }

		[JsonIgnore]
		ObservableCollection<INamespace> INamespace.IChildren { get { return ((ObservableCollection<INamespace>)Children.AsEnumerable()); } }

		[JsonIgnore]
		ObservableCollection<IEnum> INamespace.IEnums { get { return ((ObservableCollection<IEnum>)Children.AsEnumerable()); } }

		[JsonIgnore]
		ObservableCollection<IData> INamespace.IData { get { return ((ObservableCollection<IData>)Data.AsEnumerable()); } }

		[JsonIgnore]
		ObservableCollection<IService> INamespace.IService { get { return ((ObservableCollection<IService>)Services.AsEnumerable()); } }

		[JsonIgnore]
		IProject INamespace.IProject { get { return Project; } }

		[JsonIgnore]
		INamespace INamespace.IParent { get { return Parent; } }

		[JsonIgnore]
		public string FullNamespace { get { return GetFullNamespace(); } }

		protected NamespaceBase(P project, N parent)
		{
			Children = new ObservableCollection<N>();
			Project = project;
			Parent = parent;
		}

		protected abstract string GetFullNamespace();

		public abstract void AddNamespace();
		public abstract void AddEnum();
		public abstract void AddData();
		public abstract void AddService();
		public abstract void RemoveNamespace(INamespace remove);
		public abstract void RemoveEnum(IEnum remove);
		public abstract void RemoveData(IData remove);
		public abstract void RemoveService(IService remove);
	}
}
