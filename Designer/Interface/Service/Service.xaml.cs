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
using C1.WPF;
using Prospective.Controls.Dialogs;
using NETPath.Projects;
using NETPath.Projects.Helpers;

namespace NETPath.Interface.Service
{
	internal partial class Service : Grid
	{
		public Projects.Service ServiceType { get { return (Projects.Service)GetValue(ServiceTypeProperty); } set { SetValue(ServiceTypeProperty, value); } }
		public static readonly DependencyProperty ServiceTypeProperty = DependencyProperty.Register("ServiceType", typeof(Projects.Service), typeof(Service));

		public Projects.Project ServiceProject { get { return (Projects.Project)GetValue(ServiceProjectProperty); } set { SetValue(ServiceProjectProperty, value); } }
		public static readonly DependencyProperty ServiceProjectProperty = DependencyProperty.Register("ServiceProject", typeof(Projects.Project), typeof(Service));

		public object ActiveOperation { get { return GetValue(ActiveOperationProperty); } set { SetValue(ActiveOperationProperty, value); } }
		public static readonly DependencyProperty ActiveOperationProperty = DependencyProperty.Register("ActiveOperation", typeof(object), typeof(Service));

		public object ActiveCallback { get { return GetValue(ActiveCallbackProperty); } set { SetValue(ActiveCallbackProperty, value); } }
		public static readonly DependencyProperty ActiveCallbackProperty = DependencyProperty.Register("ActiveCallback", typeof(object), typeof(Service));

		private int DragItemStartIndex;
		private int DragItemNewIndex;
		private Point DragStartPos;
		private Themes.DragAdorner DragAdorner;
		private AdornerLayer DragLayer;
		public bool IsDragging { get; set; }
		public bool DragHasLeftScope { get; set; }

		public Service(Projects.Service Data)
		{
			ServiceType = Data;
			ServiceProject = Data.Parent.Owner;

			InitializeComponent();

			Operation s = ServiceType.ServiceOperations.FirstOrDefault(a => a.IsSelected);
			if (s != null) ServiceOperationsList.SelectedItem = s;
			Operation c = ServiceType.CallbackOperations.FirstOrDefault(a => a.IsSelected);
			if (c != null) CallbackOperationsList.SelectedItem = c;
		}

		#region - Service Drag/Drop Support - 

		private void ServiceOperationsList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragStartPos = e.GetPosition(null);
		}

