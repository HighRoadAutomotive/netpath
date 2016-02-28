using Newtonsoft.Json;
using System;
using System.CodeDom;
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
using System.Xml;

namespace NETPath.Projects.WebApi
{
	public class WebApiService : DataType
	{
		public ObservableCollection<WebApiMethod> ServiceOperations { get { return (ObservableCollection<WebApiMethod>)GetValue(ServiceOperationsProperty); } set { SetValue(ServiceOperationsProperty, value); } }
		public static readonly DependencyProperty ServiceOperationsProperty = DependencyProperty.Register("ServiceOperations", typeof(ObservableCollection<WebApiMethod>), typeof(WebApiService));

		public WebApiHttpConfiguration RequestConfiguration { get { return (WebApiHttpConfiguration)GetValue(RequestConfigurationProperty); } set { SetValue(RequestConfigurationProperty, value); } }
		public static readonly DependencyProperty RequestConfigurationProperty = DependencyProperty.Register("RequestConfiguration", typeof(WebApiHttpConfiguration), typeof(WebApiService));

		public WebApiJsonSettings JsonConfiguration { get { return (WebApiJsonSettings)GetValue(JsonConfigurationProperty); } set { SetValue(JsonConfigurationProperty, value); } }
		public static readonly DependencyProperty JsonConfigurationProperty = DependencyProperty.Register("JsonConfiguration", typeof(WebApiJsonSettings), typeof(WebApiService));

		public WebApiXmlSettings XmlConfiguration { get { return (WebApiXmlSettings)GetValue(XmlConfigurationProperty); } set { SetValue(XmlConfigurationProperty, value); } }
		public static readonly DependencyProperty XmlConfigurationProperty = DependencyProperty.Register("XmlConfiguration", typeof(WebApiXmlSettings), typeof(WebApiService));

		public Documentation ServiceDocumentation { get { return (Documentation)GetValue(ServiceDocumentationProperty); } set { SetValue(ServiceDocumentationProperty, value); } }
		public static readonly DependencyProperty ServiceDocumentationProperty = DependencyProperty.Register("ServiceDocumentation", typeof(Documentation), typeof(WebApiService));

		public Documentation ClientDocumentation { get { return (Documentation)GetValue(ClientDocumentationProperty); } set { SetValue(ClientDocumentationProperty, value); } }
		public static readonly DependencyProperty ClientDocumentationProperty = DependencyProperty.Register("ClientDocumentation", typeof(Documentation), typeof(WebApiService));


		//Security
		public string AuthenticationFilter { get { return (string)GetValue(AuthenticationFilterProperty); } set { SetValue(AuthenticationFilterProperty, value); } }
		public static readonly DependencyProperty AuthenticationFilterProperty = DependencyProperty.Register("AuthenticationFilter", typeof(string), typeof(WebApiService), new PropertyMetadata(""));

		public string AuthorizationFilter { get { return (string)GetValue(AuthorizationFilterProperty); } set { SetValue(AuthorizationFilterProperty, value); } }
		public static readonly DependencyProperty AuthorizationFilterProperty = DependencyProperty.Register("AuthorizationFilter", typeof(string), typeof(WebApiService), new PropertyMetadata(""));

		public WebApiService() : base(DataTypeMode.Class)
		{
			ID = Guid.NewGuid();
			ServiceOperations = new ObservableCollection<WebApiMethod>();
			ServiceDocumentation = new Documentation { IsClass = true };
		}

		public WebApiService(string Name, Namespace Parent) : base(DataTypeMode.Class)
		{
			this.Name = Name;
			this.Parent = Parent;
			ServiceOperations = new ObservableCollection<WebApiMethod>();
			RequestConfiguration = new WebApiHttpConfiguration();
			ID = Guid.NewGuid();
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
	}

	public enum WebApiMethodVerbs
	{
		Get,
		Post,
		Put,
		Delete,
		Head,
		Options,
		Patch,
		Custom,
	}

	public class WebApiMethod : DependencyObject
	{
		public Guid ID { get; set; }

