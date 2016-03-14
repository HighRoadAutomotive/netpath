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
						AddMessage(new CompileMessage("GS2018", "The method parameter '" + m.Name + "' in the '" + o.Name + "' service has a blank route name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
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
						AddMessage(new CompileMessage("GS2018", "The method parameter '" + m.Name + "' in the '" + o.Name + "' service has a blank route name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
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
			var p = o.Parent.Owner as WebApiProject;

			//Generate the service proxy
			if (o.ServiceDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ServiceDocumentation));
			code.AppendLine($"\t[Route{(p.EnableEntityFramework7 ? "" : "Prefix")}(\"{o.Name.ToLowerInvariant()}\")]");
			code.AppendLine($"\t{DataTypeGenerator.GenerateScope(o.Scope)} abstract class {o.Name}Base : {(p.EnableEntityFramework7 ? "" : "Api")}Controller");
			code.AppendLine("\t{");

			code.AppendLine();
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
			var p = o.Parent.Owner as WebApiProject;
			if (p == null) return "";

			//Generate the Proxy Class
			if (o.ClientDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ClientDocumentation));
			code.AppendLine($"\t[System.CodeDom.Compiler.GeneratedCode(\"{Globals.ApplicationTitle}\", \"{Globals.ApplicationVersion}\")]");
			code.AppendLine($"\t{DataTypeGenerator.GenerateScope(o.Scope)} static partial class {o.Name}");
			code.AppendLine("\t{");
			code.AppendLine();
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Request Configuration");
				code.AppendLine();
			}
			code.AppendLine($"\t\tprivate static readonly string _baseUri = {(RegExs.MatchHttpUri.IsMatch(p.BaseUri) ? $"\"{p.BaseUri}/{o.Name.ToLowerInvariant()}\"" : p.BaseUri)};");
			code.AppendLine();
			code.AppendLine("\t\tstatic partial void CommonPreExecute(HttpRequestMessage headers);");
			code.AppendLine("\t\tstatic partial void CommonPostExecute(HttpResponseMessage headers);");
			code.AppendLine("\t\tstatic partial void SetCommonHeaders(HttpRequestHeaders headers);");
			code.AppendLine("\t\tstatic partial void SetDefaultHandler(ref HttpMessageHandler handler);");
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

			if (o.UriIncludesName)
				uriBuilder.AppendFormat("/{0}", o.Name.ToLowerInvariant());

			foreach (var pp in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => string.IsNullOrEmpty(a.DefaultValue)))
				uriBuilder.AppendFormat("/{{{0}}}", pp.RouteName.ToLowerInvariant());

			foreach (var pp in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => !string.IsNullOrEmpty(a.DefaultValue)))
				uriBuilder.AppendFormat("/{{{0}?}}", pp.RouteName.ToLowerInvariant());

			if (uriBuilder.Length > 0)
				uriBuilder.Remove(0, 1); //Remove the beginning slant from the Route

			return uriBuilder.ToString();
		}

		public static string GenerateServerProxyMethod(WebApiMethod o)
		{
			var p = o.Owner.Parent.Owner as WebApiProject;
			if (p == null) return "";
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (var mp in o.RouteParameters.OfType<WebApiMethodParameter>().Where(mp => mp.Documentation != null))
					code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				foreach (var mp in o.QueryParameters.Where(mp => mp.Documentation != null))
					code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
			}
			if (!string.IsNullOrWhiteSpace(o.AuthenticationFilter) || !string.IsNullOrWhiteSpace(o.Owner.AuthenticationFilter))
				code.AppendLine($"\t\t[{(!string.IsNullOrWhiteSpace(o.AuthenticationFilter) ? o.AuthenticationFilter : o.Owner.AuthenticationFilter)}]");
			if (!string.IsNullOrWhiteSpace(o.AuthorizationFilter) || !string.IsNullOrWhiteSpace(o.Owner.AuthorizationFilter))
				code.AppendLine($"\t\t[{(!string.IsNullOrWhiteSpace(o.AuthorizationFilter) ? o.AuthorizationFilter : o.Owner.AuthorizationFilter)}]");
			code.AppendLine($"\t\t[Route(\"{BuildUriTemplate(o)}\")]");
			if (o.Method != WebApiMethodVerbs.Custom)
				code.AppendLine($"\t\t[Http{o.Method}]");
			else
				code.AppendLine($"\t\t[AcceptVerbs(\"{o.CustomMethod}\")]");

			if (o.HasContent && o.ContentType.TypeMode == DataTypeMode.Primitive && o.ContentType.Primitive == PrimitiveTypes.String)
			{
				code.AppendFormat("\t\tpublic async {0} {1}Base(", p.EnableEntityFramework7 ? "Task<IActionResult>" : o.ServerAsync ? o.ReturnType.IsVoid ? "Task" : $"Task<{DataTypeGenerator.GenerateType(o.ReturnType)}>" : DataTypeGenerator.GenerateType(o.ReturnType), o.Name);
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
				code.AppendLine(p.EnableEntityFramework7 ? "await (new StreamReader(Request.Body)).ReadToEndAsync());" : "await Request.Content.ReadAsStringAsync());");
				code.AppendLine("\t\t}");
			}

			if (p.EnableEntityFramework7 && o.HasContent && o.ContentType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendFormat("\t\tpublic async Task<IActionResult> {0}Base(", o.Name);
				foreach (var op in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => string.IsNullOrEmpty(a.DefaultValue)))
					code.AppendFormat("{0}, ", GenerateMethodParameterServerCode(op));
				foreach (var op in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => !string.IsNullOrEmpty(a.DefaultValue)))
					code.AppendFormat("{0}, ", GenerateMethodParameterServerCode(op));
				foreach (var op in o.QueryParameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterServerCode(op));
				if (o.RouteParameters.OfType<WebApiMethodParameter>().Any() || o.QueryParameters.Any()) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tusing (var sr = new StreamReader(Request.Body))");
				code.AppendFormat("\t\t\t\treturn await {0}(", o.Name);
				foreach (var op in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => string.IsNullOrEmpty(a.DefaultValue)))
					code.AppendFormat("{0}, ", op.Name);
				foreach (var op in o.RouteParameters.OfType<WebApiMethodParameter>().Where(a => !string.IsNullOrEmpty(a.DefaultValue)))
					code.AppendFormat("{0}, ", op.Name);
				foreach (var op in o.QueryParameters)
					code.AppendFormat("{0}, ", op.Name);
				code.AppendLine("Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(await sr.ReadToEndAsync()));");
				code.AppendLine("\t\t}");
			}

			code.AppendFormat("\t\tpublic abstract {0} {1}(", p.EnableEntityFramework7 ? "Task<IActionResult>" : o.ServerAsync ? o.ReturnType.IsVoid ? "Task" : $"Task<{DataTypeGenerator.GenerateType(o.ReturnType)}>" : DataTypeGenerator.GenerateType(o.ReturnType), o.Name);
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
					code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				foreach (var mp in o.QueryParameters.Where(mp => mp.Documentation != null))
					code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
			}

			//Construct method declaration
			code.AppendFormat("\t\tpublic static {1} {0}(", o.Name, o.ReturnResponseData ? "System.Net.Http.HttpResponseMessage" : DataTypeGenerator.GenerateType(o.ReturnType));
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

			//Construct the Uri
			code.AppendLine("\t\t\tvar uri = new StringBuilder(_baseUri, 2048);");
			code.Append("\t\t\turi.Append(\"");
			code.Append($"/{o.Owner.Name.ToLowerInvariant()}");
			foreach (var op in o.RouteParameters)
			{
				if (op.GetType() == typeof (WebApiRouteParameter))
					code.Append($"/{op.RouteName.ToLowerInvariant()}");
			}
			if (o.UriIncludesName)
				code.Append($"/{o.Name.ToLowerInvariant()}");
			code.AppendLine("\");");
			foreach (var op in o.RouteParameters)
			{

				if (op.GetType() == typeof (WebApiMethodParameter))
					code.AppendLine($"\t\t\turi.AppendFormat(\"/{{0}}\", {op.Name});");
			}
			if (o.QueryParameters.Any())
				code.AppendLine("\t\t\turi.Append(\"?\");");
			foreach (var op in o.QueryParameters)
				code.AppendLine(string.Format("\t\t\tif ({1} != null) uri.AppendFormat(\"&{0}={{0}}\", {1});", op.RouteName.ToLowerInvariant(), op.Name));
			if (o.QueryParameters.Any())
				code.AppendLine("\t\t\turi.Replace(\"?&\", \"?\");");

			//Create the HttpRequestMessage
			code.AppendLine($"\t\t\tvar rm = new HttpRequestMessage(HttpMethod.{o.Method}, new Uri(uri.ToString(), UriKind.RelativeOrAbsolute));");
			if (conf.UseHTTP10)
				code.AppendLine("\t\t\trm.Version = new Version(1, 0);");
			code.AppendLine("\t\t\tCommonPreExecute(rm);");
			code.AppendLine($"\t\t\t{o.Name}PreExecute(rm);");
			code.AppendLine("\t\t\tSetCommonHeaders(rm.Headers);");
			code.AppendLine($"\t\t\tSet{o.Name}Headers(rm.Headers);");
			code.AppendLine("\t\t\trm.Headers.Accept.Clear();");
			code.AppendLine($"\t\t\trm.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(\"{((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.String) ? "text/plain" : conf.ResponseFormat == RestSerialization.Json ? "application/json" : conf.ResponseFormat == RestSerialization.Bson ? "application/bson" : "application/xml")}\"));");

			//Serialize any parameters
			if (o.HasContent)
			{
				var pt = o.ContentType;
				if(pt.TypeMode == DataTypeMode.Primitive && pt.Primitive == PrimitiveTypes.String)
					code.AppendLine($"\t\t\trm.Content = new System.Net.Http.StringContent({o.ContentParameterName});");
				else
					code.AppendLine($"\t\t\trm.Content = new System.Net.Http.ObjectContent<{DataTypeGenerator.GenerateType(pt)}>({o.ContentParameterName}, new {(conf.RequestFormat == RestSerialization.Json ? "JsonMediaTypeFormatter" : conf.RequestFormat == RestSerialization.Bson ? "BsonMediaTypeFormatter" : "XmlMediaTypeFormatter")}());");
			}

			code.AppendLine("\t\t\tSystem.Net.Http.HttpMessageHandler mr = new System.Net.Http.HttpClientHandler();");
			code.AppendLine("\t\t\tSetDefaultHandler(ref mr);");
			code.AppendLine($"\t\t\tSet{o.Name}Handler(ref mr);");
			code.AppendLine("\t\t\tusing(var client = new HttpClient(mr))");
			code.AppendLine("\t\t\t{");
			code.AppendLine("\t\t\t\tvar rr = client.SendAsync(rm, System.Net.Http.HttpCompletionOption.ResponseHeadersRead).Result;");
			if (o.EnsureSuccessStatusCode)
			{
				code.AppendLine("\t\t\t\tif(!rr.IsSuccessStatusCode())");
				code.AppendLine("\t\t\t\t\tthrow new SimpleHttpRequestException(rr.StatusCode, await rr.Content.ReadAsStringAsync().Result, rr.ReasonPhrase);");
			}
			code.AppendLine("\t\t\t\tCommonPostExecute(rr);");
			code.AppendLine($"\t\t\t\t{o.Name}PostExecute(rr);");
			if (o.ReturnResponseData)
			{
				code.AppendLine("\t\t\t\treturn rr;");
			}
			else if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.String)
			{
				code.AppendLine("\t\t\t\treturn await rr.Content.ReadAsStringAsync().Result;");
			}
			else if (!(o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) && o.DeserializeContent)
			{
				code.AppendLine($"\t\t\t\treturn rr.Content.ReadAsAsync<{DataTypeGenerator.GenerateType(o.ReturnType)}>(new MediaTypeFormatter[] {{ new {(conf.ResponseFormat == RestSerialization.Json ? "JsonMediaTypeFormatter" : conf.ResponseFormat == RestSerialization.Bson ? "BsonMediaTypeFormatter" : "XmlMediaTypeFormatter")}() }}).Result;");
			}
			code.AppendLine("\t\t\t}");
			code.AppendLine("\t\t}");
			code.AppendLine($"\t\tstatic partial void Set{o.Name}Handler(ref System.Net.Http.HttpMessageHandler handler);");
			code.AppendLine($"\t\tstatic partial void Set{o.Name}Headers(System.Net.Http.Headers.HttpRequestHeaders headers);");
			code.AppendLine($"\t\tstatic partial void {o.Name}PreExecute(HttpRequestMessage request);");
			code.AppendLine($"\t\tstatic partial void {o.Name}PostExecute(HttpResponseMessage response);");
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
					code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				foreach (var mp in o.QueryParameters.Where(mp => mp.Documentation != null))
					code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
			}

			//Construct method declaration
			code.AppendFormat("\t\tpublic static async System.Threading.Tasks.Task{0} {1}Async(", o.ReturnType.Primitive == PrimitiveTypes.Void ? "" : o.ReturnResponseData ? "<System.Net.Http.HttpResponseMessage>" : $"<{DataTypeGenerator.GenerateType(o.ReturnType)}>", o.Name);
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

			//Construct the Uri
			code.AppendLine("\t\t\tvar uri = new StringBuilder(_baseUri, 2048);");
			code.Append("\t\t\turi.Append(\"");
			code.Append($"/{o.Owner.Name.ToLowerInvariant()}");
			foreach (var op in o.RouteParameters)
			{
				if (op.GetType() == typeof(WebApiRouteParameter))
					code.Append($"/{op.RouteName.ToLowerInvariant()}");
			}
			if (o.UriIncludesName)
				code.Append($"/{o.Name.ToLowerInvariant()}");
			code.AppendLine("\");");
			foreach (var op in o.RouteParameters)
			{

				if (op.GetType() == typeof(WebApiMethodParameter))
					code.AppendLine($"\t\t\turi.AppendFormat(\"/{{0}}\", {op.Name});");
			}
			if (o.QueryParameters.Any())
				code.AppendLine("\t\t\turi.Append(\"?\");");
			foreach (var op in o.QueryParameters)
				code.AppendLine(string.Format("\t\t\tif ({1} != null) uri.AppendFormat(\"&{0}={{0}}\", {1});", op.RouteName.ToLowerInvariant(), op.Name));
			if (o.QueryParameters.Any())
				code.AppendLine("\t\t\turi.Replace(\"?&\", \"?\");");

			//Create the HttpRequestMessage
			code.AppendLine($"\t\t\tvar rm = new HttpRequestMessage(HttpMethod.{o.Method}, new Uri(uri.ToString(), UriKind.RelativeOrAbsolute));");
			if (conf.UseHTTP10)
				code.AppendLine("\t\t\trm.Version = new Version(1, 0);");
			code.AppendLine("\t\t\tCommonPreExecute(rm);");
			code.AppendLine($"\t\t\t{o.Name}PreExecute(rm);");
			code.AppendLine("\t\t\tSetCommonHeaders(rm.Headers);");
			code.AppendLine($"\t\t\tSet{o.Name}Headers(rm.Headers);");
			code.AppendLine("\t\t\trm.Headers.Accept.Clear();");
			code.AppendLine($"\t\t\trm.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(\"{((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.String) ? "text/plain" : conf.ResponseFormat == RestSerialization.Json ? "application/json" : conf.ResponseFormat == RestSerialization.Bson ? "application/bson" : "application/xml")}\"));");

			//Serialize any parameters
			if (o.HasContent)
			{
				var pt = o.ContentType;
				if (pt.TypeMode == DataTypeMode.Primitive && pt.Primitive == PrimitiveTypes.String)
					code.AppendLine($"\t\t\trm.Content = new System.Net.Http.StringContent({o.ContentParameterName});");
				else
					code.AppendLine($"\t\t\trm.Content = new System.Net.Http.ObjectContent<{DataTypeGenerator.GenerateType(pt)}>({o.ContentParameterName}, new {(conf.RequestFormat == RestSerialization.Json ? "JsonMediaTypeFormatter" : conf.RequestFormat == RestSerialization.Bson ? "BsonMediaTypeFormatter" : "XmlMediaTypeFormatter")}());");
			}

			code.AppendLine("\t\t\tSystem.Net.Http.HttpMessageHandler mr = new System.Net.Http.HttpClientHandler();");
			code.AppendLine("\t\t\tSetDefaultHandler(ref mr);");
			code.AppendLine($"\t\t\tSet{o.Name}Handler(ref mr);");
			code.AppendLine("\t\t\tusing(var client = new HttpClient(mr))");
			code.AppendLine("\t\t\t{");
			code.AppendLine("\t\t\t\tvar rr = await client.SendAsync(rm, System.Net.Http.HttpCompletionOption.ResponseHeadersRead);");
			if (o.EnsureSuccessStatusCode)
			{
				code.AppendLine("\t\t\t\tif(!rr.IsSuccessStatusCode)");
				code.AppendLine("\t\t\t\t\tthrow new SimpleHttpRequestException(rr.StatusCode, await rr.Content.ReadAsStringAsync(), rr.ReasonPhrase);");
			}
			code.AppendLine("\t\t\t\tCommonPostExecute(rr);");
			code.AppendLine($"\t\t\t\t{o.Name}PostExecute(rr);");
			if (o.ReturnResponseData)
			{
				code.AppendLine("\t\t\t\treturn rr;");
			}
			else if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.String)
			{
				code.AppendLine("\t\t\t\treturn await rr.Content.ReadAsStringAsync();");
			}
			else if (!(o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) && o.DeserializeContent)
			{
				code.AppendLine($"\t\t\t\treturn await rr.Content.ReadAsAsync<{DataTypeGenerator.GenerateType(o.ReturnType)}>(new MediaTypeFormatter[] {{ new {(conf.ResponseFormat == RestSerialization.Json ? "JsonMediaTypeFormatter" : conf.ResponseFormat == RestSerialization.Bson ? "BsonMediaTypeFormatter" : "XmlMediaTypeFormatter")}() }});");
			}
			code.AppendLine("\t\t\t}");
			code.AppendLine("\t\t}");
			code.AppendLine($"\t\tstatic partial void Set{o.Name}Handler(ref System.Net.Http.HttpMessageHandler handler);");
			code.AppendLine($"\t\tstatic partial void Set{o.Name}Headers(System.Net.Http.Headers.HttpRequestHeaders headers);");
			code.AppendLine($"\t\tstatic partial void {o.Name}PreExecute(HttpRequestMessage request);");
			code.AppendLine($"\t\tstatic partial void {o.Name}PostExecute(HttpResponseMessage response);");

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
				code.AppendLine($"{tabs}{l}");
		}

		#endregion

		#region - Method Parameters -

		public static string GenerateMethodParameterServerCode(WebApiMethodParameter o)
		{
			return o.IsHidden ? "" : $"{DataTypeGenerator.GenerateType(o.Type)} {o.Name}{(string.IsNullOrEmpty(o.DefaultValue) ? "" : $" = {o.DefaultValue}")}";
		}

		public static string GenerateMethodParameterClientCode(WebApiMethodParameter o)
		{
			if (o.IsHidden) return "";

			if (o.Type.TypeMode == DataTypeMode.Class || o.Type.TypeMode == DataTypeMode.Struct || o.Type.TypeMode == DataTypeMode.Enum)
			{
				var ptype = o.Type as WebApiData;
				return $"{(ptype != null && ptype.HasClientType ? DataTypeGenerator.GenerateType(ptype.ClientType) : DataTypeGenerator.GenerateType(o.Type))} {o.Name}{(string.IsNullOrWhiteSpace(o.DefaultValue) ? "" : $" = {o.DefaultValue}")}";
			}

			return $"{DataTypeGenerator.GenerateType(o.Type)} {o.Name}{(string.IsNullOrWhiteSpace(o.DefaultValue) ? "" : $" = {o.DefaultValue}")}";
		}

		#endregion

		#region - Update Service -

		public static void GenerateServerUpdateService(StringBuilder code, WebApiProject o)
		{
			if (o.EnableEntityFramework7)
				return;
			if (!o.ClientGenerationTargets.Any(c => c.TargetTypes.OfType<WebApiData>().Any(a => a.Elements.Any(b => b.EnableUpdates))))
				return;

			code.AppendLine($"namespace {o.Namespace.FullName}");
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
						code.AppendLine($"\tusing {pun.Namespace};");
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

			code.AppendLine($"\t[Route{(o.EnableEntityFramework7 ? "" : "Prefix")}(\"__dus\")]");
			code.AppendLine($"\tpublic sealed class DataUpdateServiceController : {(o.EnableEntityFramework7 ? "" : "Api")}Controller");
			code.AppendLine("\t{");
			foreach(var t in o.ClientGenerationTargets)
			{
				foreach (var d in t.TargetTypes.OfType<WebApiData>().Where(a => a.Elements.Any(b => b.EnableUpdates)))
				{
					foreach (var e in d.Elements.Where(a => a.EnableUpdates && a.DataType.TypeMode == DataTypeMode.Primitive && a.DataType.Primitive != PrimitiveTypes.Object))
					{
						if (!string.IsNullOrWhiteSpace(e.UpdateAuthenticationFilter))
							code.AppendLine($"\t\t[{e.UpdateAuthenticationFilter}]");
						if (!string.IsNullOrWhiteSpace(e.UpdateAuthorizationFilter))
							code.AppendLine($"\t\t[{e.UpdateAuthorizationFilter}]");
						code.Append($"\t\t[Route(\"{d.Name.ToLowerInvariant()}/{e.DataName.ToLowerInvariant()}");
						foreach (var l in d.Elements.Where(a => a.IsUpdateLookup))
							code.AppendFormat("/{{{0}}}", l.DataName);
						code.AppendLine("\")]");
						code.AppendLine("\t\t[HttpPut]");
						code.AppendFormat("\t\tpublic async Task{2} Update{0}{1}(", d.Name, e.DataName, o.EnableEntityFramework7 ? "<IActionResult>" : "");
						foreach (var l in d.Elements.Where(a => a.IsUpdateLookup))
							code.AppendFormat("{0} {1}, ", l.DataType, l.DataName);
						code.Remove(code.Length - 2, 2);
						code.AppendLine(")");
						code.AppendLine("\t\t{");
						if (e.DataType.Primitive == PrimitiveTypes.ByteArray)
						{
							if (o.EnableEntityFramework7)
							{
								code.AppendLine("\t\t\tvar value = new byte[Convert.ToInt32(Request.ContentLength.Value)];");
								code.AppendLine("\t\t\tawait Request.Body.ReadAsync(value, 0, Convert.ToInt32(Request.ContentLength.Value));");
							}
							else
								code.AppendLine("\t\t\tvar value = await Request.Content.ReadAsByteArrayAsync();");
						}
						else
						{
							code.AppendLine($"\t\t\tvar {(e.DataType.Primitive == PrimitiveTypes.String ? "value" : "payload")} = await {(o.EnableEntityFramework7 ? "(new StreamReader(Request.Body)).ReadToEndAsync()" : "Request.Content.ReadAsStringAsync()")};");
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
									code.AppendLine("\t\t\tvar value = Int16.Parse(payload, System.Globalization.CultureInfo.InvariantCulture);");
									break;
								case PrimitiveTypes.Int:
									code.AppendLine("\t\t\tvar value = Int32.Parse(payload, System.Globalization.CultureInfo.InvariantCulture);");
									break;
								case PrimitiveTypes.Long:
									code.AppendLine("\t\t\tvar value = Int64.Parse(payload, System.Globalization.CultureInfo.InvariantCulture);");
									break;
								case PrimitiveTypes.UShort:
									code.AppendLine("\t\t\tvar value = UInt16.Parse(payload, System.Globalization.CultureInfo.InvariantCulture);");
									break;
								case PrimitiveTypes.UInt:
									code.AppendLine("\t\t\tvar value = UInt32.Parse(payload, System.Globalization.CultureInfo.InvariantCulture);");
									break;
								case PrimitiveTypes.ULong:
									code.AppendLine("\t\t\tvar value = UInt64.Parse(payload, System.Globalization.CultureInfo.InvariantCulture);");
									break;
								case PrimitiveTypes.Float:
									code.AppendLine("\t\t\tvar value = Single.Parse(payload, System.Globalization.CultureInfo.InvariantCulture);");
									break;
								case PrimitiveTypes.Double:
									code.AppendLine("\t\t\tvar value = Double.Parse(payload, System.Globalization.CultureInfo.InvariantCulture);");
									break;
								case PrimitiveTypes.Decimal:
									code.AppendLine("\t\t\tvar value = Decimal.Parse(payload, System.Globalization.CultureInfo.InvariantCulture);");
									break;
								case PrimitiveTypes.DateTime:
									code.AppendLine("\t\t\tvar value = DateTime.Parse(payload, \"O\", System.Globalization.CultureInfo.InvariantCulture);");
									break;
								case PrimitiveTypes.DateTimeOffset:
									code.AppendLine("\t\t\tvar value = DateTimeOffset.Parse(payload, \"O\", System.Globalization.CultureInfo.InvariantCulture);");
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
							code.AppendLine($"\t\t\tusing (var db = new {o.EnitityDatabaseType}())");
							code.AppendLine("\t\t\t{");
							code.AppendFormat("\t\t\t\tvar item = await db.{0}.Where(a => ", d.EntityContext);
							foreach (var l in d.Elements.Where(a => a.IsUpdateLookup))
								code.AppendFormat("a.{0} == {1} && ", l.EntityName, l.DataName);
							code.Remove(code.Length - 4, 4);
							code.AppendLine(").FirstOrDefaultAsync();");
							if (o.EnableEntityFramework7)
								code.AppendLine("\t\t\t\tif (item == null) return new HttpNotFoundResult();");
							else
								code.AppendLine("\t\t\t\tif (item == null) throw new HttpResponseException(HttpStatusCode.NotFound);");
							code.AppendLine($"\t\t\t\titem.{e.EntityName} = value;");
							code.AppendLine("\t\t\t\tawait db.SaveChangesAsync();");
							if (o.EnableEntityFramework7)
								code.AppendLine("\t\t\t\treturn new HttpOkResult();");
							code.AppendLine("\t\t\t}");
						}

						code.AppendLine("\t\t}");
						code.AppendLine();
					}

				}
			}
			code.AppendLine("\t}");
			code.AppendLine("}");
			code.AppendLine();
		}

		public static void GenerateClientUpdateService(StringBuilder code, WebApiProject o)
		{
			if (!o.ClientGenerationTargets.Any(c => c.TargetTypes.OfType<WebApiData>().Any(a => a.Elements.Any(b => b.EnableUpdates))))
				return;

			code.AppendLine($"namespace {o.Namespace.FullName}");
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
						code.AppendLine($"\tusing {pun.Namespace};");
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
			code.AppendLine($"\t\tprivate static readonly string _baseUri = \"{o.Namespace.Uri}\";");
			code.AppendLine("\t\tprivate static readonly System.Net.Http.HttpClient _httpClient = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false, AutomaticDecompression = DecompressionMethods.None });");
			code.AppendLine();
			code.AppendLine("\t\tstatic partial void SetCommonUpdateHeaders(System.Net.Http.Headers.HttpRequestHeaders headers);");
			code.AppendLine();

			foreach (var t in o.ClientGenerationTargets)
			{
				foreach (var d in t.TargetTypes.OfType<WebApiData>().Where(a => a.Elements.Any(b => b.EnableUpdates)))
				{
					foreach (var e in d.Elements.Where(a => a.EnableUpdates && a.DataType.TypeMode == DataTypeMode.Primitive && a.DataType.Primitive != PrimitiveTypes.Object))
					{
						code.AppendFormat("\t\tpublic static async Task Update{0}{1}(", d.Name, e.DataName);
						foreach (var l in d.Elements.Where(a => a.IsUpdateLookup))
							code.AppendFormat("{0} {1}, ", l.DataType, l.DataName);
						code.AppendLine($"{e.DataType} {e.DataName})");
						code.AppendLine("\t\t{");
						code.AppendLine("\t\t\tvar uri = new StringBuilder(_baseUri, 256);");
						code.AppendLine($"\t\t\turi.Append(\"__dus/{d.Name.ToLowerInvariant()}/{e.DataName.ToLowerInvariant()}\");");
						foreach (var l in d.Elements.Where(a => a.IsUpdateLookup))
							code.AppendLine($"\t\t\turi.AppendFormat(\"/{{0}}\", {l.DataName});");

						code.AppendLine("\t\t\tvar rm = new HttpRequestMessage(HttpMethod.Put, new Uri(uri.ToString(), UriKind.RelativeOrAbsolute));");
						code.AppendLine("\t\t\tSetCommonUpdateHeaders(rm.Headers);");
						code.AppendLine($"\t\t\tSetUpdate{d.Name}{e.DataName}Headers(rm.Headers);");

						if (e.DataType.Primitive == PrimitiveTypes.ByteArray)
						{
							code.AppendLine($"\t\t\trm.Content = new System.Net.Http.ByteArrayContent({e.DataName});");
						}
						else
						{
							switch (e.DataType.Primitive)
							{
								case PrimitiveTypes.Byte:
									code.AppendLine($"\t\t\trm.Content = new System.Net.Http.StringContent(Convert.ToBase64String(new byte[] {{ {e.DataName} }}));");
									break;
								case PrimitiveTypes.SByte:
									code.AppendLine($"\t\t\trm.Content = new System.Net.Http.StringContent(Convert.ToBase64String(new byte[] {{ Convert.ToSByte({e.DataName}) }}));");
									break;
								case PrimitiveTypes.DateTime:
								case PrimitiveTypes.DateTimeOffset:
									code.AppendLine($"\t\t\trm.Content = new System.Net.Http.StringContent({e.DataName}.ToString(\"O\", System.Globalization.CultureInfoCultureInfo.InvariantCulture));");
									break;
								case PrimitiveTypes.TimeSpan:
									code.AppendLine($"\t\t\trm.Content = new System.Net.Http.StringContent({e.DataName}.Ticks);");
									break;
								default:
									code.AppendLine($"\t\t\trm.Content = new System.Net.Http.StringContent({e.DataName}.ToString(System.Globalization.CultureInfo.InvariantCulture));");
									break;
							}
						}

						code.AppendLine("\t\t\tawait _httpClient.SendAsync(rm, System.Net.Http.HttpCompletionOption.ResponseHeadersRead);");

						code.AppendLine("\t\t}");
						code.AppendLine($"\t\tstatic partial void SetUpdate{d.Name}{e.DataName}Headers(System.Net.Http.Headers.HttpRequestHeaders headers);");
						code.AppendLine();
					}
				}
			}
			code.AppendLine("\t}");
			code.AppendLine("}");
			code.AppendLine();
		}

		#endregion
	}
}