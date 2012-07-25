using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
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

namespace WCFArchitect.Interface.Enum
{
	internal partial class Enum : Grid
	{
		public Projects.Enum Data { get; set; }

		private int DragItemStartIndex;
		private int DragItemNewIndex;
		private Projects.EnumElement DragElement;
		private Point DragStartPos;
		private Themes.DragAdorner DragAdorner;
		private AdornerLayer DragLayer;
		public bool IsDragging { get; set; }
		public bool DragHasLeftScope { get; set; }

		public Enum(Projects.Enum Data)
		{
			this.Data = Data;

			InitializeComponent();

			if (Data.IsFlags == false)
			{
				FlagsDataType.Visibility = System.Windows.Visibility.Collapsed;
				Flags.Visibility = System.Windows.Visibility.Collapsed;
				DataType.Visibility = System.Windows.Visibility.Visible;
				Values.Visibility = System.Windows.Visibility.Visible;

				FlagsList.ItemsSource = null;
				ValuesList.ItemsSource = Data.Elements;
			}
			else
			{
				FlagsDataType.Visibility = System.Windows.Visibility.Visible;
				Flags.Visibility = System.Windows.Visibility.Visible;
				DataType.Visibility = System.Windows.Visibility.Collapsed;
				Values.Visibility = System.Windows.Visibility.Collapsed;

				ValuesList.ItemsSource = null;
				FlagsList.ItemsSource = Data.Elements;
			}

			Data.Elements.CollectionChanged += new NotifyCollectionChangedEventHandler(Elements_CollectionChanged);
		}

		#region - Value Drag/Drop Support -

		private void ValuesList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragStartPos = e.GetPosition(null);
		}

