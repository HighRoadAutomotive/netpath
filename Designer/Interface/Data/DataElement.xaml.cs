using System;
using System.Collections.Generic;
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
using WCFArchitect.Projects;

namespace WCFArchitect.Interface.Data
{
	public partial class DataElement : Grid
	{
		public Projects.DataElement Element { get { return (Projects.DataElement)GetValue(ElementProperty); } set { SetValue(ElementProperty, value); } }
		public static readonly DependencyProperty ElementProperty = DependencyProperty.Register("Element", typeof(Projects.DataElement), typeof(DataElement));

		public Projects.Project Project { get { return (Projects.Project)GetValue(ProjectProperty); } set { SetValue(ProjectProperty, value); } }
		public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(Projects.Project), typeof(DataElement));

		public DataElement(Projects.DataElement Element)
		{
			InitializeComponent();

			this.Element = Element;
			Project = Element.Owner.Parent.Owner;

			ClientScope.SelectedIndex = -1;
			XAMLScope.SelectedIndex = -1;
		}

		private void ElementName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			var elementName = sender as Prospective.Controls.TextBox;
			if (elementName == null) return;
			if(string.IsNullOrEmpty(elementName.Text)) return;

			string t = Helpers.RegExs.ReplaceSpaces.Replace(elementName.Text ?? "", @"");
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(t);
		}

		private void Order_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			var order = sender as Prospective.Controls.TextBox;
			if (order == null) return;
			if (string.IsNullOrEmpty(order.Text)) return;

			try { Convert.ToInt32(order.Text); }
			catch { e.IsValid = false; return; }
			e.IsValid = true;
		}

		private void GenerateClientType_Unchecked(object sender, RoutedEventArgs e)
		{
			Element.ClientScope = DataScope.Disabled;
			ClientType.OpenType = null;
			ClientName.Text = "";
		}

		private void GenerateXAMLBinding_Unchecked(object sender, RoutedEventArgs e)
		{
			Element.XAMLScope = DataScope.Disabled;
			XAMLType.OpenType = null;
			XAMLName.Text = "";
		}
	}
}