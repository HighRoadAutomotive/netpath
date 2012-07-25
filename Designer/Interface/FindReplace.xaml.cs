using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
	public partial class FindReplace : Grid
	{
		internal ObservableCollection<Projects.FindReplaceResult> FindResults { get; private set; }
		internal Projects.FindReplaceInfo FindInfo { get; set; }

		public FindReplace()
		{
			InitializeComponent();

			FindResults = new ObservableCollection<Projects.FindReplaceResult>();
			FindResultsList.ItemsSource = FindResults;
		}

		private void FindResultsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			if (FindResultsList.SelectedIndex < 0) return;
			Projects.FindReplaceResult FRR = FindResults[FindResultsList.SelectedIndex];

			if (FRR.Item.GetType() == typeof(Projects.Project) || FRR.Item.GetType() == typeof(Projects.DependencyProject))
			{
				Projects.Project T = FRR.Item as Projects.Project;
				Globals.MainScreen.OpenProjectItem(T);
				Interface.Project.Project TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Project;
				if (TW == null) return;

				if (FRR.Field == "Name") TW.ProjectName.Focus();
				if (FRR.Field == "Namespace") TW.txtProjectNamespace.Focus();
				if (FRR.Field == "Namespace URI") TW.txtProjectNamespaceURI.Focus();
				if (FRR.Field == "Bindings Namespace") TW.BindingsNamespace.Focus();
				if (FRR.Field == "Security Class") TW.SecurityClass.Focus();
				if (FRR.Field == "Hosts Namespace") TW.HostsNamespace.Focus();
			}
			if (FRR.Item.GetType() == typeof(Projects.Namespace))
			{
				Projects.Namespace T = FRR.Item as Projects.Namespace;
				Globals.MainScreen.OpenProjectItem(T);
				Interface.Namespace.Namespace TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Namespace.Namespace;
				if (TW == null) return;

				if (FRR.Field == "Name") TW.CodeName.Focus();
				if (FRR.Field == "Full Name") TW.FullName.Focus();
				if (FRR.Field == "URI") TW.NamespaceURI.Focus();
			}
			if (FRR.Item.GetType() == typeof(Projects.Service))
			{
				Projects.Service T = FRR.Item as Projects.Service;
				Globals.MainScreen.OpenProjectItem(T);
				Interface.Service.Service TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Service.Service;
				if (TW == null) return;

				if (FRR.Field == "Name") TW.DisplayName.Focus();
				if (FRR.Field == "Code Name") TW.CodeName.Focus();
				if (FRR.Field == "Contract Name") TW.ContractName.Focus();
			}
			if (FRR.Item.GetType() == typeof(Projects.Operation))
			{
				Projects.Operation T = FRR.Item as Projects.Operation;
				Globals.MainScreen.OpenProjectItem(T.Owner);
				Interface.Service.Operation TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Service.Operation;
				if (TW == null) return;

				if (FRR.Field == "Name") TW.DisplayName.Focus();
				if (FRR.Field == "Code Name") TW.CodeName.Focus();
				if (FRR.Field == "Contract Name") TW.ContractName.Focus();
				if (FRR.Field == "Return Type") TW.ReturnType.Focus();
			}
			if (FRR.Item.GetType() == typeof(Projects.OperationParameter))
			{
				Projects.OperationParameter T = FRR.Item as Projects.OperationParameter;
				Globals.MainScreen.OpenProjectItem(T.Owner);
				Projects.OperationParameter TW = Globals.MainScreen.GetOpenProjectScreen(T) as Projects.OperationParameter;
				if (TW == null) return;
				Interface.Service.Service TE = Globals.MainScreen.GetOpenProjectScreen(TW.Owner) as Interface.Service.Service;
				if (TE == null) return;

				TE.OperationsList.SelectedItem = TW;
				TE.OperationsList.ScrollIntoView(TW);
			}
			if (FRR.Item.GetType() == typeof(Projects.Property))
			{
				Projects.Property T = FRR.Item as Projects.Property;
				Globals.MainScreen.OpenProjectItem(T.Owner);
				Interface.Service.Property TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Service.Property;
				if (TW == null) return;

				if (FRR.Field == "Name") TW.DisplayName.Focus();
				if (FRR.Field == "Code Name") TW.CodeName.Focus();
				if (FRR.Field == "Return Type") TW.ReturnType.Focus();
			}
			if (FRR.Item.GetType() == typeof(Projects.Data))
			{
				Projects.Data T = FRR.Item as Projects.Data;
				Globals.MainScreen.OpenProjectItem(T);
				Interface.Data.Data TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Data.Data;
				if (TW == null) return;

				if (FRR.Field == "Name") TW.DisplayName.Focus();
				if (FRR.Field == "Code Name") TW.CodeName.Focus();
				if (FRR.Field == "Contract Name") TW.ContractName.Focus();
				if (FRR.Field == "WPF Name") TW.WPFName.Focus();
			}
			if (FRR.Item.GetType() == typeof(Projects.DataElement))
			{
				Projects.DataElement T = FRR.Item as Projects.DataElement;
				Globals.MainScreen.OpenProjectItem(T.Owner);
				Projects.DataElement TW = Globals.MainScreen.GetOpenProjectScreen(T) as Projects.DataElement;
				if (TW == null) return;
				Interface.Data.Data TE = Globals.MainScreen.GetOpenProjectScreen(TW.Owner) as Interface.Data.Data;
				if (TE == null) return;

				TE.ValuesList.SelectedItem = TW;
				TE.ValuesList.ScrollIntoView(TW);
			}
			if (FRR.Item.GetType() == typeof(Projects.Enum))
			{
				Projects.Enum T = FRR.Item as Projects.Enum;
				Globals.MainScreen.OpenProjectItem(T);
				Interface.Enum.Enum TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Enum.Enum;
				if (TW == null) return;

				if (FRR.Field == "Name") TW.DisplayName.Focus();
				if (FRR.Field == "Code Name") TW.CodeName.Focus();
				if (FRR.Field == "Contract Name") TW.ContractName.Focus();
			}
			if (FRR.Item.GetType() == typeof(Projects.EnumElement))
			{
				Projects.EnumElement T = FRR.Item as Projects.EnumElement;
				Globals.MainScreen.OpenProjectItem(T.Owner);

				Projects.EnumElement TW = Globals.MainScreen.GetOpenProjectScreen(T) as Projects.EnumElement;
				if (TW != null)
				{
					Interface.Enum.Enum TE = Globals.MainScreen.GetOpenProjectScreen(TW.Owner) as Interface.Enum.Enum;
					if (TE == null) return;
					if (TW.Owner.IsFlags == false)
					{
						TE.ValuesList.SelectedItem = TW;
						TE.ValuesList.ScrollIntoView(TW);
					}
					else
					{
						TE.FlagsList.SelectedItem = TW;
						TE.FlagsList.ScrollIntoView(TW);
					}
					return;
				}
			}
			if (FRR.Item.GetType() == typeof(Projects.ServiceBinding))
			{
				Projects.ServiceBinding T = FRR.Item as Projects.ServiceBinding;
				Globals.MainScreen.OpenProjectItem(T.Parent);

				{
					Interface.Project.Bindings.BasicHTTP TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Bindings.BasicHTTP;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						if (FRR.Field == "Namespace") TW.Namespace.Focus();
						return;
					}
				}
				{
					Interface.Project.Bindings.MSMQ TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Bindings.MSMQ;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						if (FRR.Field == "Namespace") TW.Namespace.Focus();
						return;
					}
				}
				{
					Interface.Project.Bindings.MSMQIntegration TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Bindings.MSMQIntegration;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						if (FRR.Field == "Namespace") TW.Namespace.Focus();
						return;
					}
				}
				{
					Interface.Project.Bindings.NamedPipe TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Bindings.NamedPipe;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						if (FRR.Field == "Namespace") TW.Namespace.Focus();
						return;
					}
				}
				{
					Interface.Project.Bindings.PeerTCP TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Bindings.PeerTCP;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						if (FRR.Field == "Namespace") TW.Namespace.Focus();
						return;
					}
				}
				{
					Interface.Project.Bindings.TCP TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Bindings.TCP;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						if (FRR.Field == "Namespace") TW.Namespace.Focus();
						return;
					}
				}
				{
					Interface.Project.Bindings.WebHTTP TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Bindings.WebHTTP;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						if (FRR.Field == "Namespace") TW.Namespace.Focus();
						return;
					}
				}
				{
					Interface.Project.Bindings.WS2007FederationHTTP TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Bindings.WS2007FederationHTTP;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						if (FRR.Field == "Namespace") TW.Namespace.Focus();
						return;
					}
				}
				{
					Interface.Project.Bindings.WS2007HTTP TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Bindings.WS2007HTTP;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						if (FRR.Field == "Namespace") TW.Namespace.Focus();
						return;
					}
				}
				{
					Interface.Project.Bindings.WSDualHTTP TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Bindings.WSDualHTTP;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						if (FRR.Field == "Namespace") TW.Namespace.Focus();
						return;
					}
				}
				{
					Interface.Project.Bindings.WSFederationHTTP TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Bindings.WSFederationHTTP;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						if (FRR.Field == "Namespace") TW.Namespace.Focus();
						return;
					}
				}
				{
					Interface.Project.Bindings.WSHTTP TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Bindings.WSHTTP;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						if (FRR.Field == "Namespace") TW.Namespace.Focus();
						return;
					}
				}
			}
			if (FRR.Item.GetType() == typeof(Projects.BindingSecurity))
			{
				Projects.BindingSecurity T = FRR.Item as Projects.BindingSecurity;
				Globals.MainScreen.OpenProjectItem(T.Parent);

				{
					Interface.Project.Security.BasicHTTP TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Security.BasicHTTP;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						return;
					}
				}
				{
					Interface.Project.Security.MSMQ TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Security.MSMQ;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						return;
					}
				}
				{
					Interface.Project.Security.MSMQIntegration TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Security.MSMQIntegration;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						return;
					}
				}
				{
					Interface.Project.Security.NamedPipe TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Security.NamedPipe;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						return;
					}
				}
				{
					Interface.Project.Security.PeerTCP TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Security.PeerTCP;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						return;
					}
				}
				{
					Interface.Project.Security.TCP TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Security.TCP;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						return;
					}
				}
				{
					Interface.Project.Security.WebHTTP TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Security.WebHTTP;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						return;
					}
				}
				{
					Interface.Project.Security.WSDualHTTP TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Security.WSDualHTTP;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						return;
					}
				}
				{
					Interface.Project.Security.WSFederationHTTP TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Security.WSFederationHTTP;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						return;
					}
				}
				{
					Interface.Project.Security.WSHTTP TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Security.WSHTTP;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						return;
					}
				}
			}
			if (FRR.Item.GetType() == typeof(Projects.Host))
			{
				Projects.Host T = FRR.Item as Projects.Host;
				Globals.MainScreen.OpenProjectItem(T.Parent);
				Interface.Project.Host.Host TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Host.Host;
				if (TW == null) return;

				if (FRR.Field == "Name") TW.DisplayName.Focus();
				if (FRR.Field == "Code Name") TW.CodeName.Focus();
				if (FRR.Field == "Namespace") TW.Namespace.Focus();
			}
			if (FRR.Item.GetType() == typeof(Projects.HostEndpoint))
			{
				Projects.HostEndpoint T = FRR.Item as Projects.HostEndpoint;
				Globals.MainScreen.OpenProjectItem(T.Parent.Parent);
				Interface.Project.Host.Endpoint TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Host.Endpoint;
				if (TW == null) return;

				if (FRR.Field == "Name") TW.DisplayName.Focus();
				if (FRR.Field == "Code Name") TW.CodeName.Focus();
				if (FRR.Field == "Server Address") TW.ServerAddress.Focus();
				if (FRR.Field == "Client Address") TW.ClientAddress.Focus();
			}
			if (FRR.Item.GetType() == typeof(Projects.HostBehavior))
			{
				Projects.HostBehavior T = FRR.Item as Projects.HostBehavior;
				Globals.MainScreen.OpenProjectItem(T.Parent.Parent);

				{
					Interface.Project.Host.DebugBehavior TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Host.DebugBehavior;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						return;
					}
				}
				{
					Interface.Project.Host.MetadataBehavior TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Host.MetadataBehavior;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						return;
					}
				}
				{
					Interface.Project.Host.ThrottlingBehavior TW = Globals.MainScreen.GetOpenProjectScreen(T) as Interface.Project.Host.ThrottlingBehavior;
					if (TW != null)
					{
						if (FRR.Field == "Name") TW.DisplayName.Focus();
						if (FRR.Field == "Code Name") TW.CodeName.Focus();
						return;
					}
				}
			}
		}

		private void FilterFieldBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ApplyFiter();
		}

		private void FilterFieldBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (FilterFieldBox.Text == "")
				FilterBarField.Visibility = System.Windows.Visibility.Visible;
		}

		private void FilterBarField_Click(object sender, RoutedEventArgs e)
		{
			FilterBarField.Visibility = System.Windows.Visibility.Hidden;
			FilterFieldBox.Focus();
		}

		private void FilterValueBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ApplyFiter();
		}

		private void FilterValueBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (FilterValueBox.Text == "")
				FilterBarValue.Visibility = System.Windows.Visibility.Visible;
		}

		private void FilterBarValue_Click(object sender, RoutedEventArgs e)
		{
			FilterBarValue.Visibility = System.Windows.Visibility.Hidden;
			FilterValueBox.Focus();
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
			foreach (Projects.FindReplaceResult FRR in FindResults)
			{
				ListBoxItem lbi = (ListBoxItem)FindResultsList.ItemContainerGenerator.ContainerFromItem(FRR);
				if (lbi == null) continue;

				Type itemtype = FRR.Item.GetType();

				lbi.Visibility = System.Windows.Visibility.Collapsed;
				if (FRR.Field.IndexOf(FilterFieldBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				if (FRR.Value.IndexOf(FilterValueBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				if (itemtype == typeof(Projects.Project) || itemtype == typeof(Projects.DependencyProject))
				{
					Projects.Project t = FRR.Item as Projects.Project;
					if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				}
				else if (itemtype == typeof(Projects.Namespace))
				{
					Projects.Namespace t = FRR.Item as Projects.Namespace;
					if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				}
				else if (itemtype == typeof(Projects.Service))
				{
					Projects.Service t = FRR.Item as Projects.Service;
					if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				}
				else if (itemtype == typeof(Projects.Operation))
				{
					Projects.Operation t = FRR.Item as Projects.Operation;
					if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				}
				else if (itemtype == typeof(Projects.OperationParameter))
				{
					Projects.OperationParameter t = FRR.Item as Projects.OperationParameter;
					if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				}
				else if (itemtype == typeof(Projects.Property))
				{
					Projects.Property t = FRR.Item as Projects.Property;
					if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				}
				else if (itemtype == typeof(Projects.Data))
				{
					Projects.Data t = FRR.Item as Projects.Data;
					if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				}
				else if (itemtype == typeof(Projects.DataElement))
				{
					Projects.DataElement t = FRR.Item as Projects.DataElement;
					if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				}
				else if (itemtype == typeof(Projects.Enum))
				{
					Projects.Enum t = FRR.Item as Projects.Enum;
					if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				}
				else if (itemtype == typeof(Projects.EnumElement))
				{
					Projects.EnumElement t = FRR.Item as Projects.EnumElement;
					if (t.Name.IndexOf(FilterItemBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				}
				if (FRR.Project.Name.IndexOf(FilterProjectBox.Text, StringComparison.InvariantCultureIgnoreCase) < 0) continue;
				lbi.Visibility = System.Windows.Visibility.Visible;
			}
		}
	}
}