		public DataType ReturnType { get { return (DataType)GetValue(ReturnTypeProperty); } set { SetValue(ReturnTypeProperty, value); } }
		public static readonly DependencyProperty ReturnTypeProperty = DependencyProperty.Register("ReturnType", typeof(DataType), typeof(WebApiMethod), new PropertyMetadata(ReturnTypeChangedCallback));

		private static void ReturnTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs p)
		{
			var de = o as WebApiMethod;
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
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(WebApiMethod));

		public bool UriIncludesName { get { return (bool)GetValue(UriIncludesNameProperty); } set { SetValue(UriIncludesNameProperty, value); } }
		public static readonly DependencyProperty UriIncludesNameProperty = DependencyProperty.Register("UriIncludesName", typeof(bool), typeof(WebApiMethod), new PropertyMetadata(false));

		public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(WebApiMethod), new PropertyMetadata(false));

		public bool ServerAsync { get { return (bool)GetValue(ServerAsyncProperty); } set { SetValue(ServerAsyncProperty, value); } }
		public static readonly DependencyProperty ServerAsyncProperty = DependencyProperty.Register("ServerAsync", typeof(bool), typeof(WebApiMethod), new PropertyMetadata(true));

		public bool ClientAsync { get { return (bool)GetValue(ClientAsyncProperty); } set { SetValue(ClientAsyncProperty, value); } }
		public static readonly DependencyProperty ClientAsyncProperty = DependencyProperty.Register("ClientAsync", typeof(bool), typeof(WebApiMethod), new PropertyMetadata(true));

		public ObservableCollection<WebApiRouteParameter> RouteParameters { get { return (ObservableCollection<WebApiRouteParameter>)GetValue(RouteParametersProperty); } set { SetValue(RouteParametersProperty, value); } }
		public static readonly DependencyProperty RouteParametersProperty = DependencyProperty.Register("RouteParameters", typeof(ObservableCollection<WebApiRouteParameter>), typeof(WebApiMethod));

