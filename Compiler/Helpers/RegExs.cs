using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WCFArchitect.Helpers
{
	internal static class RegExs
	{
		public static Regex ReplaceSpaces { get; private set; }
		public static Regex MatchFileName { get; private set; }
		public static Regex MatchHTTPURI { get; private set; }
		public static Regex MatchTCPURI { get; private set; }
		public static Regex MatchP2PURI { get; private set; }
		public static Regex MatchPipeURI { get; private set; }
		public static Regex MatchMSMQURI { get; private set; }
		public static Regex MatchCodeName { get; private set; }
		public static Regex MatchIPv4 { get; private set; }
		public static Regex MatchIPv6 { get; private set; }

		static RegExs()
		{
			ReplaceSpaces = new Regex(@"\s+", RegexOptions.Compiled);
			MatchFileName = new Regex(@"^[^ \\/:*?""<>|]+([ ]+[^ \\/:*?""<>|]+)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			MatchHTTPURI = new Regex(@"^(http|https)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$", RegexOptions.Compiled);
			MatchTCPURI = new Regex(@"^(net\.tcp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$", RegexOptions.Compiled);
			MatchP2PURI = new Regex(@"^(net\.p2p)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$", RegexOptions.Compiled);
			MatchPipeURI = new Regex(@"^(net\.pipe)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$", RegexOptions.Compiled);
			MatchMSMQURI = new Regex(@"^(net\.msmq)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$", RegexOptions.Compiled);
			MatchCodeName = new Regex(@"^[A-Za-z][A-Za-z0-9]+", RegexOptions.Compiled);
			MatchIPv4 = new Regex(@"^(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[0-9]{1,2})(\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[0-9]{1,2})){3}$", RegexOptions.Compiled);
			MatchIPv6 = new Regex(@"^([0-9A-Fa-f]{1,4}:){7}[0-9A-Fa-f]{1,4}$", RegexOptions.Compiled);
		}
	}
}