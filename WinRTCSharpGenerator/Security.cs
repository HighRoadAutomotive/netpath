using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFArchitect.Projects;
using WCFArchitect.Projects.Helpers;

namespace WCFArchitect.Generators.WinRT.CS
{
	internal static class SecurityGenerator
	{
		public static string GenerateCode45(BindingSecurity o)
		{
			Type t = o.GetType();
			if (t == typeof(BindingSecurityBasicHTTP)) return SecurityBasicHTTPCSGenerator.GenerateCode45(o as BindingSecurityBasicHTTP);
			if (t == typeof(BindingSecurityTCP)) return SecurityTCPCSGenerator.GenerateCode45(o as BindingSecurityTCP);
			if (t == typeof(BindingSecurityWebHTTP)) return SecurityWebHTTPCSGenerator.GenerateCode45(o as BindingSecurityWebHTTP);
			return "";
		}
	}

	#region - BindingSecurityBasicHTTP Class -

	public static class SecurityBasicHTTPCSGenerator
	{
		public static string GenerateCode30(BindingSecurityBasicHTTP o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode35(BindingSecurityBasicHTTP o)
		{
			return GenerateCode40(o);
		}

		public static string GenerateCode40(BindingSecurityBasicHTTP o)
		{
			return GenerateCode45(o);
		}

		public static string GenerateCode45(BindingSecurityBasicHTTP o)
		{
			var code = new StringBuilder();
			code.AppendLine(string.Format("\t\t\tthis.Security.Mode = BasicHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpSecurityMode), o.Mode)));
			code.AppendLine(string.Format("\t\t\tthis.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), o.TransportProxyCredentialType)));
			return code.ToString();
		}

		public static string GenerateCode35Client(BindingSecurityBasicHTTP o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode40Client(BindingSecurityBasicHTTP o)
		{
			return GenerateCode40(o);
		}
	}

	#endregion

	#region - BindingSecurityTCP Class -

	public static class SecurityTCPCSGenerator
	{
		public static string GenerateCode30(BindingSecurityTCP o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode35(BindingSecurityTCP o)
		{
			return GenerateCode40(o);
		}

		public static string GenerateCode40(BindingSecurityTCP o)
		{
			return GenerateCode45(o);
		}

		public static string GenerateCode45(BindingSecurityTCP o)
		{
			var code = new StringBuilder();
			code.AppendLine(string.Format("\t\t\tthis.Security.Mode = SecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), o.Mode)));
			code.AppendLine(string.Format("\t\t\tthis.Security.Transport.ClientCredentialType = TcpClientCredentialType.{0};", System.Enum.GetName(typeof (System.ServiceModel.TcpClientCredentialType), o.TransportClientCredentialType)));
			code.AppendLine(string.Format("\t\t\tthis.Security.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), o.MessageClientCredentialType)));
			return code.ToString();
		}

		public static string GenerateCode35Client(BindingSecurityTCP o)
		{
			return GenerateCode35(o);
		}

		public static string GenerateCode40Client(BindingSecurityTCP o)
		{
			return GenerateCode40(o);
		}
	}

	#endregion

	#region - BindingSecurityWebHTTP Class -

	public static class SecurityWebHTTPCSGenerator
	{
		public static string GenerateCode30(BindingSecurityWebHTTP o)
		{
			return "";
		}

		public static string GenerateCode35(BindingSecurityWebHTTP o)
		{
			return GenerateCode40(o);
		}

		public static string GenerateCode40(BindingSecurityWebHTTP o)
		{
			return GenerateCode45(o);
		}

		public static string GenerateCode45(BindingSecurityWebHTTP o)
		{
			var code = new StringBuilder();
			code.AppendLine(string.Format("\t\t\tthis.Security.Mode = WebHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.WebHttpSecurityMode), o.Mode)));
			code.AppendLine(string.Format("\t\t\tthis.Security.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), o.TransportClientCredentialType)));
			code.AppendLine(string.Format("\t\t\tthis.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), o.TransportProxyCredentialType)));
			code.AppendLine(string.Format("\t\t\tthis.Security.Transport.Realm = \"{0}\";", o.TransportRealm));
			return code.ToString();
		}

		public static string GenerateCode35Client(BindingSecurityWebHTTP o)
		{										  
			return GenerateCode35(o);			  
		}										  
												  
		public static string GenerateCode40Client(BindingSecurityWebHTTP o)
		{
			return "";
		}
	}

	#endregion

}