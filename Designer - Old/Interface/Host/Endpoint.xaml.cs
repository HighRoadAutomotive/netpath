using System;
using System.Collections.Generic;
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

namespace WCFArchitect.Interface.Project.Host
{
	internal partial class Endpoint : Grid
	{
		public Projects.HostEndpoint Data { get; set; }

		public Endpoint()
		{
			InitializeComponent();
		}

		public Endpoint(Projects.HostEndpoint Data)
		{
			this.Data = Data;

			InitializeComponent();

			if (Data.Binding != null)
				Binding.Text = Data.Binding.Name;
			ServerUseHTTPS.IsEnabled = false;
			if (Data.Binding != null)
				if (Data.Binding.GetType() == typeof(Projects.ServiceBindingBasicHTTP) || Data.Binding.GetType() == typeof(Projects.ServiceBindingWebHTTP) || Data.Binding.GetType() == typeof(Projects.ServiceBindingWSHTTP) || Data.Binding.GetType() == typeof(Projects.ServiceBindingWS2007HTTP) || Data.Binding.GetType() == typeof(Projects.ServiceBindingWSDualHTTP) || Data.Binding.GetType() == typeof(Projects.ServiceBindingWSFederationHTTP) || Data.Binding.GetType() == typeof(Projects.ServiceBindingWS2007FederationHTTP))
					ServerUseHTTPS.IsEnabled = true;

			AddressHeadersList.ItemsSource = Data.ClientAddressHeaders;
		}

		private void CodeName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.CodeName = Helpers.RegExs.ReplaceSpaces.Replace(CodeName.Text, "");
		}

		private void CodeName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(CodeName.Text);
		}

		private void SelectBinding_Click(object sender, RoutedEventArgs e)
		{
			Bindings.SelectBinding SB = new Bindings.SelectBinding(Data.Parent.Parent);
			if (SB.ShowDialog() == true)
			{
				if (SB.SelectedBinding == null) return;
				Data.Binding = SB.SelectedBinding;
				Binding.Text = Data.Binding.Name;
			}
			else
				return;
			
			ServerUseHTTPS.IsEnabled = false;
			if (Data.Binding.GetType() == typeof(Projects.ServiceBindingBasicHTTP) || Data.Binding.GetType() == typeof(Projects.ServiceBindingWebHTTP) || Data.Binding.GetType() == typeof(Projects.ServiceBindingWSHTTP) || Data.Binding.GetType() == typeof(Projects.ServiceBindingWS2007HTTP) || Data.Binding.GetType() == typeof(Projects.ServiceBindingWSDualHTTP) || Data.Binding.GetType() == typeof(Projects.ServiceBindingWSFederationHTTP) || Data.Binding.GetType() == typeof(Projects.ServiceBindingWS2007FederationHTTP))
				ServerUseHTTPS.IsEnabled = true;
		}

		private void ServerUseHTTPS_Checked(object sender, RoutedEventArgs e)
		{
			ServerUseHTTPS.Content = "Yes";
		}

		private void ServerUseHTTPS_Unchecked(object sender, RoutedEventArgs e)
		{
			ServerUseHTTPS.Content = "No";
		}

		private void ClientIdentityType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			ClientIdentityData.IsEnabled = true;
			if (ClientIdentityType.SelectedIndex == 3 || ClientIdentityType.SelectedIndex == 6)
				ClientIdentityData.IsEnabled = false;
		}

		private void AddAddressHeader_Click(object sender, RoutedEventArgs e)
		{
			if (IsAddressHeaderValid() == false) return;
			Data.ClientAddressHeaders.Add(new Projects.HostEndpointAddressHeader(AddressHeaderName.Text, AddressHeader.Text));
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

		private void AddressHeadersList_KeyDown(object sender, KeyEventArgs e)
		{
			if (AddressHeadersList.SelectedItem == null) return;

			if (e.Key == Key.Delete)
			{
				Projects.HostEndpointAddressHeader HEAH = (Projects.HostEndpointAddressHeader)AddressHeadersList.SelectedItem;
				if(Prospective.Controls.MessageBox.Show("Are you sure you want to delete the address header for '" + HEAH.Name + "'?", "Delete Host Endpoint Address Header", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
					Data.ClientAddressHeaders.Remove(HEAH);
			}
		}

		private bool IsAddressHeaderValid()
		{
			if (Helpers.RegExs.MatchHTTPURI.IsMatch(AddressHeader.Text) == false) return false;
			if (AddressHeader.Text == "") return false;
			if (AddressHeaderName.Text == "") return false;
			return true;
		}

		private void ServerPort_ValueChanged(object sender, C1.WPF.PropertyChangedEventArgs<double> e)
		{
			if (IsLoaded == false) return;
			Data.ServerPort = Convert.ToInt32(e.NewValue);
		}
	}
}