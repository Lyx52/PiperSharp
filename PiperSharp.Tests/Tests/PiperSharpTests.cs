using System.Diagnostics;
using NAudio.Wave;
using NUnit.Framework;
using PiperSharp.Models;
namespace PiperSharp.Tests.Tests;

[TestFixture]
public class PiperSharpTests
{
    [Test]
    public async Task TestDownloadPiper()
    {
        var cwd = Directory.GetCurrentDirectory();
        var piperPath = Path.Join(cwd, "piper");
        if (Directory.Exists(piperPath)) Directory.Delete(piperPath, true);
        await PiperDownloader.DownloadPiper().ExtractPiper(cwd);
        Assert.That(Directory.Exists(piperPath), "Piper doesn't exist");
        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
        {
            var process = Process.Start(new ProcessStartInfo()
            {
                FileName = "/bin/chmod",
                Arguments = $"+x {Path.Join(piperPath, "piper")}",
                UseShellExecute = false
            });
            await process!.WaitForExitAsync();
        }
    }
    
    [Test]
    public async Task TestDownloadModel()
    {
        var cwd = Directory.GetCurrentDirectory();
        const string modelName = "ar_JO-kareem-low";
        var modelPath = Path.Join(cwd, modelName);
        if (Directory.Exists(modelPath)) Directory.Delete(modelPath, true);
        var models = await PiperDownloader.GetHuggingFaceModelList();
        Assert.That(models is { Count: > 0 }, "Failed to get models from hugging face");
        var model = models![modelName];
        Assert.That(model is { Key: modelName }, "Expected model doesn't exist!");
        model = await model.DownloadModel(cwd);
        Assert.That(model.ModelLocation == modelPath && Directory.Exists(modelPath), "Model not downloaded!");
        model = await VoiceModel.LoadModel(modelPath);
        Assert.That(model is { Key: modelName }, "Failed to load model expected model!");
    }

    [Test]
    public async Task TestTTSInference()
    {
        var cwd = Directory.GetCurrentDirectory();
        const string modelName = "ar_JO-kareem-low";
        var modelPath = Path.Join(cwd, modelName);
        var piperPath = Path.Join(cwd, "piper", Environment.OSVersion.Platform == PlatformID.Win32NT ? "piper.exe" : "piper");
        var model = await VoiceModel.LoadModel(modelPath);
        var piperModel = new PiperProvider(new PiperConfiguration()
        {
            Location = piperPath,
            Model = model,
        });
        var result = await piperModel.InferAsync("Hello there!", AudioOutputType.Wav);
        Assert.That(
            result[0] == 82 &&
            result[1] == 73 &&
            result[2] == 70 &&
            result[3] == 70,
            "Expected WAV MAGIC number"
        );
        Assert.That(result.Length > 20_000, "Expected larger filesize!");
    }
    [Test]
    public async Task TestTTSInferenceWaveProvider()
    {
        var cwd = Directory.GetCurrentDirectory();
        const string modelName = "ar_JO-kareem-low";
        var modelPath = Path.Join(cwd, modelName);
        var piperPath = Path.Join(cwd, "piper", Environment.OSVersion.Platform == PlatformID.Win32NT ? "piper.exe" : "piper");
        var model = await VoiceModel.LoadModel(modelPath);
        var piperModel = new PiperWaveProvider(new PiperConfiguration()
        {
            Location = piperPath,
            Model = model,
        });
        piperModel.Start();
        await piperModel.InferPlayback("Hello there!");
        var result = new byte[19200];
        piperModel.Read(result, 0, result.Length);
        var rs = new RawSourceWaveStream(result, 0, result.Length, piperModel.WaveFormat);
        var stream = WaveFormatConversionStream.CreatePcmStream(rs);
        var testStream = new MemoryStream();
        WaveFileWriter.WriteWavFileToStream(testStream, stream);
        testStream.Seek(0, SeekOrigin.Begin);
        Assert.That(
            testStream.ReadByte() == 82 &&
            testStream.ReadByte() == 73 &&
            testStream.ReadByte() == 70 &&
            testStream.ReadByte() == 70,
            "Expected WAV MAGIC number"
        );
    }
}