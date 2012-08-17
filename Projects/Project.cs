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

		public Project() : base()
		{
			this.DependencyProjects = new ObservableCollection<DependencyProject>();
			this.UsingNamespaces = new ObservableCollection<ProjectUsingNamespace>();

			this.ServerGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();
			this.ClientGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();
		}

		public Project(string Name) : base()
		{
			this.DependencyProjects = new ObservableCollection<DependencyProject>();
			this.UsingNamespaces = new ObservableCollection<ProjectUsingNamespace>();

			this.ServerGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();
			this.ClientGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();

			this.Namespace = new Namespace(Helpers.RegExs.ReplaceSpaces.Replace(Name, "."), null, this);
			this.Namespace.URI = "http://tempuri.org/" + Namespace.Name.Replace(".", "/") + "/";
			this.Name = Name;

			//Default Using Namespaces
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
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("System.Windows", false, true, true, true, false));
			UsingNamespaces.Add(new Projects.ProjectUsingNamespace("Windows.UI.Xaml", false, true, false, false, true));
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

		public static Project Open(string SolutionPath, string ProjectPath)
		{
			string abspath = new Uri(new Uri(SolutionPath), ProjectPath).AbsolutePath;

			//Check the file to make sure it exists
			if (!System.IO.File.Exists(abspath))
				throw new System.IO.FileNotFoundException("Unable to locate the Project file '" + abspath + "'");

			Project t = Storage.Open<Project>(abspath);
			t.AbsolutePath = abspath;
			return t;
		}

		public static void Save(Project Data)
		{
			Storage.Save<Project>(Data.AbsolutePath, Data);
		}

		public static void Save(Project Data, string Path)
		{
			Storage.Save<Project>(Path, Data);
		}

		public bool HasGenerationFramework(ProjectGenerationFramework Framework)
		{
			foreach (ProjectGenerationTarget t in ServerGenerationTargets)
				if (t.Framework == Framework)
					return true;
			foreach (ProjectGenerationTarget t in ClientGenerationTargets)
				if (t.Framework == Framework)
					return true;

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
							if (Namespace.URI != null && Namespace.URI != "") if (Namespace.URI.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("ContractName", Namespace.URI, this, this));
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
		public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(DependencyProject), new PropertyMetadata(0));

		[IgnoreDataMember()] public Project Project { get { return (Project)GetValue(ProjectProperty); } set { SetValue(ProjectProperty, value); } }
		public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(Project), typeof(DependencyProject));
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

		public ProjectGenerationTarget() { }

		public ProjectGenerationTarget(Guid ProjectID, string Path)
		{
			this.ID = Guid.NewGuid();
			this.ProjectID = ProjectID;
			this.Path = Path;
			this.Framework = ProjectGenerationFramework.NET45;
		}
	}
}