using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFArchitect.Projects;

namespace WCFArchitect.Compiler.Generators
{
	internal static class ServiceCSGenerator
	{

		public static void VerifyCode(Service o)
		{
			if (string.IsNullOrEmpty(o.Name))
				Program.AddMessage(new CompileMessage("GS2000", "An service in the '" + o.Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
			else
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.Name) == false)
					Program.AddMessage(new CompileMessage("GS2001", "The service '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
			if (o.HasClientType) { }
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.ClientType.Name) == false)
					Program.AddMessage(new CompileMessage("GS2002", "The service '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));

			foreach (Method m in o.Operations)
			{
				if (string.IsNullOrEmpty(m.Name))
					Program.AddMessage(new CompileMessage("GS2004", "An operation in the '" + o.Name + "' service has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));
				else
					if (Helpers.RegExs.MatchCodeName.IsMatch(m.Name) == false)
						Program.AddMessage(new CompileMessage("GS2005", "The operation '" + m.Name + "' in the '" + o.Name + "' service contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));
				if (string.IsNullOrEmpty(m.ClientName))
					if (Helpers.RegExs.MatchCodeName.IsMatch(m.ClientName) == false)
						Program.AddMessage(new CompileMessage("GS2006", "The operation '" + m.Name + "' in the '" + o.Name + "' service contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));
				if (m.ReturnType == null)
					Program.AddMessage(new CompileMessage("GS2007", "The operation '" + m.Name + "' in the '" + o.Name + "' service has a blank Return Type. A Return Type MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));

				foreach (MethodParameter mp in m.Parameters.Where(mp => string.IsNullOrEmpty(mp.Name)))
					Program.AddMessage(new CompileMessage("GS2008", "The method parameter '" + m.Name + "' in the '" + o.Name + "' service has a parameter with a blank name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));
			}

			foreach (Property p in o.Operations)
			{
				if (string.IsNullOrEmpty(p.Name))
					Program.AddMessage(new CompileMessage("GS2010", "A property in the '" + o.Name + "' service has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o, p, p.GetType(), o.ID, p.ID));
				else
					if (Helpers.RegExs.MatchCodeName.IsMatch(p.Name) == false)
						Program.AddMessage(new CompileMessage("GS2011", "The Property '" + p.Name + "' in the '" + o.Name + "' service contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o, p, p.GetType(), o.ID, p.ID));
			}
		}

		public static string GenerateServerCode30(Service o)
		{
			return GenerateServerCode35(o);
		}

		public static string GenerateServerCode35(Service o)
		{
			return GenerateServerCode40(o);
		}

		public static string GenerateServerCode40(Service o)
		{
			return GenerateServerCode45(o);
		}

		public static string GenerateServerCode45(Service o)
		{
			var code = new StringBuilder();
			if (o.IsCallback == false)
			{
				foreach (DataType dt in o.KnownTypes)
					code.AppendLine(string.Format("\t[KnownType(typeof({0}))]", dt));
				code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
				code.Append("\t[ServiceContract(");
				if (o.Callback != null)
					code.AppendFormat("CallbackContract = typeof(I{0}), ", o.Callback.Name);
				if (!string.IsNullOrEmpty(o.ConfigurationName))
					code.AppendFormat("ConfigurationName = \"{0}\", ", o.ConfigurationName);
				if (o.ProtectionLevel != System.Net.Security.ProtectionLevel.None)
					code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel));
				code.AppendFormat("SessionMode = System.ServiceModel.SessionMode.{0}, ", System.Enum.GetName(typeof(System.ServiceModel.SessionMode), o.SessionMode));
				if (o.HasClientType)
					code.AppendFormat("Name = \"{0}\", ", o.ClientType.Name);
				code.AppendFormat("Namespace = \"{0}\"", o.Parent.URI);
				code.AppendLine(")]");
			}
			code.AppendFormat("\t{0} interface I{1}{2}", DataTypeCSGenerator.GenerateScope(o.Scope), o.Name, Environment.NewLine);
			code.AppendLine("\t{");
			foreach (Property p in o.Operations)
				code.Append(GeneratePropertyServerCode45(p));
			foreach (Method m in o.Operations)
				code.Append(GenerateOperationServerCode45(m));
			code.AppendLine("\t}");
			return code.ToString();
		}

		public static string GenerateClientCode30(Service o)
		{
			return GenerateClientCode35(o);
		}

		public static string GenerateClientCode35(Service o)
		{
			return GenerateClientCode40(o);
		}

		public static string GenerateClientCode40(Service o)
		{
			if (o.IsCallback) return "";
			var code = new StringBuilder();

			//Generate the Client interface
			if (o.ClientType != null)
				foreach (DataType dt in o.ClientType.KnownTypes)
					code.AppendLine(string.Format("\t[KnownType(typeof({0}))]", dt));
			code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t{0} interface {1}{2}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, Environment.NewLine);
			code.AppendLine("\t{");
			foreach (Method m in o.Operations)
				code.AppendLine(GenerateOperationInterfaceCode40(m));
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate Callback Interface (if any)
			if (o.Callback != null)
			{
				code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
				code.AppendFormat("\t{0} interface {1}{2}", o.Callback.HasClientType ? DataTypeCSGenerator.GenerateScope(o.Callback.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Callback.Scope), o.Callback.HasClientType ? o.Callback.ClientType.Name : o.Callback.Name, Environment.NewLine);
				code.AppendLine("\t{");
				foreach (Method m in o.Callback.Operations)
					code.AppendLine(GenerateOperationInterfaceCode40(m));
				code.AppendLine("\t}");
				code.AppendLine();
			}
			//Generate Channel Interface
			code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t{0} interface {1}Channel : {2}, System.ServiceModel.IClientChannel{3}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, o.HasClientType ? DataTypeCSGenerator.GenerateType(o.ClientType) : DataTypeCSGenerator.GenerateType(o), Environment.NewLine);
			code.AppendLine("\t{");
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate the Proxy Class
			code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t{0} partial class {1}Client : System.ServiceModel.ClientBase<{1}>, {1}{2}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, Environment.NewLine);
			code.AppendLine("\t{");
			Host h = o.Parent.Owner.Namespace.GetServiceHost(o);
			if (h != null)
				code.Append(HostCSGenerator.GenerateClientCode40(h));
			foreach (Property p in o.Operations)
				code.AppendLine(GeneratePropertyClientCode(p));
			foreach (Method m in o.Operations)
				code.AppendLine(GenerateOperationProxyCode40(m));
			code.AppendLine("\t}");

			return code.ToString();
		}

		public static string GenerateClientCode45(Service o)
		{
			if (o.IsCallback) return "";
			var code = new StringBuilder();

			//Generate the Client interface
			if (o.ClientType != null)
				foreach (DataType dt in o.ClientType.KnownTypes)
					code.AppendLine(string.Format("\t[KnownType(typeof({0}))]", dt));
			code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t{0} interface {1}{2}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, Environment.NewLine);
			code.AppendLine("\t{");
			foreach (Method m in o.Operations)
				code.AppendLine(GenerateOperationInterfaceCode45(m));
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate Callback Interface (if any)
			if (o.Callback != null)
			{
				code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
				code.AppendFormat("\t{0} interface {1}{2}", o.Callback.HasClientType ? DataTypeCSGenerator.GenerateScope(o.Callback.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Callback.Scope), o.Callback.HasClientType ? o.Callback.ClientType.Name : o.Callback.Name, Environment.NewLine);
				code.AppendLine("\t{");
				foreach (Method m in o.Callback.Operations)
					code.AppendLine(GenerateOperationInterfaceCode45(m));
				code.AppendLine("\t}");
				code.AppendLine();
			}
			//Generate Channel Interface
			code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t{0} interface {1}Channel : {2}, System.ServiceModel.IClientChannel{3}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, o.HasClientType ? DataTypeCSGenerator.GenerateType(o.ClientType) : DataTypeCSGenerator.GenerateType(o), Environment.NewLine);
			code.AppendLine("\t{");
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate the Proxy Class
			code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t{0} partial class {1}Client : System.ServiceModel.ClientBase<{1}>, {1}{2}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, Environment.NewLine);
			code.AppendLine("\t{");
			Host h = o.Parent.Owner.Namespace.GetServiceHost(o);
				if (h != null)
					code.Append(HostCSGenerator.GenerateClientCode45(h));
			foreach (Property p in o.Operations)
				code.AppendLine(GeneratePropertyClientCode(p));
			foreach (Method m in o.Operations)
				code.AppendLine(GenerateOperationProxyCode45(m));
			code.AppendLine("\t}");

			return code.ToString();
		}

		public static string GenerateOperationServerCode30(Method o)
		{
			return GenerateOperationServerCode35(o);
		}

		public static string GenerateOperationServerCode35(Method o)
		{
			return GenerateOperationServerCode40(o);
		}

		public static string GenerateOperationServerCode40(Method o)
		{
			return GenerateOperationServerCode45(o);
		}

		public static string GenerateOperationServerCode45(Method o)
		{
			var code = new StringBuilder();

			code.Append("\t\t[OperationContract(");
			if (o.IsInitiating && o.IsTerminating == false)
				code.Append("IsInitiating = true, ");
			if (o.IsInitiating == false && o.IsTerminating)
				code.Append("IsTerminating = true, ");
			if (o.IsOneWay)
				code.Append("IsOneWay = true, ");
			if (o.ProtectionLevel != System.Net.Security.ProtectionLevel.None)
				code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel));
			if (string.IsNullOrEmpty(o.ClientName))
				code.AppendFormat("Name = \"{0}\", ", o.ClientName);
			if (code.Length > 21) code.Remove(code.Length - 2, 2);
			code.AppendFormat(")] {0} {1}(", DataTypeCSGenerator.GenerateType(o.ReturnType), o.Name);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0},", GenerateMethodParameterServerCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 1, 1);
			code.AppendLine(");");

			if (o.UseAsyncPattern)
			{
				code.Append("\t\t[OperationContract(");
				if (o.IsInitiating && o.IsTerminating == false)
					code.Append("IsInitiating = true, ");
				if (o.IsInitiating == false && o.IsTerminating)
					code.Append("IsTerminating = true, ");
				if (o.IsOneWay)
					code.Append("IsOneWay = true, ");
				if (o.ProtectionLevel != System.Net.Security.ProtectionLevel.None)
					code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel));
				if (string.IsNullOrEmpty(o.ClientName))
					code.AppendFormat("Name = \"{0}\", ", o.ClientName);
				if (code.Length > 21) code.Remove(code.Length - 2, 2);
				code.AppendFormat(")] IAsyncResult Begin{0}(", o.Name);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0},", GenerateMethodParameterServerCode(op));
				code.AppendLine(" AsyncCallback Callback, object AsyncState);");
				code.AppendFormat("\t\t{0} End{1}(IAsyncResult result);{2}", DataTypeCSGenerator.GenerateType(o.ReturnType), o.Name, Environment.NewLine);
			}

			return code.ToString();
		}

		public static string GenerateOperationInterfaceCode30(Method o)
		{
			return GenerateOperationInterfaceCode35(o);
		}

		public static string GenerateOperationInterfaceCode35(Method o)
		{
			return GenerateOperationInterfaceCode40(o);
		}

		//TODO: This functions needs format args from the Action/RelayAction
		public static string GenerateOperationInterfaceCode40(Method o)
		{
			var code = new StringBuilder();

			code.Append("\t\t[OperationContract(");
			code.AppendFormat(o.IsOneWay ? "IsOneWay = true, Action = \"{0}/{1}/{2}\"" : "Action = \"{0}/{1}/{2}\", ReplyAction = \"{0}/{1}/{2}Response\"");
			code.AppendLine(")]");
			code.AppendFormat("\t\t{0} {1}(", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) ? o.ClientName : o.Name);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");

			if (o.UseAsyncPattern)
			{
				code.Append("\t\t[OperationContract(");
				code.AppendFormat(o.IsOneWay ? "IsOneWay = true, Action = \"{0}/{1}/{2}\"" : "Action = \"{0}/{1}/{2}\", ReplyAction = \"{0}/{1}/{2}Response\"");
				code.AppendLine(")]");
				code.AppendFormat("\t\tIAsyncResult {0}(", UseOperationClientName(o) ? o.ClientName : o.Name);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				code.AppendLine("AsyncCallback Callback, object AsyncState);");
				code.AppendFormat("\t\t{0} End{1}(IAsyncResult result);{2}", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) ? o.ClientName : o.Name, Environment.NewLine);
			}

			return code.ToString();
		}

