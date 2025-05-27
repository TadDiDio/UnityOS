namespace DeveloperConsole
{
    public class FloatParser : BaseTypeParser<float>
    {
        public override int TokenCount() => 1;
        protected override bool TryParseType(TokenStream tokenSubSteam, out float obj)
        {
            return float.TryParse(tokenSubSteam.Next(), out obj);
        }
    }
}