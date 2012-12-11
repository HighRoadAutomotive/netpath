using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETPath.Projects;
using NETPath.Projects.Helpers;

namespace NETPath.Generators.WinRT.CS
{
	internal static class BindingsGenerator
	{
		public static void VerifyCode(ServiceBinding o, Action<CompileMessage> AddMessage)
		{
			if (string.IsNullOrEmpty(o.Name))
				AddMessage(new CompileMessage("GS6000", "A binding in the '" + o.Parent.Name + "' project has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
			else
				if (RegExs.MatchCodeName.IsMatch(o.Name) == false)
					AddMessage(new CompileMessage("GS6001", "The binding '" + o.Name + "' in the '" + o.Parent.Name + "' project contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
			if (string.IsNullOrEmpty(o.Namespace)) { }
			else
				if (RegExs.MatchHTTPURI.IsMatch(o.Namespace) == false)
					AddMessage(new CompileMessage("GS6002", "The Namespace '" + o.Namespace + "' for the '" + o.Name + "' binding in the '" + o.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

			Type t = o.GetType();
			if (t == typeof(ServiceBindingBasicHTTP))
			{
				var b = o as ServiceBindingBasicHTTP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6003", "The Security for the '" + b.Name + "' Basic HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						AddMessage(new CompileMessage("GS6004", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' Basic HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
			}
			else if (t == typeof(ServiceBindingBasicHTTPS) || t == typeof(ServiceBindingNetHTTP) || t == typeof(ServiceBindingNetHTTPS))
			{
				var b = o as ServiceBindingNetHTTP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6003", "The Security for the '" + b.Name + "' Net HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						AddMessage(new CompileMessage("GS6004", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' Net HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
			}
			else if (t == typeof(ServiceBindingWSHTTP))
			{
				var b = o as ServiceBindingWSHTTP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6005", "The Security for the '" + b.Name + "' WS HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						AddMessage(new CompileMessage("GS6006", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
			}
			else if (t == typeof(ServiceBindingWS2007HTTP))
			{
				var b = o as ServiceBindingWS2007HTTP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6007", "The Security for the '" + b.Name + "' WS 2007 HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						AddMessage(new CompileMessage("GS6008", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS 2007 HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
			}
			else if (t == typeof(ServiceBindingWSDualHTTP))
			{
				var b = o as ServiceBindingWSDualHTTP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6009", "The Security for the '" + b.Name + "' WS Dual Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						AddMessage(new CompileMessage("GS6010", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS Dual HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
			}
			else if (t == typeof(ServiceBindingWSFederationHTTP))
			{
				var b = o as ServiceBindingWSFederationHTTP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6011", "The Security for the '" + b.Name + "' WS Federation HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						AddMessage(new CompileMessage("GS6012", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS Federation HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
			}
			else if (t == typeof(ServiceBindingWS2007FederationHTTP))
			{
				var b = o as ServiceBindingWS2007FederationHTTP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6013", "The Security for the '" + b.Name + "' WS 2007 Federation HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						AddMessage(new CompileMessage("GS6014", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS 2007 Federation HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
			}
			else if (t == typeof(ServiceBindingTCP))
			{
				var b = o as ServiceBindingTCP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6015", "The Security for the '" + b.Name + "' TCP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
			}
			else if (t == typeof(ServiceBindingNamedPipe))
			{
				var b = o as ServiceBindingNamedPipe;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6016", "The Security for the '" + b.Name + "' Named Pipe Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
			}
			else if (t == typeof(ServiceBindingMSMQ))
			{
				var b = o as ServiceBindingMSMQ;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6017", "The Security for the '" + b.Name + "' MSMQ Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
			}
			else if (t == typeof(ServiceBindingPeerTCP))
			{
				var b = o as ServiceBindingPeerTCP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6018", "The Security for the '" + b.Name + "' Peer TCP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
				if (b != null && !string.IsNullOrEmpty(b.ListenIPAddress))
					if (RegExs.MatchIPv4.IsMatch(b.ListenIPAddress) == false && RegExs.MatchIPv6.IsMatch(b.ListenIPAddress) == false)
						AddMessage(new CompileMessage("GS6019", "The Listen IP Address for the '" + b.Name + "' Peer TCP Binding in the '" + b.Parent.Name + "' project is not valid.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
			}
			else if (t == typeof(ServiceBindingWebHTTP))
			{
				var b = o as ServiceBindingWebHTTP;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6020", "The Security for the '" + b.Name + "' MSMQ Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
				if (b != null && !string.IsNullOrEmpty(b.ProxyAddress))
					if (RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						AddMessage(new CompileMessage("GS6021", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' Basic HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
			}
			else if (t == typeof(ServiceBindingMSMQIntegration))
			{
				var b = o as ServiceBindingMSMQIntegration;
				if (b != null && b.Security == null)
					AddMessage(new CompileMessage("GS6021", "The Security for the '" + b.Name + "' MSMQ Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType(), b.Parent.Owner.ID));
			}
		}

		public static string GenerateCode45(ServiceBinding o)
		{
			Type t = o.GetType();
			if (t != typeof (ServiceBindingBasicHTTP) && t != typeof (ServiceBindingNetHTTP) && t != typeof (ServiceBindingTCP)) return "";
			var code = new StringBuilder();
			if (o.Parent.Owner.EnableDocumentationWarnings) code.AppendLine("#pragma warning disable 1591");
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0}", DataTypeGenerator.GenerateTypeDeclaration(o)));
			code.AppendLine("\t{");
			code.AppendLine(string.Format("\t\tpublic {0}()", o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t}");
			if (t == typeof(ServiceBindingBasicHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingNetHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpSecurity CustomSecurity)", o.Name));
			if (t == typeof(ServiceBindingTCP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.NetTcpSecurity CustomSecurity)", o.Name));
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
			if (t == typeof(ServiceBindingNetHTTP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.BasicHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
			if (t == typeof(ServiceBindingTCP)) code.AppendLine(string.Format("\t\tpublic {0}(System.ServiceModel.NetTcpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)", o.Name));
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
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingNetHTTP))
			{
				var b = o as ServiceBindingNetHTTP;
				if (b == null) return "";
				code.AppendLine(string.Format("\t\t\tthis.AllowCookies = {0};", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower()));
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
					code.AppendLine(string.Format("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode)));
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
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
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
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
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
				if (b.Security != null) code.AppendLine(SecurityGenerator.GenerateCode45(b.Security));
			}
			code.AppendLine("\t\t}");
			code.AppendLine("\t}");
			if (o.Parent.Owner.EnableDocumentationWarnings) code.AppendLine("#pragma warning enable 1591");
			return code.ToString();
		}

		public static string GenerateBindingBaseType(ServiceBinding o)
		{
			return o.InheritedTypes.Count <= 0 ? "" : DataTypeGenerator.GenerateType(o.InheritedTypes[0]);
		}
	}
}