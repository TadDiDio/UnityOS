using UnityEngine.UIElements;

namespace DeveloperConsole.Windowing
{
    public class TerminalClientBehavior : IWindowBehavior
    {
        private TerminalClientModel _model;
        private ScrollView _scrollView;
        private TextField _inputField;
        private int _lastRenderedIndex;

        public void Attach(WindowModel model, VisualElement root)
        {
            if (!model.TryGetFeature(out _model))
            {
                Log.Error("Terminal model not found in a terminal behavior.");
                return;
            }

            _model.OnUpdate += Refresh;

            var container = root.Q<VisualElement>(WindowController.ContentName);

            _scrollView = new ScrollView(ScrollViewMode.Vertical)
            {
                style =
                {
                    flexGrow = 1,
                    marginBottom = 2
                }
            };

            container.Add(_scrollView);

            _inputField = new TextField
            {
                style =
                {
                    height = 24,
                    marginTop = 2,
                    marginLeft = 2,
                    marginRight = 2
                }
            };

            // Submit on Enter
            _inputField.RegisterCallback<NavigationSubmitEvent>(evt => {
                _model.SubmitInput();
                evt.StopPropagation();
                _inputField.Focus();
            }, TrickleDown.TrickleDown);

            _inputField.RegisterValueChangedCallback(evt =>
            {
                _model.SetInputBuffer(evt.newValue);
            });

            container.Add(_inputField);

            Refresh();
        }

        private void Refresh()
        {
            // Update input
            _inputField.value = _model.InputBuffer;

            // Cleared the buffer
            if (_model.OutputBuffer.Count == 0)
            {
                _lastRenderedIndex = 0;
                _scrollView.Clear();
                return;
            }

            // Added to the buffer
            for (int i = _lastRenderedIndex; i < _model.OutputBuffer.Count; i++)
            {
                _scrollView.Add(new Label(_model.OutputBuffer[i]));
            }

            // Scroll to the bottom
            if (_scrollView.contentContainer.childCount > 0)
            {
                _scrollView.ScrollTo(_scrollView.contentContainer[_scrollView.contentContainer.childCount - 1]);
            }

            _lastRenderedIndex = _model.OutputBuffer.Count;
        }
    }
}
