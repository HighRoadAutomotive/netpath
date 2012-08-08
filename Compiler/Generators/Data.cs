﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFArchitect.Projects;

namespace WCFArchitect.Compiler.Generators
{
	internal static class DataCSGenerator
	{
		public static bool VerifyCode(Data o)
		{
			bool NoErrors = true;

			if (o.Name == "" || o.Name == null)
			{
				Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3000", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.Name) == false)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3001", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Contract Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
					NoErrors = false;
				}

			if (o.HasContractType == true)
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.ContractType.Name) == false)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3002", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Contract Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
					NoErrors = false;
				}

			if (o.HasWPFType == true)
				if (Helpers.RegExs.MatchCodeName.IsMatch(o.WPFType.Name) == false)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3003", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Contract Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
					NoErrors = false;
				}

			foreach (Projects.DataElement D in o.Elements)
			{
				if (Helpers.RegExs.MatchCodeName.IsMatch(D.DataType.Name) == false)
				{
					Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3005", "The data element '" + D.DataType.Name + "' in the '" + o.Name + "' data object contains invalid characters in the Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o, D, D.GetType()));
					NoErrors = false;
				}

				if (D.HasContractType == true)
					if (Helpers.RegExs.MatchCodeName.IsMatch(D.ContractType.Name) == false)
					{
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3002", "The data object '" + D.ContractType.Name + "' in the '" + o.Name + "' namespace contains invalid characters in the Contract Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
						NoErrors = false;
					}

				if (D.HasWPFType == true)
					if (Helpers.RegExs.MatchCodeName.IsMatch(D.WPFType.Name) == false)
					{
						Compiler.Program.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS3003", "The data object '" + D.WPFType.Name + "' in the '" + o.Name + "' namespace contains invalid characters in the WPF Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, o.Parent, o, o.GetType()));
						NoErrors = false;
					}
			}

			return NoErrors;
		}

		public static string GenerateServerCode30(Data o)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.Append("\t[DataContract(");
			if (o.HasContractType == true) Code.AppendFormat("Name = \"{0}\", ", o.HasContractType == true ? o.ContractType.Name : o.Name);
			Code.AppendFormat("Namespace = \"{0}\"", o.Parent.URI);
			Code.AppendLine(")]");
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			foreach (DataElement DE in o.Elements)
				Code.Append(GenerateElementServerCode30(DE));
			Code.AppendLine("\t}");
			return Code.ToString();
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
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.Append("\t[DataContract(");
			if (o.IsReference == true) Code.AppendFormat("IsReference = true, ");
			if (o.HasContractType == true) Code.AppendFormat("Name = \"{0}\", ", o.HasContractType == true ? o.ContractType.Name : o.Name);
			Code.AppendFormat("Namespace = \"{0}\"", o.Parent.URI);
			Code.AppendLine(")]");
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			foreach (DataElement DE in o.Elements)
				Code.Append(GenerateElementServerCode45(DE));
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public static string GenerateProxyCode30(Data o)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendLine("[System.Diagnostics.DebuggerStepThroughAttribute()]");
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.Append("\t[DataContract(");
			if (o.HasContractType == true) Code.AppendFormat("Name = \"{0}\", ", o.HasContractType == true ? o.ContractType.Name : o.Name);
			Code.AppendFormat("Namespace = \"{0}\"", o.Parent.URI);
			Code.AppendLine(")]");
			Code.AppendFormat("\t{0} : System.Runtime.Serialization.IExtensibleDataObject{1}", o.HasContractType == true ? DataTypeCSGenerator.GenerateTypeDeclaration(o.ContractType) : DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendLine("\t\tprivate System.Runtime.Serialization.ExtensionDataObject extensionDataField;");
			Code.AppendLine("\t\tpublic System.Runtime.Serialization.ExtensionDataObject ExtensionData { get { return extensionDataField; } set { extensionDataField = value; } }");
			foreach (DataElement DE in o.Elements)
				Code.Append(GenerateElementServerCode30(DE));
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public static string GenerateProxyCode35(Data o)
		{
			return GenerateProxyCode40(o);
		}

		public static string GenerateProxyCode40(Data o)
		{
			return GenerateProxyCode45(o);
		}

		public static string GenerateProxyCode45(Data o)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendLine("[System.Diagnostics.DebuggerStepThroughAttribute()]");
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.Append("\t[DataContract(");
			if (o.IsReference == true) Code.AppendFormat("IsReference = true, ");
			if (o.HasContractType == true) Code.AppendFormat("Name = \"{0}\", ", o.HasContractType == true ? o.ContractType.Name : o.Name);
			Code.AppendFormat("Namespace = \"{0}\"", o.Parent.URI);
			Code.AppendLine(")]");
			Code.AppendFormat("\t{0} : System.Runtime.Serialization.IExtensibleDataObject{1}", o.HasContractType == true ? DataTypeCSGenerator.GenerateTypeDeclaration(o.ContractType) : DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendLine("\t\tpublic System.Runtime.Serialization.ExtensionDataObject ExtensionData { get; set; }");
			foreach (DataElement DE in o.Elements)
				Code.Append(GenerateElementServerCode45(DE));
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public static string GenerateWPFCode30(Data o)
		{
			if (o.HasWPFType == false) return "";
			bool HasWPF = false;
			foreach (DataElement DE in o.Elements)
				if (DE.HasWPFType == true)
				{
					HasWPF = true;
					break;
				}
			if (HasWPF == false) return "";

			//This is a shim to ensure there is ALWAYS a WPF name.
			if (o.WPFType.Name == "" || o.WPFType.Name == null) o.WPFType.Name = o.Name + "WPF";

			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t//WPF Integration Object for the {0} DTO{1}", o.HasContractType ? o.ContractType.Name : o.Name, Environment.NewLine);
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");

			Code.AppendLine("\t\t//Properties");
			foreach (DataElement DE in o.Elements)
				Code.Append(GenerateElementWPFCode30(DE));
			Code.AppendLine();

			Code.AppendLine("\t\t//Implicit Conversion");
			Code.AppendFormat("\t\tpublic static implicit operator {0}({1} Data){2}", o.HasContractType ? DataTypeCSGenerator.GenerateType(o.ContractType) : DataTypeCSGenerator.GenerateType(o), DataTypeCSGenerator.GenerateType(o.WPFType), Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tif (Data == null) return null;");
			Code.AppendLine("\t\t\tif(Application.Current.Dispatcher.CheckAccess() == true)");
			Code.AppendLine("\t\t\t{");
			Code.AppendFormat("\t\t\t\treturn {0}.ConvertFromWPFObject(Data);{1}", o.WPFType.Name, Environment.NewLine);
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t\telse");
			Code.AppendLine("\t\t\t{");
			Code.AppendFormat("\t\t\t\treturn ({0})Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Windows.Threading.DispatcherOperationCallback(delegate(object ignore){{ return {1}.ConvertFromWPFObject(Data); }}), null);{2}", o.HasContractType ? o.ContractType.Name : o.Name, DataTypeCSGenerator.GenerateType(o.WPFType), Environment.NewLine);
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic static implicit operator {0}({1} Data){2}", o.WPFType.Name, DataTypeCSGenerator.GenerateType(o.HasContractType ? o.ContractType : o), Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tif (Data == null) return null;");
			Code.AppendLine("\t\t\tif(Application.Current.Dispatcher.CheckAccess() == true)");
			Code.AppendLine("\t\t\t{");
			Code.AppendFormat("\t\t\t\treturn {0}.ConvertToWPFObject(Data);{1}", o.WPFType.Name, Environment.NewLine);
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t\telse");
			Code.AppendLine("\t\t\t{");
			Code.AppendFormat("\t\t\t\treturn ({0})Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Windows.Threading.DispatcherOperationCallback(delegate(object ignore){{ return {0}.ConvertToWPFObject(Data); }}), null);{1}", DataTypeCSGenerator.GenerateType(o.WPFType), Environment.NewLine);
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");
			Code.AppendLine();

			Code.AppendLine("\t\t//Constructors");
			Code.AppendFormat("\t\tpublic {0}({1} Data){3}", o.WPFType.Name, DataTypeCSGenerator.GenerateType(o.HasContractType ? o.ContractType : o), Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tType t_DT = Data.GetType();");
			foreach (DataElement DE in o.Elements)
				Code.Append(GenerateElementWPFConstructorCode30(DE, o));
			Code.AppendLine("\t\t}");
			Code.AppendLine();
			Code.AppendFormat("\t\tpublic {0}(){1}", o.WPFType.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t}");
			Code.AppendLine();

			Code.AppendLine("\t\t//WPF/DTO Conversion Functions");
			Code.AppendFormat("\t\tpublic static {0} ConvertFromWPFObject({1} Data){2}", o.HasContractType ? o.ContractType.Name : o.Name, DataTypeCSGenerator.GenerateType(o.WPFType), Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\t{0} DTO = new {0}();{1}", DataTypeCSGenerator.GenerateType(o.HasContractType ? o.ContractType : o), Environment.NewLine);
			Code.AppendLine("\t\t\tType t_WPF = Data.GetType();");
			Code.AppendLine("\t\t\tType t_DTO = DTO.GetType();");
			foreach (DataElement DE in o.Elements)
				Code.Append(GenerateElementWPFConversionFromWPF30(DE, o));
			Code.AppendLine("\t\t\treturn DTO;");
			Code.AppendLine("\t\t}");
			Code.AppendLine();
			Code.AppendFormat("\t\tpublic static {0} ConvertToWPFObject({1} Data){2}", o.WPFType.Name, DataTypeCSGenerator.GenerateType(o.HasContractType ? o.ContractType : o), Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\t{0} WPF = new {0}();{1}", DataTypeCSGenerator.GenerateType(o.WPFType), Environment.NewLine);
			Code.AppendLine("\t\t\tType t_DTO = Data.GetType();");
			Code.AppendLine("\t\t\tType t_WPF = WPF.GetType();");
			foreach (DataElement DE in o.Elements)
				Code.Append(GenerateElementWPFConversionToWPF30(DE, o));
			Code.AppendLine("\t\t\treturn WPF;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");

			return Code.ToString();
		}

		public static string GenerateWPFCode35(Data o)
		{
			return GenerateWPFCode40(o);
		}

		public static string GenerateWPFCode40(Data o)
		{
			return GenerateWPFCode45(o);
		}

		public static string GenerateWPFCode45(Data o)
		{
			if (o.HasWPFType == false) return "";
			bool HasWPF = false;
			foreach (DataElement DE in o.Elements)
				if (DE.HasWPFType == true)
				{
					HasWPF = true;
					break;
				}
			if (HasWPF == false) return "";

			//This is a shim to ensure there is ALWAYS a WPF name.
			if (o.WPFType.Name == "" || o.WPFType.Name == null) o.WPFType.Name = o.Name + "WPF";

			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t//WPF Integration Object for the {0} DTO{1}", o.HasContractType ? o.ContractType.Name : o.Name, Environment.NewLine);
			Code.AppendFormat("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]{2}", Globals.ApplicationTitle, Globals.ApplicationVersion.ToString(), Environment.NewLine);
			Code.AppendFormat("\t{0}{1}", DataTypeCSGenerator.GenerateTypeDeclaration(o), Environment.NewLine);
			Code.AppendLine("\t{");

			Code.AppendLine("\t\t//Properties");
			foreach (DataElement DE in o.Elements)
				Code.Append(GenerateElementWPFCode45(DE));
			Code.AppendLine();

			Code.AppendLine("\t\t//Implicit Conversion");
			Code.AppendFormat("\t\tpublic static implicit operator {0}({1} Data){2}", o.HasContractType ? DataTypeCSGenerator.GenerateType(o.ContractType) : DataTypeCSGenerator.GenerateType(o), DataTypeCSGenerator.GenerateType(o.WPFType), Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tif (Data == null) return null;");
			Code.AppendLine("\t\t\tif(Application.Current.Dispatcher.CheckAccess() == true)");
			Code.AppendLine("\t\t\t{");
			Code.AppendFormat("\t\t\t\treturn {0}.ConvertFromWPFObject(Data);{1}", o.WPFType.Name, Environment.NewLine);
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t\telse");
			Code.AppendLine("\t\t\t{");
			Code.AppendFormat("\t\t\t\treturn ({0})Application.Current.Dispatcher.Invoke(new Func<{0}>(() => {1}.ConvertFromWPFObject(Data)), System.Windows.Threading.DispatcherPriority.Normal);{2}", DataTypeCSGenerator.GenerateType(o.HasContractType ? o.ContractType : o), DataTypeCSGenerator.GenerateType(o.WPFType), Environment.NewLine);
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic static implicit operator {1}({0} Data){2}", o.WPFType.Name, DataTypeCSGenerator.GenerateType(o.HasContractType ? o.ContractType : o), Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tif (Data == null) return null;");
			Code.AppendLine("\t\t\tif(Application.Current.Dispatcher.CheckAccess() == true)");
			Code.AppendLine("\t\t\t{");
			Code.AppendFormat("\t\t\t\treturn {0}.ConvertToWPFObject(Data);{1}", o.WPFType.Name, Environment.NewLine);
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t\telse");
			Code.AppendLine("\t\t\t{");
			Code.AppendFormat("\t\t\t\treturn ({0})Application.Current.Dispatcher.Invoke(new Func<{1}>(() => {0}.ConvertToWPFObject(Data)), System.Windows.Threading.DispatcherPriority.Normal);{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.Name, Environment.NewLine);
			Code.AppendLine("\t\t\t}");
			Code.AppendLine("\t\t}");
			Code.AppendLine();

			Code.AppendLine("\t\t//Constructors");
			Code.AppendFormat("\t\tpublic {0}({1} Data){2}", o.WPFType.Name, DataTypeCSGenerator.GenerateType(o.HasContractType ? o.ContractType : o), Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tType t_DT = Data.GetType();");
			foreach (DataElement DE in o.Elements)
				Code.Append(GenerateElementWPFConstructorCode45(DE, o));
			Code.AppendLine("\t\t}");
			Code.AppendLine();
			Code.AppendFormat("\t\tpublic {0}(){1}", o.WPFType.Name, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t}");
			Code.AppendLine();

			Code.AppendLine("\t\t//WPF/DTO Conversion Functions");
			Code.AppendFormat("\t\tpublic static {0} ConvertFromWPFObject({1} Data){2}", o.HasContractType ? o.ContractType.Name : o.Name, DataTypeCSGenerator.GenerateType(o.WPFType), Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\t{0} DTO = new {0}();{1}", DataTypeCSGenerator.GenerateType(o.HasContractType ? o.ContractType : o), Environment.NewLine);
			Code.AppendLine("\t\t\tType t_WPF = Data.GetType();");
			Code.AppendLine("\t\t\tType t_DTO = DTO.GetType();");
			foreach (DataElement DE in o.Elements)
				Code.Append(GenerateElementWPFConversionFromWPF45(DE, o));
			Code.AppendLine("\t\t\treturn DTO;");
			Code.AppendLine("\t\t}");
			Code.AppendLine();
			Code.AppendFormat("\t\tpublic static {0} ConvertToWPFObject({1} Data){2}", o.WPFType.Name, DataTypeCSGenerator.GenerateType(o.HasContractType ? o.ContractType : o), Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\t{0} WPF = new {0}();{1}", DataTypeCSGenerator.GenerateType(o.WPFType), Environment.NewLine);
			Code.AppendLine("\t\t\tType t_DTO = Data.GetType();");
			Code.AppendLine("\t\t\tType t_WPF = WPF.GetType();");
			foreach (DataElement DE in o.Elements)
				Code.Append(GenerateElementWPFConversionToWPF45(DE, o));
			Code.AppendLine("\t\t\treturn WPF;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");

			return Code.ToString();
		}

		private static string GenerateElementServerCode30(DataElement o)
		{
			if (o.IsHidden == true) return "";
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t\tprivate {0} m_{1};{2}", DataTypeCSGenerator.GenerateType(o.DataType), o.DataType.Name, Environment.NewLine);
			if (o.IsDataMember == true)
			{
				Code.Append("\t\t[DataMember(");
				if (o.EmitDefaultValue == true)
					Code.AppendFormat("EmitDefaultValue = false, ");
				if (o.IsRequired == true)
					Code.AppendFormat("IsRequired = true, ");
				if (o.Order >= 0)
					Code.AppendFormat("Order = {0}, ", o.Order);
				Code.AppendFormat("Name = \"{0}\")] ", o.HasContractType ? o.ContractType.Name : o.DataType.Name);
			}
			else
			{
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.DataContract) Code.Append("\t\t");
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.XML) Code.Append("\t\t[XmlIgnore()]");
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.Auto) Code.Append("\t\t");
			}
			if (o.IsReadOnly == false) Code.AppendFormat("{0} {1} {2} {{ get {{ return m_{2}; }} set {{ m_{2} = value; }} }}{3}", DataTypeCSGenerator.GenerateScope(o.DataType.Scope, o.Owner.Parent.Owner.GetType()), DataTypeCSGenerator.GenerateType(o.DataType), o.DataType.Name, Environment.NewLine);
			else Code.AppendFormat("{0} {1} {2} {{ get {{ return m_{2}; }} protected set {{ m_{2} = value; }} }}{3}", DataTypeCSGenerator.GenerateScope(o.DataType.Scope, o.Owner.Parent.Owner.GetType()), o.DataType, o.DataType.Name, Environment.NewLine);
			return Code.ToString();
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
			if (o.IsHidden == true) return "";
			StringBuilder Code = new StringBuilder();
			if (o.IsDataMember == true)
			{
				Code.Append("\t\t[DataMember(");
				if (o.EmitDefaultValue == true)
					Code.AppendFormat("EmitDefaultValue = false, ");
				if (o.IsRequired == true)
					Code.AppendFormat("IsRequired = true, ");
				if (o.Order >= 0)
					Code.AppendFormat("Order = {0}, ", o.Order);
				Code.AppendFormat("Name = \"{0}\")] ", o.HasContractType ? o.ContractType.Name : o.DataType.Name);
			}
			else
			{
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.DataContract) Code.Append("\t\t[IgnoreDataMember()]");
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.XML) Code.Append("\t\t[XmlIgnore()]");
				if (o.Owner.Parent.Owner.ServiceSerializer == ProjectServiceSerializerType.Auto) Code.Append("\t\t[IgnoreDataMember()]");
			}
			if (o.IsReadOnly == false) Code.AppendFormat("{0} {1} {2} {{ get; set; }}{3}", DataTypeCSGenerator.GenerateScope(o.DataType.Scope, o.Owner.Parent.Owner.GetType()), DataTypeCSGenerator.GenerateType(o.DataType), o.DataType.Name, Environment.NewLine);
			else Code.AppendFormat("{0} {1} {2} {{ get; protected set; }}{3}", DataTypeCSGenerator.GenerateScope(o.DataType.Scope, o.Owner.Parent.Owner.GetType()), DataTypeCSGenerator.GenerateType(o.DataType), o.DataType.Name, Environment.NewLine);
			return Code.ToString();
		}

		private static string GenerateElementWPFConstructorCode30(DataElement o, Data c)
		{
			return GenerateElementWPFConstructorCode35(o, c);
		}

		private static string GenerateElementWPFConstructorCode35(DataElement o, Data c)
		{
			return GenerateElementWPFConstructorCode40(o, c);
		}

		private static string GenerateElementWPFConstructorCode40(DataElement o, Data c)
		{
			return GenerateElementWPFConstructorCode45(o, c);
		}

		private static string GenerateElementWPFConstructorCode45(DataElement o, Data c)
		{
			if (o.IsHidden == true) return "";
			if (o.IsDataMember == false) return "";
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("\t\t\tFieldInfo fi_{0} = t_DT.GetField(\"{0}Field\", BindingFlags.NonPublic | BindingFlags.Instance);{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tif(fi_{0} != null){1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
			Code.AppendLine("\t\t\t{");
			if (o.WPFType.TypeMode == DataTypeMode.Array)
			{
				Code.AppendLine("\t\t\t{");
				Code.AppendFormat("\t\t\t\t{0} v_{1} = fi_{1}.GetValue(Data) as {0};{2}", DataTypeCSGenerator.GenerateType(o.DataType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendFormat("\t\t\t\tif(v_{0} != null){1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendLine("\t\t\t\t{");
				if (o.Owner.Parent.Owner.CollectionSerializationOverride == ProjectCollectionSerializationOverride.List)
				{
					Code.AppendFormat("\t\t\t\t\t{0} tv = new {1}[v_{2}.Count];{3}", DataTypeCSGenerator.GenerateType(o.WPFType), DataTypeCSGenerator.GenerateType(o.WPFType).Replace("[]", ""), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\t\tfor(int i = 0; i < v_{0}.Count; i++) {{ try {{ tv[i] = v_{0}[i]; }} catch {{ continue; }} }}{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					if (o.IsAttached == true) Code.AppendFormat("\t\t\t\t\t{0}.Set{1}(this, tv);{2}", c.HasContractType ? c.ContractType.Name : c.Name, DataTypeCSGenerator.GenerateType(o.WPFType), Environment.NewLine);
				}
				else
				{
					Code.AppendFormat("\t\t\t\t\t{0} tv = new {1}[v_{2}.GetLength(0)];{3}", DataTypeCSGenerator.GenerateType(o.WPFType), DataTypeCSGenerator.GenerateType(o.WPFType).Replace("[]", ""), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\t\tfor(int i = 0; i < v_{1}.GetLength(0); i++) {{ try {{ tv[i] = v_{1}[i]; }} catch {{ continue; }} }}{2}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					if (o.IsAttached == true) Code.AppendFormat("\t\t\t\t\t{0}.Set{1}(this, tv);{2}", c.HasContractType ? c.ContractType.Name : c.Name, DataTypeCSGenerator.GenerateType(o.WPFType), Environment.NewLine);
				}
				Code.AppendLine("\t\t\t\t}");
				Code.AppendLine("\t\t\t}");
			}
			else if (o.WPFType.TypeMode == DataTypeMode.Collection)
			{
				Code.AppendLine("\t\t\t{");
				Code.AppendFormat("\t\t\t\t{0} v_{1} = fi_{1}.GetValue(Data) as {0};{2}", DataTypeCSGenerator.GenerateType(o.DataType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendFormat("\t\t\t\tif(v_{0} != null){1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendLine("\t\t\t\t{");
				Code.AppendFormat("\t\t\t\t\t{0} tv = new {0}();{1}", DataTypeCSGenerator.GenerateType(o.WPFType), Environment.NewLine);
				Code.AppendFormat("\t\t\t\t\tforeach({0} a in v_{1}) {{ tv.Add(a); }}{2}", DataTypeCSGenerator.GenerateTypeGenerics(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				if (o.IsAttached == true) Code.AppendFormat("\t\t\t\t\t{0}.Set{1}(this, tv);{2}", c.HasContractType ? c.ContractType.Name : c.Name, DataTypeCSGenerator.GenerateType(o.WPFType), Environment.NewLine);
				Code.AppendLine("\t\t\t\t}");
				Code.AppendLine("\t\t\t}");
			}
			else if (o.WPFType.TypeMode == DataTypeMode.Dictionary)
			{
				Code.AppendLine("\t\t\t{");
				Code.AppendFormat("\t\t\t\t{0} tv = new {0}();{1}", DataTypeCSGenerator.GenerateType(o.DataType), Environment.NewLine);
				Code.AppendFormat("\t\t\t\tDictionary{1} v_{0} = fi_{0}.GetValue(Data) as Dictionary{1};{2}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, DataTypeCSGenerator.GenerateTypeGenerics(o.WPFType), Environment.NewLine);
				Code.AppendFormat("\t\t\t\tif(v_{0} != null){1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendFormat("\t\t\t\t\tforeach(KeyValuePair{0} a in v_{1}) {{ tv.Add(a.Key, a.Value); }}{2}", DataTypeCSGenerator.GenerateTypeGenerics(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				if (o.IsAttached == true) Code.AppendFormat("\t\t\t\t\t{0}.Set{1}(this, tv);{2}", c.HasContractType ? c.ContractType.Name : c.Name, DataTypeCSGenerator.GenerateType(o.WPFType), Environment.NewLine);
				Code.AppendLine("\t\t\t}");
			}
			else
			{
				if (o.IsAttached == false) Code.AppendFormat("\t\t\t\t{2} = ({0})fi_{1}.GetValue(Data){3}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, o.WPFType.Name, Environment.NewLine);
				else Code.AppendFormat("\t\t\t\t{3}.Set{2}(this, (({0})fi_{1}.GetValue(Data));{4}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, o.WPFType.Name, DataTypeCSGenerator.GenerateType(o.WPFType), Environment.NewLine);
			}
			Code.AppendLine("\t\t\t}");

			return Code.ToString();
		}

		private static string GenerateElementWPFConversionToWPF30(DataElement o, Data c)
		{
			return GenerateElementWPFConversionToWPF35(o, c);
		}

		private static string GenerateElementWPFConversionToWPF35(DataElement o, Data c)
		{
			return GenerateElementWPFConversionToWPF40(o, c);
		}

		private static string GenerateElementWPFConversionToWPF40(DataElement o, Data c)
		{
			return GenerateElementWPFConversionToWPF45(o, c);
		}

		private static string GenerateElementWPFConversionToWPF45(DataElement o, Data c)
		{
			if (o.IsHidden == true) return "";
			if (o.IsDataMember == false) return "";
			StringBuilder Code = new StringBuilder();

			if (o.HasWPFType == true)
			{
				if (o.IsAttached == false)
					Code.AppendFormat("\t\t\tPropertyInfo pi_{0} = t_WPF.GetProperty(\"{0}\", BindingFlags.Public | BindingFlags.Instance);{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
			}
			else
			{
				if (o.IsAttached == false)
					if (o.DataType.Scope == DataScope.Public)
						Code.AppendFormat("\t\t\tPropertyInfo pi_{0} = t_WPF.GetProperty(\"{0}\", BindingFlags.Public | BindingFlags.Instance);{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					else
						Code.AppendFormat("\t\t\tPropertyInfo pi_{0} = t_WPF.GetProperty(\"{0}\", BindingFlags.NonPublic | BindingFlags.Instance);{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
			}
			Code.AppendFormat("\t\t\tFieldInfo fi_{0} = t_DTO.GetField(\"{0}Field\", BindingFlags.NonPublic | BindingFlags.Instance);{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
			if (o.IsAttached == false) Code.AppendFormat("\t\t\tif(fi_{0} != null && pi_{0} != null){1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
			else Code.AppendFormat("\t\t\tif(fi_{0} != null){1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
			if (o.WPFType.TypeMode == DataTypeMode.Array)
			{
				Code.AppendLine("\t\t\t{");
				Code.AppendFormat("\t\t\t\t{0} v_{1} = fi_{1}.GetValue(Data) as {0};{2}", DataTypeCSGenerator.GenerateType(o.DataType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendFormat("\t\t\t\tif(v_{0} != null){1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendLine("\t\t\t\t{");
				if (o.Owner.Parent.Owner.CollectionSerializationOverride == ProjectCollectionSerializationOverride.List)
				{
					Code.AppendFormat("\t\t\t\t\t{0} wpf_{1} = new {0}[v_{1}.Count];{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					if (o.IsAttached == false) Code.AppendFormat("\t\t\t\t\tpi_{0}.SetValue(WPF, wpf_{0}, null);{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					else Code.AppendFormat("\t\t\t\t\t{0}.Set{1}(WPF, wpf_{2});{3}", DataTypeCSGenerator.GenerateType(c.WPFType), o.WPFType.Name, o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\t\tfor(int i = 0; i < v_{0}.Count; i++) {{ wpf_{0}[i] = v_{0}[i]; }}{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				}
				else
				{
					Code.AppendFormat("\t\t\t\t\t{1}[] wpf_{0} = new {1}[v_{2}.GetLength(0)];{3}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					if (o.IsAttached == false) Code.AppendFormat("\t\t\t\t\tpi_{0}.SetValue(WPF, wpf_{0}, null);{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					else Code.AppendFormat("\t\t\t\t\t{0}.Set{1}(WPF, wpf_{2});{3}", DataTypeCSGenerator.GenerateType(c.WPFType), o.WPFType.Name, o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\t\tfor(int i = 0; i < v_{0}.GetLength(0); i++) {{ wpf_{0}[i] = v_{0}[i]; }}{2}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				}
				Code.AppendLine("\t\t\t\t}");
				Code.AppendLine("\t\t\t}");
			}
			else if (o.WPFType.TypeMode == DataTypeMode.Collection)
			{
				Code.AppendLine("\t\t\t{");
				Code.AppendFormat("\t\t\t\t{0} v_{1} = fi_{1}.GetValue(Data) as {0};{2}", DataTypeCSGenerator.GenerateType(o.DataType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendFormat("\t\t\t\tif(v_{0} != null){1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendLine("\t\t\t\t{");
				Code.AppendFormat("\t\t\t\t\t{0} wpf_{1} = new {0}();{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				if (o.IsAttached == false) Code.AppendFormat("\t\t\t\t\tpi_{0}.SetValue(WPF, wpf_{0}, null);{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				else Code.AppendFormat("\t\t\t\t\t{0}.Set{1}(WPF, wpf_{2});{3}", DataTypeCSGenerator.GenerateType(c.WPFType), o.WPFType.Name, o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendFormat("\t\t\t\t\tforeach({1} a in v_{2}) {{ wpf_{0}.Add(a); }}{3}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, DataTypeCSGenerator.GenerateType(o.HasContractType ? o.ContractType : o.DataType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendLine("\t\t\t\t}");
				Code.AppendLine("\t\t\t}");
			}
			else if (o.WPFType.TypeMode == DataTypeMode.Dictionary)
			{
				Code.AppendLine("\t\t\t{");
				Code.AppendFormat("\t\t\t\t{0} wpf_{1} = new {0}();{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				if (o.IsAttached == false) Code.AppendFormat("\t\t\t\t\tpi_{0}.SetValue(WPF, wpf_{0}, null);{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				else Code.AppendFormat("\t\t\t\t\t{0}.Set{1}(WPF, wpf_{2});{3}", DataTypeCSGenerator.GenerateType(c.WPFType), o.WPFType.Name, o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendFormat("\t\t\t\t{1} v_{0} = fi_{0}.GetValue(Data) as {1};{2}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, DataTypeCSGenerator.GenerateType(o.DataType), Environment.NewLine);
				Code.AppendFormat("\t\t\t\tif(v_{2} != null) foreach(KeyValuePair{1} a in v_{2}) {{ wpf_{0}.Add(a.Key, a.Value); }}{3}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, DataTypeCSGenerator.GenerateTypeGenerics(o.HasContractType ? o.ContractType : o.DataType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendLine("\t\t\t}");
			}
			else
			{
				if (o.IsAttached == false) Code.AppendFormat("\t\t\t\tpi_{1}.SetValue(WPF, ({0})fi_{1}.GetValue(Data), null);{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				else Code.AppendFormat("\t\t\t\t{4}.Set{2}(WPF, ({0})fi_{1}.GetValue(Data));{4}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, o.WPFType.Name, DataTypeCSGenerator.GenerateType(c.WPFType), Environment.NewLine);
			}

			return Code.ToString();
		}

		private static string GenerateElementWPFConversionFromWPF30(DataElement o, Data c)
		{
			return GenerateElementWPFConversionFromWPF35(o, c);
		}

		private static string GenerateElementWPFConversionFromWPF35(DataElement o, Data c)
		{
			return GenerateElementWPFConversionFromWPF40(o, c);
		}

		private static string GenerateElementWPFConversionFromWPF40(DataElement o, Data c)
		{
			return GenerateElementWPFConversionFromWPF45(o, c);
		}

		private static string GenerateElementWPFConversionFromWPF45(DataElement o, Data c)
		{
			if (o.IsHidden == true) return "";
			if (o.IsDataMember == false) return "";
			StringBuilder Code = new StringBuilder();

			if (o.HasWPFType == true)
			{
				if (o.IsAttached == false)
					Code.AppendFormat("\t\t\tPropertyInfo pi_{0} = t_WPF.GetProperty(\"{0}\", BindingFlags.Public | BindingFlags.Instance);{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
			}
			else
			{
				if (o.IsAttached == false)
					if (o.DataType.Scope == DataScope.Public)
						Code.AppendFormat("\t\t\tPropertyInfo pi_{0} = t_WPF.GetProperty(\"{0}\", BindingFlags.Public | BindingFlags.Instance);{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					else
						Code.AppendFormat("\t\t\tPropertyInfo pi_{0} = t_WPF.GetProperty(\"{0}\", BindingFlags.NonPublic | BindingFlags.Instance);{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
			}
			Code.AppendFormat("\t\t\tFieldInfo fi_{0} = t_DTO.GetField(\"{0}Field\", BindingFlags.NonPublic | BindingFlags.Instance);{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
			if (o.WPFType.TypeMode == DataTypeMode.Array)
			{
				if (o.IsAttached == false) Code.AppendFormat("\t\t\tif(fi_{0} != null && pi_{0} != null){1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				else Code.AppendFormat("\t\t\tif(fi_{0} != null){1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendLine("\t\t\t{");
				if (o.Owner.Parent.Owner.CollectionSerializationOverride == ProjectCollectionSerializationOverride.List)
				{
					Code.AppendFormat("\t\t\t\t{0} v_{1} = new {0}();{2}", DataTypeCSGenerator.GenerateType(o.HasContractType ? o.ContractType : o.DataType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\tfi_{0}.SetValue(DTO, v_{0});{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					if (o.IsAttached == false) Code.AppendFormat("\t\t\t\t{0} wpf_{1} = pi_{1}.GetValue(Data, null) as {0};{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					else Code.AppendFormat("\t\t\t\t{0} wpf_{1} = {3}.Get{2}(Data) as {0};{4}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, o.WPFType.Name, DataTypeCSGenerator.GenerateType(c.WPFType), Environment.NewLine);
					Code.AppendFormat("\t\t\t\tforeach({0} a in wpf_{1}) {{ v_{1}.Add(a); }}{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				}
				else
				{
					if (o.IsAttached == false) Code.AppendFormat("\t\t\t\t{0} wpf_{1} = pi_{1}.GetValue(Data, null) as {0};{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					else Code.AppendFormat("\t\t\t\t{0} wpf_{1} = {2}.Get{3}(Data) as {0};{4}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\t{0}[] v_{1} = new {0}[wpf_{1}.GetLength(0)];{2}", DataTypeCSGenerator.GenerateType(o.DataType).Replace("[]", ""), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\tfor(int i = 0; i < wpf_{0}.GetLength(0); i++) {{ v_{1}[i] = Data.{1}[i]; }}{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\tfi_{0}.SetValue(DTO, v_{0});{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				}
				Code.AppendLine("\t\t\t}");
			}
			else if (o.WPFType.TypeMode == DataTypeMode.Collection)
			{
				if (o.IsAttached == false) Code.AppendFormat("\t\t\tif(fi_{0} != null && pi_{0} != null){1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				else Code.AppendFormat("\t\t\tif(fi_{0} != null){1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendLine("\t\t\t{");
				if (o.Owner.Parent.Owner.CollectionSerializationOverride == ProjectCollectionSerializationOverride.List)
				{
					Code.AppendFormat("\t\t\t\t{0} v_{1} = new {0}();{2}", DataTypeCSGenerator.GenerateType(o.HasContractType ? o.ContractType : o.DataType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\tfi_{0}.SetValue(DTO, v_{0});{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					if (o.IsAttached == false) Code.AppendFormat("\t\t\t\t{0} wpf_{1} = pi_{1}.GetValue(Data, null) as {0};{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					else Code.AppendFormat("\t\t\t\t{0} wpf_{1} = {3}.Get{2}(Data) as {0};{4}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, o.WPFType.Name, DataTypeCSGenerator.GenerateType(c.WPFType), Environment.NewLine);
					Code.AppendFormat("\t\t\t\tforeach({0} a in wpf_{1}) {{ v_{1}.Add(a); }}{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				}
				else
				{
					if (o.IsAttached == false) Code.AppendFormat("\t\t\t\t{0} wpf_{1} = pi_{1}.GetValue(Data, null) as {0};{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					else Code.AppendFormat("\t\t\t\t{0} wpf_{1} = {2}.Get{3}(Data) as {0};{4}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\t{0}[] v_{1} = new {0}[wpf_{1}.GetLength(0)];{2}", DataTypeCSGenerator.GenerateType(o.DataType).Replace("[]", ""), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\tfor(int i = 0; i < wpf_{0}.GetLength(0); i++) {{ v_{1}[i] = Data.{1}[i]; }}{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
					Code.AppendFormat("\t\t\t\tfi_{0}.SetValue(DTO, v_{0});{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				}
				Code.AppendLine("\t\t\t}");
			}
			else if (o.WPFType.TypeMode == DataTypeMode.Dictionary)
			{
				if (o.IsAttached == false) Code.AppendFormat("\t\t\tif(fi_{0} != null && pi_{0} != null){1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				else Code.AppendFormat("\t\t\tif(fi_{0} != null){1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendLine("\t\t\t{");
				Code.AppendFormat("\t\t\t\tDictionary<{0}> v_{1} = new Dictionary<{0}>();{2}", DataTypeCSGenerator.GenerateType(o.DataType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendFormat("\t\t\t\tfi_{0}.SetValue(DTO, v_{0});{1}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				if (o.IsAttached == false) Code.AppendFormat("\t\t\t\t{0} wpf_{1} = pi_{1}.GetValue(Data, null) as {0};{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				else Code.AppendFormat("\t\t\t\t{0} wpf_{1} = {3}.Get{2}(Data) as {0};{4}", DataTypeCSGenerator.GenerateType(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, o.WPFType.Name, DataTypeCSGenerator.GenerateType(c.WPFType), Environment.NewLine);
				Code.AppendFormat("\t\t\t\tforeach(KeyValuePair{0} a in wpf_{1}) {{ v_{1}.Add(a.Key, a.Value); }}{2}", DataTypeCSGenerator.GenerateTypeGenerics(o.WPFType), o.HasContractType ? o.ContractType.Name : o.DataType.Name, Environment.NewLine);
				Code.AppendLine("\t\t\t}");
			}
			else
			{
				if (o.IsAttached == false) Code.AppendFormat("\t\t\tif(fi_{0} != null && pi_{0} != null) fi_{0}.SetValue(DTO, ({2})pi_{1}.GetValue(Data, null));{3}", o.HasContractType ? o.ContractType.Name : o.DataType.Name, DataTypeCSGenerator.GenerateType(o.HasContractType ? o.ContractType : o.DataType), Environment.NewLine);
				else Code.AppendFormat("\t\t\tif(fi_{1} != null) fi_{1}.SetValue(DTO, ({2}){3}.Get{0}(Data));{4}", o.WPFType.Name, o.HasContractType ? o.ContractType.Name : o.DataType.Name, o.DataType, DataTypeCSGenerator.GenerateType(c.WPFType), Environment.NewLine);
			}

			return Code.ToString();
		}

		private static string GenerateElementWPFCode30(DataElement o)
		{
			if (o.IsHidden == true) return "";
			if (o.IsDataMember == false) return "";

			StringBuilder Code = new StringBuilder();
			if (o.HasWPFType == true)
			{
				if (o.IsReadOnly == false && o.IsAttached == false)
				{
					Code.AppendFormat("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue({1}Property); }} set {{ SetValue({1}Property, value); }} }}{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
					Code.AppendFormat("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.Register(\"{1}\", typeof({0}), typeof({2}));{3}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, DataTypeCSGenerator.GenerateType(o.Owner), Environment.NewLine);
				}
				if (o.IsReadOnly == true && o.IsAttached == false)
				{
					Code.AppendFormat("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue({1}Property); }} protected set {{ SetValue({1}PropertyKey, value); }}{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
					Code.AppendFormat("\t\tprivate static readonly DependencyPropertyKey {1}PropertyKey = DependencyProperty.RegisterReadOnly(\"{1}\", typeof({0}), typeof({2}), new PropertyMetadata(null));{3}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, DataTypeCSGenerator.GenerateType(o.Owner), Environment.NewLine);
					Code.AppendFormat("\t\tpublic static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;{1}", o.WPFType.Name, Environment.NewLine);
				}
				if (o.IsReadOnly == false && o.IsAttached == true)
				{
					if (o.AttachedBrowsable == true)
					{
						if (o.AttachedBrowsableIncludeDescendants == true) { Code.AppendLine("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=true)]"); }
						else { Code.AppendLine("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=false)]"); }
					}
					if (o.AttachedTargetTypes != "")
					{
						List<string> TTL = new List<string>(o.AttachedTargetTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string TT in TTL)
							Code.AppendFormat("\t\t[AttachedPropertyBrowsableForType(typeof({0}))]{1}", TT.Trim(), Environment.NewLine);
					}
					if (o.AttachedAttributeTypes != "")
					{
						List<string> TTL = new List<string>(o.AttachedAttributeTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string TT in TTL)
							Code.AppendFormat("\t\t[AttachedPropertyBrowsableWhenAttributePresent(typeof({0}))]{1}", TT.Trim(), Environment.NewLine);
					}
					Code.AppendFormat("\t\tpublic static {0} Get{1}(DependencyObject obj) {{ return ({0})obj.GetValue({1}Property); }}{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
					Code.AppendFormat("\t\tpublic static void Set{1}(DependencyObject obj, {0} value) {{ obj.SetValue({1}Property, value); }}{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
					Code.AppendFormat("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.RegisterAttached(\"{1}\", typeof({0}), typeof({2}), new UIPropertyMetadata(null));{3}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, DataTypeCSGenerator.GenerateType(o.Owner), Environment.NewLine);
				}
				if (o.IsReadOnly == true && o.IsAttached == true)
				{
					if (o.AttachedBrowsable == true)
					{
						if (o.AttachedBrowsableIncludeDescendants == true) { Code.AppendLine("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=true)]"); }
						else { Code.AppendLine("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=false)]"); }
					}
					if (o.AttachedTargetTypes != "")
					{
						List<string> TTL = new List<string>(o.AttachedTargetTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string TT in TTL)
							Code.AppendFormat("\t\t[AttachedPropertyBrowsableForType(typeof({0}))]{1}", TT.Trim(), Environment.NewLine);
					}
					if (o.AttachedAttributeTypes != "")
					{
						List<string> TTL = new List<string>(o.AttachedAttributeTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string TT in TTL)
							Code.AppendFormat("\t\t[AttachedPropertyBrowsableWhenAttributePresent(typeof({0}))]{1}", TT.Trim(), Environment.NewLine);
					}
					Code.AppendFormat("\t\tpublic static {0} Get{1}(DependencyObject obj) {{ return ({0})obj.GetValue({1}Property); }}{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
					Code.AppendFormat("\t\tprotected static void Set{1}(DependencyObject obj, {0} value) {{ obj.SetValue({1}PropertyKey, value); }}{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
					Code.AppendFormat("\t\tprivate static readonly DependencyPropertyKey {1}PropertyKey = DependencyProperty.RegisterAttachedReadOnly(\"{1}\", typeof({0}), typeof({2}), new PropertyMetadata(null));{3}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, DataTypeCSGenerator.GenerateType(o.Owner), Environment.NewLine);
					Code.AppendFormat("\t\tpublic static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;{1}", o.WPFType.Name, Environment.NewLine);
				}
			}
			else
			{
				if (o.IsReadOnly == false)
				{
					Code.AppendFormat("\t\tprivate {0} m_{1};{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
					Code.AppendFormat("\t\t{0} {1} {2} {{ get {{ return m_{2}; }} set {{ m_{2} = value; }} }}{3}", DataTypeCSGenerator.GenerateScope(o.DataType.Scope, o.Owner.Parent.Owner.GetType()), DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
				}
				else
				{
					Code.AppendFormat("\t\tprivate {0} m_{1};{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
					Code.AppendFormat("\t\t{0} {1} {2} {{ get {{ return m_{2}; }} protected set {{ m_{2} = value; }} }}{3}", DataTypeCSGenerator.GenerateScope(o.DataType.Scope, o.Owner.Parent.Owner.GetType()), DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
				}
			}
			return Code.ToString();
		}

		private static string GenerateElementWPFCode35(DataElement o)
		{
			return GenerateElementWPFCode40(o);
		}

		private static string GenerateElementWPFCode40(DataElement o)
		{
			return GenerateElementWPFCode45(o);
		}

		private static string GenerateElementWPFCode45(DataElement o)
		{
			if (o.IsHidden == true) return "";
			if (o.IsDataMember == false) return "";

			StringBuilder Code = new StringBuilder();
			if (o.HasWPFType == true)
			{
				if (o.IsReadOnly == false && o.IsAttached == false)
				{
					Code.AppendFormat("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue({1}Property); }} set {{ SetValue({1}Property, value); }} }}{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
					Code.AppendFormat("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.Register(\"{1}\", typeof({0}), typeof({2}));{3}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, DataTypeCSGenerator.GenerateType(o.Owner), Environment.NewLine);
				}
				if (o.IsReadOnly == true && o.IsAttached == false)
				{
					Code.AppendFormat("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue({1}Property); }} protected set {{ SetValue({1}PropertyKey, value); }} }}{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
					Code.AppendFormat("\t\tprivate static readonly DependencyPropertyKey {1}PropertyKey = DependencyProperty.RegisterReadOnly(\"{1}\", typeof({0}), typeof({2}), new PropertyMetadata(null));{3}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, DataTypeCSGenerator.GenerateType(o.Owner), Environment.NewLine);
					Code.AppendFormat("\t\tpublic static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;{1}", o.WPFType.Name, Environment.NewLine);
				}
				if (o.IsReadOnly == false && o.IsAttached == true)
				{
					if (o.AttachedBrowsable == true)
					{
						if (o.AttachedBrowsableIncludeDescendants == true) { Code.AppendLine("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=true)]"); }
						else { Code.AppendLine("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=false)]"); }
					}
					if (o.AttachedTargetTypes != "")
					{
						List<string> TTL = new List<string>(o.AttachedTargetTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string TT in TTL)
							Code.AppendFormat("\t\t[AttachedPropertyBrowsableForType(typeof({0}))]{1}", TT.Trim(), Environment.NewLine);
					}
					if (o.AttachedAttributeTypes != "")
					{
						List<string> TTL = new List<string>(o.AttachedAttributeTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string TT in TTL)
							Code.AppendFormat("\t\t[AttachedPropertyBrowsableWhenAttributePresent(typeof({0}))]{1}", TT.Trim(), Environment.NewLine);
					}
					Code.AppendFormat("\t\tpublic static {0} Get{1}(DependencyObject obj) {{ return ({0})obj.GetValue({1}Property); }}{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
					Code.AppendFormat("\t\tpublic static void Set{1}(DependencyObject obj, {0} value) {{ obj.SetValue({1}Property, value); }}{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
					Code.AppendFormat("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.RegisterAttached(\"{1}\", typeof({0}), typeof({2}), new UIPropertyMetadata(null));{3}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, DataTypeCSGenerator.GenerateType(o.Owner), Environment.NewLine);
				}
				if (o.IsReadOnly == true && o.IsAttached == true)
				{
					if (o.AttachedBrowsable == true)
					{
						if (o.AttachedBrowsableIncludeDescendants == true) { Code.AppendLine("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=true)]"); }
						else { Code.AppendLine("\t\t[AttachedPropertyBrowsableForChildren(IncludeDescendants=false)]"); }
					}
					if (o.AttachedTargetTypes != "")
					{
						List<string> TTL = new List<string>(o.AttachedTargetTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string TT in TTL)
							Code.AppendFormat("\t\t[AttachedPropertyBrowsableForType(typeof({0}))]{1}", TT.Trim(), Environment.NewLine);
					}
					if (o.AttachedAttributeTypes != "")
					{
						List<string> TTL = new List<string>(o.AttachedAttributeTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						foreach (string TT in TTL)
							Code.AppendFormat("\t\t[AttachedPropertyBrowsableWhenAttributePresent(typeof({0}))]{1}", TT.Trim(), Environment.NewLine);
					}
					Code.AppendFormat("\t\tpublic static {0} Get{1}(DependencyObject obj) {{ return ({0})obj.GetValue({1}Property); }}{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
					Code.AppendFormat("\t\tprotected static void Set{1}(DependencyObject obj, {0} value) {{ obj.SetValue({1}PropertyKey, value); }}{2}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
					Code.AppendFormat("\t\tprivate static readonly DependencyPropertyKey {1}PropertyKey = DependencyProperty.RegisterAttachedReadOnly(\"{1}\", typeof({0}), typeof({2}), new PropertyMetadata(null));{3}", DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, DataTypeCSGenerator.GenerateType(o.Owner), Environment.NewLine);
					Code.AppendFormat("\t\tpublic static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;{1}", o.WPFType.Name, Environment.NewLine);
				}
			}
			else
			{
				if (o.IsReadOnly == false)
				{
					Code.AppendFormat("\t\t{0} {1} {2} {{ get; set; }}{3}", DataTypeCSGenerator.GenerateScope(o.DataType.Scope, o.Owner.Parent.Owner.GetType()), DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
				}
				else
				{
					Code.AppendFormat("\t\t{0} {1} {2} {{ get; protected set; }}{3}", DataTypeCSGenerator.GenerateScope(o.DataType.Scope, o.Owner.Parent.Owner.GetType()), DataTypeCSGenerator.GenerateType(o.WPFType), o.WPFType.Name, Environment.NewLine);
				}
			}
			return Code.ToString();
		}
	}
}
