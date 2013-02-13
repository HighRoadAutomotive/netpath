using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NETPath.Toolkit.NET45
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
			ClientID = GetClientID();

			OperationContext.Current.Channel.Faulted += ChannelFaulted;
			OperationContext.Current.Channel.Closing += ChannelClosed;

			if (OperationContext.Current.Channel.State != CommunicationState.Created)
				OperationContext.Current.Channel.Open();

			return true;
		}

		protected virtual void Terminate()
		{
			OperationContext.Current.Channel.Faulted -= ChannelFaulted;
			OperationContext.Current.Channel.Closing -= ChannelClosed;

			if (OperationContext.Current.Channel.State != CommunicationState.Closed && OperationContext.Current.Channel.State != CommunicationState.Closing)
				OperationContext.Current.Channel.Close();

			IsTerminated = true;
		}

		protected virtual void ChannelClosed(object sender, EventArgs e)
		{
			Terminate();
		}

		protected virtual void ChannelFaulted(object sender, EventArgs e)
		{
			ChannelClosed(sender, e);
		}

		public abstract Guid GetClientID();
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
			ClientID = GetClientID();

			OperationContext.Current.Channel.Faulted += ChannelFaulted;
			OperationContext.Current.Channel.Closing += ChannelClosed;

			if (OperationContext.Current.Channel.State != CommunicationState.Created)
				OperationContext.Current.Channel.Open();

			return true;
		}

		protected virtual void Terminate()
		{
			OperationContext.Current.Channel.Faulted -= ChannelFaulted;
			OperationContext.Current.Channel.Closing -= ChannelClosed;

			if (OperationContext.Current.Channel.State != CommunicationState.Closed && OperationContext.Current.Channel.State != CommunicationState.Closing)
				OperationContext.Current.Channel.Close();

			IsTerminated = true;
		}

		protected virtual void ChannelClosed(object sender, EventArgs e)
		{
			Terminate();
		}

		protected virtual void ChannelFaulted(object sender, EventArgs e)
		{
			ChannelClosed(sender, e);
		}

		public abstract Guid GetClientID();
	}
}