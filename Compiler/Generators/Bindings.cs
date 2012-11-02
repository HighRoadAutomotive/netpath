using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFArchitect.Projects;
using WCFArchitect.Projects.Helpers;

namespace WCFArchitect.Compiler.Generators
{
	internal static class BindingsCSGenerator
	{
		public static void VerifyCode(ServiceBinding o)
		{
			if (string.IsNullOrEmpty(o.Name))
				Program.AddMessage(new CompileMessage("GS6000", "A binding in the '" + o.Parent.Name + "' project has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
			else
				if (RegExs.MatchCodeName.IsMatch(o.Name) == false)
					Program.AddMessage(new CompileMessage("GS6001", "The binding '" + o.Name + "' in the '" + o.Parent.Name + "' project contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
			if (string.IsNullOrEmpty(o.Namespace)) { }
			else
				if (RegExs.MatchHTTPURI.IsMatch(o.Namespace) == false)
					Program.AddMessage(new CompileMessage("GS6002", "The Namespace '" + o.Namespace + "' for the '" + o.Name + "' binding in the '" + o.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));

			Type t = o.GetType();
			if (t == typeof(ServiceBindingBasicHTTP))
			{
				var b = o as ServiceBindingBasicHTTP;
				if (b != null && b.Security == null)
					Program.AddMessage(new CompileMessage("GS6003", "The Security for the '" + b.Name + "' Basic HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						Program.AddMessage(new CompileMessage("GS6004", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' Basic HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
			}
			else if (t == typeof(ServiceBindingBasicHTTPS) || t == typeof(ServiceBindingNetHTTP) || t == typeof(ServiceBindingNetHTTPS))
			{
				var b = o as ServiceBindingNetHTTP;
				if (b != null && ((Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET45 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) && b.Parent.Owner != null))
					Program.AddMessage(new CompileMessage("GS6001", "The binding '" + o.Name + "' in the '" + o.Parent.Name + "' project cannot be used with any other target framework level than 4.5. Please select .NET Framework 4.5 or Windows Runtime as your Target Framework in the Project page or remove this binding.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
				if (b != null && b.Security == null)
					Program.AddMessage(new CompileMessage("GS6003", "The Security for the '" + b.Name + "' Net HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						Program.AddMessage(new CompileMessage("GS6004", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' Net HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
			}
			else if (t == typeof(ServiceBindingWSHTTP))
			{
				var b = o as ServiceBindingWSHTTP;
				if (b != null && b.Security == null)
					Program.AddMessage(new CompileMessage("GS6005", "The Security for the '" + b.Name + "' WS HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						Program.AddMessage(new CompileMessage("GS6006", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
			}
			else if (t == typeof(ServiceBindingWS2007HTTP))
			{
				var b = o as ServiceBindingWS2007HTTP;
				if (b != null && b.Security == null)
					Program.AddMessage(new CompileMessage("GS6007", "The Security for the '" + b.Name + "' WS 2007 HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						Program.AddMessage(new CompileMessage("GS6008", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS 2007 HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
			}
			else if (t == typeof(ServiceBindingWSDualHTTP))
			{
				var b = o as ServiceBindingWSDualHTTP;
				if (b != null && b.Security == null)
					Program.AddMessage(new CompileMessage("GS6009", "The Security for the '" + b.Name + "' WS Dual Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						Program.AddMessage(new CompileMessage("GS6010", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS Dual HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
			}
			else if (t == typeof(ServiceBindingWSFederationHTTP))
			{
				var b = o as ServiceBindingWSFederationHTTP;
				if (b != null && b.Security == null)
					Program.AddMessage(new CompileMessage("GS6011", "The Security for the '" + b.Name + "' WS Federation HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						Program.AddMessage(new CompileMessage("GS6012", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS Federation HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
			}
			else if (t == typeof(ServiceBindingWS2007FederationHTTP))
			{
				var b = o as ServiceBindingWS2007FederationHTTP;
				if (b != null && b.Security == null)
					Program.AddMessage(new CompileMessage("GS6013", "The Security for the '" + b.Name + "' WS 2007 Federation HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						Program.AddMessage(new CompileMessage("GS6014", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS 2007 Federation HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
			}
			else if (t == typeof(ServiceBindingTCP))
			{
				var b = o as ServiceBindingTCP;
				if (b != null && b.Security == null)
					Program.AddMessage(new CompileMessage("GS6015", "The Security for the '" + b.Name + "' TCP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
			}
			else if (t == typeof(ServiceBindingNamedPipe))
			{
				var b = o as ServiceBindingNamedPipe;
				if (b != null && b.Security == null)
					Program.AddMessage(new CompileMessage("GS6016", "The Security for the '" + b.Name + "' Named Pipe Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
			}
			else if (t == typeof(ServiceBindingMSMQ))
			{
				var b = o as ServiceBindingMSMQ;
				if (b != null && b.Security == null)
					Program.AddMessage(new CompileMessage("GS6017", "The Security for the '" + b.Name + "' MSMQ Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
			}
			else if (t == typeof(ServiceBindingPeerTCP))
			{
				var b = o as ServiceBindingPeerTCP;
				if (b != null && b.Security == null)
					Program.AddMessage(new CompileMessage("GS6018", "The Security for the '" + b.Name + "' Peer TCP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
				if (b != null && !string.IsNullOrEmpty(b.ListenIPAddress))
					if (RegExs.MatchIPv4.IsMatch(b.ListenIPAddress) == false && RegExs.MatchIPv6.IsMatch(b.ListenIPAddress) == false)
						Program.AddMessage(new CompileMessage("GS6019", "The Listen IP Address for the '" + b.Name + "' Peer TCP Binding in the '" + b.Parent.Name + "' project is not valid.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
			}
			else if (t == typeof(ServiceBindingWebHTTP))
			{
				var b = o as ServiceBindingWebHTTP;
				if (b != null && b.Security == null)
					Program.AddMessage(new CompileMessage("GS6020", "The Security for the '" + b.Name + "' MSMQ Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						Program.AddMessage(new CompileMessage("GS6021", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' Basic HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
			}
			else if (t == typeof(ServiceBindingMSMQIntegration))
			{
				var b = o as ServiceBindingMSMQIntegration;
				if (b != null && b.Security == null)
					Program.AddMessage(new CompileMessage("GS6021", "The Security for the '" + b.Name + "' MSMQ Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.ID, b.ID));
			}
		}

		public static string GenerateCode30(ServiceBinding o)
		{
			Type t = o.GetType();
			return t == typeof(ServiceBindingWebHTTP) ? "" : GenerateCode35(o);
		}

		public static string GenerateCode35(ServiceBinding o)
		{
			Type t = o.GetType();
			var code = new StringBuilder();
			if (o.Parent.Owner.EnableDocumentationWarnings) code.AppendLine("#pragma warning disable 1591");
			if (o.Documentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0}", DataTypeCSGenerator.GenerateTypeDeclaration(o)));
			code.AppendLine("\t{");
			code.AppendLine(string.Format("\t\tpublic {0}()", o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t}");
			if (t == typeof(ServiceBindingBasicHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingBasicHTTPS)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpsSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingNetHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingNetHTTPS)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpsSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingWSHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingWS2007HTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingWSDualHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSDualHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingWSFederationHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSFederationHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingWS2007FederationHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSFederationHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingTCP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.NetTcpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingNamedPipe)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.NetNamedPipeSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingMSMQ)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.NetMsmqSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingPeerTCP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.PeerSecuritySettings CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingWebHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WebHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingMSMQIntegration)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity CustomSecurity)", o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t\tsec = CustomSecurity;");
			code.AppendLine("\t\t}");
			code.AppendLine(string.Format("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			code.AppendLine("\t\t}");
			if (t == typeof(ServiceBindingBasicHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingBasicHTTPS)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpsSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingNetHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingNetHTTPS)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpsSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingWSHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingWS2007HTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingWSDualHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSDualHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingWSFederationHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSFederationHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingWS2007FederationHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSFederationHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingTCP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.NetTcpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingNamedPipe)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.NetNamedPipeSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingMSMQ)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.NetMsmqSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingPeerTCP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.PeerSecuritySettings CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingWebHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WebHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingMSMQIntegration)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t\tsec = CustomSecurity;");
			code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			code.AppendLine("\t\t}");
			code.AppendLine("\t\tprivate void SetDefaults()");
			code.AppendLine("\t\t{");
			// Generic Binding code.
			code.AppendLine(string.Format("\t\t\tthis.CloseTimeout = new TimeSpan({0});", o.CloseTimeout.Ticks));
			code.AppendLine(string.Format("\t\t\tthis.Name = \"{0}\";", o.Name));
			code.AppendLine(string.Format("\t\t\tthis.Namespace = \"{0}\";", o.Namespace));
			code.AppendLine(string.Format("\t\t\tthis.OpenTimeout = new TimeSpan({0});", o.OpenTimeout.Ticks));
			code.AppendLine(string.Format("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});", o.ReceiveTimeout.Ticks));
			code.AppendLine(string.Format("\t\t\tthis.SendTimeout = new TimeSpan({0});", o.SendTimeout.Ticks));
			// Binding Specific code.
			if (t == typeof(ServiceBindingBasicHTTP))
			{
				var b = o as ServiceBindingBasicHTTP;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.AllowCookies = {0};", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferSize = {0};", Convert.ToInt32(b.MaxBufferSize)));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.MessageEncoding = WSMessageEncoding.{0};", System.Enum.GetName(typeof (System.ServiceModel.WSMessageEncoding), b.MessageEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof (ServiceBindingTextEncoding), b.TextEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.TransferMode = TransferMode.{0};", System.Enum.GetName(typeof (System.ServiceModel.TransferMode), b.TransferMode)));
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine(string.Format("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingWSHTTP))
			{
				var b = o as ServiceBindingWSHTTP;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.AllowCookies = {0};", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Enabled = {0};", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});", b.ReliableSessionInactivityTimeout.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Ordered = {0};", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.TransactionFlow = {0};", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine(string.Format("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingWS2007HTTP))
			{
				var b = o as ServiceBindingWS2007HTTP;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.AllowCookies = {0};", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Enabled = {0};", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});", b.ReliableSessionInactivityTimeout.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Ordered = {0};", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.TransactionFlow = {0};", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine(string.Format("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingWSDualHTTP))
			{
				var b = o as ServiceBindingWSDualHTTP;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				if (!string.IsNullOrEmpty(b.ClientBaseAddress)) code.AppendLine(string.Format("\t\t\tthis.ClientBaseAddress = new Uri(\"{0}\");", b.ClientBaseAddress));
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});", b.ReliableSessionInactivityTimeout.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Ordered = {0};", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.TransactionFlow = {0};", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine(string.Format("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingWSFederationHTTP))
			{
				var b = o as ServiceBindingWSFederationHTTP;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				if (b.PrivacyNoticeAt != "")
				{
					code.AppendLine(string.Format("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");", b.PrivacyNoticeAt));
					code.AppendLine(string.Format("\t\t\tthis.PrivacyNoticeVersion = {0};", b.PrivacyNoticeVersion));
				}
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Enabled = {0};", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});", b.ReliableSessionInactivityTimeout.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Ordered = {0};", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.TransactionFlow = {0};", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine(string.Format("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingWS2007FederationHTTP))
			{
				var b = o as ServiceBindingWS2007FederationHTTP;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				if (b.PrivacyNoticeAt != "")
				{
					code.AppendLine(string.Format("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");", b.PrivacyNoticeAt));
					code.AppendLine(string.Format("\t\t\tthis.PrivacyNoticeVersion = {0};", b.PrivacyNoticeVersion));
				}
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Enabled = {0};", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});", b.ReliableSessionInactivityTimeout.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Ordered = {0};", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.TransactionFlow = {0};", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine(string.Format("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingTCP))
			{
				var b = o as ServiceBindingTCP;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.ListenBacklog = {0};", b.ListenBacklog));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferSize = {0};", Convert.ToInt32(b.MaxBufferSize)));
				code.AppendLine(string.Format("\t\t\tthis.MaxConnections = {0};", b.MaxConnections));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.PortSharingEnabled = {0};", b.PortSharingEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Enabled = {0};", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});", b.ReliableSessionInactivityTimeout.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Ordered = {0};", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.TransactionFlow = {0};", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};", System.Enum.GetName(typeof(ServiceBindingTransactionProtocol), b.TransactionProtocol)));
				code.AppendLine(string.Format("\t\t\tthis.TransferMode = TransferMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode)));
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingNamedPipe))
			{
				var b = o as ServiceBindingNamedPipe;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferSize = {0};", Convert.ToInt32(b.MaxBufferSize)));
				code.AppendLine(string.Format("\t\t\tthis.MaxConnections = {0};", b.MaxConnections));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.TransactionFlow = {0};", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};", System.Enum.GetName(typeof(ServiceBindingTransactionProtocol), b.TransactionProtocol)));
				code.AppendLine(string.Format("\t\t\tthis.TransferMode = TransferMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode)));
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingMSMQ))
			{
				var b = o as ServiceBindingMSMQ;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");", b.CustomDeadLetterQueue));
				code.AppendLine(string.Format("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), b.DeadLetterQueue)));
				code.AppendLine(string.Format("\t\t\tthis.Durable = {0};", b.Durable ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ExactlyOnce = {0};", b.ExactlyOnce ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxRetryCycles = {0};", b.MaxRetryCycles));
				code.AppendLine(string.Format("\t\t\tthis.QueueTransferProtocol = QueueTransferProtocol.{0};", System.Enum.GetName(typeof(System.ServiceModel.QueueTransferProtocol), b.QueueTransferProtocol)));
				code.AppendLine(string.Format("\t\t\tthis.ReceiveContextEnabled = {0};", b.ReceiveContextEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), b.ReceiveErrorHandling)));
				code.AppendLine(string.Format("\t\t\tthis.ReceiveRetryCount = {0};", b.ReceiveRetryCount));
				code.AppendLine(string.Format("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});", b.RetryCycleDelay.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.TimeToLive = new TimeSpan({0});", b.TimeToLive.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.UseActiveDirectory = {0};", b.UseActiveDirectory ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.UseMSMQTracing = {0};", b.UseMSMQTracing ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.UseSourceJournal = {0};", b.UseSourceJournal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				if (b.ValidityDuration != TimeSpan.Zero) code.AppendLine(string.Format("\t\t\tthis.ValidityDuration = new TimeSpan({0});", b.ValidityDuration.Ticks));
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingPeerTCP))
			{
				var b = o as ServiceBindingPeerTCP;
				if (b == null) return "";
				code.AppendLine(b.ListenIPAddress == "" ? "\t\t\tthis.ListenIPAddress = null;" : string.Format("\t\t\tthis.ListenIPAddress = IPAddress.Parse(\"{0}\");", b.ListenIPAddress));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.Port = {0};", b.Port));
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingWebHTTP))
			{
				var b = o as ServiceBindingWebHTTP;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.AllowCookies = {0};", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.CrossDomainScriptAccessEnabled = {0};", b.CrossDomainScriptAccessEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferSize = {0};", Convert.ToInt32(b.MaxBufferSize)));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.TransferMode = TransferMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode)));
				code.AppendLine(string.Format("\t\t\tthis.WriteEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.WriteEncoding)));
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine(string.Format("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingMSMQIntegration))
			{
				var b = o as ServiceBindingMSMQIntegration;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");", b.CustomDeadLetterQueue));
				code.AppendLine(string.Format("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), b.DeadLetterQueue)));
				code.AppendLine(string.Format("\t\t\tthis.Durable = {0};", b.Durable ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ExactlyOnce = {0};", b.ExactlyOnce ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxRetryCycles = {0};", b.MaxRetryCycles));
				code.AppendLine(string.Format("\t\t\tthis.ReceiveContextEnabled = {0};", b.ReceiveContextEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), b.ReceiveErrorHandling)));
				code.AppendLine(string.Format("\t\t\tthis.ReceiveRetryCount = {0};", b.ReceiveRetryCount));
				code.AppendLine(string.Format("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});", b.RetryCycleDelay.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.TimeToLive = new TimeSpan({0});", b.TimeToLive.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.UseMSMQTracing = {0};", b.UseMSMQTracing ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.UseSourceJournal = {0};", b.UseSourceJournal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				if (b.ValidityDuration != TimeSpan.Zero) code.AppendLine(string.Format("\t\t\tthis.ValidityDuration = new TimeSpan({0});", b.ValidityDuration.Ticks));
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			code.AppendLine("\t\t}");
			code.AppendLine("\t}");
			if (o.Parent.Owner.EnableDocumentationWarnings) code.AppendLine("#pragma warning enable 1591");
			return code.ToString();
		}

		public static string GenerateCode40(ServiceBinding o)
		{
			return GenerateCode45(o);
		}

		public static string GenerateCode45(ServiceBinding o)
		{
			Type t = o.GetType();
			var code = new StringBuilder();
			if (o.Parent.Owner.EnableDocumentationWarnings) code.AppendLine("#pragma warning disable 1591");
			if (o.Documentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0}", DataTypeCSGenerator.GenerateTypeDeclaration(o)));
			code.AppendLine("\t{");
			code.AppendLine(string.Format("\t\tpublic {0}()", o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t}");
			if (t == typeof(ServiceBindingBasicHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingBasicHTTPS)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpsSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingNetHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingNetHTTPS)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpsSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingWSHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingWS2007HTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingWSDualHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSDualHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingWSFederationHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSFederationHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingWS2007FederationHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSFederationHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingTCP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.NetTcpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingNamedPipe)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.NetNamedPipeSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingMSMQ)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.NetMsmqSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingPeerTCP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.PeerSecuritySettings CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingWebHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WebHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingMSMQIntegration)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity CustomSecurity)", o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			code.AppendLine("\t\t}");
			code.AppendLine(string.Format("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			code.AppendLine("\t\t}");
			if (t == typeof(ServiceBindingBasicHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingBasicHTTPS)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpsSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingNetHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingNetHTTPS)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpsSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingWSHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingWS2007HTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingWSDualHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSDualHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingWSFederationHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSFederationHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingWS2007FederationHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WSFederationHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingTCP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.NetTcpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingNamedPipe)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.NetNamedPipeSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingMSMQ)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.NetMsmqSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingPeerTCP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.PeerSecuritySettings CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingWebHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.WebHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingMSMQIntegration)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			code.AppendLine("\t\t}");
			code.AppendLine("\t\tprivate void SetDefaults()");
			code.AppendLine("\t\t{");
			// Generic Binding code.
			code.AppendLine(string.Format("\t\t\tthis.Name = \"{0}\";", o.Name));
			code.AppendLine(string.Format("\t\t\tthis.Namespace = \"{0}\";", o.Namespace));
			code.AppendLine(string.Format("\t\t\tthis.OpenTimeout = new TimeSpan({0});", o.OpenTimeout.Ticks));
			code.AppendLine(string.Format("\t\t\tthis.CloseTimeout = new TimeSpan({0});", o.CloseTimeout.Ticks));
			code.AppendLine(string.Format("\t\t\tthis.SendTimeout = new TimeSpan({0});", o.SendTimeout.Ticks));
			code.AppendLine(string.Format("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});", o.ReceiveTimeout.Ticks));
			// Binding Specific code.
			if (t == typeof(ServiceBindingBasicHTTP))
			{
				var b = o as ServiceBindingBasicHTTP;
				if (b == null) return "";
				if (Globals.CurrentGenerationTarget != ProjectGenerationFramework.WIN8)
				{
					code.AppendLine(string.Format("\t\t\tthis.AllowCookies = {0};", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
					code.AppendLine(string.Format("\t\t\tthis.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
					code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
					code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
					code.AppendLine(string.Format("\t\t\tthis.MaxBufferSize = {0};", Convert.ToInt32(b.MaxBufferSize)));
					code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
					code.AppendLine(string.Format("\t\t\tthis.MessageEncoding = WSMessageEncoding.{0};", System.Enum.GetName(typeof (System.ServiceModel.WSMessageEncoding), b.MessageEncoding)));
					code.AppendLine(string.Format("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof (ServiceBindingTextEncoding), b.TextEncoding)));
					code.AppendLine(string.Format("\t\t\tthis.TransferMode = TransferMode.{0};", System.Enum.GetName(typeof (System.ServiceModel.TransferMode), b.TransferMode)));
					if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
					{
						code.AppendLine(string.Format("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
						code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
					}
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			if (t == typeof(ServiceBindingBasicHTTPS))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingBasicHTTPS;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.AllowCookies = {0};", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferSize = {0};", Convert.ToInt32(b.MaxBufferSize)));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.MessageEncoding = WSMessageEncoding.{0};", System.Enum.GetName(typeof (System.ServiceModel.WSMessageEncoding), b.MessageEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof (ServiceBindingTextEncoding), b.TextEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.TransferMode = TransferMode.{0};", System.Enum.GetName(typeof (System.ServiceModel.TransferMode), b.TransferMode)));
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine(string.Format("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingNetHTTP))
			{
				var b = o as ServiceBindingNetHTTP;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.AllowCookies = {0};", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferSize = {0};", Convert.ToInt32(b.MaxBufferSize)));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.MessageEncoding = NetHttpMessageEncoding.{0};", System.Enum.GetName(typeof(System.ServiceModel.NetHttpMessageEncoding), b.MessageEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.TransferMode = TransferMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode)));
				code.AppendLine(string.Format("\t\t\tthis.WebSocketSettings.DisablePayloadMasking = {0};", b.DisablePayloadMasking ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.WebSocketSettings.KeepAliveInterval = new TimeSpan({0});", b.KeepAliveInterval.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.WebSocketSettings.SubProtocol = \"{0}\";", b.SubProtocol));
				code.AppendLine(string.Format("\t\t\tthis.WebSocketSettings.TransportUsage = System.ServiceModel.Channels.WebSocketTransportUsage.{0};", System.Enum.GetName(typeof(System.ServiceModel.Channels.WebSocketTransportUsage), b.TransportUsage)));
				if (Globals.CurrentGenerationTarget != ProjectGenerationFramework.WIN8)
				{
					code.AppendLine(string.Format("\t\t\tthis.WebSocketSettings.CreateNotificationOnConnection = {0};", b.CreateNotificationOnConnection ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
					code.AppendLine(string.Format("\t\t\tthis.WebSocketSettings.MaxPendingConnections = {0};", b.MaxPendingConnections));
					code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Enabled = {0};", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
					code.AppendLine(string.Format("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});", b.ReliableSessionInactivityTimeout.Ticks));
					code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Ordered = {0};", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
					code.AppendLine(string.Format("\t\t\tthis.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
					if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
					{
						code.AppendLine(string.Format("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
						code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
					}
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingNetHTTPS))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingNetHTTPS;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.AllowCookies = {0};", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferSize = {0};", Convert.ToInt32(b.MaxBufferSize)));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.MessageEncoding = NetHttpMessageEncoding.{0};", System.Enum.GetName(typeof(System.ServiceModel.NetHttpMessageEncoding), b.MessageEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Enabled = {0};", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});", b.ReliableSessionInactivityTimeout.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Ordered = {0};", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.TransferMode = TransferMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode)));
				code.AppendLine(string.Format("\t\t\tthis.WebSocketSettings.CreateNotificationOnConnection = {0};", b.CreateNotificationOnConnection ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.WebSocketSettings.DisablePayloadMasking = {0};", b.DisablePayloadMasking ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.WebSocketSettings.KeepAliveInterval = new TimeSpan({0});", b.KeepAliveInterval.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.WebSocketSettings.MaxPendingConnections = {0};", b.MaxPendingConnections));
				code.AppendLine(string.Format("\t\t\tthis.WebSocketSettings.SubProtocol = \"{0}\";", b.SubProtocol));
				code.AppendLine(string.Format("\t\t\tthis.WebSocketSettings.TransportUsage = System.ServiceModel.Channels.WebSocketTransportUsage.{0};", System.Enum.GetName(typeof(System.ServiceModel.Channels.WebSocketTransportUsage), b.TransportUsage)));
				code.AppendLine(string.Format("\t\t\tthis.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine(string.Format("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingWSHTTP))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingWSHTTP;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.AllowCookies = {0};", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Enabled = {0};", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});", b.ReliableSessionInactivityTimeout.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Ordered = {0};", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.TransactionFlow = {0};", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine(string.Format("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingWS2007HTTP))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingWS2007HTTP;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.AllowCookies = {0};", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Enabled = {0};", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});", b.ReliableSessionInactivityTimeout.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Ordered = {0};", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.TransactionFlow = {0};", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine(string.Format("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingWSDualHTTP))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingWSDualHTTP;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				if (!string.IsNullOrEmpty(b.ClientBaseAddress)) code.AppendLine(string.Format("\t\t\tthis.ClientBaseAddress = new Uri(\"{0}\");", b.ClientBaseAddress));
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});", b.ReliableSessionInactivityTimeout.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Ordered = {0};", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.TransactionFlow = {0};", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine(string.Format("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingWSFederationHTTP))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingWSFederationHTTP;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				if (!string.IsNullOrEmpty(b.PrivacyNoticeAt))
				{
					code.AppendLine(string.Format("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");", b.PrivacyNoticeAt));
					code.AppendLine(string.Format("\t\t\tthis.PrivacyNoticeVersion = {0};", b.PrivacyNoticeVersion));
				}
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Enabled = {0};", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});", b.ReliableSessionInactivityTimeout.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Ordered = {0};", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.TransactionFlow = {0};", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine(string.Format("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingWS2007FederationHTTP))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingWS2007FederationHTTP;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				if (!string.IsNullOrEmpty(b.PrivacyNoticeAt))
				{
					code.AppendLine(string.Format("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");", b.PrivacyNoticeAt));
					code.AppendLine(string.Format("\t\t\tthis.PrivacyNoticeVersion = {0};", b.PrivacyNoticeVersion));
				}
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Enabled = {0};", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});", b.ReliableSessionInactivityTimeout.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Ordered = {0};", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding)));
				code.AppendLine(string.Format("\t\t\tthis.TransactionFlow = {0};", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine(string.Format("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingTCP))
			{
				var b = o as ServiceBindingTCP;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferSize = {0};", Convert.ToInt32(b.MaxBufferSize)));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.TransferMode = TransferMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode)));
				if (Globals.CurrentGenerationTarget != ProjectGenerationFramework.WIN8)
				{
					code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
					code.AppendLine(string.Format("\t\t\tthis.ListenBacklog = {0};", b.ListenBacklog));
					code.AppendLine(string.Format("\t\t\tthis.MaxConnections = {0};", b.MaxConnections));
					code.AppendLine(string.Format("\t\t\tthis.PortSharingEnabled = {0};", b.PortSharingEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
					code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Enabled = {0};", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
					code.AppendLine(string.Format("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});", b.ReliableSessionInactivityTimeout.Ticks));
					code.AppendLine(string.Format("\t\t\tthis.ReliableSession.Ordered = {0};", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
					code.AppendLine(string.Format("\t\t\tthis.TransactionFlow = {0};", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
					code.AppendLine(string.Format("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};", System.Enum.GetName(typeof (ServiceBindingTransactionProtocol), b.TransactionProtocol)));
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingNamedPipe))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingNamedPipe;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferSize = {0};", Convert.ToInt32(b.MaxBufferSize)));
				code.AppendLine(string.Format("\t\t\tthis.MaxConnections = {0};", b.MaxConnections));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.TransactionFlow = {0};", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};", System.Enum.GetName(typeof(ServiceBindingTransactionProtocol), b.TransactionProtocol)));
				code.AppendLine(string.Format("\t\t\tthis.TransferMode = TransferMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode)));
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingMSMQ))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingMSMQ;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");", b.CustomDeadLetterQueue));
				code.AppendLine(string.Format("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), b.DeadLetterQueue)));
				code.AppendLine(string.Format("\t\t\tthis.Durable = {0};", b.Durable ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ExactlyOnce = {0};", b.ExactlyOnce ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxRetryCycles = {0};", b.MaxRetryCycles));
				code.AppendLine(string.Format("\t\t\tthis.QueueTransferProtocol = QueueTransferProtocol.{0};", System.Enum.GetName(typeof(System.ServiceModel.QueueTransferProtocol), b.QueueTransferProtocol)));
				code.AppendLine(string.Format("\t\t\tthis.ReceiveContextEnabled = {0};", b.ReceiveContextEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), b.ReceiveErrorHandling)));
				code.AppendLine(string.Format("\t\t\tthis.ReceiveRetryCount = {0};", b.ReceiveRetryCount));
				code.AppendLine(string.Format("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});", b.RetryCycleDelay.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.TimeToLive = new TimeSpan({0});", b.TimeToLive.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.UseActiveDirectory = {0};", b.UseActiveDirectory ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.UseMSMQTracing = {0};", b.UseMSMQTracing ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.UseSourceJournal = {0};", b.UseSourceJournal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				if (b.ValidityDuration != TimeSpan.Zero) code.AppendLine(string.Format("\t\t\tthis.ValidityDuration = new TimeSpan({0});", b.ValidityDuration.Ticks));
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingPeerTCP))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingPeerTCP;
				if (b == null) return "";
				code.AppendLine(b.ListenIPAddress == "" ? "\t\t\tthis.ListenIPAddress = null;" : string.Format("\t\t\tthis.ListenIPAddress = IPAddress.Parse(\"{0}\");", b.ListenIPAddress));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.Port = {0};", b.Port));
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingWebHTTP))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingWebHTTP;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.AllowCookies = {0};", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.BypassProxyOnLocal = {0};", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.CrossDomainScriptAccessEnabled = {0};", b.CrossDomainScriptAccessEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferPoolSize = {0};", b.MaxBufferPoolSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxBufferSize = {0};", Convert.ToInt32(b.MaxBufferSize)));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.TransferMode = TransferMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode)));
				code.AppendLine(string.Format("\t\t\tthis.WriteEncoding = System.Text.Encoding.{0};", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.WriteEncoding)));
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine(string.Format("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");", b.ProxyAddress));
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingMSMQIntegration))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingMSMQIntegration;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");", b.CustomDeadLetterQueue));
				code.AppendLine(string.Format("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), b.DeadLetterQueue)));
				code.AppendLine(string.Format("\t\t\tthis.Durable = {0};", b.Durable ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ExactlyOnce = {0};", b.ExactlyOnce ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.MaxReceivedMessageSize = {0};", b.MaxReceivedMessageSize));
				code.AppendLine(string.Format("\t\t\tthis.MaxRetryCycles = {0};", b.MaxRetryCycles));
				code.AppendLine(string.Format("\t\t\tthis.ReceiveContextEnabled = {0};", b.ReceiveContextEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), b.ReceiveErrorHandling)));
				code.AppendLine(string.Format("\t\t\tthis.ReceiveRetryCount = {0};", b.ReceiveRetryCount));
				code.AppendLine(string.Format("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});", b.RetryCycleDelay.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.TimeToLive = new TimeSpan({0});", b.TimeToLive.Ticks));
				code.AppendLine(string.Format("\t\t\tthis.UseMSMQTracing = {0};", b.UseMSMQTracing ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				code.AppendLine(string.Format("\t\t\tthis.UseSourceJournal = {0};", b.UseSourceJournal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
				if (b.ValidityDuration != TimeSpan.Zero) code.AppendLine(string.Format("\t\t\tthis.ValidityDuration = new TimeSpan({0});", b.ValidityDuration.Ticks));
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			code.AppendLine("\t\t}");
			code.AppendLine("\t}");
			if (o.Parent.Owner.EnableDocumentationWarnings) code.AppendLine("#pragma warning enable 1591");
			return code.ToString();
		}
		public static string GenerateCode35Client(ServiceBinding o)
		{
			return GenerateCode35(o);
		}
		public static string GenerateCode40Client(ServiceBinding o)
		{
			Type t = o.GetType();
			return t == typeof(ServiceBindingWebHTTP) ? "" : GenerateCode40(o);
		}

		public static string GenerateBindingBaseType(ServiceBinding o)
		{
			return o.InheritedTypes.Count <= 0 ? "" : DataTypeCSGenerator.GenerateType(o.InheritedTypes[0]);
		}
	}
}