//---------------------------------------------------------------------------
// This code was generated by a tool. Changes to this file may cause 
// incorrect behavior and will be lost if the code is regenerated.
//
// WCF Architect Version:	1.1.1100.0
// .NET Framework Version:	4.0 (Client)
//---------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Windows;

namespace WCFArchitect.Samples.HelloWCF
{
//Data Enumerations
	[DataContract(Name = "FavoriteColor", Namespace = "http://tempuri.org/WCFArchitect/Samples/HelloWCF/")]
	public enum FavoriteColor : int
	{
		[EnumMember()] Red = 1,
		[EnumMember()] Green = 2,
		[EnumMember()] Blue = 3,
	}

//Data Contracts
	[DataContract(Name = "User", Namespace = "http://tempuri.org/WCFArchitect/Samples/HelloWCF/")]
	public partial class User
	{
		[DataMember(Name = "ID")] public Guid ID { get; set; }
		[DataMember(Name = "FirstName")] public string FirstName { get; set; }
		[DataMember(Name = "LastName")] public string LastName { get; set; }
		[DataMember(Name = "Address")] public string Address { get; set; }
		[DataMember(Name = "City")] public string City { get; set; }
		[DataMember(Name = "State")] public string State { get; set; }
		[DataMember(Name = "Zip")] public string Zip { get; set; }
		[DataMember(Name = "UserColor")] public FavoriteColor UserColor { get; set; }
	}

//Service Contracts
	[ServiceContract(CallbackContract = typeof(IUsersCallback), SessionMode = System.ServiceModel.SessionMode.Required, Name = "Users", Namespace = "http://tempuri.org/WCFArchitect/Samples/HelloWCF/")]
	public interface IUsers
	{
		User UserInfo { [OperationContract(Name = "GetUserInfo")] get;  }
		[OperationContract(Name = "SetUserInfo")] void SetUserInfo(User UserInfo);
		[OperationContract(IsInitiating = true, Name = "Connect")] Guid Connect();
		[OperationContract(IsTerminating = true, Name = "Disconnect")] void Disconnect(Guid UserID);
	}
	public interface IUsersCallback
	{
		[OperationContract(Name = "UserInfoUpdated")] void UserInfoUpdated(User NewUserInfo);
	}

}
//Server Services

