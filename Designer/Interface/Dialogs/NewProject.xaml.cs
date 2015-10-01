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
	public partial class NewProject : Grid, IDialogContent
	{
		public System.Collections.ObjectModel.ObservableCollection<DialogAction> Actions { get; set; }
		private ContentDialog _host;
		public ContentDialog Host { get { return _host; } set { _host = value; ActionState(false); } }
		public void SetFocus()
		{
			NewProjectName.Focus();
		}

		internal Type ProjectType { get; set; }
		internal string FileName { get; set; }

		public NewProject(Type ProjectType)
		{
			InitializeComponent();

			this.ProjectType = ProjectType;
			NewProjectName.Focus();
		}

		private void NewProjectBrowse_Click(object sender, RoutedEventArgs e)
		{
			string openpath = Globals.UserProfile.DefaultProjectFolder;
			if (!string.IsNullOrEmpty(NewProjectLocation.Text)) openpath = NewProjectLocation.Text;

			var sfd = new Microsoft.WindowsAPICodePack.Dialogs.CommonSaveFileDialog("Select a Location to Save the Project");
			sfd.AlwaysAppendDefaultExtension = true;
			sfd.EnsurePathExists = true;
			sfd.InitialDirectory = openpath;
			sfd.ShowPlacesList = true;
			sfd.DefaultFileName = NewProjectName.Text;
			sfd.DefaultExtension = ".npp";
			sfd.Filters.Add(new Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter("NETPath Project Files", ".npp"));
			if (sfd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;
			FileName = sfd.FileName;

			NewProjectLocation.Text = new System.IO.FileInfo(FileName).Directory.FullName;
		}

		internal void Create()
		{
			if (ProjectType == typeof(Projects.WebApi.WebApiProject))
				Globals.MainScreen.NewWebApiProject(NewProjectName.Text, FileName);
		}

		private void NewProjectName_TextChanged(object Sender, TextChangedEventArgs E)
		{
			ActionState(!string.IsNullOrEmpty(NewProjectName.Text) && RegExs.MatchFileName.IsMatch(NewProjectName.Text) && !string.IsNullOrEmpty(NewProjectLocation.Text));
		}

		private void NewProjectLocation_TextChanged(object Sender, TextChangedEventArgs E)
		{
			ActionState(!string.IsNullOrEmpty(NewProjectName.Text) && RegExs.MatchFileName.IsMatch(NewProjectName.Text) && !string.IsNullOrEmpty(NewProjectLocation.Text));
		}

		private void ActionState(bool Enabled)
		{
			foreach (DialogAction da in Actions.Where(a => !a.IsCancel))
				da.IsEnabled = Enabled;
		}
	}
}