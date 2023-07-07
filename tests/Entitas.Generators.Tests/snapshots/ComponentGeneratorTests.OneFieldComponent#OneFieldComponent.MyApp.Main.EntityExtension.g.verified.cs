﻿//HintName: OneFieldComponent.MyApp.Main.EntityExtension.g.cs
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by
//     Entitas.Generators.ComponentGenerator.EntityExtension
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable

using global::MyApp.Main;
using static global::MyAppMainOneFieldComponentIndex;

public static class MyAppMainOneFieldEntityExtension
{
    public static Entity SetOneField(this Entity entity, string value)
    {
        var index = Index.Value;
        var componentPool = entity.GetComponentPool(index);
        var component = componentPool.Count > 0
            ? (OneFieldComponent)componentPool.Pop()
            : new OneFieldComponent();
        component.Value = value;
        entity.ReplaceComponent(index, component);
        return entity;
    }

    public static Entity UnsetOneField(this Entity entity)
    {
        if (entity.HasComponent(Index.Value))
            entity.RemoveComponent(Index.Value);

        return entity;
    }

    public static OneFieldComponent? GetOneField(this Entity entity)
    {
        return entity.HasComponent(Index.Value)
            ? (OneFieldComponent)entity.GetComponent(Index.Value)
            : null;
    }
}
