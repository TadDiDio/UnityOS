namespace DeveloperConsole
{
    public class IntParser : BaseTypeParser<int>
    {
        public override int TokenCount() => 1;

        protected override bool TryParseType(TokenStream tokenSubSteam, out int obj)
        {
            return int.TryParse(tokenSubSteam.Next(), out obj);
        }
    }
}