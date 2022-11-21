using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Injection;

[ParentModule("Entity")]
public class User : MonoBehaviour, IAutoInjectObject
{
    [Inject] private Manager manager;

    public Weapon gun;

    public void OnInjected()
    {
        Debug.Log(manager.n);
    }
}
