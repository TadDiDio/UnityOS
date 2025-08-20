using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeveloperConsole.Windowing
{
    public class WindowModel
    {
        public string Id { get; }
        public string Title { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public bool IsVisible { get; set; } = true;

        private readonly Dictionary<Type, object> _dynamicProperties = new();

        /// <summary>
        /// Adds a window feature.
        /// </summary>
        /// <param name="feature">The feature to add.</param>
        /// <typeparam name="T">The type of the feature.</typeparam>
        public void AddFeature<T>(T feature)
        {
            _dynamicProperties[typeof(T)] = feature;
        }

        public bool TryGetFeature<T>(out T feature)
        {
            if (_dynamicProperties.TryGetValue(typeof(T), out var obj))
            {
                feature = (T)obj;
                return true;
            }

            feature = default;
            return false;
        }

        public event Action OnChanged;
        public void NotifyChanged() => OnChanged?.Invoke();
    }
}
