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
		public static bool VerifyCode(Projects.Enum o)
		{
			bool NoErrors = true;

			if (o.Name == "" || o.Name == null)
			{
				Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4000", "The enumeration '" + o.Name + "' in the '" + o.Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.Name) == false)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4001", "The enumeration '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
					NoErrors = false;
				}

			if (o.HasClientType == true)
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.ClientType.Name) == false)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4002", "The enumeration '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
					NoErrors = false;
				}

			foreach (EnumElement E in o.Elements)
			{
				if (E.Name == "" || E.Name == null)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4003", "The enumeration '" + o.Name + "' in the '" + o.Parent.Name + "' namespace has an enumeration element with a blank Name. A Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o, E, E.GetType()));
					NoErrors = false;
				}
				else
					if (Helpers.RegExs.MatchCodeName.IsMatch(E.Name) == false)
					{
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4004", "The enumeration element '" + E.Name + "' in the '" + o.Name + "' enumeration contains invalid characters in the Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o, E, E.GetType()));
						NoErrors = false;
					}

				if (o.IsFlags == false)
				{
					bool BadValue = false;
					if (o.Primitive == Projects.PrimitiveTypes.Int)
						try { Convert.ToInt32(E.Value); }
						catch { BadValue = true; }
					else if (o.Primitive == Projects.PrimitiveTypes.SByte)
						try { Convert.ToSByte(E.Value); }
						catch { BadValue = true; }
					else if (o.Primitive == Projects.PrimitiveTypes.Byte)
						try { Convert.ToByte(E.Value); }
						catch { BadValue = true; }
					else if (o.Primitive == Projects.PrimitiveTypes.UShort)
						try { Convert.ToUInt16(E.Value); }
						catch { BadValue = true; }
					else if (o.Primitive == Projects.PrimitiveTypes.Short)
						try { Convert.ToInt16(E.Value); }
						catch { BadValue = true; }
					else if (o.Primitive == Projects.PrimitiveTypes.UInt)
						try { Convert.ToUInt32(E.Value); }
						catch { BadValue = true; }
					else if (o.Primitive == Projects.PrimitiveTypes.Long)
						try { Convert.ToInt64(E.Value); }
						catch { BadValue = true; }
					else if (o.Primitive == Projects.PrimitiveTypes.ULong)
						try { Convert.ToUInt64(E.Value); }
						catch { BadValue = true; }

					if (E.Value == "") BadValue = false;

					if (BadValue == true)
					{
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4005", "The enumeration element '" + E.Name + "' in the '" + o.Name + "' enumeration contains an invalid value.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o, E, E.GetType()));
						NoErrors = false;
					}
				}
			}

			if (o.IsFlags == true)
			{
				if (o.Primitive == Projects.PrimitiveTypes.Short && o.Elements.Count() > 14) Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4006", "The number of elements in the Enumeration '" + o.Name + "' exceeds the maximum number of elements (14) supported by the 'short' data type. Any elements above this limit will not be generated.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, o.Parent, o, o.GetType()));
				if (o.Primitive == Projects.PrimitiveTypes.UShort && o.Elements.Count() > 15) Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4006", "The number of elements in the Enumeration '" + o.Name + "' exceeds the maximum number of elements (15) supported by the 'unsigned short' data type. Any elements above this limit will not be generated.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, o.Parent, o, o.GetType()));
				if (o.Primitive == Projects.PrimitiveTypes.Int && o.Elements.Count() > 30) Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4006", "The number of elements in the Enumeration '" + o.Name + "' exceeds the maximum number of elements (30) supported by the 'int' data type. Any elements above this limit will not be generated.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, o.Parent, o, o.GetType()));
				if (o.Primitive == Projects.PrimitiveTypes.UInt && o.Elements.Count() > 31) Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4006", "The number of elements in the Enumeration '" + o.Name + "' exceeds the maximum number of elements (31) supported by the 'unsigned int' data type. Any elements above this limit will not be generated.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, o.Parent, o, o.GetType()));
				if (o.Primitive == Projects.PrimitiveTypes.Long && o.Elements.Count() > 62) Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4006", "The number of elements in the Enumeration '" + o.Name + "' exceeds the maximum number of elements (62) supported by the 'long' data type. Any elements above this limit will not be generated.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, o.Parent, o, o.GetType()));
				if (o.Primitive == Projects.PrimitiveTypes.ULong && o.Elements.Count() > 63) Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4006", "The number of elements in the Enumeration '" + o.Name + "' exceeds the maximum number of elements (63) supported by the 'unsigned long' data type. Any elements above this limit will not be generated.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, o.Parent, o, o.GetType()));
			}

			if (o.IsFlags == true)
			{
				bool BadValue = true;
				if (o.Primitive == Projects.PrimitiveTypes.Int) BadValue = false;
				else if (o.Primitive == Projects.PrimitiveTypes.UShort) BadValue = false;
				else if (o.Primitive == Projects.PrimitiveTypes.Short) BadValue = false;
				else if (o.Primitive == Projects.PrimitiveTypes.UInt) BadValue = false;
				else if (o.Primitive == Projects.PrimitiveTypes.Long) BadValue = false;
				else if (o.Primitive == Projects.PrimitiveTypes.ULong) BadValue = false;

				if (BadValue == true)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4007", "The flags enumeration '" + o.Name + "' must be one of the following types: short, ushort, int, uint, long, ulong.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
					NoErrors = false;
				}
			}

			return NoErrors;
		}

		public static string GenerateServerCode30(Projects.Enum o)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.Append("\t[DataContract(");
			if (o.HasClientType == true) Code.AppendFormat("Name = \"{0}\", ", o.ClientType.Name);
			Code.AppendFormat("Namespace = \"{0}\"", o.Parent.URI);
			Code.AppendLine(")]");
			if (o.IsFlags == true) Code.AppendLine("[Flags]");
			Code.AppendFormat("\t{0} enum {2} : {1}{3}", DataTypeCSGenerator.GenerateScope(o.Scope), DataTypeCSGenerator.GenerateType(o.BaseType), o.Name, Environment.NewLine);
			Code.AppendLine("\t{");
			int FV = 0;
			foreach (EnumElement EE in o.Elements)
			{
				if (EE.IsHidden == true) continue;
				if (o.IsFlags == false) Code.Append(GenerateElementServerCode(EE));
				else Code.Append(GenerateElementServerCode(EE, FV++));
			}
			Code.AppendLine("\t}");
			return Code.ToString();
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
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.Append("\t[DataContract(");
			if (o.IsReference == true) Code.AppendFormat("IsReference = true ");
			if (o.HasClientType == true) Code.AppendFormat("Name = \"{0}\", ", o.ClientType.Name);
			Code.AppendFormat("Namespace = \"{0}\"", o.Parent.URI);
			Code.AppendLine(")]");
			if (o.IsFlags == true) Code.AppendLine("\t[Flags]");
			Code.AppendFormat("\t{0} enum {2} : {1}{3}", DataTypeCSGenerator.GenerateScope(o.Scope), DataTypeCSGenerator.GenerateType(o.BaseType), o.Name, Environment.NewLine);
			Code.AppendLine("\t{");
			int FV = 0;
			foreach (EnumElement EE in o.Elements)
			{
				if (EE.IsHidden == true) continue;
				if (o.IsFlags == false) Code.Append(GenerateElementServerCode(EE));
				else Code.Append(GenerateElementServerCode(EE, FV++));
			}
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		private static string GenerateElementServerCode(EnumElement o)
		{
			StringBuilder Code = new StringBuilder();
			Code.Append("\t\t[EnumMember(");
			if (o.ContractValue == "" || o.ContractValue == null) { }
			else
				Code.AppendFormat("Value = \"{0}\"", o.ContractValue);
			Code.AppendFormat(")] {0}", o.Name);
			if (o.Value == "" || o.Value == null) { }
			else Code.AppendFormat(" = {0}", o.Value);
			Code.AppendLine(",");
			return Code.ToString();
		}

		private static string GenerateElementServerCode(EnumElement o, int ElementIndex)
		{
			if (o.Owner.Primitive == PrimitiveTypes.Short && ElementIndex > 14) return "";
			if (o.Owner.Primitive == PrimitiveTypes.UShort && ElementIndex > 15) return "";
			if (o.Owner.Primitive == PrimitiveTypes.Int && ElementIndex > 30) return "";
			if (o.Owner.Primitive == PrimitiveTypes.UInt && ElementIndex > 31) return "";
			if (o.Owner.Primitive == PrimitiveTypes.Long && ElementIndex > 62) return "";
			if (o.Owner.Primitive == PrimitiveTypes.ULong && ElementIndex > 63) return "";

			StringBuilder Code = new StringBuilder();
			Code.Append("\t\t");
			if (o.IsExcluded == false) Code.Append("[EnumMember()] ");
			Code.Append(o.Name);
			if (ElementIndex == 0) Code.Append(" = 0");
			if (ElementIndex == 1) Code.Append(" = 1");
			if (ElementIndex > 1) Code.AppendFormat(" = {0}", Convert.ToInt32(Decimal.Round((decimal)Math.Pow(2, ElementIndex - 1))));
			Code.AppendLine(",");
			return Code.ToString();
		}

		public static string GenerateProxyCode30(Projects.Enum o)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.Append("\t[DataContract(");
			if (o.HasClientType == true) Code.AppendFormat("Name = \"{0}\", ", o.ClientType.Name);
			Code.AppendFormat("Namespace = \"{0}\"", o.Parent.URI);
			Code.AppendLine(")]");
			if (o.IsFlags == true) Code.AppendLine("[Flags]");
			Code.AppendFormat("\t{0} enum {2} : {1}{3}", DataTypeCSGenerator.GenerateScope(o.Scope), DataTypeCSGenerator.GenerateType(o.BaseType), o.Name, Environment.NewLine);
			Code.AppendLine("\t{");
			int FV = 0;
			foreach (EnumElement EE in o.Elements)
			{
				if (EE.IsHidden == true) continue;
				if (o.IsFlags == false) Code.Append(GenerateElementProxyCode(EE));
				else Code.Append(GenerateElementProxyCode(EE, FV++));
			}
			Code.AppendLine("\t}");
			return Code.ToString();
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
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.Append("\t[DataContract(");
			if (o.IsReference == true) Code.AppendFormat("IsReference = true ");
			if (o.HasClientType == true) Code.AppendFormat("Name = \"{0}\", ", o.ClientType.Name);
			Code.AppendFormat("Namespace = \"{0}\"", o.Parent.URI);
			Code.AppendLine(")]");
			if (o.IsFlags == true) Code.AppendLine("\t[Flags]");
			Code.AppendFormat("\t{0} enum {2} : {1}{3}", DataTypeCSGenerator.GenerateScope(o.Scope), DataTypeCSGenerator.GenerateType(o.BaseType), o.Name, Environment.NewLine);
			Code.AppendLine("\t{");
			int FV = 0;
			foreach (EnumElement EE in o.Elements)
			{
				if (EE.IsHidden == true) continue;
				if (o.IsFlags == false) Code.Append(GenerateElementProxyCode(EE));
				else Code.Append(GenerateElementProxyCode(EE, FV++));
			}
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		private static string GenerateElementProxyCode(EnumElement o)
		{
			if (o.IsExcluded == true) return "";
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t\t[EnumMember()] {0}", o.Name);
			if (o.ContractValue == "" || o.ContractValue == null) 
				Code.AppendFormat(" = {0}", o.Value);
			else
				Code.AppendFormat(" = {0}", o.ContractValue);
			Code.AppendLine(",");
			return Code.ToString();
		}

		private static string GenerateElementProxyCode(EnumElement o, int ElementIndex)
		{
			if (o.IsExcluded == true) return "";
			if (o.Owner.Primitive == PrimitiveTypes.Short && ElementIndex > 14) return "";
			if (o.Owner.Primitive == PrimitiveTypes.UShort && ElementIndex > 15) return "";
			if (o.Owner.Primitive == PrimitiveTypes.Int && ElementIndex > 30) return "";
			if (o.Owner.Primitive == PrimitiveTypes.UInt && ElementIndex > 31) return "";
			if (o.Owner.Primitive == PrimitiveTypes.Long && ElementIndex > 62) return "";
			if (o.Owner.Primitive == PrimitiveTypes.ULong && ElementIndex > 63) return "";

			StringBuilder Code = new StringBuilder();
			Code.Append("\t\t");
			Code.AppendFormat("[EnumMember()] {0}", o.Name);
			if (ElementIndex == 0) Code.Append(" = 0");
			if (ElementIndex == 1) Code.Append(" = 1");
			if (ElementIndex > 1) Code.AppendFormat(" = {0}", Convert.ToInt32(Decimal.Round((decimal)Math.Pow(2, ElementIndex - 1))));
			Code.AppendLine(",");
			return Code.ToString();
		}
	}
}