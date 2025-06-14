using System.Text.RegularExpressions;

namespace DeveloperConsole.Command
{
    // TODO: Allow user overrides here
    /// <summary>
    /// A processor to ensure global command consistency.
    /// </summary>
    public static class CommandMetaProcessor
    {
        /// <summary>
        /// Normalizes command names.
        /// </summary>
        /// <param name="rawName">The command's given name.</param>
        /// <returns>The normalized name.</returns>
        public static string Name(string rawName)
        {
            if (string.IsNullOrEmpty(rawName))
            {
                string message = $"Name of a was null or empty. This is not allowed.";
                Log.Info(message);
                return "";
            }
    
            string name = rawName.Trim().ToLower();
            return Regex.Replace(name, @"[^a-zA-Z0-9_\-]+", "");
        }
        
        
        /// <summary>
        /// Normalizes command descriptions.
        /// </summary>
        /// <param name="rawDescription">The command's given description.</param>
        /// <returns>The normalized description.</returns>
        public static string Description(string rawDescription)
        {
            if (string.IsNullOrEmpty(rawDescription))
            {
                Log.Error("Description of a command or argument was null or empty. This is not allowed.");
                return "";
            }
            
            string description = rawDescription.Trim();
            description = char.ToUpper(description[0]) + description[1..];
            
            char last = description[^1];
            if (last != '.' && last != '!' && last != '?')
            {
                description += ".";
            }
        
            return description;
        }
    }
}