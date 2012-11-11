//---------------------------------------------------------------------------
// This code was generated by a tool. Changes to this file may cause 
// incorrect behavior and will be lost if the code is regenerated.
//
// WCF Architect Version:	1.0.0.0
// .NET Framework Version:	4.5
//---------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

#pragma warning disable 1591
namespace Test1
{
	/**************************************************************************
	*	Enumerations
	**************************************************************************/

	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect .NET CSharp Generator - BETA", "1.0.0.0")]
	[DataContract(Namespace = "http://www.prospectivesoftware.com/Test1/")]
	public enum TestEnum : long
	{
		[EnumMember()] _ssads = qweasd | sfsd,
		[EnumMember()] asdas,
		[EnumMember(Value = "321")] qweasd = 123,
		[EnumMember()] sfsd,
	}


	/**************************************************************************
	*	Data Contracts
	**************************************************************************/

	[KnownType(typeof(string[]))]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect .NET CSharp Generator - BETA", "1.0.0.0")]
	[DataContract(Name = "TestData1", Namespace = "http://www.prospectivesoftware.com/Test1/")]
	public partial class TestData1
	{
		[DataMember(Name = "ID")] public Guid ID { get; set; }
		[DataMember(Name = "collectiontest")] public List<string> collectiontest { get; set; }
		[DataMember(Name = "arraytest")] public string[] arraytest { get; set; }
		[DataMember(Name = "dictionarytest")] public Dictionary<int, string> dictionarytest { get; set; }
		[DataMember(Name = "asdas")] public byte[] asdas { get; protected set; }
	}


	/**************************************************************************
	*	Service Contracts
	**************************************************************************/

	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect .NET CSharp Generator - BETA", "1.0.0.0")]
	[ServiceContract(CallbackContract = typeof(ITestServiceCallback), SessionMode = System.ServiceModel.SessionMode.Allowed, Namespace = "http://www.prospectivesoftware.com/Test1/")]
	public interface ITestService
	{
		///<param name='asdsasd'></param>
		///<param name='assdasd'></param>
		[OperationContract(IsInitiating = false)]
		[System.ServiceModel.Web.WebGet(UriTemplate="SynchronousTest/{asdsasd}/{assdasd}", BodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.Bare, RequestFormat = System.ServiceModel.Web.WebMessageFormat.Xml, ResponseFormat = System.ServiceModel.Web.WebMessageFormat.Xml)]
		string SynchronousTest(string asdsasd,bool assdasd);

		[OperationContract(IsInitiating = false)]
		string AsynchronousTestInvoke();

		[OperationContract(IsInitiating = false)]
		Test1.TestData1 AwaitableTestAsync();

	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect .NET CSharp Generator - BETA", "1.0.0.0")]
	public interface ITestServiceCallback
	{
		[OperationContract(IsInitiating = false)]
		bool SyncTest();

		[OperationContract(IsInitiating = false)]
		sbyte AsyncTestInvoke();

		[OperationContract(IsInitiating = false)]
		ObservableCollection<Test1.TestData1> AwaitTestAsync();

	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect .NET CSharp Generator - BETA", "1.0.0.0")]
	public partial class TestServiceCallback : ITestServiceCallback
	{

		private readonly ITestServiceCallback __callback;

		public TestServiceCallback()
		{
			__callback = System.ServiceModel.OperationContext.Current.GetCallbackChannel<ITestServiceCallback>();
		}

		public TestServiceCallback(ITestServiceCallback callback)
		{
			__callback = callback;
		}

		public bool SyncTest()
		{
			return __callback.SyncTest();
		}

		public sbyte AsyncTestInvoke()
		{
			return __callback.AsyncTestInvoke();
		}

		public ObservableCollection<Test1.TestData1> AwaitTestAsync()
		{
			return __callback.AwaitTestAsync();
		}

	}


