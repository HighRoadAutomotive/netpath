using System;
using System.Collections.ObjectModel;
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
	internal partial class Project : Grid
	{
		public Projects.Project Settings { get; private set; }

		public Project(Projects.Project Project)
		{
			this.Settings = Project;

			InitializeComponent();

			DependencyItems.ItemsSource = Settings.DependencyProjects;
			UsingList.ItemsSource = Settings.UsingNamespaces;

			Settings.ServerGenerationTargets.CollectionChanged += ServerOutputPaths_CollectionChanged;
			Settings.ClientGenerationTargets.CollectionChanged += ClientOutputPaths_CollectionChanged;
			ServerOutputPaths_CollectionChanged(null, null);
			ClientOutputPaths_CollectionChanged(null, null);
		}

		#region - Project -

		private void DependencyAdd_Click(object sender, RoutedEventArgs e)
		{
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (!(Settings.AbsolutePath == "" || Settings.AbsolutePath == null)) openpath = System.IO.Path.GetDirectoryName(Settings.AbsolutePath);

			//Select the project
			Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select Poject");
			ofd.AllowNonFileSystemItems = false;
			ofd.EnsurePathExists = true;
			ofd.IsFolderPicker = false;
			ofd.InitialDirectory = openpath;
			ofd.Multiselect = false;
			ofd.ShowPlacesList = true;
			if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;

			//Load the project and add it to the dependency projects list.
			Projects.Project op = Projects.Project.Open(Globals.SolutionPath, Globals.GetRelativePath(Globals.SolutionPath, ofd.FileName));
			Projects.DependencyProject ndp = new Projects.DependencyProject();
			ndp.Path = Globals.GetRelativePath(Globals.SolutionPath, ofd.FileName);
			ndp.Project = op;
			Settings.DependencyProjects.Add(ndp);
		}

		private void DependencyRemove_Click(object sender, RoutedEventArgs e)
		{
			List<Projects.DependencyProject> Delete = new List<Projects.DependencyProject>();
			foreach (Projects.DependencyProject R in DependencyItems.SelectedItems)
			{
				Delete.Add(R);
			}
			foreach (Projects.DependencyProject R in Delete)
			{
				Settings.DependencyProjects.Remove(R);
			}
			Delete.Clear();
		}

		private void UsingNamespace_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				AddUsingNamespace_Click(null, null);
		}

		private void AddUsingNamespace_Click(object sender, RoutedEventArgs e)
		{
			Projects.ProjectUsingNamespace NPUN = new Projects.ProjectUsingNamespace(UsingNamespace.Text);
			Settings.UsingNamespaces.Add(NPUN);
			UsingNamespace.Text = "";
		}

		private void DeleteSelectedUsingItem_Click(object sender, RoutedEventArgs e)
		{
			Projects.ProjectUsingNamespace TO = null;
			ListBoxItem clickedListBoxItem = Globals.GetVisualParent<ListBoxItem>(sender);
			if (clickedListBoxItem != null) { TO = clickedListBoxItem.Content as Projects.ProjectUsingNamespace; }

			if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + TO.Namespace + "' Namespace?", "Delete Using Namespace", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				Settings.UsingNamespaces.Remove(TO);
			}
		}

		#endregion

		#region - Build -

		private void ServerOutputBrowse_Click(object sender, RoutedEventArgs e)
		{
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (!(Settings.AbsolutePath == "" || Settings.AbsolutePath == null)) openpath = System.IO.Path.GetDirectoryName(Settings.AbsolutePath);

			Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select an Output Path");
			ofd.AllowNonFileSystemItems = false;
			ofd.EnsurePathExists = true;
			ofd.IsFolderPicker = true;
			ofd.InitialDirectory = openpath;
			ofd.Multiselect = false;
			ofd.ShowPlacesList = true;
			if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;

			ServerOutputPath.Text = Globals.GetRelativePath(Globals.SolutionPath, ofd.FileName + "\\");
		}

		private void ServerOutputAdd_Click(object sender, RoutedEventArgs e)
		{
			Settings.ServerGenerationTargets.Add(new Projects.ProjectGenerationTarget(Settings.ID, ServerOutputPath.Text));
			ServerOutputPath.Text = "";
		}

		private void ServerOutputPaths_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			ServerOutputPathsList.Children.Clear();
			foreach (Projects.ProjectGenerationTarget POP in Settings.ServerGenerationTargets)
				ServerOutputPathsList.Children.Add(new GenerationTarget(Settings, POP));
		}

		private void ClientOutputBrowse_Click(object sender, RoutedEventArgs e)
		{
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (!(Settings.AbsolutePath == "" || Settings.AbsolutePath == null)) openpath = Settings.AbsolutePath;

			Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select an Output Path");
			ofd.AllowNonFileSystemItems = false;
			ofd.EnsurePathExists = true;
			ofd.IsFolderPicker = true;
			ofd.InitialDirectory = openpath;
			ofd.Multiselect = false;
			ofd.ShowPlacesList = true;
			if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;

			ClientOutputPath.Text = Globals.GetRelativePath(Globals.SolutionPath, ofd.FileName + "\\");
		}

		private void ClientOutputAdd_Click(object sender, RoutedEventArgs e)
		{
			Settings.ClientGenerationTargets.Add(new Projects.ProjectGenerationTarget(Settings.ID, ClientOutputPath.Text));
			ClientOutputPath.Text = "";
		}

		private void ClientOutputPaths_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			ClientOutputPathsList.Children.Clear();
			foreach (Projects.ProjectGenerationTarget POP in Settings.ClientGenerationTargets)
				ClientOutputPathsList.Children.Add(new GenerationTarget(Settings, POP));
		}

		#endregion

	}

	[ValueConversion(typeof(Projects.ProjectServiceSerializerType), typeof(int))]
	public class ProjectServiceSerializerTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Projects.ProjectServiceSerializerType lt = (Projects.ProjectServiceSerializerType)value;
			if (lt == Projects.ProjectServiceSerializerType.Auto) return 0;
			if (lt == Projects.ProjectServiceSerializerType.DataContract) return 1;
			if (lt == Projects.ProjectServiceSerializerType.XML) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return Projects.ProjectServiceSerializerType.Auto;
			if (lt == 1) return Projects.ProjectServiceSerializerType.DataContract;
			if (lt == 2) return Projects.ProjectServiceSerializerType.XML;
			return Projects.ProjectServiceSerializerType.Auto;
		}
	}

	[ValueConversion(typeof(Projects.ProjectCollectionSerializationOverride), typeof(int))]
	public class ProjectCollectionSerializationOverrideConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Projects.ProjectCollectionSerializationOverride lt = (Projects.ProjectCollectionSerializationOverride)value;
			if (lt == Projects.ProjectCollectionSerializationOverride.List) return 1;
			if (lt == Projects.ProjectCollectionSerializationOverride.Array) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 1) return Projects.ProjectCollectionSerializationOverride.List;
			if (lt == 2) return Projects.ProjectCollectionSerializationOverride.Array;
			return Projects.ProjectCollectionSerializationOverride.None;
		}
	}
}