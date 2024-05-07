using PiperSharp.Models;

namespace PiperSharp.Examples;

public class SimpleSaveFileProgram
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

        var process = new PiperProvider(new PiperConfiguration()
        {
            Model = model,
            UseCuda = false
        });
        consoleThread.Start(process);
        consoleThread.Join();
    }

    public static void ConsoleThread(object? obj)
    {
        var process = (PiperProvider)obj!;
        var input = string.Empty;
        while (input != "EXIT")
        {
            Console.Write(">>> ");
            input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) continue;
            if (input == "EXIT") break;
            var data = process.InferAsync(input, AudioOutputType.Wav).GetAwaiter().GetResult();
            var fs = File.OpenWrite("audiofile.wav");
            fs.Write(data);
            fs.Flush();
            fs.Close();
            Console.WriteLine("File saved to audiofile.wav");
        }
    }
}