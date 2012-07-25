﻿using System;
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
	internal partial class WSFederationHTTP : Grid
	{
		public Projects.ServiceBindingWSFederationHTTP Binding { get; set; }

		public WSFederationHTTP()
		{
			InitializeComponent();
		}

		public WSFederationHTTP(Projects.ServiceBindingWSFederationHTTP Binding)
		{
			this.Binding = Binding;

			InitializeComponent();

			MaxBufferPoolSize.Text = Binding.MaxBufferPoolSize.ValueRoundedBinary;
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

		private void TransactionFlow_Checked(object sender, RoutedEventArgs e)
		{
			TransactionFlow.Content = "Yes";
		}

		private void TransactionFlow_Unchecked(object sender, RoutedEventArgs e)
		{
			TransactionFlow.Content = "No";
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

		private void ReliableSessionsEnabled_Checked(object sender, RoutedEventArgs e)
		{
			ReliableSessionsEnabled.Content = "Yes";
		}

		private void ReliableSessionsEnabled_Unchecked(object sender, RoutedEventArgs e)
		{
			ReliableSessionsEnabled.Content = "No";
		}

		private void ReliableSessionsOrdered_Checked(object sender, RoutedEventArgs e)
		{
			ReliableSessionsOrdered.Content = "Yes";
		}

		private void ReliableSessionsOrdered_Unchecked(object sender, RoutedEventArgs e)
		{
			ReliableSessionsOrdered.Content = "No";
		}

		private void SelectSecurity_Click(object sender, RoutedEventArgs e)
		{
			Interface.Project.Security.SelectSecurity SS = new Security.SelectSecurity(Binding.Parent, typeof(Projects.BindingSecurityWSFederationHTTP));
			if (SS.ShowDialog().Value == true)
			{
				Binding.Security = (Projects.BindingSecurityWSFederationHTTP)SS.SelectedSecurity;
				Security.Text = Binding.Security.Name;
			}
		}

		private void MaxBufferPoolSize_TextChanged(object sender, TextChangedEventArgs e)
		{
			MaxBufferPoolSize.Background = Brushes.White;
			Binding.MaxBufferPoolSize.ValueRoundedBinary = MaxBufferPoolSize.Text;
			if (Binding.MaxBufferPoolSize.IsErrored == true) MaxBufferPoolSize.Background = Brushes.Red;
		}

		private void MaxReceivedMessageSize_TextChanged(object sender, TextChangedEventArgs e)
		{
			MaxReceivedMessageSize.Background = Brushes.White;
			Binding.MaxReceivedMessageSize.ValueRoundedBinary = MaxReceivedMessageSize.Text;
			if (Binding.MaxReceivedMessageSize.IsErrored == true) MaxReceivedMessageSize.Background = Brushes.Red;
		}

		private void PrivacyNoticeVersion_ValueChanged(object sender, C1.WPF.PropertyChangedEventArgs<double> e)
		{
			if (IsLoaded == false) return;
			Binding.PrivacyNoticeVersion = Convert.ToInt32(e.NewValue);
		}
	}
}