﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace System.ServiceModel
{
	public abstract class ServerBase<T> where T : ServerBase<T>
	{
		public Guid ClientID { get; protected set; }
		public bool IsTerminated { get; protected set; }
		public T ServerInstance { get; private set; }

		public ServerBase()
		{
			ClientID = Guid.NewGuid();
			ServerInstance = this as T;
		}

		protected virtual bool Initialize()
		{
			OperationContext.Current.Channel.Faulted += ChannelFaulted;
			OperationContext.Current.Channel.Closing += ChannelClosed;

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
	}

	public abstract class ServerDuplexBase<T, TCallback, TInterface> : ServerBase<T>
		where T : ServerDuplexBase<T, TCallback, TInterface>
		where TCallback : ServerCallbackBase<TInterface>, new()
		where TInterface : class
	{
		private static DeltaDictionary<Guid, T> Clients { get; set; }

		static ServerDuplexBase()
		{
			Clients = new DeltaDictionary<Guid, T>();
		}

		public static CType GetClient<CType>(Guid ClientID) where CType : ServerDuplexBase<T, TCallback, TInterface>
		{
			T v;
			Clients.TryGetValue(ClientID, out v);
			return v as CType;
		}

		public static List<CType> GetClientList<CType>() where CType : ServerDuplexBase<T, TCallback, TInterface>
		{
			return Clients.Values.Select(t => t as CType).Where(t => t != null).ToList();
		}

		protected TCallback Callback { get; private set; }
		public ushort MaxReconnectionAttempts { get; private set; }
		internal Func<bool> Disconnected { get; set; }
		internal Action Reconnected { get; set; }

		public ServerDuplexBase()
		{
			MaxReconnectionAttempts = 0;
		}

		public ServerDuplexBase(ushort MaxReconnectionAttempts, Func<bool> Disconnected = null, Action Reconnected = null)
		{
			this.MaxReconnectionAttempts = MaxReconnectionAttempts;
			this.Disconnected = Disconnected ?? (() => false);
			this.Reconnected = Reconnected ?? (() => { });
		}

		protected virtual bool Initialize(Func<Guid> ClientID = null)
		{
			try
			{
				Callback = new TCallback { MaxReconnectionAttempts = MaxReconnectionAttempts, Disconnected = CallbackDisconnected, Reconnected = CallbackReconnected };

				base.Initialize();

				this.ClientID = ClientID != null ? ClientID() : Guid.NewGuid();
				Callback.ClientID = this.ClientID;

				Clients.AddOrUpdate(this.ClientID, this as T, (k, v) => this as T);
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		protected override void Terminate()
		{
			T t;
			Clients.TryRemove(ClientID, out t);
			base.Terminate();
		}

		protected override void ChannelClosed(object sender, EventArgs e)
		{
			Terminate();
		}

		protected override void ChannelFaulted(object sender, EventArgs e)
		{
			ChannelClosed(sender, e);
		}

		private void CallbackDisconnected()
		{
			if (Disconnected())
				Terminate();
		}

		private void CallbackReconnected()
		{
			if(!IsTerminated)
				Reconnected();
		}
	}

	public abstract class ServerCallbackBase<TCallback> where TCallback : class
	{
		public ushort MaxReconnectionAttempts { get; internal set; }
		public Guid ClientID { get; internal set; }
		internal Action Disconnected { get; set; }
		internal Action Reconnected { get; set; }

		protected TCallback __callback;
		public TCallback Callback { get { return __callback; } private set { __callback = value; } }
		public bool IsCallbackConnected { get { return __callback != null; } }

		public ServerCallbackBase()
		{
			MaxReconnectionAttempts = 0;
		}

		public bool Reconnect()
		{
			Threading.Interlocked.Exchange(ref __callback, null);
			if (MaxReconnectionAttempts == 0) return false;
			Exception stored = null;
			for (int i = 0; i < MaxReconnectionAttempts; i++)
				try { Threading.Interlocked.CompareExchange(ref __callback, OperationContext.Current.GetCallbackChannel<TCallback>(), null); }
				catch (Exception ex) { stored = ex; }
			if (stored != null)
			{
				Disconnected();
				throw stored;
			}
			Reconnected();
			return IsCallbackConnected;
		}

		public bool Reconnect(ushort MaxReconnectionAttempts)
		{
			Threading.Interlocked.Exchange(ref __callback, null);
			if (MaxReconnectionAttempts == 0) return false;
			Exception stored = null;
			for (int i = 0; i < MaxReconnectionAttempts; i++)
				try { Threading.Interlocked.CompareExchange(ref __callback, OperationContext.Current.GetCallbackChannel<TCallback>(), null); }
				catch (Exception ex) { stored = ex; }
			if (stored != null)
			{
				Disconnected();
				throw stored;
			}
			Reconnected();
			return IsCallbackConnected;
		}

		protected class InvokeAsyncCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
		{
			public object[] Results { get; set; }

			public InvokeAsyncCompletedEventArgs(object[] results, System.Exception error, bool cancelled, Object userState) : base(error, cancelled, userState)
			{
				Results = results;
			}
		}

		protected delegate IAsyncResult BeginOperationDelegate(object[] inValues, AsyncCallback asyncCallback, Object state);
		protected delegate object[] EndOperationDelegate(IAsyncResult result);

		protected void InvokeAsync(BeginOperationDelegate beginOperationDelegate, object[] inValues, EndOperationDelegate endOperationDelegate, System.Threading.SendOrPostCallback operationCompletedCallback, object userState)
		{
			if (beginOperationDelegate == null) throw new ArgumentNullException("Argument 'beginOperationDelegate' cannot be null.");
			if (endOperationDelegate == null) throw new ArgumentNullException("Argument 'endOperationDelegate' cannot be null.");
			AsyncCallback cb = delegate(IAsyncResult ar)
			{
				object[] results = null;
				Exception error = null;
				try
				{
					results = endOperationDelegate(ar);
				}
				catch (Exception ex)
				{
					error = ex;
				}
				if (operationCompletedCallback != null)
					operationCompletedCallback(new InvokeAsyncCompletedEventArgs(results, error, false, userState));
			};
			Task.Factory.StartNew(() => beginOperationDelegate(inValues, cb, userState), TaskCreationOptions.PreferFairness);
		}
	}
}