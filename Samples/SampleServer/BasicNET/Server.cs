//---------------------------------------------------------------------------
// This code was generated by a tool. Changes to this file may cause 
// incorrect behavior and will be lost if the code is regenerated.
//
// WCF Architect Version:	2.0.0.12504
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
namespace WCFArchitect.SampleServer.BasicNET
{
	/**************************************************************************
	*	Service Contracts
	**************************************************************************/

	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect .NET CSharp Generator - BETA", "2.0.0.12504")]
	[ServiceContract(CallbackContract = typeof(ITestNETCallback), SessionMode = System.ServiceModel.SessionMode.Allowed, Namespace = "http://tempuri.org/WCFArchitect/SampleServer/BasicNET/")]
	public interface ITestNET
	{
		[OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.None)]
		WCFArchitect.SampleServer.BasicWinRT.Customer RefTestAsync();

	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect .NET CSharp Generator - BETA", "2.0.0.12504")]
	public interface ITestNETCallback
	{
		[OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.None)]
		WCFArchitect.SampleServer.BasicWinRT.Customer RetTestCallbackAsync();

	}
	[System.CodeDom.Compiler.GeneratedCodeAttribute("WCF Architect .NET CSharp Generator - BETA", "2.0.0.12504")]
	public partial class TestNETCallback : ITestNETCallback
	{

		private readonly ITestNETCallback __callback;

		public TestNETCallback()
		{
			__callback = System.ServiceModel.OperationContext.Current.GetCallbackChannel<ITestNETCallback>();
		}

		public TestNETCallback(ITestNETCallback callback)
		{
			__callback = callback;
		}

		public WCFArchitect.SampleServer.BasicWinRT.Customer RetTestCallbackAsync()
		{
			return __callback.RetTestCallbackAsync();
		}

	}


}

#pragma warning restore 1591
