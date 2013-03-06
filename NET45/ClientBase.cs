/******************************************************************************
*	Copyright 2013 Prospective Software Inc.
*	Licensed under the Apache License, Version 2.0 (the "License");
*	you may not use this file except in compliance with the License.
*	You may obtain a copy of the License at
*
*		http://www.apache.org/licenses/LICENSE-2.0
*
*	Unless required by applicable law or agreed to in writing, software
*	distributed under the License is distributed on an "AS IS" BASIS,
*	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*	See the License for the specific language governing permissions and
*	limitations under the License.
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
	public abstract class ClientBaseEx<T, TChannel> : ClientBase<TChannel> where T : ClientBaseEx<T, TChannel>, new() where TChannel : class 
	{
		public Guid ClientID { get; protected set; }
		public bool IsTerminated { get; protected set; }
		private static T current;
		protected static T Current { get { return current; } }

		public ClientBaseEx()
		{
			ClientID = Guid.NewGuid();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			System.Threading.Interlocked.Exchange(ref current, this as T);
		}
		public ClientBaseEx(string endpointConfigurationName)
			: base(endpointConfigurationName)
		{
			ClientID = Guid.NewGuid();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			System.Threading.Interlocked.Exchange(ref current, this as T);
		}
		public ClientBaseEx(string endpointConfigurationName, string remoteAddress)
			: base(endpointConfigurationName, remoteAddress)
		{
			ClientID = Guid.NewGuid();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			System.Threading.Interlocked.Exchange(ref current, this as T);
		}
		public ClientBaseEx(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress)
			: base(endpointConfigurationName, remoteAddress)
		{
			ClientID = Guid.NewGuid();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			System.Threading.Interlocked.Exchange(ref current, this as T);
		}
		public ClientBaseEx(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress)
			: base(binding, remoteAddress)
		{
			ClientID = Guid.NewGuid();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			System.Threading.Interlocked.Exchange(ref current, this as T);
		}

		protected virtual bool Initialize()
		{
			if (State != CommunicationState.Created)
				Open();

			System.Threading.Interlocked.Exchange(ref current, this as T);

			return true;
		}

		public static void Reconnect()
		{
			var t = Current;
			System.Threading.Interlocked.Exchange(ref current, null);
			t.IsTerminated = true;
			t.InnerChannel.Faulted -= t.ChannelFaulted;
			t.InnerChannel.Closing -= t.ChannelClosed;

			//Create the new Client object and copy the relevant info into it.
			var x = new T();
			x.Endpoint.Address = t.Endpoint.Address;
			foreach(var y in t.Endpoint.Behaviors) x.Endpoint.Behaviors.Add(y);
			x.Endpoint.Binding = t.Endpoint.Binding;
			x.Endpoint.Contract = t.Endpoint.Contract;
			x.Endpoint.IsSystemEndpoint = t.Endpoint.IsSystemEndpoint;
			x.Endpoint.ListenUri = t.Endpoint.ListenUri;
			x.Endpoint.ListenUriMode = t.Endpoint.ListenUriMode;
			x.Endpoint.Name = t.Endpoint.Name;
			x.ClientID = x.ClientID;
			if (t.ClientCredentials != null && x.ClientCredentials != null)
			{
				x.ClientCredentials.ClientCertificate.Certificate = t.ClientCredentials.ClientCertificate.Certificate;
				x.ClientCredentials.HttpDigest.AllowedImpersonationLevel = t.ClientCredentials.HttpDigest.AllowedImpersonationLevel;
				x.ClientCredentials.HttpDigest.ClientCredential = t.ClientCredentials.HttpDigest.ClientCredential;
				x.ClientCredentials.IssuedToken.CacheIssuedTokens = t.ClientCredentials.IssuedToken.CacheIssuedTokens;
				x.ClientCredentials.IssuedToken.DefaultKeyEntropyMode = t.ClientCredentials.IssuedToken.DefaultKeyEntropyMode;
				x.ClientCredentials.IssuedToken.IssuedTokenRenewalThresholdPercentage = t.ClientCredentials.IssuedToken.IssuedTokenRenewalThresholdPercentage;
				foreach(var z in t.ClientCredentials.IssuedToken.IssuerChannelBehaviors) x.ClientCredentials.IssuedToken.IssuerChannelBehaviors.Add(z.Key, z.Value);
				x.ClientCredentials.IssuedToken.LocalIssuerAddress = t.ClientCredentials.IssuedToken.LocalIssuerAddress;
				x.ClientCredentials.IssuedToken.LocalIssuerBinding = t.ClientCredentials.IssuedToken.LocalIssuerBinding;
				foreach(var z in t.ClientCredentials.IssuedToken.LocalIssuerChannelBehaviors) x.ClientCredentials.IssuedToken.LocalIssuerChannelBehaviors.Add(z);
				x.ClientCredentials.IssuedToken.MaxIssuedTokenCachingTime = t.ClientCredentials.IssuedToken.MaxIssuedTokenCachingTime;
				x.ClientCredentials.Peer.Certificate = t.ClientCredentials.Peer.Certificate;
				x.ClientCredentials.Peer.MeshPassword = t.ClientCredentials.Peer.MeshPassword;
				x.ClientCredentials.Peer.MessageSenderAuthentication = t.ClientCredentials.Peer.MessageSenderAuthentication;
				x.ClientCredentials.Peer.PeerAuthentication = t.ClientCredentials.Peer.PeerAuthentication;
				x.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = t.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode;
				x.ClientCredentials.ServiceCertificate.Authentication.CustomCertificateValidator = t.ClientCredentials.ServiceCertificate.Authentication.CustomCertificateValidator;
				x.ClientCredentials.ServiceCertificate.Authentication.RevocationMode = t.ClientCredentials.ServiceCertificate.Authentication.RevocationMode;
				x.ClientCredentials.ServiceCertificate.Authentication.TrustedStoreLocation = t.ClientCredentials.ServiceCertificate.Authentication.TrustedStoreLocation;
				x.ClientCredentials.ServiceCertificate.DefaultCertificate = t.ClientCredentials.ServiceCertificate.DefaultCertificate;
				foreach (var z in t.ClientCredentials.ServiceCertificate.ScopedCertificates) t.ClientCredentials.ServiceCertificate.ScopedCertificates.Add(z.Key, z.Value);
				x.ClientCredentials.SupportInteractive = t.ClientCredentials.SupportInteractive;
				x.ClientCredentials.UserName.Password = t.ClientCredentials.UserName.Password;
				x.ClientCredentials.UserName.UserName = t.ClientCredentials.UserName.UserName;
				x.ClientCredentials.Windows.AllowedImpersonationLevel = t.ClientCredentials.Windows.AllowedImpersonationLevel;
				x.ClientCredentials.Windows.ClientCredential = t.ClientCredentials.Windows.ClientCredential;
			}

			x.InnerChannel.Faulted += x.ChannelFaulted;
			x.InnerChannel.Closing += x.ChannelClosed;

			x.Open();

			System.Threading.Interlocked.Exchange(ref current, x);
		}

		protected virtual void Terminate()
		{
			InnerChannel.Faulted -= ChannelFaulted;
			InnerChannel.Closing -= ChannelClosed;

			if (State != CommunicationState.Closed && State != CommunicationState.Closing)
				Close();

			System.Threading.Interlocked.Exchange(ref current, null);

			IsTerminated = true;
		}

		protected virtual void ChannelClosed(object sender, EventArgs e)
		{
			Terminate();
		}

		protected virtual void ChannelFaulted(object sender, EventArgs e)
		{
			Abort();
			ChannelClosed(sender, e);
		}
	}

	public abstract class ClientDuplexBaseEx<T, TChannel> : DuplexClientBase<TChannel> where T : ClientDuplexBaseEx<T, TChannel>, new() where TChannel : class
	{
		public Guid ClientID { get; protected set; }
		public bool IsTerminated { get; protected set; }
		private static T current;
		protected static T Current { get { return current; } }

		protected ClientDuplexBaseEx() : this(new ServiceEndpoint(new ContractDescription("empty"), new BasicHttpBinding(), new EndpointAddress("http://127.0.0.1")))
		{
			ClientID = Guid.NewGuid();

			var t = Current;
			if (t != null)
			{
				Endpoint.Address = t.Endpoint.Address;
				foreach (var y in t.Endpoint.Behaviors) Endpoint.Behaviors.Add(y);
				Endpoint.Binding = t.Endpoint.Binding;
				Endpoint.Contract = t.Endpoint.Contract;
				Endpoint.IsSystemEndpoint = t.Endpoint.IsSystemEndpoint;
				Endpoint.ListenUri = t.Endpoint.ListenUri;
				Endpoint.ListenUriMode = t.Endpoint.ListenUriMode;
				Endpoint.Name = t.Endpoint.Name;
				ClientID = ClientID;
				if (t.ClientCredentials != null && ClientCredentials != null)
				{
					ClientCredentials.ClientCertificate.Certificate = t.ClientCredentials.ClientCertificate.Certificate;
					ClientCredentials.HttpDigest.AllowedImpersonationLevel = t.ClientCredentials.HttpDigest.AllowedImpersonationLevel;
					ClientCredentials.HttpDigest.ClientCredential = t.ClientCredentials.HttpDigest.ClientCredential;
					ClientCredentials.IssuedToken.CacheIssuedTokens = t.ClientCredentials.IssuedToken.CacheIssuedTokens;
					ClientCredentials.IssuedToken.DefaultKeyEntropyMode = t.ClientCredentials.IssuedToken.DefaultKeyEntropyMode;
					ClientCredentials.IssuedToken.IssuedTokenRenewalThresholdPercentage =
						t.ClientCredentials.IssuedToken.IssuedTokenRenewalThresholdPercentage;
					foreach (var z in t.ClientCredentials.IssuedToken.IssuerChannelBehaviors)
						ClientCredentials.IssuedToken.IssuerChannelBehaviors.Add(z.Key, z.Value);
					ClientCredentials.IssuedToken.LocalIssuerAddress = t.ClientCredentials.IssuedToken.LocalIssuerAddress;
					ClientCredentials.IssuedToken.LocalIssuerBinding = t.ClientCredentials.IssuedToken.LocalIssuerBinding;
					foreach (var z in t.ClientCredentials.IssuedToken.LocalIssuerChannelBehaviors)
						ClientCredentials.IssuedToken.LocalIssuerChannelBehaviors.Add(z);
					ClientCredentials.IssuedToken.MaxIssuedTokenCachingTime =
						t.ClientCredentials.IssuedToken.MaxIssuedTokenCachingTime;
					ClientCredentials.Peer.Certificate = t.ClientCredentials.Peer.Certificate;
					ClientCredentials.Peer.MeshPassword = t.ClientCredentials.Peer.MeshPassword;
					ClientCredentials.Peer.MessageSenderAuthentication = t.ClientCredentials.Peer.MessageSenderAuthentication;
					ClientCredentials.Peer.PeerAuthentication = t.ClientCredentials.Peer.PeerAuthentication;
					ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode =
						t.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode;
					ClientCredentials.ServiceCertificate.Authentication.CustomCertificateValidator =
						t.ClientCredentials.ServiceCertificate.Authentication.CustomCertificateValidator;
					ClientCredentials.ServiceCertificate.Authentication.RevocationMode =
						t.ClientCredentials.ServiceCertificate.Authentication.RevocationMode;
					ClientCredentials.ServiceCertificate.Authentication.TrustedStoreLocation =
						t.ClientCredentials.ServiceCertificate.Authentication.TrustedStoreLocation;
					ClientCredentials.ServiceCertificate.DefaultCertificate =
						t.ClientCredentials.ServiceCertificate.DefaultCertificate;
					foreach (var z in t.ClientCredentials.ServiceCertificate.ScopedCertificates)
						t.ClientCredentials.ServiceCertificate.ScopedCertificates.Add(z.Key, z.Value);
					ClientCredentials.SupportInteractive = t.ClientCredentials.SupportInteractive;
					ClientCredentials.UserName.Password = t.ClientCredentials.UserName.Password;
					ClientCredentials.UserName.UserName = t.ClientCredentials.UserName.UserName;
					ClientCredentials.Windows.AllowedImpersonationLevel = t.ClientCredentials.Windows.AllowedImpersonationLevel;
					ClientCredentials.Windows.ClientCredential = t.ClientCredentials.Windows.ClientCredential;
				}
			}

			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;

			System.Threading.Interlocked.Exchange(ref current, this as T);
		}

		public ClientDuplexBaseEx(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
			ClientID = Guid.NewGuid();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			System.Threading.Interlocked.Exchange(ref current, this as T);
		}
		public ClientDuplexBaseEx(InstanceContext callbackInstance) : base(callbackInstance)
		{
			ClientID = Guid.NewGuid();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			System.Threading.Interlocked.Exchange(ref current, this as T);
		}
		public ClientDuplexBaseEx(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
		{
			ClientID = Guid.NewGuid();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			System.Threading.Interlocked.Exchange(ref current, this as T);
		}

		public ClientDuplexBaseEx(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
		{
			ClientID = Guid.NewGuid();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			System.Threading.Interlocked.Exchange(ref current, this as T);
		}
		public ClientDuplexBaseEx(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
		{
			ClientID = Guid.NewGuid();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			System.Threading.Interlocked.Exchange(ref current, this as T);
		}
		public ClientDuplexBaseEx(InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
		{
			ClientID = Guid.NewGuid();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			System.Threading.Interlocked.Exchange(ref current, this as T);
		}
		public ClientDuplexBaseEx(System.ServiceModel.Description.ServiceEndpoint endpoint) : base(endpoint)
		{
			ClientID = Guid.NewGuid();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			System.Threading.Interlocked.Exchange(ref current, this as T);
		}
		public ClientDuplexBaseEx(InstanceContext callbackInstance, System.ServiceModel.Description.ServiceEndpoint endpoint) : base(callbackInstance, endpoint)
		{
			ClientID = Guid.NewGuid();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			System.Threading.Interlocked.Exchange(ref current, this as T);
		}

		protected virtual bool Initialize()
		{
			if (State != CommunicationState.Created)
				Open();

			System.Threading.Interlocked.Exchange(ref current, this as T);

			return true;
		}

		public static void Reconnect()
		{
			var t = Current;
			System.Threading.Interlocked.Exchange(ref current, null);
			t.IsTerminated = true;
			t.InnerChannel.Faulted -= t.ChannelFaulted;
			t.InnerChannel.Closing -= t.ChannelClosed;

			var x = new T();
			x.Initialize();
		}

		protected virtual void Terminate()
		{
			InnerChannel.Faulted -= ChannelFaulted;
			InnerChannel.Closing -= ChannelClosed;

			if (State != CommunicationState.Closed && State != CommunicationState.Closing)
				Close();

			System.Threading.Interlocked.Exchange(ref current, null);

			IsTerminated = true;
		}

		protected virtual void ChannelClosed(object sender, EventArgs e)
		{
			Terminate();
		}

		protected virtual void ChannelFaulted(object sender, EventArgs e)
		{
			Abort();
			ChannelClosed(sender, e);
		}
	}
}