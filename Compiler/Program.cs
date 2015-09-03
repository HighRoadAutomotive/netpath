using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using NETPath.Options;
using NETPath.Projects;
using NETPath.Generators.Interfaces;
using NETPath.Projects.WebApi;

namespace NETPath.Compiler
{
	public static class Program
	{
		public static string ProjectPath { get; private set; }
		public static Project OpenProject { get; set; }
		public static StreamWriter LogFile { get; private set; }
		public static Stream ErrorStream { get; private set; }
		public static bool StdError { get; private set; }
		public static bool Quiet { get; private set; }
		public static string ApplicationTitle { get; set; }
		public static Version ApplicationVersion { get; set; }
		public static UserProfile UserProfile { get; set; }

		private static List<CompileMessage> Messages { get; set; }
		public static CompileMessageSeverity HighestSeverity { get; private set; }

		[System.Reflection.Obfuscation(Feature = "encryptmethod", Exclude = false, StripAfterObfuscation = true)]
		public static void Main(string[] args)
		{
			string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
			ApplicationVersion = new Version(System.Diagnostics.FileVersionInfo.GetVersionInfo(appPath + "npsc.exe").FileVersion);
			ApplicationTitle = "NETPath Service Compiler";

			if (args.GetLongLength(0) == 0) Environment.Exit(0);

			if (args.Contains("-?"))
				PrintHelp();

			ParseOptions(new List<string>(args));

			PrintHeader();

			ErrorStream = Console.OpenStandardError();

			OpenProject = Project.Open(ProjectPath);

			string project = "Wcf";
			if (OpenProject.GetType() == typeof (WebApiProject)) project = "WebApi";
			GeneratorLoader.LoadModules();
			IGenerator CSharp = GeneratorLoader.LoadedModules.First(a => a.Value.Language == "CSharp" && string.Equals(a.Value.Type, project, StringComparison.InvariantCultureIgnoreCase)).Value;
			CSharp.Initialize(OpenProject, Path.Combine(Environment.CurrentDirectory, ProjectPath), OutputHandler, AddMessage);

			//Run project code generation
			if (CSharp.IsInitialized)
			{
				CSharp.Build();
			}
			else
			{
				Console.WriteLine("FATAL ERROR: Unable to initialize any code generators.");
				Environment.Exit(4);
			}

			//If the code generation produced any errors we need to exit with an error code.
			if (CSharp.HighestSeverity == CompileMessageSeverity.ERROR)
				Environment.Exit(3);

			//Everything completed successfully
			Environment.ExitCode = 0;
		}

		private static void OutputHandler(string S)
		{
			Console.WriteLine(S);
		}

		internal static void AddMessage(CompileMessage Message)
		{
			Messages.Add(Message);
			if (Message.Severity == CompileMessageSeverity.ERROR && HighestSeverity != CompileMessageSeverity.ERROR) HighestSeverity = CompileMessageSeverity.ERROR;
			if (Message.Severity == CompileMessageSeverity.WARN && HighestSeverity == CompileMessageSeverity.INFO) HighestSeverity = CompileMessageSeverity.WARN;

			//Write message to the log file and console
			string mstr = string.Format("{0} {1}: {2} Object: {3} Owner: {4}", Message.Severity, Message.Code, Message.Description, Message.ErrorObject, Message.Owner);
			if (LogFile != null)
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

			Console.WriteLine("Usage: npsc <project file> [options]");
			Console.WriteLine();
			Console.WriteLine("<project file>\tThe path to the project file relative to solution file.");
			Console.WriteLine("-log\tSave the output to a log file. Usage: -log \"<directory>\".");
			Console.WriteLine("-stderr\tEnables stderr output.");
			Console.WriteLine("-q\tSupresses informational output.");
			Console.WriteLine("-?\tDisplay this message. Overrides all other options.");

			Environment.Exit(0);
		}

		//Print the program header.
		public static void PrintHeader()
		{
			if (Quiet) return;
			Console.WriteLine(ApplicationTitle);
			Console.WriteLine("Version: {0}", ApplicationVersion);
			Console.WriteLine("Copyright: 2012-2013 Prospective Software Inc.");
			Console.WriteLine();

			if (LogFile == null) return;
			LogFile.WriteLine(ApplicationTitle);
			LogFile.WriteLine("Version: {0}", ApplicationVersion);
			LogFile.WriteLine("Copyright: 2012-2013 Prospective Software Inc.");
			LogFile.WriteLine();
		}

		public static void ParseOptions(List<string> args)
		{
			ProjectPath = args[0];
			Messages = new List<CompileMessage>();
			HighestSeverity = CompileMessageSeverity.INFO;

			if (!File.Exists(ProjectPath))
			{
				Console.WriteLine("Unable to locate project file: " + ProjectPath);
				Environment.Exit(1);
			}

			for (int i = 2; i < args.Count; i++)
			{
				if (args[i] == "-q")
					Quiet = true;
				if (args[i] == "-stderr")
					StdError = true;
				if (args[i] == "-log")
					LogFile = File.CreateText(args[i++]);
			}
		}
	}
}