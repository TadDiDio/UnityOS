using System;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ExcludeFromCmdRegistry : Attribute { }
}