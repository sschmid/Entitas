﻿//HintName: MyFeature.MyAppMainReservedKeywordFieldsNamespacedEntityExtension.g.cs
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
public static class MyAppMainReservedKeywordFieldsNamespacedEntityExtension
{
    public static bool HasReservedKeywordFieldsNamespaced(this global::MyApp.Main.Entity entity)
    {
        return entity.HasComponent(MyAppMainReservedKeywordFieldsNamespacedComponentIndex.Index.Value);
    }

    public static global::MyApp.Main.Entity AddReservedKeywordFieldsNamespaced(this global::MyApp.Main.Entity entity, string @namespace, string @class, string @public)
    {
        var index = MyAppMainReservedKeywordFieldsNamespacedComponentIndex.Index.Value;
        var componentPool = entity.GetComponentPool(index);
        var component = componentPool.Count > 0
            ? (ReservedKeywordFieldsNamespacedComponent)componentPool.Pop()
            : new ReservedKeywordFieldsNamespacedComponent();
        component.Namespace = @namespace;
        component.Class = @class;
        component.Public = @public;
        entity.AddComponent(index, component);
        return entity;
    }

    public static global::MyApp.Main.Entity ReplaceReservedKeywordFieldsNamespaced(this global::MyApp.Main.Entity entity, string @namespace, string @class, string @public)
    {
        var index = MyAppMainReservedKeywordFieldsNamespacedComponentIndex.Index.Value;
        var componentPool = entity.GetComponentPool(index);
        var component = componentPool.Count > 0
            ? (ReservedKeywordFieldsNamespacedComponent)componentPool.Pop()
            : new ReservedKeywordFieldsNamespacedComponent();
        component.Namespace = @namespace;
        component.Class = @class;
        component.Public = @public;
        entity.ReplaceComponent(index, component);
        return entity;
    }

    public static global::MyApp.Main.Entity RemoveReservedKeywordFieldsNamespaced(this global::MyApp.Main.Entity entity)
    {
        entity.RemoveComponent(MyAppMainReservedKeywordFieldsNamespacedComponentIndex.Index.Value);
        return entity;
    }

    public static ReservedKeywordFieldsNamespacedComponent GetReservedKeywordFieldsNamespaced(this global::MyApp.Main.Entity entity)
    {
        return (ReservedKeywordFieldsNamespacedComponent)entity.GetComponent(MyAppMainReservedKeywordFieldsNamespacedComponentIndex.Index.Value);
    }
}
}
