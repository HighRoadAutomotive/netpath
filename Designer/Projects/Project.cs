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

	internal class Project : OpenableDocument
	{
		private Guid id = Guid.Empty;
		public Guid ID { get { return id; } }

		protected bool isDependencyProject = false;
		public bool IsDependencyProject { get { return isDependencyProject; } }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Project));

		public string NamespaceName { get { return (string)GetValue(NamespaceNameProperty); } set { SetValue(NamespaceNameProperty, value); } }
		public static readonly DependencyProperty NamespaceNameProperty = DependencyProperty.Register("NamespaceName", typeof(string), typeof(Project));

		public Namespace Namespace { get { return (Namespace)GetValue(NamespaceProperty); } set { SetValue(NamespaceProperty, value); } }
		public static readonly DependencyProperty NamespaceProperty = DependencyProperty.Register("Namespace", typeof(Namespace), typeof(Project));

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

		public bool Silverlight30 { get { return (bool)GetValue(Silverlight30Property); } set { SetValue(Silverlight30Property, value); } }
		public static readonly DependencyProperty Silverlight30Property = DependencyProperty.Register("Silverlight30", typeof(bool), typeof(Project));

		public bool Silverlight40 { get { return (bool)GetValue(Silverlight40Property); } set { SetValue(Silverlight40Property, value); } }
		public static readonly DependencyProperty Silverlight40Property = DependencyProperty.Register("Silverlight40", typeof(bool), typeof(Project));
		
		public bool PlatformX86 { get { return (bool)GetValue(PlatformX86Property); } set { SetValue(PlatformX86Property, value); } }
		public static readonly DependencyProperty PlatformX86Property = DependencyProperty.Register("PlatformX86", typeof(bool), typeof(Project));

		public bool PlatformX64 { get { return (bool)GetValue(PlatformX64Property); } set { SetValue(PlatformX64Property, value); } }
		public static readonly DependencyProperty PlatformX64Property = DependencyProperty.Register("PlatformX64", typeof(bool), typeof(Project));

		public bool PlatformItanium { get { return (bool)GetValue(PlatformItaniumProperty); } set { SetValue(PlatformItaniumProperty, value); } }
		public static readonly DependencyProperty PlatformItaniumProperty = DependencyProperty.Register("PlatformItanium", typeof(bool), typeof(Project));

		public bool PlatformAnyCPU { get { return (bool)GetValue(PlatformAnyCPUProperty); } set { SetValue(PlatformAnyCPUProperty, value); } }
		public static readonly DependencyProperty PlatformAnyCPUProperty = DependencyProperty.Register("PlatformAnyCPU", typeof(bool), typeof(Project));

		public bool ConfigurationDebug { get { return (bool)GetValue(ConfigurationDebugProperty); } set { SetValue(ConfigurationDebugProperty, value); } }
		public static readonly DependencyProperty ConfigurationDebugProperty = DependencyProperty.Register("ConfigurationDebug", typeof(bool), typeof(Project));

		public bool ConfigurationRelease { get { return (bool)GetValue(ConfigurationReleaseProperty); } set { SetValue(ConfigurationReleaseProperty, value); } }
		public static readonly DependencyProperty ConfigurationReleaseProperty = DependencyProperty.Register("ConfigurationRelease", typeof(bool), typeof(Project));

		public ObservableCollection<Namespace> Namespaces { get { return (ObservableCollection<Namespace>)GetValue(NamespacesProperty); } set { SetValue(NamespacesProperty, value); } }
		public static readonly DependencyProperty NamespacesProperty = DependencyProperty.Register("Namespaces", typeof(ObservableCollection<Namespace>), typeof(Project));

		public ObservableCollection<Enum> Enums { get { return (ObservableCollection<Enum>)GetValue(EnumsProperty); } set { SetValue(EnumsProperty, value); } }
		public static readonly DependencyProperty EnumsProperty = DependencyProperty.Register("Enums", typeof(ObservableCollection<Enum>), typeof(Project));

		public ObservableCollection<Data> Data { get { return (ObservableCollection<Data>)GetValue(DataProperty); } set { SetValue(DataProperty, value); } }
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(ObservableCollection<Data>), typeof(Project));

		public ObservableCollection<Service> Services { get { return (ObservableCollection<Service>)GetValue(ServicesProperty); } set { SetValue(ServicesProperty, value); } }
		public static readonly DependencyProperty ServicesProperty = DependencyProperty.Register("Services", typeof(ObservableCollection<Service>), typeof(Project));

		public ObservableCollection<ProjectUsingNamespace> UsingNamespaces { get { return (ObservableCollection<ProjectUsingNamespace>)GetValue(UsingNamespacesProperty); } set { SetValue(UsingNamespacesProperty, value); } }
		public static readonly DependencyProperty UsingNamespacesProperty = DependencyProperty.Register("UsingNamespaces", typeof(ObservableCollection<ProjectUsingNamespace>), typeof(Project));

		public ObservableCollection<ServiceBinding> Bindings { get { return (ObservableCollection<ServiceBinding>)GetValue(BindingsProperty); } set { SetValue(BindingsProperty, value); } }
		public static readonly DependencyProperty BindingsProperty = DependencyProperty.Register("Bindings", typeof(ObservableCollection<ServiceBinding>), typeof(Project));

		public string BindingsNamespace { get { return (string)GetValue(BindingsNamespaceProperty); } set { SetValue(BindingsNamespaceProperty, value); } }
		public static readonly DependencyProperty BindingsNamespaceProperty = DependencyProperty.Register("BindingsNamespace", typeof(string), typeof(Project));

		public ObservableCollection<BindingSecurity> Security { get { return (ObservableCollection<BindingSecurity>)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(ObservableCollection<BindingSecurity>), typeof(Project));

		public string SecurityClass { get { return (string)GetValue(SecurityClassProperty); } set { SetValue(SecurityClassProperty, value); } }
		public static readonly DependencyProperty SecurityClassProperty = DependencyProperty.Register("SecurityClass", typeof(string), typeof(Project));

		public ObservableCollection<Host> Hosts { get { return (ObservableCollection<Host>)GetValue(HostsProperty); } set { SetValue(HostsProperty, value); } }
		public static readonly DependencyProperty HostsProperty = DependencyProperty.Register("Hosts", typeof(ObservableCollection<Host>), typeof(Project));

		public string HostsNamespace { get { return (string)GetValue(HostsNamespaceProperty); } set { SetValue(HostsNamespaceProperty, value); } }
		public static readonly DependencyProperty HostsNamespaceProperty = DependencyProperty.Register("HostsNamespace", typeof(string), typeof(Project));

		public ObservableCollection<Reference> References { get { return (ObservableCollection<Reference>)GetValue(ReferencesProperty); } set { SetValue(ReferencesProperty, value); } }
		public static readonly DependencyProperty ReferencesProperty = DependencyProperty.Register("References", typeof(ObservableCollection<Reference>), typeof(Project));

		public bool ServerPublicClasses { get { return (bool)GetValue(ServerPublicClassesProperty); } set { SetValue(ServerPublicClassesProperty, value); } }
		public static readonly DependencyProperty ServerPublicClassesProperty = DependencyProperty.Register("ServerPublicClasses", typeof(bool), typeof(Project));

		public bool ServerInternalClasses { get { return (bool)GetValue(ServerInternalClassesProperty); } set { SetValue(ServerInternalClassesProperty, value); } }
		public static readonly DependencyProperty ServerInternalClassesProperty = DependencyProperty.Register("ServerInternalClasses", typeof(bool), typeof(Project));

		public string ServerAssemblyName { get { return (string)GetValue(ServerAssemblyNameProperty); } set { SetValue(ServerAssemblyNameProperty, value); } }
		public static readonly DependencyProperty ServerAssemblyNameProperty = DependencyProperty.Register("ServerAssemblyName", typeof(string), typeof(Project));

		public ObservableCollection<ProjectOutputPath> ServerOutputPaths { get { return (ObservableCollection<ProjectOutputPath>)GetValue(ServerOutputPathsProperty); } set { SetValue(ServerOutputPathsProperty, value); } }
		public static readonly DependencyProperty ServerOutputPathsProperty = DependencyProperty.Register("ServerOutputPaths", typeof(ObservableCollection<ProjectOutputPath>), typeof(Project));

		public bool ClientPublicClasses { get { return (bool)GetValue(ClientPublicClassesProperty); } set { SetValue(ClientPublicClassesProperty, value); } }
		public static readonly DependencyProperty ClientPublicClassesProperty = DependencyProperty.Register("ClientPublicClasses", typeof(bool), typeof(Project));

		public bool ClientInternalClasses { get { return (bool)GetValue(ClientInternalClassesProperty); } set { SetValue(ClientInternalClassesProperty, value); } }
		public static readonly DependencyProperty ClientInternalClassesProperty = DependencyProperty.Register("ClientInternalClasses", typeof(bool), typeof(Project));

		public string ClientDefaultNamespace { get { return (string)GetValue(ClientDefaultNamespaceProperty); } set { SetValue(ClientDefaultNamespaceProperty, value); } }
		public static readonly DependencyProperty ClientDefaultNamespaceProperty = DependencyProperty.Register("ClientDefaultNamespace", typeof(string), typeof(Project));

		public string ClientAssemblyName { get { return (string)GetValue(ClientAssemblyNameProperty); } set { SetValue(ClientAssemblyNameProperty, value); } }
		public static readonly DependencyProperty ClientAssemblyNameProperty = DependencyProperty.Register("ClientAssemblyName", typeof(string), typeof(Project));

		public ObservableCollection<ProjectOutputPath> ClientOutputPaths { get { return (ObservableCollection<ProjectOutputPath>)GetValue(ClientOutputPathsProperty); } set { SetValue(ClientOutputPathsProperty, value); } }
		public static readonly DependencyProperty ClientOutputPathsProperty = DependencyProperty.Register("ClientOutputPaths", typeof(ObservableCollection<ProjectOutputPath>), typeof(Project));

		public bool ServiceGenerateDataContracts { get { return (bool)GetValue(ServiceGenerateDataContractsProperty); } set { SetValue(ServiceGenerateDataContractsProperty, value); } } 
		public static readonly DependencyProperty ServiceGenerateDataContractsProperty = DependencyProperty.Register("ServiceGenerateDataContracts", typeof(bool), typeof(Project));

		public ObservableCollection<string> ServiceExcludedTypes { get { return (ObservableCollection<string>)GetValue(ServiceExcludedTypesProperty); } set { SetValue(ServiceExcludedTypesProperty, value); } }
		public static readonly DependencyProperty ServiceExcludedTypesProperty = DependencyProperty.Register("ServiceExcludedTypes", typeof(ObservableCollection<string>), typeof(Project));

		public bool ServiceAsyncMethods { get { return (bool)GetValue(ServiceAsyncMethodsProperty); } set { SetValue(ServiceAsyncMethodsProperty, value); } }
		public static readonly DependencyProperty ServiceAsyncMethodsProperty = DependencyProperty.Register("ServiceAsyncMethods", typeof(bool), typeof(Project));

		public bool ServiceEnableDataBinding { get { return (bool)GetValue(ServiceEnableDataBindingProperty); } set { SetValue(ServiceEnableDataBindingProperty, value); } }
		public static readonly DependencyProperty ServiceEnableDataBindingProperty = DependencyProperty.Register("ServiceEnableDataBinding", typeof(bool), typeof(Project));

		public bool ServiceImportXMLTypes { get { return (bool)GetValue(ServiceImportXMLTypesProperty); } set { SetValue(ServiceImportXMLTypesProperty, value); } }
		public static readonly DependencyProperty ServiceImportXMLTypesProperty = DependencyProperty.Register("ServiceImportXMLTypes", typeof(bool), typeof(Project));

		public bool ServiceGenerateMessageContracts { get { return (bool)GetValue(ServiceGenerateMessageContractsProperty); } set { SetValue(ServiceGenerateMessageContractsProperty, value); } }
		public static readonly DependencyProperty ServiceGenerateMessageContractsProperty = DependencyProperty.Register("ServiceGenerateMessageContracts", typeof(bool), typeof(Project));

		public bool ServiceNoConfig { get { return (bool)GetValue(ServiceNoConfigProperty); } set { SetValue(ServiceNoConfigProperty, value); } }
		public static readonly DependencyProperty ServiceNoConfigProperty = DependencyProperty.Register("ServiceNoConfig", typeof(bool), typeof(Project));

		public bool ServiceNoStandardLibrary { get { return (bool)GetValue(ServiceNoStandardLibraryProperty); } set { SetValue(ServiceNoStandardLibraryProperty, value); } }
		public static readonly DependencyProperty ServiceNoStandardLibraryProperty = DependencyProperty.Register("ServiceNoStandardLibrary", typeof(bool), typeof(Project));

		public bool ServiceSerializable { get { return (bool)GetValue(ServiceSerializableProperty); } set { SetValue(ServiceSerializableProperty, value); } }
		public static readonly DependencyProperty ServiceSerializableProperty = DependencyProperty.Register("ServiceSerializable", typeof(bool), typeof(Project));

		public bool ServiceWrapped { get { return (bool)GetValue(ServiceWrappedProperty); } set { SetValue(ServiceWrappedProperty, value); } }
		public static readonly DependencyProperty ServiceWrappedProperty = DependencyProperty.Register("ServiceWrapped", typeof(bool), typeof(Project));

		public ProjectServiceSerializerType ServiceSerializer { get { return (ProjectServiceSerializerType)GetValue(ServiceSerializerProperty); } set { SetValue(ServiceSerializerProperty, value); } }
		public static readonly DependencyProperty ServiceSerializerProperty = DependencyProperty.Register("ServiceSerializer", typeof(ProjectServiceSerializerType), typeof(Project));

		public ProjectCollectionType ServiceCollectionType { get { return (ProjectCollectionType)GetValue(ServiceCollectionTypeProperty); } set { SetValue(ServiceCollectionTypeProperty, value); } }
		public static readonly DependencyProperty ServiceCollectionTypeProperty = DependencyProperty.Register("ServiceCollectionType", typeof(ProjectCollectionType), typeof(Project));

		public ObservableCollection<string> ServiceCollectionTypes { get { return (ObservableCollection<string>)GetValue(ServiceCollectionTypesProperty); } set { SetValue(ServiceCollectionTypesProperty, value); } }
		public static readonly DependencyProperty ServiceCollectionTypesProperty = DependencyProperty.Register("ServiceCollectionTypes", typeof(ObservableCollection<string>), typeof(Project));

		public string AssemblyTitle { get { return (string)GetValue(AssemblyTitleProperty); } set { SetValue(AssemblyTitleProperty, value); } }
		public static readonly DependencyProperty AssemblyTitleProperty = DependencyProperty.Register("AssemblyTitle", typeof(string), typeof(Project));

		public string AssemblyDescription { get { return (string)GetValue(AssemblyDescriptionProperty); } set { SetValue(AssemblyDescriptionProperty, value); } }
		public static readonly DependencyProperty AssemblyDescriptionProperty = DependencyProperty.Register("AssemblyDescription", typeof(string), typeof(Project));

		public string AssemblyCompany { get { return (string)GetValue(AssemblyCompanyProperty); } set { SetValue(AssemblyCompanyProperty, value); } }
		public static readonly DependencyProperty AssemblyCompanyProperty = DependencyProperty.Register("AssemblyCompany", typeof(string), typeof(Project));

		public string AssemblyProduct { get { return (string)GetValue(AssemblyProductProperty); } set { SetValue(AssemblyProductProperty, value); } }
		public static readonly DependencyProperty AssemblyProductProperty = DependencyProperty.Register("AssemblyProduct", typeof(string), typeof(Project));

		public string AssemblyCopyright { get { return (string)GetValue(AssemblyCopyrightProperty); } set { SetValue(AssemblyCopyrightProperty, value); } }
		public static readonly DependencyProperty AssemblyCopyrightProperty = DependencyProperty.Register("AssemblyCopyright", typeof(string), typeof(Project));

		public string AssemblyTrademark { get { return (string)GetValue(AssemblyTrademarkProperty); } set { SetValue(AssemblyTrademarkProperty, value); } }
		public static readonly DependencyProperty AssemblyTrademarkProperty = DependencyProperty.Register("AssemblyTrademark", typeof(string), typeof(Project));

		public Prospective.Utilities.Types.Version AssemblyVersion { get { return (Prospective.Utilities.Types.Version)GetValue(AssemblyVersionProperty); } set { SetValue(AssemblyVersionProperty, value); } }
		public static readonly DependencyProperty AssemblyVersionProperty = DependencyProperty.Register("AssemblyVersion", typeof(Prospective.Utilities.Types.Version), typeof(Project));

		public Prospective.Utilities.Types.Version AssemblyFileVersion { get { return (Prospective.Utilities.Types.Version)GetValue(AssemblyFileVersionProperty); } set { SetValue(AssemblyFileVersionProperty, value); } }
		public static readonly DependencyProperty AssemblyFileVersionProperty = DependencyProperty.Register("AssemblyFileVersion", typeof(Prospective.Utilities.Types.Version), typeof(Project));

		public bool AssemblyCOMVisible { get { return (bool)GetValue(AssemblyCOMVisibleProperty); } set { SetValue(AssemblyCOMVisibleProperty, value); } }
		public static readonly DependencyProperty AssemblyCOMVisibleProperty = DependencyProperty.Register("AssemblyCOMVisible", typeof(bool), typeof(Project));

		public Guid AssemblyCOMGuid { get { return (Guid)GetValue(AssemblyCOMGuidProperty); } set { SetValue(AssemblyCOMGuidProperty, value); } }
		public static readonly DependencyProperty AssemblyCOMGuidProperty = DependencyProperty.Register("AssemblyCOMGuid", typeof(Guid), typeof(Project));

		public ObservableCollection<DependencyProject> DependencyProjects { get { return (ObservableCollection<DependencyProject>)GetValue(DependencyProjectsProperty); } set { SetValue(DependencyProjectsProperty, value); } }
		public static readonly DependencyProperty DependencyProjectsProperty = DependencyProperty.Register("DependencyProjects", typeof(ObservableCollection<DependencyProject>), typeof(Project));

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
			this.DependencyProjects = new ObservableCollection<DependencyProject>();
			this.Services = new ObservableCollection<Service>();
			this.Data = new ObservableCollection<Data>();
			this.Enums = new ObservableCollection<Enum>();

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

		public Project(string Name) : base()
		{
			this.UserOpenedList = new List<UserOpened>();
			this.Namespaces = new ObservableCollection<Namespace>();
			this.Services = new ObservableCollection<Service>();
			this.Data = new ObservableCollection<Data>();
			this.Enums = new ObservableCollection<Enum>();
			this.Bindings = new ObservableCollection<ServiceBinding>();
			this.Security = new ObservableCollection<BindingSecurity>();
			this.Hosts = new ObservableCollection<Host>();
			this.References = new ObservableCollection<Reference>();
			this.ServiceExcludedTypes = new ObservableCollection<string>();
			this.ServiceCollectionTypes = new ObservableCollection<string>();
			this.ServerOutputPaths = new ObservableCollection<ProjectOutputPath>();
			this.ClientOutputPaths = new ObservableCollection<ProjectOutputPath>();
			this.DependencyProjects = new ObservableCollection<DependencyProject>();

			this.id = Guid.NewGuid();
			this.Name = Name;
			this.AssemblyTitle = Name;
			this.NamespaceName = Helpers.RegExs.ReplaceSpaces.Replace(Name, ".");
			this.PlatformAnyCPU = true;
			this.ConfigurationDebug = true;
			this.Framework40 = true;
			this.AssemblyFileVersion = new Prospective.Utilities.Types.Version(1, 0);
			this.AssemblyVersion = new Prospective.Utilities.Types.Version(1, 0);
			this.AssemblyCOMGuid = Guid.Empty;
			this.ServiceNoConfig = true;
			this.ClientPublicClasses = true;
			this.ServerPublicClasses = true;
			this.Namespace = new Namespace(NamespaceName, null, this);
			this.Namespace.URI = "http://tempuri.org/" + NamespaceName.Replace(".", "/") + "/";

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
			if (Value != "")
			{
				foreach (Projects.ServiceBinding T in Bindings)
				{
					T.IsSearching = true;
					T.IsSearchMatch = T.Name.Contains(Value);
				}
				foreach (Projects.BindingSecurity T in Security)
				{
					T.IsSearching = true;
					T.IsSearchMatch = T.Name.Contains(Value);
				}
				foreach (Projects.Host T in Hosts)
				{
					T.IsSearching = true;
					T.IsSearchMatch = T.Name.Contains(Value);
				}
				foreach (Projects.Namespace T in Namespaces)
					T.Search(Value);
				foreach (Service T in Services)
				{
					T.IsSearching = true;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
				foreach (Data T in Data)
				{
					T.IsSearching = true;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
				foreach (Enum T in Enums)
				{
					T.IsSearching = true;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
			}
			else
			{
				foreach (Projects.ServiceBinding T in Bindings)
				{
					T.IsSearching = false;
					T.IsSearchMatch = false;
				}
				foreach (Projects.BindingSecurity T in Security)
				{
					T.IsSearching = false;
					T.IsSearchMatch = false;
				}
				foreach (Projects.Host T in Hosts)
				{
					T.IsSearching = false;
					T.IsSearchMatch = false;
				}
				foreach (Projects.Namespace T in Namespaces)
					T.Search(Value);
				foreach (Service T in Services)
				{
					T.IsSearching = false;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
				foreach (Data T in Data)
				{
					T.IsSearching = false;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
				foreach (Enum T in Enums)
				{
					T.IsSearching = false;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
			}
		}

		public void Filter(bool FilterAll, bool FilterNamespaces, bool FilterServices, bool FilterData, bool FilterEnums)
		{
			foreach (Service T in Services)
			{
				T.IsFiltering = !FilterAll;
				T.IsFilterMatch = false;
				if (FilterServices == true) T.IsFilterMatch = true;
			}
			foreach (Data T in Data)
			{
				T.IsFiltering = !FilterAll;
				T.IsFilterMatch = false;
				if (FilterData == true) T.IsFilterMatch = true;
			}
			foreach (Enum T in Enums)
			{
				T.IsFiltering = !FilterAll;
				T.IsFilterMatch = false;
				if (FilterEnums == true) T.IsFilterMatch = true;
			}
			foreach (Namespace T in Namespaces)
				T.Filter(FilterAll, FilterNamespaces, FilterServices, FilterData, FilterEnums);
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
							if (Namespace.Name != null && Namespace.Name != "") if (Namespace.Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Namespace", Namespace.Name, this, this));
							if (Namespace.URI != null && Namespace.URI != "") if (Namespace.URI.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Namespace URI", Namespace.URI, this, this));
							if (BindingsNamespace != null && BindingsNamespace != "") if (BindingsNamespace.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Bindings Namespace", BindingsNamespace, this, this));
							if (SecurityClass != null && SecurityClass != "") if (SecurityClass.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Security Class", SecurityClass, this, this));
							if (HostsNamespace != null && HostsNamespace != "") if (HostsNamespace.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Hosts Namespace", HostsNamespace, this, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, this, this));
							if (Namespace.Name != null && Namespace.Name != "") if (Namespace.Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Namespace", Namespace.Name, this, this));
							if (Namespace.URI != null && Namespace.URI != "") if (Namespace.URI.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("ContractName", Namespace.URI, this, this));
							if (BindingsNamespace != null && BindingsNamespace != "") if (BindingsNamespace.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Bindings Namespace", BindingsNamespace, this, this));
							if (SecurityClass != null && SecurityClass != "") if (SecurityClass.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Security Class", SecurityClass, this, this));
							if (HostsNamespace != null && HostsNamespace != "") if (HostsNamespace.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Hosts Namespace", HostsNamespace, this, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, this, this));
						if (Namespace.Name != null && Namespace.Name != "") if (Args.RegexSearch.IsMatch(Namespace.Name)) results.Add(new FindReplaceResult("Namespace", Namespace.Name, this, this));
						if (Namespace.URI != null && Namespace.URI != "") if (Args.RegexSearch.IsMatch(Namespace.URI)) results.Add(new FindReplaceResult("Namespace URI", Namespace.URI, this, this));
						if (BindingsNamespace != null && BindingsNamespace != "") if (Args.RegexSearch.IsMatch(BindingsNamespace)) results.Add(new FindReplaceResult("Bindings Namespace", BindingsNamespace, this, this));
						if (SecurityClass != null && SecurityClass != "") if (Args.RegexSearch.IsMatch(SecurityClass)) results.Add(new FindReplaceResult("Security Class", SecurityClass, this, this));
						if (HostsNamespace != null && HostsNamespace != "") if (Args.RegexSearch.IsMatch(HostsNamespace)) results.Add(new FindReplaceResult("Hosts Namespace", HostsNamespace, this, this));
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
								if (Namespace.Name != null && Namespace.Name != "") Namespace.Name = Microsoft.VisualBasic.Strings.Replace(Namespace.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (Namespace.URI != null && Namespace.URI != "") Namespace.URI = Microsoft.VisualBasic.Strings.Replace(Namespace.URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (BindingsNamespace != null && BindingsNamespace != "") BindingsNamespace = Microsoft.VisualBasic.Strings.Replace(BindingsNamespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (SecurityClass != null && SecurityClass != "") SecurityClass = Microsoft.VisualBasic.Strings.Replace(SecurityClass, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (HostsNamespace != null && HostsNamespace != "") HostsNamespace = Microsoft.VisualBasic.Strings.Replace(HostsNamespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (Namespace.Name != null && Namespace.Name != "") Namespace.Name = Microsoft.VisualBasic.Strings.Replace(Namespace.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (Namespace.URI != null && Namespace.URI != "") Namespace.URI = Microsoft.VisualBasic.Strings.Replace(Namespace.URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (BindingsNamespace != null && BindingsNamespace != "") BindingsNamespace = Microsoft.VisualBasic.Strings.Replace(BindingsNamespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (SecurityClass != null && SecurityClass != "") SecurityClass = Microsoft.VisualBasic.Strings.Replace(SecurityClass, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (HostsNamespace != null && HostsNamespace != "") HostsNamespace = Microsoft.VisualBasic.Strings.Replace(HostsNamespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (Namespace.Name != null && Namespace.Name != "") Namespace.Name = Args.RegexSearch.Replace(Namespace.Name, Args.Replace);
							if (Namespace.URI != null && Namespace.URI != "") Namespace.URI = Args.RegexSearch.Replace(Namespace.URI, Args.Replace);
							if (BindingsNamespace != null && BindingsNamespace != "") BindingsNamespace = Args.RegexSearch.Replace(BindingsNamespace, Args.Replace);
							if (SecurityClass != null && SecurityClass != "") SecurityClass = Args.RegexSearch.Replace(SecurityClass, Args.Replace);
							if (HostsNamespace != null && HostsNamespace != "") HostsNamespace = Args.RegexSearch.Replace(HostsNamespace, Args.Replace);
						}
					}
					IsActive = ia;
				}
			}

			foreach (Service S in Services)
				results.AddRange(S.FindReplace(Args));

			foreach (Data D in Data)
				results.AddRange(D.FindReplace(Args));

			foreach (Enum E in Enums)
				results.AddRange(E.FindReplace(Args));

			foreach (Namespace N in Namespaces)
				results.AddRange(N.FindReplace(Args));

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
							if (Field == "Namespace") Namespace.Name = Microsoft.VisualBasic.Strings.Replace(Namespace.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Namespace URI") Namespace.URI = Microsoft.VisualBasic.Strings.Replace(Namespace.URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Bindings Namespace") BindingsNamespace = Microsoft.VisualBasic.Strings.Replace(BindingsNamespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Security Class") SecurityClass = Microsoft.VisualBasic.Strings.Replace(SecurityClass, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Hosts Namespace") HostsNamespace = Microsoft.VisualBasic.Strings.Replace(HostsNamespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Namespace") Namespace.Name = Microsoft.VisualBasic.Strings.Replace(Namespace.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Namespace URI") Namespace.URI = Microsoft.VisualBasic.Strings.Replace(Namespace.URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Bindings Namespace") BindingsNamespace = Microsoft.VisualBasic.Strings.Replace(BindingsNamespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Security Class") SecurityClass = Microsoft.VisualBasic.Strings.Replace(SecurityClass, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Hosts Namespace") HostsNamespace = Microsoft.VisualBasic.Strings.Replace(HostsNamespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (Field == "Namespace") Namespace.Name = Args.RegexSearch.Replace(Namespace.Name, Args.Replace);
						if (Field == "Namespace URI") Namespace.URI = Args.RegexSearch.Replace(Namespace.URI, Args.Replace);
						if (Field == "Bindings Namespace") BindingsNamespace = Args.RegexSearch.Replace(BindingsNamespace, Args.Replace);
						if (Field == "Security Class") SecurityClass = Args.RegexSearch.Replace(SecurityClass, Args.Replace);
						if (Field == "Hosts Namespace") HostsNamespace = Args.RegexSearch.Replace(HostsNamespace, Args.Replace);
					}
				}
				IsActive = ia;
			}
		}

		public bool ProjectHasServices()
		{
			foreach (Namespace N in Namespaces)
				if (N.HasServices() == true) return true;
			if (Services.Count > 0) return true;	 //Hack for root namespace

			return false;
		}

		public virtual void Open()
		{
			//Check the File Version property to see if it has changed and update the reference data accordingly. This has no effect on the build process and is merely a courtesy to the user.
			foreach (Reference R in References)
				R.UpdateVersion();

			//Verify that we have a NamespaceObject and if not make one
			if (Namespace == null)
			{
				Namespace = new Namespace(NamespaceName, null, this);
				Namespace.URI = "http://tempuri.org/" + NamespaceName.Replace(".", "/") + "/";
			}
			Namespace.SetProjectRoot();
			Namespace.Owner = this;

			if (IsOpen == true)
				Globals.MainScreen.OpenProjectItem(this);

			foreach (ServiceBinding SB in Bindings)
				SB.Parent = this;

			foreach (BindingSecurity BS in Security)
				BS.Parent = this;

			foreach(Host H in Hosts)
			{
				H.Parent = this;
				H.Credentials.Owner = H;
				foreach (HostBehavior HB in H.Behaviors)
					HB.Parent = H;
				foreach (HostEndpoint HE in H.Endpoints)
					HE.Parent = H;
			}

			foreach (Enum E in Enums)
			{
				if (E.IsOpen == true)
					Globals.MainScreen.OpenProjectItem(E);

				//Set owners
				E.Parent = this.Namespace;
				foreach (EnumElement EE in E.Elements)
					EE.Owner = E;
			}

			foreach (Data D in Data)
			{
				if (D.IsOpen == true)
					Globals.MainScreen.OpenProjectItem(D);

				//Set owners
				D.Parent = this.Namespace;
				foreach (DataElement DE in D.Elements)
					DE.Owner = D;
			}

			foreach (Service S in Services)
			{
				if (S.IsOpen == true)
					Globals.MainScreen.OpenProjectItem(S);

				//Set owners
				S.Parent = this.Namespace;
				foreach (Operation O in S.Operations)
				{
					O.Owner = S;
					foreach (OperationParameter OP in O.Parameters)
						OP.Owner = S;
				}
				foreach (Property P in S.Properties)
					P.Owner = S;
			}

			foreach (Namespace N in Namespaces)
			{
				N.Parent = this.Namespace;
				N.OpenItems();
			}

			Compiler = new Compiler.Compiler(this);
			Globals.MainScreen.OpenProjectItem(Compiler);
		}

		public void PreBuild(bool IsOutputBuild)
		{
			if (IsDependencyProject == false)
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
			}

			foreach (Reference R in References)
			{
				R.IsGACInstalled = Helpers.GACUtil.IsAssemblyInGAC(R.Name);
			}
		}

		public bool VerifyCode()
		{
			bool NoErrors = true;

			if (IsDependencyProject == true)
			{
				ServerAssemblyName = Helpers.RegExs.ReplaceSpaces.Replace(Name, ".") + ".Server";
				ClientAssemblyName = Helpers.RegExs.ReplaceSpaces.Replace(Name, ".") + ".Client";
			}

			if (Bindings.Count > 0 && Security.Count > 0)
				if ((BindingsNamespace == "" || BindingsNamespace == null))
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS0000", "The '" + Name + "' project does not have a Bindings Namespace. You must specify a namespace for the bindings.", WCFArchitect.Compiler.CompileMessageSeverity.Error, null, this, this.GetType()));
					NoErrors = false;
				}
			if (Security.Count > 0)
				if (SecurityClass == "" || SecurityClass == null)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS0001", "The '" + Name + "' project does not have a Security Class Name specified. You must specify a class name for the security elements.", WCFArchitect.Compiler.CompileMessageSeverity.Error, null, this, this.GetType()));
					NoErrors = false;
				}

			if (IsDependencyProject == false)
			{
				if (Hosts.Count > 0)
					if ((HostsNamespace == "" || HostsNamespace == null))
					{
						Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS0002", "The '" + Name + "' project does not have a Hosts Namespace. You must specify a namespace for the hosts.", WCFArchitect.Compiler.CompileMessageSeverity.Error, null, this, this.GetType()));
						NoErrors = false;
					}
				if ((ServerAssemblyName == "" || ServerAssemblyName == null))
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS0003", "The '" + Name + "' project does not have a Server Assembly Name. You must specify a Server Assembly Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, null, this, this.GetType()));
					NoErrors = false;
				}
				else
					if (Helpers.RegExs.MatchFileName.IsMatch(ServerAssemblyName) == false)
					{
						Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS0004", "The Server Assembly Name in '" + Name + "' project is not set or contains invalid characters. You must specify a valid Windows file name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, null, this, this.GetType()));
						NoErrors = false;
					}
				if ((ClientAssemblyName == "" || ClientAssemblyName == null))
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS0005", "The '" + Name + "' project does not have a Client Assembly Name. You must specify a Client Assembly Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, null, this, this.GetType()));
					NoErrors = false;
				}
				else
					if (Helpers.RegExs.MatchFileName.IsMatch(ClientAssemblyName) == false)
					{
						Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS0006", "The Client Assembly Name in '" + Name + "' project is not set or contains invalid characters. You must specify a valid Windows file name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, null, this, this.GetType()));
						NoErrors = false;
					}
				if ((ServerAssemblyName == ClientAssemblyName))
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS0007", "The '" + Name + "' project Client and Server Assembly Names are the same. You must specify a different Server or Client Assembly Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, null, this, this.GetType()));
					NoErrors = false;
				}
			}

			if (Namespace.URI == null || Namespace.URI == "")
			{
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS1000", "The '" + Namespace.Name + "' namespace in the '" + Name + "' project does not have a URI. Namespaces MUST have a URI.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, this, this.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchHTTPURI.IsMatch(Namespace.URI) == false)
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS1001", "The URI '" + Namespace.URI + "' for the '" + Namespace.Name + "' namespace in the '" + Name + "' project is not a valid URI. Any associated services and data may not function properly.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, this, this, this.GetType()));

			foreach (Enum EE in Enums)
				if (EE.VerifyCode(Compiler) == false) NoErrors = false;

			foreach (Data DE in Data)
				if (DE.VerifyCode(Compiler) == false) NoErrors = false;

			foreach (Service SE in Services)
				if (SE.VerifyCode(Compiler) == false) NoErrors = false;

			foreach (Namespace NS in Namespaces)
				if (NS.VerifyCode(Compiler) == false) NoErrors = false;

			foreach (ServiceBinding SB in Bindings)
				if (SB.VerifyCode(Compiler) == false) NoErrors = false;

			foreach (BindingSecurity BS in Security)
				if (BS.VerifyCode(Compiler) == false) NoErrors = false;

			foreach (Host HE in Hosts)
				if (HE.VerifyCode(Compiler) == false) NoErrors = false;

			return NoErrors;
		}

		public virtual string GenerateServerCode30(bool GenerateAssemblyCode, string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET30) + ServerAssemblyName + ".NET30.cs";

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

			if (GenerateAssemblyCode == true)
			{
				Code.AppendLine("//Assembly Information");
				Code.AppendFormat("[assembly: AssemblyTitle(\"{0}\")]{1}", AssemblyTitle, Environment.NewLine); ;
				Code.AppendFormat("[assembly: AssemblyDescription(\"{0}\")]{1}", AssemblyDescription, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCompany(\"{0}\")]{1}", AssemblyCompany, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyProduct(\"{0}\")]{1}", AssemblyProduct, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCopyright(\"{0}\")]{1}", AssemblyCopyright, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyTrademark(\"{0}\")]{1}", AssemblyTrademark, Environment.NewLine);
				Code.AppendFormat("[assembly: ComVisible({0})]{1}", AssemblyCOMVisible ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendLine("[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]");
				Code.AppendFormat("[assembly: AssemblyVersion(\"{0}\")]{1}", AssemblyVersion, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyFileVersion(\"{0}\")]{1}", AssemblyFileVersion, Environment.NewLine);
				if (AssemblyCOMVisible == true)
					Code.AppendFormat("[assembly: GuidAttribute(\"{0}\")]{1}", AssemblyCOMGuid.ToString().Replace("{", "").Replace("}", ""), Environment.NewLine);
				Code.AppendLine();
			}

			//Generate Root Namespace
			Code.AppendFormat("namespace {0}{1}", Namespace.Name, Environment.NewLine);
			Code.AppendLine("{");
			if (Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumerations");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Enum EE in Enums)
					Code.AppendLine(EE.GenerateServerCode30(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			if (Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in Data)
					Code.AppendLine(DE.GenerateServerCode30(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			if (Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in Services)
					Code.AppendLine(SE.GenerateServerCode30(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			Code.AppendLine("}");
			Code.AppendLine();

			foreach (Namespace NS in Namespaces)
				Code.AppendLine(NS.GenerateServerCode30(GenerateAssemblyCode, ProjectName));
			Code.AppendLine();

			if (Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				foreach (ServiceBinding SB in Bindings)
					Code.AppendLine(SB.GenerateServerCode30(GenerateAssemblyCode, ProjectName));
				Code.AppendLine("}");
				Code.AppendLine();
				Code.AppendLine();
			}

			if (Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				Code.AppendFormat("\t{0} partial class {1}{2}", Compiler.ServerClassVisibility(GenerateAssemblyCode, ProjectName), SecurityClass, Environment.NewLine);
				Code.AppendLine("\t{");
				foreach (BindingSecurity BS in Security)
					Code.AppendLine(BS.GenerateServerCode30());
				Code.AppendLine("\t}");
				Code.AppendLine("}");
				Code.AppendLine();
				Code.AppendLine();
			}

			if (Hosts.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Hosts");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, HostsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				foreach (Host HE in Hosts)
					Code.AppendLine(HE.GenerateServerCode30(GenerateAssemblyCode, ProjectName));
				Code.AppendLine("}");
			}

			if (GenerateAssemblyCode == false) System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateServerCode35(bool GenerateAssemblyCode, string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET35) + ServerAssemblyName + ".NET35.cs";

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

			if (GenerateAssemblyCode == true)
			{
				Code.AppendLine("//Assembly Information");
				Code.AppendFormat("[assembly: AssemblyTitle(\"{0}\")]{1}", AssemblyTitle, Environment.NewLine); ;
				Code.AppendFormat("[assembly: AssemblyDescription(\"{0}\")]{1}", AssemblyDescription, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCompany(\"{0}\")]{1}", AssemblyCompany, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyProduct(\"{0}\")]{1}", AssemblyProduct, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCopyright(\"{0}\")]{1}", AssemblyCopyright, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyTrademark(\"{0}\")]{1}", AssemblyTrademark, Environment.NewLine);
				Code.AppendFormat("[assembly: ComVisible({0})]{1}", AssemblyCOMVisible ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendLine("[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]");
				Code.AppendFormat("[assembly: AssemblyVersion(\"{0}\")]{1}", AssemblyVersion, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyFileVersion(\"{0}\")]{1}", AssemblyFileVersion, Environment.NewLine);
				if (AssemblyCOMVisible == true)
					Code.AppendFormat("[assembly: GuidAttribute(\"{0}\")]{1}", AssemblyCOMGuid.ToString().Replace("{", "").Replace("}", ""), Environment.NewLine);
				Code.AppendLine();
			}

			//Generate Root Namespace
			Code.AppendFormat("namespace {0}{1}", Namespace.Name, Environment.NewLine);
			Code.AppendLine("{");
			if (Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumerations");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Enum EE in Enums)
					Code.AppendLine(EE.GenerateServerCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			if (Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in Data)
					Code.AppendLine(DE.GenerateServerCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			if (Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in Services)
					Code.AppendLine(SE.GenerateServerCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			Code.AppendLine("}");
			Code.AppendLine();

			foreach (Namespace NS in Namespaces)
				Code.AppendLine(NS.GenerateServerCode35(GenerateAssemblyCode, ProjectName));
			Code.AppendLine();

			if (Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				foreach (ServiceBinding SB in Bindings)
					Code.AppendLine(SB.GenerateServerCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine("}");
				Code.AppendLine();
				Code.AppendLine();
			}

			if (Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				Code.AppendFormat("\t{0} partial class {1}{2}", Compiler.ServerClassVisibility(GenerateAssemblyCode, ProjectName), SecurityClass, Environment.NewLine);
				Code.AppendLine("\t{");
				foreach (BindingSecurity BS in Security)
					Code.AppendLine(BS.GenerateServerCode35());
				Code.AppendLine("\t}");
				Code.AppendLine("}");
				Code.AppendLine();
				Code.AppendLine();
			}

			if (Hosts.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Hosts");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, HostsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				foreach (Host HE in Hosts)
					Code.AppendLine(HE.GenerateServerCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine("}");
			}

			if (GenerateAssemblyCode == false) System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateServerCode35Client(bool GenerateAssemblyCode, string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET35Client) + ServerAssemblyName + ".NET35Client.cs";

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

			if (GenerateAssemblyCode == true)
			{
				Code.AppendLine("//Assembly Information");
				Code.AppendFormat("[assembly: AssemblyTitle(\"{0}\")]{1}", AssemblyTitle, Environment.NewLine); ;
				Code.AppendFormat("[assembly: AssemblyDescription(\"{0}\")]{1}", AssemblyDescription, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCompany(\"{0}\")]{1}", AssemblyCompany, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyProduct(\"{0}\")]{1}", AssemblyProduct, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCopyright(\"{0}\")]{1}", AssemblyCopyright, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyTrademark(\"{0}\")]{1}", AssemblyTrademark, Environment.NewLine);
				Code.AppendFormat("[assembly: ComVisible({0})]{1}", AssemblyCOMVisible ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendLine("[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]");
				Code.AppendFormat("[assembly: AssemblyVersion(\"{0}\")]{1}", AssemblyVersion, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyFileVersion(\"{0}\")]{1}", AssemblyFileVersion, Environment.NewLine);
				if (AssemblyCOMVisible == true)
					Code.AppendFormat("[assembly: GuidAttribute(\"{0}\")]{1}", AssemblyCOMGuid.ToString().Replace("{", "").Replace("}", ""), Environment.NewLine);
				Code.AppendLine();
			}

			//Generate Root Namespace
			Code.AppendFormat("namespace {0}{1}", Namespace.Name, Environment.NewLine);
			Code.AppendLine("{");
			if (Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumerations");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Enum EE in Enums)
					Code.AppendLine(EE.GenerateServerCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			if (Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in Data)
					Code.AppendLine(DE.GenerateServerCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			if (Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in Services)
					Code.AppendLine(SE.GenerateServerCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			Code.AppendLine("}");
			Code.AppendLine();

			foreach (Namespace NS in Namespaces)
				Code.AppendLine(NS.GenerateServerCode35(GenerateAssemblyCode, ProjectName));
			Code.AppendLine();

			if (Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				foreach (ServiceBinding SB in Bindings)
					Code.AppendLine(SB.GenerateServerCode35Client(GenerateAssemblyCode, ProjectName));
				Code.AppendLine("}");
				Code.AppendLine();
				Code.AppendLine();
			}

			if (Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				Code.AppendFormat("\t{0} partial class {1}{2}", Compiler.ServerClassVisibility(GenerateAssemblyCode, ProjectName), SecurityClass, Environment.NewLine);
				Code.AppendLine("\t{");
				foreach (BindingSecurity BS in Security)
					Code.AppendLine(BS.GenerateServerCode35Client());
				Code.AppendLine("\t}");
				Code.AppendLine("}");
				Code.AppendLine();
				Code.AppendLine();
			}

			if (Hosts.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Hosts");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, HostsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				foreach (Host HE in Hosts)
					Code.AppendLine(HE.GenerateServerCode35Client(GenerateAssemblyCode, ProjectName));
				Code.AppendLine("}");
			}

			if (GenerateAssemblyCode == false) System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateServerCode40(bool GenerateAssemblyCode, string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET40) + ServerAssemblyName + ".NET40.cs";

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

			if (GenerateAssemblyCode == true)
			{
				Code.AppendLine("//Assembly Information");
				Code.AppendFormat("[assembly: AssemblyTitle(\"{0}\")]{1}", AssemblyTitle, Environment.NewLine); ;
				Code.AppendFormat("[assembly: AssemblyDescription(\"{0}\")]{1}", AssemblyDescription, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCompany(\"{0}\")]{1}", AssemblyCompany, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyProduct(\"{0}\")]{1}", AssemblyProduct, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCopyright(\"{0}\")]{1}", AssemblyCopyright, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyTrademark(\"{0}\")]{1}", AssemblyTrademark, Environment.NewLine);
				Code.AppendFormat("[assembly: ComVisible({0})]{1}", AssemblyCOMVisible ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendLine("[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]");
				Code.AppendFormat("[assembly: AssemblyVersion(\"{0}\")]{1}", AssemblyVersion, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyFileVersion(\"{0}\")]{1}", AssemblyFileVersion, Environment.NewLine);
				if (AssemblyCOMVisible == true)
					Code.AppendFormat("[assembly: GuidAttribute(\"{0}\")]{1}", AssemblyCOMGuid.ToString().Replace("{", "").Replace("}", ""), Environment.NewLine);
				Code.AppendLine();
			}

			//Generate Root Namespace
			Code.AppendFormat("namespace {0}{1}", Namespace.Name, Environment.NewLine);
			Code.AppendLine("{");
			if (Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumerations");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Enum EE in Enums)
					Code.AppendLine(EE.GenerateServerCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			if (Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in Data)
					Code.AppendLine(DE.GenerateServerCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			if (Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in Services)
					Code.Append(SE.GenerateServerCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			Code.AppendLine("}");
			Code.AppendLine();

			foreach (Namespace NS in Namespaces)
				Code.AppendLine(NS.GenerateServerCode40(GenerateAssemblyCode, ProjectName));
			Code.AppendLine();

			if (Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				foreach (ServiceBinding SB in Bindings)
					Code.AppendLine(SB.GenerateServerCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine("}");
				Code.AppendLine();
				Code.AppendLine();
			}

			if (Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				Code.AppendFormat("\t{0} partial class {1}{2}", Compiler.ServerClassVisibility(GenerateAssemblyCode, ProjectName), SecurityClass, Environment.NewLine);
				Code.AppendLine("\t{");
				foreach (BindingSecurity BS in Security)
					Code.AppendLine(BS.GenerateServerCode40());
				Code.AppendLine("\t}");
				Code.AppendLine("}");
				Code.AppendLine();
				Code.AppendLine();
			}

			if (Hosts.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Hosts");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, HostsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				foreach (Host HE in Hosts)
					Code.AppendLine(HE.GenerateServerCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine("}");
			}

			if (GenerateAssemblyCode == false) System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateServerCode40Client(bool GenerateAssemblyCode, string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET40Client) + ServerAssemblyName + ".NET40Client.cs";

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

			if (GenerateAssemblyCode == true)
			{
				Code.AppendLine("//Assembly Information");
				Code.AppendFormat("[assembly: AssemblyTitle(\"{0}\")]{1}", AssemblyTitle, Environment.NewLine); ;
				Code.AppendFormat("[assembly: AssemblyDescription(\"{0}\")]{1}", AssemblyDescription, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCompany(\"{0}\")]{1}", AssemblyCompany, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyProduct(\"{0}\")]{1}", AssemblyProduct, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCopyright(\"{0}\")]{1}", AssemblyCopyright, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyTrademark(\"{0}\")]{1}", AssemblyTrademark, Environment.NewLine);
				Code.AppendFormat("[assembly: ComVisible({0})]{1}", AssemblyCOMVisible ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendLine("[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]");
				Code.AppendFormat("[assembly: AssemblyVersion(\"{0}\")]{1}", AssemblyVersion, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyFileVersion(\"{0}\")]{1}", AssemblyFileVersion, Environment.NewLine);
				if (AssemblyCOMVisible == true)
					Code.AppendFormat("[assembly: GuidAttribute(\"{0}\")]{1}", AssemblyCOMGuid.ToString().Replace("{", "").Replace("}", ""), Environment.NewLine);
				Code.AppendLine();
			}

			//Generate Root Namespace
			Code.AppendFormat("namespace {0}{1}", Namespace.Name, Environment.NewLine);
			Code.AppendLine("{");
			if (Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumerations");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Enum EE in Enums)
					Code.AppendLine(EE.GenerateServerCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			if (Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in Data)
					Code.AppendLine(DE.GenerateServerCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			if (Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in Services)
					Code.AppendLine(SE.GenerateServerCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			Code.AppendLine("}");
			Code.AppendLine();

			foreach (Namespace NS in Namespaces)
				Code.AppendLine(NS.GenerateServerCode40(GenerateAssemblyCode, ProjectName));
			Code.AppendLine();

			if (Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				foreach (ServiceBinding SB in Bindings)
					Code.AppendLine(SB.GenerateServerCode40Client(GenerateAssemblyCode, ProjectName));
				Code.AppendLine("}");
				Code.AppendLine();
				Code.AppendLine();
			}

			if (Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				Code.AppendFormat("\t{0} partial class {1}{2}", Compiler.ServerClassVisibility(GenerateAssemblyCode, ProjectName), SecurityClass, Environment.NewLine);
				Code.AppendLine("\t{");
				foreach (BindingSecurity BS in Security)
					Code.AppendLine(BS.GenerateServerCode40Client());
				Code.AppendLine("\t}");
				Code.AppendLine("}");
				Code.AppendLine();
				Code.AppendLine();
			}

			if (Hosts.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Hosts");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, HostsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				foreach (Host HE in Hosts)
					Code.AppendLine(HE.GenerateServerCode40Client(GenerateAssemblyCode, ProjectName));
				Code.AppendLine("}");
			}

			if (GenerateAssemblyCode == false) System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateClientCode30(bool GenerateAssemblyCode, string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET30) + ClientAssemblyName + ".NET30.cs";

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

			if (GenerateAssemblyCode == true)
			{
				Code.AppendLine("//Assembly Information");
				Code.AppendFormat("[assembly: AssemblyTitle(\"{0}\")]{1}", AssemblyTitle, Environment.NewLine); ;
				Code.AppendFormat("[assembly: AssemblyDescription(\"{0}\")]{1}", AssemblyDescription, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCompany(\"{0}\")]{1}", AssemblyCompany, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyProduct(\"{0}\")]{1}", AssemblyProduct, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCopyright(\"{0}\")]{1}", AssemblyCopyright, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyTrademark(\"{0}\")]{1}", AssemblyTrademark, Environment.NewLine);
				Code.AppendFormat("[assembly: ComVisible({0})]{1}", AssemblyCOMVisible ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendLine("[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]");
				Code.AppendFormat("[assembly: AssemblyVersion(\"{0}\")]{1}", AssemblyVersion, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyFileVersion(\"{0}\")]{1}", AssemblyFileVersion, Environment.NewLine);
				if (AssemblyCOMVisible == true)
					Code.AppendFormat("[assembly: GuidAttribute(\"{0}\")]{1}", AssemblyCOMGuid.ToString().Replace("{", "").Replace("}", ""), Environment.NewLine);
				Code.AppendLine();
			}

			//Generate Root Namespace
			Code.AppendFormat("namespace {0}{1}", Namespace.Name, Environment.NewLine);
			Code.AppendLine("{");
			if (Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in Data)
					Code.AppendLine(DE.GenerateClientCode30(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			if (Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in Services)
					Code.AppendLine(SE.GenerateClientCode30(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			Code.AppendLine("}");
			Code.AppendLine();

			foreach (Namespace NS in Namespaces)
				Code.AppendLine(NS.GenerateClientCode30(GenerateAssemblyCode, ProjectName));
			Code.AppendLine();

			if (Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				foreach (ServiceBinding SB in Bindings)
					Code.AppendLine(SB.GenerateClientCode30(GenerateAssemblyCode, ProjectName));
				Code.AppendLine("}");
				Code.AppendLine();
				Code.AppendLine();
			}

			if (Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				Code.AppendFormat("\t{0} partial class {1}{2}", Compiler.ClientClassVisibility(GenerateAssemblyCode, ProjectName), SecurityClass, Environment.NewLine);
				Code.AppendLine("\t{");
				foreach (BindingSecurity BS in Security)
					Code.AppendLine(BS.GenerateClientCode30());
				Code.AppendLine("\t}");
				Code.AppendLine("}");
			}

			if (GenerateAssemblyCode == false) System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateClientCode35(bool GenerateAssemblyCode, string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET35) + ClientAssemblyName + ".NET35.cs";

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

			if (GenerateAssemblyCode == true)
			{
				Code.AppendLine("//Assembly Information");
				Code.AppendFormat("[assembly: AssemblyTitle(\"{0}\")]{1}", AssemblyTitle, Environment.NewLine); ;
				Code.AppendFormat("[assembly: AssemblyDescription(\"{0}\")]{1}", AssemblyDescription, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCompany(\"{0}\")]{1}", AssemblyCompany, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyProduct(\"{0}\")]{1}", AssemblyProduct, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCopyright(\"{0}\")]{1}", AssemblyCopyright, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyTrademark(\"{0}\")]{1}", AssemblyTrademark, Environment.NewLine);
				Code.AppendFormat("[assembly: ComVisible({0})]{1}", AssemblyCOMVisible ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendLine("[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]");
				Code.AppendFormat("[assembly: AssemblyVersion(\"{0}\")]{1}", AssemblyVersion, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyFileVersion(\"{0}\")]{1}", AssemblyFileVersion, Environment.NewLine);
				if (AssemblyCOMVisible == true)
					Code.AppendFormat("[assembly: GuidAttribute(\"{0}\")]{1}", AssemblyCOMGuid.ToString().Replace("{", "").Replace("}", ""), Environment.NewLine);
				Code.AppendLine();
			}

			//Generate Root Namespace
			Code.AppendFormat("namespace {0}{1}", Namespace.Name, Environment.NewLine);
			Code.AppendLine("{");
			if (Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in Data)
					Code.AppendLine(DE.GenerateClientCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			if (Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in Services)
					Code.AppendLine(SE.GenerateClientCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			Code.AppendLine("}");
			Code.AppendLine();

			foreach (Namespace NS in Namespaces)
				Code.AppendLine(NS.GenerateClientCode35(GenerateAssemblyCode, ProjectName));
			Code.AppendLine();

			if (Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				foreach (ServiceBinding SB in Bindings)
					Code.AppendLine(SB.GenerateClientCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine("}");
				Code.AppendLine();
				Code.AppendLine();
			}

			if (Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				Code.AppendFormat("\t{0} partial class {1}{2}", Compiler.ClientClassVisibility(GenerateAssemblyCode, ProjectName), SecurityClass, Environment.NewLine);
				Code.AppendLine("\t{");
				foreach (BindingSecurity BS in Security)
					Code.AppendLine(BS.GenerateClientCode35());
				Code.AppendLine("\t}");
				Code.AppendLine("}");
			}

			if (GenerateAssemblyCode == false) System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateClientCode35Client(bool GenerateAssemblyCode, string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET35Client) + ClientAssemblyName + ".NET35Client.cs";

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

			if (GenerateAssemblyCode == true)
			{
				Code.AppendLine("//Assembly Information");
				Code.AppendFormat("[assembly: AssemblyTitle(\"{0}\")]{1}", AssemblyTitle, Environment.NewLine); ;
				Code.AppendFormat("[assembly: AssemblyDescription(\"{0}\")]{1}", AssemblyDescription, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCompany(\"{0}\")]{1}", AssemblyCompany, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyProduct(\"{0}\")]{1}", AssemblyProduct, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCopyright(\"{0}\")]{1}", AssemblyCopyright, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyTrademark(\"{0}\")]{1}", AssemblyTrademark, Environment.NewLine);
				Code.AppendFormat("[assembly: ComVisible({0})]{1}", AssemblyCOMVisible ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendLine("[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]");
				Code.AppendFormat("[assembly: AssemblyVersion(\"{0}\")]{1}", AssemblyVersion, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyFileVersion(\"{0}\")]{1}", AssemblyFileVersion, Environment.NewLine);
				if (AssemblyCOMVisible == true)
					Code.AppendFormat("[assembly: GuidAttribute(\"{0}\")]{1}", AssemblyCOMGuid.ToString().Replace("{", "").Replace("}", ""), Environment.NewLine);
				Code.AppendLine();
			}

			//Generate Root Namespace
			Code.AppendFormat("namespace {0}{1}", Namespace.Name, Environment.NewLine);
			Code.AppendLine("{");
			if (Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in Data)
					Code.AppendLine(DE.GenerateClientCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			if (Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in Services)
					Code.AppendLine(SE.GenerateClientCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			Code.AppendLine("}");
			Code.AppendLine();

			foreach (Namespace NS in Namespaces)
				Code.AppendLine(NS.GenerateClientCode35(GenerateAssemblyCode, ProjectName));
			Code.AppendLine();

			if (Services.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				foreach (ServiceBinding SB in Bindings)
					Code.AppendLine(SB.GenerateClientCode35Client(GenerateAssemblyCode, ProjectName));
				Code.AppendLine("}");
				Code.AppendLine();
				Code.AppendLine();
			}

			if (Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				Code.AppendFormat("\t{0} partial class {1}{2}", Compiler.ClientClassVisibility(GenerateAssemblyCode, ProjectName), SecurityClass, Environment.NewLine);
				Code.AppendLine("\t{");
				foreach (BindingSecurity BS in Security)
					Code.AppendLine(BS.GenerateClientCode35Client());
				Code.AppendLine("\t}");
				Code.AppendLine("}");
			}

			if (GenerateAssemblyCode == false) System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateClientCode40(bool GenerateAssemblyCode, string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET40) + ClientAssemblyName + ".NET40.cs";

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

			if (GenerateAssemblyCode == true)
			{
				Code.AppendLine("//Assembly Information");
				Code.AppendFormat("[assembly: AssemblyTitle(\"{0}\")]{1}", AssemblyTitle, Environment.NewLine); ;
				Code.AppendFormat("[assembly: AssemblyDescription(\"{0}\")]{1}", AssemblyDescription, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCompany(\"{0}\")]{1}", AssemblyCompany, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyProduct(\"{0}\")]{1}", AssemblyProduct, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCopyright(\"{0}\")]{1}", AssemblyCopyright, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyTrademark(\"{0}\")]{1}", AssemblyTrademark, Environment.NewLine);
				Code.AppendFormat("[assembly: ComVisible({0})]{1}", AssemblyCOMVisible ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendLine("[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]");
				Code.AppendFormat("[assembly: AssemblyVersion(\"{0}\")]{1}", AssemblyVersion, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyFileVersion(\"{0}\")]{1}", AssemblyFileVersion, Environment.NewLine);
				if (AssemblyCOMVisible == true)
					Code.AppendFormat("[assembly: GuidAttribute(\"{0}\")]{1}", AssemblyCOMGuid.ToString().Replace("{", "").Replace("}", ""), Environment.NewLine);
				Code.AppendLine();
			}

			//Generate Root Namespace
			Code.AppendFormat("namespace {0}{1}", Namespace.Name, Environment.NewLine);
			Code.AppendLine("{");
			if (Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in Data)
					Code.AppendLine(DE.GenerateClientCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			if (Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in Services)
					Code.AppendLine(SE.GenerateClientCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			Code.AppendLine("}");
			Code.AppendLine();

			foreach (Namespace NS in Namespaces)
				Code.AppendLine(NS.GenerateClientCode40(GenerateAssemblyCode, ProjectName));
			Code.AppendLine();

			if (Services.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				foreach (ServiceBinding SB in Bindings)
					Code.AppendLine(SB.GenerateClientCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine("}");
				Code.AppendLine();
				Code.AppendLine();
			}

			if (Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				Code.AppendFormat("\t{0} partial class {1}{2}", Compiler.ClientClassVisibility(GenerateAssemblyCode, ProjectName), SecurityClass, Environment.NewLine);
				Code.AppendLine("\t{");
				foreach (BindingSecurity BS in Security)
					Code.AppendLine(BS.GenerateClientCode40Client());
				Code.AppendLine("\t}");
				Code.AppendLine("}");
			}

			if (GenerateAssemblyCode == false) System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public virtual string GenerateClientCode40Client(bool GenerateAssemblyCode ,string ProjectName)
		{
			string CodeFilePath = Compiler.GetBuildOutputPath(ProjectName, ProjectOutputFramework.NET40Client) + ClientAssemblyName + ".NET40Client.cs";

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

			if (GenerateAssemblyCode == true)
			{
				Code.AppendLine("//Assembly Information");
				Code.AppendFormat("[assembly: AssemblyTitle(\"{0}\")]{1}", AssemblyTitle, Environment.NewLine); ;
				Code.AppendFormat("[assembly: AssemblyDescription(\"{0}\")]{1}", AssemblyDescription, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCompany(\"{0}\")]{1}", AssemblyCompany, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyProduct(\"{0}\")]{1}", AssemblyProduct, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyCopyright(\"{0}\")]{1}", AssemblyCopyright, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyTrademark(\"{0}\")]{1}", AssemblyTrademark, Environment.NewLine);
				Code.AppendFormat("[assembly: ComVisible({0})]{1}", AssemblyCOMVisible ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendLine("[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]");
				Code.AppendFormat("[assembly: AssemblyVersion(\"{0}\")]{1}", AssemblyVersion, Environment.NewLine);
				Code.AppendFormat("[assembly: AssemblyFileVersion(\"{0}\")]{1}", AssemblyFileVersion, Environment.NewLine);
				if (AssemblyCOMVisible == true)
					Code.AppendFormat("[assembly: GuidAttribute(\"{0}\")]{1}", AssemblyCOMGuid.ToString().Replace("{", "").Replace("}", ""), Environment.NewLine);
				Code.AppendLine();
			}

			//Generate Root Namespace
			Code.AppendFormat("namespace {0}{1}", Namespace.Name, Environment.NewLine);
			Code.AppendLine("{");
			if (Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in Data)
					Code.AppendLine(DE.GenerateClientCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			if (Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in Services)
					Code.AppendLine(SE.GenerateClientCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}
			Code.AppendLine("}");
			Code.AppendLine();

			foreach (Namespace NS in Namespaces)
				Code.AppendLine(NS.GenerateClientCode40(GenerateAssemblyCode, ProjectName));
			Code.AppendLine();

			if (Services.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				foreach (ServiceBinding SB in Bindings)
					Code.AppendLine(SB.GenerateClientCode40Client(GenerateAssemblyCode, ProjectName));
				Code.AppendLine("}");
				Code.AppendLine();
				Code.AppendLine();
			}

			if (Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				Code.AppendFormat("namespace {0}.{1}{2}", Namespace.Name, BindingsNamespace, Environment.NewLine);
				Code.AppendLine("{");
				Code.AppendFormat("\t{0} partial class {1}{2}", Compiler.ClientClassVisibility(GenerateAssemblyCode, ProjectName), SecurityClass, Environment.NewLine);
				Code.AppendLine("\t{");
				foreach (BindingSecurity BS in Security)
					Code.AppendLine(BS.GenerateClientCode40Client());
				Code.AppendLine("\t}");
				Code.AppendLine("}");
			}

			if (GenerateAssemblyCode == false) System.IO.File.WriteAllText(CodeFilePath, Code.ToString());

			return Code.ToString();
		}

		public string GenerateClientProxyArgs(bool GenerateAssemblyCode, bool ClientInternalClasses)
		{
			StringBuilder CodeArgs = new StringBuilder();
			Compiler.GenerateClientProxyNamespaces(CodeArgs, Namespace);
			foreach (Projects.Namespace N in Namespaces)
				Compiler.GenerateClientProxyNamespaces(CodeArgs, N);
			if (ServiceAsyncMethods == true) CodeArgs.Append(" /async");
			CodeArgs.Append(" /nologo");
			if (ServiceEnableDataBinding == true) CodeArgs.Append(" /edb");
			if (ServiceImportXMLTypes == true) CodeArgs.Append(" /importXmlTypes");
			if (GenerateAssemblyCode == false) if (ClientInternalClasses == true) CodeArgs.Append(" /i");
			if (ServiceGenerateMessageContracts == true) CodeArgs.Append(" /mc");
			if (ServiceSerializable == true) CodeArgs.Append(" /s");
			if (ServiceSerializer == Projects.ProjectServiceSerializerType.DataContract) CodeArgs.Append(" /ser:DataContractSerializer");
			if (ServiceSerializer == Projects.ProjectServiceSerializerType.XML) CodeArgs.Append(" /ser:XmlSerializer");
			if (ServiceWrapped == true) CodeArgs.Append(" /wrapped");
			if (ServiceCollectionType == ProjectCollectionType.List) CodeArgs.Append(" /ct:System.Collections.Generic.List`1");
			if (ServiceCollectionType == ProjectCollectionType.SynchronizedCollection) CodeArgs.Append(" /ct:System.Collections.Generic.SynchronizedCollection`1");
			if (ServiceCollectionType == ProjectCollectionType.Collection) CodeArgs.Append(" /ct:System.Collections.ObjectModel.Collection`1");
			if (ServiceCollectionType == ProjectCollectionType.ObservableCollection) CodeArgs.Append(" /ct:System.Collections.ObjectModel.ObservableCollection`1");
			if (ServiceCollectionType == ProjectCollectionType.BindingList) CodeArgs.Append(" /ct:System.ComponentModel.BindingList`1");
			if (ServiceCollectionTypes.Count > 0)
				foreach (string CT in ServiceCollectionTypes)
					CodeArgs.Append(" /ct:" + CT);
			return CodeArgs.ToString();
		}
	}

	internal enum ProjectOutputFramework
	{
		NET30,
		NET35,
		NET35Client,
		NET40,
		NET40Client,
		SL30,
		SL40,
		SL50,
	}

	internal enum ProjectOutputPlatform
	{
		x86,
		x64,
		Itanium,
		AnyCPU,
	}

	internal enum ProjectOutputConfiguration
	{
		Debug,
		Release,
	}

	internal class ProjectOutputPath : DependencyObject
	{
		private Guid projectID;
		public Guid ProjectID { get { return projectID; } }
		private Guid id;
		public Guid ID { get { return id; } }
		private Guid userProfileID;
		public Guid UserProfileID { get { return userProfileID; } }

		public bool IsEnabled { get { return (bool)GetValue(IsEnabledProperty); } set { SetValue(IsEnabledProperty, value); } }
		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(ProjectOutputPath), new UIPropertyMetadata(true));

		public string Path { get { return (string)GetValue(PathProperty); } set { SetValue(PathProperty, value); } }
		public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(ProjectOutputPath));

		public ProjectOutputFramework Framework { get { return (ProjectOutputFramework)GetValue(FrameworkProperty); } set { SetValue(FrameworkProperty, value); } }
		public static readonly DependencyProperty FrameworkProperty = DependencyProperty.Register("Framework", typeof(ProjectOutputFramework), typeof(ProjectOutputPath));

		public ProjectOutputPlatform Platform { get { return (ProjectOutputPlatform)GetValue(PlatformProperty); } set { SetValue(PlatformProperty, value); } }
		public static readonly DependencyProperty PlatformProperty = DependencyProperty.Register("Platform", typeof(ProjectOutputPlatform), typeof(ProjectOutputPath));

		public ProjectOutputConfiguration Configuration { get { return (ProjectOutputConfiguration)GetValue(ConfigurationProperty); } set { SetValue(ConfigurationProperty, value); } }
		public static readonly DependencyProperty ConfigurationProperty = DependencyProperty.Register("Configuration", typeof(ProjectOutputConfiguration), typeof(ProjectOutputPath));

		public bool GenerateCode { get { return (bool)GetValue(GenerateCodeProperty); } set { SetValue(GenerateCodeProperty, value); } }
		public static readonly DependencyProperty GenerateCodeProperty = DependencyProperty.Register("GenerateCode", typeof(bool), typeof(ProjectOutputPath));

		public bool GenerateAssembly { get { return (bool)GetValue(GenerateAssemblyProperty); } set { SetValue(GenerateAssemblyProperty, value); } }
		public static readonly DependencyProperty GenerateAssemblyProperty = DependencyProperty.Register("GenerateAssembly", typeof(bool), typeof(ProjectOutputPath));

		public bool OutputXSDWSDL { get { return (bool)GetValue(OutputXSDWSDLProperty); } set { SetValue(OutputXSDWSDLProperty, value); } }
		public static readonly DependencyProperty OutputXSDWSDLProperty = DependencyProperty.Register("OutputXSDWSDL", typeof(bool), typeof(ProjectOutputPath), new UIPropertyMetadata(false));

		public bool IsDependencyOutputPath { get { return (bool)GetValue(IsDependencyOutputPathProperty); } set { SetValue(IsDependencyOutputPathProperty, value); } }
		public static readonly DependencyProperty IsDependencyOutputPathProperty = DependencyProperty.Register("IsDependencyOutputPath", typeof(bool), typeof(ProjectOutputPath));

		public ProjectOutputPath() { }

		public ProjectOutputPath(Guid ProjectID, string Path)
		{
			id = Guid.NewGuid();
			projectID = ProjectID;
			userProfileID = Globals.UserProfile.ID;
			this.Path = Path;
			this.Framework = ProjectOutputFramework.NET40;
			this.Platform = ProjectOutputPlatform.AnyCPU;
			this.Configuration = ProjectOutputConfiguration.Debug;
			this.GenerateCode = true;
		}

		//This provides upgrade path support from older project files. DO NOT REMOVE!
		public void SetOwnerProject(Project Project)
		{
			projectID = Project.ID;
		}
	}

	internal class DependencyProject : Project
	{
		public DependencyProject() : base() { }

		public DependencyProject(string Name) : base(Name)
		{
			isDependencyProject = true;
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