using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;

namespace WCFArchitect.Projects
{
	public class Namespace : DataType 
	{
		public string FullName { get { return GetNamespaceString(); } set { SetValue(FullNameProperty, value); } }
		public static readonly DependencyProperty FullNameProperty = DependencyProperty.Register("FullName", typeof(string), typeof(Namespace));

		public string URI { get { return (string)GetValue(URIProperty); } set { SetValue(URIProperty, value); } }
		public static readonly DependencyProperty URIProperty = DependencyProperty.Register("URI", typeof(string), typeof(Namespace));

		public Project Owner { get { return (Project)GetValue(OwnerProperty); } set { SetValue(OwnerProperty, value); } }
		public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register("Owner", typeof(Project), typeof(Namespace));

		public ObservableCollection<Namespace> Children { get { return (ObservableCollection<Namespace>)GetValue(ChildrenProperty); } set { SetValue(ChildrenProperty, value); } }
		public static readonly DependencyProperty ChildrenProperty = DependencyProperty.Register("Children", typeof(ObservableCollection<Namespace>), typeof(Namespace));

		public ObservableCollection<Enum> Enums { get { return (ObservableCollection<Enum>)GetValue(EnumsProperty); } set { SetValue(EnumsProperty, value); } }
		public static readonly DependencyProperty EnumsProperty = DependencyProperty.Register("Enums", typeof(ObservableCollection<Enum>), typeof(Namespace));

		public ObservableCollection<Data> Data { get { return (ObservableCollection<Data>)GetValue(DataProperty); } set { SetValue(DataProperty, value); } }
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(ObservableCollection<Data>), typeof(Namespace));

		public ObservableCollection<Service> Services { get { return (ObservableCollection<Service>)GetValue(ServicesProperty); } set { SetValue(ServicesProperty, value); } }
		public static readonly DependencyProperty ServicesProperty = DependencyProperty.Register("Services", typeof(ObservableCollection<Service>), typeof(Namespace));

		public ObservableCollection<ServiceBinding> Bindings { get { return (ObservableCollection<ServiceBinding>)GetValue(BindingsProperty); } set { SetValue(BindingsProperty, value); } }
		public static readonly DependencyProperty BindingsProperty = DependencyProperty.Register("Bindings", typeof(ObservableCollection<ServiceBinding>), typeof(Namespace));

		public ObservableCollection<Host> Hosts { get { return (ObservableCollection<Host>)GetValue(HostsProperty); } set { SetValue(HostsProperty, value); } }
		public static readonly DependencyProperty HostsProperty = DependencyProperty.Register("Hosts", typeof(ObservableCollection<Host>), typeof(Namespace));

		//System
		public bool IsTreeExpanded { get { return (bool)GetValue(IsTreeExpandedProperty); } set { SetValue(IsTreeExpandedProperty, value); } }
		public static readonly DependencyProperty IsTreeExpandedProperty = DependencyProperty.Register("IsTreeExpanded", typeof(bool), typeof(Namespace));

		[IgnoreDataMember] public string FullURI { get { return GetFullURI(this); } }

		private Namespace() : base(DataTypeMode.Namespace)
		{
			Parent = null;
			Children = new ObservableCollection<Namespace>();
			Enums = new ObservableCollection<Enum>();
			Data = new ObservableCollection<Data>();
			Services = new ObservableCollection<Service>();
			Bindings = new ObservableCollection<ServiceBinding>();
			Hosts = new ObservableCollection<Host>();
			URI = "";
		}

		public Namespace(string Name, Namespace Parent, Project Owner) : base(DataTypeMode.Namespace)
		{
			Children = new ObservableCollection<Namespace>();
			Enums = new ObservableCollection<Enum>();
			Data = new ObservableCollection<Data>();
			Services = new ObservableCollection<Service>();
			Bindings = new ObservableCollection<ServiceBinding>();
			Hosts = new ObservableCollection<Host>();
			ID = Guid.NewGuid();
			this.Name = Name;
			this.Parent = Parent;
			this.Owner = Owner;
			URI = GetFullURI(this);
		}

		public List<ServiceBinding> GetBindings()
		{
			var sbl = new List<ServiceBinding>();
			foreach (Namespace ns in Children)
				sbl.AddRange(ns.GetBindings());
			sbl.AddRange(Bindings);
			return sbl;
		}

		public List<Service> GetServices()
		{
			var sbl = new List<Service>();
			foreach (Namespace ns in Children)
				sbl.AddRange(ns.GetServices());
			sbl.AddRange(Services);
			return sbl;
		}

		private string GetFullURI(Namespace o)
		{
			if (o.Parent == null) return o.URI + o.Name.Replace(".", "/") + "/";
			string uri = GetFullURI(o.Parent);
			return uri + o.Name.Replace(".", "/") + "/";
		}

		public void UpdateURI()
		{
			foreach(Namespace ns in Children)
				ns.UpdateURI();
			if(Parent != null)
				URI = GetFullURI(this);
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if(e.Property == NameProperty)
				UpdateURI();
		}

