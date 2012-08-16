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
		public bool HasXAMLType { get { return (bool)GetValue(HasXAMLTypeProperty); } set { SetValue(HasXAMLTypeProperty, value); if (value == false) { XAMLType = null; } else { XAMLType = new DataType(DataTypeMode.Class); XAMLType.Name = Name + "XAML"; XAMLType.Scope = Scope; XAMLType.InheritedTypes.Add(new DataType("DependencyObject", DataTypeMode.Class)); } } }
		public static readonly DependencyProperty HasXAMLTypeProperty = DependencyProperty.Register("HasXAMLType", typeof(bool), typeof(Data));

		public DataType XAMLType { get { return (DataType)GetValue(XAMLTypeProperty); } set { SetValue(XAMLTypeProperty, value); } }
		public static readonly DependencyProperty XAMLTypeProperty = DependencyProperty.Register("XAMLType", typeof(DataType), typeof(Data));

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
			this.HasXAMLType = true;
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
							if (XAMLType.Name != null && XAMLType.Name != "") if (XAMLType.Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("WPF Name", XAMLType.Name, Parent.Owner, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Code Name", Name, Parent.Owner, this));
							if (ContractType.Name != null && ContractType.Name != "") if (ContractType.Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractType.Name, Parent.Owner, this));
							if (XAMLType.Name != null && XAMLType.Name != "") if (XAMLType.Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("WPF Name", XAMLType.Name, Parent.Owner, this));
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
						if (XAMLType.Name != null && XAMLType.Name != "") if (Args.RegexSearch.IsMatch(XAMLType.Name)) results.Add(new FindReplaceResult("WPF Name", XAMLType.Name, Parent.Owner, this));
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
								if (XAMLType.Name != null && XAMLType.Name != "") XAMLType.Name = Microsoft.VisualBasic.Strings.Replace(XAMLType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (ContractType.Name != null && ContractType.Name != "") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (XAMLType.Name != null && XAMLType.Name != "") XAMLType.Name = Microsoft.VisualBasic.Strings.Replace(XAMLType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
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
							if (XAMLType.Name != null && XAMLType.Name != "") XAMLType.Name = Args.RegexSearch.Replace(XAMLType.Name, Args.Replace);
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
							if (Field == "WPF Name") XAMLType.Name = Microsoft.VisualBasic.Strings.Replace(XAMLType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Code Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Contract Name") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "WPF Name") XAMLType.Name = Microsoft.VisualBasic.Strings.Replace(XAMLType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
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
						if (Field == "WPF Name") XAMLType.Name = Args.RegexSearch.Replace(XAMLType.Name, Args.Replace);
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
		public DataScope DataScope { get { return (DataScope)GetValue(DataScopeProperty); } set { SetValue(DataScopeProperty, value); } }
		public static readonly DependencyProperty DataScopeProperty = DependencyProperty.Register("DataScope", typeof(DataScope), typeof(DataElement), new PropertyMetadata(DataScope.Public));

		public DataType DataType { get { return (DataType)GetValue(DataTypeProperty); } set { SetValue(DataTypeProperty, value); } }
		public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType", typeof(DataType), typeof(DataElement));

		public string DataName { get { return (string)GetValue(DataNameProperty); } set { SetValue(DataNameProperty, value); } }
		public static readonly DependencyProperty DataNameProperty = DependencyProperty.Register("DataName", typeof(string), typeof(DataElement), new PropertyMetadata(""));

		public bool HasContractType { get { return (bool)GetValue(HasContractTypeProperty); } set { SetValue(HasContractTypeProperty, value); if (value == false) { ContractType = null; } else { if (ContractType != null) return; ContractType = new DataType(PrimitiveTypes.String); ContractName = DataName; ContractScope = DataScope; } } }
		public static readonly DependencyProperty HasContractTypeProperty = DependencyProperty.Register("HasContractType", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		public DataScope ContractScope { get { return (DataScope)GetValue(ContractScopeProperty); } set { SetValue(ContractScopeProperty, value); } }
		public static readonly DependencyProperty ContractScopeProperty = DependencyProperty.Register("ContractScope", typeof(DataScope), typeof(DataElement), new PropertyMetadata(DataScope.Public));

		public DataType ContractType { get { return (DataType)GetValue(ContractTypeProperty); } set { SetValue(ContractTypeProperty, value); } }
		public static readonly DependencyProperty ContractTypeProperty = DependencyProperty.Register("ContractType", typeof(DataType), typeof(DataElement));

		public string ContractName { get { return (string)GetValue(ContractNameProperty); } set { SetValue(ContractNameProperty, value); } }
		public static readonly DependencyProperty ContractNameProperty = DependencyProperty.Register("ContractName", typeof(string), typeof(DataElement), new PropertyMetadata(""));

		public bool HasXAMLType { get { return (bool)GetValue(HasXAMLTypeProperty); } set { SetValue(HasXAMLTypeProperty, value); if (value == false) { XAMLType = null; } else { if (XAMLType != null) return; XAMLType = new DataType(PrimitiveTypes.String); XAMLName = DataName; XAMLScope = DataScope; } } }
		public static readonly DependencyProperty HasXAMLTypeProperty = DependencyProperty.Register("HasXAMLType", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		public DataScope XAMLScope { get { return (DataScope)GetValue(XAMLScopeProperty); } set { SetValue(XAMLScopeProperty, value); } }
		public static readonly DependencyProperty XAMLScopeProperty = DependencyProperty.Register("XAMLScope", typeof(DataScope), typeof(DataElement), new PropertyMetadata(DataScope.Public));

		public DataType XAMLType { get { return (DataType)GetValue(XAMLTypeProperty); } set { SetValue(XAMLTypeProperty, value); } }
		public static readonly DependencyProperty XAMLTypeProperty = DependencyProperty.Register("XAMLType", typeof(DataType), typeof(DataElement));

		public string XAMLName { get { return (string)GetValue(XAMLNameProperty); } set { SetValue(XAMLNameProperty, value); } }
		public static readonly DependencyProperty XAMLNameProperty = DependencyProperty.Register("XAMLName", typeof(string), typeof(DataElement), new PropertyMetadata(""));

		public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		public bool IsReadOnly { get { return (bool)GetValue(IsReadOnlyProperty); } set { SetValue(IsReadOnlyProperty, value); } }
		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		public bool GenerateWinFormsSupport { get { return (bool)GetValue(GenerateWinFormsSupportProperty); } set { SetValue(GenerateWinFormsSupportProperty, value); } }
		public static readonly DependencyProperty GenerateWinFormsSupportProperty = DependencyProperty.Register("GenerateWinFormsSupport", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		public bool Serializable { get { return (bool)GetValue(SerializableProperty); } set { SetValue(SerializableProperty, value); } }
		public static readonly DependencyProperty SerializableProperty = DependencyProperty.Register("Serializable", typeof(bool), typeof(Data), new PropertyMetadata(false));

		//DataMember Settings
		public bool IsDataMember { get { return (bool)GetValue(IsDataMemberProperty); } set { SetValue(IsDataMemberProperty, value); } }
		public static readonly DependencyProperty IsDataMemberProperty = DependencyProperty.Register("IsDataMember", typeof(bool), typeof(DataElement), new UIPropertyMetadata(true));
		
		public bool EmitDefaultValue { get { return (bool)GetValue(EmitDefaultValueProperty); } set { SetValue(EmitDefaultValueProperty, value); } }
		public static readonly DependencyProperty EmitDefaultValueProperty = DependencyProperty.Register("EmitDefaultValue", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		public bool IsRequired { get { return (bool)GetValue(IsRequiredProperty); } set { SetValue(IsRequiredProperty, value); } }
		public static readonly DependencyProperty IsRequiredProperty = DependencyProperty.Register("IsRequired", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		public int Order { get { return (int)GetValue(OrderProperty); } set { SetValue(OrderProperty, value); } }
		public static readonly DependencyProperty OrderProperty = DependencyProperty.Register("Order", typeof(int), typeof(DataElement), new PropertyMetadata(-1));

		//WPF Class Settings
		public bool IsAttached { get { return (bool)GetValue(IsAttachedProperty); } set { SetValue(IsAttachedProperty, value); } }
		public static readonly DependencyProperty IsAttachedProperty = DependencyProperty.Register("IsAttached", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		public bool AttachedBrowsable { get { return (bool)GetValue(AttachedBrowsableProperty); } set { SetValue(AttachedBrowsableProperty, value); } }
		public static readonly DependencyProperty AttachedBrowsableProperty = DependencyProperty.Register("AttachedBrowsable", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		public bool AttachedBrowsableIncludeDescendants { get { return (bool)GetValue(AttachedBrowsableIncludeDescendantsProperty); } set { SetValue(AttachedBrowsableIncludeDescendantsProperty, value); } }
		public static readonly DependencyProperty AttachedBrowsableIncludeDescendantsProperty = DependencyProperty.Register("AttachedBrowsableIncludeDescendants", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		public string AttachedTargetTypes { get { return (string)GetValue(AttachedTargetTypesProperty); } set { SetValue(AttachedTargetTypesProperty, value); } }
		public static readonly DependencyProperty AttachedTargetTypesProperty = DependencyProperty.Register("AttachedTargetTypes", typeof(string), typeof(DataElement), new PropertyMetadata(""));

		public string AttachedAttributeTypes { get { return (string)GetValue(AttachedAttributeTypesProperty); } set { SetValue(AttachedAttributeTypesProperty, value); } }
		public static readonly DependencyProperty AttachedAttributeTypesProperty = DependencyProperty.Register("AttachedAttributeTypes", typeof(string), typeof(DataElement), new PropertyMetadata(""));

		//Internal Use
		[IgnoreDataMember()] public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		[IgnoreDataMember()] public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		[IgnoreDataMember()] public bool IsFiltering { get { return false; } set { } }
		[IgnoreDataMember()] public bool IsFilterMatch { get { return false; } set { } }
		public bool IsTreeExpanded { get { return false; } set { } }
		public Data Owner { get; set; }

		public DataElement()
		{
			this.ID = Guid.NewGuid();
			this.DataType = new DataType(PrimitiveTypes.String);
			this.DataScope = DataScope.Public;
			this.HasXAMLType = true;
			this.AttachedTargetTypes = "";
			this.AttachedAttributeTypes = "";
			this.EmitDefaultValue = false;
			this.Order = -1;
		}

		public DataElement(DataScope Scope, DataType DataType, string Name, Data Owner)
		{
			this.ID = Guid.NewGuid();
			this.DataType = DataType;
			this.DataName = Name;
			this.DataScope = DataScope.Public;
			this.HasXAMLType = true;
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
							if (XAMLType.Name != null && XAMLType.Name != "") if (XAMLType.Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("WPF Name", XAMLType.Name, Owner.Parent.Owner, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (DataType.Name != null && DataType.Name != "") if (DataType.Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", DataType.Name, Owner.Parent.Owner, this));
							if (ContractType.Name != null && ContractType.Name != "") if (ContractType.Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractType.Name, Owner.Parent.Owner, this));
							if (XAMLType.Name != null && XAMLType.Name != "") if (XAMLType.Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("WPF Name", XAMLType.Name, Owner.Parent.Owner, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (DataType.Name != null && DataType.Name != "") if (Args.RegexSearch.IsMatch(DataType.Name)) results.Add(new FindReplaceResult("Name", DataType.Name, Owner.Parent.Owner, this));
						if (ContractType.Name != null && ContractType.Name != "") if (ContractType.Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractType.Name, Owner.Parent.Owner, this));
						if (XAMLType.Name != null && XAMLType.Name != "") if (Args.RegexSearch.IsMatch(XAMLType.Name)) results.Add(new FindReplaceResult("WPF Name", XAMLType.Name, Owner.Parent.Owner, this));
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
								if (XAMLType.Name != null && XAMLType.Name != "") XAMLType.Name = Microsoft.VisualBasic.Strings.Replace(XAMLType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (DataType.Name != null && DataType.Name != "") DataType.Name = Microsoft.VisualBasic.Strings.Replace(DataType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (ContractType.Name != null && ContractType.Name != "") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (XAMLType.Name != null && XAMLType.Name != "") XAMLType.Name = Microsoft.VisualBasic.Strings.Replace(XAMLType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (DataType.Name != null && DataType.Name != "") DataType.Name = Args.RegexSearch.Replace(DataType.Name, Args.Replace);
							if (ContractType.Name != null && ContractType.Name != "") ContractType.Name = Args.RegexSearch.Replace(ContractType.Name, Args.Replace);
							if (XAMLType.Name != null && XAMLType.Name != "") XAMLType.Name = Args.RegexSearch.Replace(XAMLType.Name, Args.Replace);
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
							if (Field == "WPF Name") XAMLType.Name = Microsoft.VisualBasic.Strings.Replace(XAMLType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") DataType.Name = Microsoft.VisualBasic.Strings.Replace(DataType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Contract Name") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "WPF Name") XAMLType.Name = Microsoft.VisualBasic.Strings.Replace(XAMLType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") DataType.Name = Args.RegexSearch.Replace(DataType.Name, Args.Replace);
						if (Field == "Contract Name") ContractType.Name = Args.RegexSearch.Replace(ContractType.Name, Args.Replace);
						if (Field == "WPF Name") XAMLType.Name = Args.RegexSearch.Replace(XAMLType.Name, Args.Replace);
					}
				}
				Owner.IsActive = ia;
			}
		}
	}
}