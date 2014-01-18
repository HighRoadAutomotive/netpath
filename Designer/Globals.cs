using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using NETPath.Interface;
using Prospective.Controls.Dialogs;
using NETPath.Projects;
using NETPath.Generators.Interfaces;

namespace NETPath
{
	internal static class Globals
	{
		public static System.Threading.Timer BackupTimer { get; set; }

		public static string ApplicationPath { get; set; }
		public static Version ApplicationVersion { get; set; }
		public static string ArgSolutionPath { get; set; }

		public static Options.UserProfile UserProfile { get; set; }
		public static string UserProfilePath { get; set; }

		public enum WindowsVersion
		{
			WinVista = 60,
			WinSeven = 61,
			WinEight = 62,
			WinEightOne = 63,
		}
		public static WindowsVersion WindowsLevel { get; set; }

		public static IGenerator NETGenerator { get; internal set; }
		public static ConcurrentDictionary<Guid, Compiler> Compilers { get; internal set; }

		public static string ProjectPath { get; set; }
		public static Project Project { get; set; }
		public static Options.RecentProject ProjectInfo { get; set; }

		public static bool IsLoading { get; set; }
		public static bool IsFinding { get; set; }
		public static bool IsSaving { get; set; }
		public static bool IsClosing { get; set; }
		public static Interface.Main MainScreen { get; set; }
		public static Interface.Navigator OpenNavigator { get; set; }

		public static T GetVisualParent<T>(object childObject) where T : Visual { var child = childObject as DependencyObject; while ((child != null) && !(child is T)) { child = VisualTreeHelper.GetParent(child); } return child as T; }
		public static T GetVisualChild<T>(Visual parent) where T : Visual { T child = default(T); int numVisuals = VisualTreeHelper.GetChildrenCount(parent); for (int i = 0; i < numVisuals; i++) { var v = (Visual)VisualTreeHelper.GetChild(parent, i); child = v as T; if (child == null) { child = GetVisualChild<T>(v); } if (child != null) { break; } } return child; }

		public static string GetRelativePath(string BasePath, string FilePath)
		{
			if (!Path.IsPathRooted(FilePath)) FilePath = Path.GetFullPath(FilePath);
			if (!Path.IsPathRooted(BasePath)) BasePath = Path.GetFullPath(BasePath);

			var t = new Uri("file:///" + FilePath);
			var b = new Uri("file:///" + BasePath);
			return Uri.UnescapeDataString(b.MakeRelativeUri(t).ToString());
		}

		public static void OpenProjectItem(OpenableDocument doc)
		{
			if (OpenNavigator == null) return;
			OpenNavigator.OpenProjectItem(doc);
		}

		public static List<ServiceBinding> GetBindings()
		{
			return Project.Namespace.GetBindings();
		}

		public static void InitializeCodeGenerators(string License)
		{
		}

		internal static void GeneratorOutput(Guid ID, string output)
		{
			Compiler t;
			if (Compilers.TryGetValue(ID, out t))
				t.GeneratorOutput(output);
		}

		internal static void GeneratorMessage(Guid ID, CompileMessage message)
		{
			Compiler t;
			if (Compilers.TryGetValue(ID, out t))
				t.GeneratorMessage(message);
		}

		public static void OpenProject(string Path, Action<bool> FinishedAction)
		{
			IsLoading = true;

			//Load projects
			if (File.Exists(System.IO.Path.ChangeExtension(Path, ".bak")))
			{
				var fi = new FileInfo(Path);
				var bfi = new FileInfo(System.IO.Path.ChangeExtension(Path, ".bak"));
				if (fi.LastWriteTime < bfi.LastWriteTime)
				{
					DialogService.ShowMessageDialog(null, "Project Load Error", "NETPath has detected that the solution '" + Path + "' was not properly closed. A newer backup exists. Would you like to use this backup?",
						new DialogAction("Yes", () => { File.Delete(Path); File.Move(System.IO.Path.ChangeExtension(Path, ".bak"), Path); }, true), new DialogAction("No", () => File.Delete(System.IO.Path.ChangeExtension(Path, ".bak")), false, true));
				}
				else
				{
					File.Delete(System.IO.Path.ChangeExtension(Path, ".bak"));
				}
			}

			//Open the project.
			try
			{
				Project = Project.Open(Path);
				ProjectPath = Path;
			}
			catch (Exception ex)
			{
				DialogService.ShowMessageDialog(null, "Project Load Error", ex.ToString(), new DialogAction("Ok", () => FinishedAction(false), true, true));
				return;
			}

			if (UserProfile.AutomaticBackupsEnabled)
				BackupTimer = new System.Threading.Timer(BackupProject, null, (long)UserProfile.AutomaticBackupsInterval.TotalMilliseconds, (long)UserProfile.AutomaticBackupsInterval.TotalMilliseconds);

			IsLoading = false;

			FinishedAction(true);
		}

		public static void BuildProject()
		{
			MainScreen.IsBuilding = true;
			foreach (var p in Compilers)
				p.Value.Build();
			MainScreen.IsBuilding = false;
		}

		public static void SaveProject()
		{
			IsFinding = true;
			IsSaving = true;

			Project.Save(Project);

			IsFinding = false;
			IsSaving = false;
		}

		public static async void BackupProject(object State)
		{
			IsFinding = true;
			IsSaving = true;

			await Application.Current.Dispatcher.InvokeAsync(() => Project.Save(Project, Path.ChangeExtension(ProjectPath, ".bak")), DispatcherPriority.Send);

			IsFinding = false;
			IsSaving = false;
		}

		public static void CloseProject()
		{
			if (UserProfile.AutomaticBackupsEnabled)
				if (BackupTimer != null)
					BackupTimer.Dispose();

			Project.Save(Project);
		}
	}
}