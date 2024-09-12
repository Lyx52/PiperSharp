using System.Text.Json.Serialization;

namespace PiperSharp.Models
{
    public class VoiceLanguage
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
    
        [JsonPropertyName("family")]
        public string Family { get; set; }
    
        [JsonPropertyName("name_english")]
        public string Name { get; set; }
    }
}