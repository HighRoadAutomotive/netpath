using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETPath.Projects;
using NETPath.Projects.Wcf;
using NETPath.Projects.Helpers;

namespace NETPath.Generators.CS.Wcf
{
	internal static class HostGenerator
	{
		public static void VerifyCode(WcfHost o, Action<CompileMessage> AddMessage)
		{
			if (string.IsNullOrEmpty(o.Name))
				AddMessage(new CompileMessage("GS5000", "A host in the '" + o.Parent.Name + "' project has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));
			else
				if (RegExs.MatchCodeName.IsMatch(o.Name) == false)
				AddMessage(new CompileMessage("GS5001", "The host '" + o.Name + "' in the '" + o.Parent.Name + "' project contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));
			if (string.IsNullOrEmpty(o.Namespace)) { }
			else
				if (RegExs.MatchHttpUri.IsMatch(o.Namespace) == false)
				AddMessage(new CompileMessage("GS5002", "The Namespace Uri '" + o.Namespace + "' for the '" + o.Name + "' host in the '" + o.Parent.Name + "' project is not a valid Uri.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType()));

			if (o.Service == null)
				AddMessage(new CompileMessage("GS5003", "The host '" + o.Name + "' in the '" + o.Parent.Name + "' project has no Service associated with it and will not be generated. A Service must be associated with this Host for it be generated.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType()));

			foreach (string ba in o.BaseAddresses)
			{
				bool baValid = RegExs.MatchHttpUri.IsMatch(ba) || RegExs.MatchTcpUri.IsMatch(ba) || RegExs.MatchP2PUri.IsMatch(ba) || RegExs.MatchPipeUri.IsMatch(ba) || RegExs.MatchMsmqUri.IsMatch(ba);
				if (baValid)
					AddMessage(new CompileMessage("GS5003", "The Uri '" + ba + "' for the '" + o.Name + "' host in the '" + o.Parent.Name + "' project is not a valid Uri. Any associated services and data may not function properly.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType()));
			}

			foreach (WcfHostEndpoint he in o.Endpoints)
			{
				if (string.IsNullOrEmpty(he.Name))
					AddMessage(new CompileMessage("GS5004", "A host in the endpoint '" + he.Parent.Name + "' Service Host has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, he.Parent, he, he.GetType()));
				else if (RegExs.MatchCodeName.IsMatch(he.Name) == false)
					AddMessage(new CompileMessage("GS5005", "The host endpoint '" + he.Name + "' in the '" + he.Parent.Name + "' Service Host contains invalid characters in the Code Name.", CompileMessageSeverity.ERROR, he.Parent, he, he.GetType()));
				if (he.Binding == null)
					AddMessage(new CompileMessage("GS5006", "The host endpoint'" + he.Name + "' in the '" + he.Parent.Name + "' Service Host must have a Binding. Please specify a Binding", CompileMessageSeverity.ERROR, he.Parent, he, he.GetType()));
				else
				{
					Type bt = he.Binding.GetType();
					if (o.Service.HasCallbackOperations && !(bt == typeof(WcfBindingNetHTTP) || bt == typeof(WcfBindingNetHTTPS) || bt == typeof(WcfBindingWSDualHTTP) || bt == typeof(WcfBindingTCP) || bt == typeof(WcfBindingNamedPipe) || bt == typeof(WcfBindingPeerTCP)))
					{
						AddMessage(new CompileMessage("GS5007", "The binding specified for the host endpoint'" + he.Name + "' in the '" + he.Parent.Name + "' Service Host does not support callback interfaces. Please specify a Binding that supports callbacks.", CompileMessageSeverity.ERROR, he.Parent, he, he.GetType()));
					}
				}
				if (string.IsNullOrEmpty(he.ServerAddress) && he.ServerAddressIsVariable)
					AddMessage(new CompileMessage("GS50015", "A host in the endpoint '" + he.Parent.Name + "' Service Host has specified a code variable for the Server Address but has provided no variable. A Server Address Variable MUST be specified.", CompileMessageSeverity.ERROR, he.Parent, he, he.GetType()));
			}

			foreach (WcfHostBehavior hb in o.Behaviors)
			{
				Type t = hb.GetType();
				if (t == typeof(WcfHostDebugBehavior))
				{
					var b = hb as WcfHostDebugBehavior;
					if (b == null) continue;
					if (b.HttpHelpPageEnabled && RegExs.MatchHttpUri.IsMatch(b.HttpHelpPageUrl) == false)
						AddMessage(new CompileMessage("GS5007", "The HTTP Help Page URL '" + b.HttpHelpPageUrl + "' for the '" + b.Parent.Name + "' host in the '" + b.Parent.Parent.Name + "' project is not a valid Uri. The software may not be able to access the specified page.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
					if (b.HttpsHelpPageEnabled && RegExs.MatchHttpUri.IsMatch(b.HttpsHelpPageUrl) == false)
						AddMessage(new CompileMessage("GS5008", "The HTTPS Help Page URL '" + b.HttpsHelpPageUrl + "' for the '" + b.Parent.Name + "' host in the '" + b.Parent.Parent.Name + "' project is not a valid Uri. The software may not be able to access the specified page.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
					if (b.HttpHelpPageEnabled && b.HttpHelpPageBinding == null)
						AddMessage(new CompileMessage("GS5011", "The HTTP Help Page Binding for the '" + b.Parent.Name + "' host in the '" + b.Parent.Parent.Name + "' project cannot be empty. Please select a binding.", CompileMessageSeverity.ERROR, b.Parent, b, b.GetType()));
					if (b.HttpsHelpPageEnabled && b.HttpsHelpPageBinding == null)
						AddMessage(new CompileMessage("GS5012", "The HTTPS Help Page Binding for the '" + b.Parent.Name + "' host in the '" + b.Parent.Parent.Name + "' project cannot be empty. Please select a binding.", CompileMessageSeverity.ERROR, b.Parent, b, b.GetType()));
				}
				else if (t == typeof(WcfHostMetadataBehavior))
				{
					var b = hb as WcfHostMetadataBehavior;
					if (b == null) continue;
					if (b.HttpGetEnabled && RegExs.MatchHttpUri.IsMatch(b.HttpGetUrl) == false)
						AddMessage(new CompileMessage("GS5009", "The HTTP Get URL '" + b.HttpGetUrl + "' for the '" + b.Parent.Name + "' host in the '" + b.Parent.Parent.Name + "' project is not a valid Uri. The software may not be able to access the specified page.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
					if (b.HttpsGetEnabled && RegExs.MatchHttpUri.IsMatch(b.HttpsGetUrl) == false)
						AddMessage(new CompileMessage("GS5010", "The HTTPS Get URL '" + b.HttpsGetUrl + "' for the '" + b.Parent.Name + "' host in the '" + b.Parent.Parent.Name + "' project is not a valid Uri. The software may not be able to access the specified page.", CompileMessageSeverity.WARN, b.Parent, b, b.GetType()));
					if (b.HttpGetEnabled && b.HttpGetBinding == null)
						AddMessage(new CompileMessage("GS5013", "The HTTP Get Binding for the '" + b.Parent.Name + "' host in the '" + b.Parent.Parent.Name + "' project cannot be empty. Please select a binding.", CompileMessageSeverity.ERROR, b.Parent, b, b.GetType()));
					if (b.HttpsGetEnabled && RegExs.MatchHttpUri.IsMatch(b.HttpsGetUrl) == false)
						AddMessage(new CompileMessage("GS5014", "The HTTPS Get Binding for the '" + b.Parent.Name + "' host in the '" + b.Parent.Parent.Name + "' project cannot be empty. Please select a binding.", CompileMessageSeverity.ERROR, b.Parent, b, b.GetType()));
				}
			}
		}

		public static string GenerateServerCode45(WcfHost o)
		{
			if (o.Service == null) return "";

			var code = new StringBuilder();
			if (o.Parent.Owner.EnableDocumentationWarnings) code.AppendLine("#pragma warning disable 1591");
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.AppendLine($"\t[System.CodeDom.Compiler.GeneratedCode(\"{Globals.ApplicationTitle}\", \"{Globals.ApplicationVersion}\")]");
			code.AppendLine($"\t{DataTypeGenerator.GenerateTypeDeclaration(o)} : ServiceHost");
			code.AppendLine("\t{");
			string baVars = "";
			for (int i = 0; i < o.BaseAddresses.Count; i++)
			{
				code.AppendFormat("\t\tprivate static Uri BaseAddr{0} = new Uri(\"{1}\");{2}", i, o.BaseAddresses[i], Environment.NewLine);
				baVars += ", " + o.Name + ".BaseAddr" + i;
			}

			foreach (WcfHostEndpoint he in o.Endpoints)
				code.AppendFormat("\t\tpublic static Uri {0}Uri {{ get {{ return new Uri(\"{1}\"); }} }}{2}", he.Name, GenerateServerEndpointURI(he), Environment.NewLine);

			foreach (WcfHostBehavior hb in o.Behaviors)
			{
				if (hb.GetType() == typeof(WcfHostDebugBehavior)) code.AppendFormat("\t\tpublic ServiceDebugBehavior {0} {{ get; private set; }}{1}", hb.Name, Environment.NewLine);
				if (hb.GetType() == typeof(WcfHostMetadataBehavior)) code.AppendFormat("\t\tpublic ServiceMetadataBehavior {0} {{ get; private set; }}{1}", hb.Name, Environment.NewLine);
				if (hb.GetType() == typeof(WcfHostThrottlingBehavior)) code.AppendFormat("\t\tpublic ServiceThrottlingBehavior {0} {{ get; private set; }}{1}", hb.Name, Environment.NewLine);
			}

			#region - Generate Default Constructors
			//Generate Singleton Service Constructor WITH default base addresses
			code.AppendFormat("\t\tpublic {0}(object singletonInstance) : base(singletonInstance{1}){2}", o.Name, baVars, Environment.NewLine);
			code.AppendLine("\t\t{");
			code.Append(GenerateCredentialsCode45(o.Credentials));
			code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (!string.IsNullOrEmpty(o.ConfigurationName)) code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (WcfHostBehavior hb in o.Behaviors)
				code.Append(GenerateBehaviorCode45(hb));
			if (o.ManualFlowControlLimit > 0) code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			foreach (WcfHostEndpoint he in o.Endpoints)
				code.AppendLine(GenerateEndpointServerCode(he));
			code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITH default base addresses
			code.AppendFormat("\t\tpublic {0}(Type serviceType) : base(serviceType{1}){2}", o.Name, baVars, Environment.NewLine);
			code.AppendLine("\t\t{");
			code.Append(GenerateCredentialsCode45(o.Credentials));
			code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (!string.IsNullOrEmpty(o.ConfigurationName)) code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (WcfHostBehavior hb in o.Behaviors)
				code.Append(GenerateBehaviorCode45(hb));
			if (o.ManualFlowControlLimit > 0) code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			foreach (WcfHostEndpoint he in o.Endpoints)
				code.AppendLine(GenerateEndpointServerCode(he));
			code.AppendLine("\t\t}");

			//Generate Singleton Service Constructor WITHOUT default base addresses
			code.AppendFormat("\t\tpublic {0}(object singletonInstance, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses){1}", o.Name, Environment.NewLine);
			code.AppendLine("\t\t{");
			code.Append(GenerateCredentialsCode45(o.Credentials));
			code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (!string.IsNullOrEmpty(o.ConfigurationName)) code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (WcfHostBehavior hb in o.Behaviors)
				code.Append(GenerateBehaviorCode45(hb));
			if (o.ManualFlowControlLimit > 0) code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			foreach (WcfHostEndpoint he in o.Endpoints)
				code.AppendLine(GenerateEndpointServerCode(he));
			code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITHOUT default base addresses
			code.AppendFormat("\t\tpublic {0}(Type serviceType, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses){1}", o.Name, Environment.NewLine);
			code.AppendLine("\t\t{");
			code.Append(GenerateCredentialsCode45(o.Credentials));
			code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (!string.IsNullOrEmpty(o.ConfigurationName)) code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (WcfHostBehavior hb in o.Behaviors)
				code.Append(GenerateBehaviorCode45(hb));
			if (o.ManualFlowControlLimit > 0) code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			foreach (WcfHostEndpoint he in o.Endpoints)
				code.AppendLine(GenerateEndpointServerCode(he));
			code.AppendLine("\t\t}");
			#endregion

			#region - Generate Optional Endpoint Constructors
			//Generate Singleton Service Constructor WITH default base addresses
			code.AppendFormat("\t\tpublic {0}(object singletonInstance, bool DisableDefaultEndpoints) : base(singletonInstance{1}){2}", o.Name, baVars, Environment.NewLine);
			code.AppendLine("\t\t{");
			code.Append(GenerateCredentialsCode45(o.Credentials));
			code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (!string.IsNullOrEmpty(o.ConfigurationName)) code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (WcfHostBehavior hb in o.Behaviors)
				code.Append(GenerateBehaviorCode45(hb));
			if (o.ManualFlowControlLimit > 0) code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			code.AppendLine("\t\t\t{");
			foreach (WcfHostEndpoint he in o.Endpoints)
				code.AppendLine(GenerateEndpointServerCode(he));
			code.AppendLine("\t\t\t}");
			code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITH default base addresses
			code.AppendFormat("\t\tpublic {0}(Type serviceType, bool DisableDefaultEndpoints) : base(serviceType{1}){2}", o.Name, baVars, Environment.NewLine);
			code.AppendLine("\t\t{");
			code.Append(GenerateCredentialsCode45(o.Credentials));
			code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (!string.IsNullOrEmpty(o.ConfigurationName)) code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (WcfHostBehavior hb in o.Behaviors)
				code.Append(GenerateBehaviorCode45(hb));
			if (o.ManualFlowControlLimit > 0) code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			code.AppendLine("\t\t\t{");
			foreach (WcfHostEndpoint he in o.Endpoints)
				code.AppendLine(GenerateEndpointServerCode(he));
			code.AppendLine("\t\t\t}");
			code.AppendLine("\t\t}");

			//Generate Singleton Service Constructor WITHOUT default base addresses
			code.AppendFormat("\t\tpublic {0}(object singletonInstance, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses){1}", o.Name, Environment.NewLine);
			code.AppendLine("\t\t{");
			code.Append(GenerateCredentialsCode45(o.Credentials));
			code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (!string.IsNullOrEmpty(o.ConfigurationName)) code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (WcfHostBehavior hb in o.Behaviors)
				code.Append(GenerateBehaviorCode45(hb));
			if (o.ManualFlowControlLimit > 0) code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			code.AppendLine("\t\t\t{");
			foreach (WcfHostEndpoint he in o.Endpoints)
				code.AppendLine(GenerateEndpointServerCode(he));
			code.AppendLine("\t\t\t}");
			code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITHOUT default base addresses
			code.AppendFormat("\t\tpublic {0}(Type serviceType, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses){1}", o.Name, Environment.NewLine);
			code.AppendLine("\t\t{");
			code.Append(GenerateCredentialsCode45(o.Credentials));
			code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (!string.IsNullOrEmpty(o.ConfigurationName)) code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (WcfHostBehavior hb in o.Behaviors)
				code.Append(GenerateBehaviorCode45(hb));
			if (o.ManualFlowControlLimit > 0) code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			code.AppendLine("\t\t\t{");
			foreach (WcfHostEndpoint he in o.Endpoints)
				code.AppendLine(GenerateEndpointServerCode(he));
			code.AppendLine("\t\t\t}");
			code.AppendLine("\t\t}");
			#endregion

			if (o.Parent.Owner.EnableDocumentationWarnings) code.AppendLine("#pragma warning enable 1591");
			code.AppendLine("\t}");

			return code.ToString();
		}

		public static string GenerateClientCode45(WcfHost o, bool IsDCMEnabled = false)
		{
			if (o.Service == null) return "";

			var code = new StringBuilder();

			foreach (WcfHostEndpoint he in o.Endpoints)
				code.AppendFormat("\t\tpublic static Uri {0}Uri {{ get {{ return new Uri({1}); }} }}{2}", he.Name, GenerateClientEndpointURI(he, false, false), Environment.NewLine);

			foreach (WcfHostEndpoint he in o.Endpoints)
				code.AppendLine(GenerateEndpointClientCode(he, IsDCMEnabled));

			return code.ToString();
		}

		public static string GenerateEndpointServerCode(WcfHostEndpoint o)
		{
			var code = new StringBuilder();
			code.AppendLine(string.Format("\t\t\tvar {3}Endpoint = this.AddServiceEndpoint(typeof({0}.I{1}), new {2}(), {3}Uri);", o.Parent.Service.Parent.FullName, o.Parent.Service.Name, DataTypeGenerator.GenerateType(o.Binding), o.Name));
			return code.ToString();
		}

		public static string GenerateEndpointClientCode(WcfHostEndpoint o, bool IsDCMEnabled = false)
		{
			var code = new StringBuilder();

			string identity = "";
			string certificateidentity = "";
			if (o.ClientIdentityType != WcfHostEndpointIdentityType.Anonymous)
			{
				if (o.ClientIdentityType == WcfHostEndpointIdentityType.DNS) identity = ", System.ServiceModel.EndpointIdentity.CreateDnsIdentity(\"" + o.ClientIdentityData + "\")";
				if (o.ClientIdentityType == WcfHostEndpointIdentityType.RSA) identity = ", System.ServiceModel.EndpointIdentity.CreateRsaIdentity(\"" + o.ClientIdentityData + "\")";
				if (o.ClientIdentityType == WcfHostEndpointIdentityType.RSAX509)
				{
					identity = ", System.ServiceModel.EndpointIdentity.CreateRsaIdentity(IdentityCertificate)";
					certificateidentity = ", System.Security.Cryptography.X509Certificates.X509Certificate2 IdentityCertificate";
				}
				if (o.ClientIdentityType == WcfHostEndpointIdentityType.SPN) identity = ", System.ServiceModel.EndpointIdentity.CreateSpnIdentity(\"" + o.ClientIdentityData + "\")";
				if (o.ClientIdentityType == WcfHostEndpointIdentityType.UPN) identity = ", System.ServiceModel.EndpointIdentity.CreateUpnIdentity(\"" + o.ClientIdentityData + "\")";
				if (o.ClientIdentityType == WcfHostEndpointIdentityType.X509)
				{
					identity = ", System.ServiceModel.EndpointIdentity.CreateX509CertificateIdentity(IdentityCertificate)";
					certificateidentity = ", System.Security.Cryptography.X509Certificates.X509Certificate2 IdentityCertificate";
				}
			}

			string ahlist = "";

			if (!o.Parent.Service.HasCallbackOperations && !o.Parent.Service.HasDCMOperations)
			{
				#region - Generate Binding Endpoint Functions -
				#region - Generate Endpoint Functions WITHOUT EndpointIdentity Parameter -
				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}Service({3}{1})", o.Name, certificateidentity.Replace(", ", ""), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {4}Proxy({5}new {3}(), new System.ServiceModel.EndpointAddress(new Uri({0}){1}{2}));", GenerateClientEndpointURI(o, false, false), identity, ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}Service({3}string Address{1})", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {4}Proxy({5}new {3}(), new System.ServiceModel.EndpointAddress(new Uri({0}){1}{2}));", GenerateClientEndpointURI(o, true, false), identity, ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}Service({3}int Port{1})", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {4}Proxy({5}new {3}(), new System.ServiceModel.EndpointAddress(new Uri({0}){1}{2}));", GenerateClientEndpointURI(o, false, true), identity, ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}Service({3}string Address, int Port{1})", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {4}Proxy({5}new {3}(), new System.ServiceModel.EndpointAddress(new Uri({0}){1}{2}));", GenerateClientEndpointURI(o, true, true), identity, ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				#endregion

				#region - Generate Endpoint Functions WITH EndpointIdentity Parameter -

				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}Service({3}System.ServiceModel.EndpointIdentity Identity{1})", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {3}Proxy({4}new {2}(), new System.ServiceModel.EndpointAddress(new Uri({0}), Identity{1}));", GenerateClientEndpointURI(o, false, false), ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}Service({3}string Address, System.ServiceModel.EndpointIdentity Identity{1})", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {3}Proxy({4}new {2}(), new System.ServiceModel.EndpointAddress(new Uri({0}), Identity{1}));", GenerateClientEndpointURI(o, true, false), ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {1}Proxy Create{0}Service({2}int Port, System.ServiceModel.EndpointIdentity Identity)", o.Name, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {3}Proxy({4}new {2}(), new System.ServiceModel.EndpointAddress(new Uri({0}), Identity{1}));", GenerateClientEndpointURI(o, false, true), ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {1}Proxy Create{0}Service({2}string Address, int Port, System.ServiceModel.EndpointIdentity Identity)", o.Name, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {3}Proxy({4}new {2}(), new System.ServiceModel.EndpointAddress(new Uri({0}), Identity{1}));", GenerateClientEndpointURI(o, true, true), ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");
				#endregion
				#endregion

				#region - Generate Binding Endpoint Configuration Functions -
				#region - Generate Endpoint Functions WITHOUT EndpointIdentity Parameter -
				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}ServiceConfig({3}string EndpointConfig{1})", o.Name, certificateidentity.Replace(", ", ""), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {4}Proxy({5}new {3}(), new System.ServiceModel.EndpointAddress(new Uri({0}){1}{2}));", GenerateClientEndpointURI(o, false, false), identity, ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}ServiceConfig({3}string EndpointConfig, string Address{1})", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {4}Proxy({5}new {3}(), new System.ServiceModel.EndpointAddress(new Uri({0}){1}{2}));", GenerateClientEndpointURI(o, true, false), identity, ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}ServiceConfig({3}string EndpointConfig, int Port{1})", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {4}Proxy({5}new {3}(), new System.ServiceModel.EndpointAddress(new Uri({0}){1}{2}));", GenerateClientEndpointURI(o, false, true), identity, ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}ServiceConfig({3}string EndpointConfig, string Address, int Port{1})", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {4}Proxy({5}new {3}(), new System.ServiceModel.EndpointAddress(new Uri({0}){1}{2}));", GenerateClientEndpointURI(o, true, true), identity, ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");
				#endregion

				#region - Generate Endpoint Functions WITH EndpointIdentity Parameter -
				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}ServiceConfig({3}string EndpointConfig, System.ServiceModel.EndpointIdentity Identity{1})", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {3}Proxy({4}new {2}(), new System.ServiceModel.EndpointAddress(new Uri({0}), Identity{1}));", GenerateClientEndpointURI(o, false, false), ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}ServiceConfig({3}string EndpointConfig, string Address, System.ServiceModel.EndpointIdentity Identity{1})", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {3}Proxy({4}new {2}(), new System.ServiceModel.EndpointAddress(new Uri({0}), Identity{1}));", GenerateClientEndpointURI(o, true, false), ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {1}Proxy Create{0}ServiceConfig({2}string EndpointConfig, int Port, System.ServiceModel.EndpointIdentity Identity)", o.Name, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {3}Proxy({4}new {2}(), new System.ServiceModel.EndpointAddress(new Uri({0}), Identity{1}));", GenerateClientEndpointURI(o, false, true), ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {1}Proxy Create{0}ServiceConfig({2}string EndpointConfig, string Address, int Port, System.ServiceModel.EndpointIdentity Identity)", o.Name, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {3}Proxy({4}new {2}(), new System.ServiceModel.EndpointAddress(new Uri({0}), Identity{1}));", GenerateClientEndpointURI(o, true, true), ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");
				#endregion
				#endregion
			}
			else
			{
				#region - Generate Duplex Binding Endpoint Functions -
				#region - Generate Endpoint Functions WITHOUT EndpointIdentity Parameter -
				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}Service<TCallback>({3}{1}) where TCallback : class, new()", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID" : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {4}Proxy({5}new TCallback(), new {3}(), new System.ServiceModel.EndpointAddress(new Uri({0}){1}{2}));", GenerateClientEndpointURI(o, false, false), identity, ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}Service<TCallback>({3}string Address{1}) where TCallback : class, new()", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {4}Proxy({5}new TCallback(), new {3}(), new System.ServiceModel.EndpointAddress(new Uri({0}){1}{2}));", GenerateClientEndpointURI(o, true, false), identity, ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}Service<TCallback>({3}int Port{1}) where TCallback : class, new()", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {4}Proxy({5}new TCallback(), new {3}(), new System.ServiceModel.EndpointAddress(new Uri({0}){1}{2}));", GenerateClientEndpointURI(o, false, true), identity, ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}Service<TCallback>({3}string Address, int Port{1}) where TCallback : class, new()", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {4}Proxy({5}new TCallback(), new {3}(), new System.ServiceModel.EndpointAddress(new Uri({0}){1}{2}));", GenerateClientEndpointURI(o, true, true), identity, ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");
				#endregion

				#region - Generate Endpoint Functions WITH EndpointIdentity Parameter -
				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}Service<TCallback>({3}System.ServiceModel.EndpointIdentity Identity{1}) where TCallback : class, new()", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {3}Proxy({4}new TCallback(), new {2}(), new System.ServiceModel.EndpointAddress(new Uri({0}), Identity{1}));", GenerateClientEndpointURI(o, false, false), ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}Service<TCallback>({3}string Address, System.ServiceModel.EndpointIdentity Identity{1}) where TCallback : class, new()", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {3}Proxy({4}new TCallback(), new {2}(), new System.ServiceModel.EndpointAddress(new Uri({0}), Identity{1}));", GenerateClientEndpointURI(o, true, false), ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {1}Proxy Create{0}Service<TCallback>({2}int Port, System.ServiceModel.EndpointIdentity Identity) where TCallback : class, new()", o.Name, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {3}Proxy({4}new TCallback(), new {2}(), new System.ServiceModel.EndpointAddress(new Uri({0}), Identity{1}));", GenerateClientEndpointURI(o, false, true), ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {1}Proxy Create{0}Service<TCallback>({2}string Address, int Port, System.ServiceModel.EndpointIdentity Identity) where TCallback : class, new()", o.Name, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {3}Proxy({4}new TCallback(), new {2}(), new System.ServiceModel.EndpointAddress(new Uri({0}), Identity{1}));", GenerateClientEndpointURI(o, true, true), ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");
				#endregion
				#endregion

				#region - Generate Duplex Binding Endpoint Configuration Functions -
				#region - Generate Endpoint Functions WITHOUT EndpointIdentity Parameter -
				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}ServiceConfig<TCallback>({3}string EndpointConfig{1}) where TCallback : class, new()", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {4}Proxy({5}new TCallback(), EndpointConfig, new System.ServiceModel.EndpointAddress(new Uri({0}){1}{2}));", GenerateClientEndpointURI(o, false, false), identity, ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}ServiceConfig<TCallback>({3}string EndpointConfig, string Address{1}) where TCallback : class, new()", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {4}Proxy({5}new TCallback(), EndpointConfig, new System.ServiceModel.EndpointAddress(new Uri({0}){1}{2}));", GenerateClientEndpointURI(o, true, false), identity, ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}ServiceConfig<TCallback>({3}string EndpointConfig, int Port{1}) where TCallback : class, new()", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {4}Proxy({5}new TCallback(), EndpointConfig, new System.ServiceModel.EndpointAddress(new Uri({0}){1}{2}));", GenerateClientEndpointURI(o, false, true), identity, ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}ServiceConfig<TCallback>({3}string EndpointConfig, string Address, int Port{1}) where TCallback : class, new()", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {4}Proxy({5}new TCallback(), EndpointConfig, new System.ServiceModel.EndpointAddress(new Uri({0}){1}{2}));", GenerateClientEndpointURI(o, true, true), identity, ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");
				#endregion

