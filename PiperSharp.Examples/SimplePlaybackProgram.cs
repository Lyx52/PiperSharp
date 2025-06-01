using NAudio.Wave;
using PiperSharp.Models;

namespace PiperSharp.Examples
{

    public class SimplePlaybackProgram
    {
        public async Task Run()
        {
            const string ModelKey = "ar_JO-kareem-low";
            if (!File.Exists(PiperDownloader.DefaultPiperExecutableLocation))
            {
                await PiperDownloader.DownloadPiper().ExtractPiper(PiperDownloader.DefaultLocation);
            }

            var modelPath = Path.Join(PiperDownloader.DefaultModelLocation, ModelKey);
            VoiceModel? model = null;
            if (Directory.Exists(modelPath))
            {
                model = await VoiceModel.LoadModelByKey(ModelKey);
            }
            else
            {
                model = await PiperDownloader.DownloadModelByKey(ModelKey);
            }

            var consoleThread = new Thread(ConsoleThread);
            var playbackThread = new Thread(PlaybackThread);
            var provider = new PiperWaveProvider(new PiperConfiguration()
            {
                Model = model,
                UseCuda = false
            });
            provider.Start();
            consoleThread.Start(provider);
            playbackThread.Start(provider);

            consoleThread.Join();
            await provider.WaitForExit();
        }

        public static void PlaybackThread(object? obj)
        {
            var provider = (PiperWaveProvider)obj!;
            using (var outputDevice = new WaveOutEvent())
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
}