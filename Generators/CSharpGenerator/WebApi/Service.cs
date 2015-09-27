using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using NETPath.Projects;
using NETPath.Projects.WebApi;
using NETPath.Projects.Helpers;

namespace NETPath.Generators.CS.WebApi
{
	internal static class ServiceGenerator
	{

		public static void VerifyCode(WebApiService o, Action<CompileMessage> AddMessage)
		{
			if (string.IsNullOrEmpty(o.Name))
				AddMessage(new CompileMessage("GS2000", "An service in the '" + o.Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));
			else if (RegExs.MatchCodeName.IsMatch(o.Name) == false)
				AddMessage(new CompileMessage("GS2001", "The service '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));
			if (o.HasClientType)
				if (RegExs.MatchCodeName.IsMatch(o.ClientType.Name) == false)
					AddMessage(new CompileMessage("GS2002", "The service '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));

			var Operations = new ObservableCollection<WebApiMethod>();
			Operations.AddRange(o.ServiceOperations);

			foreach (WebApiMethod m in Operations)
			{
				if (string.IsNullOrEmpty(m.Name))
					AddMessage(new CompileMessage("GS2004", "An method in the '" + o.Name + "' service has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
				else
				{
					if (RegExs.MatchCodeName.IsMatch(m.Name) == false)
						AddMessage(new CompileMessage("GS2005", "The method '" + m.Name + "' in the '" + o.Name + "' service contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
					if (m.Name == "__callback")
						AddMessage(new CompileMessage("GS2015", "The name of the method '" + m.Name + "' in the '" + o.Name + "' service is invalid. Please rename it.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
				}

				if (m.ReturnType == null)
					AddMessage(new CompileMessage("GS2007", "The method '" + m.Name + "' in the '" + o.Name + "' service has a blank Return Type. A Return Type MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
				else if (m.ReturnType.TypeMode == DataTypeMode.Primitive && m.ReturnType.Primitive == PrimitiveTypes.None)
					AddMessage(new CompileMessage("GS2012", "The method return type '" + m.ReturnType + "' in the '" + o.Name + "' service is not a valid Rest return type. Please specify a valid Rest return type.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
				if (m.ReturnType.TypeMode == DataTypeMode.Namespace || m.ReturnType.TypeMode == DataTypeMode.Interface)
					AddMessage(new CompileMessage("GS2013", "The method return type '" + m.ReturnType + "' in the '" + o.Name + "' service is not a valid return type. Please specify a valid return type.", CompileMessageSeverity.ERROR, o, m, m.GetType()));

				foreach (var mp in m.RouteParameters.OfType<WebApiMethodParameter>())
				{
					if (string.IsNullOrEmpty(mp.Name))
						AddMessage(new CompileMessage("GS2008", "The method parameter '" + m.Name + "' in the '" + o.Name + "' service has a blank parameter name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
					if (string.IsNullOrEmpty(mp.Name))
						AddMessage(new CompileMessage("GS20018", "The method parameter '" + m.Name + "' in the '" + o.Name + "' service has a blank route name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
					if (mp.Name == "__callback")
						AddMessage(new CompileMessage("GS2016", "The name of the method parameter '" + mp.Name + "' in the '" + m.Name + "' method is invalid. Please rename it.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
				}

				foreach (var mp in m.QueryParameters)
				{
					//Automatically enforce query parameter config
					mp.Type.IsNullable = true;
					mp.DefaultValue = "null";
					if (string.IsNullOrEmpty(mp.Name))
						AddMessage(new CompileMessage("GS2008", "The method parameter '" + m.Name + "' in the '" + o.Name + "' service has a blank parameter name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
					if (string.IsNullOrEmpty(mp.Name))
						AddMessage(new CompileMessage("GS20018", "The method parameter '" + m.Name + "' in the '" + o.Name + "' service has a blank route name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
					//if (string.IsNullOrEmpty(mp.DefaultValue))
					//	AddMessage(new CompileMessage("GS2009", "The method  parameter '" + m.Name + "' in the '" + o.Name + "' method does not have a default value. Please specify a default value for this parameter.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
					if (mp.Name == "__callback")
						AddMessage(new CompileMessage("GS2016", "The name of the method parameter '" + mp.Name + "' in the '" + m.Name + "' method is invalid. Please rename it.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
				}
			}
		}

		#region - Server Interfaces -

		public static string GenerateServerCode(WebApiService o)
		{
			var code = new StringBuilder();

			//Generate the service proxy
			if (o.ServiceDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ServiceDocumentation));
			code.AppendLine(string.Format("\t{0} abstract class {1}Base : ApiController", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
			code.AppendLine("\t{");

			code.AppendLine();
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Constructors");
				code.AppendLine();
			}

			code.AppendLine(string.Format("\t\tpublic {0}Base() : base()", o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tInitialize();");
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine();
			code.AppendLine("\t\tprivate void Initialize()");
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t}");
			code.AppendLine();
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Methods");
				code.AppendLine();
			}

			foreach (WebApiMethod m in o.ServiceOperations)
				code.AppendLine(GenerateServerProxyMethod(m));
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}
			code.AppendLine("\t}");

			return code.ToString();
		}

		#endregion

		#region - Client Interfaces -

		public static string GenerateClientCode45(WebApiService o)
		{
			var code = new StringBuilder();

			//Generate the Proxy Class
			if (o.ClientDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ClientDocumentation));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCode(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} {2}partial class {1}{3}", DataTypeGenerator.GenerateScope(o.Scope), o.Name, o.Abstract ? "abstract " : "", o.Abstract ? "Base" : ""));
			code.AppendLine("\t{");
			code.AppendLine();
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Request Configuration");
				code.AppendLine();
			}
			code.AppendLine("\t\tprivate readonly string _baseUri;");
			//code.AppendLine("\t\tprivate readonly CookieContainer _cookies;");
			//code.AppendLine("\t\tprivate readonly CredentialCache _credentialCache;");
			//code.AppendLine("\t\tprivate readonly NetworkCredential _credentials;");
			//code.AppendLine("\t\tprivate readonly IWebProxy _proxy;");
			//code.AppendLine("\t\tprivate readonly System.Net.Http.Headers.HttpRequestHeaders _requestHeaders = new System.Net.Http.HttpRequestMessage().Headers;");
			//code.AppendLine("\t\tprivate readonly System.Net.Http.Headers.HttpContentHeaders _contentHeaders = new System.Net.Http.StringContent(\"\").Headers;");

			code.AppendLine("\t\tpublic string BaseUri { get { return _baseUri; } }");
			//code.AppendLine("\t\tpublic CookieContainer Cookies { get { return _cookies; } }");
			//code.AppendLine("\t\tpublic CredentialCache CredentialCache { get { return _credentialCache; } }");
			//code.AppendLine("\t\tpublic NetworkCredential Credentials { get { return _credentials; } }");
			//code.AppendLine("\t\tpublic IWebProxy Proxy { get { return _proxy; } }");
			//code.AppendLine("\t\tpublic HttpRequestHeaders DefaultRequestHeaders { get { return _requestHeaders; } }");
			//code.AppendLine("\t\tpublic HttpContentHeaders DefaultContentHeaders { get { return _contentHeaders; } }");

			code.AppendLine(string.Format("\t\tprivate readonly System.Net.Http.HttpClient _HttpClient;"));
			code.AppendLine(string.Format("\t\tprotected System.Net.Http.HttpClient HttpClient {{ get {{ return _HttpClient; }} }}"));
			code.AppendLine();
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Constructors");
				code.AppendLine();
			}
			//code.AppendLine(string.Format("\t\t{1} {0}{2}(string BaseURI = \"{3}\", CookieContainer cookies = null, NetworkCredential credentials = null, CredentialCache credentialCache = null, IWebProxy proxy = null)", o.Name, o.Abstract ? "protected" : "public", o.Abstract ? "Base" : "", (o.Parent.Owner as WebApiProject)?.Namespace.URI));
			code.AppendLine(string.Format("\t\t{1} {0}{2}(System.Net.Http.HttpClient client, string BaseURI = \"{3}\")", o.Name, o.Abstract ? "protected" : "public", o.Abstract ? "Base" : "", (o.Parent.Owner as WebApiProject)?.Namespace.URI));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\t_baseUri = BaseURI;");
			code.AppendLine("\t\t\tif (_baseUri.EndsWith(\"/\")) _baseUri = _baseUri.Remove(_baseUri.Length - 1, 1);");
			code.AppendLine("\t\t\t_HttpClient = client;");
			//code.AppendLine("\t\t\t_cookies = cookies ?? new CookieContainer();");
			//code.AppendLine("\t\t\t_credentials = credentials;");
			//code.AppendLine("\t\t\t_credentialCache = credentialCache;");
			//code.AppendLine("\t\t\t_proxy = proxy;");
			//code.AppendLine(string.Format("\t\t\t_HttpClient = new System.Net.Http.HttpClient(new System.Net.Http.HttpClientHandler() {{ AllowAutoRedirect = {0}, AutomaticDecompression = System.Net.DecompressionMethods.{1}, ClientCertificateOptions = System.Net.Http.ClientCertificateOption.{2}, CookieContainer = {3}, Credentials = (this.Credentials == null) ? (ICredentials)this.CredentialCache : (ICredentials)this.Credentials, MaxAutomaticRedirections = {4}, MaxRequestContentBufferSize = {5}, PreAuthenticate = {6}, Proxy = this.Proxy, UseCookies = {7}, UseDefaultCredentials = {8}, UseProxy = {9} }});",
			//	o.RequestConfiguration.AllowAutoRedirect ? bool.TrueString.ToLower() : bool.FalseString.ToLower(), o.RequestConfiguration.AutomaticDecompression, o.RequestConfiguration.ClientCertificateOptions,
			//	o.RequestConfiguration.CookieContainerMode == CookieContainerMode.None ? "null" : o.RequestConfiguration.CookieContainerMode == CookieContainerMode.Instance ? "new CookieContainer()" : "Cookies",
			//	o.RequestConfiguration.MaxAutomaticRedirections, o.RequestConfiguration.MaxRequestContentBufferSize, o.RequestConfiguration.PreAuthenticate ? bool.TrueString.ToLower() : bool.FalseString.ToLower(),
			//	(o.RequestConfiguration.CookieContainerMode == CookieContainerMode.None || o.RequestConfiguration.CookieContainerMode == CookieContainerMode.Custom) ? bool.FalseString.ToLower() : bool.TrueString.ToLower(),
			//	o.RequestConfiguration.UseDefaultCredentials ? bool.TrueString.ToLower() : bool.FalseString.ToLower(), o.RequestConfiguration.UseProxy ? bool.TrueString.ToLower() : bool.FalseString.ToLower()));
			code.AppendLine("\t\t}");
			code.AppendLine();
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Methods");
				code.AppendLine();
			}
			foreach (WebApiMethod m in o.ServiceOperations)
				code.AppendLine(m.ClientAsync ? GenerateClientMethodClientAsync45(m) : GenerateClientMethodClient45(m));
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}
			code.AppendLine("\t}");

			return code.ToString();
		}

		#endregion

		#region - Server Controller Methods -

		private static string BuildUriTemplate(WebApiMethod o)
		{
			var uriBuilder = new StringBuilder(512);

			foreach (var pp in o.RouteParameters.Where(a => !(a is WebApiMethodParameter)))
				uriBuilder.AppendFormat("/{0}", pp.RouteName.ToLowerInvariant());

			uriBuilder.AppendFormat("/{0}", o.Name.ToLowerInvariant());

			foreach (var pp in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => string.IsNullOrEmpty(a.DefaultValue)))
				uriBuilder.AppendFormat("/{{{0}}}", pp.RouteName.ToLowerInvariant());

			foreach (var pp in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => !string.IsNullOrEmpty(a.DefaultValue)))
				uriBuilder.AppendFormat("/{{{0}?}}", pp.RouteName.ToLowerInvariant());

			uriBuilder.Remove(0, 1); //Remove the beginning slant from the Route

			return uriBuilder.ToString();
		}

		public static string GenerateServerProxyMethod(WebApiMethod o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (var mp in o.RouteParameters.OfType<WebApiMethodParameter>().Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				foreach (var mp in o.QueryParameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
			}
			code.AppendLine(string.Format("\t\t[System.Web.Http.Route(\"{0}\")]", BuildUriTemplate(o)));
			if (o.Method != WebApiMethodVerbs.Custom)
				code.AppendLine(string.Format("\t\t[System.Web.Http.Http{0}]", o.Method));
			else
				code.AppendLine(string.Format("\t\t[System.Web.Http.AcceptVerbs(\"{0}\")]", o.Method));

			if (o.HasContent && o.ContentType.TypeMode == DataTypeMode.Primitive && o.ContentType.Primitive == PrimitiveTypes.String)
			{
				code.AppendFormat("\t\tpublic async {0} {1}Base(", o.ServerAsync ? o.ReturnType.IsVoid ? "Task" : string.Format("Task<{0}>", DataTypeGenerator.GenerateType(o.ReturnType)) : DataTypeGenerator.GenerateType(o.ReturnType), o.Name);
				foreach (var op in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => string.IsNullOrEmpty(a.DefaultValue)))
					code.AppendFormat("{0}, ", GenerateMethodParameterServerCode(op));
				foreach (var op in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => !string.IsNullOrEmpty(a.DefaultValue)))
					code.AppendFormat("{0}, ", GenerateMethodParameterServerCode(op));
				foreach (var op in o.QueryParameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterServerCode(op));
				if (o.RouteParameters.OfType<WebApiMethodParameter>().Any() || o.QueryParameters.Any()) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\t{0}await {1}(", !o.ReturnType.IsVoid ? "return " : "", o.Name);
				foreach (var op in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => string.IsNullOrEmpty(a.DefaultValue)))
					code.AppendFormat("{0}, ", op.Name);
				foreach (var op in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => !string.IsNullOrEmpty(a.DefaultValue)))
					code.AppendFormat("{0}, ", op.Name);
				foreach (var op in o.QueryParameters)
					code.AppendFormat("{0}, ", op.Name);
				code.AppendLine("await Request.Content.ReadAsStringAsync());");
				code.AppendLine("\t\t}");
			}

			code.AppendFormat("\t\tpublic abstract {0} {1}(", o.ServerAsync ? o.ReturnType.IsVoid ? "Task" : string.Format("Task<{0}>", DataTypeGenerator.GenerateType(o.ReturnType)) : DataTypeGenerator.GenerateType(o.ReturnType), o.Name);
			foreach (var op in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => string.IsNullOrEmpty(a.DefaultValue)))
				code.AppendFormat("{0}, ", GenerateMethodParameterServerCode(op));
			foreach (var op in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => !string.IsNullOrEmpty(a.DefaultValue)))
				code.AppendFormat("{0}, ", GenerateMethodParameterServerCode(op));
			foreach (var op in o.QueryParameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterServerCode(op));
			if (o.RouteParameters.OfType<WebApiMethodParameter>().Any() || o.QueryParameters.Any()) code.Remove(code.Length - 2, 2);
			if (o.HasContent)
				code.AppendFormat("{2}[FromBody] {0} {1}", DataTypeGenerator.GenerateType(o.ContentType), o.ContentParameterName, (o.QueryParameters.Any() || o.RouteParameters.OfType<WebApiMethodParameter>().Any()) ? ", " : "");
			code.AppendLine(");");
			return code.ToString();
		}

		#endregion

		#region - Client HttpClient Methods -

		public static string GenerateClientMethodClient45(WebApiMethod o)
		{
			if (o.IsHidden) return "";
			var conf = o.Owner.RequestConfiguration;
			var code = new StringBuilder();

			//Generate documentation
			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (var mp in o.RouteParameters.OfType<WebApiMethodParameter>().Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				foreach (var mp in o.QueryParameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
			}

			//Construct method declaration
			code.AppendFormat("\t\tpublic virtual async void {0}(", o.Name);
			foreach (var op in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => string.IsNullOrEmpty(a.DefaultValue)))
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			foreach (var op in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => !string.IsNullOrEmpty(a.DefaultValue)))
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			foreach (var op in o.QueryParameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (!o.HasContent && (o.RouteParameters.OfType<WebApiMethodParameter>().Any() || o.QueryParameters.Any())) code.Remove(code.Length - 2, 2);
			if (o.HasContent)
				code.AppendFormat("{0} {1}", DataTypeGenerator.GenerateType(o.ContentType), o.ContentParameterName);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			GenerateMethodPreamble(code, o.ClientPreambleCode, 3);

			//Construct the URI
			code.AppendLine("\t\t\tvar uri = new StringBuilder(_baseUri, 2048);");
			code.Append("\t\t\turi.Append(\"");
			foreach (var op in o.RouteParameters)
			{
				if (op.GetType() == typeof (WebApiRouteParameter))
					code.Append(string.Format("/{0}", op.RouteName.ToLowerInvariant()));
			}
			code.Append(string.Format("/{0}", o.Name.ToLowerInvariant()));
			code.AppendLine("\");");
			foreach (var op in o.RouteParameters)
			{

				if (op.GetType() == typeof (WebApiMethodParameter))
					code.AppendLine(string.Format("\t\t\turi.AppendFormat(\"/{{0}}\", {0});", op.Name));
			}
			if (o.QueryParameters.Any())
				code.AppendLine("\t\t\turi.Append(\"?\");");
			foreach (var op in o.QueryParameters)
				code.AppendLine(string.Format("\t\t\tif ({1} != null) uri.AppendFormat(\"&{0}={{0}}\", {1});", op.RouteName.ToLowerInvariant(), op.Name));
			if (o.QueryParameters.Any())
				code.AppendLine("\t\t\turi.Replace(\"?&\", \"?\");");

			//Create the HttpRequestMessage
			code.AppendLine(string.Format("\t\t\tvar rm = new HttpRequestMessage(HttpMethod.{0}, new Uri(uri.ToString(), UriKind.RelativeOrAbsolute));", o.Method));
			if (conf.UseHTTP10)
				code.AppendLine("\t\t\trm.Version = new Version(1, 0););");
			//code.AppendLine("\t\t\trm.Headers.Clear();");
			//code.AppendLine("\t\t\tforeach (var x in _requestHeaders) rm.Headers.Add(x.Key, x.Value);");
			//code.AppendLine("\t\t\trm.Headers.Authorization = _requestHeaders.Authorization;");
			//code.AppendLine("\t\t\trm.Headers.CacheControl = _requestHeaders.CacheControl;");
			//code.AppendLine("\t\t\trm.Headers.Date = _requestHeaders.Date;");
			//code.AppendLine("\t\t\trm.Headers.From = _requestHeaders.From;");
			//code.AppendLine("\t\t\trm.Headers.Host = _requestHeaders.Host;");
			//code.AppendLine("\t\t\trm.Headers.IfModifiedSince = _requestHeaders.IfModifiedSince;");
			//code.AppendLine("\t\t\trm.Headers.IfRange = _requestHeaders.IfRange;");
			//code.AppendLine("\t\t\trm.Headers.IfUnmodifiedSince = _requestHeaders.IfUnmodifiedSince;");
			//code.AppendLine("\t\t\trm.Headers.MaxForwards = _requestHeaders.MaxForwards;");
			//code.AppendLine("\t\t\trm.Headers.ProxyAuthorization = _requestHeaders.ProxyAuthorization;");
			//code.AppendLine("\t\t\trm.Headers.Range = _requestHeaders.Range;");
			code.AppendLine("\t\t\trm.Headers.Accept.Clear();");
			code.AppendLine(string.Format("\t\t\trm.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(\"application/{0}\"));", conf.ResponseFormat == RestSerialization.Json ? "json" : conf.ResponseFormat == RestSerialization.Bson ? "bson" : "xml"));

			//Serialize any parameters
			if (o.HasContent)
			{
				var pt = o.ContentType;
				if(pt.TypeMode == DataTypeMode.Primitive && pt.Primitive == PrimitiveTypes.String)
					code.AppendLine(string.Format("\t\t\trm.Content = new System.Net.Http.StringContent({0});", o.ContentParameterName));
				else
					code.AppendLine(string.Format("\t\t\trm.Content = new System.Net.Http.ObjectContent<{0}>({1}, new {2}());", DataTypeGenerator.GenerateType(pt), o.ContentParameterName, conf.RequestFormat == RestSerialization.Json ? "JsonMediaTypeFormatter" : conf.RequestFormat == RestSerialization.Bson ? "BsonMediaTypeFormatter" : "XmlMediaTypeFormatter"));
				//code.AppendLine(string.Format("\t\t\tforeach (var x in _contentHeaders) rm.Content.Headers.Add(x.Key, x.Value);"));
			}

			code.AppendLine(string.Format("\t\t\tvar rr = await _HttpClient.SendAsync(rm, System.Net.Http.HttpCompletionOption.ResponseHeadersRead);"));
			if (o.EnsureSuccessStatusCode) code.AppendLine("\t\t\trr.EnsureSuccessStatusCode();");
			if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent)
			{
				code.AppendLine(string.Format("\t\t\t{0}Result({1});", o.Name, o.ReturnResponseData ? "rr" : ""));
			}
			else
			{
				code.AppendLine(string.Format("\t\t\tvar rs = await rr.Content.ReadAsAsync<{0}>(new MediaTypeFormatter[] {{ new {1}() }});", DataTypeGenerator.GenerateType(o.ReturnType), conf.ResponseFormat == RestSerialization.Json ? "JsonMediaTypeFormatter" : conf.ResponseFormat == RestSerialization.Bson ? "BsonMediaTypeFormatter" : "XmlMediaTypeFormatter"));
				code.AppendLine(string.Format("\t\t\t{0}Result(rs{1});", o.Name, o.ReturnResponseData ? ", rr" : ""));
			}
			code.AppendLine("\t\t}");
			if (o.ReturnResponseData)
			{
				if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendLine(string.Format("\t\tpublic abstract void {0}Result(System.Net.Http.HttpResponseMessage Response);", o.Name));
				else code.AppendLine(string.Format("\t\tpublic abstract void {0}Result({1} Value, System.Net.Http.HttpResponseMessage Response);", o.Name, DataTypeGenerator.GenerateType(o.ReturnType)));
			}
			else
			{
				if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendLine(string.Format("\t\tpublic abstract void {0}Result();", o.Name));
				else code.AppendLine(string.Format("\t\tpublic abstract void {0}Result({1} Value);", o.Name, DataTypeGenerator.GenerateType(o.ReturnType)));
			}
			return code.ToString();
		}

		public static string GenerateClientMethodClientAsync45(WebApiMethod o)
		{
			if (o.IsHidden) return "";
			var conf = o.Owner.RequestConfiguration;
			var code = new StringBuilder();

			//Generate documentation
			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (var mp in o.RouteParameters.OfType<WebApiMethodParameter>().Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				foreach (var mp in o.QueryParameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
			}

			//Construct method declaration
			if (o.ReturnResponseData)
			{
				if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendFormat("\t\tpublic async virtual System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> {0}Async(", o.Name);
				else code.AppendFormat("\t\tpublic async virtual System.Threading.Tasks.Task<Tuple<{0}, System.Net.Http.HttpResponseMessage>> {1}Async(", DataTypeGenerator.GenerateType(o.ReturnType), o.Name);
			}
			else
			{
				if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendFormat("\t\tpublic async virtual System.Threading.Tasks.Task {0}Async(", o.Name);
				else code.AppendFormat("\t\tpublic async virtual System.Threading.Tasks.Task<{0}> {1}Async(", DataTypeGenerator.GenerateType(o.ReturnType), o.Name);
			}
			foreach (var op in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => string.IsNullOrEmpty(a.DefaultValue)))
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			foreach (var op in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => !string.IsNullOrEmpty(a.DefaultValue)))
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			foreach (var op in o.QueryParameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (!o.HasContent && (o.RouteParameters.OfType<WebApiMethodParameter>().Any() || o.QueryParameters.Any())) code.Remove(code.Length - 2, 2);
			if (o.HasContent)
				code.AppendFormat("{0} {1}", DataTypeGenerator.GenerateType(o.ContentType), o.ContentParameterName);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			GenerateMethodPreamble(code, o.ClientPreambleCode, 3);

			//Construct the URI
			code.AppendLine("\t\t\tvar uri = new StringBuilder(_baseUri, 2048);");
			code.Append("\t\t\turi.Append(\"");
			foreach (var op in o.RouteParameters)
			{
				if (op.GetType() == typeof(WebApiRouteParameter))
					code.Append(string.Format("/{0}", op.RouteName.ToLowerInvariant()));
			}
			code.Append(string.Format("/{0}", o.Name.ToLowerInvariant()));
			code.AppendLine("\");");
			foreach (var op in o.RouteParameters)
			{

				if (op.GetType() == typeof(WebApiMethodParameter))
					code.AppendLine(string.Format("\t\t\turi.AppendFormat(\"/{{0}}\", {0});", op.Name));
			}
			if (o.QueryParameters.Any())
				code.AppendLine("\t\t\turi.Append(\"?\");");
			foreach (var op in o.QueryParameters)
				code.AppendLine(string.Format("\t\t\tif ({1} != null) uri.AppendFormat(\"&{0}={{0}}\", {1});", op.RouteName.ToLowerInvariant(), op.Name));
			if (o.QueryParameters.Any())
				code.AppendLine("\t\t\turi.Replace(\"?&\", \"?\");");

			//Create the HttpRequestMessage
			code.AppendLine(string.Format("\t\t\tvar rm = new HttpRequestMessage(HttpMethod.{0}, new Uri(uri.ToString(), UriKind.RelativeOrAbsolute));", o.Method));
			if (conf.UseHTTP10)
				code.AppendLine("\t\t\trm.Version = new Version(1, 0););");
			//code.AppendLine("\t\t\trm.Headers.Clear();");
			//code.AppendLine("\t\t\tforeach (var x in _requestHeaders) rm.Headers.Add(x.Key, x.Value);");
			//code.AppendLine("\t\t\trm.Headers.Authorization = _requestHeaders.Authorization;");
			//code.AppendLine("\t\t\trm.Headers.CacheControl = _requestHeaders.CacheControl;");
			//code.AppendLine("\t\t\trm.Headers.Date = _requestHeaders.Date;");
			//code.AppendLine("\t\t\trm.Headers.From = _requestHeaders.From;");
			//code.AppendLine("\t\t\trm.Headers.Host = _requestHeaders.Host;");
			//code.AppendLine("\t\t\trm.Headers.IfModifiedSince = _requestHeaders.IfModifiedSince;");
			//code.AppendLine("\t\t\trm.Headers.IfRange = _requestHeaders.IfRange;");
			//code.AppendLine("\t\t\trm.Headers.IfUnmodifiedSince = _requestHeaders.IfUnmodifiedSince;");
			//code.AppendLine("\t\t\trm.Headers.MaxForwards = _requestHeaders.MaxForwards;");
			//code.AppendLine("\t\t\trm.Headers.ProxyAuthorization = _requestHeaders.ProxyAuthorization;");
			//code.AppendLine("\t\t\trm.Headers.Range = _requestHeaders.Range;");
			code.AppendLine("\t\t\trm.Headers.Accept.Clear();");
			code.AppendLine(string.Format("\t\t\trm.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(\"application/{0}\"));", conf.ResponseFormat == RestSerialization.Json ? "json" : conf.ResponseFormat == RestSerialization.Bson ? "bson" : "xml"));

			//Serialize any parameters
			if (o.HasContent)
			{
				var pt = o.ContentType;
				if (pt.TypeMode == DataTypeMode.Primitive && pt.Primitive == PrimitiveTypes.String)
					code.AppendLine(string.Format("\t\t\trm.Content = new System.Net.Http.StringContent({0});", o.ContentParameterName));
				else
					code.AppendLine(string.Format("\t\t\trm.Content = new System.Net.Http.ObjectContent<{0}>({1}, new {2}());", DataTypeGenerator.GenerateType(pt), o.ContentParameterName, conf.RequestFormat == RestSerialization.Json ? "JsonMediaTypeFormatter" : conf.RequestFormat == RestSerialization.Bson ? "BsonMediaTypeFormatter" : "XmlMediaTypeFormatter"));
				//code.AppendLine("\t\t\tforeach (var x in _contentHeaders) rm.Content.Headers.Add(x.Key, x.Value);");
			}

			code.AppendLine("\t\t\tvar rr = await _HttpClient.SendAsync(rm, System.Net.Http.HttpCompletionOption.ResponseHeadersRead);");
			if (o.EnsureSuccessStatusCode) code.AppendLine("\t\t\trr.EnsureSuccessStatusCode();");
			if (o.ReturnType.Primitive != PrimitiveTypes.Void || !o.DeserializeContent)
			{
				if (o.ReturnResponseData)
				{
					code.AppendLine(string.Format("\t\t\tvar rs = await rr.Content.ReadAsAsync<{0}>(new MediaTypeFormatter[] {{ new {1}() }});", DataTypeGenerator.GenerateType(o.ReturnType), conf.ResponseFormat == RestSerialization.Json ? "JsonMediaTypeFormatter" : conf.ResponseFormat == RestSerialization.Bson ? "BsonMediaTypeFormatter" : "XmlMediaTypeFormatter"));
					code.AppendLine(string.Format("\t\t\treturn new Tuple<{0}, System.Net.Http.HttpResponseMessage>(rs, rr);", DataTypeGenerator.GenerateType(o.ReturnType)));
				}
				else
					code.AppendLine(string.Format("\t\t\treturn await rr.Content.ReadAsAsync<{0}>(new MediaTypeFormatter[] {{ new {1}() }});", DataTypeGenerator.GenerateType(o.ReturnType), conf.ResponseFormat == RestSerialization.Json ? "JsonMediaTypeFormatter" : conf.ResponseFormat == RestSerialization.Bson ? "BsonMediaTypeFormatter" : "XmlMediaTypeFormatter"));
			}
			code.AppendLine("\t\t}");

			return code.ToString();
		}

		#endregion

		#region - Method Preamble Code -

		public static void GenerateMethodPreamble(StringBuilder code, string preamble, short tabDepth)
		{
			if (string.IsNullOrWhiteSpace(preamble)) return;
			string tabs = "";
			for (short i = 0; i < tabDepth; i++)
				tabs += "\t";

			var ll = new List<string>(preamble.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
			foreach (var l in ll)
				code.AppendLine(string.Format("{0}{1}", tabs, l));
		}

		#endregion

		#region - Method Parameters -

		public static string GenerateMethodParameterServerCode(WebApiMethodParameter o)
		{
			return o.IsHidden ? "" : string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(o.Type), o.Name, string.IsNullOrEmpty(o.DefaultValue) ? "" : $" = {o.DefaultValue}");
		}

		public static string GenerateMethodParameterClientCode(WebApiMethodParameter o)
		{
			if (o.IsHidden) return "";

			if (o.Type.TypeMode == DataTypeMode.Class || o.Type.TypeMode == DataTypeMode.Struct || o.Type.TypeMode == DataTypeMode.Enum)
			{
				var ptype = o.Type as WebApiData;
				return string.Format("{0} {1}{2}", ptype != null && ptype.HasClientType ? DataTypeGenerator.GenerateType(ptype.ClientType) : DataTypeGenerator.GenerateType(o.Type), o.Name, string.IsNullOrWhiteSpace(o.DefaultValue) ? "" : string.Format(" = {0}", o.DefaultValue));
			}

			return string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(o.Type), o.Name, string.IsNullOrWhiteSpace(o.DefaultValue) ? "" : string.Format(" = {0}", o.DefaultValue));
		}

		#endregion

		#region - Update Service -

		public static void GenerateServerUpdateService(StringBuilder code, WebApiProject o)
		{
			if (!o.ClientGenerationTargets.OfType<WebApiData>().Any(a => a.Elements.Any(b => b.EnableUpdates)))
				return;

			code.AppendLine(string.Format("namespace {0}", o.Namespace.FullName));
			code.AppendLine("{");

			if (o.UsingInsideNamespace)
			{
				if (o.GenerateRegions)
				{
					code.AppendLine("\t#region Using");
					code.AppendLine();
				}
				// Generate using namespaces
				foreach (ProjectUsingNamespace pun in o.UsingNamespaces)
				{
					if (pun.Client && pun.NET)
						code.AppendLine(string.Format("\tusing {0};", pun.Namespace));
				}
				code.AppendLine();
				if (o.GenerateRegions)
				{
					code.AppendLine("\t#endregion");
					code.AppendLine();
				}
			}

			if (!o.GenerateRegions)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Update Service");
				code.AppendLine("\t**************************************************************************/");
			}

			code.AppendLine("\t[RoutePrefix(\"__dus__\")]");
			code.AppendLine("\tpublic sealed class DataUpdateServiceController : ApiController");
			code.AppendLine("\t{");
			foreach (var d in o.ClientGenerationTargets.OfType<WebApiData>().Where(a => a.Elements.Any(b => b.EnableUpdates)))
			{
				foreach (var e in d.Elements.Where(a => a.EnableUpdates && a.DataType.TypeMode == DataTypeMode.Primitive && a.DataType.Primitive != PrimitiveTypes.Object))
				{
					code.AppendLine(string.Format("\t\t[System.Web.Http.Route(\"{0}/{1}", d.Name.ToLowerInvariant(), e.DataName.ToLowerInvariant()));
					foreach (var l in d.Elements.Where(a => a.IsUpdateLookup))
						code.AppendFormat("/{0}", l.DataName);
					code.AppendLine("\")]");
					code.AppendLine("\t\t[System.Web.Http.AcceptVerbs(\"PUT\")]");
					code.AppendFormat("\t\tpublic async Task Update{0}{1}(", d.Name, e.DataName);
					foreach (var l in d.Elements.Where(a => a.IsUpdateLookup))
						code.AppendFormat("{0} {1},  ", l.DataType, l.DataName);
					code.Remove(code.Length - 2, 2);
					code.AppendLine(")");
					code.AppendLine("\t\t{");
					if (e.DataType.Primitive == PrimitiveTypes.ByteArray)
					{
						code.AppendLine("\t\t\tvar value = await Request.Content.ReadAsByteArrayAsync();");
					}
					else
					{
						code.AppendLine(string.Format("\t\t\tvar {0} = await Request.Content.ReadAsStringAsync());", e.DataType.Primitive == PrimitiveTypes.String ? "value" : "payload"));
						switch (e.DataType.Primitive)
						{
						case PrimitiveTypes.Bool:
							code.AppendLine("\t\t\tvar value = Convert.ToBoolean(payload);");
							break;
						case PrimitiveTypes.Char:
							code.AppendLine("\t\t\tvar value = Convert.ToChar(payload);");
							break;
						case PrimitiveTypes.Byte:
							code.AppendLine("\t\t\tvar value = Convert.FromBase64String(payload)[0];");
							break;
						case PrimitiveTypes.SByte:
							code.AppendLine("\t\t\tvar value = Convert.ToSByte(Convert.FromBase64String(payload)[0]);");
							break;
						case PrimitiveTypes.Short:
							code.AppendLine("\t\t\tvar value = Convert.ToInt16(payload);");
							break;
						case PrimitiveTypes.Int:
							code.AppendLine("\t\t\tvar value = Convert.ToInt32(payload);");
							break;
						case PrimitiveTypes.Long:
							code.AppendLine("\t\t\tvar value = Convert.ToInt64(payload);");
							break;
						case PrimitiveTypes.UShort:
							code.AppendLine("\t\t\tvar value = Convert.ToUInt16(payload);");
							break;
						case PrimitiveTypes.UInt:
							code.AppendLine("\t\t\tvar value = Convert.ToUInt32(payload);");
							break;
						case PrimitiveTypes.ULong:
							code.AppendLine("\t\t\tvar value = Convert.ToUInt64(payload);");
							break;
						case PrimitiveTypes.Float:
							code.AppendLine("\t\t\tvar value = Convert.ToSingle(payload);");
							break;
						case PrimitiveTypes.Double:
							code.AppendLine("\t\t\tvar value = Convert.ToDouble(payload);");
							break;
						case PrimitiveTypes.Decimal:
							code.AppendLine("\t\t\tvar value = Convert.ToDecimal(payload);");
							break;
						case PrimitiveTypes.DateTime:
							code.AppendLine("\t\t\tvar value = DateTime.Parse(payload, \"O\", CultureInfo.InvariantCulture);");
							break;
						case PrimitiveTypes.DateTimeOffset:
							code.AppendLine("\t\t\tvar value = DateTimeOffset.Parse(payload, \"O\", CultureInfo.InvariantCulture);");
							break;
						case PrimitiveTypes.TimeSpan:
							code.AppendLine("\t\t\tvar value = new TimeSpan(Convert.ToInt64(payload));");
							break;
						case PrimitiveTypes.GUID:
							code.AppendLine("\t\t\tvar value = new Guid(payload);");
							break;
						case PrimitiveTypes.URI:
							code.AppendLine("\t\t\tvar value = new Uri(payload);");
							break;
						case PrimitiveTypes.Version:
							code.AppendLine("\t\t\tvar value = new Version(payload);");
							break;
						}
					}

					if (d.HasEntity)
					{
						code.AppendLine(string.Format("\t\t\tusing (var db = new {0}())", o.EnitityDatabaseType));
						code.AppendLine("\t\t\t{");
						code.AppendFormat("\t\t\t\tvar item = await db.{0}.Where(a => ", d.EntityContext);
						foreach (var l in d.Elements.Where(a => a.IsUpdateLookup))
							code.AppendFormat("a.{0} == {1} && ", l.EntityName, l.DataName);
						code.Remove(code.Length - 4, 4);
						code.AppendLine(").FirstOrDefaultAsync();");
						code.AppendLine("\t\t\t\tif (item == null) throw new HttpResponseException(HttpStatusCode.NotFound);");
						code.AppendLine(string.Format("\t\t\t\titem.{0} = Data.{1};", e.EntityName, e.DataName));
						code.AppendLine("\t\t\t\tawait db.SaveChangesAsync();");
						code.AppendLine("\t\t\t}");
					}

					code.AppendLine("\t\t}");
					code.AppendLine();
				}
			}
			code.AppendLine("\t}");
			code.AppendLine("}");
		}

		public static void GenerateClientUpdateService(StringBuilder code, WebApiProject o)
		{
			if (!o.ClientGenerationTargets.OfType<WebApiData>().Any(a => a.Elements.Any(b => b.EnableUpdates)))
				return;

			code.AppendLine(string.Format("namespace {0}", o.Namespace.FullName));
			code.AppendLine("{");

			if (o.UsingInsideNamespace)
			{
				if (o.GenerateRegions)
				{
					code.AppendLine("\t#region Using");
					code.AppendLine();
				}
				// Generate using namespaces
				foreach (ProjectUsingNamespace pun in o.UsingNamespaces)
				{
					if (pun.Client && pun.NET)
						code.AppendLine(string.Format("\tusing {0};", pun.Namespace));
				}
				code.AppendLine();
				if (o.GenerateRegions)
				{
					code.AppendLine("\t#endregion");
					code.AppendLine();
				}
			}

			if (!o.GenerateRegions)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Update Service");
				code.AppendLine("\t**************************************************************************/");
			}

			code.AppendLine("\tpublic static partial class DataUpdateService");
			code.AppendLine("\t{");
			code.AppendLine(string.Format("\t\tprivate readonly string _baseUri = \"{0}\";", o.Namespace.URI));
			code.AppendLine("\t\tpublic string BaseUri { get { return _baseUri; } }");
			code.AppendLine("\t\tprivate readonly System.Net.Http.HttpClient _HttpClient= new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false, AutomaticDecompression = DecompressionMethods.None});");
			code.AppendLine("\t\tprotected System.Net.Http.HttpClient HttpClient {{ get {{ return _HttpClient; }} }}");
			code.AppendLine();
			code.AppendLine("\t\t\tpartial void SetCommonUpdateHeaders(System.Net.Http.Headers.HttpRequestHeaders headers);");
			code.AppendLine();

			foreach (var d in o.ClientGenerationTargets.OfType<WebApiData>().Where(a => a.Elements.Any(b => b.EnableUpdates)))
			{
				foreach (var e in d.Elements.Where(a => a.EnableUpdates && a.DataType.TypeMode == DataTypeMode.Primitive && a.DataType.Primitive != PrimitiveTypes.Object))
				{
					code.AppendFormat("\t\tpublic async Task Update{0}{1}(", d.Name, e.DataName);
					foreach (var l in d.Elements.Where(a => a.IsUpdateLookup))
						code.AppendFormat("{0} {1}, ", l.DataType, l.DataName);
					code.AppendLine(string.Format("{0} {1})", e.DataType, e.DataName));
					code.AppendLine("\t\t{");
					code.AppendLine("\t\t\tvar uri = new StringBuilder(_baseUri, 256);");
					code.AppendLine(string.Format("\t\t\turi.Append(\"__dus__/{0}/{1}\");", d.Name.ToLowerInvariant(), e.DataName.ToLowerInvariant()));
					foreach (var l in d.Elements.Where(a => a.IsUpdateLookup))
						code.AppendLine(string.Format("\t\t\turi.AppendFormat(\"/{{0}}\", {0});", l.DataName));

					code.AppendLine("\t\t\tvar rm = new HttpRequestMessage(HttpMethod.Put, new Uri(uri.ToString(), UriKind.RelativeOrAbsolute));");
					code.AppendLine("\t\t\tSetCommonUpdateHeaders(rm.Headers);");
					code.AppendLine(string.Format("\t\t\tSetUpdate{0}{1}Headers(rm.Headers);", d.Name, e.DataName));

					if (e.DataType.Primitive == PrimitiveTypes.ByteArray)
					{
						code.AppendLine(string.Format("\t\t\trm.Content = new System.Net.Http.ByteArrayContent({0});", e.DataName));
					}
					else
					{
						switch (e.DataType.Primitive)
						{
						case PrimitiveTypes.Byte:
							code.AppendLine(string.Format("\t\t\trm.Content = new System.Net.Http.StringContent(Convert.ToBase64String(new byte[] {{ {0} }}));", e.DataName));
							break;
						case PrimitiveTypes.SByte:
							code.AppendLine(string.Format("\t\t\trm.Content = new System.Net.Http.StringContent(Convert.ToBase64String(new byte[] {{ Convert.ToSByte({0}) }}));", e.DataName));
							break;
						case PrimitiveTypes.DateTime:
						case PrimitiveTypes.DateTimeOffset:
							code.AppendLine(string.Format("\t\t\trm.Content = new System.Net.Http.StringContent({0}.ToString(\"O\", CultureInfo.InvariantCulture));", e.DataName));
							break;
						case PrimitiveTypes.TimeSpan:
							code.AppendLine(string.Format("\t\t\trm.Content = new System.Net.Http.StringContent({0}.Ticks);", e.DataName));
							break;
						default:
							code.AppendLine(string.Format("\t\t\trm.Content = new System.Net.Http.StringContent({0});", e.DataName));
							break;
						}
					}

					code.AppendLine("\t\t}");
					code.AppendLine(string.Format("\t\t\tpartial void SetUpdate{0}{1}Headers(System.Net.Http.Headers.HttpRequestHeaders headers);", d.Name, e.DataName));
					code.AppendLine();
				}
			}
			code.AppendLine("\t}");
			code.AppendLine("}");
		}

		#endregion
	}
}