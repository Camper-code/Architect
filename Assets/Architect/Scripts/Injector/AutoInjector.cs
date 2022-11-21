using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Injection
{

    public class AutoInjector : MonoBehaviour
    {
        private void Awake()
        {
            List<IAutoBaseObject> baseObjects = FindObjectsOfType<MonoBehaviour>().OfType<IAutoBaseObject>().ToList();
            List<IAutoInjectObject> injectObjects = FindObjectsOfType<MonoBehaviour>().OfType<IAutoInjectObject>().ToList();

            baseObjects.ForEach(o => Injector.AddToBase(o));

            injectObjects.ForEach(o => Injector.Inject(o));

            injectObjects.ForEach(o => o.OnInjected());
        }
    }
}