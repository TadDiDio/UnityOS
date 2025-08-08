using System;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Determines if an argument value is within a range.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class InRangeAttribute : ArgumentAttribute, IAttributeValidatorFactory
    {
        public readonly float Min;
        public readonly float Max;

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

        public IAttributeValidator CreateValidatorInstance()
        {
            return new InRangeAttributeValidator(Min, Max);
        }
    }
}
