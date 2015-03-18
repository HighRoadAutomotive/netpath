using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		NET45 = 0x1,
		WINRT = 0x2
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
					t.ClientType = Convert.ToBoolean(e.NewValue) == false ? null : new DataType(t.TypeMode) { ID = t.ID, Name = t.Name, Scope = t.Scope, Partial = t.Partial, Abstract = t.Abstract, Sealed = t.Sealed };
					if (t.ClientType != null)
					{
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

		public Namespace Parent { get { return (Namespace)GetValue(ParentProperty); } set { SetValue(ParentProperty, value); } }
		public static readonly DependencyProperty ParentProperty = DependencyProperty.Register("Parent", typeof(Namespace), typeof(DataType));

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

		public bool IsNullable { get { return (bool)GetValue(IsNullableProperty); } set { SetValue(IsNullableProperty, value); } }
		public static readonly DependencyProperty IsNullableProperty = DependencyProperty.Register("IsNullable", typeof(bool), typeof(DataType), new PropertyMetadata(false));

		[IgnoreDataMember]
		public string TypeName { get { return (string)GetValue(TypeNameProperty); } private set { SetValue(TypeNamePropertyKey, value); } }
		private static readonly DependencyPropertyKey TypeNamePropertyKey = DependencyProperty.RegisterReadOnly("TypeName", typeof(string), typeof(DataType), new PropertyMetadata(""));
		public static readonly DependencyProperty TypeNameProperty = TypeNamePropertyKey.DependencyProperty;

		[IgnoreDataMember]
		public string Declaration { get { return (string)GetValue(DeclarationProperty); } private set { SetValue(DeclarationPropertyKey, value); } }
		private static readonly DependencyPropertyKey DeclarationPropertyKey = DependencyProperty.RegisterReadOnly("Declaration", typeof(string), typeof(DataType), new PropertyMetadata(""));
		public static readonly DependencyProperty DeclarationProperty = DeclarationPropertyKey.DependencyProperty;

		public DataTypeMode TypeMode { get { return (DataTypeMode)GetValue(TypeModeProperty); } set { SetValue(TypeModeProperty, value); } }
		public static readonly DependencyProperty TypeModeProperty = DependencyProperty.Register("TypeMode", typeof(DataTypeMode), typeof(DataType), new PropertyMetadata(DataTypeMode.Class));

		public bool IsExternalType { get; set; }
		public PrimitiveTypes Primitive { get; set; }
		public bool IsTypeReference { get; set; }
		public SupportedFrameworks SupportedFrameworks { get; set; }

		[IgnoreDataMember]
		public bool IsDataObject { get; set; }
		[IgnoreDataMember]
		public bool IsCollectionType { get { return TypeMode == DataTypeMode.Array || TypeMode == DataTypeMode.Collection || TypeMode == DataTypeMode.Dictionary || TypeMode == DataTypeMode.Queue || TypeMode == DataTypeMode.Stack; } }
		[IgnoreDataMember]
		public bool IsValueType { get { return TypeMode == DataTypeMode.Primitive || TypeMode == DataTypeMode.Enum || TypeMode == DataTypeMode.Struct; } }
		[IgnoreDataMember]
		public bool IsReferenceType { get { return ((TypeMode == DataTypeMode.Primitive && Primitive == PrimitiveTypes.Object) || TypeMode == DataTypeMode.Class || TypeMode == DataTypeMode.Interface); } }
		[IgnoreDataMember]
		public bool IsVoid { get { return TypeMode == DataTypeMode.Primitive && Primitive == PrimitiveTypes.Void; } }

		private static void DataTypePropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataType;
			if (t == null) return;

			t.TypeName = t.ToString();
			t.Declaration = t.ToDeclarationString();
		}

		internal DataType()
		{
			ID = Guid.NewGuid();
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
			ID = Guid.NewGuid();
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

		public DataType(PrimitiveTypes Primitive, bool IsNullable = false)
		{
			if (Primitive == PrimitiveTypes.None) throw new ArgumentException("Cannot set PrimitiveTypes.None using the Primitive DataType constructor. Please use the Mode Type constructor.");
			ID = Guid.NewGuid();
			this.Primitive = Primitive;
			this.IsNullable = IsNullable;
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
			ID = Guid.NewGuid();
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
				if (Primitive == PrimitiveTypes.Void) return string.Format("{0} void{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Object) return string.Format("{0} object{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Byte) return string.Format("{0} byte{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.SByte) return string.Format("{0} sbyte{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Short) return string.Format("{0} short{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Int) return string.Format("{0} int{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Long) return string.Format("{0} long{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.UShort) return string.Format("{0} ushort{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.UInt) return string.Format("{0} uint{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.ULong) return string.Format("{0} ulong{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Float) return string.Format("{0} float{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Double) return string.Format("{0} double{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Decimal) return string.Format("{0} decimal{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Bool) return string.Format("{0} bool{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Char) return string.Format("{0} char{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.String) return string.Format("{0} string{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.DateTime) return string.Format("{0} DateTime{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.DateTimeOffset) return string.Format("{0} DateTimeOffset{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.TimeSpan) return string.Format("{0} TimeSpan{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.GUID) return string.Format("{0} Guid{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.URI) return string.Format("{0} Uri{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Version) return string.Format("{0} Version{1}", ToScopeString(), IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.ByteArray) return string.Format("{0} byte[]{1}", ToScopeString(), IsNullable ? "?" : "");
			}
			else if (IsExternalType == false && (TypeMode == DataTypeMode.Class || TypeMode == DataTypeMode.Struct || TypeMode == DataTypeMode.Enum))
				return string.Format("{0} {2}{3}{1}{4} {5}{6}", ToScopeString(), Partial ? "partial " : "", Abstract ? "abstract " : "", Sealed ? "sealed " : "", ToStorageClassString(), Name, ToInheritedString());
			else if (TypeMode == DataTypeMode.Array)
				return string.Format("{0} {1}[]{3} {2}", ToScopeString(), CollectionGenericType, Name, IsNullable ? "?" : "");
			else if (TypeMode == DataTypeMode.Collection || TypeMode == DataTypeMode.Stack || TypeMode == DataTypeMode.Queue)
				return string.Format("{0} {1}<{2}>{3}", ToScopeString(), Name, CollectionGenericType, IsNullable ? "?" : "");
			else if (TypeMode == DataTypeMode.Dictionary)
				return string.Format("{0} {1}<{2}, {3}>{4}", ToScopeString(), Name, DictionaryKeyGenericType, DictionaryValueGenericType, IsNullable ? "?" : "");
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
				if (Primitive == PrimitiveTypes.Object) return string.Format("object{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Byte) return string.Format("byte{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.SByte) return string.Format("sbyte{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Short) return string.Format("short{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Int) return string.Format("int{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Long) return string.Format("long{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.UShort) return string.Format("ushort{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.UInt) return string.Format("uint{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.ULong) return string.Format("ulong{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Float) return string.Format("float{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Double) return string.Format("double{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Decimal) return string.Format("decimal{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Bool) return string.Format("bool{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Char) return string.Format("char{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.String) return string.Format("string{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.DateTime) return string.Format("DateTime{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.DateTimeOffset) return string.Format("DateTimeOffset{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.TimeSpan) return string.Format("TimeSpan{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.GUID) return string.Format("Guid{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.URI) return string.Format("Uri{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.Version) return string.Format("Version{0}", IsNullable ? "?" : "");
				if (Primitive == PrimitiveTypes.ByteArray) return string.Format("byte[]{0}", IsNullable ? "?" : "");
			}
			else if (TypeMode == DataTypeMode.Class || TypeMode == DataTypeMode.Namespace)
				return Parent != null ? string.Format("{0}.{1}", Parent.FullName, Name) : Name;
			else if (TypeMode == DataTypeMode.Struct || TypeMode == DataTypeMode.Enum)
				return Parent != null ? string.Format("{0}.{1}{2}", Parent.FullName, Name, IsNullable ? "?" : "") : Name;
			else if (TypeMode == DataTypeMode.Array)
				return string.Format("{0}[]{1}", CollectionGenericType, IsNullable ? "?" : "");
			else if (TypeMode == DataTypeMode.Collection || TypeMode == DataTypeMode.Stack || TypeMode == DataTypeMode.Queue)
				return string.Format("{0}<{1}>", Name, CollectionGenericType?.ToString() ?? "");
			else if (TypeMode == DataTypeMode.Dictionary)
				return string.Format("{0}<{1}, {2}>", Name, DictionaryKeyGenericType?.ToString() ?? "", DictionaryValueGenericType?.ToString() ?? "");
			return Name;
		}

		public string GetSqlAdoName()
		{
			if (TypeMode == DataTypeMode.Primitive)
			{
				if (Primitive == PrimitiveTypes.Byte) return "Byte";
				if (Primitive == PrimitiveTypes.SByte) return "Byte";
				if (Primitive == PrimitiveTypes.Short) return "Int16";
				if (Primitive == PrimitiveTypes.Int) return "Int32";
				if (Primitive == PrimitiveTypes.Long) return "Int64";
				if (Primitive == PrimitiveTypes.UShort) return "Int16";
				if (Primitive == PrimitiveTypes.UInt) return "Int32";
				if (Primitive == PrimitiveTypes.ULong) return "Int64";
				if (Primitive == PrimitiveTypes.Float) return "Single";
				if (Primitive == PrimitiveTypes.Double) return "Double";
				if (Primitive == PrimitiveTypes.Decimal) return "Decimal";
				if (Primitive == PrimitiveTypes.Bool) return "Boolean";
				if (Primitive == PrimitiveTypes.Char) return "Byte";
				if (Primitive == PrimitiveTypes.String) return "String";
				if (Primitive == PrimitiveTypes.DateTime) return "DateTime";
				if (Primitive == PrimitiveTypes.DateTimeOffset) return "DateTimeOffset";
				if (Primitive == PrimitiveTypes.TimeSpan) return "TimeSpan";
				if (Primitive == PrimitiveTypes.GUID) return "Guid";
				if (Primitive == PrimitiveTypes.URI) return "String";
				if (Primitive == PrimitiveTypes.Version) return "String";
				if (Primitive == PrimitiveTypes.ByteArray) return "Bytes";
			}
			return "";
		}

		public DataType Reference()
		{
			if (string.IsNullOrEmpty(Name)) return null;

			if (TypeMode == DataTypeMode.Class || TypeMode == DataTypeMode.Struct || TypeMode == DataTypeMode.Enum)
				return new DataType(TypeMode) { ID = ID, IsTypeReference = true, Name = ToString() };
			return null;
		}
	}
}