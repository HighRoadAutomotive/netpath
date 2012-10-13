using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFArchitect.Projects;

namespace WCFArchitect.Compiler.Generators
{
	internal static class DataCSGenerator
	{
		public static void VerifyCode(Data o)
		{
			if (string.IsNullOrEmpty(o.Name))
				Program.AddMessage(new CompileMessage("GS3000", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
			else
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.Name) == false)
					Program.AddMessage(new CompileMessage("GS3001", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));

			if (o.HasClientType)
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.ClientType.Name) == false)
					Program.AddMessage(new CompileMessage("GS3002", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));

			if (o.HasXAMLType)
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.XAMLType.Name) == false)
					Program.AddMessage(new CompileMessage("GS3003", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));

			foreach (DataElement d in o.Elements)
			{
				if (Helpers.RegExs.MatchCodeName.IsMatch(d.DataType.Name) == false && d.DataType.TypeMode != DataTypeMode.Array)
					Program.AddMessage(new CompileMessage("GS3001", "The data element '" + d.DataType.Name + "' in the '" + o.Name + "' data object contains invalid characters in the Name.", CompileMessageSeverity.ERROR, o, d, d.GetType(), o.ID, d.ID));

				if (d.HasClientType)
					if (Helpers.RegExs.MatchCodeName.IsMatch(d.ClientType.Name) == false && d.DataType.TypeMode != DataTypeMode.Array)
						Program.AddMessage(new CompileMessage("GS3002", "The data object '" + d.ClientType.Name + "' in the '" + o.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));

				if (d.HasXAMLType)
					if (Helpers.RegExs.MatchCodeName.IsMatch(d.XAMLType.Name) == false && d.DataType.TypeMode != DataTypeMode.Array)
						Program.AddMessage(new CompileMessage("GS3003", "The data object '" + d.XAMLType.Name + "' in the '" + o.Name + "' namespace contains invalid characters in the XAML Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
			}

			if (o.InheritedTypes.Any(a => a.Name.IndexOf("INotifyPropertyChanged", StringComparison.CurrentCultureIgnoreCase) >= 0))
				Program.AddMessage(new CompileMessage("GS3005", "The server data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace is unable to inherit from INotifyPropertyChanged.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
			if (o.HasXAMLType && o.XAMLType.InheritedTypes.Any(a => a.Name.IndexOf("INotifyPropertyChanged", StringComparison.CurrentCultureIgnoreCase) >= 0))
				Program.AddMessage(new CompileMessage("GS3006", "The XAML integration object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace is unable to inherit from INotifyPropertyChanged.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));

			if (o.InheritedTypes.Any(a => a.Name.IndexOf("DependencyObject", StringComparison.CurrentCultureIgnoreCase) >= 0))
				Program.AddMessage(new CompileMessage("GS3007", "The server data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace is unable to inherit from DependencyObject.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
			if (o.HasClientType && o.ClientType.InheritedTypes.Any(a => a.Name.IndexOf("DependencyObject", StringComparison.CurrentCultureIgnoreCase) >= 0))
				Program.AddMessage(new CompileMessage("GS3008", "The client data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace is unable to inherit from DependencyObject.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType(), o.Parent.ID, o.ID));
		}

		public static string GenerateServerCode30(Data o)
		{
			var code = new StringBuilder();
			foreach (DataType dt in o.KnownTypes)
				code.AppendLine(string.Format("\t[KnownType(typeof({0}))]", dt));
			code.AppendFormat("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t[DataContract(Name = \"{0}\", Namespace = \"{1}\")]{2}", o.HasClientType ? o.ClientType.Name : o.Name, o.Parent.URI, Environment.NewLine);
			code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			code.AppendLine("\t{");

			if (o.DataHasExtensionData)
			{
				code.AppendLine("\t\tprivate System.Runtime.Serialization.ExtensionDataObject extensionDataField;");
				code.AppendLine("\t\tpublic System.Runtime.Serialization.ExtensionDataObject ExtensionData { get { return extensionDataField; } set { extensionDataField = value; } }");
				code.AppendLine();
			}

			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementServerCode30(de));
			code.AppendLine("\t}");
			return code.ToString();
		}

		public static string GenerateServerCode35(Data o)
		{
			return GenerateServerCode40(o);
		}

		public static string GenerateServerCode40(Data o)
		{
			return GenerateServerCode45(o);
		}

		public static string GenerateServerCode45(Data o)
		{
			var code = new StringBuilder();
			foreach (DataType dt in o.KnownTypes)
				code.AppendLine(string.Format("\t[KnownType(typeof({0}))]", dt));
			code.AppendFormat("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t[DataContract({0}Name = \"{1}\", Namespace = \"{2}\")]{3}", o.IsReference ? "IsReference = true, " : "", o.HasClientType ? o.ClientType.Name : o.Name, o.Parent.URI, Environment.NewLine);
			code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
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

		public static string GenerateProxyCode30(Data o)
		{
			var code = new StringBuilder();
			foreach (DataType dt in o.ClientType != null ? o.ClientType.KnownTypes : o.KnownTypes)
				code.AppendLine(string.Format("\t[KnownType(typeof({0}))]", dt));
			code.AppendLine("\t[System.Diagnostics.DebuggerStepThroughAttribute]");
			code.AppendFormat("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t[DataContract(Name = \"{0}\", Namespace = \"{1}\")]{2}", o.HasClientType ? o.ClientType.Name : o.Name, o.Parent.URI, Environment.NewLine);
			code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o.HasClientType ? o.ClientType : o, o.ClientHasImpliedExtensionData, o.HasWinFormsBindings), Environment.NewLine);
			code.AppendLine("\t{");
			if (o.ClientHasExtensionData || o.ClientHasImpliedExtensionData)
			{
				code.AppendLine("\t\tprivate System.Runtime.Serialization.ExtensionDataObject extensionDataField;");
				code.AppendLine("\t\tpublic System.Runtime.Serialization.ExtensionDataObject ExtensionData { get { return extensionDataField; } set { extensionDataField = value; } }");
				code.AppendLine();
			}
			if(o.HasWinFormsBindings)
			{
				code.AppendLine("\t\tpublic event PropertyChangedEventHandler PropertyChanged;");
				code.AppendLine("\t\tprivate void NotifyPropertyChanged(string propertyName)");
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tif (PropertyChanged != null)");
				code.AppendLine("\t\t\t\tPropertyChanged(this, new PropertyChangedEventArgs(propertyName));");
				code.AppendLine("\t\t}");
				code.AppendLine();
			}
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementProxyCode30(de));
			code.AppendLine("\t}");
			return code.ToString();
		}

		public static string GenerateProxyCode35(Data o)
		{
			return GenerateProxyCode40(o);
		}

		public static string GenerateProxyCode40(Data o)
		{
			var code = new StringBuilder();
			foreach (DataType dt in o.ClientType != null ? o.ClientType.KnownTypes : o.KnownTypes)
				code.AppendLine(string.Format("\t[KnownType(typeof({0}))]", dt));
			code.AppendLine("\t[System.Diagnostics.DebuggerStepThroughAttribute]");
			code.AppendFormat("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t[DataContract({0}Name = \"{1}\", Namespace = \"{2}\")]{3}", o.IsReference ? "IsReference = true, " : "", o.HasClientType ? o.ClientType.Name : o.Name, o.Parent.URI, Environment.NewLine);
			code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o.HasClientType ? o.ClientType : o, o.ClientHasImpliedExtensionData, o.HasWinFormsBindings), Environment.NewLine);
			code.AppendLine("\t{");
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
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementProxyCode40(de));
			code.AppendLine("\t}");
			return code.ToString();
		}

		public static string GenerateProxyCode45(Data o)
		{
			var code = new StringBuilder();
			foreach (DataType dt in o.ClientType != null ? o.ClientType.KnownTypes : o.KnownTypes)
				code.AppendLine(string.Format("\t[KnownType(typeof({0}))]", dt));
			code.AppendLine("\t[System.Diagnostics.DebuggerStepThroughAttribute]");
			code.AppendFormat("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t[DataContract({0}Name = \"{1}\", Namespace = \"{2}\")]{3}", o.IsReference ? "IsReference = true, " : "", o.HasClientType ? o.ClientType.Name : o.Name , o.Parent.URI, Environment.NewLine);
			code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o.HasClientType ? o.ClientType : o, o.ClientHasImpliedExtensionData, o.HasWinFormsBindings), Environment.NewLine);
			code.AppendLine("\t{");
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
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementProxyCode45(de));
			code.AppendLine("\t}");
			return code.ToString();
		}

		public static string GenerateXAMLCode30(Data o)
		{
			if (o.HasXAMLType == false) return "";
			if (o.Elements.Any(de => de.HasXAMLType) == false) return "";

			//This is a shim to ensure there is ALWAYS a XAML name.
			if (string.IsNullOrEmpty(o.XAMLType.Name)) o.XAMLType.Name = o.Name + "XAML";

			var code = new StringBuilder();
			code.AppendFormat("\t//XAML Integration Object for the {0} DTO{1}", o.HasClientType ? o.ClientType.Name : o.Name, Environment.NewLine);
			code.AppendFormat("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o.XAMLType), Environment.NewLine);
			code.AppendLine("\t{");

			if (o.XAMLHasExtensionData)
			{
				code.AppendLine("\t\tprivate System.Runtime.Serialization.ExtensionDataObject extensionDataField;");
				code.AppendLine("\t\tpublic System.Runtime.Serialization.ExtensionDataObject ExtensionData { get { return extensionDataField; } set { extensionDataField = value; } }");
				code.AppendLine();
			}

			code.AppendLine("\t\t//Properties");
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementXAMLCode30(de));
			code.AppendLine();

			code.AppendLine("\t\t//Implicit Conversion");
			code.AppendFormat("\t\tpublic static implicit operator {0}({1} Data){2}", o.HasClientType ? DataTypeCSGenerator.GenerateType(o.ClientType) : DataTypeCSGenerator.GenerateType(o), DataTypeCSGenerator.GenerateType(o.XAMLType), Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tif (Data == null) return null;");
			code.AppendLine("\t\t\tif (Application.Current.Dispatcher.CheckAccess())");
			code.AppendFormat("\t\t\t\treturn {0}.ConvertFromXAMLObject(Data);{1}", o.XAMLType.Name, Environment.NewLine);
			code.AppendLine("\t\t\telse");
			code.AppendFormat("\t\t\t\treturn ({0})Application.Current.Dispatcher.Invoke(new Func<{0}>(() => {1}.ConvertFromXAMLObject(Data)), System.Windows.Threading.DispatcherPriority.Normal);{2}", DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o), DataTypeCSGenerator.GenerateType(o.XAMLType), Environment.NewLine);
			code.AppendLine("\t\t}");
			code.AppendFormat("\t\tpublic static implicit operator {0}({1} Data){2}", DataTypeCSGenerator.GenerateType(o.XAMLType), DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o), Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tif (Data == null) return null;");
			code.AppendLine("\t\t\tif (Application.Current.Dispatcher.CheckAccess())");
			code.AppendFormat("\t\t\t\treturn {0}.ConvertToXAMLObject(Data);{1}", o.XAMLType.Name, Environment.NewLine);
			code.AppendLine("\t\t\telse");
			code.AppendFormat("\t\t\t\treturn ({0})Application.Current.Dispatcher.Invoke(new Func<{1}>(() => {0}.ConvertToXAMLObject(Data)), System.Windows.Threading.DispatcherPriority.Normal);{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientType.Name : o.Name, Environment.NewLine);
			code.AppendLine("\t\t}");
			code.AppendLine();

			code.AppendLine("\t\t//Constructors");
			code.AppendFormat("\t\tpublic {0}({1} Data){2}", o.XAMLType.Name, DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o), Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tType t_DT = Data.GetType();");
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementXAMLConstructorCode30(de, o));
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendFormat("\t\tpublic {0}(){1}", o.XAMLType.Name, Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t}");
			code.AppendLine();

			code.AppendLine("\t\t//XAML/DTO Conversion Functions");
			code.AppendFormat("\t\tpublic static {0} ConvertFromXAMLObject({1} Data){2}", o.HasClientType ? o.ClientType.Name : o.Name, DataTypeCSGenerator.GenerateType(o.XAMLType), Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendFormat("\t\t\t{0} DTO = new {0}();{1}", DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o), Environment.NewLine);
			code.AppendLine("\t\t\tType t_XAML = Data.GetType();");
			code.AppendLine("\t\t\tType t_DTO = DTO.GetType();");
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementXAMLConversionFromXAML30(de, o));
			code.AppendLine("\t\t\treturn DTO;");
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendFormat("\t\tpublic static {0} ConvertToXAMLObject({1} Data){2}", o.XAMLType.Name, DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o), Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendFormat("\t\t\t{0} XAML = new {0}();{1}", DataTypeCSGenerator.GenerateType(o.XAMLType), Environment.NewLine);
			code.AppendLine("\t\t\tType t_DTO = Data.GetType();");
			code.AppendLine("\t\t\tType t_XAML = XAML.GetType();");
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementXAMLConversionToXAML30(de, o));
			code.AppendLine("\t\t\treturn XAML;");
			code.AppendLine("\t\t}");
			code.AppendLine("\t}");

			return code.ToString();
		}

		public static string GenerateXAMLCode35(Data o)
		{
			return GenerateXAMLCode40(o);
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
			code.AppendFormat("\t//XAML Integration Object for the {0} DTO{1}", o.HasClientType ? o.ClientType.Name : o.Name, Environment.NewLine);
			code.AppendFormat("\t[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion, Environment.NewLine);
			code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o.XAMLType), Environment.NewLine);
			code.AppendLine("\t{");

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
			code.AppendFormat("\t\tpublic static implicit operator {0}({1} Data){2}", o.HasClientType ? DataTypeCSGenerator.GenerateType(o.ClientType) : DataTypeCSGenerator.GenerateType(o), DataTypeCSGenerator.GenerateType(o.XAMLType), Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tif (Data == null) return null;");
			code.AppendLine("\t\t\tif (Application.Current.Dispatcher.CheckAccess())");
			code.AppendFormat("\t\t\t\treturn {0}.ConvertFromXAMLObject(Data);{1}", o.XAMLType.Name, Environment.NewLine);
			code.AppendLine("\t\t\telse");
			code.AppendFormat("\t\t\t\treturn ({0})Application.Current.Dispatcher.Invoke(new Func<{0}>(() => {1}.ConvertFromXAMLObject(Data)), System.Windows.Threading.DispatcherPriority.Normal);{2}", DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o), DataTypeCSGenerator.GenerateType(o.XAMLType), Environment.NewLine);
			code.AppendLine("\t\t}");
			code.AppendFormat("\t\tpublic static implicit operator {0}({1} Data){2}", DataTypeCSGenerator.GenerateType(o.XAMLType), DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o), Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tif (Data == null) return null;");
			code.AppendLine("\t\t\tif (Application.Current.Dispatcher.CheckAccess())");
			code.AppendFormat("\t\t\t\treturn {0}.ConvertToXAMLObject(Data);{1}", o.XAMLType.Name, Environment.NewLine);
			code.AppendLine("\t\t\telse");
			code.AppendFormat("\t\t\t\treturn ({0})Application.Current.Dispatcher.Invoke(new Func<{1}>(() => {0}.ConvertToXAMLObject(Data)), System.Windows.Threading.DispatcherPriority.Normal);{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientType.Name : o.Name, Environment.NewLine);
			code.AppendLine("\t\t}");
			code.AppendLine();

			code.AppendLine("\t\t//Constructors");
			code.AppendFormat("\t\tpublic {0}({1} Data){2}", o.XAMLType.Name, DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o), Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tType t_DT = Data.GetType();");
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementXAMLConstructorCode45(de, o));
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendFormat("\t\tpublic {0}(){1}", o.XAMLType.Name, Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t}");
			code.AppendLine();

			code.AppendLine("\t\t//XAML/DTO Conversion Functions");
			code.AppendFormat("\t\tpublic static {0} ConvertFromXAMLObject({1} Data){2}", o.HasClientType ? o.ClientType.Name : o.Name, DataTypeCSGenerator.GenerateType(o.XAMLType), Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendFormat("\t\t\t{0} DTO = new {0}();{1}", DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o), Environment.NewLine);
			code.AppendLine("\t\t\tType t_XAML = Data.GetType();");
			code.AppendLine("\t\t\tType t_DTO = DTO.GetType();");
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementXAMLConversionFromXAML45(de, o));
			code.AppendLine("\t\t\treturn DTO;");
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendFormat("\t\tpublic static {0} ConvertToXAMLObject({1} Data){2}", o.XAMLType.Name, DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o), Environment.NewLine);
			code.AppendLine("\t\t{");
			code.AppendFormat("\t\t\t{0} XAML = new {0}();{1}", DataTypeCSGenerator.GenerateType(o.XAMLType), Environment.NewLine);
			code.AppendLine("\t\t\tType t_DTO = Data.GetType();");
			code.AppendLine("\t\t\tType t_XAML = XAML.GetType();");
			foreach (DataElement de in o.Elements)
				code.Append(GenerateElementXAMLConversionToXAML45(de, o));
			code.AppendLine("\t\t\treturn XAML;");
			code.AppendLine("\t\t}");
			code.AppendLine("\t}");

			return code.ToString();
		}

		private static string GenerateElementServerCode30(DataElement o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			code.AppendFormat("\t\tprivate {0} m_{1};{2}", DataTypeCSGenerator.GenerateType(o.DataType), o.DataName, Environment.NewLine);
			if (o.IsDataMember)
			{
				code.AppendFormat("\t\t[DataMember({0}{1}{2}Name = \"{3}\")] ", o.EmitDefaultValue ? "EmitDefaultValue = false, " : "", o.IsRequired ? "IsRequired = true, " : "", o.Order >= 0 ? string.Format("Order = {0}, ", o.Order) : "", o.HasClientType ? o.ClientName : o.DataName);
			}
			else
			{
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.DataContract) code.Append("\t\t");
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.XML) code.Append("\t\t[XmlIgnore()]");
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.Auto) code.Append("\t\t");
			}
			if (o.IsReadOnly == false) code.AppendFormat("{0} {1} {2} {{ get {{ return m_{2}; }} set {{ m_{2} = value; }} }}{3}", DataTypeCSGenerator.GenerateScope(o.DataType.Scope), DataTypeCSGenerator.GenerateType(o.DataType), o.DataName, Environment.NewLine);
			else code.AppendFormat("{0} {1} {2} {{ get {{ return m_{2}; }} protected set {{ m_{2} = value; }} }}{3}", DataTypeCSGenerator.GenerateScope(o.DataType.Scope), o.DataType, o.DataName, Environment.NewLine);
			return code.ToString();
		}

		private static string GenerateElementServerCode35(DataElement o)
		{
			return GenerateElementServerCode40(o);
		}

		private static string GenerateElementServerCode40(DataElement o)
		{
			return GenerateElementServerCode45(o);
		}

		private static string GenerateElementServerCode45(DataElement o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			if (o.IsDataMember)
			{
				code.AppendFormat("\t\t[DataMember({0}{1}{2}Name = \"{3}\")] ", o.EmitDefaultValue ? "EmitDefaultValue = false, " : "", o.IsRequired ? "IsRequired = true, " : "", o.Order >= 0 ? string.Format("Order = {0}, ", o.Order) : "", o.HasClientType ? o.ClientName : o.DataName);
			}
			else
			{
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.DataContract) code.Append("\t\t[IgnoreDataMember()]");
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.XML) code.Append("\t\t[XmlIgnore()]");
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.Auto) code.Append("\t\t[IgnoreDataMember()]");
			}
			code.AppendFormat(o.IsReadOnly == false ? "{0} {1} {2} {{ get; set; }}{3}" : "{0} {1} {2} {{ get; protected set; }}{3}", DataTypeCSGenerator.GenerateScope(o.DataType.Scope), DataTypeCSGenerator.GenerateType(o.DataType), o.DataName, Environment.NewLine);
			return code.ToString();
		}

		private static string GenerateElementProxyCode30(DataElement o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			code.AppendFormat("\t\tprivate {0} {1}Field;{2}", DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.DataName, Environment.NewLine);
			if (o.IsDataMember)
			{
				code.AppendFormat("\t\t[DataMember({0}{1}{2}Name = \"{3}\")] ", o.EmitDefaultValue ? "EmitDefaultValue = false, " : "", o.IsRequired ? "IsRequired = true, " : "", o.Order >= 0 ? string.Format("Order = {0}, ", o.Order) : "", o.HasClientType ? o.ClientName : o.DataName);
			}
			else
			{
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.DataContract) code.Append("\t\t");
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.XML) code.Append("\t\t[XmlIgnore()]");
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.Auto) code.Append("\t\t");
			}
			if (!o.GenerateWinFormsSupport)
			{
				if (o.IsReadOnly == false) code.AppendFormat("{0} {1} {2} {{ get {{ return {2}Field; }} set {{ {2}Field = value; }} }}{3}", DataTypeCSGenerator.GenerateScope(o.HasClientType ? o.ClientType.Scope : o.DataType.Scope), DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				else code.AppendFormat("{0} {1} {2} {{ get {{ return {2}Field; }} protected set {{ {2}Field = value; }} }}{3}", DataTypeCSGenerator.GenerateScope(o.HasClientType ? o.ClientType.Scope : o.DataType.Scope), DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
			}
			else
			{
				if (o.IsReadOnly == false) code.AppendFormat("{0} {1} {2} {{ get {{ return {2}Field; }} set {{ if (value != {2}Field) {{ {2}Field = value; NotifyPropertyChanged(\"{2}\"); }} }} }}{3}", DataTypeCSGenerator.GenerateScope(o.HasClientType ? o.ClientType.Scope : o.DataType.Scope), DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				else code.AppendFormat("{0} {1} {2} {{ get {{ return {2}Field; }} protected set {{ if (value != {2}Field) {{ {2}Field = value; NotifyPropertyChanged(\"{2}\"); }} }} }}{3}", DataTypeCSGenerator.GenerateScope(o.HasClientType ? o.ClientType.Scope : o.DataType.Scope), DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
			}
			return code.ToString();
		}

		private static string GenerateElementProxyCode35(DataElement o)
		{
			return GenerateElementProxyCode40(o);
		}

		private static string GenerateElementProxyCode40(DataElement o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			code.AppendFormat("\t\tprivate {0} {1}Field;{2}", DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.DataName, Environment.NewLine);
			if (o.IsDataMember)
			{
				code.AppendFormat("\t\t[DataMember({0}{1}{2}Name = \"{3}\")] ", o.EmitDefaultValue ? "EmitDefaultValue = false, " : "", o.IsRequired ? "IsRequired = true, " : "", o.Order >= 0 ? string.Format("Order = {0}, ", o.Order) : "", o.HasClientType ? o.ClientName : o.DataName);
			}
			else
			{
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.DataContract) code.Append("\t\t[IgnoreDataMember()]");
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.XML) code.Append("\t\t[XmlIgnore()]");
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.Auto) code.Append("\t\t[IgnoreDataMember()]");
			}
			if (!o.GenerateWinFormsSupport)
			{
				if (o.IsReadOnly == false) code.AppendFormat("{0} {1} {2} {{ get {{ return {2}Field; }} set {{ {2}Field = value; }} }}{3}", DataTypeCSGenerator.GenerateScope(o.HasClientType ? o.ClientType.Scope : o.DataType.Scope), DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				else code.AppendFormat("{0} {1} {2} {{ get {{ return {2}Field; }} protected set {{ {2}Field = value; }} }}{3}", DataTypeCSGenerator.GenerateScope(o.HasClientType ? o.ClientType.Scope : o.DataType.Scope), DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
			}
			else
			{
				if (o.IsReadOnly == false) code.AppendFormat("{0} {1} {2} {{ get {{ return {2}Field; }} set {{ if (value != {2}Field) {{ {2}Field = value; NotifyPropertyChanged(\"{2}\"); }} }} }}{3}", DataTypeCSGenerator.GenerateScope(o.HasClientType ? o.ClientType.Scope : o.DataType.Scope), DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				else code.AppendFormat("{0} {1} {2} {{ get {{ return {2}Field; }} protected set {{ if (value != {2}Field) {{ {2}Field = value; NotifyPropertyChanged(\"{2}\"); }} }} }}{3}", DataTypeCSGenerator.GenerateScope(o.HasClientType ? o.ClientType.Scope : o.DataType.Scope), DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
			}
			return code.ToString();
		}

		private static string GenerateElementProxyCode45(DataElement o)
		{
			if (o.IsHidden) return "";
			var code = new StringBuilder();
			code.AppendFormat("\t\tprivate {0} {1}Field;{2}", DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.DataName, Environment.NewLine);
			if (o.IsDataMember)
			{
				code.AppendFormat("\t\t[DataMember({0}{1}{2}Name = \"{3}\")] ", o.EmitDefaultValue ? "EmitDefaultValue = false, " : "", o.IsRequired ? "IsRequired = true, " : "", o.Order >= 0 ? string.Format("Order = {0}, ", o.Order) : "", o.HasClientType ? o.ClientName : o.DataName);
			}
			else
			{
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.DataContract) code.Append("\t\t[IgnoreDataMember()]");
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.XML) code.Append("\t\t[XmlIgnore()]");
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.Auto) code.Append("\t\t[IgnoreDataMember()]");
			}
			if (!o.GenerateWinFormsSupport)
			{
				if (o.IsReadOnly == false) code.AppendFormat("{0} {1} {2} {{ get {{ return {2}Field; }} set {{ {2}Field = value; }} }}{3}", DataTypeCSGenerator.GenerateScope(o.HasClientType ? o.ClientType.Scope : o.DataType.Scope), DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				else code.AppendFormat("{0} {1} {2} {{ get {{ return {2}Field; }} protected set {{ {2}Field = value; }} }}{3}", DataTypeCSGenerator.GenerateScope(o.HasClientType ? o.ClientType.Scope : o.DataType.Scope), DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
			}
			else
			{
				if (o.IsReadOnly == false) code.AppendFormat("{0} {1} {2} {{ get {{ return {2}Field; }} set {{ if (value != {2}Field) {{ {2}Field = value; NotifyPropertyChanged(); }} }} }}{3}", DataTypeCSGenerator.GenerateScope(o.HasClientType ? o.ClientType.Scope : o.DataType.Scope), DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				else code.AppendFormat("{0} {1} {2} {{ get {{ return {2}Field; }} protected set {{ if (value != {2}Field) {{ {2}Field = value; NotifyPropertyChanged(); }} }} }}{3}", DataTypeCSGenerator.GenerateScope(o.HasClientType ? o.ClientType.Scope : o.DataType.Scope), DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
			}
			return code.ToString();
		}

		private static string GenerateElementXAMLConstructorCode30(DataElement o, Data c)
		{
			return GenerateElementXAMLConstructorCode35(o, c);
		}

		private static string GenerateElementXAMLConstructorCode35(DataElement o, Data c)
		{
			return GenerateElementXAMLConstructorCode40(o, c);
		}

		private static string GenerateElementXAMLConstructorCode40(DataElement o, Data c)
		{
			return GenerateElementXAMLConstructorCode45(o, c);
		}

		private static string GenerateElementXAMLConstructorCode45(DataElement o, Data c)
		{
			if (o.IsHidden) return "";
			if (o.IsDataMember == false) return "";
			var code = new StringBuilder();

			code.AppendFormat("\t\t\tFieldInfo fi_{0} = t_DT.GetField(\"{0}Field\", BindingFlags.NonPublic | BindingFlags.Instance);{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
			code.AppendFormat("\t\t\tif(fi_{0} != null){1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
			if (o.XAMLType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine("\t\t\t{");
				code.AppendFormat("\t\t\t\t{0} v_{1} = fi_{1}.GetValue(Data) as {0};{2}", DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendFormat("\t\t\t\tif(v_{0} != null){1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendLine("\t\t\t\t{");
				if (o.Owner.Parent.Owner.CollectionSerializationOverride == ProjectCollectionSerializationOverride.List)
				{
					code.AppendFormat("\t\t\t\t\t{0} tv = new {1}[v_{2}.Count];{3}", DataTypeCSGenerator.GenerateType(o.XAMLType), DataTypeCSGenerator.GenerateType(o.XAMLType).Replace("[]", ""), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					code.AppendFormat("\t\t\t\t\tfor(int i = 0; i < v_{0}.Count; i++) {{ try {{ tv[i] = v_{0}[i]; }} catch {{ continue; }} }}{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					if (o.IsAttached) code.AppendFormat("\t\t\t\t\t{0}.Set{1}(this, tv);{2}", c.HasClientType ? c.ClientType.Name : c.Name, DataTypeCSGenerator.GenerateType(o.XAMLType), Environment.NewLine);
				}
				else
				{
					code.AppendFormat("\t\t\t\t\t{0} tv = new {1}[v_{2}.GetLength(0)];{3}", DataTypeCSGenerator.GenerateType(o.XAMLType), DataTypeCSGenerator.GenerateType(o.XAMLType).Replace("[]", ""), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					code.AppendFormat("\t\t\t\t\tfor(int i = 0; i < v_{0}.GetLength(0); i++) {{ try {{ tv[i] = v_{0}[i]; }} catch {{ continue; }} }}{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					if (o.IsAttached) code.AppendFormat("\t\t\t\t\t{0}.Set{1}(this, tv);{2}", c.HasClientType ? c.ClientType.Name : c.Name, DataTypeCSGenerator.GenerateType(o.XAMLType), Environment.NewLine);
				}
				code.AppendLine("\t\t\t\t}");
				code.AppendLine("\t\t\t}");
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine("\t\t\t{");
				code.AppendFormat("\t\t\t\t{0} v_{1} = fi_{1}.GetValue(Data) as {0};{2}", DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendFormat("\t\t\t\tif(v_{0} != null){1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendLine("\t\t\t\t{");
				code.AppendFormat("\t\t\t\t\t{0} tv = new {0}();{1}", DataTypeCSGenerator.GenerateType(o.XAMLType), Environment.NewLine);
				code.AppendFormat("\t\t\t\t\tforeach({0} a in v_{1}) {{ tv.Add(a); }}{2}", DataTypeCSGenerator.GenerateTypeGenerics(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				if (o.IsAttached) code.AppendFormat("\t\t\t\t\t{0}.Set{1}(this, tv);{2}", c.HasClientType ? c.ClientType.Name : c.Name, DataTypeCSGenerator.GenerateType(o.XAMLType), Environment.NewLine);
				code.AppendLine("\t\t\t\t}");
				code.AppendLine("\t\t\t}");
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine("\t\t\t{");
				code.AppendFormat("\t\t\t\t{0} tv = new {0}();{1}", DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), Environment.NewLine);
				code.AppendFormat("\t\t\t\tDictionary<{1}> v_{0} = fi_{0}.GetValue(Data) as Dictionary<{1}>;{2}", o.HasClientType ? o.ClientName : o.DataName, DataTypeCSGenerator.GenerateTypeGenerics(o.XAMLType), Environment.NewLine);
				code.AppendFormat("\t\t\t\tif(v_{0} != null){1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendFormat("\t\t\t\t\tforeach(KeyValuePair<{0}> a in v_{1}) {{ tv.Add(a.Key, a.Value); }}{2}", DataTypeCSGenerator.GenerateTypeGenerics(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				if (o.IsAttached) code.AppendFormat("\t\t\t\t\t{0}.Set{1}(this, tv);{2}", c.HasClientType ? c.ClientType.Name : c.Name, DataTypeCSGenerator.GenerateType(o.XAMLType), Environment.NewLine);
				code.AppendLine("\t\t\t}");
			}
			else
			{
				if (o.IsAttached == false) code.AppendFormat("\t\t\t\t{2} = ({0})fi_{1}.GetValue(Data);{3}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName, Environment.NewLine);
				else code.AppendFormat("\t\t\t\t{3}.Set{2}(this, (({0})fi_{1}.GetValue(Data));{4}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName, DataTypeCSGenerator.GenerateType(o.XAMLType), Environment.NewLine);
			}

			return code.ToString();
		}

		private static string GenerateElementXAMLConversionToXAML30(DataElement o, Data c)
		{
			return GenerateElementXAMLConversionToXAML35(o, c);
		}

		private static string GenerateElementXAMLConversionToXAML35(DataElement o, Data c)
		{
			return GenerateElementXAMLConversionToXAML40(o, c);
		}

		private static string GenerateElementXAMLConversionToXAML40(DataElement o, Data c)
		{
			return GenerateElementXAMLConversionToXAML45(o, c);
		}

		private static string GenerateElementXAMLConversionToXAML45(DataElement o, Data c)
		{
			if (o.IsHidden) return "";
			if (o.IsDataMember == false) return "";
			var code = new StringBuilder();

			if (o.HasXAMLType)
				if (o.IsAttached == false)
					code.AppendFormat("\t\t\tPropertyInfo pi_{0} = t_XAML.GetProperty(\"{0}\", BindingFlags.Public | BindingFlags.Instance);{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
			else
				if (o.IsAttached == false)
					code.AppendFormat(o.DataType.Scope == DataScope.Public ? "\t\t\tPropertyInfo pi_{0} = t_XAML.GetProperty(\"{0}\", BindingFlags.Public | BindingFlags.Instance);{1}" : "\t\t\tPropertyInfo pi_{0} = t_XAML.GetProperty(\"{0}\", BindingFlags.NonPublic | BindingFlags.Instance);{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
			code.AppendFormat("\t\t\tFieldInfo fi_{0} = t_DTO.GetField(\"{0}Field\", BindingFlags.NonPublic | BindingFlags.Instance);{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
			code.AppendFormat(o.IsAttached == false ? "\t\t\tif(fi_{0} != null && pi_{0} != null){1}" : "\t\t\tif(fi_{0} != null){1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
			if (o.XAMLType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine("\t\t\t{");
				code.AppendFormat("\t\t\t\t{0} v_{1} = fi_{1}.GetValue(Data) as {0};{2}", DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendFormat("\t\t\t\tif(v_{0} != null){1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendLine("\t\t\t\t{");
				if (o.Owner.Parent.Owner.CollectionSerializationOverride == ProjectCollectionSerializationOverride.List)
				{
					code.AppendFormat("\t\t\t\t\t{0} XAML_{1} = new {0}[v_{1}.Count];{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					if (o.IsAttached == false) code.AppendFormat("\t\t\t\t\tpi_{0}.SetValue(XAML, XAML_{0}, null);{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					else code.AppendFormat("\t\t\t\t\t{0}.Set{1}(XAML, XAML_{2});{3}", DataTypeCSGenerator.GenerateType(c.XAMLType), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					code.AppendFormat("\t\t\t\t\tfor(int i = 0; i < v_{0}.Count; i++) {{ XAML_{0}[i] = v_{0}[i]; }}{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				}
				else
				{
					code.AppendFormat("\t\t\t\t\t{0}[] XAML_{1} = new {0}[v_{1}.GetLength(0)];{2}", DataTypeCSGenerator.GenerateTypeGenerics(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					if (o.IsAttached == false) code.AppendFormat("\t\t\t\t\tpi_{0}.SetValue(XAML, XAML_{0}, null);{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					else code.AppendFormat("\t\t\t\t\t{0}.Set{1}(XAML, XAML_{2});{3}", DataTypeCSGenerator.GenerateType(c.XAMLType), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					code.AppendFormat("\t\t\t\t\tfor(int i = 0; i < v_{0}.GetLength(0); i++) {{ XAML_{0}[i] = v_{0}[i]; }}{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				}
				code.AppendLine("\t\t\t\t}");
				code.AppendLine("\t\t\t}");
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine("\t\t\t{");
				code.AppendFormat("\t\t\t\t{0} v_{1} = fi_{1}.GetValue(Data) as {0};{2}", DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendFormat("\t\t\t\tif(v_{0} != null){1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendLine("\t\t\t\t{");
				code.AppendFormat("\t\t\t\t\t{0} XAML_{1} = new {0}();{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				if (o.IsAttached == false) code.AppendFormat("\t\t\t\t\tpi_{0}.SetValue(XAML, XAML_{0}, null);{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				else code.AppendFormat("\t\t\t\t\t{0}.Set{1}(XAML, XAML_{2});{3}", DataTypeCSGenerator.GenerateType(c.XAMLType), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendFormat("\t\t\t\t\tforeach({1} a in v_{2}) {{ XAML_{0}.Add(a); }}{3}", o.HasClientType ? o.ClientName : o.DataName, DataTypeCSGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendLine("\t\t\t\t}");
				code.AppendLine("\t\t\t}");
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine("\t\t\t{");
				code.AppendFormat("\t\t\t\t{0} XAML_{1} = new {0}();{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				if (o.IsAttached == false) code.AppendFormat("\t\t\t\tpi_{0}.SetValue(XAML, XAML_{0}, null);{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				else code.AppendFormat("\t\t\t\t{0}.Set{1}(XAML, XAML_{2});{3}", DataTypeCSGenerator.GenerateType(c.XAMLType), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendFormat("\t\t\t\t{1} v_{0} = fi_{0}.GetValue(Data) as {1};{2}", o.HasClientType ? o.ClientName : o.DataName, DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), Environment.NewLine);
				code.AppendFormat("\t\t\t\tif(v_{2} != null) foreach(KeyValuePair<{1}> a in v_{2}) {{ XAML_{0}.Add(a.Key, a.Value); }}{3}", o.HasClientType ? o.ClientName : o.DataName, DataTypeCSGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendLine("\t\t\t}");
			}
			else
			{
				if (o.IsAttached == false) code.AppendFormat("\t\t\t\tpi_{1}.SetValue(XAML, ({0})fi_{1}.GetValue(Data), null);{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				else code.AppendFormat("\t\t\t\t{3}.Set{2}(XAML, ({0})fi_{1}.GetValue(Data));{4}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName, DataTypeCSGenerator.GenerateType(c.XAMLType), Environment.NewLine);
			}

			return code.ToString();
		}

		private static string GenerateElementXAMLConversionFromXAML30(DataElement o, Data c)
		{
			return GenerateElementXAMLConversionFromXAML35(o, c);
		}

		private static string GenerateElementXAMLConversionFromXAML35(DataElement o, Data c)
		{
			return GenerateElementXAMLConversionFromXAML40(o, c);
		}

		private static string GenerateElementXAMLConversionFromXAML40(DataElement o, Data c)
		{
			return GenerateElementXAMLConversionFromXAML45(o, c);
		}

		private static string GenerateElementXAMLConversionFromXAML45(DataElement o, Data c)
		{
			if (o.IsHidden) return "";
			if (o.IsDataMember == false) return "";
			var code = new StringBuilder();

			if (o.HasXAMLType)
				if (o.IsAttached == false)
					code.AppendFormat("\t\t\tPropertyInfo pi_{0} = t_XAML.GetProperty(\"{0}\", BindingFlags.Public | BindingFlags.Instance);{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
			else
				if (o.IsAttached == false)
					code.AppendFormat(o.DataType.Scope == DataScope.Public ? "\t\t\tPropertyInfo pi_{0} = t_XAML.GetProperty(\"{0}\", BindingFlags.Public | BindingFlags.Instance);{1}" : "\t\t\tPropertyInfo pi_{0} = t_XAML.GetProperty(\"{0}\", BindingFlags.NonPublic | BindingFlags.Instance);{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
			code.AppendFormat("\t\t\tFieldInfo fi_{0} = t_DTO.GetField(\"{0}Field\", BindingFlags.NonPublic | BindingFlags.Instance);{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
			if (o.XAMLType.TypeMode == DataTypeMode.Array)
			{
				code.AppendFormat(o.IsAttached == false ? "\t\t\tif(fi_{0} != null && pi_{0} != null){1}" : "\t\t\tif(fi_{0} != null){1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendLine("\t\t\t{");
				if (o.Owner.Parent.Owner.CollectionSerializationOverride == ProjectCollectionSerializationOverride.List)
				{
					code.AppendFormat("\t\t\t\t{0} v_{1} = new {0}();{2}", DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					code.AppendFormat("\t\t\t\tfi_{0}.SetValue(DTO, v_{0});{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					if (o.IsAttached == false) code.AppendFormat("\t\t\t\t{0} XAML_{1} = pi_{1}.GetValue(Data, null) as {0};{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					else code.AppendFormat("\t\t\t\t{0} XAML_{1} = {3}.Get{2}(Data) as {0};{4}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName, DataTypeCSGenerator.GenerateType(c.XAMLType), Environment.NewLine);
					code.AppendFormat("\t\t\t\tforeach({0} a in XAML_{1}) {{ v_{1}.Add(a); }}{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				}
				else
				{
					if (o.IsAttached == false) code.AppendFormat("\t\t\t\t{0} XAML_{1} = pi_{1}.GetValue(Data, null) as {0};{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					else code.AppendFormat("\t\t\t\t{0} XAML_{1} = {2}.Get{3}(Data) as {0};{4}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
					code.AppendFormat("\t\t\t\t{0}[] v_{1} = new {0}[XAML_{1}.GetLength(0)];{2}", DataTypeCSGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					code.AppendFormat("\t\t\t\tfor(int i = 0; i < XAML_{0}.GetLength(0); i++) {{ v_{0}[i] = Data.{0}[i]; }}{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					code.AppendFormat("\t\t\t\tfi_{0}.SetValue(DTO, v_{0});{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				}
				code.AppendLine("\t\t\t}");
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendFormat(o.IsAttached == false ? "\t\t\tif(fi_{0} != null && pi_{0} != null){1}" : "\t\t\tif(fi_{0} != null){1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendLine("\t\t\t{");
				if (o.Owner.Parent.Owner.CollectionSerializationOverride != ProjectCollectionSerializationOverride.Array)
				{
					code.AppendFormat("\t\t\t\t{0} v_{1} = new {0}();{2}", DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					code.AppendFormat("\t\t\t\tfi_{0}.SetValue(DTO, v_{0});{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					if (o.IsAttached == false) code.AppendFormat("\t\t\t\t{0} XAML_{1} = pi_{1}.GetValue(Data, null) as {0};{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					else code.AppendFormat("\t\t\t\t{0} XAML_{1} = {3}.Get{2}(Data) as {0};{4}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName, DataTypeCSGenerator.GenerateType(c.XAMLType), Environment.NewLine);
					code.AppendFormat("\t\t\t\tforeach({0} a in XAML_{1}) {{ v_{1}.Add(a); }}{2}", DataTypeCSGenerator.GenerateTypeGenerics(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				}
				else
				{
					if (o.IsAttached == false) code.AppendFormat("\t\t\t\t{0} XAML_{1} = pi_{1}.GetValue(Data, null) as {0};{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					else code.AppendFormat("\t\t\t\t{0} XAML_{1} = {2}.Get{3}(Data) as {0};{4}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
					code.AppendFormat("\t\t\t\t{0}[] v_{1} = new {0}[XAML_{1}.GetLength(0)];{2}", DataTypeCSGenerator.GenerateTypeGenerics(o.HasClientType ? o.ClientType : o.DataType).Replace("[]", ""), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					code.AppendFormat("\t\t\t\tfor(int i = 0; i < XAML_{0}.GetLength(0); i++) {{ v_{0}[i] = Data.{0}[i]; }}{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
					code.AppendFormat("\t\t\t\tfi_{0}.SetValue(DTO, v_{0});{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				}
				code.AppendLine("\t\t\t}");
			}
			else if (o.XAMLType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendFormat(o.IsAttached == false ? "\t\t\tif(fi_{0} != null && pi_{0} != null){1}" : "\t\t\tif(fi_{0} != null){1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendLine("\t\t\t{");
				code.AppendFormat("\t\t\t\t{0} v_{1} = new {0}();{2}", DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendFormat("\t\t\t\tfi_{0}.SetValue(DTO, v_{0});{1}", o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				if (o.IsAttached == false) code.AppendFormat("\t\t\t\t{0} XAML_{1} = pi_{1}.GetValue(Data, null) as {0};{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				else code.AppendFormat("\t\t\t\t{0} XAML_{1} = {3}.Get{2}(Data) as {0};{4}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName, DataTypeCSGenerator.GenerateType(c.XAMLType), Environment.NewLine);
				code.AppendFormat("\t\t\t\tforeach(KeyValuePair<{0}> a in XAML_{1}) {{ v_{1}.Add(a.Key, a.Value); }}{2}", DataTypeCSGenerator.GenerateTypeGenerics(o.XAMLType), o.HasClientType ? o.ClientName : o.DataName, Environment.NewLine);
				code.AppendLine("\t\t\t}");
			}
			else
			{
				if (o.IsAttached == false) code.AppendFormat("\t\t\tif(fi_{0} != null && pi_{0} != null) fi_{0}.SetValue(DTO, ({1})pi_{0}.GetValue(Data, null));{2}", o.HasClientType ? o.ClientName : o.DataName, DataTypeCSGenerator.GenerateType(o.HasClientType ? o.ClientType : o.DataType), Environment.NewLine);
				else code.AppendFormat("\t\t\tif(fi_{1} != null) fi_{1}.SetValue(DTO, ({2}){3}.Get{0}(Data));{4}", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName, o.DataType, DataTypeCSGenerator.GenerateType(c.XAMLType), Environment.NewLine);
			}

			return code.ToString();
		}

		private static string GenerateElementXAMLCode30(DataElement o)
		{
			if (o.IsHidden) return "";
			if (o.IsDataMember == false) return "";

			var code = new StringBuilder();
			if (o.HasXAMLType)
			{
				if (o.IsReadOnly == false && o.IsAttached == false)
				{
					code.AppendFormat("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue({1}Property); }} set {{ SetValue({1}Property, value); }} }}{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
					code.AppendFormat("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.Register(\"{1}\", typeof({0}), typeof({2}));{3}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, DataTypeCSGenerator.GenerateType(o.Owner), Environment.NewLine);
				}
				if (o.IsReadOnly && o.IsAttached == false)
				{
					code.AppendFormat("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue({1}Property); }} protected set {{ SetValue({1}PropertyKey, value); }}{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
					code.AppendFormat("\t\tprivate static readonly DependencyPropertyKey {1}PropertyKey = DependencyProperty.RegisterReadOnly(\"{1}\", typeof({0}), typeof({2}), new PropertyMetadata(null));{3}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, DataTypeCSGenerator.GenerateType(o.Owner), Environment.NewLine);
					code.AppendFormat("\t\tpublic static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;{1}", o.XAMLName, Environment.NewLine);
				}
				if (o.IsReadOnly == false && o.IsAttached)
				{
					if (o.AttachedBrowsable)
					{
						code.AppendLine(o.AttachedBrowsableIncludeDescendants ? "\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=true)]" : "\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=false)]");
					}
					if (o.AttachedTargetTypes != "")
					{
						var ttl = new List<string>(o.AttachedTargetTypes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string tt in ttl)
							code.AppendFormat("\t\t[AttachedPropertyBrowsableForType(typeof({0}))]{1}", tt.Trim(), Environment.NewLine);
					}
					if (o.AttachedAttributeTypes != "")
					{
						var ttl = new List<string>(o.AttachedAttributeTypes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string tt in ttl)
							code.AppendFormat("\t\t[AttachedPropertyBrowsableWhenAttributePresent(typeof({0}))]{1}", tt.Trim(), Environment.NewLine);
					}
					code.AppendFormat("\t\tpublic static {0} Get{1}(DependencyObject obj) {{ return ({0})obj.GetValue({1}Property); }}{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
					code.AppendFormat("\t\tpublic static void Set{1}(DependencyObject obj, {0} value) {{ obj.SetValue({1}Property, value); }}{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
					code.AppendFormat("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.RegisterAttached(\"{1}\", typeof({0}), typeof({2}), new UIPropertyMetadata(null));{3}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, DataTypeCSGenerator.GenerateType(o.Owner), Environment.NewLine);
				}
				if (o.IsReadOnly && o.IsAttached)
				{
					if (o.AttachedBrowsable)
					{
						code.AppendLine(o.AttachedBrowsableIncludeDescendants ? "\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=true)]" : "\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=false)]");
					}
					if (o.AttachedTargetTypes != "")
					{
						var ttl = new List<string>(o.AttachedTargetTypes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string tt in ttl)
							code.AppendFormat("\t\t[AttachedPropertyBrowsableForType(typeof({0}))]{1}", tt.Trim(), Environment.NewLine);
					}
					if (o.AttachedAttributeTypes != "")
					{
						var ttl = new List<string>(o.AttachedAttributeTypes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string tt in ttl)
							code.AppendFormat("\t\t[AttachedPropertyBrowsableWhenAttributePresent(typeof({0}))]{1}", tt.Trim(), Environment.NewLine);
					}
					code.AppendFormat("\t\tpublic static {0} Get{1}(DependencyObject obj) {{ return ({0})obj.GetValue({1}Property); }}{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
					code.AppendFormat("\t\tprotected static void Set{1}(DependencyObject obj, {0} value) {{ obj.SetValue({1}PropertyKey, value); }}{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
					code.AppendFormat("\t\tprivate static readonly DependencyPropertyKey {1}PropertyKey = DependencyProperty.RegisterAttachedReadOnly(\"{1}\", typeof({0}), typeof({2}), new PropertyMetadata(null));{3}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, DataTypeCSGenerator.GenerateType(o.Owner), Environment.NewLine);
					code.AppendFormat("\t\tpublic static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;{1}", o.XAMLName, Environment.NewLine);
				}
			}
			else
			{
				if (o.IsReadOnly == false)
				{
					code.AppendFormat("\t\tprivate {0} m_{1};{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
					code.AppendFormat("\t\t{0} {1} {2} {{ get {{ return m_{2}; }} set {{ m_{2} = value; }} }}{3}", DataTypeCSGenerator.GenerateScope(o.DataType.Scope), DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
				}
				else
				{
					code.AppendFormat("\t\tprivate {0} m_{1};{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
					code.AppendFormat("\t\t{0} {1} {2} {{ get {{ return m_{2}; }} protected set {{ m_{2} = value; }} }}{3}", DataTypeCSGenerator.GenerateScope(o.DataType.Scope), DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
				}
			}
			return code.ToString();
		}

		private static string GenerateElementXAMLCode35(DataElement o)
		{
			return GenerateElementXAMLCode40(o);
		}

		private static string GenerateElementXAMLCode40(DataElement o)
		{
			return GenerateElementXAMLCode45(o);
		}

		private static string GenerateElementXAMLCode45(DataElement o)
		{
			if (o.IsHidden) return "";
			if (o.IsDataMember == false) return "";

			var code = new StringBuilder();
			if (o.HasXAMLType)
			{
				if (o.IsReadOnly == false && o.IsAttached == false)
				{
					code.AppendFormat("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue({1}Property); }} set {{ SetValue({1}Property, value); }} }}{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
					code.AppendFormat("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.Register(\"{1}\", typeof({0}), typeof({2}));{3}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, DataTypeCSGenerator.GenerateType(o.Owner), Environment.NewLine);
				}
				if (o.IsReadOnly && o.IsAttached == false)
				{
					code.AppendFormat("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue({1}Property); }} protected set {{ SetValue({1}PropertyKey, value); }} }}{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
					code.AppendFormat("\t\tprivate static readonly DependencyPropertyKey {1}PropertyKey = DependencyProperty.RegisterReadOnly(\"{1}\", typeof({0}), typeof({2}), new PropertyMetadata(null));{3}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, DataTypeCSGenerator.GenerateType(o.Owner), Environment.NewLine);
					code.AppendFormat("\t\tpublic static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;{1}", o.XAMLName, Environment.NewLine);
				}
				if (o.IsReadOnly == false && o.IsAttached)
				{
					if (o.AttachedBrowsable)
						code.AppendLine(o.AttachedBrowsableIncludeDescendants ? "\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=true)]" : "\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=false)]");
					if (o.AttachedTargetTypes != "")
					{
						var ttl = new List<string>(o.AttachedTargetTypes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string tt in ttl)
							code.AppendFormat("\t\t[AttachedPropertyBrowsableForType(typeof({0}))]{1}", tt.Trim(), Environment.NewLine);
					}
					if (o.AttachedAttributeTypes != "")
					{
						var ttl = new List<string>(o.AttachedAttributeTypes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string tt in ttl)
							code.AppendFormat("\t\t[AttachedPropertyBrowsableWhenAttributePresent(typeof({0}))]{1}", tt.Trim(), Environment.NewLine);
					}
					code.AppendFormat("\t\tpublic static {0} Get{1}(DependencyObject obj) {{ return ({0})obj.GetValue({1}Property); }}{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
					code.AppendFormat("\t\tpublic static void Set{1}(DependencyObject obj, {0} value) {{ obj.SetValue({1}Property, value); }}{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
					code.AppendFormat("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.RegisterAttached(\"{1}\", typeof({0}), typeof({2}), new UIPropertyMetadata(null));{3}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, DataTypeCSGenerator.GenerateType(o.Owner), Environment.NewLine);
				}
				if (o.IsReadOnly && o.IsAttached)
				{
					if (o.AttachedBrowsable)
						code.AppendLine(o.AttachedBrowsableIncludeDescendants ? "\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=true)]" : "\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=false)]");
					if (o.AttachedTargetTypes != "")
					{
						var ttl = new List<string>(o.AttachedTargetTypes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string tt in ttl)
							code.AppendFormat("\t\t[AttachedPropertyBrowsableForType(typeof({0}))]{1}", tt.Trim(), Environment.NewLine);
					}
					if (o.AttachedAttributeTypes != "")
					{
						var ttl = new List<string>(o.AttachedAttributeTypes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string tt in ttl)
							code.AppendFormat("\t\t[AttachedPropertyBrowsableWhenAttributePresent(typeof({0}))]{1}", tt.Trim(), Environment.NewLine);
					}
					code.AppendFormat("\t\tpublic static {0} Get{1}(DependencyObject obj) {{ return ({0})obj.GetValue({1}Property); }}{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
					code.AppendFormat("\t\tprotected static void Set{1}(DependencyObject obj, {0} value) {{ obj.SetValue({1}PropertyKey, value); }}{2}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
					code.AppendFormat("\t\tprivate static readonly DependencyPropertyKey {1}PropertyKey = DependencyProperty.RegisterAttachedReadOnly(\"{1}\", typeof({0}), typeof({2}), new PropertyMetadata(null));{3}", DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, DataTypeCSGenerator.GenerateType(o.Owner), Environment.NewLine);
					code.AppendFormat("\t\tpublic static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;{1}", o.XAMLName, Environment.NewLine);
				}
			}
			else
			{
				code.AppendFormat(o.IsReadOnly == false ? "\t\t{0} {1} {2} {{ get; set; }}{3}" : "\t\t{0} {1} {2} {{ get; protected set; }}{3}", DataTypeCSGenerator.GenerateScope(o.DataType.Scope), DataTypeCSGenerator.GenerateType(o.XAMLType), o.XAMLName, Environment.NewLine);
			}
			return code.ToString();
		}
	}
}