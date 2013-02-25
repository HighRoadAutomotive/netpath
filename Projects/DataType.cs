﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Serialization;

namespace NETPath.Projects
{
	public enum DataTypeMode
	{
		Primitive,
		Namespace,
		Enum,
		Class,
		Struct,
		Interface,
		Array,
		Collection,
		Dictionary,
		Stack,
		Queue,
	}

	public enum PrimitiveTypes
	{
		None,
		Void,
		Object,
		Byte,
		SByte,
		Short,
		Int,
		Long,
		UShort,
		UInt,
		ULong,
		Float,
		Double,
		Decimal,
		Bool,
		Char,
		String,
		DateTime,
		DateTimeOffset,
		TimeSpan,
		GUID,
		URI,
		Version,
		ByteArray
	}

	[Flags]
	public enum SupportedFrameworks
	{
		None = 0x0,
		NET30 = 0x1,
		NET35 = 0x2,
		NET40 = 0x4,
		NET45 = 0x8,
		SL40 = 0x10,
		SL50 = 0x20,
		WIN8 = 0x40
	}

	public class DataType : OpenableDocument
	{
		public Guid ID { get; set; }

		public bool HasClientType { get { return (bool)GetValue(HasClientTypeProperty); } set { SetValue(HasClientTypeProperty, value); } }
		public static readonly DependencyProperty HasClientTypeProperty = DependencyProperty.Register("HasClientType", typeof(bool), typeof(DataType), new PropertyMetadata(false, HasClientTypeChangedCallback));

