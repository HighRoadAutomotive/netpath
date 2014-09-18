using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;

namespace NETPath.Projects
{
	public enum DataScope
	{
		Public,
		Internal,
		Disabled,
	}

	public enum DataUpdateMode
	{
		None,
		Immediate,
		Batch
	}

	public enum ProtoBufDataFormat
	{
		Default,
		FixedSize,
		Group,
		TwosComplement,
		ZigZag,
	}

	public struct DataRevisionName
	{
		public readonly string Path;
		public readonly bool IsServer;
		public readonly bool IsServerAwaitable;
		public readonly bool IsClientAwaitable;
		public readonly Guid ProjectID;

		public DataRevisionName(string Path, bool IsServer, bool IsServerAwaitable, bool IsClientAwaitable, Guid ProjectID)
		{
			this.Path = Path;
			this.IsServer = IsServer;
			this.IsServerAwaitable = IsServerAwaitable;
			this.IsClientAwaitable = IsClientAwaitable;
			this.ProjectID = ProjectID;
		}
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
					t.XAMLType = Convert.ToBoolean(e.NewValue) == false ? null : new DataType(t.TypeMode) { ID = t.ID, Parent = t.Parent, Name = t.Name + "XAML", Scope = t.Scope, Partial = t.Partial, Abstract = t.Abstract, Sealed = t.Sealed };
					if (t.XAMLType != null) t.XAMLType.InheritedTypes.Add(new DataType(t.CMDEnabled ? "DependencyObjectEx" : "DependencyObject", DataTypeMode.Class));
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

		public bool HasEntity { get { return (bool)GetValue(HasEntityProperty); } set { SetValue(HasEntityProperty, value); } }
		public static readonly DependencyProperty HasEntityProperty = DependencyProperty.Register("HasEntity", typeof(bool), typeof(Data), new PropertyMetadata(false));

		public string EntityType { get { return (string)GetValue(EntityTypeProperty); } set { SetValue(EntityTypeProperty, value); } }
		public static readonly DependencyProperty EntityTypeProperty = DependencyProperty.Register("EntityType", typeof(string), typeof(Data), new PropertyMetadata(""));

		public string EntityContext { get { return (string)GetValue(EntityContextProperty); } set { SetValue(EntityContextProperty, value); } }
		public static readonly DependencyProperty EntityContextProperty = DependencyProperty.Register("EntityContext", typeof(string), typeof(Data), new PropertyMetadata(""));

		//Concurrently Mutable Data
		public bool CMDEnabled { get { return (bool)GetValue(CMDEnabledProperty); } set { SetValue(CMDEnabledProperty, value); } }
		public static readonly DependencyProperty CMDEnabledProperty = DependencyProperty.Register("CMDEnabled", typeof(bool), typeof(Data), new PropertyMetadata(false, CMDEnabledChangedCallback));

		private static void CMDEnabledChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as Data;
			if (t == null) return;
			if (!t.HasXAMLType) return;
			if (t.XAMLType == null) return;

			if (!Convert.ToBoolean(e.NewValue)) t.DREEnabled = false;

			DataType doex = t.XAMLType.InheritedTypes.FirstOrDefault(a => a.Name == "DependencyObjectEx");
			if (doex != null && Convert.ToBoolean(e.NewValue)) return;
			if (doex != null && !Convert.ToBoolean(e.NewValue)) doex.Name = "DependencyObject";
			DataType wdo = t.XAMLType.InheritedTypes.FirstOrDefault(a => a.Name == "DependencyObject");
			if (wdo != null && Convert.ToBoolean(e.NewValue)) wdo.Name = "DependencyObjectEx";
		}

		//Data Revision Exchange
		public bool DREEnabled { get { return (bool)GetValue(DREEnabledProperty); } set { SetValue(DREEnabledProperty, value); } }
		public static readonly DependencyProperty DREEnabledProperty = DependencyProperty.Register("DREEnabled", typeof(bool), typeof(Data), new PropertyMetadata(false));

