using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using NETPath.Projects;
using NETPath.Projects.Helpers;

namespace NETPath.Generators.NET.CS
{
	internal static class RESTServiceGenerator
	{

		public static void VerifyCode(RESTService o, Action<CompileMessage> AddMessage)
		{
			if (string.IsNullOrEmpty(o.Name))
				AddMessage(new CompileMessage("GS2000", "An service in the '" + o.Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
			else
				if (RegExs.MatchCodeName.IsMatch(o.Name) == false)
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

				if(m.Parameters.Count > 1 && (m.BodyStyle == WebMessageBodyStyle.Bare || m.BodyStyle == WebMessageBodyStyle.WrappedResponse))
					AddMessage(new CompileMessage("GS2017", "The method '" + m.ServerName + "' in the '" + o.Name + "' service contains more than one parameter and therefore cannot use the Bare or WrappedResponse BodyStyles. Please specify a valid body style.", CompileMessageSeverity.WARN, o, m, m.GetType(), o.Parent.Owner.ID));

				if (m.ReturnType == null)
					AddMessage(new CompileMessage("GS2007", "The method '" + m.ServerName + "' in the '" + o.Name + "' service has a blank Return Type. A Return Type MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
				else
					if (m.ReturnType.TypeMode == DataTypeMode.Primitive && (m.ReturnType.Primitive == PrimitiveTypes.Void || m.ReturnType.Primitive == PrimitiveTypes.None))
						AddMessage(new CompileMessage("GS2012", "The method return type '" + m.ReturnType + "' in the '" + o.Name + "' service is not a valid REST return type. Please specify a valid REST return type.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
				if (m.ReturnType.TypeMode == DataTypeMode.Namespace || m.ReturnType.TypeMode == DataTypeMode.Interface)
					AddMessage(new CompileMessage("GS2013", "The method return type '" + m.ReturnType + "' in the '" + o.Name + "' service is not a valid return type. Please specify a valid return type.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));

				foreach (RESTMethodParameter mp in m.Parameters)
				{
					if(string.IsNullOrEmpty(mp.Name))
						AddMessage(new CompileMessage("GS2008", "The method parameter '" + m.ServerName + "' in the '" + o.Name + "' service has a parameter with a blank name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
					if (mp.IsRESTInvalid)
						AddMessage(new CompileMessage("GS2009", "The method REST parameter '" + m.ServerName + "' in the '" + m.ServerName + "' method is not a valid REST parameter. Please specify a valid REST parameter.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
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
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t[ServiceContract({0}{1}Namespace = \"{2}\")]", o.ProtectionLevel != System.Net.Security.ProtectionLevel.None ? string.Format("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel)) : "", !string.IsNullOrEmpty(o.ConfigurationName) ? string.Format("ConfigurationName = \"{0}\", ", o.ConfigurationName) : "", o.Parent.FullURI));
			code.AppendLine(string.Format("\t{0} interface I{1}", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
			code.AppendLine("\t{");
			foreach (RESTMethod m in o.ServiceOperations)
				code.AppendLine(GenerateServiceInterfaceMethod(m));
			code.AppendLine("\t}");

			//Generate the service proxy
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t[System.ServiceModel.ServiceBehaviorAttribute(AutomaticSessionShutdown = {0}, ConcurrencyMode = ConcurrencyMode.{1}, IgnoreExtensionDataObject = {2}, IncludeExceptionDetailInFaults = {3}, MaxItemsInObjectGraph = {4}, {5}TransactionTimeout = \"{6}\", UseSynchronizationContext = {7}, ValidateMustUnderstand = {8}, AddressFilterMode = AddressFilterMode.{9}, EnsureOrderedDispatch = {10}, InstanceContextMode = InstanceContextMode.{11}, ReleaseServiceInstanceOnTransactionComplete = {12}, TransactionAutoCompleteOnSessionClose = {13})]", o.SBAutomaticSessionShutdown ? "true" : "false", o.SBConcurrencyMode, o.SBIgnoreExtensionDataObject ? "true" : "false", o.SBIncludeExceptionDetailInFaults ? "true" : "false", o.SBMaxItemsInObjectGraph > 0 ? Convert.ToString(o.SBMaxItemsInObjectGraph) : "Int32.MaxValue", o.SBTransactionIsolationLevel != IsolationLevel.Unspecified ? string.Format("TransactionIsolationLevel = System.Transactions.IsolationLevel.{0}", o.SBTransactionIsolationLevel) : "", o.SBTransactionTimeout, o.SBUseSynchronizationContext ? "true" : "false", o.SBValidateMustUnderstand ? "true" : "false", o.SBAddressFilterMode, o.SBEnsureOrderedDispatch ? "true" : "false", o.SBInstanceContextMode, o.SBReleaseServiceInstanceOnTransactionComplete ? "true" : "false", o.SBTransactionAutoCompleteOnSessionClose ? "true" : "false"));
			code.AppendLine(string.Format("\t{0} abstract class {1}Base<T> : RESTServerBase, I{1} where T : {1}Base<T>", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
			code.AppendLine("\t{");
			code.AppendLine(string.Format("\t\tpublic {0}Base(string BaseAddress, string DefaultEndpointAddress) : base(typeof(T), new Uri[] {{ new Uri(BaseAddress) }})", o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tthis.DefaultEndpointAddress = new Uri(DefaultEndpointAddress);");
			code.AppendLine("\t\t\tInitialize();");
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}Base(Uri BaseURI, Uri DefaultEndpointAddress) : base(typeof(T), new Uri[] {{ BaseURI }})", o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tthis.DefaultEndpointAddress = DefaultEndpointAddress;");
			code.AppendLine("\t\t\tInitialize();");
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}Base(Uri[] BaseURIs, Uri DefaultEndpointAddress) : base(typeof(T), BaseURIs)", o.Name));
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

		public static string GenerateClientCode40(RESTService o)
		{
			var code = new StringBuilder();

			//Generate the Proxy Class
			if (o.ClientDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ClientDocumentation));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} abstract partial class {1}", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
			code.AppendLine("\t{");
			code.AppendLine(string.Format("\t\tpublic {0}(string BaseURI)", o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t}");
			code.AppendLine();
			foreach (RESTMethod m in o.ServiceOperations)
				code.AppendLine(GenerateMethodClientCode40(m));
			code.AppendLine("\t}");

			return code.ToString();
		}

		public static string GenerateClientCode45(RESTService o)
		{
			var code = new StringBuilder();

			//Generate the Proxy Class
			if (o.ClientDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ClientDocumentation));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} partial class {1}", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
			code.AppendLine("\t{");
			code.AppendLine(string.Format("\t\tpublic {0}()", o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t}");
			code.AppendLine();
			foreach (RESTMethod m in o.ServiceOperations)
				code.AppendLine(GenerateMethodClientCode45(m));
			code.AppendLine("\t}");

			return code.ToString();
		}

		#endregion

		#region - Service Interface Methods -

		public static string GenerateServiceInterfaceMethod(RESTMethod o)
		{
			var code = new StringBuilder();

			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (RESTMethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
			}
			code.AppendLine(string.Format("\t\t[OperationContract({0}{1})]", o.IsOneWay ? "IsOneWay = true, " : "", string.Format("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}", o.ProtectionLevel)));
			code.AppendLine(string.Format("\t\t[System.ServiceModel.Web.{0}(UriTemplate=\"{1}\", {2}BodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.{3}, RequestFormat = System.ServiceModel.Web.WebMessageFormat.{4}, ResponseFormat = System.ServiceModel.Web.WebMessageFormat.{5})]", o.Method == MethodRESTVerbs.GET ? "WebGet" : "WebInvoke", o.UriTemplate, o.Method != MethodRESTVerbs.GET ? string.Format("Method = \"{0}\", ", o.Method) : "", o.BodyStyle, o.RequestFormat, o.ResponseFormat));
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
			var code = new StringBuilder();
			code.AppendFormat("\t\tpublic abstract {0} {1}(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
			foreach (RESTMethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");
			return code.ToString();
		}

		#endregion

		#region - Client Proxy Methods -

		public static string GenerateMethodClientSync(RESTMethod o)
		{
			var code = new StringBuilder();
			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (RESTMethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
			}
			code.AppendFormat("\t\tpublic {0} {1}(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
			foreach (RESTMethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t}");
			return code.ToString();
		}

		public static string GenerateMethodClientCode40(RESTMethod o)
		{
			var code = new StringBuilder();

			if (o.ClientAsync)
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (RESTMethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				code.AppendFormat("\t\tpublic void {0}(", o.ServerName);
				foreach (RESTMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t}");
			}
			else
				code.Append(GenerateMethodClientSync(o));

			return code.ToString();
		}

		public static string GenerateMethodClientCode45(RESTMethod o)
		{
			var code = new StringBuilder();

			if (o.ClientAsync)
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (RESTMethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tpublic System.Threading.Tasks.Task {0}Async(", o.ServerName);
				else code.AppendFormat("\t\tpublic System.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
				foreach (RESTMethodParameter op in o.Parameters)
					code.AppendFormat("{0}{1}", GenerateMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t}");
			}
			else
				code.Append(GenerateMethodClientSync(o));

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
				return string.Format("{0} {1}", ptype != null && ptype.HasClientType ? DataTypeGenerator.GenerateType(ptype.ClientType) : DataTypeGenerator.GenerateType(o.Type), o.Name);
			}													    
			if (o.Type.TypeMode == DataTypeMode.Struct)			    
			{													    
				var ptype = o.Type as Data;						    
				return string.Format("{0} {1}", ptype != null && ptype.HasClientType ? DataTypeGenerator.GenerateType(ptype.ClientType) : DataTypeGenerator.GenerateType(o.Type), o.Name);
			}													    
			if (o.Type.TypeMode == DataTypeMode.Enum)			    
			{													    
				var ptype = o.Type as Projects.Enum;			    
				return string.Format("{0} {1}", ptype != null && ptype.HasClientType ? DataTypeGenerator.GenerateType(ptype.ClientType) : DataTypeGenerator.GenerateType(o.Type), o.Name);
			}
			
			return string.Format("{0} {1}", DataTypeGenerator.GenerateType(o.Type), o.Name);
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
					code.AppendLine(string.Format("\t\t\tDebugBehavior.HttpHelpPageBinding = new {0}();", DataTypeGenerator.GenerateType(b.HttpHelpPageBinding)));
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