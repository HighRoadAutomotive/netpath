using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;

namespace WCFArchitect.Projects
{
	public class Enum : DataType
	{
		public bool IsReference { get { return (bool)GetValue(IsReferenceProperty); } set { SetValue(IsReferenceProperty, value); } }
		public static readonly DependencyProperty IsReferenceProperty = DependencyProperty.Register("IsReference", typeof(bool), typeof(Enum));

		public bool IsFlags { get { return (bool)GetValue(IsFlagsProperty); } set { SetValue(IsFlagsProperty, value); } }
		public static readonly DependencyProperty IsFlagsProperty = DependencyProperty.Register("IsFlags", typeof(bool), typeof(Enum));

		public DataType BaseType { get { return (DataType)GetValue(BaseTypeProperty); } set { SetValue(BaseTypeProperty, value); } }
		public static readonly DependencyProperty BaseTypeProperty = DependencyProperty.Register("BaseType", typeof(DataType), typeof(Enum));

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(Enum));

		public ObservableCollection<EnumElement> Elements { get { return (ObservableCollection<EnumElement>)GetValue(ElementsProperty); } set { SetValue(ElementsProperty, value); } }
		public static readonly DependencyProperty ElementsProperty = DependencyProperty.Register("Elements", typeof(ObservableCollection<EnumElement>), typeof(Enum));

		//System
		public bool IsTreeExpanded { get { return (bool)GetValue(IsTreeExpandedProperty); } set { SetValue(IsTreeExpandedProperty, value); } }
		public static readonly DependencyProperty IsTreeExpandedProperty = DependencyProperty.Register("IsTreeExpanded", typeof(bool), typeof(Enum));

		public Enum() : base(DataTypeMode.Enum)
		{
		}

		public Enum(string Name, Namespace Parent) : base(DataTypeMode.Enum)
		{
			Elements = new ObservableCollection<EnumElement>();
			ID = Guid.NewGuid();
			this.Name = Name;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			HasClientType = false;
			this.Parent = Parent;
			BaseType = new DataType(PrimitiveTypes.Int);
			Documentation = new Documentation { IsClass = true };
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == IsDirtyProperty) return;
			if (e.Property == IsTreeExpandedProperty) return;