				#region - Generate Endpoint Functions WITH EndpointIdentity Parameter -
				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}ServiceConfig<TCallback>({3}string EndpointConfig, System.ServiceModel.EndpointIdentity Identity{1}) where TCallback : class, new()", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {3}Proxy({4}new TCallback(), EndpointConfig, new System.ServiceModel.EndpointAddress(new Uri({0}), Identity{1}));", GenerateClientEndpointURI(o, false, false), ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {2}Proxy Create{0}ServiceConfig<TCallback>({3}string EndpointConfig, string Address, System.ServiceModel.EndpointIdentity Identity{1}) where TCallback : class, new()", o.Name, certificateidentity, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {3}Proxy({4}new TCallback(), EndpointConfig, new System.ServiceModel.EndpointAddress(new Uri({0}), Identity{1}));", GenerateClientEndpointURI(o, true, false), ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {1}Proxy Create{0}ServiceConfig<TCallback>({2}string EndpointConfig, int Port, System.ServiceModel.EndpointIdentity Identity) where TCallback : class, new()", o.Name, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {3}Proxy({4}new TCallback(), EndpointConfig, new System.ServiceModel.EndpointAddress(new Uri({0}), Identity{1}));", GenerateClientEndpointURI(o, false, true), ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");

				code.AppendLine(string.Format("\t\tpublic static {1}Proxy Create{0}ServiceConfig<TCallback>({2}string EndpointConfig, string Address, int Port, System.ServiceModel.EndpointIdentity Identity) where TCallback : class, new()", o.Name, DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "Guid ClientID, " : ""));
				code.AppendLine("\t\t{");
				ahlist = "";
				for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
				{
					code.AppendLine(string.Format("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace));
					ahlist += (", ah" + i);
				}
				code.AppendLine(string.Format("\t\t\tvar t = new {3}Proxy({4}new TCallback(), EndpointConfig, new System.ServiceModel.EndpointAddress(new Uri({0}), Identity{1}));", GenerateClientEndpointURI(o, true, true), ahlist, DataTypeGenerator.GenerateType(o.Binding.HasClientType ? o.Binding.ClientType : o.Binding), DataTypeGenerator.GenerateType(o.Parent.Service.HasClientType ? o.Parent.Service.ClientType : o.Parent.Service), IsDCMEnabled ? "ClientID, " : ""));
				code.AppendLine(string.Format("\t\t\treturn t;"));
				code.AppendLine("\t\t}");
				#endregion
				#endregion
			}

			return code.ToString();
		}

