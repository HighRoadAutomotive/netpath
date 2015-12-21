using RestForge.Solutions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RestForge.Designer.Model
{
	public class SolutionXaml : DependencyObject
	{
		public Solution Solution { get; }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(SolutionXaml), new PropertyMetadata("", delegate(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as SolutionXaml;
			if (t == null) return;

			t.Solution.Name = (string)e.NewValue;
		}));

		public ObservableCollection<SolutionFolderXaml> Folders { get { return (ObservableCollection<SolutionFolderXaml>)GetValue(FoldersProperty); } set { SetValue(FoldersProperty, value); } }
		public static readonly DependencyProperty FoldersProperty = DependencyProperty.Register("Folders", typeof(ObservableCollection<SolutionFolderXaml>), typeof(SolutionXaml), new PropertyMetadata(null));

		public ObservableCollection<ProjectXaml> Projects { get { return (ObservableCollection<ProjectXaml>)GetValue(ProjectsProperty); } set { SetValue(ProjectsProperty, value); } }
		public static readonly DependencyProperty ProjectsProperty = DependencyProperty.Register("Projects", typeof(ObservableCollection<ProjectXaml>), typeof(SolutionXaml), new PropertyMetadata(null));

		public SolutionXaml(string name)
		{
			Folders = new ObservableCollection<SolutionFolderXaml>();
			Folders.CollectionChanged += Folders_CollectionChanged;
			Projects = new ObservableCollection<ProjectXaml>();
			Projects.CollectionChanged += Projects_CollectionChanged;

			Name = name;
			Solution = new Solution(name);
		}

		public SolutionXaml(Solution solution)
		{
			Folders = new ObservableCollection<SolutionFolderXaml>();
			Folders.CollectionChanged += Folders_CollectionChanged;
			Projects = new ObservableCollection<ProjectXaml>();
			Projects.CollectionChanged += Projects_CollectionChanged;

			Solution = solution;

			Name = Solution.Name;
			foreach (SolutionFolder t in Solution.Folders)
				Folders.Add(new SolutionFolderXaml(t));
		}

		private void Projects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
				Solution.Projects.Clear();

			if (e.Action == NotifyCollectionChangedAction.Add)
				foreach (ProjectXaml t in e.NewItems)
					Solution.Projects.Add(t.SolutionProject);

			if (e.Action == NotifyCollectionChangedAction.Remove)
				foreach (ProjectXaml t in e.OldItems)
				{
					var x = Solution.Projects.FirstOrDefault(a => a.ID == t.SolutionProject.ID);
					if (x != null)
						Solution.Projects.Remove(x);
				}

			if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				int c = 0;
				foreach (ProjectXaml t in e.NewItems)
				{
					Solution.Projects[e.OldStartingIndex + c] = t.SolutionProject;
					c++;
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Move)
			{
				int c = 0;
				foreach (ProjectXaml t in e.NewItems)
				{
					Solution.Projects.RemoveAt(e.OldStartingIndex + c);
					Solution.Projects.Insert(e.NewStartingIndex + c, t.SolutionProject);
					c++;
				}
			}
		}

		private void Folders_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
				Solution.Folders.Clear();

			if (e.Action == NotifyCollectionChangedAction.Add)
				foreach (SolutionFolderXaml t in e.NewItems)
					Solution.Folders.Add(t.Folder);

			if (e.Action == NotifyCollectionChangedAction.Remove)
				foreach (SolutionFolderXaml t in e.OldItems)
				{
					var x = Solution.Folders.FirstOrDefault(a => a.ID == t.Folder.ID);
					if (x != null)
						Solution.Folders.Remove(x);
				}

			if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				int c = 0;
				foreach (SolutionFolderXaml t in e.NewItems)
				{
					Solution.Folders[e.OldStartingIndex + c] = t.Folder;
					c++;
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Move)
			{
				int c = 0;
				foreach (SolutionFolderXaml t in e.NewItems)
				{
					Solution.Folders.RemoveAt(e.OldStartingIndex + c);
					Solution.Folders.Insert(e.NewStartingIndex + c, t.Folder);
					c++;
				}
			}
		}
	}
}
