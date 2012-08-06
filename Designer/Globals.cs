using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using MP.Karvonite;

namespace WCFArchitect
{
	internal static class Globals
	{
		public static System.Threading.Timer BackupTimer { get; set; }

		public static string ApplicationPath { get; set; }
		public static Version ApplicationVersion { get; set; }
		public static string ArgSolutionPath { get; set; }

		public static bool IsNewVersionAvailable { get; set; }
		public static string NewVersionPath { get; set; }

		public static string LicenseKeyPath { get; set; }

		public static Options.UserProfile UserProfile { get; set; }
		public static ObjectSpace UserProfileSpace { get; set; }
		public static string UserProfilePath { get; set; }

		public static ConcurrentQueue<MessageBox> Messages { get; set; }

		public enum WindowsVersion
		{
			WinVista = 60,
			WinSeven = 61,
			WinEight = 62,
		}
		public static WindowsVersion WindowsLevel { get; set; }

		public static string SolutionPath { get; set; }
		public static Projects.Solution Solution { get; set; }
		public static Options.RecentSolution SolutionInfo { get; set; }
		public static ObservableCollectionSortable<Projects.Project> Projects { get; set; }

		public static bool IsLoading { get; set; }
		public static bool IsLoadSorting { get; set; }
		public static bool IsOpening { get; set; }
		public static bool IsDocumentOpening { get; set; }
		public static bool IsFinding { get; set; }
		public static bool IsSaving { get; set; }
		public static bool IsClosing { get; set; }
		public static Interface.Main MainScreen { get; set; }

		public static T GetVisualParent<T>(object childObject) where T : Visual { DependencyObject child = childObject as DependencyObject; while ((child != null) && !(child is T)) { child = VisualTreeHelper.GetParent(child); } return child as T; }
		public static T GetVisualChild<T>(Visual parent) where T : Visual { T child = default(T); int numVisuals = VisualTreeHelper.GetChildrenCount(parent); for (int i = 0; i < numVisuals; i++) { Visual v = (Visual)VisualTreeHelper.GetChild(parent, i); child = v as T; if (child == null) { child = GetVisualChild<T>(v); } if (child != null) { break; } } return child; }

		static Globals()
		{
			Messages = new ConcurrentQueue<MessageBox>();
		}

		public static string GetRelativePath(string BasePath, string FilePath)
		{
			if (!Path.IsPathRooted(FilePath)) FilePath = Path.GetFullPath(FilePath);
			if (!Path.IsPathRooted(BasePath)) BasePath = Path.GetFullPath(BasePath);

			if (!BasePath.EndsWith("\\")) BasePath += "\\";

			Uri t = new Uri("file:///" + FilePath);
			Uri b = new Uri("file:///" + BasePath);
			return b.MakeRelativeUri(t).ToString();
		}

		public static void OpenSolution(string Path, Action<bool> FinishedAction)
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
					Globals.Solution = WCFArchitect.Projects.Solution.Open(Path, false);
				}
				catch (Exception ex)
				{
					Prospective.Controls.MessageBox.Show(ex.Message, "Solution Load Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
						Prospective.Controls.MessageBox.Show(ex.Message, "Project Load Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
						FinishedAction(false);
						return;
					}
				}
				Globals.Projects.Sort(a => a.Name);

				if (Globals.UserProfile.AutomaticBackupsEnabled == true)
					BackupTimer = new System.Threading.Timer(new System.Threading.TimerCallback(Globals.BackupSolution), null, (long)Globals.UserProfile.AutomaticBackupsInterval.TotalMilliseconds, (long)Globals.UserProfile.AutomaticBackupsInterval.TotalMilliseconds);

				//TODO: Setup Project Navigator here.

				Globals.IsLoading = false;

				FinishedAction(true);
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		public static void SaveSolution()
		{
			Application.Current.Dispatcher.BeginInvoke(new Action(() =>
			{
				IsFinding = true;
				IsSaving = true;

				WCFArchitect.Projects.Solution.Save(Globals.Solution);

				foreach (WCFArchitect.Projects.Project p in Globals.Projects)
					WCFArchitect.Projects.Project.Save(p);

				IsFinding = false;
				IsSaving = false;

			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		public static void BackupSolution(object State)
		{
			Application.Current.Dispatcher.BeginInvoke(new Action(() =>
			{
				IsFinding = true;
				IsSaving = true;

				WCFArchitect.Projects.Solution.Save(Globals.Solution, System.IO.Path.ChangeExtension(SolutionPath, ".bak"));

				foreach (WCFArchitect.Projects.Project p in Globals.Projects)
					WCFArchitect.Projects.Project.Save(p, System.IO.Path.ChangeExtension(p.AbsolutePath, ".bak"));

				IsFinding = false;
				IsSaving = false;
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		public static void CloseSolution(bool SaveData)
		{
			if (Globals.UserProfile.AutomaticBackupsEnabled == true)
				if(BackupTimer != null)
					BackupTimer.Dispose();

			WCFArchitect.Projects.Solution.Close(Globals.Solution, Globals.SolutionPath, SaveData);

			foreach (WCFArchitect.Projects.Project p in Globals.Projects)
				WCFArchitect.Projects.Project.Close(p, p.AbsolutePath, SaveData);
		}

		public static void ShowMessageBox(Projects.Project Origin, string Caption, string Message, params MessageAction[] Actions)
		{
			Messages.Enqueue(new MessageBox(Origin, Caption, Message, new List<MessageAction>(Actions)));
			Globals.MainScreen.ProcessNextMessage();
		}

		public static void ShowDialogBox(Projects.Project Origin, string Caption, object Content, params MessageAction[] Actions)
		{
			Messages.Enqueue(new MessageBox(Origin, Caption, Content, new List<MessageAction>(Actions)));
			Globals.MainScreen.ProcessNextMessage();
		}
	}

	internal class MessageBox : DependencyObject
	{
		public string Caption { get { return (string)GetValue(CaptionProperty); } set { SetValue(CaptionProperty, value); } }
		public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(MessageBox), new PropertyMetadata(""));

		public object Message { get { return (object)GetValue(MessageProperty); } set { SetValue(MessageProperty, value); } }
		public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(object), typeof(MessageBox), new PropertyMetadata(new object()));

		public List<MessageAction> Actions { get { return (List<MessageAction>)GetValue(ActionsProperty); } set { SetValue(ActionsProperty, value); } }
		public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register("Actions", typeof(List<MessageAction>), typeof(MessageBox));

		public Projects.Project Origin { get; private set; }

		public MessageBox(Projects.Project Origin, string Caption, object Message, List<MessageAction> Actions)
		{
			this.Origin = Origin;
			this.Caption = Caption;
			this.Message = Message;
			this.Actions = Actions;
		}
	}

	public class MessageAction
	{
		public string Title { get; private set; }
		public Action Action { get; private set; }
		public object Owner { get; internal set; }

		public MessageAction(string Title, Action Action)
		{
			this.Title = Title;
			this.Action = Action;
			this.Owner = null;
		}
	}
}