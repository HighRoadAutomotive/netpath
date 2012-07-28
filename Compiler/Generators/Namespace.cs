using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFArchitect.Projects;

namespace WCFArchitect.Compiler.Generators
{
	internal static class NamespaceCSGenerator
	{
		public static bool VerifyCode(Namespace o)
		{
			bool NoErrors = true;

			if (o.URI == null || o.URI == "")
			{
				Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS1000", "The '" + o.Name + "' namespace in the '" + o.Owner.Name + "' project does not have a URI. Namespaces MUST have a URI.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchHTTPURI.IsMatch(o.URI) == false)
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS1001", "The URI '" + o.URI + "' for the '" + o.Name + "' namespace in the '" + o.Owner.Name + "' project is not a valid URI. Any associated services and data may not function properly.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, o.Parent, o, o.GetType()));

			foreach (Projects.Enum EE in o.Enums)
				if (EnumCSGenerator.VerifyCode(EE) == false) NoErrors = false;

			foreach (Data DE in o.Data)
				if (DataCSGenerator.VerifyCode(DE) == false) NoErrors = false;

			foreach (Service SE in o.Services)
				if (ServiceCSGenerator.VerifyCode(SE) == false) NoErrors = false;

			foreach (Namespace TN in o.Children)
				if (NamespaceCSGenerator.VerifyCode(TN) == false) NoErrors = false;

