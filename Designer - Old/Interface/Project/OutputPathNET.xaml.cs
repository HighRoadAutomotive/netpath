using System;
using System.Collections.Generic;
using System.Globalization;
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
using Microsoft.Win32;

namespace WCFArchitect.Interface.Project
{
	internal partial class OutputPathNET : Grid
	{
		public Projects.ProjectNETOutputPath Path { get; set; }

		public OutputPathNET(Projects.ProjectNETOutputPath Path)
		{
			this.Path = Path;

			InitializeComponent();

			OptionsGrid.IsEnabled = Path.IsEnabled;
			if (Path.IsEnabled == true) EnableImage.Source = new BitmapImage(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/ReferenceAdded.png"));
			else EnableImage.Source = new BitmapImage(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/NotAvailable.png"));
		}

		private void Enable_Click(object sender, RoutedEventArgs e)
		{
			Path.IsEnabled = !Path.IsEnabled;
			OptionsGrid.IsEnabled = Path.IsEnabled;
			if (Path.IsEnabled == true) EnableImage.Source = new BitmapImage(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/ReferenceAdded.png"));
			else EnableImage.Source = new BitmapImage(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/NotAvailable.png"));
		}

		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the output path: " + Path.Path, "Confirm Output Path Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No, MessageBoxOptions.None) == MessageBoxResult.Yes)
			{
				foreach (Projects.ProjectNET P in Globals.Projects)
					if (P.ID == Path.ProjectID)
					{
						P.ServerOutputPaths.Remove(Path);
						P.ClientOutputPaths.Remove(Path);
						break;
					}
			}
		}

		private void OutputBrowse_Click(object sender, RoutedEventArgs e)
		{
			string FileName = "";
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (!(Path.Path == "" || Path.Path == null)) openpath = Path.Path;
			try
			{

				Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select an Output Path");
				ofd.AllowNonFileSystemItems = false;
				ofd.EnsurePathExists = true;
				ofd.IsFolderPicker = true;
				ofd.InitialDirectory = openpath;
				ofd.Multiselect = false;
				ofd.ShowPlacesList = true;
				if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;

				FileName = Globals.GetRelativePath(System.IO.Path.GetDirectoryName(Globals.ProjectSpace.FileName), ofd.FileName);
			}
			catch
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.InitialDirectory = openpath;
				ofd.FileName = "_";
				ofd.DefaultExt = ".dll";
				ofd.Filter = "DLL files (*.dll)|*.dll";
				ofd.CheckPathExists = true;
				ofd.Title = "Select a Output Path";
				if (ofd.ShowDialog().Value == false) return;

				FileName = Globals.GetRelativePath(System.IO.Path.GetDirectoryName(Globals.ProjectSpace.FileName), System.IO.Path.GetDirectoryName(ofd.FileName));
			}
			
			Path.Path = FileName + "\\";
		}
	}

	[ValueConversion(typeof(Projects.ProjectNETOutputFramework), typeof(int))]
	public class ProjectNETOutputPathFrameworkConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Projects.ProjectNETOutputFramework lt = (Projects.ProjectNETOutputFramework)value;
			if (lt == Projects.ProjectNETOutputFramework.NET45) return 0;
			if (lt == Projects.ProjectNETOutputFramework.NET40) return 1;
			if (lt == Projects.ProjectNETOutputFramework.NET40Client) return 2;
			if (lt == Projects.ProjectNETOutputFramework.NET35) return 3;
			if (lt == Projects.ProjectNETOutputFramework.NET35Client) return 4;
			if (lt == Projects.ProjectNETOutputFramework.NET30) return 5;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return Projects.ProjectNETOutputFramework.NET40;
			if (lt == 1) return Projects.ProjectNETOutputFramework.NET40;
			if (lt == 2) return Projects.ProjectNETOutputFramework.NET40Client;
			if (lt == 3) return Projects.ProjectNETOutputFramework.NET35;
			if (lt == 4) return Projects.ProjectNETOutputFramework.NET35Client;
			if (lt == 5) return Projects.ProjectNETOutputFramework.NET30;
			return Projects.ProjectNETOutputFramework.NET45;
		}
	}

	[ValueConversion(typeof(Projects.ProjectNETOutputFramework), typeof(string))]
	public class ProjectNETOutputPathFrameworkNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Projects.ProjectNETOutputFramework lt = (Projects.ProjectNETOutputFramework)value;
			if (lt == Projects.ProjectNETOutputFramework.NET45) return ".NET Framework 4.5";
			if (lt == Projects.ProjectNETOutputFramework.NET40) return ".NET Framework 4.0";
			if (lt == Projects.ProjectNETOutputFramework.NET40Client) return ".NET Framework 4.0 Client Profile";
			if (lt == Projects.ProjectNETOutputFramework.NET35) return ".NET Framework 3.5";
			if (lt == Projects.ProjectNETOutputFramework.NET35Client) return ".NET Framework 3.5 Client Profile";
			if (lt == Projects.ProjectNETOutputFramework.NET30) return ".NET Framework 3.0";
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}