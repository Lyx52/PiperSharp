 using PiperSharp;

 public class Program
 {
     public static async Task Main()
     {
         var list = await PiperDownloader.GetHuggingFaceModelList();
         var model = list!.Values.First();
         var path = await model.DownloadModel();
     }
 }