using System.Collections.Generic;
using System.Linq;

namespace DeveloperConsole
{
    public abstract class BaseTypeParser
    {
        public abstract int TokenCount();

        public bool TryParse(TokenStream tokenStream, out object obj)
        {
            List<string> subtokens = tokenStream.Read(TokenCount()).ToList();
            TokenStream subtokenStream = new(subtokens);

            obj = default;
            bool result = TryParseType(subtokenStream, out obj);

            if (subtokenStream.HasMore())
            {
                // TODO: This should not happen and needs to be logged 
                // Notify user their parser is fucked
            }
            
            return result;
        }
        protected abstract bool TryParseType(TokenStream tokenSubSteam, out object obj);
    }
}