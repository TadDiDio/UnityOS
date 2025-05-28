using System;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InRange : ValidatedAttribute
    {
        private float Min;
        private float Max;

        public InRange(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public override bool Validate(AttributeValidationData data)
        {
            try
            {
                float value = Convert.ToSingle(data.FieldInfo.GetValue(data.Object));
                return value >= Min && value <= Max;
            }
            catch
            {
                return false;
            }
        }
    }
}