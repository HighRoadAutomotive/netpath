using RestForge.Projects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestForge.Modules
{
	public static class Loader
	{
		public static readonly ConcurrentDictionary<Guid, ModuleBase> Modules = new ConcurrentDictionary<Guid, ModuleBase>();

		private static void LoadModules()
		{
			//Build the module path and and determine if it is available on this system.
			string asmfp = System.IO.Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetCallingAssembly().CodeBase).LocalPath);
			if (asmfp == null) return;

			var gl = System.IO.Directory.EnumerateFiles(asmfp, "RestForge.Modules.*.dll");

			foreach (var g in gl)
			{
				var moduleDLL = System.Reflection.Assembly.LoadFile(g);

				var instances = from t in moduleDLL.GetExportedTypes()
								where t.GetInterfaces().Contains(typeof(ModuleBase)) && t.GetConstructor(Type.EmptyTypes) != null
								select Activator.CreateInstance(t) as ModuleBase;

				foreach (var instance in instances)
				{
					Modules.TryAdd(instance.ModuleID, instance);
				}
			}
		}

		public static Task LoadModulesAsync()
		{
			return Task.Run(() => LoadModules());
		}
	}

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
