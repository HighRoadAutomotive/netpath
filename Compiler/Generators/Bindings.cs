using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFArchitect.Compiler.Generators
{
	internal static class BindingsCSGenerator
	{
		public static bool VerifyCode(Projects.ServiceBinding o)
		{
			bool NoErrors = true;

			if (o.Name == "" || o.Name == null)
			{
				Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6000", "A binding in the '" + o.Parent.Name + "' project has a blank Code Name. A Code Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.Name) == false)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6001", "The binding '" + o.Name + "' in the '" + o.Parent.Name + "' project contains invalid characters in the Code Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
					NoErrors = false;
				}
			if (o.Namespace == "" || o.Namespace == null) { }
			else
				if (Helpers.RegExs.MatchHTTPURI.IsMatch(o.Namespace) == false)
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6002", "The Namespace '" + o.Namespace + "' for the '" + o.Name + "' binding in the '" + o.Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, o.Parent, o, o.GetType()));

			Type t = o.GetType();
			if (t == typeof(Projects.ServiceBindingBasicHTTP))
			{
				Projects.ServiceBindingBasicHTTP b = o as Projects.ServiceBindingBasicHTTP;
				if (b.Security == null)
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6003", "The Security for the '" + b.Name + "' Basic HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
				if (b.ProxyAddress != "" && b.ProxyAddress != null)
					if (Helpers.RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6004", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' Basic HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(Projects.ServiceBindingNetHTTP))
			{
				Projects.ServiceBindingNetHTTP b = o as Projects.ServiceBindingNetHTTP;
				if (b.Parent.Owner.GetType() == typeof(Projects.ProjectNET))
				{
					Projects.ProjectNET p = b.Parent.Owner as Projects.ProjectNET;
					if (Globals.CurrentNETOutput == Projects.ProjectNETOutputFramework.NET45 && p != null)
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6001", "The binding '" + o.Name + "' in the '" + o.Parent.Name + "' project cannot be used with any other target framework level than 4.5. Please select .NET Framework 4.5 as your Target Framework in the Project page or remove this binding.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
				}
				if (b.Security == null)
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6003", "The Security for the '" + b.Name + "' Net HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
				if (b.ProxyAddress != "" && b.ProxyAddress != null)
					if (Helpers.RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6004", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' Net HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(Projects.ServiceBindingWSHTTP))
			{
				Projects.ServiceBindingWSHTTP b = o as Projects.ServiceBindingWSHTTP;
				if (b.Security == null)
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6005", "The Security for the '" + b.Name + "' WS HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
				if (b.ProxyAddress != "" && b.ProxyAddress != null)
					if (Helpers.RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6006", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(Projects.ServiceBindingWS2007HTTP))
			{
				Projects.ServiceBindingWS2007HTTP b = o as Projects.ServiceBindingWS2007HTTP;
				if (b.Security == null)
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6007", "The Security for the '" + b.Name + "' WS 2007 HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
				if (b.ProxyAddress != "" && b.ProxyAddress != null)
					if (Helpers.RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6008", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS 2007 HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(Projects.ServiceBindingWSDualHTTP))
			{
				Projects.ServiceBindingWSDualHTTP b = o as Projects.ServiceBindingWSDualHTTP;
				if (b.Security == null)
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6009", "The Security for the '" + b.Name + "' WS Dual Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
				if (b.ProxyAddress != "" && b.ProxyAddress != null)
					if (Helpers.RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6010", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS Dual HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(Projects.ServiceBindingWSFederationHTTP))
			{
				Projects.ServiceBindingWSFederationHTTP b = o as Projects.ServiceBindingWSFederationHTTP;
				if (b.Security == null)
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6011", "The Security for the '" + b.Name + "' WS Federation HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
				if (b.ProxyAddress != "" && b.ProxyAddress != null)
					if (Helpers.RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6012", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS Federation HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(Projects.ServiceBindingWS2007FederationHTTP))
			{
				Projects.ServiceBindingWS2007FederationHTTP b = o as Projects.ServiceBindingWS2007FederationHTTP;
				if (b.Security == null)
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6013", "The Security for the '" + b.Name + "' WS 2007 Fenderation HTTP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
				if (b.ProxyAddress != "" && b.ProxyAddress != null)
					if (Helpers.RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6014", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' WS 2007 Federation HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(Projects.ServiceBindingTCP))
			{
				Projects.ServiceBindingTCP b = o as Projects.ServiceBindingTCP;
				if (b.Security == null)
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6015", "The Security for the '" + b.Name + "' TCP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(Projects.ServiceBindingNamedPipe))
			{
				Projects.ServiceBindingNamedPipe b = o as Projects.ServiceBindingNamedPipe;
				if (b.Security == null)
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6016", "The Security for the '" + b.Name + "' Named Pipe Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(Projects.ServiceBindingMSMQ))
			{
				Projects.ServiceBindingMSMQ b = o as Projects.ServiceBindingMSMQ;
				if (b.Security == null)
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6017", "The Security for the '" + b.Name + "' MSMQ Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(Projects.ServiceBindingPeerTCP))
			{
				Projects.ServiceBindingPeerTCP b = o as Projects.ServiceBindingPeerTCP;
				if (b.Security == null)
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6018", "The Security for the '" + b.Name + "' Peer TCP Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
				if (b.ListenIPAddress != "" && b.ListenIPAddress != null)
					if (Helpers.RegExs.MatchIPv4.IsMatch(b.ListenIPAddress) == false && Helpers.RegExs.MatchIPv6.IsMatch(b.ListenIPAddress) == false)
					{
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6019", "The Listen IP Address for the '" + b.Name + "' Peer TCP Binding in the '" + b.Parent.Name + "' project is not valid.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
						NoErrors = true;
					}
			}
			else if (t == typeof(Projects.ServiceBindingWebHTTP))
			{
				Projects.ServiceBindingWebHTTP b = o as Projects.ServiceBindingWebHTTP;
				if (b.Security == null)
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6020", "The Security for the '" + b.Name + "' MSMQ Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
				if (b.ProxyAddress != "" && b.ProxyAddress != null)
					if (Helpers.RegExs.MatchHTTPURI.IsMatch(b.ProxyAddress) == false)
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6021", "The Proxy Address '" + b.ProxyAddress + "' for the '" + b.Name + "' Basic HTTP Binding in the '" + b.Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
			}
			else if (t == typeof(Projects.ServiceBindingMSMQIntegration))
			{
				Projects.ServiceBindingMSMQIntegration b = o as Projects.ServiceBindingMSMQIntegration;
				if (b.Security == null)
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6021", "The Security for the '" + b.Name + "' MSMQ Binding in the '" + b.Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
			}

			return NoErrors;
		}

		public static string GenerateCode30(Projects.ServiceBinding o)
		{
			Type t = o.GetType();
			if (t == typeof(Projects.ServiceBindingWebHTTP)) return "";

			return GenerateCode35(o);
		}
		public static string GenerateCode35(Projects.ServiceBinding o)
		{
			Type t = o.GetType();
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}({1} CustomSecurity){2}", o.Name, GenerateBindingBaseType(o), Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tBasicHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}({1} CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){2}", o.Name, GenerateBindingBaseType(o), Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tBasicHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", o.Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", o.ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", o.SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			if (t == typeof(Projects.ServiceBindingBasicHTTP))
			{
				Projects.ServiceBindingBasicHTTP b = o as Projects.ServiceBindingBasicHTTP;
				Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MessageEncoding = WSMessageEncoding.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.WSMessageEncoding), b.MessageEncoding), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\t = {0}.SetSecurity(this.Security);{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingWSHTTP))
			{
				Projects.ServiceBindingWSHTTP b = o as Projects.ServiceBindingWSHTTP;
				Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\t = {0}.SetSecurity(this.Security);{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingWS2007HTTP))
			{
				Projects.ServiceBindingWS2007HTTP b = o as Projects.ServiceBindingWS2007HTTP;
				Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\t = {0}.SetSecurity(this.Security);{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingWSDualHTTP))
			{
				Projects.ServiceBindingWSDualHTTP b = o as Projects.ServiceBindingWSDualHTTP;
				Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ClientBaseAddress != "") Code.AppendFormat("\t\t\tthis.ClientBaseAddress = new Uri(\"{0}\");{1}", b.ClientBaseAddress, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\t = {0}.SetSecurity(this.Security);{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingWSFederationHTTP))
			{
				Projects.ServiceBindingWSFederationHTTP b = o as Projects.ServiceBindingWSFederationHTTP;
				Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.PrivacyNoticeAt != "")
				{
					Code.AppendFormat("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");{1}", b.PrivacyNoticeAt, Environment.NewLine);
					Code.AppendFormat("\t\t\tthis.PrivacyNoticeVersion = {0};{1}", b.PrivacyNoticeVersion, Environment.NewLine);
				}
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\t = {0}.SetSecurity(this.Security);{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingWS2007FederationHTTP))
			{
				Projects.ServiceBindingWS2007FederationHTTP b = o as Projects.ServiceBindingWS2007FederationHTTP;
				Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.PrivacyNoticeAt != "")
				{
					Code.AppendFormat("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");{1}", b.PrivacyNoticeAt, Environment.NewLine);
					Code.AppendFormat("\t\t\tthis.PrivacyNoticeVersion = {0};{1}", b.PrivacyNoticeVersion, Environment.NewLine);
				}
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\t = {0}.SetSecurity(this.Security);{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingTCP))
			{
				Projects.ServiceBindingTCP b = o as Projects.ServiceBindingTCP;
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ListenBacklog = {0};{1}", b.ListenBacklog, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxConnections = {0};{1}", b.MaxConnections, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.PortSharingEnabled = {0};{1}", b.PortSharingEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTransactionProtocol), b.TransactionProtocol), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\t = {0}.SetSecurity(this.Security);{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingNamedPipe))
			{
				Projects.ServiceBindingNamedPipe b = o as Projects.ServiceBindingNamedPipe;
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxConnections = {0};{1}", b.MaxConnections, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTransactionProtocol), b.TransactionProtocol), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\t = {0}.SetSecurity(this.Security);{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingMSMQ))
			{
				Projects.ServiceBindingMSMQ b = o as Projects.ServiceBindingMSMQ;
				Code.AppendFormat("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");{1}", b.CustomDeadLetterQueue, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), b.DeadLetterQueue), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Durable = {0};{1}", b.Durable == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ExactlyOnce = {0};{1}", b.ExactlyOnce == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxRetryCycles = {0};{1}", b.MaxRetryCycles, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.QueueTransferProtocol = QueueTransferProtocol.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.QueueTransferProtocol), b.QueueTransferProtocol), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReceiveContextEnabled = {0};{1}", b.ReceiveContextEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), b.ReceiveErrorHandling), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReceiveRetryCount = {0};{1}", b.ReceiveRetryCount, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});{1}", b.RetryCycleDelay.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TimeToLive = new TimeSpan({0});{1}", b.TimeToLive.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UseActiveDirectory = {0};{1}", b.UseActiveDirectory == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UseMSMQTracing = {0};{1}", b.UseMSMQTracing == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UseSourceJournal = {0};{1}", b.UseSourceJournal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ValidityDuration != TimeSpan.Zero) Code.AppendFormat("\t\t\tthis.ValidityDuration = new TimeSpan({0});{1}", b.ValidityDuration.Ticks, Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\tthis.Security = new {0}();{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingPeerTCP))
			{
				Projects.ServiceBindingPeerTCP b = o as Projects.ServiceBindingPeerTCP;
				if (b.ListenIPAddress == "") { Code.AppendLine("\t\t\tthis.ListenIPAddress = null;"); } else { Code.AppendFormat("\t\t\tthis.ListenIPAddress = IPAddress.Parse(\"{0}\");{1}", b.ListenIPAddress, Environment.NewLine); }
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Port = {0};{1}", b.Port, Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\t = {0}.SetSecurity(this.Security);{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingWebHTTP))
			{
				Projects.ServiceBindingWebHTTP b = o as Projects.ServiceBindingWebHTTP;
				Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.CrossDomainScriptAccessEnabled = {0};{1}", b.CrossDomainScriptAccessEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.WriteEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTextEncoding), b.WriteEncoding), Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\t = {0}.SetSecurity(this.Security);{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingMSMQIntegration))
			{
				Projects.ServiceBindingMSMQIntegration b = o as Projects.ServiceBindingMSMQIntegration;
				Code.AppendFormat("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");{1}", b.CustomDeadLetterQueue, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), b.DeadLetterQueue), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Durable = {0};{1}", b.Durable == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ExactlyOnce = {0};{1}", b.ExactlyOnce == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxRetryCycles = {0};{1}", b.MaxRetryCycles, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReceiveContextEnabled = {0};{1}", b.ReceiveContextEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), b.ReceiveErrorHandling), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReceiveRetryCount = {0};{1}", b.ReceiveRetryCount, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});{1}", b.RetryCycleDelay.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TimeToLive = new TimeSpan({0});{1}", b.TimeToLive.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UseMSMQTracing = {0};{1}", b.UseMSMQTracing == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UseSourceJournal = {0};{1}", b.UseSourceJournal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ValidityDuration != TimeSpan.Zero) Code.AppendFormat("\t\t\tthis.ValidityDuration = new TimeSpan({0});{1}", b.ValidityDuration.Ticks, Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\t = {0}.SetSecurity(this.Security);{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public static string GenerateCode40(Projects.ServiceBinding o)
		{
			return GenerateCode45(o);
		}

		public static string GenerateCode45(Projects.ServiceBinding o)
		{
			Type t = o.GetType();
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0} class {1} : {2}{3}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}({1} CustomSecurity){2}", o.Name, GenerateBindingBaseType(o), Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}({1} CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){2}", o.Name, GenerateBindingBaseType(o), Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", o.Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", o.ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", o.SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			if (t == typeof(Projects.ServiceBindingBasicHTTP))
			{
				Projects.ServiceBindingBasicHTTP b = o as Projects.ServiceBindingBasicHTTP;
				Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MessageEncoding = WSMessageEncoding.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.WSMessageEncoding), b.MessageEncoding), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\tthis.Security = new {0}();{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingNetHTTP))
			{
				Projects.ServiceBindingNetHTTP b = o as Projects.ServiceBindingNetHTTP;
				Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MessageEncoding = NetHttpMessageEncoding.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.NetHttpMessageEncoding), b.MessageEncoding), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.WebSocketSettings.CreateNotificationOnConnection = {0};{1}", b.CreateNotificationOnConnection == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.WebSocketSettings.DisablePayloadMasking = {0};{1}", b.DisablePayloadMasking == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.WebSocketSettings.KeepAliveInterval = new TimeSpan({0});{1}", b.KeepAliveInterval.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.WebSocketSettings.MaxPendingConnections = {0};{1}", b.MaxPendingConnections, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.WebSocketSettings.SubProtocol = \"{0}\";{1}", b.SubProtocol, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.WebSocketSettings.TransportUsage = System.ServiceModel.Channels.WebSocketTransportUsage.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Channels.WebSocketTransportUsage), b.TransportUsage), Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\tthis.Security = new {0}();{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingWSHTTP))
			{
				Projects.ServiceBindingWSHTTP b = o as Projects.ServiceBindingWSHTTP;
				Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\tthis.Security = new {0}();{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingWS2007HTTP))
			{
				Projects.ServiceBindingWS2007HTTP b = o as Projects.ServiceBindingWS2007HTTP;
				Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\tthis.Security = new {0}();{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingWSDualHTTP))
			{
				Projects.ServiceBindingWSDualHTTP b = o as Projects.ServiceBindingWSDualHTTP;
				Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ClientBaseAddress != "") Code.AppendFormat("\t\t\tthis.ClientBaseAddress = new Uri(\"{0}\");{1}", b.ClientBaseAddress, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\tthis.Security = new {0}();{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingWSFederationHTTP))
			{
				Projects.ServiceBindingWSFederationHTTP b = o as Projects.ServiceBindingWSFederationHTTP;
				Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.PrivacyNoticeAt != "")
				{
					Code.AppendFormat("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");{1}", b.PrivacyNoticeAt, Environment.NewLine);
					Code.AppendFormat("\t\t\tthis.PrivacyNoticeVersion = {0};{1}", b.PrivacyNoticeVersion, Environment.NewLine);
				}
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\tthis.Security = new {0}();{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingWS2007FederationHTTP))
			{
				Projects.ServiceBindingWS2007FederationHTTP b = o as Projects.ServiceBindingWS2007FederationHTTP;
				Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.PrivacyNoticeAt != "")
				{
					Code.AppendFormat("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");{1}", b.PrivacyNoticeAt, Environment.NewLine);
					Code.AppendFormat("\t\t\tthis.PrivacyNoticeVersion = {0};{1}", b.PrivacyNoticeVersion, Environment.NewLine);
				}
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTextEncoding), b.TextEncoding), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\tthis.Security = new {0}();{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingTCP))
			{
				Projects.ServiceBindingTCP b = o as Projects.ServiceBindingTCP;
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ListenBacklog = {0};{1}", b.ListenBacklog, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxConnections = {0};{1}", b.MaxConnections, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.PortSharingEnabled = {0};{1}", b.PortSharingEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", b.ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", b.ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", b.ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTransactionProtocol), b.TransactionProtocol), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\tthis.Security = new {0}();{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingNamedPipe))
			{
				Projects.ServiceBindingNamedPipe b = o as Projects.ServiceBindingNamedPipe;
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxConnections = {0};{1}", b.MaxConnections, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", b.TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTransactionProtocol), b.TransactionProtocol), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\tthis.Security = new {0}();{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingMSMQ))
			{
				Projects.ServiceBindingMSMQ b = o as Projects.ServiceBindingMSMQ;
				Code.AppendFormat("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");{1}", b.CustomDeadLetterQueue, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), b.DeadLetterQueue), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Durable = {0};{1}", b.Durable == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ExactlyOnce = {0};{1}", b.ExactlyOnce == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxRetryCycles = {0};{1}", b.MaxRetryCycles, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.QueueTransferProtocol = QueueTransferProtocol.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.QueueTransferProtocol), b.QueueTransferProtocol), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReceiveContextEnabled = {0};{1}", b.ReceiveContextEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), b.ReceiveErrorHandling), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReceiveRetryCount = {0};{1}", b.ReceiveRetryCount, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});{1}", b.RetryCycleDelay.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TimeToLive = new TimeSpan({0});{1}", b.TimeToLive.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UseActiveDirectory = {0};{1}", b.UseActiveDirectory == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UseMSMQTracing = {0};{1}", b.UseMSMQTracing == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UseSourceJournal = {0};{1}", b.UseSourceJournal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ValidityDuration != TimeSpan.Zero) Code.AppendFormat("\t\t\tthis.ValidityDuration = new TimeSpan({0});{1}", b.ValidityDuration.Ticks, Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\tthis.Security = new {0}();{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingPeerTCP))
			{
				Projects.ServiceBindingPeerTCP b = o as Projects.ServiceBindingPeerTCP;
				if (b.ListenIPAddress == "") { Code.AppendLine("\t\t\tthis.ListenIPAddress = null;"); } else { Code.AppendFormat("\t\t\tthis.ListenIPAddress = IPAddress.Parse(\"{0}\");{1}", b.ListenIPAddress, Environment.NewLine); }
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Port = {0};{1}", b.Port, Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\tthis.Security = new {0}();{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingWebHTTP))
			{
				Projects.ServiceBindingWebHTTP b = o as Projects.ServiceBindingWebHTTP;
				Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", b.AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", b.BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.CrossDomainScriptAccessEnabled = {0};{1}", b.CrossDomainScriptAccessEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), b.HostNameComparisonMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", b.MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(b.MaxBufferSize.BytesNormalized), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", b.ProxyAddress, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), b.TransferMode), Environment.NewLine);
				if (b.ProxyAddress != "" && b.UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.WriteEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(Projects.ServiceBindingTextEncoding), b.WriteEncoding), Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\tthis.Security = new {0}();{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			else if (t == typeof(Projects.ServiceBindingMSMQIntegration))
			{
				Projects.ServiceBindingMSMQIntegration b = o as Projects.ServiceBindingMSMQIntegration;
				Code.AppendFormat("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");{1}", b.CustomDeadLetterQueue, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), b.DeadLetterQueue), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Durable = {0};{1}", b.Durable == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ExactlyOnce = {0};{1}", b.ExactlyOnce == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", b.MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.MaxRetryCycles = {0};{1}", b.MaxRetryCycles, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReceiveContextEnabled = {0};{1}", b.ReceiveContextEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), b.ReceiveErrorHandling), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ReceiveRetryCount = {0};{1}", b.ReceiveRetryCount, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});{1}", b.RetryCycleDelay.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.TimeToLive = new TimeSpan({0});{1}", b.TimeToLive.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UseMSMQTracing = {0};{1}", b.UseMSMQTracing == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UseSourceJournal = {0};{1}", b.UseSourceJournal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.ValidityDuration != TimeSpan.Zero) Code.AppendFormat("\t\t\tthis.ValidityDuration = new TimeSpan({0});{1}", b.ValidityDuration.Ticks, Environment.NewLine);
				if (b.Security != null) Code.AppendFormat("\t\t\tthis.Security = new {0}();{1}", DataTypeCSGenerator.GenerateType(b.Security), Environment.NewLine);
			}
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}
		public static string GenerateCode35Client(Projects.ServiceBinding o)
		{
			return GenerateCode35(o);
		}
		public static string GenerateCode40Client(Projects.ServiceBinding o)
		{
			Type t = o.GetType();
			if (t == typeof(Projects.ServiceBindingWebHTTP)) return "";

			return GenerateCode40(o);
		}

		public static string GenerateBindingBaseType(Projects.ServiceBinding o)
		{
			if (o.InheritedTypes.Count <= 0) return "";
			return DataTypeCSGenerator.GenerateType(o.InheritedTypes[0]);
		}
	}
}