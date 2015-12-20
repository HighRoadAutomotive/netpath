using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;

namespace RestForge.Projects
{
	public interface IService : IType
	{
		Guid ID { get; }
		string Name { get; }
		bool Collapsed { get; }
		string Path { get; }
		ObservableCollection<IServiceMethod> IMethods { get; }
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
		private readonly ObservableCollection<IServiceMethod> _methods;

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

		[JsonProperty("path")]
		public string Path { get; set; }

		[JsonProperty("methods")]
		public ObservableCollection<SM> Methods { get; private set; }

		[JsonIgnore]
		ObservableCollection<IServiceMethod> IService.IMethods { get { return _methods; } }

		[JsonIgnore]
		TypeMode IType.Mode { get { return TypeMode.Object; } }

		[JsonIgnore]
		TypePrimitive IType.Primitive { get { return TypePrimitive.None; } }

		[JsonIgnore]
		IProject IService.IProject { get { return Project; } }

		[JsonIgnore]
		INamespace IService.IParent { get { return Parent; } }

		[JsonConstructor]
		protected ServiceBase()
		{
			_methods = new ObservableCollection<IServiceMethod>();
			Methods = new ObservableCollection<SM>();

			Methods.CollectionChanged += Methods_CollectionChanged;
		}

		protected ServiceBase(string name, N parent, P project)
		{
			ID = Guid.NewGuid();
			Name = name;
			Project = project;
			Parent = parent;

			_methods = new ObservableCollection<IServiceMethod>();
			Methods = new ObservableCollection<SM>();

			Methods.CollectionChanged += Methods_CollectionChanged;
		}

		private void Methods_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
				_methods.Clear();

			if (e.Action == NotifyCollectionChangedAction.Add)
				foreach (SM t in e.NewItems)
					_methods.Add(t);

			if (e.Action == NotifyCollectionChangedAction.Remove)
				foreach (SM t in e.OldItems)
					_methods.Remove(t);

			if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				int c = 0;
				foreach (SM t in e.NewItems)
				{
					_methods[e.OldStartingIndex + c] = t;
					c++;
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Move)
			{
				int c = 0;
				foreach (SM t in e.NewItems)
				{
					_methods.RemoveAt(e.OldStartingIndex + c);
					_methods.Insert(e.NewStartingIndex + c, t);
					c++;
				}
			}
		}

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
		private readonly ObservableCollection<ServiceMethodParameterBase> _parameters;

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
		ObservableCollection<ServiceMethodParameterBase> IServiceMethod.IParameters { get { return _parameters; } }

		[JsonConstructor]
		protected ServiceMethodBase()
		{
			_parameters = new ObservableCollection<ServiceMethodParameterBase>();
			Parameters = new ObservableCollection<SMP>();

			Parameters.CollectionChanged += Parameters_CollectionChanged;
		}

		protected ServiceMethodBase(string name, S parent)
		{
			Name = name;
			Parent = parent;

			_parameters = new ObservableCollection<ServiceMethodParameterBase>();
			Parameters = new ObservableCollection<SMP>();

			Parameters.CollectionChanged += Parameters_CollectionChanged;
		}

		private void Parameters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
				_parameters.Clear();

			if (e.Action == NotifyCollectionChangedAction.Add)
				foreach (SMP t in e.NewItems)
					_parameters.Add(t);

			if (e.Action == NotifyCollectionChangedAction.Remove)
				foreach (SMP t in e.OldItems)
					_parameters.Remove(t);

			if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				int c = 0;
				foreach (SMP t in e.NewItems)
				{
					_parameters[e.OldStartingIndex + c] = t;
					c++;
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Move)
			{
				int c = 0;
				foreach (SMP t in e.NewItems)
				{
					_parameters.RemoveAt(e.OldStartingIndex + c);
					_parameters.Insert(e.NewStartingIndex + c, t);
					c++;
				}
			}
		}

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

		[JsonConstructor]
		protected ServiceMethodParameterBase() { }

		protected ServiceMethodParameterBase(string name, TypeBase type)
		{
			Name = name;
			DataType = type;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
