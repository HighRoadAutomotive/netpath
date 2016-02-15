using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETPath.Projects;
using NETPath.Projects.Helpers;
using NETPath.Projects.Wcf;

namespace NETPath.Generators.CS.Wcf
{
	internal static class NamespaceGenerator
	{
		public static void VerifyCode(WcfNamespace o, Action<CompileMessage> AddMessage)
		{
			if (string.IsNullOrEmpty(o.Uri))
				AddMessage(new CompileMessage("GS1000", "The '" + o.Name + "' namespace in the '" + o.Owner.Name + "' project does not have a Uri. Namespaces MUST have a Uri.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));
			else
				if (RegExs.MatchHttpUri.IsMatch(o.Uri) == false)
				AddMessage(new CompileMessage("GS1001", "The Uri '" + o.Uri + "' for the '" + o.Name + "' namespace in the '" + o.Owner.Name + "' project is not a valid Uri. Any associated services and data may not function properly.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType()));

			foreach (Projects.Enum ee in o.Enums)
				EnumGenerator.VerifyCode(ee, AddMessage);

			foreach (WcfData de in o.Data)
				DataGenerator.VerifyCode(de, AddMessage);

			foreach (WcfService se in o.Services)
				ServiceGenerator.VerifyCode(se, AddMessage);

			foreach (WcfHost se in o.Hosts)
				HostGenerator.VerifyCode(se, AddMessage);

			foreach (WcfBinding se in o.Bindings)
				BindingsGenerator.VerifyCode(se, AddMessage);

			foreach (WcfNamespace tn in o.Children)
				VerifyCode(tn, AddMessage);

		}

		public static string GenerateContractNamespaceAttributes(WcfNamespace o, ProjectGenerationTarget target)
		{
			if (!target.TargetTypes.Intersect(o.Enums).Any() && !target.TargetTypes.Intersect(o.Data).Any() && !target.TargetTypes.Intersect(o.Services).Any() && !target.TargetTypes.Intersect(o.Bindings).Any())
				return "";

			var code = new StringBuilder();

			code.AppendLine(string.Format("[assembly: System.Runtime.Serialization.ContractNamespaceAttribute(\"{0}\", ClrNamespace=\"{1}\")]", o.FullURI, o.FullName));

			foreach (WcfNamespace tn in o.Children)
				code.Append(GenerateContractNamespaceAttributes(tn, target));

			return code.ToString();
		}

		public static string GenerateServerCode45(WcfNamespace o, ProjectGenerationTarget target)
		{
			if (!o.Children.Any() && !target.TargetTypes.Intersect(o.Enums).Any() && !target.TargetTypes.Intersect(o.Data).Any() && !target.TargetTypes.Intersect(o.Services).Any() && !target.TargetTypes.Intersect(o.Bindings).Any() && !target.TargetTypes.Intersect(o.Hosts).Any())
				return "";

			var code = new StringBuilder();

			if (target.TargetTypes.Intersect(o.Enums).Any() || target.TargetTypes.Intersect(o.Data).Any() || target.TargetTypes.Intersect(o.Services).Any() || target.TargetTypes.Intersect(o.Bindings).Any() || target.TargetTypes.Intersect(o.Hosts).Any())
			{
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
					if (!o.Owner.GenerateRegions)
					{
						code.AppendLine("\t/**************************************************************************");
						code.AppendLine("\t*\tEnumerations");
						code.AppendLine("\t**************************************************************************/");
					}
					if (o.Owner.GenerateRegions) code.AppendLine("\t#region Enumeration Contracts");
					code.AppendLine();
					foreach (var ee in target.TargetTypes.Intersect(o.Enums))
						code.AppendLine(EnumGenerator.GenerateServerCode45((Projects.Enum)ee));
					if (o.Owner.GenerateRegions) code.AppendLine("\t#endregion");
					code.AppendLine();
				}

				if (target.TargetTypes.Intersect(o.Data).Any())
				{
					if (!o.Owner.GenerateRegions)
					{
						code.AppendLine("\t/**************************************************************************");
						code.AppendLine("\t*\tData Contracts");
						code.AppendLine("\t**************************************************************************/");
					}
					if (o.Owner.GenerateRegions) code.AppendLine("\t#region Data Contracts");
					code.AppendLine();
					foreach (var de in target.TargetTypes.Intersect(o.Data))
						code.AppendLine(DataGenerator.GenerateServerCode45((WcfData)de));
					if (o.Owner.GenerateRegions) code.AppendLine("\t#endregion");
					code.AppendLine();
				}

				if (target.TargetTypes.Intersect(o.Services).Any())
				{
					if (!o.Owner.GenerateRegions)
					{
						code.AppendLine("\t/**************************************************************************");
						code.AppendLine("\t*\tService Contracts");
						code.AppendLine("\t**************************************************************************/");
					}
					if (o.Owner.GenerateRegions) code.AppendLine("\t#region Service Contracts");
					code.AppendLine();
					foreach (var se in target.TargetTypes.Intersect(o.Services))
						code.AppendLine(ServiceGenerator.GenerateServerCode45((WcfService)se));
					if (o.Owner.GenerateRegions) code.AppendLine("\t#endregion");
					code.AppendLine();
				}

				if (target.TargetTypes.Intersect(o.Bindings).Any())
				{
					if (!o.Owner.GenerateRegions)
					{
						code.AppendLine("\t/**************************************************************************");
						code.AppendLine("\t*\tService Bindings");
						code.AppendLine("\t**************************************************************************/");
					}
					if (o.Owner.GenerateRegions) code.AppendLine("\t#region Service Bindings");
					code.AppendLine();
					foreach (var sb in target.TargetTypes.Intersect(o.Bindings))
						code.AppendLine(BindingsGenerator.GenerateCode45((WcfBinding)sb, ProjectGenerationFramework.NET45));
					if (o.Owner.GenerateRegions) code.AppendLine("\t#endregion");
					code.AppendLine();
				}

				if (target.TargetTypes.Intersect(o.Hosts).Any())
				{
					if (!o.Owner.GenerateRegions)
					{
						code.AppendLine("\t/**************************************************************************");
						code.AppendLine("\t*\tService Hosts");
						code.AppendLine("\t**************************************************************************/");
					}
					if (o.Owner.GenerateRegions) code.AppendLine("\t#region Service Hosts");
					code.AppendLine();
					foreach (var he in target.TargetTypes.Intersect(o.Hosts))
						code.AppendLine(HostGenerator.GenerateServerCode45((WcfHost)he));
					if (o.Owner.GenerateRegions) code.AppendLine("\t#endregion");
					code.AppendLine();
				}

				code.AppendLine("}");
			}

			foreach (WcfNamespace tn in o.Children)
				code.AppendLine(GenerateServerCode45(tn, target));

			return code.ToString();
		}

		public static string GenerateClientCode45(WcfNamespace o, ProjectGenerationTarget target)
		{
			if (!o.Children.Any() && !target.TargetTypes.Intersect(o.Enums).Any() && !target.TargetTypes.Intersect(o.Data).Any() && !target.TargetTypes.Intersect(o.Services).Any() && !target.TargetTypes.Intersect(o.Bindings).Any() && !target.TargetTypes.Intersect(o.Hosts).Any())
				return "";

			var code = new StringBuilder();

			if (target.TargetTypes.Intersect(o.Enums).Any() || target.TargetTypes.Intersect(o.Data).Any() || target.TargetTypes.Intersect(o.Services).Any() || target.TargetTypes.Intersect(o.Bindings).Any() || target.TargetTypes.Intersect(o.Hosts).Any())
			{
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
					if (!o.Owner.GenerateRegions)
					{
						code.AppendLine("\t/**************************************************************************");
						code.AppendLine("\t*\tEnumeration Contracts");
						code.AppendLine("\t**************************************************************************/");
					}
					if (o.Owner.GenerateRegions) code.AppendLine("\t#region Enumeration Contracts");
					code.AppendLine();
					foreach (var ee in target.TargetTypes.Intersect(o.Enums))
						code.AppendLine(EnumGenerator.GenerateProxyCode45((Projects.Enum)ee));
					if (o.Owner.GenerateRegions) code.AppendLine("\t#endregion");
					code.AppendLine();
				}

				if (target.TargetTypes.Intersect(o.Data).Any())
				{
					if (!o.Owner.GenerateRegions)
					{
						code.AppendLine("\t/**************************************************************************");
						code.AppendLine("\t*\tData Contracts");
						code.AppendLine("\t**************************************************************************/");
					}
					if (o.Owner.GenerateRegions) code.AppendLine("\t#region Data Contracts");
					code.AppendLine();
					foreach (var de in target.TargetTypes.Intersect(o.Data))
					{
						code.AppendLine(DataGenerator.GenerateProxyCode45((WcfData)de));
						code.AppendLine(DataGenerator.GenerateXAMLCode45((WcfData)de));
					}
					if (o.Owner.GenerateRegions) code.AppendLine("\t#endregion");
					code.AppendLine();
				}

				if (target.TargetTypes.Intersect(o.Services).Any())
				{
					if (!o.Owner.GenerateRegions)
					{
						code.AppendLine("\t/**************************************************************************");
						code.AppendLine("\t*\tService Contracts");
						code.AppendLine("\t**************************************************************************/");
					}
					if (o.Owner.GenerateRegions) code.AppendLine("\t#region Service Contracts");
					code.AppendLine();
					foreach (var se in target.TargetTypes.Intersect(o.Services))
						code.AppendLine(ServiceGenerator.GenerateClientCode45((WcfService)se, ProjectGenerationFramework.NET45));
					if (o.Owner.GenerateRegions) code.AppendLine("\t#endregion");
					code.AppendLine();
				}

				if (target.TargetTypes.Intersect(o.Bindings).Any())
				{
					if (!o.Owner.GenerateRegions)
					{
						code.AppendLine("\t/**************************************************************************");
						code.AppendLine("\t*\tService Bindings");
						code.AppendLine("\t**************************************************************************/");
					}
					if (o.Owner.GenerateRegions) code.AppendLine("\t#region Service Bindings");
					code.AppendLine();
					foreach (var sb in target.TargetTypes.Intersect(o.Bindings))
						code.AppendLine(BindingsGenerator.GenerateCode45((WcfBinding)sb, ProjectGenerationFramework.NET45));
					if (o.Owner.GenerateRegions) code.AppendLine("\t#endregion");
					code.AppendLine();
				}

				code.AppendLine("}");
			}

			foreach (WcfNamespace tn in o.Children)
				code.AppendLine(GenerateClientCode45(tn, target));

			return code.ToString();
		}

		public static string GenerateClientCodeRT8(WcfNamespace o, ProjectGenerationTarget target)
		{
			if (!o.Children.Any() && !target.TargetTypes.Intersect(o.Enums).Any() && !target.TargetTypes.Intersect(o.Data).Any() && !target.TargetTypes.Intersect(o.Services).Any() && !target.TargetTypes.Intersect(o.Bindings).Any() && !target.TargetTypes.Intersect(o.Hosts).Any())
				return "";

			var code = new StringBuilder();

			if (target.TargetTypes.Intersect(o.Enums).Any() || target.TargetTypes.Intersect(o.Data).Any() || target.TargetTypes.Intersect(o.Services).Any() || target.TargetTypes.Intersect(o.Bindings).Any() || target.TargetTypes.Intersect(o.Hosts).Any())
			{
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
					if (!o.Owner.GenerateRegions)
					{
						code.AppendLine("\t/**************************************************************************");
						code.AppendLine("\t*\tEnumeration Contracts");
						code.AppendLine("\t**************************************************************************/");
					}
					if (o.Owner.GenerateRegions) code.AppendLine("\t#region Enumeration Contracts");
					code.AppendLine();
					foreach (var ee in target.TargetTypes.Intersect(o.Enums))
						code.AppendLine(EnumGenerator.GenerateProxyCode45((Projects.Enum)ee));
					if (o.Owner.GenerateRegions) code.AppendLine("\t#endregion");
					code.AppendLine();
				}

				if (target.TargetTypes.Intersect(o.Data).Any())
				{
					if (!o.Owner.GenerateRegions)
					{
						code.AppendLine("\t/**************************************************************************");
						code.AppendLine("\t*\tData Contracts");
						code.AppendLine("\t**************************************************************************/");
					}
					if (o.Owner.GenerateRegions) code.AppendLine("\t#region Data Contracts");
					code.AppendLine();
					foreach (var de in target.TargetTypes.Intersect(o.Data))
					{
						code.AppendLine(DataGenerator.GenerateProxyCodeRT8((WcfData)de));
						code.AppendLine(DataGenerator.GenerateXAMLCodeRT8((WcfData)de));
					}
					if (o.Owner.GenerateRegions) code.AppendLine("\t#endregion");
					code.AppendLine();
				}

				if (target.TargetTypes.Intersect(o.Services).Any())
				{
					if (!o.Owner.GenerateRegions)
					{
						code.AppendLine("\t/**************************************************************************");
						code.AppendLine("\t*\tService Contracts");
						code.AppendLine("\t**************************************************************************/");
					}
					if (o.Owner.GenerateRegions) code.AppendLine("\t#region Service Contracts");
					code.AppendLine();
					foreach (var se in target.TargetTypes.Intersect(o.Services))
						code.AppendLine(ServiceGenerator.GenerateClientCode45((WcfService)se, ProjectGenerationFramework.NET45));
					if (o.Owner.GenerateRegions) code.AppendLine("\t#endregion");
					code.AppendLine();
				}

				if (target.TargetTypes.Intersect(o.Bindings).Any())
				{
					if (!o.Owner.GenerateRegions)
					{
						code.AppendLine("\t/**************************************************************************");
						code.AppendLine("\t*\tService Bindings");
						code.AppendLine("\t**************************************************************************/");
					}
					if (o.Owner.GenerateRegions) code.AppendLine("\t#region Service Bindings");
					code.AppendLine();
					foreach (var sb in target.TargetTypes.Intersect(o.Bindings))
						code.AppendLine(BindingsGenerator.GenerateCodeRT8((WcfBinding)sb));
					if (o.Owner.GenerateRegions) code.AppendLine("\t#endregion");
					code.AppendLine();
				}

				code.AppendLine("}");
			}

			foreach (WcfNamespace tn in o.Children)
				code.AppendLine(GenerateClientCodeRT8(tn, target));

			return code.ToString();
		}
	}
}