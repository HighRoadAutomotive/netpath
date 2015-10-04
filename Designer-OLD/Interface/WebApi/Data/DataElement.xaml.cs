﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NETPath.Projects;
using NETPath.Projects.Helpers;
using NETPath.Projects.WebApi;

namespace NETPath.Interface.WebApi.Data
{
	public partial class DataElement : Grid
	{
		public WebApiDataElement Element { get { return (WebApiDataElement)GetValue(ElementProperty); } set { SetValue(ElementProperty, value); } }
		public static readonly DependencyProperty ElementProperty = DependencyProperty.Register("Element", typeof(WebApiDataElement), typeof(DataElement));

		public WebApiProject Project { get { return (WebApiProject)GetValue(ProjectProperty); } set { SetValue(ProjectProperty, value); } }
		public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(WebApiProject), typeof(DataElement));

		public DataElement(WebApiDataElement Element)
		{
			InitializeComponent();

			this.Element = Element;
			Project = Element.Owner.Parent.Owner as WebApiProject;

			DataType.Project = Project;
		}

		private void ElementName_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			var elementName = sender as EllipticBit.Controls.WPF.TextBox;
			if (elementName == null) return;
			if(string.IsNullOrEmpty(elementName.Text)) return;

			string t = RegExs.ReplaceSpaces.Replace(elementName.Text ?? "", @"");
			e.IsValid = RegExs.MatchCodeName.IsMatch(t);
		}

		private void Order_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			var order = sender as EllipticBit.Controls.WPF.TextBox;
			if (order == null) return;
			if (string.IsNullOrEmpty(order.Text)) return;

			try { Convert.ToInt32(order.Text); }
			catch { e.IsValid = false; return; }
			e.IsValid = true;
		}

		private void GenerateContractName_Unchecked(object sender, RoutedEventArgs e)
		{
			ContractName.Text = "";
		}

		private void GenerateClientType_Unchecked(object sender, RoutedEventArgs e)
		{
			ClientName.Text = "";
		}

		private void GenerateXAMLBinding_Unchecked(object sender, RoutedEventArgs e)
		{
			XAMLName.Text = "";
		}
	}
}