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
using Prospective.Controls.Dialogs;
using WCFArchitect.Projects;
using WCFArchitect.Projects.Helpers;

namespace WCFArchitect.Interface.Bindings
{
	internal partial class TCP : Grid
	{
		public ServiceBindingTCP Binding { get { return (ServiceBindingTCP)GetValue(BindingProperty); } set { SetValue(BindingProperty, value); } }
		public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding", typeof(ServiceBindingTCP), typeof(TCP));

		public TCP()
		{
			InitializeComponent();
		}

		public TCP(ServiceBindingTCP Binding)
		{
			this.Binding = Binding;

			InitializeComponent();

			MaxBufferPoolSize.Text = Binding.MaxBufferPoolSize.ValueRoundedBinary;
			MaxBufferSize.Text = Binding.MaxBufferSize.ValueRoundedBinary;
			MaxReceivedMessageSize.Text = Binding.MaxReceivedMessageSize.ValueRoundedBinary;
		}

		private void Namespace_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Binding.Namespace = RegExs.ReplaceSpaces.Replace(Namespace.Text, @"");
		}

		private void Namespace_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchHTTPURI.IsMatch(Namespace.Text);
		}

		private void EndpointAddress_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Binding.EndpointAddress = RegExs.ReplaceSpaces.Replace(EndpointAddress.Text, @"");
		}

		private void EndpointAddress_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchTCPURI.IsMatch(EndpointAddress.Text);
		}

		private void ListenAddress_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Binding.ListenAddress = RegExs.ReplaceSpaces.Replace(ListenAddress.Text, @"");
		}

		private void ListenAddress_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchTCPURI.IsMatch(ListenAddress.Text);
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

		private void ListenBacklog_ValueChanged(object sender, C1.WPF.PropertyChangedEventArgs<double> e)
		{
			Binding.ListenBacklog = Convert.ToInt32(e.NewValue);
		}

		private void MaxConnections_ValueChanged(object sender, C1.WPF.PropertyChangedEventArgs<double> e)
		{
			Binding.MaxConnections = Convert.ToInt32(e.NewValue);
		}
	}
}