		//TODO: This functions needs format args from the Action/RelayAction
		public static string GenerateOperationInterfaceCode45(Method o)
		{
			var code = new StringBuilder();

			code.Append("\t\t[OperationContract(");
			code.AppendFormat(o.IsOneWay ? "IsOneWay = true, Action = \"{0}/{1}/{2}\"" : "Action = \"{0}/{1}/{2}\", ReplyAction = \"{0}/{1}/{2}Response\"");
			code.AppendLine(")]");
			code.AppendFormat("\t\t{0} {1}(", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) ? o.ClientName : o.Name);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");

			if (o.UseAwaitPattern)
			{
				code.Append("\t\t[OperationContract(");
				code.AppendFormat(o.IsOneWay ? "IsOneWay = true, Action = \"{0}/{1}/{2}\"" : "Action = \"{0}/{1}/{2}\", ReplyAction = \"{0}/{1}/{2}Response\"");
				code.AppendLine(")]");
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tSystem.Threading.Tasks.Task {0}(", UseOperationClientName(o) ? o.ClientName : o.Name);
				else code.AppendFormat("\t\tSystem.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) ? o.ClientName : o.Name);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(");");
			}

			if (o.UseAsyncPattern)
			{
				code.Append("\t\t[OperationContract(");
				code.AppendFormat(o.IsOneWay ? "IsOneWay = true, Action = \"{0}/{1}/{2}\"" : "Action = \"{0}/{1}/{2}\", ReplyAction = \"{0}/{1}/{2}Response\"");
				code.AppendLine(")]");
				code.AppendFormat("\t\tIAsyncResult {0}(", UseOperationClientName(o) ? o.ClientName : o.Name);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				code.AppendLine("AsyncCallback Callback, object AsyncState);");
				code.AppendFormat("\t\t{0} End{1}(IAsyncResult result);{2}", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) ? o.ClientName : o.Name, Environment.NewLine);
			}

