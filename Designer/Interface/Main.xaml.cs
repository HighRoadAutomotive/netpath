using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;

namespace WCFArchitect.Interface
{
	public partial class Main : Window
	{
		//Message Box Properties
		public string MessageProject { get { return (string)GetValue(MessageProjectProperty); } set { SetValue(MessageProjectProperty, value); } }
		public static readonly DependencyProperty MessageProjectProperty = DependencyProperty.Register("MessageProject", typeof(string), typeof(Main));

		public string MessageCaption { get { return (string)GetValue(MessageCaptionProperty); } set { SetValue(MessageCaptionProperty, value); } }
		public static readonly DependencyProperty MessageCaptionProperty = DependencyProperty.Register("MessageCaption", typeof(string), typeof(Main));

		public object Message { get { return (object)GetValue(MessageProperty); } set { SetValue(MessageProperty, value); } }
		public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(object), typeof(Main));

		public ObservableCollection<Button> MessageActions { get { return (ObservableCollection<Button>)GetValue(MessageActionsProperty); } set { SetValue(MessageActionsProperty, value); } }
		public static readonly DependencyProperty MessageActionsProperty = DependencyProperty.Register("MessageActions", typeof(ObservableCollection<Button>), typeof(Main));

		//Options Properties
		public Prospective.Server.Licensing.LicenseInfoWPF LicenseInfo { get { return (Prospective.Server.Licensing.LicenseInfoWPF)GetValue(LicenseInfoProperty); } set { SetValue(LicenseInfoProperty, value); } }
		public static readonly DependencyProperty LicenseInfoProperty = DependencyProperty.Register("LicenseInfo", typeof(Prospective.Server.Licensing.LicenseInfoWPF), typeof(Main));

		public WCFArchitect.Options.UserProfile UserProfile { get { return (WCFArchitect.Options.UserProfile)GetValue(UserProfileProperty); } set { SetValue(UserProfileProperty, value); } }
		public static readonly DependencyProperty UserProfileProperty = DependencyProperty.Register("UserProfile", typeof(WCFArchitect.Options.UserProfile), typeof(Main));

		public bool IsProcessingMessage { get; private set; }

		public Main()
		{
			if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 0) Globals.WindowsLevel = Globals.WindowsVersion.WinVista;
			if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1) Globals.WindowsLevel = Globals.WindowsVersion.WinSeven;
			if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 2) Globals.WindowsLevel = Globals.WindowsVersion.WinEight;

			MessageActions = new ObservableCollection<Button>();
			
			InitializeComponent();

			//Initialize the Options screen.
			UserProfile = Globals.UserProfile;
			if (Globals.UserProfile.AutomaticBackupsEnabled == true) AutomaticBackupsEnabled.Content = "Yes";
			AboutVersion.Content = "Version " + System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
#if TRIAL
			ProductTitle.Content = "Prospective Software WCF Architect Trial Edition";
#elif WINRT
			ProductTitle.Content = "Prospective Software WCF Architect for Windows Runtime";
#elif PROFESSIONAL
			ProductTitle.Content = "Prospective Software WCF Architect Professional";
#else
			ProductTitle.Content = "Prospective Software WCF Architect Developer";
#endif

			//Set the logo in the options screen.
			BitmapImage logo = new BitmapImage();
			logo.BeginInit();
#if TRIAL
			logo.UriSource = new Uri("pack://application:,,,/WCFArchitect;component/Icons/Odd/FullLogoTrial.png");
#elif WINRT
			logo.UriSource = new Uri("pack://application:,,,/WCFArchitect;component/Icons/Odd/FullLogoWinRT.png");
#elif PROFESSIONAL
			logo.UriSource = new Uri("pack://application:,,,/WCFArchitect;component/Icons/Odd/FullLogoProfessional.png");
#else
			logo.UriSource = new Uri("pack://application:,,,/WCFArchitect;component/Icons/Odd/FullLogoDeveloper.png");
