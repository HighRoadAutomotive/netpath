using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestForge.Projects
{
	public enum BuildMessageSeverity
	{
		Info,
		Warn,
		Error,
	}

	public sealed class BuildMessage
	{
		public BuildMessageSeverity Severity { get; private set; }
		public string Code { get; private set; }
		public string Description { get; private set; }
		public IProject Owner { get; private set; }
		public string ErrorObject { get; private set; }

		public BuildMessage() { }

		public BuildMessage(string Code, string Description, BuildMessageSeverity Severity, IProject Owner, object ErrorObject)
		{
			this.Code = Code;
			this.Description = Description;
			this.Severity = Severity;
			this.Owner = Owner;
			this.ErrorObject = ErrorObject.ToString();
		}
	}
}
