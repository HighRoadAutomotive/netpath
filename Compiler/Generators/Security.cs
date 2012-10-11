using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFArchitect.Projects;

namespace WCFArchitect.Compiler.Generators
{
	internal static class SecurityCSGenerator
	{
		public static void VerifyCode(Projects.BindingSecurity o)
		{
			if (string.IsNullOrEmpty(o.Name))
				Program.AddMessage(new CompileMessage("GS7000", "A binding security element in the '" + o.Parent.Name + "' project has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
			else
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.Name) == false)
					Program.AddMessage(new CompileMessage("GS7001", "The binding security element '" + o.Name + "' in the '" + o.Parent.Name + "' project contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
		}

		public static string GenerateCode30(Projects.BindingSecurity o)
		{
			Type t = o.GetType();
			if (t == typeof(Projects.BindingSecurityBasicHTTP)) return SecurityBasicHTTPCSGenerator.GenerateCode30(o as Projects.BindingSecurityBasicHTTP);
			if (t == typeof(Projects.BindingSecurityWSHTTP)) return SecurityWSHTTPCSGenerator.GenerateCode30(o as Projects.BindingSecurityWSHTTP);
			if (t == typeof(Projects.BindingSecurityWSDualHTTP)) return SecurityWSDualHTTPCSGenerator.GenerateCode30(o as Projects.BindingSecurityWSDualHTTP);
			if (t == typeof(Projects.BindingSecurityWSFederationHTTP)) return SecurityWSFederationHTTPCSGenerator.GenerateCode30(o as Projects.BindingSecurityWSFederationHTTP);
			if (t == typeof(Projects.BindingSecurityTCP)) return SecurityTCPCSGenerator.GenerateCode30(o as Projects.BindingSecurityTCP);
			if (t == typeof(Projects.BindingSecurityNamedPipe)) return SecurityNamedPipeCSGenerator.GenerateCode30(o as Projects.BindingSecurityNamedPipe);
			if (t == typeof(Projects.BindingSecurityMSMQ)) return SecurityMSMQCSGenerator.GenerateCode30(o as Projects.BindingSecurityMSMQ);
			if (t == typeof(Projects.BindingSecurityPeerTCP)) return SecurityPeerTCPCSGenerator.GenerateCode30(o as Projects.BindingSecurityPeerTCP);
			if (t == typeof(Projects.BindingSecurityWebHTTP)) return SecurityWebHTTPCSGenerator.GenerateCode30(o as Projects.BindingSecurityWebHTTP);
			if (t == typeof(Projects.BindingSecurityMSMQIntegration)) return SecurityMSMQIntegrationCSGenerator.GenerateCode30(o as Projects.BindingSecurityMSMQIntegration);
			return "";
		}
		public static string GenerateCode35(Projects.BindingSecurity o)
		{
			Type t = o.GetType();
			if (t == typeof(Projects.BindingSecurityBasicHTTP)) return SecurityBasicHTTPCSGenerator.GenerateCode35(o as Projects.BindingSecurityBasicHTTP);
			if (t == typeof(Projects.BindingSecurityWSHTTP)) return SecurityWSHTTPCSGenerator.GenerateCode35(o as Projects.BindingSecurityWSHTTP);
			if (t == typeof(Projects.BindingSecurityWSDualHTTP)) return SecurityWSDualHTTPCSGenerator.GenerateCode35(o as Projects.BindingSecurityWSDualHTTP);
			if (t == typeof(Projects.BindingSecurityWSFederationHTTP)) return SecurityWSFederationHTTPCSGenerator.GenerateCode35(o as Projects.BindingSecurityWSFederationHTTP);
			if (t == typeof(Projects.BindingSecurityTCP)) return SecurityTCPCSGenerator.GenerateCode35(o as Projects.BindingSecurityTCP);
			if (t == typeof(Projects.BindingSecurityNamedPipe)) return SecurityNamedPipeCSGenerator.GenerateCode35(o as Projects.BindingSecurityNamedPipe);
			if (t == typeof(Projects.BindingSecurityMSMQ)) return SecurityMSMQCSGenerator.GenerateCode35(o as Projects.BindingSecurityMSMQ);
			if (t == typeof(Projects.BindingSecurityPeerTCP)) return SecurityPeerTCPCSGenerator.GenerateCode35(o as Projects.BindingSecurityPeerTCP);
			if (t == typeof(Projects.BindingSecurityWebHTTP)) return SecurityWebHTTPCSGenerator.GenerateCode35(o as Projects.BindingSecurityWebHTTP);
			if (t == typeof(Projects.BindingSecurityMSMQIntegration)) return SecurityMSMQIntegrationCSGenerator.GenerateCode35(o as Projects.BindingSecurityMSMQIntegration);
			return "";
		}
		public static string GenerateCode40(Projects.BindingSecurity o)
		{
			Type t = o.GetType();
			if (t == typeof(Projects.BindingSecurityBasicHTTP)) return SecurityBasicHTTPCSGenerator.GenerateCode40(o as Projects.BindingSecurityBasicHTTP);
			if (t == typeof(Projects.BindingSecurityWSHTTP)) return SecurityWSHTTPCSGenerator.GenerateCode40(o as Projects.BindingSecurityWSHTTP);
			if (t == typeof(Projects.BindingSecurityWSDualHTTP)) return SecurityWSDualHTTPCSGenerator.GenerateCode40(o as Projects.BindingSecurityWSDualHTTP);
			if (t == typeof(Projects.BindingSecurityWSFederationHTTP)) return SecurityWSFederationHTTPCSGenerator.GenerateCode40(o as Projects.BindingSecurityWSFederationHTTP);
			if (t == typeof(Projects.BindingSecurityTCP)) return SecurityTCPCSGenerator.GenerateCode40(o as Projects.BindingSecurityTCP);
			if (t == typeof(Projects.BindingSecurityNamedPipe)) return SecurityNamedPipeCSGenerator.GenerateCode40(o as Projects.BindingSecurityNamedPipe);
			if (t == typeof(Projects.BindingSecurityMSMQ)) return SecurityMSMQCSGenerator.GenerateCode40(o as Projects.BindingSecurityMSMQ);
			if (t == typeof(Projects.BindingSecurityPeerTCP)) return SecurityPeerTCPCSGenerator.GenerateCode40(o as Projects.BindingSecurityPeerTCP);
			if (t == typeof(Projects.BindingSecurityWebHTTP)) return SecurityWebHTTPCSGenerator.GenerateCode40(o as Projects.BindingSecurityWebHTTP);
			if (t == typeof(Projects.BindingSecurityMSMQIntegration)) return SecurityMSMQIntegrationCSGenerator.GenerateCode40(o as Projects.BindingSecurityMSMQIntegration);
			return "";
		}
		public static string GenerateCode45(Projects.BindingSecurity o)
		{
			Type t = o.GetType();
			if (t == typeof(Projects.BindingSecurityBasicHTTP)) return SecurityBasicHTTPCSGenerator.GenerateCode45(o as Projects.BindingSecurityBasicHTTP);
			if (t == typeof(Projects.BindingSecurityWSHTTP)) return SecurityWSHTTPCSGenerator.GenerateCode45(o as Projects.BindingSecurityWSHTTP);
			if (t == typeof(Projects.BindingSecurityWSDualHTTP)) return SecurityWSDualHTTPCSGenerator.GenerateCode45(o as Projects.BindingSecurityWSDualHTTP);
			if (t == typeof(Projects.BindingSecurityWSFederationHTTP)) return SecurityWSFederationHTTPCSGenerator.GenerateCode45(o as Projects.BindingSecurityWSFederationHTTP);
			if (t == typeof(Projects.BindingSecurityTCP)) return SecurityTCPCSGenerator.GenerateCode45(o as Projects.BindingSecurityTCP);
			if (t == typeof(Projects.BindingSecurityNamedPipe)) return SecurityNamedPipeCSGenerator.GenerateCode45(o as Projects.BindingSecurityNamedPipe);
			if (t == typeof(Projects.BindingSecurityMSMQ)) return SecurityMSMQCSGenerator.GenerateCode45(o as Projects.BindingSecurityMSMQ);
			if (t == typeof(Projects.BindingSecurityPeerTCP)) return SecurityPeerTCPCSGenerator.GenerateCode45(o as Projects.BindingSecurityPeerTCP);
			if (t == typeof(Projects.BindingSecurityWebHTTP)) return SecurityWebHTTPCSGenerator.GenerateCode45(o as Projects.BindingSecurityWebHTTP);
			if (t == typeof(Projects.BindingSecurityMSMQIntegration)) return SecurityMSMQIntegrationCSGenerator.GenerateCode45(o as Projects.BindingSecurityMSMQIntegration);
			return "";
		}
		public static string GenerateCode35Client(Projects.BindingSecurity o)
		{
			Type t = o.GetType();
			if (t == typeof(Projects.BindingSecurityBasicHTTP)) return SecurityBasicHTTPCSGenerator.GenerateCode35Client(o as Projects.BindingSecurityBasicHTTP);
			if (t == typeof(Projects.BindingSecurityWSHTTP)) return SecurityWSHTTPCSGenerator.GenerateCode35Client(o as Projects.BindingSecurityWSHTTP);
			if (t == typeof(Projects.BindingSecurityWSDualHTTP)) return SecurityWSDualHTTPCSGenerator.GenerateCode35Client(o as Projects.BindingSecurityWSDualHTTP);
			if (t == typeof(Projects.BindingSecurityWSFederationHTTP)) return SecurityWSFederationHTTPCSGenerator.GenerateCode35Client(o as Projects.BindingSecurityWSFederationHTTP);
			if (t == typeof(Projects.BindingSecurityTCP)) return SecurityTCPCSGenerator.GenerateCode35Client(o as Projects.BindingSecurityTCP);
			if (t == typeof(Projects.BindingSecurityNamedPipe)) return SecurityNamedPipeCSGenerator.GenerateCode35Client(o as Projects.BindingSecurityNamedPipe);
			if (t == typeof(Projects.BindingSecurityMSMQ)) return SecurityMSMQCSGenerator.GenerateCode35Client(o as Projects.BindingSecurityMSMQ);
			if (t == typeof(Projects.BindingSecurityPeerTCP)) return SecurityPeerTCPCSGenerator.GenerateCode35Client(o as Projects.BindingSecurityPeerTCP);
			if (t == typeof(Projects.BindingSecurityWebHTTP)) return SecurityWebHTTPCSGenerator.GenerateCode35Client(o as Projects.BindingSecurityWebHTTP);
			if (t == typeof(Projects.BindingSecurityMSMQIntegration)) return SecurityMSMQIntegrationCSGenerator.GenerateCode35Client(o as Projects.BindingSecurityMSMQIntegration);
			return "";
		}
		public static string GenerateCode40Client(Projects.BindingSecurity o)
		{
			Type t = o.GetType();
			if (t == typeof(Projects.BindingSecurityBasicHTTP)) return SecurityBasicHTTPCSGenerator.GenerateCode40Client(o as Projects.BindingSecurityBasicHTTP);
			if (t == typeof(Projects.BindingSecurityWSHTTP)) return SecurityWSHTTPCSGenerator.GenerateCode40Client(o as Projects.BindingSecurityWSHTTP);
			if (t == typeof(Projects.BindingSecurityWSDualHTTP)) return SecurityWSDualHTTPCSGenerator.GenerateCode40Client(o as Projects.BindingSecurityWSDualHTTP);
			if (t == typeof(Projects.BindingSecurityWSFederationHTTP)) return SecurityWSFederationHTTPCSGenerator.GenerateCode40Client(o as Projects.BindingSecurityWSFederationHTTP);
			if (t == typeof(Projects.BindingSecurityTCP)) return SecurityTCPCSGenerator.GenerateCode40Client(o as Projects.BindingSecurityTCP);
			if (t == typeof(Projects.BindingSecurityNamedPipe)) return SecurityNamedPipeCSGenerator.GenerateCode40Client(o as Projects.BindingSecurityNamedPipe);
			if (t == typeof(Projects.BindingSecurityMSMQ)) return SecurityMSMQCSGenerator.GenerateCode40Client(o as Projects.BindingSecurityMSMQ);
			if (t == typeof(Projects.BindingSecurityPeerTCP)) return SecurityPeerTCPCSGenerator.GenerateCode40Client(o as Projects.BindingSecurityPeerTCP);
			if (t == typeof(Projects.BindingSecurityWebHTTP)) return SecurityWebHTTPCSGenerator.GenerateCode40Client(o as Projects.BindingSecurityWebHTTP);
			if (t == typeof(Projects.BindingSecurityMSMQIntegration)) return SecurityMSMQIntegrationCSGenerator.GenerateCode40Client(o as Projects.BindingSecurityMSMQIntegration);
			return "";
		}
	}

	#region - BindingSecurityBasicHTTP Class -

	public static class SecurityBasicHTTPCSGenerator
	{
		public static bool VerifyCode(Projects.BindingSecurityBasicHTTP o)
		{
			bool NoErrors = true;

			//Nothing to do here

			return NoErrors;
		}

		public static string GenerateCode30(Projects.BindingSecurityBasicHTTP o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode35(Projects.BindingSecurityBasicHTTP o)
		{
			return GenerateCode40(o);
		}

		public static string GenerateCode40(Projects.BindingSecurityBasicHTTP o)
		{
			return GenerateCode45(o);
		}

		public static string GenerateCode45(Projects.BindingSecurityBasicHTTP o)
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tthis.Mode = BasicHttpSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), o.MessageClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ClientCredentialType = HttpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), o.TransportProxyCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.Realm = \"{0}\";{1}", o.TransportRealm, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tpublic static void SetSecurity(BasicHttpSecurity sec)");
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = BasicHttpSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), o.MessageClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), o.TransportProxyCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";{1}", o.TransportRealm, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public static string GenerateCode35Client(Projects.BindingSecurityBasicHTTP o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode40Client(Projects.BindingSecurityBasicHTTP o)
		{
			return GenerateCode40(o);
		}
	}

