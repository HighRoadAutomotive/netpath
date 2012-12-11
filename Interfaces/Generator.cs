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
	public enum GenerationLanguage
	{
		CSharp,
		VisualBasic,
		CPlusPlus,
	}

	public enum GenerationModule
	{
		NET,
		Silverlight,
		WindowsRuntime,
	}
	
	public static class Loader
	{
		public static readonly ConcurrentDictionary<string, IGenerator> LoadedModules = new ConcurrentDictionary<string, IGenerator>();

		public static IGenerator LoadModule(GenerationModule Module, GenerationLanguage Language)
		{
			//Build the module path and and determine if it is available on this system.
			string asmfp = System.IO.Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetCallingAssembly().CodeBase).LocalPath);
			if (asmfp == null) return null;
			string mod = "NET";
			if (Module == GenerationModule.Silverlight) mod = "SL";
			if (Module == GenerationModule.WindowsRuntime) mod = "WinRT";
			string lang = "CS";
			if (Language == GenerationLanguage.VisualBasic) lang = "VB";
			if (Language == GenerationLanguage.CPlusPlus) lang = "CPP";
			string modpath = System.IO.Path.Combine(asmfp, string.Format("NETPath.Generators.{0}.{1}.dll", mod, lang));
			if (!System.IO.File.Exists(modpath)) return null;
			//Load and initialize the module
			System.Reflection.Assembly moduleDLL = System.Reflection.Assembly.LoadFile(modpath);
			var cm = moduleDLL.CreateInstance(string.Format("NETPath.Generators.{0}.{1}.Generator", mod, lang)) as IGenerator;
			if (cm == null) return null;
			LoadedModules.TryAdd(cm.Name, cm);
			return cm;
		}

		public static Task<IGenerator> LoadModuleAsync(GenerationModule Module, GenerationLanguage Language)
		{
			return Task.Run<IGenerator>(() => LoadModule(Module, Language));
		}
	}

	public interface IGenerator
	{
		void Initialize(string License, Action<Guid, string> OutputHandler, Action<Guid, CompileMessage> CompileMessageHandler);
		void Verify(Project Data);
		Task VerifyAsync(Project Data);
		string GenerateServer(Project Data, ProjectGenerationFramework Framework, bool GenerateReferences);
		string GenerateClient(Project Data, ProjectGenerationFramework Framework, bool GenerateReferences);
		Task<string> GenerateServerAsync(Project Data, ProjectGenerationFramework Framework, bool GenerateReferences);
		Task<string> GenerateClientAsync(Project Data, ProjectGenerationFramework Framework, bool GenerateReferences);
		void Build(Project Data, bool ClientOnly = false);
		Task BuildAsync(Project Data, bool ClientOnly = false);

		Action<Guid, string> NewOutput { get; }
		Action<Guid, CompileMessage> NewMessage { get; }
		ObservableCollection<CompileMessage> Messages { get; }
		CompileMessageSeverity HighestSeverity { get; }
		string Name { get; }
		GenerationLanguage Language { get; }
		GenerationModule Module { get; }
		bool IsInitialized { get; }
	}
}