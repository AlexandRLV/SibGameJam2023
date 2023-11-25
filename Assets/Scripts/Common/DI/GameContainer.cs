using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Common.DI
{
    public static class GameContainer
    {
        public static Container Common { get; set; }
        public static Container InGame { get; set; }

        public static void InjectToInstance<T>(T instance) where T : class
        {
            var type = typeof(T);
            
            InjectFields(instance, type);
            InjectMethods(type, instance);
        }

        // Method for creating instance of common C# class
        public static T Create<T>()
        {
            var type = typeof(T);
            var constructors = type.GetConstructors()
                .Where(x => x.IsDefined(typeof(ConstructAttribute)));

            var constructor = constructors.FirstOrDefault();
            if (constructor == null)
                return Activator.CreateInstance<T>();

            var parameters = constructor.GetParameters();
            object[] parametersValues = ArrayPool<object>.New(parameters.Length);
            
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                if (!TryResolve(parameterType, out object value))
                    throw new ArgumentException(
                        $"Cannot create instance of {type.Name}: its first [Inject] constructor has unknown dependency: {parameterType.Name}");

                parametersValues[i] = value;
            }

            object instance = constructor.Invoke(parametersValues);
            InjectToInstance(instance);
            return (T)instance;
        }
        
        // Method for instantiating prefab and passing all [Inject] fields and methods in it
        public static T InstantiateAndResolve<T>(T prefab) where T : MonoBehaviour
        {
            var spawnedObject = Object.Instantiate(prefab);
            InjectToInstance(spawnedObject);
            return spawnedObject;
        }
        
        public static T InstantiateAndResolve<T>(T prefab, Transform parent) where T : MonoBehaviour
        {
            var spawnedObject = Object.Instantiate(prefab, parent);
            InjectToInstance(spawnedObject);
            return spawnedObject;
        }

        private static void InjectFields(object spawnedObject, Type type)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(x => x.IsDefined(typeof(InjectAttribute)));
            
            foreach (var field in fields)
            {
                var fieldType = field.FieldType;
                if (!TryResolve(fieldType, out object value))
                {
                    throw new ArgumentException(
                        $"Cannot inject type {type.Name}! There's no registration for {fieldType.Name} in both containers!");
                }
                
                field.SetValue(spawnedObject, value);
            }
        }

        private static void InjectMethods(Type type, object spawnedObject)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(x => x.IsDefined(typeof(ConstructAttribute)));
            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                var parametersValues = ArrayPool<object>.New(parameters.Length);
                if (parameters.Length > 0)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var parameter = parameters[i];
                        var parameterType = parameter.ParameterType;
                        
                        if (!TryResolve(parameterType, out object value))
                        {
                            throw new ArgumentException(
                                $"Cannot inject type {type.Name}! There's no registration for {parameterType.Name} in both containers!");
                        }

                        parametersValues[i] = value;
                    }
                }

                method.Invoke(spawnedObject, parametersValues);
                ArrayPool<object>.Free(parametersValues);
            }
        }

        private static bool TryResolve(Type type, out object value)
        {
            if (Common.HasRegistration(type))
            {
                value = Common.Resolve(type);
                return true;
            }
                        
            if (InGame != null && InGame.HasRegistration(type))
            {
                value = InGame.Resolve(type);
                return true;
            }

            value = null;
            return false;
        } 
    }
}