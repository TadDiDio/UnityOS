using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DeveloperConsole.Command
{
    public class BindingProcessor : ICommandValidator
    {
        public bool Validate(ICommand target, out string errorMessage)
        {
            errorMessage = string.Empty;

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
            var bindings = target
                .GetType()
                .GetFields(flags)
                .Where(f => f.GetCustomAttribute<BindAttribute>() != null);

            string error = null;
            bool success = bindings.All(field => ProcessBinding(field, target, out error));
            errorMessage = error;
            return success;
        }

        private bool ProcessBinding(FieldInfo field, ICommand target, out string error)
        {
            error = null;
            var attr = field.GetCustomAttribute<BindAttribute>();

            if (typeof(Object).IsAssignableFrom(field.FieldType))
            {
                if (!ConsoleAPI.Bindings.TryGetUnityObjectBinding(field.FieldType, attr.Name, attr.Tag, out var result))
                {
                    error = $"Could not find an object of type {field.FieldType.Name} in the scene.";
                    return false;
                }

                field.SetValue(target, result);
                return true;
            }

            if (!ConsoleAPI.Bindings.TryGetPlainCSharpBinding(field.FieldType, out var plainResult))
            {
                error = $"No object of type '{field.FieldType.Name}' was bound. You can bind objects using the " +
                        $"ConsoleAPI.Bindings.BindObject() API.";
                return false;
            }

            field.SetValue(target, plainResult);
            return true;
        }
    }
}
