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
		public ObservableCollection<RESTMethod> ServiceOperations { get { return (ObservableCollection<RESTMethod>)GetValue(ServiceOperationsProperty); } set { SetValue(ServiceOperationsProperty, value); } }
		public static readonly DependencyProperty ServiceOperationsProperty = DependencyProperty.Register("ServiceOperations", typeof(ObservableCollection<RESTMethod>), typeof(RESTService));

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

		public RESTService() : base(DataTypeMode.Class)
		{
			ServiceOperations = new ObservableCollection<RESTMethod>();
			ServiceDocumentation = new Documentation { IsClass = true };
		}

		public RESTService(string Name, Namespace Parent) : base(DataTypeMode.Class)
		{
			this.Name = Name;
			this.Parent = Parent;
			ServiceOperations = new ObservableCollection<RESTMethod>();
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
		
		//System
		[IgnoreDataMember] public string Declaration { get { return (string)GetValue(DeclarationProperty); } protected set { SetValue(DeclarationPropertyKey, value); } }
		private static readonly DependencyPropertyKey DeclarationPropertyKey = DependencyProperty.RegisterReadOnly("Declaration", typeof(string), typeof(RESTMethod), new PropertyMetadata(""));
		public static readonly DependencyProperty DeclarationProperty = DeclarationPropertyKey.DependencyProperty;

		public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(RESTMethod), new PropertyMetadata(false));

		public RESTService Owner { get; set; }

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
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(RESTMethodParameter));

		public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(RESTMethodParameter));

		public bool IsRESTInvalid { get { return (bool)GetValue(IsRESTInvalidProperty); } set { SetValue(IsRESTInvalidProperty, value); } }
		public static readonly DependencyProperty IsRESTInvalidProperty = DependencyProperty.Register("IsRESTInvalid", typeof(bool), typeof(RESTMethodParameter), new PropertyMetadata(false));

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
}