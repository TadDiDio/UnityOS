using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Command;
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
            
            // TODO: Inject these
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
        
        /// <summary>
        /// Parses an argument token stream into the parse target.
        /// Note that this stream should only include arguments and not
        /// target identifiers like command names.
        /// </summary>
        /// <param name="stream">The token stream.</param>
        /// <param name="target">The target.</param>
        /// <returns>The result.</returns>
        public ParseResult Parse(TokenStream stream, IParseTarget target)
        {
            ParseContext context = new(target);
            
            HashSet<ArgumentSpecification> unsetArgs = new(target.GetArguments());

            // Iterate over tokens in order
            while (stream.HasMore())
            {
                string token = stream.Peek();
                bool tokensConsumed = false;
                
                // Find the first rule that matches and only set a single arg.
                foreach (var arg in unsetArgs.ToList())
                {
                    bool applied = false;
                    foreach (var rule in _rules)
                    {
                        if (!rule.CanMatch(token, arg, context)) continue;

                        int remainingTokens = stream.Count();
                        var parseResult = rule.TryParse(stream, arg);

                        if (parseResult.Status is not Status.Success) return parseResult;
                        if (stream.Count() == remainingTokens)
                        {
                            return ParseResult.TokenNotConsumed(token, arg);
                        }
                        
                        tokensConsumed = true;
                        unsetArgs.Remove(arg);
                        target.SetArgument(arg, parseResult.Value);
                        applied = true;
                        break;
                    }
                    
                    if (applied) break;
                }

                if (!tokensConsumed)
                {
                    return ParseResult.UnexpectedToken(token);
                }
            }

            if (!target.Validate(out string errorMessage))
            {
                return ParseResult.ValidationFailed(errorMessage);
            }
            
            return ParseResult.Finished();
        }
    }
}