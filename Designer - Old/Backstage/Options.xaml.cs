using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
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
using Microsoft.Win32;

namespace WCFArchitect.Backstage
{
	internal partial class Options : Grid
	{
		private WCFArchitect.Options.UserProfileReferencePath SelectedReferencePath { get; set; }

		public WCFArchitect.Options.UserProfile UserProfile { get; set; }

		public WCFArchitect.Options.License LicenseKey { get; set; }

		public Prospective.Server.Licensing.LicenseInfoWPF LicenseInfo { get { return (Prospective.Server.Licensing.LicenseInfoWPF)GetValue(LicenseInfoProperty); } set { SetValue(LicenseInfoProperty, value); } }
		public static readonly DependencyProperty LicenseInfoProperty = DependencyProperty.Register("LicenseInfo", typeof(Prospective.Server.Licensing.LicenseInfoWPF), typeof(Options));

		public Options()
		{
			UserProfile = Globals.UserProfile;
			LicenseKey = Globals.LicenseKey;
			LicenseInfo = Globals.LicenseInfo;

			InitializeComponent();
			
			AboutVersion.Content = "Version " + System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
			DefaultProjectFolder.Text = Globals.UserProfile.DefaultProjectFolder;

			ReferencePathsList.ItemsSource = Globals.UserProfile.ReferencePaths;

			if (Globals.UserProfile.AutomaticBackupsEnabled == true)
			{
				AutomaticBackupsEnabled.Content = "Yes";
				AutomaticBackupsInterval.IsEnabled = true;
			}

			if (Globals.LicenseKey.Authorization == "" || Globals.LicenseKey.Authorization == null) KeyActivation.IsChecked = false;
			else KeyActivation.IsChecked = true;

#if TRIAL
			ProductTitle.Content = "Prospective Software WCF Architect Trial";
			KeyGrid.Visibility = System.Windows.Visibility.Collapsed;
			AuthGrid.Visibility = System.Windows.Visibility.Collapsed;
#elif STANDARD
			ProductTitle.Content = "Prospective Software WCF Architect Standard";
#elif PROFESSIONAL
			ProductTitle.Content = "Prospective Software WCF Architect Professional";
#else
			ProductTitle.Content = "Prospective Software WCF Architect Developer";
			KeyGrid.Visibility = System.Windows.Visibility.Collapsed;
			AuthGrid.Visibility = System.Windows.Visibility.Collapsed;
#endif

			BitmapImage logo = new BitmapImage();
			logo.BeginInit();
#if TRIAL
			logo.UriSource = new Uri("pack://application:,,,/WCFArchitect;component/Icons/Odd/FullLogoTrial.png");
#elif STANDARD
			logo.UriSource = new Uri("pack://application:,,,/WCFArchitect;component/Icons/Odd/FullLogoStandard.png");
#elif PROFESSIONAL
			logo.UriSource = new Uri("pack://application:,,,/WCFArchitect;component/Icons/Odd/FullLogoProfessional.png");
#else
			logo.UriSource = new Uri("pack://application:,,,/WCFArchitect;component/Icons/Odd/FullLogoDeveloper.png");
#endif
			logo.EndInit();
			SKULevel.Source = logo;
		}

		private void DefaultProjectFolderBrowse_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				if (!(Globals.UserProfile.DefaultProjectFolder == "" || Globals.UserProfile.DefaultProjectFolder == null)) openpath = Globals.UserProfile.DefaultProjectFolder;

				Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select Default Project Directory");
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
			catch
			{
				string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				if (!(Globals.UserProfile.DefaultProjectFolder == "" || Globals.UserProfile.DefaultProjectFolder == null)) openpath = Globals.UserProfile.DefaultProjectFolder;

				OpenFileDialog ofd = new OpenFileDialog();
				ofd.InitialDirectory = openpath;
				ofd.CheckFileExists = false;
				ofd.FileName = "_";
				ofd.DefaultExt = "_";
				ofd.CheckPathExists = false;
				ofd.Title = "Select Default Project Directory";
				if (ofd.ShowDialog().Value == false) return;

				Globals.UserProfile.DefaultProjectFolder = new System.IO.FileInfo(ofd.FileName).Directory.FullName + "\\";
				DefaultProjectFolder.Text = Globals.UserProfile.DefaultProjectFolder;
			}
		}

