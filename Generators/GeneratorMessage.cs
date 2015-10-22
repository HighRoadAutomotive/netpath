using RestForge.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestForge.Generators
{
	public enum GeneratorMessageSeverity
	{
		Info,
		Warn,
		Error,
	}

	public sealed class GeneratorMessage
	{
		public GeneratorMessageSeverity Severity { get; private set; }
		public string Code { get; private set; }
		public string Description { get; private set; }
		public IProject Owner { get; private set; }
		public string ErrorObject { get; private set; }

		public GeneratorMessage() { }

		public GeneratorMessage(string Code, string Description, GeneratorMessageSeverity Severity, IProject Owner, object ErrorObject)
		{
			this.Code = Code;
			this.Description = Description;
			this.Severity = Severity;
			this.Owner = Owner;
			this.ErrorObject = ErrorObject.ToString();
		}
	}
}
