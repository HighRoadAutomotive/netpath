using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using Prospective.Controls.Dialogs;

namespace WCFArchitect
{
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			//Get executable data. 
			Globals.ApplicationPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
			Globals.ApplicationVersion = new Version(System.Diagnostics.FileVersionInfo.GetVersionInfo(Globals.ApplicationPath + "WCFArchitect.exe").FileVersion);

			//Check to see if the User Profile path exists and make a folder if it does not.
			Globals.UserProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Prospective Software\\WCF Architect\\";
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

			Globals.InitializeCodeGenerators("NgAAAR34yUVouM0B89nN/6JGzgGHo04qSoBnf8S47pP6T/Awg2aOLNXVHFlxYaTAmetprPIDC9YxTuDJsAf3Er3NdiI=");
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			Options.UserProfile.Save(Globals.UserProfilePath, Globals.UserProfile);
		}

		public void ResetUserProfile()
		{
			Globals.UserProfile = new Options.UserProfile(Environment.UserDomainName + "\\" + Environment.UserName);
			Options.UserProfile.Save(Globals.UserProfilePath, Globals.UserProfile);
		}

		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			//TODO: Add reporting functionality when the licensing integration is built.
			DialogService.ShowMessageDialog(null,"We've Encountered an Unknown Problem.", "The following exception was caught by WCF Architect. Please report the error using the Bug Report Page link in the About section of the Options page." + Environment.NewLine + Environment.NewLine + e.Exception,
				new DialogAction("Report", () => {}, true), new DialogAction("Dismiss", false, true));
			e.Handled = true;
		}
	}
}