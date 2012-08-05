using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Windows;

namespace WCFArchitect.Options
{
	internal class Licensing
	{
		public static bool Validate()
		{
#if PROFESSIONAL || STANDARD
			if (TL.Key == "" && (TL.Authorization != "" || TL.Authorization != null))
			{
				TL.Authorization = "";
			}

			if(TL.Key == "" || TL.Authorization=="")
			{
				DateTime CheckDate = GetNISTDate(true);

				if (CheckDate == DateTime.MaxValue)
				{
					MessageBox.Show("WCF Architect was unable to connect to the internet to verify the activation. Please connect to the internet to continue.", "Unable to Verify Date", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Stop);
					return false;
				}
			}

			if (TL.Key != "")
			{
				try
				{
					Globals.LicenseInfo = Prospective.Server.Licensing.LicenseClient.GetLicenseKeyInfo(TL.Key);

					Version nv = new Version(Globals.LicenseInfo.LatestVersion.Major, Globals.LicenseInfo.LatestVersion.Minor, Globals.LicenseInfo.LatestVersion.Build, Globals.LicenseInfo.LatestVersion.Revision);
					Version cv = new Version(System.Diagnostics.FileVersionInfo.GetVersionInfo(Globals.ApplicationPath + "WCFArchitect.exe").FileVersion);
					if (nv > cv)
					{
						Globals.IsNewVersionAvailable = true;
						Globals.NewVersionPath = Globals.LicenseInfo.LatestVersionLink;
					}

				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString(), "Unable to retrieve the license information for this computer!");
					return false;
				}

				try
				{
#if PROFESSIONAL
					if (Prospective.Server.Licensing.LicenseClient.VerifyLicenseKey(TL.Key, TL.Authorization, "A2BDD3211835F004B56B18A164E3C78668DE0638972340CC46AC164F945B6FD0D0E33D1799987C741EB8C6D731A72BC5CE5F658157EA99E9AE5C453433AF9C1D") != "Valid")
#elif STANDARD
					if (Prospective.Server.Licensing.LicenseClient.VerifyLicenseKey(TL.Key, TL.Authorization, "F9957099B7836A262A2609F9F2375225F7FB936A0EE43B90493C8DF974FA7EA3FDD3BCF6DA7ABD73DA9D13F9F92F6432ED8D4766F46878AB218CFCC3550F9F76") != "Valid")
#endif
					{
#if PROFESSIONAL
						string KeyAuth = Prospective.Server.Licensing.LicenseClient.AuthorizeComputer(TL.Key, "A2BDD3211835F004B56B18A164E3C78668DE0638972340CC46AC164F945B6FD0D0E33D1799987C741EB8C6D731A72BC5CE5F658157EA99E9AE5C453433AF9C1D");
#elif STANDARD
						string KeyAuth = Prospective.Server.Licensing.LicenseClient.AuthorizeComputer(TL.Key, "F9957099B7836A262A2609F9F2375225F7FB936A0EE43B90493C8DF974FA7EA3FDD3BCF6DA7ABD73DA9D13F9F92F6432ED8D4766F46878AB218CFCC3550F9F76");
#endif
						if (KeyAuth.StartsWith("ERROR:") == true)
						{
							MessageBox.Show("WCF Architect was unable to verify the license key provided. Please try a different key and try again." + Environment.NewLine + Environment.NewLine + KeyAuth);
							return false;
						}

						TL.Authorization = KeyAuth;
						Globals.LicenseKeySpace.Save();
					}
					if (Globals.LicenseInfo == null) Globals.LicenseInfo = Prospective.Server.Licensing.LicenseClient.GetLicenseKeyInfo(TL.Key);
					Globals.LicenseKey = TL;
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString(), "Unable to authenticate this computer!");
					return false;
				}
			}
			else
			{
				try
				{
					MessageBox.Show("WARNING: This product is not yet activated. You will have seven (7) days to activate the product before you must contact support to continue using this software.", "Product Not Activated", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					if (Prospective.Server.Licensing.LicenseClient.VerifyLicenseKey("41CAC5445E3A4FBA8097C96FA", TL.Authorization, "8C8ECC9E9132277B3C4CC15632A6F0D228DDA01A84010734E6777E72D129CC3663C803A0782646605983B0BFE6339AC45DB0A9341197251378751C24FEAB0548") != "Valid")
					{
						string KeyAuth = Prospective.Server.Licensing.LicenseClient.AuthorizeComputer("41CAC5445E3A4FBA8097C96FA", "8C8ECC9E9132277B3C4CC15632A6F0D228DDA01A84010734E6777E72D129CC3663C803A0782646605983B0BFE6339AC45DB0A9341197251378751C24FEAB0548");
						if (KeyAuth.StartsWith("ERROR:") == true)
						{
							MessageBox.Show("WCF Architect was unable to verify the license key provided. Please try a different key and try again." + Environment.NewLine + Environment.NewLine + KeyAuth);
							return false;
						}
						TL.Authorization = KeyAuth;
						Globals.LicenseKeySpace.Save();
					}
					if (Globals.LicenseInfo == null) Globals.LicenseInfo = Prospective.Server.Licensing.LicenseClient.GetTrialLicenseKeyInfo("41CAC5445E3A4FBA8097C96FA", TL.Authorization);
					Globals.LicenseKey = TL;
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString(), "Unable to authenticate this computer!");
					return false;
				}
			}
#elif TRIAL
			try
			{
				Globals.LicenseInfo = Prospective.Server.Licensing.LicenseClient.GetTrialLicenseKeyInfo("41CAC5445E3A4FBA8097C96FA", TL.Authorization);
				if (Globals.LicenseInfo != null)
				{
					if (Globals.LicenseInfo.ExpirationDate < DateTime.Now)
					{
						if (Globals.LicenseInfo.ExpirationURL != "" && Globals.LicenseInfo.ExpirationURL != null)
						{
							if (Globals.LicenseInfo.ExpirationMessage != "" && Globals.LicenseInfo.ExpirationMessage != null)
							{
								if (MessageBox.Show(Globals.LicenseInfo.ExpirationMessage, "License Expiration Notice", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
									System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(Globals.LicenseInfo.ExpirationURL));
								Application.Current.Shutdown(0);
								return false;
							}
							else
							{
								System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(Globals.LicenseInfo.ExpirationURL));
								Application.Current.Shutdown(0);
								return false;
							}
						}
						Application.Current.Shutdown(0);
						return false;
					}
					else
					{
						MessageBox.Show("Your trial will expire on " + Globals.LicenseInfo.ExpirationDate.ToShortDateString() + ".", "Trial Notice", MessageBoxButton.OK, MessageBoxImage.Information);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Unable to retrieve the license information for this computer!");
				return false;
			}

			try
			{
				if (Prospective.Server.Licensing.LicenseClient.VerifyLicenseKey("41CAC5445E3A4FBA8097C96FA", TL.Authorization, "8C8ECC9E9132277B3C4CC15632A6F0D228DDA01A84010734E6777E72D129CC3663C803A0782646605983B0BFE6339AC45DB0A9341197251378751C24FEAB0548") != "Valid")
				{
					string KeyAuth = Prospective.Server.Licensing.LicenseClient.AuthorizeComputer("41CAC5445E3A4FBA8097C96FA", "8C8ECC9E9132277B3C4CC15632A6F0D228DDA01A84010734E6777E72D129CC3663C803A0782646605983B0BFE6339AC45DB0A9341197251378751C24FEAB0548");
					if (KeyAuth.StartsWith("ERROR:") == true)
					{
						MessageBox.Show("WCF Architect was unable to authenticate the trial license provided for the following reason:" + Environment.NewLine + Environment.NewLine + KeyAuth);
						return false;
					}
					TL.Authorization = KeyAuth;
					Globals.LicenseKeySpace.Save();
				}
				if (Globals.LicenseInfo == null) Globals.LicenseInfo = Prospective.Server.Licensing.LicenseClient.GetTrialLicenseKeyInfo("41CAC5445E3A4FBA8097C96FA", TL.Authorization);
				Globals.LicenseKey = TL;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Unable to authenticate this computer!");
				return false;
			}
#endif
			return true;
		}

		public static bool IsActivated()
		{
			//Enable this in RTM builds.
#if PROFESSIONAL || STANDARD
			if (Globals.LicenseKey.Authorization == "" || Globals.LicenseKey.Authorization == null) return false;
			return true;
#elif DEVELOPER
			return true;
#else
			return false;
#endif
		}

	}
}
