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

namespace WCFArchitect.Interface.Project.Bindings
{
	internal partial class WebHTTP : Grid
	{
		public Projects.ServiceBindingWebHTTP Binding { get; set; }

		public WebHTTP()
		{
			InitializeComponent();
		}

		public WebHTTP(Projects.ServiceBindingWebHTTP Binding)
		{
			this.Binding = Binding;

			InitializeComponent();

			MaxBufferPoolSize.Text = Binding.MaxBufferPoolSize.ValueRoundedBinary;
			MaxBufferSize.Text = Binding.MaxBufferSize.ValueRoundedBinary;
			MaxReceivedMessageSize.Text = Binding.MaxReceivedMessageSize.ValueRoundedBinary;

			if (Binding.Security != null)
				Security.Text = Binding.Security.Name;
		}

		private void CodeName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Binding.CodeName = Helpers.RegExs.ReplaceSpaces.Replace(CodeName.Text, @"");
		}

		private void CodeName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(CodeName.Text);
		}

		private void Namespace_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Binding.Namespace = Helpers.RegExs.ReplaceSpaces.Replace(Namespace.Text, @"");
		}

		private void Namespace_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchHTTPURI.IsMatch(Namespace.Text);
		}

		private void EndpointAddress_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Binding.EndpointAddress = Helpers.RegExs.ReplaceSpaces.Replace(EndpointAddress.Text, @"");
		}

		private void EndpointAddress_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchHTTPURI.IsMatch(EndpointAddress.Text);
		}

		private void ListenAddress_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Binding.ListenAddress = Helpers.RegExs.ReplaceSpaces.Replace(ListenAddress.Text, @"");
		}

		private void ListenAddress_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchHTTPURI.IsMatch(ListenAddress.Text);
		}

		private void ProxyAddress_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Binding.ProxyAddress = Helpers.RegExs.ReplaceSpaces.Replace(ProxyAddress.Text, @"");
		}

		private void ProxyAddress_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchHTTPURI.IsMatch(ProxyAddress.Text);
		}

		private void AllowCookies_Checked(object sender, RoutedEventArgs e)
		{
			AllowCookies.Content = "Yes";
		}

		private void AllowCookies_Unchecked(object sender, RoutedEventArgs e)
		{
			AllowCookies.Content = "No";
		}

		private void BypassProxyOnLocal_Checked(object sender, RoutedEventArgs e)
		{
			BypassProxyOnLocal.Content = "Yes";
		}

		private void BypassProxyOnLocal_Unchecked(object sender, RoutedEventArgs e)
		{
			BypassProxyOnLocal.Content = "No";
		}

		private void UseDefaultWebProxy_Checked(object sender, RoutedEventArgs e)
		{
			UseDefaultWebProxy.Content = "Yes";
		}

		private void UseDefaultWebProxy_Unchecked(object sender, RoutedEventArgs e)
		{
			UseDefaultWebProxy.Content = "No";
		}

		private void CrossDomainScriptAccessEnabled_Checked(object sender, RoutedEventArgs e)
		{
			CrossDomainScriptAccessEnabled.Content = "Yes";
		}

		private void CrossDomainScriptAccessEnabled_Unchecked(object sender, RoutedEventArgs e)
		{
			CrossDomainScriptAccessEnabled.Content = "No";
		}

		private void SelectSecurity_Click(object sender, RoutedEventArgs e)
		{
			Interface.Project.Security.SelectSecurity SS = new Security.SelectSecurity(Binding.Parent, typeof(Projects.BindingSecurityWebHTTP));
			if (SS.ShowDialog().Value == true)
			{
				Binding.Security = (Projects.BindingSecurityWebHTTP)SS.SelectedSecurity;
				Security.Text = Binding.Security.Name;
			}
		}

		private void MaxBufferPoolSize_TextChanged(object sender, TextChangedEventArgs e)
		{
			MaxBufferPoolSize.Background = Brushes.White;
			Binding.MaxBufferPoolSize.ValueRoundedBinary = MaxBufferPoolSize.Text;
			if (Binding.MaxBufferPoolSize.IsErrored == true) MaxBufferPoolSize.Background = Brushes.Red;
		}

		private void MaxBufferSize_TextChanged(object sender, TextChangedEventArgs e)
		{
			MaxBufferSize.Background = Brushes.White;
			Binding.MaxBufferSize.ValueRoundedBinary = MaxBufferSize.Text;
			if (Binding.MaxBufferSize.IsErrored == true) MaxBufferSize.Background = Brushes.Red;
		}

		private void MaxReceivedMessageSize_TextChanged(object sender, TextChangedEventArgs e)
		{
			MaxReceivedMessageSize.Background = Brushes.White;
			Binding.MaxReceivedMessageSize.ValueRoundedBinary = MaxReceivedMessageSize.Text;
			if (Binding.MaxReceivedMessageSize.IsErrored == true) MaxReceivedMessageSize.Background = Brushes.Red;
		}
	}
}