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
	public class Solution : DependencyObject
	{
		public Guid ID { get; set; }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Solution));

		public ObservableCollection<string> Projects { get { return (ObservableCollection<string>)GetValue(ProjectsProperty); } set { SetValue(ProjectsProperty, value); } }
		public static readonly DependencyProperty ProjectsProperty = DependencyProperty.Register("Projects", typeof(ObservableCollection<string>), typeof(Solution));

		[IgnoreDataMember] public string AbsolutePath { get; private set; }

		public Solution() { }

		public Solution(string Name)
		{
			Projects = new ObservableCollection<string>();
			ID = Guid.NewGuid();
			this.Name = Name;
		}

		public static Solution Open(string Path)
		{
			var t = Storage.Open<Solution>(Path);
			t.AbsolutePath = Path;
			return t;
		}

		public static void Save(Solution Data)
		{
			Storage.Save(Data.AbsolutePath, Data);
		}

		public static void Save(Solution Data, string Path)
		{
			Storage.Save(Path, Data);
		}

		public static byte[] Dump(Solution Data)
		{
			return Storage.Dump(Data);
		}
	}
}