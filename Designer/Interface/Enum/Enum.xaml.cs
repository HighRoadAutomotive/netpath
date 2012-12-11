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
using Prospective.Controls;
using Prospective.Controls.Dialogs;
using NETPath.Projects.Helpers;
using Button = System.Windows.Controls.Button;

namespace NETPath.Interface.Enum
{
	internal partial class Enum : Grid
	{
		public Projects.Enum OpenType { get { return (Projects.Enum)GetValue(OpenTypeProperty); } set { SetValue(OpenTypeProperty, value); } }
		public static readonly DependencyProperty OpenTypeProperty = DependencyProperty.Register("OpenType", typeof(Projects.Enum), typeof(Enum));

		public Enum(Projects.Enum Data)
		{
			OpenType = Data;

			InitializeComponent();
		}

		#region - Value Drag/Drop Support -

		private int DragItemStartIndex;
		private int DragItemNewIndex;
		private Projects.EnumElement DragElement;
		private Point DragStartPos;
		private Themes.DragAdorner DragAdorner;
		private AdornerLayer DragLayer;
		private bool IsDragging { get; set; }
		private bool DragHasLeftScope { get; set; }

		private void ValuesList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragStartPos = e.GetPosition(null);
		}

		private void ValuesList_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			ContentPresenter cp = GetValuesListBoxItemPresenter();
			if (cp == null) return;
			var ts = ValuesList.ItemTemplate.FindName("ValueName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("Value", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("ContractValue", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;

			if (e.LeftButton != MouseButtonState.Pressed || IsDragging) return;
			Point position = e.GetPosition(null);

			if (Math.Abs(position.X - DragStartPos.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - DragStartPos.Y) > SystemParameters.MinimumVerticalDragDistance)
			{
				StartValueDrag(e);
			}
		}

		private void StartValueDrag(MouseEventArgs e)
		{
			//Here we create our adorner.. 
			DragElement = ValuesList.SelectedItem as Projects.EnumElement;
			DragItemStartIndex = ValuesList.SelectedIndex;
			var tuie = (UIElement)ValuesList.ItemContainerGenerator.ContainerFromItem(ValuesList.SelectedItem);
			if (tuie == null) return;
			try { DragAdorner = new Themes.DragAdorner(ValuesList, tuie, true, 0.5); }
			catch { return; }
			DragLayer = AdornerLayer.GetAdornerLayer(ValuesList);
			DragLayer.Add(DragAdorner);

			IsDragging = true;
			DragHasLeftScope = false;

			var data = new DataObject(DataFormats.Text, "");
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
			if (!Equals(e.OriginalSource, ValuesList)) return;
			Point p = e.GetPosition(ValuesList);
			Rect r = VisualTreeHelper.GetContentBounds(ValuesList);
			if (r.Contains(p)) return;
			DragHasLeftScope = true;
			e.Handled = true;

			foreach (Projects.EnumElement O in OpenType.Elements)
			{
				var lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(O);
				if (lbi == null) continue;
				lbi.Margin = new Thickness(0);
			}
		}

		private void ValuesList_PreviewDragOver(object sender, DragEventArgs e)
		{
			ContentPresenter cp = GetValuesListBoxItemPresenter();
			if (cp == null) return;
			var ts = ValuesList.ItemTemplate.FindName("ValueName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("Value", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("ContractValue", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;

			if (DragAdorner != null)
			{
				DragAdorner.LeftOffset = e.GetPosition(ValuesList).X;
				DragAdorner.TopOffset = e.GetPosition(ValuesList).Y;
			}

			Point MP = e.GetPosition(ValuesList);
			HitTestResult htr = VisualTreeHelper.HitTest(ValuesList, MP);
			if (htr == null) return;
			foreach (Projects.EnumElement O in OpenType.Elements)
			{
				var lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(O);
				var lbiht = Globals.GetVisualParent<ListBoxItem>(htr.VisualHit);
				if (lbi == null) continue;
				if (lbiht == null) continue;
				if (!Equals(lbi, lbiht))
				{
					lbi.Margin = new Thickness(0);
				}
				else
				{
					if (DragAdorner != null) lbi.Margin = new Thickness(0, DragAdorner.ActualHeight, 0, 0);
					DragItemNewIndex = ValuesList.ItemContainerGenerator.IndexFromContainer(lbi);
				}
			}
		}

		private void ValuesList_Drop(object sender, DragEventArgs e)
		{
			OpenType.Elements.Move(DragItemStartIndex, DragItemNewIndex);

			foreach (Projects.EnumElement O in OpenType.Elements)
			{
				var lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(O);
				if (lbi == null) continue;
				lbi.Margin = new Thickness(0);
			}
		}

		#endregion

		private ContentPresenter GetValuesListBoxItemPresenter()
		{
			if (ValuesList.SelectedItem == null) return null;
			var lbi = ValuesList.ItemContainerGenerator.ContainerFromIndex(ValuesList.SelectedIndex) as ListBoxItem;
			return Globals.GetVisualChild<ContentPresenter>(lbi);
		}

		private void AddValueName_Validate(object sender, ValidateEventArgs e)
		{
			e.IsValid = false;
			if (string.IsNullOrEmpty(AddValueName.Text)) return;
			AddValue.IsEnabled = e.IsValid = RegExs.MatchCodeName.IsMatch(AddValueName.Text);
		}

		private void AddValueName_KeyUp(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter)
				AddValue_Click(null, null);
		}

