using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFArchitect.Compiler.Generators
{
	internal static class DataTypeCSGenerator
	{
		public static string GenerateScope(Projects.DataScope o, bool IsSilverlight = false)
		{
			if (IsSilverlight == true && o == Projects.DataScope.Public) return "public";
			if (IsSilverlight == true && o != Projects.DataScope.Public) return "internal";
			if (o == Projects.DataScope.Internal) return "internal";
			if (o== Projects.DataScope.Private) return "private";
			if (o== Projects.DataScope.Protected) return "protected";
			if (o== Projects.DataScope.ProtectedInternal) return "protected internal";
			if (o== Projects.DataScope.Public) return "public";
			return "private";
		}

		public static string GenerateType(Projects.DataType o)
		{
			if (o.TypeMode == Projects.DataTypeMode.Primitive)
			{
				if (o.Primitive == Projects.PrimitiveTypes.Byte) return "byte";
				if (o.Primitive == Projects.PrimitiveTypes.SByte) return "sbyte";
				if (o.Primitive == Projects.PrimitiveTypes.Short) return "short";
				if (o.Primitive == Projects.PrimitiveTypes.Int) return "int";
				if (o.Primitive == Projects.PrimitiveTypes.Long) return "long";
				if (o.Primitive == Projects.PrimitiveTypes.UShort) return "ushort";
				if (o.Primitive == Projects.PrimitiveTypes.UInt) return "uint";
				if (o.Primitive == Projects.PrimitiveTypes.ULong) return "ulong";
				if (o.Primitive == Projects.PrimitiveTypes.Float) return "float";
				if (o.Primitive == Projects.PrimitiveTypes.Double) return "double";
				if (o.Primitive == Projects.PrimitiveTypes.Decimal) return "decimal";
				if (o.Primitive == Projects.PrimitiveTypes.Bool) return "bool";
				if (o.Primitive == Projects.PrimitiveTypes.Char) return "char";
				if (o.Primitive == Projects.PrimitiveTypes.String) return "string";
				if (o.Primitive == Projects.PrimitiveTypes.DateTime) return "DateTime";
				if (o.Primitive == Projects.PrimitiveTypes.DateTimeOffset) return "DateTimeOffset";
				if (o.Primitive == Projects.PrimitiveTypes.TimeSpan) return "TimeSpan";
				if (o.Primitive == Projects.PrimitiveTypes.GUID) return "Guid";
				if (o.Primitive == Projects.PrimitiveTypes.URI) return "Uri";
				if (o.Primitive == Projects.PrimitiveTypes.Version) return "Version";
				if (o.Primitive == Projects.PrimitiveTypes.ByteArray) return "byte[]";
			}
			else if (o.TypeMode == Projects.DataTypeMode.Class || o.TypeMode == Projects.DataTypeMode.Struct || o.TypeMode == Projects.DataTypeMode.Enum)
				return string.Format("{0}.{1}", o.Parent.FullName, o.Name);
			else if (o.TypeMode == Projects.DataTypeMode.Array)
				return string.Format("{0}[]", GenerateType(o.CollectionGenericType));
			else if (o.TypeMode == Projects.DataTypeMode.Collection)
				return string.Format("{0}<{1}>", o.Name, GenerateType(o.CollectionGenericType));
			else if (o.TypeMode == Projects.DataTypeMode.Dictionary)
				return string.Format("{0}<{1}, {2}>", o.Name, GenerateType(o.DictionaryKeyGenericType), GenerateType(o.DictionaryValueGenericType));
			else if (o.TypeMode == Projects.DataTypeMode.External)
				return o.Name;
			return o.Name;
		}

		public static string GenerateTypeDeclaration(Projects.DataType o)
		{
			StringBuilder sb = new StringBuilder();
			if (o.TypeMode == Projects.DataTypeMode.Class)
			{
				sb.AppendFormat("{0}{1}{2} class {3}", GenerateScope(o.Scope), o.IsPartial ? " partial" : "", GenerateScope(o.Scope), o.IsAbstract ? " abstract" : "", o.Name);
				if (o.InheritedTypes.Count() > 0)
				{
					sb.Append(" : ");
					for (int i = 0; i < o.InheritedTypes.Count; i++)
					{
						sb.Append(GenerateType(o.InheritedTypes[i]));
						if ((i + 1) > o.InheritedTypes.Count) sb.Append(", ");
					}
				}
			}
			else if (o.TypeMode == Projects.DataTypeMode.Struct)
			{
				sb.AppendFormat("{0}{1} struct {3}", GenerateScope(o.Scope), o.IsPartial ? " partial" : "", o.Name);
				if (o.InheritedTypes.Count() > 0)
				{
					sb.Append(" : ");
					for (int i = 0; i < o.InheritedTypes.Count; i++)
					{
						sb.Append(GenerateType(o.InheritedTypes[i]));
						if ((i + 1) > o.InheritedTypes.Count) sb.Append(", ");
					}
				}
			}
			else if (o.TypeMode == Projects.DataTypeMode.Enum)
			{
				return string.Format("{0} enum {1}", GenerateScope(o.Scope), o.Name);
			}
			else
				return "";
			return sb.ToString();
		}

		public static string GenerateTypeGenerics(Projects.DataType o)
		{
			StringBuilder s = new StringBuilder("<");
			if (o.TypeMode == Projects.DataTypeMode.Collection)
			{
				s.Append(GenerateType(o.CollectionGenericType));
			}
			else if (o.TypeMode == Projects.DataTypeMode.Dictionary)
			{
				s.AppendFormat("{0}, {1}", GenerateType(o.DictionaryKeyGenericType), GenerateType(o.DictionaryValueGenericType));
			}
			else
				return "";
			s.Append(">");
			return s.ToString();
		}
	}

	internal static class DataTypeVBGenerator
	{
		public static string GenerateScope(Projects.DataScope o, bool IsSilverlight = false)
		{
			if (IsSilverlight == true && o == Projects.DataScope.Public) return "Public";
			if (IsSilverlight == true && o != Projects.DataScope.Public) return "Friend";
			if (o == Projects.DataScope.Internal) return "Friend";
			if (o == Projects.DataScope.Private) return "Private";
			if (o == Projects.DataScope.Protected) return "Protected";
			if (o == Projects.DataScope.ProtectedInternal) return "ProtectedFriend";
			if (o == Projects.DataScope.Public) return "Public";
			return "Private";
		}

		public static string GenerateType(Projects.DataType o)
		{
			if (o.TypeMode == Projects.DataTypeMode.Primitive)
			{
				if (o.Primitive == Projects.PrimitiveTypes.Byte) return "Byte";
				if (o.Primitive == Projects.PrimitiveTypes.SByte) return "SByte";
				if (o.Primitive == Projects.PrimitiveTypes.Short) return "Short";
				if (o.Primitive == Projects.PrimitiveTypes.Int) return "Integer";
				if (o.Primitive == Projects.PrimitiveTypes.Long) return "Long";
				if (o.Primitive == Projects.PrimitiveTypes.UShort) return "UShort";
				if (o.Primitive == Projects.PrimitiveTypes.UInt) return "UInteger";
				if (o.Primitive == Projects.PrimitiveTypes.ULong) return "ULong";
				if (o.Primitive == Projects.PrimitiveTypes.Float) return "Single";
				if (o.Primitive == Projects.PrimitiveTypes.Double) return "Double";
				if (o.Primitive == Projects.PrimitiveTypes.Decimal) return "Decimal";
				if (o.Primitive == Projects.PrimitiveTypes.Bool) return "Boolean";
				if (o.Primitive == Projects.PrimitiveTypes.Char) return "Char";
				if (o.Primitive == Projects.PrimitiveTypes.String) return "String";
				if (o.Primitive == Projects.PrimitiveTypes.DateTime) return "DateTime";
				if (o.Primitive == Projects.PrimitiveTypes.DateTimeOffset) return "DateTimeOffset";
				if (o.Primitive == Projects.PrimitiveTypes.TimeSpan) return "TimeSpan";
				if (o.Primitive == Projects.PrimitiveTypes.GUID) return "Guid";
				if (o.Primitive == Projects.PrimitiveTypes.URI) return "Uri";
				if (o.Primitive == Projects.PrimitiveTypes.Version) return "Version";
				if (o.Primitive == Projects.PrimitiveTypes.ByteArray) return "Byte()";
			}
			else if (o.TypeMode == Projects.DataTypeMode.Class || o.TypeMode == Projects.DataTypeMode.Struct || o.TypeMode == Projects.DataTypeMode.Enum)
				return string.Format("{0}.{1}", o.Parent.FullName, o.Name);
			else if (o.TypeMode == Projects.DataTypeMode.Array)
				return string.Format("{0}()", GenerateType(o.CollectionGenericType));
			else if (o.TypeMode == Projects.DataTypeMode.Collection)
				return string.Format("{0}.{1}(Of {2})", o.Parent.FullName, o.Name, GenerateType(o.CollectionGenericType));
			else if (o.TypeMode == Projects.DataTypeMode.Dictionary)
				return string.Format("{0}.{1}(Of {2}, {3})", o.Parent.FullName, o.Name, GenerateType(o.DictionaryKeyGenericType), GenerateType(o.DictionaryValueGenericType));
			else if (o.TypeMode == Projects.DataTypeMode.External)
				return o.Name;
			return o.Name;
		}


		public static string GenerateTypeDeclaration(Projects.DataType o)
		{
			StringBuilder sb = new StringBuilder();
			if (o.TypeMode == Projects.DataTypeMode.Class)
			{
				sb.AppendFormat("{0}{1}{2} Class {3}", GenerateScope(o.Scope), o.IsPartial ? " Partial" : "", GenerateScope(o.Scope), o.IsAbstract ? " MustInherit" : "", o.Name);
				if (o.InheritedTypes.Count() > 0)
				{
					sb.Append(" : ");
					for (int i = 0; i < o.InheritedTypes.Count; i++)
					{
						sb.Append(GenerateType(o.InheritedTypes[i]));
						if ((i + 1) > o.InheritedTypes.Count) sb.Append(", ");
					}
				}
			}
			else if (o.TypeMode == Projects.DataTypeMode.Struct)
			{
				sb.AppendFormat("{0}{1} Structure {3}", GenerateScope(o.Scope), o.IsPartial ? " Partial" : "", o.Name);
				if (o.InheritedTypes.Count() > 0)
				{
					sb.Append(" : ");
					for (int i = 0; i < o.InheritedTypes.Count; i++)
					{
						sb.Append(GenerateType(o.InheritedTypes[i]));
						if ((i + 1) > o.InheritedTypes.Count) sb.Append(", ");
					}
				}
			}
			else if (o.TypeMode == Projects.DataTypeMode.Enum)
			{
				return string.Format("{0} Enum {1}", GenerateScope(o.Scope), o.Name);
			}
			else
				return "";
			return sb.ToString();
		}

		public static string GenerateVariableDeclaration(Projects.DataType o)
		{
			if (o.TypeMode == Projects.DataTypeMode.Primitive)
			{
				if (o.Primitive == Projects.PrimitiveTypes.Byte) return " As Byte";
				if (o.Primitive == Projects.PrimitiveTypes.SByte) return " As SByte";
				if (o.Primitive == Projects.PrimitiveTypes.Short) return " As Short";
				if (o.Primitive == Projects.PrimitiveTypes.Int) return " As Integer";
				if (o.Primitive == Projects.PrimitiveTypes.Long) return " As Long";
				if (o.Primitive == Projects.PrimitiveTypes.UShort) return " As UShort";
				if (o.Primitive == Projects.PrimitiveTypes.UInt) return " As UInteger";
				if (o.Primitive == Projects.PrimitiveTypes.ULong) return " As ULong";
				if (o.Primitive == Projects.PrimitiveTypes.Float) return " As Single";
				if (o.Primitive == Projects.PrimitiveTypes.Double) return " As Double";
				if (o.Primitive == Projects.PrimitiveTypes.Decimal) return " As Decimal";
				if (o.Primitive == Projects.PrimitiveTypes.Bool) return " As Boolean";
				if (o.Primitive == Projects.PrimitiveTypes.Char) return " As Char";
				if (o.Primitive == Projects.PrimitiveTypes.String) return " As String";
				if (o.Primitive == Projects.PrimitiveTypes.DateTime) return " As DateTime";
				if (o.Primitive == Projects.PrimitiveTypes.DateTimeOffset) return " As DateTimeOffset";
				if (o.Primitive == Projects.PrimitiveTypes.TimeSpan) return " As TimeSpan";
				if (o.Primitive == Projects.PrimitiveTypes.GUID) return " As Guid";
				if (o.Primitive == Projects.PrimitiveTypes.URI) return " As Uri";
				if (o.Primitive == Projects.PrimitiveTypes.Version) return " As Version";
				if (o.Primitive == Projects.PrimitiveTypes.ByteArray) return "() As Byte";
			}
			else if (o.TypeMode == Projects.DataTypeMode.Class || o.TypeMode == Projects.DataTypeMode.Struct || o.TypeMode == Projects.DataTypeMode.Enum)
				return string.Format(" As {0}.{1}", o.Parent.FullName, o.Name);
			else if (o.TypeMode == Projects.DataTypeMode.Array)
				return string.Format("() As {0}.{1}", o.Parent.FullName, o.Name);
			else if (o.TypeMode == Projects.DataTypeMode.Collection)
				return string.Format("As {0}(Of {2})", o.Name, GenerateType(o.CollectionGenericType));
			else if (o.TypeMode == Projects.DataTypeMode.Collection)
				return string.Format("As {0}(Of {1}, {2})", o.Name, GenerateType(o.DictionaryKeyGenericType), GenerateType(o.DictionaryValueGenericType));
			else if (o.TypeMode == Projects.DataTypeMode.External)
				return o.Name;
			return o.Name;
		}
		
		public static string GenerateTypeGenerics(Projects.DataType o)
		{
			StringBuilder s = new StringBuilder("(Of ");
			if (o.TypeMode == Projects.DataTypeMode.Collection)
			{
				s.Append(GenerateType(o.CollectionGenericType));
			}
			else if (o.TypeMode == Projects.DataTypeMode.Dictionary)
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