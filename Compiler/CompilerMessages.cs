using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFArchitect.Compiler
{
	internal enum CompilerMessageStage
	{
		Verify,
		Server,
		Proxy,
		Client,
		Output,
		Complete,
	}

	internal enum CompileMessageSeverity
	{
		Message,
		Warning,
		Error,
	}

	internal class CompileMessage
	{
		public string Code { get; private set; }
		public string Description { get; private set; }
		public CompileMessageSeverity Severity { get; private set; }
		public string Line { get; private set; }
		public object Owner { get; private set; }
		public object ErrorObject { get; private set; }
		public Type ErrorObjectType { get; private set; }

		public CompileMessage(string Code, string Description, CompileMessageSeverity Severity, object Owner, object ErrorObject, Type ErrorObjectType, string Line = "")
		{
			this.Code = Code;
			this.Description = Description;
			this.Severity = Severity;
			this.Line = Line;
			this.Owner = Owner;
			this.ErrorObject = ErrorObject;
			this.ErrorObjectType = ErrorObjectType;
		}
	}
}
