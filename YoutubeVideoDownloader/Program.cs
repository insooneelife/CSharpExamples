using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

class Program
{
	static async Task Main(string[] args)
	{
		try
		{
			Console.WriteLine("Enter the YouTube video URL:");
			string videoUrl = Console.ReadLine();

			// Initialize YoutubeExplode client
			var youtube = new YoutubeClient();

			// Get video ID from URL
			var videoId = YoutubeExplode.Videos.VideoId.TryParse(videoUrl) ?? throw new ArgumentException("Failed to parse video URL.");

			// Get video stream manifest
			var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);

			// Select the best muxed stream (contains both audio and video)
			var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

			if (streamInfo is null)
			{
				Console.WriteLine("No suitable video stream available.");
				return;
			}

			// Download video
			Console.WriteLine("Downloading video...");
			string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string filePath = Path.Combine(exeDirectory, $"{videoId}.mp4");
			await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);
			Console.WriteLine($"Download complete. File saved as: {filePath}");
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error occurred: " + ex.Message);
		}
	}
}
