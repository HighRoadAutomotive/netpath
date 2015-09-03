using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETPath.Projects;

namespace NETPath.Generators.Interfaces
{
	public static class GeneratorLoader
	{
		public static readonly ConcurrentDictionary<string, IGenerator> LoadedModules = new ConcurrentDictionary<string, IGenerator>();

		public static void LoadModules()
		{
			//Build the module path and and determine if it is available on this system.
			string asmfp = System.IO.Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetCallingAssembly().CodeBase).LocalPath);
			if (asmfp == null) return;

			var gl = System.IO.Directory.EnumerateFiles(asmfp, "NETPath.Generators.*.dll");

			foreach (var g in gl)
			{
				var moduleDLL = System.Reflection.Assembly.LoadFile(g);

				var instances = from t in moduleDLL.GetExportedTypes()
								where t.GetInterfaces().Contains(typeof(IGenerator)) && t.GetConstructor(Type.EmptyTypes) != null
								select Activator.CreateInstance(t) as IGenerator;

				foreach (var instance in instances)
				{
					LoadedModules.TryAdd(instance.Name, instance);
				}
			}
		}

		public static Task LoadModulesAsync()
		{
			return Task.Run(() => LoadModules());
		}
	}

	public interface IGenerator
	{
		void Initialize(Project Data, string ProjectPath, Action<string> OutputHandler, Action<CompileMessage> CompileMessageHandler);
		void Verify();
		Task VerifyAsync();
		void Build(bool ClientOnly = false);
		Task BuildAsync(bool ClientOnly = false);

		Action<string> NewOutput { get; }
		Action<CompileMessage> NewMessage { get; }
		ObservableCollection<CompileMessage> Messages { get; }
		CompileMessageSeverity HighestSeverity { get; }
		string Name { get; }
		string Language { get; }
		string Type { get; }
		bool IsInitialized { get; }
	}
}