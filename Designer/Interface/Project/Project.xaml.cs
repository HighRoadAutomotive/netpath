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

		public Projects.ServiceBinding SelectedBinding { get; set; }
		public Projects.BindingSecurity SelectedSecurity { get; set; }
		public Projects.Host SelectedHost { get; set; }

		public Project(Projects.Project Project)
		{
			Globals.ProjectScreens.Add(this);
			this.Settings = Project;
			this.SelectedBinding = null;
			this.SelectedSecurity = null;
			this.SelectedHost = null;

			InitializeComponent();

			DependencyItems.ItemsSource = Settings.DependencyProjects;
			UsingList.ItemsSource = Settings.UsingNamespaces;
			ReferenceItems.ItemsSource = Settings.References;

			BindingsList.ItemsSource = Settings.Bindings;
			SecurityList.ItemsSource = Settings.Security;
			HostsList.ItemsSource = Settings.Hosts;
			Settings.ServerOutputPaths.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ServerOutputPaths_CollectionChanged);
			Settings.ClientOutputPaths.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ClientOutputPaths_CollectionChanged);
			ServerOutputPaths_CollectionChanged(null, null);
			ClientOutputPaths_CollectionChanged(null, null);

			//This provides upgrade path support from older project files. DO NOT REMOVE!
			foreach (Projects.ProjectOutputPath POP in Settings.ServerOutputPaths)
				POP.SetOwnerProject(Settings);
			foreach (Projects.ProjectOutputPath POP in Settings.ClientOutputPaths)
				POP.SetOwnerProject(Settings);

			if (Project.IsDependencyProject == true)
			{
				FrameworkSelect.IsEnabled = false;
				ReferencesGroup.Visibility = System.Windows.Visibility.Collapsed;
				BuildTab.IsEnabled = false;
			}
		}

		#region - Project -

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			AssemblyCOMGuid.Text = Settings.AssemblyCOMGuid.ToString();
		}

		private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			AssemblyCOMGuid.Text = "";
		}

		private void FrameworkSelect_Click(object sender, RoutedEventArgs e)
		{
			FrameworkSelect.ContextMenu.Visibility = System.Windows.Visibility.Visible;
			FrameworkSelect.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
			FrameworkSelect.ContextMenu.PlacementTarget = FrameworkSelect;
			FrameworkSelect.ContextMenu.IsOpen = true;
		}

		private void FrameworkSelect_MouseDown(object sender, MouseButtonEventArgs e)
		{
			FrameworkSelect.ContextMenu.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void DependencyAdd_Click(object sender, RoutedEventArgs e)
		{
			AddDependency NAD = new AddDependency(Settings);
			NAD.ShowDialog();
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

		private void ReferenceAdd_Click(object sender, RoutedEventArgs e)
		{
			AddReference NAR = new AddReference(Settings);
			NAR.Show();
		}

		private void ReferenceRemove_Click(object sender, RoutedEventArgs e)
		{
			List<Projects.Reference> Delete = new List<Projects.Reference>();
			foreach (Projects.Reference R in ReferenceItems.SelectedItems)
			{
				Delete.Add(R);
			}
			foreach (Projects.Reference R in Delete)
			{
				Settings.References.Remove(R);
			}
			Delete.Clear();
		}

		#endregion

		#region - Build -

		private void ServerOutputBrowse_Click(object sender, RoutedEventArgs e)
		{
			string FileName = "";
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (!(Globals.ProjectSpace.FileName == "" || Globals.ProjectSpace.FileName == null)) openpath = Globals.ProjectSpace.FileName;
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

			ServerOutputPath.Text = FileName + "\\";
		}

		private void ServerOutputAdd_Click(object sender, RoutedEventArgs e)
		{
			Settings.ServerOutputPaths.Add(new Projects.ProjectOutputPath(Settings.ID, ServerOutputPath.Text));
			ServerOutputPath.Text = "";
		}

		private void ServerOutputPaths_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			ServerOutputPathsList.Children.Clear();
			foreach (Projects.ProjectOutputPath POP in Settings.ServerOutputPaths)
				if (POP.UserProfileID == Globals.UserProfile.ID)
					ServerOutputPathsList.Children.Add(new OutputPath(POP));
		}

		private void DeleteSelectedExcludedType_Click(object sender, RoutedEventArgs e)
		{
			string TSB = null;
			ListBoxItem clickedListBoxItem = Globals.GetVisualParent<ListBoxItem>(sender);
			if (clickedListBoxItem != null) { TSB = clickedListBoxItem.Content as string; }

			if (Prospective.Controls.MessageBox.Show("Are you sure you want to remove '" + TSB + "' the excluded types list?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				int SI = Settings.ServiceExcludedTypes.IndexOf(TSB);
				Settings.ServiceExcludedTypes.Remove(TSB);
				if (SI >= Settings.ServiceExcludedTypes.Count)
					ExcludedTypesList.SelectedIndex = Settings.ServiceExcludedTypes.Count - 1;
				else
					ExcludedTypesList.SelectedIndex = SI + 1;
			}
		}

		private void DeleteSelectedCollectionType_Click(object sender, RoutedEventArgs e)
		{
			string TSB = null;
			ListBoxItem clickedListBoxItem = Globals.GetVisualParent<ListBoxItem>(sender);
			if (clickedListBoxItem != null) { TSB = clickedListBoxItem.Content as string; }

			if (Prospective.Controls.MessageBox.Show("Are you sure you want to remove '" + TSB + "' the collection types list?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				int SI = Settings.ServiceCollectionTypes.IndexOf(TSB);
				Settings.ServiceCollectionTypes.Remove(TSB);
				if (SI >= Settings.ServiceCollectionTypes.Count)
					CollectionTypesList.SelectedIndex = Settings.ServiceCollectionTypes.Count - 1;
				else
					CollectionTypesList.SelectedIndex = SI + 1;
			}
		}

		private void ServiceExcludedTypeAdd_Click(object sender, RoutedEventArgs e)
		{
			Settings.ServiceExcludedTypes.Add(ServerServiceExcludedType.Text);
			ServerServiceExcludedType.Text = "";
		}

		private void ClientOutputBrowse_Click(object sender, RoutedEventArgs e)
		{
			string FileName = "";
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (!(Globals.ProjectSpace.FileName == "" || Globals.ProjectSpace.FileName == null)) openpath = Globals.ProjectSpace.FileName;
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

			ClientOutputPath.Text = FileName + "\\";
		}

		private void ClientOutputAdd_Click(object sender, RoutedEventArgs e)
		{
			Settings.ClientOutputPaths.Add(new Projects.ProjectOutputPath(Settings.ID, ClientOutputPath.Text));
			ClientOutputPath.Text = "";
		}

		private void ClientOutputPaths_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			ClientOutputPathsList.Children.Clear();
			foreach (Projects.ProjectOutputPath POP in Settings.ClientOutputPaths)
				if (POP.UserProfileID == Globals.UserProfile.ID)
					ClientOutputPathsList.Children.Add(new OutputPath(POP));
		}

		private void CollectionTypeAdd_Click(object sender, RoutedEventArgs e)
		{
			Settings.ServiceCollectionTypes.Add(ClientCollectionType.Text);
			ClientCollectionType.Text = "";
		}

		#endregion

		#region - Bindings -

		public void DeleteBinding()
		{
			if (BindingsList.SelectedItem == null) return;

			Projects.ServiceBinding t = (Projects.ServiceBinding)BindingsList.SelectedItem;
			if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + t.Name + "' binding?", "Delete Binding?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				Settings.Bindings.Remove(t);
				if (BindingsList.SelectedIndex >= 0) BindingsList.SelectedIndex = 0;
			}
		}

		private void BindingsList_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
				DeleteBinding();
		}

		private void BindingsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (BindingsList.SelectedItem != null)
			{
				BindingContentEmpty.Visibility = System.Windows.Visibility.Collapsed;
				Type valueType = BindingsList.SelectedItem.GetType();
				if (valueType == typeof(Projects.ServiceBindingBasicHTTP)) BindingContent.Content = new Bindings.BasicHTTP(BindingsList.SelectedItem as Projects.ServiceBindingBasicHTTP);
				if (valueType == typeof(Projects.ServiceBindingWSHTTP)) BindingContent.Content = new Bindings.WSHTTP(BindingsList.SelectedItem as Projects.ServiceBindingWSHTTP);
				if (valueType == typeof(Projects.ServiceBindingWS2007HTTP)) BindingContent.Content = new Bindings.WS2007HTTP(BindingsList.SelectedItem as Projects.ServiceBindingWS2007HTTP);
				if (valueType == typeof(Projects.ServiceBindingWSDualHTTP)) BindingContent.Content = new Bindings.WSDualHTTP(BindingsList.SelectedItem as Projects.ServiceBindingWSDualHTTP);
				if (valueType == typeof(Projects.ServiceBindingWSFederationHTTP)) BindingContent.Content = new Bindings.WSFederationHTTP(BindingsList.SelectedItem as Projects.ServiceBindingWSFederationHTTP);
				if (valueType == typeof(Projects.ServiceBindingWS2007FederationHTTP)) BindingContent.Content = new Bindings.WS2007FederationHTTP(BindingsList.SelectedItem as Projects.ServiceBindingWS2007FederationHTTP);
				if (valueType == typeof(Projects.ServiceBindingTCP)) BindingContent.Content = new Bindings.TCP(BindingsList.SelectedItem as Projects.ServiceBindingTCP);
				if (valueType == typeof(Projects.ServiceBindingNamedPipe)) BindingContent.Content = new Bindings.NamedPipe(BindingsList.SelectedItem as Projects.ServiceBindingNamedPipe);
				if (valueType == typeof(Projects.ServiceBindingMSMQ)) BindingContent.Content = new Bindings.MSMQ(BindingsList.SelectedItem as Projects.ServiceBindingMSMQ);
				if (valueType == typeof(Projects.ServiceBindingPeerTCP)) BindingContent.Content = new Bindings.PeerTCP(BindingsList.SelectedItem as Projects.ServiceBindingPeerTCP);
				if (valueType == typeof(Projects.ServiceBindingWebHTTP)) BindingContent.Content = new Bindings.WebHTTP(BindingsList.SelectedItem as Projects.ServiceBindingWebHTTP);
				if (valueType == typeof(Projects.ServiceBindingMSMQIntegration)) BindingContent.Content = new Bindings.MSMQIntegration(BindingsList.SelectedItem as Projects.ServiceBindingMSMQIntegration);
				SelectedBinding = (Projects.ServiceBinding)BindingsList.SelectedItem;
			}
			else
			{
				BindingContentEmpty.Visibility = System.Windows.Visibility.Visible;
				BindingContent.Content = null;
				SelectedBinding = null;
			}
		}

		#endregion

		#region - Security -

		public void DeleteSecurity()
		{
			if (SecurityList.SelectedItem == null) return;

			Projects.BindingSecurity t = (Projects.BindingSecurity)SecurityList.SelectedItem;
			if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + t.Name + "' binding security?", "Delete Binding Security?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				Settings.Security.Remove(t);
				if (SecurityList.SelectedIndex >= 0) SecurityList.SelectedIndex = 0;
			}
		}

		private void SecurityList_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
				DeleteSecurity();
		}

		private void SecurityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (SecurityList.SelectedItem != null)
			{
				SecurityContentEmpty.Visibility = System.Windows.Visibility.Collapsed;
				Type valueType = SecurityList.SelectedItem.GetType();
				if (valueType == typeof(Projects.BindingSecurityBasicHTTP)) SecurityContent.Content = new Security.BasicHTTP(SecurityList.SelectedItem as Projects.BindingSecurityBasicHTTP);
				if (valueType == typeof(Projects.BindingSecurityWSHTTP)) SecurityContent.Content = new Security.WSHTTP(SecurityList.SelectedItem as Projects.BindingSecurityWSHTTP);
				if (valueType == typeof(Projects.BindingSecurityWSDualHTTP)) SecurityContent.Content = new Security.WSDualHTTP(SecurityList.SelectedItem as Projects.BindingSecurityWSDualHTTP);
				if (valueType == typeof(Projects.BindingSecurityWSFederationHTTP)) SecurityContent.Content = new Security.WSFederationHTTP(SecurityList.SelectedItem as Projects.BindingSecurityWSFederationHTTP);
				if (valueType == typeof(Projects.BindingSecurityTCP)) SecurityContent.Content = new Security.TCP(SecurityList.SelectedItem as Projects.BindingSecurityTCP);
				if (valueType == typeof(Projects.BindingSecurityNamedPipe)) SecurityContent.Content = new Security.NamedPipe(SecurityList.SelectedItem as Projects.BindingSecurityNamedPipe);
				if (valueType == typeof(Projects.BindingSecurityMSMQ)) SecurityContent.Content = new Security.MSMQ(SecurityList.SelectedItem as Projects.BindingSecurityMSMQ);
				if (valueType == typeof(Projects.BindingSecurityPeerTCP)) SecurityContent.Content = new Security.PeerTCP(SecurityList.SelectedItem as Projects.BindingSecurityPeerTCP);
				if (valueType == typeof(Projects.BindingSecurityWebHTTP)) SecurityContent.Content = new Security.WebHTTP(SecurityList.SelectedItem as Projects.BindingSecurityWebHTTP);
				if (valueType == typeof(Projects.BindingSecurityMSMQIntegration)) SecurityContent.Content = new Security.MSMQIntegration(SecurityList.SelectedItem as Projects.BindingSecurityMSMQIntegration);
				SelectedSecurity = (Projects.BindingSecurity)SecurityList.SelectedItem;
			}
			else
			{
				SecurityContentEmpty.Visibility = System.Windows.Visibility.Visible;
				SecurityContent.Content = null;
				SelectedSecurity = null;
			}
		}
		
		#endregion

		#region - Hosts -

		public void DeleteHost()
		{
			if (HostsList.SelectedItem == null) return;

			Projects.Host t = (Projects.Host)HostsList.SelectedItem;
			if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the '" + t.Name + "' host?", "Delete Host?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				Settings.Hosts.Remove(t);
				if (HostsList.SelectedIndex >= 0) HostsList.SelectedIndex = 0;
			}
		}

		private void HostsList_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
				DeleteHost();
		}

		private void HostsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (HostsList.SelectedItem != null)
			{
				HostContentEmpty.Visibility = System.Windows.Visibility.Collapsed;
				HostContent.Content = new Host.Host((Projects.Host)HostsList.SelectedItem);
				SelectedHost = (Projects.Host)HostsList.SelectedItem;
			}
			else
			{
				HostContentEmpty.Visibility = System.Windows.Visibility.Visible;
				HostContent.Content = null;
				SelectedHost = null;
			}
		}

		#endregion

	}

	[ValueConversion(typeof(Projects.ServiceBinding), typeof(string))]
	public class ServiceBindingTypeImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "";
			Type valueType = value.GetType();
			if (valueType == typeof(Projects.ServiceBindingBasicHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png";
			if (valueType == typeof(Projects.ServiceBindingWSHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WSHTTP.png";
			if (valueType == typeof(Projects.ServiceBindingWS2007HTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WS2007HTTP.png";
			if (valueType == typeof(Projects.ServiceBindingWSDualHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WSDualHTTP.png";
			if (valueType == typeof(Projects.ServiceBindingWSFederationHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WSFederationHTTP.png";
			if (valueType == typeof(Projects.ServiceBindingWS2007FederationHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WS2007FederationHTTP.png";
			if (valueType == typeof(Projects.ServiceBindingTCP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/TCP.png";
			if (valueType == typeof(Projects.ServiceBindingNamedPipe)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/NamedPipe.png";
			if (valueType == typeof(Projects.ServiceBindingMSMQ)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQ.png";
			if (valueType == typeof(Projects.ServiceBindingPeerTCP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/PeerTCP.png";
			if (valueType == typeof(Projects.ServiceBindingWebHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WebHTTP.png";
			if (valueType == typeof(Projects.ServiceBindingMSMQIntegration)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQIntegration.png";
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(Projects.ServiceBinding), typeof(string))]
	public class ServiceBindingTypeNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "pack://application:,,,/WCFArchitect;component/Icons/Blank.png";
			Type valueType = value.GetType();
			if (valueType == typeof(Projects.ServiceBindingBasicHTTP)) return "Basic HTTP";
			if (valueType == typeof(Projects.ServiceBindingWSHTTP)) return "WS HTTP";
			if (valueType == typeof(Projects.ServiceBindingWS2007HTTP)) return "WS 2007 HTTP";
			if (valueType == typeof(Projects.ServiceBindingWSDualHTTP)) return "WS Dual HTTP";
			if (valueType == typeof(Projects.ServiceBindingWSFederationHTTP)) return "WS Federation HTTP";
			if (valueType == typeof(Projects.ServiceBindingWS2007FederationHTTP)) return "WS 2007 Federation HTTP";
			if (valueType == typeof(Projects.ServiceBindingTCP)) return "TCP";
			if (valueType == typeof(Projects.ServiceBindingNamedPipe)) return "Named Pipe";
			if (valueType == typeof(Projects.ServiceBindingMSMQ)) return "MSMQ";
			if (valueType == typeof(Projects.ServiceBindingPeerTCP)) return "Peer TCP";
			if (valueType == typeof(Projects.ServiceBindingWebHTTP)) return "Web HTTP";
			if (valueType == typeof(Projects.ServiceBindingMSMQIntegration)) return "MSMQ Integration";
			return "pack://application:,,,/WCFArchitect;component/Icons/Blank.png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(Projects.ServiceBinding), typeof(string))]
	public class BindingSecurityTypeImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "pack://application:,,,/WCFArchitect;component/Icons/Blank.png";
			Type valueType = value.GetType();
			if (valueType == typeof(Projects.BindingSecurityBasicHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png";
			if (valueType == typeof(Projects.BindingSecurityWSHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WSHTTP.png";
			if (valueType == typeof(Projects.BindingSecurityWSDualHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WSDualHTTP.png";
			if (valueType == typeof(Projects.BindingSecurityWSFederationHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WSFederationHTTP.png";
			if (valueType == typeof(Projects.BindingSecurityTCP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/TCP.png";
			if (valueType == typeof(Projects.BindingSecurityNamedPipe)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/NamedPipe.png";
			if (valueType == typeof(Projects.BindingSecurityMSMQ)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQ.png";
			if (valueType == typeof(Projects.BindingSecurityPeerTCP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/PeerTCP.png";
			if (valueType == typeof(Projects.BindingSecurityWebHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WebHTTP.png";
			if (valueType == typeof(Projects.BindingSecurityMSMQIntegration)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQIntegration.png";
			return "pack://application:,,,/WCFArchitect;component/Icons/Blank.png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(Projects.ServiceBinding), typeof(string))]
	public class BindingSecurityTypeNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "";
			Type valueType = value.GetType();
			if (valueType == typeof(Projects.BindingSecurityBasicHTTP)) return "Basic HTTP";
			if (valueType == typeof(Projects.BindingSecurityWSHTTP)) return "WS HTTP";
			if (valueType == typeof(Projects.BindingSecurityWSDualHTTP)) return "WS Dual HTTP";
			if (valueType == typeof(Projects.BindingSecurityWSFederationHTTP)) return "WS Federation HTTP";
			if (valueType == typeof(Projects.BindingSecurityTCP)) return "TCP";
			if (valueType == typeof(Projects.BindingSecurityNamedPipe)) return "Named Pipe";
			if (valueType == typeof(Projects.BindingSecurityMSMQ)) return "MSMQ";
			if (valueType == typeof(Projects.BindingSecurityPeerTCP)) return "Peer TCP";
			if (valueType == typeof(Projects.BindingSecurityWebHTTP)) return "Web HTTP";
			if (valueType == typeof(Projects.BindingSecurityMSMQIntegration)) return "MSMQ Integration";
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
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

	[ValueConversion(typeof(Projects.ProjectCollectionType), typeof(int))]
	public class ProjectCollectionTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Projects.ProjectCollectionType lt = (Projects.ProjectCollectionType)value;
			if (lt == Projects.ProjectCollectionType.List) return 0;
			if (lt == Projects.ProjectCollectionType.SynchronizedCollection) return 1;
			if (lt == Projects.ProjectCollectionType.Collection) return 2;
			if (lt == Projects.ProjectCollectionType.ObservableCollection) return 3;
			if (lt == Projects.ProjectCollectionType.BindingList) return 4;
			if (lt == Projects.ProjectCollectionType.Array) return 5;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return Projects.ProjectCollectionType.List;
			if (lt == 1) return Projects.ProjectCollectionType.SynchronizedCollection;
			if (lt == 2) return Projects.ProjectCollectionType.Collection;
			if (lt == 3) return Projects.ProjectCollectionType.ObservableCollection;
			if (lt == 4) return Projects.ProjectCollectionType.BindingList;
			if (lt == 5) return Projects.ProjectCollectionType.Array;
			return Projects.ProjectCollectionType.List;
		}
	}

	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class BoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool val = (bool)value;

			return val ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}