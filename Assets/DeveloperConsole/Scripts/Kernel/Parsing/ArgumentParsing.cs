using System.Reflection;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public static class ArgumentParsing
    {
        public static bool ValidatePositionalArgs(List<FieldInfo> positionalFields, TokenStream positionalArgs)
        {
            foreach (var field in positionalFields)
            {
                // if (!Parser.TryGetTypeParser(field.FieldType, out var parser))
                // {
                //     // TODO: Console error
                //     return false;
                // }
                
                // Do parsing here
            }
            
            return true;
        }
        public static bool ValidateSwitchArgs(List<FieldInfo> switchFields, TokenStream switchArgs)
        {
            
            
            return true;
        }
    }
}