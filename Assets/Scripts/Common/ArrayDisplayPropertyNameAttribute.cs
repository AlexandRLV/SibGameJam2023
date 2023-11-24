using UnityEngine;

namespace Common
{
    public class ArrayDisplayPropertyNameAttribute : PropertyAttribute
    {
        public string propertyName;
        
        public ArrayDisplayPropertyNameAttribute(string propertyName)
        {
            this.propertyName = propertyName;
        }
    }
}