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
		Enum,
		Class,
		Struct,
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
		public Guid ID { get; protected set; }

		public bool HasContractType { get { return (bool)GetValue(HasContractTypeProperty); } set { SetValue(HasContractTypeProperty, value); if (value == false) { ContractType = null; } else { ContractType = new DataType(DataTypeMode.Class); ContractType.Name = Name; ContractType.Scope = Scope; } } }
		public static readonly DependencyProperty HasContractTypeProperty = DependencyProperty.Register("HasContractType", typeof(bool), typeof(Data));

		public DataType ContractType { get { return (DataType)GetValue(ContractTypeProperty); } set { SetValue(ContractTypeProperty, value); } }
		public static readonly DependencyProperty ContractTypeProperty = DependencyProperty.Register("ContractType", typeof(DataType), typeof(Data));

		public DataScope Scope { get { return (DataScope)GetValue(ScopeProperty); } set { SetValue(ScopeProperty, value); } }
		public static readonly DependencyProperty ScopeProperty = DependencyProperty.Register("Scope", typeof(DataScope), typeof(DataType), new PropertyMetadata(DataScope.Public));

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(DataType));

		public bool IsPartial { get { return (bool)GetValue(IsPartialProperty); } set { SetValue(IsPartialProperty, value); } }
		public static readonly DependencyProperty IsPartialProperty = DependencyProperty.Register("IsPartial", typeof(bool), typeof(DataType));

		public bool IsAbstract { get { return (bool)GetValue(IsAbstractProperty); } set { SetValue(IsAbstractProperty, value); } }
		public static readonly DependencyProperty IsAbstractProperty = DependencyProperty.Register("IsAbstract", typeof(bool), typeof(DataType));

		public DataType CollectionGenericType { get { return (DataType)GetValue(CollectionGenericTypeProperty); } set { SetValue(CollectionGenericTypeProperty, value); } }
		public static readonly DependencyProperty CollectionGenericTypeProperty = DependencyProperty.Register("CollectionGenericType", typeof(DataType), typeof(DataType));

		public DataType DictionaryKeyGenericType { get { return (DataType)GetValue(DictionaryKeyGenericTypeProperty); } set { SetValue(DictionaryKeyGenericTypeProperty, value); } }
		public static readonly DependencyProperty DictionaryKeyGenericTypeProperty = DependencyProperty.Register("DictionaryKeyGenericType", typeof(DataType), typeof(DataType));

		public DataType DictionaryValueGenericType { get { return (DataType)GetValue(DictionaryValueGenericTypeProperty); } set { SetValue(DictionaryValueGenericTypeProperty, value); } }
		public static readonly DependencyProperty DictionaryValueGenericTypeProperty = DependencyProperty.Register("DictionaryValueGenericType", typeof(DataType), typeof(DataType));

		public DataTypeMode ExternalType { get { return (DataTypeMode)GetValue(ExternalTypeProperty); } set { SetValue(ExternalTypeProperty, value); } }
		public static readonly DependencyProperty ExternalTypeProperty = DependencyProperty.Register("ExternalType", typeof(DataTypeMode), typeof(DataType));

		public ObservableCollection<DataType> InheritedTypes { get { return (ObservableCollection<DataType>)GetValue(InheritedTypesProperty); } set { SetValue(InheritedTypesProperty, value); } }
		public static readonly DependencyProperty InheritedTypesProperty = DependencyProperty.Register("InheritedTypes", typeof(ObservableCollection<DataType>), typeof(DataType));

		public Namespace Parent { get { return (Namespace)GetValue(ParentProperty); } set { SetValue(ParentProperty, value); } }
		public static readonly DependencyProperty ParentProperty = DependencyProperty.Register("Parent", typeof(Namespace), typeof(DataType));

		public DataTypeMode TypeMode { get; private set; }
		public PrimitiveTypes Primitive { get; private set; }

		public DataType()
		{
			this.Scope = DataScope.Public;
			this.TypeMode = DataTypeMode.Class;
			this.IsPartial = true;
			this.Primitive = PrimitiveTypes.None;
			InheritedTypes = new ObservableCollection<DataType>();
		}

		public DataType(DataTypeMode Mode)
		{
			if (Mode == DataTypeMode.Primitive) throw new ArgumentException("Cannot use DataTypeMode.Primitive without specifying a Primitive. Please use the Primitive constructor.");
			this.Scope = DataScope.Public;
			this.TypeMode = Mode;
			this.IsPartial = false;
			if (Mode == DataTypeMode.Class || Mode == DataTypeMode.Struct) this.IsPartial = true;
			this.Primitive = PrimitiveTypes.None;
			InheritedTypes = new ObservableCollection<DataType>();
		}

		public DataType(PrimitiveTypes Primitive)
		{
			if (Primitive == PrimitiveTypes.None) throw new ArgumentException("Cannot set PrimitiveTypes.None using the Primitive DataType constructor. Please use the mode constructor.");
			this.Scope = DataScope.Public;
			this.Primitive = Primitive;
			this.IsPartial = false;
			this.TypeMode = DataTypeMode.Primitive;
		}

		public DataType(string External, DataTypeMode ExternalType)
		{
			this.Name = External;
			this.Scope = DataScope.Public;
			this.IsPartial = false;
			this.TypeMode = DataTypeMode.External;
			this.ExternalType = ExternalType;
		}
	}
}