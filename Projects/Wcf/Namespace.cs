using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;
using NETPath.Projects.Wcf;

namespace NETPath.Projects.Wcf
{
	public class WcfNamespace : Namespace
	{
		public ObservableCollection<WcfNamespace> Children { get { return (ObservableCollection<WcfNamespace>)GetValue(ChildrenProperty); } set { SetValue(ChildrenProperty, value); } }
		public static readonly DependencyProperty ChildrenProperty = DependencyProperty.Register("Children", typeof(ObservableCollection<WcfNamespace>), typeof(WcfNamespace));

		public ObservableCollection<WcfData> Data { get { return (ObservableCollection<WcfData>)GetValue(DataProperty); } set { SetValue(DataProperty, value); } }
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(ObservableCollection<WcfData>), typeof(WcfNamespace));

		public ObservableCollection<WcfService> Services { get { return (ObservableCollection<WcfService>)GetValue(ServicesProperty); } set { SetValue(ServicesProperty, value); } }
		public static readonly DependencyProperty ServicesProperty = DependencyProperty.Register("Services", typeof(ObservableCollection<WcfService>), typeof(WcfNamespace));

		public ObservableCollection<WcfBinding> Bindings { get { return (ObservableCollection<WcfBinding>)GetValue(BindingsProperty); } set { SetValue(BindingsProperty, value); } }
		public static readonly DependencyProperty BindingsProperty = DependencyProperty.Register("Bindings", typeof(ObservableCollection<WcfBinding>), typeof(WcfNamespace));

		public ObservableCollection<WcfHost> Hosts { get { return (ObservableCollection<WcfHost>)GetValue(HostsProperty); } set { SetValue(HostsProperty, value); } }
		public static readonly DependencyProperty HostsProperty = DependencyProperty.Register("Hosts", typeof(ObservableCollection<WcfHost>), typeof(WcfNamespace));

		private WcfNamespace()
		{
			Parent = null;
			Children = new ObservableCollection<WcfNamespace>();
			Data = new ObservableCollection<WcfData>();
			Services = new ObservableCollection<WcfService>();
			Bindings = new ObservableCollection<WcfBinding>();
			Hosts = new ObservableCollection<WcfHost>();
			URI = "";
		}

		public WcfNamespace(string Name, WcfNamespace Parent, WcfProject Owner)
		{
			Children = new ObservableCollection<WcfNamespace>();
			Data = new ObservableCollection<WcfData>();
			Services = new ObservableCollection<WcfService>();
			Bindings = new ObservableCollection<WcfBinding>();
			Hosts = new ObservableCollection<WcfHost>();
			this.Name = Name;
			this.Parent = Parent;
			this.Owner = Owner;
			URI = GetFullURI(this);
		}

		public List<WcfBinding> GetBindings()
		{
			var sbl = new List<WcfBinding>();
			foreach (WcfNamespace ns in Children)
				sbl.AddRange(ns.GetBindings());
			sbl.AddRange(Bindings);
			return sbl;
		}

		public List<WcfService> GetServices()
		{
			var sbl = new List<WcfService>();
			foreach (WcfNamespace ns in Children)
				sbl.AddRange(ns.GetServices());
			sbl.AddRange(Services);
			return sbl;
		}

		public bool HasServices()
		{
			bool hn = false;

			foreach (WcfNamespace n in Children.Where(n => n.HasServices()))
				hn = true;

			if (Services.Count > 0) hn = true;

			return hn;
		}

		public WcfHost GetServiceHost(WcfService s)
		{
			foreach (WcfHost h in Hosts)
				if (Equals(h.Service, s)) return h;

			return Children.Select(n => n.GetServiceHost(s)).FirstOrDefault(t => t != null);
		}

		protected override sealed string GetFullURI(Namespace ns)
		{
			var o = ns as WcfNamespace;
			if (o == null) return "";
			if (o.Parent == null) return o.URI + o.Name.Replace(".", "/") + "/";
			string uri = GetFullURI(o.Parent as WcfNamespace);
			return uri + o.Name.Replace(".", "/") + "/";
		}

		public override void UpdateURI()
		{
			foreach (WcfNamespace ns in Children)
				ns.UpdateURI();
			if (Parent != null)
				URI = GetFullURI(this);
		}

		public override void UpdateFullNamespace()
		{
			FullName = GetNamespaceString();
			foreach (WcfNamespace n in Children)
				n.UpdateFullNamespace();
		}

		protected override string GetNamespaceString()
		{
			if (Parent != null)
			{
				string ns = "";

				var tp = Parent as WcfNamespace;
				if (tp == null)
				{
					var on = Owner as WcfProject;
					if (on == null) return "";
					tp = on.Namespace;
				}

				string tns = "";
				while (tp != null)
				{
					tns = tns.Insert(0, tp.Name + ".");
					tp = tp.Parent as WcfNamespace;
				}
				ns += tns;

				ns += Name;

				return ns;
			}
			return Name;
		}

		public override OpenableDocument GetLastSelectedItem()
		{
			foreach (WcfBinding s in Bindings)
				if (s.IsSelected) return s;

			foreach (WcfHost s in Hosts)
				if (s.IsSelected) return s;

			foreach (WcfService s in Services)
				if (s.IsSelected) return s;

			foreach (WcfData s in Data)
				if (s.IsSelected) return s;

			foreach (Enum s in Enums)
				if (s.IsSelected) return s;

			foreach (WcfNamespace s in Children)
			{
				if (s.IsSelected) return s;
				OpenableDocument t = s.GetLastSelectedItem();
				if (t != null) return t;
			}

			return null;
		}

		public override void SetSelectedItem(OpenableDocument Item)
		{
			//Reset all items to unselected.
			foreach (WcfBinding s in Bindings)
				s.IsSelected = false;

			foreach (WcfHost s in Hosts)
				s.IsSelected = false;

			foreach (WcfService s in Services)
				s.IsSelected = false;

			foreach (WcfData s in Data)
				s.IsSelected = false;

			foreach (Enum s in Enums)
				s.IsSelected = false;

			foreach (WcfNamespace s in Children)
			{
				s.IsSelected = false;
				s.SetSelectedItem(Item);
			}

			if (Item == null) return;
			Item.IsSelected = true;
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == NameProperty)
				UpdateURI();
		}

		public IEnumerable<DataType> SearchTypes(string Search)
		{
			var results = new List<DataType>();

			results.AddRange(from a in Enums where a.Name.IndexOf(Search, StringComparison.CurrentCultureIgnoreCase) >= 0 select a);
			results.AddRange(from a in Data where a.Name.IndexOf(Search, StringComparison.CurrentCultureIgnoreCase) >= 0 select a);

			foreach(WcfNamespace n in Children)
				results.AddRange(n.SearchTypes(Search));

			return results;
		}
	}
}