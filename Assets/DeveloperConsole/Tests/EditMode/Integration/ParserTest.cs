using System.Collections.Generic;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Tokenizing;
using NUnit.Framework;
using UnityEngine;

namespace DeveloperConsole.Tests.EditMode.Integration
{
    public class ParserIntegrationTest
    {
        private IParser _parser;
        private MockParseTarget _parseTarget;
        
        [SetUp]
        public void SetUp()
        {
            _parser = new Parser(new DefaultTokenizer());
            _parseTarget = null;
        }
        
        [Test]
        public void Parser_ShouldSucceed_WithNoArguments()
        {
            List<string> tokens = new() { };
            TokenStream stream = new TokenStream(tokens);
        
            var commandType = new CommandBuilder()
                .WithName("test")
                .BuildType();
        
            var specs = ArgumentSpecification.GetAllFromType(commandType);
            _parseTarget = new MockParseTarget(specs);
        
            var result = _parser.Parse(stream, _parseTarget);
            
            Assert.AreEqual(Status.Success, result.Status);
        }
        
        [Test]
        public void Parser_ShouldSucceed_WithOneArgument()
        {
            List<string> tokens = new() { "-f" };
            TokenStream stream = new TokenStream(tokens);
        
            var commandType = new CommandBuilder()
                .WithName("test")
                .WithSwitch("flag", typeof(bool))
                .BuildType();
        
            var specs = ArgumentSpecification.GetAllFromType(commandType);
            _parseTarget = new MockParseTarget(specs);
        
            var result = _parser.Parse(stream, _parseTarget);
            
            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(true, _parseTarget.GetArg("flag"));
        }

        [Test]
        public void Parser_ShouldSucceed_WithMultipleArgument()
        {
            List<string> tokens = new() { "--flag", "filename", "-1", "0", "1" };
            TokenStream stream = new TokenStream(tokens);
        
            var commandType = new CommandBuilder()
                .WithName("test")
                .WithSwitch("flag", typeof(bool))
                .WithPositional("file", typeof(string))
                .WithVariadic("nums", typeof(int))
                .BuildType();
        
            var specs = ArgumentSpecification.GetAllFromType(commandType);
        
            _parseTarget = new MockParseTarget(specs);
        
            var result = _parser.Parse(stream, _parseTarget);
            
            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(true, _parseTarget.GetArg("flag"));
            Assert.AreEqual("filename", _parseTarget.GetArg("file"));
            CollectionAssert.AreEqual(new List<int> {-1, 0, 1}, (List<int>)_parseTarget.GetArg("nums"));
        }
        
        [Test]
        public void Parser_ShouldSucceed_WithMultipleExplicitFlagArguments()
        {
            List<string> tokens = new() { "--flag", "false", "filename", "-1", "0", "1" };
            TokenStream stream = new TokenStream(tokens);
        
            var commandType = new CommandBuilder()
                .WithName("test")
                .WithSwitch("flag", typeof(bool))
                .WithPositional("file", typeof(string))
                .WithVariadic("nums", typeof(int))
                .BuildType();
        
            var specs = ArgumentSpecification.GetAllFromType(commandType);
        
            _parseTarget = new MockParseTarget(specs);
        
            var result = _parser.Parse(stream, _parseTarget);
            
            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(false, _parseTarget.GetArg("flag"));
            Assert.AreEqual("filename", _parseTarget.GetArg("file"));
            CollectionAssert.AreEqual(new List<int> {-1, 0, 1}, (List<int>)_parseTarget.GetArg("nums"));
        }
        
