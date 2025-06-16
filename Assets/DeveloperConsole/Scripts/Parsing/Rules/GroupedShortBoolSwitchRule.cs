using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Rules
{
    public class GroupedShortBoolSwitchRule : IParseRule
    {
        public int Priority() => 350;

        public ArgumentSpecification[] Filter(string token, ArgumentSpecification[] allArgs, ParseContext context)
        {
            if (!token.StartsWith("-") || token.StartsWith("--") || token.Length < 3) return null;

            var flagChars = token.Substring(1).ToCharArray();
            var matchedArgs = new List<ArgumentSpecification>();

            foreach (char ch in flagChars)
            {
                var match = allArgs
                    .Where(arg => arg.FieldInfo.FieldType == typeof(bool))
                    .FirstOrDefault(arg => arg.Attributes
                        .OfType<SwitchAttribute>()
                        .Any(attr => attr.Alias == ch));

                // If any fail, reject the whole token
                if (match == null) return null; 

                matchedArgs.Add(match);
            }

            return matchedArgs.ToArray();
        }

        public ParseResult Apply(TokenStream tokenStream, ArgumentSpecification[] args)
        {
            // Consume the token
            string token = tokenStream.Next();

            foreach (var arg in args)
            {
                if (arg.FieldInfo.FieldType != typeof(bool)) return ParseResult.TypeParsingFailed(token, arg);
            }
            
            // Return a result mapping each argument to true
            var dict = args.ToDictionary(arg => arg, _ => (object)true);

            return ParseResult.Success(dict);
        }
    }
}