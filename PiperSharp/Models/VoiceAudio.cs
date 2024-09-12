using System.Text.Json.Serialization;

namespace PiperSharp.Models
{
    public class VoiceAudio
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("quality")]
        public VoiceQuality Quality { get; set; }
    
        [JsonPropertyName("sample_rate")]
        public uint SampleRate { get; set; }
    }
}