using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;

namespace NETPath.Projects
{
	public enum HostEndpointIdentityType
	{
		Anonymous,
		DNS,
		RSA,
		RSAX509,
		SPN,
		UPN,
		X509
	}

	public class Host : DataType
	{
		public string Namespace { get { return (string)GetValue(NamespaceProperty); } set { SetValue(NamespaceProperty, value); } }
		public static readonly DependencyProperty NamespaceProperty = DependencyProperty.Register("Namespace", typeof(string), typeof(Host));
		
		public string ConfigurationName { get { return (string)GetValue(ConfigurationNameProperty); } set { SetValue(ConfigurationNameProperty, value); } }
		public static readonly DependencyProperty ConfigurationNameProperty = DependencyProperty.Register("ConfigurationName", typeof(string), typeof(Host));
		
		public HostCredentials Credentials { get { return (HostCredentials)GetValue(CredentialsProperty); } set { SetValue(CredentialsProperty, value); } }
		public static readonly DependencyProperty CredentialsProperty = DependencyProperty.Register("Credentials", typeof(HostCredentials), typeof(Host));

		public ObservableCollection<string> BaseAddresses { get { return (ObservableCollection<string>)GetValue(BaseAddressProperty); } set { SetValue(BaseAddressProperty, value); } }
		public static readonly DependencyProperty BaseAddressProperty = DependencyProperty.Register("BaseAddress", typeof(ObservableCollection<string>), typeof(Host));
		
		public TimeSpan CloseTimeout { get { return (TimeSpan)GetValue(CloseTimeoutProperty); } set { SetValue(CloseTimeoutProperty, value); } }
		public static readonly DependencyProperty CloseTimeoutProperty = DependencyProperty.Register("CloseTimeout", typeof (TimeSpan), typeof (Host), new PropertyMetadata(new TimeSpan(0, 1, 0)));
		
		public TimeSpan OpenTimeout { get { return (TimeSpan)GetValue(OpenTimeoutProperty); } set { SetValue(OpenTimeoutProperty, value); } }
		public static readonly DependencyProperty OpenTimeoutProperty = DependencyProperty.Register("OpenTimeout", typeof(TimeSpan), typeof(Host), new PropertyMetadata(new TimeSpan(0, 1, 0)));

		public int ManualFlowControlLimit { get { return (int)GetValue(ManualFlowControlLimitProperty); } set { SetValue(ManualFlowControlLimitProperty, value); } }
		public static readonly DependencyProperty ManualFlowControlLimitProperty = DependencyProperty.Register("ManualFlowControlLimit", typeof(int), typeof(Host));

		public bool AuthorizationImpersonateCallerForAllOperations { get { return (bool)GetValue(AuthorizationImpersonateCallerForAllOperationsProperty); } set { SetValue(AuthorizationImpersonateCallerForAllOperationsProperty, value); } }
		public static readonly DependencyProperty AuthorizationImpersonateCallerForAllOperationsProperty = DependencyProperty.Register("AuthorizationImpersonateCallerForAllOperations", typeof(bool), typeof(Host));
		
		public System.ServiceModel.Description.PrincipalPermissionMode AuthorizationPrincipalPermissionMode { get { return (System.ServiceModel.Description.PrincipalPermissionMode)GetValue(AuthorizationPrincipalPermissionModeProperty); } set { SetValue(AuthorizationPrincipalPermissionModeProperty, value); } }
		public static readonly DependencyProperty AuthorizationPrincipalPermissionModeProperty = DependencyProperty.Register("AuthorizationPrincipalPermissionMode", typeof(System.ServiceModel.Description.PrincipalPermissionMode), typeof(Host));

		public Service Service { get { return (Service)GetValue(ServiceProperty); } set { SetValue(ServiceProperty, value); } }
		public static readonly DependencyProperty ServiceProperty = DependencyProperty.Register("Service", typeof(Service), typeof(Host));

		public Documentation Documentation { get { return (Documentation)GetValue(DocumentationProperty); } set { SetValue(DocumentationProperty, value); } }
		public static readonly DependencyProperty DocumentationProperty = DependencyProperty.Register("Documentation", typeof(Documentation), typeof(Host));

		public ObservableCollection<HostBehavior> Behaviors { get { return (ObservableCollection<HostBehavior>)GetValue(BehaviorsProperty); } set { SetValue(BehaviorsProperty, value); } }
		public static readonly DependencyProperty BehaviorsProperty = DependencyProperty.Register("Behaviors", typeof(ObservableCollection<HostBehavior>), typeof(Host));

		public ObservableCollection<HostEndpoint> Endpoints { get { return (ObservableCollection<HostEndpoint>)GetValue(EndpointsProperty); } set { SetValue(EndpointsProperty, value); } }
		public static readonly DependencyProperty EndpointsProperty = DependencyProperty.Register("Endpoints", typeof(ObservableCollection<HostEndpoint>), typeof(Host));

		public Host() : base(DataTypeMode.Class)
		{
			Endpoints = new ObservableCollection<HostEndpoint>();
			Documentation = new Documentation { IsClass = true };
		}

