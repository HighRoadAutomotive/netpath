﻿using System;
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
		Disabled,
	}

	public class Data : DataType
	{
		public bool HasXAMLType { get { return (bool)GetValue(HasXAMLTypeProperty); } set { SetValue(HasXAMLTypeProperty, value); } }
		public static readonly DependencyProperty HasXAMLTypeProperty = DependencyProperty.Register("HasXAMLType", typeof(bool), typeof(Data), new PropertyMetadata(true, HasXAMLTypeChangedCallback));

		private static void HasXAMLTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as Data;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue) == false)
				t.XAMLType = null; 
			else
			{
				if (t.XAMLType == null)
				{
					t.XAMLType = Convert.ToBoolean(e.NewValue) == false ? null : new DataType(t.TypeMode) {ID = t.ID, Parent = t.Parent, Name = t.Name + "XAML", Scope = t.Scope, Partial = t.Partial, Abstract = t.Abstract, Sealed = t.Sealed};
					if (t.XAMLType != null) t.XAMLType.InheritedTypes.Add(new DataType("System.Windows.DependencyObject", DataTypeMode.Class));
				}
			}
		}

		public DataType XAMLType { get { return (DataType)GetValue(XAMLTypeProperty); } set { SetValue(XAMLTypeProperty, value); } }
		public static readonly DependencyProperty XAMLTypeProperty = DependencyProperty.Register("XAMLType", typeof(DataType), typeof(Data));

		public bool IsReference { get { return (bool)GetValue(IsReferenceProperty); } set { SetValue(IsReferenceProperty, value); } }
		public static readonly DependencyProperty IsReferenceProperty = DependencyProperty.Register("IsReference", typeof(bool), typeof(Data));

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(Data));

		public ObservableCollection<DataElement> Elements { get { return (ObservableCollection<DataElement>)GetValue(ElementsProperty); } set { SetValue(ElementsProperty, value); } }
		public static readonly DependencyProperty ElementsProperty = DependencyProperty.Register("Elements", typeof(ObservableCollection<DataElement>), typeof(Data));

		//System
		[IgnoreDataMember] public bool HasWinFormsBindings { get { return Elements.Any(a => a.GenerateWinFormsSupport); } }
		[IgnoreDataMember] public bool XAMLHasExtensionData { get { return HasXAMLType && (XAMLType.InheritedTypes.Any(a => a.Name.IndexOf("IExtensibleDataObject", StringComparison.CurrentCultureIgnoreCase) >= 0)); } }

		public Data() : base(DataTypeMode.Class)
		{
			Documentation = new Documentation { IsClass = true };
			IsDataObject = true;
		}

		public Data(string Name, Namespace Parent) : base(DataTypeMode.Class)
		{
			Elements = new ObservableCollection<DataElement>();
			ID = Guid.NewGuid();
			this.Name = Name;
			this.Name = Helpers.RegExs.ReplaceSpaces.Replace(Name, @"");
			HasClientType = false;
			HasXAMLType = true;
			IsDataObject = true;
			XAMLType = new DataType(DataTypeMode.Class) { Name = this.Name + "XAML", Scope = Scope, Parent = Parent };
			XAMLType.InheritedTypes.Add(new DataType("DependencyObject", DataTypeMode.Class));
			this.Parent = Parent;
			Documentation = new Documentation { IsClass = true };
		}

		public void AddKnownType(DataType Type, bool IsClientType = false, bool IsXAMLType = false)
		{
			if (IsClientType == false && IsXAMLType == false)
			{
				if (KnownTypes.Any(dt => dt.TypeName == Type.TypeName)) return;
				KnownTypes.Add(Type);
			}
			if (IsClientType && IsXAMLType == false)
			{
				if (ClientType.KnownTypes.Any(dt => dt.TypeName == Type.TypeName)) return;
				ClientType.KnownTypes.Add(Type);
			}
			if (IsClientType == false || IsXAMLType == false) return;
			if (XAMLType.KnownTypes.Any(dt => dt.TypeName == Type.TypeName)) return;
			XAMLType.KnownTypes.Add(Type);
		}

		public void RemoveKnownType(DataType Type, bool IsClientType = false, bool IsXAMLType = false)
		{
			if (IsClientType == false && IsXAMLType == false)
			{
				if (Elements.Any(dt => dt.DataType.TypeName == Type.TypeName)) return;
				var d = KnownTypes.FirstOrDefault(a => a.TypeName == Type.TypeName);
				if (d != null) KnownTypes.Remove(d);
			}
			if (IsClientType && IsXAMLType == false)
			{
				if (Elements.Any(dt => dt.ClientType.TypeName == Type.TypeName)) return;
				var d = KnownTypes.FirstOrDefault(a => a.TypeName == Type.TypeName);
				if (d != null) KnownTypes.Remove(d);
			}
			if (IsClientType == false || IsXAMLType == false) return;
			if (Elements.Any(dt => dt.XAMLType.TypeName == Type.TypeName)) return;
			var t = KnownTypes.FirstOrDefault(a => a.TypeName == Type.TypeName);
			if (t != null) KnownTypes.Remove(t);
		}

		public IEnumerable<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			var results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Data || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Server Name", Name, Parent.Owner, this));
						if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) if (ClientType.Name.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Client Name", ClientType.Name, Parent.Owner, this));
						if (HasXAMLType && !string.IsNullOrEmpty(XAMLType.Name)) if (XAMLType.Name.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("XAML Name", XAMLType.Name, Parent.Owner, this));
					}
					else
					{
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Server Name", Name, Parent.Owner, this));
						if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) if (ClientType.Name.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Client Name", ClientType.Name, Parent.Owner, this));
						if (HasXAMLType && !string.IsNullOrEmpty(XAMLType.Name)) if (XAMLType.Name.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("XAML Name", XAMLType.Name, Parent.Owner, this));
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(Name)) if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Server Name", Name, Parent.Owner, this));
					if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) if (Args.RegexSearch.IsMatch(ClientType.Name)) results.Add(new FindReplaceResult("Client Name", ClientType.Name, Parent.Owner, this));
					if (HasXAMLType && !string.IsNullOrEmpty(XAMLType.Name)) if (Args.RegexSearch.IsMatch(XAMLType.Name)) results.Add(new FindReplaceResult("XAML Name", XAMLType.Name, Parent.Owner, this));
				}

				if (Args.ReplaceAll)
				{
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) ClientType.Name = Microsoft.VisualBasic.Strings.Replace(ClientType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (HasXAMLType && !string.IsNullOrEmpty(XAMLType.Name)) XAMLType.Name = Microsoft.VisualBasic.Strings.Replace(XAMLType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						else
						{
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
							if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) ClientType.Name = Microsoft.VisualBasic.Strings.Replace(ClientType.Name, Args.Search, Args.Replace);
							if (HasXAMLType && !string.IsNullOrEmpty(XAMLType.Name)) XAMLType.Name = Microsoft.VisualBasic.Strings.Replace(XAMLType.Name, Args.Search, Args.Replace);
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(Name)) Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (HasClientType && !string.IsNullOrEmpty(ClientType.Name)) ClientType.Name = Args.RegexSearch.Replace(ClientType.Name, Args.Replace);
						if (HasXAMLType && !string.IsNullOrEmpty(XAMLType.Name)) XAMLType.Name = Args.RegexSearch.Replace(XAMLType.Name, Args.Replace);
					}
				}
			}

			foreach (DataElement de in Elements)
				results.AddRange(de.FindReplace(Args));

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
		{
			if (!Args.ReplaceAll) return;
			if (Args.UseRegex == false)
			{
				if (Args.MatchCase == false)
				{
					if (Field == "Server Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					if (Field == "Client Name") ClientType.Name = Microsoft.VisualBasic.Strings.Replace(ClientType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					if (Field == "XAML Name") XAMLType.Name = Microsoft.VisualBasic.Strings.Replace(XAMLType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
				}
				else
				{
					if (Field == "Server Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
					if (Field == "Client Name") ClientType.Name = Microsoft.VisualBasic.Strings.Replace(ClientType.Name, Args.Search, Args.Replace);
					if (Field == "XAML Name") XAMLType.Name = Microsoft.VisualBasic.Strings.Replace(XAMLType.Name, Args.Search, Args.Replace);
				}
			}
			else
			{
				if (Field == "Server Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
				if (Field == "Client Name") ClientType.Name = Args.RegexSearch.Replace(ClientType.Name, Args.Replace);
				if (Field == "XAML Name") XAMLType.Name = Args.RegexSearch.Replace(XAMLType.Name, Args.Replace);
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
		public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType", typeof(DataType), typeof(DataElement), new PropertyMetadata(DataTypeChangedCallback));

		private static void DataTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs p)
		{
			var de = o as DataElement;
			if (de == null) return;
			if (de.Owner == null) return;
			var nt = p.NewValue as DataType;
			if (nt == null) return;
			var ot = p.OldValue as DataType;
			if (ot == null) return;
			
			if (ot.TypeMode == DataTypeMode.Array && ot.CollectionGenericType.TypeMode == DataTypeMode.Primitive) de.Owner.RemoveKnownType(nt);
			if (nt.TypeMode == DataTypeMode.Array && nt.CollectionGenericType.TypeMode == DataTypeMode.Primitive) de.Owner.AddKnownType(nt);
			if (ot.TypeMode == DataTypeMode.Primitive && ot.Primitive == PrimitiveTypes.DateTimeOffset) de.Owner.RemoveKnownType(new DataType(PrimitiveTypes.DateTimeOffset));
			if (nt.TypeMode == DataTypeMode.Primitive && nt.Primitive == PrimitiveTypes.DateTimeOffset) de.Owner.AddKnownType(new DataType(PrimitiveTypes.DateTimeOffset));
		}

		public string DataName { get { return (string)GetValue(DataNameProperty); } set { SetValue(DataNameProperty, value); } }
		public static readonly DependencyProperty DataNameProperty = DependencyProperty.Register("DataName", typeof(string), typeof(DataElement), new PropertyMetadata(""));

		public bool HasClientType { get { return (bool)GetValue(HasClientTypeProperty); } set { SetValue(HasClientTypeProperty, value); } }
		public static readonly DependencyProperty HasClientTypeProperty = DependencyProperty.Register("HasClientType", typeof(bool), typeof(DataElement), new PropertyMetadata(false, HasClientTypeChangedCallback));

		private static void HasClientTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataElement;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue))
			{
				t.ClientScope = t.DataScope;
				t.ClientType = t.DataType;
				t.ClientName = t.DataName;
				if (t.ClientType == null || t.Owner == null) return;
				if (t.ClientType.TypeMode == DataTypeMode.Array && t.ClientType.CollectionGenericType.TypeMode == DataTypeMode.Primitive) t.Owner.AddKnownType(t.ClientType, true);
				if (t.ClientType.TypeMode == DataTypeMode.Primitive && t.ClientType.Primitive == PrimitiveTypes.DateTimeOffset) t.Owner.AddKnownType(new DataType(PrimitiveTypes.DateTimeOffset), true);
			}
			else
			{
				var tct = e.OldValue as DataType;
				if (tct != null)
				{
					if (tct.TypeMode == DataTypeMode.Array && tct.CollectionGenericType.TypeMode == DataTypeMode.Primitive) t.Owner.RemoveKnownType(tct, true);
					if (tct.TypeMode == DataTypeMode.Primitive && tct.Primitive == PrimitiveTypes.DateTimeOffset) t.Owner.RemoveKnownType(new DataType(PrimitiveTypes.DateTimeOffset), true);
				}
				t.ClientScope = DataScope.Disabled;
				t.ClientName = "";
				t.ClientType = null;
				t.IsAttached = false;
			}
		}

		public DataScope ClientScope { get { return (DataScope)GetValue(ClientScopeProperty); } set { SetValue(ClientScopeProperty, value); } }
		public static readonly DependencyProperty ClientScopeProperty = DependencyProperty.Register("ClientScope", typeof(DataScope), typeof(DataElement), new PropertyMetadata(DataScope.Public));

		public DataType ClientType { get { return (DataType)GetValue(ClientTypeProperty); } set { SetValue(ClientTypeProperty, value); } }
		public static readonly DependencyProperty ClientTypeProperty = DependencyProperty.Register("ClientType", typeof(DataType), typeof(DataElement), new PropertyMetadata(ClientTypeChangedCallback));

		private static void ClientTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs p)
		{
			var de = o as DataElement;
			if (de == null) return;
			var nt = p.NewValue as DataType;
			if (nt == null) return;
			var ot = p.OldValue as DataType;
			if (ot == null) return;

			if (ot.TypeMode == DataTypeMode.Array && ot.CollectionGenericType.TypeMode == DataTypeMode.Primitive) de.Owner.RemoveKnownType(nt, true);
			if (nt.TypeMode == DataTypeMode.Array && nt.CollectionGenericType.TypeMode == DataTypeMode.Primitive) de.Owner.AddKnownType(nt, true);
			if (ot.TypeMode == DataTypeMode.Primitive && ot.Primitive == PrimitiveTypes.DateTimeOffset) de.Owner.RemoveKnownType(new DataType(PrimitiveTypes.DateTimeOffset), true);
			if (nt.TypeMode == DataTypeMode.Primitive && nt.Primitive == PrimitiveTypes.DateTimeOffset) de.Owner.AddKnownType(new DataType(PrimitiveTypes.DateTimeOffset), true);
		}

		public string ClientName { get { return (string)GetValue(ClientNameProperty); } set { SetValue(ClientNameProperty, value); } }
		public static readonly DependencyProperty ClientNameProperty = DependencyProperty.Register("ClientName", typeof(string), typeof(DataElement), new PropertyMetadata(""));

		public bool HasXAMLType { get { return (bool)GetValue(HasXAMLTypeProperty); } set { SetValue(HasXAMLTypeProperty, value); } }
		public static readonly DependencyProperty HasXAMLTypeProperty = DependencyProperty.Register("HasXAMLType", typeof(bool), typeof(DataElement), new PropertyMetadata(false, HasXAMLTypeChangedCallback));

		private static void HasXAMLTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataElement;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue))
			{
				t.XAMLScope = t.DataScope;
				t.XAMLType = t.DataType;
				t.XAMLName = t.DataName;
			}
			else
			{
				t.XAMLScope = DataScope.Disabled;
				t.XAMLType = null;
				t.XAMLName = "";
				t.IsAttached = false;
			}
		}

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

		//DataMember Settings
		public bool IsDataMember { get { return (bool)GetValue(IsDataMemberProperty); } set { SetValue(IsDataMemberProperty, value); } }
		public static readonly DependencyProperty IsDataMemberProperty = DependencyProperty.Register("IsDataMember", typeof(bool), typeof(DataElement), new UIPropertyMetadata(true, IsDataMemberChangedCallback));

		private static void IsDataMemberChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataElement;
			if (t == null) return;
			if (Convert.ToBoolean(e.NewValue)) return;

			t.IsRequired = false;
			t.EmitDefaultValue = false;
			t.HasClientType = false;
			t.GenerateWinFormsSupport = false;
			t.HasXAMLType = false;
			t.IsAttached = false;
		}

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

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(DataElement));

		//System
		public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		public Data Owner { get; set; }

		public DataElement()
		{
			ID = Guid.NewGuid();
			DataType = new DataType(PrimitiveTypes.String);
			DataScope = DataScope.Public;
			ClientScope = DataScope.Disabled;
			XAMLScope = DataScope.Public;
			AttachedTargetTypes = "";
			AttachedAttributeTypes = "";
			EmitDefaultValue = false;
			Order = -1;
			Documentation = new Documentation { IsProperty = true };
			HasClientType = false;
			HasXAMLType = true;
		}

		public DataElement(DataScope Scope, DataType DataType, string Name, Data Owner)
		{
			ID = Guid.NewGuid();
			this.DataType = DataType;
			this.Owner = Owner;
			DataName = Name;
			DataScope = Scope;
			ClientScope = DataScope.Disabled;
			XAMLScope = DataScope.Public;
			AttachedTargetTypes = "";
			AttachedAttributeTypes = "";
			EmitDefaultValue = false;
			Order = -1;
			Documentation = new Documentation { IsProperty = true };
			HasClientType = false;
			HasXAMLType = true;
		}

		public override string ToString()
		{
			return DataName;
		}

		public IEnumerable<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			var results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Data || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (!string.IsNullOrEmpty(DataName)) if (DataName.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Server Name", DataName, Owner.Parent.Owner, this));
						if (HasClientType && !string.IsNullOrEmpty(ClientName)) if (ClientName.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Client Name", ClientName, Owner.Parent.Owner, this));
						if (HasXAMLType && !string.IsNullOrEmpty(XAMLName)) if (XAMLName.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("XAML Name", XAMLName, Owner.Parent.Owner, this));
					}
					else
					{
						if (!string.IsNullOrEmpty(DataName)) if (DataName.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Server Name", DataName, Owner.Parent.Owner, this));
						if (HasClientType && !string.IsNullOrEmpty(ClientName)) if (ClientName.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Client Name", ClientName, Owner.Parent.Owner, this));
						if (HasXAMLType && !string.IsNullOrEmpty(XAMLName)) if (XAMLName.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("XAML Name", XAMLName, Owner.Parent.Owner, this));
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(DataName)) if (Args.RegexSearch.IsMatch(DataName)) results.Add(new FindReplaceResult("Server Name", DataName, Owner.Parent.Owner, this));
					if (HasClientType && !string.IsNullOrEmpty(ClientName)) if (ClientName.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Client Name", ClientName, Owner.Parent.Owner, this));
					if (HasXAMLType && !string.IsNullOrEmpty(XAMLName)) if (Args.RegexSearch.IsMatch(XAMLName)) results.Add(new FindReplaceResult("XAML Name", XAMLName, Owner.Parent.Owner, this));
				}

				if (Args.ReplaceAll)
				{
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (!string.IsNullOrEmpty(DataName)) DataName = Microsoft.VisualBasic.Strings.Replace(DataName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (HasClientType && !string.IsNullOrEmpty(ClientName)) ClientName = Microsoft.VisualBasic.Strings.Replace(ClientName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (HasXAMLType && !string.IsNullOrEmpty(XAMLName)) XAMLName = Microsoft.VisualBasic.Strings.Replace(XAMLName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						else
						{
							if (!string.IsNullOrEmpty(DataName)) DataName = Microsoft.VisualBasic.Strings.Replace(DataName, Args.Search, Args.Replace);
							if (HasClientType && !string.IsNullOrEmpty(ClientName)) ClientName = Microsoft.VisualBasic.Strings.Replace(ClientName, Args.Search, Args.Replace);
							if (HasXAMLType && !string.IsNullOrEmpty(XAMLName)) XAMLName = Microsoft.VisualBasic.Strings.Replace(XAMLName, Args.Search, Args.Replace);
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(DataName)) DataName = Args.RegexSearch.Replace(DataName, Args.Replace);
						if (HasClientType && !string.IsNullOrEmpty(ClientName)) ClientName = Args.RegexSearch.Replace(ClientName, Args.Replace);
						if (HasXAMLType && !string.IsNullOrEmpty(XAMLName)) XAMLName = Args.RegexSearch.Replace(XAMLName, Args.Replace);
					}
				}
			}

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
		{
			if (!Args.ReplaceAll) return;
			if (Args.UseRegex == false)
			{
				if (Args.MatchCase == false)
				{
					if (Field == "Server Name") DataName = Microsoft.VisualBasic.Strings.Replace(DataName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					if (HasClientType && Field == "Client Name") ClientName = Microsoft.VisualBasic.Strings.Replace(ClientName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					if (HasXAMLType && Field == "XAML Name") XAMLName = Microsoft.VisualBasic.Strings.Replace(XAMLName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
				}
				else
				{
					if (Field == "Server Name") DataName = Microsoft.VisualBasic.Strings.Replace(DataName, Args.Search, Args.Replace);
					if (HasClientType && Field == "Client Name") ClientName = Microsoft.VisualBasic.Strings.Replace(ClientName, Args.Search, Args.Replace);
					if (HasXAMLType && Field == "XAML Name") XAMLName = Microsoft.VisualBasic.Strings.Replace(XAMLName, Args.Search, Args.Replace);
				}
			}
			else
			{
				if (Field == "Server Name") DataName = Args.RegexSearch.Replace(DataName, Args.Replace);
				if (HasClientType && Field == "Client Name") ClientName = Args.RegexSearch.Replace(ClientName, Args.Replace);
				if (HasXAMLType && Field == "XAML Name") XAMLName = Args.RegexSearch.Replace(XAMLName, Args.Replace);
			}
		}
	}
}