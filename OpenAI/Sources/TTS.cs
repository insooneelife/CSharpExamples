using System;
using System.Collections.Immutable;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace OpenAI
{
	class TTS
	{
		public static async Task CreateSpeech()
		{
			string apiKey = Common.API_Key;

			string outputPath = "speech.mp3";
			string apiUrl = "https://api.openai.com/v1/audio/speech";

			var ttsRequest = new
			{
				model = "tts-1",
				input = "The quick brown fox jumped over the lazy dog.",
				voice = "alloy"
			};

			string requestBody = JsonConvert.SerializeObject(ttsRequest);

			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				using (var content = new StringContent(requestBody, Encoding.UTF8, "application/json"))
				{
					HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);
					if (response.IsSuccessStatusCode)
					{
						byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
						await System.IO.File.WriteAllBytesAsync(outputPath, responseBytes);
						Console.WriteLine($"Speech synthesis successful. Output saved to {outputPath}");
					}
					else
					{
						string errorMessage = await response.Content.ReadAsStringAsync();
						Console.WriteLine($"Error calling TTS API: {response.StatusCode} - {errorMessage}");
					}
				}
			}
		}
	}
}