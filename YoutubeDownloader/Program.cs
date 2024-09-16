using System.Diagnostics;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

class Program
{
    static async Task Main(string[] args)
    {
        var youtube = new YoutubeClient();
        string rootDirectory = Path.Combine(Environment.CurrentDirectory, "Downloads");

        if (!Directory.Exists(rootDirectory))
        {
            Directory.CreateDirectory(rootDirectory);
            Console.WriteLine("Pasta 'Downloads' criada na raiz do projeto.");
        }


        Console.WriteLine("Digite o link do vídeo do YouTube:");
        string videoUrl = Console.ReadLine();

        try
        {
            var video = await youtube.Videos.GetAsync(videoUrl);
            Console.WriteLine($"Encontrado: {video.Title}");

            Console.WriteLine("Escolha uma opção:");
            Console.WriteLine("1 - Baixar vídeo");
            Console.WriteLine("2 - Baixar áudio (MP3)");
            string choice = Console.ReadLine();

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            var videoStreamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

            string sanitizedTitle = video.Title.Replace(" ", "_").Replace(":", "-"); 
            var videoPath = Path.Combine(rootDirectory, $"{sanitizedTitle}.mp4");
            var audioPath = Path.Combine(rootDirectory, $"{sanitizedTitle}.mp3");

            if (choice == "1")
            {
                if (videoStreamInfo != null)
                {
                    Console.WriteLine("Baixando vídeo...");
                    await DownloadWithProgressAsync(youtube, videoStreamInfo, videoPath);
                    Console.WriteLine($"\nVídeo salvo em: {videoPath}");
                }
            }
            else if (choice == "2")
            {
                if (audioStreamInfo != null)
                {
                    Console.WriteLine("Baixando áudio...");
                    var tempAudioPath = Path.Combine(rootDirectory, $"{sanitizedTitle}.m4a");
                    await DownloadWithProgressAsync(youtube, audioStreamInfo, tempAudioPath);
                    Console.WriteLine($"\nÁudio salvo temporariamente em: {tempAudioPath}");

                    Console.WriteLine("Convertendo para MP3...");
                    ConvertToMp3(tempAudioPath, audioPath);

                    File.Delete(tempAudioPath);
                    Console.WriteLine($"Áudio MP3 salvo em: {audioPath}");
                }
            }
            else
            {
                Console.WriteLine("Opção inválida.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
    static async Task DownloadWithProgressAsync(YoutubeClient youtube, IStreamInfo streamInfo, string filePath)
    {
        var progress = new Progress<double>(p =>
        {
            Console.Write($"\rProgresso: [{GetProgressBar(p)}] {p:P0}");
        });

        await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath, progress);
    }

    static string GetProgressBar(double progress)
    {
        int totalBlocks = 30; 
        int filledBlocks = (int)(progress * totalBlocks);
        return new string('=', filledBlocks) + new string('-', totalBlocks - filledBlocks);
    }

    static void ConvertToMp3(string inputPath, string outputPath)
    {
        try
        {
            var ffmpegPath = "ffmpeg";  
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = $"-i \"{inputPath}\" \"{outputPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
            if (process.ExitCode == 0)
            {
                Console.WriteLine("Conversão concluída com sucesso!");
            }
            else
            {
                Console.WriteLine("Erro durante a conversão.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao executar o FFmpeg: {ex.Message}");
        }
    }
}
