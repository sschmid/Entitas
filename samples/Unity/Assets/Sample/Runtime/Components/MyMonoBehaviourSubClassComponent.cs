using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

[Context(typeof(GameContext))]
public sealed class MyMonoBehaviourSubClassComponent : IComponent
{
    public MyMonoBehaviourSubClass Value;
}

public class MyMonoBehaviourSubClass : MonoBehaviour { }
