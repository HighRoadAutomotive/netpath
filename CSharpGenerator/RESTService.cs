using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using NETPath.Projects.Helpers;

namespace NETPath.Generators.CS
{
	internal static class RESTServiceGenerator
	{

		public static void VerifyCode(RESTService o, Action<CompileMessage> AddMessage)
		{
			if (string.IsNullOrEmpty(o.Name))
				AddMessage(new CompileMessage("GS2000", "An service in the '" + o.Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
			else if (RegExs.MatchCodeName.IsMatch(o.Name) == false)
				AddMessage(new CompileMessage("GS2001", "The service '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
			if (o.HasClientType)
				if (RegExs.MatchCodeName.IsMatch(o.ClientType.Name) == false)
					AddMessage(new CompileMessage("GS2002", "The service '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

			var Operations = new ObservableCollection<RESTMethod>();
			Operations.AddRange(o.ServiceOperations);

			foreach (RESTMethod m in Operations)
			{
				if (string.IsNullOrEmpty(m.ServerName))
					AddMessage(new CompileMessage("GS2004", "An method in the '" + o.Name + "' service has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
				else
				{
					if (RegExs.MatchCodeName.IsMatch(m.ServerName) == false)
						AddMessage(new CompileMessage("GS2005", "The method '" + m.ServerName + "' in the '" + o.Name + "' service contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
					if (m.ServerName == "__callback")
						AddMessage(new CompileMessage("GS2015", "The name of the method '" + m.ServerName + "' in the '" + o.Name + "' service is invalid. Please rename it.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
				}

				if (m.Parameters.Count > 1 && (m.BodyStyle == WebMessageBodyStyle.Bare || m.BodyStyle == WebMessageBodyStyle.WrappedResponse))
					AddMessage(new CompileMessage("GS2017", "The method '" + m.ServerName + "' in the '" + o.Name + "' service contains more than one parameter and therefore cannot use the Bare or WrappedResponse BodyStyles. Please specify a valid body style.", CompileMessageSeverity.WARN, o, m, m.GetType(), o.Parent.Owner.ID));

				if (m.ReturnType == null)
					AddMessage(new CompileMessage("GS2007", "The method '" + m.ServerName + "' in the '" + o.Name + "' service has a blank Return Type. A Return Type MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
				else if (m.ReturnType.TypeMode == DataTypeMode.Primitive && (m.ReturnType.Primitive == PrimitiveTypes.Void || m.ReturnType.Primitive == PrimitiveTypes.None))
					AddMessage(new CompileMessage("GS2012", "The method return type '" + m.ReturnType + "' in the '" + o.Name + "' service is not a valid REST return type. Please specify a valid REST return type.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
				if (m.ReturnType.TypeMode == DataTypeMode.Namespace || m.ReturnType.TypeMode == DataTypeMode.Interface)
					AddMessage(new CompileMessage("GS2013", "The method return type '" + m.ReturnType + "' in the '" + o.Name + "' service is not a valid return type. Please specify a valid return type.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
				if (m.RequestConfiguration == null)
					AddMessage(new CompileMessage("GS2017", "The Request Configuration type in method '" + m.ServerName + "' in the '" + o.Name + "' service is not set. A Request Configuration MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));

				foreach (RESTMethodParameter mp in m.Parameters)
				{
					if (string.IsNullOrEmpty(mp.Name))
						AddMessage(new CompileMessage("GS2008", "The method parameter '" + m.ServerName + "' in the '" + o.Name + "' service has a parameter with a blank name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
					//if (mp.IsRESTInvalid)
					//	AddMessage(new CompileMessage("GS2009", "The method REST parameter '" + m.ServerName + "' in the '" + m.ServerName + "' method is not a valid REST parameter. Please specify a valid REST parameter.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
					if (mp.Name == "__callback")
						AddMessage(new CompileMessage("GS2016", "The name of the method parameter '" + mp.Name + "' in the '" + m.ServerName + "' method is invalid. Please rename it.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
				}
			}
		}

		#region - Server Interfaces -

		public static string GenerateServerCode(RESTService o)
		{
			var code = new StringBuilder();
			if (o.ServiceDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ServiceDocumentation));
			foreach (DataType dt in o.KnownTypes)
				code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCode(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t[ServiceContract({0}{1}Namespace = \"{2}\")]", o.ProtectionLevel != System.Net.Security.ProtectionLevel.None ? string.Format("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel)) : "", !string.IsNullOrEmpty(o.ConfigurationName) ? string.Format("ConfigurationName = \"{0}\", ", o.ConfigurationName) : "", o.Parent.FullURI));
			code.AppendLine(string.Format("\t{0} interface I{1}", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
			code.AppendLine("\t{");
			foreach (RESTMethod m in o.ServiceOperations)
				code.AppendLine(GenerateServiceInterfaceMethod(m));
			code.AppendLine("\t}");

			//Generate the service proxy
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCode(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t[System.ServiceModel.ServiceBehaviorAttribute(AutomaticSessionShutdown = {0}, ConcurrencyMode = ConcurrencyMode.{1}, IgnoreExtensionDataObject = {2}, IncludeExceptionDetailInFaults = {3}, MaxItemsInObjectGraph = {4}, {5}{6}UseSynchronizationContext = {7}, ValidateMustUnderstand = {8}, EnsureOrderedDispatch = {10}, InstanceContextMode = InstanceContextMode.{11}, {12}{13}AddressFilterMode = AddressFilterMode.{9})]", o.SBAutomaticSessionShutdown ? "true" : "false", o.SBConcurrencyMode, o.SBIgnoreExtensionDataObject ? "true" : "false", o.SBIncludeExceptionDetailInFaults ? "true" : "false", o.SBMaxItemsInObjectGraph > 0 ? Convert.ToString(o.SBMaxItemsInObjectGraph) : "Int32.MaxValue", o.SBTransactionIsolationLevel != IsolationLevel.Unspecified ? string.Format("TransactionIsolationLevel = System.Transactions.IsolationLevel.{0}, ", o.SBTransactionIsolationLevel) : "", o.SBTransactionTimeout.Ticks != 0L ? string.Format("TransactionTimeout = \"{0}\", ", o.SBTransactionTimeout) : "", o.SBUseSynchronizationContext ? "true" : "false", o.SBValidateMustUnderstand ? "true" : "false", o.SBAddressFilterMode, o.SBEnsureOrderedDispatch ? "true" : "false", o.SBInstanceContextMode, !o.SBReleaseServiceInstanceOnTransactionComplete ? string.Format("ReleaseServiceInstanceOnTransactionComplete = false, ") : "", o.SBTransactionAutoCompleteOnSessionClose ? string.Format("TransactionAutoCompleteOnSessionClose = true, ") : ""));
			code.AppendLine(string.Format("\t{0} abstract class {1}Base<T> : RESTServerBase, I{1} where T : {1}Base<T>", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
			code.AppendLine("\t{");
			code.AppendLine(string.Format("\t\tpublic {0}Base(string BaseAddress, string DefaultEndpointAddress) : base(typeof(T), new Uri[] {{ new Uri(BaseAddress) }}, WebHttpSecurityMode.{1})", o.Name, o.EndpointBinding.Security.Mode));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tthis.DefaultEndpointAddress = new Uri(DefaultEndpointAddress);");
			code.AppendLine("\t\t\tInitialize();");
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}Base(Uri BaseURI, Uri DefaultEndpointAddress) : base(typeof(T), new Uri[] {{ BaseURI }}, WebHttpSecurityMode.{1})", o.Name, o.EndpointBinding.Security.Mode));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tthis.DefaultEndpointAddress = DefaultEndpointAddress;");
			code.AppendLine("\t\t\tInitialize();");
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}Base(Uri[] BaseURIs, Uri DefaultEndpointAddress) : base(typeof(T), BaseURIs, WebHttpSecurityMode.{1})", o.Name, o.EndpointBinding.Security.Mode));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tthis.DefaultEndpointAddress = DefaultEndpointAddress;");
			code.AppendLine("\t\t\tInitialize();");
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine("\t\tprivate void Initialize()");
			code.AppendLine("\t\t{");
			code.AppendLine(GenerateBinding(o.EndpointBinding));
			code.AppendLine(GenerateCredentialsCode(o.Credentials));
			if (o.HasDebugBehavior) code.AppendLine(GenerateBehaviorCode(o.DebugBehavior));
			if (o.HasThrottlingBehavior) code.AppendLine(GenerateBehaviorCode(o.ThrottlingBehavior));
			if (o.HasWebHTTPBehavior) code.AppendLine(GenerateBehaviorCode(o.WebHTTPBehavior));
			code.AppendLine(string.Format("\t\t\tEndpoint = Host.AddServiceEndpoint(typeof(I{0}), Binding, DefaultEndpointAddress);", o.Name));
			code.AppendLine(string.Format("\t\t\tEndpoint.Behaviors.Add(WebHttpBehavior);"));
			code.AppendLine("\t\t}");
			foreach (RESTMethod m in o.ServiceOperations)
				code.AppendLine(GenerateServerProxyMethod(m));
			code.AppendLine("\t}");

			return code.ToString();
		}

		#endregion

		#region - Client Interfaces -

		public static string GenerateClientCode45(RESTService o)
		{
			var code = new StringBuilder();

			//Generate the Proxy Class
			if (o.ClientDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ClientDocumentation));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCode(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} {2}partial class {1}{3} : RestClientBase", DataTypeGenerator.GenerateScope(o.Scope), o.Name, o.Abstract ? "abstract " : "", o.Abstract ? "Base" : ""));
			code.AppendLine("\t{");
			foreach (RESTHTTPClientConfiguration c in o.RequestConfigurations.Where(a => a.GetType() == typeof(RESTHTTPClientConfiguration)).Select(t => t as RESTHTTPClientConfiguration).Where(c => o.ServiceOperations.Any(a => Equals(a.RequestConfiguration, c))))
			{
				code.AppendLine(string.Format("\t\tprivate System.Net.Http.HttpClient _{0}Client;", RegExs.ReplaceSpaces.Replace(c.Name, "")));
				code.AppendLine(string.Format("\t\tprotected System.Net.Http.HttpClient {0}Client {{ get {{ return _{0}Client; }} }}", RegExs.ReplaceSpaces.Replace(c.Name, "")));
			}
			code.AppendLine();
			code.AppendLine(string.Format("\t\t{1} {0}{2}(string BaseURI, CookieContainer Cookies = null, NetworkCredential Credentials = null, CredentialCache CredentialCache = null, IWebProxy Proxy = null)", o.Name, o.Abstract ? "protected" : "public", o.Abstract ? "Base" : ""));
			code.AppendLine("\t\t\t : base(BaseURI, Cookies, Credentials, CredentialCache, Proxy)");
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tInitialize();");
			code.AppendLine("\t\t}");
			code.AppendLine("\t\tprivate void Initialize()");
			code.AppendLine("\t\t{");
			foreach (RESTHTTPClientConfiguration c in o.RequestConfigurations.Where(a => a.GetType() == typeof(RESTHTTPClientConfiguration)).Select(t => t as RESTHTTPClientConfiguration).Where(c => o.ServiceOperations.Any(a => Equals(a.RequestConfiguration, c))))
				code.AppendLine(string.Format("\t\t\t_{0}Client = new System.Net.Http.HttpClient(new System.Net.Http.HttpClientHandler() {{ AllowAutoRedirect = {1}, AutomaticDecompression = System.Net.DecompressionMethods.{2}, ClientCertificateOptions = System.Net.Http.ClientCertificateOption.{3}, CookieContainer = {4}, Credentials = (this.Credentials == null) ? (ICredentials)this.CredentialCache : (ICredentials)this.Credentials, MaxAutomaticRedirections = {5}, MaxRequestContentBufferSize = {6}, PreAuthenticate = {7}, Proxy = this.Proxy, UseCookies = {8}, UseDefaultCredentials = {9}, UseProxy = {10} }});",
					RegExs.ReplaceSpaces.Replace(c.Name, ""), c.AllowAutoRedirect ? bool.TrueString.ToLower() : bool.FalseString.ToLower(), c.AutomaticDecompression, c.ClientCertificateOptions,
					c.CookieContainerMode == CookieContainerMode.None ? "null" : c.CookieContainerMode == CookieContainerMode.Global ? "GlobalCookies" : c.CookieContainerMode == CookieContainerMode.Instance ? "Cookies" : "new CookieContainer()",
					c.MaxAutomaticRedirections, c.MaxRequestContentBufferSize, c.PreAuthenticate ? bool.TrueString.ToLower() : bool.FalseString.ToLower(),
					(c.CookieContainerMode == CookieContainerMode.None || c.CookieContainerMode == CookieContainerMode.Custom) ? bool.FalseString.ToLower() : bool.TrueString.ToLower(),
					c.UseDefaultCredentials ? bool.TrueString.ToLower() : bool.FalseString.ToLower(), c.UseProxy ? bool.TrueString.ToLower() : bool.FalseString.ToLower()));
			code.AppendLine("\t\t}");
			code.AppendLine();
			foreach (RESTMethod m in o.ServiceOperations.Where(a => a.RequestConfiguration.GetType() == typeof(RESTHTTPClientConfiguration)))
				code.AppendLine(m.ClientAsync ? GenerateClientMethodClientAsync45(m) : GenerateClientMethodClient45(m));
			foreach (RESTMethod m in o.ServiceOperations.Where(a => a.RequestConfiguration.GetType() == typeof(RESTHTTPWebConfiguration)))
				code.AppendLine(m.ClientAsync ? GenerateClientMethodWebAsync45(m) : GenerateClientMethodWeb45(m));
			code.AppendLine("\t}");

			return code.ToString();
		}

		#endregion

		#region - Service Interface Methods -

		public static string GenerateServiceInterfaceMethod(RESTMethod o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (RESTMethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
			}
			code.AppendLine(string.Format("\t\t[OperationContract({0}{1})]", o.IsOneWay ? "IsOneWay = true, " : "", string.Format("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}", o.ProtectionLevel)));
			code.AppendLine(string.Format("\t\t[System.ServiceModel.Web.{0}(UriTemplate=\"/{6}{1}\", {2}BodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.{3}, RequestFormat = System.ServiceModel.Web.WebMessageFormat.{4}, ResponseFormat = System.ServiceModel.Web.WebMessageFormat.{5})]", o.Method == MethodRESTVerbs.GET ? "WebGet" : "WebInvoke", o.UriTemplate, o.Method != MethodRESTVerbs.GET ? string.Format("Method = \"{0}\", ", o.Method) : "", o.BodyStyle, o.RequestFormat, o.ResponseFormat, o.ServerName));
			code.AppendFormat("\t\t{0} {1}(", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
			foreach (RESTMethodParameter op in o.Parameters)
				code.AppendFormat("{0},", GenerateMethodParameterServerCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 1, 1);
			code.AppendLine(");");

			return code.ToString();
		}

		#endregion

		#region - Server Proxy Methods -

		public static string GenerateServerProxyMethod(RESTMethod o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			code.AppendFormat("\t\tpublic abstract {0} {1}(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
			foreach (RESTMethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");
			return code.ToString();
		}

		#endregion

		#region - Client HttpWebRequest Methods -

		public static string GenerateClientMethodWeb45(RESTMethod o)
		{
			var conf = o.RequestConfiguration as RESTHTTPWebConfiguration;
			var code = new StringBuilder();
			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (RESTMethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				code.AppendLine(string.Format("\t\t///<param name='Configuration'>HTTP Client Configuration</param>"));
			}
			if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendFormat("\t\tpublic virtual async System.Threading.Tasks.Task {0}(", o.ServerName);
			else code.AppendFormat("\t\tpublic virtual async System.Threading.Tasks.Task {0}(", o.ServerName);
			foreach (RESTMethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			code.AppendLine(!o.UseDefaultHeaders ? "RESTHttpWebConfig Configuration = null)" : ")");
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tvar rc = Configuration ?? new RESTHttpWebConfig();");
			if (conf != null && conf.CookieContainerMode != CookieContainerMode.Custom) code.AppendLine(string.Format("\t\t\trc.CookieContainer = {0};", conf.CookieContainerMode == CookieContainerMode.Global ? "GlobalCookies" : conf.CookieContainerMode == CookieContainerMode.Instance ? "Cookies" : "null"));
			code.Append("\t\t\tvar rp = new Dictionary<string, object>() { ");
			foreach (RESTMethodParameter op in o.Parameters.Where(a => a.Type.TypeMode == DataTypeMode.Primitive || a.Type.TypeMode == DataTypeMode.Enum))
				code.AppendFormat("{{ \"{0}\", {0} }}, ", op.Name);
			code.AppendLine("};");
			if (o.Parameters.Any(a => a.Serialize))
			{
				var p = o.Parameters.First(a => a.Serialize);
				var pt = p.Type;
				if (o.ResponseFormat == WebMessageFormat.Json && pt.TypeMode != DataTypeMode.Primitive) code.AppendLine(string.Format("\t\t\tvar rd = Encoding.UTF8.GetBytes(SerializeJson<{0}>({1}));", DataTypeGenerator.GenerateType(pt), p.Name));
				if (o.ResponseFormat == WebMessageFormat.Xml && pt.TypeMode != DataTypeMode.Primitive) code.AppendLine(string.Format("\t\t\tvar rd = Encoding.UTF8.GetBytes(SerializeXml<{0}>({1}));", DataTypeGenerator.GenerateType(pt), p.Name));
				if (pt.TypeMode == DataTypeMode.Primitive && pt.Primitive == PrimitiveTypes.ByteArray)
					code.AppendLine(string.Format("\t\t\tvar rd = {0};", p.Name));
				if (pt.TypeMode == DataTypeMode.Primitive && pt.Primitive == PrimitiveTypes.String)
					code.AppendLine(string.Format("\t\t\tvar rd = Encoding.UTF8.GetBytes({0});", p.Name));
			}
			code.AppendLine(string.Format("\t\t\tvar rm = await rc.CreateRequestAsync(new Uri(BaseURI, ParseURI(\"{0}\", rp)).ToString(), \"{1}\", {2}, {3});", o.UriTemplate, o.Method, o.Parameters.Any(a => a.Serialize) ? "rd" : "null", o.RequestConfiguration.UseHTTP10 ? bool.TrueString.ToLower() : bool.FalseString.ToLower()));
			if (!conf.AllowAutoRedirect) code.AppendLine("\t\t\trm.AllowAutoRedirect = false;");
			if (!conf.AllowReadStreamBuffering) code.AppendLine("\t\t\trm.AllowReadStreamBuffering = false;");
			if (!conf.AllowWriteStreamBuffering) code.AppendLine("\t\t\trm.AllowWriteStreamBuffering = false;");
			if (conf.AuthenticationLevel != AuthenticationLevel.MutualAuthRequested) code.AppendLine(string.Format("\t\t\trm.AuthenticationLevel = System.Net.Security.AuthenticationLevel.{0}", conf.AuthenticationLevel));
			code.AppendLine(conf.CanContinue ? string.Format("\t\t\trm.ContinueTimeout = new TimeSpan({0});", conf.ContinueTimeout.Ticks) : "\t\t\trm.ContinueDelegate = null;");
			if (!(string.IsNullOrEmpty(conf.ConnectionGroupName))) code.AppendLine(string.Format("\t\t\trm.ConnectionGroupName = \"{0}\";", conf.ConnectionGroupName));
			if (conf.ImpersonationLevel != TokenImpersonationLevel.None) code.AppendLine(string.Format("\t\t\trm.ImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.{0}", conf.ImpersonationLevel));
			if (!conf.KeepAlive) code.AppendLine("\t\t\trm.KeepAlive = false;");
			if (conf.MaxAutomaticRedirections != 50) code.AppendLine(string.Format("\t\t\trm.MaxAutomaticRedirections = {0};", conf.MaxAutomaticRedirections));
			if (conf.MaximumResponseHeadersLength != -1) code.AppendLine(string.Format("\t\t\trm.MaximumResponseHeadersLength = {0};", conf.MaximumResponseHeadersLength));
			if (!(string.IsNullOrEmpty(conf.MediaType))) code.AppendLine(string.Format("\t\t\trm.MediaType = \"{0}\";", conf.MediaType));
			if (!conf.Pipelined) code.AppendLine("\t\t\trm.Pipelined = false;");
			if (conf.PreAuthenticate) code.AppendLine("\t\t\trm.PreAuthenticate = true;");
			if (conf.ReadWriteTimeout != new TimeSpan(0, 0, 30)) code.AppendLine(string.Format("\t\t\trm.ReadWriteTimeout = new TimeSpan({0});", conf.ReadWriteTimeout.Ticks));
			if (conf.RequestCacheLevel != HttpRequestCacheLevel.Default) code.AppendLine(string.Format("\t\t\trm.CachePolicy = HttpRequestCachePolicy(HttpRequestCacheLevel.{0});", conf.RequestCacheLevel));
			if (conf.SendChunked) code.AppendLine("\t\t\trm.SendChunked = true;");
			if (conf.Timeout != new TimeSpan(0, 0, 100)) code.AppendLine(string.Format("\t\t\trm.Timeout = new TimeSpan({0});", conf.Timeout.Ticks));
			if (conf.UnsafeAuthenticatedConnectionSharing) code.AppendLine("\t\t\trm.UnsafeAuthenticatedConnectionSharing = true;");
			if (conf.UseDefaultCredentials) code.AppendLine("\t\t\trm.UseDefaultCredentials = true;");
			code.AppendLine(string.Format("\t\t\tvar rr = (System.Net.HttpWebResponse) await rm.GetResponseAsync();"));
			code.AppendLine("\t\t\tEnsureSuccessStatusCode(rr.StatusCode, rr.StatusDescription);");
			if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendLine(string.Format("\t\t\t{0}Result(rr);", o.ServerName));
			else
			{
				code.AppendLine("\t\t\tvar rss = rr.GetResponseStream();");
				code.AppendLine("\t\t\tvar rsd = new byte[rr.ContentLength];");
				code.AppendLine("\t\t\tawait rss.ReadAsync(rsd, 0, Convert.ToInt32(rr.ContentLength));");
				code.AppendLine("\t\t\tvar rs = Encoding.UTF8.GetString(rsd);");
				code.AppendLine("\t\t\trr.Close();");
			}
			if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.Append("");
			else
			{
				if (o.ResponseFormat == WebMessageFormat.Json) code.AppendLine(string.Format("\t\t\t{1}Result(DeserializeJson<{0}>(rs), rr);", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName));
				if (o.ResponseFormat == WebMessageFormat.Xml) code.AppendLine(string.Format("\t\t\t{1}Result(DeserializeXml<{0}>(rs), rr);", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName));
			}
			code.AppendLine("\t\t}");
			if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendLine(string.Format("\t\tpublic abstract void {0}Result(System.Net.HttpWebResponse Response);", o.ServerName));
			else code.AppendLine(string.Format("\t\tpublic abstract void {0}Result({1} Value, System.Net.HttpWebResponse Response);", o.ServerName, DataTypeGenerator.GenerateType(o.ReturnType)));
			return code.ToString();
		}

		public static string GenerateClientMethodWebAsync45(RESTMethod o)
		{
			var conf = o.RequestConfiguration as RESTHTTPWebConfiguration;
			var code = new StringBuilder();

			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (RESTMethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				code.AppendLine(string.Format("\t\t///<param name='Configuration'>HTTP Client Configuration</param>"));
			}
			if (o.ReturnResponseData)
			{
				if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendFormat("\t\tpublic async virtual System.Threading.Tasks.Task<HttpWebResponse> {0}Async(", o.ServerName);
				else code.AppendFormat("\t\tpublic async virtual System.Threading.Tasks.Task<Tuple<{0}, HttpWebResponse>> {1}Async(", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
			}
			else
			{
				if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendFormat("\t\tpublic async virtual System.Threading.Tasks.Task {0}Async(", o.ServerName);
				else code.AppendFormat("\t\tpublic async virtual System.Threading.Tasks.Task<{0}> {1}Async(", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
			}
			foreach (RESTMethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			code.AppendLine(!o.UseDefaultHeaders ? "RESTHttpWebConfig Configuration = null)" : ")");
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSystem.Net.HttpWebResponse rr = null;");
			if (!(o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendLine("\t\t\tstring rs = \"\";");
			code.AppendLine("\t\t\tvar rc = Configuration ?? new RESTHttpWebConfig();");
			if (conf != null && conf.CookieContainerMode != CookieContainerMode.Custom) code.AppendLine(string.Format("\t\t\trc.CookieContainer = {0};", conf.CookieContainerMode == CookieContainerMode.Global ? "GlobalCookies" : conf.CookieContainerMode == CookieContainerMode.Instance ? "Cookies" : "null"));
			code.Append("\t\t\tvar rp = new Dictionary<string, object>() { ");
			foreach (RESTMethodParameter op in o.Parameters.Where(a => a.Type.TypeMode == DataTypeMode.Primitive || a.Type.TypeMode == DataTypeMode.Enum))
				code.AppendFormat("{{ \"{0}\", {0} }}, ", op.Name);
			code.AppendLine("};");
			if (o.Parameters.Any(a => a.Serialize))
			{
				var p = o.Parameters.First(a => a.Serialize);
				var pt = p.Type;
				if (o.ResponseFormat == WebMessageFormat.Json && pt.TypeMode != DataTypeMode.Primitive) code.AppendLine(string.Format("\t\t\tvar rd = Encoding.UTF8.GetBytes(SerializeJson<{0}>({1}));", DataTypeGenerator.GenerateType(pt), p.Name));
				if (o.ResponseFormat == WebMessageFormat.Xml && pt.TypeMode != DataTypeMode.Primitive) code.AppendLine(string.Format("\t\t\tvar rd = Encoding.UTF8.GetBytes(SerializeXml<{0}>({1}));", DataTypeGenerator.GenerateType(pt), p.Name));
				if (pt.TypeMode == DataTypeMode.Primitive && pt.Primitive == PrimitiveTypes.ByteArray)
					code.AppendLine(string.Format("\t\t\tvar rd = {0};", p.Name));
				if (pt.TypeMode == DataTypeMode.Primitive && pt.Primitive == PrimitiveTypes.String)
					code.AppendLine(string.Format("\t\t\tvar rd = Encoding.UTF8.GetBytes({0});", p.Name));
			}
			code.AppendLine(string.Format("\t\t\tvar rm = await rc.CreateRequestAsync(new Uri(BaseURI, ParseURI(\"{0}\", rp)).ToString(), \"{1}\", {2}, {3});", o.UriTemplate, o.Method, o.Parameters.Any(a => a.Serialize) ? "rd" : "null", o.RequestConfiguration.UseHTTP10 ? bool.TrueString.ToLower() : bool.FalseString.ToLower()));
			if (!conf.AllowAutoRedirect) code.AppendLine("\t\t\trm.AllowAutoRedirect = false;");
			if (!conf.AllowReadStreamBuffering) code.AppendLine("\t\t\trm.AllowReadStreamBuffering = false;");
			if (!conf.AllowWriteStreamBuffering) code.AppendLine("\t\t\trm.AllowWriteStreamBuffering = false;");
			if (conf.AuthenticationLevel != AuthenticationLevel.MutualAuthRequested) code.AppendLine(string.Format("\t\t\trm.AuthenticationLevel = System.Net.Security.AuthenticationLevel.{0}", conf.AuthenticationLevel));
			code.AppendLine(conf.CanContinue ? string.Format("\t\t\trm.ContinueTimeout = new TimeSpan({0});", conf.ContinueTimeout.Ticks) : "\t\t\trm.ContinueDelegate = null;");
			if (!(string.IsNullOrEmpty(conf.ConnectionGroupName))) code.AppendLine(string.Format("\t\t\trm.ConnectionGroupName = \"{0}\";", conf.ConnectionGroupName));
			if (conf.ImpersonationLevel != TokenImpersonationLevel.None) code.AppendLine(string.Format("\t\t\trm.ImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.{0}", conf.ImpersonationLevel));
			if (!conf.KeepAlive) code.AppendLine("\t\t\trm.KeepAlive = false;");
			if (conf.MaxAutomaticRedirections != 50) code.AppendLine(string.Format("\t\t\trm.MaxAutomaticRedirections = {0};", conf.MaxAutomaticRedirections));
			if (conf.MaximumResponseHeadersLength != -1) code.AppendLine(string.Format("\t\t\trm.MaximumResponseHeadersLength = {0};", conf.MaximumResponseHeadersLength));
			if (!(string.IsNullOrEmpty(conf.MediaType))) code.AppendLine(string.Format("\t\t\trm.MediaType = \"{0}\";", conf.MediaType));
			if (!conf.Pipelined) code.AppendLine("\t\t\trm.Pipelined = false;");
			if (conf.PreAuthenticate) code.AppendLine("\t\t\trm.PreAuthenticate = true;");
			if (conf.ReadWriteTimeout != new TimeSpan(0, 0, 30)) code.AppendLine(string.Format("\t\t\trm.ReadWriteTimeout = new TimeSpan({0});", conf.ReadWriteTimeout.Ticks));
			if (conf.RequestCacheLevel != HttpRequestCacheLevel.Default) code.AppendLine(string.Format("\t\t\trm.CachePolicy = HttpRequestCachePolicy(HttpRequestCacheLevel.{0});", conf.RequestCacheLevel));
			if (conf.SendChunked) code.AppendLine("\t\t\trm.SendChunked = true;");
			if (conf.Timeout != new TimeSpan(0, 0, 100)) code.AppendLine(string.Format("\t\t\trm.Timeout = new TimeSpan({0});", conf.Timeout.Ticks));
			if (conf.UnsafeAuthenticatedConnectionSharing) code.AppendLine("\t\t\trm.UnsafeAuthenticatedConnectionSharing = true;");
			if (conf.UseDefaultCredentials) code.AppendLine("\t\t\trm.UseDefaultCredentials = true;");
			code.AppendLine(string.Format("\t\t\trr = (System.Net.HttpWebResponse) await rm.GetResponseAsync();"));
			code.AppendLine("\t\t\tEnsureSuccessStatusCode(rr.StatusCode, rr.StatusDescription);");
			if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendLine("\t\t\treturn rr);");
			else
			{
				code.AppendLine("\t\t\tvar rss = rr.GetResponseStream();");
				code.AppendLine("\t\t\tvar rsd = new byte[rr.ContentLength];");
				code.AppendLine("\t\t\tawait rss.ReadAsync(rsd, 0, Convert.ToInt32(rr.ContentLength));");
				code.AppendLine("\t\t\trs = Encoding.UTF8.GetString(rsd);");
				code.AppendLine("\t\t\trr.Close();");
			}
			if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent)
			{
				if (o.ReturnResponseData)
				{
					if (o.ResponseFormat == WebMessageFormat.Json) code.AppendLine(string.Format("\t\t\t{0}Result(rr);", o.ServerName));
					if (o.ResponseFormat == WebMessageFormat.Xml) code.AppendLine(string.Format("\t\t\t{0}Result(rr);", o.ServerName));
				}
				else
				{
					if (o.ResponseFormat == WebMessageFormat.Json) code.AppendLine(string.Format("\t\t\t{0}Result();", o.ServerName));
					if (o.ResponseFormat == WebMessageFormat.Xml) code.AppendLine(string.Format("\t\t\t{0}Result();", o.ServerName));
				}
			}
			else
			{
				code.AppendLine("\t\t\tstring rs = await rr.Content.ReadAsStringAsync();");
				if (o.ReturnResponseData)
				{
					if (o.ResponseFormat == WebMessageFormat.Json) code.AppendLine(string.Format("\t\t\t{1}Result(DeserializeJson<{0}>(rs), rr);", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName));
					if (o.ResponseFormat == WebMessageFormat.Xml) code.AppendLine(string.Format("\t\t\t{1}Result(DeserializeXml<{0}>(rs), rr);", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName));
				}
				else
				{
					if (o.ResponseFormat == WebMessageFormat.Json) code.AppendLine(string.Format("\t\t\t{1}Result(DeserializeJson<{0}>(rs));", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName));
					if (o.ResponseFormat == WebMessageFormat.Xml) code.AppendLine(string.Format("\t\t\t{1}Result(DeserializeXml<{0}>(rs));", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName));
				}
			}
			code.AppendLine("\t\t}");
			if (o.ReturnResponseData)
			{
				if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendLine(string.Format("\t\tpublic abstract void {0}Result(System.Net.Http.HttpResponseMessage Response);", o.ServerName));
				else code.AppendLine(string.Format("\t\tpublic abstract void {0}Result({1} Value, System.Net.Http.HttpResponseMessage Response);", o.ServerName, DataTypeGenerator.GenerateType(o.ReturnType)));
			}
			else
			{
				if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendLine(string.Format("\t\tpublic abstract void {0}Result();", o.ServerName));
				else code.AppendLine(string.Format("\t\tpublic abstract void {0}Result({1} Value);", o.ServerName, DataTypeGenerator.GenerateType(o.ReturnType)));
			}
			code.AppendLine("\t\t}");

			return code.ToString();
		}

		#endregion

		#region - Client HttpClient Methods -

		public static string GenerateClientMethodClient45(RESTMethod o)
		{
			if (o.IsHidden) return "";
			var conf = o.RequestConfiguration as RESTHTTPClientConfiguration;
			var code = new StringBuilder();
			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (RESTMethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				if (!o.UseDefaultHeaders)
				{
					code.AppendLine(string.Format("\t\t///<param name='configuration'>HTTP Client Request Headers specific to this method.</param>"));
					code.AppendLine(string.Format("\t\t///<param name='ignoreDefaultHeaders'>Do not include the default headers in the request.</param>"));
				}
			}
			if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendFormat("\t\tpublic virtual async void {0}(", o.ServerName);
			else code.AppendFormat("\t\tpublic virtual async void {0}(", o.ServerName);
			foreach (RESTMethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.UseDefaultHeaders && o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(!o.UseDefaultHeaders ? "RestHttpClientRequestHeaders headers = null, bool ignoreDefaultHeaders = false)" : ")");
			code.AppendLine("\t\t{");
			code.AppendLine(string.Format("\t\t\tvar urisb = new StringBuilder(\"{0}\", 1024);", o.UriTemplate));
			foreach (RESTMethodParameter op in o.Parameters.Where(a => !a.Serialize))
			{
				if (op.Nullable)
					code.AppendLine(string.Format("\t\t\tif ({0}.HasValue) BuildUri<{2}>(urisb, \"{1}\", UriBuildMode.Query, {0}.Value);", op.Name, op.RestName, DataTypeGenerator.GenerateType(op.Type)));
				else
					code.AppendLine(string.Format("\t\t\tBuildUri<{3}>(urisb, \"{1}\", {2}, {0});", op.Name, op.RestName, op.IsPath ? "UriBuildMode.Path" : "UriBuildMode.Query", DataTypeGenerator.GenerateType(op.Type)));
			}
			if (o.Parameters.Any(a => a.Serialize))
			{
				var p = o.Parameters.First(a => a.Serialize);
				var pt = p.Type;
				if (o.ResponseFormat == WebMessageFormat.Json && pt.TypeMode != DataTypeMode.Primitive) code.AppendLine(string.Format("\t\t\tvar rd = new System.Net.Http.ByteArrayContent(Encoding.UTF8.GetBytes(SerializeJson<{0}>({1})));", DataTypeGenerator.GenerateType(pt), p.Name));
				if (o.ResponseFormat == WebMessageFormat.Xml && pt.TypeMode != DataTypeMode.Primitive) code.AppendLine(string.Format("\t\t\tvar rd = new System.Net.Http.ByteArrayContent(Encoding.UTF8.GetBytes(SerializeXml<{0}>({1})));", DataTypeGenerator.GenerateType(pt), p.Name));
				if (pt.TypeMode == DataTypeMode.Primitive && pt.Primitive == PrimitiveTypes.ByteArray)
					code.AppendLine(string.Format("\t\t\tvar rd = new System.Net.Http.ByteArrayContent({0});", p.Name));
				if (pt.TypeMode == DataTypeMode.Primitive && pt.Primitive == PrimitiveTypes.String)
					code.AppendLine(string.Format("\t\t\tvar rd = new System.Net.Http.ByteArrayContent(Encoding.UTF8.GetBytes({0}));", p.Name));
			}
			code.AppendLine(
				string.Format(
					"\t\t\tvar rm = CreateHttpClientRequest(new Uri(BaseUri, urisb.ToString()).ToString(), System.Net.Http.HttpMethod.{0}{1}{2}{3});",
					o.Method == MethodRESTVerbs.GET ? "Get" : o.Method == MethodRESTVerbs.POST ? "Post" : o.Method == MethodRESTVerbs.PUT ? "Put" : "Delete",
					o.Parameters.Any(a => a.Serialize) ? ", rd" : "",
					o.RequestConfiguration.UseHTTP10 ? ", true" : "",
					!o.UseDefaultHeaders ? ", headers.Headers, ignoreDefaultHeaders" : ""));
			code.AppendLine(string.Format("\t\t\tvar rr = await {0}Client.SendAsync(rm, System.Net.Http.HttpCompletionOption.ResponseHeadersRead);", RegExs.ReplaceSpaces.Replace(o.RequestConfiguration.Name, "")));
			code.AppendLine("\t\t\trr.EnsureSuccessStatusCode();");
			if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent)
			{
				if (o.ReturnResponseData)
				{
					if (o.ResponseFormat == WebMessageFormat.Json) code.AppendLine(string.Format("\t\t\t{0}Result(rr);", o.ServerName));
					if (o.ResponseFormat == WebMessageFormat.Xml) code.AppendLine(string.Format("\t\t\t{0}Result(rr);", o.ServerName));
				}
				else
				{
					if (o.ResponseFormat == WebMessageFormat.Json) code.AppendLine(string.Format("\t\t\t{0}Result();", o.ServerName));
					if (o.ResponseFormat == WebMessageFormat.Xml) code.AppendLine(string.Format("\t\t\t{0}Result();", o.ServerName));
				}
			}
			else
			{
				code.AppendLine("\t\t\tstring rs = await rr.Content.ReadAsStringAsync();");
				if (o.ReturnResponseData)
				{
					if (o.ResponseFormat == WebMessageFormat.Json) code.AppendLine(string.Format("\t\t\t{1}Result(DeserializeJson<{0}>(rs), rr);", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName));
					if (o.ResponseFormat == WebMessageFormat.Xml) code.AppendLine(string.Format("\t\t\t{1}Result(DeserializeXml<{0}>(rs), rr);", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName));
				}
				else
				{
					if (o.ResponseFormat == WebMessageFormat.Json) code.AppendLine(string.Format("\t\t\t{1}Result(DeserializeJson<{0}>(rs));", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName));
					if (o.ResponseFormat == WebMessageFormat.Xml) code.AppendLine(string.Format("\t\t\t{1}Result(DeserializeXml<{0}>(rs));", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName));
				}
			}
			code.AppendLine("\t\t}");
			if (o.ReturnResponseData)
			{
				if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendLine(string.Format("\t\tpublic abstract void {0}Result(System.Net.Http.HttpResponseMessage Response);", o.ServerName));
				else code.AppendLine(string.Format("\t\tpublic abstract void {0}Result({1} Value, System.Net.Http.HttpResponseMessage Response);", o.ServerName, DataTypeGenerator.GenerateType(o.ReturnType)));
			}
			else
			{
				if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendLine(string.Format("\t\tpublic abstract void {0}Result();", o.ServerName));
				else code.AppendLine(string.Format("\t\tpublic abstract void {0}Result({1} Value);", o.ServerName, DataTypeGenerator.GenerateType(o.ReturnType)));
			}
			return code.ToString();
		}

		public static string GenerateClientMethodClientAsync45(RESTMethod o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (RESTMethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				if (!o.UseDefaultHeaders)
				{
					code.AppendLine(string.Format("\t\t///<param name='configuration'>HTTP Client Request Headers specific to this method.</param>"));
					code.AppendLine(string.Format("\t\t///<param name='ignoreDefaultHeaders'>Do not include the default headers in the request.</param>"));
				}
			}
			if (o.ReturnResponseData)
			{
				if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendFormat("\t\tpublic async virtual System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> {0}Async(", o.ServerName);
				else code.AppendFormat("\t\tpublic async virtual System.Threading.Tasks.Task<Tuple<{0}, System.Net.Http.HttpResponseMessage>> {1}Async(", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
			}
			else
			{
				if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.AppendFormat("\t\tpublic async virtual System.Threading.Tasks.Task {0}Async(", o.ServerName);
				else code.AppendFormat("\t\tpublic async virtual System.Threading.Tasks.Task<{0}> {1}Async(", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
			}
			foreach (RESTMethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.UseDefaultHeaders && o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(!o.UseDefaultHeaders ? "RestHttpClientRequestHeaders headers = null, bool ignoreDefaultHeaders = false)" : ")");
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSystem.Net.Http.HttpResponseMessage rr = null;");
			code.AppendLine(string.Format("\t\t\tvar urisb = new StringBuilder(\"{0}\", 1024);", o.UriTemplate));
			foreach (RESTMethodParameter op in o.Parameters.Where(a => !a.Serialize))
			{
				if (op.Nullable)
					code.AppendLine(string.Format("\t\t\tif ({0}.HasValue) BuildUri<{2}>(urisb, \"{1}\", UriBuildMode.Query, {0}.Value);", op.Name, op.RestName, DataTypeGenerator.GenerateType(op.Type)));
				else
					code.AppendLine(string.Format("\t\t\tBuildUri<{3}>(urisb, \"{1}\", {2}, {0});", op.Name, op.RestName, op.IsPath ? "UriBuildMode.Path" : "UriBuildMode.Query", DataTypeGenerator.GenerateType(op.Type)));
			}
			if (o.Parameters.Any(a => a.Serialize))
			{
				var p = o.Parameters.First(a => a.Serialize);
				var pt = p.Type;
				if (o.ResponseFormat == WebMessageFormat.Json && pt.TypeMode != DataTypeMode.Primitive) code.AppendLine(string.Format("\t\t\tvar rd = new System.Net.Http.ByteArrayContent(Encoding.UTF8.GetBytes(SerializeJson<{0}>({1})));", DataTypeGenerator.GenerateType(pt), p.Name));
				if (o.ResponseFormat == WebMessageFormat.Xml && pt.TypeMode != DataTypeMode.Primitive) code.AppendLine(string.Format("\t\t\tvar rd = new System.Net.Http.ByteArrayContent(Encoding.UTF8.GetBytes(SerializeXml<{0}>({1})));", DataTypeGenerator.GenerateType(pt), p.Name));
				if (pt.TypeMode == DataTypeMode.Primitive && pt.Primitive == PrimitiveTypes.ByteArray)
					code.AppendLine(string.Format("\t\t\tvar rd = new System.Net.Http.ByteArrayContent({0});", p.Name));
				if (pt.TypeMode == DataTypeMode.Primitive && pt.Primitive == PrimitiveTypes.String)
					code.AppendLine(string.Format("\t\t\tvar rd = new System.Net.Http.ByteArrayContent(Encoding.UTF8.GetBytes({0}));", p.Name));
			}
			code.AppendLine(
				string.Format(
					"\t\t\tvar rm = CreateHttpClientRequest(new Uri(BaseUri, urisb.ToString()).ToString(), System.Net.Http.HttpMethod.{0}{1}{2}{3});",
					o.Method == MethodRESTVerbs.GET ? "Get" : o.Method == MethodRESTVerbs.POST ? "Post" : o.Method == MethodRESTVerbs.PUT ? "Put" : "Delete",
					o.Parameters.Any(a => a.Serialize) ? ", rd" : "",
					o.RequestConfiguration.UseHTTP10 ? ", true" : "",
					!o.UseDefaultHeaders ? ", headers.Headers, ignoreDefaultHeaders" : ""));
			code.AppendLine(string.Format("\t\t\trr = await {0}Client.SendAsync(rm, System.Net.Http.HttpCompletionOption.ResponseHeadersRead);", RegExs.ReplaceSpaces.Replace(o.RequestConfiguration.Name, "")));
			code.AppendLine("\t\t\trr.EnsureSuccessStatusCode();");
			if ((o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) || !o.DeserializeContent) code.Append("");
			else
			{
				code.AppendLine("\t\t\tvar rs = await rr.Content.ReadAsStringAsync();");
				if (o.ReturnResponseData)
				{
					if (o.ResponseFormat == WebMessageFormat.Json) code.AppendLine(string.Format("\t\t\treturn new Tuple<{0}, System.Net.Http.HttpResponseMessage>(DeserializeJson<{0}>(rs), rr);", DataTypeGenerator.GenerateType(o.ReturnType)));
					if (o.ResponseFormat == WebMessageFormat.Xml) code.AppendLine(string.Format("\t\t\treturn new Tuple<{0}, System.Net.Http.HttpResponseMessage>(DeserializeXml<{0}>(rs), rr);", DataTypeGenerator.GenerateType(o.ReturnType)));
				}
				else
				{
					if (o.ResponseFormat == WebMessageFormat.Json) code.AppendLine(string.Format("\t\t\treturn DeserializeJson<{0}>(rs);", DataTypeGenerator.GenerateType(o.ReturnType)));
					if (o.ResponseFormat == WebMessageFormat.Xml) code.AppendLine(string.Format("\t\t\treturn DeserializeXml<{0}>(rs);", DataTypeGenerator.GenerateType(o.ReturnType)));
				}
			}
			code.AppendLine("\t\t}");

			return code.ToString();
		}

		#endregion

		#region - Method Parameters -

		public static string GenerateMethodParameterServerCode(RESTMethodParameter o)
		{
			return o.IsHidden ? "" : string.Format("{0} {1}", DataTypeGenerator.GenerateType(o.Type), o.Name);
		}

		public static string GenerateMethodParameterClientCode(RESTMethodParameter o)
		{
			if (o.IsHidden) return "";

			if (o.Type.TypeMode == DataTypeMode.Class)
			{
				var ptype = o.Type as Data;
				return string.Format("{0}{3} {1}{2}", ptype != null && ptype.HasClientType ? DataTypeGenerator.GenerateType(ptype.ClientType) : DataTypeGenerator.GenerateType(o.Type), o.Name, string.IsNullOrWhiteSpace(o.DefaultValue) ? "" : string.Format(" = {0}", o.DefaultValue), o.Nullable ? "?" : "");
			}
			if (o.Type.TypeMode == DataTypeMode.Struct)
			{
				var ptype = o.Type as Data;
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

		#region - Host -

		public static string GenerateBinding(ServiceBindingWebHTTP b)
		{
			var code = new StringBuilder();
			code.AppendLine(string.Format("\t\t\tBinding.AllowCookies = {0};", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
			code.AppendLine(string.Format("\t\t\tBinding.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
			code.AppendLine(string.Format("\t\t\tBinding.CrossDomainScriptAccessEnabled = {0};", b.CrossDomainScriptAccessEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
			code.AppendLine(string.Format("\t\t\tBinding.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(HostNameComparisonMode), b.HostNameComparisonMode)));
			code.AppendLine(string.Format("\t\t\tBinding.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
			code.AppendLine(string.Format("\t\t\tBinding.MaxBufferSize = {0};", Convert.ToInt32(b.MaxBufferSize)));
			code.AppendLine(string.Format("\t\t\tBinding.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
			code.AppendLine(string.Format("\t\t\tBinding.TransferMode = TransferMode.{0};", System.Enum.GetName(typeof(TransferMode), b.TransferMode)));
			code.AppendLine(string.Format("\t\t\tBinding.WriteEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.WriteEncoding)));
			if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
			{
				code.AppendLine(string.Format("\t\t\tBinding.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
				code.AppendLine(string.Format("\t\t\tBinding.UseDefaultWebProxy = false;"));
			}
			if (b.Security != null)
			{
				code.AppendLine(string.Format("\t\t\tBinding.Security.Mode = WebHttpSecurityMode.{0};", System.Enum.GetName(typeof(WebHttpSecurityMode), b.Security.Mode)));
				code.AppendLine(string.Format("\t\t\tBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(HttpClientCredentialType), b.Security.TransportClientCredentialType)));
				code.AppendLine(string.Format("\t\t\tBinding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(HttpProxyCredentialType), b.Security.TransportProxyCredentialType)));
				code.AppendLine(string.Format("\t\t\tBinding.Security.Transport.Realm = \"{0}\";", b.Security.TransportRealm));
			}
			return code.ToString();
		}

		public static string GenerateCredentialsCode(HostCredentials o)
		{
			var code = new StringBuilder();
			if (o.UseCertificatesSecurity)
			{
				code.AppendLine(string.Format("\t\t\tHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), o.ClientCertificateAuthenticationValidationMode)));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.ClientCertificate.Authentication.IncludeWindowsGroups = {0};", o.ClientCertificateAuthenticationIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.ClientCertificate.Authentication.MapClientCertificateToWindowsAccount = {0};", o.ClientCertificateAuthenticationMapClientCertificateToWindowsAccount ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.ClientCertificate.Authentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), o.ClientCertificateAuthenticationRevocationMode)));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.ClientCertificate.Authentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), o.ClientCertificateAuthenticationStoreLocation)));
			}
			if (o.UseIssuedTokenSecurity)
			{
				foreach (string aauri in o.IssuedTokenAllowedAudiencesUris) code.AppendLine(string.Format("\t\t\tthis.IssuedTokenAuthentication.AllowedAudienceUris.Add(\"{0}\");", aauri));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.IssuedTokenAuthentication.AllowUntrustedRsaIssuers = {0};", o.IssuedTokenAllowUntrustedRsaIssuers ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.IssuedTokenAuthentication.AudienceUriMode = System.IdentityModel.Selectors.AudienceUriMode.{0};", System.Enum.GetName(typeof(System.IdentityModel.Selectors.AudienceUriMode), o.IssuedTokenAudienceUriMode)));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.IssuedTokenAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), o.IssuedTokenCertificateValidationMode)));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.IssuedTokenAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), o.IssuedTokenRevocationMode)));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.IssuedTokenAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), o.IssuedTokenTrustedStoreLocation)));
			}
			if (o.UsePeerSecurity)
			{
				code.AppendLine(string.Format("\t\t\tHost.Credentials.Peer.MeshPassword = \"{0}\";", o.PeerMeshPassword));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.Peer.MessageSenderAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), o.PeerMessageSenderAuthenticationCertificateValidationMode)));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.Peer.MessageSenderAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), o.PeerMessageSenderAuthenticationRevocationMode)));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.Peer.MessageSenderAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), o.PeerMessageSenderAuthenticationTrustedStoreLocation)));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.Peer.PeerAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), o.PeerAuthenticationCertificateValidationMode)));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.Peer.PeerAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), o.PeerAuthenticationRevocationMode)));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.Peer.PeerAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), o.PeerAuthenticationTrustedStoreLocation)));
			}
			if (o.UseUserNamePasswordSecurity)
			{
				code.AppendLine(string.Format("\t\t\tHost.Credentials.UserNameAuthentication.CachedLogonTokenLifetime = new TimeSpan({0});", o.UserNamePasswordCachedLogonTokenLifetime.Ticks));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.UserNameAuthentication.CacheLogonTokens = {0};", o.UserNamePasswordCacheLogonTokens ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.UserNameAuthentication.IncludeWindowsGroups = {0};", o.UserNamePasswordIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.UserNameAuthentication.MaxCachedLogonTokens = {0};", o.UserNamePasswordMaxCachedLogonTokens));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = System.ServiceModel.Security.UserNamePasswordValidationMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.Security.UserNamePasswordValidationMode), o.UserNamePasswordValidationMode)));
			}
			if (o.UseWindowsServiceSecurity)
			{
				code.AppendLine(string.Format("\t\t\tHost.Credentials.WindowsAuthentication.AllowAnonymousLogons = {0};", o.WindowsServiceAllowAnonymousLogons ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tHost.Credentials.WindowsAuthentication.IncludeWindowsGroups = {0};", o.WindowsServiceIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
			}
			return code.ToString();
		}

		public static string GenerateBehaviorCode(HostBehavior o)
		{
			Type t = o.GetType();
			var code = new StringBuilder();
			if (t == typeof(HostDebugBehavior))
			{
				var b = o as HostDebugBehavior;
				if (b == null) return "";
				if (b.HttpHelpPageEnabled)
				{
					code.AppendLine(string.Format("\t\t\tDebugBehavior.HttpHelpPageEnabled = {0};", b.HttpHelpPageEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
					code.AppendLine(string.Format("\t\t\tDebugBehavior.HttpHelpPageUrl = new Uri(\"{0}\");", b.HttpHelpPageUrl));
				}
				if (b.HttpsHelpPageEnabled)
				{
					code.AppendLine(string.Format("\t\t\tDebugBehavior.HttpsHelpPageEnabled = {0};", b.HttpsHelpPageEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
					code.AppendLine(string.Format("\t\t\tDebugBehavior.HttpsHelpPageUrl = new Uri(\"{0}\");", b.HttpsHelpPageUrl));
				}
				code.AppendLine(string.Format("\t\t\tDebugBehavior.IncludeExceptionDetailInFaults = {0};", b.IncludeExceptionDetailInFaults ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
			}
			if (t == typeof(HostThrottlingBehavior))
			{
				var b = o as HostThrottlingBehavior;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tThrottlingBehavior.MaxConcurrentCalls = {0};", b.MaxConcurrentCalls));
				code.AppendLine(string.Format("\t\t\tThrottlingBehavior.MaxConcurrentInstances = {0};", b.MaxConcurrentInstances));
				code.AppendLine(string.Format("\t\t\tThrottlingBehavior.MaxConcurrentSessions = {0};", b.MaxConcurrentSessions));
			}
			if (t == typeof(HostWebHTTPBehavior))
			{
				var b = o as HostWebHTTPBehavior;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tWebHttpBehavior.AutomaticFormatSelectionEnabled = {0};", b.AutomaticFormatSelectionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tWebHttpBehavior.DefaultBodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.{0};", System.Enum.GetName(typeof(WebMessageBodyStyle), b.DefaultBodyStyle)));
				code.AppendLine(string.Format("\t\t\tWebHttpBehavior.DefaultOutgoingRequestFormat = System.ServiceModel.Web.WebMessageFormat.{0};", System.Enum.GetName(typeof(WebMessageFormat), b.DefaultOutgoingRequestFormat)));
				code.AppendLine(string.Format("\t\t\tWebHttpBehavior.DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.{0};", System.Enum.GetName(typeof(WebMessageFormat), b.DefaultOutgoingResponseFormat)));
				code.AppendLine(string.Format("\t\t\tWebHttpBehavior.FaultExceptionEnabled = {0};", b.FaultExceptionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tWebHttpBehavior.HelpEnabled = {0};", b.HelpEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
			}
			return code.ToString();
		}

		#endregion
	}
}