using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using NETPath.Generators.Interfaces;
using NETPath.Projects;
using NETPath.Projects.Helpers;

namespace NETPath.Generators.CS
{
	public class Generator : IGenerator
	{
		public Action<Guid, string> NewOutput { get; private set; }
		public Action<Guid, CompileMessage> NewMessage { get; private set; }
		public ObservableCollection<CompileMessage> Messages { get; private set; }
		public CompileMessageSeverity HighestSeverity { get; private set; }
		public string Name { get; private set; }
		public GenerationLanguage Language { get; private set; }
		public GenerationModule Module { get; private set; }
		public bool IsInitialized { get; private set; }

		public Generator()
		{
			Messages = new ObservableCollection<CompileMessage>();
			Name = "NETPath .NET CSharp Generator";
			Language = GenerationLanguage.CSharp;
			Module = GenerationModule.WindowsRuntime;
		}

		public void Initialize(Action<Guid, string> OutputHandler, Action<Guid, CompileMessage> CompileMessageHandler)
		{
			NewOutput = OutputHandler;
			NewMessage = CompileMessageHandler;
			IsInitialized = true;
		}

		[System.Reflection.Obfuscation(Feature = "encryptmethod", Exclude = false, StripAfterObfuscation = true)]
		public void Build(Project Data, bool ClientOnly = false)
		{
			HighestSeverity = CompileMessageSeverity.INFO;
			Messages.Clear();
			NewOutput(Data.ID, Globals.ApplicationTitle);
			NewOutput(Data.ID, string.Format("Version: {0}", Globals.ApplicationVersion));
			NewOutput(Data.ID, "Copyright © 2012-2013 Prospective Software Inc.");

			Verify(Data);

			//If the verification produced errors exit with an error code, we cannot proceed.
			if (HighestSeverity == CompileMessageSeverity.ERROR)
				return;

			if (!ClientOnly)
				foreach (ProjectGenerationTarget t in Data.ServerGenerationTargets)
				{
					string op = new Uri(new Uri(Data.AbsolutePath), t.Path).LocalPath;
					op = Uri.UnescapeDataString(op);
					NewOutput(Data.ID, string.Format("Writing Server Output: {0}", op));
					System.IO.File.WriteAllText(op, GenerateServer(Data, t.Framework, t.GenerateReferences));
				}

			foreach (ProjectGenerationTarget t in Data.ClientGenerationTargets.Where(a => a.Framework != ProjectGenerationFramework.WIN8))
			{
				string op = new Uri(new Uri(Data.AbsolutePath), t.Path).LocalPath;
				op = Uri.UnescapeDataString(op);
				NewOutput(Data.ID, string.Format("Writing Client Output: {0}", op));
				System.IO.File.WriteAllText(op, GenerateClient(Data, t.Framework, t.GenerateReferences));
			}
		}

		public Task BuildAsync(Project Data, bool ClientOnly = false)
		{
			return System.Windows.Application.Current == null ? null : Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(() => Build(Data, ClientOnly), DispatcherPriority.Normal));
		}

		[System.Reflection.Obfuscation(Feature = "encryptmethod", Exclude = false, StripAfterObfuscation = true)]
		public void Verify(Project Data)
		{
			if (string.IsNullOrEmpty(Data.ServerOutputFile))
				AddMessage(new CompileMessage("GS0003", "The '" + Data.Name + "' project does not have a Server Assembly Name. You must specify a Server Assembly Name.", CompileMessageSeverity.ERROR, null, Data, Data.GetType(), Data.ID));
			else
				if (RegExs.MatchFileName.IsMatch(Data.ServerOutputFile) == false)
					AddMessage(new CompileMessage("GS0004", "The Server Assembly Name in '" + Data.Name + "' project is not set or contains invalid characters. You must specify a valid Windows file name.", CompileMessageSeverity.ERROR, null, Data, Data.GetType(), Data.ID));
			if (string.IsNullOrEmpty(Data.ClientOutputFile))
				AddMessage(new CompileMessage("GS0005", "The '" + Data.Name + "' project does not have a Client Assembly Name. You must specify a Client Assembly Name.", CompileMessageSeverity.ERROR, null, Data, Data.GetType(), Data.ID));
			else
				if (RegExs.MatchFileName.IsMatch(Data.ClientOutputFile) == false)
					AddMessage(new CompileMessage("GS0006", "The Client Assembly Name in '" + Data.Name + "' project is not set or contains invalid characters. You must specify a valid Windows file name.", CompileMessageSeverity.ERROR, null, Data, Data.GetType(), Data.ID));
			if ((Data.ServerOutputFile == Data.ClientOutputFile))
				AddMessage(new CompileMessage("GS0007", "The '" + Data.Name + "' project Client and Server Assembly Names are the same. You must specify a different Server or Client Assembly Name.", CompileMessageSeverity.ERROR, null, Data, Data.GetType(), Data.ID));

			NamespaceGenerator.VerifyCode(Data.Namespace, AddMessage);
		}

