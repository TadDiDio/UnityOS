using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace DeveloperConsole.Command
{
    public class DefaultCommandBatcher
    {
        /// <summary>
        /// Turns a string into a batch of commands.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The batch.</returns>
        public CommandBatch GetBatch(string input)
        {
            var parts = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    current.Append(c);
                }
                else if (!inQuotes && i + 1 < input.Length && input[i] == '&' && input[i + 1] == '&')
                {
                    parts.Add(current.ToString().Trim());
                    current.Clear();
                    parts.Add("&&");
                    i++;
                }
                else if (!inQuotes && c == ';')
                {
                    parts.Add(current.ToString().Trim());
                    current.Clear();
                    parts.Add(";");
                }
                else
                {
                    current.Append(c);
                }
            }

            if (current.Length > 0)
                parts.Add(current.ToString().Trim());

            var matches = parts.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            var batch = new CommandBatch();
            string lastSep = null;

            foreach (var part in matches)
            {
                if (part is "&&" or ";")
                {
                    lastSep = part;
                }
                else
                {
                    bool windowed = part.StartsWith("!");
                    var cmdText = windowed ? part[1..] : part;

                    // Tokenize the command text
                    var tokens = ConsoleAPI.Parsing.Tokenize(cmdText).Tokens;

                    batch.Requests.Add(new FrontEndCommandRequest
                    {
                        Resolver = new TokenCommandResolver(tokens),
                        Condition = lastSep == "&&" ? CommandCondition.OnPreviousSuccess : CommandCondition.Always,
                        Windowed = windowed
                    });

                    lastSep = null;
                }
            }

            return batch;
        }


        public CommandBatch GetBatchFromTokens(List<string> tokens)
        {
            return GetBatch(string.Join(" ", tokens));
        }
    }
}
