using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using LogicNP.CryptoLicensing;
using WCFArchitect.Generators.Interfaces;
using WCFArchitect.Projects;
using WCFArchitect.Projects.Helpers;

namespace WCFArchitect.Generators.NET.CS
{
    public class Generator : IGenerator
    {
		public Action<Guid, string> NewOutput { get; private set; }
		public Action<Guid, CompileMessage> NewMessage { get; private set; }
	    public ObservableCollection<CompileMessage> Messages { get; private set; }
		public CompileMessageSeverity HighestSeverity { get; private set; }
		public string Name { get; private set; }
	    public GenerationLanguage Language { get; private set; }
	    public GenerationModule Module { get; private set; }
		public bool IsInitialized { get; private set; }

		public Generator()
		{
			Messages = new ObservableCollection<CompileMessage>();
			Name = "WCF Architect .NET CSharp Generator";
			Language = GenerationLanguage.CSharp;
			Module = GenerationModule.WindowsRuntime;
		}

		public void Initialize(string License, Action<Guid, string> OutputHandler, Action<Guid, CompileMessage> CompileMessageHandler)
	    {
		    Globals.LicenseKey = License;
		    NewOutput = OutputHandler;
		    NewMessage = CompileMessageHandler;
			var t = new CryptoLicense(Globals.LicenseKey, Globals.LicenseVerification);
			IsInitialized = (t.Status == LicenseStatus.Valid);
		}

		[System.Reflection.Obfuscation(Feature = "encryptmethod", Exclude = false, StripAfterObfuscation = true)]
		public void Build(Project Data, bool ClientOnly = false)
		{
			var l = new CryptoLicense(Globals.LicenseKey, Globals.LicenseVerification);
			if (l.Status != LicenseStatus.Valid) return;

			Messages.Clear();
			NewOutput(Data.ID, Globals.ApplicationTitle);
			NewOutput(Data.ID, string.Format("Version: {0}", Globals.ApplicationVersion));
			NewOutput(Data.ID, "Copyright © 2012-2013 Prospective Software Inc.");

			Verify(Data);

			//If the verification produced errors exit with an error code, we cannot proceed.
			if (HighestSeverity == CompileMessageSeverity.ERROR)
				return;

			if(!ClientOnly)
				foreach (ProjectGenerationTarget t in Data.ServerGenerationTargets)
				{
					string op = new Uri(new Uri(Data.AbsolutePath), System.IO.Path.Combine(t.Path, Data.ServerOutputFile + ".cs")).LocalPath;
					op = Uri.UnescapeDataString(op);
					NewOutput(Data.ID, string.Format("Writing Server Output: {0}", op));
					System.IO.File.WriteAllText(op, GenerateServer(Data, t.Framework));
				}

			foreach (ProjectGenerationTarget t in Data.ClientGenerationTargets.Where(a => a.Framework != ProjectGenerationFramework.WIN8))
			{
				string op = new Uri(new Uri(Data.AbsolutePath), System.IO.Path.Combine(t.Path, Data.ClientOutputFile + ".cs")).LocalPath;
				op = Uri.UnescapeDataString(op);
				NewOutput(Data.ID, string.Format("Writing Client Output: {0}", op));
				System.IO.File.WriteAllText(op, GenerateClient(Data, t.Framework));
			}
		}

		public Task BuildAsync(Project Data, bool ClientOnly = false)
		{
			return System.Windows.Application.Current == null ? null : Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(() => Build(Data, ClientOnly), DispatcherPriority.Normal));
		}

