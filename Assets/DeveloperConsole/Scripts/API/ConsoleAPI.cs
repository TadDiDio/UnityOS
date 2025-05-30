namespace DeveloperConsole
{
    public static class ConsoleAPI
    {
        public static readonly ParserWrapper Parser = new();

        // TODO: Put client facing API here
        public class ParserWrapper
        {
            public void RegisterTypeParser<T>(BaseTypeParser parser) => Kernel.Instance.Get<ITypeParserRegistryProvider>().RegisterTypeParser<T>(parser);
        }
    }
}