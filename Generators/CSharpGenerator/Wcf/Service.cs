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
using NETPath.Projects.Wcf;

namespace NETPath.Generators.CS.Wcf
{
	internal static class ServiceGenerator
	{

		public static void VerifyCode(WcfService o, Action<CompileMessage> AddMessage)
		{
			if (string.IsNullOrEmpty(o.Name))
				AddMessage(new CompileMessage("GS2000", "An service in the '" + o.Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));
			else
				if (RegExs.MatchCodeName.IsMatch(o.Name) == false)
				AddMessage(new CompileMessage("GS2001", "The service '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));
			if (o.HasClientType)
				if (RegExs.MatchCodeName.IsMatch(o.ClientType.Name) == false)
					AddMessage(new CompileMessage("GS2002", "The service '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));

			var Operations = new ObservableCollection<WcfOperation>();
			Operations.AddRange(o.ServiceOperations);
			Operations.AddRange(o.CallbackOperations);

			foreach (var m in Operations.OfType<WcfMethod>())
			{
				if (string.IsNullOrEmpty(m.ServerName))
					AddMessage(new CompileMessage("GS2004", "An method in the '" + o.Name + "' service has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
				else
				{
					if (RegExs.MatchCodeName.IsMatch(m.ServerName) == false)
						AddMessage(new CompileMessage("GS2005", "The method '" + m.ServerName + "' in the '" + o.Name + "' service contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
					if (m.ServerName == "__callback")
						AddMessage(new CompileMessage("GS2015", "The name of the method '" + m.ServerName + "' in the '" + o.Name + "' service is invalid. Please rename it.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
				}

				if (!string.IsNullOrEmpty(m.ClientName))
					if (RegExs.MatchCodeName.IsMatch(m.ClientName) == false)
						AddMessage(new CompileMessage("GS2006", "The method '" + m.ServerName + "' in the '" + o.Name + "' service contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
				if (m.ReturnType == null)
					AddMessage(new CompileMessage("GS2007", "The method '" + m.ServerName + "' in the '" + o.Name + "' service has a blank Return Type. A Return Type MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
				if (m.ReturnType.TypeMode == DataTypeMode.Namespace || m.ReturnType.TypeMode == DataTypeMode.Interface)
					AddMessage(new CompileMessage("GS2013", "The method return type '" + m.ReturnType + "' in the '" + o.Name + "' service is not a valid return type. Please specify a valid return type.", CompileMessageSeverity.ERROR, o, m, m.GetType()));

				foreach (var mp in m.Parameters)
				{
					if (string.IsNullOrEmpty(mp.Name))
						AddMessage(new CompileMessage("GS2008", "The method parameter '" + m.ServerName + "' in the '" + o.Name + "' service has a parameter with a blank name. A Parameter Name MUST be specified.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
					if (mp.Name == "__callback")
						AddMessage(new CompileMessage("GS2016", "The name of the method parameter '" + mp.Name + "' in the '" + m.ServerName + "' method is invalid. Please rename it.", CompileMessageSeverity.ERROR, o, m, m.GetType()));
				}
			}

			foreach (var p in Operations.OfType<WcfProperty>())
			{
				if (string.IsNullOrEmpty(p.ServerName))
					AddMessage(new CompileMessage("GS2010", "A property in the '" + o.Name + "' service has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o, p, p.GetType()));
				else
				{
					if (RegExs.MatchCodeName.IsMatch(p.ServerName) == false)
						AddMessage(new CompileMessage("GS2011", "The property '" + p.ServerName + "' in the '" + o.Name + "' service contains invalid characters in the Server Name.", CompileMessageSeverity.ERROR, o, p, p.GetType()));
					if (p.ServerName == "__callback")
						AddMessage(new CompileMessage("GS2015", "The name of the property '" + p.ServerName + "' in the '" + o.Name + "' service is invalid. Please rename it.", CompileMessageSeverity.ERROR, o, p, p.GetType()));
				}
				if (p.ReturnType.TypeMode == DataTypeMode.Namespace || p.ReturnType.TypeMode == DataTypeMode.Interface)
					AddMessage(new CompileMessage("GS2014", "The property value type '" + p.ReturnType + "' in the '" + o.Name + "' service is not a valid value type. Please specify a valid value type.", CompileMessageSeverity.ERROR, o, p, p.GetType()));
			}
		}

		#region - Server Interfaces -

		public static string GenerateServerCode45(WcfService o)
		{
			var code = new StringBuilder();
			if (o.ServiceDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ServiceDocumentation));
			foreach (DataType dt in o.KnownTypes)
				code.AppendLine($"\t[ServiceKnownType(typeof({dt}))]");
			code.AppendLine($"\t[System.CodeDom.Compiler.GeneratedCode(\"{Globals.ApplicationTitle}\", \"{Globals.ApplicationVersion}\")]");
			code.AppendLine($"\t[ServiceContract({(o.HasCallback || o.HasDCMOperations ? $"CallbackContract = typeof(I{o.Name}Callback), " : "")}SessionMode = System.ServiceModel.SessionMode.{System.Enum.GetName(typeof (System.ServiceModel.SessionMode), o.SessionMode)}, {(o.ProtectionLevel != System.Net.Security.ProtectionLevel.None ? $"ProtectionLevel = System.Net.Security.ProtectionLevel.{System.Enum.GetName(typeof (System.Net.Security.ProtectionLevel), o.ProtectionLevel)}, " : "")}{(!string.IsNullOrEmpty(o.ConfigurationName) ? $"ConfigurationName = \"{o.ConfigurationName}\", " : "")}{(o.HasClientType ? $"Name = \"{o.ClientType.Name}\", " : "")}Namespace = \"{o.Parent.FullURI}\")]");
			code.AppendLine($"\t{DataTypeGenerator.GenerateScope(o.Scope)} interface I{o.Name}");
			code.AppendLine("\t{");
			code.AppendLine();
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Properties");
				code.AppendLine();
			}
			foreach (var p in o.ServiceOperations.OfType<WcfProperty>())
				code.AppendLine(GeneratePropertyServerCode45(p));
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Methods");
				code.AppendLine();
			}
			foreach (var m in o.ServiceOperations.OfType<WcfMethod>())
				code.AppendLine(GenerateServiceInterfaceMethodCode45(m));
			foreach (var m in o.ServiceOperations.OfType<WcfDataChangeMethod>())
				code.AppendLine(GenerateServiceInterfaceDCM45(m, true));
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}
			code.AppendLine("\t}");

			if (o.HasCallback || o.HasDCMOperations)
			{
				//Generate the service proxy
				if (o.CallbackDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.CallbackDocumentation));
				code.AppendLine($"\t[System.CodeDom.Compiler.GeneratedCode(\"{Globals.ApplicationTitle}\", \"{Globals.ApplicationVersion}\")]");
				code.AppendLine(string.Format("\t[System.ServiceModel.ServiceBehaviorAttribute(AutomaticSessionShutdown = {0}, ConcurrencyMode = ConcurrencyMode.{1}, IgnoreExtensionDataObject = {2}, IncludeExceptionDetailInFaults = {3}, MaxItemsInObjectGraph = {4}, {5}{6}UseSynchronizationContext = {7}, ValidateMustUnderstand = {8}, EnsureOrderedDispatch = {10}, InstanceContextMode = InstanceContextMode.{11}, {12}{13}AddressFilterMode = AddressFilterMode.{9})]", o.SBAutomaticSessionShutdown ? "true" : "false", o.SBConcurrencyMode, o.SBIgnoreExtensionDataObject ? "true" : "false", o.SBIncludeExceptionDetailInFaults ? "true" : "false", o.SBMaxItemsInObjectGraph > 0 ? Convert.ToString(o.SBMaxItemsInObjectGraph) : "Int32.MaxValue", o.SBTransactionIsolationLevel != IsolationLevel.Unspecified ? $"TransactionIsolationLevel = System.Transactions.IsolationLevel.{o.SBTransactionIsolationLevel}, " : "", o.SBTransactionTimeout.Ticks != 0L ? $"TransactionTimeout = \"{o.SBTransactionTimeout}\", " : "", o.SBUseSynchronizationContext ? "true" : "false", o.SBValidateMustUnderstand ? "true" : "false", o.SBAddressFilterMode, o.SBEnsureOrderedDispatch ? "true" : "false", o.SBInstanceContextMode, o.SBReleaseServiceInstanceOnTransactionComplete ? string.Format("ReleaseServiceInstanceOnTransactionComplete = true, ") : "", o.SBTransactionAutoCompleteOnSessionClose ? string.Format("TransactionAutoCompleteOnSessionClose = true, ") : ""));
				code.AppendLine(string.Format("\t{0} abstract class {1}Base : ServerDuplexBase<{1}Base, {1}Callback, I{1}Callback>, I{1}", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
				code.AppendLine("\t{");
				code.AppendLine();
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#region Properties");
					code.AppendLine();
				}
				foreach (var p in o.ServiceOperations.OfType<WcfProperty>())
					code.AppendLine(GenerateServerProxyPropertyCode(p));
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#endregion");
					code.AppendLine();
				}
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#region Methods");
					code.AppendLine();
				}
				foreach (var m in o.ServiceOperations.OfType<WcfMethod>())
					code.AppendLine(GenerateServerProxyMethodCode45(m));
				foreach (var m in o.ServiceOperations.OfType<WcfDataChangeMethod>())
					code.AppendLine(GenerateServiceImplementationDCM45(m, true));
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#endregion");
					code.AppendLine();
				}
				code.AppendLine("\t}");

				//Generate the callback interface
				if (o.CallbackDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.CallbackDocumentation));
				foreach (DataType dt in o.KnownTypes)
					code.AppendLine($"\t[ServiceKnownType(typeof({dt}))]");
				code.AppendLine($"\t[System.CodeDom.Compiler.GeneratedCode(\"{Globals.ApplicationTitle}\", \"{Globals.ApplicationVersion}\")]");
				code.AppendLine($"\t{DataTypeGenerator.GenerateScope(o.Scope)} interface I{o.Name}Callback");
				code.AppendLine("\t{");
				code.AppendLine();
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#region Properties");
					code.AppendLine();
				}
				foreach (var p in o.ServiceOperations.OfType<WcfProperty>())
					code.AppendLine(GeneratePropertyInterfaceCode45(p));
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#endregion");
					code.AppendLine();
				}
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#region Methods");
					code.AppendLine();
				}
				foreach (var m in o.CallbackOperations.OfType<WcfCallback>())
					code.AppendLine(GenerateCallbackInterfaceMethodCode45(m));
				foreach (var m in o.ServiceOperations.OfType<WcfDataChangeMethod>())
					code.AppendLine(GenerateCallbackInterfaceDCM45(m, true));
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#endregion");
					code.AppendLine();
				}
				code.AppendLine("\t}");

				//Generate the callback facade implementation
				if (o.CallbackDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.CallbackDocumentation));
				code.AppendLine($"\t[System.CodeDom.Compiler.GeneratedCode(\"{Globals.ApplicationTitle}\", \"{Globals.ApplicationVersion}\")]");
				code.AppendLine(string.Format("\t{0} partial class {1}Callback : ServerCallbackBase<I{1}Callback>, I{1}Callback", DataTypeGenerator.GenerateScope(o.Scope), o.Name));
				code.AppendLine("\t{");
				code.AppendLine();
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#region Constructors");
					code.AppendLine();
				}
				code.AppendLine($"\t\tpublic {o.Name}Callback()");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine(string.Format("\t\tpublic {0}Callback(I{0}Callback callback)", o.Name));
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\t__callback = callback;");
				code.AppendLine("\t\t}");
				code.AppendLine();
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#endregion");
					code.AppendLine();
				}
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#region Properties");
					code.AppendLine();
				}
				foreach (var p in o.ServiceOperations.OfType<WcfProperty>())
					code.AppendLine(GeneratePropertyClientCode(p));
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#endregion");
					code.AppendLine();
				}
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#region Methods");
					code.AppendLine();
				}
				foreach (var m in o.CallbackOperations.OfType<WcfCallback>())
					code.AppendLine(GenerateCallbackMethodProxy45(m));
				foreach (var m in o.ServiceOperations.OfType<WcfDataChangeMethod>())
					code.AppendLine(GenerateCallbackImplementationDCM45(m, true));
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#endregion");
					code.AppendLine();
				}
				code.AppendLine("\t}");
			}

			return code.ToString();
		}

		#endregion

		#region - Client Interfaces -

		public static string GenerateClientCode45(WcfService o, ProjectGenerationFramework GenerationTarget)
		{
			var code = new StringBuilder();

			//Generate the Client interface
			if (o.ServiceDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.ServiceDocumentation));
			foreach (DataType dt in o.KnownTypes)
				code.AppendLine($"\t[ServiceKnownType(typeof({dt}))]");
			code.AppendLine($"\t[System.CodeDom.Compiler.GeneratedCode(\"{Globals.ApplicationTitle}\", \"{Globals.ApplicationVersion}\")]");
			code.AppendLine(GenerationTarget == ProjectGenerationFramework.WINRT ? $"\t[ServiceContract({(o.HasCallback || o.HasDCMOperations ? $"CallbackContract = typeof(I{o.Name}Callback), " : "")}{(!string.IsNullOrEmpty(o.ConfigurationName) ? $"ConfigurationName = \"{o.ConfigurationName}\", " : "")}{(o.HasClientType ? $"Name = \"{o.ClientType.Name}\", " : "")}Namespace = \"{o.Parent.FullURI}\")]" : $"\t[ServiceContract({(o.HasCallback || o.HasDCMOperations ? $"CallbackContract = typeof(I{o.Name}Callback), " : "")}SessionMode = System.ServiceModel.SessionMode.{System.Enum.GetName(typeof (System.ServiceModel.SessionMode), o.SessionMode)}, {(o.ProtectionLevel != System.Net.Security.ProtectionLevel.None ? $"ProtectionLevel = System.Net.Security.ProtectionLevel.{System.Enum.GetName(typeof (System.Net.Security.ProtectionLevel), o.ProtectionLevel)}, " : "")}{(!string.IsNullOrEmpty(o.ConfigurationName) ? $"ConfigurationName = \"{o.ConfigurationName}\", " : "")}{(o.HasClientType ? $"Name = \"{o.ClientType.Name}\", " : "")}Namespace = \"{o.Parent.FullURI}\")]");
			code.AppendLine($"\t{(o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope))} interface I{(o.HasClientType ? o.ClientType.Name : o.Name)}");
			code.AppendLine("\t{");
			code.AppendLine();
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Properties");
				code.AppendLine();
			}
			foreach (var p in o.ServiceOperations.OfType<WcfProperty>())
				code.AppendLine(GeneratePropertyInterfaceCode45(p));
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Methods");
				code.AppendLine();
			}
			foreach (var m in o.ServiceOperations.OfType<WcfMethod>())
				code.AppendLine(GenerateClientInterfaceMethodCode45(m));
			foreach (var m in o.ServiceOperations.OfType<WcfDataChangeMethod>())
				code.AppendLine(GenerateServiceInterfaceDCM45(m, false));
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate Channel Interface
			code.AppendLine($"\t[System.CodeDom.Compiler.GeneratedCode(\"{Globals.ApplicationTitle}\", \"{Globals.ApplicationVersion}\")]");
			code.AppendLine($"\t{(o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope))} interface I{(o.HasClientType ? o.ClientType.Name : o.Name)}Channel : I{(o.HasClientType ? o.ClientType.Name : o.Name)}, System.ServiceModel.IClientChannel");
			code.AppendLine("\t{");
			code.AppendLine("\t}");
			code.AppendLine();
			//Generate the Proxy Class
			code.AppendLine($"\t[System.CodeDom.Compiler.GeneratedCode(\"{Globals.ApplicationTitle}\", \"{Globals.ApplicationVersion}\")]");
			code.AppendLine(string.Format("\t{0} partial class {1}Proxy : {2}, I{1}", o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name, o.HasDCMOperations || o.HasCallbackOperations ? string.Format("System.ServiceModel.ClientDuplexBaseEx<{0}Proxy, I{0}>", o.HasClientType ? o.ClientType.Name : o.Name) : string.Format("System.ServiceModel.ClientBaseEx<{0}Proxy, I{0}>", o.HasClientType ? o.ClientType.Name : o.Name)));
			code.AppendLine("\t{");
			code.AppendLine();
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Constructors");
				code.AppendLine();
			}
			if (o.HasCallbackOperations || o.HasDCMOperations)
			{
				code.AppendLine($"\t\tpublic {(o.HasClientType ? o.ClientType.Name : o.Name)}Proxy()");
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = Guid.NewGuid();");
				code.AppendLine("\t\t}");
				code.AppendLine();
				if (o.HasDCMOperations)
				{
					code.AppendLine($"\t\tpublic {(o.HasClientType ? o.ClientType.Name : o.Name)}Proxy(Guid ClientID)");
					code.AppendLine("\t\t{");
					code.AppendLine("\t\t\tbase.ClientID = ClientID;");
					code.AppendLine("\t\t}");
					code.AppendLine();
				}
				code.AppendLine($"\t\tpublic {(o.HasClientType ? o.ClientType.Name : o.Name)}Proxy({(o.HasDCMOperations ? "Guid ClientID, " : "")}object callback, string endpointConfigurationName) : base(callback, endpointConfigurationName)");
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = ClientID;");
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine($"\t\tpublic {(o.HasClientType ? o.ClientType.Name : o.Name)}Proxy({(o.HasDCMOperations ? "Guid ClientID, " : "")}object callback, string endpointConfigurationName, string remoteAddress) : base(callback, endpointConfigurationName, remoteAddress)");
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = ClientID;");
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine($"\t\tpublic {(o.HasClientType ? o.ClientType.Name : o.Name)}Proxy({(o.HasDCMOperations ? "Guid ClientID, " : "")}object callback, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(callback, endpointConfigurationName, remoteAddress)");
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = ClientID;");
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine($"\t\tpublic {(o.HasClientType ? o.ClientType.Name : o.Name)}Proxy({(o.HasDCMOperations ? "Guid ClientID, " : "")}object callback, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(callback, binding, remoteAddress)");
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = ClientID;");
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine($"\t\tpublic {(o.HasClientType ? o.ClientType.Name : o.Name)}Proxy({(o.HasDCMOperations ? "Guid ClientID, " : "")}object callback, System.ServiceModel.Description.ServiceEndpoint endpoint) : base(callback, endpoint)");
				code.AppendLine("\t\t{");
				if (o.HasDCMOperations) code.AppendLine("\t\t\tbase.ClientID = ClientID;");
				code.AppendLine("\t\t}");
				code.AppendLine();
			}
			else
			{
				code.AppendLine($"\t\tpublic {(o.HasClientType ? o.ClientType.Name : o.Name)}Proxy()");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine($"\t\tpublic {(o.HasClientType ? o.ClientType.Name : o.Name)}Proxy({(o.HasDCMOperations ? "Guid ClientID, " : "")}string endpointConfigurationName) : base(endpointConfigurationName)");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine($"\t\tpublic {(o.HasClientType ? o.ClientType.Name : o.Name)}Proxy({(o.HasDCMOperations ? "Guid ClientID, " : "")}string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine($"\t\tpublic {(o.HasClientType ? o.ClientType.Name : o.Name)}Proxy({(o.HasDCMOperations ? "Guid ClientID, " : "")}string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t}");
				code.AppendLine();
				code.AppendLine($"\t\tpublic {(o.HasClientType ? o.ClientType.Name : o.Name)}Proxy({(o.HasDCMOperations ? "Guid ClientID, " : "")}System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress)");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t}");
				code.AppendLine();
			}
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Host Configuration");
				code.AppendLine();
			}
			WcfHost h = (o.Parent.Owner as WcfProject)?.Namespace.GetServiceHost(o);
			if (h != null)
				code.Append(HostGenerator.GenerateClientCode45(h, o.HasDCMOperations));
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\tregion Properties");
				code.AppendLine();
			}
			foreach (var p in o.ServiceOperations.OfType<WcfProperty>())
				code.AppendLine(GeneratePropertyClientCode(p));
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Methods");
				code.AppendLine();
			}
			foreach (var m in o.ServiceOperations.OfType<WcfMethod>())
				code.AppendLine(GenerateServiceMethodProxy45(m));
			foreach (var m in o.ServiceOperations.OfType<WcfDataChangeMethod>())
				code.AppendLine(GenerateServiceImplementationDCM45(m, false));
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}
			code.AppendLine("\t}");
			//Generate Callback Interface (if any)
			if (o.HasCallback || o.HasDCMOperations)
			{
				if (o.CallbackDocumentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.CallbackDocumentation));
				foreach (DataType dt in o.KnownTypes)
					code.AppendLine($"\t[ServiceKnownType(typeof({dt}))]");
				code.AppendLine($"\t[System.CodeDom.Compiler.GeneratedCode(\"{Globals.ApplicationTitle}\", \"{Globals.ApplicationVersion}\")]");
				code.AppendFormat("\t{0} interface I{1}Callback{2}", o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope), o.Name, Environment.NewLine);
				code.AppendLine("\t{");
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#region Properties");
					code.AppendLine();
				}
				foreach (var p in o.ServiceOperations.OfType<WcfProperty>())
					code.AppendLine(GeneratePropertyInterfaceCode45(p));
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#endregion");
					code.AppendLine();
				}
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#region Methods");
					code.AppendLine();
				}
				foreach (var m in o.CallbackOperations.OfType<WcfCallback>())
					code.AppendLine(GenerateClientInterfaceCallbackCode45(m));
				foreach (var m in o.ServiceOperations.OfType<WcfDataChangeMethod>())
					code.AppendLine(GenerateCallbackInterfaceDCM45(m, false));
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#endregion");
					code.AppendLine();
				}
				code.AppendLine("\t}");
				code.AppendLine();
				code.AppendLine($"\t[System.CodeDom.Compiler.GeneratedCode(\"{Globals.ApplicationTitle}\", \"{Globals.ApplicationVersion}\")]");
				code.AppendLine($"\t[System.ServiceModel.CallbackBehaviorAttribute(AutomaticSessionShutdown = {(o.CBAutomaticSessionShutdown ? "true" : "false")}, ConcurrencyMode = ConcurrencyMode.{o.CBConcurrencyMode}, IgnoreExtensionDataObject = {(o.CBIgnoreExtensionDataObject ? "true" : "false")}, IncludeExceptionDetailInFaults = {(o.CBIncludeExceptionDetailInFaults ? "true" : "false")}, MaxItemsInObjectGraph = {(o.CBMaxItemsInObjectGraph > 0 ? Convert.ToString(o.CBMaxItemsInObjectGraph) : "Int32.MaxValue")}, {(o.CBTransactionIsolationLevel != IsolationLevel.Unspecified ? $"TransactionIsolationLevel = System.Transactions.IsolationLevel.{o.CBTransactionIsolationLevel}, " : "")}{(o.CBTransactionTimeout.Ticks != 0L ? $"TransactionTimeout = \"{o.CBTransactionTimeout}\", " : "")}UseSynchronizationContext = {(o.CBUseSynchronizationContext ? "true" : "false")}, ValidateMustUnderstand = {(o.CBValidateMustUnderstand ? "true" : "false")})]");
				code.AppendLine(string.Format("\t{0} abstract class {1}Callback : I{1}Callback", o.HasClientType ? DataTypeGenerator.GenerateScope(o.ClientType.Scope) : DataTypeGenerator.GenerateScope(o.Scope), o.HasClientType ? o.ClientType.Name : o.Name));
				code.AppendLine("\t{");
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#region Properties");
					code.AppendLine();
				}
				foreach (var p in o.ServiceOperations.OfType<WcfProperty>())
					code.AppendLine(GeneratePropertyServerCode45(p));
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#endregion");
					code.AppendLine();
				}
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#region Methods");
					code.AppendLine();
				}
				foreach (var m in o.CallbackOperations.OfType<WcfCallback>())
					code.AppendLine(GenerateCallbackClientMethodCode45(m));
				foreach (var m in o.ServiceOperations.OfType<WcfDataChangeMethod>())
					code.AppendLine(GenerateCallbackImplementationDCM45(m, false));
				if (o.Parent.Owner.GenerateRegions)
				{
					code.AppendLine("\t\t#endregion");
					code.AppendLine();
				}
				code.AppendLine("\t}");
			}

			return code.ToString();
		}

		#endregion

		#region - Service/Callback Interface Methods -

		public static string GenerateServiceInterfaceSyncMethod(WcfMethod o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (var mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
			}
			code.AppendLine($"\t\t[OperationContract({string.Format($"ProtectionLevel = System.Net.Security.ProtectionLevel.{o.ProtectionLevel}", o.ProtectionLevel)}{(o.Owner.SessionMode == SessionMode.Required ? o.IsInitiating ? ", IsInitiating = true" : ", IsInitiating = false" : "")}{(o.Owner.SessionMode == SessionMode.Required ? o.IsTerminating ? ", IsTerminating = true" : ", IsTerminating = false" : "")}{(o.IsOneWay ? ", IsOneWay = true" : "")})]");
			code.AppendFormat("\t\t{0} {1}(", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
			foreach (var op in o.Parameters)
				code.AppendFormat("{0},", GenerateWcfMethodParameterServerCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 1, 1);
			code.AppendLine(");");

			return code.ToString();
		}

		public static string GenerateServiceInterfaceMethodCode45(WcfMethod o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.UseServerAwaitPattern)
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (var mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				}
				code.AppendLine($"\t\t[OperationContract({string.Format($"ProtectionLevel = System.Net.Security.ProtectionLevel.{o.ProtectionLevel}", o.ProtectionLevel)}{(o.Owner.SessionMode == SessionMode.Required ? o.IsInitiating ? ", IsInitiating = true" : ", IsInitiating = false" : "")}{(o.Owner.SessionMode == SessionMode.Required ? o.IsTerminating ? ", IsTerminating = true" : ", IsTerminating = false" : "")}{(o.IsOneWay ? ", IsOneWay = true" : "")})]");
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tSystem.Threading.Tasks.Task {0}Async(", o.ServerName);
				else code.AppendFormat("\t\tSystem.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
				foreach (var op in o.Parameters)
					code.AppendFormat("{0},", GenerateWcfMethodParameterServerCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 1, 1);
				code.AppendLine(");");
			}
			else
				code.Append(GenerateServiceInterfaceSyncMethod(o));

			return code.ToString();
		}

		public static string GenerateCallbackInterfaceSyncMethod(WcfCallback o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (var mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
			}
			code.AppendLine($"\t\t[OperationContract({string.Format($"ProtectionLevel = System.Net.Security.ProtectionLevel.{o.ProtectionLevel}", o.ProtectionLevel)}{(o.IsOneWay ? ", IsOneWay = true" : "")})]");
			code.AppendFormat("\t\t{0} {1}(", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
			foreach (var op in o.Parameters)
				code.AppendFormat("{0},", GenerateWcfMethodParameterServerCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 1, 1);
			code.AppendLine(");");

			return code.ToString();
		}

		public static string GenerateCallbackInterfaceMethodCode45(WcfCallback o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.UseServerAwaitPattern)
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (var mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				}
				code.AppendLine($"\t\t[OperationContract({string.Format($"ProtectionLevel = System.Net.Security.ProtectionLevel.{o.ProtectionLevel}", o.ProtectionLevel)}{(o.IsOneWay ? ", IsOneWay = true" : "")})]");
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tSystem.Threading.Tasks.Task {0}Async(", o.ServerName);
				else code.AppendFormat("\t\tSystem.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName);
				foreach (var op in o.Parameters)
					code.AppendFormat("{0},", GenerateWcfMethodParameterServerCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 1, 1);
				code.AppendLine(");");
			}
			else
				code.Append(GenerateCallbackInterfaceSyncMethod(o));

			return code.ToString();
		}

		#endregion

		#region - Server Proxy Methods -

		public static string GenerateServerProxyPropertyCode(WcfProperty o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.AppendFormat("\t\tpublic abstract {0} {1} {{ get; {2}}}", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName, o.IsReadOnly ? "" : "set; ");

			return code.ToString();
		}

		public static string GenerateServerProxySyncMethod(WcfMethod o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation, true));
				foreach (var mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
			}
			if (o.HasOperationBehavior) code.AppendLine($"\t\t[OperationBehavior({(!o.OBAutoDisposeParameters ? "AutoDisposeParameters = false, " : "")}{(o.OBImpersonation != ImpersonationOption.NotAllowed ? $"Impersonation = ImpersonationOption.{o.OBImpersonation}, " : "")}{(o.OBReleaseInstanceMode != ReleaseInstanceMode.None ? $"ReleaseInstanceMode = ReleaseInstanceMode.{o.OBReleaseInstanceMode}, " : "")}{(!o.OBTransactionAutoComplete ? "TransactionAutoComplete = false, " : "")}TransactionScopeRequired = {(o.OBTransactionScopeRequired ? "true" : "false")})]");
			if (o.HasOperationBehavior && o.OBTransactionFlowMode != TransactionFlowMode.None) code.AppendLine($"\t\t[TransactionFlow(TransactionFlowOption.{o.OBTransactionFlowMode})]");
			code.AppendFormat("\t\tpublic abstract {0} {1}(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
			foreach (var op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");
			return code.ToString();
		}

		public static string GenerateServerProxyMethodCode45(WcfMethod o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.UseServerAwaitPattern)
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (var mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				}
				if (o.HasOperationBehavior) code.AppendLine($"\t\t[OperationBehavior({(!o.OBAutoDisposeParameters ? "AutoDisposeParameters = false, " : "")}{(o.OBImpersonation != ImpersonationOption.NotAllowed ? $"Impersonation = ImpersonationOption.{o.OBImpersonation}, " : "")}{(o.OBReleaseInstanceMode != ReleaseInstanceMode.None ? $"ReleaseInstanceMode = ReleaseInstanceMode.{o.OBReleaseInstanceMode}, " : "")}{(!o.OBTransactionAutoComplete ? "TransactionAutoComplete = false, " : "")}TransactionScopeRequired = {(o.OBTransactionScopeRequired ? "true" : "false")})]");
				if (o.HasOperationBehavior && o.OBTransactionFlowMode != TransactionFlowMode.None) code.AppendLine($"\t\t[TransactionFlow(TransactionFlowOption.{o.OBTransactionFlowMode})]");
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tpublic abstract System.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				else code.AppendFormat("\t\tpublic abstract System.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
				foreach (var op in o.Parameters)
					code.AppendFormat("{0}{1}", GenerateWcfMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(");");
			}
			else
				code.Append(GenerateServerProxySyncMethod(o));

			return code.ToString();
		}

		#endregion

		#region - Server Callback Methods -

		public static string GenerateCallbackMethodProxy(WcfCallback o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (var mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
			}
			code.AppendFormat("\t\tpublic {0} {1}(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
			foreach (var op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			code.AppendFormat("\t\t\t{0}__callback.{1}(", o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : "", o.HasClientType ? o.ClientName : o.ServerName);
			foreach (var op in o.Parameters)
				code.AppendFormat("{0}, ", op.Name);
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");
			code.AppendLine("\t\t}");
			return code.ToString();
		}

		public static string GenerateCallbackMethodProxy45(WcfCallback o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.UseServerSyncPattern)
				code.Append(GenerateCallbackMethodProxy(o));

			if (o.UseServerAwaitPattern)
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (var mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				}
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tpublic System.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				else code.AppendFormat("\t\tpublic System.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
				foreach (var op in o.Parameters)
					code.AppendFormat("{0}{1}", GenerateWcfMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendFormat("\t\t\treturn __callback.{0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (var op in o.Parameters)
					code.AppendFormat("{0}{1}", op.Name, o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(");");
				code.AppendLine("\t\t}");
			}

			return code.ToString();
		}

		#endregion

		#region - Client Interface Methods -

		public static string GenerateClientInterfaceSyncMethod(WcfMethod o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (var mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
			}
			code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/I{1}/{2}\")]" : "\t\t[OperationContract(Action = \"{0}/I{1}/{2}\", ReplyAction = \"{0}/I{1}/{2}Response\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
			code.AppendFormat("\t\t{0} {1}(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
			foreach (var op in o.Parameters)
				code.AppendFormat("{0}{1}", GenerateWcfMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
			code.AppendLine(");");
			return code.ToString();
		}

		public static string GenerateClientInterfaceMethodCode45(WcfMethod o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.UseClientSyncPattern)
				code.Append(GenerateClientInterfaceSyncMethod(o));

			if (o.UseClientAwaitPattern)
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (var mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				}
				code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/I{1}/{2}\")]" : "\t\t[OperationContract(Action = \"{0}/I{1}/{2}\", ReplyAction = \"{0}/I{1}/{2}Response\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tSystem.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				else code.AppendFormat("\t\tSystem.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
				foreach (var op in o.Parameters)
					code.AppendFormat("{0}{1}", GenerateWcfMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(");");
			}

			return code.ToString();
		}

		public static string GenerateClientInterfaceSyncCallback(WcfCallback o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (var mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
			}
			code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/I{1}/{2}\")]" : "\t\t[OperationContract(Action = \"{0}/I{1}/{2}\", ReplyAction = \"{0}/I{1}/{2}Response\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
			code.AppendFormat("\t\t{0} {1}(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
			foreach (var op in o.Parameters)
				code.AppendFormat("{0}{1}", GenerateWcfMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
			code.AppendLine(");");
			return code.ToString();
		}

		public static string GenerateClientInterfaceCallbackCode45(WcfCallback o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.UseClientSyncPattern || o.Owner.CBUseSynchronizationContext)
				code.Append(GenerateClientInterfaceSyncCallback(o));

			if (o.UseClientAwaitPattern && !o.Owner.CBUseSynchronizationContext)
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (var mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				}
				code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/I{1}/{2}\")]" : "\t\t[OperationContract(Action = \"{0}/I{1}/{2}\", ReplyAction = \"{0}/I{1}/{2}Response\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tSystem.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				else code.AppendFormat("\t\tSystem.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
				foreach (var op in o.Parameters)
					code.AppendFormat("{0}{1}", GenerateWcfMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(");");
			}

			return code.ToString();
		}

		#endregion

		#region - Client Proxy Methods -

		public static string GenerateServiceMethodProxy(WcfMethod o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (var mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
			}
			code.AppendFormat("\t\tpublic {0} {1}(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
			foreach (var op in o.Parameters)
				code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(")");
			code.AppendLine("\t\t{");
			if (o.IsInitiating) code.AppendLine("\t\t\tbase.Initialize();");
			code.AppendFormat("\t\t\t{0}base.Channel.{1}(", o.ReturnType.Primitive != PrimitiveTypes.Void && !o.IsTerminating ? "return " : o.ReturnType.Primitive != PrimitiveTypes.Void && o.IsTerminating ? "var __t = " : "", o.HasClientType ? o.ClientName : o.ServerName);
			foreach (var op in o.Parameters)
				code.AppendFormat("{0}, ", op.Name);
			if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
			code.AppendLine(");");
			if (o.IsTerminating) code.AppendLine("\t\t\tbase.Terminate();");
			if (o.IsTerminating && o.ReturnType.Primitive != PrimitiveTypes.Void) code.AppendLine("\t\t\treturn __t;");
			code.AppendLine("\t\t}");
			return code.ToString();
		}

		public static string GenerateServiceMethodProxy45(WcfMethod o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.UseClientSyncPattern || o.IsTerminating)
				code.Append(GenerateServiceMethodProxy(o));

			if (o.UseClientAwaitPattern && !o.IsTerminating)
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (var mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				}
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tpublic System.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				else code.AppendFormat("\t\tpublic System.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
				foreach (var op in o.Parameters)
					code.AppendFormat("{0}{1}", GenerateWcfMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (o.IsInitiating) code.AppendLine("\t\t\tbase.Initialize();");
				code.AppendFormat("\t\t\treturn base.Channel.{0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (var op in o.Parameters)
					code.AppendFormat("{0}{1}", op.Name, o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(");");
				code.AppendLine("\t\t}");
			}

			return code.ToString();
		}

		#endregion

		#region - Client Callback Methods -

		public static string GenerateClientCallbackSyncMethod(WcfCallback o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.Documentation != null)
			{
				code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
				foreach (var mp in o.Parameters.Where(mp => mp.Documentation != null))
					code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
			}
			code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/I{1}/{2}Async\")]" : "\t\t[OperationContract(Action = \"{0}/I{1}/{2}Async\", ReplyAction = \"{0}/I{1}/{2}AsyncResponse\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
			code.AppendFormat("\t\tpublic abstract {0} {1}(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
			foreach (var op in o.Parameters)
				code.AppendFormat("{0}{1}", GenerateWcfMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
			code.AppendLine(");");
			return code.ToString();
		}

		public static string GenerateCallbackClientMethodCode45(WcfCallback o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			if (o.UseClientSyncPattern || o.Owner.CBUseSynchronizationContext)
				code.Append(GenerateClientCallbackSyncMethod(o));

			if (o.UseClientAwaitPattern && !o.Owner.CBUseSynchronizationContext)
			{
				if (o.Documentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
					foreach (var mp in o.Parameters.Where(mp => mp.Documentation != null))
						code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				}
				code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/I{1}/{2}Async\")]" : "\t\t[OperationContract(Action = \"{0}/I{1}/{2}Async\", ReplyAction = \"{0}/I{1}/{2}AsyncResponse\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
				if (o.ReturnType.TypeMode == DataTypeMode.Primitive && o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tpublic abstract System.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				else code.AppendFormat("\t\tpublic abstract System.Threading.Tasks.Task<{0}> {1}Async(", o.ReturnType.HasClientType ? DataTypeGenerator.GenerateType(o.ReturnType.ClientType) : DataTypeGenerator.GenerateType(o.ReturnType), o.HasClientType ? o.ClientName : o.ServerName);
				foreach (var op in o.Parameters)
					code.AppendFormat("{0}{1}", GenerateWcfMethodParameterClientCode(op), o.Parameters.IndexOf(op) != (o.Parameters.Count() - 1) ? ", " : "");
				code.AppendLine(");");
			}

			return code.ToString();
		}

		#endregion

		#region - Service DRE -

		public static string GenerateServiceInterfaceDCM45(WcfDataChangeMethod o, bool IsServer)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			var dcmtype = o.ReturnType as WcfData;
			if (dcmtype == null) return "";

			if (o.GenerateGetFunction)
			{
				if (o.GetDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.GetDocumentation));
					foreach (var mp in o.GetParameters.Where(mp => mp.Documentation != null))
						code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				}
				var x = new WcfMethod($"Get{dcmtype.Name}DRE", o.Owner) { Parameters = o.GetParameters, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = o.ReturnType };
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(x) : GenerateClientInterfaceMethodCode45(x));
			}
			if (o.GenerateNewDeleteFunction)
			{
				if (o.NewDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.NewDocumentation));
					foreach (var mp in o.NewParameters.Where(mp => mp.Documentation != null))
						code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				}
				var xp = new ObservableCollection<WcfMethodParameter>(o.NewParameters);
				var x = new WcfMethod($"New{dcmtype.Name}DRE", o.Owner) { Parameters = xp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				xp.Insert(0, new WcfMethodParameter(dcmtype, "DREData", o.Owner, x));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(x) : GenerateClientInterfaceMethodCode45(x));
				if (o.DeleteDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.DeleteDocumentation));
					foreach (var mp in o.DeleteParameters.Where(mp => mp.Documentation != null))
						code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				}
				var yp = new ObservableCollection<WcfMethodParameter>(o.DeleteParameters);
				var y = new WcfMethod($"Delete{dcmtype.Name}DRE", o.Owner) { Parameters = yp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				yp.Insert(0, new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(y) : GenerateClientInterfaceMethodCode45(y));
			}
			if (o.GenerateOpenCloseFunction)
			{
				if (o.OpenDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.OpenDocumentation));
					foreach (var mp in o.OpenParameters.Where(mp => mp.Documentation != null))
						code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				}
				var xp = new ObservableCollection<WcfMethodParameter>(o.OpenParameters);
				var x = new WcfMethod($"Open{dcmtype.Name}DRE", o.Owner) { Parameters = xp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = dcmtype };
				xp.Insert(0, new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, x));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(x) : GenerateClientInterfaceMethodCode45(x));
				if (o.CloseDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.CloseDocumentation));
					foreach (var mp in o.CloseParameters.Where(mp => mp.Documentation != null))
						code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				}
				var yp = new ObservableCollection<WcfMethodParameter>(o.CloseParameters);
				var y = new WcfMethod($"Close{dcmtype.Name}DRE", o.Owner) { Parameters = yp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				yp.Insert(0, new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(y) : GenerateClientInterfaceMethodCode45(y));
			}

			foreach (var de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && (a.DREUpdateMode == DataUpdateMode.Immediate || a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
			{
				DataType edt = de.DataType;

				var tp = new ObservableCollection<WcfMethodParameter>();
				var x = new WcfMethod($"Update{dcmtype.Name}{de.DataName}DRE", o.Owner) { Parameters = tp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));

				if (edt.TypeMode == DataTypeMode.Collection) tp.Add(new WcfMethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = edt.CollectionGenericType } }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Dictionary) tp.Add(new WcfMethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType } }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Queue) { continue; }
				else if (edt.TypeMode == DataTypeMode.Stack) { continue; }
				else tp.Add(new WcfMethodParameter(edt, "ChangedValue", o.Owner, x));

				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(x) : GenerateClientInterfaceMethodCode45(x));
			}

			if (dcmtype.Elements.Any(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch))
			{
				var tp = new ObservableCollection<WcfMethodParameter>();
				var x = new WcfMethod($"BatchUpdate{dcmtype.Name}DRE", o.Owner) { Parameters = tp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));
				foreach (var de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode != DataTypeMode.Collection && a.DataType.TypeMode != DataTypeMode.Dictionary))
					tp.Add(new WcfMethodParameter(new DataType("CMDItemValue", DataTypeMode.Collection, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = de.DataType }, $"{(de.HasClientType ? de.ClientName : de.DataName)}Delta", o.Owner, x));
				//foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Collection))
				//{
				//	DataType edt = de.DataType;
				//	tp.Add(new WcfMethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = edt.CollectionGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				//}
				//foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Dictionary))
				//{
				//	DataType edt = de.DataType;
				//	tp.Add(new WcfMethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				//}

				code.Append(IsServer ? GenerateServiceInterfaceMethodCode45(x) : GenerateClientInterfaceMethodCode45(x));
			}

			code.AppendLine();

			return code.ToString();
		}

		public static string GenerateServiceImplementationDCM45(WcfDataChangeMethod o, bool IsServer)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			var dcmtype = o.ReturnType as WcfData;
			if (dcmtype == null) return "";

			if (IsServer)
			{
				if (o.GenerateGetFunction)
				{
					if (o.UseServerAwaitPattern)
					{
						code.Append(string.Format("\t\tpublic abstract System.Threading.Tasks.Task<{1}> Get{0}DREAsync(", dcmtype.Name, o.ReturnType.HasClientType ? o.ReturnType.ClientType : o.ReturnType));
						foreach (WcfMethodParameter mp in o.GetParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.GetParameters.IndexOf(mp) < o.GetParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(");");
					}
					if (o.UseClientSyncPattern)
					{
						code.Append(string.Format("\t\tpublic abstract {1} Get{0}DRE(", dcmtype.Name, o.ReturnType.HasClientType ? o.ReturnType.ClientType : o.ReturnType));
						foreach (WcfMethodParameter mp in o.GetParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.GetParameters.IndexOf(mp) < o.GetParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(");");
					}
				}
				if (o.GenerateNewDeleteFunction)
				{
					if (o.UseServerAwaitPattern)
					{
						code.Append($"\t\tpublic virtual System.Threading.Tasks.Task New{dcmtype.Name}DREAsync({DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)} DREData{(o.NewParameters.Count != 0 ? ", " : "")}");
						foreach (WcfMethodParameter mp in o.NewParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.NewParameters.IndexOf(mp) < o.NewParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(")");
						code.AppendLine("\t\t{");
						code.AppendLine($"\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(() => {{ var t = {DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)}.Register(ClientID, DREData); {(dcmtype.HasEntity ? "if (t != null) t.Register();" : "")}}}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
						code.AppendLine("\t\t}");
						code.Append($"\t\tpublic virtual System.Threading.Tasks.Task Delete{dcmtype.Name}DREAsync(Guid DREDataID{(o.NewParameters.Count != 0 ? ", " : "")}");
						foreach (WcfMethodParameter mp in o.DeleteParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.DeleteParameters.IndexOf(mp) < o.DeleteParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(")");
						code.AppendLine("\t\t{");
						code.AppendLine($"\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(() => {{ var t = {DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)}.Unregister(ClientID, DREDataID); {(dcmtype.HasEntity ? "if (t != null) t.Unregister();" : "")}}}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
						code.AppendLine("\t\t}");
					}
					if (o.UseClientSyncPattern)
					{
						code.Append($"\t\tpublic virtual void New{dcmtype.Name}DRE({DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)} DREData{(o.NewParameters.Count != 0 ? ", " : "")}");
						foreach (WcfMethodParameter mp in o.NewParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.NewParameters.IndexOf(mp) < o.NewParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(")");
						code.AppendLine("\t\t{");
						code.AppendLine($"\t\t\tvar t = {DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)}.Register(ClientID, DREData);");
						if (dcmtype.HasEntity) code.AppendLine("\t\t\tif (t != null) t.Register();");
						code.AppendLine("\t\t}");
						code.Append($"\t\tpublic virtual void Delete{dcmtype.Name}DRE(Guid DREDataID{(o.DeleteParameters.Count != 0 ? ", " : "")}");
						foreach (WcfMethodParameter mp in o.DeleteParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.DeleteParameters.IndexOf(mp) < o.DeleteParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(")");
						code.AppendLine("\t\t{");
						code.AppendLine($"\t\t\tvar t = {DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)}.Unregister(ClientID, DREDataID);");
						if (dcmtype.HasEntity) code.AppendLine("\t\t\tif (t != null) t.Unregister();");
						code.AppendLine("\t\t}");
					}
				}
				if (o.GenerateOpenCloseFunction)
				{
					if (o.UseServerAwaitPattern)
					{
						code.Append(string.Format("\t\tpublic abstract System.Threading.Tasks.Task<{1}> Open{0}DREAsync(Guid DREDataID{2}", dcmtype.Name, DataTypeGenerator.GenerateType(dcmtype), o.OpenParameters.Count != 0 ? ", " : ""));
						foreach (WcfMethodParameter mp in o.OpenParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.OpenParameters.IndexOf(mp) < o.OpenParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(");");
						code.Append($"\t\tpublic virtual System.Threading.Tasks.Task Close{dcmtype.Name}DREAsync(Guid DREDataID{(o.CloseParameters.Count != 0 ? ", " : "")}");
						foreach (WcfMethodParameter mp in o.CloseParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.CloseParameters.IndexOf(mp) < o.CloseParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(")");
						code.AppendLine("\t\t{");
						code.AppendLine($"\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(() => {{ var t = {DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)}.Unregister(ClientID, DREDataID); {(dcmtype.HasEntity ? "if (t != null) t.Unregister();" : "")}}}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
						code.AppendLine("\t\t}");
					}
					if (o.UseClientSyncPattern)
					{
						code.Append(string.Format("\t\tpublic abstract {1} Open{0}DRE(Guid DREDataID{2}", dcmtype.Name, DataTypeGenerator.GenerateType(dcmtype), o.OpenParameters.Count != 0 ? ", " : ""));
						foreach (WcfMethodParameter mp in o.OpenParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.OpenParameters.IndexOf(mp) < o.OpenParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(");");
						code.Append($"\t\tpublic virtual void Close{dcmtype.Name}DRE(Guid DREDataID{(o.CloseParameters.Count != 0 ? ", " : "")}");
						foreach (WcfMethodParameter mp in o.CloseParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.CloseParameters.IndexOf(mp) < o.CloseParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(")");
						code.AppendLine("\t\t{");
						code.AppendLine($"\t\t\tvar t = {DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)}.Unregister(ClientID, DREDataID);");
						if (dcmtype.HasEntity) code.AppendLine("\t\t\tif (t != null) t.Unregister();");
						code.AppendLine("\t\t}");
					}
				}
			}
			else
			{
				if (o.GenerateGetFunction)
				{
					var x = new WcfMethod($"Get{dcmtype.Name}DRE", o.Owner) { Parameters = o.GetParameters, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = o.ReturnType };
					code.Append(GenerateServiceClientMethodDCM45(x, o.UseTPLForCallbacks));
				}
				if (o.GenerateNewDeleteFunction)
				{
					var xp = new ObservableCollection<WcfMethodParameter>(o.NewParameters);
					var x = new WcfMethod($"New{dcmtype.Name}DRE", o.Owner) { Parameters = xp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
					xp.Insert(0, new WcfMethodParameter(dcmtype, "DREData", o.Owner, x));
					code.Append(GenerateServiceClientMethodDCM45(x, o.UseTPLForCallbacks));
					var yp = new ObservableCollection<WcfMethodParameter>(o.DeleteParameters);
					var y = new WcfMethod($"Delete{dcmtype.Name}DRE", o.Owner) { Parameters = yp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
					yp.Insert(0, new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
					code.Append(GenerateServiceClientMethodDCM45(y, o.UseTPLForCallbacks));
				}
				if (o.GenerateOpenCloseFunction)
				{
					var xp = new ObservableCollection<WcfMethodParameter>(o.OpenParameters);
					var x = new WcfMethod($"Open{dcmtype.Name}DRE", o.Owner) { Parameters = xp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = dcmtype };
					xp.Insert(0, new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, x));
					code.Append(GenerateServiceClientMethodDCM45(x, o.UseTPLForCallbacks));
					var yp = new ObservableCollection<WcfMethodParameter>(o.CloseParameters);
					var y = new WcfMethod($"Close{dcmtype.Name}DRE", o.Owner) { Parameters = yp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
					yp.Insert(0, new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
					code.Append(GenerateServiceClientMethodDCM45(y, o.UseTPLForCallbacks));
				}
			}

			foreach (var de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && (a.DREUpdateMode == DataUpdateMode.Immediate || a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
			{
				DataType edt = de.DataType;

				var tp = new ObservableCollection<WcfMethodParameter>();
				var x = new WcfMethod($"Update{dcmtype.Name}{de.DataName}DRE", o.Owner) { Parameters = tp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));

				if (edt.TypeMode == DataTypeMode.Collection) tp.Add(new WcfMethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = edt.CollectionGenericType } }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Dictionary) tp.Add(new WcfMethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType } }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Queue) { continue; }
				else if (edt.TypeMode == DataTypeMode.Stack) { continue; }
				else tp.Add(new WcfMethodParameter(edt, "ChangedValue", o.Owner, x));

				code.Append(IsServer ? GenerateServiceServerImmediateMethodDCM45(x, dcmtype, de.DataName, edt.TypeMode, o.UseTPLForCallbacks) : GenerateServiceClientMethodDCM45(x, o.UseTPLForCallbacks));
			}

			if (dcmtype.Elements.Any(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch))
			{
				var tp = new ObservableCollection<WcfMethodParameter>();
				var x = new WcfMethod($"BatchUpdate{dcmtype.Name}DRE", o.Owner) { Parameters = tp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));
				foreach (var de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode != DataTypeMode.Collection && a.DataType.TypeMode != DataTypeMode.Dictionary))
					tp.Add(new WcfMethodParameter(new DataType("CMDItemValue", DataTypeMode.Collection, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = de.DataType }, $"{(de.HasClientType ? de.ClientName : de.DataName)}Delta", o.Owner, x));
				//foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Collection))
				//{
				//	DataType edt = de.DataType;
				//	tp.Add(new WcfMethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) {CollectionGenericType = new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = edt.CollectionGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				//}
				//foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Dictionary))
				//{
				//	DataType edt = de.DataType;
				//	tp.Add(new WcfMethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				//}

				code.Append(IsServer ? GenerateServiceServerBatchMethodDCM45(x, dcmtype, o.UseTPLForCallbacks) : GenerateServiceClientMethodDCM45(x, o.UseTPLForCallbacks, false));
			}

			code.AppendLine();

			return code.ToString();
		}

		public static string GenerateServiceServerImmediateMethodDCM45(WcfMethod o, DataType DCMType, string ElementName, DataTypeMode TypeMode, bool UseTPL)
		{
			var code = new StringBuilder();
			if (o.UseServerAwaitPattern)
			{
				code.AppendFormat("\t\tpublic virtual System.Threading.Tasks.Task {0}Async(", o.ServerName);
				foreach (WcfMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(async() => {");
				if (TypeMode == DataTypeMode.Collection)
				{
					code.AppendLine($"\t\t\tvar temp = {DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)}.GetDataFromID(UpdateID);");
					code.AppendLine($"\t\t\tif (temp != null) temp.{ElementName}.ApplyDelta(ChangedItem);");
				}
				else if (TypeMode == DataTypeMode.Dictionary)
				{
					code.AppendLine($"\t\t\tvar temp = {DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)}.GetDataFromID(UpdateID);");
					code.AppendLine($"\t\t\tif (temp != null) temp.{ElementName}.ApplyDelta(ChangedItem);");
				}
				else code.AppendLine(string.Format("\t\t\t{0}.UpdateValue(UpdateID, {0}.{1}Property, ChangedValue);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType), ElementName));
				code.Append($"\t\t\t\t\tawait Callback{o.ServerName}Async(this, ");
				foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
				code.AppendLine(");");
				code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");

				code.AppendFormat("\t\tpublic static System.Threading.Tasks.Task Callback{0}Async({1}Base Sender, ", o.ServerName, o.Owner.Name);
				foreach (WcfMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(async() => {");
				code.AppendLine(string.Format("\t\t\t\tvar tcl = Sender != null ? GetClients<{0}Base>(Sender.Get{2}Clients(Sender.ClientID) ?? GetClientMessageList<{1}>(UpdateID)).Where(a => a.ClientID != Sender.ClientID) : GetClients<{0}Base>(GetClientMessageList<{1}>(UpdateID));", o.Owner.Name, DataTypeGenerator.GenerateType(DCMType), o.ServerName));
				code.AppendLine(string.Format("\t\t\t\tforeach(var tc in tcl)"));
				code.Append($"\t\t\t\t\tawait tc.Callback.{o.ServerName}CallbackAsync(");
				foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
				code.AppendLine(");");
				code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");
				code.AppendLine($"\t\tprotected virtual IEnumerable<Guid> Get{o.ServerName}Clients(Guid SenderID) {{ return null; }}");
			}
			if (o.UseServerSyncPattern)
			{
				code.AppendFormat("\t\tpublic virtual void {0}(", o.ServerName);
				foreach (WcfMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
				if (TypeMode == DataTypeMode.Collection)
				{
					code.AppendLine($"\t\t\tvar temp = {DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)}.GetDataFromID(UpdateID);");
					code.AppendLine($"\t\t\tif (temp != null) temp.{ElementName}.ApplyDelta(ChangedItem);");
				}
				else if (TypeMode == DataTypeMode.Dictionary)
				{
					code.AppendLine($"\t\t\tvar temp = {DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)}.GetDataFromID(UpdateID);");
					code.AppendLine($"\t\t\tif (temp != null) temp.{ElementName}.ApplyDelta(ChangedItem);");
				}
				else code.AppendLine(string.Format("\t\t\t{0}.UpdateValue(UpdateID, {0}.{1}Property, ChangedValue);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType), ElementName));
				code.Append($"\t\t\t\t\tCallback{o.ServerName}(this, ");
				foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
				code.AppendLine(");");
				if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");

				code.AppendFormat("\t\tpublic static void Callback{0}({1}Base Sender, ", o.ServerName, o.Owner.Name);
				foreach (WcfMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
				code.AppendLine(string.Format("\t\t\t\tvar tcl = Sender != null ? GetClients<{0}Base>(Sender.Get{2}Clients(Sender.ClientID) ?? GetClientMessageList<{1}>(UpdateID)).Where(a => a.ClientID != Sender.ClientID) : GetClients<{0}Base>(GetClientMessageList<{1}>(UpdateID));", o.Owner.Name, DataTypeGenerator.GenerateType(DCMType), o.ServerName));
				code.AppendLine(string.Format("\t\t\t\tforeach(var tc in tcl)"));
				code.Append($"\t\t\t\t\ttc.Callback.{o.ServerName}Callback(");
				foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
				code.AppendLine(");");
				if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");
				code.AppendLine($"\t\tprotected virtual IEnumerable<Guid> Get{o.ServerName}Clients(Guid SenderID) {{ return null; }}");
			}
			return code.ToString();
		}

		public static string GenerateServiceServerBatchMethodDCM45(WcfMethod o, WcfData DCMType, bool UseTPL)
		{
			var code = new StringBuilder();
			if (o.UseServerAwaitPattern)
			{
				code.AppendFormat("\t\tpublic virtual System.Threading.Tasks.Task {0}Async(", o.ServerName);
				foreach (WcfMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(async() => {");
				code.AppendLine($"\t\t\t\tvar t = {DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)}.GetDataFromID(UpdateID);");
				code.AppendLine(string.Format("\t\t\t\tif (t != null)"));
				code.AppendLine("\t\t\t\t{");
				foreach (var de in o.Parameters.Where(a => a.Type.Name == "CMDItemValue"))
					code.AppendLine($"\t\t\t\t\tt.ApplyDelta({de.Name});");
				//foreach (DataElement de in DCMType.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && (a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
				//	code.AppendLine(string.Format("\t\t\t\t\tt.{0}.ApplyDelta({0}Delta);", de.HasClientType ? de.ClientName : de.DataName));
				code.AppendLine("\t\t\t\t}");
				code.Append($"\t\t\t\tCallback{o.ServerName}Async(this, ");
				foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
				code.AppendLine(");");
				code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");

				code.AppendFormat("\t\tpublic static void Callback{0}Async({1}Base Sender, ", o.ServerName, o.Owner.Name);
				foreach (WcfMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(async() => {");
				code.AppendLine(string.Format("\t\t\t\tvar tcl = Sender != null ? GetClients<{0}Base>(Sender.Get{2}Clients(Sender.ClientID) ?? GetClientMessageList<{1}>(UpdateID)).Where(a => a.ClientID != Sender.ClientID) : GetClients<{0}Base>(GetClientMessageList<{1}>(UpdateID));", o.Owner.Name, DataTypeGenerator.GenerateType(DCMType), o.ServerName));
				code.AppendLine(string.Format("\t\t\t\tforeach(var tc in tcl)"));
				code.Append($"\t\t\t\t\tawait tc.Callback.{o.ServerName}CallbackAsync(");
				foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
				code.AppendLine(");");
				code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");
				code.AppendLine($"\t\tprotected virtual IEnumerable<Guid> Get{o.ServerName}Clients(Guid SenderID) {{ return null; }}");
			}
			if (o.UseServerSyncPattern)
			{
				code.AppendFormat("\t\tpublic virtual void {0}(", o.ServerName);
				foreach (WcfMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
				code.AppendLine($"\t\t\t\tvar t = {DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)}.GetDataFromID(UpdateID);");
				code.AppendLine(string.Format("\t\t\t\tif (t != null)"));
				code.AppendLine("\t\t\t\t{");
				foreach (var de in o.Parameters.Where(a => a.Type.Name == "CMDItemValue"))
					code.AppendLine($"\t\t\t\t\tt.ApplyDelta({de.Name});");
				//foreach (DataElement de in DCMType.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && (a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
				//	code.AppendLine(string.Format("\t\t\t\t\tt.{0}.ApplyDelta({0}Delta);", de.HasClientType ? de.ClientName : de.DataName));
				code.AppendLine("\t\t\t\t}");
				code.Append($"\t\t\t\tCallback{o.ServerName}(this, ");
				foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
				code.AppendLine(");");
				if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");

				code.AppendFormat("\t\tpublic static void Callback{0}({1}Base Sender, ", o.ServerName, o.Owner.Name);
				foreach (WcfMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
				code.AppendLine(string.Format("\t\t\t\tvar tcl = Sender != null ? GetClients<{0}Base>(Sender.Get{2}Clients(Sender.ClientID) ?? GetClientMessageList<{1}>(UpdateID)).Where(a => a.ClientID != Sender.ClientID) : GetClients<{0}Base>(GetClientMessageList<{1}>(UpdateID));", o.Owner.Name, DataTypeGenerator.GenerateType(DCMType), o.ServerName));
				code.AppendLine(string.Format("\t\t\t\tforeach(var tc in tcl)"));
				code.Append($"\t\t\t\t\ttc.Callback.{o.ServerName}Callback(");
				foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
				code.AppendLine(");");
				if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");
				code.AppendLine($"\t\tprotected virtual IEnumerable<Guid> Get{o.ServerName}Clients(Guid SenderID) {{ return null; }}");
			}
			return code.ToString();
		}

		public static string GenerateServiceClientMethodDCM45(WcfMethod o, bool UseTPL, bool IsImmediate = true)
		{
			var code = new StringBuilder();
			if (o.UseClientAwaitPattern)
			{
				if (o.ReturnType.Primitive == PrimitiveTypes.Void) code.AppendFormat("\t\tpublic virtual System.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				else code.AppendFormat("\t\tpublic virtual System.Threading.Tasks.Task<{1}> {0}Async(", o.HasClientType ? o.ClientName : o.ServerName, DataTypeGenerator.GenerateType(o.ReturnType));
				foreach (WcfMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (IsImmediate)
				{
					code.Append($"\t\t\treturn Channel.{o.ServerName}Async(");
					foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
					code.AppendLine(");");
				}
				else
				{
					code.Append($"\t\t\treturn Channel.{o.ServerName}Async(");
					foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
					code.AppendLine(");");
				}
				code.AppendLine("\t\t}");
			}
			if (o.UseClientSyncPattern)
			{
				code.AppendFormat("\t\tpublic virtual {1} {0}(", o.HasClientType ? o.ClientName : o.ServerName, DataTypeGenerator.GenerateType(o.ReturnType));
				foreach (WcfMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (o.ReturnType.Primitive != PrimitiveTypes.Void)
				{
					if (IsImmediate)
					{
						code.Append(string.Format("\t\t\t{1}Channel.{0}(", o.ServerName, o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : ""));
						foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
						code.AppendLine(");");
					}
					else
					{
						code.Append(string.Format("\t\t\t{1}Channel.{0}(", o.ServerName, o.ReturnType.Primitive != PrimitiveTypes.Void ? "return " : ""));
						foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
						code.AppendLine(");");
					}
				}
				else
				{
					if (IsImmediate)
					{
						if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
						code.Append(string.Format("{1}\t\t\t{2}Channel.{0}(", o.ServerName, UseTPL ? "\t" : "", o.ReturnType.Primitive != PrimitiveTypes.Void ? "t = " : ""));
						foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
						code.AppendLine(");");
						if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
					}
					else
					{
						if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
						code.Append(string.Format("{1}\t\t\t{2}Channel.{0}(", o.ServerName, UseTPL ? "\t" : "", o.ReturnType.Primitive != PrimitiveTypes.Void ? "t = " : ""));
						foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
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

		public static string GenerateCallbackInterfaceDCM45(WcfDataChangeMethod o, bool IsServer)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			var dcmtype = o.ReturnType as WcfData;
			if (dcmtype == null) return "";

			if (o.GenerateNewDeleteFunction)
			{
				if (o.NewDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.NewDocumentation));
					foreach (WcfMethodParameter mp in o.NewParameters.Where(mp => mp.Documentation != null))
						code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				}
				var xp = new ObservableCollection<WcfMethodParameter>(o.NewParameters);
				var x = new WcfCallback($"New{dcmtype.Name}DRECallback", o.Owner) { Parameters = xp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				xp.Insert(0, new WcfMethodParameter(dcmtype, "DREData", o.Owner, x));
				code.Append(IsServer ? GenerateCallbackInterfaceMethodCode45(x) : GenerateClientInterfaceCallbackCode45(x));
				if (o.DeleteDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.DeleteDocumentation));
					foreach (WcfMethodParameter mp in o.DeleteParameters.Where(mp => mp.Documentation != null))
						code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				}
				var yp = new ObservableCollection<WcfMethodParameter>(o.DeleteParameters);
				var y = new WcfCallback($"Delete{dcmtype.Name}DRECallback", o.Owner) { Parameters = yp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				yp.Insert(0, new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
				code.Append(IsServer ? GenerateCallbackInterfaceMethodCode45(y) : GenerateClientInterfaceCallbackCode45(y));
			}
			if (o.GenerateOpenCloseFunction)
			{
				if (o.OpenDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.OpenDocumentation));
					foreach (WcfMethodParameter mp in o.OpenParameters.Where(mp => mp.Documentation != null))
						code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				}
				var xp = new ObservableCollection<WcfMethodParameter>(o.OpenParameters);
				var x = new WcfCallback($"Open{dcmtype.Name}DRECallback", o.Owner) { Parameters = xp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				xp.Insert(0, new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, x));
				code.Append(IsServer ? GenerateCallbackInterfaceMethodCode45(x) : GenerateClientInterfaceCallbackCode45(x));
				if (o.CloseDocumentation != null)
				{
					code.Append(DocumentationGenerator.GenerateDocumentation(o.CloseDocumentation));
					foreach (WcfMethodParameter mp in o.CloseParameters.Where(mp => mp.Documentation != null))
						code.AppendLine($"\t\t///<param name='{mp.Name}'>{mp.Documentation.Summary}</param>");
				}
				var yp = new ObservableCollection<WcfMethodParameter>(o.CloseParameters);
				var y = new WcfCallback($"Close{dcmtype.Name}DRECallback", o.Owner) { Parameters = yp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				yp.Insert(0, new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
				code.Append(IsServer ? GenerateCallbackInterfaceMethodCode45(y) : GenerateClientInterfaceCallbackCode45(y));
			}

			foreach (var de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && (a.DREUpdateMode == DataUpdateMode.Immediate || a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
			{
				DataType edt = de.DataType;

				var tp = new ObservableCollection<WcfMethodParameter>();
				var x = new WcfCallback($"Update{dcmtype.Name}{de.DataName}DRECallback", o.Owner) { Parameters = tp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));

				if (edt.TypeMode == DataTypeMode.Collection) tp.Add(new WcfMethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = edt.CollectionGenericType } }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Dictionary) tp.Add(new WcfMethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType } }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Queue) { continue; }
				else if (edt.TypeMode == DataTypeMode.Stack) { continue; }
				else tp.Add(new WcfMethodParameter(edt, "ChangedValue", o.Owner, x));

				code.Append(IsServer ? GenerateCallbackInterfaceMethodCode45(x) : GenerateClientInterfaceCallbackCode45(x));
			}

			if (dcmtype.Elements.Any(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch))
			{
				var tp = new ObservableCollection<WcfMethodParameter>();
				var x = new WcfCallback($"BatchUpdate{dcmtype.Name}DRECallback", o.Owner) { Parameters = tp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));
				foreach (var de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode != DataTypeMode.Collection && a.DataType.TypeMode != DataTypeMode.Dictionary))
					tp.Add(new WcfMethodParameter(new DataType("CMDItemValue", DataTypeMode.Collection, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = de.DataType }, $"{(de.HasClientType ? de.ClientName : de.DataName)}Delta", o.Owner, x));
				//foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Collection))
				//{
				//	DataType edt = de.DataType;
				//	tp.Add(new WcfMethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = edt.CollectionGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				//}
				//foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Dictionary))
				//{
				//	DataType edt = de.DataType;
				//	tp.Add(new WcfMethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				//}

				code.Append(IsServer ? GenerateCallbackInterfaceMethodCode45(x) : GenerateClientInterfaceCallbackCode45(x));
			}

			code.AppendLine();

			return code.ToString();
		}

		public static string GenerateCallbackImplementationDCM45(WcfDataChangeMethod o, bool IsServer)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			var dcmtype = o.ReturnType as WcfData;
			if (dcmtype == null) return "";

			if (!IsServer)
			{
				if (o.GenerateNewDeleteFunction)
				{
					if (o.UseClientAwaitPattern && !o.Owner.CBUseSynchronizationContext)
					{
						code.Append($"\t\tpublic abstract System.Threading.Tasks.Task New{dcmtype.Name}DRECallbackAsync({DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)} DREData{(o.NewParameters.Count != 0 ? ", " : "")}");
						foreach (WcfMethodParameter mp in o.NewParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.NewParameters.IndexOf(mp) < o.NewParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(");");
						code.Append($"\t\tpublic abstract System.Threading.Tasks.Task Delete{dcmtype.Name}DRECallbackAsync(Guid DREDataID{(o.NewParameters.Count != 0 ? ", " : "")}");
						foreach (WcfMethodParameter mp in o.DeleteParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.DeleteParameters.IndexOf(mp) < o.DeleteParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(");");
					}
					if (o.UseClientSyncPattern || o.Owner.CBUseSynchronizationContext)
					{
						code.Append($"\t\tpublic abstract void New{dcmtype.Name}DRECallback({DataTypeGenerator.GenerateType(dcmtype.HasClientType ? dcmtype.ClientType : dcmtype)} DREData{(o.NewParameters.Count != 0 ? ", " : "")}");
						foreach (WcfMethodParameter mp in o.NewParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.NewParameters.IndexOf(mp) < o.NewParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(");");
						code.Append($"\t\tpublic abstract void Delete{dcmtype.Name}DRECallback(Guid DREDataID{(o.DeleteParameters.Count != 0 ? ", " : "")}");
						foreach (WcfMethodParameter mp in o.DeleteParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.DeleteParameters.IndexOf(mp) < o.DeleteParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(");");
					}
				}
				if (o.GenerateOpenCloseFunction)
				{
					if (o.UseClientAwaitPattern && !o.Owner.CBUseSynchronizationContext)
					{
						code.Append($"\t\tpublic abstract System.Threading.Tasks.Task Open{dcmtype.Name}DRECallbackAsync(Guid DREDataID{(o.OpenParameters.Count != 0 ? ", " : "")}");
						foreach (WcfMethodParameter mp in o.OpenParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.OpenParameters.IndexOf(mp) < o.OpenParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(");");
						code.Append($"\t\tpublic abstract System.Threading.Tasks.Task Close{dcmtype.Name}DRECallbackAsync(Guid DREDataID{(o.CloseParameters.Count != 0 ? ", " : "")}");
						foreach (WcfMethodParameter mp in o.CloseParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.CloseParameters.IndexOf(mp) < o.CloseParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(");");
					}
					if (o.UseClientSyncPattern || o.Owner.CBUseSynchronizationContext)
					{
						code.Append($"\t\tpublic abstract void Open{dcmtype.Name}DRECallback(Guid DREDataID{(o.OpenParameters.Count != 0 ? ", " : "")}");
						foreach (WcfMethodParameter mp in o.OpenParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.OpenParameters.IndexOf(mp) < o.OpenParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(");");
						code.Append($"\t\tpublic abstract void Close{dcmtype.Name}DRECallback(Guid DREDataID{(o.CloseParameters.Count != 0 ? ", " : "")}");
						foreach (WcfMethodParameter mp in o.CloseParameters) code.Append($"{DataTypeGenerator.GenerateType(mp.Type.HasClientType ? mp.Type.ClientType : mp.Type)} {mp.Name}{(o.CloseParameters.IndexOf(mp) < o.CloseParameters.Count - 1 ? ", " : "")}");
						code.AppendLine(");");
					}
				}
			}
			else
			{
				if (o.GenerateNewDeleteFunction)
				{
					var xp = new ObservableCollection<WcfMethodParameter>(o.NewParameters);
					var x = new WcfCallback($"New{dcmtype.Name}DRECallback", o.Owner) { Parameters = xp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
					xp.Insert(0, new WcfMethodParameter(dcmtype, "DREData", o.Owner, x));
					code.Append(GenerateCallbackServerMethodDCM45(x, o.UseTPLForCallbacks));
					var yp = new ObservableCollection<WcfMethodParameter>(o.DeleteParameters);
					var y = new WcfCallback($"Delete{dcmtype.Name}DRECallback", o.Owner) { Parameters = yp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
					yp.Insert(0, new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
					code.Append(GenerateCallbackServerMethodDCM45(y, o.UseTPLForCallbacks));
				}
				if (o.GenerateOpenCloseFunction)
				{
					var xp = new ObservableCollection<WcfMethodParameter>(o.OpenParameters);
					var x = new WcfCallback($"Open{dcmtype.Name}DRECallback", o.Owner) { Parameters = xp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
					xp.Insert(0, new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, x));
					code.Append(GenerateCallbackServerMethodDCM45(x, o.UseTPLForCallbacks));
					var yp = new ObservableCollection<WcfMethodParameter>(o.CloseParameters);
					var y = new WcfCallback($"Close{dcmtype.Name}DRECallback", o.Owner) { Parameters = yp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
					yp.Insert(0, new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "DREDataID", o.Owner, y));
					code.Append(GenerateCallbackServerMethodDCM45(y, o.UseTPLForCallbacks));
				}
			}

			foreach (var de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && (a.DREUpdateMode == DataUpdateMode.Immediate || a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
			{
				DataType edt = de.DataType;

				var tp = new ObservableCollection<WcfMethodParameter>();
				var x = new WcfCallback($"Update{dcmtype.Name}{de.DataName}DRECallback", o.Owner) { Parameters = tp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));

				if (edt.TypeMode == DataTypeMode.Collection) tp.Add(new WcfMethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = edt.CollectionGenericType } }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Dictionary) tp.Add(new WcfMethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType } }, "ChangedItem", o.Owner, x));
				else if (edt.TypeMode == DataTypeMode.Queue) { continue; }
				else if (edt.TypeMode == DataTypeMode.Stack) { continue; }
				else tp.Add(new WcfMethodParameter(edt, "ChangedValue", o.Owner, x));

				code.Append(!IsServer ? GenerateCallbackClientImmediateMethodDCM45(x, dcmtype, de.DataName, edt.TypeMode, o.UseTPLForCallbacks) : GenerateCallbackServerMethodDCM45(x, o.UseTPLForCallbacks));
			}

			if (dcmtype.Elements.Any(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch))
			{
				var tp = new ObservableCollection<WcfMethodParameter>();
				var x = new WcfCallback($"BatchUpdate{dcmtype.Name}DRECallback", o.Owner) { Parameters = tp, UseServerSyncPattern = o.UseServerSyncPattern, UseServerAwaitPattern = o.UseServerAwaitPattern, UseClientSyncPattern = o.UseClientSyncPattern, UseClientAwaitPattern = o.UseClientAwaitPattern, ReturnType = new DataType(PrimitiveTypes.Void) };
				tp.Add(new WcfMethodParameter(new DataType(PrimitiveTypes.GUID), "UpdateID", o.Owner, x));
				foreach (var de in dcmtype.Elements.Where(a => a.DREEnabled && !a.DREPrimaryKey && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode != DataTypeMode.Collection && a.DataType.TypeMode != DataTypeMode.Dictionary))
					tp.Add(new WcfMethodParameter(new DataType("CMDItemValue", DataTypeMode.Collection, SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = de.DataType }, $"{(de.HasClientType ? de.ClientName : de.DataName)}Delta", o.Owner, x));
				//foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Collection))
				//{
				//	DataType edt = de.DataType;
				//	tp.Add(new WcfMethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = new DataType("ChangeListItem", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = edt.CollectionGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				//}
				//foreach (DataElement de in dcmtype.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && a.DataType.TypeMode == DataTypeMode.Dictionary))
				//{
				//	DataType edt = de.DataType;
				//	tp.Add(new WcfMethodParameter(new DataType("IEnumerable", DataTypeMode.Collection, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { CollectionGenericType = new DataType("ChangeDictionaryItem", DataTypeMode.Dictionary, SupportedFrameworks.NET40 | SupportedFrameworks.NET45 | SupportedFrameworks.WINRT) { DictionaryKeyGenericType = edt.DictionaryKeyGenericType, DictionaryValueGenericType = edt.DictionaryValueGenericType } }, string.Format("{0}Delta", de.HasClientType ? de.ClientName : de.DataName), o.Owner, x));
				//}

				code.Append(!IsServer ? GenerateCallbackClientBatchMethodDCM45(x, dcmtype, o.UseTPLForCallbacks) : GenerateCallbackServerMethodDCM45(x, o.UseTPLForCallbacks, false));
			}

			code.AppendLine();

			return code.ToString();
		}

		public static string GenerateCallbackClientImmediateMethodDCM45(WcfCallback o, DataType DCMType, string ElementName, DataTypeMode TypeMode, bool UseTPL)
		{
			var code = new StringBuilder();
			if (o.UseClientAwaitPattern && !o.Owner.CBUseSynchronizationContext)
			{
				code.AppendFormat("\t\tpublic virtual System.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (WcfMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(() => {");
				if (TypeMode == DataTypeMode.Collection)
				{
					code.AppendLine($"\t\t\tvar temp = {DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)}.GetDataFromID(UpdateID);");
					code.AppendLine($"\t\t\tif (temp != null) temp.{ElementName}.ApplyDelta(ChangedItem);");
				}
				else if (TypeMode == DataTypeMode.Dictionary)
				{
					code.AppendLine($"\t\t\tvar temp = {DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)}.GetDataFromID(UpdateID);");
					code.AppendLine($"\t\t\tif (temp != null) temp.{ElementName}.ApplyDelta(ChangedItem);");
				}
				else code.AppendLine(string.Format("\t\t\t{0}.UpdateValue(UpdateID, {0}.{1}Property, ChangedValue);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType), ElementName));
				code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");
			}
			if (o.UseClientSyncPattern || o.Owner.CBUseSynchronizationContext)
			{
				code.AppendFormat("\t\tpublic virtual void {0}(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (WcfMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
				if (TypeMode == DataTypeMode.Collection)
				{
					code.AppendLine($"\t\t\tvar temp = {DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)}.GetDataFromID(UpdateID);");
					code.AppendLine($"\t\t\tif (temp != null) temp.{ElementName}.ApplyDelta(ChangedItem);");
				}
				else if (TypeMode == DataTypeMode.Dictionary)
				{
					code.AppendLine($"\t\t\tvar temp = {DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)}.GetDataFromID(UpdateID);");
					code.AppendLine($"\t\t\tif (temp != null) temp.{ElementName}.ApplyDelta(ChangedItem);");
				}
				else code.AppendLine(string.Format("\t\t\t{0}.UpdateValue(UpdateID, {0}.{1}Property, ChangedValue);", DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType), ElementName));
				if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");
			}
			return code.ToString();
		}

		public static string GenerateCallbackClientBatchMethodDCM45(WcfCallback o, WcfData DCMType, bool UseTPL)
		{
			var code = new StringBuilder();
			if (o.UseClientAwaitPattern && !o.Owner.CBUseSynchronizationContext)
			{
				code.AppendFormat("\t\tpublic virtual System.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (WcfMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(() => {");
				code.AppendLine($"\t\t\t\tvar t = {DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)}.GetDataFromID(UpdateID);");
				code.AppendLine(string.Format("\t\t\t\tif (t != null)"));
				code.AppendLine("\t\t\t\t{");
				foreach (var de in o.Parameters.Where(a => a.Type.Name == "CMDItemValue"))
					code.AppendLine($"\t\t\t\t\tt.ApplyDelta({de.Name});");
				//foreach (DataElement de in DCMType.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && (a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
				//	code.AppendLine(string.Format("\t\t\t\t\tt.{0}.ApplyDelta({0}Delta);", de.HasClientType ? de.ClientName : de.DataName));
				code.AppendLine("\t\t\t\t}");
				code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");
			}
			if (o.UseClientSyncPattern || o.Owner.CBUseSynchronizationContext)
			{
				code.AppendFormat("\t\tpublic virtual void {0}(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (WcfMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
				code.AppendLine($"\t\t\t\tvar t = {DataTypeGenerator.GenerateType(DCMType.HasClientType ? DCMType.ClientType : DCMType)}.GetDataFromID(UpdateID);");
				code.AppendLine(string.Format("\t\t\t\tif (t != null)"));
				code.AppendLine("\t\t\t\t{");
				foreach (var de in o.Parameters.Where(a => a.Type.Name == "CMDItemValue"))
					code.AppendLine($"\t\t\t\t\tt.ApplyDelta({de.Name});");
				//foreach (DataElement de in DCMType.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && (a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
				//	code.AppendLine(string.Format("\t\t\t\t\tt.{0}.ApplyDelta({0}Delta);", de.HasClientType ? de.ClientName : de.DataName));
				code.AppendLine("\t\t\t\t}");
				if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				code.AppendLine("\t\t}");
			}
			return code.ToString();
		}

		public static string GenerateCallbackServerMethodDCM45(WcfCallback o, bool UseTPL, bool IsImmediate = true)
		{
			var code = new StringBuilder();
			if (o.UseServerAwaitPattern)
			{
				code.AppendFormat("\t\tpublic virtual System.Threading.Tasks.Task {0}Async(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (WcfMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (IsImmediate)
				{
					code.AppendLine("\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(() => {");
					code.Append(string.Format("{1}\t\t\t__callback.{0}Async(", o.ServerName, UseTPL ? "\t" : ""));
					foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
					code.AppendLine(");");
					code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				}
				else
				{
					code.AppendLine("\t\t\treturn System.Threading.Tasks.Task.Factory.StartNew(() => {");
					code.Append(string.Format("{1}\t\t\t__callback.{0}Async(", o.ServerName, UseTPL ? "\t" : ""));
					foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
					code.AppendLine(");");
					code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				}
				code.AppendLine("\t\t}");
			}
			if (o.UseServerSyncPattern)
			{
				code.AppendFormat("\t\tpublic virtual void {0}(", o.HasClientType ? o.ClientName : o.ServerName);
				foreach (WcfMethodParameter op in o.Parameters)
					code.AppendFormat("{0}, ", GenerateWcfMethodParameterClientCode(op));
				if (o.Parameters.Count > 0) code.Remove(code.Length - 2, 2);
				code.AppendLine(")");
				code.AppendLine("\t\t{");
				if (IsImmediate)
				{
					if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
					code.Append(string.Format("{1}\t\t\t__callback.{0}(", o.ServerName, UseTPL ? "\t" : ""));
					foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
					code.AppendLine(");");
					if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				}
				else
				{
					if (UseTPL) code.AppendLine("\t\t\tSystem.Threading.Tasks.Task.Factory.StartNew(() => {");
					code.Append(string.Format("{1}\t\t\t__callback.{0}(", o.ServerName, UseTPL ? "\t" : ""));
					foreach (WcfMethodParameter mp in o.Parameters) code.Append($"{mp.Name}{(o.Parameters.IndexOf(mp) < o.Parameters.Count - 1 ? ", " : "")}");
					code.AppendLine(");");
					if (UseTPL) code.AppendLine("\t\t\t}, System.Threading.Tasks.TaskCreationOptions.PreferFairness);");
				}
				code.AppendLine("\t\t}");
			}
			return code.ToString();
		}

		#endregion

		#region - Method Parameters -

		public static string GenerateWcfMethodParameterServerCode(WcfMethodParameter o)
		{
			return o.IsHidden ? "" : $"{DataTypeGenerator.GenerateType(o.Type)} {o.Name}";
		}

		public static string GenerateWcfMethodParameterClientCode(WcfMethodParameter o)
		{
			if (o.IsHidden) return "";

			if (o.Type.TypeMode == DataTypeMode.Class)
			{
				var ptype = o.Type as WcfData;
				return $"{(ptype != null && ptype.HasClientType ? DataTypeGenerator.GenerateType(ptype.ClientType) : DataTypeGenerator.GenerateType(o.Type))} {o.Name}";
			}
			if (o.Type.TypeMode == DataTypeMode.Struct)
			{
				var ptype = o.Type as WcfData;
				return $"{(ptype != null && ptype.HasClientType ? DataTypeGenerator.GenerateType(ptype.ClientType) : DataTypeGenerator.GenerateType(o.Type))} {o.Name}";
			}
			if (o.Type.TypeMode == DataTypeMode.Enum)
			{
				var ptype = o.Type as Projects.Enum;
				return $"{(ptype != null && ptype.HasClientType ? DataTypeGenerator.GenerateType(ptype.ClientType) : DataTypeGenerator.GenerateType(o.Type))} {o.Name}";
			}

			return $"{DataTypeGenerator.GenerateType(o.Type)} {o.Name}";
		}

		#endregion

		#region - Properties -

		public static string GeneratePropertyServerCode45(WcfProperty o)
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

		public static string GeneratePropertyInterfaceCode45(WcfProperty o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/{1}/Get{2}\")]" : "\t\t[OperationContract(Action = \"{0}/{1}/Get{2}\", ReplyAction = \"{0}/{1}/Set{2}Response\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
			code.AppendLine($"\t\t{DataTypeGenerator.GenerateType(o.ReturnType)} Get{o.ServerName}();");
			if (!o.IsReadOnly)
			{
				code.AppendLine(string.Format(o.IsOneWay ? "\t\t[OperationContract(IsOneWay = true, Action = \"{0}/{1}/Get{2}\")]" : "\t\t[OperationContract(Action = \"{0}/{1}/Get{2}\", ReplyAction = \"{0}/{1}/Set{2}Response\")]", o.Owner.Parent.FullURI.Substring(0, o.Owner.Parent.FullURI.Length - 1), o.Owner.Name, o.ServerName));
				code.AppendLine(string.Format("\t\tvoid Set{1}({0} value);", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName));
			}
			return code.ToString();
		}

		public static string GeneratePropertyClientCode(WcfProperty o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();

			code.AppendLine($"\t\t{DataTypeGenerator.GenerateType(o.ReturnType)} I{(o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name)}.Get{o.ServerName}()");
			code.AppendLine("\t\t{");
			code.AppendLine($"\t\t\treturn base.Channel.Get{(o.HasClientType ? o.ClientName : o.ServerName)}();");
			code.AppendLine("\t\t}");

			if (!o.IsReadOnly)
			{
				code.AppendLine(string.Format("\t\tvoid I{1}.Set{2}({0} value)", DataTypeGenerator.GenerateType(o.ReturnType), o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, o.ServerName));
				code.AppendLine("\t\t{");
				code.AppendLine($"\t\t\tbase.Channel.Set{(o.HasClientType ? o.ClientName : o.ServerName)}(value);");
				code.AppendLine("\t\t}");
			}

			code.AppendLine(string.Format(o.IsReadOnly == false ? "\t\tpublic {0} {1} {{ get {{ return ((I{2})this).Get{1}(); }} set {{ ((I{2})this).Set{1}(value); }} }}" : "\t\tpublic {0} {1} {{ get {{ return ((I{2})this).Get{1}(); }} }}", DataTypeGenerator.GenerateType(o.ReturnType), o.ServerName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name));

			return code.ToString();
		}

		#endregion
	}
}