		#region - Generate Server Endpoint Uri -
		public static string GenerateServerEndpointURI(WcfHostEndpoint o)
		{
			string uri = $"http://localhost{(o.ServerPort > 0 ? $":{o.ServerPort}" : "")}/{o.Name}";

			if (string.IsNullOrEmpty(o.ServerAddress))
			{
				if (o.Binding.GetType() == typeof(WcfBindingBasicHTTP) || o.Binding.GetType() == typeof(WcfBindingNetHTTP) || o.Binding.GetType() == typeof(WcfBindingWebHTTP) || o.Binding.GetType() == typeof(WcfBindingWSHTTP) || o.Binding.GetType() == typeof(WcfBindingWS2007HTTP) || o.Binding.GetType() == typeof(WcfBindingWSDualHTTP) || o.Binding.GetType() == typeof(WcfBindingWSFederationHTTP) || o.Binding.GetType() == typeof(WcfBindingWS2007FederationHTTP))
				{
					if (o.ServerUseHTTPS == false)
					{
						uri = $"http://\" + Environment.MachineName + \"{(o.ServerPort > 0 ? $":{o.ServerPort}" : "")}/{o.Name}";
					}
					else
					{
						uri = $"https://\" + Environment.MachineName + \"{(o.ServerPort > 0 ? $":{o.ServerPort}" : "")}/{o.Name}";
					}
				}
				else if (o.Binding.GetType() == typeof(WcfBindingBasicHTTPS) || o.Binding.GetType() == typeof(WcfBindingNetHTTPS))
				{
					uri = $"https://\" + Environment.MachineName + \"{(o.ServerPort > 0 ? $":{o.ServerPort}" : "")}/{o.Name}";
				}
				else if (o.Binding.GetType() == typeof(WcfBindingTCP))
				{
					uri = $"net.tcp://\" + Environment.MachineName + \"{(o.ServerPort > 0 ? $":{o.ServerPort}" : "")}/{o.Name}";
				}
				else if (o.Binding.GetType() == typeof(WcfBindingPeerTCP))
				{
					uri = $"net.p2p://\" + Environment.MachineName + \"{(o.ServerPort > 0 ? $":{o.ServerPort}" : "")}/{o.Name}";
				}
				else if (o.Binding.GetType() == typeof(WcfBindingMSMQ) || o.Binding.GetType() == typeof(WcfBindingMSMQIntegration))
				{
					uri = $"net.msmq://\" + Environment.MachineName + \"{(o.ServerPort > 0 ? $":{o.ServerPort}" : "")}/{o.Name}";
				}
				else if (o.Binding.GetType() == typeof(WcfBindingNamedPipe))
				{
					uri = $"net.pipe://\" + Environment.MachineName + \"{(o.ServerPort > 0 ? $":{o.ServerPort}" : "")}/{o.Name}";
				}
			}
			else if (o.ServerAddressIsVariable)
			{
				if (o.Binding.GetType() == typeof(WcfBindingBasicHTTP) || o.Binding.GetType() == typeof(WcfBindingNetHTTP) || o.Binding.GetType() == typeof(WcfBindingWebHTTP) || o.Binding.GetType() == typeof(WcfBindingWSHTTP) || o.Binding.GetType() == typeof(WcfBindingWS2007HTTP) || o.Binding.GetType() == typeof(WcfBindingWSDualHTTP) || o.Binding.GetType() == typeof(WcfBindingWSFederationHTTP) || o.Binding.GetType() == typeof(WcfBindingWS2007FederationHTTP))
				{
					if (o.ServerUseHTTPS == false)
					{
						uri = string.Format("http://\" + {2} + \"{0}/{1}", o.ServerPort > 0 ? $":{o.ServerPort}" : "", o.Name, o.ServerAddress);
					}
					else
					{
						uri = string.Format("https://\" + {2} + \"{0}/{1}", o.ServerPort > 0 ? $":{o.ServerPort}" : "", o.Name, o.ServerAddress);
					}
				}
				else if (o.Binding.GetType() == typeof(WcfBindingBasicHTTPS) || o.Binding.GetType() == typeof(WcfBindingNetHTTPS))
				{
					uri = string.Format("https://\" + {2} + \"{0}/{1}", o.ServerPort > 0 ? $":{o.ServerPort}" : "", o.Name, o.ServerAddress);
				}
				else if (o.Binding.GetType() == typeof(WcfBindingTCP))
				{
					uri = string.Format("net.tcp://\" + {2} + \"{0}/{1}", o.ServerPort > 0 ? $":{o.ServerPort}" : "", o.Name, o.ServerAddress);
				}
				else if (o.Binding.GetType() == typeof(WcfBindingPeerTCP))
				{
					uri = string.Format("net.p2p://\" + {2} + \"{0}/{1}", o.ServerPort > 0 ? $":{o.ServerPort}" : "", o.Name, o.ServerAddress);
				}
				else if (o.Binding.GetType() == typeof(WcfBindingMSMQ) || o.Binding.GetType() == typeof(WcfBindingMSMQIntegration))
				{
					uri = string.Format("net.msmq://\" + {2} + \"{0}/{1}", o.ServerPort > 0 ? $":{o.ServerPort}" : "", o.Name, o.ServerAddress);
				}
				else if (o.Binding.GetType() == typeof(WcfBindingNamedPipe))
				{
					uri = string.Format("net.pipe://\" + {2} + \"{0}/{1}", o.ServerPort > 0 ? $":{o.ServerPort}" : "", o.Name, o.ServerAddress);
				}
			}
			else
			{
				if (o.Binding.GetType() == typeof(WcfBindingBasicHTTP) || o.Binding.GetType() == typeof(WcfBindingNetHTTP) || o.Binding.GetType() == typeof(WcfBindingWebHTTP) || o.Binding.GetType() == typeof(WcfBindingWSHTTP) || o.Binding.GetType() == typeof(WcfBindingWS2007HTTP) || o.Binding.GetType() == typeof(WcfBindingWSDualHTTP) || o.Binding.GetType() == typeof(WcfBindingWSFederationHTTP) || o.Binding.GetType() == typeof(WcfBindingWS2007FederationHTTP))
				{
					if (o.ServerUseHTTPS == false)
					{
						uri = $"http://{o.ServerAddress}{(o.ServerPort > 0 ? $":{o.ServerPort}" : "")}/{o.Name}";
					}
					else
					{
						uri = $"https://{o.ServerAddress}{(o.ServerPort > 0 ? $":{o.ServerPort}" : "")}/{o.Name}";
					}
				}
				else if (o.Binding.GetType() == typeof(WcfBindingBasicHTTPS) || o.Binding.GetType() == typeof(WcfBindingNetHTTPS))
				{
					uri = $"https://{o.ServerAddress}{(o.ServerPort > 0 ? $":{o.ServerPort}" : "")}/{o.Name}";
				}
				else if (o.Binding.GetType() == typeof(WcfBindingTCP))
				{
					uri = $"net.tcp://{o.ServerAddress}{(o.ServerPort > 0 ? $":{o.ServerPort}" : "")}/{o.Name}";
				}
				else if (o.Binding.GetType() == typeof(WcfBindingPeerTCP))
				{
					uri = $"net.p2p://{o.ServerAddress}{(o.ServerPort > 0 ? $":{o.ServerPort}" : "")}/{o.Name}";
				}
				else if (o.Binding.GetType() == typeof(WcfBindingMSMQ) || o.Binding.GetType() == typeof(WcfBindingMSMQIntegration))
				{
					uri = $"net.msmq://{o.ServerAddress}{(o.ServerPort > 0 ? $":{o.ServerPort}" : "")}/{o.Name}";
				}
				else if (o.Binding.GetType() == typeof(WcfBindingNamedPipe))
				{
					uri = $"net.pipe://{o.ServerAddress}{(o.ServerPort > 0 ? $":{o.ServerPort}" : "")}/{o.Name}";
				}
			}

			return uri;
		}
		#endregion

