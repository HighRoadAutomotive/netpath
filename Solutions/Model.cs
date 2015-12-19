using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestForge.Solutions
{
	[JsonObject(MemberSerialization.OptIn)]
	public sealed class Solution
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("solutionVersion")]
		public string SolutionVersion { get; internal set; }

		[JsonProperty("minimumVersion")]
		public string MinimumVersion { get; internal set; }

		[JsonProperty("subfolders")]
		public List<SolutionFolder> Folders { get; private set; }

		[JsonProperty("projects")]
		public List<Project> Projects { get; private set; }

		public Solution()
		{
			Folders = new List<SolutionFolder>();
			Projects = new List<Project>();
		}
	}

	[JsonObject(MemberSerialization.OptIn)]
	public sealed class SolutionFolder
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("collapsed")]
		public bool Collapsed { get; set; }

		[JsonProperty("subfolders")]
		public List<SolutionFolder> Folders { get; private set; }

		[JsonProperty("projects")]
		public List<Project> Projects { get; private set; }

		public SolutionFolder()
		{
			Collapsed = false;
			Folders = new List<SolutionFolder>();
			Projects = new List<Project>();
		}
	}

	[JsonObject(MemberSerialization.OptIn)]
	public sealed class Project
	{
		[JsonProperty("id")]
		public Guid ID { get; private set; }

		[JsonProperty("handler")]
		public Guid ModuleHandler { get; set; }

		[JsonProperty("path")]
		public string Path { get; set; }

		public Project(Guid ProjectID)
		{
			ID = ProjectID;
		}
	}
}
