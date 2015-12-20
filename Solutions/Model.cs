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
		public const int SolutionVersion = 1;
		public const int MinimumVersion = 1;

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("solutionVersion")]
		public int Version { get; private set; }

		[JsonProperty("minimumVersion")]
		public int MinVersion { get; private set; }

		[JsonProperty("subfolders")]
		public List<SolutionFolder> Folders { get; private set; }

		[JsonProperty("projects")]
		public List<Project> Projects { get; private set; }

		[JsonConstructor]
		public Solution()
		{
			Folders = new List<SolutionFolder>();
			Projects = new List<Project>();
		}

		public Solution(string name)
		{
			Name = name;
			Version = SolutionVersion;
			MinVersion = MinimumVersion;
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

		[JsonConstructor]
		public SolutionFolder(string name)
		{
			Name = name;
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

		[JsonConstructor]
		public Project(Guid id, Guid handler, string path)
		{
			ID = id;
			ModuleHandler = handler;
			Path = path;
		}
	}
}
