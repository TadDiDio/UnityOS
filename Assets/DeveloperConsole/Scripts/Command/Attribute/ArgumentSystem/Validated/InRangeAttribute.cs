using System;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Determines if an argument value is within a range.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class InRangeAttribute : ValidatedAttribute
    {
        public readonly float Min;
        public readonly float Max;
        
        private object _setValue;
        private float _numericalValue;
        
        /// <summary>
        /// Creates a ranged arg.
        /// </summary>
        /// <param name="min">The min acceptable value.</param>
        /// <param name="max">The max acceptable value.</param>
        public InRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public override void Record(RecordingContext context)
        {
            _setValue = context.ArgumentValue;
        }

        public override bool Validate(ArgumentSpecification _)
        {
            try
            {
                float value = Convert.ToSingle(_setValue);
                _numericalValue = value;
                return _numericalValue >= Min && _numericalValue <= Max;
            }
            catch
            {
                return false;
            }
        }

        public override string ErrorMessage() => $"Value {_numericalValue} is not in the range [{Min}, {Max}].";
    }
}