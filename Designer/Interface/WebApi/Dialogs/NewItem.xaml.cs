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
using System.Windows.Navigation;
using System.Windows.Shapes;
using EllipticBit.Controls.WPF.Dialogs;
using NETPath.Interface;
using NETPath.Projects.WebApi;

namespace NETPath.Interface.WebApi.Dialogs
{
	public partial class NewItem : Grid, IDialogContent
	{
		public System.Collections.ObjectModel.ObservableCollection<DialogAction> Actions { get; set; }
		public ContentDialog Host { get; set; }
		public void SetFocus()
		{
			NewItemTypesList.Focus();
		}

		private bool IsNamespaceListUpdating { get; set; }
		public WebApiProject ActiveProject { get; private set; }
		private Action<Projects.OpenableDocument> OpenProjectItem { get; set; }

		public NewItem(WebApiProject Project, Action<Projects.OpenableDocument> OpenItemAction)
		{
			InitializeComponent();

			ActiveProject = Project;
			OpenProjectItem = OpenItemAction;

			EnableAddItem();
		}

		public void EnableAddItem()
		{
			NewItemAdd.IsEnabled = false;

			if (NewItemTypesList.SelectedItem == null) return;
			var NIT = NewItemTypesList.SelectedItem as NewItemType;
			if (NIT == null) return;
			if (NewItemProjectNamespaceList.SelectedItem == null && NewItemProjectNamespaceRoot.IsChecked == false) return;
			if (NewItemName.Text == "") return;

			NewItemAdd.IsEnabled = true;
		}

		private void NewItemTypesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (NewItemTypesList.SelectedItem == null)
			{
				NewItemProjectNamespaces.Visibility = Visibility.Collapsed;
			}

			NewItemAdd.IsEnabled = false;
			NewItemProjectNamespaceList.ItemsSource = null;
			NewItemProjectNamespaces.Visibility = Visibility.Collapsed;

			IsNamespaceListUpdating = true;
			NewItemProjectNamespaceRoot.IsChecked = true;
			if (NewItemTypesList.SelectedItem == null) return;

			var NIT = NewItemTypesList.SelectedItem as NewItemType;
			if (NIT == null) return;
			if (NIT.DataType == 1 || NIT.DataType == 2 || NIT.DataType == 3 || NIT.DataType == 4)
			{
				NewItemProjectNamespaceList.ItemsSource = ActiveProject.Namespace.Children;
				NewItemProjectNamespaceRoot.IsChecked = true;
				NewItemProjectNamespaceRoot.Content = ActiveProject.Namespace.Name;
				NewItemProjectNamespaces.Visibility = Visibility.Visible;
			}
			IsNamespaceListUpdating = false;
		}

		private void NewItemProjectNamespaceRoot_Checked(object sender, RoutedEventArgs e)
		{
			if (IsNamespaceListUpdating) return;
			NewItemProjectNamespaceList.ItemsSource = null;

			EnableAddItem();
		}

		private void NewItemProjectNamespaceRoot_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (NewItemProjectNamespaceList.SelectedItem != null) NewItemProjectNamespaceRoot.IsChecked = false;
			NewItemName.Focus();

			EnableAddItem();
		}

		private void NewItemName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				NewItemAdd_Click(null, null);
		}

		private void NewItemName_TextChanged(object sender, TextChangedEventArgs e)
		{
			EnableAddItem();
		}

		private void NewItemAdd_Click(object sender, RoutedEventArgs e)
		{
			if (NewItemTypesList.SelectedItem == null) return;
			Globals.IsLoading = true;

			try
			{
				var NIT = NewItemTypesList.SelectedItem as NewItemType;
				if (NIT == null) return;

				if (NIT.DataType == 1)
				{
					var NIN = NewItemProjectNamespaceList.SelectedItem as WebApiNamespace ?? ActiveProject.Namespace;
					var NI = new WebApiNamespace(NewItemName.Text, NIN, ActiveProject);
					NIN.Children.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
				else if (NIT.DataType == 2)
				{
					var NIN = NewItemProjectNamespaceList.SelectedItem as WebApiNamespace ?? ActiveProject.Namespace;
					var NI = new WebApiService(NewItemName.Text, NIN);
					foreach (var t in ActiveProject.ServerGenerationTargets) t.TargetTypes.Add(NI);
					foreach (var t in ActiveProject.ClientGenerationTargets) t.TargetTypes.Add(NI);
					NIN.Services.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
				else if (NIT.DataType == 3)
				{
					var NIN = NewItemProjectNamespaceList.SelectedItem as WebApiNamespace ?? ActiveProject.Namespace;
					var NI = new WebApiData(NewItemName.Text, NIN);
					foreach (var t in ActiveProject.ServerGenerationTargets) t.TargetTypes.Add(NI);
					foreach (var t in ActiveProject.ClientGenerationTargets) t.TargetTypes.Add(NI);
					NIN.Data.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
				else if (NIT.DataType == 4)
				{
					var NIN = NewItemProjectNamespaceList.SelectedItem as WebApiNamespace ?? ActiveProject.Namespace;
					var NI = new Projects.Enum(NewItemName.Text, NIN);
					foreach (var t in ActiveProject.ServerGenerationTargets) t.TargetTypes.Add(NI);
					foreach (var t in ActiveProject.ClientGenerationTargets) t.TargetTypes.Add(NI);
					NIN.Enums.Add(NI);
					Globals.IsLoading = false;
					OpenProjectItem(NI);
				}
			}
			finally
			{
				Globals.IsLoading = false;
			}
			NewItemCancel_Click(null, null);
		}

		private void NewItemCancel_Click(object sender, RoutedEventArgs e)
		{
			EllipticBit.Controls.WPF.Dialogs.DialogService.CloseActiveMessageBox();
		}
	}
}
