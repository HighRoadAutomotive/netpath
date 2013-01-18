using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace System.ServiceModel
{
	public abstract class DuplexController<T, TCallback> where T : DuplexController<T, TCallback>
	{
		private static DependencyDictionary<Guid, T> Clients { get; set; }

		public Guid ClientToken { get; private set; }
		protected TCallback Callback { get; private set; }

		static DuplexController()
		{
			Clients = new DependencyDictionary<Guid, T>();
		}

		protected virtual bool OpenClient(Func<Guid> ClientToken)
		{
			this.ClientToken = ClientToken();

			try
			{
				Callback = OperationContext.Current.GetCallbackChannel<TCallback>();
				OperationContext.Current.Channel.Faulted += SessionFaulted;
				OperationContext.Current.Channel.Closing += SessionClosed;
			}
			catch (Exception ex)
			{
				return false;
			}

			return Clients.TryAdd(this.ClientToken, this as T);
		}

		protected virtual void CloseClient()
		{
			T t = null;
			Clients.TryRemove(ClientToken, out t);
			OperationContext.Current.Channel.Faulted -= SessionFaulted;
			OperationContext.Current.Channel.Closing -= SessionClosed;
		}

		protected virtual void SessionClosed(object sender, EventArgs e)
		{
			CloseClient();
		}

		protected virtual void SessionFaulted(object sender, EventArgs e)
		{
			SessionClosed(sender, e);
		}

		public static DuplexController<T, TCallback> GetClient(Guid ClientID)
		{
			T v = null;
			Clients.TryGetValue(ClientID, out v);
			return v;
		}
	}
}