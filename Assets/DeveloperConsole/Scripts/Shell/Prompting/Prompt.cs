using System;
using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.TypeAdapting.Types;

namespace DeveloperConsole.Core.Shell
{
    public delegate bool PromptValidator(object input);

    public class Prompt
    {
        public readonly PromptKind Kind;
        public readonly string Message;
        public readonly Type RequestedType;
        public readonly PromptValidator Validator;
        public readonly Dictionary<string, object> Metadata = new();

        private Prompt(PromptKind kind, Type requestedType, string message, PromptValidator validator)
        {
            Kind = kind;
            Message = message;
            RequestedType = requestedType;
            Validator = validator;
        }

        public static Prompt Command()
        {
            return new Prompt(PromptKind.Command, typeof(ICommandResolver), "", i =>
            {
                return typeof(ICommandResolver).IsAssignableFrom(i.GetType());
            });
        }

        public static Prompt General<T>(string message)
        {
            return new Prompt(PromptKind.General, typeof(T), message, i =>
            {
                return typeof(T).IsAssignableFrom(i.GetType());
            });
        }

        public static Prompt Choice<T>(string message, PromptChoice[] choices)
        {
            var prompt = new Prompt(PromptKind.Choice, typeof(T), message, i =>
            {
                return typeof(T).IsAssignableFrom(i.GetType()) &&
                       choices.Any(c => i.Equals(c.Value));
            });
            prompt.Metadata.Add(PromptMetaKeys.Choices, choices);
            return prompt;
        }

        public static Prompt Confirmation(string message)
        {
            var prompt = new Prompt(PromptKind.Confirmation, typeof(ConfirmationResult), message, i =>
            {
                return i is ConfirmationResult;
            });
            return prompt;
        }
    }
}