	/**************************************************************************
	*	Service Bindings
	**************************************************************************/

	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect .NET CSharp Generator - BETA", "1.0.0.0")]
	public partial class TestBinding : System.ServiceModel.NetTcpBinding
	{
		public TestBinding()
		{
			SetDefaults();
		}
		public TestBinding(System.ServiceModel.NetTcpSecurity CustomSecurity)
		{
			SetDefaults();
			this.Security = CustomSecurity;
		}
		public TestBinding(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)
		{
			SetDefaults();
			this.ReaderQuotas = ReaderQuotas;
		}
		public TestBinding(System.ServiceModel.NetTcpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas)
		{
			SetDefaults();
			this.Security = CustomSecurity;
			this.ReaderQuotas = ReaderQuotas;
		}
		private void SetDefaults()
		{
			this.Name = "TestBinding";
			this.Namespace = "";
			this.OpenTimeout = new TimeSpan(600000000);
			this.CloseTimeout = new TimeSpan(600000000);
			this.SendTimeout = new TimeSpan(600000000);
			this.ReceiveTimeout = new TimeSpan(6000000000);
			this.MaxBufferPoolSize = 524288;
			this.MaxBufferSize = 65536;
			this.MaxReceivedMessageSize = 65536;
			this.TransferMode = TransferMode.Buffered;
			this.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
			this.ListenBacklog = 10;
			this.MaxConnections = 10;
			this.PortSharingEnabled = false;
			this.ReliableSession.Enabled = false;
			this.ReliableSession.InactivityTimeout = new TimeSpan(6000000000);
			this.ReliableSession.Ordered = false;
			this.TransactionFlow = true;
			this.TransactionProtocol = TransactionProtocol.Default;
			this.Security.Mode = SecurityMode.Transport;
			this.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
			this.Security.Transport.ProtectionLevel = ProtectionLevel.None;
			this.Security.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.Basic256;
			this.Security.Message.ClientCredentialType = MessageCredentialType.Windows;

		}
	}


