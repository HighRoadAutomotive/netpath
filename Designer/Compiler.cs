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
using WCFArchitect.Projects;

namespace WCFArchitect
{
	public class Compiler
	{
		private Interface.Navigator NavWindow { get; set; }
		private Paragraph OutputBlock { get; set; }

		private readonly ProcessStartInfo psi;
		private readonly Process cp;

		public Compiler(Interface.Navigator NavWindow)
		{
			this.NavWindow = NavWindow;

			OutputBlock = new Paragraph();
			this.NavWindow.OutputBox.Document.Blocks.Add(OutputBlock);

			//Setup the basic process start info, this is reused.
			psi = new ProcessStartInfo(Path.Combine(Globals.ApplicationPath, "wasc.exe"));
			psi.Arguments = string.Format("{0} {1} -stderr", Globals.SolutionPath, NavWindow.Project.AbsolutePath);
			psi.WorkingDirectory = Globals.ApplicationPath;
			psi.CreateNoWindow = true;
			psi.RedirectStandardError = true;
			psi.RedirectStandardOutput = true;
			psi.UseShellExecute = false;
			psi.WindowStyle = ProcessWindowStyle.Hidden;
			cp = new Process();
			cp.StartInfo = psi;
			cp.ErrorDataReceived += StdErr;
			cp.OutputDataReceived += StdOut;
		}

		public async void Build()
		{
			OutputBlock.Inlines.Clear();
			OutputBlock.Inlines.Add(new Run(string.Format("------ Building Project: {0} ------", NavWindow.Project.Name)));
			OutputBlock.Inlines.Add(new LineBreak());
			NavWindow.ErrorCount = 0;
			NavWindow.ErrorList.Items.Clear();

			//Run the process
			cp.Start();
			cp.BeginErrorReadLine();
			cp.BeginOutputReadLine();
			await BuildFinished();
			cp.CancelErrorRead();
			cp.CancelOutputRead();
			cp.Close();

			if (NavWindow.ErrorCount == 0) NavWindow.ErrorCount = null;
			OutputBlock.Inlines.Add(new Run(string.Format("====== Finished Project: {0} ======", NavWindow.Project.Name)));
		}

		private Task BuildFinished()
		{
			return Task.Run(() => cp.WaitForExit());
		}
		
		private void StdOut(object sender, DataReceivedEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Data)) return;

			Application.Current.Dispatcher.Invoke(() =>
													  {
														  var tr = new Run(e.Data);
														  OutputBlock.Inlines.Add(tr);
														  OutputBlock.Inlines.Add(new LineBreak());
														  if (e.Data.IndexOf("ERROR", StringComparison.InvariantCulture) >= 0) tr.Foreground = Brushes.DarkRed;
														  if (e.Data.IndexOf("WARN", StringComparison.InvariantCulture) >= 0) tr.Foreground = Brushes.DarkOrange;
													  }, DispatcherPriority.Send);
		}

		private void StdErr(object sender, DataReceivedEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Data)) return;

			Application.Current.Dispatcher.Invoke(() =>
				                                      {
					                                      try
					                                      {
															  var dcs = new DataContractSerializer(typeof (CompileMessage), new DataContractSerializerSettings {MaxItemsInObjectGraph = Int32.MaxValue, IgnoreExtensionDataObject = true, SerializeReadOnlyTypes = true, PreserveObjectReferences = true});
															  var tr = XmlReader.Create(new StringReader(e.Data), new XmlReaderSettings() {CloseInput = true, Async = false});
															  var t = (CompileMessage) dcs.ReadObject(tr);
															  NavWindow.ErrorList.Items.Add(t);
															  NavWindow.ErrorCount++;
					                                      }
					                                      catch (Exception)
					                                      {
															  var tr = new Run(e.Data);
															  OutputBlock.Inlines.Add(tr);
															  OutputBlock.Inlines.Add(new LineBreak());
															  if (e.Data.IndexOf("ERROR", StringComparison.InvariantCulture) >= 0) tr.Foreground = Brushes.DarkRed;
															  if (e.Data.IndexOf("WARN", StringComparison.InvariantCulture) >= 0) tr.Foreground = Brushes.DarkOrange;
														  }
													  }, DispatcherPriority.Send);
		}
	}
}