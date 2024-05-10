using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class Ollama
{
	public static async Task ExampleMistral()
	{
		var baseUrl = "http://localhost:11434/api/chat";
		var jsonGenerateContent = @"{
            ""model"": ""mistral"",
            ""messages"": [
                { ""role"": ""user"", ""content"": ""why is the sky blue?"" }
            ]
        }";

		var jsonChatContent = @"{
			""model"": ""mistral"",
			""stream"": false,
			""role"": ""한국어를 사용한다."",
			""messages"": [
				{ ""role"": ""user"", ""content"": ""안녕하세요. 자기소개좀 해주세요."" }
			]
		}";

		using (var httpClient = new HttpClient())
		{
			var content = new StringContent(jsonChatContent, Encoding.UTF8, "application/json");
			var response = await httpClient.PostAsync(baseUrl, content);

			if (response.IsSuccessStatusCode)
			{
				var responseContent = await response.Content.ReadAsStringAsync();
				Console.WriteLine(responseContent);
			}
			else
			{
				Console.WriteLine($"Failed to send request: {response.StatusCode}");
			}
		}
	}
}


