

OpenAI.Common.LoadAPIKey();

string currentDir = Directory.GetCurrentDirectory();
string filePath = Path.Combine(currentDir, "Data", "helloKR.m4a");

await OpenAI.STT.Translation(filePath);
await OpenAI.STT.Transcription(filePath);


await OpenAI.TTS.CreateSpeech();