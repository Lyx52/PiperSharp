using System.Diagnostics;
using NAudio.Wave;
using PiperSharp.Models;

namespace PiperSharp;

public class PiperWaveProvider : IWaveProvider
{
    public PiperConfiguration Configuration { get; set; }
    public bool Started { get; private set; } = false;
    private Process _process;
    private RawSourceWaveStream? _internalAudioStream;

    public PiperWaveProvider(PiperConfiguration configuration)
    {
        Configuration = configuration;
        _process = new Process()
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
        WaveFormat = new WaveFormat((int)(configuration.Model.Audio?.SampleRate ?? 16000), 1);
    }

    public Task StartAndWaitForExitAsync(CancellationToken token = default(CancellationToken))
    {
        _process.Start();
        _internalAudioStream = new RawSourceWaveStream(_process.StandardOutput.BaseStream, WaveFormat);
        Started = true;
        return _process.WaitForExitAsync(token);
    }
    
    public int Read(byte[] buffer, int offset, int count)
    {
        if (!Started) throw new ApplicationException("Piper process not initialized!");
        return _internalAudioStream!.Read(buffer, offset, count);
    }

    public Task InferPlayback(string text, CancellationToken token = default(CancellationToken))
    {
        if (!Started) throw new ApplicationException("Piper process not initialized!");
        return _process.StandardInput.WriteLineAsync(text.AsMemory(), token);
    }
    public WaveFormat WaveFormat { get; }
}