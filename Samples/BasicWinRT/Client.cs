//---------------------------------------------------------------------------
// This code was generated by a tool. Changes to this file may cause 
// incorrect behavior and will be lost if the code is regenerated.
//
// NETPath Version:	2.0.0.1335
// .NET Framework Version:	4.5 (Windows Runtime)
//---------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using Windows.UI.Core;
using Windows.UI.Xaml;

[assembly: System.Runtime.Serialization.ContractNamespaceAttribute("http://tempuri.org/WCFArchitect/SampleServer/BasicWinRT/", ClrNamespace="WCFArchitect.SampleServer.BasicWinRT")]

#pragma warning disable 1591
namespace WCFArchitect.SampleServer.BasicWinRT
{
	/**************************************************************************
	*	Enumeration Contracts
	**************************************************************************/

	[System.CodeDom.Compiler.GeneratedCodeAttribute("NETPath WinRT CSharp Generator - BETA", "2.0.0.1335")]
	[DataContract(Namespace = "http://tempuri.org/WCFArchitect/SampleServer/BasicWinRT/")]
	public enum Colors : long
	{
		[EnumMember()] Blue,
		[EnumMember()] Green,
		[EnumMember()] Red,
		[EnumMember()] Yellow,
		[EnumMember()] Orange,
		[EnumMember()] Gray,
		[EnumMember()] Teal,
		[EnumMember()] Black,
	}


	/**************************************************************************
	*	Data Contracts
	**************************************************************************/