		private void AddValue_Click(object sender, RoutedEventArgs e)
		{
			if (AddValue.IsEnabled == false) return;
			OpenType.Elements.Add(new Projects.EnumElement(OpenType, AddValueName.Text));
			AddValueName.Text = "";
			AddValueName.Focus();
			AddValue.IsEnabled = false;
		}

		private void ServerValue_Validate(object sender, ValidateEventArgs e)
		{
			var ServerValue = sender as Prospective.Controls.TextBox;
			e.IsValid = true;
			if (ServerValue == null) return;
			if (string.IsNullOrEmpty(ServerValue.Text)) return;

			if (OpenType.IsFlags)
				try { Convert.ToUInt64(ServerValue.Text); }
				catch { e.IsValid = false; }
			else
				try { Convert.ToInt64(ServerValue.Text); }
				catch { e.IsValid = false; }
		}

		private void ClientValue_Validate(object sender, ValidateEventArgs e)
		{
			var ClientValue = sender as Prospective.Controls.TextBox;
			e.IsValid = true;
			if (ClientValue == null) return;
			if (string.IsNullOrEmpty(ClientValue.Text)) return;

			if (OpenType.IsFlags)
				try { Convert.ToUInt64(ClientValue.Text); }
				catch { e.IsValid = false; }
			else
				try { Convert.ToInt64(ClientValue.Text); }
				catch { e.IsValid = false; }
		}

		private void DeleteValue_Click(object sender, RoutedEventArgs e)
		{
			var lbi = Globals.GetVisualParent<ListBoxItem>(sender);
			var OP = lbi.Content as Projects.EnumElement;
			if (OP == null) return;

			DialogService.ShowMessageDialog("NETPath", "Delete Enumeration Value?", "Are you sure you want to delete the '" + OP.Name + "' enumeration value?", new DialogAction("Yes", () => OpenType.Elements.Remove(OP), true), new DialogAction("No", false, true));
		}

		private void AddAggregateValue_Click(object sender, RoutedEventArgs e)
		{
			var lbi = Globals.GetVisualParent<ListBoxItem>(sender);
			var OP = lbi.Content as Projects.EnumElement;
			if (OP == null) return;
			var b = sender as Button;
			if (b == null) return;
			var v = b.Tag as Projects.EnumElement;
			if (v == null) return;

			OP.AggregateValues.Add(v);

			var AggSel = Globals.GetVisualChild<System.Windows.Controls.ComboBox>(lbi);
			if (AggSel == null) return;
			AggSel.SelectedIndex = -1;
			var bindingExpression = BindingOperations.GetMultiBindingExpression(AggSel, System.Windows.Controls.ComboBox.ItemsSourceProperty);
			if (bindingExpression != null) bindingExpression.UpdateTarget();
		}

		private void DeleteSelectedAggregateValue_Click(object sender, RoutedEventArgs e)
		{
			var lbi = Globals.GetVisualParent<ListBoxItem>(sender);
			var OP = lbi.Content as Projects.EnumElement;
			if (OP == null) return;
			var hlbi = Globals.GetVisualParent<ListBoxItem>(Globals.GetVisualParent<ListBox>(lbi));
			var hop = hlbi.Content as Projects.EnumElement;
			if (hop == null) return;

			DialogService.ShowMessageDialog("NETPath", "Delete Aggregate Enumeration Value?", "Are you sure you want to delete the '" + OP.Name + "' aggregate value from the enumeration value'" + hop.Name + "'?", new DialogAction("Yes", () =>
				                                                                                                                                                                                                                                       {
																																																														   hop.AggregateValues.Remove(OP); 
																																																														   var AggSel = Globals.GetVisualChild<System.Windows.Controls.ComboBox>(hlbi);
																																																														   if (AggSel == null) return;
																																																														   var bindingExpression = BindingOperations.GetMultiBindingExpression(AggSel, System.Windows.Controls.ComboBox.ItemsSourceProperty);
																																																														   if (bindingExpression != null) bindingExpression.UpdateTarget();
				                                                                                                                                                                                                                                       }, true), new DialogAction("No", false, true));
		}
	}

	public class AvailableAggregatesConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var ee = values[1] as Projects.EnumElement;
			if (ee == null) return new ObservableCollection<Projects.EnumElement>();
			var al = values[0] as ObservableCollection<Projects.EnumElement>;
			if (al == null) return new ObservableCollection<Projects.EnumElement>();

			return new ObservableCollection<Projects.EnumElement>(al.Where(e => !ee.AggregateValues.Contains(e) && !Equals(ee, e)));
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}