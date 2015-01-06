using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;

namespace NETPath.Projects
{
	public class Enum : DataType
	{
		public bool IsFlags { get { return (bool)GetValue(IsFlagsProperty); } set { SetValue(IsFlagsProperty, value); } }
		public static readonly DependencyProperty IsFlagsProperty = DependencyProperty.Register("IsFlags", typeof(bool), typeof(Enum));

		public DataType BaseType { get { return (DataType)GetValue(BaseTypeProperty); } set { SetValue(BaseTypeProperty, value); } }
		public static readonly DependencyProperty BaseTypeProperty = DependencyProperty.Register("BaseType", typeof(DataType), typeof(Enum));

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(Enum));

		public ObservableCollection<EnumElement> Elements { get { return (ObservableCollection<EnumElement>)GetValue(ElementsProperty); } set { SetValue(ElementsProperty, value); } }
		public static readonly DependencyProperty ElementsProperty = DependencyProperty.Register("Elements", typeof(ObservableCollection<EnumElement>), typeof(Enum));

		public Enum() : base(DataTypeMode.Enum)
		{
			Elements = new ObservableCollection<EnumElement>();
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
	}

	public class EnumElement : DependencyObject
	{
		public Guid ID { get; set; }

		public Enum Owner { get { return (Enum)GetValue(OwnerProperty); } set { SetValue(OwnerProperty, value); } }
		public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register("Owner", typeof (Enum), typeof (EnumElement));

		//Basic
		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value ?? "", @"")); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(EnumElement));

		public bool IsExcluded { get { return (bool)GetValue(IsExcludedProperty); } set { SetValue(IsExcludedProperty, value); } }
		public static readonly DependencyProperty IsExcludedProperty = DependencyProperty.Register("IsExcluded", typeof(bool), typeof(EnumElement));

		public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(EnumElement));

		public bool IsAutoValue { get { return (bool)GetValue(IsAutoValueProperty); } set { SetValue(IsAutoValueProperty, value); } }
		public static readonly DependencyProperty IsAutoValueProperty = DependencyProperty.Register("IsAutoValue", typeof(bool), typeof(EnumElement), new PropertyMetadata(false, IsAutoValueChangedCallback));

		private static void IsAutoValueChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as EnumElement;
			if (t == null) return;

			if (!Convert.ToBoolean(e.NewValue)) return;
			t.IsCustomValue = false;
			t.IsAggregateValue = false;
		}

		public bool IsCustomValue { get { return (bool)GetValue(IsCustomValueProperty); } set { SetValue(IsCustomValueProperty, value); } }
		public static readonly DependencyProperty IsCustomValueProperty = DependencyProperty.Register("IsCustomValue", typeof(bool), typeof(EnumElement), new PropertyMetadata(false, IsCustomValueChangedCallback));

		private static void IsCustomValueChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as EnumElement;
			if (t == null) return;

			if (!Convert.ToBoolean(e.NewValue)) return;
			t.IsAutoValue = false;
			t.IsAggregateValue = false;
		}

		public bool IsAggregateValue { get { return (bool)GetValue(IsAggregateValueProperty); } set { SetValue(IsAggregateValueProperty, value); } }
		public static readonly DependencyProperty IsAggregateValueProperty = DependencyProperty.Register("IsAggregateValue", typeof(bool), typeof(EnumElement), new PropertyMetadata(false, IsAggregateValueChangedCallback));

		private static void IsAggregateValueChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as EnumElement;
			if (t == null) return;

			if (!Convert.ToBoolean(e.NewValue)) return;
			t.IsAutoValue = false;
			t.IsCustomValue = false;
		}

		public string Documentation { get { return (string)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(string), typeof(EnumElement), new PropertyMetadata(""));

		//Regular Enums
		public long ServerValue { get { return (long)GetValue(ServerValueProperty); } set { SetValue(ServerValueProperty, value); } }
		public static readonly DependencyProperty ServerValueProperty = DependencyProperty.Register("ServerValue", typeof(long), typeof(EnumElement));

		public long? ClientValue { get { return (long?)GetValue(ClientValueProperty); } set { SetValue(ClientValueProperty, value); } }
		public static readonly DependencyProperty ClientValueProperty = DependencyProperty.Register("ClientValue", typeof(long?), typeof(EnumElement));

		//Aggregate Enums
		public ObservableCollection<EnumElement> AggregateValues { get { return (ObservableCollection<EnumElement>)GetValue(AggregateValuesProperty); } set { SetValue(AggregateValuesProperty, value); } }
		public static readonly DependencyProperty AggregateValuesProperty = DependencyProperty.Register("AggregateValues", typeof(ObservableCollection<EnumElement>), typeof(EnumElement));

		public EnumElement()
		{
			ID = Guid.NewGuid();
			IsExcluded = false;
			AggregateValues = new ObservableCollection<EnumElement>();
		}

		public EnumElement(Enum Owner, string Name, long ServerValue, long? ClientValue = null)
		{
			ID = Guid.NewGuid();
			this.Name = Name;
			IsCustomValue = true;
			this.ServerValue = ServerValue;
			this.ClientValue = ClientValue;
			IsExcluded = false;
			AggregateValues = new ObservableCollection<EnumElement>();
			this.Owner = Owner;
		}

		public EnumElement(Enum Owner, string Name)
		{
			ID = Guid.NewGuid();
			this.Name = Name;
			IsExcluded = false;
			IsAutoValue = true;
			this.Owner = Owner;
			AggregateValues = new ObservableCollection<EnumElement>();
		}

		public override string ToString()
		{
			return Name;
		}
	}
}