		private void ServiceOperationsList_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed || IsDragging) return;
			Point position = e.GetPosition(null);

			if (Math.Abs(position.X - DragStartPos.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - DragStartPos.Y) > SystemParameters.MinimumVerticalDragDistance)
			{
				StartServiceOperationDrag(e);
			}
		}

		private void StartServiceOperationDrag(MouseEventArgs e)
		{
			//Here we create our adorner.. 
			DragItemStartIndex = ServiceOperationsList.SelectedIndex;
			try { DragAdorner = new Themes.DragAdorner(ServiceOperationsList, (UIElement)ServiceOperationsList.ItemContainerGenerator.ContainerFromItem(ServiceOperationsList.SelectedItem), true, 0.5); }
			catch { return; }
			DragLayer = AdornerLayer.GetAdornerLayer(ServiceOperationsList);
			DragLayer.Add(DragAdorner);

			IsDragging = true;
			DragHasLeftScope = false;

			var data = new DataObject(DataFormats.Text, "");
			DragDropEffects de = DragDrop.DoDragDrop(ServiceOperationsList, data, DragDropEffects.Move);

			// Clean up our mess :) 
			AdornerLayer.GetAdornerLayer(ServiceOperationsList).Remove(DragAdorner);
			DragAdorner = null;

			IsDragging = false;
		}

		private void ServiceOperationsList_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			if (!DragHasLeftScope) return;
			e.Action = DragAction.Cancel;
			e.Handled = true;
		}

		private void ServiceOperationsList_DragLeave(object sender, DragEventArgs e)
		{
			if (!Equals(e.OriginalSource, ServiceOperationsList)) return;
			Point p = e.GetPosition(ServiceOperationsList);
			Rect r = VisualTreeHelper.GetContentBounds(ServiceOperationsList);
			if (r.Contains(p)) return;
			DragHasLeftScope = true;
			e.Handled = true;
		}

		private void ServiceOperationsList_Drop(object sender, DragEventArgs e)
		{
			ServiceType.ServiceOperations.Move(DragItemStartIndex, DragItemNewIndex);

			foreach (Projects.Operation O in ServiceType.ServiceOperations)
			{
				var lbi = (ListBoxItem)ServiceOperationsList.ItemContainerGenerator.ContainerFromItem(O);
				if (lbi == null) continue;
				lbi.Margin = new Thickness(0);
			}
		}

		private void ServiceOperationsList_PreviewDragOver(object sender, DragEventArgs e)
		{
			if (DragAdorner != null)
			{
				DragAdorner.LeftOffset = e.GetPosition(ServiceOperationsList).X;
				DragAdorner.TopOffset = e.GetPosition(ServiceOperationsList).Y;
			}

			Point MP = e.GetPosition(ServiceOperationsList);
			HitTestResult htr = VisualTreeHelper.HitTest(ServiceOperationsList, MP);
			if (htr == null) return;
			foreach (Projects.Operation O in ServiceType.ServiceOperations)
			{
				var lbi = (ListBoxItem)ServiceOperationsList.ItemContainerGenerator.ContainerFromItem(O);
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
					DragItemNewIndex = ServiceOperationsList.ItemContainerGenerator.IndexFromContainer(lbi);
				}
			}
		}
		
		#endregion

		#region - Callback Drag/Drop Support -

		private void CallbackOperationsList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragStartPos = e.GetPosition(null);
		}

		private void CallbackOperationsList_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed || IsDragging) return;
			Point position = e.GetPosition(null);

			if (Math.Abs(position.X - DragStartPos.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - DragStartPos.Y) > SystemParameters.MinimumVerticalDragDistance)
			{
				StartCallbackOperationDrag(e);
			}
		}

		private void StartCallbackOperationDrag(MouseEventArgs e)
		{
			//Here we create our adorner.. 
			DragItemStartIndex = CallbackOperationsList.SelectedIndex;
			try { DragAdorner = new Themes.DragAdorner(CallbackOperationsList, (UIElement)CallbackOperationsList.ItemContainerGenerator.ContainerFromItem(CallbackOperationsList.SelectedItem), true, 0.5); }
			catch { return; }
			DragLayer = AdornerLayer.GetAdornerLayer(CallbackOperationsList);
			DragLayer.Add(DragAdorner);

			IsDragging = true;
			DragHasLeftScope = false;

			var data = new DataObject(DataFormats.Text, "");
			DragDropEffects de = DragDrop.DoDragDrop(CallbackOperationsList, data, DragDropEffects.Move);

			// Clean up our mess :) 
			AdornerLayer.GetAdornerLayer(CallbackOperationsList).Remove(DragAdorner);
			DragAdorner = null;

			IsDragging = false;
		}

		private void CallbackOperationsList_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			if (!DragHasLeftScope) return;
			e.Action = DragAction.Cancel;
			e.Handled = true;
		}

		private void CallbackOperationsList_DragLeave(object sender, DragEventArgs e)
		{
			if (!Equals(e.OriginalSource, CallbackOperationsList)) return;
			Point p = e.GetPosition(CallbackOperationsList);
			Rect r = VisualTreeHelper.GetContentBounds(CallbackOperationsList);
			if (r.Contains(p)) return;
			DragHasLeftScope = true;
			e.Handled = true;
		}

		private void CallbackOperationsList_Drop(object sender, DragEventArgs e)
		{
			ServiceType.CallbackOperations.Move(DragItemStartIndex, DragItemNewIndex);

			foreach (Projects.Operation O in ServiceType.CallbackOperations)
			{
				var lbi = (ListBoxItem)CallbackOperationsList.ItemContainerGenerator.ContainerFromItem(O);
				if (lbi == null) continue;
				lbi.Margin = new Thickness(0);
			}
		}

		private void CallbackOperationsList_PreviewDragOver(object sender, DragEventArgs e)
		{
			if (DragAdorner != null)
			{
				DragAdorner.LeftOffset = e.GetPosition(CallbackOperationsList).X;
				DragAdorner.TopOffset = e.GetPosition(CallbackOperationsList).Y;
			}

			Point MP = e.GetPosition(CallbackOperationsList);
			HitTestResult htr = VisualTreeHelper.HitTest(CallbackOperationsList, MP);
			if (htr == null) return;
			foreach (Projects.Operation O in ServiceType.CallbackOperations)
			{
				var lbi = (ListBoxItem)CallbackOperationsList.ItemContainerGenerator.ContainerFromItem(O);
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
					DragItemNewIndex = CallbackOperationsList.ItemContainerGenerator.IndexFromContainer(lbi);
				}
			}
		}

		#endregion

		#region - Service Operation Handlers -

		private void AddServiceMemberType_ValidationChanged(object Sender, RoutedEventArgs E)
		{
			if (AddServiceMemberType.OpenType == null) return;
			AddServiceMethod.IsEnabled = (!string.IsNullOrEmpty(AddServiceMemberName.Text) && !AddServiceMemberName.IsInvalid && AddServiceMemberType.IsValid);
			AddServiceDCMethod.IsEnabled = (!string.IsNullOrEmpty(AddServiceMemberName.Text) && (AddServiceMemberType.OpenType.TypeMode == DataTypeMode.Class || AddServiceMemberType.OpenType.TypeMode == DataTypeMode.Struct) && !AddServiceMemberName.IsInvalid && AddServiceMemberType.IsValid);
			AddServiceProperty.IsEnabled = (!string.IsNullOrEmpty(AddServiceMemberName.Text) && AddServiceMemberType.OpenType.Primitive != PrimitiveTypes.Void && !AddServiceMemberName.IsInvalid && AddServiceMemberType.IsValid);
		}

		private void AddServiceMemberName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = true;
			if (string.IsNullOrEmpty(AddServiceMemberName.Text)) return;

			e.IsValid = RegExs.MatchCodeName.IsMatch(AddServiceMemberName.Text);
			if (AddServiceMemberType.OpenType == null) return;
			AddServiceMethod.IsEnabled = (!string.IsNullOrEmpty(AddServiceMemberName.Text) && e.IsValid && AddServiceMemberType.IsValid);
			AddServiceDCMethod.IsEnabled = (!string.IsNullOrEmpty(AddServiceMemberName.Text) && (AddServiceMemberType.OpenType.TypeMode == DataTypeMode.Class || AddServiceMemberType.OpenType.TypeMode == DataTypeMode.Struct) && !AddServiceMemberName.IsInvalid && AddServiceMemberType.IsValid);
			AddServiceProperty.IsEnabled = (!string.IsNullOrEmpty(AddServiceMemberName.Text) && AddServiceMemberType.OpenType.Primitive != PrimitiveTypes.Void && !AddServiceMemberName.IsInvalid && AddServiceMemberType.IsValid);
		}

		private void AddServiceMemberName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				if (!string.IsNullOrEmpty(AddServiceMemberName.Text))
					AddServiceMethod_Click(sender, null);
		}

		private void AddServiceMemberName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (AddServiceMemberName.IsInvalid == false)
			{
				AddServiceMethod.IsEnabled = false;
				AddServiceProperty.IsEnabled = false;
				return;
			}
			if (AddServiceMemberType.OpenType != null && AddServiceMemberName.Text != "") AddServiceMethod.IsEnabled = AddServiceDCMethod.IsEnabled = true;
			else AddServiceMethod.IsEnabled = AddServiceDCMethod.IsEnabled = false;
			if (AddServiceMemberType.OpenType != null && (AddServiceMemberType.OpenType.TypeMode == DataTypeMode.Class || AddServiceMemberType.OpenType.TypeMode == DataTypeMode.Struct) && AddServiceMemberName.Text != "") AddServiceDCMethod.IsEnabled = true;
			else AddServiceDCMethod.IsEnabled = false;
			if (AddServiceMemberType.OpenType != null && AddServiceMemberType.OpenType.Primitive != PrimitiveTypes.Void && AddServiceMemberName.Text != "") AddServiceProperty.IsEnabled = true;
			else AddServiceProperty.IsEnabled = false;
		}

		private void AddServiceMethod_Click(object sender, RoutedEventArgs e)
		{
			if (AddServiceMethod.IsEnabled == false) return;

			var t = new Projects.Method(AddServiceMemberType.OpenType, AddServiceMemberName.Text, ServiceType);
			ServiceType.ServiceOperations.Add(t);

			AddServiceMemberType.Focus();
			AddServiceMemberType.OpenType = null;
			AddServiceMemberName.Text = "";

			ServiceOperationsList.SelectedItem = t;
		}

		private void AddServiceDCMethod_Click(object sender, RoutedEventArgs e)
		{
			if (AddServiceMethod.IsEnabled == false) return;

			var t = new Projects.DataChangeMethod(AddServiceMemberType.OpenType, AddServiceMemberName.Text, ServiceType);
			ServiceType.ServiceOperations.Add(t);

			AddServiceMemberType.Focus();
			AddServiceMemberType.OpenType = null;
			AddServiceMemberName.Text = "";

			ServiceOperationsList.SelectedItem = t;
		}

		private void AddServiceProperty_Click(object sender, RoutedEventArgs e)
		{
			if (AddServiceMethod.IsEnabled == false) return;

			var t = new Projects.Property(AddServiceMemberType.OpenType, AddServiceMemberName.Text, ServiceType);
			ServiceType.ServiceOperations.Add(t);

			AddServiceMemberType.Focus();
			AddServiceMemberType.OpenType = null;
			AddServiceMemberName.Text = "";

			ServiceOperationsList.SelectedItem = t;
		}

		private void ServiceOperationsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var t = ServiceOperationsList.SelectedItem as Operation;
			if (t == null) return;

			//Set the new item as selected.
			foreach (Operation o in ServiceType.ServiceOperations)
				o.IsSelected = false;
			t.IsSelected = true;

			if (t.GetType() == typeof(Projects.Method))
			{
				var m = t as Projects.Method;
				if (m == null) return;
				ActiveOperation = new Method(m);
			}

			if (t.GetType() == typeof(Projects.DataChangeMethod))
			{
				var m = t as Projects.DataChangeMethod;
				if (m == null) return;
				ActiveOperation = new DataChangeMethod(m);
			}
			else
			{
				var m = t as Projects.Property;
				if (m == null) return;
				ActiveOperation = new Property(m);
			}

		}

		#endregion

		#region - Callback Operation Handlers -

		private void AddCallbackMemberType_ValidationChanged(object Sender, RoutedEventArgs E)
		{
			if (AddCallbackMemberType.OpenType == null) return;
			AddCallbackMethod.IsEnabled = (!string.IsNullOrEmpty(AddCallbackMemberName.Text) && !AddCallbackMemberName.IsInvalid && AddCallbackMemberType.IsValid);
			AddCallbackProperty.IsEnabled = (!string.IsNullOrEmpty(AddCallbackMemberName.Text) && AddCallbackMemberType.OpenType.Primitive != PrimitiveTypes.Void && !AddCallbackMemberName.IsInvalid && AddCallbackMemberType.IsValid);
		}

		private void AddCallbackMemberName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = true;
			if (string.IsNullOrEmpty(AddCallbackMemberName.Text)) return;

			e.IsValid = RegExs.MatchCodeName.IsMatch(AddCallbackMemberName.Text);
			if (AddCallbackMemberType.OpenType == null) return;
			AddCallbackMethod.IsEnabled = (!string.IsNullOrEmpty(AddCallbackMemberName.Text) && e.IsValid && AddCallbackMemberType.IsValid);
			AddCallbackProperty.IsEnabled = (!string.IsNullOrEmpty(AddCallbackMemberName.Text) && AddCallbackMemberType.OpenType.Primitive != PrimitiveTypes.Void && !AddCallbackMemberName.IsInvalid && AddCallbackMemberType.IsValid);
		}

		private void AddCallbackMemberName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				if (!string.IsNullOrEmpty(AddCallbackMemberName.Text))
					AddCallbackMethod_Click(sender, null);
		}

		private void AddCallbackMemberName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (AddCallbackMemberName.IsInvalid == false)
			{
				AddCallbackMethod.IsEnabled = false;
				AddCallbackProperty.IsEnabled = false;
				return;
			}
			if (AddCallbackMemberType.OpenType != null && AddCallbackMemberName.Text != "") AddCallbackMethod.IsEnabled = true;
			else AddCallbackMethod.IsEnabled = false;
			if (AddCallbackMemberType.OpenType != null && AddCallbackMemberType.OpenType.Primitive != PrimitiveTypes.Void && AddCallbackMemberName.Text != "") AddCallbackProperty.IsEnabled = true;
			else AddCallbackProperty.IsEnabled = false;
		}

		private void AddCallbackMethod_Click(object sender, RoutedEventArgs e)
		{
			if (AddCallbackMethod.IsEnabled == false) return;

			var t = new Projects.Method(AddCallbackMemberType.OpenType, AddCallbackMemberName.Text, ServiceType);
			ServiceType.CallbackOperations.Add(t);

			AddCallbackMemberType.Focus();
			AddCallbackMemberType.OpenType = null;
			AddCallbackMemberName.Text = "";

			CallbackOperationsList.SelectedItem = t;
		}

		private void AddCallbackProperty_Click(object sender, RoutedEventArgs e)
		{
			if (AddCallbackMethod.IsEnabled == false) return;

			var t = new Projects.Property(AddCallbackMemberType.OpenType, AddCallbackMemberName.Text, ServiceType);
			ServiceType.CallbackOperations.Add(t);

			AddCallbackMemberType.Focus();
			AddCallbackMemberType.OpenType = null;
			AddCallbackMemberName.Text = "";

			CallbackOperationsList.SelectedItem = t;
		}

		private void CallbackOperationsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var t = CallbackOperationsList.SelectedItem as Operation;
			if (t == null) return;

			//Set the new item as selected.
			foreach (Operation o in ServiceType.CallbackOperations)
				o.IsSelected = false;
			t.IsSelected = true;

			if (t.GetType() == typeof(Projects.Method))
			{
				var m = t as Projects.Method;
				if (m == null) return;
				ActiveCallback = new Method(m);
			}
			else
			{
				var m = t as Projects.Property;
				if (m == null) return;
				ActiveCallback = new Property(m);
			}
		}

		#endregion

		private void DeleteOperation_Click(object sender, RoutedEventArgs e)
		{
			var lbi = Globals.GetVisualParent<ListBoxItem>(sender);
			var OP = lbi.Content as Operation;
			if (OP == null) return;

			DialogService.ShowMessageDialog("NETPath", "Delete Method?", "Are you sure you want to delete the '" + OP.ReturnType + " " + OP.ServerName + "' method?", new DialogAction("Yes", () => { ServiceType.ServiceOperations.Remove(OP); ServiceType.CallbackOperations.Remove(OP); }, true), new DialogAction("No", false, true));
		}

		private void DeleteProperty_Click(object sender, RoutedEventArgs e)
		{
			var lbi = Globals.GetVisualParent<ListBoxItem>(sender);
			var OP = lbi.Content as Operation;
			if (OP == null) return;

			DialogService.ShowMessageDialog("NETPath", "Delete Property?", "Are you sure you want to delete the '" + OP.ReturnType + " " + OP.ServerName + "' property?", new DialogAction("Yes", () => { ServiceType.ServiceOperations.Remove(OP); ServiceType.CallbackOperations.Remove(OP); }, true), new DialogAction("No", false, true));
		}

		private void SBMaxItemsInObjectGraph_ValueChanged(object Sender, PropertyChangedEventArgs<double> E)
		{
			ServiceType.SBMaxItemsInObjectGraph = Convert.ToInt32(E.NewValue);
		}

		private void CBMaxItemsInObjectGraph_ValueChanged(object Sender, PropertyChangedEventArgs<double> E)
		{
			ServiceType.CBMaxItemsInObjectGraph = Convert.ToInt32(E.NewValue);
		}
	}
}