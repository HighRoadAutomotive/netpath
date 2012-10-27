using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFArchitect.Projects;
using WCFArchitect.Projects.Helpers;

namespace WCFArchitect.Compiler.Generators
{
	internal static class ServiceCSGenerator
	{

		public static void VerifyCode(Service o)
		{
			if (string.IsNullOrEmpty(o.Name))
				Program.AddMessage(new CompileMessage("GS2000", "An service in the '" + o.Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
			else
				if (RegExs.MatchCodeName.IsMatch(o.Name) == false)
					Program.AddMessage(new CompileMessage("GS2001", "The service '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
			if (o.HasClientType) 
				if (RegExs.MatchCodeName.IsMatch(o.ClientType.Name) == false)
					Program.AddMessage(new CompileMessage("GS2002", "The service '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));

			var Operations = new ObservableCollection<Operation>();
			Operations.AddRange(o.ServiceOperations);
			Operations.AddRange(o.CallbackOperations);

			foreach (Method m in Operations.Where(a => a.GetType() == typeof(Method)))
			{
				if (string.IsNullOrEmpty(m.ServerName))
					Program.AddMessage(new CompileMessage("GS2004", "An method in the '" + o.Name + "' service has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));
				else
				{
					if (RegExs.MatchCodeName.IsMatch(m.ServerName) == false)
						Program.AddMessage(new CompileMessage("GS2005", "The method '" + m.ServerName + "' in the '" + o.Name + "' service contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));
					if (m.ServerName == "__callback")
						Program.AddMessage(new CompileMessage("GS2015", "The name of the method '" + m.ServerName + "' in the '" + o.Name + "' service is invalid. Please rename it.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));
				}

				if (!string.IsNullOrEmpty(m.ClientName))
					if (RegExs.MatchCodeName.IsMatch(m.ClientName) == false)
						Program.AddMessage(new CompileMessage("GS2006", "The method '" + m.ServerName + "' in the '" + o.Name + "' service contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));
				if (m.ReturnType == null)
					Program.AddMessage(new CompileMessage("GS2007", "The method '" + m.ServerName + "' in the '" + o.Name + "' service has a blank Return Type. A Return Type MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));
				else
					if (m.IsRESTMethod && m.ReturnType.TypeMode == DataTypeMode.Primitive && (m.ReturnType.Primitive == PrimitiveTypes.Void || m.ReturnType.Primitive == PrimitiveTypes.None))
						Program.AddMessage(new CompileMessage("GS2012", "The method return type '" + m.ReturnType + "' in the '" + o.Name + "' service is not a valid REST return type. Please specify a valid REST return type.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));
				if (m.ReturnType.TypeMode == DataTypeMode.Namespace || m.ReturnType.TypeMode == DataTypeMode.Interface)
					Program.AddMessage(new CompileMessage("GS2013", "The method return type '" + m.ReturnType + "' in the '" + o.Name + "' service is not a valid return type. Please specify a valid return type.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));

				foreach (MethodParameter mp in m.Parameters)
				{
					if(string.IsNullOrEmpty(mp.Name))
						Program.AddMessage(new CompileMessage("GS2008", "The method parameter '" + m.ServerName + "' in the '" + o.Name + "' service has a parameter with a blank name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));
					if (m.IsRESTMethod && mp.IsRESTInvalid)
						Program.AddMessage(new CompileMessage("GS2009", "The method REST parameter '" + (string.IsNullOrEmpty(m.REST.RESTName) ? m.ServerName : m.REST.RESTName) + "' in the '" + m.ServerName + "' method is not a valid REST parameter. Please specify a valid REST parameter.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));
					if (mp.Name == "__callback")
						Program.AddMessage(new CompileMessage("GS2016", "The name of the method parameter '" + mp.Name + "' in the '" + m.ServerName + "' method is invalid. Please rename it.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.ID, m.ID));
				}
			}

			foreach (Property p in Operations.Where(a => a.GetType() == typeof(Property)))
			{
				if (string.IsNullOrEmpty(p.ServerName))
					Program.AddMessage(new CompileMessage("GS2010", "A property in the '" + o.Name + "' service has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o, p, p.GetType(), o.ID, p.ID));
				else
				{
					if (RegExs.MatchCodeName.IsMatch(p.ServerName) == false)
						Program.AddMessage(new CompileMessage("GS2011", "The property '" + p.ServerName + "' in the '" + o.Name + "' service contains invalid characters in the Server Name.", CompileMessageSeverity.ERROR, o, p, p.GetType(), o.ID, p.ID));
					if (p.ServerName == "__callback")
						Program.AddMessage(new CompileMessage("GS2015", "The name of the property '" + p.ServerName + "' in the '" + o.Name + "' service is invalid. Please rename it.", CompileMessageSeverity.ERROR, o, p, p.GetType(), o.ID, p.ID));
				}
				if (p.ReturnType.TypeMode == DataTypeMode.Namespace || p.ReturnType.TypeMode == DataTypeMode.Interface)
					Program.AddMessage(new CompileMessage("GS2014", "The property value type '" + p.ReturnType + "' in the '" + o.Name + "' service is not a valid value type. Please specify a valid value type.", CompileMessageSeverity.ERROR, o, p, p.GetType(), o.ID, p.ID));
			}
		}

		public static bool CanGenerateAsync(Service o, bool IsServer)
		{
			return IsServer ? (o.AsynchronyMode == ServiceAsynchronyMode.Server || o.AsynchronyMode == ServiceAsynchronyMode.Both) : (o.AsynchronyMode == ServiceAsynchronyMode.Client || o.AsynchronyMode == ServiceAsynchronyMode.Both || o.AsynchronyMode == ServiceAsynchronyMode.Default);
		}

		#region - Server Interfaces -

		public static string GenerateServerCode30(Service o)
		{
			var code = new StringBuilder();
			if (o.ServiceDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.ServiceDocumentation));
			foreach (DataType dt in o.KnownTypes)
				code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t[ServiceContract({0}SessionMode = System.ServiceModel.SessionMode.{1}, {2}{3}{4}Namespace = \"{5}\")]", o.HasCallback ? string.Format("CallbackContract = typeof(I{0}Callback), ", o.Name) : "", System.Enum.GetName(typeof(System.ServiceModel.SessionMode), o.SessionMode), o.ProtectionLevel != System.Net.Security.ProtectionLevel.None ? string.Format("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel)) : "", !string.IsNullOrEmpty(o.ConfigurationName) ? string.Format("ConfigurationName = \"{0}\", ", o.ConfigurationName) : "", o.HasClientType ? string.Format("Name = \"{0}\", ", o.ClientType.Name) : "", o.Parent.FullURI));
			code.AppendLine(string.Format("\t{0} interface I{1}", DataTypeCSGenerator.GenerateScope(o.Scope), o.Name));
			code.AppendLine("\t{");
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
				code.AppendLine(GeneratePropertyServerCode40(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.AppendLine(GenerateServiceInterfaceMethodCode30(m, false));
			code.AppendLine("\t}");

			if (o.HasCallback)
			{
				//Generate the callback interface
				if (o.CallbackDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.CallbackDocumentation));
				foreach (DataType dt in o.KnownTypes)
					code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendLine(string.Format("\t{0} interface I{1}Callback", DataTypeCSGenerator.GenerateScope(o.Scope), o.Name));
				code.AppendLine("\t{");
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyServerCode30(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateServiceInterfaceMethodCode30(m, true));
				code.AppendLine("\t}");

				//Generate the callback facade implementation
				if (o.HasAsyncServiceOperations && CanGenerateAsync(o, false))
				{
					code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
					code.AppendLine("\tpublic class AsyncOperationCompletedArgs<T>");
					code.AppendLine("\t{");
					code.AppendLine("\t\tprivate T result;");
					code.AppendLine("\t\tpublic T Result { get { return result; } private set { result = value; } }");
					code.AppendLine("\t\tprivate System.Exception error;");
					code.AppendLine("\t\tpublic System.Exception Error { get { return error; } private set { error = value; } }");
					code.AppendLine("\t\tprivate bool cancelled;");
					code.AppendLine("\t\tpublic bool Cancelled { get { return cancelled; } private set { cancelled = value; } }");
					code.AppendLine("\t\tprivate object userState;");
					code.AppendLine("\t\tpublic object UserState { get { return userState; } private set { userState = value; } }");
					code.AppendLine("\t\tpublic AsyncOperationCompletedArgs(T result, System.Exception error, bool cancelled, Object userState)");
					code.AppendLine("\t\t{");
					code.AppendLine("\t\t\tResult = result;");
					code.AppendLine("\t\t\tError = error;");
					code.AppendLine("\t\t\tCancelled = cancelled;");
					code.AppendLine("\t\t\tUserState = userState;");
					code.AppendLine("\t\t}");
					code.AppendLine("\t}");
				}
				if (o.CallbackDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.CallbackDocumentation));
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendLine(string.Format("\t{0} partial class {1}Callback : I{1}Callback", DataTypeCSGenerator.GenerateScope(o.Scope), o.Name));
				code.AppendLine("\t{");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tprivate readonly I{0}Callback __callback;", o.Name));
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Callback()", o.Name));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\t__callback = System.ServiceModel.OperationContext.Current.GetCallbackChannel<I{0}Callback>();", o.Name));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Callback(I{0}Callback callback)", o.Name));
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\t__callback = callback;");
				code.AppendLine("\t\t}");
				code.AppendLine();
				if (o.HasAsyncCallbackOperations && CanGenerateAsync(o, true))
				{
					code.AppendLine("\t\tprotected class InvokeAsyncCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs");
					code.AppendLine("\t\t{");
					code.AppendLine("\t\t\tpublic object[] Results { get; set; }");
					code.AppendLine("\t\t\tpublic InvokeAsyncCompletedEventArgs(object[] results, System.Exception error, bool cancelled, Object userState) : base(error, cancelled, userState)");
					code.AppendLine("\t\t\t{");
					code.AppendLine("\t\t\t\tResults = results;");
					code.AppendLine("\t\t\t}");
					code.AppendLine("\t\t}");
					code.AppendLine();
					code.AppendLine("\t\tprotected delegate IAsyncResult BeginOperationDelegate(object[] inValues, AsyncCallback asyncCallback, Object state);");
					code.AppendLine("\t\tprotected delegate object[] EndOperationDelegate(IAsyncResult result);");
					code.AppendLine();
					code.AppendLine("\t\tprotected void InvokeAsync(BeginOperationDelegate beginOperationDelegate, object[] inValues, EndOperationDelegate endOperationDelegate, System.Threading.SendOrPostCallback operationCompletedCallback, object userState)");
					code.AppendLine("\t\t{");
					code.AppendLine("\t\t\tif (beginOperationDelegate == null) throw new ArgumentNullException(\"Argument 'beginOperationDelegate' cannot be null.\");");
					code.AppendLine("\t\t\tif (endOperationDelegate == null) throw new ArgumentNullException(\"Argument 'endOperationDelegate' cannot be null.\");");
					code.AppendLine("\t\t\tAsyncCallback cb = delegate(IAsyncResult ar)");
					code.AppendLine("\t\t\t{");
					code.AppendLine("\t\t\t\tobject[] results = null;");
					code.AppendLine("\t\t\t\tException error = null;");
					code.AppendLine("\t\t\t\ttry { results = endOperationDelegate(ar); }");
					code.AppendLine("\t\t\t\tcatch (Exception ex) { error = ex; }");
					code.AppendLine("\t\t\t\tif (operationCompletedCallback != null) operationCompletedCallback(new InvokeAsyncCompletedEventArgs(results, error, false, userState));");
					code.AppendLine("\t\t\t};");
					code.AppendLine("\t\t\tbeginOperationDelegate(inValues, cb, userState);");
					code.AppendLine("\t\t}");
					code.AppendLine();
				}
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyClientCode(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateMethodProxyCode30(m, true, true));
				code.AppendLine("\t}");
			}

			return code.ToString();
		}

		public static string GenerateServerCode35(Service o)
		{
			return GenerateServerCode40(o);
		}

		public static string GenerateServerCode40(Service o)
		{
			var code = new StringBuilder();
			if (o.ServiceDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.ServiceDocumentation));
			foreach (DataType dt in o.KnownTypes)
				code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t[ServiceContract({0}SessionMode = System.ServiceModel.SessionMode.{1}, {2}{3}{4}Namespace = \"{5}\")]", o.HasCallback ? string.Format("CallbackContract = typeof(I{0}Callback), ", o.Name) : "", System.Enum.GetName(typeof(System.ServiceModel.SessionMode), o.SessionMode), o.ProtectionLevel != System.Net.Security.ProtectionLevel.None ? string.Format("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel)) : "", !string.IsNullOrEmpty(o.ConfigurationName) ? string.Format("ConfigurationName = \"{0}\", ", o.ConfigurationName) : "", o.HasClientType ? string.Format("Name = \"{0}\", ", o.ClientType.Name) : "", o.Parent.FullURI));
			code.AppendLine(string.Format("\t{0} interface I{1}", DataTypeCSGenerator.GenerateScope(o.Scope), o.Name));
			code.AppendLine("\t{");
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
				code.AppendLine(GeneratePropertyServerCode40(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.AppendLine(GenerateServiceInterfaceMethodCode40(m, false));
			code.AppendLine("\t}");

			if (o.HasCallback)
			{
				//Generate the callback interface
				if (o.CallbackDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.CallbackDocumentation));
				foreach (DataType dt in o.KnownTypes)
					code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendLine(string.Format("\t{0} interface I{1}Callback", DataTypeCSGenerator.GenerateScope(o.Scope), o.Name));
				code.AppendLine("\t{");
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyServerCode40(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateServiceInterfaceMethodCode40(m, true));
				code.AppendLine("\t}");

				//Generate the callback facade implementation
				if (o.CallbackDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.CallbackDocumentation));
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendLine(string.Format("\t{0} partial class {1}Callback : I{1}Callback", DataTypeCSGenerator.GenerateScope(o.Scope), o.Name));
				code.AppendLine("\t{");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tprivate readonly I{0}Callback __callback;", o.Name));
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Callback()", o.Name));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\t__callback = System.ServiceModel.OperationContext.Current.GetCallbackChannel<I{0}Callback>();", o.Name));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, true));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Callback(I{0}Callback callback)", o.Name));
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\t__callback = callback;");
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, true));
				code.AppendLine("\t\t}");
				code.AppendLine();
				if (o.HasAsyncCallbackOperations && CanGenerateAsync(o, true))
				{
					code.AppendLine("\t\tprotected class InvokeAsyncCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs");
					code.AppendLine("\t\t{");
					code.AppendLine("\t\t\tpublic object[] Results { get; set; }");
					code.AppendLine("\t\t\tpublic InvokeAsyncCompletedEventArgs(object[] results, System.Exception error, bool cancelled, Object userState) : base(error, cancelled, userState)");
					code.AppendLine("\t\t\t{");
					code.AppendLine("\t\t\t\tResults = results;");
					code.AppendLine("\t\t\t}");
					code.AppendLine("\t\t}");
					code.AppendLine();
					code.AppendLine("\t\tprotected delegate IAsyncResult BeginOperationDelegate(object[] inValues, AsyncCallback asyncCallback, Object state);");
					code.AppendLine("\t\tprotected delegate object[] EndOperationDelegate(IAsyncResult result);");
					code.AppendLine();
					code.AppendLine("\t\tprotected void InvokeAsync(BeginOperationDelegate beginOperationDelegate, object[] inValues, EndOperationDelegate endOperationDelegate, System.Threading.SendOrPostCallback operationCompletedCallback, object userState)");
					code.AppendLine("\t\t{");
					code.AppendLine("\t\t\tif (beginOperationDelegate == null) throw new ArgumentNullException(\"Argument 'beginOperationDelegate' cannot be null.\");");
					code.AppendLine("\t\t\tif (endOperationDelegate == null) throw new ArgumentNullException(\"Argument 'endOperationDelegate' cannot be null.\");");
					code.AppendLine("\t\t\tAsyncCallback cb = delegate(IAsyncResult ar)");
					code.AppendLine("\t\t\t{");
					code.AppendLine("\t\t\t\tobject[] results = null;");
					code.AppendLine("\t\t\t\tException error = null;");
					code.AppendLine("\t\t\t\ttry { results = endOperationDelegate(ar); }");
					code.AppendLine("\t\t\t\tcatch (Exception ex) { error = ex; }");
					code.AppendLine("\t\t\t\tif (operationCompletedCallback != null) operationCompletedCallback(new InvokeAsyncCompletedEventArgs(results, error, false, userState));");
					code.AppendLine("\t\t\t};");
					code.AppendLine("\t\t\tbeginOperationDelegate(inValues, cb, userState);");
					code.AppendLine("\t\t}");
					code.AppendLine();
				}
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyClientCode(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateMethodProxyCode40(m, true, true));
				code.AppendLine("\t}");
			}

			return code.ToString();
		}

		public static string GenerateServerCode45(Service o)
		{
			var code = new StringBuilder();
			if (o.ServiceDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.ServiceDocumentation));
			foreach (DataType dt in o.KnownTypes)
				code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t[ServiceContract({0}SessionMode = System.ServiceModel.SessionMode.{1}, {2}{3}{4}Namespace = \"{5}\")]", o.HasCallback ? string.Format("CallbackContract = typeof(I{0}Callback), ", o.Name) : "", System.Enum.GetName(typeof(System.ServiceModel.SessionMode), o.SessionMode), o.ProtectionLevel != System.Net.Security.ProtectionLevel.None ? string.Format("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel)) : "", !string.IsNullOrEmpty(o.ConfigurationName) ? string.Format("ConfigurationName = \"{0}\", ", o.ConfigurationName) : "", o.HasClientType ? string.Format("Name = \"{0}\", ", o.ClientType.Name) : "", o.Parent.FullURI));
			code.AppendLine(string.Format("\t{0} interface I{1}", DataTypeCSGenerator.GenerateScope(o.Scope), o.Name));
			code.AppendLine("\t{");
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
				code.AppendLine(GeneratePropertyServerCode45(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.AppendLine(GenerateServiceInterfaceMethodCode45(m, false));
			code.AppendLine("\t}");

			if (o.HasCallback)
			{
				//Generate the callback interface
				if (o.CallbackDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.CallbackDocumentation));
				foreach (DataType dt in o.KnownTypes)
					code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendLine(string.Format("\t{0} interface I{1}Callback", DataTypeCSGenerator.GenerateScope(o.Scope), o.Name));
				code.AppendLine("\t{");
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyInterfaceCode45(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateServiceInterfaceMethodCode45(m, true));
				code.AppendLine("\t}");

				//Generate the callback facade implementation
				if (o.CallbackDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.CallbackDocumentation));
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendLine(string.Format("\t{0} partial class {1}Callback : I{1}Callback", DataTypeCSGenerator.GenerateScope(o.Scope), o.Name));
				code.AppendLine("\t{");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tprivate readonly I{0}Callback __callback;", o.Name));
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Callback()", o.Name));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\t__callback = System.ServiceModel.OperationContext.Current.GetCallbackChannel<I{0}Callback>();", o.Name));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, true));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Callback(I{0}Callback callback)", o.Name));
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\t__callback = callback;");
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, true));
				code.AppendLine("\t\t}");
				code.AppendLine();
				if (o.HasAsyncCallbackOperations && CanGenerateAsync(o, true))
				{
					code.AppendLine("\t\tprotected class InvokeAsyncCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs");
					code.AppendLine("\t\t{");
					code.AppendLine("\t\t\tpublic object[] Results { get; set; }");
					code.AppendLine("\t\t\tpublic InvokeAsyncCompletedEventArgs(object[] results, System.Exception error, bool cancelled, Object userState) : base(error, cancelled, userState)");
					code.AppendLine("\t\t\t{");
					code.AppendLine("\t\t\t\tResults = results;");
					code.AppendLine("\t\t\t}");
					code.AppendLine("\t\t}");
					code.AppendLine();
					code.AppendLine("\t\tprotected delegate IAsyncResult BeginOperationDelegate(object[] inValues, AsyncCallback asyncCallback, Object state);");
					code.AppendLine("\t\tprotected delegate object[] EndOperationDelegate(IAsyncResult result);");
					code.AppendLine();
					code.AppendLine("\t\tprotected void InvokeAsync(BeginOperationDelegate beginOperationDelegate, object[] inValues, EndOperationDelegate endOperationDelegate, System.Threading.SendOrPostCallback operationCompletedCallback, object userState)");
					code.AppendLine("\t\t{");
					code.AppendLine("\t\t\tif (beginOperationDelegate == null) throw new ArgumentNullException(\"Argument 'beginOperationDelegate' cannot be null.\");");
					code.AppendLine("\t\t\tif (endOperationDelegate == null) throw new ArgumentNullException(\"Argument 'endOperationDelegate' cannot be null.\");");
					code.AppendLine("\t\t\tAsyncCallback cb = delegate(IAsyncResult ar)");
					code.AppendLine("\t\t\t{");
					code.AppendLine("\t\t\t\tobject[] results = null;");
					code.AppendLine("\t\t\t\tException error = null;");
					code.AppendLine("\t\t\t\ttry { results = endOperationDelegate(ar); }");
					code.AppendLine("\t\t\t\tcatch (Exception ex) { error = ex; }");
					code.AppendLine("\t\t\t\tif (operationCompletedCallback != null) operationCompletedCallback(new InvokeAsyncCompletedEventArgs(results, error, false, userState));");
					code.AppendLine("\t\t\t};");
					code.AppendLine("\t\t\tbeginOperationDelegate(inValues, cb, userState);");
					code.AppendLine("\t\t}");
					code.AppendLine();
				}
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyClientCode(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateMethodProxyCode45(m, true, true));
				code.AppendLine("\t}");
			}

			return code.ToString();
		}

		#endregion

		#region - Client Interfaces -

		public static string GenerateClientCode30(Service o)
		{
			var code = new StringBuilder();

			//Generate the Client interface
			if (o.ServiceDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.ServiceDocumentation));
			if (o.ClientType != null)
				foreach (DataType dt in o.ClientType.KnownTypes)
					code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} interface I{1}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t{");
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
				code.AppendLine(GeneratePropertyInterfaceCode40(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.AppendLine(GenerateClientInterfaceMethodCode30(m));
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate Callback Interface (if any)
			if (o.HasCallback)
			{
				if (o.CallbackDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.CallbackDocumentation));
				foreach (DataType dt in o.KnownTypes)
					code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendFormat("\t{0} interface I{1}Callback{2}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.Name, Environment.NewLine);
				code.AppendLine("\t{");
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyInterfaceCode40(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateClientInterfaceMethodCode30(m));
				code.AppendLine("\t}");
				code.AppendLine();
			}
			//Generate Channel Interface
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} interface I{1}Channel : I{2}, System.ServiceModel.IClientChannel", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t{");
			code.AppendLine("\t}");
			code.AppendLine();
			if (o.HasAsyncServiceOperations && CanGenerateAsync(o, false))
			{
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendLine("\tpublic class AsyncOperationCompletedArgs<T>");
				code.AppendLine("\t{");
				code.AppendLine("\t\tprivate T result;");
				code.AppendLine("\t\tpublic T Result { get { return result; } private set { result = value; } }");
				code.AppendLine("\t\tprivate System.Exception error;");
				code.AppendLine("\t\tpublic System.Exception Error { get { return error; } private set { error = value; } }");
				code.AppendLine("\t\tprivate bool cancelled;");
				code.AppendLine("\t\tpublic bool Cancelled { get { return cancelled; } private set { cancelled = value; } }");
				code.AppendLine("\t\tprivate object userState;");
				code.AppendLine("\t\tpublic object UserState { get { return userState; } private set { userState = value; } }");
				code.AppendLine("\t\tpublic AsyncOperationCompletedArgs(T result, System.Exception error, bool cancelled, Object userState)");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tResult = result;");
				code.AppendLine("\t\t\tError = error;");
				code.AppendLine("\t\t\tCancelled = cancelled;");
				code.AppendLine("\t\t\tUserState = userState;");
				code.AppendLine("\t\t}");
				code.AppendLine("\t}");
			}
			//Generate the Proxy Class
			code.AppendLine("\t[System.Diagnostics.DebuggerStepThroughAttribute()]");
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} partial class {1}Proxy : System.ServiceModel.ClientBase<I{1}>, I{1}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t{");
			code.AppendLine(string.Format("\t\tpublic {0}Proxy(string endpointConfigurationName) : base(endpointConfigurationName)", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}Proxy(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}Proxy(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}Proxy(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t}");
			code.AppendLine();
			if (o.HasAsyncServiceOperations && CanGenerateAsync(o, false))
			{
				code.AppendLine("\t\tprotected class InvokeAsyncCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tpublic object[] Results { get; set; }");
				code.AppendLine("\t\t\tpublic InvokeAsyncCompletedEventArgs(object[] results, System.Exception error, bool cancelled, Object userState) : base(error, cancelled, userState)");
				code.AppendLine("\t\t\t{");
				code.AppendLine("\t\t\t\tResults = results;");
				code.AppendLine("\t\t\t}");
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine("\t\tprotected delegate IAsyncResult BeginOperationDelegate(object[] inValues, AsyncCallback asyncCallback, Object state);");
				code.AppendLine("\t\tprotected delegate object[] EndOperationDelegate(IAsyncResult result);");
				code.AppendLine();
				code.AppendLine("\t\tprotected void InvokeAsync(BeginOperationDelegate beginOperationDelegate, object[] inValues, EndOperationDelegate endOperationDelegate, System.Threading.SendOrPostCallback operationCompletedCallback, object userState)");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tif (beginOperationDelegate == null) throw new ArgumentNullException(\"Argument 'beginOperationDelegate' cannot be null.\");");
				code.AppendLine("\t\t\tif (endOperationDelegate == null) throw new ArgumentNullException(\"Argument 'endOperationDelegate' cannot be null.\");");
				code.AppendLine("\t\t\tAsyncCallback cb = delegate(IAsyncResult ar)");
				code.AppendLine("\t\t\t{");
				code.AppendLine("\t\t\t\tobject[] results = null;");
				code.AppendLine("\t\t\t\tException error = null;");
				code.AppendLine("\t\t\t\ttry { results = endOperationDelegate(ar); }");
				code.AppendLine("\t\t\t\tcatch (Exception ex) { error = ex; }");
				code.AppendLine("\t\t\t\tif (operationCompletedCallback != null) operationCompletedCallback(new InvokeAsyncCompletedEventArgs(results, error, false, userState));");
				code.AppendLine("\t\t\t};");
				code.AppendLine("\t\t\tbeginOperationDelegate(inValues, cb, userState);");
				code.AppendLine("\t\t}");
				code.AppendLine();
			}
			Host h = o.Parent.Owner.Namespace.GetServiceHost(o);
			if (h != null)
				code.Append(HostCSGenerator.GenerateClientCode40(h));
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
				code.AppendLine(GeneratePropertyClientCode(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.AppendLine(GenerateMethodProxyCode30(m, false, false));
			code.AppendLine("\t}");

			return code.ToString();
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
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} interface I{1}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t{");
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
				code.AppendLine(GeneratePropertyInterfaceCode40(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.AppendLine(GenerateClientInterfaceMethodCode40(m));
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate Callback Interface (if any)
			if (o.HasCallback)
			{
				if (o.CallbackDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.CallbackDocumentation));
				foreach (DataType dt in o.KnownTypes)
					code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendFormat("\t{0} interface I{1}Callback{2}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.Name, Environment.NewLine);
				code.AppendLine("\t{");
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyInterfaceCode40(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateClientInterfaceMethodCode40(m));
				code.AppendLine("\t}");
				code.AppendLine();
			}
			//Generate Channel Interface
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} interface I{1}Channel : I{2}, System.ServiceModel.IClientChannel", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t{");
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate the Proxy Class
			code.AppendLine("\t[System.Diagnostics.DebuggerStepThroughAttribute()]");
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} partial class {1}Proxy : System.ServiceModel.ClientBase<I{1}>, I{1}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t{");
			code.AppendLine(string.Format("\t\tpublic {0}Proxy(string endpointConfigurationName) : base(endpointConfigurationName)", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.Append(GenerateMethodProxyConstructorCode(m, false));
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}Proxy(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.Append(GenerateMethodProxyConstructorCode(m, false));
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}Proxy(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.Append(GenerateMethodProxyConstructorCode(m, false));
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}Proxy(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.Append(GenerateMethodProxyConstructorCode(m, false));
			code.AppendLine("\t\t}");
			code.AppendLine();
			Host h = o.Parent.Owner.Namespace.GetServiceHost(o);
			if (h != null)
				code.Append(HostCSGenerator.GenerateClientCode40(h));
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
				code.AppendLine(GeneratePropertyClientCode(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.AppendLine(GenerateMethodProxyCode40(m, false, false));
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
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} interface I{1}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t{");
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
				code.AppendLine(GeneratePropertyInterfaceCode45(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.AppendLine(GenerateClientInterfaceMethodCode45(m));
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate Callback Interface (if any)
			if (o.HasCallback)
			{
				if (o.CallbackDocumentation != null) code.Append(DocumentationCSGenerator.GenerateDocumentation(o.CallbackDocumentation));
				foreach (DataType dt in o.KnownTypes)
					code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendFormat("\t{0} interface I{1}Callback{2}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.Name, Environment.NewLine);
				code.AppendLine("\t{");
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyInterfaceCode45(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateClientInterfaceMethodCode45(m));
				code.AppendLine("\t}");
				code.AppendLine();
			}
			//Generate Channel Interface
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} interface I{1}Channel : I{2}, System.ServiceModel.IClientChannel", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t{");
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate the Proxy Class
			code.AppendLine("\t[System.Diagnostics.DebuggerStepThroughAttribute()]");
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} partial class {1}Proxy : System.ServiceModel.ClientBase<I{1}>, I{1}", o.HasClientType ? DataTypeCSGenerator.GenerateScope(o.ClientType.Scope) : DataTypeCSGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t{");
			code.AppendLine(string.Format("\t\tpublic {0}Proxy(string endpointConfigurationName) : base(endpointConfigurationName)", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.Append(GenerateMethodProxyConstructorCode(m, false));
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}Proxy(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.Append(GenerateMethodProxyConstructorCode(m, false));
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}Proxy(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.Append(GenerateMethodProxyConstructorCode(m, false));
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}Proxy(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.Append(GenerateMethodProxyConstructorCode(m, false));
			code.AppendLine("\t\t}");
			code.AppendLine();
			Host h = o.Parent.Owner.Namespace.GetServiceHost(o);
			if (h != null)
				code.Append(HostCSGenerator.GenerateClientCode45(h));
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
			code.AppendLine(GeneratePropertyClientCode(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
			code.AppendLine(GenerateMethodProxyCode45(m, false, false));
			code.AppendLine("\t}");

			return code.ToString();
		}

		#endregion

		#region - Service/Callback Interface Methods -

		public static string GenerateServiceInterfaceSyncMethod(Method o, bool IsCallback, bool IsAsync = false, bool IsAwait = false)
		{
			var code = new StringBuilder();

			if (o.Documentation != null)
			{
				code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
				foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
			}
			code.AppendLine(string.Format("\t\t[OperationContract(IsInitiating = {0}{1}{2}{3}{4})]", o.IsInitiating ? "true" : "false", o.IsTerminating ? ", IsTerminating = true" : "", o.IsOneWay ? ", IsOneWay = true" : "", o.ProtectionLevel != System.Net.Security.ProtectionLevel.None ? string.Format(", ProtectionLevel = System.Net.Security.ProtectionLevel.{0}", o.ProtectionLevel) : "", !string.IsNullOrEmpty(o.ClientName) ? string.Format(", Name = \"{0}\"", o.ClientName) : ""));
			if (!IsCallback && o.IsRESTMethod && (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET45 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35Client || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35))
				code.AppendLine(string.Format("\t\t[System.ServiceModel.Web.{0}(UriTemplate=\"{1}\", {2}BodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.{3}, RequestFormat = System.ServiceModel.Web.WebMessageFormat.{4}, ResponseFormat = System.ServiceModel.Web.WebMessageFormat.{5})]", o.REST.Method == MethodRESTVerbs.GET ? "WebGet" : "WebInvoke", o.REST.BuildUriTemplate(), o.REST.Method != MethodRESTVerbs.GET ? string.Format("Method = \"{0}\", ", o.REST.Method) : "", o.REST.BodyStyle, o.REST.RequestFormat, o.REST.ResponseFormat));
			code.AppendFormat("\t\t{0} {1}{2}{3}(", DataTypeCSGenerator.GenerateType(o.ReturnType), o.ServerName, IsAsync ? "Invoke" : "", IsAwait ? "Async" : "");
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0},", GenerateMethodParameterServerCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 1, 1);
			code.AppendLine(");");

			return code.ToString();
		}

		public static string GenerateServiceInterfaceMethodCode30(Method o, bool IsCallback)
		{
			return GenerateServiceInterfaceMethodCode35(o, IsCallback);
		}

		public static string GenerateServiceInterfaceMethodCode35(Method o, bool IsCallback)
		{
			return GenerateServiceInterfaceMethodCode40(o, IsCallback);
		}

		public static string GenerateServiceInterfaceMethodCode40(Method o, bool IsCallback)
		{
			var code = new StringBuilder();

			if (o.UseSyncPattern) code.Append(GenerateServiceInterfaceSyncMethod(o, IsCallback));

			if (o.UseAsyncPattern && CanGenerateAsync(o.Owner, true))
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation, true));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
					code.AppendLine(string.Format("\t\t///<param name='Callback'>The function to call when the operation is complete.</param>"));
					code.AppendLine(string.Format("\t\t///<param name='AsyncState'>An object representing the state of the operation.</param>"));
				}
				code.AppendLine(string.Format("\t\t[OperationContract(AsyncPattern = true, IsInitiating = {0}{1}{2}{3}{4})]", o.IsInitiating ? "true" : "false", o.IsTerminating ? ", IsTerminating = true" : "", o.IsOneWay ? ", IsOneWay = true" : "", o.ProtectionLevel != System.Net.Security.ProtectionLevel.None ? string.Format(", ProtectionLevel = System.Net.Security.ProtectionLevel.{0}", o.ProtectionLevel) : "", !string.IsNullOrEmpty(o.ClientName) ? string.Format(", Name = \"{0}\"", o.ClientName) : ""));
				if (!IsCallback && o.IsRESTMethod && (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET45 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35Client || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35))
					code.AppendLine(string.Format("\t\t[System.ServiceModel.Web.{0}(UriTemplate=\"{1}\", {2}BodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.{3}, RequestFormat = System.ServiceModel.Web.WebMessageFormat.{4}, ResponseFormat = System.ServiceModel.Web.WebMessageFormat.{5})]", o.REST.Method == MethodRESTVerbs.GET ? "WebGet" : "WebInvoke", o.REST.BuildUriTemplate(), o.REST.Method != MethodRESTVerbs.GET ? string.Format("Method = \"{0}\", ", o.REST.Method) : "", o.REST.BodyStyle, o.REST.RequestFormat, o.REST.ResponseFormat));
				code.AppendFormat("\t\tIAsyncResult Begin{0}Invoke(", o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0},", GenerateMethodParameterServerCode(op));
				code.AppendLine(" AsyncCallback Callback, object AsyncState);");
				if (o.Documentation != null)
				{
					code.AppendLine("\t\t///<summary>Finalizes the asynchronous operation.</summary>");
					code.AppendLine("\t\t///<returns>");
					code.AppendLine(string.Format("\t\t///{0}", o.Documentation.Returns.Replace(Environment.NewLine, Environment.NewLine + "\t///")));
					code.AppendLine("\t\t///</returns>");
					code.AppendLine(string.Format("\t\t///<param name='result'>The result of the operation.</param>"));
				}
				code.AppendFormat("\t\t{0} End{1}Invoke(IAsyncResult result);{2}", DataTypeCSGenerator.GenerateType(o.ReturnType), o.ServerName, Environment.NewLine);
			}
			else if (o.UseAsyncPattern && !CanGenerateAsync(o.Owner, true))		 //If server asynchrony is disabled and the sync pattern is disabled, use the sync pattern.
				code.Append(GenerateServiceInterfaceSyncMethod(o, IsCallback, true));

			return code.ToString();
		}

		public static string GenerateServiceInterfaceMethodCode45(Method o, bool IsCallback)
		{
			var code = new StringBuilder();

			code.Append(GenerateServiceInterfaceMethodCode40(o, IsCallback));

			if (o.UseAwaitPattern && CanGenerateAsync(o.Owner, true))
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				code.AppendLine(string.Format("\t\t[OperationContract(IsInitiating = {0}{1}{2}{3}{4})]", o.IsInitiating ? "true" : "false", o.IsTerminating ? ", IsTerminating = true" : "", o.IsOneWay ? ", IsOneWay = true" : "", o.ProtectionLevel != System.Net.Security.ProtectionLevel.None ? string.Format(", ProtectionLevel = System.Net.Security.ProtectionLevel.{0}", o.ProtectionLevel) : "", !string.IsNullOrEmpty(o.ClientName) ? string.Format(", Name = \"{0}\"", o.ClientName) : ""));
				if (!IsCallback && o.IsRESTMethod && (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET45 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35Client || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35))
					code.AppendLine(string.Format("\t\t[System.ServiceModel.Web.{0}(UriTemplate=\"{1}\", {2}BodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.{3}, RequestFormat = System.ServiceModel.Web.WebMessageFormat.{4}, ResponseFormat = System.ServiceModel.Web.WebMessageFormat.{5})]", o.REST.Method == MethodRESTVerbs.GET ? "WebGet" : "WebInvoke", o.REST.BuildUriTemplate(), o.REST.Method != MethodRESTVerbs.GET ? string.Format("Method = \"{0}\", ", o.REST.Method) : "", o.REST.BodyStyle, o.REST.RequestFormat, o.REST.ResponseFormat));
				code.AppendFormat("\t\tSystem.Threading.Tasks.Task<{0}> {1}Async(", DataTypeCSGenerator.GenerateType(o.ReturnType), o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0},", GenerateMethodParameterServerCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 1, 1);
				code.AppendLine(");");
			}
			else if (o.UseAwaitPattern && !CanGenerateAsync(o.Owner, true))		 //If server asynchrony is disabled and the sync pattern is disabled, use the sync pattern.
				code.Append(GenerateServiceInterfaceSyncMethod(o, IsCallback, false, true));

			return code.ToString();
		}

		#endregion

		#region - Client Interface Methods -

		public static string GenerateClientInterfaceSyncMethod(Method o, bool IsAsync = false, bool IsAwait = false)
		{
			var code = new StringBuilder();

			if (o.Documentation != null)
			{
				code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
				foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
			}
			code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/{1}/{2}{3}{4}\")]" : "\t\t[OperationContract(Action = \"{0}/{1}/{2}{3}{4}\", ReplyAction = \"{0}/{1}/{2}{3}{4}Response\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName, IsAsync ? "Invoke" : "", IsAwait ? "Async" : ""));
			code.AppendFormat("\t\t{0} {1}{2}{3}(", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName, IsAsync ? "Invoke" : "", IsAwait ? "Async" : "");
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}{1}", GenerateMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
			code.AppendLine(");");
			return code.ToString();
		}

		public static string GenerateClientInterfaceMethodCode30(Method o)
		{
			return GenerateClientInterfaceMethodCode35(o);
		}

		public static string GenerateClientInterfaceMethodCode35(Method o)
		{
			return GenerateClientInterfaceMethodCode40(o);
		}

		public static string GenerateClientInterfaceMethodCode40(Method o)
		{
			var code = new StringBuilder();

			if (o.UseSyncPattern) code.Append(GenerateClientInterfaceSyncMethod(o));

			if (o.UseAsyncPattern && CanGenerateAsync(o.Owner, false))
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation, true));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
					code.AppendLine(string.Format("\t\t///<param name='Callback'>The function to call when the operation is complete.</param>"));
					code.AppendLine(string.Format("\t\t///<param name='AsyncState'>An object representing the state of the operation.</param>"));
				}
				code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/{1}/{2}Invoke\")]" : "\t\t[OperationContract(Action = \"{0}/{1}/{2}Invoke\", ReplyAction = \"{0}/{1}/{2}InvokeResponse\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
				code.AppendFormat("\t\tIAsyncResult Begin{0}Invoke(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				code.AppendLine("AsyncCallback Callback, object AsyncState);");
				if (o.Documentation != null)
				{
					code.AppendLine("\t\t///<summary>Finalizes the asynchronous operation.</summary>");
					code.AppendLine("\t\t///<returns>");
					code.AppendLine(string.Format("\t\t///{0}", o.Documentation.Returns.Replace(Environment.NewLine, Environment.NewLine + "\t///")));
					code.AppendLine("\t\t///</returns>");
					code.AppendLine(string.Format("\t\t///<param name='result'>The result of the operation.</param>"));
				}
				code.AppendFormat("\t\t{0} End{1}Invoke(IAsyncResult result);{2}", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName, Environment.NewLine);
			}
			else if (o.UseAsyncPattern && !CanGenerateAsync(o.Owner, false))
				code.Append(GenerateClientInterfaceSyncMethod(o, true));

			return code.ToString();
		}

		public static string GenerateClientInterfaceMethodCode45(Method o)
		{
			var code = new StringBuilder();

			code.Append(GenerateClientInterfaceMethodCode40(o));

			if (o.UseAwaitPattern && CanGenerateAsync(o.Owner, false))
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/{1}/{2}Async\")]" : "\t\t[OperationContract(Action = \"{0}/{1}/{2}Async\", ReplyAction = \"{0}/{1}/{2}AsyncResponse\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tSystem.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				else code.AppendFormat("\t\tSystem.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}{1}", GenerateMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(");");
			}
			else if (o.UseAwaitPattern && !CanGenerateAsync(o.Owner, false))
				code.Append(GenerateClientInterfaceSyncMethod(o, false, true));

			return code.ToString();
		}

		#endregion

		#region - Proxy Methods -

		public static string GenerateMethodProxyConstructorCode(Method o, bool IsServer)
		{
			if (o.UseAsyncPattern && CanGenerateAsync(o.Owner, IsServer))
			{
				var code = new StringBuilder();
				code.AppendLine(string.Format("\t\t\tonBegin{0}Delegate = new BeginOperationDelegate(this.OnBegin{0});", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine(string.Format("\t\t\tonEnd{0}Delegate = new EndOperationDelegate(this.OnEnd{0});", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine(string.Format("\t\t\ton{0}CompletedDelegate = new System.Threading.SendOrPostCallback(this.On{0}Completed);", o.HasClientType ? o.ClientName : o.ServerName));
				return code.ToString();
			}
			return "";
		}

		public static string GenerateSyncMethodProxy(Method o, bool IsCallback, bool IsAsync = false, bool IsAwait = false)
		{
			var code = new StringBuilder();
			if (o.Documentation != null)
			{
				code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
				foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
			}
			code.AppendFormat("\t\tpublic {0} {1}{2}{3}(", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName, IsAsync ? "Invoke" : "", IsAwait ? "Async" : "");
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			code.AppendFormat("\t\t\t{0}{2}.{1}{3}{4}(", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName, IsCallback ? "__callback" : "base.Channel", IsAsync ? "Invoke" : "", IsAwait ? "Async" : "");
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", op.Name);
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");
			code.AppendLine("\t\t}");
			return code.ToString();
		}

		public static string GenerateMethodProxyCode30(Method o, bool IsServer, bool IsCallback)
		{
			var code = new StringBuilder();

			if (o.UseSyncPattern) code.Append(GenerateSyncMethodProxy(o, IsCallback));

			if (o.UseAsyncPattern && CanGenerateAsync(o.Owner, IsServer))
			{
				//Generate the delegate fields for this function
				code.AppendLine(string.Format("\t\tprivate readonly BeginOperationDelegate onBegin{0}Delegate;", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine(string.Format("\t\tprivate readonly EndOperationDelegate onEnd{0}Delegate;", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine(string.Format("\t\tprivate readonly System.Threading.SendOrPostCallback on{0}CompletedDelegate;", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine(string.Format("\t\tpublic Action<AsyncOperationCompletedArgs<{1}>> {0}Completed;", o.HasClientType ? o.ClientName : o.ServerName, o.ReturnType.Primitive == PrimitiveTypes.Void ? "object" : o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType)));

				//Generate the interface functions.
				if (o.Documentation != null)
				{
					code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation, true));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
					code.AppendLine(string.Format("\t\t///<param name='Callback'>The function to call when the operation is complete.</param>"));
					code.AppendLine(string.Format("\t\t///<param name='AsyncState'>An object representing the state of the operation.</param>"));
				}
				code.AppendFormat("\t\tIAsyncResult I{1}{2}.Begin{0}Invoke(", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : "");
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				code.AppendLine("AsyncCallback Callback, object AsyncState)");
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\t{0}{2}.Begin{1}Invoke(", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName, IsCallback ? "__callback" : "base.Channel");
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", op.Name);
				code.AppendLine("Callback, AsyncState);");
				code.AppendLine("\t\t}");
				if (o.Documentation != null)
				{
					code.AppendLine("\t\t///<summary>Finalizes the asynchronous operation.</summary>");
					code.AppendLine("\t\t///<returns>");
					code.AppendLine(string.Format("\t\t///{0}", o.Documentation.Returns.Replace(Environment.NewLine, Environment.NewLine + "\t///")));
					code.AppendLine("\t\t///</returns>");
					code.AppendLine(string.Format("\t\t///<param name='result'>The result of the operation.</param>"));
				}
				code.AppendLine(string.Format("\t\t{0} I{2}{3}.End{1}Invoke(IAsyncResult result)", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : ""));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\t{0}{2}.End{1}Invoke(result);", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName, IsCallback ? "__callback" : "base.Channel"));
				code.AppendLine("\t\t}");

				//Generate the delegate implementation functions.
				code.AppendLine(string.Format("\t\tprivate IAsyncResult OnBegin{0}(object[] Values, AsyncCallback Callback, object AsyncState)", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\treturn ((I{1}{2})this).Begin{0}Invoke(", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : "");
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("({0})Values[{1}], ", DataTypeCSGenerator.GenerateType(op.Type), o.Parameters.IndexOf(op));
				code.AppendLine("Callback, AsyncState);");
				code.AppendLine("\t\t}");
				code.AppendLine(string.Format("\t\tprivate object[] OnEnd{0}(IAsyncResult result)", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t{");
				if (o.ReturnType.Primitive == PrimitiveTypes.Void)
				{
					code.AppendLine(string.Format("\t\t\t((I{1}{2})this).End{0}Invoke(result);", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : ""));
					code.AppendLine("\t\t\treturn null;");
				}
				else
					code.AppendLine(string.Format("\t\t\treturn new object[] {{ ((I{1}{2})this).End{0}Invoke(result) }};", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : ""));
				code.AppendLine("\t\t}");
				code.AppendLine(string.Format("\t\tprivate void On{0}Completed(object state)", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\tif (this.{0}Completed == null) return;", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t\tInvokeAsyncCompletedEventArgs e = (InvokeAsyncCompletedEventArgs)state;");
				code.AppendLine(string.Format("\t\t\tthis.{0}Completed(new AsyncOperationCompletedArgs<{2}>({1}e.Error, e.Cancelled, e.UserState));", o.HasClientType ? o.ClientName : o.ServerName, o.ReturnType.Primitive == PrimitiveTypes.Void ? "null" : string.Format("({0})e.Results[0], ", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType)), o.ReturnType.Primitive == PrimitiveTypes.Void ? "object" : o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType)));
				code.AppendLine("\t\t}");

				//Generate invocation functions
				if (o.Documentation != null)
				{
					code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				code.AppendFormat("\t\tpublic void {0}Invoke(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}{1}", GenerateMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\tthis.{0}Invoke(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", op.Name);
				code.AppendLine("null);");
				code.AppendLine("\t\t}");
				if (o.Documentation != null)
				{
					code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
					code.AppendLine(string.Format("\t\t///<param name='userState'>Allows the user of this function to distinguish between different calls.</param>"));
				}
				code.AppendFormat("\t\tpublic void {0}Invoke(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				code.AppendLine("object userState)");
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\tInvokeAsync(this.onBegin{0}Delegate, new object[] {{ ", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}{1}", op.Name, o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(string.Format(" }}, this.onEnd{0}Delegate, this.on{0}CompletedDelegate, userState);", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t}");
			}
			else if (o.UseAsyncPattern && !CanGenerateAsync(o.Owner, IsServer))
				code.Append(GenerateSyncMethodProxy(o, IsCallback, true));

			return code.ToString();
		}

		public static string GenerateMethodProxyCode35(Method o, bool IsServer, bool IsCallback)
		{
			return GenerateMethodProxyCode40(o, IsServer, IsCallback);
		}

		public static string GenerateMethodProxyCode40(Method o, bool IsServer, bool IsCallback)
		{
			var code = new StringBuilder();

			if (o.UseSyncPattern) code.Append(GenerateSyncMethodProxy(o, IsCallback));

			if (o.UseAsyncPattern && CanGenerateAsync(o.Owner, IsServer))
			{
				//Generate the delegate fields for this function
				code.AppendLine(string.Format("\t\tprivate readonly BeginOperationDelegate onBegin{0}Delegate;", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine(string.Format("\t\tprivate readonly EndOperationDelegate onEnd{0}Delegate;", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine(string.Format("\t\tprivate readonly System.Threading.SendOrPostCallback on{0}CompletedDelegate;", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine(string.Format("\t\tpublic Action<{1}System.Exception, bool, object> {0}Completed;", o.HasClientType ? o.ClientName : o.ServerName, o.ReturnType.Primitive == PrimitiveTypes.Void ? "" : string.Format("{0}, ", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType))));

				//Generate the interface functions.
				if (o.Documentation != null)
				{
					code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation, true));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
					code.AppendLine(string.Format("\t\t///<param name='Callback'>The function to call when the operation is complete.</param>"));
					code.AppendLine(string.Format("\t\t///<param name='AsyncState'>An object representing the state of the operation.</param>"));
				}
				code.AppendFormat("\t\tIAsyncResult I{1}{2}.Begin{0}Invoke(", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : "");
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				code.AppendLine("AsyncCallback Callback, object AsyncState)");
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\t{0}{2}.Begin{1}Invoke(", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName, IsCallback ? "__callback" : "base.Channel");
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", op.Name);
				code.AppendLine("Callback, AsyncState);");
				code.AppendLine("\t\t}");
				if (o.Documentation != null)
				{
					code.AppendLine("\t\t///<summary>Finalizes the asynchronous operation.</summary>");
					code.AppendLine("\t\t///<returns>");
					code.AppendLine(string.Format("\t\t///{0}", o.Documentation.Returns.Replace(Environment.NewLine, Environment.NewLine + "\t///")));
					code.AppendLine("\t\t///</returns>");
					code.AppendLine(string.Format("\t\t///<param name='result'>The result of the operation.</param>"));
				}
				code.AppendLine(string.Format("\t\t{0} I{2}{3}.End{1}Invoke(IAsyncResult result)", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : ""));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\t{0}{2}.End{1}Invoke(result);", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName, IsCallback ? "__callback" : "base.Channel"));
				code.AppendLine("\t\t}");

				//Generate the delegate implementation functions.
				code.AppendLine(string.Format("\t\tprivate IAsyncResult OnBegin{0}(object[] Values, AsyncCallback Callback, object AsyncState)", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\treturn ((I{1}{2})this).Begin{0}Invoke(", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : "");
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("({0})Values[{1}], ", DataTypeCSGenerator.GenerateType(op.Type), o.Parameters.IndexOf(op));
				code.AppendLine("Callback, AsyncState);");
				code.AppendLine("\t\t}");
				code.AppendLine(string.Format("\t\tprivate object[] OnEnd{0}(IAsyncResult result)", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t{");
				if (o.ReturnType.Primitive == PrimitiveTypes.Void)
				{
					code.AppendLine(string.Format("\t\t\t((I{1}{2})this).End{0}Invoke(result);", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : ""));
					code.AppendLine("\t\t\treturn null;");
				}
				else
					code.AppendLine(string.Format("\t\t\treturn new object[] {{ ((I{1}{2})this).End{0}Invoke(result) }};", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : ""));
				code.AppendLine("\t\t}");
				code.AppendLine(string.Format("\t\tprivate void On{0}Completed(object state)", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\tif (this.{0}Completed == null) return;", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t\tInvokeAsyncCompletedEventArgs e = (InvokeAsyncCompletedEventArgs)state;");
				code.AppendLine(string.Format("\t\t\tthis.{0}Completed({1}e.Error, e.Cancelled, e.UserState);", o.HasClientType ? o.ClientName : o.ServerName, o.ReturnType.Primitive == PrimitiveTypes.Void ? "" : string.Format("({0})e.Results[0], ", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType))));
				code.AppendLine("\t\t}");

				//Generate invocation functions
				if (o.Documentation != null)
				{
					code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				code.AppendFormat("\t\tpublic void {0}Invoke(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}{1}", GenerateMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\tthis.{0}Invoke(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", op.Name);
				code.AppendLine("null);");
				code.AppendLine("\t\t}");
				if (o.Documentation != null)
				{
					code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
					code.AppendLine(string.Format("\t\t///<param name='userState'>Allows the user of this function to distinguish between different calls.</param>"));
				}
				code.AppendFormat("\t\tpublic void {0}Invoke(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				code.AppendLine("object userState)");
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\tInvokeAsync(this.onBegin{0}Delegate, new object[] {{ ", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}{1}", op.Name, o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(string.Format(" }}, this.onEnd{0}Delegate, this.on{0}CompletedDelegate, userState);", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t}");
			}
			else if (o.UseAsyncPattern && !CanGenerateAsync(o.Owner, IsServer))
				code.Append(GenerateSyncMethodProxy(o, IsCallback, true));

			return code.ToString();
		}

		public static string GenerateMethodProxyCode45(Method o, bool IsServer, bool IsCallback)
		{
			var code = new StringBuilder();

			code.Append(GenerateMethodProxyCode40(o, IsCallback, IsServer));

			if (o.UseAwaitPattern && CanGenerateAsync(o.Owner, IsServer))
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationCSGenerator.GenerateDocumentation(o.Documentation));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tSystem.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				else code.AppendFormat("\t\tpublic System.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeCSGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeCSGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}{1}", GenerateMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\treturn {1}.{0}Async(", o.HasClientType ? o.ClientName : o.ServerName, IsCallback ? "__callback" : "base.Channel");
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}{1}", op.Name, o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(");");
				code.AppendLine("\t\t}");
			}
			else if (o.UseAwaitPattern && !CanGenerateAsync(o.Owner, IsServer))
				code.Append(GenerateSyncMethodProxy(o, IsCallback, false, true));

			return code.ToString();
		}

		#endregion

		#region - Method Parameters -

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

		#endregion

		#region - Properties -

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

			code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/{1}/Get{2}\")]" : "\t\t[OperationContract(Action = \"{0}/{1}/Get{2}\", ReplyAction = \"{0}/{1}/Set{2}Response\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
			code.AppendLine(string.Format("\t\t{0} Get{1}();", DataTypeCSGenerator.GenerateType(o.ReturnType), o.ServerName));
			if (!o.IsReadOnly)
			{
				code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/{1}/Get{2}\")]" : "\t\t[OperationContract(Action = \"{0}/{1}/Get{2}\", ReplyAction = \"{0}/{1}/Set{2}Response\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
				code.AppendLine(string.Format("\t\tvoid Set{1}({0} value);", DataTypeCSGenerator.GenerateType(o.ReturnType), o.ServerName));
			}
			return code.ToString();
		}

		public static string GeneratePropertyClientCode(Property o)
		{
			var code = new StringBuilder();

			code.AppendLine(string.Format("\t\t{0} I{1}.Get{2}()", DataTypeCSGenerator.GenerateType(o.ReturnType), o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, o.ServerName));
			code.AppendLine("\t\t{");
			code.AppendLine(string.Format("\t\t\treturn base.Channel.Get{0}();", o.HasClientType ? o.ClientName : o.ServerName));
			code.AppendLine("\t\t}");

			if(!o.IsReadOnly)
			{
				code.AppendLine(string.Format("\t\tvoid I{1}.Set{2}({0} value)", DataTypeCSGenerator.GenerateType(o.ReturnType), o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, o.ServerName));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\tbase.Channel.Set{0}(value);", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t}");
			}

			code.AppendLine(string.Format(o.IsReadOnly == false ? "\t\tpublic {0} {1} {{ get {{ return ((I{2})this).Get{1}(); }} set {{ ((I{2})this).Set{1}(value); }} }}" : "\t\tpublic {0} {1} {{ get {{ return (({2})this).Get{1}(); }} }}", DataTypeCSGenerator.GenerateType(o.ReturnType), o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name));

			return code.ToString();
		}

		#endregion
	}
}