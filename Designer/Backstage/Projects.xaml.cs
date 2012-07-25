using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace WCFArchitect.Backstage
{
	public partial class Projects : Grid
	{
		public Projects()
		{
			InitializeComponent();

			if (Globals.IsNewVersionAvailable == true)
				UpdateAvailable.Visibility = System.Windows.Visibility.Visible;

			RefreshRecentList();

#if STANDARD
			NewDependency.Visibility = System.Windows.Visibility.Collapsed;
			NewDependencyOptions.Visibility = System.Windows.Visibility.Collapsed;
#endif
		}

		public void RefreshRecentList()
		{
			if (Globals.UserProfile.ImportantProjects.Count > 0) Globals.UserProfile.ImportantProjects.Sort(delegate(WCFArchitect.Options.RecentSolution p1, WCFArchitect.Options.RecentSolution p2) { return p2.LastAccessed.CompareTo(p1.LastAccessed); });
			if (Globals.UserProfile.RecentProjects.Count > 0) Globals.UserProfile.RecentProjects.Sort(delegate(WCFArchitect.Options.RecentSolution p1, WCFArchitect.Options.RecentSolution p2) { return p2.LastAccessed.CompareTo(p1.LastAccessed); });

			ImportantProjectsBlock.Visibility = System.Windows.Visibility.Collapsed;
			ImportantProjectsList.Children.Clear();
			RecentProjectsList.Children.Clear();

			foreach (WCFArchitect.Options.RecentSolution RP in Globals.UserProfile.ImportantProjects)
			{
				ImportantProjectsBlock.Visibility = System.Windows.Visibility.Visible;
				RecentProjectItem NRPI = new RecentProjectItem(RP, true);
				ImportantProjectsList.Children.Add(NRPI);
			}
			foreach (WCFArchitect.Options.RecentSolution RP in Globals.UserProfile.RecentProjects)
			{
				RecentProjectItem NRPI = new RecentProjectItem(RP, false);
				RecentProjectsList.Children.Add(NRPI);
			}
		}

		private void NewSolution_Checked(object sender, RoutedEventArgs e)
		{
			NewSolutionOptionsName.Text = "New Solution";
			NewSolutionOptionsLocation.Text = Globals.UserProfile.DefaultProjectFolder;
			NewSolutionOptions.Visibility = System.Windows.Visibility.Visible;
		}

		private void NewSolution_Unchecked(object sender, RoutedEventArgs e)
		{
			NewSolutionOptions.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void NewSolutionOptionsBrowse_Click(object sender, RoutedEventArgs e)
		{
			string FileName = "";
			string openpath = Globals.UserProfile.DefaultProjectFolder;
			if (!(NewSolutionOptionsLocation.Text == "" || NewSolutionOptionsLocation.Text == null)) openpath = NewSolutionOptionsLocation.Text;

			try
			{
				Microsoft.WindowsAPICodePack.Dialogs.CommonSaveFileDialog sfd = new Microsoft.WindowsAPICodePack.Dialogs.CommonSaveFileDialog("Select a Location to Save the Solution");
				sfd.AlwaysAppendDefaultExtension = true;
				sfd.EnsurePathExists = true;
				sfd.InitialDirectory = openpath;
				sfd.ShowPlacesList = true;
				sfd.DefaultFileName = NewSolutionOptionsName.Text;
				sfd.DefaultExtension = ".was";
				sfd.Filters.Add(new Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter("WCF Architect Solution Files", ".was"));
				if (sfd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;
				FileName = sfd.FileName;
			}
			catch
			{
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.InitialDirectory = openpath;
				sfd.FileName = NewSolutionOptionsName.Text;
				sfd.DefaultExt = ".was";
				sfd.Filter = "WCF Architect Solution Files (*.was)|*.was";
				sfd.Title = "Select a Location to Save the Solution";
				if (sfd.ShowDialog().Value == false) return;
				FileName = sfd.FileName;
			}

			NewSolutionOptionsLocation.Text = new System.IO.FileInfo(FileName).Directory.FullName + "\\";
		}

		private void NewSolutionCreate_Click(object sender, RoutedEventArgs e)
		{
			Create();
		}

		private void NewProject_Checked(object sender, RoutedEventArgs e)
		{
			NewProjectOptionsName.Text = "New Project";
			NewProjectOptions.Visibility = System.Windows.Visibility.Visible;
			NewDependency.IsChecked = false;
		}

		private void NewProject_Unchecked(object sender, RoutedEventArgs e)
		{
			NewProjectOptions.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void NewProjectCreate_Click(object sender, RoutedEventArgs e)
		{
			Create();
		}

		private void NewDependency_Checked(object sender, RoutedEventArgs e)
		{
			NewDependencyOptionsName.Text = "New Dependency";
			NewDependencyOptions.Visibility = System.Windows.Visibility.Visible;
			NewProject.IsChecked = false;
		}

		private void NewDependency_Unchecked(object sender, RoutedEventArgs e)
		{
			NewDependencyOptions.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void NewDependencyCreate_Click(object sender, RoutedEventArgs e)
		{
			Create();
		}

		private void Create()
		{
			//Verify the names aren't blank
			if (NewSolution.IsChecked == true)
			{
				if (NewSolutionOptionsLocation.Text == "")
				{
					Prospective.Controls.MessageBox.Show("You must specify a path for the new solution.", "Solution Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
					NewSolutionOptionsLocation.Focus();
					return;
				}
				if (NewSolutionOptionsName.Text == "")
				{
					Prospective.Controls.MessageBox.Show("You must specify a name for the new solution.", "Solution Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
					NewSolutionOptionsName.Focus();
					return;
				}
			}

			if (NewProject.IsChecked == true)
			{
				if (NewProjectOptionsName.Text == "")
				{
					Prospective.Controls.MessageBox.Show("You must specify a name for the new project.", "Project Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
					NewProjectOptionsName.Focus();
					return;
				}
			}

			if (NewDependency.IsChecked == true)
			{
				if (NewDependencyOptionsName.Text == "")
				{
					Prospective.Controls.MessageBox.Show("You must specify a name for the new dependency project.", "Project Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
					NewDependencyOptionsName.Focus();
					return;
				}
			}

			//Verify the names don't conflict with existing projects
			if (NewSolution.IsChecked == true)
			{
				if (Globals.Projects != null)
				{
					if (NewProject.IsChecked == true)
					{
						foreach (WCFArchitect.Projects.Project P in Globals.Projects)
							if (P.Name == NewProjectOptionsName.Text)
							{
								Prospective.Controls.MessageBox.Show("The name '" + NewProjectOptionsName.Text + "' already exists in the project. Please specify a different name for the new project.", "Project Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
								NewProjectOptionsName.Focus();
								return;
							}
					}

					if (NewDependency.IsChecked == true)
					{
						foreach (WCFArchitect.Projects.Project P in Globals.Projects)
							if (P.Name == NewDependencyOptionsName.Text)
							{
								Prospective.Controls.MessageBox.Show("The name '" + NewDependencyOptionsName.Text + "' already exists in the project. Please specify a different name for the new project.", "Project Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
								NewDependencyOptionsName.Focus();
								return;
							}
					}
				}
			}
			else
			{
				if (NewProject.IsChecked == true && NewDependency.IsChecked == true)
				{
					if (NewProjectOptionsName.Text == NewDependencyOptionsName.Text)
					{
						Prospective.Controls.MessageBox.Show("The new project name cannot be the same as the new dependency project. Please specify a different name for one of the new projects.", "Project Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
						NewProjectOptionsName.Focus();
						return;
					}
				}
			}

			bool SolutionCreated = true;
			//Create the new projects
			if (NewSolution.IsChecked == true)
			{
				if (NewSolutionOptionsLocation.Text.EndsWith("\\") == false) NewSolutionOptionsLocation.Text += "\\";
				SolutionCreated = Globals.MainScreen.NewSolution(NewSolutionOptionsName.Text, NewSolutionOptionsLocation.Text + NewSolutionOptionsName.Text + ".was");
			}

			if (NewProject.IsChecked == true && SolutionCreated == true)
			{
				Globals.MainScreen.NewProject(NewProjectOptionsName.Text);
			}

			if (NewDependency.IsChecked == true && SolutionCreated == true)
			{
				Globals.MainScreen.NewDependency(NewDependencyOptionsName.Text);
			}

			NewSolution.IsChecked = false;
			NewProject.IsChecked = false;
			NewDependency.IsChecked = false;
		}

		private void UpdateYes_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(Globals.NewVersionPath));
			Application.Current.Shutdown(0);
		}

		private void UpdateNo_Click(object sender, RoutedEventArgs e)
		{
			UpdateAvailable.Visibility = System.Windows.Visibility.Collapsed;
		}
	}
}