	#endregion

	#region - BindingSecurityBasicHTTPS Class -

	public static class SecurityBasicHTTPSCSGenerator
	{
		public static bool VerifyCode(Projects.BindingSecurityBasicHTTPS o)
		{
			bool NoErrors = true;

			//Nothing to do here

			return NoErrors;
		}

		public static string GenerateCode30(Projects.BindingSecurityBasicHTTPS o)
		{
			return "";
		}

		public static string GenerateCode35(Projects.BindingSecurityBasicHTTPS o)
		{
			return "";
		}

		public static string GenerateCode40(Projects.BindingSecurityBasicHTTPS o)
		{
			return "";
		}

		public static string GenerateCode45(Projects.BindingSecurityBasicHTTPS o)
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tthis.Mode = BasicHttpSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), o.MessageClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ClientCredentialType = HttpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), o.TransportProxyCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.Realm = \"{0}\";{1}", o.TransportRealm, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tpublic static void SetSecurity(BasicHttpSecurity sec)");
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = BasicHttpSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), o.MessageClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), o.TransportProxyCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";{1}", o.TransportRealm, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public static string GenerateCode35Client(Projects.BindingSecurityBasicHTTPS o)
		{
			return "";
		}

		public static string GenerateCode40Client(Projects.BindingSecurityBasicHTTPS o)
		{
			return "";
		}
	}

	#endregion

	#region - BindingSecurityWSHTTP Class -

	public static class SecurityWSHTTPCSGenerator
	{
		public static bool VerifyCode(Projects.BindingSecurityWSHTTP o)
		{
			bool NoErrors = true;

			//Nothing to do here

			return NoErrors;
		}

		public static string GenerateCode30(Projects.BindingSecurityWSHTTP o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode35(Projects.BindingSecurityWSHTTP o)
		{
			return GenerateCode40(o);
		}

		public static string GenerateCode40(Projects.BindingSecurityWSHTTP o)
		{
			return GenerateCode45(o);
		}

		public static string GenerateCode45(Projects.BindingSecurityWSHTTP o)
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tthis.Mode = SecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), o.MessageClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.EstablishSecurityContext = {0};{1}", o.MessageEstablishSecurityContext == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.NegotiateServiceCredential = {0};{1}", o.MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ClientCredentialType = HttpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), o.TransportProxyCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.Realm = \"{0}\";{1}", o.TransportRealm, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tpublic static void SetSecurity(WSHttpSecurity sec)");
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = SecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), o.MessageClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.EstablishSecurityContext = {0};{1}", o.MessageEstablishSecurityContext == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.NegotiateServiceCredential = {0};{1}", o.MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), o.TransportProxyCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";{1}", o.TransportRealm, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public static string GenerateCode35Client(Projects.BindingSecurityWSHTTP o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode40Client(Projects.BindingSecurityWSHTTP o)
		{
			return GenerateCode40(o);
		}
	}

	#endregion

	#region - BindingSecurityWSDualHTTP Class -

	public static class SecurityWSDualHTTPCSGenerator
	{
		public static bool VerifyCode(Projects.BindingSecurityWSDualHTTP o)
		{
			bool NoErrors = true;

			//Nothing to do here

			return NoErrors;
		}

		public static string GenerateCode30(Projects.BindingSecurityWSDualHTTP o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode35(Projects.BindingSecurityWSDualHTTP o)
		{
			return GenerateCode40(o);
		}

		public static string GenerateCode40(Projects.BindingSecurityWSDualHTTP o)
		{
			return GenerateCode45(o);
		}

		public static string GenerateCode45(Projects.BindingSecurityWSDualHTTP o)
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tthis.Mode = WSDualHttpSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.WSDualHttpSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), o.MessageClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.NegotiateServiceCredential = {0};{1}", o.MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tpublic static void SetSecurity(WSDualHttpSecurity sec)");
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = WSDualHttpSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.WSDualHttpSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), o.MessageClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.NegotiateServiceCredential = {0};{1}", o.MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public static string GenerateCode35Client(Projects.BindingSecurityWSDualHTTP o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode40Client(Projects.BindingSecurityWSDualHTTP o)
		{
			return GenerateCode40(o);
		}
	}

	#endregion

	#region - BindingSecurityWSFederationHTTP Class -

	public static class SecurityWSFederationHTTPCSGenerator
	{
		public static bool VerifyCode(Projects.BindingSecurityWSFederationHTTP o)
		{
			bool NoErrors = true;

			//Nothing to do here

			return NoErrors;
		}

		public static string GenerateCode30(Projects.BindingSecurityWSFederationHTTP o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode35(Projects.BindingSecurityWSFederationHTTP o)
		{
			return GenerateCode40(o);
		}

		public static string GenerateCode40(Projects.BindingSecurityWSFederationHTTP o)
		{
			return GenerateCode45(o);
		}

		public static string GenerateCode45(Projects.BindingSecurityWSFederationHTTP o)
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tthis.Mode = WSFederationHttpSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.WSFederationHttpSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.EstablishSecurityContext = {0};{1}", o.MessageEstablishSecurityContext == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.IssuedKeyType = System.IdentityModel.Tokens.SecurityKeyType.{0};{1}", System.Enum.GetName(typeof(System.IdentityModel.Tokens.SecurityKeyType), o.MessageIssuedKeyType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.IssuedTokenType = \"{0}\";{1}", o.MessageIssuedTokenType, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.IssuerAddress = new EndpointAddress(\"{0}\");{1}", o.MessageIssuerAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.IssuerMetadataAddress = new EndpointAddress(\"{0}\");{1}", o.MessageIssuerMetadataAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.NegotiateServiceCredential = {0};{1}", o.MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tpublic static void SetSecurity(WSFederationHttpSecurity sec)");
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = WSFederationHttpSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.WSFederationHttpSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.EstablishSecurityContext = {0};{1}", o.MessageEstablishSecurityContext == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.IssuedKeyType = System.IdentityModel.Tokens.SecurityKeyType.{0};{1}", System.Enum.GetName(typeof(System.IdentityModel.Tokens.SecurityKeyType), o.MessageIssuedKeyType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.IssuedTokenType = \"{0}\";{1}", o.MessageIssuedTokenType, Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.IssuerAddress = new EndpointAddress(\"{0}\");{1}", o.MessageIssuerAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.IssuerMetadataAddress = new EndpointAddress(\"{0}\");{1}", o.MessageIssuerMetadataAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.NegotiateServiceCredential = {0};{1}", o.MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public static string GenerateCode35Client(Projects.BindingSecurityWSFederationHTTP o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode40Client(Projects.BindingSecurityWSFederationHTTP o)
		{
			return GenerateCode40(o);
		}
	}

	#endregion

	#region - BindingSecurityTCP Class -

	public static class SecurityTCPCSGenerator
	{
		public static bool VerifyCode(Projects.BindingSecurityTCP o)
		{
			bool NoErrors = true;

			//Nothing to do here

			return NoErrors;
		}

		public static string GenerateCode30(Projects.BindingSecurityTCP o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode35(Projects.BindingSecurityTCP o)
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tthis.Mode = SecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.ClientCredentialType = MessageCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), o.MessageClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ClientCredentialType = TcpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ProtectionLevel = ProtectionLevel.{0};{1}", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.TransportProtectionLevel), Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tpublic static void SetSecurity(NetTcpSecurity sec)");
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = SecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), o.MessageClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = TcpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};{1}", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.TransportProtectionLevel), Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public static string GenerateCode40(Projects.BindingSecurityTCP o)
		{
			return GenerateCode45(o);
		}

		public static string GenerateCode45(Projects.BindingSecurityTCP o)
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tthis.Mode = SecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.ClientCredentialType = MessageCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), o.MessageClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ClientCredentialType = TcpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ProtectionLevel = ProtectionLevel.{0};{1}", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.TransportProtectionLevel), Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy Policy){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tthis.Mode = SecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.ClientCredentialType = MessageCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), o.MessageClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ClientCredentialType = TcpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ProtectionLevel = ProtectionLevel.{0};{1}", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.TransportProtectionLevel), Environment.NewLine);
			Code.AppendLine("\t\t\tthis.Transport.ExtendedProtectionPolicy = Policy;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tpublic static void SetSecurity(NetTcpSecurity sec)");
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = SecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), o.MessageClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = TcpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};{1}", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.TransportProtectionLevel), Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tpublic static void SetSecurity(NetTcpSecurity sec, System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy Policy)");
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = SecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), o.MessageClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = TcpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};{1}", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.TransportProtectionLevel), Environment.NewLine);
			Code.AppendLine("\t\t\tsec.Transport.ExtendedProtectionPolicy = Policy;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public static string GenerateCode35Client(Projects.BindingSecurityTCP o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode40Client(Projects.BindingSecurityTCP o)
		{
			return GenerateCode40(o);
		}
	}

	#endregion

	#region - BindingSecurityNamedPipe Class -

	public static class SecurityNamedPipeCSGenerator
	{
		public static bool VerifyCode(Projects.BindingSecurityNamedPipe o)
		{
			bool NoErrors = true;

			//Nothing to do here

			return NoErrors;
		}

		public static string GenerateCode30(Projects.BindingSecurityNamedPipe o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode35(Projects.BindingSecurityNamedPipe o)
		{
			return GenerateCode40(o);
		}

		public static string GenerateCode40(Projects.BindingSecurityNamedPipe o)
		{
			return GenerateCode45(o);
		}

		public static string GenerateCode45(Projects.BindingSecurityNamedPipe o)
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tthis.Mode = NetNamedPipeSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.NetNamedPipeSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ProtectionLevel = ProtectionLevel.{0};{1}", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.TransportProtectionLevel), Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tpublic static void SetSecurity(NetNamedPipeSecurity sec)");
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = NetNamedPipeSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.NetNamedPipeSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};{1}", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.TransportProtectionLevel), Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public static string GenerateCode35Client(Projects.BindingSecurityNamedPipe o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode40Client(Projects.BindingSecurityNamedPipe o)
		{
			return GenerateCode40(o);
		}
	}

	#endregion

	#region - BindingSecurityMSMQ Class -

	public static class SecurityMSMQCSGenerator
	{
		public static bool VerifyCode(Projects.BindingSecurityMSMQ o)
		{
			bool NoErrors = true;

			//Nothing to do here

			return NoErrors;
		}

		public static string GenerateCode30(Projects.BindingSecurityMSMQ o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode35(Projects.BindingSecurityMSMQ o)
		{
			return GenerateCode40(o);
		}

		public static string GenerateCode40(Projects.BindingSecurityMSMQ o)
		{
			return GenerateCode45(o);
		}

		public static string GenerateCode45(Projects.BindingSecurityMSMQ o)
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tthis.Mode = NetMsmqSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.NetMsmqSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Message.ClientCredentialType = MessageCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), o.MessageClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MsmqAuthenticationMode), o.TransportAuthenticationMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MsmqEncryptionAlgorithm), o.TransportEncryptionAlgorithm), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.MsmqProtectionLevel = MsmqProtectionLevel.{0};{1}", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.TransportProtectionLevel), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MsmqSecureHashAlgorithm), o.TransportSecureHashAlgorithm), Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tpublic static void SetSecurity(NetMsmqSecurity sec)");
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = NetMsmqSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.NetMsmqSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};{1}", System.Enum.GetName(typeof(Projects.BindingSecurityAlgorithmSuite), o.MessageAlgorithmSuite), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), o.MessageClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MsmqAuthenticationMode), o.TransportAuthenticationMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MsmqEncryptionAlgorithm), o.TransportEncryptionAlgorithm), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.MsmqProtectionLevel = MsmqProtectionLevel.{0};{1}", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.TransportProtectionLevel), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MsmqSecureHashAlgorithm), o.TransportSecureHashAlgorithm), Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public static string GenerateCode35Client(Projects.BindingSecurityMSMQ o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode40Client(Projects.BindingSecurityMSMQ o)
		{
			return GenerateCode40(o);
		}
	}

	#endregion

	#region - BindingSecurityPeerTCP Class -

	public static class SecurityPeerTCPCSGenerator
	{
		public static bool VerifyCode(Projects.BindingSecurityPeerTCP o)
		{
			bool NoErrors = true;

			//Nothing to do here

			return NoErrors;
		}

		public static string GenerateCode30(Projects.BindingSecurityPeerTCP o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode35(Projects.BindingSecurityPeerTCP o)
		{
			return GenerateCode40(o);
		}

		public static string GenerateCode40(Projects.BindingSecurityPeerTCP o)
		{
			return GenerateCode45(o);
		}

		public static string GenerateCode45(Projects.BindingSecurityPeerTCP o)
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tthis.Mode = PeerTransportCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.PeerTransportCredentialType), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.CredentialType = PeerTransportCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.PeerTransportCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tpublic static void SetSecurity(PeerSecuritySettings sec)");
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = PeerTransportCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.PeerTransportCredentialType), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.CredentialType = PeerTransportCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.PeerTransportCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public static string GenerateCode35Client(Projects.BindingSecurityPeerTCP o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode40Client(Projects.BindingSecurityPeerTCP o)
		{
			return GenerateCode40(o);
		}
	}

	#endregion

	#region - BindingSecurityWebHTTP Class -

	public static class SecurityWebHTTPCSGenerator
	{
		public static bool VerifyCode(Projects.BindingSecurityWebHTTP o)
		{
			bool NoErrors = true;

			//Nothing to do here

			return NoErrors;
		}

		public static string GenerateCode30(Projects.BindingSecurityWebHTTP o)
		{
			return "";
		}

		public static string GenerateCode35(Projects.BindingSecurityWebHTTP o)
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tthis.Mode = WebHttpSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.WebHttpSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ClientCredentialType = HttpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), o.TransportProxyCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.Realm = \"{0}\";{1}", o.TransportRealm, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tpublic static void SetSecurity(WebHttpSecurity sec)");
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = WebHttpSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.WebHttpSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), o.TransportProxyCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";{1}", o.TransportRealm, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public static string GenerateCode40(Projects.BindingSecurityWebHTTP o)
		{
			return GenerateCode45(o);
		}

		public static string GenerateCode45(Projects.BindingSecurityWebHTTP o)
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tthis.Mode = WebHttpSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.WebHttpSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ClientCredentialType = HttpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), o.TransportProxyCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.Realm = \"{0}\";{1}", o.TransportRealm, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy Policy){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tthis.Mode = WebHttpSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.WebHttpSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ClientCredentialType = HttpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), o.TransportProxyCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.Realm = \"{0}\";{1}", o.TransportRealm, Environment.NewLine);
			Code.AppendLine("\t\t\tthis.Transport.ExtendedProtectionPolicy = Policy;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tpublic static void SetSecurity(WebHttpSecurity sec)");
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = WebHttpSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.WebHttpSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), o.TransportProxyCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";{1}", o.TransportRealm, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tpublic static void SetSecurity(WebHttpSecurity sec, System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy Policy)");
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = WebHttpSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.WebHttpSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), o.TransportClientCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), o.TransportProxyCredentialType), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";{1}", o.TransportRealm, Environment.NewLine);
			Code.AppendLine("\t\t\tsec.Transport.ExtendedProtectionPolicy = Policy;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public static string GenerateCode35Client(Projects.BindingSecurityWebHTTP o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode40Client(Projects.BindingSecurityWebHTTP o)
		{
			return "";
		}
	}

	#endregion

	#region - BindingSecurityMSMQIntegration Class -

	public static class SecurityMSMQIntegrationCSGenerator
	{
		public static bool VerifyCode(Projects.BindingSecurityMSMQIntegration o)
		{
			bool NoErrors = true;

			//Nothing to do here

			return NoErrors;
		}

		public static string GenerateCode30(Projects.BindingSecurityMSMQIntegration o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode35(Projects.BindingSecurityMSMQIntegration o)
		{
			return GenerateCode40(o);
		}

		public static string GenerateCode40(Projects.BindingSecurityMSMQIntegration o)
		{
			return GenerateCode45(o);
		}

		public static string GenerateCode45(Projects.BindingSecurityMSMQIntegration o)
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tthis.Mode = NetMsmqSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MsmqAuthenticationMode), o.TransportAuthenticationMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MsmqEncryptionAlgorithm), o.TransportEncryptionAlgorithm), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.MsmqProtectionLevel = MsmqProtectionLevel.{0};{1}", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.TransportProtectionLevel), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Transport.MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MsmqSecureHashAlgorithm), o.TransportSecureHashAlgorithm), Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tpublic static void SetSecurity(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity sec)");
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = NetMsmqSecurityMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode), o.Mode), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MsmqAuthenticationMode), o.TransportAuthenticationMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MsmqEncryptionAlgorithm), o.TransportEncryptionAlgorithm), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.MsmqProtectionLevel = MsmqProtectionLevel.{0};{1}", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.TransportProtectionLevel), Environment.NewLine);
			Code.AppendFormat("\t\t\tsec.Transport.MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.MsmqSecureHashAlgorithm), o.TransportSecureHashAlgorithm), Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public static string GenerateCode35Client(Projects.BindingSecurityMSMQIntegration o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode40Client(Projects.BindingSecurityMSMQIntegration o)
		{
			return GenerateCode40(o);
		}
	}
	#endregion

}