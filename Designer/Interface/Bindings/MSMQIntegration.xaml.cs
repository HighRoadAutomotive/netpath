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
using Prospective.Controls;
using Prospective.Controls.Dialogs;
using WCFArchitect.Projects;
using WCFArchitect.Projects.Helpers;

namespace WCFArchitect.Interface.Bindings
{
	internal partial class MSMQIntegration : Grid
	{
		public ServiceBindingMSMQIntegration Binding { get { return (ServiceBindingMSMQIntegration)GetValue(BindingProperty); } set { SetValue(BindingProperty, value); } }
		public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding", typeof(ServiceBindingMSMQIntegration), typeof(MSMQIntegration));

		public MSMQIntegration()
		{
			InitializeComponent();
		}

		public MSMQIntegration(ServiceBindingMSMQIntegration Binding)
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

		private void CustomDeadLetterQueue_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Binding.CustomDeadLetterQueue = RegExs.ReplaceSpaces.Replace(CustomDeadLetterQueue.Text, @"");
		}

		private void CustomDeadLetterQueue_Validate(object sender, ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchMSMQURI.IsMatch(CustomDeadLetterQueue.Text);
		}

		private void MaxReceivedMessageSize_Validate(object sender, ValidateEventArgs e)
		{
			e.IsValid = true;
			try { Convert.ToInt64(MaxReceivedMessageSize.Text); }
			catch (Exception) { e.IsValid = false; }
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