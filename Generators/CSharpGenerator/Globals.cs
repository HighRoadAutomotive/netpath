
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETPath.Generators.CS
{
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
