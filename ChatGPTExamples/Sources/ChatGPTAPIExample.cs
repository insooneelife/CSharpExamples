using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace ChatGPTAPIExamples
{

	class Program
	{
		static async Task Main(string[] args)
		{
			var httpClient = new HttpClient();
			var apiKey = "API_KEY";
			var url = "https://api.openai.com/v1/chat/completions";

			var requestData = new
			{
				model = "gpt-3.5-turbo",
				messages = new[]
				{
				new { role = "system", content = "You are a fruit expert." },
				new { role = "user", content = "I want to eat fruit. Recommend something sour." }
			}
			};

			var requestJson = JsonSerializer.Serialize(requestData);
			var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

			httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

			try
			{
				var response = await httpClient.PostAsync(url, requestContent);
				response.EnsureSuccessStatusCode();

				var responseContent = await response.Content.ReadAsStringAsync();
				var responseData = JsonSerializer.Deserialize<object>(responseContent);

				Console.WriteLine("Response data:");
				Console.WriteLine(JsonSerializer.Serialize(responseData, new JsonSerializerOptions { WriteIndented = true }));
			}
			catch (HttpRequestException e)
			{
				Console.WriteLine("\nException Caught!");
				Console.WriteLine("Message :{0} ", e.Message);
			}
		}
	}

}
