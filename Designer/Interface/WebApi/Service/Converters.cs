using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NETPath.Interface.WebApi.Service
{
	[ValueConversion(typeof(Projects.WebApi.RestSerialization), typeof(int))]
	public class RestSerializationConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Projects.WebApi.RestSerialization)value;
			if (lt == Projects.WebApi.RestSerialization.Json) return 0;
			if (lt == Projects.WebApi.RestSerialization.Bson) return 1;
			if (lt == Projects.WebApi.RestSerialization.Xml) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return Projects.WebApi.RestSerialization.Json;
			if (lt == 1) return Projects.WebApi.RestSerialization.Bson;
			if (lt == 2) return Projects.WebApi.RestSerialization.Xml;
			return Projects.WebApi.RestSerialization.Json;
		}
	}

	[ValueConversion(typeof(Projects.WebApi.WebApiMethodVerbs), typeof(int))]
	public class WebApiMethodVerbsConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Projects.WebApi.WebApiMethodVerbs)value;
			if (lt == Projects.WebApi.WebApiMethodVerbs.Get) return 0;
			if (lt == Projects.WebApi.WebApiMethodVerbs.Post) return 1;
			if (lt == Projects.WebApi.WebApiMethodVerbs.Put) return 2;
			if (lt == Projects.WebApi.WebApiMethodVerbs.Delete) return 3;
			if (lt == Projects.WebApi.WebApiMethodVerbs.Head) return 4;
			if (lt == Projects.WebApi.WebApiMethodVerbs.Options) return 5;
			if (lt == Projects.WebApi.WebApiMethodVerbs.Trace) return 6;
			if (lt == Projects.WebApi.WebApiMethodVerbs.Custom) return 7;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return Projects.WebApi.WebApiMethodVerbs.Get;
			if (lt == 1) return Projects.WebApi.WebApiMethodVerbs.Post;
			if (lt == 2) return Projects.WebApi.WebApiMethodVerbs.Put;
			if (lt == 3) return Projects.WebApi.WebApiMethodVerbs.Delete;
			if (lt == 4) return Projects.WebApi.WebApiMethodVerbs.Head;
			if (lt == 5) return Projects.WebApi.WebApiMethodVerbs.Options;
			if (lt == 6) return Projects.WebApi.WebApiMethodVerbs.Trace;
			if (lt == 7) return Projects.WebApi.WebApiMethodVerbs.Custom;
			return Projects.WebApi.WebApiMethodVerbs.Get;
		}
	}

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

	[ValueConversion(typeof(Projects.WebApi.CredentialsMode), typeof(int))]
	public class CredentialsModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Projects.WebApi.CredentialsMode)value;
			if (lt == Projects.WebApi.CredentialsMode.None) return 0;
			if (lt == Projects.WebApi.CredentialsMode.Allowed) return 1;
			if (lt == Projects.WebApi.CredentialsMode.Required) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return Projects.WebApi.CredentialsMode.None;
			if (lt == 1) return Projects.WebApi.CredentialsMode.Allowed;
			if (lt == 2) return Projects.WebApi.CredentialsMode.Required;
			return Projects.WebApi.CredentialsMode.None;
		}
	}

	[ValueConversion(typeof(Projects.WebApi.CookieContainerMode), typeof(int))]
	public class CookieContainerModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Projects.WebApi.CookieContainerMode)value;
			if (lt == Projects.WebApi.CookieContainerMode.None) return 0;
			if (lt == Projects.WebApi.CookieContainerMode.Instance) return 1;
			if (lt == Projects.WebApi.CookieContainerMode.Custom) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return Projects.WebApi.CookieContainerMode.None;
			if (lt == 1) return Projects.WebApi.CookieContainerMode.Instance;
			if (lt == 2) return Projects.WebApi.CookieContainerMode.Custom;
			return Projects.WebApi.CookieContainerMode.None;
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

	[ValueConversion(typeof(Projects.WebApi.ContentMode), typeof(int))]
	public class ContentModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Projects.WebApi.ContentMode)value;
			if (lt == Projects.WebApi.ContentMode.Default) return 0;
			if (lt == Projects.WebApi.ContentMode.ByteArray) return 1;
			if (lt == Projects.WebApi.ContentMode.String) return 2;
			if (lt == Projects.WebApi.ContentMode.Stream) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return Projects.WebApi.ContentMode.Default;
			if (lt == 1) return Projects.WebApi.ContentMode.ByteArray;
			if (lt == 2) return Projects.WebApi.ContentMode.String;
			if (lt == 3) return Projects.WebApi.ContentMode.Stream;
			return Projects.WebApi.ContentMode.Default;
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