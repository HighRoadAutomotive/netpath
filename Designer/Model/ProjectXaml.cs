using RestForge.Solutions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RestForge.Projects;

namespace RestForge.Designer.Model
{
	public class ProjectXaml : DependencyObject
	{
		public Project SolutionProject { get; }
		public IProject Project { get; }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof (string), typeof (ProjectXaml), new PropertyMetadata(""));

		public ProjectXaml(IProject project, Project solutionProject)
		{
			Project = project;
			SolutionProject = solutionProject;
			Name = Project.Name;
		}

		public ProjectXaml(IProject project, string path, Guid handler)
		{
			Project = project;
			Name = Project.Name;

			SolutionProject = new Project(project.ID, handler, path);
		}
	}
}
