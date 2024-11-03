namespace PWABuilder.IOS.Cli;

internal static class Cli
{
    internal static CliSettings? ParseArgs(string[] args)
    {
        if (args.Contains("--help"))
        {
            Console.WriteLine("Options: [--input=config.json] [--output=./out] [--clear-output]");
            return null;
        }

        var input = args.FirstOrDefault(x => x.StartsWith("--input="))?.Substring("--input=".Length) 
                    ?? "config.json";
        var output = args.FirstOrDefault(x => x.StartsWith("--output="))?.Substring("--output=".Length)
                     ?? "./out";

        Console.WriteLine("Welcome to IOS PWA Builder!");
        if (!Directory.Exists(output))
        {
            Console.WriteLine($"Directory {output} is not exists");
            return null;
        }

        if (args.Contains("--clear-output"))
        {
            foreach (var entry in Directory.EnumerateFileSystemEntries(output))
            {
                File.Delete(entry);
            }
        } else if (Directory.EnumerateFileSystemEntries(output).Any())
        {
            Console.WriteLine($"Directory {output} is not empty");
            return null;
        }

        return new CliSettings(input, output);
    }
}