#endif
			logo.EndInit();
			SKULevel.Source = logo;
		}

		#region - Window Events -

		private void Main_SourceInitialized(object sender, EventArgs e)
		{
		}

		private void Main_StateChanged(object sender, EventArgs e)
		{
			if (this.WindowState == System.Windows.WindowState.Maximized)
			{
				if (Globals.WindowsLevel == Globals.WindowsVersion.WinVista || Globals.WindowsLevel == Globals.WindowsVersion.WinSeven)
				{
					WindowBorder.Margin = new Thickness(5, 4, 5, 5);
				}
				else if (Globals.WindowsLevel == Globals.WindowsVersion.WinEight)
				{
					WindowBorder.Margin = new Thickness(8);
				}
				WindowBorder.BorderThickness = new Thickness(0);
				MaximizeWindow.Visibility = System.Windows.Visibility.Collapsed;
				RestoreWindow.Visibility = System.Windows.Visibility.Visible;
			}
			if (this.WindowState == System.Windows.WindowState.Normal)
			{
				WindowBorder.Margin = new Thickness(0);
				WindowBorder.BorderThickness = new Thickness(1);
				MaximizeWindow.Visibility = System.Windows.Visibility.Visible;
				RestoreWindow.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
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
			this.Close();
		}

		private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = System.Windows.WindowState.Minimized;
		}

		private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = System.Windows.WindowState.Maximized;
		}

		private void RestoreWindow_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = System.Windows.WindowState.Normal;
		}

		#endregion

		#region - MessageBox -

		public async void ProcessNextMessage()
		{
			if (Application.Current.Dispatcher.CheckAccess() == true)
				InternalProcessNextMessage();
			else
				await Application.Current.Dispatcher.InvokeAsync(() => InternalProcessNextMessage(), System.Windows.Threading.DispatcherPriority.Normal);
		}

		private void InternalProcessNextMessage()
		{
			if (IsProcessingMessage == true) return;
			IsProcessingMessage = true;
			MessageBox.Visibility = System.Windows.Visibility.Visible;
			MessageActions.Clear();

			if (Globals.Messages.Count > 0)
			{
				MessageBox next = null;
				if (Globals.Messages.TryDequeue(out next) == false)
				{
					IsProcessingMessage = false;
					MessageBox.Visibility = System.Windows.Visibility.Hidden;
					return;
				}

				if (next != null)
				{
					MessageProject = next.Origin.Name;
					MessageCaption = next.Caption;
					Message = next.Message;
					foreach (MessageAction a in next.Actions)
					{
						Button nb = new Button();
						nb.Content = a.Title;
						nb.Tag = a.Action;
						nb.Margin = new Thickness(5);
						nb.Click += MessageAction_Click;
						MessageActions.Add(nb);
					}
				}
			}
		}

		private void MessageAction_Click(object sender, RoutedEventArgs e)
		{
			Button b = sender as Button;
			Action a = b.Tag as Action;
			a.Invoke();

			IsProcessingMessage = false;
			MessageBox.Visibility = System.Windows.Visibility.Hidden;

			if (Globals.Messages.Count > 0)
				ProcessNextMessage();
		}

		#endregion

		#region - System Menu -

		private void SystemMenuHome_Click(object sender, RoutedEventArgs e)
		{
			ActiveProjectScreen.Visibility = System.Windows.Visibility.Collapsed;
			HomeScreen.Visibility = System.Windows.Visibility.Visible;
			OptionsScreen.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void SystemMenu_Click(object sender, RoutedEventArgs e)
		{
			SystemMenu.ContextMenu.PlacementTarget = SystemMenu;
			SystemMenu.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
			SystemMenu.ContextMenu.IsOpen = true;
		}

		private void SystemMenuOpen_Click(object sender, RoutedEventArgs e)
		{
		}

		private void SystemMenuSave_Click(object sender, RoutedEventArgs e)
		{
			ScreenButtons.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void SystemMenuSaveAs_Click(object sender, RoutedEventArgs e)
		{
		}

		private void SystemMenuClose_Click(object sender, RoutedEventArgs e)
		{
		}

		private void SystemMenuOptions_Click(object sender, RoutedEventArgs e)
		{
			ActiveProjectScreen.Visibility = System.Windows.Visibility.Collapsed;
			HomeScreen.Visibility = System.Windows.Visibility.Collapsed;
			OptionsScreen.Visibility = System.Windows.Visibility.Visible;
		}

		private void SystemMenuHelp_Click(object sender, RoutedEventArgs e)
		{
		}

		private void SystemMenuExit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		#endregion

		#region - Home -

		private void NewSolution_Click(object sender, RoutedEventArgs e)
		{

		}

		private void NewProject_Click(object sender, RoutedEventArgs e)
		{

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

		#endregion

		#region - Options -

		private void DefaultProjectFolderBrowse_Click(object sender, RoutedEventArgs e)
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
				Globals.BackupTimer = new System.Threading.Timer(new System.Threading.TimerCallback(Globals.BackupSolution), null, (long)UserProfile.AutomaticBackupsInterval.TotalMilliseconds, (long)UserProfile.AutomaticBackupsInterval.TotalMilliseconds);
			}
		}

		#endregion
	}
}
