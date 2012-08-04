using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace WCFArchitect.Interface.Namespace
{
	internal partial class Namespace : Grid
	{
		public Projects.Namespace Data { get; set; }

		public Namespace(Projects.Namespace Data)
		{
			this.Data = Data;

			InitializeComponent();

			FullName.Text = Data.FullName;
		}

		private void Grid_GotFocus(object sender, RoutedEventArgs e)
		{
			FullName.Text = Data.FullName;
		}

		private void CodeName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			CodeName.Text = Helpers.RegExs.ReplaceSpaces.Replace(CodeName.Text, "");
			Data.UpdateFullNamespace();
		}

		private void CodeName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(CodeName.Text);
		}

		private void NamespaceURI_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.URI = Helpers.RegExs.ReplaceSpaces.Replace(NamespaceURI.Text, "");
		}

		private void NamespaceURI_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchHTTPURI.IsMatch(NamespaceURI.Text);
		}

		private void NamespaceList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Globals.MainScreen.OpenProjectItem(NamespaceList.SelectedItem);
		}

		private void ServiceList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Globals.MainScreen.OpenProjectItem(ServiceList.SelectedItem);
		}

		private void DataList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Globals.MainScreen.OpenProjectItem(DataList.SelectedItem);
		}

		private void EnumList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Globals.MainScreen.OpenProjectItem(EnumList.SelectedItem);
		}

		private void NamespaceList_LostFocus(object sender, RoutedEventArgs e)
		{
			NamespaceList.SelectedItem = null;
		}

		private void ServiceList_LostFocus(object sender, RoutedEventArgs e)
		{
			ServiceList.SelectedItem = null;
		}

		private void DataList_LostFocus(object sender, RoutedEventArgs e)
		{
			DataList.SelectedItem = null;
		}

		private void EnumList_LostFocus(object sender, RoutedEventArgs e)
		{
			EnumList.SelectedItem = null;
		}
	}
}