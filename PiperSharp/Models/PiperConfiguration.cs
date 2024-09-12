using System.Collections.Generic;

namespace PiperSharp.Models
{
    public class PiperConfiguration
    {
        public string ExecutableLocation { get; set; } = PiperDownloader.DefaultPiperExecutableLocation;
        public string WorkingDirectory { get; set; } = PiperDownloader.DefaultPiperLocation;
        public VoiceModel Model { get; set; }
        public uint SpeakerId { get; set; }
        /// <summary>
        /// The speaking rate, lower value is faster, higher value is slower
        /// </summary>
        public float SpeakingRate { get; set; } = 1f;
        public bool UseCuda { get; set; }

        public string BuildArguments()
        {
            var args = new List<string>()
            {
                "--quiet",
                "--output-raw",
                $"--model {Model.GetModelLocation()}"
            };
            if (SpeakerId > 0) args.Add($"--speaker {SpeakerId}");
            if (SpeakingRate != 1f) {
                var lengthScaleStr = SpeakingRate.ToString("0.00").Replace(',', '.');
                args.Add($"--length_scale {lengthScaleStr}");
            }
            if (UseCuda) args.Add("--use-cuda");
            return string.Join(' ', args);
        }
    }
}