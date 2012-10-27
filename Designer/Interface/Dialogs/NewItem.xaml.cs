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
using WCFArchitect.Interface;

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

			ActiveProject = Project;
			OpenProjectItem = OpenItemAction;

			EnableAddItem();

#if !WINRT
			if (ActiveProject.HasGenerationFramework(Projects.ProjectGenerationFramework.NET30) || 
				ActiveProject.HasGenerationFramework(Projects.ProjectGenerationFramework.NET35) || 
				ActiveProject.HasGenerationFramework(Projects.ProjectGenerationFramework.NET35Client) || 
				ActiveProject.HasGenerationFramework(Projects.ProjectGenerationFramework.NET40) || 
				ActiveProject.HasGenerationFramework(Projects.ProjectGenerationFramework.NET40Client) || 
				ActiveProject.HasGenerationFramework(Projects.ProjectGenerationFramework.NET45))
			{
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png", "Basic HTTP Binding", "A binding that can be used with any web service that conforms to the WS-I Basic Profile.", 1));
				if (ActiveProject.HasGenerationFramework(Projects.ProjectGenerationFramework.NET45))
				{
					NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png", "Basic HTTPS Binding", "A binding that can be used with any web service that conforms to the WS-I Basic Profile.", 2));
					NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/NetHTTP.png", "Net HTTP Binding", "A binding that can be used with any web service that conforms to the WebSockets Profile.", 3));
					NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/NetHTTP.png", "Net HTTPS Binding", "A binding that can be used with any web service that conforms to the WebSockets Profile.", 4));
				}
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/TCP.png", "TCP Binding", "A binding that provides security and reliability for cross-machine communications.", 5));
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/NamedPipe.png", "Named Pipe Binding", "A binding that provides security and reliability for intra-machine communications.", 6));
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQ.png", "MSMQ Binding", "A queued binding for cross-machine communications.", 7));
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/PeerTCP.png", "Peer TCP Binding", "A secure binding for peer-to-peer applications", 8));
				if (ActiveProject.HasGenerationFramework(Projects.ProjectGenerationFramework.NET35) || ActiveProject.HasGenerationFramework(Projects.ProjectGenerationFramework.NET35Client) || ActiveProject.HasGenerationFramework(Projects.ProjectGenerationFramework.NET40) || ActiveProject.HasGenerationFramework(Projects.ProjectGenerationFramework.NET45))
					NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WebHTTP.png", "Web HTTP Binding", "A binding that provides the use of HTTP requests instead SOAP messags.", 9));
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQIntegration.png", "MSMQ Integration Binding", "A binding for mapping MSMQ messages to WCF messages.", 10));
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSHTTP.png", "WS HTTP Binding", "A binding that supports, security, reliable sessions, and distributed transactions over HTTP.", 11));
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSHTTP.png", "WS 2007 HTTP Binding", "A binding that derives from the WS HTTP Binding and provides updated support for Security, Reliable Sessions, and Transaction Flows.", 12));
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSDualHTTP.png", "WS Dual HTTP Binding", "A binding that supports duplex contracts over HTTP.", 13));
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSFederationHTTP.png", "WS Federation HTTP Binding", "A binding that supports federated security over HTTP.", 14));
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/WSFederationHTTP.png", "WS 2007 Federation HTTP Binding", "A binding that derives from the WS 2007 HTTP Binding and supports federated security.", 15));
			}
			if (ActiveProject.HasGenerationFramework(Projects.ProjectGenerationFramework.SL50) || ActiveProject.HasGenerationFramework(Projects.ProjectGenerationFramework.SL40))
			{
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png", "Basic HTTP Binding", "A binding that can be used with any web service that conforms to the WS-I Basic Profile.", 1));
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/TCP.png", "TCP Binding", "A binding that provides security and reliability for cross-machine communications.", 5));
			}
			if (ActiveProject.HasGenerationFramework(Projects.ProjectGenerationFramework.WIN8))
			{
#endif
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png", "Basic HTTP Binding", "A binding that can be used with any web service that conforms to the WS-I Basic Profile.", 1));
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png", "Basic HTTPS Binding", "A binding that can be used with any web service that conforms to the WS-I Basic Profile.", 2));
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/NetHTTP.png", "Net HTTP Binding", "A binding that can be used with any web service that conforms to the WebSockets Profile.", 3));
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/NetHTTP.png", "Net HTTPS Binding", "A binding that can be used with any web service that conforms to the WebSockets Profile.", 4));
				NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/WCFArchitect;component/Icons/X32/TCP.png", "TCP Binding", "A binding that provides security and reliability for cross-machine communications.", 5));
#if !WINRT
			}
