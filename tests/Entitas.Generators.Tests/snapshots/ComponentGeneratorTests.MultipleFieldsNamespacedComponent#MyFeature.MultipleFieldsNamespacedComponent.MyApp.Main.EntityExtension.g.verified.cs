﻿//HintName: MyFeature.MultipleFieldsNamespacedComponent.MyApp.Main.EntityExtension.g.cs
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
using static global::MyFeature.MyAppMainMultipleFieldsNamespacedComponentIndex;

namespace MyFeature
{
public static class MyAppMainMultipleFieldsNamespacedEntityExtension
{
    public static Entity SetMultipleFieldsNamespaced(this Entity entity, string value1, string value2, string value3)
    {
        var index = Index.Value;
        var componentPool = entity.GetComponentPool(index);
        var component = componentPool.Count > 0
            ? (MultipleFieldsNamespacedComponent)componentPool.Pop()
            : new MultipleFieldsNamespacedComponent();
        component.Value1 = value1;
        component.Value2 = value2;
        component.Value3 = value3;
        entity.ReplaceComponent(index, component);
        return entity;
    }

    public static Entity UnsetMultipleFieldsNamespaced(this Entity entity)
    {
        if (entity.HasComponent(Index.Value))
            entity.RemoveComponent(Index.Value);

        return entity;
    }

    public static MultipleFieldsNamespacedComponent? GetMultipleFieldsNamespaced(this Entity entity)
    {
        return entity.HasComponent(Index.Value)
            ? (MultipleFieldsNamespacedComponent)entity.GetComponent(Index.Value)
            : null;
    }
}
}
