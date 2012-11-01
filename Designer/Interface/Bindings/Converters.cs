using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace WCFArchitect.Interface.Bindings
{
	[ValueConversion(typeof(System.ServiceModel.HostNameComparisonMode), typeof(int))]
	public class HostNameComparisonModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.HostNameComparisonMode)value;
			if (lt == System.ServiceModel.HostNameComparisonMode.StrongWildcard) return 0;
			if (lt == System.ServiceModel.HostNameComparisonMode.Exact) return 1;
			if (lt == System.ServiceModel.HostNameComparisonMode.WeakWildcard) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			if (lt == 1) return System.ServiceModel.HostNameComparisonMode.Exact;
			if (lt == 2) return System.ServiceModel.HostNameComparisonMode.WeakWildcard;
			return System.ServiceModel.HostNameComparisonMode.StrongWildcard;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.WSMessageEncoding), typeof(int))]
	public class WSMessageEncodingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.WSMessageEncoding)value;
			if (lt == System.ServiceModel.WSMessageEncoding.Text) return 0;
			if (lt == System.ServiceModel.WSMessageEncoding.Mtom) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.WSMessageEncoding.Text;
			if (lt == 1) return System.ServiceModel.WSMessageEncoding.Mtom;
			return System.ServiceModel.WSMessageEncoding.Text;
		}
	}

	[ValueConversion(typeof(Projects.ServiceBindingTextEncoding), typeof(int))]
	public class ServiceBindingTextEncodingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Projects.ServiceBindingTextEncoding)value;
			if (lt == Projects.ServiceBindingTextEncoding.ASCII) return 0;
			if (lt == Projects.ServiceBindingTextEncoding.UTF7) return 1;
			if (lt == Projects.ServiceBindingTextEncoding.UTF8) return 2;
			if (lt == Projects.ServiceBindingTextEncoding.Unicode) return 3;
			if (lt == Projects.ServiceBindingTextEncoding.UTF32) return 4;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return Projects.ServiceBindingTextEncoding.ASCII;
			if (lt == 1) return Projects.ServiceBindingTextEncoding.UTF7;
			if (lt == 2) return Projects.ServiceBindingTextEncoding.UTF8;
			if (lt == 3) return Projects.ServiceBindingTextEncoding.Unicode;
			if (lt == 4) return Projects.ServiceBindingTextEncoding.UTF32;
			return Projects.ServiceBindingTextEncoding.UTF8;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.TransferMode), typeof(int))]
	public class TransferModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.TransferMode)value;
			if (lt == System.ServiceModel.TransferMode.Buffered) return 0;
			if (lt == System.ServiceModel.TransferMode.Streamed) return 1;
			if (lt == System.ServiceModel.TransferMode.StreamedRequest) return 2;
			if (lt == System.ServiceModel.TransferMode.StreamedResponse) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.TransferMode.Buffered;
			if (lt == 1) return System.ServiceModel.TransferMode.Streamed;
			if (lt == 2) return System.ServiceModel.TransferMode.StreamedRequest;
			if (lt == 3) return System.ServiceModel.TransferMode.StreamedResponse;
			return System.ServiceModel.TransferMode.Buffered;
		}
	}

	[ValueConversion(typeof(Projects.ServiceBindingTransactionProtocol), typeof(int))]
	public class ServiceBindingTransactionProtocolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (Projects.ServiceBindingTransactionProtocol)value;
			if (lt == Projects.ServiceBindingTransactionProtocol.Default) return 0;
			if (lt == Projects.ServiceBindingTransactionProtocol.OleTransactions) return 1;
			if (lt == Projects.ServiceBindingTransactionProtocol.WSAtomicTransaction11) return 2;
			if (lt == Projects.ServiceBindingTransactionProtocol.WSAtomicTransactionOctober2004) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return Projects.ServiceBindingTransactionProtocol.Default;
			if (lt == 1) return Projects.ServiceBindingTransactionProtocol.OleTransactions;
			if (lt == 2) return Projects.ServiceBindingTransactionProtocol.WSAtomicTransaction11;
			if (lt == 3) return Projects.ServiceBindingTransactionProtocol.WSAtomicTransactionOctober2004;
			return Projects.ServiceBindingTransactionProtocol.Default;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.DeadLetterQueue), typeof(int))]
	public class DeadLetterQueueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.DeadLetterQueue)value;
			if (lt == System.ServiceModel.DeadLetterQueue.None) return 0;
			if (lt == System.ServiceModel.DeadLetterQueue.System) return 1;
			if (lt == System.ServiceModel.DeadLetterQueue.Custom) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.DeadLetterQueue.None;
			if (lt == 1) return System.ServiceModel.DeadLetterQueue.System;
			if (lt == 2) return System.ServiceModel.DeadLetterQueue.Custom;
			return System.ServiceModel.DeadLetterQueue.None;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.QueueTransferProtocol), typeof(int))]
	public class QueueTransferProtocolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.QueueTransferProtocol)value;
			if (lt == System.ServiceModel.QueueTransferProtocol.Native) return 0;
			if (lt == System.ServiceModel.QueueTransferProtocol.Srmp) return 1;
			if (lt == System.ServiceModel.QueueTransferProtocol.SrmpSecure) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.QueueTransferProtocol.Native;
			if (lt == 1) return System.ServiceModel.QueueTransferProtocol.Srmp;
			if (lt == 2) return System.ServiceModel.QueueTransferProtocol.SrmpSecure;
			return System.ServiceModel.QueueTransferProtocol.Native;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.ReceiveErrorHandling), typeof(int))]
	public class ReceiveErrorHandlingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.ReceiveErrorHandling)value;
			if (lt == System.ServiceModel.ReceiveErrorHandling.Fault) return 0;
			if (lt == System.ServiceModel.ReceiveErrorHandling.Drop) return 1;
			if (lt == System.ServiceModel.ReceiveErrorHandling.Reject) return 2;
			if (lt == System.ServiceModel.ReceiveErrorHandling.Move) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.ReceiveErrorHandling.Fault;
			if (lt == 1) return System.ServiceModel.ReceiveErrorHandling.Drop;
			if (lt == 2) return System.ServiceModel.ReceiveErrorHandling.Reject;
			if (lt == 3) return System.ServiceModel.ReceiveErrorHandling.Move;
			return System.ServiceModel.ReceiveErrorHandling.Fault;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat), typeof(int))]
	public class MsmqMessageSerializationFormatConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat)value;
			if (lt == System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.Xml) return 0;
			if (lt == System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.Binary) return 1;
			if (lt == System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.ActiveX) return 2;
			if (lt == System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.ByteArray) return 3;
			if (lt == System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.Stream) return 4;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (int)value;
			if (lt == 0) return System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.Xml;
			if (lt == 1) return System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.Binary;
			if (lt == 2) return System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.ActiveX;
			if (lt == 3) return System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.ByteArray;
			if (lt == 4) return System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.Stream;
			return System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.Xml;
		}
	}
	[ValueConversion(typeof(Projects.BindingSecurityAlgorithmSuite), typeof(int))]
	public class BindingSecurityAlgorithmSuiteConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Projects.BindingSecurityAlgorithmSuite lt = (Projects.BindingSecurityAlgorithmSuite)value;
			if (lt == Projects.BindingSecurityAlgorithmSuite.Default) return 0;
			if (lt == Projects.BindingSecurityAlgorithmSuite.Basic128) return 1;
			if (lt == Projects.BindingSecurityAlgorithmSuite.Basic128Rsa15) return 2;
			if (lt == Projects.BindingSecurityAlgorithmSuite.Basic128Sha256) return 3;
			if (lt == Projects.BindingSecurityAlgorithmSuite.Basic128Sha256Rsa15) return 4;
			if (lt == Projects.BindingSecurityAlgorithmSuite.Basic192) return 1;
			if (lt == Projects.BindingSecurityAlgorithmSuite.Basic192Rsa15) return 2;
			if (lt == Projects.BindingSecurityAlgorithmSuite.Basic192Sha256) return 3;
			if (lt == Projects.BindingSecurityAlgorithmSuite.Basic192Sha256Rsa15) return 4;
			if (lt == Projects.BindingSecurityAlgorithmSuite.Basic256) return 1;
			if (lt == Projects.BindingSecurityAlgorithmSuite.Basic256Rsa15) return 2;
			if (lt == Projects.BindingSecurityAlgorithmSuite.Basic256Sha256) return 3;
			if (lt == Projects.BindingSecurityAlgorithmSuite.Basic256Sha256Rsa15) return 4;
			if (lt == Projects.BindingSecurityAlgorithmSuite.TripleDes) return 1;
			if (lt == Projects.BindingSecurityAlgorithmSuite.TripleDesRsa15) return 2;
			if (lt == Projects.BindingSecurityAlgorithmSuite.TripleDesSha256) return 3;
			if (lt == Projects.BindingSecurityAlgorithmSuite.TripleDesSha256Rsa15) return 4;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return Projects.BindingSecurityAlgorithmSuite.Default;
			if (lt == 1) return Projects.BindingSecurityAlgorithmSuite.Basic128;
			if (lt == 2) return Projects.BindingSecurityAlgorithmSuite.Basic128Rsa15;
			if (lt == 3) return Projects.BindingSecurityAlgorithmSuite.Basic128Sha256;
			if (lt == 4) return Projects.BindingSecurityAlgorithmSuite.Basic128Sha256Rsa15;
			if (lt == 1) return Projects.BindingSecurityAlgorithmSuite.Basic192;
			if (lt == 2) return Projects.BindingSecurityAlgorithmSuite.Basic192Rsa15;
			if (lt == 3) return Projects.BindingSecurityAlgorithmSuite.Basic192Sha256;
			if (lt == 4) return Projects.BindingSecurityAlgorithmSuite.Basic192Sha256Rsa15;
			if (lt == 1) return Projects.BindingSecurityAlgorithmSuite.Basic256;
			if (lt == 2) return Projects.BindingSecurityAlgorithmSuite.Basic256Rsa15;
			if (lt == 3) return Projects.BindingSecurityAlgorithmSuite.Basic256Sha256;
			if (lt == 4) return Projects.BindingSecurityAlgorithmSuite.Basic256Sha256Rsa15;
			if (lt == 1) return Projects.BindingSecurityAlgorithmSuite.TripleDes;
			if (lt == 2) return Projects.BindingSecurityAlgorithmSuite.TripleDesRsa15;
			if (lt == 3) return Projects.BindingSecurityAlgorithmSuite.TripleDesSha256;
			if (lt == 4) return Projects.BindingSecurityAlgorithmSuite.TripleDesSha256Rsa15;
			return Projects.BindingSecurityAlgorithmSuite.Default;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.BasicHttpSecurityMode), typeof(int))]
	public class BasicHttpSecurityModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.BasicHttpSecurityMode lt = (System.ServiceModel.BasicHttpSecurityMode)value;
			if (lt == System.ServiceModel.BasicHttpSecurityMode.None) return 0;
			if (lt == System.ServiceModel.BasicHttpSecurityMode.Transport) return 1;
			if (lt == System.ServiceModel.BasicHttpSecurityMode.Message) return 2;
			if (lt == System.ServiceModel.BasicHttpSecurityMode.TransportWithMessageCredential) return 3;
			if (lt == System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly) return 4;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.BasicHttpSecurityMode.None;
			if (lt == 1) return System.ServiceModel.BasicHttpSecurityMode.Transport;
			if (lt == 2) return System.ServiceModel.BasicHttpSecurityMode.Message;
			if (lt == 3) return System.ServiceModel.BasicHttpSecurityMode.TransportWithMessageCredential;
			if (lt == 4) return System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;
			return System.ServiceModel.BasicHttpSecurityMode.None;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.BasicHttpMessageCredentialType), typeof(int))]
	public class BasicHttpMessageCredentialTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.BasicHttpMessageCredentialType lt = (System.ServiceModel.BasicHttpMessageCredentialType)value;
			if (lt == System.ServiceModel.BasicHttpMessageCredentialType.UserName) return 0;
			if (lt == System.ServiceModel.BasicHttpMessageCredentialType.Certificate) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.BasicHttpMessageCredentialType.UserName;
			if (lt == 1) return System.ServiceModel.BasicHttpMessageCredentialType.Certificate;
			return System.ServiceModel.BasicHttpMessageCredentialType.UserName;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.HttpClientCredentialType), typeof(int))]
	public class HttpClientCredentialTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.HttpClientCredentialType lt = (System.ServiceModel.HttpClientCredentialType)value;
			if (lt == System.ServiceModel.HttpClientCredentialType.None) return 0;
			if (lt == System.ServiceModel.HttpClientCredentialType.Basic) return 1;
			if (lt == System.ServiceModel.HttpClientCredentialType.Digest) return 2;
			if (lt == System.ServiceModel.HttpClientCredentialType.Ntlm) return 3;
			if (lt == System.ServiceModel.HttpClientCredentialType.Windows) return 4;
			if (lt == System.ServiceModel.HttpClientCredentialType.Certificate) return 5;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.HttpClientCredentialType.None;
			if (lt == 1) return System.ServiceModel.HttpClientCredentialType.Basic;
			if (lt == 2) return System.ServiceModel.HttpClientCredentialType.Digest;
			if (lt == 3) return System.ServiceModel.HttpClientCredentialType.Ntlm;
			if (lt == 4) return System.ServiceModel.HttpClientCredentialType.Windows;
			if (lt == 5) return System.ServiceModel.HttpClientCredentialType.Certificate;
			return System.ServiceModel.HttpClientCredentialType.None;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.HttpProxyCredentialType), typeof(int))]
	public class HttpProxyCredentialTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.HttpProxyCredentialType lt = (System.ServiceModel.HttpProxyCredentialType)value;
			if (lt == System.ServiceModel.HttpProxyCredentialType.None) return 0;
			if (lt == System.ServiceModel.HttpProxyCredentialType.Basic) return 1;
			if (lt == System.ServiceModel.HttpProxyCredentialType.Digest) return 2;
			if (lt == System.ServiceModel.HttpProxyCredentialType.Ntlm) return 3;
			if (lt == System.ServiceModel.HttpProxyCredentialType.Windows) return 4;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.HttpProxyCredentialType.None;
			if (lt == 1) return System.ServiceModel.HttpProxyCredentialType.Basic;
			if (lt == 2) return System.ServiceModel.HttpProxyCredentialType.Digest;
			if (lt == 3) return System.ServiceModel.HttpProxyCredentialType.Ntlm;
			if (lt == 4) return System.ServiceModel.HttpProxyCredentialType.Windows;
			return System.ServiceModel.HttpProxyCredentialType.None;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.SecurityMode), typeof(int))]
	public class SecurityModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.SecurityMode lt = (System.ServiceModel.SecurityMode)value;
			if (lt == System.ServiceModel.SecurityMode.None) return 0;
			if (lt == System.ServiceModel.SecurityMode.Transport) return 1;
			if (lt == System.ServiceModel.SecurityMode.Message) return 2;
			if (lt == System.ServiceModel.SecurityMode.TransportWithMessageCredential) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.SecurityMode.None;
			if (lt == 1) return System.ServiceModel.SecurityMode.Transport;
			if (lt == 2) return System.ServiceModel.SecurityMode.Message;
			if (lt == 3) return System.ServiceModel.SecurityMode.TransportWithMessageCredential;
			return System.ServiceModel.SecurityMode.None;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.MessageCredentialType), typeof(int))]
	public class MessageCredentialTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.MessageCredentialType lt = (System.ServiceModel.MessageCredentialType)value;
			if (lt == System.ServiceModel.MessageCredentialType.None) return 0;
			if (lt == System.ServiceModel.MessageCredentialType.Windows) return 1;
			if (lt == System.ServiceModel.MessageCredentialType.UserName) return 2;
			if (lt == System.ServiceModel.MessageCredentialType.Certificate) return 3;
			if (lt == System.ServiceModel.MessageCredentialType.IssuedToken) return 4;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.MessageCredentialType.None;
			if (lt == 1) return System.ServiceModel.MessageCredentialType.Windows;
			if (lt == 2) return System.ServiceModel.MessageCredentialType.UserName;
			if (lt == 3) return System.ServiceModel.MessageCredentialType.Certificate;
			if (lt == 4) return System.ServiceModel.MessageCredentialType.IssuedToken;
			return System.ServiceModel.MessageCredentialType.None;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.WSDualHttpSecurityMode), typeof(int))]
	public class WSDualHttpSecurityModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.WSDualHttpSecurityMode lt = (System.ServiceModel.WSDualHttpSecurityMode)value;
			if (lt == System.ServiceModel.WSDualHttpSecurityMode.None) return 0;
			if (lt == System.ServiceModel.WSDualHttpSecurityMode.Message) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.WSDualHttpSecurityMode.None;
			if (lt == 1) return System.ServiceModel.WSDualHttpSecurityMode.Message;
			return System.ServiceModel.WSDualHttpSecurityMode.None;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.WSFederationHttpSecurityMode), typeof(int))]
	public class WSFederationHttpSecurityModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.WSFederationHttpSecurityMode lt = (System.ServiceModel.WSFederationHttpSecurityMode)value;
			if (lt == System.ServiceModel.WSFederationHttpSecurityMode.None) return 0;
			if (lt == System.ServiceModel.WSFederationHttpSecurityMode.Message) return 1;
			if (lt == System.ServiceModel.WSFederationHttpSecurityMode.TransportWithMessageCredential) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.WSFederationHttpSecurityMode.None;
			if (lt == 1) return System.ServiceModel.WSFederationHttpSecurityMode.Message;
			if (lt == 2) return System.ServiceModel.WSFederationHttpSecurityMode.TransportWithMessageCredential;
			return System.ServiceModel.WSFederationHttpSecurityMode.None;
		}
	}

	[ValueConversion(typeof(System.IdentityModel.Tokens.SecurityKeyType), typeof(int))]
	public class SecurityKeyTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.IdentityModel.Tokens.SecurityKeyType lt = (System.IdentityModel.Tokens.SecurityKeyType)value;
			if (lt == System.IdentityModel.Tokens.SecurityKeyType.SymmetricKey) return 0;
			if (lt == System.IdentityModel.Tokens.SecurityKeyType.AsymmetricKey) return 1;
			if (lt == System.IdentityModel.Tokens.SecurityKeyType.BearerKey) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.IdentityModel.Tokens.SecurityKeyType.SymmetricKey;
			if (lt == 1) return System.IdentityModel.Tokens.SecurityKeyType.AsymmetricKey;
			if (lt == 2) return System.IdentityModel.Tokens.SecurityKeyType.BearerKey;
			return System.IdentityModel.Tokens.SecurityKeyType.SymmetricKey;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.TcpClientCredentialType), typeof(int))]
	public class TcpClientCredentialTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.TcpClientCredentialType lt = (System.ServiceModel.TcpClientCredentialType)value;
			if (lt == System.ServiceModel.TcpClientCredentialType.None) return 0;
			if (lt == System.ServiceModel.TcpClientCredentialType.Windows) return 1;
			if (lt == System.ServiceModel.TcpClientCredentialType.Certificate) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.TcpClientCredentialType.None;
			if (lt == 1) return System.ServiceModel.TcpClientCredentialType.Windows;
			if (lt == 2) return System.ServiceModel.TcpClientCredentialType.Certificate;
			return System.ServiceModel.TcpClientCredentialType.None;
		}
	}

	[ValueConversion(typeof(System.Net.Security.ProtectionLevel), typeof(int))]
	public class ProtectionLevelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.Net.Security.ProtectionLevel lt = (System.Net.Security.ProtectionLevel)value;
			if (lt == System.Net.Security.ProtectionLevel.None) return 0;
			if (lt == System.Net.Security.ProtectionLevel.Sign) return 1;
			if (lt == System.Net.Security.ProtectionLevel.EncryptAndSign) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.Net.Security.ProtectionLevel.None;
			if (lt == 1) return System.Net.Security.ProtectionLevel.Sign;
			if (lt == 2) return System.Net.Security.ProtectionLevel.EncryptAndSign;
			return System.Net.Security.ProtectionLevel.None;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.NetNamedPipeSecurityMode), typeof(int))]
	public class NetNamedPipeSecurityModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.NetNamedPipeSecurityMode lt = (System.ServiceModel.NetNamedPipeSecurityMode)value;
			if (lt == System.ServiceModel.NetNamedPipeSecurityMode.None) return 0;
			if (lt == System.ServiceModel.NetNamedPipeSecurityMode.Transport) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.NetNamedPipeSecurityMode.None;
			if (lt == 1) return System.ServiceModel.NetNamedPipeSecurityMode.Transport;
			return System.ServiceModel.NetNamedPipeSecurityMode.None;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.NetMsmqSecurityMode), typeof(int))]
	public class NetMsmqSecurityModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.NetMsmqSecurityMode lt = (System.ServiceModel.NetMsmqSecurityMode)value;
			if (lt == System.ServiceModel.NetMsmqSecurityMode.None) return 0;
			if (lt == System.ServiceModel.NetMsmqSecurityMode.Transport) return 1;
			if (lt == System.ServiceModel.NetMsmqSecurityMode.Message) return 2;
			if (lt == System.ServiceModel.NetMsmqSecurityMode.Both) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.NetMsmqSecurityMode.None;
			if (lt == 1) return System.ServiceModel.NetMsmqSecurityMode.Transport;
			if (lt == 2) return System.ServiceModel.NetMsmqSecurityMode.Message;
			if (lt == 3) return System.ServiceModel.NetMsmqSecurityMode.Both;
			return System.ServiceModel.NetMsmqSecurityMode.None;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.MsmqAuthenticationMode), typeof(int))]
	public class MsmqAuthenticationModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.MsmqAuthenticationMode lt = (System.ServiceModel.MsmqAuthenticationMode)value;
			if (lt == System.ServiceModel.MsmqAuthenticationMode.None) return 0;
			if (lt == System.ServiceModel.MsmqAuthenticationMode.WindowsDomain) return 1;
			if (lt == System.ServiceModel.MsmqAuthenticationMode.Certificate) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.MsmqAuthenticationMode.None;
			if (lt == 1) return System.ServiceModel.MsmqAuthenticationMode.WindowsDomain;
			if (lt == 2) return System.ServiceModel.MsmqAuthenticationMode.Certificate;
			return System.ServiceModel.MsmqAuthenticationMode.None;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.MsmqEncryptionAlgorithm), typeof(int))]
	public class MsmqEncryptionAlgorithmConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.MsmqEncryptionAlgorithm lt = (System.ServiceModel.MsmqEncryptionAlgorithm)value;
			if (lt == System.ServiceModel.MsmqEncryptionAlgorithm.RC4Stream) return 0;
			if (lt == System.ServiceModel.MsmqEncryptionAlgorithm.Aes) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.MsmqEncryptionAlgorithm.RC4Stream;
			if (lt == 1) return System.ServiceModel.MsmqEncryptionAlgorithm.Aes;
			return System.ServiceModel.MsmqEncryptionAlgorithm.RC4Stream;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.MsmqSecureHashAlgorithm), typeof(int))]
	public class MsmqSecureHashAlgorithmConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.MsmqSecureHashAlgorithm lt = (System.ServiceModel.MsmqSecureHashAlgorithm)value;
			if (lt == System.ServiceModel.MsmqSecureHashAlgorithm.MD5) return 0;
			if (lt == System.ServiceModel.MsmqSecureHashAlgorithm.Sha1) return 1;
			if (lt == System.ServiceModel.MsmqSecureHashAlgorithm.Sha256) return 2;
			if (lt == System.ServiceModel.MsmqSecureHashAlgorithm.Sha512) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.MsmqSecureHashAlgorithm.MD5;
			if (lt == 1) return System.ServiceModel.MsmqSecureHashAlgorithm.Sha1;
			if (lt == 2) return System.ServiceModel.MsmqSecureHashAlgorithm.Sha256;
			if (lt == 3) return System.ServiceModel.MsmqSecureHashAlgorithm.Sha512;
			return System.ServiceModel.MsmqSecureHashAlgorithm.MD5;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.PeerTransportCredentialType), typeof(int))]
	public class PeerTransportCredentialTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.PeerTransportCredentialType lt = (System.ServiceModel.PeerTransportCredentialType)value;
			if (lt == System.ServiceModel.PeerTransportCredentialType.Password) return 0;
			if (lt == System.ServiceModel.PeerTransportCredentialType.Certificate) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.PeerTransportCredentialType.Password;
			if (lt == 1) return System.ServiceModel.PeerTransportCredentialType.Certificate;
			return System.ServiceModel.PeerTransportCredentialType.Password;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.WebHttpSecurityMode), typeof(int))]
	public class WebHttpSecurityModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.WebHttpSecurityMode lt = (System.ServiceModel.WebHttpSecurityMode)value;
			if (lt == System.ServiceModel.WebHttpSecurityMode.None) return 0;
			if (lt == System.ServiceModel.WebHttpSecurityMode.Transport) return 1;
			if (lt == System.ServiceModel.WebHttpSecurityMode.TransportCredentialOnly) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.WebHttpSecurityMode.None;
			if (lt == 1) return System.ServiceModel.WebHttpSecurityMode.Transport;
			if (lt == 2) return System.ServiceModel.WebHttpSecurityMode.TransportCredentialOnly;
			return System.ServiceModel.WebHttpSecurityMode.None;
		}
	}

	[ValueConversion(typeof(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode), typeof(int))]
	public class MsmqIntegrationSecurityModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode lt = (System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode)value;
			if (lt == System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode.None) return 0;
			if (lt == System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode.Transport) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode.None;
			if (lt == 1) return System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode.Transport;
			return System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode.None;
		}
	}

	[ValueConversion(typeof(Projects.BindingSecurity), typeof(string))]
	public class SecurityTypeImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "pack://application:,,,/WCFArchitect;component/Icons/Blank.png";
			Projects.BindingSecurity lt = (Projects.BindingSecurity)value;
			Type valueType = lt.GetType();
			if (valueType == typeof(Projects.BindingSecurityBasicHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png";
			if (valueType == typeof(Projects.BindingSecurityWSHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WSHTTP.png";
			if (valueType == typeof(Projects.BindingSecurityWSDualHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WSDualHTTP.png";
			if (valueType == typeof(Projects.BindingSecurityWSFederationHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WSFederationHTTP.png";
			if (valueType == typeof(Projects.BindingSecurityTCP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/TCP.png";
			if (valueType == typeof(Projects.BindingSecurityNamedPipe)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/NamedPipe.png";
			if (valueType == typeof(Projects.BindingSecurityMSMQ)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQ.png";
			if (valueType == typeof(Projects.BindingSecurityPeerTCP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/PeerTCP.png";
			if (valueType == typeof(Projects.BindingSecurityWebHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WebHTTP.png";
			if (valueType == typeof(Projects.BindingSecurityMSMQIntegration)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQIntegration.png";
			return "pack://application:,,,/WCFArchitect;component/Icons/Blank.png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(Projects.BindingSecurity), typeof(string))]
	public class SecurityTypeNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "";
			Projects.BindingSecurity lt = (Projects.BindingSecurity)value;
			Type valueType = lt.GetType();
			if (valueType == typeof(Projects.BindingSecurityBasicHTTP)) return "Basic HTTP Security";
			if (valueType == typeof(Projects.BindingSecurityWSHTTP)) return "WS HTTP Security";
			if (valueType == typeof(Projects.BindingSecurityWSDualHTTP)) return "WS Dual HTTP Security";
			if (valueType == typeof(Projects.BindingSecurityWSFederationHTTP)) return "WS Federation Security";
			if (valueType == typeof(Projects.BindingSecurityTCP)) return "TCP Security";
			if (valueType == typeof(Projects.BindingSecurityNamedPipe)) return "Named Pipe Security";
			if (valueType == typeof(Projects.BindingSecurityMSMQ)) return "MSMQ Security";
			if (valueType == typeof(Projects.BindingSecurityPeerTCP)) return "Peer TCP Security";
			if (valueType == typeof(Projects.BindingSecurityWebHTTP)) return "Web HTTP Security";
			if (valueType == typeof(Projects.BindingSecurityMSMQIntegration)) return "MSMQ Integration Security";
			return "pack://application:,,,/WCFArchitect;component/Icons/Blank.png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(System.ServiceModel.Channels.WebSocketTransportUsage), typeof(int))]
	public class WebSocketTransportUsageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (System.ServiceModel.Channels.WebSocketTransportUsage)value;
			if (lt == System.ServiceModel.Channels.WebSocketTransportUsage.WhenDuplex) return 0;
			if (lt == System.ServiceModel.Channels.WebSocketTransportUsage.Always) return 1;
			if (lt == System.ServiceModel.Channels.WebSocketTransportUsage.Never) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.Channels.WebSocketTransportUsage.WhenDuplex;
			if (lt == 1) return System.ServiceModel.Channels.WebSocketTransportUsage.Always;
			if (lt == 2) return System.ServiceModel.Channels.WebSocketTransportUsage.Never;
			return System.ServiceModel.Channels.WebSocketTransportUsage.WhenDuplex;
		}
	}
}