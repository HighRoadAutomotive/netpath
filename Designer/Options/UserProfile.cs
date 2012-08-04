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

		//Automatic Backup Configuration
		public bool AutomaticBackupsEnabled { get { return (bool)GetValue(AutomaticBackupsEnabledProperty); } set { SetValue(AutomaticBackupsEnabledProperty, value); } }
		public static readonly DependencyProperty AutomaticBackupsEnabledProperty = DependencyProperty.Register("AutomaticBackupsEnabled", typeof(bool), typeof(UserProfile));

		public TimeSpan AutomaticBackupsInterval { get { return (TimeSpan)GetValue(AutomaticBackupsIntervalProperty); } set { SetValue(AutomaticBackupsIntervalProperty, value); } }
		public static readonly DependencyProperty AutomaticBackupsIntervalProperty = DependencyProperty.Register("AutomaticBackupsInterval", typeof(TimeSpan), typeof(UserProfile));

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
			this.DefaultProjectFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			this.Monitor = 0;
			this.AutomaticBackupsEnabled = true;
			this.AutomaticBackupsInterval = new TimeSpan(0, 5, 0);
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