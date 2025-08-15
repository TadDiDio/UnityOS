using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit;
using UnityEngine;

namespace DeveloperConsole
{
    /// <summary>
    /// A utility for formatting string messages.
    /// </summary>
    public static class Formatter
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
        /// Pads a list of (label, description) pairs so that all descriptions align,
        /// based on the maximum label width.
        /// </summary>
        /// <param name="lines">The lines as (label, description) pairs.</param>
        /// <returns>The padded, aligned string.</returns>
        public static string PadLeft(IEnumerable<(string Label, string Description)> lines)
        {
            var enumerable = lines as (string Label, string Description)[] ?? lines.ToArray();
            enumerable = enumerable.Where(l => l is { Label: not null, Description: not null }).ToArray();

            if (!enumerable.Any()) return string.Empty;

            // Find the maximum width of the first column (Label)
            int maxLabelLength = enumerable.Max(pair => pair.Label?.Length ?? 0);

            var builder = new StringBuilder();
            foreach (var (label, description) in enumerable)
            {
                if (string.IsNullOrWhiteSpace(label) && string.IsNullOrWhiteSpace(description))
                {
                    builder.AppendLine();
                }
                else
                {
                    builder.AppendLine($"{label!.PadRight(maxLabelLength + 2)}{description}");
                }
            }

            string result = builder.ToString();

            if (result.EndsWith(Environment.NewLine))
            {
                result = result[..^Environment.NewLine.Length];
            }

            return result;
        }

        /// <summary>
        /// Indents every line of the given text by the specified number of spaces.
        /// </summary>
        /// <param name="text">The full multiline text to indent.</param>
        /// <param name="indent">The number of spaces to indent.</param>
        /// <returns>The indented text.</returns>
        public static string IndentLines(string text, int indent)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var lines = text.Split(Environment.NewLine);
            return IndentLines(lines, indent);
        }


        /// <summary>
        /// Indents every line in the collection by the specified number of spaces.
        /// </summary>
        /// <param name="lines">The individual lines to indent.</param>
        /// <param name="indent">The number of spaces to indent.</param>
        /// <returns>The indented text as a single string.</returns>
        public static string IndentLines(IEnumerable<string> lines, int indent)
        {
            var indentString = new string(' ', indent);
            return string.Join(Environment.NewLine, lines.Select(line => indentString + line.TrimEnd()));
        }
    }
}