		public Host(string Name, Namespace Parent) : base(DataTypeMode.Class)
		{
			BaseAddresses = new ObservableCollection<string>();
			Behaviors = new ObservableCollection<HostBehavior>();
			Endpoints = new ObservableCollection<HostEndpoint>();
			ID = Guid.NewGuid();
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;
			Namespace = Parent.Owner.Namespace.URI;
			Credentials = new HostCredentials(this);
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

			foreach (HostEndpoint HE in Endpoints)
				results.AddRange(HE.FindReplace(Args));

			foreach (HostBehavior HB in Behaviors)
				results.AddRange(HB.FindReplace(Args));

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

	public class HostEndpoint : DependencyObject
	{
		public Guid ID { get; set; }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(HostEndpoint));

		public ServiceBinding Binding { get { return (ServiceBinding)GetValue(BindingProperty); } set { SetValue(BindingProperty, value); } }
		public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding", typeof(ServiceBinding), typeof(HostEndpoint));

		public bool UseProtocolBufferSerialization { get { return (bool)GetValue(UseProtocolBufferSerializationProperty); } set { SetValue(UseProtocolBufferSerializationProperty, value); } }
		public static readonly DependencyProperty UseProtocolBufferSerializationProperty = DependencyProperty.Register("UseProtocolBufferSerialization", typeof(bool), typeof(HostEndpoint), new PropertyMetadata(false));

		public string ServerAddress { get { return (string)GetValue(ServerAddressProperty); } set { SetValue(ServerAddressProperty, value); } }
		public static readonly DependencyProperty ServerAddressProperty = DependencyProperty.Register("ServerAddress", typeof(string), typeof(HostEndpoint));

		public int ServerPort { get { return (int)GetValue(ServerPortProperty); } set { SetValue(ServerPortProperty, value); } }
		public static readonly DependencyProperty ServerPortProperty = DependencyProperty.Register("ServerPort", typeof(int), typeof(HostEndpoint));

		public bool ServerUseHTTPS { get { return (bool)GetValue(ServerUseHTTPSProperty); } set { SetValue(ServerUseHTTPSProperty, value); } }
		public static readonly DependencyProperty ServerUseHTTPSProperty = DependencyProperty.Register("ServerUseHTTPS", typeof(bool), typeof(HostEndpoint), new PropertyMetadata(false));

		public string ClientAddress { get { return (string)GetValue(ClientAddressProperty); } set { SetValue(ClientAddressProperty, value); } }
		public static readonly DependencyProperty ClientAddressProperty = DependencyProperty.Register("ClientAddress", typeof(string), typeof(HostEndpoint));

		public HostEndpointIdentityType ClientIdentityType { get { return (HostEndpointIdentityType)GetValue(ClientIdentityTypeProperty); } set { SetValue(ClientIdentityTypeProperty, value); } }
		public static readonly DependencyProperty ClientIdentityTypeProperty = DependencyProperty.Register("ClientIdentityType", typeof(HostEndpointIdentityType), typeof(HostEndpoint));

		public string ClientIdentityData { get { return (string)GetValue(ClientIdentityDataProperty); } set { SetValue(ClientIdentityDataProperty, value); } }
		public static readonly DependencyProperty ClientIdentityDataProperty = DependencyProperty.Register("ClientIdentityData", typeof(string), typeof(HostEndpoint));

		public ObservableCollection<HostEndpointAddressHeader> ClientAddressHeaders { get { return (ObservableCollection<HostEndpointAddressHeader>)GetValue(ClientAddressHeadersProperty); } set { SetValue(ClientAddressHeadersProperty, value); } }
		public static readonly DependencyProperty ClientAddressHeadersProperty = DependencyProperty.Register("ClientAddressHeaders", typeof(ObservableCollection<HostEndpointAddressHeader>), typeof(HostEndpoint));

		public Host Parent { get; set; }
		public bool IsSelected { get; set; }

		public HostEndpoint() { }

		public HostEndpoint(Host Parent, string Name)
		{
			ClientAddressHeaders = new ObservableCollection<HostEndpointAddressHeader>();
			ID = Guid.NewGuid();
			this.Parent = Parent;
			this.Name = Helpers.RegExs.ReplaceSpaces.Replace(Name, "");
		}

		public override string ToString()
		{
			return Name;
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
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Parent.Owner, this));
						if (!string.IsNullOrEmpty(ServerAddress)) if (ServerAddress.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Server Address", ServerAddress, Parent.Parent.Owner, this));
						if (!string.IsNullOrEmpty(ClientAddress)) if (ClientAddress.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Client Address", ClientAddress, Parent.Parent.Owner, this));
					}
					else
					{
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Parent.Owner, this));
						if (!string.IsNullOrEmpty(ServerAddress)) if (ServerAddress.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Server Address", ServerAddress, Parent.Parent.Owner, this));
						if (!string.IsNullOrEmpty(ClientAddress)) if (ClientAddress.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Client Address", ClientAddress, Parent.Parent.Owner, this));
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(Name)) if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Parent.Parent.Owner, this));
					if (!string.IsNullOrEmpty(ServerAddress)) if (Args.RegexSearch.IsMatch(ServerAddress)) results.Add(new FindReplaceResult("Server Address", ServerAddress, Parent.Parent.Owner, this));
					if (!string.IsNullOrEmpty(ClientAddress)) if (Args.RegexSearch.IsMatch(ClientAddress)) results.Add(new FindReplaceResult("Client Address", ClientAddress, Parent.Parent.Owner, this));
				}

				if (Args.ReplaceAll)
				{
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (!string.IsNullOrEmpty(ServerAddress)) ServerAddress = Microsoft.VisualBasic.Strings.Replace(ServerAddress, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (!string.IsNullOrEmpty(ClientAddress)) ClientAddress = Microsoft.VisualBasic.Strings.Replace(ClientAddress, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
						else
						{
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
							if (!string.IsNullOrEmpty(ServerAddress)) ServerAddress = Microsoft.VisualBasic.Strings.Replace(ServerAddress, Args.Search, Args.Replace);
							if (!string.IsNullOrEmpty(ClientAddress)) ClientAddress = Microsoft.VisualBasic.Strings.Replace(ClientAddress, Args.Search, Args.Replace);
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(Name)) Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (!string.IsNullOrEmpty(ServerAddress)) ServerAddress = Args.RegexSearch.Replace(ServerAddress, Args.Replace);
						if (!string.IsNullOrEmpty(ClientAddress)) ClientAddress = Args.RegexSearch.Replace(ClientAddress, Args.Replace);
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
					if (Field == "Server Address") ServerAddress = Microsoft.VisualBasic.Strings.Replace(ServerAddress, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					if (Field == "Client Address") ClientAddress = Microsoft.VisualBasic.Strings.Replace(ClientAddress, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
				}
				else
				{
					if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
					if (Field == "Server Address") ServerAddress = Microsoft.VisualBasic.Strings.Replace(ServerAddress, Args.Search, Args.Replace);
					if (Field == "Client Address") ClientAddress = Microsoft.VisualBasic.Strings.Replace(ClientAddress, Args.Search, Args.Replace);
				}
			}
			else
			{
				if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
				if (Field == "Server Address") ServerAddress = Args.RegexSearch.Replace(ServerAddress, Args.Replace);
				if (Field == "Client Address") ClientAddress = Args.RegexSearch.Replace(ClientAddress, Args.Replace);
			}
		}
	}

	public class HostEndpointAddressHeader : DependencyObject
	{
		public Guid ID { get; set; }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(HostEndpointAddressHeader));

		public string Namespace { get { return (string)GetValue(NamespaceProperty); } set { SetValue(NamespaceProperty, value); } }
		public static readonly DependencyProperty NamespaceProperty = DependencyProperty.Register("Namespace", typeof(string), typeof(HostEndpointAddressHeader));

		public HostEndpointAddressHeader() { }

		public HostEndpointAddressHeader(string Name, string Namespace)
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

	public class HostCredentials : DependencyObject
	{
		public bool UseCertificatesSecurity { get { return (bool)GetValue(UseCertificatesSecurityProperty); } set { SetValue(UseCertificatesSecurityProperty, value); } }
		public static readonly DependencyProperty UseCertificatesSecurityProperty = DependencyProperty.Register("UseCertificatesSecurity", typeof(bool), typeof(HostCredentials), new PropertyMetadata(false));
		
		public bool UseIssuedTokenSecurity { get { return (bool)GetValue(UseIssuedTokenSecurityProperty); } set { SetValue(UseIssuedTokenSecurityProperty, value); } }
		public static readonly DependencyProperty UseIssuedTokenSecurityProperty = DependencyProperty.Register("UseIssuedTokenSecurity", typeof(bool), typeof(HostCredentials), new PropertyMetadata(false));
		
		public bool UsePeerSecurity { get { return (bool)GetValue(UsePeerSecurityProperty); } set { SetValue(UsePeerSecurityProperty, value); } }
		public static readonly DependencyProperty UsePeerSecurityProperty = DependencyProperty.Register("UsePeerSecurity", typeof(bool), typeof(HostCredentials), new PropertyMetadata(false));
		
		public bool UseUserNamePasswordSecurity { get { return (bool)GetValue(UseUserNamePasswordSecurityProperty); } set { SetValue(UseUserNamePasswordSecurityProperty, value); } }
		public static readonly DependencyProperty UseUserNamePasswordSecurityProperty = DependencyProperty.Register("UseUserNamePasswordSecurity", typeof(bool), typeof(HostCredentials), new PropertyMetadata(false));
		
		public bool UseWindowsServiceSecurity { get { return (bool)GetValue(UseWindowsServiceSecurityProperty); } set { SetValue(UseWindowsServiceSecurityProperty, value); } }
		public static readonly DependencyProperty UseWindowsServiceSecurityProperty = DependencyProperty.Register("UseWindowsServiceSecurity", typeof(bool), typeof(HostCredentials), new PropertyMetadata(false));

		public System.ServiceModel.Security.X509CertificateValidationMode ClientCertificateAuthenticationValidationMode { get { return (System.ServiceModel.Security.X509CertificateValidationMode)GetValue(ClientCertificateAuthenticationValidationModeProperty); } set { SetValue(ClientCertificateAuthenticationValidationModeProperty, value); } }
		public static readonly DependencyProperty ClientCertificateAuthenticationValidationModeProperty = DependencyProperty.Register("ClientCertificateAuthenticationValidationMode", typeof(System.ServiceModel.Security.X509CertificateValidationMode), typeof(HostBehavior), new PropertyMetadata(System.ServiceModel.Security.X509CertificateValidationMode.None));
		
		public bool ClientCertificateAuthenticationIncludeWindowsGroups { get { return (bool)GetValue(ClientCertificateAuthenticationIncludeWindowsGroupsProperty); } set { SetValue(ClientCertificateAuthenticationIncludeWindowsGroupsProperty, value); } }
		public static readonly DependencyProperty ClientCertificateAuthenticationIncludeWindowsGroupsProperty = DependencyProperty.Register("ClientCertificateAuthenticationIncludeWindowsGroups", typeof(bool), typeof(HostBehavior));
		
		public bool ClientCertificateAuthenticationMapClientCertificateToWindowsAccount { get { return (bool)GetValue(ClientCertificateAuthenticationMapClientCertificateToWindowsAccountProperty); } set { SetValue(ClientCertificateAuthenticationMapClientCertificateToWindowsAccountProperty, value); } }
		public static readonly DependencyProperty ClientCertificateAuthenticationMapClientCertificateToWindowsAccountProperty = DependencyProperty.Register("ClientCertificateAuthenticationMapClientCertificateToWindowsAccount", typeof(bool), typeof(HostBehavior));
		
		public System.Security.Cryptography.X509Certificates.X509RevocationMode ClientCertificateAuthenticationRevocationMode { get { return (System.Security.Cryptography.X509Certificates.X509RevocationMode)GetValue(ClientCertificateAuthenticationRevocationModeProperty); } set { SetValue(ClientCertificateAuthenticationRevocationModeProperty, value); } }
		public static readonly DependencyProperty ClientCertificateAuthenticationRevocationModeProperty = DependencyProperty.Register("ClientCertificateAuthenticationRevocationMode", typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), typeof(HostBehavior), new PropertyMetadata(System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck));
		
		public System.Security.Cryptography.X509Certificates.StoreLocation ClientCertificateAuthenticationStoreLocation { get { return (System.Security.Cryptography.X509Certificates.StoreLocation)GetValue(ClientCertificateAuthenticationStoreLocationProperty); } set { SetValue(ClientCertificateAuthenticationStoreLocationProperty, value); } }
		public static readonly DependencyProperty ClientCertificateAuthenticationStoreLocationProperty = DependencyProperty.Register("ClientCertificateAuthenticationStoreLocation", typeof(System.Security.Cryptography.X509Certificates.StoreLocation), typeof(HostBehavior), new PropertyMetadata(System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine));

		public ObservableCollection<string> IssuedTokenAllowedAudiencesUris { get { return (ObservableCollection<string>)GetValue(IssuedTokenAllowedAudiencesUrisProperty); } set { SetValue(IssuedTokenAllowedAudiencesUrisProperty, value); } }
		public static readonly DependencyProperty IssuedTokenAllowedAudiencesUrisProperty = DependencyProperty.Register("IssuedTokenAllowedAudiencesUris", typeof(ObservableCollection<string>), typeof(HostBehavior));

		public bool IssuedTokenAllowUntrustedRsaIssuers { get { return (bool)GetValue(IssuedTokenAllowUntrustedRsaIssuersProperty); } set { SetValue(IssuedTokenAllowUntrustedRsaIssuersProperty, value); } }
		public static readonly DependencyProperty IssuedTokenAllowUntrustedRsaIssuersProperty = DependencyProperty.Register("IssuedTokenAllowUntrustedRsaIssuers", typeof(bool), typeof(HostBehavior));
		
		public System.IdentityModel.Selectors.AudienceUriMode IssuedTokenAudienceUriMode { get { return (System.IdentityModel.Selectors.AudienceUriMode)GetValue(IssuedTokenAudienceUriModeProperty); } set { SetValue(IssuedTokenAudienceUriModeProperty, value); } }
		public static readonly DependencyProperty IssuedTokenAudienceUriModeProperty = DependencyProperty.Register("IssuedTokenAudienceUriMode", typeof(System.IdentityModel.Selectors.AudienceUriMode), typeof(HostBehavior), new PropertyMetadata(System.IdentityModel.Selectors.AudienceUriMode.Never));

		public System.ServiceModel.Security.X509CertificateValidationMode IssuedTokenCertificateValidationMode { get { return (System.ServiceModel.Security.X509CertificateValidationMode)GetValue(IssuedTokenCertificateValidationModeProperty); } set { SetValue(IssuedTokenCertificateValidationModeProperty, value); } }
		public static readonly DependencyProperty IssuedTokenCertificateValidationModeProperty = DependencyProperty.Register("IssuedTokenCertificateValidationMode", typeof(System.ServiceModel.Security.X509CertificateValidationMode), typeof(HostBehavior), new PropertyMetadata(System.ServiceModel.Security.X509CertificateValidationMode.None));

		public System.Security.Cryptography.X509Certificates.X509RevocationMode IssuedTokenRevocationMode { get { return (System.Security.Cryptography.X509Certificates.X509RevocationMode)GetValue(IssuedTokenRevocationModeProperty); } set { SetValue(IssuedTokenRevocationModeProperty, value); } }
		public static readonly DependencyProperty IssuedTokenRevocationModeProperty = DependencyProperty.Register("IssuedTokenRevocationMode", typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), typeof(HostBehavior), new PropertyMetadata(System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck));

		public System.Security.Cryptography.X509Certificates.StoreLocation IssuedTokenTrustedStoreLocation { get { return (System.Security.Cryptography.X509Certificates.StoreLocation)GetValue(IssuedTokenTrustedStoreLocationProperty); } set { SetValue(IssuedTokenTrustedStoreLocationProperty, value); } }
		public static readonly DependencyProperty IssuedTokenTrustedStoreLocationProperty = DependencyProperty.Register("IssuedTokenTrustedStoreLocation", typeof(System.Security.Cryptography.X509Certificates.StoreLocation), typeof(HostBehavior), new PropertyMetadata(System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine));

		public string PeerMeshPassword { get { return (string)GetValue(PeerMeshPasswordProperty); } set { SetValue(PeerMeshPasswordProperty, value); } }
		public static readonly DependencyProperty PeerMeshPasswordProperty = DependencyProperty.Register("PeerMeshPassword", typeof(string), typeof(HostBehavior));

		public System.ServiceModel.Security.X509CertificateValidationMode PeerMessageSenderAuthenticationCertificateValidationMode { get { return (System.ServiceModel.Security.X509CertificateValidationMode)GetValue(PeerMessageSenderAuthenticationCertificateValidationModeProperty); } set { SetValue(PeerMessageSenderAuthenticationCertificateValidationModeProperty, value); } }
		public static readonly DependencyProperty PeerMessageSenderAuthenticationCertificateValidationModeProperty = DependencyProperty.Register("PeerMessageSenderAuthenticationCertificateValidationMode ", typeof(System.ServiceModel.Security.X509CertificateValidationMode), typeof(HostBehavior), new PropertyMetadata(System.ServiceModel.Security.X509CertificateValidationMode.None));

		public System.Security.Cryptography.X509Certificates.X509RevocationMode PeerMessageSenderAuthenticationRevocationMode { get { return (System.Security.Cryptography.X509Certificates.X509RevocationMode)GetValue(PeerMessageSenderAuthenticationRevocationModeProperty); } set { SetValue(PeerMessageSenderAuthenticationRevocationModeProperty, value); } }
		public static readonly DependencyProperty PeerMessageSenderAuthenticationRevocationModeProperty = DependencyProperty.Register("PeerMessageSenderAuthenticationRevocationMode", typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), typeof(HostBehavior), new PropertyMetadata(System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck));

		public System.Security.Cryptography.X509Certificates.StoreLocation PeerMessageSenderAuthenticationTrustedStoreLocation { get { return (System.Security.Cryptography.X509Certificates.StoreLocation)GetValue(PeerMessageSenderAuthenticationTrustedStoreLocationProperty); } set { SetValue(PeerMessageSenderAuthenticationTrustedStoreLocationProperty, value); } }
		public static readonly DependencyProperty PeerMessageSenderAuthenticationTrustedStoreLocationProperty = DependencyProperty.Register("PeerMessageSenderAuthenticationTrustedStoreLocation", typeof(System.Security.Cryptography.X509Certificates.StoreLocation), typeof(HostBehavior), new PropertyMetadata(System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine));

		public System.ServiceModel.Security.X509CertificateValidationMode PeerAuthenticationCertificateValidationMode { get { return (System.ServiceModel.Security.X509CertificateValidationMode)GetValue(PeerAuthenticationCertificateValidationModeProperty); } set { SetValue(PeerAuthenticationCertificateValidationModeProperty, value); } }
		public static readonly DependencyProperty PeerAuthenticationCertificateValidationModeProperty = DependencyProperty.Register("PeerAuthenticationCertificateValidationMode", typeof(System.ServiceModel.Security.X509CertificateValidationMode), typeof(HostBehavior), new PropertyMetadata(System.ServiceModel.Security.X509CertificateValidationMode.None));

		public System.Security.Cryptography.X509Certificates.X509RevocationMode PeerAuthenticationRevocationMode { get { return (System.Security.Cryptography.X509Certificates.X509RevocationMode)GetValue(PeerAuthenticationRevocationModeProperty); } set { SetValue(PeerAuthenticationRevocationModeProperty, value); } }
		public static readonly DependencyProperty PeerAuthenticationRevocationModeProperty = DependencyProperty.Register("PeerAuthenticationRevocationMode", typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), typeof(HostBehavior), new PropertyMetadata(System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck));

		public System.Security.Cryptography.X509Certificates.StoreLocation PeerAuthenticationTrustedStoreLocation { get { return (System.Security.Cryptography.X509Certificates.StoreLocation)GetValue(PeerAuthenticationTrustedStoreLocationProperty); } set { SetValue(PeerAuthenticationTrustedStoreLocationProperty, value); } }
		public static readonly DependencyProperty PeerAuthenticationTrustedStoreLocationProperty = DependencyProperty.Register("PeerAuthenticationTrustedStoreLocation", typeof(System.Security.Cryptography.X509Certificates.StoreLocation), typeof(HostBehavior), new PropertyMetadata(System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine));

		public TimeSpan UserNamePasswordCachedLogonTokenLifetime { get { return (TimeSpan)GetValue(UserNamePasswordCachedLogonTokenLifetimeProperty); } set { SetValue(UserNamePasswordCachedLogonTokenLifetimeProperty, value); } } 
		public static readonly DependencyProperty UserNamePasswordCachedLogonTokenLifetimeProperty = DependencyProperty.Register("UserNamePasswordCachedLogonTokenLifetime", typeof(TimeSpan), typeof(HostBehavior));
		
		public bool UserNamePasswordCacheLogonTokens { get { return (bool)GetValue(UserNamePasswordCacheLogonTokensProperty); } set { SetValue(UserNamePasswordCacheLogonTokensProperty, value); } }
		public static readonly DependencyProperty UserNamePasswordCacheLogonTokensProperty = DependencyProperty.Register("UserNamePasswordCacheLogonTokens", typeof(bool), typeof(HostBehavior));
		
		public bool UserNamePasswordIncludeWindowsGroups { get { return (bool)GetValue(UserNamePasswordIncludeWindowsGroupsProperty); } set { SetValue(UserNamePasswordIncludeWindowsGroupsProperty, value); } }
		public static readonly DependencyProperty UserNamePasswordIncludeWindowsGroupsProperty = DependencyProperty.Register("UserNamePasswordIncludeWindowsGroups", typeof(bool), typeof(HostBehavior));
		
		public int UserNamePasswordMaxCachedLogonTokens { get { return (int)GetValue(UserNamePasswordMaxCachedLogonTokensProperty); } set { SetValue(UserNamePasswordMaxCachedLogonTokensProperty, value); } }
		public static readonly DependencyProperty UserNamePasswordMaxCachedLogonTokensProperty = DependencyProperty.Register("UserNamePasswordMaxCachedLogonTokens", typeof(int), typeof(HostBehavior));
		
		public System.ServiceModel.Security.UserNamePasswordValidationMode UserNamePasswordValidationMode { get { return (System.ServiceModel.Security.UserNamePasswordValidationMode)GetValue(UserNamePasswordValidationModeProperty); } set { SetValue(UserNamePasswordValidationModeProperty, value); } }
		public static readonly DependencyProperty UserNamePasswordValidationModeProperty = DependencyProperty.Register("UserNamePasswordValidationMode", typeof(System.ServiceModel.Security.UserNamePasswordValidationMode), typeof(HostBehavior), new PropertyMetadata(System.ServiceModel.Security.UserNamePasswordValidationMode.Windows));
		
		public bool WindowsServiceAllowAnonymousLogons { get { return (bool)GetValue(WindowsServiceAllowAnonymousLogonsProperty); } set { SetValue(WindowsServiceAllowAnonymousLogonsProperty, value); } }
		public static readonly DependencyProperty WindowsServiceAllowAnonymousLogonsProperty = DependencyProperty.Register("WindowsServiceAllowAnonymousLogons", typeof(bool), typeof(HostBehavior));
		
		public bool WindowsServiceIncludeWindowsGroups { get { return (bool)GetValue(WindowsServiceIncludeWindowsGroupsProperty); } set { SetValue(WindowsServiceIncludeWindowsGroupsProperty, value); } }
		public static readonly DependencyProperty WindowsServiceIncludeWindowsGroupsProperty = DependencyProperty.Register("WindowsServiceIncludeWindowsGroups", typeof(bool), typeof(HostBehavior));

		public Host Owner { get; set; }

		public HostCredentials()
		{
			IssuedTokenAllowedAudiencesUris = new ObservableCollection<string>();
		}

		public HostCredentials(Host Owner)
		{
			IssuedTokenAllowedAudiencesUris = new ObservableCollection<string>();
			this.Owner = Owner;
		}

		public override string ToString()
		{
			return Owner.Name + "Credentials";
		}
	}

	public abstract class HostBehavior : DependencyObject
	{
		public Guid ID { get; set; }
		
		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(HostBehavior));

		public bool IsDefaultBehavior { get { return (bool)GetValue(IsDefaultBehaviorProperty); } set { SetValue(IsDefaultBehaviorProperty, value); } }
		public static readonly DependencyProperty IsDefaultBehaviorProperty = DependencyProperty.Register("IsDefaultBehavior", typeof(bool), typeof(HostBehavior), new UIPropertyMetadata(false, IsDefaultBehaviorChangedCallback));

		private static void IsDefaultBehaviorChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as HostBehavior;
			if (t == null) return;
			if (t.Parent == null) return;
			if (t.Parent.Behaviors == null) return;

			if(o.GetType() == typeof(HostDebugBehavior))
				foreach (HostDebugBehavior hb in t.Parent.Behaviors.Where(a => a.GetType() == typeof (HostDebugBehavior) && !Equals(a, o)))
					hb.IsDefaultBehavior = false;
			if (o.GetType() == typeof(HostMetadataBehavior))
				foreach (HostMetadataBehavior hb in t.Parent.Behaviors.Where(a => a.GetType() == typeof(HostMetadataBehavior) && !Equals(a, o)))
					hb.IsDefaultBehavior = false;
			if (o.GetType() == typeof(HostThrottlingBehavior))
				foreach (HostThrottlingBehavior hb in t.Parent.Behaviors.Where(a => a.GetType() == typeof(HostThrottlingBehavior) && !Equals(a, o)))
					hb.IsDefaultBehavior = false;
			if (o.GetType() == typeof(HostWebHTTPBehavior))
				foreach (HostWebHTTPBehavior hb in t.Parent.Behaviors.Where(a => a.GetType() == typeof(HostWebHTTPBehavior) && !Equals(a, o)))
					hb.IsDefaultBehavior = false;
		}

		public Host Parent { get { return (Host)GetValue(ParentProperty); } set { SetValue(ParentProperty, value); } }
		public static readonly DependencyProperty ParentProperty =  DependencyProperty.Register("Parent", typeof(Host), typeof(HostBehavior));

		public bool IsSelected { get; set; }

		public HostBehavior() { }

		public override string ToString()
		{
			return Name;
		}

		public IEnumerable<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			var results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Project || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Parent.Owner, this));
					else
						if (!string.IsNullOrEmpty(Name)) if (Name.IndexOf(Args.Search, StringComparison.CurrentCulture) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Parent.Owner, this));
				}
				else
					if (!string.IsNullOrEmpty(Name)) if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Parent.Parent.Owner, this));

				if (Args.ReplaceAll)
				{
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						else
							if (!string.IsNullOrEmpty(Name)) Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
					}
					else
						if (!string.IsNullOrEmpty(Name)) Name = Args.RegexSearch.Replace(Name, Args.Replace);
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
					if (Field == "Name")
						Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
					else
						if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace);
			}
			else
				if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
		}
	}

	public class HostDebugBehavior : HostBehavior
	{
		public ServiceBinding HttpHelpPageBinding { get { return (ServiceBinding)GetValue(HttpHelpPageBindingProperty); } set { SetValue(HttpHelpPageBindingProperty, value); } }
		public static readonly DependencyProperty HttpHelpPageBindingProperty = DependencyProperty.Register("HttpHelpPageBinding", typeof(ServiceBinding), typeof(HostDebugBehavior));

		public bool HttpHelpPageEnabled { get { return (bool)GetValue(HttpHelpPageEnabledProperty); } set { SetValue(HttpHelpPageEnabledProperty, value); } }
		public static readonly DependencyProperty HttpHelpPageEnabledProperty = DependencyProperty.Register("HttpHelpPageEnabled", typeof(bool), typeof(HostDebugBehavior));
		
		public string HttpHelpPageUrl { get { return (string)GetValue(HttpHelpPageUrlProperty); } set { SetValue(HttpHelpPageUrlProperty, value); } }
		public static readonly DependencyProperty HttpHelpPageUrlProperty = DependencyProperty.Register("HttpHelpPageUrl", typeof(string), typeof(HostDebugBehavior));
		
		public ServiceBinding HttpsHelpPageBinding { get { return (ServiceBinding)GetValue(HttpsHelpPageBindingProperty); } set { SetValue(HttpsHelpPageBindingProperty, value); } }
		public static readonly DependencyProperty HttpsHelpPageBindingProperty = DependencyProperty.Register("HttpsHelpPageBinding", typeof(ServiceBinding), typeof(HostDebugBehavior));
		
		public bool HttpsHelpPageEnabled { get { return (bool)GetValue(HttpsHelpPageEnabledProperty); } set { SetValue(HttpsHelpPageEnabledProperty, value); } }
		public static readonly DependencyProperty HttpsHelpPageEnabledProperty = DependencyProperty.Register("HttpsHelpPageEnabled", typeof(bool), typeof(HostDebugBehavior));
		
		public string HttpsHelpPageUrl { get { return (string)GetValue(HttpsHelpPageUrlProperty); } set { SetValue(HttpsHelpPageUrlProperty, value); } }
		public static readonly DependencyProperty HttpsHelpPageUrlProperty = DependencyProperty.Register("HttpsHelpPageUrl", typeof(string), typeof(HostDebugBehavior));
		
		public bool IncludeExceptionDetailInFaults { get { return (bool)GetValue(IncludeExceptionDetailInFaultsProperty); } set { SetValue(IncludeExceptionDetailInFaultsProperty, value); } }
		public static readonly DependencyProperty IncludeExceptionDetailInFaultsProperty = DependencyProperty.Register("IncludeExceptionDetailInFaults", typeof(bool), typeof(HostDebugBehavior));
				
		public HostDebugBehavior()
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

		public HostDebugBehavior(string Name, Host Parent)
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

	public class HostMetadataBehavior : HostBehavior
	{
		public string ExternalMetadataLocation { get { return (string)GetValue(ExternalMetadataLocationProperty); } set { SetValue(ExternalMetadataLocationProperty, value); } }
		public static readonly DependencyProperty ExternalMetadataLocationProperty =  DependencyProperty.Register("ExternalMetadataLocation", typeof(string), typeof(HostMetadataBehavior));

		public ServiceBinding HttpGetBinding { get { return (ServiceBinding)GetValue(HttpGetBindingProperty); } set { SetValue(HttpGetBindingProperty, value); } }
		public static readonly DependencyProperty HttpGetBindingProperty = DependencyProperty.Register("HttpGetBinding", typeof(ServiceBinding), typeof(HostMetadataBehavior));

		public bool HttpGetEnabled { get { return (bool)GetValue(HttpGetEnabledProperty); } set { SetValue(HttpGetEnabledProperty, value); } }
		public static readonly DependencyProperty HttpGetEnabledProperty = DependencyProperty.Register("HttpGetEnabled", typeof(bool), typeof(HostMetadataBehavior), new PropertyMetadata(false, HttpGetEnabledChangedCallback));

		private static void HttpGetEnabledChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as HostMetadataBehavior;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue)) return;
			t.HttpGetBinding = null;
			t.HttpGetUrl = "";
		}

		public string HttpGetUrl { get { return (string)GetValue(HttpGetUrlProperty); } set { SetValue(HttpGetUrlProperty, value); } }
		public static readonly DependencyProperty HttpGetUrlProperty = DependencyProperty.Register("HttpGetUrl", typeof(string), typeof(HostMetadataBehavior));
		
		public ServiceBinding HttpsGetBinding { get { return (ServiceBinding)GetValue(HttpsGetBindingProperty); } set { SetValue(HttpsGetBindingProperty, value); } }
		public static readonly DependencyProperty HttpsGetBindingProperty = DependencyProperty.Register("HttpsGetBinding", typeof(ServiceBinding), typeof(HostMetadataBehavior));
		
		public bool HttpsGetEnabled { get { return (bool)GetValue(HttpsGetEnabledProperty); } set { SetValue(HttpsGetEnabledProperty, value); } }
		public static readonly DependencyProperty HttpsGetEnabledProperty = DependencyProperty.Register("HttpsGetEnabled", typeof(bool), typeof(HostMetadataBehavior), new PropertyMetadata(false, HttpsGetEnabledChangedCallback));

		private static void HttpsGetEnabledChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as HostMetadataBehavior;
			if (t == null) return;

			if (Convert.ToBoolean(e.NewValue)) return;
			t.HttpsGetBinding = null;
			t.HttpsGetUrl = "";
		}
		
		public string HttpsGetUrl { get { return (string)GetValue(HttpsGetUrlProperty); } set { SetValue(HttpsGetUrlProperty, value); } }
		public static readonly DependencyProperty HttpsGetUrlProperty = DependencyProperty.Register("HttpsGetUrl", typeof(string), typeof(HostMetadataBehavior));

		public HostMetadataBehavior()
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

		public HostMetadataBehavior(string Name, Host Parent)
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

	public class HostThrottlingBehavior : HostBehavior
	{
		public int MaxConcurrentCalls { get { return (int)GetValue(MaxConcurrentCallsProperty); } set { SetValue(MaxConcurrentCallsProperty, value); } }
		public static readonly DependencyProperty MaxConcurrentCallsProperty =  DependencyProperty.Register("MaxConcurrentCalls", typeof(int), typeof(HostThrottlingBehavior));
		
		public int MaxConcurrentInstances { get { return (int)GetValue(MaxConcurrentInstancesProperty); } set { SetValue(MaxConcurrentInstancesProperty, value); } }
		public static readonly DependencyProperty MaxConcurrentInstancesProperty = DependencyProperty.Register("MaxConcurrentInstances", typeof(int), typeof(HostThrottlingBehavior));

		public int MaxConcurrentSessions { get { return (int)GetValue(MaxConcurrentSessionsProperty); } set { SetValue(MaxConcurrentSessionsProperty, value); } }
		public static readonly DependencyProperty MaxConcurrentSessionsProperty = DependencyProperty.Register("MaxConcurrentSessions", typeof(int), typeof(HostThrottlingBehavior));

		public HostThrottlingBehavior()
		{
			ID = Guid.NewGuid();

			MaxConcurrentCalls = 16;
			MaxConcurrentInstances = 26;
			MaxConcurrentSessions = 10;
		}

		public HostThrottlingBehavior(string Name, Host Parent)
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

	public class HostWebHTTPBehavior : HostBehavior
	{
		public bool AutomaticFormatSelectionEnabled { get { return (bool)GetValue(AutomaticFormatSelectionEnabledProperty); } set { SetValue(AutomaticFormatSelectionEnabledProperty, value); } }
		public static readonly DependencyProperty AutomaticFormatSelectionEnabledProperty = DependencyProperty.Register("AutomaticFormatSelectionEnabled", typeof(bool), typeof(HostWebHTTPBehavior), new PropertyMetadata(false));

		public System.ServiceModel.Web.WebMessageBodyStyle DefaultBodyStyle { get { return (System.ServiceModel.Web.WebMessageBodyStyle)GetValue(DefaultBodyStyleProperty); } set { SetValue(DefaultBodyStyleProperty, value); } }
		public static readonly DependencyProperty DefaultBodyStyleProperty = DependencyProperty.Register("DefaultBodyStyle", typeof(System.ServiceModel.Web.WebMessageBodyStyle), typeof(HostWebHTTPBehavior), new PropertyMetadata(System.ServiceModel.Web.WebMessageBodyStyle.Bare));

		public System.ServiceModel.Web.WebMessageFormat DefaultOutgoingRequestFormat { get { return (System.ServiceModel.Web.WebMessageFormat)GetValue(DefaultOutgoingRequestFormatProperty); } set { SetValue(DefaultOutgoingRequestFormatProperty, value); } }
		public static readonly DependencyProperty DefaultOutgoingRequestFormatProperty = DependencyProperty.Register("DefaultOutgoingRequestFormat", typeof(System.ServiceModel.Web.WebMessageFormat), typeof(HostWebHTTPBehavior), new PropertyMetadata(System.ServiceModel.Web.WebMessageFormat.Xml));

		public System.ServiceModel.Web.WebMessageFormat DefaultOutgoingResponseFormat { get { return (System.ServiceModel.Web.WebMessageFormat)GetValue(DefaultOutgoingResponseFormatProperty); } set { SetValue(DefaultOutgoingResponseFormatProperty, value); } }
		public static readonly DependencyProperty DefaultOutgoingResponseFormatProperty = DependencyProperty.Register("DefaultOutgoingResponseFormat", typeof(System.ServiceModel.Web.WebMessageFormat), typeof(HostWebHTTPBehavior), new PropertyMetadata(System.ServiceModel.Web.WebMessageFormat.Xml));

		public bool FaultExceptionEnabled { get { return (bool)GetValue(FaultExceptionEnabledProperty); } set { SetValue(FaultExceptionEnabledProperty, value); } }
		public static readonly DependencyProperty FaultExceptionEnabledProperty = DependencyProperty.Register("FaultExceptionEnabled", typeof(bool), typeof(HostWebHTTPBehavior), new PropertyMetadata(false));

		public bool HelpEnabled { get { return (bool)GetValue(HelpEnabledProperty); } set { SetValue(HelpEnabledProperty, value); } }
		public static readonly DependencyProperty HelpEnabledProperty = DependencyProperty.Register("HelpEnabled", typeof(bool), typeof(HostWebHTTPBehavior), new PropertyMetadata(false));

		public HostWebHTTPBehavior()
		{
			ID = Guid.NewGuid();
		}

		public HostWebHTTPBehavior(string Name, Host Parent)
		{
			ID = Guid.NewGuid();
			var r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;
		}
	}
}