using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETPath.Projects;
using NETPath.Projects.Helpers;

namespace NETPath.Generators.WinRT.CS
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

			foreach (DataElement d in o.Elements)
			{
				if (RegExs.MatchCodeName.IsMatch(d.DataType.Name) == false && d.DataType.TypeMode != DataTypeMode.Array)
					AddMessage(new CompileMessage("GS3001", "The data element '" + d.DataName + "' in the '" + o.Name + "' data object contains invalid characters in the Name.", CompileMessageSeverity.ERROR, o, d, d.GetType(), o.Parent.Owner.ID));

				if (d.HasClientType)
					if (RegExs.MatchCodeName.IsMatch(d.ClientName) == false && d.DataType.TypeMode != DataTypeMode.Array)
						AddMessage(new CompileMessage("GS3002", "The data element '" + d.ClientName + "' in the '" + o.Name + "' data object contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

				if (d.HasXAMLType)
					if (RegExs.MatchCodeName.IsMatch(d.XAMLName) == false && d.DataType.TypeMode != DataTypeMode.Array)
						AddMessage(new CompileMessage("GS3003", "The data element '" + d.XAMLName + "' in the '" + o.Name + "' data object contains invalid characters in the XAML Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

				if (d.DREEnabled && d.DataType.TypeMode == DataTypeMode.Stack)
					AddMessage(new CompileMessage("GS3009", "Stacks are invalid for the data change enabled element '" + d.DataName + "' in the '" + o.Name + "' data object. Please replace it with a valid type.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
				if (d.DREEnabled && d.DataType.TypeMode == DataTypeMode.Queue)
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

		public static string GenerateImmediateDREValueCallbacks(IEnumerable<DataRevisionName> DRS, string Class, string Property, bool IsServer)
		{
			if (DRS == null || !DRS.Any()) return "";
			var code = new StringBuilder();
			foreach (var drs in DRS.Where(d => d.IsServer == IsServer))
				if (!IsServer && Globals.CurrentProjectID == drs.ProjectID) code.Append(string.Format("{0}Proxy.Current.Update{1}{2}DRE{4}(t.{3}, n); ", drs.Path, Class, Property, "_DREID", drs.IsAwaitable ? "Async" : ""));
				else if (IsServer) code.Append(string.Format("{0}Base.CallbackUpdate{1}{2}DRE{4}(Guid.Empty, t.{3}, n); ", drs.Path, Class, Property, "_DREID", drs.IsAwaitable ? "Async" : ""));
			return code.ToString();
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
			code.AppendLine(string.Format("\t{0}", DataTypeGenerator.GenerateTypeDeclaration(o, false, false, o.CMDEnabled, o.CMDEnabled, o.DREEnabled)));
			code.AppendLine("\t{");
	
			if (o.DataHasExtensionData)
			{
				code.AppendLine("\t\tpublic System.Runtime.Serialization.ExtensionDataObject ExtensionData { get; set; }");
				code.AppendLine();
			}

			code.Append(GenerateProxyDCMCode(o, true));

			int protoCount = 1;
			foreach (DataElement de in o.Elements.Where(a => !a.IsHidden))
				code.AppendLine(o.CMDEnabled ? GenerateElementDCMServerCode45(de, ref protoCount) : GenerateElementServerCode45(de, ref protoCount));
			code.AppendLine("\t}");
			return code.ToString();
		}

		public static string GenerateProxyCodeRT8(Data o)
		{
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			foreach (DataType dt in o.ClientType != null ? o.ClientType.KnownTypes : o.KnownTypes)
				code.AppendLine(string.Format("\t[KnownType(typeof({0}))]", dt));
			code.AppendLine("\t[System.Diagnostics.DebuggerStepThroughAttribute]");
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			if (o.EnableProtocolBuffers) code.AppendLine(string.Format("\t[ProtoBuf.ProtoContract({0}{1}{2})]", o.ProtoSkipConstructor ? "SkipConstructor = true" : "SkipConstructor = false", o.ProtoMembersOnly ? ", UseProtoMembersOnly = true" : "", o.ProtoIgnoreListHandling ? ", IgnoreListHandling = true" : ""));
			else code.AppendLine(string.Format("\t[DataContract({0}Name = \"{1}\", Namespace = \"{2}\")]", o.IsReference ? "IsReference = true, " : "", o.HasClientType ? o.ClientType.Name : o.Name, o.Parent.URI));
			code.AppendLine(string.Format("\t{0}", DataTypeGenerator.GenerateTypeDeclaration(o.HasClientType ? o.ClientType : o, false, o.HasWinFormsBindings, o.CMDEnabled, o.CMDEnabled, o.DREEnabled)));
			code.AppendLine("\t{");
			if (o.HasXAMLType)
			{
				if (!o.CMDEnabled) code.AppendLine(string.Format("\t\tprivate {0} BaseXAMLObject;", o.XAMLType.Name));
				code.AppendLine(string.Format("\t\tpublic {0} XAMLObject {{ get {{ if (BaseXAMLObject == null) Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {{ BaseXAMLObject = new {0}(this); }}).GetResults(); return BaseXAMLObject as {0}; }} {1}}}", o.XAMLType.Name, !o.CMDEnabled ? "set { BaseXAMLObject = value; } " : ""));
				code.AppendLine();
			}
			code.Append(GenerateProxyDCMCode(o));
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

			if (o.HasXAMLType)
			{
				code.AppendLine("\t\t//Implicit Conversion");
				code.AppendLine(string.Format("\t\tpublic static implicit operator {0}({1} Data)", o.HasClientType ? o.ClientType.Name : o.Name, o.XAMLType.Name));
				code.AppendLine("\t\t{");
				if (o.CMDEnabled)
				{
					code.AppendLine("\t\t\tif (Data == null) return null;");
					code.AppendLine(string.Format("\t\t\t{0} v = null;", o.HasClientType ? o.ClientType.Name : o.Name));
					code.AppendLine("\t\t\tif (Window.Current.Dispatcher.HasThreadAccess) v = ConvertFromXAMLObject(Data);");
					code.AppendLine("\t\t\telse Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { v = ConvertFromXAMLObject(Data); }).GetResults();");
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
					code.AppendLine();
				}
			}

			code.AppendLine("\t\t//Constuctors");
			code.AppendLine(string.Format("\t\tpublic {0}(){1}", o.HasClientType ? o.ClientType.Name : o.Name, o.DREBatchCount > 0 ? string.Format(" : base({0})", o.DREBatchCount) : ""));
			code.AppendLine("\t\t{");
			foreach (DataElement de in o.Elements)
				if (de.DataType.TypeMode == DataTypeMode.Collection || de.DataType.TypeMode == DataTypeMode.Dictionary || de.DataType.TypeMode == DataTypeMode.Queue || de.DataType.TypeMode == DataTypeMode.Stack)
					code.AppendLine(string.Format("\t\t\t{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(de.DataType, o.CMDEnabled)), de.HasClientType ? de.ClientName : de.DataName));
				else if (de.DataType.TypeMode == DataTypeMode.Array)
					code.AppendLine(string.Format("\t\t\t{1} = new {0}[0];", DataTypeGenerator.GenerateType(GetPreferredDTOType(de.DataType.CollectionGenericType, o.CMDEnabled)), de.HasClientType ? de.ClientName : de.DataName));
			if (o.HasXAMLType) code.AppendLine(string.Format("\t\t\tWindow.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {{ BaseXAMLObject = new {0}(this); }}).GetResults();", o.XAMLType.Name));
			if (o.CMDEnabled && o.HasXAMLType)
				foreach (DataElement de in o.Elements)
				{
					if (de.DataType.TypeMode == DataTypeMode.Collection && de.HasXAMLType)
						code.AppendLine(string.Format("\t\t\t{0}.SetEvents((x) => {{ foreach (var z in x) XAMLObject.{1}.AddNoUpdate(z); {0}Added(x); }}, (x) => {{ foreach (var z in x) XAMLObject.{1}.RemoveNoUpdate(z); {0}Removed(x); }}, (x) => {{ XAMLObject.{1}.ClearNoUpdate(); {0}Cleared(x); }}, (idx, x) => {{ int c = idx; foreach (var z in x) XAMLObject.{1}.InsertNoUpdate(c++, z); {0}Inserted(idx, x); }}, (idx, x) => {{ foreach (var z in x) XAMLObject.{1}.RemoveNoUpdate(z); {0}RemovedAt(idx, x); }}, (x, nidx) => {{ XAMLObject.{1}.MoveNoUpdate(x, nidx); {0}Moved(x, nidx); }}, (ox, nx) => {{ XAMLObject.{1}.ReplaceNoUpdate(nx, ox); {0}Replaced(ox, nx); }});", de.HasClientType ? de.ClientName : de.DataName, de.XAMLName));
					if (de.DataType.TypeMode == DataTypeMode.Dictionary && de.HasXAMLType)
						code.AppendLine(string.Format("\t\t\t{0}.SetEvents((xk, xv) => {{ XAMLObject.{1}.AddOrUpdateNoUpdate(xk, xv, (k,v) => xv);  {0}Added(xk, xv); }}, (xk, xv) => {{ {2} result; XAMLObject.{1}.TryRemoveNoUpdate(xk, out result); {0}Removed(xk, xv); }}, (x) => {{ XAMLObject.{1}.ClearNoUpdate(); {0}Cleared(x); }}, (xk, ox, nx) => {{ XAMLObject.{1}.AddOrUpdateNoUpdate(xk, nx, (k,v) => nx); {0}Updated(xk, ox, nx); }});", de.HasClientType ? de.ClientName : de.DataName, de.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(de.DataType.DictionaryValueGenericType))));
				}
			if (o.DREEnabled) code.AppendLine("\t\t\tEnableBatching = true;");
			code.AppendLine("\t\t}");
			if (o.HasXAMLType)
			{
				code.AppendLine(string.Format("\t\tpublic {0}({1} Data){2}", o.HasClientType ? o.ClientType.Name : o.Name, o.XAMLType.Name, o.CMDEnabled ? string.Format(" : base(Data{0})", o.DREBatchCount > 0 ? string.Format(", {0}", o.DREBatchCount) : "") : o.DREBatchCount > 0 ? string.Format(" : base({0})", o.DREBatchCount) : ""));
				code.AppendLine("\t\t{");
				if (!o.CMDEnabled) code.AppendLine("\t\t\tBaseXAMLObject = Data;");
				foreach (DataElement de in o.Elements)
					code.Append(GenerateElementProxyConstructorCode45(de, o));
				if (o.DREEnabled) code.AppendLine("\t\t\tEnableBatching = true;");
				code.AppendLine("\t\t}");
			}
			code.AppendLine();

			if (o.HasXAMLType)
			{
				code.AppendLine("\t\t//DTO->XMAL Conversion Function");
				code.AppendLine(string.Format("\t\tpublic static {0} ConvertFromXAMLObject({1} Data)", o.HasClientType ? o.ClientType.Name : o.Name, o.XAMLType.Name));
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tif (Data.DataObject != null) return Data.DataObject;");
				code.AppendLine(string.Format("\t\t\treturn new {0}(Data);", o.HasClientType ? o.ClientType.Name : o.Name));
				code.AppendLine("\t\t}");
				code.AppendLine();
			}

			int protoCount = 1;
			foreach (DataElement de in o.Elements.Where(a => !a.IsHidden && a.IsDataMember))
				code.AppendLine(o.CMDEnabled ? GenerateElementDCMProxyCode45(de, ref protoCount) : GenerateElementProxyCode45(de, ref protoCount));
			code.AppendLine("\t}");
			return code.ToString();
		}

		private static string GenerateProxyDCMCode(Data o, bool IsServer = false)
		{
			if (!o.CMDEnabled) return "";

			var code = new StringBuilder();
			code.AppendLine("\t\t//Concurrently Mutable Data Support");
			code.AppendLine("\t\t[OnDeserializing]");
			code.AppendLine("\t\tprivate void OnDeserializing(StreamingContext context)");
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tOnDeserializingBase(context);");
			if (o.DREEnabled) code.AppendLine(string.Format("\t\t\tBatchInterval = {0};", o.DREBatchCount));
			code.AppendLine("\t\t}");
			code.AppendLine("\t\t[OnDeserialized]");
			code.AppendLine("\t\tprivate void OnDeserialized(StreamingContext context)");
			code.AppendLine("\t\t{");
			if (o.HasXAMLType && !IsServer) code.AppendLine(string.Format("\t\t\tWindow.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {{ BaseXAMLObject = new {0}(this); }}).GetResults();", o.XAMLType.Name));
			if (o.CMDEnabled && o.HasXAMLType)
				foreach (DataElement de in o.Elements)
				{
					if (de.DataType.TypeMode == DataTypeMode.Collection && de.HasXAMLType)
						code.AppendLine(string.Format("\t\t\t{0}.SetEvents((x) => {{ foreach (var z in x) XAMLObject.{1}.AddNoUpdate(z); {0}Added(x); }}, (x) => {{ foreach (var z in x) XAMLObject.{1}.RemoveNoUpdate(z); {0}Removed(x); }}, (x) => {{ XAMLObject.{1}.ClearNoUpdate(); {0}Cleared(x); }}, (idx, x) => {{ int c = idx; foreach (var z in x) XAMLObject.{1}.InsertNoUpdate(c++, z); {0}Inserted(idx, x); }}, (idx, x) => {{ foreach (var z in x) XAMLObject.{1}.RemoveNoUpdate(z); {0}RemovedAt(idx, x); }}, (x, nidx) => {{ XAMLObject.{1}.MoveNoUpdate(x, nidx); {0}Moved(x, nidx); }}, (ox, nx) => {{ XAMLObject.{1}.ReplaceNoUpdate(nx, ox); {0}Replaced(ox, nx); }});", de.HasClientType ? de.ClientName : de.DataName, de.XAMLName));
					if (de.DataType.TypeMode == DataTypeMode.Dictionary && de.HasXAMLType)
						code.AppendLine(string.Format("\t\t\t{0}.SetEvents((xk, xv) => {{ XAMLObject.{1}.AddOrUpdateNoUpdate(xk, xv, (k,v) => xv);  {0}Added(xk, xv); }}, (xk, xv) => {{ {2} result; XAMLObject.{1}.TryRemoveNoUpdate(xk, out result); {0}Removed(xk, xv); }}, (x) => {{ XAMLObject.{1}.ClearNoUpdate(); {0}Cleared(x); }}, (xk, ox, nx) => {{ XAMLObject.{1}.AddOrUpdateNoUpdate(xk, nx, (k,v) => nx); {0}Updated(xk, ox, nx); }});", de.HasClientType ? de.ClientName : de.DataName, de.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(de.DataType.DictionaryValueGenericType))));
				}
			if (o.CMDEnabled && o.HasXAMLType && IsServer)
				foreach (DataElement de in o.Elements)
				{
					if (de.DataType.TypeMode == DataTypeMode.Collection && de.HasXAMLType)
						code.AppendLine(string.Format("\t\t\t{0}.SetEvents((x) => {{ {0}Added(x); }}, (x) => {{ {0}Removed(x); }}, (x) => {{ {0}Cleared(x); }}, (idx, x) => {{ {0}Inserted(idx, x); }}, (idx, x) => {{ {0}RemovedAt(idx, x); }}, (x, nidx) => {{ {0}Moved(x, nidx); }}, (ox, nx) => {{ {0}Replaced(ox, nx); }});", de.HasClientType ? de.ClientName : de.DataName, de.XAMLName));
					if (de.DataType.TypeMode == DataTypeMode.Dictionary && de.HasXAMLType)
						code.AppendLine(string.Format("\t\t\t{0}.SetEvents((xk, xv) => {{ {0}Added(xk, xv); }}, (xk, xv) => {{ {0}Removed(xk, xv); }}, (x) => {{ {0}Cleared(x); }}, (xk, ox, nx) => {{ {0}Updated(xk, ox, nx); }});", de.HasClientType ? de.ClientName : de.DataName));
				}
			if (o.DREEnabled) code.AppendLine("\t\t\tEnableBatching = true;");
			code.AppendLine("\t\t}");
			if (o.DREEnabled)
			{
				code.AppendLine("\t\t//Data Revision Exchange Support");
				code.AppendLine(string.Format("\t\tprotected override {0}void BatchUpdates()", o.DataRevisionServiceNames.Any(a => a.IsAwaitable) ? "async " : ""));
				code.AppendLine("\t\t{");
				if (o.Elements.Any(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch))
				{
					code.AppendLine("\t\t\tvar delta = new List<CMDItemBase>(GetDeltaValues());");
					foreach (var drs in o.DataRevisionServiceNames.Where(d => d.IsServer == IsServer))
					{
						if (o.Elements.Any(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch) && !IsServer && Globals.CurrentProjectID == drs.ProjectID)
							code.AppendLine(string.Format("\t\t\t{4}{0}Proxy.Current.BatchUpdate{1}DRE{3}({2},", drs.Path, o.HasClientType ? o.ClientType.Name : o.Name, "_DREID", drs.IsAwaitable ? "Async" : "", drs.IsAwaitable ? "await " : ""));
						else if (IsServer)
							code.AppendLine(string.Format("\t\t\t{4}{0}Base.CallbackBatchUpdate{1}DRE{3}(Guid.Empty, {2},", drs.Path, o.Name, "_DREID", drs.IsAwaitable ? "Async" : "", drs.IsAwaitable ? "await " : ""));
						else continue;
						foreach (var t in o.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && !a.DREPrimaryKey && !(a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
							code.AppendLine(string.Format("\t\t\t\tdelta.FirstOrDefault(a => a.Key == {0}Property.ID) != null ? delta.First(a => a.Key == {0}Property.ID) as CMDItemValue<{1}> : null,", t.HasClientType ? t.ClientName : t.DataName, DataTypeGenerator.GenerateType(t.DataType)));
						foreach (var t in o.Elements.Where(a => a.DREEnabled && a.DREUpdateMode == DataUpdateMode.Batch && !a.DREPrimaryKey && (a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
							code.AppendLine(string.Format("\t\t\t\t{0}.GetDelta(),", t.HasClientType ? t.ClientName : t.DataName));
						code.Remove(code.Length - 3, 3);
						code.AppendLine(");");
					}
				}
				code.AppendLine("\t\t}");
			}
			code.AppendLine();
			return code.ToString();
		}

		public static string GenerateXAMLCodeRT8(Data o)
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
			if (!o.CMDEnabled) code.AppendLine(string.Format("\t\tprivate {0} BaseDataObject;", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine(string.Format("\t\tpublic {0} DataObject {{ get {{ if (BaseDataObject == null) BaseDataObject = new {0}(); return BaseDataObject as {0}; }} {1}}}", o.HasClientType ? o.ClientType.Name : o.Name, !o.CMDEnabled ? "set { BaseDataObject = value; } " : ""));
			code.AppendLine();

			code.AppendLine("\t\t//Properties");
			foreach (DataElement de in o.Elements.Where(a => !a.IsHidden && a.IsDataMember))
				code.AppendLine(GenerateElementXAMLCodeRT8(de));

			code.AppendLine(string.Format("\t\tpublic static implicit operator {0}({1} Data)", o.XAMLType.Name, o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			if (o.CMDEnabled)
			{
				code.AppendLine("\t\t\tif (Data == null) return null;");
				code.AppendLine(string.Format("\t\t\t{0} v = null;", o.XAMLType.Name));
				code.AppendLine("\t\t\tif (Window.Current.Dispatcher.HasThreadAccess) v = ConvertToXAMLObject(Data);");
				code.AppendLine("\t\t\telse Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { v = ConvertToXAMLObject(Data); }).GetResults();");
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
			foreach (DataElement de in o.Elements)
				if (de.DataType.TypeMode == DataTypeMode.Collection || de.DataType.TypeMode == DataTypeMode.Dictionary || de.DataType.TypeMode == DataTypeMode.Queue || de.DataType.TypeMode == DataTypeMode.Stack)
					code.AppendLine(string.Format("\t\t\t{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(de.DataType, o.CMDEnabled)), de.XAMLName));
				else if (de.DataType.TypeMode == DataTypeMode.Array)
					code.AppendLine(string.Format("\t\t\t{1} = new {0}[0];", DataTypeGenerator.GenerateType(GetPreferredXAMLType(de.DataType.CollectionGenericType, o.CMDEnabled)), de.XAMLName));
			code.AppendLine(string.Format("\t\t\tBaseDataObject = new {0}(this);", o.HasClientType ? o.ClientType.Name : o.Name));
			if (o.CMDEnabled)
				foreach (DataElement de in o.Elements)
				{
					if (de.DataType.TypeMode == DataTypeMode.Collection && de.HasXAMLType)
						code.AppendLine(string.Format("\t\t\t{0}.SetEvents((x) => {{ foreach (var z in x) DataObject.{1}.AddNoUpdate(z); {0}Added(x); }}, (x) => {{ foreach (var z in x) DataObject.{1}.RemoveNoUpdate(z); {0}Removed(x); }}, (x) => {{ DataObject.{1}.ClearNoUpdate(); {0}Cleared(x); }}, (idx, x) => {{ int c = idx; foreach (var z in x) DataObject.{1}.InsertNoUpdate(c++, z); {0}Inserted(idx, x); }}, (idx, x) => {{ foreach (var z in x) DataObject.{1}.RemoveNoUpdate(z); {0}RemovedAt(idx, x); }}, (x, nidx) => {{ DataObject.{1}.MoveNoUpdate(x, nidx); {0}Moved(x, nidx); }}, (ox, nx) => {{ DataObject.{1}.ReplaceNoUpdate(ox, nx); {0}Replaced(ox, nx); }});", de.XAMLName, de.HasClientType ? de.ClientName : de.DataName));
					if (de.DataType.TypeMode == DataTypeMode.Dictionary && de.HasXAMLType)
						code.AppendLine(string.Format("\t\t\t{0}.SetEvents((xk, xv) => {{ DataObject.{1}.AddOrUpdateNoUpdate(xk, xv, (k,v) => xv);  {0}Added(xk, xv); }}, (xk, xv) => {{ {2} result; DataObject.{1}.TryRemoveNoUpdate(xk, out result); {0}Removed(xk, xv); }}, (x) => {{ DataObject.{1}.ClearNoUpdate(); {0}Cleared(x); }}, (xk, ox, nx) => {{ DataObject.{1}.AddOrUpdateNoUpdate(xk, nx, (k,v) => nx); {0}Updated(xk, ox, nx); }});", de.XAMLName, de.HasClientType ? de.ClientName : de.DataName, DataTypeGenerator.GenerateType(de.DataType.DictionaryValueGenericType)));
				}
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}({1} Data){2}", o.XAMLType.Name, o.HasClientType ? o.ClientType.Name : o.Name, o.CMDEnabled ? " : base(Data)" : ""));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tIsExternalUpdate = true;");
			if (!o.CMDEnabled) code.AppendLine("\t\t\tBaseDataObject = Data;");
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementXAMLConstructorCode45(de, o));
			code.AppendLine("\t\t\tIsExternalUpdate = false;");
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

		private static string GenerateElementServerCode45(DataElement o, ref int ProtoCount)
		{
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.Append("\t\t");
			if (o.IsDataMember)
			{
				if (o.IsDataMember && o.ProtocolBufferEnabled && o.Owner.EnableProtocolBuffers) code.AppendFormat("[ProtoBuf.ProtoMember({0}{1}{2}{3}{4}{5}{6})] ", ++ProtoCount, o.ProtoDataFormat != ProtoBufDataFormat.Default ? string.Format(", DataFormat = ProtoBuf.DataFormat.{0}", System.Enum.GetName(typeof(ProtoBufDataFormat), o.ProtoDataFormat)) : "", o.IsRequired ? ", IsRequired = true" : "", o.ProtoIsPacked ? ", IsPacked = true" : "", o.ProtoOverwriteList ? ", OverwriteList = true" : "", o.ProtoAsReference ? ", AsReference = true" : "", o.ProtoDynamicType ? ", DynamicType = true" : "");
				else code.AppendFormat("[DataMember({0}{1}{2}{3})] ", o.IsRequired ? "IsRequired = true" : "IsRequired = false", o.EmitDefaultValue ? ", EmitDefaultValue = false" : "", o.ProtocolBufferEnabled ? string.Format(", Order = {0}{1}", ProtoCount, o.HasContractName ? ", " : "") : o.Order >= 0 ? string.Format(", Order = {0}{1}", o.Order, o.HasContractName ? ", " : "") : "", o.HasContractName ? string.Format(", Name = \"{0}\"", o.ContractName) : ""); 
			}
			else
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.XML) code.Append("[XmlIgnore] ");
			code.AppendLine(string.Format("public {0} {1} {{ get; {2}set; }}", DataTypeGenerator.GenerateType(o.DataType), o.DataName, o.IsReadOnly ? "protected " : ""));
			return code.ToString();
		}

		private static string GenerateElementDCMServerCode45(DataElement o, ref int ProtoCount, bool IsOverride = false)
		{
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.Append("\t\t");
			if (o.ProtocolBufferEnabled && o.Owner.EnableProtocolBuffers) code.AppendFormat("[ProtoBuf.ProtoMember({0}{1}{2}{3}{4}{5}{6})] ", ++ProtoCount, o.ProtoDataFormat != ProtoBufDataFormat.Default ? string.Format(", DataFormat = ProtoBuf.DataFormat.{0}", System.Enum.GetName(typeof(ProtoBufDataFormat), o.ProtoDataFormat)) : "", o.IsRequired ? ", IsRequired = true" : "", o.ProtoIsPacked ? ", IsPacked = true" : "", o.ProtoOverwriteList ? ", OverwriteList = true" : "", o.ProtoAsReference ? ", AsReference = true" : "", o.ProtoDynamicType ? ", DynamicType = true" : "");
			else code.AppendFormat("[DataMember({0}{1}{2}{3})] ", o.IsRequired ? "IsRequired = true" : "IsRequired = false", o.EmitDefaultValue ? ", EmitDefaultValue = false" : "", o.ProtocolBufferEnabled ? string.Format(", Order = {0}{1}", ProtoCount, o.HasContractName ? ", " : "") : o.Order >= 0 ? string.Format(", Order = {0}{1}", o.Order, o.HasContractName ? ", " : "") : "", o.HasContractName ? string.Format(", Name = \"{0}\"", o.ContractName) : "");
			code.AppendLine(string.Format("public {4}{0} {1} {{ get {{ return GetValue({1}Property); }} {2}set {{ SetValue({1}Property, value); {5}{3}}} }}", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.IsReadOnly ? "protected " : "", o.GenerateWinFormsSupport ? "NotifyPropertyChanged(); " : "", IsOverride ? "override " : "", o.DREPrimaryKey ? "SetDREID(value); " : ""));
			if (o.DataType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\tpublic static readonly DREProperty<DeltaList<{3}>> {1}Property = DREProperty<DeltaList<{3}>>.RegisterList<{3}>(\"{1}\", typeof({2}), (s, o, n) => {{ var t = s as {2}; if (t == null) return;", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.DREEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, DataTypeGenerator.GenerateType(o.DataType.CollectionGenericType)));
				code.AppendLine(string.Format("\t\t\tn.SetEvents((x) => t.{0}Added(x), (x) => t.{0}Removed(x), (x) => t.{0}Cleared(x), (idx, x) => t.{0}Inserted(idx, x), (idx, x) => t.{0}RemovedAt(idx, x), (x, nidx) => t.{0}Moved(x, nidx), (ox, nx) => t.{0}Replaced(ox, nx));", o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine("\t\t\to.ClearEvents();");
				code.AppendLine("\t\t});");
				code.AppendLine(string.Format("\t\tpartial void {0}Added(IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Removed(IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Cleared(IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Inserted(int Index, IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}RemovedAt(int Index, IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Moved({1} Value, int Index);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Replaced({1} OldValue, {1} NewValue);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.CollectionGenericType))));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Stack)
			{
			}
			else if (o.DataType.TypeMode == DataTypeMode.Queue)
			{
			}
			else if (o.DataType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\tpublic static readonly DREProperty<DeltaDictionary<{3}, {4}>> {1}Property = DREProperty<DeltaDictionary<{3}, {4}>>.RegisterDictionary<{3}, {4}>(\"{1}\", typeof({2}), (s, o, n) => {{ var t = s as {2}; if (t == null) return;", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.DREEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, DataTypeGenerator.GenerateType(o.DataType.DictionaryKeyGenericType), DataTypeGenerator.GenerateType(o.DataType.DictionaryValueGenericType)));
				code.AppendLine(string.Format("\t\t\tn.SetEvents((xk, xv) => t.{0}Added(xk, xv), (xk, xv) => t.{0}Removed(xk, xv), (x) => t.{0}Cleared(x), (xk, ox, nx) => t.{0}Updated(xk, ox, nx));", o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine("\t\t\to.ClearEvents();");
				code.AppendLine("\t\t});");
				code.AppendLine(string.Format("\t\tpartial void {0}Added({1} Key, {2} Value);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.DictionaryValueGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Removed({1} Key, {2} Value);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.DictionaryValueGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Cleared(IEnumerable<KeyValuePair<{1}, {2}>> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.DictionaryValueGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Updated({1} Key, {2} OldValue, {2} NewValue);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.DictionaryValueGenericType))));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\tpublic static readonly DREProperty<{2}> {0}Property = DREProperty<{2}>.Register(\"{0}\", typeof({1}), default({2}), {4}, {3});", o.DataName, o.Owner.Name, DataTypeGenerator.GenerateType(o.DataType), o.DREEnabled && !o.DREPrimaryKey && o.DREUpdateMode == DataUpdateMode.Immediate ? string.Format("(s, o, n) => {{ var t = s as {1}; if (t == null) return; {0}}}", GenerateImmediateDREValueCallbacks(o.Owner.DataRevisionServiceNames, o.Owner.Name, o.DataName, true), o.Owner.Name) : "null", o.PropertyChangedCallback ? string.Format("(s, o, n) => {{ var t = s as {1}; if (t == null) return; t.{0}PropertyChanged(o, n); }}", o.DataName, o.Owner.Name) : "null"));
				if (o.PropertyChangedCallback) code.AppendLine(string.Format("\t\tpartial void {0}PropertyChanged({1} OldValue, {1} NewValue);", o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled))));
				if (o.PropertyUpdatedCallback) code.AppendLine(string.Format("\t\tpartial void {0}PropertyUpdated({1} OldValue, {1} NewValue);", o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled))));
				if (o.PropertyValidationCallback) code.AppendLine(string.Format("\t\tpartial void {0}ValidateValue({1} Value, ref bool Valid);", o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled))));
			}																																																																																																																																												   
			else																																																																																																																																											   
			{
				code.AppendLine(string.Format("\t\tpublic static readonly DREProperty<{2}> {0}Property = DREProperty<{2}>.Register(\"{0}\", typeof({1}), default({2}), {4}, {3});", o.DataName, o.Owner.Name, DataTypeGenerator.GenerateType(o.DataType), o.DREEnabled && !o.DREPrimaryKey && o.DREUpdateMode == DataUpdateMode.Immediate ? string.Format("(s, o, n) => {{ var t = s as {1}; if (t == null) return; {0}}}", GenerateImmediateDREValueCallbacks(o.Owner.DataRevisionServiceNames, o.Owner.Name, o.DataName, true), o.Owner.Name) : "null", o.PropertyChangedCallback ? string.Format("(s, o, n) => {{ var t = s as {1}; if (t == null) return; t.{0}PropertyChanged(o, n); }}", o.DataName, o.Owner.Name) : "null"));
				if (o.PropertyChangedCallback) code.AppendLine(string.Format("\t\tpartial void {0}PropertyChanged({1} OldValue, {1} NewValue);", o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled))));
				if (o.PropertyUpdatedCallback) code.AppendLine(string.Format("\t\tpartial void {0}PropertyUpdated({1} OldValue, {1} NewValue);", o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled))));
				if (o.PropertyValidationCallback) code.AppendLine(string.Format("\t\tpartial void {0}ValidateValue({1} Value, ref bool Valid);", o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled))));
			}
			return code.ToString();
		}

		private static string GenerateElementProxyCode45(DataElement o, ref int ProtoCount)
		{
			var code = new StringBuilder();
			code.AppendLine(string.Format("\t\tprivate {0} {1}Field;", DataTypeGenerator.GenerateType(o.DataType), o.HasClientType ? o.ClientName : o.DataName));
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.Append("\t\t");
			if (o.ProtocolBufferEnabled && o.Owner.EnableProtocolBuffers) code.AppendFormat("[ProtoBuf.ProtoMember({0}{1}{2}{3}{4}{5}{6})] ", ++ProtoCount, o.ProtoDataFormat != ProtoBufDataFormat.Default ? string.Format(", DataFormat = ProtoBuf.DataFormat.{0}", System.Enum.GetName(typeof(ProtoBufDataFormat), o.ProtoDataFormat)) : "", o.IsRequired ? ", IsRequired = true" : "", o.ProtoIsPacked ? ", IsPacked = true" : "", o.ProtoOverwriteList ? ", OverwriteList = true" : "", o.ProtoAsReference ? ", AsReference = true" : "", o.ProtoDynamicType ? ", DynamicType = true" : "");
			else code.AppendFormat("[DataMember({0}{1}{2}{3})] ", o.IsRequired ? "IsRequired = true" : "IsRequired = false", o.EmitDefaultValue ? ", EmitDefaultValue = false" : "", o.ProtocolBufferEnabled ? string.Format(", Order = {0}{1}", ProtoCount, o.HasContractName ? ", " : "") : o.Order >= 0 ? string.Format(", Order = {0}{1}", o.Order, o.HasContractName ? ", " : "") : "", o.HasContractName ? string.Format(", Name = \"{0}\"", o.ContractName) : "");
			code.AppendLine(string.Format("public {0} {1} {{ get {{ return {1}Field; }} {2}set {{ {1}Field = value; {3}}} }}", DataTypeGenerator.GenerateType(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.IsReadOnly ? "protected " : "", o.GenerateWinFormsSupport ? "NotifyPropertyChanged(); " : ""));
			return code.ToString();
		}

		private static string GenerateElementDCMProxyCode45(DataElement o, ref int ProtoCount, bool IsOverride = false)
		{
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.Append("\t\t");
			if (o.ProtocolBufferEnabled && o.Owner.EnableProtocolBuffers) code.AppendFormat("[ProtoBuf.ProtoMember({0}{1}{2}{3}{4}{5}{6})] ", ++ProtoCount, o.ProtoDataFormat != ProtoBufDataFormat.Default ? string.Format(", DataFormat = ProtoBuf.DataFormat.{0}", System.Enum.GetName(typeof(ProtoBufDataFormat), o.ProtoDataFormat)) : "", o.IsRequired ? ", IsRequired = true" : "", o.ProtoIsPacked ? ", IsPacked = true" : "", o.ProtoOverwriteList ? ", OverwriteList = true" : "", o.ProtoAsReference ? ", AsReference = true" : "", o.ProtoDynamicType ? ", DynamicType = true" : "");
			else code.AppendFormat("[DataMember({0}{1}{2}{3})] ", o.IsRequired ? "IsRequired = true" : "IsRequired = false", o.EmitDefaultValue ? ", EmitDefaultValue = false" : "", o.ProtocolBufferEnabled ? string.Format(", Order = {0}{1}", ProtoCount, o.HasContractName ? ", " : "") : o.Order >= 0 ? string.Format(", Order = {0}{1}", o.Order, o.HasContractName ? ", " : "") : "", o.HasContractName ? string.Format(", Name = \"{0}\"", o.ContractName) : "");
			code.AppendLine(string.Format("public {4}{0} {1} {{ get {{ return GetValue({1}Property); }} {2}set {{ SetValue({1}Property, value); {5}{3}}} }}", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.IsReadOnly ? "protected " : "", o.GenerateWinFormsSupport ? "NotifyPropertyChanged(); " : "", IsOverride ? "override " : "", o.DREPrimaryKey ? "SetDREID(value); " : ""));
			if (o.DataType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\tpublic static readonly DREProperty<DeltaList<{3}>> {1}Property = DREProperty<DeltaList<{3}>>.RegisterList<{3}>(\"{1}\", typeof({2}), (s, o, n) => {{ var t = s as {2}; if (t == null) return;", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.DREEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, DataTypeGenerator.GenerateType(o.DataType.CollectionGenericType)));
				if (!o.Owner.HasXAMLType) code.AppendLine(string.Format("\t\t\tn.SetEvents((x) => {{ t.{0}Added(x); }}, (x) => {{ t.{0}Removed(x); }}, (x) => {{ t.{0}Cleared(x); }}, (idx, x) => {{ t.{0}Inserted(idx, x); }}, (idx, x) => {{ t.{0}RemovedAt(idx, x); }}, (x, nidx) => {{ t.{0}Moved(x, nidx); }}, (ox, nx) => {{ t.{0}Replaced(ox, nx); }});", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("\t\t\tn.SetEvents((x) => {{ foreach (var z in x) t.XAMLObject.{1}.AddNoUpdate(z); t.{0}Added(x); }}, (x) => {{ foreach (var z in x) t.XAMLObject.{1}.RemoveNoUpdate(z); t.{0}Removed(x); }}, (x) => {{ t.XAMLObject.{1}.ClearNoUpdate(); t.{0}Cleared(x); }}, (idx, x) => {{ int c = idx; foreach (var z in x) t.XAMLObject.{1}.InsertNoUpdate(c++, z); t.{0}Inserted(idx, x); }}, (idx, x) => {{ foreach (var z in x) t.XAMLObject.{1}.RemoveNoUpdate(z); t.{0}RemovedAt(idx, x); }}, (x, nidx) => {{ t.XAMLObject.{1}.MoveNoUpdate(x, nidx); t.{0}Moved(x, nidx); }}, (ox, nx) => {{ t.XAMLObject.{1}.ReplaceNoUpdate(nx, ox); t.{0}Replaced(ox, nx); }});", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine("\t\t\tif (o != null) o.ClearEvents();");
				code.AppendLine("\t\t});");
				code.AppendLine(string.Format("\t\tpartial void {0}Added(IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Removed(IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Cleared(IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Inserted(int Index, IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}RemovedAt(int Index, IEnumerable<{1}> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Moved({1} Value, int Index);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.CollectionGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Replaced({1} OldValue, {1} NewValue);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.CollectionGenericType))));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Stack)
			{
			}
			else if (o.DataType.TypeMode == DataTypeMode.Queue)
			{
			}
			else if (o.DataType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\tpublic static readonly DREProperty<DeltaDictionary<{3}, {4}>> {1}Property = DREProperty<DeltaDictionary<{3}, {4}>>.RegisterDictionary<{3}, {4}>(\"{1}\", typeof({2}), (s, o, n) => {{ var t = s as {2}; if (t == null) return;", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.DREEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, DataTypeGenerator.GenerateType(o.DataType.DictionaryKeyGenericType), DataTypeGenerator.GenerateType(o.DataType.DictionaryValueGenericType)));
				if (!o.Owner.HasXAMLType) code.AppendLine(string.Format("\t\t\tn.SetEvents((xk, xv) => {{ t.{0}Added(xk, xv); }}, (xk, xv) => {{ t.{0}Removed(xk, xv); }}, (x) => {{ t.{0}Cleared(x); }}, (xk, ox, nx) => {{ t.{0}Updated(xk, ox, nx); }});", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.DictionaryValueGenericType))));
				else code.AppendLine(string.Format("\t\t\tn.SetEvents((xk, xv) => {{ t.XAMLObject.{1}.AddOrUpdateNoUpdate(xk, xv, (k,v) => xv); t.{0}Added(xk, xv); }}, (xk, xv) => {{ {2} result; t.XAMLObject.{1}.TryRemoveNoUpdate(xk, out result); t.{0}Removed(xk, xv); }}, (x) => {{ t.XAMLObject.{1}.ClearNoUpdate(); t.{0}Cleared(x); }}, (xk, ox, nx) => {{ t.XAMLObject.{1}.AddOrUpdateNoUpdate(xk, nx, (k,v) => nx); t.{0}Updated(xk, ox, nx); }});", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.DictionaryValueGenericType))));
				code.AppendLine("\t\t\tif (o != null) o.ClearEvents();");
				code.AppendLine("\t\t});");
				code.AppendLine(string.Format("\t\tpartial void {0}Added({1} Key, {2} Value);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.DictionaryValueGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Removed({1} Key, {2} Value);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.DictionaryValueGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Cleared(IEnumerable<KeyValuePair<{1}, {2}>> Values);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.DictionaryValueGenericType))));
				code.AppendLine(string.Format("\t\tpartial void {0}Updated({1} Key, {2} OldValue, {2} NewValue);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType.DictionaryValueGenericType))));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\tpublic static readonly DREProperty<{3}> {1}Property = DREProperty<{3}>.Register(\"{1}\", typeof({2}), default({3}), {8}, (s, o, n) => {{ var t = s as {2}; var nv = n as {0}; if (t == null || nv == null) return; var z = new {5}[nv.Length]; for (int i = 0; i < nv.Length; i++) z[i] = nv[i]; if (t.XAMLObject != null) t.XAMLObject.{4} = z; {6}{10}}}, {9}, {7});", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.DREEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, DataTypeGenerator.GenerateType(o.DataType), o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.CollectionGenericType, o.DREEnabled)), !o.DREPrimaryKey ? GenerateImmediateDREValueCallbacks(o.Owner.DataRevisionServiceNames, o.Owner.Name, o.DataName, false) : "", o.DREUpdateMode != DataUpdateMode.None && o.DREEnabled && !o.IsReadOnly && !o.DataType.IsCollectionType ? string.Format("{0}.{1}Property", o.Owner.XAMLType.Name, o.XAMLName) : "null", o.PropertyChangedCallback ? string.Format("(s, o, n) => {{ var t = s as {1}; if (t == null) return; t.{0}PropertyChanged(o, n); }}", o.HasClientType ? o.ClientName : o.DataName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name) : "null", o.PropertyValidationCallback ? string.Format("(s, v) => {{ var t = s as {1}; if (t == null) return false; bool valid = false; t.{0}ValidateValue(v, ref valid); return valid; }}", o.HasClientType ? o.ClientName : o.DataName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name) : "null", o.PropertyUpdatedCallback ? string.Format(" t.{0}PropertyUpdated(o, n);", o.HasClientType ? o.ClientName : o.DataName) : ""));
				if (o.PropertyChangedCallback) code.AppendLine(string.Format("\t\tpartial void {0}PropertyChanged({1} OldValue, {1} NewValue);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled))));
				if (o.PropertyUpdatedCallback) code.AppendLine(string.Format("\t\tpartial void {0}PropertyUpdated({1} OldValue, {1} NewValue);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled))));
				if (o.PropertyValidationCallback) code.AppendLine(string.Format("\t\tpartial void {0}ValidateValue({1} Value, ref bool Valid);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled))));
			}
			else
			{
				code.AppendLine(string.Format("\t\tpublic static readonly DREProperty<{2}> {0}Property = DREProperty<{2}>.Register(\"{0}\", typeof({1}), default({2}), {5}, {3}, {6}, {4});", o.HasClientType ? o.ClientName : o.DataName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, DataTypeGenerator.GenerateType(o.DataType), o.DREEnabled && !o.DREPrimaryKey && o.DREUpdateMode == DataUpdateMode.Immediate ? string.Format("(s, o, n) => {{ var t = s as {1}; if (t == null) return; {0}{2}}}", GenerateImmediateDREValueCallbacks(o.Owner.DataRevisionServiceNames, o.Owner.Name, o.DataName, false), o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name, o.PropertyUpdatedCallback ? string.Format(" t.{0}PropertyUpdated(o, n);", o.HasClientType ? o.ClientName : o.DataName) : "") : "null", o.DREUpdateMode != DataUpdateMode.None && o.DREEnabled && !o.IsReadOnly && !o.DataType.IsCollectionType && o.Owner.XAMLType != null ? string.Format("{0}.{1}Property", o.Owner.XAMLType.Name, o.XAMLName) : "null", o.PropertyChangedCallback ? string.Format("(s, o, n) => {{ var t = s as {1}; if (t == null) return; t.{0}PropertyChanged(o, n); }}", o.HasClientType ? o.ClientName : o.DataName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name) : "null", o.PropertyValidationCallback ? string.Format("(s, v) => {{ var t = s as {1}; if (t == null) return false; bool valid = false; t.{0}ValidateValue(v, ref valid); return valid; }}", o.HasClientType ? o.ClientName : o.DataName, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name) : "null"));
				if (o.PropertyChangedCallback) code.AppendLine(string.Format("\t\tpartial void {0}PropertyChanged({1} OldValue, {1} NewValue);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled))));
				if (o.PropertyUpdatedCallback) code.AppendLine(string.Format("\t\tpartial void {0}PropertyUpdated({1} OldValue, {1} NewValue);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled))));
				if (o.PropertyValidationCallback) code.AppendLine(string.Format("\t\tpartial void {0}ValidateValue({1} Value, ref bool Valid);", o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled))));
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
						if (value.TypeMode == DataTypeMode.Stack) return t.HasXAMLType ? new DataType("DependencyStack", value.TypeMode) { Name = "DependencyStack", CollectionGenericType = t.XAMLType } : new DataType("DependencyStack", value.TypeMode) { Name = "DependencyStack", CollectionGenericType = t.HasClientType ? t.ClientType : t };
						if (value.TypeMode == DataTypeMode.Queue) return t.HasXAMLType ? new DataType("DependencyQueue", value.TypeMode) { Name = "DependencyQueue", CollectionGenericType = t.XAMLType } : new DataType("DependencyQueue", value.TypeMode) { Name = "DependencyQueue", CollectionGenericType = t.HasClientType ? t.ClientType : t };
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

		private static string GenerateElementXAMLCodeRT8(DataElement o)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			if (!o.Owner.HasXAMLType) return "";
			if (!o.HasXAMLType) return "";

			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			if (o.IsAttached)
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
				code.AppendLine(string.Format("\t\tpublic static {4} Get{1}(DependencyObject obj) {{ return {0}obj.GetValue{2}{3}({1}Property){5}; }}", o.Owner.CMDEnabled ? "" : string.Format("({0})", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType))), o.XAMLName, o.Owner.CMDEnabled ? "Threaded" : "", o.Owner.CMDEnabled ? string.Format("<{0}>", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, true))) : "", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.Owner.CMDEnabled)), o.Owner.CMDEnabled ? ".Result" : ""));
				code.AppendLine(string.Format("\t\tpublic static void Set{1}(DependencyObject obj, {0} value) {{ obj.SetValue{2}({1}Property, value); }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.Owner.CMDEnabled)), o.Owner.CMDEnabled ? "Threaded" : "", o.XAMLName));
				code.Append(GenerateElementXAMLDPCode45(o));
			}
			else
			{
				code.AppendLine(string.Format("\t\tpublic {5} {1} {{ get {{ return {0}GetValue{3}{4}({1}Property){6}; }} {2}set {{ SetValue{3}({1}Property, value); }} }}", o.Owner.CMDEnabled ? "" : string.Format("({0})", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType))), o.XAMLName, o.IsReadOnly ? "protected " : "", o.Owner.CMDEnabled ? "Threaded" : "", o.Owner.CMDEnabled ? string.Format("<{0}>", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, true))) : "", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.Owner.CMDEnabled)), o.Owner.CMDEnabled ? ".Result" : ""));
				code.Append(GenerateElementXAMLDPCode45(o));
			}
			return code.ToString();
		}

		private static string GenerateElementXAMLDPCode45(DataElement o)
		{
			var code = new StringBuilder();

			if (o.Owner.CMDEnabled)
			{
				if (o.DataType.TypeMode == DataTypeMode.Collection)
				{
					code.AppendLine(string.Format("\t\tprivate static readonly PropertyMetadata {0}PropertyMetadata = new PropertyMetadata(default(DependencyList<{2}>), (o, e) => {{ var t = o as {1}; var nl = e.NewValue as DependencyList<{2}>; var ol = e.OldValue as DependencyList<{2}>; if (t == null || nl == null) return; ", o.XAMLName, o.Owner.XAMLType.Name, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.CollectionGenericType))));
					code.AppendLine(string.Format("\t\t\tnl.SetEvents((x) => {{ foreach (var z in x) t.DataObject.{1}.AddNoUpdate(z); t.{0}Added(x); }}, (x) => {{ foreach (var z in x) t.DataObject.{1}.RemoveNoUpdate(z); t.{0}Removed(x); }}, (x) => {{ t.DataObject.{1}.ClearNoUpdate(); t.{0}Cleared(x); }}, (idx, x) => {{ int c = idx; foreach (var z in x) t.DataObject.{1}.InsertNoUpdate(c++, z); t.{0}Inserted(idx, x); }}, (idx, x) => {{ foreach (var z in x) t.DataObject.{1}.RemoveNoUpdate(z); t.{0}RemovedAt(idx, x); }}, (x, nidx) => {{ t.DataObject.{1}.MoveNoUpdate(x, nidx); t.{0}Moved(x, nidx); }}, (ox, nx) => {{ t.DataObject.{1}.ReplaceNoUpdate(ox, nx); t.{0}Replaced(ox, nx); }});", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
					code.AppendLine("\t\t\tif (ol != null) ol.ClearEvents();");
					code.AppendLine("\t\t});");
					code.AppendLine(string.Format("\t\tpartial void {0}Added(IEnumerable<{1}> Values);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.CollectionGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}Removed(IEnumerable<{1}> Values);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.CollectionGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}Cleared(IEnumerable<{1}> Values);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.CollectionGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}Inserted(int Index, IEnumerable<{1}> Values);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.CollectionGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}RemovedAt(int Index, IEnumerable<{1}> Values);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.CollectionGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}Moved({1} Value, int Index);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.CollectionGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}Replaced({1} OldValue, {1} NewValue);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.CollectionGenericType))));
				}
				else if (o.DataType.TypeMode == DataTypeMode.Stack)
				{
				}
				else if (o.DataType.TypeMode == DataTypeMode.Queue)
				{
				}
				else if (o.DataType.TypeMode == DataTypeMode.Dictionary)
				{
					code.AppendLine(string.Format("\t\tprivate static readonly PropertyMetadata {0}PropertyMetadata = new PropertyMetadata(default(DependencyDictionary<{2}, {3}>), (o, e) => {{ var t = o as {1}; var nl = e.NewValue as DependencyDictionary<{2}, {3}>; var ol = e.OldValue as DependencyDictionary<{2}, {3}>; if (t == null || nl == null) return; ", o.XAMLName, o.Owner.XAMLType.Name, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.DictionaryValueGenericType))));
					code.AppendLine(string.Format("\t\t\tnl.SetEvents((xk, xv) => {{ t.DataObject.{1}.AddOrUpdateNoUpdate(xk, xv, (k,v) => xv);  t.{0}Added(xk, xv); }}, (xk, xv) => {{ {2} result; t.DataObject.{1}.TryRemoveNoUpdate(xk, out result); t.{0}Removed(xk, xv); }}, (x) => {{ t.DataObject.{1}.ClearNoUpdate(); t.{0}Cleared(x); }}, (xk, ox, nx) => {{ t.DataObject.{1}.AddOrUpdateNoUpdate(xk, nx, (k,v) => nx); t.{0}Updated(xk, ox, nx); }});", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName, DataTypeGenerator.GenerateType(o.DataType.DictionaryValueGenericType)));
					code.AppendLine("\t\t\tif (ol != null) ol.ClearEvents();");
					code.AppendLine("\t\t});");
					code.AppendLine(string.Format("\t\tpartial void {0}Added({1} Key, {2} Value);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.DictionaryValueGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}Removed({1} Key, {2} Value);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.DictionaryValueGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}Cleared(IEnumerable<KeyValuePair<{1}, {2}>> Values);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.DictionaryValueGenericType))));
					code.AppendLine(string.Format("\t\tpartial void {0}Updated({1} Key, {2} OldValue, {2} NewValue);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.DictionaryKeyGenericType)), DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType.DictionaryValueGenericType))));
				}
				else if (o.DataType.TypeMode == DataTypeMode.Array)
				{
					code.AppendLine(string.Format("\t\tprivate static readonly PropertyMetadata {0}PropertyMetadata = new PropertyMetadata(default({1}), (o, e) => {{ var t = o as {3}; var nv = e.NewValue as {1}; if (t == null || nv == null) return; if (!t.IsExternalUpdate && t.DataObject != null) {{ var z = new {4}[nv.Length]; for (int i = 0; i < nv.Length; i++) z[i] = nv[i]; t.DataObject.UpdateValue({5}.{2}Property, z); }} t.{0}PropertyChanged(({1})e.NewValue, ({1})e.OldValue); }});", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.Owner.CMDEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.Owner.XAMLType.Name, o.DataType.CollectionGenericType, o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name));
					code.AppendLine(string.Format("\t\tpartial void {0}PropertyChanged({1} OldValue, {1} NewValue);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.Owner.CMDEnabled))));
				}
				else
				{
					code.AppendLine(string.Format("\t\tprivate static readonly PropertyMetadata {0}PropertyMetadata = new PropertyMetadata(default({1}), (o, e) => {{ var t = o as {3}; if (t == null) return; if (!t.IsExternalUpdate) t.DataObject.UpdateValue({5}.{2}Property, ({1})e.NewValue); t.{0}PropertyChanged(({1})e.NewValue, ({1})e.OldValue); }});", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.DREEnabled)), o.HasClientType ? o.ClientName : o.DataName, o.Owner.XAMLType.Name, DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.DREEnabled)), o.Owner.HasClientType ? o.Owner.ClientType.Name : o.Owner.Name));
					code.AppendLine(string.Format("\t\tpartial void {0}PropertyChanged({1} OldValue, {1} NewValue);", o.XAMLName, DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.Owner.CMDEnabled))));
				}
			}

			code.AppendLine(string.Format("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.Register{4}(\"{1}\", typeof({0}), typeof({2}){3});", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.Owner.CMDEnabled)), o.XAMLName, o.Owner.XAMLType.Name, o.DREEnabled ? string.Format(", {0}PropertyMetadata", o.XAMLName) : ", null", o.IsAttached ? "Attached" : ""));

			return code.ToString();
		}

		private static string GenerateElementProxyConstructorCode45(DataElement o, Data c)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			if (!o.HasXAMLType) return "";
			var code = new StringBuilder();

			if (o.DataType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\t\t{0}[] v{3} = new {1}[Data.{2}.Length];", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled)).Replace("[]", ""), DataTypeGenerator.GenerateType(o.DataType).Replace("[]", ""), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) for(int i = 0; i < Data.{0}.Length; i++) {{ v{1}[i] = Data.{0}[i]; }}", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Add(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Stack)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Push(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Queue)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Enqueue(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(o.DataType.Name == "System.Collections.Concurrent.ConcurrentDictionary" ? string.Format("\t\t\tif (Data.{1} != null) foreach(KeyValuePair<{0}> a in Data.{1}) {{ v{2}.TryAdd(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName) : string.Format("\t\t\tif (Data.{1} != null) foreach(KeyValuePair<{0}> a in Data.{1}) {{ v{2}.Add(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
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

			if (o.DataType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\t\t{0}[] v{3} = new {1}[XAMLObject.{2}.Length];", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled)).Replace("[]", ""), DataTypeGenerator.GenerateType(o.DataType).Replace("[]", ""), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (XAMLObject.{1} != null) for(int i = 0; i < XAMLObject.{0}.Length; i++) {{ v{1}[i] = XAMLObject.{0}[i]; }}", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (XAMLObject.{1} != null) foreach({0} a in XAMLObject.{1}) {{ v{2}.Add(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Stack)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (XAMLObject.{1} != null) foreach({0} a in XAMLObject.{1}) {{ v{2}.Push(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Queue)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (XAMLObject.{1} != null) foreach({0} a in XAMLObject.{1}) {{ v{2}.Enqueue(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType, o.Owner.CMDEnabled)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(o.DataType.Name == "System.Collections.Concurrent.ConcurrentDictionary" ? string.Format("\t\t\tif (XAMLObject.{1} != null) foreach(KeyValuePair<{0}> a in XAMLObject.{1}) {{ v{2}.TryAdd(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName) : string.Format("\t\t\tif (XAMLObject.{1} != null) foreach(KeyValuePair<{0}> a in XAMLObject.{1}) {{ v{2}.Add(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.HasClientType ? o.ClientName : o.DataName));
			}
			else
				code.AppendLine(string.Format("\t\t\t{1} = XAMLObject.{0};", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));

			return code.ToString();
		}

		private static string GenerateElementXAMLConstructorCode45(DataElement o, Data c)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			if (!o.HasXAMLType) return "";
			var code = new StringBuilder();

			if (o.DataType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\t\t{0}[] v{3} = new {1}[Data.{2}.Length];", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.Owner.CMDEnabled)).Replace("[]", ""), DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType)).Replace("[]", ""), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) for(int i = 0; i < Data.{0}.Length; i++) {{ v{1}[i] = Data.{0}[i]; }}", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.Owner.CMDEnabled)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Add(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Stack)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.Owner.CMDEnabled)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Push(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Queue)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.Owner.CMDEnabled)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Enqueue(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.Owner.CMDEnabled)), o.XAMLName));
				code.AppendLine(o.DataType.Name == "System.Collections.Concurrent.ConcurrentDictionary" ? string.Format("\t\t\tif (Data.{1} != null) foreach(KeyValuePair<{0}> a in Data.{1}) {{ v{2}.TryAdd(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName) : string.Format("\t\t\tif (Data.{1} != null) foreach(KeyValuePair<{0}> a in Data.{1}) {{ v{2}.Add(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
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

			if (o.DataType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\t\t{0}[] v{3} = new {1}[DataObject.{2}.Length];", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.Owner.CMDEnabled)).Replace("[]", ""), DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType)).Replace("[]", ""), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (DataObject.{1} != null) for(int i = 0; i < DataObject.{0}.Length; i++) {{ v{1}[i] = DataObject.{0}[i]; }}", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.Owner.CMDEnabled)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (DataObject.{1} != null) foreach({0} a in DataObject.{1}) {{ v{2}.Add(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Stack)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.Owner.CMDEnabled)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (DataObject.{1} != null) foreach({0} a in DataObject.{1}) {{ v{2}.Push(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Queue)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.Owner.CMDEnabled)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (DataObject.{1} != null) foreach({0} a in DataObject.{1}) {{ v{2}.Enqueue(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType, o.Owner.CMDEnabled)), o.XAMLName));
				code.AppendLine(o.DataType.Name == "System.Collections.Concurrent.ConcurrentDictionary" ? string.Format("\t\t\tif (DataObject.{1} != null) foreach(KeyValuePair<{0}> a in DataObject.{1}) {{ v{2}.TryAdd(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName) : string.Format("\t\t\tif (DataObject.{1} != null) foreach(KeyValuePair<{0}> a in DataObject.{1}) {{ v{2}.Add(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
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