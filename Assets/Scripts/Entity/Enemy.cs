using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architect;

[ParentModule("Entity")]
public class Enemy : MonoBehaviour
{
    public Manager manager;

    public Weapon gun;

    public void OnInjected()
    {
        Debug.LogWarning(manager.n);
    }
}