		private void ResetEnvironment_Click(object sender, RoutedEventArgs e)
		{
			if (Prospective.Controls.MessageBox.Show("This action will reset your environment to its default state. Are you sure you want to proceed?", "Environment Reset", MessageBoxButton.YesNo) == MessageBoxResult.No) return;

			Globals.UserProfile.IsDockConfigurationAvailable = true;
			Globals.UserProfile.DockSolutionNavigatorHeight = 200;
			Globals.UserProfile.DockSolutionNavigatorWidth = 200;
			Globals.UserProfile.DockSolutionNavigatorMode = C1.WPF.Docking.DockMode.Docked;
			Globals.UserProfile.DockSolutionNavigatorPosition = C1.WPF.Dock.Left;
			Globals.UserProfile.DockErrorListHeight = 200;
			Globals.UserProfile.DockErrorListWidth = 200;
			Globals.UserProfile.DockErrorListMode = C1.WPF.Docking.DockMode.Sliding;
			Globals.UserProfile.DockErrorListPosition = C1.WPF.Dock.Bottom;
			Globals.UserProfile.DockOutputHeight = 200;
			Globals.UserProfile.DockOutputWidth = 200;
			Globals.UserProfile.DockOutputMode = C1.WPF.Docking.DockMode.Sliding;
			Globals.UserProfile.DockOutputPosition = C1.WPF.Dock.Bottom;
			Globals.MainScreen.LoadDockLayout();
			TabMaximumWidth.Value = 200;
			TabPosition.SelectedIndex = 0;

			Prospective.Controls.MessageBox.Show("Environment reset completed. Please restart the software for the changes to take effect", "Environment Reset", MessageBoxButton.OK);
		}

