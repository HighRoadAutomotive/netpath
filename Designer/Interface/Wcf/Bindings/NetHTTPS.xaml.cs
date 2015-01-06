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
	internal partial class NetHTTPS : Grid
	{
		public WcfBindingNetHTTPS Binding { get { return (WcfBindingNetHTTPS)GetValue(BindingProperty); } set { SetValue(BindingProperty, value); } }
		public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding", typeof(WcfBindingNetHTTPS), typeof(NetHTTPS));

		public NetHTTPS()
		{
			InitializeComponent();
		}

		public NetHTTPS(WcfBindingNetHTTPS Binding)
		{
			this.Binding = Binding;

			InitializeComponent();
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

		private void MaxBufferSize_Validate(object sender, ValidateEventArgs e)
		{
			e.IsValid = true;
			try { Convert.ToInt64(MaxBufferSize.Text); }
			catch (Exception) { e.IsValid = false; }
		}

		private void MaxReceivedMessageSize_Validate(object sender, ValidateEventArgs e)
		{
			e.IsValid = true;
			try { Convert.ToInt64(MaxReceivedMessageSize.Text); }
			catch (Exception) { e.IsValid = false; }
		}

		private void MaxPendingConnections_Validate(object sender, ValidateEventArgs e)
		{
			e.IsValid = true;
			try { Convert.ToInt32(MaxPendingConnections.Text); }
			catch (Exception) { e.IsValid = false; }
		}

		private void ReceiveBufferSize_Validate(object sender, ValidateEventArgs e)
		{
			e.IsValid = true;
			try { Convert.ToInt32(ReceiveBufferSize.Text); }
			catch (Exception) { e.IsValid = false; }
		}

		private void SendBufferSize_Validate(object sender, ValidateEventArgs e)
		{
			e.IsValid = true;
			try { Convert.ToInt32(SendBufferSize.Text); }
			catch (Exception) { e.IsValid = false; }
		}
	}
}