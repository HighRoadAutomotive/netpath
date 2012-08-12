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
using WCFArchitect.Interface.Project;

namespace WCFArchitect.Interface.Dialogs
{
	public partial class NewItem : Grid
	{
		private bool IsNamespaceListUpdating { get; set; }

		public Projects.Project ActiveProject { get; private set; }

		private Action<Projects.OpenableDocument> OpenProjectItem { get; set; }

		public NewItem(Projects.Project Project, Action<Projects.OpenableDocument> OpenItemAction)
		{
			InitializeComponent();

			this.ActiveProject = Project;
			this.OpenProjectItem = OpenItemAction;
#if !WINRT
			if(ActiveProject.GetType() == typeof(Projects.ProjectNET))
			{
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png", "Basic HTTP Binding", "A binding that can be used with any web service that conforms to the WS-I Basic Profile.", 1));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png", "Basic HTTPS Binding", "A binding that can be used with any web service that conforms to the WS-I Basic Profile.", 2));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/NetHTTP.png", "Net HTTP Binding", "A binding that can be used with any web service that conforms to the WebSockets Profile.", 3));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/NetHTTP.png", "Net HTTPS Binding", "A binding that can be used with any web service that conforms to the WebSockets Profile.", 4));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/TCP.png", "TCP Binding", "A binding that provides security and reliability for cross-machine communications.", 5));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/NamedPipe.png", "Named Pipe Binding", "A binding that provides security and reliability for intra-machine communications.", 6));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQ.png", "MSMQ Binding", "A queued binding for cross-machine communications.", 7));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/PeerTCP.png", "Peer TCP Binding", "A secure binding for peer-to-peer applications", 8));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WebHTTP.png", "Web HTTP Binding", "A binding that provides the use of HTTP requests instead SOAP messags.", 9));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQIntegration.png", "MSMQ Integration Binding", "A binding for mapping MSMQ messages to WCF messages.", 10));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSHTTP.png", "WS HTTP Binding", "A binding that supports, security, reliable sessions, and distributed transactions over HTTP.", 11));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSHTTP.png", "WS 2007 HTTP Binding", "A binding that derives from the WS HTTP Binding and provides updated support for Security, Reliable Sessions, and Transaction Flows.", 12));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSDualHTTP.png", "WS Dual HTTP Binding", "A binding that supports duplex contracts over HTTP.", 13));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSFederationHTTP.png", "WS Federation HTTP Binding", "A binding that supports federated security over HTTP.", 14));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSFederationHTTP.png", "WS 2007 Federation HTTP Binding", "A binding that derives from the WS 2007 HTTP Binding and supports federated security.", 15));

				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png", "Basic HTTP Security", "Security configuration for the Basic HTTP and Net HTTP Bindings.", 1));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png", "Basic HTTPS Security", "Security configuration for the Basic HTTPS and Net HTTPS Bindings.", 2));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/TCP.png", "TCP Security", "Security configuration for a TCP Binding.", 3));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/NamedPipe.png", "Named Pipe Security", "Security configuration for a Named Pipe Binding.", 4));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQ.png", "MSMQ Security", "Security configuration for an MSMQ Binding.", 5));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/PeerTCP.png", "Peer TCP Security", "Security configuration for a Peer TCP Binding", 6));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WebHTTP.png", "Web HTTP Security", "Security configuration for a Web HTTP Binding.", 7));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQIntegration.png", "MSMQ Integration Security", "Security configuration for an MSMQ Integration Binding", 8));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSHTTP.png", "WS HTTP Security", "Security configuration for a WS HTTP or WS 2007 HTTP Binding.", 9));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSDualHTTP.png", "WS Dual Security", "Security configuration for a WS Dual Binding.", 10));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSFederationHTTP.png", "WS Federation Security", "Security configuration for a WS Federation or WS 2007 Federation Binding.", 11));
			}
			if (ActiveProject.GetType() == typeof(Projects.ProjectSL))
			{
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png", "Basic HTTP Binding", "A binding that can be used with any web service that conforms to the WS-I Basic Profile.", 1));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/TCP.png", "TCP Binding", "A binding that provides security and reliability for cross-machine communications.", 5));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/NamedPipe.png", "Named Pipe Binding", "A binding that provides security and reliability for intra-machine communications.", 6));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQ.png", "MSMQ Binding", "A queued binding for cross-machine communications.", 7));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/PeerTCP.png", "Peer TCP Binding", "A secure binding for peer-to-peer applications", 8));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WebHTTP.png", "Web HTTP Binding", "A binding that provides the use of HTTP requests instead SOAP messags.", 9));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQIntegration.png", "MSMQ Integration Binding", "A binding for mapping MSMQ messages to WCF messages.", 10));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSHTTP.png", "WS HTTP Binding", "A binding that supports, security, reliable sessions, and distributed transactions over HTTP.", 11));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSHTTP.png", "WS 2007 HTTP Binding", "A binding that derives from the WS HTTP Binding and provides updated support for Security, Reliable Sessions, and Transaction Flows.", 12));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSDualHTTP.png", "WS Dual HTTP Binding", "A binding that supports duplex contracts over HTTP.", 13));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSFederationHTTP.png", "WS Federation HTTP Binding", "A binding that supports federated security over HTTP.", 14));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSFederationHTTP.png", "WS 2007 Federation HTTP Binding", "A binding that derives from the WS 2007 HTTP Binding and supports federated security.", 15));

				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png", "Basic HTTP Security", "Security configuration for the Basic HTTP and Net HTTP Bindings.", 1));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/TCP.png", "TCP Security", "Security configuration for a TCP Binding.", 3));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/NamedPipe.png", "Named Pipe Security", "Security configuration for a Named Pipe Binding.", 4));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQ.png", "MSMQ Security", "Security configuration for an MSMQ Binding.", 5));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/PeerTCP.png", "Peer TCP Security", "Security configuration for a Peer TCP Binding", 6));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WebHTTP.png", "Web HTTP Security", "Security configuration for a Web HTTP Binding.", 7));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQIntegration.png", "MSMQ Integration Security", "Security configuration for an MSMQ Integration Binding", 8));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSHTTP.png", "WS HTTP Security", "Security configuration for a WS HTTP or WS 2007 HTTP Binding.", 9));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSDualHTTP.png", "WS Dual Security", "Security configuration for a WS Dual Binding.", 10));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSFederationHTTP.png", "WS Federation Security", "Security configuration for a WS Federation or WS 2007 Federation Binding.", 11));
			}
			if (ActiveProject.GetType() == typeof(Projects.ProjectRT))
			{
#endif
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png", "Basic HTTP Binding", "A binding that can be used with any web service that conforms to the WS-I Basic Profile.", 1));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png", "Basic HTTPS Binding", "A binding that can be used with any web service that conforms to the WS-I Basic Profile.", 2));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/NetHTTP.png", "Net HTTP Binding", "A binding that can be used with any web service that conforms to the WebSockets Profile.", 3));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/NetHTTP.png", "Net HTTPS Binding", "A binding that can be used with any web service that conforms to the WebSockets Profile.", 4));
				NewItemBindingTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/TCP.png", "TCP Binding", "A binding that provides security and reliability for cross-machine communications.", 5));

				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png", "Basic HTTP Security", "Security configuration for the Basic HTTP and Net HTTP Bindings.", 1));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png", "Basic HTTPS Security", "Security configuration for the Basic HTTPS and Net HTTPS Bindings.", 2));
				NewItemSecurityTypesList.Items.Add(new Project.NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/TCP.png", "TCP Security", "Security configuration for a TCP Binding.", 3));
#if !WINRT
			}
