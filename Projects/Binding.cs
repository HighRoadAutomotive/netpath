using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;

namespace WCFArchitect.Projects
{
	public enum ServiceBindingTextEncoding
	{
		ASCII = 1,
		UTF7 = 2,
		UTF8 = 0,
		Unicode = 3,
		UTF32 = 4
	}

	public enum ServiceBindingTransactionProtocol
	{
		Default = 0,
		OleTransactions = 1,
		WSAtomicTransaction11 = 2,
		WSAtomicTransactionOctober2004 = 3
	}

	public abstract class ServiceBinding : DataType
	{
		//Basic Binding settings
		public string Namespace { get { return (string)GetValue(NamespaceProperty); } set { SetValue(NamespaceProperty, value); } }
		public static readonly DependencyProperty NamespaceProperty = DependencyProperty.Register("Namespace", typeof(string), typeof(ServiceBinding));

		public string EndpointAddress { get { return (string)GetValue(EndpointAddressProperty); } set { SetValue(EndpointAddressProperty, value); } }
		public static readonly DependencyProperty EndpointAddressProperty = DependencyProperty.Register("EndpointAddress", typeof(string), typeof(ServiceBinding));

		public string ListenAddress { get { return (string)GetValue(ListenAddressProperty); } set { SetValue(ListenAddressProperty, value); } }
		public static readonly DependencyProperty ListenAddressProperty = DependencyProperty.Register("ListenAddress", typeof(string), typeof(ServiceBinding));

		public TimeSpan CloseTimeout { get { return (TimeSpan)GetValue(CloseTimeoutProperty); } set { SetValue(CloseTimeoutProperty, value); } }
		public static readonly DependencyProperty CloseTimeoutProperty = DependencyProperty.Register("CloseTimeout", typeof(TimeSpan), typeof(ServiceBinding), new PropertyMetadata(new TimeSpan(0, 1, 0)));

		public TimeSpan OpenTimeout { get { return (TimeSpan)GetValue(OpenTimeoutProperty); } set { SetValue(OpenTimeoutProperty, value); } }
		public static readonly DependencyProperty OpenTimeoutProperty = DependencyProperty.Register("OpenTimeout", typeof(TimeSpan), typeof(ServiceBinding), new PropertyMetadata(new TimeSpan(0, 1, 0)));

		public TimeSpan ReceiveTimeout { get { return (TimeSpan)GetValue(ReceiveTimeoutProperty); } set { SetValue(ReceiveTimeoutProperty, value); } }
		public static readonly DependencyProperty ReceiveTimeoutProperty = DependencyProperty.Register("ReceiveTimeout", typeof(TimeSpan), typeof(ServiceBinding), new PropertyMetadata(new TimeSpan(0, 10, 0)));

		public TimeSpan SendTimeout { get { return (TimeSpan)GetValue(SendTimeoutProperty); } set { SetValue(SendTimeoutProperty, value); } }
		public static readonly DependencyProperty SendTimeoutProperty = DependencyProperty.Register("SendTimeout", typeof(TimeSpan), typeof(ServiceBinding), new PropertyMetadata(new TimeSpan(0, 10, 0)));

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(ServiceBinding));

		public ServiceBinding() : base(DataTypeMode.Class)
		{
			Documentation = new Documentation { IsClass = true };
		}

		public IEnumerable<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			var results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Project || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
						if (!string.IsNullOrEmpty(Namespace)) if (Namespace.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Namespace", Namespace, Parent.Owner, this));
					}
					else
					{
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
						if (!string.IsNullOrEmpty(Namespace)) if (Namespace.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Namespace", Namespace, Parent.Owner, this));
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(Name)) if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
					if (!string.IsNullOrEmpty(Namespace)) if (Args.RegexSearch.IsMatch(Namespace)) results.Add(new FindReplaceResult("Namespace", Namespace, Parent.Owner, this));
				}

