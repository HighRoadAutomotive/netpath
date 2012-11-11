using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFArchitect.Projects;

namespace WCFArchitect.Generators.NET.CS
{
	internal static class DataTypeGenerator
	{
		public static string GenerateScope(DataScope o)
		{
			if (o == DataScope.Internal) return "internal";
			if (o == DataScope.Public) return "public";
			return "public";
		}

		public static string GenerateType(DataType o)
		{
			return o.ToString();
		}

		public static string GenerateTypeDeclaration(DataType o, bool ImpliedExtensionData = false, bool HasWinFormsDatabinding = false)
		{
			var sb = new StringBuilder();
			if (o.TypeMode == DataTypeMode.Class)
			{
				sb.AppendFormat("{0} {1}{2}{3}class {4}", GenerateScope(o.Scope), o.Partial ? "partial " : "", o.Abstract ? "abstract " : "", o.Sealed ? "sealed " : "", o.Name);
				if (o.InheritedTypes.Any() || ImpliedExtensionData || HasWinFormsDatabinding)
				{
					sb.Append(" : ");
					for (int i = 0; i < o.InheritedTypes.Count; i++)
					{
						sb.Append(GenerateType(o.InheritedTypes[i]));
						if (i < o.InheritedTypes.Count - 1) sb.Append(", ");
					}
					if (ImpliedExtensionData) sb.Append(o.InheritedTypes.Count > 0 ? ", System.Runtime.Serialization.IExtensibleDataObject" : "System.Runtime.Serialization.IExtensibleDataObject");
					if (HasWinFormsDatabinding) sb.Append(o.InheritedTypes.Count > 0 || ImpliedExtensionData ? ", System.ComponentModel.INotifyPropertyChanged" : "System.ComponentModel.INotifyPropertyChanged");
				}
			}
			else if (o.TypeMode == DataTypeMode.Struct)
			{
				sb.AppendFormat("{0} {1}struct {2}", GenerateScope(o.Scope), o.Partial ? "partial " : "", o.Name);
				var dataTypes = (from a in o.InheritedTypes where a.TypeMode == DataTypeMode.Interface select a).ToList();
				if (dataTypes.Any() || ImpliedExtensionData || HasWinFormsDatabinding)
				{
					sb.Append(" : ");
					for (int i = 0; i < dataTypes.Count(); i++)
					{
						sb.Append(GenerateType(dataTypes[i]));
						if (i < dataTypes.Count - 1) sb.Append(", ");
					}
					if (ImpliedExtensionData) sb.Append(o.InheritedTypes.Count > 0 ? ", System.Runtime.Serialization.IExtensibleDataObject" : "System.Runtime.Serialization.IExtensibleDataObject");
					if (HasWinFormsDatabinding) sb.Append(o.InheritedTypes.Count > 0 || ImpliedExtensionData ? ", System.ComponentModel.INotifyPropertyChanged" : "System.ComponentModel.INotifyPropertyChanged");
				}
			}
			else if (o.TypeMode == DataTypeMode.Enum)
				return string.Format("{0} enum {1}", GenerateScope(o.Scope), o.Name);
			else
				return "";
			return sb.ToString();
		}

		public static string GenerateTypeGenerics(DataType o)
		{
			if (o.TypeMode == DataTypeMode.Collection || o.TypeMode == DataTypeMode.Array)
				return GenerateType(o.CollectionGenericType);
			return o.TypeMode == DataTypeMode.Dictionary ? string.Format("{0}, {1}", GenerateType(o.DictionaryKeyGenericType), GenerateType(o.DictionaryValueGenericType)) : "";
		}
	}

	internal static class DataTypeVBGenerator
	{
		public static string GenerateScope(DataScope o, bool IsSilverlight = false)
		{
			if (IsSilverlight && o == DataScope.Public) return "Public";
			if (IsSilverlight && o != DataScope.Public) return "Friend";
			if (o == DataScope.Internal) return "Friend";
			if (o == DataScope.Public) return "Public";
			return "Public";
		}

