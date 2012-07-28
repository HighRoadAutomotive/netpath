using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFArchitect.Compiler.Generators
{
	internal static class HostCSGenerator
	{
		public static bool VerifyCode(Projects.Host o)
		{
			bool NoErrors = true;

			if (o.Name == "" || o.Name == null)
			{
				Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5000", "A host in the '" + o.Parent.Name + "' project has a blank Code Name. A Code Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.Name) == false)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5001", "The host '" + o.Name + "' in the '" + o.Parent.Name + "' project contains invalid characters in the Code Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
					NoErrors = false;
				}
			if (o.Namespace == "" || o.Namespace == null) { }
			else
				if (Helpers.RegExs.MatchHTTPURI.IsMatch(o.Namespace) == false)
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5002", "The Namespace URI '" + o.Namespace + "' for the '" + o.Name + "' host in the '" + o.Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, o.Parent, o, o.GetType()));

			if (o.Service == null)
				Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5003", "The host '" + o.Name + "' in the '" + o.Parent.Name + "' project has no Service associated with it and will not be generated. A Service must be associated with this Host for it be generated.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, o.Parent, o, o.GetType()));

			foreach (string BA in o.BaseAddresses)
			{
				bool BAValid = false;
				if (Helpers.RegExs.MatchHTTPURI.IsMatch(BA) == true) BAValid = true;
				if (Helpers.RegExs.MatchTCPURI.IsMatch(BA) == true) BAValid = true;
				if (Helpers.RegExs.MatchP2PURI.IsMatch(BA) == true) BAValid = true;
				if (Helpers.RegExs.MatchPipeURI.IsMatch(BA) == true) BAValid = true;
				if (Helpers.RegExs.MatchMSMQURI.IsMatch(BA) == true) BAValid = true;
				if (BAValid == true)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5003", "The URI '" + BA + "' for the '" + o.Name + "' host in the '" + o.Parent.Name + "' project is not a valid URI. Any associated services and data may not function properly.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, o.Parent, o, o.GetType()));
				}
			}

			foreach (Projects.HostEndpoint HE in o.Endpoints)
			{
				if (HE.Name == "" || HE.Name == null)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5004", "A host in the endpoint '" + HE.Parent.Name + "' project has a blank Code Name. A Code Name MUST be spcified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, HE.Parent, HE, HE.GetType()));
					NoErrors = false;
				}
				else
					if (Helpers.RegExs.MatchCodeName.IsMatch(HE.Name) == false)
					{
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5005", "The host endpoint '" + HE.Name + "' in the '" + HE.Parent.Name + "' project contains invalid characters in the Code Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, HE.Parent, HE, HE.GetType()));
						NoErrors = false;
					}
				if (HE.Binding == null)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5006", "The host endpoint'" + HE.Name + "' in the '" + HE.Parent.Name + "' must have a Binding. Please specify a Binding", WCFArchitect.Compiler.CompileMessageSeverity.Error, HE.Parent, HE, HE.GetType()));
					NoErrors = false;
				}
			}

			foreach (Projects.HostBehavior HB in o.Behaviors)
			{
				Type t = HB.GetType();
				if (t == typeof(Projects.HostDebugBehavior))
				{
					Projects.HostDebugBehavior b = HB as Projects.HostDebugBehavior;
					if (Helpers.RegExs.MatchHTTPURI.IsMatch(b.HttpHelpPageUrl) == false)
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5007", "The HTTP Help Page URL '" + b.HttpHelpPageUrl + "' for the '" + b.Parent.Name + "' host in the '" + b.Parent.Parent.Name + "' project is not a valid URI. The software may not be able to access the specified page.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
					if (Helpers.RegExs.MatchHTTPURI.IsMatch(b.HttpsHelpPageUrl) == false)
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5008", "The HTTPS Help Page URL '" + b.HttpsHelpPageUrl + "' for the '" + b.Parent.Name + "' host in the '" + b.Parent.Parent.Name + "' project is not a valid URI. The software may not be able to access the specified page.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
				}
				else if (t == typeof(Projects.HostMetadataBehavior))
				{
					Projects.HostMetadataBehavior b = HB as Projects.HostMetadataBehavior;
					if (Helpers.RegExs.MatchHTTPURI.IsMatch(b.HttpGetUrl) == false)
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5009", "The HTTP Get URL '" + b.HttpGetUrl + "' for the '" + b.Parent.Name + "' host in the '" + b.Parent.Parent.Name + "' project is not a valid URI. The software may not be able to access the specified page.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
					if (Helpers.RegExs.MatchHTTPURI.IsMatch(b.HttpsGetUrl) == false)
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5010", "The HTTPS Get URL '" + b.HttpsGetUrl + "' for the '" + b.Parent.Name + "' host in the '" + b.Parent.Parent.Name + "' project is not a valid URI. The software may not be able to access the specified page.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, b.Parent, b, b.GetType()));
				}
				else if (t == typeof(Projects.HostThrottlingBehavior))
				{
				}
			}

			return NoErrors;
		} 


		public static string GenerateServerCode30(Projects.Host o)
		{
			if (o.Service == null) return "";

			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0} : ServiceHost{1}", DataTypeCSGenerator.GenerateType(o), Environment.NewLine);
			Code.AppendLine("\t{");
			string BAVars = "";
			for (int i = 0; i < o.BaseAddresses.Count; i++)
			{
				Code.AppendFormat("\t\tprivate static Uri BaseAddr{0} = new Uri(\"{1}\");{2}", i, o.BaseAddresses[i], Environment.NewLine);
				BAVars += ", " + o.Name + ".BaseAddr" + i.ToString();
			}

			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendFormat("\t\tpublic static Uri {0}URI {{ get {{ return new Uri(\"{1}\"); }} }}{2}", HE.Name, GenerateServerEndpointURI(HE), Environment.NewLine);

			foreach (Projects.HostBehavior HB in o.Behaviors)
			{
				if (HB.GetType() == typeof(Projects.HostDebugBehavior))
				{
					Code.AppendFormat("\t\tprivate ServiceDebugBehavior m_{0} = null;{1}", HB.Name, Environment.NewLine);
					Code.AppendFormat("\t\tpublic ServiceDebugBehavior {0} {{ get {{ return m_{0} }} private set {{ m_{0} = value; }} }}{1}", HB.Name, Environment.NewLine);
				}
				if (HB.GetType() == typeof(Projects.HostMetadataBehavior))
				{
					Code.AppendFormat("\t\tprivate ServiceMetadataBehavior m_{0} = null;{1}", HB.Name, Environment.NewLine);
					Code.AppendFormat("\t\tpublic ServiceMetadataBehavior {0} {{ get {{ return m_{0} }} private set {{ m_{0} = value; }} }}{1}", HB.Name, Environment.NewLine);
				}
				if (HB.GetType() == typeof(Projects.HostThrottlingBehavior))
				{
					Code.AppendFormat("\t\tprivate ServiceThrottlingBehavior m_{0} = null;{1}", HB.Name, Environment.NewLine);
					Code.AppendFormat("\t\tpublic ServiceThrottlingBehavior {0} {{ get {{ return m_{0} }} private set {{ m_{0} = value; }} }}{1}", HB.Name, Environment.NewLine);
				}
			}
			#region - Generate Default Constructors -
			//Generate Singleton Service Constructor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance) : base(singletonInstance{1}){2}", o.Name, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode30(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode30(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine(GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType) : base(serviceType{1}){2}", o.Name, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode30(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode30(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine(GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t}");

			//Generate Singleton Service Constructor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode30(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode30(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine(GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode30(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode30(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine(GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t\tthis.Context = new InstanceContext(this);");
			Code.AppendLine("\t\t}");
			#endregion

			#region - Generate Optional Endpoint Constructors -
			//Generate Singleton Service Constructor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, bool DisableDefaultEndpoints) : base(singletonInstance{1}){2}", o.Name, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode30(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode30(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine("\t" + GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, bool DisableDefaultEndpoints) : base(serviceType{1}){2}", o.Name, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode30(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode30(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine("\t" + GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");

			//Generate Singleton Service Constructor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode30(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode30(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine("\t" + GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode30(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode30(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine("\t" + GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");
			#endregion

			Code.AppendLine("\t}");

			return Code.ToString();
		}

		public static string GenerateServerCode35(Projects.Host o)
		{
			if (o.Service == null) return "";

			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0} : ServiceHost{1}", DataTypeCSGenerator.GenerateType(o), Environment.NewLine);
			Code.AppendLine("\t{");
			string BAVars = "";
			for (int i = 0; i < o.BaseAddresses.Count; i++)
			{
				Code.AppendFormat("\t\tprivate static Uri BaseAddr{0} = new Uri(\"{1}\");{2}", i, o.BaseAddresses[i], Environment.NewLine);
				BAVars += ", " + o.Name + ".BaseAddr" + i.ToString();
			}

			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendFormat("\t\tpublic static Uri {0}URI {{ get {{ return new Uri(\"{1}\"); }} }}{2}", HE.Name, GenerateServerEndpointURI(HE), Environment.NewLine);

			foreach (Projects.HostBehavior HB in o.Behaviors)
			{
				if (HB.GetType() == typeof(Projects.HostDebugBehavior))
				{
					Code.AppendFormat("\t\tprivate ServiceDebugBehavior m_{0} = null;{1}", HB.Name, Environment.NewLine);
					Code.AppendFormat("\t\tpublic ServiceDebugBehavior {0} {{ get {{ return m_{0} }} private set {{ m_{0} = value; }} }}{1}", HB.Name, Environment.NewLine);
				}
				if (HB.GetType() == typeof(Projects.HostMetadataBehavior))
				{
					Code.AppendFormat("\t\tprivate ServiceMetadataBehavior m_{0} = null;{1}", HB.Name, Environment.NewLine);
					Code.AppendFormat("\t\tpublic ServiceMetadataBehavior {0} {{ get {{ return m_{0} }} private set {{ m_{0} = value; }} }}{1}", HB.Name, Environment.NewLine);
				}
				if (HB.GetType() == typeof(Projects. HostThrottlingBehavior))
				{
					Code.AppendFormat("\t\tprivate ServiceThrottlingBehavior m_{0} = null;{1}", HB.Name, Environment.NewLine);
					Code.AppendFormat("\t\tpublic ServiceThrottlingBehavior {0} {{ get {{ return m_{0} }} private set {{ m_{0} = value; }} }}{1}", HB.Name, Environment.NewLine);
				}
			}

			#region - Generate Default Constructors -
			//Generate Singleton Service Constructor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance) : base(singletonInstance{1}){2}", o.Name, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode35(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode35(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine(GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType) : base(serviceType{1}){2}", o.Name, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode35(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode35(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine(GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t}");

			//Generate Singleton Service Constructor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode35(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode35(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine(GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode35(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode35(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine(GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t}");
			#endregion

			#region - Generate Optional Endpoint Constructors -
			//Generate Singleton Service Constructor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, bool DisableDefaultEndpoints) : base(singletonInstance{1}){2}", o.Name, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode35(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode35(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine("\t" + GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t\tthis.Context = new InstanceContext(this);");
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, bool DisableDefaultEndpoints) : base(serviceType{1}){2}", o.Name, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode35(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode35(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine("\t" + GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t\tthis.Context = new InstanceContext(this);");
			Code.AppendLine("\t\t}");

			//Generate Singleton Service Constructor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode35(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode35(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine("\t" + GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t\tthis.Context = new InstanceContext(this);");
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode35(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode35(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine("\t" + GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t\tthis.Context = new InstanceContext(this);");
			Code.AppendLine("\t\t}");
			#endregion

			Code.AppendLine("\t}");

			return Code.ToString();
		}

		public static string GenerateServerCode40(Projects.Host o)
		{
			return GenerateServerCode45(o);
		}

		public static string GenerateServerCode45(Projects.Host o)
		{
			if (o.Service == null) return "";

			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0} : ServiceHost{1}", DataTypeCSGenerator.GenerateType(o), Environment.NewLine);
			Code.AppendLine("\t{");
			string BAVars = "";
			for (int i = 0; i < o.BaseAddresses.Count; i++)
			{
				Code.AppendFormat("\t\tprivate static Uri BaseAddr{0} = new Uri(\"{1}\");{2}", i, o.BaseAddresses[i], Environment.NewLine);
				BAVars += ", " + o.Name + ".BaseAddr" + i.ToString();
			}

			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendFormat("\t\tpublic static Uri {0}URI {{ get {{ return new Uri(\"{1}\"); }} }}{2}", HE.Name, GenerateServerEndpointURI(HE), Environment.NewLine);

			foreach (Projects.HostBehavior HB in o.Behaviors)
			{
				if (HB.GetType() == typeof(Projects.HostDebugBehavior)) Code.AppendFormat("\t\tpublic ServiceDebugBehavior {0} {{ get; private set; }}{1}", HB.Name, Environment.NewLine);
				if (HB.GetType() == typeof(Projects.HostMetadataBehavior)) Code.AppendFormat("\t\tpublic ServiceMetadataBehavior {0} {{ get; private set; }}{1}", HB.Name, Environment.NewLine);
				if (HB.GetType() == typeof(Projects.HostThrottlingBehavior)) Code.AppendFormat("\t\tpublic ServiceThrottlingBehavior {0} {{ get; private set; }}{1}", HB.Name, Environment.NewLine);
			}

			#region - Generate Default Constructors
			//Generate Singleton Service Constructor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance) : base(singletonInstance{1}){2}", o.Name, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode45(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode45(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine(GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t\tthis.Context = new InstanceContext(this);");
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType) : base(serviceType{1}){2}", o.Name, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode45(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode45(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine(GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t\tthis.Context = new InstanceContext(this);");
			Code.AppendLine("\t\t}");

			//Generate Singleton Service Constructor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode45(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode45(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine(GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t\tthis.Context = new InstanceContext(this);");
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode45(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode45(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine(GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t\tthis.Context = new InstanceContext(this);");
			Code.AppendLine("\t\t}");
			#endregion

			#region - Generate Optional Endpoint Constructors
			//Generate Singleton Service Constructor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, bool DisableDefaultEndpoints) : base(singletonInstance{1}){2}", o.Name, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode45(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode45(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine(GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t\tthis.Context = new InstanceContext(this);");
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, bool DisableDefaultEndpoints) : base(serviceType{1}){2}", o.Name, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode45(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode45(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine(GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t\tthis.Context = new InstanceContext(this);");
			Code.AppendLine("\t\t}");

			//Generate Singleton Service Constructor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode45(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode45(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine(GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t\tthis.Context = new InstanceContext(this);");
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(GenerateCredentialsCode45(o.Credentials));
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", o.AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), o.AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (o.CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", o.CloseTimeout.Ticks, Environment.NewLine);
			if (o.OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", o.OpenTimeout.Ticks, Environment.NewLine);
			if (o.ConfigurationName != null && o.ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", o.ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(o.Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", o.Namespace, Environment.NewLine);
			foreach (Projects.HostBehavior HB in o.Behaviors)
				Code.Append(GenerateBehaviorCode45(HB));
			if (o.ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", o.ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine(GenerateEndpointServerCode(HE));
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t\tthis.Context = new InstanceContext(this);");
			Code.AppendLine("\t\t}");
			#endregion

			Code.AppendLine("\t}");

			return Code.ToString();
		}

		public static string GenerateServerCode35Client(Projects.Host o)
		{
			return GenerateServerCode35(o);
		}

		public static string GenerateServerCode40Client(Projects.Host o)
		{
			return GenerateServerCode40(o);
		}

		public static string GenerateClientCode30(Projects.Host o)
		{
			return GenerateClientCode35(o);
		}

		public static string GenerateClientCode35(Projects.Host o)
		{
			return GenerateClientCode40(o);
		}

		public static string GenerateClientCode40(Projects.Host o)
		{
			return GenerateClientCode45(o);
		}

		public static string GenerateClientCode45(Projects.Host o)
		{
			if (o.Service == null) return "";

			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateType(o), Environment.NewLine);
			Code.AppendLine("\t{");

			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendFormat("\t\tpublic static Uri {0}URI {{ get {{ return new Uri(\"{1}\"); }} }}{2}", HE.Name, GenerateClientEndpointURI(HE, false, false), Environment.NewLine);

			foreach (Projects.HostEndpoint HE in o.Endpoints)
				Code.AppendLine(GenerateEndpointClientCode(HE));

			Code.AppendLine("\t}");

			return Code.ToString();
		}

		public static string GenerateClientCode35Client(Projects.Host o)
		{
			return GenerateClientCode40Client(o);
		}

		public static string GenerateClientCode40Client(Projects.Host o)
		{
			return GenerateClientCode40(o);
		}

		public static string GenerateEndpointServerCode(Projects.HostEndpoint o)
		{
			return string.Format("\t\t\tthis.AddServiceEndpoint(typeof({0}.I{1}), new {2}(), {3}URI);", o.Parent.Service.Parent.FullName, o.Parent.Service.Name, DataTypeCSGenerator.GenerateType(o.Binding), o.Name);
		}

		public static string GenerateEndpointClientCode(Projects.HostEndpoint o)
		{
			StringBuilder Code = new StringBuilder();

			string ahlist = "";
			string identity = "";
			string certificateidentity = "";
			if (o.ClientIdentityType != Projects.HostEndpointIdentityType.Anonymous)
			{
				if (o.ClientIdentityType == Projects.HostEndpointIdentityType.DNS) identity = ", System.ServiceModel.EndpointIdentity.CreateDnsIdentity(\"" + o.ClientIdentityData + "\")";
				if (o.ClientIdentityType == Projects.HostEndpointIdentityType.RSA) identity = ", System.ServiceModel.EndpointIdentity.CreateRsaIdentity(\"" + o.ClientIdentityData + "\")";
				if (o.ClientIdentityType == Projects.HostEndpointIdentityType.RSAX509)
				{
					identity = ", System.ServiceModel.EndpointIdentity.CreateRsaIdentity(IdentityCertificate)";
					certificateidentity = ", System.Security.Cryptography.X509Certificates.X509Certificate2 IdentityCertificate";
				}
				if (o.ClientIdentityType == Projects.HostEndpointIdentityType.SPN) identity = ", System.ServiceModel.EndpointIdentity.CreateSpnIdentity(\"" + o.ClientIdentityData + "\")";
				if (o.ClientIdentityType == Projects.HostEndpointIdentityType.UPN) identity = ", System.ServiceModel.EndpointIdentity.CreateUpnIdentity(\"" + o.ClientIdentityData + "\")";
				if (o.ClientIdentityType == Projects.HostEndpointIdentityType.X509)
				{
					identity = ", System.ServiceModel.EndpointIdentity.CreateX509CertificateIdentity(IdentityCertificate)";
					certificateidentity = ", System.Security.Cryptography.X509Certificates.X509Certificate2 IdentityCertificate";
				}
			}

			#region - Generate Endpoint Functions WITHOUT EndpointIdentity Parameter -
			Code.AppendFormat("\t\tpublic static System.ServiceModel.EndpointAddress Create{0}Endpoint({1}){2}", o.Name, certificateidentity.Replace(", ", ""), Environment.NewLine);
			Code.AppendLine("\t\t{");
			ahlist = "";
			for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
			{
				Code.AppendFormat("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});{3}", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace, Environment.NewLine);
				ahlist += (", ah" + i.ToString());
			}
			Code.AppendFormat("\t\t\treturn new System.ServiceModel.EndpointAddress(new Uri(\"{0}\"){1}{2});{3}", GenerateClientEndpointURI(o, false, false), identity, ahlist, Environment.NewLine);
			Code.AppendLine("\t\t}");

			Code.AppendFormat("\t\tpublic static System.ServiceModel.EndpointAddress Create{0}Endpoint(string Address{1}){2}", o.Name, certificateidentity, Environment.NewLine);
			Code.AppendLine("\t\t{");
			ahlist = "";
			for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
			{
				Code.AppendFormat("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});{3}", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace, Environment.NewLine);
				ahlist += (", ah" + i.ToString());
			}
			Code.AppendFormat("\t\t\treturn new System.ServiceModel.EndpointAddress(new Uri(\"{0}\"){1}{2});{3}", GenerateClientEndpointURI(o, true, false), identity, ahlist, Environment.NewLine);
			Code.AppendLine("\t\t}");

			Code.AppendFormat("\t\tpublic static System.ServiceModel.EndpointAddress Create{0}Endpoint(int Port{1}){2}", o.Name, certificateidentity, Environment.NewLine);
			Code.AppendLine("\t\t{");
			ahlist = "";
			for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
			{
				Code.AppendFormat("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});{3}", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace, Environment.NewLine);
				ahlist += (", ah" + i.ToString());
			}
			Code.AppendFormat("\t\t\treturn new System.ServiceModel.EndpointAddress(new Uri(\"{0}\"){1}{2});{3}", GenerateClientEndpointURI(o, false, true), identity, ahlist, Environment.NewLine);
			Code.AppendLine("\t\t}");

			Code.AppendFormat("\t\tpublic static System.ServiceModel.EndpointAddress Create{0}Endpoint(string Address, int Port{1}){2}", o.Name, certificateidentity, Environment.NewLine);
			Code.AppendLine("\t\t{");
			ahlist = "";
			for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
			{
				Code.AppendFormat("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});{3}", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace, Environment.NewLine);
				ahlist += (", ah" + i.ToString());
			}
			Code.AppendFormat("\t\t\treturn new System.ServiceModel.EndpointAddress(new Uri(\"{0}\"){1}{2});{3}", GenerateClientEndpointURI(o, true, true), identity, ahlist, Environment.NewLine);
			Code.AppendLine("\t\t}");
			#endregion

			#region - Generate Endpoint Functions WITH EndpointIdentity Parameter -
			Code.AppendFormat("\t\tpublic static System.ServiceModel.EndpointAddress Create{0}Endpoint(System.ServiceModel.EndpointIdentity Identity{1}){2}", o.Name, certificateidentity, Environment.NewLine);
			Code.AppendLine("\t\t{");
			ahlist = "";
			for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
			{
				Code.AppendFormat("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});{3}", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace, Environment.NewLine);
				ahlist += (", ah" + i.ToString());
			}
			Code.AppendFormat("\t\t\treturn new System.ServiceModel.EndpointAddress(new Uri(\"{0}\"), Identity{1});{2}", GenerateClientEndpointURI(o, false, false), ahlist, Environment.NewLine);
			Code.AppendLine("\t\t}");

			Code.AppendFormat("\t\tpublic static System.ServiceModel.EndpointAddress Create{0}Endpoint(string Address, System.ServiceModel.EndpointIdentity Identity{1}){2}", o.Name, certificateidentity, Environment.NewLine);
			Code.AppendLine("\t\t{");
			ahlist = "";
			for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
			{
				Code.AppendFormat("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});{3}", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace, Environment.NewLine);
				ahlist += (", ah" + i.ToString());
			}
			Code.AppendFormat("\t\t\treturn new System.ServiceModel.EndpointAddress(new Uri(\"{0}\"), Identity{1});{2}", GenerateClientEndpointURI(o, true, false), ahlist, Environment.NewLine);
			Code.AppendLine("\t\t}");

			Code.AppendFormat("\t\tpublic static System.ServiceModel.EndpointAddress Create{0}Endpoint(int Port, System.ServiceModel.EndpointIdentity Identity){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			ahlist = "";
			for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
			{
				Code.AppendFormat("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});{3}", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace, Environment.NewLine);
				ahlist += (", ah" + i.ToString());
			}
			Code.AppendFormat("\t\t\treturn new System.ServiceModel.EndpointAddress(new Uri(\"{0}\"), Identity{1});{2}", GenerateClientEndpointURI(o, false, true), ahlist, Environment.NewLine);
			Code.AppendLine("\t\t}");

			Code.AppendFormat("\t\tpublic static System.ServiceModel.EndpointAddress Create{0}Endpoint(string Address, int Port, System.ServiceModel.EndpointIdentity Identity){1}", o.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			ahlist = "";
			for (int i = 0; i < o.ClientAddressHeaders.Count; i++)
			{
				Code.AppendFormat("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});{3}", i + 1, o.ClientAddressHeaders[i].Name, o.ClientAddressHeaders[i].Namespace, Environment.NewLine);
				ahlist += (", ah" + i.ToString());
			}
			Code.AppendFormat("\t\t\treturn new System.ServiceModel.EndpointAddress(new Uri(\"{0}\"), Identity{1});{2}", GenerateClientEndpointURI(o, true, true), ahlist, Environment.NewLine);
			Code.AppendLine("\t\t}");
			#endregion

			return Code.ToString();
		}

		#region - Generate Server Endpoint URI -
		public static string GenerateServerEndpointURI(Projects.HostEndpoint o)
		{
			string URI = "";

			if (o.ServerAddress == null || o.ServerAddress == "")
			{
				if (o.Binding.GetType() == typeof(Projects.ServiceBindingBasicHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWebHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWS2007HTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSDualHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSFederationHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWS2007FederationHTTP))
				{
					if (o.ServerUseHTTPS == false)
					{
						URI = "http://\" + Environment.MachineName + \":" + o.ServerPort.ToString() + "/" + o.Name;
					}
					else
					{
						URI = "https://\" + Environment.MachineName + \":" + o.ServerPort.ToString() + "/" + o.Name;
					}
				}
				else if (o.Binding.GetType() == typeof(Projects.ServiceBindingTCP))
				{
					URI = "net.tcp://\" + Environment.MachineName + \":" + o.ServerPort.ToString() + "/" + o.Name;
				}
				else if (o.Binding.GetType() == typeof(Projects.ServiceBindingPeerTCP))
				{
					URI = "net.p2p://\" + Environment.MachineName + \":" + o.ServerPort.ToString() + "/" + o.Name;
				}
				else if (o.Binding.GetType() == typeof(Projects.ServiceBindingMSMQ) || o.Binding.GetType() == typeof(Projects.ServiceBindingMSMQIntegration))
				{
					URI = "net.msmq://\" + Environment.MachineName + \":" + o.ServerPort.ToString() + "/" + o.Name;
				}
				else if (o.Binding.GetType() == typeof(Projects.ServiceBindingNamedPipe))
				{
					URI = "net.pipe://\" + Environment.MachineName + \"/" + o.Name;
				}
			}
			else
			{
				if (o.Binding.GetType() == typeof(Projects.ServiceBindingBasicHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWebHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWS2007HTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSDualHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSFederationHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWS2007FederationHTTP))
				{
					if (o.ServerUseHTTPS == false)
					{
						URI = "http://" + o.ServerAddress + ":" + o.ServerPort.ToString() + "/" + o.Name;
					}
					else
					{
						URI = "https://" + o.ServerAddress + ":" + o.ServerPort.ToString() + "/" + o.Name;
					}
				}
				else if (o.Binding.GetType() == typeof(Projects.ServiceBindingTCP))
				{
					URI = "net.tcp://" + o.ServerAddress + ":" + o.ServerPort.ToString() + "/" + o.Name;
				}
				else if (o.Binding.GetType() == typeof(Projects.ServiceBindingPeerTCP))
				{
					URI = "net.p2p://" + o.ServerAddress + ":" + o.ServerPort.ToString() + "/" + o.Name;
				}
				else if (o.Binding.GetType() == typeof(Projects.ServiceBindingMSMQ) || o.Binding.GetType() == typeof(Projects.ServiceBindingMSMQIntegration))
				{
					URI = "net.msmq://" + o.ServerAddress + ":" + o.ServerPort.ToString() + "/" + o.Name;
				}
				else if (o.Binding.GetType() == typeof(Projects.ServiceBindingNamedPipe))
				{
					URI = "net.pipe://" + o.ServerAddress + "/" + o.Name;
				}
			}

			return URI;
		}
		#endregion

		#region - Generate Client Endpoint URI
		public static string GenerateClientEndpointURI(Projects.HostEndpoint o, bool IgnoreAddress, bool IgnorePort)
		{
			string URI = "";

			string tca = o.ClientAddress;
			if (tca == null || tca == "") tca = "\" + Environment.MachineName + \"";

			if (IgnorePort == false)
			{
				if (IgnoreAddress == false)
				{
					if (o.Binding.GetType() == typeof(Projects.ServiceBindingBasicHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWebHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWS2007HTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSDualHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSFederationHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWS2007FederationHTTP))
					{
						if (o.ServerUseHTTPS == false)
						{
							URI = "http://" + tca + ":" + o.ServerPort.ToString() + "/" + o.Name;
						}
						else
						{
							URI = "https://" + tca + ":" + o.ServerPort.ToString() + "/" + o.Name;
						}
					}
					else if (o.Binding.GetType() == typeof(Projects.ServiceBindingTCP))
					{
						URI = "net.tcp://" + tca + ":" + o.ServerPort.ToString() + "/" + o.Name;
					}
					else if (o.Binding.GetType() == typeof(Projects.ServiceBindingPeerTCP))
					{
						URI = "net.p2p://" + tca + ":" + o.ServerPort.ToString() + "/" + o.Name;
					}
					else if (o.Binding.GetType() == typeof(Projects.ServiceBindingMSMQ) || o.Binding.GetType() == typeof(Projects.ServiceBindingMSMQIntegration))
					{
						URI = "net.msmq://" + tca + ":" + o.ServerPort.ToString() + "/" + o.Name;
					}
					else if (o.Binding.GetType() == typeof(Projects.ServiceBindingNamedPipe))
					{
						URI = "net.pipe://" + tca + "/" + o.Name;
					}
				}
				else
				{
					if (o.Binding.GetType() == typeof(Projects.ServiceBindingBasicHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWebHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWS2007HTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSDualHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSFederationHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWS2007FederationHTTP))
					{
						if (o.ServerUseHTTPS == false)
						{
							URI = "http://\" + Address + \":" + o.ServerPort.ToString() + "/" + o.Name;
						}
						else
						{
							URI = "https://\" + Address + \":" + o.ServerPort.ToString() + "/" + o.Name;
						}
					}
					else if (o.Binding.GetType() == typeof(Projects.ServiceBindingTCP))
					{
						URI = "net.tcp://\" + Address + \":" + o.ServerPort.ToString() + "/" + o.Name;
					}
					else if (o.Binding.GetType() == typeof(Projects.ServiceBindingPeerTCP))
					{
						URI = "net.p2p://\" + Address + \":" + o.ServerPort.ToString() + "/" + o.Name;
					}
					else if (o.Binding.GetType() == typeof(Projects.ServiceBindingMSMQ) || o.Binding.GetType() == typeof(Projects.ServiceBindingMSMQIntegration))
					{
						URI = "net.msmq://\" + Address + \":" + o.ServerPort.ToString() + "/" + o.Name;
					}
					else if (o.Binding.GetType() == typeof(Projects.ServiceBindingNamedPipe))
					{
						URI = "net.pipe://\" + Address + \"/" + o.Name;
					}
				}
			}
			else
			{
				if (IgnoreAddress == false)
				{
					if (o.Binding.GetType() == typeof(Projects.ServiceBindingBasicHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWebHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWS2007HTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSDualHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSFederationHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWS2007FederationHTTP))
					{
						if (o.ServerUseHTTPS == false)
						{
							URI = "http://" + tca + ":\" + Port.ToString() +\"/" + o.Name;
						}
						else
						{
							URI = "https://" + tca + ":\" + Port.ToString() +\"/" + o.Name;
						}
					}
					else if (o.Binding.GetType() == typeof(Projects.ServiceBindingTCP))
					{
						URI = "net.tcp://" + tca + ":\" + Port.ToString() +\"/" + o.Name;
					}
					else if (o.Binding.GetType() == typeof(Projects.ServiceBindingPeerTCP))
					{
						URI = "net.p2p://" + tca + ":\" + Port.ToString() +\"/" + o.Name;
					}
					else if (o.Binding.GetType() == typeof(Projects.ServiceBindingMSMQ) || o.Binding.GetType() == typeof(Projects.ServiceBindingMSMQIntegration))
					{
						URI = "net.msmq://" + tca + ":\" + Port.ToString() +\"/" + o.Name;
					}
					else if (o.Binding.GetType() == typeof(Projects.ServiceBindingNamedPipe))
					{
						URI = "net.pipe://" + tca + "/" + o.Name;
					}
				}
				else
				{
					if (o.Binding.GetType() == typeof(Projects.ServiceBindingBasicHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWebHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWS2007HTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSDualHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWSFederationHTTP) || o.Binding.GetType() == typeof(Projects.ServiceBindingWS2007FederationHTTP))
					{
						if (o.ServerUseHTTPS == false)
						{
							URI = "http://\" + Address + \":\" + Port.ToString() +\"/" + o.Name;
						}
						else
						{
							URI = "https://\" + Address + \":\" + Port.ToString() +\"/" + o.Name;
						}
					}
					else if (o.Binding.GetType() == typeof(Projects.ServiceBindingTCP))
					{
						URI = "net.tcp://\" + Address + \":\" + Port.ToString() +\"/" + o.Name;
					}
					else if (o.Binding.GetType() == typeof(Projects.ServiceBindingPeerTCP))
					{
						URI = "net.p2p://\" + Address + \":\" + Port.ToString() +\"/" + o.Name;
					}
					else if (o.Binding.GetType() == typeof(Projects.ServiceBindingMSMQ) || o.Binding.GetType() == typeof(Projects.ServiceBindingMSMQIntegration))
					{
						URI = "net.msmq://\" + Address + \":\" + Port.ToString() +\"/" + o.Name;
					}
					else if (o.Binding.GetType() == typeof(Projects.ServiceBindingNamedPipe))
					{
						URI = "net.pipe://\" + Address + \"/" + o.Name;
					}
				}
			}

			return URI;
		}
		#endregion

		public static string GenerateCredentialsCode30(Projects.HostCredentials o)
		{
			StringBuilder Code = new StringBuilder();
			if (o.UseCertificatesSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.ClientCertificate.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), o.ClientCertificateAuthenticationValidationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ClientCertificate.IncludeWindowsGroups = {0};{1}", o.ClientCertificateAuthenticationIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ClientCertificate.MapClientCertificateToWindowsAccount = {0};{1}", o.ClientCertificateAuthenticationMapClientCertificateToWindowsAccount ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ClientCertificate.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), o.ClientCertificateAuthenticationRevocationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ClientCertificate.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), o.ClientCertificateAuthenticationStoreLocation), Environment.NewLine);
			}
			if (o.UseIssuedTokenSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.AllowUntrustedRsaIssuers = {0};{1}", o.IssuedTokenAllowUntrustedRsaIssuers ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), o.IssuedTokenCertificateValidationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), o.IssuedTokenRevocationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), o.IssuedTokenTrustedStoreLocation), Environment.NewLine);
			}
			if (o.UsePeerSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.Peer.MeshPassword = \"{0}\";{1}", o.PeerMeshPassword, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.MessageSenderAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), o.PeerMessageSenderAuthenticationCertificateValidationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.MessageSenderAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), o.PeerMessageSenderAuthenticationRevocationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.MessageSenderAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), o.PeerMessageSenderAuthenticationTrustedStoreLocation), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.PeerAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), o.PeerAuthenticationCertificateValidationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.PeerAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), o.PeerAuthenticationRevocationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.PeerAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), o.PeerAuthenticationTrustedStoreLocation), Environment.NewLine);
			}
			if (o.UseUserNamePasswordSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.CachedLogonTokenLifetime = new TimeSpan({0});{1}", o.UserNamePasswordCachedLogonTokenLifetime.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.CacheLogonTokens = {0};{1}", o.UserNamePasswordCacheLogonTokens ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.IncludeWindowsGroups = {0};{1}", o.UserNamePasswordIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.MaxCachedLogonTokens = {0};{1}", o.UserNamePasswordMaxCachedLogonTokens, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.UserNamePasswordValidationMode = System.ServiceModel.Security.UserNamePasswordValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.UserNamePasswordValidationMode), o.UserNamePasswordValidationMode), Environment.NewLine);
			}
			if (o.UseWindowsServiceSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.WindowsAuthentication.AllowAnonymousLogons = {0};{1}", o.WindowsServiceAllowAnonymousLogons ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.WindowsAuthentication.IncludeWindowsGroups = {0};{1}", o.WindowsServiceIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			}
			return Code.ToString();
		}

		public static string GenerateCredentialsCode35(Projects.HostCredentials o)
		{
			return GenerateCredentialsCode40(o);
		}

		public static string GenerateCredentialsCode40(Projects.HostCredentials o)
		{
			return GenerateCredentialsCode45(o);
		}

		public static string GenerateCredentialsCode45(Projects.HostCredentials o)
		{
			StringBuilder Code = new StringBuilder();
			if (o.UseCertificatesSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.ClientCertificate.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), o.ClientCertificateAuthenticationValidationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ClientCertificate.IncludeWindowsGroups = {0};{1}", o.ClientCertificateAuthenticationIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ClientCertificate.MapClientCertificateToWindowsAccount = {0};{1}", o.ClientCertificateAuthenticationMapClientCertificateToWindowsAccount ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ClientCertificate.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), o.ClientCertificateAuthenticationRevocationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ClientCertificate.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), o.ClientCertificateAuthenticationStoreLocation), Environment.NewLine);
			}
			if (o.UseIssuedTokenSecurity == true)
			{
				foreach (string AAURI in o.IssuedTokenAllowedAudiencesUris) Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.AllowedAudienceUris.Add(\"{0}\");{1}", AAURI, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.AllowUntrustedRsaIssuers = {0};{1}", o.IssuedTokenAllowUntrustedRsaIssuers ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.AudienceUriMode = System.IdentityModel.Selectors.AudienceUriMode.{0};{1}", System.Enum.GetName(typeof(System.IdentityModel.Selectors.AudienceUriMode), o.IssuedTokenAudienceUriMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), o.IssuedTokenCertificateValidationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), o.IssuedTokenRevocationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), o.IssuedTokenTrustedStoreLocation), Environment.NewLine);
			}
			if (o.UsePeerSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.Peer.MeshPassword = \"{0}\";{1}", o.PeerMeshPassword, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.MessageSenderAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), o.PeerMessageSenderAuthenticationCertificateValidationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.MessageSenderAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), o.PeerMessageSenderAuthenticationRevocationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.MessageSenderAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), o.PeerMessageSenderAuthenticationTrustedStoreLocation), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.PeerAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), o.PeerAuthenticationCertificateValidationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.PeerAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), o.PeerAuthenticationRevocationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.PeerAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), o.PeerAuthenticationTrustedStoreLocation), Environment.NewLine);
			}
			if (o.UseUserNamePasswordSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.CachedLogonTokenLifetime = new TimeSpan({0});{1}", o.UserNamePasswordCachedLogonTokenLifetime.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.CacheLogonTokens = {0};{1}", o.UserNamePasswordCacheLogonTokens ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.IncludeWindowsGroups = {0};{1}", o.UserNamePasswordIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.MaxCachedLogonTokens = {0};{1}", o.UserNamePasswordMaxCachedLogonTokens, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.CertificateValidationMode = System.ServiceModel.Security.UserNamePasswordValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.UserNamePasswordValidationMode), o.UserNamePasswordValidationMode), Environment.NewLine);
			}
			if (o.UseWindowsServiceSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.WindowsAuthentication.AllowAnonymousLogons = {0};{1}", o.WindowsServiceAllowAnonymousLogons ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.WindowsAuthentication.IncludeWindowsGroups = {0};{1}", o.WindowsServiceIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			}
			return Code.ToString();
		}

		public static string GenerateBehaviorCode30(Projects.HostBehavior o)
		{
			Type t = o.GetType();
			if (t == typeof(Projects.HostDebugBehavior))
			{
				Projects.HostDebugBehavior b = o as Projects.HostDebugBehavior;
				StringBuilder Code = new System.Text.StringBuilder();
				Code.AppendFormat("\t\t\tthis.{0} = new ServiceDebugBehavior();{1}", b.Name, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpHelpPageEnabled = {1};{2}", b.Name, b.HttpHelpPageEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpHelpPageUrl = new Uri({1});{2}", b.Name, b.HttpHelpPageUrl, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpsHelpPageEnabled = {1};{2}", b.Name, b.HttpsHelpPageEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpsHelpPageUrl = new Uri({1});{2}", b.Name, b.HttpsHelpPageUrl, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.IncludeExceptionDetailInFaults = new Uri({1});{2}", b.Name, b.IncludeExceptionDetailInFaults == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.IsDefaultBehavior == true) Code.AppendFormat("\t\t\tthis.Description.Behaviors.Add({0});{1}", b.Name, Environment.NewLine);
				return Code.ToString();
			}
			if (t == typeof(Projects.HostMetadataBehavior))
			{
				Projects.HostMetadataBehavior b = o as Projects.HostMetadataBehavior;
				StringBuilder Code = new System.Text.StringBuilder();
				Code.AppendFormat("\t\t\tthis.{0} = new ServiceMetadataBehavior();{1}", b.Name, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.ExternalMetadataLocation = new Uri({1});{2}", b.Name, b.ExternalMetadataLocation, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpGetEnabled = {1};{2}", b.Name, b.HttpGetEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpGetUrl = new Uri({1});{2}", b.Name, b.HttpGetUrl, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpsGetEnabled = {1};{2}", b.Name, b.HttpsGetEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpsGetUrl = new Uri({1});{2}", b.Name, b.HttpsGetUrl, Environment.NewLine);
				if (b.IsDefaultBehavior == true) Code.AppendFormat("\t\t\tthis.Description.Behaviors.Add({0});{1}", b.Name, Environment.NewLine);
				return Code.ToString();
			}
			return GenerateBehaviorCode35(o);
		}

		public static string GenerateBehaviorCode35(Projects.HostBehavior o)
		{
			return GenerateBehaviorCode40(o);
		}

		public static string GenerateBehaviorCode40(Projects.HostBehavior o)
		{
			return GenerateBehaviorCode45(o);
		}

		public static string GenerateBehaviorCode45(Projects.HostBehavior o)
		{
			Type t = o.GetType();
			StringBuilder Code = new System.Text.StringBuilder();
			if (t == typeof(Projects.HostDebugBehavior))
			{
				Projects.HostDebugBehavior b = o as Projects.HostDebugBehavior;
				Code.AppendFormat("\t\t\tthis.{0} = new ServiceDebugBehavior();{1}", b.Name, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpHelpPageBinding = new {1}();{2}", b.Name, DataTypeCSGenerator.GenerateType(b.HttpHelpPageBinding), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpHelpPageEnabled = {1};{2}", b.Name, b.HttpHelpPageEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpHelpPageUrl = new Uri({1});{2}", b.Name, b.HttpHelpPageUrl, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpsHelpPageBinding = new {1}();{2}", b.Name, DataTypeCSGenerator.GenerateType(b.HttpsHelpPageBinding), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpsHelpPageEnabled = {1};{2}", b.Name, b.HttpsHelpPageEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpsHelpPageUrl = new Uri({1});{2}", b.Name, b.HttpsHelpPageUrl, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.IncludeExceptionDetailInFaults = new Uri({1});{2}", b.Name, b.IncludeExceptionDetailInFaults == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				if (b.IsDefaultBehavior == true) Code.AppendFormat("\t\t\tthis.Description.Behaviors.Add({0});{1}", b.Name, Environment.NewLine);
			}
			if (t == typeof(Projects.HostMetadataBehavior))
			{
				Projects.HostMetadataBehavior b = o as Projects.HostMetadataBehavior;
				Code.AppendFormat("\t\t\tthis.{0} = new ServiceMetadataBehavior();{1}", b.Name, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.ExternalMetadataLocation = new Uri({1});{2}", b.Name, b.ExternalMetadataLocation, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpGetBinding = new {1}();{2}", b.Name, DataTypeCSGenerator.GenerateType(b.HttpGetBinding), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpGetEnabled = {1};{2}", b.Name, b.HttpGetEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpGetUrl = new Uri({1});{2}", b.Name, b.HttpGetUrl, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpsGetBinding = new {1}();{2}", b.Name, DataTypeCSGenerator.GenerateType(b.HttpsGetBinding), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpsGetEnabled = {1};{2}", b.Name, b.HttpsGetEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.HttpsGetUrl = new Uri({1});{2}", b.Name, b.HttpsGetUrl, Environment.NewLine);
				if (b.IsDefaultBehavior == true) Code.AppendFormat("\t\t\tthis.Description.Behaviors.Add({0});{1}", b.Name, Environment.NewLine);
			}
			if (t == typeof(Projects.HostThrottlingBehavior))
			{
				Projects.HostThrottlingBehavior b = o as Projects.HostThrottlingBehavior;
				Code.AppendFormat("\t\t\tthis.{0} = new ServiceThrottlingBehavior();{1}", b.Name, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.MaxConcurrentCalls = {1};{2}", b.Name, b.MaxConcurrentCalls, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.MaxConcurrentInstances = {1};{2}", b.Name, b.MaxConcurrentInstances, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.{0}.MaxConcurrentSessions = {1};{2}", b.Name, b.MaxConcurrentSessions, Environment.NewLine);
				if (b.IsDefaultBehavior == true) Code.AppendFormat("\t\t\tthis.Description.Behaviors.Add({0});{1}", b.Name, Environment.NewLine);
			}
			return Code.ToString();
		}
	}
}
