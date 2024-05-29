using System.Diagnostics;
using NAudio.Wave;
using PiperSharp.Models;

namespace PiperSharp;

public class PiperWaveProvider : PiperStreamProvider, IWaveProvider
{
    public PiperConfiguration Configuration { get; set; }
    public bool Started => _piperStreamProvider.Started;
    private PiperStreamProvider _piperStreamProvider;
    private RawSourceWaveStream? _internalAudioStream;

    public PiperWaveProvider(PiperConfiguration configuration) : base(configuration)
    {
        _piperStreamProvider = new PiperStreamProvider(configuration);
        Configuration = configuration;
        WaveFormat = new WaveFormat((int)(configuration.Model.Audio?.SampleRate ?? 16000), 1);
    }

    public void Start()
    {
        _piperStreamProvider.Start();
        _internalAudioStream = new RawSourceWaveStream(_piperStreamProvider, WaveFormat);
    }
    
    public Task WaitForExit(CancellationToken token = default(CancellationToken)) => _piperStreamProvider.WaitForExit(token);
    
    public int Read(byte[] buffer, int offset, int count)
    {
        if (!Started) throw new ApplicationException("Piper process not initialized!");
        return _internalAudioStream!.Read(buffer, offset, count);
    }

    public Task InferPlayback(string text, CancellationToken token = default(CancellationToken))
        => _piperStreamProvider.InferPlayback(text, token);
    public WaveFormat WaveFormat { get; }
}