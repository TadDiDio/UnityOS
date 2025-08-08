using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DeveloperConsole.Command;

namespace DeveloperConsole.Parsing
{
    public static class FileBatcher
    {
        public static CommandBatch BatchFile(string filepath)
        {
            try
            {
                var lines = new List<string>();

                foreach (var rawLine in File.ReadLines(filepath))
                {
                    string line = rawLine.Trim();

                    if (string.IsNullOrEmpty(line) || line.StartsWith("//")) continue;

                    int commentIndex = line.IndexOf("//", StringComparison.Ordinal);
                    if (commentIndex >= 0)
                    {
                        line = line.Substring(0, commentIndex).TrimEnd();
                    }

                    if (!string.IsNullOrEmpty(line)) lines.Add(line);
                }

                if (lines.Count == 0) return new CommandBatch();

                var sb = new StringBuilder();

                for (int i = 0; i < lines.Count; i++)
                {
                    sb.Append(lines[i]);

                    bool lastEndsWithAndAnd = lines[i].EndsWith("&&");
                    bool nextStartsWithAndAnd = (i + 1 < lines.Count) && lines[i + 1].StartsWith("&&");

                    if (i == lines.Count - 1) break;

                    if (lastEndsWithAndAnd || nextStartsWithAndAnd) sb.Append(" && ");
                    else sb.Append("; ");
                }

                return new DefaultCommandBatcher().GetBatch(sb.ToString());
            }
            catch (Exception e)
            {
                Log.Error($"Could not batch file: {e.Message}");
                return new CommandBatch();
            }
        }
    }
}