			return NoErrors;
		}

		public static string GenerateServerCode30(Namespace o)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumerations");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Enum EE in o.Enums)
					Code.AppendLine(EnumCSGenerator.GenerateServerCode30(EE));
				Code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in o.Data)
					Code.AppendLine(DataCSGenerator.GenerateServerCode30(DE));
				Code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in o.Services)
					Code.AppendLine(ServiceCSGenerator.GenerateServerCode30(SE));
				Code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.ServiceBinding SB in o.Bindings)
					Code.AppendLine(BindingsCSGenerator.GenerateCode30(SB));
				Code.AppendLine();
			}

			if (o.Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.BindingSecurity BS in o.Security)
					Code.AppendLine(SecurityCSGenerator.GenerateCode30(BS));
				Code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Hosts");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Host HE in o.Hosts)
					Code.AppendLine(HostCSGenerator.GenerateServerCode30(HE));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in o.Children)
				Code.AppendLine(NamespaceCSGenerator.GenerateServerCode30(TN));

			return Code.ToString();
		}

		public static string GenerateServerCode35(Namespace o)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumerations");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Enum EE in o.Enums)
					Code.AppendLine(EnumCSGenerator.GenerateServerCode35(EE));
				Code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in o.Data)
					Code.AppendLine(DataCSGenerator.GenerateServerCode35(DE));
				Code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in o.Services)
					Code.AppendLine(ServiceCSGenerator.GenerateServerCode35(SE));
				Code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.ServiceBinding SB in o.Bindings)
					Code.AppendLine(BindingsCSGenerator.GenerateCode35(SB));
				Code.AppendLine();
			}

			if (o.Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.BindingSecurity BS in o.Security)
					Code.AppendLine(SecurityCSGenerator.GenerateCode35(BS));
				Code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Hosts");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Host HE in o.Hosts)
					Code.AppendLine(HostCSGenerator.GenerateServerCode35(HE));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in o.Children)
				Code.AppendLine(NamespaceCSGenerator.GenerateServerCode35(TN));

			return Code.ToString();
		}

		public static string GenerateServerCode35Client(Namespace o)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumerations");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Enum EE in o.Enums)
					Code.AppendLine(EnumCSGenerator.GenerateServerCode35(EE));
				Code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in o.Data)
					Code.AppendLine(DataCSGenerator.GenerateServerCode35(DE));
				Code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in o.Services)
					Code.AppendLine(ServiceCSGenerator.GenerateServerCode35(SE));
				Code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.ServiceBinding SB in o.Bindings)
					Code.AppendLine(BindingsCSGenerator.GenerateCode35Client(SB));
				Code.AppendLine();
			}

			if (o.Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.BindingSecurity BS in o.Security)
					Code.AppendLine(SecurityCSGenerator.GenerateCode35Client(BS));
				Code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Hosts");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Host HE in o.Hosts)
					Code.AppendLine(HostCSGenerator.GenerateServerCode35Client(HE));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in o.Children)
				Code.AppendLine(NamespaceCSGenerator.GenerateServerCode35Client(TN));

			return Code.ToString();
		}

		public static string GenerateServerCode40(Namespace o)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumerations");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Enum EE in o.Enums)
					Code.AppendLine(EnumCSGenerator.GenerateServerCode40(EE));
				Code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in o.Data)
					Code.AppendLine(DataCSGenerator.GenerateServerCode40(DE));
				Code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in o.Services)
					Code.AppendLine(ServiceCSGenerator.GenerateServerCode40(SE));
				Code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.ServiceBinding SB in o.Bindings)
					Code.AppendLine(BindingsCSGenerator.GenerateCode40(SB));
				Code.AppendLine();
			}

			if (o.Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.BindingSecurity BS in o.Security)
					Code.AppendLine(SecurityCSGenerator.GenerateCode40(BS));
				Code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Hosts");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Host HE in o.Hosts)
					Code.AppendLine(HostCSGenerator.GenerateServerCode40(HE));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in o.Children)
				Code.AppendLine(NamespaceCSGenerator.GenerateServerCode40(TN));

			return Code.ToString();
		}

		public static string GenerateServerCode45(Namespace o)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumerations");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Enum EE in o.Enums)
					Code.AppendLine(EnumCSGenerator.GenerateServerCode45(EE));
				Code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in o.Data)
					Code.AppendLine(DataCSGenerator.GenerateServerCode45(DE));
				Code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in o.Services)
					Code.AppendLine(ServiceCSGenerator.GenerateServerCode45(SE));
				Code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.ServiceBinding SB in o.Bindings)
					Code.AppendLine(BindingsCSGenerator.GenerateCode45(SB));
				Code.AppendLine();
			}

			if (o.Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.BindingSecurity BS in o.Security)
					Code.AppendLine(SecurityCSGenerator.GenerateCode45(BS));
				Code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Hosts");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Host HE in o.Hosts)
					Code.AppendLine(HostCSGenerator.GenerateServerCode45(HE));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in o.Children)
				Code.AppendLine(NamespaceCSGenerator.GenerateServerCode45(TN));

			return Code.ToString();
		}

		public static string GenerateServerCode40Client(Namespace o)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumerations");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Enum EE in o.Enums)
					Code.AppendLine(EnumCSGenerator.GenerateServerCode40(EE));
				Code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in o.Data)
					Code.AppendLine(DataCSGenerator.GenerateServerCode40(DE));
				Code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in o.Services)
					Code.AppendLine(ServiceCSGenerator.GenerateServerCode40(SE));
				Code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.ServiceBinding SB in o.Bindings)
					Code.AppendLine(BindingsCSGenerator.GenerateCode40Client(SB));
				Code.AppendLine();
			}

			if (o.Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.BindingSecurity BS in o.Security)
					Code.AppendLine(SecurityCSGenerator.GenerateCode40Client(BS));
				Code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Hosts");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Host HE in o.Hosts)
					Code.AppendLine(HostCSGenerator.GenerateServerCode40Client(HE));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in o.Children)
				Code.AppendLine(NamespaceCSGenerator.GenerateServerCode40Client(TN));

			return Code.ToString();
		}

		public static string GenerateClientCode30(Namespace o)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumeration Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Enum EE in o.Enums)
					Code.AppendLine(EnumCSGenerator.GenerateProxyCode30(EE));
				Code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in o.Data)
				{
					Code.AppendLine(DataCSGenerator.GenerateProxyCode30(DE));
					Code.AppendLine(DataCSGenerator.GenerateWPFCode30(DE));
				}
				Code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in o.Services)
					Code.AppendLine(ServiceCSGenerator.GenerateClientCode30(SE));
				Code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.ServiceBinding SB in o.Bindings)
					Code.AppendLine(BindingsCSGenerator.GenerateCode30(SB));
				Code.AppendLine();
			}

			if (o.Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.BindingSecurity BS in o.Security)
					Code.AppendLine(SecurityCSGenerator.GenerateCode30(BS));
				Code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Hosts");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Host HE in o.Hosts)
					Code.AppendLine(HostCSGenerator.GenerateClientCode30(HE));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in o.Children)
				Code.AppendLine(NamespaceCSGenerator.GenerateClientCode30(TN));

			return Code.ToString();
		}

		public static string GenerateClientCode35(Namespace o)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumeration Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Enum EE in o.Enums)
					Code.AppendLine(EnumCSGenerator.GenerateProxyCode35(EE));
				Code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in o.Data)
				{
					Code.AppendLine(DataCSGenerator.GenerateProxyCode35(DE));
					Code.AppendLine(DataCSGenerator.GenerateWPFCode35(DE));
				}
				Code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in o.Services)
					Code.AppendLine(ServiceCSGenerator.GenerateClientCode35(SE));
				Code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.ServiceBinding SB in o.Bindings)
					Code.AppendLine(BindingsCSGenerator.GenerateCode35(SB));
				Code.AppendLine();
			}

			if (o.Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.BindingSecurity BS in o.Security)
					Code.AppendLine(SecurityCSGenerator.GenerateCode35(BS));
				Code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Hosts");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Host HE in o.Hosts)
					Code.AppendLine(HostCSGenerator.GenerateClientCode35(HE));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in o.Children)
				Code.AppendLine(NamespaceCSGenerator.GenerateClientCode35(TN));

			return Code.ToString();
		}

		public static string GenerateClientCode35Client(Namespace o)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumeration Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Enum EE in o.Enums)
					Code.AppendLine(EnumCSGenerator.GenerateProxyCode35(EE));
				Code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in o.Data)
				{
					Code.AppendLine(DataCSGenerator.GenerateProxyCode35(DE));
					Code.AppendLine(DataCSGenerator.GenerateWPFCode35(DE));
				}
				Code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in o.Services)
					Code.AppendLine(ServiceCSGenerator.GenerateClientCode35(SE));
				Code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.ServiceBinding SB in o.Bindings)
					Code.AppendLine(BindingsCSGenerator.GenerateCode35Client(SB));
				Code.AppendLine();
			}

			if (o.Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.BindingSecurity BS in o.Security)
					Code.AppendLine(SecurityCSGenerator.GenerateCode35Client(BS));
				Code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Hosts");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Host HE in o.Hosts)
					Code.AppendLine(HostCSGenerator.GenerateClientCode35Client(HE));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in o.Children)
				Code.AppendLine(NamespaceCSGenerator.GenerateClientCode35Client(TN));

			return Code.ToString();
		}

		public static string GenerateClientCode40(Namespace o)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumeration Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Enum EE in o.Enums)
					Code.AppendLine(EnumCSGenerator.GenerateProxyCode40(EE));
				Code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in o.Data)
				{
					Code.AppendLine(DataCSGenerator.GenerateProxyCode40(DE));
					Code.AppendLine(DataCSGenerator.GenerateWPFCode40(DE));
				}
				Code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in o.Services)
					Code.AppendLine(ServiceCSGenerator.GenerateClientCode40(SE));
				Code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.ServiceBinding SB in o.Bindings)
					Code.AppendLine(BindingsCSGenerator.GenerateCode40(SB));
				Code.AppendLine();
			}

			if (o.Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.BindingSecurity BS in o.Security)
					Code.AppendLine(SecurityCSGenerator.GenerateCode40(BS));
				Code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Hosts");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Host HE in o.Hosts)
					Code.AppendLine(HostCSGenerator.GenerateClientCode40(HE));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in o.Children)
				Code.AppendLine(NamespaceCSGenerator.GenerateClientCode40(TN));

			return Code.ToString();
		}

		public static string GenerateClientCode40Client(Namespace o)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumeration Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Enum EE in o.Enums)
					Code.AppendLine(EnumCSGenerator.GenerateProxyCode40(EE));
				Code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in o.Data)
				{
					Code.AppendLine(DataCSGenerator.GenerateProxyCode40(DE));
					Code.AppendLine(DataCSGenerator.GenerateWPFCode40(DE));
				}
				Code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in o.Services)
				{
					Code.AppendLine(ServiceCSGenerator.GenerateClientCode40(SE));
				}
				Code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.ServiceBinding SB in o.Bindings)
					Code.AppendLine(BindingsCSGenerator.GenerateCode40Client(SB));
				Code.AppendLine();
			}

			if (o.Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.BindingSecurity BS in o.Security)
					Code.AppendLine(SecurityCSGenerator.GenerateCode40Client(BS));
				Code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Hosts");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Host HE in o.Hosts)
					Code.AppendLine(HostCSGenerator.GenerateClientCode40Client(HE));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in o.Children)
				Code.AppendLine(NamespaceCSGenerator.GenerateClientCode40Client(TN));

			return Code.ToString();
		}

		public static string GenerateClientCode45(Namespace o)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumeration Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Enum EE in o.Enums)
					Code.AppendLine(EnumCSGenerator.GenerateProxyCode45(EE));
				Code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in o.Data)
				{
					Code.AppendLine(DataCSGenerator.GenerateProxyCode45(DE));
					Code.AppendLine(DataCSGenerator.GenerateWPFCode45(DE));
				}
				Code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in o.Services)
					Code.AppendLine(ServiceCSGenerator.GenerateClientCode45(SE));
				Code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Bindings");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.ServiceBinding SB in o.Bindings)
					Code.AppendLine(BindingsCSGenerator.GenerateCode45(SB));
				Code.AppendLine();
			}

			if (o.Security.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tBinding Security");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.BindingSecurity BS in o.Security)
					Code.AppendLine(SecurityCSGenerator.GenerateCode45(BS));
				Code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				Code.AppendLine("/**************************************************************************");
				Code.AppendLine("*\tService Hosts");
				Code.AppendLine("**************************************************************************/");
				Code.AppendLine();
				foreach (Projects.Host HE in o.Hosts)
					Code.AppendLine(HostCSGenerator.GenerateClientCode45(HE));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in o.Children)
				Code.AppendLine(NamespaceCSGenerator.GenerateClientCode45(TN));

			return Code.ToString();
		}
	}
}