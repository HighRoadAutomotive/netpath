using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NETPath.Generators.Interfaces;
using EllipticBit.Controls.WPF.Dialogs;

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
			if (!Directory.Exists(Globals.UserProfilePath)) Directory.CreateDirectory(Globals.UserProfilePath);

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

		public static async Task<bool> CheckForInternetConnection()
		{
			try
			{
				var p = new Ping();
				PingReply pr = await p.SendPingAsync("http://www.google.com", 10000);
				return pr.Status == IPStatus.Success;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			// For use in debugging.
			//DialogService.ShowMessageDialog(null, "We've Encountered an Unknown Problem.", e.Exception.ToString(), new DialogAction("Dismiss", false, true));
			Clipboard.SetText(e.Exception.ToString());
			System.IO.File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Prospective Software", "NETPath", "NPError.txt"), e.Exception.ToString());

			if (Globals.MainScreen == null || !Globals.MainScreen.IsLoaded) return;

			//Take a screenshot
			var targetBitmap = new RenderTargetBitmap((int)Globals.MainScreen.ActualWidth, (int)Globals.MainScreen.ActualHeight, 96d, 96d, PixelFormats.Default);
			targetBitmap.Render(Globals.MainScreen);
			var encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(targetBitmap));
			var ms = new MemoryStream();
			encoder.Save(ms);

			//Get project files
			var pfd = new Dictionary<string, byte[]>();
			if (Globals.Project != null)
				pfd.Add(Path.GetFileName(Globals.Project.AbsolutePath) ?? Globals.Project.Name + ".npp", Projects.Project.Dump(Globals.Project));

			var nr = new Interface.Dialogs.ReportError(e.Exception, ms.ToArray(), pfd);

			//TODO: Add reporting functionality when the JIRA integration is built.
			DialogService.ShowContentDialog(null, "We've Encountered an Unknown Problem.", nr, new DialogAction("Send Report", nr.SendReport, true), new DialogAction("Dismiss", false, true));
			e.Handled = true;
		}
	}
}