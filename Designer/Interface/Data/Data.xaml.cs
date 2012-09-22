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
using System.Globalization;

namespace WCFArchitect.Interface.Data
{
	internal partial class Data : ContentControl
	{
		public Projects.Data OpenType { get { return (Projects.Data)GetValue(OpenTypeProperty); } set { SetValue(OpenTypeProperty, value); } }
		public static readonly DependencyProperty OpenTypeProperty = DependencyProperty.Register("OpenType", typeof(Projects.Data), typeof(Data));

		public bool FilterScopePublic { get { return (bool)GetValue(FilterScopePublicProperty); } set { SetValue(FilterScopePublicProperty, value); } }
		public static readonly DependencyProperty FilterScopePublicProperty = DependencyProperty.Register("FilterScopePublic", typeof(bool), typeof(Data));

		public bool FilterScopeProtected { get { return (bool)GetValue(FilterScopeProtectedProperty); } set { SetValue(FilterScopeProtectedProperty, value); } }
		public static readonly DependencyProperty FilterScopeProtectedProperty = DependencyProperty.Register("FilterScopeProtected", typeof(bool), typeof(Data));

		public bool FilterScopePrivate { get { return (bool)GetValue(FilterScopePrivateProperty); } set { SetValue(FilterScopePrivateProperty, value); } }
		public static readonly DependencyProperty FilterScopePrivateProperty = DependencyProperty.Register("FilterScopePrivate", typeof(bool), typeof(Data));

		public bool FilterScopeInternal { get { return (bool)GetValue(FilterScopeInternalProperty); } set { SetValue(FilterScopeInternalProperty, value); } }
		public static readonly DependencyProperty FilterScopeInternalProperty = DependencyProperty.Register("FilterScopeInternal", typeof(bool), typeof(Data));

		public bool FilterScopeProtectedInternal { get { return (bool)GetValue(FilterScopeProtectedInternalProperty); } set { SetValue(FilterScopeProtectedInternalProperty, value); } }
		public static readonly DependencyProperty FilterScopeProtectedInternalProperty = DependencyProperty.Register("FilterScopeProtectedInternal", typeof(bool), typeof(Data));

		public Data() { }

		public Data(Projects.Data Data)
		{
			OpenType = Data;

			InitializeComponent();
		}

		private void AddMemberType_ValidationChanged(object Sender, RoutedEventArgs E)
		{
			AddMember.IsEnabled = (!string.IsNullOrEmpty(AddMemberName.Text) && !AddMemberName.IsInvalid && AddMemberType.IsValid);
		}

		private void AddMemberType_Selected(object sender, RoutedEventArgs e)
		{
			AddMemberName.Focus();
		}

		private void AddMemberName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = true;
			if (string.IsNullOrEmpty(AddMemberName.Text)) return;

			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(AddMemberName.Text);
			AddMember.IsEnabled = (!string.IsNullOrEmpty(AddMemberName.Text) &&  e.IsValid && AddMemberType.IsValid);
		}

