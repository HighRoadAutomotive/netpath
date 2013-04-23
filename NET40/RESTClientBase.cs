using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace System.ServiceModel
{
	public abstract class RESTClientBase
	{
		public static CookieContainer GlobalCookie { get; private set; }
		static RESTClientBase()
		{
			GlobalCookie = new CookieContainer();
		}

		public Uri BaseURI { get; protected set; }
		public CookieContainer Cookies { get; private set; }
		public CredentialCache Credentials { get; private set; }

		protected RESTClientBase()
		{
			Cookies = new CookieContainer();
			Credentials = null;
		}

		protected RESTClientBase(string BaseURI, CookieContainer Cookies = null, CredentialCache Credentials = null)
		{
			this.BaseURI = new Uri(BaseURI);
			this.Cookies = Cookies ?? new CookieContainer();
			this.Credentials = Credentials;
		}
	}
}