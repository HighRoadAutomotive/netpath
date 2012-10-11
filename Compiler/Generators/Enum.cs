using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFArchitect.Projects;

namespace WCFArchitect.Compiler.Generators
{
	internal static class EnumCSGenerator
	{
		public static void VerifyCode(Projects.Enum o)
		{
			if (string.IsNullOrEmpty(o.Name))
				Program.AddMessage(new CompileMessage("GS4000", "The enumeration '" + o.Name + "' in the '" + o.Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
			else
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.Name) == false)
					Program.AddMessage(new CompileMessage("GS4001", "The enumeration '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));

			if (o.HasClientType)
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.ClientType.Name) == false)
					Program.AddMessage(new CompileMessage("GS4002", "The enumeration '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));

			foreach (EnumElement e in o.Elements)
			{
				if (string.IsNullOrEmpty(e.Name))
					Program.AddMessage(new CompileMessage("GS4003", "The enumeration '" + o.Name + "' in the '" + o.Parent.Name + "' namespace has an enumeration element with a blank Name. A Name MUST be specified.", CompileMessageSeverity.ERROR, o, e, e.GetType(), o.ID, e.ID));
				else
					if (Helpers.RegExs.MatchCodeName.IsMatch(e.Name) == false)
						Program.AddMessage(new CompileMessage("GS4004", "The enumeration element '" + e.Name + "' in the '" + o.Name + "' enumeration contains invalid characters in the Name.", CompileMessageSeverity.ERROR, o, e, e.GetType(), o.ID, e.ID));

				if (o.IsFlags == false)
				{
					bool badValue = false;
					if (o.Primitive == PrimitiveTypes.Int)
						try { Convert.ToInt32(e.Value); }
						catch { badValue = true; }
					else if (o.Primitive == PrimitiveTypes.SByte)
						try { Convert.ToSByte(e.Value); }
						catch { badValue = true; }
					else if (o.Primitive == PrimitiveTypes.Byte)
						try { Convert.ToByte(e.Value); }
						catch { badValue = true; }
					else if (o.Primitive == PrimitiveTypes.UShort)
						try { Convert.ToUInt16(e.Value); }
						catch { badValue = true; }
					else if (o.Primitive == PrimitiveTypes.Short)
						try { Convert.ToInt16(e.Value); }
						catch { badValue = true; }
					else if (o.Primitive == PrimitiveTypes.UInt)
						try { Convert.ToUInt32(e.Value); }
						catch { badValue = true; }
					else if (o.Primitive == PrimitiveTypes.Long)
						try { Convert.ToInt64(e.Value); }
						catch { badValue = true; }
					else if (o.Primitive == PrimitiveTypes.ULong)
						try { Convert.ToUInt64(e.Value); }
						catch { badValue = true; }

					if (e.Value == "") badValue = false;

					if (badValue)
						Program.AddMessage(new CompileMessage("GS4005", "The enumeration element '" + e.Name + "' in the '" + o.Name + "' enumeration contains an invalid value.", CompileMessageSeverity.ERROR, o, e, e.GetType(), o.ID, e.ID));
				}
			}

			if (o.IsFlags)
			{
				if (o.Primitive == PrimitiveTypes.Short && o.Elements.Count() > 14) Program.AddMessage(new CompileMessage("GS4006", "The number of elements in the Enumeration '" + o.Name + "' exceeds the maximum number of elements (14) supported by the 'short' data type. Any elements above this limit will not be generated.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
				if (o.Primitive == PrimitiveTypes.UShort && o.Elements.Count() > 15) Program.AddMessage(new CompileMessage("GS4006", "The number of elements in the Enumeration '" + o.Name + "' exceeds the maximum number of elements (15) supported by the 'unsigned short' data type. Any elements above this limit will not be generated.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
				if (o.Primitive == PrimitiveTypes.Int && o.Elements.Count() > 30) Program.AddMessage(new CompileMessage("GS4006", "The number of elements in the Enumeration '" + o.Name + "' exceeds the maximum number of elements (30) supported by the 'int' data type. Any elements above this limit will not be generated.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
				if (o.Primitive == PrimitiveTypes.UInt && o.Elements.Count() > 31) Program.AddMessage(new CompileMessage("GS4006", "The number of elements in the Enumeration '" + o.Name + "' exceeds the maximum number of elements (31) supported by the 'unsigned int' data type. Any elements above this limit will not be generated.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
				if (o.Primitive == PrimitiveTypes.Long && o.Elements.Count() > 62) Program.AddMessage(new CompileMessage("GS4006", "The number of elements in the Enumeration '" + o.Name + "' exceeds the maximum number of elements (62) supported by the 'long' data type. Any elements above this limit will not be generated.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
				if (o.Primitive == PrimitiveTypes.ULong && o.Elements.Count() > 63) Program.AddMessage(new CompileMessage("GS4006", "The number of elements in the Enumeration '" + o.Name + "' exceeds the maximum number of elements (63) supported by the 'unsigned long' data type. Any elements above this limit will not be generated.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
			}

			if (o.IsFlags)
			{
				bool badValue = true;
				if (o.Primitive == PrimitiveTypes.Int) badValue = false;
				else if (o.Primitive == PrimitiveTypes.UShort) badValue = false;
				else if (o.Primitive == PrimitiveTypes.Short) badValue = false;
				else if (o.Primitive == PrimitiveTypes.UInt) badValue = false;
				else if (o.Primitive == PrimitiveTypes.Long) badValue = false;
				else if (o.Primitive == PrimitiveTypes.ULong) badValue = false;

				if (badValue)
					Program.AddMessage(new CompileMessage("GS4007", "The flags enumeration '" + o.Name + "' must be one of the following types: short, ushort, int, uint, long, ulong.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
			}
		}

		public static string GenerateServerCode30(Projects.Enum o)
		{
			var code = new StringBuilder();
			code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.Append("\t[DataContract(");
			if (o.HasClientType) code.AppendFormat("Name = \"{0}\", ", o.ClientType.Name);
			code.AppendFormat("Namespace = \"{0}\"", o.Parent.URI);
			code.AppendLine(")]");
			if (o.IsFlags) code.AppendLine("[Flags]");
			code.AppendFormat("\t{0} enum {2} : {1}{3}", DataTypeCSGenerator.GenerateScope(o.Scope), DataTypeCSGenerator.GenerateType(o.BaseType), o.Name, Environment.NewLine);
			code.AppendLine("\t{");
			int fv = 0;
			foreach (EnumElement ee in o.Elements.Where(ee => !ee.IsHidden))
				code.Append(o.IsFlags == false ? GenerateElementServerCode(ee) : GenerateElementServerCode(ee, fv++));
			code.AppendLine("\t}");
			return code.ToString();
		}

		public static string GenerateServerCode35(Projects.Enum o)
		{
			return GenerateServerCode40(o);
		}

		public static string GenerateServerCode40(Projects.Enum o)
		{
			return GenerateServerCode45(o);
		}

		public static string GenerateServerCode45(Projects.Enum o)
		{
			var code = new StringBuilder();
			code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.Append("\t[DataContract(");
			if (o.IsReference) code.AppendFormat("IsReference = true ");
			if (o.HasClientType) code.AppendFormat("Name = \"{0}\", ", o.ClientType.Name);
			code.AppendFormat("Namespace = \"{0}\"", o.Parent.URI);
			code.AppendLine(")]");
			if (o.IsFlags) code.AppendLine("\t[Flags]");
			code.AppendFormat("\t{0} enum {2} : {1}{3}", DataTypeCSGenerator.GenerateScope(o.Scope), DataTypeCSGenerator.GenerateType(o.BaseType), o.Name, Environment.NewLine);
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
			code.Append("\t\t[EnumMember(");
			if (!string.IsNullOrEmpty(o.ContractValue))
				code.AppendFormat("Value = \"{0}\"", o.ContractValue);
			code.AppendFormat(")] {0}", o.Name);
			if (!string.IsNullOrEmpty(o.Value))
				code.AppendFormat(" = {0}", o.Value);
			code.AppendLine(",");
			return code.ToString();
		}

		private static string GenerateElementServerCode(EnumElement o, int ElementIndex)
		{
			if (o.Owner.Primitive == PrimitiveTypes.Short && ElementIndex > 14) return "";
			if (o.Owner.Primitive == PrimitiveTypes.UShort && ElementIndex > 15) return "";
			if (o.Owner.Primitive == PrimitiveTypes.Int && ElementIndex > 30) return "";
			if (o.Owner.Primitive == PrimitiveTypes.UInt && ElementIndex > 31) return "";
			if (o.Owner.Primitive == PrimitiveTypes.Long && ElementIndex > 62) return "";
			if (o.Owner.Primitive == PrimitiveTypes.ULong && ElementIndex > 63) return "";

			var code = new StringBuilder();
			code.Append("\t\t");
			if (o.IsExcluded == false) code.Append("[EnumMember()] ");
			code.Append(o.Name);
			if (ElementIndex == 0) code.Append(" = 0");
			if (ElementIndex == 1) code.Append(" = 1");
			if (ElementIndex > 1) code.AppendFormat(" = {0}", Convert.ToInt32(Decimal.Round((decimal)Math.Pow(2, ElementIndex - 1))));
			code.AppendLine(",");
			return code.ToString();
		}

		public static string GenerateProxyCode30(Projects.Enum o)
		{
			var code = new StringBuilder();
			code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.Append("\t[DataContract(");
			if (o.HasClientType) code.AppendFormat("Name = \"{0}\", ", o.ClientType.Name);
			code.AppendFormat("Namespace = \"{0}\"", o.Parent.URI);
			code.AppendLine(")]");
			if (o.IsFlags) code.AppendLine("[Flags]");
			code.AppendFormat("\t{0} enum {2} : {1}{3}", DataTypeCSGenerator.GenerateScope(o.Scope), DataTypeCSGenerator.GenerateType(o.BaseType), o.Name, Environment.NewLine);
			code.AppendLine("\t{");
			int fv = 0;
			foreach (EnumElement ee in o.Elements.Where(ee => !ee.IsHidden))
				code.Append(o.IsFlags == false ? GenerateElementProxyCode(ee) : GenerateElementProxyCode(ee, fv++));
			code.AppendLine("\t}");
			return code.ToString();
		}

		public static string GenerateProxyCode35(Projects.Enum o)
		{
			return GenerateProxyCode40(o);
		}

		public static string GenerateProxyCode40(Projects.Enum o)
		{
			return GenerateProxyCode45(o);
		}

		public static string GenerateProxyCode45(Projects.Enum o)
		{
			var code = new StringBuilder();
			code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.Append("\t[DataContract(");
			if (o.IsReference) code.AppendFormat("IsReference = true ");
			if (o.HasClientType) code.AppendFormat("Name = \"{0}\", ", o.ClientType.Name);
			code.AppendFormat("Namespace = \"{0}\"", o.Parent.URI);
			code.AppendLine(")]");
			if (o.IsFlags) code.AppendLine("\t[Flags]");
			code.AppendFormat("\t{0} enum {2} : {1}{3}", DataTypeCSGenerator.GenerateScope(o.Scope), DataTypeCSGenerator.GenerateType(o.BaseType), o.Name, Environment.NewLine);
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
			code.AppendFormat("\t\t[EnumMember()] {0}", o.Name);
			code.AppendFormat(" = {0}", string.IsNullOrEmpty(o.ContractValue) ? o.Value : o.ContractValue);
			code.AppendLine(",");
			return code.ToString();
		}

		private static string GenerateElementProxyCode(EnumElement o, int ElementIndex)
		{
			if (o.IsExcluded) return "";
			if (o.Owner.Primitive == PrimitiveTypes.Short && ElementIndex > 14) return "";
			if (o.Owner.Primitive == PrimitiveTypes.UShort && ElementIndex > 15) return "";
			if (o.Owner.Primitive == PrimitiveTypes.Int && ElementIndex > 30) return "";
			if (o.Owner.Primitive == PrimitiveTypes.UInt && ElementIndex > 31) return "";
			if (o.Owner.Primitive == PrimitiveTypes.Long && ElementIndex > 62) return "";
			if (o.Owner.Primitive == PrimitiveTypes.ULong && ElementIndex > 63) return "";

			var code = new StringBuilder();
			code.Append("\t\t");
			code.AppendFormat("[EnumMember()] {0}", o.Name);
			if (ElementIndex == 0) code.Append(" = 0");
			if (ElementIndex == 1) code.Append(" = 1");
			if (ElementIndex > 1) code.AppendFormat(" = {0}", Convert.ToInt32(Decimal.Round((decimal)Math.Pow(2, ElementIndex - 1))));
			code.AppendLine(",");
			return code.ToString();
		}
	}
}