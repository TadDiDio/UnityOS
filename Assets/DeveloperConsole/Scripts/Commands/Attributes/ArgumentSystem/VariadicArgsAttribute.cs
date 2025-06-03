using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Field)]
    public class VariadicArgsAttribute : ArgumentValidator
    {
        public bool IsCommandPath;
     
        private string _commandPath;
        
        public VariadicArgsAttribute(bool isCommandPath = false)
        {
            IsCommandPath = isCommandPath;
        }
    
        public override bool Validate(AttributeValidationData data)
        {
            if (!IsCommandPath) return true;

            if (data.FieldInfo.GetValue(data.Object) is not List<string> path) return false;
            _commandPath = string.Join(".", path);
            
            return ConsoleAPI.IsValidCommand(_commandPath);
        }

        public override string ErrorMessage() => $"{_commandPath} is not a valid command.";
    }
}