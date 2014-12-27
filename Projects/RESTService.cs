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

namespace NETPath.Projects
{
	public class RestService : DataType
	{
		public ObservableCollection<RestMethod> ServiceOperations { get { return (ObservableCollection<RestMethod>)GetValue(ServiceOperationsProperty); } set { SetValue(ServiceOperationsProperty, value); } }
		public static readonly DependencyProperty ServiceOperationsProperty = DependencyProperty.Register("ServiceOperations", typeof(ObservableCollection<RestMethod>), typeof(RestService));

		public ObservableCollection<RestHttpConfiguration> RequestConfigurations { get { return (ObservableCollection<RestHttpConfiguration>)GetValue(RequestConfigurationsProperty); } set { SetValue(RequestConfigurationsProperty, value); } }
		public static readonly DependencyProperty RequestConfigurationsProperty = DependencyProperty.Register("RequestConfigurations", typeof(ObservableCollection<RestHttpConfiguration>), typeof(ObservableCollection<RestHttpConfiguration>));

		public Documentation ServiceDocumentation { get { return (Documentation)GetValue(ServiceDocumentationProperty); } set { SetValue(ServiceDocumentationProperty, value); } }
		public static readonly DependencyProperty ServiceDocumentationProperty = DependencyProperty.Register("ServiceDocumentation", typeof(Documentation), typeof(RestService));

		public Documentation ClientDocumentation { get { return (Documentation)GetValue(ClientDocumentationProperty); } set { SetValue(ClientDocumentationProperty, value); } }
		public static readonly DependencyProperty ClientDocumentationProperty = DependencyProperty.Register("ClientDocumentation", typeof(Documentation), typeof(RestService));

		//Host

		//Endpoint
		public string EndpointAddress { get { return (string)GetValue(EndpointAddressProperty); } set { SetValue(EndpointAddressProperty, value); } }
		public static readonly DependencyProperty EndpointAddressProperty = DependencyProperty.Register("EndpointAddress", typeof(string), typeof(RestService));

		public int EndpointPort { get { return (int)GetValue(EndpointPortProperty); } set { SetValue(EndpointPortProperty, value); } }
		public static readonly DependencyProperty EndpointPortProperty = DependencyProperty.Register("EndpointPort", typeof(int), typeof(RestService));

		public bool EndpointUseHTTPS { get { return (bool)GetValue(EndpointUseHTTPSProperty); } set { SetValue(EndpointUseHTTPSProperty, value); } }
		public static readonly DependencyProperty EndpointUseHTTPSProperty = DependencyProperty.Register("EndpointUseHTTPS", typeof(bool), typeof(RestService), new PropertyMetadata(false));

		public RestService() : base(DataTypeMode.Class)
		{
			ID = Guid.NewGuid();
			ServiceOperations = new ObservableCollection<RestMethod>();
			RequestConfigurations = new ObservableCollection<RestHttpConfiguration>();
			ServiceDocumentation = new Documentation { IsClass = true };
		}

		public RestService(string Name, Namespace Parent) : base(DataTypeMode.Class)
		{
			this.Name = Name;
			this.Parent = Parent;
			ServiceOperations = new ObservableCollection<RestMethod>();
			RequestConfigurations = new ObservableCollection<RestHttpConfiguration>();
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

			foreach (RestMethod o in ServiceOperations)
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

	public enum RestSerialization
	{
		JSON,
		BSON,
		XmlSerializer,
		DataContract,
	}

	public class RestMethod : DependencyObject
	{
		public Guid ID { get; set; }

		public DataType ReturnType { get { return (DataType)GetValue(ReturnTypeProperty); } set { SetValue(ReturnTypeProperty, value); } }
		public static readonly DependencyProperty ReturnTypeProperty = DependencyProperty.Register("ReturnType", typeof(DataType), typeof(RestMethod), new PropertyMetadata(ReturnTypeChangedCallback));

		private static void ReturnTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs p)
		{
			var de = o as RestMethod;
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
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(RestMethod));

		public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(RestMethod), new PropertyMetadata(false));

		public bool ServerAsync { get { return (bool)GetValue(ServerAsyncProperty); } set { SetValue(ServerAsyncProperty, value); } }
		public static readonly DependencyProperty ServerAsyncProperty = DependencyProperty.Register("ServerAsync", typeof(bool), typeof(RestMethod), new PropertyMetadata(true));

