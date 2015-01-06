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

namespace NETPath.Projects.WebApi
{
	public class WebApiService : DataType
	{
		public ObservableCollection<WebApiMethod> ServiceOperations { get { return (ObservableCollection<WebApiMethod>)GetValue(ServiceOperationsProperty); } set { SetValue(ServiceOperationsProperty, value); } }
		public static readonly DependencyProperty ServiceOperationsProperty = DependencyProperty.Register("ServiceOperations", typeof(ObservableCollection<WebApiMethod>), typeof(WebApiService));

		public ObservableCollection<WebApiHttpConfiguration> RequestConfigurations { get { return (ObservableCollection<WebApiHttpConfiguration>)GetValue(RequestConfigurationsProperty); } set { SetValue(RequestConfigurationsProperty, value); } }
		public static readonly DependencyProperty RequestConfigurationsProperty = DependencyProperty.Register("RequestConfigurations", typeof(ObservableCollection<WebApiHttpConfiguration>), typeof(ObservableCollection<WebApiHttpConfiguration>));

		public Documentation ServiceDocumentation { get { return (Documentation)GetValue(ServiceDocumentationProperty); } set { SetValue(ServiceDocumentationProperty, value); } }
		public static readonly DependencyProperty ServiceDocumentationProperty = DependencyProperty.Register("ServiceDocumentation", typeof(Documentation), typeof(WebApiService));

		public Documentation ClientDocumentation { get { return (Documentation)GetValue(ClientDocumentationProperty); } set { SetValue(ClientDocumentationProperty, value); } }
		public static readonly DependencyProperty ClientDocumentationProperty = DependencyProperty.Register("ClientDocumentation", typeof(Documentation), typeof(WebApiService));

		//Host

		public WebApiService() : base(DataTypeMode.Class)
		{
			ID = Guid.NewGuid();
			ServiceOperations = new ObservableCollection<WebApiMethod>();
			RequestConfigurations = new ObservableCollection<WebApiHttpConfiguration>();
			ServiceDocumentation = new Documentation { IsClass = true };
		}

		public WebApiService(string Name, Namespace Parent) : base(DataTypeMode.Class)
		{
			this.Name = Name;
			this.Parent = Parent;
			ServiceOperations = new ObservableCollection<WebApiMethod>();
			RequestConfigurations = new ObservableCollection<WebApiHttpConfiguration>();
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
		Trace,
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

		//Preamble Code Injection
		public string ServerPreambleCode { get { return (string)GetValue(ServerPreambleCodeProperty); } set { SetValue(ServerPreambleCodeProperty, value); } }
		public static readonly DependencyProperty ServerPreambleCodeProperty = DependencyProperty.Register("ServerPreambleCode", typeof(string), typeof(WebApiMethod));

		public string ClientPreambleCode { get { return (string)GetValue(ClientPreambleCodeProperty); } set { SetValue(ClientPreambleCodeProperty, value); } }
		public static readonly DependencyProperty ClientPreambleCodeProperty = DependencyProperty.Register("ClientPreambleCode", typeof(string), typeof(WebApiMethod));

		//Rest
		public WebApiMethodVerbs Method { get { return (WebApiMethodVerbs)GetValue(MethodProperty); } set { SetValue(MethodProperty, value); } }
		public static readonly DependencyProperty MethodProperty = DependencyProperty.Register("Method", typeof(WebApiMethodVerbs), typeof(WebApiMethod), new PropertyMetadata(WebApiMethodVerbs.Get));

		public WebApiHttpConfiguration RequestConfiguration { get { return (WebApiHttpConfiguration)GetValue(RequestConfigurationProperty); } set { SetValue(RequestConfigurationProperty, value); } }
		public static readonly DependencyProperty RequestConfigurationProperty = DependencyProperty.Register("RequestConfiguration", typeof(WebApiHttpConfiguration), typeof(WebApiMethod), new PropertyMetadata(null));

		public bool DeserializeContent { get { return (bool)GetValue(DeserializeContentProperty); } set { SetValue(DeserializeContentProperty, value); } }
		public static readonly DependencyProperty DeserializeContentProperty = DependencyProperty.Register("DeserializeContent", typeof(bool), typeof(WebApiMethod), new PropertyMetadata(true));

		public bool ReturnResponseData { get { return (bool)GetValue(ReturnResponseDataProperty); } set { SetValue(ReturnResponseDataProperty, value); } }
		public static readonly DependencyProperty ReturnResponseDataProperty = DependencyProperty.Register("ReturnResponseData", typeof(bool), typeof(WebApiMethod), new PropertyMetadata(false));

		public bool EnsureSuccessStatusCode { get { return (bool)GetValue(EnsureSuccessStatusCodeProperty); } set { SetValue(EnsureSuccessStatusCodeProperty, value); } }
		public static readonly DependencyProperty EnsureSuccessStatusCodeProperty = DependencyProperty.Register("EnsureSuccessStatusCode", typeof(bool), typeof(WebApiMethod), new PropertyMetadata(false));

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
			RequestConfiguration = Owner.RequestConfigurations.FirstOrDefault();
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
			RequestConfiguration = Owner.RequestConfigurations.FirstOrDefault();
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

			de.IsSerializable = false;
			de.IsNullable = false;
			if (nt.TypeMode == DataTypeMode.Array || nt.TypeMode == DataTypeMode.Class || nt.TypeMode == DataTypeMode.Collection || nt.TypeMode == DataTypeMode.Dictionary || nt.TypeMode == DataTypeMode.Struct || nt.TypeMode == DataTypeMode.Queue || nt.TypeMode == DataTypeMode.Stack) de.IsSerializable = true;
			if (nt.TypeMode == DataTypeMode.Primitive && (nt.Primitive == PrimitiveTypes.ByteArray || nt.Primitive == PrimitiveTypes.String)) de.IsSerializable = true;
			if (nt.TypeMode == DataTypeMode.Primitive && nt.Primitive != PrimitiveTypes.ByteArray && nt.Primitive != PrimitiveTypes.String && nt.Primitive != PrimitiveTypes.Object) de.IsNullable = true;
			if (nt.TypeMode == DataTypeMode.Struct || nt.TypeMode == DataTypeMode.Enum) de.IsNullable = true;
		}