		[System.Reflection.Obfuscation(Feature = "encryptmethod", Exclude = false, StripAfterObfuscation = true)]
		public void Verify(Project Data)
	    {
			var t = new CryptoLicense(Globals.LicenseKey, Globals.LicenseVerification);
		    if (t.Status != LicenseStatus.Valid) return;

			if (string.IsNullOrEmpty(Data.ServerOutputFile))
				AddMessage(new CompileMessage("GS0003", "The '" + Data.Name + "' project does not have a Server Assembly Name. You must specify a Server Assembly Name.", CompileMessageSeverity.ERROR, null, Data, Data.GetType(), Guid.Empty, Data.ID));
			else
				if (RegExs.MatchFileName.IsMatch(Data.ServerOutputFile) == false)
					AddMessage(new CompileMessage("GS0004", "The Server Assembly Name in '" + Data.Name + "' project is not set or contains invalid characters. You must specify a valid Windows file name.", CompileMessageSeverity.ERROR, null, Data, Data.GetType(), Guid.Empty, Data.ID));
			if (string.IsNullOrEmpty(Data.ClientOutputFile))
				AddMessage(new CompileMessage("GS0005", "The '" + Data.Name + "' project does not have a Client Assembly Name. You must specify a Client Assembly Name.", CompileMessageSeverity.ERROR, null, Data, Data.GetType(), Guid.Empty, Data.ID));
			else
				if (RegExs.MatchFileName.IsMatch(Data.ClientOutputFile) == false)
					AddMessage(new CompileMessage("GS0006", "The Client Assembly Name in '" + Data.Name + "' project is not set or contains invalid characters. You must specify a valid Windows file name.", CompileMessageSeverity.ERROR, null, Data, Data.GetType(), Guid.Empty, Data.ID));
			if ((Data.ServerOutputFile == Data.ClientOutputFile))
				AddMessage(new CompileMessage("GS0007", "The '" + Data.Name + "' project Client and Server Assembly Names are the same. You must specify a different Server or Client Assembly Name.", CompileMessageSeverity.ERROR, null, Data, Data.GetType(), Guid.Empty, Data.ID));
			
			NamespaceGenerator.VerifyCode(Data.Namespace, AddMessage);
	    }

