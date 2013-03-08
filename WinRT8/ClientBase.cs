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
				InnerChannel.Open();

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
			x.Endpoint.Binding = t.Endpoint.Binding;
			x.Endpoint.Contract = t.Endpoint.Contract;
			x.Endpoint.Name = t.Endpoint.Name;
			x.ClientID = x.ClientID;
			if (t.ClientCredentials != null && x.ClientCredentials != null)
			{
				x.ClientCredentials.UserName.Password = t.ClientCredentials.UserName.Password;
				x.ClientCredentials.UserName.UserName = t.ClientCredentials.UserName.UserName;
				x.ClientCredentials.Windows.AllowedImpersonationLevel = t.ClientCredentials.Windows.AllowedImpersonationLevel;
				x.ClientCredentials.Windows.ClientCredential = t.ClientCredentials.Windows.ClientCredential;
			}

			x.InnerChannel.Faulted += x.ChannelFaulted;
			x.InnerChannel.Closing += x.ChannelClosed;

			x.InnerChannel.Open();

			System.Threading.Interlocked.Exchange(ref current, x);
		}

		protected virtual void Terminate()
		{
			InnerChannel.Faulted -= ChannelFaulted;
			InnerChannel.Closing -= ChannelClosed;

			if (State != CommunicationState.Closed && State != CommunicationState.Closing)
				InnerChannel.Close();

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

	public abstract class ClientDuplexBaseEx<T, TChannel, TCallback> : DuplexClientBase<TChannel> where T : ClientDuplexBaseEx<T, TChannel, TCallback>, new() where TChannel : class where TCallback : class, new()
	{
		public Guid ClientID { get; protected set; }
		public bool IsTerminated { get; protected set; }
		private static T current;
		protected static T Current { get { return current; } }

		protected ClientDuplexBaseEx() : base(new InstanceContext(new TCallback()))
		{
			ClientID = Guid.NewGuid();

			var t = Current;
			if (t != null)
			{
				
				Endpoint.Address = t.Endpoint.Address;
				Endpoint.Binding = t.Endpoint.Binding;
				Endpoint.Contract = t.Endpoint.Contract;
				Endpoint.Name = t.Endpoint.Name;
				ClientID = ClientID;
				if (t.ClientCredentials != null && ClientCredentials != null)
				{
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

		public ClientDuplexBaseEx(string endpointConfigurationName) : base(new InstanceContext(new TCallback()), endpointConfigurationName)
		{
			ClientID = Guid.NewGuid();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			System.Threading.Interlocked.Exchange(ref current, this as T);
		}

		public ClientDuplexBaseEx(string endpointConfigurationName, string remoteAddress) : base(new InstanceContext(new TCallback()), endpointConfigurationName, remoteAddress)
		{
			ClientID = Guid.NewGuid();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			System.Threading.Interlocked.Exchange(ref current, this as T);
		}
		public ClientDuplexBaseEx(string endpointConfigurationName, EndpointAddress remoteAddress) : base(new InstanceContext(new TCallback()), endpointConfigurationName, remoteAddress)
		{
			ClientID = Guid.NewGuid();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			System.Threading.Interlocked.Exchange(ref current, this as T);
		}
		public ClientDuplexBaseEx(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress) : base(new InstanceContext(new TCallback()), binding, remoteAddress)
		{
			ClientID = Guid.NewGuid();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			System.Threading.Interlocked.Exchange(ref current, this as T);
		}

		protected virtual bool Initialize()
		{
			if (State != CommunicationState.Created)
				InnerChannel.Open();

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

			x.InnerChannel.Open();

			System.Threading.Interlocked.Exchange(ref current, x);
		}

		protected virtual void Terminate()
		{
			InnerChannel.Faulted -= ChannelFaulted;
			InnerChannel.Closing -= ChannelClosed;

			if (State != CommunicationState.Closed && State != CommunicationState.Closing)
				InnerChannel.Close();

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