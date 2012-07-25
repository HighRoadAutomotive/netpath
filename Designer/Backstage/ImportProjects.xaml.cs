using System;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using MP.Karvonite;
using Microsoft.Win32;

namespace WCFArchitect.Backstage
{
	internal partial class ImportProjects : Window
	{
		public List<WCFArchitect.Projects.Project> SelectedProjects { get; private set; }

		public ImportProjects()
		{
			InitializeComponent();

			string FileName = "";
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (!(Globals.UserProfile.DefaultProjectFolder == "" || Globals.UserProfile.DefaultProjectFolder == null)) openpath = Globals.UserProfile.DefaultProjectFolder;

			try
			{
				Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select a Solution to Import");
				ofd.AllowNonFileSystemItems = true;
				ofd.EnsurePathExists = true;
				ofd.IsFolderPicker = false;
				ofd.InitialDirectory = openpath;
				ofd.Multiselect = false;
				ofd.ShowPlacesList = true;
				ofd.DefaultExtension = ".was";
				ofd.Filters.Add(new Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter("WCF Architect Solution Files", ".was"));
				if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;
				FileName = ofd.FileName;
			}
			catch
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.InitialDirectory = openpath;
				ofd.DefaultExt = ".was";
				ofd.Filter = "WCF Architect Solution Files (*.was)|*.was";
				ofd.Title = "Select a Solution to Import";
				ofd.ShowDialog();
				FileName = ofd.FileName;
			}

			Globals.IsLoading = true;

			//Load the project file
			try
			{
				ObjectSpace os = new ObjectSpace("Projects.kvtmodel", "WCFArchitect");
				if (!System.IO.File.Exists(FileName))
				{
					Prospective.Controls.MessageBox.Show("Unable to located the requested file, please try again.", "Unable to Locate Project File.", MessageBoxButton.OK);
					return;
				}
				os.EnableConcurrency = false;
				os.Open(FileName, ObjectSpaceOpenMode.ReadWrite);

				WCFArchitect.Projects.Solution TS = os.OfType<WCFArchitect.Projects.Solution>().First<WCFArchitect.Projects.Solution>();
				if (TS.ID == Globals.Solution.ID)
				{
					Prospective.Controls.MessageBox.Show("You import a project from the currently loaded project!", "Import Error", MessageBoxButton.OK, MessageBoxImage.Stop);
					CloseWindow_Click(null, null);
				}

				ProjectItems.ItemsSource = new List<WCFArchitect.Projects.Project>(os.OfType<WCFArchitect.Projects.Project>());
			}
			catch
			{
				Prospective.Controls.MessageBox.Show("There was an error loading the selected import solution. Please try again.", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				Globals.IsLoading = false;
			}
		}

		private void Import_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedProjects == null)
			{
				Prospective.Controls.MessageBox.Show("You must select at least one project to import!", "Error Importing Projects", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}

			Globals.IsClosing = true;
			Globals.OpenDocuments.Clear();
			Globals.IsClosing = false;

			foreach (WCFArchitect.Projects.Project P in SelectedProjects)
			{
#if STANDARD
				if(P.GetType() == typeof(WCFArchitect.Projects.DependencyProject)) continue;
#endif
				WCFArchitect.Projects.Project LP = ContainsProject(P.ID);
				if (LP != null)
					Globals.ProjectSpace.Remove(LP);
				Globals.ProjectSpace.Add(P);

				Globals.SaveProjectSpace();
			}

			string Path = Globals.ProjectSpace.FileName;
			Globals.MainScreen.CloseSolution();

			Globals.MainScreen.OpenSolution(Path);

			this.DialogResult = true;
			this.Close();
		}

		private void CloseWindow_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		private void ProjectItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ProjectItems.SelectedItem == null)
			{
				SelectedProjects = null;
				return;
			}
			SelectedProjects = new List<WCFArchitect.Projects.Project>(e.AddedItems.Cast<WCFArchitect.Projects.Project>());
		}

		private WCFArchitect.Projects.Project ContainsProject(Guid ID)
		{
			foreach (WCFArchitect.Projects.Project P in Globals.Projects)
			{
				if (P.ID == ID) return P;
			}
			return null;
		}
	}
}