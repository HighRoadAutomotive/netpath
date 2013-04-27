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
using System.Net;
using System.Net.Http;
using System.Net.Cache;

namespace NETPath.Projects
{
	public class RESTService : DataType
	{
		public ObservableCollection<RESTMethod> ServiceOperations { get { return (ObservableCollection<RESTMethod>)GetValue(ServiceOperationsProperty); } set { SetValue(ServiceOperationsProperty, value); } }
		public static readonly DependencyProperty ServiceOperationsProperty = DependencyProperty.Register("ServiceOperations", typeof(ObservableCollection<RESTMethod>), typeof(RESTService));

		public ObservableCollection<RESTHTTPConfiguration> RequestConfigurations { get { return (ObservableCollection<RESTHTTPConfiguration>)GetValue(RequestConfigurationsProperty); } set { SetValue(RequestConfigurationsProperty, value); } }
		public static readonly DependencyProperty RequestConfigurationsProperty = DependencyProperty.Register("RequestConfigurations", typeof(ObservableCollection<RESTHTTPConfiguration>), typeof(ObservableCollection<RESTHTTPConfiguration>));

		public bool GenerateServer { get { return (bool)GetValue(GenerateServerProperty); } set { SetValue(GenerateServerProperty, value); } }
		public static readonly DependencyProperty GenerateServerProperty = DependencyProperty.Register("GenerateServer", typeof(bool), typeof(RESTService), new PropertyMetadata(true));

		public bool GenerateClient { get { return (bool)GetValue(GenerateClientProperty); } set { SetValue(GenerateClientProperty, value); } }
		public static readonly DependencyProperty GenerateClientProperty = DependencyProperty.Register("GenerateClient", typeof(bool), typeof(RESTService), new PropertyMetadata(true));

		public ProtectionLevel ProtectionLevel { get { return (ProtectionLevel)GetValue(ProtectionLevelProperty); } set { SetValue(ProtectionLevelProperty, value); } }
		public static readonly DependencyProperty ProtectionLevelProperty = DependencyProperty.Register("ProtectionLevel", typeof(ProtectionLevel), typeof(RESTService));

		public string ConfigurationName { get { return (string)GetValue(ConfigurationNameProperty); } set { SetValue(ConfigurationNameProperty, value); } }
		public static readonly DependencyProperty ConfigurationNameProperty = DependencyProperty.Register("ConfigurationName", typeof(string), typeof(RESTService));

		public Documentation ServiceDocumentation { get { return (Documentation)GetValue(ServiceDocumentationProperty); } set { SetValue(ServiceDocumentationProperty, value); } }
		public static readonly DependencyProperty ServiceDocumentationProperty = DependencyProperty.Register("ServiceDocumentation", typeof(Documentation), typeof(RESTService));

		public Documentation ClientDocumentation { get { return (Documentation)GetValue(ClientDocumentationProperty); } set { SetValue(ClientDocumentationProperty, value); } }
		public static readonly DependencyProperty ClientDocumentationProperty = DependencyProperty.Register("ClientDocumentation", typeof(Documentation), typeof(RESTService));

		//Service Behavior
		public AddressFilterMode SBAddressFilterMode { get { return (AddressFilterMode)GetValue(SBAddressFilterModeProperty); } set { SetValue(SBAddressFilterModeProperty, value); } }
		public static readonly DependencyProperty SBAddressFilterModeProperty = DependencyProperty.Register("SBAddressFilterMode", typeof(AddressFilterMode), typeof(RESTService), new PropertyMetadata(AddressFilterMode.Any));

		public bool SBAutomaticSessionShutdown { get { return (bool)GetValue(SBAutomaticSessionShutdownProperty); } set { SetValue(SBAutomaticSessionShutdownProperty, value); } }
		public static readonly DependencyProperty SBAutomaticSessionShutdownProperty = DependencyProperty.Register("SBAutomaticSessionShutdown", typeof(bool), typeof(RESTService), new PropertyMetadata(true));

		public ConcurrencyMode SBConcurrencyMode { get { return (ConcurrencyMode)GetValue(SBConcurrencyModeProperty); } set { SetValue(SBConcurrencyModeProperty, value); } }
		public static readonly DependencyProperty SBConcurrencyModeProperty = DependencyProperty.Register("SBConcurrencyMode", typeof(ConcurrencyMode), typeof(RESTService), new PropertyMetadata(ConcurrencyMode.Multiple));

		public bool SBEnsureOrderedDispatch { get { return (bool)GetValue(SBEnsureOrderedDispatchProperty); } set { SetValue(SBEnsureOrderedDispatchProperty, value); } }
		public static readonly DependencyProperty SBEnsureOrderedDispatchProperty = DependencyProperty.Register("SBEnsureOrderedDispatch", typeof(bool), typeof(RESTService), new PropertyMetadata(false));

		public bool SBIgnoreExtensionDataObject { get { return (bool)GetValue(SBIgnoreExtensionDataObjectProperty); } set { SetValue(SBIgnoreExtensionDataObjectProperty, value); } }
		public static readonly DependencyProperty SBIgnoreExtensionDataObjectProperty = DependencyProperty.Register("SBIgnoreExtensionDataObject", typeof(bool), typeof(RESTService), new PropertyMetadata(false));

		public bool SBIncludeExceptionDetailInFaults { get { return (bool)GetValue(SBIncludeExceptionDetailInFaultsProperty); } set { SetValue(SBIncludeExceptionDetailInFaultsProperty, value); } }
		public static readonly DependencyProperty SBIncludeExceptionDetailInFaultsProperty = DependencyProperty.Register("SBIncludeExceptionDetailInFaults", typeof(bool), typeof(RESTService), new PropertyMetadata(false));

