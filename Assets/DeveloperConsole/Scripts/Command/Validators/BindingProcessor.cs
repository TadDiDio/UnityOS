using DeveloperConsole.Parsing;
using System.Linq;
using System.Reflection;

namespace DeveloperConsole.Command
{
    public class BindingProcessor : ICommandValidator
    {
        public bool Validate(CommandParseTarget target, out string errorMessage)
        {
            errorMessage = string.Empty;

            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
            var bindings = target
                .Command
                .GetType()
                .GetFields(flags)
                .Where(f => f.GetCustomAttribute<BindingAttribute>() != null);

            foreach (var field in bindings)
            {
                if (!ProcessBinding(field, target.Command))
                {
                    errorMessage = "Set error message here";
                    return false;
                }
            }

            return true;
        }

        private bool ProcessBinding(FieldInfo field, ICommand target)
        {
            Log.Info("Trying to bind to " + field.Name);

            return true;
        }
    }
}