		private void ReferencePathBrowse_Click(object sender, RoutedEventArgs e)
		{
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (!(Globals.UserProfile.DefaultProjectFolder == "" || Globals.UserProfile.DefaultProjectFolder == null)) openpath = Globals.UserProfile.DefaultProjectFolder;
			try
			{
				Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select a Reference Path");
				ofd.AllowNonFileSystemItems = false;
				ofd.EnsurePathExists = true;
				ofd.IsFolderPicker = true;
				ofd.InitialDirectory = openpath;
				ofd.Multiselect = false;
				ofd.ShowPlacesList = true;
				if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;

				ReferencePath.Text = ofd.FileName;
			}
			catch
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.InitialDirectory = openpath;
				ofd.FileName = "_";
				ofd.DefaultExt = ".dll";
				ofd.Filter = "DLL files (*.dll)|*.dll";
				ofd.CheckPathExists = true;
				ofd.Title = "Select a Reference Path";
				if (ofd.ShowDialog().Value == false) return;

				ReferencePath.Text = new System.IO.FileInfo(ofd.FileName).Directory.FullName + "\\";
			}
		}

		private void ReferencePath_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (ReferencePath.Text == "" || ReferencePath.Text == null) ReferencePathAdd.IsEnabled = false;
			else ReferencePathAdd.IsEnabled = true;
		}

		private void ReferencePathAdd_Click(object sender, RoutedEventArgs e)
		{
			Globals.UserProfile.ReferencePaths.Add(new WCFArchitect.Options.UserProfileReferencePath(ReferencePath.Text));
			ReferencePath.Text = "";
		}

		#region - Reference Path Item Handlers -

		private void ReferencePath_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Up) if (ReferencePathsList.SelectedIndex == 0) ReferencePathsList.SelectedIndex = Globals.UserProfile.ReferencePaths.Count - 1; else ReferencePathsList.SelectedIndex--;
			if (e.Key == System.Windows.Input.Key.Down) if (ReferencePathsList.SelectedIndex == Globals.UserProfile.ReferencePaths.Count - 1) ReferencePathsList.SelectedIndex = 0; else ReferencePathsList.SelectedIndex++;

			if (e.Key == System.Windows.Input.Key.Up || e.Key == System.Windows.Input.Key.Down)
			{
				ListBoxItem lbi = ReferencePathsList.ItemContainerGenerator.ContainerFromIndex(ReferencePathsList.SelectedIndex) as ListBoxItem;
				ContentPresenter cp = Globals.GetVisualChild<ContentPresenter>(lbi);
				Prospective.Controls.TextBox ts = ReferencePathsList.ItemTemplate.FindName("ReferencePath", cp) as Prospective.Controls.TextBox;
				if (ts == null) return;
				ts.Focus();
			}
		}

		private void ReferencePathFramework_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Up) if (ReferencePathsList.SelectedIndex == 0) ReferencePathsList.SelectedIndex = Globals.UserProfile.ReferencePaths.Count - 1; else ReferencePathsList.SelectedIndex--;
			if (e.Key == System.Windows.Input.Key.Down) if (ReferencePathsList.SelectedIndex == Globals.UserProfile.ReferencePaths.Count - 1) ReferencePathsList.SelectedIndex = 0; else ReferencePathsList.SelectedIndex++;

			if (e.Key == System.Windows.Input.Key.Up || e.Key == System.Windows.Input.Key.Down)
			{
				ListBoxItem lbi = ReferencePathsList.ItemContainerGenerator.ContainerFromIndex(ReferencePathsList.SelectedIndex) as ListBoxItem;
				ContentPresenter cp = Globals.GetVisualChild<ContentPresenter>(lbi);
				Prospective.Controls.TextBox ts = ReferencePathsList.ItemTemplate.FindName("ReferencePathFramework", cp) as Prospective.Controls.TextBox;
				if (ts == null) return;
				ts.Focus();
			}
		}

		private void ReferencePathDelete_Click(object sender, RoutedEventArgs e)
		{
			WCFArchitect.Options.UserProfileReferencePath OP = ReferencePathsList.SelectedItem as WCFArchitect.Options.UserProfileReferencePath;
			if (OP == null) return;

			if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the following Reference Path:" + Environment.NewLine + OP.Path, "Delete Reference Path", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				Globals.UserProfile.ReferencePaths.Remove(OP);
			}
		}

		private void ReferencePathUpdate_Click(object sender, RoutedEventArgs e)
		{
			WCFArchitect.Options.UserProfileReferencePath OP = ReferencePathsList.SelectedItem as WCFArchitect.Options.UserProfileReferencePath;
			if (OP == null) return;

			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
			if (!(OP.Path == "" || OP.Path == null)) openpath = OP.Path;
			try
			{
				Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select a Reference Path");
				ofd.AllowNonFileSystemItems = false;
				ofd.EnsurePathExists = true;
				ofd.IsFolderPicker = true;
				ofd.InitialDirectory = openpath;
				ofd.Multiselect = false;
				ofd.ShowPlacesList = true;
				if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;

				OP.Path = ofd.FileName;
			}
			catch
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.InitialDirectory = openpath;
				ofd.FileName = "_";
				ofd.DefaultExt = ".dll";
				ofd.Filter = "DLL files (*.dll)|*.dll";
				ofd.CheckPathExists = true;
				if (ofd.ShowDialog().Value == false) return;

				OP.Path = new System.IO.FileInfo(ofd.FileName).Directory.FullName + "\\";
			}
		}

		#endregion

		private void SvcUtilPathBrowse_Click(object sender, RoutedEventArgs e)
		{
			string FileName = "";
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
			if (!(UserProfile.SvcUtilPath == null || UserProfile.SvcUtilPath == null)) openpath = (UserProfile.SvcUtilPath);

			try
			{
				Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select Path to SvcUtil");
				ofd.AllowNonFileSystemItems = false;
				ofd.EnsurePathExists = true;
				ofd.IsFolderPicker = false;
				ofd.DefaultExtension = ".exe";
				ofd.DefaultFileName="SvcUtil";
				ofd.Filters.Add(new Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter("EXE files", ".exe"));
				ofd.EnsurePathExists = true;
				ofd.InitialDirectory = openpath;
				ofd.Multiselect = false;
				ofd.ShowPlacesList = true;
				if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;
				FileName = ofd.FileName;
			}
			catch
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.InitialDirectory = openpath;
				ofd.FileName = "SvcUtil";
				ofd.DefaultExt = ".exe";
				ofd.Filter = "EXE files (*.exe)|*.exe";
				ofd.CheckPathExists = true;
				ofd.Title = "Select Path to SvcUtil";
				if (ofd.ShowDialog().Value == false) return; 
				FileName = ofd.FileName;
			}

			SvcUtilPath.Text = FileName;
			UserProfile.SvcUtilPath = FileName;
		}

		private void MILink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			string navigateUri = MILink.NavigateUri.ToString();
			Process.Start(new ProcessStartInfo(navigateUri));
			e.Handled = true;
		}

		private void OILLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			string navigateUri = OILLink.NavigateUri.ToString();
			Process.Start(new ProcessStartInfo(navigateUri));
			e.Handled = true;
		}

		private void SLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			string navigateUri = SLink.NavigateUri.ToString();
			Process.Start(new ProcessStartInfo(navigateUri));
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

		private void AutomaticBackupsInterval_ValueChanged(object sender, C1.WPF.DateTimeEditors.NullablePropertyChangedEventArgs<TimeSpan> e)
		{
			if (IsLoaded == false) return;
			if (e.NewValue.HasValue == false) { UserProfile.AutomaticBackupsInterval = new TimeSpan(0, 5, 0); return; }
			if (e.NewValue.Value.TotalMinutes < 1) { UserProfile.AutomaticBackupsInterval = new TimeSpan(0, 1, 0); return; }
			UserProfile.AutomaticBackupsInterval = e.NewValue.Value;
			if (Globals.ProjectSpace.State == MP.Karvonite.ObjectSpaceState.Open)
			{
				Globals.BackupTimer.Dispose();
				Globals.BackupTimer = new System.Threading.Timer(new System.Threading.TimerCallback(Globals.BackupProjectSpace), null, (long)UserProfile.AutomaticBackupsInterval.TotalMilliseconds, (long)UserProfile.AutomaticBackupsInterval.TotalMilliseconds);
			}
		}

		private void TabMaximumWidth_ValueChanged(object sender, C1.WPF.PropertyChangedEventArgs<double> e)
		{
			if (IsLoaded == false) return;
			Globals.UserProfile.TabMaximumWidth = (int)e.NewValue;
			Globals.MainScreen.UpdateTabsLayout();
		}

		private void TabPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Globals.MainScreen.UpdateTabsLayout();
		}

		private void Key_GotFocus(object sender, RoutedEventArgs e)
		{
			Key.SelectionLength = 0;
			Key.SelectionStart = 0;
		}

		private void KeyActivation_Checked(object sender, RoutedEventArgs e)
		{
			KeyActivation.IsChecked = true;
			KeyActivation.Content = "Activated";
			if (IsLoaded == false) return;
			Globals.LicenseKey.Key = Key.Value;
			if (WCFArchitect.Options.Licensing.Activate() == false)
				KeyActivation_Unchecked(null, null);
		}

		private void KeyActivation_Unchecked(object sender, RoutedEventArgs e)
		{
			KeyActivation.IsChecked = false;
			KeyActivation.Content = "Activate";
			if (IsLoaded == false) return;
			if (WCFArchitect.Options.Licensing.Deactivate() == false)
				KeyActivation_Checked(null, null);
			Key.Value = Globals.LicenseKey.Key;
		}
	}

	[ValueConversion(typeof(WCFArchitect.Options.UserProfileReferencePathFramework), typeof(int))]
	public class UserProfileReferencePathFrameworkConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			WCFArchitect.Options.UserProfileReferencePathFramework lt = (WCFArchitect.Options.UserProfileReferencePathFramework)value;
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.Any) return 0;
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.NET40) return 1;
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.NET40Client) return 2;
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.NET35) return 3;
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.NET35Client) return 4;
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.NET30) return 5;
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.SL50) return 6;
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.SL40) return 7;
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.SL30) return 8;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return WCFArchitect.Options.UserProfileReferencePathFramework.Any;
			if (lt == 1) return WCFArchitect.Options.UserProfileReferencePathFramework.NET40;
			if (lt == 2) return WCFArchitect.Options.UserProfileReferencePathFramework.NET40Client;
			if (lt == 3) return WCFArchitect.Options.UserProfileReferencePathFramework.NET35;
			if (lt == 4) return WCFArchitect.Options.UserProfileReferencePathFramework.NET35Client;
			if (lt == 5) return WCFArchitect.Options.UserProfileReferencePathFramework.NET30;
			if (lt == 6) return WCFArchitect.Options.UserProfileReferencePathFramework.SL50;
			if (lt == 7) return WCFArchitect.Options.UserProfileReferencePathFramework.SL40;
			if (lt == 8) return WCFArchitect.Options.UserProfileReferencePathFramework.SL30;
			return WCFArchitect.Options.UserProfileReferencePathFramework.Any;
		}
	}

	[ValueConversion(typeof(WCFArchitect.Options.UserProfileReferencePathFramework), typeof(string))]
	public class UserProfileReferencePathFrameworkNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			WCFArchitect.Options.UserProfileReferencePathFramework lt = (WCFArchitect.Options.UserProfileReferencePathFramework)value;
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.Any) return "Any";
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.NET40) return ".NET Framework 4.0";
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.NET40Client) return ".NET Framework 4.0 (Client Profile)";
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.NET35) return ".NET Framework 3.5";
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.NET35Client) return ".NET Framework 3.5 (Client Profile)";
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.NET30) return ".NET Framework 3.0";
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.SL50) return "Silverlight 5.0";
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.SL40) return "Silverlight 4.0";
			if (lt == WCFArchitect.Options.UserProfileReferencePathFramework.SL30) return "Silverlight 3.0";
			return "Any";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string lt = (string)value;
			if (lt == "Any") return WCFArchitect.Options.UserProfileReferencePathFramework.Any;
			if (lt == ".NET Framework 4.0") return WCFArchitect.Options.UserProfileReferencePathFramework.NET40;
			if (lt == ".NET Framework 4.0 (Client Profile)") return WCFArchitect.Options.UserProfileReferencePathFramework.NET40Client;
			if (lt == ".NET Framework 3.5") return WCFArchitect.Options.UserProfileReferencePathFramework.NET35;
			if (lt == ".NET Framework 3.5 (Client Profile)") return WCFArchitect.Options.UserProfileReferencePathFramework.NET35Client;
			if (lt == ".NET Framework 3.0") return WCFArchitect.Options.UserProfileReferencePathFramework.NET30;
			if (lt == "Silverlight 5.0") return WCFArchitect.Options.UserProfileReferencePathFramework.SL50;
			if (lt == "Silverlight 4.0") return WCFArchitect.Options.UserProfileReferencePathFramework.SL40;
			if (lt == "Silverlight 3.0") return WCFArchitect.Options.UserProfileReferencePathFramework.SL30;
			return WCFArchitect.Options.UserProfileReferencePathFramework.Any;
		}
	}
}