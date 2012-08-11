using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;

namespace WCFArchitect.Projects
{
	public enum DataScope
	{
		Public,
		Protected,
		Private,
		Internal,
		ProtectedInternal,
	}

	public class Data : DataType
	{
		public bool HasWPFType { get { return (bool)GetValue(HasWPFTypeProperty); } set { SetValue(HasWPFTypeProperty, value); if (value == false) { ContractType = null; } else { ContractType = new DataType(DataTypeMode.Class); WPFType.Name = Name + "WPF"; WPFType.Scope = Scope; WPFType.InheritedTypes.Add(new DataType("DependencyObject", DataTypeMode.Class)); } } }
		public static readonly DependencyProperty HasWPFTypeProperty = DependencyProperty.Register("HasWPFType", typeof(bool), typeof(Data));

		public DataType WPFType { get { return (DataType)GetValue(WPFTypeProperty); } set { SetValue(WPFTypeProperty, value); } }
		public static readonly DependencyProperty WPFTypeProperty = DependencyProperty.Register("WPFType", typeof(DataType), typeof(Data));

		public bool IsReference { get { return (bool)GetValue(IsReferenceProperty); } set { SetValue(IsReferenceProperty, value); } }
		public static readonly DependencyProperty IsReferenceProperty = DependencyProperty.Register("IsReference", typeof(bool), typeof(Data));

		public ObservableCollection<DataElement> Elements { get { return (ObservableCollection<DataElement>)GetValue(ElementsProperty); } set { SetValue(ElementsProperty, value); } }
		public static readonly DependencyProperty ElementsProperty = DependencyProperty.Register("Elements", typeof(ObservableCollection<DataElement>), typeof(Data));

		//Internal Use - Searching / Filtering
		[IgnoreDataMember()] public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(Data));