				if (Args.ReplaceAll)
				{
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (!string.IsNullOrEmpty(Namespace)) Namespace = Microsoft.VisualBasic.Strings.Replace(Namespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						else
						{
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
							if (!string.IsNullOrEmpty(Namespace)) Namespace = Microsoft.VisualBasic.Strings.Replace(Namespace, Args.Search, Args.Replace);
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(Name)) Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (!string.IsNullOrEmpty(Namespace)) Namespace = Args.RegexSearch.Replace(Namespace, Args.Replace);
					}
				}
			}

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
		{
			if (!Args.ReplaceAll) return;
			if (Args.UseRegex == false)
			{
				if (Args.MatchCase == false)
				{
					if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					if (Field == "Namespace") Namespace = Microsoft.VisualBasic.Strings.Replace(Namespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
				}
				else
				{
					if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
					if (Field == "Namespace") Namespace = Microsoft.VisualBasic.Strings.Replace(Namespace, Args.Search, Args.Replace);
				}
			}
			else
			{
				if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
				if (Field == "Namespace") Namespace = Args.RegexSearch.Replace(Namespace, Args.Replace);
			}
		}
	}

	#region  - ServiceBindingBasicHTTP Class -

	public class ServiceBindingBasicHTTP : ServiceBinding
	{
		public BindingSecurityBasicHTTP Security { get { return (BindingSecurityBasicHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityBasicHTTP), typeof(ServiceBindingBasicHTTP));

		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(ServiceBindingBasicHTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(ServiceBindingBasicHTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingBasicHTTP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(ServiceBindingBasicHTTP));

		public long MaxBufferSize { get { return (long)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(long), typeof(ServiceBindingBasicHTTP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(ServiceBindingBasicHTTP));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(ServiceBindingBasicHTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(ServiceBindingBasicHTTP));

		public ServiceBindingTextEncoding TextEncoding { get { return (ServiceBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(ServiceBindingTextEncoding), typeof(ServiceBindingBasicHTTP));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(ServiceBindingBasicHTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(ServiceBindingBasicHTTP));

		public ServiceBindingBasicHTTP(): base() { }

		public ServiceBindingBasicHTTP(string Name, Namespace Parent) : base()
		{
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			InheritedTypes.Add(new DataType("System.ServiceModel.BasicHttpBinding", DataTypeMode.Class));
			ID = Guid.NewGuid();
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;
			Security = new BindingSecurityBasicHTTP(this);

			CloseTimeout = new TimeSpan(0, 1, 0);
			OpenTimeout = new TimeSpan(0, 1, 0);
			ReceiveTimeout = new TimeSpan(0, 10, 0);
			SendTimeout = new TimeSpan(0, 1, 0);

			AllowCookies = false;
			BypassProxyOnLocal = false;
			HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			MaxBufferPoolSize = 524288;
			MaxBufferSize = 65536;
			MaxReceivedMessageSize = 65536;
			MessageEncoding = System.ServiceModel.WSMessageEncoding.Text;
			TextEncoding = ServiceBindingTextEncoding.UTF8;
			TransferMode = System.ServiceModel.TransferMode.Buffered;
			UseDefaultWebProxy = true;
		}
	}

	#endregion

	#region  - ServiceBindingBasicHTTPS Class -

	public class ServiceBindingBasicHTTPS : ServiceBinding
	{
		public BindingSecurityBasicHTTPS Security { get { return (BindingSecurityBasicHTTPS)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityBasicHTTPS), typeof(ServiceBindingBasicHTTPS));

		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(ServiceBindingBasicHTTPS));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(ServiceBindingBasicHTTPS));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingBasicHTTPS));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(ServiceBindingBasicHTTPS));

		public long MaxBufferSize { get { return (long)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(long), typeof(ServiceBindingBasicHTTPS));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(ServiceBindingBasicHTTPS));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(ServiceBindingBasicHTTPS));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(ServiceBindingBasicHTTPS));

		public ServiceBindingTextEncoding TextEncoding { get { return (ServiceBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(ServiceBindingTextEncoding), typeof(ServiceBindingBasicHTTPS));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(ServiceBindingBasicHTTPS));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(ServiceBindingBasicHTTPS));

		public ServiceBindingBasicHTTPS() : base() { }

		public ServiceBindingBasicHTTPS(string Name, Namespace Parent) : base()
		{
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			InheritedTypes.Add(new DataType("System.ServiceModel.BasicHttpsBinding", DataTypeMode.Class));
			ID = Guid.NewGuid();
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;
			Security = new BindingSecurityBasicHTTPS(this);

			CloseTimeout = new TimeSpan(0, 1, 0);
			OpenTimeout = new TimeSpan(0, 1, 0);
			ReceiveTimeout = new TimeSpan(0, 10, 0);
			SendTimeout = new TimeSpan(0, 1, 0);

			AllowCookies = false;
			BypassProxyOnLocal = false;
			HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			MaxBufferPoolSize = 524288;
			MaxBufferSize = 65536;
			MaxReceivedMessageSize = 65536;
			MessageEncoding = System.ServiceModel.WSMessageEncoding.Text;
			TextEncoding = ServiceBindingTextEncoding.UTF8;
			TransferMode = System.ServiceModel.TransferMode.Buffered;
			UseDefaultWebProxy = true;
		}
	}

	#endregion

	#region  - ServiceBindingNetHTTP Class -

	public class ServiceBindingNetHTTP : ServiceBinding
	{
		public BindingSecurityBasicHTTP Security { get { return (BindingSecurityBasicHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityBasicHTTP), typeof(ServiceBindingNetHTTP));

		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(ServiceBindingNetHTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(ServiceBindingNetHTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingNetHTTP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(ServiceBindingNetHTTP));

		public long MaxBufferSize { get { return (long)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(long), typeof(ServiceBindingNetHTTP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(ServiceBindingNetHTTP));

		public System.ServiceModel.NetHttpMessageEncoding MessageEncoding { get { return (System.ServiceModel.NetHttpMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.NetHttpMessageEncoding), typeof(ServiceBindingNetHTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(ServiceBindingNetHTTP));

		public ServiceBindingTextEncoding TextEncoding { get { return (ServiceBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(ServiceBindingTextEncoding), typeof(ServiceBindingNetHTTP));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(ServiceBindingNetHTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(ServiceBindingNetHTTP));

		//WebSocket Settings
		public bool CreateNotificationOnConnection { get { return (bool)GetValue(CreateNotificationOnConnectionProperty); } set { SetValue(CreateNotificationOnConnectionProperty, value); } }
		public static readonly DependencyProperty CreateNotificationOnConnectionProperty = DependencyProperty.Register("CreateNotificationOnConnection", typeof(bool), typeof(ServiceBindingNetHTTP));

		public bool DisablePayloadMasking { get { return (bool)GetValue(DisablePayloadMaskingProperty); } set { SetValue(DisablePayloadMaskingProperty, value); } }
		public static readonly DependencyProperty DisablePayloadMaskingProperty = DependencyProperty.Register("DisablePayloadMasking", typeof(bool), typeof(ServiceBindingNetHTTP));

		public TimeSpan KeepAliveInterval { get { return (TimeSpan)GetValue(KeepAliveIntervalProperty); } set { SetValue(KeepAliveIntervalProperty, value); } }
		public static readonly DependencyProperty KeepAliveIntervalProperty = DependencyProperty.Register("KeepAliveInterval", typeof(TimeSpan), typeof(ServiceBindingNetHTTP), new PropertyMetadata(new TimeSpan(0, 10, 0)));

		public int MaxPendingConnections { get { return (int)GetValue(MaxPendingConnectionsProperty); } set { SetValue(MaxPendingConnectionsProperty, value); } }
		public static readonly DependencyProperty MaxPendingConnectionsProperty = DependencyProperty.Register("MaxPendingConnections", typeof(int), typeof(ServiceBindingNetHTTP));

		public string SubProtocol { get { return (string)GetValue(SubProtocolProperty); } set { SetValue(SubProtocolProperty, value); } }
		public static readonly DependencyProperty SubProtocolProperty = DependencyProperty.Register("SubProtocol", typeof(string), typeof(ServiceBindingNetHTTP));

		public System.ServiceModel.Channels.WebSocketTransportUsage TransportUsage { get { return (System.ServiceModel.Channels.WebSocketTransportUsage)GetValue(TransportUsageProperty); } set { SetValue(TransportUsageProperty, value); } }
		public static readonly DependencyProperty TransportUsageProperty = DependencyProperty.Register("TransportUsage", typeof(System.ServiceModel.Channels.WebSocketTransportUsage), typeof(ServiceBindingNetHTTP));

		//Reliable Sessions
		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(ServiceBindingNetHTTP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(ServiceBindingNetHTTP), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(ServiceBindingNetHTTP));

		public ServiceBindingNetHTTP() : base() { }

		public ServiceBindingNetHTTP(string Name, Namespace Parent) : base()
		{
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			InheritedTypes.Add(new DataType("System.ServiceModel.NetHTTPBinding", DataTypeMode.Class));
			ID = Guid.NewGuid();
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;
			Security = new BindingSecurityBasicHTTP(this);

			CloseTimeout = new TimeSpan(0, 1, 0);
			OpenTimeout = new TimeSpan(0, 1, 0);
			ReceiveTimeout = new TimeSpan(0, 10, 0);
			SendTimeout = new TimeSpan(0, 1, 0);

			AllowCookies = false;
			BypassProxyOnLocal = false;
			HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			MaxBufferPoolSize = 524288;
			MaxBufferSize = 65536;
			MaxReceivedMessageSize = 65536;
			MessageEncoding = System.ServiceModel.NetHttpMessageEncoding.Text;
			TextEncoding = ServiceBindingTextEncoding.UTF8;
			TransferMode = System.ServiceModel.TransferMode.Buffered;
			UseDefaultWebProxy = true;
			CreateNotificationOnConnection = false;
			DisablePayloadMasking = false;
			MaxPendingConnections = 32;
			TransportUsage = System.ServiceModel.Channels.WebSocketTransportUsage.WhenDuplex;
			ReliableSessionEnabled = false;
			ReliableSessionInactivityTimeout = new TimeSpan(0, 10, 0);
			ReliableSessionsOrdered = false;
		}
	}

	#endregion

	#region  - ServiceBindingNetHTTPS Class -

	public class ServiceBindingNetHTTPS : ServiceBinding
	{
		public BindingSecurityBasicHTTPS Security { get { return (BindingSecurityBasicHTTPS)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityBasicHTTPS), typeof(ServiceBindingNetHTTPS));

		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(ServiceBindingNetHTTPS));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(ServiceBindingNetHTTPS));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingNetHTTPS));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(ServiceBindingNetHTTPS));

		public long MaxBufferSize { get { return (long)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(long), typeof(ServiceBindingNetHTTPS));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(ServiceBindingNetHTTPS));

		public System.ServiceModel.NetHttpMessageEncoding MessageEncoding { get { return (System.ServiceModel.NetHttpMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.NetHttpMessageEncoding), typeof(ServiceBindingNetHTTPS));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(ServiceBindingNetHTTPS));

		public ServiceBindingTextEncoding TextEncoding { get { return (ServiceBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(ServiceBindingTextEncoding), typeof(ServiceBindingNetHTTPS));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(ServiceBindingNetHTTPS));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(ServiceBindingNetHTTPS));

		//WebSocket Settings
		public bool CreateNotificationOnConnection { get { return (bool)GetValue(CreateNotificationOnConnectionProperty); } set { SetValue(CreateNotificationOnConnectionProperty, value); } }
		public static readonly DependencyProperty CreateNotificationOnConnectionProperty = DependencyProperty.Register("CreateNotificationOnConnection", typeof(bool), typeof(ServiceBindingNetHTTPS));

		public bool DisablePayloadMasking { get { return (bool)GetValue(DisablePayloadMaskingProperty); } set { SetValue(DisablePayloadMaskingProperty, value); } }
		public static readonly DependencyProperty DisablePayloadMaskingProperty = DependencyProperty.Register("DisablePayloadMasking", typeof(bool), typeof(ServiceBindingNetHTTPS));

		public TimeSpan KeepAliveInterval { get { return (TimeSpan)GetValue(KeepAliveIntervalProperty); } set { SetValue(KeepAliveIntervalProperty, value); } }
		public static readonly DependencyProperty KeepAliveIntervalProperty = DependencyProperty.Register("KeepAliveInterval", typeof(TimeSpan), typeof(ServiceBindingNetHTTPS), new PropertyMetadata(new TimeSpan(0, 10, 0)));

		public int MaxPendingConnections { get { return (int)GetValue(MaxPendingConnectionsProperty); } set { SetValue(MaxPendingConnectionsProperty, value); } }
		public static readonly DependencyProperty MaxPendingConnectionsProperty = DependencyProperty.Register("MaxPendingConnections", typeof(int), typeof(ServiceBindingNetHTTPS));

		public string SubProtocol { get { return (string)GetValue(SubProtocolProperty); } set { SetValue(SubProtocolProperty, value); } }
		public static readonly DependencyProperty SubProtocolProperty = DependencyProperty.Register("SubProtocol", typeof(string), typeof(ServiceBindingNetHTTPS));

		public System.ServiceModel.Channels.WebSocketTransportUsage TransportUsage { get { return (System.ServiceModel.Channels.WebSocketTransportUsage)GetValue(TransportUsageProperty); } set { SetValue(TransportUsageProperty, value); } }
		public static readonly DependencyProperty TransportUsageProperty = DependencyProperty.Register("TransportUsage", typeof(System.ServiceModel.Channels.WebSocketTransportUsage), typeof(ServiceBindingNetHTTPS));

		//Reliable Sessions
		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(ServiceBindingNetHTTPS));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(ServiceBindingNetHTTPS), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(ServiceBindingNetHTTPS));

		public ServiceBindingNetHTTPS() : base() { }

		public ServiceBindingNetHTTPS(string Name, Namespace Parent) : base()
		{
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			InheritedTypes.Add(new DataType("System.ServiceModel.NetHTTPBinding", DataTypeMode.Class));
			ID = Guid.NewGuid();
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;
			Security = new BindingSecurityBasicHTTPS(this);

			CloseTimeout = new TimeSpan(0, 1, 0);
			OpenTimeout = new TimeSpan(0, 1, 0);
			ReceiveTimeout = new TimeSpan(0, 10, 0);
			SendTimeout = new TimeSpan(0, 1, 0);

			AllowCookies = false;
			BypassProxyOnLocal = false;
			HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			MaxBufferPoolSize = 524288;
			MaxBufferSize = 65536;
			MaxReceivedMessageSize = 65536;
			MessageEncoding = System.ServiceModel.NetHttpMessageEncoding.Text;
			TextEncoding = ServiceBindingTextEncoding.UTF8;
			TransferMode = System.ServiceModel.TransferMode.Buffered;
			UseDefaultWebProxy = true;
			CreateNotificationOnConnection = false;
			DisablePayloadMasking = false;
			MaxPendingConnections = 32;
			TransportUsage = System.ServiceModel.Channels.WebSocketTransportUsage.WhenDuplex;
			ReliableSessionEnabled = false;
			ReliableSessionInactivityTimeout = new TimeSpan(0, 10, 0);
			ReliableSessionsOrdered = false;
		}
	}

	#endregion

	#region - ServiceBindingWSHTTP Class -

	public class ServiceBindingWSHTTP : ServiceBinding
	{
		public BindingSecurityWSHTTP Security { get { return (BindingSecurityWSHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityWSHTTP), typeof(ServiceBindingWSHTTP));

		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(ServiceBindingWSHTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(ServiceBindingWSHTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingWSHTTP));

		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(ServiceBindingWSHTTP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(ServiceBindingWSHTTP), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(ServiceBindingWSHTTP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(ServiceBindingWSHTTP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(ServiceBindingWSHTTP));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(ServiceBindingWSHTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(ServiceBindingWSHTTP));

		public ServiceBindingTextEncoding TextEncoding { get { return (ServiceBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(ServiceBindingTextEncoding), typeof(ServiceBindingWSHTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(ServiceBindingWSHTTP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(ServiceBindingWSHTTP));

		public ServiceBindingWSHTTP() : base() { }

		public ServiceBindingWSHTTP(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.WSHttpBinding", DataTypeMode.Class));
			Security = new BindingSecurityWSHTTP(this);

			CloseTimeout = new TimeSpan(0, 1, 0);
			OpenTimeout = new TimeSpan(0, 1, 0);
			ReceiveTimeout = new TimeSpan(0, 10, 0);
			SendTimeout = new TimeSpan(0, 1, 0);

			AllowCookies = false;
			BypassProxyOnLocal = false;
			HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			MaxBufferPoolSize = 524288;
			MaxReceivedMessageSize = 65536;
			MessageEncoding = System.ServiceModel.WSMessageEncoding.Text;
			ReliableSessionEnabled = false;
			ReliableSessionInactivityTimeout = new TimeSpan(0, 10, 0);
			ReliableSessionsOrdered = false;
			TextEncoding = ServiceBindingTextEncoding.UTF8;
			TransactionFlow = true;
			UseDefaultWebProxy = true;
		}
	}

	#endregion

	#region - ServiceBindingWS2007HTTP Class -

	public class ServiceBindingWS2007HTTP : ServiceBinding
	{
		public BindingSecurityWSHTTP Security { get { return (BindingSecurityWSHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityWSHTTP), typeof(ServiceBindingWSHTTP));

		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingWS2007HTTP));

		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(ServiceBindingWS2007HTTP), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(ServiceBindingWS2007HTTP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(ServiceBindingWS2007HTTP));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(ServiceBindingWS2007HTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(ServiceBindingWS2007HTTP));

		public ServiceBindingTextEncoding TextEncoding { get { return (ServiceBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(ServiceBindingTextEncoding), typeof(ServiceBindingWS2007HTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public ServiceBindingWS2007HTTP() : base() { }

		public ServiceBindingWS2007HTTP(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.WS2007HttpBinding", DataTypeMode.Class));
			Security = new BindingSecurityWSHTTP(this);

			CloseTimeout = new TimeSpan(0, 1, 0);
			OpenTimeout = new TimeSpan(0, 1, 0);
			ReceiveTimeout = new TimeSpan(0, 10, 0);
			SendTimeout = new TimeSpan(0, 1, 0);

			AllowCookies = false;
			BypassProxyOnLocal = false;
			HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			MaxBufferPoolSize = 524288;
			MaxReceivedMessageSize = 65536;
			MessageEncoding = System.ServiceModel.WSMessageEncoding.Text;
			ReliableSessionEnabled = false;
			ReliableSessionInactivityTimeout = new TimeSpan(0, 10, 0);
			ReliableSessionsOrdered = false;
			TextEncoding = ServiceBindingTextEncoding.UTF8;
			TransactionFlow = true;
			UseDefaultWebProxy = true;
		}
	}

	#endregion

	#region - ServiceBindingWSDualHTTP Class -

	public class ServiceBindingWSDualHTTP : ServiceBinding
	{
		public BindingSecurityWSDualHTTP Security { get { return (BindingSecurityWSDualHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityWSDualHTTP), typeof(ServiceBindingWSDualHTTP));

		public string ClientBaseAddress { get { return (string)GetValue(ClientBaseAddressProperty); } set { SetValue(ClientBaseAddressProperty, value); } }
		public static readonly DependencyProperty ClientBaseAddressProperty = DependencyProperty.Register("ClientBaseAddress", typeof(string), typeof(ServiceBindingWSDualHTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingWS2007HTTP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(ServiceBindingWS2007HTTP), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(ServiceBindingWS2007HTTP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(ServiceBindingWS2007HTTP));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(ServiceBindingWS2007HTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(ServiceBindingWS2007HTTP));

		public ServiceBindingTextEncoding TextEncoding { get { return (ServiceBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(ServiceBindingTextEncoding), typeof(ServiceBindingWS2007HTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public ServiceBindingWSDualHTTP() : base() { }

		public ServiceBindingWSDualHTTP(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.WSDualHttpBinding", DataTypeMode.Class));
			Security = new BindingSecurityWSDualHTTP(this);

			CloseTimeout = new TimeSpan(0, 1, 0);
			OpenTimeout = new TimeSpan(0, 1, 0);
			ReceiveTimeout = new TimeSpan(0, 10, 0);
			SendTimeout = new TimeSpan(0, 1, 0);

			BypassProxyOnLocal = false;
			HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			MaxBufferPoolSize = 524288;
			MaxReceivedMessageSize = 65536;
			MessageEncoding = System.ServiceModel.WSMessageEncoding.Text;
			ReliableSessionInactivityTimeout = new TimeSpan(0, 10, 0);
			ReliableSessionsOrdered = false;
			TextEncoding = ServiceBindingTextEncoding.UTF8;
			TransactionFlow = true;
			UseDefaultWebProxy = true;
		}
	}

	#endregion

	#region - ServiceBindingWSFederationHTTP Class -

	public class ServiceBindingWSFederationHTTP : ServiceBinding
	{
		public BindingSecurityWSFederationHTTP Security { get { return (BindingSecurityWSFederationHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityWSFederationHTTP), typeof(ServiceBindingWSFederationHTTP));
		
		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(ServiceBindingWSFederationHTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingWSFederationHTTP));

		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(ServiceBindingWSFederationHTTP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(ServiceBindingWSFederationHTTP), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(ServiceBindingWSFederationHTTP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(ServiceBindingWSFederationHTTP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(ServiceBinding));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(ServiceBindingWSFederationHTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(ServiceBindingWSFederationHTTP));

		public ServiceBindingTextEncoding TextEncoding { get { return (ServiceBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(ServiceBindingTextEncoding), typeof(ServiceBindingWSFederationHTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(ServiceBindingWSFederationHTTP));

		public string PrivacyNoticeAt { get { return (string)GetValue(PrivacyNoticeAtProperty); } set { SetValue(PrivacyNoticeAtProperty, value); } }
		public static readonly DependencyProperty PrivacyNoticeAtProperty = DependencyProperty.Register("PrivacyNoticeAt", typeof(string), typeof(ServiceBindingWSFederationHTTP));

		public int PrivacyNoticeVersion { get { return (int)GetValue(PrivacyNoticeVersionProperty); } set { SetValue(PrivacyNoticeVersionProperty, value); } }
		public static readonly DependencyProperty PrivacyNoticeVersionProperty = DependencyProperty.Register("PrivacyNoticeVersion", typeof(int), typeof(ServiceBindingWSFederationHTTP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(ServiceBindingWSFederationHTTP));

		public ServiceBindingWSFederationHTTP() : base() { }

		public ServiceBindingWSFederationHTTP(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.WSFederationHttpBinding", DataTypeMode.Class));
			Security = new BindingSecurityWSFederationHTTP(this);

			CloseTimeout = new TimeSpan(0, 1, 0);
			OpenTimeout = new TimeSpan(0, 1, 0);
			ReceiveTimeout = new TimeSpan(0, 10, 0);
			SendTimeout = new TimeSpan(0, 1, 0);

			BypassProxyOnLocal = false;
			HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			MaxBufferPoolSize = 524288;
			MaxReceivedMessageSize = 65536;
			MessageEncoding = System.ServiceModel.WSMessageEncoding.Text;
			ReliableSessionEnabled = false;
			ReliableSessionInactivityTimeout = new TimeSpan(0, 10, 0);
			ReliableSessionsOrdered = false;
			TextEncoding = ServiceBindingTextEncoding.UTF8;
			TransactionFlow = true;
			UseDefaultWebProxy = true;
		}
	}

	#endregion

	#region - ServiceBindingWS2007FederationHTTP Class -

	public class ServiceBindingWS2007FederationHTTP : ServiceBinding
	{
		public BindingSecurityWSFederationHTTP Security { get { return (BindingSecurityWSFederationHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityWSFederationHTTP), typeof(ServiceBindingWS2007FederationHTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(ServiceBindingWS2007FederationHTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingWS2007FederationHTTP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(ServiceBindingWS2007FederationHTTP), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(ServiceBindingWS2007FederationHTTP));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(ServiceBindingWS2007FederationHTTP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(ServiceBindingWS2007FederationHTTP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(ServiceBinding));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(ServiceBindingWS2007FederationHTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(ServiceBindingWS2007FederationHTTP));

		public ServiceBindingTextEncoding TextEncoding { get { return (ServiceBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(ServiceBindingTextEncoding), typeof(ServiceBindingWS2007FederationHTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(ServiceBindingWS2007FederationHTTP));

		public string PrivacyNoticeAt { get { return (string)GetValue(PrivacyNoticeAtProperty); } set { SetValue(PrivacyNoticeAtProperty, value); } }
		public static readonly DependencyProperty PrivacyNoticeAtProperty = DependencyProperty.Register("PrivacyNoticeAt", typeof(string), typeof(ServiceBindingWS2007FederationHTTP));

		public int PrivacyNoticeVersion { get { return (int)GetValue(PrivacyNoticeVersionProperty); } set { SetValue(PrivacyNoticeVersionProperty, value); } }
		public static readonly DependencyProperty PrivacyNoticeVersionProperty = DependencyProperty.Register("PrivacyNoticeVersion", typeof(int), typeof(ServiceBindingWS2007FederationHTTP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(ServiceBindingWS2007FederationHTTP));

		public ServiceBindingWS2007FederationHTTP() : base() { }

		public ServiceBindingWS2007FederationHTTP(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.WS2007FederationHttpBinding", DataTypeMode.Class));
			Security = new BindingSecurityWSFederationHTTP(this);

			CloseTimeout = new TimeSpan(0, 1, 0);
			OpenTimeout = new TimeSpan(0, 1, 0);
			ReceiveTimeout = new TimeSpan(0, 10, 0);
			SendTimeout = new TimeSpan(0, 1, 0);

			BypassProxyOnLocal = false;
			HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			MaxBufferPoolSize = 524288;
			MaxReceivedMessageSize = 65536;
			MessageEncoding = System.ServiceModel.WSMessageEncoding.Text;
			ReliableSessionEnabled = false;
			ReliableSessionInactivityTimeout = new TimeSpan(0, 10, 0);
			ReliableSessionsOrdered = false;
			TextEncoding = ServiceBindingTextEncoding.UTF8;
			TransactionFlow = true;
			UseDefaultWebProxy = true;
		}
	}

	#endregion

	#region - ServiceBindingTCP Class -

	public class ServiceBindingTCP : ServiceBinding
	{
		public BindingSecurityTCP Security { get { return (BindingSecurityTCP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityTCP), typeof(ServiceBindingTCP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingTCP));

		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(ServiceBindingTCP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(ServiceBindingTCP), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(ServiceBindingTCP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(ServiceBindingTCP));

		public long MaxBufferSize { get { return (long)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(long), typeof(ServiceBindingTCP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(ServiceBinding));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(ServiceBindingTCP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(ServiceBindingTCP));

		public int ListenBacklog { get { return (int)GetValue(ListenBacklogProperty); } set { SetValue(ListenBacklogProperty, value); } }
		public static readonly DependencyProperty ListenBacklogProperty = DependencyProperty.Register("ListenBacklog", typeof(int), typeof(ServiceBindingTCP));

		public int MaxConnections { get { return (int)GetValue(MaxConnectionsProperty); } set { SetValue(MaxConnectionsProperty, value); } }
		public static readonly DependencyProperty MaxConnectionsProperty = DependencyProperty.Register("MaxConnections", typeof(int), typeof(ServiceBindingTCP));

		public bool PortSharingEnabled { get { return (bool)GetValue(PortSharingEnabledProperty); } set { SetValue(PortSharingEnabledProperty, value); } }
		public static readonly DependencyProperty PortSharingEnabledProperty = DependencyProperty.Register("PortSharingEnabled", typeof(bool), typeof(ServiceBindingTCP));

		public ServiceBindingTransactionProtocol TransactionProtocol { get { return (ServiceBindingTransactionProtocol)GetValue(TransactionProtocolProperty); } set { SetValue(TransactionProtocolProperty, value); } }
		public static readonly DependencyProperty TransactionProtocolProperty = DependencyProperty.Register("TransactionProtocol", typeof(ServiceBindingTransactionProtocol), typeof(ServiceBindingTCP));

		public ServiceBindingTCP() : base() { }

		public ServiceBindingTCP(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.NetTcpBinding", DataTypeMode.Class));
			Security = new BindingSecurityTCP(this);

			CloseTimeout = new TimeSpan(0, 1, 0);
			OpenTimeout = new TimeSpan(0, 1, 0);
			ReceiveTimeout = new TimeSpan(0, 10, 0);
			SendTimeout = new TimeSpan(0, 1, 0);

			HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			ListenBacklog = 10;
			MaxBufferPoolSize = 524288;
			MaxBufferSize = 65536;
			MaxReceivedMessageSize = 65536;
			MaxConnections = 10;
			PortSharingEnabled = false;
			ReliableSessionEnabled = false;
			ReliableSessionInactivityTimeout = new TimeSpan(0, 10, 0);
			ReliableSessionsOrdered = false;
			TransactionFlow = true;
			TransactionProtocol = ServiceBindingTransactionProtocol.Default;
			TransferMode = System.ServiceModel.TransferMode.Buffered;
		}
	}

	#endregion

	#region - ServiceBindingNamedPipe Class -

	public class ServiceBindingNamedPipe : ServiceBinding
	{
		public BindingSecurityNamedPipe Security { get { return (BindingSecurityNamedPipe)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityNamedPipe), typeof(ServiceBindingNamedPipe));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingNamedPipe));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(ServiceBindingNamedPipe));

		public long MaxBufferSize { get { return (long)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(long), typeof(ServiceBindingNamedPipe));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(ServiceBindingNamedPipe));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(ServiceBindingNamedPipe));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(ServiceBindingNamedPipe));

		public int MaxConnections { get { return (int)GetValue(MaxConnectionsProperty); } set { SetValue(MaxConnectionsProperty, value); } }
		public static readonly DependencyProperty MaxConnectionsProperty = DependencyProperty.Register("MaxConnections", typeof(int), typeof(ServiceBindingNamedPipe));

		public ServiceBindingTransactionProtocol TransactionProtocol { get { return (ServiceBindingTransactionProtocol)GetValue(TransactionProtocolProperty); } set { SetValue(TransactionProtocolProperty, value); } }
		public static readonly DependencyProperty TransactionProtocolProperty = DependencyProperty.Register("TransactionProtocol", typeof(ServiceBindingTransactionProtocol), typeof(ServiceBindingNamedPipe));

		public ServiceBindingNamedPipe() : base() { }

		public ServiceBindingNamedPipe(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.NetNamedPipeBinding", DataTypeMode.Class));
			Security = new BindingSecurityNamedPipe(this);

			CloseTimeout = new TimeSpan(0, 1, 0);
			OpenTimeout = new TimeSpan(0, 1, 0);
			ReceiveTimeout = new TimeSpan(0, 10, 0);
			SendTimeout = new TimeSpan(0, 1, 0);

			HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			MaxBufferPoolSize = 524288;
			MaxBufferSize = 65536;
			MaxReceivedMessageSize = 65536;
			MaxConnections = 10;
			TransactionFlow = true;
			TransactionProtocol = ServiceBindingTransactionProtocol.Default;
			TransferMode = System.ServiceModel.TransferMode.Buffered;
		}
	}

	#endregion

	#region - ServiceBindingMSMQ Class -

	public class ServiceBindingMSMQ : ServiceBinding
	{
		public BindingSecurityMSMQ Security { get { return (BindingSecurityMSMQ)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityMSMQ), typeof(ServiceBindingMSMQ));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(ServiceBindingMSMQ));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(ServiceBindingMSMQ));

		public string CustomDeadLetterQueue { get { return (string)GetValue(CustomDeadLetterQueueProperty); } set { SetValue(CustomDeadLetterQueueProperty, value); } }
		public static readonly DependencyProperty CustomDeadLetterQueueProperty = DependencyProperty.Register("CustomDeadLetterQueue", typeof(string), typeof(ServiceBindingMSMQ));

		public System.ServiceModel.DeadLetterQueue DeadLetterQueue { get { return (System.ServiceModel.DeadLetterQueue)GetValue(DeadLetterQueueProperty); } set { SetValue(DeadLetterQueueProperty, value); } }
		public static readonly DependencyProperty DeadLetterQueueProperty = DependencyProperty.Register("DeadLetterQueue", typeof(System.ServiceModel.DeadLetterQueue), typeof(ServiceBindingMSMQ));

		public bool ExactlyOnce { get { return (bool)GetValue(ExactlyOnceProperty); } set { SetValue(ExactlyOnceProperty, value); } }
		public static readonly DependencyProperty ExactlyOnceProperty = DependencyProperty.Register("ExactlyOnce", typeof(bool), typeof(ServiceBindingMSMQ));

		public int MaxRetryCycles { get { return (int)GetValue(MaxRetryCyclesProperty); } set { SetValue(MaxRetryCyclesProperty, value); } }
		public static readonly DependencyProperty MaxRetryCyclesProperty = DependencyProperty.Register("MaxRetryCycles", typeof(int), typeof(ServiceBindingMSMQ));

		public System.ServiceModel.QueueTransferProtocol QueueTransferProtocol { get { return (System.ServiceModel.QueueTransferProtocol)GetValue(QueueTransferProtocolProperty); } set { SetValue(QueueTransferProtocolProperty, value); } }
		public static readonly DependencyProperty QueueTransferProtocolProperty = DependencyProperty.Register("QueueTransferProtocol", typeof(System.ServiceModel.QueueTransferProtocol), typeof(ServiceBindingMSMQ));

		public bool ReceiveContextEnabled { get { return (bool)GetValue(ReceiveContextEnabledProperty); } set { SetValue(ReceiveContextEnabledProperty, value); } }
		public static readonly DependencyProperty ReceiveContextEnabledProperty = DependencyProperty.Register("ReceiveContextEnabled", typeof(bool), typeof(ServiceBindingMSMQ));

		public System.ServiceModel.ReceiveErrorHandling ReceiveErrorHandling { get { return (System.ServiceModel.ReceiveErrorHandling)GetValue(ReceiveErrorHandlingProperty); } set { SetValue(ReceiveErrorHandlingProperty, value); } }
		public static readonly DependencyProperty ReceiveErrorHandlingProperty = DependencyProperty.Register("ReceiveErrorHandling", typeof(System.ServiceModel.ReceiveErrorHandling), typeof(ServiceBindingMSMQ));

		public int ReceiveRetryCount { get { return (int)GetValue(ReceiveRetryCountProperty); } set { SetValue(ReceiveRetryCountProperty, value); } }
		public static readonly DependencyProperty ReceiveRetryCountProperty = DependencyProperty.Register("ReceiveRetryCount", typeof(int), typeof(ServiceBindingMSMQ));

		public TimeSpan RetryCycleDelay { get { return (TimeSpan)GetValue(RetryCycleDelayProperty); } set { SetValue(RetryCycleDelayProperty, value); } }
		public static readonly DependencyProperty RetryCycleDelayProperty = DependencyProperty.Register("RetryCycleDelay", typeof(TimeSpan), typeof(ServiceBindingMSMQ), new PropertyMetadata(new TimeSpan(0, 1, 0)));

		public TimeSpan TimeToLive { get { return (TimeSpan)GetValue(TimeToLiveProperty); } set { SetValue(TimeToLiveProperty, value); } }
		public static readonly DependencyProperty TimeToLiveProperty = DependencyProperty.Register("TimeToLive", typeof(TimeSpan), typeof(ServiceBindingMSMQ), new PropertyMetadata(new TimeSpan(0, 10, 0)));

		public bool UseActiveDirectory { get { return (bool)GetValue(UseActiveDirectoryProperty); } set { SetValue(UseActiveDirectoryProperty, value); } }
		public static readonly DependencyProperty UseActiveDirectoryProperty = DependencyProperty.Register("UseActiveDirectory", typeof(bool), typeof(ServiceBindingMSMQ));

		public bool UseMSMQTracing { get { return (bool)GetValue(UseMSMQTracingProperty); } set { SetValue(UseMSMQTracingProperty, value); } }
		public static readonly DependencyProperty UseMSMQTracingProperty = DependencyProperty.Register("UseMSMQTracing", typeof(bool), typeof(ServiceBindingMSMQ));

		public bool UseSourceJournal { get { return (bool)GetValue(UseSourceJournalProperty); } set { SetValue(UseSourceJournalProperty, value); } }
		public static readonly DependencyProperty UseSourceJournalProperty = DependencyProperty.Register("UseSourceJournal", typeof(bool), typeof(ServiceBindingMSMQ));

		public TimeSpan ValidityDuration { get { return (TimeSpan)GetValue(ValidityDurationProperty); } set { SetValue(ValidityDurationProperty, value); } }
		public static readonly DependencyProperty ValidityDurationProperty = DependencyProperty.Register("ValidityDuration", typeof(TimeSpan), typeof(ServiceBindingMSMQ), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool Durable { get { return (bool)GetValue(DurableProperty); } set { SetValue(DurableProperty, value); } }
		public static readonly DependencyProperty DurableProperty = DependencyProperty.Register("Durable", typeof(bool), typeof(ServiceBindingMSMQ));

		public ServiceBindingMSMQ() : base() { }

		public ServiceBindingMSMQ(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.NetMsmqBinding", DataTypeMode.Class));
			Security = new BindingSecurityMSMQ(this);

			CloseTimeout = new TimeSpan(0, 1, 0);
			OpenTimeout = new TimeSpan(0, 1, 0);
			ReceiveTimeout = new TimeSpan(0, 10, 0);
			SendTimeout = new TimeSpan(0, 1, 0);

			DeadLetterQueue = System.ServiceModel.DeadLetterQueue.System;		//This is the default if Durable is true
			Durable = true;			//Must be true if ExactlyOnce is true.
			ExactlyOnce = true;
			MaxBufferPoolSize = 524288;
			MaxReceivedMessageSize = 65536;
			MaxRetryCycles = 2;
			QueueTransferProtocol = System.ServiceModel.QueueTransferProtocol.Native;
			ReceiveContextEnabled = true;
			ReceiveErrorHandling = System.ServiceModel.ReceiveErrorHandling.Reject;
			ReceiveRetryCount = 5;
			RetryCycleDelay = new TimeSpan(0, 10, 0);
			TimeToLive = new TimeSpan(1, 0, 0, 0);
			UseActiveDirectory = false;
			UseMSMQTracing = false;
			UseSourceJournal = false;
			ValidityDuration = TimeSpan.Zero;
		}
	}

	#endregion

	#region - ServiceBindingPeerTCP Class -

	public class ServiceBindingPeerTCP : ServiceBinding
	{
		public BindingSecurityPeerTCP Security { get { return (BindingSecurityPeerTCP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityPeerTCP), typeof(ServiceBindingPeerTCP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(ServiceBindingPeerTCP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(ServiceBindingPeerTCP));

		public string ListenIPAddress { get { return (string)GetValue(ListenIPAddressProperty); } set { SetValue(ListenIPAddressProperty, value); } }
		public static readonly DependencyProperty ListenIPAddressProperty = DependencyProperty.Register("ListenIPAddress", typeof(string), typeof(ServiceBindingPeerTCP));
		
		public int Port { get { return (int)GetValue(PortProperty); } set { SetValue(PortProperty, value); } }
		public static readonly DependencyProperty PortProperty = DependencyProperty.Register("Port", typeof(int), typeof(ServiceBindingPeerTCP));

		public ServiceBindingPeerTCP() : base() { }

		public ServiceBindingPeerTCP(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.NetPeerTcpBinding", DataTypeMode.Class));
			Security = new BindingSecurityPeerTCP(this);

			CloseTimeout = new TimeSpan(0, 1, 0);
			OpenTimeout = new TimeSpan(0, 1, 0);
			ReceiveTimeout = new TimeSpan(0, 10, 0);
			SendTimeout = new TimeSpan(0, 1, 0);

			MaxBufferPoolSize = 524288;
			MaxReceivedMessageSize = 65536;
			ListenIPAddress = "";
			Port = 31337;		
		}
	}

	#endregion

	#region - ServiceBindingWebHTTP Class -

	public class ServiceBindingWebHTTP : ServiceBinding
	{
		public BindingSecurityWebHTTP Security { get { return (BindingSecurityWebHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityWebHTTP), typeof(ServiceBindingWebHTTP));
				
		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(ServiceBindingWebHTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(ServiceBindingWebHTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingWebHTTP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(ServiceBindingWebHTTP));

		public long MaxBufferSize { get { return (long)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(long), typeof(ServiceBindingWebHTTP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(ServiceBindingWebHTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(ServiceBindingWebHTTP));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(ServiceBindingWebHTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(ServiceBindingWebHTTP));

		public bool CrossDomainScriptAccessEnabled { get { return (bool)GetValue(CrossDomainScriptAccessEnabledProperty); } set { SetValue(CrossDomainScriptAccessEnabledProperty, value); } }
		public static readonly DependencyProperty CrossDomainScriptAccessEnabledProperty = DependencyProperty.Register("CrossDomainScriptAccessEnabled", typeof(bool), typeof(ServiceBindingWebHTTP));

		public ServiceBindingTextEncoding WriteEncoding { get { return (ServiceBindingTextEncoding)GetValue(WriteEncodingProperty); } set { SetValue(WriteEncodingProperty, value); } }
		public static readonly DependencyProperty WriteEncodingProperty = DependencyProperty.Register("WriteEncoding", typeof(ServiceBindingTextEncoding), typeof(ServiceBindingWebHTTP));

		public ServiceBindingWebHTTP() : base() { }

		public ServiceBindingWebHTTP(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.WebHttpBinding", DataTypeMode.Class));
			Security = new BindingSecurityWebHTTP(this);

			CloseTimeout = new TimeSpan(0, 1, 0);
			OpenTimeout = new TimeSpan(0, 1, 0);
			ReceiveTimeout = new TimeSpan(0, 10, 0);
			SendTimeout = new TimeSpan(0, 1, 0);

			AllowCookies = false;
			BypassProxyOnLocal = false;
			CrossDomainScriptAccessEnabled = false;
			HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			MaxBufferPoolSize = 524288;
			MaxBufferSize = 65536;
			MaxReceivedMessageSize = 65536;
			TransferMode = System.ServiceModel.TransferMode.Buffered;
			UseDefaultWebProxy = true;
			WriteEncoding = ServiceBindingTextEncoding.UTF8;
		}
	}

	#endregion 

	#region - ServiceBindingMSMQIntegration Class -

	public class ServiceBindingMSMQIntegration : ServiceBinding
	{
		public BindingSecurityMSMQIntegration Security { get { return (BindingSecurityMSMQIntegration)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityMSMQIntegration), typeof(ServiceBindingMSMQIntegration));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(ServiceBindingMSMQIntegration));

		public string CustomDeadLetterQueue { get { return (string)GetValue(CustomDeadLetterQueueProperty); } set { SetValue(CustomDeadLetterQueueProperty, value); } }
		public static readonly DependencyProperty CustomDeadLetterQueueProperty = DependencyProperty.Register("CustomDeadLetterQueue", typeof(string), typeof(ServiceBindingMSMQIntegration));

		public System.ServiceModel.DeadLetterQueue DeadLetterQueue { get { return (System.ServiceModel.DeadLetterQueue)GetValue(DeadLetterQueueProperty); } set { SetValue(DeadLetterQueueProperty, value); } }
		public static readonly DependencyProperty DeadLetterQueueProperty = DependencyProperty.Register("DeadLetterQueue", typeof(System.ServiceModel.DeadLetterQueue), typeof(ServiceBindingMSMQIntegration));

		public bool ExactlyOnce { get { return (bool)GetValue(ExactlyOnceProperty); } set { SetValue(ExactlyOnceProperty, value); } }
		public static readonly DependencyProperty ExactlyOnceProperty = DependencyProperty.Register("ExactlyOnce", typeof(bool), typeof(ServiceBindingMSMQIntegration));

		public int MaxRetryCycles { get { return (int)GetValue(MaxRetryCyclesProperty); } set { SetValue(MaxRetryCyclesProperty, value); } }
		public static readonly DependencyProperty MaxRetryCyclesProperty = DependencyProperty.Register("MaxRetryCycles", typeof(int), typeof(ServiceBindingMSMQIntegration));

		public bool ReceiveContextEnabled { get { return (bool)GetValue(ReceiveContextEnabledProperty); } set { SetValue(ReceiveContextEnabledProperty, value); } }
		public static readonly DependencyProperty ReceiveContextEnabledProperty = DependencyProperty.Register("ReceiveContextEnabled", typeof(bool), typeof(ServiceBindingMSMQIntegration));

		public System.ServiceModel.ReceiveErrorHandling ReceiveErrorHandling { get { return (System.ServiceModel.ReceiveErrorHandling)GetValue(ReceiveErrorHandlingProperty); } set { SetValue(ReceiveErrorHandlingProperty, value); } }
		public static readonly DependencyProperty ReceiveErrorHandlingProperty = DependencyProperty.Register("ReceiveErrorHandling", typeof(System.ServiceModel.ReceiveErrorHandling), typeof(ServiceBindingMSMQIntegration));

		public int ReceiveRetryCount { get { return (int)GetValue(ReceiveRetryCountProperty); } set { SetValue(ReceiveRetryCountProperty, value); } }
		public static readonly DependencyProperty ReceiveRetryCountProperty = DependencyProperty.Register("ReceiveRetryCount", typeof(int), typeof(ServiceBindingMSMQIntegration));

		public TimeSpan RetryCycleDelay { get { return (TimeSpan)GetValue(RetryCycleDelayProperty); } set { SetValue(RetryCycleDelayProperty, value); } }
		public static readonly DependencyProperty RetryCycleDelayProperty = DependencyProperty.Register("RetryCycleDelay", typeof(TimeSpan), typeof(ServiceBindingMSMQIntegration), new PropertyMetadata(new TimeSpan(0, 1, 0)));

		public TimeSpan TimeToLive { get { return (TimeSpan)GetValue(TimeToLiveProperty); } set { SetValue(TimeToLiveProperty, value); } }
		public static readonly DependencyProperty TimeToLiveProperty = DependencyProperty.Register("TimeToLive", typeof(TimeSpan), typeof(ServiceBindingMSMQIntegration), new PropertyMetadata(new TimeSpan(0, 10, 0)));

		public bool UseMSMQTracing { get { return (bool)GetValue(UseMSMQTracingProperty); } set { SetValue(UseMSMQTracingProperty, value); } }
		public static readonly DependencyProperty UseMSMQTracingProperty = DependencyProperty.Register("UseMSMQTracing", typeof(bool), typeof(ServiceBindingMSMQIntegration));

		public bool UseSourceJournal { get { return (bool)GetValue(UseSourceJournalProperty); } set { SetValue(UseSourceJournalProperty, value); } }
		public static readonly DependencyProperty UseSourceJournalProperty = DependencyProperty.Register("UseSourceJournal", typeof(bool), typeof(ServiceBindingMSMQIntegration));

		public TimeSpan ValidityDuration { get { return (TimeSpan)GetValue(ValidityDurationProperty); } set { SetValue(ValidityDurationProperty, value); } }
		public static readonly DependencyProperty ValidityDurationProperty = DependencyProperty.Register("ValidityDuration", typeof(TimeSpan), typeof(ServiceBindingMSMQIntegration), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool Durable { get { return (bool)GetValue(DurableProperty); } set { SetValue(DurableProperty, value); } }
		public static readonly DependencyProperty DurableProperty = DependencyProperty.Register("Durable", typeof(bool), typeof(ServiceBinding));

		public System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat SerializationFormat { get { return (System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat)GetValue(SerializationFormatProperty); } set { SetValue(SerializationFormatProperty, value); } }
		public static readonly DependencyProperty SerializationFormatProperty = DependencyProperty.Register("SerializationFormat", typeof(System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat), typeof(ServiceBindingMSMQIntegration));

		public ServiceBindingMSMQIntegration() : base() { }

		public ServiceBindingMSMQIntegration(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.MsmqIntegration.MsmqIntegrationBinding", DataTypeMode.Class));
			Security = new BindingSecurityMSMQIntegration(this);

			CloseTimeout = new TimeSpan(0, 1, 0);
			OpenTimeout = new TimeSpan(0, 1, 0);
			ReceiveTimeout = new TimeSpan(0, 10, 0);
			SendTimeout = new TimeSpan(0, 1, 0);

			DeadLetterQueue = System.ServiceModel.DeadLetterQueue.System;		//This is the default if Durable is true
			Durable = true;			//Must be true if ExactlyOnce is true.
			ExactlyOnce = true;
			MaxReceivedMessageSize = 65536;
			MaxRetryCycles = 2;
			ReceiveContextEnabled = true;
			ReceiveErrorHandling = System.ServiceModel.ReceiveErrorHandling.Reject;
			ReceiveRetryCount = 5;
			RetryCycleDelay = new TimeSpan(0, 10, 0);
			TimeToLive = new TimeSpan(1, 0, 0, 0);
			UseMSMQTracing = false;
			UseSourceJournal = false;
			ValidityDuration = TimeSpan.Zero;
		}
	}
	#endregion

}