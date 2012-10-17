using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFArchitect.Compiler.Generators
{
	internal static class DocumentationCSGenerator
	{
		public static string GenerateDocumentation(Projects.Documentation o)
		{
			var tabs = o.IsClass ? "\t" : "\t\t";

			var code = new StringBuilder();
			if(string.IsNullOrEmpty(o.Summary))
			{
				code.AppendLine(string.Format("{0}///<summary>", tabs));
				code.AppendLine(o.Summary.Replace(Environment.NewLine, Environment.NewLine + "\t///"));
				code.AppendLine(string.Format("{0}///</summary>", tabs));
			}
			if (string.IsNullOrEmpty(o.Remarks))
			{
				code.AppendLine(string.Format("{0}///<remarks>", tabs));
				code.AppendLine(o.Remarks.Replace(Environment.NewLine, Environment.NewLine + "\t///"));
				code.AppendLine(string.Format("{0}///</remarks>", tabs));
			}
			if (string.IsNullOrEmpty(o.Returns))
			{
				code.AppendLine(string.Format("{0}///<returns>", tabs));
				code.AppendLine(o.Returns.Replace(Environment.NewLine, Environment.NewLine + "\t///"));
				code.AppendLine(string.Format("{0}///</returns>", tabs));
			}
			if (string.IsNullOrEmpty(o.Example))
			{
				code.AppendLine(string.Format("{0}///<example>", tabs));
				code.AppendLine(o.Example.Replace(Environment.NewLine, Environment.NewLine + "\t///"));
				code.AppendLine(string.Format("{0}///</example>", tabs));
			}
			if (string.IsNullOrEmpty(o.Value))
			{
				code.AppendLine(string.Format("{0}///<value>", tabs));
				code.AppendLine(o.Value.Replace(Environment.NewLine, Environment.NewLine + "\t///"));
				code.AppendLine(string.Format("{0}///</value>", tabs));
			}
			return code.ToString();
		}
	}
}