		private void ValuesList_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			ContentPresenter cp = GetValuesListBoxItemPresenter();
			if (cp == null) return;
			Prospective.Controls.TextBox ts = ValuesList.ItemTemplate.FindName("ValueName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("Value", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("ContractValue", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
#if STANDARD
			if (Data.Parent.Owner.IsDependencyProject == true) return;
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
			DragElement = ValuesList.SelectedItem as Projects.EnumElement;
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

					foreach (Projects.EnumElement O in Data.Elements)
					{
						ListBoxItem lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(O);
						if (lbi == null) continue;
						lbi.Margin = new Thickness(0);
					}
				}
			}
		}

		private void ValuesList_PreviewDragOver(object sender, DragEventArgs e)
		{
			ContentPresenter cp = GetValuesListBoxItemPresenter();
			if (cp == null) return;
			Prospective.Controls.TextBox ts = ValuesList.ItemTemplate.FindName("ValueName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("Value", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("ContractValue", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;

			if (DragAdorner != null)
			{
				DragAdorner.LeftOffset = e.GetPosition(ValuesList).X;
				DragAdorner.TopOffset = e.GetPosition(ValuesList).Y;
			}

			Point MP = e.GetPosition(ValuesList);
			HitTestResult htr = VisualTreeHelper.HitTest(ValuesList, MP);
			if (htr != null)
			{
				foreach (Projects.EnumElement O in Data.Elements)
				{
					ListBoxItem lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(O);
					ListBoxItem lbiht = Globals.GetVisualParent<ListBoxItem>(htr.VisualHit);
					if (lbi == null) continue;
					if (lbiht == null) continue;
					if (lbi != lbiht)
					{
						lbi.Margin = new Thickness(0);
					}
					else
					{
						lbi.Margin = new Thickness(0, DragAdorner.ActualHeight, 0, 0);
						DragItemNewIndex = ValuesList.ItemContainerGenerator.IndexFromContainer(lbi);
					}
				}
			}
		}

		private void ValuesList_Drop(object sender, DragEventArgs e)
		{
			Data.Elements.Move(DragItemStartIndex, DragItemNewIndex);

			foreach (Projects.EnumElement O in Data.Elements)
			{
				ListBoxItem lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(O);
				if (lbi == null) continue;
				lbi.Margin = new Thickness(0);
			}
		}

		#endregion

		#region - Flags Drag/Drop Support -

		private void FlagsList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragStartPos = e.GetPosition(null);
		}

		private void FlagsList_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			ContentPresenter cp = GetFlagsListBoxItemPresenter();
			if (cp == null) return;
			Prospective.Controls.TextBox ts = FlagsList.ItemTemplate.FindName("FlagName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = FlagsList.ItemTemplate.FindName("FlagValue", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
#if STANDARD
			if (Data.Parent.Owner.IsDependencyProject == true) return;
#endif
			if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
			{
				Point position = e.GetPosition(null);

				if (Math.Abs(position.X - DragStartPos.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - DragStartPos.Y) > SystemParameters.MinimumVerticalDragDistance)
				{
					StartFlagDrag(e);
				}
			}
		}

		private void StartFlagDrag(MouseEventArgs e)
		{
			//Here we create our adorner.. 
			DragElement = FlagsList.SelectedItem as Projects.EnumElement;
			DragItemStartIndex = FlagsList.SelectedIndex;
			UIElement tuie = (UIElement)FlagsList.ItemContainerGenerator.ContainerFromItem(FlagsList.SelectedItem);
			if (tuie == null) return;
			try { DragAdorner = new Themes.DragAdorner(FlagsList, tuie, true, 0.5); }
			catch { return; }
			DragLayer = AdornerLayer.GetAdornerLayer(FlagsList as Visual);
			DragLayer.Add(DragAdorner);

			IsDragging = true;
			DragHasLeftScope = false;

			DataObject data = new DataObject(System.Windows.DataFormats.Text.ToString(), "");
			DragDropEffects de = DragDrop.DoDragDrop(FlagsList, data, DragDropEffects.Move);

			// Clean up our mess :) 
			AdornerLayer.GetAdornerLayer(FlagsList).Remove(DragAdorner);
			DragAdorner = null;

			IsDragging = false;
		}

		private void FlagsList_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			if (DragHasLeftScope)
			{
				e.Action = DragAction.Cancel;
				e.Handled = true;
			}
		}

		private void FlagsList_DragLeave(object sender, DragEventArgs e)
		{
			if (e.OriginalSource == FlagsList)
			{
				Point p = e.GetPosition(FlagsList);
				Rect r = VisualTreeHelper.GetContentBounds(FlagsList);
				if (!r.Contains(p))
				{
					this.DragHasLeftScope = true;
					e.Handled = true;

					foreach (Projects.EnumElement O in Data.Elements)
					{
						ListBoxItem lbi = (ListBoxItem)FlagsList.ItemContainerGenerator.ContainerFromItem(O);
						if (lbi == null) continue;
						lbi.Margin = new Thickness(0);
					}
				}
			}
		}

		private void FlagsList_PreviewDragOver(object sender, DragEventArgs e)
		{
			ContentPresenter cp = GetFlagsListBoxItemPresenter();
			if (cp == null) return;
			Prospective.Controls.TextBox ts = FlagsList.ItemTemplate.FindName("FlagName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = FlagsList.ItemTemplate.FindName("FlagValue", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;

			if (DragAdorner != null)
			{
				DragAdorner.LeftOffset = e.GetPosition(FlagsList).X;
				DragAdorner.TopOffset = e.GetPosition(FlagsList).Y;
			}

			Point MP = e.GetPosition(FlagsList);
			HitTestResult htr = VisualTreeHelper.HitTest(FlagsList, MP);
			if (htr != null)
			{
				foreach (Projects.EnumElement O in Data.Elements)
				{
					ListBoxItem lbi = (ListBoxItem)FlagsList.ItemContainerGenerator.ContainerFromItem(O);
					ListBoxItem lbiht = Globals.GetVisualParent<ListBoxItem>(htr.VisualHit);
					if (lbi == null) continue;
					if (lbiht == null) continue;
					if (lbi != lbiht)
					{
						lbi.Margin = new Thickness(0);
					}
					else
					{
						lbi.Margin = new Thickness(0, DragAdorner.ActualHeight, 0, 0);
						DragItemNewIndex = FlagsList.ItemContainerGenerator.IndexFromContainer(lbi);
					}
				}
			}
		}

		private void FlagsList_Drop(object sender, DragEventArgs e)
		{
			Data.Elements.Move(DragItemStartIndex, DragItemNewIndex);

			foreach (Projects.EnumElement O in Data.Elements)
			{
				ListBoxItem lbi = (ListBoxItem)FlagsList.ItemContainerGenerator.ContainerFromItem(O);
				if (lbi == null) continue;
				lbi.Margin = new Thickness(0);
			}
		}

		#endregion

		private ContentPresenter GetValuesListBoxItemPresenter()
		{
			if (ValuesList.SelectedItem == null) return null;
			ListBoxItem lbi = ValuesList.ItemContainerGenerator.ContainerFromIndex(ValuesList.SelectedIndex) as ListBoxItem;
			return Globals.GetVisualChild<ContentPresenter>(lbi);
		}

		private ContentPresenter GetFlagsListBoxItemPresenter()
		{
			if (FlagsList.SelectedItem == null) return null;
			ListBoxItem lbi = FlagsList.ItemContainerGenerator.ContainerFromIndex(FlagsList.SelectedIndex) as ListBoxItem;
			return Globals.GetVisualChild<ContentPresenter>(lbi);
		}

		private void CodeName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(CodeName.Text);
		}

		private void ContractName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(ContractName.Text);
		}

		private void IsReference_Checked(object sender, RoutedEventArgs e)
		{
			IsReference.Content = "Yes";
		}

		private void IsReference_Unchecked(object sender, RoutedEventArgs e)
		{
			IsReference.Content = "No";
		}

		private void IsFlags_Checked(object sender, RoutedEventArgs e)
		{
			IsFlags.Content = "Yes";
			if (IsLoaded == false) return;
			FlagsDataType.Visibility = System.Windows.Visibility.Visible;
			Flags.Visibility = System.Windows.Visibility.Visible;
			DataType.Visibility = System.Windows.Visibility.Collapsed;
			Values.Visibility = System.Windows.Visibility.Collapsed;

			ValuesList.ItemsSource = null;
			FlagsList.ItemsSource = Data.Elements;
		}

		private void IsFlags_Unchecked(object sender, RoutedEventArgs e)
		{
			IsFlags.Content = "No";
			if (IsLoaded == false) return;
			FlagsDataType.Visibility = System.Windows.Visibility.Collapsed;
			Flags.Visibility = System.Windows.Visibility.Collapsed;
			DataType.Visibility = System.Windows.Visibility.Visible;
			Values.Visibility = System.Windows.Visibility.Visible;

			FlagsList.ItemsSource = null;
			ValuesList.ItemsSource = Data.Elements;
		}

		private void AddValueName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				AddValueValue.Focus();
		}

		private void AddValueName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (AddValueName.Text == "") AddValue.IsEnabled = false;
			else AddValue.IsEnabled = true;
		}

		private void AddValueValue_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				AddValueContractValue.Focus();
		}

		private void AddValueContractValue_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				AddValue_Click(null, null);
		}

		private void AddValue_Click(object sender, RoutedEventArgs e)
		{
			if (AddValue.IsEnabled == false) return;
			Data.Elements.Add(new Projects.EnumElement(AddValueName.Text, AddValueValue.Text, AddValueContractValue.Text, Data));
			AddValueName.Text = "";
			AddValueName.Focus();
			AddValueValue.Text = "";
			AddValueContractValue.Text = "";
		}

		private void ValueFilterBarName_Click(object sender, RoutedEventArgs e)
		{
			ValueFilterBarName.Visibility = System.Windows.Visibility.Hidden;
			ValueFilterNameBox.Focus();
		}

		private void ValueFilterNameBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ValueApplyFilter();
		}

		private void ValueFilterNameBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (ValueFilterNameBox.Text == "")
				ValueFilterBarName.Visibility = System.Windows.Visibility.Visible;
		}

		private void ValueFilterBarValue_Click(object sender, RoutedEventArgs e)
		{
			ValueFilterBarValue.Visibility = System.Windows.Visibility.Hidden;
			ValueFilterValueBox.Focus();
		}

		private void ValueFilterValueBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ValueApplyFilter();
		}

		private void ValueFilterValueBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (ValueFilterValueBox.Text == "")
				ValueFilterBarValue.Visibility = System.Windows.Visibility.Visible;
		}

		private void ValueFilterBarContract_Click(object sender, RoutedEventArgs e)
		{
			ValueFilterBarContract.Visibility = System.Windows.Visibility.Hidden;
			ValueFilterContractBox.Focus();
		}

		private void ValueFilterContractBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ValueApplyFilter();
		}

		private void ValueFilterContractBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (ValueFilterContractBox.Text == "")
				ValueFilterBarContract.Visibility = System.Windows.Visibility.Visible;
		}

		private void ValueApplyFilter()
		{
			foreach (Projects.EnumElement EE in Data.Elements)
			{
				ListBoxItem lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(EE);
				if (lbi == null) continue;

				lbi.Visibility = System.Windows.Visibility.Collapsed;
				if (EE.Name.IndexOf(ValueFilterNameBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				if (EE.Value.IndexOf(ValueFilterValueBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				if (EE.ContractValue.IndexOf(ValueFilterContractBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				lbi.Visibility = System.Windows.Visibility.Visible;
			}
		}

		private void AddFlagName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				AddFlagValue.Focus();
		}

		private void AddFlagName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (AddFlagName.Text == "") AddFlag.IsEnabled = false;
			else AddFlag.IsEnabled = true;
		}

		private void AddFlagValue_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				AddFlag_Click(null, null);
		}

		private void AddFlag_Click(object sender, RoutedEventArgs e)
		{
			if (AddFlag.IsEnabled == false) return;
			Data.Elements.Add(new Projects.EnumElement(AddFlagName.Text, AddFlagValue.Text, Data));
			AddFlagName.Focus();
			AddFlagName.Text = "";
			AddFlagValue.Text = "";
		}

		private void FlagFilterBarName_Click(object sender, RoutedEventArgs e)
		{
			FlagFilterBarName.Visibility = System.Windows.Visibility.Hidden;
			FlagFilterNameBox.Focus();
		}

		private void FlagFilterNameBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			FlagApplyFilter();
		}

		private void FlagFilterNameBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (FlagFilterNameBox.Text == "")
				FlagFilterBarName.Visibility = System.Windows.Visibility.Visible;
		}

		private void FlagFilterBarValue_Click(object sender, RoutedEventArgs e)
		{
			FlagFilterBarValue.Visibility = System.Windows.Visibility.Hidden;
			FlagFilterValueBox.Focus();
		}

		private void FlagFilterValueBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			FlagApplyFilter();
		}

		private void FlagFilterValueBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (FlagFilterValueBox.Text == "")
				FlagFilterBarValue.Visibility = System.Windows.Visibility.Visible;
		}

		private void FlagApplyFilter()
		{
			foreach (Projects.EnumElement EE in Data.Elements)
			{
				ListBoxItem lbi = (ListBoxItem)FlagsList.ItemContainerGenerator.ContainerFromItem(EE);
				if (lbi == null) continue;

				lbi.Visibility = System.Windows.Visibility.Collapsed;
				if (EE.Name.IndexOf(FlagFilterNameBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				if (EE.FlagValue.IndexOf(FlagFilterValueBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				lbi.Visibility = System.Windows.Visibility.Visible;
			}
		}

		private void Elements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Data.IsDirty = true;
		}

		private void ValueName_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (ValuesList.SelectedIndex == 0) ValuesList.SelectedIndex = Data.Elements.Count - 1; else ValuesList.SelectedIndex--;
			if (e.Key == Key.Down) if (ValuesList.SelectedIndex == Data.Elements.Count - 1) ValuesList.SelectedIndex = 0; else ValuesList.SelectedIndex++;

			if (e.Key == Key.Up || e.Key == Key.Down)
			{
				ListBoxItem lbi = ValuesList.ItemContainerGenerator.ContainerFromIndex(ValuesList.SelectedIndex) as ListBoxItem;
				ContentPresenter cp = Globals.GetVisualChild<ContentPresenter>(lbi);
				Prospective.Controls.TextBox ts = ValuesList.ItemTemplate.FindName("ValueName", cp) as Prospective.Controls.TextBox;
				if (ts == null) return;
				ts.Focus();
			}
		}

		private void ValueName_GotFocus(object sender, RoutedEventArgs e)
		{
			Prospective.Controls.TextBox ValueName = sender as Prospective.Controls.TextBox;
			ValueName.Background = Brushes.White;
		}

		private void ValueName_LostFocus(object sender, RoutedEventArgs e)
		{
			Prospective.Controls.TextBox ValueName = sender as Prospective.Controls.TextBox;
			ValueName.Background = Brushes.Transparent;
		}

		private void Value_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (ValuesList.SelectedIndex == 0) ValuesList.SelectedIndex = Data.Elements.Count - 1; else ValuesList.SelectedIndex--;
			if (e.Key == Key.Down) if (ValuesList.SelectedIndex == Data.Elements.Count - 1) ValuesList.SelectedIndex = 0; else ValuesList.SelectedIndex++;

			if (e.Key == Key.Up || e.Key == Key.Down)
			{
				ListBoxItem lbi = ValuesList.ItemContainerGenerator.ContainerFromIndex(ValuesList.SelectedIndex) as ListBoxItem;
				ContentPresenter cp = Globals.GetVisualChild<ContentPresenter>(lbi);
				Prospective.Controls.TextBox ts = ValuesList.ItemTemplate.FindName("Value", cp) as Prospective.Controls.TextBox;
				if (ts == null) return;
				ts.Focus();
			}
		}

		private void Value_GotFocus(object sender, RoutedEventArgs e)
		{
			Prospective.Controls.TextBox Value = sender as Prospective.Controls.TextBox;
			Value.Background = Brushes.White;
		}

		private void Value_LostFocus(object sender, RoutedEventArgs e)
		{
			Prospective.Controls.TextBox Value = sender as Prospective.Controls.TextBox;
			Value.Background = Brushes.Transparent;
		}

		private void Value_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			Prospective.Controls.TextBox Value = sender as Prospective.Controls.TextBox;
			e.IsValid = true;

			if (Data.DataType == Projects.EnumDataType.Int)
				try { Convert.ToInt32(Value.Text); }
				catch { e.IsValid = false; }
			else if (Data.DataType == Projects.EnumDataType.SByte)
				try { Convert.ToSByte(Value.Text); }
				catch { e.IsValid = false; }
			else if (Data.DataType == Projects.EnumDataType.Byte)
				try { Convert.ToByte(Value.Text); }
				catch { e.IsValid = false; }
			else if (Data.DataType == Projects.EnumDataType.UShort)
				try { Convert.ToUInt16(Value.Text); }
				catch { e.IsValid = false; }
			else if (Data.DataType == Projects.EnumDataType.Short)
				try { Convert.ToInt16(Value.Text); }
				catch { e.IsValid = false; }
			else if (Data.DataType == Projects.EnumDataType.UInt)
				try { Convert.ToUInt32(Value.Text); }
				catch { e.IsValid = false; }
			else if (Data.DataType == Projects.EnumDataType.Long)
				try { Convert.ToInt64(Value.Text); }
				catch { e.IsValid = false; }
			else if (Data.DataType == Projects.EnumDataType.ULong)
				try { Convert.ToUInt64(Value.Text); }
				catch { e.IsValid = false; }
		}

		private void ContractValue_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (ValuesList.SelectedIndex == 0) ValuesList.SelectedIndex = Data.Elements.Count - 1; else ValuesList.SelectedIndex--;
			if (e.Key == Key.Down) if (ValuesList.SelectedIndex == Data.Elements.Count - 1) ValuesList.SelectedIndex = 0; else ValuesList.SelectedIndex++;

			if (e.Key == Key.Up || e.Key == Key.Down)
			{
				ListBoxItem lbi = ValuesList.ItemContainerGenerator.ContainerFromIndex(ValuesList.SelectedIndex) as ListBoxItem;
				ContentPresenter cp = Globals.GetVisualChild<ContentPresenter>(lbi);
				Prospective.Controls.TextBox ts = ValuesList.ItemTemplate.FindName("ContractValue", cp) as Prospective.Controls.TextBox;
				if (ts == null) return;
				ts.Focus();
			}
		}

		private void ContractValue_GotFocus(object sender, RoutedEventArgs e)
		{
			Prospective.Controls.TextBox ContractValue = sender as Prospective.Controls.TextBox;
			ContractValue.Background = Brushes.White;
		}

		private void ContractValue_LostFocus(object sender, RoutedEventArgs e)
		{
			Prospective.Controls.TextBox ContractValue = sender as Prospective.Controls.TextBox;
			ContractValue.Background = Brushes.Transparent;
		}

		private void ContractValue_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			Prospective.Controls.TextBox ContractValue = sender as Prospective.Controls.TextBox;
			e.IsValid = true;

			if (Data.DataType == Projects.EnumDataType.Int)
				try { Convert.ToInt32(ContractValue.Text); }
				catch { e.IsValid = false; }
			else if (Data.DataType == Projects.EnumDataType.SByte)
				try { Convert.ToSByte(ContractValue.Text); }
				catch { e.IsValid = false; }
			else if (Data.DataType == Projects.EnumDataType.Byte)
				try { Convert.ToByte(ContractValue.Text); }
				catch { e.IsValid = false; }
			else if (Data.DataType == Projects.EnumDataType.UShort)
				try { Convert.ToUInt16(ContractValue.Text); }
				catch { e.IsValid = false; }
			else if (Data.DataType == Projects.EnumDataType.Short)
				try { Convert.ToInt16(ContractValue.Text); }
				catch { e.IsValid = false; }
			else if (Data.DataType == Projects.EnumDataType.UInt)
				try { Convert.ToUInt32(ContractValue.Text); }
				catch { e.IsValid = false; }
			else if (Data.DataType == Projects.EnumDataType.Long)
				try { Convert.ToInt64(ContractValue.Text); }
				catch { e.IsValid = false; }
			else if (Data.DataType == Projects.EnumDataType.ULong)
				try { Convert.ToUInt64(ContractValue.Text); }
				catch { e.IsValid = false; }
		}

		private void DeleteValue_Click(object sender, RoutedEventArgs e)
		{
			Projects.EnumElement OP = ValuesList.SelectedItem as Projects.EnumElement;
			if (OP == null) return;

			if (Prospective.Controls.MessageBox.Show("Are you sure you wish to delete the '" + OP.Name + "' enumeration value?", "Delete Enumeration Value?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
			Data.Elements.Remove(OP);
		}

		private void FlagName_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (FlagsList.SelectedIndex == 0) FlagsList.SelectedIndex = Data.Elements.Count - 1; else FlagsList.SelectedIndex--;
			if (e.Key == Key.Down) if (FlagsList.SelectedIndex == Data.Elements.Count - 1) FlagsList.SelectedIndex = 0; else FlagsList.SelectedIndex++;

			if (e.Key == Key.Up || e.Key == Key.Down)
			{
				ListBoxItem lbi = FlagsList.ItemContainerGenerator.ContainerFromIndex(FlagsList.SelectedIndex) as ListBoxItem;
				ContentPresenter cp = Globals.GetVisualChild<ContentPresenter>(lbi);
				Prospective.Controls.TextBox ts = FlagsList.ItemTemplate.FindName("FlagName", cp) as Prospective.Controls.TextBox;
				if (ts == null) return;
				ts.Focus();
			}
		}

		private void FlagName_GotFocus(object sender, RoutedEventArgs e)
		{
			Prospective.Controls.TextBox FlagName = sender as Prospective.Controls.TextBox;
			FlagName.Background = Brushes.White;
		}

		private void FlagName_LostFocus(object sender, RoutedEventArgs e)
		{
			Prospective.Controls.TextBox FlagName = sender as Prospective.Controls.TextBox;
			FlagName.Background = Brushes.Transparent;
		}

		private void FlagValue_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (FlagsList.SelectedIndex == 0) FlagsList.SelectedIndex = Data.Elements.Count - 1; else FlagsList.SelectedIndex--;
			if (e.Key == Key.Down) if (FlagsList.SelectedIndex == Data.Elements.Count - 1) FlagsList.SelectedIndex = 0; else FlagsList.SelectedIndex++;

			if (e.Key == Key.Up || e.Key == Key.Down)
			{
				ListBoxItem lbi = FlagsList.ItemContainerGenerator.ContainerFromIndex(FlagsList.SelectedIndex) as ListBoxItem;
				ContentPresenter cp = Globals.GetVisualChild<ContentPresenter>(lbi);
				Prospective.Controls.TextBox ts = FlagsList.ItemTemplate.FindName("ContractValue", cp) as Prospective.Controls.TextBox;
				if (ts == null) return;
				ts.Focus();
			}
		}

		private void FlagValue_GotFocus(object sender, RoutedEventArgs e)
		{
			Prospective.Controls.TextBox FlagValue = sender as Prospective.Controls.TextBox;
			FlagValue.Background = Brushes.White;
		}

		private void FlagValue_LostFocus(object sender, RoutedEventArgs e)
		{
			Prospective.Controls.TextBox FlagValue = sender as Prospective.Controls.TextBox;
			FlagValue.Background = Brushes.Transparent;
		}

		private void DeleteFlag_Click(object sender, RoutedEventArgs e)
		{
			Projects.EnumElement OP = FlagsList.SelectedItem as Projects.EnumElement;
			if (OP == null) return;

			if (Prospective.Controls.MessageBox.Show("Are you sure you wish to delete the '" + OP.Name + "' enumeration flag?", "Delete Enumeration Flag?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
			Data.Elements.Remove(OP);
		}

	}

	[ValueConversion(typeof(Projects.EnumDataType), typeof(int))]
	public class EnumDataTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Projects.EnumDataType lt = (Projects.EnumDataType)value;
			if (lt == Projects.EnumDataType.Int) return 0;
			if (lt == Projects.EnumDataType.SByte) return 1;
			if (lt == Projects.EnumDataType.Byte) return 2;
			if (lt == Projects.EnumDataType.Short) return 3;
			if (lt == Projects.EnumDataType.UShort) return 4;
			if (lt == Projects.EnumDataType.UInt) return 5;
			if (lt == Projects.EnumDataType.Long) return 6;
			if (lt == Projects.EnumDataType.ULong) return 7;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return Projects.EnumDataType.Int;
			if (lt == 1) return Projects.EnumDataType.SByte;
			if (lt == 2) return Projects.EnumDataType.Byte;
			if (lt == 3) return Projects.EnumDataType.Short;
			if (lt == 4) return Projects.EnumDataType.UShort;
			if (lt == 5) return Projects.EnumDataType.UInt;
			if (lt == 6) return Projects.EnumDataType.Long;
			if (lt == 7) return Projects.EnumDataType.ULong;
			return Projects.EnumDataType.Int;
		}
	}

	[ValueConversion(typeof(Projects.EnumFlagsDataType), typeof(int))]
	public class EnumFlagsDataTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Projects.EnumDataType lt = (Projects.EnumDataType)value;
			if (lt == Projects.EnumDataType.ULong) return 0;
			if (lt == Projects.EnumDataType.Long) return 1;
			if (lt == Projects.EnumDataType.UInt) return 2;
			if (lt == Projects.EnumDataType.Int) return 3;
			if (lt == Projects.EnumDataType.UShort) return 4;
			if (lt == Projects.EnumDataType.Short) return 5;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return Projects.EnumDataType.ULong;
			if (lt == 1) return Projects.EnumDataType.Long;
			if (lt == 2) return Projects.EnumDataType.UInt;
			if (lt == 3) return Projects.EnumDataType.Int;
			if (lt == 4) return Projects.EnumDataType.UShort;
			if (lt == 5) return Projects.EnumDataType.Short;
			return Projects.EnumDataType.ULong;
		}
	}
}