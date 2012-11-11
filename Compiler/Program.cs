using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using LogicNP.CryptoLicensing;
using WCFArchitect.Projects;
using WCFArchitect.Generators.Interfaces;

namespace WCFArchitect.Compiler
{
	public static class Program
	{
		public static string SolutionPath { get; private set; }
		public static string ProjectPath { get; private set; }
		public static Project OpenProject { get; set; }
		public static Dictionary<string, ProjectGenerationFramework> OverrideServerOutput { get; private set; }
		public static Dictionary<string, ProjectGenerationFramework> OverrideClientOutput { get; private set; }
		public static StreamWriter LogFile { get; private set; }
		public static Stream ErrorStream { get; private set; }
		public static bool StdError { get; private set; }
		public static bool Quiet { get; private set; }
		public static bool Experimental { get; private set; }

		private static List<CompileMessage> Messages { get; set; }
		public static CompileMessageSeverity HighestSeverity { get; private set; }

		public static void Main(string[] args)
		{
			string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
			Globals.ApplicationVersion = new Version(System.Diagnostics.FileVersionInfo.GetVersionInfo(appPath + "wasc.exe").FileVersion);
			Globals.ApplicationTitle = "WCF Architect Service Compiler - BETA";

			if (args.GetLongLength(0) == 0) Environment.Exit(0);

			if(args.Contains("-?"))
				PrintHelp();

			ParseOptions(new List<string>(args));

			PrintHeader();

			const string vk = "AMAAMACnZigmLe9LpWcsYIBVFHYRZeUhr1oYyxDRFmL/qon4ijMx6X/xXyYldZs/A8Df9MsDAAEAAQ==";
			var t = new CryptoLicense("NgAAAR34yUVouM0B89nN/6JGzgGHo04qSoBnf8S47pP6T/Awg2aOLNXVHFlxYaTAmetprPIDC9YxTuDJsAf3Er3NdiI=", vk);
			if (t.Status != LicenseStatus.Valid)
			{
				Console.WriteLine("This copy of WCF Architect is BETA software and expired on {0}.", t.DateExpires.ToShortDateString());
				Console.WriteLine("Please visit us at http://www.prospectivesoftware.com to purchase a license if you wish to continue to use this software.");
				Environment.Exit(4);
			}
			else
			{
				Console.WriteLine("This copy of WCF Architect is BETA software and will expire on {0}.", t.DateExpires.ToShortDateString());
			}

			ErrorStream = Console.OpenStandardError();

			OpenProject = Project.Open(SolutionPath, ProjectPath);

			Experimental = OpenProject.EnableExperimental;

			IGenerator NET = Loader.LoadModule(GenerationModule.NET, GenerationLanguage.CSharp);
			NET.Initialize("NgAAAR34yUVouM0B89nN/6JGzgGHo04qSoBnf8S47pP6T/Awg2aOLNXVHFlxYaTAmetprPIDC9YxTuDJsAf3Er3NdiI=", OutputHandler, AddMessage);
			IGenerator WinRT = Loader.LoadModule(GenerationModule.WindowsRuntime, GenerationLanguage.CSharp);
			WinRT.Initialize("NgAAAR34yUVouM0B89nN/6JGzgGHo04qSoBnf8S47pP6T/Awg2aOLNXVHFlxYaTAmetprPIDC9YxTuDJsAf3Er3NdiI=", OutputHandler, AddMessage);

			//Run project code generation
			if (NET.IsInitialized && WinRT.IsInitialized)
			{
				NET.Build(OpenProject);
				if(OpenProject.ClientGenerationTargets.Any(a => a.Framework == ProjectGenerationFramework.WIN8)) WinRT.Build(OpenProject, true);
			}
			else if (WinRT.IsInitialized)
				WinRT.Build(OpenProject);
			else
			{
				Console.WriteLine("FATAL ERROR: Unable to initialize any code generators.");
				Environment.Exit(4);
			}

			//If the code generation produced any errors we need to exit with an error code.
			if (NET.HighestSeverity == CompileMessageSeverity.ERROR || WinRT.HighestSeverity == CompileMessageSeverity.ERROR)
				Environment.Exit(3);

			//Everything completed successfully
			Environment.ExitCode = 0;
		}

		//TODO: Delete these when the generators are removed.
		private static void OutputHandler(string S) {}
		internal static void AddMessage(CompileMessage Message) {}

		private static void OutputHandler(Guid ID, string S)
		{
			Console.WriteLine(S);
		}

