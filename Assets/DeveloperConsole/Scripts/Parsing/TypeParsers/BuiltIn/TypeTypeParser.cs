using System;
using System.Linq;

namespace DeveloperConsole
{
    public class TypeTypeParser : BaseTypeParser
    {
        public override int TokenCount() => 1;

        protected override bool TryParseType(TokenStream tokenSubSteam, out object obj)
        {
            obj = null;
            string typeName = tokenSubSteam.Next();
            
            // Test without namespace
            obj = Type.GetType(typeName);

            return obj != null;
        }
    }
}