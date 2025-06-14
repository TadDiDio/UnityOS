using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class BoolParser : BaseTypeParser
    {
        public override bool TryParse(TokenStream tokenSubSteam, out object obj)
        {
            // Existence implies true, parse rules may invert this based on '--no-' prefix
            if (!tokenSubSteam.HasMore())
            {
                obj = true;
                return true;
            }

            string token = tokenSubSteam.Peek();
            bool success = bool.TryParse(token, out bool typed);

            if (success)
            {
                tokenSubSteam.Next();
            }
            
            obj = typed;
            return success;
        }
    }
}