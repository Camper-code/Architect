using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Injection;
using Architect;

[ParentModule("Global")]
public class Manager : MonoBehaviour, IAutoBaseObject
{
    private User u;
    public int n = 3000;
}
