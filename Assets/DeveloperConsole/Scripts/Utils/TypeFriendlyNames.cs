using System;

namespace DeveloperConsole
{
    /// <summary>
    /// Converts system type names to friendly names and vis versa.
    /// </summary>
    public static class TypeFriendlyNames
    {
        /// <summary>
        /// Converts a type to a friendly name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The friendly name.</returns>
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
                _ => type.Name.ToLowerInvariant()
            };
        }


        /// <summary>
        /// Converts a friendly name to a type. Name must be fully qualified if not primitive.
        /// </summary>
        /// <param name="name">The common or friendly name.</param>
        /// <returns>The type.</returns>
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