        [Test]
        public void Parser_ShouldSucceed_WithMultipleExplicitFlagAndMultiplePositionalArguments()
        {
            List<string> tokens = new() { "filename", "--flag", "false", "freddo", "-1", "0", "1" };
            TokenStream stream = new TokenStream(tokens);

            var commandType = new CommandBuilder()
                .WithName("test")
                .WithSwitch("flag", typeof(bool))
                .WithPositional("file", typeof(string))
                .WithPositional("name", typeof(string), 1)
                .WithVariadic("nums", typeof(int))
                .BuildType();

            var specs = ArgumentSpecification.GetAllFromType(commandType);
            _parseTarget = new MockParseTarget(specs);

            var result = _parser.Parse(stream, _parseTarget);
            
            Assert.AreEqual(Status.Success, result.Status);
            Assert.AreEqual(false, _parseTarget.GetArg("flag"));
            Assert.AreEqual("filename", _parseTarget.GetArg("file"));
            Assert.AreEqual("freddo", _parseTarget.GetArg("name"));
            CollectionAssert.AreEqual(new List<int> {-1, 0, 1}, (List<int>)_parseTarget.GetArg("nums"));
        }
        
        // [Test]
        // public void Parser_ShouldSucceed_HugeCombination()
        // {
        //     List<string> tokens = new() { "--no-help", "-p", "1", "0", "-no-a", "false", "filename", "--flag", "false", "freddo", "0", "-1", "0", "1" };
        //     TokenStream stream = new TokenStream(tokens);
        //
        //     var commandType = new CommandBuilder()
        //         .WithName("test")
        //         .WithSwitch("flag", typeof(bool))
        //         .WithSwitch("help", typeof(bool))
        //         .WithSwitch("position", typeof(Vector2))
        //         .WithSwitch("act", typeof(bool))
        //         .WithPositional("file", typeof(string), 0)
        //         .WithPositional("name", typeof(string), 1)
        //         .WithPositional("count", typeof(int), 2)
        //         .WithVariadic("nums", typeof(int))
        //         .BuildType();
        //
        //     var specs = ArgumentSpecification.GetAllFromType(commandType);
        //     _parseTarget = new MockParseTarget(specs);
        //
        //     var result = _parser.Parse(stream, _parseTarget);
        //     
        //     Assert.AreEqual(Status.Success, result.Status);
        //     Assert.AreEqual(false, _parseTarget.GetArg("flag"));
        //     Assert.AreEqual(new Vector2(1, 0), _parseTarget.GetArg("position"));
        //     Assert.AreEqual(false, _parseTarget.GetArg("help"));
        //     Assert.AreEqual(true, _parseTarget.GetArg("act"));
        //     Assert.AreEqual("filename", _parseTarget.GetArg("file"));
        //     Assert.AreEqual("freddo", _parseTarget.GetArg("name"));
        //     Assert.AreEqual(0, _parseTarget.GetArg("count"));
        //     CollectionAssert.AreEqual(new List<int> {-1, 0, 1}, (List<int>)_parseTarget.GetArg("nums"));
        // }
        //
        
