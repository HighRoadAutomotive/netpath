using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFArchitect.Projects;

namespace WCFArchitect.Compiler
{
	internal static class Globals
	{
		public static Project OpenProject { get; set; }

		public static Projects.ProjectGenerationFramework CurrentGenerationTarget { get; set; }

		public static string ApplicationTitle { get; set; }
		public static Version ApplicationVersion { get; set; }
	}
}