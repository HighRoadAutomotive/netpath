using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;

namespace NETPath.Projects.Wcf
{
	public enum WcfBindingTextEncoding
	{
		ASCII = 1,
		UTF7 = 2,
		UTF8 = 0,
		Unicode = 3,
		UTF32 = 4
	}

	public enum WcfBindingTransactionProtocol
	{
		Default = 0,
		OleTransactions = 1,
		WSAtomicTransaction11 = 2,
		WSAtomicTransactionOctober2004 = 3
	}

	public abstract class WcfBinding : DataType
	{
		//Basic Binding settings
		public string Namespace { get { return (string)GetValue(NamespaceProperty); } set { SetValue(NamespaceProperty, value); } }
		public static readonly DependencyProperty NamespaceProperty = DependencyProperty.Register("Namespace", typeof(string), typeof(WcfBinding));

		public TimeSpan CloseTimeout { get { return (TimeSpan)GetValue(CloseTimeoutProperty); } set { SetValue(CloseTimeoutProperty, value); } }
		public static readonly DependencyProperty CloseTimeoutProperty = DependencyProperty.Register("CloseTimeout", typeof(TimeSpan), typeof(WcfBinding), new PropertyMetadata(new TimeSpan(0, 1, 0)));

		public TimeSpan OpenTimeout { get { return (TimeSpan)GetValue(OpenTimeoutProperty); } set { SetValue(OpenTimeoutProperty, value); } }
		public static readonly DependencyProperty OpenTimeoutProperty = DependencyProperty.Register("OpenTimeout", typeof(TimeSpan), typeof(WcfBinding), new PropertyMetadata(new TimeSpan(0, 1, 0)));

		public TimeSpan ReceiveTimeout { get { return (TimeSpan)GetValue(ReceiveTimeoutProperty); } set { SetValue(ReceiveTimeoutProperty, value); } }
		public static readonly DependencyProperty ReceiveTimeoutProperty = DependencyProperty.Register("ReceiveTimeout", typeof(TimeSpan), typeof(WcfBinding), new PropertyMetadata(new TimeSpan(0, 10, 0)));

		public TimeSpan SendTimeout { get { return (TimeSpan)GetValue(SendTimeoutProperty); } set { SetValue(SendTimeoutProperty, value); } }
		public static readonly DependencyProperty SendTimeoutProperty = DependencyProperty.Register("SendTimeout", typeof(TimeSpan), typeof(WcfBinding), new PropertyMetadata(new TimeSpan(0, 10, 0)));

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(WcfBinding));

