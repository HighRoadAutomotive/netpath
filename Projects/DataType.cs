using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Serialization;

namespace WCFArchitect.Projects
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
		External
	}

	public enum PrimitiveTypes
	{
		None,
		Void,
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

	public class DataType : OpenableDocument
	{
		public Guid ID { get; set; }

		public bool HasContractType { get { return (bool)GetValue(HasContractTypeProperty); } set { SetValue(HasContractTypeProperty, value); if (value == false) { ContractType = null; } else { ContractType = new DataType(TypeMode); ContractType.Name = Name; ContractType.Scope = Scope; } } }
		public static readonly DependencyProperty HasContractTypeProperty = DependencyProperty.Register("HasContractType", typeof(bool), typeof(Data));

		public DataType ContractType { get { return (DataType)GetValue(ContractTypeProperty); } set { SetValue(ContractTypeProperty, value); } }
		public static readonly DependencyProperty ContractTypeProperty = DependencyProperty.Register("ContractType", typeof(DataType), typeof(Data));

		public DataScope Scope { get { return (DataScope)GetValue(ScopeProperty); } set { SetValue(ScopeProperty, value); } }
		public static readonly DependencyProperty ScopeProperty = DependencyProperty.Register("Scope", typeof(DataScope), typeof(DataType), new PropertyMetadata(DataScope.Public));

		public bool IsPartial { get { return (bool)GetValue(IsPartialProperty); } set { SetValue(IsPartialProperty, value); } }
		public static readonly DependencyProperty IsPartialProperty = DependencyProperty.Register("IsPartial", typeof(bool), typeof(DataType));

		public bool IsAbstract { get { return (bool)GetValue(IsAbstractProperty); } set { SetValue(IsAbstractProperty, value); } }
		public static readonly DependencyProperty IsAbstractProperty = DependencyProperty.Register("IsAbstract", typeof(bool), typeof(DataType));

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(DataType));

		public ObservableCollection<DataType> InheritedTypes { get { return (ObservableCollection<DataType>)GetValue(InheritedTypesProperty); } set { SetValue(InheritedTypesProperty, value); } }
		public static readonly DependencyProperty InheritedTypesProperty = DependencyProperty.Register("InheritedTypes", typeof(ObservableCollection<DataType>), typeof(DataType));

		public DataType CollectionGenericType { get { return (DataType)GetValue(CollectionGenericTypeProperty); } set { SetValue(CollectionGenericTypeProperty, value); } }
		public static readonly DependencyProperty CollectionGenericTypeProperty = DependencyProperty.Register("CollectionGenericType", typeof(DataType), typeof(DataType));

		public DataType DictionaryKeyGenericType { get { return (DataType)GetValue(DictionaryKeyGenericTypeProperty); } set { SetValue(DictionaryKeyGenericTypeProperty, value); } }
		public static readonly DependencyProperty DictionaryKeyGenericTypeProperty = DependencyProperty.Register("DictionaryKeyGenericType", typeof(DataType), typeof(DataType));

		public DataType DictionaryValueGenericType { get { return (DataType)GetValue(DictionaryValueGenericTypeProperty); } set { SetValue(DictionaryValueGenericTypeProperty, value); } }
		public static readonly DependencyProperty DictionaryValueGenericTypeProperty = DependencyProperty.Register("DictionaryValueGenericType", typeof(DataType), typeof(DataType));

		public DataTypeMode ExternalType { get { return (DataTypeMode)GetValue(ExternalTypeProperty); } set { SetValue(ExternalTypeProperty, value); } }
		public static readonly DependencyProperty ExternalTypeProperty = DependencyProperty.Register("ExternalType", typeof(DataTypeMode), typeof(DataType));

		public Namespace Parent { get { return (Namespace)GetValue(ParentProperty); } set { SetValue(ParentProperty, value); } }
		public static readonly DependencyProperty ParentProperty = DependencyProperty.Register("Parent", typeof(Namespace), typeof(DataType));

		[IgnoreDataMember()] public string TypeName { get { return (string)GetValue(TypeNameProperty); } private set { SetValue(TypeNamePropertyKey, value); } }
		private static readonly DependencyPropertyKey TypeNamePropertyKey = DependencyProperty.RegisterReadOnly("TypeName", typeof(string), typeof(DataType), new PropertyMetadata(""));
		public static readonly DependencyProperty TypeNameProperty = TypeNamePropertyKey.DependencyProperty;

		[IgnoreDataMember()] public string Declaration { get { return (string)GetValue(DeclarationProperty); } private set { SetValue(DeclarationPropertyKey, value); } }
		private static readonly DependencyPropertyKey DeclarationPropertyKey = DependencyProperty.RegisterReadOnly("Declaration", typeof(string), typeof(DataType), new PropertyMetadata(""));
		public static readonly DependencyProperty DeclarationProperty = DeclarationPropertyKey.DependencyProperty;

		public DataTypeMode TypeMode { get; set; }
		public PrimitiveTypes Primitive { get; private set; }

		public DataType() : base()
		{
			this.Scope = DataScope.Public;
			this.TypeMode = DataTypeMode.Class;
			this.IsPartial = true;
			this.Primitive = PrimitiveTypes.None;
			this.HasContractType = false;
			InheritedTypes = new ObservableCollection<DataType>();
		}

		public DataType(DataTypeMode Mode) : base()
		{
			if (Mode == DataTypeMode.Primitive) throw new ArgumentException("Cannot use DataTypeMode.Primitive without specifying a Primitive. Please use the Primitive constructor.");
			this.Scope = DataScope.Public;
			this.TypeMode = Mode;
			this.IsPartial = false;
			if (Mode == DataTypeMode.Class || Mode == DataTypeMode.Struct) this.IsPartial = true;
			this.Primitive = PrimitiveTypes.None;
			this.HasContractType = false;
			InheritedTypes = new ObservableCollection<DataType>();
		}

		public DataType(PrimitiveTypes Primitive) : base()
		{
			if (Primitive == PrimitiveTypes.None) throw new ArgumentException("Cannot set PrimitiveTypes.None using the Primitive DataType constructor. Please use the mode constructor.");
			this.Scope = DataScope.Public;
			this.Primitive = Primitive;
			this.IsPartial = false;
			this.HasContractType = false;
			this.TypeMode = DataTypeMode.Primitive;
		}

		public DataType(string External, DataTypeMode ExternalType) : base()
		{
			this.Name = External;
			this.Scope = DataScope.Public;
			this.IsPartial = false;
			this.TypeMode = DataTypeMode.External;
			this.HasContractType = false;
			this.ExternalType = ExternalType;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == OpenableDocument.IsDirtyProperty) return;

			if (e.Property != DataType.DeclarationProperty && e.Property != DataType.TypeNameProperty)
			{
				TypeName = ToString();
				Declaration = ToDeclarationString();
			}
		}

		public string ToScopeString()
		{
			if (Scope == DataScope.Public) return "public";
			if (Scope == DataScope.Protected) return "protected";
			if (Scope == DataScope.ProtectedInternal) return "protected internal";
			if (Scope == DataScope.Internal) return "internal";
			if (Scope == DataScope.Private) return "private";
			return "public";
		}

		public string ToInheritedString()
		{
			if (InheritedTypes.Count == 0) return "";
			StringBuilder sb = new StringBuilder(" : ");
			foreach (DataType t in InheritedTypes)
				sb.AppendFormat("{0}, ", t.ToString());
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

			if (TypeMode == Projects.DataTypeMode.Primitive)
			{
				if (Primitive == Projects.PrimitiveTypes.Byte) return string.Format("{0} byte {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.SByte) return string.Format("{0} sbyte {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.Short) return string.Format("{0} short {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.Int) return string.Format("{0} int {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.Long) return string.Format("{0} long {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.UShort) return string.Format("{0} ushort {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.UInt) return string.Format("{0} uint {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.ULong) return string.Format("{0} ulong {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.Float) return string.Format("{0} float {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.Double) return string.Format("{0} double {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.Decimal) return string.Format("{0} decimal {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.Bool) return string.Format("{0} bool {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.Char) return string.Format("{0} char {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.String) return string.Format("{0} string {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.DateTime) return string.Format("{0} DateTime {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.DateTimeOffset) return string.Format("{0} DateTimeOffset {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.TimeSpan) return string.Format("{0} TimeSpan {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.GUID) return string.Format("{0} Guid {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.URI) return string.Format("{0} Uri {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.Version) return string.Format("{0} Version {1}", ToScopeString(), Name);
				if (Primitive == Projects.PrimitiveTypes.ByteArray) return string.Format("{0} byte[] {1}", ToScopeString(), Name);
			}
			else if (TypeMode == DataTypeMode.Namespace)
				return Parent == null ? string.Format("namespace {0}", Name) : string.Format("namespace {0}.{1}", Parent.FullName, Name);
			else if (TypeMode == Projects.DataTypeMode.Class || TypeMode == Projects.DataTypeMode.Struct || TypeMode == Projects.DataTypeMode.Enum)
				return string.Format("{0} {1}{2}{3} {4}{5}", ToScopeString(), IsPartial == true ? "partial " : "", IsAbstract == true ? "abstract " : "", ToStorageClassString(), Name, ToInheritedString());
			else if (TypeMode == Projects.DataTypeMode.Array)
				return string.Format("{0} {1}[] {2}", ToScopeString(), CollectionGenericType.ToString(), Name);
			else if (TypeMode == Projects.DataTypeMode.Collection)
				return string.Format("{0} {1}<{2}>", ToScopeString(), Name, CollectionGenericType.ToString());
			else if (TypeMode == Projects.DataTypeMode.Dictionary)
				return string.Format("{0} {1}<{2}, {3}>", ToScopeString(), Name, DictionaryKeyGenericType.ToString(), DictionaryValueGenericType.ToString());
			else if (TypeMode == Projects.DataTypeMode.External)
				return Name;
			return Name;
		}

		public override string ToString()
		{
			if (Name == null) return "";

			if (TypeMode == Projects.DataTypeMode.Primitive)
			{
				if (Primitive == Projects.PrimitiveTypes.Byte) return "byte";
				if (Primitive == Projects.PrimitiveTypes.SByte) return "sbyte";
				if (Primitive == Projects.PrimitiveTypes.Short) return "short";
				if (Primitive == Projects.PrimitiveTypes.Int) return "int";
				if (Primitive == Projects.PrimitiveTypes.Long) return "long";
				if (Primitive == Projects.PrimitiveTypes.UShort) return "ushort";
				if (Primitive == Projects.PrimitiveTypes.UInt) return "uint";
				if (Primitive == Projects.PrimitiveTypes.ULong) return "ulong";
				if (Primitive == Projects.PrimitiveTypes.Float) return "float";
				if (Primitive == Projects.PrimitiveTypes.Double) return "double";
				if (Primitive == Projects.PrimitiveTypes.Decimal) return "decimal";
				if (Primitive == Projects.PrimitiveTypes.Bool) return "bool";
				if (Primitive == Projects.PrimitiveTypes.Char) return "char";
				if (Primitive == Projects.PrimitiveTypes.String) return "string";
				if (Primitive == Projects.PrimitiveTypes.DateTime) return "DateTime";
				if (Primitive == Projects.PrimitiveTypes.DateTimeOffset) return "DateTimeOffset";
				if (Primitive == Projects.PrimitiveTypes.TimeSpan) return "TimeSpan";
				if (Primitive == Projects.PrimitiveTypes.GUID) return "Guid";
				if (Primitive == Projects.PrimitiveTypes.URI) return "Uri";
				if (Primitive == Projects.PrimitiveTypes.Version) return "Version";
				if (Primitive == Projects.PrimitiveTypes.ByteArray) return "byte[]";
			}
			else if (TypeMode == DataTypeMode.Namespace)
				return Parent == null ? Name : string.Format("{0}.{1}", Parent.FullName, Name);
			else if (TypeMode == Projects.DataTypeMode.Class || TypeMode == Projects.DataTypeMode.Struct || TypeMode == Projects.DataTypeMode.Enum)
				if (Parent != null) return string.Format("{0}.{1}", Parent.FullName, Name);
				else return Name;
			else if (TypeMode == Projects.DataTypeMode.Array)
				return string.Format("{0}[]", CollectionGenericType.ToString());
			else if (TypeMode == Projects.DataTypeMode.Collection)
				return string.Format("{0}<{1}>", Name, CollectionGenericType != null ? CollectionGenericType.ToString() : "");
			else if (TypeMode == Projects.DataTypeMode.Dictionary)
				return string.Format("{0}<{1}, {2}>", Name, DictionaryKeyGenericType != null ? DictionaryKeyGenericType.ToString() : "", DictionaryValueGenericType != null ? DictionaryValueGenericType.ToString() : "");
			else if (TypeMode == Projects.DataTypeMode.External)
				return Name;
			return Name;
		}
	}
}