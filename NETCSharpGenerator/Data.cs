using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETPath.Projects;
using NETPath.Projects.Helpers;

namespace NETPath.Generators.NET.CS
{
	internal static class DataGenerator
	{
		public static void VerifyCode(Data o, Action<CompileMessage> AddMessage)
		{
			if (string.IsNullOrEmpty(o.Name))
				AddMessage(new CompileMessage("GS3000", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
			else
				if (RegExs.MatchCodeName.IsMatch(o.Name) == false)
					AddMessage(new CompileMessage("GS3001", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

			if (o.HasClientType)
				if (RegExs.MatchCodeName.IsMatch(o.ClientType.Name) == false)
					AddMessage(new CompileMessage("GS3002", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

			if (o.HasXAMLType)
				if (RegExs.MatchCodeName.IsMatch(o.XAMLType.Name) == false)
					AddMessage(new CompileMessage("GS3003", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

			if (o.Parent.Owner.EnableExperimental && !o.HasAutoDataID)
			{
				AddMessage(new CompileMessage("GS3003", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace does not have an Automatic Data ID specified. A default ID will be used.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
				if (!o.Elements.Any(a => a.IsAutoDataID)) o.Elements.Add(new DataElement(new DataType(PrimitiveTypes.GUID), "_DCMID", o) { IsAutoDataID = true, IsDataMember = true, IsReadOnly = true, ProtocolBufferEnabled = o.Elements.Any(a => a.ProtocolBufferEnabled)});
			}

			foreach (DataElement d in o.Elements)
			{
				if (RegExs.MatchCodeName.IsMatch(d.DataType.Name) == false && d.DataType.TypeMode != DataTypeMode.Array)
					AddMessage(new CompileMessage("GS3001", "The data element '" + d.DataName + "' in the '" + o.Name + "' data object contains invalid characters in the Name.", CompileMessageSeverity.ERROR, o, d, d.GetType(), o.Parent.Owner.ID));

				if (d.HasClientType)
					if (RegExs.MatchCodeName.IsMatch(d.ClientType.Name) == false && d.DataType.TypeMode != DataTypeMode.Array)
						AddMessage(new CompileMessage("GS3002", "The data element '" + d.ClientName + "' in the '" + o.Name + "' data object contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

				if (d.HasXAMLType)
					if (RegExs.MatchCodeName.IsMatch(d.XAMLType.Name) == false && d.DataType.TypeMode != DataTypeMode.Array)
						AddMessage(new CompileMessage("GS3003", "The data element '" + d.XAMLName + "' in the '" + o.Name + "' data object contains invalid characters in the XAML Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

				if (d.DataType.SupportedFrameworks != SupportedFrameworks.None && d.ClientType.SupportedFrameworks != SupportedFrameworks.None && d.XAMLType.SupportedFrameworks != SupportedFrameworks.None)
				{
					if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET30 && !(d.DataType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET30) || d.ClientType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET30) || d.XAMLType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET30)))
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element'" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the .NET 3.0 Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
					if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35 && !(d.DataType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET35) || d.ClientType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET35) || d.XAMLType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET35)))
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element'" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the .NET 3.5 Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
					if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35Client && !(d.DataType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET35) || d.ClientType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET35) || d.XAMLType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET35)))
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element'" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the .NET 3.5 Client Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
					if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40 && !(d.DataType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET40) || d.ClientType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET40) || d.XAMLType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET40)))
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element'" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the .NET 4.0 Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
					if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40Client && !(d.DataType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET40) || d.ClientType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET40) || d.XAMLType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET40)))
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element'" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the .NET 4.0 Client Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
					if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET45 && !(d.DataType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET45) || d.ClientType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET45) || d.XAMLType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET45)))
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element'" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the .NET 4.5 Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

					if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.SL40 && !(d.DataType.SupportedFrameworks.HasFlag(SupportedFrameworks.SL40) || d.ClientType.SupportedFrameworks.HasFlag(SupportedFrameworks.SL40) || d.XAMLType.SupportedFrameworks.HasFlag(SupportedFrameworks.SL40)))
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element'" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the Silverlight 4 Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
					if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.SL50 && !(d.DataType.SupportedFrameworks.HasFlag(SupportedFrameworks.SL50) || d.ClientType.SupportedFrameworks.HasFlag(SupportedFrameworks.SL50) || d.XAMLType.SupportedFrameworks.HasFlag(SupportedFrameworks.SL50)))
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element'" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the Silverlight 5 Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

					if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8 && !(d.DataType.SupportedFrameworks.HasFlag(SupportedFrameworks.WIN8) || d.ClientType.SupportedFrameworks.HasFlag(SupportedFrameworks.WIN8) || d.XAMLType.SupportedFrameworks.HasFlag(SupportedFrameworks.WIN8)))
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element'" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the Windows 8 Runtime Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
				}
			}

			if (o.InheritedTypes.Any(a => a.Name.IndexOf("INotifyPropertyChanged", StringComparison.CurrentCultureIgnoreCase) >= 0))
				AddMessage(new CompileMessage("GS3005", "The server data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace is unable to inherit from INotifyPropertyChanged.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
			if (o.HasXAMLType && o.XAMLType.InheritedTypes.Any(a => a.Name.IndexOf("INotifyPropertyChanged", StringComparison.CurrentCultureIgnoreCase) >= 0))
				AddMessage(new CompileMessage("GS3006", "The XAML integration object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace is unable to inherit from INotifyPropertyChanged.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

			if (o.InheritedTypes.Any(a => a.Name.IndexOf("DependencyObject", StringComparison.CurrentCultureIgnoreCase) >= 0))
				AddMessage(new CompileMessage("GS3007", "The server data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace is unable to inherit from DependencyObject.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
			if (o.HasClientType && o.ClientType.InheritedTypes.Any(a => a.Name.IndexOf("DependencyObject", StringComparison.CurrentCultureIgnoreCase) >= 0))
				AddMessage(new CompileMessage("GS3008", "The client data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace is unable to inherit from DependencyObject.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
		}

		public static string GenerateServerCode40(Data o)
		{
			return GenerateServerCode45(o);
		}

		public static string GenerateServerCode45(Data o)
		{
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			foreach (DataType dt in o.KnownTypes)
				code.AppendLine(string.Format("\t[KnownType(typeof({0}))]", dt));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			if (o.EnableProtocolBuffers) code.AppendLine(string.Format("\t[ProtoBuf.ProtoContract({0}{1}{2})]", o.ProtoSkipConstructor ? "SkipConstructor = true" : "SkipConstructor = false", o.ProtoMembersOnly ? ", UseProtoMembersOnly = true" : "", o.ProtoIgnoreListHandling ? ", IgnoreListHandling = true" : ""));
			code.AppendLine(string.Format("\t[DataContract({0}Name = \"{1}\", Namespace = \"{2}\")]", o.IsReference ? "IsReference = true, " : "", o.HasClientType ? o.ClientType.Name : o.Name, o.Parent.URI));
			code.AppendLine(string.Format("\t{0}", DataTypeGenerator.GenerateTypeDeclaration(o)));
			code.AppendLine("\t{");
	
			if (o.DataHasExtensionData)
			{
				code.AppendLine("\t\tpublic System.Runtime.Serialization.ExtensionDataObject ExtensionData { get; set; }");
				code.AppendLine();
			}

			if (o.Parent.Owner.EnableExperimental && o.AutoDataEnabled)
			{
				code.AppendLine("\t\tprivate bool IsUpdateObjectField;");
				code.AppendLine("\t\t[DataMember(Name = \"IsUpdateObject\")] public bool IsUpdateObject { get { return IsUpdateObjectField; } set { IsUpdateObjectField = value; } }");
			}

			int protoCount = 0;
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementServerCode45(de, ref protoCount));
			code.AppendLine("\t}");
			return code.ToString();
		}

		public static string GenerateProxyCode40(Data o)
		{
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			foreach (DataType dt in o.ClientType != null ? o.ClientType.KnownTypes : o.KnownTypes)
				code.AppendLine(string.Format("\t[KnownType(typeof({0}))]", dt));
			code.AppendLine("\t[System.Diagnostics.DebuggerStepThroughAttribute]");
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			if (o.EnableProtocolBuffers) code.AppendLine(string.Format("\t[ProtoBuf.ProtoContract({0}{1}{2})]", o.ProtoSkipConstructor ? "SkipConstructor = true" : "SkipConstructor = false", o.ProtoMembersOnly ? ", UseProtoMembersOnly = true" : "", o.ProtoIgnoreListHandling ? ", IgnoreListHandling = true" : ""));
			code.AppendLine(string.Format("\t[DataContract({0}Name = \"{1}\", Namespace = \"{2}\")]", o.IsReference ? "IsReference = true, " : "", o.HasClientType ? o.ClientType.Name : o.Name, o.Parent.URI));
			code.AppendLine(string.Format("\t{0}", DataTypeGenerator.GenerateTypeDeclaration(o.HasClientType ? o.ClientType : o, o.ClientHasImpliedExtensionData, o.HasWinFormsBindings)));
			code.AppendLine("\t{");
			code.AppendLine(GenerateProxyDCMCode(o));
			if (o.ClientHasExtensionData || o.ClientHasImpliedExtensionData)
			{
				code.AppendLine("\t\tpublic System.Runtime.Serialization.ExtensionDataObject ExtensionData { get; set; }");
				code.AppendLine();
			}
			if (o.HasWinFormsBindings)
			{
				code.AppendLine("\t\tpublic event PropertyChangedEventHandler PropertyChanged;");
				code.AppendLine("\t\tprivate void NotifyPropertyChanged(string propertyName)");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tif (PropertyChanged != null)");
				code.AppendLine("\t\t\t\tPropertyChanged(this, new PropertyChangedEventArgs(propertyName));");
				code.AppendLine("\t\t}");
				code.AppendLine();
			}
			code.AppendLine("\t\t//Constuctors");
			code.AppendLine(string.Format("\t\tpublic {0}()", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tXAMLObject = this;");
			code.AppendLine("\t\t}");
			if (o.HasXAMLType)
			{
				code.AppendLine(string.Format("\t\tpublic {0}({1} Data)", o.HasClientType ? o.ClientType.Name : o.Name, DataTypeGenerator.GenerateType(o.XAMLType)));
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tXAMLObject = Data;");
				foreach (DataElement de in o.Elements)
					code.Append(GenerateElementXAMLConstructorCode45(de, o));
				code.AppendLine("\t\t}");
			}
			code.AppendLine();
			int protoCount = 0;
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementProxyCode45(de, ref protoCount));
			code.AppendLine("\t}");
			return code.ToString();
		}

		public static string GenerateProxyCode45(Data o)
		{
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			foreach (DataType dt in o.ClientType != null ? o.ClientType.KnownTypes : o.KnownTypes)
				code.AppendLine(string.Format("\t[KnownType(typeof({0}))]", dt));
			code.AppendLine("\t[System.Diagnostics.DebuggerStepThroughAttribute]");
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			if (o.EnableProtocolBuffers) code.AppendLine(string.Format("\t[ProtoBuf.ProtoContract({0}{1}{2})]", o.ProtoSkipConstructor ? "SkipConstructor = true" : "SkipConstructor = false", o.ProtoMembersOnly ? ", UseProtoMembersOnly = true" : "", o.ProtoIgnoreListHandling ? ", IgnoreListHandling = true" : ""));
			code.AppendLine(string.Format("\t[DataContract({0}Name = \"{1}\", Namespace = \"{2}\")]", o.IsReference ? "IsReference = true, " : "", o.HasClientType ? o.ClientType.Name : o.Name, o.Parent.URI));
			code.AppendLine(string.Format("\t{0}", DataTypeGenerator.GenerateTypeDeclaration(o.HasClientType ? o.ClientType : o, o.ClientHasImpliedExtensionData, o.HasWinFormsBindings)));
			code.AppendLine("\t{");
			if (o.HasXAMLType)
			{
				code.AppendLine(string.Format("\t\tpublic {0} XAMLObject {{ get; private set; }}", o.XAMLType.Name));
				code.AppendLine();
			}
			code.AppendLine(GenerateProxyDCMCode(o));
			if (o.ClientHasExtensionData || o.ClientHasImpliedExtensionData)
			{
				code.AppendLine("\t\tpublic System.Runtime.Serialization.ExtensionDataObject ExtensionData { get; set; }");
				code.AppendLine();
			}
			if (o.HasWinFormsBindings)
			{
				code.AppendLine("\t\tpublic event PropertyChangedEventHandler PropertyChanged;");
				code.AppendLine("\t\tprivate void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberNameAttribute] string propertyName = \"\")");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tif (PropertyChanged != null)");
				code.AppendLine("\t\t\t\tPropertyChanged(this, new PropertyChangedEventArgs(propertyName));");
				code.AppendLine("\t\t}");
				code.AppendLine();
			}
			code.AppendLine("\t\t//Constuctors");
			code.AppendLine(string.Format("\t\tpublic {0}()", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tXAMLObject = this;");
			code.AppendLine("\t\t}");
			if (o.HasXAMLType)
			{
				code.AppendLine(string.Format("\t\tpublic {0}({1} Data)", o.HasClientType ? o.ClientType.Name : o.Name, o.XAMLType.Name));
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tXAMLObject = Data;");
				foreach (DataElement de in o.Elements)
					code.Append(GenerateElementXAMLConstructorCode45(de, o));
				code.AppendLine("\t\t}");
			}
			code.AppendLine();
			int protoCount = 0;
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementProxyCode45(de, ref protoCount));
			code.AppendLine("\t}");
			return code.ToString();
		}

		private static string GenerateProxyDCMCode(Data o)
		{
			if (!o.Parent.Owner.EnableExperimental) return "";
			if (!o.AutoDataEnabled) return "";

			var code = new StringBuilder();
			code.AppendLine("\t\t//Data Change Messaging Support");
			code.AppendLine("\t\tprivate readonly System.Threading.ReaderWriterLockSlim __dcmlock = new System.Threading.ReaderWriterLockSlim();");
			code.AppendLine(string.Format("\t\tprivate static readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, {0}> __dcm;", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\tprivate bool IsUpdateObjectField;");
			code.AppendLine("\t\t[DataMember(Name = \"IsUpdateObject\")] public bool IsUpdateObject { get { return IsUpdateObjectField; } set { IsUpdateObjectField = value; } }");
			code.AppendLine(string.Format("\t\tstatic {0}()", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine(string.Format("\t\t\t__dcm = new System.Collections.Concurrent.ConcurrentDictionary<Guid, {0}>();", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t}");
			code.AppendLine("\t\t[OnDeserialized]");
			code.AppendLine("\t\tprivate void OnDeserialized(StreamingContext context)");
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tif(!IsUpdateObject)");
			code.AppendLine(o.HasAutoDataID ? string.Format("\t\t\t\t__dcm.TryAdd({0}, this);", o.AutoDataID.HasClientType ? o.AutoDataID.ClientName : o.AutoDataID.DataName) : "\t\t\t\t__dcm.TryAdd(_DCMID, this);");
			code.AppendLine("\t\t\telse");
			code.AppendLine("\t\t\t{");
			foreach (DataElement e in o.Elements)
				code.AppendLine(GenerateProxyElementDCMBatchUpdateCode(e));
			code.AppendLine("\t\t\t}");
			code.AppendLine("\t\t}");
			code.AppendLine(string.Format("\t\t~{0}()", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine(string.Format("\t\t\t{0} t;", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine(o.HasAutoDataID ? string.Format("\t\t\t__dcm.TryRemove({0}, out t);", o.AutoDataID.HasClientType ? o.AutoDataID.ClientName : o.AutoDataID.DataName) : "\t\t\t__dcm.TryRemove(_DCMID, out t);");
			code.AppendLine("\t\t}");
			return code.ToString();
		}

		private static string GenerateProxyElementDCMBatchUpdateCode(DataElement o)
		{
			if (!o.Owner.Parent.Owner.EnableExperimental) return "";
			if (!o.Owner.AutoDataEnabled) return "";
			if (o.AutoDataUpdateMode != DataUpdateMode.Batch) return "";

			var code = new StringBuilder();
			code.AppendLine(string.Format("\t\t\t\tif({0}Changed)", o.HasClientType ? o.ClientName : o.DataName));
			code.AppendLine("\t\t\t\t{");
			code.AppendLine("\t\t\t\t}");
			return code.ToString();
		}

		public static string GenerateXAMLCode40(Data o)
		{
			return GenerateXAMLCode45(o);
		}

		public static string GenerateXAMLCode45(Data o)
		{
			if (o.HasXAMLType == false) return "";
			if (o.Elements.Any(de => de.HasXAMLType) == false) return "";

			//This is a shim to ensure there is ALWAYS a XAML name.
			if (string.IsNullOrEmpty(o.XAMLType.Name)) o.XAMLType.Name = o.Name + "XAML";

			var code = new StringBuilder();
			code.AppendLine(string.Format("\t//XAML Integration Object for the {0} DTO", o.HasClientType ? o.ClientType.Name : o.Name));
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0}", DataTypeGenerator.GenerateTypeDeclaration(o.XAMLType, false, false, true)));
			code.AppendLine("\t{");
			code.AppendLine(string.Format("\t\tpublic {0} DataObject {{ get; private set; }}", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\tpublic bool IsUpdating { get; set; }");
			code.AppendLine();

			if (o.XAMLHasExtensionData)
			{
				code.AppendLine("\t\tpublic System.Runtime.Serialization.ExtensionDataObject ExtensionData { get; set; }");
				code.AppendLine();
			}

			code.AppendLine("\t\t//Properties");
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementXAMLCode45(de));
			code.AppendLine();

			code.AppendLine("\t\t//Implicit Conversion");
			code.AppendLine(string.Format("\t\tpublic static implicit operator {0}({1} Data)", o.HasClientType ? o.ClientType.Name : o.Name, o.XAMLType.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tif (Data == null) return null;");
			code.AppendLine(string.Format("\t\t\t{0} v = null;", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t\tif (Application.Current.Dispatcher.CheckAccess()) v = ConvertFromXAMLObject(Data);");
			code.AppendLine("\t\t\telse Application.Current.Dispatcher.Invoke(() => { v = ConvertFromXAMLObject(Data); }, System.Windows.Threading.DispatcherPriority.Normal);");
			code.AppendLine("\t\t\treturn v;");
			code.AppendLine("\t\t}");
			code.AppendLine(string.Format("\t\tpublic static implicit operator {0}({1} Data)", o.XAMLType.Name, o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tif (Data == null) return null;");
			code.AppendLine(string.Format("\t\t\t{0} v = null;", o.XAMLType.Name));
			code.AppendLine("\t\t\tif (Application.Current.Dispatcher.CheckAccess()) v = ConvertToXAMLObject(Data);");
			code.AppendLine("\t\t\telse Application.Current.Dispatcher.Invoke(() => { v = ConvertToXAMLObject(Data); }, System.Windows.Threading.DispatcherPriority.Normal);");
			code.AppendLine("\t\t\treturn v;");
			code.AppendLine("\t\t}");
			code.AppendLine();

			code.AppendLine("\t\t//Constructors");
			code.AppendLine(string.Format("\t\tpublic {0}()", o.XAMLType.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}({1} Data)", o.XAMLType.Name, o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementXAMLConstructorCode45(de, o));
			code.AppendLine("\t\t}");
			code.AppendLine();

			code.AppendLine("\t\t//XAML/DTO Conversion Functions");
			code.AppendLine(string.Format("\t\tpublic static {0} ConvertFromXAMLObject({1} Data)", o.HasClientType ? o.ClientType.Name : o.Name, o.XAMLType.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tif (Data.DataObject != null) return Data.DataObject;");
			code.AppendLine(string.Format("\t\t\treturn new {0}(Data);", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic static {0} ConvertToXAMLObject({1} Data)", o.XAMLType.Name, o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tif (Data.XAMLObject != null) return Data.XAMLObject;");
			code.AppendLine(string.Format("\t\t\treturn new {0}(Data);", o.XAMLType.Name));
			code.AppendLine("\t\t}");
			code.AppendLine("\t}");

			return code.ToString();
		}

		private static string GenerateElementServerCode40(DataElement o, ref int ProtoCount)
		{
			return GenerateElementServerCode45(o, ref ProtoCount);
		}

		private static string GenerateElementServerCode45(DataElement o, ref int ProtoCount)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			if (o.Owner.Parent.Owner.EnableExperimental && o.IsDataMember && o.AutoDataEnabled && !o.IsReadOnly) code.AppendLine(string.Format("\t\t{1}[DataMember] private bool {0}Changed;", o.HasClientType ? o.ClientName : o.DataName, (o.ProtocolBufferEnabled && o.Owner.EnableProtocolBuffers) ? string.Format("[ProtoBuf.ProtoMember({0})] ", ProtoCount++) : ""));
			code.Append("\t\t");
			if (o.IsDataMember)
			{
				if (o.IsDataMember && o.ProtocolBufferEnabled && o.Owner.EnableProtocolBuffers) code.AppendFormat("[ProtoBuf.ProtoMember({0}{1}{2}{3}{4}{5}{6})] ", ProtoCount++, o.ProtoDataFormat != ProtoBufDataFormat.Default ? string.Format(", DataFormat = ProtoBuf.DataFormat.{0}", System.Enum.GetName(typeof(ProtoBufDataFormat), o.ProtoDataFormat)) : "", o.IsRequired ? ", IsRequired = true" : "", o.ProtoIsPacked ? ", IsPacked = true" : "", o.ProtoOverwriteList ? ", OverwriteList = true" : "", o.ProtoAsReference ? ", AsReference = true" : "", o.ProtoDynamicType ? ", DynamicType = true" : "");
				code.AppendFormat("[DataMember({0}{1}{2}Name = \"{3}\")] ", o.EmitDefaultValue ? "EmitDefaultValue = false, " : "", o.IsRequired ? "IsRequired = true, " : "", o.Order >= 0 ? string.Format("Order = {0}, ", o.Order) : "", o.HasClientType ? o.ClientName : o.DataName);
			}
			else
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.XML) code.Append("[XmlIgnore] ");
			code.AppendLine(string.Format("public {0} {1} {{ get; {2}set; }}", DataTypeGenerator.GenerateType(o.DataType), o.DataName, o.IsReadOnly ? "protected " : ""));
			return code.ToString();
		}

		private static string GenerateElementProxyCode40(DataElement o, ref int ProtoCount)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			var code = new StringBuilder();
			if (o.Owner.Parent.Owner.EnableExperimental && o.AutoDataEnabled && !o.IsReadOnly) code.AppendLine(string.Format("\t\t{1}[DataMember] private bool {0}Changed;", o.HasClientType ? o.ClientName : o.DataName, (o.ProtocolBufferEnabled && o.Owner.EnableProtocolBuffers) ? string.Format("[ProtoBuf.ProtoMember({0})] ", ProtoCount++) : ""));
			code.AppendLine(string.Format("\t\tprivate {0} {1}Field;", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName));
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.Append("\t\t");
			if (o.ProtocolBufferEnabled && o.Owner.EnableProtocolBuffers) code.AppendFormat("[ProtoBuf.ProtoMember({0}{1}{2}{3}{4}{5}{6})] ", ProtoCount++, o.ProtoDataFormat != ProtoBufDataFormat.Default ? string.Format(", DataFormat = ProtoBuf.DataFormat.{0}", System.Enum.GetName(typeof(ProtoBufDataFormat), o.ProtoDataFormat)) : "", o.IsRequired ? ", IsRequired = true" : "", o.ProtoIsPacked ? ", IsPacked = true" : "", o.ProtoOverwriteList ? ", OverwriteList = true" : "", o.ProtoAsReference ? ", AsReference = true" : "", o.ProtoDynamicType ? ", DynamicType = true" : "");
			code.AppendFormat("[DataMember({0}{1}{2}Name = \"{3}\")] ", o.EmitDefaultValue ? "EmitDefaultValue = false, " : "", o.IsRequired ? "IsRequired = true, " : "", o.ProtocolBufferEnabled ? string.Format("Order = {0}, ", o.Owner.Elements.IndexOf(o)) : o.Order >= 0 ? string.Format("Order = {0}, ", o.Order) : "", o.HasClientType ? o.ClientName : o.DataName);
			if (o.Owner.Parent.Owner.EnableExperimental && o.AutoDataEnabled)
				if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET30) code.AppendLine(string.Format("public {0} {1} {{ get {{ __dcmlock.{4}; try {{ return {1}Field; }} finally {{ __dcmlock.ReleaseReaderLock(); }} }} {2}set {{ __dcmlock.{5}; try {{ {1}Field = value; {1}Changed = true; {3}}} finally {{ __dcmlock.ReleaseWriterLock(); }} }} }}", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.IsReadOnly ? "protected " : "", o.GenerateWinFormsSupport ? "NotifyPropertyChanged(); " : "", string.Format("AcquireReaderLock({0})", o.AutoDataTimeout), string.Format("AcquireWriterLock({0})", o.AutoDataTimeout)));
				else code.AppendLine(string.Format("public {0} {1} {{ get {{ __dcmlock.{4}; try {{ return {1}Field; }} finally {{ __dcmlock.ExitReadLock(); }} }} {2}set {{ __dcmlock.{5}; try {{ {1}Field = value; {1}Changed = true; {3}}} finally {{ __dcmlock.ExitWriteLock(); }} }} }}", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.IsReadOnly ? "protected " : "", o.GenerateWinFormsSupport ? "NotifyPropertyChanged(); " : "", o.AutoDataTimeout == 0 ? "EnterReadLock()" : string.Format("TryEnterReadLock({0})", o.AutoDataTimeout), o.AutoDataTimeout == 0 ? "EnterWriteLock()" : string.Format("TryEnterWriteLock({0})", o.AutoDataTimeout)));
			else
				code.AppendLine(string.Format("public {0} {1} {{ get {{ return {1}Field; }} {2}set {{ {1}Field = value; {3}}} }}", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.IsReadOnly ? "protected " : "", o.GenerateWinFormsSupport ? string.Format("NotifyPropertyChanged(\"{0}\"); ", o.HasClientType ? o.ClientName : o.DataName) : ""));
			return code.ToString();
		}

		private static string GenerateElementProxyCode45(DataElement o, ref int ProtoCount)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			var code = new StringBuilder();
			if (o.Owner.Parent.Owner.EnableExperimental && o.AutoDataEnabled && !o.IsReadOnly) code.AppendLine(string.Format("\t\t{1}[DataMember] private bool {0}Changed;", o.HasClientType ? o.ClientName : o.DataName, (o.ProtocolBufferEnabled && o.Owner.EnableProtocolBuffers) ? string.Format("[ProtoBuf.ProtoMember({0})] ", ProtoCount++) : ""));
			code.AppendLine(string.Format("\t\tprivate {0} {1}Field;", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName));
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.Append("\t\t");
			if (o.ProtocolBufferEnabled && o.Owner.EnableProtocolBuffers) code.AppendFormat("[ProtoBuf.ProtoMember({0}{1}{2}{3}{4}{5}{6})] ", ProtoCount++, o.ProtoDataFormat != ProtoBufDataFormat.Default ? string.Format(", DataFormat = ProtoBuf.DataFormat.{0}", System.Enum.GetName(typeof(ProtoBufDataFormat), o.ProtoDataFormat)) : "", o.IsRequired ? ", IsRequired = true" : "", o.ProtoIsPacked ? ", IsPacked = true" : "", o.ProtoOverwriteList ? ", OverwriteList = true" : "", o.ProtoAsReference ? ", AsReference = true" : "", o.ProtoDynamicType ? ", DynamicType = true" : "");
			code.AppendFormat("[DataMember({0}{1}{2}Name = \"{3}\")] ", o.EmitDefaultValue ? "EmitDefaultValue = false, " : "", o.IsRequired ? "IsRequired = true, " : "", o.ProtocolBufferEnabled ? string.Format("Order = {0}, ", o.Owner.Elements.IndexOf(o)) : o.Order >= 0 ? string.Format("Order = {0}, ", o.Order) : "", o.HasClientType ? o.ClientName : o.DataName);
			if (o.Owner.Parent.Owner.EnableExperimental && o.AutoDataEnabled)
				code.AppendLine(string.Format("public {0} {1} {{ get {{ __dcmlock.{4}; try {{ return {1}Field; }} finally {{ __dcmlock.ExitReadLock(); }} }} {2}set {{ __dcmlock.{5}; try {{ {1}Field = value; {1}Changed = true; {3}}} finally {{ __dcmlock.ExitWriteLock(); }} }} }}", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.IsReadOnly ? "protected " : "", o.GenerateWinFormsSupport ? "NotifyPropertyChanged(); " : "", o.AutoDataTimeout == 0 ? "EnterReadLock()" : string.Format("TryEnterReadLock({0})", o.AutoDataTimeout), o.AutoDataTimeout == 0 ? "EnterWriteLock()" : string.Format("TryEnterWriteLock({0})", o.AutoDataTimeout)));
			else
				code.AppendLine(string.Format("public {0} {1} {{ get {{ return {1}Field; }} {2}set {{ {1}Field = value; {3}}} }}", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.IsReadOnly ? "protected " : "", o.GenerateWinFormsSupport ? "NotifyPropertyChanged(); " : ""));
			return code.ToString();
		}

		private static DataType GetPreferredXAMLType(DataType value)
		{
			if (value.TypeMode == DataTypeMode.Array || value.TypeMode == DataTypeMode.Collection || value.TypeMode == DataTypeMode.Stack || value.TypeMode == DataTypeMode.Queue)
			{
				if (value.CollectionGenericType.TypeMode == DataTypeMode.Class || value.CollectionGenericType.TypeMode == DataTypeMode.Struct)
				{
					var t = value.CollectionGenericType as Data;
					if (t == null) return value;
					return t.HasXAMLType ? new DataType(value.Name, value.TypeMode) { Name = value.Name, CollectionGenericType = t.XAMLType } : new DataType(value.Name, value.TypeMode) { Name = value.Name, CollectionGenericType = t.HasClientType ? t.ClientType : t };
				}
			}
			else if (value.TypeMode == DataTypeMode.Dictionary)
			{
				DataType dk = value.DictionaryKeyGenericType;
				DataType dv = value.DictionaryValueGenericType;
				if (value.DictionaryKeyGenericType.TypeMode == DataTypeMode.Class || value.DictionaryKeyGenericType.TypeMode == DataTypeMode.Struct)
				{
					var t = value.DictionaryKeyGenericType as Data;
					if (t == null) return value;
					dk = t.HasXAMLType ? t.XAMLType : t.HasClientType ? t.ClientType : t;
				}
				if (value.DictionaryValueGenericType.TypeMode == DataTypeMode.Class || value.DictionaryValueGenericType.TypeMode == DataTypeMode.Struct)
				{
					var t = value.DictionaryValueGenericType as Data;
					if (t == null) return value;
					dv = t.HasXAMLType ? t.XAMLType : t.HasClientType ? t.ClientType : t;
				}
				return new DataType(value.Name, DataTypeMode.Dictionary) { Name = value.Name, DictionaryKeyGenericType = dk, DictionaryValueGenericType = dv };
			}
			else if (value.TypeMode == DataTypeMode.Class || value.TypeMode == DataTypeMode.Struct)
			{
				var t = value as Data;
				if (t == null) return value;
				return t.HasXAMLType ? t.XAMLType : t.HasClientType ? t.ClientType : t;
			}
			return value;
		}

		private static string GenerateElementXAMLCode40(DataElement o)
		{
			return GenerateElementXAMLCode45(o);
		}

		private static string GenerateElementXAMLCode45(DataElement o)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			if (!o.Owner.HasXAMLType) return "";
			if (!o.HasXAMLType) return "";

			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			if (o.IsReadOnly == false && o.IsAttached == false)
			{
				code.AppendLine(string.Format("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue{2}({1}Property); }} set {{ SetValue{2}({1}Property, value); }} }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)), o.XAMLName, (o.Owner.Parent.Owner.EnableExperimental && o.Owner.AutoDataEnabled) ? "Threaded" : ""));
				code.AppendLine(string.Format("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.Register(\"{1}\", typeof({0}), typeof({2}){3});", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)), o.XAMLName, o.Owner.XAMLType.Name, o.AutoDataEnabled ? string.Format(", new PropertyMetadata({0}ChangedCallback)", o.XAMLName) : ""));
				if (o.AutoDataEnabled)
				{
					code.AppendLine(string.Format("\t\tprivate static void {0}ChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)", o.XAMLName));
					code.AppendLine("\t\t{");
					code.AppendLine(string.Format("\t\t\tvar t = o as {0};", DataTypeGenerator.GenerateType(o.Owner.XAMLType)));
					code.AppendLine("\t\t\tif (t == null) return;");
					code.AppendLine(string.Format("\t\t\tt.DataObject.{0} = ({1}) e.NewValue;", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(o.XAMLType)));
					code.AppendLine("\t\t}");
				}
			}
			if (o.IsReadOnly && o.IsAttached == false)
			{
				code.AppendLine(string.Format("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue{2}({1}Property); }} protected set {{ SetValue{2}({1}PropertyKey, value); }} }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)), o.XAMLName, (o.Owner.Parent.Owner.EnableExperimental && o.Owner.AutoDataEnabled) ? "Threaded" : ""));
				code.AppendLine(string.Format("\t\tprivate static readonly DependencyPropertyKey {1}PropertyKey = DependencyProperty.RegisterReadOnly(\"{1}\", typeof({0}), typeof({2}), null);", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)), o.XAMLName, o.Owner.XAMLType.Name));
				code.AppendLine(string.Format("\t\tpublic static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;", o.XAMLName));
			}
			if (!o.IsReadOnly && o.IsAttached)
			{
				if (o.AttachedBrowsable)
					code.AppendLine(string.Format("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants={0})]", o.AttachedBrowsableIncludeDescendants ? "true" : "false"));
				if (o.AttachedTargetTypes != "")
				{
					var ttl = new List<string>(o.AttachedTargetTypes.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries));
					foreach (string tt in ttl)
						code.AppendLine(string.Format("\t\t[AttachedPropertyBrowsableForType(typeof({0}))]", tt.Trim()));
				}
				if (o.AttachedAttributeTypes != "")
				{
					var ttl = new List<string>(o.AttachedAttributeTypes.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries));
					foreach (string tt in ttl)
						code.AppendLine(string.Format("\t\t[AttachedPropertyBrowsableWhenAttributePresent(typeof({0}))]", tt.Trim()));
				}
				code.AppendLine(string.Format("\t\tpublic static {0} Get{1}(DependencyObject{2} obj) {{ return ({0})obj.GetValue{3}({1}Property); }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)), o.XAMLName, (o.Owner.Parent.Owner.EnableExperimental && o.AutoDataEnabled) ? "Ex" : "", (o.Owner.Parent.Owner.EnableExperimental && o.Owner.AutoDataEnabled) ? "Threaded" : ""));
				code.AppendLine(string.Format("\t\tpublic static void Set{1}(DependencyObject{2} obj, {0} value) {{ obj.SetValue{3}({1}Property, value); }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)), o.XAMLName, (o.Owner.Parent.Owner.EnableExperimental && o.AutoDataEnabled) ? "Ex" : "", (o.Owner.Parent.Owner.EnableExperimental && o.Owner.AutoDataEnabled) ? "Threaded" : ""));
				code.AppendLine(string.Format("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.RegisterAttached(\"{1}\", typeof({0}), typeof({2}), {3});", DataTypeGenerator.GenerateType(o.XAMLType), o.XAMLName, o.Owner.XAMLType.Name, o.AutoDataEnabled ? string.Format("new PropertyMetadata({0}ChangedCallback)", o.XAMLName) : "null"));
				if (o.AutoDataEnabled)
				{
					code.AppendLine(string.Format("\t\tprivate static void {0}ChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)", o.XAMLName));
					code.AppendLine("\t\t{");
					code.AppendLine(string.Format("\t\t\tvar t = o as {0};", DataTypeGenerator.GenerateType(o.Owner.XAMLType)));
					code.AppendLine("\t\t\tif (t == null) return;");
					code.AppendLine(string.Format("\t\t\tt.DataObject.{0} = ({1}) e.NewValue;", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(o.XAMLType)));
					code.AppendLine("\t\t}");
				}
			}
			if (o.IsReadOnly && o.IsAttached)
			{
				if (o.AttachedBrowsable)
					code.AppendLine(string.Format("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants={0})]", o.AttachedBrowsableIncludeDescendants ? "true" : "false"));
				if (o.AttachedTargetTypes != "")
				{
					var ttl = new List<string>(o.AttachedTargetTypes.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries));
					foreach (string tt in ttl)
						code.AppendLine(string.Format("\t\t[AttachedPropertyBrowsableForType(typeof({0}))]", tt.Trim()));
				}
				if (o.AttachedAttributeTypes != "")
				{
					var ttl = new List<string>(o.AttachedAttributeTypes.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries));
					foreach (string tt in ttl)
						code.AppendLine(string.Format("\t\t[AttachedPropertyBrowsableWhenAttributePresent(typeof({0}))]", tt.Trim()));
				}
				code.AppendLine(string.Format("\t\tpublic static {0} Get{1}(DependencyObject{2} obj) {{ return ({0})obj.GetValue{3}({1}Property); }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)), o.XAMLName, (o.Owner.Parent.Owner.EnableExperimental && o.AutoDataEnabled) ? "Ex" : "", (o.Owner.Parent.Owner.EnableExperimental && o.Owner.AutoDataEnabled) ? "Threaded" : ""));
				code.AppendLine(string.Format("\t\tpublic static void Set{1}(DependencyObject{2} obj, {0} value) {{ obj.SetValue{3}({1}PropertyKey, value); }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)), o.XAMLName, (o.Owner.Parent.Owner.EnableExperimental && o.AutoDataEnabled) ? "Ex" : "", (o.Owner.Parent.Owner.EnableExperimental && o.Owner.AutoDataEnabled) ? "Threaded" : ""));
				code.AppendLine(string.Format("\t\tprivate static readonly DependencyPropertyKey {1}PropertyKey = DependencyProperty.RegisterAttachedReadOnly(\"{1}\", typeof({0}), typeof({2}), null);", DataTypeGenerator.GenerateType(o.XAMLType), o.XAMLName, o.Owner.XAMLType.Name));
				code.AppendLine(string.Format("\t\tpublic static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;", o.XAMLName));
			}
			return code.ToString();
		}

		private static string GenerateElementXAMLConstructorCode40(DataElement o, Data c)
		{
			return GenerateElementXAMLConstructorCode45(o, c);
		}

		private static string GenerateElementXAMLConstructorCode45(DataElement o, Data c)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			if (!o.HasXAMLType) return "";
			var code = new StringBuilder();

			if (o.XAMLType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\t\t{0}[] v{3} = new {1}[Data.{2}.GetLength(0)];", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)).Replace("[]", ""), DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)).Replace("[]", ""), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tfor(int i = 0; i < Data.{0}.GetLength(0); i++) {{ v{1}[i] = Data.{0}[i]; }}", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tforeach({0} a in Data.{1}) {{ v{2}.Add(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Stack)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tforeach({0} a in Data.{1}) {{ v{2}.Push(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Queue)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tforeach({0} a in Data.{1}) {{ v{2}.Enqueue(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)), o.XAMLName));
				code.AppendLine(o.XAMLType.Name == "System.Collections.Concurrent.ConcurrentDictionary" ? string.Format("\t\t\tforeach(KeyValuePair<{0}> a in Data.{1}) {{ v{2}.TryAdd(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName) : string.Format("\t\t\tforeach(KeyValuePair<{0}> a in Data.{1}) {{ v{2}.Add(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else
			{
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{1} = Data.{0};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				else code.AppendLine(string.Format("\t\t\t{2}.Set{1}(this, Data.{0});", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName, DataTypeGenerator.GenerateType(c.XAMLType)));
			}

			return code.ToString();
		}
	}
}