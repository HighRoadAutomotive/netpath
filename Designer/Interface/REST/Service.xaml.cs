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
using NETPath.Projects.Helpers;
using Prospective.Controls;
using Prospective.Controls.Dialogs;
using NETPath.Projects;
using NETPath.Projects.Helpers;

namespace NETPath.Interface.REST
{
	internal partial class Service : Grid
	{
		public Projects.RESTService ServiceType { get { return (Projects.RESTService)GetValue(ServiceTypeProperty); } set { SetValue(ServiceTypeProperty, value); } }
		public static readonly DependencyProperty ServiceTypeProperty = DependencyProperty.Register("ServiceType", typeof(Projects.RESTService), typeof(Service));

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

		public Service(Projects.RESTService Data)
		{
			ServiceType = Data;
			ServiceProject = Data.Parent.Owner;

			InitializeComponent();

			RESTMethod s = ServiceType.ServiceOperations.FirstOrDefault(a => a.IsSelected);
			if (s != null) ServiceOperationsList.SelectedItem = s;
		}

		#region - Operations Screen -

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

			foreach (Projects.RESTMethod O in ServiceType.ServiceOperations)
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
			foreach (Projects.RESTMethod O in ServiceType.ServiceOperations)
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

		private void AddServiceMemberType_ValidationChanged(object Sender, RoutedEventArgs E)
		{
			if (AddServiceMemberType.OpenType == null) return;
			AddServiceMethod.IsEnabled = (!string.IsNullOrEmpty(AddServiceMemberName.Text) && AddServiceMemberType.OpenType.Primitive != PrimitiveTypes.Void && !AddServiceMemberName.IsInvalid && AddServiceMemberType.IsValid);
		}

		private void AddServiceMemberName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = true;
			if (string.IsNullOrEmpty(AddServiceMemberName.Text)) return;

			e.IsValid = RegExs.MatchCodeName.IsMatch(AddServiceMemberName.Text);
			if (AddServiceMemberType.OpenType == null) return;
			AddServiceMethod.IsEnabled = (!string.IsNullOrEmpty(AddServiceMemberName.Text) && AddServiceMemberType.OpenType.Primitive != PrimitiveTypes.Void && !AddServiceMemberName.IsInvalid && AddServiceMemberType.IsValid);
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
				return;
			}
			if (AddServiceMemberType.OpenType != null && AddServiceMemberType.OpenType.Primitive != PrimitiveTypes.Void && AddServiceMemberName.Text != "") AddServiceMethod.IsEnabled = true;
			else AddServiceMethod.IsEnabled = false;
		}

		private void AddServiceMethod_Click(object sender, RoutedEventArgs e)
		{
			if (AddServiceMethod.IsEnabled == false) return;

			var t = new Projects.RESTMethod(AddServiceMemberType.OpenType, AddServiceMemberName.Text, ServiceType);
			ServiceType.ServiceOperations.Add(t);

			AddServiceMemberType.Focus();
			AddServiceMemberType.OpenType = null;
			AddServiceMemberName.Text = "";

			ServiceOperationsList.SelectedItem = t;
		}

		private void ServiceOperationsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var t = ServiceOperationsList.SelectedItem as RESTMethod;
			if (t == null) return;

			//Set the new item as selected.
			foreach (RESTMethod o in ServiceType.ServiceOperations)
				o.IsSelected = false;
			t.IsSelected = true;

			ActiveOperation = new Method(t);
		}

		private void DeleteOperation_Click(object sender, RoutedEventArgs e)
		{
			var lbi = Globals.GetVisualParent<ListBoxItem>(sender);
			var OP = lbi.Content as RESTMethod;
			if (OP == null) return;

			DialogService.ShowMessageDialog("NETPath", "Delete Method?", "Are you sure you want to delete the '" + OP.ReturnType + " " + OP.ServerName + "' method?", new DialogAction("Yes", () => ServiceType.ServiceOperations.Remove(OP), true), new DialogAction("No", false, true));
		}

		#endregion

		#region - Endpoint Screen -

		private void AddAddressHeader_Click(object sender, RoutedEventArgs e)
		{
			if (IsAddressHeaderValid() == false) return;
			ServiceType.EndpointAddressHeaders.Add(new Projects.HostEndpointAddressHeader(AddressHeaderName.Text, AddressHeader.Text));
		}

		private void AddressHeader_TextChanged(object sender, TextChangedEventArgs e)
		{
			AddAddressHeader.IsEnabled = true;
			if (IsAddressHeaderValid() == false)
				AddAddressHeader.IsEnabled = false;
		}

		private void AddressHeaderName_TextChanged(object sender, TextChangedEventArgs e)
		{
			AddAddressHeader.IsEnabled = true;
			if (IsAddressHeaderValid() == false)
				AddAddressHeader.IsEnabled = false;
		}

		private void DeleteAddressHeader_Click(object sender, RoutedEventArgs e)
		{
			var lbi = Globals.GetVisualParent<ListBoxItem>(sender);
			var OP = lbi.Content as Projects.HostEndpointAddressHeader;
			if (OP == null) return;

			DialogService.ShowMessageDialog("NETPath", "Delete Endpoint Address Header?", "Are you sure you want to delete the address header '" + OP.Name + "'?", new DialogAction("Yes", () => ServiceType.EndpointAddressHeaders.Remove(OP), true), new DialogAction("No", false, true));
		}

		private bool IsAddressHeaderValid()
		{
			if (!RegExs.MatchHTTPURI.IsMatch(AddressHeader.Text)) return false;
			if (AddressHeader.Text == "") return false;
			if (AddressHeaderName.Text == "") return false;
			return true;
		}

		private void ProxyAddress_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			ServiceType.EndpointBinding.ProxyAddress = RegExs.ReplaceSpaces.Replace(ProxyAddress.Text, @"");
		}

		private void ProxyAddress_Validate(object sender, ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchHTTPURI.IsMatch(ProxyAddress.Text);
		}

		private void MaxBufferPoolSize_Validate(object sender, ValidateEventArgs e)
		{
			e.IsValid = true;
			try { Convert.ToInt64(MaxBufferPoolSize.Text); }
			catch (Exception) { e.IsValid = false; }
		}

		private void MaxBufferSize_Validate(object sender, ValidateEventArgs e)
		{
			e.IsValid = true;
			try { Convert.ToInt64(MaxBufferSize.Text); }
			catch (Exception) { e.IsValid = false; }
		}

		private void MaxReceivedMessageSize_Validate(object sender, ValidateEventArgs e)
		{
			e.IsValid = true;
			try { Convert.ToInt64(MaxReceivedMessageSize.Text); }
			catch (Exception) { e.IsValid = false; }
		}

		#endregion

		#region - Behaviors Screen -

		private void DebugHttpHelpPageUrl_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			ServiceType.DebugBehavior.HttpHelpPageUrl = RegExs.ReplaceSpaces.Replace(DebugHttpHelpPageUrl.Text, "");
		}

		private void DebugHttpHelpPageUrl_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchHTTPURI.IsMatch(DebugHttpHelpPageUrl.Text);
		}

		private void DebugHttpsHelpPageUrl_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			ServiceType.DebugBehavior.HttpsHelpPageUrl = RegExs.ReplaceSpaces.Replace(DebugHttpsHelpPageUrl.Text, "");
		}

		private void DebugHttpsHelpPageUrl_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchHTTPURI.IsMatch(DebugHttpsHelpPageUrl.Text);
		}

		#endregion
	}
}