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
using Prospective.Controls.Dialogs;
using WCFArchitect.Projects;

namespace WCFArchitect.Interface.Project
{
	internal partial class Project : Grid
	{
		public Projects.Project Settings { get; private set; }

		public Project(Projects.Project Project)
		{
			Settings = Project;

			InitializeComponent();

			DependencyItems.ItemsSource = Settings.DependencyProjects;
			UsingList.ItemsSource = Settings.UsingNamespaces;

			Settings.ServerGenerationTargets.CollectionChanged += ServerOutputPaths_CollectionChanged;
			Settings.ClientGenerationTargets.CollectionChanged += ClientOutputPaths_CollectionChanged;
			ServerOutputPaths_CollectionChanged(null, null);
			ClientOutputPaths_CollectionChanged(null, null);
		}

		#region - Project -

		private void txtProjectNamespaceURI_TextChanged(object Sender, TextChangedEventArgs E)
		{
			foreach (Projects.Namespace ns in Settings.Namespace.Children)
				ns.UpdateURI();
		}

		private void DependencyAdd_Click(object sender, RoutedEventArgs e)
		{
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (!string.IsNullOrEmpty(Settings.AbsolutePath)) openpath = System.IO.Path.GetDirectoryName(Settings.AbsolutePath);

			//Select the project
			var ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select Project")
				          {
							  DefaultExtension = ".wap",
					          AllowNonFileSystemItems = false,
					          EnsurePathExists = true,
					          IsFolderPicker = false,
					          InitialDirectory = openpath,
					          Multiselect = false,
					          ShowPlacesList = true
				          };
			if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;

			//Load the project and add it to the dependency projects list.
			Projects.Project op = Projects.Project.Open(Globals.SolutionPath, Globals.GetRelativePath(Globals.SolutionPath, ofd.FileName));
			var ndp = new DependencyProject {Path = Globals.GetRelativePath(Globals.SolutionPath, ofd.FileName), Project = op};

			if (Settings.ID == ndp.ProjectID)
			{
				DialogService.ShowMessageDialog(Settings, "Invalid Project Selected.", "You cannot add a project as a dependency of itself.");
				return;
			}

			if (Settings.HasDependencyProject(ndp.ProjectID))
			{
				DialogService.ShowMessageDialog(Settings, "Invalid Project Selected.", "The project you selected cannot be added. It is already available as a dependency project.");
				return;
			}

			Settings.DependencyProjects.Add(ndp);
		}

		private void DependencyRemove_Click(object sender, RoutedEventArgs e)
		{
			var delete = DependencyItems.SelectedItems.Cast<DependencyProject>().ToList();
			foreach (DependencyProject r in delete)
				Settings.DependencyProjects.Remove(r);
			delete.Clear();
		}

		private void UsingNamespace_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				AddUsingNamespace_Click(null, null);
		}

		private void AddUsingNamespace_Click(object sender, RoutedEventArgs e)
		{
			var npun = new ProjectUsingNamespace(UsingNamespace.Text);
			Settings.UsingNamespaces.Add(npun);
			UsingNamespace.Text = "";
		}

		private void DeleteSelectedUsingItem_Click(object sender, RoutedEventArgs e)
		{
			ProjectUsingNamespace to = null;
			var clickedListBoxItem = Globals.GetVisualParent<ListBoxItem>(sender);
			if (clickedListBoxItem != null) { to = clickedListBoxItem.Content as ProjectUsingNamespace; }

			if (to == null) return;
			DialogService.ShowMessageDialog(Settings, "Delete Using Namespace", "Are you sure you want to delete the '" + to.Namespace + "' Namespace?", new DialogAction("Yes", () => Settings.UsingNamespaces.Remove(to), true), new DialogAction("No", false, true));
		}

		#endregion

		#region - Build -

		private void ServerOutputBrowse_Click(object sender, RoutedEventArgs e)
		{
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (!string.IsNullOrEmpty(Settings.AbsolutePath)) openpath = System.IO.Path.GetDirectoryName(Settings.AbsolutePath);

			var ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select an Output Path")
				          {
					          AllowNonFileSystemItems = false,
					          EnsurePathExists = true,
					          IsFolderPicker = true,
					          InitialDirectory = openpath,
					          Multiselect = false,
					          ShowPlacesList = true
				          };
			if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;

			ServerOutputPath.Text = Globals.GetRelativePath(Settings.AbsolutePath, ofd.FileName + "\\");
		}

		private void ServerOutputAdd_Click(object sender, RoutedEventArgs e)
		{
			Settings.ServerGenerationTargets.Add(new ProjectGenerationTarget(Settings.ID, ServerOutputPath.Text, true));
			ServerOutputPath.Text = "";
		}

		private void ServerOutputPaths_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			ServerOutputPathsList.Children.Clear();
			foreach (ProjectGenerationTarget pop in Settings.ServerGenerationTargets)
				ServerOutputPathsList.Children.Add(new GenerationTarget(Settings, pop));
		}

		private void ClientOutputBrowse_Click(object sender, RoutedEventArgs e)
		{
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (!string.IsNullOrEmpty(Settings.AbsolutePath)) openpath = Settings.AbsolutePath;

			var ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select an Output Path")
				          {
					          AllowNonFileSystemItems = false,
					          EnsurePathExists = true,
					          IsFolderPicker = true,
					          InitialDirectory = openpath,
					          Multiselect = false,
					          ShowPlacesList = true
				          };
			if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;

			ClientOutputPath.Text = Globals.GetRelativePath(Settings.AbsolutePath, ofd.FileName + "\\");
		}

		private void ClientOutputAdd_Click(object sender, RoutedEventArgs e)
		{
			Settings.ClientGenerationTargets.Add(new ProjectGenerationTarget(Settings.ID, ClientOutputPath.Text, false));
			ClientOutputPath.Text = "";
		}

		private void ClientOutputPaths_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			ClientOutputPathsList.Children.Clear();
			foreach (ProjectGenerationTarget pop in Settings.ClientGenerationTargets)
				ClientOutputPathsList.Children.Add(new GenerationTarget(Settings, pop));
		}

		#endregion
	}

	[ValueConversion(typeof(ProjectServiceSerializerType), typeof(int))]
	public class ProjectServiceSerializerTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (ProjectServiceSerializerType)value;
			if (lt == ProjectServiceSerializerType.Auto) return 0;
			if (lt == ProjectServiceSerializerType.DataContract) return 1;
			if (lt == ProjectServiceSerializerType.XML) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return ProjectServiceSerializerType.Auto;
			if (lt == 1) return ProjectServiceSerializerType.DataContract;
			if (lt == 2) return ProjectServiceSerializerType.XML;
			return ProjectServiceSerializerType.Auto;
		}
	}

	[ValueConversion(typeof(ProjectCollectionSerializationOverride), typeof(int))]
	public class ProjectCollectionSerializationOverrideConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (ProjectCollectionSerializationOverride)value;
			if (lt == ProjectCollectionSerializationOverride.List) return 1;
			if (lt == ProjectCollectionSerializationOverride.Array) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 1) return ProjectCollectionSerializationOverride.List;
			if (lt == 2) return ProjectCollectionSerializationOverride.Array;
			return ProjectCollectionSerializationOverride.None;
		}
	}
}