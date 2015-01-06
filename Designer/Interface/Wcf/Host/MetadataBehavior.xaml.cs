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
using NETPath.Projects.Wcf;

namespace NETPath.Interface.Wcf.Host
{
	internal partial class MetadataBehavior : Grid
	{
		public WcfHostMetadataBehavior Data { get { return (WcfHostMetadataBehavior)GetValue(DataProperty); } set { SetValue(DataProperty, value); } }
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(WcfHostMetadataBehavior), typeof(MetadataBehavior));

		public MetadataBehavior()
		{
			InitializeComponent();

			DataContext = this;
		}

		public MetadataBehavior(WcfHostMetadataBehavior Data)
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

		private void Name_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchCodeName.IsMatch(DisplayName.Text);
		}

		private void HttpGetUrl_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.HttpGetUrl = RegExs.ReplaceSpaces.Replace(HttpGetUrl.Text, "");
		}

		private void HttpGetUrl_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchHTTPURI.IsMatch(HttpGetUrl.Text);
		}

		private void HttpsGetUrl_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.HttpsGetUrl = RegExs.ReplaceSpaces.Replace(HttpsGetUrl.Text, "");
		}

		private void HttpsGetUrl_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchHTTPURI.IsMatch(HttpsGetUrl.Text);
		}
	}
}