			return code.ToString();
		}

		public static string GenerateOperationProxyCode30(Method o)
		{
			return GenerateOperationProxyCode35(o);
		}

		public static string GenerateOperationProxyCode35(Method o)
		{
			return GenerateOperationProxyCode40(o);
		}

		public static string GenerateOperationProxyCode40(Method o)
		{
			var code = new StringBuilder();

			code.AppendFormat("\t\t{0} {1}(", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) ? o.ClientName : o.Name);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			code.AppendFormat("\t\t\treturn base.Channel.{0}(", UseOperationClientName(o) ? o.ClientName : o.Name);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", op.Name);
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");
			code.AppendLine("\t\t}");

			if (o.UseAsyncPattern)
			{
				code.AppendFormat("\t\tIAsyncResult {0}(", UseOperationClientName(o) ? o.ClientName : UseOperationClientName(o) ? o.ClientName : o.Name);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				code.AppendLine("AsyncCallback Callback, object AsyncState)");
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\treturn base.Channel.{0}(", UseOperationClientName(o) ? o.ClientName : UseOperationClientName(o) ? o.ClientName : o.Name);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", op.Name);
				code.AppendLine("Callback, AsyncState);");
				code.AppendLine("\t\t}");
				code.AppendFormat("\t\t{0} End{1}(IAsyncResult result){2}", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) ? o.ClientName : UseOperationClientName(o) ? o.ClientName : o.Name, Environment.NewLine);
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\treturn base.Channel.{0}(", UseOperationClientName(o) ? o.ClientName : UseOperationClientName(o) ? o.ClientName : o.Name);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", op.Name);
				code.AppendLine("Callback, AsyncState);");
				code.AppendLine("\t\t}");
			}

			return code.ToString();
		}

		public static string GenerateOperationProxyCode45(Method o)
		{
			var code = new StringBuilder();

			code.AppendFormat("\t\t{0} {1}(", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) ? o.ClientName : o.Name);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			code.AppendFormat("\t\t\treturn base.Channel.{0}(", UseOperationClientName(o) ? o.ClientName : o.Name);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", op.Name);
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");
			code.AppendLine("\t\t}");

			if (o.UseAwaitPattern)
			{
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tSystem.Threading.Tasks.Task {0}(", UseOperationClientName(o) ? o.ClientName : UseOperationClientName(o) ? o.ClientName : o.Name);
				else code.AppendFormat("\t\tSystem.Threading.Tasks.Task<{0}> {1}(", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) ? o.ClientName : UseOperationClientName(o) ? o.ClientName : o.Name);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(");");
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\treturn base.Channel.{0}Async(", UseOperationClientName(o) ? o.ClientName : UseOperationClientName(o) ? o.ClientName : o.Name);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", op.Name);
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(");");
				code.AppendLine("\t\t}");
			}

			if (o.UseAsyncPattern)
			{
				code.AppendFormat("\t\tIAsyncResult {0}(", UseOperationClientName(o) ? o.ClientName : UseOperationClientName(o) ? o.ClientName : o.Name);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				code.AppendLine("AsyncCallback Callback, object AsyncState)");
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\treturn base.Channel.{0}(", UseOperationClientName(o) ? o.ClientName : UseOperationClientName(o) ? o.ClientName : o.Name);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", op.Name);
				code.AppendLine("Callback, AsyncState);");
				code.AppendLine("\t\t}");
				code.AppendFormat("\t\t{0} End{1}(IAsyncResult result){2}", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) ? o.ClientName : UseOperationClientName(o) ? o.ClientName : o.Name, Environment.NewLine);
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\treturn base.Channel.{0}(", UseOperationClientName(o) ? o.ClientName : UseOperationClientName(o) ? o.ClientName : o.Name);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", op.Name);
				code.AppendLine("Callback, AsyncState);");
				code.AppendLine("\t\t}");
			}

			return code.ToString();
		}

		public static string GenerateMethodParameterServerCode(MethodParameter o)
		{
			return o.IsHidden ? "" : string.Format("{0} {1}", DataTypeCSGenerator.GenerateType(o.Type), o.Name);
		}

		public static string GenerateMethodParameterClientCode(MethodParameter o)
		{
			if (o.IsHidden) return "";

			if (o.Type.TypeMode == DataTypeMode.Class)
			{
				var ptype = o.Type as Data;
				return string.Format("{0} {1}", ptype != null && ptype.HasClientType ? DataTypeCSGenerator.GenerateType(ptype.ClientType) : DataTypeCSGenerator.GenerateType(o.Type), o.Name);
			}													    
			if (o.Type.TypeMode == DataTypeMode.Struct)			    
			{													    
				var ptype = o.Type as Data;						    
				return string.Format("{0} {1}", ptype != null && ptype.HasClientType ? DataTypeCSGenerator.GenerateType(ptype.ClientType) : DataTypeCSGenerator.GenerateType(o.Type), o.Name);
			}													    
			if (o.Type.TypeMode == DataTypeMode.Enum)			    
			{													    
				var ptype = o.Type as Projects.Enum;			    
				return string.Format("{0} {1}", ptype != null && ptype.HasClientType ? DataTypeCSGenerator.GenerateType(ptype.ClientType) : DataTypeCSGenerator.GenerateType(o.Type), o.Name);
			}
			
			return string.Format("{0} {1}", DataTypeCSGenerator.GenerateType(o.Type), o.Name);
		}

		public static bool UseOperationClientName(Operation o)
		{
			return !string.IsNullOrEmpty(o.ClientName);
		}

		public static string GeneratePropertyServerCode30(Property o)
		{
			return GeneratePropertyServerCode35(o);
		}

		public static string GeneratePropertyServerCode35(Property o)
		{
			return GeneratePropertyServerCode40(o);
		}

		public static string GeneratePropertyServerCode40(Property o)
		{
			return GeneratePropertyServerCode45(o);
		}

		public static string GeneratePropertyServerCode45(Property o)
		{
			var code = new StringBuilder();
			code.AppendFormat("\t\t{0} {1} {{ ", DataTypeCSGenerator.GenerateType(o.ReturnType), o.Name);
			if (o.IsReadOnly == false)
			{
				//Write Getter Code
				code.Append("[OperationContract(");
				if (o.IsOneWay)
					code.Append("IsOneWay = true, ");
				if (o.ProtectionLevel != System.Net.Security.ProtectionLevel.None)
					code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel));
				code.AppendFormat("Name = \"Get{0}\"", o.Name);
				code.Append(")] get; ");

				//Write Setter Code
				code.Append("[OperationContract(");
				if (o.IsOneWay)
					code.Append("IsOneWay = true, ");
				if (o.ProtectionLevel != System.Net.Security.ProtectionLevel.None)
					code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel));
				code.AppendFormat("Name = \"Set{0}\"", o.Name);
				code.Append(")] set;");
			}
			else
			{
				//Write Getter Code
				code.Append("[OperationContract(");
				if (o.IsOneWay)
					code.Append("IsOneWay = true, ");
				if (o.ProtectionLevel != System.Net.Security.ProtectionLevel.None)
					code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel));
				code.AppendFormat("Name = \"Get{0}\"", o.Name);
				code.Append(")] get; ");
			}
			code.AppendLine(" }");

			return code.ToString();
		}

		public static string GeneratePropertyClientCode(Property o)
		{
			return string.Format(o.IsReadOnly == false ? "\t\tpublic {0} {1} {{ get {{ return Get{1}(); }} set {{ Set{1}(value); }} }}" : "\t\tpublic {0} {1} {{ get {{ return Get{1}(); }} }}", DataTypeCSGenerator.GenerateType(o.ReturnType), o.Name);
		}
	}
}