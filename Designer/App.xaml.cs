using System; 
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Prospective.Controls.Dialogs;
using Prospective.Server.Licensing;

namespace NETPath
{
	public partial class App : Application
	{
		[System.Reflection.Obfuscation(Feature = "encryptmethod", Exclude = false, StripAfterObfuscation = true)]
		public async void Application_Startup(object sender, StartupEventArgs e)
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

#if LICENSE
			DateTime servertime = Prospective.Server.Licensing.LicensingClient.GetDateTime();
			DateTime localtime = DateTime.UtcNow;

			if (localtime > servertime.AddHours(-2) && localtime < servertime.AddHours(2))
			{
				if (Globals.UserProfile.IsTrial || Globals.UserProfile.Serial == "TRIAL" || Globals.UserProfile.License == "")
				{
					InitializeUsageData();
					Globals.InitializeCodeGenerators("lgKkAVJQzjxpdM4BHgAcAE5QMjBUUkkjUHJvc3BlY3RpdmUgU29mdHdhcmUBAzdrgDQFwUK+35DB7JVSoJdNhP6J1+i+aXlSvgdxxIQjTwmIlFmjQ5afJpevTTpY5+czoQeJl9SW6A/MAVvoU/wjAu3mfv5XNvfnxwwOqBpS1gc9rVipqgZgSKwAi197kJ7wqkXhbHPjrkE+ATwOB2rCEunyJ8sKrGAO810ovBXs2GWhgJUC/i2TDN456PmIrAN4DqS/tgL9aKdvefI0CxNIa5QPmQIocQDgjuppTT2op9wVTj/GxRMLxZBr4Iy1ktCB5CiT0ILzc0+umxYR5RSVKJwMlwWZjO528mKv1k4m07uaH1Tsz2V5sTVKYkAmfDmITZwei+phCkBbI0Xjy6E=");
					var lic = new LogicNP.CryptoLicensing.CryptoLicense("lgKkAVJQzjxpdM4BHgAcAE5QMjBUUkkjUHJvc3BlY3RpdmUgU29mdHdhcmUBAzdrgDQFwUK+35DB7JVSoJdNhP6J1+i+aXlSvgdxxIQjTwmIlFmjQ5afJpevTTpY5+czoQeJl9SW6A/MAVvoU/wjAu3mfv5XNvfnxwwOqBpS1gc9rVipqgZgSKwAi197kJ7wqkXhbHPjrkE+ATwOB2rCEunyJ8sKrGAO810ovBXs2GWhgJUC/i2TDN456PmIrAN4DqS/tgL9aKdvefI0CxNIa5QPmQIocQDgjuppTT2op9wVTj/GxRMLxZBr4Iy1ktCB5CiT0ILzc0+umxYR5RSVKJwMlwWZjO528mKv1k4m07uaH1Tsz2V5sTVKYkAmfDmITZwei+phCkBbI0Xjy6E=", Globals.LicenseVerification);
					Globals.UserProfile.SKU = lic.UserData.Split(new[] { '#' })[0];
					Globals.UserProfile.LicenseeName = lic.UserData.Split(new[] { '#' })[1];

					if (CheckForInternetConnection())
					{
						try
						{
							if (Globals.UserProfile.PriorUsage != null) await LicensingClient.PostUsage(Globals.UserProfile.PriorUsage);
						}
						catch (Exception)
						{
						}
					}
				}
				else
				{
					Globals.InitializeCodeGenerators(Globals.UserProfile.License);
					var lic = new LogicNP.CryptoLicensing.CryptoLicense(Globals.UserProfile.License, Globals.LicenseVerification);
					Globals.UserProfile.SKU = lic.UserData.Split(new[] { '#' })[0];
					Globals.UserProfile.LicenseeName = lic.UserData.Split(new[] { '#' })[1];
					
					if (CheckForInternetConnection())
					{
						try
						{
							Prospective.Server.Licensing.LicenseData LD = await Prospective.Server.Licensing.LicensingClient.Retrieve(Globals.UserProfile.Serial, Globals.ApplicationVersion);
							if(Globals.UserProfile.PriorUsage != null) await LicensingClient.PostUsage(Globals.UserProfile.PriorUsage);
							Globals.UserProfile.UserName = LD.UserName;
							if (LD.Revoked)
							{
								Globals.UserProfile.Serial = "TRIAL";
								Globals.UserProfile.IsTrial = true;
								Globals.UserProfile.License = "";
							}
						}
						catch (Exception)
						{
						}
					}
				}
			}

#else
			CheckForInternetConnection();
			Globals.InitializeCodeGenerators("FACAAEaR+9dgdM4BAQNUsbrHWerJElJe1mwbYMfD9oP5NDjkX10HHZGT2+BWJClw+rEip3LXwSEvOI5NxbIV9KtnubS1wt2Ay3KqT6CL/ds6njfwnOisLB1BEJE8bymCiSZmU82Ij05i2wAfxYz4j0WfCZsCdR835J5kVPw3kTI+1KJLkHPUN1rI7uQbdkCtdIDwRvt8HAfOYh3rR5e0GETZ/ctzXnnT90w/ps+1TK5dh9hy6y6rrBap/KX+OeLWwccMfGSFoBEKjPXynozWNuK4IFOQr5b8TDjXcrG3OrLIZiAk2Qz2kX6+wvjedxO3Q4nB0vOuUO2zD5HngyxQt6B7xXz8OAlxpuYgLSk5");
			Globals.UserProfile.IsTrial = false;
			Globals.UserProfile.Serial = "DEVELOPER";
#endif
			InitializeUsageData();
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			Globals.Usage.EndTime = DateTime.UtcNow;
			Options.UserProfile.Save(Globals.UserProfilePath, Globals.UserProfile);
		}

		private void InitializeUsageData()
		{
			Globals.Usage = new UsageData();
			Globals.Usage.SKU = Globals.UserProfile.SKU;
			Globals.Usage.Version = Globals.ApplicationVersion;
			Globals.Usage.Serial = Globals.UserProfile.Serial;
			Globals.Usage.CLRVersion = Environment.Version;
			Globals.Usage.OSVersion = Environment.OSVersion.ToString();
			Globals.Usage.Processors = Environment.ProcessorCount;
			Globals.Usage.UserID = System.Utilities.Cryptography.Hash.Compute512Hex(Environment.UserDomainName + "\\" + Environment.UserName);
			Globals.Usage.StartTime = DateTime.UtcNow;
			Globals.UserProfile.PriorUsage = Globals.Usage;
		}

		private void ResetUserProfile()
		{
			Globals.UserProfile = new Options.UserProfile(Environment.UserDomainName + "\\" + Environment.UserName);
			Options.UserProfile.Save(Globals.UserProfilePath, Globals.UserProfile);
		}

		public static bool CheckForInternetConnection()
		{
			var client = new HttpClient() {Timeout = new TimeSpan(0, 0, 3)};
			return client.GetAsync("http://www.google.com").Result.IsSuccessStatusCode;
		}

		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			// For use in debugging.
			//DialogService.ShowMessageDialog(null, "We've Encountered an Unknown Problem.", e.Exception.ToString(), new DialogAction("Dismiss", false, true));

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

			//TODO: Add reporting functionality when the JIRA integration is built.
			DialogService.ShowContentDialog(null,"We've Encountered an Unknown Problem.", nr, new DialogAction("Send Report", nr.SendReport, true), new DialogAction("Dismiss", false, true));
			e.Handled = true;
		}
	}
}