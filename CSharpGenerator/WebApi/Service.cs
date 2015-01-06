using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
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

namespace NETPath.Generators.CS
{
	internal static class WebApiServiceGenerator
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
				else if (m.ReturnType.TypeMode == DataTypeMode.Primitive && (m.ReturnType.Primitive == PrimitiveTypes.Void || m.ReturnType.Primitive == PrimitiveTypes.None))
					AddMessage(new CompileMessage("GS2012", "The method return type '" + m.ReturnType + "' in the '" + o.Name + "' service is not a valid Rest return type. Please specify a valid Rest return type.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
				if (m.ReturnType.TypeMode == DataTypeMode.Namespace || m.ReturnType.TypeMode == DataTypeMode.Interface)
					AddMessage(new CompileMessage("GS2013", "The method return type '" + m.ReturnType + "' in the '" + o.Name + "' service is not a valid return type. Please specify a valid return type.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
				if (m.RequestConfiguration == null)
					AddMessage(new CompileMessage("GS2017", "The Request Configuration type in method '" + m.Name + "' in the '" + o.Name + "' service is not set. A Request Configuration MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType()));

				foreach (var mp in m.RouteParameters)
				{
					if (string.IsNullOrEmpty(mp.Name))
						AddMessage(new CompileMessage("GS2008", "The method parameter '" + m.Name + "' in the '" + o.Name + "' service has a blank parameter name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
					if (string.IsNullOrEmpty(mp.Name))
						AddMessage(new CompileMessage("GS20018", "The method parameter '" + m.Name + "' in the '" + o.Name + "' service has a blank route name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
					//if (mp.IsRestInvalid)
					//	AddMessage(new CompileMessage("GS2009", "The method Rest parameter '" + m.Name + "' in the '" + m.Name + "' method is not a valid Rest parameter. Please specify a valid Rest parameter.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
					if (mp.Name == "__callback")
						AddMessage(new CompileMessage("GS2016", "The name of the method parameter '" + mp.Name + "' in the '" + m.Name + "' method is invalid. Please rename it.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
				}

				foreach (var mp in m.QueryParameters)
				{
					if (string.IsNullOrEmpty(mp.Name))
						AddMessage(new CompileMessage("GS2008", "The method parameter '" + m.Name + "' in the '" + o.Name + "' service has a blank parameter name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
					if (string.IsNullOrEmpty(mp.Name))
						AddMessage(new CompileMessage("GS20018", "The method parameter '" + m.Name + "' in the '" + o.Name + "' service has a blank route name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
					//if (mp.IsRestInvalid)
					//	AddMessage(new CompileMessage("GS2009", "The method Rest parameter '" + m.Name + "' in the '" + m.Name + "' method is not a valid Rest parameter. Please specify a valid Rest parameter.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
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
			code.AppendLine(string.Format("\t{0} abstract class {1}Controller<T> : ApiController, where T : {1}Controller<T>", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
			code.AppendLine("\t{");

			code.AppendLine();
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Constructors");
				code.AppendLine();
			}

			var conf = o.RequestConfigurations.FirstOrDefault();

			code.AppendLine(string.Format("\t\tpublic {0}Controller() : base()", o.Name));
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
			code.AppendLine("\t\tprivate readonly string _baseUri;");
			code.AppendLine("\t\tprivate readonly CookieContainer _cookies;");
			code.AppendLine("\t\tprivate readonly CredentialCache _credentialCache;");
			code.AppendLine("\t\tprivate readonly NetworkCredential _credentials;");
			code.AppendLine("\t\tprivate readonly IWebProxy _proxy;");
			code.AppendLine("\t\tprivate readonly System.Net.Http.Headers.HttpRequestHeaders _requestHeaders = new System.Net.Http.HttpRequestMessage().Headers;");
			code.AppendLine("\t\tprivate readonly System.Net.Http.Headers.HttpContentHeaders _contentHeaders = new System.Net.Http.StringContent(\"\").Headers;");

			code.AppendLine("\t\tpublic string BaseUri { get { return _baseUri; } }");
			code.AppendLine("\t\tpublic CookieContainer Cookies { get { return _cookies; } }");
			code.AppendLine("\t\tpublic CredentialCache CredentialCache { get { return _credentialCache; } }");
			code.AppendLine("\t\tpublic NetworkCredential Credentials { get { return _credentials; } ");
			code.AppendLine("\t\tpublic IWebProxy Proxy { get { return _proxy; } }");
			code.AppendLine("\t\tpublic HttpRequestHeaders DefaultRequestHeaders { get { return _requestHeaders; } }");
            code.AppendLine("\t\tpublic HttpContentHeaders DefaultContentHeaders { get { return _contentHeaders; } }");

			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Request Configurations");
				code.AppendLine();
			}
			foreach (WebApiHttpConfiguration c in o.RequestConfigurations.Where(a => a.GetType() == typeof(WebApiHttpConfiguration)).Select(t => t).Where(c => o.ServiceOperations.Any(a => Equals(a.RequestConfiguration, c))))
			{
				code.AppendLine(string.Format("\t\tprivate System.Net.Http.HttpClient _{0}Client;", RegExs.ReplaceSpaces.Replace(c.Name, "")));
				code.AppendLine(string.Format("\t\tprotected System.Net.Http.HttpClient {0}Client {{ get {{ return _{0}Client; }} }}", RegExs.ReplaceSpaces.Replace(c.Name, "")));
			}
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
			code.AppendLine(string.Format("\t\t{1} {0}{2}(string BaseURI = \"{3}\", CookieContainer cookies = null, NetworkCredential credentials = null, CredentialCache credentialCache = null, IWebProxy proxy = null)", o.Name, o.Abstract ? "protected" : "public", o.Abstract ? "Base" : "", (o.Parent.Owner as WebApiProject)?.Namespace.URI));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\t_baseUri = BaseURI;");
			code.AppendLine("\t\t\tif (_baseUri.EndsWith(\"/\")) _baseUri = _baseUri.Remove(_baseUri.Length - 1, 1);");
			code.AppendLine("\t\t\t_cookies = cookies ?? new CookieContainer();");
			code.AppendLine("\t\t\t_credentials = credentials;");
			code.AppendLine("\t\t\t_credentialCache = credentialCache;");
			code.AppendLine("\t\t\t_proxy = proxy;");
			code.AppendLine("\t\t\tInitialize();");
			code.AppendLine("\t\t}");
			code.AppendLine("\t\tprivate void Initialize()");
			code.AppendLine("\t\t{");
			foreach (WebApiHttpConfiguration c in o.RequestConfigurations.Where(a => a.GetType() == typeof(WebApiHttpConfiguration)).Select(t => t).Where(c => o.ServiceOperations.Any(a => Equals(a.RequestConfiguration, c))))
				code.AppendLine(string.Format("\t\t\t_{0}Client = new System.Net.Http.HttpClient(new System.Net.Http.HttpClientHandler() {{ AllowAutoRedirect = {1}, AutomaticDecompression = System.Net.DecompressionMethods.{2}, ClientCertificateOptions = System.Net.Http.ClientCertificateOption.{3}, CookieContainer = {4}, Credentials = (this.Credentials == null) ? (ICredentials)this.CredentialCache : (ICredentials)this.Credentials, MaxAutomaticRedirections = {5}, MaxRequestContentBufferSize = {6}, PreAuthenticate = {7}, Proxy = this.Proxy, UseCookies = {8}, UseDefaultCredentials = {9}, UseProxy = {10} }});",
					RegExs.ReplaceSpaces.Replace(c.Name, ""), c.AllowAutoRedirect ? bool.TrueString.ToLower() : bool.FalseString.ToLower(), c.AutomaticDecompression, c.ClientCertificateOptions,
					c.CookieContainerMode == CookieContainerMode.None ? "null" : c.CookieContainerMode == CookieContainerMode.Instance ? "new CookieContainer()" : "Cookies",
					c.MaxAutomaticRedirections, c.MaxRequestContentBufferSize, c.PreAuthenticate ? bool.TrueString.ToLower() : bool.FalseString.ToLower(),
					(c.CookieContainerMode == CookieContainerMode.None || c.CookieContainerMode == CookieContainerMode.Custom) ? bool.FalseString.ToLower() : bool.TrueString.ToLower(),
					c.UseDefaultCredentials ? bool.TrueString.ToLower() : bool.FalseString.ToLower(), c.UseProxy ? bool.TrueString.ToLower() : bool.FalseString.ToLower()));
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
			foreach (WebApiMethod m in o.ServiceOperations.Where(a => a.RequestConfiguration.GetType() == typeof(WebApiHttpConfiguration)))
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

			foreach (var pp in o.RouteParameters)
				uriBuilder.AppendFormat("/{{{0}}}", pp.RouteName);

			if (!o.QueryParameters.Any()) return uriBuilder.ToString();

			uriBuilder.Append("?");

			foreach (var pq in o.QueryParameters)
				uriBuilder.AppendFormat("&{0}={{{0}}}", pq.RouteName);

			uriBuilder.Replace("?&", "?");

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
			code.AppendFormat("\t\tpublic abstract {0} {1}(", o.ServerAsync ? o.ReturnType.IsVoid ? "Task" : string.Format("Task<{0}>", DataTypeGenerator.GenerateType(o.ReturnType)) : DataTypeGenerator.GenerateType(o.ReturnType), o.Name);
			foreach (var op in o.RouteParameters.OfType<WebApiMethodParameter>())
				code.AppendFormat("{0}, ", GenerateMethodParameterServerCode(op));
			foreach (var op in o.QueryParameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterServerCode(op));
			if (o.RouteParameters.OfType<WebApiMethodParameter>().Any() || o.QueryParameters.Any()) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");
			return code.ToString();
		}

		#endregion

		#region - Client HttpClient Methods -

		public static string GenerateClientMethodClient45(WebApiMethod o)
		{
			if (o.IsHidden) return "";
			var conf = o.RequestConfiguration;
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
			foreach (var op in o.RouteParameters.OfType<WebApiMethodParameter>())
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			foreach (var op in o.QueryParameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.RouteParameters.OfType<WebApiMethodParameter>().Any() || o.QueryParameters.Any()) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			GenerateMethodPreamble(code, o.ClientPreambleCode, 3);

			//Construct the URI
			code.AppendLine("\t\t\tvar uri = StringBuilder(_baseUri, 2048);");
			foreach (var op in o.RouteParameters)
			{
				if (op.GetType() == typeof (WebApiRouteParameter))
					code.AppendLine(string.Format("\t\t\turi.Append(\"/{0}\");", op.RouteName));
				if (op.GetType() == typeof (WebApiMethodParameter))
					code.AppendLine(string.Format("\t\t\turi.Append(\"/{{0}}\", {0});", op.Name));
			}
			if (o.QueryParameters.Any(a => !a.Serialize))
				code.AppendLine("\t\t\turi.Append(\"?\"");
			foreach (var op in o.QueryParameters.Where(a => !a.Serialize))
				code.AppendLine(string.Format(!op.Nullable ? "\t\t\turi.Append(\"&{0}={{0}}\", {1});" : "\t\t\tif ({1} != null) uri.Append(\"&{0}={{0}}\", {1});", op.RouteName, op.Name));
			code.AppendLine("\t\t\turi.Replace(\"?&\", \"?\"");

			//Create the HttpRequestMessage
			code.AppendLine(string.Format("\t\t\tvar rm = new HttpRequestMessage(HttpMethod.{0}, new Uri(uri.ToString(), UriKind.RelativeOrAbsolute));", o.Method));
			if (conf.UseHTTP10)
				code.AppendLine("\t\t\trm.Version = new Version(1, 0););");
			code.AppendLine("\t\t\trm.Headers.Clear();");
            code.AppendLine("\t\t\tforeach (var x in _requestHeaders) rm.Headers.Add(x.Key, x.Value);");
			code.AppendLine("\t\t\trm.Headers.Authorization = _requestHeaders.Authorization;");
			code.AppendLine("\t\t\trm.Headers.CacheControl = _requestHeaders.CacheControl;");
			code.AppendLine("\t\t\trm.Headers.Date = _requestHeaders.Date;");
			code.AppendLine("\t\t\trm.Headers.From = _requestHeaders.From;");
			code.AppendLine("\t\t\trm.Headers.Host = _requestHeaders.Host;");
			code.AppendLine("\t\t\trm.Headers.IfModifiedSince = _requestHeaders.IfModifiedSince;");
			code.AppendLine("\t\t\trm.Headers.IfRange = _requestHeaders.IfRange;");
			code.AppendLine("\t\t\trm.Headers.IfUnmodifiedSince = _requestHeaders.IfUnmodifiedSince;");
			code.AppendLine("\t\t\trm.Headers.MaxForwards = _requestHeaders.MaxForwards;");
			code.AppendLine("\t\t\trm.Headers.ProxyAuthorization = _requestHeaders.ProxyAuthorization;");
			code.AppendLine("\t\t\trm.Headers.Range = _requestHeaders.Range;");
			code.AppendLine("\t\t\trm.Headers.Accept.Clear()");
			code.AppendLine(string.Format("\t\t\trm.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(\"application/{0}\"));", conf.ResponseFormat == RestSerialization.Json ? "json" : conf.ResponseFormat == RestSerialization.Bson ? "bson" : "xml"));

			//Serialize any parameters
			if (o.QueryParameters.Any(a => a.Serialize))
			{
				var p = o.QueryParameters.First(a => a.Serialize);
				var pt = p.Type;
				code.AppendLine(string.Format("\t\t\trm.Content = new System.Net.Http.ObjectContent<{0}>({1}, new {2}());", DataTypeGenerator.GenerateType(pt), p.Name, conf.RequestFormat == RestSerialization.Json ? "JsonMediaTypeFormatter" : conf.RequestFormat == RestSerialization.Bson ? "BsonMediaTypeFormatter" : "XmlMediaTypeFormatter"));
				code.AppendLine(string.Format("\t\t\tforeach (var x in _contentHeaders) rm.Content.Headers.Add(x.Key, x.Value);"));
			}

			code.AppendLine(string.Format("\t\t\tvar rr = await {0}Client.SendAsync(rm, System.Net.Http.HttpCompletionOption.ResponseHeadersRead);", RegExs.ReplaceSpaces.Replace(o.RequestConfiguration.Name, "")));
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
			var conf = o.RequestConfiguration;
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
			foreach (var op in o.RouteParameters.OfType<WebApiMethodParameter>())
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			foreach (var op in o.QueryParameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.RouteParameters.OfType<WebApiMethodParameter>().Any() || o.QueryParameters.Any()) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			GenerateMethodPreamble(code, o.ClientPreambleCode, 3);

			//Construct the URI
			code.AppendLine("\t\t\tvar uri = StringBuilder(_baseUri, 2048);");
			foreach (var op in o.RouteParameters)
			{
				if (op.GetType() == typeof (WebApiRouteParameter))
					code.AppendLine(string.Format("\t\t\turi.Append(\"/{0}\");", op.RouteName));
				if (op.GetType() == typeof (WebApiMethodParameter))
					code.AppendLine(string.Format("\t\t\turi.Append(\"/{{0}}\", {0});", op.Name));
			}
			if (o.QueryParameters.Any(a => !a.Serialize))
				code.AppendLine("\t\t\turi.Append(\"?\"");
			foreach (var op in o.QueryParameters.Where(a => !a.Serialize))
				code.AppendLine(string.Format(!op.Nullable ? "\t\t\turi.Append(\"&{0}={{0}}\", {1});" : "\t\t\tif ({1} != null) uri.Append(\"&{0}={{0}}\", {1});", op.RouteName, op.Name));
			code.AppendLine("\t\t\turi.Replace(\"?&\", \"?\"");

			//Create the HttpRequestMessage
			code.AppendLine(string.Format("\t\t\tvar rm = new HttpRequestMessage(HttpMethod.{0}, new Uri(uri.ToString(), UriKind.RelativeOrAbsolute));", o.Method));
			if (conf.UseHTTP10)
				code.AppendLine("\t\t\trm.Version = new Version(1, 0););");
			code.AppendLine("\t\t\trm.Headers.Clear();");
			code.AppendLine("\t\t\tforeach (var x in _requestHeaders) rm.Headers.Add(x.Key, x.Value);");
			code.AppendLine("\t\t\trm.Headers.Authorization = _requestHeaders.Authorization;");
			code.AppendLine("\t\t\trm.Headers.CacheControl = _requestHeaders.CacheControl;");
			code.AppendLine("\t\t\trm.Headers.Date = _requestHeaders.Date;");
			code.AppendLine("\t\t\trm.Headers.From = _requestHeaders.From;");
			code.AppendLine("\t\t\trm.Headers.Host = _requestHeaders.Host;");
			code.AppendLine("\t\t\trm.Headers.IfModifiedSince = _requestHeaders.IfModifiedSince;");
			code.AppendLine("\t\t\trm.Headers.IfRange = _requestHeaders.IfRange;");
			code.AppendLine("\t\t\trm.Headers.IfUnmodifiedSince = _requestHeaders.IfUnmodifiedSince;");
			code.AppendLine("\t\t\trm.Headers.MaxForwards = _requestHeaders.MaxForwards;");
			code.AppendLine("\t\t\trm.Headers.ProxyAuthorization = _requestHeaders.ProxyAuthorization;");
			code.AppendLine("\t\t\trm.Headers.Range = _requestHeaders.Range;");
			code.AppendLine("\t\t\trm.Headers.Accept.Clear()");
			code.AppendLine(string.Format("\t\t\trm.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(\"application/{0}\"));", conf.ResponseFormat == RestSerialization.Json ? "json" : conf.ResponseFormat == RestSerialization.Bson ? "bson" : "xml"));

			//Serialize any parameters
			if (o.QueryParameters.Any(a => a.Serialize))
			{
				var p = o.QueryParameters.First(a => a.Serialize);
				var pt = p.Type;
				code.AppendLine(string.Format("\t\t\trm.Content = new System.Net.Http.ObjectContent<{0}>({1}, new {2}());", DataTypeGenerator.GenerateType(pt), p.Name, conf.RequestFormat == RestSerialization.Json ? "JsonMediaTypeFormatter" : conf.RequestFormat == RestSerialization.Bson ? "BsonMediaTypeFormatter" : "XmlMediaTypeFormatter"));
				code.AppendLine(string.Format("\t\t\tforeach (var x in _contentHeaders) rm.Content.Headers.Add(x.Key, x.Value);"));
			}

			code.AppendLine(string.Format("\t\t\tvar rr = await {0}Client.SendAsync(rm, System.Net.Http.HttpCompletionOption.ResponseHeadersRead);", RegExs.ReplaceSpaces.Replace(o.RequestConfiguration.Name, "")));
			if (o.EnsureSuccessStatusCode) code.AppendLine("\t\t\trr.EnsureSuccessStatusCode();");
			if ((o.ReturnType.TypeMode != DataTypeMode.Primitive && o.ReturnType.Primitive != PrimitiveTypes.Void) || !o.DeserializeContent)
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
			return o.IsHidden ? "" : string.Format("{0}{2} {1}", DataTypeGenerator.GenerateType(o.Type), o.Name, o.Nullable ? "?" : "");
		}

		public static string GenerateMethodParameterClientCode(WebApiMethodParameter o)
		{
			if (o.IsHidden) return "";

			if (o.Type.TypeMode == DataTypeMode.Class)
			{
				var ptype = o.Type as WebApiData;
				return string.Format("{0}{3} {1}{2}", ptype != null && ptype.HasClientType ? DataTypeGenerator.GenerateType(ptype.ClientType) : DataTypeGenerator.GenerateType(o.Type), o.Name, string.IsNullOrWhiteSpace(o.DefaultValue) ? "" : string.Format(" = {0}", o.DefaultValue), o.Nullable ? "?" : "");
			}
			if (o.Type.TypeMode == DataTypeMode.Struct)
			{
				var ptype = o.Type as WebApiData;
				return string.Format("{0}{3} {1}{2}", ptype != null && ptype.HasClientType ? DataTypeGenerator.GenerateType(ptype.ClientType) : DataTypeGenerator.GenerateType(o.Type), o.Name, string.IsNullOrWhiteSpace(o.DefaultValue) ? "" : string.Format(" = {0}", o.DefaultValue), o.Nullable ? "?" : "");
			}
			if (o.Type.TypeMode == DataTypeMode.Enum)
			{
				var ptype = o.Type as Projects.Enum;
				return string.Format("{0}{3} {1}{2}", ptype != null && ptype.HasClientType ? DataTypeGenerator.GenerateType(ptype.ClientType) : DataTypeGenerator.GenerateType(o.Type), o.Name, string.IsNullOrWhiteSpace(o.DefaultValue) ? "" : string.Format(" = {0}", o.DefaultValue), o.Nullable ? "?" : "");
			}

			return string.Format("{0}{3} {1}{2}", DataTypeGenerator.GenerateType(o.Type), o.Name, string.IsNullOrWhiteSpace(o.DefaultValue) ? "" : string.Format(" = {0}", o.DefaultValue), o.Nullable ? "?" : "");
		}

		#endregion
	}
}