using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WCFArchitect.Projects;

namespace WCFArchitect.Interface.Data
{
	internal partial class TypeEditor : ContentControl
	{
		public DataType OpenType { get { return (DataType)GetValue(OpenTypeProperty); } set { SetValue(OpenTypeProperty, value); } }
		public static readonly DependencyProperty OpenTypeProperty = DependencyProperty.Register("OpenType", typeof(DataType), typeof(TypeEditor), new PropertyMetadata(null, OpenTypeChangedCallback));

		public bool CanSetType { get { return (bool)GetValue(CanSetTypeProperty); } set { SetValue(CanSetTypeProperty, value); } }
		public static readonly DependencyProperty CanSetTypeProperty = DependencyProperty.Register("CanSetType", typeof(bool), typeof(TypeEditor), new PropertyMetadata(true, CanSetTypeChangedCallback));

		private static void CanSetTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as TypeEditor;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue))
			{
				t.ClassType.Visibility = Visibility.Visible;
				t.StructType.Visibility = Visibility.Visible;
			}
			else
			{
				t.ClassType.Visibility = Visibility.Hidden;
				t.StructType.Visibility = Visibility.Hidden;
			}
		}

		public bool IsEnumType { get { return (bool)GetValue(IsEnumTypeProperty); } set { SetValue(IsEnumTypeProperty, value); } }
		public static readonly DependencyProperty IsEnumTypeProperty = DependencyProperty.Register("IsEnumType", typeof(bool), typeof(TypeEditor), new PropertyMetadata(false, IsEnumTypeChangedCallback));

		private static void IsEnumTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as TypeEditor;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue) == false)
			{
				t.Partial.Visibility = Visibility.Visible;
				t.Abstract.Visibility = Visibility.Visible;
				t.Sealed.Visibility = Visibility.Visible;
				t.ClassType.Visibility = Visibility.Visible;
				t.StructType.Visibility = Visibility.Visible;
			}
			else
			{
				t.Partial.Visibility = Visibility.Collapsed;
				t.Abstract.Visibility = Visibility.Collapsed;
				t.Sealed.Visibility = Visibility.Collapsed;
				t.ClassType.Visibility = Visibility.Collapsed;
				t.StructType.Visibility = Visibility.Collapsed;
			}
		}

		public bool SupportInheritedTypes { get { return (bool)GetValue(SupportInheritedTypesProperty); } set { SetValue(SupportInheritedTypesProperty, value); } }
		public static readonly DependencyProperty SupportInheritedTypesProperty = DependencyProperty.Register("SupportInheritedTypes", typeof(bool), typeof(TypeEditor), new PropertyMetadata(false));

		public bool SupportKnownTypes { get { return (bool)GetValue(SupportKnownTypesProperty); } set { SetValue(SupportKnownTypesProperty, value); } }
		public static readonly DependencyProperty SupportKnownTypesProperty = DependencyProperty.Register("SupportKnownTypes", typeof(bool), typeof(TypeEditor), new PropertyMetadata(false));

		private static void OpenTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as TypeEditor;
			if (t == null) return;
			var tdt = e.NewValue as DataType;

			if (tdt == null)
			{
				t.SelectInheritedType.Project = null;
				t.IsEnabled = false;
				return;
			}
			t.SelectInheritedType.Project = tdt.Parent.Owner;
			t.SelectKnownType.Project = tdt.Parent.Owner;
			t.IsEnabled = true;
		}

		public TypeEditor()
		{
			InitializeComponent();

			Scope.SelectedIndex = -1;
			IsEnabled = false;
			BG.DataContext = this;
		}

		private void Abstract_Checked(object sender, RoutedEventArgs e)
		{
			Sealed.Visibility = Visibility.Hidden;
			OpenType.Sealed = false;
		}

		private void Abstract_Unchecked(object sender, RoutedEventArgs e)
		{
			Sealed.Visibility = Visibility.Visible;
		}

		private void Sealed_Checked(object sender, RoutedEventArgs e)
		{
			Abstract.Visibility = Visibility.Hidden;
			OpenType.Abstract = false;
		}

		private void Sealed_Unchecked(object sender, RoutedEventArgs e)
		{
			Abstract.Visibility = Visibility.Visible;
		}

		private void ClassType_Checked(object sender, RoutedEventArgs e)
		{
			Abstract.Visibility = Visibility.Visible;
			Sealed.Visibility = Visibility.Visible;
		}

		private void StructType_Checked(object sender, RoutedEventArgs e)
		{
			Abstract.Visibility = Visibility.Hidden;
			Sealed.Visibility = Visibility.Hidden;
			OpenType.Abstract = false;
			OpenType.Sealed = false;
		}

		private void AddInheritedType_Click(object sender, RoutedEventArgs e)
		{
			if(SelectInheritedType.OpenType != null)
				OpenType.InheritedTypes.Add(SelectInheritedType.OpenType);
			else
				return;

			SelectInheritedType.OpenType = null;

			InheritedTypes.Visibility = Visibility.Visible;
		}

		private void DeleteSelectedInheritedType_Click(object sender, RoutedEventArgs e)
		{
			if (sender == null) return;
			var lbi = Globals.GetVisualParent<ListBoxItem>(sender);
			if (lbi == null) return;
			OpenType.InheritedTypes.Remove(lbi.Content as DataType);

			if (OpenType.InheritedTypes.Count == 0)
				InheritedTypes.Visibility = Visibility.Collapsed;
		}

		private void AddKnownType_Click(object sender, RoutedEventArgs e)
		{
			if (SelectKnownType.OpenType != null)
				OpenType.KnownTypes.Add(SelectKnownType.OpenType);
			else
				return;

			SelectKnownType.OpenType = null;

			KnownTypes.Visibility = Visibility.Visible;
		}

		private void DeleteSelectedKnownType_Click(object sender, RoutedEventArgs e)
		{
			if (sender == null) return;
			var lbi = Globals.GetVisualParent<ListBoxItem>(sender);
			if (lbi == null) return;
			OpenType.KnownTypes.Remove(lbi.Content as DataType);

			if (OpenType.KnownTypes.Count == 0)
				KnownTypes.Visibility = Visibility.Collapsed;
		}

		private void ContentControl_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if(IsKeyboardFocusWithin && SupportInheritedTypes)
				InheritedTypesGrid.Visibility = Visibility.Visible;
			else
				InheritedTypesGrid.Visibility = Visibility.Collapsed;
			if(IsKeyboardFocusWithin && SupportKnownTypes)
				KnownTypesGrid.Visibility = Visibility.Visible;
			else
				KnownTypesGrid.Visibility = Visibility.Collapsed;

			if (IsKeyboardFocusWithin && OpenType.InheritedTypes.Count > 0)
				InheritedTypes.Visibility = Visibility.Visible;
			else
				InheritedTypes.Visibility = Visibility.Collapsed;
			if (IsKeyboardFocusWithin && OpenType.KnownTypes.Count > 0)
				KnownTypes.Visibility = Visibility.Visible;
			else
				KnownTypes.Visibility = Visibility.Collapsed;
		}
	}

	[ValueConversion(typeof(DataScope), typeof(int))]
	public class ClassScopeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return -1;
			var lt = (DataScope)value;
			if (lt == DataScope.Public) return 0;
			if (lt == DataScope.Internal) return 1;
			if (lt == DataScope.Disabled) return -1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return null;
			var lt = (int)value;
			if (lt == -1) return DataScope.Disabled;
			if (lt == 0) return DataScope.Public;
			if (lt == 1) return DataScope.Internal;
			return DataScope.Public;
		}
	}
	[ValueConversion(typeof(DataTypeMode), typeof(bool))]
	public class ClassTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (DataTypeMode)value;
			return lt == DataTypeMode.Class;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (bool)value;
			return lt ? DataTypeMode.Class : DataTypeMode.Struct;
		}
	}

	[ValueConversion(typeof(DataTypeMode), typeof(bool))]
	public class StructTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (DataTypeMode) value;
			return lt == DataTypeMode.Struct;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (bool) value;
			return lt ? DataTypeMode.Struct : DataTypeMode.Class;
		}
	}
}