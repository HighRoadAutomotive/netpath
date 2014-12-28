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
using EllipticBit.Controls.WPF.Dialogs;
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
		}

		private void ActionState(bool Enabled)
		{
			foreach (DialogAction da in Actions.Where(a => !a.IsCancel))
				da.IsEnabled = Enabled;
		}

		public SetTrial()
		{
			InitializeComponent();
		}

		public string UserName { get { return (string)GetValue(UserNameProperty); } set { SetValue(UserNameProperty, value); } }
		public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register("UserName", typeof(string), typeof(SetTrial), new PropertyMetadata("", PropertyChangedCallback));

		private static void PropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as SetTrial;
			if (t == null) return;

			if(!string.IsNullOrWhiteSpace(t.UserName) && !string.IsNullOrWhiteSpace(t.UserEmail) && !string.IsNullOrWhiteSpace(t.Country) && !string.IsNullOrWhiteSpace(t.Country))
				foreach (DialogAction da in t.Actions.Where(a => !a.IsCancel))
					da.IsEnabled = true;
		}

		public string UserEmail { get { return (string)GetValue(UserEmailProperty); } set { SetValue(UserEmailProperty, value); } }
		public static readonly DependencyProperty UserEmailProperty = DependencyProperty.Register("UserEmail", typeof(string), typeof(SetTrial), new PropertyMetadata("", PropertyChangedCallback));

		public string Company { get { return (string)GetValue(CompanyProperty); } set { SetValue(CompanyProperty, value); } }
		public static readonly DependencyProperty CompanyProperty = DependencyProperty.Register("Company", typeof(string), typeof(SetTrial), new PropertyMetadata("", PropertyChangedCallback));

		public string Country { get { return (string)GetValue(CountryProperty); } set { SetValue(CountryProperty, value); } }
		public static readonly DependencyProperty CountryProperty = DependencyProperty.Register("Country", typeof(string), typeof(SetTrial), new PropertyMetadata("UNITED STATES", PropertyChangedCallback));

		public bool AllowProductEmails { get { return (bool)GetValue(AllowProductEmailsProperty); } set { SetValue(AllowProductEmailsProperty, value); } }
		public static readonly DependencyProperty AllowProductEmailsProperty = DependencyProperty.Register("AllowProductEmails", typeof(bool), typeof(SetTrial), new PropertyMetadata(true));

		public bool AllowOtherEmails { get { return (bool)GetValue(AllowOtherEmailsProperty); } set { SetValue(AllowOtherEmailsProperty, value); } }
		public static readonly DependencyProperty AllowOtherEmailsProperty = DependencyProperty.Register("AllowOtherEmails", typeof(bool), typeof(SetTrial), new PropertyMetadata(false));
	}
}