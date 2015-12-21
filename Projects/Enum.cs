using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestForge.Projects
{
	public interface IEnum : IType
	{
		Guid ID { get; }
		string Name { get; }
		bool IsPacked { get; }
		IProject IProject { get; }
		INamespace IParent { get; }
		ObservableCollection<IEnumElement> IElements { get; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class EnumBase<P, N, E, EE, D, DE, S, SM, SMP> : TypeEnum, IEnum
		where P : ProjectBase<P, N, E, EE, D, DE, S, SM, SMP> where N : NamespaceBase<P, N, E, EE, D, DE, S, SM, SMP>
		where E : EnumBase<P, N, E, EE, D, DE, S, SM, SMP> where EE : EnumElementBase<P, N, E, EE, D, DE, S, SM, SMP>
		where D : DataBase<P, N, E, EE, D, DE, S, SM, SMP> where DE : DataElementBase<P, N, E, EE, D, DE, S, SM, SMP>
		where S : ServiceBase<P, N, E, EE, D, DE, S, SM, SMP> where SM : ServiceMethodBase<P, N, E, EE, D, DE, S, SM, SMP> where SMP : ServiceMethodParameterBase
	{
		private readonly ObservableCollection<IEnumElement> _elements;

		public sealed override Guid ID { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("packed")]
		public bool IsPacked { get; set; }

		[JsonProperty("project", IsReference = true)]
		public P Project { get; private set; }

		[JsonProperty("parent", IsReference = true)]
		public N Parent { get; private set; }

		[JsonProperty("values")]
		public ObservableCollection<EE> Elements { get; private set; }

		[JsonIgnore]
		ObservableCollection<IEnumElement> IEnum.IElements { get { return _elements; } }

		[JsonIgnore]
		TypeMode IType.Mode { get { return TypeMode.Enum; } }

		[JsonIgnore]
		IProject IEnum.IProject { get { return Project; } }

		[JsonIgnore]
		INamespace IEnum.IParent { get { return Parent; } }

		[JsonConstructor]
		protected EnumBase()
		{
			_elements = new ObservableCollection<IEnumElement>();
			Elements = new ObservableCollection<EE>();

			Elements.CollectionChanged += Elements_CollectionChanged;
		}

		protected EnumBase(string name, N parent, P project)
		{
			ID = Guid.NewGuid();
			Name = name;
			Project = project;
			Parent = parent;

			_elements = new ObservableCollection<IEnumElement>();
			Elements = new ObservableCollection<EE>();

			Elements.CollectionChanged += Elements_CollectionChanged;
		}

		private void Elements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
				Elements.Clear();

			if (e.Action == NotifyCollectionChangedAction.Add)
				foreach (EE t in e.NewItems)
					Elements.Add(t);

			if (e.Action == NotifyCollectionChangedAction.Remove)
				foreach (EE t in e.OldItems)
					Elements.Remove(t);

			if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				int c = 0;
				foreach (EE t in e.NewItems)
				{
					Elements[e.OldStartingIndex + c] = t;
					c++;
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Move)
			{
				int c = 0;
				foreach (EE t in e.NewItems)
				{
					Elements.RemoveAt(e.OldStartingIndex + c);
					Elements.Insert(e.NewStartingIndex + c, t);
					c++;
				}
			}
		}

		public override string ToString()
		{
			return Name;
		}
	}

	public interface IEnumElement
	{
		string Name { get; }
		IEnum IParent { get; }
		bool IsHidden { get; }
		bool IsAuto { get; }
		bool IsCustom { get; }
		bool IsAggregate { get; }
		long ServerValue { get; }
		long? ClientValue { get; }
		ObservableCollection<IEnumElement> IAggregateValues { get; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class EnumElementBase<P, N, E, EE, D, DE, S, SM, SMP> : IEnumElement
		where P : ProjectBase<P, N, E, EE, D, DE, S, SM, SMP> where N : NamespaceBase<P, N, E, EE, D, DE, S, SM, SMP>
		where E : EnumBase<P, N, E, EE, D, DE, S, SM, SMP> where EE : EnumElementBase<P, N, E, EE, D, DE, S, SM, SMP>
		where D : DataBase<P, N, E, EE, D, DE, S, SM, SMP> where DE : DataElementBase<P, N, E, EE, D, DE, S, SM, SMP>
		where S : ServiceBase<P, N, E, EE, D, DE, S, SM, SMP> where SM : ServiceMethodBase<P, N, E, EE, D, DE, S, SM, SMP> where SMP : ServiceMethodParameterBase
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

		[JsonProperty("aggregates")]
		public ObservableCollection<EnumElementBase<P, N, E, EE, D, DE, S, SM, SMP>> AggregateValues { get; private set; }

		[JsonIgnore]
		IEnum IEnumElement.IParent { get { return Parent;} }

		[JsonIgnore]
		ObservableCollection<IEnumElement> IEnumElement.IAggregateValues { get { return ((ObservableCollection<IEnumElement>)AggregateValues.AsEnumerable()); } }

		[JsonConstructor]
		protected EnumElementBase() { }

		protected EnumElementBase(E parent, string name)
		{
			Name = name;
			Parent = parent;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
