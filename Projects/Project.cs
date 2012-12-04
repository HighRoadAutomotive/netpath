using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Runtime.Serialization;
using System.Windows.Threading;

namespace WCFArchitect.Projects
{
	public enum ProjectServiceSerializerType
	{
		Auto,
		DataContract,
		XML,
	}

	public enum ProjectCollectionSerializationOverride
	{
		None,
		List,
		Array
	}

	public class ProjectUsingNamespace : DependencyObject
	{
		public Guid ID { get; set; }
		
		public string Namespace { get { return (string)GetValue(NamespaceProperty); } set { SetValue(NamespaceProperty, value); } }
		public static readonly DependencyProperty NamespaceProperty = DependencyProperty.Register("Namespace", typeof(string), typeof(ProjectUsingNamespace));

		public bool IsFullFrameworkOnly { get { return (bool)GetValue(IsFullFrameworkOnlyProperty); } set { SetValue(IsFullFrameworkOnlyProperty, value); } }
		public static readonly DependencyProperty IsFullFrameworkOnlyProperty = DependencyProperty.Register("IsFullFrameworkOnly", typeof(bool), typeof(ProjectUsingNamespace));

		public bool Server { get { return (bool)GetValue(ServerProperty); } set { SetValue(ServerProperty, value); } }
		public static readonly DependencyProperty ServerProperty = DependencyProperty.Register("Server", typeof(bool), typeof(ProjectUsingNamespace));

		public bool Client { get { return (bool)GetValue(ClientProperty); } set { SetValue(ClientProperty, value); } }
		public static readonly DependencyProperty ClientProperty = DependencyProperty.Register("Client", typeof(bool), typeof(ProjectUsingNamespace));

		public bool NET { get { return (bool)GetValue(NETProperty); } set { SetValue(NETProperty, value); } }
		public static readonly DependencyProperty NETProperty = DependencyProperty.Register("NET", typeof(bool), typeof(ProjectUsingNamespace));

		public bool SL { get { return (bool)GetValue(SLProperty); } set { SetValue(SLProperty, value); } }
		public static readonly DependencyProperty SLProperty = DependencyProperty.Register("SL", typeof(bool), typeof(ProjectUsingNamespace));

		public bool RT { get { return (bool)GetValue(RTProperty); } set { SetValue(RTProperty, value); } }
		public static readonly DependencyProperty RTProperty = DependencyProperty.Register("RT", typeof(bool), typeof(ProjectUsingNamespace));

		public ProjectUsingNamespace()
		{
		}

		public ProjectUsingNamespace(string Namespace)
		{
			ID = Guid.NewGuid();
			this.Namespace = Namespace;
			IsFullFrameworkOnly = false;
			Server = true;
			Client = true;
			NET = true;
			SL = false;
			RT = false;
		}

		public ProjectUsingNamespace(string Namespace, bool Server, bool Client, bool NET, bool SL, bool RT)
		{
			ID = Guid.NewGuid();
			this.Namespace = Namespace;
			IsFullFrameworkOnly = false;
			this.Server = Server;
			this.Client = Client;
			this.NET = NET;
			this.SL = SL;
			this.RT = RT;
		}
	}

		//Open document handling
	public class OpenableDocument : DependencyObject
	{
		public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(OpenableDocument), new PropertyMetadata(false));

