# CAUTION
Xamarin Studio 5.7.x currently has a bug when find-replace (Menu -> Search -> Replace in files...)
https://bugzilla.xamarin.com/show_bug.cgi?id=25360

Please be aware of that issue and use any other text editor for that task.


# Entitas 0.12.0 upgrade guide

Entitas 0.12.0 generates prefixed matchers based on the PoolAttribute and introduces some
API changes. In your existing project with a Entitas version < 0.12.0 manually rename the
following classes and methods.

## Before installing Entitas 0.12.0

#### Rename

    pool.CreateSystem()                                 -> pool.CreateExecuteSystem()

Now that you're prepared for integrating the latest version, delete your existing version
of Entitas, EntitasCodeGenerator and EntitasUnity.

#### Delete

    Entitas
    EntitasCodeGenerator
    EntitasUnity

## Install Entitas 0.12.0

#### Setup Entitas Preferences

    Open the Unity preference panel and select Entitas. Check and update the path to the folder where
    the code generator will save all generated files. If you are using the `PoolAttribue` in your components,
    add all custom pool names used in your application. Make sure that all existing custom PoolAttribues call
    the base constructor with the same name as the class (without 'Attribute').
    If you are not using the `PoolAttribue` in your components, you can skip this process.

```cs
using Entitas.CodeGenerator;

public class CoreGameAttribute : PoolAttribute {
    public CoreGameAttribute() : base("CoreGame") {
    }
}
```

#### Code Generator

    Use the code generator and generate

#### Update API

    Click the MenuItem "Entias/Update API". All occurrences of the old `Matcher` will be updated
    to the new version, which is prefixed based on the `PoolAttribute`.

#### Delete

    Delete all custom PoolAttributes


# Entitas 0.10.0 upgrade guide

Beside features, Entitas 0.10.0 includes lots of renaming. If your current Entitas
version is < 0.10.0, you might want to follow the next few simple renaming steps,
to speed up the integration of the latest version of Entitas.
In your existing project with a Entitas version < 0.10.0 manually rename the following
classes and methods.

## Before installing Entitas 0.10.0

#### Rename

    EntityRepository                                    -> Pool
    EntityRepository.GetCollection()                    -> Pool.GetGroup()

    EntityCollection                                    -> Group
    EntityCollection.EntityCollectionChange             -> Group.GroupChanged

    EntityRepositoryObserver                            -> GroupObserver
    EntityRepositoryObserver.EntityCollectionEventType  -> GroupObserver.GroupEventType

    IEntityMatcher                                      -> IMatcher
    IEntitySystem                                       -> IExecuteSystem
    AllOfEntityMatcher                                  -> AllOfMatcher
    EntityRepositoryAttribute                           -> PoolAttribute
    IReactiveSubEntitySystem                            -> IReactiveSystem
    ReactiveEntitySystem                                -> ReactiveSystem

#### Delete

    EntityWillBeRemovedEntityRepositoryObserver         -> DELETE
    IReactiveSubEntityWillBeRemovedSystem               -> DELETE
    ReactiveEntityWillBeRemovedSystem                   -> DELETE

Now that you're prepared for integrating the latest version, delete your existing version
of Entitas, EntitasCodeGenerator and ToolKit.

#### Delete

    Entitas
    EntitasCodeGenerator
    ToolKit (unless you use classes from ToolKit. The new version of Entitas doesn't depend on ToolKit anymore)


## Install Entitas 0.10.0

#### Fix remaining issues

    IReactiveSubEntityWillBeRemovedSystem
        - Consider implementing ISystem & ISetPool and use group.OnEntityWillBeRemoved += foobar;

#### Code Generator

    Use the code generator and generate

