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
			if (CheckForInternetConnection())
			{
				DateTime servertime = LicensingClient.GetDateTime();
				DateTime localtime = DateTime.UtcNow;

				if (localtime > servertime.AddHours(-2) && localtime < servertime.AddHours(2))
				{
					if (Globals.UserProfile.IsTrial || Globals.UserProfile.Serial == "TRIAL" || Globals.UserProfile.License == "")
					{
						Globals.InitializeCodeGenerators(Globals.TrialLicense);
						var lic = new LogicNP.CryptoLicensing.CryptoLicense(Globals.TrialLicense, Globals.LicenseVerification);
						Globals.UserProfile.SKU = lic.GetUserDataFieldValue("SKU", "#");
						Globals.UserProfile.LicenseeName = lic.GetUserDataFieldValue("LicenseeName", "#");

						Globals.UserProfile.Serial = "TRIAL";
						Globals.UserProfile.IsTrial = true;
						Globals.UserProfile.IsUsageEnabled = true;
						Globals.UserProfile.License = "";
						try
						{
							if (Globals.UserProfile.PriorUsage != null) await LicensingClient.PostUsage(Globals.UserProfile.PriorUsage);
						}
						catch (Exception)
						{
						}
					}
					else
					{
						Globals.InitializeCodeGenerators(Globals.UserProfile.License);
						var lic = new LogicNP.CryptoLicensing.CryptoLicense(Globals.UserProfile.License, Globals.LicenseVerification);
						Globals.UserProfile.SKU = lic.GetUserDataFieldValue("SKU", "#");
						Globals.UserProfile.LicenseeName = lic.GetUserDataFieldValue("LicenseeName", "#");

						try
						{
							LicenseData LD = await LicensingClient.Retrieve(Globals.UserProfile.Serial, Globals.ApplicationVersion);
							if (Globals.UserProfile.PriorUsage != null) await LicensingClient.PostUsage(Globals.UserProfile.PriorUsage);
							if (LD.MaxLicensedVersion < Globals.ApplicationVersion) Current.Shutdown(-10);
							Globals.UserProfile.UserName = LD.UserName;
							if (LD.Revoked)
							{
								Globals.UserProfile.Serial = "TRIAL";
								Globals.UserProfile.IsTrial = true;
								Globals.UserProfile.IsUsageEnabled = true;
								Globals.UserProfile.License = "";
							}
						}
						catch (Exception)
						{
						}
					}
				}
			}
			else
			{
				if (Globals.UserProfile.IsTrial || Globals.UserProfile.Serial == "TRIAL" || Globals.UserProfile.License == "")
				{
					Globals.InitializeCodeGenerators(Globals.TrialLicense);
					var lic = new LogicNP.CryptoLicensing.CryptoLicense(Globals.TrialLicense, Globals.LicenseVerification);
					Globals.UserProfile.SKU = lic.GetUserDataFieldValue("SKU", "#");
					Globals.UserProfile.LicenseeName = lic.GetUserDataFieldValue("LicenseeName", "#");

					Globals.UserProfile.Serial = "TRIAL";
					Globals.UserProfile.IsTrial = true;
					Globals.UserProfile.IsUsageEnabled = true;
					Globals.UserProfile.License = "";
				}
				else
				{
					Globals.InitializeCodeGenerators(Globals.UserProfile.License);
					var lic = new LogicNP.CryptoLicensing.CryptoLicense(Globals.UserProfile.License, Globals.LicenseVerification);
					Globals.UserProfile.SKU = lic.GetUserDataFieldValue("SKU", "#");
					Globals.UserProfile.LicenseeName = lic.GetUserDataFieldValue("LicenseeName", "#");
				}
			}

#else
			Globals.InitializeCodeGenerators(Globals.DeveloperLicense);
			var dlic = new LogicNP.CryptoLicensing.CryptoLicense(Globals.DeveloperLicense, Globals.LicenseVerification);
			Globals.UserProfile.SKU = dlic.GetUserDataFieldValue("SKU", "#");
			Globals.UserProfile.LicenseeName = dlic.GetUserDataFieldValue("LicenseeName", "#");
			Globals.UserProfile.IsTrial = false;
			Globals.UserProfile.Serial = "DEVELOPER";
			try
			{
				if (Globals.UserProfile.PriorUsage != null && CheckForInternetConnection()) await LicensingClient.PostUsage(Globals.UserProfile.PriorUsage);
			}
			catch (Exception)
			{
			}
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
			Globals.Usage.UserID = System.Utilities.Cryptography.Hash.Compute512Hex(Environment.MachineName + Environment.UserDomainName + "\\" + Environment.UserName);
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

			System.IO.File.WriteAllText(Path.Combine("C:\\", "NPError.txt"), e.Exception.ToString());

			if (!Globals.MainScreen.IsLoaded) return;

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