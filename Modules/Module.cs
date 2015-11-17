using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using RestForge.Projects;
using RestForge.Solutions;

namespace RestForge.Modules
{
	public abstract class Module
	{
		public string Name { get; protected set; }
		public string Description { get; protected set; }
		public PathGeometry Icon { get; protected set; }

		public abstract IProject NewProject(string name, string path, Solution solution, SolutionFolder folder = null);
	}
}
