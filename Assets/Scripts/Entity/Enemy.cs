using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Injection;
using Architect;

[ParentModule("Entity")]
public class Enemy : MonoBehaviour, IAutoInjectObject
{
    [Inject] private Manager manager;

    public Weapon gun;

    public void OnInjected()
    {
        Debug.LogWarning(manager.n);
    }
}
