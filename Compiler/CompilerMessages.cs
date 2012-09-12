using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WCFArchitect.Compiler
{
	internal enum CompileMessageSeverity
	{
		INFO,
		WARN,
		ERROR,
	}

	internal class CompileMessage
	{
		public string Code { get; private set; }
		public string Description { get; private set; }
		public CompileMessageSeverity Severity { get; private set; }
		[IgnoreDataMember] public object Owner { get; private set; }
		[IgnoreDataMember] public object ErrorObject { get; private set; }
		[IgnoreDataMember] public Type ErrorObjectType { get; private set; }
		public Guid OwnerID { get; private set; }
		public Guid ObjectID { get; private set; }

		public CompileMessage(string Code, string Description, CompileMessageSeverity Severity, object Owner, object ErrorObject, Type ErrorObjectType, Guid OwnerID, Guid ObjectID)
		{
			this.Code = Code;
			this.Description = Description;
			this.Severity = Severity;
			this.Owner = Owner;
			this.ErrorObject = ErrorObject;
			this.ErrorObjectType = ErrorObjectType;
			this.OwnerID = OwnerID;
			this.ObjectID = ObjectID;
		}
	}
}
