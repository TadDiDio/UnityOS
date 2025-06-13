using UnityEngine;

namespace DeveloperConsole
{
    /// <summary>
    /// Forwards logical and gui update events to the kernel while in play mode.
    /// </summary>
    public class PlayModeTicker : MonoBehaviour
    {
        private KernelUpdater _updater;
        
        /// <summary>
        /// Sets the kernel updater.
        /// </summary>
        /// <param name="updater">The updater.</param>
        public void SetUpdater(KernelUpdater updater) => _updater = updater;    
        
        private void Update() => _updater.Tick();

        private void OnGUI()
        {
            _updater.Input(Event.current);
            _updater.Draw(Screen.width, Screen.height, false);
        }
    }
}