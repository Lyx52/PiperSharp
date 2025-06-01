using PiperSharp.Models;

namespace PiperSharp.TestApplication
{

    public class Program
    {
        public static async Task Main()
        {
            if (!Directory.Exists(PiperDownloader.DefaultPiperLocation))
                await PiperDownloader.DownloadPiper().ExtractPiper();
            string text = "Âñåì ïðèâåò, ýòî òåñò ìîåé ïðîãðàììû";
            VoiceModel model;
            try
            {
                model = await VoiceModel.LoadModelByKey("ru_RU-dmitri-medium");
            }
            catch
            {
                model = await PiperDownloader.DownloadModelByKey("ru_RU-dmitri-medium");
            }


            var piperModel = new PiperProvider(new PiperConfiguration()
            {
                Model = model,
            });
            var result = await piperModel.InferAsync(text, AudioOutputType.Wav); // Returns byte[]
            var fs = File.OpenWrite("audiofile.wav");
            fs.Write(result);
            fs.Flush();
            fs.Close();
        }
    }
}