using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DeveloperConsole
{
    // TODO: Allow user overrides here
    public static class CommandMetaProcessor
    {
        public static string Name(string rawName)
        {
            if (string.IsNullOrEmpty(rawName))
            {
                string message = $"Name of a was null or empty. This is not allowed.";
                Debug.Log(message);
                return "";
            }
    
            string name = rawName.Trim().ToLower();
            return Regex.Replace(name, @"[^a-zA-Z0-9]+", "");
        }
        
        public static string Description(string rawDescription)
        {
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