//Server Service Bindings
namespace WCFArchitect.Samples.HelloWCF.Bindings
{
	public class NamedPipeBinding : NetNamedPipeBinding
	{
		public NamedPipeBinding()
		{
			SetDefaults();
		}
		public NamedPipeBinding(NetNamedPipeSecurity CustomSecurity)
		{
			SetDefaults();
			this.Security = CustomSecurity;
		}
		public NamedPipeBinding(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)
		{
			SetDefaults();
			this.ReaderQuotas = ReaderQuotas;
		}
		public NamedPipeBinding(NetNamedPipeSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)
		{
			SetDefaults();
			this.Security = CustomSecurity;
			this.ReaderQuotas = ReaderQuotas;
		}
		private void SetDefaults()
		{
			this.CloseTimeout = new TimeSpan(600000000);
			this.Name = "Named Pipe Binding";
			this.Namespace = "http://tempuri.org";
			this.OpenTimeout = new TimeSpan(600000000);
			this.ReceiveTimeout = new TimeSpan(6000000000);
			this.SendTimeout = new TimeSpan(6000000000);
			this.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
			this.MaxBufferPoolSize = 524288;
			this.MaxBufferSize = 65536;
			this.MaxConnections = 10;
			this.MaxReceivedMessageSize = 65536;
			this.TransactionFlow = true;
			this.TransactionProtocol = TransactionProtocol.Default;
			this.TransferMode = TransferMode.Buffered;
			this.Security = WCFArchitect.Samples.HelloWCF.Bindings.Security.CreateNamedPipeSecuritySecurity();
		}
	}
}
//Server Service Binding Security
namespace WCFArchitect.Samples.HelloWCF.Bindings
{
	public partial class Security
	{
		public static NetNamedPipeSecurity CreateNamedPipeSecuritySecurity()
		{
			NetNamedPipeSecurity sec = new NetNamedPipeSecurity();
			sec.Mode = NetNamedPipeSecurityMode.None;
			return sec;
		}
	}
}
//Server Hosts
namespace WCFArchitect.Samples.HelloWCF.Hosts
{
	public class PipeHost : ServiceHost
	{
		public static Uri PipeEndpointURI { get { return new Uri("net.pipe://localhost/PipeEndpoint"); } }
		public PipeHost(object singletonInstance) : base(singletonInstance)
		{
			this.Authorization.ImpersonateCallerForAllOperations = false;
			this.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.None;
			this.CloseTimeout = new TimeSpan(600000000);
			this.OpenTimeout = new TimeSpan(600000000);
			this.Description.Name = "PipeHost";
			this.Description.Namespace = "http://tempuri.org";
			this.AddServiceEndpoint(typeof(WCFArchitect.Samples.HelloWCF.IUsers), new WCFArchitect.Samples.HelloWCF.Bindings.NamedPipeBinding(), PipeEndpointURI);
		}
		public PipeHost(Type serviceType) : base(serviceType)
		{
			this.Authorization.ImpersonateCallerForAllOperations = false;
			this.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.None;
			this.CloseTimeout = new TimeSpan(600000000);
			this.OpenTimeout = new TimeSpan(600000000);
			this.Description.Name = "PipeHost";
			this.Description.Namespace = "http://tempuri.org";
			this.AddServiceEndpoint(typeof(WCFArchitect.Samples.HelloWCF.IUsers), new WCFArchitect.Samples.HelloWCF.Bindings.NamedPipeBinding(), PipeEndpointURI);
		}
		public PipeHost(object singletonInstance, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses)
		{
			this.Authorization.ImpersonateCallerForAllOperations = false;
			this.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.None;
			this.CloseTimeout = new TimeSpan(600000000);
			this.OpenTimeout = new TimeSpan(600000000);
			this.Description.Name = "PipeHost";
			this.Description.Namespace = "http://tempuri.org";
			this.AddServiceEndpoint(typeof(WCFArchitect.Samples.HelloWCF.IUsers), new WCFArchitect.Samples.HelloWCF.Bindings.NamedPipeBinding(), PipeEndpointURI);
		}
		public PipeHost(Type serviceType, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses)
		{
			this.Authorization.ImpersonateCallerForAllOperations = false;
			this.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.None;
			this.CloseTimeout = new TimeSpan(600000000);
			this.OpenTimeout = new TimeSpan(600000000);
			this.Description.Name = "PipeHost";
			this.Description.Namespace = "http://tempuri.org";
			this.AddServiceEndpoint(typeof(WCFArchitect.Samples.HelloWCF.IUsers), new WCFArchitect.Samples.HelloWCF.Bindings.NamedPipeBinding(), PipeEndpointURI);
		}
		public PipeHost(object singletonInstance, bool DisableDefaultEndpoints) : base(singletonInstance)
		{
			this.Authorization.ImpersonateCallerForAllOperations = false;
			this.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.None;
			this.CloseTimeout = new TimeSpan(600000000);
			this.OpenTimeout = new TimeSpan(600000000);
			this.Description.Name = "PipeHost";
			this.Description.Namespace = "http://tempuri.org";
			if(DisableDefaultEndpoints == false)
			{
				this.AddServiceEndpoint(typeof(WCFArchitect.Samples.HelloWCF.IUsers), new WCFArchitect.Samples.HelloWCF.Bindings.NamedPipeBinding(), PipeEndpointURI);
			}
		}
		public PipeHost(Type serviceType, bool DisableDefaultEndpoints) : base(serviceType)
		{
			this.Authorization.ImpersonateCallerForAllOperations = false;
			this.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.None;
			this.CloseTimeout = new TimeSpan(600000000);
			this.OpenTimeout = new TimeSpan(600000000);
			this.Description.Name = "PipeHost";
			this.Description.Namespace = "http://tempuri.org";
			if(DisableDefaultEndpoints == false)
			{
				this.AddServiceEndpoint(typeof(WCFArchitect.Samples.HelloWCF.IUsers), new WCFArchitect.Samples.HelloWCF.Bindings.NamedPipeBinding(), PipeEndpointURI);
			}
		}
		public PipeHost(object singletonInstance, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses)
		{
			this.Authorization.ImpersonateCallerForAllOperations = false;
			this.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.None;
			this.CloseTimeout = new TimeSpan(600000000);
			this.OpenTimeout = new TimeSpan(600000000);
			this.Description.Name = "PipeHost";
			this.Description.Namespace = "http://tempuri.org";
			if(DisableDefaultEndpoints == false)
			{
				this.AddServiceEndpoint(typeof(WCFArchitect.Samples.HelloWCF.IUsers), new WCFArchitect.Samples.HelloWCF.Bindings.NamedPipeBinding(), PipeEndpointURI);
			}
		}
		public PipeHost(Type serviceType, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses)
		{
			this.Authorization.ImpersonateCallerForAllOperations = false;
			this.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.None;
			this.CloseTimeout = new TimeSpan(600000000);
			this.OpenTimeout = new TimeSpan(600000000);
			this.Description.Name = "PipeHost";
			this.Description.Namespace = "http://tempuri.org";
			if(DisableDefaultEndpoints == false)
			{
				this.AddServiceEndpoint(typeof(WCFArchitect.Samples.HelloWCF.IUsers), new WCFArchitect.Samples.HelloWCF.Bindings.NamedPipeBinding(), PipeEndpointURI);
			}
		}
	}
}
