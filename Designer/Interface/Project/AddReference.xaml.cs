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
using System.Reflection;
using Microsoft.Win32;

namespace WCFArchitect.Interface.Project
{
	[Serializable]
	internal partial class AddReference : Window
	{
		ObservableCollection<Projects.Reference> References;

		Projects.Project Project { get; set; }

		List<string> FileNames { get; set; }

		public AddReference(Projects.Project Project)
		{
			References = new ObservableCollection<Projects.Reference>();
			this.Project = Project;

			InitializeComponent();

			ReferenceItems.ItemsSource = References;
		}

		private void FrameworkFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			References.Clear();
			List<Options.UserProfileReferencePath> ReferencePaths = new List<Options.UserProfileReferencePath>();

			string FV = "";

			if (FrameworkFilter.SelectedIndex == 0)
			{
				FV = "3.0.0.0";
				foreach (Options.UserProfileReferencePath UPRP in Globals.UserProfile.ReferencePaths)
					if (UPRP.IsValid == true)
						if (UPRP.Framework == Options.UserProfileReferencePathFramework.NET30 || UPRP.Framework == Options.UserProfileReferencePathFramework.Any)
							ReferencePaths.Add(UPRP);
			}

			if (FrameworkFilter.SelectedIndex == 1)
			{
				FV = "3.5.0.0";
				foreach (Options.UserProfileReferencePath UPRP in Globals.UserProfile.ReferencePaths)
					if (UPRP.IsValid == true)
						if (UPRP.Framework == Options.UserProfileReferencePathFramework.NET35 || UPRP.Framework == Options.UserProfileReferencePathFramework.Any)
							ReferencePaths.Add(UPRP);
			}

			if (FrameworkFilter.SelectedIndex == 2)
			{
				FV = "3.5.0.0";
				foreach (Options.UserProfileReferencePath UPRP in Globals.UserProfile.ReferencePaths)
					if (UPRP.IsValid == true)
						if (UPRP.Framework == Options.UserProfileReferencePathFramework.NET35Client || UPRP.Framework == Options.UserProfileReferencePathFramework.Any)
							ReferencePaths.Add(UPRP);

			}
			if (FrameworkFilter.SelectedIndex == 3)
			{
				FV = "4.0.0.0";
				foreach (Options.UserProfileReferencePath UPRP in Globals.UserProfile.ReferencePaths)
					if (UPRP.IsValid == true)
						if (UPRP.Framework == Options.UserProfileReferencePathFramework.NET40 || UPRP.Framework == Options.UserProfileReferencePathFramework.Any)
							ReferencePaths.Add(UPRP);
			}

			if (FrameworkFilter.SelectedIndex == 4)
			{
				FV = "4.0.0.0";
				foreach (Options.UserProfileReferencePath UPRP in Globals.UserProfile.ReferencePaths)
					if (UPRP.IsValid == true)
						if (UPRP.Framework == Options.UserProfileReferencePathFramework.NET40Client || UPRP.Framework == Options.UserProfileReferencePathFramework.Any)
							ReferencePaths.Add(UPRP);
			}

			foreach (Options.UserProfileReferencePath UPRP in ReferencePaths)
			{
				string[] files = System.IO.Directory.GetFiles(UPRP.Path, "*.dll", System.IO.SearchOption.TopDirectoryOnly);
				foreach (string file in files)
				{
					try
					{
						bool IsClientProfile = false;
						if (UPRP.Framework == Options.UserProfileReferencePathFramework.NET35Client || UPRP.Framework == Options.UserProfileReferencePathFramework.NET40Client)
							IsClientProfile = true;

						AssemblyName an = AssemblyName.GetAssemblyName(file);
						Projects.Reference NR = new Projects.Reference(an.Name, file, new Version(FV), System.Diagnostics.FileVersionInfo.GetVersionInfo(file).FileVersion, BitConverter.ToString(an.GetPublicKeyToken()).Replace("-", ""), IsClientProfile, Helpers.GACUtil.IsAssemblyInGAC(an.Name));
						if (ProjectHasAssembly(file)) NR.HasAssembly = true;
						References.Add(NR);
					}
					catch { }
				}
			}
		}

		private bool ProjectHasAssembly(string Path)
		{
			foreach (Projects.Reference R in Project.References)
				if (R.Path == Path) return true;
			return false;
		}

		private void BrowseAssemblyPath_Click(object sender, RoutedEventArgs e)
		{
			List<string> FileNames = null;
			string openpath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

			try
			{
				Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog("Select an Assembly");
				ofd.AllowNonFileSystemItems = false;
				ofd.EnsurePathExists = true;
				ofd.IsFolderPicker = false;
				ofd.InitialDirectory = openpath;
				ofd.Multiselect = true;
				ofd.ShowPlacesList = true;
				ofd.DefaultExtension = ".dll";
				ofd.Filters.Add(new Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter("Assembly files", ".dll"));
				if (ofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Cancel) return;

				FileNames = new List<string>(ofd.FileNames);
			}
			catch
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.InitialDirectory = openpath;
				ofd.FileName = "_";
				ofd.DefaultExt = ".dll";
				ofd.Filter = "DLL files (*.dll)|*.dll";
				ofd.CheckPathExists = true;
				ofd.Multiselect = true;
				ofd.AddExtension = false;
				if (ofd.ShowDialog().Value == false) return;

				FileNames = new List<string>(ofd.FileNames);
			}

