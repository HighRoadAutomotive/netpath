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
using EllipticBit.Controls.WPF;
using EllipticBit.Controls.WPF.Dialogs;
using NETPath.Projects;
using NETPath.Projects.Helpers;
using NETPath.Projects.Wcf;

namespace NETPath.Interface.Wcf.Bindings
{
	internal partial class WS2007FederationHTTP : Grid
	{
		public WcfBindingWS2007FederationHTTP Binding { get { return (WcfBindingWS2007FederationHTTP)GetValue(BindingProperty); } set { SetValue(BindingProperty, value); } }
		public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding", typeof(WcfBindingWS2007FederationHTTP), typeof(WS2007FederationHTTP));

		public WS2007FederationHTTP()
		{
			InitializeComponent();
		}

		public WS2007FederationHTTP(WcfBindingWS2007FederationHTTP Binding)
		{
			this.Binding = Binding;

			InitializeComponent();

			IssuerBinding.ItemsSource = Globals.GetBindings();
			IssuerBinding.SelectedItem = Binding.Security.MessageIssuerBinding;
		}

		private void Namespace_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Binding.Namespace = RegExs.ReplaceSpaces.Replace(Namespace.Text, @"");
		}

		private void Namespace_Validate(object sender, ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchHTTPURI.IsMatch(Namespace.Text);
		}

		private void ProxyAddress_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Binding.ProxyAddress = RegExs.ReplaceSpaces.Replace(ProxyAddress.Text, @"");
		}

		private void ProxyAddress_Validate(object sender, ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchHTTPURI.IsMatch(ProxyAddress.Text);
		}

		private void MaxBufferPoolSize_Validate(object sender, ValidateEventArgs e)
		{
			e.IsValid = true;
			try { Convert.ToInt64(MaxBufferPoolSize.Text); }
			catch (Exception) { e.IsValid = false; }
		}

		private void MaxReceivedMessageSize_Validate(object sender, ValidateEventArgs e)
		{
			e.IsValid = true;
			try { Convert.ToInt64(MaxReceivedMessageSize.Text); }
			catch (Exception) { e.IsValid = false; }
		}

		private void IssuerBinding_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Binding.Security.MessageIssuerBinding = IssuerBinding.SelectedValue as WcfBinding;
		}
	}
}