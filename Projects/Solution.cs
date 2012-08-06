using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Serialization;

namespace WCFArchitect.Projects
{
	[DataContract()]
	public class Solution : DependencyObject
	{
		[DataMember()] private Guid id = Guid.Empty;
		public Guid ID { get { return id; } }

		[DataMember()] public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Solution));

		[DataMember()] public ObservableCollection<string> Projects { get { return (ObservableCollection<string>)GetValue(ProjectsProperty); } set { SetValue(ProjectsProperty, value); } }
		public static readonly DependencyProperty ProjectsProperty = DependencyProperty.Register("Projects", typeof(ObservableCollection<string>), typeof(Solution));

		public string AbsolutePath { get; private set; }

		public Solution() { }

		public Solution(string Name)
		{
			this.Projects = new ObservableCollection<string>();
			this.id = Guid.NewGuid();
			this.Name = Name;
		}

		public static Solution Open(string Path)
		{
			Solution t = Storage.Open<Solution>(Path);
			t.AbsolutePath = Path;
			return t;
		}

		public static void Save(Solution Data)
		{
			Storage.Save<Solution>(Data.AbsolutePath, Data);
		}

		public static void Save(Solution Data, string Path)
		{
			Storage.Save<Solution>(Path, Data);
		}
	}
}