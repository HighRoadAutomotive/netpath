using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WCFArchitect.Projects
{
	internal class Solution : DependencyObject
	{
		private Guid id = Guid.Empty;
		public Guid ID { get { return id; } }

		public string TrialID { get; set; }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Solution));

		public Solution() { }

#if TRIAL
		public Solution(string Name, string TrialID)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.TrialID = TrialID;
		}
#else
		public Solution(string Name)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.TrialID = "";
		}
#endif
	}

	public class UserOpened
	{
		private Guid userProfileID;
		public Guid UserProfileID { get { return userProfileID; } }
		public int OpenIndex { get; set; }

		public UserOpened()
		{
			userProfileID = Globals.UserProfile.ID;
			OpenIndex = -1;
		}
	}

	internal class Reference
	{
		private Guid id = Guid.Empty;
		public Guid ID { get { return id; } }
		public string Name { get; set; }
		public string Path { get; set; }
		private Prospective.Utilities.Types.Version frameworkVersion;
		public string FrameworkVersion { get { return (string)Application.Current.Dispatcher.Invoke(new Func<string>(() => { return frameworkVersion.Major.ToString() + "." + frameworkVersion.Minor.ToString() + "." + frameworkVersion.Build.ToString(); }), System.Windows.Threading.DispatcherPriority.Send); } }
		public string FrameworkVersionNoBuild { get { return (string)Application.Current.Dispatcher.Invoke(new Func<string>(() => { return frameworkVersion.Major.ToString() + "." + frameworkVersion.Minor.ToString() + ".0.0"; }), System.Windows.Threading.DispatcherPriority.Send); } }
		public string Version { get; set; }
		public string PublicKeyToken { get; set; }
		public bool HasAssembly { get; set; }
		public bool IsClientProfile { get; set; }
		public bool IsGACInstalled { get; set; }

		public Reference()
		{
			this.IsGACInstalled = true;
		}

		public Reference(string Name, string Path, Version FrameworkVersion, string AssemblyVersion, string PublicKeyToken, bool IsClientProfile, bool IsGACInstalled)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.Path = Path;
			this.frameworkVersion = new Prospective.Utilities.Types.Version(FrameworkVersion);
			this.Version = AssemblyVersion;
			this.PublicKeyToken = PublicKeyToken;
			this.IsClientProfile = IsClientProfile;
			this.IsGACInstalled = IsGACInstalled;
		}

		public bool IsFrameworkVersion(Version FrameworkVersion)
		{
			if (this.frameworkVersion.Major != FrameworkVersion.Major) return false;
			if (this.frameworkVersion.Minor != FrameworkVersion.Minor) return false;
			if (this.frameworkVersion.Build != FrameworkVersion.Build) return false;
			if (this.frameworkVersion.Revision != FrameworkVersion.Revision) return false;
			return true;
		}

		public void SetFrameworkVersion(Version NewFrameworkVersion)
		{
			frameworkVersion = new Prospective.Utilities.Types.Version(NewFrameworkVersion);
		}

		public void UpdateVersion()
		{
			try
			{
				this.Version = System.Diagnostics.FileVersionInfo.GetVersionInfo(Path).FileVersion;
			}
			catch { }
		}
	}

	internal enum ProjectServiceSerializerType
	{
		Auto,
		DataContract,
		XML,
	}

	internal enum ProjectCollectionType
	{
		List,
		SynchronizedCollection,
		Collection,
		ObservableCollection,
		BindingList,
		Array,
	}

	internal class ProjectUsingNamespace : DependencyObject
	{
		private Guid id = Guid.Empty;
		public Guid ID { get { return id; } }
		
		public string Namespace { get { return (string)GetValue(NamespaceProperty); } set { SetValue(NamespaceProperty, value); } }
		public static readonly DependencyProperty NamespaceProperty = DependencyProperty.Register("Namespace", typeof(string), typeof(ProjectUsingNamespace));

		public bool IsFullFrameworkOnly { get { return (bool)GetValue(IsFullFrameworkOnlyProperty); } set { SetValue(IsFullFrameworkOnlyProperty, value); } }
		public static readonly DependencyProperty IsFullFrameworkOnlyProperty = DependencyProperty.Register("IsFullFrameworkOnly", typeof(bool), typeof(ProjectUsingNamespace));

		public ProjectUsingNamespace()
		{
		}

		public ProjectUsingNamespace(string Namespace)
		{
			this.id = Guid.NewGuid();
			this.Namespace = Namespace;
			this.IsFullFrameworkOnly = false;
		}
	}

	public class OpenableDocument : DependencyObject
	{
		//Open document handling
		protected List<UserOpened> UserOpenedList { get; set; }
		bool ia = false;
		public bool IsActive { get { return ia; }  set { if(Globals.IsClosing == false) ia = value; } }
		public bool IsOpen { get { UserOpened t = UserOpenedList.FirstOrDefault(a => a.UserProfileID == Globals.UserProfile.ID); if (t != null) { return true; } else { return false; } } set { if (value == true) { UserOpened t = UserOpenedList.FirstOrDefault(a => a.UserProfileID == Globals.UserProfile.ID); if (t == null) { UserOpenedList.Add(new UserOpened()); } } else { UserOpenedList.RemoveAll(a => a.UserProfileID == Globals.UserProfile.ID); } } }
		public int OpenIndex { get { UserOpened t = UserOpenedList.FirstOrDefault(a => a.UserProfileID == Globals.UserProfile.ID); if (t != null) { return t.OpenIndex; } else { return int.MaxValue; } } set { UserOpened t = UserOpenedList.FirstOrDefault(a => a.UserProfileID == Globals.UserProfile.ID); if (t == null) { UserOpened n = new UserOpened(); n.OpenIndex = value; UserOpenedList.Add(n); } else { t.OpenIndex = value; } } }

		public bool IsDirty { get { return (bool)GetValue(IsDirtyProperty); } set { if (IsActive == true) if (Globals.IsSaving == false) if (Globals.IsLoading == false) if(Globals.IsDocumentOpening == false) SetValue(IsDirtyProperty, value); } }
		public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register("IsDirty", typeof(bool), typeof(OpenableDocument), new UIPropertyMetadata(false));

		public bool IsLocked { get { return (bool)GetValue(IsLockedProperty); } set { SetValue(IsLockedProperty, value); } }
		public static readonly DependencyProperty IsLockedProperty = DependencyProperty.Register("IsLocked", typeof(bool), typeof(OpenableDocument), new UIPropertyMetadata(false));

		public OpenableDocument()
		{
			UserOpenedList = new List<UserOpened>();
			IsActive = false;
			IsDirty = false;
			IsLocked = false;
		}
	}

	public enum ProjectOutputFramework
	{
		NET30,
		NET35,
		NET35Client,
		NET40,
		NET40Client,
		NET45,
	}

	internal class DependencyProject : Project
	{

	}

	public class ProjectOutputPath : DependencyObject
	{
		private Guid projectID;
		public Guid ProjectID { get { return projectID; } }
		private Guid id;
		public Guid ID { get { return id; } }

		public bool IsEnabled { get { return (bool)GetValue(IsEnabledProperty); } set { SetValue(IsEnabledProperty, value); } }
		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(ProjectOutputPath), new UIPropertyMetadata(true));

		public string Path { get { return (string)GetValue(PathProperty); } set { SetValue(PathProperty, value); } }
		public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(ProjectOutputPath));

		public ProjectOutputFramework Framework { get { return (ProjectOutputFramework)GetValue(FrameworkProperty); } set { SetValue(FrameworkProperty, value); } }
		public static readonly DependencyProperty FrameworkProperty = DependencyProperty.Register("Framework", typeof(ProjectOutputFramework), typeof(ProjectOutputPath));

		public ProjectOutputPath() { }

		public ProjectOutputPath(Guid ProjectID, string Path)
		{
			id = Guid.NewGuid();
			projectID = ProjectID;
			this.Path = Path;
			this.Framework = ProjectOutputFramework.NET45;
		}
	}

	internal class Project : OpenableDocument
	{
		private Guid id = Guid.Empty;
		public Guid ID { get { return id; } }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Project));

		public Namespace Root { get { return (Namespace)GetValue(RootProperty); } set { SetValue(RootProperty, value); } }
		public static readonly DependencyProperty RootProperty = DependencyProperty.Register("Root", typeof(Namespace), typeof(Project));

		public bool Framework30 { get { return (bool)GetValue(Framework30Property); } set { SetValue(Framework30Property, value); } }
		public static readonly DependencyProperty Framework30Property = DependencyProperty.Register("Framework30", typeof(bool), typeof(Project));

		public bool Framework35 { get { return (bool)GetValue(Framework35Property); } set { SetValue(Framework35Property, value); } }
		public static readonly DependencyProperty Framework35Property = DependencyProperty.Register("Framework35", typeof(bool), typeof(Project));

		public bool Framework35Client { get { return (bool)GetValue(Framework35ClientProperty); } set { SetValue(Framework35ClientProperty, value); } }
		public static readonly DependencyProperty Framework35ClientProperty = DependencyProperty.Register("Framework35Client", typeof(bool), typeof(Project));

		public bool Framework40 { get { return (bool)GetValue(Framework40Property); } set { SetValue(Framework40Property, value); } }
		public static readonly DependencyProperty Framework40Property = DependencyProperty.Register("Framework40", typeof(bool), typeof(Project));

		public bool Framework40Client { get { return (bool)GetValue(Framework40ClientProperty); } set { SetValue(Framework40ClientProperty, value); } }
		public static readonly DependencyProperty Framework40ClientProperty = DependencyProperty.Register("Framework40Client", typeof(bool), typeof(Project));

		public bool Framework45 { get { return (bool)GetValue(Framework45Property); } set { SetValue(Framework45Property, value); } }
		public static readonly DependencyProperty Framework45Property = DependencyProperty.Register("Framework45", typeof(bool), typeof(Project));

		public ObservableCollection<string> DependencyProjects { get { return (ObservableCollection<string>)GetValue(DependencyProjectsProperty); } set { SetValue(DependencyProjectsProperty, value); } }
		public static readonly DependencyProperty DependencyProjectsProperty = DependencyProperty.Register("DependencyProjects", typeof(ObservableCollection<string>), typeof(Project));

		public ObservableCollection<ProjectUsingNamespace> UsingNamespaces { get { return (ObservableCollection<ProjectUsingNamespace>)GetValue(UsingNamespacesProperty); } set { SetValue(UsingNamespacesProperty, value); } }
		public static readonly DependencyProperty UsingNamespacesProperty = DependencyProperty.Register("UsingNamespaces", typeof(ObservableCollection<ProjectUsingNamespace>), typeof(Project));

		public ObservableCollection<Reference> References { get { return (ObservableCollection<Reference>)GetValue(ReferencesProperty); } set { SetValue(ReferencesProperty, value); } }
		public static readonly DependencyProperty ReferencesProperty = DependencyProperty.Register("References", typeof(ObservableCollection<Reference>), typeof(Project));

		public bool ServiceGenerateDataContracts { get { return (bool)GetValue(ServiceGenerateDataContractsProperty); } set { SetValue(ServiceGenerateDataContractsProperty, value); } } 
		public static readonly DependencyProperty ServiceGenerateDataContractsProperty = DependencyProperty.Register("ServiceGenerateDataContracts", typeof(bool), typeof(Project));

		public ObservableCollection<string> ServiceExcludedTypes { get { return (ObservableCollection<string>)GetValue(ServiceExcludedTypesProperty); } set { SetValue(ServiceExcludedTypesProperty, value); } }
		public static readonly DependencyProperty ServiceExcludedTypesProperty = DependencyProperty.Register("ServiceExcludedTypes", typeof(ObservableCollection<string>), typeof(Project));

		public bool ServiceAsyncMethods { get { return (bool)GetValue(ServiceAsyncMethodsProperty); } set { SetValue(ServiceAsyncMethodsProperty, value); } }
		public static readonly DependencyProperty ServiceAsyncMethodsProperty = DependencyProperty.Register("ServiceAsyncMethods", typeof(bool), typeof(Project));

		public bool ServiceAwaitableMethods { get { return (bool)GetValue(ServiceAwaitableMethodsProperty); } set { SetValue(ServiceAwaitableMethodsProperty, value); } }
		public static readonly DependencyProperty ServiceAwaitableMethodsProperty = DependencyProperty.Register("ServiceAwaitableMethods", typeof(bool), typeof(Project));

		public bool ServiceSerializerFaultsMethods { get { return (bool)GetValue(ServiceSerializerFaultsMethodsProperty); } set { SetValue(ServiceSerializerFaultsMethodsProperty, value); } }
		public static readonly DependencyProperty ServiceSerializerFaultsMethodsProperty = DependencyProperty.Register("ServiceSerializerFaultsMethods", typeof(bool), typeof(Project));

		public bool ServiceEnableDataBinding { get { return (bool)GetValue(ServiceEnableDataBindingProperty); } set { SetValue(ServiceEnableDataBindingProperty, value); } }
		public static readonly DependencyProperty ServiceEnableDataBindingProperty = DependencyProperty.Register("ServiceEnableDataBinding", typeof(bool), typeof(Project));

		public bool ServiceImportXMLTypes { get { return (bool)GetValue(ServiceImportXMLTypesProperty); } set { SetValue(ServiceImportXMLTypesProperty, value); } }
		public static readonly DependencyProperty ServiceImportXMLTypesProperty = DependencyProperty.Register("ServiceImportXMLTypes", typeof(bool), typeof(Project));

		public bool ServiceSerializable { get { return (bool)GetValue(ServiceSerializableProperty); } set { SetValue(ServiceSerializableProperty, value); } }
		public static readonly DependencyProperty ServiceSerializableProperty = DependencyProperty.Register("ServiceSerializable", typeof(bool), typeof(Project));

		public ProjectServiceSerializerType ServiceSerializer { get { return (ProjectServiceSerializerType)GetValue(ServiceSerializerProperty); } set { SetValue(ServiceSerializerProperty, value); } }
		public static readonly DependencyProperty ServiceSerializerProperty = DependencyProperty.Register("ServiceSerializer", typeof(ProjectServiceSerializerType), typeof(Project));

		public ProjectCollectionType ServiceCollectionType { get { return (ProjectCollectionType)GetValue(ServiceCollectionTypeProperty); } set { SetValue(ServiceCollectionTypeProperty, value); } }
		public static readonly DependencyProperty ServiceCollectionTypeProperty = DependencyProperty.Register("ServiceCollectionType", typeof(ProjectCollectionType), typeof(Project));

		public bool ServerPublicClasses { get { return (bool)GetValue(ServerPublicClassesProperty); } set { SetValue(ServerPublicClassesProperty, value); } }
		public static readonly DependencyProperty ServerPublicClassesProperty = DependencyProperty.Register("ServerPublicClasses", typeof(bool), typeof(Project));

		public bool ServerInternalClasses { get { return (bool)GetValue(ServerInternalClassesProperty); } set { SetValue(ServerInternalClassesProperty, value); } }
		public static readonly DependencyProperty ServerInternalClassesProperty = DependencyProperty.Register("ServerInternalClasses", typeof(bool), typeof(Project));

		public string ServerOutputFile { get { return (string)GetValue(ServerOutputFileProperty); } set { SetValue(ServerOutputFileProperty, value); } }
		public static readonly DependencyProperty ServerOutputFileProperty = DependencyProperty.Register("ServerOutputFile", typeof(string), typeof(Project));

		public ObservableCollection<ProjectOutputPath> ServerOutputPaths { get { return (ObservableCollection<ProjectOutputPath>)GetValue(ServerOutputPathsProperty); } set { SetValue(ServerOutputPathsProperty, value); } }
		public static readonly DependencyProperty ServerOutputPathsProperty = DependencyProperty.Register("ServerOutputPaths", typeof(ObservableCollection<ProjectOutputPath>), typeof(Project));

		public bool ClientPublicClasses { get { return (bool)GetValue(ClientPublicClassesProperty); } set { SetValue(ClientPublicClassesProperty, value); } }
		public static readonly DependencyProperty ClientPublicClassesProperty = DependencyProperty.Register("ClientPublicClasses", typeof(bool), typeof(Project));

		public bool ClientInternalClasses { get { return (bool)GetValue(ClientInternalClassesProperty); } set { SetValue(ClientInternalClassesProperty, value); } }
		public static readonly DependencyProperty ClientInternalClassesProperty = DependencyProperty.Register("ClientInternalClasses", typeof(bool), typeof(Project));

		public string ClientOutputFile { get { return (string)GetValue(ClientOutputFileProperty); } set { SetValue(ClientOutputFileProperty, value); } }
		public static readonly DependencyProperty ClientOutputFileProperty = DependencyProperty.Register("ClientOutputFile", typeof(string), typeof(Project));

		public ObservableCollection<ProjectOutputPath> ClientOutputPaths { get { return (ObservableCollection<ProjectOutputPath>)GetValue(ClientOutputPathsProperty); } set { SetValue(ClientOutputPathsProperty, value); } }
		public static readonly DependencyProperty ClientOutputPathsProperty = DependencyProperty.Register("ClientOutputPaths", typeof(ObservableCollection<ProjectOutputPath>), typeof(Project));

		//Internal Use - Searching / Filtering
		public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(Project));

		public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(Project));

		public bool IsFiltering { get { return (bool)GetValue(IsFilteringProperty); } set { SetValue(IsFilteringProperty, value); } }
		public static readonly DependencyProperty IsFilteringProperty = DependencyProperty.Register("IsFiltering", typeof(bool), typeof(Project));

		public bool IsFilterMatch { get { return (bool)GetValue(IsFilterMatchProperty); } set { SetValue(IsFilterMatchProperty, value); } }
		public static readonly DependencyProperty IsFilterMatchProperty = DependencyProperty.Register("IsFilterMatch", typeof(bool), typeof(Project));

		public bool IsTreeExpanded { get { return (bool)GetValue(IsTreeExpandedProperty); } set { SetValue(IsTreeExpandedProperty, value); } }
		public static readonly DependencyProperty IsTreeExpandedProperty = DependencyProperty.Register("IsTreeExpanded", typeof(bool), typeof(Project), new UIPropertyMetadata(true));

		private static System.Windows.Threading.Dispatcher WPFThread = Application.Current.Dispatcher;
		public Compiler.Compiler Compiler { get; protected set; }

		public string BuildOutputPath { get; set; }

		public Project() : base()
		{
			this.UserOpenedList = new List<UserOpened>();
			this.References = new ObservableCollection<Reference>();
			this.ServiceExcludedTypes = new ObservableCollection<string>();
			this.ServerOutputPaths = new ObservableCollection<ProjectOutputPath>();
			this.ClientOutputPaths = new ObservableCollection<ProjectOutputPath>();
			this.DependencyProjects = new ObservableCollection<string>();
		}

		public Project(string Name) : base()
		{
			this.UserOpenedList = new List<UserOpened>();
			this.References = new ObservableCollection<Reference>();
			this.ServiceExcludedTypes = new ObservableCollection<string>();
			this.ServerOutputPaths = new ObservableCollection<ProjectOutputPath>();
			this.ClientOutputPaths = new ObservableCollection<ProjectOutputPath>();
			this.DependencyProjects = new ObservableCollection<string>();

			this.id = Guid.NewGuid();
			this.Name = Name;
			this.Framework45 = true;
			this.ServiceAwaitableMethods = true;
			this.ClientPublicClasses = true;
			this.ServerPublicClasses = true;
			this.Root = new Namespace(Helpers.RegExs.ReplaceSpaces.Replace(Name, "."), null, this);
			this.Root.URI = "http://tempuri.org/" + this.Root.URI.Replace(".", "/") + "/";

			//Default Using Namespaces
			this.UsingNamespaces = new ObservableCollection<ProjectUsingNamespace>();
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System"));
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System.Collections"));
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System.Collections.Generic"));
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System.Collections.ObjectModel"));
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System.Collections.Specialized"));
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System.ComponentModel"));
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System.Net"));
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System.Net.Security"));
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System.Text"));
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System.Reflection"));
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System.Resources"));
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System.Runtime.CompilerServices"));
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System.Runtime.InteropServices"));
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System.Runtime.Serialization"));
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System.ServiceModel"));
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System.ServiceModel.Description"));
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System.Windows"));
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (e.Property == Project.IsSearchingProperty) return;
			if (e.Property == Project.IsSearchMatchProperty) return;
			if (e.Property == Project.IsFilteringProperty) return;
			if (e.Property == Project.IsFilterMatchProperty) return;
			if (e.Property == Project.IsTreeExpandedProperty) return;

			IsDirty = true;
		}

		public void Search(string Value)
		{
			Root.Search(Value);
		}

		public void Filter(bool FilterAll, bool FilterNamespaces, bool FilterServices, bool FilterData, bool FilterEnums)
		{
			Root.Filter(FilterAll, FilterNamespaces, FilterServices, FilterData, FilterEnums);
		}

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Project || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, this, this));
							if (Root.Name != null && Root.Name != "") if (Root.Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Namespace", Root.Name, this, this));
							if (Root.URI != null && Root.URI != "") if (Root.URI.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Namespace URI", Root.URI, this, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, this, this));
							if (Root.Name != null && Root.Name != "") if (Root.Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Namespace", Root.Name, this, this));
							if (Root.URI != null && Root.URI != "") if (Root.URI.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("ContractName", Root.URI, this, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, this, this));
						if (Root.Name != null && Root.Name != "") if (Args.RegexSearch.IsMatch(Root.Name)) results.Add(new FindReplaceResult("Namespace", Root.Name, this, this));
						if (Root.URI != null && Root.URI != "") if (Args.RegexSearch.IsMatch(Root.URI)) results.Add(new FindReplaceResult("Namespace URI", Root.URI, this, this));
					}
				}

				if (Args.ReplaceAll == true)
				{
					bool ia = IsActive;
					IsActive = true;
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (Root.Name != null && Root.Name != "") Root.Name = Microsoft.VisualBasic.Strings.Replace(Root.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (Root.URI != null && Root.URI != "") Root.URI = Microsoft.VisualBasic.Strings.Replace(Root.URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (Root.Name != null && Root.Name != "") Root.Name = Microsoft.VisualBasic.Strings.Replace(Root.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (Root.URI != null && Root.URI != "") Root.URI = Microsoft.VisualBasic.Strings.Replace(Root.URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (Root.Name != null && Root.Name != "") Root.Name = Args.RegexSearch.Replace(Root.Name, Args.Replace);
							if (Root.URI != null && Root.URI != "") Root.URI = Args.RegexSearch.Replace(Root.URI, Args.Replace);
						}
					}
					IsActive = ia;
				}
			}

			results.AddRange(Root.FindReplace(Args));

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
		{
			if (Args.ReplaceAll == true)
			{
				bool ia = IsActive;
				IsActive = true;
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Namespace") Root.Name = Microsoft.VisualBasic.Strings.Replace(Root.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Namespace URI") Root.URI = Microsoft.VisualBasic.Strings.Replace(Root.URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Namespace") Root.Name = Microsoft.VisualBasic.Strings.Replace(Root.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Namespace URI") Root.URI = Microsoft.VisualBasic.Strings.Replace(Root.URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (Field == "Namespace") Root.Name = Args.RegexSearch.Replace(Root.Name, Args.Replace);
						if (Field == "Namespace URI") Root.URI = Args.RegexSearch.Replace(Root.URI, Args.Replace);
					}
				}
				IsActive = ia;
			}
		}

		public bool ProjectHasServices()
		{
			if (Root.HasServices() == true) return true;
			return false;
		}

		public virtual void Open()
		{
			//Check the File Version property to see if it has changed and update the reference data accordingly. This has no effect on the build process and is merely a courtesy to the user.
			foreach (Reference R in References)
				R.UpdateVersion();

			Root.OpenItems();

			Compiler = new Compiler.Compiler(this);
			Globals.MainScreen.OpenProjectItem(Compiler);
		}

		public void PreBuild(bool IsOutputBuild)
		{
			BuildOutputPath = System.IO.Path.GetDirectoryName(Globals.ProjectSpace.FileName);
			BuildOutputPath = System.IO.Path.Combine(BuildOutputPath, Globals.ActiveProjectInfo.Name, Name);
			if (BuildOutputPath.EndsWith("\\") == false) BuildOutputPath += "\\";
			if (System.IO.Directory.Exists(BuildOutputPath) == false) System.IO.Directory.CreateDirectory(BuildOutputPath);

			//Execute a pre-build clean of all files in the output directory, but not if is an output build.
			if (IsOutputBuild == false)
			{
				string[] filePaths = System.IO.Directory.GetFiles(BuildOutputPath);
				foreach (string filePath in filePaths)
					System.IO.File.Delete(filePath);
			}

			foreach (Reference R in References)
			{
				R.IsGACInstalled = Helpers.GACUtil.IsAssemblyInGAC(R.Name);
			}
		}

		public bool VerifyCode()
		{
			bool NoErrors = true;

			if ((ServerOutputFile == "" || ServerOutputFile == null))
			{
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS0003", "The '" + Name + "' project does not have a Server Output File Name. You must specify a Server Output File Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, null, this, this.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchFileName.IsMatch(ServerOutputFile) == false)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS0004", "The Server Output File Name in '" + Name + "' project is not set or contains invalid characters. You must specify a valid Windows file name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, null, this, this.GetType()));
					NoErrors = false;
				}
			if ((ClientOutputFile == "" || ClientOutputFile == null))
			{
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS0005", "The '" + Name + "' project does not have a Client Output File Name. You must specify a Client Output File Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, null, this, this.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchFileName.IsMatch(ClientOutputFile) == false)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS0006", "The Client Output File Name in '" + Name + "' project is not set or contains invalid characters. You must specify a valid Windows file name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, null, this, this.GetType()));
					NoErrors = false;
				}
			if ((ServerOutputFile == ClientOutputFile))
			{
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS0007", "The '" + Name + "' project Client and Server Output File Names are the same. You must specify a different Server or Client Assembly Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, null, this, this.GetType()));
				NoErrors = false;
			}

			if (Root.URI == null || Root.URI == "")
			{
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS1000", "The '" + Root.Name + "' namespace in the '" + Name + "' project does not have a URI. Namespaces MUST have a URI.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, this, this.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchHTTPURI.IsMatch(Root.URI) == false)
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS1001", "The URI '" + Root.URI + "' for the '" + Root.Name + "' namespace in the '" + Name + "' project is not a valid URI. Any associated services and data may not function properly.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, this, this, this.GetType()));

			return NoErrors;
		}

		public virtual string GenerateServerCode30(string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET30) + ServerOutputFile + ".NET30.cs";

			StringBuilder Code = new StringBuilder();
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine("// This code was generated by a tool. Changes to this file may cause ");
			Code.AppendLine("// incorrect behavior and will be lost if the code is regenerated.");
			Code.AppendLine("//");
			Code.AppendFormat("// WCF Architect Version:\t{0}{1}", Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendLine("// .NET Framework Version:\t3.0");
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine();
			foreach (ProjectUsingNamespace PUN in UsingNamespaces)
				Code.AppendFormat("using {0};{1}", PUN.Namespace, Environment.NewLine);
			Code.AppendLine();

			//Generate Root Namespace
			Code.AppendLine(Root.GenerateServerCode30(ProjectName));
			Code.AppendLine();

			System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateServerCode35(string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET35) + ServerOutputFile + ".NET35.cs";

			StringBuilder Code = new StringBuilder();
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine("// This code was generated by a tool. Changes to this file may cause ");
			Code.AppendLine("// incorrect behavior and will be lost if the code is regenerated.");
			Code.AppendLine("//");
			Code.AppendFormat("// WCF Architect Version:\t{0}{1}", Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendLine("// .NET Framework Version:\t3.5");
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine();
			foreach (ProjectUsingNamespace PUN in UsingNamespaces)
				Code.AppendFormat("using {0};{1}", PUN.Namespace, Environment.NewLine);
			Code.AppendLine();

			//Generate Root Namespace
			Code.AppendLine(Root.GenerateServerCode35(ProjectName));
			Code.AppendLine();
			
			System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateServerCode35Client(string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET35Client) + ServerOutputFile + ".NET35Client.cs";

			StringBuilder Code = new StringBuilder();
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine("// This code was generated by a tool. Changes to this file may cause ");
			Code.AppendLine("// incorrect behavior and will be lost if the code is regenerated.");
			Code.AppendLine("//");
			Code.AppendFormat("// WCF Architect Version:\t{0}{1}", Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendLine("// .NET Framework Version:\t3.5 (Client)");
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine();
			foreach (ProjectUsingNamespace PUN in UsingNamespaces)
				if (PUN.IsFullFrameworkOnly == false)
					Code.AppendFormat("using {0};{1}", PUN.Namespace, Environment.NewLine);
			Code.AppendLine();

			//Generate Root Namespace
			Code.AppendLine(Root.GenerateServerCode35Client(ProjectName));
			Code.AppendLine();

			System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateServerCode40(string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET40) + ServerOutputFile + ".NET40.cs";

			StringBuilder Code = new StringBuilder();
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine("// This code was generated by a tool. Changes to this file may cause ");
			Code.AppendLine("// incorrect behavior and will be lost if the code is regenerated.");
			Code.AppendLine("//");
			Code.AppendFormat("// WCF Architect Version:\t{0}{1}", Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendLine("// .NET Framework Version:\t4.0");
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine();
			foreach (ProjectUsingNamespace PUN in UsingNamespaces)
				Code.AppendFormat("using {0};{1}", PUN.Namespace, Environment.NewLine);
			Code.AppendLine();

			//Generate Root Namespace
			Code.AppendLine(Root.GenerateServerCode40(ProjectName));
			Code.AppendLine();

			System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateServerCode40Client(string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET40Client) + ServerOutputFile + ".NET40Client.cs";

			StringBuilder Code = new StringBuilder();
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine("// This code was generated by a tool. Changes to this file may cause ");
			Code.AppendLine("// incorrect behavior and will be lost if the code is regenerated.");
			Code.AppendLine("//");
			Code.AppendFormat("// WCF Architect Version:\t{0}{1}", Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendLine("// .NET Framework Version:\t4.0 (Client)");
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine();
			foreach (ProjectUsingNamespace PUN in UsingNamespaces)
				if (PUN.IsFullFrameworkOnly == false)
					Code.AppendFormat("using {0};{1}", PUN.Namespace, Environment.NewLine);
			Code.AppendLine();

			//Generate Root Namespace
			Code.AppendLine(Root.GenerateServerCode40Client(ProjectName));
			Code.AppendLine();

			System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateClientCode30(string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET30) + ClientOutputFile + ".NET30.cs";

			StringBuilder Code = new StringBuilder();
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine("// This code was generated by a tool. Changes to this file may cause ");
			Code.AppendLine("// incorrect behavior and will be lost if the code is regenerated.");
			Code.AppendLine("//");
			Code.AppendFormat("// WCF Architect Version:\t{0}{1}", Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendLine("// .NET Framework Version:\t3.0");
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine();
			foreach (ProjectUsingNamespace PUN in UsingNamespaces)
				Code.AppendFormat("using {0};{1}", PUN.Namespace, Environment.NewLine);
			Code.AppendLine();

			//Generate Root Namespace
			Code.AppendLine(Root.GenerateClientCode30(ProjectName));
			Code.AppendLine();

			System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateClientCode35(string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET35) + ClientOutputFile + ".NET35.cs";

			StringBuilder Code = new StringBuilder();
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine("// This code was generated by a tool. Changes to this file may cause ");
			Code.AppendLine("// incorrect behavior and will be lost if the code is regenerated.");
			Code.AppendLine("//");
			Code.AppendFormat("// WCF Architect Version:\t{0}{1}", Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendLine("// .NET Framework Version:\t3.5");
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine();
			foreach (ProjectUsingNamespace PUN in UsingNamespaces)
				Code.AppendFormat("using {0};{1}", PUN.Namespace, Environment.NewLine);
			Code.AppendLine();

			//Generate Root Namespace
			Code.AppendLine(Root.GenerateClientCode35(ProjectName));
			Code.AppendLine();

			System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateClientCode35Client(string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET35Client) + ClientOutputFile + ".NET35Client.cs";

			StringBuilder Code = new StringBuilder();
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine("// This code was generated by a tool. Changes to this file may cause ");
			Code.AppendLine("// incorrect behavior and will be lost if the code is regenerated.");
			Code.AppendLine("//");
			Code.AppendFormat("// WCF Architect Version:\t{0}{1}", Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendLine("// .NET Framework Version:\t3.5 (Client)");
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine();
			foreach (ProjectUsingNamespace PUN in UsingNamespaces)
				if (PUN.IsFullFrameworkOnly == false)
					Code.AppendFormat("using {0};{1}", PUN.Namespace, Environment.NewLine);
			Code.AppendLine();

			//Generate Root Namespace
			Code.AppendLine(Root.GenerateClientCode35Client(ProjectName));
			Code.AppendLine();

			System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateClientCode40(string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET40) + ClientOutputFile + ".NET40.cs";

			StringBuilder Code = new StringBuilder();
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine("// This code was generated by a tool. Changes to this file may cause ");
			Code.AppendLine("// incorrect behavior and will be lost if the code is regenerated.");
			Code.AppendLine("//");
			Code.AppendFormat("// WCF Architect Version:\t{0}{1}", Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendLine("// .NET Framework Version:\t4.0");
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine();
			foreach (ProjectUsingNamespace PUN in UsingNamespaces)
				Code.AppendFormat("using {0};{1}", PUN.Namespace, Environment.NewLine);
			Code.AppendLine();

			//Generate Root Namespace
			Code.AppendLine(Root.GenerateClientCode40(ProjectName));
			Code.AppendLine();

			System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateClientCode40Client(string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET40Client) + ClientOutputFile + ".NET40Client.cs";

			StringBuilder Code = new StringBuilder();
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine("// This code was generated by a tool. Changes to this file may cause ");
			Code.AppendLine("// incorrect behavior and will be lost if the code is regenerated.");
			Code.AppendLine("//");
			Code.AppendFormat("// WCF Architect Version:\t{0}{1}", Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendLine("// .NET Framework Version:\t4.0 (Client)");
			Code.AppendLine("//---------------------------------------------------------------------------");
			Code.AppendLine();
			foreach (ProjectUsingNamespace PUN in UsingNamespaces)
				if (PUN.IsFullFrameworkOnly == false)
					Code.AppendFormat("using {0};{1}", PUN.Namespace, Environment.NewLine);
			Code.AppendLine();

			//Generate Root Namespace
			Code.AppendLine(Root.GenerateClientCode40Client(ProjectName));
			Code.AppendLine();

			System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public string GenerateClientProxyArgs(bool ClientInternalClasses)
		{
			StringBuilder CodeArgs = new StringBuilder();
			Compiler.GenerateClientProxyNamespaces(CodeArgs, Root);
			if (ServiceAsyncMethods == true) CodeArgs.Append(" /async");
			if (ServiceAwaitableMethods == false && Framework45 == true) CodeArgs.Append(" /syncOnly");
			if (ServiceEnableDataBinding == true) CodeArgs.Append(" /edb");
			if (ServiceImportXMLTypes == true) CodeArgs.Append(" /importXmlTypes");
			if (ClientInternalClasses == true) CodeArgs.Append(" /i");
			if (ServiceSerializable == true) CodeArgs.Append(" /s");
			if (ServiceSerializer == Projects.ProjectServiceSerializerType.DataContract) CodeArgs.Append(" /ser:DataContractSerializer");
			if (ServiceSerializer == Projects.ProjectServiceSerializerType.XML) CodeArgs.Append(" /ser:XmlSerializer");
			if (ServiceCollectionType == ProjectCollectionType.List) CodeArgs.Append(" /ct:System.Collections.Generic.List`1");
			if (ServiceCollectionType == ProjectCollectionType.SynchronizedCollection) CodeArgs.Append(" /ct:System.Collections.Generic.SynchronizedCollection`1");
			if (ServiceCollectionType == ProjectCollectionType.Collection) CodeArgs.Append(" /ct:System.Collections.ObjectModel.Collection`1");
			if (ServiceCollectionType == ProjectCollectionType.ObservableCollection) CodeArgs.Append(" /ct:System.Collections.ObjectModel.ObservableCollection`1");
			if (ServiceCollectionType == ProjectCollectionType.BindingList) CodeArgs.Append(" /ct:System.ComponentModel.BindingList`1");
			CodeArgs.Append(" /nologo");
			return CodeArgs.ToString();
		}
	}

	internal class Developer : DependencyObject
	{
		public Guid ID { get { return (Guid)GetValue(IDProperty); } set { SetValue(IDProperty, value); } }
		public static readonly DependencyProperty IDProperty = DependencyProperty.Register("ID", typeof(Guid), typeof(Developer));

		public string UserName { get { return (string)GetValue(UserNameProperty); } set { SetValue(UserNameProperty, value); } }
		public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register("UserName", typeof(string), typeof(Developer));

		public string ComputerName { get { return (string)GetValue(ComputerNameProperty); } set { SetValue(ComputerNameProperty, value); } }
		public static readonly DependencyProperty ComputerNameProperty = DependencyProperty.Register("ComputerName", typeof(string), typeof(Developer));

		public bool LastEditedBy { get { return (bool)GetValue(LastEditedByProperty); } set { SetValue(LastEditedByProperty, value); } }
		public static readonly DependencyProperty LastEditedByProperty = DependencyProperty.Register("LastEditedBy", typeof(bool), typeof(Developer));
	}
}