		public ObservableCollection<WebApiMethodParameter> QueryParameters { get { return (ObservableCollection<WebApiMethodParameter>)GetValue(QueryParametersProperty); } set { SetValue(QueryParametersProperty, value); } }
		public static readonly DependencyProperty QueryParametersProperty = DependencyProperty.Register("QueryParameters", typeof(ObservableCollection<WebApiMethodParameter>), typeof(WebApiMethod));

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(WebApiMethod));

		//Rest
		public WebApiMethodVerbs Method { get { return (WebApiMethodVerbs)GetValue(MethodProperty); } set { SetValue(MethodProperty, value); } }
		public static readonly DependencyProperty MethodProperty = DependencyProperty.Register("Method", typeof(WebApiMethodVerbs), typeof(WebApiMethod), new PropertyMetadata(WebApiMethodVerbs.Get));

		public string CustomMethod { get { return (string)GetValue(CustomMethodProperty); } set { SetValue(CustomMethodProperty, value); } }
		public static readonly DependencyProperty CustomMethodProperty = DependencyProperty.Register("CustomMethod", typeof(string), typeof(WebApiMethod));

		public bool DeserializeContent { get { return (bool)GetValue(DeserializeContentProperty); } set { SetValue(DeserializeContentProperty, value); } }
		public static readonly DependencyProperty DeserializeContentProperty = DependencyProperty.Register("DeserializeContent", typeof(bool), typeof(WebApiMethod), new PropertyMetadata(true));

		public bool ReturnResponseData { get { return (bool)GetValue(ReturnResponseDataProperty); } set { SetValue(ReturnResponseDataProperty, value); } }
		public static readonly DependencyProperty ReturnResponseDataProperty = DependencyProperty.Register("ReturnResponseData", typeof(bool), typeof(WebApiMethod), new PropertyMetadata(false));

		public bool EnsureSuccessStatusCode { get { return (bool)GetValue(EnsureSuccessStatusCodeProperty); } set { SetValue(EnsureSuccessStatusCodeProperty, value); } }
		public static readonly DependencyProperty EnsureSuccessStatusCodeProperty = DependencyProperty.Register("EnsureSuccessStatusCode", typeof(bool), typeof(WebApiMethod), new PropertyMetadata(true));

		public string AuthenticationFilter { get { return (string)GetValue(AuthenticationFilterProperty); } set { SetValue(AuthenticationFilterProperty, value); } }
		public static readonly DependencyProperty AuthenticationFilterProperty = DependencyProperty.Register("AuthenticationFilter", typeof(string), typeof(WebApiMethod), new PropertyMetadata(""));

		public string AuthorizationFilter { get { return (string)GetValue(AuthorizationFilterProperty); } set { SetValue(AuthorizationFilterProperty, value); } }
		public static readonly DependencyProperty AuthorizationFilterProperty = DependencyProperty.Register("AuthorizationFilter", typeof(string), typeof(WebApiMethod), new PropertyMetadata(""));

		//Content
		public bool HasContent { get { return (bool)GetValue(HasContentProperty); } set { SetValue(HasContentProperty, value); } }
		public static readonly DependencyProperty HasContentProperty = DependencyProperty.Register("HasContent", typeof(bool), typeof(WebApiMethod), new PropertyMetadata(false));

		public DataType ContentType { get { return (DataType)GetValue(ContentTypeProperty); } set { SetValue(ContentTypeProperty, value); } }
		public static readonly DependencyProperty ContentTypeProperty = DependencyProperty.Register("ContentType", typeof(DataType), typeof(WebApiMethod), new PropertyMetadata(ContentTypeChangedCallback));

		private static void ContentTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs p)
		{
			var de = o as WebApiMethod;
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

		public string ContentParameterName { get { return (string)GetValue(ContentParameterNameProperty); } set { SetValue(ContentParameterNameProperty, value); } }
		public static readonly DependencyProperty ContentParameterNameProperty = DependencyProperty.Register("ContentParameterName", typeof(string), typeof(WebApiMethod));

		//System
		[IgnoreDataMember]
		public string Declaration { get { return (string)GetValue(DeclarationProperty); } protected set { SetValue(DeclarationPropertyKey, value); } }
		private static readonly DependencyPropertyKey DeclarationPropertyKey = DependencyProperty.RegisterReadOnly("Declaration", typeof(string), typeof(WebApiMethod), new PropertyMetadata(""));
		public static readonly DependencyProperty DeclarationProperty = DeclarationPropertyKey.DependencyProperty;

		public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(WebApiMethod), new PropertyMetadata(false));

		public WebApiService Owner { get { return (WebApiService)GetValue(OwnerProperty); } set { SetValue(OwnerProperty, value); } }
		public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register("Owner", typeof(WebApiService), typeof(WebApiMethod));

		public WebApiMethod()
		{
			ID = Guid.NewGuid();
			RouteParameters = new ObservableCollection<WebApiRouteParameter>();
			RouteParameters.CollectionChanged += Parameters_CollectionChanged;
			QueryParameters = new ObservableCollection<WebApiMethodParameter>();
			QueryParameters.CollectionChanged += Parameters_CollectionChanged;
			Documentation = new Documentation { IsMethod = true };
		}

		public WebApiMethod(string Name, WebApiService Owner)
		{
			ID = Guid.NewGuid();
			this.Name = Name;
			ReturnType = new DataType(PrimitiveTypes.Void);
			this.Owner = Owner;
			RouteParameters = new ObservableCollection<WebApiRouteParameter>();
			RouteParameters.CollectionChanged += Parameters_CollectionChanged;
			QueryParameters = new ObservableCollection<WebApiMethodParameter>();
			QueryParameters.CollectionChanged += Parameters_CollectionChanged;
			ReturnType = new DataType(PrimitiveTypes.Void);
			Documentation = new Documentation { IsMethod = true };
		}

		public WebApiMethod(DataType ReturnType, string Name, WebApiService Owner)
		{
			ID = Guid.NewGuid();
			this.Name = Name;
			this.ReturnType = ReturnType;
			this.Owner = Owner;
			RouteParameters = new ObservableCollection<WebApiRouteParameter>();
			RouteParameters.CollectionChanged += Parameters_CollectionChanged;
			QueryParameters = new ObservableCollection<WebApiMethodParameter>();
			QueryParameters.CollectionChanged += Parameters_CollectionChanged;
			this.ReturnType = ReturnType;
			Documentation = new Documentation { IsMethod = true };
		}

		public override string ToString()
		{
			return Name;
		}

		private void Parameters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			UpdateDeclaration();
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (RouteParameters == null || QueryParameters == null) return;
			if (e.Property == DeclarationProperty) return;

			UpdateDeclaration();
		}

		internal void UpdateDeclaration()
		{
			var sb = new StringBuilder();
			foreach (var p in RouteParameters.OfType<WebApiMethodParameter>())
				sb.AppendFormat("{0}, ", p);
			foreach (var p in QueryParameters)
				sb.AppendFormat("{0}, ", p);
			if (RouteParameters.OfType<WebApiMethodParameter>().Any() || QueryParameters.Any()) sb.Remove(sb.Length - 2, 2);
			Declaration = string.Format("{0} {1}({2})", ReturnType, Name, sb);
		}
	}

	public class WebApiRouteParameter : DependencyObject
	{
		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value ?? "", @"")); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(WebApiRouteParameter));

		public string RouteName { get { return (string)GetValue(RouteNameProperty); } set { SetValue(RouteNameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value ?? "", @"")); } }
		public static readonly DependencyProperty RouteNameProperty = DependencyProperty.Register("RouteName", typeof(string), typeof(WebApiRouteParameter));

		public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(WebApiRouteParameter), new PropertyMetadata(false));

		//Internal Use
		public WebApiService Owner { get; set; }
		public WebApiMethod Parent { get; set; }

		public WebApiRouteParameter() { }

		public WebApiRouteParameter(string routeName, WebApiService Owner, WebApiMethod Parent)
		{
			RouteName = routeName;
			IsHidden = false;
			this.Owner = Owner;
			this.Parent = Parent;
		}
	}

	public class WebApiMethodParameter : WebApiRouteParameter
	{
		public DataType Type { get { return (DataType)GetValue(TypeProperty); } set { SetValue(TypeProperty, value); } }
		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(DataType), typeof(WebApiMethodParameter), new PropertyMetadata(TypeChangedCallback));

		private static void TypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs p)
		{
			var de = o as WebApiMethodParameter;
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

		public string DefaultValue { get { return (string)GetValue(DefaultValueProperty); } set { SetValue(DefaultValueProperty, Helpers.RegExs.ReplaceSpaces.Replace(value ?? "", @"")); } }
		public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(string), typeof(WebApiMethodParameter));

		//Rest Specific
		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(WebApiMethodParameter));

		public WebApiMethodParameter() { }

		public WebApiMethodParameter(DataType Type, string Name, WebApiService Owner, WebApiMethod Parent)
		{
			this.Type = Type;
			this.Name = Name;
			this.Owner = Owner;
			this.Parent = Parent;
			RouteName = Name.ToLowerInvariant();
			Documentation = new Documentation { IsParameter = true };

			IsHidden = false;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			Parent?.UpdateDeclaration();
		}

		public override string ToString()
		{
			return string.Format("{0} {1}{2}", Type, Name, string.IsNullOrWhiteSpace(DefaultValue) ? "" : string.Format(" = {0}", DefaultValue));
		}
	}

	public enum CookieContainerMode
	{
		None,
		Instance,
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

	public enum RestSerialization
	{
		Json,
		Bson,
		Xml,
	}

	public class WebApiHttpConfiguration : DependencyObject
	{
		//HTTP Request Configuration
		public bool AllowAutoRedirect { get { return (bool)GetValue(AllowAutoRedirectProperty); } set { SetValue(AllowAutoRedirectProperty, value); } }
		public static readonly DependencyProperty AllowAutoRedirectProperty = DependencyProperty.Register("AllowAutoRedirect", typeof(bool), typeof(WebApiHttpConfiguration), new PropertyMetadata(true));

		public DecompressionMethods AutomaticDecompression { get { return (DecompressionMethods)GetValue(AutomaticDecompressionProperty); } set { SetValue(AutomaticDecompressionProperty, value); } }
		public static readonly DependencyProperty AutomaticDecompressionProperty = DependencyProperty.Register("AutomaticDecompression", typeof(DecompressionMethods), typeof(WebApiHttpConfiguration), new PropertyMetadata(DecompressionMethods.None));

		public RestSerialization RequestFormat { get { return (RestSerialization)GetValue(RequestFormatProperty); } set { SetValue(RequestFormatProperty, value); } }
		public static readonly DependencyProperty RequestFormatProperty = DependencyProperty.Register("RequestFormat", typeof(RestSerialization), typeof(WebApiHttpConfiguration), new PropertyMetadata(RestSerialization.Json));

		public RestSerialization ResponseFormat { get { return (RestSerialization)GetValue(ResponseFormatProperty); } set { SetValue(ResponseFormatProperty, value); } }
		public static readonly DependencyProperty ResponseFormatProperty = DependencyProperty.Register("ResponseFormat", typeof(RestSerialization), typeof(WebApiHttpConfiguration), new PropertyMetadata(RestSerialization.Json));

		public int MaxAutomaticRedirections { get { return (int)GetValue(MaxAutomaticRedirectionsProperty); } set { SetValue(MaxAutomaticRedirectionsProperty, value); } }
		public static readonly DependencyProperty MaxAutomaticRedirectionsProperty = DependencyProperty.Register("MaxAutomaticRedirections", typeof(int), typeof(WebApiHttpConfiguration), new PropertyMetadata(50));

		public bool UseProxy { get { return (bool)GetValue(UseProxyProperty); } set { SetValue(UseProxyProperty, value); } }
		public static readonly DependencyProperty UseProxyProperty = DependencyProperty.Register("UseProxy", typeof(bool), typeof(WebApiHttpConfiguration), new PropertyMetadata(false));

		public bool UseHTTP10 { get { return (bool)GetValue(UseHTTP10Property); } set { SetValue(UseHTTP10Property, value); } }
		public static readonly DependencyProperty UseHTTP10Property = DependencyProperty.Register("UseHTTP10", typeof(bool), typeof(WebApiHttpConfiguration), new PropertyMetadata(false));

		//Security
		public CredentialsMode CredentialsMode { get { return (CredentialsMode)GetValue(CredentialsModeProperty); } set { SetValue(CredentialsModeProperty, value); } }
		public static readonly DependencyProperty CredentialsModeProperty = DependencyProperty.Register("CredentialsMode", typeof(CredentialsMode), typeof(WebApiHttpConfiguration), new PropertyMetadata(CredentialsMode.None));

		public CookieContainerMode CookieContainerMode { get { return (CookieContainerMode)GetValue(CookieContainerModeProperty); } set { SetValue(CookieContainerModeProperty, value); } }
		public static readonly DependencyProperty CookieContainerModeProperty = DependencyProperty.Register("CookieContainerMode", typeof(CookieContainerMode), typeof(WebApiHttpConfiguration), new PropertyMetadata(CookieContainerMode.Instance));

		public bool PreAuthenticate { get { return (bool)GetValue(PreAuthenticateProperty); } set { SetValue(PreAuthenticateProperty, value); } }
		public static readonly DependencyProperty PreAuthenticateProperty = DependencyProperty.Register("PreAuthenticate", typeof(bool), typeof(WebApiHttpConfiguration), new PropertyMetadata(false));

		public bool UseDefaultCredentials { get { return (bool)GetValue(UseDefaultCredentialsProperty); } set { SetValue(UseDefaultCredentialsProperty, value); } }
		public static readonly DependencyProperty UseDefaultCredentialsProperty = DependencyProperty.Register("UseDefaultCredentials", typeof(bool), typeof(WebApiHttpConfiguration), new PropertyMetadata(false));

		public ClientCertificateOption ClientCertificateOptions { get { return (ClientCertificateOption)GetValue(ClientCertificateOptionsProperty); } set { SetValue(ClientCertificateOptionsProperty, value); } }
		public static readonly DependencyProperty ClientCertificateOptionsProperty = DependencyProperty.Register("ClientCertificateOptions", typeof(ClientCertificateOption), typeof(WebApiHttpConfiguration), new PropertyMetadata(ClientCertificateOption.Automatic));

		public long MaxRequestContentBufferSize { get { return (long)GetValue(MaxRequestContentBufferSizeProperty); } set { SetValue(MaxRequestContentBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxRequestContentBufferSizeProperty = DependencyProperty.Register("MaxRequestContentBufferSize", typeof(long), typeof(WebApiHttpConfiguration), new PropertyMetadata(2147483647L));

		public WebApiHttpConfiguration() { }
	}

	public class WebApiJsonSettings : DependencyObject
	{
		public ConstructorHandling ConstructorHandling { get { return (ConstructorHandling)GetValue(ConstructorHandlingProperty); } set { SetValue(ConstructorHandlingProperty, value); } }
		public static readonly DependencyProperty ConstructorHandlingProperty = DependencyProperty.Register("ConstructorHandling", typeof(ConstructorHandling), typeof(WebApiJsonSettings), new PropertyMetadata(ConstructorHandling.Default));

		public DateFormatHandling DateFormatHandling { get { return (DateFormatHandling)GetValue(DateFormatHandlingProperty); } set { SetValue(DateFormatHandlingProperty, value); } }
		public static readonly DependencyProperty DateFormatHandlingProperty = DependencyProperty.Register("DateFormatHandling", typeof(DateFormatHandling), typeof(WebApiJsonSettings), new PropertyMetadata(DateFormatHandling.IsoDateFormat));

		public string DateFormatString { get { return (string)GetValue(DateFormatStringProperty); } set { SetValue(DateFormatStringProperty, value); } }
		public static readonly DependencyProperty DateFormatStringProperty = DependencyProperty.Register("DateFormatString", typeof(string), typeof(WebApiJsonSettings), new PropertyMetadata("s"));

		public DateParseHandling DateParseHandling { get { return (DateParseHandling)GetValue(DateParseHandlingProperty); } set { SetValue(DateParseHandlingProperty, value); } }
		public static readonly DependencyProperty DateParseHandlingProperty = DependencyProperty.Register("DateParseHandling", typeof(DateParseHandling), typeof(WebApiJsonSettings), new PropertyMetadata(DateParseHandling.DateTime));

		public DateTimeZoneHandling DateTimeZoneHandling { get { return (DateTimeZoneHandling)GetValue(DateTimeZoneHandlingProperty); } set { SetValue(DateTimeZoneHandlingProperty, value); } }
		public static readonly DependencyProperty DateTimeZoneHandlingProperty = DependencyProperty.Register("DateTimeZoneHandling", typeof(DateTimeZoneHandling), typeof(WebApiJsonSettings), new PropertyMetadata(DateTimeZoneHandling.Local));

		public DefaultValueHandling DefaultValueHandling { get { return (DefaultValueHandling)GetValue(DefaultValueHandlingProperty); } set { SetValue(DefaultValueHandlingProperty, value); } }
		public static readonly DependencyProperty DefaultValueHandlingProperty = DependencyProperty.Register("DefaultValueHandling", typeof(DefaultValueHandling), typeof(WebApiJsonSettings), new PropertyMetadata(DefaultValueHandling.Ignore));

		public FloatFormatHandling FloatFormatHandling { get { return (FloatFormatHandling)GetValue(FloatFormatHandlingProperty); } set { SetValue(FloatFormatHandlingProperty, value); } }
		public static readonly DependencyProperty FloatFormatHandlingProperty = DependencyProperty.Register("FloatFormatHandling", typeof(FloatFormatHandling), typeof(WebApiJsonSettings), new PropertyMetadata(FloatFormatHandling.String));

		public FloatParseHandling FloatParseHandling { get { return (FloatParseHandling)GetValue(FloatParseHandlingProperty); } set { SetValue(FloatParseHandlingProperty, value); } }
		public static readonly DependencyProperty FloatParseHandlingProperty = DependencyProperty.Register("FloatParseHandling", typeof(FloatParseHandling), typeof(WebApiJsonSettings), new PropertyMetadata(FloatParseHandling.Decimal));

		public Newtonsoft.Json.Formatting Formatting { get { return (Newtonsoft.Json.Formatting)GetValue(FormattingProperty); } set { SetValue(FormattingProperty, value); } }
		public static readonly DependencyProperty FormattingProperty = DependencyProperty.Register("Formatting", typeof(Newtonsoft.Json.Formatting), typeof(WebApiJsonSettings), new PropertyMetadata(Newtonsoft.Json.Formatting.None));

		public int MaxDepth { get { return (int)GetValue(MaxDepthProperty); } set { SetValue(MaxDepthProperty, value); } }
		public static readonly DependencyProperty MaxDepthProperty = DependencyProperty.Register("MaxDepth", typeof(int), typeof(WebApiJsonSettings), new PropertyMetadata(int.MaxValue));

		public MetadataPropertyHandling MetadataPropertyHandling { get { return (MetadataPropertyHandling)GetValue(MetadataPropertyHandlingProperty); } set { SetValue(MetadataPropertyHandlingProperty, value); } }
		public static readonly DependencyProperty MetadataPropertyHandlingProperty = DependencyProperty.Register("MetadataPropertyHandling", typeof(MetadataPropertyHandling), typeof(WebApiJsonSettings), new PropertyMetadata(MetadataPropertyHandling.Default));

		public MissingMemberHandling MissingMemberHandling { get { return (MissingMemberHandling)GetValue(MissingMemberHandlingProperty); } set { SetValue(MissingMemberHandlingProperty, value); } }
		public static readonly DependencyProperty MissingMemberHandlingProperty = DependencyProperty.Register("MissingMemberHandling", typeof(MissingMemberHandling), typeof(WebApiJsonSettings), new PropertyMetadata(MissingMemberHandling.Ignore));

		public NullValueHandling NullValueHandling { get { return (NullValueHandling)GetValue(NullValueHandlingProperty); } set { SetValue(NullValueHandlingProperty, value); } }
		public static readonly DependencyProperty NullValueHandlingProperty = DependencyProperty.Register("NullValueHandling", typeof(NullValueHandling), typeof(WebApiJsonSettings), new PropertyMetadata(NullValueHandling.Ignore));

		public ObjectCreationHandling ObjectCreationHandling { get { return (ObjectCreationHandling)GetValue(ObjectCreationHandlingProperty); } set { SetValue(ObjectCreationHandlingProperty, value); } }
		public static readonly DependencyProperty ObjectCreationHandlingProperty = DependencyProperty.Register("ObjectCreationHandling", typeof(ObjectCreationHandling), typeof(WebApiJsonSettings), new PropertyMetadata(ObjectCreationHandling.Auto));

		public PreserveReferencesHandling PreserveReferencesHandling { get { return (PreserveReferencesHandling)GetValue(PreserveReferencesHandlingProperty); } set { SetValue(PreserveReferencesHandlingProperty, value); } }
		public static readonly DependencyProperty PreserveReferencesHandlingProperty = DependencyProperty.Register("PreserveReferencesHandling", typeof(PreserveReferencesHandling), typeof(WebApiJsonSettings), new PropertyMetadata(PreserveReferencesHandling.None));

		public ReferenceLoopHandling ReferenceLoopHandling { get { return (ReferenceLoopHandling)GetValue(ReferenceLoopHandlingProperty); } set { SetValue(ReferenceLoopHandlingProperty, value); } }
		public static readonly DependencyProperty ReferenceLoopHandlingProperty = DependencyProperty.Register("ReferenceLoopHandling", typeof(ReferenceLoopHandling), typeof(WebApiJsonSettings), new PropertyMetadata(ReferenceLoopHandling.Ignore));

		public StringEscapeHandling StringEscapeHandling { get { return (StringEscapeHandling)GetValue(StringEscapeHandlingProperty); } set { SetValue(StringEscapeHandlingProperty, value); } }
		public static readonly DependencyProperty StringEscapeHandlingProperty = DependencyProperty.Register("StringEscapeHandling", typeof(StringEscapeHandling), typeof(WebApiJsonSettings), new PropertyMetadata(StringEscapeHandling.Default));

		public System.Runtime.Serialization.Formatters.FormatterAssemblyStyle TypeNameAssemblyFormat { get { return (System.Runtime.Serialization.Formatters.FormatterAssemblyStyle)GetValue(TypeNameAssemblyFormatProperty); } set { SetValue(TypeNameAssemblyFormatProperty, value); } }
		public static readonly DependencyProperty TypeNameAssemblyFormatProperty = DependencyProperty.Register("TypeNameAssemblyFormat", typeof(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle), typeof(WebApiJsonSettings), new PropertyMetadata(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple));

		public TypeNameHandling TypeNameHandling { get { return (TypeNameHandling)GetValue(TypeNameHandlingProperty); } set { SetValue(TypeNameHandlingProperty, value); } }
		public static readonly DependencyProperty TypeNameHandlingProperty = DependencyProperty.Register("TypeNameHandling", typeof(TypeNameHandling), typeof(WebApiJsonSettings), new PropertyMetadata(TypeNameHandling.Auto));
	}

	public class WebApiXmlSettings : DependencyObject
	{
		public bool CheckCharacters { get { return (bool)GetValue(CheckCharactersProperty); } set { SetValue(CheckCharactersProperty, value); } }
		public static readonly DependencyProperty CheckCharactersProperty = DependencyProperty.Register("CheckCharacters", typeof(bool), typeof(WebApiXmlSettings), new PropertyMetadata(true));

		public ConformanceLevel ConformanceLevel { get { return (ConformanceLevel)GetValue(ConformanceLevelProperty); } set { SetValue(ConformanceLevelProperty, value); } }
		public static readonly DependencyProperty ConformanceLevelProperty = DependencyProperty.Register("ConformanceLevel", typeof(ConformanceLevel), typeof(WebApiXmlSettings), new PropertyMetadata(ConformanceLevel.Auto));

		public bool Indent { get { return (bool)GetValue(IndentProperty); } set { SetValue(IndentProperty, value); } }
		public static readonly DependencyProperty IndentProperty = DependencyProperty.Register("Indent", typeof(bool), typeof(WebApiXmlSettings), new PropertyMetadata(false));

		public NamespaceHandling NamespaceHandling { get { return (NamespaceHandling)GetValue(NamespaceHandlingProperty); } set { SetValue(NamespaceHandlingProperty, value); } }
		public static readonly DependencyProperty NamespaceHandlingProperty = DependencyProperty.Register("NamespaceHandling", typeof(NamespaceHandling), typeof(WebApiXmlSettings), new PropertyMetadata(NamespaceHandling.Default));

		public NewLineHandling NewLineHandling { get { return (NewLineHandling)GetValue(NewLineHandlingProperty); } set { SetValue(NewLineHandlingProperty, value); } }
		public static readonly DependencyProperty NewLineHandlingProperty = DependencyProperty.Register("NewLineHandling", typeof(NewLineHandling), typeof(WebApiXmlSettings), new PropertyMetadata(NewLineHandling.Entitize));

		public bool NewLineOnAttributes { get { return (bool)GetValue(NewLineOnAttributesProperty); } set { SetValue(NewLineOnAttributesProperty, value); } }
		public static readonly DependencyProperty NewLineOnAttributesProperty = DependencyProperty.Register("NewLineOnAttributes", typeof(bool), typeof(WebApiXmlSettings), new PropertyMetadata(false));

		public bool OmitXmlDeclaration { get { return (bool)GetValue(OmitXmlDeclarationProperty); } set { SetValue(OmitXmlDeclarationProperty, value); } }
		public static readonly DependencyProperty OmitXmlDeclarationProperty = DependencyProperty.Register("OmitXmlDeclaration", typeof(bool), typeof(WebApiXmlSettings), new PropertyMetadata(false));
	}
}