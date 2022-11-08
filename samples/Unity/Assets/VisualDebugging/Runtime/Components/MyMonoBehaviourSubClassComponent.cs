using Entitas;
using UnityEngine;

[Game]
public class MyMonoBehaviourSubClassComponent : IComponent
{
    public MyMonoBehaviourSubClass Value;
}

public class MyMonoBehaviourSubClass : MonoBehaviour { }
