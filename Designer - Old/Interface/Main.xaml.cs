using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using C1.WPF;
using C1.WPF.Docking;
using MP.Karvonite;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Taskbar;
using WCFArchitect.Themes;

namespace WCFArchitect.Interface
{
	public partial class Main : Window
	{
		public Backstage.Projects ProjectScreen { get; set; }
		public Backstage.ProjectInfo ProjectInfoScreen { get; set; }
		internal Backstage.Options OptionsScreen { get; set; }

		private bool IsNamespaceListUpdating;
		private TreeViewItem LastHitItem;
		private object DragSolutionItem;
		private object DropSolutionItem;
		private Point DragStartPos;
		private Themes.DragAdorner DragAdorner;
		private AdornerLayer DragLayer;
		public bool IsDragging { get; set; }
		public bool DragHasLeftScope { get; set; }

		public bool SolutionNavigatorShowAll { get { return (bool)GetValue(SolutionNavigatorShowAllProperty); } set { SetValue(SolutionNavigatorShowAllProperty, value); } }
		public static readonly DependencyProperty SolutionNavigatorShowAllProperty = DependencyProperty.Register("SolutionNavigatorShowAll", typeof(bool), typeof(Main), new UIPropertyMetadata(true));

		public bool SolutionNavigatorShowNamespaces { get { return (bool)GetValue(SolutionNavigatorShowNamespacesProperty); } set { SetValue(SolutionNavigatorShowNamespacesProperty, value); } }
		public static readonly DependencyProperty SolutionNavigatorShowNamespacesProperty = DependencyProperty.Register("SolutionNavigatorShowNamespaces", typeof(bool), typeof(Main), new UIPropertyMetadata(false));

		public bool SolutionNavigatorShowServices { get { return (bool)GetValue(SolutionNavigatorShowServicesProperty); } set { SetValue(SolutionNavigatorShowServicesProperty, value); } }
		public static readonly DependencyProperty SolutionNavigatorShowServicesProperty = DependencyProperty.Register("SolutionNavigatorShowServices", typeof(bool), typeof(Main), new UIPropertyMetadata(false));

		public bool SolutionNavigatorShowData { get { return (bool)GetValue(SolutionNavigatorShowDataProperty); } set { SetValue(SolutionNavigatorShowDataProperty, value); } }
		public static readonly DependencyProperty SolutionNavigatorShowDataProperty = DependencyProperty.Register("SolutionNavigatorShowData", typeof(bool), typeof(Main), new UIPropertyMetadata(false));

		public bool SolutionNavigatorShowEnums { get { return (bool)GetValue(SolutionNavigatorShowEnumsProperty); } set { SetValue(SolutionNavigatorShowEnumsProperty, value); } }
		public static readonly DependencyProperty SolutionNavigatorShowEnumsProperty = DependencyProperty.Register("SolutionNavigatorShowEnums", typeof(bool), typeof(Main), new UIPropertyMetadata(false));

		public bool IsSwitcherWindowOpen { get { return (bool)GetValue(IsSwitcherWindowOpenProperty); } set { SetValue(IsSwitcherWindowOpenProperty, value); } }
		public static readonly DependencyProperty IsSwitcherWindowOpenProperty = DependencyProperty.Register("IsSwitcherWindowOpen", typeof(bool), typeof(Main), new UIPropertyMetadata(false));

		private bool IsSortingOpenWindows = false;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")] public static RoutedCommand BuildProjectCommand = new RoutedCommand();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")] public static RoutedCommand OutputProjectCommand = new RoutedCommand();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")] public static RoutedCommand AddItemCommand = new RoutedCommand();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")] public static RoutedCommand AddBindingCommand = new RoutedCommand();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")] public static RoutedCommand AddSecurityCommand = new RoutedCommand();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")] public static RoutedCommand AddHostCommand = new RoutedCommand();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")] public static RoutedCommand AddNamespaceCommand = new RoutedCommand();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")] public static RoutedCommand AddServiceCommand = new RoutedCommand();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")] public static RoutedCommand AddDataCommand = new RoutedCommand();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")] public static RoutedCommand AddEnumCommand = new RoutedCommand();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")] public static RoutedCommand DeleteItemCommand = new RoutedCommand();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")] public static RoutedCommand DockModeChangedCommand = new RoutedCommand();

		static Main()
		{
			CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(Main.BuildProjectCommand, OnBuildProjectCommandExecuted));
			CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(Main.OutputProjectCommand, OnOutputProjectCommandExecuted));
			CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(Main.AddItemCommand, OnAddItemCommandExecuted));
			CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(Main.AddBindingCommand, OnAddBindingCommandExecuted));
			CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(Main.AddSecurityCommand, OnAddSecurityCommandExecuted));
			CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(Main.AddHostCommand, OnAddHostCommandExecuted));
			CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(Main.AddNamespaceCommand, OnAddNamespaceCommandExecuted));
			CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(Main.AddServiceCommand, OnAddServiceCommandExecuted));
			CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(Main.AddDataCommand, OnAddDataCommandExecuted));
			CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(Main.AddEnumCommand, OnAddEnumCommandExecuted));
			CommandManager.RegisterClassCommandBinding(typeof(Main), new CommandBinding(Main.DeleteItemCommand, OnDeleteItemCommandExecuted));
		}

		public Main()
		{
			Globals.MainScreen = this;
			Globals.ProjectSpace = new ObjectSpace("Projects.kvtmodel", "WCFArchitect");
			Globals.ProjectScreens = new List<Interface.Project.Project>();

			ProjectScreen = new Backstage.Projects();
			ProjectInfoScreen = new Backstage.ProjectInfo();
			OptionsScreen = new Backstage.Options();

			Globals.CompilerManager = new Compiler.CompilerManager();

			//Initialize the window configuration
			if (Globals.UserProfile.IsWindowMaximized == true)
			{
				int SI = Globals.UserProfile.Monitor;
				if (SI > System.Windows.Forms.SystemInformation.MonitorCount - 1) SI = 0;
				this.Left = System.Windows.Forms.Screen.AllScreens[SI].WorkingArea.Left + 10;
				this.Top = System.Windows.Forms.Screen.AllScreens[SI].WorkingArea.Top + 10;
			}
			else
			{
				this.Left = Globals.UserProfile.WindowX;
				this.Top = Globals.UserProfile.WindowY;
				this.Height = Globals.UserProfile.WindowHeight;
				this.Width = Globals.UserProfile.WindowWidth;
			}

			InitializeComponent();

			//ProjectTabs.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("DocumentData.OpenIndex", System.ComponentModel.ListSortDirection.Descending));
			Globals.OpenDocuments = new ObservableCollectionSortable<Projects.OpenableDocument>();
			Globals.OpenDocuments.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OpenDocuments_CollectionChanged);

			BackstageToggle.IsChecked = true;
			BackstageProjects.Content = ProjectScreen;
			BackstageProjectInfo.Content = ProjectInfoScreen;
			BackstageOptions.Content = OptionsScreen;
			SwitcherDocumentList.ItemsSource = Globals.OpenDocuments;
			NewItem.IsEnabled = false;
			Save.IsEnabled = false;
			BuildSolution.IsEnabled = false;
			BuildProject.IsEnabled = false;
			BuildOutput.IsEnabled = false;

			//Get the current state of the Keyboard and setup the Status Bar accordingly.
			Microsoft.VisualBasic.Devices.Keyboard KB = new Microsoft.VisualBasic.Devices.Keyboard();
			if (KB.ScrollLock == true) KeyboardScrollStatus.Visibility = System.Windows.Visibility.Visible;
			else KeyboardScrollStatus.Visibility = System.Windows.Visibility.Hidden;
			if (KB.CapsLock == true) KeyboardCapsLockStatus.Visibility = System.Windows.Visibility.Visible;
			else KeyboardCapsLockStatus.Visibility = System.Windows.Visibility.Hidden;
			if (KB.NumLock == true) KeyboardNumLockStatus.Visibility = System.Windows.Visibility.Visible;
			else KeyboardNumLockStatus.Visibility = System.Windows.Visibility.Hidden;
			KeyboardInsertStatusOn.Visibility = System.Windows.Visibility.Hidden;
			KeyboardInsertStatusOff.Visibility = System.Windows.Visibility.Visible;

			SolutionNavigatorView.ItemsSource = Globals.Projects;

#if !DEVELOPER
			OptionsScreen.LicenseName.Content = Globals.LicenseInfo.LicenseeName;
			OptionsScreen.LicenseCompany.Content = Globals.LicenseInfo.CompanyName;
