using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
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
		public static Prospective.Server.Licensing.UsageData Usage { get; set; }
		public const string TrialLicense = "lgKkARmvMXTEec4BHgAvAFNLVT1OUDIwVFJJQUwjTGljZW5zZWVOYW1lPVByb3NwZWN0aXZlIFNvZnR3YXJlAQNEZ9ul5WCNDyXclcMH0Y/O5xklUKqd0W1M/lZjqHL9jcp7tt741X38fW4nYB7Xb88vVO6ks+KNQ7RvKoyjVI1j86CkkQXWvGkgpq6STx0ORAfhmEnvn0+AyfKWAuMn6ZQCzFYoZ46V9SwA+FQz2+vpM2+DP6Ik1QfvHnrirxDLEsndJzuzSGB+3MFEZd+0k2BtA9yUSa7CRa+6QAfHQjZf4FWxVMrJQ7hcSEvN8fLd9oCLozBCVYEtpNPPEyLNB1Kfq2nBJKkO+36gTYR1CvqW+UXvMa0jtDYe1+5la89Kiis7tvH4vMiFwSxKdpy1krparGtHBT7xQT6xGhwzUo7p";
		public const string LicenseVerification = "AAAEAAG6rTV/gUg+VZjvEZQDqWy9l63DgzkUSg0tyJOBDDS58FKoRvErRfUkvxdlgUCCTTvw5b7lXtVPFxd3HI+SFzzTi5X0neWXCNXjWX/FVnIaCBioKHG6eYwgSE86j2ybYQbGlmy+R9vpj3cA12E6a4efoQl/5yqawkUk67iQGnJi0YiA6LUAQUoCN+XipZN3pEn+EuAPGVAz1W0b8pYX99oSrWr3CQwnGCg6/2Y5radzYdPDsZgWkKkWhPU/ZGXcDo+GB4e35OaO6hp8lcq3lmxc+3Ic9eDsVK1kHaccRI/hWcgmkp39/3/zk1mnVtgiED8RI0eUniUTWXTGVTtBvBGLAwABAAE=";

		public static bool IsNewVersionAvailable { get; set; }
		public static string NewVersionPath { get; set; }

		public static string LicenseKeyPath { get; set; }

		public static Options.UserProfile UserProfile { get; set; }
		public static string UserProfilePath { get; set; }

		public enum WindowsVersion
		{
			WinVista = 60,
			WinSeven = 61,
			WinEight = 62,
		}
		public static WindowsVersion WindowsLevel { get; set; }

		public static IGenerator NETGenerator { get; private set; }
		public static IGenerator WinRTGenerator { get; private set; }
		public static ConcurrentDictionary<Guid, Compiler> Compilers { get; private set; }

		public static string SolutionPath { get; set; }
		public static Projects.Solution Solution { get; set; }
		public static Options.RecentSolution SolutionInfo { get; set; }
		public static ObservableCollection<Projects.Project> Projects { get; set; }

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
			var sbl = new List<ServiceBinding>();
			foreach (Project p in Projects)
				sbl.AddRange(p.Namespace.GetBindings());
			return sbl;
		}

		public static void InitializeCodeGenerators(string License)
		{
			Compilers = new ConcurrentDictionary<Guid, Compiler>();
			NETGenerator = Loader.LoadModule(GenerationModule.NET, GenerationLanguage.CSharp);
			NETGenerator.Initialize(License, GeneratorOutput, GeneratorMessage);
			WinRTGenerator = Loader.LoadModule(GenerationModule.WindowsRuntime, GenerationLanguage.CSharp);
			WinRTGenerator.Initialize(License, GeneratorOutput, GeneratorMessage);
		}

		private static void GeneratorOutput(Guid ID, string output)
		{
			Compiler t;
			if(Compilers.TryGetValue(ID, out t))
				t.GeneratorOutput(output);
		}

		private static void GeneratorMessage(Guid ID, CompileMessage message)
		{
			Compiler t;
			if (Compilers.TryGetValue(ID, out t))
				t.GeneratorMessage(message);
		}

		public static void OpenSolution(string Path, Action<bool> FinishedAction)
		{
			IsLoading = true;

			//Check for backups and open the solution.
			if (File.Exists(System.IO.Path.ChangeExtension(Path, ".bak")))
			{
				var fi = new FileInfo(Path);
				var bfi = new FileInfo(System.IO.Path.ChangeExtension(Path, ".bak"));
				if (fi.LastWriteTime < bfi.LastWriteTime)
					DialogService.ShowMessageDialog(null, "Solution Load Error", "NETPath has detected that the solution '" + Path + "' was not properly closed. A newer backup exists. Would you like to use this backup?",
						new DialogAction("Yes", () => { File.Delete(Path); File.Move(System.IO.Path.ChangeExtension(Path, ".bak"), Path); }, true), new DialogAction("No", () => File.Delete(System.IO.Path.ChangeExtension(Path, ".bak")), false, true));
				else
					File.Delete(System.IO.Path.ChangeExtension(Path, ".bak"));
			}
			try
			{
				SolutionPath = Path;
				Solution = NETPath.Projects.Solution.Open(Path);
			}
			catch (Exception ex)
			{
				DialogService.ShowMessageDialog(null, "Solution Load Error", ex.Message, new DialogAction("Ok", () => FinishedAction(false), true, true));
				return;
			}
				
			//Load projects
			Projects = new ObservableCollection<Project>();
			Projects.CollectionChanged += MainScreen.Projects_CollectionChanged;
			foreach (string p in Solution.Projects)
			{
				if (File.Exists(System.IO.Path.ChangeExtension(p, ".bak")))
				{
					var fi = new FileInfo(p);
					var bfi = new FileInfo(System.IO.Path.ChangeExtension(p, ".bak"));
					if (fi.LastWriteTime < bfi.LastWriteTime)
					{
						DialogService.ShowMessageDialog(null, "Project Load Error", "NETPath has detected that the solution '" + Path + "' was not properly closed. A newer backup exists. Would you like to use this backup?",
							new DialogAction("Yes", () => { File.Delete(Path); File.Move(System.IO.Path.ChangeExtension(Path, ".bak"), Path); }, true), new DialogAction("No", () => File.Delete(System.IO.Path.ChangeExtension(Path, ".bak")), false, true));
					}
					else
					{
						File.Delete(System.IO.Path.ChangeExtension(p, ".bak"));
					}
				}

				//Open the project.
				try
				{
					Projects.Add(NETPath.Projects.Project.Open(SolutionPath, p));
				}
				catch (Exception ex)
				{
					DialogService.ShowMessageDialog(null, "Project Load Error", ex.ToString(), new DialogAction("Ok", () => FinishedAction(false), true, true));
					return;
				}
			}
			Projects.Sort(a => a.Name);

			foreach(Project p in Projects)
				InitializeDependencies(p);

			if (UserProfile.AutomaticBackupsEnabled)
				BackupTimer = new System.Threading.Timer(BackupSolution, null, (long)UserProfile.AutomaticBackupsInterval.TotalMilliseconds, (long)UserProfile.AutomaticBackupsInterval.TotalMilliseconds);

			IsLoading = false;

			FinishedAction(true);
		}

		private static void InitializeDependencies(Project Data)
		{
			foreach (DependencyProject dp in Data.DependencyProjects)
			{
				if (Projects.Any(a => a.ID == dp.ProjectID)) dp.Project = Projects.First(a => a.ID == dp.ProjectID);
				else dp.EnableAutoReload = true;

				InitializeDependencies(dp.Project);
			}
		}

		public static void BuildSolution()
		{
			MainScreen.IsBuilding = true;
			foreach (var p in Compilers)
				p.Value.Build();
			MainScreen.IsBuilding = false;
		}

		public static void SaveSolution(bool SaveProjects)
		{
			IsFinding = true;
			IsSaving = true;

			NETPath.Projects.Solution.Save(Solution);

			if (SaveProjects)
				foreach (Project p in Projects)
					NETPath.Projects.Project.Save(p);

			IsFinding = false;
			IsSaving = false;
		}

		public static async void BackupSolution(object State)
		{
			IsFinding = true;
			IsSaving = true;

			await Application.Current.Dispatcher.InvokeAsync(() =>
			{
				NETPath.Projects.Solution.Save(Solution, Path.ChangeExtension(SolutionPath, ".bak"));

				foreach (Project p in Projects)
					NETPath.Projects.Project.Save(p, Path.ChangeExtension(p.AbsolutePath, ".bak"));
			}, DispatcherPriority.Send);

			IsFinding = false;
			IsSaving = false;
		}

		public static void CloseSolution()
		{
			if (UserProfile.AutomaticBackupsEnabled)
				if(BackupTimer != null)
					BackupTimer.Dispose();

			NETPath.Projects.Solution.Save(Solution);

			foreach (Project p in Projects)
				NETPath.Projects.Project.Save(p);

			Projects.Clear();
		}
	}
}