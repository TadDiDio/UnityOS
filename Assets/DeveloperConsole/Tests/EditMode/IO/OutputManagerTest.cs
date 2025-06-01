using NUnit.Framework;

namespace DeveloperConsole.Tests
{
    public class OutputManagerTest
    {
        #region TEST TYPES
        private class TestOutputSink1 : IOutputSink
        {
            public void ReceiveOutput(string message)
            {
                throw new System.NotImplementedException();
            }
        }
        private class TestOutputSink2 : IOutputSink
        {
            public void ReceiveOutput(string message)
            {
                throw new System.NotImplementedException();
            }
        }
        #endregion

        private IOutputManager _outputManager;
        [SetUp]
        public void SetUp()
        {
            _outputManager = new OutputManager();
        }
        
        [Test]
        public void ConsoleInputManager_All()
        {
            // Test registering
            
            IOutputSink sink1 = new TestOutputSink1();
            IOutputSink sink2 = new TestOutputSink2();
            IOutputSink sink3 = new TestOutputSink1();
            
            Assert.IsEmpty(_outputManager.OutputSinks);
            
            _outputManager.RegisterOutputSink(sink1);
            Assert.That(_outputManager.OutputSinks, Has.Exactly(1).EqualTo(sink1));
            
            _outputManager.RegisterOutputSink(sink2);
            Assert.AreEqual(_outputManager.OutputSinks[1], sink2);
            
            _outputManager.RegisterOutputSink(sink3);
            Assert.AreEqual(_outputManager.OutputSinks[2], sink3);
            
            _outputManager.RegisterOutputSink(sink1);
            Assert.AreEqual(_outputManager.OutputSinks.Count, 3);
            
            // Test Unregistering
            _outputManager.UnregisterOutputSink(sink1);
            Assert.AreEqual(_outputManager.OutputSinks[0], sink2);
            Assert.AreEqual(_outputManager.OutputSinks[1], sink3);
            
            _outputManager.UnregisterAllOutputSinks();
            Assert.IsEmpty(_outputManager.OutputSinks);
        }
    }
}