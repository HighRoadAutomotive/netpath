using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
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
	public partial class ReportError : Grid, IDialogContent
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

		private Exception Exception { get; set; }
		private byte[] Screenshot { get; set; }
		private Dictionary<string, byte[]> Projects { get; set; }

		private Version OSVersion { get; set; }
		private string OSPlatform { get; set; }
		private string OSServicePack { get; set; }
		private Version CLRVersion { get; set; }
		private int ProcessorCount { get; set; }
		private long WorkingSet { get; set; }
		private bool IsX64OS { get; set; }
		private bool IsX64Process { get; set; }
		
		public ReportError(Exception Exception, byte[] Screenshot, Dictionary<string, byte[]> Projects)
		{
			InitializeComponent();

			this.Exception = Exception;
			this.Screenshot = Screenshot;
			this.Projects = Projects;

			OSVersion = Environment.OSVersion.Version;
			OSPlatform = Environment.OSVersion.Platform.ToString();
			OSServicePack = Environment.OSVersion.ServicePack;
			CLRVersion = Environment.Version;
			ProcessorCount = Environment.ProcessorCount;
			WorkingSet = Environment.WorkingSet;
			IsX64OS = Environment.Is64BitOperatingSystem;
			IsX64Process = Environment.Is64BitProcess;
		}

		public void SendReport()
		{
			var message = new MailMessage();
			message.From = new MailAddress(ReporterEmail.Text, ReporterName.Text);

			message.To.Add(new MailAddress("support@prospectivesoftware.com"));
			message.Subject = Exception.Message;
			message.Body += "Error Description:" + Environment.NewLine;
			message.Body += ReportContext.Text;
			message.Body += Environment.NewLine + Environment.NewLine;
			message.Body += "Exception:" + Environment.NewLine;
			message.Body += Exception.ToString();
			message.Body += Environment.NewLine + Environment.NewLine;
			message.Body += "System Information:" + Environment.NewLine;
			message.Body += string.Format("OS Version:\t\t\t{0}{1}", OSVersion, Environment.NewLine);
			message.Body += string.Format("OS Platform:\t\t\t{0}{1}", OSPlatform, Environment.NewLine);
			message.Body += string.Format("OS Service Pack:\t\t{0}{1}", OSServicePack, Environment.NewLine);
			message.Body += string.Format("CLR Version:\t\t\t{0}{1}", CLRVersion, Environment.NewLine);
			message.Body += string.Format("Processors:\t\t\t{0}{1}", ProcessorCount, Environment.NewLine);
			message.Body += string.Format("Working Set:\t\t\t{0}{1}", WorkingSet, Environment.NewLine);
			message.Body += string.Format("Is x64 OS:\t\t\t{0}{1}", IsX64OS, Environment.NewLine);
			message.Body += string.Format("Is x64 Process:\t\t{0}{1}", IsX64Process, Environment.NewLine);

			if (ReportScreenshot.IsChecked != null && ReportScreenshot.IsChecked.Value)
				message.Attachments.Add(new Attachment(new MemoryStream(Screenshot), "ExceptionScreenshot.png"));

			if (ReportProjectFiles.IsChecked != null && ReportProjectFiles.IsChecked.Value)
			{
				foreach (KeyValuePair<string, byte[]> pf in Projects)
					message.Attachments.Add(new Attachment(new MemoryStream(pf.Value), pf.Key));
			}

			var client = new SmtpClient("mail.prospectivesoftware.com", 25);
			client.Credentials = new NetworkCredential("PSSupport", "!@#invictussdp)(*", "HRA");
			client.Send(message);
		}

		private void ReporterEmail_Validate(object Sender, ValidateEventArgs E)
		{
			E.IsValid = NETPath.Projects.Helpers.RegExs.MatchEmail.IsMatch(ReporterEmail.Text);
			Report_TextChanged(null, null);
		}

		private void Report_TextChanged(object Sender, TextChangedEventArgs E)
		{
			if (string.IsNullOrEmpty(ReporterName.Text) || ReporterEmail.IsInvalid || string.IsNullOrEmpty(ReporterEmail.Text) || string.IsNullOrEmpty(ReportContext.Text)) return;

			foreach (DialogAction da in Actions)
				if ((string)da.Content == "Send Report")
					da.IsEnabled = true;
		}

		private void ReportScreenshot_Checked(object Sender, RoutedEventArgs E)
		{
			ReportScreenshot.Content = "Yes";
		}

		private void ReportScreenshot_Unchecked(object Sender, RoutedEventArgs E)
		{
			ReportScreenshot.Content = "No";
		}

		private void ReportProjectFiles_Checked(object Sender, RoutedEventArgs E)
		{
			ReportProjectFiles.Content = "Yes";
		}

		private void ReportProjectFiles_Unchecked(object Sender, RoutedEventArgs E)
		{
			ReportProjectFiles.Content = "No";
		}
	}
}