		#region - Generate Client Endpoint Uri -
		public static string GenerateClientEndpointURI(WcfHostEndpoint o, bool IgnoreAddress, bool IgnorePort)
		{
			string uri = "";

			string tca = o.ClientAddress;
			if (string.IsNullOrEmpty(tca)) tca = "\" + Environment.MachineName + \"";

			if (IgnorePort == false)
			{
				if (IgnoreAddress == false)
				{
					if (o.Binding.GetType() == typeof(WcfBindingBasicHTTP) || o.Binding.GetType() == typeof(WcfBindingNetHTTP) || o.Binding.GetType() == typeof(WcfBindingWebHTTP) || o.Binding.GetType() == typeof(WcfBindingWSHTTP) || o.Binding.GetType() == typeof(WcfBindingWS2007HTTP) || o.Binding.GetType() == typeof(WcfBindingWSDualHTTP) || o.Binding.GetType() == typeof(WcfBindingWSFederationHTTP) || o.Binding.GetType() == typeof(WcfBindingWS2007FederationHTTP))
					{
						if (o.ServerUseHTTPS == false)
						{
							uri = "\"http://" + tca + (o.ServerPort > 0 ? $":{o.ServerPort}" : "") + "/" + o.Name + "\"";
						}
						else
						{
							uri = "\"https://" + tca + (o.ServerPort > 0 ? $":{o.ServerPort}" : "") + "/" + o.Name + "\"";
						}
					}
					else if (o.Binding.GetType() == typeof(WcfBindingBasicHTTPS) || o.Binding.GetType() == typeof(WcfBindingNetHTTPS))
					{
						uri = "\"https://" + tca + (o.ServerPort > 0 ? $":{o.ServerPort}" : "") + "/" + o.Name + "\"";
					}
					else if (o.Binding.GetType() == typeof(WcfBindingTCP))
					{
						uri = "\"net.tcp://" + tca + (o.ServerPort > 0 ? $":{o.ServerPort}" : "") + "/" + o.Name + "\"";
					}
					else if (o.Binding.GetType() == typeof(WcfBindingPeerTCP))
					{
						uri = "\"net.p2p://" + tca + (o.ServerPort > 0 ? $":{o.ServerPort}" : "") + "/" + o.Name + "\"";
					}
					else if (o.Binding.GetType() == typeof(WcfBindingMSMQ) || o.Binding.GetType() == typeof(WcfBindingMSMQIntegration))
					{
						uri = "\"net.msmq://" + tca + (o.ServerPort > 0 ? $":{o.ServerPort}" : "") + "/" + o.Name + "\"";
					}
					else if (o.Binding.GetType() == typeof(WcfBindingNamedPipe))
					{
						uri = "\"net.pipe://" + tca + "/" + o.Name + "\"";
					}
				}
				else
				{
					if (o.Binding.GetType() == typeof(WcfBindingBasicHTTP) || o.Binding.GetType() == typeof(WcfBindingNetHTTP) || o.Binding.GetType() == typeof(WcfBindingWebHTTP) || o.Binding.GetType() == typeof(WcfBindingWSHTTP) || o.Binding.GetType() == typeof(WcfBindingWS2007HTTP) || o.Binding.GetType() == typeof(WcfBindingWSDualHTTP) || o.Binding.GetType() == typeof(WcfBindingWSFederationHTTP) || o.Binding.GetType() == typeof(WcfBindingWS2007FederationHTTP))
					{
						if (o.ServerUseHTTPS == false)
						{
							uri = "string.Format(\"http://{0}" + (o.ServerPort > 0 ? $":{o.ServerPort}" : "") + "/" + o.Name + "\", Address)";
						}
						else
						{
							uri = "string.Format(\"https://{0}" + (o.ServerPort > 0 ? $":{o.ServerPort}" : "") + "/" + o.Name + "\", Address)";
						}
					}
					else if (o.Binding.GetType() == typeof(WcfBindingBasicHTTPS) || o.Binding.GetType() == typeof(WcfBindingNetHTTPS))
					{
						uri = "string.Format(\"https://{0}" + (o.ServerPort > 0 ? $":{o.ServerPort}" : "") + "/" + o.Name + "\", Address)";
					}
					else if (o.Binding.GetType() == typeof(WcfBindingTCP))
					{
						uri = "string.Format(\"net.tcp://{0}" + (o.ServerPort > 0 ? $":{o.ServerPort}" : "") + "/" + o.Name + "\", Address)";
					}
					else if (o.Binding.GetType() == typeof(WcfBindingPeerTCP))
					{
						uri = "string.Format(\"net.p2p://{0}" + (o.ServerPort > 0 ? $":{o.ServerPort}" : "") + "/" + o.Name + "\", Address)";
					}
					else if (o.Binding.GetType() == typeof(WcfBindingMSMQ) || o.Binding.GetType() == typeof(WcfBindingMSMQIntegration))
					{
						uri = "string.Format(\"net.msmq://{0}" + (o.ServerPort > 0 ? $":{o.ServerPort}" : "") + "/" + o.Name + "\", Address)";
					}
					else if (o.Binding.GetType() == typeof(WcfBindingNamedPipe))
					{
						uri = "string.Format(\"net.pipe://{0}/" + o.Name + "\", Address)";
					}
				}
			}
			else
			{
				if (IgnoreAddress == false)
				{
					if (o.Binding.GetType() == typeof(WcfBindingBasicHTTP) || o.Binding.GetType() == typeof(WcfBindingNetHTTP) || o.Binding.GetType() == typeof(WcfBindingWebHTTP) || o.Binding.GetType() == typeof(WcfBindingWSHTTP) || o.Binding.GetType() == typeof(WcfBindingWS2007HTTP) || o.Binding.GetType() == typeof(WcfBindingWSDualHTTP) || o.Binding.GetType() == typeof(WcfBindingWSFederationHTTP) || o.Binding.GetType() == typeof(WcfBindingWS2007FederationHTTP))
					{
						if (o.ServerUseHTTPS == false)
						{
							uri = "string.Format(\"http://" + tca + "{0}/" + o.Name + "\", Port > 0 ? string.Format(\":{0}\", Port) : \"\")";
						}
						else
						{
							uri = "string.Format(\"https://" + tca + "{0}/" + o.Name + "\", Port > 0 ? string.Format(\":{0}\", Port) : \"\")";
						}
					}
					else if (o.Binding.GetType() == typeof(WcfBindingBasicHTTPS) || o.Binding.GetType() == typeof(WcfBindingNetHTTPS))
					{
						uri = "string.Format(\"https://" + tca + "{0}/" + o.Name + "\", Port > 0 ? string.Format(\":{0}\", Port) : \"\")";
					}
					else if (o.Binding.GetType() == typeof(WcfBindingTCP))
					{
						uri = "string.Format(\"net.tcp://" + tca + "{0}/" + o.Name + "\", Port > 0 ? string.Format(\":{0}\", Port) : \"\")";
					}
					else if (o.Binding.GetType() == typeof(WcfBindingPeerTCP))
					{
						uri = "string.Format(\"net.p2p://" + tca + "{0}/" + o.Name + "\", Port > 0 ? string.Format(\":{0}\", Port) : \"\")";
					}
					else if (o.Binding.GetType() == typeof(WcfBindingMSMQ) || o.Binding.GetType() == typeof(WcfBindingMSMQIntegration))
					{
						uri = "string.Format(\"net.msmq://" + tca + "{0}/" + o.Name + "\", Port > 0 ? string.Format(\":{0}\", Port) : \"\")";
					}
					else if (o.Binding.GetType() == typeof(WcfBindingNamedPipe))
					{
						uri = "\"net.pipe://" + tca + "/" + o.Name + "\"";
					}
				}
				else
				{
					if (o.Binding.GetType() == typeof(WcfBindingBasicHTTP) || o.Binding.GetType() == typeof(WcfBindingNetHTTP) || o.Binding.GetType() == typeof(WcfBindingWebHTTP) || o.Binding.GetType() == typeof(WcfBindingWSHTTP) || o.Binding.GetType() == typeof(WcfBindingWS2007HTTP) || o.Binding.GetType() == typeof(WcfBindingWSDualHTTP) || o.Binding.GetType() == typeof(WcfBindingWSFederationHTTP) || o.Binding.GetType() == typeof(WcfBindingWS2007FederationHTTP))
					{
						if (o.ServerUseHTTPS == false)
						{
							uri = "string.Format(\"http://{0}{1}/" + o.Name + "\", Address, Port > 0 ? string.Format(\":{0}\", Port) : \"\")";
						}
						else
						{
							uri = "string.Format(\"https://{0}{1}/" + o.Name + "\", Address, Port > 0 ? string.Format(\":{0}\", Port) : \"\")";
						}
					}
					else if (o.Binding.GetType() == typeof(WcfBindingBasicHTTPS) || o.Binding.GetType() == typeof(WcfBindingNetHTTPS))
					{
						uri = "string.Format(\"https://{0}{1}/" + o.Name + "\", Address, Port > 0 ? string.Format(\":{0}\", Port) : \"\")";
					}
					else if (o.Binding.GetType() == typeof(WcfBindingTCP))
					{
						uri = "string.Format(\"net.tcp://{0}{1}/" + o.Name + "\", Address, Port > 0 ? string.Format(\":{0}\", Port) : \"\")";
					}
					else if (o.Binding.GetType() == typeof(WcfBindingPeerTCP))
					{
						uri = "string.Format(\"net.p2p://{0}{1}/" + o.Name + "\", Address, Port > 0 ? string.Format(\":{0}\", Port) : \"\")";
					}
					else if (o.Binding.GetType() == typeof(WcfBindingMSMQ) || o.Binding.GetType() == typeof(WcfBindingMSMQIntegration))
					{
						uri = "string.Format(\"net.msmq://{0}{1}/" + o.Name + "\", Address, Port > 0 ? string.Format(\":{0}\", Port) : \"\")";
					}
					else if (o.Binding.GetType() == typeof(WcfBindingNamedPipe))
					{
						uri = "string.Format(\"net.pipe://{0}/" + o.Name + "\", Address)";
					}
				}
			}

			return uri;
		}
		#endregion


