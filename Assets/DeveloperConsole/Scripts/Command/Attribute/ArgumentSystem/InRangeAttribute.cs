using System;
using DeveloperConsole.Parsing;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Determines if an argument value is within a range.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class InRangeAttribute : ValidatedAttribute
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

        public override bool Validate(ParseContext context)
        {
            try
            {
                var result = context.Target.GetFirstArgumentMatchingAttribute(this);
                if (!result.HasValue) return false;

                var (_, rawValue) = result.Value;
                float value = Convert.ToSingle(rawValue);
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