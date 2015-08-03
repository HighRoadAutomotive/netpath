using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;
using NETPath.Projects.Wcf;

namespace NETPath.Projects.Data
{
	public class DataData : DataType
	{
		public bool HasXAMLType { get { return (bool)GetValue(HasXAMLTypeProperty); } set { SetValue(HasXAMLTypeProperty, value); } }
		public static readonly DependencyProperty HasXAMLTypeProperty = DependencyProperty.Register("HasXAMLType", typeof(bool), typeof(DataData), new PropertyMetadata(true, HasXAMLTypeChangedCallback));

		private static void HasXAMLTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataData;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue) == false)
				t.XAMLType = null;
			else
			{
				if (t.XAMLType != null) return;
				t.XAMLType = Convert.ToBoolean(e.NewValue) == false ? null : new DataType(t.TypeMode) { ID = t.ID, Parent = t.Parent, Name = t.Name + "XAML", Scope = t.Scope, Partial = t.Partial, Abstract = t.Abstract, Sealed = t.Sealed };
				t.XAMLType?.InheritedTypes.Add(new DataType("DependencyObject", DataTypeMode.Class));
			}
		}

		public DataType XAMLType { get { return (DataType)GetValue(XAMLTypeProperty); } set { SetValue(XAMLTypeProperty, value); } }
		public static readonly DependencyProperty XAMLTypeProperty = DependencyProperty.Register("XAMLType", typeof(DataType), typeof(DataData));

		public bool IsReference { get { return (bool)GetValue(IsReferenceProperty); } set { SetValue(IsReferenceProperty, value); } }
		public static readonly DependencyProperty IsReferenceProperty = DependencyProperty.Register("IsReference", typeof(bool), typeof(DataData));

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(DataData));

		public ObservableCollection<DataDataElement> Elements { get { return (ObservableCollection<DataDataElement>)GetValue(ElementsProperty); } set { SetValue(ElementsProperty, value); } }
		public static readonly DependencyProperty ElementsProperty = DependencyProperty.Register("Elements", typeof(ObservableCollection<DataDataElement>), typeof(DataData));

		public bool HasEntity { get { return (bool)GetValue(HasEntityProperty); } set { SetValue(HasEntityProperty, value); } }
		public static readonly DependencyProperty HasEntityProperty = DependencyProperty.Register("HasEntity", typeof(bool), typeof(DataData), new PropertyMetadata(false));

		public string EntityType { get { return (string)GetValue(EntityTypeProperty); } set { SetValue(EntityTypeProperty, value); } }
		public static readonly DependencyProperty EntityTypeProperty = DependencyProperty.Register("EntityType", typeof(string), typeof(DataData), new PropertyMetadata(""));

		public string EntityContext { get { return (string)GetValue(EntityContextProperty); } set { SetValue(EntityContextProperty, value); } }
		public static readonly DependencyProperty EntityContextProperty = DependencyProperty.Register("EntityContext", typeof(string), typeof(DataData), new PropertyMetadata(""));

		public bool HasSql { get { return (bool)GetValue(HasSqlProperty); } set { SetValue(HasSqlProperty, value); } }
		public static readonly DependencyProperty HasSqlProperty = DependencyProperty.Register("HasSql", typeof(bool), typeof(DataData), new PropertyMetadata(false, HasSqlChangedCallback));

		private static void HasSqlChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataData;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue))
			{
				if (t.SchemaName == "") t.SchemaName = t.Parent.Name;
				if (t.TableName == "") t.TableName = t.Name;
			}
			else
			{
				t.SchemaName = "";
				t.TableName = "";
			}
		}

		public string SchemaName { get { return (string)GetValue(SchemaNameProperty); } set { SetValue(SchemaNameProperty, value); } }
		public static readonly DependencyProperty SchemaNameProperty = DependencyProperty.Register("SchemaName", typeof(string), typeof(DataData), new PropertyMetadata(""));

		public string TableName { get { return (string)GetValue(TableNameProperty); } set { SetValue(TableNameProperty, value); } }
		public static readonly DependencyProperty TableNameProperty = DependencyProperty.Register("TableName", typeof(string), typeof(DataData), new PropertyMetadata(""));

		public bool CanSelect { get { return (bool)GetValue(CanSelectProperty); } set { SetValue(CanSelectProperty, value); } }
		public static readonly DependencyProperty CanSelectProperty = DependencyProperty.Register("CanSelect", typeof(bool), typeof(DataData), new PropertyMetadata(false));

		public bool CanMerge { get { return (bool)GetValue(CanMergeProperty); } set { SetValue(CanMergeProperty, value); } }
		public static readonly DependencyProperty CanMergeProperty = DependencyProperty.Register("CanMerge", typeof(bool), typeof(DataData), new PropertyMetadata(false));

		public string MergeTransactionName { get { return (string)GetValue(MergeTransactionNameProperty); } set { SetValue(MergeTransactionNameProperty, value); } }
		public static readonly DependencyProperty MergeTransactionNameProperty = DependencyProperty.Register("MergeTransactionName", typeof(string), typeof(DataData), new PropertyMetadata(""));

		public IsolationLevel? MergeTransactionLevel { get { return (IsolationLevel?)GetValue(MergeTransactionLevelProperty); } set { SetValue(MergeTransactionLevelProperty, value); } }
		public static readonly DependencyProperty MergeTransactionLevelProperty = DependencyProperty.Register("MergeTransactionLevel", typeof(IsolationLevel?), typeof(DataData), new PropertyMetadata(null));

		public bool CanDelete { get { return (bool)GetValue(CanDeleteProperty); } set { SetValue(CanDeleteProperty, value); } }
		public static readonly DependencyProperty CanDeleteProperty = DependencyProperty.Register("CanDelete", typeof(bool), typeof(DataData), new PropertyMetadata(false));

		public string DeleteTransactionName { get { return (string)GetValue(DeleteTransactionNameProperty); } set { SetValue(DeleteTransactionNameProperty, value); } }
		public static readonly DependencyProperty DeleteTransactionNameProperty = DependencyProperty.Register("DeleteTransactionName", typeof(string), typeof(DataData), new PropertyMetadata(""));

		public IsolationLevel? DeleteTransactionLevel { get { return (IsolationLevel?)GetValue(DeleteTransactionLevelProperty); } set { SetValue(DeleteTransactionLevelProperty, value); } }
		public static readonly DependencyProperty DeleteTransactionLevelProperty = DependencyProperty.Register("DeleteTransactionLevel", typeof(IsolationLevel?), typeof(DataData), new PropertyMetadata(null));

		//System
		[IgnoreDataMember]
		public bool HasWinFormsBindings { get { return Elements.Any(a => a.GenerateWinFormsSupport); } }

		public DataData()
			: base(DataTypeMode.Class)
		{
			Documentation = new Documentation {IsClass = true};
			IsDataObject = true;
		}

		public DataData(string Name, Namespace Parent)
			: base(DataTypeMode.Class)
		{
			Elements = new ObservableCollection<DataDataElement>();
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

		public void RemoveKnownType(DataType Type)
		{
			if (Elements.Any(dt => dt.DataType.TypeName == Type.TypeName)) return;
			var d = KnownTypes.FirstOrDefault(a => a.TypeName == Type.TypeName);
			if (d != null) KnownTypes.Remove(d);
		}
	}

	public class DataDataElement : DependencyObject
	{
		public Guid ID { get; set; }

		//Basic Data-Type Settings
		public DataType DataType { get { return (DataType)GetValue(DataTypeProperty); } set { SetValue(DataTypeProperty, value); } }
		public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType", typeof(DataType), typeof(DataDataElement), new PropertyMetadata(DataTypeChangedCallback));

		private static void DataTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs p)
		{
			var de = o as DataDataElement;
			if (de == null) return;

			var nt = p.NewValue as DataType;
			if (nt == null) return;

			var ot = p.OldValue as DataType;
			if (ot == null) return;
			if (de.Owner == null) return;

			if (ot.TypeMode == DataTypeMode.Array && ot.CollectionGenericType.TypeMode == DataTypeMode.Primitive) de.Owner.RemoveKnownType(nt);
			if (nt.TypeMode == DataTypeMode.Array && nt.CollectionGenericType.TypeMode == DataTypeMode.Primitive) de.Owner.AddKnownType(nt);
			if (ot.TypeMode == DataTypeMode.Primitive && ot.Primitive == PrimitiveTypes.DateTimeOffset) de.Owner.RemoveKnownType(new DataType(PrimitiveTypes.DateTimeOffset));
			if (nt.TypeMode == DataTypeMode.Primitive && nt.Primitive == PrimitiveTypes.DateTimeOffset) de.Owner.AddKnownType(new DataType(PrimitiveTypes.DateTimeOffset));
		}

		public string DataName { get { return (string)GetValue(DataNameProperty); } set { SetValue(DataNameProperty, value); } }
		public static readonly DependencyProperty DataNameProperty = DependencyProperty.Register("DataName", typeof(string), typeof(DataDataElement), new PropertyMetadata(""));

		public bool HasContractName { get { return (bool)GetValue(HasContractNameProperty); } set { SetValue(HasContractNameProperty, value); } }
		public static readonly DependencyProperty HasContractNameProperty = DependencyProperty.Register("HasContractName", typeof(bool), typeof(DataDataElement), new PropertyMetadata(false, HasContractNameChangedCallback));

		private static void HasContractNameChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataDataElement;
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
		public static readonly DependencyProperty ContractNameProperty = DependencyProperty.Register("ContractName", typeof(string), typeof(DataDataElement), new PropertyMetadata(""));

		public bool HasClientType { get { return (bool)GetValue(HasClientTypeProperty); } set { SetValue(HasClientTypeProperty, value); } }
		public static readonly DependencyProperty HasClientTypeProperty = DependencyProperty.Register("HasClientType", typeof(bool), typeof(DataDataElement), new PropertyMetadata(false, HasClientTypeChangedCallback));

		private static void HasClientTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataDataElement;
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
		public static readonly DependencyProperty ClientNameProperty = DependencyProperty.Register("ClientName", typeof(string), typeof(DataDataElement), new PropertyMetadata(""));

		public bool HasXAMLType { get { return (bool)GetValue(HasXAMLTypeProperty); } set { SetValue(HasXAMLTypeProperty, value); } }
		public static readonly DependencyProperty HasXAMLTypeProperty = DependencyProperty.Register("HasXAMLType", typeof(bool), typeof(DataDataElement), new PropertyMetadata(false, HasXAMLTypeChangedCallback));

		private static void HasXAMLTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataDataElement;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue))
			{
				if (t.XamlName == "") t.XamlName = t.DataName;
			}
			else
			{
				t.XamlName = "";
			}
		}

		public string XamlName { get { return (string)GetValue(XamlNameProperty); } set { SetValue(XamlNameProperty, value); } }
		public static readonly DependencyProperty XamlNameProperty = DependencyProperty.Register("XamlName", typeof(string), typeof(DataDataElement), new PropertyMetadata(""));

		public bool HasEntity { get { return (bool)GetValue(HasEntityProperty); } set { SetValue(HasEntityProperty, value); } }
		public static readonly DependencyProperty HasEntityProperty = DependencyProperty.Register("HasEntity", typeof(bool), typeof(DataDataElement), new PropertyMetadata(false, HasEntityChangedCallback));

		private static void HasEntityChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataDataElement;
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
		public static readonly DependencyProperty EntityNameProperty = DependencyProperty.Register("EntityName", typeof(string), typeof(DataDataElement), new PropertyMetadata(""));

		public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(DataDataElement), new PropertyMetadata(false));

		public bool IsReadOnly { get { return (bool)GetValue(IsReadOnlyProperty); } set { SetValue(IsReadOnlyProperty, value); } }
		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(DataDataElement), new PropertyMetadata(false));

		public bool GenerateWinFormsSupport { get { return (bool)GetValue(GenerateWinFormsSupportProperty); } set { SetValue(GenerateWinFormsSupportProperty, value); } }
		public static readonly DependencyProperty GenerateWinFormsSupportProperty = DependencyProperty.Register("GenerateWinFormsSupport", typeof(bool), typeof(DataDataElement), new PropertyMetadata(false));

		//DataMember Settings
		public bool IsDataMember { get { return (bool)GetValue(IsDataMemberProperty); } set { SetValue(IsDataMemberProperty, value); } }
		public static readonly DependencyProperty IsDataMemberProperty = DependencyProperty.Register("IsDataMember", typeof(bool), typeof(DataDataElement), new UIPropertyMetadata(true, IsDataMemberChangedCallback));

		private static void IsDataMemberChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataDataElement;
			if (t == null) return;
			if (Convert.ToBoolean(e.NewValue)) return;

			t.IsRequired = false;
			t.EmitDefaultValue = false;
			t.HasClientType = false;
			t.GenerateWinFormsSupport = false;
			t.HasXAMLType = false;
		}

		public bool EmitDefaultValue { get { return (bool)GetValue(EmitDefaultValueProperty); } set { SetValue(EmitDefaultValueProperty, value); } }
		public static readonly DependencyProperty EmitDefaultValueProperty = DependencyProperty.Register("EmitDefaultValue", typeof(bool), typeof(DataDataElement), new PropertyMetadata(false));

		public bool IsRequired { get { return (bool)GetValue(IsRequiredProperty); } set { SetValue(IsRequiredProperty, value); } }
		public static readonly DependencyProperty IsRequiredProperty = DependencyProperty.Register("IsRequired", typeof(bool), typeof(DataDataElement), new PropertyMetadata(false));

		public int Order { get { return (int)GetValue(OrderProperty); } set { SetValue(OrderProperty, value); } }
		public static readonly DependencyProperty OrderProperty = DependencyProperty.Register("Order", typeof(int), typeof(DataDataElement), new PropertyMetadata(-1));

		//SQL Support
		public bool HasSql { get { return (bool)GetValue(HasSqlProperty); } set { SetValue(HasSqlProperty, value); } }
		public static readonly DependencyProperty HasSqlProperty = DependencyProperty.Register("HasSql", typeof(bool), typeof(DataDataElement), new PropertyMetadata(false, HasSqlChangedCallback));

		private static void HasSqlChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataDataElement;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue))
			{
				if (t.SqlName == "") t.SqlName = t.DataName;
			}
			else
			{
				t.SqlName = "";
			}
		}

		public string SqlName { get { return (string)GetValue(SqlNameProperty); } set { SetValue(SqlNameProperty, value); } }
		public static readonly DependencyProperty SqlNameProperty = DependencyProperty.Register("SqlName", typeof(string), typeof(DataDataElement), new PropertyMetadata(""));

		public SqlDbType SqlType { get { return (SqlDbType)GetValue(SqlTypeProperty); } set { SetValue(SqlTypeProperty, value); } }
		public static readonly DependencyProperty SqlTypeProperty = DependencyProperty.Register("SqlType", typeof(SqlDbType), typeof(DataDataElement), new PropertyMetadata(SqlDbType.Int));

		public bool IsSqlPrimaryKey { get { return (bool)GetValue(IsSqlPrimaryKeyProperty); } set { SetValue(IsSqlPrimaryKeyProperty, value); } }
		public static readonly DependencyProperty IsSqlPrimaryKeyProperty = DependencyProperty.Register("IsSqlPrimaryKey", typeof(bool), typeof(DataDataElement), new PropertyMetadata(false));

		public bool IsSqlPrimaryKeyIdentity { get { return (bool)GetValue(IsSqlPrimaryKeyIdentityProperty); } set { SetValue(IsSqlPrimaryKeyIdentityProperty, value); } }
		public static readonly DependencyProperty IsSqlPrimaryKeyIdentityProperty = DependencyProperty.Register("IsSqlPrimaryKeyIdentity", typeof(bool), typeof(DataDataElement), new PropertyMetadata(false));

		public bool IsSqlComputed { get { return (bool)GetValue(IsSqlComputedProperty); } set { SetValue(IsSqlComputedProperty, value); } }
		public static readonly DependencyProperty IsSqlComputedProperty = DependencyProperty.Register("IsSqlComputed", typeof(bool), typeof(DataDataElement), new PropertyMetadata(false));

		//System
		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(DataDataElement));

		public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(DataDataElement), new PropertyMetadata(false));

		public DataData Owner { get; set; }

		public DataDataElement()
		{
			ID = Guid.NewGuid();
			DataType = new DataType(PrimitiveTypes.String);
			EmitDefaultValue = false;
			Order = -1;
			Documentation = new Documentation { IsProperty = true };
			HasClientType = false;
			HasXAMLType = true;
			HasXAMLType = false;
		}

		public DataDataElement(DataType DataType, string Name, DataData Owner)
		{
			ID = Guid.NewGuid();
			this.DataType = DataType;
			this.Owner = Owner;
			DataName = Name;
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
	}
}