//---------------------------------------------------------------------------
// This code was generated by a tool. Changes to this file may cause 
// incorrect behavior and will be lost if the code is regenerated.
//
// WCF Architect Version:	2.0.2000.0
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
using System.Windows;

[assembly: System.Runtime.Serialization.ContractNamespaceAttribute("http://www.prospectivesoftware.com/Test1/", ClrNamespace="Test1")]
[assembly: System.Runtime.Serialization.ContractNamespaceAttribute("http://www.prospectivesoftware.com/Test1/TestNS/", ClrNamespace="Test1.TestNS")]


#pragma warning disable 1591
namespace Test1
{
	/**************************************************************************
	*	Enumeration Contracts
	**************************************************************************/

	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect Service Compiler - BETA", "2.0.2000.0")]
	[DataContract(Namespace = "http://www.prospectivesoftware.com/Test1/")]
	public enum TestEnum : long
	{
		[EnumMember()] _ssads = qweasd | sfsd,
		[EnumMember()] asdas,
		[EnumMember()] qweasd = 321,
		[EnumMember()] sfsd,
	}


	/**************************************************************************
	*	Data Contracts
	**************************************************************************/

	[KnownType(typeof(string[]))]
	[System.Diagnostics.DebuggerStepThroughAttribute]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect Service Compiler - BETA", "2.0.2000.0")]
	[DataContract(Name = "TestData1", Namespace = "http://www.prospectivesoftware.com/")]
	public partial class TestData1 : System.Runtime.Serialization.IExtensibleDataObject
	{
		//Automatic Data Update Support
		private readonly System.Threading.ReaderWriterLockSlim __autodatalock = new System.Threading.ReaderWriterLockSlim();
		private static readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, Test1.TestData1> __autodata;
		static TestData1()
		{
			__autodata = new System.Collections.Concurrent.ConcurrentDictionary<Guid, Test1.TestData1>();
		}
		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			__autodata.TryAdd(ID, this);
		}
		~TestData1()
		{
			Test1.TestData1 t;
			__autodata.TryRemove(ID, out t);
		}

		public System.Runtime.Serialization.ExtensionDataObject ExtensionData { get; set; }

