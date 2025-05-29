namespace DeveloperConsole
{
    public class BoolParser : BaseTypeParser
    {
        public override int TokenCount() => 1;

        protected override bool TryParseType(TokenStream tokenSubSteam, out object obj)
        {
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