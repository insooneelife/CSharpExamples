

STT.WhisperAPI.LoadAPIKey();

string currentDir = Directory.GetCurrentDirectory();
string filePath = Path.Combine(currentDir, "Data", "helloKR.m4a");

await STT.WhisperAPI.Translation(filePath);
await STT.WhisperAPI.Transcription(filePath);