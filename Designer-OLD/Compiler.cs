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
using EllipticBit.Controls.WPF.Dialogs;

namespace NETPath
{
	public class Compiler
	{
		private Interface.Navigator NavWindow { get; set; }
		private Paragraph OutputBlock { get; set; }

		private readonly IGenerator NET;

		public Compiler(Interface.Navigator NavWindow)
		{
			this.NavWindow = NavWindow;

			OutputBlock = new Paragraph();
			this.NavWindow.OutputBox.Document.Blocks.Add(OutputBlock);

			NET = Globals.CSharpGenerator;
		}

		public async void Build()
		{
			Globals.MainScreen.IsBuilding = true;
			OutputBlock.Inlines.Clear();
			OutputBlock.Inlines.Add(new Run(string.Format("------ Building Project: {0} ------", NavWindow.Project.Name)));
			OutputBlock.Inlines.Add(new LineBreak());
			NavWindow.ErrorCount = 0;
			NavWindow.ErrorList.Items.Clear();

			//Run project code generation
			if (NET.IsInitialized)
				await NET.BuildAsync();
			else
				GeneratorOutput("FATAL ERROR: Unable to initialize any code generators.");

			if (NavWindow.ErrorCount == 0) NavWindow.ErrorCount = null;
			OutputBlock.Inlines.Add(new Run(string.Format("====== Finished Project: {0} ======", NavWindow.Project.Name)));
			Globals.MainScreen.IsBuilding = false;
		}

		public async static void BuildProject(Projects.Project Project)
		{
			IGenerator NET = Globals.CSharpGenerator;

			//Run project code generation
			if (NET.IsInitialized)
				await NET.BuildAsync();
			else
				DialogService.ShowMessageDialog("COMPILER", "FATAL ERROR: Unable to initialize any code generators.", "NETPath was unable to initialize any code generators. This is usually caused by a corrupted installation or an invalid license. Please reinstall the software to continue.");
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