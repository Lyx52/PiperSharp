using System.Text;

namespace PiperSharp;

public static class Extensions
{
    public static string ToUtf8(this string text)
    {
        return Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(text));
    }
    
    public static string AddPathQuotesIfRequired(this string text)
    {
        if (Environment.OSVersion.Platform != PlatformID.Win32NT) return text;
        var sb = new StringBuilder();
        if (!text.StartsWith('"')) sb.Append('"');
        sb.Append(text);
        if (!text.EndsWith('"')) sb.Append('"');

        return sb.ToString();
    }
}