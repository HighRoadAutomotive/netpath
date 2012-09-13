using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;

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
			this.ID = Guid.NewGuid();
			this.Namespace = Namespace;
			this.IsFullFrameworkOnly = false;
			this.Server = true;
			this.Client = true;
			this.NET = true;
			this.SL = false;
			this.RT = false;
		}

		public ProjectUsingNamespace(string Namespace, bool Server, bool Client, bool NET, bool SL, bool RT)
		{
			this.ID = Guid.NewGuid();
			this.Namespace = Namespace;
			this.IsFullFrameworkOnly = false;
			this.Server = Server;
			this.Client = Client;
			this.NET = NET;
			this.SL = SL;
			this.RT = RT;
		}
	}

	public class OpenableDocument : DependencyObject
	{
		//Open document handling
		[IgnoreDataMember()] public bool IsActive { get; set; }
		[IgnoreDataMember()] public bool IsOpen { get; set; }

		[IgnoreDataMember()] public bool IsDirty { get { return (bool)GetValue(IsDirtyProperty); } set { if (IsActive == true) SetValue(IsDirtyProperty, value); } }
		public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register("IsDirty", typeof(bool), typeof(OpenableDocument), new UIPropertyMetadata(false));

		public OpenableDocument() : base()
		{
			IsActive = false;
			IsDirty = false;
		}
	}

	public enum ProjectTypeSearchMode
	{
		All,
		Data,
		Enum
	}

	public partial class Project : OpenableDocument
	{
		public Guid ID { get; set; }
		[IgnoreDataMember()] public string AbsolutePath { get; private set; }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Project));

		public ObservableCollection<ProjectUsingNamespace> UsingNamespaces { get { return (ObservableCollection<ProjectUsingNamespace>)GetValue(UsingNamespacesProperty); } set { SetValue(UsingNamespacesProperty, value); } }
		public static readonly DependencyProperty UsingNamespacesProperty = DependencyProperty.Register("UsingNamespaces", typeof(ObservableCollection<ProjectUsingNamespace>), typeof(Project));

		public Namespace Namespace { get { return (Namespace)GetValue(NamespaceProperty); } set { SetValue(NamespaceProperty, value); } }
		public static readonly DependencyProperty NamespaceProperty = DependencyProperty.Register("Namespace", typeof(Namespace), typeof(Project));

		public string ServerOutputFile { get { return (string)GetValue(ServerOutputFileProperty); } set { SetValue(ServerOutputFileProperty, value); } }
		public static readonly DependencyProperty ServerOutputFileProperty = DependencyProperty.Register("ServerOutputFile", typeof(string), typeof(Project), new PropertyMetadata("Server"));

		public string ClientOutputFile { get { return (string)GetValue(ClientOutputFileProperty); } set { SetValue(ClientOutputFileProperty, value); } }
		public static readonly DependencyProperty ClientOutputFileProperty = DependencyProperty.Register("ClientOutputName", typeof(string), typeof(Project), new PropertyMetadata("Client"));

		public bool ServiceSerializeFaults { get { return (bool)GetValue(ServiceSerializeFaultsProperty); } set { SetValue(ServiceSerializeFaultsProperty, value); } }
		public static readonly DependencyProperty ServiceSerializeFaultsProperty = DependencyProperty.Register("ServiceSerializeFaults", typeof(bool), typeof(Project), new PropertyMetadata(false));

		public ProjectServiceSerializerType ServiceSerializer { get { return (ProjectServiceSerializerType)GetValue(ServiceSerializerProperty); } set { SetValue(ServiceSerializerProperty, value); } }
		public static readonly DependencyProperty ServiceSerializerProperty = DependencyProperty.Register("ServiceSerializer", typeof(ProjectServiceSerializerType), typeof(Project));

		public ProjectCollectionSerializationOverride CollectionSerializationOverride { get { return (ProjectCollectionSerializationOverride)GetValue(CollectionSerializationOverrideProperty); } set { SetValue(CollectionSerializationOverrideProperty, value); } }
		public static readonly DependencyProperty CollectionSerializationOverrideProperty = DependencyProperty.Register("CollectionSerializationOverride", typeof(ProjectCollectionSerializationOverride), typeof(Project), new PropertyMetadata(ProjectCollectionSerializationOverride.None));

		public ObservableCollection<DependencyProject> DependencyProjects { get { return (ObservableCollection<DependencyProject>)GetValue(DependencyProjectsProperty); } set { SetValue(DependencyProjectsProperty, value); } }
		public static readonly DependencyProperty DependencyProjectsProperty = DependencyProperty.Register("DependencyProjects", typeof(ObservableCollection<DependencyProject>), typeof(Project));

		public ObservableCollection<ProjectGenerationTarget> ServerGenerationTargets { get { return (ObservableCollection<ProjectGenerationTarget>)GetValue(ServerGenerationTargetsProperty); } set { SetValue(ServerGenerationTargetsProperty, value); } }
		public static readonly DependencyProperty ServerGenerationTargetsProperty = DependencyProperty.Register("ServerGenerationTargets", typeof(ObservableCollection<ProjectGenerationTarget>), typeof(Project));

		public ObservableCollection<ProjectGenerationTarget> ClientGenerationTargets { get { return (ObservableCollection<ProjectGenerationTarget>)GetValue(ClientGenerationTargetsProperty); } set { SetValue(ClientGenerationTargetsProperty, value); } }
		public static readonly DependencyProperty ClientGenerationTargetsProperty = DependencyProperty.Register("ClientGenerationTargets", typeof(ObservableCollection<ProjectGenerationTarget>), typeof(Project));

		//Internal Use - Searching / Filtering
		[IgnoreDataMember()] public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(Project));

		[IgnoreDataMember()] public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(Project));

		[IgnoreDataMember()] public bool IsFiltering { get { return (bool)GetValue(IsFilteringProperty); } set { SetValue(IsFilteringProperty, value); } }
		public static readonly DependencyProperty IsFilteringProperty = DependencyProperty.Register("IsFiltering", typeof(bool), typeof(Project));

		[IgnoreDataMember()] public bool IsFilterMatch { get { return (bool)GetValue(IsFilterMatchProperty); } set { SetValue(IsFilterMatchProperty, value); } }
		public static readonly DependencyProperty IsFilterMatchProperty = DependencyProperty.Register("IsFilterMatch", typeof(bool), typeof(Project));

		public bool IsTreeExpanded { get { return (bool)GetValue(IsTreeExpandedProperty); } set { SetValue(IsTreeExpandedProperty, value); } }
		public static readonly DependencyProperty IsTreeExpandedProperty = DependencyProperty.Register("IsTreeExpanded", typeof(bool), typeof(Project), new UIPropertyMetadata(true));

		public List<DataType> DefaultTypes { get; private set; }

		public Project()
		{
			ID = Guid.NewGuid();
			DependencyProjects = new ObservableCollection<DependencyProject>();
			UsingNamespaces = new ObservableCollection<ProjectUsingNamespace>();

			ServerGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();
			ClientGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();

			//Add the default types
			DefaultTypes = new List<DataType>
				                    {
										//Primitive Types
					                    new DataType(PrimitiveTypes.Void),
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
		}

		public Project(string Name)
		{
			ID = Guid.NewGuid();
			DependencyProjects = new ObservableCollection<DependencyProject>();

			ServerGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();
			ClientGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();

			Namespace = new Namespace(Helpers.RegExs.ReplaceSpaces.Replace(Name, "."), null, this) {URI = "http://tempuri.org/" + Namespace.Name.Replace(".", "/") + "/"};
			this.Name = Name;

			//Add the default types
			DefaultTypes = new List<DataType>
				                    {
										//Primitive Types
					                    new DataType(PrimitiveTypes.Void),
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
					                  new ProjectUsingNamespace("Windows.UI.Xaml", false, true, false, false, true)
				                  };
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == IsDirtyProperty) return;
			if (e.Property == IsSearchingProperty) return;
			if (e.Property == IsSearchMatchProperty) return;
			if (e.Property == IsFilteringProperty) return;
			if (e.Property == IsFilterMatchProperty) return;
			if (e.Property == IsTreeExpandedProperty) return;

			IsDirty = true;
		}

		public static Project Open(string SolutionPath, string ProjectPath)
		{
			string abspath = new Uri(new Uri(SolutionPath), ProjectPath).AbsolutePath;

			//Check the file to make sure it exists
			if (!System.IO.File.Exists(abspath))
				throw new System.IO.FileNotFoundException("Unable to locate the Project file '" + abspath + "'");

			//Open the project
			var t = Storage.Open<Project>(abspath);
			t.AbsolutePath = abspath;

			// Open the project's dependencies
			foreach(DependencyProject dp in t.DependencyProjects)
				dp.Project = Open(SolutionPath, dp.Path);

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

		public List<DataType> SearchTypes(string Search, bool DataOnly = false, bool IsDependency = false)
		{
			if(string.IsNullOrEmpty(Search)) return new List<DataType>();

			var results = new List<DataType>();

			if (DataOnly == false)
				if (IsDependency == false) results.AddRange(from a in DefaultTypes where a.Name.IndexOf(Search, StringComparison.CurrentCultureIgnoreCase) >= 0 select a);
			results.AddRange(Namespace.SearchTypes(Search, DataOnly));

			foreach(DependencyProject dp in DependencyProjects)
				results.AddRange(dp.Project.SearchTypes(Search, true));

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

		public void Search(string Value)
		{
			Namespace.Search(Value);
		}

		public void Filter(bool FilterAll, bool FilterNamespaces, bool FilterServices, bool FilterData, bool FilterEnums)
		{
			Namespace.Filter(FilterAll, FilterNamespaces, FilterServices, FilterData, FilterEnums);
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
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, this, this));
							if (Namespace.Name != null && Namespace.Name != "") if (Namespace.Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Namespace", Namespace.Name, this, this));
							if (Namespace.URI != null && Namespace.URI != "") if (Namespace.URI.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("ClientName", Namespace.URI, this, this));
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
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (Namespace.Name != null && Namespace.Name != "") Namespace.Name = Microsoft.VisualBasic.Strings.Replace(Namespace.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (Namespace.URI != null && Namespace.URI != "") Namespace.URI = Microsoft.VisualBasic.Strings.Replace(Namespace.URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
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
						}
					}
					IsActive = ia;
				}
			}

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
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Namespace") Namespace.Name = Microsoft.VisualBasic.Strings.Replace(Namespace.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Namespace URI") Namespace.URI = Microsoft.VisualBasic.Strings.Replace(Namespace.URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
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
					}
				}
				IsActive = ia;
			}
		}

		public bool ProjectHasServices()
		{
			if (Namespace.HasServices() == true) return true;

			return false;
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

		[IgnoreDataMember()] public Project Project { get { return (Project)GetValue(ProjectProperty); } set { SetValue(ProjectProperty, value); if (value != null) ProjectID = Project.ID; } }
		public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(Project), typeof(DependencyProject));

		public Guid ProjectID { get; private set; }
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
		public Guid ProjectID { get; protected set; }
		public Guid ID { get; set; }

		public bool IsEnabled { get { return (bool)GetValue(IsEnabledProperty); } set { SetValue(IsEnabledProperty, value); } }
		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(ProjectGenerationTarget), new UIPropertyMetadata(true));

		public string Path { get { return (string)GetValue(PathProperty); } set { SetValue(PathProperty, value); } }
		public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(ProjectGenerationTarget));

		public ProjectGenerationFramework Framework { get { return (ProjectGenerationFramework)GetValue(FrameworkProperty); } set { SetValue(FrameworkProperty, value); } }
		public static readonly DependencyProperty FrameworkProperty = DependencyProperty.Register("Framework", typeof(ProjectGenerationFramework), typeof(ProjectGenerationTarget), new UIPropertyMetadata(ProjectGenerationFramework.NET45));

		public bool IsServerPath { get { return (bool)GetValue(IsServerPathProperty); } set { SetValue(IsServerPathProperty, value); } }
		public static readonly DependencyProperty IsServerPathProperty = DependencyProperty.Register("IsServerPath", typeof(bool), typeof(ProjectGenerationTarget), new PropertyMetadata(true));

		public ProjectGenerationTarget() { }

		public ProjectGenerationTarget(Guid ProjectID, string Path, bool IsServerPath)
		{
			this.ID = Guid.NewGuid();
			this.ProjectID = ProjectID;
			this.Path = Path;
			this.Framework = ProjectGenerationFramework.NET45;
			this.IsServerPath = IsServerPath;
		}
	}
}