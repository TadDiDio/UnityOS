namespace DeveloperConsole
{
    public class FloatParser : BaseTypeParser
    {
        public override int TokenCount() => 1;
        protected override bool TryParseType(TokenStream tokenSubSteam, out object obj)
        {
            bool success = float.TryParse(tokenSubSteam.Next(), out float typed);

            obj = typed;
            return success;
        }
    }
}