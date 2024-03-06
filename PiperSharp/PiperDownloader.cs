using System.IO.Compression;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Formats;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using PiperSharp.Models;

namespace PiperSharp;

public static class PiperDownloader
{
    private const string PIPER_REPO_URL = "https://github.com/rhasspy/piper";
    private const string MODEL_REPO_URL = "https://huggingface.co/rhasspy/piper-voices";
    private const string MODEL_LIST_URL = "https://huggingface.co/rhasspy/piper-voices/raw/main/voices.json";
    private const string MODEL_DOWNLOAD_URL =
        "https://huggingface.co/rhasspy/piper-voices/resolve/main/MODEL_FILE_URL?download=true";
    
    private static Dictionary<string, VoiceModel>? _voiceModels;
    private static Regex RemoveLastSlash = new Regex(@"\/$", RegexOptions.Compiled);

    public static async Task<Stream> DownloadPiper(string version = "latest", string repo = PIPER_REPO_URL)
    {
        if (!Environment.Is64BitOperatingSystem)
            throw new NotSupportedException();
        var arch = typeof(object).Assembly.GetName().ProcessorArchitecture;
        var os = Environment.OSVersion.Platform;
        var fileName = os switch
        {
            PlatformID.Win32NT => "piper_windows_amd64.zip",
            PlatformID.Unix => arch == ProcessorArchitecture.Arm
                ? "piper_linux_aarch64.tar.gz"
                : "piper_linux_x86_64.tar.gz",
            PlatformID.MacOSX => arch == ProcessorArchitecture.Arm
                ? "piper_macos_aarch64.tar.gz"
                : "piper_macos_x64.tar.gz",
            _ => throw new NotSupportedException()
        };
        version = version == "latest" ? "latest/download" : $"download/{version}";
        var url = $"{RemoveLastSlash.Replace(repo, "")}/releases/{version}/{fileName}";
        var client = new HttpClient();
        var downloadStream = await client.GetStreamAsync(url);
        return downloadStream;
    }

    public static async Task<string> ExtractPiper(this Task<Stream> downloadStream, string extractTo = "./")
        => ExtractPiper(await downloadStream, extractTo);
    public static string ExtractPiper(this Stream downloadStream, string extractTo="./")
    {
        bool isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
        if (isWindows)
        {
            using var archive = new ZipArchive(downloadStream);
            archive.ExtractToDirectory(extractTo);
        }
        else
        {
            using var archive = TarArchive.CreateInputTarArchive(downloadStream, Encoding.UTF8);
            archive.ExtractContents(extractTo);
        }
        return extractTo;
    }

    public static async Task<Dictionary<string, VoiceModel>?> GetHuggingFaceModelList()
    {
        if (_voiceModels is not null) return _voiceModels;
        var client = new HttpClient();
        var response = await client.GetAsync(MODEL_LIST_URL);
        if (!response.IsSuccessStatusCode) throw new HttpRequestException();
        var data = await response.Content.ReadAsStringAsync();
        if (data is null) throw new ApplicationException();
        _voiceModels = JsonSerializer.Deserialize<Dictionary<string, VoiceModel>>(data);
        return _voiceModels;
    }

    public static async Task<string> DownloadModel(this VoiceModel model, string extractTo = "./")
    {
        var path = Path.Join(extractTo, model.Key);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var client = new HttpClient();
        foreach (var file in model.Files.Keys)
        {
            var filePath = Path.Join(path, Path.GetFileName(file));
            var downloadStream = await client.GetStreamAsync(MODEL_DOWNLOAD_URL.Replace("MODEL_FILE_URL", file));
            await using var fs = File.OpenWrite(filePath);
            await downloadStream.CopyToAsync(fs);
            fs.Close();
            downloadStream.Close();
        }

        return path;
    }
}