		private void AddMemberName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				if (AddMemberType.OpenType == null || AddMemberName.Text == "")
					AddMember_Click(sender, null);
		}

		private void AddMemberName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (AddMemberName.IsInvalid == false)
			{
				AddMember.IsEnabled = false;
				return;
			}
			if (AddMemberType.OpenType != null && AddMemberName.Text != "") AddMember.IsEnabled = true;
			else AddMember.IsEnabled = false;
		}

		private void AddMember_Click(object sender, RoutedEventArgs e)
		{
			if (AddMember.IsEnabled == false) return;
			OpenType.Elements.Add(new Projects.DataElement(Projects.DataScope.Public, AddMemberType.OpenType, AddMemberName.Text, OpenType));
			AddMemberType.Focus();
			AddMemberType.OpenType = null;
			AddMemberName.Text = "";
		}

		private void ElementName_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (ValuesList.SelectedIndex == 0) ValuesList.SelectedIndex = OpenType.Elements.Count - 1; else ValuesList.SelectedIndex--;
			if (e.Key == Key.Down) if (ValuesList.SelectedIndex == OpenType.Elements.Count - 1) ValuesList.SelectedIndex = 0; else ValuesList.SelectedIndex++;

			if (e.Key != Key.Up && e.Key != Key.Down) return;
			var ts = ValuesList.ItemTemplate.FindName("ElementName", GetListBoxItemPresenter()) as Prospective.Controls.TextBox;
			if (ts == null) return;
			ts.Focus();
		}

		private void ElementName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			var elementName = sender as Prospective.Controls.TextBox;
			if (elementName == null) return;
			string t = Helpers.RegExs.ReplaceSpaces.Replace(elementName.Text ?? "", @"");

			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(t);
		}

		private void Order_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			var order = sender as Prospective.Controls.TextBox;
			if (order == null) return;
			if (string.IsNullOrEmpty(order.Text)) return;

			try { Convert.ToInt32(order.Text); }
			catch { e.IsValid = false; return; }
			e.IsValid = true;
		}

		private void DeleteElement_Click(object sender, RoutedEventArgs e)
		{
			var op = ValuesList.SelectedItem as Projects.DataElement;
			if (op == null) return;

			if (Prospective.Controls.MessageBox.Show("Are you sure you wish to delete the '" + op.DataType + " " + op.DataName + "' data member?", "Delete Data Member?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
			OpenType.Elements.Remove(op);
		}

		#region - Drag/Drop Support -

		private int dragItemStartIndex;
		private int dragItemNewIndex;
		private Projects.DataElement dragElement;
		private Point dragStartPos;
		private Themes.DragAdorner dragAdorner;
		private AdornerLayer dragLayer;
		public bool IsDragging { get; set; }
		public bool DragHasLeftScope { get; set; }

		private void ValuesList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			dragStartPos = e.GetPosition(null);
		}

		private void ValuesList_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			ContentPresenter cp = GetListBoxItemPresenter();
			if (cp == null) return;
			var ts = ValuesList.ItemTemplate.FindName("DataType", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("ElementName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("Order", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("WPFDataType", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("WPFName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("AttachedTargetTypes", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("AttachedAttributeTypes", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;

			if (e.LeftButton != MouseButtonState.Pressed || IsDragging) return;
			Point position = e.GetPosition(null);

			if (Math.Abs(position.X - dragStartPos.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - dragStartPos.Y) > SystemParameters.MinimumVerticalDragDistance)
				StartValueDrag(e);
		}

		private void StartValueDrag(MouseEventArgs e)
		{
			//Here we create our adorner.. 
			dragElement = ValuesList.SelectedItem as Projects.DataElement;
			dragItemStartIndex = ValuesList.SelectedIndex;
			var tuie = (UIElement)ValuesList.ItemContainerGenerator.ContainerFromItem(ValuesList.SelectedItem);
			if (tuie == null) return;
			try { dragAdorner = new Themes.DragAdorner(ValuesList, tuie, true, 0.5); }
			catch { return; }
			dragLayer = AdornerLayer.GetAdornerLayer(ValuesList as Visual);
			dragLayer.Add(dragAdorner);

			IsDragging = true;
			DragHasLeftScope = false;

			var data = new DataObject(DataFormats.Text.ToString(), "");
			DragDropEffects de = DragDrop.DoDragDrop(ValuesList, data, DragDropEffects.Move);

			// Clean up our mess
			AdornerLayer.GetAdornerLayer(ValuesList).Remove(dragAdorner);
			dragAdorner = null;

			IsDragging = false;
		}

		private void ValuesList_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			if (!DragHasLeftScope) return;
			e.Action = DragAction.Cancel;
			e.Handled = true;
		}

		private void ValuesList_DragLeave(object sender, DragEventArgs e)
		{
			if (e.OriginalSource != ValuesList) return;
			Point p = e.GetPosition(ValuesList);
			Rect r = VisualTreeHelper.GetContentBounds(ValuesList);

			if (r.Contains(p)) return;
			DragHasLeftScope = true;
			e.Handled = true;

			foreach (var lbi in OpenType.Elements.Select(o => (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(o)).Where(lbi => lbi != null))
				lbi.Margin = new Thickness(0);
		}

		private void ValuesList_PreviewDragOver(object sender, DragEventArgs e)
		{
			ContentPresenter cp = GetListBoxItemPresenter();
			if (cp == null) return;
			var ts = ValuesList.ItemTemplate.FindName("DataType", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("ElementName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("Order", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("WPFDataType", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("WPFName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("AttachedTargetTypes", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("AttachedAttributeTypes", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;

			if (dragAdorner != null)
			{
				dragAdorner.LeftOffset = e.GetPosition(ValuesList).X;
				dragAdorner.TopOffset = e.GetPosition(ValuesList).Y;
			}

			Point mp = e.GetPosition(ValuesList);
			HitTestResult htr = VisualTreeHelper.HitTest(ValuesList, mp);
			if (htr == null) return;

			foreach (Projects.DataElement o in OpenType.Elements)
			{
				var lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(o);
				var lbiht = Globals.GetVisualParent<ListBoxItem>(htr.VisualHit);
				if (lbi == null) continue;
				if (lbiht == null) continue;
				if (lbi != lbiht)
					lbi.Margin = new Thickness(0);
				else
				{
					if (dragAdorner != null) lbi.Margin = new Thickness(0, dragAdorner.ActualHeight, 0, 0);
					dragItemNewIndex = ValuesList.ItemContainerGenerator.IndexFromContainer(lbi);
				}
			}
		}

		private void ValuesList_Drop(object sender, DragEventArgs e)
		{
			OpenType.Elements.Move(dragItemStartIndex, dragItemNewIndex);

			foreach (var lbi in OpenType.Elements.Select(o => (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(o)).Where(lbi => lbi != null))
				lbi.Margin = new Thickness(0);
		}

		private ContentPresenter GetListBoxItemPresenter()
		{
			if (ValuesList.SelectedItem == null) return null;
			var lbi = ValuesList.ItemContainerGenerator.ContainerFromIndex(ValuesList.SelectedIndex) as ListBoxItem;
			return Globals.GetVisualChild<ContentPresenter>(lbi);
		}

		#endregion

		#region - Filtering -

		private void FilterBarScope_Click(object sender, RoutedEventArgs e)
		{
			FilterBarScope.ContextMenu.Visibility = Visibility.Visible;
			FilterBarScope.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
			FilterBarScope.ContextMenu.PlacementTarget = FilterBarScope;
			FilterBarScope.ContextMenu.IsOpen = true;
		}

		private void FilterBarScopeMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ApplyFiter();
		}

		private void FilterBarDataType_Click(object sender, RoutedEventArgs e)
		{
			FilterBarDataType.Visibility = Visibility.Hidden;
			FilterDataTypeBox.Visibility = Visibility.Visible;
			FilterDataTypeBox.Focus();
		}

		private void FilterDataTypeBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ApplyFiter();
		}

		private void FilterDataTypeBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (FilterDataTypeBox.Text == "")
			{
				FilterBarDataType.Visibility = Visibility.Visible;
				FilterDataTypeBox.Visibility = Visibility.Hidden;
			}
		}

		private void FilterBarName_Click(object sender, RoutedEventArgs e)
		{
			FilterBarName.Visibility = Visibility.Hidden;
			FilterNameBox.Visibility = Visibility.Visible;
			FilterNameBox.Focus();
		}

		private void FilterNameBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ApplyFiter();
		}

		private void FilterNameBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (FilterNameBox.Text == "")
			{
				FilterBarName.Visibility = Visibility.Visible;
				FilterNameBox.Visibility = Visibility.Hidden;
			}
		}

		private void ApplyFiter()
		{
			FilteredScopes.Visibility = Visibility.Collapsed;
			FilteredScopes.Text = "Filtering:";
			if (FilterScopePublic == false) { FilteredScopes.Text += " Public,"; FilteredScopes.Visibility = Visibility.Visible; }
			if (FilterScopeProtected == false) { FilteredScopes.Text += " Protected,"; FilteredScopes.Visibility = Visibility.Visible; }
			if (FilterScopePrivate == false) { FilteredScopes.Text += " Private,"; FilteredScopes.Visibility = Visibility.Visible; }
			if (FilterScopeInternal == false) { FilteredScopes.Text += " Internal,"; FilteredScopes.Visibility = Visibility.Visible; }
			if (FilterScopeProtectedInternal == false) { FilteredScopes.Text += " Protected Internal,"; FilteredScopes.Visibility = Visibility.Visible; }
			FilteredScopes.Text = FilteredScopes.Text.Remove(FilteredScopes.Text.Length - 1);

			foreach (Projects.DataElement DE in OpenType.Elements)
			{
				ListBoxItem lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(DE);
				if (lbi == null) continue;

				lbi.Visibility = Visibility.Collapsed;
				if (DE.DataScope == Projects.DataScope.Public && FilterScopePublic == false) continue;
				if (DE.DataScope == Projects.DataScope.Protected && FilterScopeProtected == false) continue;
				if (DE.DataScope == Projects.DataScope.Private && FilterScopePrivate == false) continue;
				if (DE.DataScope == Projects.DataScope.Internal && FilterScopeInternal == false) continue;
				if (DE.DataScope == Projects.DataScope.ProtectedInternal && FilterScopeProtectedInternal == false) continue;
				if (DE.DataType.Name.IndexOf(FilterDataTypeBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				if (DE.DataName.IndexOf(FilterNameBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				lbi.Visibility = Visibility.Visible;
			}
		}

		#endregion
	}

	[ValueConversion(typeof(Projects.DataScope), typeof(int))]
	public class DataScopeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Projects.DataScope)value;
			if (lt == Projects.DataScope.Public) return 0;
			if (lt == Projects.DataScope.Protected) return 1;
			if (lt == Projects.DataScope.Private) return 2;
			if (lt == Projects.DataScope.Internal) return 3;
			if (lt == Projects.DataScope.ProtectedInternal) return 4;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return Projects.DataScope.Public;
			if (lt == 1) return Projects.DataScope.Protected;
			if (lt == 2) return Projects.DataScope.Private;
			if (lt == 3) return Projects.DataScope.Internal;
			if (lt == 4) return Projects.DataScope.ProtectedInternal;
			return Projects.DataScope.Public;
		}
	}

	[ValueConversion(typeof(Projects.DataScope), typeof(string))]
	public class DataScopeNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var param = System.Convert.ToBoolean(parameter);
			var lt = (Projects.DataScope)value;
			if (lt == Projects.DataScope.Public) return param ? "public" : "Public";
			if (lt == Projects.DataScope.Protected) return param ? "protected" : "Protected";
			if (lt == Projects.DataScope.Private) return param ? "private" : "Private";
			if (lt == Projects.DataScope.Internal) return param ? "internal" : "Internal";
			if (lt == Projects.DataScope.ProtectedInternal) return param ? "protected internal" : "Protected Internal";
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (string)value;
			if (lt == "Public" || lt == "public") return Projects.DataScope.Public;
			if (lt == "Protected" || lt == "protected") return Projects.DataScope.Protected;
			if (lt == "Private" || lt == "private") return Projects.DataScope.Private;
			if (lt == "Internal" || lt == "internal") return Projects.DataScope.Internal;
			if (lt == "Protected Internal" || lt == "protected internal") return Projects.DataScope.ProtectedInternal;
			return "Public";
		}
	}

	[ValueConversion(typeof(int), typeof(string))]
	public class ElementOrderConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var v = (int)value;
			return v < 0 ? "" : v.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var v = (string)value;
			if (string.IsNullOrEmpty(v)) return -1;
			try { return System.Convert.ToInt32(v); }
			catch { return -1; }
		}
	}
}