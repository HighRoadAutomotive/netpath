using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
			if (o.HasClientType) 
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.ClientType.Name) == false)
					Program.AddMessage(new CompileMessage("GS2002", "The service '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));

			var Operations = new ObservableCollection<Operation>();
			Operations.AddRange(o.ServiceOperations);
			Operations.AddRange(o.CallbackOperations);

			foreach (Method m in Operations.Where(a => a.GetType() == typeof(Method)))
			{
				if (string.IsNullOrEmpty(m.ServerName))
					Program.AddMessage(new CompileMessage("GS2004", "An operation in the '" + o.Name + "' service has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));
				else
					if (Helpers.RegExs.MatchCodeName.IsMatch(m.ServerName) == false)
						Program.AddMessage(new CompileMessage("GS2005", "The operation '" + m.ServerName + "' in the '" + o.Name + "' service contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));
				if (!string.IsNullOrEmpty(m.ClientName))
					if (Helpers.RegExs.MatchCodeName.IsMatch(m.ClientName) == false)
						Program.AddMessage(new CompileMessage("GS2006", "The operation '" + m.ServerName + "' in the '" + o.Name + "' service contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));
				if (m.ReturnType == null)
					Program.AddMessage(new CompileMessage("GS2007", "The operation '" + m.ServerName + "' in the '" + o.Name + "' service has a blank Return Type. A Return Type MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));

				foreach (MethodParameter mp in m.Parameters.Where(mp => string.IsNullOrEmpty(mp.Name)))
					Program.AddMessage(new CompileMessage("GS2008", "The method parameter '" + m.ServerName + "' in the '" + o.Name + "' service has a parameter with a blank name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));
			}

			foreach (Property p in Operations.Where(a => a.GetType() == typeof(Property)))
			{
				if (string.IsNullOrEmpty(p.ServerName))
					Program.AddMessage(new CompileMessage("GS2010", "A property in the '" + o.Name + "' service has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o, p, p.GetType(), o.ID, p.ID));
				else
					if (Helpers.RegExs.MatchCodeName.IsMatch(p.ServerName) == false)
						Program.AddMessage(new CompileMessage("GS2011", "The Property '" + p.ServerName + "' in the '" + o.Name + "' service contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o, p, p.GetType(), o.ID, p.ID));
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
			if (o.ServiceDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.ServiceDocumentation));
			foreach (DataType dt in o.KnownTypes)
				code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
			code.AppendFormat("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.Append("\t[ServiceContract(");
			if (o.HasCallback)
				code.AppendFormat("CallbackContract = typeof(I{0}Callback), ", o.Name);
			if (!string.IsNullOrEmpty(o.ConfigurationName))
				code.AppendFormat("ConfigurationName = \"{0}\", ", o.ConfigurationName);
			if (o.ProtectionLevel != System.Net.Security.ProtectionLevel.None)
				code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel));
			code.AppendFormat("SessionMode = System.ServiceModel.SessionMode.{0}, ", System.Enum.GetName(typeof(System.ServiceModel.SessionMode), o.SessionMode));
			if (o.HasClientType)
				code.AppendFormat("Name = \"{0}\", ", o.ClientType.Name);
			code.AppendFormat("Namespace = \"{0}\"", o.Parent.URI);
			code.AppendLine(")]");
			code.AppendFormat("\t{0} interface I{1}{2}", DataTypeCSGenerator.GenerateScope(o.Scope), o.Name, Environment.NewLine);
			code.AppendLine("\t{");
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
				code.Append(GeneratePropertyServerCode45(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.Append(GenerateOperationServerCode45(m));
			code.AppendLine("\t}");

			if (o.HasCallback)
			{
				if (o.CallbackDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.CallbackDocumentation));
				foreach (DataType dt in o.KnownTypes)
					code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
				code.AppendFormat("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
				code.AppendFormat("\t{0} interface I{1}Callback{2}", DataTypeCSGenerator.GenerateScope(o.Scope), o.Name, Environment.NewLine);
				code.AppendLine("\t{");
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.Append(GeneratePropertyServerCode45(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateOperationServerCode45(m));
				code.AppendLine("\t}");
			}

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
			var code = new StringBuilder();

			//Generate the Client interface
			if (o.ServiceDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.ServiceDocumentation));
			if (o.ClientType != null)
				foreach (DataType dt in o.ClientType.KnownTypes)
					code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
			code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t{0} interface {1}{2}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, Environment.NewLine);
			code.AppendLine("\t{");
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
				code.AppendLine(GeneratePropertyInterfaceCode40(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.AppendLine(GenerateMethodInterfaceCode40(m));
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate Callback Interface (if any)
			if (o.HasCallback)
			{
				if (o.CallbackDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.CallbackDocumentation));
				foreach (DataType dt in o.KnownTypes)
					code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
				code.AppendFormat("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
				code.AppendFormat("\t{0} interface {1}Callback{2}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.Name, Environment.NewLine);
				code.AppendLine("\t{");
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyInterfaceCode40(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateMethodInterfaceCode40(m));
				code.AppendLine("\t}");
				code.AppendLine();
			}
			//Generate Channel Interface
			code.AppendFormat("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t{0} interface {1}Channel : {2}, System.ServiceModel.IClientChannel{3}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, o.HasClientType ? DataTypeCSGenerator.GenerateType(o.ClientType) : DataTypeCSGenerator.GenerateType(o), Environment.NewLine);
			code.AppendLine("\t{");
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate the Proxy Class
			code.AppendFormat("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t{0} partial class {1}Client : System.ServiceModel.ClientBase<{1}>, {1}{2}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, Environment.NewLine);
			code.AppendLine("\t{");
			Host h = o.Parent.Owner.Namespace.GetServiceHost(o);
			if (h != null)
				code.Append(HostCSGenerator.GenerateClientCode40(h));
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
				code.AppendLine(GeneratePropertyClientCode(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.AppendLine(GenerateOperationProxyCode40(m));
			code.AppendLine("\t}");

			return code.ToString();
		}

		public static string GenerateClientCode45(Service o)
		{
			var code = new StringBuilder();

			//Generate the Client interface
			if (o.ServiceDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.ServiceDocumentation));
			foreach (DataType dt in o.KnownTypes)
				code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
			code.AppendFormat("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t{0} interface {1}{2}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, Environment.NewLine);
			code.AppendLine("\t{");
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
				code.AppendLine(GeneratePropertyInterfaceCode45(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.AppendLine(GenerateMethodInterfaceCode45(m));
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate Callback Interface (if any)
			if (o.HasCallback)
			{
				if (o.CallbackDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.CallbackDocumentation));
				foreach (DataType dt in o.KnownTypes)
					code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
				code.AppendFormat("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
				code.AppendFormat("\t{0} interface {1}Callback{2}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.Name, Environment.NewLine);
				code.AppendLine("\t{");
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyInterfaceCode45(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateMethodInterfaceCode45(m));
				code.AppendLine("\t}");
				code.AppendLine();
			}
			//Generate Channel Interface
			code.AppendFormat("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t{0} interface {1}Channel : {2}, System.ServiceModel.IClientChannel{3}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, o.HasClientType ? DataTypeCSGenerator.GenerateType(o.ClientType) : DataTypeCSGenerator.GenerateType(o), Environment.NewLine);
			code.AppendLine("\t{");
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate the Proxy Class
			code.AppendFormat("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t{0} partial class {1}Client : System.ServiceModel.ClientBase<{1}>, {1}{2}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, Environment.NewLine);
			code.AppendLine("\t{");
			Host h = o.Parent.Owner.Namespace.GetServiceHost(o);
			if (h != null)
				code.Append(HostCSGenerator.GenerateClientCode45(h));
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
			code.AppendLine(GeneratePropertyClientCode(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
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
			if (o.Documentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
			foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
				code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));

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
			code.AppendFormat(")] {0} {1}(", DataTypeCSGenerator.GenerateType(o.ReturnType), o.ServerName);
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
				code.AppendFormat(")] IAsyncResult Begin{0}(", o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0},", GenerateMethodParameterServerCode(op));
				code.AppendLine(" AsyncCallback Callback, object AsyncState);");
				code.AppendFormat("\t\t{0} End{1}(IAsyncResult result);{2}", DataTypeCSGenerator.GenerateType(o.ReturnType), o.ServerName, Environment.NewLine);
			}

			return code.ToString();
		}

		public static string GenerateMethodInterfaceCode30(Method o)
		{
			return GenerateMethodInterfaceCode35(o);
		}

		public static string GenerateMethodInterfaceCode35(Method o)
		{
			return GenerateMethodInterfaceCode40(o);
		}

		public static string GenerateMethodInterfaceCode40(Method o)
		{
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
			foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
				code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));

			code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/{1}/{2}\")]" : "\t\t[OperationContract(Action = \"{0}/{1}/{2}\", ReplyAction = \"{0}/{1}/{2}Response\")]", o.Owner.Parent.URI, o.Owner.Name, o.ServerName));
			code.AppendFormat("\t\t{0} {1}(", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");

			if (o.UseAsyncPattern)
			{
				code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/{1}/{2}\")]" : "\t\t[OperationContract(Action = \"{0}/{1}/{2}\", ReplyAction = \"{0}/{1}/{2}Response\")]", o.Owner.Parent.URI, o.Owner.Name, o.ServerName));
				code.AppendFormat("\t\tIAsyncResult {0}(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				code.AppendLine("AsyncCallback Callback, object AsyncState);");
				code.AppendFormat("\t\t{0} End{1}(IAsyncResult result);{2}", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName, Environment.NewLine);
			}

			return code.ToString();
		}

		public static string GenerateMethodInterfaceCode45(Method o)
		{
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
			foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
				code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));

			code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/{1}/{2}\")]" : "\t\t[OperationContract(Action = \"{0}/{1}/{2}\", ReplyAction = \"{0}/{1}/{2}Response\")]", o.Owner.Parent.URI, o.Owner.Name, o.ServerName));
			code.AppendFormat("\t\t{0} {1}(", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");

			if (o.UseAwaitPattern)
			{
				code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/{1}/{2}\")]" : "\t\t[OperationContract(Action = \"{0}/{1}/{2}\", ReplyAction = \"{0}/{1}/{2}Response\")]", o.Owner.Parent.URI, o.Owner.Name, o.ServerName));
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tSystem.Threading.Tasks.Task {0}(", o.HasClientType ? o.ClientName : o.ServerName);
				else code.AppendFormat("\t\tSystem.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(");");
			}

			if (o.UseAsyncPattern)
			{
				code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/{1}/{2}\")]" : "\t\t[OperationContract(Action = \"{0}/{1}/{2}\", ReplyAction = \"{0}/{1}/{2}Response\")]", o.Owner.Parent.URI, o.Owner.Name, o.ServerName));
				code.AppendFormat("\t\tIAsyncResult {0}(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				code.AppendLine("AsyncCallback Callback, object AsyncState);");
				code.AppendFormat("\t\t{0} End{1}(IAsyncResult result);{2}", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName, Environment.NewLine);
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
			if (o.Documentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
			foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
				code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));

			code.AppendFormat("\t\tpublic {0} {1}(", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			code.AppendFormat("\t\t\t{0}base.Channel.{1}(", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", op.Name);
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");
			code.AppendLine("\t\t}");

			if (o.UseAsyncPattern)
			{
				code.AppendFormat("\t\tpublic IAsyncResult {0}(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				code.AppendLine("AsyncCallback Callback, object AsyncState)");
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\t{0}base.Channel.{1}(", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", op.Name);
				code.AppendLine("Callback, AsyncState);");
				code.AppendLine("\t\t}");
				code.AppendFormat("\t\tpublic {0} End{1}(IAsyncResult result){2}", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName, Environment.NewLine);
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\t{0}base.Channel.{1}(", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName);
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
			if (o.Documentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
			foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
				code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));

			code.AppendFormat("\t\tpublic {0} {1}(", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			code.AppendFormat("\t\t\t{0}base.Channel.{1}(", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", op.Name);
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");
			code.AppendLine("\t\t}");

			if (o.UseAwaitPattern)
			{
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tSystem.Threading.Tasks.Task {0}(", o.HasClientType ? o.ClientName : o.ServerName);
				else code.AppendFormat("\t\tpublic System.Threading.Tasks.Task<{0}> {1}(", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(");");
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\t{0}base.Channel.{1}(", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", op.Name);
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(");");
				code.AppendLine("\t\t}");
			}

			if (o.UseAsyncPattern)
			{
				code.AppendFormat("\t\tpublic IAsyncResult {0}(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				code.AppendLine("AsyncCallback Callback, object AsyncState)");
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\t{0}base.Channel.{1}(", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", op.Name);
				code.AppendLine("Callback, AsyncState);");
				code.AppendLine("\t\t}");
				code.AppendFormat("\t\tpublic {0} End{1}(IAsyncResult result){2}", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName, Environment.NewLine);
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\t{0}base.Channel.{1}(", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName);
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
			if (o.Documentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
			code.AppendFormat("\t\t{0} {1} {{ ", DataTypeCSGenerator.GenerateType(o.ReturnType), o.ServerName);
			if (o.IsReadOnly == false)
			{
				//Write Getter Code
				code.Append("[OperationContract(");
				if (o.IsOneWay)
					code.Append("IsOneWay = true, ");
				if (o.ProtectionLevel != System.Net.Security.ProtectionLevel.None)
					code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel));
				code.AppendFormat("Name = \"Get{0}\"", o.ServerName);
				code.Append(")] get; ");

				//Write Setter Code
				code.Append("[OperationContract(");
				if (o.IsOneWay)
					code.Append("IsOneWay = true, ");
				if (o.ProtectionLevel != System.Net.Security.ProtectionLevel.None)
					code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel));
				code.AppendFormat("Name = \"Set{0}\"", o.ServerName);
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
				code.AppendFormat("Name = \"Get{0}\"", o.ServerName);
				code.Append(")] get; ");
			}
			code.AppendLine(" }");

			return code.ToString();
		}

		public static string GeneratePropertyInterfaceCode40(Property o)
		{
			return GeneratePropertyInterfaceCode45(o);
		}

		public static string GeneratePropertyInterfaceCode45(Property o)
		{
			var code = new StringBuilder();

			code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/{1}/{2}\")]" : "\t\t[OperationContract(Action = \"{0}/{1}/{2}\", ReplyAction = \"{0}/{1}/{2}Response\")]", o.Owner.Parent.URI, o.Owner.Name, o.ServerName));
			code.AppendLine(string.Format("\t\t{0} Get{1}();", DataTypeCSGenerator.GenerateType(o.ReturnType), o.ServerName));
			if (!o.IsReadOnly)
			{
				code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/{1}/{2}\")]" : "\t\t[OperationContract(Action = \"{0}/{1}/{2}\", ReplyAction = \"{0}/{1}/{2}Response\")]", o.Owner.Parent.URI, o.Owner.Name, o.ServerName));
				code.AppendLine(string.Format("\t\tvoid Set{1}({0} value);", DataTypeCSGenerator.GenerateType(o.ReturnType), o.ServerName));
			}
			return code.ToString();
		}

		public static string GeneratePropertyClientCode(Property o)
		{
			var code = new StringBuilder();

			code.AppendLine(string.Format("\t\t{0} {1}.Get{2}()", DataTypeCSGenerator.GenerateType(o.ReturnType), o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, o.ServerName));
			code.AppendLine("\t\t{");
			code.AppendLine(string.Format("\t\t\treturn base.Channel.Get{0}();", o.HasClientType ? o.ClientName : o.ServerName));
			code.AppendLine("\t\t}");

			if(!o.IsReadOnly)
			{
				code.AppendLine(string.Format("\t\tvoid {1}.Set{2}({0} value)", DataTypeCSGenerator.GenerateType(o.ReturnType), o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, o.ServerName));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\tbase.Channel.Set{0}(value);", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t}");
			}

			code.AppendLine(string.Format(o.IsReadOnly == false ? "\t\tpublic {0} {1} {{ get {{ return (({2})this).Get{1}(); }} set {{ (({2})this).Set{1}(value); }} }}" : "\t\tpublic {0} {1} {{ get {{ return (({2})this).Get{1}(); }} }}", DataTypeCSGenerator.GenerateType(o.ReturnType), o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name));

			return code.ToString();
		}
	}
}