using System;

namespace DeveloperConsole.Command
{
    public class InRangeAttributeValidator : IAttributeValidator
    {
        private object _setValue;
        private float _numericalValue;

        private float _min;
        private float _max;

        public InRangeAttributeValidator(float min, float max)
        {
            _min = min;
            _max = max;
        }

        public void Record(RecordingContext context)
        {
            _setValue = context.ArgumentValue;
        }

        public bool Validate(ArgumentSpecification _)
        {
            try
            {
                float value = Convert.ToSingle(_setValue);
                _numericalValue = value;
                return _numericalValue >= _min && _numericalValue <= _max;
            }
            catch
            {
                return false;
            }
        }

        public string ErrorMessage() => $"Value {_numericalValue} is not in the range [{_min}, {_max}].";
    }
}
