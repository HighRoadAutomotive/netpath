using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Runtime.Serialization;
using System.Windows.Threading;

namespace NETPath.Projects
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
		[IgnoreDataMember]
		public string AbsolutePath { get; private set; }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Project));

		public Namespace Namespace { get { return (Namespace)GetValue(NamespaceProperty); } set { SetValue(NamespaceProperty, value); } }
		public static readonly DependencyProperty NamespaceProperty = DependencyProperty.Register("Namespace", typeof(Namespace), typeof(Project));

		public bool EnableDocumentationWarnings { get { return (bool)GetValue(EnableDocumentationWarningsProperty); } set { SetValue(EnableDocumentationWarningsProperty, value); } }
		public static readonly DependencyProperty EnableDocumentationWarningsProperty = DependencyProperty.Register("EnableDocumentationWarnings", typeof(bool), typeof(Project), new PropertyMetadata(false));

		public ProjectServiceSerializerType ServiceSerializer { get { return (ProjectServiceSerializerType)GetValue(ServiceSerializerProperty); } set { SetValue(ServiceSerializerProperty, value); } }
		public static readonly DependencyProperty ServiceSerializerProperty = DependencyProperty.Register("ServiceSerializer", typeof(ProjectServiceSerializerType), typeof(Project));

		public ObservableCollection<DataType> ExternalTypes { get { return (ObservableCollection<DataType>)GetValue(ExternalTypesProperty); } set { SetValue(ExternalTypesProperty, value); } }
		public static readonly DependencyProperty ExternalTypesProperty = DependencyProperty.Register("ExternalTypes", typeof(ObservableCollection<DataType>), typeof(Project));

		public ObservableCollection<ProjectGenerationTarget> ServerGenerationTargets { get { return (ObservableCollection<ProjectGenerationTarget>)GetValue(ServerGenerationTargetsProperty); } set { SetValue(ServerGenerationTargetsProperty, value); } }
		public static readonly DependencyProperty ServerGenerationTargetsProperty = DependencyProperty.Register("ServerGenerationTargets", typeof(ObservableCollection<ProjectGenerationTarget>), typeof(Project));

		public ObservableCollection<ProjectGenerationTarget> ClientGenerationTargets { get { return (ObservableCollection<ProjectGenerationTarget>)GetValue(ClientGenerationTargetsProperty); } set { SetValue(ClientGenerationTargetsProperty, value); } }
		public static readonly DependencyProperty ClientGenerationTargetsProperty = DependencyProperty.Register("ClientGenerationTargets", typeof(ObservableCollection<ProjectGenerationTarget>), typeof(Project));

		public ObservableCollection<ProjectUsingNamespace> UsingNamespaces { get { return (ObservableCollection<ProjectUsingNamespace>)GetValue(UsingNamespacesProperty); } set { SetValue(UsingNamespacesProperty, value); } }
		public static readonly DependencyProperty UsingNamespacesProperty = DependencyProperty.Register("UsingNamespaces", typeof(ObservableCollection<ProjectUsingNamespace>), typeof(Project));

		public bool EnableEntityFramework { get { return (bool)GetValue(EnableEntityFrameworkProperty); } set { SetValue(EnableEntityFrameworkProperty, value); } }
		public static readonly DependencyProperty EnableEntityFrameworkProperty = DependencyProperty.Register("EnableEntityFramework", typeof(bool), typeof(Project), new PropertyMetadata(true));

		public string EnitityDatabaseType { get { return (string)GetValue(EnitityDatabaseTypeProperty); } set { SetValue(EnitityDatabaseTypeProperty, value); } }
		public static readonly DependencyProperty EnitityDatabaseTypeProperty = DependencyProperty.Register("EnitityDatabaseType", typeof(string), typeof(Project), new PropertyMetadata(""));

		public bool GenerateRegions { get { return (bool)GetValue(GenerateRegionsProperty); } set { SetValue(GenerateRegionsProperty, value); } }
		public static readonly DependencyProperty GenerateRegionsProperty = DependencyProperty.Register("GenerateRegions", typeof(bool), typeof(Project), new PropertyMetadata(true));

		public bool UsingInsideNamespace { get { return (bool)GetValue(UsingInsideNamespaceProperty); } set { SetValue(UsingInsideNamespaceProperty, value); } }
		public static readonly DependencyProperty UsingInsideNamespaceProperty = DependencyProperty.Register("UsingInsideNamespace", typeof(bool), typeof(Project), new PropertyMetadata(false));

		[IgnoreDataMember]
		public List<DataType> DefaultTypes { get; private set; }
		[IgnoreDataMember]
		public List<DataType> InheritableTypes { get; private set; }
		[IgnoreDataMember]
		public DataType VoidType { get; private set; }

		public DataType DeltaList { get; private set; }
		public DataType DeltaDictionary { get; private set; }
		public DataType DeltaStack { get; private set; }
		public DataType DeltaQueue { get; private set; }

		public Project()
		{
			ID = Guid.NewGuid();
			UsingNamespaces = new ObservableCollection<ProjectUsingNamespace>();
			ExternalTypes = new ObservableCollection<DataType>();

			ServerGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();
			ClientGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();

			DeltaList = new DataType("DeltaList", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8);
			DeltaDictionary = new DataType("DeltaDictionary", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8);
			DeltaStack = new DataType("DeltaStack", DataTypeMode.Stack, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8);
			DeltaQueue = new DataType("DeltaQueue", DataTypeMode.Queue, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8);

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
										new DataType("SortedSet", DataTypeMode.Collection),
										new DataType("SynchronizedCollection", DataTypeMode.Collection),
										new DataType("Dictionary", DataTypeMode.Dictionary),
										new DataType("SortedDictionary", DataTypeMode.Dictionary),
										new DataType("SortedList", DataTypeMode.Dictionary),
										new DataType("SynchronizedKeyedCollection", DataTypeMode.Dictionary),
										new DataType("Stack", DataTypeMode.Stack),
										new DataType("Queue", DataTypeMode.Queue),
										//System.Collections.ObjectModel Types
										new DataType("Collection", DataTypeMode.Collection),
										new DataType("ObservableCollection", DataTypeMode.Collection),
										new DataType("KeyedCollection", DataTypeMode.Dictionary),
										//System.Collections.Concurrent Types
										new DataType("ConcurrentDictionary", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8),
										new DataType("ConcurrentStack", DataTypeMode.Stack, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8),
										new DataType("ConcurrentQueue", DataTypeMode.Queue, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8),
										//Delta Collections for DCM
										DeltaList,
										DeltaDictionary,
										DeltaStack,
										DeltaQueue,
										//Streaming Support
										new DataType("Stream", DataTypeMode.Class, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8),
									};

			//Add the default inheritable types.
			InheritableTypes = new List<DataType>() { new DataType("System.Windows.DependencyObject", DataTypeMode.Class), new DataType("System.Runtime.Serialization.IExtensibleDataObject", DataTypeMode.Interface), new DataType("System.ComponentModel.INotifyPropertyChanged", DataTypeMode.Interface) };

			//Add the void type
			VoidType = new DataType(PrimitiveTypes.Void);
		}

		public Project(string Name, string Path)
		{
			ID = Guid.NewGuid();
			ExternalTypes = new ObservableCollection<DataType>();

			ServerGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();
			ClientGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();

			Namespace = new Namespace(Helpers.RegExs.ReplaceSpaces.Replace(Name, "."), null, this) { URI = "http://tempuri.org/" };
			this.Name = Name;
			this.AbsolutePath = Path;

			DeltaList = new DataType("DeltaList", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8);
			DeltaDictionary = new DataType("DeltaDictionary", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8);
			DeltaStack = new DataType("DeltaStack", DataTypeMode.Stack, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8);
			DeltaQueue = new DataType("DeltaQueue", DataTypeMode.Queue, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8);

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
										new DataType("SortedSet", DataTypeMode.Collection),
										new DataType("SynchronizedCollection", DataTypeMode.Collection),
										new DataType("Dictionary", DataTypeMode.Dictionary),
										new DataType("SortedDictionary", DataTypeMode.Dictionary),
										new DataType("SortedList", DataTypeMode.Dictionary),
										new DataType("SynchronizedKeyedCollection", DataTypeMode.Dictionary),
										new DataType("Stack", DataTypeMode.Stack),
										new DataType("Queue", DataTypeMode.Queue),
										//System.Collections.ObjectModel Types
										new DataType("Collection", DataTypeMode.Collection),
										new DataType("ObservableCollection", DataTypeMode.Collection),
										new DataType("KeyedCollection", DataTypeMode.Dictionary),
										//System.Collections.Concurrent Types
										new DataType("ConcurrentDictionary", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8),
										new DataType("ConcurrentStack", DataTypeMode.Stack, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8),
										new DataType("ConcurrentQueue", DataTypeMode.Queue, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8),
										//Delta Collections for DCM
										DeltaList,
										DeltaDictionary,
										DeltaStack,
										DeltaQueue,
										//Streaming Support
										new DataType("Stream", DataTypeMode.Class, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8),
									};

			//Add the default inheritable types.
			InheritableTypes = new List<DataType>() { new DataType("System.Windows.DependencyObject", DataTypeMode.Class), new DataType("System.Runtime.Serialization.IExtensibleDataObject", DataTypeMode.Interface), new DataType("System.ComponentModel.INotifyPropertyChanged", DataTypeMode.Interface) };

			//Default Using Namespaces
			UsingNamespaces = new ObservableCollection<ProjectUsingNamespace>
								  {
									  new ProjectUsingNamespace("System", true, true, true, true, true),
									  new ProjectUsingNamespace("System.Collections", true, true, true, true, true),
									  new ProjectUsingNamespace("System.Collections.Generic", true, true, true, true, true),
									  new ProjectUsingNamespace("System.Collections.Concurrent", true, true, true, false, true),
									  new ProjectUsingNamespace("System.Collections.ObjectModel", true, true, true, true, true),
									  new ProjectUsingNamespace("System.Collections.Specialized", true, true, true, true, true),
									  new ProjectUsingNamespace("System.ComponentModel", true, true, true, true, true),
									  new ProjectUsingNamespace("System.Linq", true, true, true, true, true),
									  new ProjectUsingNamespace("System.IO", true, true, true, true, true),
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
									  new ProjectUsingNamespace("System.Threading.Tasks", true, true, true, true, true),
									  new ProjectUsingNamespace("System.Windows", false, true, true, true, false),
									  new ProjectUsingNamespace("Windows.UI.Core", false, true, false, false, true),
									  new ProjectUsingNamespace("Windows.UI.Xaml", false, true, false, false, true),
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

		public static Project Open(string ProjectPath)
		{
			//Check the file to make sure it exists
			if (!File.Exists(ProjectPath))
				throw new FileNotFoundException("Unable to locate the Project file '" + ProjectPath + "'");

			//Open the project
			var t = Storage.Open<Project>(ProjectPath);
			t.AbsolutePath = ProjectPath;

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

		public void SetSelected(DataType Type, Namespace ns = null)
		{
			if (Type == null) return;
			if (ns == null) ns = Namespace;
			Type tt = Type.GetType();

			if (tt == typeof(Enum))
				foreach (var t in ns.Enums)
					t.IsSelected = false;

			if (tt == typeof(Data))
				foreach (var t in ns.Data)
					t.IsSelected = false;

			if (tt == typeof(Service))
				foreach (var t in ns.Services)
					t.IsSelected = false;

			if (tt == typeof(RestService))
				foreach (var t in ns.RestServices)
					t.IsSelected = false;

			if (tt == typeof(Host))
				foreach (var t in ns.Hosts)
					t.IsSelected = false;

			if (tt == typeof(ServiceBinding))
				foreach (var t in ns.Bindings)
					t.IsSelected = false;

			foreach (var tn in ns.Children)
				SetSelected(Type, tn);

			Type.IsSelected = true;
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

			if (IncludeInheritable) results.RemoveAll(a => a.GetType() == typeof(Enum));

			if (IncludeVoid && VoidType.Name.IndexOf(Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(VoidType);

			return results;
		}

		public bool HasGenerationFramework(ProjectGenerationFramework Framework)
		{
			return ServerGenerationTargets.Any(t => t.Framework == Framework) || ClientGenerationTargets.Any(t => t.Framework == Framework);
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

	public enum ProjectGenerationFramework
	{
		NET45,
		WIN8,
	}

	public class ProjectGenerationTarget : DependencyObject
	{
		public Guid ProjectID { get; set; }
		public Guid ID { get; set; }

		public bool IsEnabled { get { return (bool)GetValue(IsEnabledProperty); } set { SetValue(IsEnabledProperty, value); } }
		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(ProjectGenerationTarget), new UIPropertyMetadata(true));

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(ProjectGenerationTarget), new PropertyMetadata(""));

		public string Path { get { return (string)GetValue(PathProperty); } set { SetValue(PathProperty, value); } }
		public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(ProjectGenerationTarget));

		public ProjectGenerationFramework Framework { get { return (ProjectGenerationFramework)GetValue(FrameworkProperty); } set { SetValue(FrameworkProperty, value); } }
		public static readonly DependencyProperty FrameworkProperty = DependencyProperty.Register("Framework", typeof(ProjectGenerationFramework), typeof(ProjectGenerationTarget), new UIPropertyMetadata(ProjectGenerationFramework.NET45));

		public bool IsServerPath { get { return (bool)GetValue(IsServerPathProperty); } set { SetValue(IsServerPathProperty, value); } }
		public static readonly DependencyProperty IsServerPathProperty = DependencyProperty.Register("IsServerPath", typeof(bool), typeof(ProjectGenerationTarget), new PropertyMetadata(true));

		public ObservableCollection<DataType> TargetTypes { get { return (ObservableCollection<DataType>)GetValue(TargetTypesProperty); } set { SetValue(TargetTypesProperty, value); } }
		public static readonly DependencyProperty TargetTypesProperty = DependencyProperty.Register("TargetTypes", typeof(ObservableCollection<DataType>), typeof(ProjectGenerationTarget));

		public ProjectGenerationTarget()
		{
			TargetTypes = new ObservableCollection<DataType>();
		}

		public ProjectGenerationTarget(Guid ProjectID, string Path, bool IsServerPath)
		{
			ID = Guid.NewGuid();
			this.ProjectID = ProjectID;
			this.Path = Path;
			Framework = ProjectGenerationFramework.NET45;
			this.IsServerPath = IsServerPath;
			TargetTypes = new ObservableCollection<DataType>();
		}
	}

	public class DCJSSettings : DependencyObject
	{
		public string DateTimeFormat { get { return (string)GetValue(DateTimeFormatProperty); } set { SetValue(DateTimeFormatProperty, value); } }
		public static readonly DependencyProperty DateTimeFormatProperty = DependencyProperty.Register("DateTimeFormat", typeof(string), typeof(DCJSSettings), new PropertyMetadata("s"));

		public EmitTypeInformation EmitTypeInformation { get { return (EmitTypeInformation)GetValue(EmitTypeInformationProperty); } set { SetValue(EmitTypeInformationProperty, value); } }
		public static readonly DependencyProperty EmitTypeInformationProperty = DependencyProperty.Register("EmitTypeInformation", typeof(EmitTypeInformation), typeof(DCJSSettings), new PropertyMetadata(EmitTypeInformation.Never));

		public int MaxItemsInObjectGraph { get { return (int)GetValue(MaxItemsInObjectGraphProperty); } set { SetValue(MaxItemsInObjectGraphProperty, value); } }
		public static readonly DependencyProperty MaxItemsInObjectGraphProperty = DependencyProperty.Register("MaxItemsInObjectGraph", typeof(int), typeof(DCJSSettings), new PropertyMetadata(int.MaxValue));

		public string RootName { get { return (string)GetValue(RootNameProperty); } set { SetValue(RootNameProperty, value); } }
		public static readonly DependencyProperty RootNameProperty = DependencyProperty.Register("RootName", typeof(string), typeof(DCJSSettings), new PropertyMetadata(""));

		public bool SerializeReadOnlyTypes { get { return (bool)GetValue(SerializeReadOnlyTypesProperty); } set { SetValue(SerializeReadOnlyTypesProperty, value); } }
		public static readonly DependencyProperty SerializeReadOnlyTypesProperty = DependencyProperty.Register("SerializeReadOnlyTypes", typeof(bool), typeof(DCJSSettings), new PropertyMetadata(true));

		public bool UseSimpleDictionaryFormat { get { return (bool)GetValue(UseSimpleDictionaryFormatProperty); } set { SetValue(UseSimpleDictionaryFormatProperty, value); } }
		public static readonly DependencyProperty UseSimpleDictionaryFormatProperty = DependencyProperty.Register("UseSimpleDictionaryFormat", typeof(bool), typeof(DCJSSettings), new PropertyMetadata(true));
	}

	public class JSONNETSettings : DependencyObject
	{
		public ConstructorHandling ConstructorHandling { get { return (ConstructorHandling)GetValue(ConstructorHandlingProperty); } set { SetValue(ConstructorHandlingProperty, value); } }
		public static readonly DependencyProperty ConstructorHandlingProperty = DependencyProperty.Register("ConstructorHandling", typeof(ConstructorHandling), typeof(JSONNETSettings), new PropertyMetadata(ConstructorHandling.Default));

		public DateFormatHandling DateFormatHandling { get { return (DateFormatHandling)GetValue(DateFormatHandlingProperty); } set { SetValue(DateFormatHandlingProperty, value); } }
		public static readonly DependencyProperty DateFormatHandlingProperty = DependencyProperty.Register("DateFormatHandling", typeof(DateFormatHandling), typeof(JSONNETSettings), new PropertyMetadata(DateFormatHandling.IsoDateFormat));

		public string DateFormatString { get { return (string)GetValue(DateFormatStringProperty); } set { SetValue(DateFormatStringProperty, value); } }
		public static readonly DependencyProperty DateFormatStringProperty = DependencyProperty.Register("DateFormatString", typeof(string), typeof(JSONNETSettings), new PropertyMetadata("s"));

		public DateParseHandling DateParseHandling { get { return (DateParseHandling)GetValue(DateParseHandlingProperty); } set { SetValue(DateParseHandlingProperty, value); } }
		public static readonly DependencyProperty DateParseHandlingProperty = DependencyProperty.Register("DateParseHandling", typeof(DateParseHandling), typeof(JSONNETSettings), new PropertyMetadata(DateParseHandling.DateTime));

		public DateTimeZoneHandling DateTimeZoneHandling { get { return (DateTimeZoneHandling)GetValue(DateTimeZoneHandlingProperty); } set { SetValue(DateTimeZoneHandlingProperty, value); } }
		public static readonly DependencyProperty DateTimeZoneHandlingProperty = DependencyProperty.Register("DateTimeZoneHandling", typeof(DateTimeZoneHandling), typeof(JSONNETSettings), new PropertyMetadata(DateTimeZoneHandling.Local));

		public DefaultValueHandling DefaultValueHandling { get { return (DefaultValueHandling)GetValue(DefaultValueHandlingProperty); } set { SetValue(DefaultValueHandlingProperty, value); } }
		public static readonly DependencyProperty DefaultValueHandlingProperty = DependencyProperty.Register("DefaultValueHandling", typeof(DefaultValueHandling), typeof(JSONNETSettings), new PropertyMetadata(DefaultValueHandling.Ignore));

		public FloatFormatHandling FloatFormatHandling { get { return (FloatFormatHandling)GetValue(FloatFormatHandlingProperty); } set { SetValue(FloatFormatHandlingProperty, value); } }
		public static readonly DependencyProperty FloatFormatHandlingProperty = DependencyProperty.Register("FloatFormatHandling ", typeof(FloatFormatHandling), typeof(JSONNETSettings), new PropertyMetadata(FloatFormatHandling.String));

		public FloatParseHandling FloatParseHandling { get { return (FloatParseHandling)GetValue(FloatParseHandlingProperty); } set { SetValue(FloatParseHandlingProperty, value); } }
		public static readonly DependencyProperty FloatParseHandlingProperty = DependencyProperty.Register("FloatParseHandling ", typeof(FloatParseHandling), typeof(JSONNETSettings), new PropertyMetadata(FloatParseHandling.Decimal));

		public Formatting Formatting { get { return (Formatting)GetValue(FormattingProperty); } set { SetValue(FormattingProperty, value); } }
		public static readonly DependencyProperty FormattingProperty = DependencyProperty.Register("Formatting ", typeof(Formatting), typeof(JSONNETSettings), new PropertyMetadata(Formatting.None));

		public int MaxDepth { get { return (int)GetValue(MaxDepthProperty); } set { SetValue(MaxDepthProperty, value); } }
		public static readonly DependencyProperty MaxDepthProperty = DependencyProperty.Register("MaxDepth ", typeof(int), typeof(JSONNETSettings), new PropertyMetadata(int.MaxValue));

		public MetadataPropertyHandling MetadataPropertyHandling { get { return (MetadataPropertyHandling)GetValue(MetadataPropertyHandlingProperty); } set { SetValue(MetadataPropertyHandlingProperty, value); } }
		public static readonly DependencyProperty MetadataPropertyHandlingProperty = DependencyProperty.Register("MetadataPropertyHandling ", typeof(MetadataPropertyHandling), typeof(JSONNETSettings), new PropertyMetadata(MetadataPropertyHandling.Default));

		public MissingMemberHandling MissingMemberHandling { get { return (MissingMemberHandling)GetValue(MissingMemberHandlingProperty); } set { SetValue(MissingMemberHandlingProperty, value); } }
		public static readonly DependencyProperty MissingMemberHandlingProperty = DependencyProperty.Register("MissingMemberHandling ", typeof(MissingMemberHandling), typeof(JSONNETSettings), new PropertyMetadata(MissingMemberHandling.Ignore));

		public NullValueHandling NullValueHandling { get { return (NullValueHandling)GetValue(NullValueHandlingProperty); } set { SetValue(NullValueHandlingProperty, value); } }
		public static readonly DependencyProperty NullValueHandlingProperty = DependencyProperty.Register("NullValueHandling ", typeof(NullValueHandling), typeof(JSONNETSettings), new PropertyMetadata(NullValueHandling.Ignore));

		public ObjectCreationHandling ObjectCreationHandling { get { return (ObjectCreationHandling)GetValue(ObjectCreationHandlingProperty); } set { SetValue(ObjectCreationHandlingProperty, value); } }
		public static readonly DependencyProperty ObjectCreationHandlingProperty = DependencyProperty.Register("NullValueHandling ", typeof(ObjectCreationHandling), typeof(JSONNETSettings), new PropertyMetadata(ObjectCreationHandling.Auto));

		public PreserveReferencesHandling PreserveReferencesHandling { get { return (PreserveReferencesHandling)GetValue(PreserveReferencesHandlingProperty); } set { SetValue(PreserveReferencesHandlingProperty, value); } }
		public static readonly DependencyProperty PreserveReferencesHandlingProperty = DependencyProperty.Register("PreserveReferencesHandling ", typeof(PreserveReferencesHandling), typeof(JSONNETSettings), new PropertyMetadata(PreserveReferencesHandling.None));

		public ReferenceLoopHandling ReferenceLoopHandling { get { return (ReferenceLoopHandling)GetValue(ReferenceLoopHandlingProperty); } set { SetValue(ReferenceLoopHandlingProperty, value); } }
		public static readonly DependencyProperty ReferenceLoopHandlingProperty = DependencyProperty.Register("ReferenceLoopHandling ", typeof(ReferenceLoopHandling), typeof(JSONNETSettings), new PropertyMetadata(ReferenceLoopHandling.Ignore));

		public StringEscapeHandling StringEscapeHandling { get { return (StringEscapeHandling)GetValue(StringEscapeHandlingProperty); } set { SetValue(StringEscapeHandlingProperty, value); } }
		public static readonly DependencyProperty StringEscapeHandlingProperty = DependencyProperty.Register("StringEscapeHandling ", typeof(StringEscapeHandling), typeof(JSONNETSettings), new PropertyMetadata(StringEscapeHandling.Default));

		public System.Runtime.Serialization.Formatters.FormatterAssemblyStyle TypeNameAssemblyFormat { get { return (System.Runtime.Serialization.Formatters.FormatterAssemblyStyle)GetValue(TypeNameAssemblyFormatProperty); } set { SetValue(TypeNameAssemblyFormatProperty, value); } }
		public static readonly DependencyProperty TypeNameAssemblyFormatProperty = DependencyProperty.Register("TypeNameAssemblyFormat ", typeof(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle), typeof(JSONNETSettings), new PropertyMetadata(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple));

		public TypeNameHandling TypeNameHandling { get { return (TypeNameHandling)GetValue(TypeNameHandlingProperty); } set { SetValue(TypeNameHandlingProperty, value); } }
		public static readonly DependencyProperty TypeNameHandlingProperty = DependencyProperty.Register("TypeNameHandling ", typeof(TypeNameHandling), typeof(JSONNETSettings), new PropertyMetadata(TypeNameHandling.Auto));
	}
}