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
using EllipticBit.Controls.WPF.Dialogs;
using NETPath.Projects;
using NETPath.Projects.Helpers;
using NETPath.Projects.Wcf;

namespace NETPath.Interface.Wcf.Service
{
	internal partial class DataChangeMethod : Grid
	{
		public WcfDataChangeMethod MethodData { get { return (WcfDataChangeMethod)GetValue(MethodDataProperty); } set { SetValue(MethodDataProperty, value); } }
		public static readonly DependencyProperty MethodDataProperty = DependencyProperty.Register("MethodData", typeof(WcfDataChangeMethod), typeof(DataChangeMethod));

		public Projects.Project ServiceProject { get { return (Projects.Project)GetValue(ServiceProjectProperty); } set { SetValue(ServiceProjectProperty, value); } }
		public static readonly DependencyProperty ServiceProjectProperty = DependencyProperty.Register("ServiceProject", typeof(Projects.Project), typeof(DataChangeMethod));

		public ObservableCollection<WcfMethodParameter> Parameters { get { return (ObservableCollection<WcfMethodParameter>)GetValue(ParametersProperty); } set { SetValue(ParametersProperty, value); } }
		public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(ObservableCollection<WcfMethodParameter>), typeof(DataChangeMethod));

		public Projects.Documentation Documentation { get { return (Projects.Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Projects.Documentation), typeof(DataChangeMethod));

		public bool GetFunctionSelected { get { return (bool)GetValue(GetFunctionSelectedProperty); } set { SetValue(GetFunctionSelectedProperty, value); } }
		public static readonly DependencyProperty GetFunctionSelectedProperty = DependencyProperty.Register("GetFunctionSelected", typeof(bool), typeof(DataChangeMethod), new PropertyMetadata(false, GetFunctionSelectedChangedCallback));

		private static void GetFunctionSelectedChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataChangeMethod;
			if (t == null) return;
			if (!(bool)e.NewValue) return;

			t.Parameters = t.MethodData.GetParameters;
			t.ValuesList.ItemsSource = t.Parameters;
			t.Documentation = t.MethodData.GetDocumentation;
		}

		public bool OpenFunctionSelected { get { return (bool)GetValue(OpenFunctionSelectedProperty); } set { SetValue(OpenFunctionSelectedProperty, value); } }
		public static readonly DependencyProperty OpenFunctionSelectedProperty = DependencyProperty.Register("OpenFunctionSelected", typeof(bool), typeof(DataChangeMethod), new PropertyMetadata(false, OpenFunctionSelectedChangedCallback));

		private static void OpenFunctionSelectedChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataChangeMethod;
			if (t == null) return;
			if (!(bool)e.NewValue) return;

