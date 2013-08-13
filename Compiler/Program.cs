using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using LogicNP.CryptoLicensing;
using NETPath.Options;
using NETPath.Projects;
using NETPath.Generators.Interfaces;

namespace NETPath.Compiler
{
	public static class Program
	{
		public static string SolutionPath { get; private set; }
		public static string ProjectPath { get; private set; }
		public static Project OpenProject { get; set; }
		public static StreamWriter LogFile { get; private set; }
		public static Stream ErrorStream { get; private set; }
		public static bool StdError { get; private set; }
		public static bool Quiet { get; private set; }
		public static string ApplicationTitle { get; set; }
		public static Version ApplicationVersion { get; set; }
		public static UserProfile UserProfile { get; set; }
		private const string TrialLicense = "lgKkARmvMXTEec4BHgAvAFNLVT1OUDIwVFJJQUwjTGljZW5zZWVOYW1lPVByb3NwZWN0aXZlIFNvZnR3YXJlAQNEZ9ul5WCNDyXclcMH0Y/O5xklUKqd0W1M/lZjqHL9jcp7tt741X38fW4nYB7Xb88vVO6ks+KNQ7RvKoyjVI1j86CkkQXWvGkgpq6STx0ORAfhmEnvn0+AyfKWAuMn6ZQCzFYoZ46V9SwA+FQz2+vpM2+DP6Ik1QfvHnrirxDLEsndJzuzSGB+3MFEZd+0k2BtA9yUSa7CRa+6QAfHQjZf4FWxVMrJQ7hcSEvN8fLd9oCLozBCVYEtpNPPEyLNB1Kfq2nBJKkO+36gTYR1CvqW+UXvMa0jtDYe1+5la89Kiis7tvH4vMiFwSxKdpy1krparGtHBT7xQT6xGhwzUo7p";
		private const string LicenseVerification = "AAAEAAG6rTV/gUg+VZjvEZQDqWy9l63DgzkUSg0tyJOBDDS58FKoRvErRfUkvxdlgUCCTTvw5b7lXtVPFxd3HI+SFzzTi5X0neWXCNXjWX/FVnIaCBioKHG6eYwgSE86j2ybYQbGlmy+R9vpj3cA12E6a4efoQl/5yqawkUk67iQGnJi0YiA6LUAQUoCN+XipZN3pEn+EuAPGVAz1W0b8pYX99oSrWr3CQwnGCg6/2Y5radzYdPDsZgWkKkWhPU/ZGXcDo+GB4e35OaO6hp8lcq3lmxc+3Ic9eDsVK1kHaccRI/hWcgmkp39/3/zk1mnVtgiED8RI0eUniUTWXTGVTtBvBGLAwABAAE=";

		private static List<CompileMessage> Messages { get; set; }
		public static CompileMessageSeverity HighestSeverity { get; private set; }

		[System.Reflection.Obfuscation(Feature = "encryptmethod", Exclude = false, StripAfterObfuscation = true)]
		public static void Main(string[] args)
		{
			string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
			ApplicationVersion = new Version(System.Diagnostics.FileVersionInfo.GetVersionInfo(appPath + "npsc.exe").FileVersion);
			ApplicationTitle = "NETPath Service Compiler";

			if (args.GetLongLength(0) == 0) Environment.Exit(0);

			if(args.Contains("-?"))
				PrintHelp();

			ParseOptions(new List<string>(args));

			PrintHeader();

			string lic = TrialLicense;
#if LICENSE
			//Check license key
			string UserProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Prospective Software\\NETPath\\";
			UserProfilePath = Path.Combine(UserProfilePath, "profile.dat");
			if (File.Exists(UserProfilePath))
				UserProfile = UserProfile.Open(UserProfilePath);
			if (!(UserProfile.IsTrial || UserProfile.Serial == "TRIAL" || UserProfile.License == ""))
				lic = UserProfile.License;
			var t = new CryptoLicense(lic, LicenseVerification);
			if (t.Status != LicenseStatus.Valid)
			{
				Console.WriteLine("This copy of NETPath is trial software and expired on {0}.", t.DateExpires.ToShortDateString());
				Console.WriteLine("Please visit us at http://www.prospectivesoftware.com to purchase a license if you wish to continue to use this software.");
				Environment.Exit(4);
			}
			else
			{
				Console.WriteLine("This copy of NETPath is trial software and will expire on {0}.", t.DateExpires.ToShortDateString());
			}
#endif

			ErrorStream = Console.OpenStandardError();

			OpenProject = Project.Open(Path.Combine(Directory.GetCurrentDirectory(), SolutionPath), ProjectPath);

			IGenerator NET = Loader.LoadModule(GenerationModule.NET, GenerationLanguage.CSharp);
			NET.Initialize(lic, OutputHandler, AddMessage);
			IGenerator WinRT = Loader.LoadModule(GenerationModule.WindowsRuntime, GenerationLanguage.CSharp);
			WinRT.Initialize(lic, OutputHandler, AddMessage);

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

			Console.WriteLine("Usage: npsc <solution file> <project file> [options]");
			Console.WriteLine();
			Console.WriteLine("<solution file>\tThe path to the solution file relative to the current working directory.");
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
			SolutionPath = args[0];
			ProjectPath = args[1];
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
			}
		}
	}
}