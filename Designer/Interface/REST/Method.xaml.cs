using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Prospective.Controls.Dialogs;
using NETPath.Projects.Helpers;

namespace NETPath.Interface.REST
{
	internal partial class Method : Grid
	{
		public Projects.RESTMethod MethodData { get { return (Projects.RESTMethod)GetValue(MethodDataProperty); } set { SetValue(MethodDataProperty, value); } }
		public static readonly DependencyProperty MethodDataProperty = DependencyProperty.Register("MethodData", typeof(Projects.RESTMethod), typeof(Method));

		public Projects.Project ServiceProject { get { return (Projects.Project)GetValue(ServiceProjectProperty); } set { SetValue(ServiceProjectProperty, value); } }
		public static readonly DependencyProperty ServiceProjectProperty = DependencyProperty.Register("ServiceProject", typeof(Projects.Project), typeof(Method));

		private int DragItemStartIndex;
		private int DragItemNewIndex;
		private Projects.MethodParameter DragElement;
		private Point DragStartPos;
		private Themes.DragAdorner DragAdorner;
		private AdornerLayer DragLayer;
		public bool IsDragging { get; set; }
		public bool DragHasLeftScope { get; set; }

		public Method()
		{
			InitializeComponent();
		}

		public Method(Projects.RESTMethod Data)
		{
			MethodData = Data;
			ServiceProject = Data.Owner.Parent.Owner;

			InitializeComponent();

			ValuesList.ItemsSource = Data.Parameters;
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
			var tds = ValuesList.ItemTemplate.FindName("OperationParameterDataType", cp) as NETPath.Interface.Data.TypeSelector;
			if (tds == null) return;
			if (tds.IsKeyboardFocusWithin) return;
			var ts = ValuesList.ItemTemplate.FindName("OperationParameterElementName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("OperationParameterDocumentation", cp) as Prospective.Controls.TextBox;
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
			DragElement = ValuesList.SelectedItem as Projects.MethodParameter;
			DragItemStartIndex = ValuesList.SelectedIndex;
			var tuie = (UIElement)ValuesList.ItemContainerGenerator.ContainerFromItem(ValuesList.SelectedItem);
			if (tuie == null) return;
			try
			{ DragAdorner = new Themes.DragAdorner(ValuesList, tuie, true, 0.5); }
			catch
			{ return; }
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

			foreach (Projects.RESTMethodParameter O in MethodData.Parameters)
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
			var tds = ValuesList.ItemTemplate.FindName("OperationParameterDataType", cp) as NETPath.Interface.Data.TypeSelector;
			if (tds == null) return;
			if (tds.IsKeyboardFocusWithin) return;
			var ts = ValuesList.ItemTemplate.FindName("OperationParameterElementName", cp) as Prospective.Controls.TextBox;
			if (ts == null) return;
			if (ts.IsKeyboardFocusWithin) return;
			ts = ValuesList.ItemTemplate.FindName("OperationParameterDocumentation", cp) as Prospective.Controls.TextBox;
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
				foreach (Projects.RESTMethodParameter O in MethodData.Parameters)
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
			MethodData.Parameters.Move(DragItemStartIndex, DragItemNewIndex);

			foreach (Projects.RESTMethodParameter O in MethodData.Parameters)
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

		private void AddParameterName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
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
			MethodData.Parameters.Add(new Projects.RESTMethodParameter(AddParameterType.OpenType, AddParameterName.Text, MethodData.Owner, MethodData));
			AddParameterType.OpenType = null;
			AddParameterType.Focus();
			AddParameterName.Text = "";
		}

		private void OperationParameterElementName_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (ValuesList.SelectedIndex == 0) ValuesList.SelectedIndex = MethodData.Parameters.Count - 1; else ValuesList.SelectedIndex--;
			if (e.Key == Key.Down) if (ValuesList.SelectedIndex == MethodData.Parameters.Count - 1) ValuesList.SelectedIndex = 0; else ValuesList.SelectedIndex++;

			if (e.Key != Key.Up && e.Key != Key.Down) return;
			var ts = ValuesList.ItemTemplate.FindName("OperationParameterElementName", GetListBoxItemPresenter()) as Prospective.Controls.TextBox;
			if (ts == null) return;
			ts.Focus();
		}

		private void OperationParameterDocumentation_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (ValuesList.SelectedIndex == 0) ValuesList.SelectedIndex = MethodData.Parameters.Count - 1; else ValuesList.SelectedIndex--;
			if (e.Key == Key.Down) if (ValuesList.SelectedIndex == MethodData.Parameters.Count - 1) ValuesList.SelectedIndex = 0; else ValuesList.SelectedIndex++;

			if (e.Key != Key.Up && e.Key != Key.Down) return;
			var ts = ValuesList.ItemTemplate.FindName("OperationParameterDocumentation", GetListBoxItemPresenter()) as Prospective.Controls.TextBox;
			if (ts == null) return;
			ts.Focus();
		}

		private void OperationParameterElementName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			var ElementName = sender as Prospective.Controls.TextBox;
			if (ElementName == null) return;
			string t = RegExs.ReplaceSpaces.Replace(string.IsNullOrEmpty(ElementName.Text) ? "" : ElementName.Text, @"");

			e.IsValid = RegExs.MatchCodeName.IsMatch(t);
		}

		private void DeleteOperationParameter_Click(object sender, RoutedEventArgs e)
		{
			var lbi = Globals.GetVisualParent<ListBoxItem>(sender);
			var OP = lbi.Content as Projects.RESTMethodParameter;
			if (OP == null) return;

			DialogService.ShowMessageDialog("NETPath", "Delete Method Parameter?", "Are you sure you want to delete the '" + OP.Type + " " + OP.Name + "' method parameter?", new DialogAction("Yes", () => MethodData.Parameters.Remove(OP), true), new DialogAction("No", false, true));
		}
	}

	public class RequestConfigurationTemplateSelector : DataTemplateSelector
	{
		public DataTemplate WebTemplate { get; set; }
		public DataTemplate ClientTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item == null) return WebTemplate;
			if (item.GetType() == typeof(Projects.RESTHTTPWebConfiguration)) return WebTemplate;
			if (item.GetType() == typeof(Projects.RESTHTTPClientConfiguration)) return ClientTemplate;
			return WebTemplate;
		}
	}
}