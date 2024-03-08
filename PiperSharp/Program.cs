 using PiperSharp;
 using PiperSharp.Models;

 public class Program
 {
     public static async Task Main()
     {
         var model = await VoiceModel.LoadModel(
             "C:\\Users\\Lietotajs\\RiderProjects\\PiperSharp\\PiperSharp\\bin\\Debug\\net6.0\\ar_JO-kareem-low");
         var process = new PiperProcess(new PiperConfiguration()
         {
             Location = "C:\\Users\\Lietotajs\\RiderProjects\\PiperSharp\\PiperSharp\\bin\\Debug\\net6.0\\piper\\piper.exe",
             Model = model,
             OutputType = AudioOutputType.Raw,
             UseCuda = false
         });
         await process.Start();
         // var list = await PiperDownloader.GetHuggingFaceModelList();
         // var model = list!.Values.First();
         // var path = await model.DownloadModel();
     }
 }