using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WCFArchitect.Projects
{
	internal enum HostEndpointIdentityType
	{
		Anonymous,
		DNS,
		RSA,
		RSAX509,
		SPN,
		UPN,
		X509
	}

	internal class Host : DependencyObject
	{
		protected Guid id;
		public Guid ID { get { return id; } }
		
		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Host));
		
		public string CodeName { get { return (string)GetValue(CodeNameProperty); } set { SetValue(CodeNameProperty, value); } }
		public static readonly DependencyProperty CodeNameProperty = DependencyProperty.Register("CodeName", typeof(string), typeof(Host));
		
		public string Namespace { get { return (string)GetValue(NamespaceProperty); } set { SetValue(NamespaceProperty, value); } }
		public static readonly DependencyProperty NamespaceProperty = DependencyProperty.Register("Namespace", typeof(string), typeof(Host));
		
		public string ConfigurationName { get { return (string)GetValue(ConfigurationNameProperty); } set { SetValue(ConfigurationNameProperty, value); } }
		public static readonly DependencyProperty ConfigurationNameProperty = DependencyProperty.Register("ConfigurationName", typeof(string), typeof(Host));
		
		public HostCredentials Credentials { get { return (HostCredentials)GetValue(CredentialsProperty); } set { SetValue(CredentialsProperty, value); } }
		public static readonly DependencyProperty CredentialsProperty = DependencyProperty.Register("Credentials", typeof(HostCredentials), typeof(Host));

		public ObservableCollection<string> BaseAddresses { get { return (ObservableCollection<string>)GetValue(BaseAddressProperty); } set { SetValue(BaseAddressProperty, value); } }
		public static readonly DependencyProperty BaseAddressProperty = DependencyProperty.Register("BaseAddress", typeof(ObservableCollection<string>), typeof(Host));
		
		public TimeSpan CloseTimeout { get { return (TimeSpan)GetValue(CloseTimeoutProperty); } set { SetValue(CloseTimeoutProperty, value); } }
		public static readonly DependencyProperty CloseTimeoutProperty = DependencyProperty.Register("CloseTimeout", typeof(TimeSpan), typeof(Host));
		
		public TimeSpan OpenTimeout { get { return (TimeSpan)GetValue(OpenTimeoutProperty); } set { SetValue(OpenTimeoutProperty, value); } }
		public static readonly DependencyProperty OpenTimeoutProperty = DependencyProperty.Register("OpenTimeout", typeof(TimeSpan), typeof(Host));

		public int ManualFlowControlLimit { get { return (int)GetValue(ManualFlowControlLimitProperty); } set { SetValue(ManualFlowControlLimitProperty, value); } }
		public static readonly DependencyProperty ManualFlowControlLimitProperty = DependencyProperty.Register("ManualFlowControlLimit", typeof(int), typeof(Project));

		public bool AuthorizationImpersonateCallerForAllOperations { get { return (bool)GetValue(AuthorizationImpersonateCallerForAllOperationsProperty); } set { SetValue(AuthorizationImpersonateCallerForAllOperationsProperty, value); } }
		public static readonly DependencyProperty AuthorizationImpersonateCallerForAllOperationsProperty = DependencyProperty.Register("AuthorizationImpersonateCallerForAllOperations", typeof(bool), typeof(Host));
		
		public System.ServiceModel.Description.PrincipalPermissionMode AuthorizationPrincipalPermissionMode { get { return (System.ServiceModel.Description.PrincipalPermissionMode)GetValue(AuthorizationPrincipalPermissionModeProperty); } set { SetValue(AuthorizationPrincipalPermissionModeProperty, value); } }
		public static readonly DependencyProperty AuthorizationPrincipalPermissionModeProperty = DependencyProperty.Register("AuthorizationPrincipalPermissionMode", typeof(System.ServiceModel.Description.PrincipalPermissionMode), typeof(Host));

		public WCFArchitect.Projects.Service Service { get { return (WCFArchitect.Projects.Service)GetValue(ServiceProperty); } set { SetValue(ServiceProperty, value); } }
		public static readonly DependencyProperty ServiceProperty = DependencyProperty.Register("Service", typeof(WCFArchitect.Projects.Service), typeof(Host));

		public ObservableCollection<WCFArchitect.Projects.HostBehavior> Behaviors { get { return (ObservableCollection<WCFArchitect.Projects.HostBehavior>)GetValue(BehaviorsProperty); } set { SetValue(BehaviorsProperty, value); } }
		public static readonly DependencyProperty BehaviorsProperty = DependencyProperty.Register("Behaviors", typeof(ObservableCollection<WCFArchitect.Projects.HostBehavior>), typeof(Host));

		public ObservableCollection<HostEndpoint> Endpoints { get { return (ObservableCollection<HostEndpoint>)GetValue(EndpointsProperty); } set { SetValue(EndpointsProperty, value); } }
		public static readonly DependencyProperty EndpointsProperty = DependencyProperty.Register("Endpoints", typeof(ObservableCollection<HostEndpoint>), typeof(Host));

		public Project Parent { get { return (Project)GetValue(ParentProperty); } set { SetValue(ParentProperty, value); } }
		public static readonly DependencyProperty ParentProperty = DependencyProperty.Register("Parent", typeof(Project), typeof(Host));

		//Internal Use - Searching / Filtering
		public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(Host));

		public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(Host));

		public bool IsFiltering { get { return false; } set { } }
		public bool IsFilterMatch { get { return false; } set { } }

		public bool IsTreeExpanded { get { return (bool)GetValue(IsTreeExpandedProperty); } set { SetValue(IsTreeExpandedProperty, value); } }
		public static readonly DependencyProperty IsTreeExpandedProperty = DependencyProperty.Register("IsTreeExpanded", typeof(bool), typeof(Host), new UIPropertyMetadata(false));

		public Host() { Endpoints = new ObservableCollection<HostEndpoint>(); }

		public Host(string Name, Project Parent)
		{
			this.BaseAddresses = new ObservableCollection<string>();
			this.Behaviors = new ObservableCollection<HostBehavior>();
			this.Endpoints = new ObservableCollection<HostEndpoint>();
			id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.Parent = Parent;
			Credentials = new HostCredentials(this);
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (Parent != null)
				Parent.IsDirty = true;
		}

		public bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (CodeName == "" || CodeName == null)
			{
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5000", "A host in the '" + Parent.Name + "' project has a blank Code Name. A Code Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchCodeName.IsMatch(CodeName) == false)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5001", "The host '" + Name + "' in the '" + Parent.Name + "' project contains invalid characters in the Code Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
					NoErrors = false;
				}
			if (Namespace == "" || Namespace == null) { }
			else
				if (Helpers.RegExs.MatchHTTPURI.IsMatch(Namespace) == false)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5002", "The Namespace URI '" + Namespace + "' for the '" + Name + "' host in the '" + Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));

			if (Service == null)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5003", "The host '" + Name + "' in the '" + Parent.Name + "' project has no Service associated with it and will not be generated. A Service must be associated with this Host for it be generated.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));

			foreach (string BA in BaseAddresses)
			{
				bool BAValid = false;
				if (Helpers.RegExs.MatchHTTPURI.IsMatch(BA) == true) BAValid = true;
				if (Helpers.RegExs.MatchTCPURI.IsMatch(BA) == true) BAValid = true;
				if (Helpers.RegExs.MatchP2PURI.IsMatch(BA) == true) BAValid = true;
				if (Helpers.RegExs.MatchPipeURI.IsMatch(BA) == true) BAValid = true;
				if (Helpers.RegExs.MatchMSMQURI.IsMatch(BA) == true) BAValid = true;
				if (BAValid == true)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5003", "The URI '" + BA + "' for the '" + Name + "' host in the '" + Parent.Name + "' project is not a valid URI. Any associated services and data may not function properly.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
				}
			}

			foreach (HostEndpoint HE in Endpoints)
			{
				if (HE.VerifyCode(Compiler) == false) NoErrors = false;
			}

			if (Credentials.VerifyCode(Compiler) == false) NoErrors = false;

			foreach (HostBehavior HB in Behaviors)
			{
				if (HB.VerifyCode(Compiler) == false) NoErrors = false;
			}

			return NoErrors;
		}

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Project || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent, this));
							if (CodeName != null && CodeName != "") if (CodeName.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Code Name", CodeName, Parent, this));
							if (Namespace != null && Namespace != "") if (Namespace.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Namespace", Namespace, Parent, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent, this));
							if (CodeName != null && CodeName != "") if (CodeName.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Code Name", CodeName, Parent, this));
							if (Namespace != null && Namespace != "") if (Namespace.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Namespace", Namespace, Parent, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Parent, this));
						if (CodeName != null && CodeName != "") if (Args.RegexSearch.IsMatch(CodeName)) results.Add(new FindReplaceResult("Code Name", CodeName, Parent, this));
						if (Namespace != null && Namespace != "") if (Args.RegexSearch.IsMatch(Namespace)) results.Add(new FindReplaceResult("Namespace", Namespace, Parent, this));
					}
				}

				if (Args.ReplaceAll == true)
				{
					bool ia = Parent.IsActive;
					Parent.IsActive = true;
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (CodeName != null && CodeName != "") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (Namespace != null && Namespace != "") Namespace = Microsoft.VisualBasic.Strings.Replace(Namespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (CodeName != null && CodeName != "") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (Namespace != null && Namespace != "") Namespace = Microsoft.VisualBasic.Strings.Replace(Namespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (CodeName != null && CodeName != "") CodeName = Args.RegexSearch.Replace(CodeName, Args.Replace);
							if (Namespace != null && Namespace != "") Namespace = Args.RegexSearch.Replace(Namespace, Args.Replace);
						}
					}
					Parent.IsActive = ia;
				}
			}

			foreach (HostEndpoint HE in Endpoints)
				results.AddRange(HE.FindReplace(Args));

			foreach (HostBehavior HB in Behaviors)
				results.AddRange(HB.FindReplace(Args));

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
		{
			if (Args.ReplaceAll == true)
			{
				bool ia = Parent.IsActive;
				Parent.IsActive = true;
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Code Name") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Namespace") Namespace = Microsoft.VisualBasic.Strings.Replace(Namespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Code Name") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Namespace") Namespace = Microsoft.VisualBasic.Strings.Replace(Namespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (Field == "Code Name") CodeName = Args.RegexSearch.Replace(CodeName, Args.Replace);
						if (Field == "Namespace") Namespace = Args.RegexSearch.Replace(Namespace, Args.Replace);
					}
				}
				Parent.IsActive = ia;
			}
		}

		public string GenerateServerCode30(string ProjectName)
		{
			if (Service == null) return "";

			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("\t{0} class {1} : ServiceHost{1}", Parent.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			string BAVars = "";
			for (int i = 0; i < BaseAddresses.Count; i++)
			{
				Code.AppendFormat("\t\tprivate static Uri BaseAddr{0} = new Uri(\"{1}\");{2}", i, BaseAddresses[i], Environment.NewLine);
				BAVars += ", " + CodeName + ".BaseAddr" + i.ToString();
			}

			foreach (HostEndpoint HE in Endpoints)
				Code.AppendFormat("\t\tpublic static Uri {0}URI {{ get {{ return new Uri(\"{1}\"); }} }}{2}", HE.CodeName, HE.GenerateServerEndpointURI(), Environment.NewLine);

			foreach (HostBehavior HB in Behaviors)
			{
				if (HB.GetType() == typeof(HostDebugBehavior))
				{
					Code.AppendFormat("\t\tprivate ServiceDebugBehavior m_{0} = null;{1}", HB.CodeName, Environment.NewLine);
					Code.AppendFormat("\t\tpublic ServiceDebugBehavior {0} {{ get {{ return m_{0} }} private set {{ m_{0} = value; }} }}{1}", HB.CodeName, Environment.NewLine);
				}
				if (HB.GetType() == typeof(HostMetadataBehavior))
				{
					Code.AppendFormat("\t\tprivate ServiceMetadataBehavior m_{0} = null;{1}", HB.CodeName, Environment.NewLine);
					Code.AppendFormat("\t\tpublic ServiceMetadataBehavior {0} {{ get {{ return m_{0} }} private set {{ m_{0} = value; }} }}{1}", HB.CodeName, Environment.NewLine);
				}
				if (HB.GetType() == typeof(HostThrottlingBehavior))
				{
					Code.AppendFormat("\t\tprivate ServiceThrottlingBehavior m_{0} = null;{1}", HB.CodeName, Environment.NewLine);
					Code.AppendFormat("\t\tpublic ServiceThrottlingBehavior {0} {{ get {{ return m_{0} }} private set {{ m_{0} = value; }} }}{1}", HB.CodeName, Environment.NewLine);
				}
			}

			#region - Generate Default Constructors -
			//Generate Singleton Service Constructor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance) : base(singletonInstance{1}){2}", CodeName, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode30());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode30());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine(HE.GenerateServerCode());
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType) : base(serviceType{1}){2}", CodeName, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode30());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode30());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine(HE.GenerateServerCode());
			Code.AppendLine("\t\t}");

			//Generate Singleton Service Constructor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode30());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode30());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine(HE.GenerateServerCode());
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode30());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode30());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine(HE.GenerateServerCode());
			Code.AppendLine("\t\t}");
			#endregion

			#region - Generate Optional Endpoint Constructors -
			//Generate Singleton Service Constructor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, bool DisableDefaultEndpoints) : base(singletonInstance{1}){2}", CodeName, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode30());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode30());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine("\t" + HE.GenerateServerCode());
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, bool DisableDefaultEndpoints) : base(serviceType{1}){2}", CodeName, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode30());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode30());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine("\t" + HE.GenerateServerCode());
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");

			//Generate Singleton Service Constructor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode30());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode30());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine("\t" + HE.GenerateServerCode());
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode30());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode30());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine("\t" + HE.GenerateServerCode());
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");
			#endregion

			Code.AppendLine("\t}");

			return Code.ToString();
		}

		public string GenerateServerCode35(string ProjectName)
		{
			if (Service == null) return "";

			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("\t{0} class {1} : ServiceHost{2}", Parent.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			string BAVars = "";
			for (int i = 0; i < BaseAddresses.Count; i++)
			{
				Code.AppendFormat("\t\tprivate static Uri BaseAddr{0} = new Uri(\"{1}\");{2}", i, BaseAddresses[i], Environment.NewLine);
				BAVars += ", " + CodeName + ".BaseAddr" + i.ToString();
			}

			foreach (HostEndpoint HE in Endpoints)
				Code.AppendFormat("\t\tpublic static Uri {0}URI {{ get {{ return new Uri(\"{1}\"); }} }}{2}", HE.CodeName, HE.GenerateServerEndpointURI(), Environment.NewLine);

			foreach (HostBehavior HB in Behaviors)
			{
				if (HB.GetType() == typeof(HostDebugBehavior))
				{
					Code.AppendFormat("\t\tprivate ServiceDebugBehavior m_{0} = null;{1}", HB.CodeName, Environment.NewLine);
					Code.AppendFormat("\t\tpublic ServiceDebugBehavior {0} {{ get {{ return m_{0} }} private set {{ m_{0} = value; }} }}{1}", HB.CodeName, Environment.NewLine);
				}
				if (HB.GetType() == typeof(HostMetadataBehavior))
				{
					Code.AppendFormat("\t\tprivate ServiceMetadataBehavior m_{0} = null;{1}", HB.CodeName, Environment.NewLine);
					Code.AppendFormat("\t\tpublic ServiceMetadataBehavior {0} {{ get {{ return m_{0} }} private set {{ m_{0} = value; }} }}{1}", HB.CodeName, Environment.NewLine);
				}
				if (HB.GetType() == typeof(HostThrottlingBehavior))
				{
					Code.AppendFormat("\t\tprivate ServiceThrottlingBehavior m_{0} = null;{1}", HB.CodeName, Environment.NewLine);
					Code.AppendFormat("\t\tpublic ServiceThrottlingBehavior {0} {{ get {{ return m_{0} }} private set {{ m_{0} = value; }} }}{1}", HB.CodeName, Environment.NewLine);
				}
			}

			#region - Generate Default Constructors -
			//Generate Singleton Service Constructor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance) : base(singletonInstance{1}){2}", CodeName, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode35());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode35());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine(HE.GenerateServerCode());
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType) : base(serviceType{1}){2}", CodeName, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode35());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode35());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine(HE.GenerateServerCode());
			Code.AppendLine("\t\t}");

			//Generate Singleton Service Constructor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode35());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode35());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine(HE.GenerateServerCode());
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode35());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode35());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine(HE.GenerateServerCode());
			Code.AppendLine("\t\t}");
			#endregion

			#region - Generate Optional Endpoint Constructors -
			//Generate Singleton Service Constructor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, bool DisableDefaultEndpoints) : base(singletonInstance{1}){2}", CodeName, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode35());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode35());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine("\t" + HE.GenerateServerCode());
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, bool DisableDefaultEndpoints) : base(serviceType{1}){2}", CodeName, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode35());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode35());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine("\t" + HE.GenerateServerCode());
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");

			//Generate Singleton Service Constructor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode35());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode35());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine("\t" + HE.GenerateServerCode());
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode35());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode35());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine("\t" + HE.GenerateServerCode());
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");
			#endregion

			Code.AppendLine("\t}");

			return Code.ToString();
		}

		public string GenerateServerCode40(string ProjectName)
		{
			if (Service == null) return "";

			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("\t{0} class {1} : ServiceHost{2}", Parent.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			string BAVars = "";
			for (int i = 0; i < BaseAddresses.Count;i++ )
			{
				Code.AppendFormat("\t\tprivate static Uri BaseAddr{0} = new Uri(\"{1}\");{2}", i, BaseAddresses[i], Environment.NewLine);
				BAVars += ", " + CodeName + ".BaseAddr" + i.ToString();
			}

			foreach (HostEndpoint HE in Endpoints)
				Code.AppendFormat("\t\tpublic static Uri {0}URI {{ get {{ return new Uri(\"{1}\"); }} }}{2}", HE.CodeName, HE.GenerateServerEndpointURI(), Environment.NewLine);

			foreach (HostBehavior HB in Behaviors)
			{
				if (HB.GetType() == typeof(HostDebugBehavior)) Code.AppendFormat("\t\tpublic ServiceDebugBehavior {0} {{ get; private set; }}{1}", HB.CodeName, Environment.NewLine);
				if (HB.GetType() == typeof(HostMetadataBehavior)) Code.AppendFormat("\t\tpublic ServiceMetadataBehavior {0} {{ get; private set; }}{1}", HB.CodeName, Environment.NewLine);
				if (HB.GetType() == typeof(HostThrottlingBehavior)) Code.AppendFormat("\t\tpublic ServiceThrottlingBehavior {0} {{ get; private set; }}{1}", HB.CodeName, Environment.NewLine);
			}

			#region - Generate Default Constructors
			//Generate Singleton Service Constructor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance) : base(singletonInstance{1}){2}", CodeName, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode40());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode40());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine(HE.GenerateServerCode());
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType) : base(serviceType{1}){2}", CodeName, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode40());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode40());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine(HE.GenerateServerCode());
			Code.AppendLine("\t\t}");

			//Generate Singleton Service Constructor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode40());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode40());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine(HE.GenerateServerCode());
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode40());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode40());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine(HE.GenerateServerCode());
			Code.AppendLine("\t\t}");
			#endregion

			#region - Generate Optional Endpoint Constructors
			//Generate Singleton Service Constructor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, bool DisableDefaultEndpoints) : base(singletonInstance{1}){2}", CodeName, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode40());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode40());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine("\t" + HE.GenerateServerCode());
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITH default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, bool DisableDefaultEndpoints) : base(serviceType{1}){2}", CodeName, BAVars, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode40());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode40());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine("\t" + HE.GenerateServerCode());
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");

			//Generate Singleton Service Constructor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(object singletonInstance, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode40());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode40());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine("\t" + HE.GenerateServerCode());
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");

			//Generate ServiceType Contrstuctor WITHOUT default base addresses
			Code.AppendFormat("\t\tpublic {0}(Type serviceType, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.Append(Credentials.GenerateCode40());
			Code.AppendFormat("\t\t\tthis.Authorization.ImpersonateCallerForAllOperations = {0};{1}", AuthorizationImpersonateCallerForAllOperations ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Description.PrincipalPermissionMode), AuthorizationPrincipalPermissionMode), Environment.NewLine);
			if (CloseTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.CloseTimeout = this.DefaultCloseTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			if (OpenTimeout.Ticks == 0)
				Code.AppendLine("\t\t\tthis.OpenTimeout = this.DefaultOpenTimeout;");
			else
				Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			if (ConfigurationName != null && ConfigurationName != "") Code.AppendFormat("\t\t\tthis.Description.ConfigurationName = \"{0}\";{1}", ConfigurationName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Name = \"{0}\";{1}", Helpers.RegExs.ReplaceSpaces.Replace(Name, ""), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Description.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			foreach (HostBehavior HB in Behaviors)
				Code.Append(HB.GenerateCode40());
			if (ManualFlowControlLimit > 0) Code.AppendFormat("\t\t\tthis.ManualFlowControlLimit = {0};{1}", ManualFlowControlLimit, Environment.NewLine);
			Code.AppendLine("\t\t\tif(DisableDefaultEndpoints == false)");
			Code.AppendLine("\t\t\t{");
			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine("\t" + HE.GenerateServerCode());
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");
			#endregion

			Code.AppendLine("\t}");

			return Code.ToString();
		}

		public string GenerateServerCode35Client(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public string GenerateServerCode40Client(string ProjectName)
		{
			return GenerateServerCode40(ProjectName);
		}

		public string GenerateClientCode30()
		{
			return GenerateClientCode35();
		}

		public string GenerateClientCode35()
		{
			return GenerateClientCode40();
		}

		public string GenerateClientCode40()
		{
			if (Service == null) return "";

			StringBuilder Code = new StringBuilder();

			foreach (HostEndpoint HE in Endpoints)
				Code.AppendFormat("\t\tpublic static Uri {0}URI {{ get {{ return new Uri(\"{1}\"); }} }}{2}", HE.CodeName, HE.GenerateClientEndpointURI(false, false), Environment.NewLine);

			foreach (HostEndpoint HE in Endpoints)
				Code.AppendLine(HE.GenerateClientCode());

			return Code.ToString();
		}

		public string GenerateClientCode35Client()
		{
			return GenerateClientCode40Client();
		}

		public string GenerateClientCode40Client()
		{
			return GenerateClientCode40();
		}
	}

	internal class HostEndpoint : DependencyObject
	{
		protected Guid id;
		public Guid ID { get { return id; } }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(HostEndpoint));

		public string CodeName { get { return (string)GetValue(CodeNameProperty); } set { SetValue(CodeNameProperty, value); } }
		public static readonly DependencyProperty CodeNameProperty = DependencyProperty.Register("CodeName", typeof(string), typeof(HostEndpoint));

		public ServiceBinding Binding { get { return (ServiceBinding)GetValue(BindingProperty); } set { SetValue(BindingProperty, value); } }
		public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding", typeof(ServiceBinding), typeof(HostEndpoint));

		public string ServerAddress { get { return (string)GetValue(ServerAddressProperty); } set { SetValue(ServerAddressProperty, value); } }
		public static readonly DependencyProperty ServerAddressProperty = DependencyProperty.Register("ServerAddress", typeof(string), typeof(HostEndpoint));

		public int ServerPort { get { return (int)GetValue(ServerPortProperty); } set { SetValue(ServerPortProperty, value); } }
		public static readonly DependencyProperty ServerPortProperty = DependencyProperty.Register("ServerPort", typeof(int), typeof(HostEndpoint));

		public bool ServerUseHTTPS { get { return (bool)GetValue(ServerUseHTTPSProperty); } set { SetValue(ServerUseHTTPSProperty, value); } }
		public static readonly DependencyProperty ServerUseHTTPSProperty = DependencyProperty.Register("ServerUseHTTPS", typeof(bool), typeof(HostEndpoint));

		public string ClientAddress { get { return (string)GetValue(ClientAddressProperty); } set { SetValue(ClientAddressProperty, value); } }
		public static readonly DependencyProperty ClientAddressProperty = DependencyProperty.Register("ClientAddress", typeof(string), typeof(HostEndpoint));

		public HostEndpointIdentityType ClientIdentityType { get { return (HostEndpointIdentityType)GetValue(ClientIdentityTypeProperty); } set { SetValue(ClientIdentityTypeProperty, value); } }
		public static readonly DependencyProperty ClientIdentityTypeProperty = DependencyProperty.Register("ClientIdentityType", typeof(HostEndpointIdentityType), typeof(HostEndpoint));

		public string ClientIdentityData { get { return (string)GetValue(ClientIdentityDataProperty); } set { SetValue(ClientIdentityDataProperty, value); } }
		public static readonly DependencyProperty ClientIdentityDataProperty = DependencyProperty.Register("ClientIdentityData", typeof(string), typeof(HostEndpoint));

		public ObservableCollection<HostEndpointAddressHeader> ClientAddressHeaders { get { return (ObservableCollection<HostEndpointAddressHeader>)GetValue(ClientAddressHeadersProperty); } set { SetValue(ClientAddressHeadersProperty, value); } }
		public static readonly DependencyProperty ClientAddressHeadersProperty = DependencyProperty.Register("ClientAddressHeaders", typeof(ObservableCollection<HostEndpointAddressHeader>), typeof(HostEndpoint));

		public Host Parent { get; set; }

		public HostEndpoint() { }

		public HostEndpoint(Host Parent, string Name)
		{
			ClientAddressHeaders = new ObservableCollection<HostEndpointAddressHeader>();
			id = Guid.NewGuid();
			this.Parent = Parent;
			this.Name = Name;
			this.CodeName = Helpers.RegExs.ReplaceSpaces.Replace(Name, "");
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (Parent != null)
				if (Parent.Parent != null)
					Parent.Parent.IsDirty = true;
		}

		public bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (CodeName == "" || CodeName == null)
			{
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5004", "A host in the endpoint '" + Parent.Name + "' project has a blank Code Name. A Code Name MUST be spcified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchCodeName.IsMatch(CodeName) == false)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5005", "The host endpoint '" + Name + "' in the '" + Parent.Name + "' project contains invalid characters in the Code Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
					NoErrors = false;
				}
			if (Binding == null)
			{
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5006", "The host endpoint'" + Name + "' in the '" + Parent.Name + "' must have a Binding. Please specify a Binding", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
				NoErrors = false;
			}

			return NoErrors;
		}

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Project || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Parent, this));
							if (CodeName != null && CodeName != "") if (CodeName.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Code Name", CodeName, Parent.Parent, this));
							if (ServerAddress != null && ServerAddress != "") if (ServerAddress.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Server Address", ServerAddress, Parent.Parent, this));
							if (ClientAddress != null && ClientAddress != "") if (ClientAddress.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Client Address", ClientAddress, Parent.Parent, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Parent, this));
							if (CodeName != null && CodeName != "") if (CodeName.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Code Name", CodeName, Parent.Parent, this));
							if (ServerAddress != null && ServerAddress != "") if (ServerAddress.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Server Address", ServerAddress, Parent.Parent, this));
							if (ClientAddress != null && ClientAddress != "") if (ClientAddress.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Client Address", ClientAddress, Parent.Parent, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Parent.Parent, this));
						if (CodeName != null && CodeName != "") if (Args.RegexSearch.IsMatch(CodeName)) results.Add(new FindReplaceResult("Code Name", CodeName, Parent.Parent, this));
						if (ServerAddress != null && ServerAddress != "") if (Args.RegexSearch.IsMatch(ServerAddress)) results.Add(new FindReplaceResult("Server Address", ServerAddress, Parent.Parent, this));
						if (ClientAddress != null && ClientAddress != "") if (Args.RegexSearch.IsMatch(ClientAddress)) results.Add(new FindReplaceResult("Client Address", ClientAddress, Parent.Parent, this));
					}
				}

				if (Args.ReplaceAll == true)
				{
					bool ia = Parent.Parent.IsActive;
					Parent.Parent.IsActive = true;
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (CodeName != null && CodeName != "") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (ServerAddress != null && ServerAddress != "") ServerAddress = Microsoft.VisualBasic.Strings.Replace(ServerAddress, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (ClientAddress != null && ClientAddress != "") ClientAddress = Microsoft.VisualBasic.Strings.Replace(ClientAddress, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (CodeName != null && CodeName != "") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (ServerAddress != null && ServerAddress != "") ServerAddress = Microsoft.VisualBasic.Strings.Replace(ServerAddress, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (ClientAddress != null && ClientAddress != "") ClientAddress = Microsoft.VisualBasic.Strings.Replace(ClientAddress, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (CodeName != null && CodeName != "") CodeName = Args.RegexSearch.Replace(CodeName, Args.Replace);
							if (ServerAddress != null && ServerAddress != "") ServerAddress = Args.RegexSearch.Replace(ServerAddress, Args.Replace);
							if (ClientAddress != null && ClientAddress != "") ClientAddress = Args.RegexSearch.Replace(ClientAddress, Args.Replace);
						}
					}
					Parent.Parent.IsActive = ia;
				}
			}

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
		{
			if (Args.ReplaceAll == true)
			{
				bool ia = Parent.Parent.IsActive;
				Parent.Parent.IsActive = true;
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Code Name") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Server Address") ServerAddress = Microsoft.VisualBasic.Strings.Replace(ServerAddress, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Client Address") ClientAddress = Microsoft.VisualBasic.Strings.Replace(ClientAddress, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Code Name") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Server Address") ServerAddress = Microsoft.VisualBasic.Strings.Replace(ServerAddress, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Client Address") ClientAddress = Microsoft.VisualBasic.Strings.Replace(ClientAddress, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (Field == "Code Name") CodeName = Args.RegexSearch.Replace(CodeName, Args.Replace);
						if (Field == "Server Address") ServerAddress = Args.RegexSearch.Replace(ServerAddress, Args.Replace);
						if (Field == "Client Address") ClientAddress = Args.RegexSearch.Replace(ClientAddress, Args.Replace);
					}
				}
				Parent.Parent.IsActive = ia;
			}
		}
		

		public string GenerateServerCode()
		{
			return string.Format("\t\t\tthis.AddServiceEndpoint(typeof({0}.I{1}), new {2}.{3}(), {4}URI);", Parent.Service.Parent.FullName, Parent.Service.CodeName, Binding.Parent.FullName, Binding.CodeName, CodeName);
		}

		public string GenerateClientCode()
		{
			StringBuilder Code = new StringBuilder();

			string ahlist = "";
			string identity = "";
			string certificateidentity = "";
			if (ClientIdentityType != HostEndpointIdentityType.Anonymous)
			{
				if (ClientIdentityType == HostEndpointIdentityType.DNS) identity = ", System.ServiceModel.EndpointIdentity.CreateDnsIdentity(\"" + ClientIdentityData + "\")";
				if (ClientIdentityType == HostEndpointIdentityType.RSA) identity = ", System.ServiceModel.EndpointIdentity.CreateRsaIdentity(\"" + ClientIdentityData + "\")";
				if (ClientIdentityType == HostEndpointIdentityType.RSAX509)
				{
					identity = ", System.ServiceModel.EndpointIdentity.CreateRsaIdentity(IdentityCertificate)";
					certificateidentity = ", System.Security.Cryptography.X509Certificates.X509Certificate2 IdentityCertificate";
				}
				if (ClientIdentityType == HostEndpointIdentityType.SPN) identity = ", System.ServiceModel.EndpointIdentity.CreateSpnIdentity(\"" + ClientIdentityData + "\")";
				if (ClientIdentityType == HostEndpointIdentityType.UPN) identity = ", System.ServiceModel.EndpointIdentity.CreateUpnIdentity(\"" + ClientIdentityData + "\")";
				if (ClientIdentityType == HostEndpointIdentityType.X509)
				{
					identity = ", System.ServiceModel.EndpointIdentity.CreateX509CertificateIdentity(IdentityCertificate)";
					certificateidentity = ", System.Security.Cryptography.X509Certificates.X509Certificate2 IdentityCertificate";
				}
			}

			#region - Generate Endpoint Functions WITHOUT EndpointIdentity Parameter -
			Code.AppendFormat("\t\tpublic static System.ServiceModel.EndpointAddress Create{0}Endpoint({1}){2}", CodeName, certificateidentity.Replace(", ", ""), Environment.NewLine);
			Code.AppendLine("\t\t{");
			ahlist = "";
			for (int i = 0; i < ClientAddressHeaders.Count; i++)
			{
				Code.AppendFormat("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});{3}", i + 1, ClientAddressHeaders[i].Name, ClientAddressHeaders[i].Namespace, Environment.NewLine);
				ahlist += (", ah" + i.ToString());
			}
			Code.AppendFormat("\t\t\treturn new System.ServiceModel.EndpointAddress(new Uri(\"{0}\"){1}{2});{3}", GenerateClientEndpointURI(false, false), identity, ahlist, Environment.NewLine);
			Code.AppendLine("\t\t}");

			Code.AppendFormat("\t\tpublic static System.ServiceModel.EndpointAddress Create{0}Endpoint(string Address{1}){2}", CodeName, certificateidentity, Environment.NewLine);
			Code.AppendLine("\t\t{");
			ahlist = "";
			for (int i = 0; i < ClientAddressHeaders.Count; i++)
			{
				Code.AppendFormat("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});{3}", i + 1, ClientAddressHeaders[i].Name, ClientAddressHeaders[i].Namespace, Environment.NewLine);
				ahlist += (", ah" + i.ToString());
			}
			Code.AppendFormat("\t\t\treturn new System.ServiceModel.EndpointAddress(new Uri(\"{0}\"){1}{2});{3}", GenerateClientEndpointURI(true, false), identity, ahlist, Environment.NewLine);
			Code.AppendLine("\t\t}");

			Code.AppendFormat("\t\tpublic static System.ServiceModel.EndpointAddress Create{0}Endpoint(int Port{1}){2}", CodeName, certificateidentity, Environment.NewLine);
			Code.AppendLine("\t\t{");
			ahlist = "";
			for (int i = 0; i < ClientAddressHeaders.Count; i++)
			{
				Code.AppendFormat("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});{3}", i + 1, ClientAddressHeaders[i].Name, ClientAddressHeaders[i].Namespace, Environment.NewLine);
				ahlist += (", ah" + i.ToString());
			}
			Code.AppendFormat("\t\t\treturn new System.ServiceModel.EndpointAddress(new Uri(\"{0}\"){1}{2});{3}", GenerateClientEndpointURI(false, true), identity, ahlist, Environment.NewLine);
			Code.AppendLine("\t\t}");

			Code.AppendFormat("\t\tpublic static System.ServiceModel.EndpointAddress Create{0}Endpoint(string Address, int Port{1}){2}", CodeName, certificateidentity, Environment.NewLine);
			Code.AppendLine("\t\t{");
			ahlist = "";
			for (int i = 0; i < ClientAddressHeaders.Count; i++)
			{
				Code.AppendFormat("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});{3}", i + 1, ClientAddressHeaders[i].Name, ClientAddressHeaders[i].Namespace, Environment.NewLine);
				ahlist += (", ah" + i.ToString());
			}
			Code.AppendFormat("\t\t\treturn new System.ServiceModel.EndpointAddress(new Uri(\"{0}\"){1}{2});{3}", GenerateClientEndpointURI(true, true), identity, ahlist, Environment.NewLine);
			Code.AppendLine("\t\t}");
			#endregion

			#region - Generate Endpoint Functions WITH EndpointIdentity Parameter -
			Code.AppendFormat("\t\tpublic static System.ServiceModel.EndpointAddress Create{0}Endpoint(System.ServiceModel.EndpointIdentity Identity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			ahlist = "";
			for (int i = 0; i < ClientAddressHeaders.Count; i++)
			{
				Code.AppendFormat("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});{3}", i + 1, ClientAddressHeaders[i].Name, ClientAddressHeaders[i].Namespace, Environment.NewLine);
				ahlist += (", ah" + i.ToString());
			}
			Code.AppendFormat("\t\t\treturn new System.ServiceModel.EndpointAddress(new Uri(\"{0}\"), Identity{1});{2}", GenerateClientEndpointURI(false, false), ahlist, Environment.NewLine);
			Code.AppendLine("\t\t}");

			Code.AppendFormat("\t\tpublic static System.ServiceModel.EndpointAddress Create{0}Endpoint(string Address, System.ServiceModel.EndpointIdentity Identity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			ahlist = "";
			for (int i = 0; i < ClientAddressHeaders.Count; i++)
			{
				Code.AppendFormat("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});{3}", i + 1, ClientAddressHeaders[i].Name, ClientAddressHeaders[i].Namespace, Environment.NewLine);
				ahlist += (", ah" + i.ToString());
			}
			Code.AppendFormat("\t\t\treturn new System.ServiceModel.EndpointAddress(new Uri(\"{0}\"), Identity{1});{2}", GenerateClientEndpointURI(true, false), ahlist, Environment.NewLine);
			Code.AppendLine("\t\t}");

			Code.AppendFormat("\t\tpublic static System.ServiceModel.EndpointAddress Create{0}Endpoint(int Port, System.ServiceModel.EndpointIdentity Identity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			ahlist = "";
			for (int i = 0; i < ClientAddressHeaders.Count; i++)
			{
				Code.AppendFormat("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});{3}", i + 1, ClientAddressHeaders[i].Name, ClientAddressHeaders[i].Namespace, Environment.NewLine);
				ahlist += (", ah" + i.ToString());
			}
			Code.AppendFormat("\t\t\treturn new System.ServiceModel.EndpointAddress(new Uri(\"{0}\"), Identity{1});{2}", GenerateClientEndpointURI(false, true), ahlist, Environment.NewLine);
			Code.AppendLine("\t\t}");

			Code.AppendFormat("\t\tpublic static System.ServiceModel.EndpointAddress Create{0}Endpoint(string Address, int Port, System.ServiceModel.EndpointIdentity Identity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			ahlist = "";
			for (int i = 0; i < ClientAddressHeaders.Count; i++)
			{
				Code.AppendFormat("\t\t\tSystem.ServiceModel.Channels.AddressHeader ah{0} = System.ServiceModel.Channels.AddressHeader.CreateAddressHeader(\"{1}\", \"{2}\", {0});{3}", i + 1, ClientAddressHeaders[i].Name, ClientAddressHeaders[i].Namespace, Environment.NewLine);
				ahlist += (", ah" + i.ToString());
			}
			Code.AppendFormat("\t\t\treturn new System.ServiceModel.EndpointAddress(new Uri(\"{0}\"), Identity{1});{2}", GenerateClientEndpointURI(true, true), ahlist, Environment.NewLine);
			Code.AppendLine("\t\t}");
			#endregion

			return Code.ToString();
		}

		#region - Generate Server Endpoint URI -
		public string GenerateServerEndpointURI()
		{
			string URI = "";

			if (ServerAddress == null || ServerAddress == "")
			{
				if (Binding.GetType() == typeof(ServiceBindingBasicHTTP) || Binding.GetType() == typeof(ServiceBindingWebHTTP) || Binding.GetType() == typeof(ServiceBindingWSHTTP) || Binding.GetType() == typeof(ServiceBindingWS2007HTTP) || Binding.GetType() == typeof(ServiceBindingWSDualHTTP) || Binding.GetType() == typeof(ServiceBindingWSFederationHTTP) || Binding.GetType() == typeof(ServiceBindingWS2007FederationHTTP))
				{
					if (ServerUseHTTPS == false)
					{
						URI = "http://\" + Environment.MachineName + \":" + ServerPort.ToString() + "/" + CodeName;
					}
					else
					{
						URI = "https://\" + Environment.MachineName + \":" + ServerPort.ToString() + "/" + CodeName;
					}
				}
				else if (Binding.GetType() == typeof(ServiceBindingTCP))
				{
					URI = "net.tcp://\" + Environment.MachineName + \":" + ServerPort.ToString() + "/" + CodeName;
				}
				else if (Binding.GetType() == typeof(ServiceBindingPeerTCP))
				{
					URI = "net.p2p://\" + Environment.MachineName + \":" + ServerPort.ToString() + "/" + CodeName;
				}
				else if (Binding.GetType() == typeof(ServiceBindingMSMQ) || Binding.GetType() == typeof(ServiceBindingMSMQIntegration))
				{
					URI = "net.msmq://\" + Environment.MachineName + \":" + ServerPort.ToString() + "/" + CodeName;
				}
				else if (Binding.GetType() == typeof(ServiceBindingNamedPipe))
				{
					URI = "net.pipe://\" + Environment.MachineName + \"/" + CodeName;
				}
			}
			else
			{
				if (Binding.GetType() == typeof(ServiceBindingBasicHTTP) || Binding.GetType() == typeof(ServiceBindingWebHTTP) || Binding.GetType() == typeof(ServiceBindingWSHTTP) || Binding.GetType() == typeof(ServiceBindingWS2007HTTP) || Binding.GetType() == typeof(ServiceBindingWSDualHTTP) || Binding.GetType() == typeof(ServiceBindingWSFederationHTTP) || Binding.GetType() == typeof(ServiceBindingWS2007FederationHTTP))
				{
					if (ServerUseHTTPS == false)
					{
						URI = "http://" + ServerAddress + ":" + ServerPort.ToString() + "/" + CodeName;
					}
					else
					{
						URI = "https://" + ServerAddress + ":" + ServerPort.ToString() + "/" + CodeName;
					}
				}
				else if (Binding.GetType() == typeof(ServiceBindingTCP))
				{
					URI = "net.tcp://" + ServerAddress + ":" + ServerPort.ToString() + "/" + CodeName;
				}
				else if (Binding.GetType() == typeof(ServiceBindingPeerTCP))
				{
					URI = "net.p2p://" + ServerAddress + ":" + ServerPort.ToString() + "/" + CodeName;
				}
				else if (Binding.GetType() == typeof(ServiceBindingMSMQ) || Binding.GetType() == typeof(ServiceBindingMSMQIntegration))
				{
					URI = "net.msmq://" + ServerAddress + ":" + ServerPort.ToString() + "/" + CodeName;
				}
				else if (Binding.GetType() == typeof(ServiceBindingNamedPipe))
				{
					URI = "net.pipe://" + ServerAddress + "/" + CodeName;
				}
			}

			return URI;
		}
		#endregion

		#region - Generate Client Endpoint URI
		public string GenerateClientEndpointURI(bool IgnoreAddress, bool IgnorePort)
		{
			string URI = "";

			string tca = ClientAddress;
			if (tca == null || tca == "") tca = "\" + Environment.MachineName + \"";

			if (IgnorePort == false)
			{
				if (IgnoreAddress == false)
				{
					if (Binding.GetType() == typeof(ServiceBindingBasicHTTP) || Binding.GetType() == typeof(ServiceBindingWebHTTP) || Binding.GetType() == typeof(ServiceBindingWSHTTP) || Binding.GetType() == typeof(ServiceBindingWS2007HTTP) || Binding.GetType() == typeof(ServiceBindingWSDualHTTP) || Binding.GetType() == typeof(ServiceBindingWSFederationHTTP) || Binding.GetType() == typeof(ServiceBindingWS2007FederationHTTP))
					{
						if (ServerUseHTTPS == false)
						{
							URI = "http://" + tca + ":" + ServerPort.ToString() + "/" + CodeName;
						}
						else
						{
							URI = "https://" + tca + ":" + ServerPort.ToString() + "/" + CodeName;
						}
					}
					else if (Binding.GetType() == typeof(ServiceBindingTCP))
					{
						URI = "net.tcp://" + tca + ":" + ServerPort.ToString() + "/" + CodeName;
					}
					else if (Binding.GetType() == typeof(ServiceBindingPeerTCP))
					{
						URI = "net.p2p://" + tca + ":" + ServerPort.ToString() + "/" + CodeName;
					}
					else if (Binding.GetType() == typeof(ServiceBindingMSMQ) || Binding.GetType() == typeof(ServiceBindingMSMQIntegration))
					{
						URI = "net.msmq://" + tca + ":" + ServerPort.ToString() + "/" + CodeName;
					}
					else if (Binding.GetType() == typeof(ServiceBindingNamedPipe))
					{
						URI = "net.pipe://" + tca + "/" + CodeName;
					}
				}
				else
				{
					if (Binding.GetType() == typeof(ServiceBindingBasicHTTP) || Binding.GetType() == typeof(ServiceBindingWebHTTP) || Binding.GetType() == typeof(ServiceBindingWSHTTP) || Binding.GetType() == typeof(ServiceBindingWS2007HTTP) || Binding.GetType() == typeof(ServiceBindingWSDualHTTP) || Binding.GetType() == typeof(ServiceBindingWSFederationHTTP) || Binding.GetType() == typeof(ServiceBindingWS2007FederationHTTP))
					{
						if (ServerUseHTTPS == false)
						{
							URI = "http://\" + Address + \":" + ServerPort.ToString() + "/" + CodeName;
						}
						else
						{
							URI = "https://\" + Address + \":" + ServerPort.ToString() + "/" + CodeName;
						}
					}
					else if (Binding.GetType() == typeof(ServiceBindingTCP))
					{
						URI = "net.tcp://\" + Address + \":" + ServerPort.ToString() + "/" + CodeName;
					}
					else if (Binding.GetType() == typeof(ServiceBindingPeerTCP))
					{
						URI = "net.p2p://\" + Address + \":" + ServerPort.ToString() + "/" + CodeName;
					}
					else if (Binding.GetType() == typeof(ServiceBindingMSMQ) || Binding.GetType() == typeof(ServiceBindingMSMQIntegration))
					{
						URI = "net.msmq://\" + Address + \":" + ServerPort.ToString() + "/" + CodeName;
					}
					else if (Binding.GetType() == typeof(ServiceBindingNamedPipe))
					{
						URI = "net.pipe://\" + Address + \"/" + CodeName;
					}
				}
			}
			else
			{
				if (IgnoreAddress == false)
				{
					if (Binding.GetType() == typeof(ServiceBindingBasicHTTP) || Binding.GetType() == typeof(ServiceBindingWebHTTP) || Binding.GetType() == typeof(ServiceBindingWSHTTP) || Binding.GetType() == typeof(ServiceBindingWS2007HTTP) || Binding.GetType() == typeof(ServiceBindingWSDualHTTP) || Binding.GetType() == typeof(ServiceBindingWSFederationHTTP) || Binding.GetType() == typeof(ServiceBindingWS2007FederationHTTP))
					{
						if (ServerUseHTTPS == false)
						{
							URI = "http://" + tca + ":\" + Port.ToString() +\"/" + CodeName;
						}
						else
						{
							URI = "https://" + tca + ":\" + Port.ToString() +\"/" + CodeName;
						}
					}
					else if (Binding.GetType() == typeof(ServiceBindingTCP))
					{
						URI = "net.tcp://" + tca + ":\" + Port.ToString() +\"/" + CodeName;
					}
					else if (Binding.GetType() == typeof(ServiceBindingPeerTCP))
					{
						URI = "net.p2p://" + tca + ":\" + Port.ToString() +\"/" + CodeName;
					}
					else if (Binding.GetType() == typeof(ServiceBindingMSMQ) || Binding.GetType() == typeof(ServiceBindingMSMQIntegration))
					{
						URI = "net.msmq://" + tca + ":\" + Port.ToString() +\"/" + CodeName;
					}
					else if (Binding.GetType() == typeof(ServiceBindingNamedPipe))
					{
						URI = "net.pipe://" + tca + "/" + CodeName;
					}
				}
				else
				{
					if (Binding.GetType() == typeof(ServiceBindingBasicHTTP) || Binding.GetType() == typeof(ServiceBindingWebHTTP) || Binding.GetType() == typeof(ServiceBindingWSHTTP) || Binding.GetType() == typeof(ServiceBindingWS2007HTTP) || Binding.GetType() == typeof(ServiceBindingWSDualHTTP) || Binding.GetType() == typeof(ServiceBindingWSFederationHTTP) || Binding.GetType() == typeof(ServiceBindingWS2007FederationHTTP))
					{
						if (ServerUseHTTPS == false)
						{
							URI = "http://\" + Address + \":\" + Port.ToString() +\"/" + CodeName;
						}
						else
						{
							URI = "https://\" + Address + \":\" + Port.ToString() +\"/" + CodeName;
						}
					}
					else if (Binding.GetType() == typeof(ServiceBindingTCP))
					{
						URI = "net.tcp://\" + Address + \":\" + Port.ToString() +\"/" + CodeName;
					}
					else if (Binding.GetType() == typeof(ServiceBindingPeerTCP))
					{
						URI = "net.p2p://\" + Address + \":\" + Port.ToString() +\"/" + CodeName;
					}
					else if (Binding.GetType() == typeof(ServiceBindingMSMQ) || Binding.GetType() == typeof(ServiceBindingMSMQIntegration))
					{
						URI = "net.msmq://\" + Address + \":\" + Port.ToString() +\"/" + CodeName;
					}
					else if (Binding.GetType() == typeof(ServiceBindingNamedPipe))
					{
						URI = "net.pipe://\" + Address + \"/" + CodeName;
					}
				}
			}

			return URI;
		}
		#endregion
	}

	internal class HostEndpointAddressHeader : DependencyObject
	{
		protected Guid id;
		public Guid ID { get { return id; } }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(HostEndpointAddressHeader));

		public string Namespace { get { return (string)GetValue(NamespaceProperty); } set { SetValue(NamespaceProperty, value); } }
		public static readonly DependencyProperty NamespaceProperty = DependencyProperty.Register("Namespace", typeof(string), typeof(HostEndpointAddressHeader));

		public HostEndpointAddressHeader() { }

		public HostEndpointAddressHeader(string Name, string Namespace)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.Namespace = Namespace;
		}
	}

	internal class HostCredentials : DependencyObject
	{
		public bool UseCertificatesSecurity { get { return (bool)GetValue(UseCertificatesSecurityProperty); } set { SetValue(UseCertificatesSecurityProperty, value); } }
		public static readonly DependencyProperty UseCertificatesSecurityProperty = DependencyProperty.Register("UseCertificatesSecurity", typeof(bool), typeof(HostCredentials));
		
		public bool UseIssuedTokenSecurity { get { return (bool)GetValue(UseIssuedTokenSecurityProperty); } set { SetValue(UseIssuedTokenSecurityProperty, value); } }
		public static readonly DependencyProperty UseIssuedTokenSecurityProperty = DependencyProperty.Register("UseIssuedTokenSecurity", typeof(bool), typeof(HostCredentials));
		
		public bool UsePeerSecurity { get { return (bool)GetValue(UsePeerSecurityProperty); } set { SetValue(UsePeerSecurityProperty, value); } }
		public static readonly DependencyProperty UsePeerSecurityProperty = DependencyProperty.Register("UsePeerSecurity", typeof(bool), typeof(HostCredentials));
		
		public bool UseUserNamePasswordSecurity { get { return (bool)GetValue(UseUserNamePasswordSecurityProperty); } set { SetValue(UseUserNamePasswordSecurityProperty, value); } }
		public static readonly DependencyProperty UseUserNamePasswordSecurityProperty = DependencyProperty.Register("UseUserNamePasswordSecurity", typeof(bool), typeof(HostCredentials));
		
		public bool UseWindowsServiceSecurity { get { return (bool)GetValue(UseWindowsServiceSecurityProperty); } set { SetValue(UseWindowsServiceSecurityProperty, value); } }
		public static readonly DependencyProperty UseWindowsServiceSecurityProperty = DependencyProperty.Register("UseWindowsServiceSecurity", typeof(bool), typeof(HostCredentials));

		public System.ServiceModel.Security.X509CertificateValidationMode ClientCertificateAuthenticationValidationMode { get { return (System.ServiceModel.Security.X509CertificateValidationMode)GetValue(ClientCertificateAuthenticationValidationModeProperty); } set { SetValue(ClientCertificateAuthenticationValidationModeProperty, value); } }
		public static readonly DependencyProperty ClientCertificateAuthenticationValidationModeProperty = DependencyProperty.Register("ClientCertificateAuthenticationValidationMode", typeof(System.ServiceModel.Security.X509CertificateValidationMode), typeof(HostBehavior));
		
		public bool ClientCertificateAuthenticationIncludeWindowsGroups { get { return (bool)GetValue(ClientCertificateAuthenticationIncludeWindowsGroupsProperty); } set { SetValue(ClientCertificateAuthenticationIncludeWindowsGroupsProperty, value); } }
		public static readonly DependencyProperty ClientCertificateAuthenticationIncludeWindowsGroupsProperty = DependencyProperty.Register("ClientCertificateAuthenticationIncludeWindowsGroups", typeof(bool), typeof(HostBehavior));
		
		public bool ClientCertificateAuthenticationMapClientCertificateToWindowsAccount { get { return (bool)GetValue(ClientCertificateAuthenticationMapClientCertificateToWindowsAccountProperty); } set { SetValue(ClientCertificateAuthenticationMapClientCertificateToWindowsAccountProperty, value); } }
		public static readonly DependencyProperty ClientCertificateAuthenticationMapClientCertificateToWindowsAccountProperty = DependencyProperty.Register("ClientCertificateAuthenticationMapClientCertificateToWindowsAccount", typeof(bool), typeof(HostBehavior));
		
		public System.Security.Cryptography.X509Certificates.X509RevocationMode ClientCertificateAuthenticationRevocationMode { get { return (System.Security.Cryptography.X509Certificates.X509RevocationMode)GetValue(ClientCertificateAuthenticationRevocationModeProperty); } set { SetValue(ClientCertificateAuthenticationRevocationModeProperty, value); } }
		public static readonly DependencyProperty ClientCertificateAuthenticationRevocationModeProperty = DependencyProperty.Register("ClientCertificateAuthenticationRevocationMode", typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), typeof(HostBehavior));
		
		public System.Security.Cryptography.X509Certificates.StoreLocation ClientCertificateAuthenticationStoreLocation { get { return (System.Security.Cryptography.X509Certificates.StoreLocation)GetValue(ClientCertificateAuthenticationStoreLocationProperty); } set { SetValue(ClientCertificateAuthenticationStoreLocationProperty, value); } }
		public static readonly DependencyProperty ClientCertificateAuthenticationStoreLocationProperty = DependencyProperty.Register("ClientCertificateAuthenticationStoreLocation", typeof(System.Security.Cryptography.X509Certificates.StoreLocation), typeof(HostBehavior));

		public ObservableCollection<string> IssuedTokenAllowedAudiencesUris { get { return (ObservableCollection<string>)GetValue(IssuedTokenAllowedAudiencesUrisProperty); } set { SetValue(IssuedTokenAllowedAudiencesUrisProperty, value); } }
		public static readonly DependencyProperty IssuedTokenAllowedAudiencesUrisProperty = DependencyProperty.Register("IssuedTokenAllowedAudiencesUris", typeof(ObservableCollection<string>), typeof(HostBehavior));

		public bool IssuedTokenAllowUntrustedRsaIssuers { get { return (bool)GetValue(IssuedTokenAllowUntrustedRsaIssuersProperty); } set { SetValue(IssuedTokenAllowUntrustedRsaIssuersProperty, value); } }
		public static readonly DependencyProperty IssuedTokenAllowUntrustedRsaIssuersProperty = DependencyProperty.Register("IssuedTokenAllowUntrustedRsaIssuers", typeof(bool), typeof(HostBehavior));
		
		public System.IdentityModel.Selectors.AudienceUriMode IssuedTokenAudienceUriMode { get { return (System.IdentityModel.Selectors.AudienceUriMode)GetValue(IssuedTokenAudienceUriModeProperty); } set { SetValue(IssuedTokenAudienceUriModeProperty, value); } }
		public static readonly DependencyProperty IssuedTokenAudienceUriModeProperty = DependencyProperty.Register("IssuedTokenAudienceUriMode", typeof(System.IdentityModel.Selectors.AudienceUriMode), typeof(HostBehavior));

		public System.ServiceModel.Security.X509CertificateValidationMode IssuedTokenCertificateValidationMode { get { return (System.ServiceModel.Security.X509CertificateValidationMode)GetValue(IssuedTokenCertificateValidationModeProperty); } set { SetValue(IssuedTokenCertificateValidationModeProperty, value); } }
		public static readonly DependencyProperty IssuedTokenCertificateValidationModeProperty = DependencyProperty.Register("IssuedTokenCertificateValidationMode", typeof(System.ServiceModel.Security.X509CertificateValidationMode), typeof(HostBehavior));

		public System.Security.Cryptography.X509Certificates.X509RevocationMode IssuedTokenRevocationMode { get { return (System.Security.Cryptography.X509Certificates.X509RevocationMode)GetValue(IssuedTokenRevocationModeProperty); } set { SetValue(IssuedTokenRevocationModeProperty, value); } }
		public static readonly DependencyProperty IssuedTokenRevocationModeProperty = DependencyProperty.Register("IssuedTokenRevocationMode", typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), typeof(HostBehavior));

		public System.Security.Cryptography.X509Certificates.StoreLocation IssuedTokenTrustedStoreLocation { get { return (System.Security.Cryptography.X509Certificates.StoreLocation)GetValue(IssuedTokenTrustedStoreLocationProperty); } set { SetValue(IssuedTokenTrustedStoreLocationProperty, value); } }
		public static readonly DependencyProperty IssuedTokenTrustedStoreLocationProperty = DependencyProperty.Register("IssuedTokenTrustedStoreLocation", typeof(System.Security.Cryptography.X509Certificates.StoreLocation), typeof(HostBehavior));

		public string PeerMeshPassword { get { return (string)GetValue(PeerMeshPasswordProperty); } set { SetValue(PeerMeshPasswordProperty, value); } }
		public static readonly DependencyProperty PeerMeshPasswordProperty = DependencyProperty.Register("PeerMeshPassword", typeof(string), typeof(HostBehavior));

		public System.ServiceModel.Security.X509CertificateValidationMode PeerMessageSenderAuthenticationCertificateValidationMode { get { return (System.ServiceModel.Security.X509CertificateValidationMode)GetValue(PeerMessageSenderAuthenticationCertificateValidationModeProperty); } set { SetValue(PeerMessageSenderAuthenticationCertificateValidationModeProperty, value); } }
		public static readonly DependencyProperty PeerMessageSenderAuthenticationCertificateValidationModeProperty = DependencyProperty.Register("PeerMessageSenderAuthenticationCertificateValidationMode ", typeof(System.ServiceModel.Security.X509CertificateValidationMode), typeof(HostBehavior));

		public System.Security.Cryptography.X509Certificates.X509RevocationMode PeerMessageSenderAuthenticationRevocationMode { get { return (System.Security.Cryptography.X509Certificates.X509RevocationMode)GetValue(PeerMessageSenderAuthenticationRevocationModeProperty); } set { SetValue(PeerMessageSenderAuthenticationRevocationModeProperty, value); } }
		public static readonly DependencyProperty PeerMessageSenderAuthenticationRevocationModeProperty = DependencyProperty.Register("PeerMessageSenderAuthenticationRevocationMode", typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), typeof(HostBehavior));

		public System.Security.Cryptography.X509Certificates.StoreLocation PeerMessageSenderAuthenticationTrustedStoreLocation { get { return (System.Security.Cryptography.X509Certificates.StoreLocation)GetValue(PeerMessageSenderAuthenticationTrustedStoreLocationProperty); } set { SetValue(PeerMessageSenderAuthenticationTrustedStoreLocationProperty, value); } }
		public static readonly DependencyProperty PeerMessageSenderAuthenticationTrustedStoreLocationProperty = DependencyProperty.Register("PeerMessageSenderAuthenticationTrustedStoreLocation", typeof(System.Security.Cryptography.X509Certificates.StoreLocation), typeof(HostBehavior));

		public System.ServiceModel.Security.X509CertificateValidationMode PeerAuthenticationCertificateValidationMode { get { return (System.ServiceModel.Security.X509CertificateValidationMode)GetValue(PeerAuthenticationCertificateValidationModeProperty); } set { SetValue(PeerAuthenticationCertificateValidationModeProperty, value); } }
		public static readonly DependencyProperty PeerAuthenticationCertificateValidationModeProperty = DependencyProperty.Register("PeerAuthenticationCertificateValidationMode", typeof(System.ServiceModel.Security.X509CertificateValidationMode), typeof(HostBehavior));

		public System.Security.Cryptography.X509Certificates.X509RevocationMode PeerAuthenticationRevocationMode { get { return (System.Security.Cryptography.X509Certificates.X509RevocationMode)GetValue(PeerAuthenticationRevocationModeProperty); } set { SetValue(PeerAuthenticationRevocationModeProperty, value); } }
		public static readonly DependencyProperty PeerAuthenticationRevocationModeProperty = DependencyProperty.Register("PeerAuthenticationRevocationMode", typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), typeof(HostBehavior));

		public System.Security.Cryptography.X509Certificates.StoreLocation PeerAuthenticationTrustedStoreLocation { get { return (System.Security.Cryptography.X509Certificates.StoreLocation)GetValue(PeerAuthenticationTrustedStoreLocationProperty); } set { SetValue(PeerAuthenticationTrustedStoreLocationProperty, value); } } 
		public static readonly DependencyProperty PeerAuthenticationTrustedStoreLocationProperty = DependencyProperty.Register("PeerAuthenticationTrustedStoreLocation", typeof(System.Security.Cryptography.X509Certificates.StoreLocation), typeof(HostBehavior));

		public TimeSpan UserNamePasswordCachedLogonTokenLifetime { get { return (TimeSpan)GetValue(UserNamePasswordCachedLogonTokenLifetimeProperty); } set { SetValue(UserNamePasswordCachedLogonTokenLifetimeProperty, value); } } 
		public static readonly DependencyProperty UserNamePasswordCachedLogonTokenLifetimeProperty = DependencyProperty.Register("UserNamePasswordCachedLogonTokenLifetime", typeof(TimeSpan), typeof(HostBehavior));
		
		public bool UserNamePasswordCacheLogonTokens { get { return (bool)GetValue(UserNamePasswordCacheLogonTokensProperty); } set { SetValue(UserNamePasswordCacheLogonTokensProperty, value); } }
		public static readonly DependencyProperty UserNamePasswordCacheLogonTokensProperty = DependencyProperty.Register("UserNamePasswordCacheLogonTokens", typeof(bool), typeof(HostBehavior));
		
		public bool UserNamePasswordIncludeWindowsGroups { get { return (bool)GetValue(UserNamePasswordIncludeWindowsGroupsProperty); } set { SetValue(UserNamePasswordIncludeWindowsGroupsProperty, value); } }
		public static readonly DependencyProperty UserNamePasswordIncludeWindowsGroupsProperty = DependencyProperty.Register("UserNamePasswordIncludeWindowsGroups", typeof(bool), typeof(HostBehavior));
		
		public int UserNamePasswordMaxCachedLogonTokens { get { return (int)GetValue(UserNamePasswordMaxCachedLogonTokensProperty); } set { SetValue(UserNamePasswordMaxCachedLogonTokensProperty, value); } }
		public static readonly DependencyProperty UserNamePasswordMaxCachedLogonTokensProperty = DependencyProperty.Register("UserNamePasswordMaxCachedLogonTokens", typeof(int), typeof(HostBehavior));
		
		public System.ServiceModel.Security.UserNamePasswordValidationMode UserNamePasswordValidationMode { get { return (System.ServiceModel.Security.UserNamePasswordValidationMode)GetValue(UserNamePasswordValidationModeProperty); } set { SetValue(UserNamePasswordValidationModeProperty, value); } }
		public static readonly DependencyProperty UserNamePasswordValidationModeProperty = DependencyProperty.Register("UserNamePasswordValidationMode", typeof(System.ServiceModel.Security.UserNamePasswordValidationMode), typeof(HostBehavior));
		
		public bool WindowsServiceAllowAnonymousLogons { get { return (bool)GetValue(WindowsServiceAllowAnonymousLogonsProperty); } set { SetValue(WindowsServiceAllowAnonymousLogonsProperty, value); } }
		public static readonly DependencyProperty WindowsServiceAllowAnonymousLogonsProperty = DependencyProperty.Register("WindowsServiceAllowAnonymousLogons", typeof(bool), typeof(HostBehavior));
		
		public bool WindowsServiceIncludeWindowsGroups { get { return (bool)GetValue(WindowsServiceIncludeWindowsGroupsProperty); } set { SetValue(WindowsServiceIncludeWindowsGroupsProperty, value); } }
		public static readonly DependencyProperty WindowsServiceIncludeWindowsGroupsProperty = DependencyProperty.Register("WindowsServiceIncludeWindowsGroups", typeof(bool), typeof(HostBehavior));

		public Host Owner { get; set; }

		public HostCredentials()
		{
			this.IssuedTokenAllowedAudiencesUris = new ObservableCollection<string>();
		}

		public HostCredentials(Host Owner)
		{
			this.IssuedTokenAllowedAudiencesUris = new ObservableCollection<string>();
			this.Owner = Owner;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (Owner != null)
				if (Owner.Parent != null)
					Owner.Parent.IsDirty = true;
		}

		public bool VerifyCode(Compiler.Compiler Compiler)
		{
			return true;
		}

		public string GenerateCode30()
		{
			StringBuilder Code = new StringBuilder();
			if (UseCertificatesSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.ClientCertificate.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), ClientCertificateAuthenticationValidationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ClientCertificate.IncludeWindowsGroups = {0};{1}", ClientCertificateAuthenticationIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ClientCertificate.MapClientCertificateToWindowsAccount = {0};{1}", ClientCertificateAuthenticationMapClientCertificateToWindowsAccount ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ClientCertificate.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), ClientCertificateAuthenticationRevocationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ClientCertificate.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), ClientCertificateAuthenticationStoreLocation), Environment.NewLine);
			}
			if (UseIssuedTokenSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.AllowUntrustedRsaIssuers = {0};{1}", IssuedTokenAllowUntrustedRsaIssuers ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), IssuedTokenCertificateValidationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), IssuedTokenRevocationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), IssuedTokenTrustedStoreLocation), Environment.NewLine);
			}
			if (UsePeerSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.Peer.MeshPassword = \"{0}\";{1}", PeerMeshPassword, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.MessageSenderAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), PeerMessageSenderAuthenticationCertificateValidationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.MessageSenderAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), PeerMessageSenderAuthenticationRevocationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.MessageSenderAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), PeerMessageSenderAuthenticationTrustedStoreLocation), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.PeerAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), PeerAuthenticationCertificateValidationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.PeerAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), PeerAuthenticationRevocationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.PeerAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), PeerAuthenticationTrustedStoreLocation), Environment.NewLine);
			}
			if (UseUserNamePasswordSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.CachedLogonTokenLifetime = new TimeSpan({0});{1}", UserNamePasswordCachedLogonTokenLifetime.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.CacheLogonTokens = {0};{1}", UserNamePasswordCacheLogonTokens ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.IncludeWindowsGroups = {0};{1}", UserNamePasswordIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.MaxCachedLogonTokens = {0};{1}", UserNamePasswordMaxCachedLogonTokens, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.UserNamePasswordValidationMode = System.ServiceModel.Security.UserNamePasswordValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.UserNamePasswordValidationMode), UserNamePasswordValidationMode), Environment.NewLine);
			}
			if (UseWindowsServiceSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.WindowsAuthentication.AllowAnonymousLogons = {0};{1}", WindowsServiceAllowAnonymousLogons ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.WindowsAuthentication.IncludeWindowsGroups = {0};{1}", WindowsServiceIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			}
			return Code.ToString();
		}

		public string GenerateCode35()
		{
			return GenerateCode40();
		}

		public string GenerateCode40()
		{
			StringBuilder Code = new StringBuilder();
			if (UseCertificatesSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.ClientCertificate.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), ClientCertificateAuthenticationValidationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ClientCertificate.IncludeWindowsGroups = {0};{1}", ClientCertificateAuthenticationIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ClientCertificate.MapClientCertificateToWindowsAccount = {0};{1}", ClientCertificateAuthenticationMapClientCertificateToWindowsAccount ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ClientCertificate.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), ClientCertificateAuthenticationRevocationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.ClientCertificate.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), ClientCertificateAuthenticationStoreLocation), Environment.NewLine);
			}
			if (UseIssuedTokenSecurity == true)
			{
				foreach (string AAURI in IssuedTokenAllowedAudiencesUris) Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.AllowedAudienceUris.Add(\"{0}\");{1}", AAURI, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.AllowUntrustedRsaIssuers = {0};{1}", IssuedTokenAllowUntrustedRsaIssuers ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.AudienceUriMode = System.IdentityModel.Selectors.AudienceUriMode.{0};{1}", System.Enum.GetName(typeof(System.IdentityModel.Selectors.AudienceUriMode), IssuedTokenAudienceUriMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), IssuedTokenCertificateValidationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), IssuedTokenRevocationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.IssuedTokenAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), IssuedTokenTrustedStoreLocation), Environment.NewLine);
			}
			if (UsePeerSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.Peer.MeshPassword = \"{0}\";{1}", PeerMeshPassword, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.MessageSenderAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), PeerMessageSenderAuthenticationCertificateValidationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.MessageSenderAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), PeerMessageSenderAuthenticationRevocationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.MessageSenderAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), PeerMessageSenderAuthenticationTrustedStoreLocation), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.PeerAuthentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.X509CertificateValidationMode), PeerAuthenticationCertificateValidationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.PeerAuthentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), PeerAuthenticationRevocationMode), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.Peer.PeerAuthentication.TrustedStoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation.{0};{1}", System.Enum.GetName(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), PeerAuthenticationTrustedStoreLocation), Environment.NewLine);
			}
			if (UseUserNamePasswordSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.CachedLogonTokenLifetime = new TimeSpan({0});{1}", UserNamePasswordCachedLogonTokenLifetime.Ticks, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.CacheLogonTokens = {0};{1}", UserNamePasswordCacheLogonTokens ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.IncludeWindowsGroups = {0};{1}", UserNamePasswordIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.MaxCachedLogonTokens = {0};{1}", UserNamePasswordMaxCachedLogonTokens, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.UserNameAuthentication.CertificateValidationMode = System.ServiceModel.Security.UserNamePasswordValidationMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.Security.UserNamePasswordValidationMode), UserNamePasswordValidationMode), Environment.NewLine);
			}
			if (UseWindowsServiceSecurity == true)
			{
				Code.AppendFormat("\t\t\tthis.WindowsAuthentication.AllowAnonymousLogons = {0};{1}", WindowsServiceAllowAnonymousLogons ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.WindowsAuthentication.IncludeWindowsGroups = {0};{1}", WindowsServiceIncludeWindowsGroups ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			}
			return Code.ToString();
		}
	}

	internal abstract class HostBehavior : DependencyObject
	{
		protected Guid id;
		public Guid ID { get { return id; } }
		
		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(HostBehavior));
		
		public string CodeName { get { return (string)GetValue(CodeNameProperty); } set { SetValue(CodeNameProperty, value); } }
		public static readonly DependencyProperty CodeNameProperty = DependencyProperty.Register("CodeName", typeof(string), typeof(HostBehavior));

		public bool IsDefaultBehavior { get { return (bool)GetValue(IsDefaultBehaviorProperty); } set { SetValue(IsDefaultBehaviorProperty, value); } }
		public static readonly DependencyProperty IsDefaultBehaviorProperty = DependencyProperty.Register("IsDefaultBehavior", typeof(bool), typeof(HostBehavior), new UIPropertyMetadata(true));

		public Host Parent { get { return (Host)GetValue(ParentProperty); } set { SetValue(ParentProperty, value); } }
		public static readonly DependencyProperty ParentProperty =  DependencyProperty.Register("Parent", typeof(Host), typeof(HostBehavior));

		public HostBehavior() { }

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Project || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Parent, this));
							if (CodeName != null && CodeName != "") if (CodeName.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Code Name", CodeName, Parent.Parent, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Parent, this));
							if (CodeName != null && CodeName != "") if (CodeName.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Code Name", CodeName, Parent.Parent, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Parent.Parent, this));
						if (CodeName != null && CodeName != "") if (Args.RegexSearch.IsMatch(CodeName)) results.Add(new FindReplaceResult("Code Name", CodeName, Parent.Parent, this));
					}
				}

				if (Args.ReplaceAll == true)
				{
					bool ia = Parent.Parent.IsActive;
					Parent.Parent.IsActive = true;
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (CodeName != null && CodeName != "") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (CodeName != null && CodeName != "") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (CodeName != null && CodeName != "") CodeName = Args.RegexSearch.Replace(CodeName, Args.Replace);
						}
					}
					Parent.Parent.IsActive = ia;
				}
			}

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
		{
			if (Args.ReplaceAll == true)
			{
				bool ia = Parent.Parent.IsActive;
				Parent.Parent.IsActive = true;
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Code Name") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Code Name") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (Field == "Code Name") CodeName = Args.RegexSearch.Replace(CodeName, Args.Replace);
					}
				}
				Parent.Parent.IsActive = ia;
			}
		}

		public abstract bool VerifyCode(Compiler.Compiler Compiler);
		public abstract string GenerateCode30();
		public abstract string GenerateCode35();
		public abstract string GenerateCode40();
	}

	internal class HostDebugBehavior : HostBehavior
	{
		public ServiceBinding HttpHelpPageBinding { get { return (ServiceBinding)GetValue(HttpHelpPageBindingProperty); } set { SetValue(HttpHelpPageBindingProperty, value); } }
		public static readonly DependencyProperty HttpHelpPageBindingProperty = DependencyProperty.Register("HttpHelpPageBinding", typeof(ServiceBinding), typeof(HostDebugBehavior));

		public bool HttpHelpPageEnabled { get { return (bool)GetValue(HttpHelpPageEnabledProperty); } set { SetValue(HttpHelpPageEnabledProperty, value); } }
		public static readonly DependencyProperty HttpHelpPageEnabledProperty = DependencyProperty.Register("HttpHelpPageEnabled", typeof(bool), typeof(HostDebugBehavior));
		
		public string HttpHelpPageUrl { get { return (string)GetValue(HttpHelpPageUrlProperty); } set { SetValue(HttpHelpPageUrlProperty, value); } }
		public static readonly DependencyProperty HttpHelpPageUrlProperty = DependencyProperty.Register("HttpHelpPageUrl", typeof(string), typeof(HostDebugBehavior));
		
		public ServiceBinding HttpsHelpPageBinding { get { return (ServiceBinding)GetValue(HttpsHelpPageBindingProperty); } set { SetValue(HttpsHelpPageBindingProperty, value); } }
		public static readonly DependencyProperty HttpsHelpPageBindingProperty = DependencyProperty.Register("HttpsHelpPageBinding", typeof(ServiceBinding), typeof(HostDebugBehavior));
		
		public bool HttpsHelpPageEnabled { get { return (bool)GetValue(HttpsHelpPageEnabledProperty); } set { SetValue(HttpsHelpPageEnabledProperty, value); } }
		public static readonly DependencyProperty HttpsHelpPageEnabledProperty = DependencyProperty.Register("HttpsHelpPageEnabled", typeof(bool), typeof(HostDebugBehavior));
		
		public string HttpsHelpPageUrl { get { return (string)GetValue(HttpsHelpPageUrlProperty); } set { SetValue(HttpsHelpPageUrlProperty, value); } }
		public static readonly DependencyProperty HttpsHelpPageUrlProperty = DependencyProperty.Register("HttpsHelpPageUrl", typeof(string), typeof(HostDebugBehavior));
		
		public bool IncludeExceptionDetailInFaults { get { return (bool)GetValue(IncludeExceptionDetailInFaultsProperty); } set { SetValue(IncludeExceptionDetailInFaultsProperty, value); } }
		public static readonly DependencyProperty IncludeExceptionDetailInFaultsProperty = DependencyProperty.Register("IncludeExceptionDetailInFaults", typeof(bool), typeof(HostDebugBehavior));
				
		public HostDebugBehavior() { }

		public HostDebugBehavior(string Name, Host Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.Parent = Parent;
			this.IsDefaultBehavior = false;

			this.HttpHelpPageBinding = null;
			this.HttpHelpPageEnabled = false;
			this.HttpHelpPageUrl = "";
			this.HttpsHelpPageBinding = null;
			this.HttpsHelpPageEnabled = false;
			this.HttpsHelpPageUrl = "";
			this.IncludeExceptionDetailInFaults = false;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (Parent != null)
				if (Parent.Parent != null)
					Parent.Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (Helpers.RegExs.MatchHTTPURI.IsMatch(HttpHelpPageUrl) == false)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5007", "The HTTP Help Page URL '" + HttpHelpPageUrl + "' for the '" + Parent.Name + "' host in the '" + Parent.Parent.Name + "' project is not a valid URI. The software may not be able to access the specified page.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
			if (Helpers.RegExs.MatchHTTPURI.IsMatch(HttpsHelpPageUrl) == false)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5008", "The HTTPS Help Page URL '" + HttpsHelpPageUrl + "' for the '" + Parent.Name + "' host in the '" + Parent.Parent.Name + "' project is not a valid URI. The software may not be able to access the specified page.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));

			return NoErrors;
		}

		public override string GenerateCode30()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\t\tthis.{0} = new ServiceDebugBehavior();{1}", CodeName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpHelpPageEnabled = {1};{2}", CodeName, HttpHelpPageEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpHelpPageUrl = new Uri({1});{2}", CodeName, HttpHelpPageUrl, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpsHelpPageEnabled = {1};{2}", CodeName, HttpsHelpPageEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpsHelpPageUrl = new Uri({1});{2}", CodeName, HttpsHelpPageUrl, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.IncludeExceptionDetailInFaults = new Uri({1});{2}", CodeName, IncludeExceptionDetailInFaults == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (IsDefaultBehavior == true) Code.AppendFormat("\t\t\tthis.Description.Behaviors.Add({0});{1}", CodeName, Environment.NewLine);
			return Code.ToString();
		}

		public override string GenerateCode35()
		{
			return GenerateCode40();
		}

		public override string GenerateCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\t\tthis.{0} = new ServiceDebugBehavior();{1}", CodeName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpHelpPageBinding = new {1}.{2}();{3}", CodeName, HttpHelpPageBinding.Parent.FullName, HttpHelpPageBinding.CodeName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpHelpPageEnabled = {1};{2}", CodeName, HttpHelpPageEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpHelpPageUrl = new Uri({1});{2}", CodeName, HttpHelpPageUrl, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpsHelpPageBinding = new {1}.{2}();{3}", CodeName, HttpsHelpPageBinding.Parent.FullName, HttpsHelpPageBinding.CodeName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpsHelpPageEnabled = {1};{2}", CodeName, HttpsHelpPageEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpsHelpPageUrl = new Uri({1});{2}", CodeName, HttpsHelpPageUrl, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.IncludeExceptionDetailInFaults = new Uri({1});{2}", CodeName, IncludeExceptionDetailInFaults == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (IsDefaultBehavior == true) Code.AppendFormat("\t\t\tthis.Description.Behaviors.Add({0});{1}", CodeName, Environment.NewLine);
			return Code.ToString();
		}
	}

	internal class HostMetadataBehavior : HostBehavior
	{
		public string ExternalMetadataLocation { get { return (string)GetValue(ExternalMetadataLocationProperty); } set { SetValue(ExternalMetadataLocationProperty, value); } }
		public static readonly DependencyProperty ExternalMetadataLocationProperty =  DependencyProperty.Register("ExternalMetadataLocation", typeof(string), typeof(HostMetadataBehavior));

		public ServiceBinding HttpGetBinding { get { return (ServiceBinding)GetValue(HttpGetBindingProperty); } set { SetValue(HttpGetBindingProperty, value); } }
		public static readonly DependencyProperty HttpGetBindingProperty = DependencyProperty.Register("HttpGetBinding", typeof(ServiceBinding), typeof(HostMetadataBehavior));

		public bool HttpGetEnabled { get { return (bool)GetValue(HttpGetEnabledProperty); } set { SetValue(HttpGetEnabledProperty, value); } }
		public static readonly DependencyProperty HttpGetEnabledProperty = DependencyProperty.Register("HttpGetEnabled", typeof(bool), typeof(HostMetadataBehavior));
		
		public string HttpGetUrl { get { return (string)GetValue(HttpGetUrlProperty); } set { SetValue(HttpGetUrlProperty, value); } }
		public static readonly DependencyProperty HttpGetUrlProperty = DependencyProperty.Register("HttpGetUrl", typeof(string), typeof(HostMetadataBehavior));
		
		public ServiceBinding HttpsGetBinding { get { return (ServiceBinding)GetValue(HttpsGetBindingProperty); } set { SetValue(HttpsGetBindingProperty, value); } }
		public static readonly DependencyProperty HttpsGetBindingProperty = DependencyProperty.Register("HttpsGetBinding", typeof(ServiceBinding), typeof(HostMetadataBehavior));
		
		public bool HttpsGetEnabled { get { return (bool)GetValue(HttpsGetEnabledProperty); } set { SetValue(HttpsGetEnabledProperty, value); } }
		public static readonly DependencyProperty HttpsGetEnabledProperty = DependencyProperty.Register("HttpsGetEnabled", typeof(bool), typeof(HostMetadataBehavior));
		
		public string HttpsGetUrl { get { return (string)GetValue(HttpsGetUrlProperty); } set { SetValue(HttpsGetUrlProperty, value); } }
		public static readonly DependencyProperty HttpsGetUrlProperty = DependencyProperty.Register("HttpsGetUrl", typeof(string), typeof(HostMetadataBehavior));

		public HostMetadataBehavior() { }

		public HostMetadataBehavior(string Name, Host Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.Parent = Parent;

			this.ExternalMetadataLocation = "";
			this.HttpGetBinding = null;
			this.HttpGetEnabled = false;
			this.HttpGetUrl = "";
			this.HttpsGetBinding = null;
			this.HttpsGetEnabled = false;
			this.HttpsGetUrl = "";
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (Parent != null)
				if (Parent.Parent != null)
					Parent.Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (Helpers.RegExs.MatchHTTPURI.IsMatch(HttpGetUrl) == false)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5009", "The HTTP Get URL '" + HttpGetUrl + "' for the '" + Parent.Name + "' host in the '" + Parent.Parent.Name + "' project is not a valid URI. The software may not be able to access the specified page.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
			if (Helpers.RegExs.MatchHTTPURI.IsMatch(HttpsGetUrl) == false)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS5010", "The HTTPS Get URL '" + HttpsGetUrl + "' for the '" + Parent.Name + "' host in the '" + Parent.Parent.Name + "' project is not a valid URI. The software may not be able to access the specified page.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));

			return NoErrors;
		}

		public override string GenerateCode30()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\t\tthis.{0} = new ServiceMetadataBehavior();{1}", CodeName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.ExternalMetadataLocation = new Uri({1});{2}", CodeName, ExternalMetadataLocation, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpGetEnabled = {1};{2}", CodeName, HttpGetEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpGetUrl = new Uri({1});{2}", CodeName, HttpGetUrl, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpsGetEnabled = {1};{2}", CodeName, HttpsGetEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpsGetUrl = new Uri({1});{2}", CodeName, HttpsGetUrl, Environment.NewLine);
			if (IsDefaultBehavior == true) Code.AppendFormat("\t\t\tthis.Description.Behaviors.Add({0});{1}", CodeName, Environment.NewLine);
			return Code.ToString();
		}

		public override string GenerateCode35()
		{
			return GenerateCode40();
		}

		public override string GenerateCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\t\tthis.{0} = new ServiceMetadataBehavior();{1}", CodeName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.ExternalMetadataLocation = new Uri({1});{2}", CodeName, ExternalMetadataLocation, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpGetBinding = new {1}.{2}();{3}", CodeName, HttpGetBinding.Parent.FullName, HttpGetBinding.CodeName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpGetEnabled = {1};{2}", CodeName, HttpGetEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpGetUrl = new Uri({1});{2}", CodeName, HttpGetUrl, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpsGetBinding = new {1}.{2}();{3}", CodeName, HttpsGetBinding.Parent.FullName, HttpsGetBinding.CodeName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpsGetEnabled = {1};{2}", CodeName, HttpsGetEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.HttpsGetUrl = new Uri({1});{2}", CodeName, HttpsGetUrl, Environment.NewLine);
			if (IsDefaultBehavior == true) Code.AppendFormat("\t\t\tthis.Description.Behaviors.Add({0});{1}", CodeName, Environment.NewLine);
			return Code.ToString();
		}
	}

	internal class HostThrottlingBehavior : HostBehavior
	{
		public int MaxConcurrentCalls { get { return (int)GetValue(MaxConcurrentCallsProperty); } set { SetValue(MaxConcurrentCallsProperty, value); } }
		public static readonly DependencyProperty MaxConcurrentCallsProperty =  DependencyProperty.Register("MaxConcurrentCalls", typeof(int), typeof(HostThrottlingBehavior));
		
		public int MaxConcurrentInstances { get { return (int)GetValue(MaxConcurrentInstancesProperty); } set { SetValue(MaxConcurrentInstancesProperty, value); } }
		public static readonly DependencyProperty MaxConcurrentInstancesProperty = DependencyProperty.Register("MaxConcurrentInstances", typeof(int), typeof(HostThrottlingBehavior));

		public int MaxConcurrentSessions { get { return (int)GetValue(MaxConcurrentSessionsProperty); } set { SetValue(MaxConcurrentSessionsProperty, value); } }
		public static readonly DependencyProperty MaxConcurrentSessionsProperty = DependencyProperty.Register("MaxConcurrentSessions", typeof(int), typeof(HostThrottlingBehavior));

		public HostThrottlingBehavior() { }

		public HostThrottlingBehavior(string Name, Host Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.Parent = Parent;

			this.MaxConcurrentCalls = 16;
			this.MaxConcurrentInstances = 26;
			this.MaxConcurrentSessions = 10;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (Parent != null)
				if (Parent.Parent != null)
					Parent.Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			return true;
		}

		public override string GenerateCode30()
		{
			return GenerateCode35();
		}

		public override string GenerateCode35()
		{
			return GenerateCode40();
		}

		public override string GenerateCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\t\tthis.{0} = new ServiceThrottlingBehavior();{1}", CodeName, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.MaxConcurrentCalls = {1};{2}", CodeName, MaxConcurrentCalls, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.MaxConcurrentInstances = {1};{2}", CodeName, MaxConcurrentInstances, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.{0}.MaxConcurrentSessions = {1};{2}", CodeName, MaxConcurrentSessions, Environment.NewLine);
			if (IsDefaultBehavior == true) Code.AppendFormat("\t\t\tthis.Description.Behaviors.Add({0});{1}", CodeName, Environment.NewLine);
			return Code.ToString();
		}
	}
}