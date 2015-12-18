using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
		Task Save(string path);
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
		public abstract Task Save(string path);
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

		INamespace AddNamespace();
		IEnum AddEnum();
		IData AddData();
		IService AddService();
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
		private readonly ObservableCollection<INamespace> _namespaces;
		private readonly ObservableCollection<IEnum> _enums;
		private readonly ObservableCollection<IData> _data;
		private readonly ObservableCollection<IService> _services;

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
		ObservableCollection<INamespace> INamespace.IChildren { get { return _namespaces; } }

		[JsonIgnore]
		ObservableCollection<IEnum> INamespace.IEnums { get { return _enums; } }

		[JsonIgnore]
		ObservableCollection<IData> INamespace.IData { get { return _data; } }

		[JsonIgnore]
		ObservableCollection<IService> INamespace.IService { get { return _services; } }

		[JsonIgnore]
		IProject INamespace.IProject { get { return Project; } }

		[JsonIgnore]
		INamespace INamespace.IParent { get { return Parent; } }

		[JsonIgnore]
		public string FullNamespace { get { return GetFullNamespace(); } }

		protected NamespaceBase(P project, N parent)
		{
			_namespaces = new ObservableCollection<INamespace>();
			_enums = new ObservableCollection<IEnum>();
			_data = new ObservableCollection<IData>();
			_services = new ObservableCollection<IService>();

			Children = new ObservableCollection<N>();
			Enumerations = new ObservableCollection<E>();
			Data = new ObservableCollection<D>();
			Services = new ObservableCollection<S>();

			Children.CollectionChanged += Children_CollectionChanged;

			Project = project;
			Parent = parent;
		}

		private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if(e.Action == NotifyCollectionChangedAction.Reset)
				_namespaces.Clear();

			if(e.Action == NotifyCollectionChangedAction.Add)
				foreach(N t in e.NewItems)
					_namespaces.Add(t);

			if(e.Action == NotifyCollectionChangedAction.Remove)
				foreach (N t in e.OldItems)
					_namespaces.Remove(t);
		}

		protected abstract string GetFullNamespace();

		public abstract INamespace AddNamespace();
		public abstract IEnum AddEnum();
		public abstract IData AddData();
		public abstract IService AddService();
		public abstract void RemoveNamespace(INamespace remove);
		public abstract void RemoveEnum(IEnum remove);
		public abstract void RemoveData(IData remove);
		public abstract void RemoveService(IService remove);
	}
}
