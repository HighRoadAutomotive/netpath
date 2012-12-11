using System;
using System.Globalization;
using System.Windows.Data;

namespace NETPath.Interface.Service
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

	[ValueConversion(typeof(System.ServiceModel.Web.WebMessageBodyStyle), typeof(int))]
	public class WebMessageBodyStyleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.Web.WebMessageBodyStyle)value;
			if (lt == System.ServiceModel.Web.WebMessageBodyStyle.Bare) return 0;
			if (lt == System.ServiceModel.Web.WebMessageBodyStyle.Wrapped) return 1;
			if (lt == System.ServiceModel.Web.WebMessageBodyStyle.WrappedRequest) return 2;
			if (lt == System.ServiceModel.Web.WebMessageBodyStyle.WrappedResponse) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.Web.WebMessageBodyStyle.Bare;
			if (lt == 1) return System.ServiceModel.Web.WebMessageBodyStyle.Wrapped;
			if (lt == 2) return System.ServiceModel.Web.WebMessageBodyStyle.WrappedRequest;
			if (lt == 3) return System.ServiceModel.Web.WebMessageBodyStyle.WrappedResponse;
			return System.ServiceModel.Web.WebMessageBodyStyle.Bare;
		}
	}

	[ValueConversion(typeof(Projects.MethodRESTVerbs), typeof(int))]
	public class MethodRESTVerbsConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Projects.MethodRESTVerbs)value;
			if (lt == Projects.MethodRESTVerbs.GET) return 0;
			if (lt == Projects.MethodRESTVerbs.POST) return 1;
			if (lt == Projects.MethodRESTVerbs.PUT) return 2;
			if (lt == Projects.MethodRESTVerbs.DELETE) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return Projects.MethodRESTVerbs.GET;
			if (lt == 1) return Projects.MethodRESTVerbs.POST;
			if (lt == 2) return Projects.MethodRESTVerbs.PUT;
			if (lt == 3) return Projects.MethodRESTVerbs.DELETE;
			return Projects.MethodRESTVerbs.GET;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.Web.WebMessageFormat), typeof(int))]
	public class WebMessageFormatConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.Web.WebMessageFormat)value;
			if (lt == System.ServiceModel.Web.WebMessageFormat.Xml) return 0;
			if (lt == System.ServiceModel.Web.WebMessageFormat.Json) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.Web.WebMessageFormat.Xml;
			if (lt == 1) return System.ServiceModel.Web.WebMessageFormat.Json;
			return System.ServiceModel.Web.WebMessageFormat.Xml;
		}
	}
}