using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using NETPath.Projects;
using NETPath.Projects.Helpers;

namespace NETPath.Generators.NET.CS
{
	internal static class RESTServiceGenerator
	{

		public static void VerifyCode(RESTService o, Action<CompileMessage> AddMessage)
		{
			if (string.IsNullOrEmpty(o.Name))
				AddMessage(new CompileMessage("GS2000", "An service in the '" + o.Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
			else
				if (RegExs.MatchCodeName.IsMatch(o.Name) == false)
					AddMessage(new CompileMessage("GS2001", "The service '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
			if (o.HasClientType) 
				if (RegExs.MatchCodeName.IsMatch(o.ClientType.Name) == false)
					AddMessage(new CompileMessage("GS2002", "The service '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

			var Operations = new ObservableCollection<RESTMethod>();
			Operations.AddRange(o.ServiceOperations);

			foreach (RESTMethod m in Operations)
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

				if(m.Parameters.Count > 1 && (m.BodyStyle == WebMessageBodyStyle.Bare || m.BodyStyle == WebMessageBodyStyle.WrappedResponse))
					AddMessage(new CompileMessage("GS2017", "The method '" + m.ServerName + "' in the '" + o.Name + "' service contains more than one parameter and therefore cannot use the Bare or WrappedResponse BodyStyles. Please specify a valid body style.", CompileMessageSeverity.WARN, o, m, m.GetType(), o.Parent.Owner.ID));

				if (m.ReturnType == null)
					AddMessage(new CompileMessage("GS2007", "The method '" + m.ServerName + "' in the '" + o.Name + "' service has a blank Return Type. A Return Type MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
				else
					if (m.ReturnType.TypeMode == DataTypeMode.Primitive && (m.ReturnType.Primitive == PrimitiveTypes.Void || m.ReturnType.Primitive == PrimitiveTypes.None))
						AddMessage(new CompileMessage("GS2012", "The method return type '" + m.ReturnType + "' in the '" + o.Name + "' service is not a valid REST return type. Please specify a valid REST return type.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
				if (m.ReturnType.TypeMode == DataTypeMode.Namespace || m.ReturnType.TypeMode == DataTypeMode.Interface)
					AddMessage(new CompileMessage("GS2013", "The method return type '" + m.ReturnType + "' in the '" + o.Name + "' service is not a valid return type. Please specify a valid return type.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));

				foreach (RESTMethodParameter mp in m.Parameters)
				{
					if(string.IsNullOrEmpty(mp.Name))
						AddMessage(new CompileMessage("GS2008", "The method parameter '" + m.ServerName + "' in the '" + o.Name + "' service has a parameter with a blank name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
					if (mp.IsRESTInvalid)
						AddMessage(new CompileMessage("GS2009", "The method REST parameter '" + m.ServerName + "' in the '" + m.ServerName + "' method is not a valid REST parameter. Please specify a valid REST parameter.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
					if (mp.Name == "__callback")
						AddMessage(new CompileMessage("GS2016", "The name of the method parameter '" + mp.Name + "' in the '" + m.ServerName + "' method is invalid. Please rename it.", CompileMessageSeverity.ERROR, o, m, m.GetType(), o.Parent.Owner.ID));
				}
			}
		}

		public static bool CanGenerateAsync(Service o, bool IsServer)
		{
			return IsServer ? (o.AsynchronyMode == ServiceAsynchronyMode.Server || o.AsynchronyMode == ServiceAsynchronyMode.Both) : (o.AsynchronyMode == ServiceAsynchronyMode.Client || o.AsynchronyMode == ServiceAsynchronyMode.Both || o.AsynchronyMode == ServiceAsynchronyMode.Default);
		}

		#region - Server Interfaces -

		public static string GenerateServerCode(RESTService o)
		{
			var code = new StringBuilder();
			if (o.ServiceDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ServiceDocumentation));
			foreach (DataType dt in o.KnownTypes)
				code.AppendLine(string.Format("\t[ServiceKnownType(typeof({0}))]", dt));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t[ServiceContract({0}{1}Namespace = \"{2}\")]", o.ProtectionLevel != System.Net.Security.ProtectionLevel.None ? string.Format("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), o.ProtectionLevel)) : "", !string.IsNullOrEmpty(o.ConfigurationName) ? string.Format("ConfigurationName = \"{0}\", ", o.ConfigurationName) : "", o.Parent.FullURI));
			code.AppendLine(string.Format("\t{0} interface I{1}", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
			code.AppendLine("\t{");
			foreach (RESTMethod m in o.ServiceOperations)
				code.AppendLine(GenerateServiceInterfaceMethod(m));
			code.AppendLine("\t}");

			//Generate the service proxy
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t[System.ServiceModel.ServiceBehaviorAttribute(AutomaticSessionShutdown = {0}, ConcurrencyMode = ConcurrencyMode.{1}, IgnoreExtensionDataObject = {2}, IncludeExceptionDetailInFaults = {3}, MaxItemsInObjectGraph = {4}, {5}TransactionTimeout = \"{6}\", UseSynchronizationContext = {7}, ValidateMustUnderstand = {8}, AddressFilterMode = AddressFilterMode.{9}, EnsureOrderedDispatch = {10}, InstanceContextMode = InstanceContextMode.{11}, ReleaseServiceInstanceOnTransactionComplete = {12}, TransactionAutoCompleteOnSessionClose = {13})]", o.SBAutomaticSessionShutdown ? "true" : "false", o.SBConcurrencyMode, o.SBIgnoreExtensionDataObject ? "true" : "false", o.SBIncludeExceptionDetailInFaults ? "true" : "false", o.SBMaxItemsInObjectGraph > 0 ? Convert.ToString(o.SBMaxItemsInObjectGraph) : "Int32.MaxValue", o.SBTransactionIsolationLevel != IsolationLevel.Unspecified ? string.Format("TransactionIsolationLevel = System.Transactions.IsolationLevel.{0}", o.SBTransactionIsolationLevel) : "", o.SBTransactionTimeout, o.SBUseSynchronizationContext ? "true" : "false", o.SBValidateMustUnderstand ? "true" : "false", o.SBAddressFilterMode, o.SBEnsureOrderedDispatch ? "true" : "false", o.SBInstanceContextMode, o.SBReleaseServiceInstanceOnTransactionComplete ? "true" : "false", o.SBTransactionAutoCompleteOnSessionClose ? "true" : "false"));
			code.AppendLine(string.Format("\t{0} abstract class {1}Base : ServerBase<{1}Base>, I{1}", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
			code.AppendLine("\t{");
			foreach (RESTMethod m in o.ServiceOperations)
				code.AppendLine(GenerateServerProxyMethod(m));
			code.AppendLine("\t}");

			return code.ToString();
		}

		#endregion

		#region - Client Interfaces -

		public static string GenerateClientCode40(RESTService o)
		{
			var code = new StringBuilder();

			//Generate the Proxy Class
			if (o.ClientDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ClientDocumentation));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} abstract partial class {1}", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
			code.AppendLine("\t{");
			code.AppendLine(string.Format("\t\tpublic {0}(string BaseURI)", o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t}");
			code.AppendLine();
			foreach (RESTMethod m in o.ServiceOperations)
				code.AppendLine(GenerateMethodClientCode40(m));
			code.AppendLine("\t}");

			return code.ToString();
		}

		public static string GenerateClientCode45(RESTService o)
		{
			var code = new StringBuilder();

			//Generate the Proxy Class
			if (o.ClientDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ClientDocumentation));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0} partial class {1}", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
			code.AppendLine("\t{");
			code.AppendLine(string.Format("\t\tpublic {0}()", o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t}");
			code.AppendLine();
			foreach (RESTMethod m in o.ServiceOperations)
				code.AppendLine(GenerateMethodClientCode45(m));
			code.AppendLine("\t}");

			return code.ToString();
		}

		#endregion

		#region - Service Interface Methods -

		public static string GenerateServiceInterfaceMethod(RESTMethod o)
		{
			var code = new StringBuilder();

			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (RESTMethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
			}
			code.AppendLine(string.Format("\t\t[OperationContract({0}{1})]", o.IsOneWay ? "IsOneWay = true, " : "", string.Format("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}", o.ProtectionLevel)));
			code.AppendLine(string.Format("\t\t[System.ServiceModel.Web.{0}(UriTemplate=\"{1}\", {2}BodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.{3}, RequestFormat = System.ServiceModel.Web.WebMessageFormat.{4}, ResponseFormat = System.ServiceModel.Web.WebMessageFormat.{5})]", o.Method == MethodRESTVerbs.GET ? "WebGet" : "WebInvoke", o.UriTemplate, o.Method != MethodRESTVerbs.GET ? string.Format("Method = \"{0}\", ", o.Method) : "", o.BodyStyle, o.RequestFormat, o.ResponseFormat));
			code.AppendFormat("\t\t{0} {1}(", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
			foreach (RESTMethodParameter op in o.Parameters)
				code.AppendFormat("{0},", GenerateMethodParameterServerCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 1, 1);
			code.AppendLine(");");

			return code.ToString();
		}

		#endregion

		#region - Server Proxy Methods -

		public static string GenerateServerProxyMethod(RESTMethod o)
		{
			var code = new StringBuilder();
			code.AppendFormat("\t\tpublic abstract {0} {1}(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
			foreach (RESTMethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");
			return code.ToString();
		}

		#endregion

		#region - Client Proxy Methods -

		public static string GenerateMethodClientSync(RESTMethod o)
		{
			var code = new StringBuilder();
			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (RESTMethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
			}
			code.AppendFormat("\t\tpublic {0} {1}(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
			foreach (RESTMethodParameter op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t}");
			return code.ToString();
		}

		public static string GenerateMethodClientCode40(RESTMethod o)
		{
			var code = new StringBuilder();

			if (o.ClientAsync)
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (RESTMethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				code.AppendFormat("\t\tpublic void {0}(", o.ServerName);
				foreach (RESTMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t}");
			}
			else
				code.Append(GenerateMethodClientSync(o));

			return code.ToString();
		}

		public static string GenerateMethodClientCode45(RESTMethod o)
		{
			var code = new StringBuilder();

			if (o.ClientAsync)
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (RESTMethodParameter mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine(string.Format("\t\t///<param name='{0}'>{1}</param>", mp.Name, mp.Documentation.Summary));
				}
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tpublic System.Threading.Tasks.Task {0}Async(", o.ServerName);
				else code.AppendFormat("\t\tpublic System.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
				foreach (RESTMethodParameter op in o.Parameters)
					code.AppendFormat("{0}{1}", GenerateMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t}");
			}
			else
				code.Append(GenerateMethodClientSync(o));

			return code.ToString();
		}

		#endregion

		#region - Method Parameters -

		public static string GenerateMethodParameterServerCode(RESTMethodParameter o)
		{
			return o.IsHidden ? "" : string.Format("{0} {1}", DataTypeGenerator.GenerateType(o.Type), o.Name);
		}

		public static string GenerateMethodParameterClientCode(RESTMethodParameter o)
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
	}
}