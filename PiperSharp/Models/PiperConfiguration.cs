using System.Text;

namespace PiperSharp.Models;

public class PiperConfiguration
{
    public string Location { get; set; }
    public VoiceModel Model { get; set; }
    public AudioOutputType OutputType { get; set; }
    public bool UseCuda { get; set; }
    public uint SpeakerId { get; set; }
    public string BuildArguments()
    {
        var sb = new StringBuilder();
        sb.Append($"--model {Model.GetModelLocation()} ");
        switch (OutputType)
        {
            case AudioOutputType.Raw: 
                sb.Append("--output-raw "); 
            break;
            case AudioOutputType.Wav: 
            default:
                sb.Append("--output_file - ");
            break;
        }
        if (SpeakerId > 0) sb.Append($"--speaker {SpeakerId}");
        if (UseCuda) sb.Append("--use-cuda ");
        sb.Append("--quiet ");
        return sb.ToString();
    }
}