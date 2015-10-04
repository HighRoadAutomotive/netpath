using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
	}
}
