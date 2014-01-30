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
using Prospective.Controls.Dialogs;

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

			NET = Globals.NETGenerator;

			Globals.Compilers.TryAdd(NavWindow.Project.ID, this);
		}

		public async void Build()
		{
			Globals.MainScreen.IsBuilding = true;
			OutputBlock.Inlines.Clear();
			OutputBlock.Inlines.Add(new Run(string.Format("------ Building Project: {0} ------", NavWindow.Project.Name)));
			OutputBlock.Inlines.Add(new LineBreak());
			NavWindow.ErrorCount = 0;
			NavWindow.ErrorList.Items.Clear();

			//Rebuild DRE Service name lists
			RebuildDREServiceList();

			//Run project code generation
			if (NET.IsInitialized)
				await NET.BuildAsync(NavWindow.Project);
			else
				GeneratorOutput("FATAL ERROR: Unable to initialize any code generators.");

			if (NavWindow.ErrorCount == 0) NavWindow.ErrorCount = null;
			OutputBlock.Inlines.Add(new Run(string.Format("====== Finished Project: {0} ======", NavWindow.Project.Name)));
			Globals.MainScreen.IsBuilding = false;
		}

		public async static void BuildProject(Projects.Project project)
		{
			IGenerator NET = Globals.NETGenerator;

			//Rebuild DRE Service name lists
			RebuildDREServiceList();

			//Run project code generation
			if (NET.IsInitialized)
				await NET.BuildAsync(project);
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

		internal static void RebuildDREServiceList()
		{
			//Get all services with DRE in the solution.
			var sl = new List<Service>();
			sl.AddRange(DataRevisionServiceScan(Globals.Project.Namespace));

			//Clean all DataRevisionServiceNames lists
			ResetDRSNames(Globals.Project.Namespace);

			//Rebuild DRE service name lists
			foreach (var sv in sl)
				foreach (var dre in sv.ServiceOperations.Where(a => a.GetType() == typeof(DataChangeMethod)).Cast<DataChangeMethod>())
				{
					Data t = DataReivsionReferenceRetrieve(sv.Parent.Owner, sv.Parent.Owner.Namespace, dre.ReturnType.ID);
					if (t == null) continue;
					if (dre.IsHidden) continue;
					t.DataRevisionServiceNames.Add(new DataRevisionName(string.Format("{0}.{1}", sv.Parent, sv.Name), true, dre.UseServerAwaitPattern, dre.UseClientAwaitPattern, sv.Parent.Owner.ID));
					t.DataRevisionServiceNames.Add(new DataRevisionName(string.Format("{0}.{1}", sv.Parent, sv.HasClientType ? sv.ClientType.Name : sv.Name), false, dre.UseServerAwaitPattern, dre.UseClientAwaitPattern, sv.Parent.Owner.ID));
				}
		}

		internal static List<Service> DataRevisionServiceScan(Namespace Namespace)
		{
			List<Service> sl = Namespace.Services.Where(sv => sv.HasDCMOperations).ToList();

			foreach (var n in Namespace.Children)
				sl.AddRange(DataRevisionServiceScan(n));

			return sl;
		}

		internal static void ResetDRSNames(Namespace Namespace)
		{
			foreach (var drs in Namespace.Data)
				drs.DataRevisionServiceNames = new List<DataRevisionName>();

			foreach (var n in Namespace.Children)
				ResetDRSNames(n);
		}

		internal static Data DataReivsionReferenceRetrieve(Project Project, Namespace Namespace, Guid TypeID)
		{
			var d = Namespace.Data.FirstOrDefault(a => a.ID == TypeID);
			if (d != null) return d;

			return Namespace.Children.Select(n => DataReivsionReferenceRetrieve(Project, n, TypeID)).FirstOrDefault(t => t != null);
		}
	}
}