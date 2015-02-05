using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization.Formatters;
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
			if (lt == Projects.WebApi.WebApiMethodVerbs.Patch) return 6;
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
			if (lt == 6) return Projects.WebApi.WebApiMethodVerbs.Patch;
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

	[ValueConversion(typeof(System.Xml.ConformanceLevel), typeof(int))]
	public class XmlConformanceLevelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.Xml.ConformanceLevel)value;
			if (lt == System.Xml.ConformanceLevel.Auto) return 0;
			if (lt == System.Xml.ConformanceLevel.Document) return 1;
			if (lt == System.Xml.ConformanceLevel.Fragment) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.Xml.ConformanceLevel.Auto;
			if (lt == 1) return System.Xml.ConformanceLevel.Document;
			if (lt == 2) return System.Xml.ConformanceLevel.Fragment;
			return System.Xml.ConformanceLevel.Auto;
		}
	}

	[ValueConversion(typeof(System.Xml.NamespaceHandling), typeof(int))]
	public class XmlNamespaceHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.Xml.NamespaceHandling)value;
			if (lt == System.Xml.NamespaceHandling.Default) return 0;
			if (lt == System.Xml.NamespaceHandling.OmitDuplicates) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.Xml.NamespaceHandling.Default;
			if (lt == 1) return System.Xml.NamespaceHandling.OmitDuplicates;
			return System.Xml.NamespaceHandling.Default;
		}
	}

	[ValueConversion(typeof(System.Xml.NewLineHandling), typeof(int))]
	public class XmlNewLineHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.Xml.NewLineHandling)value;
			if (lt == System.Xml.NewLineHandling.None) return 0;
			if (lt == System.Xml.NewLineHandling.Entitize) return 1;
			if (lt == System.Xml.NewLineHandling.Replace) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.Xml.NewLineHandling.None;
			if (lt == 1) return System.Xml.NewLineHandling.Entitize;
			if (lt == 2) return System.Xml.NewLineHandling.Replace;
			return System.Xml.NewLineHandling.None;
		}
	}

	[ValueConversion(typeof(ConstructorHandling), typeof(int))]
	public class JsonConstructorHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (ConstructorHandling)value;
			if (lt == ConstructorHandling.Default) return 0;
			if (lt == ConstructorHandling.AllowNonPublicDefaultConstructor) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return ConstructorHandling.Default;
			if (lt == 1) return ConstructorHandling.AllowNonPublicDefaultConstructor;
			return ConstructorHandling.Default;
		}
	}

	[ValueConversion(typeof(DateFormatHandling), typeof(int))]
	public class JsonDateFormatHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (DateFormatHandling)value;
			if (lt == DateFormatHandling.IsoDateFormat) return 0;
			if (lt == DateFormatHandling.MicrosoftDateFormat) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return DateFormatHandling.IsoDateFormat;
			if (lt == 1) return DateFormatHandling.MicrosoftDateFormat;
			return DateFormatHandling.IsoDateFormat;
		}
	}

	[ValueConversion(typeof(DateParseHandling), typeof(int))]
	public class JsonDateParseHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (DateParseHandling)value;
			if (lt == DateParseHandling.None) return 0;
			if (lt == DateParseHandling.DateTime) return 1;
			if (lt == DateParseHandling.DateTimeOffset) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return DateParseHandling.None;
			if (lt == 1) return DateParseHandling.DateTime;
			if (lt == 2) return DateParseHandling.DateTimeOffset;
			return DateParseHandling.None;
		}
	}

	[ValueConversion(typeof(DateTimeZoneHandling), typeof(int))]
	public class JsonDateTimeZoneHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (DateTimeZoneHandling)value;
			if (lt == DateTimeZoneHandling.Local) return 0;
			if (lt == DateTimeZoneHandling.Utc) return 1;
			if (lt == DateTimeZoneHandling.Unspecified) return 2;
			if (lt == DateTimeZoneHandling.RoundtripKind) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return DateTimeZoneHandling.Local;
			if (lt == 1) return DateTimeZoneHandling.Utc;
			if (lt == 2) return DateTimeZoneHandling.Unspecified;
			if (lt == 3) return DateTimeZoneHandling.RoundtripKind;
			return DateTimeZoneHandling.Local;
		}
	}

	[ValueConversion(typeof(DefaultValueHandling), typeof(int))]
	public class JsonDefaultValueHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (DefaultValueHandling)value;
			if (lt == DefaultValueHandling.Include) return 0;
			if (lt == DefaultValueHandling.Ignore) return 1;
			if (lt == DefaultValueHandling.Populate) return 2;
			if (lt == DefaultValueHandling.IgnoreAndPopulate) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return DefaultValueHandling.Include;
			if (lt == 1) return DefaultValueHandling.Ignore;
			if (lt == 2) return DefaultValueHandling.Populate;
			if (lt == 3) return DefaultValueHandling.IgnoreAndPopulate;
			return DefaultValueHandling.Ignore;
		}
	}

	[ValueConversion(typeof(FloatFormatHandling), typeof(int))]
	public class JsonFloatFormatHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (FloatFormatHandling)value;
			if (lt == FloatFormatHandling.String) return 0;
			if (lt == FloatFormatHandling.Symbol) return 1;
			if (lt == FloatFormatHandling.DefaultValue) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return FloatFormatHandling.String;
			if (lt == 1) return FloatFormatHandling.Symbol;
			if (lt == 2) return FloatFormatHandling.DefaultValue;
			return FloatFormatHandling.String;
		}
	}

	[ValueConversion(typeof(FloatParseHandling), typeof(int))]
	public class JsonFloatParseHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (FloatParseHandling)value;
			if (lt == FloatParseHandling.Double) return 0;
			if (lt == FloatParseHandling.Decimal) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return FloatParseHandling.Double;
			if (lt == 1) return FloatParseHandling.Decimal;
			return FloatParseHandling.Double;
		}
	}

	[ValueConversion(typeof(Formatting), typeof(int))]
	public class JsonFormattingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Formatting)value;
			if (lt == Formatting.None) return 0;
			if (lt == Formatting.Indented) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return Formatting.None;
			if (lt == 1) return Formatting.Indented;
			return Formatting.None;
		}
	}

	[ValueConversion(typeof(MemberSerialization), typeof(int))]
	public class JsonMemberSerializationConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (MemberSerialization)value;
			if (lt == MemberSerialization.OptOut) return 0;
			if (lt == MemberSerialization.OptIn) return 1;
			if (lt == MemberSerialization.Fields) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return MemberSerialization.OptOut;
			if (lt == 1) return MemberSerialization.OptIn;
			if (lt == 2) return MemberSerialization.Fields;
			return MemberSerialization.OptOut;
		}
	}

	[ValueConversion(typeof(MetadataPropertyHandling), typeof(int))]
	public class JsonMetadataPropertyHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (MetadataPropertyHandling)value;
			if (lt == MetadataPropertyHandling.Default) return 0;
			if (lt == MetadataPropertyHandling.ReadAhead) return 1;
			if (lt == MetadataPropertyHandling.Ignore) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return MetadataPropertyHandling.Default;
			if (lt == 1) return MetadataPropertyHandling.ReadAhead;
			if (lt == 2) return MetadataPropertyHandling.Ignore;
			return MetadataPropertyHandling.Default;
		}
	}

	[ValueConversion(typeof(MissingMemberHandling), typeof(int))]
	public class JsonMissingMemberHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (MissingMemberHandling)value;
			if (lt == MissingMemberHandling.Ignore) return 0;
			if (lt == MissingMemberHandling.Error) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return MissingMemberHandling.Ignore;
			if (lt == 1) return MissingMemberHandling.Error;
			return MissingMemberHandling.Ignore;
		}
	}

	[ValueConversion(typeof(NullValueHandling), typeof(int))]
	public class JsonNullValueHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (NullValueHandling)value;
			if (lt == NullValueHandling.Include) return 0;
			if (lt == NullValueHandling.Ignore) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return NullValueHandling.Include;
			if (lt == 1) return NullValueHandling.Ignore;
			return NullValueHandling.Include;
		}
	}

	[ValueConversion(typeof(ObjectCreationHandling), typeof(int))]
	public class JsonObjectCreationHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (ObjectCreationHandling)value;
			if (lt == ObjectCreationHandling.Auto) return 0;
			if (lt == ObjectCreationHandling.Reuse) return 1;
			if (lt == ObjectCreationHandling.Replace) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return ObjectCreationHandling.Auto;
			if (lt == 1) return ObjectCreationHandling.Reuse;
			if (lt == 2) return ObjectCreationHandling.Replace;
			return ObjectCreationHandling.Auto;
		}
	}

	[ValueConversion(typeof(PreserveReferencesHandling), typeof(int))]
	public class JsonPreserveReferencesHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (PreserveReferencesHandling)value;
			if (lt == PreserveReferencesHandling.None) return 0;
			if (lt == PreserveReferencesHandling.Objects) return 1;
			if (lt == PreserveReferencesHandling.Arrays) return 2;
			if (lt == PreserveReferencesHandling.All) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return PreserveReferencesHandling.None;
			if (lt == 1) return PreserveReferencesHandling.Objects;
			if (lt == 2) return PreserveReferencesHandling.Arrays;
			if (lt == 3) return PreserveReferencesHandling.All;
			return PreserveReferencesHandling.None;
		}
	}

	[ValueConversion(typeof(ReferenceLoopHandling), typeof(int))]
	public class JsonReferenceLoopHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (ReferenceLoopHandling)value;
			if (lt == ReferenceLoopHandling.Error) return 0;
			if (lt == ReferenceLoopHandling.Ignore) return 1;
			if (lt == ReferenceLoopHandling.Serialize) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return ReferenceLoopHandling.Error;
			if (lt == 1) return ReferenceLoopHandling.Ignore;
			if (lt == 2) return ReferenceLoopHandling.Serialize;
			return ReferenceLoopHandling.Error;
		}
	}

	[ValueConversion(typeof(StringEscapeHandling), typeof(int))]
	public class JsonStringEscapeHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (StringEscapeHandling)value;
			if (lt == StringEscapeHandling.Default) return 0;
			if (lt == StringEscapeHandling.EscapeNonAscii) return 1;
			if (lt == StringEscapeHandling.EscapeHtml) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return StringEscapeHandling.Default;
			if (lt == 1) return StringEscapeHandling.EscapeNonAscii;
			if (lt == 2) return StringEscapeHandling.EscapeHtml;
			return StringEscapeHandling.Default;
		}
	}

	[ValueConversion(typeof(FormatterAssemblyStyle), typeof(int))]
	public class JsonFormatterAssemblyStyleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (FormatterAssemblyStyle)value;
			if (lt == FormatterAssemblyStyle.Simple) return 0;
			if (lt == FormatterAssemblyStyle.Full) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return FormatterAssemblyStyle.Simple;
			if (lt == 1) return FormatterAssemblyStyle.Full;
			return FormatterAssemblyStyle.Simple;
		}
	}

	[ValueConversion(typeof(TypeNameHandling), typeof(int))]
	public class JsonTypeNameHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (TypeNameHandling)value;
			if (lt == TypeNameHandling.None) return 0;
			if (lt == TypeNameHandling.Objects) return 1;
			if (lt == TypeNameHandling.Arrays) return 2;
			if (lt == TypeNameHandling.All) return 3;
			if (lt == TypeNameHandling.Auto) return 4;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return TypeNameHandling.None;
			if (lt == 1) return TypeNameHandling.Objects;
			if (lt == 2) return TypeNameHandling.Arrays;
			if (lt == 3) return TypeNameHandling.All;
			if (lt == 4) return TypeNameHandling.Auto;
			return TypeNameHandling.None;
		}
	}

}