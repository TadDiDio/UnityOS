using System;
using System.Linq;

namespace DeveloperConsole
{
    public static class TypeFriendlyNames
    {
        public static string TypeToName(Type type)
        {
            return type switch
            {
                null => "null",
                not null when type == typeof(int) => "int",
                not null when type == typeof(float) => "float",
                not null when type == typeof(bool) => "bool",
                not null when type == typeof(string) => "string",
                not null when type == typeof(double) => "double",
                not null when type == typeof(long) => "long",
                not null when type == typeof(short) => "short",
                not null when type == typeof(byte) => "byte",
                not null when type == typeof(char) => "char",
                _ => type.Name
            };
        }
        
        public static Type NameToType(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            string normalized = name.Trim().ToLower();
            
            return normalized switch
            {
                "int" => typeof(int),
                "float" => typeof(float),
                "bool" => typeof(bool),
                "string" => typeof(string),
                "double" => typeof(double),
                "long" => typeof(long),
                "short" => typeof(short),
                "byte" => typeof(byte),
                "char" => typeof(char),
                "null" => null,
                _ => Type.GetType(name, throwOnError: false, ignoreCase: true)
            };
        }
    }
}