namespace PiperSharp.Examples;

public class Program
{
    public static async Task Main()
    {
        choose:
            Console.Clear();
            Console.WriteLine("1. Playback sample");
            Console.WriteLine("2. Save to file sample");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var choice))
            {
                switch (choice)
                {
                    case 1: await (new SimplePlaybackProgram()).Run(); break;
                    case 2: await (new SimpleSaveFileProgram()).Run(); break;
                    default: goto choose;
                }
            }
            goto choose;
    }
}