			AssemblyPath.Text = "";
			foreach (string FN in FileNames)
				AssemblyPath.Text += "\"" + FN + "\" ";
			AssemblyPath.Text = AssemblyPath.Text.Trim();
		}

		private void Add_Click(object sender, RoutedEventArgs e)
		{
			if (AssemblyPath.Text != "")
			{
				FileNames = new List<string>(AssemblyPath.Text.Split(new string[] { "\" \"" }, StringSplitOptions.RemoveEmptyEntries));
				foreach (string FN in FileNames)
				{
					try
					{
						string FV = "";
						if (FrameworkFilter.SelectedIndex == 0) FV = "3.0.0.0";
						if (FrameworkFilter.SelectedIndex == 1) FV = "3.5.0.0";
						if (FrameworkFilter.SelectedIndex == 2) FV = "3.5.0.0";
						if (FrameworkFilter.SelectedIndex == 3) FV = "4.0.0.0";
						if (FrameworkFilter.SelectedIndex == 4) FV = "4.0.0.0";

						AssemblyName an = AssemblyName.GetAssemblyName(FN.Replace("\"", ""));
						string GACCheck = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\assembly\\" + an.Name;
						Project.References.Add(new Projects.Reference(an.Name, FN.Replace("\"", ""), new Version(FV), System.Diagnostics.FileVersionInfo.GetVersionInfo(FN.Replace("\"", "")).FileVersion, BitConverter.ToString(an.GetPublicKeyToken()).Replace("-", ""), IsClientProfile.IsChecked.Value, System.IO.Directory.Exists(GACCheck)));
					}
					catch (Exception ex)
					{
						Prospective.Controls.MessageBox.Show("Unable to load the assembly '" + FN.Replace("\"", "") + "'. Skipping." + Environment.NewLine + Environment.NewLine + "Error Description:" + Environment.NewLine + ex.ToString(), "Error Adding Assembly", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
				AssemblyPath.Text = "";
			}
			else
			{
				foreach (Projects.Reference R in ReferenceItems.SelectedItems)
				{
					try
					{
						if (R.HasAssembly == false)
							Project.References.Add(R);
					}
					catch (Exception ex)
					{
						Prospective.Controls.MessageBox.Show("Unable to load the assembly '" + R.Path + "'. Skipping." + Environment.NewLine + Environment.NewLine + "Error Description:" + Environment.NewLine + ex.ToString(), "Error Adding Assembly", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
				FrameworkFilter_SelectionChanged(null, null);
			}
		}

		private void CloseWindow_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		public void GetAssemblyFrameworkVersion(string RP)
		{
			Assembly AI = Assembly.Load(System.IO.File.ReadAllBytes(RP));
			AppDomain.CurrentDomain.SetData("AIV", AI.ImageRuntimeVersion.Substring(1));
		}

		private void ReferenceItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ReferenceItems.SelectedItems.Count == 0)
			{
				AssemblyInternalName.Text = "";
				AssemblyVersion.Text = "";
				AssemblyFileVersion.Text = "";
				AssemblyToken.Text = "";
				if (AssemblyPath.Text == "") Add.IsEnabled = false;
				return;
			}
			Add.IsEnabled = true;

			if (e.AddedItems.Count == 0 && e.RemovedItems.Count > 0)
			{
				Projects.Reference TR = (Projects.Reference)ReferenceItems.SelectedItems[0];
				AssemblyInternalName.Text = TR.Name;
				AssemblyVersion.Text = TR.Version.ToString();
				AssemblyFileVersion.Text = TR.FrameworkVersion;
				AssemblyToken.Text = TR.PublicKeyToken;
			}
			if (e.AddedItems.Count > 0 && e.RemovedItems.Count == 0)
			{
				Projects.Reference TR = (Projects.Reference)e.AddedItems[0];
				AssemblyInternalName.Text = TR.Name;
				AssemblyVersion.Text = TR.Version.ToString();
				AssemblyFileVersion.Text = TR.FrameworkVersion;
				AssemblyToken.Text = TR.PublicKeyToken;
			}
		}

		private void AssemblyPath_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (AssemblyPath.Text == "")
			{
				if (FrameworkFilter.SelectedIndex == 2 || FrameworkFilter.SelectedIndex == 4)
					IsClientProfile.IsChecked = true;
				else
					IsClientProfile.IsChecked = false;
				IsClientProfile.IsEnabled = false;

				if (ReferenceItems.SelectedItems.Count == 0)
					Add.IsEnabled = false;
			}
			else
			{
				IsClientProfile.IsEnabled = true;
				Add.IsEnabled = true;
			}
		}
	}

	[ValueConversion(typeof(bool), typeof(string))]
	public class HasAssemblyImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool lt = (bool)value;
			if (lt == true)
			{
				return "pack://application:,,,/WCFArchitect;component/Icons/X16/ReferenceAdded.png";
			}
			else
			{
				return "pack://application:,,,/WCFArchitect;component/Icons/Blank.png";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}