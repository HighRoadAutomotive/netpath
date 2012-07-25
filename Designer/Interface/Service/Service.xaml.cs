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

namespace WCFArchitect.Interface.Service
{
	internal partial class Service : Grid
	{
		public Projects.Service Data { get; set; }

		Projects.Operation SelectedOperation { get; set; }
		Projects.Property SelectedProperty { get; set; }

		private int DragItemStartIndex;
		private int DragItemNewIndex;
		private Point DragStartPos;
		private Themes.DragAdorner DragAdorner;
		private AdornerLayer DragLayer;
		public bool IsDragging { get; set; }
		public bool DragHasLeftScope { get; set; }

		public Service(Projects.Service Data)
		{
			this.Data = Data;

			InitializeComponent();

			OperationsList.ItemsSource = Data.Operations;
			PropertiesList.ItemsSource = Data.Properties;

			if (Data.IsCallback == true)
			{
				SCIsCallback.Content = "Yes";
				CSIsCallback.Content = "Yes";
				ServiceCallbackGroup.Visibility = System.Windows.Visibility.Collapsed;
				CallbackServiceGroup.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				SCIsCallback.Content = "No";
				CSIsCallback.Content = "No";
				ServiceCallbackGroup.Visibility = System.Windows.Visibility.Visible;
				CallbackServiceGroup.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		#region - Drag/Drop Support - 

		private void OperationsList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragStartPos = e.GetPosition(null);
		}

		private void OperationsList_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
			{
				Point position = e.GetPosition(null);

				if (Math.Abs(position.X - DragStartPos.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - DragStartPos.Y) > SystemParameters.MinimumVerticalDragDistance)
				{
					StartOperationDrag(e);
				}
			}
		}

		private void PropertiesList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragStartPos = e.GetPosition(null);
		}

