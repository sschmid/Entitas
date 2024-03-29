﻿//HintName: MyFeature.MyAppMainAnyFlagEventNamespacedRemovedListenerComponent.g.cs
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by
//     Entitas.Generators.ComponentGenerator.Events
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace MyFeature
{
public interface IMyAppMainAnyFlagEventNamespacedRemovedListener
{
    void OnAnyFlagEventNamespacedRemoved(global::MyApp.Main.Entity entity);
}

public sealed class MyAppMainAnyFlagEventNamespacedRemovedListenerComponent : global::Entitas.IComponent
{
    public global::System.Collections.Generic.List<IMyAppMainAnyFlagEventNamespacedRemovedListener> Value;
}

public static class MyAppMainAnyFlagEventNamespacedRemovedListenerEventEntityExtension
{
    public static global::MyApp.Main.Entity AddAnyFlagEventNamespacedRemovedListener(this global::MyApp.Main.Entity entity, IMyAppMainAnyFlagEventNamespacedRemovedListener value)
    {
        var listeners = entity.HasAnyFlagEventNamespacedRemovedListener()
            ? entity.GetAnyFlagEventNamespacedRemovedListener().Value
            : new global::System.Collections.Generic.List<IMyAppMainAnyFlagEventNamespacedRemovedListener>();
        listeners.Add(value);
        return entity.ReplaceAnyFlagEventNamespacedRemovedListener(listeners);
    }

    public static void RemoveAnyFlagEventNamespacedRemovedListener(this global::MyApp.Main.Entity entity, IMyAppMainAnyFlagEventNamespacedRemovedListener value, bool removeListenerWhenEmpty = true)
    {
        var listeners = entity.GetAnyFlagEventNamespacedRemovedListener().Value;
        listeners.Remove(value);
        if (removeListenerWhenEmpty && listeners.Count == 0)
        {
            entity.RemoveAnyFlagEventNamespacedRemovedListener();
            if (entity.IsEmpty())
                entity.Destroy();
        }
        else
        {
            entity.ReplaceAnyFlagEventNamespacedRemovedListener(listeners);
        }
    }
}

public sealed class MyAppMainAnyFlagEventNamespacedRemovedEventSystem : global::Entitas.ReactiveSystem<global::MyApp.Main.Entity>
{
    readonly global::Entitas.IGroup<global::MyApp.Main.Entity> _listeners;
    readonly global::System.Collections.Generic.List<global::MyApp.Main.Entity> _entityBuffer;
    readonly global::System.Collections.Generic.List<IMyAppMainAnyFlagEventNamespacedRemovedListener> _listenerBuffer;

    public MyAppMainAnyFlagEventNamespacedRemovedEventSystem(MyApp.MainContext context) : base(context)
    {
        _listeners = context.GetGroup(MyAppMainAnyFlagEventNamespacedRemovedListenerMatcher.AnyFlagEventNamespacedRemovedListener);
        _entityBuffer = new global::System.Collections.Generic.List<global::MyApp.Main.Entity>();
        _listenerBuffer = new global::System.Collections.Generic.List<IMyAppMainAnyFlagEventNamespacedRemovedListener>();
    }

    protected override global::Entitas.ICollector<global::MyApp.Main.Entity> GetTrigger(global::Entitas.IContext<global::MyApp.Main.Entity> context)
    {
        return global::Entitas.CollectorContextExtension.CreateCollector(
            context, global::Entitas.TriggerOnEventMatcherExtension.Removed(MyAppMainFlagEventNamespacedMatcher.FlagEventNamespaced)
        );
    }

    protected override bool Filter(global::MyApp.Main.Entity entity)
    {
        return !entity.HasFlagEventNamespaced();
    }

    protected override void Execute(global::System.Collections.Generic.List<global::MyApp.Main.Entity> entities)
    {
        foreach (var entity in entities)
        {
            foreach (var listenerEntity in _listeners.GetEntities(_entityBuffer))
            {
                _listenerBuffer.Clear();
                _listenerBuffer.AddRange(listenerEntity.GetAnyFlagEventNamespacedRemovedListener().Value);
                foreach (var listener in _listenerBuffer)
                {
                    listener.OnAnyFlagEventNamespacedRemoved(entity);
                }
            }
        }
    }
}
}