#endif

			FindNewSearch_Click(null, null);
			LoadDockLayout();
			UpdateTabsLayout();
		}

		#region - Window -

		private void Window_Activated(object sender, EventArgs e)
		{
			//Get the current state of the Keyboard and setup the Status Bar accordingly.
			Microsoft.VisualBasic.Devices.Keyboard KB = new Microsoft.VisualBasic.Devices.Keyboard();
			if (KB.ScrollLock == true) KeyboardScrollStatus.Visibility = System.Windows.Visibility.Visible;
			else KeyboardScrollStatus.Visibility = System.Windows.Visibility.Hidden;
			if (KB.CapsLock == true) KeyboardCapsLockStatus.Visibility = System.Windows.Visibility.Visible;
			else KeyboardCapsLockStatus.Visibility = System.Windows.Visibility.Hidden;
			if (KB.NumLock == true) KeyboardNumLockStatus.Visibility = System.Windows.Visibility.Visible;
			else KeyboardNumLockStatus.Visibility = System.Windows.Visibility.Hidden;
		}

		private void Window_SourceInitialized(object sender, EventArgs e)
		{
			if (Globals.UserProfile.IsWindowMaximized == true)
				this.WindowState = System.Windows.WindowState.Maximized;

			if (Globals.ArgSolutionPath != "")
			{
				OpenSolution(Globals.ArgSolutionPath);
			}

			//Taskbar Management
			//Create the Build buttons
			if (TaskbarManager.IsPlatformSupported == true)
			{
				Globals.BuildSolutionButton = new ThumbnailToolBarButton(new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/BuildSolution.ico")).Stream), "Build Solution");
				Globals.BuildSolutionButton.Click += new EventHandler<ThumbnailButtonClickedEventArgs>(BSB_Click);
				Globals.BuildSolutionButton.Enabled = false;
				Globals.BuildOutputButton = new ThumbnailToolBarButton(new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/BuildOutput.ico")).Stream), "Build Output");
				Globals.BuildOutputButton.Click += new EventHandler<ThumbnailButtonClickedEventArgs>(BOB_Click);
				Globals.BuildOutputButton.Enabled = false;

				WindowInteropHelper windowInteropHelper = new WindowInteropHelper(this);
				TaskbarManager.Instance.ThumbnailToolBars.AddButtons(windowInteropHelper.Handle, Globals.BuildSolutionButton, Globals.BuildOutputButton);
			}
		}

		private void BSB_Click(object sender, ThumbnailButtonClickedEventArgs e)
		{
			BuildSolution_Click(sender, null);
		}

		private void BOB_Click(object sender, ThumbnailButtonClickedEventArgs e)
		{
			BuildOutput_Click(sender, null);
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Globals.IsClosing = true;
			
			if (Globals.MainScreen.CloseSolution(false, true) == false)
			{
				e.Cancel = true;
				return;
			}

			WindowInteropHelper windowInteropHelper = new WindowInteropHelper(this);
			System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);
			for (int i = 0; i < System.Windows.Forms.Screen.AllScreens.Length; i++)
			{
				if (System.Windows.Forms.Screen.AllScreens[i].DeviceName == screen.DeviceName) Globals.UserProfile.Monitor = i;
			}
			if (this.WindowState == System.Windows.WindowState.Maximized)
			{
				Globals.UserProfile.IsWindowMaximized = true;
			}
			else
			{
				Globals.UserProfile.IsWindowMaximized = false;
				Globals.UserProfile.WindowX = (int)this.Left;
				Globals.UserProfile.WindowY = (int)this.Top;
				Globals.UserProfile.WindowHeight = (int)this.ActualHeight;
				Globals.UserProfile.WindowWidth = (int)this.ActualWidth;
			}

			SaveDockLayout();
		}

		private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			Microsoft.VisualBasic.Devices.Keyboard KB = new Microsoft.VisualBasic.Devices.Keyboard();

			if (e.Key == Key.Scroll)
			{
				if (KB.ScrollLock == true) KeyboardScrollStatus.Visibility = System.Windows.Visibility.Visible;
				else KeyboardScrollStatus.Visibility = System.Windows.Visibility.Hidden;
			}
			else if (e.Key == Key.CapsLock)
			{
				if (KB.CapsLock == true) KeyboardCapsLockStatus.Visibility = System.Windows.Visibility.Visible;
				else KeyboardCapsLockStatus.Visibility = System.Windows.Visibility.Hidden;
			}
			else if (e.Key == Key.NumLock)
			{
				if (KB.NumLock == true) KeyboardNumLockStatus.Visibility = System.Windows.Visibility.Visible;
				else KeyboardNumLockStatus.Visibility = System.Windows.Visibility.Hidden;
			}
			else if (e.Key == Key.Insert && e.IsToggled == true)
			{
				KeyboardInsertStatusOn.Visibility = System.Windows.Visibility.Hidden;
				KeyboardInsertStatusOff.Visibility = System.Windows.Visibility.Visible;
			}
			else if (e.Key == Key.Insert && e.IsToggled == false)
			{
				KeyboardInsertStatusOn.Visibility = System.Windows.Visibility.Visible;
				KeyboardInsertStatusOff.Visibility = System.Windows.Visibility.Hidden;
			}

			//Window Switcher
			if (BackstageToggle.IsChecked == false && NewItem.IsChecked == false)
			{
				if (Globals.OpenDocuments.Count > 0)
				{
					if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.Key == Key.Tab)
					{
						e.Handled = true;
						IsSwitcherWindowOpen = true;
						SwitcherDocumentList.SelectedIndex = ProjectTabs.SelectedIndex;
						SwitcherWindow.Visibility = System.Windows.Visibility.Visible;
					}
				}
			}
		}

		private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			//Window Switcher
			if (BackstageToggle.IsChecked == false && NewItem.IsChecked == false)
			{
				if (Globals.OpenDocuments.Count > 0)
				{
					if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
					{
						e.Handled = true;
						IsSwitcherWindowOpen = false;
						SwitcherWindow.Visibility = System.Windows.Visibility.Collapsed;
					}
					if (e.Key == Key.Tab)
					{
						if (IsSwitcherWindowOpen == true)
						{
							e.Handled = true;
							if (SwitcherDocumentList.SelectedIndex == ProjectTabs.Items.Count - 1) SwitcherDocumentList.SelectedIndex = 0;
							else SwitcherDocumentList.SelectedIndex++;
							SwitcherDocumentList.ScrollIntoView(SwitcherDocumentList.SelectedItem);
						}
					}
				}
			}
		}

		private void Window_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{
			//Window Switcher
			if (SwitcherWindow.Visibility == System.Windows.Visibility.Visible)
			{
				if (BackstageToggle.IsChecked == false && NewItem.IsChecked == false)
				{
					if (Globals.OpenDocuments.Count > 0)
					{
						if (e.Delta > 0)
						{
							e.Handled = true;
							if (SwitcherDocumentList.SelectedIndex == 0) SwitcherDocumentList.SelectedIndex = ProjectTabs.Items.Count - 1;
							else SwitcherDocumentList.SelectedIndex--;
							SwitcherDocumentList.ScrollIntoView(SwitcherDocumentList.SelectedItem);
						}
						else if (e.Delta < 0)
						{
							e.Handled = true;
							if (SwitcherDocumentList.SelectedIndex == ProjectTabs.Items.Count - 1) SwitcherDocumentList.SelectedIndex = 0;
							else SwitcherDocumentList.SelectedIndex++;
							SwitcherDocumentList.ScrollIntoView(SwitcherDocumentList.SelectedItem);
						}
					}
				}
			}
		}

		#endregion

		#region - File / Backstage Handlers -

		public void OpenBackstage()
		{
			BackstageToggle.IsChecked = true;
		}

		private void BackstageToggle_Checked(object sender, RoutedEventArgs e)
		{
			Backstage.Visibility = System.Windows.Visibility.Visible;
			DockView.Visibility = System.Windows.Visibility.Collapsed;
			NewItemScreen.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void BackstageToggle_Unchecked(object sender, RoutedEventArgs e)
		{
			if (Globals.ProjectSpace.State == ObjectSpaceState.Closed)
			{
				BackstageToggle.IsChecked = true;
				return;
			}
			Backstage.Visibility = System.Windows.Visibility.Collapsed;
			if (NewItem.IsChecked == true) NewItemScreen.Visibility = System.Windows.Visibility.Visible;
			else DockView.Visibility = System.Windows.Visibility.Visible;
		}

		public void OpenNewItemScreen()
		{
			NewItem.IsChecked = true;
		}

		private void NewItem_Checked(object sender, RoutedEventArgs e)
		{
			if (BackstageToggle.IsChecked == true)
			{
				Backstage.Visibility = System.Windows.Visibility.Visible;
				DockView.Visibility = System.Windows.Visibility.Collapsed;
				NewItemScreen.Visibility = System.Windows.Visibility.Collapsed;
			}
			else
			{
				Backstage.Visibility = System.Windows.Visibility.Collapsed;
				DockView.Visibility = System.Windows.Visibility.Collapsed;
				NewItemScreen.Visibility = System.Windows.Visibility.Visible;
			}
		}

		private void NewItem_Unchecked(object sender, RoutedEventArgs e)
		{
			if (BackstageToggle.IsChecked == true)
			{
				Backstage.Visibility = System.Windows.Visibility.Visible;
				DockView.Visibility = System.Windows.Visibility.Collapsed;
				NewItemScreen.Visibility = System.Windows.Visibility.Collapsed;
			}
			else
			{
				Backstage.Visibility = System.Windows.Visibility.Collapsed;
				DockView.Visibility = System.Windows.Visibility.Visible;
				NewItemScreen.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			Globals.SaveProjectSpace();
		}

		private void SaveAs_Click(object sender, RoutedEventArgs e)
		{
			string FileName = "";
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (!(Globals.ProjectSpace.FileName == "" || Globals.ProjectSpace.FileName == null)) System.IO.Path.GetDirectoryName(openpath = Globals.ProjectSpace.FileName);

			try
			{
				Microsoft.WindowsAPICodePack.Dialogs.CommonSaveFileDialog sfd = new Microsoft.WindowsAPICodePack.Dialogs.CommonSaveFileDialog("Select a Location to Save the Solution");
				sfd.EnsurePathExists = true;
				sfd.InitialDirectory = openpath;
				sfd.ShowPlacesList = true;
				sfd.DefaultExtension = ".was";
				sfd.Filters.Add(new Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter("WCF Architect Solution Files", ".was"));
				if (sfd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;
				FileName = sfd.FileName;
			}
			catch
			{
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.InitialDirectory = openpath;
				sfd.DefaultExt = ".was";
				sfd.Filter = "WCF Architect Solution Files (*.was)|*.was";
				sfd.Title = "Select a Location to Save the Solution";
				if (sfd.ShowDialog().Value == false) return;
				FileName = sfd.FileName;
			}

			//Create the project file
			ObjectSpace os = new ObjectSpace("Projects.kvtmodel", "WCFArchitect");
			os.CreateObjectLibrary(FileName, ExistingFileAction.Overwrite);
			os.Open(FileName, ObjectSpaceOpenMode.ReadWrite);

			os.Add(Globals.Solution);
			foreach (Projects.Project TP in Globals.Projects)
			{
				os.Add(TP);
			}
			foreach (Projects.Developer TD in Globals.ProjectSpace.OfType<Projects.Developer>())
			{
				os.Add(TD);
			}


			os.Save();
			os.Close();

			Globals.UserProfile.RecentProjects.Add(new Options.RecentSolution(Globals.Solution.Name, FileName));
			ProjectScreen.RefreshRecentList();
		}

		private void Open_Click(object sender, RoutedEventArgs e)
		{
			string FileName = "";
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (!(Globals.UserProfile.DefaultProjectFolder == "" || Globals.UserProfile.DefaultProjectFolder == null)) openpath = Globals.UserProfile.DefaultProjectFolder;

			try
			{
				Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select a Solution to Open");
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
				ofd.Title = "Select a Solution to Open";
				if (ofd.ShowDialog().Value == false) return;
				FileName = ofd.FileName;
			}

			OpenSolution(FileName);
		}

		private void Import_Click(object sender, RoutedEventArgs e)
		{
			WCFArchitect.Backstage.ImportProjects NIP = new Backstage.ImportProjects();
			NIP.ShowDialog();
		}

		private void Close_Click(object sender, RoutedEventArgs e)
		{
			CloseSolution();
		}

		private void BuildSolution_Click(object sender, RoutedEventArgs e)
		{
			Globals.SaveProjectSpace();

			SolutionNavigatorTab.IsEnabled = false;
			ProjectTabs.IsEnabled = false;
			OutputTab.IsSelected = true;
			DockOutput.SlideOpen();

			Globals.CompilerManager.Build();
		}

		private void BuildProject_Click(object sender, RoutedEventArgs e)
		{
			if (ProjectTabs.SelectedContent == null) return;

			Globals.SaveProjectSpace();

			SolutionNavigatorTab.IsEnabled = false;
			OutputTab.IsSelected = true;
			DockOutput.SlideOpen();
			if (ProjectTabs.SelectedContent.GetType() == typeof(Interface.Project.Project))
			{
				Interface.Project.Project TD = ProjectTabs.SelectedContent as Interface.Project.Project;
				Globals.CompilerManager.BuildProject(TD.Settings);
			}
			else if (ProjectTabs.SelectedContent.GetType() == typeof(Interface.Namespace.Namespace))
			{
				Interface.Namespace.Namespace TD = ProjectTabs.SelectedContent as Interface.Namespace.Namespace;
				Globals.CompilerManager.BuildProject(TD.Data.Owner);
			}
			else if (ProjectTabs.SelectedContent.GetType() == typeof(Interface.Service.Service))
			{
				Interface.Service.Service TD = ProjectTabs.SelectedContent as Interface.Service.Service;
				Globals.CompilerManager.BuildProject(TD.Data.Parent.Owner);
			}
			else if (ProjectTabs.SelectedContent.GetType() == typeof(Interface.Data.Data))
			{
				Interface.Data.Data TD = ProjectTabs.SelectedContent as Interface.Data.Data;
				Globals.CompilerManager.BuildProject(TD.PData.Parent.Owner);
			}
			else if (ProjectTabs.SelectedContent.GetType() == typeof(Interface.Enum.Enum))
			{
				Interface.Enum.Enum TD = ProjectTabs.SelectedContent as Interface.Enum.Enum;
				Globals.CompilerManager.BuildProject(TD.Data.Parent.Owner);
			}
			ProjectTabs.IsEnabled = false;
		}

		private void BuildOutput_Click(object sender, RoutedEventArgs e)
		{
			if (ProjectTabs.SelectedContent == null) return;
			OutputTab.IsSelected = true;
			DockOutput.SlideOpen();
			if (ProjectTabs.SelectedContent.GetType() == typeof(Interface.Project.Project))
			{
				Interface.Project.Project TD = ProjectTabs.SelectedContent as Interface.Project.Project;
				Globals.CompilerManager.BuildProjectOutput(TD.Settings);
			}
			else if (ProjectTabs.SelectedContent.GetType() == typeof(Interface.Namespace.Namespace))
			{
				Interface.Namespace.Namespace TD = ProjectTabs.SelectedContent as Interface.Namespace.Namespace;
				Globals.CompilerManager.BuildProjectOutput(TD.Data.Owner);
			}
			else if (ProjectTabs.SelectedContent.GetType() == typeof(Interface.Service.Service))
			{
				Interface.Service.Service TD = ProjectTabs.SelectedContent as Interface.Service.Service;
				Globals.CompilerManager.BuildProjectOutput(TD.Data.Parent.Owner);
			}
			else if (ProjectTabs.SelectedContent.GetType() == typeof(Interface.Data.Data))
			{
				Interface.Data.Data TD = ProjectTabs.SelectedContent as Interface.Data.Data;
				Globals.CompilerManager.BuildProjectOutput(TD.PData.Parent.Owner);
			}
			else if (ProjectTabs.SelectedContent.GetType() == typeof(Interface.Enum.Enum))
			{
				Interface.Enum.Enum TD = ProjectTabs.SelectedContent as Interface.Enum.Enum;
				Globals.CompilerManager.BuildProjectOutput(TD.Data.Parent.Owner);
			}
		}

		private void Help_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(Globals.ApplicationPath + "WCF Architect 1.1 User Guide.pdf"));
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		#endregion

		#region - Project File -

		public bool NewSolution(string Name, string Path)
		{
			if (CloseSolution(true) == false) return false;

			//Create the project file
			if (!System.IO.File.Exists(Path))
				Globals.ProjectSpace.CreateObjectLibrary(Path, ExistingFileAction.Overwrite);
			Globals.ProjectSpace.EnableConcurrency = true;
			Globals.ProjectSpace.Open(Path, ObjectSpaceOpenMode.ReadWrite);

			//Add the current developer to the developers list.
			Globals.ProjectSpace.Add(new Projects.Developer() { ID = Globals.UserProfile.ID, ComputerName = Globals.UserProfile.ComputerName, UserName = Globals.UserProfile.User, LastEditedBy = true });
			Globals.SaveProjectSpace();

#if TRIAL
			Projects.Solution NS = new Projects.Solution(Name, Globals.LicenseKey.Authorization);
#else
			Projects.Solution NS = new Projects.Solution(Name);
#endif
			Globals.ProjectSpace.Add(NS);
			Globals.Solution = NS;
			Globals.Projects = new ObservableCollectionSortable<Projects.Project>();
			SolutionNavigatorView.ItemsSource = Globals.Projects;

			BackstageToggle.IsChecked = false;
			BackstageProjectInfo.IsEnabled = true;
			BackstageSave.IsEnabled = true;
			BackstageSaveAs.IsEnabled = true;
			BackstageImport.IsEnabled = true;
			BackstageClose.IsEnabled = true;
			NewItem.IsEnabled = true;
			Save.IsEnabled = true;
			BuildSolution.IsEnabled = true;
			BuildProject.IsEnabled = true;
			BuildOutput.IsEnabled = true;

			Options.RecentSolution NRP = new Options.RecentSolution(Name, Path);
			Globals.UserProfile.RecentProjects.Add(NRP);
			Globals.ActiveProjectInfo = NRP;
			ProjectScreen.RefreshRecentList();
			ProjectInfoScreen.RefreshProjectInfo();
			this.Title = Name + " - Prospective Software WCF Architect";

			SolutionNavigatorShowAll = true;

			return true;
		}

		public void NewProject(string Name)
		{
			Globals.IsLoading = true;
			Projects.Project NP = new Projects.Project(Name);
			Globals.ProjectSpace.Add(NP);
			Globals.Projects.Add(NP);
			Globals.Projects.Sort(x => x.Name);

			NP.Open();
			Globals.IsLoading = false;

			OpenProjectItem(NP);

			BackstageToggle.IsChecked = false;

			ProjectScreen.RefreshRecentList();
			ProjectInfoScreen.RefreshProjectInfo();
		}

		public void NewDependency(string Name)
		{
#if !STANDARD
			Globals.IsLoading = true;
			Projects.DependencyProject NP = new Projects.DependencyProject(Name);
			Globals.ProjectSpace.Add(NP);
			Globals.Projects.Add(NP);
			Globals.Projects.Sort(x => x.Name);

			NP.Open();
			Globals.IsLoading = false;

			OpenProjectItem(NP);

			BackstageToggle.IsChecked = false;

			ProjectScreen.RefreshRecentList();
			ProjectInfoScreen.RefreshProjectInfo();
#else
			Prospective.Controls.MessageBox.Show("You cannot create a Dependency Project using the Standard Version of WCF Architect!", "Not Supported", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
		}

		public void OpenSolution(string Path)
		{
			if (CloseSolution(true) == false) return;

			Globals.OpenProjectSpace(Path, OpenSolutionFinished);
		}

		public void OpenSolutionFinished(bool Success)
		{
			if (Success == true)
			{
				//Determine if there is already a recent entry. Create one if false. Update the current one if true.
				bool ProjectHasRecentEntry = false;
				foreach (WCFArchitect.Options.RecentSolution RP in Globals.UserProfile.ImportantProjects)
					if (RP.Path == Globals.ProjectSpace.FileName)
					{
						ProjectHasRecentEntry = true;
						RP.LastAccessed = DateTime.Now;
						Globals.ActiveProjectInfo = RP;
					}
				foreach (WCFArchitect.Options.RecentSolution RP in Globals.UserProfile.RecentProjects)
					if (RP.Path == Globals.ProjectSpace.FileName)
					{
						ProjectHasRecentEntry = true;
						RP.LastAccessed = DateTime.Now;
						Globals.ActiveProjectInfo = RP;
					}
				if (ProjectHasRecentEntry == false)
				{
					Options.RecentSolution NRP = new Options.RecentSolution(Globals.Solution.Name, Globals.ProjectSpace.FileName);
					Globals.UserProfile.RecentProjects.Add(NRP);
					Globals.ActiveProjectInfo = NRP;
				}

				ProjectScreen.RefreshRecentList();
				ProjectInfoScreen.RefreshProjectInfo();

				BackstageProjectInfo.IsEnabled = true;
				BackstageSave.IsEnabled = true;
				BackstageSaveAs.IsEnabled = true;
				BackstageImport.IsEnabled = true;
				BackstageClose.IsEnabled = true;
				NewItem.IsEnabled = true;
				Save.IsEnabled = true;
				BuildSolution.IsEnabled = true;
				BuildProject.IsEnabled = true;
				BuildOutput.IsEnabled = true;
				this.Title = Globals.Solution.Name + " - Prospective Software WCF Architect";
				BackstageToggle.IsChecked = false;
			}
		}

		public bool CloseSolution(bool AskBeforeClose = false, bool Closing = false)
		{
			Globals.IsClosing = true;

			if (Globals.ProjectSpace.State == ObjectSpaceState.Open)
			{
				if (AskBeforeClose == true)
				{
					if (Prospective.Controls.MessageBox.Show("In order to perform the requested action, the current project will be saved and closed. Would you like to continue?", "Continue", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
					{
						Globals.CloseProjectSpace();
					}
					else
					{
						return false;
					}
				}
				else
				{
					if (Closing == true)
					{
						MessageBoxResult MBR = MessageBoxResult.No;
						MBR = Prospective.Controls.MessageBox.Show("Would you like to save the solution?", "Save Solution", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);
						if (MBR == MessageBoxResult.Yes)
						{
							Globals.CloseProjectSpace();
						}
						if (MBR == MessageBoxResult.Cancel)
						{
							Globals.IsClosing = false;
							return false;
						}
						else
						{
						}
					}
					else
					{
						Globals.CloseProjectSpace();
					}
				}
			}

			Globals.ActiveProjectInfo = null;
			BackstageProjectInfo.IsEnabled = false;
			BackstageSave.IsEnabled = false;
			BackstageSaveAs.IsEnabled = false;
			BackstageImport.IsEnabled = false;
			BackstageClose.IsEnabled = false;
			NewItem.IsEnabled = false;
			Save.IsEnabled = false;
			BuildSolution.IsEnabled = false;
			BuildProject.IsEnabled = false;
			BuildOutput.IsEnabled = false;
			this.Title = "Prospective Software WCF Architect";

			if (!(Globals.ProjectSpace.FileName == null || Globals.ProjectSpace.FileName == ""))
				System.IO.File.Delete(System.IO.Path.ChangeExtension(Globals.ProjectSpace.FileName, ".bak"));

			Globals.IsClosing = false;
			return true;
		}

		public object GetOpenProjectScreen(object Item)
		{
			if (Item == null) return null;
			Type valueType = Item.GetType();

			if (valueType == typeof(Projects.Project) || valueType == typeof(Projects.DependencyProject))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Project.Project))
					{
						Interface.Project.Project TD = TI.Content as Interface.Project.Project;
						if (TD.Settings == Item as Projects.Project)
						{
							TI.IsSelected = true;
							TD.ProjectTab.IsSelected = true;
							return TD;
						}
					}
				}
			}
			else if (valueType.IsSubclassOf(typeof(Projects.ServiceBinding)) == true)
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Project.Project))
					{
						Interface.Project.Project TD = TI.Content as Interface.Project.Project;
						foreach (Projects.ServiceBinding SB in TD.Settings.Bindings)
						{
							if (SB == Item as Projects.ServiceBinding)
							{
								TI.IsSelected = true;
								TD.BindingsTab.IsSelected = true;
								TD.BindingsList.SelectedItem = SB;
								return TD.BindingContent.Content;
							}
						}
					}
				}
			}
			else if (valueType.IsSubclassOf(typeof(Projects.BindingSecurity)) == true)
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Project.Project))
					{
						Interface.Project.Project TD = TI.Content as Interface.Project.Project;
						foreach (Projects.BindingSecurity SB in TD.Settings.Security)
						{
							if (SB == Item as Projects.BindingSecurity)
							{
								TI.IsSelected = true;
								TD.SecurityTab.IsSelected = true;
								TD.SecurityList.SelectedItem = SB;
								return TD.SecurityContent.Content;
							}
						}
					}
				}
			}
			else if (valueType == typeof(Projects.Host))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Project.Project))
					{
						Interface.Project.Project TD = TI.Content as Interface.Project.Project;
						foreach (Projects.Host SB in TD.Settings.Hosts)
						{
							if (SB == Item as Projects.Host)
							{
								TI.IsSelected = true;
								TD.HostsTab.IsSelected = true;
								TD.HostsList.SelectedItem = SB;
								return TD.HostContent.Content;
							}
						}
					}
				}
			}
			else if (valueType == typeof(Projects.HostEndpoint))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Project.Project))
					{
						Interface.Project.Project TD = TI.Content as Interface.Project.Project;
						foreach (Projects.Host SB in TD.Settings.Hosts)
						{
							Projects.HostEndpoint THE = Item as Projects.HostEndpoint;
							if (SB == THE.Parent as Projects.Host)
							{
								TI.IsSelected = true;
								TD.HostsTab.IsSelected = true;
								TD.HostsList.SelectedItem = SB;
								Interface.Project.Host.Host H = TD.HostContent.Content as Interface.Project.Host.Host;
								foreach (Projects.HostEndpoint HE in SB.Endpoints)
								{
									if (HE == THE)
									{
										H.EndpointList.SelectedItem = HE;
										return H.EndpointContent.Content;
									}
								}
							}
						}
					}
				}
			}
			else if (valueType == typeof(Projects.HostBehavior))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Project.Project))
					{
						Interface.Project.Project TD = TI.Content as Interface.Project.Project;
						foreach (Projects.Host SB in TD.Settings.Hosts)
						{
							Projects.HostBehavior THE = Item as Projects.HostBehavior;
							if (SB == THE.Parent as Projects.Host)
							{
								TI.IsSelected = true;
								TD.HostsTab.IsSelected = true;
								TD.HostsList.SelectedItem = SB;
								Interface.Project.Host.Host H = TD.HostContent.Content as Interface.Project.Host.Host;
								foreach (Projects.HostBehavior HE in SB.Behaviors)
								{
									if (HE == THE)
									{
										H.BehaviorList.SelectedItem = HE;
										return H.BehaviorContent.Content;
									}
								}
							}
						}
					}
				}
			}
			else if (valueType == typeof(Projects.Namespace))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Namespace.Namespace))
					{
						Interface.Namespace.Namespace TD = TI.Content as Interface.Namespace.Namespace;
						if (TD.Data == Item as Projects.Namespace)
						{
							TI.IsSelected = true;
							return TD;
						}
					}
				}
			}
			else if (valueType == typeof(Projects.Service))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Service.Service))
					{
						Interface.Service.Service TD = TI.Content as Interface.Service.Service;
						if (TD.Data == Item as Projects.Service)
						{
							TI.IsSelected = true;
							return TD;
						}
					}
				}
			}
			else if (valueType == typeof(Projects.Operation))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Service.Service))
					{
						Interface.Service.Service TD = TI.Content as Interface.Service.Service;
						Projects.Operation O = Item as Projects.Operation;
						if (TD.Data == O.Owner)
						{
							TI.IsSelected = true;
							TD.OperationsList.SelectedItem = O;
							return TD.OperationContent.Content;
						}
					}
				}
			}
			else if (valueType == typeof(Projects.OperationParameter))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Service.Service))
					{
						Interface.Service.Service TD = TI.Content as Interface.Service.Service;
						Projects.OperationParameter OP = Item as Projects.OperationParameter;
						if (TD.Data == OP.Owner)
						{
							TI.IsSelected = true;
							foreach (Projects.Operation O in TD.Data.Operations)
							{
								foreach (Projects.OperationParameter TOP in O.Parameters)
								{
									if (TOP == OP)
									{
										TD.OperationsList.SelectedItem = O;
										Interface.Service.Operation TO = TD.OperationContent.Content as Interface.Service.Operation;
										TO.ValuesList.SelectedItem = OP;
										return OP;
									}
								}
							}
						}
					}
				}
			}
			else if (valueType == typeof(Projects.Property))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Service.Service))
					{
						Interface.Service.Service TD = TI.Content as Interface.Service.Service;
						Projects.Property O = Item as Projects.Property;
						if (TD.Data == O.Owner)
						{
							TI.IsSelected = true;
							TD.PropertiesList.SelectedItem = O;
							return TD.PropertyContent.Content;
						}
					}
				}
			}
			else if (valueType == typeof(Projects.Data))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Data.Data))
					{
						Interface.Data.Data TD = TI.Content as Interface.Data.Data;
						if (TD.PData == Item as Projects.Data)
						{
							TI.IsSelected = true;
							return TD;
						}
					}
				}
			}
			else if (valueType == typeof(Projects.DataElement))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Data.Data))
					{
						Interface.Data.Data TD = TI.Content as Interface.Data.Data;
						Projects.DataElement DE = Item as Projects.DataElement;
						if (TD.PData == DE.Owner)
						{
							TI.IsSelected = true;
							TD.ValuesList.SelectedItem = DE;
							return DE;
						}
					}
				}
			}
			else if (valueType == typeof(Projects.Enum))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Enum.Enum))
					{
						Interface.Enum.Enum TD = TI.Content as Interface.Enum.Enum;
						if (TD.Data == Item as Projects.Enum)
						{
							TI.IsSelected = true;
							return TD;
						}
					}
				}
			}
			else if (valueType == typeof(Projects.EnumElement))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Data.Data))
					{
						Interface.Enum.Enum TE = TI.Content as Interface.Enum.Enum;
						Projects.EnumElement EE = Item as Projects.EnumElement;
						if (TE.Data == EE.Owner)
						{
							TI.IsSelected = true;
							if (TE.Data.IsFlags == false) TE.ValuesList.SelectedItem = EE;
							else TE.FlagsList.SelectedItem = EE;
							return EE;
						}
					}
				}
			}

			return null;
		}

		public void OpenProjectItem(object Item)
		{
			if (Item == null) return;
			Type valueType = Item.GetType();
			Globals.IsDocumentOpening = true;

			try
			{
				if (valueType == typeof(Projects.Project) || valueType == typeof(Projects.DependencyProject))
				{
					foreach (C1DockTabItem TI in ProjectTabs.Items)
					{
						if (TI.Content.GetType() == typeof(Interface.Project.Project))
						{
							Interface.Project.Project TD = TI.Content as Interface.Project.Project;
							if (TD.Settings == Item as Projects.Project)
							{
								if (Globals.IsLoading == false) TI.IsSelected = true;
								TD.ProjectTab.IsSelected = true;
								return;
							}
						}
					}

					//If the project window is not open, open it. 
					Globals.OpenDocuments.Add(Item as Projects.Project);
				}
				else if (valueType.IsSubclassOf(typeof(Projects.ServiceBinding)) == true)
				{
					foreach (C1DockTabItem TI in ProjectTabs.Items)
					{
						if (TI.Content.GetType() == typeof(Interface.Project.Project))
						{
							Interface.Project.Project TD = TI.Content as Interface.Project.Project;
							foreach (Projects.ServiceBinding SB in TD.Settings.Bindings)
							{
								if (SB == Item as Projects.ServiceBinding)
								{
									if (Globals.IsLoading == false) TI.IsSelected = true;
									TD.BindingsTab.IsSelected = true;
									TD.BindingsList.SelectedItem = SB;
									return;
								}
							}
						}
					}

					//If the project window is not open, open it. 
					Projects.ServiceBinding TPD = Item as Projects.ServiceBinding;
					Globals.OpenDocuments.Add(TPD.Parent);
				}
				else if (valueType.IsSubclassOf(typeof(Projects.BindingSecurity)) == true)
				{
					foreach (C1DockTabItem TI in ProjectTabs.Items)
					{
						if (TI.Content.GetType() == typeof(Interface.Project.Project))
						{
							Interface.Project.Project TD = TI.Content as Interface.Project.Project;
							foreach (Projects.BindingSecurity SB in TD.Settings.Security)
							{
								if (SB == Item as Projects.BindingSecurity)
								{
									if (Globals.IsLoading == false) TI.IsSelected = true;
									TD.SecurityTab.IsSelected = true;
									TD.SecurityList.SelectedItem = SB;
									return;
								}
							}
						}
					}

					//If the project window is not open, open it.
					Projects.BindingSecurity TPD = Item as Projects.BindingSecurity;
					Globals.OpenDocuments.Add(TPD.Parent);
				}
				else if (valueType == typeof(Projects.Host))
				{
					foreach (C1DockTabItem TI in ProjectTabs.Items)
					{
						if (TI.Content.GetType() == typeof(Interface.Project.Project))
						{
							Interface.Project.Project TD = TI.Content as Interface.Project.Project;
							foreach (Projects.Host SB in TD.Settings.Hosts)
							{
								if (SB == Item as Projects.Host)
								{
									if (Globals.IsLoading == false) TI.IsSelected = true;
									TD.HostsTab.IsSelected = true;
									TD.HostsList.SelectedItem = SB;
									return;
								}
							}
						}
					}

					//If the project window is not open, open it.
					Projects.Host TPD = Item as Projects.Host;
					Globals.OpenDocuments.Add(TPD.Parent);
				}
				else if (valueType == typeof(Projects.Namespace))
				{
					foreach (C1DockTabItem TI in ProjectTabs.Items)
					{
						if (TI.Content.GetType() == typeof(Interface.Namespace.Namespace))
						{
							Interface.Namespace.Namespace TD = TI.Content as Interface.Namespace.Namespace;
							if (TD.Data == Item as Projects.Namespace)
							{
								if (Globals.IsLoading == false) TI.IsSelected = true;
								return;
							}
						}
					}

					//If the project window is not open, open it.
					Globals.OpenDocuments.Add(Item as Projects.Namespace);
				}
				else if (valueType == typeof(Projects.Service))
				{
					foreach (C1DockTabItem TI in ProjectTabs.Items)
					{
						if (TI.Content.GetType() == typeof(Interface.Service.Service))
						{
							Interface.Service.Service TD = TI.Content as Interface.Service.Service;
							if (TD.Data == Item as Projects.Service)
							{
								if (Globals.IsLoading == false) TI.IsSelected = true;
								return;
							}
						}
					}

					//If the project window is not open, open it.
					Globals.OpenDocuments.Add(Item as Projects.Service);
				}
				else if (valueType == typeof(Projects.Data))
				{
					foreach (C1DockTabItem TI in ProjectTabs.Items)
					{
						if (TI.Content.GetType() == typeof(Interface.Data.Data))
						{
							Interface.Data.Data TD = TI.Content as Interface.Data.Data;
							if (TD.PData == Item as Projects.Data)
							{
								TI.IsSelected = true;
								return;
							}
						}
					}

					//If the project window is not open, open it.
					Globals.OpenDocuments.Add(Item as Projects.Data);
				}
				else if (valueType == typeof(Projects.Enum))
				{
					foreach (C1DockTabItem TI in ProjectTabs.Items)
					{
						if (TI.Content.GetType() == typeof(Interface.Enum.Enum))
						{
							Interface.Enum.Enum TD = TI.Content as Interface.Enum.Enum;
							if (TD.Data == Item as Projects.Enum)
							{
								if (Globals.IsLoading == false) TI.IsSelected = true;
								return;
							}
						}
					}

					//If the project window is not open, open it.
					Globals.OpenDocuments.Add(Item as Projects.Enum);
				}
				else if (Item.GetType() == typeof(Compiler.Compiler))
				{
					Compiler.Compiler TC = Item as Compiler.Compiler;

					TabItem NDW = new TabItem();
					NDW.Header = TC.Project.Name;
					NDW.Content = new ErrorList(TC.Messages);
					ErrorListTabs.Items.Add(NDW);

					TabItem NDW2 = new TabItem();
					NDW2.Header = TC.Project.Name;
					NDW2.Content = new Output(TC.Output);
					OutputTabs.Items.Add(NDW2);
				}
				else
					return;
			}
			catch (Exception ex)
			{
				Prospective.Controls.MessageBox.Show("The following exception was caught by WCF Architect. Please report the error using the Bug Report Page link in the About section of the Options page." + Environment.NewLine + Environment.NewLine + ex.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				Globals.IsDocumentOpening = false;
			}
		}

		#endregion

		#region - Dock/Tab Handling -

		private static void OnDockModeChangedCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Button a = e.Source as Button;
			C1DockTabControl tc = Globals.GetVisualParent<C1DockTabControl>(a);

			if (tc.DockMode == DockMode.Docked) tc.DockMode = DockMode.Sliding;
			else if (tc.DockMode == DockMode.Sliding) tc.DockMode = DockMode.Docked;
		}

		private void ProjectTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			BuildSolution.IsEnabled = true;
			BuildProject.IsEnabled = true;
			BuildOutput.IsEnabled = true;

			Interface.Project.Project P = ProjectTabs.SelectedContent as Interface.Project.Project;
			if (P != null)
			{
				if (P.Settings.IsDependencyProject == true)
				{
					BuildSolution.IsEnabled = false;
					BuildProject.IsEnabled = false;
					BuildOutput.IsEnabled = false;
				}
			}

			Interface.Namespace.Namespace N = ProjectTabs.SelectedContent as Interface.Namespace.Namespace;
			if (N != null)
			{
				if (N.Data.Owner.IsDependencyProject == true)
				{
					BuildSolution.IsEnabled = false;
					BuildProject.IsEnabled = false;
					BuildOutput.IsEnabled = false;
				}
				N.CodeName.Focus();
				N.FullName.Text = N.Data.FullName;
			}

			Interface.Service.Service S = ProjectTabs.SelectedContent as Interface.Service.Service;
			if (S != null)
			{
				if (S.Data.Parent.Owner.IsDependencyProject == true)
				{
					BuildSolution.IsEnabled = false;
					BuildProject.IsEnabled = false;
					BuildOutput.IsEnabled = false;
				}
			}

			Interface.Data.Data D = ProjectTabs.SelectedContent as Interface.Data.Data;
			if (D != null)
			{
				if (D.PData.Parent.Owner.IsDependencyProject == true)
				{
					BuildSolution.IsEnabled = false;
					BuildProject.IsEnabled = false;
					BuildOutput.IsEnabled = false;
				}
			}

			Interface.Enum.Enum E = ProjectTabs.SelectedContent as Interface.Enum.Enum;
			if (E != null)
			{
				if (E.Data.Parent.Owner.IsDependencyProject == true)
				{
					BuildSolution.IsEnabled = false;
					BuildProject.IsEnabled = false;
					BuildOutput.IsEnabled = false;
				}
			}
		}

		public void CloseTabWindow(Themes.C1DockTabItemWindow Window)
		{
			C1.WPF.CancelSourceEventArgs e = new CancelSourceEventArgs();
			e.Source = Window;
			ProjectTabs_TabItemClosing(null, e);
		}

		private void ProjectTabs_TabItemClosing(object sender, C1.WPF.CancelSourceEventArgs e)
		{
			if (Globals.IsClosing == true) return;
			Themes.C1DockTabItemWindow Window = e.Source as Themes.C1DockTabItemWindow;
			if (Window == null) return;

			Globals.OpenDocuments.Remove(Window.DocumentData);	
		}

		private void ProjectTabs_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				HitTestResult htr = VisualTreeHelper.HitTest(ProjectTabs, e.GetPosition(ProjectTabs));
				if (htr == null) return;
				if (htr.VisualHit == null) return;
				Themes.C1DockTabItemWindow t = Globals.GetVisualParent<Themes.C1DockTabItemWindow>(htr.VisualHit);
				Globals.OpenDocuments.Remove(t.DocumentData);
			}
		}

		public void UpdateTabsLayout()
		{
			ProjectTabs.TabStripPlacement = Globals.UserProfile.TabPosition;
			foreach (C1.WPF.Docking.C1DockTabItem TI in ProjectTabs.Items)
			{
				if (Globals.UserProfile.TabPosition == C1.WPF.Dock.Left || Globals.UserProfile.TabPosition == C1.WPF.Dock.Right)
					TI.MaxWidth = TI.MinWidth = Globals.UserProfile.TabMaximumWidth;
				else
				{
					TI.MaxWidth = Globals.UserProfile.TabMaximumWidth;
					TI.MinWidth = 50;
				}
			}
		}

		public void LoadDockLayout()
		{
			if (Globals.UserProfile.IsDockConfigurationAvailable == false) return;

			DockSolutionNavigator.Dock = Globals.UserProfile.DockSolutionNavigatorPosition;
			DockSolutionNavigator.DockMode = Globals.UserProfile.DockSolutionNavigatorMode;
			if(Globals.UserProfile.DockSolutionNavigatorPosition == C1.WPF.Dock.Top || Globals.UserProfile.DockSolutionNavigatorPosition == C1.WPF.Dock.Bottom) DockSolutionNavigator.DockHeight = Globals.UserProfile.DockSolutionNavigatorHeight;
			if (Globals.UserProfile.DockSolutionNavigatorPosition == C1.WPF.Dock.Right || Globals.UserProfile.DockSolutionNavigatorPosition == C1.WPF.Dock.Left) DockSolutionNavigator.DockWidth = Globals.UserProfile.DockSolutionNavigatorWidth;

			DockErrorList.Dock = Globals.UserProfile.DockErrorListPosition;
			DockErrorList.DockMode = Globals.UserProfile.DockErrorListMode;
			if (Globals.UserProfile.DockErrorListPosition == C1.WPF.Dock.Top || Globals.UserProfile.DockErrorListPosition == C1.WPF.Dock.Bottom) DockErrorList.DockHeight = Globals.UserProfile.DockErrorListHeight;
			if (Globals.UserProfile.DockErrorListPosition == C1.WPF.Dock.Right || Globals.UserProfile.DockErrorListPosition == C1.WPF.Dock.Left) DockErrorList.DockWidth = Globals.UserProfile.DockErrorListWidth;

			DockOutput.Dock = Globals.UserProfile.DockOutputPosition;
			DockOutput.DockMode = Globals.UserProfile.DockOutputMode;
			if (Globals.UserProfile.DockOutputPosition == C1.WPF.Dock.Top || Globals.UserProfile.DockOutputPosition == C1.WPF.Dock.Bottom) DockOutput.DockHeight = Globals.UserProfile.DockOutputHeight;
			if (Globals.UserProfile.DockOutputPosition == C1.WPF.Dock.Right || Globals.UserProfile.DockOutputPosition == C1.WPF.Dock.Left) DockOutput.DockWidth = Globals.UserProfile.DockOutputWidth;
		}

		public void SaveDockLayout()
		{
			Globals.UserProfile.DockSolutionNavigatorHeight = DockSolutionNavigator.DockHeight;
			Globals.UserProfile.DockSolutionNavigatorWidth = DockSolutionNavigator.DockWidth;

			Globals.UserProfile.DockErrorListHeight = DockErrorList.DockHeight;
			Globals.UserProfile.DockErrorListWidth = DockErrorList.DockWidth;

			Globals.UserProfile.DockOutputHeight = DockOutput.DockHeight;
			Globals.UserProfile.DockOutputWidth = DockOutput.DockWidth;

			Globals.UserProfile.DockFindHeight = DockFindReplace.DockHeight;
			Globals.UserProfile.DockFindWidth = DockFindReplace.DockWidth;

			Globals.UserProfile.IsDockConfigurationAvailable = true;
		}

		private void DockView_ViewChanged(object sender, EventArgs e)
		{
			if (IsLoaded == false) return;

			Globals.UserProfile.DockSolutionNavigatorPosition = DockSolutionNavigator.Dock;
			Globals.UserProfile.DockSolutionNavigatorMode = DockSolutionNavigator.DockMode;

			Globals.UserProfile.DockErrorListPosition = DockErrorList.Dock;
			Globals.UserProfile.DockErrorListMode = DockErrorList.DockMode;

			Globals.UserProfile.DockOutputPosition = DockOutput.Dock;
			Globals.UserProfile.DockOutputMode = DockOutput.DockMode;

			Globals.UserProfile.DockFindPosition = DockFindReplace.Dock;
			Globals.UserProfile.DockFindMode = DockFindReplace.DockMode;
		}

		#endregion

		#region  - Solution Navigator -

		private void SolutionNavigatorShowAllToggle_Checked(object sender, RoutedEventArgs e)
		{
			SolutionNavigatorShowNamespaces = false;
			SolutionNavigatorShowServices = false;
			SolutionNavigatorShowData = false;
			SolutionNavigatorShowEnums = false;

			RefreshSolutionNavigator();
		}

		private void SolutionNavigatorShowNamespacesToggle_Checked(object sender, RoutedEventArgs e)
		{
			SolutionNavigatorShowAll = false;
			SolutionNavigatorShowServices = false;
			SolutionNavigatorShowData = false;
			SolutionNavigatorShowEnums = false;

			RefreshSolutionNavigator();
		}

		private void SolutionNavigatorShowServicesToggle_Checked(object sender, RoutedEventArgs e)
		{
			SolutionNavigatorShowAll = false;
			SolutionNavigatorShowNamespaces = false;
			SolutionNavigatorShowData = false;
			SolutionNavigatorShowEnums = false;

			RefreshSolutionNavigator();
		}

		private void SolutionNavigatorShowDataToggle_Checked(object sender, RoutedEventArgs e)
		{
			SolutionNavigatorShowAll = false;
			SolutionNavigatorShowNamespaces = false;
			SolutionNavigatorShowServices = false;
			SolutionNavigatorShowEnums = false;

			RefreshSolutionNavigator();
		}

		private void SolutionNavigatorShowEnumToggle_Checked(object sender, RoutedEventArgs e)
		{
			SolutionNavigatorShowAll = false;
			SolutionNavigatorShowNamespaces = false;
			SolutionNavigatorShowServices = false;
			SolutionNavigatorShowData = false;

			RefreshSolutionNavigator();
		}

		private void SolutionNavigatorSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			foreach (Projects.Project P in Globals.Projects)
			{
				if (SolutionNavigatorSearch.Text.Length < 3)
					P.Search("");
				else
					P.Search(SolutionNavigatorSearch.Text);
			}
		}

		private void SolutionNavigatorView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (SolutionNavigatorView.SelectedItem == null) return;

			Type itemtype = SolutionNavigatorView.SelectedItem.GetType();
			if (itemtype == typeof(Projects.Project) || itemtype == typeof(Projects.DependencyProject)) e.Handled = true;
			if (itemtype == typeof(Projects.Service)) e.Handled = true;
			if (itemtype == typeof(Projects.Data)) e.Handled = true;
			if (itemtype == typeof(Projects.Enum)) e.Handled = true;

			OpenProjectItem(SolutionNavigatorView.SelectedItem);
		}

		private void SolutionNavigatorView_KeyUp(object sender, KeyEventArgs e)
		{
			if (SolutionNavigatorView.SelectedItem == null) return;

			if (e.Key == Key.Enter)
			{
				OpenProjectItem(SolutionNavigatorView.SelectedItem);
			}

			if (e.Key == Key.Delete)
			{
				Type valueType = SolutionNavigatorView.SelectedItem.GetType();
				if (valueType == typeof(Projects.Project) || valueType == typeof(Projects.DependencyProject))
				{
					Projects.Project D = SolutionNavigatorView.SelectedItem as Projects.Project;
					if (D == null) return;
#if STANDARD
					if (D.IsDependencyProject == true) return;
#endif
					if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + D.Name + "' project? Doing so will also delete anything contained within the project.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
					{
						CloseProject(D);
						Globals.Projects.Remove(D);
						Globals.ProjectSpace.Remove(D);
					}
				}
				if (valueType == typeof(Projects.ServiceBinding))
				{
					Projects.ServiceBinding D = SolutionNavigatorView.SelectedItem as Projects.ServiceBinding;
					if (D == null) return;
#if STANDARD
					if (D.Parent.IsDependencyProject == true) return;
#endif
					if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + D.Name + "' binding? Doing so will also delete anything contained in the binding.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
					{
						CloseProjectItem(D);
						if (D.Parent != null) D.Parent.Bindings.Remove(D);
					}
				}
				if (valueType == typeof(Projects.BindingSecurity))
				{
					Projects.BindingSecurity D = SolutionNavigatorView.SelectedItem as Projects.BindingSecurity;
					if (D == null) return;
#if STANDARD
					if (D.Parent.IsDependencyProject == true) return;
#endif
					if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + D.Name + "' binding security object? Doing so will also delete anything contained in the security object.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
					{
						CloseProjectItem(D);
						if (D.Parent != null) D.Parent.Security.Remove(D);
					}
				}
				if (valueType == typeof(Projects.Host))
				{
					Projects.Host D = SolutionNavigatorView.SelectedItem as Projects.Host;
					if (D == null) return;
#if STANDARD
					if (D.Parent.IsDependencyProject == true) return;
#endif
					if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + D.Name + "' host? Doing so will also delete anything contained in the host.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
					{
						CloseProjectItem(D);
						if (D.Parent != null) D.Parent.Hosts.Remove(D);
					}
				}
				if (valueType == typeof(Projects.Namespace))
				{
					Projects.Namespace D = SolutionNavigatorView.SelectedItem as Projects.Namespace;
					if (D == null) return;
#if STANDARD
					if (D.Owner.IsDependencyProject == true) return;
#endif
					if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + D.Name + "' namespace? Doing so will also delete anything contained in the namespace.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
					{
						CloseProjectItem(D);
						D.Owner.Namespaces.Remove(D);
						D.Parent.Children.Remove(D);
					}
				}
				if (valueType == typeof(Projects.Service))
				{
					Projects.Service D = SolutionNavigatorView.SelectedItem as Projects.Service;
					if (D == null) return;
#if STANDARD
					if (D.Parent.Owner.IsDependencyProject == true) return;
#endif
					if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + D.Name + "' service? Doing so will also delete anything contained in the service.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
					{
						CloseProjectItem(D);
						if (D.Parent.Services.Count == 0) D.Parent.Owner.Services.Remove(D);
						else D.Parent.Services.Remove(D);
					}
				}
				if (valueType == typeof(Projects.Data))
				{
					Projects.Data D = SolutionNavigatorView.SelectedItem as Projects.Data;
					if (D == null) return;
#if STANDARD
					if (D.Parent.Owner.IsDependencyProject == true) return;
#endif
					if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + D.Name + "' data object? Doing so will also delete anything contained in the data object.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
					{
						CloseProjectItem(D);
						if (D.Parent.Data.Count == 0) D.Parent.Owner.Data.Remove(D);
						else D.Parent.Data.Remove(D);
					}
				}
				if (valueType == typeof(Projects.Enum))
				{
					Projects.Enum D = SolutionNavigatorView.SelectedItem as Projects.Enum;
					if (D == null) return;
#if STANDARD
					if (D.Parent.Owner.IsDependencyProject == true) return;
#endif
					if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + D.Name + "' enumeration? Doing so will also delete anything contained in the enumeration.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
					{
						CloseProjectItem(D);
						if (D.Parent.Enums.Count == 0) D.Parent.Owner.Enums.Remove(D);
						else D.Parent.Enums.Remove(D);
					}
				}
			}
		}

		private void CloseProject(Projects.Project Project)
		{
			foreach (Projects.Namespace N in Project.Namespaces)
				N.CloseItems();

			foreach (Projects.Enum E in Project.Enums)
			{
				if (E.IsOpen == true)
					CloseProjectItem(E);
			}

			foreach (Projects.Data D in Project.Data)
			{
				if (D.IsOpen == true)
					CloseProjectItem(D);
			}

			foreach (Projects.Service S in Project.Services)
			{
				if (S.IsOpen == true)
					CloseProjectItem(S);
			}

			CloseProjectItem(Project);
		}

		public void CloseProjectItem(object Item)
		{
			if (Item == null) return;
			Type valueType = Item.GetType();

			if (valueType == typeof(Projects.Project) || valueType == typeof(Projects.DependencyProject))
			{
				foreach (Themes.C1DockTabItemWindow DW in ProjectTabs.Items)
				{
					if (DW.Content.GetType() == typeof(Interface.Project.Project))
					{
						Interface.Project.Project TD = DW.Content as Interface.Project.Project;
						if (TD.Settings == Item) { ProjectTabs.Items.Remove(DW); break; }
					}
				}
			}
			if (valueType == typeof(Projects.Namespace))
			{
				foreach (Themes.C1DockTabItemWindow DW in ProjectTabs.Items)
				{
					if (DW.Content.GetType() == typeof(Interface.Namespace.Namespace))
					{
						Interface.Namespace.Namespace TD = DW.Content as Interface.Namespace.Namespace;
						if (TD.Data == Item) { ProjectTabs.Items.Remove(DW); break; }
					}
				}
			}
			if (valueType == typeof(Projects.Service))
			{
				foreach (Themes.C1DockTabItemWindow DW in ProjectTabs.Items)
				{
					if (DW.Content.GetType() == typeof(Interface.Service.Service))
					{
						Interface.Service.Service TD = DW.Content as Interface.Service.Service;
						if (TD.Data == Item) { ProjectTabs.Items.Remove(DW); break; }
					}
				}
			}
			if (valueType == typeof(Projects.Data))
			{
				foreach (Themes.C1DockTabItemWindow DW in ProjectTabs.Items)
				{
					if (DW.Content.GetType() == typeof(Interface.Data.Data))
					{
						Interface.Data.Data TD = DW.Content as Interface.Data.Data;
						if (TD.PData == Item) { ProjectTabs.Items.Remove(DW); break; }
					}
				}
			}
			if (valueType == typeof(Projects.Enum))
			{
				foreach (Themes.C1DockTabItemWindow DW in ProjectTabs.Items)
				{
					if (DW.Content.GetType() == typeof(Interface.Enum.Enum))
					{
						Interface.Enum.Enum TD = DW.Content as Interface.Enum.Enum;
						if (TD.Data == Item) { ProjectTabs.Items.Remove(DW); break; }
					}
				}
			}
		}

		public void RefreshSolutionNavigator()
		{
			if (Globals.Projects == null) return;

			if (SolutionNavigatorShowAll == true)
			{
				int Count = Globals.Projects.Count;

				if (Count == 1)
					SolutionNavigatorHeaderTitle.Text = "Solution '" + Globals.Solution.Name + "' (1 Project)";
				else
					SolutionNavigatorHeaderTitle.Text = "Solution '" + Globals.Solution.Name + "' (" + Count.ToString() + " Projects)";
			}

			if (SolutionNavigatorShowNamespaces == true)
			{
				int Count = 0;
				foreach (Projects.Project P in Globals.Projects)
				{
					Count = P.Namespaces.Count;
					foreach (Projects.Namespace N in P.Namespaces)
						Count += CountNamespaces(N);
				}

				if (Count == 1)
					SolutionNavigatorHeaderTitle.Text = "Solution '" + Globals.Solution.Name + "' (1 Namespace)";
				else
					SolutionNavigatorHeaderTitle.Text = "Solution '" + Globals.Solution.Name + "' (" + Count.ToString() + " Namespaces)";
			}

			if (SolutionNavigatorShowServices == true)
			{
				int Count = 0;
				foreach (Projects.Project P in Globals.Projects)
				{
					foreach (Projects.Namespace N in P.Namespaces)
						Count += CountServices(N);
				}

				if (Count == 1)
					SolutionNavigatorHeaderTitle.Text = "Solution '" + Globals.Solution.Name + "' (1 Service)";
				else
					SolutionNavigatorHeaderTitle.Text = "Solution '" + Globals.Solution.Name + "' (" + Count.ToString() + " Serivces)";
			}

			if (SolutionNavigatorShowData == true)
			{
				int Count = 0;
				foreach (Projects.Project P in Globals.Projects)
				{
					foreach (Projects.Namespace N in P.Namespaces)
						Count += CountData(N);
				}

				if (Count == 1)
					SolutionNavigatorHeaderTitle.Text = "Solution '" + Globals.Solution.Name + "' (1 Data)";
				else
					SolutionNavigatorHeaderTitle.Text = "Solution '" + Globals.Solution.Name + "' (" + Count.ToString() + " Data)";
			}

			if (SolutionNavigatorShowEnums == true)
			{
				int Count = 0;
				foreach (Projects.Project P in Globals.Projects)
				{
					foreach (Projects.Namespace N in P.Namespaces)
						Count += CountEnums(N);
				}

				if (Count == 1)
					SolutionNavigatorHeaderTitle.Text = "Solution '" + Globals.Solution.Name + "' (1 Enum)";
				else
					SolutionNavigatorHeaderTitle.Text = "Solution '" + Globals.Solution.Name + "' (" + Count.ToString() + " Enums)";
			}

			SolutionNavigatorHeaderTitle.ToolTip = SolutionNavigatorHeaderTitle.Text;

			SolutionNavigatorView.ItemsSource = null;
			foreach (Projects.Project P in Globals.Projects)
				P.Filter(SolutionNavigatorShowAll, SolutionNavigatorShowNamespaces, SolutionNavigatorShowServices, SolutionNavigatorShowData, SolutionNavigatorShowEnums);
			SolutionNavigatorView.ItemsSource = Globals.Projects;
		}

		private int CountNamespaces(Projects.Namespace Parent)
		{
			int Count = 0;
			Count = Parent.Children.Count;
			foreach (Projects.Namespace N in Parent.Children)
				Count += CountNamespaces(N);

			return Count;
		}

		private int CountServices(Projects.Namespace Parent)
		{
			int Count = 0;
			Count = Parent.Services.Count;
			foreach (Projects.Namespace N in Parent.Children)
				Count += CountServices(N);

			return Count;
		}

		private int CountData(Projects.Namespace Parent)
		{
			int Count = 0;
			Count = Parent.Data.Count;
			foreach (Projects.Namespace N in Parent.Children)
				Count += CountData(N);

			return Count;
		}

		private int CountEnums(Projects.Namespace Parent)
		{
			int Count = 0;
			Count = Parent.Enums.Count;
			foreach (Projects.Namespace N in Parent.Children)
				Count += CountEnums(N);

			return Count;
		}

		#endregion

		#region - Command Handlers -

		private static void OnBuildProjectCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Globals.SaveProjectSpace();

			Globals.MainScreen.SolutionNavigatorTab.IsEnabled = false;
			Globals.MainScreen.ProjectTabs.IsEnabled = false;
			Globals.MainScreen.OutputTab.IsSelected = true;
			Globals.MainScreen.DockOutput.SlideOpen();
			if (e.Parameter.GetType() == typeof(WCFArchitect.Projects.Project))
			{
				WCFArchitect.Projects.Project TD = e.Parameter as WCFArchitect.Projects.Project;
				Globals.CompilerManager.BuildProject(TD);
			}
			else if (e.Parameter.GetType() == typeof(WCFArchitect.Projects.Namespace))
			{
				WCFArchitect.Projects.Namespace TD = e.Parameter as WCFArchitect.Projects.Namespace;
				Globals.CompilerManager.BuildProject(TD.Owner);
			}
		}

		private static void OnOutputProjectCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Globals.SaveProjectSpace();

			Globals.MainScreen.SolutionNavigatorTab.IsEnabled = false;
			Globals.MainScreen.ProjectTabs.IsEnabled = false;
			Globals.MainScreen.OutputTab.IsSelected = true;
			Globals.MainScreen.DockOutput.SlideOpen();
			if (e.Parameter.GetType() == typeof(WCFArchitect.Projects.Project))
			{
				Interface.Project.Project TD = e.Parameter as Interface.Project.Project;
				Globals.CompilerManager.BuildProjectOutput(TD.Settings);
			}
			else if (e.Parameter.GetType() == typeof(WCFArchitect.Projects.Namespace))
			{
				Interface.Namespace.Namespace TD = e.Parameter as Interface.Namespace.Namespace;
				Globals.CompilerManager.BuildProjectOutput(TD.Data.Owner);
			}
		}

		private static void OnAddItemCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Projects.Project D = e.Parameter as Projects.Project;
			if (D == null) return;
			Globals.MainScreen.OpenNewItemScreen();
		}

		private static void OnAddBindingCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Projects.Project D = e.Parameter as Projects.Project;
			if (D == null) return;
			if (e.Parameter.GetType() == typeof(Projects.Project) || e.Parameter.GetType() == typeof(Projects.DependencyProject))
			{
				Globals.MainScreen.NewItemTypesList.SelectedIndex = 4;
				Globals.MainScreen.NewItemProjectsList.SelectedItem = e.Parameter;
				Globals.MainScreen.OpenNewItemScreen();
			}
		}

		private static void OnAddSecurityCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Projects.Project D = e.Parameter as Projects.Project;
			if (D == null) return;
			if (e.Parameter.GetType() == typeof(Projects.Project) || e.Parameter.GetType() == typeof(Projects.DependencyProject))
			{
				Globals.MainScreen.NewItemTypesList.SelectedIndex = 5;
				Globals.MainScreen.NewItemProjectsList.SelectedItem = e.Parameter;
				Globals.MainScreen.OpenNewItemScreen();
			}
		}

		private static void OnAddHostCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Parameter.GetType() == typeof(Projects.Project) || e.Parameter.GetType() == typeof(Projects.DependencyProject))
			{
				Projects.Project D = e.Parameter as Projects.Project;
				Projects.Host t = new Projects.Host("New Host", D);
				D.Hosts.Add(t);
				Globals.MainScreen.OpenProjectItem(t);
			}
		}

		private static void OnAddNamespaceCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Parameter.GetType() == typeof(Projects.Project) || e.Parameter.GetType() == typeof(Projects.DependencyProject))
			{
				Projects.Project D = e.Parameter as Projects.Project;
				if (D == null) return;
				Globals.IsLoading = true;
				Projects.Namespace t = new Projects.Namespace("New Namespace", D.Namespace, D);
				Globals.IsLoading = false;
				D.Namespaces.Add(t);
				Globals.MainScreen.OpenProjectItem(t);
			}
			if (e.Parameter.GetType() == typeof(Projects.Namespace))
			{
				Projects.Namespace D = e.Parameter as Projects.Namespace;
				Globals.IsLoading = true;
				Projects.Namespace t = new Projects.Namespace("New Namespace", D, D.Owner);
				Globals.IsLoading = false;
				D.Children.Add(t);
				Globals.MainScreen.OpenProjectItem(t);
			}
		}

		private static void OnAddServiceCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Parameter.GetType() == typeof(Projects.Project) || e.Parameter.GetType() == typeof(Projects.DependencyProject))
			{
				Projects.Project D = e.Parameter as Projects.Project;
				if (D == null) return;
				Globals.IsLoading = true;
				Projects.Service t = new Projects.Service("New Service", D.Namespace);
				Globals.IsLoading = false;
				D.Services.Add(t);
				Globals.MainScreen.OpenProjectItem(t);
			}
			if (e.Parameter.GetType() == typeof(Projects.Namespace))
			{
				Projects.Namespace D = e.Parameter as Projects.Namespace;
				if (D == null) return;
				Globals.IsLoading = true;
				Projects.Service t = new Projects.Service("New Service", D);
				Globals.IsLoading = false;
				D.Services.Add(t);
				Globals.MainScreen.OpenProjectItem(t);
			}
		}

		private static void OnAddDataCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Parameter.GetType() == typeof(Projects.Project) || e.Parameter.GetType() == typeof(Projects.DependencyProject))
			{
				Projects.Project D = e.Parameter as Projects.Project;
				if (D == null) return;
				Globals.IsLoading = true;
				Projects.Data t = new Projects.Data("New Data", D.Namespace);
				Globals.IsLoading = false;
				D.Data.Add(t);
				Globals.MainScreen.OpenProjectItem(t);
			}
			if (e.Parameter.GetType() == typeof(Projects.Namespace))
			{
				Projects.Namespace D = e.Parameter as Projects.Namespace;
				if (D == null) return;
				Globals.IsLoading = true;
				Projects.Data t = new Projects.Data("New Data", D);
				Globals.IsLoading = false;
				D.Data.Add(t);
				Globals.MainScreen.OpenProjectItem(t);
			}
		}

		private static void OnAddEnumCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Parameter.GetType() == typeof(Projects.Project) || e.Parameter.GetType() == typeof(Projects.DependencyProject))
			{
				Projects.Project D = e.Parameter as Projects.Project;
				if (D == null) return;
				Globals.IsLoading = true;
				Projects.Enum t = new Projects.Enum("New Enum", D.Namespace);
				Globals.IsLoading = false;
				D.Enums.Add(t);
				Globals.MainScreen.OpenProjectItem(t);
			}
			if (e.Parameter.GetType() == typeof(Projects.Namespace))
			{
				Projects.Namespace D = e.Parameter as Projects.Namespace;
				if (D == null) return;
				Globals.IsLoading = true;
				Projects.Enum t = new Projects.Enum("New Enum", D);
				Globals.IsLoading = false;
				D.Enums.Add(t);
				Globals.MainScreen.OpenProjectItem(t);
			}
		}

		private static void OnDeleteItemCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Parameter.GetType() == typeof(Projects.Project) || e.Parameter.GetType() == typeof(Projects.DependencyProject))
			{
				Projects.Project D = e.Parameter as Projects.Project;
				if (D == null) return;
				if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + D.Name + "' project? Doing so will also delete anything contained within the project.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					Globals.MainScreen.CloseProject(D);
					Globals.Projects.Remove(D);
					Globals.ProjectSpace.Remove(D);
				}
			}
			if (e.Parameter.GetType() == typeof(Projects.Namespace))
			{
				Projects.Namespace D = e.Parameter as Projects.Namespace;
				if (D == null) return;
				if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + D.Name + "' namespace? Doing so will also delete anything contained in the namespace.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					Globals.MainScreen.CloseProjectItem(D);
					D.Parent.Children.Remove(D);
					D.Owner.Namespaces.Remove(D);
				}
			}
			//if (e.Parameter.GetType() == typeof(Projects.Service))
			//{
			//    Projects.Service D = e.Parameter as Projects.Service;
			//    if (D == null) return;
			//    if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + D.Name + "' service? Doing so will also delete anything contained in the service.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			//    {
			//        Globals.MainScreen.CloseProjectItem(D);
			//        D.Parent.Services.Remove(D);
			//    }
			//}
			//if (e.Parameter.GetType() == typeof(Projects.Data))
			//{
			//    Projects.Data D = e.Parameter as Projects.Data;
			//    if (D == null) return;
			//    if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + D.Name + "' data object? Doing so will also delete anything contained in the data object.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			//    {
			//        Globals.MainScreen.CloseProjectItem(D);
			//        D.Parent.Data.Remove(D);
			//    }
			//}
			//if (e.Parameter.GetType() == typeof(Projects.Enum))
			//{
			//    Projects.Enum D = e.Parameter as Projects.Enum;
			//    if (D == null) return;
			//    if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + D.Name + "' enumeration? Doing so will also delete anything contained in the enumeration.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			//    {
			//        Globals.MainScreen.CloseProjectItem(D);
			//        D.Parent.Enums.Remove(D);
			//    }
			//}
		}

		#endregion

		#region - New Item Handlers -

		private void NewItemTypesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (NewItemTypesList.SelectedItem == null)
			{
				NewItemProjectsList.ItemsSource = null;
				NewItemProjectsList.Visibility = System.Windows.Visibility.Collapsed;
				NewItemProjectNamespaces.Visibility = System.Windows.Visibility.Collapsed;
				NewItemBindingTypesList.Visibility = System.Windows.Visibility.Collapsed;
				NewItemSecurityTypesList.Visibility = System.Windows.Visibility.Collapsed;
			}
			else
			{
				NewItemProjectsList.ItemsSource = Globals.Projects;
				NewItemProjectsList.Visibility = System.Windows.Visibility.Visible;
				NewItemProjectsList.Focus();
			}

			NewItemAdd.IsEnabled = false;
			NewItemProjectsList.SelectedItem = null;
			NewItemProjectNamespaceList.ItemsSource = null;
			NewItemBindingTypesList.SelectedItem = null;
			NewItemSecurityTypesList.SelectedItem = null;
			NewItemProjectNamespaces.Visibility = System.Windows.Visibility.Collapsed;
			NewItemBindingTypesList.Visibility = System.Windows.Visibility.Collapsed;
			NewItemSecurityTypesList.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void NewItemProjectNamespaceRoot_Checked(object sender, RoutedEventArgs e)
		{
			if (IsNamespaceListUpdating == true) return;
			NewItemProjectNamespaceList.ItemsSource = null;
			NewItemProjectsList_SelectionChanged(null, null);
		}
		
		private void NewItemProjectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			IsNamespaceListUpdating = true;
			NewItemProjectNamespaceRoot.IsChecked = false;
			if (NewItemTypesList.SelectedItem == null) return;
			if (NewItemProjectsList.SelectedItem == null) return;

			NewItemType NIT = NewItemTypesList.SelectedItem as NewItemType;
			Projects.Project NIP = NewItemProjectsList.SelectedItem as Projects.Project;

			if (NIT.DataType == 1 || NIT.DataType == 2 || NIT.DataType == 3 || NIT.DataType == 4)
			{
				NewItemProjectNamespaceList.ItemsSource = NIP.Namespaces;
				NewItemProjectNamespaceRoot.IsChecked = true;
				NewItemProjectNamespaceRoot.Content = NIP.Namespace.Name;
				NewItemProjectNamespaces.Visibility = System.Windows.Visibility.Visible;
			}
			if (NIT.DataType == 5)
			{
				NewItemBindingTypesList.Visibility = System.Windows.Visibility.Visible;
				NewItemBindingTypesList.Focus();
			}
			if (NIT.DataType == 6)
			{
				NewItemSecurityTypesList.Visibility = System.Windows.Visibility.Visible;
				NewItemSecurityTypesList.Focus();
			}
			if (NIT.DataType == 7)
			{
				NewItemName.Focus();
			}
			IsNamespaceListUpdating = false;
		}

		private void NewItemProjectNamespaceRoot_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (NewItemProjectNamespaceList.SelectedItem != null) NewItemProjectNamespaceRoot.IsChecked = false;
			NewItemName.Focus();
		}

		private void NewItemBindingTypesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			NewItemName.Focus();
		}

		private void NewItemSecurityTypesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			NewItemName.Focus();
		}

		private void NewItemName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				NewItemAdd_Click(null, null);
		}

		private void NewItemName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (NewItemName.Text == "") NewItemAdd.IsEnabled = false;
			else NewItemAdd.IsEnabled = true;
		}

		private void NewItemAdd_Click(object sender, RoutedEventArgs e)
		{
			if (NewItemTypesList.SelectedItem == null) return;
			if (NewItemProjectsList.SelectedItem == null) return;
			Globals.IsLoading = true;

			try
			{
				NewItemType NIT = NewItemTypesList.SelectedItem as NewItemType;
				Projects.Project NIP = NewItemProjectsList.SelectedItem as Projects.Project;

#if STANDARD
				if (NIP.IsDependencyProject == true)
				{
					Prospective.Controls.MessageBox.Show("This version of WCF Architect does not support Dependency Projects, please select a different project.", "Invalid Project Selected", MessageBoxButton.OK, MessageBoxImage.Stop);
					return;
				}
#endif

				if (NIT.DataType == 1)
				{
					if (NewItemProjectNamespaceRoot.IsChecked == true)
					{
						Projects.Namespace NI = new Projects.Namespace(NewItemName.Text, NIP.Namespace, NIP);
						if (NewItemProjectNamespaceList.SelectedItem == null) NIP.Namespaces.Add(NI);
						else NIP.Namespaces.Add(NI);
						Globals.IsLoading = false;
						OpenProjectItem(NI);
					}
					else
					{
						Projects.Namespace NIN = NewItemProjectNamespaceList.SelectedItem as Projects.Namespace;
						Projects.Namespace NI = new Projects.Namespace(NewItemName.Text, NIN, NIP);
						if (NewItemProjectNamespaceList.SelectedItem == null) NIP.Namespaces.Add(NI);
						else NIN.Children.Add(NI);
						Globals.IsLoading = false;
						OpenProjectItem(NI);
					}
				}
				else if (NIT.DataType == 2)
				{
					if (NewItemProjectNamespaceRoot.IsChecked == true)
					{
						Projects.Service NI = new Projects.Service(NewItemName.Text, NIP.Namespace);
						NIP.Services.Add(NI);
						Globals.IsLoading = false;
						OpenProjectItem(NI);
					}
					else
					{
						Projects.Namespace NIN = NewItemProjectNamespaceList.SelectedItem as Projects.Namespace;
						if (NIN == null)
						{
							if (Prospective.Controls.MessageBox.Show("You must select a select a Namespace from the list or use the Root Namespace to create a new Service. Would you like to use the Root Namespace?", "No Namespace Selected", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
								NewItemProjectNamespaceRoot.IsChecked = true;
							return;
						}

						Projects.Service NI = new Projects.Service(NewItemName.Text, NIN);
						NIN.Services.Add(NI);
						Globals.IsLoading = false;
						OpenProjectItem(NI);
					}
				}
				else if (NIT.DataType == 3)
				{
					if (NewItemProjectNamespaceRoot.IsChecked == true)
					{
						Projects.Data NI = new Projects.Data(NewItemName.Text, NIP.Namespace);
						NIP.Data.Add(NI);
						Globals.IsLoading = false;
						OpenProjectItem(NI);
					}
					else
					{
						Projects.Namespace NIN = NewItemProjectNamespaceList.SelectedItem as Projects.Namespace;
						if (NIN == null)
						{
							if (Prospective.Controls.MessageBox.Show("You must select a select a Namespace from the list or use the Root Namespace to create a new Data Object. Would you like to use the Root Namespace?", "No Namespace Selected", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
								NewItemProjectNamespaceRoot.IsChecked = true;
							return;
						}

						Projects.Data NI = new Projects.Data(NewItemName.Text, NIN);
						NIN.Data.Add(NI);
						Globals.IsLoading = false;
						OpenProjectItem(NI);
					}
				}
				else if (NIT.DataType == 4)
				{
					if (NewItemProjectNamespaceRoot.IsChecked == true)
					{
						Projects.Enum NI = new Projects.Enum(NewItemName.Text, NIP.Namespace);
						NIP.Enums.Add(NI);
						Globals.IsLoading = false;
						OpenProjectItem(NI);
					}
					else
					{
						Projects.Namespace NIN = NewItemProjectNamespaceList.SelectedItem as Projects.Namespace;
						if (NIN == null)
						{
							if (Prospective.Controls.MessageBox.Show("You must select a select a Namespace from the list or use the Root Namespace to create a new Enum. Would you like to use the Root Namespace?", "No Namespace Selected", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
								NewItemProjectNamespaceRoot.IsChecked = true;
							return;
						}

						Projects.Enum NI = new Projects.Enum(NewItemName.Text, NIN);
						NIN.Enums.Add(NI);
						Globals.IsLoading = false;
						OpenProjectItem(NI);
					}
				}
				else if (NIT.DataType == 5)
				{
					NewItemType NBT = NewItemBindingTypesList.SelectedItem as NewItemType;
					if (NBT == null)
					{
						Prospective.Controls.MessageBox.Show("You must select a Binding Type for this Binding!", "No Binding Type Selected", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}
					Projects.ServiceBinding NI = null;
					if (NBT.DataType == 1) NI = new Projects.ServiceBindingBasicHTTP(NewItemName.Text, NIP);
					if (NBT.DataType == 2) NI = new Projects.ServiceBindingWSHTTP(NewItemName.Text, NIP);
					if (NBT.DataType == 3) NI = new Projects.ServiceBindingWS2007HTTP(NewItemName.Text, NIP);
					if (NBT.DataType == 4) NI = new Projects.ServiceBindingWSDualHTTP(NewItemName.Text, NIP);
					if (NBT.DataType == 5) NI = new Projects.ServiceBindingWSFederationHTTP(NewItemName.Text, NIP);
					if (NBT.DataType == 6) NI = new Projects.ServiceBindingWS2007FederationHTTP(NewItemName.Text, NIP);
					if (NBT.DataType == 7) NI = new Projects.ServiceBindingTCP(NewItemName.Text, NIP);
					if (NBT.DataType == 8) NI = new Projects.ServiceBindingNamedPipe(NewItemName.Text, NIP);
					if (NBT.DataType == 9) NI = new Projects.ServiceBindingMSMQ(NewItemName.Text, NIP);
					if (NBT.DataType == 10) NI = new Projects.ServiceBindingPeerTCP(NewItemName.Text, NIP);
					if (NBT.DataType == 11) NI = new Projects.ServiceBindingWebHTTP(NewItemName.Text, NIP);
					if (NBT.DataType == 12) NI = new Projects.ServiceBindingMSMQIntegration(NewItemName.Text, NIP);

					NIP.Bindings.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
				else if (NIT.DataType == 6)
				{
					NewItemType NBT = NewItemSecurityTypesList.SelectedItem as NewItemType;
					if (NBT == null)
					{
						Prospective.Controls.MessageBox.Show("You must select a Security Type for this Binding!", "No Security Type Selected", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}

					Projects.BindingSecurity NI = null;
					if (NBT.DataType == 1) NI = new Projects.BindingSecurityBasicHTTP(NewItemName.Text, NIP);
					if (NBT.DataType == 2) NI = new Projects.BindingSecurityWSHTTP(NewItemName.Text, NIP);
					if (NBT.DataType == 3) NI = new Projects.BindingSecurityWSDualHTTP(NewItemName.Text, NIP);
					if (NBT.DataType == 4) NI = new Projects.BindingSecurityWSFederationHTTP(NewItemName.Text, NIP);
					if (NBT.DataType == 5) NI = new Projects.BindingSecurityTCP(NewItemName.Text, NIP);
					if (NBT.DataType == 6) NI = new Projects.BindingSecurityNamedPipe(NewItemName.Text, NIP);
					if (NBT.DataType == 7) NI = new Projects.BindingSecurityMSMQ(NewItemName.Text, NIP);
					if (NBT.DataType == 8) NI = new Projects.BindingSecurityPeerTCP(NewItemName.Text, NIP);
					if (NBT.DataType == 9) NI = new Projects.BindingSecurityWebHTTP(NewItemName.Text, NIP);
					if (NBT.DataType == 10) NI = new Projects.BindingSecurityMSMQIntegration(NewItemName.Text, NIP);

					NIP.Security.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
				else if (NIT.DataType == 7)
				{
					Projects.Host NI = new Projects.Host(NewItemName.Text, NIP);
					NIP.Hosts.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
			}
			finally
			{
				Globals.IsLoading = false;
			}
			NewItemCancel_Click(null, null);
		}

		private void NewItemCancel_Click(object sender, RoutedEventArgs e)
		{
			NewItem.IsChecked = false;
			NewItemAdd.IsEnabled = false;
			NewItemTypesList.SelectedItem = null;
			NewItemProjectsList.SelectedItem = null;
			NewItemProjectNamespaceList.ItemsSource = null;
			NewItemBindingTypesList.SelectedItem = null;
			NewItemSecurityTypesList.SelectedItem = null;
			NewItemName.Text = "";
			NewItemProjectsList.Visibility = System.Windows.Visibility.Collapsed;
			NewItemProjectNamespaces.Visibility = System.Windows.Visibility.Collapsed;
			NewItemBindingTypesList.Visibility = System.Windows.Visibility.Collapsed;
			NewItemSecurityTypesList.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void NewItemClose_Click(object sender, RoutedEventArgs e)
		{
			NewItemCancel_Click(null, null);
		}

		#endregion

		#region - Error List / Output -

		public void ProjectBuildFinished(bool HasErrors)
		{
			BuildProgress.Value++;
			Globals.CompilerManager.CompletedBuildSteps++;

			if (Globals.CompilerManager.CompletedBuildSteps >= Globals.CompilerManager.TotalBuildSteps) BuildCompleted(HasErrors);
		}

		public void BuildCompleted(bool HasErrors)
		{
			if (HasErrors == true)
			{
				ErrorListTab.IsSelected = true;
				DockErrorList.SlideOpen();
				SystemStatus.Text = "Build Failed";
			}
			else
			{
				SystemStatus.Text = "Build Succeeded";
			}

			BuildStatusGroup.Visibility = System.Windows.Visibility.Hidden;
			BuildProgress.Value = 0;
			BuildProgress.Maximum = 0;
			BuildSolution.IsEnabled = true;
			BuildProject.IsEnabled = true;
			SolutionNavigatorTab.IsEnabled = true;
			ProjectTabs.IsEnabled = true;
		}

		#endregion

		#region - Find / Replace -

		private void FindNewSearch_Click(object sender, RoutedEventArgs e)
		{
			TabItem NDW = new TabItem();
			NDW.Header = "New Search";
			NDW.Content = new FindReplace();
			FindReplaceTabs.Items.Add(NDW);
			FindReplaceTabs.SelectedItem = NDW;
		}

		private void FindClose_Click(object sender, RoutedEventArgs e)
		{
			if (FindReplaceTabs.SelectedItem == null) return;
			FindReplaceTabs.Items.Remove(FindReplaceTabs.SelectedItem);
		}

		private void FindNext_Click(object sender, RoutedEventArgs e)
		{
			if (FindReplaceTabs.SelectedItem == null) return;
			TabItem TI = FindReplaceTabs.SelectedItem as TabItem;
			FindReplace FR = TI.Content as FindReplace;
			if (FR == null) return;
			DockViewControl.SlideClose(false);

			if (FR.FindInfo == null) FindAll_Click(null, null);
			TI.Header = "Searching for '" + FR.FindInfo.Search + "'";
			if (FR.FindResultsList.SelectedIndex == FR.FindResults.Count - 1) FR.FindResultsList.SelectedIndex = 0;
			else FR.FindResultsList.SelectedIndex++;
		}

		private void FindReplace_Click(object sender, RoutedEventArgs e)
		{
			if (FindReplaceTabs.SelectedItem == null) return;
			TabItem TI = FindReplaceTabs.SelectedItem as TabItem;
			FindReplace FR = TI.Content as FindReplace;
			if (FR == null) return;
			if (FR.FindInfo == null) return;
			TI.Header = "Searching for '" + FR.FindInfo.Search + "'";

			Globals.IsFinding = true;

			Projects.FindReplaceResult FRR = FR.FindResults[FR.FindResultsList.SelectedIndex];
			Type valueType = FRR.Item.GetType();
			if (valueType == typeof(Projects.Project) || valueType == typeof(Projects.DependencyProject))
			{
				Projects.Project T = FRR.Item as Projects.Project;
				T.Replace(new Projects.FindReplaceInfo(Projects.FindItems.Any, Projects.FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), FRR.Field);
			}
			if (valueType == typeof(Projects.Namespace))
			{
				Projects.Namespace T = FRR.Item as Projects.Namespace;
				T.Replace(new Projects.FindReplaceInfo(Projects.FindItems.Any, Projects.FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), FRR.Field);
			}
			if (valueType == typeof(Projects.Service))
			{
				Projects.Service T = FRR.Item as Projects.Service;
				T.Replace(new Projects.FindReplaceInfo(Projects.FindItems.Any, Projects.FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), FRR.Field);
			}
			if (valueType == typeof(Projects.Operation))
			{
				Projects.Operation T = FRR.Item as Projects.Operation;
				T.Replace(new Projects.FindReplaceInfo(Projects.FindItems.Any, Projects.FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), FRR.Field);
			}
			if (valueType == typeof(Projects.OperationParameter))
			{
				Projects.OperationParameter T = FRR.Item as Projects.OperationParameter;
				T.Replace(new Projects.FindReplaceInfo(Projects.FindItems.Any, Projects.FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), FRR.Field);
			}
			if (valueType == typeof(Projects.Property))
			{
				Projects.Property T = FRR.Item as Projects.Property;
				T.Replace(new Projects.FindReplaceInfo(Projects.FindItems.Any, Projects.FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), FRR.Field);
			}
			if (valueType == typeof(Projects.Data))
			{
				Projects.Data T = FRR.Item as Projects.Data;
				T.Replace(new Projects.FindReplaceInfo(Projects.FindItems.Any, Projects.FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), FRR.Field);
			}
			if (valueType == typeof(Projects.DataElement))
			{
				Projects.DataElement T = FRR.Item as Projects.DataElement;
				T.Replace(new Projects.FindReplaceInfo(Projects.FindItems.Any, Projects.FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), FRR.Field);
			}
			if (valueType == typeof(Projects.Enum))
			{
				Projects.Enum T = FRR.Item as Projects.Enum;
				T.Replace(new Projects.FindReplaceInfo(Projects.FindItems.Any, Projects.FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), FRR.Field);
			}
			if (valueType == typeof(Projects.EnumElement))
			{
				Projects.EnumElement T = FRR.Item as Projects.EnumElement;
				T.Replace(new Projects.FindReplaceInfo(Projects.FindItems.Any, Projects.FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), FRR.Field);
			}
			if (valueType == typeof(Projects.ServiceBinding))
			{
				Projects.ServiceBinding T = FRR.Item as Projects.ServiceBinding;
				T.Replace(new Projects.FindReplaceInfo(Projects.FindItems.Any, Projects.FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), FRR.Field);
			}
			if (valueType == typeof(Projects.BindingSecurity))
			{
				Projects.BindingSecurity T = FRR.Item as Projects.BindingSecurity;
				T.Replace(new Projects.FindReplaceInfo(Projects.FindItems.Any, Projects.FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), FRR.Field);
			}
			if (valueType == typeof(Projects.Host))
			{
				Projects.Host T = FRR.Item as Projects.Host;
				T.Replace(new Projects.FindReplaceInfo(Projects.FindItems.Any, Projects.FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), FRR.Field);
			}
			if (valueType == typeof(Projects.HostEndpoint))
			{
				Projects.HostEndpoint T = FRR.Item as Projects.HostEndpoint;
				T.Replace(new Projects.FindReplaceInfo(Projects.FindItems.Any, Projects.FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), FRR.Field);
			}
			if (valueType == typeof(Projects.HostBehavior))
			{
				Projects.HostBehavior T = FRR.Item as Projects.HostBehavior;
				T.Replace(new Projects.FindReplaceInfo(Projects.FindItems.Any, Projects.FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), FRR.Field);
			}

			Globals.IsFinding = false;
		}

		private void FindAll_Click(object sender, RoutedEventArgs e)
		{
			if (FindReplaceTabs.SelectedItem == null) return;
			TabItem TI = FindReplaceTabs.SelectedItem as TabItem;
			FindReplace FR = TI.Content as FindReplace;
			if (FR == null) return;

			Globals.IsFinding = true;

			FR.FindResults.Clear();
			FR.FindInfo = new Projects.FindReplaceInfo((Projects.FindItems)FindItem.SelectedIndex, (Projects.FindLocations)FindLocation.SelectedIndex, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value);
			List<Projects.FindReplaceResult> results = new List<Projects.FindReplaceResult>();
			TI.Header = "Searching for '" + FR.FindInfo.Search + "'";

			if (FR.FindInfo.Location == Projects.FindLocations.EntireSolution)
			{
				foreach (Projects.Project P in Globals.Projects)
					results.AddRange(P.FindReplace(FR.FindInfo));
			}
			if (FR.FindInfo.Location == Projects.FindLocations.CurrentProject)
			{
				foreach (Projects.OpenableDocument OD in Globals.OpenDocuments)
				{
					if (OD.IsActive == false) continue;
					Type valueType = OD.GetType();
					if (valueType == typeof(Projects.Project) || valueType == typeof(Projects.DependencyProject))
					{
						Projects.Project P = OD as Projects.Project;
						results.AddRange(P.FindReplace(FR.FindInfo));
					}
					if (valueType == typeof(Projects.Namespace))
					{
						Projects.Namespace P = OD as Projects.Namespace;
						results.AddRange(P.Owner.FindReplace(FR.FindInfo));
					}
					if (valueType == typeof(Projects.Service))
					{
						Projects.Service P = OD as Projects.Service;
						results.AddRange(P.Parent.Owner.FindReplace(FR.FindInfo));
					}
					if (valueType == typeof(Projects.Data))
					{
						Projects.Data P = OD as Projects.Data;
						results.AddRange(P.Parent.Owner.FindReplace(FR.FindInfo));
					}
					if (valueType == typeof(Projects.Enum))
					{
						Projects.Enum P = OD as Projects.Enum;
						results.AddRange(P.Parent.Owner.FindReplace(FR.FindInfo));
					}
				}
			}
			if (FR.FindInfo.Location == Projects.FindLocations.OpenDocuments)
			{
				foreach (Projects.OpenableDocument OD in Globals.OpenDocuments)
				{
					Type valueType = OD.GetType();
					if (valueType == typeof(Projects.Project) || valueType == typeof(Projects.DependencyProject))
					{
						Projects.Project P = OD as Projects.Project;
						results.AddRange(P.FindReplace(FR.FindInfo));
					}
					if (valueType == typeof(Projects.Namespace))
					{
						Projects.Namespace P = OD as Projects.Namespace;
						results.AddRange(P.FindReplace(FR.FindInfo));
					}
					if (valueType == typeof(Projects.Service))
					{
						Projects.Service P = OD as Projects.Service;
						results.AddRange(P.FindReplace(FR.FindInfo));
					}
					if (valueType == typeof(Projects.Data))
					{
						Projects.Data P = OD as Projects.Data;
						results.AddRange(P.FindReplace(FR.FindInfo));
					}
					if (valueType == typeof(Projects.Enum))
					{
						Projects.Enum P = OD as Projects.Enum;
						results.AddRange(P.FindReplace(FR.FindInfo));
					}
				}
			}

			foreach (Projects.FindReplaceResult FRR in results)
				FR.FindResults.Add(FRR);

			Globals.IsFinding = false;
		}

		private void FindReplaceAll_Click(object sender, RoutedEventArgs e)
		{
			if (FindReplaceTabs.SelectedItem == null) return;
			TabItem TI = FindReplaceTabs.SelectedItem as TabItem;
			FindReplace FR = TI.Content as FindReplace;
			if (FR == null) return;

			Globals.IsFinding = true;

			FR.FindResults.Clear();
			FR.FindInfo = new Projects.FindReplaceInfo((Projects.FindItems)FindItem.SelectedIndex, (Projects.FindLocations)FindLocation.SelectedIndex, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text);
			List<Projects.FindReplaceResult> results = new List<Projects.FindReplaceResult>();
			TI.Header = "Searching for '" + FR.FindInfo.Search + "'";

			if (FR.FindInfo.Location == Projects.FindLocations.EntireSolution)
			{
				foreach (Projects.Project P in Globals.Projects)
					results.AddRange(P.FindReplace(FR.FindInfo));
			}
			if (FR.FindInfo.Location == Projects.FindLocations.CurrentProject)
			{
				foreach (Projects.OpenableDocument OD in Globals.OpenDocuments)
				{
					if (OD.IsActive == false) continue;
					Type valueType = OD.GetType();
					if (valueType == typeof(Projects.Project) || valueType == typeof(Projects.DependencyProject))
					{
						Projects.Project P = OD as Projects.Project;
						results.AddRange(P.FindReplace(FR.FindInfo));
					}
					if (valueType == typeof(Projects.Namespace))
					{
						Projects.Namespace P = OD as Projects.Namespace;
						results.AddRange(P.Owner.FindReplace(FR.FindInfo));
					}
					if (valueType == typeof(Projects.Service))
					{
						Projects.Service P = OD as Projects.Service;
						results.AddRange(P.Parent.Owner.FindReplace(FR.FindInfo));
					}
					if (valueType == typeof(Projects.Data))
					{
						Projects.Data P = OD as Projects.Data;
						results.AddRange(P.Parent.Owner.FindReplace(FR.FindInfo));
					}
					if (valueType == typeof(Projects.Enum))
					{
						Projects.Enum P = OD as Projects.Enum;
						results.AddRange(P.Parent.Owner.FindReplace(FR.FindInfo));
					}
				}
			}
			if (FR.FindInfo.Location == Projects.FindLocations.OpenDocuments)
			{
				foreach (Projects.OpenableDocument OD in Globals.OpenDocuments)
				{
					Type valueType = OD.GetType();
					if (valueType == typeof(Projects.Project) || valueType == typeof(Projects.DependencyProject))
					{
						Projects.Project P = OD as Projects.Project;
						results.AddRange(P.FindReplace(FR.FindInfo));
					}
					if (valueType == typeof(Projects.Namespace))
					{
						Projects.Namespace P = OD as Projects.Namespace;
						results.AddRange(P.FindReplace(FR.FindInfo));
					}
					if (valueType == typeof(Projects.Service))
					{
						Projects.Service P = OD as Projects.Service;
						results.AddRange(P.FindReplace(FR.FindInfo));
					}
					if (valueType == typeof(Projects.Data))
					{
						Projects.Data P = OD as Projects.Data;
						results.AddRange(P.FindReplace(FR.FindInfo));
					}
					if (valueType == typeof(Projects.Enum))
					{
						Projects.Enum P = OD as Projects.Enum;
						results.AddRange(P.FindReplace(FR.FindInfo));
					}
				}
			}

			foreach (Projects.FindReplaceResult FRR in results)
				FR.FindResults.Add(FRR);

			Globals.IsFinding = false;
		}

		private void FindReplaceTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (FindReplaceTabs.SelectedItem == null) return;
			TabItem TI = FindReplaceTabs.SelectedItem as TabItem;
			FindReplace FR = TI.Content as FindReplace;
			if (FR == null) return;
			if (FR.FindInfo == null) return;

			TI.Header = FR.FindInfo.Search;
			FindValue.Text = FR.FindInfo.Search;
			FindReplaceValue.Text = FR.FindInfo.Replace;
			FindItem.SelectedIndex = (int)FR.FindInfo.Items;
			FindLocation.SelectedIndex = (int)FR.FindInfo.Location;
			FindUseRegex.IsChecked = FR.FindInfo.UseRegex;
			FindMatchCase.IsChecked = FR.FindInfo.MatchCase;
		}

		#endregion

		#region - Window Switcher -

		private void SwitcherDocumentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			object Item = SwitcherDocumentList.SelectedItem;
			if (Item == null) return;
			Type valueType = Item.GetType();

			if (valueType == typeof(Projects.DependencyProject))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Project.Project))
					{
						Interface.Project.Project TD = TI.Content as Interface.Project.Project;
						if (TD.Settings == Item as Projects.Project)
						{
							TI.IsSelected = true;
							TD.ProjectTab.IsSelected = true;
						}
					}
				}

				Projects.DependencyProject D = Item as Projects.DependencyProject;
				SwitcherSelectedItemImage.Source = new BitmapImage(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/DependencyProject.png"));
				SwitcherSelectedItemName.Text = D.Name;
				SwitcherSelectedItemNamespace.Text = D.Namespace.Name;
				SwitcherSelectedItemProject.Text = D.Name;
			}
			else if (valueType == typeof(Projects.Project))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Project.Project))
					{
						Interface.Project.Project TD = TI.Content as Interface.Project.Project;
						if (TD.Settings == Item as Projects.Project)
						{
							TI.IsSelected = true;
							TD.ProjectTab.IsSelected = true;
						}
					}
				}

				Projects.Project D = Item as Projects.Project;
				SwitcherSelectedItemImage.Source = new BitmapImage(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/Project.png"));
				SwitcherSelectedItemName.Text = D.Name;
				SwitcherSelectedItemNamespace.Text = D.Namespace.Name;
				SwitcherSelectedItemProject.Text = D.Name;
			}
			else if (valueType == typeof(Projects.Namespace))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Namespace.Namespace))
					{
						Interface.Namespace.Namespace TD = TI.Content as Interface.Namespace.Namespace;
						if (TD.Data == Item as Projects.Namespace)
						{
							TI.IsSelected = true;
						}
					}
				}

				Projects.Namespace D = Item as Projects.Namespace;
				SwitcherSelectedItemImage.Source = new BitmapImage(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/Namespace.png"));
				SwitcherSelectedItemName.Text = D.Name;
				if(D.Parent != null) SwitcherSelectedItemNamespace.Text = D.Parent.FullName;
				else SwitcherSelectedItemNamespace.Text = D.Owner.Namespace.FullName;
				SwitcherSelectedItemProject.Text = D.Owner.Name;
			}
			else if (valueType == typeof(Projects.Service))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Service.Service))
					{
						Interface.Service.Service TD = TI.Content as Interface.Service.Service;
						if (TD.Data == Item as Projects.Service)
						{
							TI.IsSelected = true;
						}
					}
				}

				Projects.Service D = Item as Projects.Service;
				SwitcherSelectedItemImage.Source = new BitmapImage(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/Service.png"));
				SwitcherSelectedItemName.Text = D.Name;
				SwitcherSelectedItemNamespace.Text = D.Parent.FullName;
				SwitcherSelectedItemProject.Text = D.Parent.Owner.Name;
			}
			else if (valueType == typeof(Projects.Data))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Data.Data))
					{
						Interface.Data.Data TD = TI.Content as Interface.Data.Data;
						if (TD.PData == Item as Projects.Data)
						{
							TI.IsSelected = true;
						}
					}
				}

				Projects.Data D = Item as Projects.Data;
				SwitcherSelectedItemImage.Source = new BitmapImage(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/Data.png"));
				SwitcherSelectedItemName.Text = D.Name;
				SwitcherSelectedItemNamespace.Text = D.Parent.FullName;
				SwitcherSelectedItemProject.Text = D.Parent.Owner.Name;
			}
			else if (valueType == typeof(Projects.Enum))
			{
				foreach (C1DockTabItem TI in ProjectTabs.Items)
				{
					if (TI.Content.GetType() == typeof(Interface.Enum.Enum))
					{
						Interface.Enum.Enum TD = TI.Content as Interface.Enum.Enum;
						if (TD.Data == Item as Projects.Enum)
						{
							TI.IsSelected = true;
						}
					}
				}

				Projects.Enum D = Item as Projects.Enum;
				SwitcherSelectedItemImage.Source = new BitmapImage(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/Enum.png"));
				SwitcherSelectedItemName.Text = D.Name;
				SwitcherSelectedItemNamespace.Text = D.Parent.FullName;
				SwitcherSelectedItemProject.Text = D.Parent.Owner.Name;
			}
		}

		#endregion

		#region - Solution Navigator Drag /Drop -

		private bool IsDragMouseInBounds = false;

		private void SolutionNavigatorView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			IsDragMouseInBounds = true;
			DragStartPos = e.GetPosition(null);

			Point t = e.GetPosition(SolutionNavigatorView);
			if ((t.X > SolutionNavigatorView.ActualWidth - SystemParameters.ScrollWidth) || (t.Y > SolutionNavigatorView.ActualHeight - SystemParameters.ScrollHeight))
				IsDragMouseInBounds = false;
		}

		private void SolutionNavigatorView_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (IsDragMouseInBounds == false) return;
			if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
			{
				Point position = e.GetPosition(null);

				if (Math.Abs(position.X - DragStartPos.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - DragStartPos.Y) > SystemParameters.MinimumVerticalDragDistance)
				{
					StartSolutionDrag(e);
				}
			}
		}

		private void StartSolutionDrag(MouseEventArgs e)
		{
			if (SolutionNavigatorView.SelectedItem == null) return;

			DragSolutionItem = SolutionNavigatorView.SelectedItem;
			Type DSIT = DragSolutionItem.GetType();
			if (DSIT != typeof(Projects.Project) && DSIT != typeof(Projects.DependencyProject) && DSIT != typeof(Projects.Namespace) && DSIT != typeof(Projects.Service) && DSIT != typeof(Projects.Data) && DSIT != typeof(Projects.Enum)) return;

#if STANDARD
			else if (DSIT == typeof(Projects.Namespace))
			{
				Projects.Namespace t = DragSolutionItem as Projects.Namespace;
				if (t.Owner.IsDependencyProject == true) return;
			}
			else if (DSIT == typeof(Projects.Service))
			{
				Projects.Service t = DragSolutionItem as Projects.Service;
				if (t.Parent.Owner.IsDependencyProject == true) return;
			}
			else if (DSIT == typeof(Projects.Data))
			{
				Projects.Data t = DragSolutionItem as Projects.Data;
				if (t.Parent.Owner.IsDependencyProject == true) return;
			}
			else if (DSIT == typeof(Projects.Enum))
			{
				Projects.Enum t = DragSolutionItem as Projects.Enum;
				if (t.Parent.Owner.IsDependencyProject == true) return;
			}
#endif

			TreeViewItem tuie = (TreeViewItem)SolutionNavigatorView.ContainerFromItem(DragSolutionItem);
			if (tuie == null) return;
			try { DragAdorner = new Themes.DragAdorner(SolutionNavigatorView, tuie, true, 0.5); }
			catch { return; }
			DragLayer = AdornerLayer.GetAdornerLayer(SolutionNavigatorView as Visual);
			DragLayer.Add(DragAdorner);

			IsDragging = true;
			DragHasLeftScope = false;

			DataObject data = new DataObject(System.Windows.DataFormats.Text.ToString(), "");
			DragDropEffects de = DragDrop.DoDragDrop(SolutionNavigatorView, data, DragDropEffects.Move);

			AdornerLayer.GetAdornerLayer(SolutionNavigatorView).Remove(DragAdorner);
			DragAdorner = null;

			IsDragging = false;
		}

		private void SolutionNavigatorView_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			if (DragHasLeftScope)
			{
				e.Action = DragAction.Cancel;
				e.Handled = true;
				if (LastHitItem != null) LastHitItem.Margin = new Thickness(0);
			}
		}

		private void SolutionNavigatorView_DragLeave(object sender, DragEventArgs e)
		{
			if (e.OriginalSource == SolutionNavigatorView)
			{
				Point p = e.GetPosition(SolutionNavigatorView);
				Rect r = VisualTreeHelper.GetContentBounds(SolutionNavigatorView);
				if (!r.Contains(p))
				{
					this.DragHasLeftScope = true;
					e.Handled = true;
				}
			}
		}

		private void SolutionNavigatorView_PreviewDragOver(object sender, DragEventArgs e)
		{
			if (DragAdorner != null)
			{
				DragAdorner.LeftOffset = e.GetPosition(SolutionNavigatorView).X;
				DragAdorner.TopOffset = e.GetPosition(SolutionNavigatorView).Y;
			}

			Point MP = e.GetPosition(SolutionNavigatorView);
			HitTestResult htr = VisualTreeHelper.HitTest(SolutionNavigatorView, MP);
			if (htr != null)
			{
				//Get the current hit item and deal with the margins.
				TreeViewItem tviht = Globals.GetVisualParent<TreeViewItem>(htr.VisualHit);
				if (tviht == null) return;
				if (LastHitItem == tviht) return;
				if (LastHitItem != null) LastHitItem.Margin = new Thickness(0);
				LastHitItem = tviht;

				Type DSIType = DragSolutionItem.GetType();
				Type TSIType = tviht.Header.GetType();

				if (DSIType == typeof(Projects.Namespace))
				{
					if (TSIType == typeof(Projects.Namespace))
					{
						tviht.Margin = new Thickness(0, 0, 0, DragAdorner.ActualHeight);
						DropSolutionItem = tviht.Header;
					}
					else if (TSIType == typeof(Projects.Project) || TSIType == typeof(Projects.DependencyProject))
					{
						tviht.Margin = new Thickness(0, 0, 0, DragAdorner.ActualHeight);
						DropSolutionItem = tviht.Header;
						tviht.IsExpanded = true;
					}
				}

				if (DSIType == typeof(Projects.Service) || DSIType == typeof(Projects.Data) || DSIType == typeof(Projects.Enum))
				{
					if (TSIType == typeof(Projects.Namespace) || TSIType == typeof(Projects.Service) || TSIType == typeof(Projects.Data) || TSIType == typeof(Projects.Enum))
					{
						tviht.Margin = new Thickness(0, 0, 0, DragAdorner.ActualHeight);
						DropSolutionItem = tviht.Header;
					}
					else if (TSIType == typeof(Projects.Project) || TSIType == typeof(Projects.DependencyProject))
					{
						DropSolutionItem = tviht.Header;
						tviht.Margin = new Thickness(0, 0, 0, DragAdorner.ActualHeight);
						tviht.IsExpanded = true;
					}
				}
			}
		}

		private void SolutionNavigatorView_Drop(object sender, DragEventArgs e)
		{
			if (LastHitItem != null) LastHitItem.Margin = new Thickness(0);
			if (DropSolutionItem == null) return;

			Type DSIType = DragSolutionItem.GetType();
			Type TSIType = DropSolutionItem.GetType();

			if (DSIType == typeof(Projects.Namespace))
			{
				if (TSIType == typeof(Projects.Project))
				{
					Projects.Namespace DSI = DragSolutionItem as Projects.Namespace;
					Projects.Project TSI = DropSolutionItem as Projects.Project;
					if (DSI == null) return;
					if (TSI == null) return;

					//Move Hosts and copy Binding/Security if needed
					if (DSI.Owner != TSI)
					{
						DSI.ChangeOwners(TSI);
					}

					if (DSI.Parent.IsProjectRoot == true) DSI.Owner.Namespaces.Remove(DSI);
					else DSI.Parent.Children.Remove(DSI);
					TSI.Namespaces.Add(DSI);

					DSI.Owner = TSI;
					DSI.Parent = TSI.Namespace;
				}
				if (TSIType == typeof(Projects.DependencyProject))
				{
					Projects.Namespace DSI = DragSolutionItem as Projects.Namespace;
					Projects.DependencyProject TSI = DropSolutionItem as Projects.DependencyProject;
					if (DSI == null) return;
					if (TSI == null) return;

					//Move Hosts and copy Binding/Security if needed
					if (DSI.Owner != TSI)
					{
						DSI.ChangeOwners(TSI);
					}

					if (DSI.Parent.IsProjectRoot == true) DSI.Owner.Namespaces.Remove(DSI);
					else DSI.Parent.Children.Remove(DSI);
					TSI.Namespaces.Add(DSI);

					DSI.Owner = TSI;
					DSI.Parent = TSI.Namespace;
				}
				else if (TSIType == typeof(Projects.Namespace))
				{
					Projects.Namespace DSI = DragSolutionItem as Projects.Namespace;
					Projects.Namespace TSI = DropSolutionItem as Projects.Namespace;
					if (DSI == null) return;
					if (TSI == null) return;

					//Move Hosts and copy Binding/Security if needed
					if (DSI.Owner != TSI.Owner)
					{
						DSI.ChangeOwners(TSI.Owner);
					}

					//Change ownerships.
					if (TSI.Parent == null || DSI.Parent == null)
					{
						//If the parent is the same we only want to move the item in its list, otherwise we want to re-parent it.
						if (DSI.Owner == TSI.Owner)
						{
							int DSII = DSI.Owner.Namespaces.IndexOf(DSI);
							int TSII = TSI.Owner.Namespaces.IndexOf(TSI);
							TSI.Owner.Namespaces.Move(DSII, TSII);
						}
						else
						{
							DSI.Owner = TSI.Owner;
							DSI.Parent = null;
							DSI.Owner.Namespaces.Remove(DSI);
							int TSII = TSI.Owner.Namespaces.IndexOf(TSI);
							TSI.Owner.Namespaces.Insert(TSII, DSI);

							DSI.Parent = TSI.Owner.Namespace;
						}
					}
					else
					{
						//If the parent is the same we only want to move the item in its list, otherwise we want to re-parent it.
						if (DSI.Parent == TSI.Parent)
						{
							int DSII = DSI.Parent.Children.IndexOf(DSI);
							int TSII = TSI.Parent.Children.IndexOf(TSI);
							TSI.Parent.Children.Move(DSII, TSII);
						}
						else
						{
							DSI.Owner = TSI.Owner;
							DSI.Parent = TSI.Parent;
							DSI.Parent.Children.Remove(DSI);
							int TSII = TSI.Parent.Children.IndexOf(TSI);
							TSI.Parent.Children.Insert(TSII, DSI);

							DSI.Owner = TSI.Owner;
							DSI.Parent = TSI.Parent;
						}
					}
				}
			}

			if (DSIType == typeof(Projects.Service))
			{
				if (TSIType == typeof(Projects.Project) || TSIType == typeof(Projects.DependencyProject))
				{
					Projects.Service DSI = DragSolutionItem as Projects.Service;
					Projects.Project TSI = DropSolutionItem as Projects.Project;
					if (DSI == null) return;
					if (TSI == null) return;

					if (DSI.Parent.IsProjectRoot == true) DSI.Parent.Owner.Services.Remove(DSI);
					else DSI.Parent.Services.Remove(DSI);
					TSI.Services.Add(DSI);
					DSI.Parent = TSI.Namespace;
				}
				else if (TSIType == typeof(Projects.Namespace))
				{
					Projects.Service DSI = DragSolutionItem as Projects.Service;
					Projects.Namespace TSI = DropSolutionItem as Projects.Namespace;
					if (DSI == null) return;
					if (TSI == null) return;

					if (DSI.Parent.IsProjectRoot == true) DSI.Parent.Owner.Services.Remove(DSI);
					else DSI.Parent.Services.Remove(DSI);
					TSI.Services.Add(DSI);

					DSI.Parent = TSI;
				}
				else if (TSIType == typeof(Projects.Service))
				{
					Projects.Service DSI = DragSolutionItem as Projects.Service;
					Projects.Service TSI = DropSolutionItem as Projects.Service;
					if (DSI == null) return;
					if (TSI == null) return;

					if (TSI.Parent == DSI.Parent)
					{
						if (TSI.Parent.IsProjectRoot == true)
						{
							int DSII = DSI.Parent.Owner.Services.IndexOf(DSI);
							int TSII = TSI.Parent.Owner.Services.IndexOf(TSI);
							TSI.Parent.Owner.Services.Move(DSII, TSII);
						}
						else
						{
							int DSII = DSI.Parent.Services.IndexOf(DSI);
							int TSII = TSI.Parent.Services.IndexOf(TSI);
							TSI.Parent.Services.Move(DSII, TSII);
						}
					}
					else
					{
						int TSII = 0;
						if (TSI.Parent.IsProjectRoot == true) TSII = TSI.Parent.Owner.Services.IndexOf(TSI);
						else TSII = TSI.Parent.Services.IndexOf(TSI);

						if (DSI.Parent.IsProjectRoot == true) DSI.Parent.Owner.Services.Remove(DSI);
						else DSI.Parent.Services.Remove(DSI);
						if (TSI.Parent.IsProjectRoot == true) TSI.Parent.Owner.Services.Insert(TSII, DSI);
						else TSI.Parent.Services.Insert(TSII, DSI);
						
						DSI.Parent = TSI.Parent;
					}
				}
				else if (TSIType == typeof(Projects.Data))
				{
					Projects.Service DSI = DragSolutionItem as Projects.Service;
					Projects.Data TSI = DropSolutionItem as Projects.Data;
					if (DSI == null) return;
					if (TSI == null) return;

					if (TSI.Parent == DSI.Parent)
					{
						if (TSI.Parent.IsProjectRoot == true)
						{
							int DSII = DSI.Parent.Owner.Services.IndexOf(DSI);
							TSI.Parent.Owner.Services.Move(DSII, DSI.Parent.Owner.Services.Count - 1);
						}
						else
						{
							int DSII = DSI.Parent.Services.IndexOf(DSI);
							TSI.Parent.Services.Move(DSII, DSI.Parent.Services.Count - 1);
						}
					}
					else
					{
						if (DSI.Parent.IsProjectRoot == true) DSI.Parent.Owner.Services.Remove(DSI);
						else DSI.Parent.Services.Remove(DSI);
						if (TSI.Parent.IsProjectRoot == true) TSI.Parent.Owner.Services.Add(DSI);
						else TSI.Parent.Services.Add(DSI);

						DSI.Parent = TSI.Parent;
					}
				}
				else if (TSIType == typeof(Projects.Enum))
				{
					Projects.Service DSI = DragSolutionItem as Projects.Service;
					Projects.Enum TSI = DropSolutionItem as Projects.Enum;
					if (DSI == null) return;
					if (TSI == null) return;

					if (TSI.Parent == DSI.Parent)
					{
						if (TSI.Parent.IsProjectRoot == true)
						{
							int DSII = DSI.Parent.Owner.Services.IndexOf(DSI);
							TSI.Parent.Owner.Services.Move(DSII, DSI.Parent.Owner.Services.Count - 1);
						}
						else
						{
							int DSII = DSI.Parent.Services.IndexOf(DSI);
							TSI.Parent.Services.Move(DSII, DSI.Parent.Services.Count - 1);
						}
					}
					else
					{
						if (DSI.Parent.IsProjectRoot == true) DSI.Parent.Owner.Services.Remove(DSI);
						else DSI.Parent.Services.Remove(DSI);
						if (TSI.Parent.IsProjectRoot == true) TSI.Parent.Owner.Services.Add(DSI);
						else TSI.Parent.Services.Add(DSI);

						DSI.Parent = TSI.Parent;
					}
				}
			}

			if (DSIType == typeof(Projects.Data))
			{
				if (TSIType == typeof(Projects.Project) || TSIType == typeof(Projects.DependencyProject))
				{
					Projects.Data DSI = DragSolutionItem as Projects.Data;
					Projects.Project TSI = DropSolutionItem as Projects.Project;
					if (DSI == null) return;
					if (TSI == null) return;

					if (DSI.Parent.IsProjectRoot == true) DSI.Parent.Owner.Data.Remove(DSI);
					else DSI.Parent.Data.Remove(DSI);
					TSI.Data.Add(DSI);
					DSI.Parent = TSI.Namespace;
				}
				if (TSIType == typeof(Projects.Namespace))
				{
					Projects.Data DSI = DragSolutionItem as Projects.Data;
					Projects.Namespace TSI = DropSolutionItem as Projects.Namespace;
					if (DSI == null) return;
					if (TSI == null) return;

					if (DSI.Parent.IsProjectRoot == true) DSI.Parent.Owner.Data.Remove(DSI);
					else DSI.Parent.Data.Remove(DSI);
					TSI.Data.Add(DSI);

					DSI.Parent = TSI;
				}
				else if (TSIType == typeof(Projects.Service))
				{
					Projects.Data DSI = DragSolutionItem as Projects.Data;
					Projects.Service TSI = DropSolutionItem as Projects.Service;
					if (DSI == null) return;
					if (TSI == null) return;

					if (TSI.Parent == DSI.Parent)
					{
						if (TSI.Parent.IsProjectRoot == true)
						{
							int DSII = DSI.Parent.Owner.Data.IndexOf(DSI);
							TSI.Parent.Owner.Data.Move(DSII, 0);
						}
						else
						{
							int DSII = DSI.Parent.Data.IndexOf(DSI);
							TSI.Parent.Data.Move(DSII, 0);
						}
					}
					else
					{
						if (DSI.Parent.IsProjectRoot == true) DSI.Parent.Owner.Data.Remove(DSI);
						else DSI.Parent.Data.Remove(DSI);
						if (TSI.Parent.IsProjectRoot == true) TSI.Parent.Owner.Data.Insert(0, DSI);
						else TSI.Parent.Data.Insert(0, DSI);

						DSI.Parent = TSI.Parent;
					}
				}
				else if (TSIType == typeof(Projects.Data))
				{
					Projects.Data DSI = DragSolutionItem as Projects.Data;
					Projects.Data TSI = DropSolutionItem as Projects.Data;
					if (DSI == null) return;
					if (TSI == null) return;

					if (TSI.Parent == DSI.Parent)
					{
						if (TSI.Parent.IsProjectRoot == true)
						{
							int DSII = DSI.Parent.Owner.Data.IndexOf(DSI);
							int TSII = TSI.Parent.Owner.Data.IndexOf(TSI);
							TSI.Parent.Owner.Data.Move(DSII, TSII);
						}
						else
						{
							int DSII = DSI.Parent.Data.IndexOf(DSI);
							int TSII = TSI.Parent.Data.IndexOf(TSI);
							TSI.Parent.Data.Move(DSII, TSII);
						}
					}
					else
					{
						int TSII = 0;
						if (TSI.Parent.IsProjectRoot == true) TSII = TSI.Parent.Owner.Data.IndexOf(TSI);
						else TSII = TSI.Parent.Data.IndexOf(TSI);

						if (DSI.Parent.IsProjectRoot == true) DSI.Parent.Owner.Data.Remove(DSI);
						else DSI.Parent.Data.Remove(DSI);
						if (TSI.Parent.IsProjectRoot == true) TSI.Parent.Owner.Data.Insert(TSII, DSI);
						else TSI.Parent.Data.Insert(TSII, DSI);

						DSI.Parent = TSI.Parent;
					}
				}
				else if (TSIType == typeof(Projects.Enum))
				{
					Projects.Data DSI = DragSolutionItem as Projects.Data;
					Projects.Enum TSI = DropSolutionItem as Projects.Enum;
					if (DSI == null) return;
					if (TSI == null) return;

					if (TSI.Parent == DSI.Parent)
					{
						if (TSI.Parent.IsProjectRoot == true)
						{
							int DSII = DSI.Parent.Owner.Data.IndexOf(DSI);
							TSI.Parent.Owner.Data.Move(DSII, DSI.Parent.Owner.Data.Count - 1);
						}
						else
						{
							int DSII = DSI.Parent.Data.IndexOf(DSI);
							TSI.Parent.Data.Move(DSII, DSI.Parent.Data.Count - 1);
						}
					}
					else
					{
						if (DSI.Parent.IsProjectRoot == true) DSI.Parent.Owner.Data.Remove(DSI);
						else DSI.Parent.Data.Remove(DSI);
						if (TSI.Parent.IsProjectRoot == true) TSI.Parent.Owner.Data.Add(DSI);
						else TSI.Parent.Data.Add(DSI);

						DSI.Parent = TSI.Parent;
					}
				}
			}

			if (DSIType == typeof(Projects.Enum))
			{
				if (TSIType == typeof(Projects.Project) || TSIType == typeof(Projects.DependencyProject))
				{
					Projects.Enum DSI = DragSolutionItem as Projects.Enum;
					Projects.Project TSI = DropSolutionItem as Projects.Project;
					if (DSI == null) return;
					if (TSI == null) return;

					if (DSI.Parent.IsProjectRoot == true) DSI.Parent.Owner.Enums.Remove(DSI);
					else DSI.Parent.Enums.Remove(DSI);
					TSI.Enums.Add(DSI);
					DSI.Parent = TSI.Namespace;
				}
				if (TSIType == typeof(Projects.Namespace))
				{
					Projects.Enum DSI = DragSolutionItem as Projects.Enum;
					Projects.Namespace TSI = DropSolutionItem as Projects.Namespace;
					if (DSI == null) return;
					if (TSI == null) return;

					if (DSI.Parent.IsProjectRoot == true) DSI.Parent.Owner.Enums.Remove(DSI);
					else DSI.Parent.Enums.Remove(DSI);
					TSI.Enums.Add(DSI);

					DSI.Parent = TSI;
				}
				else if (TSIType == typeof(Projects.Service))
				{
					Projects.Enum DSI = DragSolutionItem as Projects.Enum;
					Projects.Service TSI = DropSolutionItem as Projects.Service;
					if (DSI == null) return;
					if (TSI == null) return;

					if (TSI.Parent == DSI.Parent)
					{
						if (TSI.Parent.IsProjectRoot == true)
						{
							int DSII = DSI.Parent.Owner.Enums.IndexOf(DSI);
							TSI.Parent.Owner.Enums.Move(DSII, 0);
						}
						else
						{
							int DSII = DSI.Parent.Enums.IndexOf(DSI);
							TSI.Parent.Enums.Move(DSII, 0);
						}
					}
					else
					{
						if (DSI.Parent.IsProjectRoot == true) DSI.Parent.Owner.Enums.Remove(DSI);
						else DSI.Parent.Enums.Remove(DSI);
						if (TSI.Parent.IsProjectRoot == true) TSI.Parent.Owner.Enums.Insert(0, DSI);
						else TSI.Parent.Enums.Insert(0, DSI);

						DSI.Parent = TSI.Parent;
					}
				}
				else if (TSIType == typeof(Projects.Data))
				{
					Projects.Enum DSI = DragSolutionItem as Projects.Enum;
					Projects.Data TSI = DropSolutionItem as Projects.Data;
					if (DSI == null) return;
					if (TSI == null) return;

					if (TSI.Parent == DSI.Parent)
					{
						if (TSI.Parent.IsProjectRoot == true)
						{
							int DSII = DSI.Parent.Owner.Enums.IndexOf(DSI);
							TSI.Parent.Owner.Enums.Move(DSII, 0);
						}
						else
						{
							int DSII = DSI.Parent.Enums.IndexOf(DSI);
							TSI.Parent.Enums.Move(DSII, 0);
						}
					}
					else
					{
						if (DSI.Parent.IsProjectRoot == true) DSI.Parent.Owner.Enums.Remove(DSI);
						else DSI.Parent.Enums.Remove(DSI);
						if (TSI.Parent.IsProjectRoot == true) TSI.Parent.Owner.Enums.Insert(0, DSI);
						else TSI.Parent.Enums.Insert(0, DSI);

						DSI.Parent = TSI.Parent;
					}
				}
				else if (TSIType == typeof(Projects.Enum))
				{
					Projects.Enum DSI = DragSolutionItem as Projects.Enum;
					Projects.Enum TSI = DropSolutionItem as Projects.Enum;
					if (DSI == null) return;
					if (TSI == null) return;

					if (TSI.Parent == DSI.Parent)
					{
						if (TSI.Parent.IsProjectRoot == true)
						{
							int DSII = DSI.Parent.Owner.Enums.IndexOf(DSI);
							int TSII = TSI.Parent.Owner.Enums.IndexOf(TSI);
							TSI.Parent.Owner.Enums.Move(DSII, TSII);
						}
						else
						{
							int DSII = DSI.Parent.Enums.IndexOf(DSI);
							int TSII = TSI.Parent.Enums.IndexOf(TSI);
							TSI.Parent.Enums.Move(DSII, TSII);
						}
					}
					else
					{
						int TSII = 0;
						if (TSI.Parent.IsProjectRoot == true) TSII = TSI.Parent.Owner.Enums.IndexOf(TSI);
						else TSII = TSI.Parent.Enums.IndexOf(TSI);

						if (DSI.Parent.IsProjectRoot == true) DSI.Parent.Owner.Enums.Remove(DSI);
						else DSI.Parent.Enums.Remove(DSI);
						if (TSI.Parent.IsProjectRoot == true) TSI.Parent.Owner.Enums.Insert(TSII, DSI);
						else TSI.Parent.Enums.Insert(TSII, DSI);

						DSI.Parent = TSI.Parent;
					}
				}
			}
		}

		#endregion
		 
	}

	public class NewItemType : DependencyObject
	{
		public string ImageSource { get { return (string)GetValue(ImageSourceProperty); } set { SetValue(ImageSourceProperty, value); } }
		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(NewItemType));

		public string Title { get { return (string)GetValue(TitleProperty); } set { SetValue(TitleProperty, value); } }
		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(NewItemType));

		public string Description { get { return (string)GetValue(DescriptionProperty); } set { SetValue(DescriptionProperty, value); } }
		public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(NewItemType));

		public string Frameworks { get { return (string)GetValue(FrameworksProperty); } set { SetValue(FrameworksProperty, value); } }
		public static readonly DependencyProperty FrameworksProperty = DependencyProperty.Register("Frameworks", typeof(string), typeof(NewItemType));

		public int DataType { get { return (int)GetValue(DataTypeProperty); } set { SetValue(DataTypeProperty, value); } }
		public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType", typeof(int), typeof(NewItemType));
	}
}