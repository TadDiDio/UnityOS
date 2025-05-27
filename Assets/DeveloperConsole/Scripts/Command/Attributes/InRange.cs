using System;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InRange : Attribute
    {
        public float Min;
        public float Max;

        public InRange(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}