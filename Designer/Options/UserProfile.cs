﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows;

namespace NETPath.Options
{
	[System.Reflection.Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
	public class UserProfile : DependencyObject
	{
		private Guid id;
		public Guid ID { get { return id; } }
		public string User { get; set; }
		public string ComputerName { get; set; }
		public Prospective.Server.Licensing.UsageData PriorUsage { get; set; }
		[IgnoreDataMember] public string SKU { get; set; }
		public List<RecentSolution> RecentProjects { get; set; }
		public List<RecentSolution> ImportantProjects { get; set; }

		public string DefaultProjectFolder { get { return (string)GetValue(DefaultProjectFolderProperty); } set { SetValue(DefaultProjectFolderProperty, value); } }
		public static readonly DependencyProperty DefaultProjectFolderProperty = DependencyProperty.Register("DefaultProjectFolder", typeof(string), typeof(UserProfile));

		//Automatic Backup Configuration
		public bool AutomaticBackupsEnabled { get { return (bool)GetValue(AutomaticBackupsEnabledProperty); } set { SetValue(AutomaticBackupsEnabledProperty, value); } }
		public static readonly DependencyProperty AutomaticBackupsEnabledProperty = DependencyProperty.Register("AutomaticBackupsEnabled", typeof(bool), typeof(UserProfile));

		public TimeSpan AutomaticBackupsInterval { get { return (TimeSpan)GetValue(AutomaticBackupsIntervalProperty); } set { SetValue(AutomaticBackupsIntervalProperty, value); } }
		public static readonly DependencyProperty AutomaticBackupsIntervalProperty = DependencyProperty.Register("AutomaticBackupsInterval", typeof(TimeSpan), typeof(UserProfile));

		//License Information
		public bool IsUsageEnabled { get { return (bool)GetValue(IsUsageEnabledProperty); } set { SetValue(IsUsageEnabledProperty, value); } }
		public static readonly DependencyProperty IsUsageEnabledProperty = DependencyProperty.Register("IsUsageEnabled", typeof(bool), typeof(UserProfile), new PropertyMetadata(false));

		public bool IsTrial { get { return (bool)GetValue(IsTrialProperty); } set { SetValue(IsTrialProperty, value); } }
		public static readonly DependencyProperty IsTrialProperty = DependencyProperty.Register("IsTrial", typeof(bool), typeof(UserProfile), new PropertyMetadata(false));
		public bool IsTrialInfoSet { get; set; }

		public string LicenseeName { get { return (string)GetValue(DeclarationProperty); } internal set { SetValue(DeclarationProperty, value); } }
		private static readonly DependencyProperty DeclarationProperty = DependencyProperty.Register("Declaration", typeof(string), typeof(UserProfile), new PropertyMetadata(""));

		public string Serial { get { return (string)GetValue(SerialProperty); } set { SetValue(SerialProperty, value); } }
		public static readonly DependencyProperty SerialProperty = DependencyProperty.Register("Serial", typeof(string), typeof(UserProfile), new PropertyMetadata(""));

		public string License { get { return (string)GetValue(LicenseProperty); } set { SetValue(LicenseProperty, value); } }
		public static readonly DependencyProperty LicenseProperty = DependencyProperty.Register("License", typeof(string), typeof(UserProfile), new PropertyMetadata(""));

		public string UserName { get { return (string)GetValue(UserNameProperty); } set { SetValue(UserNameProperty, value); } }
		public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register("UserName", typeof(string), typeof(UserProfile), new PropertyMetadata(""));

		public string UserEmail { get { return (string)GetValue(UserEmailProperty); } set { SetValue(UserEmailProperty, value); } }
		public static readonly DependencyProperty UserEmailProperty = DependencyProperty.Register("UserEmail", typeof(string), typeof(UserProfile), new PropertyMetadata(""));

		public UserProfile()
		{
			AutomaticBackupsEnabled = true;
			AutomaticBackupsInterval = new TimeSpan(0, 5, 0);
		}

		public UserProfile(string User)
		{
			id = Guid.NewGuid();
			this.User = User;
			ComputerName = Environment.MachineName;
			RecentProjects = new List<RecentSolution>();
			ImportantProjects = new List<RecentSolution>();
			DefaultProjectFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			AutomaticBackupsEnabled = true;
			AutomaticBackupsInterval = new TimeSpan(0, 5, 0);
			IsTrial = true;
			IsTrialInfoSet = false;
			IsUsageEnabled = true;
			Serial = "TRIAL";
		}

		public static UserProfile Open(string Path)
		{
			//Check the file to make sure it exists
			if (!File.Exists(Path))
				throw new FileNotFoundException("Unable to locate file '" + Path + "'");

			var kt = new List<Type>(new Type[] { 
				typeof(UserProfile), typeof(RecentSolution)
			});

			var dcs = new DataContractSerializer(typeof(UserProfile), new DataContractSerializerSettings { KnownTypes = kt, MaxItemsInObjectGraph = Int32.MaxValue, IgnoreExtensionDataObject = true, SerializeReadOnlyTypes = true, PreserveObjectReferences = true });
			var fs = new FileStream(Path, FileMode.Open, FileAccess.Read);
			var tr = XmlReader.Create(fs, new XmlReaderSettings { CloseInput = true, Async = false });

			var t = (UserProfile)dcs.ReadObject(tr);
			tr.Close();
			return t;
		}

		public static void Save(string Path, UserProfile Data)
		{
			//Make sure the solution isn't read-only.
			if (File.Exists(Path))
			{
				var fi = new FileInfo(Path);
				if (fi.IsReadOnly)
					throw new IOException("The file '" + Path + "' is currently read-only. Please disable read-only mode on this file.");
			}

			var kt = new List<Type>(new Type[] { 
				typeof(UserProfile), typeof(RecentSolution)
			});

			var dcs = new DataContractSerializer(typeof(UserProfile), new DataContractSerializerSettings { KnownTypes = kt, MaxItemsInObjectGraph = Int32.MaxValue, IgnoreExtensionDataObject = true, SerializeReadOnlyTypes = true, PreserveObjectReferences = true });
			var fs = new FileStream(Path, FileMode.Create);
			var xw = XmlWriter.Create(fs, new XmlWriterSettings { Encoding = Encoding.UTF8, NewLineChars = Environment.NewLine, NewLineHandling = NewLineHandling.Entitize, CloseOutput = true, WriteEndDocumentOnClose = true, Indent = true, Async = false });
			dcs.WriteObject(xw, Data);
			xw.Flush();
			xw.Close();
		}
	}

	[System.Reflection.Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
	public class RecentSolution : DependencyObject
	{
		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(RecentSolution));

		public string Path { get { return (string)GetValue(PathProperty); } set { SetValue(PathProperty, value); } }
		public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(RecentSolution));

		public DateTime LastAccessed { get { return (DateTime)GetValue(LastAccessedProperty); } set { SetValue(LastAccessedProperty, value); } }
		public static readonly DependencyProperty LastAccessedProperty = DependencyProperty.Register("LastAccessed", typeof(DateTime), typeof(RecentSolution));

		public RecentSolution() { }

		public RecentSolution(string Name, string Path)
		{
			this.Name = Name;
			this.Path = Path;
			LastAccessed = DateTime.Now;
		}
	}
}