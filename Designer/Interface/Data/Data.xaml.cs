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
using Prospective.Controls.Dialogs;
using WCFArchitect.Projects.Helpers;

namespace WCFArchitect.Interface.Data
{
	internal partial class Data : ContentControl
	{
		public Projects.Data OpenType { get { return (Projects.Data)GetValue(OpenTypeProperty); } set { SetValue(OpenTypeProperty, value); } }
		public static readonly DependencyProperty OpenTypeProperty = DependencyProperty.Register("OpenType", typeof(Projects.Data), typeof(Data));

		public object ActiveElement { get { return GetValue(ActiveElementProperty); } set { SetValue(ActiveElementProperty, value); } }
		public static readonly DependencyProperty ActiveElementProperty = DependencyProperty.Register("ActiveElement", typeof(object), typeof(Data));

		public Data() { }

		public Data(Projects.Data Data)
		{
			OpenType = Data;

			InitializeComponent();

			Projects.DataElement t = OpenType.Elements.FirstOrDefault(a => a.IsSelected);
			if (t != null) ValuesList.SelectedItem = t;
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

			e.IsValid = RegExs.MatchCodeName.IsMatch(AddMemberName.Text);
			AddMember.IsEnabled = (!string.IsNullOrEmpty(AddMemberName.Text) &&  e.IsValid && AddMemberType.IsValid);
		}

		private void AddMemberName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				if (!string.IsNullOrEmpty(AddMemberName.Text))
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
			
			var t = new Projects.DataElement(Projects.DataScope.Public, AddMemberType.OpenType, AddMemberName.Text, OpenType);
			OpenType.Elements.Add(t);
			
			AddMemberType.Focus();
			AddMemberType.OpenType = null;
			AddMemberName.Text = "";

			ValuesList.SelectedItem = t;
		}

		private void ValuesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var t = ValuesList.SelectedItem as Projects.DataElement;
			if (t == null) return;

			//Set the new item as selected.
			foreach (Projects.DataElement de in OpenType.Elements)
				de.IsSelected = false;
			t.IsSelected = true;

			ActiveElement = new DataElement(t);
		}

		private void DeleteElement_Click(object sender, RoutedEventArgs e)
		{
			var op = ValuesList.SelectedItem as Projects.DataElement;
			if (op == null) return;

			DialogService.ShowMessageDialog("WCF ARCHITECT", "Delete Data Member?", "Are you sure you wish to delete the '" + op.DataType + " " + op.DataName + "' data member?", new DialogAction("Yes", () => { ActiveElement = null; OpenType.Elements.Remove(op); }, true), new DialogAction("No", false, true));
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
	}

	[ValueConversion(typeof(Projects.DataScope), typeof(int))]
	public class DataScopeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return -1;
			var lt = (Projects.DataScope)value;
			if (lt == Projects.DataScope.Public) return 0;
			if (lt == Projects.DataScope.Protected) return 1;
			if (lt == Projects.DataScope.Private) return 2;
			if (lt == Projects.DataScope.Internal) return 3;
			if (lt == Projects.DataScope.ProtectedInternal) return 4;
			if (lt == Projects.DataScope.Disabled) return -1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return null;
			var lt = (int)value;
			if (lt == -1) return Projects.DataScope.Disabled;
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
			if (value == null) return "";
			var lt = (Projects.DataScope)value;
			if (lt == Projects.DataScope.Disabled) return "";
			if (lt == Projects.DataScope.Public) return param ? "public" : "Public";
			if (lt == Projects.DataScope.Protected) return param ? "protected" : "Protected";
			if (lt == Projects.DataScope.Private) return param ? "private" : "Private";
			if (lt == Projects.DataScope.Internal) return param ? "internal" : "Internal";
			if (lt == Projects.DataScope.ProtectedInternal) return param ? "protected internal" : "Protected Internal";
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (string)value;
			if (lt == "Public" || lt == "public") return Projects.DataScope.Public;
			if (lt == "Protected" || lt == "protected") return Projects.DataScope.Protected;
			if (lt == "Private" || lt == "private") return Projects.DataScope.Private;
			if (lt == "Internal" || lt == "internal") return Projects.DataScope.Internal;
			if (lt == "Protected Internal" || lt == "protected internal") return Projects.DataScope.ProtectedInternal;
			return Projects.DataScope.Disabled;
		}
	}

	[ValueConversion(typeof(int), typeof(string))]
	public class ElementOrderConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var v = (int)value;
			return v < 0 ? "" : v.ToString(CultureInfo.InvariantCulture);
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