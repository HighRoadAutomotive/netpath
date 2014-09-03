using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETPath.Projects;
using NETPath.Projects.Helpers;

namespace NETPath.Generators.CS
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

		public static string GenerateContractNamespaceAttributes(Namespace o, ProjectGenerationTarget target)
		{
			if (!target.TargetTypes.Intersect(o.Enums).Any() && !target.TargetTypes.Intersect(o.Data).Any() && !target.TargetTypes.Intersect(o.Services).Any() && !target.TargetTypes.Intersect(o.RESTServices).Any() && !target.TargetTypes.Intersect(o.Bindings).Any())
				return "";

			var code = new StringBuilder();

			code.AppendLine(string.Format("[assembly: System.Runtime.Serialization.ContractNamespaceAttribute(\"{0}\", ClrNamespace=\"{1}\")]", o.FullURI, o.FullName));

			foreach (Namespace tn in o.Children)
				code.Append(GenerateContractNamespaceAttributes(tn, target));

			return code.ToString();
		}

		public static string GenerateServerCode45(Namespace o, ProjectGenerationTarget target)
		{
			if (!target.TargetTypes.Intersect(o.Enums).Any() && !target.TargetTypes.Intersect(o.Data).Any() && !target.TargetTypes.Intersect(o.Services).Any() && !target.TargetTypes.Intersect(o.RESTServices).Any() && !target.TargetTypes.Intersect(o.Bindings).Any() && !target.TargetTypes.Intersect(o.Hosts).Any())
				return "";

			var code = new StringBuilder();

			code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			code.AppendLine("{");

			if (o.Owner.UsingInsideNamespace)
			{
				if (o.Owner.GenerateRegions)
				{
					code.AppendLine("\t#region Using");
					code.AppendLine();
				}
				// Generate using namespaces
				foreach (ProjectUsingNamespace pun in o.Owner.UsingNamespaces)
				{
					if ((pun.Server))
						code.AppendLine(string.Format("\tusing {0};", pun.Namespace));
				}
				if (o.Owner.EnableEntityFramework) code.AppendLine("\tusing System.Data.Entity.Core.Objects;");
				code.AppendLine();
				if (o.Owner.GenerateRegions)
				{
					code.AppendLine("\t#endregion");
					code.AppendLine();
				}
			}

			if (target.TargetTypes.Intersect(o.Enums).Any())
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tEnumerations");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (var ee in target.TargetTypes.Intersect(o.Enums))
					code.AppendLine(EnumGenerator.GenerateServerCode45((Projects.Enum)ee));
				code.AppendLine();
			}

			if (target.TargetTypes.Intersect(o.Data).Any())
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (var de in target.TargetTypes.Intersect(o.Data))
					code.AppendLine(DataGenerator.GenerateServerCode45((Data)de));
				code.AppendLine();
			}

			if (target.TargetTypes.Intersect(o.Services).Any())
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (var se in target.TargetTypes.Intersect(o.Services))
					code.AppendLine(ServiceGenerator.GenerateServerCode45((Service)se));
				code.AppendLine();
			}

			if (target.TargetTypes.Intersect(o.RESTServices).Any())
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tREST Service Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (var se in target.TargetTypes.Intersect(o.RESTServices))
					code.AppendLine(RESTServiceGenerator.GenerateServerCode((RESTService)se));
				code.AppendLine();
			}

			if (target.TargetTypes.Intersect(o.Bindings).Any())
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (var sb in target.TargetTypes.Intersect(o.Bindings))
					code.AppendLine(BindingsGenerator.GenerateCode45((ServiceBinding)sb, ProjectGenerationFramework.NET45));
				code.AppendLine();
			}

			if (target.TargetTypes.Intersect(o.Hosts).Any())
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Hosts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (var he in target.TargetTypes.Intersect(o.Hosts))
					code.AppendLine(HostGenerator.GenerateServerCode45((Host)he));
				code.AppendLine();
			}

			code.AppendLine("}");

			foreach (Namespace tn in o.Children)
				code.AppendLine(GenerateServerCode45(tn, target));

			return code.ToString();
		}

		public static string GenerateClientCode45(Namespace o, ProjectGenerationTarget target)
		{
			if (!target.TargetTypes.Intersect(o.Enums).Any() && !target.TargetTypes.Intersect(o.Data).Any() && !target.TargetTypes.Intersect(o.Services).Any() && !target.TargetTypes.Intersect(o.RESTServices).Any() && !target.TargetTypes.Intersect(o.Bindings).Any())
				return "";

			var code = new StringBuilder();

			code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			code.AppendLine("{");

			if (o.Owner.UsingInsideNamespace)
			{
				if (o.Owner.GenerateRegions)
				{
					code.AppendLine("\t#region Using");
					code.AppendLine();
				}
				// Generate using namespaces
				foreach (ProjectUsingNamespace pun in o.Owner.UsingNamespaces)
				{
					if (pun.Client && pun.NET)
						code.AppendLine(string.Format("\tusing {0};", pun.Namespace));
				}
				code.AppendLine();
				if (o.Owner.GenerateRegions)
				{
					code.AppendLine("\t#endregion");
					code.AppendLine();
				}
			}

			if (target.TargetTypes.Intersect(o.Enums).Any())
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tEnumeration Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (var ee in target.TargetTypes.Intersect(o.Enums))
					code.AppendLine(EnumGenerator.GenerateProxyCode45((Projects.Enum)ee));
				code.AppendLine();
			}

			if (target.TargetTypes.Intersect(o.Data).Any())
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (var de in target.TargetTypes.Intersect(o.Data))
				{
					code.AppendLine(DataGenerator.GenerateProxyCode45((Data)de));
					code.AppendLine(DataGenerator.GenerateXAMLCode45((Data)de));
				}
				code.AppendLine();
			}

			if (target.TargetTypes.Intersect(o.Services).Any())
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (var se in target.TargetTypes.Intersect(o.Services))
					code.AppendLine(ServiceGenerator.GenerateClientCode45((Service)se, ProjectGenerationFramework.NET45));
				code.AppendLine();
			}

			if (target.TargetTypes.Intersect(o.RESTServices).Any())
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tREST Service Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (var se in target.TargetTypes.Intersect(o.RESTServices))
					code.AppendLine(RESTServiceGenerator.GenerateClientCode45((RESTService)se));
				code.AppendLine();
			}

			if (target.TargetTypes.Intersect(o.Bindings).Any())
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (var sb in target.TargetTypes.Intersect(o.Bindings))
					code.AppendLine(BindingsGenerator.GenerateCode45((ServiceBinding)sb, ProjectGenerationFramework.NET45));
				code.AppendLine();
			}

			code.AppendLine("}");

			foreach (Namespace tn in o.Children)
				code.AppendLine(GenerateClientCode45(tn, target));

			return code.ToString();
		}

		public static string GenerateClientCodeRT8(Namespace o, ProjectGenerationTarget target)
		{
			if (!target.TargetTypes.Intersect(o.Enums).Any() && !target.TargetTypes.Intersect(o.Data).Any() && !target.TargetTypes.Intersect(o.Services).Any() && !target.TargetTypes.Intersect(o.RESTServices).Any() && !target.TargetTypes.Intersect(o.Bindings).Any())
				return "";

			var code = new StringBuilder();

			code.AppendFormat("namespace {0}{1}", o.FullName, Environment.NewLine);
			code.AppendLine("{");

			if (o.Owner.UsingInsideNamespace)
			{
				if (o.Owner.GenerateRegions)
				{
					code.AppendLine("\t#region Using");
					code.AppendLine();
				}
				// Generate using namespaces
				foreach (ProjectUsingNamespace pun in o.Owner.UsingNamespaces)
				{
					if (pun.Client && pun.RT)
						code.AppendLine(string.Format("\tusing {0};", pun.Namespace));
				}
				code.AppendLine();
				if (o.Owner.GenerateRegions)
				{
					code.AppendLine("\t#endregion");
					code.AppendLine();
				}
			}

			if (target.TargetTypes.Intersect(o.Enums).Any())
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tEnumeration Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (var ee in target.TargetTypes.Intersect(o.Enums))
					code.AppendLine(EnumGenerator.GenerateProxyCode45((Projects.Enum)ee));
				code.AppendLine();
			}

			if (target.TargetTypes.Intersect(o.Data).Any())
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tData Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (var de in target.TargetTypes.Intersect(o.Data))
				{
					code.AppendLine(DataGenerator.GenerateProxyCodeRT8((Data)de));
					code.AppendLine(DataGenerator.GenerateXAMLCodeRT8((Data)de));
				}
				code.AppendLine();
			}

			if (target.TargetTypes.Intersect(o.Services).Any())
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (var se in target.TargetTypes.Intersect(o.Services))
					code.AppendLine(ServiceGenerator.GenerateClientCode45((Service)se, ProjectGenerationFramework.NET45));
				code.AppendLine();
			}

			if (target.TargetTypes.Intersect(o.RESTServices).Any())
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tREST Service Contracts");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (var se in target.TargetTypes.Intersect(o.RESTServices))
					code.AppendLine(RESTServiceGenerator.GenerateClientCode45((RESTService)se));
				code.AppendLine();
			}

			if (target.TargetTypes.Intersect(o.Bindings).Any())
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tService Bindings");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (var sb in target.TargetTypes.Intersect(o.Bindings))
					code.AppendLine(BindingsGenerator.GenerateCodeRT8((ServiceBinding)sb));
				code.AppendLine();
			}

			code.AppendLine("}");

			foreach (Namespace tn in o.Children)
				code.AppendLine(GenerateClientCodeRT8(tn, target));

			return code.ToString();
		}
	}
}