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
		public static string SolutionPath { get; set; }
		public static ObservableCollectionSortable<Projects.Project> Projects { get; set; }
		public static Options.RecentSolution ActiveProjectInfo { get; set; }
		public static ObservableCollectionSortable<Projects.OpenableDocument> OpenDocuments { get; set; }
		public static ThumbnailToolBarButton BuildSolutionButton { get; set; }
		public static ThumbnailToolBarButton BuildOutputButton { get; set; }

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

				//Check for backups and open the solution.
				if (System.IO.File.Exists(System.IO.Path.ChangeExtension(Path, ".bak")))
				{
					System.IO.FileInfo fi = new System.IO.FileInfo(Path);
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
				try
				{
					SolutionPath = Path;
					Globals.Solution = WCFArchitect.Projects.Solution.Open(Path);
				}
				catch (Exception ex)
				{
					Prospective.Controls.MessageBox.Show(ex.Message, "Solution Load Error", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
					FinishedAction(false);
					return;
				}
				
				//Load projects
				Globals.Projects = new ObservableCollectionSortable<Projects.Project>();
				foreach (string p in Globals.Solution.Projects)
				{
					if (System.IO.File.Exists(System.IO.Path.ChangeExtension(p, ".bak")))
					{
						System.IO.FileInfo fi = new System.IO.FileInfo(p);
						System.IO.FileInfo bfi = new System.IO.FileInfo(System.IO.Path.ChangeExtension(p, ".bak"));
						if (fi.LastWriteTime < bfi.LastWriteTime)
						{
							if (Prospective.Controls.MessageBox.Show("WCF Architect has detected that the solution '" + p + "' was not properly closed. A newer backup exists. Would you like to use this backup?", "Open Error", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
							{
								System.IO.File.Delete(p);
								System.IO.File.Move(System.IO.Path.ChangeExtension(p, ".bak"), p);
							}
							else
							{
								System.IO.File.Delete(System.IO.Path.ChangeExtension(p, ".bak"));
							}
						}
						else
						{
							System.IO.File.Delete(System.IO.Path.ChangeExtension(p, ".bak"));
						}
					}

					//Open the project.
					try
					{
						Globals.Projects.Add(WCFArchitect.Projects.Project.Open(SolutionPath, p));
					}
					catch (Exception ex)
					{
						Prospective.Controls.MessageBox.Show(ex.Message, "Project Load Error", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
						FinishedAction(false);
						return;
					}
				}
				Globals.Projects.Sort(a => a.Name);

				if (Globals.UserProfile.AutomaticBackupsEnabled == true)
					BackupTimer = new System.Threading.Timer(new System.Threading.TimerCallback(Globals.BackupSolution), null, (long)Globals.UserProfile.AutomaticBackupsInterval.TotalMilliseconds, (long)Globals.UserProfile.AutomaticBackupsInterval.TotalMilliseconds);

				Globals.MainScreen.SolutionNavigatorHeaderTitle.Text = "Solution '" + Globals.Solution.Name + "' (" + Globals.Projects.Count + " Projects)";
				if (Globals.Projects.Count == 1) Globals.MainScreen.SolutionNavigatorHeaderTitle.Text = "Solution '" + Globals.Solution.Name + "' (1 Project)";
				Globals.MainScreen.SolutionNavigatorView.ItemsSource = Globals.Projects;

				MainScreen.OutputTabs.Items.Clear();
				MainScreen.ErrorListTabs.Items.Clear();

				Globals.IsLoading = false;

				if (TaskbarManager.IsPlatformSupported == true)
				{
					Globals.BuildSolutionButton.Enabled = true;
					Globals.BuildOutputButton.Enabled = true;
				}

				MainScreen.UpdateTabsLayout();

				FinishedAction(true);
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		public static void SaveSolution()
		{
			Application.Current.Dispatcher.BeginInvoke(new Action(() =>
			{
				if (ProjectSpace.State == ObjectSpaceState.Closed) return;
				IsFinding = true;
				IsSaving = true;

				Globals.MainScreen.SystemStatus.Text = "Saving...";

				WCFArchitect.Projects.Solution.Save(Globals.Solution, SolutionPath);

				foreach (WCFArchitect.Projects.Project p in Globals.Projects)
					WCFArchitect.Projects.Project.Save(p, p.AbsolutePath);

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

		public static void BackupSolution(object State)
		{
			Application.Current.Dispatcher.BeginInvoke(new Action(() =>
			{
				if (ProjectSpace.State == ObjectSpaceState.Closed) return;
				IsFinding = true;
				IsSaving = true;

				Globals.MainScreen.SystemStatus.Text = "Auto-Saving Backup File...";

				WCFArchitect.Projects.Solution.Save(Globals.Solution, System.IO.Path.ChangeExtension(SolutionPath, ".bak"));

				foreach (WCFArchitect.Projects.Project p in Globals.Projects)
					WCFArchitect.Projects.Project.Save(p, System.IO.Path.ChangeExtension(p.AbsolutePath, ".bak"));

				IsFinding = false;
				IsSaving = false;
				Globals.MainScreen.SystemStatus.Text = "Ready";
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		public static void CloseSolution()
		{
			if (ProjectSpace == null) return;
			if (ProjectSpace.State == ObjectSpaceState.Closed) return;

			if (Globals.UserProfile.AutomaticBackupsEnabled == true)
				if(BackupTimer != null)
					BackupTimer.Dispose();

			if (TaskbarManager.IsPlatformSupported == true)
			{
				Globals.BuildSolutionButton.Enabled = false;
				Globals.BuildOutputButton.Enabled = false;
			}

			SaveSolution();

			MainScreen.ProjectTabs.Items.Clear();
			OpenDocuments.Clear();
		}
	}
}