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
using C1.WPF;
using Prospective.Controls;
using Prospective.Controls.Dialogs;
using WCFArchitect.Projects;
using WCFArchitect.Projects.Helpers;

namespace WCFArchitect.Interface.Bindings
{
	internal partial class PeerTCP : Grid
	{
		public ServiceBindingPeerTCP Binding { get { return (ServiceBindingPeerTCP)GetValue(BindingProperty); } set { SetValue(BindingProperty, value); } }
		public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding", typeof(ServiceBindingPeerTCP), typeof(PeerTCP));

		public PeerTCP()
		{
			InitializeComponent();
		}

		public PeerTCP(ServiceBindingPeerTCP Binding)
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

		private void ListenIPAddress_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Binding.ListenIPAddress = RegExs.ReplaceSpaces.Replace(ListenIPAddress.Text, @"");
		}

		private void ListenIPAddress_Validate(object sender, ValidateEventArgs e)
		{
			e.IsValid = (RegExs.MatchIPv4.IsMatch(ListenIPAddress.Text) || RegExs.MatchIPv6.IsMatch(ListenIPAddress.Text));
		}

		private void Port_ValueChanged(object sender, PropertyChangedEventArgs<double> e)
		{
			Binding.Port = Convert.ToInt32(e.NewValue);
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
	}
}