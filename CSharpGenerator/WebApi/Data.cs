using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading.Tasks;
using NETPath.Projects;
using NETPath.Projects.WebApi;
using NETPath.Projects.Helpers;

namespace NETPath.Generators.CS.WebApi
{
	internal static class DataGenerator
	{
		public static void VerifyCode(WebApiData o, Action<CompileMessage> AddMessage)
		{
			if (string.IsNullOrEmpty(o.Name))
				AddMessage(new CompileMessage("GS3000", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace has a blank Code Name. A Code Name MUST be specified.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));
			else
				if (RegExs.MatchCodeName.IsMatch(o.Name) == false)
				AddMessage(new CompileMessage("GS3001", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));

			if (o.HasClientType)
				if (RegExs.MatchCodeName.IsMatch(o.ClientType.Name) == false)
					AddMessage(new CompileMessage("GS3002", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));

			if (o.HasXAMLType)
				if (RegExs.MatchCodeName.IsMatch(o.XAMLType.Name) == false)
					AddMessage(new CompileMessage("GS3003", "The data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));

			foreach (var d in o.Elements)
			{
				if (RegExs.MatchCodeName.IsMatch(d.DataType.Name) == false && d.DataType.TypeMode != DataTypeMode.Array)
					AddMessage(new CompileMessage("GS3001", "The data element '" + d.DataName + "' in the '" + o.Name + "' data object contains invalid characters in the Name.", CompileMessageSeverity.ERROR, o, d, d.GetType()));

				if (d.HasClientType)
					if (RegExs.MatchCodeName.IsMatch(d.ClientName) == false && d.DataType.TypeMode != DataTypeMode.Array)
						AddMessage(new CompileMessage("GS3002", "The data element '" + d.ClientName + "' in the '" + o.Name + "' data object contains invalid characters in the Client Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));

				if (d.HasXAMLType)
					if (RegExs.MatchCodeName.IsMatch(d.XAMLName) == false && d.DataType.TypeMode != DataTypeMode.Array)
						AddMessage(new CompileMessage("GS3003", "The data element '" + d.XAMLName + "' in the '" + o.Name + "' data object contains invalid characters in the XAML Name.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));
			}

			if (o.InheritedTypes.Any(a => a.Name.IndexOf("INotifyPropertyChanged", StringComparison.CurrentCultureIgnoreCase) >= 0))
				AddMessage(new CompileMessage("GS3005", "The server data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace is unable to inherit from INotifyPropertyChanged.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));
			if (o.HasXAMLType && o.XAMLType.InheritedTypes.Any(a => a.Name.IndexOf("INotifyPropertyChanged", StringComparison.CurrentCultureIgnoreCase) >= 0))
				AddMessage(new CompileMessage("GS3006", "The XAML integration object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace is unable to inherit from INotifyPropertyChanged.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));

			if (o.InheritedTypes.Any(a => a.Name.IndexOf("DependencyObject", StringComparison.CurrentCultureIgnoreCase) >= 0))
				AddMessage(new CompileMessage("GS3007", "The server data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace is unable to inherit from DependencyObject.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));
			if (o.HasClientType && o.ClientType.InheritedTypes.Any(a => a.Name.IndexOf("DependencyObject", StringComparison.CurrentCultureIgnoreCase) >= 0))
				AddMessage(new CompileMessage("GS3008", "The client data object '" + o.Name + "' in the '" + o.Parent.Name + "' namespace is unable to inherit from DependencyObject.", CompileMessageSeverity.ERROR, o.Parent, o, o.GetType()));
		}

		#region - Server -

		public static string GenerateServerCode45(WebApiData o)
		{
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			foreach (DataType dt in o.KnownTypes)
				code.AppendLine(string.Format("\t[KnownType(typeof({0}))]", dt));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCode(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t[DataContract({0}Name = \"{1}\", Namespace = \"{2}\")]", o.IsReference ? "IsReference = true, " : "", o.HasClientType ? o.ClientType.Name : o.Name, o.Parent.URI));
			code.AppendLine(string.Format("\t{0}", DataTypeGenerator.GenerateTypeDeclaration(o, false, false, false, false, false, o.HasEntity, o.Parent.Owner.EnitityDatabaseType)));
			code.AppendLine("\t{");

			if (o.Parent.Owner.GenerateRegions) code.AppendLine("\t\t#region Constructor");
			else code.AppendLine("\t\t//Constuctors");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}()", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			foreach (var de in o.Elements)
				if (de.DataType.TypeMode == DataTypeMode.Collection || de.DataType.TypeMode == DataTypeMode.Dictionary)
					code.AppendLine(string.Format("\t\t\t_{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(de.DataType))));
				else if (de.DataType.TypeMode == DataTypeMode.Queue || de.DataType.TypeMode == DataTypeMode.Stack)
					code.AppendLine(string.Format("\t\t\t_{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(de.DataType)), de.DataName));
				else if (de.DataType.TypeMode == DataTypeMode.Array)
					code.AppendLine(string.Format("\t\t\t_{1} = new {0}[0];", DataTypeGenerator.GenerateType(GetPreferredDTOType(de.DataType.CollectionGenericType)), de.DataName));
			code.AppendLine("\t\t}");
			code.AppendLine();
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}

			if (o.Parent.Owner.GenerateRegions && o.HasEntity)
			{
				code.AppendLine("\t\t#region Entity Framework Support");
				code.AppendLine();
			}

			if (o.HasEntity)
			{
				code.AppendLine(string.Format("\t\tpublic static implicit operator {0}({1} DBType)", o.Name, o.EntityType));
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tif (DBType == null) return null;");
				code.AppendLine(string.Format("\t\t\tvar t = new {0}()", o.Name));
				code.AppendLine("\t\t\t{");
				foreach (var efe in o.Elements.Where(a => a.HasEntity && !(a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
					code.AppendLine(string.Format("\t\t\t\t{0} = DBType.{1},", efe.DataName, efe.EntityName));
				code.AppendLine("\t\t\t};");
				foreach (var efe in o.Elements.Where(a => a.HasEntity && a.DataType.TypeMode == DataTypeMode.Collection))
					code.AppendLine(string.Format("\t\t\tif (DBType.{0} != null) foreach(var x in DBType.{0}) t.{1}.Add(x);", efe.EntityName, efe.DataName));
				code.AppendLine("\t\t\tFinishCastToNetwork(ref t, DBType);");
				code.AppendLine("\t\t\treturn t;");
				code.AppendLine("\t\t}");
				code.AppendLine(string.Format("\t\tstatic partial void FinishCastToNetwork(ref {0} NetworkType, {1} DBType);", o.Name, o.EntityType));
				code.AppendLine();

				code.AppendLine(string.Format("\t\tpublic static implicit operator {0}({1} NetworkType)", o.EntityType, o.Name));
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tif (NetworkType == null) return null;");
				code.AppendLine(string.Format("\t\t\tvar t = new {0}()", o.EntityType));
				code.AppendLine("\t\t\t{");
				foreach (var efe in o.Elements.Where(a => a.HasEntity && !a.IsReadOnly && !(a.DataType.TypeMode == DataTypeMode.Collection || a.DataType.TypeMode == DataTypeMode.Dictionary)))
					code.AppendLine(string.Format("\t\t\t\t{0} = NetworkType.{1},", efe.EntityName, efe.DataName));
				code.AppendLine("\t\t\t};");
				foreach (var efe in o.Elements.Where(a => a.HasEntity && !a.IsReadOnly && a.DataType.TypeMode == DataTypeMode.Collection))
					code.AppendLine(string.Format("\t\t\tif (NetworkType.{0} != null) foreach(var x in NetworkType.{0}) t.{1}.Add(x);", efe.DataName, efe.EntityName));
				code.AppendLine("\t\t\tFinishCastToDatabase(ref t, NetworkType);");
				code.AppendLine("\t\t\treturn t;");
				code.AppendLine("\t\t}");
				code.AppendLine(string.Format("\t\tstatic partial void FinishCastToDatabase(ref {0} DBType, {1} NetworkType);", o.EntityType, o.Name));
				code.AppendLine();
			}
			if (o.Parent.Owner.GenerateRegions && o.HasEntity)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}

			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Data Members");
				code.AppendLine();
			}
			foreach (var de in o.Elements.Where(a => !a.IsHidden))
				code.AppendLine(GenerateElementServerCode45(de));
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}

			code.AppendLine("\t}");
			return code.ToString();
		}

		private static string GenerateElementServerCode45(WebApiDataElement o)
		{
			var code = new StringBuilder();
			code.AppendLine(string.Format("\t\t[DataMember({0}{1}{2}{3})]", o.IsRequired ? "IsRequired = true" : "IsRequired = false", o.EmitDefaultValue ? ", EmitDefaultValue = false" : "", o.Order >= 0 ? string.Format(", Order = {0}{1}", o.Order, o.HasContractName ? ", " : "") : "", o.HasContractName ? string.Format(", Name = \"{0}\"", o.ContractName) : ""));
			code.AppendLine(string.Format("\t\tprivate {2}{0} _{1};", DataTypeGenerator.GenerateType(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.IsReadOnly && ((o.Owner.HasClientType && o.Owner.ClientType.Sealed) || (!o.Owner.HasClientType && o.Owner.Sealed)) ? "readonly " : ""));
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.AppendLine(string.Format("\t\tpublic {0} {1} {{ get {{ return _{1}; }} {2}}}", DataTypeGenerator.GenerateType(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.IsReadOnly && ((o.Owner.HasClientType && o.Owner.ClientType.Sealed) || (!o.Owner.HasClientType && o.Owner.Sealed)) ? "" : string.Format("{0}set {{ _{1} = value;}} ", o.IsReadOnly && ((o.Owner.HasClientType && !o.Owner.ClientType.Sealed) || (!o.Owner.HasClientType && !o.Owner.Sealed)) ? "protected " : "", o.HasClientType ? o.ClientName : o.DataName)));
			return code.ToString();
		}

		#endregion

		#region - Proxy -

		public static string GenerateProxyCode45(WebApiData o)
		{
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			foreach (DataType dt in o.ClientType != null ? o.ClientType.KnownTypes : o.KnownTypes)
				code.AppendLine(string.Format("\t[KnownType(typeof({0}))]", dt));
			code.AppendLine("\t[System.Diagnostics.DebuggerStepThrough]");
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCode(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t[DataContract({0}Name = \"{1}\", Namespace = \"{2}\")]", o.IsReference ? "IsReference = true, " : "", o.HasClientType ? o.ClientType.Name : o.Name, o.Parent.URI));
			code.AppendLine(string.Format("\t{0}", DataTypeGenerator.GenerateTypeDeclaration(o.HasClientType ? o.ClientType : o, false, o.HasWinFormsBindings)));
			code.AppendLine("\t{");
			code.AppendLine();

			if (o.HasWinFormsBindings)
			{
				code.AppendLine("\t\t//Windows Forms Binding Support");
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
				code.AppendLine("\t\t//Implicit Conversion from XAML Type");
				code.AppendLine(string.Format("\t\tpublic static implicit operator {0}({1} Data)", o.HasClientType ? o.ClientType.Name : o.Name, o.XAMLType.Name));
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tif (Data == null) return null;");
				code.AppendLine(string.Format("\t\t\treturn new {0}(Data);", o.ClientType.Name));
				code.AppendLine("\t\t}");
				code.AppendLine();
			}

			if (o.Parent.Owner.GenerateRegions) code.AppendLine("\t\t#region Constructors");
			else code.AppendLine("\t\t//Constuctors");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}()", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			foreach (var de in o.Elements)
				if (de.DataType.TypeMode == DataTypeMode.Collection || de.DataType.TypeMode == DataTypeMode.Dictionary || de.DataType.TypeMode == DataTypeMode.Queue || de.DataType.TypeMode == DataTypeMode.Stack)
					code.AppendLine(string.Format("\t\t\t_{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(de.DataType)), de.HasXAMLType ? de.XAMLName : de.HasClientType ? de.ClientName : de.DataName));
				else if (de.DataType.TypeMode == DataTypeMode.Array)
					code.AppendLine(string.Format("\t\t\t_{1} = new {0}[0];", DataTypeGenerator.GenerateType(GetPreferredDTOType(de.DataType.CollectionGenericType)), de.HasXAMLType ? de.XAMLName : de.HasClientType ? de.ClientName : de.DataName));
			if (o.HasXAMLType) code.AppendLine(string.Format("\t\t\tApplication.Current.Dispatcher.Invoke(() => {{ BaseXAMLObject = new {0}(this); }}, System.Windows.Threading.DispatcherPriority.Normal);", o.XAMLType.Name));
			code.AppendLine("\t\t}");
			if (o.HasXAMLType)
			{
				code.AppendLine(string.Format("\t\tpublic {0}({1} Data)", o.HasClientType ? o.ClientType.Name : o.Name, o.XAMLType.Name));
				code.AppendLine("\t\t{");
				foreach (var de in o.Elements)
					code.Append(GenerateElementProxyConstructorCode45(de, o));
				code.AppendLine("\t\t}");
			}
			code.AppendLine();
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}

			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Data Members");
				code.AppendLine();
			}
			foreach (var de in o.Elements.Where(a => !a.IsHidden && a.IsDataMember))
				code.AppendLine(GenerateElementProxyCode(de));
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}
			code.AppendLine("\t}");
			return code.ToString();
		}

		public static string GenerateProxyCodeRT8(WebApiData o)
		{
			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			foreach (DataType dt in o.ClientType != null ? o.ClientType.KnownTypes : o.KnownTypes)
				code.AppendLine(string.Format("\t[KnownType(typeof({0}))]", dt));
			code.AppendLine("\t[System.Diagnostics.DebuggerStepThrough]");
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCode(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t[DataContract({0}Name = \"{1}\", Namespace = \"{2}\")]", o.IsReference ? "IsReference = true, " : "", o.HasClientType ? o.ClientType.Name : o.Name, o.Parent.URI));
			code.AppendLine(string.Format("\t{0}", DataTypeGenerator.GenerateTypeDeclaration(o.HasClientType ? o.ClientType : o, false, o.HasWinFormsBindings)));
			code.AppendLine("\t{");
			code.AppendLine();

			if (o.HasWinFormsBindings)
			{
				code.AppendLine("\t\t//Windows Forms Binding Support");
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
				code.AppendLine("\t\t//Implicit Conversion from XAML Type");
				code.AppendLine(string.Format("\t\tpublic static implicit operator {0}({1} Data)", o.HasClientType ? o.ClientType.Name : o.Name, o.XAMLType.Name));
				code.AppendLine("\t\t{");
				code.AppendLine("\t\t\tif (Data == null) return null;");
				code.AppendLine(string.Format("\t\t\treturn new {0}(Data);", o.ClientType.Name));
				code.AppendLine("\t\t}");
				code.AppendLine();
			}

			if (o.Parent.Owner.GenerateRegions) code.AppendLine("\t\t#region Constructors");
			else code.AppendLine("\t\t//Constuctors");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}()", o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			foreach (var de in o.Elements)
				if (de.DataType.TypeMode == DataTypeMode.Collection || de.DataType.TypeMode == DataTypeMode.Dictionary || de.DataType.TypeMode == DataTypeMode.Queue || de.DataType.TypeMode == DataTypeMode.Stack)
					code.AppendLine(string.Format("\t\t\t_{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(de.DataType)), de.HasXAMLType ? de.XAMLName : de.HasClientType ? de.ClientName : de.DataName));
				else if (de.DataType.TypeMode == DataTypeMode.Array)
					code.AppendLine(string.Format("\t\t\t_{1} = new {0}[0];", DataTypeGenerator.GenerateType(GetPreferredDTOType(de.DataType.CollectionGenericType)), de.HasXAMLType ? de.XAMLName : de.HasClientType ? de.ClientName : de.DataName));
			if (o.HasXAMLType) code.AppendLine(string.Format("\t\t\tWindow.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {{ BaseXAMLObject = new {0}(this); }}).GetResults();", o.XAMLType.Name));
			code.AppendLine("\t\t}");
			if (o.HasXAMLType)
			{
				code.AppendLine(string.Format("\t\tpublic {0}({1} Data)", o.HasClientType ? o.ClientType.Name : o.Name, o.XAMLType.Name));
				code.AppendLine("\t\t{");
				foreach (var de in o.Elements)
					code.Append(GenerateElementProxyConstructorCode45(de, o));
				code.AppendLine("\t\t}");
			}
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine();
				code.AppendLine("\t\t#endregion");
			}
			code.AppendLine();

			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#region Data Members");
				code.AppendLine();
			}
			foreach (var de in o.Elements.Where(a => !a.IsHidden && a.IsDataMember))
				code.AppendLine(GenerateElementProxyCode(de));
			if (o.Parent.Owner.GenerateRegions)
			{
				code.AppendLine("\t\t#endregion");
				code.AppendLine();
			}
			code.AppendLine("\t}");
			return code.ToString();
		}

		private static string GenerateElementProxyCode(WebApiDataElement o)
		{
			var code = new StringBuilder();
			code.AppendLine(string.Format("\t\t[DataMember({0}{1}{2}{3})]", o.IsRequired ? "IsRequired = true" : "IsRequired = false", o.EmitDefaultValue ? ", EmitDefaultValue = false" : "", o.Order >= 0 ? string.Format(", Order = {0}{1}", o.Order, o.HasContractName ? ", " : "") : "", o.HasContractName ? string.Format(", Name = \"{0}\"", o.ContractName) : ""));
			code.AppendLine(string.Format("\t\tprivate {2}{0} _{1};", DataTypeGenerator.GenerateType(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.IsReadOnly && ((o.Owner.HasClientType && o.Owner.ClientType.Sealed) || (!o.Owner.HasClientType && o.Owner.Sealed)) ? "readonly " : ""));
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.AppendLine(string.Format("\t\tpublic {0} {1} {{ get {{ return _{1}; }} {2}}}", DataTypeGenerator.GenerateType(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.IsReadOnly && ((o.Owner.HasClientType && o.Owner.ClientType.Sealed) || (!o.Owner.HasClientType && o.Owner.Sealed)) ? "" : string.Format("{0}set {{ _{1} = value; {2}}} ", o.IsReadOnly && ((o.Owner.HasClientType && !o.Owner.ClientType.Sealed) || (!o.Owner.HasClientType && !o.Owner.Sealed)) ? "protected " : "", o.HasClientType ? o.ClientName : o.DataName, o.GenerateWinFormsSupport ? "NotifyPropertyChanged(); " : "")));
			return code.ToString();
		}

		private static string GenerateElementProxyConstructorCode45(WebApiDataElement o, WebApiData c)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			if (!o.HasXAMLType) return "";
			var code = new StringBuilder();

			if (o.DataType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\t\tvar v{2} = new {0}[Data.{1}.Length];", DataTypeGenerator.GenerateType(o.DataType).Replace("[]", ""), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) for(int i = 0; i < Data.{0}.Length; i++) {{ v{1}[i] = Data.{0}[i]; }}", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\tvar v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Add(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Stack)
			{
				code.AppendLine(string.Format("\t\t\tvar v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Push(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Queue)
			{
				code.AppendLine(string.Format("\t\t\tvar v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Enqueue(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\tvar v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(o.DataType.Name == "System.Collections.Concurrent.ConcurrentDictionary" ? string.Format("\t\t\tif (Data.{1} != null) foreach(KeyValuePair<{0}> a in Data.{1}) {{ v{2}.TryAdd(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName) : string.Format("\t\t\tif (Data.{1} != null) foreach(KeyValuePair<{0}> a in Data.{1}) {{ v{2}.Add(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
			}
			else
				code.AppendLine(string.Format("\t\t\t{1} = Data.{0};", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));

			return code.ToString();
		}

		// Updates the DTO from the XAML
		private static string GenerateElementProxyUpdateCode45(WebApiDataElement o, WebApiData c)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			if (!o.HasXAMLType) return "";
			var code = new StringBuilder();

			if (o.DataType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\t\tvar v{2} = new {0}[XAMLObject.{1}.Length];", DataTypeGenerator.GenerateType(o.DataType).Replace("[]", ""), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (XAMLObject.{1} != null) for(int i = 0; i < XAMLObject.{0}.Length; i++) {{ v{1}[i] = XAMLObject.{0}[i]; }}", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\tvar v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (XAMLObject.{1} != null) foreach({0} a in XAMLObject.{1}) {{ v{2}.Add(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Stack)
			{
				code.AppendLine(string.Format("\t\t\tvar v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (XAMLObject.{1} != null) foreach({0} a in XAMLObject.{1}) {{ v{2}.Push(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Queue)
			{
				code.AppendLine(string.Format("\t\t\tvar {1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\tif (XAMLObject.{1} != null) foreach({0} a in XAMLObject.{1}) {{ v{2}.Enqueue(a); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\tvar v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredDTOType(o.DataType)), o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(o.DataType.Name == "System.Collections.Concurrent.ConcurrentDictionary" ? string.Format("\t\t\tif (XAMLObject.{1} != null) foreach(KeyValuePair<{0}> a in XAMLObject.{1}) {{ v{2}.TryAdd(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName) : string.Format("\t\t\tif (XAMLObject.{1} != null) foreach(KeyValuePair<{0}> a in XAMLObject.{1}) {{ v{2}.Add(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.HasClientType ? o.ClientName : o.DataName));
			}
			else
				code.AppendLine(string.Format("\t\t\t{1} = XAMLObject.{0};", o.XAMLName, o.HasClientType ? o.ClientName : o.DataName));

			return code.ToString();
		}

		#endregion

		#region - XAML -

		public static string GenerateXAMLCode45(WebApiData o)
		{
			if (o.HasXAMLType == false) return "";
			if (o.Elements.Any(de => de.HasXAMLType) == false) return "";

			//This is a shim to ensure there is ALWAYS a XAML name.
			if (string.IsNullOrEmpty(o.XAMLType.Name)) o.XAMLType.Name = o.Name + "XAML";

			var code = new StringBuilder();
			code.AppendLine(string.Format("\t//XAML Integration Object for the {0} DTO", o.HasClientType ? o.ClientType.Name : o.Name));
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCode(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0}", DataTypeGenerator.GenerateTypeDeclaration(o.XAMLType, false, false, true)));
			code.AppendLine("\t{");
			code.AppendLine();

			code.AppendLine("\t\t//Implicit Conversion from DTO Type");
			code.AppendLine(string.Format("\t\tpublic static implicit operator {0}({1} Data)", o.XAMLType.Name, o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tif (Data == null) return null;");
			code.AppendLine(string.Format("\t\t\treturn new {0}(Data);", o.XAMLType.Name));
			code.AppendLine("\t\t}");
			code.AppendLine();

			code.AppendLine("\t\t//Constructors");
			code.AppendLine(string.Format("\t\tpublic {0}()", o.XAMLType.Name));
			code.AppendLine("\t\t{");
			foreach (var de in o.Elements)
				if (de.DataType.TypeMode == DataTypeMode.Collection || de.DataType.TypeMode == DataTypeMode.Dictionary || de.DataType.TypeMode == DataTypeMode.Queue || de.DataType.TypeMode == DataTypeMode.Stack)
					code.AppendLine(string.Format("\t\t\t{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(de.DataType)), de.XAMLName));
				else if (de.DataType.TypeMode == DataTypeMode.Array)
					code.AppendLine(string.Format("\t\t\t{1} = new {0}[0];", DataTypeGenerator.GenerateType(GetPreferredXAMLType(de.DataType.CollectionGenericType)), de.XAMLName));
			code.AppendLine("\t\t}");
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}({1} Data)", o.XAMLType.Name, o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			foreach (var de in o.Elements)
				code.Append(GenerateElementXAMLConstructorCode45(de, o));
			code.AppendLine("\t\t}");
			code.AppendLine();

			code.AppendLine("\t\t//Properties");
			foreach (var de in o.Elements.Where(a => !a.IsHidden && a.IsDataMember))
				code.AppendLine(GenerateElementXAMLCode45(de));

			code.AppendLine("\t}");
			code.AppendLine();

			return code.ToString();
		}

		private static string GenerateElementXAMLCode45(WebApiDataElement o)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			if (!o.Owner.HasXAMLType) return "";
			if (!o.HasXAMLType) return "";

			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));

			code.AppendLine(string.Format("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue({1}Property); }} {2}set {{ SetValue({1}Property, value); }} }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.IsReadOnly ? "protected " : ""));
			code.AppendLine(string.Format("\t\tpublic static readonly DependencyProperty{4} {1}Property{4} = DependencyProperty.Register{3}(\"{1}\", typeof({0}), typeof({2}));", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.Owner.XAMLType.Name, o.IsReadOnly ? "ReadOnly" : "", o.IsReadOnly ? "Key" : ""));
			if (o.IsReadOnly) code.AppendLine(string.Format("\t\tpublic static readonly DependencyProperty {0}Property = {0}PropertyKey.DependencyProperty;", o.XAMLName));

			return code.ToString();
		}

		public static string GenerateXAMLCodeRT8(WebApiData o)
		{
			if (o.HasXAMLType == false) return "";
			if (o.Elements.Any(de => de.HasXAMLType) == false) return "";

			//This is a shim to ensure there is ALWAYS a XAML name.
			if (string.IsNullOrEmpty(o.XAMLType.Name)) o.XAMLType.Name = o.Name + "XAML";

			var code = new StringBuilder();
			code.AppendLine(string.Format("\t//XAML Integration Object for the {0} DTO", o.HasClientType ? o.ClientType.Name : o.Name));
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.AppendLine(string.Format("\t[System.CodeDom.Compiler.GeneratedCode(\"{0}\", \"{1}\")]", Globals.ApplicationTitle, Globals.ApplicationVersion));
			code.AppendLine(string.Format("\t{0}", DataTypeGenerator.GenerateTypeDeclaration(o.XAMLType, false, false, true)));
			code.AppendLine("\t{");
			code.AppendLine();

			code.AppendLine("\t\t//Implicit Conversion from DTO Type");
			code.AppendLine(string.Format("\t\tpublic static implicit operator {0}({1} Data)", o.XAMLType.Name, o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			code.AppendLine("\t\t\tif (Data == null) return null;");
			code.AppendLine(string.Format("\t\t\treturn new {0}(Data);", o.XAMLType.Name));
			code.AppendLine("\t\t}");
			code.AppendLine();

			code.AppendLine("\t\t//Constructors");
			code.AppendLine(string.Format("\t\tpublic {0}()", o.XAMLType.Name));
			code.AppendLine("\t\t{");
			foreach (var de in o.Elements)
				if (de.DataType.TypeMode == DataTypeMode.Collection || de.DataType.TypeMode == DataTypeMode.Dictionary || de.DataType.TypeMode == DataTypeMode.Queue || de.DataType.TypeMode == DataTypeMode.Stack)
					code.AppendLine(string.Format("\t\t\t{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(de.DataType)), de.XAMLName));
				else if (de.DataType.TypeMode == DataTypeMode.Array)
					code.AppendLine(string.Format("\t\t\t{1} = new {0}[0];", DataTypeGenerator.GenerateType(GetPreferredXAMLType(de.DataType.CollectionGenericType)), de.XAMLName));
			code.AppendLine();
			code.AppendLine(string.Format("\t\tpublic {0}({1} Data)", o.XAMLType.Name, o.HasClientType ? o.ClientType.Name : o.Name));
			code.AppendLine("\t\t{");
			foreach (var de in o.Elements)
				code.Append(GenerateElementXAMLConstructorCode45(de, o));
			code.AppendLine("\t\t}");
			code.AppendLine();

			code.AppendLine("\t\t//Properties");
			foreach (var de in o.Elements.Where(a => !a.IsHidden && a.IsDataMember))
				code.AppendLine(GenerateElementXAMLCodeRT8(de));

			code.AppendLine("\t}");
			code.AppendLine();

			return code.ToString();
		}

		private static string GenerateElementXAMLCodeRT8(WebApiDataElement o)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			if (!o.Owner.HasXAMLType) return "";
			if (!o.HasXAMLType) return "";

			var code = new StringBuilder();
			if (o.Documentation != null) code.Append(DocumentationGenerator.GenerateDocumentation(o.Documentation));
			code.AppendLine(string.Format("\t\tpublic {0} {1} {{ get {{ return ({0})GetValue({1}Property); }} {2}set {{ SetValue({1}Property, value); }} }}", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.IsReadOnly ? "protected " : ""));
			code.AppendLine(string.Format("\t\tpublic static readonly DependencyProperty {1}Property = DependencyProperty.Register(\"{1}\", typeof({0}), typeof({2}));", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType)), o.XAMLName, o.Owner.XAMLType.Name));
			return code.ToString();
		}

		private static string GenerateElementXAMLConstructorCode45(WebApiDataElement o, WebApiData c)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			if (!o.HasXAMLType) return "";
			var code = new StringBuilder();

			if (o.DataType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\t\tvar v{2} = new {0}[Data.{1}.Length];", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType)).Replace("[]", ""), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) for(int i = 0; i < Data.{0}.Length; i++) {{ v{1}[i] = Data.{0}[i]; }}", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\tvar v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Add(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Stack)
			{
				code.AppendLine(string.Format("\t\t\tvar v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Push(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Queue)
			{
				code.AppendLine(string.Format("\t\t\tvar v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (Data.{1} != null) foreach({0} a in Data.{1}) {{ v{2}.Enqueue(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\tvar v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType)), o.XAMLName));
				code.AppendLine(o.DataType.Name == "System.Collections.Concurrent.ConcurrentDictionary" ? string.Format("\t\t\tif (Data.{1} != null) foreach(KeyValuePair<{0}> a in Data.{1}) {{ v{2}.TryAdd(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName) : string.Format("\t\t\tif (Data.{1} != null) foreach(KeyValuePair<{0}> a in Data.{1}) {{ v{2}.Add(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
			}
			else
				code.AppendLine(string.Format("\t\t\t{1} = Data.{0};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));

			return code.ToString();
		}

		// Updates the XAML from the DTO
		private static string GenerateElementXAMLUpdateCode45(WebApiDataElement o, WebApiData c)
		{
			if (o.IsHidden) return "";
			if (!o.IsDataMember) return "";
			if (!o.HasXAMLType) return "";
			var code = new StringBuilder();

			if (o.DataType.TypeMode == DataTypeMode.Array)
			{
				code.AppendLine(string.Format("\t\t\tvar v{2} = new {0}[DataObject.{1}.Length];", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType)).Replace("[]", ""), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (DataObject.{1} != null) for(int i = 0; i < DataObject.{0}.Length; i++) {{ v{1}[i] = DataObject.{0}[i]; }}", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Collection)
			{
				code.AppendLine(string.Format("\t\t\tvar v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (DataObject.{1} != null) foreach({0} a in DataObject.{1}) {{ v{2}.Add(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Stack)
			{
				code.AppendLine(string.Format("\t\t\tvar v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (DataObject.{1} != null) foreach({0} a in DataObject.{1}) {{ v{2}.Push(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Queue)
			{
				code.AppendLine(string.Format("\t\t\tvar v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType)), o.XAMLName));
				code.AppendLine(string.Format("\t\t\tif (DataObject.{1} != null) foreach({0} a in DataObject.{1}) {{ v{2}.Enqueue(a); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
			}
			else if (o.DataType.TypeMode == DataTypeMode.Dictionary)
			{
				code.AppendLine(string.Format("\t\t\tvar v{1} = new {0}();", DataTypeGenerator.GenerateType(GetPreferredXAMLType(o.DataType)), o.XAMLName));
				code.AppendLine(o.DataType.Name == "System.Collections.Concurrent.ConcurrentDictionary" ? string.Format("\t\t\tif (DataObject.{1} != null) foreach(KeyValuePair<{0}> a in DataObject.{1}) {{ v{2}.TryAdd(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName) : string.Format("\t\t\tif (DataObject.{1} != null) foreach(KeyValuePair<{0}> a in DataObject.{1}) {{ v{2}.Add(a.Key, a.Value); }}", DataTypeGenerator.GenerateTypeGenerics(o.DataType), o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));
				code.AppendLine(string.Format("\t\t\t{0} = v{0};", o.XAMLName));
			}
			else
				code.AppendLine(string.Format("\t\t\t{1} = DataObject.{0};", o.HasClientType ? o.ClientName : o.DataName, o.XAMLName));

			return code.ToString();
		}

		#endregion

		#region - Helpers -

		private static DataType GetPreferredDTOType(DataType value)
		{
			if (value.TypeMode == DataTypeMode.Array)
			{
				if (value.CollectionGenericType.TypeMode == DataTypeMode.Class || value.CollectionGenericType.TypeMode == DataTypeMode.Struct)
				{
					var t = value.CollectionGenericType as WebApiData;
					if (t == null) return value;
					return new DataType(DataTypeMode.Array) { Name = value.Name, CollectionGenericType = t.HasClientType ? t.ClientType : t };
				}
			}
			else if (value.TypeMode == DataTypeMode.Collection || value.TypeMode == DataTypeMode.Stack || value.TypeMode == DataTypeMode.Queue)
			{
				if (value.CollectionGenericType.TypeMode == DataTypeMode.Class || value.CollectionGenericType.TypeMode == DataTypeMode.Struct)
				{
					var t = value.CollectionGenericType as WebApiData;
					if (t == null) return value;
					return new DataType(value.Name, value.TypeMode) { Name = value.Name, CollectionGenericType = t.HasClientType ? t.ClientType : t };
				}
			}
			else if (value.TypeMode == DataTypeMode.Dictionary)
			{
				DataType dk = value.DictionaryKeyGenericType;
				DataType dv = value.DictionaryValueGenericType;
				if (value.DictionaryKeyGenericType.TypeMode == DataTypeMode.Class || value.DictionaryKeyGenericType.TypeMode == DataTypeMode.Struct)
				{
					var t = value.DictionaryKeyGenericType as WebApiData;
					if (t == null) return value;
					dk = t.HasClientType ? t.ClientType : t;
				}
				if (value.DictionaryValueGenericType.TypeMode == DataTypeMode.Class || value.DictionaryValueGenericType.TypeMode == DataTypeMode.Struct)
				{
					var t = value.DictionaryValueGenericType as WebApiData;
					if (t == null) return value;
					dv = t.HasClientType ? t.ClientType : t;
				}
				return new DataType(value.Name, DataTypeMode.Dictionary) { Name = value.Name, DictionaryKeyGenericType = dk, DictionaryValueGenericType = dv };
			}
			else if (value.TypeMode == DataTypeMode.Class || value.TypeMode == DataTypeMode.Struct)
			{
				var t = value as WebApiData;
				if (t == null) return value;
				return t.HasClientType ? t.ClientType : t;
			}
			return value;
		}

		private static DataType GetPreferredXAMLType(DataType value)
		{
			if (value.TypeMode == DataTypeMode.Array)
			{
				if (value.CollectionGenericType.TypeMode == DataTypeMode.Class || value.CollectionGenericType.TypeMode == DataTypeMode.Struct)
				{
					var t = value.CollectionGenericType as WebApiData;
					if (t == null) return value;
					return t.HasXAMLType ? new DataType(DataTypeMode.Array) { Name = value.Name, CollectionGenericType = t.XAMLType } : new DataType(DataTypeMode.Array) { Name = value.Name, CollectionGenericType = t.HasClientType ? t.ClientType : t };
				}
			}
			else if (value.TypeMode == DataTypeMode.Collection || value.TypeMode == DataTypeMode.Stack || value.TypeMode == DataTypeMode.Queue)
			{
				if (value.CollectionGenericType.TypeMode == DataTypeMode.Class || value.CollectionGenericType.TypeMode == DataTypeMode.Struct)
				{
					var t = value.CollectionGenericType as WebApiData;
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
					var t = value.DictionaryKeyGenericType as WebApiData;
					if (t == null) return value;
					dk = t.HasXAMLType ? t.XAMLType : t.HasClientType ? t.ClientType : t;
				}
				if (value.DictionaryValueGenericType.TypeMode == DataTypeMode.Class || value.DictionaryValueGenericType.TypeMode == DataTypeMode.Struct)
				{
					var t = value.DictionaryValueGenericType as WebApiData;
					if (t == null) return value;
					dv = t.HasXAMLType ? t.XAMLType : t.HasClientType ? t.ClientType : t;
				}
				return new DataType(value.Name, DataTypeMode.Dictionary) { Name = value.Name, DictionaryKeyGenericType = dk, DictionaryValueGenericType = dv };
			}
			else if (value.TypeMode == DataTypeMode.Class || value.TypeMode == DataTypeMode.Struct)
			{
				var t = value as WebApiData;
				if (t == null) return value;
				return t.HasXAMLType ? t.XAMLType : t.HasClientType ? t.ClientType : t;
			}
			return value;
		}

		#endregion

	}
}