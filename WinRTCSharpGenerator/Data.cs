using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFArchitect.Projects;
using WCFArchitect.Projects.Helpers;

namespace WCFArchitect.Generators.WinRT.CS
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
				o.Elements.Add(new DataElement(new DataType(PrimitiveTypes.GUID), "_ID", o) {IsAutoDataID = true, IsDataMember = true});
			}

			foreach (DataElement d in o.Elements)
			{
				if (RegExs.MatchCodeName.IsMatch(d.DataType.Name) == false && d.DataType.TypeMode != DataTypeMode.Array)
					AddMessage(new CompileMessage("GS3001", "The data element '" + d.DataType.Name + "' in the '" + o.Name + "' data object contains invalid characters in the Name.", CompileMessageSeverity.ERROR, o, d, d.GetType(), o.Parent.Owner.ID));

				if (d.HasClientType)
					if (RegExs.MatchCodeName.IsMatch(d.ClientType.Name) == false && d.DataType.TypeMode != DataTypeMode.Array)
						AddMessage(new CompileMessage("GS3002", "The data object '" + d.ClientType.Name + "' in the '" + o.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));

				if (d.HasXAMLType)
					if (RegExs.MatchCodeName.IsMatch(d.XAMLType.Name) == false && d.DataType.TypeMode != DataTypeMode.Array)
						AddMessage(new CompileMessage("GS3003", "The data object '" + d.XAMLType.Name + "' in the '" + o.Name + "' namespace contains invalid characters in the XAML Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.Owner.ID));
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

		public static string GenerateServerCode45(Data o)
		{
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			foreach (DataType dt in o.KnownTypes)
				code.AppendLine(string.Format("\t[KnownType(typeof({0}))]", dt));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t[DataContract({0}Name = \"{1}\", Namespace = \"{2}\")]", o.IsReference ? "IsReference = true, " : "", o.HasClientType ? o.ClientType.Name : o.Name, o.Parent.FullURI));
			code.AppendLine(string.Format("\t{0}", DataTypeGenerator.GenerateTypeDeclaration(o)));
			code.AppendLine("\t{");
	
			if (o.DataHasExtensionData)
			{
				code.AppendLine("\t\tpublic System.Runtime.Serialization.ExtensionDataObject ExtensionData { get; set; }");
				code.AppendLine();
			}

			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementServerCode45(de));
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
			code.AppendLine(string.Format("\t[DataContract({0}Name = \"{1}\", Namespace = \"{2}\")]", o.IsReference ? "IsReference = true, " : "", o.HasClientType ? o.ClientType.Name : o.Name, o.Parent.URI));
			code.AppendLine(string.Format("\t{0}", DataTypeGenerator.GenerateTypeDeclaration(o.HasClientType ? o.ClientType : o, false, o.HasWinFormsBindings)));
			code.AppendLine("\t{");
			code.AppendLine("\t\t//Automatic Data Update Support");
			code.AppendLine(GenerateProxyAutoDataCode(o));
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
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementProxyCode45(de));
			code.AppendLine("\t}");
			return code.ToString();
		}

		private static string GenerateProxyAutoDataCode(Data o)
		{
			if (!o.Parent.Owner.EnableExperimental) return "";
			if (!o.AutoDataEnabled) return "";

			var code = new StringBuilder();
			if (Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET30 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35 || Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET35Client)
			{
				code.AppendLine(Globals.CurrentGenerationTarget == ProjectGenerationFramework.NET30 ? "\t\tprivate readonly System.Threading.ReaderWriterLock __autodatalock = new System.Threading.ReaderWriterLock();" : "\t\tprivate readonly System.Threading.ReaderWriterLockSlim __autodatalock = new System.Threading.ReaderWriterLockSlim();");
				code.AppendLine(string.Format("\t\tprivate static readonly System.Collections.Generic.Dictionary<Guid, {0}> __autodata;", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o)));
				code.AppendLine(string.Format("\t\tstatic {0}()", o.HasClientType ? o.ClientType.Name : o.Name));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\t__autodata = new System.Collections.Generic.Dictionary<Guid, {0}>();", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o)));
				code.AppendLine("\t\t}");
				code.AppendLine("\t\t[OnDeserialized]");
				code.AppendLine("\t\tprivate void OnDeserialized(StreamingContext context)");
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\tlock(((ICollection)__autodata).SyncRoot) {{ __autodata.Add({0}, this); }}", o.AutoDataID.HasClientType ? o.AutoDataID.ClientName : o.AutoDataID.DataName));
				code.AppendLine("\t\t}");
				code.AppendLine(string.Format("\t\t~{0}()", o.HasClientType ? o.ClientType.Name : o.Name));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\t{0} t;", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o)));
				code.AppendLine(string.Format("\t\t\tlock(((ICollection)__autodata).SyncRoot) {{ __autodata.Remove({0}, out t); }}", o.AutoDataID.HasClientType ? o.AutoDataID.ClientName : o.AutoDataID.DataName));
				code.AppendLine("\t\t}");
			}
			else
			{
				code.AppendLine("\t\tprivate readonly System.Threading.ReaderWriterLockSlim __autodatalock = new System.Threading.ReaderWriterLockSlim();");
				code.AppendLine(string.Format("\t\tprivate static readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, {0}> __autodata;", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o)));
				code.AppendLine(string.Format("\t\tstatic {0}()", o.HasClientType ? o.ClientType.Name : o.Name));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\t__autodata = new System.Collections.Concurrent.ConcurrentDictionary<Guid, {0}>();", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o)));
				code.AppendLine("\t\t}");
				code.AppendLine("\t\t[OnDeserialized]");
				code.AppendLine("\t\tprivate void OnDeserialized(StreamingContext context)");
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\t__autodata.TryAdd({0}, this);", o.AutoDataID.HasClientType ? o.AutoDataID.ClientName : o.AutoDataID.DataName));
				code.AppendLine("\t\t}");
				code.AppendLine(string.Format("\t\t~{0}()", o.HasClientType ? o.ClientType.Name : o.Name));
				code.AppendLine("\t\t{");
				code.AppendLine(string.Format("\t\t\t{0} t;", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o)));
				code.AppendLine(string.Format("\t\t\t__autodata.TryRemove({0}, out t);", o.AutoDataID.HasClientType ? o.AutoDataID.ClientName : o.AutoDataID.DataName));
				code.AppendLine("\t\t}");
			}
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

			code.AppendLine("\t\t//Properties");
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementXAMLCodeRT8(de));
			code.AppendLine();

			code.AppendLine("\t\t//Implicit Conversion");
			code.AppendLine(string.Format("\t\tpublic static implicit operator {0}({1} Data)", o.HasClientType ? DataTypeGenerator.GenerateType(o.ClientType) : DataTypeGenerator.GenerateType(o), DataTypeGenerator.GenerateType(o.XAMLType)));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tif (Data == null) return null;");
			code.AppendLine(string.Format("\t\t\t{0} v = null;", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t\tif (Window.Current.Dispatcher.HasThreadAccess) v = ConvertFromXAMLObject(Data);");
			code.AppendLine("\t\t\telse Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { v = ConvertToXAMLObject(Data); });");
			code.AppendLine("\t\t\treturn v;");
			code.AppendLine("\t\t}");
			code.AppendLine(string.Format("\t\tpublic static implicit operator {0}({1} Data)", DataTypeGenerator.GenerateType(o.XAMLType), DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o)));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tif (Data == null) return null;");
			code.AppendLine(string.Format("\t\t\t{0} v = null;", o.XAMLType.Name));
			code.AppendLine("\t\t\tif (Window.Current.Dispatcher.HasThreadAccess) v = ConvertToXAMLObject(Data);");
			code.AppendLine("\t\t\telse Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { v = ConvertToXAMLObject(Data); });");
			code.AppendLine("\t\t\treturn v;");
			code.AppendLine("\t\t}");
			code.AppendLine();

			code.AppendLine("\t\t//Constructors");
			code.AppendLine(string.Format("\t\tpublic {0}()", o.XAMLType.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}({1} Data)", o.XAMLType.Name, DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o)));
			code.AppendLine("\t\t{");
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementXAMLConstructorCode45(de, o));
			code.AppendLine("\t\t}");
			code.AppendLine();

			code.AppendLine("\t\t//XAML/DTO Conversion Functions");
			code.AppendLine(string.Format("\t\tpublic static {0} ConvertFromXAMLObject({1} Data)", o.HasClientType ? o.ClientType.Name : o.Name, DataTypeGenerator.GenerateType(o.XAMLType)));
			code.AppendLine("\t\t{");
			code.AppendLine(string.Format("\t\t\t{0} DTO = new {0}();", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o)));
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementXAMLConversionFromXAML45(de, o));
			code.AppendLine("\t\t\treturn DTO;");
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic static {0} ConvertToXAMLObject({1} Data)", o.XAMLType.Name, DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o)));
			code.AppendLine("\t\t{");
			code.AppendLine(string.Format("\t\t\t{0} XAML = new {0}();", DataTypeGenerator.GenerateType(o.XAMLType)));
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementXAMLConversionToXAML45(de, o));
			code.AppendLine("\t\t\treturn XAML;");
			code.AppendLine("\t\t}");
			code.AppendLine("\t}");

			return code.ToString();
		}

		private static string GenerateElementServerCode45(DataElement o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			if (o.IsDataMember)
				code.AppendFormat("\t\t[DataMember({0}{1}{2}Name = \"{3}\")] ", o.EmitDefaultValue ? "EmitDefaultValue = false, " : "", o.IsRequired ? "IsRequired = true, " : "", o.Order >= 0 ? string.Format("Order = {0}, ", o.Order) : "", o.HasClientType ? o.ClientName : o.DataName);
			code.AppendLine(string.Format("{3}public {0} {1} {{ get; {2}set; }}", DataTypeGenerator.GenerateType(o.DataType), o.DataName, o.IsReadOnly ? "protected " : "", (o.IsDataMember && o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.XML) ? "\t\t[XmlIgnore] " : ""));
			return code.ToString();
		}

		private static string GenerateElementProxyCode45(DataElement o)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			var code = new StringBuilder();
			if (o.Owner.Parent.Owner.EnableExperimental && o.AutoDataEnabled) code.AppendLine(string.Format("\t\tprivate bool {0}Changed;", o.HasClientType ? o.ClientName : o.DataName));
			code.AppendLine(string.Format("\t\tprivate {0} {1}Field;", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName));
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.AppendFormat("\t\t[DataMember({0}{1}{2}Name = \"{3}\")] ", o.EmitDefaultValue ? "EmitDefaultValue = false, " : "", o.IsRequired ? "IsRequired = true, " : "", o.Order >= 0 ? string.Format("Order = {0}, ", o.Order) : "", o.HasClientType ? o.ClientName : o.DataName);
			if (o.Owner.Parent.Owner.EnableExperimental && o.AutoDataEnabled)
				code.AppendLine(string.Format("public {0} {1} {{ get {{ __autodatalock.{4}; try {{ return {1}Field; }} finally {{ __autodatalock.ExitReadLock(); }} }} {2}set {{ __autodatalock.{5}; try {{ {1}Field = value; {1}Changed = true; {3}}} finally {{ __autodatalock.ExitWriteLock(); }} }} }}", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.IsReadOnly ? "protected " : "", o.GenerateWinFormsSupport ? "NotifyPropertyChanged(); " : "", o.AutoDataTimeout == 0 ? "EnterReadLock()" : string.Format("TryEnterReadLock({0})", o.AutoDataTimeout), o.AutoDataTimeout == 0 ? "EnterWriteLock()" : string.Format("TryEnterWriteLock({0})", o.AutoDataTimeout)));
			else
				code.AppendLine(string.Format("public {0} {1} {{ get {{ return {1}Field; }} {2}set {{ {1}Field = value; {3}}} }}", DataTypeGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.IsReadOnly ? "protected " : "", o.GenerateWinFormsSupport ? "NotifyPropertyChanged(); " : ""));
			return code.ToString();
		}

		private static DataType GetPreferredXAMLType(DataType value)
		{
			if (value.TypeMode == DataTypeMode.Collection)
			{
				if (value.CollectionGenericType.TypeMode == DataTypeMode.Class || value.CollectionGenericType.TypeMode == DataTypeMode.Struct)
				{
					var t = value.CollectionGenericType as Data;
					if (t == null) return value;
					return t.HasXAMLType ? new DataType(value.Name, DataTypeMode.Collection) { Name = value.Name, CollectionGenericType = t.XAMLType } : new DataType(value.Name, DataTypeMode.Collection) { Name = value.Name, CollectionGenericType = t.HasClientType ? t.ClientType : t };
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

		private static string GenerateElementXAMLCodeRT8(DataElement o)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
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
				code.AppendLine(string.Format("\t\tpublic static {0} Get{1}(DependencyObject obj) {{ return ({0})obj.GetValue({1}Property); }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)), o.XAMLName));
				code.AppendLine(string.Format("\t\tpublic static void Set{1}(DependencyObject obj, {0} value) {{ obj.SetValue({1}Property, value); }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)), o.XAMLName));
				code.AppendLine(string.Format("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.RegisterAttached(\"{1}\", typeof({0}), typeof({2}), null)", DataTypeGenerator.GenerateType(o.XAMLType), o.XAMLName, DataTypeGenerator.GenerateType(o.Owner.XAMLType)));
			}
			else
			{
				code.AppendLine(string.Format("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue({1}Property); }} set {{ SetValue({1}Property, value); }} }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)), o.XAMLName));
				code.AppendLine(string.Format("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.Register(\"{1}\", typeof({0}), typeof({2}), null);", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.XAMLType)), o.XAMLName, DataTypeGenerator.GenerateType(o.Owner.XAMLType)));
			}
			return code.ToString();
		}

		private static string GenerateElementXAMLConstructorCode45(DataElement o, Data c)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			if (!o.HasXAMLType) return "";
			var code = new StringBuilder();

			if (o.XAMLType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{3} = new {1}[Data.{2}.GetLength(0)];", DataTypeGenerator.GenerateType(o.XAMLType), DataTypeGenerator.GenerateType(o.XAMLType).Replace("[]", ""), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tfor(int i = 0; i < Data.{0}.GetLength(0); i++) {{ v{1}[i] = Data.{0}[i]; }}", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached && (!o.IsReadOnly || Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8)) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(o.XAMLType), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tforeach({0} a in Data.{1}) {{ v{2}.Add(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached && (!o.IsReadOnly || Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8)) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(o.XAMLType), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tforeach(KeyValuePair<{0}> a in Data.{1}) {{ v{2}.Add(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached && (!o.IsReadOnly || Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8)) code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(this, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else
			{
				if (!o.IsAttached && (!o.IsReadOnly || Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8)) code.AppendLine(string.Format("\t\t\t{1} = Data.{0};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				else code.AppendLine(string.Format("\t\t\t{2}.Set{1}(this, Data.{0});", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName, DataTypeGenerator.GenerateType(c.XAMLType)));
			}

			return code.ToString();
		}

		private static string GenerateElementXAMLConversionToXAML45(DataElement o, Data c)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			if (!o.HasXAMLType) return "";
			var code = new StringBuilder();

			if (o.XAMLType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{3} = new {1}[Data.{2}.GetLength(0)];", DataTypeGenerator.GenerateType(o.XAMLType), DataTypeGenerator.GenerateType(o.XAMLType).Replace("[]", ""), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tfor(int i = 0; i < Data.{0}.GetLength(0); i++) {{ v{1}[i] = Data.{0}[i]; }}", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached && (!o.IsReadOnly || Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8)) code.AppendLine(string.Format("\t\t\tXAML.{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(XAML, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(o.XAMLType), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tforeach({0} a in Data.{1}) {{ v{2}.Add(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached && (!o.IsReadOnly || Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8)) code.AppendLine(string.Format("\t\t\tXAML.{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(XAML, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(o.XAMLType), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tforeach(KeyValuePair<{0}> a in Data.{1}) {{ v{2}.Add(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached && (!o.IsReadOnly || Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8)) code.AppendLine(string.Format("\t\t\tXAML.{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(XAML, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else
			{
				if (!o.IsAttached && (!o.IsReadOnly || Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8)) code.AppendLine(string.Format("\t\t\tXAML.{1} = Data.{0};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				else code.AppendLine(string.Format("\t\t\t{2}.Set{1}(XAML, Data.{0});", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName, DataTypeGenerator.GenerateType(c.XAMLType)));
			}

			return code.ToString();
		}

		private static string GenerateElementXAMLConversionFromXAML45(DataElement o, Data c)
		{
			if (o.IsHidden) return "";
			if (o.IsReadOnly) return "";
			if (!o.IsDataMember) return "";
			if (!o.HasXAMLType) return "";
			var code = new StringBuilder();

			if (o.XAMLType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{3} = new {1}[Data.{2}.GetLength(0)];", DataTypeGenerator.GenerateType(o.XAMLType), DataTypeGenerator.GenerateType(o.XAMLType).Replace("[]", ""), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tfor(int i = 0; i < Data.{0}.GetLength(0); i++) {{ v{1}[i] = Data.{0}[i]; }}", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached && (!o.IsReadOnly || Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8)) code.AppendLine(string.Format("\t\t\tDTO.{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(DTO, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(o.XAMLType), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tforeach({0} a in Data.{1}) {{ v{2}.Add(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached && (!o.IsReadOnly || Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8)) code.AppendLine(string.Format("\t\t\tDTO.{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(DTO, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\t{0} v{1} = new {0}();", DataTypeGenerator.GenerateType(o.XAMLType), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tforeach(KeyValuePair<{0}> a in Data.{1}) {{ v{2}.Add(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				if (!o.IsAttached && (!o.IsReadOnly || Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8)) code.AppendLine(string.Format("\t\t\tDTO.{0} = v{0};", o.XAMLName));
				else code.AppendLine(string.Format("t\t\t{0}.Set{1}(DTO, v{1});", DataTypeGenerator.GenerateType(c.XAMLType), o.XAMLName));
			}
			else
			{
				if (!o.IsAttached && (!o.IsReadOnly || Globals.CurrentGenerationTarget == ProjectGenerationFramework.WIN8)) code.AppendLine(string.Format("\t\t\tDTO.{1} = Data.{0};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				else code.AppendLine(string.Format("\t\t\t{2}.Set{1}(DTO, Data.{0});", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName, DataTypeGenerator.GenerateType(c.XAMLType)));
			}

			return code.ToString();
		}
	}
}