#endif
		}

		private void NewItemTypesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (NewItemTypesList.SelectedItem == null)
			{
				NewItemProjectNamespaces.Visibility = System.Windows.Visibility.Collapsed;
				NewItemBindingTypesList.Visibility = System.Windows.Visibility.Collapsed;
				NewItemSecurityTypesList.Visibility = System.Windows.Visibility.Collapsed;
			}

			NewItemAdd.IsEnabled = false;
			NewItemProjectNamespaceList.ItemsSource = null;
			NewItemBindingTypesList.SelectedItem = null;
			NewItemSecurityTypesList.SelectedItem = null;
			NewItemProjectNamespaces.Visibility = System.Windows.Visibility.Collapsed;
			NewItemBindingTypesList.Visibility = System.Windows.Visibility.Collapsed;
			NewItemSecurityTypesList.Visibility = System.Windows.Visibility.Collapsed;
			
			IsNamespaceListUpdating = true;
			NewItemProjectNamespaceRoot.IsChecked = true;
			if (NewItemTypesList.SelectedItem == null) return;

			NewItemType NIT = NewItemTypesList.SelectedItem as NewItemType;
			if (NIT.DataType == 1 || NIT.DataType == 2 || NIT.DataType == 3 || NIT.DataType == 4)
			{
				NewItemProjectNamespaceList.ItemsSource = ActiveProject.Namespace.Children;
				NewItemProjectNamespaceRoot.IsChecked = true;
				NewItemProjectNamespaceRoot.Content = ActiveProject.Namespace.Name;
				NewItemProjectNamespaces.Visibility = System.Windows.Visibility.Visible;
			}
			if (NIT.DataType == 5)
			{
				NewItemBindingTypesList.Visibility = System.Windows.Visibility.Visible;
				NewItemBindingTypesList.Focus();
			}
			if (NIT.DataType == 6)
			{
				NewItemSecurityTypesList.Visibility = System.Windows.Visibility.Visible;
				NewItemSecurityTypesList.Focus();
			}
			if (NIT.DataType == 7)
			{
				NewItemName.Focus();
			}
			IsNamespaceListUpdating = false;
		}

		private void NewItemProjectNamespaceRoot_Checked(object sender, RoutedEventArgs e)
		{
			if (IsNamespaceListUpdating == true) return;
			NewItemProjectNamespaceList.ItemsSource = null;
		}

		private void NewItemProjectNamespaceRoot_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (NewItemProjectNamespaceList.SelectedItem != null) NewItemProjectNamespaceRoot.IsChecked = false;
			NewItemName.Focus();
		}

		private void NewItemBindingTypesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			NewItemProjectNamespaceList.ItemsSource = ActiveProject.Namespace.Children;
			NewItemProjectNamespaceRoot.IsChecked = true;
			NewItemProjectNamespaceRoot.Content = ActiveProject.Namespace.Name;
			NewItemProjectNamespaces.Visibility = System.Windows.Visibility.Visible;
		}

		private void NewItemSecurityTypesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			NewItemProjectNamespaceList.ItemsSource = ActiveProject.Namespace.Children;
			NewItemProjectNamespaceRoot.IsChecked = true;
			NewItemProjectNamespaceRoot.Content = ActiveProject.Namespace.Name;
			NewItemProjectNamespaces.Visibility = System.Windows.Visibility.Visible;
		}

		private void NewItemName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				NewItemAdd_Click(null, null);
		}

		private void NewItemName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (NewItemName.Text == "") NewItemAdd.IsEnabled = false;
			else NewItemAdd.IsEnabled = true;
		}

		private void NewItemAdd_Click(object sender, RoutedEventArgs e)
		{
			if (NewItemTypesList.SelectedItem == null) return;
			Globals.IsLoading = true;

			try
			{
				NewItemType NIT = NewItemTypesList.SelectedItem as NewItemType;

				if (NIT.DataType == 1)
				{
					if (NewItemProjectNamespaceRoot.IsChecked == true)
					{
						Projects.Namespace NI = new Projects.Namespace(NewItemName.Text, ActiveProject.Namespace, ActiveProject);
						NI.IsTreeExpanded = true;
						if (NewItemProjectNamespaceList.SelectedItem == null) ActiveProject.Namespace.Children.Add(NI);
						else ActiveProject.Namespace.Children.Add(NI);
						Globals.IsLoading = false;
						OpenProjectItem(NI);
					}
					else
					{
						Projects.Namespace NIN = NewItemProjectNamespaceList.SelectedItem as Projects.Namespace;
						Projects.Namespace NI = new Projects.Namespace(NewItemName.Text, NIN, ActiveProject);
						NI.IsTreeExpanded = true;
						if (NewItemProjectNamespaceList.SelectedItem == null) ActiveProject.Namespace.Children.Add(NI);
						else NIN.Children.Add(NI);
						Globals.IsLoading = false;
						OpenProjectItem(NI);
					}
				}
				else if (NIT.DataType == 2)
				{
					if (NewItemProjectNamespaceRoot.IsChecked == true)
					{
						Projects.Service NI = new Projects.Service(NewItemName.Text, ActiveProject.Namespace);
						NI.IsTreeExpanded = true;
						ActiveProject.Namespace.Services.Add(NI);
						Globals.IsLoading = false;
						OpenProjectItem(NI);
					}
					else
					{
						Projects.Namespace NIN = NewItemProjectNamespaceList.SelectedItem as Projects.Namespace;
						if (NIN == null)
						{
							if (Prospective.Controls.MessageBox.Show("You must select a select a Namespace from the list or use the Root Namespace to create a new Service. Would you like to use the Root Namespace?", "No Namespace Selected", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
								NewItemProjectNamespaceRoot.IsChecked = true;
							return;
						}

						Projects.Service NI = new Projects.Service(NewItemName.Text, NIN);
						NI.IsTreeExpanded = true;
						NIN.Services.Add(NI);
						Globals.IsLoading = false;
						OpenProjectItem(NI);
					}
				}
				else if (NIT.DataType == 3)
				{
					if (NewItemProjectNamespaceRoot.IsChecked == true)
					{
						Projects.Data NI = new Projects.Data(NewItemName.Text, ActiveProject.Namespace);
						NI.IsTreeExpanded = true;
						ActiveProject.Namespace.Data.Add(NI);
						Globals.IsLoading = false;
						OpenProjectItem(NI);
					}
					else
					{
						Projects.Namespace NIN = NewItemProjectNamespaceList.SelectedItem as Projects.Namespace;
						if (NIN == null)
						{
							if (Prospective.Controls.MessageBox.Show("You must select a select a Namespace from the list or use the Root Namespace to create a new Data Object. Would you like to use the Root Namespace?", "No Namespace Selected", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
								NewItemProjectNamespaceRoot.IsChecked = true;
							return;
						}

						Projects.Data NI = new Projects.Data(NewItemName.Text, NIN);
						NI.IsTreeExpanded = true;
						NIN.Data.Add(NI);
						Globals.IsLoading = false;
						OpenProjectItem(NI);
					}
				}
				else if (NIT.DataType == 4)
				{
					if (NewItemProjectNamespaceRoot.IsChecked == true)
					{
						Projects.Enum NI = new Projects.Enum(NewItemName.Text, ActiveProject.Namespace);
						ActiveProject.Namespace.Enums.Add(NI);
						NI.IsTreeExpanded = true;
						Globals.IsLoading = false;
						OpenProjectItem(NI);
					}
					else
					{
						Projects.Namespace NIN = NewItemProjectNamespaceList.SelectedItem as Projects.Namespace;
						if (NIN == null)
						{
							if (Prospective.Controls.MessageBox.Show("You must select a select a Namespace from the list or use the Root Namespace to create a new Enum. Would you like to use the Root Namespace?", "No Namespace Selected", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
								NewItemProjectNamespaceRoot.IsChecked = true;
							return;
						}

						Projects.Enum NI = new Projects.Enum(NewItemName.Text, NIN);
						NI.IsTreeExpanded = true;
						NIN.Enums.Add(NI);
						Globals.IsLoading = false;
						OpenProjectItem(NI);
					}
				}
				else if (NIT.DataType == 5)
				{
					NewItemType NBT = NewItemBindingTypesList.SelectedItem as NewItemType;
					if (NBT == null)
					{
						Prospective.Controls.MessageBox.Show("You must select a Binding Type for this Binding!", "No Binding Type Selected", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}
					Projects.Namespace NIN = NewItemProjectNamespaceList.SelectedItem as Projects.Namespace;
					if (NewItemProjectNamespaceRoot.IsChecked == true) NIN = ActiveProject.Namespace;
					if (NIN == null)
					{
						if (Prospective.Controls.MessageBox.Show("You must select a select a Namespace from the list or use the Root Namespace to create a new Enum. Would you like to use the Root Namespace?", "No Namespace Selected", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
							NewItemProjectNamespaceRoot.IsChecked = true;
						return;
					}

					Projects.ServiceBinding NI = null;
					if (NBT.DataType == 1) NI = new Projects.ServiceBindingBasicHTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 2) NI = new Projects.ServiceBindingBasicHTTPS(NewItemName.Text, NIN);
					if (NBT.DataType == 3) NI = new Projects.ServiceBindingNetHTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 4) NI = new Projects.ServiceBindingNetHTTPS(NewItemName.Text, NIN);
					if (NBT.DataType == 5) NI = new Projects.ServiceBindingTCP(NewItemName.Text, NIN);
					if (NBT.DataType == 6) NI = new Projects.ServiceBindingNamedPipe(NewItemName.Text, NIN);
					if (NBT.DataType == 7) NI = new Projects.ServiceBindingMSMQ(NewItemName.Text, NIN);
					if (NBT.DataType == 8) NI = new Projects.ServiceBindingPeerTCP(NewItemName.Text, NIN);
					if (NBT.DataType == 9) NI = new Projects.ServiceBindingWebHTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 10) NI = new Projects.ServiceBindingMSMQIntegration(NewItemName.Text, NIN);
					if (NBT.DataType == 11) NI = new Projects.ServiceBindingWSHTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 12) NI = new Projects.ServiceBindingWS2007HTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 13) NI = new Projects.ServiceBindingWSDualHTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 14) NI = new Projects.ServiceBindingWSFederationHTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 15) NI = new Projects.ServiceBindingWS2007FederationHTTP(NewItemName.Text, NIN);

					NIN.Bindings.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
				else if (NIT.DataType == 6)
				{
					NewItemType NBT = NewItemSecurityTypesList.SelectedItem as NewItemType;
					if (NBT == null)
					{
						Prospective.Controls.MessageBox.Show("You must select a Security Type for this Binding!", "No Security Type Selected", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}
					Projects.Namespace NIN = NewItemProjectNamespaceList.SelectedItem as Projects.Namespace;
					if (NewItemProjectNamespaceRoot.IsChecked == true) NIN = ActiveProject.Namespace;
					if (NIN == null)
					{
						if (Prospective.Controls.MessageBox.Show("You must select a select a Namespace from the list or use the Root Namespace to create a new Enum. Would you like to use the Root Namespace?", "No Namespace Selected", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
							NewItemProjectNamespaceRoot.IsChecked = true;
						return;
					}

					Projects.BindingSecurity NI = null;
					if (NBT.DataType == 1) NI = new Projects.BindingSecurityBasicHTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 2) NI = new Projects.BindingSecurityBasicHTTPS(NewItemName.Text, NIN);
					if (NBT.DataType == 3) NI = new Projects.BindingSecurityTCP(NewItemName.Text, NIN);
					if (NBT.DataType == 4) NI = new Projects.BindingSecurityNamedPipe(NewItemName.Text, NIN);
					if (NBT.DataType == 5) NI = new Projects.BindingSecurityMSMQ(NewItemName.Text, NIN);
					if (NBT.DataType == 6) NI = new Projects.BindingSecurityPeerTCP(NewItemName.Text, NIN);
					if (NBT.DataType == 7) NI = new Projects.BindingSecurityWebHTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 8) NI = new Projects.BindingSecurityMSMQIntegration(NewItemName.Text, NIN);
					if (NBT.DataType == 9) NI = new Projects.BindingSecurityWSHTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 10) NI = new Projects.BindingSecurityWSDualHTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 11) NI = new Projects.BindingSecurityWSFederationHTTP(NewItemName.Text, NIN);

					NIN.Security.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
				else if (NIT.DataType == 7)
				{
					Projects.Namespace NIP = null;
					if (NewItemProjectNamespaceRoot.IsChecked == true) NIP = ActiveProject.Namespace;
					else NIP = NewItemProjectNamespaceList.SelectedItem as Projects.Namespace;
					
					Projects.Host NI = new Projects.Host(NewItemName.Text, NIP);
					NIP.Hosts.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
			}
			finally
			{
				Globals.IsLoading = false;
			}
			NewItemCancel_Click(null, null);
		}

		private void NewItemCancel_Click(object sender, RoutedEventArgs e)
		{
			Globals.MainScreen.CloseActiveMessageBox();
		}
	}
}
