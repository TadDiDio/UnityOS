using NUnit.Framework;

namespace DeveloperConsole.Tests
{
    public class InputManagerTest : ConsoleResetTest
    {
        #region TEST TYPES
        private class TestInputSource1 : IConsoleInputSource
        {
            public bool InputAvailable()
            {
                throw new System.NotImplementedException();
            }

            public string GetInput()
            {
                throw new System.NotImplementedException();
            }
        }

        private class TestInputSource2: IConsoleInputSource
        {
            public bool InputAvailable()
            {
                throw new System.NotImplementedException();
            }

            public string GetInput()
            {
                throw new System.NotImplementedException();
            }
        }
        #endregion
        
        [Test]
        public void ConsoleInputManager_All()
        {
            // Test registering
            
            IConsoleInputSource source1 = new TestInputSource1();
            IConsoleInputSource source2 = new TestInputSource2();
            IConsoleInputSource source3 = new TestInputSource1();
            
            Assert.IsEmpty(InputManager.InputMethods);
            
            InputManager.RegisterInputMethod(source1);
            Assert.That(InputManager.InputMethods, Has.Exactly(1).EqualTo(source1));
            
            InputManager.RegisterInputMethod(source2);
            Assert.AreEqual(InputManager.InputMethods[1], source2);
            
            InputManager.RegisterInputMethod(source3);
            Assert.AreEqual(InputManager.InputMethods[2], source3);
            
            InputManager.RegisterInputMethod(source1);
            Assert.AreEqual(InputManager.InputMethods.Count, 3);
            
            // Test Unregistering
            InputManager.UnregisterInputMethod(source1);
            Assert.AreEqual(InputManager.InputMethods[0], source2);
            Assert.AreEqual(InputManager.InputMethods[1], source3);
            
            InputManager.UnregisterAllInputMethods();
            Assert.IsEmpty(InputManager.InputMethods);
        }
    }
}