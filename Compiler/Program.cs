using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using LogicNP.CryptoLicensing;
using WCFArchitect.Projects;

namespace WCFArchitect.Compiler
{
	public static class Program
	{

		public static string ProjectPath { get; private set; }
		public static Project OpenProject { get; set; }
		public static Dictionary<string, ProjectGenerationFramework> OverrideServerOutput { get; private set; }
		public static Dictionary<string, ProjectGenerationFramework> OverrideClientOutput { get; private set; }
		public static StreamWriter LogFile { get; private set; }
		public static Stream ErrorStream { get; private set; }
		public static bool StdError { get; private set; }
		public static bool Quiet { get; private set; }

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

			OpenProject = Project.Open(ProjectPath);

			//Build generators for the server and client
			var serverGenerators = new List<Generator>(OverrideServerOutput.Count == 0 ? OpenProject.ServerGenerationTargets.Select(pgt => new Generator(OpenProject, pgt.Framework, pgt.Path, pgt.IsServerPath)) : OverrideServerOutput.Select(osc => new Generator(OpenProject, osc.Value, osc.Key)));
			var clientGenerators = new List<Generator>(OverrideClientOutput.Count == 0 ? OpenProject.ClientGenerationTargets.Select(pgt => new Generator(OpenProject, pgt.Framework, pgt.Path, pgt.IsServerPath)) : OverrideClientOutput.Select(osc => new Generator(OpenProject, osc.Value, osc.Key, false)));

			//Run project verification
			foreach(Generator g in serverGenerators)
				g.Verify();
			foreach (Generator g in clientGenerators)
				g.Verify();

			//If the verification produced errors exit with an error code, we cannot proceed.
			if (HighestSeverity == CompileMessageSeverity.ERROR)
				Environment.Exit(2);

			//Run project code generation
			foreach (Generator g in serverGenerators)
				g.Generate();
			foreach (Generator g in clientGenerators)
				g.Generate();

			//If the code generation produced any errors we need to exit with an error code.
			if (HighestSeverity == CompileMessageSeverity.ERROR)
				Environment.Exit(3);

			//Everything completed successfully
			Environment.ExitCode = 0;
		}

		internal static void AddMessage(CompileMessage Message)
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

			Console.WriteLine("Usage: wasc <project file> [options]");
			Console.WriteLine();
			Console.WriteLine("-os\tOverrides all server output targets and directories in the project.");
			Console.WriteLine("\tTargets: NET30|NET35|NET35Client|NET40|NET40Client|NET45");
			Console.WriteLine("\tCan be specified multiple times. Usage: -os <target> \"<directory>\".");
			Console.WriteLine();
			Console.WriteLine("-oc\tOverrides all client output targets and directories in the project.");
			Console.WriteLine("\tTargets: NET30|NET35|NET35Client|NET40|NET40Client|NET45|WIN8");
			Console.WriteLine("\tCan be specified multiple times. Usage: -oc <target> \"<directory>\".");
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
			ProjectPath = args[0];
			OverrideServerOutput = new Dictionary<string, ProjectGenerationFramework>();
			OverrideClientOutput = new Dictionary<string, ProjectGenerationFramework>();
			Messages = new List<CompileMessage>();
			HighestSeverity = CompileMessageSeverity.INFO;

			if(!File.Exists(ProjectPath))
			{
				Console.WriteLine("Unable to locate project file: " + ProjectPath);
				Environment.Exit(1);
			}

			for(int i=1;i<args.Count;i++)
			{
				if(args[i]=="-q")
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