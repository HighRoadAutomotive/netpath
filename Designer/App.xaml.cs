using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Prospective.Controls.Dialogs;

namespace NETPath
{
	public partial class App : Application
	{
		[System.Reflection.Obfuscation(Feature = "encryptmethod", Exclude = false, StripAfterObfuscation = true)]
		public void Application_Startup(object sender, StartupEventArgs e)
		{
			//Get executable data. 
			Globals.ApplicationPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
			Globals.ApplicationVersion = new Version(System.Diagnostics.FileVersionInfo.GetVersionInfo(Globals.ApplicationPath + "NETPath.exe").FileVersion);

			//Check to see if the User Profile path exists and make a folder if it does not.
			Globals.UserProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Prospective Software\\NETPath\\";
			if (Directory.Exists(Globals.UserProfilePath) == false) Directory.CreateDirectory(Globals.UserProfilePath);

			//Load the user profile if it exists or make a new one if it doesn't.
			Globals.UserProfilePath = Path.Combine(Globals.UserProfilePath, "profile.dat");
			if (File.Exists(Globals.UserProfilePath))
			{
				Globals.UserProfile = Options.UserProfile.Open(Globals.UserProfilePath);
			}
			else
			{
				Globals.UserProfile = new Options.UserProfile(Environment.UserDomainName + "\\" + Environment.UserName);
				Options.UserProfile.Save(Globals.UserProfilePath, Globals.UserProfile);
			}

			//Process the command line
			Globals.ArgSolutionPath = "";
			var args = new List<string>(e.Args);
			if (args.Count > 0)
			{
				for (int i = 0; i < args.Count; i++)
				{
					if (args[i] == "/ResetUserProfile" || args[i] == "/resetuserprofile" || args[i] == "/RU" || args[i] == "/ru")
						ResetUserProfile();

					if (args[i] == "/o")
						Globals.ArgSolutionPath = args[i + 1];
				}
			}

			Globals.InitializeCodeGenerators("NgAAAVKb1dSPMM4BgC7KbyR4zgFNG/5h+9s3tmGvwerMnzfvh7MoEFx9eEY02pTdvOE2lu7yrZxnu/oGXVgjPNa01zM=");
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			Options.UserProfile.Save(Globals.UserProfilePath, Globals.UserProfile);
		}

		private void ResetUserProfile()
		{
			Globals.UserProfile = new Options.UserProfile(Environment.UserDomainName + "\\" + Environment.UserName);
			Options.UserProfile.Save(Globals.UserProfilePath, Globals.UserProfile);
		}

		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			//Take a screenshot
			var targetBitmap = new RenderTargetBitmap((int)Globals.MainScreen.ActualWidth, (int)Globals.MainScreen.ActualHeight, 96d, 96d, PixelFormats.Default);
			targetBitmap.Render(Globals.MainScreen);
			var encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(targetBitmap));
			var ms = new MemoryStream();
			encoder.Save(ms);

			//Get project files
			var pfd = new Dictionary<string, byte[]>();
			if (Globals.Solution != null)
			{
				pfd.Add(Path.GetFileName(Globals.Solution.AbsolutePath) ?? Globals.Solution.Name + ".nps", Projects.Solution.Dump(Globals.Solution));
				foreach (Projects.Project p in Globals.Projects)
					pfd.Add(Path.GetFileName(p.AbsolutePath) ?? p.Name + ".npp", Projects.Project.Dump(p));
			}

			var nr = new Interface.Dialogs.ReportError(e.Exception, ms.ToArray(), pfd);

			//TODO: Add reporting functionality when the licensing integration is built.
			DialogService.ShowContentDialog(null,"We've Encountered an Unknown Problem.", nr, new DialogAction("Send Report", nr.SendReport, true), new DialogAction("Dismiss", false, true));
			e.Handled = true;
		}
	}
}