using System;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.IO;

namespace DeveloperConsole.Core.Shell.Prompting
{
    public class RetryUntilSuccessPromptWrapper : IPromptWrapper
    {
        public async Task<PromptResult<T>> HandlePrompt<T>
        (
            IPromptable promptable,
            IOutputChannel output,
            Prompt prompt,
            CancellationToken cancellationToken
        )
        {
            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var response = await promptable.HandlePrompt(prompt, cancellationToken);

                    // If the response is the correct type, we can return or retry without adapting
                    if (response is T typed)
                    {
                        if (prompt.Validator.Invoke(typed))
                        {
                            return PromptResult<T>.Successful(typed);
                        }
                        output.WriteLine(Formatter.Error($"Cannot convert {response.GetType().FullName} to type {typeof(T).FullName}."));
                        continue;
                    }

                    // Fail loudly if the submission is not a string or the correct type.
                    if (response is not string stringResponse) throw new InvalidOperationException
                        ($"{promptable.GetType().Name} responded with {response.GetType().Name} when asked for {{prompt.RequestedType.Name}}!");

                    // Don't process null input
                    if (stringResponse is "") continue;

                    // Fail loudly if we cannot handle converting to this type
                    if (!ConsoleAPI.Parsing.CanAdaptType(typeof(T)))
                    {
                        throw new InvalidOperationException($"No type adapter registered for type {typeof(T).Name}");
                    }

                    // Try adapting and show result
                    var result = ConsoleAPI.Parsing.AdaptTypeFromString(typeof(T), stringResponse);

                    if (!result.Success)
                    {
                        output.WriteLine(Formatter.Error($"Cannot convert {response.GetType().FullName} to type {typeof(T).FullName}."));
                        continue;
                    }

                    // Success
                    if (prompt.Validator.Invoke(result.As<T>()))
                    {
                        return PromptResult<T>.Successful(result.As<T>());
                    }

                    // Failure
                    output.WriteLine(Formatter.Error($"Cannot convert {response.GetType().FullName} to type {typeof(T).FullName}."));
                }
            }
            catch (OperationCanceledException)
            {
                // Propagate operation cancelled
                throw;
            }
            catch (Exception e)
            {
                // Display all others
                output.WriteLine(Formatter.Error(e.Message));
                throw;
            }
        }
    }
}
