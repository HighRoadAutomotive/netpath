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
	internal partial class OutputPath : Grid
	{
		public Projects.ProjectOutputPath Path { get; set; }

		public OutputPath(Projects.ProjectOutputPath Path)
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
				foreach (Projects.Project P in Globals.Projects)
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

				FileName = ofd.FileName;
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

				FileName = new System.IO.FileInfo(ofd.FileName).Directory.FullName + "\\";
			}
			
			Path.Path = FileName + "\\";
		}
	}

	[ValueConversion(typeof(Projects.ProjectOutputFramework), typeof(int))]
	public class ProjectOutputPathFrameworkConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Projects.ProjectOutputFramework lt = (Projects.ProjectOutputFramework)value;
			if (lt == Projects.ProjectOutputFramework.NET40) return 0;
			if (lt == Projects.ProjectOutputFramework.NET40Client) return 1;
			if (lt == Projects.ProjectOutputFramework.NET35) return 2;
			if (lt == Projects.ProjectOutputFramework.NET35Client) return 3;
			if (lt == Projects.ProjectOutputFramework.NET30) return 4;
			if (lt == Projects.ProjectOutputFramework.SL40) return 5;
			if (lt == Projects.ProjectOutputFramework.SL30) return 6;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return Projects.ProjectOutputFramework.NET40;
			if (lt == 1) return Projects.ProjectOutputFramework.NET40Client;
			if (lt == 2) return Projects.ProjectOutputFramework.NET35;
			if (lt == 3) return Projects.ProjectOutputFramework.NET35Client;
			if (lt == 4) return Projects.ProjectOutputFramework.NET30;
			if (lt == 5) return Projects.ProjectOutputFramework.SL40;
			if (lt == 6) return Projects.ProjectOutputFramework.SL30;
			return Projects.ProjectOutputFramework.NET40;
		}
	}

	[ValueConversion(typeof(Projects.ProjectOutputFramework), typeof(string))]
	public class ProjectOutputPathFrameworkNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Projects.ProjectOutputFramework lt = (Projects.ProjectOutputFramework)value;
			if (lt == Projects.ProjectOutputFramework.NET40) return ".NET Framework 4.0";
			if (lt == Projects.ProjectOutputFramework.NET40Client) return ".NET Framework 4.0 Client Profile";
			if (lt == Projects.ProjectOutputFramework.NET35) return ".NET Framework 3.5";
			if (lt == Projects.ProjectOutputFramework.NET35Client) return ".NET Framework 3.5 Client Profile";
			if (lt == Projects.ProjectOutputFramework.NET30) return ".NET Framework 3.0";
			if (lt == Projects.ProjectOutputFramework.SL50) return "Silverlight 5";
			if (lt == Projects.ProjectOutputFramework.SL40) return "Silverlight 4";
			if (lt == Projects.ProjectOutputFramework.SL30) return "Silverlight 3";
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}