		public InstanceContextMode SBInstanceContextMode { get { return (InstanceContextMode)GetValue(SBInstanceContextModeProperty); } set { SetValue(SBInstanceContextModeProperty, value); } }
		public static readonly DependencyProperty SBInstanceContextModeProperty = DependencyProperty.Register("SBInstanceContextMode", typeof(InstanceContextMode), typeof(RESTService), new PropertyMetadata(InstanceContextMode.PerSession));

		public int SBMaxItemsInObjectGraph { get { return (int)GetValue(SBMaxItemsInObjectGraphProperty); } set { SetValue(SBMaxItemsInObjectGraphProperty, value); } }
		public static readonly DependencyProperty SBMaxItemsInObjectGraphProperty = DependencyProperty.Register("SBMaxItemsInObjectGraph", typeof(int), typeof(RESTService), new PropertyMetadata(65536));

		public bool SBReleaseServiceInstanceOnTransactionComplete { get { return (bool)GetValue(SBReleaseServiceInstanceOnTransactionCompleteProperty); } set { SetValue(SBReleaseServiceInstanceOnTransactionCompleteProperty, value); } }
		public static readonly DependencyProperty SBReleaseServiceInstanceOnTransactionCompleteProperty = DependencyProperty.Register("SReleaseServiceInstanceOnTransactionComplete", typeof(bool), typeof(RESTService), new PropertyMetadata(true));

		public bool SBTransactionAutoCompleteOnSessionClose { get { return (bool)GetValue(SBTransactionAutoCompleteOnSessionCloseProperty); } set { SetValue(SBTransactionAutoCompleteOnSessionCloseProperty, value); } }
		public static readonly DependencyProperty SBTransactionAutoCompleteOnSessionCloseProperty = DependencyProperty.Register("SBTransactionAutoCompleteOnSessionClose", typeof(bool), typeof(RESTService), new PropertyMetadata(false));

		public IsolationLevel SBTransactionIsolationLevel { get { return (IsolationLevel)GetValue(SBTransactionIsolationLevelProperty); } set { SetValue(SBTransactionIsolationLevelProperty, value); } }
		public static readonly DependencyProperty SBTransactionIsolationLevelProperty = DependencyProperty.Register("SBTransactionIsolationLevel", typeof(IsolationLevel), typeof(RESTService), new PropertyMetadata(IsolationLevel.Unspecified));

		public TimeSpan SBTransactionTimeout { get { return (TimeSpan)GetValue(SBTransactionTimeoutProperty); } set { SetValue(SBTransactionTimeoutProperty, value); } }
		public static readonly DependencyProperty SBTransactionTimeoutProperty = DependencyProperty.Register("SBTransactionTimeout", typeof(TimeSpan), typeof(RESTService), new PropertyMetadata(new TimeSpan(0, 0, 30)));

		public bool SBUseSynchronizationContext { get { return (bool)GetValue(SBUseSynchronizationContextProperty); } set { SetValue(SBUseSynchronizationContextProperty, value); } }
		public static readonly DependencyProperty SBUseSynchronizationContextProperty = DependencyProperty.Register("SBUseSynchronizationContext", typeof(bool), typeof(RESTService), new PropertyMetadata(true));

		public bool SBValidateMustUnderstand { get { return (bool)GetValue(SBValidateMustUnderstandProperty); } set { SetValue(SBValidateMustUnderstandProperty, value); } }
		public static readonly DependencyProperty SBValidateMustUnderstandProperty = DependencyProperty.Register("SBValidateMustUnderstand", typeof(bool), typeof(RESTService), new PropertyMetadata(true));

		//Host
		public HostCredentials Credentials { get { return (HostCredentials)GetValue(CredentialsProperty); } set { SetValue(CredentialsProperty, value); } }
		public static readonly DependencyProperty CredentialsProperty = DependencyProperty.Register("Credentials", typeof(HostCredentials), typeof(RESTService));

		public string HostConfigurationName { get { return (string)GetValue(HostConfigurationNameProperty); } set { SetValue(HostConfigurationNameProperty, value); } }
		public static readonly DependencyProperty HostConfigurationNameProperty = DependencyProperty.Register("HostConfigurationName", typeof(string), typeof(RESTService));

		public TimeSpan CloseTimeout { get { return (TimeSpan)GetValue(CloseTimeoutProperty); } set { SetValue(CloseTimeoutProperty, value); } }
		public static readonly DependencyProperty CloseTimeoutProperty = DependencyProperty.Register("CloseTimeout", typeof(TimeSpan), typeof(RESTService), new PropertyMetadata(new TimeSpan(0, 1, 0)));

		public TimeSpan OpenTimeout { get { return (TimeSpan)GetValue(OpenTimeoutProperty); } set { SetValue(OpenTimeoutProperty, value); } }
		public static readonly DependencyProperty OpenTimeoutProperty = DependencyProperty.Register("OpenTimeout", typeof(TimeSpan), typeof(RESTService), new PropertyMetadata(new TimeSpan(0, 1, 0)));

		public int ManualFlowControlLimit { get { return (int)GetValue(ManualFlowControlLimitProperty); } set { SetValue(ManualFlowControlLimitProperty, value); } }
		public static readonly DependencyProperty ManualFlowControlLimitProperty = DependencyProperty.Register("ManualFlowControlLimit", typeof(int), typeof(RESTService));

