using System;
using Object = UnityEngine.Object;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BindingAttribute : ValidatedArgument
    {
        private Type _type;
        private string _tag;
        private string _name;
        public BindingAttribute(string name = "", string tag = "")
        {
            _tag = tag;
            _name = name;
        }
        
        public override bool Validate(AttributeValidationData data)
        {
            _type = data.FieldInfo.FieldType;
            return ConsoleAPI.TryGetBinding(data.FieldInfo.FieldType, _name, _tag, out var obj) && BindObject(data, obj);
        }

        private bool BindObject(AttributeValidationData data, Object target)
        {
            try
            {
                data.FieldInfo.SetValue(data.Object, target);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string ErrorMessage()
        {
            return $"There is no binding of type '{_type}' and one could not be found in the loaded scenes. " +
                   $"Try using the 'bind' command to set a binding.";
        }
    }
}