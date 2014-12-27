using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NETPath.Projects
{
	internal static class Storage
	{
		public static T Open<T>(string Path)
		{
			//Check the file to make sure it exists
			if (!File.Exists(Path))
				throw new FileNotFoundException("Unable to locate file '" + Path + "'");

			var kt = new List<Type>(new Type[] {
				typeof(Project), typeof(ProjectGenerationTarget), typeof(DataType), typeof(Namespace), typeof(Documentation),
				typeof(BindingSecurity), typeof(BindingSecurityBasicHTTP), typeof(BindingSecurityBasicHTTPS), typeof(BindingSecurityMSMQ), typeof(BindingSecurityMSMQIntegration), typeof(BindingSecurityNamedPipe), typeof(BindingSecurityPeerTCP), typeof(BindingSecurityTCP), typeof(BindingSecurityWebHTTP), typeof(BindingSecurityWSDualHTTP), typeof(BindingSecurityWSFederationHTTP), typeof(BindingSecurityWSHTTP),
				typeof(ServiceBinding), typeof(ServiceBindingBasicHTTP), typeof(ServiceBindingBasicHTTPS), typeof(ServiceBindingMSMQ), typeof(ServiceBindingMSMQIntegration), typeof(ServiceBindingNamedPipe), typeof(ServiceBindingNetHTTP), typeof(ServiceBindingNetHTTPS), typeof(ServiceBindingPeerTCP), typeof(ServiceBindingTCP), typeof(ServiceBindingUDP), typeof(ServiceBindingWebHTTP), typeof(ServiceBindingWS2007FederationHTTP), typeof(ServiceBindingWS2007HTTP), typeof(ServiceBindingWSDualHTTP), typeof(ServiceBindingWSFederationHTTP), typeof(ServiceBindingWSHTTP),
				typeof(Host), typeof(HostBehavior), typeof(HostCredentials), typeof(HostDebugBehavior), typeof(HostEndpoint), typeof(HostEndpointAddressHeader), typeof(HostMetadataBehavior), typeof(HostThrottlingBehavior), typeof(HostWebHTTPBehavior),
				typeof(Service), typeof(Operation), typeof(Method), typeof(Callback), typeof(MethodRest), typeof(DataChangeMethod), typeof(MethodParameter), typeof(Property),
				typeof(RestService), typeof(RestMethod), typeof(RestHttpConfiguration),
				typeof(Enum), typeof(EnumElement),
				typeof(Data), typeof(DataElement)
			});

			var dcs = new DataContractSerializer(typeof(T), new DataContractSerializerSettings() { KnownTypes = kt, MaxItemsInObjectGraph = Int32.MaxValue, IgnoreExtensionDataObject = true, SerializeReadOnlyTypes = true, PreserveObjectReferences = true });
			var fs = new FileStream(Path, FileMode.Open, FileAccess.Read);
			var tr = XmlReader.Create(fs, new XmlReaderSettings() { CloseInput = true, Async = false });

			var t = (T)dcs.ReadObject(tr);
			tr.Close();
			return t;
		}

		public static void Save<T>(string Path, T Data)
		{
			//Make sure the solution isn't read-only.
			if (File.Exists(Path))
			{
				var fi = new FileInfo(Path);
				if (fi.IsReadOnly)
					throw new IOException("The file '" + Path + "' is currently read-only. Please disable read-only mode on this file.");
			}

			var kt = new List<Type>(new Type[] {
				typeof(Project), typeof(ProjectGenerationTarget), typeof(DataType), typeof(Namespace), typeof(Documentation),
				typeof(BindingSecurity), typeof(BindingSecurityBasicHTTP), typeof(BindingSecurityBasicHTTPS), typeof(BindingSecurityMSMQ), typeof(BindingSecurityMSMQIntegration), typeof(BindingSecurityNamedPipe), typeof(BindingSecurityPeerTCP), typeof(BindingSecurityTCP), typeof(BindingSecurityWebHTTP), typeof(BindingSecurityWSDualHTTP), typeof(BindingSecurityWSFederationHTTP), typeof(BindingSecurityWSHTTP),
				typeof(ServiceBinding), typeof(ServiceBindingBasicHTTP), typeof(ServiceBindingBasicHTTPS), typeof(ServiceBindingMSMQ), typeof(ServiceBindingMSMQIntegration), typeof(ServiceBindingNamedPipe), typeof(ServiceBindingNetHTTP), typeof(ServiceBindingNetHTTPS), typeof(ServiceBindingPeerTCP), typeof(ServiceBindingTCP), typeof(ServiceBindingUDP), typeof(ServiceBindingWebHTTP), typeof(ServiceBindingWS2007FederationHTTP), typeof(ServiceBindingWS2007HTTP), typeof(ServiceBindingWSDualHTTP), typeof(ServiceBindingWSFederationHTTP), typeof(ServiceBindingWSHTTP),
				typeof(Host), typeof(HostBehavior), typeof(HostCredentials), typeof(HostDebugBehavior), typeof(HostEndpoint), typeof(HostEndpointAddressHeader), typeof(HostMetadataBehavior), typeof(HostThrottlingBehavior), typeof(HostWebHTTPBehavior),
				typeof(Service), typeof(Operation), typeof(Method), typeof(Callback), typeof(MethodRest), typeof(DataChangeMethod), typeof(MethodParameter), typeof(Property),
				typeof(RestService), typeof(RestMethod), typeof(RestHttpConfiguration),
				typeof(Enum), typeof(EnumElement),
				typeof(Data), typeof(DataElement)
			});

			var dcs = new DataContractSerializer(typeof(T), new DataContractSerializerSettings() { KnownTypes = kt, MaxItemsInObjectGraph = Int32.MaxValue, IgnoreExtensionDataObject = true, SerializeReadOnlyTypes = true, PreserveObjectReferences = true });
			var fs = new FileStream(Path, FileMode.Create);
			var xw = XmlWriter.Create(fs, new XmlWriterSettings() { Encoding = Encoding.UTF8, NewLineChars = Environment.NewLine, NewLineHandling = NewLineHandling.Entitize, CloseOutput = true, WriteEndDocumentOnClose = true, Indent = true, Async = false });
			dcs.WriteObject(xw, Data);
			xw.Flush();
			xw.Close();
		}

		public static byte[] Dump<T>(T Data)
		{
			var kt = new List<Type>(new Type[] {
				typeof(Project), typeof(ProjectGenerationTarget), typeof(DataType), typeof(Namespace), typeof(Documentation),
				typeof(BindingSecurity), typeof(BindingSecurityBasicHTTP), typeof(BindingSecurityBasicHTTPS), typeof(BindingSecurityMSMQ), typeof(BindingSecurityMSMQIntegration), typeof(BindingSecurityNamedPipe), typeof(BindingSecurityPeerTCP), typeof(BindingSecurityTCP), typeof(BindingSecurityWebHTTP), typeof(BindingSecurityWSDualHTTP), typeof(BindingSecurityWSFederationHTTP), typeof(BindingSecurityWSHTTP),
				typeof(ServiceBinding), typeof(ServiceBindingBasicHTTP), typeof(ServiceBindingBasicHTTPS), typeof(ServiceBindingMSMQ), typeof(ServiceBindingMSMQIntegration), typeof(ServiceBindingNamedPipe), typeof(ServiceBindingNetHTTP), typeof(ServiceBindingNetHTTPS), typeof(ServiceBindingPeerTCP), typeof(ServiceBindingTCP), typeof(ServiceBindingUDP), typeof(ServiceBindingWebHTTP), typeof(ServiceBindingWS2007FederationHTTP), typeof(ServiceBindingWS2007HTTP), typeof(ServiceBindingWSDualHTTP), typeof(ServiceBindingWSFederationHTTP), typeof(ServiceBindingWSHTTP),
				typeof(Host), typeof(HostBehavior), typeof(HostCredentials), typeof(HostDebugBehavior), typeof(HostEndpoint), typeof(HostEndpointAddressHeader), typeof(HostMetadataBehavior), typeof(HostThrottlingBehavior), typeof(HostWebHTTPBehavior),
				typeof(Service), typeof(Operation), typeof(Method), typeof(Callback), typeof(MethodRest), typeof(DataChangeMethod), typeof(MethodParameter), typeof(Property),
				typeof(RestService), typeof(RestMethod), typeof(RestHttpConfiguration),
				typeof(Enum), typeof(EnumElement),
				typeof(Data), typeof(DataElement)
			});

			var dcs = new DataContractSerializer(typeof(T), new DataContractSerializerSettings() { KnownTypes = kt, MaxItemsInObjectGraph = Int32.MaxValue, IgnoreExtensionDataObject = true, SerializeReadOnlyTypes = true, PreserveObjectReferences = true });
			var ms = new MemoryStream();
			var xw = XmlWriter.Create(ms, new XmlWriterSettings() { Encoding = Encoding.UTF8, NewLineChars = Environment.NewLine, NewLineHandling = NewLineHandling.Entitize, CloseOutput = true, WriteEndDocumentOnClose = true, Indent = true, Async = false });
			dcs.WriteObject(xw, Data);
			return ms.ToArray();
		}
	}
}