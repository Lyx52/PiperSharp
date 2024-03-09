using System.Diagnostics;
using NAudio.Wave;
using PiperSharp.Models;

namespace PiperSharp;

public class PiperProvider
{
    public PiperConfiguration Configuration { get; set; }
    public PiperProvider(PiperConfiguration configuration)
    {
        Configuration = configuration;
    }

    public static Process ConfigureProcess(PiperConfiguration configuration)
    {
        return new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = configuration.Location,
                Arguments = configuration.BuildArguments(),
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(configuration.Location)
            },
        };
    }
    
    public async Task<byte[]> InferAsync(string text, AudioOutputType outputType = AudioOutputType.Wav, CancellationToken token = default(CancellationToken))
    {
        var process = ConfigureProcess(Configuration);
        process.Start();
        await process.StandardInput.WriteLineAsync(text);
        await process.StandardInput.FlushAsync();
        process.StandardInput.Close();
        using var ms = new MemoryStream();
        await process.StandardOutput.BaseStream.CopyToAsync(ms, token);
        await process.WaitForExitAsync(token);
        ms.Seek(0, SeekOrigin.Begin);
        
        await using var fs = new RawSourceWaveStream(ms, new WaveFormat((int)(Configuration.Model.Audio?.SampleRate ?? 16000), 1));
        return await ConvertToArray(fs, outputType, token);
    }

    private async Task<byte[]> ConvertToArray(RawSourceWaveStream stream, AudioOutputType outputType, CancellationToken token)
    {
        using var output = new MemoryStream();
        switch (outputType)
        {
            case AudioOutputType.Mp3:
            {
                await stream.FlushAsync(token);
                MediaFoundationEncoder.EncodeToMp3(stream, output);
            } break;
            case AudioOutputType.Raw:
            {
                await stream.CopyToAsync(output, token);
                await stream.FlushAsync(token);
            } break;
            case AudioOutputType.Wav:
            default:
            {
                var waveStream = new WaveFileWriter(output, stream.WaveFormat);
                await stream.CopyToAsync(waveStream, token);
                await stream.FlushAsync(token);
            } break;
        }
        await output.FlushAsync(token);
        return output.ToArray();
    }
}