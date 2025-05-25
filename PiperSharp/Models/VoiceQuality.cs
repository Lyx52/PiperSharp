using System.Text.Json.Serialization;

namespace PiperSharp.Models
{
    public enum VoiceQuality
    {
        [JsonStringEnumMemberName("x_low")]
        XLow,
        Low,
        Medium,
        High
    }
}