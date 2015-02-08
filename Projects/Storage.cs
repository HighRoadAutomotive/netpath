using NETPath.Projects.Wcf;
using NETPath.Projects.WebApi;
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
				typeof(ProjectGenerationTarget), typeof(DataType), typeof(Documentation),
				typeof(Project), typeof(WcfProject), typeof(WebApiProject),
				typeof(Namespace), typeof(WcfNamespace), typeof(WebApiNamespace),
				typeof(WcfSecurity), typeof(WcfSecurityBasicHTTP), typeof(WcfSecurityBasicHTTPS), typeof(WcfSecurityMSMQ), typeof(WcfSecurityMSMQIntegration), typeof(WcfSecurityNamedPipe), typeof(WcfSecurityPeerTCP), typeof(WcfSecurityTCP), typeof(WcfSecurityWebHTTP), typeof(WcfSecurityWSDualHTTP), typeof(WcfSecurityWSFederationHTTP), typeof(WcfSecurityWSHTTP),
				typeof(WcfBinding), typeof(WcfBindingBasicHTTP), typeof(WcfBindingBasicHTTPS), typeof(WcfBindingMSMQ), typeof(WcfBindingMSMQIntegration), typeof(WcfBindingNamedPipe), typeof(WcfBindingNetHTTP), typeof(WcfBindingNetHTTPS), typeof(WcfBindingPeerTCP), typeof(WcfBindingTCP), typeof(WcfBindingUDP), typeof(WcfBindingWebHTTP), typeof(WcfBindingWS2007FederationHTTP), typeof(WcfBindingWS2007HTTP), typeof(WcfBindingWSDualHTTP), typeof(WcfBindingWSFederationHTTP), typeof(WcfBindingWSHTTP),
				typeof(WcfHost), typeof(WcfHostBehavior), typeof(WcfHostCredentials), typeof(WcfHostDebugBehavior), typeof(WcfHostEndpoint), typeof(WcfHostEndpointAddressHeader), typeof(WcfHostMetadataBehavior), typeof(WcfHostThrottlingBehavior),
				typeof(WcfService), typeof(WcfOperation), typeof(WcfMethod), typeof(WcfCallback), typeof(WcfDataChangeMethod), typeof(WcfMethodParameter), typeof(WcfProperty),
				typeof(WcfData), typeof(WcfDataElement),
				typeof(WebApiService), typeof(WebApiMethod), typeof(WebApiHttpConfiguration), typeof(WebApiRouteParameter), typeof(WebApiMethodParameter),
				typeof(WebApiData), typeof(WebApiDataElement),
				typeof(Enum), typeof(EnumElement),
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
				typeof(ProjectGenerationTarget), typeof(DataType), typeof(Documentation),
				typeof(Project), typeof(WcfProject), typeof(WebApiProject),
				typeof(Namespace), typeof(WcfNamespace), typeof(WebApiNamespace),
				typeof(WcfSecurity), typeof(WcfSecurityBasicHTTP), typeof(WcfSecurityBasicHTTPS), typeof(WcfSecurityMSMQ), typeof(WcfSecurityMSMQIntegration), typeof(WcfSecurityNamedPipe), typeof(WcfSecurityPeerTCP), typeof(WcfSecurityTCP), typeof(WcfSecurityWebHTTP), typeof(WcfSecurityWSDualHTTP), typeof(WcfSecurityWSFederationHTTP), typeof(WcfSecurityWSHTTP),
				typeof(WcfBinding), typeof(WcfBindingBasicHTTP), typeof(WcfBindingBasicHTTPS), typeof(WcfBindingMSMQ), typeof(WcfBindingMSMQIntegration), typeof(WcfBindingNamedPipe), typeof(WcfBindingNetHTTP), typeof(WcfBindingNetHTTPS), typeof(WcfBindingPeerTCP), typeof(WcfBindingTCP), typeof(WcfBindingUDP), typeof(WcfBindingWebHTTP), typeof(WcfBindingWS2007FederationHTTP), typeof(WcfBindingWS2007HTTP), typeof(WcfBindingWSDualHTTP), typeof(WcfBindingWSFederationHTTP), typeof(WcfBindingWSHTTP),
				typeof(WcfHost), typeof(WcfHostBehavior), typeof(WcfHostCredentials), typeof(WcfHostDebugBehavior), typeof(WcfHostEndpoint), typeof(WcfHostEndpointAddressHeader), typeof(WcfHostMetadataBehavior), typeof(WcfHostThrottlingBehavior),
				typeof(WcfService), typeof(WcfOperation), typeof(WcfMethod), typeof(WcfCallback), typeof(WcfDataChangeMethod), typeof(WcfMethodParameter), typeof(WcfProperty),
				typeof(WcfData), typeof(WcfDataElement),
				typeof(WebApiService), typeof(WebApiMethod), typeof(WebApiHttpConfiguration), typeof(WebApiRouteParameter), typeof(WebApiMethodParameter),
				typeof(WebApiData), typeof(WebApiDataElement),
				typeof(Enum), typeof(EnumElement),
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
				typeof(ProjectGenerationTarget), typeof(DataType), typeof(Documentation),
				typeof(Project), typeof(WcfProject), typeof(WebApiProject),
				typeof(Namespace), typeof(WcfNamespace), typeof(WebApiNamespace),
				typeof(WcfSecurity), typeof(WcfSecurityBasicHTTP), typeof(WcfSecurityBasicHTTPS), typeof(WcfSecurityMSMQ), typeof(WcfSecurityMSMQIntegration), typeof(WcfSecurityNamedPipe), typeof(WcfSecurityPeerTCP), typeof(WcfSecurityTCP), typeof(WcfSecurityWebHTTP), typeof(WcfSecurityWSDualHTTP), typeof(WcfSecurityWSFederationHTTP), typeof(WcfSecurityWSHTTP),
				typeof(WcfBinding), typeof(WcfBindingBasicHTTP), typeof(WcfBindingBasicHTTPS), typeof(WcfBindingMSMQ), typeof(WcfBindingMSMQIntegration), typeof(WcfBindingNamedPipe), typeof(WcfBindingNetHTTP), typeof(WcfBindingNetHTTPS), typeof(WcfBindingPeerTCP), typeof(WcfBindingTCP), typeof(WcfBindingUDP), typeof(WcfBindingWebHTTP), typeof(WcfBindingWS2007FederationHTTP), typeof(WcfBindingWS2007HTTP), typeof(WcfBindingWSDualHTTP), typeof(WcfBindingWSFederationHTTP), typeof(WcfBindingWSHTTP),
				typeof(WcfHost), typeof(WcfHostBehavior), typeof(WcfHostCredentials), typeof(WcfHostDebugBehavior), typeof(WcfHostEndpoint), typeof(WcfHostEndpointAddressHeader), typeof(WcfHostMetadataBehavior), typeof(WcfHostThrottlingBehavior),
				typeof(WcfService), typeof(WcfOperation), typeof(WcfMethod), typeof(WcfCallback), typeof(WcfDataChangeMethod), typeof(WcfMethodParameter), typeof(WcfProperty),
				typeof(WcfData), typeof(WcfDataElement),
				typeof(WebApiService), typeof(WebApiMethod), typeof(WebApiHttpConfiguration), typeof(WebApiRouteParameter), typeof(WebApiMethodParameter),
				typeof(WebApiData), typeof(WebApiDataElement),
				typeof(Enum), typeof(EnumElement),
			});

			var dcs = new DataContractSerializer(typeof(T), new DataContractSerializerSettings() { KnownTypes = kt, MaxItemsInObjectGraph = Int32.MaxValue, IgnoreExtensionDataObject = true, SerializeReadOnlyTypes = true, PreserveObjectReferences = true });
			var ms = new MemoryStream();
			var xw = XmlWriter.Create(ms, new XmlWriterSettings() { Encoding = Encoding.UTF8, NewLineChars = Environment.NewLine, NewLineHandling = NewLineHandling.Entitize, CloseOutput = true, WriteEndDocumentOnClose = true, Indent = true, Async = false });
			dcs.WriteObject(xw, Data);
			return ms.ToArray();
		}
	}
}