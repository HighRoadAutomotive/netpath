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
			code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			code.AppendLine("\t{");
			code.AppendFormat("\t\tpublic {0}(){1}", o.Name, Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t}");
			code.AppendFormat("\t\tpublic {0}({1} CustomSecurity){2}", o.Name, GenerateBindingBaseType(o), Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t\tBasicHttpSecurity sec = this.Security;");
			code.AppendLine("\t\t\tsec = CustomSecurity;");
			code.AppendLine("\t\t}");
			code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", o.Name, Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			code.AppendLine("\t\t}");
			code.AppendFormat("\t\tpublic {0}({1} CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){2}", o.Name, GenerateBindingBaseType(o), Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t\tBasicHttpSecurity sec = this.Security;");
			code.AppendLine("\t\t\tsec = CustomSecurity;");
			code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			code.AppendLine("\t\t}");
			code.AppendLine("\t\tprivate void SetDefaults()");
			code.AppendLine("\t\t{");
			// Generic Binding code.
			code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", o.Name, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", o.ReceiveTimeout.Ticks, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", o.SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			if (t == typeof(ServiceBindingBasicHTTP))
			{
				var b = o as ServiceBindingBasicHTTP;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MessageEncoding = WSMessageEncoding.{0};{1}", System.Enum.GetName(typeof (System.ServiceModel.WSMessageEncoding), b.MessageEncoding), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof (ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof (System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingWSHTTP))
			{
				var b = o as ServiceBindingWSHTTP;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingWS2007HTTP))
			{
				var b = o as ServiceBindingWS2007HTTP;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingWSDualHTTP))
			{
				var b = o as ServiceBindingWSDualHTTP;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ClientBaseAddress != "") code.AppendFormat("\t\t\tthis.ClientBaseAddress = new Uri(\"{0}\");{1}", b.ClientBaseAddress, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingWSFederationHTTP))
			{
				var b = o as ServiceBindingWSFederationHTTP;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.PrivacyNoticeAt != "")
				{
					code.AppendFormat("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");{1}", b.PrivacyNoticeAt, Environment.NewLine);
					code.AppendFormat("\t\t\tthis.PrivacyNoticeVersion = {0};{1}", b.PrivacyNoticeVersion, Environment.NewLine);
				}
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingWS2007FederationHTTP))
			{
				var b = o as ServiceBindingWS2007FederationHTTP;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.PrivacyNoticeAt != "")
				{
					code.AppendFormat("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");{1}", b.PrivacyNoticeAt, Environment.NewLine);
					code.AppendFormat("\t\t\tthis.PrivacyNoticeVersion = {0};{1}", b.PrivacyNoticeVersion, Environment.NewLine);
				}
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingTCP))
			{
				var b = o as ServiceBindingTCP;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ListenBacklog = {0};{1}", b.ListenBacklog, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxConnections = {0};{1}", b.MaxConnections, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.PortSharingEnabled = {0};{1}", b.PortSharingEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTransactionProtocol), b.TransactionProtocol), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingNamedPipe))
			{
				var b = o as ServiceBindingNamedPipe;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxConnections = {0};{1}", b.MaxConnections, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTransactionProtocol), b.TransactionProtocol), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingMSMQ))
			{
				var b = o as ServiceBindingMSMQ;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");{1}", b.CustomDeadLetterQueue, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), b.DeadLetterQueue), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Durable = {0};{1}", b.Durable ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ExactlyOnce = {0};{1}", b.ExactlyOnce ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxRetryCycles = {0};{1}", b.MaxRetryCycles, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.QueueTransferProtocol = QueueTransferProtocol.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.QueueTransferProtocol), b.QueueTransferProtocol), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReceiveContextEnabled = {0};{1}", b.ReceiveContextEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), b.ReceiveErrorHandling), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReceiveRetryCount = {0};{1}", b.ReceiveRetryCount, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});{1}", b.RetryCycleDelay.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TimeToLive = new TimeSpan({0});{1}", b.TimeToLive.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.UseActiveDirectory = {0};{1}", b.UseActiveDirectory ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.UseMSMQTracing = {0};{1}", b.UseMSMQTracing ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.UseSourceJournal = {0};{1}", b.UseSourceJournal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ValidityDuration != TimeSpan.Zero) code.AppendFormat("\t\t\tthis.ValidityDuration = new TimeSpan({0});{1}", b.ValidityDuration.Ticks, Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingPeerTCP))
			{
				var b = o as ServiceBindingPeerTCP;
				if (b == null) return "";
				if (b.ListenIPAddress == "") { code.AppendLine("\t\t\tthis.ListenIPAddress = null;"); } else { code.AppendFormat("\t\t\tthis.ListenIPAddress = IPAddress.Parse(\"{0}\");{1}", b.ListenIPAddress, Environment.NewLine); }
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Port = {0};{1}", b.Port, Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingWebHTTP))
			{
				var b = o as ServiceBindingWebHTTP;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.CrossDomainScriptAccessEnabled = {0};{1}", b.CrossDomainScriptAccessEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				code.AppendFormat("\t\t\tthis.WriteEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.WriteEncoding), Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode35(b.Security));
			}
			else if (t == typeof(ServiceBindingMSMQIntegration))
			{
				var b = o as ServiceBindingMSMQIntegration;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");{1}", b.CustomDeadLetterQueue, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), b.DeadLetterQueue), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Durable = {0};{1}", b.Durable ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ExactlyOnce = {0};{1}", b.ExactlyOnce ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxRetryCycles = {0};{1}", b.MaxRetryCycles, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReceiveContextEnabled = {0};{1}", b.ReceiveContextEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), b.ReceiveErrorHandling), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReceiveRetryCount = {0};{1}", b.ReceiveRetryCount, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});{1}", b.RetryCycleDelay.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TimeToLive = new TimeSpan({0});{1}", b.TimeToLive.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.UseMSMQTracing = {0};{1}", b.UseMSMQTracing ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.UseSourceJournal = {0};{1}", b.UseSourceJournal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ValidityDuration != TimeSpan.Zero) code.AppendFormat("\t\t\tthis.ValidityDuration = new TimeSpan({0});{1}", b.ValidityDuration.Ticks, Environment.NewLine);
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
			code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendLine(string.Format("\t{0}", DataTypeCSGenerator.GenerateTypeDeclaration(o)));
			code.AppendLine("\t{");
			code.AppendFormat("\t\tpublic {0}(){1}", o.Name, Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t}");
			code.AppendFormat("\t\tpublic {0}({1} CustomSecurity){2}", o.Name, GenerateBindingBaseType(o), Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			code.AppendLine("\t\t}");
			code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", o.Name, Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			code.AppendLine("\t\t}");
			code.AppendFormat("\t\tpublic {0}({1} CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){2}", o.Name, GenerateBindingBaseType(o), Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tSetDefaults();");
			code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			code.AppendLine("\t\t}");
			code.AppendLine("\t\tprivate void SetDefaults()");
			code.AppendLine("\t\t{");
			// Generic Binding code.
			code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", o.Name, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", o.SendTimeout.Ticks, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", o.ReceiveTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			if (t == typeof(ServiceBindingBasicHTTP))
			{
				var b = o as ServiceBindingBasicHTTP;
				if (b == null) return "";
				if (Globals.CurrentGenerationTarget != ProjectGenerationFramework.WIN8)
				{
					code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
					code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
					code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
					code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
					code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
					code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
					code.AppendFormat("\t\t\tthis.MessageEncoding = WSMessageEncoding.{0};{1}", System.Enum.GetName(typeof (System.ServiceModel.WSMessageEncoding), b.MessageEncoding), Environment.NewLine);
					if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
					code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof (ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
					code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof (System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
					if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			if (t == typeof(ServiceBindingBasicHTTPS))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingBasicHTTPS;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MessageEncoding = WSMessageEncoding.{0};{1}", System.Enum.GetName(typeof (System.ServiceModel.WSMessageEncoding), b.MessageEncoding), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof (ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof (System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingNetHTTP))
			{
				var b = o as ServiceBindingNetHTTP;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MessageEncoding = NetHttpMessageEncoding.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.NetHttpMessageEncoding), b.MessageEncoding), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.WebSocketSettings.CreateNotificationOnConnection = {0};{1}", b.CreateNotificationOnConnection ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.WebSocketSettings.DisablePayloadMasking = {0};{1}", b.DisablePayloadMasking ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.WebSocketSettings.KeepAliveInterval = new TimeSpan({0});{1}", b.KeepAliveInterval.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.WebSocketSettings.MaxPendingConnections = {0};{1}", b.MaxPendingConnections, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.WebSocketSettings.SubProtocol = \"{0}\";{1}", b.SubProtocol, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.WebSocketSettings.TransportUsage = System.ServiceModel.Channels.WebSocketTransportUsage.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Channels.WebSocketTransportUsage), b.TransportUsage), Environment.NewLine);
				if (Globals.CurrentGenerationTarget != ProjectGenerationFramework.WIN8)
				{
					code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
					if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
					if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingNetHTTPS))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingNetHTTPS;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MessageEncoding = NetHttpMessageEncoding.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.NetHttpMessageEncoding), b.MessageEncoding), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.WebSocketSettings.CreateNotificationOnConnection = {0};{1}", b.CreateNotificationOnConnection ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.WebSocketSettings.DisablePayloadMasking = {0};{1}", b.DisablePayloadMasking ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.WebSocketSettings.KeepAliveInterval = new TimeSpan({0});{1}", b.KeepAliveInterval.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.WebSocketSettings.MaxPendingConnections = {0};{1}", b.MaxPendingConnections, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.WebSocketSettings.SubProtocol = \"{0}\";{1}", b.SubProtocol, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.WebSocketSettings.TransportUsage = System.ServiceModel.Channels.WebSocketTransportUsage.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Channels.WebSocketTransportUsage), b.TransportUsage), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingWSHTTP))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingWSHTTP;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingWS2007HTTP))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingWS2007HTTP;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingWSDualHTTP))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingWSDualHTTP;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ClientBaseAddress != "") code.AppendFormat("\t\t\tthis.ClientBaseAddress = new Uri(\"{0}\");{1}", b.ClientBaseAddress, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingWSFederationHTTP))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingWSFederationHTTP;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.PrivacyNoticeAt != "")
				{
					code.AppendFormat("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");{1}", b.PrivacyNoticeAt, Environment.NewLine);
					code.AppendFormat("\t\t\tthis.PrivacyNoticeVersion = {0};{1}", b.PrivacyNoticeVersion, Environment.NewLine);
				}
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingWS2007FederationHTTP))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingWS2007FederationHTTP;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.PrivacyNoticeAt != "")
				{
					code.AppendFormat("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");{1}", b.PrivacyNoticeAt, Environment.NewLine);
					code.AppendFormat("\t\t\tthis.PrivacyNoticeVersion = {0};{1}", b.PrivacyNoticeVersion, Environment.NewLine);
				}
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingTCP))
			{
				var b = o as ServiceBindingTCP;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				if (Globals.CurrentGenerationTarget != ProjectGenerationFramework.WIN8)
				{
					code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof (System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
					code.AppendFormat("\t\t\tthis.ListenBacklog = {0};{1}", b.ListenBacklog, Environment.NewLine);
					code.AppendFormat("\t\t\tthis.MaxConnections = {0};{1}", b.MaxConnections, Environment.NewLine);
					code.AppendFormat("\t\t\tthis.PortSharingEnabled = {0};{1}", b.PortSharingEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
					code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
					code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
					code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
					code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
					code.AppendFormat("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};{1}", System.Enum.GetName(typeof (ServiceBindingTransactionProtocol), b.TransactionProtocol), Environment.NewLine);
				}
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingNamedPipe))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingNamedPipe;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxConnections = {0};{1}", b.MaxConnections, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTransactionProtocol), b.TransactionProtocol), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingMSMQ))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingMSMQ;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");{1}", b.CustomDeadLetterQueue, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), b.DeadLetterQueue), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Durable = {0};{1}", b.Durable ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ExactlyOnce = {0};{1}", b.ExactlyOnce ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxRetryCycles = {0};{1}", b.MaxRetryCycles, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.QueueTransferProtocol = QueueTransferProtocol.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.QueueTransferProtocol), b.QueueTransferProtocol), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReceiveContextEnabled = {0};{1}", b.ReceiveContextEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), b.ReceiveErrorHandling), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReceiveRetryCount = {0};{1}", b.ReceiveRetryCount, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});{1}", b.RetryCycleDelay.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TimeToLive = new TimeSpan({0});{1}", b.TimeToLive.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.UseActiveDirectory = {0};{1}", b.UseActiveDirectory ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.UseMSMQTracing = {0};{1}", b.UseMSMQTracing ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.UseSourceJournal = {0};{1}", b.UseSourceJournal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ValidityDuration != TimeSpan.Zero) code.AppendFormat("\t\t\tthis.ValidityDuration = new TimeSpan({0});{1}", b.ValidityDuration.Ticks, Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingPeerTCP))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingPeerTCP;
				if (b == null) return "";
				if (b.ListenIPAddress == "") { code.AppendLine("\t\t\tthis.ListenIPAddress = null;"); } else { code.AppendFormat("\t\t\tthis.ListenIPAddress = IPAddress.Parse(\"{0}\");{1}", b.ListenIPAddress, Environment.NewLine); }
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Port = {0};{1}", b.Port, Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingWebHTTP))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingWebHTTP;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.CrossDomainScriptAccessEnabled = {0};{1}", b.CrossDomainScriptAccessEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				code.AppendFormat("\t\t\tthis.WriteEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), b.WriteEncoding), Environment.NewLine);
				if (b.Security != null) code.AppendLine(SecurityCSGenerator.GenerateCode45(b.Security));
			}
			else if (t == typeof(ServiceBindingMSMQIntegration))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8) return "";
				var b = o as ServiceBindingMSMQIntegration;
				if (b == null) return "";
				code.AppendFormat("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");{1}", b.CustomDeadLetterQueue, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), b.DeadLetterQueue), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Durable = {0};{1}", b.Durable ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ExactlyOnce = {0};{1}", b.ExactlyOnce ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.MaxRetryCycles = {0};{1}", b.MaxRetryCycles, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReceiveContextEnabled = {0};{1}", b.ReceiveContextEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), b.ReceiveErrorHandling), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.ReceiveRetryCount = {0};{1}", b.ReceiveRetryCount, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});{1}", b.RetryCycleDelay.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.TimeToLive = new TimeSpan({0});{1}", b.TimeToLive.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.UseMSMQTracing = {0};{1}", b.UseMSMQTracing ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.UseSourceJournal = {0};{1}", b.UseSourceJournal ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ValidityDuration != TimeSpan.Zero) code.AppendFormat("\t\t\tthis.ValidityDuration = new TimeSpan({0});{1}", b.ValidityDuration.Ticks, Environment.NewLine);
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