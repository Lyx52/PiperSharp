using System.Text;

namespace PiperSharp;

public static class Extensions
{
    public static string ToUtf8(this string text)
    {
        return Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(text));
    }
}