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
		CPPCX,
		FSharp,
		DLang,
	}

	public enum GenerationModule
	{
		NET,
		WindowsRuntime,
	}

	public enum GenerationType {
		Wcf,
		WebApi,
		VibeD
	}

	public static class Loader
	{
		public static readonly ConcurrentDictionary<string, IGenerator> LoadedModules = new ConcurrentDictionary<string, IGenerator>();

		public static IGenerator LoadModule(GenerationLanguage Language, GenerationType ProjectType)
		{
			//Build the module path and and determine if it is available on this system.
			string asmfp = System.IO.Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetCallingAssembly().CodeBase).LocalPath);
			if (asmfp == null) return null;
			string lang = "CS";
			if (Language == GenerationLanguage.VisualBasic) lang = "VB";
			if (Language == GenerationLanguage.CPPCX) lang = "CPPCX";
			if (Language == GenerationLanguage.FSharp) lang = "FS";
			string modpath = System.IO.Path.Combine(asmfp, string.Format("NETPath.Generators.{0}.dll", lang));
			if (!System.IO.File.Exists(modpath)) return null;
			//Load and initialize the module
			System.Reflection.Assembly moduleDLL = System.Reflection.Assembly.LoadFile(modpath);
			var cm = moduleDLL.CreateInstance(string.Format("NETPath.Generators.{0}.{1}.Generator", lang, ProjectType)) as IGenerator;
			if (cm == null) return null;
			LoadedModules.TryAdd(cm.Name, cm);
			return cm;
		}

		public static Task<IGenerator> LoadModuleAsync(GenerationLanguage Language, GenerationType ProjectType)
		{
			return Task.Run(() => LoadModule(Language, ProjectType));
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
		GenerationLanguage Language { get; }
		GenerationModule Module { get; }
		bool IsInitialized { get; }
	}
}