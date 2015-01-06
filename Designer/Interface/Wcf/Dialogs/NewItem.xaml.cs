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
using EllipticBit.Controls.WPF.Dialogs;
using NETPath.Interface;
using NETPath.Projects.Wcf;

namespace NETPath.Interface.Wcf.Dialogs
{
	public partial class NewItem : Grid, IDialogContent
	{
		public System.Collections.ObjectModel.ObservableCollection<DialogAction> Actions { get; set; }
		public ContentDialog Host { get; set; }
		public void SetFocus()
		{
			NewItemTypesList.Focus();
		}

		private bool IsNamespaceListUpdating { get; set; }
		public WcfProject ActiveProject { get; private set; }
		private Action<Projects.OpenableDocument> OpenProjectItem { get; set; }

		public NewItem(WcfProject Project, Action<Projects.OpenableDocument> OpenItemAction)
		{
			InitializeComponent();

			ActiveProject = Project;
			OpenProjectItem = OpenItemAction;

			EnableAddItem();

			NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/NETPath;component/Icons/X32/BasicHTTP.png", "Basic HTTP Binding", "A binding that can be used with any web service that conforms to the WS-I Basic Profile.", 1));
			NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/NETPath;component/Icons/X32/NetHTTP.png", "Net HTTP Binding", "A binding that can be used with any web service that conforms to the WebSockets Profile.", 3));
			NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/NETPath;component/Icons/X32/BasicHTTP.png", "Basic HTTPS Binding", "A binding that can be used with any web service that conforms to the WS-I Basic Profile.", 2));
			NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/NETPath;component/Icons/X32/NetHTTP.png", "Net HTTPS Binding", "A binding that can be used with any web service that conforms to the WebSockets Profile.", 4));
			NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/NETPath;component/Icons/X32/TCP.png", "TCP Binding", "A binding that provides security and reliability for cross-machine communications.", 5));
			NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/NETPath;component/Icons/X32/UDP.png", "UDP Binding", "A binding the user datagram protocol.", 16));
			NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/NETPath;component/Icons/X32/NamedPipe.png", "Named Pipe Binding", "A binding that provides security and reliability for intra-machine communications.", 6));
			NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/NETPath;component/Icons/X32/MSMQ.png", "MSMQ Binding", "A queued binding for cross-machine communications.", 7));
			NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/NETPath;component/Icons/X32/PeerTCP.png", "Peer TCP Binding", "A secure binding for peer-to-peer applications", 8));
			NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/NETPath;component/Icons/X32/MSMQIntegration.png", "MSMQ Integration Binding", "A binding for mapping MSMQ messages to WCF messages.", 10));
			NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/NETPath;component/Icons/X32/WSHTTP.png", "WS HTTP Binding", "A binding that supports, security, reliable sessions, and distributed transactions over HTTP.", 11));
			NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/NETPath;component/Icons/X32/WSHTTP.png", "WS 2007 HTTP Binding", "A binding that derives from the WS HTTP Binding and provides updated support for Security, Reliable Sessions, and Transaction Flows.", 12));
			NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/NETPath;component/Icons/X32/WSDualHTTP.png", "WS Dual HTTP Binding", "A binding that supports duplex contracts over HTTP.", 13));
			NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/NETPath;component/Icons/X32/WSFederationHTTP.png", "WS Federation HTTP Binding", "A binding that supports federated security over HTTP.", 14));
			NewItemBindingTypesList.Items.Add(new NewItemType("pack://application:,,,/NETPath;component/Icons/X32/WSFederationHTTP.png", "WS 2007 Federation HTTP Binding", "A binding that derives from the WS 2007 HTTP Binding and supports federated security.", 15));
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
			if (NIT.DataType == 1 || NIT.DataType == 2 || NIT.DataType == 3 || NIT.DataType == 4 || NIT.DataType == 7)
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
					var NIN = NewItemProjectNamespaceList.SelectedItem as WcfNamespace ?? ActiveProject.Namespace;
					var NI = new WcfNamespace(NewItemName.Text, NIN, ActiveProject);
					NIN.Children.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
				else if (NIT.DataType == 2)
				{
					var NIN = NewItemProjectNamespaceList.SelectedItem as WcfNamespace ?? ActiveProject.Namespace;
					var NI = new WcfService(NewItemName.Text, NIN);
					foreach (var t in ActiveProject.ServerGenerationTargets) t.TargetTypes.Add(NI);
					foreach (var t in ActiveProject.ClientGenerationTargets) t.TargetTypes.Add(NI);
					NIN.Services.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
				else if (NIT.DataType == 3)
				{
					var NIN = NewItemProjectNamespaceList.SelectedItem as WcfNamespace ?? ActiveProject.Namespace;
					var NI = new WcfData(NewItemName.Text, NIN);
					foreach (var t in ActiveProject.ServerGenerationTargets) t.TargetTypes.Add(NI);
					foreach (var t in ActiveProject.ClientGenerationTargets) t.TargetTypes.Add(NI);
					NIN.Data.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
				else if (NIT.DataType == 4)
				{
					var NIN = NewItemProjectNamespaceList.SelectedItem as WcfNamespace ?? ActiveProject.Namespace;
					var NI = new Projects.Enum(NewItemName.Text, NIN);
					foreach (var t in ActiveProject.ServerGenerationTargets) t.TargetTypes.Add(NI);
					foreach (var t in ActiveProject.ClientGenerationTargets) t.TargetTypes.Add(NI);
					NIN.Enums.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
				else if (NIT.DataType == 5)
				{
					var NBT = NewItemBindingTypesList.SelectedItem as NewItemType;
					if (NBT == null) return;
					var NIN = NewItemProjectNamespaceList.SelectedItem as WcfNamespace ?? ActiveProject.Namespace;

					WcfBinding NI = null;
					if (NBT.DataType == 1) NI = new WcfBindingBasicHTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 2) NI = new WcfBindingBasicHTTPS(NewItemName.Text, NIN);
					if (NBT.DataType == 3) NI = new WcfBindingNetHTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 4) NI = new WcfBindingNetHTTPS(NewItemName.Text, NIN);
					if (NBT.DataType == 5) NI = new WcfBindingTCP(NewItemName.Text, NIN);
					if (NBT.DataType == 6) NI = new WcfBindingNamedPipe(NewItemName.Text, NIN);
					if (NBT.DataType == 7) NI = new WcfBindingMSMQ(NewItemName.Text, NIN);
					if (NBT.DataType == 8) NI = new WcfBindingPeerTCP(NewItemName.Text, NIN);
					if (NBT.DataType == 10) NI = new WcfBindingMSMQIntegration(NewItemName.Text, NIN);
					if (NBT.DataType == 11) NI = new WcfBindingWSHTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 12) NI = new WcfBindingWS2007HTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 13) NI = new WcfBindingWSDualHTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 14) NI = new WcfBindingWSFederationHTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 15) NI = new WcfBindingWS2007FederationHTTP(NewItemName.Text, NIN);
					if (NBT.DataType == 16) NI = new WcfBindingUDP(NewItemName.Text, NIN);

					foreach (var t in ActiveProject.ServerGenerationTargets) t.TargetTypes.Add(NI);
					foreach (var t in ActiveProject.ClientGenerationTargets) t.TargetTypes.Add(NI);

					NIN.Bindings.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
				else if (NIT.DataType == 6)
				{
					var NIN = NewItemProjectNamespaceList.SelectedItem as WcfNamespace ?? ActiveProject.Namespace;
					var NI = new WcfHost(NewItemName.Text, NIN);
					foreach (var t in ActiveProject.ServerGenerationTargets) t.TargetTypes.Add(NI);
					foreach (var t in ActiveProject.ClientGenerationTargets) t.TargetTypes.Add(NI);
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
			EllipticBit.Controls.WPF.Dialogs.DialogService.CloseActiveMessageBox();
		}
	}
}
