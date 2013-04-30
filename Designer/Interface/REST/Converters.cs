using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NETPath.Interface.REST
{
	[ValueConversion(typeof(System.Net.DecompressionMethods), typeof(int))]
	public class DecompressionMethodsConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.Net.DecompressionMethods)value;
			if (lt == System.Net.DecompressionMethods.None) return 0;
			if (lt == System.Net.DecompressionMethods.GZip) return 1;
			if (lt == System.Net.DecompressionMethods.Deflate) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.Net.DecompressionMethods.None;
			if (lt == 1) return System.Net.DecompressionMethods.GZip;
			if (lt == 2) return System.Net.DecompressionMethods.Deflate;
			return System.Net.DecompressionMethods.None;
		}
	}

	[ValueConversion(typeof(Projects.CredentialsMode), typeof(int))]
	public class CredentialsModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Projects.CredentialsMode)value;
			if (lt == Projects.CredentialsMode.None) return 0;
			if (lt == Projects.CredentialsMode.Allowed) return 1;
			if (lt == Projects.CredentialsMode.Required) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return Projects.CredentialsMode.None;
			if (lt == 1) return Projects.CredentialsMode.Allowed;
			if (lt == 2) return Projects.CredentialsMode.Required;
			return Projects.CredentialsMode.None;
		}
	}

	[ValueConversion(typeof(Projects.CookieContainerMode), typeof(int))]
	public class CookieContainerModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Projects.CookieContainerMode)value;
			if (lt == Projects.CookieContainerMode.None) return 0;
			if (lt == Projects.CookieContainerMode.Instance) return 1;
			if (lt == Projects.CookieContainerMode.Global) return 2;
			if (lt == Projects.CookieContainerMode.Custom) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return Projects.CookieContainerMode.None;
			if (lt == 1) return Projects.CookieContainerMode.Instance;
			if (lt == 2) return Projects.CookieContainerMode.Global;
			if (lt == 3) return Projects.CookieContainerMode.Custom;
			return Projects.CookieContainerMode.None;
		}
	}

	[ValueConversion(typeof(System.Net.Cache.RequestCacheLevel), typeof(int))]
	public class RequestCacheLevelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.Net.Cache.HttpRequestCacheLevel)value;
			if (lt == System.Net.Cache.HttpRequestCacheLevel.Default) return 0;
			if (lt == System.Net.Cache.HttpRequestCacheLevel.BypassCache) return 1;
			if (lt == System.Net.Cache.HttpRequestCacheLevel.CacheOnly) return 2;
			if (lt == System.Net.Cache.HttpRequestCacheLevel.CacheIfAvailable) return 3;
			if (lt == System.Net.Cache.HttpRequestCacheLevel.Revalidate) return 4;
			if (lt == System.Net.Cache.HttpRequestCacheLevel.Reload) return 5;
			if (lt == System.Net.Cache.HttpRequestCacheLevel.NoCacheNoStore) return 6;
			if (lt == System.Net.Cache.HttpRequestCacheLevel.CacheOrNextCacheOnly) return 7;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.Net.Cache.HttpRequestCacheLevel.Default;
			if (lt == 1) return System.Net.Cache.HttpRequestCacheLevel.BypassCache;
			if (lt == 2) return System.Net.Cache.HttpRequestCacheLevel.CacheOnly;
			if (lt == 3) return System.Net.Cache.HttpRequestCacheLevel.CacheIfAvailable;
			if (lt == 4) return System.Net.Cache.HttpRequestCacheLevel.Revalidate;
			if (lt == 5) return System.Net.Cache.HttpRequestCacheLevel.Reload;
			if (lt == 6) return System.Net.Cache.HttpRequestCacheLevel.NoCacheNoStore;
			if (lt == 7) return System.Net.Cache.HttpRequestCacheLevel.CacheOrNextCacheOnly;
			return System.Net.Cache.HttpRequestCacheLevel.Default;
		}
	}

	[ValueConversion(typeof(System.Net.Security.AuthenticationLevel), typeof(int))]
	public class AuthenticationLevelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.Net.Security.AuthenticationLevel)value;
			if (lt == System.Net.Security.AuthenticationLevel.None) return 0;
			if (lt == System.Net.Security.AuthenticationLevel.MutualAuthRequested) return 1;
			if (lt == System.Net.Security.AuthenticationLevel.MutualAuthRequired) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.Net.Security.AuthenticationLevel.None;
			if (lt == 1) return System.Net.Security.AuthenticationLevel.MutualAuthRequested;
			if (lt == 2) return System.Net.Security.AuthenticationLevel.MutualAuthRequired;
			return System.Net.Security.AuthenticationLevel.None;
		}
	}

	[ValueConversion(typeof(Projects.ContentMode), typeof(int))]
	public class ContentModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Projects.ContentMode)value;
			if (lt == Projects.ContentMode.Default) return 0;
			if (lt == Projects.ContentMode.ByteArray) return 1;
			if (lt == Projects.ContentMode.String) return 2;
			if (lt == Projects.ContentMode.Stream) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return Projects.ContentMode.Default;
			if (lt == 1) return Projects.ContentMode.ByteArray;
			if (lt == 2) return Projects.ContentMode.String;
			if (lt == 3) return Projects.ContentMode.Stream;
			return Projects.ContentMode.Default;
		}
	}

	[ValueConversion(typeof(System.Net.Http.ClientCertificateOption), typeof(int))]
	public class ClientCertificateOptionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.Net.Http.ClientCertificateOption)value;
			if (lt == System.Net.Http.ClientCertificateOption.Manual) return 0;
			if (lt == System.Net.Http.ClientCertificateOption.Automatic) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.Net.Http.ClientCertificateOption.Manual;
			if (lt == 1) return System.Net.Http.ClientCertificateOption.Automatic;
			return System.Net.Http.ClientCertificateOption.Manual;
		}
	}

	[ValueConversion(typeof(System.Security.Principal.TokenImpersonationLevel), typeof(int))]
	public class TokenImpersonationLevelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.Security.Principal.TokenImpersonationLevel)value;
			if (lt == System.Security.Principal.TokenImpersonationLevel.None) return 0;
			if (lt == System.Security.Principal.TokenImpersonationLevel.Anonymous) return 1;
			if (lt == System.Security.Principal.TokenImpersonationLevel.Identification) return 2;
			if (lt == System.Security.Principal.TokenImpersonationLevel.Impersonation) return 3;
			if (lt == System.Security.Principal.TokenImpersonationLevel.Delegation) return 4;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.Security.Principal.TokenImpersonationLevel.None;
			if (lt == 1) return System.Security.Principal.TokenImpersonationLevel.Anonymous;
			if (lt == 2) return System.Security.Principal.TokenImpersonationLevel.Identification;
			if (lt == 3) return System.Security.Principal.TokenImpersonationLevel.Impersonation;
			if (lt == 5) return System.Security.Principal.TokenImpersonationLevel.Delegation;
			return System.Security.Principal.TokenImpersonationLevel.None;
		}
	}
}