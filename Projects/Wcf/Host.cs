using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;

namespace NETPath.Projects.Wcf
{
	public enum WcfHostEndpointIdentityType
	{
		Anonymous,
		DNS,
		RSA,
		RSAX509,
		SPN,
		UPN,
		X509
	}

	public class WcfHost : DataType
	{
		public string Namespace { get { return (string)GetValue(NamespaceProperty); } set { SetValue(NamespaceProperty, value); } }
		public static readonly DependencyProperty NamespaceProperty = DependencyProperty.Register("Namespace", typeof(string), typeof(WcfHost));

		public string ConfigurationName { get { return (string)GetValue(ConfigurationNameProperty); } set { SetValue(ConfigurationNameProperty, value); } }
		public static readonly DependencyProperty ConfigurationNameProperty = DependencyProperty.Register("ConfigurationName", typeof(string), typeof(WcfHost));

		public WcfHostCredentials Credentials { get { return (WcfHostCredentials)GetValue(CredentialsProperty); } set { SetValue(CredentialsProperty, value); } }
		public static readonly DependencyProperty CredentialsProperty = DependencyProperty.Register("Credentials", typeof(WcfHostCredentials), typeof(WcfHost));

		public ObservableCollection<string> BaseAddresses { get { return (ObservableCollection<string>)GetValue(BaseAddressProperty); } set { SetValue(BaseAddressProperty, value); } }
		public static readonly DependencyProperty BaseAddressProperty = DependencyProperty.Register("BaseAddress", typeof(ObservableCollection<string>), typeof(WcfHost));

		public TimeSpan CloseTimeout { get { return (TimeSpan)GetValue(CloseTimeoutProperty); } set { SetValue(CloseTimeoutProperty, value); } }
		public static readonly DependencyProperty CloseTimeoutProperty = DependencyProperty.Register("CloseTimeout", typeof (TimeSpan), typeof (WcfHost), new PropertyMetadata(new TimeSpan(0, 1, 0)));

		public TimeSpan OpenTimeout { get { return (TimeSpan)GetValue(OpenTimeoutProperty); } set { SetValue(OpenTimeoutProperty, value); } }
		public static readonly DependencyProperty OpenTimeoutProperty = DependencyProperty.Register("OpenTimeout", typeof(TimeSpan), typeof(WcfHost), new PropertyMetadata(new TimeSpan(0, 1, 0)));

		public int ManualFlowControlLimit { get { return (int)GetValue(ManualFlowControlLimitProperty); } set { SetValue(ManualFlowControlLimitProperty, value); } }
		public static readonly DependencyProperty ManualFlowControlLimitProperty = DependencyProperty.Register("ManualFlowControlLimit", typeof(int), typeof(WcfHost));

		public bool AuthorizationImpersonateCallerForAllOperations { get { return (bool)GetValue(AuthorizationImpersonateCallerForAllOperationsProperty); } set { SetValue(AuthorizationImpersonateCallerForAllOperationsProperty, value); } }
		public static readonly DependencyProperty AuthorizationImpersonateCallerForAllOperationsProperty = DependencyProperty.Register("AuthorizationImpersonateCallerForAllOperations", typeof(bool), typeof(WcfHost));

		public System.ServiceModel.Description.PrincipalPermissionMode AuthorizationPrincipalPermissionMode { get { return (System.ServiceModel.Description.PrincipalPermissionMode)GetValue(AuthorizationPrincipalPermissionModeProperty); } set { SetValue(AuthorizationPrincipalPermissionModeProperty, value); } }
		public static readonly DependencyProperty AuthorizationPrincipalPermissionModeProperty = DependencyProperty.Register("AuthorizationPrincipalPermissionMode", typeof(System.ServiceModel.Description.PrincipalPermissionMode), typeof(WcfHost));

		public WcfService Service { get { return (WcfService)GetValue(ServiceProperty); } set { SetValue(ServiceProperty, value); } }
		public static readonly DependencyProperty ServiceProperty = DependencyProperty.Register("Service", typeof(WcfService), typeof(WcfHost));

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(WcfHost));

		public ObservableCollection<WcfHostBehavior> Behaviors { get { return (ObservableCollection<WcfHostBehavior>)GetValue(BehaviorsProperty); } set { SetValue(BehaviorsProperty, value); } }
		public static readonly DependencyProperty BehaviorsProperty = DependencyProperty.Register("Behaviors", typeof(ObservableCollection<WcfHostBehavior>), typeof(WcfHost));

		public ObservableCollection<WcfHostEndpoint> Endpoints { get { return (ObservableCollection<WcfHostEndpoint>)GetValue(EndpointsProperty); } set { SetValue(EndpointsProperty, value); } }
		public static readonly DependencyProperty EndpointsProperty = DependencyProperty.Register("Endpoints", typeof(ObservableCollection<WcfHostEndpoint>), typeof(WcfHost));

