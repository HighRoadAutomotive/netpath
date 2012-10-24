using System;
using System.Globalization;
using System.Windows.Data;

namespace WCFArchitect.Interface.Service
{
	[ValueConversion(typeof(System.Net.Security.ProtectionLevel), typeof(int))]
	public class ProtectionLevelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.Net.Security.ProtectionLevel)value;
			if (lt == System.Net.Security.ProtectionLevel.None) return 0;
			if (lt == System.Net.Security.ProtectionLevel.Sign) return 1;
			if (lt == System.Net.Security.ProtectionLevel.EncryptAndSign) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.Net.Security.ProtectionLevel.None;
			if (lt == 1) return System.Net.Security.ProtectionLevel.Sign;
			if (lt == 2) return System.Net.Security.ProtectionLevel.EncryptAndSign;
			return System.Net.Security.ProtectionLevel.None;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.SessionMode), typeof(int))]
	public class SessionModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.SessionMode)value;
			if (lt == System.ServiceModel.SessionMode.Allowed) return 0;
			if (lt == System.ServiceModel.SessionMode.Required) return 1;
			if (lt == System.ServiceModel.SessionMode.NotAllowed) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.SessionMode.Allowed;
			if (lt == 1) return System.ServiceModel.SessionMode.Required;
			if (lt == 2) return System.ServiceModel.SessionMode.NotAllowed;
			return System.ServiceModel.SessionMode.Allowed;
		}
	}

	[ValueConversion(typeof(System.Net.Security.ProtectionLevel), typeof(int))]
	public class ServiceAsynchronyModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Projects.ServiceAsynchronyMode)value;
			if (lt == Projects.ServiceAsynchronyMode.Default) return 0;
			if (lt == Projects.ServiceAsynchronyMode.Client) return 1;
			if (lt == Projects.ServiceAsynchronyMode.Server) return 2;
			if (lt == Projects.ServiceAsynchronyMode.Both) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return Projects.ServiceAsynchronyMode.Default;
			if (lt == 1) return Projects.ServiceAsynchronyMode.Client;
			if (lt == 2) return Projects.ServiceAsynchronyMode.Server;
			if (lt == 3) return Projects.ServiceAsynchronyMode.Both;
			return Projects.ServiceAsynchronyMode.Default;
		}
	}
}