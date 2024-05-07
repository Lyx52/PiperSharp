using System.Text;

namespace PiperSharp.Models;

public class PiperConfiguration
{
    public string ExecutableLocation { get; set; } = PiperDownloader.DefaultPiperExecutableLocation;
    public string WorkingDirectory { get; set; } = PiperDownloader.DefaultPiperLocation;
    public VoiceModel Model { get; set; }
    public bool UseCuda { get; set; }
    public uint SpeakerId { get; set; }
    public string BuildArguments()
    {
        var args = new List<string>()
        {
            "--quiet",
            "--output-raw",
            $"--model {Model.GetModelLocation()}"
        };
        if (SpeakerId > 0) args.Add($"--speaker {SpeakerId}");
        if (UseCuda) args.Add("--use-cuda");
        return string.Join(' ', args);
    }
}