		public string DefaultValue { get { return (string)GetValue(DefaultValueProperty); } set { SetValue(DefaultValueProperty, Helpers.RegExs.ReplaceSpaces.Replace(value ?? "", @"")); } }
		public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(string), typeof(WebApiMethodParameter));

		//Rest Specific
		[IgnoreDataMember]
		public bool IsSerializable { get { return (bool)GetValue(IsSerializableProperty); } protected set { SetValue(IsSerializablePropertyKey, value); } }
		private static readonly DependencyPropertyKey IsSerializablePropertyKey = DependencyProperty.RegisterReadOnly("IsSerializable", typeof(bool), typeof(WebApiMethodParameter), new PropertyMetadata(false));
		public static readonly DependencyProperty IsSerializableProperty = IsSerializablePropertyKey.DependencyProperty;

		public bool Serialize { get { return (bool)GetValue(SerializeProperty); } set { SetValue(SerializeProperty, value); } }
		public static readonly DependencyProperty SerializeProperty = DependencyProperty.Register("Serialize", typeof(bool), typeof(WebApiMethodParameter), new PropertyMetadata(false, SerializeChangedCallback));

		private static void SerializeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var de = o as WebApiMethodParameter;
			if (de == null) return;
			if (Convert.ToBoolean(e.NewValue) == false) return;

			foreach (var t in de.Parent.QueryParameters.Where(a => !Equals(a, de)))
				t.Serialize = false;
			de.Serialize = true;
		}

		[IgnoreDataMember]
		public bool IsNullable { get { return (bool)GetValue(IsNullableProperty); } protected set { SetValue(IsNullablePropertyKey, value); } }
		private static readonly DependencyPropertyKey IsNullablePropertyKey = DependencyProperty.RegisterReadOnly("IsNullable", typeof(bool), typeof(WebApiMethodParameter), new PropertyMetadata(false));
		public static readonly DependencyProperty IsNullableProperty = IsNullablePropertyKey.DependencyProperty;

		public bool Nullable { get { return (bool)GetValue(NullableProperty); } set { SetValue(NullableProperty, value); } }
		public static readonly DependencyProperty NullableProperty = DependencyProperty.Register("Nullable", typeof(bool), typeof(WebApiMethodParameter), new PropertyMetadata(false));

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

			IsSerializable = false;
			IsNullable = false;
			IsHidden = false;
			if (Type.TypeMode == DataTypeMode.Array || Type.TypeMode == DataTypeMode.Class || Type.TypeMode == DataTypeMode.Collection || Type.TypeMode == DataTypeMode.Dictionary || Type.TypeMode == DataTypeMode.Struct || Type.TypeMode == DataTypeMode.Queue || Type.TypeMode == DataTypeMode.Stack) IsSerializable = true;
			if (Type.TypeMode == DataTypeMode.Primitive && (Type.Primitive == PrimitiveTypes.ByteArray || Type.Primitive == PrimitiveTypes.String)) IsSerializable = true;
			if ((Type.TypeMode == DataTypeMode.Primitive && Type.Primitive != PrimitiveTypes.ByteArray && Type.Primitive != PrimitiveTypes.String && Type.Primitive != PrimitiveTypes.Object)) IsNullable = true;
			if (Type.TypeMode == DataTypeMode.Struct || Type.TypeMode == DataTypeMode.Enum) IsNullable = true;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			Parent?.UpdateDeclaration();
		}

		public override string ToString()
		{
			return string.Format("{0}{3} {1}{2}", Type, Name, string.IsNullOrWhiteSpace(DefaultValue) ? "" : string.Format(" = {0}", DefaultValue), Nullable ? "?" : "");
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
		//System
		public Guid ID { get; set; }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(WebApiHttpConfiguration), new PropertyMetadata(""));

		public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(WebApiHttpConfiguration), new PropertyMetadata(false));

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

		public WebApiHttpConfiguration()
		{
			ID = Guid.NewGuid();
		}

		public WebApiHttpConfiguration(string Name)
		{
			ID = Guid.NewGuid();
			this.Name = Name;
		}
	}
}