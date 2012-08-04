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

namespace WCFArchitect.Interface.Data
{
	internal partial class Data : Grid
	{
		public Projects.Data PData { get; set; }

		private int DragItemStartIndex;
		private int DragItemNewIndex;
		private Projects.DataElement DragElement;
		private Point DragStartPos;
		private Themes.DragAdorner DragAdorner;
		private AdornerLayer DragLayer;
		public bool IsDragging { get; set; }
		public bool DragHasLeftScope { get; set; }

		//Filters
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

		public bool FilterExpandAll { get { return (bool)GetValue(FilterExpandAllProperty); } set { SetValue(FilterExpandAllProperty, value); } }
		public static readonly DependencyProperty FilterExpandAllProperty = DependencyProperty.Register("FilterExpandAll", typeof(bool), typeof(Data));

		private bool IsValidArrayType(string DataType) { if (DataType == "" || DataType == null) { return true; } return DataType.Count(a => a == '[' || a == ']') == 2 && DataType.Count(a => a == '<' || a == '>') == 0 && DataType.Count(a => a == ',') == 0; }
		private bool IsValidCollectionType(string DataType) { if (DataType == "" || DataType == null) { return true; } return DataType.Count(a => a == '[' || a == ']') == 0 && DataType.Count(a => a == '<' || a == '>') == 2 && DataType.Count(a => a == ',') == 0; }
		private bool IsValidDictionaryType(string DataType) { if (DataType == "" || DataType == null) { return true; } return DataType.Count(a => a == '[' || a == ']') == 0 && DataType.Count(a => a == '<' || a == '>') == 2 && DataType.Count(a => a == ',') == 1; } 

		public Data(Projects.Data Data)
		{
			this.PData = Data;
			this.FilterScopePublic = true;
			this.FilterScopeProtected = true;
			this.FilterScopePrivate = true;
			this.FilterScopeInternal = true;
			this.FilterScopeProtectedInternal = true;
			this.FilterExpandAll = false;

			InitializeComponent();

			this.FilterDataTypeBox.Text = "";
			this.FilterNameBox.Text = "";
			this.AddDataElementDataType.Text = "string";

			PData.Elements.CollectionChanged +=new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Elements_CollectionChanged);
			ValuesList.ItemsSource = PData.Elements;

			AddDataElement.IsEnabled = false;
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
			Prospective.Controls.TextBox ts = ValuesList.ItemTemplate.FindName("DataType", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("ElementName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("Order", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("WPFDataType", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("WPFName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("AttachedTargetTypes", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("AttachedAttributeTypes", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
#if STANDARD
			if (PData.Parent.Owner.IsDependencyProject == true) return;
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
			DragElement = ValuesList.SelectedItem as Projects.DataElement;
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

					foreach (Projects.DataElement O in PData.Elements)
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
			ContentPresenter cp = GetListBoxItemPresenter();
			if (cp == null) return;
			Prospective.Controls.TextBox ts = ValuesList.ItemTemplate.FindName("DataType", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("ElementName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("Order", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("WPFDataType", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("WPFName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("AttachedTargetTypes", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin == true) return;
			ts = ValuesList.ItemTemplate.FindName("AttachedAttributeTypes", cp) as Prospective.Controls.TextBox;
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
				foreach (Projects.DataElement O in PData.Elements)
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
			PData.Elements.Move(DragItemStartIndex, DragItemNewIndex);

			foreach (Projects.DataElement O in PData.Elements)
			{
				ListBoxItem lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(O);
				if (lbi == null) continue;
				lbi.Margin = new Thickness(0);
			}
		}

		#endregion

		private ContentPresenter GetListBoxItemPresenter()
		{
			if (ValuesList.SelectedItem == null) return null;
			ListBoxItem lbi = ValuesList.ItemContainerGenerator.ContainerFromIndex(ValuesList.SelectedIndex) as ListBoxItem;
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

		private void WPFName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(WPFName.Text);
		}

		private void IsReference_Checked(object sender, RoutedEventArgs e)
		{
			IsReference.Content = "Yes";
		}

		private void IsReference_Unchecked(object sender, RoutedEventArgs e)
		{
			IsReference.Content = "No";
		}

		private void AddDataElementScope_SelectedIndexChanged(object sender, C1.WPF.PropertyChangedEventArgs<int> e)
		{
			if (IsLoaded == false) return;
			AddDataElementDataType.Focus();
		}

		private void AddDataElementDataType_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				AddDataElementName.Focus();
		}

		private void AddDataElementDataType_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (AddDataElementDataType.Text != "" && AddDataElementName.Text != "") AddDataElement.IsEnabled = true;
			else AddDataElement.IsEnabled = false;
		}

		private void AddDataElementDataType_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = true;
			if ((AddDataElementDataType.Text.IndexOf("[") >= 0 || AddDataElementDataType.Text.IndexOf("]") >= 0) && IsValidArrayType(AddDataElementDataType.Text) == false) e.IsValid = false;
			if ((AddDataElementDataType.Text.IndexOf("<") >= 0 || AddDataElementDataType.Text.IndexOf(">") >= 0) && AddDataElementDataType.Text.IndexOf(",") < 0 && IsValidCollectionType(AddDataElementDataType.Text) == false) e.IsValid = false;
			if ((AddDataElementDataType.Text.IndexOf("<") >= 0 || AddDataElementDataType.Text.IndexOf(">") >= 0) && AddDataElementDataType.Text.IndexOf(",") >= 0 && IsValidDictionaryType(AddDataElementDataType.Text) == false) e.IsValid = false;
			AddDataElement.IsEnabled = e.IsValid;
		}

		private void AddDataElementName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				AddDataElement_Click(null, null);
		}

		private void AddDataElementName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (AddDataElementName.IsInvalid == false)
			{
				AddDataElement.IsEnabled = false;
				return;
			}
			if (AddDataElementDataType.Text != "" && AddDataElementName.Text != "") AddDataElement.IsEnabled = true;
			else AddDataElement.IsEnabled = false;
		}

