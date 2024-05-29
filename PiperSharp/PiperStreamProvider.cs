using System.Diagnostics;
using PiperSharp.Models;

namespace PiperSharp;

public class PiperStreamProvider : Stream
{
    public PiperConfiguration Configuration { get; set; }
    public bool Started { get; private set; } = false;
    private Process _process;
    private Stream? _baseStream;
    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => _baseStream?.Length ?? -1;

    public override long Position
    {
        get => _baseStream?.Position ?? -1; 
        set => throw new NotSupportedException();
    }

    public PiperStreamProvider(PiperConfiguration configuration)
    {
        Configuration = configuration;
        _process = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = configuration.ExecutableLocation,
                Arguments = configuration.BuildArguments(),
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = configuration.WorkingDirectory
            },
        };
    }

    public void Start()
    {
        _process.Start();
        _baseStream = _process.StandardOutput.BaseStream;
        Started = true;
    }
    
    public Task WaitForExit(CancellationToken token = default(CancellationToken)) => _process.WaitForExitAsync(token);
    
    public override int Read(byte[] buffer, int offset, int count)
    {
        if (!Started) throw new ApplicationException("Piper process not initialized!");
        return _baseStream!.Read(buffer, offset, count);
    }
    
    public Task InferPlayback(string text, CancellationToken token = default(CancellationToken))
    {
        if (!Started) throw new ApplicationException("Piper process not initialized!");
        return _process.StandardInput.WriteLineAsync(text.AsMemory(), token);
    }
    
    public override void Flush()
    {
        throw new NotSupportedException();
    }
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }
    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }
}