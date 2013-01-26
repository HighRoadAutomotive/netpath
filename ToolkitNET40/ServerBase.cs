using System;
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

		public ServerBase()
		{
			ClientID = Guid.NewGuid();
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

			OperationContext.Current.Channel.Close();
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

	public abstract class ServerDuplexBase<T, TCallback, TInterface> : ServerBase<T> where T : ServerDuplexBase<T, TCallback, TInterface> where TCallback : ServerCallackBase<TInterface>, new() where TInterface : class
	{
		private static DeltaDictionary<Guid, T> Clients { get; set; }

		static ServerDuplexBase()
		{
			Clients = new DeltaDictionary<Guid, T>();
		}

		public static T GetClient(Guid ClientID)
		{
			T v;
			Clients.TryGetValue(ClientID, out v);
			return v;
		}

		public static List<T> GetClientList()
		{
			return new List<T>(Clients.Values);
		}

		protected TCallback Callback { get; private set; }
		public ushort MaxReconnectionAttempts { get; private set; }

		public ServerDuplexBase()
		{
			MaxReconnectionAttempts = 0;
		}

		public ServerDuplexBase(ushort MaxReconnectionAttempts)
		{
			this.MaxReconnectionAttempts = MaxReconnectionAttempts;
		}

		protected virtual bool Initialize(Func<Guid> ClientID = null)
		{
			try
			{
				Callback = new TCallback {MaxReconnectionAttempts = MaxReconnectionAttempts};

				base.Initialize();

				this.ClientID = ClientID != null ? ClientID() : Guid.NewGuid();

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
	}

	public abstract class ServerCallackBase<TCallback> where TCallback : class
	{
		public ushort MaxReconnectionAttempts { get; internal set; }

		protected TCallback __callback;
		public TCallback Callback { get { return __callback; } private set { __callback = value; } }

		public ServerCallackBase() { }

		public bool Reconnect()
		{
			try
			{
				__callback = null;
				if (MaxReconnectionAttempts < 0) return false;
				else if (MaxReconnectionAttempts == 0)
					while (__callback == null)
						__callback = OperationContext.Current.GetCallbackChannel<TCallback>();
				else
					for (int i = 0; i < MaxReconnectionAttempts; i++)
						__callback = OperationContext.Current.GetCallbackChannel<TCallback>();
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		protected new class InvokeAsyncCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
		{
			public object[] Results { get; set; }

			public InvokeAsyncCompletedEventArgs(object[] results, System.Exception error, bool cancelled, Object userState) : base(error, cancelled, userState)
			{
				Results = results;
			}
		}

		protected new delegate IAsyncResult BeginOperationDelegate(object[] inValues, AsyncCallback asyncCallback, Object state);
		protected new delegate object[] EndOperationDelegate(IAsyncResult result);

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