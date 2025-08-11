using UnityEngine;

namespace DeveloperConsole.Windowing
{
    public class WindowConfigFactory
    {
        private WindowConfig _config = new();

        public WindowConfigFactory Draggable()
        {
            _config.IsDraggable = true;
            return this;
        }

        public WindowConfigFactory WithMinSize(float width, float height)
        {
            _config.MinSize = new Vector2(width, height);
            return this;
        }

        public WindowConfigFactory WithMinSize(Vector2 minSize)
        {
            _config.MinSize = minSize;
            return this;
        }

        public WindowConfigFactory WithMaxSize(Vector2 maxSize)
        {
            _config.MaxSize = maxSize;
            return this;
        }

        public WindowConfigFactory Closeable()
        {
            _config.Closeable = true;
            return this;
        }

        public WindowConfigFactory Minimizable()
        {
            _config.IsMinimizable = true;
            return this;
        }

        public WindowConfigFactory WithPadding(int padding)
        {
            _config.Padding = padding;
            return this;
        }

        public WindowConfigFactory WithHeaderHeight(int headerHeight)
        {
            _config.HeaderHeight = headerHeight;
            return this;
        }

        public WindowConfigFactory FullScreen()
        {
            _config.FullScreen = true;
            return this;
        }

        public WindowConfigFactory WithName(string name)
        {
            _config.Name = name;
            return this;
        }

        public WindowConfigFactory Resizeable()
        {
            _config.IsResizeable = true;
            return this;
        }

        public WindowConfig Build() => _config;


        public static WindowConfig CommandWindow()
        {
            return new WindowConfigFactory()
                .Draggable()
                .Closeable()
                .Resizeable()
                .WithPadding(0)
                .WithMinSize(200, 100)
                .Build();
        }
    }
}
