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
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
	public abstract class ClientBaseEx<T, TChannel> : ClientBase<TChannel> where T : ClientBaseEx<T, TChannel> where TChannel : class 
	{
		public Guid ClientID { get; protected set; }
		public bool IsTerminated { get; protected set; }
		public T ClientInstance { get; private set; }

		public ClientBaseEx()
		{
			ClientID = Guid.NewGuid();
			ClientInstance = this as T;
		}
		public ClientBaseEx(string endpointConfigurationName)
			: base(endpointConfigurationName)
		{
			ClientID = Guid.NewGuid();
			ClientInstance = this as T;
		}
		public ClientBaseEx(string endpointConfigurationName, string remoteAddress)
			: base(endpointConfigurationName, remoteAddress)
		{
			ClientID = Guid.NewGuid();
			ClientInstance = this as T;
		}
		public ClientBaseEx(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress)
			: base(endpointConfigurationName, remoteAddress)
		{
			ClientID = Guid.NewGuid();
			ClientInstance = this as T;
		}
		public ClientBaseEx(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress)
			: base(binding, remoteAddress)
		{
			ClientID = Guid.NewGuid();
			ClientInstance = this as T;
		}

		protected virtual bool Initialize()
		{
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;

			if (State != CommunicationState.Created)
				Open();

			return true;
		}

		public virtual void Reconnect()
		{
			IsTerminated = true;
			ChannelFactory.CreateChannel();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;

			if (State != CommunicationState.Opened && State != CommunicationState.Opening)
				Open();

			IsTerminated = false;
		}

		protected virtual void Terminate()
		{
			InnerChannel.Faulted -= ChannelFaulted;
			InnerChannel.Closing -= ChannelClosed;

			if (State != CommunicationState.Closed && State != CommunicationState.Closing)
				Close();

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

	public abstract class ClientDuplexBaseEx<T, TChannel> : DuplexClientBase<TChannel> where T : ClientDuplexBaseEx<T, TChannel> where TChannel : class
	{
		public Guid ClientID { get; protected set; }
		public bool IsTerminated { get; protected set; }
		public T ClientInstance { get; private set; }

		public ClientDuplexBaseEx(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
			ClientID = Guid.NewGuid();
			ClientInstance = this as T;
		}
		public ClientDuplexBaseEx(InstanceContext callbackInstance) : base(callbackInstance)
		{
			ClientID = Guid.NewGuid();
			ClientInstance = this as T;
		}
		public ClientDuplexBaseEx(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
		{
			ClientID = Guid.NewGuid();
			ClientInstance = this as T;
		}

		public ClientDuplexBaseEx(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
		{
			ClientID = Guid.NewGuid();
			ClientInstance = this as T;
		}
		public ClientDuplexBaseEx(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
		{
			ClientID = Guid.NewGuid();
			ClientInstance = this as T;
		}
		public ClientDuplexBaseEx(InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
		{
			ClientID = Guid.NewGuid();
			ClientInstance = this as T;
		}
		public ClientDuplexBaseEx(System.ServiceModel.Description.ServiceEndpoint endpoint) : base(endpoint)
		{
			ClientID = Guid.NewGuid();
			ClientInstance = this as T;
		}
		public ClientDuplexBaseEx(InstanceContext callbackInstance, System.ServiceModel.Description.ServiceEndpoint endpoint) : base(callbackInstance, endpoint)
		{
			ClientID = Guid.NewGuid();
			ClientInstance = this as T;
		}

		protected virtual bool Initialize()
		{
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;

			if (State != CommunicationState.Created)
				Open();

			return true;
		}

		public virtual void Reconnect()
		{
			IsTerminated = true;
			ChannelFactory.CreateChannel();
			InnerChannel.Faulted += ChannelFaulted;
			InnerChannel.Closing += ChannelClosed;
			
			if (State != CommunicationState.Opened && State != CommunicationState.Opening)
				Open();

			IsTerminated = false;
		}

		protected virtual void Terminate()
		{
			InnerChannel.Faulted -= ChannelFaulted;
			InnerChannel.Closing -= ChannelClosed;

			if (State != CommunicationState.Closed && State != CommunicationState.Closing)
				Close();

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