	[System.Diagnostics.DebuggerStepThroughAttribute]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("NETPath WinRT CSharp Generator - BETA", "2.0.0.1335")]
	[ProtoBuf.ProtoContract(SkipConstructor = false, UseProtoMembersOnly = true)]
	[DataContract(Name = "Customer", Namespace = "http://tempuri.org/")]
	public partial class Customer
	{
		public CustomerXAML XAMLObject { get; private set; }

		//Data Change Messaging Support
		private readonly System.Threading.ReaderWriterLockSlim __dcmlock = new System.Threading.ReaderWriterLockSlim();
		private static readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, Customer> __dcm;
		static Customer()
		{
			__dcm = new System.Collections.Concurrent.ConcurrentDictionary<Guid, Customer>();
		}
		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			__dcm.TryAdd(_DCMID, this);
		}
		~Customer()
		{
			Customer t;
			__dcm.TryAdd(_DCMID, this);
		}

		//Constuctors
		public Customer()
		{
			XAMLObject = this;
		}
		public Customer(CustomerXAML Data)
		{
			XAMLObject = Data;
			ID = Data.ID;
			Name = Data.Name;
			AddressLine1 = Data.AddressLine1;
			AddressLine2 = Data.AddressLine2;
			City = Data.City;
			State = Data.State;
			ZipCode = Data.ZipCode;
			Color = Data.Color;
			_DCMID = Data._DCMID;
		}

		private Guid IDField;
		[DataMember(Name = "ID")] public Guid ID { get { return IDField; } set { IDField = value; } }
		private string NameField;
		[DataMember(Name = "Name")] public string Name { get { return NameField; } set { NameField = value; } }
		private string AddressLine1Field;
		[DataMember(Name = "AddressLine1")] public string AddressLine1 { get { return AddressLine1Field; } set { AddressLine1Field = value; } }
		private string AddressLine2Field;
		[DataMember(Name = "AddressLine2")] public string AddressLine2 { get { return AddressLine2Field; } set { AddressLine2Field = value; } }
		private string CityField;
		[DataMember(Name = "City")] public string City { get { return CityField; } set { CityField = value; } }
		private string StateField;
		[ProtoBuf.ProtoMember(0, IsPacked = true, OverwriteList = true, AsReference = true, DynamicType = true)] [DataMember(Order = 5, Name = "State")] public string State { get { return StateField; } set { StateField = value; } }
		private int ZipCodeField;
		[ProtoBuf.ProtoMember(1, OverwriteList = true)] [DataMember(Order = 6, Name = "ZipCode")] public int ZipCode { get { return ZipCodeField; } set { ZipCodeField = value; } }
		private WCFArchitect.SampleServer.BasicWinRT.Colors ColorField;
		[ProtoBuf.ProtoMember(2, OverwriteList = true)] [DataMember(Order = 7, Name = "Color")] public WCFArchitect.SampleServer.BasicWinRT.Colors Color { get { return ColorField; } set { ColorField = value; } }
		private Guid _DCMIDField;
		[ProtoBuf.ProtoMember(3, OverwriteList = true)] [DataMember(Order = 8, Name = "_DCMID")] public Guid _DCMID { get { return _DCMIDField; } protected set { _DCMIDField = value; } }
	}

	//XAML Integration Object for the Customer DTO
	[System.CodeDom.Compiler.GeneratedCodeAttribute("NETPath WinRT CSharp Generator - BETA", "2.0.0.1335")]
	public partial class CustomerXAML : DependencyObjectEx
	{
		public Customer DataObject { get; private set; }
		public bool IsUpdating { get; set; }

		//Properties
		public Guid ID { get { return (Guid)GetValue(IDProperty); }  set { SetValue(IDProperty, value); } }
		public static readonly DependencyProperty IDProperty = DependencyProperty.Register("ID", typeof(Guid), typeof(CustomerXAML), null);
		public string Name { get { return (string)GetValue(NameProperty); }  set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(CustomerXAML), null);
		public string AddressLine1 { get { return (string)GetValue(AddressLine1Property); }  set { SetValue(AddressLine1Property, value); } }
		public static readonly DependencyProperty AddressLine1Property = DependencyProperty.Register("AddressLine1", typeof(string), typeof(CustomerXAML), null);
		public string AddressLine2 { get { return (string)GetValue(AddressLine2Property); }  set { SetValue(AddressLine2Property, value); } }
		public static readonly DependencyProperty AddressLine2Property = DependencyProperty.Register("AddressLine2", typeof(string), typeof(CustomerXAML), null);
		public string City { get { return (string)GetValue(CityProperty); }  set { SetValue(CityProperty, value); } }
		public static readonly DependencyProperty CityProperty = DependencyProperty.Register("City", typeof(string), typeof(CustomerXAML), null);
		public string State { get { return (string)GetValue(StateProperty); }  set { SetValue(StateProperty, value); } }
		public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(string), typeof(CustomerXAML), null);
		public int ZipCode { get { return (int)GetValue(ZipCodeProperty); }  set { SetValue(ZipCodeProperty, value); } }
		public static readonly DependencyProperty ZipCodeProperty = DependencyProperty.Register("ZipCode", typeof(int), typeof(CustomerXAML), null);
		public WCFArchitect.SampleServer.BasicWinRT.Colors Color { get { return (WCFArchitect.SampleServer.BasicWinRT.Colors)GetValue(ColorProperty); }  set { SetValue(ColorProperty, value); } }
		public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(WCFArchitect.SampleServer.BasicWinRT.Colors), typeof(CustomerXAML), null);
		public Guid _DCMID { get { return (Guid)GetValue(_DCMIDProperty); } protected set { SetValue(_DCMIDProperty, value); } }
		public static readonly DependencyProperty _DCMIDProperty = DependencyProperty.Register("_DCMID", typeof(Guid), typeof(CustomerXAML), null);

		//Implicit Conversion
		public static implicit operator Customer(CustomerXAML Data)
		{
			if (Data == null) return null;
			Customer v = null;
			if (Window.Current.Dispatcher.HasThreadAccess) v = ConvertFromXAMLObject(Data);
			else Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { v = ConvertToXAMLObject(Data); });
			return v;
		}
		public static implicit operator CustomerXAML(Customer Data)
		{
			if (Data == null) return null;
			CustomerXAML v = null;
			if (Window.Current.Dispatcher.HasThreadAccess) v = ConvertToXAMLObject(Data);
			else Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { v = ConvertToXAMLObject(Data); });
			return v;
		}

		//Constructors
		public CustomerXAML()
		{
		}

		public CustomerXAML(Customer Data)
		{
			DataObject = Data;
			ID = Data.ID;
			Name = Data.Name;
			AddressLine1 = Data.AddressLine1;
			AddressLine2 = Data.AddressLine2;
			City = Data.City;
			State = Data.State;
			ZipCode = Data.ZipCode;
			Color = Data.Color;
			_DCMID = Data._DCMID;
		}

		//XAML/DTO Conversion Functions
		public static Customer ConvertFromXAMLObject(CustomerXAML Data)
		{
			if (Data.DataObject != null) return Data.DataObject;
			return new Customer(Data);
		}

		public static CustomerXAML ConvertToXAMLObject(Customer Data)
		{
			if (Data.XAMLObject != null) return Data.XAMLObject;
			return new CustomerXAML(Data);
		}
	}


	/**************************************************************************
	*	Service Contracts
	**************************************************************************/

	[System.CodeDom.Compiler.GeneratedCodeAttribute("NETPath WinRT CSharp Generator - BETA", "2.0.0.1335")]
	public interface ICustomers
	{
		///<param name='NewCustomer'></param>
		[OperationContract(IsOneWay = true, Action = "http://tempuri.org/WCFArchitect/SampleServer/BasicWinRT/Customers/AddCustomerAsync")]
		System.Threading.Tasks.Task AddCustomerAsync(WCFArchitect.SampleServer.BasicWinRT.Customer NewCustomer);

		[OperationContract(Action = "http://tempuri.org/WCFArchitect/SampleServer/BasicWinRT/Customers/GetCustomerAsync", ReplyAction = "http://tempuri.org/WCFArchitect/SampleServer/BasicWinRT/Customers/GetCustomerAsyncResponse")]
		System.Threading.Tasks.Task<WCFArchitect.SampleServer.BasicWinRT.Customer> GetCustomerAsync();

		///<param name='Updated'></param>
		[OperationContract(IsOneWay = true, Action = "http://tempuri.org/WCFArchitect/SampleServer/BasicWinRT/Customers/UpdateCustomerAsync")]
		System.Threading.Tasks.Task UpdateCustomerAsync(WCFArchitect.SampleServer.BasicWinRT.Customer Updated);

		///<param name='CustomerID'></param>
		[OperationContract(Action = "http://tempuri.org/WCFArchitect/SampleServer/BasicWinRT/Customers/DeleteCustomer", ReplyAction = "http://tempuri.org/WCFArchitect/SampleServer/BasicWinRT/Customers/DeleteCustomerResponse")]
		bool DeleteCustomer(Guid CustomerID);

	}

	[System.CodeDom.Compiler.GeneratedCodeAttribute("NETPath WinRT CSharp Generator - BETA", "2.0.0.1335")]
	public interface ICustomersCallback
	{
		///<param name='Callback'>The function to call when the operation is complete.</param>
		///<param name='AsyncState'>An object representing the state of the operation.</param>
		[OperationContract(Action = "http://tempuri.org/WCFArchitect/SampleServer/BasicWinRT/Customers/AsyncCallbackTestInvoke", ReplyAction = "http://tempuri.org/WCFArchitect/SampleServer/BasicWinRT/Customers/AsyncCallbackTestInvokeResponse")]
		IAsyncResult BeginAsyncCallbackTestInvoke(AsyncCallback Callback, object AsyncState);
		///<summary>Finalizes the asynchronous operation.</summary>
		///<returns>
		///
		///</returns>
		///<param name='result'>The result of the operation.</param>
		bool EndAsyncCallbackTestInvoke(IAsyncResult result);

	}

	[System.CodeDom.Compiler.GeneratedCodeAttribute("NETPath WinRT CSharp Generator - BETA", "2.0.0.1335")]
	public interface ICustomersChannel : ICustomers, System.ServiceModel.IClientChannel
	{
	}

	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("NETPath WinRT CSharp Generator - BETA", "2.0.0.1335")]
	public partial class CustomersProxy : System.ServiceModel.DuplexClientBase<ICustomers>, ICustomers
	{
		public CustomersProxy(System.ServiceModel.InstanceContext callbackInstance) : base(callbackInstance)
		{
		}

		public CustomersProxy(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
		{
		}

		public CustomersProxy(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
		{
		}

		public CustomersProxy(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
		{
		}

		public CustomersProxy(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
		{
		}

		public static Uri NetHttpEndpointURI { get { return new Uri("http://localhost/NetHttpEndpoint"); } }
		public static WCFArchitect.SampleServer.BasicWinRT.CustomersProxy CreateCustomerHostService(InstanceContext CallbackInstance)
		{
			var t = new WCFArchitect.SampleServer.BasicWinRT.CustomersProxy(CallbackInstance, new WCFArchitect.SampleServer.BasicWinRT.CustomerNetHttpBinding(), new System.ServiceModel.EndpointAddress(new Uri("http://localhost/NetHttpEndpoint")));
			return t;
		}
		public static WCFArchitect.SampleServer.BasicWinRT.CustomersProxy CreateCustomerHostService(InstanceContext CallbackInstance, string Address)
		{
			var t = new WCFArchitect.SampleServer.BasicWinRT.CustomersProxy(CallbackInstance, new WCFArchitect.SampleServer.BasicWinRT.CustomerNetHttpBinding(), new System.ServiceModel.EndpointAddress(new Uri(string.Format("http://{0}/NetHttpEndpoint", Address))));
			return t;
		}
		public static WCFArchitect.SampleServer.BasicWinRT.CustomersProxy CreateCustomerHostService(InstanceContext CallbackInstance, int Port)
		{
			var t = new WCFArchitect.SampleServer.BasicWinRT.CustomersProxy(CallbackInstance, new WCFArchitect.SampleServer.BasicWinRT.CustomerNetHttpBinding(), new System.ServiceModel.EndpointAddress(new Uri(string.Format("http://localhost{0}/NetHttpEndpoint", Port > 0 ? string.Format(":{0}", Port) : ""))));
			return t;
		}
		public static WCFArchitect.SampleServer.BasicWinRT.CustomersProxy CreateCustomerHostService(InstanceContext CallbackInstance, string Address, int Port)
		{
			var t = new WCFArchitect.SampleServer.BasicWinRT.CustomersProxy(CallbackInstance, new WCFArchitect.SampleServer.BasicWinRT.CustomerNetHttpBinding(), new System.ServiceModel.EndpointAddress(new Uri(string.Format("http://{0}{1}/NetHttpEndpoint", Address, Port > 0 ? string.Format(":{0}", Port) : ""))));
			return t;
		}
		public static WCFArchitect.SampleServer.BasicWinRT.CustomersProxy CreateCustomerHostService(InstanceContext CallbackInstance, System.ServiceModel.EndpointIdentity Identity)
		{
			var t = new WCFArchitect.SampleServer.BasicWinRT.CustomersProxy(CallbackInstance, new WCFArchitect.SampleServer.BasicWinRT.CustomerNetHttpBinding(), new System.ServiceModel.EndpointAddress(new Uri("http://localhost/NetHttpEndpoint"), Identity));
			return t;
		}
		public static WCFArchitect.SampleServer.BasicWinRT.CustomersProxy CreateCustomerHostService(InstanceContext CallbackInstance, string Address, System.ServiceModel.EndpointIdentity Identity)
		{
			var t = new WCFArchitect.SampleServer.BasicWinRT.CustomersProxy(CallbackInstance, new WCFArchitect.SampleServer.BasicWinRT.CustomerNetHttpBinding(), new System.ServiceModel.EndpointAddress(new Uri(string.Format("http://{0}/NetHttpEndpoint", Address)), Identity));
			return t;
		}
		public static WCFArchitect.SampleServer.BasicWinRT.CustomersProxy CreateCustomerHostService(InstanceContext CallbackInstance, int Port, System.ServiceModel.EndpointIdentity Identity)
		{
			var t = new WCFArchitect.SampleServer.BasicWinRT.CustomersProxy(CallbackInstance, new WCFArchitect.SampleServer.BasicWinRT.CustomerNetHttpBinding(), new System.ServiceModel.EndpointAddress(new Uri(string.Format("http://localhost{0}/NetHttpEndpoint", Port > 0 ? string.Format(":{0}", Port) : "")), Identity));
			return t;
		}
		public static WCFArchitect.SampleServer.BasicWinRT.CustomersProxy CreateCustomerHostService(InstanceContext CallbackInstance, string Address, int Port, System.ServiceModel.EndpointIdentity Identity)
		{
			var t = new WCFArchitect.SampleServer.BasicWinRT.CustomersProxy(CallbackInstance, new WCFArchitect.SampleServer.BasicWinRT.CustomerNetHttpBinding(), new System.ServiceModel.EndpointAddress(new Uri(string.Format("http://{0}{1}/NetHttpEndpoint", Address, Port > 0 ? string.Format(":{0}", Port) : "")), Identity));
			return t;
		}
		public static WCFArchitect.SampleServer.BasicWinRT.CustomersProxy CreateCustomerHostServiceConfig(InstanceContext CallbackInstance, string EndpointConfig)
		{
			var t = new WCFArchitect.SampleServer.BasicWinRT.CustomersProxy(CallbackInstance, EndpointConfig, new System.ServiceModel.EndpointAddress(new Uri("http://localhost/NetHttpEndpoint")));
			return t;
		}
		public static WCFArchitect.SampleServer.BasicWinRT.CustomersProxy CreateCustomerHostServiceConfig(InstanceContext CallbackInstance, string EndpointConfig, string Address)
		{
			var t = new WCFArchitect.SampleServer.BasicWinRT.CustomersProxy(CallbackInstance, EndpointConfig, new System.ServiceModel.EndpointAddress(new Uri(string.Format("http://{0}/NetHttpEndpoint", Address))));
			return t;
		}
		public static WCFArchitect.SampleServer.BasicWinRT.CustomersProxy CreateCustomerHostServiceConfig(InstanceContext CallbackInstance, string EndpointConfig, int Port)
		{
			var t = new WCFArchitect.SampleServer.BasicWinRT.CustomersProxy(CallbackInstance, EndpointConfig, new System.ServiceModel.EndpointAddress(new Uri(string.Format("http://localhost{0}/NetHttpEndpoint", Port > 0 ? string.Format(":{0}", Port) : ""))));
			return t;
		}
		public static WCFArchitect.SampleServer.BasicWinRT.CustomersProxy CreateCustomerHostServiceConfig(InstanceContext CallbackInstance, string EndpointConfig, string Address, int Port)
		{
			var t = new WCFArchitect.SampleServer.BasicWinRT.CustomersProxy(CallbackInstance, EndpointConfig, new System.ServiceModel.EndpointAddress(new Uri(string.Format("http://{0}{1}/NetHttpEndpoint", Address, Port > 0 ? string.Format(":{0}", Port) : ""))));
			return t;
		}
		public static WCFArchitect.SampleServer.BasicWinRT.CustomersProxy CreateCustomerHostServiceConfig(InstanceContext CallbackInstance, string EndpointConfig, System.ServiceModel.EndpointIdentity Identity)
		{
			var t = new WCFArchitect.SampleServer.BasicWinRT.CustomersProxy(CallbackInstance, EndpointConfig, new System.ServiceModel.EndpointAddress(new Uri("http://localhost/NetHttpEndpoint"), Identity));
			return t;
		}
		public static WCFArchitect.SampleServer.BasicWinRT.CustomersProxy CreateCustomerHostServiceConfig(InstanceContext CallbackInstance, string EndpointConfig, string Address, System.ServiceModel.EndpointIdentity Identity)
		{
			var t = new WCFArchitect.SampleServer.BasicWinRT.CustomersProxy(CallbackInstance, EndpointConfig, new System.ServiceModel.EndpointAddress(new Uri(string.Format("http://{0}/NetHttpEndpoint", Address)), Identity));
			return t;
		}
		public static WCFArchitect.SampleServer.BasicWinRT.CustomersProxy CreateCustomerHostServiceConfig(InstanceContext CallbackInstance, string EndpointConfig, int Port, System.ServiceModel.EndpointIdentity Identity)
		{
			var t = new WCFArchitect.SampleServer.BasicWinRT.CustomersProxy(CallbackInstance, EndpointConfig, new System.ServiceModel.EndpointAddress(new Uri(string.Format("http://localhost{0}/NetHttpEndpoint", Port > 0 ? string.Format(":{0}", Port) : "")), Identity));
			return t;
		}
		public static WCFArchitect.SampleServer.BasicWinRT.CustomersProxy CreateCustomerHostServiceConfig(InstanceContext CallbackInstance, string EndpointConfig, string Address, int Port, System.ServiceModel.EndpointIdentity Identity)
		{
			var t = new WCFArchitect.SampleServer.BasicWinRT.CustomersProxy(CallbackInstance, EndpointConfig, new System.ServiceModel.EndpointAddress(new Uri(string.Format("http://{0}{1}/NetHttpEndpoint", Address, Port > 0 ? string.Format(":{0}", Port) : "")), Identity));
			return t;
		}

		///<param name='NewCustomer'></param>
		public System.Threading.Tasks.Task AddCustomerAsync(WCFArchitect.SampleServer.BasicWinRT.Customer NewCustomer)
		{
			return base.Channel.AddCustomerAsync(NewCustomer);
		}

		public System.Threading.Tasks.Task<WCFArchitect.SampleServer.BasicWinRT.Customer> GetCustomerAsync()
		{
			return base.Channel.GetCustomerAsync();
		}

		///<param name='Updated'></param>
		public System.Threading.Tasks.Task UpdateCustomerAsync(WCFArchitect.SampleServer.BasicWinRT.Customer Updated)
		{
			return base.Channel.UpdateCustomerAsync(Updated);
		}

		///<param name='CustomerID'></param>
		public bool DeleteCustomer(Guid CustomerID)
		{
			return base.Channel.DeleteCustomer(CustomerID);
		}

	}


	/**************************************************************************
	*	Service Bindings
	**************************************************************************/

	[System.CodeDom.Compiler.GeneratedCodeAttribute("NETPath WinRT CSharp Generator - BETA", "2.0.0.1335")]
	public partial class CustomerNetHttpBinding : System.ServiceModel.NetHttpBinding
	{
		public CustomerNetHttpBinding()
		{
			SetDefaults();
		}
		public CustomerNetHttpBinding(System.ServiceModel.BasicHttpSecurity CustomSecurity)
		{
			SetDefaults();
			this.Security = CustomSecurity;
		}
		public CustomerNetHttpBinding(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)
		{
			SetDefaults();
			this.ReaderQuotas = ReaderQuotas;
		}
		public CustomerNetHttpBinding(System.ServiceModel.BasicHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)
		{
			SetDefaults();
			this.Security = CustomSecurity;
			this.ReaderQuotas = ReaderQuotas;
		}
		private void SetDefaults()
		{
			this.Name = "CustomerNetHttpBinding";
			this.Namespace = "http://tempuri.org";
			this.OpenTimeout = new TimeSpan(600000000);
			this.CloseTimeout = new TimeSpan(600000000);
			this.SendTimeout = new TimeSpan(600000000);
			this.ReceiveTimeout = new TimeSpan(6000000000);
			this.AllowCookies = false;
			this.MaxBufferPoolSize = 524288;
			this.MaxBufferSize = 65536;
			this.MaxReceivedMessageSize = 65536;
			this.MessageEncoding = NetHttpMessageEncoding.Text;
			this.TextEncoding = System.Text.Encoding.UTF8;
			this.TransferMode = TransferMode.Buffered;
			this.WebSocketSettings.DisablePayloadMasking = false;
			this.WebSocketSettings.KeepAliveInterval = new TimeSpan(6000000000);
			this.WebSocketSettings.SubProtocol = "";
			this.WebSocketSettings.TransportUsage = System.ServiceModel.Channels.WebSocketTransportUsage.WhenDuplex;
			this.Security.Mode = BasicHttpSecurityMode.None;
			this.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

		}
	}


}

#pragma warning restore 1591
