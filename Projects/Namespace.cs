using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NETPath.Projects
{
	public abstract class Namespace : DataType
	{
		public string FullName { get { return GetNamespaceString(); } set { SetValue(FullNameProperty, value); } }
		public static readonly DependencyProperty FullNameProperty = DependencyProperty.Register("FullName", typeof(string), typeof(Namespace));

		public string URI { get { return (string)GetValue(URIProperty); } set { SetValue(URIProperty, value); } }
		public static readonly DependencyProperty URIProperty = DependencyProperty.Register("URI", typeof(string), typeof(Namespace));

		public bool Collapsed { get { return (bool)GetValue(CollapsedProperty); } set { SetValue(CollapsedProperty, value); } }
		public static readonly DependencyProperty CollapsedProperty = DependencyProperty.Register("Collapsed", typeof(bool), typeof(Namespace), new UIPropertyMetadata(false));

		public Project Owner { get { return (Project)GetValue(OwnerProperty); } set { SetValue(OwnerProperty, value); } }
		public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register("Owner", typeof(Project), typeof(Namespace));

		public ObservableCollection<Enum> Enums { get { return (ObservableCollection<Enum>)GetValue(EnumsProperty); } set { SetValue(EnumsProperty, value); } }
		public static readonly DependencyProperty EnumsProperty = DependencyProperty.Register("Enums", typeof(ObservableCollection<Enum>), typeof(Namespace));

		[IgnoreDataMember] public string FullURI { get { return GetFullURI(this); } }

		protected Namespace() : base(DataTypeMode.Namespace)
		{
			Enums = new ObservableCollection<Enum>();
		}

		protected abstract string GetNamespaceString();
		protected abstract string GetFullURI(Namespace ns);
		public abstract void UpdateURI();
		public abstract void UpdateFullNamespace();

		public abstract OpenableDocument GetLastSelectedItem();
		public abstract void SetSelectedItem(OpenableDocument Item);

	}
}
