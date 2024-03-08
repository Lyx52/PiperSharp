﻿using System.Text.Json;
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
    
    [JsonIgnore]
    public string? ModelLocation { get; set; }

    public string GetModelLocation()
    {
        if (ModelLocation is null) throw new FileNotFoundException("Model not downloaded!");
        var modelFileName = Path.GetFileName(Files.Keys.FirstOrDefault(f => f.EndsWith(".onnx")));
        return Path.Join(ModelLocation, modelFileName);
    }
    
    public static async Task<VoiceModel> LoadModel(string directory)
    {
        if (!Directory.Exists(directory)) throw new DirectoryNotFoundException("Model directory not found!");
        var modelInfoFile = Path.Join(directory, "model.json");
        if (!File.Exists(modelInfoFile)) throw new FileNotFoundException("model.json file not found!");
        var fs = File.OpenRead(modelInfoFile);
        var model = await JsonSerializer.DeserializeAsync<VoiceModel>(fs);
        if (model is null) throw new ApplicationException("Could not parse model.json file!");
        model.ModelLocation = directory;
        return model;
    }
}