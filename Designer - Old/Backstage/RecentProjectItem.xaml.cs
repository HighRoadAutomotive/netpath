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
using System.Windows.Shapes;

namespace WCFArchitect.Backstage
{
	internal partial class RecentProjectItem : Grid
	{
		public WCFArchitect.Options.RecentSolution Data;
		public bool IsImportant = false;

		public RecentProjectItem()
		{
			InitializeComponent();
		}

		public RecentProjectItem(WCFArchitect.Options.RecentSolution Data, bool IsImportant)
		{
			this.Data = Data;
			this.IsImportant = IsImportant;

			InitializeComponent();

			ItemTitle.Text = Data.Name;
			ItemPath.Text = Data.Path;

			if (IsImportant == true)
			{
				BitmapImage flag = new BitmapImage();
				flag.BeginInit();
				flag.UriSource = new Uri("pack://application:,,,/WCFArchitect;component/Icons/X24/flag-red.png");
				flag.EndInit();
				IsImportantFlag.Source = flag;
			}
		}

		private void FlagImportant_Click(object sender, RoutedEventArgs e)
		{
			if (IsImportant == true)
			{
				Globals.UserProfile.ImportantProjects.Remove(Data);
				Globals.UserProfile.RecentProjects.Add(Data);

				BitmapImage flag = new BitmapImage();
				flag.BeginInit();
				flag.UriSource = new Uri("pack://application:,,,/WCFArchitect;component/Icons/X24/flag-red.png");
				flag.EndInit();
				IsImportantFlag.Source = flag;
			}
			else
			{
				Globals.UserProfile.RecentProjects.Remove(Data);
				Globals.UserProfile.ImportantProjects.Add(Data);

				BitmapImage flag = new BitmapImage();
				flag.BeginInit();
				flag.UriSource = new Uri("pack://application:,,,/WCFArchitect;component/Icons/X24/flag-black.png");
				flag.EndInit();
				IsImportantFlag.Source = flag;
			}

			Globals.MainScreen.ProjectScreen.RefreshRecentList();
		}

		private void OpenProject_Click(object sender, RoutedEventArgs e)
		{
			if (!System.IO.File.Exists(Data.Path))
			{
				if (Prospective.Controls.MessageBox.Show("Unable to located the requested file, would you like to remove this project from the list?", "Unable to Locate Project File.", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
					if (IsImportant == true) { Globals.UserProfile.ImportantProjects.Remove(Data); } else { Globals.UserProfile.RecentProjects.Remove(Data); }
				Globals.MainScreen.ProjectScreen.RefreshRecentList();
				return;
			}

			Data.LastAccessed = DateTime.Now;
			Globals.ActiveProjectInfo = Data;
			Globals.MainScreen.OpenSolution(Data.Path);
		}

		private void CMOpenProject_Click(object sender, RoutedEventArgs e)
		{
			OpenProject_Click(null, null);
		}

		private void CMOpenFolder_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.ProcessStartInfo PSI = new System.Diagnostics.ProcessStartInfo(System.IO.Path.GetDirectoryName(Data.Path));
			PSI.UseShellExecute = true;
			System.Diagnostics.Process.Start(PSI);
		}

		private void CMRemove_Click(object sender, RoutedEventArgs e)
		{
			if (IsImportant == true)
			{
				Globals.UserProfile.ImportantProjects.Remove(Data);
			}
			else
			{
				Globals.UserProfile.RecentProjects.Remove(Data);
			}
			Globals.MainScreen.ProjectScreen.RefreshRecentList();
		}
	}
}
