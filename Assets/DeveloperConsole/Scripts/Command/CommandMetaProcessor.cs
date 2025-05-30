using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DeveloperConsole
{
    // TODO: Allow user overrides here
    public static class CommandMetaProcessor
    {
        public static string Name(string rawName, Type commandType)
        {
            if (string.IsNullOrEmpty(rawName))
            {
                string message = $"Name of {commandType} was null or empty. This is not allowed.";
                Debug.Log(message);
                return "";
            }
    
            string name = rawName.Trim().ToLower();
            return Regex.Replace(name, @"[^a-zA-Z]+", "");
        }
        
        public static string Name(Type commandType)
        {
            CommandAttribute attribute = commandType.GetCustomAttribute<CommandAttribute>();

            if (attribute != null) return Name(attribute.Name, commandType);
            
            string message = $"Command type {commandType} does not have a {nameof(CommandAttribute)} attribute.";
            Debug.Log(message);
            return "";
        }
        
        public static string Description(string rawDescription, Type commandType)
        {
            if (string.IsNullOrEmpty(rawDescription))
            {
                string message = $"Description of {commandType} was null or empty. This is not allowed.";
                Debug.Log(message);
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
        public static string Description(Type commandType)
        {
            CommandAttribute attribute = commandType.GetCustomAttribute<CommandAttribute>();

            if (attribute != null) return Description(attribute.Description, commandType);
            
            string message = $"Command type {commandType} does not have a {nameof(CommandAttribute)} attribute.";
            Debug.Log(message);
            return "";
        }
    }
}