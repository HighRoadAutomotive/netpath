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

	[ValueConversion(typeof(Projects.MethodRestVerbs), typeof(int))]
	public class MethodRestVerbsConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Projects.MethodRestVerbs)value;
			if (lt == Projects.MethodRestVerbs.GET) return 0;
			if (lt == Projects.MethodRestVerbs.POST) return 1;
			if (lt == Projects.MethodRestVerbs.PUT) return 2;
			if (lt == Projects.MethodRestVerbs.DELETE) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return Projects.MethodRestVerbs.GET;
			if (lt == 1) return Projects.MethodRestVerbs.POST;
			if (lt == 2) return Projects.MethodRestVerbs.PUT;
			if (lt == 3) return Projects.MethodRestVerbs.DELETE;
			return Projects.MethodRestVerbs.GET;
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

	[ValueConversion(typeof(System.ServiceModel.AddressFilterMode), typeof(int))]
	public class AddressFilterModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.AddressFilterMode)value;
			if (lt == System.ServiceModel.AddressFilterMode.Any) return 0;
			if (lt == System.ServiceModel.AddressFilterMode.Exact) return 1;
			if (lt == System.ServiceModel.AddressFilterMode.Prefix) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.AddressFilterMode.Any;
			if (lt == 1) return System.ServiceModel.AddressFilterMode.Exact;
			if (lt == 2) return System.ServiceModel.AddressFilterMode.Prefix;
			return System.ServiceModel.AddressFilterMode.Any;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.ConcurrencyMode), typeof(int))]
	public class ConcurrencyModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.ConcurrencyMode)value;
			if (lt == System.ServiceModel.ConcurrencyMode.Single) return 0;
			if (lt == System.ServiceModel.ConcurrencyMode.Reentrant) return 1;
			if (lt == System.ServiceModel.ConcurrencyMode.Multiple) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.ConcurrencyMode.Single;
			if (lt == 1) return System.ServiceModel.ConcurrencyMode.Reentrant;
			if (lt == 2) return System.ServiceModel.ConcurrencyMode.Multiple;
			return System.ServiceModel.ConcurrencyMode.Single;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.InstanceContextMode), typeof(int))]
	public class InstanceContextModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.InstanceContextMode)value;
			if (lt == System.ServiceModel.InstanceContextMode.Single) return 0;
			if (lt == System.ServiceModel.InstanceContextMode.PerCall) return 1;
			if (lt == System.ServiceModel.InstanceContextMode.PerSession) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.InstanceContextMode.Single;
			if (lt == 1) return System.ServiceModel.InstanceContextMode.PerCall;
			if (lt == 2) return System.ServiceModel.InstanceContextMode.PerSession;
			return System.ServiceModel.InstanceContextMode.Single;
		}
	}

	[ValueConversion(typeof(System.Transactions.IsolationLevel), typeof(int))]
	public class IsolationLevelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.Transactions.IsolationLevel)value;
			if (lt == System.Transactions.IsolationLevel.Serializable) return 0;
			if (lt == System.Transactions.IsolationLevel.RepeatableRead) return 1;
			if (lt == System.Transactions.IsolationLevel.ReadCommitted) return 2;
			if (lt == System.Transactions.IsolationLevel.ReadUncommitted) return 3;
			if (lt == System.Transactions.IsolationLevel.Snapshot) return 4;
			if (lt == System.Transactions.IsolationLevel.Chaos) return 5;
			if (lt == System.Transactions.IsolationLevel.Unspecified) return 6;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.Transactions.IsolationLevel.Serializable;
			if (lt == 1) return System.Transactions.IsolationLevel.RepeatableRead;
			if (lt == 2) return System.Transactions.IsolationLevel.ReadCommitted;
			if (lt == 3) return System.Transactions.IsolationLevel.ReadCommitted;
			if (lt == 4) return System.Transactions.IsolationLevel.Snapshot;
			if (lt == 5) return System.Transactions.IsolationLevel.Chaos;
			if (lt == 6) return System.Transactions.IsolationLevel.Unspecified;
			return System.Transactions.IsolationLevel.Unspecified;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.ImpersonationOption), typeof(int))]
	public class ImpersonationOptionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.ImpersonationOption)value;
			if (lt == System.ServiceModel.ImpersonationOption.Allowed) return 0;
			if (lt == System.ServiceModel.ImpersonationOption.NotAllowed) return 1;
			if (lt == System.ServiceModel.ImpersonationOption.Required) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.ImpersonationOption.Allowed;
			if (lt == 1) return System.ServiceModel.ImpersonationOption.NotAllowed;
			if (lt == 2) return System.ServiceModel.ImpersonationOption.Required;
			return System.ServiceModel.ImpersonationOption.Allowed;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.ReleaseInstanceMode), typeof(int))]
	public class ReleaseInstanceModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.ReleaseInstanceMode)value;
			if (lt == System.ServiceModel.ReleaseInstanceMode.None) return 0;
			if (lt == System.ServiceModel.ReleaseInstanceMode.AfterCall) return 1;
			if (lt == System.ServiceModel.ReleaseInstanceMode.BeforeAndAfterCall) return 2;
			if (lt == System.ServiceModel.ReleaseInstanceMode.BeforeCall) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.ReleaseInstanceMode.None;
			if (lt == 1) return System.ServiceModel.ReleaseInstanceMode.AfterCall;
			if (lt == 2) return System.ServiceModel.ReleaseInstanceMode.BeforeAndAfterCall;
			if (lt == 3) return System.ServiceModel.ReleaseInstanceMode.BeforeCall;
			return System.ServiceModel.ReleaseInstanceMode.None;
		}
	}

	[ValueConversion(typeof(Projects.TransactionFlowMode), typeof(int))]
	public class TransactionFlowModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Projects.TransactionFlowMode)value;
			if (lt == Projects.TransactionFlowMode.None) return 0;
			if (lt == Projects.TransactionFlowMode.Allowed) return 1;
			if (lt == Projects.TransactionFlowMode.Mandatory) return 2;
			if (lt == Projects.TransactionFlowMode.NotAllowed) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return Projects.TransactionFlowMode.None;
			if (lt == 1) return Projects.TransactionFlowMode.Allowed;
			if (lt == 2) return Projects.TransactionFlowMode.Mandatory;
			if (lt == 3) return Projects.TransactionFlowMode.NotAllowed;
			return Projects.TransactionFlowMode.None;
		}
	}
}