	/**************************************************************************
	*	Service Hosts
	**************************************************************************/

	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect .NET CSharp Generator - BETA", "1.0.0.0")]
	public partial class TestHost : ServiceHost
	{
		public ServiceThrottlingBehavior TestT1 { get; private set; }
		public ServiceDebugBehavior TestD1 { get; private set; }
		public ServiceMetadataBehavior TestM1 { get; private set; }
		public WebHttpBehavior TestW1 { get; private set; }
		public ServiceThrottlingBehavior TestT3 { get; private set; }
		public TestHost(object singletonInstance) : base(singletonInstance)
		{
			this.Authorization.ImpersonateCallerForAllOperations = false;
			this.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.None;
			this.CloseTimeout = new TimeSpan(600000000);
			this.OpenTimeout = new TimeSpan(600000000);
			this.Description.Name = "TestHost";
			this.Description.Namespace = "http://www.prospectivesoftware.com/";
			this.TestT1 = new ServiceThrottlingBehavior();
			this.TestT1.MaxConcurrentCalls = 16;
			this.TestT1.MaxConcurrentInstances = 26;
			this.TestT1.MaxConcurrentSessions = 10;
			this.TestD1 = new ServiceDebugBehavior();
			this.TestD1.IncludeExceptionDetailInFaults = false;
			this.TestM1 = new ServiceMetadataBehavior();
			this.TestM1.ExternalMetadataLocation = new Uri("");
			this.Description.Behaviors.Add(TestM1);
			this.TestW1 = new WebHttpBehavior();
			this.TestW1.AutomaticFormatSelectionEnabled = false;
			this.TestW1.DefaultBodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.Bare;
			this.TestW1.DefaultOutgoingRequestFormat = System.ServiceModel.Web.WebMessageFormat.Xml;
			this.TestW1.DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Xml;
			this.TestW1.FaultExceptionEnabled = false;
			this.TestW1.HelpEnabled = false;
			this.TestT3 = new ServiceThrottlingBehavior();
			this.TestT3.MaxConcurrentCalls = 16;
			this.TestT3.MaxConcurrentInstances = 26;
			this.TestT3.MaxConcurrentSessions = 10;
			this.Description.Behaviors.Add(TestT3);
		}
		public TestHost(Type serviceType) : base(serviceType)
		{
			this.Authorization.ImpersonateCallerForAllOperations = false;
			this.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.None;
			this.CloseTimeout = new TimeSpan(600000000);
			this.OpenTimeout = new TimeSpan(600000000);
			this.Description.Name = "TestHost";
			this.Description.Namespace = "http://www.prospectivesoftware.com/";
			this.TestT1 = new ServiceThrottlingBehavior();
			this.TestT1.MaxConcurrentCalls = 16;
			this.TestT1.MaxConcurrentInstances = 26;
			this.TestT1.MaxConcurrentSessions = 10;
			this.TestD1 = new ServiceDebugBehavior();
			this.TestD1.IncludeExceptionDetailInFaults = false;
			this.TestM1 = new ServiceMetadataBehavior();
			this.TestM1.ExternalMetadataLocation = new Uri("");
			this.Description.Behaviors.Add(TestM1);
			this.TestW1 = new WebHttpBehavior();
			this.TestW1.AutomaticFormatSelectionEnabled = false;
			this.TestW1.DefaultBodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.Bare;
			this.TestW1.DefaultOutgoingRequestFormat = System.ServiceModel.Web.WebMessageFormat.Xml;
			this.TestW1.DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Xml;
			this.TestW1.FaultExceptionEnabled = false;
			this.TestW1.HelpEnabled = false;
			this.TestT3 = new ServiceThrottlingBehavior();
			this.TestT3.MaxConcurrentCalls = 16;
			this.TestT3.MaxConcurrentInstances = 26;
			this.TestT3.MaxConcurrentSessions = 10;
			this.Description.Behaviors.Add(TestT3);
		}
		public TestHost(object singletonInstance, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses)
		{
			this.Authorization.ImpersonateCallerForAllOperations = false;
			this.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.None;
			this.CloseTimeout = new TimeSpan(600000000);
			this.OpenTimeout = new TimeSpan(600000000);
			this.Description.Name = "TestHost";
			this.Description.Namespace = "http://www.prospectivesoftware.com/";
			this.TestT1 = new ServiceThrottlingBehavior();
			this.TestT1.MaxConcurrentCalls = 16;
			this.TestT1.MaxConcurrentInstances = 26;
			this.TestT1.MaxConcurrentSessions = 10;
			this.TestD1 = new ServiceDebugBehavior();
			this.TestD1.IncludeExceptionDetailInFaults = false;
			this.TestM1 = new ServiceMetadataBehavior();
			this.TestM1.ExternalMetadataLocation = new Uri("");
			this.Description.Behaviors.Add(TestM1);
			this.TestW1 = new WebHttpBehavior();
			this.TestW1.AutomaticFormatSelectionEnabled = false;
			this.TestW1.DefaultBodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.Bare;
			this.TestW1.DefaultOutgoingRequestFormat = System.ServiceModel.Web.WebMessageFormat.Xml;
			this.TestW1.DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Xml;
			this.TestW1.FaultExceptionEnabled = false;
			this.TestW1.HelpEnabled = false;
			this.TestT3 = new ServiceThrottlingBehavior();
			this.TestT3.MaxConcurrentCalls = 16;
			this.TestT3.MaxConcurrentInstances = 26;
			this.TestT3.MaxConcurrentSessions = 10;
			this.Description.Behaviors.Add(TestT3);
		}
		public TestHost(Type serviceType, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses)
		{
			this.Authorization.ImpersonateCallerForAllOperations = false;
			this.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.None;
			this.CloseTimeout = new TimeSpan(600000000);
			this.OpenTimeout = new TimeSpan(600000000);
			this.Description.Name = "TestHost";
			this.Description.Namespace = "http://www.prospectivesoftware.com/";
			this.TestT1 = new ServiceThrottlingBehavior();
			this.TestT1.MaxConcurrentCalls = 16;
			this.TestT1.MaxConcurrentInstances = 26;
			this.TestT1.MaxConcurrentSessions = 10;
			this.TestD1 = new ServiceDebugBehavior();
			this.TestD1.IncludeExceptionDetailInFaults = false;
			this.TestM1 = new ServiceMetadataBehavior();
			this.TestM1.ExternalMetadataLocation = new Uri("");
			this.Description.Behaviors.Add(TestM1);
			this.TestW1 = new WebHttpBehavior();
			this.TestW1.AutomaticFormatSelectionEnabled = false;
			this.TestW1.DefaultBodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.Bare;
			this.TestW1.DefaultOutgoingRequestFormat = System.ServiceModel.Web.WebMessageFormat.Xml;
			this.TestW1.DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Xml;
			this.TestW1.FaultExceptionEnabled = false;
			this.TestW1.HelpEnabled = false;
			this.TestT3 = new ServiceThrottlingBehavior();
			this.TestT3.MaxConcurrentCalls = 16;
			this.TestT3.MaxConcurrentInstances = 26;
			this.TestT3.MaxConcurrentSessions = 10;
			this.Description.Behaviors.Add(TestT3);
		}
		public TestHost(object singletonInstance, bool DisableDefaultEndpoints) : base(singletonInstance)
		{
			this.Authorization.ImpersonateCallerForAllOperations = false;
			this.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.None;
			this.CloseTimeout = new TimeSpan(600000000);
			this.OpenTimeout = new TimeSpan(600000000);
			this.Description.Name = "TestHost";
			this.Description.Namespace = "http://www.prospectivesoftware.com/";
			this.TestT1 = new ServiceThrottlingBehavior();
			this.TestT1.MaxConcurrentCalls = 16;
			this.TestT1.MaxConcurrentInstances = 26;
			this.TestT1.MaxConcurrentSessions = 10;
			this.TestD1 = new ServiceDebugBehavior();
			this.TestD1.IncludeExceptionDetailInFaults = false;
			this.TestM1 = new ServiceMetadataBehavior();
			this.TestM1.ExternalMetadataLocation = new Uri("");
			this.Description.Behaviors.Add(TestM1);
			this.TestW1 = new WebHttpBehavior();
			this.TestW1.AutomaticFormatSelectionEnabled = false;
			this.TestW1.DefaultBodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.Bare;
			this.TestW1.DefaultOutgoingRequestFormat = System.ServiceModel.Web.WebMessageFormat.Xml;
			this.TestW1.DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Xml;
			this.TestW1.FaultExceptionEnabled = false;
			this.TestW1.HelpEnabled = false;
			this.TestT3 = new ServiceThrottlingBehavior();
			this.TestT3.MaxConcurrentCalls = 16;
			this.TestT3.MaxConcurrentInstances = 26;
			this.TestT3.MaxConcurrentSessions = 10;
			this.Description.Behaviors.Add(TestT3);
			if(DisableDefaultEndpoints == false)
			{
			}
		}
		public TestHost(Type serviceType, bool DisableDefaultEndpoints) : base(serviceType)
		{
			this.Authorization.ImpersonateCallerForAllOperations = false;
			this.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.None;
			this.CloseTimeout = new TimeSpan(600000000);
			this.OpenTimeout = new TimeSpan(600000000);
			this.Description.Name = "TestHost";
			this.Description.Namespace = "http://www.prospectivesoftware.com/";
			this.TestT1 = new ServiceThrottlingBehavior();
			this.TestT1.MaxConcurrentCalls = 16;
			this.TestT1.MaxConcurrentInstances = 26;
			this.TestT1.MaxConcurrentSessions = 10;
			this.TestD1 = new ServiceDebugBehavior();
			this.TestD1.IncludeExceptionDetailInFaults = false;
			this.TestM1 = new ServiceMetadataBehavior();
			this.TestM1.ExternalMetadataLocation = new Uri("");
			this.Description.Behaviors.Add(TestM1);
			this.TestW1 = new WebHttpBehavior();
			this.TestW1.AutomaticFormatSelectionEnabled = false;
			this.TestW1.DefaultBodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.Bare;
			this.TestW1.DefaultOutgoingRequestFormat = System.ServiceModel.Web.WebMessageFormat.Xml;
			this.TestW1.DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Xml;
			this.TestW1.FaultExceptionEnabled = false;
			this.TestW1.HelpEnabled = false;
			this.TestT3 = new ServiceThrottlingBehavior();
			this.TestT3.MaxConcurrentCalls = 16;
			this.TestT3.MaxConcurrentInstances = 26;
			this.TestT3.MaxConcurrentSessions = 10;
			this.Description.Behaviors.Add(TestT3);
			if(DisableDefaultEndpoints == false)
			{
			}
		}
		public TestHost(object singletonInstance, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(singletonInstance, BaseAddresses)
		{
			this.Authorization.ImpersonateCallerForAllOperations = false;
			this.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.None;
			this.CloseTimeout = new TimeSpan(600000000);
			this.OpenTimeout = new TimeSpan(600000000);
			this.Description.Name = "TestHost";
			this.Description.Namespace = "http://www.prospectivesoftware.com/";
			this.TestT1 = new ServiceThrottlingBehavior();
			this.TestT1.MaxConcurrentCalls = 16;
			this.TestT1.MaxConcurrentInstances = 26;
			this.TestT1.MaxConcurrentSessions = 10;
			this.TestD1 = new ServiceDebugBehavior();
			this.TestD1.IncludeExceptionDetailInFaults = false;
			this.TestM1 = new ServiceMetadataBehavior();
			this.TestM1.ExternalMetadataLocation = new Uri("");
			this.Description.Behaviors.Add(TestM1);
			this.TestW1 = new WebHttpBehavior();
			this.TestW1.AutomaticFormatSelectionEnabled = false;
			this.TestW1.DefaultBodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.Bare;
			this.TestW1.DefaultOutgoingRequestFormat = System.ServiceModel.Web.WebMessageFormat.Xml;
			this.TestW1.DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Xml;
			this.TestW1.FaultExceptionEnabled = false;
			this.TestW1.HelpEnabled = false;
			this.TestT3 = new ServiceThrottlingBehavior();
			this.TestT3.MaxConcurrentCalls = 16;
			this.TestT3.MaxConcurrentInstances = 26;
			this.TestT3.MaxConcurrentSessions = 10;
			this.Description.Behaviors.Add(TestT3);
			if(DisableDefaultEndpoints == false)
			{
			}
		}
		public TestHost(Type serviceType, bool DisableDefaultEndpoints, params Uri[] BaseAddresses) : base(serviceType, BaseAddresses)
		{
			this.Authorization.ImpersonateCallerForAllOperations = false;
			this.Authorization.PrincipalPermissionMode = System.ServiceModel.Description.PrincipalPermissionMode.None;
			this.CloseTimeout = new TimeSpan(600000000);
			this.OpenTimeout = new TimeSpan(600000000);
			this.Description.Name = "TestHost";
			this.Description.Namespace = "http://www.prospectivesoftware.com/";
			this.TestT1 = new ServiceThrottlingBehavior();
			this.TestT1.MaxConcurrentCalls = 16;
			this.TestT1.MaxConcurrentInstances = 26;
			this.TestT1.MaxConcurrentSessions = 10;
			this.TestD1 = new ServiceDebugBehavior();
			this.TestD1.IncludeExceptionDetailInFaults = false;
			this.TestM1 = new ServiceMetadataBehavior();
			this.TestM1.ExternalMetadataLocation = new Uri("");
			this.Description.Behaviors.Add(TestM1);
			this.TestW1 = new WebHttpBehavior();
			this.TestW1.AutomaticFormatSelectionEnabled = false;
			this.TestW1.DefaultBodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.Bare;
			this.TestW1.DefaultOutgoingRequestFormat = System.ServiceModel.Web.WebMessageFormat.Xml;
			this.TestW1.DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Xml;
			this.TestW1.FaultExceptionEnabled = false;
			this.TestW1.HelpEnabled = false;
			this.TestT3 = new ServiceThrottlingBehavior();
			this.TestT3.MaxConcurrentCalls = 16;
			this.TestT3.MaxConcurrentInstances = 26;
			this.TestT3.MaxConcurrentSessions = 10;
			this.Description.Behaviors.Add(TestT3);
			if(DisableDefaultEndpoints == false)
			{
			}
		}
	}


}
namespace Test1.TestNS
{
}


#pragma warning restore 1591
