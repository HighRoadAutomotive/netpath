using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WCFArchitect.Interface.Enum
{
	internal partial class FlagsEnumElement : Grid
	{
		public Projects.EnumElement Data { get; set; }
		private Projects.EnumFlagsDataType DataType { get; set; }

		private Action<FlagsEnumElement, int> FocusUpAction;
		private Action<FlagsEnumElement, int> FocusDownAction;
		private Action<FlagsEnumElement> DeleteAction;

		public FlagsEnumElement(Projects.EnumElement Data, Projects.EnumFlagsDataType DataType, Action<FlagsEnumElement, int> FocusUp, Action<FlagsEnumElement, int> FocusDown, Action<FlagsEnumElement> Delete)
		{
			this.Data = Data;
			this.DataType = DataType;
			this.FocusUpAction = FocusUp;
			this.FocusDownAction = FocusDown;
			this.DeleteAction = Delete;

			InitializeComponent();
		}

		private void ElementName_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) FocusUpAction(this, 0);
			if (e.Key == Key.Down) FocusDownAction(this, 0);
		}

		private void ElementName_GotFocus(object sender, RoutedEventArgs e)
		{
			FlagName.Background = Brushes.White;
		}

		private void ElementName_LostFocus(object sender, RoutedEventArgs e)
		{
			FlagName.Background = Brushes.Transparent;
		}

		private void FlagValue_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) FocusUpAction(this, 1);
			if (e.Key == Key.Down) FocusDownAction(this, 1);
		}

		private void FlagValue_GotFocus(object sender, RoutedEventArgs e)
		{
			FlagValue.Background = Brushes.White;
		}

		private void FlagValue_LostFocus(object sender, RoutedEventArgs e)
		{
			FlagValue.Background = Brushes.Transparent;
		}

		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			DeleteAction(this);
		}
	}
}