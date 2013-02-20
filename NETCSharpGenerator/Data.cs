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

			if (!o.HasDCMID && o.CMDEnabled)
			{
				AddMessage(new CompileMessage("GS3003", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace does not have an Automatic Data ID specified. A default ID will be used.", CompileMessageSeverity.WARN, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
				if (!o.Elements.Any(a => a.IsDCMID)) o.Elements.Add(new DataElement(new DataType(PrimitiveTypes.GUID), "_DCMID", o) { IsDCMID = true, IsDataMember = true, IsReadOnly = true, ProtocolBufferEnabled = o.Elements.Any(a => a.ProtocolBufferEnabled)});
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
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element '" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the .NET 3.0 Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
					if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35 && !(d.DataType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET35) || d.ClientType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET35) || d.XAMLType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET35)))
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element '" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the .NET 3.5 Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
					if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35Client && !(d.DataType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET35) || d.ClientType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET35) || d.XAMLType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET35)))
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element '" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the .NET 3.5 Client Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
					if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40 && !(d.DataType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET40) || d.ClientType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET40) || d.XAMLType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET40)))
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element '" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the .NET 4.0 Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
					if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET40Client && !(d.DataType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET40) || d.ClientType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET40) || d.XAMLType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET40)))
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element '" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the .NET 4.0 Client Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
					if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET45 && !(d.DataType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET45) || d.ClientType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET45) || d.XAMLType.SupportedFrameworks.HasFlag(SupportedFrameworks.NET45)))
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element '" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the .NET 4.5 Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

					if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.SL40 && !(d.DataType.SupportedFrameworks.HasFlag(SupportedFrameworks.SL40) || d.ClientType.SupportedFrameworks.HasFlag(SupportedFrameworks.SL40) || d.XAMLType.SupportedFrameworks.HasFlag(SupportedFrameworks.SL40)))
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element '" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the Silverlight 4 Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
					if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.SL50 && !(d.DataType.SupportedFrameworks.HasFlag(SupportedFrameworks.SL50) || d.ClientType.SupportedFrameworks.HasFlag(SupportedFrameworks.SL50) || d.XAMLType.SupportedFrameworks.HasFlag(SupportedFrameworks.SL50)))
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element '" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the Silverlight 5 Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

					if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8 && !(d.DataType.SupportedFrameworks.HasFlag(SupportedFrameworks.WIN8) || d.ClientType.SupportedFrameworks.HasFlag(SupportedFrameworks.WIN8) || d.XAMLType.SupportedFrameworks.HasFlag(SupportedFrameworks.WIN8)))
						AddMessage(new CompileMessage("GS3009", "The data element type '" + d.XAMLType.TypeName + "' for the element '" + d.DataName + "' in the '" + o.Name + "' data object is unsupported in the Windows 8 Runtime Generation Target. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
				}

				if (d.DCMEnabled && d.DataType.TypeMode == DataTypeMode.Stack)
					AddMessage(new CompileMessage("GS3009", "Stacks are invalid for the data change enabled element '" + d.DataName + "' in the '" + o.Name + "' data object. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
				if (d.DCMEnabled && d.DataType.TypeMode == DataTypeMode.Queue)
					AddMessage(new CompileMessage("GS3009", "Queues are invalid for the data change enabled element '" + d.DataName + "' in the '" + o.Name + "' data object. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
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
			else code.AppendLine(string.Format("\t[DataContract({0}Name = \"{1}\", Namespace = \"{2}\")]", o.IsReference ? "IsReference = true, " : "", o.HasClientType ? o.ClientType.Name : o.Name, o.Parent.URI));
			code.AppendLine(string.Format("\t{0}", DataTypeGenerator.GenerateTypeDeclaration(o, false, false, o.CMDEnabled, o.CMDEnabled)));
			code.AppendLine("\t{");
	
			if (o.DataHasExtensionData)
			{
				code.AppendLine("\t\tpublic System.Runtime.Serialization.ExtensionDataObject ExtensionData { get; set; }");
				code.AppendLine();
			}

			code.Append(GenerateProxyDCMCode(o));

			int protoCount = 0;
			foreach (DataElement de in o.Elements.Where(a => !a.IsHidden))
				code.Append(o.CMDEnabled ? GenerateElementDCMServerCode45(de, ref protoCount) : GenerateElementServerCode45(de, ref protoCount));
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
			else code.AppendLine(string.Format("\t[DataContract({0}Name = \"{1}\", Namespace = \"{2}\")]", o.IsReference ? "IsReference = true, " : "", o.HasClientType ? o.ClientType.Name : o.Name, o.Parent.URI));
			code.AppendLine(string.Format("\t{0}", DataTypeGenerator.GenerateTypeDeclaration(o.HasClientType ? o.ClientType : o, o.ClientHasImpliedExtensionData, o.HasWinFormsBindings)));
			code.AppendLine("\t{");
			if (o.HasXAMLType)
			{
				code.AppendLine(string.Format("\t\tprivate {0} xamlObject;", o.XAMLType.Name));
				code.AppendLine(string.Format("\t\tpublic {0} XAMLObject {{ get {{ if(xamlObject == null) Application.Current.Dispatcher.Invoke(() => {{ xamlObject = new {0}(this); }}, System.Windows.Threading.DispatcherPriority.Normal); return xamlObject; }} private set {{ xamlObject = value; }} }}", o.XAMLType.Name));
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
				code.AppendLine("\t\tprivate void NotifyPropertyChanged(string propertyName)");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tif (PropertyChanged != null)");
				code.AppendLine("\t\t\t\tPropertyChanged(this, new PropertyChangedEventArgs(propertyName));");
				code.AppendLine("\t\t}");
				code.AppendLine();
			}
			code.AppendLine("\t\t//Implicit Conversion");
			code.AppendLine(string.Format("\t\tpublic static implicit operator {0}({1} Data)", o.HasClientType ? o.ClientType.Name : o.Name, o.XAMLType.Name));
			code.AppendLine("\t\t{");
			if (o.CMDEnabled)
			{
				code.AppendLine("\t\t\tif (Data == null) return null;");
				code.AppendLine(string.Format("\t\t\t{0} v = null;", o.HasClientType ? o.ClientType.Name : o.Name));
				code.AppendLine("\t\t\tif (Application.Current.Dispatcher.CheckAccess()) v = ConvertFromXAMLObject(Data);");
				code.AppendLine("\t\t\telse Application.Current.Dispatcher.Invoke(() => { v = ConvertFromXAMLObject(Data); }, System.Windows.Threading.DispatcherPriority.Normal);");
				code.AppendLine("\t\t\treturn v;");
			}
			else
			{
				code.AppendLine("\t\t\tData.DataObject.UpdateFromXAML();");
				code.AppendLine("\t\t\treturn Data.DataObject;");
			}
			code.AppendLine("\t\t}");
			code.AppendLine();
			if (!o.CMDEnabled)
			{
				code.AppendLine("\t\tprivate void UpdateFromXAML()");
				code.AppendLine("\t\t{");
				foreach (DataElement de in o.Elements)
					code.Append(GenerateElementProxyUpdateCode45(de, o));
				code.AppendLine("\t\t}");
			}

			code.AppendLine("\t\t//Constuctors");
			code.AppendLine(string.Format("\t\tpublic {0}()", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			if (o.HasXAMLType) code.AppendLine(string.Format("\t\t\tApplication.Current.Dispatcher.Invoke(() => {{ XAMLObject = new {0}(this); }}, System.Windows.Threading.DispatcherPriority.Normal);", o.XAMLType.Name));
			foreach (DataElement de in o.Elements)
				if (de.XAMLType.TypeMode == DataTypeMode.Collection || de.XAMLType.TypeMode == DataTypeMode.Dictionary)
					code.AppendLine(string.Format("\t\t\t{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(de.HasClientType ? de.ClientType : de.DataType, o.CMDEnabled && de.DCMEnabled)), de.HasClientType ? de.ClientName : de.DataName));
			code.AppendLine("\t\t}");
			if (o.HasXAMLType)
			{
				code.AppendLine(string.Format("\t\tpublic {0}({1} Data)", o.HasClientType ? o.ClientType.Name : o.Name, o.XAMLType.Name));
				code.AppendLine("\t\t{");
				if (o.HasXAMLType) code.AppendLine("\t\t\tXAMLObject = Data;");
				foreach (DataElement de in o.Elements)
					code.Append(GenerateElementProxyConstructorCode45(de, o));
				code.AppendLine("\t\t}");
			}
			code.AppendLine();

			code.AppendLine("\t\t//DTO->XMAL Conversion Function");
			code.AppendLine(string.Format("\t\tpublic static {0} ConvertFromXAMLObject({1} Data)", o.HasClientType ? o.ClientType.Name : o.Name, o.XAMLType.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tif (Data.DataObject != null) return Data.DataObject;");
			code.AppendLine(string.Format("\t\t\treturn new {0}(Data);", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t}");
			code.AppendLine();

			int protoCount = 0;
			foreach (DataElement de in o.Elements.Where(a => !a.IsHidden && a.IsDataMember))
			{
				code.Append(o.CMDEnabled && de.DCMEnabled ? GenerateElementDCMProxyCode45(de, ref protoCount) : GenerateElementProxyCode45(de, ref protoCount));
				code.AppendLine();
			}
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
			else code.AppendLine(string.Format("\t[DataContract({0}Name = \"{1}\", Namespace = \"{2}\")]", o.IsReference ? "IsReference = true, " : "", o.HasClientType ? o.ClientType.Name : o.Name, o.Parent.URI));
			code.AppendLine(string.Format("\t{0}", DataTypeGenerator.GenerateTypeDeclaration(o.HasClientType ? o.ClientType : o, o.ClientHasImpliedExtensionData, o.HasWinFormsBindings, o.CMDEnabled, o.CMDEnabled)));
			code.AppendLine("\t{");
			if (o.HasXAMLType)
			{
				code.AppendLine(string.Format("\t\tprivate {0} xamlObject;", o.XAMLType.Name));
				code.AppendLine(string.Format("\t\tpublic {0} XAMLObject {{ get {{ if(xamlObject == null) Application.Current.Dispatcher.Invoke(() => {{ xamlObject = new {0}(this); }}, System.Windows.Threading.DispatcherPriority.Normal); return xamlObject; }} private set {{ xamlObject = value; }} }}", o.XAMLType.Name));
				code.AppendLine();
			}
			code.Append(GenerateProxyDCMCode(o));
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

			code.AppendLine("\t\t//Implicit Conversion");
			code.AppendLine(string.Format("\t\tpublic static implicit operator {0}({1} Data)", o.HasClientType ? o.ClientType.Name : o.Name, o.XAMLType.Name));
			code.AppendLine("\t\t{");
			if (o.CMDEnabled)
			{
				code.AppendLine("\t\t\tif (Data == null) return null;");
				code.AppendLine(string.Format("\t\t\t{0} v = null;", o.HasClientType ? o.ClientType.Name : o.Name));
				code.AppendLine("\t\t\tif (Application.Current.Dispatcher.CheckAccess()) v = ConvertFromXAMLObject(Data);");
				code.AppendLine("\t\t\telse Application.Current.Dispatcher.Invoke(() => { v = ConvertFromXAMLObject(Data); }, System.Windows.Threading.DispatcherPriority.Normal);");
				code.AppendLine("\t\t\treturn v;");
			}
			else
			{
				code.AppendLine("\t\t\tData.DataObject.UpdateFromXAML();");
				code.AppendLine("\t\t\treturn Data.DataObject;");
			}
			code.AppendLine("\t\t}");
			code.AppendLine();
			if (!o.CMDEnabled)
			{
				code.AppendLine("\t\tprivate void UpdateFromXAML()");
				code.AppendLine("\t\t{");
				foreach (DataElement de in o.Elements)
					code.Append(GenerateElementProxyUpdateCode45(de, o));
				code.AppendLine("\t\t}");
			}

			code.AppendLine("\t\t//Constuctors");
			code.AppendLine(string.Format("\t\tpublic {0}()", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			if (o.HasXAMLType) code.AppendLine(string.Format("\t\t\tXAMLObject = new {0}(this);", o.XAMLType.Name));
			foreach (DataElement de in o.Elements)
				if (de.XAMLType.TypeMode == DataTypeMode.Collection || de.XAMLType.TypeMode == DataTypeMode.Dictionary)
					code.AppendLine(string.Format("\t\t\t{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(de.HasClientType ? de.ClientType : de.DataType, o.CMDEnabled && de.DCMEnabled)), de.HasClientType ? de.ClientName : de.DataName));
			code.AppendLine("\t\t}");
			if (o.HasXAMLType)
			{
				code.AppendLine(string.Format("\t\tpublic {0}({1} Data)", o.HasClientType ? o.ClientType.Name : o.Name, o.XAMLType.Name));
				code.AppendLine("\t\t{");
				if (o.HasXAMLType) code.AppendLine("\t\t\tXAMLObject = Data;");
				foreach (DataElement de in o.Elements)
					code.Append(GenerateElementProxyConstructorCode45(de, o));
				code.AppendLine("\t\t}");
			}
			code.AppendLine();

			code.AppendLine("\t\t//DTO->XMAL Conversion Function");
			code.AppendLine(string.Format("\t\tpublic static {0} ConvertFromXAMLObject({1} Data)", o.HasClientType ? o.ClientType.Name : o.Name, o.XAMLType.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tif (Data.DataObject != null) return Data.DataObject;");
			code.AppendLine(string.Format("\t\t\treturn new {0}(Data);", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t}");
			code.AppendLine();

			int protoCount = 0;
			foreach (DataElement de in o.Elements.Where(a => !a.IsHidden && a.IsDataMember))
			{
				code.Append(o.CMDEnabled && de.DCMEnabled ? GenerateElementDCMProxyCode45(de, ref protoCount) : GenerateElementProxyCode45(de, ref protoCount));
				code.AppendLine();
			}
			code.AppendLine("\t}");
			return code.ToString();
		}

		private static string GenerateProxyDCMCode(Data o)
		{
			if (!o.CMDEnabled) return "";
			DataElement dcmid = o.Elements.FirstOrDefault(a => a.IsDCMID);
			if (dcmid == null)
				return "";

			var code = new StringBuilder();
			code.AppendLine("\t\t//Data Change Messaging Support");
			code.AppendLine("\t\tprotected override void BatchUpdates() { }");
			code.AppendLine(string.Format("\t\tprivate static readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, {0}> __dcm;", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine(string.Format("\t\tstatic {0}()", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine(string.Format("\t\t\t__dcm = new System.Collections.Concurrent.ConcurrentDictionary<Guid, {0}>();", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t}");
			code.AppendLine(string.Format("\t\tpublic static bool HasData({0} data)", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine(string.Format("\t\t\treturn __dcm.ContainsKey(data.{0});", dcmid.HasClientType ? dcmid.ClientName : dcmid.DataName));
			code.AppendLine("\t\t}");
			code.AppendLine(string.Format("\t\tpublic static {0} RegisterData({0} data)", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine(string.Format("\t\t\treturn __dcm.GetOrAdd(data.{0}, data);", dcmid.HasClientType ? dcmid.ClientName : dcmid.DataName));
			code.AppendLine("\t\t}");
			code.AppendLine(string.Format("\t\tpublic static bool UnregisterData({0} data)", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine(string.Format("\t\t\t{0} t;", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine(string.Format("\t\t\treturn __dcm.TryRemove(data.{0}, out t);", dcmid.HasClientType ? dcmid.ClientName : dcmid.DataName));
			code.AppendLine("\t\t}");
			code.AppendLine("\t\t[OnDeserializing]");
			code.AppendLine("\t\tprotected void OnDeserializingMethod(StreamingContext context)");
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tbase.OnDeserializingBase(context);");
			code.AppendLine(string.Format("\t\t\tBatchInterval = {0};", o.DCMBatchCount > 0 ? o.DCMBatchCount : -1));
			code.AppendLine("\t\t}");
			code.AppendLine();
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
			code.AppendLine(string.Format("\t\tprivate {0} dataObject;", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine(string.Format("\t\tpublic {0} DataObject {{ get {{ return dataObject ?? (dataObject = new {0}(this)); }} private set {{ dataObject = value; }} }}", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine();

			if (o.XAMLHasExtensionData)
			{
				code.AppendLine("\t\tpublic System.Runtime.Serialization.ExtensionDataObject ExtensionData { get; set; }");
				code.AppendLine();
			}

			code.AppendLine("\t\t//Properties");
			foreach (DataElement de in o.Elements.Where(a => !a.IsHidden && a.IsDataMember))
			{
				code.Append(GenerateElementXAMLCode45(de));
				code.AppendLine();
			}

			code.AppendLine("\t\t//Implicit Conversion");
			code.AppendLine(string.Format("\t\tpublic static implicit operator {0}({1} Data)", o.XAMLType.Name, o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			if (o.CMDEnabled)
			{
				code.AppendLine("\t\t\tif (Data == null) return null;");
				code.AppendLine(string.Format("\t\t\t{0} v = null;", o.XAMLType.Name));
				code.AppendLine("\t\t\tif (Application.Current.Dispatcher.CheckAccess()) v = ConvertToXAMLObject(Data);");
				code.AppendLine("\t\t\telse Application.Current.Dispatcher.Invoke(() => { v = ConvertToXAMLObject(Data); }, System.Windows.Threading.DispatcherPriority.Normal);");
				code.AppendLine("\t\t\treturn v;");
			}
			else
			{
				code.AppendLine("\t\t\tData.XAMLObject.UpdateFromDTO();");
				code.AppendLine("\t\t\treturn Data.XAMLObject;");
			}
			code.AppendLine("\t\t}");
			code.AppendLine();
			if (!o.CMDEnabled)
			{
				code.AppendLine("\t\tprivate void UpdateFromDTO()");
				code.AppendLine("\t\t{");
				foreach (DataElement de in o.Elements)
					code.Append(GenerateElementXAMLUpdateCode45(de, o));
				code.AppendLine("\t\t}");
				code.AppendLine();
			}

			code.AppendLine("\t\t//Constructors");
			code.AppendLine(string.Format("\t\tpublic {0}()", o.XAMLType.Name));
			code.AppendLine("\t\t{");
			code.AppendLine(string.Format("\t\t\tDataObject = new {0}(this);", o.HasClientType ? o.ClientType.Name : o.Name));
			foreach (DataElement de in o.Elements)
				if (de.XAMLType.TypeMode == DataTypeMode.Collection || de.XAMLType.TypeMode == DataTypeMode.Dictionary || de.XAMLType.TypeMode == DataTypeMode.Dictionary || de.XAMLType.TypeMode == DataTypeMode.Stack)
					code.AppendLine(string.Format("\t\t\t{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(de.XAMLType, o.CMDEnabled && de.DCMEnabled)), de.XAMLName));
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}({1} Data)", o.XAMLType.Name, o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tDataObject = Data;");
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementXAMLConstructorCode45(de, o));
			code.AppendLine("\t\t}");
			code.AppendLine();

			code.AppendLine("\t\t//XAML->DTO Conversion Function");
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
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.Append("\t\t");
			if (o.IsDataMember)
			{
				if (o.IsDataMember && o.ProtocolBufferEnabled && o.Owner.EnableProtocolBuffers) code.AppendFormat("[ProtoBuf.ProtoMember({0}{1}{2}{3}{4}{5}{6})] ", ++ProtoCount, o.ProtoDataFormat != ProtoBufDataFormat.Default ? string.Format(", DataFormat = ProtoBuf.DataFormat.{0}", System.Enum.GetName(typeof(ProtoBufDataFormat), o.ProtoDataFormat)) : "", o.IsRequired ? ", IsRequired = true" : "", o.ProtoIsPacked ? ", IsPacked = true" : "", o.ProtoOverwriteList ? ", OverwriteList = true" : "", o.ProtoAsReference ? ", AsReference = true" : "", o.ProtoDynamicType ? ", DynamicType = true" : "");
				else code.AppendFormat("[DataMember({0}{1}{2}Name = \"{3}\")] ", o.EmitDefaultValue ? "EmitDefaultValue = false, " : "", o.IsRequired ? "IsRequired = true, " : "", o.ProtocolBufferEnabled ? string.Format("Order = {0}, ", ProtoCount) : o.Order >= 0 ? string.Format("Order = {0}, ", o.Order) : "", o.HasClientType ? o.ClientName : o.DataName);
			}
			else
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.XML) code.Append("[XmlIgnore] ");
			code.AppendLine(string.Format("public {0} {1} {{ get; {2}set; }}", DataTypeGenerator.GenerateType(o.DataType), o.DataName, o.IsReadOnly ? "protected " : ""));
			return code.ToString();
		}

		private static string GenerateElementDCMServerCode45(DataElement o, ref int ProtoCount)
		{
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.Append("\t\t");
			if (o.ProtocolBufferEnabled && o.Owner.EnableProtocolBuffers) code.AppendFormat("[ProtoBuf.ProtoMember({0}{1}{2}{3}{4}{5}{6})] ", ++ProtoCount, o.ProtoDataFormat != ProtoBufDataFormat.Default ? string.Format(", DataFormat = ProtoBuf.DataFormat.{0}", System.Enum.GetName(typeof(ProtoBufDataFormat), o.ProtoDataFormat)) : "", o.IsRequired ? ", IsRequired = true" : "", o.ProtoIsPacked ? ", IsPacked = true" : "", o.ProtoOverwriteList ? ", OverwriteList = true" : "", o.ProtoAsReference ? ", AsReference = true" : "", o.ProtoDynamicType ? ", DynamicType = true" : "");
			else code.AppendFormat("[DataMember({0}{1}{2}Name = \"{3}\")] ", o.EmitDefaultValue ? "EmitDefaultValue = false, " : "", o.IsRequired ? "IsRequired = true, " : "", o.ProtocolBufferEnabled ? string.Format("Order = {0}, ", ProtoCount) : o.Order >= 0 ? string.Format("Order = {0}, ", o.Order) : "", o.HasClientType ? o.ClientName : o.DataName);
			code.AppendLine(string.Format("public {0} {1} {{ get {{ return GetValue({1}Property); }} {2}set {{ SetValue({1}Property, value); {3}}} }}", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.IsReadOnly ? "protected " : "", o.GenerateWinFormsSupport ? "NotifyPropertyChanged(); " : ""));
			if (o.DataType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\tpublic static readonly DeltaProperty<DeltaList<{3}>> {1}Property = DeltaProperty<{3}>.RegisterList<{3}>(\"{1}\", typeof({2}), (s, o, n) => {{ var t = s as {2}; if (t == null) return;", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType.CollectionGenericType : o.DataType.CollectionGenericType)));
				code.AppendLine(string.Format("\t\t\tn.Added += (x) => t.{0}Added(x);", o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tn.Removed += (x) => t.{0}Removed(x);", o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tn.Cleared += (x) => t.{0}Cleared(x);", o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tn.Inserted += (idx, x) => t.{0}Inserted(idx, x);", o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tn.RemovedAt += (idx, x) => t.{0}RemovedAt(idx, x);", o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tn.Moved += (x, nidx) => t.{0}Moved(x, nidx);", o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tn.Replaced += (ox, nx) => t.{0}Replaced(ox, nx);", o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine("\t\t\to.ClearEventHandlers();");
				code.AppendLine("\t\t});");
				code.AppendLine(string.Format("\t\tpartial void {0}Added(IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.CollectionGenericType : o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Removed(IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.CollectionGenericType : o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Cleared(IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.CollectionGenericType : o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Inserted(int Index, IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.CollectionGenericType : o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}RemovedAt(int Index, IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.CollectionGenericType : o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Moved({1} Value, int Index);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.CollectionGenericType : o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Replaced({1} OldValue, {1} NewValue);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.CollectionGenericType : o.DataType.CollectionGenericType))));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Stack)
			{
			}
			else if (o.DataType.TypeMode == DataTypeMode.Queue)
			{
			}
			else if (o.DataType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\tpublic static readonly DeltaProperty<DeltaDictionary<{3}, {4}>> {1}Property = DeltaProperty<{3}>.RegisterDictionary<{3}, {4}>(\"{1}\", typeof({2}), (s, o, n) => {{ var t = s as {2}; if (t == null) return;", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType.DictionaryKeyGenericType : o.DataType.DictionaryKeyGenericType), DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType.DictionaryValueGenericType : o.DataType.DictionaryValueGenericType)));
				code.AppendLine(string.Format("\t\t\tn.Added += (xk, xv) => t.{0}Added(xk, xv);", o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tn.Removed += (xk, xv) => t.{0}Removed(xk, xv);", o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tn.Cleared += (x) => t.{0}Cleared(x);", o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tn.Updated += (xk, ox, nx) => t.{0}Updated(xk, ox, nx);", o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine("\t\t\to.ClearEventHandlers();");
				code.AppendLine("\t\t});");
				code.AppendLine(string.Format("\t\tpartial void {0}Added({1} Key, {2} Value);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.DictionaryKeyGenericType : o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.DictionaryValueGenericType : o.DataType.DictionaryValueGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Removed({1} Key, {2} Value);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.DictionaryKeyGenericType : o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.DictionaryValueGenericType : o.DataType.DictionaryValueGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Cleared(IEnumerable<KeyValuePair<{1}, {2}>> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.DictionaryKeyGenericType : o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.DictionaryValueGenericType : o.DataType.DictionaryValueGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Updated({1} Key, {2} OldValue, {2} NewValue);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.DictionaryKeyGenericType : o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.DictionaryValueGenericType : o.DataType.DictionaryValueGenericType))));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\tpublic static readonly DeltaProperty<{2}> {0}Property = DeltaProperty<{2}>.Register<{2}>(\"{0}\", typeof({1}), default({2}), (s, o, n) => {{ var t = s as {1}; if (t == null) return; t.{0}PropertyChanged(o, n); }});", o.DataName, o.Owner.Name, DataTypeGenerator.GenerateType(o.DataType)));
				code.AppendLine(string.Format("\t\tpartial void {0}PropertyChanged({1} OldValue, {1} NewValue);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.DCMEnabled))));
			}
			else
			{
				code.AppendLine(string.Format("\t\tpublic static readonly DeltaProperty<{2}> {0}Property = DeltaProperty<{2}>.Register<{2}>(\"{0}\", typeof({1}), default({2}), (s, o, n) => {{ var t = s as {1}; if (t == null) return; t.{0}PropertyChanged(o, n); }});", o.DataName, o.Owner.Name, DataTypeGenerator.GenerateType(o.DataType)));
				code.AppendLine(string.Format("\t\tpartial void {0}PropertyChanged({1} OldValue, {1} NewValue);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.DCMEnabled))));
			}
			return code.ToString();
		}

		private static string GenerateElementProxyCode45(DataElement o, ref int ProtoCount)
		{
			var code = new StringBuilder();
			code.AppendLine(string.Format("\t\tprivate {0} {1}Field;", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName));
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.Append("\t\t");
			if (o.ProtocolBufferEnabled && o.Owner.EnableProtocolBuffers) code.AppendFormat("[ProtoBuf.ProtoMember({0}{1}{2}{3}{4}{5}{6})] ", ++ProtoCount, o.ProtoDataFormat != ProtoBufDataFormat.Default ? string.Format(", DataFormat = ProtoBuf.DataFormat.{0}", System.Enum.GetName(typeof(ProtoBufDataFormat), o.ProtoDataFormat)) : "", o.IsRequired ? ", IsRequired = true" : "", o.ProtoIsPacked ? ", IsPacked = true" : "", o.ProtoOverwriteList ? ", OverwriteList = true" : "", o.ProtoAsReference ? ", AsReference = true" : "", o.ProtoDynamicType ? ", DynamicType = true" : "");
			else code.AppendFormat("[DataMember({0}{1}{2}Name = \"{3}\")] ", o.EmitDefaultValue ? "EmitDefaultValue = false, " : "", o.IsRequired ? "IsRequired = true, " : "", o.ProtocolBufferEnabled ? string.Format("Order = {0}, ", ProtoCount) : o.Order >= 0 ? string.Format("Order = {0}, ", o.Order) : "", o.HasClientType ? o.ClientName : o.DataName);
			code.AppendLine(string.Format("public {0} {1} {{ get {{ return {1}Field; }} {2}set {{ {1}Field = value; {3}}} }}", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.IsReadOnly ? "protected " : "", o.GenerateWinFormsSupport ? "NotifyPropertyChanged(); " : ""));
			return code.ToString();
		}

		private static string GenerateElementDCMProxyCode45(DataElement o, ref int ProtoCount)
		{
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.Append("\t\t");
			if (o.ProtocolBufferEnabled && o.Owner.EnableProtocolBuffers) code.AppendFormat("[ProtoBuf.ProtoMember({0}{1}{2}{3}{4}{5}{6})] ", ++ProtoCount, o.ProtoDataFormat != ProtoBufDataFormat.Default ? string.Format(", DataFormat = ProtoBuf.DataFormat.{0}", System.Enum.GetName(typeof(ProtoBufDataFormat), o.ProtoDataFormat)) : "", o.IsRequired ? ", IsRequired = true" : "", o.ProtoIsPacked ? ", IsPacked = true" : "", o.ProtoOverwriteList ? ", OverwriteList = true" : "", o.ProtoAsReference ? ", AsReference = true" : "", o.ProtoDynamicType ? ", DynamicType = true" : "");
			else code.AppendFormat("[DataMember({0}{1}{2}Name = \"{3}\")] ", o.EmitDefaultValue ? "EmitDefaultValue = false, " : "", o.IsRequired ? "IsRequired = true, " : "", o.ProtocolBufferEnabled ? string.Format("Order = {0}, ", ProtoCount) : o.Order >= 0 ? string.Format("Order = {0}, ", o.Order) : "", o.HasClientType ? o.ClientName : o.DataName);
			code.AppendLine(string.Format("public {0} {1} {{ get {{ return GetValue({1}Property); }} {2}set {{ SetValue({1}Property, value); {3}}} }}", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.IsReadOnly ? "protected " : "", o.GenerateWinFormsSupport ? "NotifyPropertyChanged(); " : ""));
			if (o.XAMLType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\tpublic static readonly DeltaProperty<DeltaList<{3}>> {1}Property = DeltaProperty<{3}>.RegisterList<{3}>(\"{1}\", typeof({2}), (s, o, n) => {{ var t = s as {2}; if (t == null) return;", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType.CollectionGenericType : o.DataType.CollectionGenericType)));
				code.AppendLine(string.Format("\t\t\tn.Added += (x) => {{ foreach (var z in x) t.XAMLObject.{1}.AddNoUpdate(z); t.{0}Added(x); }};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tn.Removed += (x) => {{ foreach (var z in x) t.XAMLObject.{1}.RemoveNoUpdate(z); t.{0}Removed(x); }};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tn.Cleared += (x) => {{ t.XAMLObject.{1}.ClearNoUpdate(); t.{0}Cleared(x); }};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tn.Inserted += (idx, x) => {{ int c = idx; foreach (var z in x) t.XAMLObject.{1}.InsertNoUpdate(c++, z); t.{0}Inserted(idx, x); }};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tn.RemovedAt += (idx, x) => {{ foreach (var z in x) t.XAMLObject.{1}.RemoveNoUpdate(z); t.{0}RemovedAt(idx, x); }};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tn.Moved += (x, nidx) => {{ t.XAMLObject.{1}.MoveNoUpdate(x, nidx); t.{0}Moved(x, nidx); }};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tn.Replaced += (ox, nx) => {{ t.XAMLObject.{1}.ReplaceNoUpdate(ox, nx); t.{0}Replaced(ox, nx); }};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine("\t\t\to.ClearEventHandlers();");
				code.AppendLine("\t\t});");
				code.AppendLine(string.Format("\t\tpartial void {0}Added(IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.CollectionGenericType : o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Removed(IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.CollectionGenericType : o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Cleared(IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.CollectionGenericType : o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Inserted(int Index, IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.CollectionGenericType : o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}RemovedAt(int Index, IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.CollectionGenericType : o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Moved({1} Value, int Index);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.CollectionGenericType : o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Replaced({1} OldValue, {1} NewValue);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.CollectionGenericType : o.DataType.CollectionGenericType))));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Stack)
			{
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Queue)
			{
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\tpublic static readonly DeltaProperty<DeltaDictionary<{3}, {4}>> {1}Property = DeltaProperty<{3}>.RegisterDictionary<{3}, {4}>(\"{1}\", typeof({2}), (s, o, n) => {{ var t = s as {2}; if (t == null) return;", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType.DictionaryKeyGenericType : o.DataType.DictionaryKeyGenericType), DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType.DictionaryValueGenericType : o.DataType.DictionaryValueGenericType)));
				code.AppendLine(string.Format("\t\t\tn.Added += (xk, xv) => {{ t.XAMLObject.{1}.AddOrUpdateNoUpdate(xk, xv, (k,v) => xv);  t.{0}Added(xk, xv); }};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tn.Removed += (xk, xv) => {{ {2} result; t.XAMLObject.{1}.TryRemoveNoUpdate(xk, out result); t.{0}Removed(xk, xv); }};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.DictionaryValueGenericType))));
				code.AppendLine(string.Format("\t\t\tn.Cleared += (x) => {{ t.XAMLObject.{1}.ClearNoUpdate(); t.{0}Cleared(x); }};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tn.Updated += (xk, ox, nx) => {{ t.XAMLObject.{1}.AddOrUpdateNoUpdate(xk, nx, (k,v) => nx); t.{0}Updated(xk, ox, nx); }};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine("\t\t\to.ClearEventHandlers();");
				code.AppendLine("\t\t});");
				code.AppendLine(string.Format("\t\tpartial void {0}Added({1} Key, {2} Value);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.DictionaryKeyGenericType : o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.DictionaryValueGenericType : o.DataType.DictionaryValueGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Removed({1} Key, {2} Value);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.DictionaryKeyGenericType : o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.DictionaryValueGenericType : o.DataType.DictionaryValueGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Cleared(IEnumerable<KeyValuePair<{1}, {2}>> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.DictionaryKeyGenericType : o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.DictionaryValueGenericType : o.DataType.DictionaryValueGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Updated({1} Key, {2} OldValue, {2} NewValue);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.DictionaryKeyGenericType : o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType.DictionaryValueGenericType : o.DataType.DictionaryValueGenericType))));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\tpublic static readonly DeltaProperty<{3}> {1}Property = DeltaProperty<{3}>.Register<{3}>(\"{1}\", typeof({2}), default({3}), (s, o, n) => {{ var t = s as {2}; var nv = n as {0}; if (t == null || nv == null) return; var z = new {5}[nv.Length]; for (int i = 0; i < nv.Length; i++) z[i] = nv[i]; t.XAMLObject.{4} = z; t.{1}PropertyChanged(o, n); }});", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.CollectionGenericType, o.DCMEnabled))));
				code.AppendLine(string.Format("\t\tpartial void {0}PropertyChanged({1} OldValue, {1} NewValue);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.DCMEnabled))));
			}
			else
			{
				code.AppendLine(string.Format("\t\tpublic static readonly DeltaProperty<{3}> {1}Property = DeltaProperty<{3}>.Register<{3}>(\"{1}\", typeof({2}), default({3}), (s, o, n) => {{ var t = s as {2}; if (t == null) return; t.XAMLObject.{4} = n; t.{1}PropertyChanged(o, n); }});", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.XAMLName));
				code.AppendLine(string.Format("\t\tpartial void {0}PropertyChanged({1} OldValue, {1} NewValue);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.DCMEnabled))));
			}
			return code.ToString();
		}

		private static DataType GetPreferredDTOType(DataType value, bool IsDCMEnabled = false)
		{
			if (value.TypeMode == DataTypeMode.Array)
			{
				if (value.CollectionGenericType.TypeMode == DataTypeMode.Class || value.CollectionGenericType.TypeMode == DataTypeMode.Struct)
				{
					var t = value.CollectionGenericType as Data;
					if (t == null) return value;
					return new DataType(DataTypeMode.Array) { Name = value.Name, CollectionGenericType = t.HasClientType ? t.ClientType : t };
				}
			}
			else if (value.TypeMode == DataTypeMode.Collection || value.TypeMode == DataTypeMode.Stack || value.TypeMode == DataTypeMode.Queue)
			{
				if (value.CollectionGenericType.TypeMode == DataTypeMode.Class || value.CollectionGenericType.TypeMode == DataTypeMode.Struct)
				{
					var t = value.CollectionGenericType as Data;
					if (t == null) return value;
					if (IsDCMEnabled)
					{
						if (value.TypeMode == DataTypeMode.Collection) return new DataType("DeltaList", value.TypeMode) { Name = "DeltaList", CollectionGenericType = t.HasClientType ? t.ClientType : t };
						if (value.TypeMode == DataTypeMode.Collection) return new DataType("DeltaStack", value.TypeMode) { Name = "DeltaStack", CollectionGenericType = t.HasClientType ? t.ClientType : t };
						if (value.TypeMode == DataTypeMode.Collection) return new DataType("DeltaQueue", value.TypeMode) { Name = "DeltaQueue", CollectionGenericType = t.HasClientType ? t.ClientType : t };
					}
					return new DataType(value.Name, value.TypeMode) { Name = value.Name, CollectionGenericType = t.HasClientType ? t.ClientType : t };
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
					dk = t.HasClientType ? t.ClientType : t;
				}
				if (value.DictionaryValueGenericType.TypeMode == DataTypeMode.Class || value.DictionaryValueGenericType.TypeMode == DataTypeMode.Struct)
				{
					var t = value.DictionaryValueGenericType as Data;
					if (t == null) return value;
					dv = t.HasClientType ? t.ClientType : t;
				}
				if (IsDCMEnabled) return new DataType("DeltaDictionary", DataTypeMode.Dictionary) { Name = "DeltaDictionary", DictionaryKeyGenericType = dk, DictionaryValueGenericType = dv };
				return new DataType(value.Name, DataTypeMode.Dictionary) { Name = value.Name, DictionaryKeyGenericType = dk, DictionaryValueGenericType = dv };
			}
			else if (value.TypeMode == DataTypeMode.Class || value.TypeMode == DataTypeMode.Struct)
			{
				var t = value as Data;
				if (t == null) return value;
				return t.HasClientType ? t.ClientType : t;
			}
			return value;
		}

		private static DataType GetPreferredXAMLType(DataType value, bool IsDCMEnabled = false)
		{
			if (value.TypeMode == DataTypeMode.Array)
			{
				if (value.CollectionGenericType.TypeMode == DataTypeMode.Class || value.CollectionGenericType.TypeMode == DataTypeMode.Struct)
				{
					var t = value.CollectionGenericType as Data;
					if (t == null) return value;
					return t.HasXAMLType ? new DataType(DataTypeMode.Array) { Name = value.Name, CollectionGenericType = t.XAMLType } : new DataType(DataTypeMode.Array) { Name = value.Name, CollectionGenericType = t.HasClientType ? t.ClientType : t };
				}
			}
			else if (value.TypeMode == DataTypeMode.Collection || value.TypeMode == DataTypeMode.Stack || value.TypeMode == DataTypeMode.Queue)
			{
				if (value.CollectionGenericType.TypeMode == DataTypeMode.Class || value.CollectionGenericType.TypeMode == DataTypeMode.Struct)
				{
					var t = value.CollectionGenericType as Data;
					if (t == null) return value;
					if (IsDCMEnabled)
					{
						if (value.TypeMode == DataTypeMode.Collection) return t.HasXAMLType ? new DataType("DependencyList", value.TypeMode) { Name = "DependencyList", CollectionGenericType = t.XAMLType } : new DataType("DependencyList", value.TypeMode) { Name = "DependencyList", CollectionGenericType = t.HasClientType ? t.ClientType : t };
						if (value.TypeMode == DataTypeMode.Collection) return t.HasXAMLType ? new DataType("DependencyStack", value.TypeMode) { Name = "DependencyStack", CollectionGenericType = t.XAMLType } : new DataType("DependencyStack", value.TypeMode) { Name = "DependencyStack", CollectionGenericType = t.HasClientType ? t.ClientType : t };
						if (value.TypeMode == DataTypeMode.Collection) return t.HasXAMLType ? new DataType("DependencyQueue", value.TypeMode) { Name = "DependencyQueue", CollectionGenericType = t.XAMLType } : new DataType("DependencyQueue", value.TypeMode) { Name = "DependencyQueue", CollectionGenericType = t.HasClientType ? t.ClientType : t };
					}
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
				if (IsDCMEnabled) return new DataType("DependencyDictionary", DataTypeMode.Dictionary) { Name = "DependencyDictionary", DictionaryKeyGenericType = dk, DictionaryValueGenericType = dv };
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
				code.AppendLine(string.Format("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue{2}({1}Property); }} set {{ SetValue{2}({1}Property, value); }} }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.Owner.CMDEnabled && o.DCMEnabled)), o.XAMLName, o.Owner.CMDEnabled && o.DCMEnabled ? "Threaded" : ""));
				code.Append(GenerateElementXAMLDPCode45(o));
			}
			if (o.IsReadOnly && o.IsAttached == false)
			{
				code.AppendLine(string.Format("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue{2}({1}Property); }} protected set {{ SetValue{2}({1}PropertyKey, value); }} }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.Owner.CMDEnabled && o.DCMEnabled)), o.XAMLName, o.Owner.CMDEnabled && o.DCMEnabled ? "Threaded" : ""));
				code.Append(GenerateElementXAMLDPCode45(o));
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
				code.AppendLine(string.Format("\t\tpublic static {0} Get{1}(DependencyObject{2} obj) {{ return ({0})obj.GetValue{3}({1}Property); }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.Owner.CMDEnabled && o.DCMEnabled)), o.XAMLName, o.Owner.CMDEnabled && o.DCMEnabled ? "Ex" : "", o.Owner.CMDEnabled && o.DCMEnabled ? "Threaded" : ""));
				code.AppendLine(string.Format("\t\tpublic static void Set{1}(DependencyObject{2} obj, {0} value) {{ obj.SetValue{3}({1}Property, value); }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.Owner.CMDEnabled && o.DCMEnabled)), o.XAMLName, o.Owner.CMDEnabled && o.DCMEnabled ? "Ex" : "", o.Owner.CMDEnabled && o.DCMEnabled ? "Threaded" : ""));
				code.Append(GenerateElementXAMLDPCode45(o));
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
				code.AppendLine(string.Format("\t\tpublic static {0} Get{1}(DependencyObject{2} obj) {{ return ({0})obj.GetValue{3}({1}Property); }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.Owner.CMDEnabled && o.DCMEnabled)), o.XAMLName, o.Owner.CMDEnabled && o.DCMEnabled ? "Ex" : "", o.Owner.CMDEnabled && o.DCMEnabled ? "Threaded" : ""));
				code.AppendLine(string.Format("\t\tpublic static void Set{1}(DependencyObject{2} obj, {0} value) {{ obj.SetValue{3}({1}PropertyKey, value); }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.Owner.CMDEnabled && o.DCMEnabled)), o.XAMLName, o.Owner.CMDEnabled && o.DCMEnabled ? "Ex" : "", o.Owner.CMDEnabled && o.DCMEnabled ? "Threaded" : ""));
				code.Append(GenerateElementXAMLDPCode45(o));
			}
			return code.ToString();
		}

		private static string GenerateElementXAMLDPCode45(DataElement o)
		{
			var code = new StringBuilder();

			if (o.Owner.CMDEnabled && o.DCMEnabled)
			{
				if (o.XAMLType.TypeMode == DataTypeMode.Collection)
				{
					code.AppendLine(string.Format("\t\tprivate static readonly PropertyMetadata {0}PropertyMetadata = new PropertyMetadata((o, e) => {{ var t = o as {1}; var nl = e.NewValue as DependencyList<{2}>; var ol = e.OldValue as DependencyList<{2}>; if (t == null || nl == null || ol == null) return; ", o.XAMLName, o.Owner.XAMLType.Name, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.CollectionGenericType))));
					code.AppendLine(string.Format("\t\t\tnl.Added += (x) => {{ foreach (var z in x) t.DataObject.{1}.AddNoUpdate(z); t.{0}Added(x); }};", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
					code.AppendLine(string.Format("\t\t\tnl.Removed += (x) => {{ foreach (var z in x) t.DataObject.{1}.RemoveNoUpdate(z); t.{0}Removed(x); }};", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
					code.AppendLine(string.Format("\t\t\tnl.Cleared += (x) => {{ t.DataObject.{1}.ClearNoUpdate(); t.{0}Cleared(x); }};", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
					code.AppendLine(string.Format("\t\t\tnl.Inserted += (idx, x) => {{ int c = idx; foreach (var z in x) t.DataObject.{1}.InsertNoUpdate(c++, z); t.{0}Inserted(idx, x); }};", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
					code.AppendLine(string.Format("\t\t\tnl.RemovedAt += (idx, x) => {{ foreach (var z in x) t.DataObject.{1}.RemoveNoUpdate(z); t.{0}RemovedAt(idx, x); }};", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
					code.AppendLine(string.Format("\t\t\tnl.Moved += (x, nidx) => {{ t.DataObject.{1}.MoveNoUpdate(x, nidx); t.{0}Moved(x, nidx); }};", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
					code.AppendLine(string.Format("\t\t\tnl.Replaced += (ox, nx) => {{ t.DataObject.{1}.ReplaceNoUpdate(ox, nx); t.{0}Replaced(ox, nx); }};", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
					code.AppendLine("\t\t\tol.ClearEventHandlers();");
					code.AppendLine("\t\t});");
					code.AppendLine(string.Format("\t\tpartial void {0}Added(IEnumerable<{1}> Values);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.CollectionGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}Removed(IEnumerable<{1}> Values);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.CollectionGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}Cleared(IEnumerable<{1}> Values);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.CollectionGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}Inserted(int Index, IEnumerable<{1}> Values);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.CollectionGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}RemovedAt(int Index, IEnumerable<{1}> Values);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.CollectionGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}Moved({1} Value, int Index);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.CollectionGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}Replaced({1} OldValue, {1} NewValue);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.CollectionGenericType))));
				}
				else if (o.XAMLType.TypeMode == DataTypeMode.Stack)
				{
				}
				else if (o.XAMLType.TypeMode == DataTypeMode.Queue)
				{
				}
				else if (o.XAMLType.TypeMode == DataTypeMode.Dictionary)
				{
					code.AppendLine(string.Format("\t\tprivate static readonly PropertyMetadata {0}PropertyMetadata = new PropertyMetadata((o, e) => {{ var t = o as {1}; var nl = e.NewValue as DependencyDictionary<{2}, {3}>; var ol = e.OldValue as DependencyDictionary<{2}, {3}>; if (t == null || nl == null || ol == null) return; ", o.XAMLName, o.Owner.XAMLType.Name, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.DictionaryValueGenericType))));
					code.AppendLine(string.Format("\t\t\tnl.Added += (xk, xv) => {{ t.DataObject.{1}.AddOrUpdateNoUpdate(xk, xv, (k,v) => xv);  t.{0}Added(xk, xv); }};", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
					code.AppendLine(string.Format("\t\t\tnl.Removed += (xk, xv) => {{ {2} result; t.DataObject.{1}.TryRemoveNoUpdate(xk, out result); t.{0}Removed(xk, xv); }};", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName, o.HasClientType ? DataTypeGenerator.GenerateType(o.ClientType.DictionaryValueGenericType) : DataTypeGenerator.GenerateType(o.DataType.DictionaryValueGenericType)));
					code.AppendLine(string.Format("\t\t\tnl.Cleared += (x) => {{ t.DataObject.{1}.ClearNoUpdate(); t.{0}Cleared(x); }};", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
					code.AppendLine(string.Format("\t\t\tnl.Updated += (xk, ox, nx) => {{ t.DataObject.{1}.AddOrUpdateNoUpdate(xk, nx, (k,v) => nx); t.{0}Updated(xk, ox, nx); }};", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
					code.AppendLine("\t\t\tol.ClearEventHandlers();");
					code.AppendLine("\t\t});");
					code.AppendLine(string.Format("\t\tpartial void {0}Added({1} Key, {2} Value);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.DictionaryValueGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}Removed({1} Key, {2} Value);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.DictionaryValueGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}Cleared(IEnumerable<KeyValuePair<{1}, {2}>> Values);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.DictionaryValueGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}Updated({1} Key, {2} OldValue, {2} NewValue);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType.DictionaryValueGenericType))));
				}
				else if (o.XAMLType.TypeMode == DataTypeMode.Array)
				{
					code.AppendLine(string.Format("\t\tprivate static readonly PropertyMetadata {0}PropertyMetadata = new PropertyMetadata((o, e) => {{ var t = o as {3}; var nv = e.NewValue as {1}; if (t == null || nv == null) return; var z = new {4}[nv.Length]; for (int i = 0; i < nv.Length; i++) z[i] = nv[i]; t.DataObject.{2} = z;  t.{0}PropertyChanged(({1})e.NewValue, ({1})e.OldValue); }});", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.Owner.XAMLType.Name, o.HasClientType ? o.ClientType.CollectionGenericType : o.DataType.CollectionGenericType));
					code.AppendLine(string.Format("\t\tpartial void {0}PropertyChanged({1} OldValue, {1} NewValue);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.DCMEnabled))));
				}
				else
				{
					code.AppendLine(string.Format("\t\tprivate static readonly PropertyMetadata {0}PropertyMetadata = new PropertyMetadata((o, e) => {{ var t = o as {3}; if (t == null) return; t.DataObject.{2} = ({1})e.NewValue; t.{0}PropertyChanged(({1})e.NewValue, ({1})e.OldValue); }});", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.Owner.XAMLType.Name));
					code.AppendLine(string.Format("\t\tpartial void {0}PropertyChanged({1} OldValue, {1} NewValue);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.DCMEnabled))));
				}
			}

			code.AppendLine(string.Format("\t\tpublic static readonly DependencyProperty{6} {1}Property{6} = DependencyProperty.Register{4}{5}(\"{1}\", typeof({0}), typeof({2}){3});", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.DCMEnabled)), o.XAMLName, o.Owner.XAMLType.Name, o.Owner.CMDEnabled && o.DCMEnabled ? string.Format(", {0}PropertyMetadata", o.XAMLName) : ", null", o.IsAttached ? "Attached" : "", o.IsReadOnly ? "ReadOnly" : "", o.IsReadOnly ? "Key" : ""));
			if (o.IsReadOnly) code.AppendLine(string.Format("\t\tpublic static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;", o.XAMLName));

			return code.ToString();
		}

		private static string GenerateElementProxyConstructorCode40(DataElement o, Data c)
		{
			return GenerateElementProxyConstructorCode45(o, c);
		}

		private static string GenerateElementProxyConstructorCode45(DataElement o, Data c)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			if (!o.HasXAMLType) return "";
			var code = new StringBuilder();

			if (o.XAMLType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\t\t{0}[] v{3} = new {1}[Data.{2}.GetLength(0)];", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.Owner.CMDEnabled && o.DCMEnabled)).Replace("[]", ""), DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType).Replace("[]", ""), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) for(int i = 0; i < Data.{0}.GetLength(0); i++) {{ v{1}[i] = Data.{0}[i]; }}", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.Owner.CMDEnabled && o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Add(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.XAMLType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Stack)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.Owner.CMDEnabled && o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Push(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.XAMLType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Queue)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.Owner.CMDEnabled && o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Enqueue(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.XAMLType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.Owner.CMDEnabled && o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(o.XAMLType.Name == "System.Collections.Concurrent.ConcurrentDictionary" ? string.Format("\t\t\tif (Data.{1} != null) foreach(KeyValuePair<{0}> a in Data.{1}) {{ v{2}.TryAdd(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.XAMLType)), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName) : string.Format("\t\t\tif (Data.{1} != null) foreach(KeyValuePair<{0}> a in Data.{1}) {{ v{2}.Add(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.XAMLType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else
				code.AppendLine(string.Format("\t\t\t{1} = Data.{0};", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));

			return code.ToString();
		}

		// Updates the DTO from the XAML
		private static string GenerateElementProxyUpdateCode45(DataElement o, Data c)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			if (!o.HasXAMLType) return "";
			var code = new StringBuilder();

			if (o.XAMLType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\t\t{0}[] v{3} = new {1}[XAMLObject.{2}.GetLength(0)];", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.Owner.CMDEnabled && o.DCMEnabled)).Replace("[]", ""), DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType).Replace("[]", ""), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (XAMLObject.{1} != null) for(int i = 0; i < XAMLObject.{0}.GetLength(0); i++) {{ v{1}[i] = XAMLObject.{0}[i]; }}", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.Owner.CMDEnabled && o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (XAMLObject.{1} != null) foreach({0} a in XAMLObject.{1}) {{ v{2}.Add(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.XAMLType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Stack)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.Owner.CMDEnabled && o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (XAMLObject.{1} != null) foreach({0} a in XAMLObject.{1}) {{ v{2}.Push(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.XAMLType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Queue)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.Owner.CMDEnabled && o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (XAMLObject.{1} != null) foreach({0} a in XAMLObject.{1}) {{ v{2}.Enqueue(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.XAMLType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.HasClientType ? o.ClientType : o.DataType, o.Owner.CMDEnabled && o.DCMEnabled)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(o.XAMLType.Name == "System.Collections.Concurrent.ConcurrentDictionary" ? string.Format("\t\t\tif (XAMLObject.{1} != null) foreach(KeyValuePair<{0}> a in XAMLObject.{1}) {{ v{2}.TryAdd(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.XAMLType)), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName) : string.Format("\t\t\tif (XAMLObject.{1} != null) foreach(KeyValuePair<{0}> a in XAMLObject.{1}) {{ v{2}.Add(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.XAMLType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else
				code.AppendLine(string.Format("\t\t\t{1} = XAMLObject.{0};", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));

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
				code.AppendLine(string.Format("\t\t\t{0}[] v{3} = new {1}[Data.{2}.GetLength(0)];", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.Owner.CMDEnabled && o.DCMEnabled)).Replace("[]", ""), DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)).Replace("[]", ""), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) for(int i = 0; i < Data.{0}.GetLength(0); i++) {{ v{1}[i] = Data.{0}[i]; }}", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.Owner.CMDEnabled && o.DCMEnabled)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Add(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Stack)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.Owner.CMDEnabled && o.DCMEnabled)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Push(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Queue)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.Owner.CMDEnabled && o.DCMEnabled)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Enqueue(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.Owner.CMDEnabled && o.DCMEnabled)), o.XAMLName));
				code.AppendLine(o.XAMLType.Name == "System.Collections.Concurrent.ConcurrentDictionary" ? string.Format("\t\t\tif (Data.{1} != null) foreach(KeyValuePair<{0}> a in Data.{1}) {{ v{2}.TryAdd(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName) : string.Format("\t\t\tif (Data.{1} != null) foreach(KeyValuePair<{0}> a in Data.{1}) {{ v{2}.Add(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
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

		// Updates the XAML from the DTO
		private static string GenerateElementXAMLUpdateCode45(DataElement o, Data c)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			if (!o.HasXAMLType) return "";
			var code = new StringBuilder();

			if (o.XAMLType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\t\t{0}[] v{3} = new {1}[DataObject.{2}.GetLength(0)];", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.Owner.CMDEnabled && o.DCMEnabled)).Replace("[]", ""), DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)).Replace("[]", ""), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (DataObject.{1} != null) for(int i = 0; i < DataObject.{0}.GetLength(0); i++) {{ v{1}[i] = DataObject.{0}[i]; }}", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.Owner.CMDEnabled && o.DCMEnabled)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (DataObject.{1} != null) foreach({0} a in DataObject.{1}) {{ v{2}.Add(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Stack)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.Owner.CMDEnabled && o.DCMEnabled)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (DataObject.{1} != null) foreach({0} a in DataObject.{1}) {{ v{2}.Push(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Queue)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.Owner.CMDEnabled && o.DCMEnabled)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (DataObject.{1} != null) foreach({0} a in DataObject.{1}) {{ v{2}.Enqueue(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType, o.Owner.CMDEnabled && o.DCMEnabled)), o.XAMLName));
				code.AppendLine(o.XAMLType.Name == "System.Collections.Concurrent.ConcurrentDictionary" ? string.Format("\t\t\tif (DataObject.{1} != null) foreach(KeyValuePair<{0}> a in DataObject.{1}) {{ v{2}.TryAdd(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName) : string.Format("\t\t\tif (DataObject.{1} != null) foreach(KeyValuePair<{0}> a in DataObject.{1}) {{ v{2}.Add(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else
			{
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{1} = DataObject.{0};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				else code.AppendLine(string.Format("\t\t\t{2}.Set{1}(this, DataObject.{0});", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName, DataTypeGenerator.GenerateType(c.XAMLType)));
			}

			return code.ToString();
		}
	}
}