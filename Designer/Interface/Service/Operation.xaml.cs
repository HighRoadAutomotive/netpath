using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WCFArchitect.Interface.Service
{
	internal partial class Operation : Grid
	{
		public Projects.Operation Data { get; set; }

		private int DragItemStartIndex;
		private int DragItemNewIndex;
		private Projects.OperationParameter DragElement;
		private Point DragStartPos;
		private Themes.DragAdorner DragAdorner;
		private AdornerLayer DragLayer;
		public bool IsDragging { get; set; }
		public bool DragHasLeftScope { get; set; }

		public Operation()
		{
			InitializeComponent();
		}

		public Operation(Projects.Operation Data)
		{
			this.Data = Data;

			InitializeComponent();

			this.FilterDataTypeBox.Text = "";
			this.FilterNameBox.Text = "";
			this.AddParameterDataType.Text = "string";

			Data.Parameters.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Parameters_CollectionChanged);
			ValuesList.ItemsSource = Data.Parameters;
		}

		#region - Drag/Drop Support -

		private void ValuesList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragStartPos = e.GetPosition(null);
		}

		private void ValuesList_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			ContentPresenter cp = GetListBoxItemPresenter();
			if (cp == null) return;
			Prospective.Controls.TextBox ts = ValuesList.ItemTemplate.FindName("OperationParameterDataType", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("OperationParameterElementName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;

#if STANDARD
			if (Data.Owner.Parent.Owner.IsDependencyProject == true) return;
#endif

			if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
			{
				Point position = e.GetPosition(null);

				if (Math.Abs(position.X - DragStartPos.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - DragStartPos.Y) > SystemParameters.MinimumVerticalDragDistance)
				{
					StartValueDrag(e);
				}
			}
		}

		private void StartValueDrag(MouseEventArgs e)
		{
			//Here we create our adorner.. 
			DragElement = ValuesList.SelectedItem as Projects.OperationParameter;
			DragItemStartIndex = ValuesList.SelectedIndex;
			UIElement tuie = (UIElement)ValuesList.ItemContainerGenerator.ContainerFromItem(ValuesList.SelectedItem);
			if (tuie == null) return;
			try { DragAdorner = new Themes.DragAdorner(ValuesList, tuie, true, 0.5); }
			catch { return; }
			DragLayer = AdornerLayer.GetAdornerLayer(ValuesList as Visual);
			DragLayer.Add(DragAdorner);

			IsDragging = true;
			DragHasLeftScope = false;

			DataObject data = new DataObject(System.Windows.DataFormats.Text.ToString(), "");
			DragDropEffects de = DragDrop.DoDragDrop(ValuesList, data, DragDropEffects.Move);

			// Clean up our mess :) 
			AdornerLayer.GetAdornerLayer(ValuesList).Remove(DragAdorner);
			DragAdorner = null;

			IsDragging = false;
		}

		private void ValuesList_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			if (DragHasLeftScope)
			{
				e.Action = DragAction.Cancel;
				e.Handled = true;
			}
		}

		private void ValuesList_DragLeave(object sender, DragEventArgs e)
		{
			if (e.OriginalSource == ValuesList)
			{
				Point p = e.GetPosition(ValuesList);
				Rect r = VisualTreeHelper.GetContentBounds(ValuesList);
				if (!r.Contains(p))
				{
					this.DragHasLeftScope = true;
					e.Handled = true;

					foreach (Projects.OperationParameter O in Data.Parameters)
					{
						ListBoxItem lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(O);
						if (lbi == null) continue;
						lbi.Margin = new Thickness(0);
						lbi.Padding = new Thickness(0);
					}
				}
			}
		}

		private void ValuesList_PreviewDragOver(object sender, DragEventArgs e)
		{
			ContentPresenter cp = GetListBoxItemPresenter();
			if (cp == null) return;
			Prospective.Controls.TextBox ts = ValuesList.ItemTemplate.FindName("OperationParameterDataType", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("OperationParameterElementName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;

			if (DragAdorner != null)
			{
				DragAdorner.LeftOffset = e.GetPosition(ValuesList).X;
				DragAdorner.TopOffset = e.GetPosition(ValuesList).Y;
			}

			Point MP = e.GetPosition(ValuesList);
			HitTestResult htr = VisualTreeHelper.HitTest(ValuesList, MP);
			if (htr != null)
			{
				foreach (Projects.OperationParameter O in Data.Parameters)
				{
					ListBoxItem lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(O);
					ListBoxItem lbiht = Globals.GetVisualParent<ListBoxItem>(htr.VisualHit);
					if (lbi == null) continue;
					if (lbiht == null) continue;
					if (lbi != lbiht)
					{
						lbi.Margin = new Thickness(0);
						lbi.Padding = new Thickness(0);
					}
					else
					{
						lbi.Margin = new Thickness(0, DragAdorner.ActualHeight, 0, 0);
						lbi.Padding = new Thickness(0);
						DragItemNewIndex = ValuesList.ItemContainerGenerator.IndexFromContainer(lbi);
					}
				}
			}
		}

		private void ValuesList_Drop(object sender, DragEventArgs e)
		{
			Data.Parameters.Move(DragItemStartIndex, DragItemNewIndex);

			foreach (Projects.OperationParameter O in Data.Parameters)
			{
				ListBoxItem lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(O);
				if (lbi == null) continue;
				lbi.Margin = new Thickness(0);
				lbi.Padding = new Thickness(0);
			}
		}

		#endregion

		private ContentPresenter GetListBoxItemPresenter()
		{
			if (ValuesList.SelectedItem == null) return null;
			ListBoxItem lbi = ValuesList.ItemContainerGenerator.ContainerFromIndex(ValuesList.SelectedIndex) as ListBoxItem;
			return Globals.GetVisualChild<ContentPresenter>(lbi);
		}

		private void CodeName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.CodeName = Helpers.RegExs.ReplaceSpaces.Replace(CodeName.Text, @"");
		}

		private void CodeName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(CodeName.Text);
		}

		private void ContractName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.ContractName = Helpers.RegExs.ReplaceSpaces.Replace(Data.ContractName, @"");
		}

		private void ContractName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(ContractName.Text);
		}

		private void IsOneWay_Checked(object sender, RoutedEventArgs e)
		{
			IsOneWay.Content = "Yes";
		}

		private void IsOneWay_Unchecked(object sender, RoutedEventArgs e)
		{
			IsOneWay.Content = "No";
		}

		private void IsInitiating_Checked(object sender, RoutedEventArgs e)
		{
			IsInitiating.Content = "Yes";
		}

		private void IsInitiating_Unchecked(object sender, RoutedEventArgs e)
		{
			IsInitiating.Content = "No";
		}

		private void IsTerminating_Checked(object sender, RoutedEventArgs e)
		{
			IsTerminating.Content = "Yes";
		}

		private void IsTerminating_Unchecked(object sender, RoutedEventArgs e)
		{
			IsTerminating.Content = "No";
		}

		private void UseAsyncPattern_Checked(object sender, RoutedEventArgs e)
		{
			UseAsyncPattern.Content = "Yes";
		}

		private void UseAsyncPattern_Unchecked(object sender, RoutedEventArgs e)
		{
			UseAsyncPattern.Content = "No";
		}

		private void AddParameterDataType_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				AddParameterName.Focus();
		}

		private void AddParameterDataType_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (AddParameterDataType.IsInvalid == false || AddParameterName.IsInvalid == false)
			{
				AddParameter.IsEnabled = false;
				return;
			}
			if (AddParameterDataType.Text != "" && AddParameterName.Text != "") AddParameter.IsEnabled = true;
			else AddParameter.IsEnabled = false;
		}

		private void AddParameterDataType_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = true;
			if (AddParameterDataType.Text == "" || AddParameterDataType == null) return;

			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(AddParameterDataType.Text);
			AddParameter.IsEnabled = e.IsValid;
		}

		private void AddParameterName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				AddParameter_Click(null, null);
		}

		private void AddParameterName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (AddParameterDataType.IsInvalid == false || AddParameterName.IsInvalid == false)
			{
				AddParameter.IsEnabled = false;
				return;
			}
			if (AddParameterDataType.Text != "" && AddParameterName.Text != "") AddParameter.IsEnabled = true;
			else AddParameter.IsEnabled = false;
		}

		private void AddParameterName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = true;
			if (AddParameterName.Text == "" || AddParameterName == null) return;

			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(AddParameterName.Text);
			AddParameter.IsEnabled = e.IsValid;
		}

		private void AddParameter_Click(object sender, RoutedEventArgs e)
		{
			if (AddParameter.IsEnabled == false) return;
			Data.Parameters.Add(new Projects.OperationParameter(AddParameterDataType.Text, AddParameterName.Text, Data.Owner));
			AddParameterDataType.Focus();
			AddParameterDataType.Text = "string";
			AddParameterDataType.SelectAll();
			AddParameterName.Text = "";

			Data.Owner.IsDirty = true;
		}

		private void FilterBarDataType_Click(object sender, RoutedEventArgs e)
		{
			FilterBarDataType.Visibility = System.Windows.Visibility.Hidden;
			FilterDataTypeBox.Focus();
		}

		private void FilterDataTypeBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ApplyFiter();
		}

		private void FilterDataTypeBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (FilterDataTypeBox.Text == "")
				FilterBarDataType.Visibility = System.Windows.Visibility.Visible;
		}

		private void FilterBarName_Click(object sender, RoutedEventArgs e)
		{
			FilterBarName.Visibility = System.Windows.Visibility.Hidden;
			FilterNameBox.Focus();
		}

		private void FilterNameBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ApplyFiter();
		}

		private void FilterNameBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (FilterNameBox.Text == "")
				FilterBarName.Visibility = System.Windows.Visibility.Visible;
		}

		private void ApplyFiter()
		{
			foreach (Projects.OperationParameter OP in Data.Parameters)
			{
				ListBoxItem lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(OP);
				if (lbi == null) continue;
				lbi.Visibility = System.Windows.Visibility.Visible;

				if (OP.DataType.IndexOf(FilterDataTypeBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) lbi.Visibility = System.Windows.Visibility.Collapsed;
				if (OP.Name.IndexOf(FilterNameBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) lbi.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		private void Parameters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Data.Owner.IsDirty = true;
		}

		private void OperationParameterDataType_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (ValuesList.SelectedIndex == 0) ValuesList.SelectedIndex = Data.Parameters.Count - 1; else ValuesList.SelectedIndex--;
			if (e.Key == Key.Down) if (ValuesList.SelectedIndex == Data.Parameters.Count - 1) ValuesList.SelectedIndex = 0; else ValuesList.SelectedIndex++;

			if (e.Key == Key.Up || e.Key == Key.Down)
			{
				Prospective.Controls.TextBox ts = ValuesList.ItemTemplate.FindName("OperationParameterDataType", GetListBoxItemPresenter()) as Prospective.Controls.TextBox;
				if (ts == null) return;
				ts.Focus(); ;
			}
		}

		private void OperationParameterElementName_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (ValuesList.SelectedIndex == 0) ValuesList.SelectedIndex = Data.Parameters.Count - 1; else ValuesList.SelectedIndex--;
			if (e.Key == Key.Down) if (ValuesList.SelectedIndex == Data.Parameters.Count - 1) ValuesList.SelectedIndex = 0; else ValuesList.SelectedIndex++;

			if (e.Key == Key.Up || e.Key == Key.Down)
			{
				Prospective.Controls.TextBox ts = ValuesList.ItemTemplate.FindName("OperationParameterElementName", GetListBoxItemPresenter()) as Prospective.Controls.TextBox;
				if (ts == null) return;
				ts.Focus();
			}
		}

		private void OperationParameterElementName_GotFocus(object sender, RoutedEventArgs e)
		{
			Prospective.Controls.TextBox ts = sender as Prospective.Controls.TextBox;
			ts.Background = Brushes.White;
		}

		private void OperationParameterElementName_LostFocus(object sender, RoutedEventArgs e)
		{
			Prospective.Controls.TextBox ts = sender as Prospective.Controls.TextBox;
			ts.Background = Brushes.Transparent;
		}

		private void OperationParameterDataType_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			Prospective.Controls.TextBox DataType = sender as Prospective.Controls.TextBox;
			Projects.OperationParameter DE = ValuesList.SelectedItem as Projects.OperationParameter;
			if (DE == null) return;

			e.IsValid = true;
			if ((DataType.Text.IndexOf("[") >= 0 || DataType.Text.IndexOf("]") >= 0) && DE.IsValidArrayType(DataType.Text) == false) e.IsValid = false;
			if ((DataType.Text.IndexOf("<") >= 0 || DataType.Text.IndexOf(">") >= 0) && DataType.Text.IndexOf(",") < 0 && DE.IsValidCollectionType(DataType.Text) == false) e.IsValid = false;
			if ((DataType.Text.IndexOf("<") >= 0 || DataType.Text.IndexOf(">") >= 0) && DataType.Text.IndexOf(",") >= 0 && DE.IsValidDictionaryType(DataType.Text) == false) e.IsValid = false;
		}

		private void OperationParameterElementName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			Prospective.Controls.TextBox ElementName = sender as Prospective.Controls.TextBox;
			string t = Helpers.RegExs.ReplaceSpaces.Replace(ElementName.Text == null ? "" : ElementName.Text, @"");

			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(t);
		}

		private void DeleteOperationParameter_Click(object sender, RoutedEventArgs e)
		{
			Projects.OperationParameter OP = ValuesList.SelectedItem as Projects.OperationParameter;
			if (OP == null) return;

			if (Prospective.Controls.MessageBox.Show("Are you sure you wish to delete the '" + OP.DataType + " " + OP.Name + "' parameter?", "Delete Operation Parameter?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
			Data.Parameters.Remove(OP);
		}
	}
}