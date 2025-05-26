using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DeveloperConsole
{
    public class DefaultTokenizer : ITokenizer
    {
        private readonly Regex _tokenizer = new(@"(?<=^|\s)""([^""]*)""(?=$|\s)|\S+", RegexOptions.Compiled);
        
        public TokenizationResult Tokenize(string input)
        {
            var matches = _tokenizer.Matches(input);
            var tokens  = new List<string>(matches.Count);
            
            foreach (Match m in matches)
            {
                string value = m.Value;
                
                // If this token is quoted, strip the surrounding quotes:
                if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
                {
                    value = value.Substring(1, value.Length - 2);
                }
                
                tokens.Add(value);
            }

            return new TokenizationResult
            {
                Success = true,
                Tokens = tokens
            };
        }
    }
}