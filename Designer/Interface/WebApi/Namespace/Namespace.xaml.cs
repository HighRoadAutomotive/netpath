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
using NETPath.Projects;
using NETPath.Projects.WebApi;
using NETPath.Projects.Helpers;

namespace NETPath.Interface.WebApi.Namespace
{
	internal partial class Namespace : Grid
	{
		public WebApiNamespace Data { get; set; }

		public Namespace(WebApiNamespace Data)
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
			CodeName.Text = RegExs.ReplaceSpaces.Replace(CodeName.Text, "");
			Data.UpdateFullNamespace();
		}

		private void CodeName_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchCodeName.IsMatch(CodeName.Text);
		}

		private void NamespaceURI_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.Uri = RegExs.ReplaceSpaces.Replace(NamespaceURI.Text, "");
		}

		private void NamespaceURI_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchHttpUri.IsMatch(NamespaceURI.Text);
		}

		private void NamespaceList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Globals.OpenProjectItem(NamespaceList.SelectedItem as OpenableDocument);
		}

		private void ServiceList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Globals.OpenProjectItem(ServiceList.SelectedItem as OpenableDocument);
		}

		private void DataList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Globals.OpenProjectItem(DataList.SelectedItem as OpenableDocument);
		}

		private void EnumList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Globals.OpenProjectItem(EnumList.SelectedItem as OpenableDocument);
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