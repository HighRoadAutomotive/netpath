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

		public ObservableCollection<BindingSecurity> Security { get { return (ObservableCollection<BindingSecurity>)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(ObservableCollection<BindingSecurity>), typeof(Namespace));

		public ObservableCollection<Host> Hosts { get { return (ObservableCollection<Host>)GetValue(HostsProperty); } set { SetValue(HostsProperty, value); } }
		public static readonly DependencyProperty HostsProperty = DependencyProperty.Register("Hosts", typeof(ObservableCollection<Host>), typeof(Namespace));

		//Internal Use - Searching / Filtering
		[IgnoreDataMember()] public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(Namespace));

		[IgnoreDataMember()] public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(Namespace));

		[IgnoreDataMember()] public bool IsFiltering { get { return (bool)GetValue(IsFilteringProperty); } set { SetValue(IsFilteringProperty, value); } }
		public static readonly DependencyProperty IsFilteringProperty = DependencyProperty.Register("IsFiltering", typeof(bool), typeof(Namespace));

		[IgnoreDataMember()] public bool IsFilterMatch { get { return (bool)GetValue(IsFilterMatchProperty); } set { SetValue(IsFilterMatchProperty, value); } }
		public static readonly DependencyProperty IsFilterMatchProperty = DependencyProperty.Register("IsFilterMatch", typeof(bool), typeof(Namespace));

		public bool IsTreeExpanded { get { return (bool)GetValue(IsTreeExpandedProperty); } set { SetValue(IsTreeExpandedProperty, value); } }
		public static readonly DependencyProperty IsTreeExpandedProperty = DependencyProperty.Register("IsTreeExpanded", typeof(bool), typeof(Namespace));

		private Namespace() : base(DataTypeMode.Namespace)
		{
			this.Parent = null;
			this.Children = new ObservableCollection<Namespace>();
			this.Enums = new ObservableCollection<Enum>();
			this.Data = new ObservableCollection<Data>();
			this.Services = new ObservableCollection<Service>();
			this.Bindings = new ObservableCollection<ServiceBinding>();
			this.Security = new ObservableCollection<BindingSecurity>();
			this.Hosts = new ObservableCollection<Host>();
			this.URI = "";
		}

		public Namespace(string Name, Namespace Parent, Project Owner) : base(DataTypeMode.Namespace)
		{
			this.ID = Guid.NewGuid();
			this.Name = Name;
			this.Parent = Parent;
			this.Owner = Owner;
			this.Children = new ObservableCollection<Namespace>();
			this.Enums = new ObservableCollection<Enum>();
			this.Data = new ObservableCollection<Data>();
			this.Services = new ObservableCollection<Service>();
			this.Bindings = new ObservableCollection<ServiceBinding>();
			this.Security = new ObservableCollection<BindingSecurity>();
			this.Hosts = new ObservableCollection<Host>();
			this.URI = "";
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (e.Property == Namespace.IsSearchingProperty) return;
			if (e.Property == Namespace.IsSearchMatchProperty) return;
			if (e.Property == Namespace.IsFilteringProperty) return;
			if (e.Property == Namespace.IsFilterMatchProperty) return;
			if (e.Property == Namespace.IsTreeExpandedProperty) return;

			IsDirty = true;
		}

		public void Search(string Value)
		{
			if (Value != "")
			{
				foreach (Service T in Services)
				{
					T.IsSearching = true;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
				foreach (Data T in Data)
				{
					T.IsSearching = true;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
				foreach (Enum T in Enums)
				{
					T.IsSearching = true;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
				foreach (Projects.ServiceBinding T in Bindings)
				{
					T.IsSearching = true;
					T.IsSearchMatch = T.Name.Contains(Value);
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
				}
				foreach (Projects.BindingSecurity T in Security)
				{
					T.IsSearching = true;
					T.IsSearchMatch = T.Name.Contains(Value);
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
				}
				foreach (Projects.Host T in Hosts)
				{
					T.IsSearching = true;
					T.IsSearchMatch = T.Name.Contains(Value);
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
				}
				foreach (Namespace T in Children)
					T.Search(Value);
			}
			else
			{
				foreach (Service T in Services)
				{
					T.IsSearching = false;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
				foreach (Data T in Data)
				{
					T.IsSearching = false;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
				foreach (Enum T in Enums)
				{
					T.IsSearching = false;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
				foreach (Projects.ServiceBinding T in Bindings)
				{
					T.IsSearching = false;
					T.IsSearchMatch = false;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
				}
				foreach (Projects.BindingSecurity T in Security)
				{
					T.IsSearching = false;
					T.IsSearchMatch = false;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
				}
				foreach (Projects.Host T in Hosts)
				{
					T.IsSearching = false;
					T.IsSearchMatch = false;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
				}
				foreach (Namespace T in Children)
					T.Search(Value);
			}
		}

		public void Filter(bool FilterAll, bool FilterNamespaces, bool FilterServices, bool FilterData, bool FilterEnums)
		{
			foreach (Service T in Services)
			{
				T.IsFiltering = !FilterAll;
				T.IsFilterMatch = false;
				if (FilterServices == true) T.IsFilterMatch = true;
			}
			foreach (Data T in Data)
			{
				T.IsFiltering = !FilterAll;
				T.IsFilterMatch = false;
				if (FilterData == true) T.IsFilterMatch = true;
			}
			foreach (Enum T in Enums)
			{
				T.IsFiltering = !FilterAll;
				T.IsFilterMatch = false;
				if (FilterEnums == true) T.IsFilterMatch = true;
			}
			foreach (Namespace T in Children)
				T.Filter(FilterAll, FilterNamespaces, FilterServices, FilterData, FilterEnums);
		}

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Namespace || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner, this));
							if (FullName != null && FullName != "") if (FullName.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Full Name", FullName, Owner, this));
							if (URI != null && URI != "") if (URI.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("URI", URI, Owner, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner, this));
							if (FullName != null && FullName != "") if (FullName.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Full Name", FullName, Owner, this));
							if (URI != null && URI != "") if (URI.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("URI", URI, Owner, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Owner, this));
						if (FullName != null && FullName != "") if (Args.RegexSearch.IsMatch(FullName)) results.Add(new FindReplaceResult("Full Name", FullName, Owner, this));
						if (URI != null && URI != "") if (Args.RegexSearch.IsMatch(URI)) results.Add(new FindReplaceResult("URI", URI, Owner, this));
					}
				}

				if (Args.ReplaceAll == true)
				{
					bool ia = IsActive;
					IsActive = true;
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (URI != null && URI != "") URI = Microsoft.VisualBasic.Strings.Replace(URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (URI != null && URI != "") URI = Microsoft.VisualBasic.Strings.Replace(URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (URI != null && URI != "") URI = Args.RegexSearch.Replace(URI, Args.Replace);
						}
					}
					IsActive = ia;
				}
			}

			foreach (Service S in Services)
				results.AddRange(S.FindReplace(Args));

			foreach (Data S in Data)
				results.AddRange(S.FindReplace(Args));

			foreach (Enum S in Enums)
				results.AddRange(S.FindReplace(Args));

			foreach (Namespace S in Children)
				results.AddRange(S.FindReplace(Args));

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
		{
			if (Args.ReplaceAll == true)
			{
				bool ia = IsActive;
				IsActive = true;
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "URI") URI = Microsoft.VisualBasic.Strings.Replace(URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "URI") URI = Microsoft.VisualBasic.Strings.Replace(URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (Field == "URI") URI = Args.RegexSearch.Replace(URI, Args.Replace);
					}
				}
				IsActive = ia;
			}
		}

		public void ChangeOwners(Projects.Namespace NewOwner)
		{
			foreach (Projects.Service S in Services)
			{
				foreach (Projects.Host H in Hosts)
				{
					if (H.Service == S)
					{
						Hosts.Remove(H);
						NewOwner.Hosts.Add(H);
						foreach (Projects.HostEndpoint HE in H.Endpoints) HE.Binding = HE.Binding.Copy(H.Name, NewOwner);
						break;
					}
				}
			}

			foreach (Namespace N in Children)
				N.ChangeOwners(NewOwner);
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
			else
			{
				return Name;
			}
		}

		public void UpdateFullNamespace()
		{
			FullName = GetNamespaceString();
			foreach (Namespace N in Children)
				N.UpdateFullNamespace();
		}

		public bool HasServices()
		{
			bool HN = false;

			foreach (Namespace N in Children)
				if (N.HasServices() == true) HN = true;

			if (Services.Count > 0) HN = true;

			return HN;
		}

		public Host GetServiceHost(Service s)
		{
			foreach (Host h in Hosts)
				if (h.Service == s)
					return h;
			foreach (Namespace n in Children)
			{
				Host t = n.GetServiceHost(s);
				if (t != null) return t;
			}
			return null;
		}
	}
}
