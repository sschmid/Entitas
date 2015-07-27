# Entitas 0.19.0 upgrade guide
Entitas 0.19.0 introduces a few breaking changes:

Added new e.OnComponentReplaced and removed all *WillBeRemoved events.

If you used `group.OnEntityWillBeRemoved`, you could replace it either with
```cs
_group.OnEntityRemoved += (group, entity, index, component) => { //... };
```
or with
```cs
_group.OnEntityUpdated += (group, entity, index, previousComponent, newComponent) => { // ...};
```
If your generated component extensions are not compiling, find/replace `WillRemoveComponent` with `//WillRemoveComponent`
to temporarily ignore the errors.

IReactiveSystem.Execute takes List<Entity> instead of Entity[]. Use the command line tool `MigrationAssistant.exe` to automatically migrate.

```
$ mono MigrationAssistant.exe
usage:
[-l]             - print all available versions
[version] [path] - apply migration of version [version] to source files located at [path]

$ mono MigrationAssistant.exe -l
0.18.0 - Migrates IReactiveSystem API
0.19.0 - Migrates IReactiveSystem.Execute

// Example from Math-One example project, where all the systems are located in the Features folder
$ mono MigrationAssistant.exe 0.19.0 /Path/To/Project/Assets/Sources/Features
```


# Entitas 0.18.0 upgrade guide
Entitas 0.18.0 changes IReactiveSystem. To upgrade your source files, follow these steps
- Install Entitas 0.18.0 (which will result in compiler errors)
- Use the command line tool `MigrationAssistant.exe` to automatically migrate

```
$ mono MigrationAssistant.exe
usage:
[-l]             - print all available versions
[version] [path] - apply migration of version [version] to source files located at [path]

$ mono MigrationAssistant.exe -l
0.18.0 - Migrates IReactiveSystem API

// Example from Math-One example project, where all the systems are located in the Features folder
$ mono MigrationAssistant.exe 0.18.0 /Path/To/Project/Assets/Sources/Features
```


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
    the code generator will save all generated files. If you are using the PoolAttribute in your components,
    add all custom pool names used in your application. Make sure that all existing custom PoolAttributes call
    the base constructor with the same name as the class (without 'Attribute').
    If you are not using the PoolAttribute in your components, you can skip this process.

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

    Click the MenuItem "Entitas/Update API". All occurrences of the old Matcher will be updated
    to the new version, which is prefixed based on the PoolAttribute.

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
