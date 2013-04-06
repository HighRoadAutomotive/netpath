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
using System.Windows.Threading;
using Prospective.Controls.Dialogs;
using NETPath.Projects.Helpers;

namespace NETPath.Interface.Dialogs
{
	public partial class NewSolution : Grid, IDialogContent
	{
		public System.Collections.ObjectModel.ObservableCollection<DialogAction> Actions { get; set; }
		private ContentDialog _host;
		public ContentDialog Host { get { return _host; } set { _host = value; ActionState(false); } }
		public void SetFocus()
		{
			NewSolutionOptionsName.Focus();
		}

		internal string FileName { get; set; }

		public NewSolution()
		{
			InitializeComponent();
		}

		private void NewSolutionOptionsBrowse_Click(object sender, RoutedEventArgs e)
		{
			string openpath = Globals.UserProfile.DefaultProjectFolder;
			if (!string.IsNullOrEmpty(NewSolutionOptionsLocation.Text)) openpath = NewSolutionOptionsLocation.Text;

			var sfd = new Microsoft.WindowsAPICodePack.Dialogs.CommonSaveFileDialog("Select a Location to Save the Solution");
			sfd.AlwaysAppendDefaultExtension = true;
			sfd.EnsurePathExists = true;
			sfd.InitialDirectory = openpath;
			sfd.ShowPlacesList = true;
			sfd.DefaultFileName = NewSolutionOptionsName.Text;
			sfd.DefaultExtension = ".nps";
			sfd.Filters.Add(new Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter("NETPath Solution Files", ".nps"));
			if (sfd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;
			FileName = sfd.FileName;

			NewSolutionOptionsLocation.Text = new System.IO.FileInfo(FileName).Directory.FullName;
		}

		internal void Create()
		{
			Globals.MainScreen.NewSolution(NewSolutionOptionsName.Text, FileName);
		}

		private void NewSolutionOptionsName_TextChanged(object Sender, TextChangedEventArgs E)
		{
			ActionState(!string.IsNullOrEmpty(NewSolutionOptionsName.Text) && RegExs.MatchFileName.IsMatch(NewSolutionOptionsName.Text) && !string.IsNullOrEmpty(NewSolutionOptionsLocation.Text));
		}

		private void NewSolutionOptionsLocation_TextChanged(object Sender, TextChangedEventArgs E)
		{
			ActionState(!string.IsNullOrEmpty(NewSolutionOptionsName.Text) && RegExs.MatchFileName.IsMatch(NewSolutionOptionsName.Text) && !string.IsNullOrEmpty(NewSolutionOptionsLocation.Text));
		}

		private void ActionState(bool Enabled)
		{
			foreach (DialogAction da in Actions.Where(a => !a.IsCancel))
				da.IsEnabled = Enabled;
		}

		private void NewSolution_GotFocus(object Sender, RoutedEventArgs E)
		{
			NewSolutionOptionsName.Focus();
		}
	}
}