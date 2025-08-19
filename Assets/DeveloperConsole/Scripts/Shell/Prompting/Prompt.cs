using System.Linq;
using System.Collections.Generic;
using DeveloperConsole.Command.Execution;
using DeveloperConsole.Parsing.TypeAdapting.Types;

namespace DeveloperConsole.Core.Shell
{
    public delegate bool PromptValidator(object input);


    public static class PromptFactory
    {
        public static Prompt<CommandGraph> Command()
        {
            return new Prompt<CommandGraph>(PromptKind.Command, "", i => typeof(CommandGraph).IsAssignableFrom(i.GetType()));
        }

        public static Prompt<T> General<T>(string message)
        {
            return new Prompt<T>(PromptKind.General, message, i => typeof(T).IsAssignableFrom(i.GetType()));
        }

        public static Prompt<T> Choice<T>(string message, PromptChoice[] choices)
        {
            var prompt = new Prompt<T>(PromptKind.Choice, message, i =>
            {
                return typeof(T).IsAssignableFrom(i.GetType()) &&
                       choices.Any(c => i.Equals(c.Value));
            });

            prompt.Metadata.Add(PromptMetaKeys.Choices, choices);
            return prompt;
        }

        public static Prompt<ConfirmationResult> Confirmation(string message)
        {
            var prompt = new Prompt<ConfirmationResult>(PromptKind.Confirmation, message, i => i is ConfirmationResult);
            return prompt;
        }
    }


    public class Prompt<T>
    {
        public readonly PromptKind Kind;
        public readonly string Message;
        public readonly PromptValidator Validator;
        public readonly Dictionary<string, object> Metadata = new();

        public Prompt(PromptKind kind, string message, PromptValidator validator)
        {
            Kind = kind;
            Message = message;
            Validator = validator;
        }
    }
}