			IsDirty = true;
		}

		public IEnumerable<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			var results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Enum || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Server Name", Name, Parent.Owner, this));
						if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) if (ClientType.Name.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Client Name", ClientType.Name, Parent.Owner, this));
					}
					else
					{
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Server Name", Name, Parent.Owner, this));
						if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) if (ClientType.Name.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Client Name", ClientType.Name, Parent.Owner, this));
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(Name)) if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Server Name", Name, Parent.Owner, this));
					if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) if (Args.RegexSearch.IsMatch(ClientType.Name)) results.Add(new FindReplaceResult("Client Name", ClientType.Name, Parent.Owner, this));
				}

				if (Args.ReplaceAll)
				{
					bool ia = IsActive;
					IsActive = true;
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) ClientType.Name = Microsoft.VisualBasic.Strings.Replace(ClientType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						else
						{
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
							if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) ClientType.Name = Microsoft.VisualBasic.Strings.Replace(ClientType.Name, Args.Search, Args.Replace);
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(Name)) Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) ClientType.Name = Args.RegexSearch.Replace(ClientType.Name, Args.Replace);
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
			if (!Args.ReplaceAll) return;
			bool ia = IsActive;
			IsActive = true;
			if (Args.UseRegex == false)
			{
				if (Args.MatchCase == false)
				{
					if (Field == "Server Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					if (HasClientType && Field == "Client Name") ClientType.Name = Microsoft.VisualBasic.Strings.Replace(ClientType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
				}
				else
				{
					if (Field == "Server Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
					if (HasClientType && Field == "Client Name") ClientType.Name = Microsoft.VisualBasic.Strings.Replace(ClientType.Name, Args.Search, Args.Replace);
				}
			}
			else
			{
				if (Field == "Server Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
				if (HasClientType && Field == "Client Name") ClientType.Name = Args.RegexSearch.Replace(ClientType.Name, Args.Replace);
			}
			IsActive = ia;
		}
	}

	public class EnumElement : DependencyObject
	{
		public Guid ID { get; set; }

		//Basic
		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value ?? "", @"")); } }
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

		public bool IsTreeExpanded { get { return false; } set { } }
		public Enum Owner { get; set; }

		public EnumElement()
		{
			this.ID = Guid.NewGuid();
			this.IsExcluded = false;
		}

		public EnumElement(string Name, string Value, string ContractValue, Enum Owner)
		{
			this.ID = Guid.NewGuid();
			this.Name = Name;
			this.Value = Value;
			this.ContractValue = ContractValue;
			this.IsExcluded = false;
			this.Owner = Owner;
		}

		public EnumElement(string Name, string Flags, Enum Owner)
		{
			this.ID = Guid.NewGuid();
			this.Name = Name;
			this.IsExcluded = false;
			this.Owner = Owner;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == OpenableDocument.IsDirtyProperty) return;

			if (Owner != null)
				Owner.IsDirty = true;
		}

		public override string ToString()
		{
			return Name;
		}

		public IEnumerable<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			var results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Enum || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) > 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
						if (!string.IsNullOrEmpty(Value)) if (Value.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) > 0) results.Add(new FindReplaceResult("Server Value", Value, Owner.Parent.Owner, this));
						if (!string.IsNullOrEmpty(ContractValue)) if (ContractValue.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) > 0) results.Add(new FindReplaceResult("Client Value", ContractValue, Owner.Parent.Owner, this));
					}
					else
					{
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCulture) > 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
						if (!string.IsNullOrEmpty(Value)) if (Value.IndexOf(Args.Search, StringComparison.CurrentCulture) > 0) results.Add(new FindReplaceResult("Server Value", Value, Owner.Parent.Owner, this));
						if (!string.IsNullOrEmpty(ContractValue)) if (ContractValue.IndexOf(Args.Search, StringComparison.CurrentCulture) > 0) results.Add(new FindReplaceResult("Client Value", ContractValue, Owner.Parent.Owner, this));
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(Name)) if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
					if (!string.IsNullOrEmpty(Value)) if (Args.RegexSearch.IsMatch(Value)) results.Add(new FindReplaceResult("Server Value", Value, Owner.Parent.Owner, this));
					if (!string.IsNullOrEmpty(ContractValue)) if (Args.RegexSearch.IsMatch(ContractValue)) results.Add(new FindReplaceResult("Client Value", ContractValue, Owner.Parent.Owner, this));
				}

				if (Args.ReplaceAll)
				{
					bool ia = Owner.IsActive;
					Owner.IsActive = true;
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (!string.IsNullOrEmpty(Value)) Value = Microsoft.VisualBasic.Strings.Replace(Value, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (!string.IsNullOrEmpty(ContractValue)) ContractValue = Microsoft.VisualBasic.Strings.Replace(ContractValue, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						else
						{
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
							if (!string.IsNullOrEmpty(Value)) Value = Microsoft.VisualBasic.Strings.Replace(Value, Args.Search, Args.Replace);
							if (!string.IsNullOrEmpty(ContractValue)) ContractValue = Microsoft.VisualBasic.Strings.Replace(ContractValue, Args.Search, Args.Replace);
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(Name)) Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (!string.IsNullOrEmpty(Value)) Value = Args.RegexSearch.Replace(Value, Args.Replace);
						if (!string.IsNullOrEmpty(ContractValue)) ContractValue = Args.RegexSearch.Replace(ContractValue, Args.Replace);
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
						if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						if (Field == "Server Value") Value = Microsoft.VisualBasic.Strings.Replace(Value, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						if (Field == "Client Value") ContractValue = Microsoft.VisualBasic.Strings.Replace(ContractValue, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					}
					else
					{
						if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
						if (Field == "Server Value") Value = Microsoft.VisualBasic.Strings.Replace(Value, Args.Search, Args.Replace);
						if (Field == "Client Value") ContractValue = Microsoft.VisualBasic.Strings.Replace(ContractValue, Args.Search, Args.Replace);
					}
				}
				else
				{
					if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
					if (Field == "Server Value") Value = Args.RegexSearch.Replace(Value, Args.Replace);
					if (Field == "Client Value") ContractValue = Args.RegexSearch.Replace(ContractValue, Args.Replace);
				}
				Owner.IsActive = ia;
			}
		}
	}
}