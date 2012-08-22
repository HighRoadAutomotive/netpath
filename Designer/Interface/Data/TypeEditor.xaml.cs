using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WCFArchitect.Interface.Data
{
	public partial class TypeEditor : ContentControl
	{
		public Projects.DataType Type { get { return (Projects.DataType)GetValue(TypeProperty); } set { SetValue(TypeProperty, value); if (value.TypeMode == Projects.DataTypeMode.Struct) { StructType.IsChecked = true; } else { ClassType.IsChecked = true; } } }
		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Projects.DataType), typeof(TypeEditor));

		public TypeEditor()
		{
			InitializeComponent();
		}

		private void ClassType_Checked(object sender, RoutedEventArgs e)
		{
			StructType.IsChecked = false;
			IsAbstract.Visibility = System.Windows.Visibility.Visible;
			Type.TypeMode = Projects.DataTypeMode.Class;
		}

		private void StructType_Checked(object sender, RoutedEventArgs e)
		{
			ClassType.IsChecked = false;
			IsAbstract.Visibility = System.Windows.Visibility.Collapsed;
			Type.TypeMode = Projects.DataTypeMode.Struct;
		}

		private void AddInheritedType_Click(object sender, RoutedEventArgs e)
		{

		}

		private void DeleteSelectedInheritedType_Click(object sender, RoutedEventArgs e)
		{
			if (InheritedTypes.SelectedItem == null) return;

			Type.InheritedTypes.Remove(InheritedTypes.SelectedItem as Projects.DataType);
		}
	}
}