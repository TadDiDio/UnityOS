using UnityEngine;

namespace DeveloperConsole
{
    public class StringParser : BaseTypeParser
    {
        public override int TokenCount() => 1;

        protected override bool TryParseType(TokenStream tokenSubSteam, out object obj)
        {
            obj = tokenSubSteam.Next();
            return true;
        }
    }
}