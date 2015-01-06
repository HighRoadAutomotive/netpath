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
	internal partial class DebugBehavior : Grid
	{
		public WcfHostDebugBehavior Data { get { return (WcfHostDebugBehavior)GetValue(DataProperty); } set { SetValue(DataProperty, value); } }
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(WcfHostDebugBehavior), typeof(DebugBehavior));

		public DebugBehavior()
		{
			InitializeComponent();

			DataContext = this;
		}

		public DebugBehavior(WcfHostDebugBehavior Data)
		{
			this.Data = Data;

			InitializeComponent();

			HttpHelpPageBinding.ItemsSource = Globals.GetBindings();
			HttpHelpPageBinding.SelectedItem = Data.HttpHelpPageBinding;
			HttpsHelpPageBinding.ItemsSource = Globals.GetBindings();
			HttpsHelpPageBinding.SelectedItem = Data.HttpsHelpPageBinding;

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

		private void HttpHelpPageUrl_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.HttpHelpPageUrl = RegExs.ReplaceSpaces.Replace(HttpHelpPageUrl.Text, "");
		}

		private void HttpHelpPageUrl_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchCodeName.IsMatch(HttpHelpPageUrl.Text);
		}

		private void HttpsHelpPageUrl_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.HttpsHelpPageUrl = RegExs.ReplaceSpaces.Replace(HttpsHelpPageUrl.Text, "");
		}

		private void HttpsHelpPageUrl_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchCodeName.IsMatch(HttpsHelpPageUrl.Text);
		}
	}
}