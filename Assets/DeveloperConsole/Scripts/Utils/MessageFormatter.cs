using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DeveloperConsole
{
    public static class MessageFormatter
    {
        public static Color Blue => new Color(0f, 0.66f, 0.86f);
        public static Color Green => Color.green;
        public static Color Yellow => Color.yellow;
        public static Color Red => Color.red;
        public static Color White => Color.white;

        public static readonly string Bar = $"──────────────────────────────";
        
        private static string ColorToHex(Color color)
        {
            Color32 c = color;
            return $"#{c.r:X2}{c.g:X2}{c.b:X2}";
        }

        public static string AddColor(string input, Color color)
        {
            return $"<color={ColorToHex(color)}>{input}</color>";
        }
        public static string Error(string message)
        {
            string tag = AddColor("[Error]", Red);
            return $"{tag} {message}";
        }

        public static string Warning(string message)
        {
            string tag = AddColor("[Warning]", Yellow);
            return $"{tag} {message}";
        }

        public static string Bold(string message)
        {
            return $"<b>{message}</b>";
        }
        
        public static string Title(string title, Color color)
        {
            return $"{AddColor(title, color)}{Environment.NewLine}{Bar}{Environment.NewLine}";
        }
        public static string PadFirstWordRight(IEnumerable<string> lines)
        {
            var splitLines = new List<(string First, string Remaining
                )>();

            int maxFirstWordLength = 0;

            // First pass: split lines and find max first word length
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    splitLines.Add((string.Empty, string.Empty));
                    continue;
                }

                int firstSpace = line.IndexOf(' ');
                if (firstSpace == -1)
                {
                    splitLines.Add((line, string.Empty));
                    maxFirstWordLength = Math.Max(maxFirstWordLength, line.Length);
                }
                else
                {
                    string first = line.Substring(0, firstSpace);
                    string rest = line.Substring(firstSpace + 1).TrimStart();
                    splitLines.Add((first, rest));
                    maxFirstWordLength = Math.Max(maxFirstWordLength, first.Length);
                }
            }

            // Second pass: align
            StringBuilder sb = new StringBuilder();
            foreach (var (first, rest) in splitLines)
            {
                if (string.IsNullOrEmpty(first) && string.IsNullOrEmpty(rest))
                {
                    sb.AppendLine();
                }
                else
                {
                    sb.AppendLine($"{first.PadRight(maxFirstWordLength + 2)}{rest}");
                }
            }

            return sb.ToString();
        }

    }
}