		public WcfBinding() : base(DataTypeMode.Class)
		{
			Documentation = new Documentation { IsClass = true };
		}
	}

	#region  - WcfBindingBasicHTTP Class -

	public class WcfBindingBasicHTTP : WcfBinding
	{
		public WcfSecurityBasicHTTP Security { get { return (WcfSecurityBasicHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(WcfSecurityBasicHTTP), typeof(WcfBindingBasicHTTP));

		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(WcfBindingBasicHTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(WcfBindingBasicHTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(WcfBindingBasicHTTP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(WcfBindingBasicHTTP));

		public long MaxBufferSize { get { return (long)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(long), typeof(WcfBindingBasicHTTP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(WcfBindingBasicHTTP));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(WcfBindingBasicHTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(WcfBindingBasicHTTP));

		public WcfBindingTextEncoding TextEncoding { get { return (WcfBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(WcfBindingTextEncoding), typeof(WcfBindingBasicHTTP));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(WcfBindingBasicHTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(WcfBindingBasicHTTP));

		//ContextBinding
		public bool UseContextBinding { get { return (bool)GetValue(UseContextBindingProperty); } set { SetValue(UseContextBindingProperty, value); } }
		public static readonly DependencyProperty UseContextBindingProperty = DependencyProperty.Register("UseContextBinding", typeof(bool), typeof(WcfBindingBasicHTTP), new PropertyMetadata(false, UseContextBindingPropertyChangedCallback));

		private static void UseContextBindingPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfBindingBasicHTTP;
			if (t == null) return;
			if (t.InheritedTypes == null) return;

			if ((bool) e.NewValue)
			{
				var til = new List<DataType>();
				foreach (var it in t.InheritedTypes)
					if (it.Name == "System.ServiceModel.BasicHttpBinding")
						til.Add(it);
				foreach (var it in til)
					t.InheritedTypes.Remove(it);

				t.InheritedTypes.Add(new DataType("System.ServiceModel.BasicHttpContextBinding", DataTypeMode.Class));
			}
			else
			{
				var til = new List<DataType>();
				foreach (var it in t.InheritedTypes)
					if (it.Name == "System.ServiceModel.BasicHttpContextBinding")
						til.Add(it);
				foreach (var it in til)
					t.InheritedTypes.Remove(it);

				t.InheritedTypes.Add(new DataType("System.ServiceModel.BasicHttpBinding", DataTypeMode.Class));
				
			}
		}

		public bool ContextManagementEnabled { get { return (bool)GetValue(ContextManagementEnabledProperty); } set { SetValue(ContextManagementEnabledProperty, value); } }
		public static readonly DependencyProperty ContextManagementEnabledProperty = DependencyProperty.Register("ContextManagementEnabled", typeof(bool), typeof(WcfBindingBasicHTTP), new PropertyMetadata(false));

		public WcfBindingBasicHTTP(): base() { }

		public WcfBindingBasicHTTP(string Name, Namespace Parent) : base()
		{
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			InheritedTypes.Add(new DataType("System.ServiceModel.BasicHttpBinding", DataTypeMode.Class));
			ID = Guid.NewGuid();
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;
			Security = new WcfSecurityBasicHTTP(this);

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
			TextEncoding = WcfBindingTextEncoding.UTF8;
			TransferMode = System.ServiceModel.TransferMode.Buffered;
			UseDefaultWebProxy = true;
		}
	}

	#endregion

	#region  - WcfBindingBasicHTTPS Class -

	public class WcfBindingBasicHTTPS : WcfBinding
	{
		public WcfSecurityBasicHTTPS Security { get { return (WcfSecurityBasicHTTPS)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(WcfSecurityBasicHTTPS), typeof(WcfBindingBasicHTTPS));

		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(WcfBindingBasicHTTPS));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(WcfBindingBasicHTTPS));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(WcfBindingBasicHTTPS));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(WcfBindingBasicHTTPS));

		public long MaxBufferSize { get { return (long)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(long), typeof(WcfBindingBasicHTTPS));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(WcfBindingBasicHTTPS));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(WcfBindingBasicHTTPS));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(WcfBindingBasicHTTPS));

		public WcfBindingTextEncoding TextEncoding { get { return (WcfBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(WcfBindingTextEncoding), typeof(WcfBindingBasicHTTPS));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(WcfBindingBasicHTTPS));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(WcfBindingBasicHTTPS));

		public WcfBindingBasicHTTPS() : base() { }

		public WcfBindingBasicHTTPS(string Name, Namespace Parent) : base()
		{
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			InheritedTypes.Add(new DataType("System.ServiceModel.BasicHttpsBinding", DataTypeMode.Class));
			ID = Guid.NewGuid();
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;
			Security = new WcfSecurityBasicHTTPS(this);

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
			TextEncoding = WcfBindingTextEncoding.UTF8;
			TransferMode = System.ServiceModel.TransferMode.Buffered;
			UseDefaultWebProxy = true;
		}
	}

	#endregion

	#region  - WcfBindingNetHTTP Class -

	public class WcfBindingNetHTTP : WcfBinding
	{
		public WcfSecurityBasicHTTP Security { get { return (WcfSecurityBasicHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(WcfSecurityBasicHTTP), typeof(WcfBindingNetHTTP));

		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(WcfBindingNetHTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(WcfBindingNetHTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(WcfBindingNetHTTP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(WcfBindingNetHTTP));

		public long MaxBufferSize { get { return (long)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(long), typeof(WcfBindingNetHTTP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(WcfBindingNetHTTP));

		public System.ServiceModel.NetHttpMessageEncoding MessageEncoding { get { return (System.ServiceModel.NetHttpMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.NetHttpMessageEncoding), typeof(WcfBindingNetHTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(WcfBindingNetHTTP));

		public WcfBindingTextEncoding TextEncoding { get { return (WcfBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(WcfBindingTextEncoding), typeof(WcfBindingNetHTTP));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(WcfBindingNetHTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(WcfBindingNetHTTP));

		//WebSocket Settings
		public bool CreateNotificationOnConnection { get { return (bool)GetValue(CreateNotificationOnConnectionProperty); } set { SetValue(CreateNotificationOnConnectionProperty, value); } }
		public static readonly DependencyProperty CreateNotificationOnConnectionProperty = DependencyProperty.Register("CreateNotificationOnConnection", typeof(bool), typeof(WcfBindingNetHTTP));

		public bool DisablePayloadMasking { get { return (bool)GetValue(DisablePayloadMaskingProperty); } set { SetValue(DisablePayloadMaskingProperty, value); } }
		public static readonly DependencyProperty DisablePayloadMaskingProperty = DependencyProperty.Register("DisablePayloadMasking", typeof(bool), typeof(WcfBindingNetHTTP));

		public TimeSpan KeepAliveInterval { get { return (TimeSpan)GetValue(KeepAliveIntervalProperty); } set { SetValue(KeepAliveIntervalProperty, value); } }
		public static readonly DependencyProperty KeepAliveIntervalProperty = DependencyProperty.Register("KeepAliveInterval", typeof(TimeSpan), typeof(WcfBindingNetHTTP), new PropertyMetadata(new TimeSpan(0, 10, 0)));

		public int MaxPendingConnections { get { return (int)GetValue(MaxPendingConnectionsProperty); } set { SetValue(MaxPendingConnectionsProperty, value); } }
		public static readonly DependencyProperty MaxPendingConnectionsProperty = DependencyProperty.Register("MaxPendingConnections", typeof(int), typeof(WcfBindingNetHTTP));

		public int ReceiveBufferSize { get { return (int)GetValue(ReceiveBufferSizeProperty); } set { SetValue(ReceiveBufferSizeProperty, value); } }
		public static readonly DependencyProperty ReceiveBufferSizeProperty = DependencyProperty.Register("ReceiveBufferSize", typeof(int), typeof(WcfBindingNetHTTP));

		public int SendBufferSize { get { return (int)GetValue(SendBufferSizeProperty); } set { SetValue(SendBufferSizeProperty, value); } }
		public static readonly DependencyProperty SendBufferSizeProperty = DependencyProperty.Register("SendBufferSize", typeof(int), typeof(WcfBindingNetHTTP));

		public string SubProtocol { get { return (string)GetValue(SubProtocolProperty); } set { SetValue(SubProtocolProperty, value); } }
		public static readonly DependencyProperty SubProtocolProperty = DependencyProperty.Register("SubProtocol", typeof(string), typeof(WcfBindingNetHTTP));

		public System.ServiceModel.Channels.WebSocketTransportUsage TransportUsage { get { return (System.ServiceModel.Channels.WebSocketTransportUsage)GetValue(TransportUsageProperty); } set { SetValue(TransportUsageProperty, value); } }
		public static readonly DependencyProperty TransportUsageProperty = DependencyProperty.Register("TransportUsage", typeof(System.ServiceModel.Channels.WebSocketTransportUsage), typeof(WcfBindingNetHTTP));

		//Reliable Sessions
		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(WcfBindingNetHTTP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(WcfBindingNetHTTP), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(WcfBindingNetHTTP));

		public WcfBindingNetHTTP() : base() { }

		public WcfBindingNetHTTP(string Name, Namespace Parent) : base()
		{
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			InheritedTypes.Add(new DataType("System.ServiceModel.NetHttpBinding", DataTypeMode.Class));
			ID = Guid.NewGuid();
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;
			Security = new WcfSecurityBasicHTTP(this);

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
			TextEncoding = WcfBindingTextEncoding.UTF8;
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

	#region  - WcfBindingNetHTTPS Class -

	public class WcfBindingNetHTTPS : WcfBinding
	{
		public WcfSecurityBasicHTTPS Security { get { return (WcfSecurityBasicHTTPS)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(WcfSecurityBasicHTTPS), typeof(WcfBindingNetHTTPS));

		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(WcfBindingNetHTTPS));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(WcfBindingNetHTTPS));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(WcfBindingNetHTTPS));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(WcfBindingNetHTTPS));

		public long MaxBufferSize { get { return (long)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(long), typeof(WcfBindingNetHTTPS));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(WcfBindingNetHTTPS));

		public System.ServiceModel.NetHttpMessageEncoding MessageEncoding { get { return (System.ServiceModel.NetHttpMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.NetHttpMessageEncoding), typeof(WcfBindingNetHTTPS));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(WcfBindingNetHTTPS));

		public WcfBindingTextEncoding TextEncoding { get { return (WcfBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(WcfBindingTextEncoding), typeof(WcfBindingNetHTTPS));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(WcfBindingNetHTTPS));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(WcfBindingNetHTTPS));

		//WebSocket Settings
		public bool CreateNotificationOnConnection { get { return (bool)GetValue(CreateNotificationOnConnectionProperty); } set { SetValue(CreateNotificationOnConnectionProperty, value); } }
		public static readonly DependencyProperty CreateNotificationOnConnectionProperty = DependencyProperty.Register("CreateNotificationOnConnection", typeof(bool), typeof(WcfBindingNetHTTPS));

		public bool DisablePayloadMasking { get { return (bool)GetValue(DisablePayloadMaskingProperty); } set { SetValue(DisablePayloadMaskingProperty, value); } }
		public static readonly DependencyProperty DisablePayloadMaskingProperty = DependencyProperty.Register("DisablePayloadMasking", typeof(bool), typeof(WcfBindingNetHTTPS));

		public TimeSpan KeepAliveInterval { get { return (TimeSpan)GetValue(KeepAliveIntervalProperty); } set { SetValue(KeepAliveIntervalProperty, value); } }
		public static readonly DependencyProperty KeepAliveIntervalProperty = DependencyProperty.Register("KeepAliveInterval", typeof(TimeSpan), typeof(WcfBindingNetHTTPS), new PropertyMetadata(new TimeSpan(0, 10, 0)));

		public int MaxPendingConnections { get { return (int)GetValue(MaxPendingConnectionsProperty); } set { SetValue(MaxPendingConnectionsProperty, value); } }
		public static readonly DependencyProperty MaxPendingConnectionsProperty = DependencyProperty.Register("MaxPendingConnections", typeof(int), typeof(WcfBindingNetHTTPS));

		public int ReceiveBufferSize { get { return (int)GetValue(ReceiveBufferSizeProperty); } set { SetValue(ReceiveBufferSizeProperty, value); } }
		public static readonly DependencyProperty ReceiveBufferSizeProperty = DependencyProperty.Register("ReceiveBufferSize", typeof(int), typeof(WcfBindingNetHTTPS));

		public int SendBufferSize { get { return (int)GetValue(SendBufferSizeProperty); } set { SetValue(SendBufferSizeProperty, value); } }
		public static readonly DependencyProperty SendBufferSizeProperty = DependencyProperty.Register("SendBufferSize", typeof(int), typeof(WcfBindingNetHTTPS));

		public string SubProtocol { get { return (string)GetValue(SubProtocolProperty); } set { SetValue(SubProtocolProperty, value); } }
		public static readonly DependencyProperty SubProtocolProperty = DependencyProperty.Register("SubProtocol", typeof(string), typeof(WcfBindingNetHTTPS));

		public System.ServiceModel.Channels.WebSocketTransportUsage TransportUsage { get { return (System.ServiceModel.Channels.WebSocketTransportUsage)GetValue(TransportUsageProperty); } set { SetValue(TransportUsageProperty, value); } }
		public static readonly DependencyProperty TransportUsageProperty = DependencyProperty.Register("TransportUsage", typeof(System.ServiceModel.Channels.WebSocketTransportUsage), typeof(WcfBindingNetHTTPS));

		//Reliable Sessions
		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(WcfBindingNetHTTPS));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(WcfBindingNetHTTPS), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(WcfBindingNetHTTPS));

		public WcfBindingNetHTTPS() : base() { }

		public WcfBindingNetHTTPS(string Name, Namespace Parent) : base()
		{
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			InheritedTypes.Add(new DataType("System.ServiceModel.NetHttpsBinding", DataTypeMode.Class));
			ID = Guid.NewGuid();
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;
			Security = new WcfSecurityBasicHTTPS(this);

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
			TextEncoding = WcfBindingTextEncoding.UTF8;
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

	#region - WcfBindingWSHTTP Class -

	public class WcfBindingWSHTTP : WcfBinding
	{
		public WcfSecurityWSHTTP Security { get { return (WcfSecurityWSHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(WcfSecurityWSHTTP), typeof(WcfBindingWSHTTP));

		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(WcfBindingWSHTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(WcfBindingWSHTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(WcfBindingWSHTTP));

		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(WcfBindingWSHTTP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(WcfBindingWSHTTP), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(WcfBindingWSHTTP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(WcfBindingWSHTTP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(WcfBindingWSHTTP));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(WcfBindingWSHTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(WcfBindingWSHTTP));

		public WcfBindingTextEncoding TextEncoding { get { return (WcfBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(WcfBindingTextEncoding), typeof(WcfBindingWSHTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(WcfBindingWSHTTP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(WcfBindingWSHTTP));

		//ContextBinding
		public bool UseContextBinding { get { return (bool)GetValue(UseContextBindingProperty); } set { SetValue(UseContextBindingProperty, value); } }
		public static readonly DependencyProperty UseContextBindingProperty = DependencyProperty.Register("UseContextBinding", typeof(bool), typeof(WcfBindingWSHTTP), new PropertyMetadata(false, UseContextBindingPropertyChangedCallback));

		private static void UseContextBindingPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfBindingWSHTTP;
			if (t == null) return;
			if (t.InheritedTypes == null) return;

			if ((bool)e.NewValue)
			{
				var til = new List<DataType>();
				foreach (var it in t.InheritedTypes)
					if (it.Name == "System.ServiceModel.WSHttpBinding")
						til.Add(it);
				foreach (var it in til)
					t.InheritedTypes.Remove(it);

				t.InheritedTypes.Add(new DataType("System.ServiceModel.WSHttpContextBinding", DataTypeMode.Class));
			}
			else
			{
				var til = new List<DataType>();
				foreach (var it in t.InheritedTypes)
					if (it.Name == "System.ServiceModel.WSHttpContextBinding")
						til.Add(it);
				foreach (var it in til)
					t.InheritedTypes.Remove(it);

				t.InheritedTypes.Add(new DataType("System.ServiceModel.WSHttpBinding", DataTypeMode.Class));

			}
		}

		public bool ContextManagementEnabled { get { return (bool)GetValue(ContextManagementEnabledProperty); } set { SetValue(ContextManagementEnabledProperty, value); } }
		public static readonly DependencyProperty ContextManagementEnabledProperty = DependencyProperty.Register("ContextManagementEnabled", typeof(bool), typeof(WcfBindingWSHTTP), new PropertyMetadata(false));

		public System.Net.Security.ProtectionLevel ContextProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(ContextProtectionLevelProperty); } set { SetValue(ContextProtectionLevelProperty, value); } }
		public static readonly DependencyProperty ContextProtectionLevelProperty = DependencyProperty.Register("ContextProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(WcfBindingWSHTTP), new PropertyMetadata(System.Net.Security.ProtectionLevel.None));

		public WcfBindingWSHTTP() : base() { }

		public WcfBindingWSHTTP(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.WSHttpBinding", DataTypeMode.Class));
			Security = new WcfSecurityWSHTTP(this);

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
			TextEncoding = WcfBindingTextEncoding.UTF8;
			TransactionFlow = true;
			UseDefaultWebProxy = true;
		}
	}

	#endregion

	#region - WcfBindingWS2007HTTP Class -

	public class WcfBindingWS2007HTTP : WcfBinding
	{
		public WcfSecurityWSHTTP Security { get { return (WcfSecurityWSHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(WcfSecurityWSHTTP), typeof(WcfBindingWSHTTP));

		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(WcfBindingWS2007HTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(WcfBindingWS2007HTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(WcfBindingWS2007HTTP));

		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(WcfBindingWS2007HTTP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(WcfBindingWS2007HTTP), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(WcfBindingWS2007HTTP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(WcfBindingWS2007HTTP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(WcfBindingWS2007HTTP));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(WcfBindingWS2007HTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(WcfBindingWS2007HTTP));

		public WcfBindingTextEncoding TextEncoding { get { return (WcfBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(WcfBindingTextEncoding), typeof(WcfBindingWS2007HTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(WcfBindingWS2007HTTP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(WcfBindingWS2007HTTP));

		public WcfBindingWS2007HTTP() : base() { }

		public WcfBindingWS2007HTTP(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.WS2007HttpBinding", DataTypeMode.Class));
			Security = new WcfSecurityWSHTTP(this);

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
			TextEncoding = WcfBindingTextEncoding.UTF8;
			TransactionFlow = true;
			UseDefaultWebProxy = true;
		}
	}

	#endregion

	#region - WcfBindingWSDualHTTP Class -

	public class WcfBindingWSDualHTTP : WcfBinding
	{
		public WcfSecurityWSDualHTTP Security { get { return (WcfSecurityWSDualHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(WcfSecurityWSDualHTTP), typeof(WcfBindingWSDualHTTP));

		public string ClientBaseAddress { get { return (string)GetValue(ClientBaseAddressProperty); } set { SetValue(ClientBaseAddressProperty, value); } }
		public static readonly DependencyProperty ClientBaseAddressProperty = DependencyProperty.Register("ClientBaseAddress", typeof(string), typeof(WcfBindingWSDualHTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(WcfBindingWS2007HTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(WcfBindingWS2007HTTP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(WcfBindingWS2007HTTP), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(WcfBindingWS2007HTTP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(WcfBindingWS2007HTTP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(WcfBindingWS2007HTTP));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(WcfBindingWS2007HTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(WcfBindingWS2007HTTP));

		public WcfBindingTextEncoding TextEncoding { get { return (WcfBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(WcfBindingTextEncoding), typeof(WcfBindingWS2007HTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(WcfBindingWS2007HTTP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(WcfBindingWS2007HTTP));

		public WcfBindingWSDualHTTP() : base() { }

		public WcfBindingWSDualHTTP(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.WSDualHttpBinding", DataTypeMode.Class));
			Security = new WcfSecurityWSDualHTTP(this);

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
			TextEncoding = WcfBindingTextEncoding.UTF8;
			TransactionFlow = true;
			UseDefaultWebProxy = true;
		}
	}

	#endregion

	#region - WcfBindingWSFederationHTTP Class -

	public class WcfBindingWSFederationHTTP : WcfBinding
	{
		public WcfSecurityWSFederationHTTP Security { get { return (WcfSecurityWSFederationHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(WcfSecurityWSFederationHTTP), typeof(WcfBindingWSFederationHTTP));
		
		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(WcfBindingWSFederationHTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(WcfBindingWSFederationHTTP));

		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(WcfBindingWSFederationHTTP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(WcfBindingWSFederationHTTP), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(WcfBindingWSFederationHTTP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(WcfBindingWSFederationHTTP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(WcfBinding));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(WcfBindingWSFederationHTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(WcfBindingWSFederationHTTP));

		public WcfBindingTextEncoding TextEncoding { get { return (WcfBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(WcfBindingTextEncoding), typeof(WcfBindingWSFederationHTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(WcfBindingWSFederationHTTP));

		public string PrivacyNoticeAt { get { return (string)GetValue(PrivacyNoticeAtProperty); } set { SetValue(PrivacyNoticeAtProperty, value); } }
		public static readonly DependencyProperty PrivacyNoticeAtProperty = DependencyProperty.Register("PrivacyNoticeAt", typeof(string), typeof(WcfBindingWSFederationHTTP));

		public int PrivacyNoticeVersion { get { return (int)GetValue(PrivacyNoticeVersionProperty); } set { SetValue(PrivacyNoticeVersionProperty, value); } }
		public static readonly DependencyProperty PrivacyNoticeVersionProperty = DependencyProperty.Register("PrivacyNoticeVersion", typeof(int), typeof(WcfBindingWSFederationHTTP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(WcfBindingWSFederationHTTP));

		public WcfBindingWSFederationHTTP() : base() { }

		public WcfBindingWSFederationHTTP(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.WSFederationHttpBinding", DataTypeMode.Class));
			Security = new WcfSecurityWSFederationHTTP(this);

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
			TextEncoding = WcfBindingTextEncoding.UTF8;
			TransactionFlow = true;
			UseDefaultWebProxy = true;
		}
	}

	#endregion

	#region - WcfBindingWS2007FederationHTTP Class -

	public class WcfBindingWS2007FederationHTTP : WcfBinding
	{
		public WcfSecurityWSFederationHTTP Security { get { return (WcfSecurityWSFederationHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(WcfSecurityWSFederationHTTP), typeof(WcfBindingWS2007FederationHTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(WcfBindingWS2007FederationHTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(WcfBindingWS2007FederationHTTP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(WcfBindingWS2007FederationHTTP), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(WcfBindingWS2007FederationHTTP));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(WcfBindingWS2007FederationHTTP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(WcfBindingWS2007FederationHTTP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(WcfBinding));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(WcfBindingWS2007FederationHTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(WcfBindingWS2007FederationHTTP));

		public WcfBindingTextEncoding TextEncoding { get { return (WcfBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(WcfBindingTextEncoding), typeof(WcfBindingWS2007FederationHTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(WcfBindingWS2007FederationHTTP));

		public string PrivacyNoticeAt { get { return (string)GetValue(PrivacyNoticeAtProperty); } set { SetValue(PrivacyNoticeAtProperty, value); } }
		public static readonly DependencyProperty PrivacyNoticeAtProperty = DependencyProperty.Register("PrivacyNoticeAt", typeof(string), typeof(WcfBindingWS2007FederationHTTP));

		public int PrivacyNoticeVersion { get { return (int)GetValue(PrivacyNoticeVersionProperty); } set { SetValue(PrivacyNoticeVersionProperty, value); } }
		public static readonly DependencyProperty PrivacyNoticeVersionProperty = DependencyProperty.Register("PrivacyNoticeVersion", typeof(int), typeof(WcfBindingWS2007FederationHTTP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(WcfBindingWS2007FederationHTTP));

		public WcfBindingWS2007FederationHTTP() : base() { }

		public WcfBindingWS2007FederationHTTP(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.WS2007FederationHttpBinding", DataTypeMode.Class));
			Security = new WcfSecurityWSFederationHTTP(this);

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
			TextEncoding = WcfBindingTextEncoding.UTF8;
			TransactionFlow = true;
			UseDefaultWebProxy = true;
		}
	}

	#endregion

	#region - WcfBindingTCP Class -

	public class WcfBindingTCP : WcfBinding
	{
		public WcfSecurityTCP Security { get { return (WcfSecurityTCP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(WcfSecurityTCP), typeof(WcfBindingTCP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(WcfBindingTCP));

		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(WcfBindingTCP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(WcfBindingTCP), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(WcfBindingTCP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(WcfBindingTCP));

		public long MaxBufferSize { get { return (long)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(long), typeof(WcfBindingTCP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(WcfBinding));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(WcfBindingTCP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(WcfBindingTCP));

		public int ListenBacklog { get { return (int)GetValue(ListenBacklogProperty); } set { SetValue(ListenBacklogProperty, value); } }
		public static readonly DependencyProperty ListenBacklogProperty = DependencyProperty.Register("ListenBacklog", typeof(int), typeof(WcfBindingTCP));

		public int MaxConnections { get { return (int)GetValue(MaxConnectionsProperty); } set { SetValue(MaxConnectionsProperty, value); } }
		public static readonly DependencyProperty MaxConnectionsProperty = DependencyProperty.Register("MaxConnections", typeof(int), typeof(WcfBindingTCP));

		public bool PortSharingEnabled { get { return (bool)GetValue(PortSharingEnabledProperty); } set { SetValue(PortSharingEnabledProperty, value); } }
		public static readonly DependencyProperty PortSharingEnabledProperty = DependencyProperty.Register("PortSharingEnabled", typeof(bool), typeof(WcfBindingTCP));

		public WcfBindingTransactionProtocol TransactionProtocol { get { return (WcfBindingTransactionProtocol)GetValue(TransactionProtocolProperty); } set { SetValue(TransactionProtocolProperty, value); } }
		public static readonly DependencyProperty TransactionProtocolProperty = DependencyProperty.Register("TransactionProtocol", typeof(WcfBindingTransactionProtocol), typeof(WcfBindingTCP));

		//ContextBinding
		public bool UseContextBinding { get { return (bool)GetValue(UseContextBindingProperty); } set { SetValue(UseContextBindingProperty, value); } }
		public static readonly DependencyProperty UseContextBindingProperty = DependencyProperty.Register("UseContextBinding", typeof(bool), typeof(WcfBindingTCP), new PropertyMetadata(false, UseContextBindingPropertyChangedCallback));

		private static void UseContextBindingPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfBindingTCP;
			if (t == null) return;
			if (t.InheritedTypes == null) return;

			if ((bool)e.NewValue)
			{
				var til = new List<DataType>();
				foreach (var it in t.InheritedTypes)
					if (it.Name == "System.ServiceModel.NetTcpBinding")
						til.Add(it);
				foreach (var it in til)
					t.InheritedTypes.Remove(it);

				t.InheritedTypes.Add(new DataType("System.ServiceModel.NetTcpContextBinding", DataTypeMode.Class));
			}
			else
			{
				var til = new List<DataType>();
				foreach (var it in t.InheritedTypes)
					if (it.Name == "System.ServiceModel.NetTcpContextBinding")
						til.Add(it);
				foreach (var it in til)
					t.InheritedTypes.Remove(it);

				t.InheritedTypes.Add(new DataType("System.ServiceModel.NetTcpBinding", DataTypeMode.Class));
			}
		}

		public bool ContextManagementEnabled { get { return (bool)GetValue(ContextManagementEnabledProperty); } set { SetValue(ContextManagementEnabledProperty, value); } }
		public static readonly DependencyProperty ContextManagementEnabledProperty = DependencyProperty.Register("ContextManagementEnabled", typeof(bool), typeof(WcfBindingTCP), new PropertyMetadata(false));

		public System.Net.Security.ProtectionLevel ContextProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(ContextProtectionLevelProperty); } set { SetValue(ContextProtectionLevelProperty, value); } }
		public static readonly DependencyProperty ContextProtectionLevelProperty = DependencyProperty.Register("ContextProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(WcfBindingTCP), new PropertyMetadata(System.Net.Security.ProtectionLevel.None));

		public WcfBindingTCP() : base() { }

		public WcfBindingTCP(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.NetTcpBinding", DataTypeMode.Class));
			Security = new WcfSecurityTCP(this);

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
			TransactionProtocol = WcfBindingTransactionProtocol.Default;
			TransferMode = System.ServiceModel.TransferMode.Buffered;
		}
	}

	#endregion

	#region - WcfBindingNamedPipe Class -

	public class WcfBindingNamedPipe : WcfBinding
	{
		public WcfSecurityNamedPipe Security { get { return (WcfSecurityNamedPipe)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(WcfSecurityNamedPipe), typeof(WcfBindingNamedPipe));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(WcfBindingNamedPipe));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(WcfBindingNamedPipe));

		public long MaxBufferSize { get { return (long)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(long), typeof(WcfBindingNamedPipe));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(WcfBindingNamedPipe));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(WcfBindingNamedPipe));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(WcfBindingNamedPipe));

		public int MaxConnections { get { return (int)GetValue(MaxConnectionsProperty); } set { SetValue(MaxConnectionsProperty, value); } }
		public static readonly DependencyProperty MaxConnectionsProperty = DependencyProperty.Register("MaxConnections", typeof(int), typeof(WcfBindingNamedPipe));

		public WcfBindingTransactionProtocol TransactionProtocol { get { return (WcfBindingTransactionProtocol)GetValue(TransactionProtocolProperty); } set { SetValue(TransactionProtocolProperty, value); } }
		public static readonly DependencyProperty TransactionProtocolProperty = DependencyProperty.Register("TransactionProtocol", typeof(WcfBindingTransactionProtocol), typeof(WcfBindingNamedPipe));

		public WcfBindingNamedPipe() : base() { }

		public WcfBindingNamedPipe(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.NetNamedPipeBinding", DataTypeMode.Class));
			Security = new WcfSecurityNamedPipe(this);

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
			TransactionProtocol = WcfBindingTransactionProtocol.Default;
			TransferMode = System.ServiceModel.TransferMode.Buffered;
		}
	}

	#endregion

	#region - WcfBindingMSMQ Class -

	public class WcfBindingMSMQ : WcfBinding
	{
		public WcfSecurityMSMQ Security { get { return (WcfSecurityMSMQ)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(WcfSecurityMSMQ), typeof(WcfBindingMSMQ));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(WcfBindingMSMQ));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(WcfBindingMSMQ));

		public string CustomDeadLetterQueue { get { return (string)GetValue(CustomDeadLetterQueueProperty); } set { SetValue(CustomDeadLetterQueueProperty, value); } }
		public static readonly DependencyProperty CustomDeadLetterQueueProperty = DependencyProperty.Register("CustomDeadLetterQueue", typeof(string), typeof(WcfBindingMSMQ));

		public System.ServiceModel.DeadLetterQueue DeadLetterQueue { get { return (System.ServiceModel.DeadLetterQueue)GetValue(DeadLetterQueueProperty); } set { SetValue(DeadLetterQueueProperty, value); } }
		public static readonly DependencyProperty DeadLetterQueueProperty = DependencyProperty.Register("DeadLetterQueue", typeof(System.ServiceModel.DeadLetterQueue), typeof(WcfBindingMSMQ));

		public bool ExactlyOnce { get { return (bool)GetValue(ExactlyOnceProperty); } set { SetValue(ExactlyOnceProperty, value); } }
		public static readonly DependencyProperty ExactlyOnceProperty = DependencyProperty.Register("ExactlyOnce", typeof(bool), typeof(WcfBindingMSMQ));

		public int MaxRetryCycles { get { return (int)GetValue(MaxRetryCyclesProperty); } set { SetValue(MaxRetryCyclesProperty, value); } }
		public static readonly DependencyProperty MaxRetryCyclesProperty = DependencyProperty.Register("MaxRetryCycles", typeof(int), typeof(WcfBindingMSMQ));

		public System.ServiceModel.QueueTransferProtocol QueueTransferProtocol { get { return (System.ServiceModel.QueueTransferProtocol)GetValue(QueueTransferProtocolProperty); } set { SetValue(QueueTransferProtocolProperty, value); } }
		public static readonly DependencyProperty QueueTransferProtocolProperty = DependencyProperty.Register("QueueTransferProtocol", typeof(System.ServiceModel.QueueTransferProtocol), typeof(WcfBindingMSMQ));

		public bool ReceiveContextEnabled { get { return (bool)GetValue(ReceiveContextEnabledProperty); } set { SetValue(ReceiveContextEnabledProperty, value); } }
		public static readonly DependencyProperty ReceiveContextEnabledProperty = DependencyProperty.Register("ReceiveContextEnabled", typeof(bool), typeof(WcfBindingMSMQ));

		public System.ServiceModel.ReceiveErrorHandling ReceiveErrorHandling { get { return (System.ServiceModel.ReceiveErrorHandling)GetValue(ReceiveErrorHandlingProperty); } set { SetValue(ReceiveErrorHandlingProperty, value); } }
		public static readonly DependencyProperty ReceiveErrorHandlingProperty = DependencyProperty.Register("ReceiveErrorHandling", typeof(System.ServiceModel.ReceiveErrorHandling), typeof(WcfBindingMSMQ));

		public int ReceiveRetryCount { get { return (int)GetValue(ReceiveRetryCountProperty); } set { SetValue(ReceiveRetryCountProperty, value); } }
		public static readonly DependencyProperty ReceiveRetryCountProperty = DependencyProperty.Register("ReceiveRetryCount", typeof(int), typeof(WcfBindingMSMQ));

		public TimeSpan RetryCycleDelay { get { return (TimeSpan)GetValue(RetryCycleDelayProperty); } set { SetValue(RetryCycleDelayProperty, value); } }
		public static readonly DependencyProperty RetryCycleDelayProperty = DependencyProperty.Register("RetryCycleDelay", typeof(TimeSpan), typeof(WcfBindingMSMQ), new PropertyMetadata(new TimeSpan(0, 1, 0)));

		public TimeSpan TimeToLive { get { return (TimeSpan)GetValue(TimeToLiveProperty); } set { SetValue(TimeToLiveProperty, value); } }
		public static readonly DependencyProperty TimeToLiveProperty = DependencyProperty.Register("TimeToLive", typeof(TimeSpan), typeof(WcfBindingMSMQ), new PropertyMetadata(new TimeSpan(0, 10, 0)));

		public bool UseActiveDirectory { get { return (bool)GetValue(UseActiveDirectoryProperty); } set { SetValue(UseActiveDirectoryProperty, value); } }
		public static readonly DependencyProperty UseActiveDirectoryProperty = DependencyProperty.Register("UseActiveDirectory", typeof(bool), typeof(WcfBindingMSMQ));

		public bool UseMSMQTracing { get { return (bool)GetValue(UseMSMQTracingProperty); } set { SetValue(UseMSMQTracingProperty, value); } }
		public static readonly DependencyProperty UseMSMQTracingProperty = DependencyProperty.Register("UseMSMQTracing", typeof(bool), typeof(WcfBindingMSMQ));

		public bool UseSourceJournal { get { return (bool)GetValue(UseSourceJournalProperty); } set { SetValue(UseSourceJournalProperty, value); } }
		public static readonly DependencyProperty UseSourceJournalProperty = DependencyProperty.Register("UseSourceJournal", typeof(bool), typeof(WcfBindingMSMQ));

		public TimeSpan ValidityDuration { get { return (TimeSpan)GetValue(ValidityDurationProperty); } set { SetValue(ValidityDurationProperty, value); } }
		public static readonly DependencyProperty ValidityDurationProperty = DependencyProperty.Register("ValidityDuration", typeof(TimeSpan), typeof(WcfBindingMSMQ), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool Durable { get { return (bool)GetValue(DurableProperty); } set { SetValue(DurableProperty, value); } }
		public static readonly DependencyProperty DurableProperty = DependencyProperty.Register("Durable", typeof(bool), typeof(WcfBindingMSMQ));

		public WcfBindingMSMQ() : base() { }

		public WcfBindingMSMQ(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.NetMsmqBinding", DataTypeMode.Class));
			Security = new WcfSecurityMSMQ(this);

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

	#region - WcfBindingPeerTCP Class -

	public class WcfBindingPeerTCP : WcfBinding
	{
		public WcfSecurityPeerTCP Security { get { return (WcfSecurityPeerTCP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(WcfSecurityPeerTCP), typeof(WcfBindingPeerTCP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(WcfBindingPeerTCP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(WcfBindingPeerTCP));

		public string ListenIPAddress { get { return (string)GetValue(ListenIPAddressProperty); } set { SetValue(ListenIPAddressProperty, value); } }
		public static readonly DependencyProperty ListenIPAddressProperty = DependencyProperty.Register("ListenIPAddress", typeof(string), typeof(WcfBindingPeerTCP));
		
		public int Port { get { return (int)GetValue(PortProperty); } set { SetValue(PortProperty, value); } }
		public static readonly DependencyProperty PortProperty = DependencyProperty.Register("Port", typeof(int), typeof(WcfBindingPeerTCP));

		public WcfBindingPeerTCP() : base() { }

		public WcfBindingPeerTCP(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.NetPeerTcpBinding", DataTypeMode.Class));
			Security = new WcfSecurityPeerTCP(this);

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

	#region - WcfBindingWebHTTP Class -

	public class WcfBindingWebHTTP : WcfBinding
	{
		public WcfSecurityWebHTTP Security { get { return (WcfSecurityWebHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(WcfSecurityWebHTTP), typeof(WcfBindingWebHTTP));
				
		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(WcfBindingWebHTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(WcfBindingWebHTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(WcfBindingWebHTTP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(WcfBindingWebHTTP));

		public long MaxBufferSize { get { return (long)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(long), typeof(WcfBindingWebHTTP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(WcfBindingWebHTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(WcfBindingWebHTTP));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(WcfBindingWebHTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(WcfBindingWebHTTP));

		public bool CrossDomainScriptAccessEnabled { get { return (bool)GetValue(CrossDomainScriptAccessEnabledProperty); } set { SetValue(CrossDomainScriptAccessEnabledProperty, value); } }
		public static readonly DependencyProperty CrossDomainScriptAccessEnabledProperty = DependencyProperty.Register("CrossDomainScriptAccessEnabled", typeof(bool), typeof(WcfBindingWebHTTP));

		public WcfBindingTextEncoding WriteEncoding { get { return (WcfBindingTextEncoding)GetValue(WriteEncodingProperty); } set { SetValue(WriteEncodingProperty, value); } }
		public static readonly DependencyProperty WriteEncodingProperty = DependencyProperty.Register("WriteEncoding", typeof(WcfBindingTextEncoding), typeof(WcfBindingWebHTTP));

		public WcfBindingWebHTTP() : base()
		{
			InheritedTypes.Add(new DataType("System.ServiceModel.WebHttpBinding", DataTypeMode.Class));
			Security = new WcfSecurityWebHTTP(this);

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
			WriteEncoding = WcfBindingTextEncoding.UTF8;
		}

		public WcfBindingWebHTTP(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.WebHttpBinding", DataTypeMode.Class));
			Security = new WcfSecurityWebHTTP(this);

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
			WriteEncoding = WcfBindingTextEncoding.UTF8;
		}
	}

	#endregion 

	#region - WcfBindingMSMQIntegration Class -

	public class WcfBindingMSMQIntegration : WcfBinding
	{
		public WcfSecurityMSMQIntegration Security { get { return (WcfSecurityMSMQIntegration)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(WcfSecurityMSMQIntegration), typeof(WcfBindingMSMQIntegration));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(WcfBindingMSMQIntegration));

		public string CustomDeadLetterQueue { get { return (string)GetValue(CustomDeadLetterQueueProperty); } set { SetValue(CustomDeadLetterQueueProperty, value); } }
		public static readonly DependencyProperty CustomDeadLetterQueueProperty = DependencyProperty.Register("CustomDeadLetterQueue", typeof(string), typeof(WcfBindingMSMQIntegration));

		public System.ServiceModel.DeadLetterQueue DeadLetterQueue { get { return (System.ServiceModel.DeadLetterQueue)GetValue(DeadLetterQueueProperty); } set { SetValue(DeadLetterQueueProperty, value); } }
		public static readonly DependencyProperty DeadLetterQueueProperty = DependencyProperty.Register("DeadLetterQueue", typeof(System.ServiceModel.DeadLetterQueue), typeof(WcfBindingMSMQIntegration));

		public bool ExactlyOnce { get { return (bool)GetValue(ExactlyOnceProperty); } set { SetValue(ExactlyOnceProperty, value); } }
		public static readonly DependencyProperty ExactlyOnceProperty = DependencyProperty.Register("ExactlyOnce", typeof(bool), typeof(WcfBindingMSMQIntegration));

		public int MaxRetryCycles { get { return (int)GetValue(MaxRetryCyclesProperty); } set { SetValue(MaxRetryCyclesProperty, value); } }
		public static readonly DependencyProperty MaxRetryCyclesProperty = DependencyProperty.Register("MaxRetryCycles", typeof(int), typeof(WcfBindingMSMQIntegration));

		public bool ReceiveContextEnabled { get { return (bool)GetValue(ReceiveContextEnabledProperty); } set { SetValue(ReceiveContextEnabledProperty, value); } }
		public static readonly DependencyProperty ReceiveContextEnabledProperty = DependencyProperty.Register("ReceiveContextEnabled", typeof(bool), typeof(WcfBindingMSMQIntegration));

		public System.ServiceModel.ReceiveErrorHandling ReceiveErrorHandling { get { return (System.ServiceModel.ReceiveErrorHandling)GetValue(ReceiveErrorHandlingProperty); } set { SetValue(ReceiveErrorHandlingProperty, value); } }
		public static readonly DependencyProperty ReceiveErrorHandlingProperty = DependencyProperty.Register("ReceiveErrorHandling", typeof(System.ServiceModel.ReceiveErrorHandling), typeof(WcfBindingMSMQIntegration));

		public int ReceiveRetryCount { get { return (int)GetValue(ReceiveRetryCountProperty); } set { SetValue(ReceiveRetryCountProperty, value); } }
		public static readonly DependencyProperty ReceiveRetryCountProperty = DependencyProperty.Register("ReceiveRetryCount", typeof(int), typeof(WcfBindingMSMQIntegration));

		public TimeSpan RetryCycleDelay { get { return (TimeSpan)GetValue(RetryCycleDelayProperty); } set { SetValue(RetryCycleDelayProperty, value); } }
		public static readonly DependencyProperty RetryCycleDelayProperty = DependencyProperty.Register("RetryCycleDelay", typeof(TimeSpan), typeof(WcfBindingMSMQIntegration), new PropertyMetadata(new TimeSpan(0, 1, 0)));

		public TimeSpan TimeToLive { get { return (TimeSpan)GetValue(TimeToLiveProperty); } set { SetValue(TimeToLiveProperty, value); } }
		public static readonly DependencyProperty TimeToLiveProperty = DependencyProperty.Register("TimeToLive", typeof(TimeSpan), typeof(WcfBindingMSMQIntegration), new PropertyMetadata(new TimeSpan(0, 10, 0)));

		public bool UseMSMQTracing { get { return (bool)GetValue(UseMSMQTracingProperty); } set { SetValue(UseMSMQTracingProperty, value); } }
		public static readonly DependencyProperty UseMSMQTracingProperty = DependencyProperty.Register("UseMSMQTracing", typeof(bool), typeof(WcfBindingMSMQIntegration));

		public bool UseSourceJournal { get { return (bool)GetValue(UseSourceJournalProperty); } set { SetValue(UseSourceJournalProperty, value); } }
		public static readonly DependencyProperty UseSourceJournalProperty = DependencyProperty.Register("UseSourceJournal", typeof(bool), typeof(WcfBindingMSMQIntegration));

		public TimeSpan ValidityDuration { get { return (TimeSpan)GetValue(ValidityDurationProperty); } set { SetValue(ValidityDurationProperty, value); } }
		public static readonly DependencyProperty ValidityDurationProperty = DependencyProperty.Register("ValidityDuration", typeof(TimeSpan), typeof(WcfBindingMSMQIntegration), new PropertyMetadata(new TimeSpan(1, 0, 0)));

		public bool Durable { get { return (bool)GetValue(DurableProperty); } set { SetValue(DurableProperty, value); } }
		public static readonly DependencyProperty DurableProperty = DependencyProperty.Register("Durable", typeof(bool), typeof(WcfBinding));

		public System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat SerializationFormat { get { return (System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat)GetValue(SerializationFormatProperty); } set { SetValue(SerializationFormatProperty, value); } }
		public static readonly DependencyProperty SerializationFormatProperty = DependencyProperty.Register("SerializationFormat", typeof(System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat), typeof(WcfBindingMSMQIntegration));

		public WcfBindingMSMQIntegration() : base() { }

		public WcfBindingMSMQIntegration(string Name, Namespace Parent) : base()
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.MsmqIntegration.MsmqIntegrationBinding", DataTypeMode.Class));
			Security = new WcfSecurityMSMQIntegration(this);

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

	#region - WcfBindingUDP Class -

	public class WcfBindingUDP : WcfBinding
	{
		public int DuplicateMessageHistoryLength { get { return (int)GetValue(DuplicateMessageHistoryLengthProperty); } set { SetValue(DuplicateMessageHistoryLengthProperty, value); } }
		public static readonly DependencyProperty DuplicateMessageHistoryLengthProperty = DependencyProperty.Register("DuplicateMessageHistoryLength ", typeof(int), typeof(WcfBindingUDP));

		public long MaxBufferPoolSize { get { return (long)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(long), typeof(WcfBindingUDP));

		public long MaxPendingMessagesTotalSize { get { return (long)GetValue(MaxPendingMessagesTotalSizeProperty); } set { SetValue(MaxPendingMessagesTotalSizeProperty, value); } }
		public static readonly DependencyProperty MaxPendingMessagesTotalSizeProperty = DependencyProperty.Register("MaxPendingMessagesTotalSize", typeof(long), typeof(WcfBindingUDP));

		public long MaxReceivedMessageSize { get { return (long)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(long), typeof(WcfBinding));

		public int MaxRetransmitCount { get { return (int)GetValue(MaxRetransmitCountProperty); } set { SetValue(MaxRetransmitCountProperty, value); } }
		public static readonly DependencyProperty MaxRetransmitCountProperty = DependencyProperty.Register("MaxRetransmitCount", typeof(int), typeof(WcfBindingUDP));

		public WcfBindingTextEncoding TextEncoding { get { return (WcfBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(WcfBindingTextEncoding), typeof(WcfBindingUDP));

		public int TimeToLive { get { return (int)GetValue(TimeToLiveProperty); } set { SetValue(TimeToLiveProperty, value); } }
		public static readonly DependencyProperty TimeToLiveProperty = DependencyProperty.Register("TimeToLive", typeof(int), typeof(WcfBindingUDP), new PropertyMetadata(64));

		public WcfBindingUDP() { }

		public WcfBindingUDP(string Name, Namespace Parent)
		{
			ID = Guid.NewGuid();
			this.Parent = Parent;
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			InheritedTypes.Add(new DataType("System.ServiceModel.UdpBinding", DataTypeMode.Class));

			CloseTimeout = new TimeSpan(0, 1, 0);
			OpenTimeout = new TimeSpan(0, 1, 0);
			ReceiveTimeout = new TimeSpan(0, 10, 0);
			SendTimeout = new TimeSpan(0, 1, 0);

			DuplicateMessageHistoryLength = 16;
			MaxBufferPoolSize = 524288;
			MaxPendingMessagesTotalSize = 65536;
			MaxReceivedMessageSize = 65536;
			MaxRetransmitCount = 10;
			TextEncoding = WcfBindingTextEncoding.UTF8;
			TimeToLive = 64;
		}
	}

	#endregion

}