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
using EllipticBit.Controls.WPF.Dialogs;
using NETPath.Projects;
using NETPath.Projects.WebApi;
using NETPath.Projects.Helpers;

namespace NETPath.Interface.WebApi.Service
{
	internal partial class Method : Grid
	{
		public WebApiMethod MethodData { get { return (WebApiMethod)GetValue(MethodDataProperty); } set { SetValue(MethodDataProperty, value); } }
		public static readonly DependencyProperty MethodDataProperty = DependencyProperty.Register("MethodData", typeof(WebApiMethod), typeof(Method));

		public Projects.Project ServiceProject { get { return (Projects.Project)GetValue(ServiceProjectProperty); } set { SetValue(ServiceProjectProperty, value); } }
		public static readonly DependencyProperty ServiceProjectProperty = DependencyProperty.Register("ServiceProject", typeof(Projects.Project), typeof(Method));

		private int DragItemStartIndex;
		private int DragItemNewIndex;
		private WebApiMethodParameter DragElement;
		private Point DragStartPos;
		private Themes.DragAdorner DragAdorner;
		private AdornerLayer DragLayer;
		public bool IsDragging { get; set; }
		public bool DragHasLeftScope { get; set; }

		public Method()
		{
			InitializeComponent();
		}

		public Method(WebApiMethod Data)
		{
			MethodData = Data;
			ServiceProject = Data.Owner.Parent.Owner;

			InitializeComponent();

			RouteList.ItemsSource = Data.RouteParameters;
			QueryList.ItemsSource = Data.QueryParameters;
			DataContext = this;
		}

		#region - Route Drag/Drop Support -

		private void RouteList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragStartPos = e.GetPosition(null);
		}

		private void RouteList_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			ContentPresenter cp = RouteGetListBoxItemPresenter();
			if (cp == null) return;

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
			DragElement = RouteList.SelectedItem as WebApiMethodParameter;
			DragItemStartIndex = RouteList.SelectedIndex;
			var tuie = (UIElement)RouteList.ItemContainerGenerator.ContainerFromItem(RouteList.SelectedItem);
			if (tuie == null) return;
			try
			{ DragAdorner = new Themes.DragAdorner(RouteList, tuie, true, 0.5); }
			catch
			{ return; }
			DragLayer = AdornerLayer.GetAdornerLayer(RouteList);
			DragLayer.Add(DragAdorner);

			IsDragging = true;
			DragHasLeftScope = false;

			var data = new DataObject(DataFormats.Text, "");
			DragDropEffects de = DragDrop.DoDragDrop(RouteList, data, DragDropEffects.Move);

			// Clean up our mess :)
			AdornerLayer.GetAdornerLayer(RouteList).Remove(DragAdorner);
			DragAdorner = null;

