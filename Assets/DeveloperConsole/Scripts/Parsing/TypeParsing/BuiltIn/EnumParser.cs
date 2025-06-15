using System;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class EnumParser<TEnum> : BaseTypeParser where TEnum : struct, Enum
    {
        protected override bool TryParse(TokenStream tokenStream, out object obj)
        {
            bool success = Enum.TryParse(tokenStream.Next(), true, out TEnum typed) &&
                   Enum.IsDefined(typeof(TEnum), typed);
            
            obj = typed;
            return success;
        }
    }
}