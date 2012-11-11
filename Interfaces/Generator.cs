using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFArchitect.Projects;

namespace WCFArchitect.Generators.Interfaces
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

		public static Task<IGenerator> LoadModule(GenerationModule Module, GenerationLanguage Language)
		{
			return Task.Run<IGenerator>(() =>
			{
				//Build the module path and and determine if it is available on this system.
				string asmfp = System.Reflection.Assembly.GetCallingAssembly().Location;
				string mod = "NET";
				if (Module == GenerationModule.Silverlight) mod = "SL";
				if (Module == GenerationModule.WindowsRuntime) mod = "WinRT";
				string lang = "CS";
				if (Language == GenerationLanguage.VisualBasic) lang = "VB";
				if (Language == GenerationLanguage.CPlusPlus) lang = "CPP";
				string modpath = System.IO.Path.Combine(asmfp, string.Format("WCFArchitect.Generators.{0}.{1}.dll", mod, lang));
				if (!System.IO.File.Exists(modpath)) return null;
				//Load and initialize the module
				System.Reflection.Assembly moduleDLL = System.Reflection.Assembly.LoadFile(modpath);
				var cm = moduleDLL.CreateInstance(string.Format("WCFArchitect.Generators.{0}.{1}.Generator", mod, lang)) as IGenerator;
				if (cm == null) return null;
				LoadedModules.TryAdd(cm.Name, cm);
				return cm;
			});
		}
    }

	public interface IGenerator
	{
		void Initialize(string License, Action<CompileMessage> CompileMessageHandler, bool Experimental);
		void Verify(Project Data);
		string GenerateServer(Project Data, ProjectGenerationFramework Framework);
		string GenerateClient(Project Data, ProjectGenerationFramework Framework);

		Action<CompileMessage> NewMessage { get; }
		ObservableCollection<CompileMessage> Messages { get; }
		string Name { get; }
		GenerationLanguage Language { get; }
		GenerationModule Module { get; }
	}
}