using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace WCFArchitect.Interface.Data
{
	internal partial class DataElement : Grid
	{
		public Projects.DataElement Data { get; set; }

		private Action<DataElement, bool> FocusUpAction;
		private Action<DataElement, bool> FocusDownAction;
		private Action<DataElement> MoveUpAction;
		private Action<DataElement> MoveDownAction;
		private Action<DataElement> DeleteAction;
		private Action<DataElement> DataElementGotFocus;

		private bool forceExpand;
		public bool ForceExpand
		{ 
			get { return forceExpand; } 
			set { 
				forceExpand = value;
				if (value == true)
				{
					DetailsGrid.Visibility = System.Windows.Visibility.Visible;
				}
				else
				{
					if (IsFocused == false)
					{
						DetailsGrid.Visibility = System.Windows.Visibility.Collapsed;
					}
				}
			} 
		}

		public DataElement(Projects.DataElement Data, Action<DataElement, bool> FocusUp, Action<DataElement, bool> FocusDown, Action<DataElement> MoveUp, Action<DataElement> MoveDown, Action<DataElement> Delete, Action<DataElement> DataElementGotFocus)
		{
			this.Data = Data;
			this.FocusUpAction = FocusUp;
			this.FocusDownAction = FocusDown;
			this.MoveUpAction = MoveUp;
			this.MoveDownAction = MoveDown;
			this.DeleteAction = Delete;
			this.DataElementGotFocus = DataElementGotFocus;

			InitializeComponent();

			this.ForceExpand = false;
			if (Data.GenerateWPFBinding == true)
			{
				IsReadOnly.IsEnabled = true;
				IsAttached.IsEnabled = true;
			}
		}

		private void Grid_GotFocus(object sender, RoutedEventArgs e)
		{
			DetailsGrid.Visibility = System.Windows.Visibility.Visible;
			Background = new SolidColorBrush(Color.FromArgb(255, 245, 245, 245));
			DataElementGotFocus(this);
		}

		public void ElementLostFocus()
		{
			if (ForceExpand == true) return;
			DetailsGrid.Visibility = System.Windows.Visibility.Collapsed;
			Background = Brushes.White;
		}

		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			DataType.MaxWidth = ItemsGridCD2.ActualWidth;
			ElementName.MaxWidth = ItemsGridCD3.ActualWidth;
		}

		private void Scope_GotFocus(object sender, RoutedEventArgs e)
		{
			ScopeName.Visibility = System.Windows.Visibility.Collapsed;
			ScopeBorder.Visibility = System.Windows.Visibility.Visible;
		}

		private void Scope_LostFocus(object sender, RoutedEventArgs e)
		{
			ScopeBorder.Visibility = System.Windows.Visibility.Collapsed;
			ScopeName.Visibility = System.Windows.Visibility.Visible;
		}

		private void Scope_SelectedIndexChanged(object sender, C1.WPF.PropertyChangedEventArgs<int> e)
		{
			if (IsLoaded == false) return;
			Data.Scope = (Projects.DataScope)e.NewValue;
		}

		private void ScopeName_GotFocus(object sender, RoutedEventArgs e)
		{
			ScopeName.Visibility = System.Windows.Visibility.Collapsed;
			ScopeBorder.Visibility = System.Windows.Visibility.Visible;
			Scope.Focus();
			Scope.IsDropDownOpen = true;
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

		private void DataType_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = true;
			if ((DataType.Text.IndexOf("[") >= 0 || DataType.Text.IndexOf("]") >= 0) && Data.IsValidArrayType(DataType.Text) == false) e.IsValid = false;
			if ((DataType.Text.IndexOf("<") >= 0 || DataType.Text.IndexOf(">") >= 0) && DataType.Text.IndexOf(",") < 0 && Data.IsValidCollectionType(DataType.Text) == false) e.IsValid = false;
			if ((DataType.Text.IndexOf("<") >= 0 || DataType.Text.IndexOf(">") >= 0) && DataType.Text.IndexOf(",") >= 0 && Data.IsValidDictionaryType(DataType.Text) == false) e.IsValid = false;
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

		private void Order_ValueChanged(object sender, C1.WPF.PropertyChangedEventArgs<double> e)
		{
			Data.Order = Convert.ToInt32(e.NewValue);
		}

		private void WPFDataType_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			if (WPFDataType.Text == "" || WPFDataType.Text == null)
			{
				e.IsValid = true;
				WPFDataType.Text = "";
				return;
			}

			if (Data.IsValidMemberType(WPFDataType.Text) == true)
			{
				e.IsValid = true;
				Data.IsArray = false;
				Data.IsCollection = false;
				Data.IsDictionary = false;
				WPFDataMode.Text = "Value";
			}
			else if (Data.IsValidArrayType(WPFDataType.Text) == true)
			{
				e.IsValid = true;
				Data.IsArray = true;
				Data.IsCollection = false;
				Data.IsDictionary = false;
				WPFDataMode.Text = "Array";
			}
			else if (Data.IsValidCollectionType(WPFDataType.Text) == true)
			{
				e.IsValid = true;
				Data.IsArray = false;
				Data.IsCollection = true;
				Data.IsDictionary = false;
				WPFDataMode.Text = "Collection";
			}
			else if (Data.IsValidDictionaryType(WPFDataType.Text) == true)
			{
				e.IsValid = true;
				Data.IsArray = false;
				Data.IsCollection = false;
				Data.IsDictionary = true;
				WPFDataMode.Text = "Dictionary";
			}
			else
			{
				e.IsValid = false;
				Data.IsArray = false;
				Data.IsCollection = false;
				Data.IsDictionary = false;
				WPFDataMode.Text = "Invalid";
			}
		}

		private void MoveUp_Click(object sender, RoutedEventArgs e)
		{
			MoveUpAction(this);
		}

		private void MoveDown_Click(object sender, RoutedEventArgs e)
		{
			MoveDownAction(this);
		}

		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			DeleteAction(this);
		}
	}
}