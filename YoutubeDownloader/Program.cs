using System;
using System.IO;
using System.Linq;
using Tyrrrz.Extensions;
using System.Threading.Tasks;
using CliWrap;
using YoutubeExplode;
using YoutubeExplode.Models;
using YoutubeExplode.Models.MediaStreams;

namespace YoutubeDownloader
{
    public class Program
    {
        private static readonly YoutubeClient YoutubeClient = new YoutubeClient();
        private static readonly Cli FfmpegCli = new Cli("ffmpeg.exe");

        private static readonly string TempDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp");
        private static readonly string OutputDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Output");

        private static MediaStreamInfo GetBestVideoStreamInfo(VideoInfo videoInfo)
        {
            if (videoInfo.VideoStreams.Any())
                return videoInfo.VideoStreams.OrderBy(s => s.VideoQuality).ThenBy(s => s.Bitrate).Last();
            throw new Exception("No applicable media streams found for this video");
        }

        private static MediaStreamInfo GetBestAudioStreamInfo(VideoInfo videoInfo)
        {
            if (videoInfo.AudioStreams.Any())
                return videoInfo.AudioStreams.OrderBy(s => s.Bitrate).Last();
            throw new Exception("No applicable media streams found for this video");
        }

        private static async Task DownloadVideoAsync(string id)
        {
            Console.WriteLine($"Working on video [{id}]...");

            // Get video info
            var videoInfo = await YoutubeClient.GetVideoInfoAsync(id);
            string cleanTitle = videoInfo.Title.Except(Path.GetInvalidFileNameChars());
            Console.WriteLine($"{videoInfo.Title}");

            // Get best streams
            var videoStreamInfo = GetBestVideoStreamInfo(videoInfo);
            var audioStreamInfo = GetBestAudioStreamInfo(videoInfo);

            // Download streams
            Console.WriteLine("Downloading...");
            Directory.CreateDirectory(TempDirectoryPath);
            string videoStreamFileExt = videoStreamInfo.Container.GetFileExtension();
            string videoStreamFilePath = Path.Combine(TempDirectoryPath, $"VID-{Guid.NewGuid()}.{videoStreamFileExt}");
            await YoutubeClient.DownloadMediaStreamAsync(videoStreamInfo, videoStreamFilePath);
            string audioStreamFileExt = audioStreamInfo.Container.GetFileExtension();
            string audioStreamFilePath = Path.Combine(TempDirectoryPath, $"AUD-{Guid.NewGuid()}.{audioStreamFileExt}");
            await YoutubeClient.DownloadMediaStreamAsync(audioStreamInfo, audioStreamFilePath);

            // Mux streams
            Console.WriteLine("Combining...");
            Directory.CreateDirectory(OutputDirectoryPath);
            string outFilePath = Path.Combine(OutputDirectoryPath, $"{cleanTitle}.mp4");
            await FfmpegCli.ExecuteAsync($"-i \"{videoStreamFilePath}\" -i \"{audioStreamFilePath}\" -shortest \"{outFilePath}\" -y");

            // Delete temp file
            Console.WriteLine("Deleting temp files...");
            File.Delete(videoStreamFilePath);
            File.Delete(audioStreamFilePath);

            Console.WriteLine($"Downloaded video [{id}] to [{outFilePath}]");
        }

        private static async Task DownloadPlaylistAsync(string id)
        {
            Console.WriteLine($"Working on playlist [{id}]...");

            // Get playlist info
            var playlistInfo = await YoutubeClient.GetPlaylistInfoAsync(id);
            Console.WriteLine($"{playlistInfo.Title} ({playlistInfo.Videos.Count} videos)");

            // Work on the videos
            Console.WriteLine();
            foreach (var video in playlistInfo.Videos)
            {
                await DownloadVideoAsync(video.Id);
                Console.WriteLine();
            }
        }

        private static async Task MainAsync(string[] args)
        {
            foreach (string arg in args)
            {
                // Try to determine the type of the URL/ID that was given

                // Playlist ID
                if (YoutubeClient.ValidatePlaylistId(arg))
                {
                    await DownloadPlaylistAsync(arg);
                }

                // Playlist URL
                else if (YoutubeClient.TryParsePlaylistId(arg, out string playlistId))
                {
                    await DownloadPlaylistAsync(playlistId);
                }

                // Video ID
                else if (YoutubeClient.ValidateVideoId(arg))
                {
                    await DownloadVideoAsync(arg);
                }

                // Video URL
                else if (YoutubeClient.TryParseVideoId(arg, out string videoId))
                {
                    await DownloadVideoAsync(videoId);
                }

                // Unknown
                else
                {
                    throw new ArgumentException($"Unrecognized URL or ID: [{arg}]", nameof(arg));
                }

                Console.WriteLine();
            }

            Console.WriteLine("Done");
        }

        public static void Main(string[] args)
        {
            Console.Title = "Youtube Music Downloader";

            MainAsync(args).GetAwaiter().GetResult();
        }
    }

}