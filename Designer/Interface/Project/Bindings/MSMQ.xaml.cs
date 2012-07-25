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
	internal partial class MSMQ : Grid
	{
		public Projects.ServiceBindingMSMQ Binding { get; set; }

		public MSMQ()
		{
			InitializeComponent();
		}

		public MSMQ(Projects.ServiceBindingMSMQ Binding)
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
			e.IsValid = Helpers.RegExs.MatchMSMQURI.IsMatch(EndpointAddress.Text);
		}

		private void ListenAddress_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Binding.ListenAddress = Helpers.RegExs.ReplaceSpaces.Replace(ListenAddress.Text, @"");
		}

		private void ListenAddress_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchMSMQURI.IsMatch(ListenAddress.Text);
		}

		private void CustomDeadLetterQueue_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Binding.CustomDeadLetterQueue = Helpers.RegExs.ReplaceSpaces.Replace(CustomDeadLetterQueue.Text, @"");
		}

		private void CustomDeadLetterQueue_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchMSMQURI.IsMatch(CustomDeadLetterQueue.Text);
		}

		private void UseActiveDirectory_Checked(object sender, RoutedEventArgs e)
		{
			UseActiveDirectory.Content = "Yes";
		}

		private void UseActiveDirectory_Unchecked(object sender, RoutedEventArgs e)
		{
			UseActiveDirectory.Content = "No";
		}

		private void UseMSMQTracing_Checked(object sender, RoutedEventArgs e)
		{
			UseMSMQTracing.Content = "Yes";
		}

		private void UseMSMQTracing_Unchecked(object sender, RoutedEventArgs e)
		{
			UseMSMQTracing.Content = "No";
		}

		private void UseSourceJournal_Checked(object sender, RoutedEventArgs e)
		{
			UseSourceJournal.Content = "Yes";
		}

		private void UseSourceJournal_Unchecked(object sender, RoutedEventArgs e)
		{
			UseSourceJournal.Content = "No";
		}

		private void Durable_Checked(object sender, RoutedEventArgs e)
		{
			Durable.Content = "Yes";
		}

		private void Durable_Unchecked(object sender, RoutedEventArgs e)
		{
			Durable.Content = "No";
		}

		private void ExactlyOnce_Unchecked(object sender, RoutedEventArgs e)
		{
			ExactlyOnce.Content = "Yes";
		}

		private void ExactlyOnce_Checked(object sender, RoutedEventArgs e)
		{
			ExactlyOnce.Content = "No";
		}

		private void ReceiveContextEnabled_Checked(object sender, RoutedEventArgs e)
		{
			ReceiveContextEnabled.Content = "Yes";
		}

		private void ReceiveContextEnabled_Unchecked(object sender, RoutedEventArgs e)
		{
			ReceiveContextEnabled.Content = "No";
		}

		private void SelectSecurity_Click(object sender, RoutedEventArgs e)
		{
			Interface.Project.Security.SelectSecurity SS = new Security.SelectSecurity(Binding.Parent, typeof(Projects.BindingSecurityMSMQ));
			if (SS.ShowDialog().Value == true)
			{
				Binding.Security = (Projects.BindingSecurityMSMQ)SS.SelectedSecurity;
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

		private void MaxRetryCycles_ValueChanged(object sender, C1.WPF.PropertyChangedEventArgs<double> e)
		{
			if (IsLoaded == false) return;
			Binding.MaxRetryCycles = Convert.ToInt32(e.NewValue);
		}

		private void ReceiveRetryCount_ValueChanged(object sender, C1.WPF.PropertyChangedEventArgs<double> e)
		{
			if (IsLoaded == false) return;
			Binding.ReceiveRetryCount = Convert.ToInt32(e.NewValue);
		}
	}
}