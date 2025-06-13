using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class BoolParser : BaseTypeParser
    {
        public override int TokenCount() => 1;

        // TODO: I think this is wrong now. prob just try to parse next token as bool and if it isnt then 
        // retunr true for existance and don't consume token.
        protected override bool TryParseType(TokenStream tokenSubSteam, out object obj)
        {
            // Existence implies true, parse rules may invert this based on '--no-' prefix
            if (!tokenSubSteam.HasMore())
            {
                obj = true;
                return true;
            }
            
            bool success = bool.TryParse(tokenSubSteam.Next(), out bool typed);

            obj = typed;
            return success;
        }
    }
}