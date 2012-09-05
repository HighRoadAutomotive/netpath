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
		public static readonly DependencyProperty CanSetTypeProperty = DependencyProperty.Register("CanSetType", typeof(bool), typeof(TypeEditor), new PropertyMetadata(true));

		private static void OpenTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as TypeEditor;
			if (t == null) return;
			var tdt = e.NewValue as DataType;

			if (tdt == null)
			{
				t.IsEnabled = false;
				return;
			} 
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

		}

		private void DeleteSelectedInheritedType_Click(object sender, RoutedEventArgs e)
		{
			if (InheritedTypes.SelectedItem == null) return;

			OpenType.InheritedTypes.Remove(InheritedTypes.SelectedItem as DataType);
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