		private void AddDataElementName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = true;
			if (AddDataElementName.Text == "" || AddDataElementName == null) return;

			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(AddDataElementName.Text);
			AddDataElement.IsEnabled = e.IsValid;
		}

		private void AddDataElement_Click(object sender, RoutedEventArgs e)
		{
			if (AddDataElement.IsEnabled == false) return;
			PData.Elements.Add(new Projects.DataElement((Projects.DataScope)AddDataElementScope.SelectedIndex, AddDataElementDataType.Text, AddDataElementName.Text, PData));
			AddDataElementDataType.Focus();
			AddDataElementDataType.Text = "string";
			AddDataElementDataType.SelectAll();
			AddDataElementName.Text = "";
		}

		private void FilterBarScope_Click(object sender, RoutedEventArgs e)
		{
			FilterBarScope.ContextMenu.Visibility = System.Windows.Visibility.Visible;
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
			FilteredScopes.Visibility = System.Windows.Visibility.Collapsed;
			FilteredScopes.Text = "Filtering:";
			if (FilterScopePublic == false) { FilteredScopes.Text += " Public,"; FilteredScopes.Visibility = System.Windows.Visibility.Visible; }
			if (FilterScopeProtected == false) { FilteredScopes.Text += " Protected,"; FilteredScopes.Visibility = System.Windows.Visibility.Visible; }
			if (FilterScopePrivate == false) { FilteredScopes.Text += " Private,"; FilteredScopes.Visibility = System.Windows.Visibility.Visible; }
			if (FilterScopeInternal == false) { FilteredScopes.Text += " Internal,"; FilteredScopes.Visibility = System.Windows.Visibility.Visible; }
			if (FilterScopeProtectedInternal == false) { FilteredScopes.Text += " Protected Internal,"; FilteredScopes.Visibility = System.Windows.Visibility.Visible; }
			FilteredScopes.Text = FilteredScopes.Text.Remove(FilteredScopes.Text.Length - 1);

			foreach (Projects.DataElement DE in PData.Elements)
			{
				ListBoxItem lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(DE);
				if (lbi == null) continue;
				
				lbi.Visibility = System.Windows.Visibility.Collapsed;
				if (DE.Scope == Projects.DataScope.Public && FilterScopePublic == false) continue;
				if (DE.Scope == Projects.DataScope.Protected && FilterScopeProtected == false) continue;
				if (DE.Scope == Projects.DataScope.Private && FilterScopePrivate == false) continue;
				if (DE.Scope == Projects.DataScope.Internal && FilterScopeInternal == false) continue;
				if (DE.Scope == Projects.DataScope.ProtectedInternal && FilterScopeProtectedInternal == false) continue;
				if (DE.DataType.IndexOf(FilterDataTypeBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				if (DE.Name.IndexOf(FilterNameBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				lbi.Visibility = System.Windows.Visibility.Visible;
			}
		}

		private void Elements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			PData.IsDirty = true;
		}

		private void DataType_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (ValuesList.SelectedIndex == 0) ValuesList.SelectedIndex = PData.Elements.Count - 1; else ValuesList.SelectedIndex--;
			if (e.Key == Key.Down) if (ValuesList.SelectedIndex == PData.Elements.Count - 1) ValuesList.SelectedIndex = 0; else ValuesList.SelectedIndex++;

			if (e.Key == Key.Up || e.Key == Key.Down)
			{
				ListBoxItem lbi = ValuesList.ItemContainerGenerator.ContainerFromIndex(ValuesList.SelectedIndex) as ListBoxItem;
				ContentPresenter cp = Globals.GetVisualChild<ContentPresenter>(lbi);
				Prospective.Controls.TextBox ts = ValuesList.ItemTemplate.FindName("DataType", cp) as Prospective.Controls.TextBox;
				if (ts == null) return;
				ts.Focus();
			}
		}

		private void DataType_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			Prospective.Controls.TextBox DataType = sender as Prospective.Controls.TextBox;
			Projects.DataElement DE = ValuesList.SelectedItem as Projects.DataElement;
			if (DE == null) return;

			e.IsValid = true;
			if ((DataType.Text.IndexOf("[") >= 0 || DataType.Text.IndexOf("]") >= 0) && DE.IsValidArrayType(DataType.Text) == false) e.IsValid = false;
			if ((DataType.Text.IndexOf("<") >= 0 || DataType.Text.IndexOf(">") >= 0) && DataType.Text.IndexOf(",") < 0 && DE.IsValidCollectionType(DataType.Text) == false) e.IsValid = false;
			if ((DataType.Text.IndexOf("<") >= 0 || DataType.Text.IndexOf(">") >= 0) && DataType.Text.IndexOf(",") >= 0 && DE.IsValidDictionaryType(DataType.Text) == false) e.IsValid = false;
		}

		private void ElementName_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (ValuesList.SelectedIndex == 0) ValuesList.SelectedIndex = PData.Elements.Count - 1; else ValuesList.SelectedIndex--;
			if (e.Key == Key.Down) if (ValuesList.SelectedIndex == PData.Elements.Count - 1) ValuesList.SelectedIndex = 0; else ValuesList.SelectedIndex++;

			if (e.Key == Key.Up || e.Key == Key.Down)
			{
				Prospective.Controls.TextBox ts = ValuesList.ItemTemplate.FindName("ElementName", GetListBoxItemPresenter()) as Prospective.Controls.TextBox;
				if (ts == null) return;
				ts.Focus();
			}
		}

