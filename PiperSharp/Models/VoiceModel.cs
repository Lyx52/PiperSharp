using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PiperSharp.Models;


public class VoiceModel
{
    [JsonPropertyName("key")]
    public string Key { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public VoiceQuality Quality { get; set; }
    
    [JsonPropertyName("num_speakers")]
    public int NumSpeakers { get; set; }
    
    [JsonPropertyName("speaker_id_map")]
    public Dictionary<string, int> SpeakerIdMap { get; set; }
    
    [JsonPropertyName("files")]
    public Dictionary<string, dynamic> Files { get; set; }
    
    [JsonPropertyName("aliases")]
    public string[] Aliases { get; set; }
    
    [JsonPropertyName("language")]
    public VoiceLanguage Language { get; set; }
}