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

			DataContractSerializer dcs = new DataContractSerializer(typeof(T), new DataContractSerializerSettings() { MaxItemsInObjectGraph = Int32.MaxValue, IgnoreExtensionDataObject = true, SerializeReadOnlyTypes = true, PreserveObjectReferences = true });
			FileStream fs = new FileStream(Path, FileMode.Open, FileAccess.Read);
			XmlReader tr = XmlReader.Create(fs, new XmlReaderSettings() { CloseInput = true, Async = false });
			XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());

			T t = (T)dcs.ReadObject(reader);
			reader.Close();
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

			DataContractSerializer dcs = new DataContractSerializer(typeof(T), new DataContractSerializerSettings() { MaxItemsInObjectGraph = Int32.MaxValue, IgnoreExtensionDataObject = true, SerializeReadOnlyTypes = true, PreserveObjectReferences = true });
			FileStream fs = new FileStream(Path, FileMode.Create);
			XmlWriter xw = XmlWriter.Create(fs, new XmlWriterSettings() { Encoding = Encoding.UTF8, NewLineChars = Environment.NewLine, NewLineHandling = System.Xml.NewLineHandling.Entitize, CloseOutput = true, WriteEndDocumentOnClose = true, Indent = true, Async = false });
			dcs.WriteObject(xw, Data);
			xw.Flush();
			xw.Close();
		}
	}
}