using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public static class GrepExtension
{
    public static IEnumerable<(string file, int line, string text)> Grep(
        string rootDir, string pattern, string searchPattern = "*.*")
    {
        var regex = new Regex(pattern, RegexOptions.Compiled);

        foreach (var file in Directory.EnumerateFiles(rootDir, searchPattern, SearchOption.AllDirectories))
        {
            var lines = File.ReadLines(file);
            int lineNum = 0;
            foreach (var line in lines)
            {
                lineNum++;
                if (regex.IsMatch(line))
                    yield return (file, lineNum, line.Trim());
            }
        }
    }
}