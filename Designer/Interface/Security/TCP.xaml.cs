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
	internal partial class TCP : Grid
	{
		public Projects.BindingSecurityTCP Security { get; set; }

		public TCP()
		{
			InitializeComponent();
		}

		public TCP(Projects.BindingSecurityTCP Security)
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
	}
}