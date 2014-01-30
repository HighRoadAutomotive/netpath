using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Xml;
using Prospective.Controls.Dialogs;
using NETPath.Options;

namespace NETPath.Interface
{
	internal partial class Main : Window
	{
		private enum SaveCloseMode
		{
			None,
			Save,
			NoSave,
		}

		//Project Properties
		public object SelectedProject { get { return GetValue(SelectedProjectProperty); } set { SetValue(SelectedProjectProperty, value); } }
		public static readonly DependencyProperty SelectedProjectProperty = DependencyProperty.Register("SelectedProject", typeof(object), typeof(Main));

		public UserProfile UserProfile { get { return (UserProfile)GetValue(UserProfileProperty); } set { SetValue(UserProfileProperty, value); } }
		public static readonly DependencyProperty UserProfileProperty = DependencyProperty.Register("UserProfile", typeof(UserProfile), typeof(Main));

		public bool IsBuilding { get { return (bool)GetValue(IsBuildingProperty); } set { SetValue(IsBuildingProperty, value); } }
		public static readonly DependencyProperty IsBuildingProperty = DependencyProperty.Register("IsBuilding", typeof(bool), typeof(Main), new PropertyMetadata(false));

		private SaveCloseMode CloseMode { get; set; }

		public Main()
		{
			CloseMode = SaveCloseMode.None;
			if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 0) Globals.WindowsLevel = Globals.WindowsVersion.WinVista;
			if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1) Globals.WindowsLevel = Globals.WindowsVersion.WinSeven;
			if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 2) Globals.WindowsLevel = Globals.WindowsVersion.WinEight;
			if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 3) Globals.WindowsLevel = Globals.WindowsVersion.WinEightOne;

			InitializeComponent();

			DialogService.Initialize("NETPath", DialogViewer);

			Globals.MainScreen = this;

			//Initialize the Home screen.
			RefreshRecentList();

			//Initialize the Options screen.
			UserProfile = Globals.UserProfile;
			if (Globals.UserProfile.AutomaticBackupsEnabled) AutomaticBackupsEnabled.Content = "Yes";
			AboutVersion.Content = string.Format("Version: {0}", FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion);
		}

		#region - Window Events -

		[System.Reflection.Obfuscation(Feature = "encryptmethod", Exclude = false, StripAfterObfuscation = true)]
		private void Main_SourceInitialized(object sender, EventArgs e)
		{
			SystemMenuHome.IsChecked = true;
		}

		private void Main_StateChanged(object sender, EventArgs e)
		{
			if (WindowState == WindowState.Maximized)
			{
				if (Globals.WindowsLevel == Globals.WindowsVersion.WinVista || Globals.WindowsLevel == Globals.WindowsVersion.WinSeven)
				{
					WindowBorder.Margin = new Thickness(5, 4, 5, 5);
				}
				else if (Globals.WindowsLevel == Globals.WindowsVersion.WinEight || Globals.WindowsLevel == Globals.WindowsVersion.WinEightOne)
				{
					WindowBorder.Margin = new Thickness(8);
				}
				WindowBorder.BorderThickness = new Thickness(0);
				MaximizeWindow.Visibility = Visibility.Collapsed;
				RestoreWindow.Visibility = Visibility.Visible;
			}

			if (WindowState != WindowState.Normal) return;
			WindowBorder.Margin = new Thickness(0);
			WindowBorder.BorderThickness = new Thickness(1);
			MaximizeWindow.Visibility = Visibility.Visible;
			RestoreWindow.Visibility = Visibility.Collapsed;
		}

		private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (Globals.Project == null) return;
			if (CloseMode == SaveCloseMode.None)
			{
				DialogService.ShowMessageDialog("NETPath", "Save Solution?", "Would you like to save your work before closing?", new DialogAction("Yes", () => { CloseMode = SaveCloseMode.Save; Application.Current.Shutdown(0); }, true), new DialogAction("No", () => { CloseMode = SaveCloseMode.NoSave; Application.Current.Shutdown(0); }), new DialogAction("Cancel", false, true));
				e.Cancel = true;
				return;
			}

			Globals.SaveProject();
		}

		private void Main_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				e.Handled = true;
			}
		}

		private void CloseWindow_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Maximized;
		}

		private void RestoreWindow_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Normal;
		}

		#endregion

		#region - System Menu -

		private void SystemMenuOpen_Click(object sender, RoutedEventArgs e)
		{
			string openpath = string.IsNullOrWhiteSpace(Globals.UserProfile.DefaultProjectFolder) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : Globals.UserProfile.DefaultProjectFolder;

			//Select the project
			var ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Open Project")
			{
				DefaultExtension = ".npp",
				AllowNonFileSystemItems = false,
				EnsurePathExists = true,
				IsFolderPicker = false,
				InitialDirectory = openpath,
				Multiselect = false,
				ShowPlacesList = true
			};
			if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;

			OpenProject(ofd.FileName);
		}

		private void SystemMenuAddItem_Click(object sender, RoutedEventArgs e)
		{
			Globals.OpenNavigator.AddNewItem();
		}

		private void SystemMenuBuild_Click(object sender, RoutedEventArgs e)
		{
			Globals.OpenNavigator.BuildProject();
		}

		private void SystemMenuSave_Click(object sender, RoutedEventArgs e)
		{
			Globals.SaveProject();
		}

		private void SystemMenuClose_Click(object sender, RoutedEventArgs e)
		{
			CloseProject(true);
		}

		#endregion

		#region - Projects -

		internal void SelectProjectScreen(Navigator NewScreen)
		{
			Globals.OpenNavigator = NewScreen;
			SelectedProject = NewScreen;
			SystemMenuHome.IsChecked = false;
		}

		public void NewProject(string Name, string Path)
		{
			var NP = new Projects.Project(Name);
			Projects.Project.Save(NP, Path);
			Globals.Project = NP;
			SelectProjectScreen(new Navigator(NP));
		}

		public void OpenProject(string Path)
		{
			if (Globals.Project != null)
				CloseProject(true, false, () => Globals.OpenProject(Path, OpenProjectFinished), () => Globals.OpenProject(Path, OpenProjectFinished));
			else
				Globals.OpenProject(Path, OpenProjectFinished);
		}

		public void OpenProjectFinished(bool Success)
		{
			if (Success == false) return;

			//Determine if there is already a recent entry. Create one if false. Update the current one if true.
			bool projectHasRecentEntry = false;
			foreach (RecentProject rp in Globals.UserProfile.ImportantProjects.Where(Rp => Rp.Path == Globals.ProjectPath))
			{
				projectHasRecentEntry = true;
				rp.LastAccessed = DateTime.Now;
				Globals.ProjectInfo = rp;
			}
			foreach (RecentProject rp in Globals.UserProfile.RecentProjects.Where(Rp => Rp.Path == Globals.ProjectPath))
			{
				projectHasRecentEntry = true;
				rp.LastAccessed = DateTime.Now;
				Globals.ProjectInfo = rp;
			}
			if (projectHasRecentEntry == false)
			{
				var nrp = new RecentProject(Globals.Project.Name, Globals.ProjectPath);
				Globals.UserProfile.RecentProjects.Add(nrp);
				Globals.ProjectInfo = nrp;
			}

			RefreshRecentList();

			SelectProjectScreen(new Navigator(Globals.Project));

			SystemProjectMenu.Visibility = Visibility.Visible;
			Title = Globals.Project.Name + " - NETPath";
		}

		public void CloseProject(bool AskBeforeClose = false, bool Closing = false, Action ContinueYes = null, Action ContinueNo = null)
		{
			Globals.IsClosing = true;

			if (Globals.Project == null) return;
			if (AskBeforeClose)
			{
				DialogService.ShowMessageDialog(null, "Continue?", "In order to perform the requested action, the current project will be saved and closed. Would you like to continue?", new DialogAction("Yes", () => { Globals.CloseProject(); CloseProjectFinished(); if (ContinueYes != null) ContinueYes(); }, true), new DialogAction("No", () => { Globals.CloseProject(); CloseProjectFinished(); if (ContinueNo != null) ContinueNo(); }), new DialogAction("Cancel", false, true));
			}
			else
			{
				if (Closing)
				{
					DialogService.ShowMessageDialog(null, "Save Solution?", "Would you like to save your work?", new DialogAction("Yes", () => { Globals.CloseProject(); CloseProjectFinished(); if (ContinueYes != null) ContinueYes(); }, true), new DialogAction("No", () => { Globals.CloseProject(); CloseProjectFinished(); if (ContinueYes != null) ContinueNo(); }), new DialogAction("Cancel", false, true));
				}
				else
				{
					Globals.CloseProject();
					CloseProjectFinished();
				}
			}
		}

		public void CloseProjectFinished()
		{
			Globals.ProjectInfo = null;
			SystemProjectMenu.Visibility = Visibility.Collapsed;
			Title = "NETPath";

			SystemMenuHome.IsChecked = true;

			if (!string.IsNullOrEmpty(Globals.ProjectPath))
				File.Delete(Path.ChangeExtension(Globals.ProjectPath, ".bak"));

			Globals.Project = null;

			Globals.IsClosing = false;
		}

		#endregion

		#region - Home -

		private void AddProject_Click(object sender, RoutedEventArgs e)
		{
			var np = new Dialogs.NewProject(typeof(Projects.Project));
			DialogService.ShowContentDialog(null, "New Project", np, new DialogAction("Create", np.Create, true), new DialogAction("Cancel", false, true));
		}

		public void RefreshRecentList()
		{
			if (Globals.UserProfile.ImportantProjects.Count > 0) Globals.UserProfile.ImportantProjects.Sort((p1, p2) => p2.LastAccessed.CompareTo(p1.LastAccessed));
			if (Globals.UserProfile.RecentProjects.Count > 0) Globals.UserProfile.RecentProjects.Sort((p1, p2) => p2.LastAccessed.CompareTo(p1.LastAccessed));

			ImportantProjectsBlock.Visibility = Visibility.Collapsed;
			ImportantProjectsList.Children.Clear();
			RecentProjectsList.Children.Clear();

			foreach (RecentProject rp in Globals.UserProfile.ImportantProjects)
			{
				ImportantProjectsBlock.Visibility = Visibility.Visible;
				var nrpi = new RecentProjectItem(rp, true);
				ImportantProjectsList.Children.Add(nrpi);
			}
			foreach (RecentProject rp in Globals.UserProfile.RecentProjects)
			{
				var nrpi = new RecentProjectItem(rp, false);
				RecentProjectsList.Children.Add(nrpi);
			}
		}
		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			var hlink = sender as Hyperlink;
			if (hlink != null)
			{
				string navigateUri = hlink.NavigateUri.ToString();
				Process.Start(new ProcessStartInfo(navigateUri));
			}
			e.Handled = true;
		}

		#endregion

		#region - Options -

		private void DefaultProjectFolderBrowse_Click(object sender, RoutedEventArgs e)
		{
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (!string.IsNullOrEmpty(Globals.UserProfile.DefaultProjectFolder)) openpath = Globals.UserProfile.DefaultProjectFolder;

			var ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select Default Project Directory");
			ofd.AllowNonFileSystemItems = false;
			ofd.EnsurePathExists = true;
			ofd.IsFolderPicker = true;
			ofd.InitialDirectory = openpath;
			ofd.Multiselect = false;
			ofd.ShowPlacesList = true;
			if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;

			Globals.UserProfile.DefaultProjectFolder = ofd.FileName;
			DefaultProjectFolder.Text = Globals.UserProfile.DefaultProjectFolder;
		}

		private void SLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			var nr = new Dialogs.ReportSuggestion();
			DialogService.ShowContentDialog(null, "Feature Requests and Trouble Reporting.", nr, new DialogAction("Send Report", nr.SendReport, true), new DialogAction("Close", false, true));
			e.Handled = true;
		}

		private void AutomaticBackupsEnabled_Checked(object sender, RoutedEventArgs e)
		{
			if (IsLoaded == false) return;
			AutomaticBackupsEnabled.Content = "Yes";
			AutomaticBackupsInterval.IsEnabled = true;
		}

		private void AutomaticBackupsEnabled_Unchecked(object sender, RoutedEventArgs e)
		{
			if (IsLoaded == false) return;
			AutomaticBackupsEnabled.Content = "No";
			AutomaticBackupsInterval.IsEnabled = false;
		}

		private void AutomaticBackupsInterval_ValueChanged(DependencyObject D, DependencyPropertyChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			if (e.NewValue == null) { UserProfile.AutomaticBackupsInterval = new TimeSpan(0, 5, 0); return; }
			if (((TimeSpan)e.NewValue).TotalMinutes < 1) { UserProfile.AutomaticBackupsInterval = new TimeSpan(0, 1, 0); return; }
			UserProfile.AutomaticBackupsInterval = (TimeSpan)e.NewValue;
			Globals.BackupTimer.Dispose();
			Globals.BackupTimer = new System.Threading.Timer(Globals.BackupProject, null, (long)UserProfile.AutomaticBackupsInterval.TotalMilliseconds, (long)UserProfile.AutomaticBackupsInterval.TotalMilliseconds);
		}

		#endregion
	}

	internal partial class RecentProjectItem : Control
	{
		public RecentProject Data;
		public bool IsImportant;

		public string ItemTitle { get { return (string)GetValue(ItemTitleProperty); } set { SetValue(ItemTitleProperty, value); } }
		public static readonly DependencyProperty ItemTitleProperty = DependencyProperty.Register("ItemTitle", typeof(string), typeof(RecentProjectItem), new PropertyMetadata(""));

		public string ItemPath { get { return (string)GetValue(ItemPathProperty); } set { SetValue(ItemPathProperty, value); } }
		public static readonly DependencyProperty ItemPathProperty = DependencyProperty.Register("ItemPath", typeof(string), typeof(RecentProjectItem), new PropertyMetadata(""));

		public string ItemFlag { get { return (string)GetValue(ItemFlagProperty); } set { SetValue(ItemFlagProperty, value); } }
		public static readonly DependencyProperty ItemFlagProperty = DependencyProperty.Register("ItemFlag", typeof(string), typeof(RecentProjectItem), new PropertyMetadata("pack://application:,,,/NETPath;component/Icons/X32/FlagRecent.png"));

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")]
		public static readonly RoutedCommand OpenCommand = new RoutedCommand();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")]
		public static readonly RoutedCommand FlagCommand = new RoutedCommand();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")]
		public static readonly RoutedCommand FolderCommand = new RoutedCommand();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")]
		public static readonly RoutedCommand RemoveCommand = new RoutedCommand();

		static RecentProjectItem()
		{
			CommandManager.RegisterClassCommandBinding(typeof(RecentProjectItem), new CommandBinding(OpenCommand, OnOpenCommandExecuted));
			CommandManager.RegisterClassCommandBinding(typeof(RecentProjectItem), new CommandBinding(FlagCommand, OnFlagCommandExecuted));
			CommandManager.RegisterClassCommandBinding(typeof(RecentProjectItem), new CommandBinding(FolderCommand, OnFolderCommandExecuted));
			CommandManager.RegisterClassCommandBinding(typeof(RecentProjectItem), new CommandBinding(RemoveCommand, OnRemoveCommandExecuted));
		}

		private static void OnRemoveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var t = e.Parameter as RecentProjectItem;
			if (t == null) return;

			t.CMRemove();
		}

		private static void OnFolderCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var t = e.Parameter as RecentProjectItem;
			if (t == null) return;

			t.CMOpenFolder();
		}

		private static void OnFlagCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var t = e.Parameter as RecentProjectItem;
			if (t == null) return;

			t.FlagImportant();
		}

		private static void OnOpenCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var t = e.Parameter as RecentProjectItem;
			if (t == null) return;

			t.OpenProject();
		}

		public RecentProjectItem(RecentProject Data, bool IsImportant)
		{
			this.Data = Data;
			this.IsImportant = IsImportant;

			ItemTitle = Data.Name;
			ItemPath = Data.Path;

			if (IsImportant) ItemFlag = "pack://application:,,,/NETPath;component/Icons/X32/FlagImportant.png";
		}

		private void FlagImportant()
		{
			if (IsImportant)
			{
				Globals.UserProfile.ImportantProjects.Remove(Data);
				Globals.UserProfile.RecentProjects.Add(Data);

				ItemFlag = "pack://application:,,,/NETPath;component/Icons/X32/FlagImportant.png";
			}
			else
			{
				Globals.UserProfile.RecentProjects.Remove(Data);
				Globals.UserProfile.ImportantProjects.Add(Data);

				ItemFlag = "pack://application:,,,/NETPath;component/Icons/X32/FlagRecent.png";
			}

			Globals.MainScreen.RefreshRecentList();
		}

		private void OpenProject()
		{
			if (!System.IO.File.Exists(Data.Path))
			{
				DialogService.ShowMessageDialog(null, "Unable to Locate Project File.", "Unable to located the requested file, would you like to remove this project from the list?",
					new DialogAction("Yes", () => { if (IsImportant) { Globals.UserProfile.ImportantProjects.Remove(Data); } else { Globals.UserProfile.RecentProjects.Remove(Data); } Globals.MainScreen.RefreshRecentList(); }, true), new DialogAction("No", false, true));
				return;
			}

			Data.LastAccessed = DateTime.Now;
			Globals.ProjectInfo = Data;
			Globals.MainScreen.OpenProject(Data.Path);
		}

		private void CMOpenFolder()
		{
			var psi = new ProcessStartInfo(System.IO.Path.GetDirectoryName(Data.Path));
			psi.UseShellExecute = true;
			Process.Start(psi);
		}

		private void CMRemove()
		{
			if (IsImportant)
			{
				Globals.UserProfile.ImportantProjects.Remove(Data);
			}
			else
			{
				Globals.UserProfile.RecentProjects.Remove(Data);
			}
			Globals.MainScreen.RefreshRecentList();
		}
	}
}