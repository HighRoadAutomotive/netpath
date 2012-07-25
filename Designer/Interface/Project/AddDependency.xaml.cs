using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;
using Microsoft.Win32;

namespace WCFArchitect.Interface.Project
{
	internal partial class AddDependency : Window
	{
		ObservableCollection<Projects.DependencyProject> DependencyProjects;

		Projects.Project Project { get; set; }

		public AddDependency(Projects.Project Project)
		{
			DependencyProjects = new ObservableCollection<Projects.DependencyProject>(Globals.ProjectSpace.OfType<Projects.DependencyProject>());
			this.Project = Project;

			InitializeComponent();

			ReferenceItems.ItemsSource = DependencyProjects;
		}

		private bool ProjectHasAssembly(string Path)
		{
			foreach (Projects.Reference R in Project.References)
				if (R.Path == Path) return true;
			return false;
		}

		private void Add_Click(object sender, RoutedEventArgs e)
		{
			foreach (Projects.DependencyProject DP in ReferenceItems.SelectedItems)
			{
				bool HasProject = false;
				foreach (Projects.DependencyProject P in Project.DependencyProjects)
					if (P == DP) HasProject = true;

				if (HasProject == false)
					Project.DependencyProjects.Add(DP);
			}

			this.Close();
		}

		private void CloseWindow_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}