        // [Test]
        // public void Parser_ShouldSucceed_HugeCombinationMultipleOrders()
        // {
        //     var commandType = new CommandBuilder()
        //         .WithName("test")
        //         .WithSwitch("flag", typeof(bool))
        //         .WithSwitch("help", typeof(bool))
        //         .WithSwitch("position", typeof(Vector2))
        //         .WithSwitch("act", typeof(bool))
        //         .WithPositional("file", typeof(string), 0)
        //         .WithPositional("name", typeof(string), 1)
        //         .WithPositional("count", typeof(int), 2)
        //         .WithVariadic("nums", typeof(int))
        //         .BuildType();
        //     
        //     List<string> tokens = new() { "--no-help", "-no-a", "false", "filename", "--flag", "False", "freddo", "0", "-p", "1", "0", "-1", "0", "1" };
        //     List<string> tokens1 = new() { "filename", "--flag", "False", "freddo", "--no-help", "-no-a", "false", "0", "-p", "1", "0", "-1", "0", "1" };
        //     List<string> tokens2 = new() { "-no-a", "false", "--no-help", "filename", "--flag", "False", "freddo", "0", "-p", "1", "0", "-1", "0", "1" };
        //     List<string> tokens3 = new() { "--no-help", "-no-a", "false", "filename", "freddo", "0", "--no-flag", "-p", "1", "0", "-1", "0", "1" };
        //
        //     List<List<string>> tokenLists = new() { tokens, tokens1, tokens2, tokens3 };
        //     for (int i = 0; i < 4; i++)
        //     {
        //         _parser = new Parser(new DefaultTokenizer());
        //         var specs = ArgumentSpecification.GetAllFromType(commandType);
        //         _parseTarget = new MockParseTarget(specs);
        //
        //         TokenStream stream = new TokenStream(tokenLists[i]);
        //         var result = _parser.Parse(stream, _parseTarget);
        //         
        //         if (result.Status != Status.Success) Log.Info(result.ErrorMessage);
        //         
        //         Assert.AreEqual(Status.Success, result.Status);
        //         Assert.AreEqual(false, _parseTarget.GetArg("flag"));
        //         Assert.AreEqual(new Vector2(1, 0), _parseTarget.GetArg("position"));
        //         Assert.AreEqual(false, _parseTarget.GetArg("help"));
        //         Assert.AreEqual(true, _parseTarget.GetArg("act"));
        //         Assert.AreEqual("filename", _parseTarget.GetArg("file"));
        //         Assert.AreEqual("freddo", _parseTarget.GetArg("name"));
        //         Assert.AreEqual(0, _parseTarget.GetArg("count"));
        //         CollectionAssert.AreEqual(new List<int> {-1, 0, 1}, (List<int>)_parseTarget.GetArg("nums"));
        //     }
        // }
        
        
        [Test]
        public void Parser_ShouldFail_UnexpectedToken()
        {
            List<string> tokens = new() { "invalid" };
            TokenStream stream = new TokenStream(tokens);

            var commandType = new CommandBuilder()
                .WithName("test")
                .BuildType();

            var specs = ArgumentSpecification.GetAllFromType(commandType);
            _parseTarget = new MockParseTarget(specs);

            var result = _parser.Parse(stream, _parseTarget);
            
            Assert.AreEqual(Status.Fail, result.Status);
            Assert.AreEqual($"Saw an unexpected token: 'invalid'", result.ErrorMessage);
        }
        
        // [Test]
        // public void Parser_ShouldFail_BadTypeParse()
        // {
        //     List<string> tokens = new() { "--num", "1.5" };
        //     TokenStream stream = new TokenStream(tokens);
        //
        //     var commandType = new CommandBuilder()
        //         .WithName("test")
        //         .WithSwitch("num", typeof(int))
        //         .BuildType();
        //
        //     var specs = ArgumentSpecification.GetAllFromType(commandType);
        //     _parseTarget = new MockParseTarget(specs);
        //
        //     var result = _parser.Parse(stream, _parseTarget);
        //     
        //     Assert.AreEqual(Status.Fail, result.Status);
        //     Assert.AreEqual($"Failed to parse '1.5' (num) as a int", result.ErrorMessage);
        // }
        
        // [Test]
        // public void Parser_ShouldFail_MultiTokenBadTypeParse()
        // {
        //     List<string> tokens = new() { "--vec", "1.5", "a" };
        //     TokenStream stream = new TokenStream(tokens);
        //
        //     var commandType = new CommandBuilder()
        //         .WithName("test")
        //         .WithSwitch("vec", typeof(Vector2))
        //         .BuildType();
        //
        //     var specs = ArgumentSpecification.GetAllFromType(commandType);
        //     _parseTarget = new MockParseTarget(specs);
        //
        //     var result = _parser.Parse(stream, _parseTarget);
        //     
        //     Assert.AreEqual(Status.Fail, result.Status);
        //     Assert.AreEqual($"Failed to parse '1.5, a' (vec) as a Vector2", result.ErrorMessage);
        // }
        
        private class MockParseTarget : IParseTarget
        {
            private HashSet<ArgumentSpecification> _specs;
            private Dictionary<string, object> _args = new();
            
            public MockParseTarget(HashSet<ArgumentSpecification> args) => _specs = args;
            public HashSet<ArgumentSpecification> GetArguments() => _specs;

            public bool Validate(out string errorMessage)
            {
                errorMessage = null;
                return true;
            }

            public void SetArgument(ArgumentSpecification argument, object argValue)
            {
                _args[argument.Name] = argValue;
            }

            public object GetArg(string name) => _args[name];
        }
    }
}