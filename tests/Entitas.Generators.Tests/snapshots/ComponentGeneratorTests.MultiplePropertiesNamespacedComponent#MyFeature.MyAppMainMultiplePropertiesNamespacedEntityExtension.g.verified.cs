﻿//HintName: MyFeature.MyAppMainMultiplePropertiesNamespacedEntityExtension.g.cs
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by
//     Entitas.Generators.ComponentGenerator.EntityExtension
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace MyFeature
{
public static class MyAppMainMultiplePropertiesNamespacedEntityExtension
{
    public static bool HasMultiplePropertiesNamespaced(this global::MyApp.Main.Entity entity)
    {
        return entity.HasComponent(MyAppMainMultiplePropertiesNamespacedComponentIndex.Index.Value);
    }

    public static global::MyApp.Main.Entity AddMultiplePropertiesNamespaced(this global::MyApp.Main.Entity entity, string value1, string value2, string value3)
    {
        var index = MyAppMainMultiplePropertiesNamespacedComponentIndex.Index.Value;
        var componentPool = entity.GetComponentPool(index);
        var component = componentPool.Count > 0
            ? (MultiplePropertiesNamespacedComponent)componentPool.Pop()
            : new MultiplePropertiesNamespacedComponent();
        component.Value1 = value1;
        component.Value2 = value2;
        component.Value3 = value3;
        entity.AddComponent(index, component);
        return entity;
    }

    public static global::MyApp.Main.Entity ReplaceMultiplePropertiesNamespaced(this global::MyApp.Main.Entity entity, string value1, string value2, string value3)
    {
        var index = MyAppMainMultiplePropertiesNamespacedComponentIndex.Index.Value;
        var componentPool = entity.GetComponentPool(index);
        var component = componentPool.Count > 0
            ? (MultiplePropertiesNamespacedComponent)componentPool.Pop()
            : new MultiplePropertiesNamespacedComponent();
        component.Value1 = value1;
        component.Value2 = value2;
        component.Value3 = value3;
        entity.ReplaceComponent(index, component);
        return entity;
    }

    public static global::MyApp.Main.Entity RemoveMultiplePropertiesNamespaced(this global::MyApp.Main.Entity entity)
    {
        entity.RemoveComponent(MyAppMainMultiplePropertiesNamespacedComponentIndex.Index.Value);
        return entity;
    }

    public static MultiplePropertiesNamespacedComponent GetMultiplePropertiesNamespaced(this global::MyApp.Main.Entity entity)
    {
        return (MultiplePropertiesNamespacedComponent)entity.GetComponent(MyAppMainMultiplePropertiesNamespacedComponentIndex.Index.Value);
    }
}
}
