using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WCFArchitect.Projects
{
	internal enum DataScope
	{
		Public,
		Protected,
		Private,
		Internal,
		ProtectedInternal,
	}

	internal class Data : OpenableDocument
	{
		private Guid id;
		public Guid ID { get { return id; } }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Data));

		public string CodeName { get { return (string)GetValue(CodeNameProperty); } set { if (Globals.IsLoading == false) Globals.ReplaceDataType(CodeName, Parent.FullName, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @""), Parent.FullName); SetValue(CodeNameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty CodeNameProperty = DependencyProperty.Register("CodeName", typeof(string), typeof(Data));

		public string ContractName { get { return (string)GetValue(ContractNameProperty); } set { if (Globals.IsLoading == false) Globals.ReplaceDataType(ContractName, Parent.FullName, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @""), Parent.FullName); SetValue(ContractNameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty ContractNameProperty = DependencyProperty.Register("ContractName", typeof(string), typeof(Data));

		public string WPFName { get { return (string)GetValue(WPFNameProperty); } set { if (Globals.IsLoading == false) Globals.ReplaceDataType(WPFName, Parent.FullName, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @""), Parent.FullName); SetValue(WPFNameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty WPFNameProperty = DependencyProperty.Register("WPFName", typeof(string), typeof(Data));

		public bool IsReference { get { return (bool)GetValue(IsReferenceProperty); } set { SetValue(IsReferenceProperty, value); } }
		public static readonly DependencyProperty IsReferenceProperty = DependencyProperty.Register("IsReference", typeof(bool), typeof(Data));

		public ObservableCollection<DataElement> Elements { get { return (ObservableCollection<DataElement>)GetValue(ElementsProperty); } set { SetValue(ElementsProperty, value); } }
		public static readonly DependencyProperty ElementsProperty = DependencyProperty.Register("Elements", typeof(ObservableCollection<DataElement>), typeof(Data));

		//Internal Use - Searching / Filtering
		public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(Data));

		public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(Data));

		public bool IsFiltering { get { return (bool)GetValue(IsFilteringProperty); } set { SetValue(IsFilteringProperty, value); } }
		public static readonly DependencyProperty IsFilteringProperty = DependencyProperty.Register("IsFiltering", typeof(bool), typeof(Data));

		public bool IsFilterMatch { get { return (bool)GetValue(IsFilterMatchProperty); } set { SetValue(IsFilterMatchProperty, value); } }
		public static readonly DependencyProperty IsFilterMatchProperty = DependencyProperty.Register("IsFilterMatch", typeof(bool), typeof(Data));

		public bool IsTreeExpanded { get { return (bool)GetValue(IsTreeExpandedProperty); } set { SetValue(IsTreeExpandedProperty, value); } }
		public static readonly DependencyProperty IsTreeExpandedProperty = DependencyProperty.Register("IsTreeExpanded", typeof(bool), typeof(Data));

		public Namespace Parent { get; set; }

		public Data() : base()
		{
			this.UserOpenedList = new List<UserOpened>();
		}

		public Data(string Name, Namespace Parent) : base()
		{
			this.UserOpenedList = new List<UserOpened>();
			this.Elements = new ObservableCollection<DataElement>();
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.CodeName = Helpers.RegExs.ReplaceSpaces.Replace(Name, @"");
			this.WPFName = CodeName + "WPF";
			this.ContractName = this.CodeName;
			this.Parent = Parent;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (e.Property == Data.IsSearchingProperty) return;
			if (e.Property == Data.IsSearchMatchProperty) return;
			if (e.Property == Data.IsFilteringProperty) return;
			if (e.Property == Data.IsFilterMatchProperty) return;
			if (e.Property == Data.IsTreeExpandedProperty) return;

			IsDirty = true;
		}

		public void Search(string Value)
		{
			if (Value != "")
			{
				foreach (DataElement T in Elements)
				{
					T.IsSearching = true;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					if (IsSearchMatch == false) IsSearchMatch = T.IsSearchMatch;
				}
			}
			else
			{
				foreach (DataElement T in Elements)
				{
					T.IsSearching = false;
					T.IsSearchMatch = false;
				}
			}
		}

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Data || Args.Items == FindItems.Any)
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
							if (WPFName != null && WPFName != "") if (WPFName.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("WPF Name", WPFName, Parent.Owner, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
							if (CodeName != null && CodeName != "") if (CodeName.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Code Name", CodeName, Parent.Owner, this));
							if (ContractName != null && ContractName != "") if (ContractName.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractName, Parent.Owner, this));
							if (WPFName != null && WPFName != "") if (WPFName.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("WPF Name", WPFName, Parent.Owner, this));
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
						if (WPFName != null && WPFName != "") if (Args.RegexSearch.IsMatch(WPFName)) results.Add(new FindReplaceResult("WPF Name", WPFName, Parent.Owner, this));
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
								if (WPFName != null && WPFName != "") WPFName = Microsoft.VisualBasic.Strings.Replace(WPFName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (CodeName != null && CodeName != "") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (ContractName != null && ContractName != "") ContractName = Microsoft.VisualBasic.Strings.Replace(ContractName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (WPFName != null && WPFName != "") WPFName = Microsoft.VisualBasic.Strings.Replace(WPFName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
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
							if (WPFName != null && WPFName != "") WPFName = Args.RegexSearch.Replace(WPFName, Args.Replace);
						}
					}
					IsActive = ia;
				}
			}

			foreach (DataElement DE in Elements)
				results.AddRange(DE.FindReplace(Args));

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
							if (Field == "WPF Name") WPFName = Microsoft.VisualBasic.Strings.Replace(WPFName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Code Name") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Contract Name") ContractName = Microsoft.VisualBasic.Strings.Replace(ContractName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "WPF Name") WPFName = Microsoft.VisualBasic.Strings.Replace(WPFName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
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
						if (Field == "WPF Name") WPFName = Args.RegexSearch.Replace(WPFName, Args.Replace);
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
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3000", "The data object '" + Name + "' in the '" + Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchCodeName.IsMatch(CodeName) == false)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3001", "The data object '" + Name + "' in the '" + Parent.Name + "' namespace contains invalid characters in the Contract Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
					NoErrors = false;
				}

			if(ContractName != "" && ContractName != null)
				if (Helpers.RegExs.MatchCodeName.IsMatch(ContractName) == false)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3002", "The data object '" + Name + "' in the '" + Parent.Name + "' namespace contains invalid characters in the Contract Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
					NoErrors = false;
				}

			foreach (DataElement D in Elements)
			{
				if (D.Name == "" || D.Name == null)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3003", "The data object '" + Name + "' in the '" + Parent.Name + "' namespace has a data element with a blank Name. A Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, D, D.GetType()));
					NoErrors = false;
				}
				else
					if (Helpers.RegExs.MatchCodeName.IsMatch(D.Name) == false)
					{
						Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3004", "The data element '" + D.Name + "' in the '" + Name + "' data object contains invalid characters in the Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, D, D.GetType()));
						NoErrors = false;
					}
				if (D.DataType == "" || D.DataType == null)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3004", "The data object '" + Name + "' in the '" + Parent.Name + "' namespace has a data element with a blank Data Type. A Data Type MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, D, D.GetType()));
					NoErrors = false;
				}
				else
				{
					if (D.DataType.IndexOf("[") >= 0 && D.IsValidArrayType(D.DataType) == false)
					{
						Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3005", "The data element '" + D.Name + "' in the '" + Name + "' data object contains an invalid array type.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, D, D.GetType()));
						NoErrors = false;
					}
					if (D.DataType.IndexOf("<") >= 0 && D.DataType.IndexOf(",") < 0 && D.IsValidCollectionType(D.DataType) == false)
					{
						Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3006", "The data element '" + D.Name + "' in the '" + Name + "' data object contains an invalid collection type.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, D, D.GetType()));
						NoErrors = false;
					}
					if (D.DataType.IndexOf(",") >= 0 && D.IsValidDictionaryType(D.DataType) == false)
					{
						Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3007", "The data element '" + D.Name + "' in the '" + Name + "' data object contains an invalid dictionary type.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, D, D.GetType()));
						NoErrors = false;
					}
				}
				if (D.WPFDataType != "" && D.WPFDataType != null)
				{
					if (D.IsArray == true && D.IsValidArrayType(D.WPFDataType) == false)
					{
						Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3008", "The data element '" + D.Name + "' in the '" + Name + "' data object contains an invalid WPF array type.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, D, D.GetType()));
						NoErrors = false;
					}
					if (D.IsCollection == true && D.IsValidCollectionType(D.WPFDataType) == false)
					{
						Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3009", "The data element '" + D.Name + "' in the '" + Name + "' data object contains an invalid WPF collection type.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, D, D.GetType()));
						NoErrors = false;
					}
					if (D.IsDictionary == true && D.IsValidDictionaryType(D.WPFDataType) == false)
					{
						Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3010", "The data element '" + D.Name + "' in the '" + Name + "' data object contains an invalid WPF dictionary type.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, D, D.GetType()));
						NoErrors = false;
					}
				}

				//Make sure the service collection type is set.
				D.ServiceCollectionType = Parent.Owner.ServiceCollectionType;
			}

			return NoErrors;
		}

		public string GenerateServerCode30(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.Append("\t[DataContract(");
			if (ContractName != "" && ContractName != null) Code.AppendFormat("Name = \"{0}\", ", ContractName);
			Code.AppendFormat("Namespace = \"{0}\"", Parent.URI);
			Code.AppendLine(")]");
			Code.AppendFormat("\t{0} partial class {1}{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			foreach (DataElement DE in Elements)
				Code.Append(DE.GenerateServerCode30());
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public string GenerateServerCode35(string ProjectName)
		{
			return GenerateServerCode40(ProjectName);
		}

		public string GenerateServerCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.Append("\t[DataContract(");
			if (IsReference == true) Code.AppendFormat("IsReference = true ");
			if (ContractName != "" && ContractName != null)  Code.AppendFormat("Name = \"{0}\", ", ContractName);
			Code.AppendFormat("Namespace = \"{0}\"", Parent.URI);
			Code.AppendLine(")]");
			Code.AppendFormat("\t{0} partial class {1}{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			foreach (DataElement DE in Elements)
				Code.Append(DE.GenerateServerCode40());
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public string GenerateClientCode30(string ProjectName)
		{
			bool HasWPF = false;
			foreach (DataElement DE in Elements)
				if (DE.GenerateWPFBinding == true)
				{
					HasWPF = true;
					break;
				}
			if (HasWPF == false) return "";

			//This is a shim to ensure there is ALWAYS a WPF name.
			if (WPFName == "" || WPFName == null) WPFName = CodeName + "WPF";

			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t//WPF Integration Object for the {0} DTO{1}", ContractName, Environment.NewLine);
			Code.AppendFormat("\t{0} partial class {1} : DependencyObject{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", WPFName, Environment.NewLine);
			Code.AppendLine("\t{");

			Code.AppendLine("\t\t//Properties");
			foreach (DataElement DE in Elements)
				Code.Append(DE.GenerateClientCode30(WPFName));
			Code.AppendLine();

			Code.AppendLine("\t\t//Implicit Conversion");
			Code.AppendFormat("\t\tpublic static implicit operator {0}({1} Data){2}", ContractName, WPFName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tif (Data == null) return null;");
			Code.AppendLine("\t\t\tif(Application.Current.Dispatcher.CheckAccess() == true)");
			Code.AppendLine("\t\t\t{");
			Code.AppendFormat("\t\t\t\treturn {0}.ConvertFromWPFObject(Data);{1}", WPFName, Environment.NewLine);
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t\telse");
			Code.AppendLine("\t\t\t{");
			Code.AppendFormat("\t\t\t\treturn ({0})Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Windows.Threading.DispatcherOperationCallback(delegate(object ignore){{ return {1}.ConvertFromWPFObject(Data); }}), null);{2}", ContractName, WPFName, Environment.NewLine);
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic static implicit operator {1}({0} Data){2}", ContractName, WPFName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tif (Data == null) return null;");
			Code.AppendLine("\t\t\tif(Application.Current.Dispatcher.CheckAccess() == true)");
			Code.AppendLine("\t\t\t{");
			Code.AppendFormat("\t\t\t\treturn {0}.ConvertToWPFObject(Data);{1}", WPFName, Environment.NewLine);
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t\telse");
			Code.AppendLine("\t\t\t{");
			Code.AppendFormat("\t\t\t\treturn ({0})Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Windows.Threading.DispatcherOperationCallback(delegate(object ignore){{ return {0}.ConvertToWPFObject(Data); }}), null);{1}", WPFName, Environment.NewLine);
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");
			Code.AppendLine();

			Code.AppendLine("\t\t//Constructors");
			Code.AppendFormat("\t\tpublic {1}({0}.{2} Data){3}", Parent.FullName, WPFName, ContractName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tType t_DT = Data.GetType();");
			foreach (DataElement DE in Elements)
				Code.Append(DE.GenerateClientConstructorCode30(WPFName));
			Code.AppendLine("\t\t}");
			Code.AppendLine();
			Code.AppendFormat("\t\tpublic {0}(){1}", WPFName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t}");
			Code.AppendLine();

			Code.AppendLine("\t\t//WPF/DTO Conversion Functions");
			Code.AppendFormat("\t\tpublic static {0} ConvertFromWPFObject({1} Data){2}", ContractName, WPFName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\t{0} DTO = new {0}();{1}", ContractName, Environment.NewLine);
			Code.AppendLine("\t\t\tType t_WPF = Data.GetType();");
			Code.AppendLine("\t\t\tType t_DTO = DTO.GetType();");
			foreach (DataElement DE in Elements)
				Code.Append(DE.GenerateClientConversionFromWPF30(WPFName));
			Code.AppendLine("\t\t\treturn DTO;"); 
			Code.AppendLine("\t\t}");
			Code.AppendLine();
			Code.AppendFormat("\t\tpublic static {0} ConvertToWPFObject({1} Data){2}", WPFName, ContractName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\t{0} WPF = new {0}();{1}", WPFName, Environment.NewLine);
			Code.AppendLine("\t\t\tType t_DTO = Data.GetType();");
			Code.AppendLine("\t\t\tType t_WPF = WPF.GetType();");
			foreach (DataElement DE in Elements)
				Code.Append(DE.GenerateClientConversionToWPF30(WPFName));
			Code.AppendLine("\t\t\treturn WPF;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");

			return Code.ToString();
		}

		public string GenerateClientCode35(string ProjectName)
		{
			return GenerateClientCode40(ProjectName);
		}

		public string GenerateClientCode40(string ProjectName)
		{
			bool HasWPF = false;
			foreach (DataElement DE in Elements)
				if (DE.GenerateWPFBinding == true)
				{
					HasWPF = true;
					break;
				}
			if (HasWPF == false) return "";

			//This is a shim to ensure there is ALWAYS a WPF name.
			if (WPFName == "" || WPFName == null) WPFName = CodeName + "WPF";

			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t//WPF Integration Object for the {0} DTO{1}", ContractName, Environment.NewLine);
			Code.AppendFormat("\t{0} partial class {1} : DependencyObject{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", WPFName, Environment.NewLine);
			Code.AppendLine("\t{");

			Code.AppendLine("\t\t//Properties");
			foreach (DataElement DE in Elements)
				Code.Append(DE.GenerateClientCode40(WPFName));
			Code.AppendLine();

			Code.AppendLine("\t\t//Implicit Conversion");
			Code.AppendFormat("\t\tpublic static implicit operator {0}({1} Data){2}", ContractName, WPFName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tif (Data == null) return null;");
			Code.AppendLine("\t\t\tif(Application.Current.Dispatcher.CheckAccess() == true)");
			Code.AppendLine("\t\t\t{");
			Code.AppendFormat("\t\t\t\treturn {0}.ConvertFromWPFObject(Data);{1}", WPFName, Environment.NewLine);
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t\telse");
			Code.AppendLine("\t\t\t{");
			Code.AppendFormat("\t\t\t\treturn ({0})Application.Current.Dispatcher.Invoke(new Func<{0}>(() => {1}.ConvertFromWPFObject(Data)), System.Windows.Threading.DispatcherPriority.Normal);{2}", ContractName, WPFName, Environment.NewLine);
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic static implicit operator {1}({0} Data){2}", ContractName, WPFName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tif (Data == null) return null;");
			Code.AppendLine("\t\t\tif(Application.Current.Dispatcher.CheckAccess() == true)");
			Code.AppendLine("\t\t\t{");
			Code.AppendFormat("\t\t\t\treturn {0}.ConvertToWPFObject(Data);{1}", WPFName, Environment.NewLine);
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t\telse");
			Code.AppendLine("\t\t\t{");
			Code.AppendFormat("\t\t\t\treturn ({0})Application.Current.Dispatcher.Invoke(new Func<{0}>(() => {0}.ConvertToWPFObject(Data)), System.Windows.Threading.DispatcherPriority.Normal);{1}", WPFName, Environment.NewLine);
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");
			Code.AppendLine();

			Code.AppendLine("\t\t//Constructors");
			Code.AppendFormat("\t\tpublic {1}({0}.{2} Data){3}", Parent.FullName, WPFName, ContractName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tType t_DT = Data.GetType();");
			foreach (DataElement DE in Elements)
				Code.Append(DE.GenerateClientConstructorCode40(WPFName));
			Code.AppendLine("\t\t}");
			Code.AppendLine();
			Code.AppendFormat("\t\tpublic {0}(){1}", WPFName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t}");
			Code.AppendLine();

			Code.AppendLine("\t\t//WPF/DTO Conversion Functions");
			Code.AppendFormat("\t\tpublic static {0} ConvertFromWPFObject({1} Data){2}", ContractName, WPFName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\t{0} DTO = new {0}();{1}", ContractName, Environment.NewLine);
			Code.AppendLine("\t\t\tType t_WPF = Data.GetType();");
			Code.AppendLine("\t\t\tType t_DTO = DTO.GetType();");
			foreach (DataElement DE in Elements)
				Code.Append(DE.GenerateClientConversionFromWPF30(WPFName));
			Code.AppendLine("\t\t\treturn DTO;");
			Code.AppendLine("\t\t}");
			Code.AppendLine();
			Code.AppendFormat("\t\tpublic static {0} ConvertToWPFObject({1} Data){2}", WPFName, ContractName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\t{0} WPF = new {0}();{1}", WPFName, Environment.NewLine);
			Code.AppendLine("\t\t\tType t_DTO = Data.GetType();");
			Code.AppendLine("\t\t\tType t_WPF = WPF.GetType();");
			foreach (DataElement DE in Elements)
				Code.Append(DE.GenerateClientConversionToWPF30(WPFName));
			Code.AppendLine("\t\t\treturn WPF;");
			Code.AppendLine("\t\t}");            
			Code.AppendLine("\t}");

			return Code.ToString();
		}
	}

	internal class DataElement : DependencyObject
	{
		private Guid id;
		public Guid ID { get { return id; } }

		//Basic Data-Type Settings
		public DataScope Scope { get { return (DataScope)GetValue(ScopeProperty); } set { SetValue(ScopeProperty, value); } }
		public static readonly DependencyProperty ScopeProperty = DependencyProperty.Register("Scope", typeof(DataScope), typeof(DataElement));

		public string DataType { get { return (string)GetValue(DataTypeProperty); } set { SetValue(DataTypeProperty, value); } }
		public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType", typeof(string), typeof(DataElement));

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(DataElement));

		public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(DataElement));

		//WPF Class Settings
		public bool GenerateWPFBinding { get { return (bool)GetValue(GenerateWPFBindingProperty); } set { SetValue(GenerateWPFBindingProperty, value); } }
		public static readonly DependencyProperty GenerateWPFBindingProperty = DependencyProperty.Register("GenerateWPFBinding", typeof(bool), typeof(DataElement));

		public string WPFDataType { get { return (string)GetValue(WPFDataTypeProperty); } set { SetValue(WPFDataTypeProperty, value); } }
		public static readonly DependencyProperty WPFDataTypeProperty = DependencyProperty.Register("WPFDataType", typeof(string), typeof(DataElement));

		public string WPFName { get { return (string)GetValue(WPFNameProperty); } set { SetValue(WPFNameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty WPFNameProperty = DependencyProperty.Register("WPFName", typeof(string), typeof(DataElement));

		public bool IsArray { get { return (bool)GetValue(IsArrayProperty); } set { SetValue(IsArrayProperty, value); } }
		public static readonly DependencyProperty IsArrayProperty = DependencyProperty.Register("IsArray", typeof(bool), typeof(DataElement));

		public bool IsCollection { get { return (bool)GetValue(IsCollectionProperty); } set { SetValue(IsCollectionProperty, value); } }
		public static readonly DependencyProperty IsCollectionProperty = DependencyProperty.Register("IsCollection", typeof(bool), typeof(DataElement));

		public bool IsDictionary { get { return (bool)GetValue(IsDictionaryProperty); } set { SetValue(IsDictionaryProperty, value); } }
		public static readonly DependencyProperty IsDictionaryProperty = DependencyProperty.Register("IsDictionary", typeof(bool), typeof(DataElement));

		public bool IsReadOnly { get { return (bool)GetValue(IsReadOnlyProperty); } set { SetValue(IsReadOnlyProperty, value); } }
		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(DataElement));

		public bool IsAttached { get { return (bool)GetValue(IsAttachedProperty); } set { SetValue(IsAttachedProperty, value); } }
		public static readonly DependencyProperty IsAttachedProperty = DependencyProperty.Register("IsAttached", typeof(bool), typeof(DataElement));

		public bool AttachedBrowsable { get { return (bool)GetValue(AttachedBrowsableProperty); } set { SetValue(AttachedBrowsableProperty, value); } }
		public static readonly DependencyProperty AttachedBrowsableProperty = DependencyProperty.Register("AttachedBrowsable", typeof(bool), typeof(DataElement));

		public bool AttachedBrowsableIncludeDescendants { get { return (bool)GetValue(AttachedBrowsableIncludeDescendantsProperty); } set { SetValue(AttachedBrowsableIncludeDescendantsProperty, value); } }
		public static readonly DependencyProperty AttachedBrowsableIncludeDescendantsProperty = DependencyProperty.Register("AttachedBrowsableIncludeDescendants", typeof(bool), typeof(DataElement));

		public string AttachedTargetTypes { get { return (string)GetValue(AttachedTargetTypesProperty); } set { SetValue(AttachedTargetTypesProperty, value); } }
		public static readonly DependencyProperty AttachedTargetTypesProperty = DependencyProperty.Register("AttachedTargetTypes", typeof(string), typeof(DataElement));

		public string AttachedAttributeTypes { get { return (string)GetValue(AttachedAttributeTypesProperty); } set { SetValue(AttachedAttributeTypesProperty, value); } }
		public static readonly DependencyProperty AttachedAttributeTypesProperty = DependencyProperty.Register("AttachedAttributeTypes", typeof(string), typeof(DataElement));

		//DataMember Settings
		public bool IsDataMember { get { return (bool)GetValue(IsDataMemberProperty); } set { SetValue(IsDataMemberProperty, value); } }
		public static readonly DependencyProperty IsDataMemberProperty = DependencyProperty.Register("IsDataMember", typeof(bool), typeof(DataElement), new UIPropertyMetadata(true));
		
		public bool EmitDefaultValue { get { return (bool)GetValue(EmitDefaultValueProperty); } set { SetValue(EmitDefaultValueProperty, value); } }
		public static readonly DependencyProperty EmitDefaultValueProperty = DependencyProperty.Register("EmitDefaultValue", typeof(bool), typeof(DataElement));

		public bool IsRequired { get { return (bool)GetValue(IsRequiredProperty); } set { SetValue(IsRequiredProperty, value); } }
		public static readonly DependencyProperty IsRequiredProperty = DependencyProperty.Register("IsRequired", typeof(bool), typeof(DataElement));

		public int Order { get { return (int)GetValue(OrderProperty); } set { SetValue(OrderProperty, value); } }
		public static readonly DependencyProperty OrderProperty = DependencyProperty.Register("Order", typeof(int), typeof(DataElement));

		//Internal Use
		public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(DataElement));

		public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(DataElement));

		public bool IsFiltering { get { return false; } set { } }
		public bool IsFilterMatch { get { return false; } set { } }
		public bool IsTreeExpanded { get { return false; } set { } }
		public Data Owner { get; set; }

		private string DataMemberScope { get { if (Scope == DataScope.Public) { return "public"; } else if (Scope == DataScope.Protected) { return "protected"; } else if (Scope == DataScope.Private) { return "private"; } else if (Scope == DataScope.Internal) { return "internal"; } else if (Scope == DataScope.ProtectedInternal) { return "protected internal"; } return "public"; } }
		private string StripGenericType(string DataType) { if (DataType.IndexOf("[]") >= 0) { return DataType.Replace("[]", ""); } else if (DataType.IndexOf("<") >= 0) { return DataType.Remove(0, DataType.IndexOf("<")).Replace("<", "").Replace(">", ""); } else { return ""; } }
		public bool IsValidMemberType(string DataType) { if (DataType == "" || DataType == null) { return true; } return DataType.Count(a => a == '[' || a == ']') == 0 && DataType.Count(a => a == '<' || a == '>') == 0 && DataType.Count(a => a == ',') == 0; }
		public bool IsValidArrayType(string DataType) { if (DataType == "" || DataType == null) { return true; } return DataType.Count(a => a == '[' || a == ']') == 2 && DataType.Count(a => a == '<' || a == '>') == 0 && DataType.Count(a => a == ',') == 0; }
		public bool IsValidCollectionType(string DataType) { if (DataType == "" || DataType == null) { return true; } return DataType.Count(a => a == '[' || a == ']') == 0 && DataType.Count(a => a == '<' || a == '>') == 2 && DataType.Count(a => a == ',') == 0; }
		public bool IsValidDictionaryType(string DataType) { if (DataType == "" || DataType == null) { return true; } return DataType.Count(a => a == '[' || a == ']') == 0 && DataType.Count(a => a == '<' || a == '>') == 2 && DataType.Count(a => a == ',') == 1; } 
		public ProjectCollectionType ServiceCollectionType { private get; set; }

		public DataElement()
		{
			this.id = Guid.NewGuid();
			this.Scope = DataScope.Public;
			this.DataType = "string";
			this.GenerateWPFBinding = true;
			this.AttachedTargetTypes = "";
			this.AttachedAttributeTypes = "";
			this.EmitDefaultValue = false;
			this.Order = -1;
		}

		public DataElement(DataScope Scope, string DataType, string Name, Data Owner)
		{
			this.id = Guid.NewGuid();
			this.Scope = Scope;
			this.DataType = DataType;
			this.Name = Name;
			this.GenerateWPFBinding = true;
			this.AttachedTargetTypes = "";
			this.AttachedAttributeTypes = "";
			this.EmitDefaultValue = false;
			this.Order = -1;
			this.Owner = Owner;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (e.Property == DataElement.IsSearchingProperty) return;
			if (e.Property == DataElement.IsSearchMatchProperty) return;

			if (Owner != null)
				Owner.IsDirty = true;
		}

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Data || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
							if (WPFName != null && WPFName != "") if (WPFName.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("WPF Name", WPFName, Owner.Parent.Owner, this));
						}
						if (WPFDataType != null && WPFDataType != "") if (WPFDataType.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("WPF Data Type", WPFDataType, Owner.Parent.Owner, this));
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
							if (WPFName != null && WPFName != "") if (WPFName.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("WPF Name", WPFName, Owner.Parent.Owner, this));
						}
						if (WPFDataType != null && WPFDataType != "") if (WPFDataType.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("WPF Data Type", WPFDataType, Owner.Parent.Owner, this));
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
						if (WPFName != null && WPFName != "") if (Args.RegexSearch.IsMatch(WPFName)) results.Add(new FindReplaceResult("WPF Name", WPFName, Owner.Parent.Owner, this));
					}
					if (WPFDataType != null && WPFDataType != "") if (Args.RegexSearch.IsMatch(WPFDataType)) results.Add(new FindReplaceResult("WPF Data Type", WPFDataType, Owner.Parent.Owner, this));
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
								if (WPFName != null && WPFName != "") WPFName = Microsoft.VisualBasic.Strings.Replace(WPFName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
							if (WPFDataType != null && WPFDataType != "") WPFDataType = Microsoft.VisualBasic.Strings.Replace(WPFDataType, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (WPFName != null && WPFName != "") WPFName = Microsoft.VisualBasic.Strings.Replace(WPFName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
							if (WPFDataType != null && WPFDataType != "") WPFDataType = Microsoft.VisualBasic.Strings.Replace(WPFDataType, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (WPFName != null && WPFName != "") WPFName = Args.RegexSearch.Replace(WPFName, Args.Replace);
						}
						if (WPFDataType != null && WPFDataType != "") WPFDataType = Args.RegexSearch.Replace(WPFDataType, Args.Replace);
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
							if (Field == "WPF Name") WPFName = Microsoft.VisualBasic.Strings.Replace(WPFName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						if (Field == "WPF Data Type") WPFDataType = Microsoft.VisualBasic.Strings.Replace(WPFDataType, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "WPF Name") WPFName = Microsoft.VisualBasic.Strings.Replace(WPFName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
						if (Field == "WPF Data Type") WPFDataType = Microsoft.VisualBasic.Strings.Replace(WPFDataType, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (Field == "WPF Name") WPFName = Args.RegexSearch.Replace(WPFName, Args.Replace);
					}
					if (Field == "WPF Data Type") WPFDataType = Args.RegexSearch.Replace(WPFDataType, Args.Replace);
				}
				Owner.IsActive = ia;
			}
		}

		public string GenerateServerCode30()
		{
			if (IsHidden == true) return "";
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t\tprivate {0} m_{1};{2}", DataType, Name, Environment.NewLine);
			if (IsDataMember == true)
			{
				Code.Append("\t\t[DataMember(");
				if (EmitDefaultValue == true)
					Code.AppendFormat("EmitDefaultValue = false, ");
				if (IsRequired == true)
					Code.AppendFormat("IsRequired = true, ");
				if (Order >= 0)
					Code.AppendFormat("Order = {0}, ", Order);
				Code.AppendFormat("Name = \"{0}\")] ", Name);
			}
			else
				Code.Append("\t\t");
			if (IsReadOnly == false) Code.AppendFormat("{0} {1} {2} {{ get {{ return m_{2}; }} set {{ m_{2} = value; }} }}{3}", DataMemberScope, DataType, Name, Environment.NewLine);
			else Code.AppendFormat("{0} {1} {2} {{ get {{ return m_{2}; }} protected set {{ m_{2} = value; }} }}{3}", DataMemberScope, DataType, Name, Environment.NewLine);
			return Code.ToString();
		}

		public string GenerateServerCode35()
		{
			return GenerateServerCode40();
		}

		public string GenerateServerCode40()
		{
			if (IsHidden == true) return "";
			StringBuilder Code = new StringBuilder();
			if (IsDataMember == true)
			{
				Code.Append("\t\t[DataMember(");
				if (EmitDefaultValue == true)
					Code.AppendFormat("EmitDefaultValue = false, ");
				if (IsRequired == true)
					Code.AppendFormat("IsRequired = true, ");
				if (Order >= 0)
					Code.AppendFormat("Order = {0}, ", Order);
				Code.AppendFormat("Name = \"{0}\")] ", Name);
			}
			else
				Code.Append("\t\t");
			if (IsReadOnly == false) Code.AppendFormat("{0} {1} {2} {{ get; set; }}{3}", DataMemberScope, DataType, Name, Environment.NewLine);
			else Code.AppendFormat("{0} {1} {2} {{ get; protected set; }}{3}", DataMemberScope, DataType, Name, Environment.NewLine);
			return Code.ToString();
		}
		
		public string GenerateClientConstructorCode30(string WPFClassName)
		{
			return GenerateClientConstructorCode35(WPFClassName);
		}

		public string GenerateClientConstructorCode35(string WPFClassName)
		{
			return GenerateClientConstructorCode40(WPFClassName);
		}

		public string GenerateClientConstructorCode40(string WPFClassName)
		{
			if (IsHidden == true) return "";
			if (IsDataMember == false) return "";
			StringBuilder Code = new StringBuilder();

			//Determine the proper Name and DataType for the WPF value.
			string wvn = "";			//WPF Variable Name
			string wdt = "";			//WPF Variable DataType
			if (WPFName == "" || WPFName == null) wvn = Name;
			else wvn = WPFName;
			if (WPFDataType == "" || WPFDataType == null) wdt = DataType;
			else wdt = WPFDataType;

			Code.AppendFormat("\t\t\tFieldInfo fi_{0} = t_DT.GetField(\"{0}Field\", BindingFlags.NonPublic | BindingFlags.Instance);{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tif(fi_{0} != null){1}", Name, Environment.NewLine);
			if (IsArray == true)
			{
				Code.AppendLine("\t\t\t{");
				if (ServiceCollectionType == ProjectCollectionType.Array) Code.AppendFormat("\t\t\t\t{0}[] v_{1} = fi_{1}.GetValue(Data) as {0}[];{2}", StripGenericType(DataType), Name, Environment.NewLine);
				else Code.AppendFormat("\t\t\t\t{0}<{1}> v_{2} = fi_{2}.GetValue(Data) as {0}<{1}>;{3}", System.Enum.GetName(typeof(ProjectCollectionType), ServiceCollectionType), StripGenericType(DataType), Name, Environment.NewLine);
				Code.AppendFormat("\t\t\t\tif(v_{0} != null){1}", Name, Environment.NewLine);
				Code.AppendLine("\t\t\t\t{");
				if (ServiceCollectionType == ProjectCollectionType.Array)
				{
					if (IsAttached == false)
					{
						Code.AppendFormat("\t\t\t\t\t{0} = new {1}[v_{2}.GetLength(0)];{3}", wvn, StripGenericType(wdt), Name, Environment.NewLine);
						Code.AppendFormat("\t\t\t\t\tfor(int i = 0; i < v_{1}.GetLength(0); i++) {{ try {{ {0}[i] = v_{1}[i]; }} catch {{ continue; }} }}{2}", wvn, Name, Environment.NewLine);
					}
					else
					{
						Code.AppendFormat("\t\t\t\t\t{1}[] tv = new {1}[v_{2}.GetLength(0)];{3}", wvn, StripGenericType(wdt), Name, Environment.NewLine);
						Code.AppendFormat("\t\t\t\t\tfor(int i = 0; i < v_{1}.GetLength(0); i++) {{ try {{ tv[i] = v_{1}[i]; }} catch {{ continue; }} }}{2}", wvn, Name, Environment.NewLine);
						Code.AppendFormat("\t\t\t\t\t{0}.Set{1}(this, tv);{2}", WPFClassName, wvn, Environment.NewLine);
					}
				}
				else
				{
					if (IsAttached == false)
					{
						Code.AppendFormat("\t\t\t\t\t{0} = new {1}[v_{2}.Count];{3}", wvn, StripGenericType(wdt), Name, Environment.NewLine);
						Code.AppendFormat("\t\t\t\t\tfor(int i = 0; i < v_{1}.Count; i++) {{ try {{ {0}[i] = v_{1}[i]; }} catch {{ continue; }} }}{2}", wvn, Name, Environment.NewLine);
					}
					else
					{
						Code.AppendFormat("\t\t\t\t\t{1}[] tv = new {1}[v_{2}.Count];{3}", wvn, StripGenericType(wdt), Name, Environment.NewLine);
						Code.AppendFormat("\t\t\t\t\tfor(int i = 0; i < v_{1}.Count; i++) {{ try {{ tv[i] = v_{1}[i]; }} catch {{ continue; }} }}{2}", wvn, Name, Environment.NewLine);
						Code.AppendFormat("\t\t\t\t\t{0}.Set{1}(this, tv);{2}", WPFClassName, wvn, Environment.NewLine);
					}
				}
				Code.AppendLine("\t\t\t\t}");
				Code.AppendLine("\t\t\t}");
			}
			else if (IsCollection == true)
			{
				Code.AppendLine("\t\t\t{");
				if (ServiceCollectionType == ProjectCollectionType.Array) Code.AppendFormat("\t\t\t\t{0}[] v_{1} = fi_{1}.GetValue(Data) as {0}[];{2}", StripGenericType(DataType), Name, Environment.NewLine);
				else Code.AppendFormat("\t\t\t\t{0}<{1}> v_{2} = fi_{2}.GetValue(Data) as {0}<{1}>;{3}", System.Enum.GetName(typeof(ProjectCollectionType), ServiceCollectionType), StripGenericType(DataType), Name, Environment.NewLine);
				Code.AppendFormat("\t\t\t\tif(v_{0} != null){1}", Name, Environment.NewLine);
				Code.AppendLine("\t\t\t\t{");
				if (IsAttached == false)
				{
					Code.AppendFormat("\t\t\t\t\t{1} = new {0}();{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\t\t\t\tforeach({1} a in v_{2}) {{ try {{ {0}.Add(a); }} catch {{ continue; }} }}{3}", wvn, StripGenericType(DataType), Name, Environment.NewLine);
				}
				else
				{
					Code.AppendFormat("\t\t\t\t\t{0} tv = new {0}();{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\t\t\t\tforeach({1} a in v_{2}) {{ try {{ tv.Add(a); }} catch {{ continue; }} }}{3}", wvn, StripGenericType(DataType), Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\t\t{0}.Set{1}(this, tv);{2}", WPFClassName, wvn, Environment.NewLine);
				}
				Code.AppendLine("\t\t\t\t}");
				Code.AppendLine("\t\t\t}");
			}
			else if (IsDictionary == true)
			{
				Code.AppendLine("\t\t\t{");
				if (IsAttached == false)
				{
					Code.AppendFormat("\t\t\t\t{1} = new {0}();{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\t\t\tDictionary<{1}> v_{0} = fi_{0}.GetValue(Data) as Dictionary<{1}>;{2}", Name, StripGenericType(DataType), Environment.NewLine);
					Code.AppendFormat("\t\t\t\tif(v_{0} != null){1}", Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\t\tforeach(KeyValuePair<{1}> a in v_{2}) {{ try {{ {0}.Add(a.Key, a.Value); }} catch {{ continue; }} }}{3}", wvn, StripGenericType(DataType), Name, Environment.NewLine);
				}
				else
				{
					Code.AppendFormat("\t\t\t\t{0} tv = new {0}();{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\t\t\tDictionary<{1}> v_{0} = fi_{0}.GetValue(Data) as Dictionary<{1}>;{2}", Name, StripGenericType(DataType), Environment.NewLine);
					Code.AppendFormat("\t\t\t\tif(v_{0} != null){1}", Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\t\tforeach(KeyValuePair<{1}> a in v_{2}) {{ try {{ tv.Add(a.Key, a.Value); }} catch {{ continue; }} }}{3}", wvn, StripGenericType(DataType), Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\t\t{0}.Set{1}(this, tv);{2}", WPFClassName, wvn, Environment.NewLine);
				}
				Code.AppendLine("\t\t\t}");
			}
			else
			{
				if (IsAttached == false) Code.AppendFormat("\t\t\t\ttry {{ {0} v_{1} = ({0})fi_{1}.GetValue(Data); {2} = ({3})v_{1}; }} catch {{ }}{4}", DataType, Name, wvn, wdt, Environment.NewLine);
				else Code.AppendFormat("\t\t\t\ttry {{ {0} v_{1} = ({0})fi_{1}.GetValue(Data); {4}.Set{2}(this, ({3})v_{1}); }} catch {{ }}{5}", DataType, Name, wvn, wdt, WPFClassName, Environment.NewLine);
			}

			return Code.ToString();
		}

		public string GenerateClientConversionToWPF30(string WPFClassName)
		{
			return GenerateClientConversionToWPF35(WPFClassName);
		}

		public string GenerateClientConversionToWPF35(string WPFClassName)
		{
			return GenerateClientConversionToWPF40(WPFClassName);
		}

		public string GenerateClientConversionToWPF40(string WPFClassName)
		{
			if (IsHidden == true) return "";
			if (IsDataMember == false) return "";
			StringBuilder Code = new StringBuilder();

			//Determine the proper Name and DataType for the WPF value.
			string wvn = "";			//WPF Variable Name
			string wdt = "";			//WPF Variable DataType
			if (WPFName == "" || WPFName == null) wvn = Name;
			else wvn = WPFName;
			if (WPFDataType == "" || WPFDataType == null) wdt = DataType;
			else wdt = WPFDataType;

			if (GenerateWPFBinding == true)
			{
				if (IsAttached == false)
					Code.AppendFormat("\t\t\tPropertyInfo pi_{0} = t_WPF.GetProperty(\"{0}\", BindingFlags.Public | BindingFlags.Instance);{1}", Name, Environment.NewLine);
			}
			else
			{
				if (IsAttached == false)
					if (Scope == DataScope.Public)
						Code.AppendFormat("\t\t\tPropertyInfo pi_{0} = t_WPF.GetProperty(\"{0}\", BindingFlags.Public | BindingFlags.Instance);{1}", Name, Environment.NewLine);
					else
						Code.AppendFormat("\t\t\tPropertyInfo pi_{0} = t_WPF.GetProperty(\"{0}\", BindingFlags.NonPublic | BindingFlags.Instance);{1}", Name, Environment.NewLine);
			}
			Code.AppendFormat("\t\t\tFieldInfo fi_{0} = t_DTO.GetField(\"{0}Field\", BindingFlags.NonPublic | BindingFlags.Instance);{1}", Name, Environment.NewLine);
			if (IsAttached == false) Code.AppendFormat("\t\t\tif(fi_{0} != null && pi_{0} != null){1}", Name, Environment.NewLine);
			else Code.AppendFormat("\t\t\tif(fi_{0} != null){1}", Name, Environment.NewLine);
			if (IsArray == true)
			{
				Code.AppendLine("\t\t\t{");
				if (ServiceCollectionType == ProjectCollectionType.Array) Code.AppendFormat("\t\t\t\t{0}[] v_{1} = fi_{1}.GetValue(Data) as {0}[];{2}", StripGenericType(DataType), Name, Environment.NewLine);
				else Code.AppendFormat("\t\t\t\t{0}<{1}> v_{2} = fi_{2}.GetValue(Data) as {0}<{1}>;{3}", System.Enum.GetName(typeof(ProjectCollectionType), ServiceCollectionType), StripGenericType(DataType), Name, Environment.NewLine);
				Code.AppendFormat("\t\t\t\tif(v_{0} != null){1}", Name, Environment.NewLine);
				Code.AppendLine("\t\t\t\t{");
				if (ServiceCollectionType == ProjectCollectionType.Array)
				{
					Code.AppendFormat("\t\t\t\t\t{1}[] wpf_{0} = new {1}[v_{2}.GetLength(0)];{3}", wvn, StripGenericType(wdt), Name, Environment.NewLine);
					if (IsAttached == false) Code.AppendFormat("\t\t\t\t\tpi_{0}.SetValue(WPF, wpf_{1}, null);{2}", Name, wvn, Environment.NewLine);
					else Code.AppendFormat("\t\t\t\t\t{0}.Set{1}(WPF, wpf_{1});{2}", WPFClassName, wvn, Environment.NewLine);
					Code.AppendFormat("\t\t\t\t\tfor(int i = 0; i < v_{1}.GetLength(0); i++) {{ try {{ wpf_{0}[i] = v_{1}[i]; }} catch {{ continue; }} }}{2}", wvn, Name, Environment.NewLine);
				}
				else
				{
					Code.AppendFormat("\t\t\t\t\t{1}[] wpf_{0} = new {1}[v_{2}.Count];{3}", wvn, StripGenericType(wdt), Name, Environment.NewLine);
					if (IsAttached == false) Code.AppendFormat("\t\t\t\t\tpi_{0}.SetValue(WPF, wpf_{1}, null);{2}", Name, wvn, Environment.NewLine);
					else Code.AppendFormat("\t\t\t\t\t{0}.Set{1}(WPF, wpf_{1});{2}", WPFClassName, wvn, Environment.NewLine);
					Code.AppendFormat("\t\t\t\t\tfor(int i = 0; i < v_{1}.Count; i++) {{ try {{ wpf_{0}[i] = v_{1}[i]; }} catch {{ continue; }} }}{2}", wvn, Name, Environment.NewLine);
				}
				Code.AppendLine("\t\t\t\t}");
				Code.AppendLine("\t\t\t}");
			}
			else if (IsCollection == true)
			{
				Code.AppendLine("\t\t\t{");
				if (ServiceCollectionType == ProjectCollectionType.Array) Code.AppendFormat("\t\t\t\t{0}[] v_{1} = fi_{1}.GetValue(Data) as {0}[];{2}", StripGenericType(DataType), Name, Environment.NewLine);
				else Code.AppendFormat("\t\t\t\t{0}<{1}> v_{2} = fi_{2}.GetValue(Data) as {0}<{1}>;{3}", System.Enum.GetName(typeof(ProjectCollectionType), ServiceCollectionType), StripGenericType(DataType), Name, Environment.NewLine);
				Code.AppendFormat("\t\t\t\tif(v_{0} != null){1}", Name, Environment.NewLine);
				Code.AppendLine("\t\t\t\t{");
				Code.AppendFormat("\t\t\t\t\t{0} wpf_{1} = new {0}();{2}", wdt, wvn, Environment.NewLine);
				if (IsAttached == false) Code.AppendFormat("\t\t\t\t\tpi_{0}.SetValue(WPF, wpf_{1}, null);{2}", Name, wvn, Environment.NewLine);
				else Code.AppendFormat("\t\t\t\t\t{0}.Set{1}(WPF, wpf_{1});{2}", WPFClassName, wvn, Environment.NewLine);
				Code.AppendFormat("\t\t\t\t\tforeach({1} a in v_{2}) {{ try {{ wpf_{0}.Add(a); }} catch {{ continue; }} }}{3}", wvn, StripGenericType(DataType), Name, Environment.NewLine);
				Code.AppendLine("\t\t\t\t}");
				Code.AppendLine("\t\t\t}");
			}
			else if (IsDictionary == true)
			{
				Code.AppendLine("\t\t\t{");
				Code.AppendFormat("\t\t\t\t{0} wpf_{1} = new {0}();{2}", wdt, wvn, Environment.NewLine);
				if (IsAttached == false) Code.AppendFormat("\t\t\t\t\tpi_{0}.SetValue(WPF, wpf_{1}, null);{2}", Name, wvn, Environment.NewLine);
				else Code.AppendFormat("\t\t\t\t{0}.Set{1}(WPF, wpf_{1});{2}", WPFClassName, wvn, Environment.NewLine);
				Code.AppendFormat("\t\t\t\tDictionary<{1}> v_{0} = fi_{0}.GetValue(Data) as Dictionary<{1}>;{2}", Name, StripGenericType(DataType), Environment.NewLine);
				Code.AppendFormat("\t\t\t\tif(v_{2} != null) foreach(KeyValuePair<{1}> a in v_{2}) {{ try {{ wpf_{0}.Add(a.Key, a.Value); }} catch {{ continue; }} }}{3}", wvn, StripGenericType(DataType), Name, Environment.NewLine);
				Code.AppendLine("\t\t\t}");
			}
			else
			{
				if (IsAttached == false) Code.AppendFormat("\t\t\t\ttry {{ {0} v_{1} = ({0})fi_{1}.GetValue(Data); pi_{1}.SetValue(WPF, ({3})v_{1}, null); }} catch {{ }}{4}", DataType, Name, wvn, wdt, Environment.NewLine);
				else Code.AppendFormat("\t\t\t\ttry {{ {0} v_{1} = ({0})fi_{1}.GetValue(Data); {4}.Set{2}(WPF, ({3})v_{1}); }} catch {{ }}{5}", DataType, Name, wvn, wdt, WPFClassName, Environment.NewLine);
			}

			return Code.ToString();
		}

		public string GenerateClientConversionFromWPF30(string WPFClassName)
		{
			return GenerateClientConversionFromWPF35(WPFClassName);
		}

		public string GenerateClientConversionFromWPF35(string WPFClassName)
		{
			return GenerateClientConversionFromWPF40(WPFClassName);
		}

		public string GenerateClientConversionFromWPF40(string WPFClassName)
		{
			if (IsHidden == true) return "";
			if (IsDataMember == false) return "";
			StringBuilder Code = new StringBuilder();

			//Determine the proper Name and DataType for the WPF value.
			string wvn = "";			//WPF Variable Name
			string wdt = "";			//WPF Variable DataType
			if (WPFName == "" || WPFName == null) wvn = Name;
			else wvn = WPFName;
			if (WPFDataType == "" || WPFDataType == null) wdt = DataType;
			else wdt = WPFDataType;

			if (GenerateWPFBinding == true)
			{
				if (IsAttached == false)
					Code.AppendFormat("\t\t\tPropertyInfo pi_{0} = t_WPF.GetProperty(\"{0}\", BindingFlags.Public | BindingFlags.Instance);{1}", Name, Environment.NewLine);
			}
			else
			{
				if (IsAttached == false)
					if (Scope == DataScope.Public)
						Code.AppendFormat("\t\t\tPropertyInfo pi_{0} = t_WPF.GetProperty(\"{0}\", BindingFlags.Public | BindingFlags.Instance);{1}", Name, Environment.NewLine);
					else
						Code.AppendFormat("\t\t\tPropertyInfo pi_{0} = t_WPF.GetProperty(\"{0}\", BindingFlags.NonPublic | BindingFlags.Instance);{1}", Name, Environment.NewLine);
			}
			Code.AppendFormat("\t\t\tFieldInfo fi_{0} = t_DTO.GetField(\"{0}Field\", BindingFlags.NonPublic | BindingFlags.Instance);{1}", Name, Environment.NewLine);
			if (IsArray == true)
			{
				if (IsAttached == false) Code.AppendFormat("\t\t\tif(fi_{0} != null && pi_{0} != null){1}", Name, Environment.NewLine);
				else Code.AppendFormat("\t\t\tif(fi_{0} != null){1}", Name, Environment.NewLine);
				Code.AppendLine("\t\t\t{");
				if (ServiceCollectionType == ProjectCollectionType.Array)
				{
					if (IsAttached == false) Code.AppendFormat("\t\t\t\t{0} wpf_{1} = pi_{2}.GetValue(Data, null) as {0};{3}", wdt, wvn, Name, Environment.NewLine);
					else Code.AppendFormat("\t\t\t\t{0} wpf_{1} = {3}.Get{1}(Data) as {0};{4}", wdt, wvn, Name, WPFClassName, Environment.NewLine);
					Code.AppendFormat("\t\t\t\t{1}[] v_{2} = new {1}[wpf_{0}.GetLength(0)];{3}", wvn, StripGenericType(DataType), Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\tfor(int i = 0; i < wpf_{0}.GetLength(0); i++) {{ try {{ v_{1}[i] = Data.{0}[i]; }} catch {{ continue; }} }}{2}", wvn, Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\tfi_{0}.SetValue(DTO, v_{0});{1}", Name, Environment.NewLine);
				}
				else
				{
					Code.AppendFormat("\t\t\t\t{0}<{1}> v_{2} = new {0}<{1}>();{3}", System.Enum.GetName(typeof(ProjectCollectionType), ServiceCollectionType), StripGenericType(DataType), Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\tfi_{0}.SetValue(DTO, v_{0});{1}", Name, Environment.NewLine);
					if (IsAttached == false) Code.AppendFormat("\t\t\t\t{0} wpf_{1} = pi_{2}.GetValue(Data, null) as {0};{3}", wdt, wvn, Name, Environment.NewLine);
					else Code.AppendFormat("\t\t\t\t{0} wpf_{1} = {3}.Get{1}(Data) as {0};{4}", wdt, wvn, Name, WPFClassName, Environment.NewLine);
					Code.AppendFormat("\t\t\t\tforeach({1} a in wpf_{0}) {{ try {{ v_{2}.Add(a); }} catch {{ continue; }} }}{3}", wvn, StripGenericType(wdt), Name, Environment.NewLine);

				}
				Code.AppendLine("\t\t\t}");
			}
			else if (IsCollection == true)
			{
				if (IsAttached == false) Code.AppendFormat("\t\t\tif(fi_{0} != null && pi_{0} != null){1}", Name, Environment.NewLine);
				else Code.AppendFormat("\t\t\tif(fi_{0} != null){1}", Name, Environment.NewLine);
				Code.AppendLine("\t\t\t{");
				if (ServiceCollectionType == ProjectCollectionType.Array)
				{
					if (IsAttached == false) Code.AppendFormat("\t\t\t\t{0} wpf_{1} = pi_{2}.GetValue(Data, null) as {0};{3}", wdt, wvn, Name, Environment.NewLine);
					else Code.AppendFormat("\t\t\t\t{0} wpf_{1} = {3}.Get{1}(Data) as {0};{4}", wdt, wvn, Name, WPFClassName, Environment.NewLine);
					Code.AppendFormat("\t\t\t\t{1}[] v_{2} = new {1}[wpf_{0}.Count];{3}", wvn, StripGenericType(DataType), Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\tfor(int i = 0; i < wpf_{0}.Count; i++) {{ try {{ v_{1}[i] = wpf_{0}[i]; }} catch {{ continue; }} }}{2}", wvn, Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\tfi_{0}.SetValue(DTO, v_{0});{1}", Name, Environment.NewLine);
				}
				else
				{
					Code.AppendFormat("\t\t\t\t{0}<{1}> v_{2} = new {0}<{1}>();{3}", System.Enum.GetName(typeof(ProjectCollectionType), ServiceCollectionType), StripGenericType(DataType), Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\tfi_{0}.SetValue(DTO, v_{0});{1}", Name, Environment.NewLine);
					if (IsAttached == false) Code.AppendFormat("\t\t\t\t{0} wpf_{1} = pi_{2}.GetValue(Data, null) as {0};{3}", wdt, wvn, Name, Environment.NewLine);
					else Code.AppendFormat("\t\t\t\t{0} wpf_{1} = {3}.Get{1}(Data) as {0};{4}", wdt, wvn, Name, WPFClassName, Environment.NewLine);
					Code.AppendFormat("\t\t\t\tforeach({1} a in wpf_{0}) {{ try {{ v_{2}.Add(a); }} catch {{ continue; }} }}{3}", wvn, StripGenericType(wdt), Name, Environment.NewLine);
				}
				Code.AppendLine("\t\t\t}");
			}
			else if (IsDictionary == true)
			{
				if (IsAttached == false) Code.AppendFormat("\t\t\tif(fi_{0} != null && pi_{0} != null){1}", Name, Environment.NewLine);
				else Code.AppendFormat("\t\t\tif(fi_{0} != null){1}", Name, Environment.NewLine);
				Code.AppendLine("\t\t\t{");
				Code.AppendFormat("\t\t\t\tDictionary<{0}> v_{1} = new Dictionary<{0}>();{2}", StripGenericType(DataType), Name, Environment.NewLine);
				Code.AppendFormat("\t\t\t\tfi_{0}.SetValue(DTO, v_{0});{1}", Name, Environment.NewLine);
				if (IsAttached == false) Code.AppendFormat("\t\t\t\t{0} wpf_{1} = pi_{2}.GetValue(Data, null) as {0};{3}", wdt, wvn, Name, Environment.NewLine);
				else Code.AppendFormat("\t\t\t\t{0} wpf_{1} = {3}.Get{1}(Data) as {0};{4}", wdt, wvn, Name, WPFClassName, Environment.NewLine);
				Code.AppendFormat("\t\t\t\tforeach(KeyValuePair<{1}> a in wpf_{0}) {{ try {{ v_{2}.Add(a.Key, a.Value); }} catch {{ continue; }} }}{3}", wvn, StripGenericType(wdt), Name, Environment.NewLine);
				Code.AppendLine("\t\t\t}");
			}
			else
			{
				if (IsAttached == false) Code.AppendFormat("\t\t\tif(fi_{1} != null && pi_{0} != null) fi_{1}.SetValue(DTO, ({2})pi_{1}.GetValue(Data, null));{3}", wvn, Name, DataType, Environment.NewLine);
				else Code.AppendFormat("\t\t\tif(fi_{1} != null) fi_{1}.SetValue(DTO, ({2}){3}.Get{0}(Data));{4}", wvn, Name, DataType, WPFClassName, Environment.NewLine);
			}

			return Code.ToString();
		}

		public string GenerateClientCode30(string ClassName)
		{
			if (IsHidden == true) return "";
			if (IsDataMember == false) return "";

			//Determine the proper Name and DataType for the WPF value.
			string wvn = "";			//WPF Variable Name
			string wdt = "";			//WPF Variable DataType
			if (WPFName == "" || WPFName == null) wvn = Name;
			else wvn = WPFName;
			if (WPFDataType == "" || WPFDataType == null) wdt = DataType;
			else wdt = WPFDataType;

			StringBuilder Code = new StringBuilder();
			if (GenerateWPFBinding == true)
			{
				if (IsReadOnly == false && IsAttached == false)
				{
					Code.AppendFormat("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue({1}Property); }} set {{ SetValue({1}Property, value); }} }}{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.Register(\"{1}\", typeof({0}), typeof({2}));{3}", wdt, wvn, ClassName, Environment.NewLine);
				}
				if (IsReadOnly == true && IsAttached == false)
				{
					Code.AppendFormat("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue({1}Property); }} protected set {{ SetValue({1}PropertyKey, value); }}{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\tprivate static readonly DependencyPropertyKey {1}PropertyKey = DependencyProperty.RegisterReadOnly(\"{1}\", typeof({0}), typeof({2}), new PropertyMetadata(null));{3}", wdt, wvn, ClassName, Environment.NewLine);
					Code.AppendFormat("\t\tpublic static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;{1}", wvn, Environment.NewLine);
				}
				if (IsReadOnly == false && IsAttached == true)
				{
					if (AttachedBrowsable == true)
					{
						if (AttachedBrowsableIncludeDescendants == true) { Code.AppendLine("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=true)]"); }
						else { Code.AppendLine("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=false)]"); }
					}
					if (AttachedTargetTypes != "")
					{
						List<string> TTL = new List<string>(AttachedTargetTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string TT in TTL)
							Code.AppendFormat("\t\t[AttachedPropertyBrowsableForType(typeof({0}))]{1}", TT.Trim(), Environment.NewLine);
					}
					if (AttachedAttributeTypes != "")
					{
						List<string> TTL = new List<string>(AttachedAttributeTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string TT in TTL)
							Code.AppendFormat("\t\t[AttachedPropertyBrowsableWhenAttributePresent(typeof({0}))]{1}", TT.Trim(), Environment.NewLine);
					}
					Code.AppendFormat("\t\tpublic static {0} Get{1}(DependencyObject obj) {{ return ({0})obj.GetValue({1}Property); }}{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\tpublic static void Set{1}(DependencyObject obj, {0} value) {{ obj.SetValue({1}Property, value); }}{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.RegisterAttached(\"{1}\", typeof({0}), typeof({2}), new UIPropertyMetadata(null));{3}", wdt, wvn, ClassName, Environment.NewLine);
				}
				if (IsReadOnly == true && IsAttached == true)
				{
					if (AttachedBrowsable == true)
					{
						if (AttachedBrowsableIncludeDescendants == true) { Code.AppendLine("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=true)]"); }
						else { Code.AppendLine("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=false)]"); }
					}
					if (AttachedTargetTypes != "")
					{
						List<string> TTL = new List<string>(AttachedTargetTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string TT in TTL)
							Code.AppendFormat("\t\t[AttachedPropertyBrowsableForType(typeof({0}))]{1}", TT.Trim(), Environment.NewLine);
					}
					if (AttachedAttributeTypes != "")
					{
						List<string> TTL = new List<string>(AttachedAttributeTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string TT in TTL)
							Code.AppendFormat("\t\t[AttachedPropertyBrowsableWhenAttributePresent(typeof({0}))]{1}", TT.Trim(), Environment.NewLine);
					}
					Code.AppendFormat("\t\tpublic static {0} Get{1}(DependencyObject obj) {{ return ({0})obj.GetValue({1}Property); }}{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\tprotected static void Set{1}(DependencyObject obj, {0} value) {{ obj.SetValue({1}PropertyKey, value); }}{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\tprivate static readonly DependencyPropertyKey {1}PropertyKey = DependencyProperty.RegisterAttachedReadOnly(\"{1}\", typeof({0}), typeof({2}), new PropertyMetadata(null));{3}", wdt, wvn, ClassName, Environment.NewLine);
					Code.AppendFormat("\t\tpublic static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;{1}", wvn, Environment.NewLine);
				}
			}
			else
			{
				if (IsReadOnly == false)
				{
					Code.AppendFormat("\t\tprivate {0} m_{1};{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\t{0} {1} {2} {{ get {{ return m_{2}; }} set {{ m_{2} = value; }} }}{3}", DataMemberScope, wdt, wvn, Environment.NewLine);
				}
				else
				{
					Code.AppendFormat("\t\tprivate {0} m_{1};{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\t{0} {1} {2} {{ get {{ return m_{2}; }} protected set {{ m_{2} = value; }} }}{3}", DataMemberScope, wdt, wvn, Environment.NewLine);
				}
			}
			return Code.ToString();
		}

		public string GenerateClientCode35(string ClassName)
		{
			return GenerateClientCode40(ClassName);
		}

		public string GenerateClientCode40(string ClassName)
		{
			if (IsHidden == true) return "";
			if (IsDataMember == false) return "";

			//Determine the proper Name and DataType for the WPF value.
			string wvn = "";			//WPF Variable Name
			string wdt = "";			//WPF Variable DataType
			if (WPFName == "" || WPFName == null) wvn = Name;
			else wvn = WPFName;
			if (WPFDataType == "" || WPFDataType == null) wdt = DataType;
			else wdt = WPFDataType;

			StringBuilder Code = new StringBuilder();
			if (GenerateWPFBinding == true)
			{
				if (IsReadOnly == false && IsAttached == false)
				{
					Code.AppendFormat("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue({1}Property); }} set {{ SetValue({1}Property, value); }} }}{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.Register(\"{1}\", typeof({0}), typeof({2}));{3}", wdt, wvn, ClassName, Environment.NewLine);
				}
				if (IsReadOnly == true && IsAttached == false)
				{
					Code.AppendFormat("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue({1}Property); }} protected set {{ SetValue({1}PropertyKey, value); }} }}{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\tprivate static readonly DependencyPropertyKey {1}PropertyKey = DependencyProperty.RegisterReadOnly(\"{1}\", typeof({0}), typeof({2}), new PropertyMetadata(null));{3}", wdt, wvn, ClassName, Environment.NewLine);
					Code.AppendFormat("\t\tpublic static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;{1}", wvn, Environment.NewLine);
				}
				if (IsReadOnly == false && IsAttached == true)
				{
					if (AttachedBrowsable == true)
					{
						if (AttachedBrowsableIncludeDescendants == true) { Code.AppendLine("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=true)]"); }
						else { Code.AppendLine("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=false)]"); }
					}
					if (AttachedTargetTypes != "")
					{
						List<string> TTL = new List<string>(AttachedTargetTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string TT in TTL)
							Code.AppendFormat("\t\t[AttachedPropertyBrowsableForType(typeof({0}))]{1}", TT.Trim(), Environment.NewLine);
					}
					if (AttachedAttributeTypes != "")
					{
						List<string> TTL = new List<string>(AttachedAttributeTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string TT in TTL)
							Code.AppendFormat("\t\t[AttachedPropertyBrowsableWhenAttributePresent(typeof({0}))]{1}", TT.Trim(), Environment.NewLine);
					}
					Code.AppendFormat("\t\tpublic static {0} Get{1}(DependencyObject obj) {{ return ({0})obj.GetValue({1}Property); }}{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\tpublic static void Set{1}(DependencyObject obj, {0} value) {{ obj.SetValue({1}Property, value); }}{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.RegisterAttached(\"{1}\", typeof({0}), typeof({2}), new UIPropertyMetadata(null));{3}", wdt, wvn, ClassName, Environment.NewLine);
				}
				if (IsReadOnly == true && IsAttached == true)
				{
					if (AttachedBrowsable == true)
					{
						if (AttachedBrowsableIncludeDescendants == true) { Code.AppendLine("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=true)]"); }
						else { Code.AppendLine("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=false)]"); }
					}
					if (AttachedTargetTypes != "")
					{
						List<string> TTL = new List<string>(AttachedTargetTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string TT in TTL)
							Code.AppendFormat("\t\t[AttachedPropertyBrowsableForType(typeof({0}))]{1}", TT.Trim(), Environment.NewLine);
					}
					if (AttachedAttributeTypes != "")
					{
						List<string> TTL = new List<string>(AttachedAttributeTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string TT in TTL)
							Code.AppendFormat("\t\t[AttachedPropertyBrowsableWhenAttributePresent(typeof({0}))]{1}", TT.Trim(), Environment.NewLine);
					}
					Code.AppendFormat("\t\tpublic static {0} Get{1}(DependencyObject obj) {{ return ({0})obj.GetValue({1}Property); }}{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\tprotected static void Set{1}(DependencyObject obj, {0} value) {{ obj.SetValue({1}PropertyKey, value); }}{2}", wdt, wvn, Environment.NewLine);
					Code.AppendFormat("\t\tprivate static readonly DependencyPropertyKey {1}PropertyKey = DependencyProperty.RegisterAttachedReadOnly(\"{1}\", typeof({0}), typeof({2}), new PropertyMetadata(null));{3}", wdt, wvn, ClassName, Environment.NewLine);
					Code.AppendFormat("\t\tpublic static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;{1}", wvn, Environment.NewLine);
				}
			}
			else
			{
				if (IsReadOnly == false)
				{
					Code.AppendFormat("\t\t{0} {1} {2} {{ get; set; }}{3}", DataMemberScope, wdt, wvn, Environment.NewLine);
				}
				else
				{
					Code.AppendFormat("\t\t{0} {1} {2} {{ get; protected set; }}{3}", DataMemberScope, wdt, wvn, Environment.NewLine);
				}
			}
			return Code.ToString();
		}
	}
}