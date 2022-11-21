using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Injection
{
    public class Injector
    {
        static private Dictionary<Type, object> objectBase = new Dictionary<Type, object>();

        private static void ClearBase()
        {
            objectBase = objectBase.Where(o => o.Value != null).ToDictionary(o => o.Key, o => o.Value);
        }

        public static void AddToBase<T>(T target)
        {
            ClearBase();
            if (objectBase.ContainsKey(target.GetType()))
            {
                objectBase[target.GetType()] = target;
                UnityEngine.Debug.LogWarning($"Base contains same object {target.GetType()}. Object has been replaced");
            }
            else
                objectBase.Add(target.GetType(), target);
        }

        public static void Inject<T>(T target)
        {
            ClearBase();

            target.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(f => Attribute.IsDefined(f, typeof(InjectAttribute)))
                .ToList().ForEach(f => InjectField(target, f));
        }

        private static void InjectField<T>(T target, System.Reflection.FieldInfo field)
        {
            if (objectBase.TryGetValue(field.FieldType, out object obj))
                field.SetValue(target, obj);
        }
    }
}