		private void PropertiesList_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
			{
				Point position = e.GetPosition(null);

				if (Math.Abs(position.X - DragStartPos.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - DragStartPos.Y) > SystemParameters.MinimumVerticalDragDistance)
				{
					StartPropertyDrag(e);
				}
			}
		}

		private void StartOperationDrag(MouseEventArgs e)
		{
			//Here we create our adorner.. 
			DragItemStartIndex = OperationsList.SelectedIndex;
			try { DragAdorner = new Themes.DragAdorner(OperationsList, (UIElement)OperationsList.ItemContainerGenerator.ContainerFromItem(OperationsList.SelectedItem), true, 0.5); }
			catch { return; }
			DragLayer = AdornerLayer.GetAdornerLayer(OperationsList as Visual);
			DragLayer.Add(DragAdorner);

			IsDragging = true;
			DragHasLeftScope = false;

			DataObject data = new DataObject(System.Windows.DataFormats.Text.ToString(), "");
			DragDropEffects de = DragDrop.DoDragDrop(OperationsList, data, DragDropEffects.Move);

			// Clean up our mess :) 
			AdornerLayer.GetAdornerLayer(OperationsList).Remove(DragAdorner);
			DragAdorner = null;

			IsDragging = false;
		}

		private void StartPropertyDrag(MouseEventArgs e)
		{
			//Here we create our adorner.. 
			DragItemStartIndex = PropertiesList.SelectedIndex;
			try { DragAdorner = new Themes.DragAdorner(PropertiesList, (UIElement)PropertiesList.ItemContainerGenerator.ContainerFromItem(PropertiesList.SelectedItem), true, 0.5); }
			catch { return; }
			DragLayer = AdornerLayer.GetAdornerLayer(PropertiesList as Visual);
			DragLayer.Add(DragAdorner);

			IsDragging = true;
			DragHasLeftScope = false;

			DataObject data = new DataObject(System.Windows.DataFormats.Text.ToString(), "");
			DragDropEffects de = DragDrop.DoDragDrop(PropertiesList, data, DragDropEffects.Move);

			// Clean up our mess :) 
			AdornerLayer.GetAdornerLayer(PropertiesList).Remove(DragAdorner);
			DragAdorner = null;

			IsDragging = false;
		}

		private void OperationsList_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			if (DragHasLeftScope)
			{
				e.Action = DragAction.Cancel;
				e.Handled = true;
			}
		}

		private void OperationsList_DragLeave(object sender, DragEventArgs e)
		{
			if (e.OriginalSource == OperationsList)
			{
				Point p = e.GetPosition(OperationsList);
				Rect r = VisualTreeHelper.GetContentBounds(OperationsList);
				if (!r.Contains(p))
				{
					this.DragHasLeftScope = true;
					e.Handled = true;
				}
			}
		}

		private void OperationsList_Drop(object sender, DragEventArgs e)
		{
			Data.Operations.Move(DragItemStartIndex, DragItemNewIndex);

			foreach (Projects.Operation O in Data.Operations)
			{
				ListBoxItem lbi = (ListBoxItem)OperationsList.ItemContainerGenerator.ContainerFromItem(O);
				if (lbi == null) continue;
				lbi.Margin = new Thickness(0);
			}
		}

		private void OperationsList_PreviewDragOver(object sender, DragEventArgs e)
		{
			if (DragAdorner != null)
			{
				DragAdorner.LeftOffset = e.GetPosition(OperationsList).X;
				DragAdorner.TopOffset = e.GetPosition(OperationsList).Y;
			}

			Point MP = e.GetPosition(OperationsList);
			HitTestResult htr = VisualTreeHelper.HitTest(OperationsList, MP);
			if (htr != null)
			{
				foreach (Projects.Operation O in Data.Operations)
				{
					ListBoxItem lbi = (ListBoxItem)OperationsList.ItemContainerGenerator.ContainerFromItem(O);
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
						DragItemNewIndex = OperationsList.ItemContainerGenerator.IndexFromContainer(lbi);
					}
				}
			}
		}

		private void PropertiesList_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			if (DragHasLeftScope)
			{
				e.Action = DragAction.Cancel;
				e.Handled = true;
			}
		}

		private void PropertiesList_DragLeave(object sender, DragEventArgs e)
		{
			if (e.OriginalSource == PropertiesList)
			{
				Point p = e.GetPosition(PropertiesList);
				Rect r = VisualTreeHelper.GetContentBounds(PropertiesList);
				if (!r.Contains(p))
				{
					this.DragHasLeftScope = true;
					e.Handled = true;
				}
			}
		}

		private void PropertiesList_PreviewDragOver(object sender, DragEventArgs e)
		{
			if (DragAdorner != null)
			{
				DragAdorner.LeftOffset = e.GetPosition(PropertiesList).X;
				DragAdorner.TopOffset = e.GetPosition(PropertiesList).Y;
			}

			Point MP = e.GetPosition(PropertiesList);
			HitTestResult htr = VisualTreeHelper.HitTest(PropertiesList, MP);
			if (htr != null)
			{
				foreach (Projects.Property O in Data.Properties)
				{
					ListBoxItem lbi = (ListBoxItem)PropertiesList.ItemContainerGenerator.ContainerFromItem(O);
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
						DragItemNewIndex = PropertiesList.ItemContainerGenerator.IndexFromContainer(lbi);
					}
				}
			}
		}

		private void PropertiesList_Drop(object sender, DragEventArgs e)
		{
			Data.Operations.Move(DragItemStartIndex, DragItemNewIndex);

			foreach (Projects.Property O in Data.Properties)
			{
				ListBoxItem lbi = (ListBoxItem)PropertiesList.ItemContainerGenerator.ContainerFromItem(O);
				if (lbi == null) continue;
				lbi.Margin = new Thickness(0);
			}
		}
		
		#endregion

		private void CodeName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(CodeName.Text);
		}

		private void ContractName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(ContractName.Text);
		}

		private void SelectCallback_Click(object sender, RoutedEventArgs e)
		{
			if (Data.Callback == null)
			{
				SelectService SS = new SelectService(Data.Parent.Owner, true);
				if (SS.ShowDialog() == true)
				{
					Data.Callback = SS.SelectedService;
					Data.Callback.Callback = Data;
					Callback.Text = Data.Callback.Name;
				}
			}
			else
			{
				Data.Callback.Callback = null;
				Data.Callback = null;
			}
		}

		private void IsCallback_Checked(object sender, RoutedEventArgs e)
		{
			if (IsLoaded == false) return;
			ServiceCallbackGroup.Visibility = System.Windows.Visibility.Collapsed;
			CallbackServiceGroup.Visibility = System.Windows.Visibility.Visible;
			if (Data.Callback != null)
			{
				Data.Callback.Callback = null;
				Data.Callback = null;
			}
			SCIsCallback.Content = "Yes";
			CSIsCallback.Content = "Yes";
		}

		private void IsCallback_Unchecked(object sender, RoutedEventArgs e)
		{
			if (IsLoaded == false) return;
			ServiceCallbackGroup.Visibility = System.Windows.Visibility.Visible;
			CallbackServiceGroup.Visibility = System.Windows.Visibility.Collapsed;
			if (Data.Callback != null)
			{
				Data.Callback = null;
			}
			SCIsCallback.Content = "No";
			CSIsCallback.Content = "No";
		}

		private void OpenService_Click(object sender, RoutedEventArgs e)
		{
			Globals.MainScreen.OpenProjectItem(Data.Callback);
		}

		private void OperationsSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (OperationsSearch.Text.Length < 3)
				Data.SearchOperations("");
			else 
				Data.SearchOperations(OperationsSearch.Text);
		}

		private void OperationsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (OperationsList.SelectedItem != null)
			{
				OperationContentEmpty.Visibility = System.Windows.Visibility.Collapsed;
				OperationContent.Content = new Operation((Projects.Operation)OperationsList.SelectedItem);
				SelectedOperation = (Projects.Operation)OperationsList.SelectedItem;
			}
			else
			{
				OperationContentEmpty.Visibility = System.Windows.Visibility.Visible;
				OperationContent.Content = null;
				SelectedOperation = null;
			}
		}

		private void PropertiesSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (PropertiesSearch.Text.Length < 3)
				Data.SearchProperties("");
			else
				Data.SearchProperties(PropertiesSearch.Text);
		}

		private void PropertiesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (PropertiesList.SelectedItem != null)
			{
				PropertyContentEmpty.Visibility = System.Windows.Visibility.Collapsed;
				PropertyContent.Content = new Property((Projects.Property)PropertiesList.SelectedItem);
				SelectedProperty = (Projects.Property)PropertiesList.SelectedItem;
			}
			else
			{
				PropertyContentEmpty.Visibility = System.Windows.Visibility.Visible;
				PropertyContent.Content = null;
				SelectedOperation = null;
			}
		}

		public void AddOperation_Click(object sender, RoutedEventArgs e)
		{
			Prospective.Controls.InputBox.Show(AddOperationFinish, Globals.MainScreen, "Please enter a name for the new operation.", "New Operation", true, Prospective.Controls.InputBoxType.String, "", "pack://application:,,,/WCFArchitect;component/Icons/X16/Operation.png", "pack://application:,,,/WCFArchitect;component/Icons/X32/Operation.png");
		}

		private void AddOperationFinish(object Name)
		{
			if (Name == null) return;
			Projects.Operation TO = new Projects.Operation((string)Name, Data);
			Data.Operations.Add(TO);
			OperationsList.SelectedItem = TO;

			Data.IsDirty = true;
		}

		private void DeleteSelectedOperation_Click(object sender, RoutedEventArgs e)
		{
#if STANDARD
			if (Data.Parent.Owner.IsDependencyProject == true) return;
#endif
			Projects.Operation TO = null;
			ListBoxItem clickedListBoxItem = Globals.GetVisualParent<ListBoxItem>(sender); 
			if (clickedListBoxItem != null) { TO = clickedListBoxItem.Content as Projects.Operation; } 

			if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + TO.Name + "' operation?", "Delete Operation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				int SI = Data.Operations.IndexOf(TO);
				Data.Operations.Remove(TO);
				if (SI >= Data.Operations.Count)
					OperationsList.SelectedIndex = Data.Operations.Count - 1;
				else
					OperationsList.SelectedIndex = SI + 1;

				Data.IsDirty = true;
			}
		}

		private void AddPropertyFinish(object Name)
		{
			if (Name == null) return;
			Projects.Property TP = new Projects.Property((string)Name, Data);
			Data.Properties.Add(TP);
			PropertiesList.SelectedItem = TP;

			Data.IsDirty = true;
		}

		public void AddProperty_Click(object sender, RoutedEventArgs e)
		{
			Prospective.Controls.InputBox.Show(AddPropertyFinish, Globals.MainScreen, "Please enter a name for the new property.", "New Property", true, Prospective.Controls.InputBoxType.String, "", "pack://application:,,,/WCFArchitect;component/Icons/X16/Property.png", "pack://application:,,,/WCFArchitect;component/Icons/X32/Property.png");
		}


		private void DeleteSelectedProperty_Click(object sender, RoutedEventArgs e)
		{
#if STANDARD
			if (Data.Parent.Owner.IsDependencyProject == true) return;
#endif
			Projects.Property TP = sender as Projects.Property;
			ListBoxItem clickedListBoxItem = Globals.GetVisualParent<ListBoxItem>(sender);
			if (clickedListBoxItem != null) { TP = clickedListBoxItem.Content as Projects.Property; } 

			if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + TP.Name + "' Property?", "Delete Property", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				int SI = Data.Properties.IndexOf(TP);
				Data.Properties.Remove(TP);
				if (SI >= Data.Properties.Count)
					PropertiesList.SelectedIndex = Data.Properties.Count - 1;
				else
					PropertiesList.SelectedIndex = SI + 1;

				Data.IsDirty = true;
			}
		}
	}

	[ValueConversion(typeof(Projects.ServiceBinding), typeof(string))]
	public class ServiceBindingTypeImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "";
			if (value.GetType() == typeof(Projects.ServiceBindingBasicHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/BasicHTTP.png";
			if (value.GetType() == typeof(Projects.ServiceBindingWSHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/WSHTTP.png";
			if (value.GetType() == typeof(Projects.ServiceBindingWS2007HTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/WS2007HTTP.png";
			if (value.GetType() == typeof(Projects.ServiceBindingWSDualHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/WSDualHTTP.png";
			if (value.GetType() == typeof(Projects.ServiceBindingWSFederationHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/WSFederationHTTP.png";
			if (value.GetType() == typeof(Projects.ServiceBindingWS2007FederationHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/WS2007FederationHTTP.png";
			if (value.GetType() == typeof(Projects.ServiceBindingTCP)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/TCP.png";
			if (value.GetType() == typeof(Projects.ServiceBindingNamedPipe)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/NamedPipe.png";
			if (value.GetType() == typeof(Projects.ServiceBindingMSMQ)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/MSMQ.png";
			if (value.GetType() == typeof(Projects.ServiceBindingPeerTCP)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/PeerTCP.png";
			if (value.GetType() == typeof(Projects.ServiceBindingWebHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/WebHTTP.png";
			if (value.GetType() == typeof(Projects.ServiceBindingMSMQIntegration)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/MSMQIntegration.png";
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(Projects.Service), typeof(string))]
	public class ServiceNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool IsCallback = System.Convert.ToBoolean(parameter);
			if (IsCallback == false) if (value == null) return "No Callback Selected";
			if (IsCallback == true) if (value == null) return "No Service Selected";
			Projects.Service val = (Projects.Service)value;
			return val.Name;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(Projects.Service), typeof(string))]
	public class ServiceSelectConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "Select";
			return "Remove";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(object), typeof(bool))]
	public class NullBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value == null ? false : true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}