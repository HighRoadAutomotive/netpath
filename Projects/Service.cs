using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;
using System.ServiceModel.Web;

namespace WCFArchitect.Projects
{
	public enum ServiceAsynchronyMode
	{
		Default = 0,
		Client = 1,
		Server = 2,
		Both = 3,
	}

	public class Service : DataType
	{
		public ObservableCollection<Operation> ServiceOperations { get { return (ObservableCollection<Operation>)GetValue(ServiceOperationsProperty); } set { SetValue(ServiceOperationsProperty, value); } }
		public static readonly DependencyProperty ServiceOperationsProperty = DependencyProperty.Register("ServiceOperations", typeof(ObservableCollection<Operation>), typeof(Service));

		public ObservableCollection<Operation> CallbackOperations { get { return (ObservableCollection<Operation>)GetValue(CallbackOperationsProperty); } set { SetValue(CallbackOperationsProperty, value); } }
		public static readonly DependencyProperty CallbackOperationsProperty = DependencyProperty.Register("CallbackOperations", typeof(ObservableCollection<Operation>), typeof(Service));

		public ServiceAsynchronyMode AsynchronyMode { get { return (ServiceAsynchronyMode)GetValue(AsynchronyModeProperty); } set { SetValue(AsynchronyModeProperty, value); } }
		public static readonly DependencyProperty AsynchronyModeProperty = DependencyProperty.Register("AsynchronyMode", typeof(ServiceAsynchronyMode), typeof(Service), new PropertyMetadata(ServiceAsynchronyMode.Default));

		public ProtectionLevel ProtectionLevel { get { return (ProtectionLevel)GetValue(ProtectionLevelProperty); } set { SetValue(ProtectionLevelProperty, value); } }
		public static readonly DependencyProperty ProtectionLevelProperty = DependencyProperty.Register("ProtectionLevel", typeof(ProtectionLevel), typeof(Service));

		public System.ServiceModel.SessionMode SessionMode { get { return (System.ServiceModel.SessionMode)GetValue(SessionModeProperty); } set { SetValue(SessionModeProperty, value); } }
		public static readonly DependencyProperty SessionModeProperty = DependencyProperty.Register("SessionMode", typeof(System.ServiceModel.SessionMode), typeof(Service));

		public string ConfigurationName { get { return (string)GetValue(ConfigurationNameProperty); } set { SetValue(ConfigurationNameProperty, value); } }
		public static readonly DependencyProperty ConfigurationNameProperty = DependencyProperty.Register("ConfigurationName", typeof(string), typeof(Service));

		public Documentation ServiceDocumentation { get { return (Documentation)GetValue(ServiceDocumentationProperty); } set { SetValue(ServiceDocumentationProperty, value); } }
		public static readonly DependencyProperty ServiceDocumentationProperty = DependencyProperty.Register("ServiceDocumentation", typeof(Documentation), typeof(Service));

		public Documentation CallbackDocumentation { get { return (Documentation)GetValue(CallbackDocumentationProperty); } set { SetValue(CallbackDocumentationProperty, value); } }
		public static readonly DependencyProperty CallbackDocumentationProperty = DependencyProperty.Register("CallbackDocumentation", typeof(Documentation), typeof(Service));

		//System
		[IgnoreDataMember] public bool HasCallback { get { return CallbackOperations.Count > 0; } }
		[IgnoreDataMember] public bool HasAsyncServiceOperations { get { return ServiceOperations.Where(a => a.GetType() == typeof (Method)).Any(a => ((Method) a).UseAsyncPattern); } }
		[IgnoreDataMember] public bool HasAsyncCallbackOperations { get { return CallbackOperations.Where(a => a.GetType() == typeof (Method)).Any(a => ((Method) a).UseAsyncPattern); } }

		public Service() : base(DataTypeMode.Class)
		{
			ServiceOperations = new ObservableCollection<Operation>();
			CallbackOperations = new ObservableCollection<Operation>();
			ServiceDocumentation = new Documentation { IsClass = true };
			CallbackDocumentation = new Documentation { IsClass = true };
		}

