using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFArchitect.Projects;
using WCFArchitect.Projects.Helpers;

namespace WCFArchitect.Generators.NET.CS
{
	internal static class NamespaceGenerator
	{
		public static void VerifyCode(Namespace o, Action<CompileMessage> AddMessage)
		{
			if (string.IsNullOrEmpty(o.URI))
				AddMessage(new CompileMessage("GS1000", "The '" + o.Name + "' namespace in the '" + o.Owner.Name + "' project does not have a URI. Namespaces MUST have a URI.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
			else
				if (RegExs.MatchHTTPURI.IsMatch(o.URI) == false)
					AddMessage(new CompileMessage("GS1001", "The URI '" + o.URI + "' for the '" + o.Name + "' namespace in the '" + o.Owner.Name + "' project is not a valid URI. Any associated services and data may not function properly.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

			foreach (Projects.Enum ee in o.Enums)
				EnumGenerator.VerifyCode(ee, AddMessage);

			foreach (Data de in o.Data)
				DataGenerator.VerifyCode(de, AddMessage);

			foreach (Service se in o.Services)
				ServiceGenerator.VerifyCode(se, AddMessage);

			foreach (Host se in o.Hosts)
				HostGenerator.VerifyCode(se, AddMessage);

			foreach (ServiceBinding se in o.Bindings)
				BindingsGenerator.VerifyCode(se, AddMessage);

			foreach (Namespace tn in o.Children)
				VerifyCode(tn, AddMessage);

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
					code.AppendLine(EnumGenerator.GenerateServerCode30(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
					code.AppendLine(DataGenerator.GenerateServerCode30(de));
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
					code.AppendLine(ServiceGenerator.GenerateServerCode30(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsGenerator.GenerateCode30(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostGenerator.GenerateServerCode30(he));
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
					code.AppendLine(EnumGenerator.GenerateServerCode35(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
					code.AppendLine(DataGenerator.GenerateServerCode35(de));
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
					code.AppendLine(ServiceGenerator.GenerateServerCode35(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsGenerator.GenerateCode35(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostGenerator.GenerateServerCode35(he));
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
					code.AppendLine(EnumGenerator.GenerateServerCode35(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
					code.AppendLine(DataGenerator.GenerateServerCode35(de));
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
					code.AppendLine(ServiceGenerator.GenerateServerCode35(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsGenerator.GenerateCode35Client(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostGenerator.GenerateServerCode35Client(he));
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
					code.AppendLine(EnumGenerator.GenerateServerCode40(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
					code.AppendLine(DataGenerator.GenerateServerCode40(de));
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
					code.AppendLine(ServiceGenerator.GenerateServerCode40(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsGenerator.GenerateCode40(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostGenerator.GenerateServerCode40(he));
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
					code.AppendLine(EnumGenerator.GenerateServerCode40(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
					code.AppendLine(DataGenerator.GenerateServerCode40(de));
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
					code.AppendLine(ServiceGenerator.GenerateServerCode40(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsGenerator.GenerateCode40Client(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostGenerator.GenerateServerCode40Client(he));
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
					code.AppendLine(EnumGenerator.GenerateServerCode45(ee));
				code.AppendLine();
			}

			if (o.Data.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Data de in o.Data)
					code.AppendLine(DataGenerator.GenerateServerCode45(de));
				code.AppendLine();
			}

			if (o.Services.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Service se in o.Services)
					code.AppendLine(ServiceGenerator.GenerateServerCode45(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsGenerator.GenerateCode45(sb));
				code.AppendLine();
			}

			if (o.Hosts.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Host he in o.Hosts)
					code.AppendLine(HostGenerator.GenerateServerCode45(he));
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
					code.AppendLine(EnumGenerator.GenerateProxyCode30(ee));
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
					code.AppendLine(DataGenerator.GenerateProxyCode30(de));
					code.AppendLine(DataGenerator.GenerateXAMLCode30(de));
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
					code.AppendLine(ServiceGenerator.GenerateClientCode30(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsGenerator.GenerateCode30(sb));
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
					code.AppendLine(EnumGenerator.GenerateProxyCode35(ee));
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
					code.AppendLine(DataGenerator.GenerateProxyCode35(de));
					code.AppendLine(DataGenerator.GenerateXAMLCode35(de));
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
					code.AppendLine(ServiceGenerator.GenerateClientCode35(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsGenerator.GenerateCode35(sb));
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
					code.AppendLine(EnumGenerator.GenerateProxyCode35(ee));
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
					code.AppendLine(DataGenerator.GenerateProxyCode35(de));
					code.AppendLine(DataGenerator.GenerateXAMLCode35(de));
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
					code.AppendLine(ServiceGenerator.GenerateClientCode35(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsGenerator.GenerateCode35Client(sb));
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
					code.AppendLine(EnumGenerator.GenerateProxyCode40(ee));
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
					code.AppendLine(DataGenerator.GenerateProxyCode40(de));
					code.AppendLine(DataGenerator.GenerateXAMLCode40(de));
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
					code.AppendLine(ServiceGenerator.GenerateClientCode40(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsGenerator.GenerateCode40(sb));
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
					code.AppendLine(EnumGenerator.GenerateProxyCode40(ee));
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
					code.AppendLine(DataGenerator.GenerateProxyCode40(de));
					code.AppendLine(DataGenerator.GenerateXAMLCode40(de));
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
					code.AppendLine(ServiceGenerator.GenerateClientCode40(se));
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
					code.AppendLine(BindingsGenerator.GenerateCode40Client(sb));
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
					code.AppendLine(EnumGenerator.GenerateProxyCode45(ee));
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
					code.AppendLine(DataGenerator.GenerateProxyCode45(de));
					code.AppendLine(DataGenerator.GenerateXAMLCode45(de));
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
					code.AppendLine(ServiceGenerator.GenerateClientCode45(se));
				code.AppendLine();
			}

			if (o.Bindings.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (ServiceBinding sb in o.Bindings)
					code.AppendLine(BindingsGenerator.GenerateCode45(sb));
				code.AppendLine();
			}

			code.AppendLine("}");

			foreach (Namespace tn in o.Children)
				code.AppendLine(GenerateClientCode45(tn));

			return code.ToString();
		}
	}
}