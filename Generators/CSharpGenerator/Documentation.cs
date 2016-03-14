using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETPath.Generators.CS
{
	internal static class DocumentationGenerator
	{
		public static string GenerateDocumentation(Projects.Documentation o, bool NoReturns = false)
		{
			var tabs = o.IsClass ? "\t" : "\t\t";

			var code = new StringBuilder();
			if(!string.IsNullOrEmpty(o.Summary))
			{
				code.AppendLine($"{tabs}///<summary>");
				code.AppendLine($"{tabs}///{o.Summary.Replace(Environment.NewLine, Environment.NewLine + "\t///")}");
				code.AppendLine($"{tabs}///</summary>");
			}
			if (!string.IsNullOrEmpty(o.Remarks))
			{
				code.AppendLine($"{tabs}///<remarks>");
				code.AppendLine($"{tabs}///{o.Remarks.Replace(Environment.NewLine, Environment.NewLine + "\t///")}");
				code.AppendLine($"{tabs}///</remarks>");
			}
			if (o.IsMethod && !string.IsNullOrEmpty(o.Returns) && !NoReturns)
			{
				code.AppendLine($"{tabs}///<returns>");
				code.AppendLine($"{tabs}///{o.Returns.Replace(Environment.NewLine, Environment.NewLine + "\t///")}");
				code.AppendLine($"{tabs}///</returns>");
			}
			if (!string.IsNullOrEmpty(o.Example))
			{
				code.AppendLine($"{tabs}///<example>");
				code.AppendLine($"{tabs}///{o.Example.Replace(Environment.NewLine, Environment.NewLine + "\t///")}");
				code.AppendLine($"{tabs}///</example>");
			}
			if (o.IsProperty && !string.IsNullOrEmpty(o.Value))
			{
				code.AppendLine($"{tabs}///<value>");
				code.AppendLine($"{tabs}///{o.Value.Replace(Environment.NewLine, Environment.NewLine + "\t///")}");
				code.AppendLine($"{tabs}///</value>");
			}
			return code.ToString();
		}
	}
}