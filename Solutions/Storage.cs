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
				var sol = JsonConvert.DeserializeObject<Solution>(json);
				sol.Profile = await LoadUserProfile(path);
				return sol;
			}
		}

		public static async Task Save(string path, Solution solution)
		{
			await SaveUserProfile(path, solution.Profile);
			var json = JsonConvert.SerializeObject(solution);
			using (var fs = File.Open(path, FileMode.OpenOrCreate))
			using (var sw = new StreamWriter(fs))
			{
				await sw.WriteAsync(json);
				await sw.FlushAsync();
				sw.Close();
			}
		}

		private static async Task<UserProfile> LoadUserProfile(string path)
		{
			path = path + ".user";
			if (!File.Exists(path))
				return new UserProfile();

			using (var fs = File.OpenRead(path))
			using (var sr = new StreamReader(fs))
			{
				var json = await sr.ReadToEndAsync();
				IsSolutionMinimumVersion(json);
				return JsonConvert.DeserializeObject<UserProfile>(json);
			}
		}

		private static async Task SaveUserProfile(string path, UserProfile profile)
		{
			var json = JsonConvert.SerializeObject(profile);
			using (var fs = File.Open(path + ".user", FileMode.OpenOrCreate))
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
