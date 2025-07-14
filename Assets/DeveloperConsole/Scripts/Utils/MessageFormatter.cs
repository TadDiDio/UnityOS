using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DeveloperConsole
{
    /// <summary>
    /// A utility for formatting string messages.
    /// </summary>
    public static class MessageFormatter
    {
        public static Color Blue => new Color(0f, 0.66f, 0.86f);
        public static Color Green => Color.green;
        public static Color Yellow => Color.yellow;
        public static Color Red => Color.red;
        public static Color White => Color.white;


        /// <summary>
        /// A standard bar: --------
        /// </summary>
        public static readonly string Bar = $"──────────────────────────────";

        private static string ColorToHex(Color color)
        {
            Color32 c = color;
            return $"#{c.r:X2}{c.g:X2}{c.b:X2}";
        }


        /// <summary>
        /// Adds a color to a string.
        /// </summary>
        /// <param name="input">The string.</param>
        /// <param name="color">The color.</param>
        /// <returns>The colorized string.</returns>
        public static string AddColor(string input, Color color)
        {
            return $"<color={ColorToHex(color)}>{input}</color>";
        }


        /// <summary>
        /// Creates an error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The styled error message.</returns>
        public static string Error(string message)
        {
            string tag = AddColor("[Error]", Red);
            return $"{tag} {message}";
        }


        /// <summary>
        /// Creates a warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The styled warning message.</returns>
        public static string Warning(string message)
        {
            string tag = AddColor("[Warning]", Yellow);
            return $"{tag} {message}";
        }


        /// <summary>
        /// Adds bold weighting to a string.
        /// </summary>
        /// <param name="message">The string.</param>
        /// <returns>The bolded string.</returns>
        public static string Bold(string message)
        {
            return $"<b>{message}</b>";
        }


        /// <summary>
        /// A quick title template.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="color">The color.</param>
        /// <returns>The title block.</returns>
        public static string Title(string title, Color color)
        {
            return $"{AddColor(title, color)}{Environment.NewLine}{Bar}{Environment.NewLine}";
        }


        /// <summary>
        /// Pads a list of lines so that the second words of each all align.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <returns>The padded lines.</returns>
        public static string PadFirstWordRight(IEnumerable<string> lines)
        {
            var splitLines = new List<(string First, string Remaining)>();

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
