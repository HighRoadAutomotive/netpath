using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFArchitect.Projects;
using WCFArchitect.Projects.Helpers;

namespace WCFArchitect.Compiler.Generators
{
	internal static class NamespaceCSGenerator
	{
		public static void VerifyCode(Namespace o)
		{
			if (string.IsNullOrEmpty(o.URI))
				Program.AddMessage(new CompileMessage("GS1000", "The '" + o.Name + "' namespace in the '" + o.Owner.Name + "' project does not have a URI. Namespaces MUST have a URI.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
			else
				if (RegExs.MatchHTTPURI.IsMatch(o.URI) == false)
					Program.AddMessage(new CompileMessage("GS1001", "The URI '" + o.URI + "' for the '" + o.Name + "' namespace in the '" + o.Owner.Name + "' project is not a valid URI. Any associated services and data may not function properly.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));

			foreach (Projects.Enum ee in o.Enums)
				EnumCSGenerator.VerifyCode(ee);

			foreach (Data de in o.Data)
				DataCSGenerator.VerifyCode(de);

			foreach (Service se in o.Services)
				ServiceCSGenerator.VerifyCode(se);

			foreach (Host se in o.Hosts)
				HostCSGenerator.VerifyCode(se);

			foreach (ServiceBinding se in o.Bindings)
				BindingsCSGenerator.VerifyCode(se);

			foreach (Namespace tn in o.Children)
				VerifyCode(tn);

		}

		public static string GenerateContractNamespaceAttributes(Namespace o)
		{
			var code = new StringBuilder();

			code.AppendLine(string.Format("[assembly: System.Runtime.Serialization.ContractNamespaceAttribute(\"{0}\", ClrNamespace=\"{1}\")]", o.FullURI, o.FullName));

			foreach (Namespace tn in o.Children)
				code.AppendLine(GenerateContractNamespaceAttributes(tn));

			return code.ToString();
		}

		public static string GenerateServerCode30(Namespace o)
		{
			var code = new StringBuilder();

			code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tEnumerations");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Projects.Enum ee in o.Enums)
					code.AppendLine(EnumCSGenerator.GenerateServerCode30(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
					code.AppendLine(DataCSGenerator.GenerateServerCode30(de));
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
					code.AppendLine(ServiceCSGenerator.GenerateServerCode30(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsCSGenerator.GenerateCode30(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostCSGenerator.GenerateServerCode30(he));
				code.AppendLine();
			}

			code.AppendLine("}");

			foreach (Namespace tn in o.Children)
				code.AppendLine(GenerateServerCode30(tn));

			return code.ToString();
		}

		public static string GenerateServerCode35(Namespace o)
		{
			var code = new StringBuilder();

			code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tEnumerations");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Projects.Enum ee in o.Enums)
					code.AppendLine(EnumCSGenerator.GenerateServerCode35(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
					code.AppendLine(DataCSGenerator.GenerateServerCode35(de));
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
					code.AppendLine(ServiceCSGenerator.GenerateServerCode35(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsCSGenerator.GenerateCode35(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostCSGenerator.GenerateServerCode35(he));
				code.AppendLine();
			}

			code.AppendLine("}");

			foreach (Namespace tn in o.Children)
				code.AppendLine(GenerateServerCode35(tn));

			return code.ToString();
		}

		public static string GenerateServerCode35Client(Namespace o)
		{
			var code = new StringBuilder();

			code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tEnumerations");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Projects.Enum ee in o.Enums)
					code.AppendLine(EnumCSGenerator.GenerateServerCode35(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
					code.AppendLine(DataCSGenerator.GenerateServerCode35(de));
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
					code.AppendLine(ServiceCSGenerator.GenerateServerCode35(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsCSGenerator.GenerateCode35Client(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostCSGenerator.GenerateServerCode35Client(he));
				code.AppendLine();
			}

			code.AppendLine("}");

			foreach (Namespace tn in o.Children)
				code.AppendLine(GenerateServerCode35Client(tn));

			return code.ToString();
		}

		public static string GenerateServerCode40(Namespace o)
		{
			var code = new StringBuilder();

			code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tEnumerations");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Projects.Enum ee in o.Enums)
					code.AppendLine(EnumCSGenerator.GenerateServerCode40(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
					code.AppendLine(DataCSGenerator.GenerateServerCode40(de));
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
					code.AppendLine(ServiceCSGenerator.GenerateServerCode40(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsCSGenerator.GenerateCode40(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostCSGenerator.GenerateServerCode40(he));
				code.AppendLine();
			}

			code.AppendLine("}");

			foreach (Namespace tn in o.Children)
				code.AppendLine(GenerateServerCode40(tn));

			return code.ToString();
		}

		public static string GenerateServerCode40Client(Namespace o)
		{
			var code = new StringBuilder();

			code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tEnumerations");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Projects.Enum ee in o.Enums)
					code.AppendLine(EnumCSGenerator.GenerateServerCode40(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
					code.AppendLine(DataCSGenerator.GenerateServerCode40(de));
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
					code.AppendLine(ServiceCSGenerator.GenerateServerCode40(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsCSGenerator.GenerateCode40Client(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostCSGenerator.GenerateServerCode40Client(he));
				code.AppendLine();
			}

			code.AppendLine("}");

			foreach (Namespace tn in o.Children)
				code.AppendLine(GenerateServerCode40Client(tn));

			return code.ToString();
		}

		public static string GenerateServerCode45(Namespace o)
		{
			var code = new StringBuilder();

			code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tEnumerations");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Projects.Enum ee in o.Enums)
					code.AppendLine(EnumCSGenerator.GenerateServerCode45(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
					code.AppendLine(DataCSGenerator.GenerateServerCode45(de));
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
					code.AppendLine(ServiceCSGenerator.GenerateServerCode45(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsCSGenerator.GenerateCode45(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostCSGenerator.GenerateServerCode45(he));
				code.AppendLine();
			}

			code.AppendLine("}");

			foreach (Namespace tn in o.Children)
				code.AppendLine(GenerateServerCode45(tn));

			return code.ToString();
		}

		public static string GenerateClientCode30(Namespace o)
		{
			var code = new StringBuilder();

			code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tEnumeration Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Projects.Enum ee in o.Enums)
					code.AppendLine(EnumCSGenerator.GenerateProxyCode30(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
				{
					code.AppendLine(DataCSGenerator.GenerateProxyCode30(de));
					code.AppendLine(DataCSGenerator.GenerateXAMLCode30(de));
				}
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
					code.AppendLine(ServiceCSGenerator.GenerateClientCode30(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsCSGenerator.GenerateCode30(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostCSGenerator.GenerateClientCode35(he));
				code.AppendLine();
			}

			code.AppendLine("}");

			foreach (Namespace tn in o.Children)
				code.AppendLine(GenerateClientCode30(tn));

			return code.ToString();
		}

		public static string GenerateClientCode35(Namespace o)
		{
			var code = new StringBuilder();

			code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tEnumeration Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Projects.Enum ee in o.Enums)
					code.AppendLine(EnumCSGenerator.GenerateProxyCode35(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
				{
					code.AppendLine(DataCSGenerator.GenerateProxyCode35(de));
					code.AppendLine(DataCSGenerator.GenerateXAMLCode35(de));
				}
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
					code.AppendLine(ServiceCSGenerator.GenerateClientCode35(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsCSGenerator.GenerateCode35(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostCSGenerator.GenerateClientCode35(he));
				code.AppendLine();
			}

			code.AppendLine("}");

			foreach (Namespace tn in o.Children)
				code.AppendLine(GenerateClientCode35(tn));

			return code.ToString();
		}

		public static string GenerateClientCode35Client(Namespace o)
		{
			var code = new StringBuilder();

			code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tEnumeration Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Projects.Enum ee in o.Enums)
					code.AppendLine(EnumCSGenerator.GenerateProxyCode35(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
				{
					code.AppendLine(DataCSGenerator.GenerateProxyCode35(de));
					code.AppendLine(DataCSGenerator.GenerateXAMLCode35(de));
				}
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
					code.AppendLine(ServiceCSGenerator.GenerateClientCode35(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsCSGenerator.GenerateCode35Client(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostCSGenerator.GenerateClientCode35Client(he));
				code.AppendLine();
			}

			code.AppendLine("}");

			foreach (Namespace tn in o.Children)
				code.AppendLine(GenerateClientCode35Client(tn));

			return code.ToString();
		}

		public static string GenerateClientCode40(Namespace o)
		{
			var code = new StringBuilder();

			code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tEnumeration Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Projects.Enum ee in o.Enums)
					code.AppendLine(EnumCSGenerator.GenerateProxyCode40(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
				{
					code.AppendLine(DataCSGenerator.GenerateProxyCode40(de));
					code.AppendLine(DataCSGenerator.GenerateXAMLCode40(de));
				}
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
					code.AppendLine(ServiceCSGenerator.GenerateClientCode40(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsCSGenerator.GenerateCode40(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostCSGenerator.GenerateClientCode40(he));
				code.AppendLine();
			}

			code.AppendLine("}");

			foreach (Namespace tn in o.Children)
				code.AppendLine(GenerateClientCode40(tn));

			return code.ToString();
		}

		public static string GenerateClientCode40Client(Namespace o)
		{
			var code = new StringBuilder();

			code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tEnumeration Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Projects.Enum ee in o.Enums)
					code.AppendLine(EnumCSGenerator.GenerateProxyCode40(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
				{
					code.AppendLine(DataCSGenerator.GenerateProxyCode40(de));
					code.AppendLine(DataCSGenerator.GenerateXAMLCode40(de));
				}
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
				{
					code.AppendLine(ServiceCSGenerator.GenerateClientCode40(se));
				}
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsCSGenerator.GenerateCode40Client(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostCSGenerator.GenerateClientCode40Client(he));
				code.AppendLine();
			}

			code.AppendLine("}");

			foreach (Namespace tn in o.Children)
				code.AppendLine(GenerateClientCode40Client(tn));

			return code.ToString();
		}

		public static string GenerateClientCode45(Namespace o)
		{
			var code = new StringBuilder();

			code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			code.AppendLine("{");

			if (o.Enums.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tEnumeration Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Projects.Enum ee in o.Enums)
					code.AppendLine(EnumCSGenerator.GenerateProxyCode45(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
				{
					code.AppendLine(Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8 ? DataCSGenerator.GenerateProxyCodeRT8(de) : DataCSGenerator.GenerateProxyCode45(de));
					code.AppendLine(Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8 ? DataCSGenerator.GenerateXAMLCodeRT8(de) : DataCSGenerator.GenerateXAMLCode45(de));
				}
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
					code.AppendLine(ServiceCSGenerator.GenerateClientCode45(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsCSGenerator.GenerateCode45(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostCSGenerator.GenerateClientCode45(he));
				code.AppendLine();
			}

			code.AppendLine("}");

			foreach (Namespace tn in o.Children)
				code.AppendLine(GenerateClientCode45(tn));

			return code.ToString();
		}
	}
}