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

namespace WCFArchitect.Interface.Dialogs
{
	public partial class NewProject : Grid
	{
		internal Type ProjectType { get; set; }
		internal string FileName { get; set; }

		public NewProject(Type ProjectType)
		{
			InitializeComponent();

			this.ProjectType = ProjectType;
		}

		private void NewProjectBrowse_Click(object sender, RoutedEventArgs e)
		{
			string openpath = Globals.UserProfile.DefaultProjectFolder;
			if (!(NewProjectLocation.Text == "" || NewProjectLocation.Text == null)) openpath = NewProjectLocation.Text;

			Microsoft.WindowsAPICodePack.Dialogs.CommonSaveFileDialog sfd = new Microsoft.WindowsAPICodePack.Dialogs.CommonSaveFileDialog("Select a Location to Save the Project");
			sfd.AlwaysAppendDefaultExtension = true;
			sfd.EnsurePathExists = true;
			sfd.InitialDirectory = openpath;
			sfd.ShowPlacesList = true;
			sfd.DefaultFileName = NewProjectName.Text;
			sfd.DefaultExtension = ".wap";
			sfd.Filters.Add(new Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter("WCF Architect Project Files", ".wap"));
			if (sfd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;
			FileName = sfd.FileName;

			NewProjectLocation.Text = new System.IO.FileInfo(FileName).Directory.FullName + "\\";
		}

		internal void Create()
		{
				Globals.MainScreen.NewProject(NewProjectName.Text, FileName);
		}
	}
}