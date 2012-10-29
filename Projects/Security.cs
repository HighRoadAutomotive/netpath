using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace WCFArchitect.Projects
{

	public enum BindingSecurityAlgorithmSuite
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

	public abstract class BindingSecurity : DependencyObject
	{
		public Guid ID { get; set; }
		public ServiceBinding Owner { get; set; }

		public BindingSecurity() {}

		public BindingSecurity(ServiceBinding Owner)
		{
			this.ID = Guid.NewGuid();
			this.Owner = Owner;
		}
	}

	#region - BindingSecurityBasicHTTP Class -

	public class BindingSecurityBasicHTTP : BindingSecurity
	{
		public System.ServiceModel.BasicHttpSecurityMode Mode { get { return (System.ServiceModel.BasicHttpSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.BasicHttpSecurityMode), typeof(BindingSecurityBasicHTTP));

		public BindingSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (BindingSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static readonly DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(BindingSecurityAlgorithmSuite), typeof(BindingSecurityBasicHTTP));

		public System.ServiceModel.BasicHttpMessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.BasicHttpMessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.BasicHttpMessageCredentialType), typeof(BindingSecurityBasicHTTP));

		public System.ServiceModel.HttpClientCredentialType TransportClientCredentialType { get { return (System.ServiceModel.HttpClientCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.HttpClientCredentialType), typeof(BindingSecurityBasicHTTP));

		public System.ServiceModel.HttpProxyCredentialType TransportProxyCredentialType { get { return (System.ServiceModel.HttpProxyCredentialType)GetValue(TransportProxyCredentialTypeProperty); } set { SetValue(TransportProxyCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportProxyCredentialTypeProperty = DependencyProperty.Register("TransportProxyCredentialType", typeof(System.ServiceModel.HttpProxyCredentialType), typeof(BindingSecurityBasicHTTP));

		public string TransportRealm { get { return (string)GetValue(TransportRealmProperty); } set { SetValue(TransportRealmProperty, value); } }
		public static readonly DependencyProperty TransportRealmProperty = DependencyProperty.Register("TransportRealm", typeof(string), typeof(BindingSecurityBasicHTTP));

		public BindingSecurityBasicHTTP() { }

		public BindingSecurityBasicHTTP(ServiceBinding Owner) : base(Owner) { }
	}

	#endregion

	#region - BindingSecurityBasicHTTPS Class -

	public class BindingSecurityBasicHTTPS : BindingSecurity
	{
		public System.ServiceModel.BasicHttpsSecurityMode Mode { get { return (System.ServiceModel.BasicHttpsSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.BasicHttpsSecurityMode), typeof(BindingSecurityBasicHTTP));

		public BindingSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (BindingSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static readonly DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(BindingSecurityAlgorithmSuite), typeof(BindingSecurityBasicHTTP));

		public System.ServiceModel.BasicHttpMessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.BasicHttpMessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.BasicHttpMessageCredentialType), typeof(BindingSecurityBasicHTTP));

		public System.ServiceModel.HttpClientCredentialType TransportClientCredentialType { get { return (System.ServiceModel.HttpClientCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.HttpClientCredentialType), typeof(BindingSecurityBasicHTTP));

		public System.ServiceModel.HttpProxyCredentialType TransportProxyCredentialType { get { return (System.ServiceModel.HttpProxyCredentialType)GetValue(TransportProxyCredentialTypeProperty); } set { SetValue(TransportProxyCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportProxyCredentialTypeProperty = DependencyProperty.Register("TransportProxyCredentialType", typeof(System.ServiceModel.HttpProxyCredentialType), typeof(BindingSecurityBasicHTTP));

		public string TransportRealm { get { return (string)GetValue(TransportRealmProperty); } set { SetValue(TransportRealmProperty, value); } }
		public static readonly DependencyProperty TransportRealmProperty = DependencyProperty.Register("TransportRealm", typeof(string), typeof(BindingSecurityBasicHTTP));

		public BindingSecurityBasicHTTPS() { }

		public BindingSecurityBasicHTTPS(ServiceBinding Owner) : base(Owner) { }
	}

	#endregion

	#region - BindingSecurityWSHTTP Class -

	public class BindingSecurityWSHTTP : BindingSecurity
	{
		public System.ServiceModel.SecurityMode Mode { get { return (System.ServiceModel.SecurityMode )GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.SecurityMode), typeof(BindingSecurityWSHTTP));

		public BindingSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (BindingSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static readonly DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(BindingSecurityAlgorithmSuite), typeof(BindingSecurityWSHTTP));

		public System.ServiceModel.MessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.MessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.MessageCredentialType), typeof(BindingSecurityWSHTTP));

		public bool MessageEstablishSecurityContext { get { return (bool)GetValue(MessageEstablishSecurityContextProperty); } set { SetValue(MessageEstablishSecurityContextProperty, value); } }
		public static readonly DependencyProperty MessageEstablishSecurityContextProperty = DependencyProperty.Register("MessageEstablishSecurityContext", typeof(bool), typeof(BindingSecurityWSHTTP));

		public bool MessageNegotiateServiceCredential { get { return (bool)GetValue(MessageNegotiateServiceCredentialProperty); } set { SetValue(MessageNegotiateServiceCredentialProperty, value); } }
		public static readonly DependencyProperty MessageNegotiateServiceCredentialProperty = DependencyProperty.Register("MessageNegotiateServiceCredential", typeof(bool), typeof(BindingSecurityWSHTTP));

		public System.ServiceModel.HttpClientCredentialType TransportClientCredentialType { get { return (System.ServiceModel.HttpClientCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.HttpClientCredentialType), typeof(BindingSecurityWSHTTP));

		public System.ServiceModel.HttpProxyCredentialType TransportProxyCredentialType { get { return (System.ServiceModel.HttpProxyCredentialType)GetValue(TransportProxyCredentialTypeProperty); } set { SetValue(TransportProxyCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportProxyCredentialTypeProperty = DependencyProperty.Register("TransportProxyCredentialType", typeof(System.ServiceModel.HttpProxyCredentialType), typeof(BindingSecurityWSHTTP));

		public string TransportRealm { get { return (string)GetValue(TransportRealmProperty); } set { SetValue(TransportRealmProperty, value); } }
		public static readonly DependencyProperty TransportRealmProperty = DependencyProperty.Register("TransportRealm", typeof(string), typeof(BindingSecurityWSHTTP));

		public BindingSecurityWSHTTP() { }

		public BindingSecurityWSHTTP(ServiceBinding Owner) : base(Owner) { }
	}

	#endregion

	#region - BindingSecurityWSDualHTTP Class -

	public class BindingSecurityWSDualHTTP : BindingSecurity
	{
		public System.ServiceModel.WSDualHttpSecurityMode Mode { get { return (System.ServiceModel.WSDualHttpSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.WSDualHttpSecurityMode), typeof(BindingSecurityWSDualHTTP));
		
		public BindingSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (BindingSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static readonly DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(BindingSecurityAlgorithmSuite), typeof(BindingSecurityWSHTTP));

		public System.ServiceModel.MessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.MessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.MessageCredentialType), typeof(BindingSecurityWSHTTP));

		public bool MessageNegotiateServiceCredential { get { return (bool)GetValue(MessageNegotiateServiceCredentialProperty); } set { SetValue(MessageNegotiateServiceCredentialProperty, value); } }
		public static readonly DependencyProperty MessageNegotiateServiceCredentialProperty = DependencyProperty.Register("MessageNegotiateServiceCredential", typeof(bool), typeof(BindingSecurityWSHTTP));
		
		public BindingSecurityWSDualHTTP() {}

		public BindingSecurityWSDualHTTP(ServiceBindingWSDualHTTP Owner) : base(Owner) { }
	}

	#endregion

	#region - BindingSecurityWSFederationHTTP Class -

	public class BindingSecurityWSFederationHTTP : BindingSecurity
	{
		public System.ServiceModel.WSFederationHttpSecurityMode Mode { get { return (System.ServiceModel.WSFederationHttpSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.WSFederationHttpSecurityMode), typeof(BindingSecurityWSFederationHTTP));

		public BindingSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (BindingSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static readonly DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(BindingSecurityAlgorithmSuite), typeof(BindingSecurityWSFederationHTTP));

		public System.ServiceModel.MessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.MessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.MessageCredentialType), typeof(BindingSecurityWSFederationHTTP));

		public bool MessageEstablishSecurityContext { get { return (bool)GetValue(MessageEstablishSecurityContextProperty); } set { SetValue(MessageEstablishSecurityContextProperty, value); } }
		public static readonly DependencyProperty MessageEstablishSecurityContextProperty = DependencyProperty.Register("MessageEstablishSecurityContext", typeof(bool), typeof(BindingSecurityWSFederationHTTP));

		public System.IdentityModel.Tokens.SecurityKeyType MessageIssuedKeyType { get { return (System.IdentityModel.Tokens.SecurityKeyType)GetValue(MessageIssuedKeyTypeProperty); } set { SetValue(MessageIssuedKeyTypeProperty, value); } }
		public static readonly DependencyProperty MessageIssuedKeyTypeProperty = DependencyProperty.Register("MessageIssuedKeyType", typeof(System.IdentityModel.Tokens.SecurityKeyType), typeof(BindingSecurityWSFederationHTTP));
		
		public string MessageIssuedTokenType { get { return (string)GetValue(MessageIssuedTokenTypeProperty); } set { SetValue(MessageIssuedTokenTypeProperty, value); } }
		public static readonly DependencyProperty MessageIssuedTokenTypeProperty = DependencyProperty.Register("MessageIssuedTokenType", typeof(string), typeof(BindingSecurityWSFederationHTTP));
		
		public string MessageIssuerAddress { get { return (string)GetValue(MessageIssuerAddressProperty); } set { SetValue(MessageIssuerAddressProperty, value); } }
		public static readonly DependencyProperty MessageIssuerAddressProperty = DependencyProperty.Register("MessageIssuerAddress", typeof(string), typeof(BindingSecurityWSFederationHTTP));
		
		public string MessageIssuerMetadataAddress { get { return (string)GetValue(MessageIssuerMetadataAddressProperty); } set { SetValue(MessageIssuerMetadataAddressProperty, value); } }
		public static readonly DependencyProperty MessageIssuerMetadataAddressProperty = DependencyProperty.Register("MessageIssuerMetadataAddress", typeof(string), typeof(BindingSecurityWSFederationHTTP));
		
		public bool MessageNegotiateServiceCredential { get { return (bool)GetValue(MessageNegotiateServiceCredentialProperty); } set { SetValue(MessageNegotiateServiceCredentialProperty, value); } }
		public static readonly DependencyProperty MessageNegotiateServiceCredentialProperty = DependencyProperty.Register("MessageNegotiateServiceCredential", typeof(bool), typeof(BindingSecurityWSFederationHTTP));

		public BindingSecurityWSFederationHTTP() {}

		public BindingSecurityWSFederationHTTP(ServiceBinding Owner) : base(Owner) { }
	}

	#endregion

	#region - BindingSecurityTCP Class -

	public class BindingSecurityTCP : BindingSecurity
	{
		public System.ServiceModel.SecurityMode Mode { get { return (System.ServiceModel.SecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.SecurityMode), typeof(BindingSecurityTCP));

		public BindingSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (BindingSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static readonly DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(BindingSecurityAlgorithmSuite), typeof(BindingSecurityTCP));

		public System.ServiceModel.MessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.MessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.MessageCredentialType), typeof(BindingSecurityTCP));

		public System.ServiceModel.TcpClientCredentialType TransportClientCredentialType { get { return (System.ServiceModel.TcpClientCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.TcpClientCredentialType), typeof(BindingSecurityTCP));

		public System.Net.Security.ProtectionLevel TransportProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(TransportProtectionLevelProperty); } set { SetValue(TransportProtectionLevelProperty, value); } }
		public static readonly DependencyProperty TransportProtectionLevelProperty = DependencyProperty.Register("TransportProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(BindingSecurityTCP));
		
		public BindingSecurityTCP() {}

		public BindingSecurityTCP(ServiceBindingTCP Owner) : base(Owner) {}
	}

	#endregion

	#region - BindingSecurityNamedPipe Class -

	public class BindingSecurityNamedPipe : BindingSecurity
	{
		public System.ServiceModel.NetNamedPipeSecurityMode Mode { get { return (System.ServiceModel.NetNamedPipeSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.NetNamedPipeSecurityMode), typeof(BindingSecurityNamedPipe));

		public System.Net.Security.ProtectionLevel TransportProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(TransportProtectionLevelProperty); } set { SetValue(TransportProtectionLevelProperty, value); } }
		public static readonly DependencyProperty TransportProtectionLevelProperty = DependencyProperty.Register("TransportProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(BindingSecurityNamedPipe));

		public BindingSecurityNamedPipe() {}

		public BindingSecurityNamedPipe(ServiceBindingNamedPipe Owner) : base(Owner) { }
	}

	#endregion

	#region - BindingSecurityMSMQ Class -

	public class BindingSecurityMSMQ : BindingSecurity
	{
		public System.ServiceModel.NetMsmqSecurityMode Mode { get { return (System.ServiceModel.NetMsmqSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.NetMsmqSecurityMode), typeof(BindingSecurityMSMQ));

		public BindingSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (BindingSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static readonly DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(BindingSecurityAlgorithmSuite), typeof(BindingSecurityMSMQ));

		public System.ServiceModel.MessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.MessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.MessageCredentialType), typeof(BindingSecurityMSMQ));

		public System.ServiceModel.MsmqAuthenticationMode TransportAuthenticationMode { get { return (System.ServiceModel.MsmqAuthenticationMode)GetValue(TransportAuthenticationModeProperty); } set { SetValue(TransportAuthenticationModeProperty, value); } }
		public static readonly DependencyProperty TransportAuthenticationModeProperty = DependencyProperty.Register("TransportAuthenticationMode", typeof(System.ServiceModel.MsmqAuthenticationMode), typeof(BindingSecurityMSMQ));
		
		public System.ServiceModel.MsmqEncryptionAlgorithm TransportEncryptionAlgorithm { get { return (System.ServiceModel.MsmqEncryptionAlgorithm)GetValue(TransportEncryptionAlgorithmProperty); } set { SetValue(TransportEncryptionAlgorithmProperty, value); } }
		public static readonly DependencyProperty TransportEncryptionAlgorithmProperty = DependencyProperty.Register("TransportEncryptionAlgorithm", typeof(System.ServiceModel.MsmqEncryptionAlgorithm), typeof(BindingSecurityMSMQ));
		
		public System.Net.Security.ProtectionLevel TransportProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(TransportProtectionLevelProperty); } set { SetValue(TransportProtectionLevelProperty, value); } }
		public static readonly DependencyProperty TransportProtectionLevelProperty = DependencyProperty.Register("TransportProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(BindingSecurityMSMQ));
		
		public System.ServiceModel.MsmqSecureHashAlgorithm TransportSecureHashAlgorithm { get { return (System.ServiceModel.MsmqSecureHashAlgorithm)GetValue(TransportSecureHashAlgorithmProperty); } set { SetValue(TransportSecureHashAlgorithmProperty, value); } }
		public static readonly DependencyProperty TransportSecureHashAlgorithmProperty = DependencyProperty.Register("TransportSecureHashAlgorithm", typeof(System.ServiceModel.MsmqSecureHashAlgorithm), typeof(BindingSecurityMSMQ));

		public BindingSecurityMSMQ() {}

		public BindingSecurityMSMQ(ServiceBindingMSMQ Owner) : base(Owner) {}
	}

	#endregion

	#region - BindingSecurityPeerTCP Class -

	public class BindingSecurityPeerTCP : BindingSecurity
	{
		public System.ServiceModel.SecurityMode Mode { get { return (System.ServiceModel.SecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.SecurityMode), typeof(BindingSecurityPeerTCP));

		public System.ServiceModel.PeerTransportCredentialType TransportClientCredentialType { get { return (System.ServiceModel.PeerTransportCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.PeerTransportCredentialType), typeof(BindingSecurityPeerTCP));

		public BindingSecurityPeerTCP() {}

		public BindingSecurityPeerTCP(ServiceBindingPeerTCP Owner) : base(Owner) {}
	}

	#endregion

	#region - BindingSecurityWebHTTP Class -

	public class BindingSecurityWebHTTP : BindingSecurity
	{
		public System.ServiceModel.WebHttpSecurityMode Mode { get { return (System.ServiceModel.WebHttpSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.WebHttpSecurityMode), typeof(BindingSecurityWebHTTP));

		public System.ServiceModel.HttpClientCredentialType TransportClientCredentialType { get { return (System.ServiceModel.HttpClientCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.HttpClientCredentialType), typeof(BindingSecurityWebHTTP));

		public System.ServiceModel.HttpProxyCredentialType TransportProxyCredentialType { get { return (System.ServiceModel.HttpProxyCredentialType)GetValue(TransportProxyCredentialTypeProperty); } set { SetValue(TransportProxyCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportProxyCredentialTypeProperty = DependencyProperty.Register("TransportProxyCredentialType", typeof(System.ServiceModel.HttpProxyCredentialType), typeof(BindingSecurityWebHTTP));

		public string TransportRealm { get { return (string)GetValue(TransportRealmProperty); } set { SetValue(TransportRealmProperty, value); } }
		public static readonly DependencyProperty TransportRealmProperty = DependencyProperty.Register("TransportRealm", typeof(string), typeof(BindingSecurityWebHTTP));

		public BindingSecurityWebHTTP() { }

		public BindingSecurityWebHTTP(ServiceBindingWebHTTP Owner) : base(Owner) {}
	}

	#endregion

	#region - BindingSecurityMSMQIntegration Class -

	public class BindingSecurityMSMQIntegration : BindingSecurity
	{
		public System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode Mode { get { return (System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode), typeof(BindingSecurityMSMQIntegration));

		public System.ServiceModel.MsmqAuthenticationMode TransportAuthenticationMode { get { return (System.ServiceModel.MsmqAuthenticationMode)GetValue(TransportAuthenticationModeProperty); } set { SetValue(TransportAuthenticationModeProperty, value); } }
		public static readonly DependencyProperty TransportAuthenticationModeProperty = DependencyProperty.Register("TransportAuthenticationMode", typeof(System.ServiceModel.MsmqAuthenticationMode), typeof(BindingSecurityMSMQIntegration));

		public System.ServiceModel.MsmqEncryptionAlgorithm TransportEncryptionAlgorithm { get { return (System.ServiceModel.MsmqEncryptionAlgorithm)GetValue(TransportEncryptionAlgorithmProperty); } set { SetValue(TransportEncryptionAlgorithmProperty, value); } }
		public static readonly DependencyProperty TransportEncryptionAlgorithmProperty = DependencyProperty.Register("TransportEncryptionAlgorithm", typeof(System.ServiceModel.MsmqEncryptionAlgorithm), typeof(BindingSecurityMSMQIntegration));

		public System.Net.Security.ProtectionLevel TransportProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(TransportProtectionLevelProperty); } set { SetValue(TransportProtectionLevelProperty, value); } }
		public static readonly DependencyProperty TransportProtectionLevelProperty = DependencyProperty.Register("TransportProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(BindingSecurityMSMQIntegration));

		public System.ServiceModel.MsmqSecureHashAlgorithm TransportSecureHashAlgorithm { get { return (System.ServiceModel.MsmqSecureHashAlgorithm)GetValue(TransportSecureHashAlgorithmProperty); } set { SetValue(TransportSecureHashAlgorithmProperty, value); } }
		public static readonly DependencyProperty TransportSecureHashAlgorithmProperty = DependencyProperty.Register("TransportSecureHashAlgorithm", typeof(System.ServiceModel.MsmqSecureHashAlgorithm), typeof(BindingSecurityMSMQIntegration));

		public BindingSecurityMSMQIntegration() {}

		public BindingSecurityMSMQIntegration(ServiceBindingMSMQIntegration Owner) : base(Owner) { }
	}
	#endregion
}