		public Task VerifyAsync(Project Data)
		{
			return System.Windows.Application.Current == null ? null : Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(() => Verify(Data), DispatcherPriority.Normal));
		}

		[System.Reflection.Obfuscation(Feature = "encryptmethod", Exclude = false, StripAfterObfuscation = true)]
		public string GenerateServer(Project Data, ProjectGenerationFramework Framework)
	    {
			var t = new CryptoLicense(Globals.LicenseKey, Globals.LicenseVerification);
			if (t.Status != LicenseStatus.Valid) return "";

			Globals.CurrentGenerationTarget = Framework;

			return Generate(Data, Framework, true);
		}

		[System.Reflection.Obfuscation(Feature = "encryptmethod", Exclude = false, StripAfterObfuscation = true)]
		public string GenerateClient(Project Data, ProjectGenerationFramework Framework)
		{
			var t = new CryptoLicense(Globals.LicenseKey, Globals.LicenseVerification);
			if (t.Status != LicenseStatus.Valid) return "";

			Globals.CurrentGenerationTarget = Framework;

			return Generate(Data, Framework, false);
		}

		public Task<string> GenerateServerAsync(Project Data, ProjectGenerationFramework Framework)
		{
			return System.Windows.Application.Current == null ? null : Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(() => GenerateServer(Data, Framework), DispatcherPriority.Normal));
		}

	    public Task<string> GenerateClientAsync(Project Data, ProjectGenerationFramework Framework)
		{
			return System.Windows.Application.Current == null ? null : Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(() => GenerateClient(Data, Framework), DispatcherPriority.Normal));
		}

	    private string Generate(Project Data, ProjectGenerationFramework Framework, bool Server)
		{
			Globals.CurrentGenerationTarget = Framework;

			var code = new StringBuilder();
			code.AppendLine("//---------------------------------------------------------------------------");
			code.AppendLine("// This code was generated by a tool. Changes to this file may cause ");
			code.AppendLine("// incorrect behavior and will be lost if the code is regenerated.");
			code.AppendLine("//");
			code.AppendLine(string.Format("// WCF Architect Version:\t{0}", Globals.ApplicationVersion));
			if (Framework == ProjectGenerationFramework.NET30) code.AppendLine("// .NET Framework Version:\t3.0");
			if (Framework == ProjectGenerationFramework.NET35) code.AppendLine("// .NET Framework Version:\t3.5");
			if (Framework == ProjectGenerationFramework.NET35Client) code.AppendLine("// .NET Framework Version:\t3.5 (Client)");
			if (Framework == ProjectGenerationFramework.NET40) code.AppendLine("// .NET Framework Version:\t4.0");
			if (Framework == ProjectGenerationFramework.NET40Client) code.AppendLine("// .NET Framework Version:\t4.0 (Client)");
			if (Framework == ProjectGenerationFramework.NET45) code.AppendLine("// .NET Framework Version:\t4.5");
			if (Framework == ProjectGenerationFramework.SL40) code.AppendLine("// Silverlight Version:\t4.0");
			if (Framework == ProjectGenerationFramework.SL50) code.AppendLine("// Silverlight Version:\t5.0");
			code.AppendLine("//---------------------------------------------------------------------------");
			code.AppendLine();

			// Generate using namespaces
			foreach (ProjectUsingNamespace pun in GetUsingNamespaces(Data))
			{
				if (pun.Server && Server)
				{
					if (pun.NET && (Framework == ProjectGenerationFramework.NET30 || Framework == ProjectGenerationFramework.NET35 || Framework == ProjectGenerationFramework.NET40 || Framework == ProjectGenerationFramework.NET45))
						code.AppendFormat("using {0};{1}", pun.Namespace, Environment.NewLine);
					else if (pun.NET && (Framework == ProjectGenerationFramework.NET35Client || Framework == ProjectGenerationFramework.NET40Client))
						if (!pun.IsFullFrameworkOnly) code.AppendFormat("using {0};{1}", pun.Namespace, Environment.NewLine);
				}
				if (!pun.Client || Server) continue;
				if (pun.NET && (Framework == ProjectGenerationFramework.NET30 || Framework == ProjectGenerationFramework.NET35 || Framework == ProjectGenerationFramework.NET40 || Framework == ProjectGenerationFramework.NET45))
					code.AppendFormat("using {0};{1}", pun.Namespace, Environment.NewLine);
				else if (pun.NET && (Framework == ProjectGenerationFramework.NET35Client || Framework == ProjectGenerationFramework.NET40Client))
					if (!pun.IsFullFrameworkOnly) code.AppendFormat("using {0};{1}", pun.Namespace, Environment.NewLine);
				if (pun.SL && (Framework == ProjectGenerationFramework.SL40 || Framework == ProjectGenerationFramework.SL50))
					code.AppendFormat("using {0};{1}", pun.Namespace, Environment.NewLine);
			}
			code.AppendLine();

			//Generate ContractNamespace Attributes
			if (!Server) code.AppendLine(NamespaceGenerator.GenerateContractNamespaceAttributes(Data.Namespace));

			//Disable XML documentation warnings 
			if (!Data.EnableDocumentationWarnings) code.AppendLine("#pragma warning disable 1591");

			//Scan, verify, and generate references
			var refs = new List<DataType>(ReferenceScan(Data.Namespace));
			if (refs.Count > 0)
			{
				code.AppendLine("\t/**************************************************************************");
				code.AppendLine("\t*\tDependency Types");
				code.AppendLine("\t**************************************************************************/");
				code.AppendLine();
				foreach (Projects.Enum e in refs.Where(a => a.GetType() == typeof(Projects.Enum)))
					EnumGenerator.VerifyCode(e, AddMessage);
				foreach (Data d in refs.Where(a => a.GetType() == typeof(Data)))
					DataGenerator.VerifyCode(d, AddMessage);
				foreach (DataType dt in refs)
					code.AppendLine(ReferenceGenerate(dt, Data, Server, AddMessage));
			}

			//Generate project
			if (Server)
			{
				if (Framework == ProjectGenerationFramework.NET30) code.AppendLine(NamespaceGenerator.GenerateServerCode30(Data.Namespace));
				if (Framework == ProjectGenerationFramework.NET35) code.AppendLine(NamespaceGenerator.GenerateServerCode35(Data.Namespace));
				if (Framework == ProjectGenerationFramework.NET35Client) code.AppendLine(NamespaceGenerator.GenerateServerCode35Client(Data.Namespace));
				if (Framework == ProjectGenerationFramework.NET40) code.AppendLine(NamespaceGenerator.GenerateServerCode40(Data.Namespace));
				if (Framework == ProjectGenerationFramework.NET40Client) code.AppendLine(NamespaceGenerator.GenerateServerCode40Client(Data.Namespace));
				if (Framework == ProjectGenerationFramework.NET45) code.AppendLine(NamespaceGenerator.GenerateServerCode45(Data.Namespace));
			}
			else
			{
				if (Framework == ProjectGenerationFramework.NET30) code.AppendLine(NamespaceGenerator.GenerateClientCode30(Data.Namespace));
				if (Framework == ProjectGenerationFramework.NET35) code.AppendLine(NamespaceGenerator.GenerateClientCode35(Data.Namespace));
				if (Framework == ProjectGenerationFramework.NET35Client) code.AppendLine(NamespaceGenerator.GenerateClientCode35Client(Data.Namespace));
				if (Framework == ProjectGenerationFramework.NET40) code.AppendLine(NamespaceGenerator.GenerateClientCode40(Data.Namespace));
				if (Framework == ProjectGenerationFramework.NET40Client) code.AppendLine(NamespaceGenerator.GenerateClientCode40Client(Data.Namespace));
				if (Framework == ProjectGenerationFramework.NET45) code.AppendLine(NamespaceGenerator.GenerateClientCode45(Data.Namespace));
			}

			//Reenable XML documentation warnings
			if (!Data.EnableDocumentationWarnings) code.AppendLine("#pragma warning restore 1591");

		    return code.ToString();
		}

		private void AddMessage(CompileMessage Message)
		{
			Messages.Add(Message);
			if (Message.Severity == CompileMessageSeverity.ERROR && HighestSeverity != CompileMessageSeverity.ERROR) HighestSeverity = CompileMessageSeverity.ERROR;
			if (Message.Severity == CompileMessageSeverity.WARN && HighestSeverity == CompileMessageSeverity.INFO) HighestSeverity = CompileMessageSeverity.WARN;
			NewOutput(Message.OwnerID, string.Format("{0} {1}: {2} Object: {3} Owner: {4}", Message.Severity, Message.Code, Message.Description, Message.ErrorObject, Message.Owner));
			NewMessage(Message.OwnerID, Message);
		}

		private static IEnumerable<DataType> ReferenceScan(Namespace Scan)
		{
			var refs = new List<DataType>();

			foreach (DataElement de in Scan.Data.SelectMany(d => d.Elements))
			{
				if (de.DataType.IsTypeReference) refs.Add(de.DataType);
				if (de.ClientType != null && de.ClientType.IsTypeReference) refs.Add(de.ClientType);
				if (de.XAMLType != null && de.XAMLType.IsTypeReference) refs.Add(de.XAMLType);
			}

			foreach (Service s in Scan.Services)
			{
				foreach (Method m in s.ServiceOperations.Where(a => a.GetType() == typeof(Method)))
				{
					if (m.ReturnType.IsTypeReference) refs.Add(m.ReturnType);
					refs.AddRange(from mp in m.Parameters where mp.Type.IsTypeReference select mp.Type);
				}
				refs.AddRange(from Property p in s.ServiceOperations.Where(a => a.GetType() == typeof(Property)) where p.ReturnType.IsTypeReference select p.ReturnType);
				foreach (Method m in s.CallbackOperations.Where(a => a.GetType() == typeof(Method)))
				{
					if (m.ReturnType.IsTypeReference) refs.Add(m.ReturnType);
					refs.AddRange(from mp in m.Parameters where mp.Type.IsTypeReference select mp.Type);
				}
				refs.AddRange(from Property p in s.CallbackOperations.Where(a => a.GetType() == typeof(Property)) where p.ReturnType.IsTypeReference select p.ReturnType);
			}

			foreach (Namespace n in Scan.Children)
				refs.AddRange(ReferenceScan(n).Where(a => refs.All(b => a.ToDeclarationString() != b.ToDeclarationString())));

			return refs;
		}

		private static DataType ReferenceRetrieve(Project Project, Namespace Namespace, Guid TypeID)
		{
			var d = Namespace.Data.FirstOrDefault(a => a.ID == TypeID);
			if (d != null) return d;
			var e = Namespace.Enums.FirstOrDefault(a => a.ID == TypeID);
			if (e != null) return e;

			foreach (Namespace n in Namespace.Children)
			{
				var t = ReferenceRetrieve(Project, n, TypeID);
				if (t != null) return t;
			}

			return !Equals(Namespace, Project.Namespace) ? null : Project.DependencyProjects.Where(a => a.GenerateReferences).Select(dp => ReferenceRetrieve(dp.Project, dp.Project.Namespace, TypeID)).FirstOrDefault(t => t != null);
		}

		private static string ReferenceGenerate(DataType Reference, Project RefProject, bool Server, Action<CompileMessage> AddMessage)
		{
			//Get the referenced type
			DataType typeref = ReferenceRetrieve(RefProject, RefProject.Namespace, Reference.ID);
			if (typeref == null)
			{
				AddMessage(new CompileMessage("GS0008", string.Format("Unable to locate type '{0}'. Please ensure that you have added the project containing this type to the Dependency Projects list and that it has not been renamed or removed from the project.", Reference.Name), CompileMessageSeverity.ERROR, null, Reference.Parent.Owner, Reference.Parent.Owner.GetType(), Reference.ID, Reference.Parent.Owner.ID));
				return "";
			}
			Type rt = typeref.GetType();

			//Generate a namespace wrapper for the type then generate the type inside the namespace
			var code = new StringBuilder();
			code.AppendFormat("namespace {0}{1}", typeref.Parent.FullName, Environment.NewLine);
			code.AppendLine("{");
			if (Server && rt == typeof(Data))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET30) code.AppendLine(DataGenerator.GenerateServerCode30(typeref as Data));
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35Client) code.AppendLine(DataGenerator.GenerateServerCode35(typeref as Data));
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40Client) code.AppendLine(DataGenerator.GenerateServerCode40(typeref as Data));
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET45) code.AppendLine(DataGenerator.GenerateServerCode45(typeref as Data));
			}
			else if (!Server && rt == typeof(Data))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET30) code.AppendLine(DataGenerator.GenerateProxyCode30(typeref as Data));
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35Client) code.AppendLine(DataGenerator.GenerateProxyCode35(typeref as Data));
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40Client) code.AppendLine(DataGenerator.GenerateProxyCode40(typeref as Data));
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET45) code.AppendLine(DataGenerator.GenerateProxyCode45(typeref as Data));
				code.AppendLine();
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET30) code.AppendLine(DataGenerator.GenerateXAMLCode30(typeref as Data));
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35Client) code.AppendLine(DataGenerator.GenerateXAMLCode35(typeref as Data));
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40Client) code.AppendLine(DataGenerator.GenerateXAMLCode40(typeref as Data));
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET45) code.AppendLine(DataGenerator.GenerateXAMLCode45(typeref as Data));
			}
			else if (Server && rt == typeof(Projects.Enum))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET30) code.AppendLine(EnumGenerator.GenerateServerCode30(typeref as Projects.Enum));
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35Client) code.AppendLine(EnumGenerator.GenerateServerCode35(typeref as Projects.Enum));
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40Client) code.AppendLine(EnumGenerator.GenerateServerCode40(typeref as Projects.Enum));
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET45) code.AppendLine(EnumGenerator.GenerateServerCode45(typeref as Projects.Enum));
			}
			else if (!Server && rt == typeof(Projects.Enum))
			{
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET30) code.AppendLine(EnumGenerator.GenerateProxyCode30(typeref as Projects.Enum));
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35Client) code.AppendLine(EnumGenerator.GenerateProxyCode35(typeref as Projects.Enum));
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40Client) code.AppendLine(EnumGenerator.GenerateProxyCode40(typeref as Projects.Enum));
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET45) code.AppendLine(EnumGenerator.GenerateProxyCode45(typeref as Projects.Enum));
			}
			code.AppendLine("}");

			return code.ToString();
		}

		private static IEnumerable<ProjectUsingNamespace> GetUsingNamespaces(Project CurProject)
		{
			var puns = new List<ProjectUsingNamespace>(CurProject.UsingNamespaces);

			foreach (DependencyProject dp in CurProject.DependencyProjects)
				puns.AddRange(GetUsingNamespaces(dp.Project).Where(a => puns.All(b => a.Namespace != b.Namespace)));

			return puns;
		}
	}

	internal static class Globals
	{
		public static readonly Version ApplicationVersion;
		public const string ApplicationTitle = "WCF Architect .NET CSharp Generator - BETA";

		public static ProjectGenerationFramework CurrentGenerationTarget { get; set; }
		
		public static string LicenseKey { get; set; }
		public const string LicenseVerification = "AMAAMACnZigmLe9LpWcsYIBVFHYRZeUhr1oYyxDRFmL/qon4ijMx6X/xXyYldZs/A8Df9MsDAAEAAQ==";

		static Globals()
		{
			string asmfp = System.IO.Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetCallingAssembly().CodeBase).LocalPath);
			if (asmfp == null) return;
			ApplicationVersion = new Version(System.Diagnostics.FileVersionInfo.GetVersionInfo(System.IO.Path.Combine(asmfp, "WCFArchitect.Generators.NET.CS.dll")).FileVersion);
		}
	}
}