		public Service(string Name, Namespace Parent) : base(DataTypeMode.Class)
		{
			this.Name = Name;
			this.Parent = Parent;
			ServiceOperations = new ObservableCollection<Operation>();
			CallbackOperations = new ObservableCollection<Operation>();
			ID = Guid.NewGuid();
			ConfigurationName = "";
			ServiceDocumentation = new Documentation { IsClass = true };
			CallbackDocumentation = new Documentation { IsClass = true };
		}

		public void AddKnownType(DataType Type)
		{
			if (KnownTypes.Any(dt => dt.TypeName == Type.TypeName)) return;
			KnownTypes.Add(Type.Copy());
		}

		public void RemoveKnownType(DataType Type)
		{
			if (ServiceOperations.Any(dt => dt.ReturnType.TypeName == Type.TypeName)) return;
			var d = KnownTypes.FirstOrDefault(a => a.TypeName == Type.TypeName);
			if (d != null) KnownTypes.Remove(d);
		}

		public IEnumerable<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			var results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Service || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
						if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) if (ClientType.Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Contract Name", ClientType.Name, Parent.Owner, this));
					}
					else
					{
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
						if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) if (ClientType.Name.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Contract Name", ClientType.Name, Parent.Owner, this));
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(Name)) if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
					if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) if (Args.RegexSearch.IsMatch(ClientType.Name)) results.Add(new FindReplaceResult("Contract Name", ClientType.Name, Parent.Owner, this));
				}

				if (Args.ReplaceAll)
				{
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) ClientType.Name = Microsoft.VisualBasic.Strings.Replace(ClientType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						else
						{
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
							if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) ClientType.Name = Microsoft.VisualBasic.Strings.Replace(ClientType.Name, Args.Search, Args.Replace);
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(Name)) Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) ClientType.Name = Args.RegexSearch.Replace(ClientType.Name, Args.Replace);
					}
				}
			}

			foreach (Operation o in ServiceOperations)
				results.AddRange(o.FindReplace(Args));

			foreach (Operation o in CallbackOperations)
				results.AddRange(o.FindReplace(Args));

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
					if (HasClientType) if (Field == "Contract Name") ClientType.Name = Microsoft.VisualBasic.Strings.Replace(ClientType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
				}
				else
				{
					if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
					if (HasClientType) if (Field == "Contract Name") ClientType.Name = Microsoft.VisualBasic.Strings.Replace(ClientType.Name, Args.Search, Args.Replace);
				}
			}
			else
			{
				if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
				if (HasClientType) if (Field == "Contract Name") ClientType.Name = Args.RegexSearch.Replace(ClientType.Name, Args.Replace);
			}
		}
	}

	public abstract class Operation : DependencyObject
	{
		public Guid ID { get; set; }

		public DataType ReturnType { get { return (DataType)GetValue(ReturnTypeProperty); } set { SetValue(ReturnTypeProperty, value); } }
		public static readonly DependencyProperty ReturnTypeProperty = DependencyProperty.Register("ReturnType", typeof(DataType), typeof(Operation), new PropertyMetadata(ReturnTypeChangedCallback));

		private static void ReturnTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs p)
		{
			var de = o as Operation;
			if (de == null) return;
			var nt = p.NewValue as DataType;
			if (nt == null) return;
			var ot = p.OldValue as DataType;
			if (ot == null) return;

			if (ot.TypeMode == DataTypeMode.Array && ot.CollectionGenericType.TypeMode == DataTypeMode.Primitive) de.Owner.RemoveKnownType(nt);
			if (nt.TypeMode == DataTypeMode.Array && nt.CollectionGenericType.TypeMode == DataTypeMode.Primitive) de.Owner.AddKnownType(nt);
			if (ot.TypeMode == DataTypeMode.Primitive && ot.Primitive == PrimitiveTypes.DateTimeOffset) de.Owner.RemoveKnownType(new DataType(PrimitiveTypes.DateTimeOffset));
			if (nt.TypeMode == DataTypeMode.Primitive && nt.Primitive == PrimitiveTypes.DateTimeOffset) de.Owner.AddKnownType(new DataType(PrimitiveTypes.DateTimeOffset));
		}

		public string ServerName { get { return (string)GetValue(ServerNameProperty); } set { SetValue(ServerNameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value ?? "", @"")); } }
		public static readonly DependencyProperty ServerNameProperty = DependencyProperty.Register("ServerName", typeof(string), typeof(Operation));

		public bool HasClientType { get { return (bool)GetValue(HasClientTypeProperty); } set { SetValue(HasClientTypeProperty, value); } }
		public static readonly DependencyProperty HasClientTypeProperty = DependencyProperty.Register("HasClientType", typeof(bool), typeof(Operation), new PropertyMetadata(false, HasClientTypeChangedCallback));

		private static void HasClientTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as Operation;
			if (t == null) return;

			t.ClientName = Convert.ToBoolean(e.NewValue) ? t.ServerName : "";
		}

		public string ClientName { get { return (string)GetValue(ClientNameProperty); } set { SetValue(ClientNameProperty, value); } }
		public static readonly DependencyProperty ClientNameProperty = DependencyProperty.Register("ClientName", typeof(string), typeof(Operation), new PropertyMetadata(""));

		public bool IsOneWay { get { return (bool)GetValue(IsOneWayProperty); } set { SetValue(IsOneWayProperty, value); } }
		public static readonly DependencyProperty IsOneWayProperty = DependencyProperty.Register("IsOneWay", typeof(bool), typeof(Operation), new PropertyMetadata(false));

		public ProtectionLevel ProtectionLevel { get { return (ProtectionLevel)GetValue(ProtectionLevelProperty); } set { SetValue(ProtectionLevelProperty, value); } }
		public static readonly DependencyProperty ProtectionLevelProperty = DependencyProperty.Register("ProtectionLevel", typeof(ProtectionLevel), typeof(Operation), new PropertyMetadata(ProtectionLevel.None));

		[IgnoreDataMember] public string Declaration { get { return (string)GetValue(DeclarationProperty); } protected set { SetValue(DeclarationPropertyKey, value); } }
		private static readonly DependencyPropertyKey DeclarationPropertyKey = DependencyProperty.RegisterReadOnly("Declaration", typeof(string), typeof(Operation), new PropertyMetadata(""));
		public static readonly DependencyProperty DeclarationProperty = DeclarationPropertyKey.DependencyProperty;
		
		[IgnoreDataMember] public string ClientDeclaration { get { return (string)GetValue(ClientDeclarationProperty); } protected set { SetValue(ClientDeclarationPropertyKey, value); } }
		private static readonly DependencyPropertyKey ClientDeclarationPropertyKey = DependencyProperty.RegisterReadOnly("ClientDeclaration", typeof(string), typeof(Operation), new PropertyMetadata(""));
		public static readonly DependencyProperty ClientDeclarationProperty = ClientDeclarationPropertyKey.DependencyProperty;

		//System
		public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(Operation), new PropertyMetadata(false));

		public Service Owner { get; set; }

		public Operation() { }

		public Operation(string Name, Service Owner)
		{
			ID = Guid.NewGuid();
			ServerName = Name;
			ReturnType = new DataType(PrimitiveTypes.Void);
			ProtectionLevel = ProtectionLevel.None;
			this.Owner = Owner;
		}

		public override string ToString()
		{
			return ServerName;
		}

		public virtual IEnumerable<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			var results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Service || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (!string.IsNullOrEmpty(ServerName)) if (ServerName.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Server Name", ServerName, Owner.Parent.Owner, this));
						if (!string.IsNullOrEmpty(ClientName)) if (ClientName.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Client Name", ClientName, Owner.Parent.Owner, this));
					}
					else
					{
						if (!string.IsNullOrEmpty(ServerName)) if (ServerName.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Server Name", ServerName, Owner.Parent.Owner, this));
						if (!string.IsNullOrEmpty(ClientName)) if (ClientName.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Client Name", ClientName, Owner.Parent.Owner, this));
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(ServerName)) if (Args.RegexSearch.IsMatch(ServerName)) results.Add(new FindReplaceResult("Server Name", ServerName, Owner.Parent.Owner, this));
					if (!string.IsNullOrEmpty(ClientName)) if (Args.RegexSearch.IsMatch(ClientName)) results.Add(new FindReplaceResult("Client Name", ClientName, Owner.Parent.Owner, this));
				}

				if (Args.ReplaceAll)
				{
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (!string.IsNullOrEmpty(ServerName)) ServerName = Microsoft.VisualBasic.Strings.Replace(ServerName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (!string.IsNullOrEmpty(ClientName)) ClientName = Microsoft.VisualBasic.Strings.Replace(ClientName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						else
						{
							if (!string.IsNullOrEmpty(ServerName)) ServerName = Microsoft.VisualBasic.Strings.Replace(ServerName, Args.Search, Args.Replace);
							if (!string.IsNullOrEmpty(ClientName)) ClientName = Microsoft.VisualBasic.Strings.Replace(ClientName, Args.Search, Args.Replace);
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(ServerName)) ServerName = Args.RegexSearch.Replace(ServerName, Args.Replace);
						if (!string.IsNullOrEmpty(ClientName)) ClientName = Args.RegexSearch.Replace(ClientName, Args.Replace);
					}
				}
			}

			return results;
		}

		public virtual void Replace(FindReplaceInfo Args, string Field)
		{
			if (!Args.ReplaceAll) return;
			if (Args.UseRegex == false)
			{
				if (Args.MatchCase == false)
				{
					if (Field == "Server Name") ServerName = Microsoft.VisualBasic.Strings.Replace(ServerName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					if (Field == "Client Name") ClientName = Microsoft.VisualBasic.Strings.Replace(ClientName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
				}
				else
				{
					if (Field == "Server Name") ServerName = Microsoft.VisualBasic.Strings.Replace(ServerName, Args.Search, Args.Replace);
					if (Field == "Client Name") ClientName = Microsoft.VisualBasic.Strings.Replace(ClientName, Args.Search, Args.Replace);
				}
			}
			else
			{
				if (Field == "Server Name") ServerName = Args.RegexSearch.Replace(ServerName, Args.Replace);
				if (Field == "Client Name") ClientName = Args.RegexSearch.Replace(ClientName, Args.Replace);
			}
		}
	}

	public class Property : Operation
	{
		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(Property));

		public bool IsReadOnly { get { return (bool)GetValue(IsReadOnlyProperty); } set { SetValue(IsReadOnlyProperty, value); } }
		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(Property));

		public Property()
		{
			ReturnType = new DataType(PrimitiveTypes.String);
			Documentation = new Documentation { IsProperty = true };
		}

		public Property(DataType ReturnType, string Name, Service Owner) : base(Name, Owner)
		{
			this.ReturnType = ReturnType;
			Documentation = new Documentation { IsProperty = true };
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == DeclarationProperty || e.Property == ClientDeclarationProperty) return;
			Declaration = string.Format("{0} {1}{{ get; {2}}}", ReturnType, ServerName, IsReadOnly ? "set; " : "");
			ClientDeclaration = string.Format("{0} {1}{{ get; {2}}}", ReturnType, ClientName, IsReadOnly ? "set; " : "");
		}
	}

	public class Method : Operation
	{
		public bool UseSyncPattern { get { return (bool)GetValue(UseSyncPatternProperty); } set { SetValue(UseSyncPatternProperty, value); } }
		public static readonly DependencyProperty UseSyncPatternProperty = DependencyProperty.Register("UseSyncPattern", typeof(bool), typeof(Method), new PropertyMetadata(true));

		public bool UseAsyncPattern { get { return (bool)GetValue(UseAsyncPatternProperty); } set { SetValue(UseAsyncPatternProperty, value); } }
		public static readonly DependencyProperty UseAsyncPatternProperty = DependencyProperty.Register("UseAsyncPattern", typeof(bool), typeof(Method), new PropertyMetadata(false));

		public bool UseAwaitPattern { get { return (bool)GetValue(UseAwaitPatternProperty); } set { SetValue(UseAwaitPatternProperty, value); } }
		public static readonly DependencyProperty UseAwaitPatternProperty = DependencyProperty.Register("UseAwaitPattern", typeof(bool), typeof(Method), new PropertyMetadata(false));

		public bool IsInitiating { get { return (bool)GetValue(IsInitiatingProperty); } set { SetValue(IsInitiatingProperty, value); } }
		public static readonly DependencyProperty IsInitiatingProperty = DependencyProperty.Register("IsInitiating", typeof(bool), typeof(Method), new PropertyMetadata(false));

		public bool IsTerminating { get { return (bool)GetValue(IsTerminatingProperty); } set { SetValue(IsTerminatingProperty, value); } }
		public static readonly DependencyProperty IsTerminatingProperty = DependencyProperty.Register("IsTerminating", typeof(bool), typeof(Method), new PropertyMetadata(false));

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(Method));

		public bool IsRESTMethod { get { return (bool)GetValue(IsRESTMethodProperty); } set { SetValue(IsRESTMethodProperty, value); } }
		public static readonly DependencyProperty IsRESTMethodProperty = DependencyProperty.Register("IsRESTMethod", typeof(bool), typeof(Method), new PropertyMetadata(false, IsRESTMethodChangedCallback));

		private static void IsRESTMethodChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as Method;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue) == false) t.REST.Unregister();
			t.REST = Convert.ToBoolean(e.NewValue) ? new MethodREST(t) : null;
		}

		public MethodREST REST { get { return (MethodREST)GetValue(RESTProperty); } set { SetValue(RESTProperty, value); } }
		public static readonly DependencyProperty RESTProperty = DependencyProperty.Register("REST", typeof(MethodREST), typeof(Method));

		public ObservableCollection<MethodParameter> Parameters { get { return (ObservableCollection<MethodParameter>)GetValue(ParametersProperty); } set { SetValue(ParametersProperty, value); } }
		public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(ObservableCollection<MethodParameter>), typeof(Method));

		public Method()
		{
			Parameters = new ObservableCollection<MethodParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			Documentation = new Documentation { IsMethod = true };
		}

		public Method(string Name, Service Owner) : base(Name, Owner)
		{
			Parameters = new ObservableCollection<MethodParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			ReturnType = new DataType(PrimitiveTypes.Void);
			Documentation = new Documentation { IsMethod = true };
		}

		public Method(DataType ReturnType, string Name, Service Owner) : base(Name, Owner)
		{
			Parameters = new ObservableCollection<MethodParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			this.ReturnType = ReturnType;
			if (ReturnType.Primitive == PrimitiveTypes.Void) IsOneWay = true;
			Documentation = new Documentation { IsMethod = true };
		}

		private void Parameters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			UpdateDeclaration();
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if(Parameters == null) return;
			if (e.Property == DeclarationProperty || e.Property == ClientDeclarationProperty) return;

			UpdateDeclaration();
		}

		internal void UpdateDeclaration()
		{
			var sb = new StringBuilder();
			foreach (MethodParameter p in Parameters)
				sb.AppendFormat("{0}, ", p);
			if (Parameters.Count > 0) sb.Remove(sb.Length - 2, 2);
			Declaration = string.Format("{0} {1}({2})", ReturnType, ServerName, sb);
			ClientDeclaration = string.Format("{0} {1}({2})", ReturnType, ClientName, sb);
		}

		public override IEnumerable<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			var results = new List<FindReplaceResult>();
			results.AddRange(base.FindReplace(Args));
			foreach (MethodParameter mp in Parameters)
				results.AddRange(mp.FindReplace(Args));
			return results;
		}

		public override void Replace(FindReplaceInfo Args, string Field)
		{
			foreach (MethodParameter mp in Parameters)
				mp.Replace(Args, Field);
		}
	}

	public class MethodParameter : DependencyObject
	{
		public Guid ID { get; set; }

		public DataType Type { get { return (DataType)GetValue(TypeProperty); } set { SetValue(TypeProperty, value); } }
		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(DataType), typeof(MethodParameter), new PropertyMetadata(TypeChangedCallback));

		private static void TypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs p)
		{
			var de = o as Operation;
			if (de == null) return;
			var nt = p.NewValue as DataType;
			if (nt == null) return;
			var ot = p.OldValue as DataType;
			if (ot == null) return;

			if (ot.TypeMode == DataTypeMode.Array && ot.CollectionGenericType.TypeMode == DataTypeMode.Primitive) de.Owner.RemoveKnownType(nt);
			if (nt.TypeMode == DataTypeMode.Array && nt.CollectionGenericType.TypeMode == DataTypeMode.Primitive) de.Owner.AddKnownType(nt);
			if (ot.TypeMode == DataTypeMode.Primitive && ot.Primitive == PrimitiveTypes.DateTimeOffset) de.Owner.RemoveKnownType(new DataType(PrimitiveTypes.DateTimeOffset));
			if (nt.TypeMode == DataTypeMode.Primitive && nt.Primitive == PrimitiveTypes.DateTimeOffset) de.Owner.AddKnownType(new DataType(PrimitiveTypes.DateTimeOffset));
		}

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value ?? "", @"")); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(MethodParameter));

		public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(MethodParameter));

		public bool IsRESTInvalid { get { return (bool)GetValue(IsRESTInvalidProperty); } set { SetValue(IsRESTInvalidProperty, value); } }
		public static readonly DependencyProperty IsRESTInvalidProperty = DependencyProperty.Register("IsRESTInvalid", typeof(bool), typeof(MethodParameter), new PropertyMetadata(false));

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(Enum));

		//Internal Use
		public Service Owner { get; set; }
		public Method Parent { get; set; }

		public MethodParameter ()
		{
			ID = Guid.NewGuid();
			Type = new DataType(PrimitiveTypes.String);
			IsHidden = false;
			Documentation = new Documentation {IsParameter = true};
		}

		public MethodParameter(DataType Type, string Name, Service Owner, Method Parent)
		{
			ID = Guid.NewGuid();
			this.Type = Type;
			this.Name = Name;
			IsHidden = false;
			this.Owner = Owner;
			this.Parent = Parent;
			Documentation = new Documentation { IsParameter = true };
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (Parent != null) Parent.UpdateDeclaration();
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", Type, Name);
		}

		public IEnumerable<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			var results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Service || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
					else
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
				}
				else
					if (!string.IsNullOrEmpty(Name)) if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));

				if (Args.ReplaceAll)
				{
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						else
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
					}
					else
						if (!string.IsNullOrEmpty(Name)) Name = Args.RegexSearch.Replace(Name, Args.Replace); 
				}
			}

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
		{
			if (!Args.ReplaceAll) return;
			if (Args.UseRegex == false)
			{
				if (Args.MatchCase == false)
					if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
				else
					if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
			}
			else
				if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
		}
	}

	public enum MethodRESTVerbs
	{
		GET,
		POST,
		PUT,
		DELETE,
	}

	public class MethodREST : DependencyObject
	{
		public Guid ID { get; private set; }

		public string RESTName { get { return (string)GetValue(RESTNameProperty); } set { SetValue(RESTNameProperty, value); } }
		public static readonly DependencyProperty RESTNameProperty = DependencyProperty.Register("RESTName", typeof(string), typeof(MethodREST), new PropertyMetadata(""));

		public string UriTemplate { get { return (string)GetValue(UriTemplateProperty); } set { SetValue(UriTemplateProperty, value); } }
		public static readonly DependencyProperty UriTemplateProperty = DependencyProperty.Register("UriTemplate", typeof(string), typeof(MethodREST), new PropertyMetadata("/"));

		public bool UseParameterFormat { get { return (bool)GetValue(UseParameterFormatProperty); } set { SetValue(UseParameterFormatProperty, value); } }
		public static readonly DependencyProperty UseParameterFormatProperty = DependencyProperty.Register("UseParameterFormat", typeof(bool), typeof(MethodREST), new PropertyMetadata(false, UseParameterFormatChangedCallback));

		private static void UseParameterFormatChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as MethodREST;
			if (t != null) t.UriTemplate = t.BuildUriTemplate();
		}

		public MethodRESTVerbs Method { get { return (MethodRESTVerbs)GetValue(MethodProperty); } set { SetValue(MethodProperty, value); } }
		public static readonly DependencyProperty MethodProperty = DependencyProperty.Register("Method", typeof(MethodRESTVerbs), typeof(MethodREST), new PropertyMetadata(MethodRESTVerbs.GET));

		public WebMessageBodyStyle BodyStyle { get { return (WebMessageBodyStyle)GetValue(BodyStyleProperty); } set { SetValue(BodyStyleProperty, value); } }
		public static readonly DependencyProperty BodyStyleProperty = DependencyProperty.Register("BodyStyle", typeof(WebMessageBodyStyle), typeof(MethodREST), new PropertyMetadata(WebMessageBodyStyle.Bare));

		public WebMessageFormat RequestFormat { get { return (WebMessageFormat)GetValue(RequestFormatProperty); } set { SetValue(RequestFormatProperty, value); } }
		public static readonly DependencyProperty RequestFormatProperty = DependencyProperty.Register("RequestFormat", typeof(WebMessageFormat), typeof(MethodREST), new PropertyMetadata(WebMessageFormat.Xml));

		public WebMessageFormat ResponseFormat { get { return (WebMessageFormat)GetValue(ResponseFormatProperty); } set { SetValue(ResponseFormatProperty, value); } }
		public static readonly DependencyProperty ResponseFormatProperty = DependencyProperty.Register("ResponseFormat", typeof(WebMessageFormat), typeof(MethodREST), new PropertyMetadata(WebMessageFormat.Xml));

		[IgnoreDataMember] private Method owner;
		public Method Owner { get { return owner; } set { owner = value; if (owner != null) owner.Parameters.CollectionChanged += Parameters_CollectionChanged; UriTemplate = BuildUriTemplate(); } }

		public MethodREST() { }		

		public MethodREST(Method Owner)
		{
			ID = new Guid();
			this.Owner = Owner;
			this.Owner.Parameters.CollectionChanged += Parameters_CollectionChanged;

			UriTemplate = BuildUriTemplate();
		}

		private void Parameters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			UriTemplate = BuildUriTemplate();
		}

		public void Unregister()
		{
			Owner.Parameters.CollectionChanged -= Parameters_CollectionChanged;
		}

		public string BuildUriTemplate()
		{
			var code = new StringBuilder();
			if (Owner.Parameters.Any(a => a.Type.TypeMode == DataTypeMode.Primitive || a.Type.TypeMode == DataTypeMode.Enum))
				code.AppendFormat(UseParameterFormat ? "{0}?" : "{0}", string.IsNullOrEmpty(RESTName) ? Owner.ServerName : RESTName);
			else
				return string.IsNullOrEmpty(RESTName) ? Owner.ServerName : RESTName;

			foreach(MethodParameter mp in Owner.Parameters.Where(a => a.Type.TypeMode == DataTypeMode.Primitive || a.Type.TypeMode == DataTypeMode.Enum))
			{
				if (mp.Type.TypeMode == DataTypeMode.Primitive && (mp.Type.Primitive == PrimitiveTypes.None || mp.Type.Primitive == PrimitiveTypes.ByteArray || mp.Type.Primitive == PrimitiveTypes.Void || mp.Type.Primitive == PrimitiveTypes.Object || mp.Type.Primitive == PrimitiveTypes.URI || mp.Type.Primitive == PrimitiveTypes.Version))
				{
					mp.IsRESTInvalid = true;
					continue;
				}

				code.AppendFormat(UseParameterFormat ? "&{0}={{{0}}}" : "/{0}", mp.Name);
			}

			Method = Owner.Parameters.Any(a => a.Type.TypeMode == DataTypeMode.Struct || a.Type.TypeMode == DataTypeMode.Class) ? (Method == MethodRESTVerbs.GET ? MethodRESTVerbs.POST : Method) : MethodRESTVerbs.GET;

			return code.Replace("?&", "?").ToString();
		}
	}
}