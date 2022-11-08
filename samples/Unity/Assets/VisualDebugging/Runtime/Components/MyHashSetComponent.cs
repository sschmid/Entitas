using System.Collections.Generic;
using Entitas;

[Game]
public class MyHashSetComponent : IComponent
{
    public HashSet<string> Value;
}
