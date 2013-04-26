using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System.ServiceModel
{
	public abstract class RESTClientBase
	{
		public static CookieContainer GlobalCookies { get; private set; }
		static RESTClientBase()
		{
			GlobalCookies = new CookieContainer();
		}

		public Uri BaseURI { get; protected set; }
		public CookieContainer Cookies { get; private set; }
		public CredentialCache CredentialCache { get; protected set; }
		public NetworkCredential Credentials { get; protected set; }
		public IWebProxy Proxy { get; protected set; }

		protected RESTClientBase()
		{
			Cookies = new CookieContainer();
			Credentials = null;
		}

		protected RESTClientBase(string BaseURI, CookieContainer Cookies = null, NetworkCredential Credentials = null, CredentialCache CredentialCache = null, IWebProxy Proxy = null)
		{
			this.BaseURI = new Uri(BaseURI);
			this.Cookies = Cookies ?? new CookieContainer();
			this.Credentials = Credentials;
			this.CredentialCache = CredentialCache;
			this.Proxy = Proxy;
		}

		public string ParseURI(string Uri, IDictionary<string, object> Parameters)
		{
			string t = Uri;
			foreach (KeyValuePair<string, object> kvp in Parameters)
				t = Regex.Replace(t, string.Format("\\{{{0}\\}}", kvp.Key), kvp.Value.ToString(), RegexOptions.IgnoreCase);
			return t;
		}
	}

	public sealed class RESTHTTPWebConfig
	{
		public CookieContainer CookieContainer { get; set; }
		public WebHeaderCollection Headers { get; private set; }
		public WebProxy Proxy { get; set; }
		public X509CertificateCollection ClientCertificates { get; set; }
		public NetworkCredential NetworkCredential { get; set; }
		public CredentialCache CredentialCache { get; set; }

		public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }
		public HttpContinueDelegate Continuation { get; set; }

		public string Accept { get { return Headers[HttpRequestHeader.Accept]; } set { Headers[HttpRequestHeader.Accept] = value; } }
		public string AcceptCharset { get { return Headers[HttpRequestHeader.AcceptCharset]; } set { Headers[HttpRequestHeader.AcceptCharset] = value; } }
		public string AcceptEncoding { get { return Headers[HttpRequestHeader.AcceptEncoding]; } set { Headers[HttpRequestHeader.AcceptEncoding] = value; } }
		public string AcceptLanguage { get { return Headers[HttpRequestHeader.AcceptLanguage]; } set { Headers[HttpRequestHeader.AcceptLanguage] = value; } }
		public string Allow { get { return Headers[HttpRequestHeader.Allow]; } set { Headers[HttpRequestHeader.Allow] = value; } }
		public string Authorization { get { return Headers[HttpRequestHeader.Authorization]; } set { Headers[HttpRequestHeader.Authorization] = value; } }
		public string CacheControl { get { return Headers[HttpRequestHeader.CacheControl]; } set { Headers[HttpRequestHeader.CacheControl] = value; } }
		public string Connection { get { return Headers[HttpRequestHeader.Connection]; } set { Headers[HttpRequestHeader.Connection] = value; } }
		public string ContentLength { get { return Headers[HttpRequestHeader.ContentLength]; } set { Headers[HttpRequestHeader.ContentLength] = value; } }
		public string ContentEncoding { get { return Headers[HttpRequestHeader.ContentEncoding]; } set { Headers[HttpRequestHeader.ContentEncoding] = value; } }
		public string ContentLanguage { get { return Headers[HttpRequestHeader.ContentLanguage]; } set { Headers[HttpRequestHeader.ContentLanguage] = value; } }
		public string ContentLocation { get { return Headers[HttpRequestHeader.ContentLocation]; } set { Headers[HttpRequestHeader.ContentLocation] = value; } }
		public string ContentMd5 { get { return Headers[HttpRequestHeader.ContentMd5]; } set { Headers[HttpRequestHeader.ContentMd5] = value; } }
		public string ContentRange { get { return Headers[HttpRequestHeader.ContentRange]; } set { Headers[HttpRequestHeader.ContentRange] = value; } }
		public string ContentType { get { return Headers[HttpRequestHeader.ContentType]; } set { Headers[HttpRequestHeader.ContentType] = value; } }
		public string Cookie { get { return Headers[HttpRequestHeader.Cookie]; } set { Headers[HttpRequestHeader.Cookie] = value; } }
		public string Date { get { return Headers[HttpRequestHeader.Date]; } set { Headers[HttpRequestHeader.Date] = value; } }
		public string Expires { get { return Headers[HttpRequestHeader.Expires]; } set { Headers[HttpRequestHeader.Expires] = value; } }
		public string Expect { get { return Headers[HttpRequestHeader.Expect]; } set { Headers[HttpRequestHeader.Expect] = value; } }
		public string From { get { return Headers[HttpRequestHeader.From]; } set { Headers[HttpRequestHeader.From] = value; } }
		public string Host { get { return Headers[HttpRequestHeader.Host]; } set { Headers[HttpRequestHeader.Host] = value; } }
		public string IfMatch { get { return Headers[HttpRequestHeader.IfMatch]; } set { Headers[HttpRequestHeader.IfMatch] = value; } }
		public string IfModifiedSince { get { return Headers[HttpRequestHeader.IfModifiedSince]; } set { Headers[HttpRequestHeader.IfModifiedSince] = value; } }
		public string IfNoneMatch { get { return Headers[HttpRequestHeader.IfNoneMatch]; } set { Headers[HttpRequestHeader.IfNoneMatch] = value; } }
		public string IfRange { get { return Headers[HttpRequestHeader.IfRange]; } set { Headers[HttpRequestHeader.IfRange] = value; } }
		public string IfUnmodifiedSince { get { return Headers[HttpRequestHeader.IfUnmodifiedSince]; } set { Headers[HttpRequestHeader.IfUnmodifiedSince] = value; } }
		public string KeepAlive { get { return Headers[HttpRequestHeader.KeepAlive]; } set { Headers[HttpRequestHeader.KeepAlive] = value; } }
		public string LastModified { get { return Headers[HttpRequestHeader.LastModified]; } set { Headers[HttpRequestHeader.LastModified] = value; } }
		public string MaxForwards { get { return Headers[HttpRequestHeader.MaxForwards]; } set { Headers[HttpRequestHeader.MaxForwards] = value; } }
		public string Pragma { get { return Headers[HttpRequestHeader.Pragma]; } set { Headers[HttpRequestHeader.Pragma] = value; } }
		public string ProxyAuthorization { get { return Headers[HttpRequestHeader.ProxyAuthorization]; } set { Headers[HttpRequestHeader.ProxyAuthorization] = value; } }
		public string Range { get { return Headers[HttpRequestHeader.Range]; } set { Headers[HttpRequestHeader.Range] = value; } }
		public string Referer { get { return Headers[HttpRequestHeader.Referer]; } set { Headers[HttpRequestHeader.Referer] = value; } }
		public string Te { get { return Headers[HttpRequestHeader.Te]; } set { Headers[HttpRequestHeader.Te] = value; } }
		public string Trailer { get { return Headers[HttpRequestHeader.Trailer]; } set { Headers[HttpRequestHeader.Trailer] = value; } }
		public string TransferEncoding { get { return Headers[HttpRequestHeader.TransferEncoding]; } set { Headers[HttpRequestHeader.TransferEncoding] = value; } }
		public string Translate { get { return Headers[HttpRequestHeader.Translate]; } set { Headers[HttpRequestHeader.Translate] = value; } }
		public string UserAgent { get { return Headers[HttpRequestHeader.UserAgent]; } set { Headers[HttpRequestHeader.UserAgent] = value; } }
		public string Upgrade { get { return Headers[HttpRequestHeader.Upgrade]; } set { Headers[HttpRequestHeader.Upgrade] = value; } }
		public string Via { get { return Headers[HttpRequestHeader.Via]; } set { Headers[HttpRequestHeader.Via] = value; } }
		public string Warning { get { return Headers[HttpRequestHeader.Warning]; } set { Headers[HttpRequestHeader.Warning] = value; } }

		public RESTHTTPWebConfig()
		{
			Headers = new WebHeaderCollection();
		}

		public HttpWebRequest CreateRequest(string RequestUri, CookieContainer CookieContainer = null, bool UseHTTP10 = false)
		{
			var t = (HttpWebRequest)WebRequest.Create(new Uri(RequestUri, UriKind.Absolute));

			if (UseHTTP10) t.ProtocolVersion = new Version(1, 0);
			if (NetworkCredential != null) t.Credentials = NetworkCredential;
			if (CredentialCache != null) t.Credentials = CredentialCache;
			t.CookieContainer = this.CookieContainer ?? CookieContainer;
			t.ClientCertificates = ClientCertificates;
			t.Proxy = Proxy;
			t.Headers = Headers;
			t.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
			t.ContinueDelegate = Continuation;
			
			return t;
		}
	}

	public sealed class RESTHTTPClientConfig
	{
		//Request Headers
		public List<MediaTypeWithQualityHeaderValue> Accept { get; private set; }
		public List<StringWithQualityHeaderValue> AcceptCharset { get; private set; }
		public List<StringWithQualityHeaderValue> AcceptEncoding { get; private set; }
		public List<StringWithQualityHeaderValue> AcceptLanguage { get; private set; }
		public AuthenticationHeaderValue Authorization { get; private set; }
		public CacheControlHeaderValue CacheControl { get; private set; }
		public List<string> Connection { get; private set; }
		public List<string> Cookie { get; private set; }
		public DateTimeOffset? Date { get; private set; }
		public List<NameValueWithParametersHeaderValue> Expect { get; private set; }
		public string From { get; private set; }
		public string Host { get; private set; }
		public List<EntityTagHeaderValue> IfMatch { get; private set; }
		public DateTimeOffset? IfModifiedSince { get; private set; }
		public List<EntityTagHeaderValue> IfNoneMatch { get; private set; }
		public RangeConditionHeaderValue IfRange { get; private set; }
		public DateTimeOffset? IfUnmodifiedSince { get; private set; }
		public int? MaxForwards { get; private set; }
		public List<NameValueHeaderValue> Pragma { get; private set; }
		public AuthenticationHeaderValue ProxyAuthorization { get; private set; }
		public RangeHeaderValue Range { get; private set; }
		public List<string> Trailer { get; private set; }
		public List<TransferCodingHeaderValue> TransferEncoding { get; private set; }
		public List<ProductInfoHeaderValue> UserAgent { get; private set; }
		public List<ProductHeaderValue> Upgrade { get; private set; }
		public List<ViaHeaderValue> Via { get; private set; }
		public List<WarningHeaderValue> Warning { get; private set; }

		//Content Headers
		public List<string> Allow { get; private set; }
		public ContentDispositionHeaderValue ContentDisposition { get; private set; }
		public List<string> ContentEncoding { get; private set; }
		public List<string> ContentLanguage { get; private set; }
		public Uri ContentLocation { get; private set; }
		public ContentRangeHeaderValue ContentRange { get; private set; }
		public MediaTypeHeaderValue ContentType { get; private set; }
		public DateTimeOffset? Expires { get; private set; }
		public DateTimeOffset? LastModified { get; private set; }

		public RESTHTTPClientConfig()
		{
			//Request Headers
			Accept = new List<MediaTypeWithQualityHeaderValue>();
			AcceptCharset = new List<StringWithQualityHeaderValue>();
			AcceptEncoding = new List<StringWithQualityHeaderValue>();
			AcceptLanguage = new List<StringWithQualityHeaderValue>();
			Connection = new List<string>();
			Cookie = new List<string>();
			Expect = new List<NameValueWithParametersHeaderValue>();
			IfMatch = new List<EntityTagHeaderValue>();
			IfNoneMatch = new List<EntityTagHeaderValue>();
			Pragma = new List<NameValueHeaderValue>();
			Trailer = new List<string>();
			TransferEncoding = new List<TransferCodingHeaderValue>();
			UserAgent = new List<ProductInfoHeaderValue>();
			Upgrade = new List<ProductHeaderValue>();
			Via = new List<ViaHeaderValue>();
			Warning = new List<WarningHeaderValue>();

			//Content Headers
			Allow = new List<string>();
			ContentEncoding = new List<string>();
			ContentLanguage = new List<string>();
		}

		public HttpRequestMessage CreateRequest(string RequestUri, HttpMethod Method, HttpContent Content, long ContentLength, bool UseHTTP10 = false)
		{
			var t = new HttpRequestMessage(Method, new Uri(RequestUri, UriKind.RelativeOrAbsolute));
			if (UseHTTP10) t.Version = HttpVersion.Version10;
			t.Content = Content;

			//Request Headers
			foreach (var x in Accept) t.Headers.Accept.Add(x);
			foreach (var x in AcceptCharset) t.Headers.AcceptCharset.Add(x);
			foreach (var x in AcceptEncoding) t.Headers.AcceptEncoding.Add(x);
			foreach (var x in AcceptLanguage) t.Headers.AcceptLanguage.Add(x);
			t.Headers.Authorization = Authorization;
			t.Headers.CacheControl = CacheControl;
			foreach (var x in Connection) t.Headers.Connection.Add(x);
			t.Headers.Date = Date;
			foreach (var x in Expect) t.Headers.Expect.Add(x);
			t.Headers.From = From;
			t.Headers.Host = Host;
			foreach (var x in IfMatch) t.Headers.IfMatch.Add(x);
			t.Headers.IfModifiedSince = IfModifiedSince;
			foreach (var x in IfNoneMatch) t.Headers.IfNoneMatch.Add(x);
			t.Headers.IfRange = IfRange;
			t.Headers.IfUnmodifiedSince = IfUnmodifiedSince;
			t.Headers.MaxForwards = MaxForwards;
			foreach (var x in Pragma) t.Headers.Pragma.Add(x);
			t.Headers.ProxyAuthorization = ProxyAuthorization;
			t.Headers.Range = Range;
			foreach (var x in Trailer) t.Headers.Trailer.Add(x);
			foreach (var x in TransferEncoding) t.Headers.TransferEncoding.Add(x);
			foreach (var x in UserAgent) t.Headers.UserAgent.Add(x);
			foreach (var x in Upgrade) t.Headers.Upgrade.Add(x);
			foreach (var x in Via) t.Headers.Via.Add(x);
			foreach (var x in Warning) t.Headers.Warning.Add(x);

			//Content Headers
			foreach (var x in Allow) t.Content.Headers.Allow.Add(x);
			t.Content.Headers.ContentDisposition = ContentDisposition;
			foreach (var x in ContentEncoding) t.Content.Headers.ContentEncoding.Add(x);
			foreach (var x in ContentLanguage) t.Content.Headers.ContentLanguage.Add(x);
			t.Content.Headers.ContentLength = ContentLength;
			t.Content.Headers.ContentLocation = ContentLocation;
			t.Content.Headers.ContentRange = ContentRange;
			t.Content.Headers.ContentType = ContentType;
			t.Content.Headers.Expires = Expires;
			t.Content.Headers.LastModified = LastModified;

			return t;
		}
	}
}