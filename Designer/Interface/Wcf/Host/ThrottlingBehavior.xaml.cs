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
	internal partial class ThrottlingBehavior : Grid
	{
		public WcfHostThrottlingBehavior Data { get { return (WcfHostThrottlingBehavior)GetValue(DataProperty); } set { SetValue(DataProperty, value); } }
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(WcfHostThrottlingBehavior), typeof(ThrottlingBehavior));

		public ThrottlingBehavior()
		{
			InitializeComponent();

			DataContext = this;
		}

		public ThrottlingBehavior(WcfHostThrottlingBehavior Data)
		{
			this.Data = Data;

			InitializeComponent();

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
	}
}