		public IEnumerable<DataType> SearchTypes(string Search)
		{
			var results = new List<DataType>();

			results.AddRange(from a in Enums where a.Name.IndexOf(Search, StringComparison.CurrentCultureIgnoreCase) >= 0 select a);
			results.AddRange(from a in Data where a.Name.IndexOf(Search, StringComparison.CurrentCultureIgnoreCase) >= 0 select a);

			foreach(Namespace n in Children)
				results.AddRange(n.SearchTypes(Search));

			return results;
		}

		public IEnumerable<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			var results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Namespace || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner, this));
						if (!string.IsNullOrEmpty(FullName)) if (FullName.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Full Name", FullName, Owner, this));
						if (!string.IsNullOrEmpty(URI)) if (URI.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("URI", URI, Owner, this));
					}
					else
					{
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner, this));
						if (!string.IsNullOrEmpty(FullName)) if (FullName.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Full Name", FullName, Owner, this));
						if (!string.IsNullOrEmpty(URI)) if (URI.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("URI", URI, Owner, this));
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(Name)) if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Owner, this));
					if (!string.IsNullOrEmpty(FullName)) if (Args.RegexSearch.IsMatch(FullName)) results.Add(new FindReplaceResult("Full Name", FullName, Owner, this));
					if (!string.IsNullOrEmpty(URI)) if (Args.RegexSearch.IsMatch(URI)) results.Add(new FindReplaceResult("URI", URI, Owner, this));
				}

				if (Args.ReplaceAll)
				{
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (!string.IsNullOrEmpty(URI)) URI = Microsoft.VisualBasic.Strings.Replace(URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						else
						{
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
							if (!string.IsNullOrEmpty(URI)) URI = Microsoft.VisualBasic.Strings.Replace(URI, Args.Search, Args.Replace);
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(Name)) Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (!string.IsNullOrEmpty(URI)) URI = Args.RegexSearch.Replace(URI, Args.Replace);
					}
				}
			}

			foreach (ServiceBinding s in Bindings)
				results.AddRange(s.FindReplace(Args));

			foreach (Host s in Hosts)
				results.AddRange(s.FindReplace(Args));

			foreach (Service s in Services)
				results.AddRange(s.FindReplace(Args));

			foreach (Data s in Data)
				results.AddRange(s.FindReplace(Args));

			foreach (Enum s in Enums)
				results.AddRange(s.FindReplace(Args));

			foreach (Namespace s in Children)
				results.AddRange(s.FindReplace(Args));

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
		{
			if (!Args.ReplaceAll) return;

			if (Args.UseRegex == false)
			{
				if (Args.MatchCase == false)
				{
					if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					if (Field == "URI") URI = Microsoft.VisualBasic.Strings.Replace(URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
				}
				else
				{
					if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
					if (Field == "URI") URI = Microsoft.VisualBasic.Strings.Replace(URI, Args.Search, Args.Replace);
				}
			}
			else
			{
				if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
				if (Field == "URI") URI = Args.RegexSearch.Replace(URI, Args.Replace);
			}
		}

		private string GetNamespaceString()
		{
			if (Parent != null)
			{
				string ns = "";

				Namespace tp = Parent;
				if (tp == null) tp = Owner.Namespace;

				string tns = "";
				while (tp != null)
				{
					tns = tns.Insert(0, tp.Name + ".");
					tp = tp.Parent;
				}
				ns += tns;

				ns += Name;

				return ns;
			}
			return Name;
		}

		public void UpdateFullNamespace()
		{
			FullName = GetNamespaceString();
			foreach (Namespace n in Children)
				n.UpdateFullNamespace();
		}

		public bool HasServices()
		{
			bool hn = false;

			foreach (Namespace n in Children.Where(n => n.HasServices()))
				hn = true;

			if (Services.Count > 0) hn = true;

			return hn;
		}

		public Host GetServiceHost(Service s)
		{
			foreach (Host h in Hosts)
				if (Equals(h.Service, s)) return h;

			return Children.Select(n => n.GetServiceHost(s)).FirstOrDefault(t => t != null);
		}

		public OpenableDocument GetLastSelectedItem()
		{
			foreach (ServiceBinding s in Bindings)
				if (s.IsSelected) return s;

			foreach (Host s in Hosts)
				if (s.IsSelected) return s;

			foreach (Service s in Services)
				if (s.IsSelected) return s;

			foreach (Data s in Data)
				if (s.IsSelected) return s;

			foreach (Enum s in Enums)
				if (s.IsSelected) return s;

			foreach (Namespace s in Children)
			{
				if (s.IsSelected) return s;
				OpenableDocument t = s.GetLastSelectedItem();
				if (t != null) return t;
			}

			return null;
		}

		public void SetSelectedItem(OpenableDocument Item)
		{
			//Reset all items to unselected.
			foreach (ServiceBinding s in Bindings)
				s.IsSelected = false;

			foreach (Host s in Hosts)
				s.IsSelected = false;

			foreach (Service s in Services)
				s.IsSelected = false;

			foreach (Data s in Data)
				s.IsSelected = false;

			foreach (Enum s in Enums)
				s.IsSelected = false;

			foreach (Namespace s in Children)
			{
				s.IsSelected = false;
				s.SetSelectedItem(null);
			}

			if (Item == null) return;
			Item.IsSelected = true;
		}
	}
}