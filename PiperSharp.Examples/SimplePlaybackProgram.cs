using NAudio.Wave;
using PiperSharp.Models;

namespace PiperSharp.Examples;

public class SimplePlaybackProgram
{
    public async Task Run()
    {
        var cwd = Directory.GetCurrentDirectory();
        var piperPath = Path.Join(cwd, "piper", Environment.OSVersion.Platform == PlatformID.Win32NT ? "piper.exe" : "piper");
        if (!File.Exists(piperPath))
        {
            await PiperDownloader.DownloadPiper().ExtractPiper(cwd);    
        }
        var modelPath = Path.Join(cwd, "ar_JO-kareem-low");
        VoiceModel? model = null;
        if (!Directory.Exists(modelPath))
        {
            var models = await PiperDownloader.GetHuggingFaceModelList();
            model = await models!["ar_JO-kareem-low"].DownloadModel();
        }
        else
        {
            model = await VoiceModel.LoadModel(modelPath);
        }
        
        var consoleThread = new Thread(ConsoleThread);
        var playbackThread = new Thread(PlaybackThread);
        var provider = new PiperWaveProvider(new PiperConfiguration()
        {
            Location = piperPath,
            Model = model,
            UseCuda = false
        });
        consoleThread.Start(provider);
        playbackThread.Start(provider);
        await provider.StartAndWaitForExitAsync();
        consoleThread.Join();
    }

    public static void PlaybackThread(object? obj)
    {
        var provider = (PiperWaveProvider)obj!;
        using(var outputDevice = new WaveOutEvent())
        {
            outputDevice.Init(provider);
            outputDevice.Play();
            while (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(1000);
            }
        }
    }
    public static void ConsoleThread(object? obj)
    {
        var provider = (PiperWaveProvider)obj!;
        var input = string.Empty;
        while (input != "EXIT")
        {
            Console.Write(">>> ");
            input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) continue;
            if (input == "EXIT") break;
            provider.InferPlayback(input).GetAwaiter().GetResult();
        }
    }
}