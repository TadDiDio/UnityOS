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
            StringBuilder sb = new StringBuilder();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    sb.AppendLine();
                    continue;
                }

                int firstSpace = line.IndexOf(' ');
                if (firstSpace == -1)
                {
                    // No space found – treat the whole line as the first word
                    sb.AppendLine(line);
                    continue;
                }

                string firstWord = line.Substring(0, firstSpace);
                string rest = line.Substring(firstSpace + 1).TrimStart();

                sb.AppendLine($"{firstWord}\t\t{rest}");
            }

            return sb.ToString();
        }
    }
}