		public int DREBatchCount { get { return (int)GetValue(DREBatchCountProperty); } set { SetValue(DREBatchCountProperty, value); } }
		public static readonly DependencyProperty DREBatchCountProperty = DependencyProperty.Register("DREBatchCount", typeof(int), typeof(Data), new PropertyMetadata(0));

		[IgnoreDataMember]
		public List<DataRevisionName> DataRevisionServiceNames { get; set; }

		//System
		[IgnoreDataMember]
		public bool HasWinFormsBindings { get { return Elements.Any(a => a.GenerateWinFormsSupport); } }
		[IgnoreDataMember]
		public bool XAMLHasExtensionData { get { return HasXAMLType && (XAMLType.InheritedTypes.Any(a => a.Name.IndexOf("IExtensibleDataObject", StringComparison.CurrentCultureIgnoreCase) >= 0)); } }

		public Data()
			: base(DataTypeMode.Class)
		{
			Documentation = new Documentation { IsClass = true };
			IsDataObject = true;
			DataRevisionServiceNames = new List<DataRevisionName>();
		}

		public Data(string Name, Namespace Parent)
			: base(DataTypeMode.Class)
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
			DataRevisionServiceNames = new List<DataRevisionName>();
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

		public void RemoveKnownType(DataType Type)
		{
			if (Elements.Any(dt => dt.DataType.TypeName == Type.TypeName)) return;
			var d = KnownTypes.FirstOrDefault(a => a.TypeName == Type.TypeName);
			if (d != null) KnownTypes.Remove(d);
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
		public DataType DataType { get { return (DataType)GetValue(DataTypeProperty); } set { SetValue(DataTypeProperty, value); } }
		public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType", typeof(DataType), typeof(DataElement), new PropertyMetadata(DataTypeChangedCallback));

		private static void DataTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs p)
		{
			var de = o as DataElement;
			if (de == null) return;

			var nt = p.NewValue as DataType;
			if (nt == null) return;

			de.CanPropertyChanged = true;
			de.CanPropertyUpdated = true;
			if (nt.TypeMode == DataTypeMode.Collection || nt.TypeMode == DataTypeMode.Dictionary || nt.TypeMode == DataTypeMode.Queue || nt.TypeMode == DataTypeMode.Stack)
			{
				de.CanPropertyChanged = false;
				de.PropertyChangedCallback = false;
				de.CanPropertyUpdated = false;
				de.PropertyUpdatedCallback = false;
			}
			de.DRECanBatch = (de.DataType.TypeMode == DataTypeMode.Collection || de.DataType.TypeMode == DataTypeMode.Dictionary);


			if (de.DataType.TypeMode == DataTypeMode.Primitive && (de.DataType.Primitive == PrimitiveTypes.Byte || de.DataType.Primitive == PrimitiveTypes.SByte || de.DataType.Primitive == PrimitiveTypes.Short || de.DataType.Primitive == PrimitiveTypes.Int || de.DataType.Primitive == PrimitiveTypes.Long || de.DataType.Primitive == PrimitiveTypes.UShort || de.DataType.Primitive == PrimitiveTypes.UInt || de.DataType.Primitive == PrimitiveTypes.ULong || de.DataType.Primitive == PrimitiveTypes.String || de.DataType.Primitive == PrimitiveTypes.GUID))
				de.DRECanPrimaryKey = true;
			else
			{
				de.DRECanPrimaryKey = false;
				de.DREPrimaryKey = false;
			}

			var ot = p.OldValue as DataType;
			if (ot == null) return;
			if (de.Owner == null) return;

			if (ot.TypeMode == DataTypeMode.Array && ot.CollectionGenericType.TypeMode == DataTypeMode.Primitive) de.Owner.RemoveKnownType(nt);
			if (nt.TypeMode == DataTypeMode.Array && nt.CollectionGenericType.TypeMode == DataTypeMode.Primitive) de.Owner.AddKnownType(nt);
			if (ot.TypeMode == DataTypeMode.Primitive && ot.Primitive == PrimitiveTypes.DateTimeOffset) de.Owner.RemoveKnownType(new DataType(PrimitiveTypes.DateTimeOffset));
			if (nt.TypeMode == DataTypeMode.Primitive && nt.Primitive == PrimitiveTypes.DateTimeOffset) de.Owner.AddKnownType(new DataType(PrimitiveTypes.DateTimeOffset));
		}

