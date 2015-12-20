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

		ObservableCollection<BuildMessage> BuildMessages { get; }

		Task<bool> Build();
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

		[JsonIgnore]
		public ObservableCollection<BuildMessage> BuildMessages { get; private set; }

		[JsonConstructor]
		protected ProjectBase()
		{
			Dependencies = new List<Guid>();
			BuildMessages = new ObservableCollection<BuildMessage>();
		}

		protected ProjectBase(string name)
		{
			ID = Guid.NewGuid();
			Name = name;

			Dependencies = new List<Guid>();
			BuildMessages = new ObservableCollection<BuildMessage>();
		}

		public async Task<bool> Build()
		{
			BuildMessages.Clear();
			return await InternalBuild();
		}

		public abstract Task<bool> InternalBuild();
		public abstract Task Save(string path);
	}

	public interface INamespace
	{
		Guid ID { get; }
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

		[JsonProperty("id")]
		public Guid ID { get; private set; }

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

		[JsonConstructor]
		protected NamespaceBase()
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
			Enumerations.CollectionChanged += Enumerations_CollectionChanged;
			Data.CollectionChanged += Data_CollectionChanged;
			Services.CollectionChanged += Services_CollectionChanged;
		}

		protected NamespaceBase(string name, N parent, P project)
		{
			ID = Guid.NewGuid();
			Name = name;
			Project = project;
			Parent = parent;

			_namespaces = new ObservableCollection<INamespace>();
			_enums = new ObservableCollection<IEnum>();
			_data = new ObservableCollection<IData>();
			_services = new ObservableCollection<IService>();

			Children = new ObservableCollection<N>();
			Enumerations = new ObservableCollection<E>();
			Data = new ObservableCollection<D>();
			Services = new ObservableCollection<S>();

			Children.CollectionChanged += Children_CollectionChanged;
			Enumerations.CollectionChanged += Enumerations_CollectionChanged;
			Data.CollectionChanged += Data_CollectionChanged;
			Services.CollectionChanged += Services_CollectionChanged;
		}

		private void Services_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
				_services.Clear();

			if (e.Action == NotifyCollectionChangedAction.Add)
				foreach (S t in e.NewItems)
					_services.Add(t);

			if (e.Action == NotifyCollectionChangedAction.Remove)
				foreach (S t in e.OldItems)
					_services.Remove(t);

			if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				int c = 0;
				foreach (S t in e.NewItems)
				{
					_services[e.OldStartingIndex + c] = t;
					c++;
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Move)
			{
				int c = 0;
				foreach (S t in e.NewItems)
				{
					_services.RemoveAt(e.OldStartingIndex + c);
					_services.Insert(e.NewStartingIndex + c, t);
					c++;
				}
			}
		}

		private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
				_data.Clear();

			if (e.Action == NotifyCollectionChangedAction.Add)
				foreach (D t in e.NewItems)
					_data.Add(t);

			if (e.Action == NotifyCollectionChangedAction.Remove)
				foreach (D t in e.OldItems)
					_data.Remove(t);

			if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				int c = 0;
				foreach (D t in e.NewItems)
				{
					_data[e.OldStartingIndex + c] = t;
					c++;
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Move)
			{
				int c = 0;
				foreach (D t in e.NewItems)
				{
					_data.RemoveAt(e.OldStartingIndex + c);
					_data.Insert(e.NewStartingIndex + c, t);
					c++;
				}
			}
		}

		private void Enumerations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
				_enums.Clear();

			if (e.Action == NotifyCollectionChangedAction.Add)
				foreach (E t in e.NewItems)
					_enums.Add(t);

			if (e.Action == NotifyCollectionChangedAction.Remove)
				foreach (E t in e.OldItems)
					_enums.Remove(t);

			if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				int c = 0;
				foreach (E t in e.NewItems)
				{
					_enums[e.OldStartingIndex + c] = t;
					c++;
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Move)
			{
				int c = 0;
				foreach (E t in e.NewItems)
				{
					_enums.RemoveAt(e.OldStartingIndex + c);
					_enums.Insert(e.NewStartingIndex + c, t);
					c++;
				}
			}
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

			if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				int c = 0;
				foreach (N t in e.NewItems)
				{
					_namespaces[e.OldStartingIndex + c] = t;
					c++;
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Move)
			{
				int c = 0;
				foreach (N t in e.NewItems)
				{
					_namespaces.RemoveAt(e.OldStartingIndex + c);
					_namespaces.Insert(e.NewStartingIndex + c, t);
					c++;
				}
			}
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
