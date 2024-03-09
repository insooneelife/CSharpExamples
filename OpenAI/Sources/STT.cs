using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace OpenAI
{
	class STT
	{
		public static async Task Translation(string filePath)
		{
			string apiKey = OpenAI.Common.API_Key; 
			string apiUrl = "https://api.openai.com/v1/audio/translations";

			using (var httpClient = new HttpClient())
			{
				// Set the authorization header with your API key
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

				using (var formData = new MultipartFormDataContent())
				{
					// Read the audio file and add it to the form data
					byte[] fileBytes = File.ReadAllBytes(filePath);
					formData.Add(new ByteArrayContent(fileBytes, 0, fileBytes.Length), "file", Path.GetFileName(filePath));

					// Add the model specification to the form data
					formData.Add(new StringContent("whisper-1"), "model");

					// Post the form data to the API
					HttpResponseMessage response = await httpClient.PostAsync(apiUrl, formData);

					if (response.IsSuccessStatusCode)
					{
						// Process the response if the request was successful
						string responseContent = await response.Content.ReadAsStringAsync();
						Console.WriteLine("Translation Response: " + responseContent);
					}
					else
					{
						// Handle errors
						Console.WriteLine("Error calling the translation API: " + response.ReasonPhrase);
					}
				}
			}
		}

		public static async Task Transcription(string filePath)
		{
			string apiKey = OpenAI.Common.API_Key;
			string apiUrl = "https://api.openai.com/v1/audio/transcriptions";

			using (var httpClient = new HttpClient())
			{
				// Set the authorization header with your API key
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

				using (var formData = new MultipartFormDataContent())
				{
					// Read the audio file and add it to the form data
					byte[] fileBytes = File.ReadAllBytes(filePath);
					formData.Add(new ByteArrayContent(fileBytes, 0, fileBytes.Length), "file", Path.GetFileName(filePath));

					// Add the model specification to the form data
					formData.Add(new StringContent("whisper-1"), "model");

					formData.Add(new StringContent("ko"), "language");

					// Post the form data to the API
					HttpResponseMessage response = await httpClient.PostAsync(apiUrl, formData);

					if (response.IsSuccessStatusCode)
					{
						// Process the response if the request was successful
						string responseContent = await response.Content.ReadAsStringAsync();
						Console.WriteLine("Translation Response: " + responseContent);
					}
					else
					{
						// Handle errors
						Console.WriteLine("Error calling the translation API: " + response.ReasonPhrase);
					}
				}
			}
		}
	}
}