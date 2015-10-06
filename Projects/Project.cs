using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace RestForge.Projects
{
	public interface IProject
	{
		Guid ID { get; }
		string Name { get; }
		string SolutionPath { get; }
		List<Guid> Dependencies { get; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class ProjectBase<N> where N : NamespaceBase<ProjectBase<N>, N>
	{
		[JsonProperty("id")]
		public Guid ID { get; private set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("solutionPath")]
		public string SolutionPath { get; set; }

		[JsonProperty("dependencies")]
		public List<Guid> Dependencies { get; private set; }

		[JsonProperty("root")]
		public N Root { get; private set; }

		protected ProjectBase()
		{
			ID = Guid.NewGuid();
			Dependencies = new List<Guid>();
		}

		public abstract Task Build();
		public abstract Task Save();
	}

	public interface INamespace
	{
		string Name { get; }
		bool Collapsed { get; }
		List<INamespace> IChildren { get; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class NamespaceBase<P, N> : INamespace where P : ProjectBase<N> where N : NamespaceBase<ProjectBase<N>, N>
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("collapsed")]
		public bool Collapsed { get; set; }

		[JsonProperty("project", ItemIsReference = true)]
		public P Project { get; private set; }

		[JsonProperty("parent", ItemIsReference = true)]
		public N Parent { get; private set; }

		[JsonProperty("children")]
		public List<N> Children { get; private set; }

		[JsonIgnore]
		List<INamespace> INamespace.IChildren { get { return (List<INamespace>)Children.AsEnumerable(); } }


		[JsonIgnore]
		public string FullNamespace { get { return GetFullNamespace(); } }

		protected NamespaceBase(P project, N parent)
		{
			Children = new List<N>();
			Project = project;
			Parent = parent;
		}

		protected abstract string GetFullNamespace();
	}
}
