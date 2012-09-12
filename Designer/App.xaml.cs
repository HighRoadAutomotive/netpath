using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using MP.Karvonite;
using System.IO;
using Prospective.Controls.Dialogs;

namespace WCFArchitect
{
	public partial class App : Application
	{
		private ObjectSpace uos;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			//Get executable data. 
			Globals.ApplicationPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
			Globals.ApplicationVersion = new Version(System.Diagnostics.FileVersionInfo.GetVersionInfo(Globals.ApplicationPath + "WCFArchitect.exe").FileVersion);

			//Check to see if the License Key path exists and make a folder if it does not.
			Globals.LicenseKeyPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Prospective Software\\WCF Architect\\key.dat";

			//Check to see if the User Profile path exists and make a folder if it does not.
			Globals.UserProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Prospective Software\\WCF Architect\\";
			if (System.IO.Directory.Exists(Globals.UserProfilePath) == false) System.IO.Directory.CreateDirectory(Globals.UserProfilePath);

			//Load the license data file
			uos = new ObjectSpace("UserProfile.kvtmodel", "WCFArchitect");
			if (!File.Exists(Globals.UserProfilePath + "UserProfile.dat"))
				uos.CreateObjectLibrary(Globals.UserProfilePath + "UserProfile.dat", ExistingFileAction.DoNotOverwrite);
			uos.EnableConcurrency = true;
			uos.Open(Globals.UserProfilePath + "UserProfile.dat", ObjectSpaceOpenMode.ReadWrite);
			Globals.UserProfileSpace = uos;

			//Process the command line
			Globals.ArgSolutionPath = "";
			bool Start = true;
			List<string> args = new List<string>(e.Args);
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
			if (Start == false)
			{
				this.Shutdown(0);
			}

			//Load the correct user profile or create a new one if no matching profile was found.
			List<Options.UserProfile> PL = uos.OfType<Options.UserProfile>().Where(a => a.User == Environment.UserDomainName + "\\" + Environment.UserName).ToList<Options.UserProfile>();
			if (PL.Count > 0)
			{
				Globals.UserProfile = PL[0];
			}
			else
			{
				Globals.UserProfile = new Options.UserProfile(Environment.UserDomainName + "\\" + Environment.UserName);
				uos.Add(Globals.UserProfile);
			}
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			uos.Save();
			uos.Close();
		}

		public void ResetUserProfile()
		{
			List<Options.UserProfile> PL = uos.OfType<Options.UserProfile>().Where(a => a.User == Environment.UserDomainName + "\\" + Environment.UserName).ToList<Options.UserProfile>();
			if (PL.Count > 0)
			{
				PL[0] = new Options.UserProfile(Environment.UserDomainName + "\\" + Environment.UserName);
			}
			else
			{
				Globals.UserProfile = new Options.UserProfile(Environment.UserDomainName + "\\" + Environment.UserName);
				uos.Add(Globals.UserProfile);
			}
			uos.Save();
		}

		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			//TODO: Add reporting functionality when the licensing integration is built.
			DialogService.ShowMessageDialog(null,"We've Encountered an Unknown Problem.", "The following exception was caught by WCF Architect. Please report the error using the Bug Report Page link in the About section of the Options page." + Environment.NewLine + Environment.NewLine + e.Exception.ToString(),
				new DialogAction("Report", new Action(() => {}), true), new DialogAction("Dismiss", false, true));
			e.Handled = true;
		}
	}
}