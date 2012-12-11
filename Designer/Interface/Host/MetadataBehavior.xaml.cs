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
using NETPath.Projects.Helpers;

namespace NETPath.Interface.Host
{
	internal partial class MetadataBehavior : Grid
	{
		public Projects.HostMetadataBehavior Data { get { return (Projects.HostMetadataBehavior)GetValue(DataProperty); } set { SetValue(DataProperty, value); } }
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Projects.HostMetadataBehavior), typeof(MetadataBehavior));

		public MetadataBehavior()
		{
			InitializeComponent();

			DataContext = this;
		}

		public MetadataBehavior(Projects.HostMetadataBehavior Data)
		{
			this.Data = Data;

			InitializeComponent();

			HttpGetBinding.ItemsSource = Globals.GetBindings();
			HttpGetBinding.SelectedItem = Data.HttpGetBinding;
			HttpsGetBinding.ItemsSource = Globals.GetBindings();
			HttpsGetBinding.SelectedItem = Data.HttpsGetBinding;

			DataContext = this;
		}

		private void Name_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.Name = RegExs.ReplaceSpaces.Replace(DisplayName.Text, "");
		}

		private void Name_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchCodeName.IsMatch(DisplayName.Text);
		}

		private void HttpGetUrl_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.HttpGetUrl = RegExs.ReplaceSpaces.Replace(HttpGetUrl.Text, "");
		}

		private void HttpGetUrl_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchCodeName.IsMatch(HttpGetUrl.Text);
		}

		private void HttpsGetUrl_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.HttpsGetUrl = RegExs.ReplaceSpaces.Replace(HttpsGetUrl.Text, "");
		}

		private void HttpsGetUrl_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchCodeName.IsMatch(HttpsGetUrl.Text);
		}
	}
}