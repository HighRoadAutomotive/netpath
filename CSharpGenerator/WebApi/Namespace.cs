using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETPath.Projects;
using NETPath.Projects.Helpers;
using NETPath.Projects.WebApi;

namespace NETPath.Generators.CS.WebApi
{
	internal static class NamespaceGenerator
	{
		public static void VerifyCode(WebApiNamespace o, Action<CompileMessage> AddMessage)
		{
			if (string.IsNullOrEmpty(o.URI))
				AddMessage(new CompileMessage("GS1000", "The '" + o.Name + "' namespace in the '" + o.Owner.Name + "' project does not have a URI. Namespaces MUST have a URI.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));
			else
				if (RegExs.MatchHTTPURI.IsMatch(o.URI) == false)
				AddMessage(new CompileMessage("GS1001", "The URI '" + o.URI + "' for the '" + o.Name + "' namespace in the '" + o.Owner.Name + "' project is not a valid URI. Any associated services and data may not function properly.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType()));

			foreach (Projects.Enum ee in o.Enums)
				EnumGenerator.VerifyCode(ee, AddMessage);

			foreach (WebApiData de in o.Data)
				DataGenerator.VerifyCode(de, AddMessage);

			foreach (WebApiService se in o.Services)
				ServiceGenerator.VerifyCode(se, AddMessage);

			foreach (WebApiNamespace tn in o.Children)
				VerifyCode(tn, AddMessage);

		}

		public static string GenerateContractNamespaceAttributes(WebApiNamespace o, ProjectGenerationTarget target)
		{
			if (!target.TargetTypes.Intersect(o.Enums).Any() && !target.TargetTypes.Intersect(o.Data).Any() && !target.TargetTypes.Intersect(o.Services).Any())
				return "";

			var code = new StringBuilder();

			code.AppendLine(string.Format("[assembly: System.Runtime.Serialization.ContractNamespaceAttribute(\"{0}\", ClrNamespace=\"{1}\")]", o.FullURI, o.FullName));

			foreach (WebApiNamespace tn in o.Children)
				code.Append(GenerateContractNamespaceAttributes(tn, target));

			return code.ToString();
		}

		public static string GenerateServerCode45(WebApiNamespace o, ProjectGenerationTarget target)
		{
			if (!o.Children.Any() && !target.TargetTypes.Intersect(o.Enums).Any() && !target.TargetTypes.Intersect(o.Data).Any() && !target.TargetTypes.Intersect(o.Services).Any())
				return "";

			var code = new StringBuilder();

			if (target.TargetTypes.Intersect(o.Enums).Any() || target.TargetTypes.Intersect(o.Data).Any() || target.TargetTypes.Intersect(o.Services).Any())
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
						code.AppendLine(DataGenerator.GenerateServerCode45((WebApiData)de));
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
					code.AppendLine("\tpublic static class ServiceController");
					code.AppendLine("\t{");
					foreach (var se in target.TargetTypes.Intersect(o.Services))
						code.AppendLine(string.Format("\t\tprivate static IDisposable _service{0}", se.Name));
					code.AppendLine("\t\tpublic static void Start()");
					code.AppendLine("\t\t{");
					foreach (var se in target.TargetTypes.Intersect(o.Services))
						code.AppendLine(string.Format("\t\t\t_service{0} = WebApp.Start<{0}Configuration>(\"{1}\")", se.Name, o.Parent.URI));
					code.AppendLine("\t\t}");
					code.AppendLine("\t\tpublic static void Stop()");
					code.AppendLine("\t\t{");
					foreach (var se in target.TargetTypes.Intersect(o.Services))
						code.AppendLine(string.Format("\t\t\tif (_service{0} != null) _service{0}.Dispose()", se.Name));
					code.AppendLine("\t\t}");
					code.AppendLine("\t}");
					code.AppendLine();
					foreach (var se in target.TargetTypes.Intersect(o.Services))
						code.AppendLine(ServiceGenerator.GenerateServerCode((WebApiService)se));
					if (o.Owner.GenerateRegions) code.AppendLine("\t#endregion");
					code.AppendLine();
				}

				code.AppendLine("}");
			}

			foreach (WebApiNamespace tn in o.Children)
				code.AppendLine(GenerateServerCode45(tn, target));

			return code.ToString();
		}

		public static string GenerateClientCode45(WebApiNamespace o, ProjectGenerationTarget target)
		{
			if (!o.Children.Any() && !target.TargetTypes.Intersect(o.Enums).Any() && !target.TargetTypes.Intersect(o.Data).Any() && !target.TargetTypes.Intersect(o.Services).Any())
				return "";

			var code = new StringBuilder();

			if (target.TargetTypes.Intersect(o.Enums).Any() || target.TargetTypes.Intersect(o.Data).Any() || target.TargetTypes.Intersect(o.Services).Any())
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
						code.AppendLine(DataGenerator.GenerateProxyCode45((WebApiData)de));
						code.AppendLine(DataGenerator.GenerateXAMLCode45((WebApiData)de));
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
						code.AppendLine(ServiceGenerator.GenerateClientCode45((WebApiService)se));
					if (o.Owner.GenerateRegions) code.AppendLine("\t#endregion");
					code.AppendLine();
				}

				code.AppendLine("}");
			}

			foreach (WebApiNamespace tn in o.Children)
				code.AppendLine(GenerateClientCode45(tn, target));

			return code.ToString();
		}

		public static string GenerateClientCodeRT8(WebApiNamespace o, ProjectGenerationTarget target)
		{
			if (!o.Children.Any() && !target.TargetTypes.Intersect(o.Enums).Any() && !target.TargetTypes.Intersect(o.Data).Any() && !target.TargetTypes.Intersect(o.Services).Any())
				return "";

			var code = new StringBuilder();

			if (target.TargetTypes.Intersect(o.Enums).Any() || target.TargetTypes.Intersect(o.Data).Any() || target.TargetTypes.Intersect(o.Services).Any())
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
						code.AppendLine(DataGenerator.GenerateProxyCodeRT8((WebApiData)de));
						code.AppendLine(DataGenerator.GenerateXAMLCodeRT8((WebApiData)de));
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
						code.AppendLine(ServiceGenerator.GenerateClientCode45((WebApiService)se));
					if (o.Owner.GenerateRegions) code.AppendLine("\t#endregion");
					code.AppendLine();
				}

				code.AppendLine("}");
			}

			foreach (WebApiNamespace tn in o.Children)
				code.AppendLine(GenerateClientCodeRT8(tn, target));

			return code.ToString();
		}
	}
}