		public bool ClientAsync { get { return (bool)GetValue(ClientAsyncProperty); } set { SetValue(ClientAsyncProperty, value); } }
		public static readonly DependencyProperty ClientAsyncProperty = DependencyProperty.Register("ClientAsync", typeof(bool), typeof(RestMethod), new PropertyMetadata(true));

		public ObservableCollection<RestRouteParameter> Parameters { get { return (ObservableCollection<RestRouteParameter>)GetValue(ParametersProperty); } set { SetValue(ParametersProperty, value); } }
		public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(ObservableCollection<RestRouteParameter>), typeof(RestMethod));

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(RestMethod));

		//Preamble Code Injection
		public string ServerPreambleCode { get { return (string)GetValue(ServerPreambleCodeProperty); } set { SetValue(ServerPreambleCodeProperty, value); } }
		public static readonly DependencyProperty ServerPreambleCodeProperty = DependencyProperty.Register("ServerPreambleCode", typeof(string), typeof(RestMethod));

		public string ClientPreambleCode { get { return (string)GetValue(ClientPreambleCodeProperty); } set { SetValue(ClientPreambleCodeProperty, value); } }
		public static readonly DependencyProperty ClientPreambleCodeProperty = DependencyProperty.Register("ClientPreambleCode", typeof(string), typeof(RestMethod));

		//Rest
		public HttpMethod Method { get { return (HttpMethod)GetValue(MethodProperty); } set { SetValue(MethodProperty, value); } }
		public static readonly DependencyProperty MethodProperty = DependencyProperty.Register("Method", typeof(HttpMethod), typeof(RestMethod), new PropertyMetadata(HttpMethod.Get));

		public RestHttpConfiguration RequestConfiguration { get { return (RestHttpConfiguration)GetValue(RequestConfigurationProperty); } set { SetValue(RequestConfigurationProperty, value); } }
		public static readonly DependencyProperty RequestConfigurationProperty = DependencyProperty.Register("RequestConfiguration", typeof(RestHttpConfiguration), typeof(RestMethod), new PropertyMetadata(null));

		public bool DeserializeContent { get { return (bool)GetValue(DeserializeContentProperty); } set { SetValue(DeserializeContentProperty, value); } }
		public static readonly DependencyProperty DeserializeContentProperty = DependencyProperty.Register("DeserializeContent", typeof(bool), typeof(RestMethod), new PropertyMetadata(true));

		public bool ReturnResponseData { get { return (bool)GetValue(ReturnResponseDataProperty); } set { SetValue(ReturnResponseDataProperty, value); } }
		public static readonly DependencyProperty ReturnResponseDataProperty = DependencyProperty.Register("ReturnResponseData", typeof(bool), typeof(RestMethod), new PropertyMetadata(false));

		public bool EnsureSuccessStatusCode { get { return (bool)GetValue(EnsureSuccessStatusCodeProperty); } set { SetValue(EnsureSuccessStatusCodeProperty, value); } }
		public static readonly DependencyProperty EnsureSuccessStatusCodeProperty = DependencyProperty.Register("EnsureSuccessStatusCode", typeof(bool), typeof(RestMethod), new PropertyMetadata(false));

		//System
		[IgnoreDataMember]
		public string Declaration { get { return (string)GetValue(DeclarationProperty); } protected set { SetValue(DeclarationPropertyKey, value); } }
		private static readonly DependencyPropertyKey DeclarationPropertyKey = DependencyProperty.RegisterReadOnly("Declaration", typeof(string), typeof(RestMethod), new PropertyMetadata(""));
		public static readonly DependencyProperty DeclarationProperty = DeclarationPropertyKey.DependencyProperty;

		public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(RestMethod), new PropertyMetadata(false));

		public RestService Owner { get { return (RestService)GetValue(OwnerProperty); } set { SetValue(OwnerProperty, value); } }
		public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register("Owner", typeof(RestService), typeof(RestMethod));

		public RestMethod()
		{
			ID = Guid.NewGuid();
			Parameters = new ObservableCollection<RestRouteParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			Documentation = new Documentation { IsMethod = true };
		}

		public RestMethod(string Name, RestService Owner)
		{
			ID = Guid.NewGuid();
			this.Name = Name;
			ReturnType = new DataType(PrimitiveTypes.Void);
			this.Owner = Owner;
			Parameters = new ObservableCollection<RestRouteParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
			ReturnType = new DataType(PrimitiveTypes.Void);
			Documentation = new Documentation { IsMethod = true };
			RequestConfiguration = Owner.RequestConfigurations.FirstOrDefault();
		}

		public RestMethod(DataType ReturnType, string Name, RestService Owner)
		{
			ID = Guid.NewGuid();
			this.Name = Name;
			this.ReturnType = ReturnType;
			this.Owner = Owner;
			Parameters = new ObservableCollection<RestRouteParameter>();
			Parameters.CollectionChanged += Parameters_CollectionChanged;
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

			if (Parameters == null) return;
			if (e.Property == DeclarationProperty) return;

			UpdateDeclaration();
		}

		internal void UpdateDeclaration()
		{
			var sb = new StringBuilder();
			foreach (var p in Parameters)
				sb.AppendFormat("{0}, ", p);
			if (Parameters.Count > 0) sb.Remove(sb.Length - 2, 2);
			Declaration = string.Format("{0} {1}({2})", ReturnType, Name, sb);
		}


		public virtual IEnumerable<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			var results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Service || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
						if (!string.IsNullOrEmpty(Name))
							if (Name.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Server Name", Name, Owner.Parent.Owner, this));
							else if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Server Name", Name, Owner.Parent.Owner, this));
				}
				else if (!string.IsNullOrEmpty(Name)) if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Server Name", Name, Owner.Parent.Owner, this));

				if (Args.ReplaceAll)
				{
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							else if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
					}
					else if (!string.IsNullOrEmpty(Name)) Name = Args.RegexSearch.Replace(Name, Args.Replace);
				}
			}

			foreach (RestMethodParameter mp in Parameters)
				results.AddRange(mp.FindReplace(Args));

			return results;
		}

		public virtual void Replace(FindReplaceInfo Args, string Field)
		{
			if (!Args.ReplaceAll) return;
			if (Args.UseRegex == false)
			{
				if (Args.MatchCase == false)
					if (Field == "Server Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					else
					if (Field == "Server Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
			}
			else
				if (Field == "Server Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);

			foreach (RestMethodParameter mp in Parameters)
				mp.Replace(Args, Field);
		}
	}

	public class RestRouteParameter : DependencyObject
	{
		public string RouteName { get { return (string)GetValue(RouteNameProperty); } set { SetValue(RouteNameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value ?? "", @"")); } }
		public static readonly DependencyProperty RouteNameProperty = DependencyProperty.Register("RouteName", typeof(string), typeof(RestRouteParameter));

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value ?? "", @"")); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(RestRouteParameter));

		//Internal Use
		public RestService Owner { get; set; }
		public RestMethod Parent { get; set; }

		public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(RestRouteParameter), new PropertyMetadata(false));

		public virtual bool IsPath { get { return true; } set { } }

		public RestRouteParameter(string routeName, RestService Owner, RestMethod Parent)
		{
			RouteName = routeName;
			IsHidden = false;
			this.Owner = Owner;
			this.Parent = Parent;
		}
	}

	public class RestMethodParameter : RestRouteParameter
	{
		public Guid ID { get; set; }

		public DataType Type { get { return (DataType)GetValue(TypeProperty); } set { SetValue(TypeProperty, value); } }
		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(DataType), typeof(RestMethodParameter), new PropertyMetadata(TypeChangedCallback));

		private static void TypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs p)
		{
			var de = o as RestMethodParameter;
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
		public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(string), typeof(RestMethodParameter));

		//Rest Specific
		public override bool IsPath { get { return (bool)GetValue(IsPathProperty); } set { SetValue(IsPathProperty, value); } }
		public static readonly DependencyProperty IsPathProperty = DependencyProperty.Register("IsPath", typeof(bool), typeof(RestMethodParameter), new PropertyMetadata(true, IsPathChangedCallback));

		private static void IsPathChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var de = o as RestMethodParameter;
			if (de == null) return;
			if (Convert.ToBoolean(e.NewValue) == false) return;
			de.IsQuery = false;
		}

		public bool IsQuery { get { return (bool)GetValue(IsQueryProperty); } set { SetValue(IsQueryProperty, value); } }
		public static readonly DependencyProperty IsQueryProperty = DependencyProperty.Register("IsQuery", typeof(bool), typeof(RestMethodParameter), new PropertyMetadata(false, IsQueryChangedCallback));

		private static void IsQueryChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var de = o as RestMethodParameter;
			if (de == null) return;
			if (Convert.ToBoolean(e.NewValue) == false) return;
			de.IsPath = false;
		}

		[IgnoreDataMember]
		public bool IsSerializable { get { return (bool)GetValue(IsSerializableProperty); } protected set { SetValue(IsSerializablePropertyKey, value); } }
		private static readonly DependencyPropertyKey IsSerializablePropertyKey = DependencyProperty.RegisterReadOnly("IsSerializable", typeof(bool), typeof(RestMethodParameter), new PropertyMetadata(false));
		public static readonly DependencyProperty IsSerializableProperty = IsSerializablePropertyKey.DependencyProperty;

		public bool Serialize { get { return (bool)GetValue(SerializeProperty); } set { SetValue(SerializeProperty, value); } }
		public static readonly DependencyProperty SerializeProperty = DependencyProperty.Register("Serialize", typeof(bool), typeof(RestMethodParameter), new PropertyMetadata(false, SerializeChangedCallback));

		private static void SerializeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var de = o as RestMethodParameter;
			if (de == null) return;
			if (Convert.ToBoolean(e.NewValue) == false) return;

			foreach (var t in de.Parent.Parameters.OfType<RestMethodParameter>().Where(a => !Equals(a, de)))
				t.Serialize = false;
			de.Serialize = true;
			de.IsPath = false;
			de.IsQuery = false;
		}

		[IgnoreDataMember]
		public bool IsNullable { get { return (bool)GetValue(IsNullableProperty); } protected set { SetValue(IsNullablePropertyKey, value); } }
		private static readonly DependencyPropertyKey IsNullablePropertyKey = DependencyProperty.RegisterReadOnly("IsNullable", typeof(bool), typeof(RestMethodParameter), new PropertyMetadata(false));
		public static readonly DependencyProperty IsNullableProperty = IsNullablePropertyKey.DependencyProperty;

		public bool Nullable { get { return (bool)GetValue(NullableProperty); } set { SetValue(NullableProperty, value); } }
		public static readonly DependencyProperty NullableProperty = DependencyProperty.Register("Nullable", typeof(bool), typeof(RestMethodParameter), new PropertyMetadata(false));

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(RestMethodParameter));

		public RestMethodParameter(DataType Type, string Name, RestService Owner, RestMethod Parent) : base(Name, Owner, Parent)
		{
			ID = Guid.NewGuid();
			this.Type = Type;
			this.Name = Name;
			Documentation = new Documentation { IsParameter = true };

			IsSerializable = false;
			IsNullable = false;
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

		public IEnumerable<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			var results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Service || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (!string.IsNullOrEmpty(Name))
							if (Name.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0)
								results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
					}
					else
					{
						if (!string.IsNullOrEmpty(Name))
							if (Name.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0)
								results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(Name))
						if (Args.RegexSearch.IsMatch(Name))
							results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
				}

				if (Args.ReplaceAll)
				{
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (!string.IsNullOrEmpty(Name))
								Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1,
									Microsoft.VisualBasic.CompareMethod.Text);
						}
						else
						{
							if (!string.IsNullOrEmpty(Name))
								Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(Name))
							Name = Args.RegexSearch.Replace(Name, Args.Replace);
					}
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

	public abstract class RestHttpConfiguration : DependencyObject
	{
		//System
		public Guid ID { get; set; }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(RestHttpConfiguration), new PropertyMetadata(""));

		public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(RestHttpConfiguration), new PropertyMetadata(false));

		//HTTP Request Configuration
		public bool AllowAutoRedirect { get { return (bool)GetValue(AllowAutoRedirectProperty); } set { SetValue(AllowAutoRedirectProperty, value); } }
		public static readonly DependencyProperty AllowAutoRedirectProperty = DependencyProperty.Register("AllowAutoRedirect", typeof(bool), typeof(RestHttpConfiguration), new PropertyMetadata(true));

		public DecompressionMethods AutomaticDecompression { get { return (DecompressionMethods)GetValue(AutomaticDecompressionProperty); } set { SetValue(AutomaticDecompressionProperty, value); } }
		public static readonly DependencyProperty AutomaticDecompressionProperty = DependencyProperty.Register("AutomaticDecompression", typeof(DecompressionMethods), typeof(RestHttpConfiguration), new PropertyMetadata(DecompressionMethods.None));

		public int MaxAutomaticRedirections { get { return (int)GetValue(MaxAutomaticRedirectionsProperty); } set { SetValue(MaxAutomaticRedirectionsProperty, value); } }
		public static readonly DependencyProperty MaxAutomaticRedirectionsProperty = DependencyProperty.Register("MaxAutomaticRedirections", typeof(int), typeof(RestHttpConfiguration), new PropertyMetadata(50));

		public bool UseProxy { get { return (bool)GetValue(UseProxyProperty); } set { SetValue(UseProxyProperty, value); } }
		public static readonly DependencyProperty UseProxyProperty = DependencyProperty.Register("UseProxy", typeof(bool), typeof(RestHttpConfiguration), new PropertyMetadata(false));

		public bool UseHTTP10 { get { return (bool)GetValue(UseHTTP10Property); } set { SetValue(UseHTTP10Property, value); } }
		public static readonly DependencyProperty UseHTTP10Property = DependencyProperty.Register("UseHTTP10", typeof(bool), typeof(RestHttpConfiguration), new PropertyMetadata(false));

		public bool UriIncludesMethodName { get { return (bool)GetValue(UriIncludesMethodNameProperty); } set { SetValue(UriIncludesMethodNameProperty, value); } }
		public static readonly DependencyProperty UriIncludesMethodNameProperty = DependencyProperty.Register("UriIncludesMethodName", typeof(bool), typeof(RestHttpConfiguration), new PropertyMetadata(false));

		//Security
		public CredentialsMode CredentialsMode { get { return (CredentialsMode)GetValue(CredentialsModeProperty); } set { SetValue(CredentialsModeProperty, value); } }
		public static readonly DependencyProperty CredentialsModeProperty = DependencyProperty.Register("CredentialsMode", typeof(CredentialsMode), typeof(RestHttpConfiguration), new PropertyMetadata(CredentialsMode.None));

		public CookieContainerMode CookieContainerMode { get { return (CookieContainerMode)GetValue(CookieContainerModeProperty); } set { SetValue(CookieContainerModeProperty, value); } }
		public static readonly DependencyProperty CookieContainerModeProperty = DependencyProperty.Register("CookieContainerMode", typeof(CookieContainerMode), typeof(RestHttpConfiguration), new PropertyMetadata(CookieContainerMode.Instance));

		public bool PreAuthenticate { get { return (bool)GetValue(PreAuthenticateProperty); } set { SetValue(PreAuthenticateProperty, value); } }
		public static readonly DependencyProperty PreAuthenticateProperty = DependencyProperty.Register("PreAuthenticate", typeof(bool), typeof(RestHttpConfiguration), new PropertyMetadata(false));

		public bool UseDefaultCredentials { get { return (bool)GetValue(UseDefaultCredentialsProperty); } set { SetValue(UseDefaultCredentialsProperty, value); } }
		public static readonly DependencyProperty UseDefaultCredentialsProperty = DependencyProperty.Register("UseDefaultCredentials", typeof(bool), typeof(RestHttpConfiguration), new PropertyMetadata(false));

		public ClientCertificateOption ClientCertificateOptions { get { return (ClientCertificateOption)GetValue(ClientCertificateOptionsProperty); } set { SetValue(ClientCertificateOptionsProperty, value); } }
		public static readonly DependencyProperty ClientCertificateOptionsProperty = DependencyProperty.Register("ClientCertificateOptions", typeof(ClientCertificateOption), typeof(RestHttpConfiguration), new PropertyMetadata(ClientCertificateOption.Automatic));

		public long MaxRequestContentBufferSize { get { return (long)GetValue(MaxRequestContentBufferSizeProperty); } set { SetValue(MaxRequestContentBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxRequestContentBufferSizeProperty = DependencyProperty.Register("MaxRequestContentBufferSize", typeof(long), typeof(RestHttpConfiguration), new PropertyMetadata(2147483647L));

		public ContentMode ContentMode { get { return (ContentMode)GetValue(ContentModeProperty); } set { SetValue(ContentModeProperty, value); } }
		public static readonly DependencyProperty ContentModeProperty = DependencyProperty.Register("ContentMode", typeof(ContentMode), typeof(RestHttpConfiguration), new PropertyMetadata(ContentMode.Default));

		public RestHttpConfiguration()
		{
			ID = Guid.NewGuid();
		}

		public RestHttpConfiguration(string Name)
		{
			ID = Guid.NewGuid();
			this.Name = Name;
		}
	}
}