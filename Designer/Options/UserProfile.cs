using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace WCFArchitect.Options
{
	public enum UserProfileReferencePathFramework
	{
		Any,
		NET30,
		NET35,
		NET35Client,
		NET40,
		NET40Client,
		SL30,
		SL40,
		SL50,
	}

	public class UserProfile : DependencyObject
	{
		private Guid id;
		public Guid ID { get { return id; } }
		public string User { get; set; }
		public string ComputerName { get; set; }
		public List<RecentSolution> RecentProjects { get; set; }
		public List<RecentSolution> ImportantProjects { get; set; }

		public string DefaultProjectFolder { get { return (string)GetValue(DefaultProjectFolderProperty); } set { SetValue(DefaultProjectFolderProperty, value); } }
		public static readonly DependencyProperty DefaultProjectFolderProperty = DependencyProperty.Register("DefaultProjectFolder", typeof(string), typeof(UserProfile));

		//SDK Configuration
		public ObservableCollection<UserProfileReferencePath> ReferencePaths { get { return (ObservableCollection<UserProfileReferencePath>)GetValue(ReferencePathsProperty); } set { SetValue(ReferencePathsProperty, value); } }
		public static readonly DependencyProperty ReferencePathsProperty = DependencyProperty.Register("ReferencePaths", typeof(ObservableCollection<UserProfileReferencePath>), typeof(UserProfile));

		public string SvcUtilPath { get { return (string)GetValue(SvcUtilPathProperty); } set { SetValue(SvcUtilPathProperty, value); } }
		public static readonly DependencyProperty SvcUtilPathProperty = DependencyProperty.Register("SvcUtilPath", typeof(string), typeof(UserProfile));

		//Automatic Backup Configuration
		public bool AutomaticBackupsEnabled { get { return (bool)GetValue(AutomaticBackupsEnabledProperty); } set { SetValue(AutomaticBackupsEnabledProperty, value); } }
		public static readonly DependencyProperty AutomaticBackupsEnabledProperty = DependencyProperty.Register("AutomaticBackupsEnabled", typeof(bool), typeof(UserProfile));

		public TimeSpan AutomaticBackupsInterval { get { return (TimeSpan)GetValue(AutomaticBackupsIntervalProperty); } set { SetValue(AutomaticBackupsIntervalProperty, value); } }
		public static readonly DependencyProperty AutomaticBackupsIntervalProperty = DependencyProperty.Register("AutomaticBackupsInterval", typeof(TimeSpan), typeof(UserProfile));

		//Project Tab Configuration
		public C1.WPF.Dock TabPosition { get { return (C1.WPF.Dock)GetValue(TabPositionProperty); } set { SetValue(TabPositionProperty, value); } }
		public static readonly DependencyProperty TabPositionProperty = DependencyProperty.Register("TabPosition", typeof(C1.WPF.Dock), typeof(UserProfile), new UIPropertyMetadata(C1.WPF.Dock.Top));

		public int TabMaximumWidth { get { return (int)GetValue(TabMaximumWidthProperty); } set { SetValue(TabMaximumWidthProperty, value); } }
		public static readonly DependencyProperty TabMaximumWidthProperty = DependencyProperty.Register("TabMaximumWidth", typeof(int), typeof(UserProfile), new UIPropertyMetadata(220));

		//Docking Configuration
		public bool IsDockConfigurationAvailable { get; set; }
		public C1.WPF.Dock DockSolutionNavigatorPosition { get; set; }
		public C1.WPF.Docking.DockMode DockSolutionNavigatorMode { get; set; }
		public double DockSolutionNavigatorWidth { get; set; }
		public double DockSolutionNavigatorHeight { get; set; }
		public C1.WPF.Dock DockErrorListPosition { get; set; }
		public C1.WPF.Docking.DockMode DockErrorListMode { get; set; }
		public double DockErrorListWidth { get; set; }
		public double DockErrorListHeight { get; set; }
		public C1.WPF.Dock DockOutputPosition { get; set; }
		public C1.WPF.Docking.DockMode DockOutputMode { get; set; }
		public double DockOutputWidth { get; set; }
		public double DockOutputHeight { get; set; }
		public C1.WPF.Dock DockFindPosition { get; set; }
		public C1.WPF.Docking.DockMode DockFindMode { get; set; }
		public double DockFindWidth { get; set; }
		public double DockFindHeight { get; set; }

		//Main Window Configuration
		public int Monitor { get; set; }
		public bool IsWindowMaximized { get; set; }
		public int WindowX { get; set; }
		public int WindowY { get; set; }
		public int WindowHeight { get; set; }
		public int WindowWidth { get; set; }

		public UserProfile()
		{
			this.AutomaticBackupsEnabled = true;
			this.AutomaticBackupsInterval = new TimeSpan(0, 5, 0);
		}

		public UserProfile(string User)
		{
			this.id = Guid.NewGuid();
			this.User = User;
			this.ComputerName = Environment.MachineName;
			this.RecentProjects = new List<RecentSolution>();
			this.ImportantProjects = new List<RecentSolution>();
			this.ReferencePaths = new ObservableCollection<UserProfileReferencePath>();
			this.DefaultProjectFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			this.Monitor = 0;
			this.AutomaticBackupsEnabled = true;
			this.AutomaticBackupsInterval = new TimeSpan(0, 5, 0);

			//Setup the Dock
			this.IsDockConfigurationAvailable = true;
			this.DockSolutionNavigatorHeight = 200;
			this.DockSolutionNavigatorWidth = 200;
			this.DockSolutionNavigatorMode = C1.WPF.Docking.DockMode.Docked;
			this.DockSolutionNavigatorPosition = C1.WPF.Dock.Left;
			this.DockErrorListHeight = 200;
			this.DockErrorListWidth = 200;
			this.DockErrorListMode = C1.WPF.Docking.DockMode.Sliding;
			this.DockErrorListPosition = C1.WPF.Dock.Bottom;
			this.DockOutputHeight = 200;
			this.DockOutputWidth = 200;
			this.DockOutputMode = C1.WPF.Docking.DockMode.Sliding;
			this.DockOutputPosition = C1.WPF.Dock.Bottom;
		}
	}

	public class UserProfileReferencePath : DependencyObject
	{
		public string Path { get { return (string)GetValue(PathProperty); } set { SetValue(PathProperty, value); UpdatePathIsValid(); } }
		public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(UserProfileReferencePath));

		public UserProfileReferencePathFramework Framework { get { return (UserProfileReferencePathFramework)GetValue(FrameworkProperty); } set { SetValue(FrameworkProperty, value); } }
		public static readonly DependencyProperty FrameworkProperty = DependencyProperty.Register("Framework", typeof(UserProfileReferencePathFramework), typeof(UserProfileReferencePathFramework));

		public bool IsValid { get { return (bool)GetValue(IsValidProperty); } set { SetValue(IsValidProperty, value); } }
		public static readonly DependencyProperty IsValidProperty = DependencyProperty.Register("IsValid", typeof(bool), typeof(UserProfileReferencePath));

		public UserProfileReferencePath() { }

		public UserProfileReferencePath(string Path)
		{
			this.Path = Path;
			this.Framework = UserProfileReferencePathFramework.Any;
			UpdatePathIsValid();
		}

		private void UpdatePathIsValid()
		{
			IsValid = System.IO.Directory.Exists(Path);
		}
	}

	public class RecentSolution : DependencyObject
	{
		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(RecentSolution));

		public string Path { get { return (string)GetValue(PathProperty); } set { SetValue(PathProperty, value); } }
		public static DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(RecentSolution));

		public DateTime LastAccessed { get { return (DateTime)GetValue(LastAccessedProperty); } set { SetValue(LastAccessedProperty, value); } }
		public static DependencyProperty LastAccessedProperty = DependencyProperty.Register("LastAccessed", typeof(DateTime), typeof(RecentSolution));

		public RecentSolution() { }

		public RecentSolution(string Name, string Path)
		{
			this.Name = Name;
			this.Path = Path;
			this.LastAccessed = DateTime.Now;
		}
	}
}