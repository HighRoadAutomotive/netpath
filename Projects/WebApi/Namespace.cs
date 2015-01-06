using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;

namespace NETPath.Projects.WebApi
{
	public class WebApiNamespace : Namespace
	{
		public ObservableCollection<WebApiNamespace> Children { get { return (ObservableCollection<WebApiNamespace>)GetValue(ChildrenProperty); } set { SetValue(ChildrenProperty, value); } }
		public static readonly DependencyProperty ChildrenProperty = DependencyProperty.Register("Children", typeof(ObservableCollection<WebApiNamespace>), typeof(WebApiNamespace));

		public ObservableCollection<WebApiData> Data { get { return (ObservableCollection<WebApiData>)GetValue(DataProperty); } set { SetValue(DataProperty, value); } }
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(ObservableCollection<WebApiData>), typeof(WebApiNamespace));

		public ObservableCollection<WebApiService> Services { get { return (ObservableCollection<WebApiService>)GetValue(ServicesProperty); } set { SetValue(ServicesProperty, value); } }
		public static readonly DependencyProperty ServicesProperty = DependencyProperty.Register("Services", typeof(ObservableCollection<WebApiService>), typeof(WebApiNamespace));

		private WebApiNamespace()
		{
			Parent = null;
			Children = new ObservableCollection<WebApiNamespace>();
			Data = new ObservableCollection<WebApiData>();
			Services = new ObservableCollection<WebApiService>();
			URI = "";
		}

		public WebApiNamespace(string Name, WebApiNamespace Parent, WebApiProject Owner)
		{
			Children = new ObservableCollection<WebApiNamespace>();
			Data = new ObservableCollection<WebApiData>();
			Services = new ObservableCollection<WebApiService>();
			this.Name = Name;
			this.Parent = Parent;
			this.Owner = Owner;
			URI = GetFullURI(this);
		}

		public List<WebApiService> GetServices()
		{
			var sbl = new List<WebApiService>();
			foreach (WebApiNamespace ns in Children)
				sbl.AddRange(ns.GetServices());
			sbl.AddRange(Services);
			return sbl;
		}

		public bool HasServices()
		{
			bool hn = false;

			foreach (WebApiNamespace n in Children.Where(n => n.HasServices()))
				hn = true;

			if (Services.Count > 0) hn = true;

			return hn;
		}

		protected override sealed string GetFullURI(Namespace ns)
		{
			var o = ns as WebApiNamespace;
			if (o == null) return "";
			if (o.Parent == null) return o.URI + o.Name.Replace(".", "/") + "/";
			string uri = GetFullURI(o.Parent as WebApiNamespace);
			return uri + o.Name.Replace(".", "/") + "/";
		}

		public override void UpdateURI()
		{
			foreach (WebApiNamespace ns in Children)
				ns.UpdateURI();
			if (Parent != null)
				URI = GetFullURI(this);
		}

		public override void UpdateFullNamespace()
		{
			FullName = GetNamespaceString();
			foreach (WebApiNamespace n in Children)
				n.UpdateFullNamespace();
		}

		protected override string GetNamespaceString()
		{
			if (Parent != null)
			{
				string ns = "";

				var tp = Parent as WebApiNamespace;
				if (tp == null)
				{
					var on = Owner as WebApiProject;
					if (on == null) return "";
					tp = on.Namespace;
				}

				string tns = "";
				while (tp != null)
				{
					tns = tns.Insert(0, tp.Name + ".");
					tp = tp.Parent as WebApiNamespace;
				}
				ns += tns;

				ns += Name;

				return ns;
			}
			return Name;
		}

		public override OpenableDocument GetLastSelectedItem()
		{
			foreach (WebApiService s in Services)
				if (s.IsSelected) return s;

			foreach (WebApiData s in Data)
				if (s.IsSelected) return s;

			foreach (Enum s in Enums)
				if (s.IsSelected) return s;

			foreach (WebApiNamespace s in Children)
			{
				if (s.IsSelected) return s;
				OpenableDocument t = s.GetLastSelectedItem();
				if (t != null) return t;
			}

			return null;
		}

		public override void SetSelectedItem(OpenableDocument Item)
		{
			foreach (WebApiService s in Services)
				s.IsSelected = false;

			foreach (WebApiData s in Data)
				s.IsSelected = false;

			foreach (Enum s in Enums)
				s.IsSelected = false;

			foreach (WebApiNamespace s in Children)
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

			foreach(WebApiNamespace n in Children)
				results.AddRange(n.SearchTypes(Search));

			return results;
		}
	}
}