		private bool IDChanged;
		private Guid IDField;
		[DataMember(Name = "ID")] public Guid ID { get { __autodatalock.EnterReadLock(); try { return IDField; } finally { __autodatalock.ExitReadLock(); } } set { __autodatalock.EnterWriteLock(); try { IDField = value; IDChanged = true; } finally { __autodatalock.ExitWriteLock(); } } }
		private List<string> collectiontestField;
		[DataMember(Name = "collectiontest")] public List<string> collectiontest { get { return collectiontestField; } set { collectiontestField = value; } }
		private string[] arraytestField;
		[DataMember(Name = "arraytest")] public string[] arraytest { get { return arraytestField; } set { arraytestField = value; } }
		private Dictionary<int, string> dictionarytestField;
		[DataMember(Name = "dictionarytest")] public Dictionary<int, string> dictionarytest { get { return dictionarytestField; } set { dictionarytestField = value; } }
		private bool asdasChanged;
		private byte[] asdasField;
		[DataMember(Name = "asdas")] public byte[] asdas { get { __autodatalock.EnterReadLock(); try { return asdasField; } finally { __autodatalock.ExitReadLock(); } } protected set { __autodatalock.EnterWriteLock(); try { asdasField = value; asdasChanged = true; } finally { __autodatalock.ExitWriteLock(); } } }
	}

	//XAML Integration Object for the TestData1 DTO
	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect Service Compiler - BETA", "2.0.2000.0")]
	public partial class TestData1XAML : DependencyObject
	{
		//Properties
		private int IDChanged;
		public Guid ID { get { return (Guid)GetValue(IDProperty); } set { SetValue(IDProperty, value); } }
		public static readonly DependencyProperty IDProperty = DependencyProperty.Register("ID", typeof(Guid), typeof(Test1.TestData1XAML));
		public List<string> collectiontest { get { return (List<string>)GetValue(collectiontestProperty); } set { SetValue(collectiontestProperty, value); } }
		public static readonly DependencyProperty collectiontestProperty = DependencyProperty.Register("collectiontest", typeof(List<string>), typeof(Test1.TestData1XAML));
		public string[] arraytest { get { return (string[])GetValue(arraytestProperty); } set { SetValue(arraytestProperty, value); } }
		public static readonly DependencyProperty arraytestProperty = DependencyProperty.Register("arraytest", typeof(string[]), typeof(Test1.TestData1XAML));
		public Dictionary<int, string> dictionarytest { get { return (Dictionary<int, string>)GetValue(dictionarytestProperty); } set { SetValue(dictionarytestProperty, value); } }
		public static readonly DependencyProperty dictionarytestProperty = DependencyProperty.Register("dictionarytest", typeof(Dictionary<int, string>), typeof(Test1.TestData1XAML));
		private int asdasChanged;
		public byte[] asdas { get { return (byte[])GetValue(asdasProperty); } protected set { SetValue(asdasPropertyKey, value); } }
		public static void Setasdas(DependencyObject obj, byte[] value) { obj.SetValue(asdasPropertyKey, value); }
		private static readonly DependencyPropertyKey asdasPropertyKey = DependencyProperty.RegisterReadOnly("asdas", typeof(byte[]), typeof(Test1.TestData1XAML), null);
		public static readonly DependencyProperty asdasProperty = asdasPropertyKey.DependencyProperty;

		//Implicit Conversion
		public static implicit operator Test1.TestData1(Test1.TestData1XAML Data)
		{
			if (Data == null) return null;
			TestData1 v = null;
			if (Application.Current.Dispatcher.CheckAccess()) v = ConvertFromXAMLObject(Data);
			else Application.Current.Dispatcher.Invoke(() => { v = ConvertFromXAMLObject(Data); }, System.Windows.Threading.DispatcherPriority.Normal);
			return v;
		}
		public static implicit operator Test1.TestData1XAML(Test1.TestData1 Data)
		{
			if (Data == null) return null;
			TestData1XAML v = null;
			if (Application.Current.Dispatcher.CheckAccess()) v = ConvertToXAMLObject(Data);
			else Application.Current.Dispatcher.Invoke(() => { v = ConvertToXAMLObject(Data); }, System.Windows.Threading.DispatcherPriority.Normal);
			return v;
		}

		//Automatic Data Update Support
		private readonly System.Threading.ReaderWriterLockSlim __autodatalock = new System.Threading.ReaderWriterLockSlim();
		private static readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, Test1.TestData1XAML> __autodata;
		static TestData1XAML()
		{
			__autodata = new System.Collections.Concurrent.ConcurrentDictionary<Guid, Test1.TestData1XAML>();
		}
		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			__autodata.TryAdd(ID, this);
		}
		~TestData1XAML()
		{
			Test1.TestData1XAML t;
			__autodata.TryRemove(ID, out t);
		}

		//Constructors
		public TestData1XAML()
		{
		}

		public TestData1XAML(Test1.TestData1 Data)
		{
			ID = Data.ID;
			List<string> vcollectiontest = new List<string>();
			foreach(string a in Data.collectiontest) { vcollectiontest.Add(a); }
			collectiontest = vcollectiontest;
			string[] varraytest = new string[Data.arraytest.GetLength(0)];
			for(int i = 0; i < Data.arraytest.GetLength(0); i++) { varraytest[i] = Data.arraytest[i]; }
			arraytest = varraytest;
			Dictionary<int, string> vdictionarytest = new Dictionary<int, string>();
			foreach(KeyValuePair<int, string> a in Data.dictionarytest) { vdictionarytest.Add(a.Key, a.Value); }
			dictionarytest = vdictionarytest;
			Test1.TestData1XAML.Setasdas(this, Data.asdas);
		}

		//XAML/DTO Conversion Functions
		public static TestData1 ConvertFromXAMLObject(Test1.TestData1XAML Data)
		{
			Test1.TestData1 DTO = new Test1.TestData1();
			DTO.ID = Data.ID;
			List<string> vcollectiontest = new List<string>();
			foreach(string a in Data.collectiontest) { vcollectiontest.Add(a); }
			DTO.collectiontest = vcollectiontest;
			string[] varraytest = new string[Data.arraytest.GetLength(0)];
			for(int i = 0; i < Data.arraytest.GetLength(0); i++) { varraytest[i] = Data.arraytest[i]; }
			DTO.arraytest = varraytest;
			Dictionary<int, string> vdictionarytest = new Dictionary<int, string>();
			foreach(KeyValuePair<int, string> a in Data.dictionarytest) { vdictionarytest.Add(a.Key, a.Value); }
			DTO.dictionarytest = vdictionarytest;
			return DTO;
		}

		public static TestData1XAML ConvertToXAMLObject(Test1.TestData1 Data)
		{
			Test1.TestData1XAML XAML = new Test1.TestData1XAML();
			XAML.ID = Data.ID;
			List<string> vcollectiontest = new List<string>();
			foreach(string a in Data.collectiontest) { vcollectiontest.Add(a); }
			XAML.collectiontest = vcollectiontest;
			string[] varraytest = new string[Data.arraytest.GetLength(0)];
			for(int i = 0; i < Data.arraytest.GetLength(0); i++) { varraytest[i] = Data.arraytest[i]; }
			XAML.arraytest = varraytest;
			Dictionary<int, string> vdictionarytest = new Dictionary<int, string>();
			foreach(KeyValuePair<int, string> a in Data.dictionarytest) { vdictionarytest.Add(a.Key, a.Value); }
			XAML.dictionarytest = vdictionarytest;
			Test1.TestData1XAML.Setasdas(XAML, Data.asdas);
			return XAML;
		}
	}


	/**************************************************************************
	*	Service Contracts
	**************************************************************************/

	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect Service Compiler - BETA", "2.0.2000.0")]
	public interface ITestService
	{
		///<param name='asdsasd'></param>
		///<param name='assdasd'></param>
		[OperationContract(Action = "http://www.prospectivesoftware.com/Test1/TestService/SynchronousTest", ReplyAction = "http://www.prospectivesoftware.com/Test1/TestService/SynchronousTestResponse")]
		string SynchronousTest(string asdsasd, bool assdasd);

		///<param name='Callback'>The function to call when the operation is complete.</param>
		///<param name='AsyncState'>An object representing the state of the operation.</param>
		[OperationContract(Action = "http://www.prospectivesoftware.com/Test1/TestService/AsynchronousTestInvoke", ReplyAction = "http://www.prospectivesoftware.com/Test1/TestService/AsynchronousTestInvokeResponse")]
		IAsyncResult BeginAsynchronousTestInvoke(AsyncCallback Callback, object AsyncState);
		///<summary>Finalizes the asynchronous operation.</summary>
		///<returns>
		///
		///</returns>
		///<param name='result'>The result of the operation.</param>
		string EndAsynchronousTestInvoke(IAsyncResult result);

		[OperationContract(Action = "http://www.prospectivesoftware.com/Test1/TestService/AwaitableTestAsync", ReplyAction = "http://www.prospectivesoftware.com/Test1/TestService/AwaitableTestAsyncResponse")]
		System.Threading.Tasks.Task<Test1.TestData1> AwaitableTestAsync();

	}

	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect Service Compiler - BETA", "2.0.2000.0")]
	public interface ITestServiceCallback
	{
		[OperationContract(Action = "http://www.prospectivesoftware.com/Test1/TestService/SyncTest", ReplyAction = "http://www.prospectivesoftware.com/Test1/TestService/SyncTestResponse")]
		bool SyncTest();

		///<param name='Callback'>The function to call when the operation is complete.</param>
		///<param name='AsyncState'>An object representing the state of the operation.</param>
		[OperationContract(Action = "http://www.prospectivesoftware.com/Test1/TestService/AsyncTestInvoke", ReplyAction = "http://www.prospectivesoftware.com/Test1/TestService/AsyncTestInvokeResponse")]
		IAsyncResult BeginAsyncTestInvoke(AsyncCallback Callback, object AsyncState);
		///<summary>Finalizes the asynchronous operation.</summary>
		///<returns>
		///
		///</returns>
		///<param name='result'>The result of the operation.</param>
		sbyte EndAsyncTestInvoke(IAsyncResult result);

		[OperationContract(Action = "http://www.prospectivesoftware.com/Test1/TestService/AwaitTestAsync", ReplyAction = "http://www.prospectivesoftware.com/Test1/TestService/AwaitTestAsyncResponse")]
		System.Threading.Tasks.Task<ObservableCollection<Test1.TestData1>> AwaitTestAsync();

	}

	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect Service Compiler - BETA", "2.0.2000.0")]
	public interface ITestServiceChannel : ITestService, System.ServiceModel.IClientChannel
	{
	}

	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect Service Compiler - BETA", "2.0.2000.0")]
	public partial class TestServiceProxy : System.ServiceModel.ClientBase<ITestService>, ITestService
	{
		public TestServiceProxy(string endpointConfigurationName) : base(endpointConfigurationName)
		{
			onBeginAsynchronousTestDelegate = new BeginOperationDelegate(this.OnBeginAsynchronousTest);
			onEndAsynchronousTestDelegate = new EndOperationDelegate(this.OnEndAsynchronousTest);
			onAsynchronousTestCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnAsynchronousTestCompleted);
		}

		public TestServiceProxy(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
			onBeginAsynchronousTestDelegate = new BeginOperationDelegate(this.OnBeginAsynchronousTest);
			onEndAsynchronousTestDelegate = new EndOperationDelegate(this.OnEndAsynchronousTest);
			onAsynchronousTestCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnAsynchronousTestCompleted);
		}

		public TestServiceProxy(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
			onBeginAsynchronousTestDelegate = new BeginOperationDelegate(this.OnBeginAsynchronousTest);
			onEndAsynchronousTestDelegate = new EndOperationDelegate(this.OnEndAsynchronousTest);
			onAsynchronousTestCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnAsynchronousTestCompleted);
		}

		public TestServiceProxy(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
			onBeginAsynchronousTestDelegate = new BeginOperationDelegate(this.OnBeginAsynchronousTest);
			onEndAsynchronousTestDelegate = new EndOperationDelegate(this.OnEndAsynchronousTest);
			onAsynchronousTestCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnAsynchronousTestCompleted);
		}

	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect Service Compiler - BETA", "2.0.2000.0")]
	public partial class TestHost
	{
	}
		///<param name='asdsasd'></param>
		///<param name='assdasd'></param>
		public string SynchronousTest(string asdsasd, bool assdasd)
		{
			return base.Channel.SynchronousTest(asdsasd, assdasd);
		}

		private readonly BeginOperationDelegate onBeginAsynchronousTestDelegate;
		private readonly EndOperationDelegate onEndAsynchronousTestDelegate;
		private readonly System.Threading.SendOrPostCallback onAsynchronousTestCompletedDelegate;
		public Action<string, System.Exception, bool, object> AsynchronousTestCompleted;
		///<param name='Callback'>The function to call when the operation is complete.</param>
		///<param name='AsyncState'>An object representing the state of the operation.</param>
		IAsyncResult ITestService.BeginAsynchronousTestInvoke(AsyncCallback Callback, object AsyncState)
		{
			return base.Channel.BeginAsynchronousTestInvoke(Callback, AsyncState);
		}
		///<summary>Finalizes the asynchronous operation.</summary>
		///<returns>
		///
		///</returns>
		///<param name='result'>The result of the operation.</param>
		string ITestService.EndAsynchronousTestInvoke(IAsyncResult result)
		{
			return base.Channel.EndAsynchronousTestInvoke(result);
		}
		private IAsyncResult OnBeginAsynchronousTest(object[] Values, AsyncCallback Callback, object AsyncState)
		{
			return ((ITestService)this).BeginAsynchronousTestInvoke(Callback, AsyncState);
		}
		private object[] OnEndAsynchronousTest(IAsyncResult result)
		{
			return new object[] { ((ITestService)this).EndAsynchronousTestInvoke(result) };
		}
		private void OnAsynchronousTestCompleted(object state)
		{
			if (this.AsynchronousTestCompleted == null) return;
			InvokeAsyncCompletedEventArgs e = (InvokeAsyncCompletedEventArgs)state;
			this.AsynchronousTestCompleted((string)e.Results[0], e.Error, e.Cancelled, e.UserState);
		}
		public void AsynchronousTestInvoke()
		{
			this.AsynchronousTestInvoke(null);
		}
		///<param name='userState'>Allows the user of this function to distinguish between different calls.</param>
		public void AsynchronousTestInvoke(object userState)
		{
			InvokeAsync(this.onBeginAsynchronousTestDelegate, new object[] {  }, this.onEndAsynchronousTestDelegate, this.onAsynchronousTestCompletedDelegate, userState);
		}

		public System.Threading.Tasks.Task<Test1.TestData1> AwaitableTestAsync()
		{
			return base.Channel.AwaitableTestAsync();
		}

	}


	/**************************************************************************
	*	Service Bindings
	**************************************************************************/

	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect Service Compiler - BETA", "2.0.2000.0")]
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

	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect Service Compiler - BETA", "2.0.2000.0")]
	public partial class TestHost
	{
	}


}
namespace Test1.TestNS
{
}


#pragma warning restore 1591