		internal static void AddMessage(Guid ID, CompileMessage Message)
		{
			Messages.Add(Message);
			if (Message.Severity == CompileMessageSeverity.ERROR && HighestSeverity != CompileMessageSeverity.ERROR) HighestSeverity = CompileMessageSeverity.ERROR;
			if (Message.Severity == CompileMessageSeverity.WARN && HighestSeverity == CompileMessageSeverity.INFO) HighestSeverity = CompileMessageSeverity.WARN;

			//Write message to the log file and console
			string mstr = string.Format("{0} {1}: {2} Object: {3} Owner: {4}", Message.Severity, Message.Code, Message.Description, Message.ErrorObject, Message.Owner);
			if(LogFile != null)
				LogFile.WriteLine(mstr);
			Console.WriteLine(mstr);

			//Write a serialized version of the message to the error stream
			if (!StdError) return;
			var dcs = new DataContractSerializer(typeof(CompileMessage), new DataContractSerializerSettings { MaxItemsInObjectGraph = Int32.MaxValue, IgnoreExtensionDataObject = true, SerializeReadOnlyTypes = true, PreserveObjectReferences = true });
			var xw = XmlWriter.Create(ErrorStream, new XmlWriterSettings { Encoding = Encoding.UTF8, NewLineChars = Environment.NewLine, NewLineHandling = NewLineHandling.Entitize, CloseOutput = true, WriteEndDocumentOnClose = true, Indent = true, Async = false });
			dcs.WriteObject(xw, Message);
			xw.Flush();
		}

		public static void PrintHelp()
		{
			Quiet = false;
			PrintHeader();

			Console.WriteLine("Usage: wasc <solution file> <project file> [options]");
			Console.WriteLine();
			Console.WriteLine("-log\tSave the output to a log file. Usage: -log \"<directory>\".");
			Console.WriteLine();
			Console.WriteLine("-stderr\tEnables stderr output.");
			Console.WriteLine();
			Console.WriteLine("-q\tSupresses informational output.");
			Console.WriteLine();
			Console.WriteLine("-?\tDisplay this message. Overrides all other options.");

			Environment.Exit(0);
		}

		//Print the program header.
		public static void PrintHeader()
		{
			if (Quiet) return;
			Console.WriteLine(Globals.ApplicationTitle);
			Console.WriteLine("Version: {0}", Globals.ApplicationVersion);
			Console.WriteLine("Copyright © 2012-2013 Prospective Software Inc.");
			Console.WriteLine();

			if (LogFile == null) return;
			LogFile.WriteLine(Globals.ApplicationTitle);
			LogFile.WriteLine("Version: {0}", Globals.ApplicationVersion);
			LogFile.WriteLine("Copyright © 2012-2013 Prospective Software Inc.");
			LogFile.WriteLine();
		}

		public static void ParseOptions(List<string> args)
		{
			SolutionPath = args[0];
			ProjectPath = args[1];
			OverrideServerOutput = new Dictionary<string, ProjectGenerationFramework>();
			OverrideClientOutput = new Dictionary<string, ProjectGenerationFramework>();
			Messages = new List<CompileMessage>();
			HighestSeverity = CompileMessageSeverity.INFO;

			if (!File.Exists(SolutionPath))
			{
				Console.WriteLine("Unable to locate solution file: " + SolutionPath);
				Environment.Exit(1);
			}

			if (!File.Exists(ProjectPath))
			{
				Console.WriteLine("Unable to locate project file: " + ProjectPath);
				Environment.Exit(1);
			}

			for(int i=2;i<args.Count;i++)
			{
				if (args[i] == "-q")
					Quiet = true;
				if (args[i] == "-stderr")
					StdError = true;
				if (args[i] == "-log")
					LogFile = File.CreateText(args[i++]);
				if (args[i] == "-os")
				{
					ProjectGenerationFramework pgf;
					if(!System.Enum.TryParse(args[++i], true, out pgf))
					{
						Console.WriteLine("Unrecognized Target: " + args[i]);
						continue;
					}
					string op = args[++i].Replace("\"", "");
					if(!Directory.Exists(op))
					{
						Console.WriteLine("Unable to locate directory: " + op);
						continue;
					}
					OverrideServerOutput.Add(op, pgf);
				}
				if (args[i] == "-oc")
				{
					ProjectGenerationFramework pgf;
					if (!System.Enum.TryParse(args[++i], true, out pgf))
					{
						Console.WriteLine("Unrecognized Target: " + args[i]);
						continue;
					}
					string op = args[++i];
					if (!Directory.Exists(op))
					{
						Console.WriteLine("Unable to locate directory: " + op);
						continue;
					}
					OverrideClientOutput.Add(op, pgf);
				}
			}
		}
	}
}