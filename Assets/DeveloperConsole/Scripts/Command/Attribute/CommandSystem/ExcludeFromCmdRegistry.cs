using System;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Excludes a command from the registry. Helpful for test types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ExcludeFromCmdRegistry : Attribute { }
}