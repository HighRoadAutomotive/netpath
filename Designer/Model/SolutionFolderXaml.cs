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
	public class SolutionFolderXaml : DependencyObject
	{
		public SolutionFolder Folder { get; }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(SolutionFolderXaml), new PropertyMetadata("", delegate (DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as SolutionFolderXaml;
			if (t == null) return;

			t.Folder.Name = (string)e.NewValue;
		}));

		public ObservableCollection<SolutionFolderXaml> Folders { get { return (ObservableCollection<SolutionFolderXaml>)GetValue(FoldersProperty); } set { SetValue(FoldersProperty, value); } }
		public static readonly DependencyProperty FoldersProperty = DependencyProperty.Register("Folders", typeof(ObservableCollection<SolutionFolderXaml>), typeof(SolutionFolderXaml), new PropertyMetadata(null));

		public ObservableCollection<ProjectXaml> Projects { get { return (ObservableCollection<ProjectXaml>)GetValue(ProjectsProperty); } set { SetValue(ProjectsProperty, value); } }
		public static readonly DependencyProperty ProjectsProperty = DependencyProperty.Register("Projects", typeof(ObservableCollection<ProjectXaml>), typeof(SolutionFolderXaml), new PropertyMetadata(null));

		public SolutionFolderXaml(string name)
		{
			Folders = new ObservableCollection<SolutionFolderXaml>();
			Folders.CollectionChanged += Folders_CollectionChanged;
			Projects = new ObservableCollection<ProjectXaml>();
			Projects.CollectionChanged += Projects_CollectionChanged;

			Name = name;
			Folder = new SolutionFolder(name);
		}

		public SolutionFolderXaml(SolutionFolder folder)
		{
			Folders = new ObservableCollection<SolutionFolderXaml>();
			Folders.CollectionChanged += Folders_CollectionChanged;
			Projects = new ObservableCollection<ProjectXaml>();
			Projects.CollectionChanged += Projects_CollectionChanged;

			Folder = folder;
			Name = Folder.Name;

			foreach (SolutionFolder t in Folder.Folders)
				Folders.Add(new SolutionFolderXaml(t));
		}

		private void Projects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
				Folder.Projects.Clear();

			if (e.Action == NotifyCollectionChangedAction.Add)
				foreach (ProjectXaml t in e.NewItems)
					Folder.Projects.Add(t.SolutionProject);

			if (e.Action == NotifyCollectionChangedAction.Remove)
				foreach (ProjectXaml t in e.OldItems)
				{
					var x = Folder.Projects.FirstOrDefault(a => a.ID == t.SolutionProject.ID);
					if (x != null)
						Folder.Projects.Remove(x);
				}

			if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				int c = 0;
				foreach (ProjectXaml t in e.NewItems)
				{
					Folder.Projects[e.OldStartingIndex + c] = t.SolutionProject;
					c++;
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Move)
			{
				int c = 0;
				foreach (ProjectXaml t in e.NewItems)
				{
					Folder.Projects.RemoveAt(e.OldStartingIndex + c);
					Folder.Projects.Insert(e.NewStartingIndex + c, t.SolutionProject);
					c++;
				}
			}
		}

		private void Folders_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
				Folder.Folders.Clear();

			if (e.Action == NotifyCollectionChangedAction.Add)
				foreach (SolutionFolderXaml t in e.NewItems)
					Folder.Folders.Add(t.Folder);

			if (e.Action == NotifyCollectionChangedAction.Remove)
				foreach (SolutionFolderXaml t in e.OldItems)
				{
					var x = Folder.Folders.FirstOrDefault(a => a.ID == t.Folder.ID);
					if (x != null)
						Folder.Folders.Remove(x);
				}

			if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				int c = 0;
				foreach (SolutionFolderXaml t in e.NewItems)
				{
					Folder.Folders[e.OldStartingIndex + c] = t.Folder;
					c++;
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Move)
			{
				int c = 0;
				foreach (SolutionFolderXaml t in e.NewItems)
				{
					Folder.Folders.RemoveAt(e.OldStartingIndex + c);
					Folder.Folders.Insert(e.NewStartingIndex + c, t.Folder);
					c++;
				}
			}
		}
	}
}
