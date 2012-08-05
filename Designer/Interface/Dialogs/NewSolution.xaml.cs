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
	public partial class NewSolution : Grid
	{
		public NewSolution()
		{
			InitializeComponent();
		}

		private void NewSolutionOptionsBrowse_Click(object sender, RoutedEventArgs e)
		{
			string FileName = "";
			string openpath = Globals.UserProfile.DefaultProjectFolder;
			if (!(NewSolutionOptionsLocation.Text == "" || NewSolutionOptionsLocation.Text == null)) openpath = NewSolutionOptionsLocation.Text;

			Microsoft.WindowsAPICodePack.Dialogs.CommonSaveFileDialog sfd = new Microsoft.WindowsAPICodePack.Dialogs.CommonSaveFileDialog("Select a Location to Save the Solution");
			sfd.AlwaysAppendDefaultExtension = true;
			sfd.EnsurePathExists = true;
			sfd.InitialDirectory = openpath;
			sfd.ShowPlacesList = true;
			sfd.DefaultFileName = NewSolutionOptionsName.Text;
			sfd.DefaultExtension = ".was";
			sfd.Filters.Add(new Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter("WCF Architect Solution Files", ".was"));
			if (sfd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;
			FileName = sfd.FileName;

			NewSolutionOptionsLocation.Text = new System.IO.FileInfo(FileName).Directory.FullName + "\\";
		}
	}
}