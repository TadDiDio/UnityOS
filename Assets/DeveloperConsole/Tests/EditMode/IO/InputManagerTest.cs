using NUnit.Framework;

namespace DeveloperConsole.Tests
{
    public class InputManagerTest
    {
        #region TEST TYPES

        private class TestInputSource1 : IInputSource
        {
            public bool InputAvailable() => false;

            public string GetInput() => "";
        }

        private class TestInputSource2 : IInputSource
        {
            public bool InputAvailable() => false;

            public string GetInput() => "";
        }

        #endregion

        private IInputManager _inputManager;
        
        [SetUp]
        public void SetUp()
        {
            _inputManager = new InputManager();
        }

        [Test]
        public void ConsoleInputManager_All()
        {
            // Test registering
            
            IInputSource source1 = new TestInputSource1();
            IInputSource source2 = new TestInputSource2();
            IInputSource source3 = new TestInputSource1();
            
            Assert.IsEmpty(_inputManager.InputSources);
            
            _inputManager.RegisterInputSource(source1);
            Assert.That(_inputManager.InputSources, Has.Exactly(1).EqualTo(source1));
            
            _inputManager.RegisterInputSource(source2);
            Assert.AreEqual(_inputManager.InputSources[1], source2);
            
            _inputManager.RegisterInputSource(source3);
            Assert.AreEqual(_inputManager.InputSources[2], source3);
            
            _inputManager.RegisterInputSource(source1);
            Assert.AreEqual(_inputManager.InputSources.Count, 3);
            
            // Test Unregistering
            _inputManager.UnregisterInputSource(source1);
            Assert.AreEqual(_inputManager.InputSources[0], source2);
            Assert.AreEqual(_inputManager.InputSources[1], source3);
            
            _inputManager.UnregisterAllInputSources();
            Assert.IsEmpty(_inputManager.InputSources);
        }
    }
}