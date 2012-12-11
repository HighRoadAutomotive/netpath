using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETPath.Generators.WinRT.CS
{
	internal static class DocumentationGenerator
	{
		public static string GenerateDocumentation(Projects.Documentation o, bool NoReturns = false)
		{
			var tabs = o.IsClass ? "\t" : "\t\t";

			var code = new StringBuilder();
			if(!string.IsNullOrEmpty(o.Summary))
			{
				code.AppendLine(string.Format("{0}///<summary>", tabs));
				code.AppendLine(string.Format("{0}///{1}", tabs, o.Summary.Replace(Environment.NewLine, Environment.NewLine + "\t///")));
				code.AppendLine(string.Format("{0}///</summary>", tabs));
			}
			if (!string.IsNullOrEmpty(o.Remarks))
			{
				code.AppendLine(string.Format("{0}///<remarks>", tabs));
				code.AppendLine(string.Format("{0}///{1}", tabs, o.Remarks.Replace(Environment.NewLine, Environment.NewLine + "\t///")));
				code.AppendLine(string.Format("{0}///</remarks>", tabs));
			}
			if (o.IsMethod && !string.IsNullOrEmpty(o.Returns) && !NoReturns)
			{
				code.AppendLine(string.Format("{0}///<returns>", tabs));
				code.AppendLine(string.Format("{0}///{1}", tabs, o.Returns.Replace(Environment.NewLine, Environment.NewLine + "\t///")));
				code.AppendLine(string.Format("{0}///</returns>", tabs));
			}
			if (!string.IsNullOrEmpty(o.Example))
			{
				code.AppendLine(string.Format("{0}///<example>", tabs));
				code.AppendLine(string.Format("{0}///{1}", tabs, o.Example.Replace(Environment.NewLine, Environment.NewLine + "\t///")));
				code.AppendLine(string.Format("{0}///</example>", tabs));
			}
			if (o.IsProperty && !string.IsNullOrEmpty(o.Value))
			{
				code.AppendLine(string.Format("{0}///<value>", tabs));
				code.AppendLine(string.Format("{0}///{1}", tabs, o.Value.Replace(Environment.NewLine, Environment.NewLine + "\t///")));
				code.AppendLine(string.Format("{0}///</value>", tabs));
			}
			return code.ToString();
		}
	}
}