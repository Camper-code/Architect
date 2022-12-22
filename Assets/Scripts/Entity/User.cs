using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ParentModule("Entity")]
public class User : MonoBehaviour
{
    public Manager manager;

    public Weapon gun;

    public void OnInjected()
    {
        Debug.Log(manager.n);
    }
}
