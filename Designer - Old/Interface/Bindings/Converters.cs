using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace WCFArchitect.Interface.Project.Bindings
{
	[ValueConversion(typeof(System.ServiceModel.HostNameComparisonMode), typeof(int))]
	public class HostNameComparisonModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.ServiceModel.HostNameComparisonMode lt = (System.ServiceModel.HostNameComparisonMode)value;
			if (lt == System.ServiceModel.HostNameComparisonMode.StrongWildcard) return 0;
			if (lt == System.ServiceModel.HostNameComparisonMode.Exact) return 1;
			if (lt == System.ServiceModel.HostNameComparisonMode.WeakWildcard) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
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
			System.ServiceModel.WSMessageEncoding lt = (System.ServiceModel.WSMessageEncoding)value;
			if (lt == System.ServiceModel.WSMessageEncoding.Text) return 0;
			if (lt == System.ServiceModel.WSMessageEncoding.Mtom) return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
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
			Projects.ServiceBindingTextEncoding lt = (Projects.ServiceBindingTextEncoding)value;
			if (lt == Projects.ServiceBindingTextEncoding.ASCII) return 0;
			if (lt == Projects.ServiceBindingTextEncoding.UTF7) return 1;
			if (lt == Projects.ServiceBindingTextEncoding.UTF8) return 2;
			if (lt == Projects.ServiceBindingTextEncoding.Unicode) return 3;
			if (lt == Projects.ServiceBindingTextEncoding.UTF32) return 4;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
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
			System.ServiceModel.TransferMode lt = (System.ServiceModel.TransferMode)value;
			if (lt == System.ServiceModel.TransferMode.Buffered) return 0;
			if (lt == System.ServiceModel.TransferMode.Streamed) return 1;
			if (lt == System.ServiceModel.TransferMode.StreamedRequest) return 2;
			if (lt == System.ServiceModel.TransferMode.StreamedResponse) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
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
			Projects.ServiceBindingTransactionProtocol lt = (Projects.ServiceBindingTransactionProtocol)value;
			if (lt == Projects.ServiceBindingTransactionProtocol.Default) return 0;
			if (lt == Projects.ServiceBindingTransactionProtocol.OleTransactions) return 1;
			if (lt == Projects.ServiceBindingTransactionProtocol.WSAtomicTransaction11) return 2;
			if (lt == Projects.ServiceBindingTransactionProtocol.WSAtomicTransactionOctober2004) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
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
			System.ServiceModel.DeadLetterQueue lt = (System.ServiceModel.DeadLetterQueue)value;
			if (lt == System.ServiceModel.DeadLetterQueue.None) return 0;
			if (lt == System.ServiceModel.DeadLetterQueue.System) return 1;
			if (lt == System.ServiceModel.DeadLetterQueue.Custom) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
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
			System.ServiceModel.QueueTransferProtocol lt = (System.ServiceModel.QueueTransferProtocol)value;
			if (lt == System.ServiceModel.QueueTransferProtocol.Native) return 0;
			if (lt == System.ServiceModel.QueueTransferProtocol.Srmp) return 1;
			if (lt == System.ServiceModel.QueueTransferProtocol.SrmpSecure) return 2;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
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
			System.ServiceModel.ReceiveErrorHandling lt = (System.ServiceModel.ReceiveErrorHandling)value;
			if (lt == System.ServiceModel.ReceiveErrorHandling.Fault) return 0;
			if (lt == System.ServiceModel.ReceiveErrorHandling.Drop) return 1;
			if (lt == System.ServiceModel.ReceiveErrorHandling.Reject) return 2;
			if (lt == System.ServiceModel.ReceiveErrorHandling.Move) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
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
			System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat lt = (System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat)value;
			if (lt == System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.Xml) return 0;
			if (lt == System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.Binary) return 1;
			if (lt == System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.ActiveX) return 2;
			if (lt == System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.ByteArray) return 3;
			if (lt == System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.Stream) return 4;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.Xml;
			if (lt == 1) return System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.Binary;
			if (lt == 2) return System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.ActiveX;
			if (lt == 3) return System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.ByteArray;
			if (lt == 4) return System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.Stream;
			return System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat.Xml;
		}
	}

	[ValueConversion(typeof(Compiler.CompileMessageSeverity), typeof(string))]
	public class BindingTypeImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "pack://application:,,,/WCFArchitect;component/Icons/Blank.png";
			Projects.ServiceBinding lt = (Projects.ServiceBinding)value;
			Type valueType = lt.GetType();
			if (valueType == typeof(Projects.ServiceBindingBasicHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/BasicHTTP.png";
			if (valueType == typeof(Projects.ServiceBindingWSHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WSHTTP.png";
			if (valueType == typeof(Projects.ServiceBindingWS2007HTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WSHTTP.png";
			if (valueType == typeof(Projects.ServiceBindingWSDualHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WSDualHTTP.png";
			if (valueType == typeof(Projects.ServiceBindingWSFederationHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WSFederationHTTP.png";
			if (valueType == typeof(Projects.ServiceBindingWS2007FederationHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WSFederationHTTP.png";
			if (valueType == typeof(Projects.ServiceBindingTCP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/TCP.png";
			if (valueType == typeof(Projects.ServiceBindingNamedPipe)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/NamedPipe.png";
			if (valueType == typeof(Projects.ServiceBindingMSMQ)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQ.png";
			if (valueType == typeof(Projects.ServiceBindingPeerTCP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/PeerTCP.png";
			if (valueType == typeof(Projects.ServiceBindingWebHTTP)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/WebHTTP.png";
			if (valueType == typeof(Projects.ServiceBindingMSMQIntegration)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/MSMQIntegration.png";
			return "pack://application:,,,/WCFArchitect;component/Icons/Blank.png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(Compiler.CompileMessageSeverity), typeof(string))]
	public class BindingTypeNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "";
			Projects.ServiceBinding lt = (Projects.ServiceBinding)value;
			Type valueType = lt.GetType();
			if (valueType == typeof(Projects.ServiceBindingBasicHTTP)) return "Basic HTTP Binding";
			if (valueType == typeof(Projects.ServiceBindingWSHTTP)) return "WS HTTP Binding";
			if (valueType == typeof(Projects.ServiceBindingWS2007HTTP)) return "WS 2007 HTTP Binding";
			if (valueType == typeof(Projects.ServiceBindingWSDualHTTP)) return "WS Dual HTTP Binding";
			if (valueType == typeof(Projects.ServiceBindingWSFederationHTTP)) return "WS Federation Binding";
			if (valueType == typeof(Projects.ServiceBindingWS2007FederationHTTP)) return "WS 2007 Federation Binding";
			if (valueType == typeof(Projects.ServiceBindingTCP)) return "TCP Binding";
			if (valueType == typeof(Projects.ServiceBindingNamedPipe)) return "Named Pipe Binding";
			if (valueType == typeof(Projects.ServiceBindingMSMQ)) return "MSMQ Binding";
			if (valueType == typeof(Projects.ServiceBindingPeerTCP)) return "Peer TCP Binding";
			if (valueType == typeof(Projects.ServiceBindingWebHTTP)) return "Web HTTP Security";
			if (valueType == typeof(Projects.ServiceBindingMSMQIntegration)) return "MSMQ Integration Binding";
			return "pack://application:,,,/WCFArchitect;component/Icons/Blank.png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}