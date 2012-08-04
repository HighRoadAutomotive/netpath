using System;
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
using Microsoft.Win32;

namespace WCFArchitect.Backstage
{
	public partial class ReferencePath : Grid
	{
		public WCFArchitect.Options.UserProfileReferencePath Data { get; set; }

		private Action<ReferencePath, bool> FocusUpAction;
		private Action<ReferencePath, bool> FocusDownAction;

		public ReferencePath(WCFArchitect.Options.UserProfileReferencePath Data, Action<ReferencePath, bool> FocusUp, Action<ReferencePath, bool> FocusDown)
		{
			this.Data = Data;
			this.FocusUpAction = FocusUp;
			this.FocusDownAction = FocusDown;
			
			InitializeComponent();
		}

		private void Grid_GotFocus(object sender, RoutedEventArgs e)
		{
			Background = new SolidColorBrush(Color.FromArgb(255, 245, 245, 245));
		}

		private void Grid_LostFocus(object sender, RoutedEventArgs e)
		{
			Background = Brushes.White;
		}

		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (Update.Visibility == System.Windows.Visibility.Collapsed) Path.MaxWidth = ItemsGridCD1.ActualWidth;
			else Path.MaxWidth = ItemsGridCD1.ActualWidth - 30;
		}

		private void Path_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) FocusUpAction(this, false);
			if (e.Key == Key.Down) FocusDownAction(this, false);
		}

		private void Path_GotFocus(object sender, RoutedEventArgs e)
		{
			Path.Background = Brushes.White;
			Update.Visibility = System.Windows.Visibility.Visible;
		}

		private void Path_LostFocus(object sender, RoutedEventArgs e)
		{
			Path.Background = Brushes.Transparent;
			Update.Visibility = System.Windows.Visibility.Collapsed;
		}

		//Crutch because C1DropDown doesn't update it binding
		private void Framework_SelectedIndexChanged(object sender, C1.WPF.PropertyChangedEventArgs<int> e)
		{
			if (IsLoaded == false) return;
			UserProfileReferencePathFrameworkConverter c = new UserProfileReferencePathFrameworkConverter();
			Data.Framework = (WCFArchitect.Options.UserProfileReferencePathFramework)c.ConvertBack(Framework.SelectedIndex, null, null, null);
			UserProfileReferencePathFrameworkNameConverter n = new UserProfileReferencePathFrameworkNameConverter();
			FrameworkName.Content = (string)n.Convert(Data.Framework, null, null, null);
		}

		private void Framework_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) FocusUpAction(this, true);
			if (e.Key == Key.Down) FocusDownAction(this, true);
		}

		private void Framework_GotFocus(object sender, RoutedEventArgs e)
		{
			FrameworkName.Visibility = System.Windows.Visibility.Collapsed;
			FrameworkBorder.Visibility = System.Windows.Visibility.Visible;
		}

		private void Framework_LostFocus(object sender, RoutedEventArgs e)
		{
			FrameworkBorder.Visibility = System.Windows.Visibility.Collapsed;
			FrameworkName.Visibility = System.Windows.Visibility.Visible;
		}

		private void FrameworkName_MouseUp(object sender, MouseButtonEventArgs e)
		{
			FrameworkName.Visibility = System.Windows.Visibility.Collapsed;
			FrameworkBorder.Visibility = System.Windows.Visibility.Visible;
			Framework.Focus();
			Framework.IsDropDownOpen = true;
		}

		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the following Reference Path:" + Environment.NewLine + Data.Path, "Delete Reference Path", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				Globals.UserProfile.ReferencePaths.Remove(Data);
			}
		}

		private void Update_Click(object sender, RoutedEventArgs e)
		{
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
			if (!(Data.Path == "" || Data.Path == null)) openpath = Data.Path;
			try
			{
				Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select a Reference Path");
				ofd.AllowNonFileSystemItems = false;
				ofd.EnsurePathExists = true;
				ofd.IsFolderPicker = true;
				ofd.InitialDirectory = openpath;
				ofd.Multiselect = false;
				ofd.ShowPlacesList = true;
				if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;

				Data.Path = new System.IO.FileInfo(ofd.FileName).Directory.FullName + "\\";
			}
			catch
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.InitialDirectory = openpath;
				ofd.FileName = "_";
				ofd.DefaultExt = ".dll";
				ofd.Filter = "DLL files (*.dll)|*.dll";
				ofd.CheckPathExists = true;
				if (ofd.ShowDialog().Value == false) return;

				Data.Path = new System.IO.FileInfo(ofd.FileName).Directory.FullName + "\\";
			}
		}
	}
}