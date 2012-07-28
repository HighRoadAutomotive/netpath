using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WCFArchitect.Compiler
{
	internal class CompilerManager
	{
		private static System.Windows.Threading.Dispatcher WPFThread = Application.Current.Dispatcher;

		public ConcurrentDictionary<string, ProjectOptions> Options { get; set; }

		public int TotalBuildSteps { get; set; }
		public int CompletedBuildSteps { get; set; }
		public bool BuildHasErrors { get; set; }

		public void Build()
		{
			Globals.MainScreen.BuildSolution.IsEnabled = false;
			Globals.MainScreen.BuildProject.IsEnabled = false;
			Globals.MainScreen.BuildStatusGroup.Visibility = Visibility.Visible;
			Options = new ConcurrentDictionary<string, ProjectOptions>();

			foreach (Projects.Project P in Globals.Projects)
			{
				P.Compiler.Messages.Clear();
				P.Compiler.Output.Blocks.Clear();
				P.Compiler.IsFinished = true;
			}
			
			var PL = Globals.Projects.Where(a => a.IsDependencyProject == false);

			//Execute pre-build
			CompletedBuildSteps = 0;
			foreach (Projects.Project Project in PL)
			{
				TotalBuildSteps += CountProjectBuildSteps(Project);
				Globals.MainScreen.BuildProgress.Maximum += TotalBuildSteps;

				Project.PreBuild(false);

				ProjectOptions npo = new ProjectOptions();
				npo.ServerOutputPaths = new List<Projects.ProjectOutputPath>(Project.ServerOutputPaths.Where(a => a.UserProfileID == Globals.UserProfile.ID));
				npo.ClientOutputPaths = new List<Projects.ProjectOutputPath>(Project.ClientOutputPaths.Where(a => a.UserProfileID == Globals.UserProfile.ID));
				npo.BuildOutputPath = Project.BuildOutputPath;
				npo.References = new List<Projects.Reference>(Project.References);
				npo.ServiceExcludedTypes = new List<string>(Project.ServiceExcludedTypes);
				npo.ServiceGenerateDataContracts = Project.ServiceGenerateDataContracts;
				npo.Framework30 = Project.Framework30;
				npo.Framework35 = Project.Framework35;
				npo.Framework35Client = Project.Framework35Client;
				npo.Framework40 = Project.Framework40;
				npo.Framework40Client = Project.Framework40Client;
				npo.Silverlight30 = Project.Silverlight30;
				npo.Silverlight40 = Project.Silverlight40;
				npo.PlatformX86 = Project.PlatformX86;
				npo.PlatformX64 = Project.PlatformX64;
				npo.PlatformItanium = Project.PlatformItanium;
				npo.PlatformAnyCPU = Project.PlatformAnyCPU;
				npo.ConfigurationDebug = Project.ConfigurationDebug;
				npo.ConfigurationRelease = Project.ConfigurationRelease;
				npo.ServerClassesInternal = Project.ServerInternalClasses;
				npo.ClientClassesInternal = Project.ClientInternalClasses;
				Options.TryAdd(Project.Name, npo);

				Project.Compiler.Messages.Clear();
				Project.Compiler.Output.Blocks.Clear();
				Project.Compiler.IsFinished = true;
			}

			//Execute build
			Task T = new Task(new Action(() =>
			{
				foreach (Projects.Project P in PL)
					BuildProject(P, true);
			}), TaskCreationOptions.LongRunning);
			T.Start();
		}

		public void BuildProject(Projects.Project Project, bool IsSolutionBuild = false)
		{
			if (IsSolutionBuild == false)
			{
				Globals.MainScreen.BuildSolution.IsEnabled = false;
				Globals.MainScreen.BuildProject.IsEnabled = false;
				CompletedBuildSteps = 0;
				TotalBuildSteps = CountProjectBuildSteps(Project);
				Globals.MainScreen.BuildProgress.Maximum = TotalBuildSteps;

				Project.PreBuild(false);
				Options = new ConcurrentDictionary<string, ProjectOptions>();

				ProjectOptions npo = new ProjectOptions();
				npo.ServerOutputPaths = new List<Projects.ProjectOutputPath>(Project.ServerOutputPaths.Where(a => a.UserProfileID == Globals.UserProfile.ID));
				npo.ClientOutputPaths = new List<Projects.ProjectOutputPath>(Project.ClientOutputPaths.Where(a => a.UserProfileID == Globals.UserProfile.ID));
				npo.BuildOutputPath = Project.BuildOutputPath;
				npo.References = new List<Projects.Reference>(Project.References);
				npo.ServiceExcludedTypes = new List<string>(Project.ServiceExcludedTypes);
				npo.ServiceGenerateDataContracts = Project.ServiceGenerateDataContracts;
				npo.Framework30 = Project.Framework30;
				npo.Framework35 = Project.Framework35;
				npo.Framework35Client = Project.Framework35Client;
				npo.Framework40 = Project.Framework40;
				npo.Framework40Client = Project.Framework40Client;
				npo.Silverlight30 = Project.Silverlight30;
				npo.Silverlight40 = Project.Silverlight40;
				npo.PlatformX86 = Project.PlatformX86;
				npo.PlatformX64 = Project.PlatformX64;
				npo.PlatformItanium = Project.PlatformItanium;
				npo.PlatformAnyCPU = Project.PlatformAnyCPU;
				npo.ConfigurationDebug = Project.ConfigurationDebug;
				npo.ConfigurationRelease = Project.ConfigurationRelease;
				npo.ServerClassesInternal = Project.ServerInternalClasses;
				npo.ClientClassesInternal = Project.ClientInternalClasses;
				Options.TryAdd(Project.Name, npo);

				Project.Compiler.Messages.Clear();
				Project.Compiler.Output.Blocks.Clear();
				Project.Compiler.IsFinished = true;
			}

			string ProjectName = (string)WPFThread.Invoke(new Func<string>(() => { return Project.Name; }), System.Windows.Threading.DispatcherPriority.Send);
			if (IsSolutionBuild == false)
			{
				Task T = new Task(new Action(() => InternalBuildProject(Project, ProjectName)), TaskCreationOptions.PreferFairness);
				T.Start();
			}
			else
			{
				InternalBuildProject(Project, ProjectName);
			}
		}

		private void InternalBuildProject(Projects.Project Project, string ProjectName)
		{
			List<CompilerOutputFile> COFL = new List<CompilerOutputFile>();
			List<Projects.DependencyProject> DPL = new List<Projects.DependencyProject>((ObservableCollection<Projects.DependencyProject>)WPFThread.Invoke(new Func<ObservableCollection<Projects.DependencyProject>>(() => { return Project.DependencyProjects; }), System.Windows.Threading.DispatcherPriority.Send));
			foreach (Projects.DependencyProject DP in DPL)
				COFL.AddRange(BuildDependency(DP, Project, ProjectName));
			COFL = new List<CompilerOutputFile>(COFL.Distinct(new CompilerOutputFileComparer()));

			Project.Compiler.Build(COFL, ProjectName);

			WPFThread.Invoke(new Action(() => Globals.MainScreen.ProjectBuildFinished(Project.Compiler.HasErrors)), System.Windows.Threading.DispatcherPriority.Send);
		}

		private List<CompilerOutputFile> BuildDependency(Projects.DependencyProject Dependency, Projects.Project Project, string ProjectName)
		{
			List<CompilerOutputFile> COFL = new List<CompilerOutputFile>();
			List<Projects.DependencyProject> DPL = new List<Projects.DependencyProject>((ObservableCollection<Projects.DependencyProject>)WPFThread.Invoke(new Func<ObservableCollection<Projects.DependencyProject>>(() => { return Dependency.DependencyProjects; }), System.Windows.Threading.DispatcherPriority.Send));
			foreach (Projects.DependencyProject DP in DPL)
				COFL.AddRange(BuildDependency(DP, Project, ProjectName));
			COFL = new List<CompilerOutputFile>(COFL.Distinct(new CompilerOutputFileComparer()));

			COFL.AddRange(Dependency.Compiler.Build(COFL, ProjectName));

			WPFThread.Invoke(new Action(() => Globals.MainScreen.ProjectBuildFinished(Project.Compiler.HasErrors)), System.Windows.Threading.DispatcherPriority.Send);
			return COFL;
		}

		public void BuildProjectOutput(Projects.Project Project, bool IsSolutionBuild = false)
		{
			Globals.MainScreen.BuildSolution.IsEnabled = false;
			Globals.MainScreen.BuildProject.IsEnabled = false;
			Globals.MainScreen.BuildProgress.Maximum += CountProjectBuildSteps(Project);

			if (IsSolutionBuild == false)
			{
				Project.PreBuild(true);
				Options = new ConcurrentDictionary<string, ProjectOptions>();

				ProjectOptions npo = new ProjectOptions();
				npo.ServerOutputPaths = new List<Projects.ProjectOutputPath>(Project.ServerOutputPaths.Where(a => a.UserProfileID == Globals.UserProfile.ID));
				npo.ClientOutputPaths = new List<Projects.ProjectOutputPath>(Project.ClientOutputPaths.Where(a => a.UserProfileID == Globals.UserProfile.ID));
				npo.BuildOutputPath = Project.BuildOutputPath;
				npo.References = new List<Projects.Reference>(Project.References);
				npo.ServiceExcludedTypes = new List<string>(Project.ServiceExcludedTypes);
				npo.ServiceGenerateDataContracts = Project.ServiceGenerateDataContracts;
				npo.Framework30 = Project.Framework30;
				npo.Framework35 = Project.Framework35;
				npo.Framework35Client = Project.Framework35Client;
				npo.Framework40 = Project.Framework40;
				npo.Framework40Client = Project.Framework40Client;
				npo.Silverlight30 = Project.Silverlight30;
				npo.Silverlight40 = Project.Silverlight40;
				npo.PlatformX86 = Project.PlatformX86;
				npo.PlatformX64 = Project.PlatformX64;
				npo.PlatformItanium = Project.PlatformItanium;
				npo.PlatformAnyCPU = Project.PlatformAnyCPU;
				npo.ConfigurationDebug = Project.ConfigurationDebug;
				npo.ConfigurationRelease = Project.ConfigurationRelease;
				npo.ServerClassesInternal = Project.ServerInternalClasses;
				npo.ClientClassesInternal = Project.ClientInternalClasses;
				Options.TryAdd(Project.Name, npo);

				Project.Compiler.Messages.Clear();
				Project.Compiler.Output.Blocks.Clear();
				Project.Compiler.IsFinished = true;
			}

			string ProjectName = Project.Name;
			Task T = new Task(new Action(() => InternalBuildOutput(Project, ProjectName)), TaskCreationOptions.PreferFairness);
			T.Start();
		}

		private void InternalBuildOutput(Projects.Project Project, string ProjectName)
		{
			List<CompilerOutputFile> COFL = new List<CompilerOutputFile>();
			List<Projects.DependencyProject> DPL = new List<Projects.DependencyProject>((ObservableCollection<Projects.DependencyProject>)WPFThread.Invoke(new Func<ObservableCollection<Projects.DependencyProject>>(() => { return Project.DependencyProjects; }), System.Windows.Threading.DispatcherPriority.Send));
			foreach (Projects.DependencyProject DP in DPL)
				COFL.AddRange(BuildDependencyOutput(DP, Project, ProjectName));
			COFL = new List<CompilerOutputFile>(COFL.Distinct(new CompilerOutputFileComparer()));

			Project.Compiler.OutputFiles(ProjectName);

			WPFThread.Invoke(new Action(() => Globals.MainScreen.ProjectBuildFinished(Project.Compiler.HasErrors)), System.Windows.Threading.DispatcherPriority.Send);
		}

		private List<CompilerOutputFile> BuildDependencyOutput(Projects.DependencyProject Dependency, Projects.Project Project, string ProjectName)
		{
			List<CompilerOutputFile> COFL = new List<CompilerOutputFile>();
			List<Projects.DependencyProject> DPL = new List<Projects.DependencyProject>((ObservableCollection<Projects.DependencyProject>)WPFThread.Invoke(new Func<ObservableCollection<Projects.DependencyProject>>(() => { return Dependency.DependencyProjects; }), System.Windows.Threading.DispatcherPriority.Send));
			foreach (Projects.DependencyProject DP in DPL)
				COFL.AddRange(BuildDependencyOutput(DP, Project, ProjectName));
			COFL = new List<CompilerOutputFile>(COFL.Distinct(new CompilerOutputFileComparer()));

			COFL.AddRange(Dependency.Compiler.OutputFiles(ProjectName));

			WPFThread.Invoke(new Action(() => Globals.MainScreen.ProjectBuildFinished(Project.Compiler.HasErrors)), System.Windows.Threading.DispatcherPriority.Send);
			return COFL;
		}

		private int CountProjectBuildSteps(Projects.Project Project)
		{
			int Count = 0;

			foreach (Projects.DependencyProject DP in Project.DependencyProjects)
				Count += CountProjectBuildSteps(DP);

			Count++;

			return Count;
		}
	}

	internal class CompilerOutputFileComparer : IEqualityComparer<CompilerOutputFile>
	{
		public bool Equals(CompilerOutputFile x, CompilerOutputFile y)
		{
			if (x.FileName == y.FileName) return true;
			return false;
		}

		public int GetHashCode(CompilerOutputFile product)
		{
			if (Object.ReferenceEquals(product, null)) return 0;
			return product.FileName == null ? 0 : product.FileName.GetHashCode();
		}

	}
}