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
	internal partial class NamedPipe : Grid
	{
		public Projects.ServiceBindingNamedPipe Binding { get; set; }

		public NamedPipe()
		{
			InitializeComponent();
		}

		public NamedPipe(Projects.ServiceBindingNamedPipe Binding)
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
			e.IsValid = Helpers.RegExs.MatchPipeURI.IsMatch(EndpointAddress.Text);
		}

		private void ListenAddress_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Binding.ListenAddress = Helpers.RegExs.ReplaceSpaces.Replace(ListenAddress.Text, @"");
		}

		private void ListenAddress_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchPipeURI.IsMatch(ListenAddress.Text);
		}

		private void TransactionFlow_Checked(object sender, RoutedEventArgs e)
		{
			TransactionFlow.Content = "Yes";
		}

		private void TransactionFlow_Unchecked(object sender, RoutedEventArgs e)
		{
			TransactionFlow.Content = "No";
		}

		private void SelectSecurity_Click(object sender, RoutedEventArgs e)
		{
			Interface.Project.Security.SelectSecurity SS = new Security.SelectSecurity(Binding.Parent, typeof(Projects.BindingSecurityNamedPipe));
			if (SS.ShowDialog().Value == true)
			{
				Binding.Security = (Projects.BindingSecurityNamedPipe)SS.SelectedSecurity;
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

		private void MaxConnections_ValueChanged(object sender, C1.WPF.PropertyChangedEventArgs<double> e)
		{
			Binding.MaxConnections = Convert.ToInt32(e.NewValue);
		}
	}
}