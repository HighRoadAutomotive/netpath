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
	internal partial class MetadataBehavior : Grid
	{
		public Projects.HostMetadataBehavior Data { get; set; }

		public MetadataBehavior()
		{
			InitializeComponent();
		}

		public MetadataBehavior(Projects.HostMetadataBehavior Data)
		{
			this.Data = Data;

			InitializeComponent();

			if (Data.HttpGetBinding != null)
				HttpBinding.Text = Data.HttpGetBinding.Name;

			if (Data.HttpsGetBinding != null)
				HttpsBinding.Text = Data.HttpsGetBinding.Name;
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

        private void IsDefaultBehavior_Checked(object sender, RoutedEventArgs e)
        {
            IsDefaultBehavior.Content = "Yes";
        }

        private void IsDefaultBehavior_Unchecked(object sender, RoutedEventArgs e)
        {
            IsDefaultBehavior.Content = "No";
        }

		private void HttpGetUrl_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.HttpGetUrl = Helpers.RegExs.ReplaceSpaces.Replace(HttpGetUrl.Text, "");
		}

		private void HttpGetUrl_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(HttpGetUrl.Text);
		}

		private void HttpsGetUrl_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.HttpsGetUrl = Helpers.RegExs.ReplaceSpaces.Replace(HttpsGetUrl.Text, "");
		}

		private void HttpsGetUrl_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(HttpsGetUrl.Text);
		}

		private void HttpSelectBinding_Click(object sender, RoutedEventArgs e)
		{
			Interface.Project.Bindings.SelectBinding SB = new Bindings.SelectBinding(Data.Parent.Parent);
			if (SB.ShowDialog().Value == true)
			{
				Data.HttpGetBinding = SB.SelectedBinding;
				HttpBinding.Text = Data.HttpGetBinding.Name;
			}
		}

		private void HttpGetEnabled_Checked(object sender, RoutedEventArgs e)
		{
			HttpGetEnabled.Content = "Yes";
			HttpGetUrl.IsEnabled = true;
		}

		private void HttpGetEnabled_Unchecked(object sender, RoutedEventArgs e)
		{
			HttpGetEnabled.Content = "No";
			HttpGetUrl.IsEnabled = false;
		}

		private void HttpsSelectBinding_Click(object sender, RoutedEventArgs e)
		{
			Interface.Project.Bindings.SelectBinding SB = new Bindings.SelectBinding(Data.Parent.Parent);
			if (SB.ShowDialog().Value == true)
			{
				Data.HttpsGetBinding = SB.SelectedBinding;
				HttpsBinding.Text = Data.HttpsGetBinding.Name;
			}
		}

		private void HttpsGetEnabled_Checked(object sender, RoutedEventArgs e)
		{
			HttpsGetEnabled.Content = "Yes";
			HttpsGetUrl.IsEnabled = true;
		}

		private void HttpsGetEnabled_Unchecked(object sender, RoutedEventArgs e)
		{
			HttpsGetEnabled.Content = "No";
			HttpsGetUrl.IsEnabled = false;
		}
	}
}