		private void ElementName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			Prospective.Controls.TextBox ElementName = sender as Prospective.Controls.TextBox;
			string t = Helpers.RegExs.ReplaceSpaces.Replace(ElementName.Text == null ? "" : ElementName.Text, @"");

			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(t);
		}

		private void Order_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			Prospective.Controls.TextBox Order = sender as Prospective.Controls.TextBox;
			if (Order.Text == "" || Order.Text == null) return;

			try { Convert.ToInt32(Order.Text); }
			catch { e.IsValid = false; return; }
			e.IsValid = true;
		}

		private void WPFDataType_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			Projects.DataElement DE = ValuesList.SelectedItem as Projects.DataElement;
			if (DE == null) return;

			Prospective.Controls.TextBox WPFDataType = sender as Prospective.Controls.TextBox;
			if (WPFDataType.Text == "" || WPFDataType.Text == null)
			{
				e.IsValid = true;
				return;
			}

			if (DE.IsValidMemberType(WPFDataType.Text) == true)
			{
				e.IsValid = true;
				DE.IsArray = false;
				DE.IsCollection = false;
				DE.IsDictionary = false;
			}
			else if (DE.IsValidArrayType(WPFDataType.Text) == true)
			{
				e.IsValid = true;
				DE.IsArray = true;
				DE.IsCollection = false;
				DE.IsDictionary = false;
			}
			else if (DE.IsValidCollectionType(WPFDataType.Text) == true)
			{
				e.IsValid = true;
				DE.IsArray = false;
				DE.IsCollection = true;
				DE.IsDictionary = false;
			}
			else if (DE.IsValidDictionaryType(WPFDataType.Text) == true)
			{
				e.IsValid = true;
				DE.IsArray = false;
				DE.IsCollection = false;
				DE.IsDictionary = true;
			}
			else
			{
				e.IsValid = false;
				DE.IsArray = false;
				DE.IsCollection = false;
				DE.IsDictionary = false;
			}
		}

		private void DeleteElement_Click(object sender, RoutedEventArgs e)
		{
			Projects.DataElement OP = ValuesList.SelectedItem as Projects.DataElement;
			if (OP == null) return;

			if (Prospective.Controls.MessageBox.Show("Are you sure you wish to delete the '" + OP.DataType + " " + OP.Name + "' data member?", "Delete Data Member?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
			PData.Elements.Remove(OP);
		}
	}

	[ValueConversion(typeof(Projects.DataScope), typeof(int))]
	public class DataScopeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Projects.DataScope lt = (Projects.DataScope)value;
			if (lt == Projects.DataScope.Public) return 0;
			if (lt == Projects.DataScope.Protected) return 1;
			if (lt == Projects.DataScope.Private) return 2;
			if (lt == Projects.DataScope.Internal) return 3;
			if (lt == Projects.DataScope.ProtectedInternal) return 4;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
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
			Projects.DataScope lt = (Projects.DataScope)value;
			if (lt == Projects.DataScope.Public) return "Public";
			if (lt == Projects.DataScope.Protected) return "Protected";
			if (lt == Projects.DataScope.Private) return "Private";
			if (lt == Projects.DataScope.Internal) return "Internal";
			if (lt == Projects.DataScope.ProtectedInternal) return "Protected Internal";
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string lt = (string)value;
			if (lt == "Public") return Projects.DataScope.Public;
			if (lt == "Protected") return Projects.DataScope.Protected;
			if (lt == "Private") return Projects.DataScope.Private;
			if (lt == "Internal") return Projects.DataScope.Internal;
			if (lt == "Protected Internal") return Projects.DataScope.ProtectedInternal;
			return "Public";
		}
	}
}