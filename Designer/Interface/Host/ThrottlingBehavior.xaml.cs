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
	internal partial class ThrottlingBehavior : Grid
	{
		public Projects.HostThrottlingBehavior Data { get { return (Projects.HostThrottlingBehavior)GetValue(DataProperty); } set { SetValue(DataProperty, value); } }
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Projects.HostThrottlingBehavior), typeof(ThrottlingBehavior));

		public ThrottlingBehavior()
		{
			InitializeComponent();

			DataContext = this;
		}

		public ThrottlingBehavior(Projects.HostThrottlingBehavior Data)
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