		public Task VerifyAsync(Project Data)
		{
			return System.Windows.Application.Current == null ? null : Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(() => Verify(Data), DispatcherPriority.Normal));
		}

		[System.Reflection.Obfuscation(Feature = "encryptmethod", Exclude = false, StripAfterObfuscation = true)]
		public string GenerateServer(Project Data, ProjectGenerationFramework Framework, bool GenerateReferences)
		{
			return Generate(Data, Framework, true, GenerateReferences);
		}

		[System.Reflection.Obfuscation(Feature = "encryptmethod", Exclude = false, StripAfterObfuscation = true)]
		public string GenerateClient(Project Data, ProjectGenerationFramework Framework, bool GenerateReferences)
		{
			return Generate(Data, Framework, false, GenerateReferences);
		}

		public Task<string> GenerateServerAsync(Project Data, ProjectGenerationFramework Framework, bool GenerateReferences)
		{
			return System.Windows.Application.Current == null ? null : Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(() => GenerateServer(Data, Framework, GenerateReferences), DispatcherPriority.Normal));
		}

		public Task<string> GenerateClientAsync(Project Data, ProjectGenerationFramework Framework, bool GenerateReferences)
		{
			return System.Windows.Application.Current == null ? null : Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(() => GenerateClient(Data, Framework, GenerateReferences), DispatcherPriority.Normal));
		}

		private string Generate(Project Data, ProjectGenerationFramework Framework, bool Server, bool GenerateReferences)
		{
			Globals.CurrentProjectID = Data.ID;

			var code = new StringBuilder();
			code.AppendLine("//---------------------------------------------------------------------------");
			code.AppendLine("// This code was generated by a tool. Changes to this file may cause ");
			code.AppendLine("// incorrect behavior and will be lost if the code is regenerated.");
			code.AppendLine("//");
			code.AppendLine(string.Format("// NETPath Version:\t{0}", Globals.ApplicationVersion));
			if (Framework == ProjectGenerationFramework.NET30) code.AppendLine("// .NET Framework Version:\t3.0");
			if (Framework == ProjectGenerationFramework.NET35) code.AppendLine("// .NET Framework Version:\t3.5");
			if (Framework == ProjectGenerationFramework.NET35Client) code.AppendLine("// .NET Framework Version:\t3.5 (Client)");
			if (Framework == ProjectGenerationFramework.NET40) code.AppendLine("// .NET Framework Version:\t4.0");
			if (Framework == ProjectGenerationFramework.NET40Client) code.AppendLine("// .NET Framework Version:\t4.0 (Client)");
			if (Framework == ProjectGenerationFramework.NET45) code.AppendLine("// .NET Framework Version:\t4.5");
			if (Framework == ProjectGenerationFramework.SL40) code.AppendLine("// Silverlight Version:\t4.0");
			if (Framework == ProjectGenerationFramework.SL50) code.AppendLine("// Silverlight Version:\t5.0");
			code.AppendLine("//---------------------------------------------------------------------------");
			code.AppendLine();

			// Generate using namespaces
			foreach (ProjectUsingNamespace pun in GetUsingNamespaces(Data))
			{
				if (pun.Server && Server)
				{
					if (pun.NET && (Framework == ProjectGenerationFramework.NET30 || Framework == ProjectGenerationFramework.NET35 || Framework == ProjectGenerationFramework.NET40 || Framework == ProjectGenerationFramework.NET45))
						code.AppendFormat("using {0};{1}", pun.Namespace, Environment.NewLine);
					else if (pun.NET && (Framework == ProjectGenerationFramework.NET35Client || Framework == ProjectGenerationFramework.NET40Client))
						if (!pun.IsFullFrameworkOnly) code.AppendFormat("using {0};{1}", pun.Namespace, Environment.NewLine);
				}
				if (!pun.Client || Server) continue;
				if (pun.NET && (Framework == ProjectGenerationFramework.NET30 || Framework == ProjectGenerationFramework.NET35 || Framework == ProjectGenerationFramework.NET40 || Framework == ProjectGenerationFramework.NET45))
					code.AppendFormat("using {0};{1}", pun.Namespace, Environment.NewLine);
				else if (pun.NET && (Framework == ProjectGenerationFramework.NET35Client || Framework == ProjectGenerationFramework.NET40Client))
					if (!pun.IsFullFrameworkOnly) code.AppendFormat("using {0};{1}", pun.Namespace, Environment.NewLine);
				if (pun.SL && (Framework == ProjectGenerationFramework.SL40 || Framework == ProjectGenerationFramework.SL50))
					code.AppendFormat("using {0};{1}", pun.Namespace, Environment.NewLine);
			}
			if (Data.EnableEntityFramework && Server) code.AppendLine("using System.Data.Entity.Core.Objects;");
			code.AppendLine();

			//Generate ContractNamespace Attributes
			if (!Server) code.AppendLine(NamespaceGenerator.GenerateContractNamespaceAttributes(Data.Namespace));

			//Disable XML documentation warnings 
			if (!Data.EnableDocumentationWarnings) code.AppendLine("#pragma warning disable 1591");

			//Generate project
			if (Server)
			{
				//if (Framework == ProjectGenerationFramework.NET30) code.AppendLine(NamespaceGenerator.GenerateServerCode30(Data.Namespace));
				//if (Framework == ProjectGenerationFramework.NET35) code.AppendLine(NamespaceGenerator.GenerateServerCode35(Data.Namespace));
				//if (Framework == ProjectGenerationFramework.NET35Client) code.AppendLine(NamespaceGenerator.GenerateServerCode35Client(Data.Namespace));
				//if (Framework == ProjectGenerationFramework.NET40) code.AppendLine(NamespaceGenerator.GenerateServerCode40(Data.Namespace));
				//if (Framework == ProjectGenerationFramework.NET40Client) code.AppendLine(NamespaceGenerator.GenerateServerCode40Client(Data.Namespace));
				if (Framework == ProjectGenerationFramework.NET45) code.AppendLine(NamespaceGenerator.GenerateServerCode45(Data.Namespace));
				if (Framework == ProjectGenerationFramework.WIN8) code.AppendLine(NamespaceGenerator.GenerateServerCode45(Data.Namespace));
			}
			else
			{
				//if (Framework == ProjectGenerationFramework.NET30) code.AppendLine(NamespaceGenerator.GenerateClientCode30(Data.Namespace));
				//if (Framework == ProjectGenerationFramework.NET35) code.AppendLine(NamespaceGenerator.GenerateClientCode35(Data.Namespace));
				//if (Framework == ProjectGenerationFramework.NET35Client) code.AppendLine(NamespaceGenerator.GenerateClientCode35Client(Data.Namespace));
				//if (Framework == ProjectGenerationFramework.NET40) code.AppendLine(NamespaceGenerator.GenerateClientCode40(Data.Namespace));
				//if (Framework == ProjectGenerationFramework.NET40Client) code.AppendLine(NamespaceGenerator.GenerateClientCode40Client(Data.Namespace));
				if (Framework == ProjectGenerationFramework.NET45) code.AppendLine(NamespaceGenerator.GenerateClientCode45(Data.Namespace));
				if (Framework == ProjectGenerationFramework.WIN8) code.AppendLine(NamespaceGenerator.GenerateClientCodeRT8(Data.Namespace));
			}

			//Reenable XML documentation warnings
			if (!Data.EnableDocumentationWarnings) code.AppendLine("#pragma warning restore 1591");

			return code.ToString();
		}

		private void AddMessage(CompileMessage Message)
		{
			Messages.Add(Message);
			if (Message.Severity == CompileMessageSeverity.ERROR && HighestSeverity != CompileMessageSeverity.ERROR) HighestSeverity = CompileMessageSeverity.ERROR;
			if (Message.Severity == CompileMessageSeverity.WARN && HighestSeverity == CompileMessageSeverity.INFO) HighestSeverity = CompileMessageSeverity.WARN;
			NewOutput(Message.ProjectID, string.Format("{0} {1}: {2} Object: {3} Owner: {4}", Message.Severity, Message.Code, Message.Description, Message.ErrorObject, Message.Owner));
			NewMessage(Message.ProjectID, Message);
		}

		private static IEnumerable<ProjectUsingNamespace> GetUsingNamespaces(Project CurProject)
		{
			return new List<ProjectUsingNamespace>(CurProject.UsingNamespaces);
		}
	}

	internal static class Globals
	{
		public static readonly Version ApplicationVersion;
		public const string ApplicationTitle = "NETPath C# Generator";

		public static Guid CurrentProjectID { get; set; }

		static Globals()
		{
			string asmfp = System.IO.Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetCallingAssembly().CodeBase).LocalPath);
			if (asmfp == null) return;
			ApplicationVersion = new Version(System.Diagnostics.FileVersionInfo.GetVersionInfo(System.IO.Path.Combine(asmfp, "NETPath.Generators.CS.dll")).FileVersion);
		}
	}
}