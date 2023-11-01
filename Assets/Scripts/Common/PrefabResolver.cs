using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Pool;

namespace Common
{
    [DefaultExecutionOrder(-1000)]
    public class PrefabResolver : MonoBehaviour
    {
        private void Awake()
        {
            var cachedComponents = DictionaryPool<Type, object>.Get();
            
            var attributeType = typeof(ResolveComponentAttribute);
            foreach (var component in gameObject.GetComponentsInChildren<MonoBehaviour>())
            {
                var allFields = component.GetType()
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                var fields = component.GetType()
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(x => x.IsDefined(attributeType, true));
                
                foreach (var field in fields)
                {
                    var type = field.FieldType;
                    if (!cachedComponents.TryGetValue(type, out object targetComponent))
                    {
                        targetComponent = gameObject.GetComponentInChildren(type);
                        cachedComponents.Add(type, targetComponent);
                    }
                    field.SetValue(component, targetComponent);
                }

                var properties = component.GetType().GetProperties().Where(x => x.IsDefined(attributeType, true));
                foreach (var property in properties)
                {
                    var type = property.PropertyType;
                    if (!cachedComponents.TryGetValue(type, out object targetComponent))
                    {
                        targetComponent = gameObject.GetComponentInChildren(type);
                        cachedComponents.Add(type, targetComponent);
                    }
                    property.SetValue(component, targetComponent);
                }
            }
            
            DictionaryPool<Type, object>.Release(cachedComponents);
        }
    }
}