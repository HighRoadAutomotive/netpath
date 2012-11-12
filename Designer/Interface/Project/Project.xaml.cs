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
		public Projects.Project Settings { get { return (Projects.Project)GetValue(SettingsProperty); } set { SetValue(SettingsProperty, value); } }
		public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register("Settings", typeof(Projects.Project), typeof(Project));

		public Project(Projects.Project Project)
		{
			Settings = Project;

			InitializeComponent();

			Settings.ServerGenerationTargets.CollectionChanged += ServerOutputPaths_CollectionChanged;
			Settings.ClientGenerationTargets.CollectionChanged += ClientOutputPaths_CollectionChanged;
			ServerOutputPaths_CollectionChanged(null, null);
			ClientOutputPaths_CollectionChanged(null, null);
		}

		#region - Project -

		private void ProjectNamespaceURI_TextChanged(object sender, TextChangedEventArgs e)
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

		private void DeleteSelectedReference_Click(object sender, RoutedEventArgs w)
		{
			DependencyProject to = null;
			var clickedListBoxItem = Globals.GetVisualParent<ListBoxItem>(sender);
			if (clickedListBoxItem != null) { to = clickedListBoxItem.Content as DependencyProject; }

			if (to == null) return;
			DialogService.ShowMessageDialog(Settings, "Delete Project Reference", "Are you sure you want to delete the '" + to.Project.Name + "' project reference?", new DialogAction("Yes", () => Settings.DependencyProjects.Remove(to), true), new DialogAction("No", false, true));
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

		private void ExternalType_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				AddExternalType_Click(null, null);
		}

		private void AddExternalType_Click(object sender, RoutedEventArgs e)
		{
			var npun = new DataType(ExternalType.Text, DataTypeMode.Class);
			Settings.ExternalTypes.Add(npun);
			ExternalType.Text = "";
		}

		private void DeleteSelectedExternalType_Click(object sender, RoutedEventArgs e)
		{
			DataType to = null;
			var clickedListBoxItem = Globals.GetVisualParent<ListBoxItem>(sender);
			if (clickedListBoxItem != null) { to = clickedListBoxItem.Content as DataType; }
			if (to == null) return;

			DialogService.ShowMessageDialog(Settings, "Remove External Type", "Are you sure you want to remove the '" + to.Name + "' External Type?", new DialogAction("Yes", () => Settings.ExternalTypes.Remove(to), true), new DialogAction("No", false, true));
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

			OutputPath.Text = Globals.GetRelativePath(Settings.AbsolutePath, ofd.FileName);
		}

		private void ServerOutputAdd_Click(object sender, RoutedEventArgs e)
		{
			Settings.ServerGenerationTargets.Add(new ProjectGenerationTarget(Settings.ID, OutputPath.Text, true));
			OutputPath.Text = "";
		}

		private void ClientOutputAdd_Click(object sender, RoutedEventArgs e)
		{
			Settings.ClientGenerationTargets.Add(new ProjectGenerationTarget(Settings.ID, OutputPath.Text, false));
			OutputPath.Text = "";
		}

		private void ServerOutputPaths_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			ServerOutputPathsList.Children.Clear();
			foreach (ProjectGenerationTarget pop in Settings.ServerGenerationTargets)
				ServerOutputPathsList.Children.Add(new GenerationTarget(Settings, pop));
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

	[ValueConversion(typeof(DataTypeMode), typeof(bool))]
	public class BoolDataTypeModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (DataTypeMode)value;
			var dt = (DataTypeMode)parameter;
			if (lt == DataTypeMode.Enum && dt == DataTypeMode.Enum) return true;
			if (lt == DataTypeMode.Class && dt == DataTypeMode.Class) return true;
			if (lt == DataTypeMode.Struct && dt == DataTypeMode.Struct) return true;
			if (lt == DataTypeMode.Interface && dt == DataTypeMode.Interface) return true;
			if (lt == DataTypeMode.Collection && dt == DataTypeMode.Collection) return true;
			if (lt == DataTypeMode.Dictionary && dt == DataTypeMode.Dictionary) return true;
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (bool) value;
			var dt = (DataTypeMode) parameter;
			if (lt && dt == DataTypeMode.Enum) return DataTypeMode.Enum;
			if (lt && dt == DataTypeMode.Class) return DataTypeMode.Class;
			if (lt && dt == DataTypeMode.Struct) return DataTypeMode.Struct;
			if (lt && dt == DataTypeMode.Interface) return DataTypeMode.Interface;
			if (lt && dt == DataTypeMode.Collection) return DataTypeMode.Collection;
			if (lt && dt == DataTypeMode.Dictionary) return DataTypeMode.Dictionary;
			return DataTypeMode.Class;
		}
	}
}