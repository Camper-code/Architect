using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Injection;
using Architect;

[ParentModule("Global")]
public class SceneLoader : MonoBehaviour
{
    public User user;
    public Enemy enemy;
    public Manager manager;

    private void Awake()
    {
        Injection.Injector.AddToBase(manager);

        Injection.Injector.Inject(user);
        Injection.Injector.Inject(enemy);
    }
}
