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

namespace WCFArchitect.Interface.Service
{
	internal partial class OperationParameter : Grid
	{
		public Projects.OperationParameter Data { get; set; }

		private Action<OperationParameter, bool> FocusUpAction;
		private Action<OperationParameter, bool> FocusDownAction;
		private Action<OperationParameter> DeleteAction;

		public OperationParameter(Projects.OperationParameter Data, Action<OperationParameter, bool> FocusUp, Action<OperationParameter, bool> FocusDown, Action<OperationParameter> Delete)
		{
			this.Data = Data;
			this.FocusUpAction = FocusUp;
			this.FocusDownAction = FocusDown;
			this.DeleteAction = Delete;
		
			InitializeComponent();
		}

		private void Grid_GotFocus(object sender, RoutedEventArgs e)
		{
			Background = new SolidColorBrush(Color.FromArgb(255, 238, 238, 238));
		}

		private void Grid_LostFocus(object sender, RoutedEventArgs e)
		{
			Background = Brushes.White;
		}

		private void DataType_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) FocusUpAction(this, true);
			if (e.Key == Key.Down) FocusDownAction(this, true);
		}

		private void DataType_GotFocus(object sender, RoutedEventArgs e)
		{
			DataType.Background = Brushes.White;
		}

		private void DataType_LostFocus(object sender, RoutedEventArgs e)
		{
			DataType.Background = Brushes.Transparent;
		}

		private void ElementName_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) FocusUpAction(this, false);
			if (e.Key == Key.Down) FocusDownAction(this, false);
		}

		private void ElementName_GotFocus(object sender, RoutedEventArgs e)
		{
			ElementName.Background = Brushes.White;
		}

		private void ElementName_LostFocus(object sender, RoutedEventArgs e)
		{
			ElementName.Background = Brushes.Transparent;
		}

		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			DeleteAction(this);
		}
	}
}