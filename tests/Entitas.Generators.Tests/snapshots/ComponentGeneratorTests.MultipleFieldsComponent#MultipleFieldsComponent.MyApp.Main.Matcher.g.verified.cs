﻿//HintName: MultipleFieldsComponent.MyApp.Main.Matcher.g.cs
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by
//     Entitas.Generators.ComponentGenerator.ComponentIndex
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable

using global::MyApp.Main;
using static global::MyAppMainMultipleFieldsComponentIndex;

public sealed class MyAppMainMultipleFieldsMatcher
{
    static global::Entitas.IMatcher<Entity>? _matcher;

    public static global::Entitas.IMatcher<Entity> MultipleFields
    {
        get
        {
            if (_matcher == null)
            {
                var matcher = (global::Entitas.Matcher<Entity>)global::Entitas.Matcher<Entity>.AllOf(Index.Value);
                matcher.componentNames = MyApp.MainContext.ComponentNames;
                _matcher = matcher;
            }

            return _matcher;
        }
    }
}
