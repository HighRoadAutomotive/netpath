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

		public static bool VerifyCode(Service o)
		{
			bool NoErrors = true;

			if (o.Name == "" || o.Name == null)
			{
				Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2000", "An service in the '" + o.Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.Name) == false)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2001", "The service '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Code Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
					NoErrors = false;
				}
			if (o.HasClientType == true) { }
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.ClientType.Name) == false)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2002", "The service '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
					NoErrors = false;
				}

			foreach (Method O in o.Operations)
			{
				if (O.Name == "" || O.Name == null)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2004", "An operation in the '" + o.Name + "' service has a blank Code Name. A Code Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o, O, O.GetType()));
					NoErrors = false;
				}
				else
					if (Helpers.RegExs.MatchCodeName.IsMatch(O.Name) == false)
					{
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2005", "The operation '" + O.Name + "' in the '" + o.Name + "' service contains invalid characters in the Code Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o, O, O.GetType()));
						NoErrors = false;
					}
				if (O.ClientName == "" || O.ClientName == null)
					if (Helpers.RegExs.MatchCodeName.IsMatch(O.ClientName) == false)
					{
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2006", "The operation '" + O.Name + "' in the '" + o.Name + "' service contains invalid characters in the Client Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o, O, O.GetType()));
						NoErrors = false;
					}
				if (O.ReturnType == null)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2007", "The operation '" + O.Name + "' in the '" + o.Name + "' service has a blank Return Type. A Return Type MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o, O, O.GetType()));
					NoErrors = false;
				}

				foreach (MethodParameter OP in O.Parameters)
				{
					if (OP.Name == "" || OP.Name == null)
					{
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2008", "The operation '" + O.Name + "' in the '" + o.Name + "' service has a parameter with a blank name. A Parameter Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o, O, O.GetType()));
						NoErrors = false;
					}
				}
			}

			foreach (Property P in o.Operations)
			{
				if (P.Name == "" || P.Name == null)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2010", "A property in the '" + o.Name + "' service has a blank Code Name. A Code Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o, P, P.GetType()));
					NoErrors = false;
				}
				else
					if (Helpers.RegExs.MatchCodeName.IsMatch(P.Name) == false)
					{
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2011", "The Property '" + P.Name + "' in the '" + o.Name + "' service contains invalid characters in the Code Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o, P, P.GetType()));
						NoErrors = false;
					}
			}

			return NoErrors;
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
			StringBuilder Code = new StringBuilder();
			if (o.IsCallback == false)
			{
				Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
				Code.Append("\t[ServiceContract(");
				if (o.Callback != null)
					Code.AppendFormat("CallbackContract = typeof(I{0}), ", o.Callback.Name);
				if (o.ConfigurationName != "" && o.ConfigurationName != null)
					Code.AppendFormat("ConfigurationName = \"{0}\", ", o.ConfigurationName);
				if (o.ProtectionLevel != System.Net.Security.ProtectionLevel.None)
					Code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel));
				Code.AppendFormat("SessionMode = System.ServiceModel.SessionMode.{0}, ", System.Enum.GetName(typeof(System.ServiceModel.SessionMode), o.SessionMode));
				if (o.HasClientType == true)
					Code.AppendFormat("Name = \"{0}\", ", o.ClientType.Name);
				Code.AppendFormat("Namespace = \"{0}\"", o.Parent.URI);
				Code.AppendLine(")]");
			}
			Code.AppendFormat("\t{0} interface I{1}{2}", DataTypeCSGenerator.GenerateScope(o.Scope), o.Name, Environment.NewLine);
			Code.AppendLine("\t{");
			foreach (Property P in o.Operations)
				Code.Append(GeneratePropertyServerCode45(P));
			foreach (Method M in o.Operations)
				Code.Append(GenerateOperationServerCode45(M));
			Code.AppendLine("\t}");
			return Code.ToString();
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
			if (o.IsCallback == true) return "";
			StringBuilder Code = new StringBuilder();

			//Generate the Client interface
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0} interface {1}{2}", o.HasClientType == true ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType == true ? o.ClientType.Name : o.Name, Environment.NewLine);
			Code.AppendLine("\t{");
			foreach (Method M in o.Operations)
				Code.AppendLine(GenerateOperationInterfaceCode40(M));
			Code.AppendLine("\t}");
			Code.AppendLine();
			//Generate Callback Interface (if any)
			if (o.Callback != null)
			{
				Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
				Code.AppendFormat("\t{0} interface {1}{2}", o.Callback.HasClientType == true ? DataTypeCSGenerator.GenerateScope(o.Callback.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Callback.Scope), o.Callback.HasClientType == true ? o.Callback.ClientType.Name : o.Callback.Name, Environment.NewLine);
				Code.AppendLine("\t{");
				foreach (Method M in o.Callback.Operations)
					Code.AppendLine(GenerateOperationInterfaceCode40(M));
				Code.AppendLine("\t}");
				Code.AppendLine();
			}
			//Generate Channel Interface
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0} interface {1}Channel : {2}, System.ServiceModel.IClientChannel{3}", o.HasClientType == true ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType == true ? o.ClientType.Name : o.Name, o.HasClientType == true ? DataTypeCSGenerator.GenerateType(o.ClientType) : DataTypeCSGenerator.GenerateType(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendLine("\t}");
			Code.AppendLine();
			//Generate the Proxy Class
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0} partial class {1}Client : System.ServiceModel.ClientBase<{1}>, {1}{3}", o.HasClientType == true ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType == true ? o.ClientType.Name : o.Name, Environment.NewLine);
			Code.AppendLine("\t{");
			Host H = o.Parent.Owner.Namespace.GetServiceHost(o);
			if (H != null)
				Code.Append(HostCSGenerator.GenerateClientCode40(H));
			foreach (Property P in o.Operations)
				Code.AppendLine(GeneratePropertyClientCode(P));
			foreach (Method M in o.Operations)
				Code.AppendLine(GenerateOperationProxyCode40(M));
			Code.AppendLine("\t}");

			return Code.ToString();
		}

		public static string GenerateClientCode45(Service o)
		{
			if (o.IsCallback == true) return "";
			StringBuilder Code = new StringBuilder();

			//Generate the Client interface
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0} interface {1}{2}", o.HasClientType == true ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType == true ? o.ClientType.Name : o.Name, Environment.NewLine);
			Code.AppendLine("\t{");
			foreach (Method M in o.Operations)
				Code.AppendLine(GenerateOperationInterfaceCode45(M));
			Code.AppendLine("\t}");
			Code.AppendLine();
			//Generate Callback Interface (if any)
			if (o.Callback != null)
			{
				Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
				Code.AppendFormat("\t{0} interface {1}{2}", o.Callback.HasClientType == true ? DataTypeCSGenerator.GenerateScope(o.Callback.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Callback.Scope), o.Callback.HasClientType == true ? o.Callback.ClientType.Name : o.Callback.Name, Environment.NewLine);
				Code.AppendLine("\t{");
				foreach (Method M in o.Callback.Operations)
					Code.AppendLine(GenerateOperationInterfaceCode45(M));
				Code.AppendLine("\t}");
				Code.AppendLine();
			}
			//Generate Channel Interface
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0} interface {1}Channel : {2}, System.ServiceModel.IClientChannel{3}", o.HasClientType == true ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType == true ? o.ClientType.Name : o.Name, o.HasClientType == true ? DataTypeCSGenerator.GenerateType(o.ClientType) : DataTypeCSGenerator.GenerateType(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendLine("\t}");
			Code.AppendLine();
			//Generate the Proxy Class
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0} partial class {1}Client : System.ServiceModel.ClientBase<{1}>, {1}{3}", o.HasClientType == true ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType == true ? o.ClientType.Name : o.Name, Environment.NewLine);
			Code.AppendLine("\t{");
			Host H = o.Parent.Owner.Namespace.GetServiceHost(o);
				if (H != null)
					Code.Append(HostCSGenerator.GenerateClientCode45(H));
			foreach (Property P in o.Operations)
				Code.AppendLine(GeneratePropertyClientCode(P));
			foreach (Method M in o.Operations)
				Code.AppendLine(GenerateOperationProxyCode45(M));
			Code.AppendLine("\t}");

			return Code.ToString();
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
			StringBuilder Code = new StringBuilder();

			Code.Append("\t\t[OperationContract(");
			if (o.IsInitiating == true && o.IsTerminating == false)
				Code.Append("IsInitiating = true, ");
			if (o.IsInitiating == false && o.IsTerminating == true)
				Code.Append("IsTerminating = true, ");
			if (o.IsOneWay == true)
				Code.Append("IsOneWay = true, ");
			if (o.ProtectionLevel != System.Net.Security.ProtectionLevel.None)
				Code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel));
			if (o.ClientName == "" || o.ClientName == null)
				Code.AppendFormat("Name = \"{0}\", ", o.ClientName);
			if (Code.Length > 21) Code.Remove(Code.Length - 2, 2);
			Code.AppendFormat(")] {0} {1}(", DataTypeCSGenerator.GenerateType(o.ReturnType), o.Name);
			foreach (MethodParameter OP in o.Parameters)
				Code.AppendFormat("{0},", GenerateMethodParameterServerCode(OP));
			if (o.Parameters.Count > 0) Code.Remove(Code.Length - 1, 1);
			Code.AppendLine(");");

			if (o.UseAsyncPattern == true)
			{
				Code.Append("\t\t[OperationContract(");
				if (o.IsInitiating == true && o.IsTerminating == false)
					Code.Append("IsInitiating = true, ");
				if (o.IsInitiating == false && o.IsTerminating == true)
					Code.Append("IsTerminating = true, ");
				if (o.IsOneWay == true)
					Code.Append("IsOneWay = true, ");
				if (o.ProtectionLevel != System.Net.Security.ProtectionLevel.None)
					Code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel));
				if (o.ClientName == "" || o.ClientName == null)
					Code.AppendFormat("Name = \"{0}\", ", o.ClientName);
				if (Code.Length > 21) Code.Remove(Code.Length - 2, 2);
				Code.AppendFormat(")] IAsyncResult Begin{0}(", o.Name);
				foreach (MethodParameter OP in o.Parameters)
					Code.AppendFormat("{0},", GenerateMethodParameterServerCode(OP));
				Code.AppendLine(" AsyncCallback Callback, object AsyncState);");
				Code.AppendFormat("\t\t{0} End{1}(IAsyncResult result);{2}", DataTypeCSGenerator.GenerateType(o.ReturnType), o.Name, Environment.NewLine);
			}

			return Code.ToString();
		}

		public static string GenerateOperationInterfaceCode30(Method o)
		{
			return GenerateOperationInterfaceCode35(o);
		}

		public static string GenerateOperationInterfaceCode35(Method o)
		{
			return GenerateOperationInterfaceCode40(o);
		}

		public static string GenerateOperationInterfaceCode40(Method o)
		{
			StringBuilder Code = new StringBuilder();

			Code.Append("\t\t[OperationContract(");
			if (o.IsOneWay == true)
				Code.AppendFormat("IsOneWay = true, Action = \"{0}/{1}/{2}\"");
			else
				Code.AppendFormat("Action = \"{0}/{1}/{2}\", ReplyAction = \"{0}/{1}/{2}Response\"");
			Code.AppendLine(")]");
			Code.AppendFormat("\t\t{0} {1}(", o.ReturnType.HasClientType == true ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) == true ? o.ClientName : o.Name);
			foreach (MethodParameter OP in o.Parameters)
				Code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(OP));
			if (o.Parameters.Count > 0) Code.Remove(Code.Length - 2, 2);
			Code.AppendLine(");");

			if (o.UseAsyncPattern == true)
			{
				Code.Append("\t\t[OperationContract(");
				if (o.IsOneWay == true)
					Code.AppendFormat("IsOneWay = true, Action = \"{0}/{1}/{2}\"");
				else
					Code.AppendFormat("Action = \"{0}/{1}/{2}\", ReplyAction = \"{0}/{1}/{2}Response\"");
				Code.AppendLine(")]");
				Code.AppendFormat("\t\tIAsyncResult {0}(", UseOperationClientName(o) == true ? o.ClientName : o.Name);
				foreach (MethodParameter OP in o.Parameters)
					Code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(OP));
				Code.AppendLine("AsyncCallback Callback, object AsyncState);");
				Code.AppendFormat("\t\t{0} End{1}(IAsyncResult result);{2}", o.ReturnType.HasClientType == true ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) == true ? o.ClientName : o.Name, Environment.NewLine);
			}

			return Code.ToString();
		}

		public static string GenerateOperationInterfaceCode45(Method o)
		{
			StringBuilder Code = new StringBuilder();

			Code.Append("\t\t[OperationContract(");
			if (o.IsOneWay == true)
				Code.AppendFormat("IsOneWay = true, Action = \"{0}/{1}/{2}\"");
			else
				Code.AppendFormat("Action = \"{0}/{1}/{2}\", ReplyAction = \"{0}/{1}/{2}Response\"");
			Code.AppendLine(")]");
			Code.AppendFormat("\t\t{0} {1}(", o.ReturnType.HasClientType == true ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) == true ? o.ClientName : o.Name);
			foreach (MethodParameter OP in o.Parameters)
				Code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(OP));
			if (o.Parameters.Count > 0) Code.Remove(Code.Length - 2, 2);
			Code.AppendLine(");");

			if (o.UseAwaitPattern == true)
			{
				Code.Append("\t\t[OperationContract(");
				if (o.IsOneWay == true)
					Code.AppendFormat("IsOneWay = true, Action = \"{0}/{1}/{2}\"");
				else
					Code.AppendFormat("Action = \"{0}/{1}/{2}\", ReplyAction = \"{0}/{1}/{2}Response\"");
				Code.AppendLine(")]");
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) Code.AppendFormat("\t\tSystem.Threading.Tasks.Task {0}(", UseOperationClientName(o) == true ? o.ClientName : o.Name);
				else Code.AppendFormat("\t\tSystem.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType == true ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) == true ? o.ClientName : o.Name);
				foreach (MethodParameter OP in o.Parameters)
					Code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(OP));
				if (o.Parameters.Count > 0) Code.Remove(Code.Length - 2, 2);
				Code.AppendLine(");");
			}

			if (o.UseAsyncPattern == true)
			{
				Code.Append("\t\t[OperationContract(");
				if (o.IsOneWay == true)
					Code.AppendFormat("IsOneWay = true, Action = \"{0}/{1}/{2}\"");
				else
					Code.AppendFormat("Action = \"{0}/{1}/{2}\", ReplyAction = \"{0}/{1}/{2}Response\"");
				Code.AppendLine(")]");
				Code.AppendFormat("\t\tIAsyncResult {0}(", UseOperationClientName(o) == true ? o.ClientName : o.Name);
				foreach (MethodParameter OP in o.Parameters)
					Code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(OP));
				Code.AppendLine("AsyncCallback Callback, object AsyncState);");
				Code.AppendFormat("\t\t{0} End{1}(IAsyncResult result);{2}", o.ReturnType.HasClientType == true ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) == true ? o.ClientName : o.Name, Environment.NewLine);
			}

			return Code.ToString();
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
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("\t\t{0} {1}(", o.ReturnType.HasClientType == true ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) == true ? o.ClientName : o.Name);
			foreach (MethodParameter OP in o.Parameters)
				Code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(OP));
			if (o.Parameters.Count > 0) Code.Remove(Code.Length - 2, 2);
			Code.AppendLine(")");
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\treturn base.Channel.{0}(", UseOperationClientName(o) == true ? o.ClientName : o.Name);
			foreach (MethodParameter op in o.Parameters)
				Code.AppendFormat("{0}, ", op.Name);
			if (o.Parameters.Count > 0) Code.Remove(Code.Length - 2, 2);
			Code.AppendLine(");");
			Code.AppendLine("\t\t}");

			if (o.UseAsyncPattern == true)
			{
				Code.AppendFormat("\t\tIAsyncResult {0}(", UseOperationClientName(o) == true ? o.ClientName : UseOperationClientName(o) == true ? o.ClientName : o.Name);
				foreach (MethodParameter OP in o.Parameters)
					Code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(OP));
				Code.AppendLine("AsyncCallback Callback, object AsyncState)");
				Code.AppendLine("\t\t{");
				Code.AppendFormat("\t\t\treturn base.Channel.{0}(", UseOperationClientName(o) == true ? o.ClientName : UseOperationClientName(o) == true ? o.ClientName : o.Name);
				foreach (MethodParameter op in o.Parameters)
					Code.AppendFormat("{0}, ", op.Name);
				Code.AppendLine("Callback, AsyncState);");
				Code.AppendLine("\t\t}");
				Code.AppendFormat("\t\t{0} End{1}(IAsyncResult result){2}", o.ReturnType.HasClientType == true ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) == true ? o.ClientName : UseOperationClientName(o) == true ? o.ClientName : o.Name, Environment.NewLine);
				Code.AppendLine("\t\t{");
				Code.AppendFormat("\t\t\treturn base.Channel.{0}(", UseOperationClientName(o) == true ? o.ClientName : UseOperationClientName(o) == true ? o.ClientName : o.Name);
				foreach (MethodParameter op in o.Parameters)
					Code.AppendFormat("{0}, ", op.Name);
				Code.AppendLine("Callback, AsyncState);");
				Code.AppendLine("\t\t}");
			}

			return Code.ToString();
		}

		public static string GenerateOperationProxyCode45(Method o)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("\t\t{0} {1}(", o.ReturnType.HasClientType == true ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) == true ? o.ClientName : o.Name);
			foreach (MethodParameter OP in o.Parameters)
				Code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(OP));
			if (o.Parameters.Count > 0) Code.Remove(Code.Length - 2, 2);
			Code.AppendLine(")");
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\treturn base.Channel.{0}(", UseOperationClientName(o) == true ? o.ClientName : o.Name);
			foreach (MethodParameter op in o.Parameters)
				Code.AppendFormat("{0}, ", op.Name);
			if (o.Parameters.Count > 0) Code.Remove(Code.Length - 2, 2);
			Code.AppendLine(");");
			Code.AppendLine("\t\t}");

			if (o.UseAwaitPattern == true)
			{
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) Code.AppendFormat("\t\tSystem.Threading.Tasks.Task {0}(", UseOperationClientName(o) == true ? o.ClientName : UseOperationClientName(o) == true ? o.ClientName : o.Name);
				else Code.AppendFormat("\t\tSystem.Threading.Tasks.Task<{0}> {1}(", o.ReturnType.HasClientType == true ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) == true ? o.ClientName : UseOperationClientName(o) == true ? o.ClientName : o.Name);
				foreach (MethodParameter OP in o.Parameters)
					Code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(OP));
				if (o.Parameters.Count > 0) Code.Remove(Code.Length - 2, 2);
				Code.AppendLine(");");
				Code.AppendLine("\t\t{");
				Code.AppendFormat("\t\t\treturn base.Channel.{0}Async(", UseOperationClientName(o) == true ? o.ClientName : UseOperationClientName(o) == true ? o.ClientName : o.Name);
				foreach (MethodParameter op in o.Parameters)
					Code.AppendFormat("{0}, ", op.Name);
				if (o.Parameters.Count > 0) Code.Remove(Code.Length - 2, 2);
				Code.AppendLine(");");
				Code.AppendLine("\t\t}");
			}

			if (o.UseAsyncPattern == true)
			{
				Code.AppendFormat("\t\tIAsyncResult {0}(", UseOperationClientName(o) == true ? o.ClientName : UseOperationClientName(o) == true ? o.ClientName : o.Name);
				foreach (MethodParameter OP in o.Parameters)
					Code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(OP));
				Code.AppendLine("AsyncCallback Callback, object AsyncState)");
				Code.AppendLine("\t\t{");
				Code.AppendFormat("\t\t\treturn base.Channel.{0}(", UseOperationClientName(o) == true ? o.ClientName : UseOperationClientName(o) == true ? o.ClientName : o.Name);
				foreach (MethodParameter op in o.Parameters)
					Code.AppendFormat("{0}, ", op.Name);
				Code.AppendLine("Callback, AsyncState);");
				Code.AppendLine("\t\t}");
				Code.AppendFormat("\t\t{0} End{1}(IAsyncResult result){2}", o.ReturnType.HasClientType == true ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), UseOperationClientName(o) == true ? o.ClientName : UseOperationClientName(o) == true ? o.ClientName : o.Name, Environment.NewLine);
				Code.AppendLine("\t\t{");
				Code.AppendFormat("\t\t\treturn base.Channel.{0}(", UseOperationClientName(o) == true ? o.ClientName : UseOperationClientName(o) == true ? o.ClientName : o.Name);
				foreach (MethodParameter op in o.Parameters)
					Code.AppendFormat("{0}, ", op.Name);
				Code.AppendLine("Callback, AsyncState);");
				Code.AppendLine("\t\t}");
			}

			return Code.ToString();
		}

		public static string GenerateMethodParameterServerCode(MethodParameter o)
		{
			if (o.IsHidden == true) return "";
			return string.Format("{0} {1}", DataTypeCSGenerator.GenerateType(o.Type), o.Name);
		}

		public static string GenerateMethodParameterClientCode(MethodParameter o)
		{
			if (o.IsHidden == true) return "";

			if (o.Type.TypeMode == DataTypeMode.Class)
			{
				Data ptype = o.Type as Data;
				return string.Format("{0} {1}", ptype.HasClientType == true ? DataTypeCSGenerator.GenerateType(ptype.ClientType) : DataTypeCSGenerator.GenerateType(o.Type), o.Name);
			}
			else if (o.Type.TypeMode == DataTypeMode.Struct)
			{
				Data ptype = o.Type as Data;
				return string.Format("{0} {1}", ptype.HasClientType == true ? DataTypeCSGenerator.GenerateType(ptype.ClientType) : DataTypeCSGenerator.GenerateType(o.Type), o.Name);
			}
			else if (o.Type.TypeMode == DataTypeMode.Enum)
			{
				Projects.Enum ptype = o.Type as Projects.Enum;
				return string.Format("{0} {1}", ptype.HasClientType == true ? DataTypeCSGenerator.GenerateType(ptype.ClientType) : DataTypeCSGenerator.GenerateType(o.Type), o.Name);
			}
			else
				return string.Format("{0} {1}", DataTypeCSGenerator.GenerateType(o.Type), o.Name);
		}

		public static bool UseOperationClientName(Operation o)
		{
			if (o.ClientName == null || o.ClientName == "") return false;
			return true;
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
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t\t{0} {1} {{ ", DataTypeCSGenerator.GenerateType(o.ReturnType), o.Name);
			if (o.IsReadOnly == false)
			{
				//Write Getter Code
				Code.Append("[OperationContract(");
				if (o.IsOneWay == true)
					Code.Append("IsOneWay = true, ");
				if (o.ProtectionLevel != System.Net.Security.ProtectionLevel.None)
					Code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel));
				Code.AppendFormat("Name = \"Get{0}\"", o.Name);
				Code.Append(")] get; ");

				//Write Setter Code
				Code.Append("[OperationContract(");
				if (o.IsOneWay == true)
					Code.Append("IsOneWay = true, ");
				if (o.ProtectionLevel != System.Net.Security.ProtectionLevel.None)
					Code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel));
				Code.AppendFormat("Name = \"Set{0}\"", o.Name);
				Code.Append(")] set;");
			}
			else
			{
				//Write Getter Code
				Code.Append("[OperationContract(");
				if (o.IsOneWay == true)
					Code.Append("IsOneWay = true, ");
				if (o.ProtectionLevel != System.Net.Security.ProtectionLevel.None)
					Code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel));
				Code.AppendFormat("Name = \"Get{0}\"", o.Name);
				Code.Append(")] get; ");
			}
			Code.AppendLine(" }");

			return Code.ToString();
		}

		public static string GeneratePropertyClientCode(Property o)
		{
			if (o.IsReadOnly == false)
			{
				return string.Format("\t\tpublic {0} {1} {{ get {{ return Get{1}(); }} set {{ Set{1}(value); }} }}", DataTypeCSGenerator.GenerateType(o.ReturnType), o.Name);
			}
			else
			{
				return string.Format("\t\tpublic {0} {1} {{ get {{ return Get{1}(); }} }}", DataTypeCSGenerator.GenerateType(o.ReturnType), o.Name);
			}
		}
	}
}