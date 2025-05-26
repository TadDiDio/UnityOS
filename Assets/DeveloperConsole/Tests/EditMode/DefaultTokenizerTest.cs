using DeveloperConsole;
using NUnit.Framework;

public class DefaultTokenizerTest
{
    private DefaultTokenizer _tokenizer;

    [SetUp]
    public void SetUp()
    {
        _tokenizer = new DefaultTokenizer();
    }
    
    [Test]
    public void DefaultTokenizerTestSimple()
    {
        var result = _tokenizer.Tokenize("Hello world");
        
        Assert.True(result.Success);
        Assert.AreEqual("Hello", result.Tokens[0]);
        Assert.AreEqual("world", result.Tokens[1]);
    }

    [Test]
    public void DefaultTokenizerTestQuotes()
    {
        var result = _tokenizer.Tokenize("\"Hello World\"");
        
        Assert.True(result.Success);
        Assert.AreEqual("Hello World", result.Tokens[0]);
    }
    
    [Test]
    public void DefaultTokenizerTestSpacesAndQuotes()
    {
        var result = _tokenizer.Tokenize("\"Hello World\" should be a token.");
        
        Assert.True(result.Success);
        Assert.AreEqual("Hello World", result.Tokens[0]);
        Assert.AreEqual("should", result.Tokens[1]);
        Assert.AreEqual("be", result.Tokens[2]);
        Assert.AreEqual("a", result.Tokens[3]);
        Assert.AreEqual("token.", result.Tokens[4]);
    }
}
