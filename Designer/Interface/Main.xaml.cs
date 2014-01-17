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
using Prospective.Server.Licensing;

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

		public ObservableCollection<SolutionItem> ProjectScreens { get { return (ObservableCollection<SolutionItem>)GetValue(ProjectScreensProperty); } set { SetValue(ProjectScreensProperty, value); } }
		public static readonly DependencyProperty ProjectScreensProperty = DependencyProperty.Register("ProjectScreens", typeof(ObservableCollection<SolutionItem>), typeof(Main), new PropertyMetadata(null));

		public ObservableCollection<AvailableUpdateXAML> AvailableUpdates { get { return (ObservableCollection<AvailableUpdateXAML>)GetValue(AvailableUpdatesProperty); } set { SetValue(AvailableUpdatesProperty, value); } }
		public static readonly DependencyProperty AvailableUpdatesProperty = DependencyProperty.Register("AvailableUpdates", typeof(ObservableCollection<AvailableUpdateXAML>), typeof(Main), new PropertyMetadata(null));

		public AvailableUpdateXAML SelectedUpdate { get { return (AvailableUpdateXAML)GetValue(SelectedUpdateProperty); } set { SetValue(SelectedUpdateProperty, value); } }
		public static readonly DependencyProperty SelectedUpdateProperty = DependencyProperty.Register("SelectedUpdate", typeof(AvailableUpdateXAML), typeof(Main), new PropertyMetadata(null));

		private WebClient UpdateDownloader { get; set; }
		private string UpdateStorePath { get; set; }

		private SaveCloseMode CloseMode { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")]
		public static readonly RoutedCommand SelectProjectCommand = new RoutedCommand();

		static Main()
		{
			CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(SelectProjectCommand, OnSelectProjectCommandExecuted));
		}

		public Main()
		{
			CloseMode = SaveCloseMode.None;
			if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 0) Globals.WindowsLevel = Globals.WindowsVersion.WinVista;
			if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1) Globals.WindowsLevel = Globals.WindowsVersion.WinSeven;
			if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 2) Globals.WindowsLevel = Globals.WindowsVersion.WinEight;
			if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 3) Globals.WindowsLevel = Globals.WindowsVersion.WinEightOne;

			ProjectScreens = new ObservableCollection<SolutionItem>();

			InitializeComponent();

			DialogService.Initialize("NETPath", DialogViewer);

			Globals.MainScreen = this;

			//Initialize the Home screen.
			RefreshRecentList();

			//Initialize the Options screen.
			UserProfile = Globals.UserProfile;
			if (Globals.UserProfile.AutomaticBackupsEnabled) AutomaticBackupsEnabled.Content = "Yes";
			AboutVersion.Content = string.Format("Version: {0}", FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion);

			if (Globals.AvailableUpdates != null)
			{
				SystemUpdates.Visibility = Visibility.Visible;
				AvailableUpdates = new ObservableCollection<AvailableUpdateXAML>(Globals.AvailableUpdates);
			}
			UpdateDownloader = null;
		}

		#region - Window Events -

		[System.Reflection.Obfuscation(Feature = "encryptmethod", Exclude = false, StripAfterObfuscation = true)]
		private async void Main_SourceInitialized(object sender, EventArgs e)
		{
#if LICENSE
			if ((Globals.UserProfile.IsTrial && !Globals.UserProfile.IsTrialInfoSet) && Globals.UserProfile.Serial == "TRIAL" && Globals.UserProfile.License == "")
			{
				var lic = new CryptoLicense(Globals.TrialLicense, Globals.LicenseVerification);
				if (lic.Status == LicenseStatus.Valid)
					DialogService.ShowMessageDialog("NETPath", "Begin Your Trial?", "Would you like to install a license or begin your trial?", new DialogAction("Start Trial", StartTrial), new DialogAction("Install License", RetrieveLicense), new DialogAction("Purchase License", PurchaseLicense, false, false, true));
				else
					DialogService.ShowMessageDialog("NETPath", "Expired Trial Notice", "Your trial has expired and you will be unable to generate any code based on the changes made to your project files. If you would like to continue using NETPath please purchase a license by clicking the purchase license button.", new DialogAction("Continue"), new DialogAction("Install License", RetrieveLicense), new DialogAction("Purchase License", PurchaseLicense, false, false, true));
			}
			else if ((Globals.UserProfile.IsTrial && Globals.UserProfile.IsTrialInfoSet) && Globals.UserProfile.Serial == "TRIAL" && Globals.UserProfile.License == "")
			{
				var lic = new CryptoLicense(Globals.TrialLicense, Globals.LicenseVerification);
				if (lic.Status == LicenseStatus.Valid)
					DialogService.ShowMessageDialog("NETPath", "Continue Your Trial?", string.Format("Would you like to install a license key or continue with your trial? Please note that your trial will expire in {0} days.", lic.RemainingUsageDays), new DialogAction("Continue Trial"), new DialogAction("Install License", RetrieveLicense), new DialogAction("Purchase License", PurchaseLicense, false, false, true));
				else
					DialogService.ShowMessageDialog("NETPath", "Expired Trial Notice", "Your trial has expired and you will be unable to generate any code based on the changes made to your project files. If you would like to continue using NETPath please purchase a license by clicking the purchase license button.", new DialogAction("Continue"), new DialogAction("Install License", RetrieveLicense), new DialogAction("Purchase License", PurchaseLicense, false, false, true));
			}
			else if (!await App.CheckForInternetConnection()) return;
#endif
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
			if (Globals.Solution == null) return;
			if (CloseMode == SaveCloseMode.None)
			{
				DialogService.ShowMessageDialog("NETPath", "Save Solution?", "Would you like to save your work before closing?", new DialogAction("Yes", () => { CloseMode = SaveCloseMode.Save; Application.Current.Shutdown(0); }, true), new DialogAction("No", () => { CloseMode = SaveCloseMode.NoSave; Application.Current.Shutdown(0); }), new DialogAction("Cancel", false, true));
				e.Cancel = true;
				return;
			}

			Globals.SaveSolution(CloseMode == SaveCloseMode.Save);
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

		public void ShowHomeScreen()
		{
			SystemMenuHome_Click(null, null);
		}

		private void SystemMenuHome_Click(object sender, RoutedEventArgs e)
		{
			ActiveProjectScreen.Visibility = Visibility.Collapsed;
			HomeScreen.Visibility = Visibility.Visible;
			OptionsScreen.Visibility = Visibility.Collapsed;
			UpdatesScreen.Visibility = Visibility.Collapsed;
		}

		private void SystemMenu_Click(object sender, RoutedEventArgs e)
		{
			SystemMenuHome.ContextMenu.PlacementTarget = SystemMenuHome;
			SystemMenuHome.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
			SystemMenuHome.ContextMenu.IsOpen = true;
		}

		private void SystemMenuOpen_Click(object sender, RoutedEventArgs e)
		{
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			//Select the project
			var ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Open Solution")
			{
				DefaultExtension = ".nps",
				AllowNonFileSystemItems = false,
				EnsurePathExists = true,
				IsFolderPicker = false,
				InitialDirectory = openpath,
				Multiselect = false,
				ShowPlacesList = true
			};
			if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;

			OpenSolution(ofd.FileName);
		}

		private void SystemMenuBuild_Click(object sender, RoutedEventArgs e)
		{
			Globals.BuildSolution();
		}

		private void SystemMenuSave_Click(object sender, RoutedEventArgs e)
		{
			Globals.SaveSolution(true);
		}

		private void SystemMenuClose_Click(object sender, RoutedEventArgs e)
		{
			CloseSolution(true);
		}

		private void SystemMenuOptions_Click(object sender, RoutedEventArgs e)
		{
			ActiveProjectScreen.Visibility = Visibility.Collapsed;
			HomeScreen.Visibility = Visibility.Collapsed;
			OptionsScreen.Visibility = Visibility.Visible;
			UpdatesScreen.Visibility = Visibility.Collapsed;
		}

		private void SystemUpdates_Click(object Sender, RoutedEventArgs E)
		{
			ActiveProjectScreen.Visibility = Visibility.Collapsed;
			HomeScreen.Visibility = Visibility.Collapsed;
			OptionsScreen.Visibility = Visibility.Collapsed;
			UpdatesScreen.Visibility = Visibility.Visible;
		}

		private void SystemMenuExit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		#endregion

		#region - Projects -

		internal void Projects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
			{
				var t = new SolutionItem(e.NewItems[0] as Projects.Project);
				ProjectScreens.Add(t);
				if (t.Project.IsSelected)
					t.Command.Execute(t.Content);
			}
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				foreach (SolutionItem pi in ProjectScreens.Cast<SolutionItem>().Where(pi => Equals(pi.Project, e.OldItems[0])))
				{
					ProjectScreens.Remove(pi);
					break;
				}
			}
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
				ProjectScreens.Clear();
		}

		private static void OnSelectProjectCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var t = e.Parameter as Navigator;
			if (t == null) return;
			var s = sender as Main;
			if (s == null) return;

			s.SelectProjectScreen(t);
		}

		internal void SelectProjectScreen(Navigator NewScreen)
		{
			Globals.OpenNavigator = NewScreen;
			SelectedProject = NewScreen;
			ActiveProjectScreen.Visibility = Visibility.Visible;
			HomeScreen.Visibility = Visibility.Collapsed;
			OptionsScreen.Visibility = Visibility.Collapsed;

			foreach (SolutionItem pi in ProjectScreens)
				pi.IsSelected = NewScreen.Project.IsSelected = Equals(pi.Project, NewScreen.Project);
		}

		private void ScrollScreenButtonsLeft_Click(object Sender, RoutedEventArgs E)
		{
			ScrollButtonsViewer.LineLeft();
		}

		private void ScrollScreenButtonsRight_Click(object Sender, RoutedEventArgs E)
		{
			ScrollButtonsViewer.LineRight();
		}

		#endregion

		#region - Home -

		private void NewSolution_Click(object sender, RoutedEventArgs e)
		{
			var np = new Dialogs.NewSolution();
			DialogService.ShowContentDialog(null, "New Solution", np, new DialogAction("Create", np.Create, true), new DialogAction("Cancel", false, true));
		}

		private void AddProject_Click(object sender, RoutedEventArgs e)
		{
			var np = new Dialogs.NewProject(typeof(Projects.Project));
			DialogService.ShowContentDialog(null, "New Project", np, new DialogAction("Create", np.Create, true), new DialogAction("Cancel", false, true));
		}

		private void AddExistingProject_Click(object Sender, RoutedEventArgs E)
		{
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			//Select the project
			var ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select Project")
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

			Globals.Projects.Add(Projects.Project.Open(Globals.SolutionPath, ofd.FileName));
			Globals.SaveSolution(false);
		}

		public void RefreshRecentList()
		{
			if (Globals.UserProfile.ImportantProjects.Count > 0) Globals.UserProfile.ImportantProjects.Sort((p1, p2) => p2.LastAccessed.CompareTo(p1.LastAccessed));
			if (Globals.UserProfile.RecentProjects.Count > 0) Globals.UserProfile.RecentProjects.Sort((p1, p2) => p2.LastAccessed.CompareTo(p1.LastAccessed));

			ImportantProjectsBlock.Visibility = Visibility.Collapsed;
			ImportantProjectsList.Children.Clear();
			RecentProjectsList.Children.Clear();

			foreach (RecentSolution rp in Globals.UserProfile.ImportantProjects)
			{
				ImportantProjectsBlock.Visibility = Visibility.Visible;
				var nrpi = new RecentProjectItem(rp, true);
				ImportantProjectsList.Children.Add(nrpi);
			}
			foreach (RecentSolution rp in Globals.UserProfile.RecentProjects)
			{
				var nrpi = new RecentProjectItem(rp, false);
				RecentProjectsList.Children.Add(nrpi);
			}
		}

		private void HelpLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			var hlink = sender as Hyperlink;
			if (hlink != null)
			{
				string navigateUri = hlink.NavigateUri.ToString();
				Process.Start(new ProcessStartInfo(navigateUri));
			}
			e.Handled = true;
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
			Globals.BackupTimer = new System.Threading.Timer(Globals.BackupSolution, null, (long)UserProfile.AutomaticBackupsInterval.TotalMilliseconds, (long)UserProfile.AutomaticBackupsInterval.TotalMilliseconds);
		}

		#endregion

		#region - Updates -

		private void AvailableUpdatesList_SelectionChanged(object Sender, SelectionChangedEventArgs E)
		{
			SelectedUpdate = AvailableUpdatesList.SelectedItem as AvailableUpdateXAML;
			if (UpdateDownloader != null)
				UpdateDownloader.CancelAsync();
			UpdateStorePath = "";
		}

		private void BeginUpdate_Click(object Sender, RoutedEventArgs E)
		{
			using (UpdateDownloader = new WebClient())
			{
				UpdateDownloader.DownloadFileCompleted += Completed;
				UpdateDownloader.DownloadProgressChanged += ProgressChanged;
				UpdateStorePath = Path.Combine(Globals.UpdateStore, string.Format("{0}.exe", Guid.NewGuid()));
				DownloadProgressPanel.Visibility = Visibility.Visible;

				try
				{
					UpdateDownloader.DownloadFileAsync(new Uri(SelectedUpdate.URL), UpdateStorePath);
				}
				catch (Exception ex)
				{
					DialogService.ShowMessageDialog("NETPath", "Problem Downloading Update", "NETpath has encountered a problem downloading the selected update. Please restart NETPath and try again. If the problem persists, please contacts us at support@prospectivesoftware.com", new DialogAction("OK", true));
				}
			}
		}

		private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			DownloadProgressBar.Value = e.ProgressPercentage;
			DownloadProgress.Text = string.Format("{0}% Downloaded", e.ProgressPercentage);
		}

		private void Completed(object sender, AsyncCompletedEventArgs e)
		{
			DownloadProgressPanel.Visibility = Visibility.Collapsed;
			if (e.Cancelled)
			{
				File.Delete(UpdateStorePath);       // Delete the incomplete file if the download is canceled
				return;
			}

			Process.Start(new ProcessStartInfo(UpdateStorePath));
			CloseMode = SaveCloseMode.Save;
			Close();
		}
		#endregion

		#region - Solutions -

		public void NewSolution(string Name, string Path)
		{
			if (Globals.Solution != null)
				CloseSolution(true, false, () => { Projects.Solution.Save(new Projects.Solution(Name), Path); Globals.OpenSolution(Path, OpenSolutionFinished); }, () => { Projects.Solution.Save(new Projects.Solution(Name), Path); Globals.OpenSolution(Path, OpenSolutionFinished); });
			else
			{
				Projects.Solution.Save(new Projects.Solution(Name), Path);
				Globals.OpenSolution(Path, OpenSolutionFinished);
			}
		}

		public void NewProject(string Name, string Path)
		{
			var NP = new Projects.Project(Name);
			NP.IsSelected = true;
			Globals.Solution.Projects.Add(Globals.GetRelativePath(Globals.SolutionPath, Path));
			Projects.Project.Save(NP, Path);
			Globals.Projects.Add(Projects.Project.Open(Globals.SolutionPath, Path));
			Globals.SaveSolution(false);
		}

		public void OpenSolution(string Path)
		{
			if (Globals.Solution != null)
				CloseSolution(true, false, () => Globals.OpenSolution(Path, OpenSolutionFinished), () => Globals.OpenSolution(Path, OpenSolutionFinished));
			else
				Globals.OpenSolution(Path, OpenSolutionFinished);
		}

		public void OpenSolutionFinished(bool Success)
		{
			if (Success == false) return;

			//Determine if there is already a recent entry. Create one if false. Update the current one if true.
			bool projectHasRecentEntry = false;
			foreach (RecentSolution rp in Globals.UserProfile.ImportantProjects.Where(Rp => Rp.Path == Globals.SolutionPath))
			{
				projectHasRecentEntry = true;
				rp.LastAccessed = DateTime.Now;
				Globals.SolutionInfo = rp;
			}
			foreach (RecentSolution rp in Globals.UserProfile.RecentProjects.Where(Rp => Rp.Path == Globals.SolutionPath))
			{
				projectHasRecentEntry = true;
				rp.LastAccessed = DateTime.Now;
				Globals.SolutionInfo = rp;
			}
			if (projectHasRecentEntry == false)
			{
				var nrp = new RecentSolution(Globals.Solution.Name, Globals.SolutionPath);
				Globals.UserProfile.RecentProjects.Add(nrp);
				Globals.SolutionInfo = nrp;
			}

			RefreshRecentList();

			//Select the first screen if any project were loaded.
			if (Globals.Projects.Count > 0)
			{
				var t = ProjectScreens[0] as SolutionItem;
				if (t != null)
					SelectProjectScreen(t.Content as Navigator);
			}

			AddProject.IsEnabled = true;
			AddExistingProject.IsEnabled = true;
			SystemProjectMenu.Visibility = Visibility.Visible;
			ScreenButtonsPanel.Visibility = Visibility.Visible;
			Title = Globals.Solution.Name + " - NETPath";
		}

		public void CloseSolution(bool AskBeforeClose = false, bool Closing = false, Action ContinueYes = null, Action ContinueNo = null)
		{
			Globals.IsClosing = true;

			if (Globals.Solution == null) return;
			if (AskBeforeClose)
			{
				DialogService.ShowMessageDialog(null, "Continue?", "In order to perform the requested action, the current project will be saved and closed. Would you like to continue?", new DialogAction("Yes", () => { Globals.CloseSolution(); CloseSolutionFinished(); if (ContinueYes != null) ContinueYes(); }, true), new DialogAction("No", () => { Globals.CloseSolution(); CloseSolutionFinished(); if (ContinueNo != null) ContinueNo(); }), new DialogAction("Cancel", false, true));
			}
			else
			{
				if (Closing)
				{
					DialogService.ShowMessageDialog(null, "Save Solution?", "Would you like to save your work?", new DialogAction("Yes", () => { Globals.CloseSolution(); CloseSolutionFinished(); if (ContinueYes != null) ContinueYes(); }, true), new DialogAction("No", () => { Globals.CloseSolution(); CloseSolutionFinished(); if (ContinueYes != null) ContinueNo(); }), new DialogAction("Cancel", false, true));
				}
				else
				{
					Globals.CloseSolution();
					CloseSolutionFinished();
				}
			}
		}

		public void CloseSolutionFinished()
		{
			Globals.SolutionInfo = null;
			AddProject.IsEnabled = false;
			AddExistingProject.IsEnabled = false;
			SystemProjectMenu.Visibility = Visibility.Collapsed;
			ScreenButtonsPanel.Visibility = Visibility.Collapsed;
			Title = "NETPath";

			ActiveProjectScreen.Visibility = Visibility.Collapsed;
			HomeScreen.Visibility = Visibility.Visible;
			OptionsScreen.Visibility = Visibility.Collapsed;

			if (!string.IsNullOrEmpty(Globals.SolutionPath))
				System.IO.File.Delete(System.IO.Path.ChangeExtension(Globals.SolutionPath, ".bak"));

			Globals.Solution = null;

			Globals.IsClosing = false;
		}

		#endregion

		private void ScreenButtonsList_OnClick(object Sender, RoutedEventArgs E)
		{
			ScreenButtonsList.ContextMenu = new ContextMenu();
			foreach (var p in Globals.Projects)
			{
				var t = new MenuItem();
				t.Header = p.Name;
				t.Tag = p;
				t.Click += ScreenButtonsList_OnSelect;
				ScreenButtonsList.ContextMenu.Items.Add(t);
			}
			ScreenButtonsList.ContextMenu.PlacementTarget = ScreenButtonsList;
			ScreenButtonsList.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
			ScreenButtonsList.ContextMenu.IsOpen = true;
		}

		private void ScreenButtonsList_OnSelect(object Sender, RoutedEventArgs E)
		{
			var t = Sender as MenuItem;
			if (t == null) return;
			SelectProjectScreen(new Navigator(t.Tag as Projects.Project));

			foreach (SolutionItem pi in ProjectScreens)
			{
				var project = t.Tag as Projects.Project;
				if (project != null && pi.Project.ID == project.ID)
				{
					var x = ScrollButtonItems.ItemContainerGenerator.ContainerFromItem(pi) as SolutionItem;
					x.BringIntoView();
				}
			}
		}

		private void ScrollButtonsViewer_ScrollChanged(object Sender, ScrollChangedEventArgs E)
		{
			if (ScrollButtonsViewer.ExtentWidth > ScrollButtonsViewer.ViewportWidth)
			{
				ScrollScreenButtonsLeft.Visibility = Visibility.Visible;
				ScrollScreenButtonsRight.Visibility = Visibility.Visible;
			}
			else
			{
				ScrollScreenButtonsLeft.Visibility = Visibility.Collapsed;
				ScrollScreenButtonsRight.Visibility = Visibility.Collapsed;
			}

			if (ScrollButtonsViewer.HorizontalOffset <= 1)
				ScrollScreenButtonsLeft.IsEnabled = false;
			else
				ScrollScreenButtonsLeft.IsEnabled = true;

			if (ScrollButtonsViewer.HorizontalOffset + ScrollButtonsViewer.ViewportWidth >= ScrollButtonsViewer.ExtentWidth)
				ScrollScreenButtonsRight.IsEnabled = false;
			else
				ScrollScreenButtonsRight.IsEnabled = true;
		}
	}

	internal partial class SolutionItem : Button
	{
		public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(SolutionItem), new PropertyMetadata(false));

		public object Header { get { return GetValue(HeaderProperty); } set { SetValue(HeaderProperty, value); } }
		public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(SolutionItem), new PropertyMetadata(new object()));

		internal Projects.Project Project { get; private set; }

		public SolutionItem(Projects.Project Project)
		{
			this.Project = Project;
			Header = Project.Name.ToUpper();
			Content = new Navigator(Project);
			Command = Main.SelectProjectCommand;
		}
	}

	internal partial class RecentProjectItem : Control
	{
		public RecentSolution Data;
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

		public RecentProjectItem(RecentSolution Data, bool IsImportant)
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
			Globals.SolutionInfo = Data;
			Globals.MainScreen.OpenSolution(Data.Path);
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