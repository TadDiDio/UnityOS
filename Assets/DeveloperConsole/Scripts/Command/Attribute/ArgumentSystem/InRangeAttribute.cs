using System;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Determines if an argument value is within a range.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class InRangeAttribute : Validated
    {
        private float Min;
        private float Max;

        private float testedValue;
        
        
        /// <summary>
        /// Creates a ranged arg.
        /// </summary>
        /// <param name="min">The min acceptable value.</param>
        /// <param name="max">The max acceptable value.</param>
        /// <param name="description">The arg's description.</param>
        /// <param name="overrideName">Overrides the field name.</param>
        public InRangeAttribute(float min, float max, string description, string overrideName = null)
            : base(description, overrideName)
        {
            Min = min;
            Max = max;
        }

        protected override bool Validate(AttributeValidationData data)
        {
            try
            {
                float value = Convert.ToSingle(data.FieldInfo.GetValue(data.Object));
                testedValue = value;
                return value >= Min && value <= Max;
            }
            catch
            {
                return false;
            }
        }

        public override string ErrorMessage() => $"Value {testedValue} is not in the range [{Min}, {Max}].";
    }
}