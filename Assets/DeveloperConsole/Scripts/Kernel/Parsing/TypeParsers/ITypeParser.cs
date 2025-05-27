namespace DeveloperConsole
{
    public interface ITypeParser { }

    public interface ITypeParser<TType> : ITypeParser
    {
        public int TokenCount();
        public bool TryParse(TokenStream tokenStream, out TType obj);
    }
}