		public OpenableDocument()
		{
			IsSelected = false;
		}
	}

	public class Project : OpenableDocument
	{
		public Guid ID { get; set; }
		[IgnoreDataMember] public string AbsolutePath { get; private set; }
		public string SolutionPath { get; set; }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Project));

		public Namespace Namespace { get { return (Namespace)GetValue(NamespaceProperty); } set { SetValue(NamespaceProperty, value); } }
		public static readonly DependencyProperty NamespaceProperty = DependencyProperty.Register("Namespace", typeof(Namespace), typeof(Project));

		public string ServerOutputFile { get { return (string)GetValue(ServerOutputFileProperty); } set { SetValue(ServerOutputFileProperty, value); } }
		public static readonly DependencyProperty ServerOutputFileProperty = DependencyProperty.Register("ServerOutputFile", typeof(string), typeof(Project), new PropertyMetadata("Server"));

		public string ClientOutputFile { get { return (string)GetValue(ClientOutputFileProperty); } set { SetValue(ClientOutputFileProperty, value); } }
		public static readonly DependencyProperty ClientOutputFileProperty = DependencyProperty.Register("ClientOutputName", typeof(string), typeof(Project), new PropertyMetadata("Client"));

		public bool EnableDocumentationWarnings { get { return (bool)GetValue(EnableDocumentationWarningsProperty); } set { SetValue(EnableDocumentationWarningsProperty, value); } }
		public static readonly DependencyProperty EnableDocumentationWarningsProperty = DependencyProperty.Register("EnableDocumentationWarnings", typeof(bool), typeof(Project), new PropertyMetadata(false));

		public ProjectServiceSerializerType ServiceSerializer { get { return (ProjectServiceSerializerType)GetValue(ServiceSerializerProperty); } set { SetValue(ServiceSerializerProperty, value); } }
		public static readonly DependencyProperty ServiceSerializerProperty = DependencyProperty.Register("ServiceSerializer", typeof(ProjectServiceSerializerType), typeof(Project));

		public ObservableCollection<DependencyProject> DependencyProjects { get { return (ObservableCollection<DependencyProject>)GetValue(DependencyProjectsProperty); } set { SetValue(DependencyProjectsProperty, value); } }
		public static readonly DependencyProperty DependencyProjectsProperty = DependencyProperty.Register("DependencyProjects", typeof(ObservableCollection<DependencyProject>), typeof(Project));

		public ObservableCollection<DataType> ExternalTypes { get { return (ObservableCollection<DataType>)GetValue(ExternalTypesProperty); } set { SetValue(ExternalTypesProperty, value); } }
		public static readonly DependencyProperty ExternalTypesProperty = DependencyProperty.Register("ExternalTypes", typeof(ObservableCollection<DataType>), typeof(Project));

		public ObservableCollection<ProjectGenerationTarget> ServerGenerationTargets { get { return (ObservableCollection<ProjectGenerationTarget>)GetValue(ServerGenerationTargetsProperty); } set { SetValue(ServerGenerationTargetsProperty, value); } }
		public static readonly DependencyProperty ServerGenerationTargetsProperty = DependencyProperty.Register("ServerGenerationTargets", typeof(ObservableCollection<ProjectGenerationTarget>), typeof(Project));

		public ObservableCollection<ProjectGenerationTarget> ClientGenerationTargets { get { return (ObservableCollection<ProjectGenerationTarget>)GetValue(ClientGenerationTargetsProperty); } set { SetValue(ClientGenerationTargetsProperty, value); } }
		public static readonly DependencyProperty ClientGenerationTargetsProperty = DependencyProperty.Register("ClientGenerationTargets", typeof(ObservableCollection<ProjectGenerationTarget>), typeof(Project));

		public ObservableCollection<ProjectUsingNamespace> UsingNamespaces { get { return (ObservableCollection<ProjectUsingNamespace>)GetValue(UsingNamespacesProperty); } set { SetValue(UsingNamespacesProperty, value); } }
		public static readonly DependencyProperty UsingNamespacesProperty = DependencyProperty.Register("UsingNamespaces", typeof(ObservableCollection<ProjectUsingNamespace>), typeof(Project));

		[IgnoreDataMember] public List<DataType> DefaultTypes { get; private set; }
		[IgnoreDataMember] public List<DataType> InheritableTypes { get; private set; }
		[IgnoreDataMember] public DataType VoidType { get; private set; }

		public bool EnableExperimental { get { return (bool)GetValue(EnableExperimentalProperty); } set { SetValue(EnableExperimentalProperty, value); } }
		public static readonly DependencyProperty EnableExperimentalProperty = DependencyProperty.Register("EnableExperimental", typeof(bool), typeof(Project), new PropertyMetadata(false));

		public Project()
		{
			ID = Guid.NewGuid();
			DependencyProjects = new ObservableCollection<DependencyProject>();
			UsingNamespaces = new ObservableCollection<ProjectUsingNamespace>();
			ExternalTypes = new ObservableCollection<DataType>();

			ServerGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();
			ClientGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();

			//Add the default types
			DefaultTypes = new List<DataType>
				                    {
										//Primitive Types
					                    new DataType(PrimitiveTypes.Object),
					                    new DataType(PrimitiveTypes.Byte),
					                    new DataType(PrimitiveTypes.SByte),
					                    new DataType(PrimitiveTypes.Short),
					                    new DataType(PrimitiveTypes.Int),
					                    new DataType(PrimitiveTypes.Long),
					                    new DataType(PrimitiveTypes.UShort),
					                    new DataType(PrimitiveTypes.UInt),
					                    new DataType(PrimitiveTypes.ULong),
					                    new DataType(PrimitiveTypes.Float),
					                    new DataType(PrimitiveTypes.Double),
					                    new DataType(PrimitiveTypes.Decimal),
					                    new DataType(PrimitiveTypes.Bool),
					                    new DataType(PrimitiveTypes.Char),
					                    new DataType(PrimitiveTypes.String),
					                    new DataType(PrimitiveTypes.DateTime),
					                    new DataType(PrimitiveTypes.DateTimeOffset),
					                    new DataType(PrimitiveTypes.TimeSpan),
					                    new DataType(PrimitiveTypes.URI),
					                    new DataType(PrimitiveTypes.GUID),
					                    new DataType(PrimitiveTypes.Version),
					                    new DataType(PrimitiveTypes.ByteArray),
										//System.Collections.Generic Types
					                    new DataType("LinkedList", DataTypeMode.Collection),
					                    new DataType("List", DataTypeMode.Collection),
					                    new DataType("HashSet", DataTypeMode.Collection),
					                    new DataType("Queue", DataTypeMode.Collection),
					                    new DataType("SortedSet", DataTypeMode.Collection),
					                    new DataType("Stack", DataTypeMode.Collection),
					                    new DataType("SynchronizedCollection", DataTypeMode.Collection),
					                    new DataType("Dictionary", DataTypeMode.Dictionary),
					                    new DataType("SortedDictionary", DataTypeMode.Dictionary),
					                    new DataType("SortedList", DataTypeMode.Dictionary),
					                    new DataType("SynchronizedKeyedCollection", DataTypeMode.Dictionary),
										//System.Collections.ObjectModel Types
					                    new DataType("Collection", DataTypeMode.Collection),
					                    new DataType("ObservableCollection", DataTypeMode.Collection),
					                    new DataType("KeyedCollection", DataTypeMode.Dictionary)
				                    };

			//Add the default inheritable types.
			InheritableTypes = new List<DataType>() {new DataType("System.Windows.DependencyObject", DataTypeMode.Class), new DataType("System.Runtime.Serialization.IExtensibleDataObject", DataTypeMode.Interface), new DataType("System.ComponentModel.INotifyPropertyChanged", DataTypeMode.Interface)};

			//Add the void type
			VoidType = new DataType(PrimitiveTypes.Void);
		}

		public Project(string Name)
		{
			ID = Guid.NewGuid();
			DependencyProjects = new ObservableCollection<DependencyProject>();
			ExternalTypes = new ObservableCollection<DataType>();

			ServerGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();
			ClientGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();

			Namespace = new Namespace(Helpers.RegExs.ReplaceSpaces.Replace(Name, "."), null, this) {URI = "http://tempuri.org/"};
			this.Name = Name;

			//Add the default types
			DefaultTypes = new List<DataType>
				                    {
										//Primitive Types
					                    new DataType(PrimitiveTypes.Object),
					                    new DataType(PrimitiveTypes.Byte),
					                    new DataType(PrimitiveTypes.SByte),
					                    new DataType(PrimitiveTypes.Short),
					                    new DataType(PrimitiveTypes.Int),
					                    new DataType(PrimitiveTypes.Long),
					                    new DataType(PrimitiveTypes.UShort),
					                    new DataType(PrimitiveTypes.UInt),
					                    new DataType(PrimitiveTypes.ULong),
					                    new DataType(PrimitiveTypes.Float),
					                    new DataType(PrimitiveTypes.Double),
					                    new DataType(PrimitiveTypes.Decimal),
					                    new DataType(PrimitiveTypes.Bool),
					                    new DataType(PrimitiveTypes.Char),
					                    new DataType(PrimitiveTypes.String),
					                    new DataType(PrimitiveTypes.DateTime),
					                    new DataType(PrimitiveTypes.DateTimeOffset),
					                    new DataType(PrimitiveTypes.TimeSpan),
					                    new DataType(PrimitiveTypes.URI),
					                    new DataType(PrimitiveTypes.GUID),
					                    new DataType(PrimitiveTypes.Version),
					                    new DataType(PrimitiveTypes.ByteArray),
										//System.Collections.Generic Types
					                    new DataType("LinkedList", DataTypeMode.Collection),
					                    new DataType("List", DataTypeMode.Collection),
					                    new DataType("HashSet", DataTypeMode.Collection),
					                    new DataType("Queue", DataTypeMode.Collection),
					                    new DataType("SortedSet", DataTypeMode.Collection),
					                    new DataType("Stack", DataTypeMode.Collection),
					                    new DataType("SynchronizedCollection", DataTypeMode.Collection),
					                    new DataType("Dictionary", DataTypeMode.Dictionary),
					                    new DataType("SortedDictionary", DataTypeMode.Dictionary),
					                    new DataType("SortedList", DataTypeMode.Dictionary),
					                    new DataType("SynchronizedKeyedCollection", DataTypeMode.Dictionary),
										//System.Collections.ObjectModel Types
					                    new DataType("Collection", DataTypeMode.Collection),
					                    new DataType("ObservableCollection", DataTypeMode.Collection),
					                    new DataType("KeyedCollection", DataTypeMode.Dictionary)
				                    };

			//Add the default inheritable types.
			InheritableTypes = new List<DataType>() { new DataType("System.Windows.DependencyObject", DataTypeMode.Class), new DataType("System.Runtime.Serialization.IExtensibleDataObject", DataTypeMode.Interface), new DataType("System.ComponentModel.INotifyPropertyChanged", DataTypeMode.Interface) };

			//Default Using Namespaces
			UsingNamespaces = new ObservableCollection<ProjectUsingNamespace>
				                  {
					                  new ProjectUsingNamespace("System", true, true, true, true, true),
					                  new ProjectUsingNamespace("System.Collections", true, true, true, true, true),
					                  new ProjectUsingNamespace("System.Collections.Generic", true, true, true, true, true),
					                  new ProjectUsingNamespace("System.Collections.ObjectModel", true, true, true, true, true),
					                  new ProjectUsingNamespace("System.Collections.Specialized", true, true, true, true, true),
					                  new ProjectUsingNamespace("System.ComponentModel", true, true, true, true, true),
					                  new ProjectUsingNamespace("System.Net", true, true, true, true, true),
					                  new ProjectUsingNamespace("System.Net.Security", true, true, true, true, false),
					                  new ProjectUsingNamespace("System.Reflection", true, true, true, true, true),
					                  new ProjectUsingNamespace("System.Resources", true, true, true, true, true),
					                  new ProjectUsingNamespace("System.Runtime.CompilerServices", true, true, true, true, true),
					                  new ProjectUsingNamespace("System.Runtime.InteropServices", true, true, true, true, true),
					                  new ProjectUsingNamespace("System.Runtime.Serialization", true, true, true, true, true),
					                  new ProjectUsingNamespace("System.ServiceModel", true, true, true, true, true),
					                  new ProjectUsingNamespace("System.ServiceModel.Description", true, true, true, true, true),
					                  new ProjectUsingNamespace("System.Text", true, true, true, true, true),
					                  new ProjectUsingNamespace("System.Windows", false, true, true, true, false),
									  new ProjectUsingNamespace("Windows.UI.Core", false, true, false, false, true),
					                  new ProjectUsingNamespace("Windows.UI.Xaml", false, true, false, false, true)
				                  };
		}

		private static string GetRelativePath(string BasePath, string FilePath)
		{
			if (!Path.IsPathRooted(FilePath)) FilePath = Path.GetFullPath(FilePath);
			if (!Path.IsPathRooted(BasePath)) BasePath = Path.GetFullPath(BasePath);

			var t = new Uri("file:///" + FilePath);
			var b = new Uri("file:///" + BasePath);
			return b.MakeRelativeUri(t).ToString();
		}

		public static Project Open(string SolutionPath, string ProjectPath)
		{
			string abspath = new Uri(new Uri(SolutionPath), ProjectPath).LocalPath;

			//Check the file to make sure it exists
			if (!File.Exists(abspath))
				throw new FileNotFoundException("Unable to locate the Project file '" + abspath + "'");

			//Open the project
			var t = Storage.Open<Project>(abspath);
			t.AbsolutePath = abspath;

			// Open the project's dependencies
			foreach (DependencyProject dp in t.DependencyProjects)
			{
				dp.SolutionPath = SolutionPath;
				dp.Project = Open(SolutionPath, dp.Path);
			}
			foreach (DependencyProject dp in t.DependencyProjects)
				dp.EnableAutoReload = true;

			return t;
		}

		public static void Save(Project Data)
		{
			Storage.Save(Data.AbsolutePath, Data);
		}

		public static void Save(Project Data, string Path)
		{
			Storage.Save(Path, Data);
		}

		public static byte[] Dump(Project Data)
		{
			return Storage.Dump(Data);
		}

		public List<DataType> SearchTypes(string Search, bool IncludeCollections = true, bool IncludeVoid = false, bool DataOnly = false, bool IncludeInheritable = false)
		{
			if (string.IsNullOrEmpty(Search)) return new List<DataType>();

			var results = new List<DataType>();

			if (IncludeInheritable) results.AddRange(from a in InheritableTypes where a.Name.IndexOf(Search, StringComparison.CurrentCultureIgnoreCase) >= 0 select a);

			if (DataOnly == false)
			{
				//Decide whether or not we want to include collections and dictionaries in the results.
				if (IncludeCollections) results.AddRange(from a in DefaultTypes where a.Name.IndexOf(Search, StringComparison.CurrentCultureIgnoreCase) >= 0 select a);
				else results.AddRange(from a in DefaultTypes where a.Name.IndexOf(Search, StringComparison.CurrentCultureIgnoreCase) >= 0 && a.TypeMode != DataTypeMode.Collection && a.TypeMode != DataTypeMode.Dictionary select a);
				//Search Externally defined types
				results.AddRange(from a in ExternalTypes where a.Name.IndexOf(Search, StringComparison.CurrentCultureIgnoreCase) >= 0 select a);
			}
			results.AddRange(Namespace.SearchTypes(Search));

			foreach (DependencyProject dp in DependencyProjects)
				results.AddRange(dp.Project.SearchTypes(Search, false, false, true));

			if (IncludeInheritable) results.RemoveAll(a => a.GetType() == typeof (Enum));

			if(IncludeVoid && VoidType.Name.IndexOf(Search, StringComparison.CurrentCultureIgnoreCase) >= 0)  results.Add(VoidType);

			return results;
		}

		public bool HasGenerationFramework(ProjectGenerationFramework Framework)
		{
			return ServerGenerationTargets.Any(t => t.Framework == Framework) || ClientGenerationTargets.Any(t => t.Framework == Framework);
		}

		public bool HasDependencyProject(Guid ID)
		{
			foreach (DependencyProject dp in DependencyProjects)
				if (dp.ProjectID == ID) return true;
				else if (dp.Project.HasDependencyProject(ID)) return true;

			return false;
		}

		public IEnumerable<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			var results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Project || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, this, this));
						if (!string.IsNullOrEmpty(Namespace.Name)) if (Namespace.Name.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Namespace", Namespace.Name, this, this));
						if (!string.IsNullOrEmpty(Namespace.URI)) if (Namespace.URI.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Namespace URI", Namespace.URI, this, this));
					}
					else
					{
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Name", Name, this, this));
						if (!string.IsNullOrEmpty(Namespace.Name)) if (Namespace.Name.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Namespace", Namespace.Name, this, this));
						if (!string.IsNullOrEmpty(Namespace.URI)) if (Namespace.URI.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("ClientName", Namespace.URI, this, this));
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(Name)) if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, this, this));
					if (!string.IsNullOrEmpty(Namespace.Name)) if (Args.RegexSearch.IsMatch(Namespace.Name)) results.Add(new FindReplaceResult("Namespace", Namespace.Name, this, this));
					if (!string.IsNullOrEmpty(Namespace.URI)) if (Args.RegexSearch.IsMatch(Namespace.URI)) results.Add(new FindReplaceResult("Namespace URI", Namespace.URI, this, this));
				}

				if (Args.ReplaceAll)
				{
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (!string.IsNullOrEmpty(Namespace.Name)) Namespace.Name = Microsoft.VisualBasic.Strings.Replace(Namespace.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (!string.IsNullOrEmpty(Namespace.URI)) Namespace.URI = Microsoft.VisualBasic.Strings.Replace(Namespace.URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						else
						{
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
							if (!string.IsNullOrEmpty(Namespace.Name)) Namespace.Name = Microsoft.VisualBasic.Strings.Replace(Namespace.Name, Args.Search, Args.Replace);
							if (!string.IsNullOrEmpty(Namespace.URI)) Namespace.URI = Microsoft.VisualBasic.Strings.Replace(Namespace.URI, Args.Search, Args.Replace);
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(Name)) Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (!string.IsNullOrEmpty(Namespace.Name)) Namespace.Name = Args.RegexSearch.Replace(Namespace.Name, Args.Replace);
						if (!string.IsNullOrEmpty(Namespace.URI)) Namespace.URI = Args.RegexSearch.Replace(Namespace.URI, Args.Replace);
					}
				}
			}

			results.AddRange(Namespace.FindReplace(Args));

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
		{
			if (!Args.ReplaceAll) return;
			if (Args.UseRegex == false)
			{
				if (Args.MatchCase == false)
				{
					if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					if (Field == "Namespace") Namespace.Name = Microsoft.VisualBasic.Strings.Replace(Namespace.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					if (Field == "Namespace URI") Namespace.URI = Microsoft.VisualBasic.Strings.Replace(Namespace.URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
				}
				else
				{
					if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
					if (Field == "Namespace") Namespace.Name = Microsoft.VisualBasic.Strings.Replace(Namespace.Name, Args.Search, Args.Replace);
					if (Field == "Namespace URI") Namespace.URI = Microsoft.VisualBasic.Strings.Replace(Namespace.URI, Args.Search, Args.Replace);
				}
			}
			else
			{
				if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
				if (Field == "Namespace") Namespace.Name = Args.RegexSearch.Replace(Namespace.Name, Args.Replace);
				if (Field == "Namespace URI") Namespace.URI = Args.RegexSearch.Replace(Namespace.URI, Args.Replace);
			}

			Namespace.Replace(Args, Field);
		}

		public bool ProjectHasServices()
		{
			return Namespace.HasServices();
		}

		public override string ToString()
		{
			return Name;
		}
	}

	public class DependencyProject : DependencyObject
	{
		public string Path { get { return (string)GetValue(PathProperty); } set { SetValue(PathProperty, value); } }
		public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(DependencyProject), new PropertyMetadata(""));

		public Guid ProjectID { get; set; }

		[IgnoreDataMember]
		public Project Project { get { return (Project)GetValue(ProjectProperty); } set { SetValue(ProjectProperty, value); } }
		public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(Project), typeof(DependencyProject), new PropertyMetadata(null, ProjectChangedCallback));

		private static void ProjectChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DependencyProject;
			if (t == null) return;
			var p = e.NewValue as Project;
			if (p == null) return;

			t.fsw.Path = System.IO.Path.GetDirectoryName(p.AbsolutePath);
			t.fsw.Filter = System.IO.Path.GetFileName(p.AbsolutePath);
			t.ProjectID = p.ID;
		}

		[IgnoreDataMember] private readonly FileSystemWatcher fsw;
		[IgnoreDataMember] public bool EnableAutoReload { get { return fsw.EnableRaisingEvents; } set { fsw.EnableRaisingEvents = value; } }
		[IgnoreDataMember] public string SolutionPath { get; set; }

		public DependencyProject()
		{
			fsw = new FileSystemWatcher();
			fsw.Changed += fsw_Changed;
			EnableAutoReload = false;
		}

		private void fsw_Changed(object sender, FileSystemEventArgs e)
		{
			try
			{
				if (e.ChangeType != WatcherChangeTypes.Changed) return;
				if (Dispatcher.CheckAccess()) Project = Project.Open(SolutionPath, Path);
				else Dispatcher.Invoke(() => { Project = Project.Open(SolutionPath, Path); }, DispatcherPriority.Normal);
			}
			catch (Exception) { }}
	}

	public enum ProjectGenerationFramework
	{
		NET30,
		NET35,
		NET35Client,
		NET40,
		NET40Client,
		NET45,
		SL40,
		SL50,
		WIN8,
	}

	public class ProjectGenerationTarget : DependencyObject
	{
		public Guid ProjectID { get; set; }
		public Guid ID { get; set; }

		public bool IsEnabled { get { return (bool)GetValue(IsEnabledProperty); } set { SetValue(IsEnabledProperty, value); } }
		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(ProjectGenerationTarget), new UIPropertyMetadata(true));

		public string Path { get { return (string)GetValue(PathProperty); } set { SetValue(PathProperty, value); } }
		public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(ProjectGenerationTarget));

		public ProjectGenerationFramework Framework { get { return (ProjectGenerationFramework)GetValue(FrameworkProperty); } set { SetValue(FrameworkProperty, value); } }
		public static readonly DependencyProperty FrameworkProperty = DependencyProperty.Register("Framework", typeof(ProjectGenerationFramework), typeof(ProjectGenerationTarget), new UIPropertyMetadata(ProjectGenerationFramework.NET45));

		public bool IsServerPath { get { return (bool)GetValue(IsServerPathProperty); } set { SetValue(IsServerPathProperty, value); } }
		public static readonly DependencyProperty IsServerPathProperty = DependencyProperty.Register("IsServerPath", typeof(bool), typeof(ProjectGenerationTarget), new PropertyMetadata(true));

		public bool GenerateReferences { get { return (bool)GetValue(GenerateReferencesProperty); } set { SetValue(GenerateReferencesProperty, value); } }
		public static readonly DependencyProperty GenerateReferencesProperty = DependencyProperty.Register("GenerateReferences", typeof(bool), typeof(ProjectGenerationTarget), new PropertyMetadata(true));

		public ProjectGenerationTarget() { }

		public ProjectGenerationTarget(Guid ProjectID, string Path, bool IsServerPath)
		{
			ID = Guid.NewGuid();
			this.ProjectID = ProjectID;
			this.Path = Path;
			Framework = ProjectGenerationFramework.NET45;
			this.IsServerPath = IsServerPath;
		}
	}
}