#endif
		}

		public void EnableAddItem()
		{
			NewItemAdd.IsEnabled = false;

			if (NewItemTypesList.SelectedItem == null) return;
			var NIT = NewItemTypesList.SelectedItem as NewItemType;
			if (NIT == null) return;
			if (NewItemBindingTypesList.SelectedItem == null && NIT.DataType == 5) return;
			if (NewItemProjectNamespaceList.SelectedItem == null && NewItemProjectNamespaceRoot.IsChecked == false) return;
			if (NewItemName.Text == "") return;

			NewItemAdd.IsEnabled = true;
		}

		private void NewItemTypesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (NewItemTypesList.SelectedItem == null)
			{
				NewItemProjectNamespaces.Visibility = Visibility.Collapsed;
				NewItemBindingTypesList.Visibility = Visibility.Collapsed;
			}

			NewItemAdd.IsEnabled = false;
			NewItemProjectNamespaceList.ItemsSource = null;
			NewItemBindingTypesList.SelectedItem = null;
			NewItemProjectNamespaces.Visibility = Visibility.Collapsed;
			NewItemBindingTypesList.Visibility = Visibility.Collapsed;
			
			IsNamespaceListUpdating = true;
			NewItemProjectNamespaceRoot.IsChecked = true;
			if (NewItemTypesList.SelectedItem == null) return;

			var NIT = NewItemTypesList.SelectedItem as NewItemType;
			if (NIT == null) return;
			if (NIT.DataType == 1 || NIT.DataType == 2 || NIT.DataType == 3 || NIT.DataType == 4)
			{
				NewItemProjectNamespaceList.ItemsSource = ActiveProject.Namespace.Children;
				NewItemProjectNamespaceRoot.IsChecked = true;
				NewItemProjectNamespaceRoot.Content = ActiveProject.Namespace.Name;
				NewItemProjectNamespaces.Visibility = Visibility.Visible;
			}
			if (NIT.DataType == 5)
			{
				NewItemBindingTypesList.Visibility = Visibility.Visible;
				NewItemBindingTypesList.Focus();
			}
			if (NIT.DataType == 6)
			{
				NewItemProjectNamespaceList.ItemsSource = ActiveProject.Namespace.Children;
				NewItemProjectNamespaceRoot.IsChecked = true;
				NewItemProjectNamespaceRoot.Content = ActiveProject.Namespace.Name;
				NewItemProjectNamespaces.Visibility = Visibility.Visible;
			}
			IsNamespaceListUpdating = false;
		}

		private void NewItemProjectNamespaceRoot_Checked(object sender, RoutedEventArgs e)
		{
			if (IsNamespaceListUpdating) return;
			NewItemProjectNamespaceList.ItemsSource = null;

			EnableAddItem();
		}

		private void NewItemProjectNamespaceRoot_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (NewItemProjectNamespaceList.SelectedItem != null) NewItemProjectNamespaceRoot.IsChecked = false;
			NewItemName.Focus();

			EnableAddItem();
		}

		private void NewItemBindingTypesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			NewItemProjectNamespaceList.ItemsSource = ActiveProject.Namespace.Children;
			NewItemProjectNamespaceRoot.IsChecked = true;
			NewItemProjectNamespaceRoot.Content = ActiveProject.Namespace.Name;
			NewItemProjectNamespaces.Visibility = Visibility.Visible;

			EnableAddItem();
		}

		private void NewItemName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				NewItemAdd_Click(null, null);
		}

		private void NewItemName_TextChanged(object sender, TextChangedEventArgs e)
		{
			EnableAddItem();
		}

		private void NewItemAdd_Click(object sender, RoutedEventArgs e)
		{
			if (NewItemTypesList.SelectedItem == null) return;
			Globals.IsLoading = true;

			try
			{
				var NIT = NewItemTypesList.SelectedItem as NewItemType;
				if (NIT == null) return;

				if (NIT.DataType == 1)
				{
					var NIN = NewItemProjectNamespaceList.SelectedItem as Projects.Namespace ?? ActiveProject.Namespace;
					var NI = new Projects.Namespace(NewItemName.Text, NIN, ActiveProject);
					NIN.Children.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
				else if (NIT.DataType == 2)
				{
					var NIN = NewItemProjectNamespaceList.SelectedItem as Projects.Namespace ?? ActiveProject.Namespace;
					var NI = new Projects.Service(NewItemName.Text, NIN);
					NIN.Services.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
				else if (NIT.DataType == 3)
				{
					var NIN = NewItemProjectNamespaceList.SelectedItem as Projects.Namespace ?? ActiveProject.Namespace;
					var NI = new Projects.Data(NewItemName.Text, NIN);
					NIN.Data.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
				else if (NIT.DataType == 4)
				{
					var NIN = NewItemProjectNamespaceList.SelectedItem as Projects.Namespace ?? ActiveProject.Namespace;
					var NI = new Projects.Enum(NewItemName.Text, NIN);
					NIN.Enums.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
				else if (NIT.DataType == 5)
				{
					var NBT = NewItemBindingTypesList.SelectedItem as NewItemType;
					if (NBT == null) return;
					var NIN = NewItemProjectNamespaceList.SelectedItem as Projects.Namespace ?? ActiveProject.Namespace;

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
					var NIN = NewItemProjectNamespaceList.SelectedItem as Projects.Namespace ?? ActiveProject.Namespace;
					var NI = new Projects.Host(NewItemName.Text, NIN);
					NIN.Hosts.Add(NI);
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
			Prospective.Controls.Dialogs.DialogService.CloseActiveMessageBox();
		}
	}
}
