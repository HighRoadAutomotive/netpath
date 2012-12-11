using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using System.Runtime.Serialization;
using System.Windows.Threading;
using System.Xml;
using NETPath.Projects;
using NETPath.Generators.Interfaces;

namespace NETPath
{
	public class Compiler
	{
		private Interface.Navigator NavWindow { get; set; }
		private Paragraph OutputBlock { get; set; }

		private readonly IGenerator NET;
		private readonly IGenerator WinRT;

		public Compiler(Interface.Navigator NavWindow)
		{
			this.NavWindow = NavWindow;

			OutputBlock = new Paragraph();
			this.NavWindow.OutputBox.Document.Blocks.Add(OutputBlock);

			NET = Globals.NETGenerator;
			WinRT = Globals.WinRTGenerator;

			Globals.Compilers.TryAdd(NavWindow.Project.ID, this);
		}

		public async void Build()
		{
			OutputBlock.Inlines.Clear();
			OutputBlock.Inlines.Add(new Run(string.Format("------ Building Project: {0} ------", NavWindow.Project.Name)));
			OutputBlock.Inlines.Add(new LineBreak());
			NavWindow.ErrorCount = 0;
			NavWindow.ErrorList.Items.Clear();

			//Run project code generation
			if (NET.IsInitialized && WinRT.IsInitialized)
			{
				await NET.BuildAsync(NavWindow.Project);
				if (NavWindow.Project.ClientGenerationTargets.Any(a => a.Framework == ProjectGenerationFramework.WIN8)) await WinRT.BuildAsync(NavWindow.Project, true);
			}
			else if (WinRT.IsInitialized)
				await WinRT.BuildAsync(NavWindow.Project);
			else
				GeneratorOutput("FATAL ERROR: Unable to initialize any code generators.");

			if (NavWindow.ErrorCount == 0) NavWindow.ErrorCount = null;
			OutputBlock.Inlines.Add(new Run(string.Format("====== Finished Project: {0} ======", NavWindow.Project.Name)));
		}

	
		public void GeneratorOutput(string output)
		{
			if (string.IsNullOrEmpty(output)) return;
			Application.Current.Dispatcher.Invoke(() =>
			{
				var tr = new Run(output);
				OutputBlock.Inlines.Add(tr);
				OutputBlock.Inlines.Add(new LineBreak());
				if (output.IndexOf("ERROR", StringComparison.InvariantCulture) >= 0) tr.Foreground = Brushes.DarkRed;
				if (output.IndexOf("WARN", StringComparison.InvariantCulture) >= 0) tr.Foreground = Brushes.DarkOrange;
			}, DispatcherPriority.Send);
		}

		public void GeneratorMessage(CompileMessage message)
		{
			if (message == null) return;
			Application.Current.Dispatcher.Invoke(() =>
			{
				NavWindow.ErrorList.Items.Add(message);
				NavWindow.ErrorCount++;
			}, DispatcherPriority.Send);
		}
	}
}