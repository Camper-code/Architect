using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architect;

[ParentModule("Global")]
public class SceneLoader : MonoBehaviour
{
    public User user;
    public Enemy enemy;
    public Manager manager;

    private void Awake()
    {
        user.manager = manager;
        enemy.manager = manager;
    }
}
