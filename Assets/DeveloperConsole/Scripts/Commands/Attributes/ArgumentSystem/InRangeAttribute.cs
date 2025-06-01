using System;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InRangeAttribute : ArgumentValidator
    {
        private float Min;
        private float Max;

        private float testedValue;
        public InRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public override bool Validate(AttributeValidationData data)
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