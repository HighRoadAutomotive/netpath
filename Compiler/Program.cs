using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WCFArchitect.Projects;

namespace WCFArchitect.Compiler
{
	public class Program
	{

		public static string ProjectPath { get; private set; }
		public static Dictionary<string, ProjectGenerationFramework> OverrideServerOutput { get; private set; }
		public static Dictionary<string, ProjectGenerationFramework> OverrideClientOutput { get; private set; }
		public static StreamWriter LogFile { get; private set; }
		public static Stream ErrorStream { get; private set; }
		public static bool Quiet { get; private set; }

		public static void Main(string[] args)
		{
			string AppPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
			Globals.ApplicationVersion = new Version(System.Diagnostics.FileVersionInfo.GetVersionInfo(AppPath + "wasc.exe").FileVersion);
			Globals.ApplicationTitle = "WCF Architect Service Compiler";

			if(args.Contains("-?"))
				PrintHelp();

			ParseOptions(new List<string>(args));

			PrintHeader();

			ErrorStream = Console.OpenStandardError();
	
		}

		internal static void AddMessage(CompileMessage Message)
		{
			string mstr = string.Format("{0} {1}: {2} Object: {3} Owner: {4}", Message.Severity.ToString(), Message.Code, Message.Description, Message.ErrorObject, Message.Owner);
			if(LogFile != null)
				LogFile.WriteLine(mstr);
			Console.WriteLine(mstr);

			var dcs = new DataContractSerializer(typeof(CompileMessage), new DataContractSerializerSettings() { MaxItemsInObjectGraph = Int32.MaxValue, IgnoreExtensionDataObject = true, SerializeReadOnlyTypes = true, PreserveObjectReferences = true });
			var xw = XmlWriter.Create(ErrorStream, new XmlWriterSettings() { Encoding = Encoding.UTF8, NewLineChars = Environment.NewLine, NewLineHandling = NewLineHandling.Entitize, CloseOutput = true, WriteEndDocumentOnClose = true, Indent = true, Async = false });
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
			Console.WriteLine("\tTargets: NET30|NET35|NET35Client|NET40|NET40Client|NET45|WINRT8");
			Console.WriteLine("\tCan be specified multiple times. Usage: -oc <target> \"<directory>\".");
			Console.WriteLine();
			Console.WriteLine("-log\tSave the output to a log file.");
			Console.WriteLine("\tUsage: -log \"<directory>\".");
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

			if(LogFile != null)
			{
				LogFile.WriteLine(Globals.ApplicationTitle);
				LogFile.WriteLine("Version: {0}", Globals.ApplicationVersion);
				LogFile.WriteLine("Copyright © 2012-2013 Prospective Software Inc.");
				LogFile.WriteLine();
			}
		}

		public static void ParseOptions(List<string> args)
		{
			ProjectPath = args[0];
			OverrideServerOutput = new Dictionary<string, ProjectGenerationFramework>();
			OverrideClientOutput = new Dictionary<string, ProjectGenerationFramework>();

			if(!File.Exists(ProjectPath))
			{
				Console.WriteLine("Unable to locate project file: " + ProjectPath);
				Environment.Exit(1);
			}

			for(int i=1;i<args.Count;i++)
			{
				if(args[i]=="-q")
					Quiet = true;
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
					string op = args[++i];
					if(!System.IO.Directory.Exists(op))
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