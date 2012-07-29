using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MP.Karvonite;

namespace WCFArchitect.Projects
{
	public class Solution : DependencyObject
	{
		private Guid id = Guid.Empty;
		public Guid ID { get { return id; } }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Solution));

		public ObservableCollection<string> Projects { get { return (ObservableCollection<string>)GetValue(ProjectsProperty); } set { SetValue(ProjectsProperty, value); } }
		public static readonly DependencyProperty ProjectsProperty = DependencyProperty.Register("Projects", typeof(ObservableCollection<string>), typeof(Solution));

		public Solution() { }

		public Solution(string Name)
		{
			this.Projects = new ObservableCollection<string>();
			this.id = Guid.NewGuid();
			this.Name = Name;
		}

		public static Solution Open(string Path)
		{
			//Check the solution to make sure it exists
			if (!System.IO.File.Exists(Path))
				throw new System.IO.FileNotFoundException("Unable to locate the Solution file '" + Path + "'");
			
			//Make sure the solution isn't read-only.
			System.IO.FileInfo fi = new System.IO.FileInfo(Path);
			if (fi.IsReadOnly == true)
				throw new System.IO.IOException("The Solution '" + Path + "' is currently read-only. Please disable read-only mode on this file.");


			ObjectSpace os = new ObjectSpace("Solution.kvtmodel", "WCFArchitect.Projects");
			os.Open(Path, ObjectSpaceOpenMode.ReadOnly);

			Solution t = os.OfType<Solution>().FirstOrDefault();

			os.Close();

			return t;
		}

		public static void Save(Solution Data, string Path)
		{
			//Make sure the solution isn't read-only.
			if (!System.IO.File.Exists(Path))
			{
				System.IO.FileInfo fi = new System.IO.FileInfo(Path);
				if (fi.IsReadOnly == true)
					throw new System.IO.IOException("The Solution '" + Path + "' is currently read-only. Please disable read-only mode on this file.");
			}

			ObjectSpace os = new ObjectSpace("Solution.kvtmodel", "WCFArchitect.Projects");
			os.CreateObjectLibrary(Path, ExistingFileAction.Overwrite);
			os.Add(Data);
			os.Save();
			os.Close();
		}
	}
}