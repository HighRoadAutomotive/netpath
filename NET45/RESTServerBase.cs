using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;

namespace System.ServiceModel
{
	public abstract class RESTServerBase
	{
		public WebServiceHost Host { get; protected set; }
		public WebHttpBinding Binding { get; protected set; }
		public ServiceEndpoint Endpoint { get; protected set; }
		public Uri DefaultEndpointAddress { get; protected set; }

		public ServiceDebugBehavior DebugBehavior { get; protected set; }
		public ServiceThrottlingBehavior ThrottlingBehavior { get; protected set; }
		public WebHttpBehavior WebHttpBehavior { get; protected set; }
		
		public RESTServerBase(Type ServiceType, Uri[] BaseAddresses, WebHttpSecurityMode SecurityMode)
		{
			ThrottlingBehavior = new ServiceThrottlingBehavior();
			WebHttpBehavior = new WebHttpBehavior();

			Host = new WebServiceHost(ServiceType, BaseAddresses);
			DebugBehavior = Host.Description.Behaviors.Find<ServiceDebugBehavior>();
			Host.Description.Behaviors.Add(ThrottlingBehavior);
			Binding = new WebHttpBinding(SecurityMode);
		}

		public virtual void Open()
		{
			Host.Open();
		}

		public virtual void Close()
		{
			Host.Close();
		}

		public virtual void Abort()
		{
			Host.Abort();
		}
	}
}