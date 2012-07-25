using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WCFArchitect.Interface
{
	internal partial class ErrorList : Grid
	{

		public bool FilterScopeErrors { get { return (bool)GetValue(FilterScopeErrorsProperty); } set { SetValue(FilterScopeErrorsProperty, value); } }
		public static readonly DependencyProperty FilterScopeErrorsProperty = DependencyProperty.Register("FilterScopeErrors", typeof(bool), typeof(ErrorList), new UIPropertyMetadata(true));

		public bool FilterScopeWarnings { get { return (bool)GetValue(FilterScopeWarningsProperty); } set { SetValue(FilterScopeWarningsProperty, value); } }
		public static readonly DependencyProperty FilterScopeWarningsProperty = DependencyProperty.Register("FilterScopeWarnings", typeof(bool), typeof(ErrorList), new UIPropertyMetadata(true));

		public bool FilterScopeMessages { get { return (bool)GetValue(FilterScopeMessagesProperty); } set { SetValue(FilterScopeMessagesProperty, value); } }
		public static readonly DependencyProperty FilterScopeMessagesProperty = DependencyProperty.Register("FilterScopeMessages", typeof(bool), typeof(ErrorList), new UIPropertyMetadata(true));

		private ObservableCollection<Compiler.CompileMessage> Messages;

		public ErrorList(ObservableCollection<Compiler.CompileMessage> Messages)
		{
			InitializeComponent();

			ErrorsList.ItemsSource = Messages;
			this.Messages = Messages;
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

		private void FilterCodeBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ApplyFiter();
		}

		private void FilterCodeBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (FilterCodeBox.Text == "")
				FilterBarCode.Visibility = System.Windows.Visibility.Visible;
		}

		private void FilterBarCode_Click(object sender, RoutedEventArgs e)
		{
			FilterBarCode.Visibility = System.Windows.Visibility.Hidden;
			FilterCodeBox.Focus();
		}

		private void FilterDescriptionBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ApplyFiter();
		}

		private void FilterDescriptionBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (FilterDescriptionBox.Text == "")
				FilterBarDescription.Visibility = System.Windows.Visibility.Visible;
		}

		private void FilterBarDescription_Click(object sender, RoutedEventArgs e)
		{
			FilterBarDescription.Visibility = System.Windows.Visibility.Hidden;
			FilterDescriptionBox.Focus();
		}

		private void FilterItemBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ApplyFiter();
		}

		private void FilterItemBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (FilterItemBox.Text == "")
				FilterBarItem.Visibility = System.Windows.Visibility.Visible;
		}

		private void FilterBarItem_Click(object sender, RoutedEventArgs e)
		{
			FilterBarItem.Visibility = System.Windows.Visibility.Hidden;
			FilterItemBox.Focus();
		}

		private void FilterProjectBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ApplyFiter();
		}

		private void FilterProjectBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (FilterProjectBox.Text == "")
				FilterBarProject.Visibility = System.Windows.Visibility.Visible;
		}

		private void FilterBarProject_Click(object sender, RoutedEventArgs e)
		{
			FilterBarProject.Visibility = System.Windows.Visibility.Hidden;
			FilterProjectBox.Focus();
		}

		private void ApplyFiter()
		{
			foreach (Compiler.CompileMessage CM in Messages)
			{
				ListBoxItem lbi = (ListBoxItem)ErrorsList.ItemContainerGenerator.ContainerFromItem(CM);
				if (lbi == null) continue;

				lbi.Visibility = System.Windows.Visibility.Collapsed;
				if (CM.Severity == Compiler.CompileMessageSeverity.Error && FilterScopeErrors == false) continue;
				if (CM.Severity == Compiler.CompileMessageSeverity.Warning && FilterScopeWarnings == false) continue;
				if (CM.Severity == Compiler.CompileMessageSeverity.Message && FilterScopeMessages == false) continue;
				if (CM.Code.IndexOf(FilterCodeBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				if (CM.Description.IndexOf(FilterDescriptionBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				if (CM.ErrorObject != null && CM.ErrorObjectType != null)
				{
					Type itemtype = CM.ErrorObjectType;
					if (itemtype == typeof(Projects.Project) || itemtype == typeof(Projects.DependencyProject))
					{
						Projects.Project t = CM.ErrorObject as Projects.Project;
						if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
					}
					else if (itemtype == typeof(Projects.Namespace))
					{
						Projects.Namespace t = CM.ErrorObject as Projects.Namespace;
						if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
					}
					else if (itemtype == typeof(Projects.Service))
					{
						Projects.Service t = CM.ErrorObject as Projects.Service;
						if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
					}
					else if (itemtype == typeof(Projects.Operation))
					{
						Projects.Operation t = CM.ErrorObject as Projects.Operation;
						if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
					}
					else if (itemtype == typeof(Projects.OperationParameter))
					{
						Projects.OperationParameter t = CM.ErrorObject as Projects.OperationParameter;
						if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
					}
					else if (itemtype == typeof(Projects.Property))
					{
						Projects.Property t = CM.ErrorObject as Projects.Property;
						if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
					}
					else if (itemtype == typeof(Projects.Data))
					{
						Projects.Data t = CM.ErrorObject as Projects.Data;
						if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
					}
					else if (itemtype == typeof(Projects.DataElement))
					{
						Projects.DataElement t = CM.ErrorObject as Projects.DataElement;
						if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
					}
					else if (itemtype == typeof(Projects.Enum))
					{
						Projects.Enum t = CM.ErrorObject as Projects.Enum;
						if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
					}
					else if (itemtype == typeof(Projects.EnumElement))
					{
						Projects.EnumElement t = CM.ErrorObject as Projects.EnumElement;
						if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
					}
				}
				if (CM.Owner != null)
				{
					Type itemtype = CM.Owner.GetType();
					if (itemtype == typeof(Projects.Project) || itemtype == typeof(Projects.DependencyProject))
					{
						Projects.Project t = CM.Owner as Projects.Project;
						if (t.Name.IndexOf(FilterProjectBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
					}
				}
				lbi.Visibility = System.Windows.Visibility.Visible;
			}
		}
	}
}