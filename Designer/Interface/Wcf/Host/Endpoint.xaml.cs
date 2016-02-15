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
using EllipticBit.Controls.WPF.Dialogs;
using NETPath.Projects.Helpers;
using NETPath.Projects.Wcf;

namespace NETPath.Interface.Wcf.Host
{
	internal partial class Endpoint : Grid
	{
		public WcfHostEndpoint Data { get { return (WcfHostEndpoint)GetValue(DataProperty); } set { SetValue(DataProperty, value); } }
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(WcfHostEndpoint), typeof(Endpoint));

		public Endpoint()
		{
			InitializeComponent();
			DataContext = this;
		}

		public Endpoint(WcfHostEndpoint Data)
		{
			this.Data = Data;

			InitializeComponent();

			EndpointBinding.ItemsSource = Globals.GetBindings();
			EndpointBinding.SelectedItem = Data.Binding;

			AddressHeadersList.ItemsSource = Data.ClientAddressHeaders;
			DataContext = this;
		}

		private void Name_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.Name = RegExs.ReplaceSpaces.Replace(DisplayName.Text, "");
		}

		private void Name_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchCodeName.IsMatch(DisplayName.Text);
		}

		private void EndpointBinding_SelectionChanged(object Sender, SelectionChangedEventArgs E)
		{
			Data.Binding = EndpointBinding.SelectedValue as WcfBinding;
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
			Data.ClientAddressHeaders.Add(new WcfHostEndpointAddressHeader(AddressHeaderName.Text, AddressHeader.Text));
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
			var OP = lbi.Content as WcfHostEndpointAddressHeader;
			if (OP == null) return;

			DialogService.ShowMessageDialog("NETPath", "Delete Endpoint Address Header?", "Are you sure you want to delete the address header '" + OP.Name + "'?", new DialogAction("Yes", () => Data.ClientAddressHeaders.Remove(OP), true), new DialogAction("No", false, true));
		}

		private bool IsAddressHeaderValid()
		{
			if (!RegExs.MatchHttpUri.IsMatch(AddressHeader.Text)) return false;
			if (AddressHeader.Text == "") return false;
			if (AddressHeaderName.Text == "") return false;
			return true;
		}
	}
}