using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace System.ServiceModel
{
	public abstract class Controller<T> where T : Controller<T>
	{
		protected virtual bool BeginClient()
		{
			try
			{
				OperationContext.Current.Channel.Faulted += ClientFaulted;
				OperationContext.Current.Channel.Closing += ClientClosing;
			}
			catch (Exception ex)
			{
				return false;
			}

			return true;
		}

		protected virtual void EndClient()
		{
			OperationContext.Current.Channel.Faulted -= ClientFaulted;
			OperationContext.Current.Channel.Closing -= ClientClosing;
		}

		protected virtual void ClientClosing(object sender, EventArgs e)
		{
			EndClient();
		}

		protected virtual void ClientFaulted(object sender, EventArgs e)
		{
			ClientClosing(sender, e);
		}
	}
}