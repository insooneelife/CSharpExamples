using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI
{
	internal class Common
	{
		public static string API_Key = "";

		public static void LoadAPIKey()
		{
			string currentDir = Directory.GetCurrentDirectory();
			string apiKeyFilePath = Path.Combine(currentDir, "Data", "api_key.txt");
			API_Key = File.ReadAllText(apiKeyFilePath).Trim();
		}
	}
}
