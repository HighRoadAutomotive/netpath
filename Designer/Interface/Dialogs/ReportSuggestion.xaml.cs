using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
using Prospective.Controls;
using Prospective.Controls.Dialogs;

namespace NETPath.Interface.Dialogs
{
	public partial class ReportSuggestion : Grid, IDialogContent
	{
		public System.Collections.ObjectModel.ObservableCollection<DialogAction> Actions { get; set; }
		public ContentDialog Host { get; set; }
		public void SetFocus()
		{
			ReporterName.Focus();

			foreach (DialogAction da in Actions)
				if ((string)da.Content == "Send Report")
					da.IsEnabled = false;
		}

		public ReportSuggestion()
		{
			InitializeComponent();
		}

		public void SendReport()
		{
			var message = new MailMessage();
			message.From = new MailAddress(ReporterEmail.Text, ReporterName.Text);

			message.To.Add(new MailAddress("support@prospectivesoftware.com"));
			message.Subject = ReportSummary.Text;
			message.Body = ReportContext.Text;

			var client = new SmtpClient("mail.prospectivesoftware.com", 25);
			client.Credentials = new NetworkCredential("PSSupport", "!@#invictussdp)(*", "HRA");
			client.Send(message);
		}

		private void ReporterEmail_Validate(object Sender, ValidateEventArgs E)
		{
			E.IsValid = Projects.Helpers.RegExs.MatchEmail.IsMatch(ReporterEmail.Text);
			Report_TextChanged(null, null);
		}

		private void Report_TextChanged(object Sender, TextChangedEventArgs E)
		{
			if (string.IsNullOrEmpty(ReporterName.Text) || ReporterEmail.IsInvalid || string.IsNullOrEmpty(ReporterEmail.Text) || string.IsNullOrEmpty(ReportSummary.Text) || string.IsNullOrEmpty(ReportContext.Text)) return;

			foreach (DialogAction da in Actions)
				if ((string)da.Content == "Send Report")
					da.IsEnabled = true;
		}
	}
}