using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;

namespace NETPath.Projects.Data
{
	public class DataNamespace : Namespace
	{
		public ObservableCollection<DataNamespace> Children { get { return (ObservableCollection<DataNamespace>)GetValue(ChildrenProperty); } set { SetValue(ChildrenProperty, value); } }
		public static readonly DependencyProperty ChildrenProperty = DependencyProperty.Register("Children", typeof(ObservableCollection<DataNamespace>), typeof(DataNamespace));

		public ObservableCollection<DataData> Data { get { return (ObservableCollection<DataData>)GetValue(DataProperty); } set { SetValue(DataProperty, value); } }
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(ObservableCollection<DataData>), typeof(DataNamespace));

		private DataNamespace()
		{
			Parent = null;
			Children = new ObservableCollection<DataNamespace>();
			Data = new ObservableCollection<DataData>();
			URI = "";
		}

		public DataNamespace(string Name, DataNamespace Parent, DataProject Owner)
		{
			Children = new ObservableCollection<DataNamespace>();
			Data = new ObservableCollection<DataData>();
			this.Name = Name;
			this.Parent = Parent;
			this.Owner = Owner;
			URI = GetFullURI(this);
		}

		protected override sealed string GetFullURI(Namespace ns)
		{
			var o = ns as DataNamespace;
			if (o == null) return "";
			if (o.Parent == null) return o.URI + o.Name.Replace(".", "/") + "/";
			string uri = GetFullURI(o.Parent as DataNamespace);
			return uri + o.Name.Replace(".", "/") + "/";
		}

		public override void UpdateURI()
		{
			foreach (DataNamespace ns in Children)
				ns.UpdateURI();
			if (Parent != null)
				URI = GetFullURI(this);
		}

		public override void UpdateFullNamespace()
		{
			FullName = GetNamespaceString();
			foreach (DataNamespace n in Children)
				n.UpdateFullNamespace();
		}

		protected override string GetNamespaceString()
		{
			if (Parent != null)
			{
				string ns = "";

				var tp = Parent as DataNamespace;
				if (tp == null)
				{
					var on = Owner as DataProject;
					if (on == null) return "";
					tp = on.Namespace;
				}

				string tns = "";
				while (tp != null)
				{
					tns = tns.Insert(0, tp.Name + ".");
					tp = tp.Parent as DataNamespace;
				}
				ns += tns;

				ns += Name;

				return ns;
			}
			return Name;
		}

		public override OpenableDocument GetLastSelectedItem()
		{
			foreach (DataData s in Data)
				if (s.IsSelected) return s;

			foreach (Enum s in Enums)
				if (s.IsSelected) return s;

			foreach (DataNamespace s in Children)
			{
				if (s.IsSelected) return s;
				OpenableDocument t = s.GetLastSelectedItem();
				if (t != null) return t;
			}

			return null;
		}

		public override void SetSelectedItem(OpenableDocument Item)
		{
			foreach (DataData s in Data)
				s.IsSelected = false;

			foreach (Enum s in Enums)
				s.IsSelected = false;

			foreach (DataNamespace s in Children)
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

			foreach(DataNamespace n in Children)
				results.AddRange(n.SearchTypes(Search));

			return results;
		}
	}
}