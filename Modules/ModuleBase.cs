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

	public abstract class ModuleBase
	{
		public abstract Task<IProject> Load(string Path);

		public abstract Guid ModuleID { get; }
		public abstract string Name { get; }
		public abstract string Description { get; }

		public abstract Document OpenProjectDocument(IProject project);
		public abstract Document OpenNamespaceDocument(INamespace namespaces);
		public abstract Document OpenServiceDocument(IService service);
		public abstract Document OpenDataDocument(IData data);
		public abstract Document OpenEnumDocument(IEnum enumeration);
	}
}
