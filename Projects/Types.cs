using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace RestForge.Projects
{
	public enum TypeMode
	{
		Void,
		Primitive,
		Enum,
		Object,
		Array,
		ByteArray,
	}

	public enum TypePrimitive
	{
		None,
		Boolean,
		Int8,
		UInt8,
		Int16,
		UInt16,
		Int32,
		UInt32,
		Int64,
		UInt64,
		Single,
		Double,
		Fixed,
		String,
		DateTime,
		Guid,
		Uri,
	}

	public interface IType
	{
		TypeMode Mode { get; }
		TypePrimitive Primitive { get; }
	}

	public abstract class TypeBase : IType
	{
		public abstract TypeMode Mode { get; }

		public abstract TypePrimitive Primitive { get; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeVoid : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Void; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.None; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeArray : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Array; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.None; } }
		[JsonProperty("arraytype")]
		public TypeBase ArrayType { get; private set; }

		public TypeArray(TypeBase arrayType)
		{
			ArrayType = arrayType;
		}
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeByteArray : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.ByteArray; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.None; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeBoolean : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Primitive; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.Boolean; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeInt8 : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Primitive; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.Int8; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeUInt8 : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Primitive; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.UInt8; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeInt16 : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Primitive; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.Int16; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeUInt16 : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Primitive; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.UInt16; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeInt32 : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Primitive; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.Int32; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeUInt32 : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Primitive; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.UInt32; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeInt64 : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Primitive; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.Int64; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeUInt64 : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Primitive; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.UInt64; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeSingle : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Primitive; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.Single; } }
	}


	[JsonObject(MemberSerialization.OptIn)]
	public class TypeDouble : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Primitive; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.Double; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeFixed : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Primitive; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.Fixed; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeString : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Primitive; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.String; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeDateTime : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Primitive; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.DateTime; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeGuid : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Primitive; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.Guid; } }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class TypeUri : TypeBase
	{
		[JsonProperty("mode")]
		public override TypeMode Mode { get { return TypeMode.Primitive; } }
		[JsonProperty("primitive")]
		public override TypePrimitive Primitive { get { return TypePrimitive.Uri; } }
	}
}
