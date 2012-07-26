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

namespace WCFArchitect.Interface.Project.Host
{
	internal partial class DebugBehavior : Grid
	{
		public Projects.HostDebugBehavior Data { get; set; }

		public DebugBehavior()
		{
			InitializeComponent();
		}

		public DebugBehavior(Projects.HostDebugBehavior Data)
		{
			this.Data = Data;

			InitializeComponent();

			if (Data.HttpHelpPageBinding != null)
				HttpBinding.Text = Data.HttpHelpPageBinding.Name;

			if (Data.HttpsHelpPageBinding != null)
				HttpsBinding.Text = Data.HttpsHelpPageBinding.Name;
		}

		private void CodeName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.CodeName = Helpers.RegExs.ReplaceSpaces.Replace(CodeName.Text, "");
		}

		private void CodeName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(CodeName.Text);
		}

		private void HttpHelpPageUrl_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.HttpHelpPageUrl = Helpers.RegExs.ReplaceSpaces.Replace(HttpHelpPageUrl.Text, "");
		}

		private void HttpHelpPageUrl_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(HttpHelpPageUrl.Text);
		}

		private void HttpsHelpPageUrl_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.HttpsHelpPageUrl = Helpers.RegExs.ReplaceSpaces.Replace(HttpsHelpPageUrl.Text, "");
		}

		private void HttpsHelpPageUrl_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(HttpsHelpPageUrl.Text);
		}

        private void IsDefaultBehavior_Checked(object sender, RoutedEventArgs e)
        {
            IsDefaultBehavior.Content = "Yes";
        }

        private void IsDefaultBehavior_Unchecked(object sender, RoutedEventArgs e)
        {
            IsDefaultBehavior.Content = "No";
        }

		private void IncludeExceptionDetailInFaults_Checked(object sender, RoutedEventArgs e)
		{
			IncludeExceptionDetailInFaults.Content = "Yes";
		}

		private void IncludeExceptionDetailInFaults_Unchecked(object sender, RoutedEventArgs e)
		{
			IncludeExceptionDetailInFaults.Content = "No";
		}

		private void HttpSelectBinding_Click(object sender, RoutedEventArgs e)
		{
			Interface.Project.Bindings.SelectBinding SB = new Bindings.SelectBinding(Data.Parent.Parent);
			if (SB.ShowDialog().Value == true)
			{
				Data.HttpHelpPageBinding = SB.SelectedBinding;
				HttpBinding.Text = Data.HttpHelpPageBinding.Name;
			}
		}

		private void HttpHelpPageEnabled_Checked(object sender, RoutedEventArgs e)
		{
			HttpHelpPageEnabled.Content = "Yes";
			HttpHelpPageUrl.IsEnabled = true;
		}

		private void HttpHelpPageEnabled_Unchecked(object sender, RoutedEventArgs e)
		{
			HttpHelpPageEnabled.Content = "No";
			HttpHelpPageUrl.IsEnabled = false;
		}

		private void HttpsSelectBinding_Click(object sender, RoutedEventArgs e)
		{
			Interface.Project.Bindings.SelectBinding SB = new Bindings.SelectBinding(Data.Parent.Parent);
			if (SB.ShowDialog().Value == true)
			{
				Data.HttpsHelpPageBinding = SB.SelectedBinding;
				HttpsBinding.Text = Data.HttpsHelpPageBinding.Name;
			}
		}

		private void HttpsHelpPageEnabled_Checked(object sender, RoutedEventArgs e)
		{
			HttpsHelpPageEnabled.Content = "Yes";
			HttpsHelpPageUrl.IsEnabled = true;
		}

		private void HttpsHelpPageEnabled_Unchecked(object sender, RoutedEventArgs e)
		{
			HttpsHelpPageEnabled.Content = "No";
			HttpsHelpPageUrl.IsEnabled = false;
		}
	}
}