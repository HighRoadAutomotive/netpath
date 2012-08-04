using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WCFArchitect.Projects
{
	internal enum EnumDataType
	{
		Int,
		SByte,
		Byte,
		Short,
		UShort,
		UInt,
		Long,
		ULong,
	}

	internal enum EnumFlagsDataType
	{
		ULong,
		Long,
		UInt,
		Int,
		UShort,
		Short,
	}

	internal class Enum : OpenableDocument
	{
		private Guid id;
		public Guid ID { get { return id; } }

		public EnumDataType DataType { get { return (EnumDataType)GetValue(DataTypeProperty); } set { SetValue(DataTypeProperty, value); } }
		public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType", typeof(EnumDataType), typeof(Enum));

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Enum));

		public string CodeName { get { return (string)GetValue(CodeNameProperty); } set { if (Globals.IsLoading == false) Globals.ReplaceDataType(CodeName, Parent.FullName, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @""), Parent.FullName); SetValue(CodeNameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty CodeNameProperty = DependencyProperty.Register("CodeName", typeof(string), typeof(Enum));

		public string ContractName { get { return (string)GetValue(ContractNameProperty); } set { if (Globals.IsLoading == false) Globals.ReplaceDataType(ContractName, Parent.FullName, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @""), Parent.FullName); SetValue(ContractNameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty ContractNameProperty = DependencyProperty.Register("ContractName", typeof(string), typeof(Enum));

		public bool IsReference { get { return (bool)GetValue(IsReferenceProperty); } set { SetValue(IsReferenceProperty, value); } }
		public static readonly DependencyProperty IsReferenceProperty = DependencyProperty.Register("IsReference", typeof(bool), typeof(Enum));

		public bool IsFlags { get { return (bool)GetValue(IsFlagsProperty); } set { SetValue(IsFlagsProperty, value); } }
		public static readonly DependencyProperty IsFlagsProperty = DependencyProperty.Register("IsFlags", typeof(bool), typeof(Enum));

		public EnumFlagsDataType FlagsDataType { get { return (EnumFlagsDataType)GetValue(FlagsDataTypeProperty); } set { SetValue(FlagsDataTypeProperty, value); } }
		public static readonly DependencyProperty FlagsDataTypeProperty = DependencyProperty.Register("FlagsDataType", typeof(EnumFlagsDataType), typeof(Enum));

		public ObservableCollection<EnumElement> Elements { get { return (ObservableCollection<EnumElement>)GetValue(ElementsProperty); } set { SetValue(ElementsProperty, value); } }
		public static readonly DependencyProperty ElementsProperty = DependencyProperty.Register("Elements", typeof(ObservableCollection<EnumElement>), typeof(Enum));

		//Internal Use - Searching / Filtering
		public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(Enum));

		public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(Enum));

		public bool IsFiltering { get { return (bool)GetValue(IsFilteringProperty); } set { SetValue(IsFilteringProperty, value); } }
		public static readonly DependencyProperty IsFilteringProperty = DependencyProperty.Register("IsFiltering", typeof(bool), typeof(Enum));

		public bool IsFilterMatch { get { return (bool)GetValue(IsFilterMatchProperty); } set { SetValue(IsFilterMatchProperty, value); } }
		public static readonly DependencyProperty IsFilterMatchProperty = DependencyProperty.Register("IsFilterMatch", typeof(bool), typeof(Enum));

		public bool IsTreeExpanded { get { return (bool)GetValue(IsTreeExpandedProperty); } set { SetValue(IsTreeExpandedProperty, value); } }
		public static readonly DependencyProperty IsTreeExpandedProperty = DependencyProperty.Register("IsTreeExpanded", typeof(bool), typeof(Enum));

		public Namespace Parent { get; set; }

		private string EnumDataType { get { if (IsFlags == false) { return System.Enum.GetName(typeof(EnumDataType), DataType).ToLower(); } else { return System.Enum.GetName(typeof(EnumFlagsDataType), FlagsDataType).ToLower(); } } }

		public Enum() : base()
		{
			this.UserOpenedList = new List<UserOpened>();
		}

		public Enum(string Name, Namespace Parent) : base()
		{
			this.UserOpenedList = new List<UserOpened>();
			this.Elements = new ObservableCollection<EnumElement>();
			this.id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.ContractName = this.CodeName;
			this.Parent = Parent;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (e.Property == Enum.IsSearchingProperty) return;
			if (e.Property == Enum.IsSearchMatchProperty) return;
			if (e.Property == Enum.IsFilteringProperty) return;
			if (e.Property == Enum.IsFilterMatchProperty) return;
			if (e.Property == Enum.IsTreeExpandedProperty) return;

			IsDirty = true;
		}

		public void Search(string Value)
		{
			if (Value != "")
			{
				foreach (EnumElement T in Elements)
				{
					T.IsSearching = true;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					if (IsSearchMatch == false) IsSearchMatch = T.IsSearchMatch;
				}
			}
			else
			{
				foreach (EnumElement T in Elements)
				{
					T.IsSearching = false;
					T.IsSearchMatch = false;
				}
			}
		}

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Enum || Args.Items == FindItems.Any)
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

			foreach (EnumElement EE in Elements)
				results.AddRange(EE.FindReplace(Args));

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
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4000", "The enumeration '" + Name + "' in the '" + Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchCodeName.IsMatch(CodeName) == false)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4001", "The enumeration '" + Name + "' in the '" + Parent.Name + "' namespace contains invalid characters in the Contract Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
					NoErrors = false;
				}

			if (ContractName != "" && ContractName != null)
				if (Helpers.RegExs.MatchCodeName.IsMatch(ContractName) == false)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4002", "The enumeration '" + Name + "' in the '" + Parent.Name + "' namespace contains invalid characters in the Contract Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
					NoErrors = false;
				}

			foreach (EnumElement E in Elements)
			{
				if (E.Name == "" || E.Name == null)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4003", "The enumeration '" + Name + "' in the '" + Parent.Name + "' namespace has an enumeration element with a blank Name. A Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, E, E.GetType()));
					NoErrors = false;
				}
				else
					if (Helpers.RegExs.MatchCodeName.IsMatch(E.Name) == false)
					{
						Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4004", "The enumeration element '" + E.Name + "' in the '" + Name + "' enumeration contains invalid characters in the Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, E, E.GetType()));
						NoErrors = false;
					}

				if (IsFlags == false)
				{
					bool BadValue = false;
					if (DataType == Projects.EnumDataType.Int) 
						try { Convert.ToInt32(E.Value); }
						catch { BadValue = true; }
					else if (DataType == Projects.EnumDataType.SByte)
						try { Convert.ToSByte(E.Value); }
						catch { BadValue = true; }
					else if (DataType == Projects.EnumDataType.Byte)
						try { Convert.ToByte(E.Value); }
						catch { BadValue = true; }
					else if (DataType == Projects.EnumDataType.UShort)
						try { Convert.ToUInt16(E.Value); }
						catch { BadValue = true; }
					else if (DataType == Projects.EnumDataType.Short)
						try { Convert.ToInt16(E.Value); }
						catch { BadValue = true; }
					else if (DataType == Projects.EnumDataType.UInt)
						try { Convert.ToUInt32(E.Value); }
						catch { BadValue = true; }
					else if (DataType == Projects.EnumDataType.Long)
						try { Convert.ToInt64(E.Value); }
						catch { BadValue = true; }
					else if (DataType == Projects.EnumDataType.ULong)
						try { Convert.ToUInt64(E.Value); }
						catch { BadValue = true; }

					if (E.Value == "") BadValue = false;

					if (BadValue == true)
					{
						Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4005", "The enumeration element '" + E.Name + "' in the '" + Name + "' enumeration contains an invalid value.", WCFArchitect.Compiler.CompileMessageSeverity.Error, this, E, E.GetType()));
						NoErrors = false;
					}
				}
			}

			if (IsFlags == true)
			{
				if (FlagsDataType == EnumFlagsDataType.Short && Elements.Where(a => a.FlagValue == "").Count() > 14) Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4006", "The number of elements in the Enumeration '" + Name + "' exceeds the maximum number of elements (14) supported by the 'short' data type. Any elements above this limit will not be generated.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
				if (FlagsDataType == EnumFlagsDataType.UShort && Elements.Where(a => a.FlagValue == "").Count() > 15) Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4006", "The number of elements in the Enumeration '" + Name + "' exceeds the maximum number of elements (15) supported by the 'unsigned short' data type. Any elements above this limit will not be generated.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
				if (FlagsDataType == EnumFlagsDataType.Int && Elements.Where(a => a.FlagValue == "").Count() > 30) Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4006", "The number of elements in the Enumeration '" + Name + "' exceeds the maximum number of elements (30) supported by the 'int' data type. Any elements above this limit will not be generated.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
				if (FlagsDataType == EnumFlagsDataType.UInt && Elements.Where(a => a.FlagValue == "").Count() > 31) Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4006", "The number of elements in the Enumeration '" + Name + "' exceeds the maximum number of elements (31) supported by the 'unsigned int' data type. Any elements above this limit will not be generated.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
				if (FlagsDataType == EnumFlagsDataType.Long && Elements.Where(a => a.FlagValue == "").Count() > 62) Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4006", "The number of elements in the Enumeration '" + Name + "' exceeds the maximum number of elements (62) supported by the 'long' data type. Any elements above this limit will not be generated.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
				if (FlagsDataType == EnumFlagsDataType.ULong && Elements.Where(a => a.FlagValue == "").Count() > 63) Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS4006", "The number of elements in the Enumeration '" + Name + "' exceeds the maximum number of elements (63) supported by the 'unsigned long' data type. Any elements above this limit will not be generated.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
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
			if (IsFlags == true) Code.AppendLine("[Flags]");
			Code.AppendFormat("\t{0} enum {2} : {1}{3}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", EnumDataType, CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			int FV = 0;
			foreach (EnumElement EE in Elements)
			{
				if (EE.IsHidden == true) continue;
				if (IsFlags == false) Code.Append(EE.GenerateCode(DataType));
				else
					if (EE.FlagValue == "" || EE.FlagValue == null) Code.Append(EE.GenerateCode(FlagsDataType, FV++));
					else Code.Append(EE.GenerateCode(FlagsDataType, 0));
			}
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
			if (ContractName != "" && ContractName != null) Code.AppendFormat("Name = \"{0}\", ", ContractName);
			Code.AppendFormat("Namespace = \"{0}\"", Parent.URI);
			Code.AppendLine(")]");
			if (IsFlags == true) Code.AppendLine("\t[Flags]");
			Code.AppendFormat("\t{0} enum {2} : {1}{3}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", EnumDataType, CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			int FV = 0;
			foreach (EnumElement EE in Elements)
			{
				if (EE.IsHidden == true) continue;
				if (IsFlags == false) Code.Append(EE.GenerateCode(DataType));
				else
					if (EE.FlagValue == "" || EE.FlagValue == null) Code.Append(EE.GenerateCode(FlagsDataType, FV++));
					else Code.Append(EE.GenerateCode(FlagsDataType, 0));
			}
			Code.AppendLine("\t}");
			return Code.ToString();
		}
	}

	internal class EnumElement : DependencyObject
	{
		private Guid id;
		public Guid ID { get { return id; } }

		//Basic
		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(EnumElement));

		public bool IsExcluded { get { return (bool)GetValue(IsExcludedProperty); } set { SetValue(IsExcludedProperty, value); } }
		public static readonly DependencyProperty IsExcludedProperty = DependencyProperty.Register("IsExcluded", typeof(bool), typeof(EnumElement));

		public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(EnumElement));

		//Regular Enums
		public string Value { get { return (string)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(EnumElement));

		public string ContractValue { get { return (string)GetValue(ContractValueProperty); } set { SetValue(ContractValueProperty, value); } }
		public static readonly DependencyProperty ContractValueProperty = DependencyProperty.Register("ContractValue", typeof(string), typeof(EnumElement));

		//Flag Enums
		public string FlagValue { get { return (string)GetValue(FlagValueProperty); } set { SetValue(FlagValueProperty, value); } }
		public static readonly DependencyProperty FlagValueProperty = DependencyProperty.Register("FlagValue", typeof(string), typeof(EnumElement));

		//Internal Use - Searching / Filtering
		public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(EnumElement));

		public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(EnumElement));

		public bool IsFiltering { get { return false; } set { } }
		public bool IsFilterMatch { get { return false; } set { } }
		public bool IsTreeExpanded { get { return false; } set { } }
		public Enum Owner { get; set; }

		public EnumElement()
		{
			this.id = Guid.NewGuid();
			this.IsExcluded = false;
			this.FlagValue = "";
		}

		public EnumElement(string Name, string Value, string ContractValue, Enum Owner)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.Value = Value;
			this.ContractValue = ContractValue;
			this.IsExcluded = false;
			this.FlagValue = "";
			this.Owner = Owner;
		}

		public EnumElement(string Name, string Flags, Enum Owner)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.FlagValue = Flags;
			this.IsExcluded = false;
			this.Owner = Owner;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (e.Property == EnumElement.IsSearchingProperty) return;
			if (e.Property == EnumElement.IsSearchMatchProperty) return;

			if (Owner != null)
				Owner.IsDirty = true;
		}

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Enum || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) > 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
							if (Value != null && Value != "") if (Value.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) > 0) results.Add(new FindReplaceResult("Value", Value, Owner.Parent.Owner, this));
							if (ContractValue != null && ContractValue != "") if (ContractValue.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) > 0) results.Add(new FindReplaceResult("Contract Value", ContractValue, Owner.Parent.Owner, this));
							if (FlagValue != null && FlagValue != "") if (FlagValue.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) > 0) results.Add(new FindReplaceResult("Flag Value", FlagValue, Owner.Parent.Owner, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) > 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
							if (Value != null && Value != "") if (Value.IndexOf(Args.Search) > 0) results.Add(new FindReplaceResult("Value", Value, Owner.Parent.Owner, this));
							if (ContractValue != null && ContractValue != "") if (ContractValue.IndexOf(Args.Search) > 0) results.Add(new FindReplaceResult("Contract Value", ContractValue, Owner.Parent.Owner, this));
							if (FlagValue != null && FlagValue != "") if (FlagValue.IndexOf(Args.Search) > 0) results.Add(new FindReplaceResult("Flag Value", FlagValue, Owner.Parent.Owner, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
						if (Value != null && Value != "") if (Args.RegexSearch.IsMatch(Value)) results.Add(new FindReplaceResult("Value", Value, Owner.Parent.Owner, this));
						if (ContractValue != null && ContractValue != "") if (Args.RegexSearch.IsMatch(ContractValue)) results.Add(new FindReplaceResult("Contract Value", ContractValue, Owner.Parent.Owner, this));
						if (FlagValue != null && FlagValue != "") if (Args.RegexSearch.IsMatch(FlagValue)) results.Add(new FindReplaceResult("Flag Value", FlagValue, Owner.Parent.Owner, this));
					}
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
								if (Value != null && Value != "") Value = Microsoft.VisualBasic.Strings.Replace(Value, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (ContractValue != null && ContractValue != "") ContractValue = Microsoft.VisualBasic.Strings.Replace(ContractValue, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (FlagValue != null && FlagValue != "") FlagValue = Microsoft.VisualBasic.Strings.Replace(FlagValue, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (Value != null && Value != "") Value = Microsoft.VisualBasic.Strings.Replace(Value, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (ContractValue != null && ContractValue != "") ContractValue = Microsoft.VisualBasic.Strings.Replace(ContractValue, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (FlagValue != null && FlagValue != "") FlagValue = Microsoft.VisualBasic.Strings.Replace(FlagValue, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (Value != null && Value != "") Value = Args.RegexSearch.Replace(Value, Args.Replace);
							if (ContractValue != null && ContractValue != "") ContractValue = Args.RegexSearch.Replace(ContractValue, Args.Replace);
							if (FlagValue != null && FlagValue != "") FlagValue = Args.RegexSearch.Replace(FlagValue, Args.Replace);
						}
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
							if (Field == "Value") Value = Microsoft.VisualBasic.Strings.Replace(Value, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Contract Value") ContractValue = Microsoft.VisualBasic.Strings.Replace(ContractValue, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Flag Value") FlagValue = Microsoft.VisualBasic.Strings.Replace(FlagValue, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Value") Value = Microsoft.VisualBasic.Strings.Replace(Value, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Contract Value") ContractValue = Microsoft.VisualBasic.Strings.Replace(ContractValue, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Flag Value") FlagValue = Microsoft.VisualBasic.Strings.Replace(FlagValue, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (Field == "Value") Value = Args.RegexSearch.Replace(Value, Args.Replace);
						if (Field == "Contract Value") ContractValue = Args.RegexSearch.Replace(ContractValue, Args.Replace);
						if (Field == "Flag Value") FlagValue = Args.RegexSearch.Replace(FlagValue, Args.Replace);
					}
				}
				Owner.IsActive = ia;
			}
		}

		public string GenerateCode(EnumDataType DataType)
		{
			StringBuilder Code = new StringBuilder();
			Code.Append("\t\t[EnumMember(");
			if (ContractValue == "" || ContractValue == null) { }
			else
				Code.AppendFormat("Value = \"{0}\"", ContractValue);
			Code.AppendFormat(")] {0}", Name);
			if (Value == "" || Value == null) { }
			else Code.AppendFormat(" = {0}", Value);
			Code.AppendLine(",");
			return Code.ToString();
		}

		public string GenerateCode(EnumFlagsDataType DataType, int ElementIndex)
		{
			if (DataType == EnumFlagsDataType.Short && ElementIndex > 14) return "";
			if (DataType == EnumFlagsDataType.UShort && ElementIndex > 15) return "";
			if (DataType == EnumFlagsDataType.Int && ElementIndex > 30) return "";
			if (DataType == EnumFlagsDataType.UInt && ElementIndex > 31) return "";
			if (DataType == EnumFlagsDataType.Long && ElementIndex > 62) return "";
			if (DataType == EnumFlagsDataType.ULong && ElementIndex > 63) return "";

			StringBuilder Code = new StringBuilder();
			Code.Append("\t\t");
			if (IsExcluded == false) Code.Append("[EnumMember()] ");
			Code.Append(Name);
			if (FlagValue == "" || FlagValue == null)
			{
				if (ElementIndex == 0) Code.Append(" = 0");
				if (ElementIndex == 1) Code.Append(" = 1");
				if (ElementIndex > 1) Code.AppendFormat(" = {0}", Convert.ToInt32(Decimal.Round((decimal)Math.Pow(2, ElementIndex - 1))));
			}
			else
				Code.AppendFormat(" = {0}", FlagValue);
			Code.AppendLine(",");
			return Code.ToString();
		}

	}
}