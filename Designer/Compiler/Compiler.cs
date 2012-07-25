using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WCFArchitect.Compiler
{
	internal enum CompilerMessageStage
	{
		Verify,
		Server,
		Proxy,
		Client,
		Output,
		Complete,
	}

	internal class ProjectOptions
	{
		public List<Projects.ProjectOutputPath> ServerOutputPaths { get; set; }
		public List<Projects.ProjectOutputPath> ClientOutputPaths { get; set; }
		public string BuildOutputPath { get; set; }
		public List<Projects.Reference> References { get; set; }
		public List<string> ServiceExcludedTypes { get; set; }
		public bool ServiceGenerateDataContracts { get; set; }

		public bool Framework30 { get; set; }
		public bool Framework35 { get; set; }
		public bool Framework35Client { get; set; }
		public bool Framework40 { get; set; }
		public bool Framework40Client { get; set; }
		public bool Silverlight30 { get; set; }
		public bool Silverlight40 { get; set; }

		public bool ServerClassesInternal { get; set; }
		public bool ClientClassesInternal { get; set; }
	}

	internal class Compiler
	{
		private static System.Windows.Threading.Dispatcher WPFThread = Application.Current.Dispatcher;

		public Projects.Project Project { get; set; }
		private object clock = new object();
		private bool isFinished;
		public bool IsFinished { get { lock (clock) { return isFinished; } } set { lock (clock) { isFinished = value; } } }

		public bool HasErrors { get; set; }
		public ObservableCollection<CompileMessage> Messages { get; private set; }
		public FlowDocument Output { get; private set; }
		private Paragraph VerifyCodeOutputHeader;
		private Paragraph ServerBuildOutputHeader;
		private Paragraph ProxyGenerateOutputHeader;
		private Paragraph ClientBuildOutputHeader;
		private Paragraph FileOutputHeader;
		private Paragraph BuildCompleteHeader;
		private Paragraph VerifyCodeOutput;
		private Paragraph ServerBuildOutput;
		private Paragraph ProxyGenerateOutput;
		private Paragraph ClientBuildOutput;
		private Paragraph FileOutput;

		private Microsoft.CSharp.CSharpCodeProvider CSCompilerNET30;
		private Microsoft.CSharp.CSharpCodeProvider CSCompilerNET35;
		private Microsoft.CSharp.CSharpCodeProvider CSCompilerNET40;

		private string SvcUtilPath;
		private string ServerAssemblyName;
		private string ClientAssemblyName;
		private bool IsDependencyProject;

		private List<Projects.ProjectOutputPath> GetServerOutputPaths(string ProjectName) { ProjectOptions po = null; Globals.CompilerManager.Options.TryGetValue(ProjectName, out po); return new List<Projects.ProjectOutputPath>(po.ServerOutputPaths.Where(a => a.IsDependencyOutputPath == this.Project.IsDependencyProject)); }
		private List<Projects.ProjectOutputPath> GetClientOutputPaths(string ProjectName) { ProjectOptions po = null; Globals.CompilerManager.Options.TryGetValue(ProjectName, out po); return new List<Projects.ProjectOutputPath>(po.ClientOutputPaths.Where(a => a.IsDependencyOutputPath == this.Project.IsDependencyProject)); }
		public string GetBuildOutputPath(string ProjectName, Projects.ProjectOutputFramework Framework) { ProjectOptions po = null; Globals.CompilerManager.Options.TryGetValue(ProjectName, out po); if (Framework == Projects.ProjectOutputFramework.NET30) return po.BuildOutputPath + "NET30\\"; if (Framework == Projects.ProjectOutputFramework.NET35) return po.BuildOutputPath + "NET35\\"; if (Framework == Projects.ProjectOutputFramework.NET35Client) return po.BuildOutputPath + "NET35Client\\"; if (Framework == Projects.ProjectOutputFramework.NET40) return po.BuildOutputPath + "NET40\\"; if (Framework == Projects.ProjectOutputFramework.NET40Client) return po.BuildOutputPath + "NET40Client\\"; if (Framework == Projects.ProjectOutputFramework.SL30) return po.BuildOutputPath + "SL30\\"; if (Framework == Projects.ProjectOutputFramework.SL40) return po.BuildOutputPath + "SL40\\"; if (Framework == Projects.ProjectOutputFramework.SL50) return po.BuildOutputPath + "SL50\\"; return po.BuildOutputPath; }
		private List<Projects.Reference> GetReferences(string ProjectName) { ProjectOptions po = null; Globals.CompilerManager.Options.TryGetValue(ProjectName, out po); return po.References; }
		private List<string> GetServiceExcludedTypes(string ProjectName) { ProjectOptions po = null; Globals.CompilerManager.Options.TryGetValue(ProjectName, out po); return po.ServiceExcludedTypes; }
		private bool GetServiceGenerateDataContracts(string ProjectName) { if ((bool)WPFThread.Invoke(new Func<bool>(() => this.Project.ProjectHasServices()), System.Windows.Threading.DispatcherPriority.Send) == false) { return true; } else { ProjectOptions po = null; Globals.CompilerManager.Options.TryGetValue(ProjectName, out po); return po.ServiceGenerateDataContracts; } }		//If the project contains no Services it MUST be outputted using the /dconly option for the proxy generator.
		private bool IsFramework30(string ProjectName) { ProjectOptions po = null; Globals.CompilerManager.Options.TryGetValue(ProjectName, out po); return po.Framework30; }
		private bool IsFramework35(string ProjectName) { ProjectOptions po = null; Globals.CompilerManager.Options.TryGetValue(ProjectName, out po); return po.Framework35; }
		private bool IsFramework35Client (string ProjectName) { ProjectOptions po = null; Globals.CompilerManager.Options.TryGetValue(ProjectName, out po); return po.Framework35Client; }
		private bool IsFramework40(string ProjectName) { ProjectOptions po = null; Globals.CompilerManager.Options.TryGetValue(ProjectName, out po); return po.Framework40; }
		private bool IsFramework40Client(string ProjectName) { ProjectOptions po = null; Globals.CompilerManager.Options.TryGetValue(ProjectName, out po); return po.Framework40Client; }
		private bool IsSilverlight30(string ProjectName) { ProjectOptions po = null; Globals.CompilerManager.Options.TryGetValue(ProjectName, out po); return po.Silverlight30; }
		private bool IsSilverlight40(string ProjectName) { ProjectOptions po = null; Globals.CompilerManager.Options.TryGetValue(ProjectName, out po); return po.Silverlight40; }
		private bool IsServerClassesInternal(string ProjectName) { ProjectOptions po = null; Globals.CompilerManager.Options.TryGetValue(ProjectName, out po); return po.ServerClassesInternal; }
		private bool IsClientClassesInternal(string ProjectName) { ProjectOptions po = null; Globals.CompilerManager.Options.TryGetValue(ProjectName, out po); return po.ClientClassesInternal; }

		public string ServerClassVisibility(bool GenerateAssemblyCode, string ProjectName) { if (GenerateAssemblyCode == true) { return "public"; } if (IsServerClassesInternal(ProjectName) == true) { return "internal"; } else { return "public"; } }
		public string ClientClassVisibility(bool GenerateAssemblyCode, string ProjectName) { if (GenerateAssemblyCode == true) { return "public"; } if (IsClientClassesInternal(ProjectName) == true) { return "internal"; } else { return "public"; } }

		public Compiler(Projects.Project Project)
		{
			this.Project = Project;
			this.Messages = new ObservableCollection<CompileMessage>();

			Output = new FlowDocument();
			VerifyCodeOutputHeader = new Paragraph(new Run("\t--- Pre-Build Data Verification ---"));
			VerifyCodeOutputHeader.FontFamily = new System.Windows.Media.FontFamily("Consolas");
			VerifyCodeOutputHeader.FontSize = 12.0D;
			ServerBuildOutputHeader = new Paragraph(new Run("\t--- Server Build Output ---"));
			ServerBuildOutputHeader.FontFamily = new System.Windows.Media.FontFamily("Consolas");
			ServerBuildOutputHeader.FontSize = 12.0D;
			ProxyGenerateOutputHeader = new Paragraph(new Run("\t--- Proxy Generation Output ---"));
			ProxyGenerateOutputHeader.FontFamily = new System.Windows.Media.FontFamily("Consolas");
			ProxyGenerateOutputHeader.FontSize = 12.0D;
			ClientBuildOutputHeader = new Paragraph(new Run("\t--- Client Build Output ---"));
			ClientBuildOutputHeader.FontFamily = new System.Windows.Media.FontFamily("Consolas");
			ClientBuildOutputHeader.FontSize = 12.0D;
			FileOutputHeader = new Paragraph(new Run("\t--- File Output ---"));
			FileOutputHeader.FontFamily = new System.Windows.Media.FontFamily("Consolas");
			FileOutputHeader.FontSize = 12.0D;
			BuildCompleteHeader = new Paragraph(new Run("\t--- Build Completed Successfully ---"));
			BuildCompleteHeader.FontFamily = new System.Windows.Media.FontFamily("Consolas");
			BuildCompleteHeader.FontSize = 12.0D;
			VerifyCodeOutput = new Paragraph();
			VerifyCodeOutput.FontFamily = new System.Windows.Media.FontFamily("Consolas");
			VerifyCodeOutput.FontSize = 12.0D;
			ServerBuildOutput = new Paragraph();
			ServerBuildOutput.FontFamily = new System.Windows.Media.FontFamily("Consolas");
			ServerBuildOutput.FontSize = 12.0D;
			ProxyGenerateOutput = new Paragraph();
			ProxyGenerateOutput.FontFamily = new System.Windows.Media.FontFamily("Consolas");
			ProxyGenerateOutput.FontSize = 12.0D;
			ClientBuildOutput = new Paragraph();
			ClientBuildOutput.FontFamily = new System.Windows.Media.FontFamily("Consolas");
			ClientBuildOutput.FontSize = 12.0D;
			FileOutput = new Paragraph();
			FileOutput.FontFamily = new System.Windows.Media.FontFamily("Consolas");
			FileOutput.FontSize = 12.0D;

			CSCompilerNET30 = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<String, String> { { "CompilerVersion", "v2.0" } });
			CSCompilerNET35 = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<String, String> { { "CompilerVersion", "v3.5" } });
			CSCompilerNET40 = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<String, String> { { "CompilerVersion", "v4.0" } });

		}

		public List<CompilerOutputFile> Build(List<CompilerOutputFile> DependencyAssemblies, string ProjectName)
		{
			IsFinished = false;

			List<CompilerOutputFile> COFL = new List<CompilerOutputFile>(InternalBuild(DependencyAssemblies, ProjectName).Where(a => a.FileType == CompilerOutputFileType.Assembly));

			IsFinished = true;
			return COFL;
		}

		public List<CompilerOutputFile> OutputFiles(string ProjectName)
		{
			IsFinished = false;

			List<CompilerOutputFile> COFL = new List<CompilerOutputFile>(InternalOutputFiles(ProjectName).Where(a => a.FileType == CompilerOutputFileType.Assembly));

			IsFinished = true;
			return COFL;
		}

		private List<CompilerOutputFile> InternalBuild(List<CompilerOutputFile> DependencyAssemblies, string ProjectName)
		{
			List<CompilerOutputFile> OutputFiles = new List<CompilerOutputFile>();

			WPFThread.Invoke(new Action(() =>
			{
				if (Project.IsDependencyProject == false)
				{
					ServerAssemblyName = Project.ServerAssemblyName;
					ClientAssemblyName = Project.ClientAssemblyName;
				}
				else
				{
					ServerAssemblyName = Helpers.RegExs.ReplaceSpaces.Replace(Project.Name, ".") + ".Server";
					ClientAssemblyName = Helpers.RegExs.ReplaceSpaces.Replace(Project.Name, ".") + ".Client";
				}

				SvcUtilPath = Globals.UserProfile.SvcUtilPath;
				IsDependencyProject = Project.IsDependencyProject;

				OutputFiles.Clear();

				Messages.Clear();
				Output.Blocks.Clear();
				VerifyCodeOutput.Inlines.Clear();
				ServerBuildOutput.Inlines.Clear();
				ProxyGenerateOutput.Inlines.Clear();
				ClientBuildOutput.Inlines.Clear();
				FileOutput.Inlines.Clear();
				HasErrors = false;

				Output.Blocks.Add(VerifyCodeOutputHeader);
				Output.Blocks.Add(VerifyCodeOutput);
				if (Project.VerifyCode() == false) HasErrors = true;
				if (HasErrors == true) return;

			}), System.Windows.Threading.DispatcherPriority.Send);

			if (IsFramework30(ProjectName) == true)
			{
				if (Directory.Exists(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30)) == false)
					Directory.CreateDirectory(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30));
			}
			if (IsFramework35(ProjectName) == true)
			{
				if (Directory.Exists(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35)) == false)
					Directory.CreateDirectory(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35));
			}
			if (IsFramework35Client(ProjectName) == true)
			{
				if (Directory.Exists(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client)) == false)
					Directory.CreateDirectory(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client));
			}
			if (IsFramework40(ProjectName) == true)
			{
				if (Directory.Exists(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40)) == false)
					Directory.CreateDirectory(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40));
			}
			if (IsFramework40Client(ProjectName) == true)
			{
				if (Directory.Exists(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client)) == false)
					Directory.CreateDirectory(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client));
			}

			//Cleanup files.
			try
			{
				List<string> Delete = new List<string>();
				if (IsFramework30((ProjectName)) == true)
				{
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), "*.config"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), "*.err"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), "*.out")); ;
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), "*.wsdl"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), "*.xsd"));
				}
				if (IsFramework35(ProjectName) == true)
				{
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), "*.config"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), "*.err"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), "*.out"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), "*.wsdl"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), "*.xsd"));
				}
				if (IsFramework35Client(ProjectName) == true)
				{
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), "*.config"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), "*.err"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), "*.out"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), "*.wsdl"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), "*.xsd"));
				}
				if (IsFramework40(ProjectName) == true)
				{
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), "*.config"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), "*.err"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), "*.out"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), "*.wsdl"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), "*.xsd"));
				}
				if (IsFramework40Client(ProjectName) == true)
				{
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), "*.config"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), "*.err"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), "*.out"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), "*.wsdl"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), "*.xsd"));
				}
				foreach (string F in Delete)
					File.Delete(F);
			}
			catch { }

			//Generate Server Code
			if (HasErrors == false)
			{
				if (IsFramework30(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 3.0 Server Code", CompilerMessageStage.Server);
					WPFThread.Invoke(new Func<string>(() => Project.GenerateServerCode30(false, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET30, CompilerOutputFileType.Code));
				}
				if (IsFramework35(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 3.5 Server Code", CompilerMessageStage.Server);
					WPFThread.Invoke(new Func<string>(() => Project.GenerateServerCode35(false, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET35, CompilerOutputFileType.Code));
				}
				if (IsFramework35Client(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 3.5 (Client Profile) Server Code", CompilerMessageStage.Server);
					WPFThread.Invoke(new Func<string>(() => Project.GenerateServerCode35Client(false, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET35Client, CompilerOutputFileType.Code));
				}
				if (IsFramework40(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 4.0 Server Code", CompilerMessageStage.Server);
					WPFThread.Invoke(new Func<string>(() => Project.GenerateServerCode40(false, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET40, CompilerOutputFileType.Code));
				}
				if (IsFramework40Client(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 4.0 (Client Profile) Server Code", CompilerMessageStage.Server);
					WPFThread.Invoke(new Func<string>(() => Project.GenerateServerCode40Client(false, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET40Client, CompilerOutputFileType.Code));
				}
			}

			//Build Server Assembly
			AddMessageStage(CompilerMessageStage.Server);
			if (HasErrors == false)
			{
				if (IsFramework30(ProjectName) == true)
				{
					AddOutputLine("Building the .NET Framework 3.0 Server Assembly", CompilerMessageStage.Server);
					if (CompileServer30(ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
				}
				if (IsFramework35(ProjectName) == true)
				{
					AddOutputLine("Building the .NET Framework 3.5 Server Assembly", CompilerMessageStage.Server);
					if (CompileServer35(ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
				}
				if (IsFramework35Client(ProjectName) == true)
				{
					AddOutputLine("Building the .NET Framework 3.5 (Client Profile) Server Assembly", CompilerMessageStage.Server);
					if (CompileServer35Client(ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
				}
				if (IsFramework40(ProjectName) == true)
				{
					AddOutputLine("Building the .NET Framework 4.0 Server Assembly", CompilerMessageStage.Server);
					if (CompileServer40(ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
				}
				if (IsFramework40Client(ProjectName) == true)
				{
					AddOutputLine("Building the .NET Framework 4.0 (Client Profile) Server Assembly", CompilerMessageStage.Server);
					if (CompileServer40Client(ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
				}
			}

			AddMessageStage(CompilerMessageStage.Proxy);
			AddMessageStage(CompilerMessageStage.Client);

			//Generate Client Code
			if (HasErrors == false)
			{
				if (IsFramework30(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 3.0 Client Code", CompilerMessageStage.Client);
					WPFThread.Invoke(new Func<string>(() => Project.GenerateClientCode30(false, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET30, CompilerOutputFileType.Code));
				}
				if (IsFramework35(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 3.5 Client Code", CompilerMessageStage.Client);
					WPFThread.Invoke(new Func<string>(() => Project.GenerateClientCode35(false, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET35, CompilerOutputFileType.Code));
				}
				if (IsFramework35Client(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 3.5 (Client Profile) Client Code", CompilerMessageStage.Client);
					WPFThread.Invoke(new Func<string>(() => Project.GenerateClientCode35Client(false, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET35Client, CompilerOutputFileType.Code));
				}
				if (IsFramework40(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 4.0 Client Code", CompilerMessageStage.Client);
					WPFThread.Invoke(new Func<string>(() => Project.GenerateClientCode40(false, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET40, CompilerOutputFileType.Code));
				}
				if (IsFramework40Client(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 4.0 (Client Profile) Client Code", CompilerMessageStage.Client);
					WPFThread.Invoke(new Func<string>(() => Project.GenerateClientCode40Client(false, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET40Client, CompilerOutputFileType.Code));
				}
			}

			//Build Client Assembly
			if (HasErrors == false)
			{
				if (IsFramework30(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 3.0 Client Service Proxy Assembly", CompilerMessageStage.Proxy);
					if (GenerateProxy30(true, ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
					AddOutputLine("Building the .NET Framework 3.0 Client Assembly", CompilerMessageStage.Client);
					if (HasErrors == false) if (CompileClient30(ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
				}
				if (IsFramework35(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 3.5 Client Service Proxy Assembly", CompilerMessageStage.Proxy);
					if (GenerateProxy35(true, ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
					AddOutputLine("Building the .NET Framework 3.5 Client Assembly", CompilerMessageStage.Client);
					if (HasErrors == false) if (CompileClient35(ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
				}
				if (IsFramework35Client(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 3.5 (Client Profile) Client Service Proxy Assembly", CompilerMessageStage.Proxy);
					if (GenerateProxy35Client(true, ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
					AddOutputLine("Building the .NET Framework 3.5 (Client Profile) Client Assembly", CompilerMessageStage.Client);
					if (HasErrors == false) if (CompileClient35Client(ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
				}
				if (IsFramework40(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 4.0 Client Service Proxy Assembly", CompilerMessageStage.Proxy);
					if (GenerateProxy40(true, ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
					AddOutputLine("Building the .NET Framework 4.0 Client Assembly", CompilerMessageStage.Client);
					if (HasErrors == false) if (CompileClient40(ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
				}
				if (IsFramework40Client(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 4.0 (Client Profile) Client Service Proxy Assembly", CompilerMessageStage.Proxy);
					if (GenerateProxy40Client(true, ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
					AddOutputLine("Building the .NET Framework 4.0 (Client Profile) Client Assembly", CompilerMessageStage.Client);
					if (HasErrors == false) if (CompileClient40Client(ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
				}
			}

			//Generate Client Proxy
			if (HasErrors == false)
			{
				if (IsFramework30((ProjectName)) == true)
				{
					AddOutputLine("Generating the .NET Framework 3.0 Client Service Proxy Code", CompilerMessageStage.Proxy);
					if (GenerateProxy30(false, ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
					if (HasErrors == false) OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), ClientAssemblyName + ".Services", false, Projects.ProjectOutputFramework.NET30, CompilerOutputFileType.Code));
				}
				if (IsFramework35(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 3.5 Client Service Proxy Code", CompilerMessageStage.Proxy);
					if (GenerateProxy35(false, ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
					if (HasErrors == false) OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), ClientAssemblyName + ".Services", false, Projects.ProjectOutputFramework.NET35, CompilerOutputFileType.Code));
				}
				if (IsFramework35Client(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 3.5 (Client Profile) Client Service Proxy Code", CompilerMessageStage.Proxy);
					if (GenerateProxy35Client(false, ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
					if (HasErrors == false) OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), ClientAssemblyName + ".Services", false, Projects.ProjectOutputFramework.NET35Client, CompilerOutputFileType.Code));
				}
				if (IsFramework40(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 4.0 Client Service Proxy Code", CompilerMessageStage.Proxy);
					if (GenerateProxy40(false, ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
					if (HasErrors == false) OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), ClientAssemblyName + ".Services", false, Projects.ProjectOutputFramework.NET40, CompilerOutputFileType.Code));
				}
				if (IsFramework40Client(ProjectName) == true)
				{
					AddOutputLine("Generating the .NET Framework 4.0 (Client Profile) Client Service Proxy Code", CompilerMessageStage.Proxy);
					if (GenerateProxy40Client(false, ProjectName, DependencyAssemblies, OutputFiles) == false) HasErrors = true;
					if (HasErrors == false) OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), ClientAssemblyName + ".Services", false, Projects.ProjectOutputFramework.NET40Client, CompilerOutputFileType.Code));
				}
			}

			//Output the built service
			AddMessageStage(CompilerMessageStage.Output);
			if (HasErrors == false)
			{
				if (IsFramework30((ProjectName)) == true)
				{
					AddOutputLine("Outputting Files for .NET Framework 3.0...", CompilerMessageStage.Output);
					WPFThread.Invoke(new Action(() => Output30(ProjectName, OutputFiles)), System.Windows.Threading.DispatcherPriority.Send);
				}
				if (IsFramework35(ProjectName) == true)
				{
					AddOutputLine("Outputting Files for .NET Framework 3.5...", CompilerMessageStage.Output);
					WPFThread.Invoke(new Action(() => Output35(ProjectName, OutputFiles)), System.Windows.Threading.DispatcherPriority.Send);
				}
				if (IsFramework35Client(ProjectName) == true)
				{
					AddOutputLine("Outputting Files for .NET Framework 3.5 (Client Profile)...", CompilerMessageStage.Output);
					WPFThread.Invoke(new Action(() => Output35Client(ProjectName, OutputFiles)), System.Windows.Threading.DispatcherPriority.Send);
				}
				if (IsFramework40(ProjectName) == true)
				{
					AddOutputLine("Outputting Files for .NET Framework 4.0...", CompilerMessageStage.Output);
					WPFThread.Invoke(new Action(() => Output40(ProjectName, OutputFiles)), System.Windows.Threading.DispatcherPriority.Send);
				}
				if (IsFramework40Client(ProjectName) == true)
				{
					AddOutputLine("Outputting Files for .NET Framework 4.0 (Client Profile)...", CompilerMessageStage.Output);
					WPFThread.Invoke(new Action(() => Output40Client(ProjectName, OutputFiles)), System.Windows.Threading.DispatcherPriority.Send);
				}
			}

			//Cleanup files.
			try
			{
				List<string> Delete = new List<string>();
				if (IsFramework30((ProjectName)) == true)
				{
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), "*.config"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), "*.err"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), "*.out")); ;
				}
				if (IsFramework35(ProjectName) == true)
				{
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), "*.config"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), "*.err"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), "*.out"));
				}
				if (IsFramework35Client(ProjectName) == true)
				{
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), "*.config"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), "*.err"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), "*.out"));
				}
				if (IsFramework40(ProjectName) == true)
				{
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), "*.config"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), "*.err"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), "*.out"));
				}
				if (IsFramework40Client(ProjectName) == true)
				{
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), "*.config"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), "*.err"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), "*.out"));
				}
				foreach (string F in Delete)
					File.Delete(F);
			}
			catch { }

			//Notify the user that the build process is complete
			AddMessageStage(CompilerMessageStage.Complete);

			return OutputFiles;
		}

		private List<CompilerOutputFile> InternalOutputFiles(string ProjectName)
		{
			List<CompilerOutputFile> OutputFiles = new List<CompilerOutputFile>();

			WPFThread.Invoke(new Action(() =>
			{
				if (Project.IsDependencyProject == false)
				{
					ServerAssemblyName = Project.ServerAssemblyName;
					ClientAssemblyName = Project.ClientAssemblyName;
				}
				else
				{
					ServerAssemblyName = Helpers.RegExs.ReplaceSpaces.Replace(Project.Name, ".") + ".Server";
					ClientAssemblyName = Helpers.RegExs.ReplaceSpaces.Replace(Project.Name, ".") + ".Client";
				}

				SvcUtilPath = Globals.UserProfile.SvcUtilPath;
				IsDependencyProject = Project.IsDependencyProject;

				OutputFiles.Clear();

				Messages.Clear();
				Output.Blocks.Clear();
				VerifyCodeOutput.Inlines.Clear();
				ServerBuildOutput.Inlines.Clear();
				ProxyGenerateOutput.Inlines.Clear();
				ClientBuildOutput.Inlines.Clear();
				FileOutput.Inlines.Clear();
				HasErrors = false;

				Output.Blocks.Add(VerifyCodeOutputHeader);
				Output.Blocks.Add(VerifyCodeOutput);
				if (Project.VerifyCode() == false) HasErrors = true;
				if (HasErrors == true) return;

			}), System.Windows.Threading.DispatcherPriority.Send);

			//Generate Server Code
			if (HasErrors == false)
			{
				if (IsFramework30(ProjectName) == true)
				{
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET30, CompilerOutputFileType.Code));
				}
				if (IsFramework35(ProjectName) == true)
				{
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET35, CompilerOutputFileType.Code));
				}
				if (IsFramework35Client(ProjectName) == true)
				{
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET35Client, CompilerOutputFileType.Code));
				}
				if (IsFramework40(ProjectName) == true)
				{
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET40, CompilerOutputFileType.Code));
				}
				if (IsFramework40Client(ProjectName) == true)
				{
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET40Client, CompilerOutputFileType.Code));
				}
			}

			//Build Server Assembly
			AddMessageStage(CompilerMessageStage.Server);
			if (HasErrors == false)
			{
				if (IsFramework30(ProjectName) == true)
				{
					if (OutputServer30(ProjectName, OutputFiles) == false) HasErrors = true;
				}
				if (IsFramework35(ProjectName) == true)
				{
					if (OutputServer35(ProjectName, OutputFiles) == false) HasErrors = true;
				}
				if (IsFramework35Client(ProjectName) == true)
				{
					if (OutputServer35Client(ProjectName, OutputFiles) == false) HasErrors = true;
				}
				if (IsFramework40(ProjectName) == true)
				{
					if (OutputServer40(ProjectName, OutputFiles) == false) HasErrors = true;
				}
				if (IsFramework40Client(ProjectName) == true)
				{
					if (OutputServer40Client(ProjectName, OutputFiles) == false) HasErrors = true;
				}
			}

			AddMessageStage(CompilerMessageStage.Proxy);
			AddMessageStage(CompilerMessageStage.Client);

			//Generate Client Code
			if (HasErrors == false)
			{
				if (IsFramework30(ProjectName) == true)
				{
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET30, CompilerOutputFileType.Code));
				}
				if (IsFramework35(ProjectName) == true)
				{
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET35, CompilerOutputFileType.Code));
				}
				if (IsFramework35Client(ProjectName) == true)
				{
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET35Client, CompilerOutputFileType.Code));
				}
				if (IsFramework40(ProjectName) == true)
				{
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET40, CompilerOutputFileType.Code));
				}
				if (IsFramework40Client(ProjectName) == true)
				{
					OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET40Client, CompilerOutputFileType.Code));
				}
			}

			//Build Client Assembly
			if (HasErrors == false)
			{
				if (IsFramework30(ProjectName) == true)
				{
					if (OutputProxy30(ProjectName, OutputFiles, OutputFiles) == false) HasErrors = true;
					if (HasErrors == false) if (OutputClient30(ProjectName, OutputFiles) == false) HasErrors = true;
				}
				if (IsFramework35(ProjectName) == true)
				{
					if (OutputProxy35(ProjectName, OutputFiles, OutputFiles) == false) HasErrors = true;
					if (HasErrors == false) if (OutputClient35(ProjectName, OutputFiles) == false) HasErrors = true;
				}
				if (IsFramework35Client(ProjectName) == true)
				{
					if (OutputProxy35Client(ProjectName, OutputFiles, OutputFiles) == false) HasErrors = true;
					if (HasErrors == false) if (OutputClient35Client(ProjectName, OutputFiles) == false) HasErrors = true;
				}
				if (IsFramework40(ProjectName) == true)
				{
					if (OutputProxy40(ProjectName, OutputFiles, OutputFiles) == false) HasErrors = true;
					if (HasErrors == false) if (OutputClient40(ProjectName, OutputFiles) == false) HasErrors = true;
				}
				if (IsFramework40Client(ProjectName) == true)
				{
					if (OutputProxy40Client(ProjectName, OutputFiles, OutputFiles) == false) HasErrors = true;
					if (HasErrors == false) if (OutputClient40Client(ProjectName, OutputFiles) == false) HasErrors = true;
				}
			}

			//Generate Client Proxy
			if (HasErrors == false)
			{
				if (IsFramework30((ProjectName)) == true)
				{
					if (HasErrors == false) OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), ClientAssemblyName + ".Services", false, Projects.ProjectOutputFramework.NET30, CompilerOutputFileType.Code));
				}
				if (IsFramework35(ProjectName) == true)
				{
					if (HasErrors == false) OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), ClientAssemblyName + ".Services", false, Projects.ProjectOutputFramework.NET35, CompilerOutputFileType.Code));
				}
				if (IsFramework35Client(ProjectName) == true)
				{
					if (HasErrors == false) OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), ClientAssemblyName + ".Services", false, Projects.ProjectOutputFramework.NET35Client, CompilerOutputFileType.Code));
				}
				if (IsFramework40(ProjectName) == true)
				{
					if (HasErrors == false) OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), ClientAssemblyName + ".Services", false, Projects.ProjectOutputFramework.NET40, CompilerOutputFileType.Code));
				}
				if (IsFramework40Client(ProjectName) == true)
				{
					if (HasErrors == false) OutputFiles.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), ClientAssemblyName + ".Services", false, Projects.ProjectOutputFramework.NET40Client, CompilerOutputFileType.Code));
				}
			}

			//Output the built service
			AddMessageStage(CompilerMessageStage.Output);
			if (HasErrors == false)
			{
				if (IsFramework30((ProjectName)) == true)
				{
					AddOutputLine("Outputting Files for .NET Framework 3.0...", CompilerMessageStage.Output);
					WPFThread.Invoke(new Action(() => Output30(ProjectName, OutputFiles)), System.Windows.Threading.DispatcherPriority.Send);
				}
				if (IsFramework35(ProjectName) == true)
				{
					AddOutputLine("Outputting Files for .NET Framework 3.5...", CompilerMessageStage.Output);
					WPFThread.Invoke(new Action(() => Output35(ProjectName, OutputFiles)), System.Windows.Threading.DispatcherPriority.Send);
				}
				if (IsFramework35Client(ProjectName) == true)
				{
					AddOutputLine("Outputting Files for .NET Framework 3.5 (Client Profile)...", CompilerMessageStage.Output);
					WPFThread.Invoke(new Action(() => Output35Client(ProjectName, OutputFiles)), System.Windows.Threading.DispatcherPriority.Send);
				}
				if (IsFramework40(ProjectName) == true)
				{
					AddOutputLine("Outputting Files for .NET Framework 4.0...", CompilerMessageStage.Output);
					WPFThread.Invoke(new Action(() => Output40(ProjectName, OutputFiles)), System.Windows.Threading.DispatcherPriority.Send);
				}
				if (IsFramework40Client(ProjectName) == true)
				{
					AddOutputLine("Outputting Files for .NET Framework 4.0 (Client Profile)...", CompilerMessageStage.Output);
					WPFThread.Invoke(new Action(() => Output40Client(ProjectName, OutputFiles)), System.Windows.Threading.DispatcherPriority.Send);
				}
			}

			//Cleanup files.
			try
			{
				List<string> Delete = new List<string>();
				if (IsFramework30((ProjectName)) == true)
				{
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), "*.config"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), "*.err"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), "*.out")); ;
				}
				if (IsFramework35(ProjectName) == true)
				{
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), "*.config"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), "*.err"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), "*.out"));
				}
				if (IsFramework35Client(ProjectName) == true)
				{
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), "*.config"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), "*.err"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), "*.out"));
				}
				if (IsFramework40(ProjectName) == true)
				{
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), "*.config"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), "*.err"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), "*.out"));
				}
				if (IsFramework40Client(ProjectName) == true)
				{
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), "*.config"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), "*.err"));
					Delete.AddRange(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), "*.out"));
				}
				foreach (string F in Delete)
					File.Delete(F);
			}
			catch { }

			//Notify the user that the build process is complete
			AddMessageStage(CompilerMessageStage.Complete);

			return OutputFiles;
		}

		#region - .NET 3.0 Compilers -

		private bool CompileServer30(string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			List<Projects.Reference> RL = new List<Projects.Reference>(GetReferences(ProjectName).Where<Projects.Reference>(a => (a.FrameworkVersionNoBuild == "3.0.0.0" || a.FrameworkVersionNoBuild == "2.0.0.0") && a.IsClientProfile == false));
			if (RL.Count == 0)
			{
				AddMessage(new CompileMessage("GS0698", "No References specified for the '" + ProjectName + "' project.", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Server);
				return false;
			}

			string Code = (string)WPFThread.Invoke(new Func<string>(() => Project.GenerateServerCode30(true, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);

			CompilerParameters Parameters = new CompilerParameters();
			CompilerResults Results;

			Parameters.GenerateInMemory = false;
			Parameters.TreatWarningsAsErrors = false;
			Parameters.TempFiles = new TempFileCollection(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30));
			Parameters.TempFiles.KeepFiles = false;
			Parameters.WarningLevel = 4;
			foreach (Projects.Reference R in RL)
				Parameters.ReferencedAssemblies.Add(R.Path);

			string BaseOptions = " /optimize /define:TRACE";

			try
			{
				Parameters.ReferencedAssemblies.Clear();
				foreach (Projects.Reference R in RL)
					Parameters.ReferencedAssemblies.Add(R.Path);
				var DL = DependencyAssemblies.Where(a => a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET30 && a.IsServerFile == true);
				foreach (var D in DL)
					Parameters.ReferencedAssemblies.Add(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30) + D.FileName + ".dll");

				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET30, CompilerOutputFileType.Assembly);
				AddOutputLine("Building '" + COF.FileName + ".dll' ... ", CompilerMessageStage.Server, false);
				Parameters.CompilerOptions = BaseOptions + " /platform:anycpu ";
				Parameters.OutputAssembly = GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30) + COF.FileName + ".dll";

				Results = CSCompilerNET30.CompileAssemblyFromSource(Parameters, Code);
				if (Results.Errors.HasErrors == true || Results.Errors.HasWarnings == true)
				{
					if (Results.Errors.HasErrors == true) AddOutputLine("Failed!", CompilerMessageStage.Server);
					CompilerOutput(Results.Errors, CompilerMessageStage.Server);
					if (Results.Errors.HasErrors == true) return false;
				}
				AddOutputLine("Success!", CompilerMessageStage.Server);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Server);
				return false;
			}

			return true;
		}

		public bool GenerateProxy30(bool GenerateAssemblyCode, string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			if (File.Exists(SvcUtilPath) == false)
			{
				AddMessage(new CompileMessage("GS0798", "Unable to locate the SvcUtil executable.", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Proxy);
				return false;
			}
			List<string> AL = new List<string>(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), ServerAssemblyName + ".NET30.*.dll", SearchOption.TopDirectoryOnly));
			if (AL.Count == 0)
			{
				AddMessage(new CompileMessage("GS0799", "Unable to locate a valid output file'", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Proxy);
				return false;
			}
			string AsmFilePath = AL[0];
			List<string> RL;

			//Setup the basic process start info, this is reused.
			ProcessStartInfo PSI = new ProcessStartInfo(SvcUtilPath);
			PSI.WorkingDirectory = GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30);
			PSI.CreateNoWindow = true;
			PSI.RedirectStandardError = true;
			PSI.RedirectStandardOutput = true;
			PSI.UseShellExecute = false;
			PSI.WindowStyle = ProcessWindowStyle.Hidden;
			Process MP = new Process();
			MP.StartInfo = PSI;
			MP.ErrorDataReceived += new DataReceivedEventHandler(SvcUtilOutput);
			MP.OutputDataReceived += new DataReceivedEventHandler(SvcUtilOutput);
			Process CP = new Process();
			CP.StartInfo = PSI;
			CP.ErrorDataReceived += new DataReceivedEventHandler(SvcUtilOutput);
			CP.OutputDataReceived += new DataReceivedEventHandler(SvcUtilOutput);

			//Generate the Metadata
			StringBuilder MetadataArgs = new StringBuilder();
			MetadataArgs.Append("\"" + AsmFilePath + "\"");
			MetadataArgs.Append(" /nologo");
			MetadataArgs.Append(" /t:metadata");
			MetadataArgs.Append(" /d:\"" + GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30).Remove(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30).Length - 1, 1) + "\"");
			RL = BuildProxyReferenceList(Projects.ProjectOutputFramework.NET30, true, ProjectName, DependencyAssemblies);
			foreach (string R in RL)
				MetadataArgs.Append(" /r:\"" + R + "\"");
			if (GetServiceGenerateDataContracts(ProjectName) == true)
			{
				MetadataArgs.Append(" /dconly");
			}
			else
			{
				foreach (string ET in GetServiceExcludedTypes(ProjectName))
					MetadataArgs.Append(" /et:\"" + ET + "\"");
			}
			PSI.Arguments = MetadataArgs.ToString();

			//Run the process
			MP.Start();
			MP.BeginErrorReadLine();
			MP.WaitForExit();

			ParseSvcUtilOutput(ProjectName, Projects.ProjectOutputFramework.NET30, MP.StandardOutput.ReadToEnd());
			AddOutputLine(MP.StandardOutput.ReadToEnd(), CompilerMessageStage.Proxy);

			//Generate the Client Code
			StringBuilder CodeArgs = new StringBuilder((string)WPFThread.Invoke(new Func<string>(() => Project.GenerateClientProxyArgs(GenerateAssemblyCode, IsClientClassesInternal(ProjectName))), System.Windows.Threading.DispatcherPriority.Send));
			CodeArgs.Append(" /l:cs");
			CodeArgs.Append(" /tcv:Version30");
			RL = BuildProxyReferenceList(Projects.ProjectOutputFramework.NET30, false, ProjectName, DependencyAssemblies);
			foreach (string R in RL)
				CodeArgs.Append(" /r:\"" + R + "\"");
			CodeArgs.Append(" /d:\"" + GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30).Remove(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30).Length - 1, 1) + "\"");
			if (GetServiceGenerateDataContracts(ProjectName) == true)
			{
				CodeArgs.Append(" /dconly *.xsd");
			}
			else
			{
				foreach (string ET in GetServiceExcludedTypes(ProjectName))
					MetadataArgs.Append(" /et:\"" + ET + "\"");
				CodeArgs.Append(" *.wsdl *.xsd");
			}
			CodeArgs.Append(" /o:\"" + ClientAssemblyName + ".Services.NET30.cs\"");

			PSI.Arguments = CodeArgs.ToString();

			//DeleteUnneededXSDFiles(ProjectName, Projects.ProjectOutputFramework.NET30);

			OutputFiles.AddRange(GetXSDWSDLCompilerOutput(ProjectName, Projects.ProjectOutputFramework.NET30));

			//Run the process
			CP.Start();
			CP.BeginErrorReadLine();
			CP.BeginOutputReadLine();
			CP.WaitForExit();

			return true;
		}

		private bool CompileClient30(string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			List<Projects.Reference> RL = new List<Projects.Reference>(GetReferences(ProjectName).Where<Projects.Reference>(a => (a.FrameworkVersionNoBuild == "3.0.0.0" || a.FrameworkVersionNoBuild == "2.0.0.0") && a.IsClientProfile == false));
			if (RL.Count == 0)
			{
				AddMessage(new CompileMessage("GS0898", "No References specified for the '" + ProjectName + "' project.", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Client);
				return false;
			}
			if (File.Exists(Path.Combine(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), ClientAssemblyName + ".Services.NET30.cs")) == false) return false;

			string Code = (string)WPFThread.Invoke(new Func<string>(() => Project.GenerateClientCode30(true, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);
			string ServiceCode = File.ReadAllText(Path.Combine(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), ClientAssemblyName + ".Services.NET30.cs"));

			CompilerParameters Parameters = new CompilerParameters();
			CompilerResults Results;

			Parameters.GenerateInMemory = false;
			Parameters.TreatWarningsAsErrors = false;
			Parameters.TempFiles = new TempFileCollection(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30));
			Parameters.TempFiles.KeepFiles = false;
			Parameters.WarningLevel = 4;
			foreach (Projects.Reference R in RL)
				Parameters.ReferencedAssemblies.Add(R.Path);

			string BaseOptions = " /optimize /define:TRACE";

			try
			{
				Parameters.ReferencedAssemblies.Clear();
				foreach (Projects.Reference R in RL)
					Parameters.ReferencedAssemblies.Add(R.Path);
				var DL = DependencyAssemblies.Where(a => a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET30 && a.IsClientFile == true);
				foreach (var D in DL)
					Parameters.ReferencedAssemblies.Add(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30) + D.FileName + ".dll");

				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET30, CompilerOutputFileType.Assembly);
				AddOutputLine("Building '" + COF.FileName + ".dll' ... ", CompilerMessageStage.Client, false);
				Parameters.CompilerOptions = BaseOptions + " /platform:anycpu ";
				Parameters.OutputAssembly = GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30) + COF.FileName + ".dll";

				Results = CSCompilerNET30.CompileAssemblyFromSource(Parameters, Code, ServiceCode);
				if (Results.Errors.HasErrors == true || Results.Errors.HasWarnings == true)
				{
					if (Results.Errors.HasErrors == true) AddOutputLine("Failed!", CompilerMessageStage.Client);
					CompilerOutput(Results.Errors, CompilerMessageStage.Client);
					if (Results.Errors.HasErrors == true) return false;
				}
				AddOutputLine("Success!", CompilerMessageStage.Client);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Client);
				return false;
			}

			return true;
		}

		#endregion

		#region - .NET 3.5 Compilers -

		private bool CompileServer35(string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			List<Projects.Reference> RL = new List<Projects.Reference>(GetReferences(ProjectName).Where<Projects.Reference>(a => (a.FrameworkVersionNoBuild == "3.5.0.0" || a.FrameworkVersionNoBuild == "3.0.0.0" || a.FrameworkVersionNoBuild == "2.0.0.0") && a.IsClientProfile == false));
			if (RL.Count == 0)
			{
				AddMessage(new CompileMessage("GS0697", "No References specified for the '" + ProjectName + "' project.", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Server);
				return false;
			}

			string Code = (string)WPFThread.Invoke(new Func<string>(() => Project.GenerateServerCode35(true, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);

			CompilerParameters Parameters = new CompilerParameters();
			CompilerResults Results;

			Parameters.GenerateInMemory = false;
			Parameters.TreatWarningsAsErrors = false;
			Parameters.TempFiles = new TempFileCollection(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35));
			Parameters.TempFiles.KeepFiles = false;
			Parameters.WarningLevel = 4;
			foreach (Projects.Reference R in RL)
				Parameters.ReferencedAssemblies.Add(R.Path);

			string BaseOptions = " /optimize /define:TRACE";

			try
			{
				Parameters.ReferencedAssemblies.Clear();
				foreach (Projects.Reference R in RL)
					Parameters.ReferencedAssemblies.Add(R.Path);
				var DL = DependencyAssemblies.Where(a => a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET35 && a.IsServerFile == true);
				foreach (var D in DL)
					Parameters.ReferencedAssemblies.Add(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35) + D.FileName + ".dll");

				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET35, CompilerOutputFileType.Assembly);
				AddOutputLine("Building '" + COF.FileName + ".dll' ... ", CompilerMessageStage.Server, false);
				Parameters.CompilerOptions = BaseOptions + " /platform:anycpu ";
				Parameters.OutputAssembly = GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35) + COF.FileName + ".dll";

				Results = CSCompilerNET35.CompileAssemblyFromSource(Parameters, Code);
				if (Results.Errors.HasErrors == true || Results.Errors.HasWarnings == true)
				{
					if (Results.Errors.HasErrors == true) AddOutputLine("Failed!", CompilerMessageStage.Server);
					CompilerOutput(Results.Errors, CompilerMessageStage.Server);
					if (Results.Errors.HasErrors == true) return false;
				}
				AddOutputLine("Success!", CompilerMessageStage.Server);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Server);
				return false;
			}

			return true;
		}

		public bool GenerateProxy35(bool GenerateAssemblyCode, string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			if (File.Exists(SvcUtilPath) == false)
			{
				AddMessage(new CompileMessage("GS0798", "Unable to locate the SvcUtil executable.", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Proxy);
				return false;
			}
			List<string> AL = new List<string>(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), ServerAssemblyName + ".NET35.*.dll", SearchOption.TopDirectoryOnly));
			if (AL.Count == 0)
			{
				AddMessage(new CompileMessage("GS0799", "Unable to locate a valid output file'", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Proxy);
				return false;
			}
			string AsmFilePath = AL[0];
			List<string> RL;

			//Setup the basic process start info, this is reused.
			ProcessStartInfo PSI = new ProcessStartInfo(SvcUtilPath);
			PSI.WorkingDirectory = GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35);
			PSI.CreateNoWindow = true;
			PSI.RedirectStandardError = true;
			PSI.RedirectStandardOutput = true;
			PSI.UseShellExecute = false;
			PSI.WindowStyle = ProcessWindowStyle.Hidden;
			Process MP = new Process();
			MP.StartInfo = PSI;
			MP.ErrorDataReceived += new DataReceivedEventHandler(SvcUtilOutput);
			MP.OutputDataReceived += new DataReceivedEventHandler(SvcUtilOutput);
			Process CP = new Process();
			CP.StartInfo = PSI;
			CP.ErrorDataReceived += new DataReceivedEventHandler(SvcUtilOutput);
			CP.OutputDataReceived += new DataReceivedEventHandler(SvcUtilOutput);

			//Generate the Metadata
			StringBuilder MetadataArgs = new StringBuilder();
			MetadataArgs.Append("\"" + AsmFilePath + "\"");
			MetadataArgs.Append(" /nologo");
			MetadataArgs.Append(" /t:metadata");
			MetadataArgs.Append(" /d:\"" + GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35).Remove(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35).Length - 1, 1) + "\"");
			RL = BuildProxyReferenceList(Projects.ProjectOutputFramework.NET35, true, ProjectName, DependencyAssemblies);
			foreach (string R in RL)
				MetadataArgs.Append(" /r:\"" + R + "\"");
			if (GetServiceGenerateDataContracts(ProjectName) == true)
			{
				MetadataArgs.Append(" /dconly");
			}
			else
			{
				foreach (string ET in GetServiceExcludedTypes(ProjectName))
					MetadataArgs.Append(" /et:\"" + ET + "\"");
			}
			PSI.Arguments = MetadataArgs.ToString();

			//Run the process
			MP.Start();
			MP.BeginErrorReadLine();
			MP.WaitForExit();

			ParseSvcUtilOutput(ProjectName, Projects.ProjectOutputFramework.NET35, MP.StandardOutput.ReadToEnd());
			AddOutputLine(MP.StandardOutput.ReadToEnd(), CompilerMessageStage.Proxy);

			//Generate the Client Code
			StringBuilder CodeArgs = new StringBuilder((string)WPFThread.Invoke(new Func<string>(() => Project.GenerateClientProxyArgs(GenerateAssemblyCode, IsClientClassesInternal(ProjectName))), System.Windows.Threading.DispatcherPriority.Send));
			CodeArgs.Append(" /l:cs");
			CodeArgs.Append(" /tcv:Version35");
			RL = BuildProxyReferenceList(Projects.ProjectOutputFramework.NET35, false, ProjectName, DependencyAssemblies);
			foreach (string R in RL)
				CodeArgs.Append(" /r:\"" + R + "\"");
			CodeArgs.Append(" /d:\"" + GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35).Remove(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35).Length - 1, 1) + "\"");
			if (GetServiceGenerateDataContracts(ProjectName) == true)
			{
				CodeArgs.Append(" /dconly *.xsd");
			}
			else
			{
				foreach (string ET in GetServiceExcludedTypes(ProjectName))
					MetadataArgs.Append(" /et:\"" + ET + "\"");
				CodeArgs.Append(" *.wsdl *.xsd");
			}
			CodeArgs.Append(" /o:\"" + ClientAssemblyName + ".Services.NET35.cs\"");

			PSI.Arguments = CodeArgs.ToString();

			//DeleteUnneededXSDFiles(ProjectName, Projects.ProjectOutputFramework.NET35);

			OutputFiles.AddRange(GetXSDWSDLCompilerOutput(ProjectName, Projects.ProjectOutputFramework.NET35));

			//Run the process
			CP.Start();
			CP.BeginErrorReadLine();
			CP.BeginOutputReadLine();
			CP.WaitForExit();

			return true;
		}

		private bool CompileClient35(string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			List<Projects.Reference> RL = new List<Projects.Reference>(GetReferences(ProjectName).Where<Projects.Reference>(a => (a.FrameworkVersionNoBuild == "3.5.0.0" || a.FrameworkVersionNoBuild == "3.0.0.0" || a.FrameworkVersionNoBuild == "2.0.0.0") && a.IsClientProfile == false));
			if (RL.Count == 0)
			{
				AddMessage(new CompileMessage("GS0697", "No References specified for the '" + ProjectName + "' project.", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Client);
				return false;
			}
			if (File.Exists(Path.Combine(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), ClientAssemblyName + ".Services.NET35.cs")) == false) return false;

			string Code = (string)WPFThread.Invoke(new Func<string>(() => Project.GenerateClientCode35(true, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);
			string ServiceCode = File.ReadAllText(Path.Combine(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), ClientAssemblyName + ".Services.NET35.cs"));

			CompilerParameters Parameters = new CompilerParameters();
			CompilerResults Results;

			Parameters.GenerateInMemory = false;
			Parameters.TreatWarningsAsErrors = false;
			Parameters.TempFiles = new TempFileCollection(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35));
			Parameters.TempFiles.KeepFiles = false;
			Parameters.WarningLevel = 4;
			foreach (Projects.Reference R in RL)
				Parameters.ReferencedAssemblies.Add(R.Path);

			string BaseOptions = " /optimize /define:TRACE";

			try
			{
				Parameters.ReferencedAssemblies.Clear();
				foreach (Projects.Reference R in RL)
					Parameters.ReferencedAssemblies.Add(R.Path);
				var DL = DependencyAssemblies.Where(a => a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET35 && a.IsClientFile == true);
				foreach (var D in DL)
					Parameters.ReferencedAssemblies.Add(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35) + D.FileName + ".dll");

				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET35, CompilerOutputFileType.Assembly);
				AddOutputLine("Building '" + COF.FileName + ".dll' ... ", CompilerMessageStage.Client, false);
				Parameters.CompilerOptions = BaseOptions + " /platform:anycpu ";
				Parameters.OutputAssembly = GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35) + COF.FileName + ".dll";

				Results = CSCompilerNET35.CompileAssemblyFromSource(Parameters, Code, ServiceCode);
				if (Results.Errors.HasErrors == true || Results.Errors.HasWarnings == true)
				{
					if (Results.Errors.HasErrors == true) AddOutputLine("Failed!", CompilerMessageStage.Client);
					CompilerOutput(Results.Errors, CompilerMessageStage.Client);
					if (Results.Errors.HasErrors == true) return false;
				}
				AddOutputLine("Success!", CompilerMessageStage.Client);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Client);
				return false;
			}

			return true;
		}

		#endregion

		#region - .NET 3.5 Client Compilers -

		private bool CompileServer35Client(string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			List<Projects.Reference> RL = new List<Projects.Reference>(GetReferences(ProjectName).Where<Projects.Reference>(a => (a.FrameworkVersionNoBuild == "3.5.0.0" || a.FrameworkVersionNoBuild == "3.0.0.0" || a.FrameworkVersionNoBuild == "2.0.0.0") && a.IsClientProfile == true));
			if (RL.Count == 0)
			{
				AddMessage(new CompileMessage("GS0697", "No References specified for the '" + ProjectName + "' project.", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Server);
				return false;
			}

			string Code = (string)WPFThread.Invoke(new Func<string>(() => Project.GenerateServerCode35Client(true, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);

			CompilerParameters Parameters = new CompilerParameters();
			CompilerResults Results;

			Parameters.GenerateInMemory = false;
			Parameters.TreatWarningsAsErrors = false;
			Parameters.TempFiles = new TempFileCollection(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client));
			Parameters.TempFiles.KeepFiles = false;
			Parameters.WarningLevel = 4;
			foreach (Projects.Reference R in RL)
				Parameters.ReferencedAssemblies.Add(R.Path);

			string BaseOptions = " /optimize /define:TRACE";
	
			try
			{
				Parameters.ReferencedAssemblies.Clear();
				foreach (Projects.Reference R in RL)
					Parameters.ReferencedAssemblies.Add(R.Path);
				var DL = DependencyAssemblies.Where(a => a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET35Client && a.IsServerFile == true);
				foreach (var D in DL)
					Parameters.ReferencedAssemblies.Add(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client) + D.FileName + ".dll");

				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET35Client, CompilerOutputFileType.Assembly);
				AddOutputLine("Building '" + COF.FileName + ".dll' ... ", CompilerMessageStage.Server, false);
				Parameters.CompilerOptions = BaseOptions + " /platform:anycpu ";
				Parameters.OutputAssembly = GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client) + COF.FileName + ".dll";

				Results = CSCompilerNET35.CompileAssemblyFromSource(Parameters, Code);
				if (Results.Errors.HasErrors == true || Results.Errors.HasWarnings == true)
				{
					if (Results.Errors.HasErrors == true) AddOutputLine("Failed!", CompilerMessageStage.Server);
					CompilerOutput(Results.Errors, CompilerMessageStage.Server);
					if (Results.Errors.HasErrors == true) return false;
				}
				AddOutputLine("Success!", CompilerMessageStage.Server);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Server);
				return false;
			}

			return true;
		}

		public bool GenerateProxy35Client(bool GenerateAssemblyCode, string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			if (File.Exists(SvcUtilPath) == false)
			{
				AddMessage(new CompileMessage("GS0798", "Unable to locate the SvcUtil executable.", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Proxy);
				return false;
			}
			List<string> AL = new List<string>(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), ServerAssemblyName + ".NET35Client.*.dll", SearchOption.TopDirectoryOnly));
			if (AL.Count == 0)
			{
				AddMessage(new CompileMessage("GS0799", "Unable to locate a valid output file'", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Proxy);
				return false;
			}
			string AsmFilePath = AL[0];
			List<string> RL;

			//Setup the basic process start info, this is reused.
			ProcessStartInfo PSI = new ProcessStartInfo(SvcUtilPath);
			PSI.WorkingDirectory = GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client);
			PSI.CreateNoWindow = true;
			PSI.RedirectStandardError = true;
			PSI.RedirectStandardOutput = true;
			PSI.UseShellExecute = false;
			PSI.WindowStyle = ProcessWindowStyle.Hidden;
			Process MP = new Process();
			MP.StartInfo = PSI;
			MP.ErrorDataReceived += new DataReceivedEventHandler(SvcUtilOutput);
			MP.OutputDataReceived += new DataReceivedEventHandler(SvcUtilOutput);
			Process CP = new Process();
			CP.StartInfo = PSI;
			CP.ErrorDataReceived += new DataReceivedEventHandler(SvcUtilOutput);
			CP.OutputDataReceived += new DataReceivedEventHandler(SvcUtilOutput);

			//Generate the Metadata
			StringBuilder MetadataArgs = new StringBuilder();
			MetadataArgs.Append("\"" + AsmFilePath + "\"");
			MetadataArgs.Append(" /nologo");
			MetadataArgs.Append(" /t:metadata");
			MetadataArgs.Append(" /d:\"" + GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client).Remove(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client).Length - 1, 1) + "\"");
			RL = BuildProxyReferenceList(Projects.ProjectOutputFramework.NET35Client, true, ProjectName, DependencyAssemblies);
			foreach (string R in RL)
				MetadataArgs.Append(" /r:\"" + R + "\"");
			if (GetServiceGenerateDataContracts(ProjectName) == true)
			{
				MetadataArgs.Append(" /dconly");
			}
			else
			{
				foreach (string ET in GetServiceExcludedTypes(ProjectName))
					MetadataArgs.Append(" /et:\"" + ET + "\"");
			}
			PSI.Arguments = MetadataArgs.ToString();

			//Run the process
			MP.Start();
			MP.BeginErrorReadLine();
			MP.WaitForExit();

			ParseSvcUtilOutput(ProjectName, Projects.ProjectOutputFramework.NET35Client, MP.StandardOutput.ReadToEnd());
			AddOutputLine(MP.StandardOutput.ReadToEnd(), CompilerMessageStage.Proxy);

			//Generate the Client Code
			StringBuilder CodeArgs = new StringBuilder((string)WPFThread.Invoke(new Func<string>(() => Project.GenerateClientProxyArgs(GenerateAssemblyCode, IsClientClassesInternal(ProjectName))), System.Windows.Threading.DispatcherPriority.Send));
			CodeArgs.Append(" /l:cs");
			CodeArgs.Append(" /tcv:Version35");
			RL = BuildProxyReferenceList(Projects.ProjectOutputFramework.NET35Client, false, ProjectName, DependencyAssemblies);
			foreach (string R in RL)
				CodeArgs.Append(" /r:\"" + R + "\"");
			CodeArgs.Append(" /d:\"" + GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client).Remove(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client).Length - 1, 1) + "\"");
			if (GetServiceGenerateDataContracts(ProjectName) == true)
			{
				CodeArgs.Append(" /dconly *.xsd");
			}
			else
			{
				foreach (string ET in GetServiceExcludedTypes(ProjectName))
					MetadataArgs.Append(" /et:\"" + ET + "\"");
				CodeArgs.Append(" *.wsdl *.xsd");
			}
			CodeArgs.Append(" /o:\"" + ClientAssemblyName + ".Services.NET35Client.cs\"");

			PSI.Arguments = CodeArgs.ToString();

			//DeleteUnneededXSDFiles(ProjectName, Projects.ProjectOutputFramework.NET35Client);

			OutputFiles.AddRange(GetXSDWSDLCompilerOutput(ProjectName, Projects.ProjectOutputFramework.NET35Client));

			//Run the process
			CP.Start();
			CP.BeginErrorReadLine();
			CP.BeginOutputReadLine();
			CP.WaitForExit();

			return true;
		}

		private bool CompileClient35Client(string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			List<Projects.Reference> RL = new List<Projects.Reference>(GetReferences(ProjectName).Where<Projects.Reference>(a => (a.FrameworkVersionNoBuild == "3.5.0.0" || a.FrameworkVersionNoBuild == "3.0.0.0" || a.FrameworkVersionNoBuild == "2.0.0.0") && a.IsClientProfile == true));
			if (RL.Count == 0)
			{
				AddMessage(new CompileMessage("GS0697", "No References specified for the '" + ProjectName + "' project.", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Client);
				return false;
			}
			if (File.Exists(Path.Combine(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), ClientAssemblyName + ".Services.NET35Client.cs")) == false) return false;

			string Code = (string)WPFThread.Invoke(new Func<string>(() => Project.GenerateClientCode35Client(true, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);
			string ServiceCode = File.ReadAllText(Path.Combine(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), ClientAssemblyName + ".Services.NET35Client.cs"));

			CompilerParameters Parameters = new CompilerParameters();
			CompilerResults Results;

			Parameters.GenerateInMemory = false;
			Parameters.TreatWarningsAsErrors = false;
			Parameters.TempFiles = new TempFileCollection(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client));
			Parameters.TempFiles.KeepFiles = false;
			Parameters.WarningLevel = 4;
			foreach (Projects.Reference R in RL)
				Parameters.ReferencedAssemblies.Add(R.Path);

			string BaseOptions = " /optimize /define:TRACE";

			try
			{
				Parameters.ReferencedAssemblies.Clear();
				foreach (Projects.Reference R in RL)
					Parameters.ReferencedAssemblies.Add(R.Path);
				var DL = DependencyAssemblies.Where(a => a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET35Client && a.IsClientFile == true);
				foreach (var D in DL)
					Parameters.ReferencedAssemblies.Add(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client) + D.FileName + ".dll");

				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET35Client, CompilerOutputFileType.Assembly);
				AddOutputLine("Building '" + COF.FileName + ".dll' ... ", CompilerMessageStage.Client, false);
				Parameters.CompilerOptions = BaseOptions + " /platform:anycpu ";
				Parameters.OutputAssembly = GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client) + COF.FileName + ".dll";

				Results = CSCompilerNET35.CompileAssemblyFromSource(Parameters, Code, ServiceCode);
				if (Results.Errors.HasErrors == true || Results.Errors.HasWarnings == true)
				{
					if (Results.Errors.HasErrors == true) AddOutputLine("Failed!", CompilerMessageStage.Client);
					CompilerOutput(Results.Errors, CompilerMessageStage.Client);
					if (Results.Errors.HasErrors == true) return false;
				}
				AddOutputLine("Success!", CompilerMessageStage.Client);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Client);
				return false;
			}

			return true;
		}

		#endregion

		#region - .NET 4.0 Compilers -

		private bool CompileServer40(string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			List<Projects.Reference> RL = new List<Projects.Reference>(GetReferences(ProjectName).Where<Projects.Reference>(a => a.FrameworkVersionNoBuild == "4.0.0.0" && a.IsClientProfile == false));
			if (RL.Count == 0)
			{
				AddMessage(new CompileMessage("GS0697", "No References specified for the '" + ProjectName + "' project.", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Server);
				return false;
			}

			string Code = (string)WPFThread.Invoke(new Func<string>(() => Project.GenerateServerCode40(true, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);

			CompilerParameters Parameters = new CompilerParameters();
			CompilerResults Results;

			Parameters.GenerateInMemory = false;
			Parameters.TreatWarningsAsErrors = false;
			Parameters.TempFiles = new TempFileCollection(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40));
			Parameters.TempFiles.KeepFiles = false;
			Parameters.WarningLevel = 4;
			foreach (Projects.Reference R in RL)
				Parameters.ReferencedAssemblies.Add(R.Path);

			string BaseOptions = " /optimize /define:TRACE";

			try
			{
				Parameters.ReferencedAssemblies.Clear();
				foreach (Projects.Reference R in RL)
					Parameters.ReferencedAssemblies.Add(R.Path);
				var DL = DependencyAssemblies.Where(a => a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET40 && a.IsServerFile == true);
				foreach (var D in DL)
					Parameters.ReferencedAssemblies.Add(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40) + D.FileName + ".dll");

				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET40, CompilerOutputFileType.Assembly);
				AddOutputLine("Building '" + COF.FileName + ".dll' ... ", CompilerMessageStage.Server, false);
				Parameters.CompilerOptions = BaseOptions + " /platform:anycpu ";
				Parameters.OutputAssembly = GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40) + COF.FileName + ".dll";

				Results = CSCompilerNET40.CompileAssemblyFromSource(Parameters, Code);
				if (Results.Errors.HasErrors == true || Results.Errors.HasWarnings == true)
				{
					if (Results.Errors.HasErrors == true) AddOutputLine("Failed!", CompilerMessageStage.Server);
					CompilerOutput(Results.Errors, CompilerMessageStage.Server);
					if (Results.Errors.HasErrors == true) return false;
				}
				AddOutputLine("Success!", CompilerMessageStage.Server);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Server);
				return false;
			}

			return true;
		}

		public bool GenerateProxy40(bool GenerateAssemblyCode, string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			if (File.Exists(SvcUtilPath) == false)
			{
				AddMessage(new CompileMessage("GS0798", "Unable to locate the SvcUtil executable.", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Proxy);
				return false;
			}
			List<string> AL = new List<string>(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), ServerAssemblyName + ".NET40.*.dll", SearchOption.TopDirectoryOnly));
			if (AL.Count == 0)
			{
				AddMessage(new CompileMessage("GS0799", "Unable to locate a valid output file'", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Proxy);
				return false;
			}
			string AsmFilePath = AL[0];
			List<string> RL;

			//Setup the basic process start info, this is reused.
			ProcessStartInfo PSI = new ProcessStartInfo(SvcUtilPath);
			PSI.WorkingDirectory = GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40);
			PSI.CreateNoWindow = true;
			PSI.RedirectStandardError = true;
			PSI.RedirectStandardOutput = true;
			PSI.UseShellExecute = false;
			PSI.WindowStyle = ProcessWindowStyle.Hidden;
			Process MP = new Process();
			MP.StartInfo = PSI;
			MP.ErrorDataReceived += new DataReceivedEventHandler(SvcUtilOutput);
			MP.OutputDataReceived += new DataReceivedEventHandler(SvcUtilOutput);
			Process CP = new Process();
			CP.StartInfo = PSI;
			CP.ErrorDataReceived += new DataReceivedEventHandler(SvcUtilOutput);
			CP.OutputDataReceived += new DataReceivedEventHandler(SvcUtilOutput);

			//Generate the Metadata
			StringBuilder MetadataArgs = new StringBuilder();
			MetadataArgs.Append(" /nologo");
			MetadataArgs.Append(" /t:metadata");
			MetadataArgs.Append(" /d:\"" + GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40).Remove(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40).Length - 1, 1) + "\"");
			RL = BuildProxyReferenceList(Projects.ProjectOutputFramework.NET40, true, ProjectName, DependencyAssemblies);
			foreach (string R in RL)
				MetadataArgs.Append(" /r:\"" + R + "\"");
			if (GetServiceGenerateDataContracts(ProjectName) == true)
			{
				MetadataArgs.Append(" /dconly");
			}
			else
			{
				foreach (string ET in GetServiceExcludedTypes(ProjectName))
					MetadataArgs.Append(" /et:\"" + ET + "\"");
			}
			MetadataArgs.Append(" \"" + AsmFilePath + "\"");
			PSI.Arguments = MetadataArgs.ToString();

			//Run the process
			MP.Start();
			MP.BeginErrorReadLine();
			MP.WaitForExit();

			ParseSvcUtilOutput(ProjectName, Projects.ProjectOutputFramework.NET40, MP.StandardOutput.ReadToEnd());
			AddOutputLine(MP.StandardOutput.ReadToEnd(), CompilerMessageStage.Proxy);

			//Generate the Client Code
			StringBuilder CodeArgs = new StringBuilder((string)WPFThread.Invoke(new Func<string>(() => Project.GenerateClientProxyArgs(GenerateAssemblyCode, IsClientClassesInternal(ProjectName))), System.Windows.Threading.DispatcherPriority.Send));
			CodeArgs.Append(" /l:cs");
			CodeArgs.Append(" /tcv:Version35");
			RL = BuildProxyReferenceList(Projects.ProjectOutputFramework.NET40, false, ProjectName, DependencyAssemblies);
			foreach (string R in RL)
				CodeArgs.Append(" /r:\"" + R + "\"");
			CodeArgs.Append(" /d:\"" + GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40).Remove(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40).Length - 1, 1) + "\"");
			if (GetServiceGenerateDataContracts(ProjectName) == true)
			{
				CodeArgs.Append(" /dconly *.xsd");
			}
			else
			{
				foreach (string ET in GetServiceExcludedTypes(ProjectName))
					MetadataArgs.Append(" /et:\"" + ET + "\"");
				CodeArgs.Append(" *.wsdl *.xsd");
			}
			CodeArgs.Append(" /o:\"" + ClientAssemblyName + ".Services.NET40.cs\"");

			PSI.Arguments = CodeArgs.ToString();

			//DeleteUnneededXSDFiles(ProjectName, Projects.ProjectOutputFramework.NET40);

			OutputFiles.AddRange(GetXSDWSDLCompilerOutput(ProjectName, Projects.ProjectOutputFramework.NET40));

			//Run the process
			CP.Start();
			CP.BeginErrorReadLine();
			CP.BeginOutputReadLine();
			CP.WaitForExit();

			return true;
		}

		private bool CompileClient40(string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			List<Projects.Reference> RL = new List<Projects.Reference>(GetReferences(ProjectName).Where<Projects.Reference>(a => a.FrameworkVersionNoBuild == "4.0.0.0" && a.IsClientProfile == false));
			if (RL.Count == 0)
			{
				AddMessage(new CompileMessage("GS0697", "No References specified for the '" + ProjectName + "' project.", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Client);
				return false;
			}
			if (File.Exists(Path.Combine(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), ClientAssemblyName + ".Services.NET40.cs")) == false) return false;

			string Code = (string)WPFThread.Invoke(new Func<string>(() => Project.GenerateClientCode40(true, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);
			string ServiceCode = File.ReadAllText(Path.Combine(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), ClientAssemblyName + ".Services.NET40.cs"));

			CompilerParameters Parameters = new CompilerParameters();
			CompilerResults Results;

			Parameters.GenerateInMemory = false;
			Parameters.TreatWarningsAsErrors = false;
			Parameters.TempFiles = new TempFileCollection(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40));
			Parameters.TempFiles.KeepFiles = false;
			Parameters.WarningLevel = 4;
			foreach (Projects.Reference R in RL)
				Parameters.ReferencedAssemblies.Add(R.Path);

			string BaseOptions = " /optimize /define:TRACE";

			try
			{
				Parameters.ReferencedAssemblies.Clear();
				foreach (Projects.Reference R in RL)
					Parameters.ReferencedAssemblies.Add(R.Path);
				var DL = DependencyAssemblies.Where(a => a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET40 && a.IsClientFile == true);
				foreach (var D in DL)
					Parameters.ReferencedAssemblies.Add(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40) + D.FileName + ".dll");

				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET40, CompilerOutputFileType.Assembly);
				AddOutputLine("Building '" + COF.FileName + ".dll' ... ", CompilerMessageStage.Client, false);
				Parameters.CompilerOptions = BaseOptions + " /platform:anycpu ";
				Parameters.OutputAssembly = GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40) + COF.FileName + ".dll";

				Results = CSCompilerNET40.CompileAssemblyFromSource(Parameters, Code, ServiceCode);
				if (Results.Errors.HasErrors == true || Results.Errors.HasWarnings == true)
				{
					if (Results.Errors.HasErrors == true) AddOutputLine("Failed!", CompilerMessageStage.Client);
					CompilerOutput(Results.Errors, CompilerMessageStage.Client);
					if (Results.Errors.HasErrors == true) return false;
				}
				AddOutputLine("Success!", CompilerMessageStage.Client);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Client);
				return false;
			}

			return true;
		}

		#endregion

		#region - .NET 4.0 Client Compilers -

		private bool CompileServer40Client(string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			List<Projects.Reference> RL = new List<Projects.Reference>(GetReferences(ProjectName).Where<Projects.Reference>(a => a.FrameworkVersionNoBuild == "4.0.0.0" && a.IsClientProfile == true));
			if (RL.Count == 0)
			{
				AddMessage(new CompileMessage("GS0697", "No References specified for the '" + ProjectName + "' project.", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Server);
				return false;
			}

			string Code = (string)WPFThread.Invoke(new Func<string>(() => Project.GenerateServerCode40Client(true, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);

			CompilerParameters Parameters = new CompilerParameters();
			CompilerResults Results;

			Parameters.GenerateInMemory = false;
			Parameters.TreatWarningsAsErrors = false;
			Parameters.TempFiles = new TempFileCollection(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client));
			Parameters.TempFiles.KeepFiles = false;
			Parameters.WarningLevel = 4;
			foreach (Projects.Reference R in RL)
				Parameters.ReferencedAssemblies.Add(R.Path);


			string BaseOptions = " /optimize /define:TRACE";

			try
			{
				Parameters.ReferencedAssemblies.Clear();
				foreach (Projects.Reference R in RL)
					Parameters.ReferencedAssemblies.Add(R.Path);
				var DL = DependencyAssemblies.Where(a => a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET40Client && a.IsServerFile == true);
				foreach (var D in DL)
					Parameters.ReferencedAssemblies.Add(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client) + D.FileName + ".dll");

				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET40Client, CompilerOutputFileType.Assembly);
				AddOutputLine("Building '" + COF.FileName + ".dll' ... ", CompilerMessageStage.Server, false);
				Parameters.CompilerOptions = BaseOptions + " /platform:anycpu ";
				Parameters.OutputAssembly = GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client) + COF.FileName + ".dll";

				Results = CSCompilerNET40.CompileAssemblyFromSource(Parameters, Code);
				if (Results.Errors.HasErrors == true || Results.Errors.HasWarnings == true)
				{
					if (Results.Errors.HasErrors == true) AddOutputLine("Failed!", CompilerMessageStage.Server);
					CompilerOutput(Results.Errors, CompilerMessageStage.Server);
					if (Results.Errors.HasErrors == true) return false;
				}
				AddOutputLine("Success!", CompilerMessageStage.Server);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Server);
				return false;
			}

			return true;
		}

		public bool GenerateProxy40Client(bool GenerateAssemblyCode, string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			if (File.Exists(SvcUtilPath) == false)
			{
				AddMessage(new CompileMessage("GS0798", "Unable to locate the SvcUtil executable.", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Proxy);
				return false;
			}
			List<string> AL = new List<string>(Directory.GetFiles(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), ServerAssemblyName + ".NET40Client.*.dll", SearchOption.TopDirectoryOnly));
			if (AL.Count == 0)
			{
				AddMessage(new CompileMessage("GS0799", "Unable to locate a valid output file'", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Proxy);
				return false;
			}
			string AsmFilePath = AL[0];
			List<string> RL;

			//Setup the basic process start info, this is reused.
			ProcessStartInfo PSI = new ProcessStartInfo(SvcUtilPath);
			PSI.WorkingDirectory = GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client);
			PSI.CreateNoWindow = true;
			PSI.RedirectStandardError = true;
			PSI.RedirectStandardOutput = true;
			PSI.UseShellExecute = false;
			PSI.WindowStyle = ProcessWindowStyle.Hidden;
			Process MP = new Process();
			MP.StartInfo = PSI;
			MP.ErrorDataReceived += new DataReceivedEventHandler(SvcUtilOutput);
			MP.OutputDataReceived += new DataReceivedEventHandler(SvcUtilOutput);
			Process CP = new Process();
			CP.StartInfo = PSI;
			CP.ErrorDataReceived += new DataReceivedEventHandler(SvcUtilOutput);
			CP.OutputDataReceived += new DataReceivedEventHandler(SvcUtilOutput);

			//Generate the Metadata
			StringBuilder MetadataArgs = new StringBuilder();
			MetadataArgs.Append("\"" + AsmFilePath + "\"");
			MetadataArgs.Append(" /nologo");
			MetadataArgs.Append(" /t:metadata");
			MetadataArgs.Append(" /d:\"" + GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client).Remove(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client).Length - 1, 1) + "\"");
			RL = BuildProxyReferenceList(Projects.ProjectOutputFramework.NET40Client, true, ProjectName, DependencyAssemblies);
			foreach (string R in RL)
				MetadataArgs.Append(" /r:\"" + R + "\"");
			if (GetServiceGenerateDataContracts(ProjectName) == true)
			{
				MetadataArgs.Append(" /dconly");
			}
			else
			{
				foreach (string ET in GetServiceExcludedTypes(ProjectName))
					MetadataArgs.Append(" /et:\"" + ET + "\"");
			}
			PSI.Arguments = MetadataArgs.ToString();

			//Run the process
			MP.Start();
			MP.BeginErrorReadLine();
			MP.WaitForExit();

			ParseSvcUtilOutput(ProjectName, Projects.ProjectOutputFramework.NET40Client, MP.StandardOutput.ReadToEnd());
			AddOutputLine(MP.StandardOutput.ReadToEnd(), CompilerMessageStage.Proxy);

			//Generate the Client Code
			StringBuilder CodeArgs = new StringBuilder((string)WPFThread.Invoke(new Func<string>(() => Project.GenerateClientProxyArgs(GenerateAssemblyCode, IsClientClassesInternal(ProjectName))), System.Windows.Threading.DispatcherPriority.Send));
			CodeArgs.Append(" /l:cs");
			CodeArgs.Append(" /tcv:Version35");
			RL = BuildProxyReferenceList(Projects.ProjectOutputFramework.NET40Client, false, ProjectName, DependencyAssemblies);
			foreach (string R in RL)
				CodeArgs.Append(" /r:\"" + R + "\"");
			CodeArgs.Append(" /d:\"" + GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client).Remove(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client).Length - 1, 1) + "\"");
			if (GetServiceGenerateDataContracts(ProjectName) == true)
			{
				CodeArgs.Append(" /dconly *.xsd");
			}
			else
			{
				foreach (string ET in GetServiceExcludedTypes(ProjectName))
					MetadataArgs.Append(" /et:\"" + ET + "\"");
				CodeArgs.Append(" *.wsdl *.xsd");
			}
			CodeArgs.Append(" /o:\"" + ClientAssemblyName + ".Services.NET40Client.cs\"");

			PSI.Arguments = CodeArgs.ToString();

			//DeleteUnneededXSDFiles(ProjectName, Projects.ProjectOutputFramework.NET40Client);

			OutputFiles.AddRange(GetXSDWSDLCompilerOutput(ProjectName, Projects.ProjectOutputFramework.NET40Client));

			//Run the process
			CP.Start();
			CP.BeginErrorReadLine();
			CP.BeginOutputReadLine();
			CP.WaitForExit();

			return true;
		}

		private bool CompileClient40Client(string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			List<Projects.Reference> RL = new List<Projects.Reference>(GetReferences(ProjectName).Where<Projects.Reference>(a => a.FrameworkVersionNoBuild == "4.0.0.0" && a.IsClientProfile == true));
			if (RL.Count == 0)
			{
				AddMessage(new CompileMessage("GS0697", "No References specified for the '" + ProjectName + "' project.", CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Client);
				return false;
			}
			if (File.Exists(Path.Combine(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), ClientAssemblyName + ".Services.NET40Client.cs")) == false) return false;

			string Code = (string)WPFThread.Invoke(new Func<string>(() => Project.GenerateClientCode40Client(true, ProjectName)), System.Windows.Threading.DispatcherPriority.Send);
			string ServiceCode = File.ReadAllText(Path.Combine(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), ClientAssemblyName + ".Services.NET40Client.cs"));

			CompilerParameters Parameters = new CompilerParameters();
			CompilerResults Results;

			Parameters.GenerateInMemory = false;
			Parameters.TreatWarningsAsErrors = false;
			Parameters.TempFiles = new TempFileCollection(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client));
			Parameters.TempFiles.KeepFiles = false;
			Parameters.WarningLevel = 4;
			foreach (Projects.Reference R in RL)
				Parameters.ReferencedAssemblies.Add(R.Path);

			string BaseOptions = " /optimize /define:TRACE";

			try
			{
				Parameters.ReferencedAssemblies.Clear();
				foreach (Projects.Reference R in RL)
					Parameters.ReferencedAssemblies.Add(R.Path);
				var DL = DependencyAssemblies.Where(a => a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET40Client && a.IsClientFile == true);
				foreach (var D in DL)
					Parameters.ReferencedAssemblies.Add(GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client) + D.FileName + ".dll");

				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET40Client, CompilerOutputFileType.Assembly);
				AddOutputLine("Building '" + COF.FileName + ".dll' ... ", CompilerMessageStage.Client, false);
				Parameters.CompilerOptions = BaseOptions + " /platform:anycpu ";
				Parameters.OutputAssembly = GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client) + COF.FileName + ".dll";

				Results = CSCompilerNET40.CompileAssemblyFromSource(Parameters, Code, ServiceCode);
				if (Results.Errors.HasErrors == true || Results.Errors.HasWarnings == true)
				{
					if (Results.Errors.HasErrors == true) AddOutputLine("Failed!", CompilerMessageStage.Client);
					CompilerOutput(Results.Errors, CompilerMessageStage.Client);
					if (Results.Errors.HasErrors == true) return false;
				}
				AddOutputLine("Success!", CompilerMessageStage.Client);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Client);
				return false;
			}

			return true;
		}

		#endregion

		#region - .NET 3.0 Output Generators -

		private bool OutputServer30(string ProjectName, List<CompilerOutputFile> OutputFiles)
		{
			try
			{
				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET30, CompilerOutputFileType.Assembly);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Server);
				return false;
			}

			return true;
		}

		public bool OutputProxy30(string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			OutputFiles.AddRange(GetXSDWSDLCompilerOutput(ProjectName, Projects.ProjectOutputFramework.NET30));
			return true;
		}

		private bool OutputClient30(string ProjectName, List<CompilerOutputFile> OutputFiles)
		{
			try
			{
				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET30), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET30, CompilerOutputFileType.Assembly);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Client);
				return false;
			}

			return true;
		}

		#endregion

		#region - .NET 3.5 Output Generators -

		private bool OutputServer35(string ProjectName, List<CompilerOutputFile> OutputFiles)
		{
			try
			{
				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET35, CompilerOutputFileType.Assembly);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Server);
				return false;
			}

			return true;
		}

		public bool OutputProxy35(string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			OutputFiles.AddRange(GetXSDWSDLCompilerOutput(ProjectName, Projects.ProjectOutputFramework.NET35));
			return true;
		}

		private bool OutputClient35(string ProjectName, List<CompilerOutputFile> OutputFiles)
		{
			try
			{
				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET35, CompilerOutputFileType.Assembly);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Client);
				return false;
			}

			return true;
		}

		#endregion

		#region - .NET 3.5 Client Output Generators -

		private bool OutputServer35Client(string ProjectName, List<CompilerOutputFile> OutputFiles)
		{
			try
			{
				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET35Client, CompilerOutputFileType.Assembly);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Server);
				return false;
			}

			return true;
		}

		public bool OutputProxy35Client(string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			OutputFiles.AddRange(GetXSDWSDLCompilerOutput(ProjectName, Projects.ProjectOutputFramework.NET35Client));
			return true;
		}

		private bool OutputClient35Client(string ProjectName, List<CompilerOutputFile> OutputFiles)
		{
			try
			{
				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET35Client), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET35Client, CompilerOutputFileType.Assembly);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Client);
				return false;
			}

			return true;
		}

		#endregion

		#region - .NET 4.0 Output Generators -

		private bool OutputServer40(string ProjectName, List<CompilerOutputFile> OutputFiles)
		{
			try
			{
				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET40, CompilerOutputFileType.Assembly);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Server);
				return false;
			}

			return true;
		}

		public bool OutputProxy40(string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			OutputFiles.AddRange(GetXSDWSDLCompilerOutput(ProjectName, Projects.ProjectOutputFramework.NET40));
			return true;
		}

		private bool OutputClient40(string ProjectName, List<CompilerOutputFile> OutputFiles)
		{
			try
			{
				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET40, CompilerOutputFileType.Assembly);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Client);
				return false;
			}

			return true;
		}

		#endregion

		#region - .NET 4.0 Client Output Generators -

		private bool OutputServer40Client(string ProjectName, List<CompilerOutputFile> OutputFiles)
		{
			try
			{
				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), ServerAssemblyName, true, Projects.ProjectOutputFramework.NET40Client, CompilerOutputFileType.Assembly);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Server);
				return false;
			}

			return true;
		}

		public bool OutputProxy40Client(string ProjectName, List<CompilerOutputFile> DependencyAssemblies, List<CompilerOutputFile> OutputFiles)
		{
			OutputFiles.AddRange(GetXSDWSDLCompilerOutput(ProjectName, Projects.ProjectOutputFramework.NET40Client));
			return true;
		}

		private bool OutputClient40Client(string ProjectName, List<CompilerOutputFile> OutputFiles)
		{
			try
			{
				CompilerOutputFile COF = new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Projects.ProjectOutputFramework.NET40Client), ClientAssemblyName, false, Projects.ProjectOutputFramework.NET40Client, CompilerOutputFileType.Assembly);
				OutputFiles.Add(COF);
			}
			catch (Exception ex)
			{
				AddMessage(new CompileMessage("GS0699", ex.Message, CompileMessageSeverity.Error, Project, null, null), CompilerMessageStage.Client);
				return false;
			}

			return true;
		}

		#endregion

		#region - Outputers -

		private void Output30(string ProjectName, List<CompilerOutputFile> OutputFiles)
		{
			List<Projects.ProjectOutputPath> SOPL = new List<Projects.ProjectOutputPath>(GetServerOutputPaths(ProjectName).Where<Projects.ProjectOutputPath>(a => a.Framework == Projects.ProjectOutputFramework.NET30 && a.IsEnabled == true));
			List<Projects.ProjectOutputPath> COPL = new List<Projects.ProjectOutputPath>(GetClientOutputPaths(ProjectName).Where<Projects.ProjectOutputPath>(a => a.Framework == Projects.ProjectOutputFramework.NET30 && a.IsEnabled == true));

			foreach (Projects.ProjectOutputPath OP in SOPL)
			{
				if (OP.GenerateAssembly == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsServerFile == true && a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET30));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.GenerateCode == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsServerFile == true && a.FileType == CompilerOutputFileType.Code && a.Framework == Projects.ProjectOutputFramework.NET30));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.OutputXSDWSDL == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsServerFile == true && a.FileType == CompilerOutputFileType.XSDWSDL && a.Framework == Projects.ProjectOutputFramework.NET30));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
			}

			foreach (Projects.ProjectOutputPath OP in COPL)
			{
				if (OP.GenerateAssembly == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsClientFile == true && a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET30));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.GenerateCode == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsClientFile == true && a.FileType == CompilerOutputFileType.Code && a.Framework == Projects.ProjectOutputFramework.NET30));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.OutputXSDWSDL == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsClientFile == true && a.FileType == CompilerOutputFileType.XSDWSDL && a.Framework == Projects.ProjectOutputFramework.NET30));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
			}
		}

		private void Output35(string ProjectName, List<CompilerOutputFile> OutputFiles)
		{
			List<Projects.ProjectOutputPath> SOPL = new List<Projects.ProjectOutputPath>(GetServerOutputPaths(ProjectName).Where<Projects.ProjectOutputPath>(a => a.Framework == Projects.ProjectOutputFramework.NET35 && a.IsEnabled == true));
			List<Projects.ProjectOutputPath> COPL = new List<Projects.ProjectOutputPath>(GetClientOutputPaths(ProjectName).Where<Projects.ProjectOutputPath>(a => a.Framework == Projects.ProjectOutputFramework.NET35 && a.IsEnabled == true));

			foreach (Projects.ProjectOutputPath OP in SOPL)
			{
				if (OP.GenerateAssembly == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsServerFile == true && a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET35));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.GenerateCode == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsServerFile == true && a.FileType == CompilerOutputFileType.Code && a.Framework == Projects.ProjectOutputFramework.NET35));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.OutputXSDWSDL == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsServerFile == true && a.FileType == CompilerOutputFileType.XSDWSDL && a.Framework == Projects.ProjectOutputFramework.NET35));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
			}

			foreach (Projects.ProjectOutputPath OP in COPL)
			{
				if (OP.GenerateAssembly == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsClientFile == true && a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET35));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.GenerateCode == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsClientFile == true && a.FileType == CompilerOutputFileType.Code && a.Framework == Projects.ProjectOutputFramework.NET35));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.OutputXSDWSDL == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsClientFile == true && a.FileType == CompilerOutputFileType.XSDWSDL && a.Framework == Projects.ProjectOutputFramework.NET35));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
			}
		}

		private void Output35Client(string ProjectName, List<CompilerOutputFile> OutputFiles)
		{
			List<Projects.ProjectOutputPath> SOPL = new List<Projects.ProjectOutputPath>(GetServerOutputPaths(ProjectName).Where<Projects.ProjectOutputPath>(a => a.Framework == Projects.ProjectOutputFramework.NET35Client && a.IsEnabled == true));
			List<Projects.ProjectOutputPath> COPL = new List<Projects.ProjectOutputPath>(GetClientOutputPaths(ProjectName).Where<Projects.ProjectOutputPath>(a => a.Framework == Projects.ProjectOutputFramework.NET35Client && a.IsEnabled == true));

			foreach (Projects.ProjectOutputPath OP in SOPL)
			{
				if (OP.GenerateAssembly == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsServerFile == true && a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET35Client));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.GenerateCode == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsServerFile == true && a.FileType == CompilerOutputFileType.Code && a.Framework == Projects.ProjectOutputFramework.NET35Client));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.OutputXSDWSDL == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsServerFile == true && a.FileType == CompilerOutputFileType.XSDWSDL && a.Framework == Projects.ProjectOutputFramework.NET35Client));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
			}

			foreach (Projects.ProjectOutputPath OP in COPL)
			{
				if (OP.GenerateAssembly == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsClientFile == true && a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET35Client));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.GenerateCode == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsClientFile == true && a.FileType == CompilerOutputFileType.Code && a.Framework == Projects.ProjectOutputFramework.NET35Client));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.OutputXSDWSDL == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsClientFile == true && a.FileType == CompilerOutputFileType.XSDWSDL && a.Framework == Projects.ProjectOutputFramework.NET35Client));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
			}
		}

		private void Output40(string ProjectName, List<CompilerOutputFile> OutputFiles)
		{
			List<Projects.ProjectOutputPath> SOPL = new List<Projects.ProjectOutputPath>(GetServerOutputPaths(ProjectName).Where<Projects.ProjectOutputPath>(a => a.Framework == Projects.ProjectOutputFramework.NET40 && a.IsEnabled == true));
			List<Projects.ProjectOutputPath> COPL = new List<Projects.ProjectOutputPath>(GetClientOutputPaths(ProjectName).Where<Projects.ProjectOutputPath>(a => a.Framework == Projects.ProjectOutputFramework.NET40 && a.IsEnabled == true));

			foreach (Projects.ProjectOutputPath OP in SOPL)
			{
				if (OP.GenerateAssembly == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsServerFile == true && a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET40));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.GenerateCode == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsServerFile == true && a.FileType == CompilerOutputFileType.Code && a.Framework == Projects.ProjectOutputFramework.NET40));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.OutputXSDWSDL == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsServerFile == true && a.FileType == CompilerOutputFileType.XSDWSDL && a.Framework == Projects.ProjectOutputFramework.NET40));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
			}

			foreach (Projects.ProjectOutputPath OP in COPL)
			{
				if (OP.GenerateAssembly == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsClientFile == true && a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET40));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.GenerateCode == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsClientFile == true && a.FileType == CompilerOutputFileType.Code && a.Framework == Projects.ProjectOutputFramework.NET40));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.OutputXSDWSDL == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsClientFile == true && a.FileType == CompilerOutputFileType.XSDWSDL && a.Framework == Projects.ProjectOutputFramework.NET40));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
			}
		}

		private void Output40Client(string ProjectName, List<CompilerOutputFile> OutputFiles)
		{
			List<Projects.ProjectOutputPath> SOPL = new List<Projects.ProjectOutputPath>(GetServerOutputPaths(ProjectName).Where<Projects.ProjectOutputPath>(a => a.Framework == Projects.ProjectOutputFramework.NET40Client && a.IsEnabled == true));
			List<Projects.ProjectOutputPath> COPL = new List<Projects.ProjectOutputPath>(GetClientOutputPaths(ProjectName).Where<Projects.ProjectOutputPath>(a => a.Framework == Projects.ProjectOutputFramework.NET40Client && a.IsEnabled == true));

			foreach (Projects.ProjectOutputPath OP in SOPL)
			{
				if (OP.GenerateAssembly == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsServerFile == true && a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET40Client));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.GenerateCode == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsServerFile == true && a.FileType == CompilerOutputFileType.Code && a.Framework == Projects.ProjectOutputFramework.NET40Client));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.OutputXSDWSDL == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsServerFile == true && a.FileType == CompilerOutputFileType.XSDWSDL && a.Framework == Projects.ProjectOutputFramework.NET40Client));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
			}

			foreach (Projects.ProjectOutputPath OP in COPL)
			{
				if (OP.GenerateAssembly == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsClientFile == true && a.FileType == CompilerOutputFileType.Assembly && a.Framework == Projects.ProjectOutputFramework.NET40Client));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.GenerateCode == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsClientFile == true && a.FileType == CompilerOutputFileType.Code && a.Framework == Projects.ProjectOutputFramework.NET40Client));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
				if (OP.OutputXSDWSDL == true)
				{
					List<CompilerOutputFile> FL = new List<CompilerOutputFile>(OutputFiles.Where<CompilerOutputFile>(a => a.IsClientFile == true && a.FileType == CompilerOutputFileType.XSDWSDL && a.Framework == Projects.ProjectOutputFramework.NET40Client));
					foreach (CompilerOutputFile COF in FL)
						COF.Copy(OP.Path);
				}
			}
		}

		#endregion

		#region - Compiler Helpers -

		public void GenerateClientProxyNamespaces(StringBuilder Args, Projects.Namespace N)
		{
			Args.AppendFormat(" /n:\"{0}\",\"{1}\"", N.URI, N.FullName);

			foreach (Projects.Namespace C in N.Children)
				GenerateClientProxyNamespaces(Args, C);
		}

		private void ParseSvcUtilOutput(string ProjectName, Projects.ProjectOutputFramework Framework, string Output)
		{
			List<string> Lines = new List<string>(Output.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));			
			List<string> FileList = new List<string>();
			string RXSD = (string)WPFThread.Invoke(new Func<string>(() => { return Project.Namespace.URI; }), System.Windows.Threading.DispatcherPriority.Send);
			if (RXSD.Contains("://") == true) RXSD = RXSD.Remove(0, RXSD.IndexOf("://") + 3);
			RXSD = RXSD.Remove(RXSD.IndexOf("/"));
			RXSD = RXSD.Replace("/", ".");
			RXSD = RXSD.ToLower();

			foreach (string L in Lines)
			{
				if (L.ToLower().Contains("www.w3.org.2001.xmlschema.xsd") == true) FileList.Add(L);
				if (L.ToLower().Contains("schemas.microsoft.com.message.xsd") == true) FileList.Add(L);
				if (L.ToLower().Contains("schemas.microsoft.com.2003.10.serialization.xsd") == true) FileList.Add(L);
				if (L.ToLower().Contains("schemas.microsoft.com.2003.10.serialization.arrays.xsd") == true) FileList.Add(L);
				if (L.ToLower().Contains(RXSD) == true && L.ToLower().EndsWith(".xsd") == true) FileList.Add(L);
				if (L.ToLower().EndsWith(".wsdl") == true) FileList.Add(L);
			}

			List<string> XSDFiles = new List<string>(Directory.GetFiles(GetBuildOutputPath(ProjectName, Framework), "*.xsd", SearchOption.TopDirectoryOnly));
			foreach (string XSD in XSDFiles)
			{
				bool Delete = true;
				foreach (string FL in FileList) if (XSD == FL) Delete = false;
				if (Delete == true) File.Delete(XSD);
			}
		}

		//private void DeleteUnneededXSDFiles(string ProjectName, Projects.ProjectOutputFramework Framework)
		//{
		//    List<string> XSDList = (List<string>)WPFThread.Invoke(new Func<List<string>>(() => GetXSDList(Project, ProjectName, Framework)), System.Windows.Threading.DispatcherPriority.Send);
		//    if (File.Exists(GetBuildOutputPath(ProjectName, Framework) + "www.w3.org.2001.XMLSchema.xsd") == true) XSDList.Add(GetBuildOutputPath(ProjectName, Framework) + "www.w3.org.2001.XMLSchema.xsd");
		//    if (File.Exists(GetBuildOutputPath(ProjectName, Framework) + "schemas.microsoft.com.Message.xsd") == true) XSDList.Add(GetBuildOutputPath(ProjectName, Framework) + "schemas.microsoft.com.Message.xsd");
		//    if (File.Exists(GetBuildOutputPath(ProjectName, Framework) + "schemas.microsoft.com.2003.10.Serialization.xsd") == true) XSDList.Add(GetBuildOutputPath(ProjectName, Framework) + "schemas.microsoft.com.2003.10.Serialization.xsd");
		//    if (File.Exists(GetBuildOutputPath(ProjectName, Framework) + "schemas.microsoft.com.2003.10.Serialization.Arrays.xsd") == true) XSDList.Add(GetBuildOutputPath(ProjectName, Framework) + "schemas.microsoft.com.2003.10.Serialization.Arrays.xsd");

		//    List<string> KeepList = new List<string>();
		//    foreach (string XSDS in XSDList)
		//    {
		//        List<string> TFL = new List<string>(Directory.GetFiles(GetBuildOutputPath(ProjectName, Framework), Path.GetFileName(XSDS), SearchOption.TopDirectoryOnly));
		//        foreach (string TFLI in TFL) KeepList.Add(TFLI);
		//    }

		//    List<string> XSDFiles = new List<string>(Directory.GetFiles(GetBuildOutputPath(ProjectName, Framework), "*.xsd", SearchOption.TopDirectoryOnly));
		//    List<string> DeleteList = new List<string>();

		//    foreach (string XSDF in XSDFiles)
		//    {
		//        bool IsXSD = false;
		//        foreach (string XSD in KeepList)
		//            if (XSD == XSDF) IsXSD = true;
		//        if (IsXSD == false) DeleteList.Add(XSDF);
		//    }

		//    foreach (string DF in DeleteList)
		//        File.Delete(DF);
		//}

		//private List<string> GetXSDList(Projects.Project Project, string ProjectName, Projects.ProjectOutputFramework Framework)
		//{
		//    List<string> XSDL = new List<string>();

		//    //Hack for root namespace
		//    string RXSD = Project.Namespace.URI;
		//    if (RXSD.Contains("://") == true) RXSD = RXSD.Remove(0, RXSD.IndexOf("://") + 3);
		//    if (RXSD.EndsWith("/") == true) RXSD = RXSD.Remove(RXSD.Length - 1);
		//    RXSD = RXSD.Replace("/", ".");
		//    XSDL.Add(GetBuildOutputPath(ProjectName, Framework) + RXSD + "*.xsd");

		//    foreach (Projects.Namespace N in Project.Namespaces)
		//    {
		//        string XSD = N.URI;
		//        if (XSD.Contains("://") == true) XSD = XSD.Remove(0, XSD.IndexOf("://") + 3);
		//        XSD = XSD.Replace("/", ".");
		//        XSDL.Add(GetBuildOutputPath(ProjectName, Framework) + XSD + "*.xsd");
		//    }

		//    foreach (Projects.Project P in Project.DependencyProjects)
		//        XSDL.AddRange(GetXSDList(P, ProjectName, Framework));

		//    return XSDL;
		//}

		List<CompilerOutputFile> GetXSDWSDLCompilerOutput(string ProjectName, Projects.ProjectOutputFramework Framework)
		{
			List<string> WSDL = new List<string>(Directory.EnumerateFiles(GetBuildOutputPath(ProjectName, Framework), "*.wsdl", SearchOption.TopDirectoryOnly));
			List<string> XSD = new List<string>(Directory.EnumerateFiles(GetBuildOutputPath(ProjectName, Framework), "*.xsd", SearchOption.TopDirectoryOnly));

			List<CompilerOutputFile> COFL = new List<CompilerOutputFile>();

			foreach (string f in WSDL)
			{
				COFL.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Framework), Path.GetFileName(f), true, Framework, CompilerOutputFileType.XSDWSDL, CompilerOutputLanguage.None));
				COFL.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Framework), Path.GetFileName(f), false, Framework, CompilerOutputFileType.XSDWSDL, CompilerOutputLanguage.None));
			}

			foreach (string f in XSD)
			{
				COFL.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Framework), Path.GetFileName(f), true, Framework, CompilerOutputFileType.XSDWSDL, CompilerOutputLanguage.None));
				COFL.Add(new CompilerOutputFile(this, GetBuildOutputPath(ProjectName, Framework), Path.GetFileName(f), false, Framework, CompilerOutputFileType.XSDWSDL, CompilerOutputLanguage.None));
			}

			return COFL;
		}

		public List<string> BuildProxyReferenceList(Projects.ProjectOutputFramework Framework, bool IsServer, string ProjectName, List<CompilerOutputFile> DependencyAssemblies)
		{
			List<string> RL = new List<string>();

			List<CompilerOutputFile> COFL = new List<CompilerOutputFile>(DependencyAssemblies.Where(a => a.Framework == Framework && a.IsServerFile == IsServer));
			List<CompilerOutputFile> TOFL = new List<CompilerOutputFile>();

			if (COFL.Count > 0)
			{
				TOFL.Add(COFL[0]);
				foreach (CompilerOutputFile COF in COFL)
				{
					bool HasFile = false;
					foreach (CompilerOutputFile TOF in TOFL)
						if (TOF.BaseFileName == COF.BaseFileName) HasFile = true;
					if (HasFile == false) TOFL.Add(COF);
				}
			}

			List<Projects.Reference> PRL = new List<Projects.Reference>();
			if (Framework == Projects.ProjectOutputFramework.NET30) PRL = new List<Projects.Reference>(GetReferences(ProjectName).Where<Projects.Reference>(a => (a.FrameworkVersionNoBuild == "3.0.0.0" || a.FrameworkVersionNoBuild == "2.0.0.0") && a.IsGACInstalled == false && a.IsClientProfile == false));
			if (Framework == Projects.ProjectOutputFramework.NET35) PRL = new List<Projects.Reference>(GetReferences(ProjectName).Where<Projects.Reference>(a => (a.FrameworkVersionNoBuild == "3.5.0.0" || a.FrameworkVersionNoBuild == "3.0.0.0" || a.FrameworkVersionNoBuild == "2.0.0.0") && a.IsGACInstalled == false && a.IsClientProfile == false));
			if (Framework == Projects.ProjectOutputFramework.NET35Client) PRL = new List<Projects.Reference>(GetReferences(ProjectName).Where<Projects.Reference>(a => (a.FrameworkVersionNoBuild == "3.5.0.0" || a.FrameworkVersionNoBuild == "3.0.0.0" || a.FrameworkVersionNoBuild == "2.0.0.0") && a.IsGACInstalled == false && a.IsClientProfile == true));
			if (Framework == Projects.ProjectOutputFramework.NET40) PRL = new List<Projects.Reference>(GetReferences(ProjectName).Where<Projects.Reference>(a => a.FrameworkVersionNoBuild == "4.0.0.0" && a.IsGACInstalled == false && a.IsClientProfile == false));
			if (Framework == Projects.ProjectOutputFramework.NET40Client) PRL = new List<Projects.Reference>(GetReferences(ProjectName).Where<Projects.Reference>(a => a.FrameworkVersionNoBuild == "4.0.0.0" && a.IsGACInstalled == false && a.IsClientProfile == true));
			if (Framework == Projects.ProjectOutputFramework.NET30) PRL.AddRange(GetReferences(ProjectName).Where<Projects.Reference>(a => (a.FrameworkVersionNoBuild == "3.0.0.0" || a.FrameworkVersionNoBuild == "2.0.0.0") && (a.Name == "System" || a.Name == "WindowsBase") && a.IsClientProfile == false));
			if (Framework == Projects.ProjectOutputFramework.NET35) PRL.AddRange(GetReferences(ProjectName).Where<Projects.Reference>(a => (a.FrameworkVersionNoBuild == "3.5.0.0" || a.FrameworkVersionNoBuild == "3.0.0.0" || a.FrameworkVersionNoBuild == "2.0.0.0") && (a.Name == "System" || a.Name == "WindowsBase") && a.IsClientProfile == false));
			if (Framework == Projects.ProjectOutputFramework.NET35Client) PRL.AddRange(GetReferences(ProjectName).Where<Projects.Reference>(a => (a.FrameworkVersionNoBuild == "3.5.0.0" || a.FrameworkVersionNoBuild == "3.0.0.0" || a.FrameworkVersionNoBuild == "2.0.0.0") && (a.Name == "System" || a.Name == "WindowsBase") && a.IsClientProfile == true));
			if (Framework == Projects.ProjectOutputFramework.NET40) PRL.AddRange(GetReferences(ProjectName).Where<Projects.Reference>(a => a.FrameworkVersionNoBuild == "4.0.0.0" && a.Name == "System" && a.IsClientProfile == false));
			if (Framework == Projects.ProjectOutputFramework.NET40Client) PRL.AddRange(GetReferences(ProjectName).Where<Projects.Reference>(a => a.FrameworkVersionNoBuild == "4.0.0.0" && a.Name == "System" && a.IsClientProfile == true));

			foreach (CompilerOutputFile COF in TOFL)
				RL.Add(GetBuildOutputPath(ProjectName, Framework) + COF.FileName + ".dll");
			foreach (Projects.Reference R in PRL)
				RL.Add(R.Path);

			return RL;
		}

		#endregion

		#region - Error Output Helpers -

		private void CompilerOutput(CompilerErrorCollection Errors, CompilerMessageStage Stage)
		{
			foreach (CompilerError CE in Errors)
			{
				AddMessage(new CompileMessage(CE.ErrorNumber, CE.ErrorText, CE.IsWarning ? CompileMessageSeverity.Warning : CompileMessageSeverity.Error, Project, null, null, CE.Line.ToString()), Stage);
			}
		}

		private void SvcUtilOutput(object sender, DataReceivedEventArgs e)
		{
			if (e.Data == null) return;

			AddOutputLine(e.Data, CompilerMessageStage.Proxy);
		}

		public void AddMessage(CompileMessage Message, CompilerMessageStage Stage = CompilerMessageStage.Verify)
		{
			WPFThread.BeginInvoke(new Action<CompileMessage, CompilerMessageStage>(AddMessageAction), System.Windows.Threading.DispatcherPriority.Normal, Message, Stage);
		}

		public void AddMessageAction(CompileMessage Message, CompilerMessageStage Stage)
		{
			Paragraph OutputBlock = VerifyCodeOutput;
			if (Stage == CompilerMessageStage.Server) OutputBlock = ServerBuildOutput;
			if (Stage == CompilerMessageStage.Proxy) OutputBlock = ProxyGenerateOutput;
			if (Stage == CompilerMessageStage.Client) OutputBlock = ClientBuildOutput;
			if (Stage == CompilerMessageStage.Output) OutputBlock = FileOutput;

			Messages.Add(Message);
			if (Message.Severity == CompileMessageSeverity.Error) HasErrors = true;

			OutputBlock.Inlines.Add(new Run(Message.Severity.ToString() + " " + Message.Code + ":\t" + Message.Description));
			OutputBlock.Inlines.Add(new LineBreak());
		}

		public void AddOutputLine(CompilerMessageStage Stage)
		{
			WPFThread.BeginInvoke(new Action<string, CompilerMessageStage, bool>(AddOutputLineAction), System.Windows.Threading.DispatcherPriority.Normal, "", Stage, true);
		}

		public void AddOutputLine(string Message, CompilerMessageStage Stage, bool NewLine = true)
		{
			WPFThread.BeginInvoke(new Action<string, CompilerMessageStage, bool>(AddOutputLineAction), System.Windows.Threading.DispatcherPriority.Normal, Message, Stage, NewLine);
		}

		private void AddOutputLineAction(string Message, CompilerMessageStage Stage, bool NewLine)
		{
			Paragraph OutputBlock = VerifyCodeOutput;
			if (Stage == CompilerMessageStage.Server) OutputBlock = ServerBuildOutput;
			if (Stage == CompilerMessageStage.Proxy) OutputBlock = ProxyGenerateOutput;
			if (Stage == CompilerMessageStage.Client) OutputBlock = ClientBuildOutput;
			if (Stage == CompilerMessageStage.Output) OutputBlock = FileOutput;

			if (Message == "")
			{
				OutputBlock.Inlines.Add(new LineBreak());
			}
			else
			{
				OutputBlock.Inlines.Add(new Run(Message));
				if (NewLine == true) OutputBlock.Inlines.Add(new LineBreak());
			}
		}

		public void AddMessageStage(CompilerMessageStage Stage)
		{
			if (Stage == CompilerMessageStage.Server) WPFThread.Invoke(new Action(() => { Output.Blocks.Add(ServerBuildOutputHeader); Output.Blocks.Add(ServerBuildOutput); }), System.Windows.Threading.DispatcherPriority.Send);
			if (Stage == CompilerMessageStage.Proxy) WPFThread.Invoke(new Action(() => { Output.Blocks.Add(ProxyGenerateOutputHeader); Output.Blocks.Add(ProxyGenerateOutput); }), System.Windows.Threading.DispatcherPriority.Send);
			if (Stage == CompilerMessageStage.Client) WPFThread.Invoke(new Action(() => { Output.Blocks.Add(ClientBuildOutputHeader); Output.Blocks.Add(ClientBuildOutput); }), System.Windows.Threading.DispatcherPriority.Send);
			if (Stage == CompilerMessageStage.Output) WPFThread.Invoke(new Action(() => { Output.Blocks.Add(FileOutputHeader); Output.Blocks.Add(FileOutput); }), System.Windows.Threading.DispatcherPriority.Send);
			if (Stage == CompilerMessageStage.Complete) WPFThread.Invoke(new Action(() => { Output.Blocks.Add(BuildCompleteHeader); }), System.Windows.Threading.DispatcherPriority.Send);
		}

		#endregion
	}

	#region - Compiler Output -

	internal enum CompilerOutputFileType
	{
		Code,
		Assembly,
		XSDWSDL,
	}

	internal enum CompilerOutputLanguage
	{
		None,
		CSharp,
		VisualBasic,
	}

	internal class CompilerOutputFile
	{
		private Compiler compiler;
		private string buildPath;
		private string fileName;
		private bool isServerFile;
		private bool isClientFile;
		private CompilerOutputFileType fileType;
		private Projects.ProjectOutputFramework framework;
		private CompilerOutputLanguage language;

		public string BaseFileName { get { return fileName; } }
		public bool IsServerFile { get { return isServerFile; } }
		public bool IsClientFile { get { return isClientFile; } }
		public CompilerOutputFileType FileType { get { return fileType; } }
		public Projects.ProjectOutputFramework Framework { get { return framework; } }
		public CompilerOutputLanguage Language { get { return language; } }

		public CompilerOutputFile(Compiler Compiler, string BuildPath, string FileName, bool IsServerFile, Projects.ProjectOutputFramework Framework, CompilerOutputFileType FileType, CompilerOutputLanguage Langauage = CompilerOutputLanguage.CSharp)
		{
			compiler = Compiler;
			buildPath = BuildPath;
			fileName = FileName;
			isServerFile = IsServerFile;
			isClientFile = !IsServerFile;
			fileType = FileType;
			framework = Framework;
			language = Langauage;
		}

		public string FileName
		{
			get
			{
				StringBuilder FN = new StringBuilder(fileName);
				if (FileType == CompilerOutputFileType.Assembly)
				{
					FN.Append(".");
					FN.Append(System.Enum.GetName(typeof(Projects.ProjectOutputFramework), framework));
				}
				else
				{
					if (language == CompilerOutputLanguage.CSharp)
					{
						FN.Append(".");
						FN.Append(System.Enum.GetName(typeof(Projects.ProjectOutputFramework), framework));
					}
					else if (language == CompilerOutputLanguage.VisualBasic)
					{
						FN.Append(".");
						FN.Append(System.Enum.GetName(typeof(Projects.ProjectOutputFramework), framework));
					}
				}
				return FN.ToString();
			}
		}
		
		public bool Copy(string DestPath)
		{
			if (FileType == CompilerOutputFileType.Assembly)
			{
				try
				{
					if (File.Exists(DestPath + FileName + ".dll"))
					{
						FileInfo fi = new FileInfo(DestPath + FileName + ".dll");
						if (fi.IsReadOnly == true)
						{
							compiler.AddMessage(new CompileMessage("OS5000", "The destination file '" + FileName + ".dll' is marked as read-only. Please set the file to set the file as writeable and compile the project again.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output);
							return false;
						}
					}
					compiler.AddOutputLine("Copying '" + FileName + ".dll' to " + DestPath + " ... ", CompilerMessageStage.Output, false);
					File.Copy(buildPath + FileName + ".dll", DestPath + FileName + ".dll", true);
					compiler.AddOutputLine("Complete!", CompilerMessageStage.Output);
				}
				catch (UnauthorizedAccessException) { compiler.AddMessage(new CompileMessage("OS5001", "Failed! You do not have permission to access the path '" + DestPath + "'.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (ArgumentException) { compiler.AddMessage(new CompileMessage("OS5002", "Failed! The path '" + DestPath + "' contains invalid characters.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (PathTooLongException) { compiler.AddMessage(new CompileMessage("OS5003", "Failed! The path '" + DestPath + "' is too long.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (DirectoryNotFoundException) { compiler.AddMessage(new CompileMessage("OS5004", "Failed! The path '" + DestPath + "' was not found.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (FileNotFoundException) { compiler.AddMessage(new CompileMessage("OS5005", "Failed! The source file was not found. This is typically due to a failed build.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (IOException) { compiler.AddMessage(new CompileMessage("OS5006", "Failed! I/O Error.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (NotSupportedException) { compiler.AddMessage(new CompileMessage("OS5007", "Failed! The path '" + DestPath + "' contains invalid characters.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (Exception) { compiler.AddMessage(new CompileMessage("OS5008", "Failed! Unspecified Reason.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				try
				{
					if (File.Exists(DestPath + FileName + ".pdb"))
					{
						FileInfo fi = new FileInfo(DestPath + FileName + ".pdb");
						if (fi.IsReadOnly == true)
						{
							compiler.AddMessage(new CompileMessage("OS5000", "The destination file '" + FileName + ".pdb' is marked as read-only. Please set the file to set the file as writeable and compile the project again.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output);
							return false;
						}
					}
					compiler.AddOutputLine("Copying '" + FileName + ".pdb' to " + DestPath + " ... ", CompilerMessageStage.Output, false);
					File.Copy(buildPath + FileName + ".pdb", DestPath + FileName + ".pdb", true);
					compiler.AddOutputLine("Complete!", CompilerMessageStage.Output);
				}
				catch (UnauthorizedAccessException) { compiler.AddMessage(new CompileMessage("OS5001", "Failed! You do not have permission to access the path '" + DestPath + "'.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (ArgumentException) { compiler.AddMessage(new CompileMessage("OS5002", "Failed! The path '" + DestPath + "' contains invalid characters.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (PathTooLongException) { compiler.AddMessage(new CompileMessage("OS5003", "Failed! The path '" + DestPath + "' is too long.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (DirectoryNotFoundException) { compiler.AddMessage(new CompileMessage("OS5004", "Failed! The path '" + DestPath + "' was not found.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (FileNotFoundException) { compiler.AddMessage(new CompileMessage("OS5005", "Failed! The source file was not found. This is typically due to a failed build.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (IOException) { compiler.AddMessage(new CompileMessage("OS5006", "Failed! I/O Error.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (NotSupportedException) { compiler.AddMessage(new CompileMessage("OS5007", "Failed! The path '" + DestPath + "' contains invalid characters.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (Exception) { compiler.AddMessage(new CompileMessage("OS5008", "Failed! Unspecified Reason.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
			}
			else
			{
				if (language == CompilerOutputLanguage.CSharp)
				{
					try
					{
						if (File.Exists(DestPath + FileName + ".cs"))
						{
							FileInfo fi = new FileInfo(DestPath + FileName + ".cs");
							if (fi.IsReadOnly == true)
							{
								compiler.AddMessage(new CompileMessage("OS5000", "The destination file '" + FileName + ".cs' is marked as read-only. Please set the file to set the file as writeable and compile the project again.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output);
								return false;
							}
						}
						compiler.AddOutputLine("Copying '" + FileName + ".cs' to " + DestPath + " ... ", CompilerMessageStage.Output, false);
						File.Copy(buildPath + FileName + ".cs", DestPath + FileName + ".cs", true);
						compiler.AddOutputLine("Complete!", CompilerMessageStage.Output);
					}
					catch (UnauthorizedAccessException) { compiler.AddMessage(new CompileMessage("OS5001", "Failed! You do not have permission to access the path '" + DestPath + "'.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
					catch (ArgumentException) { compiler.AddMessage(new CompileMessage("OS5002", "Failed! The path '" + DestPath + "' contains invalid characters.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
					catch (PathTooLongException) { compiler.AddMessage(new CompileMessage("OS5003", "Failed! The path '" + DestPath + "' is too long.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
					catch (DirectoryNotFoundException) { compiler.AddMessage(new CompileMessage("OS5004", "Failed! The path '" + DestPath + "' was not found.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
					catch (FileNotFoundException) { compiler.AddMessage(new CompileMessage("OS5005", "Failed! The source file was not found. This is typically due to a failed build.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
					catch (IOException) { compiler.AddMessage(new CompileMessage("OS5006", "Failed! I/O Error.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
					catch (NotSupportedException) { compiler.AddMessage(new CompileMessage("OS5007", "Failed! The path '" + DestPath + "' contains invalid characters.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
					catch (Exception) { compiler.AddMessage(new CompileMessage("OS5008", "Failed! Unspecified Reason.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				}
				else if (language == CompilerOutputLanguage.VisualBasic)
				{
					try
					{
						if (File.Exists(DestPath + FileName + ".vb"))
						{
							FileInfo fi = new FileInfo(DestPath + FileName + ".vb");
							if (fi.IsReadOnly == true)
							{
								compiler.AddMessage(new CompileMessage("OS5000", "The destination file '" + FileName + ".vb' is marked as read-only. Please set the file to set the file as writeable and compile the project again.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output);
								return false;
							}
						}
						compiler.AddOutputLine("Copying '" + FileName + ".vb' to " + DestPath + " ... ", CompilerMessageStage.Output, false);
						File.Copy(buildPath + FileName + ".vb", DestPath + FileName + ".vb", true);
						compiler.AddOutputLine("Complete!", CompilerMessageStage.Output);
					}
					catch (UnauthorizedAccessException) { compiler.AddMessage(new CompileMessage("OS5001", "Failed! You do not have permission to access the path '" + DestPath + "'.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
					catch (ArgumentException) { compiler.AddMessage(new CompileMessage("OS5002", "Failed! The path '" + DestPath + "' contains invalid characters.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
					catch (PathTooLongException) { compiler.AddMessage(new CompileMessage("OS5003", "Failed! The path '" + DestPath + "' is too long.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
					catch (DirectoryNotFoundException) { compiler.AddMessage(new CompileMessage("OS5004", "Failed! The path '" + DestPath + "' was not found.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
					catch (FileNotFoundException) { compiler.AddMessage(new CompileMessage("OS5005", "Failed! The source file was not found. This is typically due to a failed build.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
					catch (IOException) { compiler.AddMessage(new CompileMessage("OS5006", "Failed! I/O Error.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
					catch (NotSupportedException) { compiler.AddMessage(new CompileMessage("OS5007", "Failed! The path '" + DestPath + "' contains invalid characters.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
					catch (Exception) { compiler.AddMessage(new CompileMessage("OS5008", "Failed! Unspecified Reason.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				}
			}
			if (FileType == CompilerOutputFileType.XSDWSDL)
			{
				try
				{
					if (File.Exists(DestPath + FileName))
					{
						FileInfo fi = new FileInfo(DestPath + FileName);
						if (fi.IsReadOnly == true)
						{
							compiler.AddMessage(new CompileMessage("OS5000", "The destination file '" + FileName + "' is marked as read-only. Please set the file to set the file as writeable and compile the project again.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output);
							return false;
						}
					}
					compiler.AddOutputLine("Copying '" + FileName + "' to " + DestPath + " ... ", CompilerMessageStage.Output, false);
					File.Copy(buildPath + FileName, DestPath + FileName, true);
					compiler.AddOutputLine("Complete!", CompilerMessageStage.Output);
				}
				catch (UnauthorizedAccessException) { compiler.AddMessage(new CompileMessage("OS5001", "Failed! You do not have permission to access the path '" + DestPath + "'.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (ArgumentException) { compiler.AddMessage(new CompileMessage("OS5002", "Failed! The path '" + DestPath + "' contains invalid characters.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (PathTooLongException) { compiler.AddMessage(new CompileMessage("OS5003", "Failed! The path '" + DestPath + "' is too long.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (DirectoryNotFoundException) { compiler.AddMessage(new CompileMessage("OS5004", "Failed! The path '" + DestPath + "' was not found.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (FileNotFoundException) { compiler.AddMessage(new CompileMessage("OS5005", "Failed! The source file was not found. This is typically due to a failed build.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (IOException) { compiler.AddMessage(new CompileMessage("OS5006", "Failed! I/O Error.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (NotSupportedException) { compiler.AddMessage(new CompileMessage("OS5007", "Failed! The path '" + DestPath + "' contains invalid characters.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
				catch (Exception) { compiler.AddMessage(new CompileMessage("OS5008", "Failed! Unspecified Reason.", CompileMessageSeverity.Error, null, null, null), CompilerMessageStage.Output); return false; }
			}

			return true;
		}
	}
	#endregion

	internal enum CompileMessageSeverity
	{
		Message,
		Warning,
		Error,
	}

	internal class CompileMessage
	{
		public string Code { get; private set; }
		public string Description { get; private set; }
		public CompileMessageSeverity Severity { get; private set; }
		public string Line { get; private set; }
		public object Owner { get; private set; }
		public object ErrorObject { get; private set; }
		public Type ErrorObjectType { get; private set; }

		public CompileMessage(string Code, string Description, CompileMessageSeverity Severity, object Owner, object ErrorObject, Type ErrorObjectType, string Line = "")
		{
			this.Code = Code;
			this.Description = Description;
			this.Severity = Severity;
			this.Line = Line;
			this.Owner = Owner;
			this.ErrorObject = ErrorObject;
			this.ErrorObjectType = ErrorObjectType;
		}
	}
}