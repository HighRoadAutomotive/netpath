using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using MP.Karvonite;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace WCFArchitect
{
	internal static class Globals
	{
		public static System.Threading.Timer BackupTimer { get; set; }

		public static Options.License LicenseKey { get; set; }
		public static Prospective.Server.Licensing.LicenseInfoWPF LicenseInfo { get; set; }
		public static string ApplicationPath { get; set; }
		public static Version ApplicationVersion { get; set; }
		public static string LicenseKeyPath { get; set; }
		public static ObjectSpace LicenseKeySpace { get; set; }
		public static string ArgSolutionPath { get; set; }
		public static bool IsNewVersionAvailable { get; set; }
		public static string NewVersionPath { get; set; }
		public static Options.UserProfile UserProfile { get; set; }
		public static ObjectSpace UserProfileSpace { get; set; }
		public static string UserProfilePath { get; set; }

		public static ObjectSpace ProjectSpace { get; set; }
		public static Projects.Solution Solution { get; set; }
		public static ObservableCollectionSortable<Projects.Project> Projects { get; set; }
		public static Options.RecentSolution ActiveProjectInfo { get; set; }
		public static ObservableCollectionSortable<Projects.OpenableDocument> OpenDocuments { get; set; }
		public static ThumbnailToolBarButton BuildSolutionButton { get; set; }
		public static ThumbnailToolBarButton BuildOutputButton { get; set; }

		public static Compiler.CompilerManager CompilerManager { get; set; }

		public static bool IsLoading { get; set; }
		public static bool IsLoadSorting { get; set; }
		public static bool IsOpening { get; set; }
		public static bool IsDocumentOpening { get; set; }
		public static bool IsFinding { get; set; }
		public static bool IsSaving { get; set; }
		public static bool IsClosing { get; set; }
		public static Interface.Main MainScreen { get; set; }
		public static List<Interface.Project.Project> ProjectScreens { get; set; }

		public static T GetVisualParent<T>(object childObject) where T : Visual { DependencyObject child = childObject as DependencyObject; while ((child != null) && !(child is T)) { child = VisualTreeHelper.GetParent(child); } return child as T; }
		public static T GetVisualChild<T>(Visual parent) where T : Visual { T child = default(T); int numVisuals = VisualTreeHelper.GetChildrenCount(parent); for (int i = 0; i < numVisuals; i++) { Visual v = (Visual)VisualTreeHelper.GetChild(parent, i); child = v as T; if (child == null) { child = GetVisualChild<T>(v); } if (child != null) { break; } } return child; }

		public static string GetRelativePath(string BasePath, string FilePath)
		{
			if (!Path.IsPathRooted(FilePath)) FilePath = Path.GetFullPath(FilePath);
			if (!Path.IsPathRooted(BasePath)) BasePath = Path.GetFullPath(BasePath);

			if (!BasePath.EndsWith("\\")) BasePath += "\\";

			Uri t = new Uri("file:///" + FilePath);
			Uri b = new Uri("file:///" + BasePath);
			return b.MakeRelativeUri(t).ToString();
		}

		public static void OpenProjectSpace(string Path, Action<bool> FinishedAction)
		{
			Application.Current.Dispatcher.BeginInvoke(new Action(() =>
			{
				IsLoading = true;
				if (ProjectSpace.State == ObjectSpaceState.Open) FinishedAction(false);

				//Load the project file
				if (!System.IO.File.Exists(Path))
				{
					Prospective.Controls.MessageBox.Show("Unable to located the requested file, please try again.", "Unable to Locate Project File.", MessageBoxButton.OK);
					FinishedAction(false);
					return;
				}
				System.IO.FileInfo fi = new System.IO.FileInfo(Path);
				if (fi.IsReadOnly == true)
				{
					Prospective.Controls.MessageBox.Show("The solution file '" + Path + "' is currently read-only. Please disable read-only mode on this file.", "Open Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					FinishedAction(false);
					return;
				}
				if (System.IO.File.Exists(System.IO.Path.ChangeExtension(Path, ".bak")))
				{
					System.IO.FileInfo bfi = new System.IO.FileInfo(System.IO.Path.ChangeExtension(Path, ".bak"));
					if (fi.LastWriteTime < bfi.LastWriteTime)
					{
						if (Prospective.Controls.MessageBox.Show("WCF Architect has detected that the solution '" + Path + "' was not properly closed. A newer backup exists. Would you like to use this backup?", "Open Error", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
						{
							System.IO.File.Delete(Path);
							System.IO.File.Move(System.IO.Path.ChangeExtension(Path, ".bak"), Path);
						}
						else
						{
							System.IO.File.Delete(System.IO.Path.ChangeExtension(Path, ".bak"));
						}
					}
					else
					{
						System.IO.File.Delete(System.IO.Path.ChangeExtension(Path, ".bak"));
					}
				}

				Globals.ProjectSpace.EnableConcurrency = true;
				Globals.ProjectSpace.Open(Path, ObjectSpaceOpenMode.ReadWrite);

#if TRIAL
				if (Globals.ProjectSpace.OfType<Projects.Project>().Count() > 3)
				{
					if (Globals.LicenseInfo.ExpirationURL != "" && Globals.LicenseInfo.ExpirationURL != null)
					{
						if (Prospective.Controls.MessageBox.Show("This trial version of WCF Architect is unable to open any solution that contains more than 3 projects." + Environment.NewLine + Environment.NewLine + "Would you like to purchase a copy of WCF Architect?", "Open Error", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
							System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(Globals.LicenseInfo.ExpirationURL));
					}
					else
						Prospective.Controls.MessageBox.Show("This trial version of WCF Architect is unable to open any solution that contains more than 3 projects." + Environment.NewLine + Environment.NewLine + "Please purchase a licensed copy of WCF Architect from http://architect.prospectivesoftware.com/pricing.htm to open this Solution.", "Open Error", MessageBoxButton.OK, MessageBoxImage.Error);
					Globals.ProjectSpace.Close();
					FinishedAction(false);
					return;
				}
#endif

				Globals.Solution = Globals.ProjectSpace.OfType<Projects.Solution>().First<Projects.Solution>();
#if TRIAL
				if (Globals.Solution.TrialID == "" || Globals.Solution.TrialID == null) Globals.Solution.TrialID = Globals.LicenseKey.Authorization;
				if (Globals.Solution.TrialID != Globals.LicenseKey.Authorization)
				{
					if (Globals.LicenseInfo.ExpirationURL != "" && Globals.LicenseInfo.ExpirationURL != null)
					{
						if (Prospective.Controls.MessageBox.Show("This solution was created using a different installation of the WCF Architect Trial and can only be opened by the installation that created it or by a licensed copy of WCF Architect." + Environment.NewLine + Environment.NewLine + "Would you like to purchase a copy of WCF Architect?", "Open Error", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
							System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(Globals.LicenseInfo.ExpirationURL));
					}
					else
						Prospective.Controls.MessageBox.Show("This solution was created using a different installation of the WCF Architect Trial and can only be opened by the installation that created it or by a licensed copy of WCF Architect." + Environment.NewLine + Environment.NewLine + "Please purchase a licensed copy of WCF Architect from http://architect.prospectivesoftware.com/pricing.htm to open this Solution.", "Open Error", MessageBoxButton.OK, MessageBoxImage.Error);
					Globals.ProjectSpace.Close();
					FinishedAction(false);
					return;
				}
#else
				Globals.Solution.TrialID = "";
#endif
				if (Globals.UserProfile.AutomaticBackupsEnabled == true)
					BackupTimer = new System.Threading.Timer(new System.Threading.TimerCallback(Globals.BackupProjectSpace), null, (long)Globals.UserProfile.AutomaticBackupsInterval.TotalMilliseconds, (long)Globals.UserProfile.AutomaticBackupsInterval.TotalMilliseconds);
				
				Globals.Solution = Globals.ProjectSpace.OfType<Projects.Solution>().First<Projects.Solution>();
				Globals.Projects = new ObservableCollectionSortable<Projects.Project>(Globals.ProjectSpace.OfType<Projects.Project>().OrderBy(x => x.Name));
				Globals.MainScreen.SolutionNavigatorHeaderTitle.Text = "Solution '" + Globals.Solution.Name + "' (" + Globals.Projects.Count + " Projects)";
				if (Globals.Projects.Count == 1) Globals.MainScreen.SolutionNavigatorHeaderTitle.Text = "Solution '" + Globals.Solution.Name + "' (1 Project)";
				Globals.MainScreen.SolutionNavigatorView.ItemsSource = Globals.Projects;

				//Check to see if this developer/computer is listed. Set last edited if it is, add developer if it isn't.
				bool HasDeveloper = false;
				foreach (Projects.Developer TD in Globals.ProjectSpace.OfType<Projects.Developer>())
				{
					TD.LastEditedBy = false;
					if (TD.UserName == Globals.UserProfile.User && TD.ComputerName == Globals.UserProfile.ComputerName)
					{
						TD.LastEditedBy = true;
						HasDeveloper = true;
					}
				}
				if (HasDeveloper == false)
					Globals.ProjectSpace.Add(new Projects.Developer() { ID = Guid.NewGuid(), ComputerName = Globals.UserProfile.ComputerName, UserName = Globals.UserProfile.User, LastEditedBy = true });
				else
				{	//This code removes redundant entries in the developer list.
					List<Projects.Developer> TDL = new List<Projects.Developer>(Globals.ProjectSpace.OfType<Projects.Developer>().Where(a => a.UserName == Globals.UserProfile.User && a.ComputerName == Globals.UserProfile.ComputerName));
					if (TDL.Count > 1)
					{
						for (int i = 1; i < TDL.Count; i++)
							Globals.ProjectSpace.Remove(TDL[i]);
					}
				}

				Globals.ProjectSpace.Save();

				MainScreen.OutputTabs.Items.Clear();
				MainScreen.ErrorListTabs.Items.Clear();

				IsOpening = true;
				foreach (Projects.Project P in Globals.Projects)
					P.Open();
				IsOpening = false;

				Globals.IsLoading = false;

				Globals.IsLoadSorting = true;
				Globals.MainScreen.SortOpenWindows();
				Globals.IsLoadSorting = false;

				bool HasActive = false;
				foreach (Themes.C1DockTabItemWindow W in MainScreen.ProjectTabs.Items)
				{
					if (W.DocumentData.IsActive == true)
					{
						MainScreen.ProjectTabs.SelectedItem = W;
						HasActive = true;
						break;
					}
				}
				if (HasActive == false)
					if (MainScreen.ProjectTabs.Items.Count > 0)
						MainScreen.ProjectTabs.SelectedIndex = 0;

				if (TaskbarManager.IsPlatformSupported == true)
				{
					Globals.BuildSolutionButton.Enabled = true;
					Globals.BuildOutputButton.Enabled = true;
				}

				MainScreen.UpdateTabsLayout();

				FinishedAction(true);
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		public static void SaveProjectSpace()
		{
			Application.Current.Dispatcher.BeginInvoke(new Action(() =>
			{
				if (ProjectSpace.State == ObjectSpaceState.Closed) return;
				IsFinding = true;
				IsSaving = true;

				Globals.MainScreen.SystemStatus.Text = "Saving...";

				Globals.MainScreen.IndexOpenWindows();

				Globals.ProjectSpace.Save();

				IsFinding = false;
				IsSaving = false;

				foreach (Projects.OpenableDocument OD in OpenDocuments)
				{
					bool ia = OD.IsActive;
					OD.IsActive = true;
					OD.IsDirty = false;
					OD.IsActive = ia;
				}

				Globals.MainScreen.SystemStatus.Text = "Ready";

			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		public static void BackupProjectSpace(object State)
		{
			Application.Current.Dispatcher.BeginInvoke(new Action(() =>
			{
				if (ProjectSpace.State == ObjectSpaceState.Closed) return;
				IsFinding = true;
				IsSaving = true;

				Globals.MainScreen.SystemStatus.Text = "Auto-Saving Backup File...";
				Globals.MainScreen.IndexOpenWindows();

				List<object> DOL = new List<object>();

				DOL.Add(Solution);
				DOL.AddRange(Projects);
				DOL.AddRange(Globals.ProjectSpace.OfType<WCFArchitect.Projects.Developer>());

				ObjectSpace os = new ObjectSpace("Projects.kvtmodel", "WCFArchitect");
				os.CreateObjectLibrary(System.IO.Path.ChangeExtension(ProjectSpace.FileName, ".bak"), ExistingFileAction.Overwrite);
				os.Open(System.IO.Path.ChangeExtension(ProjectSpace.FileName, ".bak"), ObjectSpaceOpenMode.ReadWrite);
				foreach (object o in DOL) os.Add(o);
				os.Save();
				os.Close();

				IsFinding = false;
				IsSaving = false;
				Globals.MainScreen.SystemStatus.Text = "Ready";
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		public static void CloseProjectSpace()
		{
			if (ProjectSpace == null) return;
			if (ProjectSpace.State == ObjectSpaceState.Closed) return;

			if (Globals.UserProfile.AutomaticBackupsEnabled == true)
				if(BackupTimer != null)
					BackupTimer.Dispose();

			Globals.MainScreen.IndexOpenWindows();

			if (TaskbarManager.IsPlatformSupported == true)
			{
				Globals.BuildSolutionButton.Enabled = false;
				Globals.BuildOutputButton.Enabled = false;
			}

			string OSPath = ProjectSpace.FileName;
			ProjectSpace.Save();
			ProjectSpace.Close();

			MainScreen.ProjectTabs.Items.Clear();
			OpenDocuments.Clear();

			//ProjectSpace.CompactObjectLibrary(OSPath);		//BROKEN! (not reported due to website being down)
		}

		public static void ReplaceDataType(string OldName, string OldNamespace, string NewName, string NewNamespace)
		{
			if (IsClosing == true) return;
			if (IsFinding == true) return;
			if (Prospective.Controls.MessageBox.Show("Would you like WCF Architect to attempt to update any references to this data type? This will search the entire solution.'", "Update References", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No) return;

			WCFArchitect.Projects.FindReplaceInfo FRINamespace = new Projects.FindReplaceInfo(WCFArchitect.Projects.FindItems.Any, WCFArchitect.Projects.FindLocations.EntireSolution, OldNamespace + "." + OldName, false, false, NewNamespace + "." + NewName, true);
			WCFArchitect.Projects.FindReplaceInfo FRINameOnly = new Projects.FindReplaceInfo(WCFArchitect.Projects.FindItems.Any, WCFArchitect.Projects.FindLocations.EntireSolution, OldName, false, false, NewName, true);

			foreach(Projects.Project P in Projects)
			{
				P.FindReplace(FRINameOnly);
				P.FindReplace(FRINamespace);
			}
		}
	}
}