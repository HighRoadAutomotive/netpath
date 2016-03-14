using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETPath.Projects;
using NETPath.Projects.Wcf;
using NETPath.Projects.Helpers;

namespace NETPath.Generators.CS.Wcf
{
	internal static class BindingsGenerator
	{
		public static void VerifyCode(WcfBinding o, Action<CompileMessage> AddMessage)
		{
			if (string.IsNullOrEmpty(o.Name))
				AddMessage(new CompileMessage("GS6000", "A binding in the '" + o.Parent.Name + "' project has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));
			else
				if (RegExs.MatchCodeName.IsMatch(o.Name) == false)
				AddMessage(new CompileMessage("GS6001", "The binding '" + o.Name + "' in the '" + o.Parent.Name + "' project contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));
			if (string.IsNullOrEmpty(o.Namespace)) { }
			else
				if (RegExs.MatchHttpUri.IsMatch(o.Namespace) == false)
				AddMessage(new CompileMessage("GS6002", "The Namespace '" + o.Namespace + "' for the '" + o.Name + "' binding in the '" + o.Parent.Name + "' project is not a valid Uri.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType()));

			Type t = o.GetType();
			if (t == typeof(WcfBindingBasicHTTP))
			{
				var b = o as WcfBindingBasicHTTP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6003", "The Security for the '" + b.Name + "' Basic HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHttpUri.IsMatch(b.ProxyAddress) == false)
						AddMessage(new CompileMessage("GS6004", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' Basic HTTP Binding in the '" + b.Parent.Name + "' project is not a valid Uri.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(WcfBindingBasicHTTPS) || t == typeof(WcfBindingNetHTTP) || t == typeof(WcfBindingNetHTTPS))
			{
				var b = o as WcfBindingNetHTTP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6003", "The Security for the '" + b.Name + "' Net HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHttpUri.IsMatch(b.ProxyAddress) == false)
						AddMessage(new CompileMessage("GS6004", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' Net HTTP Binding in the '" + b.Parent.Name + "' project is not a valid Uri.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(WcfBindingWSHTTP))
			{
				var b = o as WcfBindingWSHTTP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6005", "The Security for the '" + b.Name + "' WS HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHttpUri.IsMatch(b.ProxyAddress) == false)
						AddMessage(new CompileMessage("GS6006", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS HTTP Binding in the '" + b.Parent.Name + "' project is not a valid Uri.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(WcfBindingWS2007HTTP))
			{
				var b = o as WcfBindingWS2007HTTP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6007", "The Security for the '" + b.Name + "' WS 2007 HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHttpUri.IsMatch(b.ProxyAddress) == false)
						AddMessage(new CompileMessage("GS6008", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS 2007 HTTP Binding in the '" + b.Parent.Name + "' project is not a valid Uri.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(WcfBindingWSDualHTTP))
			{
				var b = o as WcfBindingWSDualHTTP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6009", "The Security for the '" + b.Name + "' WS Dual Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHttpUri.IsMatch(b.ProxyAddress) == false)
						AddMessage(new CompileMessage("GS6010", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS Dual HTTP Binding in the '" + b.Parent.Name + "' project is not a valid Uri.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(WcfBindingWSFederationHTTP))
			{
				var b = o as WcfBindingWSFederationHTTP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6011", "The Security for the '" + b.Name + "' WS Federation HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHttpUri.IsMatch(b.ProxyAddress) == false)
						AddMessage(new CompileMessage("GS6012", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS Federation HTTP Binding in the '" + b.Parent.Name + "' project is not a valid Uri.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(WcfBindingWS2007FederationHTTP))
			{
				var b = o as WcfBindingWS2007FederationHTTP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6013", "The Security for the '" + b.Name + "' WS 2007 Federation HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHttpUri.IsMatch(b.ProxyAddress) == false)
						AddMessage(new CompileMessage("GS6014", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS 2007 Federation HTTP Binding in the '" + b.Parent.Name + "' project is not a valid Uri.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(WcfBindingTCP))
			{
				var b = o as WcfBindingTCP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6015", "The Security for the '" + b.Name + "' TCP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(WcfBindingNamedPipe))
			{
				var b = o as WcfBindingNamedPipe;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6016", "The Security for the '" + b.Name + "' Named Pipe Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(WcfBindingMSMQ))
			{
				var b = o as WcfBindingMSMQ;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6017", "The Security for the '" + b.Name + "' MSMQ Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(WcfBindingPeerTCP))
			{
				var b = o as WcfBindingPeerTCP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6018", "The Security for the '" + b.Name + "' Peer TCP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
				if (b != null && !string.IsNullOrEmpty(b.ListenIPAddress))
					if (RegExs.MatchIPv4.IsMatch(b.ListenIPAddress) == false && RegExs.MatchIPv6.IsMatch(b.ListenIPAddress) == false)
						AddMessage(new CompileMessage("GS6019", "The Listen IP Address for the '" + b.Name + "' Peer TCP Binding in the '" + b.Parent.Name + "' project is not valid.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(WcfBindingWebHTTP))
			{
				var b = o as WcfBindingWebHTTP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6020", "The Security for the '" + b.Name + "' MSMQ Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHttpUri.IsMatch(b.ProxyAddress) == false)
						AddMessage(new CompileMessage("GS6021", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' Basic HTTP Binding in the '" + b.Parent.Name + "' project is not a valid Uri.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(WcfBindingMSMQIntegration))
			{
				var b = o as WcfBindingMSMQIntegration;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6021", "The Security for the '" + b.Name + "' MSMQ Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
			}
		}

		public static string GenerateCodeRT8(WcfBinding o)
		{
			Type t = o.GetType();
			if (t == typeof(WcfBindingBasicHTTP)) return GenerateCode45(o, ProjectGenerationFramework.WINRT);
			if (t == typeof(WcfBindingNetHTTP)) return GenerateCode45(o, ProjectGenerationFramework.WINRT);
			if (t == typeof(WcfBindingTCP)) return GenerateCode45(o, ProjectGenerationFramework.WINRT);
			return "";
		}

		public static string GenerateCode45(WcfBinding o, ProjectGenerationFramework GenerationTarget)
		{
			Type t = o.GetType();
			var code = new StringBuilder();
			if (o.Parent.Owner.EnableDocumentationWarnings) code.AppendLine("#pragma warning disable 1591");
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.AppendLine($"\t[System.CodeDom.Compiler.GeneratedCode(\"{Globals.ApplicationTitle}\", \"{Globals.ApplicationVersion}\")]");
			code.AppendLine($"\t{DataTypeGenerator.GenerateTypeDeclaration(o)}");
			code.AppendLine("\t{");
			code.AppendLine($"\t\tpublic {o.Name}()");
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t}");
			if (t == typeof(WcfBindingBasicHTTP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.BasicHttpSecurity CustomSecurity)");
			if (t == typeof(WcfBindingBasicHTTPS)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.BasicHttpsSecurity CustomSecurity)");
			if (t == typeof(WcfBindingNetHTTP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.BasicHttpSecurity CustomSecurity)");
			if (t == typeof(WcfBindingNetHTTPS)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.BasicHttpsSecurity CustomSecurity)");
			if (t == typeof(WcfBindingWSHTTP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.WSHttpSecurity CustomSecurity)");
			if (t == typeof(WcfBindingWS2007HTTP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.WSHttpSecurity CustomSecurity)");
			if (t == typeof(WcfBindingWSDualHTTP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.WSDualHttpSecurity CustomSecurity)");
			if (t == typeof(WcfBindingWSFederationHTTP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.WSFederationHttpSecurity CustomSecurity)");
			if (t == typeof(WcfBindingWS2007FederationHTTP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.WSFederationHttpSecurity CustomSecurity)");
			if (t == typeof(WcfBindingTCP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.NetTcpSecurity CustomSecurity)");
			if (t == typeof(WcfBindingNamedPipe)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.NetNamedPipeSecurity CustomSecurity)");
			if (t == typeof(WcfBindingMSMQ)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.NetMsmqSecurity CustomSecurity)");
			if (t == typeof(WcfBindingPeerTCP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.PeerSecuritySettings CustomSecurity)");
			if (t == typeof(WcfBindingWebHTTP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.WebHttpSecurity CustomSecurity)");
			if (t == typeof(WcfBindingMSMQIntegration)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity CustomSecurity)");
			if (t != typeof(WcfBindingUDP))
			{
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tSetDefaults();");
				code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
				code.AppendLine("\t\t}");
			}
			code.AppendLine($"\t\tpublic {o.Name}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)");
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			code.AppendLine("\t\t}");
			if (t == typeof(WcfBindingBasicHTTP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.BasicHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)");
			if (t == typeof(WcfBindingBasicHTTPS)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.BasicHttpsSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)");
			if (t == typeof(WcfBindingNetHTTP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.BasicHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)");
			if (t == typeof(WcfBindingNetHTTPS)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.BasicHttpsSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)");
			if (t == typeof(WcfBindingWSHTTP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.WSHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)");
			if (t == typeof(WcfBindingWS2007HTTP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.WSHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)");
			if (t == typeof(WcfBindingWSDualHTTP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.WSDualHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)");
			if (t == typeof(WcfBindingWSFederationHTTP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.WSFederationHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)");
			if (t == typeof(WcfBindingWS2007FederationHTTP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.WSFederationHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)");
			if (t == typeof(WcfBindingTCP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.NetTcpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)");
			if (t == typeof(WcfBindingNamedPipe)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.NetNamedPipeSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)");
			if (t == typeof(WcfBindingMSMQ)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.NetMsmqSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)");
			if (t == typeof(WcfBindingPeerTCP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.PeerSecuritySettings CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)");
			if (t == typeof(WcfBindingWebHTTP)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.WebHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)");
			if (t == typeof(WcfBindingMSMQIntegration)) code.AppendLine($"\t\tpublic {o.Name}(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)");
			if (t != typeof(WcfBindingUDP))
			{
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tSetDefaults();");
				code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
				code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
				code.AppendLine("\t\t}");
			}
			code.AppendLine("\t\tprivate void SetDefaults()");
			code.AppendLine("\t\t{");
			// Generic Binding code.
			code.AppendLine($"\t\t\tthis.Name = \"{o.Name}\";");
			code.AppendLine($"\t\t\tthis.Namespace = \"{o.Namespace}\";");
			code.AppendLine($"\t\t\tthis.OpenTimeout = new TimeSpan({o.OpenTimeout.Ticks});");
			code.AppendLine($"\t\t\tthis.CloseTimeout = new TimeSpan({o.CloseTimeout.Ticks});");
			code.AppendLine($"\t\t\tthis.SendTimeout = new TimeSpan({o.SendTimeout.Ticks});");
			code.AppendLine($"\t\t\tthis.ReceiveTimeout = new TimeSpan({o.ReceiveTimeout.Ticks});");
			// Binding Specific code.
			if (t == typeof(WcfBindingBasicHTTP))
			{
				var b = o as WcfBindingBasicHTTP;
				if (b == null) return "";
				//TODO: Verify WinRT can connect with these set.
				if (GenerationTarget != ProjectGenerationFramework.WINRT)
				{
					code.AppendLine($"\t\t\tthis.AllowCookies = {(b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
					code.AppendLine($"\t\t\tthis.BypassProxyOnLocal = {(b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
					code.AppendLine($"\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)};");
					code.AppendLine($"\t\t\tthis.MaxBufferPoolSize = {b.MaxBufferPoolSize};");
					code.AppendLine($"\t\t\tthis.MaxBufferSize = {Convert.ToInt32(b.MaxBufferSize)};");
					code.AppendLine($"\t\t\tthis.MaxReceivedMessageSize = {b.MaxReceivedMessageSize};");
					code.AppendLine($"\t\t\tthis.MessageEncoding = WSMessageEncoding.{System.Enum.GetName(typeof (System.ServiceModel.WSMessageEncoding), b.MessageEncoding)};");
					code.AppendLine($"\t\t\tthis.TextEncoding = System.Text.Encoding.{System.Enum.GetName(typeof (TextEncoding), b.TextEncoding)};");
					code.AppendLine($"\t\t\tthis.TransferMode = TransferMode.{System.Enum.GetName(typeof (System.ServiceModel.TransferMode), b.TransferMode)};");
					if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
					{
						code.AppendLine($"\t\t\tthis.ProxyAddress = new Uri(\"{b.ProxyAddress}\");");
						code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
					}
					if (b.UseContextBinding)
					{
						code.AppendLine($"\t\t\tthis.ContextManagementEnabled = {(b.ContextManagementEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
					}
				}
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
			}
			if (t == typeof(WcfBindingBasicHTTPS))
			{
				var b = o as WcfBindingBasicHTTPS;
				if (b == null) return "";
				code.AppendLine($"\t\t\tthis.AllowCookies = {(b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.BypassProxyOnLocal = {(b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)};");
				code.AppendLine($"\t\t\tthis.MaxBufferPoolSize = {b.MaxBufferPoolSize};");
				code.AppendLine($"\t\t\tthis.MaxBufferSize = {Convert.ToInt32(b.MaxBufferSize)};");
				code.AppendLine($"\t\t\tthis.MaxReceivedMessageSize = {b.MaxReceivedMessageSize};");
				code.AppendLine($"\t\t\tthis.MessageEncoding = WSMessageEncoding.{System.Enum.GetName(typeof (System.ServiceModel.WSMessageEncoding), b.MessageEncoding)};");
				code.AppendLine($"\t\t\tthis.TextEncoding = System.Text.Encoding.{System.Enum.GetName(typeof (TextEncoding), b.TextEncoding)};");
				code.AppendLine($"\t\t\tthis.TransferMode = TransferMode.{System.Enum.GetName(typeof (System.ServiceModel.TransferMode), b.TransferMode)};");
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine($"\t\t\tthis.ProxyAddress = new Uri(\"{b.ProxyAddress}\");");
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(WcfBindingNetHTTP))
			{
				var b = o as WcfBindingNetHTTP;
				if (b == null) return "";
				code.AppendLine($"\t\t\tthis.AllowCookies = {(b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.MaxBufferPoolSize = {b.MaxBufferPoolSize};");
				code.AppendLine($"\t\t\tthis.MaxBufferSize = {Convert.ToInt32(b.MaxBufferSize)};");
				code.AppendLine($"\t\t\tthis.MaxReceivedMessageSize = {b.MaxReceivedMessageSize};");
				code.AppendLine($"\t\t\tthis.MessageEncoding = NetHttpMessageEncoding.{System.Enum.GetName(typeof (System.ServiceModel.NetHttpMessageEncoding), b.MessageEncoding)};");
				code.AppendLine($"\t\t\tthis.TextEncoding = System.Text.Encoding.{System.Enum.GetName(typeof (TextEncoding), b.TextEncoding)};");
				code.AppendLine($"\t\t\tthis.TransferMode = TransferMode.{System.Enum.GetName(typeof (System.ServiceModel.TransferMode), b.TransferMode)};");
				code.AppendLine($"\t\t\tthis.WebSocketSettings.DisablePayloadMasking = {(b.DisablePayloadMasking ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.WebSocketSettings.KeepAliveInterval = new TimeSpan({b.KeepAliveInterval.Ticks});");
				code.AppendLine($"\t\t\tthis.WebSocketSettings.SubProtocol = {(string.IsNullOrWhiteSpace(b.SubProtocol) ? "null" : $"\"{b.SubProtocol}\"")};");
				code.AppendLine($"\t\t\tthis.WebSocketSettings.TransportUsage = System.ServiceModel.Channels.WebSocketTransportUsage.{System.Enum.GetName(typeof (System.ServiceModel.Channels.WebSocketTransportUsage), b.TransportUsage)};");
				//TODO: Verify WinRT can connect with these set.
				if (GenerationTarget != ProjectGenerationFramework.WINRT)
				{
					code.AppendLine($"\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)};");
					code.AppendLine($"\t\t\tthis.WebSocketSettings.CreateNotificationOnConnection = {(b.CreateNotificationOnConnection ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
					code.AppendLine($"\t\t\tthis.WebSocketSettings.MaxPendingConnections = {b.MaxPendingConnections};");
					code.AppendLine($"\t\t\tthis.ReliableSession.Enabled = {(b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
					code.AppendLine($"\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({b.ReliableSessionInactivityTimeout.Ticks});");
					code.AppendLine($"\t\t\tthis.ReliableSession.Ordered = {(b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
					code.AppendLine($"\t\t\tthis.BypassProxyOnLocal = {(b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
					if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
					{
						code.AppendLine($"\t\t\tthis.ProxyAddress = new Uri(\"{b.ProxyAddress}\");");
						code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
					}
				}
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(WcfBindingNetHTTPS))
			{
				var b = o as WcfBindingNetHTTPS;
				if (b == null) return "";
				code.AppendLine($"\t\t\tthis.AllowCookies = {(b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)};");
				code.AppendLine($"\t\t\tthis.MaxBufferPoolSize = {b.MaxBufferPoolSize};");
				code.AppendLine($"\t\t\tthis.MaxBufferSize = {Convert.ToInt32(b.MaxBufferSize)};");
				code.AppendLine($"\t\t\tthis.MaxReceivedMessageSize = {b.MaxReceivedMessageSize};");
				code.AppendLine($"\t\t\tthis.MessageEncoding = NetHttpMessageEncoding.{System.Enum.GetName(typeof (System.ServiceModel.NetHttpMessageEncoding), b.MessageEncoding)};");
				code.AppendLine($"\t\t\tthis.ReliableSession.Enabled = {(b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({b.ReliableSessionInactivityTimeout.Ticks});");
				code.AppendLine($"\t\t\tthis.ReliableSession.Ordered = {(b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.TextEncoding = System.Text.Encoding.{System.Enum.GetName(typeof (TextEncoding), b.TextEncoding)};");
				code.AppendLine($"\t\t\tthis.TransferMode = TransferMode.{System.Enum.GetName(typeof (System.ServiceModel.TransferMode), b.TransferMode)};");
				code.AppendLine($"\t\t\tthis.WebSocketSettings.CreateNotificationOnConnection = {(b.CreateNotificationOnConnection ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.WebSocketSettings.DisablePayloadMasking = {(b.DisablePayloadMasking ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.WebSocketSettings.KeepAliveInterval = new TimeSpan({b.KeepAliveInterval.Ticks});");
				code.AppendLine($"\t\t\tthis.WebSocketSettings.MaxPendingConnections = {b.MaxPendingConnections};");
				code.AppendLine($"\t\t\tthis.WebSocketSettings.SubProtocol = {(string.IsNullOrWhiteSpace(b.SubProtocol) ? "null" : $"\"{b.SubProtocol}\"")};");
				code.AppendLine($"\t\t\tthis.WebSocketSettings.TransportUsage = System.ServiceModel.Channels.WebSocketTransportUsage.{System.Enum.GetName(typeof (System.ServiceModel.Channels.WebSocketTransportUsage), b.TransportUsage)};");
				code.AppendLine($"\t\t\tthis.BypassProxyOnLocal = {(b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine($"\t\t\tthis.ProxyAddress = new Uri(\"{b.ProxyAddress}\");");
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(WcfBindingWSHTTP))
			{
				var b = o as WcfBindingWSHTTP;
				if (b == null) return "";
				code.AppendLine($"\t\t\tthis.AllowCookies = {(b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.BypassProxyOnLocal = {(b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)};");
				code.AppendLine($"\t\t\tthis.MaxBufferPoolSize = {b.MaxBufferPoolSize};");
				code.AppendLine($"\t\t\tthis.MaxReceivedMessageSize = {b.MaxReceivedMessageSize};");
				code.AppendLine($"\t\t\tthis.ReliableSession.Enabled = {(b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({b.ReliableSessionInactivityTimeout.Ticks});");
				code.AppendLine($"\t\t\tthis.ReliableSession.Ordered = {(b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.TextEncoding = System.Text.Encoding.{System.Enum.GetName(typeof (TextEncoding), b.TextEncoding)};");
				code.AppendLine($"\t\t\tthis.TransactionFlow = {(b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine($"\t\t\tthis.ProxyAddress = new Uri(\"{b.ProxyAddress}\");");
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.UseContextBinding)
				{
					code.AppendLine($"\t\t\tthis.ContextManagementEnabled = {(b.ContextManagementEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
					code.AppendLine($"\t\t\tthis.ContextProtectionLevel = System.Net.Security.ProtectionLevel.{b.ContextProtectionLevel};");
				}
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(WcfBindingWS2007HTTP))
			{
				var b = o as WcfBindingWS2007HTTP;
				if (b == null) return "";
				code.AppendLine($"\t\t\tthis.AllowCookies = {(b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.BypassProxyOnLocal = {(b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)};");
				code.AppendLine($"\t\t\tthis.MaxBufferPoolSize = {b.MaxBufferPoolSize};");
				code.AppendLine($"\t\t\tthis.MaxReceivedMessageSize = {b.MaxReceivedMessageSize};");
				code.AppendLine($"\t\t\tthis.ReliableSession.Enabled = {(b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({b.ReliableSessionInactivityTimeout.Ticks});");
				code.AppendLine($"\t\t\tthis.ReliableSession.Ordered = {(b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.TextEncoding = System.Text.Encoding.{System.Enum.GetName(typeof (TextEncoding), b.TextEncoding)};");
				code.AppendLine($"\t\t\tthis.TransactionFlow = {(b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine($"\t\t\tthis.ProxyAddress = new Uri(\"{b.ProxyAddress}\");");
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(WcfBindingWSDualHTTP))
			{
				var b = o as WcfBindingWSDualHTTP;
				if (b == null) return "";
				code.AppendLine($"\t\t\tthis.BypassProxyOnLocal = {(b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				if (!string.IsNullOrEmpty(b.ClientBaseAddress)) code.AppendLine($"\t\t\tthis.ClientBaseAddress = new Uri(\"{b.ClientBaseAddress}\");");
				code.AppendLine($"\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)};");
				code.AppendLine($"\t\t\tthis.MaxBufferPoolSize = {b.MaxBufferPoolSize};");
				code.AppendLine($"\t\t\tthis.MaxReceivedMessageSize = {b.MaxReceivedMessageSize};");
				code.AppendLine($"\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({b.ReliableSessionInactivityTimeout.Ticks});");
				code.AppendLine($"\t\t\tthis.ReliableSession.Ordered = {(b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.TextEncoding = System.Text.Encoding.{System.Enum.GetName(typeof (TextEncoding), b.TextEncoding)};");
				code.AppendLine($"\t\t\tthis.TransactionFlow = {(b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine($"\t\t\tthis.ProxyAddress = new Uri(\"{b.ProxyAddress}\");");
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(WcfBindingWSFederationHTTP))
			{
				var b = o as WcfBindingWSFederationHTTP;
				if (b == null) return "";
				code.AppendLine($"\t\t\tthis.BypassProxyOnLocal = {(b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)};");
				code.AppendLine($"\t\t\tthis.MaxBufferPoolSize = {b.MaxBufferPoolSize};");
				code.AppendLine($"\t\t\tthis.MaxReceivedMessageSize = {b.MaxReceivedMessageSize};");
				if (!string.IsNullOrEmpty(b.PrivacyNoticeAt))
				{
					code.AppendLine($"\t\t\tthis.PrivacyNoticeAt = new Uri(\"{b.PrivacyNoticeAt}\");");
					code.AppendLine($"\t\t\tthis.PrivacyNoticeVersion = {b.PrivacyNoticeVersion};");
				}
				code.AppendLine($"\t\t\tthis.ReliableSession.Enabled = {(b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({b.ReliableSessionInactivityTimeout.Ticks});");
				code.AppendLine($"\t\t\tthis.ReliableSession.Ordered = {(b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.TextEncoding = System.Text.Encoding.{System.Enum.GetName(typeof (TextEncoding), b.TextEncoding)};");
				code.AppendLine($"\t\t\tthis.TransactionFlow = {(b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine($"\t\t\tthis.ProxyAddress = new Uri(\"{b.ProxyAddress}\");");
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(WcfBindingWS2007FederationHTTP))
			{
				var b = o as WcfBindingWS2007FederationHTTP;
				if (b == null) return "";
				code.AppendLine($"\t\t\tthis.BypassProxyOnLocal = {(b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)};");
				code.AppendLine($"\t\t\tthis.MaxBufferPoolSize = {b.MaxBufferPoolSize};");
				code.AppendLine($"\t\t\tthis.MaxReceivedMessageSize = {b.MaxReceivedMessageSize};");
				if (!string.IsNullOrEmpty(b.PrivacyNoticeAt))
				{
					code.AppendLine($"\t\t\tthis.PrivacyNoticeAt = new Uri(\"{b.PrivacyNoticeAt}\");");
					code.AppendLine($"\t\t\tthis.PrivacyNoticeVersion = {b.PrivacyNoticeVersion};");
				}
				code.AppendLine($"\t\t\tthis.ReliableSession.Enabled = {(b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({b.ReliableSessionInactivityTimeout.Ticks});");
				code.AppendLine($"\t\t\tthis.ReliableSession.Ordered = {(b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.TextEncoding = System.Text.Encoding.{System.Enum.GetName(typeof (TextEncoding), b.TextEncoding)};");
				code.AppendLine($"\t\t\tthis.TransactionFlow = {(b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine($"\t\t\tthis.ProxyAddress = new Uri(\"{b.ProxyAddress}\");");
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(WcfBindingTCP))
			{
				var b = o as WcfBindingTCP;
				if (b == null) return "";
				code.AppendLine($"\t\t\tthis.MaxBufferPoolSize = {b.MaxBufferPoolSize};");
				code.AppendLine($"\t\t\tthis.MaxBufferSize = {Convert.ToInt32(b.MaxBufferSize)};");
				code.AppendLine($"\t\t\tthis.MaxReceivedMessageSize = {b.MaxReceivedMessageSize};");
				code.AppendLine($"\t\t\tthis.TransferMode = TransferMode.{System.Enum.GetName(typeof (System.ServiceModel.TransferMode), b.TransferMode)};");
				//TODO: Verify WinRT can connect with these set.
				if (GenerationTarget != ProjectGenerationFramework.WINRT)
				{
					code.AppendLine($"\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)};");
					code.AppendLine($"\t\t\tthis.ListenBacklog = {b.ListenBacklog};");
					code.AppendLine($"\t\t\tthis.MaxConnections = {b.MaxConnections};");
					code.AppendLine($"\t\t\tthis.PortSharingEnabled = {(b.PortSharingEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
					code.AppendLine($"\t\t\tthis.ReliableSession.Enabled = {(b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
					code.AppendLine($"\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({b.ReliableSessionInactivityTimeout.Ticks});");
					code.AppendLine($"\t\t\tthis.ReliableSession.Ordered = {(b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
					code.AppendLine($"\t\t\tthis.TransactionFlow = {(b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
					code.AppendLine($"\t\t\tthis.TransactionProtocol = TransactionProtocol.{System.Enum.GetName(typeof (WcfBindingTransactionProtocol), b.TransactionProtocol)};");
					if (b.UseContextBinding)
					{
						code.AppendLine($"\t\t\tthis.ContextManagementEnabled = {(b.ContextManagementEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
						code.AppendLine($"\t\t\tthis.ContextProtectionLevel = System.Net.Security.ProtectionLevel.{b.ContextProtectionLevel};");
					}
				}
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(WcfBindingUDP))
			{
				var b = o as WcfBindingUDP;
				if (b == null) return "";
				code.AppendLine($"\t\t\tthis.DuplicateMessageHistoryLength = {b.DuplicateMessageHistoryLength};");
				code.AppendLine($"\t\t\tthis.MaxBufferPoolSize = {b.MaxBufferPoolSize};");
				code.AppendLine($"\t\t\tthis.MaxPendingMessagesTotalSize = {b.MaxPendingMessagesTotalSize};");
				code.AppendLine($"\t\t\tthis.MaxReceivedMessageSize = {b.MaxReceivedMessageSize};");
				code.AppendLine($"\t\t\tthis.MaxRetransmitCount = {b.MaxRetransmitCount};");
				code.AppendLine($"\t\t\tthis.TextEncoding = System.Text.Encoding.{System.Enum.GetName(typeof (TextEncoding), b.TextEncoding)};");
				code.AppendLine($"\t\t\tthis.TimeToLive = {b.TimeToLive};");
			}
			else if (t == typeof(WcfBindingNamedPipe))
			{
				var b = o as WcfBindingNamedPipe;
				if (b == null) return "";
				code.AppendLine($"\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)};");
				code.AppendLine($"\t\t\tthis.MaxBufferPoolSize = {b.MaxBufferPoolSize};");
				code.AppendLine($"\t\t\tthis.MaxBufferSize = {Convert.ToInt32(b.MaxBufferSize)};");
				code.AppendLine($"\t\t\tthis.MaxConnections = {b.MaxConnections};");
				code.AppendLine($"\t\t\tthis.MaxReceivedMessageSize = {b.MaxReceivedMessageSize};");
				code.AppendLine($"\t\t\tthis.TransactionFlow = {(b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.TransactionProtocol = TransactionProtocol.{System.Enum.GetName(typeof (WcfBindingTransactionProtocol), b.TransactionProtocol)};");
				code.AppendLine($"\t\t\tthis.TransferMode = TransferMode.{System.Enum.GetName(typeof (System.ServiceModel.TransferMode), b.TransferMode)};");
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(WcfBindingMSMQ))
			{
				var b = o as WcfBindingMSMQ;
				if (b == null) return "";
				code.AppendLine($"\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{b.CustomDeadLetterQueue}\");");
				code.AppendLine($"\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{System.Enum.GetName(typeof (System.ServiceModel.DeadLetterQueue), b.DeadLetterQueue)};");
				code.AppendLine($"\t\t\tthis.Durable = {(b.Durable ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.ExactlyOnce = {(b.ExactlyOnce ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.MaxBufferPoolSize = {b.MaxBufferPoolSize};");
				code.AppendLine($"\t\t\tthis.MaxReceivedMessageSize = {b.MaxReceivedMessageSize};");
				code.AppendLine($"\t\t\tthis.MaxRetryCycles = {b.MaxRetryCycles};");
				code.AppendLine($"\t\t\tthis.QueueTransferProtocol = QueueTransferProtocol.{System.Enum.GetName(typeof (System.ServiceModel.QueueTransferProtocol), b.QueueTransferProtocol)};");
				code.AppendLine($"\t\t\tthis.ReceiveContextEnabled = {(b.ReceiveContextEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{System.Enum.GetName(typeof (System.ServiceModel.ReceiveErrorHandling), b.ReceiveErrorHandling)};");
				code.AppendLine($"\t\t\tthis.ReceiveRetryCount = {b.ReceiveRetryCount};");
				code.AppendLine($"\t\t\tthis.RetryCycleDelay = new TimeSpan({b.RetryCycleDelay.Ticks});");
				code.AppendLine($"\t\t\tthis.TimeToLive = new TimeSpan({b.TimeToLive.Ticks});");
				code.AppendLine($"\t\t\tthis.UseActiveDirectory = {(b.UseActiveDirectory ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.UseMSMQTracing = {(b.UseMSMQTracing ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.UseSourceJournal = {(b.UseSourceJournal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				if (b.ValidityDuration != TimeSpan.Zero) code.AppendLine($"\t\t\tthis.ValidityDuration = new TimeSpan({b.ValidityDuration.Ticks});");
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(WcfBindingPeerTCP))
			{
				var b = o as WcfBindingPeerTCP;
				if (b == null) return "";
				code.AppendLine(b.ListenIPAddress == "" ? "\t\t\tthis.ListenIPAddress = null;" : $"\t\t\tthis.ListenIPAddress = IPAddress.Parse(\"{b.ListenIPAddress}\");");
				code.AppendLine($"\t\t\tthis.MaxBufferPoolSize = {b.MaxBufferPoolSize};");
				code.AppendLine($"\t\t\tthis.MaxReceivedMessageSize = {b.MaxReceivedMessageSize};");
				code.AppendLine($"\t\t\tthis.Port = {b.Port};");
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(WcfBindingWebHTTP))
			{
				var b = o as WcfBindingWebHTTP;
				if (b == null) return "";
				code.AppendLine($"\t\t\tthis.AllowCookies = {(b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.BypassProxyOnLocal = {(b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.CrossDomainScriptAccessEnabled = {(b.CrossDomainScriptAccessEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)};");
				code.AppendLine($"\t\t\tthis.MaxBufferPoolSize = {b.MaxBufferPoolSize};");
				code.AppendLine($"\t\t\tthis.MaxBufferSize = {Convert.ToInt32(b.MaxBufferSize)};");
				code.AppendLine($"\t\t\tthis.MaxReceivedMessageSize = {b.MaxReceivedMessageSize};");
				code.AppendLine($"\t\t\tthis.TransferMode = TransferMode.{System.Enum.GetName(typeof (System.ServiceModel.TransferMode), b.TransferMode)};");
				code.AppendLine($"\t\t\tthis.WriteEncoding = System.Text.Encoding.{System.Enum.GetName(typeof (TextEncoding), b.WriteEncoding)};");
				if (!string.IsNullOrEmpty(b.ProxyAddress) && b.UseDefaultWebProxy == false)
				{
					code.AppendLine($"\t\t\tthis.ProxyAddress = new Uri(\"{b.ProxyAddress}\");");
					code.AppendLine(string.Format("\t\t\tthis.UseDefaultWebProxy = false;"));
				}
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(WcfBindingMSMQIntegration))
			{
				var b = o as WcfBindingMSMQIntegration;
				if (b == null) return "";
				code.AppendLine($"\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{b.CustomDeadLetterQueue}\");");
				code.AppendLine($"\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{System.Enum.GetName(typeof (System.ServiceModel.DeadLetterQueue), b.DeadLetterQueue)};");
				code.AppendLine($"\t\t\tthis.Durable = {(b.Durable ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.ExactlyOnce = {(b.ExactlyOnce ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.MaxReceivedMessageSize = {b.MaxReceivedMessageSize};");
				code.AppendLine($"\t\t\tthis.MaxRetryCycles = {b.MaxRetryCycles};");
				code.AppendLine($"\t\t\tthis.ReceiveContextEnabled = {(b.ReceiveContextEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{System.Enum.GetName(typeof (System.ServiceModel.ReceiveErrorHandling), b.ReceiveErrorHandling)};");
				code.AppendLine($"\t\t\tthis.ReceiveRetryCount = {b.ReceiveRetryCount};");
				code.AppendLine($"\t\t\tthis.RetryCycleDelay = new TimeSpan({b.RetryCycleDelay.Ticks});");
				code.AppendLine($"\t\t\tthis.TimeToLive = new TimeSpan({b.TimeToLive.Ticks});");
				code.AppendLine($"\t\t\tthis.UseMSMQTracing = {(b.UseMSMQTracing ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				code.AppendLine($"\t\t\tthis.UseSourceJournal = {(b.UseSourceJournal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				if (b.ValidityDuration != TimeSpan.Zero) code.AppendLine($"\t\t\tthis.ValidityDuration = new TimeSpan({b.ValidityDuration.Ticks});");
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
			}
			code.AppendLine("\t\t}");
			code.AppendLine("\t}");
			if (o.Parent.Owner.EnableDocumentationWarnings) code.AppendLine("#pragma warning enable 1591");
			return code.ToString();
		}

		public static string GenerateBindingBaseType(WcfBinding o)
		{
			return o.InheritedTypes.Count <= 0 ? "" : DataTypeGenerator.GenerateType(o.InheritedTypes[0]);
		}
	}
}