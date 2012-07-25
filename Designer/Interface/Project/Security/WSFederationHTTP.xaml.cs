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

namespace WCFArchitect.Interface.Project.Security
{
	internal partial class WSFederationHTTP : Grid
	{
		public Projects.BindingSecurityWSFederationHTTP Security { get; set; }

		public WSFederationHTTP()
		{
			InitializeComponent();
		}

		public WSFederationHTTP(Projects.BindingSecurityWSFederationHTTP Security)
		{
			this.Security = Security;

			InitializeComponent();
		}

		private void CodeName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Security.CodeName = Helpers.RegExs.ReplaceSpaces.Replace(CodeName.Text, "");
		}

		private void CodeName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(CodeName.Text);
		}

		private void MessageEstablishSecurityContext_Checked(object sender, RoutedEventArgs e)
		{
			MessageEstablishSecurityContext.Content = "Yes";
		}

		private void MessageEstablishSecurityContext_Unchecked(object sender, RoutedEventArgs e)
		{
			MessageEstablishSecurityContext.Content = "No";
		}

		private void MessageNegotiateServiceCredential_Checked(object sender, RoutedEventArgs e)
		{
			MessageNegotiateServiceCredential.Content = "Yes";
		}

		private void MessageNegotiateServiceCredential_Unchecked(object sender, RoutedEventArgs e)
		{
			MessageNegotiateServiceCredential.Content = "No";
		}
	}
}