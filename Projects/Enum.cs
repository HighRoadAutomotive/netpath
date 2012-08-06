using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;

namespace WCFArchitect.Projects
{
	[DataContract()]
	public class Enum : DataType
	{
		[DataMember()] public bool IsReference { get { return (bool)GetValue(IsReferenceProperty); } set { SetValue(IsReferenceProperty, value); } }
		public static readonly DependencyProperty IsReferenceProperty = DependencyProperty.Register("IsReference", typeof(bool), typeof(Enum));

		[DataMember()] public bool IsFlags { get { return (bool)GetValue(IsFlagsProperty); } set { SetValue(IsFlagsProperty, value); } }
		public static readonly DependencyProperty IsFlagsProperty = DependencyProperty.Register("IsFlags", typeof(bool), typeof(Enum));

		[DataMember()] public DataType BaseType { get { return (DataType)GetValue(BaseTypeProperty); } set { SetValue(BaseTypeProperty, value); } }
		public static readonly DependencyProperty BaseTypeProperty = DependencyProperty.Register("BaseType", typeof(DataType), typeof(Enum));

		[DataMember()] public ObservableCollection<EnumElement> Elements { get { return (ObservableCollection<EnumElement>)GetValue(ElementsProperty); } set { SetValue(ElementsProperty, value); } }
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

		[DataMember()] public bool IsTreeExpanded { get { return (bool)GetValue(IsTreeExpandedProperty); } set { SetValue(IsTreeExpandedProperty, value); } }
		public static readonly DependencyProperty IsTreeExpandedProperty = DependencyProperty.Register("IsTreeExpanded", typeof(bool), typeof(Enum));

		public Enum() : base(DataTypeMode.Enum)
		{
		}

		public Enum(string Name, Namespace Parent) : base(DataTypeMode.Enum)
		{
			this.Elements = new ObservableCollection<EnumElement>();
			this.id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.HasContractType = false;
			this.Parent = Parent;
			this.BaseType = new DataType(PrimitiveTypes.Int);
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
							if (HasContractType == true) if (ContractType.Name != null && ContractType.Name != "") if (ContractType.Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractType.Name, Parent.Owner, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
							if (HasContractType == true) if (ContractType.Name != null && ContractType.Name != "") if (ContractType.Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractType.Name, Parent.Owner, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
						if (HasContractType == true) if (ContractType.Name != null && ContractType.Name != "") if (Args.RegexSearch.IsMatch(ContractType.Name)) results.Add(new FindReplaceResult("Contract Name", ContractType.Name, Parent.Owner, this));
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
								if (HasContractType == true) if (ContractType.Name != null && ContractType.Name != "") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (HasContractType == true) if (ContractType.Name != null && ContractType.Name != "") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (HasContractType == true) if (ContractType.Name != null && ContractType.Name != "") ContractType.Name = Args.RegexSearch.Replace(ContractType.Name, Args.Replace);
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
							if (HasContractType == true) if (Field == "Contract Name") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (HasContractType == true) if (Field == "Contract Name") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (HasContractType == true) if (Field == "Contract Name") ContractType.Name = Args.RegexSearch.Replace(ContractType.Name, Args.Replace);
					}
				}
				IsActive = ia;
			}
		}
	}

	[DataContract()]
	public class EnumElement : DependencyObject
	{
		private Guid id;
		public Guid ID { get { return id; } }

		//Basic
		[DataMember()] public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(EnumElement));

		[DataMember()] public bool IsExcluded { get { return (bool)GetValue(IsExcludedProperty); } set { SetValue(IsExcludedProperty, value); } }
		public static readonly DependencyProperty IsExcludedProperty = DependencyProperty.Register("IsExcluded", typeof(bool), typeof(EnumElement));

		[DataMember()] public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(EnumElement));

		//Regular Enums
		[DataMember()] public string Value { get { return (string)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(EnumElement));

		[DataMember()] public string ContractValue { get { return (string)GetValue(ContractValueProperty); } set { SetValue(ContractValueProperty, value); } }
		public static readonly DependencyProperty ContractValueProperty = DependencyProperty.Register("ContractValue", typeof(string), typeof(EnumElement));

		//Internal Use - Searching / Filtering
		public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(EnumElement));

		public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(EnumElement));

		public bool IsFiltering { get { return false; } set { } }
		public bool IsFilterMatch { get { return false; } set { } }
		public bool IsTreeExpanded { get { return false; } set { } }
		[DataMember()] public Enum Owner { get; set; }

		public EnumElement()
		{
			this.id = Guid.NewGuid();
			this.IsExcluded = false;
		}

		public EnumElement(string Name, string Value, string ContractValue, Enum Owner)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.Value = Value;
			this.ContractValue = ContractValue;
			this.IsExcluded = false;
			this.Owner = Owner;
		}

		public EnumElement(string Name, string Flags, Enum Owner)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
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
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) > 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
							if (Value != null && Value != "") if (Value.IndexOf(Args.Search) > 0) results.Add(new FindReplaceResult("Value", Value, Owner.Parent.Owner, this));
							if (ContractValue != null && ContractValue != "") if (ContractValue.IndexOf(Args.Search) > 0) results.Add(new FindReplaceResult("Contract Value", ContractValue, Owner.Parent.Owner, this));
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
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (Value != null && Value != "") Value = Microsoft.VisualBasic.Strings.Replace(Value, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (ContractValue != null && ContractValue != "") ContractValue = Microsoft.VisualBasic.Strings.Replace(ContractValue, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
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
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Value") Value = Microsoft.VisualBasic.Strings.Replace(Value, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Contract Value") ContractValue = Microsoft.VisualBasic.Strings.Replace(ContractValue, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
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
					}
				}
				Owner.IsActive = ia;
			}
		}
	}
}