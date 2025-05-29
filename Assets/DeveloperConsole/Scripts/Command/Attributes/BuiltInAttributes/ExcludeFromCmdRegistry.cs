using System;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExcludeFromCmdRegistry : Attribute { }
}