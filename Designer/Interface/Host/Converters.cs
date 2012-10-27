using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace WCFArchitect.Interface.Host
{
	[ValueConversion(typeof(System.ServiceModel.Description.PrincipalPermissionMode), typeof(int))]
	public class PrincipalPermissionModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.Description.PrincipalPermissionMode)value;
			if (lt == System.ServiceModel.Description.PrincipalPermissionMode.None) return 0;
			if (lt == System.ServiceModel.Description.PrincipalPermissionMode.UseWindowsGroups) return 1;
			if (lt == System.ServiceModel.Description.PrincipalPermissionMode.UseAspNetRoles) return 2;
			if (lt == System.ServiceModel.Description.PrincipalPermissionMode.Custom) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.Description.PrincipalPermissionMode.None;
			if (lt == 1) return System.ServiceModel.Description.PrincipalPermissionMode.UseWindowsGroups;
			if (lt == 2) return System.ServiceModel.Description.PrincipalPermissionMode.UseAspNetRoles;
			if (lt == 3) return System.ServiceModel.Description.PrincipalPermissionMode.Custom;
			return System.ServiceModel.Description.PrincipalPermissionMode.None;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.Security.X509CertificateValidationMode), typeof(int))]
	public class X509CertificateValidationModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.Security.X509CertificateValidationMode)value;
			if (lt == System.ServiceModel.Security.X509CertificateValidationMode.None) return 0;
			if (lt == System.ServiceModel.Security.X509CertificateValidationMode.PeerTrust) return 1;
			if (lt == System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust) return 2;
			if (lt == System.ServiceModel.Security.X509CertificateValidationMode.PeerOrChainTrust) return 3;
			if (lt == System.ServiceModel.Security.X509CertificateValidationMode.Custom) return 4;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.Security.X509CertificateValidationMode.None;
			if (lt == 1) return System.ServiceModel.Security.X509CertificateValidationMode.PeerTrust;
			if (lt == 2) return System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
			if (lt == 3) return System.ServiceModel.Security.X509CertificateValidationMode.PeerOrChainTrust;
			if (lt == 4) return System.ServiceModel.Security.X509CertificateValidationMode.Custom;
			return System.ServiceModel.Security.X509CertificateValidationMode.None;
		}
	}

	[ValueConversion(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), typeof(int))]
	public class X509RevocationModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.Security.Cryptography.X509Certificates.X509RevocationMode)value;
			if (lt == System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck) return 0;
			if (lt == System.Security.Cryptography.X509Certificates.X509RevocationMode.Online) return 1;
			if (lt == System.Security.Cryptography.X509Certificates.X509RevocationMode.Offline) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;
			if (lt == 1) return System.Security.Cryptography.X509Certificates.X509RevocationMode.Online;
			if (lt == 2) return System.Security.Cryptography.X509Certificates.X509RevocationMode.Offline;
			return System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;
		}
	}

	[ValueConversion(typeof(System.Security.Cryptography.X509Certificates.StoreLocation), typeof(int))]
	public class StoreLocationConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.Security.Cryptography.X509Certificates.StoreLocation)value;
			if (lt == System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser) return 0;
			if (lt == System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
			if (lt == 1) return System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine;
			return System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.Security.UserNamePasswordValidationMode), typeof(int))]
	public class UserNamePasswordValidationModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.Security.UserNamePasswordValidationMode)value;
			if (lt == System.ServiceModel.Security.UserNamePasswordValidationMode.Windows) return 0;
			if (lt == System.ServiceModel.Security.UserNamePasswordValidationMode.MembershipProvider) return 1;
			if (lt == System.ServiceModel.Security.UserNamePasswordValidationMode.Custom) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.Security.UserNamePasswordValidationMode.Windows;
			if (lt == 1) return System.ServiceModel.Security.UserNamePasswordValidationMode.MembershipProvider;
			if (lt == 2) return System.ServiceModel.Security.UserNamePasswordValidationMode.Custom;
			return System.ServiceModel.Security.UserNamePasswordValidationMode.Windows;
		}
	}

	[ValueConversion(typeof(Projects.HostEndpointIdentityType), typeof(int))]
	public class HostEndpointIdentityTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Projects.HostEndpointIdentityType)value;
			if (lt == Projects.HostEndpointIdentityType.Anonymous) return 0;
			if (lt == Projects.HostEndpointIdentityType.DNS) return 1;
			if (lt == Projects.HostEndpointIdentityType.RSA) return 2;
			if (lt == Projects.HostEndpointIdentityType.RSAX509) return 3;
			if (lt == Projects.HostEndpointIdentityType.SPN) return 4;
			if (lt == Projects.HostEndpointIdentityType.UPN) return 5;
			if (lt == Projects.HostEndpointIdentityType.X509) return 6;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return Projects.HostEndpointIdentityType.Anonymous;
			if (lt == 1) return Projects.HostEndpointIdentityType.DNS;
			if (lt == 2) return Projects.HostEndpointIdentityType.RSA;
			if (lt == 3) return Projects.HostEndpointIdentityType.RSAX509;
			if (lt == 4) return Projects.HostEndpointIdentityType.SPN;
			if (lt == 5) return Projects.HostEndpointIdentityType.UPN;
			if (lt == 6) return Projects.HostEndpointIdentityType.X509;
			return Projects.HostEndpointIdentityType.Anonymous;
		}
	}
}