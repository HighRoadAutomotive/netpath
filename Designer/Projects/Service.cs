using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WCFArchitect.Projects
{
	internal class Service : OpenableDocument
	{
		private Guid id;
		public Guid ID { get { return id; } }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Service));

		public string CodeName { get { return (string)GetValue(CodeNameProperty); } set { if (Globals.IsLoading == false) Globals.ReplaceDataType(CodeName, Parent.FullName, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @""), Parent.FullName); SetValue(CodeNameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty CodeNameProperty = DependencyProperty.Register("CodeName", typeof(string), typeof(Service));

		public string ContractName { get { return (string)GetValue(ContractNameProperty); } set { if (Globals.IsLoading == false) Globals.ReplaceDataType(ContractName, Parent.FullName, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @""), Parent.FullName); SetValue(ContractNameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty ContractNameProperty = DependencyProperty.Register("ContractName", typeof(string), typeof(Service));

		public ObservableCollection<Operation> Operations { get { return (ObservableCollection<Operation>)GetValue(OperationsProperty); } set { SetValue(OperationsProperty, value); } }
		public static readonly DependencyProperty OperationsProperty = DependencyProperty.Register("Operations", typeof(ObservableCollection<Operation>), typeof(Service));

		public ObservableCollection<Property> Properties { get { return (ObservableCollection<Property>)GetValue(PropertiesProperty); } set { SetValue(PropertiesProperty, value); } }
		public static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register("Properties", typeof(ObservableCollection<Property>), typeof(Service));

		public bool IsCallback { get { return (bool)GetValue(IsCallbackProperty); } set { SetValue(IsCallbackProperty, value); } }
		public static readonly DependencyProperty IsCallbackProperty = DependencyProperty.Register("IsCallback", typeof(bool), typeof(Service));

		public Service Callback { get { return (Service)GetValue(CallbackProperty); } set { SetValue(CallbackProperty, value); } }
		public static readonly DependencyProperty CallbackProperty = DependencyProperty.Register("Callback", typeof(Service), typeof(Service));

		public System.Net.Security.ProtectionLevel ProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(ProtectionLevelProperty); } set { SetValue(ProtectionLevelProperty, value); } }
		public static readonly DependencyProperty ProtectionLevelProperty = DependencyProperty.Register("ProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(Service));

		public System.ServiceModel.SessionMode SessionMode { get { return (System.ServiceModel.SessionMode)GetValue(SessionModeProperty); } set { SetValue(SessionModeProperty, value); } }
		public static readonly DependencyProperty SessionModeProperty = DependencyProperty.Register("SessionMode", typeof(System.ServiceModel.SessionMode), typeof(Service));

		public string ConfigurationName { get { return (string)GetValue(ConfigurationNameProperty); } set { SetValue(ConfigurationNameProperty, value); } }
		public static readonly DependencyProperty ConfigurationNameProperty = DependencyProperty.Register("ConfigurationName", typeof(string), typeof(Service));

		//Internal Use - Searching / Filtering
		public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(Service));

		public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(Service));

		public bool IsFiltering { get { return (bool)GetValue(IsFilteringProperty); } set { SetValue(IsFilteringProperty, value); } }
		public static readonly DependencyProperty IsFilteringProperty = DependencyProperty.Register("IsFiltering", typeof(bool), typeof(Service));

		public bool IsFilterMatch { get { return (bool)GetValue(IsFilterMatchProperty); } set { SetValue(IsFilterMatchProperty, value); } }
		public static readonly DependencyProperty IsFilterMatchProperty = DependencyProperty.Register("IsFilterMatch", typeof(bool), typeof(Service));

		public bool IsTreeExpanded { get { return (bool)GetValue(IsTreeExpandedProperty); } set { SetValue(IsTreeExpandedProperty, value); } }
		public static readonly DependencyProperty IsTreeExpandedProperty = DependencyProperty.Register("IsTreeExpanded", typeof(bool), typeof(Service));

		public Namespace Parent { get; set; }

		public Service() : base()
		{
			this.UserOpenedList = new List<UserOpened>();
		}

		public Service(string Name, Namespace Parent) : base()
		{
			this.IsOpen = false;
			this.IsCallback = false;
			this.UserOpenedList = new List<UserOpened>();
			this.Operations = new ObservableCollection<Operation>();
			this.Properties = new ObservableCollection<Property>();
			this.id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.ContractName = this.CodeName;
			this.ConfigurationName = "";
			this.Parent = Parent;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			
			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (e.Property == Service.IsSearchingProperty) return;
			if (e.Property == Service.IsSearchMatchProperty) return;
			if (e.Property == Service.IsFilteringProperty) return;
			if (e.Property == Service.IsFilterMatchProperty) return;
			if (e.Property == Service.IsTreeExpandedProperty) return;
		
			IsDirty = true;
		}
	
		public void Search(string Value)
		{
			if (Value != "")
			{
				foreach (Operation T in Operations)
				{
					T.IsSearching = true;
					T.IsSearchMatch = T.Name.Contains(Value);
					if (IsSearchMatch == false) IsSearchMatch = T.IsSearchMatch;
				}
				foreach (Property T in Properties)
				{
					T.IsSearching = true;
					T.IsSearchMatch = T.Name.Contains(Value);
					if (IsSearchMatch == false) IsSearchMatch = T.IsSearchMatch;
				}
			}
			else
			{
				foreach (Operation T in Operations)
				{
					T.IsSearching = false;
					T.IsSearchMatch = false;
				}
				foreach (Property T in Properties)
				{
					T.IsSearching = false;
					T.IsSearchMatch = false;
				}
			}
		}

		public void SearchOperations(string Value)
		{
			if (Value != "")
			{
				foreach (Operation T in Operations)
				{
					T.IsSearching = true;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					if (IsSearchMatch == false) IsSearchMatch = T.IsSearchMatch;
				}
			}
			else
			{
				foreach (Operation T in Operations)
				{
					T.IsSearching = false;
					T.IsSearchMatch = false;
				}
			}
		}

		public void SearchProperties(string Value)
		{
			if (Value != "")
			{
				foreach (Property T in Properties)
				{
					T.IsSearching = true;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					if (IsSearchMatch == false) IsSearchMatch = T.IsSearchMatch;
				}
			}
			else
			{
				foreach (Property T in Properties)
				{
					T.IsSearching = false;
					T.IsSearchMatch = false;
				}
			}
		}

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Service || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
							if (CodeName != null && CodeName != "") if (CodeName.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Code Name", CodeName, Parent.Owner, this));
							if (ContractName != null && ContractName != "") if (ContractName.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractName, Parent.Owner, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
							if (CodeName != null && CodeName != "") if (CodeName.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Code Name", CodeName, Parent.Owner, this));
							if (ContractName != null && ContractName != "") if (ContractName.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractName, Parent.Owner, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
						if (CodeName != null && CodeName != "") if (Args.RegexSearch.IsMatch(CodeName)) results.Add(new FindReplaceResult("Code Name", CodeName, Parent.Owner, this));
						if (ContractName != null && ContractName != "") if (Args.RegexSearch.IsMatch(ContractName)) results.Add(new FindReplaceResult("Contract Name", ContractName, Parent.Owner, this));
					}
				}

				if (Args.ReplaceAll == true)
				{
					bool ia = IsActive;
					IsActive = true;
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (CodeName != null && CodeName != "") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (ContractName != null && ContractName != "") ContractName = Microsoft.VisualBasic.Strings.Replace(ContractName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (CodeName != null && CodeName != "") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (ContractName != null && ContractName != "") ContractName = Microsoft.VisualBasic.Strings.Replace(ContractName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (CodeName != null && CodeName != "") CodeName = Args.RegexSearch.Replace(CodeName, Args.Replace);
							if (ContractName != null && ContractName != "") ContractName = Args.RegexSearch.Replace(ContractName, Args.Replace);
						}
					}
					IsActive = ia;
				}
			}

			foreach (Operation O in Operations)
				results.AddRange(O.FindReplace(Args));

			foreach (Property P in Properties)
				results.AddRange(P.FindReplace(Args));

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
		{
			if (Args.ReplaceAll == true)
			{
				bool ia = IsActive;
				IsActive = true;
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Code Name") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Contract Name") ContractName = Microsoft.VisualBasic.Strings.Replace(ContractName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Code Name") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Contract Name") ContractName = Microsoft.VisualBasic.Strings.Replace(ContractName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (Field == "Code Name") CodeName = Args.RegexSearch.Replace(CodeName, Args.Replace);
						if (Field == "Contract Name") ContractName = Args.RegexSearch.Replace(ContractName, Args.Replace);
					}
				}
				IsActive = ia;
			}
		}
		
		public bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (CodeName == "" || CodeName == null)
			{
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2000", "An service in the '" + Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchCodeName.IsMatch(CodeName) == false)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2001", "The service '" + Name + "' in the '" + Parent.Name + "' namespace contains invalid characters in the Code Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
					NoErrors = false;
				}
			if (ContractName == "" || ContractName == null) { }
			else
				if (Helpers.RegExs.MatchCodeName.IsMatch(ContractName) == false)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2002", "The service '" + Name + "' in the '" + Parent.Name + "' namespace contains invalid characters in the Contract Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
					NoErrors = false;
				}

			foreach (Operation O in Operations)
			{
				if (O.CodeName == "" || O.CodeName == null)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2004", "An operation in the '" + Name + "' service has a blank Code Name. A Code Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, O, O.GetType()));
					NoErrors = false;
				}
				else
					if (Helpers.RegExs.MatchCodeName.IsMatch(O.CodeName) == false)
					{
						Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2005", "The operation '" + O.CodeName + "' in the '" + Name + "' service contains invalid characters in the Code Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, O, O.GetType()));
						NoErrors = false;
					}
				if (O.ContractName == "" || O.ContractName == null) { }
				else
					if (Helpers.RegExs.MatchCodeName.IsMatch(O.ContractName) == false)
					{
						Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2006", "The operation '" + O.CodeName + "' in the '" + Name + "' service contains invalid characters in the Contract Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, O, O.GetType()));
						NoErrors = false;
					}
				if (O.ReturnType == "" || O.ReturnType == null)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2007", "The operation '" + O.CodeName + "' in the '" + Name + "' service has a blank Return Type. A Return Type MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, O, O.GetType()));
					NoErrors = false;
				}

				foreach (OperationParameter OP in O.Parameters)
				{
					if (OP.Name == "" || OP.Name == null)
					{
						Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2008", "The operation '" + O.Name + "' in the '" + Name + "' service has a parameter with a blank name. A Parameter Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, O, O.GetType()));
						NoErrors = false;
					}
					if (OP.DataType == "" || OP.DataType == null)
					{
						Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2009", "The operation '" + O.Name + "' in the '" + Name + "' service has a blank Data Type. A Data Type MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, O, O.GetType()));
						NoErrors = false;
					}
				}
			}

			foreach (Property P in Properties)
			{
				if (P.CodeName == "" || P.CodeName == null)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2010", "A property in the '" + Name + "' service has a blank Code Name. A Code Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, P, P.GetType()));
					NoErrors = false;
				}
				else
					if (Helpers.RegExs.MatchCodeName.IsMatch(P.CodeName) == false)
					{
						Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS2011", "The Property '" + P.CodeName + "' in the '" + Name + "' service contains invalid characters in the Code Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, P, P.GetType()));
						NoErrors = false;
					}
			}

			return NoErrors;
		}

		public string GenerateServerCode30(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public string GenerateServerCode35(string ProjectName)
		{
			return GenerateServerCode40(ProjectName);
		}

		public string GenerateServerCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			if (IsCallback == false)
			{
				Code.Append("\t[ServiceContract(");
				if (Callback != null)
					Code.AppendFormat("CallbackContract = typeof(I{0}), ", Callback.CodeName);
				if(ConfigurationName != "" && ConfigurationName != null)
					Code.AppendFormat("ConfigurationName = \"{0}\", ", ConfigurationName);
				if (ProtectionLevel != System.Net.Security.ProtectionLevel.None)
					Code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), ProtectionLevel));
				Code.AppendFormat("SessionMode = System.ServiceModel.SessionMode.{0}, ", System.Enum.GetName(typeof(System.ServiceModel.SessionMode), SessionMode));
				if (ContractName != "" && ContractName != null) Code.AppendFormat("Name = \"{0}\", ", ContractName);
				Code.AppendFormat("Namespace = \"{0}\"", Parent.URI);
				Code.AppendLine(")]");
			}
			Code.AppendFormat("\t{0} interface I{1}{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			foreach (Property P in Properties)
				Code.Append(P.GenerateServerCode40());
			foreach (Operation OP in Operations)
				Code.Append(OP.GenerateCode40());
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public string GenerateClientCode30(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public string GenerateClientCode35(string ProjectName)
		{
			return GenerateClientCode40(ProjectName);
		}

		public string GenerateClientCode40(string ProjectName)
		{
			if (IsCallback == true) return "";
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("\t{0} partial class {1}Client{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			foreach (Host H in Globals.ProjectSpace.OfType<Host>())
				if (H.Service == this)
					Code.Append(H.GenerateClientCode40());
			foreach (Property P in Properties)
				Code.AppendLine(P.GenerateClientCode());
			Code.AppendLine("\t}");

			return Code.ToString();
		}
	}

	internal class Operation : DependencyObject
	{
		private Guid id;
		public Guid ID { get { return id; } }

		public bool UseAsyncPattern { get { return (bool)GetValue(UseAsyncPatternProperty); } set { SetValue(UseAsyncPatternProperty, value); } }
		public static readonly DependencyProperty UseAsyncPatternProperty = DependencyProperty.Register("UseAsyncPattern", typeof(bool), typeof(Operation));

		public bool IsInitiating { get { return (bool)GetValue(IsInitiatingProperty); } set { SetValue(IsInitiatingProperty, value); } }
		public static readonly DependencyProperty IsInitiatingProperty = DependencyProperty.Register("IsInitiating", typeof(bool), typeof(Operation));

		public bool IsTerminating { get { return (bool)GetValue(IsTerminatingProperty); } set { SetValue(IsTerminatingProperty, value); } }
		public static readonly DependencyProperty IsTerminatingProperty = DependencyProperty.Register("IsTerminating", typeof(bool), typeof(Operation));

		public bool IsOneWay { get { return (bool)GetValue(IsOneWayProperty); } set { SetValue(IsOneWayProperty, value); } }
		public static readonly DependencyProperty IsOneWayProperty = DependencyProperty.Register("IsOneWay", typeof(bool), typeof(Operation));

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Operation));

		public string CodeName { get { return (string)GetValue(CodeNameProperty); } set { SetValue(CodeNameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty CodeNameProperty = DependencyProperty.Register("CodeName", typeof(string), typeof(Operation));

		public string ContractName { get { return (string)GetValue(ContractNameProperty); } set { SetValue(ContractNameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty ContractNameProperty = DependencyProperty.Register("ContractName", typeof(string), typeof(Operation));

		public System.Net.Security.ProtectionLevel ProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(ProtectionLevelProperty); } set { SetValue(ProtectionLevelProperty, value); } }
		public static readonly DependencyProperty ProtectionLevelProperty = DependencyProperty.Register("ProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(Operation));

		public ObservableCollection<OperationParameter> Parameters { get { return (ObservableCollection<OperationParameter>)GetValue(ParametersProperty); } set { SetValue(ParametersProperty, value); } }
		public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(ObservableCollection<OperationParameter>), typeof(Operation));

		public string ReturnType { get { return (string)GetValue(ReturnTypeProperty); } set { SetValue(ReturnTypeProperty, value); } }
		public static readonly DependencyProperty ReturnTypeProperty = DependencyProperty.Register("ReturnType", typeof(string), typeof(Operation));

		//Internal Use - Searching / Filtering
		public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(Operation));

		public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(Operation));

		public bool IsFiltering { get { return false; } set { } }
		public bool IsFilterMatch { get { return false; } set { } }
		public bool IsTreeExpanded { get { return false; } set { } }
		public Service Owner { get; set; }

		public Operation() { }

		public Operation(string Name, Service Owner)
		{
			this.Parameters = new ObservableCollection<OperationParameter>();
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.ReturnType = "void";
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.ContractName = this.CodeName;
			this.Owner = Owner;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
	
			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (e.Property == Operation.IsSearchingProperty) return;
			if (e.Property == Operation.IsSearchMatchProperty) return;
			
			if (Owner != null)
				Owner.IsDirty = true;
		}

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Service || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
							if (CodeName != null && CodeName != "") if (CodeName.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Code Name", CodeName, Owner.Parent.Owner, this));
							if (ContractName != null && ContractName != "") if (ContractName.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractName, Owner.Parent.Owner, this)); ;
						}
						if (ReturnType != null && ReturnType != "") if (Name.IndexOf(ReturnType, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Return Type", ReturnType, Owner.Parent.Owner, this));
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
							if (CodeName != null && CodeName != "") if (CodeName.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Code Name", CodeName, Owner.Parent.Owner, this));
							if (ContractName != null && ContractName != "") if (ContractName.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractName, Owner.Parent.Owner, this));
						}
						if (ReturnType != null && ReturnType != "") if (Name.IndexOf(ReturnType) >= 0) results.Add(new FindReplaceResult("Return Type", ReturnType, Owner.Parent.Owner, this));
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
						if (CodeName != null && CodeName != "") if (Args.RegexSearch.IsMatch(CodeName)) results.Add(new FindReplaceResult("Code Name", CodeName, Owner.Parent.Owner, this));
						if (ContractName != null && ContractName != "") if (Args.RegexSearch.IsMatch(ContractName)) results.Add(new FindReplaceResult("Contract Name", ContractName, Owner.Parent.Owner, this)); ;
					}
					if (ReturnType != null && ReturnType != "") if (Args.RegexSearch.IsMatch(ReturnType)) results.Add(new FindReplaceResult("Return Type", ReturnType, Owner.Parent.Owner, this));
				}

				if (Args.ReplaceAll == true)
				{
					bool ia = Owner.IsActive;
					Owner.IsActive = true;
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (CodeName != null && CodeName != "") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (ContractName != null && ContractName != "") ContractName = Microsoft.VisualBasic.Strings.Replace(ContractName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
							if (ReturnType != null && ReturnType != "") ReturnType = Microsoft.VisualBasic.Strings.Replace(ReturnType, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (CodeName != null && CodeName != "") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (ContractName != null && ContractName != "") ContractName = Microsoft.VisualBasic.Strings.Replace(ContractName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary); ;
							}
							if (ReturnType != null && ReturnType != "") ReturnType = Microsoft.VisualBasic.Strings.Replace(ReturnType, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (CodeName != null && CodeName != "") CodeName = Args.RegexSearch.Replace(CodeName, Args.Replace);
							if (ContractName != null && ContractName != "") ContractName = Args.RegexSearch.Replace(ContractName, Args.Replace);
						}
						if (ReturnType != null && ReturnType != "") ReturnType = Args.RegexSearch.Replace(ReturnType, Args.Replace);
					}
					Owner.IsActive = ia;
				}
			}

			foreach (OperationParameter OP in Parameters)
				results.AddRange(OP.FindReplace(Args));

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
		{
			if (Args.ReplaceAll == true)
			{
				bool ia = Owner.IsActive;
				Owner.IsActive = true;
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Code Name") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Contract Name") ContractName = Microsoft.VisualBasic.Strings.Replace(ContractName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						if (Field == "Return Type") ReturnType = Microsoft.VisualBasic.Strings.Replace(ReturnType, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Code Name") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Contract Name") ContractName = Microsoft.VisualBasic.Strings.Replace(ContractName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
						if (Field == "Return Type") ReturnType = Microsoft.VisualBasic.Strings.Replace(ReturnType, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (Field == "Code Name") CodeName = Args.RegexSearch.Replace(CodeName, Args.Replace);
						if (Field == "Contract Name") ContractName = Args.RegexSearch.Replace(ContractName, Args.Replace);
					}
					if (Field == "Return Type") ReturnType = Args.RegexSearch.Replace(ReturnType, Args.Replace);
				}
				Owner.IsActive = ia;
			}
		}

		public string GenerateCode30()
		{
			return GenerateCode35();
		}

		public string GenerateCode35()
		{
			return GenerateCode40();
		}

		public string GenerateCode40()
		{
			StringBuilder Code = new StringBuilder();

			Code.Append("\t\t[OperationContract(");
			if (UseAsyncPattern == true)
				Code.Append("AsyncPattern = true, ");
			if (IsInitiating == true && IsTerminating == false)
				Code.Append("IsInitiating = true, ");
			if (IsInitiating == false && IsTerminating == true)
				Code.Append("IsTerminating = true, ");
			if (IsOneWay == true)
				Code.Append("IsOneWay = true, ");
			if (ProtectionLevel != System.Net.Security.ProtectionLevel.None)
				Code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), ProtectionLevel));
			if (ContractName != "" && ContractName != null)
				Code.AppendFormat("Name = \"{0}\", ", ContractName);
			if (Code.Length > 21) Code.Remove(Code.Length - 2, 2);
			Code.AppendFormat(")] {0} {1}(", ReturnType, CodeName);
			foreach (OperationParameter OP in Parameters)
				Code.AppendFormat("{0},", OP.GenerateCode());
			if (Parameters.Count > 0) Code.Remove(Code.Length - 1, 1);
			Code.AppendLine(");");
			
			return Code.ToString();
		}
	}

	internal class Property : DependencyObject
	{
		private Guid id;
		public Guid ID { get { return id; } }

		public bool IsReadOnly { get { return (bool)GetValue(IsReadOnlyProperty); } set { SetValue(IsReadOnlyProperty, value); } }
		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(Property));

		public bool IsOneWay { get { return (bool)GetValue(IsOneWayProperty); } set { SetValue(IsOneWayProperty, value); } }
		public static readonly DependencyProperty IsOneWayProperty = DependencyProperty.Register("IsOneWay", typeof(bool), typeof(Property));

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Property));

		public string CodeName { get { return (string)GetValue(CodeNameProperty); } set { SetValue(CodeNameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty CodeNameProperty = DependencyProperty.Register("CodeName", typeof(string), typeof(Property));

		public System.Net.Security.ProtectionLevel ProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(ProtectionLevelProperty); } set { SetValue(ProtectionLevelProperty, value); } }
		public static readonly DependencyProperty ProtectionLevelProperty = DependencyProperty.Register("ProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(Property));

		public string ReturnType { get { return (string)GetValue(ReturnTypeProperty); } set { SetValue(ReturnTypeProperty, value); } }
		public static readonly DependencyProperty ReturnTypeProperty = DependencyProperty.Register("ReturnType", typeof(string), typeof(Property));

		//Internal Use - Searching / Filtering
		public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(Property));

		public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(Property));

		public bool IsFiltering { get { return false; } set { } }
		public bool IsFilterMatch { get { return false; } set { } }
		public bool IsTreeExpanded { get { return false; } set { } }
		public Service Owner { get; set; }

		public Property() { }

		public Property(string Name, Service Owner)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.ReturnType = "string";
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.Owner = Owner;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (e.Property == Property.IsSearchingProperty) return;
			if (e.Property == Property.IsSearchMatchProperty) return;

			if (Owner != null)
				Owner.IsDirty = true;
		}

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Service || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) > 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
							if (CodeName != null && CodeName != "") if (CodeName.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) > 0) results.Add(new FindReplaceResult("Code Name", CodeName, Owner.Parent.Owner, this));
						}
						if (ReturnType != null && ReturnType != "") if (ReturnType.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) > 0) results.Add(new FindReplaceResult("Return Type", ReturnType, Owner.Parent.Owner, this));
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) > 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
							if (CodeName != null && CodeName != "") if (CodeName.IndexOf(Args.Search) > 0) results.Add(new FindReplaceResult("Code Name", CodeName, Owner.Parent.Owner, this));
						}
						if (ReturnType != null && ReturnType != "") if (ReturnType.IndexOf(Args.Search) > 0) results.Add(new FindReplaceResult("Return Type", ReturnType, Owner.Parent.Owner, this));
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
						if (CodeName != null && CodeName != "") if (Args.RegexSearch.IsMatch(CodeName)) results.Add(new FindReplaceResult("Code Name", CodeName, Owner.Parent.Owner, this));
					}
					if (ReturnType != null && ReturnType != "") if (Args.RegexSearch.IsMatch(ReturnType)) results.Add(new FindReplaceResult("Return Type", ReturnType, Owner.Parent.Owner, this));
				}

				if (Args.ReplaceAll == true)
				{
					bool ia = Owner.IsActive;
					Owner.IsActive = true;
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (CodeName != null && CodeName != "") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
							if (ReturnType != null && ReturnType != "") ReturnType = Microsoft.VisualBasic.Strings.Replace(ReturnType, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (CodeName != null && CodeName != "") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
							if (ReturnType != null && ReturnType != "") ReturnType = Microsoft.VisualBasic.Strings.Replace(ReturnType, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (CodeName != null && CodeName != "") CodeName = Args.RegexSearch.Replace(CodeName, Args.Replace);
						}
						if (ReturnType != null && ReturnType != "") ReturnType = Args.RegexSearch.Replace(ReturnType, Args.Replace);
					}
					Owner.IsActive = ia;
				}
			}

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
		{
			if (Args.ReplaceAll == true)
			{
				bool ia = Owner.IsActive;
				Owner.IsActive = true;
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Code Name") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						if (Field == "Return Type") ReturnType = Microsoft.VisualBasic.Strings.Replace(ReturnType, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Code Name") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
						if (Field == "Return Type") ReturnType = Microsoft.VisualBasic.Strings.Replace(ReturnType, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (Field == "Code Name") CodeName = Args.RegexSearch.Replace(CodeName, Args.Replace);
					}
					if (Field == "Return Type") ReturnType = Args.RegexSearch.Replace(ReturnType, Args.Replace);
				}
				Owner.IsActive = ia;
			}
		}

		public string GenerateServerCode30()
		{
			return GenerateServerCode35();
		}

		public string GenerateServerCode35()
		{
			return GenerateServerCode40();
		}

		public string GenerateServerCode40()
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t\t{0} {1} {{ ", ReturnType, CodeName);
			if (IsReadOnly == false)
			{
				//Write Getter Code
				Code.Append("[OperationContract(");
				if (IsOneWay == true)
					Code.Append("IsOneWay = true, ");
				if (ProtectionLevel != System.Net.Security.ProtectionLevel.None)
					Code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), ProtectionLevel));
				Code.AppendFormat("Name = \"Get{0}\"", CodeName);
				Code.Append(")] get; ");

				//Write Setter Code
				Code.Append("[OperationContract(");
				if (IsOneWay == true)
					Code.Append("IsOneWay = true, ");
				if (ProtectionLevel != System.Net.Security.ProtectionLevel.None)
					Code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), ProtectionLevel));
				Code.AppendFormat("Name = \"Set{0}\"", CodeName);
				Code.Append(")] set;");
			}
			else
			{
				//Write Getter Code
				Code.Append("[OperationContract(");
				if (IsOneWay == true)
					Code.Append("IsOneWay = true, ");
				if (ProtectionLevel != System.Net.Security.ProtectionLevel.None)
					Code.AppendFormat("ProtectionLevel = System.Net.Security.ProtectionLevel.{0}, ", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), ProtectionLevel));
				Code.AppendFormat("Name = \"Get{0}\"", CodeName);
				Code.Append(")] get; ");
			}
			Code.AppendLine(" }");

			return Code.ToString();
		}

		public string GenerateClientCode()
		{
			if (IsReadOnly == false)
			{
				return string.Format("\t\tpublic {0} {1} {{ get {{ return Get{1}(); }} set {{ Set{1}(value); }} }}", ReturnType, CodeName);
			}
			else
			{
				return string.Format("\t\tpublic {0} {1} {{ get {{ return Get{1}(); }} }}", ReturnType, CodeName);
			}
		}
	}

	internal class OperationParameter : DependencyObject
	{
		private Guid id;
		public Guid ID { get { return id; } }

		public string DataType { get { return (string)GetValue(DataTypeProperty); } set { SetValue(DataTypeProperty, value); } }
		public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType", typeof(string), typeof(OperationParameter));

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(OperationParameter));

		public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } } 
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(OperationParameter));

		//Internal Use
		public Service Owner { get; set; }

		public bool IsValidMemberType(string DataType) { if (DataType == "" || DataType == null) { return true; } return DataType.Count(a => a == '[' || a == ']') == 0 && DataType.Count(a => a == '<' || a == '>') == 0 && DataType.Count(a => a == ',') == 0; }
		public bool IsValidArrayType(string DataType) { if (DataType == "" || DataType == null) { return true; } return DataType.Count(a => a == '[' || a == ']') == 2 && DataType.Count(a => a == '<' || a == '>') == 0 && DataType.Count(a => a == ',') == 0; }
		public bool IsValidCollectionType(string DataType) { if (DataType == "" || DataType == null) { return true; } return DataType.Count(a => a == '[' || a == ']') == 0 && DataType.Count(a => a == '<' || a == '>') == 2 && DataType.Count(a => a == ',') == 0; }
		public bool IsValidDictionaryType(string DataType) { if (DataType == "" || DataType == null) { return true; } return DataType.Count(a => a == '[' || a == ']') == 0 && DataType.Count(a => a == '<' || a == '>') == 2 && DataType.Count(a => a == ',') == 1; } 

		public OperationParameter()
		{
			this.id = Guid.NewGuid();
			this.DataType = "string";
			IsHidden = false;
		}

		public OperationParameter(string DataType, string Name, Service Owner)
		{
			this.id = Guid.NewGuid();
			this.DataType = DataType;
			this.Name = Name;
			IsHidden = false;
			this.Owner = Owner;
		}

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Service || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
						}
						if (DataType != null && DataType != "") if (DataType.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Data Type", DataType, Owner.Parent.Owner, this));
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
						}
						if (DataType != null && DataType != "") if (DataType.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Data Type", DataType, Owner.Parent.Owner, this));
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
					}
					if (DataType != null && DataType != "") if (Args.RegexSearch.IsMatch(DataType)) results.Add(new FindReplaceResult("Data Type", DataType, Owner.Parent.Owner, this));
				}

				if (Args.ReplaceAll == true)
				{
					bool ia = Owner.IsActive;
					Owner.IsActive = true;
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
							if (DataType != null && DataType != "") DataType = Microsoft.VisualBasic.Strings.Replace(DataType, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
							if (DataType != null && DataType != "") DataType = Microsoft.VisualBasic.Strings.Replace(DataType, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace); 
						}
						if (DataType != null && DataType != "") DataType = Args.RegexSearch.Replace(DataType, Args.Replace);
					}
					Owner.IsActive = ia;
				}
			}

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
		{
			if (Args.ReplaceAll == true)
			{
				bool ia = Owner.IsActive;
				Owner.IsActive = true;
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						if (Field == "Data Type") DataType = Microsoft.VisualBasic.Strings.Replace(DataType, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
						if (Field == "Data Type") DataType = Microsoft.VisualBasic.Strings.Replace(DataType, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
					}
					if (Field == "Data Type") DataType = Args.RegexSearch.Replace(DataType, Args.Replace);
				}
				Owner.IsActive = ia;
			}
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (Owner != null)
				Owner.IsDirty = true;
		}

		public string GenerateCode()
		{
			if (IsHidden == true) return "";
			return DataType + " " + Name;
		}
	}
}