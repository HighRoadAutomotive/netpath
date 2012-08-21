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
using Prospective.Controls.Dialogs;

namespace WCFArchitect.Interface.Project
{
	internal partial class GenerationTarget : Grid
	{
		private Projects.Project Project { get; set; }
		public Projects.ProjectGenerationTarget Path { get; set; }

		public GenerationTarget(Projects.Project Project, Projects.ProjectGenerationTarget Path)
		{
			this.Project = Project;
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
			DialogService.ShowMessageDialog(Project, "Confirm Generation Target Deletion", "Are you sure you want to delete the generation target: " + Path.Path,
				new DialogAction("Yes", new Action(() => { Project.ServerGenerationTargets.Remove(Path); Project.ClientGenerationTargets.Remove(Path); }), true), new DialogAction("No", false, true));
		}

		private void OutputBrowse_Click(object sender, RoutedEventArgs e)
		{
			string FileName = "";
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (!(Path.Path == "" || Path.Path == null)) openpath = Path.Path;

			Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select an Generation Target Path");
			ofd.AllowNonFileSystemItems = false;
			ofd.EnsurePathExists = true;
			ofd.IsFolderPicker = true;
			ofd.InitialDirectory = openpath;
			ofd.Multiselect = false;
			ofd.ShowPlacesList = true;
			if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;

			FileName = Globals.GetRelativePath(System.IO.Path.GetDirectoryName(Project.AbsolutePath), ofd.FileName);
			
			Path.Path = FileName + "\\";
		}
	}

	[ValueConversion(typeof(Projects.ProjectGenerationFramework), typeof(int))]
	public class ProjectGenerationTargetFrameworkConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Projects.ProjectGenerationFramework lt = (Projects.ProjectGenerationFramework)value;
			//Uncomment to enable Silverlight support
			//if (lt == Projects.ProjectGenerationFramework.WIN8) return 0;
			//if (lt == Projects.ProjectGenerationFramework.SL50) return 1;
			//if (lt == Projects.ProjectGenerationFramework.SL40) return 2;
			//if (lt == Projects.ProjectGenerationFramework.NET45) return 3;
			//if (lt == Projects.ProjectGenerationFramework.NET40) return 4;
			//if (lt == Projects.ProjectGenerationFramework.NET40Client) return 5;
			//if (lt == Projects.ProjectGenerationFramework.NET35) return 6;
			//if (lt == Projects.ProjectGenerationFramework.NET35Client) return 7;
			//if (lt == Projects.ProjectGenerationFramework.NET30) return 8;
			if (lt == Projects.ProjectGenerationFramework.WIN8) return 0;
			if (lt == Projects.ProjectGenerationFramework.NET45) return 1;
			if (lt == Projects.ProjectGenerationFramework.NET40) return 2;
			if (lt == Projects.ProjectGenerationFramework.NET40Client) return 3;
			if (lt == Projects.ProjectGenerationFramework.NET35) return 4;
			if (lt == Projects.ProjectGenerationFramework.NET35Client) return 5;
			if (lt == Projects.ProjectGenerationFramework.NET30) return 6;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			//Uncomment to enable Silverlight support
			//if (lt == 0) return Projects.ProjectGenerationFramework.WIN8;
			//if (lt == 1) return Projects.ProjectGenerationFramework.SL50;
			//if (lt == 2) return Projects.ProjectGenerationFramework.SL40;
			//if (lt == 3) return Projects.ProjectGenerationFramework.NET45;
			//if (lt == 4) return Projects.ProjectGenerationFramework.NET40;
			//if (lt == 5) return Projects.ProjectGenerationFramework.NET40Client;
			//if (lt == 6) return Projects.ProjectGenerationFramework.NET35;
			//if (lt == 7) return Projects.ProjectGenerationFramework.NET35Client;
			//if (lt == 8) return Projects.ProjectGenerationFramework.NET30;
			if (lt == 0) return Projects.ProjectGenerationFramework.WIN8;
			if (lt == 1) return Projects.ProjectGenerationFramework.NET45;
			if (lt == 2) return Projects.ProjectGenerationFramework.NET40;
			if (lt == 3) return Projects.ProjectGenerationFramework.NET40Client;
			if (lt == 4) return Projects.ProjectGenerationFramework.NET35;
			if (lt == 5) return Projects.ProjectGenerationFramework.NET35Client;
			if (lt == 6) return Projects.ProjectGenerationFramework.NET30;
			return Projects.ProjectGenerationFramework.NET45;
		}
	}

	[ValueConversion(typeof(Projects.ProjectGenerationFramework), typeof(string))]
	public class ProjectGenerationTargetFrameworkNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Projects.ProjectGenerationFramework lt = (Projects.ProjectGenerationFramework)value;
			if (lt == Projects.ProjectGenerationFramework.WIN8) return "Windows 8 Runtime";
			//Uncomment to enable Silverlight support
			//if (lt == Projects.ProjectGenerationFramework.SL50) return "Silverlight 5.0";
			//if (lt == Projects.ProjectGenerationFramework.SL40) return "Silverlight 4.0";
			if (lt == Projects.ProjectGenerationFramework.NET45) return ".NET Framework 4.5";
			if (lt == Projects.ProjectGenerationFramework.NET40) return ".NET Framework 4.0";
			if (lt == Projects.ProjectGenerationFramework.NET40Client) return ".NET Framework 4.0 Client Profile";
			if (lt == Projects.ProjectGenerationFramework.NET35) return ".NET Framework 3.5";
			if (lt == Projects.ProjectGenerationFramework.NET35Client) return ".NET Framework 3.5 Client Profile";
			if (lt == Projects.ProjectGenerationFramework.NET30) return ".NET Framework 3.0";
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}