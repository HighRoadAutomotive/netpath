using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using NETPath.Projects;
using NETPath.Projects.Helpers;

namespace NETPath.Generators.NET.CS
{
	internal static class ServiceGenerator
	{

		public static void VerifyCode(Service o, Action<CompileMessage> AddMessage)
		{
			if (string.IsNullOrEmpty(o.Name))
				AddMessage(new CompileMessage("GS2000", "An service in the '" + o.Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
			else
				if (RegExs.MatchCodeName.IsMatch(o.Name) == false)
					AddMessage(new CompileMessage("GS2001", "The service '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
			if (o.HasClientType) 
				if (RegExs.MatchCodeName.IsMatch(o.ClientType.Name) == false)
					AddMessage(new CompileMessage("GS2002", "The service '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

			var Operations = new ObservableCollection<Operation>();
			Operations.AddRange(o.ServiceOperations);
			Operations.AddRange(o.CallbackOperations);

			foreach (Method m in Operations.Where(a => a.GetType() == typeof(Method)))
			{
				if (string.IsNullOrEmpty(m.ServerName))
					AddMessage(new CompileMessage("GS2004", "An method in the '" + o.Name + "' service has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
				else
				{
					if (RegExs.MatchCodeName.IsMatch(m.ServerName) == false)
						AddMessage(new CompileMessage("GS2005", "The method '" + m.ServerName + "' in the '" + o.Name + "' service contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
					if (m.ServerName == "__callback")
						AddMessage(new CompileMessage("GS2015", "The name of the method '" + m.ServerName + "' in the '" + o.Name + "' service is invalid. Please rename it.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
				}

				if (!string.IsNullOrEmpty(m.ClientName))
					if (RegExs.MatchCodeName.IsMatch(m.ClientName) == false)
						AddMessage(new CompileMessage("GS2006", "The method '" + m.ServerName + "' in the '" + o.Name + "' service contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
				if (m.ReturnType == null)
					AddMessage(new CompileMessage("GS2007", "The method '" + m.ServerName + "' in the '" + o.Name + "' service has a blank Return Type. A Return Type MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
				if (m.ReturnType.TypeMode == DataTypeMode.Namespace || m.ReturnType.TypeMode == DataTypeMode.Interface)
					AddMessage(new CompileMessage("GS2013", "The method return type '" + m.ReturnType + "' in the '" + o.Name + "' service is not a valid return type. Please specify a valid return type.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));

				foreach (MethodParameter mp in m.Parameters)
				{
					if(string.IsNullOrEmpty(mp.Name))
						AddMessage(new CompileMessage("GS2008", "The method parameter '" + m.ServerName + "' in the '" + o.Name + "' service has a parameter with a blank name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
					if (mp.Name == "__callback")
						AddMessage(new CompileMessage("GS2016", "The name of the method parameter '" + mp.Name + "' in the '" + m.ServerName + "' method is invalid. Please rename it.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
				}
			}

			foreach (Property p in Operations.Where(a => a.GetType() == typeof(Property)))
			{
				if (string.IsNullOrEmpty(p.ServerName))
					AddMessage(new CompileMessage("GS2010", "A property in the '" + o.Name + "' service has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o, p, p.GetType(), o.Parent.Owner.ID));
				else
				{
					if (RegExs.MatchCodeName.IsMatch(p.ServerName) == false)
						AddMessage(new CompileMessage("GS2011", "The property '" + p.ServerName + "' in the '" + o.Name + "' service contains invalid characters in the Server Name.", CompileMessageSeverity.ERROR, o, p, p.GetType(), o.Parent.Owner.ID));
					if (p.ServerName == "__callback")
						AddMessage(new CompileMessage("GS2015", "The name of the property '" + p.ServerName + "' in the '" + o.Name + "' service is invalid. Please rename it.", CompileMessageSeverity.ERROR, o, p, p.GetType(), o.Parent.Owner.ID));
				}
				if (p.ReturnType.TypeMode == DataTypeMode.Namespace || p.ReturnType.TypeMode == DataTypeMode.Interface)
					AddMessage(new CompileMessage("GS2014", "The property value type '" + p.ReturnType + "' in the '" + o.Name + "' service is not a valid value type. Please specify a valid value type.", CompileMessageSeverity.ERROR, o, p, p.GetType(), o.Parent.Owner.ID));
			}
		}

		public static bool CanGenerateAsync(Service o, bool IsServer)
		{
			return IsServer ? (o.AsynchronyMode == ServiceAsynchronyMode.Server || o.AsynchronyMode == ServiceAsynchronyMode.Both) : (o.AsynchronyMode == ServiceAsynchronyMode.Client || o.AsynchronyMode == ServiceAsynchronyMode.Both || o.AsynchronyMode == ServiceAsynchronyMode.Default);
		}

		#region - Server Interfaces -

		#region - Obsolete .NET 3.x Code -

		//public static string GenerateServerCode30(Service o)
		//{
		//	var code = new StringBuilder();
		//	if (o.ServiceDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ServiceDocumentation));
		//	foreach (DataType dt in o.KnownTypes)
		//		code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
		//	code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
		//	code.AppendLine(string.Format("\t[ServiceContract({0}SessionMode = System.ServiceModel.SessionMode.{1}, {2}{3}{4}Namespace = \"{5}\")]", o.HasCallback ? string.Format("CallbackContract = typeof(I{0}Callback), ", o.Name) : "", System.Enum.GetName(typeof(System.ServiceModel.SessionMode), o.SessionMode), o.ProtectionLevel != System.Net.Security.ProtectionLevel.None ? string.Format("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel)) : "", !string.IsNullOrEmpty(o.ConfigurationName) ? string.Format("ConfigurationName = \"{0}\", ", o.ConfigurationName) : "", o.HasClientType ? string.Format("Name = \"{0}\", ", o.ClientType.Name) : "", o.Parent.FullURI));
		//	code.AppendLine(string.Format("\t{0} interface I{1}", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
		//	code.AppendLine("\t{");
		//	foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
		//		code.AppendLine(GeneratePropertyServerCode40(p));
		//	foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
		//		code.AppendLine(GenerateServiceInterfaceMethodCode30(m, false));
		//	code.AppendLine("\t}");

		//	if (o.HasCallback)
		//	{
		//		//Generate the callback interface
		//		if (o.CallbackDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.CallbackDocumentation));
		//		foreach (DataType dt in o.KnownTypes)
		//			code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
		//		code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
		//		code.AppendLine(string.Format("\t{0} interface I{1}Callback", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
		//		code.AppendLine("\t{");
		//		foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
		//			code.AppendLine(GeneratePropertyServerCode30(p));
		//		foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
		//			code.AppendLine(GenerateServiceInterfaceMethodCode30(m, true));
		//		code.AppendLine("\t}");

		//		//Generate the callback facade implementation
		//		if (o.HasAsyncServiceOperations && CanGenerateAsync(o, false))
		//		{
		//			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
		//			code.AppendLine("\tpublic class AsyncOperationCompletedArgs<T>");
		//			code.AppendLine("\t{");
		//			code.AppendLine("\t\tprivate T result;");
		//			code.AppendLine("\t\tpublic T Result { get { return result; } private set { result = value; } }");
		//			code.AppendLine("\t\tprivate System.Exception error;");
		//			code.AppendLine("\t\tpublic System.Exception Error { get { return error; } private set { error = value; } }");
		//			code.AppendLine("\t\tprivate bool cancelled;");
		//			code.AppendLine("\t\tpublic bool Cancelled { get { return cancelled; } private set { cancelled = value; } }");
		//			code.AppendLine("\t\tprivate object userState;");
		//			code.AppendLine("\t\tpublic object UserState { get { return userState; } private set { userState = value; } }");
		//			code.AppendLine("\t\tpublic AsyncOperationCompletedArgs(T result, System.Exception error, bool cancelled, Object userState)");
		//			code.AppendLine("\t\t{");
		//			code.AppendLine("\t\t\tResult = result;");
		//			code.AppendLine("\t\t\tError = error;");
		//			code.AppendLine("\t\t\tCancelled = cancelled;");
		//			code.AppendLine("\t\t\tUserState = userState;");
		//			code.AppendLine("\t\t}");
		//			code.AppendLine("\t}");
		//		}
		//		if (o.CallbackDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.CallbackDocumentation));
		//		code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
		//		code.AppendLine(string.Format("\t{0} partial class {1}Callback : I{1}Callback", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
		//		code.AppendLine("\t{");
		//		code.AppendLine();
		//		code.AppendLine(string.Format("\t\tprivate readonly I{0}Callback __callback;", o.Name));
		//		code.AppendLine();
		//		code.AppendLine(string.Format("\t\tpublic {0}Callback()", o.Name));
		//		code.AppendLine("\t\t{");
		//		code.AppendLine(string.Format("\t\t\t__callback = System.ServiceModel.OperationContext.Current.GetCallbackChannel<I{0}Callback>();", o.Name));
		//		code.AppendLine("\t\t}");
		//		code.AppendLine();
		//		code.AppendLine(string.Format("\t\tpublic {0}Callback(I{0}Callback callback)", o.Name));
		//		code.AppendLine("\t\t{");
		//		code.AppendLine("\t\t\t__callback = callback;");
		//		code.AppendLine("\t\t}");
		//		code.AppendLine();
		//		if (o.HasAsyncCallbackOperations && CanGenerateAsync(o, true))
		//		{
		//			code.AppendLine("\t\tprotected class InvokeAsyncCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs");
		//			code.AppendLine("\t\t{");
		//			code.AppendLine("\t\t\tpublic object[] Results { get; set; }");
		//			code.AppendLine("\t\t\tpublic InvokeAsyncCompletedEventArgs(object[] results, System.Exception error, bool cancelled, Object userState) : base(error, cancelled, userState)");
		//			code.AppendLine("\t\t\t{");
		//			code.AppendLine("\t\t\t\tResults = results;");
		//			code.AppendLine("\t\t\t}");
		//			code.AppendLine("\t\t}");
		//			code.AppendLine();
		//			code.AppendLine("\t\tprotected delegate IAsyncResult BeginOperationDelegate(object[] inValues, AsyncCallback asyncCallback, Object state);");
		//			code.AppendLine("\t\tprotected delegate object[] EndOperationDelegate(IAsyncResult result);");
		//			code.AppendLine();
		//			code.AppendLine("\t\tprotected void InvokeAsync(BeginOperationDelegate beginOperationDelegate, object[] inValues, EndOperationDelegate endOperationDelegate, System.Threading.SendOrPostCallback operationCompletedCallback, object userState)");
		//			code.AppendLine("\t\t{");
		//			code.AppendLine("\t\t\tif (beginOperationDelegate == null) throw new ArgumentNullException(\"Argument 'beginOperationDelegate' cannot be null.\");");
		//			code.AppendLine("\t\t\tif (endOperationDelegate == null) throw new ArgumentNullException(\"Argument 'endOperationDelegate' cannot be null.\");");
		//			code.AppendLine("\t\t\tAsyncCallback cb = delegate(IAsyncResult ar)");
		//			code.AppendLine("\t\t\t{");
		//			code.AppendLine("\t\t\t\tobject[] results = null;");
		//			code.AppendLine("\t\t\t\tException error = null;");
		//			code.AppendLine("\t\t\t\ttry { results = endOperationDelegate(ar); }");
		//			code.AppendLine("\t\t\t\tcatch (Exception ex) { error = ex; }");
		//			code.AppendLine("\t\t\t\tif (operationCompletedCallback != null) operationCompletedCallback(new InvokeAsyncCompletedEventArgs(results, error, false, userState));");
		//			code.AppendLine("\t\t\t};");
		//			code.AppendLine("\t\t\tbeginOperationDelegate(inValues, cb, userState);");
		//			code.AppendLine("\t\t}");
		//			code.AppendLine();
		//		}
		//		foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
		//			code.AppendLine(GeneratePropertyClientCode(p));
		//		foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
		//			code.AppendLine(GenerateMethodProxyCode30(m, true, true));
		//		code.AppendLine("\t}");
		//	}

		//	return code.ToString();
		//}

		//public static string GenerateServerCode35(Service o)
		//{
		//	return GenerateServerCode40(o);
		//}

		#endregion

		public static string GenerateServerCode40(Service o)
		{
			var code = new StringBuilder();
			if (o.ServiceDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ServiceDocumentation));
			foreach (DataType dt in o.KnownTypes)
				code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t[ServiceContract({0}SessionMode = System.ServiceModel.SessionMode.{1}, {2}{3}{4}Namespace = \"{5}\")]", o.HasCallback || o.HasDCMOperations ? string.Format("CallbackContract = typeof(I{0}Callback), ", o.Name) : "", System.Enum.GetName(typeof(System.ServiceModel.SessionMode), o.SessionMode), o.ProtectionLevel != System.Net.Security.ProtectionLevel.None ? string.Format("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel)) : "", !string.IsNullOrEmpty(o.ConfigurationName) ? string.Format("ConfigurationName = \"{0}\", ", o.ConfigurationName) : "", o.HasClientType ? string.Format("Name = \"{0}\", ", o.ClientType.Name) : "", o.Parent.FullURI));
			code.AppendLine(string.Format("\t{0} interface I{1}", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
			code.AppendLine("\t{");
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
				code.AppendLine(GeneratePropertyServerCode40(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.AppendLine(GenerateServiceInterfaceMethodCode40(m, false));
			foreach (DataChangeMethod m in o.ServiceOperations.Where(a => a.GetType() == typeof(DataChangeMethod)))
				code.AppendLine(GenerateServiceInterfaceDCM40(m, true));
			code.AppendLine("\t}");

			if (o.HasCallback || o.HasDCMOperations)
			{
				//Generate the service proxy
				if (o.CallbackDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.CallbackDocumentation));
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendLine(string.Format("\t[System.ServiceModel.ServiceBehaviorAttribute(AutomaticSessionShutdown = {0}, ConcurrencyMode = ConcurrencyMode.{1}, IgnoreExtensionDataObject = {2}, IncludeExceptionDetailInFaults = {3}, MaxItemsInObjectGraph = {4}, {5}{6}UseSynchronizationContext = {7}, ValidateMustUnderstand = {8}, EnsureOrderedDispatch = {10}, InstanceContextMode = InstanceContextMode.{11}, {12}{13}AddressFilterMode = AddressFilterMode.{9})]", o.SBAutomaticSessionShutdown ? "true" : "false", o.SBConcurrencyMode, o.SBIgnoreExtensionDataObject ? "true" : "false", o.SBIncludeExceptionDetailInFaults ? "true" : "false", o.SBMaxItemsInObjectGraph > 0 ? Convert.ToString(o.SBMaxItemsInObjectGraph) : "Int32.MaxValue", o.SBTransactionIsolationLevel != IsolationLevel.Unspecified ? string.Format("TransactionIsolationLevel = System.Transactions.IsolationLevel.{0}, ", o.SBTransactionIsolationLevel) : "", o.SBTransactionTimeout.Ticks != 0L ? string.Format("TransactionTimeout = \"{0}\", ", o.SBTransactionTimeout) : "", o.SBUseSynchronizationContext ? "true" : "false", o.SBValidateMustUnderstand ? "true" : "false", o.SBAddressFilterMode, o.SBEnsureOrderedDispatch ? "true" : "false", o.SBInstanceContextMode, !o.SBReleaseServiceInstanceOnTransactionComplete ? string.Format("ReleaseServiceInstanceOnTransactionComplete = false, ") : "", o.SBTransactionAutoCompleteOnSessionClose ? string.Format("TransactionAutoCompleteOnSessionClose = true, ") : ""));
				code.AppendLine(string.Format("\t{0} abstract class {1}Base : ServerDuplexBase<{1}Base, {1}Callback, I{1}Callback>, I{1}", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
				code.AppendLine("\t{");
				foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GenerateServerProxyPropertyCode(p));
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateServerProxyMethodCode40(m));
				foreach (DataChangeMethod m in o.ServiceOperations.Where(a => a.GetType() == typeof(DataChangeMethod)))
					code.AppendLine(GenerateServiceImplementationDCM40(m, true));
				code.AppendLine("\t}");

				//Generate the callback interface
				if (o.CallbackDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.CallbackDocumentation));
				foreach (DataType dt in o.KnownTypes)
					code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendLine(string.Format("\t{0} interface I{1}Callback", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
				code.AppendLine("\t{");
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyServerCode40(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateServiceInterfaceMethodCode40(m, true));
				foreach (DataChangeMethod m in o.ServiceOperations.Where(a => a.GetType() == typeof(DataChangeMethod)))
					code.AppendLine(GenerateCallbackInterfaceDCM40(m, true));
				code.AppendLine("\t}");

				//Generate the callback facade implementation
				if (o.CallbackDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.CallbackDocumentation));
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendLine(string.Format("\t{0} partial class {1}Callback : ServerCallbackBase<I{1}Callback>, I{1}Callback", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
				code.AppendLine("\t{");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Callback()", o.Name));
				code.AppendLine("\t\t{");
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
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyClientCode(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateMethodProxyCode40(m, true, true));
				foreach (DataChangeMethod m in o.ServiceOperations.Where(a => a.GetType() == typeof(DataChangeMethod)))
					code.AppendLine(GenerateCallbackImplementationDCM40(m, true));
				code.AppendLine("\t}");
			}

			return code.ToString();
		}

		public static string GenerateServerCode45(Service o)
		{
			var code = new StringBuilder();
			if (o.ServiceDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ServiceDocumentation));
			foreach (DataType dt in o.KnownTypes)
				code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t[ServiceContract({0}SessionMode = System.ServiceModel.SessionMode.{1}, {2}{3}{4}Namespace = \"{5}\")]", o.HasCallback || o.HasDCMOperations ? string.Format("CallbackContract = typeof(I{0}Callback), ", o.Name) : "", System.Enum.GetName(typeof(System.ServiceModel.SessionMode), o.SessionMode), o.ProtectionLevel != System.Net.Security.ProtectionLevel.None ? string.Format("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel)) : "", !string.IsNullOrEmpty(o.ConfigurationName) ? string.Format("ConfigurationName = \"{0}\", ", o.ConfigurationName) : "", o.HasClientType ? string.Format("Name = \"{0}\", ", o.ClientType.Name) : "", o.Parent.FullURI));
			code.AppendLine(string.Format("\t{0} interface I{1}", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
			code.AppendLine("\t{");
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
				code.AppendLine(GeneratePropertyServerCode45(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.AppendLine(GenerateServiceInterfaceMethodCode45(m, false));
			foreach (DataChangeMethod m in o.ServiceOperations.Where(a => a.GetType() == typeof(DataChangeMethod)))
				code.AppendLine(GenerateServiceInterfaceDCM45(m, true));
			code.AppendLine("\t}");

			if (o.HasCallback || o.HasDCMOperations)
			{
				//Generate the service proxy
				if (o.CallbackDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.CallbackDocumentation));
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendLine(string.Format("\t[System.ServiceModel.ServiceBehaviorAttribute(AutomaticSessionShutdown = {0}, ConcurrencyMode = ConcurrencyMode.{1}, IgnoreExtensionDataObject = {2}, IncludeExceptionDetailInFaults = {3}, MaxItemsInObjectGraph = {4}, {5}{6}UseSynchronizationContext = {7}, ValidateMustUnderstand = {8}, EnsureOrderedDispatch = {10}, InstanceContextMode = InstanceContextMode.{11}, {12}{13}AddressFilterMode = AddressFilterMode.{9})]", o.SBAutomaticSessionShutdown ? "true" : "false", o.SBConcurrencyMode, o.SBIgnoreExtensionDataObject ? "true" : "false", o.SBIncludeExceptionDetailInFaults ? "true" : "false", o.SBMaxItemsInObjectGraph > 0 ? Convert.ToString(o.SBMaxItemsInObjectGraph) : "Int32.MaxValue", o.SBTransactionIsolationLevel != IsolationLevel.Unspecified ? string.Format("TransactionIsolationLevel = System.Transactions.IsolationLevel.{0}, ", o.SBTransactionIsolationLevel) : "", o.SBTransactionTimeout.Ticks != 0L ? string.Format("TransactionTimeout = \"{0}\", ", o.SBTransactionTimeout) : "", o.SBUseSynchronizationContext ? "true" : "false", o.SBValidateMustUnderstand ? "true" : "false", o.SBAddressFilterMode, o.SBEnsureOrderedDispatch ? "true" : "false", o.SBInstanceContextMode, !o.SBReleaseServiceInstanceOnTransactionComplete ? string.Format("ReleaseServiceInstanceOnTransactionComplete = false, ") : "", o.SBTransactionAutoCompleteOnSessionClose ? string.Format("TransactionAutoCompleteOnSessionClose = true, ") : ""));
				code.AppendLine(string.Format("\t{0} abstract class {1}Base : ServerDuplexBase<{1}Base, {1}Callback, I{1}Callback>, I{1}", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
				code.AppendLine("\t{");
				foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GenerateServerProxyPropertyCode(p));
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateServerProxyMethodCode45(m));
				foreach (DataChangeMethod m in o.ServiceOperations.Where(a => a.GetType() == typeof(DataChangeMethod)))
					code.AppendLine(GenerateServiceImplementationDCM45(m, true));
				code.AppendLine("\t}");

				//Generate the callback interface
				if (o.CallbackDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.CallbackDocumentation));
				foreach (DataType dt in o.KnownTypes)
					code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendLine(string.Format("\t{0} interface I{1}Callback", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
				code.AppendLine("\t{");
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyInterfaceCode45(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateServiceInterfaceMethodCode45(m, true));
				foreach (DataChangeMethod m in o.ServiceOperations.Where(a => a.GetType() == typeof(DataChangeMethod)))
					code.AppendLine(GenerateCallbackInterfaceDCM45(m, true));
				code.AppendLine("\t}");

				//Generate the callback facade implementation
				if (o.CallbackDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.CallbackDocumentation));
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendLine(string.Format("\t{0} partial class {1}Callback : ServerCallbackBase<I{1}Callback>, I{1}Callback", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
				code.AppendLine("\t{");
				code.AppendLine(string.Format("\t\tpublic {0}Callback()", o.Name));
				code.AppendLine("\t\t{");
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
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyClientCode(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateServerCallbackMethodProxyCode45(m));
				foreach (DataChangeMethod m in o.ServiceOperations.Where(a => a.GetType() == typeof(DataChangeMethod)))
					code.AppendLine(GenerateCallbackImplementationDCM45(m, true));
				code.AppendLine("\t}");
			}

			return code.ToString();
		}

		#endregion

		#region - Client Interfaces -

		#region - Obsolete .NET 3.x Code -

		//public static string GenerateClientCode30(Service o)
		//{
		//	var code = new StringBuilder();

		//	//Generate the Client interface
		//	if (o.ServiceDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ServiceDocumentation));
		//	if (o.ClientType != null)
		//		foreach (DataType dt in o.ClientType.KnownTypes)
		//			code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
		//	code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
		//	code.AppendLine(string.Format("\t{0} interface I{1}", o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name));
		//	code.AppendLine("\t{");
		//	foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
		//		code.AppendLine(GeneratePropertyInterfaceCode40(p));
		//	foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
		//		code.AppendLine(GenerateClientInterfaceMethodCode30(m));
		//	code.AppendLine("\t}");
		//	code.AppendLine();
		//	//Generate Callback Interface (if any)
		//	if (o.HasCallback)
		//	{
		//		if (o.CallbackDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.CallbackDocumentation));
		//		foreach (DataType dt in o.KnownTypes)
		//			code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
		//		code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
		//		code.AppendFormat("\t{0} interface I{1}Callback{2}", o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope), o.Name, Environment.NewLine);
		//		code.AppendLine("\t{");
		//		foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
		//			code.AppendLine(GeneratePropertyInterfaceCode40(p));
		//		foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
		//			code.AppendLine(GenerateClientInterfaceMethodCode30(m));
		//		code.AppendLine("\t}");
		//		code.AppendLine();
		//	}
		//	//Generate Channel Interface
		//	code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
		//	code.AppendLine(string.Format("\t{0} interface I{1}Channel : I{2}, System.ServiceModel.IClientChannel", o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, o.HasClientType ? o.ClientType.Name : o.Name));
		//	code.AppendLine("\t{");
		//	code.AppendLine("\t}");
		//	code.AppendLine();
		//	//Generate the Proxy Class
		//	code.AppendLine("\t[System.Diagnostics.DebuggerStepThroughAttribute()]");
		//	code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
		//	code.AppendLine(string.Format("\t{0} partial class {1}Proxy : System.ServiceModel.{2}<I{1}>, I{1}", o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, o.HasCallbackOperations ? "DuplexClientBase" : "ClientBase"));
		//	code.AppendLine("\t{");
		//	if (o.HasCallbackOperations)
		//	{
		//		code.AppendLine(string.Format("\t\tpublic {0}Proxy(System.ServiceModel.InstanceContext callbackInstance) : base(callbackInstance)", o.HasClientType ? o.ClientType.Name : o.Name));
		//		code.AppendLine("\t\t{");
		//		foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
		//			code.Append(GenerateMethodProxyConstructorCode(m, false));
		//		code.AppendLine("\t\t}");
		//		code.AppendLine();
		//		code.AppendLine(string.Format("\t\tpublic {0}Proxy(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)", o.HasClientType ? o.ClientType.Name : o.Name));
		//		code.AppendLine("\t\t{");
		//		foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
		//			code.Append(GenerateMethodProxyConstructorCode(m, false));
		//		code.AppendLine("\t\t}");
		//		code.AppendLine();
		//		code.AppendLine(string.Format("\t\tpublic {0}Proxy(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name));
		//		code.AppendLine("\t\t{");
		//		foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
		//			code.Append(GenerateMethodProxyConstructorCode(m, false));
		//		code.AppendLine("\t\t}");
		//		code.AppendLine();
		//		code.AppendLine(string.Format("\t\tpublic {0}Proxy(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name));
		//		code.AppendLine("\t\t{");
		//		foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
		//			code.Append(GenerateMethodProxyConstructorCode(m, false));
		//		code.AppendLine("\t\t}");
		//		code.AppendLine();
		//		code.AppendLine(string.Format("\t\tpublic {0}Proxy(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name));
		//		code.AppendLine("\t\t{");
		//		foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
		//			code.Append(GenerateMethodProxyConstructorCode(m, false));
		//		code.AppendLine("\t\t}");
		//		code.AppendLine();
		//		code.AppendLine(string.Format("\t\tpublic {0}Proxy(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Description.ServiceEndpoint endpoint) : base(callbackInstance, endpoint)", o.HasClientType ? o.ClientType.Name : o.Name));
		//		code.AppendLine("\t\t{");
		//		foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
		//			code.Append(GenerateMethodProxyConstructorCode(m, false));
		//		code.AppendLine("\t\t}");
		//		code.AppendLine();
		//	}

		//	else
		//	{
		//		code.AppendLine(string.Format("\t\tpublic {0}Proxy(string endpointConfigurationName) : base(endpointConfigurationName)", o.HasClientType ? o.ClientType.Name : o.Name));
		//		code.AppendLine("\t\t{");
		//		foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
		//			code.Append(GenerateMethodProxyConstructorCode(m, false));
		//		code.AppendLine("\t\t}");
		//		code.AppendLine();
		//		code.AppendLine(string.Format("\t\tpublic {0}Proxy(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name));
		//		code.AppendLine("\t\t{");
		//		foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
		//			code.Append(GenerateMethodProxyConstructorCode(m, false));
		//		code.AppendLine("\t\t}");
		//		code.AppendLine();
		//		code.AppendLine(string.Format("\t\tpublic {0}Proxy(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name));
		//		code.AppendLine("\t\t{");
		//		foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
		//			code.Append(GenerateMethodProxyConstructorCode(m, false));
		//		code.AppendLine("\t\t}");
		//		code.AppendLine();
		//		code.AppendLine(string.Format("\t\tpublic {0}Proxy(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name));
		//		code.AppendLine("\t\t{");
		//		foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
		//			code.Append(GenerateMethodProxyConstructorCode(m, false));
		//		code.AppendLine("\t\t}");
		//		code.AppendLine();
		//		code.AppendLine(string.Format("\t\tpublic {0}Proxy(System.ServiceModel.Description.ServiceEndpoint endpoint) : base(endpoint)", o.HasClientType ? o.ClientType.Name : o.Name));
		//		code.AppendLine("\t\t{");
		//		foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
		//			code.Append(GenerateMethodProxyConstructorCode(m, false));
		//		code.AppendLine("\t\t}");
		//		code.AppendLine();
		//	}

		//	Host h = o.Parent.Owner.Namespace.GetServiceHost(o);
		//	if (h != null)
		//		code.Append(HostGenerator.GenerateClientCode40(h));
		//	foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
		//		code.AppendLine(GeneratePropertyClientCode(p));
		//	foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
		//		code.AppendLine(GenerateMethodProxyCode30(m, false, false));
		//	code.AppendLine("\t}");

		//	return code.ToString();
		//}

		//public static string GenerateClientCode35(Service o)
		//{
		//	return GenerateClientCode40(o);
		//}

		#endregion

		public static string GenerateClientCode40(Service o)
		{
			var code = new StringBuilder();

			//Generate the Client interface
			if (o.ServiceDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ServiceDocumentation));
			if (o.ClientType != null)
				foreach (DataType dt in o.ClientType.KnownTypes)
					code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t[ServiceContract({0}SessionMode = System.ServiceModel.SessionMode.{1}, {2}{3}{4}Namespace = \"{5}\")]", o.HasCallback || o.HasDCMOperations ? string.Format("CallbackContract = typeof(I{0}Callback), ", o.Name) : "", System.Enum.GetName(typeof(System.ServiceModel.SessionMode), o.SessionMode), o.ProtectionLevel != System.Net.Security.ProtectionLevel.None ? string.Format("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel)) : "", !string.IsNullOrEmpty(o.ConfigurationName) ? string.Format("ConfigurationName = \"{0}\", ", o.ConfigurationName) : "", o.HasClientType ? string.Format("Name = \"{0}\", ", o.ClientType.Name) : "", o.Parent.FullURI));
			code.AppendLine(string.Format("\t{0} interface I{1}", o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t{");
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
				code.AppendLine(GeneratePropertyInterfaceCode40(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.AppendLine(GenerateClientInterfaceMethodCode40(m, false));
			foreach (DataChangeMethod m in o.ServiceOperations.Where(a => a.GetType() == typeof(DataChangeMethod)))
				code.AppendLine(GenerateServiceInterfaceDCM40(m, false));
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate Callback Interface (if any)
			if (o.HasCallback || o.HasDCMOperations)
			{
				if (o.CallbackDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.CallbackDocumentation));
				foreach (DataType dt in o.KnownTypes)
					code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendFormat("\t{0} interface I{1}Callback{2}", o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope), o.Name, Environment.NewLine);
				code.AppendLine("\t{");
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyInterfaceCode40(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateClientInterfaceMethodCode40(m, true));
				foreach (DataChangeMethod m in o.ServiceOperations.Where(a => a.GetType() == typeof(DataChangeMethod)))
					code.AppendLine(GenerateCallbackInterfaceDCM40(m, false));
				code.AppendLine("\t}");
				code.AppendLine();
			}
			//Generate Channel Interface
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} interface I{1}Channel : I{2}, System.ServiceModel.IClientChannel", o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t{");
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate the Proxy Class
			code.AppendLine("\t[System.Diagnostics.DebuggerStepThroughAttribute()]");
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} partial class {1}Proxy : {2}, I{1}", o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations || o.HasCallbackOperations ? string.Format("System.ServiceModel.ClientDuplexBaseEx<{0}Proxy, I{0}>", o.HasClientType ? o.ClientType.Name : o.Name) : string.Format("System.ServiceModel.ClientBaseEx<{0}Proxy, I{0}>", o.HasClientType ? o.ClientType.Name : o.Name)));
			code.AppendLine("\t{");
			if (o.HasCallbackOperations || o.HasDCMOperations)
			{
				code.AppendLine(string.Format("\t\tpublic {0}Proxy()", o.HasClientType ? o.ClientType.Name : o.Name));
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = Guid.NewGuid();");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				if (o.HasDCMOperations)
				{
					code.AppendLine(string.Format("\t\tpublic {0}Proxy(Guid ClientID)", o.HasClientType ? o.ClientType.Name : o.Name));
					code.AppendLine("\t\t{");
					code.AppendLine("\t\t\tbase.ClientID = ClientID;");
					foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
						code.Append(GenerateMethodProxyConstructorCode(m, false));
					code.AppendLine("\t\t}");
					code.AppendLine();
				}
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}object callback, string endpointConfigurationName) : base(callback, endpointConfigurationName)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = ClientID;");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}object callback, string endpointConfigurationName, string remoteAddress) : base(callback, endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = ClientID;");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}object callback, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(callback, endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = ClientID;");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}object callback, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(callback, binding, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = ClientID;");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}object callback, System.ServiceModel.Description.ServiceEndpoint endpoint) : base(callback, endpoint)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = ClientID;");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
			}
			else
			{
				code.AppendLine(string.Format("\t\tpublic {0}Proxy()", o.HasClientType ? o.ClientType.Name : o.Name));
				code.AppendLine("\t\t{");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}string endpointConfigurationName) : base(endpointConfigurationName)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
			}
			Host h = o.Parent.Owner.Namespace.GetServiceHost(o);
			if (h != null)
				code.Append(HostGenerator.GenerateClientCode40(h, o.HasDCMOperations));
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof(Property)))
				code.AppendLine(GeneratePropertyClientCode(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				code.AppendLine(GenerateMethodProxyCode40(m, false, false));
			foreach (DataChangeMethod m in o.ServiceOperations.Where(a => a.GetType() == typeof(DataChangeMethod)))
				code.AppendLine(GenerateServiceImplementationDCM40(m, false));
			code.AppendLine("\t}");
			if (o.HasDCMOperations)
			{
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendLine(string.Format("\t[System.ServiceModel.CallbackBehaviorAttribute(AutomaticSessionShutdown = {0}, ConcurrencyMode = ConcurrencyMode.{1}, IgnoreExtensionDataObject = {2}, IncludeExceptionDetailInFaults = {3}, MaxItemsInObjectGraph = {4}, {5}{6}UseSynchronizationContext = {7}, ValidateMustUnderstand = {8})]", o.CBAutomaticSessionShutdown ? "true" : "false", o.CBConcurrencyMode, o.CBIgnoreExtensionDataObject ? "true" : "false", o.CBIncludeExceptionDetailInFaults ? "true" : "false", o.CBMaxItemsInObjectGraph > 0 ? Convert.ToString(o.CBMaxItemsInObjectGraph) : "Int32.MaxValue", o.CBTransactionIsolationLevel != IsolationLevel.Unspecified ? string.Format("TransactionIsolationLevel = System.Transactions.IsolationLevel.{0}, ", o.CBTransactionIsolationLevel) : "", o.CBTransactionTimeout.Ticks != 0L ? string.Format("TransactionTimeout = \"{0}\", ", o.CBTransactionTimeout) : "", o.CBUseSynchronizationContext ? "true" : "false", o.CBValidateMustUnderstand ? "true" : "false"));
				code.AppendLine(string.Format("\t{0} abstract class {1}Callback : I{1}Callback", o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name));
				code.AppendLine("\t{");
				code.AppendLine(string.Format("\t\tpublic {0}Callback() {{ }}", o.HasClientType ? o.ClientType.Name : o.Name));
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyServerCode40(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateCallbackClientMethodCode40(m));
				foreach (DataChangeMethod m in o.ServiceOperations.Where(a => a.GetType() == typeof(DataChangeMethod)))
					code.AppendLine(GenerateCallbackImplementationDCM40(m, false));
				code.AppendLine("\t}");
			}

			return code.ToString();
		}

		public static string GenerateClientCode45(Service o)
		{
			var code = new StringBuilder();

			//Generate the Client interface
			if (o.ServiceDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ServiceDocumentation));
			foreach (DataType dt in o.KnownTypes)
				code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8 ? string.Format("\t[ServiceContract({0}{1}{2}Namespace = \"{3}\")]", o.HasCallback || o.HasDCMOperations ? string.Format("CallbackContract = typeof(I{0}Callback), ", o.Name) : "", !string.IsNullOrEmpty(o.ConfigurationName) ? string.Format("ConfigurationName = \"{0}\", ", o.ConfigurationName) : "", o.HasClientType ? string.Format("Name = \"{0}\", ", o.ClientType.Name) : "", o.Parent.FullURI) : string.Format("\t[ServiceContract({0}SessionMode = System.ServiceModel.SessionMode.{1}, {2}{3}{4}Namespace = \"{5}\")]", o.HasCallback || o.HasDCMOperations ? string.Format("CallbackContract = typeof(I{0}Callback), ", o.Name) : "", System.Enum.GetName(typeof(System.ServiceModel.SessionMode), o.SessionMode), o.ProtectionLevel != System.Net.Security.ProtectionLevel.None ? string.Format("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel)) : "", !string.IsNullOrEmpty(o.ConfigurationName) ? string.Format("ConfigurationName = \"{0}\", ", o.ConfigurationName) : "", o.HasClientType ? string.Format("Name = \"{0}\", ", o.ClientType.Name) : "", o.Parent.FullURI));
			code.AppendLine(string.Format("\t{0} interface I{1}", o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t{");
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof (Property)))
				code.AppendLine(GeneratePropertyInterfaceCode45(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof (Method)))
				code.AppendLine(GenerateClientInterfaceMethodCode45(m, false));
			foreach (DataChangeMethod m in o.ServiceOperations.Where(a => a.GetType() == typeof(DataChangeMethod)))
				code.AppendLine(GenerateServiceInterfaceDCM45(m, false));
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate Callback Interface (if any)
			if (o.HasCallback || o.HasDCMOperations)
			{
				if (o.CallbackDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.CallbackDocumentation));
				foreach (DataType dt in o.KnownTypes)
					code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendFormat("\t{0} interface I{1}Callback{2}", o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope), o.Name, Environment.NewLine);
				code.AppendLine("\t{");
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof (Property)))
					code.AppendLine(GeneratePropertyInterfaceCode45(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof (Method)))
					code.AppendLine(GenerateClientInterfaceMethodCode45(m, true));
				foreach (DataChangeMethod m in o.ServiceOperations.Where(a => a.GetType() == typeof(DataChangeMethod)))
					code.AppendLine(GenerateCallbackInterfaceDCM45(m, false));
				code.AppendLine("\t}");
				code.AppendLine();
			}
			//Generate Channel Interface
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} interface I{1}Channel : I{2}, System.ServiceModel.IClientChannel", o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t{");
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate the Proxy Class
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} partial class {1}Proxy : {2}, I{1}", o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations || o.HasCallbackOperations ? string.Format("System.ServiceModel.ClientDuplexBaseEx<{0}Proxy, I{0}>", o.HasClientType ? o.ClientType.Name : o.Name) : string.Format("System.ServiceModel.ClientBaseEx<{0}Proxy, I{0}>", o.HasClientType ? o.ClientType.Name : o.Name)));
			code.AppendLine("\t{");
			if (o.HasCallbackOperations || o.HasDCMOperations)
			{
				code.AppendLine(string.Format("\t\tpublic {0}Proxy()", o.HasClientType ? o.ClientType.Name : o.Name));
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = Guid.NewGuid();");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				if (o.HasDCMOperations)
				{
					code.AppendLine(string.Format("\t\tpublic {0}Proxy(Guid ClientID)", o.HasClientType ? o.ClientType.Name : o.Name));
					code.AppendLine("\t\t{");
					code.AppendLine("\t\t\tbase.ClientID = ClientID;");
					foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof (Method)))
						code.Append(GenerateMethodProxyConstructorCode(m, false));
					code.AppendLine("\t\t}");
					code.AppendLine();
				}
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}object callback, string endpointConfigurationName) : base(callback, endpointConfigurationName)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = ClientID;");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}object callback, string endpointConfigurationName, string remoteAddress) : base(callback, endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = ClientID;");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}object callback, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(callback, endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = ClientID;");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}object callback, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(callback, binding, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = ClientID;");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}object callback, System.ServiceModel.Description.ServiceEndpoint endpoint) : base(callback, endpoint)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = ClientID;");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
			}
			else
			{
				code.AppendLine(string.Format("\t\tpublic {0}Proxy()", o.HasClientType ? o.ClientType.Name : o.Name));
				code.AppendLine("\t\t{");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}string endpointConfigurationName) : base(endpointConfigurationName)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Proxy({1}System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress)", o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
					code.Append(GenerateMethodProxyConstructorCode(m, false));
				code.AppendLine("\t\t}");
				code.AppendLine();
			}
			Host h = o.Parent.Owner.Namespace.GetServiceHost(o);
			if (h != null)
				code.Append(HostGenerator.GenerateClientCode45(h, o.HasDCMOperations));
			foreach (Property p in o.ServiceOperations.Where(a => a.GetType() == typeof (Property)))
				code.AppendLine(GeneratePropertyClientCode(p));
			foreach (Method m in o.ServiceOperations.Where(a => a.GetType() == typeof (Method)))
				code.AppendLine(GenerateMethodProxyCode45(m, false, false));
			foreach (DataChangeMethod m in o.ServiceOperations.Where(a => a.GetType() == typeof(DataChangeMethod)))
				code.AppendLine(GenerateServiceImplementationDCM45(m, false));
			code.AppendLine("\t}");
			if (o.HasDCMOperations)
			{
				code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
				code.AppendLine(string.Format("\t[System.ServiceModel.CallbackBehaviorAttribute(AutomaticSessionShutdown = {0}, ConcurrencyMode = ConcurrencyMode.{1}, IgnoreExtensionDataObject = {2}, IncludeExceptionDetailInFaults = {3}, MaxItemsInObjectGraph = {4}, {5}{6}UseSynchronizationContext = {7}, ValidateMustUnderstand = {8})]", o.CBAutomaticSessionShutdown ? "true" : "false", o.CBConcurrencyMode, o.CBIgnoreExtensionDataObject ? "true" : "false", o.CBIncludeExceptionDetailInFaults ? "true" : "false", o.CBMaxItemsInObjectGraph > 0 ? Convert.ToString(o.CBMaxItemsInObjectGraph) : "Int32.MaxValue", o.CBTransactionIsolationLevel != IsolationLevel.Unspecified ? string.Format("TransactionIsolationLevel = System.Transactions.IsolationLevel.{0}, ", o.CBTransactionIsolationLevel) : "", o.CBTransactionTimeout.Ticks != 0L ? string.Format("TransactionTimeout = \"{0}\", ", o.CBTransactionTimeout) : "", o.CBUseSynchronizationContext ? "true" : "false", o.CBValidateMustUnderstand ? "true" : "false"));
				code.AppendLine(string.Format("\t{0} abstract class {1}Callback : I{1}Callback", o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name));
				code.AppendLine("\t{");
				foreach (Property p in o.CallbackOperations.Where(a => a.GetType() == typeof(Property)))
					code.AppendLine(GeneratePropertyServerCode45(p));
				foreach (Method m in o.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
					code.AppendLine(GenerateCallbackClientMethodCode45(m));
				foreach (DataChangeMethod m in o.ServiceOperations.Where(a => a.GetType() == typeof(DataChangeMethod)))
					code.AppendLine(GenerateCallbackImplementationDCM45(m, false));
				code.AppendLine("\t}");
			}

			return code.ToString();
		}

		#endregion

		#region - Service/Callback Interface Methods -

		public static string GenerateServiceInterfaceSyncMethod(Method o, bool IsCallback)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
			}
			code.AppendLine(string.Format("\t\t[OperationContract({0}{1}{2}{3})]", string.Format("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}", o.ProtectionLevel), o.Owner.SessionMode == SessionMode.Required ? o.IsInitiating ? ", IsInitiating = true" : ", IsInitiating = false" : "", o.Owner.SessionMode == SessionMode.Required ? o.IsTerminating ? ", IsTerminating = true" : ", IsTerminating = false" : "", o.IsOneWay ? ", IsOneWay = true" : ""));
			if (!IsCallback && o.IsRESTMethod && (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET45 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35Client || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35))
				code.AppendLine(string.Format("\t\t[System.ServiceModel.Web.{0}(UriTemplate=\"{1}\", {2}BodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.{3}, RequestFormat = System.ServiceModel.Web.WebMessageFormat.{4}, ResponseFormat = System.ServiceModel.Web.WebMessageFormat.{5})]", o.REST.Method == MethodRESTVerbs.GET ? "WebGet" : "WebInvoke", o.REST.UriTemplate, o.REST.Method != MethodRESTVerbs.GET ? string.Format("Method = \"{0}\", ", o.REST.Method) : "", o.REST.BodyStyle, o.REST.RequestFormat, o.REST.ResponseFormat));
			code.AppendFormat("\t\t{0} {1}(", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0},", GenerateMethodParameterServerCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 1, 1);
			code.AppendLine(");");

			return code.ToString();
		}

		public static string GenerateServiceInterfaceMethodCode40(Method o, bool IsCallback)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			
			if (o.UseAsyncPattern && CanGenerateAsync(o.Owner, true))
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation, true));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
					code.AppendLine(string.Format("\t\t///<param name='Callback'>The function to call when the operation is complete.</param>"));
					code.AppendLine(string.Format("\t\t///<param name='AsyncState'>An object representing the state of the operation.</param>"));
				}
				code.AppendLine(string.Format("\t\t[OperationContract({0}, AsyncPattern = true{1}{2}{3})]", string.Format("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}", o.ProtectionLevel), o.Owner.SessionMode == SessionMode.Required ? o.IsInitiating ? ", IsInitiating = true" : ", IsInitiating = false" : "", o.Owner.SessionMode == SessionMode.Required ? o.IsTerminating ? ", IsTerminating = true" : ", IsTerminating = false" : "", o.IsOneWay ? ", IsOneWay = true" : ""));
				if (!IsCallback && o.IsRESTMethod && (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET45 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35Client || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35))
					code.AppendLine(string.Format("\t\t[System.ServiceModel.Web.{0}(UriTemplate=\"{1}\", {2}BodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.{3}, RequestFormat = System.ServiceModel.Web.WebMessageFormat.{4}, ResponseFormat = System.ServiceModel.Web.WebMessageFormat.{5})]", o.REST.Method == MethodRESTVerbs.GET ? "WebGet" : "WebInvoke", o.REST.UriTemplate, o.REST.Method != MethodRESTVerbs.GET ? string.Format("Method = \"{0}\", ", o.REST.Method) : "", o.REST.BodyStyle, o.REST.RequestFormat, o.REST.ResponseFormat));
				code.AppendFormat("\t\tIAsyncResult Begin{0}(", o.ServerName);
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
				code.AppendFormat("\t\t{0} End{1}(IAsyncResult result);{2}", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName, Environment.NewLine);
			}
			else
				code.Append(GenerateServiceInterfaceSyncMethod(o, IsCallback));

