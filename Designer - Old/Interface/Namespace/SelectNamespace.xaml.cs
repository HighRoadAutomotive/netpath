using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
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

namespace WCFArchitect.Interface.Namespace
{
	internal partial class SelectNamespace : Window
	{
		private Projects.Project Project { get; set; }
		public Projects.Namespace SelectedNamespace { get; set; }

		public SelectNamespace(Projects.Project Project)
		{
			InitializeComponent();

			this.Project = Project;
			ProjectNamespaceView.ItemsSource = Project.Namespaces;
		}

		private void Add_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}

		private void CloseWindow_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		private void ProjectNamespaceView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (ProjectNamespaceView.SelectedItem == null)
			{
				SelectedNamespace = null;
				return;
			}

			SelectedNamespace = ProjectNamespaceView.SelectedItem as Projects.Namespace;
			NamespacePath.Text = SelectedNamespace.FullName;
		}
	}
}