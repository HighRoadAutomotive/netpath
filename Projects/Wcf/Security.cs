using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NETPath.Projects.Wcf
{

	public enum WcfSecurityAlgorithmSuite
	{
		Default,
		Basic128,
		Basic128Rsa15,
		Basic128Sha256,
		Basic128Sha256Rsa15,
		Basic192,
		Basic192Rsa15,
		Basic192Sha256,
		Basic192Sha256Rsa15,
		Basic256,
		Basic256Rsa15,
		Basic256Sha256,
		Basic256Sha256Rsa15,
		TripleDes,
		TripleDesRsa15,
		TripleDesSha256,
		TripleDesSha256Rsa15
	}

	public abstract class WcfSecurity : DependencyObject
	{
		public Guid ID { get; set; }
		public WcfBinding Owner { get; set; }

		public WcfSecurity() {}

		public WcfSecurity(WcfBinding Owner)
		{
			this.ID = Guid.NewGuid();
			this.Owner = Owner;
		}
	}

	#region - WcfSecurityBasicHTTP Class -

	public class WcfSecurityBasicHTTP : WcfSecurity
	{
		public System.ServiceModel.BasicHttpSecurityMode Mode { get { return (System.ServiceModel.BasicHttpSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.BasicHttpSecurityMode), typeof(WcfSecurityBasicHTTP), new PropertyMetadata(System.ServiceModel.BasicHttpSecurityMode.None));

		public WcfSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (WcfSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static readonly DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(WcfSecurityAlgorithmSuite), typeof(WcfSecurityBasicHTTP), new PropertyMetadata(WcfSecurityAlgorithmSuite.Basic256));

		public System.ServiceModel.BasicHttpMessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.BasicHttpMessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.BasicHttpMessageCredentialType), typeof(WcfSecurityBasicHTTP), new PropertyMetadata(System.ServiceModel.BasicHttpMessageCredentialType.UserName));

		public System.ServiceModel.HttpClientCredentialType TransportClientCredentialType { get { return (System.ServiceModel.HttpClientCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.HttpClientCredentialType), typeof(WcfSecurityBasicHTTP), new PropertyMetadata(System.ServiceModel.HttpClientCredentialType.None));

		public System.ServiceModel.HttpProxyCredentialType TransportProxyCredentialType { get { return (System.ServiceModel.HttpProxyCredentialType)GetValue(TransportProxyCredentialTypeProperty); } set { SetValue(TransportProxyCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportProxyCredentialTypeProperty = DependencyProperty.Register("TransportProxyCredentialType", typeof(System.ServiceModel.HttpProxyCredentialType), typeof(WcfSecurityBasicHTTP), new PropertyMetadata(System.ServiceModel.HttpProxyCredentialType.None));

		public string TransportRealm { get { return (string)GetValue(TransportRealmProperty); } set { SetValue(TransportRealmProperty, value); } }
		public static readonly DependencyProperty TransportRealmProperty = DependencyProperty.Register("TransportRealm", typeof(string), typeof(WcfSecurityBasicHTTP), new PropertyMetadata(""));

		public WcfSecurityBasicHTTP() { }

		public WcfSecurityBasicHTTP(WcfBinding Owner) : base(Owner) { }
	}

	#endregion

	#region - WcfSecurityBasicHTTPS Class -

	public class WcfSecurityBasicHTTPS : WcfSecurity
	{
		public System.ServiceModel.BasicHttpsSecurityMode Mode { get { return (System.ServiceModel.BasicHttpsSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.BasicHttpsSecurityMode), typeof(WcfSecurityBasicHTTPS), new PropertyMetadata(System.ServiceModel.BasicHttpsSecurityMode.Transport));

		public WcfSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (WcfSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static readonly DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(WcfSecurityAlgorithmSuite), typeof(WcfSecurityBasicHTTPS), new PropertyMetadata(WcfSecurityAlgorithmSuite.Basic256));

		public System.ServiceModel.BasicHttpMessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.BasicHttpMessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.BasicHttpMessageCredentialType), typeof(WcfSecurityBasicHTTPS), new PropertyMetadata(System.ServiceModel.BasicHttpMessageCredentialType.UserName));

		public System.ServiceModel.HttpClientCredentialType TransportClientCredentialType { get { return (System.ServiceModel.HttpClientCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.HttpClientCredentialType), typeof(WcfSecurityBasicHTTPS), new PropertyMetadata(System.ServiceModel.HttpClientCredentialType.None));

		public System.ServiceModel.HttpProxyCredentialType TransportProxyCredentialType { get { return (System.ServiceModel.HttpProxyCredentialType)GetValue(TransportProxyCredentialTypeProperty); } set { SetValue(TransportProxyCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportProxyCredentialTypeProperty = DependencyProperty.Register("TransportProxyCredentialType", typeof(System.ServiceModel.HttpProxyCredentialType), typeof(WcfSecurityBasicHTTPS), new PropertyMetadata(System.ServiceModel.HttpProxyCredentialType.None));

		public string TransportRealm { get { return (string)GetValue(TransportRealmProperty); } set { SetValue(TransportRealmProperty, value); } }
		public static readonly DependencyProperty TransportRealmProperty = DependencyProperty.Register("TransportRealm", typeof(string), typeof(WcfSecurityBasicHTTPS), new PropertyMetadata(""));

		public WcfSecurityBasicHTTPS() { }

		public WcfSecurityBasicHTTPS(WcfBinding Owner) : base(Owner) { }
	}

	#endregion

	#region - WcfSecurityWSHTTP Class -

	public class WcfSecurityWSHTTP : WcfSecurity
	{
		public System.ServiceModel.SecurityMode Mode { get { return (System.ServiceModel.SecurityMode )GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.SecurityMode), typeof(WcfSecurityWSHTTP), new PropertyMetadata(System.ServiceModel.SecurityMode.Message));

		public WcfSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (WcfSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static readonly DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(WcfSecurityAlgorithmSuite), typeof(WcfSecurityWSHTTP), new PropertyMetadata(WcfSecurityAlgorithmSuite.Basic256));

		public System.ServiceModel.MessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.MessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.MessageCredentialType), typeof(WcfSecurityWSHTTP), new PropertyMetadata(System.ServiceModel.MessageCredentialType.Windows));

		public bool MessageEstablishSecurityContext { get { return (bool)GetValue(MessageEstablishSecurityContextProperty); } set { SetValue(MessageEstablishSecurityContextProperty, value); } }
		public static readonly DependencyProperty MessageEstablishSecurityContextProperty = DependencyProperty.Register("MessageEstablishSecurityContext", typeof(bool), typeof(WcfSecurityWSHTTP), new PropertyMetadata(true));

		public bool MessageNegotiateServiceCredential { get { return (bool)GetValue(MessageNegotiateServiceCredentialProperty); } set { SetValue(MessageNegotiateServiceCredentialProperty, value); } }
		public static readonly DependencyProperty MessageNegotiateServiceCredentialProperty = DependencyProperty.Register("MessageNegotiateServiceCredential", typeof(bool), typeof(WcfSecurityWSHTTP), new PropertyMetadata(true));

		public System.ServiceModel.HttpClientCredentialType TransportClientCredentialType { get { return (System.ServiceModel.HttpClientCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.HttpClientCredentialType), typeof(WcfSecurityWSHTTP), new PropertyMetadata(System.ServiceModel.HttpClientCredentialType.None));

		public System.ServiceModel.HttpProxyCredentialType TransportProxyCredentialType { get { return (System.ServiceModel.HttpProxyCredentialType)GetValue(TransportProxyCredentialTypeProperty); } set { SetValue(TransportProxyCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportProxyCredentialTypeProperty = DependencyProperty.Register("TransportProxyCredentialType", typeof(System.ServiceModel.HttpProxyCredentialType), typeof(WcfSecurityWSHTTP), new PropertyMetadata(System.ServiceModel.HttpProxyCredentialType.None));

		public string TransportRealm { get { return (string)GetValue(TransportRealmProperty); } set { SetValue(TransportRealmProperty, value); } }
		public static readonly DependencyProperty TransportRealmProperty = DependencyProperty.Register("TransportRealm", typeof(string), typeof(WcfSecurityWSHTTP), new PropertyMetadata(""));

		public WcfSecurityWSHTTP() { }

		public WcfSecurityWSHTTP(WcfBinding Owner) : base(Owner) { }
	}

	#endregion

	#region - WcfSecurityWSDualHTTP Class -

	public class WcfSecurityWSDualHTTP : WcfSecurity
	{
		public System.ServiceModel.WSDualHttpSecurityMode Mode { get { return (System.ServiceModel.WSDualHttpSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.WSDualHttpSecurityMode), typeof(WcfSecurityWSDualHTTP), new PropertyMetadata(System.ServiceModel.WSDualHttpSecurityMode.Message));
		
		public WcfSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (WcfSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static readonly DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(WcfSecurityAlgorithmSuite), typeof(WcfSecurityWSHTTP), new PropertyMetadata(WcfSecurityAlgorithmSuite.Basic256));

		public System.ServiceModel.MessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.MessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.MessageCredentialType), typeof(WcfSecurityWSHTTP), new PropertyMetadata(System.ServiceModel.MessageCredentialType.Windows));

		public bool MessageNegotiateServiceCredential { get { return (bool)GetValue(MessageNegotiateServiceCredentialProperty); } set { SetValue(MessageNegotiateServiceCredentialProperty, value); } }
		public static readonly DependencyProperty MessageNegotiateServiceCredentialProperty = DependencyProperty.Register("MessageNegotiateServiceCredential", typeof(bool), typeof(WcfSecurityWSHTTP), new PropertyMetadata(true));
		
		public WcfSecurityWSDualHTTP() {}

		public WcfSecurityWSDualHTTP(WcfBindingWSDualHTTP Owner) : base(Owner) { }
	}

	#endregion

	#region - WcfSecurityWSFederationHTTP Class -

	public class WcfSecurityWSFederationHTTP : WcfSecurity
	{
		public System.ServiceModel.WSFederationHttpSecurityMode Mode { get { return (System.ServiceModel.WSFederationHttpSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.WSFederationHttpSecurityMode), typeof(WcfSecurityWSFederationHTTP), new PropertyMetadata(System.ServiceModel.WSFederationHttpSecurityMode.Message));

		public WcfSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (WcfSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static readonly DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(WcfSecurityAlgorithmSuite), typeof(WcfSecurityWSFederationHTTP), new PropertyMetadata(WcfSecurityAlgorithmSuite.Basic256));

		public bool MessageEstablishSecurityContext { get { return (bool)GetValue(MessageEstablishSecurityContextProperty); } set { SetValue(MessageEstablishSecurityContextProperty, value); } }
		public static readonly DependencyProperty MessageEstablishSecurityContextProperty = DependencyProperty.Register("MessageEstablishSecurityContext", typeof(bool), typeof(WcfSecurityWSFederationHTTP), new PropertyMetadata(false));

		public System.IdentityModel.Tokens.SecurityKeyType MessageIssuedKeyType { get { return (System.IdentityModel.Tokens.SecurityKeyType)GetValue(MessageIssuedKeyTypeProperty); } set { SetValue(MessageIssuedKeyTypeProperty, value); } }
		public static readonly DependencyProperty MessageIssuedKeyTypeProperty = DependencyProperty.Register("MessageIssuedKeyType", typeof(System.IdentityModel.Tokens.SecurityKeyType), typeof(WcfSecurityWSFederationHTTP), new PropertyMetadata(System.IdentityModel.Tokens.SecurityKeyType.SymmetricKey));
		
		public string MessageIssuedTokenType { get { return (string)GetValue(MessageIssuedTokenTypeProperty); } set { SetValue(MessageIssuedTokenTypeProperty, value); } }
		public static readonly DependencyProperty MessageIssuedTokenTypeProperty = DependencyProperty.Register("MessageIssuedTokenType", typeof(string), typeof(WcfSecurityWSFederationHTTP), new PropertyMetadata(""));
		
		public string MessageIssuerAddress { get { return (string)GetValue(MessageIssuerAddressProperty); } set { SetValue(MessageIssuerAddressProperty, value); } }
		public static readonly DependencyProperty MessageIssuerAddressProperty = DependencyProperty.Register("MessageIssuerAddress", typeof(string), typeof(WcfSecurityWSFederationHTTP), new PropertyMetadata(""));
		
		public string MessageIssuerMetadataAddress { get { return (string)GetValue(MessageIssuerMetadataAddressProperty); } set { SetValue(MessageIssuerMetadataAddressProperty, value); } }
		public static readonly DependencyProperty MessageIssuerMetadataAddressProperty = DependencyProperty.Register("MessageIssuerMetadataAddress", typeof(string), typeof(WcfSecurityWSFederationHTTP), new PropertyMetadata(""));
		
		public bool MessageNegotiateServiceCredential { get { return (bool)GetValue(MessageNegotiateServiceCredentialProperty); } set { SetValue(MessageNegotiateServiceCredentialProperty, value); } }
		public static readonly DependencyProperty MessageNegotiateServiceCredentialProperty = DependencyProperty.Register("MessageNegotiateServiceCredential", typeof(bool), typeof(WcfSecurityWSFederationHTTP), new PropertyMetadata(true));

		public WcfBinding MessageIssuerBinding { get { return (WcfBinding)GetValue(MessageIssuerBindingProperty); } set { SetValue(MessageIssuerBindingProperty, value); } }
		public static readonly DependencyProperty MessageIssuerBindingProperty = DependencyProperty.Register("MessageIssuerBinding", typeof(WcfBinding), typeof(WcfSecurityWSFederationHTTP));

		public WcfSecurityWSFederationHTTP() { }

		public WcfSecurityWSFederationHTTP(WcfBinding Owner) : base(Owner) { }
	}

	#endregion

	#region - WcfSecurityTCP Class -

	public class WcfSecurityTCP : WcfSecurity
	{
		public System.ServiceModel.SecurityMode Mode { get { return (System.ServiceModel.SecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.SecurityMode), typeof(WcfSecurityTCP), new PropertyMetadata(System.ServiceModel.SecurityMode.Transport));

		public WcfSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (WcfSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static readonly DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(WcfSecurityAlgorithmSuite), typeof(WcfSecurityTCP), new PropertyMetadata(WcfSecurityAlgorithmSuite.Basic256));

		public System.ServiceModel.MessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.MessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.MessageCredentialType), typeof(WcfSecurityTCP), new PropertyMetadata(System.ServiceModel.MessageCredentialType.Windows));

		public System.ServiceModel.TcpClientCredentialType TransportClientCredentialType { get { return (System.ServiceModel.TcpClientCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.TcpClientCredentialType), typeof(WcfSecurityTCP), new PropertyMetadata(System.ServiceModel.TcpClientCredentialType.None));

		public System.Net.Security.ProtectionLevel TransportProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(TransportProtectionLevelProperty); } set { SetValue(TransportProtectionLevelProperty, value); } }
		public static readonly DependencyProperty TransportProtectionLevelProperty = DependencyProperty.Register("TransportProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(WcfSecurityTCP), new PropertyMetadata(System.Net.Security.ProtectionLevel.None));
		
		public WcfSecurityTCP() {}

		public WcfSecurityTCP(WcfBindingTCP Owner) : base(Owner) {}
	}

	#endregion

	#region - WcfSecurityNamedPipe Class -

	public class WcfSecurityNamedPipe : WcfSecurity
	{
		public System.ServiceModel.NetNamedPipeSecurityMode Mode { get { return (System.ServiceModel.NetNamedPipeSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.NetNamedPipeSecurityMode), typeof(WcfSecurityNamedPipe), new PropertyMetadata(System.ServiceModel.NetNamedPipeSecurityMode.Transport));

		public System.Net.Security.ProtectionLevel TransportProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(TransportProtectionLevelProperty); } set { SetValue(TransportProtectionLevelProperty, value); } }
		public static readonly DependencyProperty TransportProtectionLevelProperty = DependencyProperty.Register("TransportProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(WcfSecurityNamedPipe), new PropertyMetadata(System.Net.Security.ProtectionLevel.EncryptAndSign));

		public WcfSecurityNamedPipe() {}

		public WcfSecurityNamedPipe(WcfBindingNamedPipe Owner) : base(Owner) { }
	}

	#endregion

	#region - WcfSecurityMSMQ Class -

	public class WcfSecurityMSMQ : WcfSecurity
	{
		public System.ServiceModel.NetMsmqSecurityMode Mode { get { return (System.ServiceModel.NetMsmqSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.NetMsmqSecurityMode), typeof(WcfSecurityMSMQ), new PropertyMetadata(System.ServiceModel.NetMsmqSecurityMode.None));

		public WcfSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (WcfSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static readonly DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(WcfSecurityAlgorithmSuite), typeof(WcfSecurityMSMQ), new PropertyMetadata(WcfSecurityAlgorithmSuite.Basic256));

		public System.ServiceModel.MessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.MessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.MessageCredentialType), typeof(WcfSecurityMSMQ), new PropertyMetadata(System.ServiceModel.MessageCredentialType.None));

		public System.ServiceModel.MsmqAuthenticationMode TransportAuthenticationMode { get { return (System.ServiceModel.MsmqAuthenticationMode)GetValue(TransportAuthenticationModeProperty); } set { SetValue(TransportAuthenticationModeProperty, value); } }
		public static readonly DependencyProperty TransportAuthenticationModeProperty = DependencyProperty.Register("TransportAuthenticationMode", typeof(System.ServiceModel.MsmqAuthenticationMode), typeof(WcfSecurityMSMQ), new PropertyMetadata(System.ServiceModel.MsmqAuthenticationMode.WindowsDomain));
		
		public System.ServiceModel.MsmqEncryptionAlgorithm TransportEncryptionAlgorithm { get { return (System.ServiceModel.MsmqEncryptionAlgorithm)GetValue(TransportEncryptionAlgorithmProperty); } set { SetValue(TransportEncryptionAlgorithmProperty, value); } }
		public static readonly DependencyProperty TransportEncryptionAlgorithmProperty = DependencyProperty.Register("TransportEncryptionAlgorithm", typeof(System.ServiceModel.MsmqEncryptionAlgorithm), typeof(WcfSecurityMSMQ), new PropertyMetadata(System.ServiceModel.MsmqEncryptionAlgorithm.RC4Stream));
		
		public System.Net.Security.ProtectionLevel TransportProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(TransportProtectionLevelProperty); } set { SetValue(TransportProtectionLevelProperty, value); } }
		public static readonly DependencyProperty TransportProtectionLevelProperty = DependencyProperty.Register("TransportProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(WcfSecurityMSMQ), new PropertyMetadata(System.Net.Security.ProtectionLevel.Sign));
		
		public System.ServiceModel.MsmqSecureHashAlgorithm TransportSecureHashAlgorithm { get { return (System.ServiceModel.MsmqSecureHashAlgorithm)GetValue(TransportSecureHashAlgorithmProperty); } set { SetValue(TransportSecureHashAlgorithmProperty, value); } }
		public static readonly DependencyProperty TransportSecureHashAlgorithmProperty = DependencyProperty.Register("TransportSecureHashAlgorithm", typeof(System.ServiceModel.MsmqSecureHashAlgorithm), typeof(WcfSecurityMSMQ), new PropertyMetadata(System.ServiceModel.MsmqSecureHashAlgorithm.Sha1));

		public WcfSecurityMSMQ() {}

		public WcfSecurityMSMQ(WcfBindingMSMQ Owner) : base(Owner) {}
	}

	#endregion

	#region - WcfSecurityPeerTCP Class -

	public class WcfSecurityPeerTCP : WcfSecurity
	{
		public System.ServiceModel.SecurityMode Mode { get { return (System.ServiceModel.SecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.SecurityMode), typeof(WcfSecurityPeerTCP), new PropertyMetadata(System.ServiceModel.SecurityMode.None));

		public System.ServiceModel.PeerTransportCredentialType TransportClientCredentialType { get { return (System.ServiceModel.PeerTransportCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.PeerTransportCredentialType), typeof(WcfSecurityPeerTCP), new PropertyMetadata(System.ServiceModel.PeerTransportCredentialType.Password));

		public WcfSecurityPeerTCP() {}

		public WcfSecurityPeerTCP(WcfBindingPeerTCP Owner) : base(Owner) {}
	}

	#endregion

	#region - WcfSecurityWebHTTP Class -

	public class WcfSecurityWebHTTP : WcfSecurity
	{
		public System.ServiceModel.WebHttpSecurityMode Mode { get { return (System.ServiceModel.WebHttpSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.WebHttpSecurityMode), typeof(WcfSecurityWebHTTP), new PropertyMetadata(System.ServiceModel.WebHttpSecurityMode.None));

		public System.ServiceModel.HttpClientCredentialType TransportClientCredentialType { get { return (System.ServiceModel.HttpClientCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.HttpClientCredentialType), typeof(WcfSecurityWebHTTP), new PropertyMetadata(System.ServiceModel.HttpClientCredentialType.None));

		public System.ServiceModel.HttpProxyCredentialType TransportProxyCredentialType { get { return (System.ServiceModel.HttpProxyCredentialType)GetValue(TransportProxyCredentialTypeProperty); } set { SetValue(TransportProxyCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportProxyCredentialTypeProperty = DependencyProperty.Register("TransportProxyCredentialType", typeof(System.ServiceModel.HttpProxyCredentialType), typeof(WcfSecurityWebHTTP), new PropertyMetadata(System.ServiceModel.HttpProxyCredentialType.None));

		public string TransportRealm { get { return (string)GetValue(TransportRealmProperty); } set { SetValue(TransportRealmProperty, value); } }
		public static readonly DependencyProperty TransportRealmProperty = DependencyProperty.Register("TransportRealm", typeof(string), typeof(WcfSecurityWebHTTP), new PropertyMetadata(""));

		public WcfSecurityWebHTTP() { }

		public WcfSecurityWebHTTP(WcfBindingWebHTTP Owner) : base(Owner) {}
	}

	#endregion

	#region - WcfSecurityMSMQIntegration Class -

	public class WcfSecurityMSMQIntegration : WcfSecurity
	{
		public System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode Mode { get { return (System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode), typeof(WcfSecurityMSMQIntegration), new PropertyMetadata(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode.Transport));

		public System.ServiceModel.MsmqAuthenticationMode TransportAuthenticationMode { get { return (System.ServiceModel.MsmqAuthenticationMode)GetValue(TransportAuthenticationModeProperty); } set { SetValue(TransportAuthenticationModeProperty, value); } }
		public static readonly DependencyProperty TransportAuthenticationModeProperty = DependencyProperty.Register("TransportAuthenticationMode", typeof(System.ServiceModel.MsmqAuthenticationMode), typeof(WcfSecurityMSMQIntegration), new PropertyMetadata(System.ServiceModel.MsmqAuthenticationMode.WindowsDomain));

		public System.ServiceModel.MsmqEncryptionAlgorithm TransportEncryptionAlgorithm { get { return (System.ServiceModel.MsmqEncryptionAlgorithm)GetValue(TransportEncryptionAlgorithmProperty); } set { SetValue(TransportEncryptionAlgorithmProperty, value); } }
		public static readonly DependencyProperty TransportEncryptionAlgorithmProperty = DependencyProperty.Register("TransportEncryptionAlgorithm", typeof(System.ServiceModel.MsmqEncryptionAlgorithm), typeof(WcfSecurityMSMQIntegration), new PropertyMetadata(System.ServiceModel.MsmqEncryptionAlgorithm.RC4Stream));

		public System.Net.Security.ProtectionLevel TransportProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(TransportProtectionLevelProperty); } set { SetValue(TransportProtectionLevelProperty, value); } }
		public static readonly DependencyProperty TransportProtectionLevelProperty = DependencyProperty.Register("TransportProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(WcfSecurityMSMQIntegration), new PropertyMetadata(System.Net.Security.ProtectionLevel.Sign));

		public System.ServiceModel.MsmqSecureHashAlgorithm TransportSecureHashAlgorithm { get { return (System.ServiceModel.MsmqSecureHashAlgorithm)GetValue(TransportSecureHashAlgorithmProperty); } set { SetValue(TransportSecureHashAlgorithmProperty, value); } }
		public static readonly DependencyProperty TransportSecureHashAlgorithmProperty = DependencyProperty.Register("TransportSecureHashAlgorithm", typeof(System.ServiceModel.MsmqSecureHashAlgorithm), typeof(WcfSecurityMSMQIntegration), new PropertyMetadata(System.ServiceModel.MsmqSecureHashAlgorithm.Sha1));

		public WcfSecurityMSMQIntegration() {}

		public WcfSecurityMSMQIntegration(WcfBindingMSMQIntegration Owner) : base(Owner) { }
	}
	#endregion
}