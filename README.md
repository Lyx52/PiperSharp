# PiperSharp TTS

[![NuGet Version](https://img.shields.io/nuget/v/PiperSharp)](https://www.nuget.org/packages/PiperSharp)
[![dotnet](https://github.com/Lyx52/PiperSharp/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/Lyx52/PiperSharp/actions/workflows/dotnet.yml)
[![License: MIT](https://img.shields.io/github/license/Lyx52/PiperSharp)](https://github.com/Lyx52/PiperSharp/blob/master/LICENSE)

A simple C# wrapper of [piper](https://github.com/rhasspy/piper) TTS application. Similar to piper you can checkout models [here](https://rhasspy.github.io/piper-samples/) and models from [huggingface](https://huggingface.co/rhasspy/piper-voices/tree/v1.0.0), 
checkout **PiperDownloader** usage.

## Usage
See [examples](PiperSharp.Examples) for example usage or [tests](PiperSharp.Tests).
```csharp
// To download piper executable use

var cwd = Directory.GetCurrentDirectory();
await PiperDownloader.DownloadPiper().ExtractPiper(cwd); // Downloads and extracts piper to cwd/piper directory
...
// You can get list of models from hugging face using
var models = await PiperDownloader.GetHuggingFaceModelList(); // Returns a dictionary with model key as key
var model = models["ar_JO-kareem-low"];

// or you can do
var model = await PiperDownloader.GetModelByKey("ar_JO-kareem-low");
...
// Before you can use the model you need to download it using
model = await model.DownloadModel(cwd);
// Or if its downloaded you can load it from directory
model = await VoiceModel.LoadModel(modelPath);

// Now you can also do
model = await PiperDownloader.DownloadModelByKey("ar_JO-kareem-low");
// Or if its downloaded you can load it by key aswell
model = await VoiceModel.LoadModelByKey("ar_JO-kareem-low");
...
// To start generating audio use PiperProvider
var piperModel = new PiperProvider(new PiperConfiguration()
{
    ExecutableLocation = Path.Join(cwd, "piper", "piper.exe"), // Path to piper executable
    WorkingDirectory = Path.Join(cwd, "piper"), // Path to piper working directory
    Model = model, // Loaded/downloaded VoiceModel
});

// Generate audio, currently supported formats are Mp3, Wav, Raw
var result = await piperModel.InferAsync("Hello there!", AudioOutputType.Wav); // Returns byte[]
```

If you want to report bugs do it here: https://github.com/Lyx52/PiperSharp/issues
