using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFArchitect.Compiler
{
	public class Program
	{

		public static bool Quiet { get; private set; }

		public static void Main(string[] args)
		{
			string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
			Globals.ApplicationVersion = new Version(System.Diagnostics.FileVersionInfo.GetVersionInfo(AppPath + "wasc.exe").FileVersion);
			Globals.ApplicationTitle = "WCF Architect Service Compiler";

			//Print the program header.
			if (Quiet != true)
			{
				Console.WriteLine(Globals.ApplicationTitle);
				Console.WriteLine("Version: {0}", Globals.ApplicationVersion.ToString());
				Console.WriteLine("Copyright © 2012 Prospective Software Inc.");
				Console.WriteLine();
			}

		}

		internal static void AddMessage(Compiler.CompileMessage Message)
		{
		}
	}
}