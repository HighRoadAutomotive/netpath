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
	public partial class SetLicense : Grid, IDialogContent
	{
		public System.Collections.ObjectModel.ObservableCollection<DialogAction> Actions { get; set; }
		private ContentDialog _host;
		public ContentDialog Host { get { return _host; } set { _host = value; ActionState(false); } }
		public void SetFocus()
		{
			SerialBox.Focus();
		}

		private void ActionState(bool Enabled)
		{
			foreach (DialogAction da in Actions.Where(a => !a.IsCancel))
				da.IsEnabled = Enabled;
		}

		internal string Serial { get { return (string)GetValue(SerialProperty); } set { SetValue(SerialProperty, value); } }
		public static readonly DependencyProperty SerialProperty = DependencyProperty.Register("Serial", typeof(string), typeof(SetLicense), new PropertyMetadata(""));

		internal string UserName { get { return (string)GetValue(UserNameProperty); } set { SetValue(UserNameProperty, value); } }
		public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register("UserName", typeof(string), typeof(SetLicense), new PropertyMetadata(""));

		public string UserEmail { get { return (string)GetValue(UserEmailProperty); } set { SetValue(UserEmailProperty, value); } }
		public static readonly DependencyProperty UserEmailProperty = DependencyProperty.Register("UserEmail", typeof(string), typeof(SetLicense), new PropertyMetadata(""));
	}
}