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

namespace WCFArchitect.Interface.Service
{
	internal partial class Property : Grid
	{
		public Projects.Property Data { get; set; }

		public Property()
		{
			InitializeComponent();
		}

		public Property(Projects.Property Data)
		{
			this.Data = Data;

			InitializeComponent();
		}

		private void CodeName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.CodeName = Helpers.RegExs.ReplaceSpaces.Replace(CodeName.Text, @"");
		}

		private void CodeName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(CodeName.Text);
		}

		private void IsReadOnly_Checked(object sender, RoutedEventArgs e)
		{
			IsReadOnly.Content = "Yes";
		}

		private void IsReadOnly_Unchecked(object sender, RoutedEventArgs e)
		{
			IsReadOnly.Content = "No";
		}

		private void IsOneWay_Checked(object sender, RoutedEventArgs e)
		{
			IsOneWay.Content = "Yes";
		}

		private void IsOneWay_Unchecked(object sender, RoutedEventArgs e)
		{
			IsOneWay.Content = "No";
		}
	}
}