using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace RestForge.Solutions
{
	public static class Storage
	{
		public static async Task<Solution> Open(string path)
		{
			using(var fs = File.OpenRead(path))
			using (var sr = new StreamReader(fs))
			{
				var json = await sr.ReadToEndAsync();
				IsSolutionMinimumVersion(json);
				return await Task.Run(() => JsonConvert.DeserializeObject<Solution>(json));
			}
		}

		public static async Task Save(string path, Solution solution)
		{
			var json = await Task.Run(() => JsonConvert.SerializeObject(solution));
			using (var fs = File.Open(path, FileMode.OpenOrCreate))
			using (var sw = new StreamWriter(fs))
			{
				await sw.WriteAsync(json);
				await sw.FlushAsync();
				sw.Close();
			}
		}

		private static void IsSolutionMinimumVersion(string json)
		{
			var j = JObject.Parse(json);
			var sv = j.GetValue("solutionVersion").Value<int>();
			var mv = j.GetValue("minimumVersion").Value<int>();
			if (mv < Solution.MinimumVersion || sv < Solution.MinimumVersion)
				throw new SolutionMinimumVersionException();
		}
	}

	public class SolutionMinimumVersionException : Exception
	{
		public SolutionMinimumVersionException()
			: base($"Unable to load the solution. This solution version is no longer supported by RestForge. The minimum solution version is {Solution.MinimumVersion}.")
		{
		}
	}
}
