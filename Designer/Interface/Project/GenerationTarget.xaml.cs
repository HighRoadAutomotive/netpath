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

namespace NETPath.Interface.Project
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

			if (Path.IsEnabled) EnableImage.Source = new BitmapImage(new Uri("pack://application:,,,/NETPath;component/Icons/X16/ReferenceAdded.png"));
			else EnableImage.Source = new BitmapImage(new Uri("pack://application:,,,/NETPath;component/Icons/X16/NotAvailable.png"));
		}

		private void Enable_Click(object sender, RoutedEventArgs e)
		{
			Path.IsEnabled = !Path.IsEnabled;
			if (Path.IsEnabled) EnableImage.Source = new BitmapImage(new Uri("pack://application:,,,/NETPath;component/Icons/X16/ReferenceAdded.png"));
			else EnableImage.Source = new BitmapImage(new Uri("pack://application:,,,/NETPath;component/Icons/X16/NotAvailable.png"));
		}

		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			DialogService.ShowMessageDialog(Project, "Confirm Generation Target Deletion", "Are you sure you want to delete the generation target: " + Path.Path,
				new DialogAction("Yes", () => { Project.ServerGenerationTargets.Remove(Path); Project.ClientGenerationTargets.Remove(Path); }, true), new DialogAction("No", false, true));
		}

		private void OutputBrowse_Click(object sender, RoutedEventArgs e)
		{
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (!string.IsNullOrEmpty(Project.AbsolutePath)) openpath = System.IO.Path.GetDirectoryName(Project.AbsolutePath);

			var ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select an Output Path")
			{
				AllowNonFileSystemItems = false,
				EnsurePathExists = true,
				IsFolderPicker = false,
				DefaultExtension = "cs",
				InitialDirectory = openpath,
				Multiselect = false,
				ShowPlacesList = true
			};
			if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;

			Path.Path = Globals.GetRelativePath(Project.AbsolutePath, ofd.FileName);
		}
	}

	[ValueConversion(typeof(Projects.ProjectGenerationFramework), typeof(int))]
	public class ProjectGenerationTargetFrameworkConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Projects.ProjectGenerationFramework)value;
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