using PiperSharp.Models;

namespace PiperSharp.Examples;

public class SimpleProgram
{
    public static async Task Main()
             {
                 var cwd = Directory.GetCurrentDirectory();
                 var piperPath = Path.Join(cwd, "piper", "piper.exe");
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
                 
                 var process = new PiperProvider(new PiperConfiguration()
                 {
                     Location = piperPath,
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