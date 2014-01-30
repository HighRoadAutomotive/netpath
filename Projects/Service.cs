using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;
using System.Transactions;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace NETPath.Projects
{
	public enum ServiceAsynchronyMode
	{
		Default = 0,
		Client = 1,
		Server = 2,
		Both = 3,
	}

	public enum TransactionFlowMode
	{
		None = 0,
		Allowed = 1,
		Mandatory = 2,
		NotAllowed = 3,
	}

	public class Service : DataType
	{
		public ObservableCollection<Operation> ServiceOperations { get { return (ObservableCollection<Operation>)GetValue(ServiceOperationsProperty); } set { SetValue(ServiceOperationsProperty, value); } }
		public static readonly DependencyProperty ServiceOperationsProperty = DependencyProperty.Register("ServiceOperations", typeof(ObservableCollection<Operation>), typeof(Service));

		public ObservableCollection<Operation> CallbackOperations { get { return (ObservableCollection<Operation>)GetValue(CallbackOperationsProperty); } set { SetValue(CallbackOperationsProperty, value); } }
		public static readonly DependencyProperty CallbackOperationsProperty = DependencyProperty.Register("CallbackOperations", typeof(ObservableCollection<Operation>), typeof(Service));

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

		//Service Behavior
		public AddressFilterMode SBAddressFilterMode { get { return (AddressFilterMode)GetValue(SBAddressFilterModeProperty); } set { SetValue(SBAddressFilterModeProperty, value); } }
		public static readonly DependencyProperty SBAddressFilterModeProperty = DependencyProperty.Register("SBAddressFilterMode", typeof(AddressFilterMode), typeof(Service), new PropertyMetadata(AddressFilterMode.Any));

		public bool SBAutomaticSessionShutdown { get { return (bool)GetValue(SBAutomaticSessionShutdownProperty); } set { SetValue(SBAutomaticSessionShutdownProperty, value); } }
		public static readonly DependencyProperty SBAutomaticSessionShutdownProperty = DependencyProperty.Register("SBAutomaticSessionShutdown", typeof(bool), typeof(Service), new PropertyMetadata(true));

		public ConcurrencyMode SBConcurrencyMode { get { return (ConcurrencyMode)GetValue(SBConcurrencyModeProperty); } set { SetValue(SBConcurrencyModeProperty, value); } }
		public static readonly DependencyProperty SBConcurrencyModeProperty = DependencyProperty.Register("SBConcurrencyMode", typeof(ConcurrencyMode), typeof(Service), new PropertyMetadata(ConcurrencyMode.Multiple));

		public bool SBEnsureOrderedDispatch { get { return (bool)GetValue(SBEnsureOrderedDispatchProperty); } set { SetValue(SBEnsureOrderedDispatchProperty, value); } }
		public static readonly DependencyProperty SBEnsureOrderedDispatchProperty = DependencyProperty.Register("SBEnsureOrderedDispatch", typeof(bool), typeof(Service), new PropertyMetadata(false));

		public bool SBIgnoreExtensionDataObject { get { return (bool)GetValue(SBIgnoreExtensionDataObjectProperty); } set { SetValue(SBIgnoreExtensionDataObjectProperty, value); } }
		public static readonly DependencyProperty SBIgnoreExtensionDataObjectProperty = DependencyProperty.Register("SBIgnoreExtensionDataObject", typeof(bool), typeof(Service), new PropertyMetadata(false));

		public bool SBIncludeExceptionDetailInFaults { get { return (bool)GetValue(SBIncludeExceptionDetailInFaultsProperty); } set { SetValue(SBIncludeExceptionDetailInFaultsProperty, value); } }
		public static readonly DependencyProperty SBIncludeExceptionDetailInFaultsProperty = DependencyProperty.Register("SBIncludeExceptionDetailInFaults", typeof(bool), typeof(Service), new PropertyMetadata(false));

		public InstanceContextMode SBInstanceContextMode { get { return (InstanceContextMode)GetValue(SBInstanceContextModeProperty); } set { SetValue(SBInstanceContextModeProperty, value); } }
		public static readonly DependencyProperty SBInstanceContextModeProperty = DependencyProperty.Register("SBInstanceContextMode", typeof(InstanceContextMode), typeof(Service), new PropertyMetadata(InstanceContextMode.PerSession));

		public int SBMaxItemsInObjectGraph { get { return (int)GetValue(SBMaxItemsInObjectGraphProperty); } set { SetValue(SBMaxItemsInObjectGraphProperty, value); } }
		public static readonly DependencyProperty SBMaxItemsInObjectGraphProperty = DependencyProperty.Register("SBMaxItemsInObjectGraph", typeof(int), typeof(Service), new PropertyMetadata(65536));

		public bool SBReleaseServiceInstanceOnTransactionComplete { get { return (bool)GetValue(SBReleaseServiceInstanceOnTransactionCompleteProperty); } set { SetValue(SBReleaseServiceInstanceOnTransactionCompleteProperty, value); } }
		public static readonly DependencyProperty SBReleaseServiceInstanceOnTransactionCompleteProperty = DependencyProperty.Register("SReleaseServiceInstanceOnTransactionComplete", typeof(bool), typeof(Service), new PropertyMetadata(true));

		public bool SBTransactionAutoCompleteOnSessionClose { get { return (bool)GetValue(SBTransactionAutoCompleteOnSessionCloseProperty); } set { SetValue(SBTransactionAutoCompleteOnSessionCloseProperty, value); } }
		public static readonly DependencyProperty SBTransactionAutoCompleteOnSessionCloseProperty = DependencyProperty.Register("SBTransactionAutoCompleteOnSessionClose", typeof(bool), typeof(Service), new PropertyMetadata(false));

		public IsolationLevel SBTransactionIsolationLevel { get { return (IsolationLevel)GetValue(SBTransactionIsolationLevelProperty); } set { SetValue(SBTransactionIsolationLevelProperty, value); } }
		public static readonly DependencyProperty SBTransactionIsolationLevelProperty = DependencyProperty.Register("SBTransactionIsolationLevel", typeof(IsolationLevel), typeof(Service), new PropertyMetadata(IsolationLevel.Unspecified));

		public TimeSpan SBTransactionTimeout { get { return (TimeSpan)GetValue(SBTransactionTimeoutProperty); } set { SetValue(SBTransactionTimeoutProperty, value); } }
		public static readonly DependencyProperty SBTransactionTimeoutProperty = DependencyProperty.Register("SBTransactionTimeout", typeof(TimeSpan), typeof(Service), new PropertyMetadata(new TimeSpan(0, 0, 00)));

		public bool SBUseSynchronizationContext { get { return (bool)GetValue(SBUseSynchronizationContextProperty); } set { SetValue(SBUseSynchronizationContextProperty, value); } }
		public static readonly DependencyProperty SBUseSynchronizationContextProperty = DependencyProperty.Register("SBUseSynchronizationContext", typeof(bool), typeof(Service), new PropertyMetadata(true));

		public bool SBValidateMustUnderstand { get { return (bool)GetValue(SBValidateMustUnderstandProperty); } set { SetValue(SBValidateMustUnderstandProperty, value); } }
		public static readonly DependencyProperty SBValidateMustUnderstandProperty = DependencyProperty.Register("SBValidateMustUnderstand", typeof(bool), typeof(Service), new PropertyMetadata(true));

		//Callback Behavior
		public bool CBAutomaticSessionShutdown { get { return (bool)GetValue(CBAutomaticSessionShutdownProperty); } set { SetValue(CBAutomaticSessionShutdownProperty, value); } }
		public static readonly DependencyProperty CBAutomaticSessionShutdownProperty = DependencyProperty.Register("CBAutomaticSessionShutdown", typeof(bool), typeof(Service), new PropertyMetadata(true));

		public ConcurrencyMode CBConcurrencyMode { get { return (ConcurrencyMode)GetValue(CBConcurrencyModeProperty); } set { SetValue(CBConcurrencyModeProperty, value); } }
		public static readonly DependencyProperty CBConcurrencyModeProperty = DependencyProperty.Register("CBConcurrencyMode", typeof(ConcurrencyMode), typeof(Service), new PropertyMetadata(ConcurrencyMode.Multiple));

		public bool CBIgnoreExtensionDataObject { get { return (bool)GetValue(CBIgnoreExtensionDataObjectProperty); } set { SetValue(CBIgnoreExtensionDataObjectProperty, value); } }
		public static readonly DependencyProperty CBIgnoreExtensionDataObjectProperty = DependencyProperty.Register("CBIgnoreExtensionDataObject", typeof(bool), typeof(Service), new PropertyMetadata(false));

		public bool CBIncludeExceptionDetailInFaults { get { return (bool)GetValue(CBIncludeExceptionDetailInFaultsProperty); } set { SetValue(CBIncludeExceptionDetailInFaultsProperty, value); } }
		public static readonly DependencyProperty CBIncludeExceptionDetailInFaultsProperty = DependencyProperty.Register("CBIncludeExceptionDetailInFaults", typeof(bool), typeof(Service), new PropertyMetadata(false));

		public int CBMaxItemsInObjectGraph { get { return (int)GetValue(CBMaxItemsInObjectGraphProperty); } set { SetValue(CBMaxItemsInObjectGraphProperty, value); } }
		public static readonly DependencyProperty CBMaxItemsInObjectGraphProperty = DependencyProperty.Register("CBMaxItemsInObjectGraph", typeof(int), typeof(Service), new PropertyMetadata(65536));

		public IsolationLevel CBTransactionIsolationLevel { get { return (IsolationLevel)GetValue(CBTransactionIsolationLevelProperty); } set { SetValue(CBTransactionIsolationLevelProperty, value); } }
		public static readonly DependencyProperty CBTransactionIsolationLevelProperty = DependencyProperty.Register("CBTransactionIsolationLevel", typeof(IsolationLevel), typeof(Service), new PropertyMetadata(IsolationLevel.Unspecified));

		public TimeSpan CBTransactionTimeout { get { return (TimeSpan)GetValue(CBTransactionTimeoutProperty); } set { SetValue(CBTransactionTimeoutProperty, value); } }
		public static readonly DependencyProperty CBTransactionTimeoutProperty = DependencyProperty.Register("CBTransactionTimeout", typeof(TimeSpan), typeof(Service), new PropertyMetadata(new TimeSpan(0, 0, 00)));

		public bool CBUseSynchronizationContext { get { return (bool)GetValue(CBUseSynchronizationContextProperty); } set { SetValue(CBUseSynchronizationContextProperty, value); } }
		public static readonly DependencyProperty CBUseSynchronizationContextProperty = DependencyProperty.Register("CBUseSynchronizationContext", typeof(bool), typeof(Service), new PropertyMetadata(true));

		public bool CBValidateMustUnderstand { get { return (bool)GetValue(CBValidateMustUnderstandProperty); } set { SetValue(CBValidateMustUnderstandProperty, value); } }
		public static readonly DependencyProperty CBValidateMustUnderstandProperty = DependencyProperty.Register("CBValidateMustUnderstand", typeof(bool), typeof(Service), new PropertyMetadata(true));

		//System
		[IgnoreDataMember]
		public bool HasCallback { get { return CallbackOperations.Count > 0; } }
		[IgnoreDataMember]
		public bool HasCallbackOperations { get { return CallbackOperations.Count > 0 || ServiceOperations.Count(a => a.GetType() == typeof(DataChangeMethod)) > 0; } }
		[IgnoreDataMember]
		public bool HasDCMOperations { get { return ServiceOperations.Count(a => a.GetType() == typeof(DataChangeMethod)) > 0; } }

		public Service()
			: base(DataTypeMode.Class)
		{
			ServiceOperations = new ObservableCollection<Operation>();
			CallbackOperations = new ObservableCollection<Operation>();
			ServiceDocumentation = new Documentation { IsClass = true };
			CallbackDocumentation = new Documentation { IsClass = true };
		}

		public Service(string Name, Namespace Parent)
			: base(DataTypeMode.Class)
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
			KnownTypes.Add(Type);
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

		public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(Operation), new PropertyMetadata(false));

		public ProtectionLevel ProtectionLevel { get { return (ProtectionLevel)GetValue(ProtectionLevelProperty); } set { SetValue(ProtectionLevelProperty, value); } }
		public static readonly DependencyProperty ProtectionLevelProperty = DependencyProperty.Register("ProtectionLevel", typeof(ProtectionLevel), typeof(Operation), new PropertyMetadata(ProtectionLevel.None));

		[IgnoreDataMember]
		public string Declaration { get { return (string)GetValue(DeclarationProperty); } protected set { SetValue(DeclarationPropertyKey, value); } }
		private static readonly DependencyPropertyKey DeclarationPropertyKey = DependencyProperty.RegisterReadOnly("Declaration", typeof(string), typeof(Operation), new PropertyMetadata(""));
		public static readonly DependencyProperty DeclarationProperty = DeclarationPropertyKey.DependencyProperty;

		[IgnoreDataMember]
		public string ClientDeclaration { get { return (string)GetValue(ClientDeclarationProperty); } protected set { SetValue(ClientDeclarationPropertyKey, value); } }
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

		internal abstract void UpdateDeclaration();

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

		public Property(DataType ReturnType, string Name, Service Owner)
			: base(Name, Owner)
		{
			this.ReturnType = ReturnType;
			Documentation = new Documentation { IsProperty = true };
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == DeclarationProperty || e.Property == ClientDeclarationProperty) return;
			Declaration = string.Format("{0} {1}{{ get; {2}}}", ReturnType, ServerName, IsReadOnly ? "" : "set; ");
			ClientDeclaration = string.Format("{0} {1}{{ get; {2}}}", ReturnType, ClientName, IsReadOnly ? "" : "set; ");
		}

		internal override void UpdateDeclaration()
		{
			Declaration = string.Format("{0} {1}{{ get; {2}}}", ReturnType, ServerName, IsReadOnly ? "" : "set; ");
			ClientDeclaration = string.Format("{0} {1}{{ get; {2}}}", ReturnType, ClientName, IsReadOnly ? "" : "set; ");
		}
	}

	public class Method : Operation
	{
		public bool UseServerSyncPattern { get { return (bool)GetValue(UseServerSyncPatternProperty); } set { SetValue(UseServerSyncPatternProperty, value); } }
		public static readonly DependencyProperty UseServerSyncPatternProperty = DependencyProperty.Register("UseServerSyncPattern", typeof(bool), typeof(Method), new PropertyMetadata(false, UseServerSyncPatternChangedCallback));

		private static void UseServerSyncPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as Method;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseServerAwaitPattern = false;
			else t.UseServerAwaitPattern = true;
		}

		public bool UseServerAwaitPattern { get { return (bool)GetValue(UseServerAwaitPatternProperty); } set { SetValue(UseServerAwaitPatternProperty, value); } }
		public static readonly DependencyProperty UseServerAwaitPatternProperty = DependencyProperty.Register("UseServerAwaitPattern", typeof(bool), typeof(Method), new PropertyMetadata(true, UseServerAwaitPatternChangedCallback));

		private static void UseServerAwaitPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as Method;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseServerSyncPattern = false;
			else t.UseServerSyncPattern = true;
		}

		public bool UseClientSyncPattern { get { return (bool)GetValue(UseClientSyncPatternProperty); } set { SetValue(UseClientSyncPatternProperty, value); } }
		public static readonly DependencyProperty UseClientSyncPatternProperty = DependencyProperty.Register("UseClientSyncPattern", typeof(bool), typeof(Method), new PropertyMetadata(false, UseClientSyncPatternChangedCallback));

		private static void UseClientSyncPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as Method;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseClientAwaitPattern = false;
			else t.UseClientAwaitPattern = true;
		}

		public bool UseClientAwaitPattern { get { return (bool)GetValue(UseClientAwaitPatternProperty); } set { SetValue(UseClientAwaitPatternProperty, value); } }
		public static readonly DependencyProperty UseClientAwaitPatternProperty = DependencyProperty.Register("UseClientAwaitPattern", typeof(bool), typeof(Method), new PropertyMetadata(true, UseClientAwaitPatternChangedCallback));

		private static void UseClientAwaitPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as Method;
			if (t == null) return;

			if (t.IsTerminating)
			{
				t.UseClientSyncPattern = true;
				t.UseClientAwaitPattern = false;
				return;
			}
			if ((bool) e.NewValue) t.UseClientSyncPattern = false;
			else t.UseClientSyncPattern = true;
		}

		public bool IsInitiating { get { return (bool)GetValue(IsInitiatingProperty); } set { SetValue(IsInitiatingProperty, value); } }
		public static readonly DependencyProperty IsInitiatingProperty = DependencyProperty.Register("IsInitiating", typeof(bool), typeof(Method), new PropertyMetadata(false));

		public bool IsTerminating { get { return (bool)GetValue(IsTerminatingProperty); } set { SetValue(IsTerminatingProperty, value); } }
		public static readonly DependencyProperty IsTerminatingProperty = DependencyProperty.Register("IsTerminating", typeof(bool), typeof(Method), new PropertyMetadata(false, IsTerminatingChangedCallback));

		private static void IsTerminatingChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as Method;
			if (t == null) return;

			t.UseClientSyncPattern = true;
			t.UseClientAwaitPattern = false;
		}

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(Method));

		public ObservableCollection<MethodParameter> Parameters { get { return (ObservableCollection<MethodParameter>)GetValue(ParametersProperty); } set { SetValue(ParametersProperty, value); } }
		public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(ObservableCollection<MethodParameter>), typeof(Method));

		// Behavior
		public bool HasOperationBehavior { get { return (bool)GetValue(HasOperationBehaviorProperty); } set { SetValue(HasOperationBehaviorProperty, value); } }
		public static readonly DependencyProperty HasOperationBehaviorProperty = DependencyProperty.Register("HasOperationBehavior", typeof(bool), typeof(Method), new PropertyMetadata(false));

		public bool OBAutoDisposeParameters { get { return (bool)GetValue(OBAutoDisposeParametersProperty); } set { SetValue(OBAutoDisposeParametersProperty, value); } }
		public static readonly DependencyProperty OBAutoDisposeParametersProperty = DependencyProperty.Register("OBAutoDisposeParameters", typeof(bool), typeof(Method), new PropertyMetadata(true));

		public ImpersonationOption OBImpersonation { get { return (ImpersonationOption)GetValue(OBImpersonationProperty); } set { SetValue(OBImpersonationProperty, value); } }
		public static readonly DependencyProperty OBImpersonationProperty = DependencyProperty.Register("OBImpersonation", typeof(ImpersonationOption), typeof(Method), new PropertyMetadata(ImpersonationOption.NotAllowed));

		public ReleaseInstanceMode OBReleaseInstanceMode { get { return (ReleaseInstanceMode)GetValue(OBReleaseInstanceModeProperty); } set { SetValue(OBReleaseInstanceModeProperty, value); } }
		public static readonly DependencyProperty OBReleaseInstanceModeProperty = DependencyProperty.Register("OBReleaseInstanceMode", typeof(ReleaseInstanceMode), typeof(Method), new PropertyMetadata(ReleaseInstanceMode.None));

		public bool OBTransactionAutoComplete { get { return (bool)GetValue(OBTransactionAutoCompleteProperty); } set { SetValue(OBTransactionAutoCompleteProperty, value); } }
		public static readonly DependencyProperty OBTransactionAutoCompleteProperty = DependencyProperty.Register("OBTransactionAutoComplete", typeof(bool), typeof(Method), new PropertyMetadata(true));

		public bool OBTransactionScopeRequired { get { return (bool)GetValue(OBTransactionScopeRequiredProperty); } set { SetValue(OBTransactionScopeRequiredProperty, value); } }
		public static readonly DependencyProperty OBTransactionScopeRequiredProperty = DependencyProperty.Register("OBTransactionScopeRequired", typeof(bool), typeof(Method), new PropertyMetadata(false));

		public TransactionFlowMode OBTransactionFlowMode { get { return (TransactionFlowMode)GetValue(OBTransactionFlowModeProperty); } set { SetValue(OBTransactionFlowModeProperty, value); } }
		public static readonly DependencyProperty OBTransactionFlowModeProperty = DependencyProperty.Register("OBTransactionFlowMode", typeof(TransactionFlowMode), typeof(Method), new PropertyMetadata(TransactionFlowMode.None));

		// REST
		public bool IsRESTMethod { get { return (bool)GetValue(IsRESTMethodProperty); } set { SetValue(IsRESTMethodProperty, value); } }
		public static readonly DependencyProperty IsRESTMethodProperty = DependencyProperty.Register("IsRESTMethod", typeof(bool), typeof(Method), new PropertyMetadata(false, IsRESTMethodChangedCallback));

		private static void IsRESTMethodChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as Method;
			if (t == null) return;

			t.REST = Convert.ToBoolean(e.NewValue) ? new MethodREST(t) : null;
		}

		public MethodREST REST { get { return (MethodREST)GetValue(RESTProperty); } set { SetValue(RESTProperty, value); } }
		public static readonly DependencyProperty RESTProperty = DependencyProperty.Register("REST", typeof(MethodREST), typeof(Method));

		public Method()
		{
			Parameters = new ObservableCollection<MethodParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			Documentation = new Documentation { IsMethod = true };
		}

		public Method(string Name, Service Owner)
			: base(Name, Owner)
		{
			Parameters = new ObservableCollection<MethodParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			ReturnType = new DataType(PrimitiveTypes.Void);
			Documentation = new Documentation { IsMethod = true };
		}

		public Method(DataType ReturnType, string Name, Service Owner)
			: base(Name, Owner)
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

			if (Parameters == null) return;
			if (e.Property == DeclarationProperty || e.Property == ClientDeclarationProperty) return;

			UpdateDeclaration();
		}

		internal override void UpdateDeclaration()
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

	public class Callback : Operation
	{
		public bool UseServerSyncPattern { get { return (bool)GetValue(UseServerSyncPatternProperty); } set { SetValue(UseServerSyncPatternProperty, value); } }
		public static readonly DependencyProperty UseServerSyncPatternProperty = DependencyProperty.Register("UseServerSyncPattern", typeof(bool), typeof(Callback), new PropertyMetadata(false, UseServerSyncPatternChangedCallback));

		private static void UseServerSyncPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as Callback;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseServerAwaitPattern = false;
			else t.UseServerAwaitPattern = true;
		}

		public bool UseServerAwaitPattern { get { return (bool)GetValue(UseServerAwaitPatternProperty); } set { SetValue(UseServerAwaitPatternProperty, value); } }
		public static readonly DependencyProperty UseServerAwaitPatternProperty = DependencyProperty.Register("UseServerAwaitPattern", typeof(bool), typeof(Callback), new PropertyMetadata(true, UseServerAwaitPatternChangedCallback));

		private static void UseServerAwaitPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as Callback;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseServerSyncPattern = false;
			else t.UseServerSyncPattern = true;
		}

		public bool UseClientSyncPattern { get { return (bool)GetValue(UseClientSyncPatternProperty); } set { SetValue(UseClientSyncPatternProperty, value); } }
		public static readonly DependencyProperty UseClientSyncPatternProperty = DependencyProperty.Register("UseClientSyncPattern", typeof(bool), typeof(Callback), new PropertyMetadata(false, UseClientSyncPatternChangedCallback));

		private static void UseClientSyncPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as Callback;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseClientAwaitPattern = false;
			else t.UseClientAwaitPattern = true;
		}

		public bool UseClientAwaitPattern { get { return (bool)GetValue(UseClientAwaitPatternProperty); } set { SetValue(UseClientAwaitPatternProperty, value); } }
		public static readonly DependencyProperty UseClientAwaitPatternProperty = DependencyProperty.Register("UseClientAwaitPattern", typeof(bool), typeof(Callback), new PropertyMetadata(true, UseClientAwaitPatternChangedCallback));

		private static void UseClientAwaitPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as Callback;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseClientSyncPattern = false;
			else t.UseClientSyncPattern = true;
		}

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(Callback));

		public ObservableCollection<MethodParameter> Parameters { get { return (ObservableCollection<MethodParameter>)GetValue(ParametersProperty); } set { SetValue(ParametersProperty, value); } }
		public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(ObservableCollection<MethodParameter>), typeof(Callback));

		// Behavior
		public bool HasOperationBehavior { get { return (bool)GetValue(HasOperationBehaviorProperty); } set { SetValue(HasOperationBehaviorProperty, value); } }
		public static readonly DependencyProperty HasOperationBehaviorProperty = DependencyProperty.Register("HasOperationBehavior", typeof(bool), typeof(Callback), new PropertyMetadata(false));

		public bool OBAutoDisposeParameters { get { return (bool)GetValue(OBAutoDisposeParametersProperty); } set { SetValue(OBAutoDisposeParametersProperty, value); } }
		public static readonly DependencyProperty OBAutoDisposeParametersProperty = DependencyProperty.Register("OBAutoDisposeParameters", typeof(bool), typeof(Callback), new PropertyMetadata(true));

		public ImpersonationOption OBImpersonation { get { return (ImpersonationOption)GetValue(OBImpersonationProperty); } set { SetValue(OBImpersonationProperty, value); } }
		public static readonly DependencyProperty OBImpersonationProperty = DependencyProperty.Register("OBImpersonation", typeof(ImpersonationOption), typeof(Callback), new PropertyMetadata(ImpersonationOption.NotAllowed));

		public ReleaseInstanceMode OBReleaseInstanceMode { get { return (ReleaseInstanceMode)GetValue(OBReleaseInstanceModeProperty); } set { SetValue(OBReleaseInstanceModeProperty, value); } }
		public static readonly DependencyProperty OBReleaseInstanceModeProperty = DependencyProperty.Register("OBReleaseInstanceMode", typeof(ReleaseInstanceMode), typeof(Callback), new PropertyMetadata(ReleaseInstanceMode.None));

		public bool OBTransactionAutoComplete { get { return (bool)GetValue(OBTransactionAutoCompleteProperty); } set { SetValue(OBTransactionAutoCompleteProperty, value); } }
		public static readonly DependencyProperty OBTransactionAutoCompleteProperty = DependencyProperty.Register("OBTransactionAutoComplete", typeof(bool), typeof(Callback), new PropertyMetadata(true));

		public bool OBTransactionScopeRequired { get { return (bool)GetValue(OBTransactionScopeRequiredProperty); } set { SetValue(OBTransactionScopeRequiredProperty, value); } }
		public static readonly DependencyProperty OBTransactionScopeRequiredProperty = DependencyProperty.Register("OBTransactionScopeRequired", typeof(bool), typeof(Callback), new PropertyMetadata(false));

		public TransactionFlowMode OBTransactionFlowMode { get { return (TransactionFlowMode)GetValue(OBTransactionFlowModeProperty); } set { SetValue(OBTransactionFlowModeProperty, value); } }
		public static readonly DependencyProperty OBTransactionFlowModeProperty = DependencyProperty.Register("OBTransactionFlowMode", typeof(TransactionFlowMode), typeof(Callback), new PropertyMetadata(TransactionFlowMode.None));

		public Callback()
		{
			Parameters = new ObservableCollection<MethodParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			Documentation = new Documentation { IsMethod = true };
		}

		public Callback(string Name, Service Owner)
			: base(Name, Owner)
		{
			Parameters = new ObservableCollection<MethodParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			ReturnType = new DataType(PrimitiveTypes.Void);
			Documentation = new Documentation { IsMethod = true };
		}

		public Callback(DataType ReturnType, string Name, Service Owner)
			: base(Name, Owner)
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

			if (Parameters == null) return;
			if (e.Property == DeclarationProperty || e.Property == ClientDeclarationProperty) return;

			UpdateDeclaration();
		}

		internal override void UpdateDeclaration()
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

	public class DataChangeMethod : Operation
	{
		public bool UseServerSyncPattern { get { return (bool)GetValue(UseServerSyncPatternProperty); } set { SetValue(UseServerSyncPatternProperty, value); } }
		public static readonly DependencyProperty UseServerSyncPatternProperty = DependencyProperty.Register("UseServerSyncPattern", typeof(bool), typeof(DataChangeMethod), new PropertyMetadata(false, UseServerSyncPatternChangedCallback));

		private static void UseServerSyncPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataChangeMethod;
			if (t == null) return;

			if ((bool) e.NewValue) t.UseServerAwaitPattern = false;
		}

		public bool UseServerAwaitPattern { get { return (bool)GetValue(UseServerAwaitPatternProperty); } set { SetValue(UseServerAwaitPatternProperty, value); } }
		public static readonly DependencyProperty UseServerAwaitPatternProperty = DependencyProperty.Register("UseServerAwaitPattern", typeof(bool), typeof(DataChangeMethod), new PropertyMetadata(true, UseServerAwaitPatternChangedCallback));

		private static void UseServerAwaitPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataChangeMethod;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseServerSyncPattern = false;
		}

		public bool UseClientSyncPattern { get { return (bool)GetValue(UseClientSyncPatternProperty); } set { SetValue(UseClientSyncPatternProperty, value); } }
		public static readonly DependencyProperty UseClientSyncPatternProperty = DependencyProperty.Register("UseClientSyncPattern", typeof(bool), typeof(DataChangeMethod), new PropertyMetadata(false, UseClientSyncPatternChangedCallback));

		private static void UseClientSyncPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataChangeMethod;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseClientAwaitPattern = false;
		}

		public bool UseClientAwaitPattern { get { return (bool)GetValue(UseClientAwaitPatternProperty); } set { SetValue(UseClientAwaitPatternProperty, value); } }
		public static readonly DependencyProperty UseClientAwaitPatternProperty = DependencyProperty.Register("UseClientAwaitPattern", typeof(bool), typeof(DataChangeMethod), new PropertyMetadata(true, UseClientAwaitPatternChangedCallback));

		private static void UseClientAwaitPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataChangeMethod;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseClientSyncPattern = false;
		}

		public bool UseTPLForCallbacks { get { return (bool)GetValue(UseTPLForCallbacksProperty); } set { SetValue(UseTPLForCallbacksProperty, value); } }
		public static readonly DependencyProperty UseTPLForCallbacksProperty = DependencyProperty.Register("UseTPLForCallbacks", typeof(bool), typeof(DataChangeMethod), new PropertyMetadata(true));

		public bool GenerateGetFunction { get { return (bool)GetValue(GenerateGetFunctionProperty); } set { SetValue(GenerateGetFunctionProperty, value); } }
		public static readonly DependencyProperty GenerateGetFunctionProperty = DependencyProperty.Register("GenerateGetFunction", typeof(bool), typeof(DataChangeMethod), new PropertyMetadata(false, GenerateGetFunctionFunctionChangedCallback));

		private static void GenerateGetFunctionFunctionChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataChangeMethod;
			if (t == null) return;

			if ((bool)e.NewValue) return;
			t.GetParameters.Clear();
		}

		public bool GenerateOpenCloseFunction { get { return (bool)GetValue(GenerateOpenCloseFunctionProperty); } set { SetValue(GenerateOpenCloseFunctionProperty, value); } }
		public static readonly DependencyProperty GenerateOpenCloseFunctionProperty = DependencyProperty.Register("GenerateOpenCloseFunction", typeof(bool), typeof(DataChangeMethod), new PropertyMetadata(true, GenerateOpenCloseFunctionChangedCallback));

		private static void GenerateOpenCloseFunctionChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataChangeMethod;
			if (t == null) return;

			if ((bool)e.NewValue) return;
			t.OpenParameters.Clear();
			t.CloseParameters.Clear();
		}

		public bool GenerateNewDeleteFunction { get { return (bool)GetValue(GenerateNewDeleteFunctionProperty); } set { SetValue(GenerateNewDeleteFunctionProperty, value); } }
		public static readonly DependencyProperty GenerateNewDeleteFunctionProperty = DependencyProperty.Register("GenerateNewDeleteFunction", typeof(bool), typeof(DataChangeMethod), new PropertyMetadata(true, GenerateNewDeleteFunctionChangedCallback));

		private static void GenerateNewDeleteFunctionChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataChangeMethod;
			if (t == null) return;

			if ((bool)e.NewValue) return;
			t.NewParameters.Clear();
			t.DeleteParameters.Clear();
		}

		public bool IsReadOnly { get { return (bool)GetValue(IsReadOnlyProperty); } set { SetValue(IsReadOnlyProperty, value); } }
		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(DataChangeMethod), new PropertyMetadata(false, IsReadOnlyChangedCallback));

		private static void IsReadOnlyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataChangeMethod;
			if (t == null) return;

			if (!(bool)e.NewValue) return;
			t.GenerateNewDeleteFunction = false;
		}

		public ObservableCollection<MethodParameter> GetParameters { get { return (ObservableCollection<MethodParameter>)GetValue(GetParametersProperty); } set { SetValue(GetParametersProperty, value); } }
		public static readonly DependencyProperty GetParametersProperty = DependencyProperty.Register("GetParameters", typeof(ObservableCollection<MethodParameter>), typeof(DataChangeMethod));

		public ObservableCollection<MethodParameter> OpenParameters { get { return (ObservableCollection<MethodParameter>)GetValue(OpenParametersProperty); } set { SetValue(OpenParametersProperty, value); } }
		public static readonly DependencyProperty OpenParametersProperty = DependencyProperty.Register("OpenParameters", typeof(ObservableCollection<MethodParameter>), typeof(DataChangeMethod));

		public ObservableCollection<MethodParameter> CloseParameters { get { return (ObservableCollection<MethodParameter>)GetValue(CloseParametersProperty); } set { SetValue(CloseParametersProperty, value); } }
		public static readonly DependencyProperty CloseParametersProperty = DependencyProperty.Register("CloseParameters", typeof(ObservableCollection<MethodParameter>), typeof(DataChangeMethod));

		public ObservableCollection<MethodParameter> NewParameters { get { return (ObservableCollection<MethodParameter>)GetValue(NewParametersProperty); } set { SetValue(NewParametersProperty, value); } }
		public static readonly DependencyProperty NewParametersProperty = DependencyProperty.Register("NewParameters", typeof(ObservableCollection<MethodParameter>), typeof(DataChangeMethod));

		public ObservableCollection<MethodParameter> DeleteParameters { get { return (ObservableCollection<MethodParameter>)GetValue(DeleteParametersProperty); } set { SetValue(DeleteParametersProperty, value); } }
		public static readonly DependencyProperty DeleteParametersProperty = DependencyProperty.Register("DeleteParameters", typeof(ObservableCollection<MethodParameter>), typeof(DataChangeMethod));

		public Documentation GetDocumentation { get { return (Documentation)GetValue(GetDocumentationProperty); } set { SetValue(GetDocumentationProperty, value); } }
		public static readonly DependencyProperty GetDocumentationProperty = DependencyProperty.Register("GetDocumentation", typeof(Documentation), typeof(DataChangeMethod));

		public Documentation OpenDocumentation { get { return (Documentation)GetValue(OpenDocumentationProperty); } set { SetValue(OpenDocumentationProperty, value); } }
		public static readonly DependencyProperty OpenDocumentationProperty = DependencyProperty.Register("OpenDocumentation", typeof(Documentation), typeof(DataChangeMethod));

		public Documentation CloseDocumentation { get { return (Documentation)GetValue(CloseDocumentationProperty); } set { SetValue(CloseDocumentationProperty, value); } }
		public static readonly DependencyProperty CloseDocumentationProperty = DependencyProperty.Register("CloseDocumentation", typeof(Documentation), typeof(DataChangeMethod));

		public Documentation NewDocumentation { get { return (Documentation)GetValue(NewDocumentationProperty); } set { SetValue(NewDocumentationProperty, value); } }
		public static readonly DependencyProperty NewDocumentationProperty = DependencyProperty.Register("NewDocumentation", typeof(Documentation), typeof(DataChangeMethod));

		public Documentation DeleteDocumentation { get { return (Documentation)GetValue(DeleteDocumentationProperty); } set { SetValue(DeleteDocumentationProperty, value); } }
		public static readonly DependencyProperty DeleteDocumentationProperty = DependencyProperty.Register("DeleteDocumentation", typeof(Documentation), typeof(DataChangeMethod));

		public DataChangeMethod()
		{
			GetParameters = new ObservableCollection<MethodParameter>();
			OpenParameters = new ObservableCollection<MethodParameter>();
			CloseParameters = new ObservableCollection<MethodParameter>();
			NewParameters = new ObservableCollection<MethodParameter>();
			DeleteParameters = new ObservableCollection<MethodParameter>();
			ReturnType = new DataType(PrimitiveTypes.Void);
			GetDocumentation = new Documentation { IsMethod = true };
			OpenDocumentation = new Documentation { IsMethod = true };
			CloseDocumentation = new Documentation { IsMethod = true };
			NewDocumentation = new Documentation { IsMethod = true };
			DeleteDocumentation = new Documentation { IsMethod = true };
		}

		public DataChangeMethod(string Name, Service Owner)
			: base(Name, Owner)
		{
			GetParameters = new ObservableCollection<MethodParameter>();
			OpenParameters = new ObservableCollection<MethodParameter>();
			CloseParameters = new ObservableCollection<MethodParameter>();
			NewParameters = new ObservableCollection<MethodParameter>();
			DeleteParameters = new ObservableCollection<MethodParameter>();
			ReturnType = new DataType(PrimitiveTypes.Void);
			GetDocumentation = new Documentation { IsMethod = true };
			OpenDocumentation = new Documentation { IsMethod = true };
			CloseDocumentation = new Documentation { IsMethod = true };
			NewDocumentation = new Documentation { IsMethod = true };
			DeleteDocumentation = new Documentation { IsMethod = true };
		}

		public DataChangeMethod(DataType ReturnType, string Name, Service Owner)
			: base(Name, Owner)
		{
			GetParameters = new ObservableCollection<MethodParameter>();
			OpenParameters = new ObservableCollection<MethodParameter>();
			CloseParameters = new ObservableCollection<MethodParameter>();
			NewParameters = new ObservableCollection<MethodParameter>();
			DeleteParameters = new ObservableCollection<MethodParameter>();
			this.ReturnType = ReturnType;
			if (ReturnType.Primitive == PrimitiveTypes.Void) IsOneWay = true;
			DeleteParameters = new ObservableCollection<MethodParameter>();
			GetDocumentation = new Documentation { IsMethod = true };
			OpenDocumentation = new Documentation { IsMethod = true };
			CloseDocumentation = new Documentation { IsMethod = true };
			NewDocumentation = new Documentation { IsMethod = true };
			DeleteDocumentation = new Documentation { IsMethod = true };
		}

		public override IEnumerable<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			var results = new List<FindReplaceResult>();
			results.AddRange(base.FindReplace(Args));
			foreach (MethodParameter mp in GetParameters)
				results.AddRange(mp.FindReplace(Args));
			foreach (MethodParameter mp in OpenParameters)
				results.AddRange(mp.FindReplace(Args));
			foreach (MethodParameter mp in CloseParameters)
				results.AddRange(mp.FindReplace(Args));
			foreach (MethodParameter mp in NewParameters)
				results.AddRange(mp.FindReplace(Args));
			foreach (MethodParameter mp in DeleteParameters)
				results.AddRange(mp.FindReplace(Args));
			return results;
		}

		public override void Replace(FindReplaceInfo Args, string Field)
		{
			foreach (MethodParameter mp in GetParameters)
				mp.Replace(Args, Field);
			foreach (MethodParameter mp in OpenParameters)
				mp.Replace(Args, Field);
			foreach (MethodParameter mp in CloseParameters)
				mp.Replace(Args, Field);
			foreach (MethodParameter mp in NewParameters)
				mp.Replace(Args, Field);
			foreach (MethodParameter mp in DeleteParameters)
				mp.Replace(Args, Field);
		}

		internal override void UpdateDeclaration()
		{
			Declaration = string.Format("{0} {1}", ReturnType, ServerName);
			ClientDeclaration = string.Format("{0} {1}", ReturnType, ClientName);
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

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(MethodParameter));

		//Internal Use
		public Service Owner { get; set; }
		public Operation Parent { get; set; }

		public MethodParameter()
		{
			ID = Guid.NewGuid();
			Type = new DataType(PrimitiveTypes.String);
			IsHidden = false;
			Documentation = new Documentation { IsParameter = true };
		}

		public MethodParameter(DataType Type, string Name, Service Owner, Operation Parent)
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

	public class MethodREST : DependencyObject
	{
		public Guid ID { get; private set; }

		public string UriTemplate { get { return (string)GetValue(UriTemplateProperty); } set { SetValue(UriTemplateProperty, value); } }
		public static readonly DependencyProperty UriTemplateProperty = DependencyProperty.Register("UriTemplate", typeof(string), typeof(MethodREST), new PropertyMetadata("/"));

		public MethodRESTVerbs Method { get { return (MethodRESTVerbs)GetValue(MethodProperty); } set { SetValue(MethodProperty, value); } }
		public static readonly DependencyProperty MethodProperty = DependencyProperty.Register("Method", typeof(MethodRESTVerbs), typeof(MethodREST), new PropertyMetadata(MethodRESTVerbs.GET));

		public WebMessageBodyStyle BodyStyle { get { return (WebMessageBodyStyle)GetValue(BodyStyleProperty); } set { SetValue(BodyStyleProperty, value); } }
		public static readonly DependencyProperty BodyStyleProperty = DependencyProperty.Register("BodyStyle", typeof(WebMessageBodyStyle), typeof(MethodREST), new PropertyMetadata(WebMessageBodyStyle.Bare));

		public WebMessageFormat RequestFormat { get { return (WebMessageFormat)GetValue(RequestFormatProperty); } set { SetValue(RequestFormatProperty, value); } }
		public static readonly DependencyProperty RequestFormatProperty = DependencyProperty.Register("RequestFormat", typeof(WebMessageFormat), typeof(MethodREST), new PropertyMetadata(WebMessageFormat.Xml));

		public WebMessageFormat ResponseFormat { get { return (WebMessageFormat)GetValue(ResponseFormatProperty); } set { SetValue(ResponseFormatProperty, value); } }
		public static readonly DependencyProperty ResponseFormatProperty = DependencyProperty.Register("ResponseFormat", typeof(WebMessageFormat), typeof(MethodREST), new PropertyMetadata(WebMessageFormat.Xml));

		public Method Owner { get; private set; }

		public MethodREST() { }

		public MethodREST(Method Owner)
		{
			ID = new Guid();
			this.Owner = Owner;
		}

	}
}