			IsDragging = false;
		}

		private void RouteList_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			if (DragHasLeftScope)
			{
				e.Action = DragAction.Cancel;
				e.Handled = true;
			}
		}

		private void RouteList_DragLeave(object sender, DragEventArgs e)
		{
			if (!Equals(e.OriginalSource, RouteList)) return;
			Point p = e.GetPosition(RouteList);
			Rect r = VisualTreeHelper.GetContentBounds(RouteList);
			if (r.Contains(p)) return;
			DragHasLeftScope = true;
			e.Handled = true;

			foreach (var O in MethodData.RouteParameters)
			{
				var lbi = (ListBoxItem)RouteList.ItemContainerGenerator.ContainerFromItem(O);
				if (lbi == null) continue;
				lbi.Margin = new Thickness(0);
				lbi.Padding = new Thickness(0);
			}
		}

		private void RouteList_PreviewDragOver(object sender, DragEventArgs e)
		{
			ContentPresenter cp = RouteGetListBoxItemPresenter();
			if (cp == null) return;

			if (DragAdorner != null)
			{
				DragAdorner.LeftOffset = e.GetPosition(RouteList).X;
				DragAdorner.TopOffset = e.GetPosition(RouteList).Y;
			}

			Point MP = e.GetPosition(RouteList);
			HitTestResult htr = VisualTreeHelper.HitTest(RouteList, MP);
			if (htr != null)
			{
				foreach (var O in MethodData.RouteParameters)
				{
					var lbi = (ListBoxItem)RouteList.ItemContainerGenerator.ContainerFromItem(O);
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
						DragItemNewIndex = RouteList.ItemContainerGenerator.IndexFromContainer(lbi);
					}
				}
			}
		}

		private void RouteList_Drop(object sender, DragEventArgs e)
		{
			MethodData.RouteParameters.Move(DragItemStartIndex, DragItemNewIndex);

			foreach (var O in MethodData.RouteParameters)
			{
				var lbi = (ListBoxItem)RouteList.ItemContainerGenerator.ContainerFromItem(O);
				if (lbi == null) continue;
				lbi.Margin = new Thickness(0);
				lbi.Padding = new Thickness(0);
			}
		}

		#endregion

		#region - Route Event Handling -

		private ContentPresenter RouteGetListBoxItemPresenter()
		{
			if (RouteList.SelectedItem == null) return null;
			var lbi = RouteList.ItemContainerGenerator.ContainerFromIndex(RouteList.SelectedIndex) as ListBoxItem;
			return Globals.GetVisualChild<ContentPresenter>(lbi);
		}

		private void AddRouteParameterType_Selected(object Sender, RoutedEventArgs E)
		{
			AddRouteParameter.IsEnabled = (!string.IsNullOrEmpty(AddRouteParameterName.Text) && !AddRouteParameterName.IsInvalid && AddRouteParameterType.IsValid);
			AddRouteParameterName.Focus();
		}

		private void AddRouteParameterName_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			e.IsValid = true;
			if (string.IsNullOrEmpty(AddRouteParameterName.Text)) return;

			e.IsValid = RegExs.MatchCodeName.IsMatch(AddRouteParameterName.Text);
			AddRouteParameter.IsEnabled = (!string.IsNullOrEmpty(AddRouteParameterName.Text) && e.IsValid && AddRouteParameterType.IsValid);
		}

		private void AddRouteParameterName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				if (!string.IsNullOrEmpty(AddRouteParameterName.Text))
					AddRouteParameter_Click(sender, null);
		}

		private void AddRouteParameterName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (AddRouteParameterName.IsInvalid == false)
			{
				AddRouteParameter.IsEnabled = false;
				return;
			}
			if (AddRouteParameterType.OpenType != null && AddRouteParameterName.Text != "") AddRouteParameter.IsEnabled = true;
			else AddRouteParameter.IsEnabled = false;
		}

		private void AddRouteParameter_Click(object sender, RoutedEventArgs e)
		{
			if (AddRouteParameter.IsEnabled == false) return;
			MethodData.RouteParameters.Add(new WebApiMethodParameter(AddRouteParameterType.OpenType, AddRouteParameterName.Text, MethodData.Owner, MethodData));
			AddRouteParameterType.Focus();
			AddRouteParameterType.OpenType = null;
			AddRouteParameterName.Text = "";
		}

		private void AddRouteFixedName_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			e.IsValid = true;
			if (string.IsNullOrEmpty(AddRouteFixedName.Text)) return;

			e.IsValid = RegExs.MatchCodeName.IsMatch(AddRouteFixedName.Text);
			AddRouteFixed.IsEnabled = (!string.IsNullOrEmpty(AddRouteFixedName.Text) && e.IsValid );
		}

		private void AddRouteFixedName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				if (!string.IsNullOrEmpty(AddRouteFixedName.Text))
					AddRouteFixed_Click(sender, null);
		}

		private void AddRouteFixedName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (AddRouteFixedName.IsInvalid == false)
			{
				AddRouteFixed.IsEnabled = false;
				return;
			}
			AddRouteFixed.IsEnabled = AddRouteFixedName.Text != "";
		}

		private void AddRouteFixed_Click(object sender, RoutedEventArgs e)
		{
			if (AddRouteFixed.IsEnabled == false) return;
			MethodData.RouteParameters.Add(new WebApiRouteParameter(AddRouteFixedName.Text, MethodData.Owner, MethodData));
			AddRouteFixedName.Text = "";
		}

		private void RouteParameterElementName_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (RouteList.SelectedIndex == 0) RouteList.SelectedIndex = MethodData.RouteParameters.Count - 1; else RouteList.SelectedIndex--;
			if (e.Key == Key.Down) if (RouteList.SelectedIndex == MethodData.RouteParameters.Count - 1) RouteList.SelectedIndex = 0; else RouteList.SelectedIndex++;

			if (e.Key != Key.Up && e.Key != Key.Down) return;
			var ts = RouteList.ItemTemplate.FindName("OperationParameterElementName", RouteGetListBoxItemPresenter()) as EllipticBit.Controls.WPF.TextBox;
			ts?.Focus();
		}

		private void RouteParameterDocumentation_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (RouteList.SelectedIndex == 0) RouteList.SelectedIndex = MethodData.RouteParameters.Count - 1; else RouteList.SelectedIndex--;
			if (e.Key == Key.Down) if (RouteList.SelectedIndex == MethodData.RouteParameters.Count - 1) RouteList.SelectedIndex = 0; else RouteList.SelectedIndex++;

			if (e.Key != Key.Up && e.Key != Key.Down) return;
			var ts = RouteList.ItemTemplate.FindName("OperationParameterDocumentation", RouteGetListBoxItemPresenter()) as EllipticBit.Controls.WPF.TextBox;
			ts?.Focus();
		}

		private void RouteParameterElementName_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			var ElementName = sender as EllipticBit.Controls.WPF.TextBox;
			if (ElementName == null) return;
			string t = RegExs.ReplaceSpaces.Replace(string.IsNullOrEmpty(ElementName.Text) ? "" : ElementName.Text, @"");

			e.IsValid = RegExs.MatchCodeName.IsMatch(t);
		}

		private void DeleteRouteParameter_Click(object sender, RoutedEventArgs e)
		{
			var lbi = Globals.GetVisualParent<ListBoxItem>(sender);
			var OP = lbi.Content as WebApiMethodParameter;
			if (OP == null) return;

			DialogService.ShowMessageDialog("NETPath", "Delete Route Parameter?", "Are you sure you want to delete the '" + OP.Type + " " + OP.Name + "' method parameter?", new DialogAction("Yes", () => MethodData.RouteParameters.Remove(OP), true), new DialogAction("No", false, true));
		}

		#endregion

		#region - Query Drag/Drop Support -

		private void QueryList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragStartPos = e.GetPosition(null);
		}

		private void QueryList_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			ContentPresenter cp = QueryGetListBoxItemPresenter();
			if (cp == null) return;

			if (e.LeftButton != MouseButtonState.Pressed || IsDragging) return;
			Point position = e.GetPosition(null);

			if (Math.Abs(position.X - DragStartPos.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - DragStartPos.Y) > SystemParameters.MinimumVerticalDragDistance)
			{
				QueryStartValueDrag(e);
			}
		}

		private void QueryStartValueDrag(MouseEventArgs e)
		{
			//Here we create our adorner..
			DragElement = QueryList.SelectedItem as WebApiMethodParameter;
			DragItemStartIndex = QueryList.SelectedIndex;
			var tuie = (UIElement)QueryList.ItemContainerGenerator.ContainerFromItem(QueryList.SelectedItem);
			if (tuie == null) return;
			try
			{ DragAdorner = new Themes.DragAdorner(QueryList, tuie, true, 0.5); }
			catch
			{ return; }
			DragLayer = AdornerLayer.GetAdornerLayer(QueryList);
			DragLayer.Add(DragAdorner);

			IsDragging = true;
			DragHasLeftScope = false;

			var data = new DataObject(DataFormats.Text, "");
			DragDropEffects de = DragDrop.DoDragDrop(QueryList, data, DragDropEffects.Move);

			// Clean up our mess :)
			AdornerLayer.GetAdornerLayer(QueryList).Remove(DragAdorner);
			DragAdorner = null;

			IsDragging = false;
		}

		private void QueryList_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			if (DragHasLeftScope)
			{
				e.Action = DragAction.Cancel;
				e.Handled = true;
			}
		}

		private void QueryList_DragLeave(object sender, DragEventArgs e)
		{
			if (!Equals(e.OriginalSource, QueryList)) return;
			Point p = e.GetPosition(QueryList);
			Rect r = VisualTreeHelper.GetContentBounds(QueryList);
			if (r.Contains(p)) return;
			DragHasLeftScope = true;
			e.Handled = true;

			foreach (WebApiMethodParameter O in MethodData.QueryParameters)
			{
				var lbi = (ListBoxItem)QueryList.ItemContainerGenerator.ContainerFromItem(O);
				if (lbi == null) continue;
				lbi.Margin = new Thickness(0);
				lbi.Padding = new Thickness(0);
			}
		}

		private void QueryList_PreviewDragOver(object sender, DragEventArgs e)
		{
			ContentPresenter cp = QueryGetListBoxItemPresenter();
			if (cp == null) return;

			if (DragAdorner != null)
			{
				DragAdorner.LeftOffset = e.GetPosition(QueryList).X;
				DragAdorner.TopOffset = e.GetPosition(QueryList).Y;
			}

			Point MP = e.GetPosition(QueryList);
			HitTestResult htr = VisualTreeHelper.HitTest(QueryList, MP);
			if (htr != null)
			{
				foreach (WebApiMethodParameter O in MethodData.QueryParameters)
				{
					var lbi = (ListBoxItem)QueryList.ItemContainerGenerator.ContainerFromItem(O);
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
						DragItemNewIndex = QueryList.ItemContainerGenerator.IndexFromContainer(lbi);
					}
				}
			}
		}

		private void QueryList_Drop(object sender, DragEventArgs e)
		{
			MethodData.QueryParameters.Move(DragItemStartIndex, DragItemNewIndex);

			foreach (WebApiMethodParameter O in MethodData.QueryParameters)
			{
				var lbi = (ListBoxItem)QueryList.ItemContainerGenerator.ContainerFromItem(O);
				if (lbi == null) continue;
				lbi.Margin = new Thickness(0);
				lbi.Padding = new Thickness(0);
			}
		}

		#endregion

		#region - Query Event Handling -

		private ContentPresenter QueryGetListBoxItemPresenter()
		{
			if (QueryList.SelectedItem == null) return null;
			var lbi = QueryList.ItemContainerGenerator.ContainerFromIndex(QueryList.SelectedIndex) as ListBoxItem;
			return Globals.GetVisualChild<ContentPresenter>(lbi);
		}

		private void AddQueryParameterType_Selected(object Sender, RoutedEventArgs E)
		{
			AddQueryParameter.IsEnabled = (!string.IsNullOrEmpty(AddQueryParameterName.Text) && !AddQueryParameterName.IsInvalid && AddQueryParameterType.IsValid);
			AddQueryParameterName.Focus();
		}

		private void AddQueryParameterName_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			e.IsValid = true;
			if (string.IsNullOrEmpty(AddQueryParameterName.Text)) return;

			e.IsValid = RegExs.MatchCodeName.IsMatch(AddQueryParameterName.Text);
			AddQueryParameter.IsEnabled = (!string.IsNullOrEmpty(AddQueryParameterName.Text) && e.IsValid && AddQueryParameterType.IsValid);
		}

		private void AddQueryParameterName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				if (!string.IsNullOrEmpty(AddQueryParameterName.Text))
					AddQueryParameter_Click(sender, null);
		}

		private void AddQueryParameterName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (AddQueryParameterName.IsInvalid == false)
			{
				AddQueryParameter.IsEnabled = false;
				return;
			}
			if (AddQueryParameterType.OpenType != null && AddQueryParameterName.Text != "") AddQueryParameter.IsEnabled = true;
			else AddQueryParameter.IsEnabled = false;
		}

		private void AddQueryParameter_Click(object sender, RoutedEventArgs e)
		{
			if (AddQueryParameter.IsEnabled == false) return;
			MethodData.QueryParameters.Add(new WebApiMethodParameter(AddQueryParameterType.OpenType, AddQueryParameterName.Text, MethodData.Owner, MethodData));
			AddQueryParameterType.OpenType = null;
			AddQueryParameterType.Focus();
			AddQueryParameterName.Text = "";
		}

		private void QueryParameterElementName_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (QueryList.SelectedIndex == 0) QueryList.SelectedIndex = MethodData.QueryParameters.Count - 1; else QueryList.SelectedIndex--;
			if (e.Key == Key.Down) if (QueryList.SelectedIndex == MethodData.QueryParameters.Count - 1) QueryList.SelectedIndex = 0; else QueryList.SelectedIndex++;

			if (e.Key != Key.Up && e.Key != Key.Down) return;
			var ts = QueryList.ItemTemplate.FindName("OperationParameterElementName", QueryGetListBoxItemPresenter()) as EllipticBit.Controls.WPF.TextBox;
			ts?.Focus();
		}

		private void QueryParameterDocumentation_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) if (QueryList.SelectedIndex == 0) QueryList.SelectedIndex = MethodData.QueryParameters.Count - 1; else QueryList.SelectedIndex--;
			if (e.Key == Key.Down) if (QueryList.SelectedIndex == MethodData.QueryParameters.Count - 1) QueryList.SelectedIndex = 0; else QueryList.SelectedIndex++;

			if (e.Key != Key.Up && e.Key != Key.Down) return;
			var ts = QueryList.ItemTemplate.FindName("OperationParameterDocumentation", QueryGetListBoxItemPresenter()) as EllipticBit.Controls.WPF.TextBox;
			ts?.Focus();
		}

		private void QueryParameterElementName_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			var ElementName = sender as EllipticBit.Controls.WPF.TextBox;
			if (ElementName == null) return;
			string t = RegExs.ReplaceSpaces.Replace(string.IsNullOrEmpty(ElementName.Text) ? "" : ElementName.Text, @"");

			e.IsValid = RegExs.MatchCodeName.IsMatch(t);
		}

		private void DeleteQueryParameter_Click(object sender, RoutedEventArgs e)
		{
			var lbi = Globals.GetVisualParent<ListBoxItem>(sender);
			var OP = lbi.Content as WebApiMethodParameter;
			if (OP == null) return;

			DialogService.ShowMessageDialog("NETPath", "Delete Query Parameter?", "Are you sure you want to delete the '" + OP.Type + " " + OP.Name + "' method parameter?", new DialogAction("Yes", () => MethodData.QueryParameters.Remove(OP), true), new DialogAction("No", false, true));
		}

		#endregion
	}

	public class RouteParameterTemplateSelector : DataTemplateSelector
	{
		public DataTemplate FixedTemplate { get; set; }
		public DataTemplate ParameterTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item == null) return FixedTemplate;
			if (item.GetType() == typeof(WebApiRouteParameter)) return FixedTemplate;
			if (item.GetType() == typeof(WebApiMethodParameter)) return ParameterTemplate;
			return FixedTemplate;
		}
	}
}