		[IgnoreDataMember()] public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(Data));

		[IgnoreDataMember()] public bool IsFiltering { get { return (bool)GetValue(IsFilteringProperty); } set { SetValue(IsFilteringProperty, value); } }
		public static readonly DependencyProperty IsFilteringProperty = DependencyProperty.Register("IsFiltering", typeof(bool), typeof(Data));

		[IgnoreDataMember()] public bool IsFilterMatch { get { return (bool)GetValue(IsFilterMatchProperty); } set { SetValue(IsFilterMatchProperty, value); } }
		public static readonly DependencyProperty IsFilterMatchProperty = DependencyProperty.Register("IsFilterMatch", typeof(bool), typeof(Data));

		public bool IsTreeExpanded { get { return (bool)GetValue(IsTreeExpandedProperty); } set { SetValue(IsTreeExpandedProperty, value); } }
		public static readonly DependencyProperty IsTreeExpandedProperty = DependencyProperty.Register("IsTreeExpanded", typeof(bool), typeof(Data));

		public Data() : base(DataTypeMode.Class)
		{
		}

		public Data(string Name, Namespace Parent) : base(DataTypeMode.Class)
		{
			this.Elements = new ObservableCollection<DataElement>();
			this.ID = Guid.NewGuid();
			this.Name = Name;
			this.Name = Helpers.RegExs.ReplaceSpaces.Replace(Name, @"");
			this.HasContractType = false;
			this.HasWPFType = true;
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
					if (T.DataType.Name != "" && T.DataType.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
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
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Code Name", Name, Parent.Owner, this));
							if (ContractType.Name != null && ContractType.Name != "") if (ContractType.Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractType.Name, Parent.Owner, this));
							if (WPFType.Name != null && WPFType.Name != "") if (WPFType.Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("WPF Name", WPFType.Name, Parent.Owner, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Code Name", Name, Parent.Owner, this));
							if (ContractType.Name != null && ContractType.Name != "") if (ContractType.Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractType.Name, Parent.Owner, this));
							if (WPFType.Name != null && WPFType.Name != "") if (WPFType.Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("WPF Name", WPFType.Name, Parent.Owner, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Code Name", Name, Parent.Owner, this));
						if (ContractType.Name != null && ContractType.Name != "") if (Args.RegexSearch.IsMatch(ContractType.Name)) results.Add(new FindReplaceResult("Contract Name", ContractType.Name, Parent.Owner, this));
						if (WPFType.Name != null && WPFType.Name != "") if (Args.RegexSearch.IsMatch(WPFType.Name)) results.Add(new FindReplaceResult("WPF Name", WPFType.Name, Parent.Owner, this));
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
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (ContractType.Name != null && ContractType.Name != "") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (WPFType.Name != null && WPFType.Name != "") WPFType.Name = Microsoft.VisualBasic.Strings.Replace(WPFType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (ContractType.Name != null && ContractType.Name != "") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (WPFType.Name != null && WPFType.Name != "") WPFType.Name = Microsoft.VisualBasic.Strings.Replace(WPFType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (ContractType.Name != null && ContractType.Name != "") ContractType.Name = Args.RegexSearch.Replace(ContractType.Name, Args.Replace);
							if (WPFType.Name != null && WPFType.Name != "") WPFType.Name = Args.RegexSearch.Replace(WPFType.Name, Args.Replace);
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
							if (Field == "Code Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Contract Name") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "WPF Name") WPFType.Name = Microsoft.VisualBasic.Strings.Replace(WPFType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Code Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Contract Name") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "WPF Name") WPFType.Name = Microsoft.VisualBasic.Strings.Replace(WPFType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (Field == "Code Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (Field == "Contract Name") ContractType.Name = Args.RegexSearch.Replace(ContractType.Name, Args.Replace);
						if (Field == "WPF Name") WPFType.Name = Args.RegexSearch.Replace(WPFType.Name, Args.Replace);
					}
				}
				IsActive = ia;
			}
		}
	}

	public class DataElement : DependencyObject
	{
		public Guid ID { get; set; }

		//Basic Data-Type Settings
		public DataType DataType { get { return (DataType)GetValue(DataTypeProperty); } set { SetValue(DataTypeProperty, value); } }
		public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType", typeof(DataType), typeof(DataElement));

		public bool HasContractType { get { return (bool)GetValue(HasContractTypeProperty); } set { SetValue(HasContractTypeProperty, value); if (value == false) { ContractType = null; } else { if (ContractType != null) return; ContractType = new DataType(DataTypeMode.Enum); ContractType.Name = DataType.Name; ContractType.Scope = DataType.Scope; } } }
		public static readonly DependencyProperty HasContractTypeProperty = DependencyProperty.Register("HasContractType", typeof(bool), typeof(DataElement));

		public DataType ContractType { get { return (DataType)GetValue(ContractTypeProperty); } set { SetValue(ContractTypeProperty, value); } }
		public static readonly DependencyProperty ContractTypeProperty = DependencyProperty.Register("ContractType", typeof(DataType), typeof(DataElement));

		public bool HasWPFType { get { return (bool)GetValue(HasWPFTypeProperty); } set { SetValue(HasWPFTypeProperty, value); if (value == false) { WPFType = null; } else { if (WPFType != null) return; WPFType = new DataType(DataTypeMode.Class); WPFType.Name = DataType.Name + "WPF"; WPFType.Scope = DataType.Scope; } } }
		public static readonly DependencyProperty HasWPFTypeProperty = DependencyProperty.Register("HasWPFType", typeof(bool), typeof(DataElement));

		public DataType WPFType { get { return (DataType)GetValue(WPFTypeProperty); } set { SetValue(WPFTypeProperty, value); } }
		public static readonly DependencyProperty WPFTypeProperty = DependencyProperty.Register("WPFType", typeof(DataType), typeof(DataElement));

		public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(DataElement));

		public bool IsReadOnly { get { return (bool)GetValue(IsReadOnlyProperty); } set { SetValue(IsReadOnlyProperty, value); } }
		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(DataElement));

		//DataMember Settings
		public bool IsDataMember { get { return (bool)GetValue(IsDataMemberProperty); } set { SetValue(IsDataMemberProperty, value); } }
		public static readonly DependencyProperty IsDataMemberProperty = DependencyProperty.Register("IsDataMember", typeof(bool), typeof(DataElement), new UIPropertyMetadata(true));
		
		public bool EmitDefaultValue { get { return (bool)GetValue(EmitDefaultValueProperty); } set { SetValue(EmitDefaultValueProperty, value); } }
		public static readonly DependencyProperty EmitDefaultValueProperty = DependencyProperty.Register("EmitDefaultValue", typeof(bool), typeof(DataElement));

		public bool IsRequired { get { return (bool)GetValue(IsRequiredProperty); } set { SetValue(IsRequiredProperty, value); } }
		public static readonly DependencyProperty IsRequiredProperty = DependencyProperty.Register("IsRequired", typeof(bool), typeof(DataElement));

		public int Order { get { return (int)GetValue(OrderProperty); } set { SetValue(OrderProperty, value); } }
		public static readonly DependencyProperty OrderProperty = DependencyProperty.Register("Order", typeof(int), typeof(DataElement));

		//WPF Class Settings
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

		//Internal Use
		[IgnoreDataMember()] public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(DataElement));

		[IgnoreDataMember()] public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(DataElement));

		[IgnoreDataMember()] public bool IsFiltering { get { return false; } set { } }
		[IgnoreDataMember()] public bool IsFilterMatch { get { return false; } set { } }
		public bool IsTreeExpanded { get { return false; } set { } }
		public Data Owner { get; set; }

		public DataElement()
		{
			this.ID = Guid.NewGuid();
			this.DataType = new DataType(PrimitiveTypes.String);
			this.DataType.Scope = DataScope.Public;
			this.HasWPFType = true;
			this.AttachedTargetTypes = "";
			this.AttachedAttributeTypes = "";
			this.EmitDefaultValue = false;
			this.Order = -1;
		}

		public DataElement(DataScope Scope, DataType DataType, string Name, Data Owner)
		{
			this.ID = Guid.NewGuid();
			this.DataType = DataType;
			this.DataType.Scope = DataScope.Public;
			this.HasWPFType = true;
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
							if (DataType.Name != null && DataType.Name != "") if (DataType.Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", DataType.Name, Owner.Parent.Owner, this));
							if (ContractType.Name != null && ContractType.Name != "") if (ContractType.Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractType.Name, Owner.Parent.Owner, this));
							if (WPFType.Name != null && WPFType.Name != "") if (WPFType.Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("WPF Name", WPFType.Name, Owner.Parent.Owner, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (DataType.Name != null && DataType.Name != "") if (DataType.Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", DataType.Name, Owner.Parent.Owner, this));
							if (ContractType.Name != null && ContractType.Name != "") if (ContractType.Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractType.Name, Owner.Parent.Owner, this));
							if (WPFType.Name != null && WPFType.Name != "") if (WPFType.Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("WPF Name", WPFType.Name, Owner.Parent.Owner, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (DataType.Name != null && DataType.Name != "") if (Args.RegexSearch.IsMatch(DataType.Name)) results.Add(new FindReplaceResult("Name", DataType.Name, Owner.Parent.Owner, this));
						if (ContractType.Name != null && ContractType.Name != "") if (ContractType.Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractType.Name, Owner.Parent.Owner, this));
						if (WPFType.Name != null && WPFType.Name != "") if (Args.RegexSearch.IsMatch(WPFType.Name)) results.Add(new FindReplaceResult("WPF Name", WPFType.Name, Owner.Parent.Owner, this));
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
								if (DataType.Name != null && DataType.Name != "") DataType.Name = Microsoft.VisualBasic.Strings.Replace(DataType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (ContractType.Name != null && ContractType.Name != "") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (WPFType.Name != null && WPFType.Name != "") WPFType.Name = Microsoft.VisualBasic.Strings.Replace(WPFType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (DataType.Name != null && DataType.Name != "") DataType.Name = Microsoft.VisualBasic.Strings.Replace(DataType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (ContractType.Name != null && ContractType.Name != "") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (WPFType.Name != null && WPFType.Name != "") WPFType.Name = Microsoft.VisualBasic.Strings.Replace(WPFType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (DataType.Name != null && DataType.Name != "") DataType.Name = Args.RegexSearch.Replace(DataType.Name, Args.Replace);
							if (ContractType.Name != null && ContractType.Name != "") ContractType.Name = Args.RegexSearch.Replace(ContractType.Name, Args.Replace);
							if (WPFType.Name != null && WPFType.Name != "") WPFType.Name = Args.RegexSearch.Replace(WPFType.Name, Args.Replace);
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
							if (Field == "Name") DataType.Name = Microsoft.VisualBasic.Strings.Replace(DataType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Contract Name") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "WPF Name") WPFType.Name = Microsoft.VisualBasic.Strings.Replace(WPFType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") DataType.Name = Microsoft.VisualBasic.Strings.Replace(DataType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Contract Name") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "WPF Name") WPFType.Name = Microsoft.VisualBasic.Strings.Replace(WPFType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") DataType.Name = Args.RegexSearch.Replace(DataType.Name, Args.Replace);
						if (Field == "Contract Name") ContractType.Name = Args.RegexSearch.Replace(ContractType.Name, Args.Replace);
						if (Field == "WPF Name") WPFType.Name = Args.RegexSearch.Replace(WPFType.Name, Args.Replace);
					}
				}
				Owner.IsActive = ia;
			}
		}
	}
}