		public static string GenerateType(DataType o)
		{
			if (o.TypeMode == DataTypeMode.Primitive)
			{
				if (o.Primitive == PrimitiveTypes.Byte) return "Byte";
				if (o.Primitive == PrimitiveTypes.SByte) return "SByte";
				if (o.Primitive == PrimitiveTypes.Short) return "Short";
				if (o.Primitive == PrimitiveTypes.Int) return "Integer";
				if (o.Primitive == PrimitiveTypes.Long) return "Long";
				if (o.Primitive == PrimitiveTypes.UShort) return "UShort";
				if (o.Primitive == PrimitiveTypes.UInt) return "UInteger";
				if (o.Primitive == PrimitiveTypes.ULong) return "ULong";
				if (o.Primitive == PrimitiveTypes.Float) return "Single";
				if (o.Primitive == PrimitiveTypes.Double) return "Double";
				if (o.Primitive == PrimitiveTypes.Decimal) return "Decimal";
				if (o.Primitive == PrimitiveTypes.Bool) return "Boolean";
				if (o.Primitive == PrimitiveTypes.Char) return "Char";
				if (o.Primitive == PrimitiveTypes.String) return "String";
				if (o.Primitive == PrimitiveTypes.DateTime) return "DateTime";
				if (o.Primitive == PrimitiveTypes.DateTimeOffset) return "DateTimeOffset";
				if (o.Primitive == PrimitiveTypes.TimeSpan) return "TimeSpan";
				if (o.Primitive == PrimitiveTypes.GUID) return "Guid";
				if (o.Primitive == PrimitiveTypes.URI) return "Uri";
				if (o.Primitive == PrimitiveTypes.Version) return "Version";
				if (o.Primitive == PrimitiveTypes.ByteArray) return "Byte()";
			}
			else if (o.TypeMode == DataTypeMode.Class || o.TypeMode == DataTypeMode.Struct || o.TypeMode == DataTypeMode.Enum)
				return string.Format("{0}.{1}", o.Parent.FullName, o.Name);
			else if (o.TypeMode == DataTypeMode.Array)
				return string.Format("{0}()", GenerateType(o.CollectionGenericType));
			else if (o.TypeMode == DataTypeMode.Collection)
				return string.Format("{0}.{1}(Of {2})", o.Parent.FullName, o.Name, GenerateType(o.CollectionGenericType));
			else if (o.TypeMode == DataTypeMode.Dictionary)
				return string.Format("{0}.{1}(Of {2}, {3})", o.Parent.FullName, o.Name, GenerateType(o.DictionaryKeyGenericType), GenerateType(o.DictionaryValueGenericType));
			else if (o.IsExternalType)
				return o.Name;
			return o.Name;
		}


		public static string GenerateTypeDeclaration(DataType o)
		{
			var sb = new StringBuilder();
			if (o.TypeMode == DataTypeMode.Class)
			{
				sb.AppendFormat("{0} {1}{2}{3}Class {4}", GenerateScope(o.Scope), o.Partial ? "Partial " : "", o.Abstract ? "MustInherit " : "", o.Sealed ? "NotInheritable " : "", o.Name);
				if (o.InheritedTypes.Any())
				{
					sb.Append(" : ");
					for (int i = 0; i < o.InheritedTypes.Count; i++)
					{
						sb.Append(GenerateType(o.InheritedTypes[i]));
						if ((i + 1) > o.InheritedTypes.Count) sb.Append(", ");
					}
				}
			}
			else if (o.TypeMode == DataTypeMode.Struct)
			{
				sb.AppendFormat("{0} {1}Structure {2}", GenerateScope(o.Scope), o.Partial ? "Partial " : "", o.Name);
				var dataTypes = (from a in o.InheritedTypes where a.TypeMode == DataTypeMode.Interface select a).ToList();
				if (dataTypes.Any())
				{
					sb.Append(" : ");
					for (int i = 0; i < dataTypes.Count; i++)
					{
						sb.Append(GenerateType(dataTypes[i]));
						if ((i + 1) > dataTypes.Count) sb.Append(", ");
					}
				}
			}
			else if (o.TypeMode == DataTypeMode.Enum)
			{
				return string.Format("{0} Enum {1}", GenerateScope(o.Scope), o.Name);
			}
			else
				return "";
			return sb.ToString();
		}

