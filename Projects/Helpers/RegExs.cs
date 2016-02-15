using System.Text.RegularExpressions;

namespace NETPath.Projects.Helpers
{
	public static class RegExs
	{
		public static Regex ReplaceSpaces { get; private set; }
		public static Regex MatchFileName { get; private set; }
		public static Regex MatchHttpUri { get; private set; }
		public static Regex MatchTcpUri { get; private set; }
		public static Regex MatchP2PUri { get; private set; }
		public static Regex MatchPipeUri { get; private set; }
		public static Regex MatchMsmqUri { get; private set; }
		public static Regex MatchCodeName { get; private set; }
		public static Regex MatchIPv4 { get; private set; }
		public static Regex MatchIPv6 { get; private set; }
		public static Regex MatchEmail { get; private set; }

		static RegExs()
		{
			ReplaceSpaces = new Regex(@"\s+", RegexOptions.Compiled | RegexOptions.Singleline);
			MatchFileName = new Regex(@"^[^ \\/:*?""<>|]+([ ]+[^ \\/:*?""<>|]+)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
			MatchHttpUri = new Regex(@"^(http|https)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$", RegexOptions.Compiled | RegexOptions.Singleline);
			MatchTcpUri = new Regex(@"^(net\.tcp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$", RegexOptions.Compiled | RegexOptions.Singleline);
			MatchP2PUri = new Regex(@"^(net\.p2p)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$", RegexOptions.Compiled | RegexOptions.Singleline);
			MatchPipeUri = new Regex(@"^(net\.pipe)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$", RegexOptions.Compiled | RegexOptions.Singleline);
			MatchMsmqUri = new Regex(@"^(net\.msmq)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$", RegexOptions.Compiled | RegexOptions.Singleline);
			MatchCodeName = new Regex(@"^[_\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}][\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}]*", RegexOptions.Compiled | RegexOptions.Singleline);
			MatchIPv4 = new Regex(@"^(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[0-9]{1,2})(\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[0-9]{1,2})){3}$", RegexOptions.Compiled | RegexOptions.Singleline);
			MatchIPv6 = new Regex(@"^([0-9A-Fa-f]{1,4}:){7}[0-9A-Fa-f]{1,4}$", RegexOptions.Compiled | RegexOptions.Singleline);
			MatchEmail = new Regex(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
		}
	}
}