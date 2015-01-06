using System;
using System.Linq;
using System.Text;
using NETPath.Projects;
using NETPath.Projects.Helpers;

namespace NETPath.Generators.CS
{
	internal static class EnumGenerator
	{
		public static void VerifyCode(Projects.Enum o, Action<CompileMessage> AddMessage)
		{
			if (string.IsNullOrEmpty(o.Name))
				AddMessage(new CompileMessage("GS4000", "The enumeration '" + o.Name + "' in the '" + o.Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));
			else
				if (RegExs.MatchCodeName.IsMatch(o.Name) == false)
				AddMessage(new CompileMessage("GS4001", "The enumeration '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));

			if (o.HasClientType)
				if (RegExs.MatchCodeName.IsMatch(o.ClientType.Name) == false)
					AddMessage(new CompileMessage("GS4002", "The enumeration '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));

			foreach (EnumElement e in o.Elements)
			{
				if (string.IsNullOrEmpty(e.Name))
					AddMessage(new CompileMessage("GS4003", "The enumeration '" + o.Name + "' in the '" + o.Parent.Name + "' namespace has an enumeration element with a blank Name. A Name MUST be specified.", CompileMessageSeverity.ERROR, o, e, e.GetType()));
				else
					if (RegExs.MatchCodeName.IsMatch(e.Name) == false)
					AddMessage(new CompileMessage("GS4004", "The enumeration element '" + e.Name + "' in the '" + o.Name + "' enumeration contains invalid characters in the Name.", CompileMessageSeverity.ERROR, o, e, e.GetType()));
			}

			if (o.IsFlags)
				if (o.Elements.Count() > 62) AddMessage(new CompileMessage("GS4005", "The number of elements in the Enumeration '" + o.Name + "' exceeds the maximum number of elements (62) supported by a flags enumerations. Any elements above this limit will not be generated.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType()));
		}

		public static string GenerateServerCode45(Projects.Enum o)
		{
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCode(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.Append("\t[DataContract(");
			if (o.HasClientType) code.AppendFormat("Name = \"{0}\", ", o.ClientType.Name);
			code.AppendFormat("Namespace = \"{0}\"", o.Parent.FullURI);
			code.AppendLine(")]");
			if (o.IsFlags) code.AppendLine("\t[Flags]");
			code.AppendLine(string.Format("\t{0} enum {1} : {2}", DataTypeGenerator.GenerateScope(o.Scope), o.Name, o.IsFlags ? "long" : "short"));
			code.AppendLine("\t{");
			int fv = 0;
			foreach (EnumElement ee in o.Elements.Where(ee => !ee.IsHidden))
				code.Append(o.IsFlags == false ? GenerateElementServerCode(ee) : GenerateElementServerCode(ee, fv++));
			code.AppendLine("\t}");
			return code.ToString();
		}

		private static string GenerateElementServerCode(EnumElement o)
		{
			var code = new StringBuilder();
			if (!string.IsNullOrEmpty(o.Documentation)) code.AppendLine(string.Format("\t\t<summary>{0}</summary>", o.Documentation));
			code.Append("\t\t[EnumMember(");
			if (o.ClientValue != null)
				code.AppendFormat("Value = \"{0}\"", o.ClientValue);
			code.AppendFormat(")] {0}", o.Name);
			if (o.IsCustomValue)
				code.AppendFormat(" = {0}", o.ServerValue);
			if (o.IsAggregateValue)
				if (o.AggregateValues.Count > 0)
				{
					code.AppendFormat(" = {0}", o.AggregateValues[0]);
					for (int i = 1; i < o.AggregateValues.Count; i++)
						code.AppendFormat(" | {0}", o.AggregateValues[i]);
				}
			code.AppendLine(",");
			return code.ToString();
		}

		private static string GenerateElementServerCode(EnumElement o, int ElementIndex)
		{
			if (ElementIndex > 61) return "";

			var code = new StringBuilder();
			if (!string.IsNullOrEmpty(o.Documentation)) code.AppendLine(string.Format("\t\t<summary>{0}</summary>", o.Documentation));
			if (o.IsExcluded == false) code.Append("\t\t[EnumMember()] ");
			code.Append(o.Name);
			if (o.IsAutoValue)
			{
				if (ElementIndex == 0) code.Append(" = 0");
				if (ElementIndex == 1) code.Append(" = 1");
				if (ElementIndex > 1) code.AppendFormat(" = {0}", Convert.ToInt32(Decimal.Round((decimal)Math.Pow(2, ElementIndex - 1))));
			}
			if (o.IsCustomValue)
				code.AppendFormat(" = {0}", o.ServerValue);
			if (o.IsAggregateValue)
				if (o.AggregateValues.Count > 0)
				{
					code.AppendFormat(" = {0}", o.AggregateValues[0]);
					for (int i = 1; i < o.AggregateValues.Count; i++)
						code.AppendFormat(" | {0}", o.AggregateValues[i]);
				}
			code.AppendLine(",");
			return code.ToString();
		}

		public static string GenerateProxyCode45(Projects.Enum o)
		{
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCode(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.Append("\t[DataContract(");
			if (o.HasClientType) code.AppendFormat("Name = \"{0}\", ", o.ClientType.Name);
			code.AppendFormat("Namespace = \"{0}\"", o.Parent.FullURI);
			code.AppendLine(")]");
			if (o.IsFlags) code.AppendLine("\t[Flags]");
			code.AppendLine(string.Format("\t{0} enum {1} : {2}", DataTypeGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, o.IsFlags ? "long" : "short"));
			code.AppendLine("\t{");
			int fv = 0;
			foreach (EnumElement ee in o.Elements.Where(ee => !ee.IsHidden))
				code.Append(o.IsFlags == false ? GenerateElementProxyCode(ee) : GenerateElementProxyCode(ee, fv++));
			code.AppendLine("\t}");
			return code.ToString();
		}

		private static string GenerateElementProxyCode(EnumElement o)
		{
			if (o.IsExcluded) return "";
			var code = new StringBuilder();
			if (!string.IsNullOrEmpty(o.Documentation)) code.AppendLine(string.Format("\t\t<summary>{0}</summary>", o.Documentation));
			code.AppendFormat("\t\t[EnumMember()] {0}", o.Name);
			if (o.IsCustomValue)
				code.AppendFormat(" = {0}", o.ClientValue ?? o.ServerValue);
			if (o.IsAggregateValue)
				if (o.AggregateValues.Count > 0)
				{
					code.AppendFormat(" = {0}", o.AggregateValues[0]);
					for (int i = 1; i < o.AggregateValues.Count; i++)
						code.AppendFormat(" | {0}", o.AggregateValues[i]);
				}
			code.AppendLine(",");
			return code.ToString();
		}

		private static string GenerateElementProxyCode(EnumElement o, int ElementIndex)
		{
			if (o.IsExcluded) return "";
			if (ElementIndex > 61) return "";

			var code = new StringBuilder();
			if (!string.IsNullOrEmpty(o.Documentation)) code.AppendLine(string.Format("\t\t<summary>{0}</summary>", o.Documentation));
			code.AppendFormat("\t\t[EnumMember()] {0}", o.Name);
			if (o.IsAutoValue)
			{
				if (ElementIndex == 0) code.Append(" = 0");
				if (ElementIndex == 1) code.Append(" = 1");
				if (ElementIndex > 1) code.AppendFormat(" = {0}", Convert.ToInt32(Decimal.Round((decimal)Math.Pow(2, ElementIndex - 1))));
			}
			if (o.IsCustomValue)
				code.AppendFormat(" = {0}", o.ClientValue ?? o.ServerValue);
			if (o.IsAggregateValue)
				if (o.AggregateValues.Count > 0)
				{
					code.AppendFormat(" = {0}", o.AggregateValues[0]);
					for (int i = 1; i < o.AggregateValues.Count; i++)
						code.AppendFormat(" | {0}", o.AggregateValues[i]);
				}
			code.AppendLine(",");
			return code.ToString();
		}
	}
}