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
	public class RESTService : DataType
	{
		public ObservableCollection<Operation> ServiceOperations { get { return (ObservableCollection<Operation>)GetValue(ServiceOperationsProperty); } set { SetValue(ServiceOperationsProperty, value); } }
		public static readonly DependencyProperty ServiceOperationsProperty = DependencyProperty.Register("ServiceOperations", typeof(ObservableCollection<Operation>), typeof(Service));

		public ProtectionLevel ProtectionLevel { get { return (ProtectionLevel)GetValue(ProtectionLevelProperty); } set { SetValue(ProtectionLevelProperty, value); } }
		public static readonly DependencyProperty ProtectionLevelProperty = DependencyProperty.Register("ProtectionLevel", typeof(ProtectionLevel), typeof(Service));

		public System.ServiceModel.SessionMode SessionMode { get { return (System.ServiceModel.SessionMode)GetValue(SessionModeProperty); } set { SetValue(SessionModeProperty, value); } }
		public static readonly DependencyProperty SessionModeProperty = DependencyProperty.Register("SessionMode", typeof(System.ServiceModel.SessionMode), typeof(Service));

		public string ConfigurationName { get { return (string)GetValue(ConfigurationNameProperty); } set { SetValue(ConfigurationNameProperty, value); } }
		public static readonly DependencyProperty ConfigurationNameProperty = DependencyProperty.Register("ConfigurationName", typeof(string), typeof(Service));

		public Documentation ServiceDocumentation { get { return (Documentation)GetValue(ServiceDocumentationProperty); } set { SetValue(ServiceDocumentationProperty, value); } }
		public static readonly DependencyProperty ServiceDocumentationProperty = DependencyProperty.Register("ServiceDocumentation", typeof(Documentation), typeof(Service));

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
		public static readonly DependencyProperty SBTransactionTimeoutProperty = DependencyProperty.Register("SBTransactionTimeout", typeof(TimeSpan), typeof(Service), new PropertyMetadata(new TimeSpan(0, 0, 30)));

		public bool SBUseSynchronizationContext { get { return (bool)GetValue(SBUseSynchronizationContextProperty); } set { SetValue(SBUseSynchronizationContextProperty, value); } }
		public static readonly DependencyProperty SBUseSynchronizationContextProperty = DependencyProperty.Register("SBUseSynchronizationContext", typeof(bool), typeof(Service), new PropertyMetadata(true));

		public bool SBValidateMustUnderstand { get { return (bool)GetValue(SBValidateMustUnderstandProperty); } set { SetValue(SBValidateMustUnderstandProperty, value); } }
		public static readonly DependencyProperty SBValidateMustUnderstandProperty = DependencyProperty.Register("SBValidateMustUnderstand", typeof(bool), typeof(Service), new PropertyMetadata(true));

		public RESTService() : base(DataTypeMode.Class)
		{
			ServiceOperations = new ObservableCollection<Operation>();
			ServiceDocumentation = new Documentation { IsClass = true };
		}

		public RESTService(string Name, Namespace Parent) : base(DataTypeMode.Class)
		{
			this.Name = Name;
			this.Parent = Parent;
			ServiceOperations = new ObservableCollection<Operation>();
			ID = Guid.NewGuid();
			ConfigurationName = "";
			ServiceDocumentation = new Documentation { IsClass = true };
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

	public abstract class RESTMethod : DependencyObject
	{
		public Guid ID { get; set; }

		public DataType ReturnType { get { return (DataType)GetValue(ReturnTypeProperty); } set { SetValue(ReturnTypeProperty, value); } }
		public static readonly DependencyProperty ReturnTypeProperty = DependencyProperty.Register("ReturnType", typeof(DataType), typeof(RESTMethod), new PropertyMetadata(ReturnTypeChangedCallback));

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
		public static readonly DependencyProperty ServerNameProperty = DependencyProperty.Register("ServerName", typeof(string), typeof(RESTMethod));

		public bool HasClientType { get { return (bool)GetValue(HasClientTypeProperty); } set { SetValue(HasClientTypeProperty, value); } }
		public static readonly DependencyProperty HasClientTypeProperty = DependencyProperty.Register("HasClientType", typeof(bool), typeof(RESTMethod), new PropertyMetadata(false, HasClientTypeChangedCallback));

		private static void HasClientTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as Operation;
			if (t == null) return;

			if (t.ClientName == "") t.ClientName = Convert.ToBoolean(e.NewValue) ? t.ServerName : "";
		}

		public string ClientName { get { return (string)GetValue(ClientNameProperty); } set { SetValue(ClientNameProperty, value); } }
		public static readonly DependencyProperty ClientNameProperty = DependencyProperty.Register("ClientName", typeof(string), typeof(RESTMethod), new PropertyMetadata(""));

		public bool IsOneWay { get { return (bool)GetValue(IsOneWayProperty); } set { SetValue(IsOneWayProperty, value); } }
		public static readonly DependencyProperty IsOneWayProperty = DependencyProperty.Register("IsOneWay", typeof(bool), typeof(RESTMethod), new PropertyMetadata(false));

		public ProtectionLevel ProtectionLevel { get { return (ProtectionLevel)GetValue(ProtectionLevelProperty); } set { SetValue(ProtectionLevelProperty, value); } }
		public static readonly DependencyProperty ProtectionLevelProperty = DependencyProperty.Register("ProtectionLevel", typeof(ProtectionLevel), typeof(RESTMethod), new PropertyMetadata(ProtectionLevel.None));

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(RESTMethod));

		public bool IsInitiating { get { return (bool)GetValue(IsInitiatingProperty); } set { SetValue(IsInitiatingProperty, value); } }
		public static readonly DependencyProperty IsInitiatingProperty = DependencyProperty.Register("IsInitiating", typeof(bool), typeof(RESTMethod), new PropertyMetadata(false));

		public bool IsTerminating { get { return (bool)GetValue(IsTerminatingProperty); } set { SetValue(IsTerminatingProperty, value); } }
		public static readonly DependencyProperty IsTerminatingProperty = DependencyProperty.Register("IsTerminating", typeof(bool), typeof(RESTMethod), new PropertyMetadata(false));

		public ObservableCollection<MethodParameter> Parameters { get { return (ObservableCollection<MethodParameter>)GetValue(ParametersProperty); } set { SetValue(ParametersProperty, value); } }
		public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(ObservableCollection<MethodParameter>), typeof(Method));

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
		
		//System
		[IgnoreDataMember] public string Declaration { get { return (string)GetValue(DeclarationProperty); } protected set { SetValue(DeclarationPropertyKey, value); } }
		private static readonly DependencyPropertyKey DeclarationPropertyKey = DependencyProperty.RegisterReadOnly("Declaration", typeof(string), typeof(RESTMethod), new PropertyMetadata(""));
		public static readonly DependencyProperty DeclarationProperty = DeclarationPropertyKey.DependencyProperty;
		
		[IgnoreDataMember] public string ClientDeclaration { get { return (string)GetValue(ClientDeclarationProperty); } protected set { SetValue(ClientDeclarationPropertyKey, value); } }
		private static readonly DependencyPropertyKey ClientDeclarationPropertyKey = DependencyProperty.RegisterReadOnly("ClientDeclaration", typeof(string), typeof(RESTMethod), new PropertyMetadata(""));
		public static readonly DependencyProperty ClientDeclarationProperty = ClientDeclarationPropertyKey.DependencyProperty;

		public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(RESTMethod), new PropertyMetadata(false));

		public Service Owner { get; set; }

		public RESTMethod()
		{
			ID = Guid.NewGuid();
			Parameters = new ObservableCollection<MethodParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			Documentation = new Documentation { IsMethod = true };
		}

		public RESTMethod(string Name, Service Owner)
		{
			ID = Guid.NewGuid();
			ServerName = Name;
			ReturnType = new DataType(PrimitiveTypes.Void);
			ProtectionLevel = ProtectionLevel.None;
			this.Owner = Owner;
			Parameters = new ObservableCollection<MethodParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			ReturnType = new DataType(PrimitiveTypes.Void);
			Documentation = new Documentation { IsMethod = true };
		}

		public RESTMethod(DataType ReturnType, string Name, Service Owner)
		{
			ID = Guid.NewGuid();
			ServerName = Name;
			ReturnType = new DataType(PrimitiveTypes.Void);
			ProtectionLevel = ProtectionLevel.None;
			this.Owner = Owner;
			Parameters = new ObservableCollection<MethodParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			this.ReturnType = ReturnType;
			if (ReturnType.Primitive == PrimitiveTypes.Void) IsOneWay = true;
			Documentation = new Documentation { IsMethod = true };
		}

		public override string ToString()
		{
			return ServerName;
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

			foreach (MethodParameter mp in Parameters)
				results.AddRange(mp.FindReplace(Args));

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

			foreach (MethodParameter mp in Parameters)
				mp.Replace(Args, Field);
		}
	}
}