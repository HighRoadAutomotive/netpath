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

	public abstract class BindingSecurity : DataType
	{
		//Internal Use - Searching / Filtering
		[IgnoreDataMember()] public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(BindingSecurity));

		[IgnoreDataMember()] public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(BindingSecurity));

		[IgnoreDataMember()] public bool IsFiltering { get { return false; } set { } }
		[IgnoreDataMember()] public bool IsFilterMatch { get { return false; } set { } }

		public bool IsTreeExpanded { get { return (bool)GetValue(IsTreeExpandedProperty); } set { SetValue(IsTreeExpandedProperty, value); } }
		public static readonly DependencyProperty IsTreeExpandedProperty = DependencyProperty.Register("IsTreeExpanded", typeof(bool), typeof(BindingSecurity), new UIPropertyMetadata(false));

		public BindingSecurity() : base(DataTypeMode.Class) { }

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Project || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
					}
				}

				if (Args.ReplaceAll == true)
				{
					bool ia = Parent.IsActive;
					Parent.IsActive = true;
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						}
					}
					Parent.IsActive = ia;
				}
			}

			return results;
		}

		public void Replace (FindReplaceInfo Args, string Field)
		{
			if (Args.ReplaceAll == true)
			{
				bool ia = Parent.IsActive;
				Parent.IsActive = true;
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
					}
				}
				Parent.IsActive = ia;
			}
		}

		public abstract BindingSecurity Copy(string HostName, Namespace Parent);
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

		public BindingSecurityBasicHTTP() : base() { }

		public BindingSecurityBasicHTTP(string Name, Namespace Parent) : base()
		{
			this.ID = Guid.NewGuid();
			InheritedTypes.Add(new DataType("System.ServiceModel.BasicHttpSecurity", DataTypeMode.Class));
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.BasicHttpSecurityMode.None;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityBasicHTTP BD = new BindingSecurityBasicHTTP(Name + HostName, Parent);
			BD.MessageAlgorithmSuite = MessageAlgorithmSuite;
			BD.MessageClientCredentialType = MessageClientCredentialType;
			BD.Mode = Mode;
			BD.TransportClientCredentialType = TransportClientCredentialType;
			BD.TransportProxyCredentialType = TransportProxyCredentialType;
			BD.TransportRealm = TransportRealm;

			Parent.Security.Add(BD);

			return BD;
		}
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

		public BindingSecurityBasicHTTPS() : base() { }

		public BindingSecurityBasicHTTPS(string Name, Namespace Parent)
			: base()
		{
			this.ID = Guid.NewGuid();
			InheritedTypes.Add(new DataType("System.ServiceModel.BasicHttpSecurity", DataTypeMode.Class));
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.BasicHttpsSecurityMode.Transport;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityBasicHTTPS BD = new BindingSecurityBasicHTTPS(Name + HostName, Parent);
			BD.MessageAlgorithmSuite = MessageAlgorithmSuite;
			BD.MessageClientCredentialType = MessageClientCredentialType;
			BD.Mode = Mode;
			BD.TransportClientCredentialType = TransportClientCredentialType;
			BD.TransportProxyCredentialType = TransportProxyCredentialType;
			BD.TransportRealm = TransportRealm;

			Parent.Security.Add(BD);

			return BD;
		}
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

		public BindingSecurityWSHTTP() : base() { }

		public BindingSecurityWSHTTP(string Name, Namespace Parent) : base()
		{
			this.ID = Guid.NewGuid();
			InheritedTypes.Add(new DataType("System.ServiceModel.WSHttpSecurity", DataTypeMode.Class));
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.SecurityMode.Message;
			this.MessageAlgorithmSuite = BindingSecurityAlgorithmSuite.Basic256;
			this.MessageClientCredentialType = System.ServiceModel.MessageCredentialType.Windows;
			this.MessageEstablishSecurityContext = true;
			this.MessageNegotiateServiceCredential = true;
			this.TransportClientCredentialType = System.ServiceModel.HttpClientCredentialType.None;
			this.TransportProxyCredentialType = System.ServiceModel.HttpProxyCredentialType.None;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityWSHTTP BD = new BindingSecurityWSHTTP(Name + HostName, Parent);
			BD.MessageAlgorithmSuite = MessageAlgorithmSuite;
			BD.MessageClientCredentialType = MessageClientCredentialType;
			BD.MessageEstablishSecurityContext = MessageEstablishSecurityContext;
			BD.MessageNegotiateServiceCredential = MessageNegotiateServiceCredential;
			BD.Mode = Mode;
			BD.TransportClientCredentialType = TransportClientCredentialType;
			BD.TransportProxyCredentialType = TransportProxyCredentialType;
			BD.TransportRealm = TransportRealm;

			Parent.Security.Add(BD);

			return BD;
		}
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

		public BindingSecurityWSDualHTTP() : base() { }

		public BindingSecurityWSDualHTTP(string Name, Namespace Parent) : base()
		{
			this.ID = Guid.NewGuid();
			InheritedTypes.Add(new DataType("System.ServiceModel.WSDualHttpSecurity", DataTypeMode.Class));
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.WSDualHttpSecurityMode.Message;
			this.MessageAlgorithmSuite = BindingSecurityAlgorithmSuite.Basic256;
			this.MessageClientCredentialType = System.ServiceModel.MessageCredentialType.Windows;
			this.MessageNegotiateServiceCredential = true;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityWSDualHTTP BD = new BindingSecurityWSDualHTTP(Name + HostName, Parent);
			BD.MessageAlgorithmSuite = MessageAlgorithmSuite;
			BD.MessageClientCredentialType = MessageClientCredentialType;
			BD.MessageNegotiateServiceCredential = MessageNegotiateServiceCredential;

			Parent.Security.Add(BD);

			return BD;
		}
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

		public BindingSecurityWSFederationHTTP() : base() { }

		public BindingSecurityWSFederationHTTP(string Name, Namespace Parent) : base()
		{
			this.ID = Guid.NewGuid();
			InheritedTypes.Add(new DataType("System.ServiceModel.WSFederationHttpSecurity", DataTypeMode.Class));
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.WSFederationHttpSecurityMode.Message;
			this.MessageAlgorithmSuite = BindingSecurityAlgorithmSuite.Basic256Rsa15;
			this.MessageEstablishSecurityContext = true;
			this.MessageIssuedKeyType = System.IdentityModel.Tokens.SecurityKeyType.SymmetricKey;
			this.MessageNegotiateServiceCredential = true;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityWSFederationHTTP BD = new BindingSecurityWSFederationHTTP(Name + HostName, Parent);
			BD.MessageAlgorithmSuite = MessageAlgorithmSuite;
			BD.MessageClientCredentialType = MessageClientCredentialType;
			BD.MessageEstablishSecurityContext = MessageEstablishSecurityContext;
			BD.MessageIssuedKeyType = MessageIssuedKeyType;
			BD.MessageIssuedTokenType = MessageIssuedTokenType;
			BD.MessageIssuerAddress = MessageIssuerAddress;
			BD.MessageIssuerMetadataAddress = MessageIssuerMetadataAddress;
			BD.MessageNegotiateServiceCredential = MessageNegotiateServiceCredential;
			BD.Mode = Mode;

			Parent.Security.Add(BD);

			return BD;
		}
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
		
		public BindingSecurityTCP() : base() {}

		public BindingSecurityTCP(string Name, Namespace Parent) : base()
		{
			this.ID = Guid.NewGuid();
			InheritedTypes.Add(new DataType("System.ServiceModel.NetTcpSecurity", DataTypeMode.Class));
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.SecurityMode.Transport;
			this.TransportClientCredentialType = System.ServiceModel.TcpClientCredentialType.Windows;
			this.TransportProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityTCP BD = new BindingSecurityTCP(Name + HostName, Parent);
			BD.MessageAlgorithmSuite = MessageAlgorithmSuite;
			BD.MessageClientCredentialType = MessageClientCredentialType;
			BD.Mode = Mode;
			BD.TransportClientCredentialType = TransportClientCredentialType;
			BD.TransportProtectionLevel = TransportProtectionLevel;

			Parent.Security.Add(BD);

			return BD;
		}
	}

	#endregion

	#region - BindingSecurityNamedPipe Class -

	public class BindingSecurityNamedPipe : BindingSecurity
	{
		public System.ServiceModel.NetNamedPipeSecurityMode Mode { get { return (System.ServiceModel.NetNamedPipeSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.NetNamedPipeSecurityMode), typeof(BindingSecurityNamedPipe));

		public System.Net.Security.ProtectionLevel TransportProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(TransportProtectionLevelProperty); } set { SetValue(TransportProtectionLevelProperty, value); } }
		public static readonly DependencyProperty TransportProtectionLevelProperty = DependencyProperty.Register("TransportProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(BindingSecurityNamedPipe));

		public BindingSecurityNamedPipe() : base() { }

		public BindingSecurityNamedPipe(string Name, Namespace Parent) : base()
		{
			this.ID = Guid.NewGuid();
			InheritedTypes.Add(new DataType("System.ServiceModel.NetNamedPipeSecurity", DataTypeMode.Class));
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.NetNamedPipeSecurityMode.Transport;
			this.TransportProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityNamedPipe BD = new BindingSecurityNamedPipe(Name + HostName, Parent);
			BD.Mode = Mode;
			BD.TransportProtectionLevel = TransportProtectionLevel;

			Parent.Security.Add(BD);

			return BD;
		}
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

		public BindingSecurityMSMQ() : base() { }

		public BindingSecurityMSMQ(string Name, Namespace Parent) : base()
		{
			this.ID = Guid.NewGuid();
			InheritedTypes.Add(new DataType("System.ServiceModel.NetMsmqSecurity", DataTypeMode.Class));
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.NetMsmqSecurityMode.Transport;
			this.TransportAuthenticationMode = System.ServiceModel.MsmqAuthenticationMode.WindowsDomain;
			this.TransportEncryptionAlgorithm = System.ServiceModel.MsmqEncryptionAlgorithm.Aes;
			this.TransportProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
			this.TransportSecureHashAlgorithm = System.ServiceModel.MsmqSecureHashAlgorithm.Sha512;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityMSMQ BD = new BindingSecurityMSMQ(Name + HostName, Parent);
			BD.MessageAlgorithmSuite = MessageAlgorithmSuite;
			BD.MessageClientCredentialType = MessageClientCredentialType;
			BD.TransportAuthenticationMode = TransportAuthenticationMode;
			BD.TransportEncryptionAlgorithm = TransportEncryptionAlgorithm;
			BD.TransportProtectionLevel = TransportProtectionLevel;
			BD.TransportSecureHashAlgorithm = TransportSecureHashAlgorithm;
			BD.Mode = Mode;

			Parent.Security.Add(BD);

			return BD;
		}
	}

	#endregion

	#region - BindingSecurityPeerTCP Class -

	public class BindingSecurityPeerTCP : BindingSecurity
	{
		public System.ServiceModel.SecurityMode Mode { get { return (System.ServiceModel.SecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.SecurityMode), typeof(BindingSecurityPeerTCP));

		public System.ServiceModel.PeerTransportCredentialType TransportClientCredentialType { get { return (System.ServiceModel.PeerTransportCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static readonly DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.PeerTransportCredentialType), typeof(BindingSecurityPeerTCP));

		public BindingSecurityPeerTCP() { }

		public BindingSecurityPeerTCP(string Name, Namespace Parent)
		{
			this.ID = Guid.NewGuid();
			InheritedTypes.Add(new DataType("System.ServiceModel.PeerSecuritySettings", DataTypeMode.Class));
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.SecurityMode.Transport;
			this.TransportClientCredentialType = System.ServiceModel.PeerTransportCredentialType.Password;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityPeerTCP BD = new BindingSecurityPeerTCP(Name + HostName, Parent);
			BD.Mode = Mode;
			BD.TransportClientCredentialType = TransportClientCredentialType;

			Parent.Security.Add(BD);

			return BD;
		}
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

		public BindingSecurityWebHTTP() : base() { }

		public BindingSecurityWebHTTP(string Name, Namespace Parent) : base()
		{
			this.ID = Guid.NewGuid();
			InheritedTypes.Add(new DataType("System.ServiceModel.WebHttpSecurity", DataTypeMode.Class));
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.WebHttpSecurityMode.None;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityWebHTTP BD = new BindingSecurityWebHTTP(Name + HostName, Parent);
			BD.Mode = Mode;
			BD.TransportClientCredentialType = TransportClientCredentialType;
			BD.TransportProxyCredentialType = TransportProxyCredentialType;
			BD.TransportRealm = TransportRealm;

			Parent.Security.Add(BD);

			return BD;
		}
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

		public BindingSecurityMSMQIntegration() : base() { }

		public BindingSecurityMSMQIntegration(string Name, Namespace Parent) : base()
		{
			this.ID = Guid.NewGuid();
			InheritedTypes.Add(new DataType("System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity", DataTypeMode.Class));
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Name = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode.Transport;
			this.TransportAuthenticationMode = System.ServiceModel.MsmqAuthenticationMode.WindowsDomain;
			this.TransportEncryptionAlgorithm = System.ServiceModel.MsmqEncryptionAlgorithm.Aes;
			this.TransportProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
			this.TransportSecureHashAlgorithm = System.ServiceModel.MsmqSecureHashAlgorithm.Sha512;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityMSMQIntegration BD = new BindingSecurityMSMQIntegration(Name + HostName, Parent);
			BD.TransportAuthenticationMode = TransportAuthenticationMode;
			BD.TransportEncryptionAlgorithm = TransportEncryptionAlgorithm;
			BD.TransportProtectionLevel = TransportProtectionLevel;
			BD.TransportSecureHashAlgorithm = TransportSecureHashAlgorithm;
			BD.Mode = Mode;

			Parent.Security.Add(BD);

			return BD;
		}
	}
	#endregion
}