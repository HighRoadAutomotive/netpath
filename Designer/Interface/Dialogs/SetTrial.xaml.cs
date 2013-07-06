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
using System.Windows.Shapes;
using Prospective.Controls.Dialogs;
using NETPath.Projects.Helpers;

namespace NETPath.Interface.Dialogs
{
	public partial class SetTrial : Grid, IDialogContent
	{
		public System.Collections.ObjectModel.ObservableCollection<DialogAction> Actions { get; set; }
		private ContentDialog _host;
		public ContentDialog Host { get { return _host; } set { _host = value; ActionState(false); } }
		public void SetFocus()
		{
			YourName.Focus();
		}

		private void ActionState(bool Enabled)
		{
			foreach (DialogAction da in Actions.Where(a => !a.IsCancel))
				da.IsEnabled = Enabled;
		}

		internal string UserName { get { return (string)GetValue(UserNameProperty); } set { SetValue(UserNameProperty, value); } }
		public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register("UserName", typeof(string), typeof(SetLicense), new PropertyMetadata(""));

		public string UserEmail { get { return (string)GetValue(UserEmailProperty); } set { SetValue(UserEmailProperty, value); } }
		public static readonly DependencyProperty UserEmailProperty = DependencyProperty.Register("UserEmail", typeof(string), typeof(SetLicense), new PropertyMetadata(""));

		internal string Company { get { return (string)GetValue(CompanyProperty); } set { SetValue(CompanyProperty, value); } }
		public static readonly DependencyProperty CompanyProperty = DependencyProperty.Register("Company", typeof(string), typeof(SetLicense), new PropertyMetadata(""));

		public string Country { get { return (string)GetValue(CountryProperty); } set { SetValue(CountryProperty, value); } }
		public static readonly DependencyProperty CountryProperty = DependencyProperty.Register("Country", typeof(string), typeof(SetTrial), new PropertyMetadata("United States"));

		public bool AllowProductEmails { get { return (bool)GetValue(AllowProductEmailsProperty); } set { SetValue(AllowProductEmailsProperty, value); } }
		public static readonly DependencyProperty AllowProductEmailsProperty = DependencyProperty.Register("AllowProductEmails", typeof(bool), typeof(SetTrial), new PropertyMetadata(true));

		public bool AllowOtherEmails { get { return (bool)GetValue(AllowOtherEmailsProperty); } set { SetValue(AllowOtherEmailsProperty, value); } }
		public static readonly DependencyProperty AllowOtherEmailsProperty = DependencyProperty.Register("AllowOtherEmails", typeof(bool), typeof(SetTrial), new PropertyMetadata(false));
	}
}