		public WcfHost() : base(DataTypeMode.Class)
		{
			Endpoints = new ObservableCollection<WcfHostEndpoint>();
			Documentation = new Documentation { IsClass = true };
		}

		public WcfHost(string Name, Namespace Parent) : base(DataTypeMode.Class)
		{
			BaseAddresses = new ObservableCollection<string>();
			Behaviors = new ObservableCollection<WcfHostBehavior>();
			Endpoints = new ObservableCollection<WcfHostEndpoint>();
			ID = Guid.NewGuid();
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;
			var p = Parent.Owner as WcfProject;
			if (p != null) Namespace = p.Namespace.Uri;
			Credentials = new WcfHostCredentials(this);
			Documentation = new Documentation { IsClass = true };
		}
	}

	public class WcfHostEndpoint : DependencyObject
	{
		public Guid ID { get; set; }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(WcfHostEndpoint));

		public WcfBinding Binding { get { return (WcfBinding)GetValue(BindingProperty); } set { SetValue(BindingProperty, value); } }
		public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding", typeof(WcfBinding), typeof(WcfHostEndpoint));

		public string ServerAddress { get { return (string)GetValue(ServerAddressProperty); } set { SetValue(ServerAddressProperty, value); } }
		public static readonly DependencyProperty ServerAddressProperty = DependencyProperty.Register("ServerAddress", typeof(string), typeof(WcfHostEndpoint));

		public bool ServerAddressIsVariable { get { return (bool)GetValue(ServerAddressIsVariableProperty); } set { SetValue(ServerAddressIsVariableProperty, value); } }
		public static readonly DependencyProperty ServerAddressIsVariableProperty = DependencyProperty.Register("ServerAddressIsVariable", typeof(bool), typeof(WcfHostEndpoint), new PropertyMetadata(false));

		public int ServerPort { get { return (int)GetValue(ServerPortProperty); } set { SetValue(ServerPortProperty, value); } }
		public static readonly DependencyProperty ServerPortProperty = DependencyProperty.Register("ServerPort", typeof(int), typeof(WcfHostEndpoint));

		public bool ServerUseHTTPS { get { return (bool)GetValue(ServerUseHTTPSProperty); } set { SetValue(ServerUseHTTPSProperty, value); } }
		public static readonly DependencyProperty ServerUseHTTPSProperty = DependencyProperty.Register("ServerUseHTTPS", typeof(bool), typeof(WcfHostEndpoint), new PropertyMetadata(false));

		public string ClientAddress { get { return (string)GetValue(ClientAddressProperty); } set { SetValue(ClientAddressProperty, value); } }
		public static readonly DependencyProperty ClientAddressProperty = DependencyProperty.Register("ClientAddress", typeof(string), typeof(WcfHostEndpoint));

		public WcfHostEndpointIdentityType ClientIdentityType { get { return (WcfHostEndpointIdentityType)GetValue(ClientIdentityTypeProperty); } set { SetValue(ClientIdentityTypeProperty, value); } }
		public static readonly DependencyProperty ClientIdentityTypeProperty = DependencyProperty.Register("ClientIdentityType", typeof(WcfHostEndpointIdentityType), typeof(WcfHostEndpoint));

		public string ClientIdentityData { get { return (string)GetValue(ClientIdentityDataProperty); } set { SetValue(ClientIdentityDataProperty, value); } }
		public static readonly DependencyProperty ClientIdentityDataProperty = DependencyProperty.Register("ClientIdentityData", typeof(string), typeof(WcfHostEndpoint));

		public ObservableCollection<WcfHostEndpointAddressHeader> ClientAddressHeaders { get { return (ObservableCollection<WcfHostEndpointAddressHeader>)GetValue(ClientAddressHeadersProperty); } set { SetValue(ClientAddressHeadersProperty, value); } }
		public static readonly DependencyProperty ClientAddressHeadersProperty = DependencyProperty.Register("ClientAddressHeaders", typeof(ObservableCollection<WcfHostEndpointAddressHeader>), typeof(WcfHostEndpoint));

		public WcfHost Parent { get; set; }
		public bool IsSelected { get; set; }

		public WcfHostEndpoint() { }

		public WcfHostEndpoint(WcfHost Parent, string Name)
		{
			ClientAddressHeaders = new ObservableCollection<WcfHostEndpointAddressHeader>();
			ID = Guid.NewGuid();
			this.Parent = Parent;
			this.Name = Helpers.RegExs.ReplaceSpaces.Replace(Name, "");
		}

		public override string ToString()
		{
			return Name;
		}
	}

	public class WcfHostEndpointAddressHeader : DependencyObject
	{
		public Guid ID { get; set; }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(WcfHostEndpointAddressHeader));

		public string Namespace { get { return (string)GetValue(NamespaceProperty); } set { SetValue(NamespaceProperty, value); } }
		public static readonly DependencyProperty NamespaceProperty = DependencyProperty.Register("Namespace", typeof(string), typeof(WcfHostEndpointAddressHeader));

		public WcfHostEndpointAddressHeader() { }

		public WcfHostEndpointAddressHeader(string Name, string Namespace)
		{
			ID = Guid.NewGuid();
			this.Name = Name;
			this.Namespace = Namespace;
		}

		public override string ToString()
		{
			return Name;
		}
	}

	public class WcfHostCredentials : DependencyObject
	{
		public bool UseCertificatesSecurity { get { return (bool)GetValue(UseCertificatesSecurityProperty); } set { SetValue(UseCertificatesSecurityProperty, value); } }
		public static readonly DependencyProperty UseCertificatesSecurityProperty = DependencyProperty.Register("UseCertificatesSecurity", typeof(bool), typeof(WcfHostCredentials), new PropertyMetadata(false));

		public bool UseIssuedTokenSecurity { get { return (bool)GetValue(UseIssuedTokenSecurityProperty); } set { SetValue(UseIssuedTokenSecurityProperty, value); } }
		public static readonly DependencyProperty UseIssuedTokenSecurityProperty = DependencyProperty.Register("UseIssuedTokenSecurity", typeof(bool), typeof(WcfHostCredentials), new PropertyMetadata(false));

		public bool UsePeerSecurity { get { return (bool)GetValue(UsePeerSecurityProperty); } set { SetValue(UsePeerSecurityProperty, value); } }
		public static readonly DependencyProperty UsePeerSecurityProperty = DependencyProperty.Register("UsePeerSecurity", typeof(bool), typeof(WcfHostCredentials), new PropertyMetadata(false));

		public bool UseUserNamePasswordSecurity { get { return (bool)GetValue(UseUserNamePasswordSecurityProperty); } set { SetValue(UseUserNamePasswordSecurityProperty, value); } }
		public static readonly DependencyProperty UseUserNamePasswordSecurityProperty = DependencyProperty.Register("UseUserNamePasswordSecurity", typeof(bool), typeof(WcfHostCredentials), new PropertyMetadata(false));

		public bool UseWindowsServiceSecurity { get { return (bool)GetValue(UseWindowsServiceSecurityProperty); } set { SetValue(UseWindowsServiceSecurityProperty, value); } }
		public static readonly DependencyProperty UseWindowsServiceSecurityProperty = DependencyProperty.Register("UseWindowsServiceSecurity", typeof(bool), typeof(WcfHostCredentials), new PropertyMetadata(false));

		public System.ServiceModel.Security.X509CertificateValidationMode ClientCertificateAuthenticationValidationMode { get { return (System.ServiceModel.Security.X509CertificateValidationMode)GetValue(ClientCertificateAuthenticationValidationModeProperty); } set { SetValue(ClientCertificateAuthenticationValidationModeProperty, value); } }
		public static readonly DependencyProperty ClientCertificateAuthenticationValidationModeProperty = DependencyProperty.Register("ClientCertificateAuthenticationValidationMode", typeof(System.ServiceModel.Security.X509CertificateValidationMode), typeof(WcfHostBehavior), new PropertyMetadata(System.ServiceModel.Security.X509CertificateValidationMode.None));

		public bool ClientCertificateAuthenticationIncludeWindowsGroups { get { return (bool)GetValue(ClientCertificateAuthenticationIncludeWindowsGroupsProperty); } set { SetValue(ClientCertificateAuthenticationIncludeWindowsGroupsProperty, value); } }
		public static readonly DependencyProperty ClientCertificateAuthenticationIncludeWindowsGroupsProperty = DependencyProperty.Register("ClientCertificateAuthenticationIncludeWindowsGroups", typeof(bool), typeof(WcfHostBehavior));

		public bool ClientCertificateAuthenticationMapClientCertificateToWindowsAccount { get { return (bool)GetValue(ClientCertificateAuthenticationMapClientCertificateToWindowsAccountProperty); } set { SetValue(ClientCertificateAuthenticationMapClientCertificateToWindowsAccountProperty, value); } }
		public static readonly DependencyProperty ClientCertificateAuthenticationMapClientCertificateToWindowsAccountProperty = DependencyProperty.Register("ClientCertificateAuthenticationMapClientCertificateToWindowsAccount", typeof(bool), typeof(WcfHostBehavior));

		public System.Security.Cryptography.X509Certificates.X509RevocationMode ClientCertificateAuthenticationRevocationMode { get { return (System.Security.Cryptography.X509Certificates.X509RevocationMode)GetValue(ClientCertificateAuthenticationRevocationModeProperty); } set { SetValue(ClientCertificateAuthenticationRevocationModeProperty, value); } }
		public static readonly DependencyProperty ClientCertificateAuthenticationRevocationModeProperty = DependencyProperty.Register("ClientCertificateAuthenticationRevocationMode", typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), typeof(WcfHostBehavior), new PropertyMetadata(System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck));

		public System.Security.Cryptography.X509Certificates.StoreLocation ClientCertificateAuthenticationStoreLocation { get { return (System.Security.Cryptography.X509Certificates.StoreLocation)GetValue(ClientCertificateAuthenticationStoreLocationProperty); } set { SetValue(ClientCertificateAuthenticationStoreLocationProperty, value); } }
		public static readonly DependencyProperty ClientCertificateAuthenticationStoreLocationProperty = DependencyProperty.Register("ClientCertificateAuthenticationStoreLocation", typeof(System.Security.Cryptography.X509Certificates.StoreLocation), typeof(WcfHostBehavior), new PropertyMetadata(System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine));

		public ObservableCollection<string> IssuedTokenAllowedAudiencesUris { get { return (ObservableCollection<string>)GetValue(IssuedTokenAllowedAudiencesUrisProperty); } set { SetValue(IssuedTokenAllowedAudiencesUrisProperty, value); } }
		public static readonly DependencyProperty IssuedTokenAllowedAudiencesUrisProperty = DependencyProperty.Register("IssuedTokenAllowedAudiencesUris", typeof(ObservableCollection<string>), typeof(WcfHostBehavior));

		public bool IssuedTokenAllowUntrustedRsaIssuers { get { return (bool)GetValue(IssuedTokenAllowUntrustedRsaIssuersProperty); } set { SetValue(IssuedTokenAllowUntrustedRsaIssuersProperty, value); } }
		public static readonly DependencyProperty IssuedTokenAllowUntrustedRsaIssuersProperty = DependencyProperty.Register("IssuedTokenAllowUntrustedRsaIssuers", typeof(bool), typeof(WcfHostBehavior));

		public System.IdentityModel.Selectors.AudienceUriMode IssuedTokenAudienceUriMode { get { return (System.IdentityModel.Selectors.AudienceUriMode)GetValue(IssuedTokenAudienceUriModeProperty); } set { SetValue(IssuedTokenAudienceUriModeProperty, value); } }
		public static readonly DependencyProperty IssuedTokenAudienceUriModeProperty = DependencyProperty.Register("IssuedTokenAudienceUriMode", typeof(System.IdentityModel.Selectors.AudienceUriMode), typeof(WcfHostBehavior), new PropertyMetadata(System.IdentityModel.Selectors.AudienceUriMode.Never));

		public System.ServiceModel.Security.X509CertificateValidationMode IssuedTokenCertificateValidationMode { get { return (System.ServiceModel.Security.X509CertificateValidationMode)GetValue(IssuedTokenCertificateValidationModeProperty); } set { SetValue(IssuedTokenCertificateValidationModeProperty, value); } }
		public static readonly DependencyProperty IssuedTokenCertificateValidationModeProperty = DependencyProperty.Register("IssuedTokenCertificateValidationMode", typeof(System.ServiceModel.Security.X509CertificateValidationMode), typeof(WcfHostBehavior), new PropertyMetadata(System.ServiceModel.Security.X509CertificateValidationMode.None));

		public System.Security.Cryptography.X509Certificates.X509RevocationMode IssuedTokenRevocationMode { get { return (System.Security.Cryptography.X509Certificates.X509RevocationMode)GetValue(IssuedTokenRevocationModeProperty); } set { SetValue(IssuedTokenRevocationModeProperty, value); } }
		public static readonly DependencyProperty IssuedTokenRevocationModeProperty = DependencyProperty.Register("IssuedTokenRevocationMode", typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), typeof(WcfHostBehavior), new PropertyMetadata(System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck));

		public System.Security.Cryptography.X509Certificates.StoreLocation IssuedTokenTrustedStoreLocation { get { return (System.Security.Cryptography.X509Certificates.StoreLocation)GetValue(IssuedTokenTrustedStoreLocationProperty); } set { SetValue(IssuedTokenTrustedStoreLocationProperty, value); } }
		public static readonly DependencyProperty IssuedTokenTrustedStoreLocationProperty = DependencyProperty.Register("IssuedTokenTrustedStoreLocation", typeof(System.Security.Cryptography.X509Certificates.StoreLocation), typeof(WcfHostBehavior), new PropertyMetadata(System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine));

		public string PeerMeshPassword { get { return (string)GetValue(PeerMeshPasswordProperty); } set { SetValue(PeerMeshPasswordProperty, value); } }
		public static readonly DependencyProperty PeerMeshPasswordProperty = DependencyProperty.Register("PeerMeshPassword", typeof(string), typeof(WcfHostBehavior));

		public System.ServiceModel.Security.X509CertificateValidationMode PeerMessageSenderAuthenticationCertificateValidationMode { get { return (System.ServiceModel.Security.X509CertificateValidationMode)GetValue(PeerMessageSenderAuthenticationCertificateValidationModeProperty); } set { SetValue(PeerMessageSenderAuthenticationCertificateValidationModeProperty, value); } }
		public static readonly DependencyProperty PeerMessageSenderAuthenticationCertificateValidationModeProperty = DependencyProperty.Register("PeerMessageSenderAuthenticationCertificateValidationMode ", typeof(System.ServiceModel.Security.X509CertificateValidationMode), typeof(WcfHostBehavior), new PropertyMetadata(System.ServiceModel.Security.X509CertificateValidationMode.None));

		public System.Security.Cryptography.X509Certificates.X509RevocationMode PeerMessageSenderAuthenticationRevocationMode { get { return (System.Security.Cryptography.X509Certificates.X509RevocationMode)GetValue(PeerMessageSenderAuthenticationRevocationModeProperty); } set { SetValue(PeerMessageSenderAuthenticationRevocationModeProperty, value); } }
		public static readonly DependencyProperty PeerMessageSenderAuthenticationRevocationModeProperty = DependencyProperty.Register("PeerMessageSenderAuthenticationRevocationMode", typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), typeof(WcfHostBehavior), new PropertyMetadata(System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck));

		public System.Security.Cryptography.X509Certificates.StoreLocation PeerMessageSenderAuthenticationTrustedStoreLocation { get { return (System.Security.Cryptography.X509Certificates.StoreLocation)GetValue(PeerMessageSenderAuthenticationTrustedStoreLocationProperty); } set { SetValue(PeerMessageSenderAuthenticationTrustedStoreLocationProperty, value); } }
		public static readonly DependencyProperty PeerMessageSenderAuthenticationTrustedStoreLocationProperty = DependencyProperty.Register("PeerMessageSenderAuthenticationTrustedStoreLocation", typeof(System.Security.Cryptography.X509Certificates.StoreLocation), typeof(WcfHostBehavior), new PropertyMetadata(System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine));

		public System.ServiceModel.Security.X509CertificateValidationMode PeerAuthenticationCertificateValidationMode { get { return (System.ServiceModel.Security.X509CertificateValidationMode)GetValue(PeerAuthenticationCertificateValidationModeProperty); } set { SetValue(PeerAuthenticationCertificateValidationModeProperty, value); } }
		public static readonly DependencyProperty PeerAuthenticationCertificateValidationModeProperty = DependencyProperty.Register("PeerAuthenticationCertificateValidationMode", typeof(System.ServiceModel.Security.X509CertificateValidationMode), typeof(WcfHostBehavior), new PropertyMetadata(System.ServiceModel.Security.X509CertificateValidationMode.None));

		public System.Security.Cryptography.X509Certificates.X509RevocationMode PeerAuthenticationRevocationMode { get { return (System.Security.Cryptography.X509Certificates.X509RevocationMode)GetValue(PeerAuthenticationRevocationModeProperty); } set { SetValue(PeerAuthenticationRevocationModeProperty, value); } }
		public static readonly DependencyProperty PeerAuthenticationRevocationModeProperty = DependencyProperty.Register("PeerAuthenticationRevocationMode", typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), typeof(WcfHostBehavior), new PropertyMetadata(System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck));

		public System.Security.Cryptography.X509Certificates.StoreLocation PeerAuthenticationTrustedStoreLocation { get { return (System.Security.Cryptography.X509Certificates.StoreLocation)GetValue(PeerAuthenticationTrustedStoreLocationProperty); } set { SetValue(PeerAuthenticationTrustedStoreLocationProperty, value); } }
		public static readonly DependencyProperty PeerAuthenticationTrustedStoreLocationProperty = DependencyProperty.Register("PeerAuthenticationTrustedStoreLocation", typeof(System.Security.Cryptography.X509Certificates.StoreLocation), typeof(WcfHostBehavior), new PropertyMetadata(System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine));

		public TimeSpan UserNamePasswordCachedLogonTokenLifetime { get { return (TimeSpan)GetValue(UserNamePasswordCachedLogonTokenLifetimeProperty); } set { SetValue(UserNamePasswordCachedLogonTokenLifetimeProperty, value); } } 
		public static readonly DependencyProperty UserNamePasswordCachedLogonTokenLifetimeProperty = DependencyProperty.Register("UserNamePasswordCachedLogonTokenLifetime", typeof(TimeSpan), typeof(WcfHostBehavior));

		public bool UserNamePasswordCacheLogonTokens { get { return (bool)GetValue(UserNamePasswordCacheLogonTokensProperty); } set { SetValue(UserNamePasswordCacheLogonTokensProperty, value); } }
		public static readonly DependencyProperty UserNamePasswordCacheLogonTokensProperty = DependencyProperty.Register("UserNamePasswordCacheLogonTokens", typeof(bool), typeof(WcfHostBehavior));

		public bool UserNamePasswordIncludeWindowsGroups { get { return (bool)GetValue(UserNamePasswordIncludeWindowsGroupsProperty); } set { SetValue(UserNamePasswordIncludeWindowsGroupsProperty, value); } }
		public static readonly DependencyProperty UserNamePasswordIncludeWindowsGroupsProperty = DependencyProperty.Register("UserNamePasswordIncludeWindowsGroups", typeof(bool), typeof(WcfHostBehavior));

		public int UserNamePasswordMaxCachedLogonTokens { get { return (int)GetValue(UserNamePasswordMaxCachedLogonTokensProperty); } set { SetValue(UserNamePasswordMaxCachedLogonTokensProperty, value); } }
		public static readonly DependencyProperty UserNamePasswordMaxCachedLogonTokensProperty = DependencyProperty.Register("UserNamePasswordMaxCachedLogonTokens", typeof(int), typeof(WcfHostBehavior));

		public System.ServiceModel.Security.UserNamePasswordValidationMode UserNamePasswordValidationMode { get { return (System.ServiceModel.Security.UserNamePasswordValidationMode)GetValue(UserNamePasswordValidationModeProperty); } set { SetValue(UserNamePasswordValidationModeProperty, value); } }
		public static readonly DependencyProperty UserNamePasswordValidationModeProperty = DependencyProperty.Register("UserNamePasswordValidationMode", typeof(System.ServiceModel.Security.UserNamePasswordValidationMode), typeof(WcfHostBehavior), new PropertyMetadata(System.ServiceModel.Security.UserNamePasswordValidationMode.Windows));

		public bool WindowsServiceAllowAnonymousLogons { get { return (bool)GetValue(WindowsServiceAllowAnonymousLogonsProperty); } set { SetValue(WindowsServiceAllowAnonymousLogonsProperty, value); } }
		public static readonly DependencyProperty WindowsServiceAllowAnonymousLogonsProperty = DependencyProperty.Register("WindowsServiceAllowAnonymousLogons", typeof(bool), typeof(WcfHostBehavior));

		public bool WindowsServiceIncludeWindowsGroups { get { return (bool)GetValue(WindowsServiceIncludeWindowsGroupsProperty); } set { SetValue(WindowsServiceIncludeWindowsGroupsProperty, value); } }
		public static readonly DependencyProperty WindowsServiceIncludeWindowsGroupsProperty = DependencyProperty.Register("WindowsServiceIncludeWindowsGroups", typeof(bool), typeof(WcfHostBehavior));

		public WcfHost Owner { get; set; }

		public WcfHostCredentials()
		{
			IssuedTokenAllowedAudiencesUris = new ObservableCollection<string>();
		}

		public WcfHostCredentials(WcfHost Owner)
		{
			IssuedTokenAllowedAudiencesUris = new ObservableCollection<string>();
			this.Owner = Owner;
		}

		public override string ToString()
		{
			return Owner.Name + "Credentials";
		}
	}

	public abstract class WcfHostBehavior : DependencyObject
	{
		public Guid ID { get; set; }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(WcfHostBehavior));

		public bool IsDefaultBehavior { get { return (bool)GetValue(IsDefaultBehaviorProperty); } set { SetValue(IsDefaultBehaviorProperty, value); } }
		public static readonly DependencyProperty IsDefaultBehaviorProperty = DependencyProperty.Register("IsDefaultBehavior", typeof(bool), typeof(WcfHostBehavior), new UIPropertyMetadata(false, IsDefaultBehaviorChangedCallback));

		private static void IsDefaultBehaviorChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfHostBehavior;
			if (t == null) return;
			if (t.Parent == null) return;
			if (t.Parent.Behaviors == null) return;

			if(o.GetType() == typeof(WcfHostDebugBehavior))
				foreach (var hb in t.Parent.Behaviors.OfType<WcfHostDebugBehavior>().Where(a => a.GetType() == typeof (WcfHostDebugBehavior) && !Equals(a, o)))
					hb.IsDefaultBehavior = false;
			if (o.GetType() == typeof(WcfHostMetadataBehavior))
				foreach (var hb in t.Parent.Behaviors.OfType<WcfHostMetadataBehavior>().Where(a => a.GetType() == typeof(WcfHostMetadataBehavior) && !Equals(a, o)))
					hb.IsDefaultBehavior = false;
			if (o.GetType() == typeof(WcfHostThrottlingBehavior))
				foreach (var hb in t.Parent.Behaviors.OfType<WcfHostThrottlingBehavior>().Where(a => a.GetType() == typeof(WcfHostThrottlingBehavior) && !Equals(a, o)))
					hb.IsDefaultBehavior = false;
		}

		public WcfHost Parent { get { return (WcfHost)GetValue(ParentProperty); } set { SetValue(ParentProperty, value); } }
		public static readonly DependencyProperty ParentProperty =  DependencyProperty.Register("Parent", typeof(WcfHost), typeof(WcfHostBehavior));

		public bool IsSelected { get; set; }

		public WcfHostBehavior() { }

		public override string ToString()
		{
			return Name;
		}
	}

	public class WcfHostDebugBehavior : WcfHostBehavior
	{
		public WcfBinding HttpHelpPageBinding { get { return (WcfBinding)GetValue(HttpHelpPageBindingProperty); } set { SetValue(HttpHelpPageBindingProperty, value); } }
		public static readonly DependencyProperty HttpHelpPageBindingProperty = DependencyProperty.Register("HttpHelpPageBinding", typeof(WcfBinding), typeof(WcfHostDebugBehavior));

		public bool HttpHelpPageEnabled { get { return (bool)GetValue(HttpHelpPageEnabledProperty); } set { SetValue(HttpHelpPageEnabledProperty, value); } }
		public static readonly DependencyProperty HttpHelpPageEnabledProperty = DependencyProperty.Register("HttpHelpPageEnabled", typeof(bool), typeof(WcfHostDebugBehavior));

		public string HttpHelpPageUrl { get { return (string)GetValue(HttpHelpPageUrlProperty); } set { SetValue(HttpHelpPageUrlProperty, value); } }
		public static readonly DependencyProperty HttpHelpPageUrlProperty = DependencyProperty.Register("HttpHelpPageUrl", typeof(string), typeof(WcfHostDebugBehavior));

		public WcfBinding HttpsHelpPageBinding { get { return (WcfBinding)GetValue(HttpsHelpPageBindingProperty); } set { SetValue(HttpsHelpPageBindingProperty, value); } }
		public static readonly DependencyProperty HttpsHelpPageBindingProperty = DependencyProperty.Register("HttpsHelpPageBinding", typeof(WcfBinding), typeof(WcfHostDebugBehavior));

		public bool HttpsHelpPageEnabled { get { return (bool)GetValue(HttpsHelpPageEnabledProperty); } set { SetValue(HttpsHelpPageEnabledProperty, value); } }
		public static readonly DependencyProperty HttpsHelpPageEnabledProperty = DependencyProperty.Register("HttpsHelpPageEnabled", typeof(bool), typeof(WcfHostDebugBehavior));

		public string HttpsHelpPageUrl { get { return (string)GetValue(HttpsHelpPageUrlProperty); } set { SetValue(HttpsHelpPageUrlProperty, value); } }
		public static readonly DependencyProperty HttpsHelpPageUrlProperty = DependencyProperty.Register("HttpsHelpPageUrl", typeof(string), typeof(WcfHostDebugBehavior));

		public bool IncludeExceptionDetailInFaults { get { return (bool)GetValue(IncludeExceptionDetailInFaultsProperty); } set { SetValue(IncludeExceptionDetailInFaultsProperty, value); } }
		public static readonly DependencyProperty IncludeExceptionDetailInFaultsProperty = DependencyProperty.Register("IncludeExceptionDetailInFaults", typeof(bool), typeof(WcfHostDebugBehavior));

		public WcfHostDebugBehavior()
		{
			ID = Guid.NewGuid();
			HttpHelpPageBinding = null;
			HttpHelpPageEnabled = false;
			HttpHelpPageUrl = "";
			HttpsHelpPageBinding = null;
			HttpsHelpPageEnabled = false;
			HttpsHelpPageUrl = "";
			IncludeExceptionDetailInFaults = false;
		}

		public WcfHostDebugBehavior(string Name, WcfHost Parent)
		{
			ID = Guid.NewGuid();
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;
			IsDefaultBehavior = false;

			HttpHelpPageBinding = null;
			HttpHelpPageEnabled = false;
			HttpHelpPageUrl = "";
			HttpsHelpPageBinding = null;
			HttpsHelpPageEnabled = false;
			HttpsHelpPageUrl = "";
			IncludeExceptionDetailInFaults = false;
		}
	}

	public class WcfHostMetadataBehavior : WcfHostBehavior
	{
		public string ExternalMetadataLocation { get { return (string)GetValue(ExternalMetadataLocationProperty); } set { SetValue(ExternalMetadataLocationProperty, value); } }
		public static readonly DependencyProperty ExternalMetadataLocationProperty =  DependencyProperty.Register("ExternalMetadataLocation", typeof(string), typeof(WcfHostMetadataBehavior));

		public WcfBinding HttpGetBinding { get { return (WcfBinding)GetValue(HttpGetBindingProperty); } set { SetValue(HttpGetBindingProperty, value); } }
		public static readonly DependencyProperty HttpGetBindingProperty = DependencyProperty.Register("HttpGetBinding", typeof(WcfBinding), typeof(WcfHostMetadataBehavior));

		public bool HttpGetEnabled { get { return (bool)GetValue(HttpGetEnabledProperty); } set { SetValue(HttpGetEnabledProperty, value); } }
		public static readonly DependencyProperty HttpGetEnabledProperty = DependencyProperty.Register("HttpGetEnabled", typeof(bool), typeof(WcfHostMetadataBehavior), new PropertyMetadata(false, HttpGetEnabledChangedCallback));

		private static void HttpGetEnabledChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfHostMetadataBehavior;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue)) return;
			t.HttpGetBinding = null;
			t.HttpGetUrl = "";
		}

		public string HttpGetUrl { get { return (string)GetValue(HttpGetUrlProperty); } set { SetValue(HttpGetUrlProperty, value); } }
		public static readonly DependencyProperty HttpGetUrlProperty = DependencyProperty.Register("HttpGetUrl", typeof(string), typeof(WcfHostMetadataBehavior));

		public WcfBinding HttpsGetBinding { get { return (WcfBinding)GetValue(HttpsGetBindingProperty); } set { SetValue(HttpsGetBindingProperty, value); } }
		public static readonly DependencyProperty HttpsGetBindingProperty = DependencyProperty.Register("HttpsGetBinding", typeof(WcfBinding), typeof(WcfHostMetadataBehavior));

		public bool HttpsGetEnabled { get { return (bool)GetValue(HttpsGetEnabledProperty); } set { SetValue(HttpsGetEnabledProperty, value); } }
		public static readonly DependencyProperty HttpsGetEnabledProperty = DependencyProperty.Register("HttpsGetEnabled", typeof(bool), typeof(WcfHostMetadataBehavior), new PropertyMetadata(false, HttpsGetEnabledChangedCallback));

		private static void HttpsGetEnabledChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as WcfHostMetadataBehavior;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue)) return;
			t.HttpsGetBinding = null;
			t.HttpsGetUrl = "";
		}

		public string HttpsGetUrl { get { return (string)GetValue(HttpsGetUrlProperty); } set { SetValue(HttpsGetUrlProperty, value); } }
		public static readonly DependencyProperty HttpsGetUrlProperty = DependencyProperty.Register("HttpsGetUrl", typeof(string), typeof(WcfHostMetadataBehavior));

		public WcfHostMetadataBehavior()
		{
			ID = Guid.NewGuid();

			ExternalMetadataLocation = "";
			HttpGetBinding = null;
			HttpGetEnabled = false;
			HttpGetUrl = "";
			HttpsGetBinding = null;
			HttpsGetEnabled = false;
			HttpsGetUrl = "";
		}

		public WcfHostMetadataBehavior(string Name, WcfHost Parent)
		{
			ID = Guid.NewGuid();
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;

			ExternalMetadataLocation = "";
			HttpGetBinding = null;
			HttpGetEnabled = false;
			HttpGetUrl = "";
			HttpsGetBinding = null;
			HttpsGetEnabled = false;
			HttpsGetUrl = "";
		}
	}

	public class WcfHostThrottlingBehavior : WcfHostBehavior
	{
		public int MaxConcurrentCalls { get { return (int)GetValue(MaxConcurrentCallsProperty); } set { SetValue(MaxConcurrentCallsProperty, value); } }
		public static readonly DependencyProperty MaxConcurrentCallsProperty =  DependencyProperty.Register("MaxConcurrentCalls", typeof(int), typeof(WcfHostThrottlingBehavior));

		public int MaxConcurrentInstances { get { return (int)GetValue(MaxConcurrentInstancesProperty); } set { SetValue(MaxConcurrentInstancesProperty, value); } }
		public static readonly DependencyProperty MaxConcurrentInstancesProperty = DependencyProperty.Register("MaxConcurrentInstances", typeof(int), typeof(WcfHostThrottlingBehavior));

		public int MaxConcurrentSessions { get { return (int)GetValue(MaxConcurrentSessionsProperty); } set { SetValue(MaxConcurrentSessionsProperty, value); } }
		public static readonly DependencyProperty MaxConcurrentSessionsProperty = DependencyProperty.Register("MaxConcurrentSessions", typeof(int), typeof(WcfHostThrottlingBehavior));

		public WcfHostThrottlingBehavior()
		{
			ID = Guid.NewGuid();

			MaxConcurrentCalls = 16;
			MaxConcurrentInstances = 26;
			MaxConcurrentSessions = 10;
		}

		public WcfHostThrottlingBehavior(string Name, WcfHost Parent)
		{
			ID = Guid.NewGuid();
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;

			MaxConcurrentCalls = 16;
			MaxConcurrentInstances = 26;
			MaxConcurrentSessions = 10;
		}
	}
}