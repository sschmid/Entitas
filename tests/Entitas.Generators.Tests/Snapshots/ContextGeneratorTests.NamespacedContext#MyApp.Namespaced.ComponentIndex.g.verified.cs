﻿//HintName: MyApp.Namespaced.ComponentIndex.g.cs
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by
//     Entitas.Generators.ContextGenerator.ComponentIndex
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace MyApp.Namespaced
{
public readonly struct ComponentIndex : System.IEquatable<ComponentIndex>
{
    public static implicit operator int(ComponentIndex index) => index.Value;
    public static implicit operator ComponentIndex(int index) => new ComponentIndex(index);

    public readonly int Value;

    public ComponentIndex(int value)
    {
        Value = value;
    }

    public bool Equals(ComponentIndex other) => Value == other.Value;
#nullable enable
    public override bool Equals(object? obj) => obj is ComponentIndex other && Equals(other);
#nullable disable
    public override int GetHashCode() => Value;
}
}