		public static string GenerateVariableDeclaration(DataType o)
		{
			if (o.TypeMode == DataTypeMode.Primitive)
			{
				if (o.Primitive == PrimitiveTypes.Byte) return " As Byte";
				if (o.Primitive == PrimitiveTypes.SByte) return " As SByte";
				if (o.Primitive == PrimitiveTypes.Short) return " As Short";
				if (o.Primitive == PrimitiveTypes.Int) return " As Integer";
				if (o.Primitive == PrimitiveTypes.Long) return " As Long";
				if (o.Primitive == PrimitiveTypes.UShort) return " As UShort";
				if (o.Primitive == PrimitiveTypes.UInt) return " As UInteger";
				if (o.Primitive == PrimitiveTypes.ULong) return " As ULong";
				if (o.Primitive == PrimitiveTypes.Float) return " As Single";
				if (o.Primitive == PrimitiveTypes.Double) return " As Double";
				if (o.Primitive == PrimitiveTypes.Decimal) return " As Decimal";
				if (o.Primitive == PrimitiveTypes.Bool) return " As Boolean";
				if (o.Primitive == PrimitiveTypes.Char) return " As Char";
				if (o.Primitive == PrimitiveTypes.String) return " As String";
				if (o.Primitive == PrimitiveTypes.DateTime) return " As DateTime";
				if (o.Primitive == PrimitiveTypes.DateTimeOffset) return " As DateTimeOffset";
				if (o.Primitive == PrimitiveTypes.TimeSpan) return " As TimeSpan";
				if (o.Primitive == PrimitiveTypes.GUID) return " As Guid";
				if (o.Primitive == PrimitiveTypes.URI) return " As Uri";
				if (o.Primitive == PrimitiveTypes.Version) return " As Version";
				if (o.Primitive == PrimitiveTypes.ByteArray) return "() As Byte";
			}
			else if (o.TypeMode == DataTypeMode.Class || o.TypeMode == DataTypeMode.Struct || o.TypeMode == DataTypeMode.Enum)
				return string.Format(" As {0}.{1}", o.Parent.FullName, o.Name);
			else if (o.TypeMode == DataTypeMode.Array)
				return string.Format("() As {0}.{1}", o.Parent.FullName, o.Name);
			else if (o.TypeMode == DataTypeMode.Collection)
				return string.Format(" As {0}(Of {1})", o.Name, GenerateType(o.CollectionGenericType));
			else if (o.TypeMode == DataTypeMode.Collection)
				return string.Format(" As {0}(Of {1}, {2})", o.Name, GenerateType(o.DictionaryKeyGenericType), GenerateType(o.DictionaryValueGenericType));
			else if (o.IsExternalType)
				return string.Format(" As {0}", o.Name);
			return o.Name;
		}
		
		public static string GenerateTypeGenerics(DataType o)
		{
			var s = new StringBuilder("(Of ");
			if (o.TypeMode == DataTypeMode.Collection)
			{
				s.Append(GenerateType(o.CollectionGenericType));
			}
			else if (o.TypeMode == DataTypeMode.Dictionary)
			{
				s.AppendFormat("{0}, {1}", GenerateType(o.DictionaryKeyGenericType), GenerateType(o.DictionaryValueGenericType));
			}
			else
				return "";
			s.Append(")");
			return s.ToString();
		}
	}
}