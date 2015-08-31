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
	public enum ProjectGenerationFramework
	{
		NET45,
		WINRT,
	}

	public class ProjectGenerationTarget : DependencyObject
	{
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

		public bool IsDefaultIncluded { get { return (bool)GetValue(IsDefaultIncludedProperty); } set { SetValue(IsDefaultIncludedProperty, value); } }
		public static readonly DependencyProperty IsDefaultIncludedProperty = DependencyProperty.Register("IsDefaultIncluded", typeof(bool), typeof(ProjectGenerationTarget), new PropertyMetadata(true));

		public ObservableCollection<DataType> TargetTypes { get { return (ObservableCollection<DataType>)GetValue(TargetTypesProperty); } set { SetValue(TargetTypesProperty, value); } }
		public static readonly DependencyProperty TargetTypesProperty = DependencyProperty.Register("TargetTypes", typeof(ObservableCollection<DataType>), typeof(ProjectGenerationTarget));

		public ProjectGenerationTarget()
		{
			TargetTypes = new ObservableCollection<DataType>();
		}

		public ProjectGenerationTarget(string Path, bool IsServerPath)
		{
			this.Path = Path;
			Framework = ProjectGenerationFramework.NET45;
			this.IsServerPath = IsServerPath;
			TargetTypes = new ObservableCollection<DataType>();
		}
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
			RT = false;
		}

		public ProjectUsingNamespace(string Namespace, bool Server, bool Client, bool NET, bool RT)
		{
			ID = Guid.NewGuid();
			this.Namespace = Namespace;
			IsFullFrameworkOnly = false;
			this.Server = Server;
			this.Client = Client;
			this.NET = NET;
			this.RT = RT;
		}
	}

	//Open document handling
	public abstract class OpenableDocument : DependencyObject
	{
		public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(OpenableDocument), new PropertyMetadata(false));

		public OpenableDocument()
		{
			IsSelected = false;
		}
	}

	public abstract class Project : OpenableDocument
	{
		[IgnoreDataMember]
		public string AbsolutePath { get; private set; }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Project));

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

		public bool EnableDocumentationWarnings { get { return (bool)GetValue(EnableDocumentationWarningsProperty); } set { SetValue(EnableDocumentationWarningsProperty, value); } }
		public static readonly DependencyProperty EnableDocumentationWarningsProperty = DependencyProperty.Register("EnableDocumentationWarnings", typeof(bool), typeof(Project), new PropertyMetadata(false));

		[IgnoreDataMember]
		public List<DataType> DefaultTypes { get; private set; }
		[IgnoreDataMember]
		public List<DataType> InheritableTypes { get; private set; }
		[IgnoreDataMember]
		public DataType VoidType { get; private set; }

		protected Project()
		{
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
									};

			//Add the default inheritable types.
			InheritableTypes = new List<DataType>() { new DataType("System.Windows.DependencyObject", DataTypeMode.Class), new DataType("System.Runtime.Serialization.IExtensibleDataObject", DataTypeMode.Interface), new DataType("System.ComponentModel.INotifyPropertyChanged", DataTypeMode.Interface) };

			//Add the void type
			VoidType = new DataType(PrimitiveTypes.Void);
		}

		protected Project(string Name, string Path)
		{
			ExternalTypes = new ObservableCollection<DataType>();

			ServerGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();
			ClientGenerationTargets = new ObservableCollection<ProjectGenerationTarget>();

			this.Name = Name;
			this.AbsolutePath = Path;

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
									};

			//Add the default inheritable types.
			InheritableTypes = new List<DataType>() { new DataType("System.Windows.DependencyObject", DataTypeMode.Class), new DataType("System.Runtime.Serialization.IExtensibleDataObject", DataTypeMode.Interface), new DataType("System.ComponentModel.INotifyPropertyChanged", DataTypeMode.Interface) };

			//Default Using Namespaces
			UsingNamespaces = new ObservableCollection<ProjectUsingNamespace>
								  {
									  new ProjectUsingNamespace("System", true, true, true, true),
									  new ProjectUsingNamespace("System.Collections", true, true, true, true),
									  new ProjectUsingNamespace("System.Collections.Generic", true, true, true, true),
									  new ProjectUsingNamespace("System.Collections.Concurrent", true, true, true, true),
									  new ProjectUsingNamespace("System.Collections.ObjectModel", true, true, true, true),
									  new ProjectUsingNamespace("System.ComponentModel", true, true, true, true),
									  new ProjectUsingNamespace("System.Linq", true, true, true, true),
									  new ProjectUsingNamespace("System.IO", true, true, true, true),
									  new ProjectUsingNamespace("System.Net", true, true, true, true),
									  new ProjectUsingNamespace("System.Net.Security", true, true, true, false),
									  new ProjectUsingNamespace("System.Runtime.Serialization", true, true, true, true),
									  new ProjectUsingNamespace("System.Text", true, true, true, true),
									  new ProjectUsingNamespace("System.Threading.Tasks", true, true, true, true),
									  new ProjectUsingNamespace("System.Windows", false, true, true, false),
									  new ProjectUsingNamespace("Windows.UI.Core", false, true, false, true),
									  new ProjectUsingNamespace("Windows.UI.Xaml", false, true, false, true),
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

		public abstract void SetSelected(DataType Type, Namespace ns = null);

		public abstract List<DataType> SearchTypes(string Search, bool IncludeCollections = true, bool IncludeVoid = false, bool DataOnly = false, bool IncludeInheritable = false);

		public override string ToString()
		{
			return Name;
		}
	}
}