		public static string GenerateCredentialsCode40(WcfHostCredentials o)
		{
			return GenerateCredentialsCode45(o);
		}

		public static string GenerateCredentialsCode45(WcfHostCredentials o)
		{
			var code = new StringBuilder();
			if (o.UseCertificatesSecurity)
			{
				code.AppendFormat("\t\t\tthis.Credentials.ClientCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), o.ClientCertificateAuthenticationValidationMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.ClientCertificate.Authentication.IncludeWindowsGroups = {0};{1}", o.ClientCertificateAuthenticationIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.ClientCertificate.Authentication.MapClientCertificateToWindowsAccount = {0};{1}", o.ClientCertificateAuthenticationMapClientCertificateToWindowsAccount ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.ClientCertificate.Authentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), o.ClientCertificateAuthenticationRevocationMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.ClientCertificate.Authentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), o.ClientCertificateAuthenticationStoreLocation), Environment.NewLine);
			}
			if (o.UseIssuedTokenSecurity)
			{
				foreach (string aauri in o.IssuedTokenAllowedAudiencesUris) code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.AllowedAudienceUris.Add(\"{0}\");{1}", aauri, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.IssuedTokenAuthentication.AllowUntrustedRsaIssuers = {0};{1}", o.IssuedTokenAllowUntrustedRsaIssuers ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.IssuedTokenAuthentication.AudienceUriMode = System.IdentityModel.Selectors.AudienceUriMode.{0};{1}", System.Enum.GetName(typeof(System.IdentityModel.Selectors.AudienceUriMode), o.IssuedTokenAudienceUriMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.IssuedTokenAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), o.IssuedTokenCertificateValidationMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.IssuedTokenAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), o.IssuedTokenRevocationMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.IssuedTokenAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), o.IssuedTokenTrustedStoreLocation), Environment.NewLine);
			}
			if (o.UsePeerSecurity)
			{
				code.AppendFormat("\t\t\tthis.Credentials.Peer.MeshPassword = \"{0}\";{1}", o.PeerMeshPassword, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.Peer.MessageSenderAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), o.PeerMessageSenderAuthenticationCertificateValidationMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.Peer.MessageSenderAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), o.PeerMessageSenderAuthenticationRevocationMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.Peer.MessageSenderAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), o.PeerMessageSenderAuthenticationTrustedStoreLocation), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.Peer.PeerAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), o.PeerAuthenticationCertificateValidationMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.Peer.PeerAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), o.PeerAuthenticationRevocationMode), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.Peer.PeerAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), o.PeerAuthenticationTrustedStoreLocation), Environment.NewLine);
			}
			if (o.UseUserNamePasswordSecurity)
			{
				code.AppendFormat("\t\t\tthis.Credentials.UserNameAuthentication.CachedLogonTokenLifetime = new TimeSpan({0});{1}", o.UserNamePasswordCachedLogonTokenLifetime.Ticks, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.UserNameAuthentication.CacheLogonTokens = {0};{1}", o.UserNamePasswordCacheLogonTokens ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.UserNameAuthentication.IncludeWindowsGroups = {0};{1}", o.UserNamePasswordIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.UserNameAuthentication.MaxCachedLogonTokens = {0};{1}", o.UserNamePasswordMaxCachedLogonTokens, Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = System.ServiceModel.Security.UserNamePasswordValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.UserNamePasswordValidationMode), o.UserNamePasswordValidationMode), Environment.NewLine);
			}
			if (o.UseWindowsServiceSecurity)
			{
				code.AppendFormat("\t\t\tthis.Credentials.WindowsAuthentication.AllowAnonymousLogons = {0};{1}", o.WindowsServiceAllowAnonymousLogons ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				code.AppendFormat("\t\t\tthis.Credentials.WindowsAuthentication.IncludeWindowsGroups = {0};{1}", o.WindowsServiceIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			}
			return code.ToString();
		}

		public static string GenerateBehaviorCode45(WcfHostBehavior o)
		{
			Type t = o.GetType();
			var code = new StringBuilder();
			if (t == typeof(WcfHostDebugBehavior))
			{
				var b = o as WcfHostDebugBehavior;
				if (b == null) return "";
				code.AppendLine($"\t\t\tthis.{b.Name} = new ServiceDebugBehavior();");
				if (b.HttpHelpPageEnabled)
				{
					code.AppendLine($"\t\t\tthis.{b.Name}.HttpHelpPageEnabled = {(b.HttpHelpPageEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
					code.AppendLine($"\t\t\tthis.{b.Name}.HttpHelpPageBinding = new {DataTypeGenerator.GenerateType(b.HttpHelpPageBinding)}();");
					code.AppendLine($"\t\t\tthis.{b.Name}.HttpHelpPageUrl = new Uri(\"{b.HttpHelpPageUrl}\");");
				}
				if (b.HttpsHelpPageEnabled)
				{
					code.AppendLine($"\t\t\tthis.{b.Name}.HttpsHelpPageBinding = new {DataTypeGenerator.GenerateType(b.HttpsHelpPageBinding)}();");
					code.AppendLine($"\t\t\tthis.{b.Name}.HttpsHelpPageEnabled = {(b.HttpsHelpPageEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
					code.AppendLine($"\t\t\tthis.{b.Name}.HttpsHelpPageUrl = new Uri(\"{b.HttpsHelpPageUrl}\");");
				}
				code.AppendLine($"\t\t\tthis.{b.Name}.IncludeExceptionDetailInFaults = {(b.IncludeExceptionDetailInFaults ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
				if (b.IsDefaultBehavior) code.AppendLine($"\t\t\tthis.Description.Behaviors.Add({b.Name});");
			}
			if (t == typeof(WcfHostMetadataBehavior))
			{
				var b = o as WcfHostMetadataBehavior;
				if (b == null) return "";
				code.AppendLine($"\t\t\tthis.{b.Name} = new ServiceMetadataBehavior();");
				code.AppendLine($"\t\t\tthis.{b.Name}.ExternalMetadataLocation = new Uri(\"{b.ExternalMetadataLocation}\");");
				if (b.HttpGetEnabled)
				{
					code.AppendLine($"\t\t\tthis.{b.Name}.HttpGetBinding = new {DataTypeGenerator.GenerateType(b.HttpGetBinding)}();");
					code.AppendLine($"\t\t\tthis.{b.Name}.HttpGetEnabled = {(b.HttpGetEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
					code.AppendLine($"\t\t\tthis.{b.Name}.HttpGetUrl = new Uri(\"{b.HttpGetUrl}\");");
				}
				if (b.HttpsGetEnabled)
				{
					code.AppendLine($"\t\t\tthis.{b.Name}.HttpsGetBinding = new {DataTypeGenerator.GenerateType(b.HttpsGetBinding)}();");
					code.AppendLine($"\t\t\tthis.{b.Name}.HttpsGetEnabled = {(b.HttpsGetEnabled ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower())};");
					code.AppendLine($"\t\t\tthis.{b.Name}.HttpsGetUrl = new Uri(\"{b.HttpsGetUrl}\");");
				}
				if (b.IsDefaultBehavior) code.AppendLine($"\t\t\tthis.Description.Behaviors.Add({b.Name});");
			}
			if (t == typeof(WcfHostThrottlingBehavior))
			{
				var b = o as WcfHostThrottlingBehavior;
				if (b == null) return "";
				code.AppendLine($"\t\t\tthis.{b.Name} = new ServiceThrottlingBehavior();");
				code.AppendLine($"\t\t\tthis.{b.Name}.MaxConcurrentCalls = {b.MaxConcurrentCalls};");
				code.AppendLine($"\t\t\tthis.{b.Name}.MaxConcurrentInstances = {b.MaxConcurrentInstances};");
				code.AppendLine($"\t\t\tthis.{b.Name}.MaxConcurrentSessions = {b.MaxConcurrentSessions};");
				if (b.IsDefaultBehavior) code.AppendLine($"\t\t\tthis.Description.Behaviors.Add({b.Name});");
			}
			return code.ToString();
		}
	}
}