		private static void HasClientTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataType;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue) == false)
				t.ClientType = null;
			else
			{
				if (t.ClientType == null)
				{
					t.ClientType = Convert.ToBoolean(e.NewValue) == false ? null : new DataType(t.TypeMode) { ID = t.ID, Parent = t.Parent, Name = t.Name, Scope = t.Scope, Partial = t.Partial, Abstract = t.Abstract, Sealed = t.Sealed };
					if (t.ClientType != null)
					{
						if (t.IsDataObject) t.ClientType.InheritedTypes.Add(new DataType("System.Runtime.Serialization.IExtensibleDataObject", DataTypeMode.Interface));
						t.ClientType.KnownTypes = new ObservableCollection<DataType>(t.KnownTypes);
					}
				}
			}
		}

		public DataType ClientType { get { return (DataType)GetValue(ClientTypeProperty); } set { SetValue(ClientTypeProperty, value); } }
		public static readonly DependencyProperty ClientTypeProperty = DependencyProperty.Register("ClientType", typeof(DataType), typeof(DataType));

		public DataScope Scope { get { return (DataScope)GetValue(ScopeProperty); } set { SetValue(ScopeProperty, value); } }
		public static readonly DependencyProperty ScopeProperty = DependencyProperty.Register("Scope", typeof(DataScope), typeof(DataType), new PropertyMetadata(DataScope.Public, DataTypePropertyChangedCallback));

		public bool Partial { get { return (bool)GetValue(PartialProperty); } set { SetValue(PartialProperty, value); } }
		public static readonly DependencyProperty PartialProperty = DependencyProperty.Register("Partial", typeof(bool), typeof(DataType), new PropertyMetadata(true, DataTypePropertyChangedCallback));

		public bool Abstract { get { return (bool)GetValue(AbstractProperty); } set { SetValue(AbstractProperty, value); } }
		public static readonly DependencyProperty AbstractProperty = DependencyProperty.Register("Abstract", typeof(bool), typeof(DataType), new PropertyMetadata(false, DataTypePropertyChangedCallback));

		public bool Sealed { get { return (bool)GetValue(SealedProperty); } set { SetValue(SealedProperty, value); } }
		public static readonly DependencyProperty SealedProperty = DependencyProperty.Register("Sealed", typeof(bool), typeof(DataType), new PropertyMetadata(false, DataTypePropertyChangedCallback));

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(DataType), new PropertyMetadata("", NameChangedCallback));

		private static void NameChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataType;
			if (t == null) return;

			t.Name = Helpers.RegExs.ReplaceSpaces.Replace(Convert.ToString(e.NewValue), @"");
			t.TypeName = t.ToString();
			t.Declaration = t.ToDeclarationString();
		}

		public ObservableCollection<DataType> InheritedTypes { get { return (ObservableCollection<DataType>)GetValue(InheritedTypesProperty); } set { SetValue(InheritedTypesProperty, value); } }
		public static readonly DependencyProperty InheritedTypesProperty = DependencyProperty.Register("InheritedTypes", typeof(ObservableCollection<DataType>), typeof(DataType));

		public ObservableCollection<DataType> KnownTypes { get { return (ObservableCollection<DataType>)GetValue(KnownTypesProperty); } set { SetValue(KnownTypesProperty, value); } }
		public static readonly DependencyProperty KnownTypesProperty = DependencyProperty.Register("KnownTypes", typeof(ObservableCollection<DataType>), typeof(DataType));

		public DataType CollectionGenericType { get { return (DataType)GetValue(CollectionGenericTypeProperty); } set { SetValue(CollectionGenericTypeProperty, value); } }
		public static readonly DependencyProperty CollectionGenericTypeProperty = DependencyProperty.Register("CollectionGenericType", typeof(DataType), typeof(DataType), new PropertyMetadata(null, DataTypePropertyChangedCallback));

		public DataType DictionaryKeyGenericType { get { return (DataType)GetValue(DictionaryKeyGenericTypeProperty); } set { SetValue(DictionaryKeyGenericTypeProperty, value); } }
		public static readonly DependencyProperty DictionaryKeyGenericTypeProperty = DependencyProperty.Register("DictionaryKeyGenericType", typeof(DataType), typeof(DataType), new PropertyMetadata(null, DataTypePropertyChangedCallback));

		public DataType DictionaryValueGenericType { get { return (DataType)GetValue(DictionaryValueGenericTypeProperty); } set { SetValue(DictionaryValueGenericTypeProperty, value); } }
		public static readonly DependencyProperty DictionaryValueGenericTypeProperty = DependencyProperty.Register("DictionaryValueGenericType", typeof(DataType), typeof(DataType), new PropertyMetadata(null, DataTypePropertyChangedCallback));

		public Namespace Parent { get { return (Namespace)GetValue(ParentProperty); } set { SetValue(ParentProperty, value); } }
		public static readonly DependencyProperty ParentProperty = DependencyProperty.Register("Parent", typeof(Namespace), typeof(DataType));

		[IgnoreDataMember] public string TypeName { get { return (string)GetValue(TypeNameProperty); } private set { SetValue(TypeNamePropertyKey, value); } }
		private static readonly DependencyPropertyKey TypeNamePropertyKey = DependencyProperty.RegisterReadOnly("TypeName", typeof(string), typeof(DataType), new PropertyMetadata(""));
		public static readonly DependencyProperty TypeNameProperty = TypeNamePropertyKey.DependencyProperty;

		[IgnoreDataMember] public string Declaration { get { return (string)GetValue(DeclarationProperty); } private set { SetValue(DeclarationPropertyKey, value); } }
		private static readonly DependencyPropertyKey DeclarationPropertyKey = DependencyProperty.RegisterReadOnly("Declaration", typeof(string), typeof(DataType), new PropertyMetadata(""));
		public static readonly DependencyProperty DeclarationProperty = DeclarationPropertyKey.DependencyProperty;

		public DataTypeMode TypeMode { get { return (DataTypeMode)GetValue(TypeModeProperty); } set { SetValue(TypeModeProperty, value); } }
		public static readonly DependencyProperty TypeModeProperty = DependencyProperty.Register("TypeMode", typeof(DataTypeMode), typeof(DataType), new PropertyMetadata(DataTypeMode.Class));

		public bool IsExternalType { get; set; }
		public PrimitiveTypes Primitive { get; set; }
		public bool IsTypeReference { get; set; }
		public SupportedFrameworks SupportedFrameworks { get; set; }

		[IgnoreDataMember] public bool DataHasExtensionData { get { return InheritedTypes.Any(a => a.Name.IndexOf("IExtensibleDataObject", StringComparison.CurrentCultureIgnoreCase) >= 0); } }
		[IgnoreDataMember] public bool ClientHasExtensionData { get { return HasClientType && (ClientType.InheritedTypes.Any(a => a.Name.IndexOf("IExtensibleDataObject", StringComparison.CurrentCultureIgnoreCase) >= 0)); } }
		[IgnoreDataMember] public bool ClientHasImpliedExtensionData { get { return !HasClientType; } }
		[IgnoreDataMember] public bool IsDataObject { get; set; }
		[IgnoreDataMember] public bool IsCollectionType { get { return TypeMode == DataTypeMode.Array || TypeMode == DataTypeMode.Collection || TypeMode == DataTypeMode.Dictionary || TypeMode == DataTypeMode.Queue || TypeMode == DataTypeMode.Stack; } }

		private static void DataTypePropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataType;
			if (t == null) return;

			t.TypeName = t.ToString();
			t.Declaration = t.ToDeclarationString();
		}

		internal DataType()
		{
			Name = "";
			Scope = DataScope.Public;
			TypeMode = DataTypeMode.Class;
			Partial = true;
			Primitive = PrimitiveTypes.None;
			HasClientType = false;
			IsExternalType = false;
			IsDataObject = false;
			InheritedTypes = new ObservableCollection<DataType>();
			KnownTypes = new ObservableCollection<DataType>();
			SupportedFrameworks = SupportedFrameworks.None;
		}

		public DataType(DataTypeMode Mode)
		{
			if (Mode == DataTypeMode.Primitive) throw new ArgumentException("Cannot use DataTypeMode.Primitive without specifying a Primitive. Please use the Primitive Type constructor.");
			if (Mode == DataTypeMode.Interface) throw new ArgumentException("Cannot use DataTypeMode.Interface without specifying an external Interface. Please use the External Type constructor.");
			if (Mode == DataTypeMode.Collection) throw new ArgumentException("Cannot use DataTypeMode.Collection without specifying an external Collection. Please use the External Type constructor.");
			if (Mode == DataTypeMode.Dictionary) throw new ArgumentException("Cannot use DataTypeMode.Dictionary without specifying an external Dictionary. Please use the External Type constructor.");
			Scope = DataScope.Public;
			TypeMode = Mode;
			Partial = (Mode == DataTypeMode.Class || Mode == DataTypeMode.Struct);
			Primitive = PrimitiveTypes.None;
			HasClientType = false;
			IsExternalType = false;
			IsDataObject = false;
			InheritedTypes = new ObservableCollection<DataType>();
			KnownTypes = new ObservableCollection<DataType>();
			SupportedFrameworks = SupportedFrameworks.None;
		}

		public DataType(PrimitiveTypes Primitive)
		{
			if (Primitive == PrimitiveTypes.None) throw new ArgumentException("Cannot set PrimitiveTypes.None using the Primitive DataType constructor. Please use the Mode Type constructor.");
			this.Primitive = Primitive;
			Scope = DataScope.Public;
			Partial = false;
			HasClientType = false;
			TypeMode = DataTypeMode.Primitive;
			IsExternalType = false;
			IsDataObject = false;
			SupportedFrameworks = SupportedFrameworks.None;
			if (TypeMode != DataTypeMode.Primitive) return;
			if (this.Primitive == PrimitiveTypes.Void) Name = "void";
			if (this.Primitive == PrimitiveTypes.Object) Name = "object";
			if (this.Primitive == PrimitiveTypes.Byte) Name = "byte";
			if (this.Primitive == PrimitiveTypes.SByte) Name = "sbyte";
			if (this.Primitive == PrimitiveTypes.Short) Name = "short";
			if (this.Primitive == PrimitiveTypes.Int) Name = "int";
			if (this.Primitive == PrimitiveTypes.Long) Name = "long";
			if (this.Primitive == PrimitiveTypes.UShort) Name = "ushort";
			if (this.Primitive == PrimitiveTypes.UInt) Name = "uint";
			if (this.Primitive == PrimitiveTypes.ULong) Name = "ulong";
			if (this.Primitive == PrimitiveTypes.Float) Name = "float";
			if (this.Primitive == PrimitiveTypes.Double) Name = "double";
			if (this.Primitive == PrimitiveTypes.Decimal) Name = "decimal";
			if (this.Primitive == PrimitiveTypes.Bool) Name = "bool";
			if (this.Primitive == PrimitiveTypes.Char) Name = "char";
			if (this.Primitive == PrimitiveTypes.String) Name = "string";
			if (this.Primitive == PrimitiveTypes.DateTime) Name = "DateTime";
			if (this.Primitive == PrimitiveTypes.DateTimeOffset) Name = "DateTimeOffset";
			if (this.Primitive == PrimitiveTypes.TimeSpan) Name = "TimeSpan";
			if (this.Primitive == PrimitiveTypes.GUID) Name = "Guid";
			if (this.Primitive == PrimitiveTypes.URI) Name = "Uri";
			if (this.Primitive == PrimitiveTypes.Version) Name = "Version";
			if (this.Primitive == PrimitiveTypes.ByteArray) Name = "byte[]";
		}

		public DataType(string External, DataTypeMode ExternalType, SupportedFrameworks SupportedFrameworks = SupportedFrameworks.None)
		{
			if (ExternalType == DataTypeMode.Primitive) throw new ArgumentException("Cannot use DataTypeMode.Primitive as an external type. Please use the Primitive Type constructor.");
			if (ExternalType == DataTypeMode.Namespace) throw new ArgumentException("Cannot use DataTypeMode.Namespace as an external type. Please use the Mode Type constructor.");
			if (ExternalType == DataTypeMode.Array) throw new ArgumentException("Cannot use DataTypeMode.Array as an external type.");
			Name = External;
			Scope = DataScope.Public;
			Partial = false;
			IsExternalType = true;
			IsDataObject = false;
			TypeMode = ExternalType;
			HasClientType = false;
			this.SupportedFrameworks = SupportedFrameworks;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == DeclarationProperty || e.Property == TypeNameProperty) return;
			TypeName = ToString();
			Declaration = ToDeclarationString();
		}

		public bool IsFrameworkSupported(ProjectGenerationFramework CurrentFramework)
		{
			if (SupportedFrameworks == SupportedFrameworks.None) return true;
			if (CurrentFramework == ProjectGenerationFramework.NET30 || CurrentFramework == ProjectGenerationFramework.NET35 || CurrentFramework == ProjectGenerationFramework.NET35Client || CurrentFramework == ProjectGenerationFramework.NET40 || CurrentFramework == ProjectGenerationFramework.NET40Client || CurrentFramework == ProjectGenerationFramework.NET45)
			{
				if (CurrentFramework == ProjectGenerationFramework.NET30) return SupportedFrameworks.HasFlag(SupportedFrameworks.NET30);
				if (CurrentFramework == ProjectGenerationFramework.NET35) return SupportedFrameworks.HasFlag(SupportedFrameworks.NET35);
				if (CurrentFramework == ProjectGenerationFramework.NET35Client) return SupportedFrameworks.HasFlag(SupportedFrameworks.NET35);
				if (CurrentFramework == ProjectGenerationFramework.NET40) return SupportedFrameworks.HasFlag(SupportedFrameworks.NET40);
				if (CurrentFramework == ProjectGenerationFramework.NET40Client) return SupportedFrameworks.HasFlag(SupportedFrameworks.NET40);
				if (CurrentFramework == ProjectGenerationFramework.NET45) return SupportedFrameworks.HasFlag(SupportedFrameworks.NET45);
			}
			else if (CurrentFramework == ProjectGenerationFramework.SL40 || CurrentFramework == ProjectGenerationFramework.SL50)
			{
				if (CurrentFramework == ProjectGenerationFramework.SL40) return SupportedFrameworks.HasFlag(SupportedFrameworks.SL40);
				if (CurrentFramework == ProjectGenerationFramework.SL50) return SupportedFrameworks.HasFlag(SupportedFrameworks.SL50);
			}
			else
			{
				if (CurrentFramework == ProjectGenerationFramework.WIN8) return SupportedFrameworks.HasFlag(SupportedFrameworks.WIN8);
			}
			return true;
		}

		public string ToScopeString()
		{
			if (Scope == DataScope.Public) return "public";
			if (Scope == DataScope.Internal) return "internal";
			return "public";
		}

		public string ToInheritedString()
		{
			if (InheritedTypes == null || InheritedTypes.Count == 0) return "";
			var sb = new StringBuilder(" : ");
			foreach (DataType t in InheritedTypes)
				sb.AppendFormat("{0}, ", t);
			sb.Remove(sb.Length - 2, 2);
			return sb.ToString();
		}

		public string ToStorageClassString()
		{
			if (TypeMode == DataTypeMode.Namespace) return "namespace";
			if (TypeMode == DataTypeMode.Class) return "class";
			if (TypeMode == DataTypeMode.Struct) return "struct";
			if (TypeMode == DataTypeMode.Enum) return "enum";
			return "";
		}

		public string ToDeclarationString()
		{
			if (Name == null) return "";

			if (TypeMode == DataTypeMode.Primitive)
			{
				if (Primitive == PrimitiveTypes.Void) return string.Format("{0} void", ToScopeString());
				if (Primitive == PrimitiveTypes.Object) return string.Format("{0} object", ToScopeString());
				if (Primitive == PrimitiveTypes.Byte) return string.Format("{0} byte", ToScopeString());
				if (Primitive == PrimitiveTypes.SByte) return string.Format("{0} sbyte", ToScopeString());
				if (Primitive == PrimitiveTypes.Short) return string.Format("{0} short", ToScopeString());
				if (Primitive == PrimitiveTypes.Int) return string.Format("{0} int", ToScopeString());
				if (Primitive == PrimitiveTypes.Long) return string.Format("{0} long", ToScopeString());
				if (Primitive == PrimitiveTypes.UShort) return string.Format("{0} ushort", ToScopeString());
				if (Primitive == PrimitiveTypes.UInt) return string.Format("{0} uint", ToScopeString());
				if (Primitive == PrimitiveTypes.ULong) return string.Format("{0} ulong", ToScopeString());
				if (Primitive == PrimitiveTypes.Float) return string.Format("{0} float", ToScopeString());
				if (Primitive == PrimitiveTypes.Double) return string.Format("{0} double", ToScopeString());
				if (Primitive == PrimitiveTypes.Decimal) return string.Format("{0} decimal", ToScopeString());
				if (Primitive == PrimitiveTypes.Bool) return string.Format("{0} bool", ToScopeString());
				if (Primitive == PrimitiveTypes.Char) return string.Format("{0} char", ToScopeString());
				if (Primitive == PrimitiveTypes.String) return string.Format("{0} string", ToScopeString());
				if (Primitive == PrimitiveTypes.DateTime) return string.Format("{0} DateTime", ToScopeString());
				if (Primitive == PrimitiveTypes.DateTimeOffset) return string.Format("{0} DateTimeOffset", ToScopeString());
				if (Primitive == PrimitiveTypes.TimeSpan) return string.Format("{0} TimeSpan", ToScopeString());
				if (Primitive == PrimitiveTypes.GUID) return string.Format("{0} Guid", ToScopeString());
				if (Primitive == PrimitiveTypes.URI) return string.Format("{0} Uri", ToScopeString());
				if (Primitive == PrimitiveTypes.Version) return string.Format("{0} Version", ToScopeString());
				if (Primitive == PrimitiveTypes.ByteArray) return string.Format("{0} byte[]", ToScopeString());
			}
			else if (TypeMode == DataTypeMode.Namespace)
				return Parent == null ? string.Format("namespace {0}", Name) : string.Format("namespace {0}.{1}", Parent.FullName, Name);
			else if (IsExternalType == false && (TypeMode == DataTypeMode.Class || TypeMode == DataTypeMode.Struct || TypeMode == DataTypeMode.Enum))
				return string.Format("{0} {1}{2}{3}{4} {5}{6}", ToScopeString(), Partial ? "partial " : "", Abstract ? "abstract " : "", Sealed ? "sealed " : "", ToStorageClassString(), Name, ToInheritedString());
			else if (TypeMode == DataTypeMode.Array)
				return string.Format("{0} {1}[] {2}", ToScopeString(), CollectionGenericType, Name);
			else if (TypeMode == DataTypeMode.Collection || TypeMode == DataTypeMode.Stack || TypeMode == DataTypeMode.Queue)
				return string.Format("{0} {1}<{2}>", ToScopeString(), Name, CollectionGenericType);
			else if (TypeMode == DataTypeMode.Dictionary)
				return string.Format("{0} {1}<{2}, {3}>", ToScopeString(), Name, DictionaryKeyGenericType, DictionaryValueGenericType);
			else if (IsExternalType)
				return Name;
			return Name;
		}

		public override string ToString()
		{
			if (Name == null) return "";

			if (TypeMode == DataTypeMode.Primitive)
			{
				if (Primitive == PrimitiveTypes.Void) return "void";
				if (Primitive == PrimitiveTypes.Object) return "object";
				if (Primitive == PrimitiveTypes.Byte) return "byte";
				if (Primitive == PrimitiveTypes.SByte) return "sbyte";
				if (Primitive == PrimitiveTypes.Short) return "short";
				if (Primitive == PrimitiveTypes.Int) return "int";
				if (Primitive == PrimitiveTypes.Long) return "long";
				if (Primitive == PrimitiveTypes.UShort) return "ushort";
				if (Primitive == PrimitiveTypes.UInt) return "uint";
				if (Primitive == PrimitiveTypes.ULong) return "ulong";
				if (Primitive == PrimitiveTypes.Float) return "float";
				if (Primitive == PrimitiveTypes.Double) return "double";
				if (Primitive == PrimitiveTypes.Decimal) return "decimal";
				if (Primitive == PrimitiveTypes.Bool) return "bool";
				if (Primitive == PrimitiveTypes.Char) return "char";
				if (Primitive == PrimitiveTypes.String) return "string";
				if (Primitive == PrimitiveTypes.DateTime) return "DateTime";
				if (Primitive == PrimitiveTypes.DateTimeOffset) return "DateTimeOffset";
				if (Primitive == PrimitiveTypes.TimeSpan) return "TimeSpan";
				if (Primitive == PrimitiveTypes.GUID) return "Guid";
				if (Primitive == PrimitiveTypes.URI) return "Uri";
				if (Primitive == PrimitiveTypes.Version) return "Version";
				if (Primitive == PrimitiveTypes.ByteArray) return "byte[]";
			}
			else if (TypeMode == DataTypeMode.Namespace)
				return Parent == null ? Name : string.Format("{0}.{1}", Parent.FullName, Name);
			else if (TypeMode == DataTypeMode.Class || TypeMode == DataTypeMode.Struct || TypeMode == DataTypeMode.Enum)
				return Parent != null ? string.Format("{0}.{1}", Parent.FullName, Name) : Name;
			else if (TypeMode == DataTypeMode.Array)
				return string.Format("{0}[]", CollectionGenericType);
			else if (TypeMode == DataTypeMode.Collection || TypeMode == DataTypeMode.Stack || TypeMode == DataTypeMode.Queue)
				return string.Format("{0}<{1}>", Name, CollectionGenericType != null ? CollectionGenericType.ToString() : "");
			else if (TypeMode == DataTypeMode.Dictionary)
				return string.Format("{0}<{1}, {2}>", Name, DictionaryKeyGenericType != null ? DictionaryKeyGenericType.ToString() : "", DictionaryValueGenericType != null ? DictionaryValueGenericType.ToString() : "");
			return Name;
		}

		public DataType Reference()
		{
			if (string.IsNullOrEmpty(Name)) return null;

			if (TypeMode == DataTypeMode.Class || TypeMode == DataTypeMode.Struct || TypeMode == DataTypeMode.Enum)
				return new DataType(TypeMode) { ID = ID, IsTypeReference = true, Name = string.Format("{0}.{1}", Parent.FullName, Name) };
			return null;
		}
	}
}