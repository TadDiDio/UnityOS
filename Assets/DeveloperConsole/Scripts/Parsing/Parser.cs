using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Parsing.Rules;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class Parser : IParser
    {
        private readonly List<IParseRule> _rules;
        private readonly ITokenizer _tokenizer;
       
        
        /// <summary>
        /// Creates a new parser.
        /// </summary>
        /// <param name="tokenizer">The tokenizer to use.</param>
        public Parser(ITokenizer tokenizer)
        {
            _tokenizer = tokenizer;
            
            List<IParseRule> rules = new()
            {
                new InvertedBoolLongSwitchParseRule(),
                new InvertedBoolShortSwitchParseRule(),
                new BoolLongSwitchParseRule(),
                new BoolShortSwitchParseRule(),
                new LongSwitchParseRule(),
                new ShortSwitchParseRule(),
                new PositionalParseRule(),
                new VariadicParseRule()
            };
            
            _rules = rules.OrderBy(rule => rule.Priority()).ToList();
        }

        public TokenizationResult Tokenize(string input)
        {
            return _tokenizer.Tokenize(input);
        }
        
        public ParseResult Parse(TokenStream stream, IParseTarget target)
        {
            ParseContext context = new();
            
            // Iterate over all combinations of first token and args and get the first matching rule by priority
            while (stream.HasMore())
            {
                // Type parsers expect that the name is already peeled off
                string token = stream.Next();
                
                foreach (var arg in target.GetArguments())
                {
                    foreach (var rule in _rules)
                    {
                        if (!rule.CanMatch(token, arg, context)) continue;
                        if (!rule.TryParse(stream, arg, out var result)) return result;

                        target.SetArgument(arg, result.Value);
                        break;
                    }
                }
            }

            // TODO: Check that the IParseTarget is in a valid state before returning success
            
            return ParseResult.Finished();
        }
    }
}