using System.Diagnostics;
using System.Text;
using PiperSharp.Models;

namespace PiperSharp;

public class PiperProcess
{
    private Process _piperProcess;

    public Encoding Encoding => _piperProcess.StartInfo.StandardOutputEncoding!;
    private readonly MemoryStream _internalBuffer;
    private Stopwatch _bufferWriterTimeout;
    private SemaphoreSlim _bufferReadSemaphore;
    public PiperProcess(PiperConfiguration configuration)
    {
        _internalBuffer = new MemoryStream();
        _bufferWriterTimeout = Stopwatch.StartNew();
        _piperProcess = ConfigureProcess(configuration);
        _bufferReadSemaphore = new SemaphoreSlim(1);
    }

    private Process ConfigureProcess(PiperConfiguration configuration)
    {
        return new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = configuration.Location,
                Arguments = configuration.BuildArguments(),
                UseShellExecute = false,
                StandardInputEncoding = Console.InputEncoding,
                StandardOutputEncoding = Console.OutputEncoding,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                WorkingDirectory = Path.GetDirectoryName(configuration.Location)
            },
        };
    }
    public async Task Start(CancellationToken token = default(CancellationToken))
    {
        _piperProcess.Start();
        _piperProcess.BeginOutputReadLine();
        _piperProcess.OutputDataReceived += ProcessDataReceived;
        while (!_piperProcess.HasExited)
        {
            
        }

        await _piperProcess.WaitForExitAsync(token);
    }

    private void ProcessDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is null) return;
        _internalBuffer.Write(Encoding.GetBytes(e.Data));
    }
}