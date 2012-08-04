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
	internal partial class ThrottlingBehavior : Grid
	{
		public Projects.HostThrottlingBehavior Data { get; set; }

		public ThrottlingBehavior()
		{
			InitializeComponent();
		}

		public ThrottlingBehavior(Projects.HostThrottlingBehavior Data)
		{
			this.Data = Data;

			InitializeComponent();
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

		private void MaxConcurrentCalls_ValueChanged(object sender, C1.WPF.PropertyChangedEventArgs<double> e)
		{
			if (IsLoaded == false) return;
			Data.MaxConcurrentCalls = Convert.ToInt32(e.NewValue);
		}

		private void MaxConcurrentInstances_ValueChanged(object sender, C1.WPF.PropertyChangedEventArgs<double> e)
		{
			if (IsLoaded == false) return;
			Data.MaxConcurrentInstances = Convert.ToInt32(e.NewValue);
		}

		private void MaxConcurrentSessions_ValueChanged(object sender, C1.WPF.PropertyChangedEventArgs<double> e)
		{
			if (IsLoaded == false) return;
			Data.MaxConcurrentSessions = Convert.ToInt32(e.NewValue);
		}
	}
}