			return code.ToString();
		}

		public static string GenerateServiceInterfaceMethodCode45(Method o, bool IsCallback)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.UseAwaitPattern && CanGenerateAsync(o.Owner, true))
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				code.AppendLine(string.Format("\t\t[OperationContract({0}{1}{2}{3})]", string.Format("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}", o.ProtectionLevel), o.Owner.SessionMode == SessionMode.Required ? o.IsInitiating ? ", IsInitiating = true" : ", IsInitiating = false" : "", o.Owner.SessionMode == SessionMode.Required ? o.IsTerminating ? ", IsTerminating = true" : ", IsTerminating = false" : "", o.IsOneWay ? ", IsOneWay = true" : ""));
				if (!IsCallback && o.IsRESTMethod && (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET45 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35Client || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35))
					code.AppendLine(string.Format("\t\t[System.ServiceModel.Web.{0}(UriTemplate=\"{1}\", {2}BodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.{3}, RequestFormat = System.ServiceModel.Web.WebMessageFormat.{4}, ResponseFormat = System.ServiceModel.Web.WebMessageFormat.{5})]", o.REST.Method == MethodRESTVerbs.GET ? "WebGet" : "WebInvoke", o.REST.UriTemplate, o.REST.Method != MethodRESTVerbs.GET ? string.Format("Method = \"{0}\", ", o.REST.Method) : "", o.REST.BodyStyle, o.REST.RequestFormat, o.REST.ResponseFormat));
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tSystem.Threading.Tasks.Task {0}Async(", o.ServerName);
				else code.AppendFormat("\t\tSystem.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0},", GenerateMethodParameterServerCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 1, 1);
				code.AppendLine(");");
			}
			else
				code.Append(GenerateServiceInterfaceMethodCode40(o, IsCallback));

			return code.ToString();
		}

		#endregion

		#region - Server Proxy Methods -

		public static string GenerateServerProxyPropertyCode(Property o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.AppendFormat("\t\tpublic abstract {0} {1} {{ get; {2}}}",  DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName, o.IsReadOnly ? "" : "set; ");

			return code.ToString();
		}

		public static string GenerateServerProxySyncMethod(Method o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation, true));
				foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
			}
			if (o.HasOperationBehavior) code.AppendLine(string.Format("\t\t[OperationBehavior({0}{1}{2}{3}TransactionScopeRequired = {4})]", !o.OBAutoDisposeParameters ? "AutoDisposeParameters = false, " : "", o.OBImpersonation != ImpersonationOption.NotAllowed ? string.Format("Impersonation = ImpersonationOption.{0}, ", o.OBImpersonation) : "", o.OBReleaseInstanceMode != ReleaseInstanceMode.None ? string.Format("ReleaseInstanceMode = ReleaseInstanceMode.{0}, ", o.OBReleaseInstanceMode) : "", !o.OBTransactionAutoComplete ? "TransactionAutoComplete = false, " : "", o.OBTransactionScopeRequired ? "true" : "false"));
			if (o.HasOperationBehavior && o.OBTransactionFlowMode != TransactionFlowMode.None) code.AppendLine(string.Format("\t\t[TransactionFlow(TransactionFlowOption.{0})]", o.OBTransactionFlowMode));
			code.AppendFormat("\t\tpublic abstract {0} {1}(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");
			return code.ToString();
		}

		public static string GenerateServerProxyMethodCode40(Method o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.UseAsyncPattern && CanGenerateAsync(o.Owner, true))
			{
				//Generate the interface functions.
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation, true));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
					code.AppendLine(string.Format("\t\t///<param name='Callback'>The function to call when the operation is complete.</param>"));
					code.AppendLine(string.Format("\t\t///<param name='AsyncState'>An object representing the state of the operation.</param>"));
				}
				if (o.HasOperationBehavior) code.AppendLine(string.Format("\t\t[OperationBehavior({0}{1}{2}{3}TransactionScopeRequired = {4})]", !o.OBAutoDisposeParameters ? "AutoDisposeParameters = false, " : "", o.OBImpersonation != ImpersonationOption.NotAllowed ? string.Format("Impersonation = ImpersonationOption.{0}, ", o.OBImpersonation) : "", o.OBReleaseInstanceMode != ReleaseInstanceMode.None ? string.Format("ReleaseInstanceMode = ReleaseInstanceMode.{0}, ", o.OBReleaseInstanceMode) : "", !o.OBTransactionAutoComplete ? "TransactionAutoComplete = false, " : "", o.OBTransactionScopeRequired ? "true" : "false"));
				if (o.HasOperationBehavior && o.OBTransactionFlowMode != TransactionFlowMode.None) code.AppendLine(string.Format("\t\t[TransactionFlow(TransactionFlowOption.{0})]", o.OBTransactionFlowMode));
				code.AppendFormat("\t\tpublic abstract IAsyncResult Begin{0}(", o.HasClientType ? o.ClientName : o.ServerName);
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
				code.AppendLine(string.Format("\t\tpublic abstract {0} End{1}(IAsyncResult result);", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName));
			}
			else
				code.Append(GenerateServerProxySyncMethod(o));

			return code.ToString();
		}

		public static string GenerateServerProxyMethodCode45(Method o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.UseAwaitPattern && CanGenerateAsync(o.Owner, true))
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				if (o.HasOperationBehavior) code.AppendLine(string.Format("\t\t[OperationBehavior({0}{1}{2}{3}TransactionScopeRequired = {4})]", !o.OBAutoDisposeParameters ? "AutoDisposeParameters = false, " : "", o.OBImpersonation != ImpersonationOption.NotAllowed ? string.Format("Impersonation = ImpersonationOption.{0}, ", o.OBImpersonation) : "", o.OBReleaseInstanceMode != ReleaseInstanceMode.None ? string.Format("ReleaseInstanceMode = ReleaseInstanceMode.{0}, ", o.OBReleaseInstanceMode) : "", !o.OBTransactionAutoComplete ? "TransactionAutoComplete = false, " : "", o.OBTransactionScopeRequired ? "true" : "false"));
				if (o.HasOperationBehavior && o.OBTransactionFlowMode != TransactionFlowMode.None) code.AppendLine(string.Format("\t\t[TransactionFlow(TransactionFlowOption.{0})]", o.OBTransactionFlowMode));
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tpublic abstract System.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				else code.AppendFormat("\t\tpublic abstract System.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}{1}", GenerateMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(");");
			}
			else
				code.Append(GenerateServerProxyMethodCode40(o));

			return code.ToString();
		}

		#endregion

		#region - Server Callback Methods -

		public static string GenerateServerCallbackSyncMethodProxy(Method o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
			}
			code.AppendFormat("\t\tpublic {0} {1}(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			code.AppendFormat("\t\t\t{0}__callback.{1}(", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", op.Name);
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");
			code.AppendLine("\t\t}");
			return code.ToString();
		}

		public static string GenerateServerCallbackMethodProxyCode40(Method o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.UseAsyncPattern && CanGenerateAsync(o.Owner, true))
			{
				//Generate the delegate fields for this function
				code.AppendLine(string.Format("\t\tprivate readonly BeginOperationDelegate onBegin{0}Delegate;", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine(string.Format("\t\tprivate readonly EndOperationDelegate onEnd{0}Delegate;", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine(string.Format("\t\tprivate readonly System.Threading.SendOrPostCallback on{0}CompletedDelegate;", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine(string.Format("\t\tpublic Action<{1}System.Exception, bool, object> {0}Completed;", o.HasClientType ? o.ClientName : o.ServerName, o.ReturnType.Primitive == PrimitiveTypes.Void ? "" : string.Format("{0}, ", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType))));

				//Generate the interface functions.
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation, true));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
					code.AppendLine(string.Format("\t\t///<param name='Callback'>The function to call when the operation is complete.</param>"));
					code.AppendLine(string.Format("\t\t///<param name='AsyncState'>An object representing the state of the operation.</param>"));
				}
				code.AppendFormat("\t\tIAsyncResult I{1}Callback.Begin{0}(", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				code.AppendLine("AsyncCallback Callback, object AsyncState)");
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\t{0}__callback.Begin{1}(", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName);
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
				code.AppendLine(string.Format("\t\t{0} I{2}Callback.End{1}(IAsyncResult result)", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\t{0}__callback.End{1}(result);", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t}");

				//Generate the delegate implementation functions.
				code.AppendLine(string.Format("\t\tprivate IAsyncResult OnBegin{0}(object[] Values, AsyncCallback Callback, object AsyncState)", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\treturn ((I{1}Callback)this).Begin{0}(", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("({0})Values[{1}], ", DataTypeGenerator.GenerateType(op.Type), o.Parameters.IndexOf(op));
				code.AppendLine("Callback, AsyncState);");
				code.AppendLine("\t\t}");
				code.AppendLine(string.Format("\t\tprivate object[] OnEnd{0}(IAsyncResult result)", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t{");
				if (o.ReturnType.Primitive == PrimitiveTypes.Void)
				{
					code.AppendLine(string.Format("\t\t\t((I{1}Callback)this).End{0}(result);", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name));
					code.AppendLine("\t\t\treturn null;");
				}
				else
					code.AppendLine(string.Format("\t\t\treturn new object[] {{ ((I{1}Callback)this).End{0}(result) }};", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name));
				code.AppendLine("\t\t}");
				code.AppendLine(string.Format("\t\tprivate void On{0}Completed(object state)", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\tif (this.{0}Completed == null) return;", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t\tInvokeAsyncCompletedEventArgs e = (InvokeAsyncCompletedEventArgs)state;");
				code.AppendLine(string.Format("\t\t\tthis.{0}Completed({1}e.Error, e.Cancelled, e.UserState);", o.HasClientType ? o.ClientName : o.ServerName, o.ReturnType.Primitive == PrimitiveTypes.Void ? "" : string.Format("({0})e.Results[0], ", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType))));
				code.AppendLine("\t\t}");

				//Generate invocation functions
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
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
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
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
			else
				code.Append(GenerateServerCallbackSyncMethodProxy(o));

			return code.ToString();
		}

		public static string GenerateServerCallbackMethodProxyCode45(Method o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.UseAwaitPattern && CanGenerateAsync(o.Owner, true))
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tpublic System.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				else code.AppendFormat("\t\tpublic System.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}{1}", GenerateMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\treturn __callback.{0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}{1}", op.Name, o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(");");
				code.AppendLine("\t\t}");
			}
			else
				code.Append(GenerateServerCallbackMethodProxyCode40(o));

			return code.ToString();
		}

		#endregion

		#region - Client Interface Methods -

		public static string GenerateClientInterfaceSyncMethod(Method o, bool IsCallback)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
			}
			code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/I{1}/{2}\")]" : "\t\t[OperationContract(Action = \"{0}/I{1}/{2}\", ReplyAction = \"{0}/I{1}/{2}Response\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
			code.AppendFormat("\t\t{0} {1}(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}{1}", GenerateMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
			code.AppendLine(");");
			return code.ToString();
		}

		public static string GenerateClientInterfaceMethodCode40(Method o, bool IsCallback)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.UseSyncPattern || o.IsTerminating)
				code.Append(GenerateClientInterfaceSyncMethod(o, IsCallback));

			if (o.UseAsyncPattern && CanGenerateAsync(o.Owner, false) && !o.IsTerminating)
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation, true));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
					code.AppendLine(string.Format("\t\t///<param name='Callback'>The function to call when the operation is complete.</param>"));
					code.AppendLine(string.Format("\t\t///<param name='AsyncState'>An object representing the state of the operation.</param>"));
				}
				code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/I{1}/{2}\")]" : "\t\t[OperationContract(Action = \"{0}/I{1}/{2}\", ReplyAction = \"{0}/I{1}/{2}Response\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
				code.AppendFormat("\t\tIAsyncResult Begin{0}(", o.HasClientType ? o.ClientName : o.ServerName);
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
				code.AppendFormat("\t\t{0} End{1}(IAsyncResult result);{2}", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName, Environment.NewLine);
			}

			return code.ToString();
		}

		public static string GenerateClientInterfaceMethodCode45(Method o, bool IsCallback)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			code.Append(GenerateClientInterfaceMethodCode40(o, IsCallback));

			if (o.UseAwaitPattern && CanGenerateAsync(o.Owner, false) && !o.IsTerminating)
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/I{1}/{2}\")]" : "\t\t[OperationContract(Action = \"{0}/I{1}/{2}\", ReplyAction = \"{0}/I{1}/{2}Response\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tSystem.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				else code.AppendFormat("\t\tSystem.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}{1}", GenerateMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(");");
			}

			return code.ToString();
		}

		#endregion

		#region - Client Proxy Methods -

		public static string GenerateMethodProxyConstructorCode(Method o, bool IsServer)
		{
			if (o.IsHidden) return "";
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

		public static string GenerateSyncMethodProxy(Method o, bool IsCallback)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
			}
			code.AppendFormat("\t\tpublic {0} {1}(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			if (o.IsInitiating) code.AppendLine("\t\t\tbase.Initialize();");
			code.AppendFormat("\t\t\t{0}{2}.{1}(", o.ReturnType.Primitive != PrimitiveTypes.Void && !o.IsTerminating ? "return " : o.ReturnType.Primitive != PrimitiveTypes.Void && o.IsTerminating ? "var __t = " : "", o.HasClientType ? o.ClientName : o.ServerName, IsCallback ? "__callback" : "base.Channel");
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", op.Name);
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");
			if (o.IsTerminating) code.AppendLine("\t\t\tbase.Terminate();");
			if (o.IsTerminating && o.ReturnType.Primitive != PrimitiveTypes.Void) code.AppendLine("\t\t\treturn __t;");
			code.AppendLine("\t\t}");
			return code.ToString();
		}

		#region - Obsolete .NET 3.x Code -

		//public static string GenerateMethodProxyCode30(Method o, bool IsServer, bool IsCallback)
		//{
		//	var code = new StringBuilder();

		//	if (o.UseSyncPattern) code.Append(GenerateSyncMethodProxy(o, IsCallback));

		//	if (o.UseAsyncPattern && CanGenerateAsync(o.Owner, IsServer))
		//	{
		//		//Generate the delegate fields for this function
		//		code.AppendLine(string.Format("\t\tprivate readonly BeginOperationDelegate onBegin{0}Delegate;", o.HasClientType ? o.ClientName : o.ServerName));
		//		code.AppendLine(string.Format("\t\tprivate readonly EndOperationDelegate onEnd{0}Delegate;", o.HasClientType ? o.ClientName : o.ServerName));
		//		code.AppendLine(string.Format("\t\tprivate readonly System.Threading.SendOrPostCallback on{0}CompletedDelegate;", o.HasClientType ? o.ClientName : o.ServerName));
		//		code.AppendLine(string.Format("\t\tpublic Action<AsyncOperationCompletedArgs<{1}>> {0}Completed;", o.HasClientType ? o.ClientName : o.ServerName, o.ReturnType.Primitive == PrimitiveTypes.Void ? "object" : o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType)));

		//		//Generate the interface functions.
		//		if (o.Documentation != null)
		//		{
		//			code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation, true));
		//			foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
		//				code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
		//			code.AppendLine(string.Format("\t\t///<param name='Callback'>The function to call when the operation is complete.</param>"));
		//			code.AppendLine(string.Format("\t\t///<param name='AsyncState'>An object representing the state of the operation.</param>"));
		//		}
		//		code.AppendFormat("\t\tIAsyncResult I{1}{2}.Begin{0}Invoke(", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : "");
		//		foreach (MethodParameter op in o.Parameters)
		//			code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
		//		code.AppendLine("AsyncCallback Callback, object AsyncState)");
		//		code.AppendLine("\t\t{");
		//		code.AppendFormat("\t\t\t{0}{2}.Begin{1}Invoke(", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName, IsCallback ? "__callback" : "base.Channel");
		//		foreach (MethodParameter op in o.Parameters)
		//			code.AppendFormat("{0}, ", op.Name);
		//		code.AppendLine("Callback, AsyncState);");
		//		code.AppendLine("\t\t}");
		//		if (o.Documentation != null)
		//		{
		//			code.AppendLine("\t\t///<summary>Finalizes the asynchronous operation.</summary>");
		//			code.AppendLine("\t\t///<returns>");
		//			code.AppendLine(string.Format("\t\t///{0}", o.Documentation.Returns.Replace(Environment.NewLine, Environment.NewLine + "\t///")));
		//			code.AppendLine("\t\t///</returns>");
		//			code.AppendLine(string.Format("\t\t///<param name='result'>The result of the operation.</param>"));
		//		}
		//		code.AppendLine(string.Format("\t\t{0} I{2}{3}.End{1}Invoke(IAsyncResult result)", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : ""));
		//		code.AppendLine("\t\t{");
		//		code.AppendLine(string.Format("\t\t\t{0}{2}.End{1}Invoke(result);", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName, IsCallback ? "__callback" : "base.Channel"));
		//		code.AppendLine("\t\t}");

		//		//Generate the delegate implementation functions.
		//		code.AppendLine(string.Format("\t\tprivate IAsyncResult OnBegin{0}(object[] Values, AsyncCallback Callback, object AsyncState)", o.HasClientType ? o.ClientName : o.ServerName));
		//		code.AppendLine("\t\t{");
		//		code.AppendFormat("\t\t\treturn ((I{1}{2})this).Begin{0}Invoke(", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : "");
		//		foreach (MethodParameter op in o.Parameters)
		//			code.AppendFormat("({0})Values[{1}], ", DataTypeGenerator.GenerateType(op.Type), o.Parameters.IndexOf(op));
		//		code.AppendLine("Callback, AsyncState);");
		//		code.AppendLine("\t\t}");
		//		code.AppendLine(string.Format("\t\tprivate object[] OnEnd{0}(IAsyncResult result)", o.HasClientType ? o.ClientName : o.ServerName));
		//		code.AppendLine("\t\t{");
		//		if (o.ReturnType.Primitive == PrimitiveTypes.Void)
		//		{
		//			code.AppendLine(string.Format("\t\t\t((I{1}{2})this).End{0}Invoke(result);", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : ""));
		//			code.AppendLine("\t\t\treturn null;");
		//		}
		//		else
		//			code.AppendLine(string.Format("\t\t\treturn new object[] {{ ((I{1}{2})this).End{0}Invoke(result) }};", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : ""));
		//		code.AppendLine("\t\t}");
		//		code.AppendLine(string.Format("\t\tprivate void On{0}Completed(object state)", o.HasClientType ? o.ClientName : o.ServerName));
		//		code.AppendLine("\t\t{");
		//		code.AppendLine(string.Format("\t\t\tif (this.{0}Completed == null) return;", o.HasClientType ? o.ClientName : o.ServerName));
		//		code.AppendLine("\t\t\tInvokeAsyncCompletedEventArgs e = (InvokeAsyncCompletedEventArgs)state;");
		//		code.AppendLine(string.Format("\t\t\tthis.{0}Completed(new AsyncOperationCompletedArgs<{2}>({1}e.Error, e.Cancelled, e.UserState));", o.HasClientType ? o.ClientName : o.ServerName, o.ReturnType.Primitive == PrimitiveTypes.Void ? "null" : string.Format("({0})e.Results[0], ", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType)), o.ReturnType.Primitive == PrimitiveTypes.Void ? "object" : o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType)));
		//		code.AppendLine("\t\t}");

		//		//Generate invocation functions
		//		if (o.Documentation != null)
		//		{
		//			code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
		//			foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
		//				code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
		//		}
		//		code.AppendFormat("\t\tpublic void {0}Invoke(", o.HasClientType ? o.ClientName : o.ServerName);
		//		foreach (MethodParameter op in o.Parameters)
		//			code.AppendFormat("{0}{1}", GenerateMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
		//		code.AppendLine(")");
		//		code.AppendLine("\t\t{");
		//		code.AppendFormat("\t\t\tthis.{0}Invoke(", o.HasClientType ? o.ClientName : o.ServerName);
		//		foreach (MethodParameter op in o.Parameters)
		//			code.AppendFormat("{0}, ", op.Name);
		//		code.AppendLine("null);");
		//		code.AppendLine("\t\t}");
		//		if (o.Documentation != null)
		//		{
		//			code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
		//			foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
		//				code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
		//			code.AppendLine(string.Format("\t\t///<param name='userState'>Allows the user of this function to distinguish between different calls.</param>"));
		//		}
		//		code.AppendFormat("\t\tpublic void {0}Invoke(", o.HasClientType ? o.ClientName : o.ServerName);
		//		foreach (MethodParameter op in o.Parameters)
		//			code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
		//		code.AppendLine("object userState)");
		//		code.AppendLine("\t\t{");
		//		code.AppendFormat("\t\t\tInvokeAsync(this.onBegin{0}Delegate, new object[] {{ ", o.HasClientType ? o.ClientName : o.ServerName);
		//		foreach (MethodParameter op in o.Parameters)
		//			code.AppendFormat("{0}{1}", op.Name, o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
		//		code.AppendLine(string.Format(" }}, this.onEnd{0}Delegate, this.on{0}CompletedDelegate, userState);", o.HasClientType ? o.ClientName : o.ServerName));
		//		code.AppendLine("\t\t}");
		//	}
		//	else if (o.UseAsyncPattern && !CanGenerateAsync(o.Owner, IsServer))
		//		code.Append(GenerateSyncMethodProxy(o, IsCallback, true));

		//	return code.ToString();
		//}

		//public static string GenerateMethodProxyCode35(Method o, bool IsServer, bool IsCallback)
		//{
		//	return GenerateMethodProxyCode40(o, IsServer, IsCallback);
		//}

		#endregion

		public static string GenerateMethodProxyCode40(Method o, bool IsServer, bool IsCallback)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.UseSyncPattern || o.IsTerminating)
				code.Append(GenerateSyncMethodProxy(o, IsCallback));

			if (o.UseAsyncPattern && CanGenerateAsync(o.Owner, IsServer) && !o.IsTerminating)
			{
				//Generate the delegate fields for this function
				code.AppendLine(string.Format("\t\tprivate readonly BeginOperationDelegate onBegin{0}Delegate;", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine(string.Format("\t\tprivate readonly EndOperationDelegate onEnd{0}Delegate;", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine(string.Format("\t\tprivate readonly System.Threading.SendOrPostCallback on{0}CompletedDelegate;", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine(string.Format("\t\tpublic Action<{1}System.Exception, bool, object> {0}Completed;", o.HasClientType ? o.ClientName : o.ServerName, o.ReturnType.Primitive == PrimitiveTypes.Void ? "" : string.Format("{0}, ", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType))));

				//Generate the interface functions.
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation, true));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
					code.AppendLine(string.Format("\t\t///<param name='Callback'>The function to call when the operation is complete.</param>"));
					code.AppendLine(string.Format("\t\t///<param name='AsyncState'>An object representing the state of the operation.</param>"));
				}
				code.AppendFormat("\t\tIAsyncResult I{1}{2}.Begin{0}(", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : "");
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				code.AppendLine("AsyncCallback Callback, object AsyncState)");
				code.AppendLine("\t\t{");
				if (o.IsInitiating) code.AppendLine("\t\t\tbase.Initialize();");
				code.AppendFormat("\t\t\t{0}{2}.Begin{1}(", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName, IsCallback ? "__callback" : "base.Channel");
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
				code.AppendLine(string.Format("\t\t{0} I{2}{3}.End{1}(IAsyncResult result)", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : ""));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\t{0}{2}.End{1}(result);", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName, IsCallback ? "__callback" : "base.Channel"));
				if (o.IsTerminating) code.AppendLine("\t\t\tbase.Terminate();");
				code.AppendLine("\t\t}");

				//Generate the delegate implementation functions.
				code.AppendLine(string.Format("\t\tprivate IAsyncResult OnBegin{0}(object[] Values, AsyncCallback Callback, object AsyncState)", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\treturn ((I{1}{2})this).Begin{0}(", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : "");
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("({0})Values[{1}], ", DataTypeGenerator.GenerateType(op.Type), o.Parameters.IndexOf(op));
				code.AppendLine("Callback, AsyncState);");
				code.AppendLine("\t\t}");
				code.AppendLine(string.Format("\t\tprivate object[] OnEnd{0}(IAsyncResult result)", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t{");
				if (o.ReturnType.Primitive == PrimitiveTypes.Void)
				{
					code.AppendLine(string.Format("\t\t\t((I{1}{2})this).End{0}(result);", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : ""));
					code.AppendLine("\t\t\treturn null;");
				}
				else
					code.AppendLine(string.Format("\t\t\treturn new object[] {{ ((I{1}{2})this).End{0}(result) }};", o.HasClientType ? o.ClientName : o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, IsCallback ? "Callback" : ""));
				code.AppendLine("\t\t}");
				code.AppendLine(string.Format("\t\tprivate void On{0}Completed(object state)", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\tif (this.{0}Completed == null) return;", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t\tInvokeAsyncCompletedEventArgs e = (InvokeAsyncCompletedEventArgs)state;");
				code.AppendLine(string.Format("\t\t\tthis.{0}Completed({1}e.Error, e.Cancelled, e.UserState);", o.HasClientType ? o.ClientName : o.ServerName, o.ReturnType.Primitive == PrimitiveTypes.Void ? "" : string.Format("({0})e.Results[0], ", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType))));
				code.AppendLine("\t\t}");

				//Generate invocation functions
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
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
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
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

			return code.ToString();
		}

		public static string GenerateMethodProxyCode45(Method o, bool IsServer, bool IsCallback)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			code.Append(GenerateMethodProxyCode40(o, IsServer, IsCallback));
			
			if (o.UseAwaitPattern && CanGenerateAsync(o.Owner, IsServer) && !o.IsTerminating)
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tpublic System.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				else code.AppendFormat("\t\tpublic System.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}{1}", GenerateMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (o.IsInitiating) code.AppendLine("\t\t\tbase.Initialize();");
				code.AppendFormat("\t\t\treturn {1}.{0}Async(", o.HasClientType ? o.ClientName : o.ServerName, IsCallback ? "__callback" : "base.Channel");
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}{1}", op.Name, o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(");");
				code.AppendLine("\t\t}");
			}

			return code.ToString();
		}

		#endregion

		#region - Client Callback Methods -

		public static string GenerateClientCallbackSyncMethod(Method o, bool IsAsync = false, bool IsAwait = false)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
			}
			code.AppendFormat("\t\tpublic abstract {0} {1}{2}{3}(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName, IsAsync ? "Invoke" : "", IsAwait ? "Async" : "");
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}{1}", GenerateMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
			code.AppendLine(");");
			return code.ToString();
		}

		public static string GenerateCallbackClientMethodCode40(Method o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if(o.UseSyncPattern)
				code.Append(GenerateClientCallbackSyncMethod(o));

			if (o.UseAsyncPattern && CanGenerateAsync(o.Owner, false))
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation, true));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
					code.AppendLine(
						string.Format("\t\t///<param name='Callback'>The function to call when the operation is complete.</param>"));
					code.AppendLine(
						string.Format("\t\t///<param name='AsyncState'>An object representing the state of the operation.</param>"));
				}
				code.AppendLine(
					string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/I{1}/{2}Invoke\")]" : "\t\t[OperationContract(Action = \"{0}/I{1}/{2}Invoke\", ReplyAction = \"{0}/I{1}/{2}InvokeResponse\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
				code.AppendFormat("\t\tpublic abstract IAsyncResult Begin{0}Invoke(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				code.AppendLine("AsyncCallback Callback, object AsyncState);");
				if (o.Documentation != null)
				{
					code.AppendLine("\t\t///<summary>Finalizes the asynchronous operation.</summary>");
					code.AppendLine("\t\t///<returns>");
					code.AppendLine(string.Format("\t\t///{0}",
					                              o.Documentation.Returns.Replace(Environment.NewLine, Environment.NewLine + "\t///")));
					code.AppendLine("\t\t///</returns>");
					code.AppendLine(string.Format("\t\t///<param name='result'>The result of the operation.</param>"));
				}
				code.AppendFormat("\t\tpublic abstract {0} End{1}Invoke(IAsyncResult result);{2}", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName, Environment.NewLine);
			}

			return code.ToString();
		}

		public static string GenerateCallbackClientMethodCode45(Method o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			code.Append(GenerateCallbackClientMethodCode40(o));

			if (o.UseAwaitPattern && CanGenerateAsync(o.Owner, false))
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (MethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/I{1}/{2}Async\")]" : "\t\t[OperationContract(Action = \"{0}/I{1}/{2}Async\", ReplyAction = \"{0}/I{1}/{2}AsyncResponse\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tpublic abstract System.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				else code.AppendFormat("\t\tpublic abstract System.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}{1}", GenerateMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(");");
			}

			return code.ToString();
		}

		#endregion

		#region - Service DRE -

		public static string GenerateServiceInterfaceDCM40(DataChangeMethod o, bool IsServer)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			var dcmtype = o.ReturnType as Data;
			if (dcmtype == null) dcmtype = Generator.ReferenceRetrieve(o.Owner.Parent.Owner, o.Owner.Parent.Owner.Namespace, o.ReturnType.ID) as Data;
			if (dcmtype == null) return "";

			if (o.GenerateGetFunction)
			{
				if(o.GetDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.GetDocumentation));
					foreach (MethodParameter mp in o.GetParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var x = new Method(string.Format("Get{0}DRE", dcmtype.Name), o.Owner) { Parameters = o.GetParameters, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = o.ReturnType };
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode40(x, false) : GenerateClientInterfaceMethodCode40(x, false));
			}
			if (o.GenerateNewDeleteFunction)
			{
				if (o.NewDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.NewDocumentation));
					foreach (MethodParameter mp in o.NewParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var xp = new ObservableCollection<MethodParameter>(o.NewParameters);
				var x = new Method(string.Format("New{0}DRE", dcmtype.Name), o.Owner) { Parameters = xp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
				xp.Insert(0, new MethodParameter(dcmtype, "DREData", o.Owner, x));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode40(x, false) : GenerateClientInterfaceMethodCode40(x, false));
				if (o.DeleteDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.DeleteDocumentation));
					foreach (MethodParameter mp in o.DeleteParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var yp = new ObservableCollection<MethodParameter>(o.DeleteParameters);
				var y = new Method(string.Format("Delete{0}DRE", dcmtype.Name), o.Owner) { Parameters = yp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
				yp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode40(y, false) : GenerateClientInterfaceMethodCode40(y, false));
			}
			if (o.GenerateOpenCloseFunction)
			{
				if (o.OpenDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.OpenDocumentation));
					foreach (MethodParameter mp in o.OpenParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var xp = new ObservableCollection<MethodParameter>(o.OpenParameters);
				var x = new Method(string.Format("Open{0}DRE", dcmtype.Name), o.Owner) { Parameters = xp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = dcmtype };
				xp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, x));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode40(x, false) : GenerateClientInterfaceMethodCode40(x, false));
				if (o.CloseDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.CloseDocumentation));
					foreach (MethodParameter mp in o.CloseParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var yp = new ObservableCollection<MethodParameter>(o.CloseParameters);
				var y = new Method(string.Format("Close{0}DRE", dcmtype.Name), o.Owner) { Parameters = yp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
				yp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode40(y, false) : GenerateClientInterfaceMethodCode40(y, false));
			}

			foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Immediate))
			{
				DataType edt = de.DataType;

				var tp = new ObservableCollection<MethodParameter>();
				var x = new Method(string.Format("Update{0}{1}DRE", dcmtype.Name, de.DataName), o.Owner) { Parameters = tp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new MethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));

				if (edt.TypeMode == DataTypeMode.Collection) tp.Add(new MethodParameter(new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) {CollectionGenericType = edt.CollectionGenericType}, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Dictionary) tp.Add(new MethodParameter(new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) {DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType}, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Queue) { continue; }
				else if (edt.TypeMode == DataTypeMode.Stack) { continue; }
				else tp.Add(new MethodParameter(edt, "ChangedValue", o.Owner, x));

				code.Append(IsServer ? GenerateServiceInterfaceMethodCode40(x, false) : GenerateClientInterfaceMethodCode40(x, false));
			}

			if (dcmtype.Elements.Any(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch))
			{
				var tp = new ObservableCollection<MethodParameter>();
				var x = new Method(string.Format("BatchUpdate{0}DRE", dcmtype.Name), o.Owner) { Parameters = tp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new MethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode != DataTypeMode.Collection && a.DataType.TypeMode != DataTypeMode.Dictionary))
					tp.Add(new MethodParameter(new DataType("CMDItemValue", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = de.DataType }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Collection))
				{
					DataType edt = de.DataType;
					tp.Add(new MethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = edt.CollectionGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				}
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Dictionary))
				{
					DataType edt = de.DataType;
					tp.Add(new MethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				}


				code.Append(IsServer ? GenerateServiceInterfaceMethodCode40(x, false) : GenerateClientInterfaceMethodCode40(x, false));
			}

			code.AppendLine();

			return code.ToString();
		}

		public static string GenerateServiceInterfaceDCM45(DataChangeMethod o, bool IsServer)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			var dcmtype = o.ReturnType as Data;
			if (dcmtype == null) dcmtype = Generator.ReferenceRetrieve(o.Owner.Parent.Owner, o.Owner.Parent.Owner.Namespace, o.ReturnType.ID) as Data;
			if (dcmtype == null) return "";

			if (o.GenerateGetFunction)
			{
				if (o.GetDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.GetDocumentation));
					foreach (MethodParameter mp in o.GetParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var x = new Method(string.Format("Get{0}DRE", dcmtype.Name), o.Owner) { Parameters = o.GetParameters, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = o.ReturnType };
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(x, false) : GenerateClientInterfaceMethodCode45(x, false));
			}
			if (o.GenerateNewDeleteFunction)
			{
				if (o.NewDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.NewDocumentation));
					foreach (MethodParameter mp in o.NewParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var xp = new ObservableCollection<MethodParameter>(o.NewParameters);
				var x = new Method(string.Format("New{0}DRE", dcmtype.Name), o.Owner) { Parameters = xp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				xp.Insert(0, new MethodParameter(dcmtype, "DREData", o.Owner, x));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(x, false) : GenerateClientInterfaceMethodCode45(x, false));
				if (o.DeleteDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.DeleteDocumentation));
					foreach (MethodParameter mp in o.DeleteParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var yp = new ObservableCollection<MethodParameter>(o.DeleteParameters);
				var y = new Method(string.Format("Delete{0}DRE", dcmtype.Name), o.Owner) { Parameters = yp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				yp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(y, false) : GenerateClientInterfaceMethodCode45(y, false));
			}
			if (o.GenerateOpenCloseFunction)
			{
				if (o.OpenDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.OpenDocumentation));
					foreach (MethodParameter mp in o.OpenParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var xp = new ObservableCollection<MethodParameter>(o.OpenParameters);
				var x = new Method(string.Format("Open{0}DRE", dcmtype.Name), o.Owner) { Parameters = xp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = dcmtype };
				xp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, x));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(x, false) : GenerateClientInterfaceMethodCode45(x, false));
				if (o.CloseDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.CloseDocumentation));
					foreach (MethodParameter mp in o.CloseParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var yp = new ObservableCollection<MethodParameter>(o.CloseParameters);
				var y = new Method(string.Format("Close{0}DRE", dcmtype.Name), o.Owner) { Parameters = yp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				yp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(y, false) : GenerateClientInterfaceMethodCode45(y, false));
			}

			foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Immediate))
			{
				DataType edt = de.DataType;

				var tp = new ObservableCollection<MethodParameter>();
				var x = new Method(string.Format("Update{0}{1}DRE", dcmtype.Name, de.DataName), o.Owner) { Parameters = tp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new MethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));

				if (edt.TypeMode == DataTypeMode.Collection) tp.Add(new MethodParameter(new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = edt.CollectionGenericType }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Dictionary) tp.Add(new MethodParameter(new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Queue) { continue; }
				else if (edt.TypeMode == DataTypeMode.Stack) { continue; }
				else tp.Add(new MethodParameter(edt, "ChangedValue", o.Owner, x));

				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(x, false) : GenerateClientInterfaceMethodCode45(x, false));
			}

			if (dcmtype.Elements.Any(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch))
			{
				var tp = new ObservableCollection<MethodParameter>();
				var x = new Method(string.Format("BatchUpdate{0}DRE", dcmtype.Name), o.Owner) { Parameters = tp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new MethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode != DataTypeMode.Collection && a.DataType.TypeMode != DataTypeMode.Dictionary))
					tp.Add(new MethodParameter(new DataType("CMDItemValue", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = de.DataType }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Collection))
				{
					DataType edt = de.DataType;
					tp.Add(new MethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = edt.CollectionGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				}
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Dictionary))
				{
					DataType edt = de.DataType;
					tp.Add(new MethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				}

				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(x, false) : GenerateClientInterfaceMethodCode45(x, false));
			}

			code.AppendLine();

			return code.ToString();
		}

		public static string GenerateServiceImplementationDCM40(DataChangeMethod o, bool IsServer)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			var dcmtype = o.ReturnType as Data;
			if (dcmtype == null) dcmtype = Generator.ReferenceRetrieve(o.Owner.Parent.Owner, o.Owner.Parent.Owner.Namespace, o.ReturnType.ID) as Data;
			if (dcmtype == null) return "";

			if (IsServer)
			{
				if (o.GenerateGetFunction)
				{
					code.Append(string.Format("\t\tpublic abstract {1} Get{0}DRE(", dcmtype.Name, o.ReturnType.HasClientType ? o.ReturnType.ClientType : o.ReturnType));
					foreach (MethodParameter mp in o.GetParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.GetParameters.IndexOf(mp) < o.GetParameters.Count - 1 ? ", " : ""));
					code.AppendLine(");");
				}
				if (o.GenerateNewDeleteFunction)
				{
					code.Append(string.Format("\t\tpublic virtual void New{0}DRE({1} DREData{2}", dcmtype.Name, DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype), o.NewParameters.Count != 0 ? ", " : ""));
					foreach (MethodParameter mp in o.NewParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.NewParameters.IndexOf(mp) < o.NewParameters.Count - 1 ? ", " : ""));
					code.AppendLine(")");
					code.AppendLine("\t\t{");
					code.AppendLine(string.Format("\t\t\t{0}.RegisterData(ClientID, DREData);", DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)));
					code.AppendLine("\t\t}");
					code.Append(string.Format("\t\tpublic virtual void Delete{0}DRE(Guid DREDataID{1}", dcmtype.Name, o.DeleteParameters.Count != 0 ? ", " : ""));
					foreach (MethodParameter mp in o.DeleteParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.DeleteParameters.IndexOf(mp) < o.DeleteParameters.Count - 1 ? ", " : ""));
					code.AppendLine(")");
					code.AppendLine("\t\t{");
					code.AppendLine(string.Format("\t\t\t{0}.UnregisterData(ClientID, DREDataID);", DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)));
					code.AppendLine("\t\t}");
				}
				if (o.GenerateOpenCloseFunction)
				{
					code.Append(string.Format("\t\tpublic abstract {1} Open{0}DRE(Guid DREDataID{2}", dcmtype.Name, DataTypeGenerator.GenerateType(dcmtype), o.OpenParameters.Count != 0 ? ", " : ""));
					foreach (MethodParameter mp in o.OpenParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.OpenParameters.IndexOf(mp) < o.OpenParameters.Count - 1 ? ", " : ""));
					code.AppendLine(");");
					code.Append(string.Format("\t\tpublic virtual void Close{0}DRE(Guid DREDataID{1}", dcmtype.Name, o.CloseParameters.Count != 0 ? ", " : ""));
					foreach (MethodParameter mp in o.CloseParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.CloseParameters.IndexOf(mp) < o.CloseParameters.Count - 1 ? ", " : ""));
					code.AppendLine(")");
					code.AppendLine("\t\t{");
					code.AppendLine(string.Format("\t\t\treturn DREDataID == Guid.Empty ? null : {0}.RegisterData(ClientID, DREDataID);", DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)));
					code.AppendLine("\t\t}");
				}
			}
			else
			{
				if (o.GenerateGetFunction)
				{
					var x = new Method(string.Format("Get{0}DRE", dcmtype.Name), o.Owner) { Parameters = o.GetParameters, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = o.ReturnType };
					code.Append(GenerateServiceClientMethodDCM40(x, o.UseTPLForCallbacks));
				}
				if (o.GenerateNewDeleteFunction)
				{
					var xp = new ObservableCollection<MethodParameter>(o.NewParameters);
					var x = new Method(string.Format("New{0}DRE", dcmtype.Name), o.Owner) { Parameters = xp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
					xp.Insert(0, new MethodParameter(dcmtype, "DREData", o.Owner, x));
					code.Append(GenerateServiceClientMethodDCM40(x, o.UseTPLForCallbacks));
					var yp = new ObservableCollection<MethodParameter>(o.DeleteParameters);
					var y = new Method(string.Format("Delete{0}DRE", dcmtype.Name), o.Owner) { Parameters = yp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
					yp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
					code.Append(GenerateServiceClientMethodDCM40(y, o.UseTPLForCallbacks));
				}
				if (o.GenerateOpenCloseFunction)
				{
					var xp = new ObservableCollection<MethodParameter>(o.OpenParameters);
					var x = new Method(string.Format("Open{0}DRE", dcmtype.Name), o.Owner) { Parameters = xp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = dcmtype };
					xp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, x));
					code.Append(GenerateServiceClientMethodDCM40(x, o.UseTPLForCallbacks));
					var yp = new ObservableCollection<MethodParameter>(o.CloseParameters);
					var y = new Method(string.Format("Close{0}DRE", dcmtype.Name), o.Owner) { Parameters = yp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
					yp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
					code.Append(GenerateServiceClientMethodDCM40(y, o.UseTPLForCallbacks));
				}
			}

			foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Immediate))
			{
				DataType edt = de.DataType;

				var tp = new ObservableCollection<MethodParameter>();
				var x = new Method(string.Format("Update{0}{1}DRE", dcmtype.Name, de.DataName), o.Owner) { Parameters = tp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new MethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));

				if (edt.TypeMode == DataTypeMode.Collection) tp.Add(new MethodParameter(new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = edt.CollectionGenericType }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Dictionary) tp.Add(new MethodParameter(new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Queue) { continue; }
				else if (edt.TypeMode == DataTypeMode.Stack) { continue; }
				else tp.Add(new MethodParameter(edt, "ChangedValue", o.Owner, x));

				code.Append(IsServer ? GenerateServiceServerImmediateMethodDCM40(x, dcmtype, de.DataName, edt.TypeMode, o.UseTPLForCallbacks) : GenerateServiceClientMethodDCM40(x, o.UseTPLForCallbacks));
			}

			if (dcmtype.Elements.Any(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch))
			{
				var tp = new ObservableCollection<MethodParameter>();
				var x = new Method(string.Format("BatchUpdate{0}DRE", dcmtype.Name), o.Owner) { Parameters = tp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new MethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode != DataTypeMode.Collection && a.DataType.TypeMode != DataTypeMode.Dictionary))
					tp.Add(new MethodParameter(new DataType("CMDItemValue", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = de.DataType }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Collection))
				{
					DataType edt = de.DataType;
					tp.Add(new MethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = edt.CollectionGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				}
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Dictionary))
				{
					DataType edt = de.DataType;
					tp.Add(new MethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				}

				code.Append(IsServer ? GenerateServiceServerBatchMethodDCM40(x, dcmtype, o.UseTPLForCallbacks) : GenerateServiceClientMethodDCM40(x, o.UseTPLForCallbacks, false));
			}

			if (IsServer) code.AppendLine(string.Format("\t\tpublic abstract IEnumerable<Guid> Get{0}Clients(Guid UpdateID);", dcmtype.Name));

			code.AppendLine();

			return code.ToString();
		}

		public static string GenerateServiceServerImmediateMethodDCM40(Method o, DataType DCMType, string ElementName, DataTypeMode TypeMode, bool UseTPL)
		{
			var code = new StringBuilder();
			code.AppendFormat("\t\tpublic virtual void {0}(", o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
			if (TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\tvar temp = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
				code.AppendLine(string.Format("\t\t\tif (temp != null) temp.{0}.ApplyDelta(ChangedItem);", ElementName));
			}
			else if (TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\tvar temp = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
				code.AppendLine(string.Format("\t\t\tif (temp != null) temp.{0}.ApplyDelta(ChangedItem);", ElementName));
			}
			else code.AppendLine(string.Format("\t\t\t{0}.UpdateValueServer(UpdateID, {0}.{1}Property, ChangedValue);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType), ElementName));
			code.Append(string.Format("\t\t\t\t\tCallback{0}(ClientID, ", o.ServerName));
			foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
			code.AppendLine(");");
			if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
			code.AppendLine("\t\t}");

			code.AppendFormat("\t\tpublic static void Callback{0}(Guid SenderID, ", o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
			code.AppendLine(string.Format("\t\t\t\tvar tcl = GetClients<{0}Base>(GetClientMessageList<{1}>(UpdateID)).Where(a => a.ClientID != SenderID);", o.Owner.Name, DataTypeGenerator.GenerateType(DCMType)));
			code.AppendLine(string.Format("\t\t\t\tforeach(var tc in tcl)"));
			code.Append(string.Format("\t\t\t\t\ttc.Callback.{0}Callback(", o.ServerName));
			foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
			code.AppendLine(");");
			if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
			code.AppendLine("\t\t}");
			return code.ToString();
		}

		public static string GenerateServiceServerBatchMethodDCM40(Method o, Data DCMType, bool UseTPL)
		{
			var code = new StringBuilder();
			code.AppendFormat("\t\tpublic virtual void {0}(", o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
			code.AppendLine(string.Format("\t\t\t\tvar t = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
			code.AppendLine(string.Format("\t\t\t\tif (t != null)"));
			code.AppendLine("\t\t\t\t{");
			foreach (var de in o.Parameters.Where(a => a.Type.Name == "CMDItemValue"))
				code.AppendLine(string.Format("\t\t\t\t\tt.ApplyDelta({0});", de.Name));
			foreach (DataElement de in DCMType.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && (a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
				code.AppendLine(string.Format("\t\t\t\t\tt.{0}.ApplyDelta({0}Delta);", de.HasClientType ? de.ClientName : de.DataName));
			code.AppendLine("\t\t\t\t}");
			code.Append(string.Format("\t\t\t\tCallback{0}(ClientID, ", o.ServerName));
			foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
			code.AppendLine(");");
			if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
			code.AppendLine("\t\t}");

			code.AppendFormat("\t\tpublic static void Callback{0}(Guid SenderID, ", o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			if (UseTPL) code.AppendLine("\t\t\t System.Threading.Tasks.Task.Factory.StartNew(() => {");
			code.AppendLine(string.Format("\t\t\t\tvar tcl = GetClients<{0}Base>(GetClientMessageList<{1}>(UpdateID)).Where(a => a.ClientID != SenderID);", o.Owner.Name, DataTypeGenerator.GenerateType(DCMType)));
			code.AppendLine(string.Format("\t\t\t\tforeach(var tc in tcl)"));
			code.Append(string.Format("\t\t\t\t\ttc.Callback.{0}Callback(", o.ServerName));
			foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
			code.AppendLine(");");
			if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
			code.AppendLine("\t\t}");
			return code.ToString();
		}

		public static string GenerateServiceClientMethodDCM40(Method o, bool UseTPL, bool IsImmediate = true)
		{
			var code = new StringBuilder();
			code.AppendFormat("\t\tpublic virtual {1} {0}(", o.HasClientType ? o.ClientName : o.ServerName, DataTypeGenerator.GenerateType(o.ReturnType));
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			if (o.ReturnType.Primitive != PrimitiveTypes.Void)
			{
				if (IsImmediate)
				{
					code.Append(string.Format("\t\t\t{1}Channel.{0}(", o.ServerName, o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : ""));
					foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
					code.AppendLine(");");
				}
				else
				{
					code.Append(string.Format("\t\t\t{1}Channel.{0}(", o.ServerName, o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : ""));
					code.Append(string.Format("{1}\t\t\t{2}Channel.{0}(", o.ServerName, UseTPL ? "\t" : "", o.ReturnType.Primitive != PrimitiveTypes.Void ? "t = " : ""));
					foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
					code.AppendLine(");");
				}
			}
			else
			{
				if (IsImmediate)
				{
					if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
					code.Append(string.Format("{1}\t\t\t{2}Channel.{0}(", o.ServerName, UseTPL ? "\t" : "", o.ReturnType.Primitive != PrimitiveTypes.Void ? "t = " : ""));
					foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
					code.AppendLine(");");
					if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				}
				else
				{
					if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
					code.Append(string.Format("{1}\t\t\t{2}Channel.{0}(", o.ServerName, UseTPL ? "\t" : "", o.ReturnType.Primitive != PrimitiveTypes.Void ? "t = " : ""));
					foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
					code.AppendLine(");");
					if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				}
			}
			code.AppendLine("\t\t}");
			return code.ToString();
		}

		public static string GenerateServiceImplementationDCM45(DataChangeMethod o, bool IsServer)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			var dcmtype = o.ReturnType as Data;
			if (dcmtype == null) dcmtype = Generator.ReferenceRetrieve(o.Owner.Parent.Owner, o.Owner.Parent.Owner.Namespace, o.ReturnType.ID) as Data;
			if (dcmtype == null) return "";
			bool uap = o.UseAwaitPattern && CanGenerateAsync(o.Owner, IsServer);

			if (IsServer)
			{
				if (o.GenerateGetFunction)
				{
					if (uap)
					{
						code.Append(string.Format("\t\tpublic abstract System.Threading.Tasks.Task<{1}> Get{0}DREAsync(", dcmtype.Name, o.ReturnType.HasClientType ? o.ReturnType.ClientType : o.ReturnType));
						foreach (MethodParameter mp in o.GetParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.GetParameters.IndexOf(mp) < o.GetParameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
					}
					if (o.UseSyncPattern || !uap)
					{
						code.Append(string.Format("\t\tpublic abstract {1} Get{0}DRE(", dcmtype.Name, o.ReturnType.HasClientType ? o.ReturnType.ClientType : o.ReturnType));
						foreach (MethodParameter mp in o.GetParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.GetParameters.IndexOf(mp) < o.GetParameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
					}
				}
				if (o.GenerateNewDeleteFunction)
				{
					if (uap)
					{
						code.Append(string.Format("\t\tpublic virtual System.Threading.Tasks.Task New{0}DREAsync({1} DREData{2}", dcmtype.Name, DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype), o.NewParameters.Count != 0 ? ", " : ""));
						foreach (MethodParameter mp in o.NewParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.NewParameters.IndexOf(mp) < o.NewParameters.Count - 1 ? ", " : ""));
						code.AppendLine(")");
						code.AppendLine("\t\t{");
						code.AppendLine(string.Format("\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(() => {{ {0}.RegisterData(ClientID, DREData); }}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);", DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)));
						code.AppendLine("\t\t}");
						code.Append(string.Format("\t\tpublic virtual System.Threading.Tasks.Task Delete{0}DREAsync(Guid DREDataID{1}", dcmtype.Name, o.NewParameters.Count != 0 ? ", " : ""));
						foreach (MethodParameter mp in o.DeleteParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.DeleteParameters.IndexOf(mp) < o.DeleteParameters.Count - 1 ? ", " : ""));
						code.AppendLine(")");
						code.AppendLine("\t\t{");
						code.AppendLine(string.Format("\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(() => {{ {0}.UnregisterData(ClientID, DREDataID); }}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);", DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)));
						code.AppendLine("\t\t}");
					}
					if (o.UseSyncPattern || !uap)
					{
						code.Append(string.Format("\t\tpublic virtual void New{0}DRE({1} DREData{2}", dcmtype.Name, DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype), o.NewParameters.Count != 0 ? ", " : ""));
						foreach (MethodParameter mp in o.NewParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.NewParameters.IndexOf(mp) < o.NewParameters.Count - 1 ? ", " : ""));
						code.AppendLine(")");
						code.AppendLine("\t\t{");
						code.AppendLine(string.Format("\t\t\t{0}.RegisterData(ClientID, DREData);", DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)));
						code.AppendLine("\t\t}");
						code.Append(string.Format("\t\tpublic virtual void Delete{0}DRE(Guid DREDataID{1}", dcmtype.Name, o.DeleteParameters.Count != 0 ? ", " : ""));
						foreach (MethodParameter mp in o.DeleteParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.DeleteParameters.IndexOf(mp) < o.DeleteParameters.Count - 1 ? ", " : ""));
						code.AppendLine(")");
						code.AppendLine("\t\t{");
						code.AppendLine(string.Format("\t\t\t{0}.UnregisterData(ClientID, DREDataID);", DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)));
						code.AppendLine("\t\t}");
					}
				}
				if (o.GenerateOpenCloseFunction)
				{
					if (uap)
					{
						code.Append(string.Format("\t\tpublic abstract System.Threading.Tasks.Task<{1}> Open{0}DREAsync(Guid DREDataID{2}", dcmtype.Name, DataTypeGenerator.GenerateType(dcmtype), o.OpenParameters.Count != 0 ? ", " : ""));
						foreach (MethodParameter mp in o.OpenParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.OpenParameters.IndexOf(mp) < o.OpenParameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
						code.Append(string.Format("\t\tpublic virtual System.Threading.Tasks.Task Close{0}DREAsync(Guid DREDataID{1}", dcmtype.Name, o.CloseParameters.Count != 0 ? ", " : ""));
						foreach (MethodParameter mp in o.CloseParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.CloseParameters.IndexOf(mp) < o.CloseParameters.Count - 1 ? ", " : ""));
						code.AppendLine(")");
						code.AppendLine("\t\t{");
						code.AppendLine(string.Format("\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(() => {{ {0}.UnregisterData(ClientID, DREDataID); }}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);", DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)));
						code.AppendLine("\t\t}");
					}
					if (o.UseSyncPattern || !uap)
					{
						code.Append(string.Format("\t\tpublic abstract {1} Open{0}DRE(Guid DREDataID{2}", dcmtype.Name, DataTypeGenerator.GenerateType(dcmtype), o.OpenParameters.Count != 0 ? ", " : ""));
						foreach (MethodParameter mp in o.OpenParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.OpenParameters.IndexOf(mp) < o.OpenParameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
						code.Append(string.Format("\t\tpublic virtual void Close{0}DRE(Guid DREDataID{1}", dcmtype.Name, o.CloseParameters.Count != 0 ? ", " : ""));
						foreach (MethodParameter mp in o.CloseParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.CloseParameters.IndexOf(mp) < o.CloseParameters.Count - 1 ? ", " : ""));
						code.AppendLine(")");
						code.AppendLine("\t\t{");
						code.AppendLine(string.Format("\t\t\t{0}.UnregisterData(ClientID, DREDataID);", DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)));
						code.AppendLine("\t\t}");
					}
				}
			}
			else
			{
				if (o.GenerateGetFunction)
				{
					var x = new Method(string.Format("Get{0}DRE", dcmtype.Name), o.Owner) { Parameters = o.GetParameters, UseSyncPattern = o.UseSyncPattern || !uap, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = o.ReturnType };
					code.Append(GenerateServiceClientMethodDCM45(x, o.UseTPLForCallbacks));
				}
				if (o.GenerateNewDeleteFunction)
				{
					var xp = new ObservableCollection<MethodParameter>(o.NewParameters);
					var x = new Method(string.Format("New{0}DRE", dcmtype.Name), o.Owner) { Parameters = xp, UseSyncPattern = o.UseSyncPattern || !uap, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
					xp.Insert(0, new MethodParameter(dcmtype, "DREData", o.Owner, x));
					code.Append(GenerateServiceClientMethodDCM45(x, o.UseTPLForCallbacks));
					var yp = new ObservableCollection<MethodParameter>(o.DeleteParameters);
					var y = new Method(string.Format("Delete{0}DRE", dcmtype.Name), o.Owner) { Parameters = yp, UseSyncPattern = o.UseSyncPattern || !uap, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
					yp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
					code.Append(GenerateServiceClientMethodDCM45(y, o.UseTPLForCallbacks));
				}
				if (o.GenerateOpenCloseFunction)
				{
					var xp = new ObservableCollection<MethodParameter>(o.OpenParameters);
					var x = new Method(string.Format("Open{0}DRE", dcmtype.Name), o.Owner) { Parameters = xp, UseSyncPattern = o.UseSyncPattern || !uap, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = dcmtype };
					xp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, x));
					code.Append(GenerateServiceClientMethodDCM45(x, o.UseTPLForCallbacks));
					var yp = new ObservableCollection<MethodParameter>(o.CloseParameters);
					var y = new Method(string.Format("Close{0}DRE", dcmtype.Name), o.Owner) { Parameters = yp, UseSyncPattern = o.UseSyncPattern || !uap, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
					yp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
					code.Append(GenerateServiceClientMethodDCM45(y, o.UseTPLForCallbacks));
				}
			}

			foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Immediate))
			{
				DataType edt = de.DataType;

				var tp = new ObservableCollection<MethodParameter>();
				var x = new Method(string.Format("Update{0}{1}DRE", dcmtype.Name, de.DataName), o.Owner) { Parameters = tp, UseSyncPattern = o.UseSyncPattern || !uap, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new MethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));

				if (edt.TypeMode == DataTypeMode.Collection) tp.Add(new MethodParameter(new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = edt.CollectionGenericType }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Dictionary) tp.Add(new MethodParameter(new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Queue) { continue; }
				else if (edt.TypeMode == DataTypeMode.Stack) { continue; }
				else tp.Add(new MethodParameter(edt, "ChangedValue", o.Owner, x));

				code.Append(IsServer ? GenerateServiceServerImmediateMethodDCM45(x, dcmtype, de.DataName, edt.TypeMode, o.UseTPLForCallbacks) : GenerateServiceClientMethodDCM45(x, o.UseTPLForCallbacks));
			}

			if (dcmtype.Elements.Any(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch))
			{
				var tp = new ObservableCollection<MethodParameter>();
				var x = new Method(string.Format("BatchUpdate{0}DRE", dcmtype.Name), o.Owner) { Parameters = tp, UseSyncPattern = o.UseSyncPattern || !uap, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new MethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode != DataTypeMode.Collection && a.DataType.TypeMode != DataTypeMode.Dictionary))
					tp.Add(new MethodParameter(new DataType("CMDItemValue", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = de.DataType }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Collection))
				{
					DataType edt = de.DataType;
					tp.Add(new MethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) {CollectionGenericType = new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = edt.CollectionGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				}
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Dictionary))
				{
					DataType edt = de.DataType;
					tp.Add(new MethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				}

				code.Append(IsServer ? GenerateServiceServerBatchMethodDCM45(x, dcmtype, o.UseTPLForCallbacks) : GenerateServiceClientMethodDCM45(x, o.UseTPLForCallbacks, false));
			}

			if (IsServer) code.AppendLine(string.Format("\t\tpublic abstract IEnumerable<Guid> Get{0}Clients(Guid UpdateID);", dcmtype.Name));

			code.AppendLine();

			return code.ToString();
		}

		public static string GenerateServiceServerImmediateMethodDCM45(Method o, DataType DCMType, string ElementName, DataTypeMode TypeMode, bool UseTPL)
		{
			var code = new StringBuilder();
			if (o.UseAwaitPattern && CanGenerateAsync(o.Owner, true))
			{
				code.AppendFormat("\t\tpublic virtual System.Threading.Tasks.Task {0}Async(", o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(async() => {");
				if (TypeMode == DataTypeMode.Collection)
				{
					code.AppendLine(string.Format("\t\t\tvar temp = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
					code.AppendLine(string.Format("\t\t\tif (temp != null) temp.{0}.ApplyDelta(ChangedItem);", ElementName));
				}
				else if (TypeMode == DataTypeMode.Dictionary)
				{
					code.AppendLine(string.Format("\t\t\tvar temp = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
					code.AppendLine(string.Format("\t\t\tif (temp != null) temp.{0}.ApplyDelta(ChangedItem);", ElementName));
				}
				else code.AppendLine(string.Format("\t\t\t{0}.UpdateValueServer(UpdateID, {0}.{1}Property, ChangedValue);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType), ElementName));
				code.Append(string.Format("\t\t\t\t\tawait Callback{0}Async(ClientID, ", o.ServerName));
				foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
				code.AppendLine(");");
				code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");

				code.AppendFormat("\t\tpublic static System.Threading.Tasks.Task Callback{0}Async(Guid SenderID, ", o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(async() => {");
				code.AppendLine(string.Format("\t\t\t\tvar tcl = GetClients<{0}Base>(GetClientMessageList<{1}>(UpdateID)).Where(a => a.ClientID != SenderID);", o.Owner.Name, DataTypeGenerator.GenerateType(DCMType)));
				code.AppendLine(string.Format("\t\t\t\tforeach(var tc in tcl)"));
				code.Append(string.Format("\t\t\t\t\tawait tc.Callback.{0}CallbackAsync(", o.ServerName));
				foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
				code.AppendLine(");");
				code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");
			}
			if (o.UseSyncPattern)
			{
				code.AppendFormat("\t\tpublic virtual void {0}(", o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
				if (TypeMode == DataTypeMode.Collection)
				{
					code.AppendLine(string.Format("\t\t\tvar temp = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
					code.AppendLine(string.Format("\t\t\tif (temp != null) temp.{0}.ApplyDelta(ChangedItem);", ElementName));
				}
				else if (TypeMode == DataTypeMode.Dictionary)
				{
					code.AppendLine(string.Format("\t\t\tvar temp = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
					code.AppendLine(string.Format("\t\t\tif (temp != null) temp.{0}.ApplyDelta(ChangedItem);", ElementName));
				}
				else code.AppendLine(string.Format("\t\t\t{0}.UpdateValueServer(UpdateID, {0}.{1}Property, ChangedValue);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType), ElementName));
				code.Append(string.Format("\t\t\t\t\tCallback{0}(ClientID, ", o.ServerName));
				foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
				code.AppendLine(");");
				if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");

				code.AppendFormat("\t\tpublic static void Callback{0}(Guid SenderID, ", o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
				code.AppendLine(string.Format("\t\t\t\tvar tcl = GetClients<{0}Base>(GetClientMessageList<{1}>(UpdateID)).Where(a => a.ClientID != SenderID);", o.Owner.Name, DataTypeGenerator.GenerateType(DCMType)));
				code.AppendLine(string.Format("\t\t\t\tforeach(var tc in tcl)"));
				code.Append(string.Format("\t\t\t\t\ttc.Callback.{0}Callback(", o.ServerName));
				foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
				code.AppendLine(");");
				if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");
			}
			return code.ToString();
		}

		public static string GenerateServiceServerBatchMethodDCM45(Method o, Data DCMType, bool UseTPL)
		{
			var code = new StringBuilder();
			if (o.UseAwaitPattern && CanGenerateAsync(o.Owner, true))
			{
				code.AppendFormat("\t\tpublic virtual System.Threading.Tasks.Task {0}Async(", o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(async() => {");
				code.AppendLine(string.Format("\t\t\t\tvar t = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
				code.AppendLine(string.Format("\t\t\t\tif (t != null)"));
				code.AppendLine("\t\t\t\t{");
				foreach (var de in o.Parameters.Where(a => a.Type.Name == "CMDItemValue"))
					code.AppendLine(string.Format("\t\t\t\t\tt.ApplyDelta({0});", de.Name));
				foreach (DataElement de in DCMType.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && (a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
					code.AppendLine(string.Format("\t\t\t\t\tt.{0}.ApplyDelta({0}Delta);", de.HasClientType ? de.ClientName : de.DataName));
				code.AppendLine("\t\t\t\t}");
				code.Append(string.Format("\t\t\t\tCallback{0}Async(ClientID, ", o.ServerName));
				foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
				code.AppendLine(");");
				code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");

				code.AppendFormat("\t\tpublic static void Callback{0}Async(Guid SenderID, ", o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(async() => {");
				code.AppendLine(string.Format("\t\t\t\tvar tcl = GetClients<{0}Base>(GetClientMessageList<{1}>(UpdateID)).Where(a => a.ClientID != SenderID);", o.Owner.Name, DataTypeGenerator.GenerateType(DCMType)));
				code.AppendLine(string.Format("\t\t\t\tforeach(var tc in tcl)"));
				code.Append(string.Format("\t\t\t\t\tawait tc.Callback.{0}CallbackAsync(", o.ServerName));
				foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
				code.AppendLine(");");
				code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");
			}
			if (o.UseSyncPattern)
			{
				code.AppendFormat("\t\tpublic virtual void {0}(", o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
				code.AppendLine(string.Format("\t\t\t\tvar t = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
				code.AppendLine(string.Format("\t\t\t\tif (t != null)"));
				code.AppendLine("\t\t\t\t{");
				foreach (var de in o.Parameters.Where(a => a.Type.Name == "CMDItemValue"))
					code.AppendLine(string.Format("\t\t\t\t\tt.ApplyDelta({0});", de.Name));
				foreach (DataElement de in DCMType.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && (a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
					code.AppendLine(string.Format("\t\t\t\t\tt.{0}.ApplyDelta({0}Delta);", de.HasClientType ? de.ClientName : de.DataName));
				code.AppendLine("\t\t\t\t}");
				code.Append(string.Format("\t\t\t\tCallback{0}(ClientID, ", o.ServerName));
				foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
				code.AppendLine(");");
				if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");

				code.AppendFormat("\t\tpublic static void Callback{0}(Guid SenderID, ", o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
				code.AppendLine(string.Format("\t\t\t\tvar tcl = GetClients<{0}Base>(GetClientMessageList<{1}>(UpdateID)).Where(a => a.ClientID != SenderID);", o.Owner.Name, DataTypeGenerator.GenerateType(DCMType)));
				code.AppendLine(string.Format("\t\t\t\tforeach(var tc in tcl)"));
				code.Append(string.Format("\t\t\t\t\ttc.Callback.{0}Callback(", o.ServerName));
				foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
				code.AppendLine(");");
				if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");
			}
			return code.ToString();
		}

		public static string GenerateServiceClientMethodDCM45(Method o, bool UseTPL, bool IsImmediate = true)
		{
			var code = new StringBuilder();
			if (o.UseAwaitPattern && CanGenerateAsync(o.Owner, false))
			{
				if (o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tpublic virtual System.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				else code.AppendFormat("\t\tpublic virtual System.Threading.Tasks.Task<{1}> {0}Async(", o.HasClientType ? o.ClientName : o.ServerName, DataTypeGenerator.GenerateType(o.ReturnType));
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (IsImmediate)
				{
					code.Append(string.Format("\t\t\treturn Channel.{0}Async(", o.ServerName));
					foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
					code.AppendLine(");");
				}
				else
				{
					code.Append(string.Format("\t\t\treturn Channel.{0}Async(", o.ServerName));
					foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
					code.AppendLine(");");
				}
				code.AppendLine("\t\t}");
			}
			if (o.UseSyncPattern)
			{
				code.AppendFormat("\t\tpublic virtual {1} {0}(", o.HasClientType ? o.ClientName : o.ServerName, DataTypeGenerator.GenerateType(o.ReturnType));
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (o.ReturnType.Primitive != PrimitiveTypes.Void)
				{
					if (IsImmediate)
					{
						code.Append(string.Format("\t\t\t{1}Channel.{0}(", o.ServerName, o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : ""));
						foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
					}
					else
					{
						code.Append(string.Format("\t\t\t{1}Channel.{0}(", o.ServerName, o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : ""));
						foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
					}
				}
				else
				{
					if (IsImmediate)
					{
						if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
						code.Append(string.Format("{1}\t\t\t{2}Channel.{0}(", o.ServerName, UseTPL ? "\t" : "", o.ReturnType.Primitive != PrimitiveTypes.Void ? "t = " : ""));
						foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
						if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
					}
					else
					{
						if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
						code.Append(string.Format("{1}\t\t\t{2}Channel.{0}(", o.ServerName, UseTPL ? "\t" : "", o.ReturnType.Primitive != PrimitiveTypes.Void ? "t = " : ""));
						foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
						if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
					}
				}
				code.AppendLine("\t\t}");
			}
			return code.ToString();
		}

		#endregion

		#region - Callback DRE -

		public static string GenerateCallbackInterfaceDCM40(DataChangeMethod o, bool IsServer)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			var dcmtype = o.ReturnType as Data;
			if (dcmtype == null) dcmtype = Generator.ReferenceRetrieve(o.Owner.Parent.Owner, o.Owner.Parent.Owner.Namespace, o.ReturnType.ID) as Data;
			if (dcmtype == null) return "";

			if (o.GenerateGetFunction)
			{
				if (o.GetDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.GetDocumentation));
					foreach (MethodParameter mp in o.GetParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var x = new Method(string.Format("Get{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = o.GetParameters, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = o.ReturnType };
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode40(x, true) : GenerateClientInterfaceMethodCode40(x, true));
			}
			if (o.GenerateNewDeleteFunction)
			{
				if (o.NewDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.NewDocumentation));
					foreach (MethodParameter mp in o.NewParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var xp = new ObservableCollection<MethodParameter>(o.NewParameters);
				var x = new Method(string.Format("New{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = xp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
				xp.Insert(0, new MethodParameter(dcmtype, "DREData", o.Owner, x));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode40(x, true) : GenerateClientInterfaceMethodCode40(x, true));
				if (o.DeleteDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.DeleteDocumentation));
					foreach (MethodParameter mp in o.DeleteParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var yp = new ObservableCollection<MethodParameter>(o.DeleteParameters);
				var y = new Method(string.Format("Delete{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = yp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
				yp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode40(y, true) : GenerateClientInterfaceMethodCode40(y, true));
			}
			if (o.GenerateOpenCloseFunction)
			{
				if (o.OpenDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.OpenDocumentation));
					foreach (MethodParameter mp in o.OpenParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var xp = new ObservableCollection<MethodParameter>(o.OpenParameters);
				var x = new Method(string.Format("Open{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = xp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				xp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, x));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode40(x, true) : GenerateClientInterfaceMethodCode40(x, true));
				if (o.CloseDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.CloseDocumentation));
					foreach (MethodParameter mp in o.CloseParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var yp = new ObservableCollection<MethodParameter>(o.CloseParameters);
				var y = new Method(string.Format("Close{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = yp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				yp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode40(y, true) : GenerateClientInterfaceMethodCode40(y, true));
			}

			foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Immediate))
			{
				DataType edt = de.DataType;

				var tp = new ObservableCollection<MethodParameter>();
				var x = new Method(string.Format("Update{0}{1}DRECallback", dcmtype.Name, de.DataName), o.Owner) { Parameters = tp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new MethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));

				if (edt.TypeMode == DataTypeMode.Collection) tp.Add(new MethodParameter(new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = edt.CollectionGenericType }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Dictionary) tp.Add(new MethodParameter(new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Queue) { continue; }
				else if (edt.TypeMode == DataTypeMode.Stack) { continue; }
				else tp.Add(new MethodParameter(edt, "ChangedValue", o.Owner, x));

				code.Append(IsServer ? GenerateServiceInterfaceMethodCode40(x, true) : GenerateClientInterfaceMethodCode40(x, true));
			}

			if (dcmtype.Elements.Any(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch))
			{
				var tp = new ObservableCollection<MethodParameter>();
				var x = new Method(string.Format("BatchUpdate{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = tp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new MethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode != DataTypeMode.Collection && a.DataType.TypeMode != DataTypeMode.Dictionary))
					tp.Add(new MethodParameter(new DataType("CMDItemValue", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = de.DataType }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Collection))
				{
					DataType edt = de.DataType;
					tp.Add(new MethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = edt.CollectionGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				}
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Dictionary))
				{
					DataType edt = de.DataType;
					tp.Add(new MethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				}

				code.Append(IsServer ? GenerateServiceInterfaceMethodCode40(x, true) : GenerateClientInterfaceMethodCode40(x, true));
			}

			code.AppendLine();

			return code.ToString();
		}

		public static string GenerateCallbackInterfaceDCM45(DataChangeMethod o, bool IsServer)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			var dcmtype = o.ReturnType as Data;
			if (dcmtype == null) dcmtype = Generator.ReferenceRetrieve(o.Owner.Parent.Owner, o.Owner.Parent.Owner.Namespace, o.ReturnType.ID) as Data;
			if (dcmtype == null) return "";

			if (o.GenerateGetFunction)
			{
				if (o.GetDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.GetDocumentation));
					foreach (MethodParameter mp in o.GetParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var x = new Method(string.Format("Get{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = o.GetParameters, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = o.ReturnType };
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(x, true) : GenerateClientInterfaceMethodCode45(x, true));
			}
			if (o.GenerateNewDeleteFunction)
			{
				if (o.NewDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.NewDocumentation));
					foreach (MethodParameter mp in o.NewParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var xp = new ObservableCollection<MethodParameter>(o.NewParameters);
				var x = new Method(string.Format("New{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = xp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				xp.Insert(0, new MethodParameter(dcmtype, "DREData", o.Owner, x));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(x, true) : GenerateClientInterfaceMethodCode45(x, true));
				if (o.DeleteDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.DeleteDocumentation));
					foreach (MethodParameter mp in o.DeleteParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var yp = new ObservableCollection<MethodParameter>(o.DeleteParameters);
				var y = new Method(string.Format("Delete{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = yp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				yp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(y, true) : GenerateClientInterfaceMethodCode45(y, true));
			}
			if (o.GenerateOpenCloseFunction)
			{
				if (o.OpenDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.OpenDocumentation));
					foreach (MethodParameter mp in o.OpenParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var xp = new ObservableCollection<MethodParameter>(o.OpenParameters);
				var x = new Method(string.Format("Open{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = xp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				xp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, x));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(x, true) : GenerateClientInterfaceMethodCode45(x, true));
				if (o.CloseDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.CloseDocumentation));
					foreach (MethodParameter mp in o.CloseParameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				var yp = new ObservableCollection<MethodParameter>(o.CloseParameters);
				var y = new Method(string.Format("Close{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = yp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				yp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(y, true) : GenerateClientInterfaceMethodCode45(y, true));
			}

			foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Immediate))
			{
				DataType edt = de.DataType;

				var tp = new ObservableCollection<MethodParameter>();
				var x = new Method(string.Format("Update{0}{1}DRECallback", dcmtype.Name, de.DataName), o.Owner) { Parameters = tp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new MethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));

				if (edt.TypeMode == DataTypeMode.Collection) tp.Add(new MethodParameter(new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = edt.CollectionGenericType }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Dictionary) tp.Add(new MethodParameter(new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Queue) { continue; }
				else if (edt.TypeMode == DataTypeMode.Stack) { continue; }
				else tp.Add(new MethodParameter(edt, "ChangedValue", o.Owner, x));

				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(x, true) : GenerateClientInterfaceMethodCode45(x, true));
			}

			if (dcmtype.Elements.Any(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch))
			{
				var tp = new ObservableCollection<MethodParameter>();
				var x = new Method(string.Format("BatchUpdate{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = tp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new MethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode != DataTypeMode.Collection && a.DataType.TypeMode != DataTypeMode.Dictionary))
					tp.Add(new MethodParameter(new DataType("CMDItemValue", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = de.DataType }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Collection))
				{
					DataType edt = de.DataType;
					tp.Add(new MethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = edt.CollectionGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				}
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Dictionary))
				{
					DataType edt = de.DataType;
					tp.Add(new MethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				}

				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(x, true) : GenerateClientInterfaceMethodCode45(x, true));
			}

			code.AppendLine();

			return code.ToString();
		}

		public static string GenerateCallbackImplementationDCM40(DataChangeMethod o, bool IsServer)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			var dcmtype = o.ReturnType as Data;
			if (dcmtype == null) dcmtype = Generator.ReferenceRetrieve(o.Owner.Parent.Owner, o.Owner.Parent.Owner.Namespace, o.ReturnType.ID) as Data;
			if (dcmtype == null) return "";

			if (!IsServer)
			{
				if (o.GenerateGetFunction)
				{
					code.Append(string.Format("\t\tpublic abstract {1} Get{0}DRECallback(", dcmtype.Name, o.ReturnType.HasClientType ? o.ReturnType.ClientType : o.ReturnType));
					foreach (MethodParameter mp in o.GetParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.GetParameters.IndexOf(mp) < o.GetParameters.Count - 1 ? ", " : ""));
					code.AppendLine(");");
				}
				if (o.GenerateNewDeleteFunction)
				{
					code.Append(string.Format("\t\tpublic abstract void New{0}DRECallback({1} DREData{2}", dcmtype.Name, DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype), o.NewParameters.Count != 0 ? ", " : ""));
					foreach (MethodParameter mp in o.NewParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.NewParameters.IndexOf(mp) < o.NewParameters.Count - 1 ? ", " : ""));
					code.AppendLine(");");
					code.Append(string.Format("\t\tpublic abstract void Delete{0}DRECallback(Guid DREDataID{1}", dcmtype.Name, o.DeleteParameters.Count != 0 ? ", " : ""));
					foreach (MethodParameter mp in o.DeleteParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.DeleteParameters.IndexOf(mp) < o.DeleteParameters.Count - 1 ? ", " : ""));
					code.AppendLine(");");
				}
				if (o.GenerateOpenCloseFunction)
				{
					code.Append(string.Format("\t\tpublic abstract void Open{0}DRECallback(Guid DREDataID{1}", dcmtype.Name, o.OpenParameters.Count != 0 ? ", " : ""));
					foreach (MethodParameter mp in o.OpenParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.OpenParameters.IndexOf(mp) < o.OpenParameters.Count - 1 ? ", " : ""));
					code.AppendLine(");");
					code.Append(string.Format("\t\tpublic abstract void Close{0}DRECallback(Guid DREDataID{1}", dcmtype.Name, o.CloseParameters.Count != 0 ? ", " : ""));
					foreach (MethodParameter mp in o.CloseParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.CloseParameters.IndexOf(mp) < o.CloseParameters.Count - 1 ? ", " : ""));
					code.AppendLine(");");
				}
			}
			else
			{
				if (o.GenerateGetFunction)
				{
					var x = new Method(string.Format("Get{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = o.GetParameters, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = o.ReturnType };
					code.Append(GenerateCallbackServerMethodDCM40(x, o.UseTPLForCallbacks));
				}
				if (o.GenerateNewDeleteFunction)
				{
					var xp = new ObservableCollection<MethodParameter>(o.NewParameters);
					var x = new Method(string.Format("New{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = xp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
					xp.Insert(0, new MethodParameter(dcmtype, "DREData", o.Owner, x));
					code.Append(GenerateCallbackServerMethodDCM40(x, o.UseTPLForCallbacks));
					var yp = new ObservableCollection<MethodParameter>(o.DeleteParameters);
					var y = new Method(string.Format("Delete{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = yp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
					yp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
					code.Append(GenerateCallbackServerMethodDCM40(y, o.UseTPLForCallbacks));
				}
				if (o.GenerateOpenCloseFunction)
				{
					var xp = new ObservableCollection<MethodParameter>(o.OpenParameters);
					var x = new Method(string.Format("Open{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = xp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
					xp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, x));
					code.Append(GenerateCallbackServerMethodDCM40(x, o.UseTPLForCallbacks));
					var yp = new ObservableCollection<MethodParameter>(o.CloseParameters);
					var y = new Method(string.Format("Close{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = yp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
					yp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
					code.Append(GenerateCallbackServerMethodDCM40(y, o.UseTPLForCallbacks));
				}
			}

			foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Immediate))
			{
				DataType edt = de.DataType;

				var tp = new ObservableCollection<MethodParameter>();
				var x = new Method(string.Format("Update{0}{1}DRECallback", dcmtype.Name, de.DataName), o.Owner) { Parameters = tp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new MethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));

				if (edt.TypeMode == DataTypeMode.Collection) tp.Add(new MethodParameter(new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = edt.CollectionGenericType }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Dictionary) tp.Add(new MethodParameter(new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Queue) { continue; }
				else if (edt.TypeMode == DataTypeMode.Stack) { continue; }
				else tp.Add(new MethodParameter(edt, "ChangedValue", o.Owner, x));

				code.Append(!IsServer ? GenerateCallbackClientImmediateMethodDCM40(x, dcmtype, de.DataName, edt.TypeMode, o.UseTPLForCallbacks) : GenerateCallbackServerMethodDCM40(x, o.UseTPLForCallbacks));
			}

			if (dcmtype.Elements.Any(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch))
			{
				var tp = new ObservableCollection<MethodParameter>();
				var x = new Method(string.Format("BatchUpdate{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = tp, UseSyncPattern = o.UseSyncPattern, UseAsyncPattern = false, UseAwaitPattern = false, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new MethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode != DataTypeMode.Collection && a.DataType.TypeMode != DataTypeMode.Dictionary))
					tp.Add(new MethodParameter(new DataType("CMDItemValue", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = de.DataType }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Collection))
				{
					DataType edt = de.DataType;
					tp.Add(new MethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = edt.CollectionGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				}
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Dictionary))
				{
					DataType edt = de.DataType;
					tp.Add(new MethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				}

				code.Append(!IsServer ? GenerateCallbackClientBatchMethodDCM40(x, dcmtype, o.UseTPLForCallbacks) : GenerateCallbackServerMethodDCM40(x, o.UseTPLForCallbacks, false));
			}

			code.AppendLine();

			return code.ToString();
		}

		public static string GenerateCallbackClientImmediateMethodDCM40(Method o, DataType DCMType, string ElementName, DataTypeMode TypeMode, bool UseTPL)
		{
			var code = new StringBuilder();
			code.AppendFormat("\t\tpublic virtual void {0}(", o.HasClientType ? o.ClientName : o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
			if (TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\tvar temp = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
				code.AppendLine(string.Format("\t\t\tif (temp != null) temp.{0}.ApplyDelta(ChangeListItem);", ElementName));
			}
			else if (TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\tvar temp = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
				code.AppendLine(string.Format("\t\t\tif (temp != null) temp.{0}.ApplyDelta(ChangeDictionaryItem);", ElementName));
			}
			else code.AppendLine(string.Format("\t\t\t{0}.UpdateValue(UpdateID, {0}.{1}Property, ChangedValue);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType), ElementName));
			if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
			code.AppendLine("\t\t}");
			return code.ToString();
		}

		public static string GenerateCallbackClientBatchMethodDCM40(Method o, Data DCMType, bool UseTPL)
		{
			var code = new StringBuilder();
			code.AppendFormat("\t\tpublic virtual void {0}(", o.HasClientType ? o.ClientName : o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
			code.AppendLine(string.Format("\t\t\t\tvar t = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
			code.AppendLine(string.Format("\t\t\t\tif (t != null)"));
			code.AppendLine("\t\t\t\t{");
			foreach (var de in o.Parameters.Where(a => a.Type.Name == "CMDItemValue"))
				code.AppendLine(string.Format("\t\t\t\t\tt.ApplyDelta({0});", de.Name));
			foreach (DataElement de in DCMType.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && (a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
				code.AppendLine(string.Format("\t\t\t\t\tt.{0}.ApplyDelta({0}Delta);", de.HasClientType ? de.ClientName : de.DataName));
			code.AppendLine("\t\t\t\t}");
			if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
			code.AppendLine("\t\t}");
			return code.ToString();
		}

		public static string GenerateCallbackServerMethodDCM40(Method o, bool UseTPL, bool IsImmediate = true)
		{
			var code = new StringBuilder();
			code.AppendFormat("\t\tpublic virtual void {0}(", o.HasClientType ? o.ClientName : o.ServerName);
			foreach (MethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			if (IsImmediate)
			{
				if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
				code.Append(string.Format("{1}\t\t\t__callback.{0}(", o.ServerName, UseTPL ? "\t" : ""));
				foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
				code.AppendLine(");");
				if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
			}
			else
			{
				if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
				code.Append(string.Format("{1}\t\t\t__callback.{0}(", o.ServerName, UseTPL ? "\t" : ""));
				foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
				code.AppendLine(");");
				if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
			}
			code.AppendLine("\t\t}");
			return code.ToString();
		}

		public static string GenerateCallbackImplementationDCM45(DataChangeMethod o, bool IsServer)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			var dcmtype = o.ReturnType as Data;
			if (dcmtype == null) dcmtype = Generator.ReferenceRetrieve(o.Owner.Parent.Owner, o.Owner.Parent.Owner.Namespace, o.ReturnType.ID) as Data;
			if (dcmtype == null) return "";
			bool uap = o.UseAwaitPattern && CanGenerateAsync(o.Owner, IsServer);

			if (!IsServer)
			{
				if (o.GenerateGetFunction)
				{
					if (uap)
					{
						code.Append(string.Format("\t\tpublic abstract System.Threading.Tasks.Task<{1}> Get{0}DRECallbackAsync(", dcmtype.Name, o.ReturnType.HasClientType ? o.ReturnType.ClientType : o.ReturnType));
						foreach (MethodParameter mp in o.GetParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.GetParameters.IndexOf(mp) < o.GetParameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
					}
					if (o.UseSyncPattern || !uap)
					{
						code.Append(string.Format("\t\tpublic abstract {1} Get{0}DRECallback(", dcmtype.Name, o.ReturnType.HasClientType ? o.ReturnType.ClientType : o.ReturnType));
						foreach (MethodParameter mp in o.GetParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.GetParameters.IndexOf(mp) < o.GetParameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
					}
				}
				if (o.GenerateNewDeleteFunction)
				{
					if (uap)
					{
						code.Append(string.Format("\t\tpublic abstract System.Threading.Tasks.Task New{0}DRECallbackAsync({1} DREData{2}", dcmtype.Name, DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype), o.NewParameters.Count != 0 ? ", " : ""));
						foreach (MethodParameter mp in o.NewParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.NewParameters.IndexOf(mp) < o.NewParameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
						code.Append(string.Format("\t\tpublic abstract System.Threading.Tasks.Task Delete{0}DRECallbackAsync(Guid DREDataID{1}", dcmtype.Name, o.NewParameters.Count != 0 ? ", " : ""));
						foreach (MethodParameter mp in o.DeleteParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.DeleteParameters.IndexOf(mp) < o.DeleteParameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
					}
					if (o.UseSyncPattern || !uap)
					{
						code.Append(string.Format("\t\tpublic abstract void New{0}DRECallback({1} DREData{2}", dcmtype.Name, DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype), o.NewParameters.Count != 0 ? ", " : ""));
						foreach (MethodParameter mp in o.NewParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.NewParameters.IndexOf(mp) < o.NewParameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
						code.Append(string.Format("\t\tpublic abstract void Delete{0}DRECallback(Guid DREDataID{1}", dcmtype.Name, o.DeleteParameters.Count != 0 ? ", " : ""));
						foreach (MethodParameter mp in o.DeleteParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.DeleteParameters.IndexOf(mp) < o.DeleteParameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
					}
				}
				if (o.GenerateOpenCloseFunction)
				{
					if (uap)
					{
						code.Append(string.Format("\t\tpublic abstract System.Threading.Tasks.Task Open{0}DRECallbackAsync(Guid DREDataID{1}", dcmtype.Name, o.OpenParameters.Count != 0 ? ", " : ""));
						foreach (MethodParameter mp in o.OpenParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.OpenParameters.IndexOf(mp) < o.OpenParameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
						code.Append(string.Format("\t\tpublic abstract System.Threading.Tasks.Task Close{0}DRECallbackAsync(Guid DREDataID{1}", dcmtype.Name, o.CloseParameters.Count != 0 ? ", " : ""));
						foreach (MethodParameter mp in o.CloseParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.CloseParameters.IndexOf(mp) < o.CloseParameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
					}
					if (o.UseSyncPattern || !uap)
					{
						code.Append(string.Format("\t\tpublic abstract void Open{0}DRECallback(Guid DREDataID{1}", dcmtype.Name, o.OpenParameters.Count != 0 ? ", " : ""));
						foreach (MethodParameter mp in o.OpenParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.OpenParameters.IndexOf(mp) < o.OpenParameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
						code.Append(string.Format("\t\tpublic abstract void Close{0}DRECallback(Guid DREDataID{1}", dcmtype.Name, o.CloseParameters.Count != 0 ? ", " : ""));
						foreach (MethodParameter mp in o.CloseParameters) code.Append(string.Format("{0} {1}{2}", DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type), mp.Name, o.CloseParameters.IndexOf(mp) < o.CloseParameters.Count - 1 ? ", " : ""));
						code.AppendLine(");");
					}
				}
			}
			else
			{
				if (o.GenerateGetFunction)
				{
					var x = new Method(string.Format("Get{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = o.GetParameters, UseSyncPattern = o.UseSyncPattern || !uap, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = o.ReturnType };
					code.Append(GenerateCallbackServerMethodDCM45(x, o.UseTPLForCallbacks));
				}
				if (o.GenerateNewDeleteFunction)
				{
					var xp = new ObservableCollection<MethodParameter>(o.NewParameters);
					var x = new Method(string.Format("New{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = xp, UseSyncPattern = o.UseSyncPattern || !uap, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
					xp.Insert(0, new MethodParameter(dcmtype, "DREData", o.Owner, x));
					code.Append(GenerateCallbackServerMethodDCM45(x, o.UseTPLForCallbacks));
					var yp = new ObservableCollection<MethodParameter>(o.DeleteParameters);
					var y = new Method(string.Format("Delete{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = yp, UseSyncPattern = o.UseSyncPattern || !uap, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
					yp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
					code.Append(GenerateCallbackServerMethodDCM45(y, o.UseTPLForCallbacks));
				}
				if (o.GenerateOpenCloseFunction)
				{
					var xp = new ObservableCollection<MethodParameter>(o.OpenParameters);
					var x = new Method(string.Format("Open{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = xp, UseSyncPattern = o.UseSyncPattern || !uap, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
					xp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, x));
					code.Append(GenerateCallbackServerMethodDCM45(x, o.UseTPLForCallbacks));
					var yp = new ObservableCollection<MethodParameter>(o.CloseParameters);
					var y = new Method(string.Format("Close{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = yp, UseSyncPattern = o.UseSyncPattern || !uap, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
					yp.Insert(0, new MethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
					code.Append(GenerateCallbackServerMethodDCM45(y, o.UseTPLForCallbacks));
				}
			}

			foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Immediate))
			{
				DataType edt = de.DataType;

				var tp = new ObservableCollection<MethodParameter>();
				var x = new Method(string.Format("Update{0}{1}DRECallback", dcmtype.Name, de.DataName), o.Owner) { Parameters = tp, UseSyncPattern = o.UseSyncPattern || !uap, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new MethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));

				if (edt.TypeMode == DataTypeMode.Collection) tp.Add(new MethodParameter(new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = edt.CollectionGenericType }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Dictionary) tp.Add(new MethodParameter(new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Queue) { continue; }
				else if (edt.TypeMode == DataTypeMode.Stack) { continue; }
				else tp.Add(new MethodParameter(edt, "ChangedValue", o.Owner, x));

				code.Append(!IsServer ? GenerateCallbackClientImmediateMethodDCM45(x, dcmtype, de.DataName, edt.TypeMode, o.UseTPLForCallbacks) : GenerateCallbackServerMethodDCM45(x, o.UseTPLForCallbacks));
			}

			if (dcmtype.Elements.Any(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch))
			{
				var tp = new ObservableCollection<MethodParameter>();
				var x = new Method(string.Format("BatchUpdate{0}DRECallback", dcmtype.Name), o.Owner) { Parameters = tp, UseSyncPattern = o.UseSyncPattern || !uap, UseAsyncPattern = false, UseAwaitPattern = o.UseAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new MethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode != DataTypeMode.Collection && a.DataType.TypeMode != DataTypeMode.Dictionary))
					tp.Add(new MethodParameter(new DataType("CMDItemValue", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = de.DataType }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Collection))
				{
					DataType edt = de.DataType;
					tp.Add(new MethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = edt.CollectionGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				}
				foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Dictionary))
				{
					DataType edt = de.DataType;
					tp.Add(new MethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { CollectionGenericType = new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WIN8) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				}

				code.Append(!IsServer ? GenerateCallbackClientBatchMethodDCM45(x, dcmtype, o.UseTPLForCallbacks) : GenerateCallbackServerMethodDCM45(x, o.UseTPLForCallbacks, false));
			}

			code.AppendLine();

			return code.ToString();
		}

		public static string GenerateCallbackClientImmediateMethodDCM45(Method o, DataType DCMType, string ElementName, DataTypeMode TypeMode, bool UseTPL)
		{
			var code = new StringBuilder();
			if (o.UseAwaitPattern && CanGenerateAsync(o.Owner, false))
			{
				code.AppendFormat("\t\tpublic virtual System.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(() => {");
				if (TypeMode == DataTypeMode.Collection)
				{
					code.AppendLine(string.Format("\t\t\tvar temp = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
					code.AppendLine(string.Format("\t\t\tif (temp != null) temp.{0}.ApplyDelta(ChangedItem);", ElementName));
				}
				else if (TypeMode == DataTypeMode.Dictionary)
				{
					code.AppendLine(string.Format("\t\t\tvar temp = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
					code.AppendLine(string.Format("\t\t\tif (temp != null) temp.{0}.ApplyDelta(ChangedItem);", ElementName));
				}
				else code.AppendLine(string.Format("\t\t\t{0}.UpdateValue(UpdateID, {0}.{1}Property, ChangedValue);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType), ElementName));
				code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");
			}
			if (o.UseSyncPattern)
			{
				code.AppendFormat("\t\tpublic virtual void {0}(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
				if (TypeMode == DataTypeMode.Collection)
				{
					code.AppendLine(string.Format("\t\t\tvar temp = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
					code.AppendLine(string.Format("\t\t\tif (temp != null) temp.{0}.ApplyDelta(ChangedItem);", ElementName));
				}
				else if (TypeMode == DataTypeMode.Dictionary)
				{
					code.AppendLine(string.Format("\t\t\tvar temp = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
					code.AppendLine(string.Format("\t\t\tif (temp != null) temp.{0}.ApplyDelta(ChangedItem);", ElementName));
				}
				else code.AppendLine(string.Format("\t\t\t{0}.UpdateValue(UpdateID, {0}.{1}Property, ChangedValue);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType), ElementName));
				if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");
			}
			return code.ToString();
		}

		public static string GenerateCallbackClientBatchMethodDCM45(Method o, Data DCMType, bool UseTPL)
		{
			var code = new StringBuilder();
			if (o.UseAwaitPattern && CanGenerateAsync(o.Owner, false))
			{
				code.AppendFormat("\t\tpublic virtual System.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(() => {");
				code.AppendLine(string.Format("\t\t\t\tvar t = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
				code.AppendLine(string.Format("\t\t\t\tif (t != null)"));
				code.AppendLine("\t\t\t\t{");
				foreach (var de in o.Parameters.Where(a => a.Type.Name == "CMDItemValue"))
					code.AppendLine(string.Format("\t\t\t\t\tt.ApplyDelta({0});", de.Name));
				foreach (DataElement de in DCMType.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && (a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
					code.AppendLine(string.Format("\t\t\t\t\tt.{0}.ApplyDelta({0}Delta);", de.HasClientType ? de.ClientName : de.DataName));
				code.AppendLine("\t\t\t\t}");
				code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");
			}
			if (o.UseSyncPattern)
			{
				code.AppendFormat("\t\tpublic virtual void {0}(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
				code.AppendLine(string.Format("\t\t\t\tvar t = {0}.GetDataFromID(UpdateID);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)));
				code.AppendLine(string.Format("\t\t\t\tif (t != null)"));
				code.AppendLine("\t\t\t\t{");
				foreach (var de in o.Parameters.Where(a => a.Type.Name == "CMDItemValue"))
					code.AppendLine(string.Format("\t\t\t\t\tt.ApplyDelta({0});", de.Name));
				foreach (DataElement de in DCMType.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && (a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
					code.AppendLine(string.Format("\t\t\t\t\tt.{0}.ApplyDelta({0}Delta);", de.HasClientType ? de.ClientName : de.DataName));
				code.AppendLine("\t\t\t\t}");
				if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");
			}
			return code.ToString();
		}

		public static string GenerateCallbackServerMethodDCM45(Method o, bool UseTPL, bool IsImmediate = true)
		{
			var code = new StringBuilder();
			if (o.UseAwaitPattern && CanGenerateAsync(o.Owner, true))
			{
				code.AppendFormat("\t\tpublic virtual System.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (IsImmediate)
				{
					code.AppendLine("\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(() => {");
					code.Append(string.Format("{1}\t\t\t__callback.{0}{2}(", o.ServerName, UseTPL ? "\t" : "", CanGenerateAsync(o.Owner, false) ? "Async" : ""));
					foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
					code.AppendLine(");");
					code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				}
				else
				{
					code.AppendLine("\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(() => {");
					code.Append(string.Format("{1}\t\t\t__callback.{0}{2}(", o.ServerName, UseTPL ? "\t" : "", CanGenerateAsync(o.Owner, false) ? "Async" : ""));
					foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
					code.AppendLine(");");
					code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				}
				code.AppendLine("\t\t}");
			}
			if (o.UseSyncPattern)
			{
				code.AppendFormat("\t\tpublic virtual void {0}(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (MethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (IsImmediate)
				{
					if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
					code.Append(string.Format("{1}\t\t\t__callback.{0}(", o.ServerName, UseTPL ? "\t" : ""));
					foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
					code.AppendLine(");");
					if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				}
				else
				{
					if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
					code.Append(string.Format("{1}\t\t\t__callback.{0}(", o.ServerName, UseTPL ? "\t" : ""));
					foreach (MethodParameter mp in o.Parameters) code.Append(string.Format("{0}{1}", mp.Name, o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : ""));
					code.AppendLine(");");
					if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				}
				code.AppendLine("\t\t}");
			}
			return code.ToString();
		}

		#endregion

		#region - Method Parameters -

		public static string GenerateMethodParameterServerCode(MethodParameter o)
		{
			return o.IsHidden ? "" : string.Format("{0} {1}", DataTypeGenerator.GenerateType(o.Type), o.Name);
		}

		public static string GenerateMethodParameterClientCode(MethodParameter o)
		{
			if (o.IsHidden) return "";

			if (o.Type.TypeMode == DataTypeMode.Class)
			{
				var ptype = o.Type as Data;
				return string.Format("{0} {1}", ptype != null && ptype.HasClientType ? DataTypeGenerator.GenerateType(ptype.ClientType) : DataTypeGenerator.GenerateType(o.Type), o.Name);
			}													    
			if (o.Type.TypeMode == DataTypeMode.Struct)			    
			{													    
				var ptype = o.Type as Data;						    
				return string.Format("{0} {1}", ptype != null && ptype.HasClientType ? DataTypeGenerator.GenerateType(ptype.ClientType) : DataTypeGenerator.GenerateType(o.Type), o.Name);
			}													    
			if (o.Type.TypeMode == DataTypeMode.Enum)			    
			{													    
				var ptype = o.Type as Projects.Enum;			    
				return string.Format("{0} {1}", ptype != null && ptype.HasClientType ? DataTypeGenerator.GenerateType(ptype.ClientType) : DataTypeGenerator.GenerateType(o.Type), o.Name);
			}
			
			return string.Format("{0} {1}", DataTypeGenerator.GenerateType(o.Type), o.Name);
		}

		#endregion

		#region - Properties -

		public static string GeneratePropertyServerCode40(Property o)
		{
			return GeneratePropertyServerCode45(o);
		}

		public static string GeneratePropertyServerCode45(Property o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.AppendFormat("\t\t{0} {1} {{ ", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
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
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/{1}/Get{2}\")]" : "\t\t[OperationContract(Action = \"{0}/{1}/Get{2}\", ReplyAction = \"{0}/{1}/Set{2}Response\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
			code.AppendLine(string.Format("\t\t{0} Get{1}();", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName));
			if (!o.IsReadOnly)
			{
				code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/{1}/Get{2}\")]" : "\t\t[OperationContract(Action = \"{0}/{1}/Get{2}\", ReplyAction = \"{0}/{1}/Set{2}Response\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
				code.AppendLine(string.Format("\t\tvoid Set{1}({0} value);", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName));
			}
			return code.ToString();
		}

		public static string GeneratePropertyClientCode(Property o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			code.AppendLine(string.Format("\t\t{0} I{1}.Get{2}()", DataTypeGenerator.GenerateType(o.ReturnType), o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, o.ServerName));
			code.AppendLine("\t\t{");
			code.AppendLine(string.Format("\t\t\treturn base.Channel.Get{0}();", o.HasClientType ? o.ClientName : o.ServerName));
			code.AppendLine("\t\t}");

			if(!o.IsReadOnly)
			{
				code.AppendLine(string.Format("\t\tvoid I{1}.Set{2}({0} value)", DataTypeGenerator.GenerateType(o.ReturnType), o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, o.ServerName));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\tbase.Channel.Set{0}(value);", o.HasClientType ? o.ClientName : o.ServerName));
				code.AppendLine("\t\t}");
			}

			code.AppendLine(string.Format(o.IsReadOnly == false ? "\t\tpublic {0} {1} {{ get {{ return ((I{2})this).Get{1}(); }} set {{ ((I{2})this).Set{1}(value); }} }}" : "\t\tpublic {0} {1} {{ get {{ return ((I{2})this).Get{1}(); }} }}", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name));

			return code.ToString();
		}

		#endregion
	}
}