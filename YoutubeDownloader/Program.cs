using System.Diagnostics;
using YoutubeExplode;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Videos.Streams;

class Program
{
    static async Task Main(string[] args)
    {
        var youtube = new YoutubeClient();
        string rootDirectory = Path.Combine(Environment.CurrentDirectory, "Downloads");

        Directory.CreateDirectory(rootDirectory);  // Cria o diretório se não existir
        Console.WriteLine("Pasta 'Downloads' criada/verificada na raiz do projeto.");

        bool continueDownloading = true;

        while (continueDownloading)
        {
            Console.WriteLine("\nDigite o link do vídeo do YouTube:");
            string videoUrl = Console.ReadLine();

            // Validação de URL
            if (!Uri.TryCreate(videoUrl, UriKind.Absolute, out var uri) || string.IsNullOrEmpty(videoUrl))
            {
                Console.WriteLine("URL inválida.");
                continue;
            }

            try
            {
                var video = await youtube.Videos.GetAsync(videoUrl);
                Console.WriteLine($"\nEncontrado: {video.Title}");

                Console.WriteLine("Escolha uma opção:");
                Console.WriteLine("1 - Baixar vídeo");
                Console.WriteLine("2 - Baixar áudio (MP3)");
                string choice = Console.ReadLine();

                if (choice != "1" && choice != "2")
                {
                    Console.WriteLine("Escolha inválida.");
                    continue;
                }

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                var videoStreamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

                string sanitizedTitle = SanitizeFileName(video.Title);
                var videoPath = Path.Combine(rootDirectory, $"{sanitizedTitle}.mp4");
                var audioPath = Path.Combine(rootDirectory, $"{sanitizedTitle}.mp3");

                if (choice == "1" && videoStreamInfo != null)
                {
                    await DownloadAndHandleVideoAsync(youtube, videoStreamInfo, videoPath);
                }
                else if (choice == "2" && audioStreamInfo != null)
                {
                    await DownloadAndHandleAudioAsync(youtube, audioStreamInfo, rootDirectory, sanitizedTitle, audioPath);
                }

                // Confirmação de download concluído
                Console.WriteLine("\nDownload concluído com sucesso!");

            }
            catch (YoutubeExplodeException ex)
            {
                Console.WriteLine($"Erro ao acessar o vídeo: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }

            // Pergunta se o usuário quer baixar outro vídeo
            Console.WriteLine("\nDeseja baixar outro vídeo? (S/N)");
            string response = Console.ReadLine().Trim().ToUpper();

            if (response != "S")
            {
                continueDownloading = false;
            }
        }

        Console.WriteLine("Encerrando o programa. Obrigado por usar o downloader!");
    }

    static async Task DownloadAndHandleVideoAsync(YoutubeClient youtube, IStreamInfo videoStreamInfo, string videoPath)
    {
        Console.WriteLine("Baixando vídeo...");
        await DownloadWithProgressAsync(youtube, videoStreamInfo, videoPath);
        Console.WriteLine($"\nVídeo salvo em: {videoPath}");
    }

    static async Task DownloadAndHandleAudioAsync(YoutubeClient youtube, IStreamInfo audioStreamInfo, string rootDirectory, string sanitizedTitle, string audioPath)
    {
        var tempAudioPath = Path.Combine(rootDirectory, $"{sanitizedTitle}.m4a");
        Console.WriteLine("Baixando áudio...");
        await DownloadWithProgressAsync(youtube, audioStreamInfo, tempAudioPath);
        Console.WriteLine($"\nÁudio salvo temporariamente em: {tempAudioPath}");

        Console.WriteLine("Convertendo para MP3...");
        await ConvertToMp3Async(tempAudioPath, audioPath);

        File.Delete(tempAudioPath);
        Console.WriteLine($"Áudio MP3 salvo em: {audioPath}");
    }

    static async Task DownloadWithProgressAsync(YoutubeClient youtube, IStreamInfo streamInfo, string filePath)
    {
        var totalSize = streamInfo.Size.Bytes;
        var progress = new Progress<double>(p =>
        {
            long downloadedBytes = (long)(p * totalSize);
            Console.Write($"\rProgresso: [{GetProgressBar(p)}] {p:P0} ({downloadedBytes / 1024 / 1024}MB / {totalSize / 1024 / 1024}MB)");
        });

        await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath, progress);
    }

    static string GetProgressBar(double progress)
    {
        int totalBlocks = 30;
        int filledBlocks = (int)(progress * totalBlocks);
        return new string('=', filledBlocks) + new string('-', totalBlocks - filledBlocks);
    }

    static string SanitizeFileName(string name)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalidChar, '_');
        }
        return name;
    }

    static async Task ConvertToMp3Async(string inputPath, string outputPath)
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
            await process.WaitForExitAsync();

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
