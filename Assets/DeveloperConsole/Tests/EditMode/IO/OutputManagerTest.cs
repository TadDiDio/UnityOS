using NUnit.Framework;

namespace DeveloperConsole.Tests
{
    public class OutputManagerTest : ConsoleResetTest
    {
        #region TEST TYPES
        private class TestOutputSink1 : IConsoleOutputSink
        {
            public void ReceiveOutput(string message)
            {
                throw new System.NotImplementedException();
            }
        }
        private class TestOutputSink2 : IConsoleOutputSink
        {
            public void ReceiveOutput(string message)
            {
                throw new System.NotImplementedException();
            }
        }
        #endregion
        
        [Test]
        public void ConsoleInputManager_All()
        {
            // Test registering
            
            IConsoleOutputSink sink1 = new TestOutputSink1();
            IConsoleOutputSink sink2 = new TestOutputSink2();
            IConsoleOutputSink sink3 = new TestOutputSink1();
            
            Assert.IsEmpty(OutputManager.OutputSinks);
            
            OutputManager.RegisterOutputSink(sink1);
            Assert.That(OutputManager.OutputSinks, Has.Exactly(1).EqualTo(sink1));
            
            OutputManager.RegisterOutputSink(sink2);
            Assert.AreEqual(OutputManager.OutputSinks[1], sink2);
            
            OutputManager.RegisterOutputSink(sink3);
            Assert.AreEqual(OutputManager.OutputSinks[2], sink3);
            
            OutputManager.RegisterOutputSink(sink1);
            Assert.AreEqual(OutputManager.OutputSinks.Count, 3);
            
            // Test Unregistering
            OutputManager.UnregisterOutputSink(sink1);
            Assert.AreEqual(OutputManager.OutputSinks[0], sink2);
            Assert.AreEqual(OutputManager.OutputSinks[1], sink3);
            
            OutputManager.UnregisterAllOutputSinks();
            Assert.IsEmpty(OutputManager.OutputSinks);
        }
    }
}