		public bool AuthorizationImpersonateCallerForAllOperations { get { return (bool)GetValue(AuthorizationImpersonateCallerForAllOperationsProperty); } set { SetValue(AuthorizationImpersonateCallerForAllOperationsProperty, value); } }
		public static readonly DependencyProperty AuthorizationImpersonateCallerForAllOperationsProperty = DependencyProperty.Register("AuthorizationImpersonateCallerForAllOperations", typeof(bool), typeof(RESTService));

		public bool AuthorizationImpersonateOnSerializingReply { get { return (bool)GetValue(AuthorizationImpersonateOnSerializingReplyProperty); } set { SetValue(AuthorizationImpersonateOnSerializingReplyProperty, value); } }
		public static readonly DependencyProperty AuthorizationImpersonateOnSerializingReplyProperty = DependencyProperty.Register("AuthorizationImpersonateOnSerializingReply", typeof(bool), typeof(RESTService), new PropertyMetadata(false));

		public System.ServiceModel.Description.PrincipalPermissionMode AuthorizationPrincipalPermissionMode { get { return (System.ServiceModel.Description.PrincipalPermissionMode)GetValue(AuthorizationPrincipalPermissionModeProperty); } set { SetValue(AuthorizationPrincipalPermissionModeProperty, value); } }
		public static readonly DependencyProperty AuthorizationPrincipalPermissionModeProperty = DependencyProperty.Register("AuthorizationPrincipalPermissionMode", typeof(System.ServiceModel.Description.PrincipalPermissionMode), typeof(RESTService));

		//Endpoint
		public string EndpointAddress { get { return (string)GetValue(EndpointAddressProperty); } set { SetValue(EndpointAddressProperty, value); } }
		public static readonly DependencyProperty EndpointAddressProperty = DependencyProperty.Register("EndpointAddress", typeof(string), typeof(RESTService));

		public int EndpointPort { get { return (int)GetValue(EndpointPortProperty); } set { SetValue(EndpointPortProperty, value); } }
		public static readonly DependencyProperty EndpointPortProperty = DependencyProperty.Register("EndpointPort", typeof(int), typeof(RESTService));

		public bool EndpointUseHTTPS { get { return (bool)GetValue(EndpointUseHTTPSProperty); } set { SetValue(EndpointUseHTTPSProperty, value); } }
		public static readonly DependencyProperty EndpointUseHTTPSProperty = DependencyProperty.Register("EndpointUseHTTPS", typeof(bool), typeof(RESTService), new PropertyMetadata(false));

		public ServiceBindingWebHTTP EndpointBinding { get { return (ServiceBindingWebHTTP)GetValue(EndpointBindingProperty); } set { SetValue(EndpointBindingProperty, value); } }
		public static readonly DependencyProperty EndpointBindingProperty = DependencyProperty.Register("EndpointBinding", typeof(ServiceBindingWebHTTP), typeof(RESTService));

		//Behaviors
		public bool HasDebugBehavior { get { return (bool)GetValue(HasDebugBehaviorProperty); } set { SetValue(HasDebugBehaviorProperty, value); } }
		public static readonly DependencyProperty HasDebugBehaviorProperty = DependencyProperty.Register("HasDebugBehavior", typeof(bool), typeof(RESTService), new PropertyMetadata(false, HasDebugBehaviorChangedCallback));

		private static void HasDebugBehaviorChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var de = o as RESTService;
			if (de == null) return;
			de.DebugBehavior = ((bool) e.NewValue) ? new HostDebugBehavior() : null;
		}

		public HostDebugBehavior DebugBehavior { get { return (HostDebugBehavior)GetValue(DebugBehaviorProperty); } set { SetValue(DebugBehaviorProperty, value); } }
		public static readonly DependencyProperty DebugBehaviorProperty = DependencyProperty.Register("DebugBehavior", typeof(HostDebugBehavior), typeof(RESTService));

		public bool HasThrottlingBehavior { get { return (bool)GetValue(HasThrottlingBehaviorProperty); } set { SetValue(HasThrottlingBehaviorProperty, value); } }
		public static readonly DependencyProperty HasThrottlingBehaviorProperty = DependencyProperty.Register("HasThrottlingBehavior", typeof(bool), typeof(RESTService), new PropertyMetadata(false, HasThrottlingBehaviorChangedCallback));

		private static void HasThrottlingBehaviorChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var de = o as RESTService;
			if (de == null) return;
			de.ThrottlingBehavior = ((bool)e.NewValue) ? new HostThrottlingBehavior() : null;
		}

		public HostThrottlingBehavior ThrottlingBehavior { get { return (HostThrottlingBehavior)GetValue(ThrottlingBehaviorProperty); } set { SetValue(ThrottlingBehaviorProperty, value); } }
		public static readonly DependencyProperty ThrottlingBehaviorProperty = DependencyProperty.Register("ThrottlingBehavior", typeof(HostThrottlingBehavior), typeof(RESTService));

		public bool HasWebHTTPBehavior { get { return (bool)GetValue(HasWebHTTPBehaviorProperty); } set { SetValue(HasWebHTTPBehaviorProperty, value); } }
		public static readonly DependencyProperty HasWebHTTPBehaviorProperty = DependencyProperty.Register("HasWebHTTPBehavior", typeof(bool), typeof(RESTService), new PropertyMetadata(true, HasWebHTTPBehaviorChangedCallback));

		private static void HasWebHTTPBehaviorChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var de = o as RESTService;
			if (de == null) return;
			de.WebHTTPBehavior = ((bool)e.NewValue) ? new HostWebHTTPBehavior() : null;
		}

		public HostWebHTTPBehavior WebHTTPBehavior { get { return (HostWebHTTPBehavior)GetValue(WebHTTPBehaviorProperty); } set { SetValue(WebHTTPBehaviorProperty, value); } }
		public static readonly DependencyProperty WebHTTPBehaviorProperty = DependencyProperty.Register("WebHTTPBehavior", typeof(HostWebHTTPBehavior), typeof(RESTService));

		public RESTService() : base(DataTypeMode.Class)
		{
			ID = Guid.NewGuid();
			ServiceOperations = new ObservableCollection<RESTMethod>();
			RequestConfigurations = new ObservableCollection<RESTHTTPConfiguration>();
			ServiceDocumentation = new Documentation { IsClass = true };
			EndpointBinding = new ServiceBindingWebHTTP();
			WebHTTPBehavior = new HostWebHTTPBehavior();
			Credentials = new HostCredentials();
		}

		public RESTService(string Name, Namespace Parent) : base(DataTypeMode.Class)
		{
			this.Name = Name;
			this.Parent = Parent;
			ServiceOperations = new ObservableCollection<RESTMethod>();
			RequestConfigurations = new ObservableCollection<RESTHTTPConfiguration>();
			ID = Guid.NewGuid();
			ConfigurationName = "";
			ServiceDocumentation = new Documentation { IsClass = true };
			EndpointBinding = new ServiceBindingWebHTTP();
			WebHTTPBehavior = new HostWebHTTPBehavior();
			Credentials = new HostCredentials();
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

			foreach (RESTMethod o in ServiceOperations)
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

	public enum MethodRESTVerbs
	{
		GET,
		POST,
		PUT,
		DELETE
	}

	public class RESTMethod : DependencyObject
	{
		public Guid ID { get; set; }

		public DataType ReturnType { get { return (DataType)GetValue(ReturnTypeProperty); } set { SetValue(ReturnTypeProperty, value); } }
		public static readonly DependencyProperty ReturnTypeProperty = DependencyProperty.Register("ReturnType", typeof(DataType), typeof(RESTMethod), new PropertyMetadata(ReturnTypeChangedCallback));

		private static void ReturnTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs p)
		{
			var de = o as RESTMethod;
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
		public static readonly DependencyProperty ServerNameProperty = DependencyProperty.Register("ServerName", typeof(string), typeof(RESTMethod));

		public bool IsOneWay { get { return (bool)GetValue(IsOneWayProperty); } set { SetValue(IsOneWayProperty, value); } }
		public static readonly DependencyProperty IsOneWayProperty = DependencyProperty.Register("IsOneWay", typeof(bool), typeof(RESTMethod), new PropertyMetadata(false));

		public ProtectionLevel ProtectionLevel { get { return (ProtectionLevel)GetValue(ProtectionLevelProperty); } set { SetValue(ProtectionLevelProperty, value); } }
		public static readonly DependencyProperty ProtectionLevelProperty = DependencyProperty.Register("ProtectionLevel", typeof(ProtectionLevel), typeof(RESTMethod), new PropertyMetadata(ProtectionLevel.None));

		public bool ClientAsync { get { return (bool)GetValue(ClientAsyncProperty); } set { SetValue(ClientAsyncProperty, value); } }
		public static readonly DependencyProperty ClientAsyncProperty = DependencyProperty.Register("ClientAsync", typeof(bool), typeof(RESTMethod), new PropertyMetadata(true));

		public ObservableCollection<RESTMethodParameter> Parameters { get { return (ObservableCollection<RESTMethodParameter>)GetValue(ParametersProperty); } set { SetValue(ParametersProperty, value); } }
		public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(ObservableCollection<RESTMethodParameter>), typeof(RESTMethod));

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(RESTMethod));

		//REST
		public string UriTemplate { get { return (string)GetValue(UriTemplateProperty); } set { SetValue(UriTemplateProperty, value); } }
		public static readonly DependencyProperty UriTemplateProperty = DependencyProperty.Register("UriTemplate", typeof(string), typeof(RESTMethod), new PropertyMetadata("/"));

		public MethodRESTVerbs Method { get { return (MethodRESTVerbs)GetValue(MethodProperty); } set { SetValue(MethodProperty, value); } }
		public static readonly DependencyProperty MethodProperty = DependencyProperty.Register("Method", typeof(MethodRESTVerbs), typeof(RESTMethod), new PropertyMetadata(MethodRESTVerbs.GET));

		public WebMessageBodyStyle BodyStyle { get { return (WebMessageBodyStyle)GetValue(BodyStyleProperty); } set { SetValue(BodyStyleProperty, value); } }
		public static readonly DependencyProperty BodyStyleProperty = DependencyProperty.Register("BodyStyle", typeof(WebMessageBodyStyle), typeof(RESTMethod), new PropertyMetadata(WebMessageBodyStyle.Bare));

		public WebMessageFormat RequestFormat { get { return (WebMessageFormat)GetValue(RequestFormatProperty); } set { SetValue(RequestFormatProperty, value); } }
		public static readonly DependencyProperty RequestFormatProperty = DependencyProperty.Register("RequestFormat", typeof(WebMessageFormat), typeof(RESTMethod), new PropertyMetadata(WebMessageFormat.Xml));

		public WebMessageFormat ResponseFormat { get { return (WebMessageFormat)GetValue(ResponseFormatProperty); } set { SetValue(ResponseFormatProperty, value); } }
		public static readonly DependencyProperty ResponseFormatProperty = DependencyProperty.Register("ResponseFormat", typeof(WebMessageFormat), typeof(RESTMethod), new PropertyMetadata(WebMessageFormat.Xml));

		public RESTHTTPConfiguration RequestConfiguration { get { return (RESTHTTPConfiguration)GetValue(RequestConfigurationProperty); } set { SetValue(RequestConfigurationProperty, value); } }
		public static readonly DependencyProperty RequestConfigurationProperty = DependencyProperty.Register("RequestConfiguration", typeof(RESTHTTPConfiguration), typeof(RESTMethod), new PropertyMetadata(null));

		public bool DeserializeContent { get { return (bool)GetValue(DeserializeContentProperty); } set { SetValue(DeserializeContentProperty, value); } }
		public static readonly DependencyProperty DeserializeContentProperty = DependencyProperty.Register("DeserializeContent", typeof(bool), typeof(RESTMethod), new PropertyMetadata(true));

		//System
		[IgnoreDataMember] public string Declaration { get { return (string)GetValue(DeclarationProperty); } protected set { SetValue(DeclarationPropertyKey, value); } }
		private static readonly DependencyPropertyKey DeclarationPropertyKey = DependencyProperty.RegisterReadOnly("Declaration", typeof(string), typeof(RESTMethod), new PropertyMetadata(""));
		public static readonly DependencyProperty DeclarationProperty = DeclarationPropertyKey.DependencyProperty;

		public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(RESTMethod), new PropertyMetadata(false));

		public RESTService Owner { get { return (RESTService)GetValue(OwnerProperty); } set { SetValue(OwnerProperty, value); } }
		public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register("Owner", typeof(RESTService), typeof(RESTMethod));

		public RESTMethod()
		{
			ID = Guid.NewGuid();
			Parameters = new ObservableCollection<RESTMethodParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			Documentation = new Documentation { IsMethod = true };
		}

		public RESTMethod(string Name, RESTService Owner)
		{
			ID = Guid.NewGuid();
			ServerName = Name;
			ReturnType = new DataType(PrimitiveTypes.Void);
			ProtectionLevel = ProtectionLevel.None;
			this.Owner = Owner;
			Parameters = new ObservableCollection<RESTMethodParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			ReturnType = new DataType(PrimitiveTypes.Void);
			Documentation = new Documentation { IsMethod = true };
		}

		public RESTMethod(DataType ReturnType, string Name, RESTService Owner)
		{
			ID = Guid.NewGuid();
			ServerName = Name;
			this.ReturnType = ReturnType;
			ProtectionLevel = ProtectionLevel.None;
			this.Owner = Owner;
			Parameters = new ObservableCollection<RESTMethodParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			this.ReturnType = ReturnType;
			Documentation = new Documentation { IsMethod = true };
		}

		public override string ToString()
		{
			return ServerName;
		}

		private void Parameters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			UpdateDeclaration();
			if (Parameters.Count > 1 && (BodyStyle == WebMessageBodyStyle.Bare || BodyStyle == WebMessageBodyStyle.WrappedResponse)) BodyStyle = WebMessageBodyStyle.Wrapped;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if(Parameters == null) return;
			if (e.Property == DeclarationProperty) return;

			UpdateDeclaration();
		}

		internal void UpdateDeclaration()
		{
			var sb = new StringBuilder();
			foreach (RESTMethodParameter p in Parameters)
				sb.AppendFormat("{0}, ", p);
			if (Parameters.Count > 0) sb.Remove(sb.Length - 2, 2);
			Declaration = string.Format("{0} {1}({2})", ReturnType, ServerName, sb);
		}


		public virtual IEnumerable<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			var results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Service || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
						if (!string.IsNullOrEmpty(ServerName)) if (ServerName.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Server Name", ServerName, Owner.Parent.Owner, this));
					else
						if (!string.IsNullOrEmpty(ServerName)) if (ServerName.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Server Name", ServerName, Owner.Parent.Owner, this));
				}
				else
					if (!string.IsNullOrEmpty(ServerName)) if (Args.RegexSearch.IsMatch(ServerName)) results.Add(new FindReplaceResult("Server Name", ServerName, Owner.Parent.Owner, this));

				if (Args.ReplaceAll)
				{
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
							if (!string.IsNullOrEmpty(ServerName)) ServerName = Microsoft.VisualBasic.Strings.Replace(ServerName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						else
							if (!string.IsNullOrEmpty(ServerName)) ServerName = Microsoft.VisualBasic.Strings.Replace(ServerName, Args.Search, Args.Replace);
					}
					else
						if (!string.IsNullOrEmpty(ServerName)) ServerName = Args.RegexSearch.Replace(ServerName, Args.Replace);
				}
			}

			foreach (RESTMethodParameter mp in Parameters)
				results.AddRange(mp.FindReplace(Args));

			return results;
		}

		public virtual void Replace(FindReplaceInfo Args, string Field)
		{
			if (!Args.ReplaceAll) return;
			if (Args.UseRegex == false)
			{
				if (Args.MatchCase == false)
					if (Field == "Server Name") ServerName = Microsoft.VisualBasic.Strings.Replace(ServerName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
				else
					if (Field == "Server Name") ServerName = Microsoft.VisualBasic.Strings.Replace(ServerName, Args.Search, Args.Replace);
			}
			else
				if (Field == "Server Name") ServerName = Args.RegexSearch.Replace(ServerName, Args.Replace);

			foreach (RESTMethodParameter mp in Parameters)
				mp.Replace(Args, Field);
		}
	}

	public class RESTMethodParameter : DependencyObject
	{
		public Guid ID { get; set; }

		public DataType Type { get { return (DataType)GetValue(TypeProperty); } set { SetValue(TypeProperty, value); } }
		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(DataType), typeof(RESTMethodParameter), new PropertyMetadata(TypeChangedCallback));

		private static void TypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs p)
		{
			var de = o as RESTMethodParameter;
			if (de == null) return;
			var nt = p.NewValue as DataType;
			if (nt == null) return;
			var ot = p.OldValue as DataType;
			if (ot == null) return;

			if (ot.TypeMode == DataTypeMode.Array && ot.CollectionGenericType.TypeMode == DataTypeMode.Primitive) de.Owner.RemoveKnownType(nt);
			if (nt.TypeMode == DataTypeMode.Array && nt.CollectionGenericType.TypeMode == DataTypeMode.Primitive) de.Owner.AddKnownType(nt);
			if (ot.TypeMode == DataTypeMode.Primitive && ot.Primitive == PrimitiveTypes.DateTimeOffset) de.Owner.RemoveKnownType(new DataType(PrimitiveTypes.DateTimeOffset));
			if (nt.TypeMode == DataTypeMode.Primitive && nt.Primitive == PrimitiveTypes.DateTimeOffset) de.Owner.AddKnownType(new DataType(PrimitiveTypes.DateTimeOffset));

			de.IsSerializable = false;
			if (nt.TypeMode == DataTypeMode.Array || nt.TypeMode == DataTypeMode.Class || nt.TypeMode == DataTypeMode.Collection || nt.TypeMode == DataTypeMode.Dictionary || nt.TypeMode == DataTypeMode.Struct || nt.TypeMode == DataTypeMode.Queue || nt.TypeMode == DataTypeMode.Stack) de.IsSerializable = true;
		}

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value ?? "", @"")); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(RESTMethodParameter));

		public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(RESTMethodParameter));

		[IgnoreDataMember] public bool IsSerializable { get { return (bool)GetValue(IsSerializableProperty); } protected set { SetValue(IsSerializablePropertyKey, value); } }
		private static readonly DependencyPropertyKey IsSerializablePropertyKey = DependencyProperty.RegisterReadOnly("IsSerializable", typeof(bool), typeof(RESTMethodParameter), new PropertyMetadata(false));
		public static readonly DependencyProperty IsSerializableProperty = IsSerializablePropertyKey.DependencyProperty;

		public bool Serialize { get { return (bool)GetValue(SerializeProperty); } set { SetValue(SerializeProperty, value); } }
		public static readonly DependencyProperty SerializeProperty = DependencyProperty.Register("Serialize", typeof(bool), typeof(RESTMethodParameter), new PropertyMetadata(false, SerializeChangedCallback));

		private static void SerializeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var de = o as RESTMethodParameter;
			if (de == null) return;

			foreach (var t in de.Parent.Parameters.Where(a => !Equals(a, de)))
				t.Serialize = false;
			de.Serialize = true;
		}

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(RESTMethodParameter));

		//Internal Use
		public RESTService Owner { get; set; }
		public RESTMethod Parent { get; set; }

		public RESTMethodParameter()
		{
			ID = Guid.NewGuid();
			Type = new DataType(PrimitiveTypes.String);
			IsHidden = false;
			Documentation = new Documentation { IsParameter = true };
		}

		public RESTMethodParameter(DataType Type, string Name, RESTService Owner, RESTMethod Parent)
		{
			ID = Guid.NewGuid();
			this.Type = Type;
			this.Name = Name;
			IsHidden = false;
			this.Owner = Owner;
			this.Parent = Parent;
			Documentation = new Documentation { IsParameter = true };

			IsSerializable = false;
			if (Type.TypeMode == DataTypeMode.Array || Type.TypeMode == DataTypeMode.Class || Type.TypeMode == DataTypeMode.Collection || Type.TypeMode == DataTypeMode.Dictionary || Type.TypeMode == DataTypeMode.Struct || Type.TypeMode == DataTypeMode.Queue || Type.TypeMode == DataTypeMode.Stack) IsSerializable = true;
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
	
	public enum CookieContainerMode
	{
		None,
		Instance,
		Global,
		Custom
	}

	public enum CredentialsMode
	{
		None,
		Allowed,
		Required
	}

	public enum ContentMode
	{
		Default,
		ByteArray,
		String,
		Stream
	}

	public abstract class RESTHTTPConfiguration : DependencyObject
	{
		//System
		public Guid ID { get; set; }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(RESTHTTPConfiguration), new PropertyMetadata(""));

		public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(RESTHTTPConfiguration), new PropertyMetadata(false));

		//HTTP Request Configuration
		public bool AllowAutoRedirect { get { return (bool)GetValue(AllowAutoRedirectProperty); } set { SetValue(AllowAutoRedirectProperty, value); } }
		public static readonly DependencyProperty AllowAutoRedirectProperty = DependencyProperty.Register("AllowAutoRedirect", typeof(bool), typeof(RESTHTTPConfiguration), new PropertyMetadata(true));

		public DecompressionMethods AutomaticDecompression { get { return (DecompressionMethods)GetValue(AutomaticDecompressionProperty); } set { SetValue(AutomaticDecompressionProperty, value); } }
		public static readonly DependencyProperty AutomaticDecompressionProperty = DependencyProperty.Register("AutomaticDecompression", typeof(DecompressionMethods), typeof(RESTHTTPConfiguration), new PropertyMetadata(DecompressionMethods.None));

		public int MaxAutomaticRedirections { get { return (int)GetValue(MaxAutomaticRedirectionsProperty); } set { SetValue(MaxAutomaticRedirectionsProperty, value); } }
		public static readonly DependencyProperty MaxAutomaticRedirectionsProperty = DependencyProperty.Register("MaxAutomaticRedirections", typeof(int), typeof(RESTHTTPConfiguration), new PropertyMetadata(50));

		public bool UseProxy { get { return (bool)GetValue(UseProxyProperty); } set { SetValue(UseProxyProperty, value); } }
		public static readonly DependencyProperty UseProxyProperty = DependencyProperty.Register("UseProxy", typeof(bool), typeof(RESTHTTPConfiguration), new PropertyMetadata(false));

		public bool UseHTTP10 { get { return (bool)GetValue(UseHTTP10Property); } set { SetValue(UseHTTP10Property, value); } }
		public static readonly DependencyProperty UseHTTP10Property = DependencyProperty.Register("UseHTTP10", typeof(bool), typeof(RESTHTTPConfiguration), new PropertyMetadata(false));

		//Security
		public CredentialsMode CredentialsMode { get { return (CredentialsMode)GetValue(CredentialsModeProperty); } set { SetValue(CredentialsModeProperty, value); } }
		public static readonly DependencyProperty CredentialsModeProperty = DependencyProperty.Register("CredentialsMode", typeof(CredentialsMode), typeof(RESTHTTPConfiguration), new PropertyMetadata(CredentialsMode.None));

		public CookieContainerMode CookieContainerMode { get { return (CookieContainerMode)GetValue(CookieContainerModeProperty); } set { SetValue(CookieContainerModeProperty, value); } }
		public static readonly DependencyProperty CookieContainerModeProperty = DependencyProperty.Register("CookieContainerMode", typeof(CookieContainerMode), typeof(RESTHTTPConfiguration), new PropertyMetadata(CookieContainerMode.Global));

		public bool PreAuthenticate { get { return (bool)GetValue(PreAuthenticateProperty); } set { SetValue(PreAuthenticateProperty, value); } }
		public static readonly DependencyProperty PreAuthenticateProperty = DependencyProperty.Register("PreAuthenticate", typeof(bool), typeof(RESTHTTPConfiguration), new PropertyMetadata(false));

		public bool UseDefaultCredentials { get { return (bool)GetValue(UseDefaultCredentialsProperty); } set { SetValue(UseDefaultCredentialsProperty, value); } }
		public static readonly DependencyProperty UseDefaultCredentialsProperty = DependencyProperty.Register("UseDefaultCredentials", typeof(bool), typeof(RESTHTTPConfiguration), new PropertyMetadata(false));

		public RESTHTTPConfiguration()
		{
			ID = Guid.NewGuid();
		}

		public RESTHTTPConfiguration(string Name)
		{
			ID = Guid.NewGuid();
			this.Name = Name;
		}
	}

	public class RESTHTTPWebConfiguration : RESTHTTPConfiguration
	{
		//HTTP Request Configuration
		public string ConnectionGroupName { get { return (string)GetValue(ConnectionGroupNameProperty); } set { SetValue(ConnectionGroupNameProperty, value); } }
		public static readonly DependencyProperty ConnectionGroupNameProperty = DependencyProperty.Register("ConnectionGroupName", typeof(string), typeof(RESTHTTPWebConfiguration), new PropertyMetadata(null));

		public bool CanContinue { get { return (bool)GetValue(CanContinueProperty); } set { SetValue(CanContinueProperty, value); } }
		public static readonly DependencyProperty CanContinueProperty = DependencyProperty.Register("CanContinue", typeof(bool), typeof(RESTHTTPWebConfiguration), new PropertyMetadata(false));

		public TimeSpan ContinueTimeout { get { return (TimeSpan)GetValue(ContinueTimeoutProperty); } set { SetValue(ContinueTimeoutProperty, value); } }
		public static readonly DependencyProperty ContinueTimeoutProperty = DependencyProperty.Register("ContinueTimeout", typeof(TimeSpan), typeof(RESTHTTPWebConfiguration), new PropertyMetadata(new TimeSpan(0,0,100)));

		public bool KeepAlive { get { return (bool)GetValue(KeepAliveProperty); } set { SetValue(KeepAliveProperty, value); } }
		public static readonly DependencyProperty KeepAliveProperty = DependencyProperty.Register("KeepAlive", typeof(bool), typeof(RESTHTTPWebConfiguration), new PropertyMetadata(true));

		public int MaximumResponseHeadersLength { get { return (int)GetValue(MaximumResponseHeadersLengthProperty); } set { SetValue(MaximumResponseHeadersLengthProperty, value); } }
		public static readonly DependencyProperty MaximumResponseHeadersLengthProperty = DependencyProperty.Register("MaximumResponseHeadersLength", typeof(int), typeof(RESTHTTPWebConfiguration), new PropertyMetadata(-1));

		public RequestCacheLevel RequestCacheLevel { get { return (RequestCacheLevel)GetValue(RequestCacheLevelProperty); } set { SetValue(RequestCacheLevelProperty, value); } }
		public static readonly DependencyProperty RequestCacheLevelProperty = DependencyProperty.Register("RequestCacheLevel", typeof(RequestCacheLevel), typeof(RESTHTTPWebConfiguration), new PropertyMetadata(RequestCacheLevel.Default));

		public bool Pipelined { get { return (bool)GetValue(PipelinedProperty); } set { SetValue(PipelinedProperty, value); } }
		public static readonly DependencyProperty PipelinedProperty = DependencyProperty.Register("Pipelined", typeof(bool), typeof(RESTHTTPWebConfiguration), new PropertyMetadata(true));

		public TimeSpan ReadWriteTimeout { get { return (TimeSpan)GetValue(ReadWriteTimeoutProperty); } set { SetValue(ReadWriteTimeoutProperty, value); } }
		public static readonly DependencyProperty ReadWriteTimeoutProperty = DependencyProperty.Register("ReadWriteTimeout", typeof(TimeSpan), typeof(RESTHTTPConfiguration), new PropertyMetadata(new TimeSpan(0,0,30)));

		public TimeSpan Timeout { get { return (TimeSpan)GetValue(TimeoutProperty); } set { SetValue(TimeoutProperty, value); } }
		public static readonly DependencyProperty TimeoutProperty = DependencyProperty.Register("Timeout", typeof(TimeSpan), typeof(RESTHTTPWebConfiguration), new PropertyMetadata(new TimeSpan(0, 0, 100)));

		//Security
		public AuthenticationLevel AuthenticationLevel { get { return (AuthenticationLevel)GetValue(AuthenticationLevelProperty); } set { SetValue(AuthenticationLevelProperty, value); } }
		public static readonly DependencyProperty AuthenticationLevelProperty = DependencyProperty.Register("AuthenticationLevel", typeof(AuthenticationLevel), typeof(RESTHTTPWebConfiguration), new PropertyMetadata(AuthenticationLevel.MutualAuthRequested));

		public System.Security.Principal.TokenImpersonationLevel ImpersonationLevel { get { return (System.Security.Principal.TokenImpersonationLevel)GetValue(ImpersonationLevelProperty); } set { SetValue(ImpersonationLevelProperty, value); } }
		public static readonly DependencyProperty ImpersonationLevelProperty = DependencyProperty.Register("ImpersonationLevel", typeof(System.Security.Principal.TokenImpersonationLevel), typeof(RESTHTTPWebConfiguration), new PropertyMetadata(System.Security.Principal.TokenImpersonationLevel.None));

		public bool UnsafeAuthenticatedConnectionSharing { get { return (bool)GetValue(UnsafeAuthenticatedConnectionSharingProperty); } set { SetValue(UnsafeAuthenticatedConnectionSharingProperty, value); } }
		public static readonly DependencyProperty UnsafeAuthenticatedConnectionSharingProperty = DependencyProperty.Register("UnsafeAuthenticatedConnectionSharing", typeof(bool), typeof(RESTHTTPWebConfiguration), new PropertyMetadata(false));

		//Content
		public bool AllowReadStreamBuffering { get { return (bool)GetValue(AllowReadStreamBufferingProperty); } set { SetValue(AllowReadStreamBufferingProperty, value); } }
		public static readonly DependencyProperty AllowReadStreamBufferingProperty = DependencyProperty.Register("AllowReadStreamBuffering", typeof(bool), typeof(RESTHTTPWebConfiguration), new PropertyMetadata(true));

		public bool AllowWriteStreamBuffering { get { return (bool)GetValue(AllowWriteStreamBufferingProperty); } set { SetValue(AllowWriteStreamBufferingProperty, value); } }
		public static readonly DependencyProperty AllowWriteStreamBufferingProperty = DependencyProperty.Register("AllowWriteStreamBuffering", typeof(bool), typeof(RESTHTTPWebConfiguration), new PropertyMetadata(true));

		public string MediaType { get { return (string)GetValue(MediaTypeProperty); } set { SetValue(MediaTypeProperty, value); } }
		public static readonly DependencyProperty MediaTypeProperty = DependencyProperty.Register("MediaType", typeof(string), typeof(RESTHTTPWebConfiguration), new PropertyMetadata(null));

		public bool SendChunked { get { return (bool)GetValue(SendChunkedProperty); } set { SetValue(SendChunkedProperty, value); } }
		public static readonly DependencyProperty SendChunkedProperty = DependencyProperty.Register("SendChunked", typeof(bool), typeof(RESTHTTPWebConfiguration), new PropertyMetadata(false));

		public RESTHTTPWebConfiguration() { }

		public RESTHTTPWebConfiguration(string Name): base(Name) { }
	}

	public class RESTHTTPClientConfiguration : RESTHTTPConfiguration
	{
		public ClientCertificateOption ClientCertificateOptions { get { return (ClientCertificateOption)GetValue(ClientCertificateOptionsProperty); } set { SetValue(ClientCertificateOptionsProperty, value); } }
		public static readonly DependencyProperty ClientCertificateOptionsProperty = DependencyProperty.Register("ClientCertificateOptions", typeof(ClientCertificateOption), typeof(RESTHTTPClientConfiguration), new PropertyMetadata(ClientCertificateOption.Automatic));

		public long MaxRequestContentBufferSize { get { return (long)GetValue(MaxRequestContentBufferSizeProperty); } set { SetValue(MaxRequestContentBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxRequestContentBufferSizeProperty = DependencyProperty.Register("MaxRequestContentBufferSize", typeof(long), typeof(RESTHTTPClientConfiguration), new PropertyMetadata(21474836480L));

		public ContentMode ContentMode { get { return (ContentMode)GetValue(ContentModeProperty); } set { SetValue(ContentModeProperty, value); } }
		public static readonly DependencyProperty ContentModeProperty = DependencyProperty.Register("ContentMode", typeof(ContentMode), typeof(RESTHTTPClientConfiguration), new PropertyMetadata(ContentMode.Default));

		public RESTHTTPClientConfiguration() { }
		
		public RESTHTTPClientConfiguration(string Name) : base(Name) { }
	}
}