			t.Parameters = t.MethodData.OpenParameters;
			t.ValuesList.ItemsSource = t.Parameters;
			t.Documentation = t.MethodData.OpenDocumentation;
		}

		public bool CloseFunctionSelected { get { return (bool)GetValue(CloseFunctionSelectedProperty); } set { SetValue(CloseFunctionSelectedProperty, value); } }
		public static readonly DependencyProperty CloseFunctionSelectedProperty = DependencyProperty.Register("CloseFunctionSelected", typeof(bool), typeof(DataChangeMethod), new PropertyMetadata(false, CloseFunctionSelectedChangedCallback));

		private static void CloseFunctionSelectedChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataChangeMethod;
			if (t == null) return;
			if (!(bool)e.NewValue) return;

			t.Parameters = t.MethodData.CloseParameters;
			t.ValuesList.ItemsSource = t.Parameters;
			t.Documentation = t.MethodData.CloseDocumentation;
		}

		public bool NewFunctionSelected { get { return (bool)GetValue(NewFunctionSelectedProperty); } set { SetValue(NewFunctionSelectedProperty, value); } }
		public static readonly DependencyProperty NewFunctionSelectedProperty = DependencyProperty.Register("NewFunctionSelected", typeof(bool), typeof(DataChangeMethod), new PropertyMetadata(false, NewFunctionSelectedChangedCallback));

		private static void NewFunctionSelectedChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataChangeMethod;
			if (t == null) return;
			if (!(bool) e.NewValue) return;

			t.Parameters = t.MethodData.NewParameters;
			t.ValuesList.ItemsSource = t.Parameters;
			t.Documentation = t.MethodData.NewDocumentation;
		}

		public bool DeleteFunctionSelected { get { return (bool)GetValue(DeleteFunctionSelectedProperty); } set { SetValue(DeleteFunctionSelectedProperty, value); } }
		public static readonly DependencyProperty DeleteFunctionSelectedProperty = DependencyProperty.Register("DeleteFunctionSelected", typeof(bool), typeof(DataChangeMethod), new PropertyMetadata(false, DeleteFunctionSelectedChangedCallback));

		private static void DeleteFunctionSelectedChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as DataChangeMethod;
			if (t == null) return;
			if (!(bool)e.NewValue) return;

			t.Parameters = t.MethodData.DeleteParameters;
			t.ValuesList.ItemsSource = t.Parameters;
			t.Documentation = t.MethodData.DeleteDocumentation;
		}

		private int DragItemStartIndex;
		private int DragItemNewIndex;
		private WcfMethodParameter DragElement;
		private Point DragStartPos;
		private Themes.DragAdorner DragAdorner;
		private AdornerLayer DragLayer;
		public bool IsDragging { get; set; }
		public bool DragHasLeftScope { get; set; }

		public DataChangeMethod()
		{
			InitializeComponent();
		}

		public DataChangeMethod(WcfDataChangeMethod Data)
		{
			MethodData = Data;
			ServiceProject = Data.Owner.Parent.Owner;

			InitializeComponent();

			if (MethodData.GenerateOpenCloseFunction) OpenFunctionSelected = true;
			else if (MethodData.GenerateGetFunction) GetFunctionSelected = true;
			else if (MethodData.GenerateNewDeleteFunction) NewFunctionSelected = true;
			else MethodDetails.Visibility = Visibility.Collapsed;

			DataContext = this;
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
			var ts = ValuesList.ItemTemplate.FindName("OperationParameterDataType", cp) as EllipticBit.Controls.WPF.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("OperationParameterElementName", cp) as EllipticBit.Controls.WPF.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;

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
			DragElement = ValuesList.SelectedItem as WcfMethodParameter;
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

			foreach (var O in Parameters)
			{
				var lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(O);
				if (lbi == null) continue;
				lbi.Margin = new Thickness(0);
				lbi.Padding = new Thickness(0);
			}
		}

		private void ValuesList_PreviewDragOver(object sender, DragEventArgs e)
		{
			ContentPresenter cp = GetListBoxItemPresenter();
			if (cp == null) return;
			var ts = ValuesList.ItemTemplate.FindName("OperationParameterDataType", cp) as EllipticBit.Controls.WPF.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("OperationParameterElementName", cp) as EllipticBit.Controls.WPF.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;

			if (DragAdorner != null)
			{
				DragAdorner.LeftOffset = e.GetPosition(ValuesList).X;
				DragAdorner.TopOffset = e.GetPosition(ValuesList).Y;
			}

			Point MP = e.GetPosition(ValuesList);
			HitTestResult htr = VisualTreeHelper.HitTest(ValuesList, MP);
			if (htr != null)
			{
				foreach (var O in Parameters)
				{
					var lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(O);
					var lbiht = Globals.GetVisualParent<ListBoxItem>(htr.VisualHit);
					if (lbi == null) continue;
					if (lbiht == null) continue;
					if (!Equals(lbi, lbiht))
					{
						lbi.Margin = new Thickness(0);
						lbi.Padding = new Thickness(0);
					}
					else
					{
						if (DragAdorner != null) lbi.Margin = new Thickness(0, DragAdorner.ActualHeight, 0, 0);
						lbi.Padding = new Thickness(0);
						DragItemNewIndex = ValuesList.ItemContainerGenerator.IndexFromContainer(lbi);
					}
				}
			}
		}

		private void ValuesList_Drop(object sender, DragEventArgs e)
		{
			Parameters.Move(DragItemStartIndex, DragItemNewIndex);

			foreach (var O in Parameters)
			{
				var lbi = (ListBoxItem)ValuesList.ItemContainerGenerator.ContainerFromItem(O);
				if (lbi == null) continue;
				lbi.Margin = new Thickness(0);
				lbi.Padding = new Thickness(0);
			}
		}

		#endregion

		private ContentPresenter GetListBoxItemPresenter()
		{
			if (ValuesList.SelectedItem == null) return null;
			var lbi = ValuesList.ItemContainerGenerator.ContainerFromIndex(ValuesList.SelectedIndex) as ListBoxItem;
			return Globals.GetVisualChild<ContentPresenter>(lbi);
		}

		private void AddParameterType_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				AddParameterName.Focus();
		}

		private void AddParameterType_ValidationChanged(object Sender, RoutedEventArgs E)
		{
			AddParameter.IsEnabled = (!string.IsNullOrEmpty(AddParameterName.Text) && !AddParameterName.IsInvalid && AddParameterType.IsValid);
		}

		private void AddParameterName_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			e.IsValid = true;
			if (string.IsNullOrEmpty(AddParameterName.Text)) return;

			e.IsValid = RegExs.MatchCodeName.IsMatch(AddParameterName.Text);
			AddParameter.IsEnabled = (!string.IsNullOrEmpty(AddParameterName.Text) && e.IsValid && AddParameterType.IsValid);
		}

		private void AddParameterName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				if (!string.IsNullOrEmpty(AddParameterName.Text))
					AddParameter_Click(sender, null);
		}

		private void AddParameterName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (AddParameterName.IsInvalid == false)
			{
				AddParameter.IsEnabled = false;
				return;
			}
			if (AddParameterType.OpenType != null && AddParameterName.Text != "") AddParameter.IsEnabled = true;
			else AddParameter.IsEnabled = false;
		}

		private void AddParameter_Click(object sender, RoutedEventArgs e)
		{
			if (AddParameter.IsEnabled == false) return;
			Parameters.Add(new WcfMethodParameter(AddParameterType.OpenType, AddParameterName.Text, MethodData.Owner, MethodData));
			AddParameterType.OpenType = null;
			AddParameterType.Focus();
			AddParameterName.Text = "";
		}

		private void OperationParameterElementName_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (ValuesList.SelectedIndex == 0) ValuesList.SelectedIndex = Parameters.Count - 1; else ValuesList.SelectedIndex--;
			if (e.Key == Key.Down) if (ValuesList.SelectedIndex == Parameters.Count - 1) ValuesList.SelectedIndex = 0; else ValuesList.SelectedIndex++;

			if (e.Key != Key.Up && e.Key != Key.Down) return;
			var ts = ValuesList.ItemTemplate.FindName("OperationParameterElementName", GetListBoxItemPresenter()) as EllipticBit.Controls.WPF.TextBox;
			if (ts == null) return;
			ts.Focus();
		}

		private void OperationParameterDocumentation_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (ValuesList.SelectedIndex == 0) ValuesList.SelectedIndex = Parameters.Count - 1; else ValuesList.SelectedIndex--;
			if (e.Key == Key.Down) if (ValuesList.SelectedIndex == Parameters.Count - 1) ValuesList.SelectedIndex = 0; else ValuesList.SelectedIndex++;

			if (e.Key != Key.Up && e.Key != Key.Down) return;
			var ts = ValuesList.ItemTemplate.FindName("OperationParameterDocumentation", GetListBoxItemPresenter()) as EllipticBit.Controls.WPF.TextBox;
			if (ts == null) return;
			ts.Focus();
		}

		private void OperationParameterElementName_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			var ElementName = sender as EllipticBit.Controls.WPF.TextBox;
			if (ElementName == null) return;
			string t = RegExs.ReplaceSpaces.Replace(string.IsNullOrEmpty(ElementName.Text) ? "" : ElementName.Text, @"");

			e.IsValid = RegExs.MatchCodeName.IsMatch(t);
		}

		private void DeleteOperationParameter_Click(object sender, RoutedEventArgs e)
		{
			var lbi = Globals.GetVisualParent<ListBoxItem>(sender);
			var OP = lbi.Content as WcfMethodParameter;
			if (OP == null) return;

			DialogService.ShowMessageDialog("NETPath", "Delete Method Parameter?", "Are you sure you want to delete the '" + OP.Type + " " + OP.Name + "' method parameter?", new DialogAction("Yes", () => Parameters.Remove(OP), true), new DialogAction("No", false, true));
		}

		private void EnableFunctions_Unchecked(object Sender, RoutedEventArgs E)
		{
			MethodDetails.Visibility = Visibility.Visible;
			if (MethodData.GenerateOpenCloseFunction) OpenFunctionSelected = true;
			else if (MethodData.GenerateGetFunction) GetFunctionSelected = true;
			else if (MethodData.GenerateNewDeleteFunction) NewFunctionSelected = true;
			else MethodDetails.Visibility = Visibility.Collapsed;
		}
	}
}