		public string DataName { get { return (string)GetValue(DataNameProperty); } set { SetValue(DataNameProperty, value); } }
		public static readonly DependencyProperty DataNameProperty = DependencyProperty.Register("DataName", typeof(string), typeof(DataElement), new PropertyMetadata(""));

		public bool HasContractName { get { return (bool)GetValue(HasContractNameProperty); } set { SetValue(HasContractNameProperty, value); } }
		public static readonly DependencyProperty HasContractNameProperty = DependencyProperty.Register("HasContractName", typeof(bool), typeof(DataElement), new PropertyMetadata(false, HasContractNameChangedCallback));

		private static void HasContractNameChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataElement;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue))
			{
				if (t.ContractName == "") t.ContractName = t.DataName;
			}
			else
			{
				t.HasClientType = false;
				t.ContractName = "";
			}
		}

		public string ContractName { get { return (string)GetValue(ContractNameProperty); } set { SetValue(ContractNameProperty, value); } }
		public static readonly DependencyProperty ContractNameProperty = DependencyProperty.Register("ContractName", typeof(string), typeof(DataElement), new PropertyMetadata(""));

		public bool HasClientType { get { return (bool)GetValue(HasClientTypeProperty); } set { SetValue(HasClientTypeProperty, value); } }
		public static readonly DependencyProperty HasClientTypeProperty = DependencyProperty.Register("HasClientType", typeof(bool), typeof(DataElement), new PropertyMetadata(false, HasClientTypeChangedCallback));

		private static void HasClientTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataElement;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue))
			{
				if (t.ClientName == "") t.ClientName = t.DataName;
				t.HasContractName = true;
			}
			else
			{
				var tct = e.OldValue as DataType;
				if (tct != null)
				{
					if (tct.TypeMode == DataTypeMode.Array && tct.CollectionGenericType.TypeMode == DataTypeMode.Primitive) t.Owner.RemoveKnownType(tct);
					if (tct.TypeMode == DataTypeMode.Primitive && tct.Primitive == PrimitiveTypes.DateTimeOffset) t.Owner.RemoveKnownType(new DataType(PrimitiveTypes.DateTimeOffset));
				}
				t.ClientName = "";
			}
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
				if (t.XAMLName == "") t.XAMLName = t.DataName;
			}
			else
			{
				t.XAMLName = "";
			}
		}

		public string XAMLName { get { return (string)GetValue(XAMLNameProperty); } set { SetValue(XAMLNameProperty, value); } }
		public static readonly DependencyProperty XAMLNameProperty = DependencyProperty.Register("XAMLName", typeof(string), typeof(DataElement), new PropertyMetadata(""));

		public bool HasEntity { get { return (bool)GetValue(HasEntityProperty); } set { SetValue(HasEntityProperty, value); } }
		public static readonly DependencyProperty HasEntityProperty = DependencyProperty.Register("HasEntity", typeof(bool), typeof(DataElement), new PropertyMetadata(false, HasEntityChangedCallback));

		private static void HasEntityChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataElement;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue))
			{
				if (t.EntityName == "") t.EntityName = t.DataName;
			}
			else
			{
				t.EntityName = "";
			}
		}

		public string EntityName { get { return (string)GetValue(EntityNameProperty); } set { SetValue(EntityNameProperty, value); } }
		public static readonly DependencyProperty EntityNameProperty = DependencyProperty.Register("EntityName", typeof(string), typeof(DataElement), new PropertyMetadata(""));

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
		}

		public bool EmitDefaultValue { get { return (bool)GetValue(EmitDefaultValueProperty); } set { SetValue(EmitDefaultValueProperty, value); } }
		public static readonly DependencyProperty EmitDefaultValueProperty = DependencyProperty.Register("EmitDefaultValue", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		public bool IsRequired { get { return (bool)GetValue(IsRequiredProperty); } set { SetValue(IsRequiredProperty, value); } }
		public static readonly DependencyProperty IsRequiredProperty = DependencyProperty.Register("IsRequired", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		public int Order { get { return (int)GetValue(OrderProperty); } set { SetValue(OrderProperty, value); } }
		public static readonly DependencyProperty OrderProperty = DependencyProperty.Register("Order", typeof(int), typeof(DataElement), new PropertyMetadata(-1));

		//Callbacks
		[IgnoreDataMember]
		public bool CanPropertyChanged { get { return (bool)GetValue(CanPropertyChangedProperty); } protected set { SetValue(CanPropertyChangedPropertyKey, value); } }
		private static readonly DependencyPropertyKey CanPropertyChangedPropertyKey = DependencyProperty.RegisterReadOnly("CanPropertyChanged", typeof(bool), typeof(DataElement), new PropertyMetadata(false));
		public static readonly DependencyProperty CanPropertyChangedProperty = CanPropertyChangedPropertyKey.DependencyProperty;

		[IgnoreDataMember]
		public bool CanPropertyUpdated { get { return (bool)GetValue(CanPropertyUpdatedProperty); } protected set { SetValue(CanPropertyUpdatedPropertyKey, value); } }
		private static readonly DependencyPropertyKey CanPropertyUpdatedPropertyKey = DependencyProperty.RegisterReadOnly("CanPropertyUpdated", typeof(bool), typeof(DataElement), new PropertyMetadata(false));
		public static readonly DependencyProperty CanPropertyUpdatedProperty = CanPropertyUpdatedPropertyKey.DependencyProperty;

		public bool PropertyChangedCallback { get { return (bool)GetValue(PropertyChangedCallbackProperty); } set { SetValue(PropertyChangedCallbackProperty, value); } }
		public static readonly DependencyProperty PropertyChangedCallbackProperty = DependencyProperty.Register("PropertyChangedCallback", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		public bool PropertyUpdatedCallback { get { return (bool)GetValue(PropertyUpdatedCallbackProperty); } set { SetValue(PropertyUpdatedCallbackProperty, value); } }
		public static readonly DependencyProperty PropertyUpdatedCallbackProperty = DependencyProperty.Register("PropertyUpdatedCallback", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		public bool PropertyValidationCallback { get { return (bool)GetValue(PropertyValidationCallbackProperty); } set { SetValue(PropertyValidationCallbackProperty, value); } }
		public static readonly DependencyProperty PropertyValidationCallbackProperty = DependencyProperty.Register("PropertyValidationCallback", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		//XAML Class Settings
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

		//Date Revision Exchange
		public bool DREEnabled { get { return (bool)GetValue(DREEnabledProperty); } set { SetValue(DREEnabledProperty, value); } }
		public static readonly DependencyProperty DREEnabledProperty = DependencyProperty.Register("DREEnabled", typeof(bool), typeof(DataElement), new PropertyMetadata(false, DREEnabledChangedCallback));

		private static void DREEnabledChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataElement;
			if (t == null) return;

			if (t.DataType.TypeMode == DataTypeMode.Primitive && (t.DataType.Primitive == PrimitiveTypes.Byte || t.DataType.Primitive == PrimitiveTypes.SByte || t.DataType.Primitive == PrimitiveTypes.Short || t.DataType.Primitive == PrimitiveTypes.Int || t.DataType.Primitive == PrimitiveTypes.Long || t.DataType.Primitive == PrimitiveTypes.UShort || t.DataType.Primitive == PrimitiveTypes.UInt || t.DataType.Primitive == PrimitiveTypes.ULong || t.DataType.Primitive == PrimitiveTypes.String || t.DataType.Primitive == PrimitiveTypes.GUID))
				t.DRECanPrimaryKey = true;
			else
			{
				t.DRECanPrimaryKey = false;
				t.DREPrimaryKey = false;
				t.DREUpdateMode = DataUpdateMode.Immediate;
			}
		}

		[IgnoreDataMember]
		public bool DRECanPrimaryKey { get { return (bool)GetValue(DRECanPrimaryKeyProperty); } protected set { SetValue(DRECanPrimaryKeyPropertyKey, value); } }
		private static readonly DependencyPropertyKey DRECanPrimaryKeyPropertyKey = DependencyProperty.RegisterReadOnly("DRECanPrimaryKey", typeof(bool), typeof(DataElement), new PropertyMetadata(false));
		public static readonly DependencyProperty DRECanPrimaryKeyProperty = DRECanPrimaryKeyPropertyKey.DependencyProperty;

		public bool DREPrimaryKey { get { return (bool)GetValue(DREPrimaryKeyProperty); } set { SetValue(DREPrimaryKeyProperty, value); } }
		public static readonly DependencyProperty DREPrimaryKeyProperty = DependencyProperty.Register("DREPrimaryKey", typeof(bool), typeof(DataElement), new PropertyMetadata(false, DREPrimaryKeyChangedCallback));

		private static void DREPrimaryKeyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataElement;
			if (t == null) return;
			if (t.Owner == null) return;
			if (!Convert.ToBoolean(e.NewValue)) return;

			foreach (var x in t.Owner.Elements)
				if (x.ID != t.ID) x.DREPrimaryKey = false;
		}

		public DataUpdateMode DREUpdateMode { get { return (DataUpdateMode)GetValue(DREUpdateModeProperty); } set { SetValue(DREUpdateModeProperty, value); } }
		public static readonly DependencyProperty DREUpdateModeProperty = DependencyProperty.Register("DREUpdateMode", typeof(DataUpdateMode), typeof(DataElement), new PropertyMetadata(DataUpdateMode.Immediate));

		[IgnoreDataMember]
		public bool DRECanBatch { get { return (bool)GetValue(DRECanBatchProperty); } protected set { SetValue(DRECanBatchPropertyKey, value); } }
		private static readonly DependencyPropertyKey DRECanBatchPropertyKey = DependencyProperty.RegisterReadOnly("DRECanBatch", typeof(bool), typeof(DataElement), new PropertyMetadata(false));
		public static readonly DependencyProperty DRECanBatchProperty = DRECanBatchPropertyKey.DependencyProperty;

		public int DREBatchCount { get { return (int)GetValue(DREBatchCountProperty); } set { SetValue(DREBatchCountProperty, value); } }
		public static readonly DependencyProperty DREBatchCountProperty = DependencyProperty.Register("DREBatchCount", typeof(int), typeof(DataElement), new PropertyMetadata(1));

		//System
		public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(DataElement), new PropertyMetadata(false));

		public Data Owner { get; set; }

		public DataElement()
		{
			ID = Guid.NewGuid();
			DataType = new DataType(PrimitiveTypes.String);
			AttachedTargetTypes = "";
			AttachedAttributeTypes = "";
			EmitDefaultValue = false;
			Order = -1;
			Documentation = new Documentation { IsProperty = true };
			HasClientType = false;
			HasXAMLType = true;
			HasXAMLType = false;
		}

		public DataElement(DataType DataType, string Name, Data Owner)
		{
			ID = Guid.NewGuid();
			this.DataType = DataType;
			this.Owner = Owner;
			DataName = Name;
			AttachedTargetTypes = "";
			AttachedAttributeTypes = "";
			EmitDefaultValue = false;
			Order = -1;
			Documentation = new Documentation { IsProperty = true };
			HasClientType = Owner.HasClientType;
			HasXAMLType = Owner.HasXAMLType;
			HasEntity = Owner.HasEntity;
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