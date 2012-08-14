using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WCFArchitect.Projects
{
	internal class Storage
	{
		public static T Open<T>(string Path)
		{
			//Check the file to make sure it exists
			if (!System.IO.File.Exists(Path))
				throw new System.IO.FileNotFoundException("Unable to locate file '" + Path + "'");

			List<Type> kt = new List<Type>(new Type[] { 
				typeof(Solution), typeof(Project), typeof(ProjectGenerationTarget), typeof(DataType), typeof(Namespace),
				typeof(BindingSecurity), typeof(BindingSecurityBasicHTTP), typeof(BindingSecurityMSMQ), typeof(BindingSecurityMSMQIntegration), typeof(BindingSecurityNamedPipe), typeof(BindingSecurityPeerTCP), typeof(BindingSecurityTCP), typeof(BindingSecurityWebHTTP), typeof(BindingSecurityWSDualHTTP), typeof(BindingSecurityWSFederationHTTP), typeof(BindingSecurityWSHTTP),
				typeof(ServiceBinding), typeof(ServiceBindingBasicHTTP), typeof(ServiceBindingMSMQ), typeof(ServiceBindingMSMQIntegration), typeof(ServiceBindingNamedPipe), typeof(ServiceBindingNetHTTP), typeof(ServiceBindingPeerTCP), typeof(ServiceBindingTCP), typeof(ServiceBindingWebHTTP), typeof(ServiceBindingWS2007FederationHTTP), typeof(ServiceBindingWS2007HTTP), typeof(ServiceBindingWSDualHTTP), typeof(ServiceBindingWSFederationHTTP), typeof(ServiceBindingWSHTTP),
				typeof(Host), typeof(HostBehavior), typeof(HostCredentials), typeof(HostDebugBehavior), typeof(HostEndpoint), typeof(HostEndpointAddressHeader), typeof(HostMetadataBehavior), typeof(HostThrottlingBehavior),
				typeof(Service), typeof(Operation), typeof(Method), typeof(MethodParameter), typeof(Property),
				typeof(Enum), typeof(EnumElement),
				typeof(Data), typeof(DataElement)
			});

			DataContractSerializer dcs = new DataContractSerializer(typeof(T), new DataContractSerializerSettings() { KnownTypes = kt, MaxItemsInObjectGraph = Int32.MaxValue, IgnoreExtensionDataObject = true, SerializeReadOnlyTypes = true, PreserveObjectReferences = true });
			FileStream fs = new FileStream(Path, FileMode.Open, FileAccess.Read);
			XmlReader tr = XmlReader.Create(fs, new XmlReaderSettings() { CloseInput = true, Async = false });

			T t = (T)dcs.ReadObject(tr);
			tr.Close();
			return t;
		}

		public static void Save<T>(string Path, T Data)
		{
			//Make sure the solution isn't read-only.
			if (System.IO.File.Exists(Path))
			{
				System.IO.FileInfo fi = new System.IO.FileInfo(Path);
				if (fi.IsReadOnly == true)
					throw new System.IO.IOException("The file '" + Path + "' is currently read-only. Please disable read-only mode on this file.");
			}

			List<Type> kt = new List<Type>(new Type[] { 
				typeof(Solution), typeof(Project), typeof(ProjectGenerationTarget), typeof(DataType), typeof(Namespace),
				typeof(BindingSecurity), typeof(BindingSecurityBasicHTTP), typeof(BindingSecurityMSMQ), typeof(BindingSecurityMSMQIntegration), typeof(BindingSecurityNamedPipe), typeof(BindingSecurityPeerTCP), typeof(BindingSecurityTCP), typeof(BindingSecurityWebHTTP), typeof(BindingSecurityWSDualHTTP), typeof(BindingSecurityWSFederationHTTP), typeof(BindingSecurityWSHTTP),
				typeof(ServiceBinding), typeof(ServiceBindingBasicHTTP), typeof(ServiceBindingMSMQ), typeof(ServiceBindingMSMQIntegration), typeof(ServiceBindingNamedPipe), typeof(ServiceBindingNetHTTP), typeof(ServiceBindingPeerTCP), typeof(ServiceBindingTCP), typeof(ServiceBindingWebHTTP), typeof(ServiceBindingWS2007FederationHTTP), typeof(ServiceBindingWS2007HTTP), typeof(ServiceBindingWSDualHTTP), typeof(ServiceBindingWSFederationHTTP), typeof(ServiceBindingWSHTTP),
				typeof(Host), typeof(HostBehavior), typeof(HostCredentials), typeof(HostDebugBehavior), typeof(HostEndpoint), typeof(HostEndpointAddressHeader), typeof(HostMetadataBehavior), typeof(HostThrottlingBehavior),
				typeof(Service), typeof(Operation), typeof(Method), typeof(MethodParameter), typeof(Property),
				typeof(Enum), typeof(EnumElement),
				typeof(Data), typeof(DataElement)
			});

			DataContractSerializer dcs = new DataContractSerializer(typeof(T), new DataContractSerializerSettings() { KnownTypes = kt, MaxItemsInObjectGraph = Int32.MaxValue, IgnoreExtensionDataObject = true, SerializeReadOnlyTypes = true, PreserveObjectReferences = true });
			FileStream fs = new FileStream(Path, FileMode.Create);
			XmlWriter xw = XmlWriter.Create(fs, new XmlWriterSettings() { Encoding = Encoding.UTF8, NewLineChars = Environment.NewLine, NewLineHandling = System.Xml.NewLineHandling.Entitize, CloseOutput = true, WriteEndDocumentOnClose = true, Indent = true, Async = false });
			dcs.WriteObject(xw, Data);
			xw.Flush();
			xw.Close();
		}
	}
}