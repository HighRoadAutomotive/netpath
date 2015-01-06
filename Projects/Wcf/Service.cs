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

namespace NETPath.Projects.Wcf
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

	public class WcfService : DataType
	{
		public ObservableCollection<WcfOperation> ServiceOperations { get { return (ObservableCollection<WcfOperation>)GetValue(ServiceOperationsProperty); } set { SetValue(ServiceOperationsProperty, value); } }
		public static readonly DependencyProperty ServiceOperationsProperty = DependencyProperty.Register("ServiceOperations", typeof(ObservableCollection<WcfOperation>), typeof(WcfService));

		public ObservableCollection<WcfOperation> CallbackOperations { get { return (ObservableCollection<WcfOperation>)GetValue(CallbackOperationsProperty); } set { SetValue(CallbackOperationsProperty, value); } }
		public static readonly DependencyProperty CallbackOperationsProperty = DependencyProperty.Register("CallbackOperations", typeof(ObservableCollection<WcfOperation>), typeof(WcfService));

		public ProtectionLevel ProtectionLevel { get { return (ProtectionLevel)GetValue(ProtectionLevelProperty); } set { SetValue(ProtectionLevelProperty, value); } }
		public static readonly DependencyProperty ProtectionLevelProperty = DependencyProperty.Register("ProtectionLevel", typeof(ProtectionLevel), typeof(WcfService));

		public System.ServiceModel.SessionMode SessionMode { get { return (System.ServiceModel.SessionMode)GetValue(SessionModeProperty); } set { SetValue(SessionModeProperty, value); } }
		public static readonly DependencyProperty SessionModeProperty = DependencyProperty.Register("SessionMode", typeof(System.ServiceModel.SessionMode), typeof(WcfService));

		public string ConfigurationName { get { return (string)GetValue(ConfigurationNameProperty); } set { SetValue(ConfigurationNameProperty, value); } }
		public static readonly DependencyProperty ConfigurationNameProperty = DependencyProperty.Register("ConfigurationName", typeof(string), typeof(WcfService));

		public Documentation ServiceDocumentation { get { return (Documentation)GetValue(ServiceDocumentationProperty); } set { SetValue(ServiceDocumentationProperty, value); } }
		public static readonly DependencyProperty ServiceDocumentationProperty = DependencyProperty.Register("ServiceDocumentation", typeof(Documentation), typeof(WcfService));

		public Documentation CallbackDocumentation { get { return (Documentation)GetValue(CallbackDocumentationProperty); } set { SetValue(CallbackDocumentationProperty, value); } }
		public static readonly DependencyProperty CallbackDocumentationProperty = DependencyProperty.Register("CallbackDocumentation", typeof(Documentation), typeof(WcfService));

		//Service Behavior
		public AddressFilterMode SBAddressFilterMode { get { return (AddressFilterMode)GetValue(SBAddressFilterModeProperty); } set { SetValue(SBAddressFilterModeProperty, value); } }
		public static readonly DependencyProperty SBAddressFilterModeProperty = DependencyProperty.Register("SBAddressFilterMode", typeof(AddressFilterMode), typeof(WcfService), new PropertyMetadata(AddressFilterMode.Any));

		public bool SBAutomaticSessionShutdown { get { return (bool)GetValue(SBAutomaticSessionShutdownProperty); } set { SetValue(SBAutomaticSessionShutdownProperty, value); } }
		public static readonly DependencyProperty SBAutomaticSessionShutdownProperty = DependencyProperty.Register("SBAutomaticSessionShutdown", typeof(bool), typeof(WcfService), new PropertyMetadata(true));

		public ConcurrencyMode SBConcurrencyMode { get { return (ConcurrencyMode)GetValue(SBConcurrencyModeProperty); } set { SetValue(SBConcurrencyModeProperty, value); } }
		public static readonly DependencyProperty SBConcurrencyModeProperty = DependencyProperty.Register("SBConcurrencyMode", typeof(ConcurrencyMode), typeof(WcfService), new PropertyMetadata(ConcurrencyMode.Multiple));

		public bool SBEnsureOrderedDispatch { get { return (bool)GetValue(SBEnsureOrderedDispatchProperty); } set { SetValue(SBEnsureOrderedDispatchProperty, value); } }
		public static readonly DependencyProperty SBEnsureOrderedDispatchProperty = DependencyProperty.Register("SBEnsureOrderedDispatch", typeof(bool), typeof(WcfService), new PropertyMetadata(false));

		public bool SBIgnoreExtensionDataObject { get { return (bool)GetValue(SBIgnoreExtensionDataObjectProperty); } set { SetValue(SBIgnoreExtensionDataObjectProperty, value); } }
		public static readonly DependencyProperty SBIgnoreExtensionDataObjectProperty = DependencyProperty.Register("SBIgnoreExtensionDataObject", typeof(bool), typeof(WcfService), new PropertyMetadata(false));

		public bool SBIncludeExceptionDetailInFaults { get { return (bool)GetValue(SBIncludeExceptionDetailInFaultsProperty); } set { SetValue(SBIncludeExceptionDetailInFaultsProperty, value); } }
		public static readonly DependencyProperty SBIncludeExceptionDetailInFaultsProperty = DependencyProperty.Register("SBIncludeExceptionDetailInFaults", typeof(bool), typeof(WcfService), new PropertyMetadata(false));

		public InstanceContextMode SBInstanceContextMode { get { return (InstanceContextMode)GetValue(SBInstanceContextModeProperty); } set { SetValue(SBInstanceContextModeProperty, value); } }
		public static readonly DependencyProperty SBInstanceContextModeProperty = DependencyProperty.Register("SBInstanceContextMode", typeof(InstanceContextMode), typeof(WcfService), new PropertyMetadata(InstanceContextMode.PerSession));

		public int SBMaxItemsInObjectGraph { get { return (int)GetValue(SBMaxItemsInObjectGraphProperty); } set { SetValue(SBMaxItemsInObjectGraphProperty, value); } }
		public static readonly DependencyProperty SBMaxItemsInObjectGraphProperty = DependencyProperty.Register("SBMaxItemsInObjectGraph", typeof(int), typeof(WcfService), new PropertyMetadata(65536));

		public bool SBReleaseServiceInstanceOnTransactionComplete { get { return (bool)GetValue(SBReleaseServiceInstanceOnTransactionCompleteProperty); } set { SetValue(SBReleaseServiceInstanceOnTransactionCompleteProperty, value); } }
		public static readonly DependencyProperty SBReleaseServiceInstanceOnTransactionCompleteProperty = DependencyProperty.Register("SReleaseServiceInstanceOnTransactionComplete", typeof(bool), typeof(WcfService), new PropertyMetadata(false));

		public bool SBTransactionAutoCompleteOnSessionClose { get { return (bool)GetValue(SBTransactionAutoCompleteOnSessionCloseProperty); } set { SetValue(SBTransactionAutoCompleteOnSessionCloseProperty, value); } }
		public static readonly DependencyProperty SBTransactionAutoCompleteOnSessionCloseProperty = DependencyProperty.Register("SBTransactionAutoCompleteOnSessionClose", typeof(bool), typeof(WcfService), new PropertyMetadata(false));

		public IsolationLevel SBTransactionIsolationLevel { get { return (IsolationLevel)GetValue(SBTransactionIsolationLevelProperty); } set { SetValue(SBTransactionIsolationLevelProperty, value); } }
		public static readonly DependencyProperty SBTransactionIsolationLevelProperty = DependencyProperty.Register("SBTransactionIsolationLevel", typeof(IsolationLevel), typeof(WcfService), new PropertyMetadata(IsolationLevel.Unspecified));

		public TimeSpan SBTransactionTimeout { get { return (TimeSpan)GetValue(SBTransactionTimeoutProperty); } set { SetValue(SBTransactionTimeoutProperty, value); } }
		public static readonly DependencyProperty SBTransactionTimeoutProperty = DependencyProperty.Register("SBTransactionTimeout", typeof(TimeSpan), typeof(WcfService), new PropertyMetadata(new TimeSpan(0, 0, 00)));

		public bool SBUseSynchronizationContext { get { return (bool)GetValue(SBUseSynchronizationContextProperty); } set { SetValue(SBUseSynchronizationContextProperty, value); } }
		public static readonly DependencyProperty SBUseSynchronizationContextProperty = DependencyProperty.Register("SBUseSynchronizationContext", typeof(bool), typeof(WcfService), new PropertyMetadata(true));

		public bool SBValidateMustUnderstand { get { return (bool)GetValue(SBValidateMustUnderstandProperty); } set { SetValue(SBValidateMustUnderstandProperty, value); } }
		public static readonly DependencyProperty SBValidateMustUnderstandProperty = DependencyProperty.Register("SBValidateMustUnderstand", typeof(bool), typeof(WcfService), new PropertyMetadata(true));

		//Callback Behavior
		public bool CBAutomaticSessionShutdown { get { return (bool)GetValue(CBAutomaticSessionShutdownProperty); } set { SetValue(CBAutomaticSessionShutdownProperty, value); } }
		public static readonly DependencyProperty CBAutomaticSessionShutdownProperty = DependencyProperty.Register("CBAutomaticSessionShutdown", typeof(bool), typeof(WcfService), new PropertyMetadata(true));

		public ConcurrencyMode CBConcurrencyMode { get { return (ConcurrencyMode)GetValue(CBConcurrencyModeProperty); } set { SetValue(CBConcurrencyModeProperty, value); } }
		public static readonly DependencyProperty CBConcurrencyModeProperty = DependencyProperty.Register("CBConcurrencyMode", typeof(ConcurrencyMode), typeof(WcfService), new PropertyMetadata(ConcurrencyMode.Multiple));

		public bool CBIgnoreExtensionDataObject { get { return (bool)GetValue(CBIgnoreExtensionDataObjectProperty); } set { SetValue(CBIgnoreExtensionDataObjectProperty, value); } }
		public static readonly DependencyProperty CBIgnoreExtensionDataObjectProperty = DependencyProperty.Register("CBIgnoreExtensionDataObject", typeof(bool), typeof(WcfService), new PropertyMetadata(false));

		public bool CBIncludeExceptionDetailInFaults { get { return (bool)GetValue(CBIncludeExceptionDetailInFaultsProperty); } set { SetValue(CBIncludeExceptionDetailInFaultsProperty, value); } }
		public static readonly DependencyProperty CBIncludeExceptionDetailInFaultsProperty = DependencyProperty.Register("CBIncludeExceptionDetailInFaults", typeof(bool), typeof(WcfService), new PropertyMetadata(false));

		public int CBMaxItemsInObjectGraph { get { return (int)GetValue(CBMaxItemsInObjectGraphProperty); } set { SetValue(CBMaxItemsInObjectGraphProperty, value); } }
		public static readonly DependencyProperty CBMaxItemsInObjectGraphProperty = DependencyProperty.Register("CBMaxItemsInObjectGraph", typeof(int), typeof(WcfService), new PropertyMetadata(65536));

		public IsolationLevel CBTransactionIsolationLevel { get { return (IsolationLevel)GetValue(CBTransactionIsolationLevelProperty); } set { SetValue(CBTransactionIsolationLevelProperty, value); } }
		public static readonly DependencyProperty CBTransactionIsolationLevelProperty = DependencyProperty.Register("CBTransactionIsolationLevel", typeof(IsolationLevel), typeof(WcfService), new PropertyMetadata(IsolationLevel.Unspecified));

		public TimeSpan CBTransactionTimeout { get { return (TimeSpan)GetValue(CBTransactionTimeoutProperty); } set { SetValue(CBTransactionTimeoutProperty, value); } }
		public static readonly DependencyProperty CBTransactionTimeoutProperty = DependencyProperty.Register("CBTransactionTimeout", typeof(TimeSpan), typeof(WcfService), new PropertyMetadata(new TimeSpan(0, 0, 00)));

		public bool CBUseSynchronizationContext { get { return (bool)GetValue(CBUseSynchronizationContextProperty); } set { SetValue(CBUseSynchronizationContextProperty, value); } }
		public static readonly DependencyProperty CBUseSynchronizationContextProperty = DependencyProperty.Register("CBUseSynchronizationContext", typeof(bool), typeof(WcfService), new PropertyMetadata(true));

		public bool CBValidateMustUnderstand { get { return (bool)GetValue(CBValidateMustUnderstandProperty); } set { SetValue(CBValidateMustUnderstandProperty, value); } }
		public static readonly DependencyProperty CBValidateMustUnderstandProperty = DependencyProperty.Register("CBValidateMustUnderstand", typeof(bool), typeof(WcfService), new PropertyMetadata(true));

		//System
		[IgnoreDataMember]
		public bool HasCallback { get { return CallbackOperations.Count > 0; } }
		[IgnoreDataMember]
		public bool HasCallbackOperations { get { return CallbackOperations.Count > 0 || ServiceOperations.Count(a => a.GetType() == typeof(WcfDataChangeMethod)) > 0; } }
		[IgnoreDataMember]
		public bool HasDCMOperations { get { return ServiceOperations.Count(a => a.GetType() == typeof(WcfDataChangeMethod)) > 0; } }

		public WcfService()
			: base(DataTypeMode.Class)
		{
			ServiceOperations = new ObservableCollection<WcfOperation>();
			CallbackOperations = new ObservableCollection<WcfOperation>();
			ServiceDocumentation = new Documentation { IsClass = true };
			CallbackDocumentation = new Documentation { IsClass = true };
		}

		public WcfService(string Name, Namespace Parent)
			: base(DataTypeMode.Class)
		{
			this.Name = Name;
			this.Parent = Parent;
			ServiceOperations = new ObservableCollection<WcfOperation>();
			CallbackOperations = new ObservableCollection<WcfOperation>();
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
	}

	public abstract class WcfOperation : DependencyObject
	{
		public Guid ID { get; set; }

		public DataType ReturnType { get { return (DataType)GetValue(ReturnTypeProperty); } set { SetValue(ReturnTypeProperty, value); } }
		public static readonly DependencyProperty ReturnTypeProperty = DependencyProperty.Register("ReturnType", typeof(DataType), typeof(WcfOperation), new PropertyMetadata(ReturnTypeChangedCallback));

		private static void ReturnTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs p)
		{
			var de = o as WcfOperation;
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
		public static readonly DependencyProperty ServerNameProperty = DependencyProperty.Register("ServerName", typeof(string), typeof(WcfOperation));

		public bool HasClientType { get { return (bool)GetValue(HasClientTypeProperty); } set { SetValue(HasClientTypeProperty, value); } }
		public static readonly DependencyProperty HasClientTypeProperty = DependencyProperty.Register("HasClientType", typeof(bool), typeof(WcfOperation), new PropertyMetadata(false, HasClientTypeChangedCallback));

		private static void HasClientTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfOperation;
			if (t == null) return;

			t.ClientName = Convert.ToBoolean(e.NewValue) ? t.ServerName : "";
		}

		public string ClientName { get { return (string)GetValue(ClientNameProperty); } set { SetValue(ClientNameProperty, value); } }
		public static readonly DependencyProperty ClientNameProperty = DependencyProperty.Register("ClientName", typeof(string), typeof(WcfOperation), new PropertyMetadata(""));

		public bool IsOneWay { get { return (bool)GetValue(IsOneWayProperty); } set { SetValue(IsOneWayProperty, value); } }
		public static readonly DependencyProperty IsOneWayProperty = DependencyProperty.Register("IsOneWay", typeof(bool), typeof(WcfOperation), new PropertyMetadata(false));

		public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(WcfOperation), new PropertyMetadata(false));

		public ProtectionLevel ProtectionLevel { get { return (ProtectionLevel)GetValue(ProtectionLevelProperty); } set { SetValue(ProtectionLevelProperty, value); } }
		public static readonly DependencyProperty ProtectionLevelProperty = DependencyProperty.Register("ProtectionLevel", typeof(ProtectionLevel), typeof(WcfOperation), new PropertyMetadata(ProtectionLevel.None));

		[IgnoreDataMember]
		public string Declaration { get { return (string)GetValue(DeclarationProperty); } protected set { SetValue(DeclarationPropertyKey, value); } }
		private static readonly DependencyPropertyKey DeclarationPropertyKey = DependencyProperty.RegisterReadOnly("Declaration", typeof(string), typeof(WcfOperation), new PropertyMetadata(""));
		public static readonly DependencyProperty DeclarationProperty = DeclarationPropertyKey.DependencyProperty;

		[IgnoreDataMember]
		public string ClientDeclaration { get { return (string)GetValue(ClientDeclarationProperty); } protected set { SetValue(ClientDeclarationPropertyKey, value); } }
		private static readonly DependencyPropertyKey ClientDeclarationPropertyKey = DependencyProperty.RegisterReadOnly("ClientDeclaration", typeof(string), typeof(WcfOperation), new PropertyMetadata(""));
		public static readonly DependencyProperty ClientDeclarationProperty = ClientDeclarationPropertyKey.DependencyProperty;

		//System
		public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(WcfOperation), new PropertyMetadata(false));

		public WcfService Owner { get; set; }

		public WcfOperation() { }

		public WcfOperation(string Name, WcfService Owner)
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
	}

	public class WcfProperty : WcfOperation
	{
		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(WcfProperty));

		public bool IsReadOnly { get { return (bool)GetValue(IsReadOnlyProperty); } set { SetValue(IsReadOnlyProperty, value); } }
		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(WcfProperty));

		public WcfProperty()
		{
			ReturnType = new DataType(PrimitiveTypes.String);
			Documentation = new Documentation { IsProperty = true };
		}

		public WcfProperty(DataType ReturnType, string Name, WcfService Owner)
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

	public class WcfMethod : WcfOperation
	{
		public bool UseServerSyncPattern { get { return (bool)GetValue(UseServerSyncPatternProperty); } set { SetValue(UseServerSyncPatternProperty, value); } }
		public static readonly DependencyProperty UseServerSyncPatternProperty = DependencyProperty.Register("UseServerSyncPattern", typeof(bool), typeof(WcfMethod), new PropertyMetadata(false, UseServerSyncPatternChangedCallback));

		private static void UseServerSyncPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfMethod;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseServerAwaitPattern = false;
			else t.UseServerAwaitPattern = true;
		}

		public bool UseServerAwaitPattern { get { return (bool)GetValue(UseServerAwaitPatternProperty); } set { SetValue(UseServerAwaitPatternProperty, value); } }
		public static readonly DependencyProperty UseServerAwaitPatternProperty = DependencyProperty.Register("UseServerAwaitPattern", typeof(bool), typeof(WcfMethod), new PropertyMetadata(true, UseServerAwaitPatternChangedCallback));

		private static void UseServerAwaitPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfMethod;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseServerSyncPattern = false;
			else t.UseServerSyncPattern = true;
		}

		public bool UseClientSyncPattern { get { return (bool)GetValue(UseClientSyncPatternProperty); } set { SetValue(UseClientSyncPatternProperty, value); } }
		public static readonly DependencyProperty UseClientSyncPatternProperty = DependencyProperty.Register("UseClientSyncPattern", typeof(bool), typeof(WcfMethod), new PropertyMetadata(false, UseClientSyncPatternChangedCallback));

		private static void UseClientSyncPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfMethod;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseClientAwaitPattern = false;
			else t.UseClientAwaitPattern = true;
		}

		public bool UseClientAwaitPattern { get { return (bool)GetValue(UseClientAwaitPatternProperty); } set { SetValue(UseClientAwaitPatternProperty, value); } }
		public static readonly DependencyProperty UseClientAwaitPatternProperty = DependencyProperty.Register("UseClientAwaitPattern", typeof(bool), typeof(WcfMethod), new PropertyMetadata(true, UseClientAwaitPatternChangedCallback));

		private static void UseClientAwaitPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfMethod;
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
		public static readonly DependencyProperty IsInitiatingProperty = DependencyProperty.Register("IsInitiating", typeof(bool), typeof(WcfMethod), new PropertyMetadata(false));

		public bool IsTerminating { get { return (bool)GetValue(IsTerminatingProperty); } set { SetValue(IsTerminatingProperty, value); } }
		public static readonly DependencyProperty IsTerminatingProperty = DependencyProperty.Register("IsTerminating", typeof(bool), typeof(WcfMethod), new PropertyMetadata(false, IsTerminatingChangedCallback));

		private static void IsTerminatingChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfMethod;
			if (t == null) return;

			t.UseClientSyncPattern = true;
			t.UseClientAwaitPattern = false;
		}

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(WcfMethod));

		public ObservableCollection<WcfMethodParameter> Parameters { get { return (ObservableCollection<WcfMethodParameter>)GetValue(ParametersProperty); } set { SetValue(ParametersProperty, value); } }
		public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(ObservableCollection<WcfMethodParameter>), typeof(WcfMethod));

		// Behavior
		public bool HasOperationBehavior { get { return (bool)GetValue(HasOperationBehaviorProperty); } set { SetValue(HasOperationBehaviorProperty, value); } }
		public static readonly DependencyProperty HasOperationBehaviorProperty = DependencyProperty.Register("HasOperationBehavior", typeof(bool), typeof(WcfMethod), new PropertyMetadata(false));

		public bool OBAutoDisposeParameters { get { return (bool)GetValue(OBAutoDisposeParametersProperty); } set { SetValue(OBAutoDisposeParametersProperty, value); } }
		public static readonly DependencyProperty OBAutoDisposeParametersProperty = DependencyProperty.Register("OBAutoDisposeParameters", typeof(bool), typeof(WcfMethod), new PropertyMetadata(true));

		public ImpersonationOption OBImpersonation { get { return (ImpersonationOption)GetValue(OBImpersonationProperty); } set { SetValue(OBImpersonationProperty, value); } }
		public static readonly DependencyProperty OBImpersonationProperty = DependencyProperty.Register("OBImpersonation", typeof(ImpersonationOption), typeof(WcfMethod), new PropertyMetadata(ImpersonationOption.NotAllowed));

		public ReleaseInstanceMode OBReleaseInstanceMode { get { return (ReleaseInstanceMode)GetValue(OBReleaseInstanceModeProperty); } set { SetValue(OBReleaseInstanceModeProperty, value); } }
		public static readonly DependencyProperty OBReleaseInstanceModeProperty = DependencyProperty.Register("OBReleaseInstanceMode", typeof(ReleaseInstanceMode), typeof(WcfMethod), new PropertyMetadata(ReleaseInstanceMode.None));

		public bool OBTransactionAutoComplete { get { return (bool)GetValue(OBTransactionAutoCompleteProperty); } set { SetValue(OBTransactionAutoCompleteProperty, value); } }
		public static readonly DependencyProperty OBTransactionAutoCompleteProperty = DependencyProperty.Register("OBTransactionAutoComplete", typeof(bool), typeof(WcfMethod), new PropertyMetadata(true));

		public bool OBTransactionScopeRequired { get { return (bool)GetValue(OBTransactionScopeRequiredProperty); } set { SetValue(OBTransactionScopeRequiredProperty, value); } }
		public static readonly DependencyProperty OBTransactionScopeRequiredProperty = DependencyProperty.Register("OBTransactionScopeRequired", typeof(bool), typeof(WcfMethod), new PropertyMetadata(false));

		public TransactionFlowMode OBTransactionFlowMode { get { return (TransactionFlowMode)GetValue(OBTransactionFlowModeProperty); } set { SetValue(OBTransactionFlowModeProperty, value); } }
		public static readonly DependencyProperty OBTransactionFlowModeProperty = DependencyProperty.Register("OBTransactionFlowMode", typeof(TransactionFlowMode), typeof(WcfMethod), new PropertyMetadata(TransactionFlowMode.None));

		public WcfMethod()
		{
			Parameters = new ObservableCollection<WcfMethodParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			Documentation = new Documentation { IsMethod = true };
		}

		public WcfMethod(string Name, WcfService Owner)
			: base(Name, Owner)
		{
			Parameters = new ObservableCollection<WcfMethodParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			ReturnType = new DataType(PrimitiveTypes.Void);
			Documentation = new Documentation { IsMethod = true };
		}

		public WcfMethod(DataType ReturnType, string Name, WcfService Owner)
			: base(Name, Owner)
		{
			Parameters = new ObservableCollection<WcfMethodParameter>();
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
			foreach (WcfMethodParameter p in Parameters)
				sb.AppendFormat("{0}, ", p);
			if (Parameters.Count > 0) sb.Remove(sb.Length - 2, 2);
			Declaration = string.Format("{0} {1}({2})", ReturnType, ServerName, sb);
			ClientDeclaration = string.Format("{0} {1}({2})", ReturnType, ClientName, sb);
		}
	}

	public class WcfCallback : WcfOperation
	{
		public bool UseServerSyncPattern { get { return (bool)GetValue(UseServerSyncPatternProperty); } set { SetValue(UseServerSyncPatternProperty, value); } }
		public static readonly DependencyProperty UseServerSyncPatternProperty = DependencyProperty.Register("UseServerSyncPattern", typeof(bool), typeof(WcfCallback), new PropertyMetadata(false, UseServerSyncPatternChangedCallback));

		private static void UseServerSyncPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfCallback;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseServerAwaitPattern = false;
			else t.UseServerAwaitPattern = true;
		}

		public bool UseServerAwaitPattern { get { return (bool)GetValue(UseServerAwaitPatternProperty); } set { SetValue(UseServerAwaitPatternProperty, value); } }
		public static readonly DependencyProperty UseServerAwaitPatternProperty = DependencyProperty.Register("UseServerAwaitPattern", typeof(bool), typeof(WcfCallback), new PropertyMetadata(true, UseServerAwaitPatternChangedCallback));

		private static void UseServerAwaitPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfCallback;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseServerSyncPattern = false;
			else t.UseServerSyncPattern = true;
		}

		public bool UseClientSyncPattern { get { return (bool)GetValue(UseClientSyncPatternProperty); } set { SetValue(UseClientSyncPatternProperty, value); } }
		public static readonly DependencyProperty UseClientSyncPatternProperty = DependencyProperty.Register("UseClientSyncPattern", typeof(bool), typeof(WcfCallback), new PropertyMetadata(false, UseClientSyncPatternChangedCallback));

		private static void UseClientSyncPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfCallback;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseClientAwaitPattern = false;
			else t.UseClientAwaitPattern = true;
		}

		public bool UseClientAwaitPattern { get { return (bool)GetValue(UseClientAwaitPatternProperty); } set { SetValue(UseClientAwaitPatternProperty, value); } }
		public static readonly DependencyProperty UseClientAwaitPatternProperty = DependencyProperty.Register("UseClientAwaitPattern", typeof(bool), typeof(WcfCallback), new PropertyMetadata(true, UseClientAwaitPatternChangedCallback));

		private static void UseClientAwaitPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfCallback;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseClientSyncPattern = false;
			else t.UseClientSyncPattern = true;
		}

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(WcfCallback));

		public ObservableCollection<WcfMethodParameter> Parameters { get { return (ObservableCollection<WcfMethodParameter>)GetValue(ParametersProperty); } set { SetValue(ParametersProperty, value); } }
		public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(ObservableCollection<WcfMethodParameter>), typeof(WcfCallback));

		// Behavior
		public bool HasOperationBehavior { get { return (bool)GetValue(HasOperationBehaviorProperty); } set { SetValue(HasOperationBehaviorProperty, value); } }
		public static readonly DependencyProperty HasOperationBehaviorProperty = DependencyProperty.Register("HasOperationBehavior", typeof(bool), typeof(WcfCallback), new PropertyMetadata(false));

		public bool OBAutoDisposeParameters { get { return (bool)GetValue(OBAutoDisposeParametersProperty); } set { SetValue(OBAutoDisposeParametersProperty, value); } }
		public static readonly DependencyProperty OBAutoDisposeParametersProperty = DependencyProperty.Register("OBAutoDisposeParameters", typeof(bool), typeof(WcfCallback), new PropertyMetadata(true));

		public ImpersonationOption OBImpersonation { get { return (ImpersonationOption)GetValue(OBImpersonationProperty); } set { SetValue(OBImpersonationProperty, value); } }
		public static readonly DependencyProperty OBImpersonationProperty = DependencyProperty.Register("OBImpersonation", typeof(ImpersonationOption), typeof(WcfCallback), new PropertyMetadata(ImpersonationOption.NotAllowed));

		public ReleaseInstanceMode OBReleaseInstanceMode { get { return (ReleaseInstanceMode)GetValue(OBReleaseInstanceModeProperty); } set { SetValue(OBReleaseInstanceModeProperty, value); } }
		public static readonly DependencyProperty OBReleaseInstanceModeProperty = DependencyProperty.Register("OBReleaseInstanceMode", typeof(ReleaseInstanceMode), typeof(WcfCallback), new PropertyMetadata(ReleaseInstanceMode.None));

		public bool OBTransactionAutoComplete { get { return (bool)GetValue(OBTransactionAutoCompleteProperty); } set { SetValue(OBTransactionAutoCompleteProperty, value); } }
		public static readonly DependencyProperty OBTransactionAutoCompleteProperty = DependencyProperty.Register("OBTransactionAutoComplete", typeof(bool), typeof(WcfCallback), new PropertyMetadata(true));

		public bool OBTransactionScopeRequired { get { return (bool)GetValue(OBTransactionScopeRequiredProperty); } set { SetValue(OBTransactionScopeRequiredProperty, value); } }
		public static readonly DependencyProperty OBTransactionScopeRequiredProperty = DependencyProperty.Register("OBTransactionScopeRequired", typeof(bool), typeof(WcfCallback), new PropertyMetadata(false));

		public TransactionFlowMode OBTransactionFlowMode { get { return (TransactionFlowMode)GetValue(OBTransactionFlowModeProperty); } set { SetValue(OBTransactionFlowModeProperty, value); } }
		public static readonly DependencyProperty OBTransactionFlowModeProperty = DependencyProperty.Register("OBTransactionFlowMode", typeof(TransactionFlowMode), typeof(WcfCallback), new PropertyMetadata(TransactionFlowMode.None));

		public WcfCallback()
		{
			Parameters = new ObservableCollection<WcfMethodParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			Documentation = new Documentation { IsMethod = true };
		}

		public WcfCallback(string Name, WcfService Owner)
			: base(Name, Owner)
		{
			Parameters = new ObservableCollection<WcfMethodParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			ReturnType = new DataType(PrimitiveTypes.Void);
			Documentation = new Documentation { IsMethod = true };
		}

		public WcfCallback(DataType ReturnType, string Name, WcfService Owner)
			: base(Name, Owner)
		{
			Parameters = new ObservableCollection<WcfMethodParameter>();
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
			foreach (WcfMethodParameter p in Parameters)
				sb.AppendFormat("{0}, ", p);
			if (Parameters.Count > 0) sb.Remove(sb.Length - 2, 2);
			Declaration = string.Format("{0} {1}({2})", ReturnType, ServerName, sb);
			ClientDeclaration = string.Format("{0} {1}({2})", ReturnType, ClientName, sb);
		}
	}

	public class WcfDataChangeMethod : WcfOperation
	{
		public bool UseServerSyncPattern { get { return (bool)GetValue(UseServerSyncPatternProperty); } set { SetValue(UseServerSyncPatternProperty, value); } }
		public static readonly DependencyProperty UseServerSyncPatternProperty = DependencyProperty.Register("UseServerSyncPattern", typeof(bool), typeof(WcfDataChangeMethod), new PropertyMetadata(false, UseServerSyncPatternChangedCallback));

		private static void UseServerSyncPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfDataChangeMethod;
			if (t == null) return;

			if ((bool) e.NewValue) t.UseServerAwaitPattern = false;
		}

		public bool UseServerAwaitPattern { get { return (bool)GetValue(UseServerAwaitPatternProperty); } set { SetValue(UseServerAwaitPatternProperty, value); } }
		public static readonly DependencyProperty UseServerAwaitPatternProperty = DependencyProperty.Register("UseServerAwaitPattern", typeof(bool), typeof(WcfDataChangeMethod), new PropertyMetadata(true, UseServerAwaitPatternChangedCallback));

		private static void UseServerAwaitPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfDataChangeMethod;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseServerSyncPattern = false;
		}

		public bool UseClientSyncPattern { get { return (bool)GetValue(UseClientSyncPatternProperty); } set { SetValue(UseClientSyncPatternProperty, value); } }
		public static readonly DependencyProperty UseClientSyncPatternProperty = DependencyProperty.Register("UseClientSyncPattern", typeof(bool), typeof(WcfDataChangeMethod), new PropertyMetadata(false, UseClientSyncPatternChangedCallback));

		private static void UseClientSyncPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfDataChangeMethod;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseClientAwaitPattern = false;
		}

		public bool UseClientAwaitPattern { get { return (bool)GetValue(UseClientAwaitPatternProperty); } set { SetValue(UseClientAwaitPatternProperty, value); } }
		public static readonly DependencyProperty UseClientAwaitPatternProperty = DependencyProperty.Register("UseClientAwaitPattern", typeof(bool), typeof(WcfDataChangeMethod), new PropertyMetadata(true, UseClientAwaitPatternChangedCallback));

		private static void UseClientAwaitPatternChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfDataChangeMethod;
			if (t == null) return;

			if ((bool)e.NewValue) t.UseClientSyncPattern = false;
		}

		public bool UseTPLForCallbacks { get { return (bool)GetValue(UseTPLForCallbacksProperty); } set { SetValue(UseTPLForCallbacksProperty, value); } }
		public static readonly DependencyProperty UseTPLForCallbacksProperty = DependencyProperty.Register("UseTPLForCallbacks", typeof(bool), typeof(WcfDataChangeMethod), new PropertyMetadata(true));

		public bool GenerateGetFunction { get { return (bool)GetValue(GenerateGetFunctionProperty); } set { SetValue(GenerateGetFunctionProperty, value); } }
		public static readonly DependencyProperty GenerateGetFunctionProperty = DependencyProperty.Register("GenerateGetFunction", typeof(bool), typeof(WcfDataChangeMethod), new PropertyMetadata(false, GenerateGetFunctionFunctionChangedCallback));

		private static void GenerateGetFunctionFunctionChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfDataChangeMethod;
			if (t == null) return;

			if ((bool)e.NewValue) return;
			t.GetParameters.Clear();
		}

		public bool GenerateOpenCloseFunction { get { return (bool)GetValue(GenerateOpenCloseFunctionProperty); } set { SetValue(GenerateOpenCloseFunctionProperty, value); } }
		public static readonly DependencyProperty GenerateOpenCloseFunctionProperty = DependencyProperty.Register("GenerateOpenCloseFunction", typeof(bool), typeof(WcfDataChangeMethod), new PropertyMetadata(true, GenerateOpenCloseFunctionChangedCallback));

		private static void GenerateOpenCloseFunctionChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfDataChangeMethod;
			if (t == null) return;

			if ((bool)e.NewValue) return;
			t.OpenParameters.Clear();
			t.CloseParameters.Clear();
		}

		public bool GenerateNewDeleteFunction { get { return (bool)GetValue(GenerateNewDeleteFunctionProperty); } set { SetValue(GenerateNewDeleteFunctionProperty, value); } }
		public static readonly DependencyProperty GenerateNewDeleteFunctionProperty = DependencyProperty.Register("GenerateNewDeleteFunction", typeof(bool), typeof(WcfDataChangeMethod), new PropertyMetadata(true, GenerateNewDeleteFunctionChangedCallback));

		private static void GenerateNewDeleteFunctionChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfDataChangeMethod;
			if (t == null) return;

			if ((bool)e.NewValue) return;
			t.NewParameters.Clear();
			t.DeleteParameters.Clear();
		}

		public bool IsReadOnly { get { return (bool)GetValue(IsReadOnlyProperty); } set { SetValue(IsReadOnlyProperty, value); } }
		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(WcfDataChangeMethod), new PropertyMetadata(false, IsReadOnlyChangedCallback));

		private static void IsReadOnlyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfDataChangeMethod;
			if (t == null) return;

			if (!(bool)e.NewValue) return;
			t.GenerateNewDeleteFunction = false;
		}

		public ObservableCollection<WcfMethodParameter> GetParameters { get { return (ObservableCollection<WcfMethodParameter>)GetValue(GetParametersProperty); } set { SetValue(GetParametersProperty, value); } }
		public static readonly DependencyProperty GetParametersProperty = DependencyProperty.Register("GetParameters", typeof(ObservableCollection<WcfMethodParameter>), typeof(WcfDataChangeMethod));

		public ObservableCollection<WcfMethodParameter> OpenParameters { get { return (ObservableCollection<WcfMethodParameter>)GetValue(OpenParametersProperty); } set { SetValue(OpenParametersProperty, value); } }
		public static readonly DependencyProperty OpenParametersProperty = DependencyProperty.Register("OpenParameters", typeof(ObservableCollection<WcfMethodParameter>), typeof(WcfDataChangeMethod));

		public ObservableCollection<WcfMethodParameter> CloseParameters { get { return (ObservableCollection<WcfMethodParameter>)GetValue(CloseParametersProperty); } set { SetValue(CloseParametersProperty, value); } }
		public static readonly DependencyProperty CloseParametersProperty = DependencyProperty.Register("CloseParameters", typeof(ObservableCollection<WcfMethodParameter>), typeof(WcfDataChangeMethod));

		public ObservableCollection<WcfMethodParameter> NewParameters { get { return (ObservableCollection<WcfMethodParameter>)GetValue(NewParametersProperty); } set { SetValue(NewParametersProperty, value); } }
		public static readonly DependencyProperty NewParametersProperty = DependencyProperty.Register("NewParameters", typeof(ObservableCollection<WcfMethodParameter>), typeof(WcfDataChangeMethod));

		public ObservableCollection<WcfMethodParameter> DeleteParameters { get { return (ObservableCollection<WcfMethodParameter>)GetValue(DeleteParametersProperty); } set { SetValue(DeleteParametersProperty, value); } }
		public static readonly DependencyProperty DeleteParametersProperty = DependencyProperty.Register("DeleteParameters", typeof(ObservableCollection<WcfMethodParameter>), typeof(WcfDataChangeMethod));

		public Documentation GetDocumentation { get { return (Documentation)GetValue(GetDocumentationProperty); } set { SetValue(GetDocumentationProperty, value); } }
		public static readonly DependencyProperty GetDocumentationProperty = DependencyProperty.Register("GetDocumentation", typeof(Documentation), typeof(WcfDataChangeMethod));

		public Documentation OpenDocumentation { get { return (Documentation)GetValue(OpenDocumentationProperty); } set { SetValue(OpenDocumentationProperty, value); } }
		public static readonly DependencyProperty OpenDocumentationProperty = DependencyProperty.Register("OpenDocumentation", typeof(Documentation), typeof(WcfDataChangeMethod));

		public Documentation CloseDocumentation { get { return (Documentation)GetValue(CloseDocumentationProperty); } set { SetValue(CloseDocumentationProperty, value); } }
		public static readonly DependencyProperty CloseDocumentationProperty = DependencyProperty.Register("CloseDocumentation", typeof(Documentation), typeof(WcfDataChangeMethod));

		public Documentation NewDocumentation { get { return (Documentation)GetValue(NewDocumentationProperty); } set { SetValue(NewDocumentationProperty, value); } }
		public static readonly DependencyProperty NewDocumentationProperty = DependencyProperty.Register("NewDocumentation", typeof(Documentation), typeof(WcfDataChangeMethod));

		public Documentation DeleteDocumentation { get { return (Documentation)GetValue(DeleteDocumentationProperty); } set { SetValue(DeleteDocumentationProperty, value); } }
		public static readonly DependencyProperty DeleteDocumentationProperty = DependencyProperty.Register("DeleteDocumentation", typeof(Documentation), typeof(WcfDataChangeMethod));

		public WcfDataChangeMethod()
		{
			GetParameters = new ObservableCollection<WcfMethodParameter>();
			OpenParameters = new ObservableCollection<WcfMethodParameter>();
			CloseParameters = new ObservableCollection<WcfMethodParameter>();
			NewParameters = new ObservableCollection<WcfMethodParameter>();
			DeleteParameters = new ObservableCollection<WcfMethodParameter>();
			ReturnType = new DataType(PrimitiveTypes.Void);
			GetDocumentation = new Documentation { IsMethod = true };
			OpenDocumentation = new Documentation { IsMethod = true };
			CloseDocumentation = new Documentation { IsMethod = true };
			NewDocumentation = new Documentation { IsMethod = true };
			DeleteDocumentation = new Documentation { IsMethod = true };
		}

		public WcfDataChangeMethod(string Name, WcfService Owner)
			: base(Name, Owner)
		{
			GetParameters = new ObservableCollection<WcfMethodParameter>();
			OpenParameters = new ObservableCollection<WcfMethodParameter>();
			CloseParameters = new ObservableCollection<WcfMethodParameter>();
			NewParameters = new ObservableCollection<WcfMethodParameter>();
			DeleteParameters = new ObservableCollection<WcfMethodParameter>();
			ReturnType = new DataType(PrimitiveTypes.Void);
			GetDocumentation = new Documentation { IsMethod = true };
			OpenDocumentation = new Documentation { IsMethod = true };
			CloseDocumentation = new Documentation { IsMethod = true };
			NewDocumentation = new Documentation { IsMethod = true };
			DeleteDocumentation = new Documentation { IsMethod = true };
		}

		public WcfDataChangeMethod(DataType ReturnType, string Name, WcfService Owner)
			: base(Name, Owner)
		{
			GetParameters = new ObservableCollection<WcfMethodParameter>();
			OpenParameters = new ObservableCollection<WcfMethodParameter>();
			CloseParameters = new ObservableCollection<WcfMethodParameter>();
			NewParameters = new ObservableCollection<WcfMethodParameter>();
			DeleteParameters = new ObservableCollection<WcfMethodParameter>();
			this.ReturnType = ReturnType;
			if (ReturnType.Primitive == PrimitiveTypes.Void) IsOneWay = true;
			DeleteParameters = new ObservableCollection<WcfMethodParameter>();
			GetDocumentation = new Documentation { IsMethod = true };
			OpenDocumentation = new Documentation { IsMethod = true };
			CloseDocumentation = new Documentation { IsMethod = true };
			NewDocumentation = new Documentation { IsMethod = true };
			DeleteDocumentation = new Documentation { IsMethod = true };
		}

		internal override void UpdateDeclaration()
		{
			Declaration = string.Format("{0} {1}", ReturnType, ServerName);
			ClientDeclaration = string.Format("{0} {1}", ReturnType, ClientName);
		}
	}

	public class WcfMethodParameter : DependencyObject
	{
		public Guid ID { get; set; }

		public DataType Type { get { return (DataType)GetValue(TypeProperty); } set { SetValue(TypeProperty, value); } }
		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(DataType), typeof(WcfMethodParameter), new PropertyMetadata(TypeChangedCallback));

		private static void TypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs p)
		{
			var de = o as WcfOperation;
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
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(WcfMethodParameter));

		public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(WcfMethodParameter));

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(WcfMethodParameter));

		//Internal Use
		public WcfService Owner { get; set; }
		public WcfOperation Parent { get; set; }

		public WcfMethodParameter()
		{
			ID = Guid.NewGuid();
			Type = new DataType(PrimitiveTypes.String);
			IsHidden = false;
			Documentation = new Documentation { IsParameter = true };
		}

		public WcfMethodParameter(DataType Type, string Name, WcfService Owner, WcfOperation Parent)
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
	}
}