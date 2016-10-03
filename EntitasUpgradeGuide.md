# General information
Entitas provides an easy way to make upgrading to new versions a breeze.
Either use the command line tool `MigrationAssistant.exe` or the Migration menu
item in Unity. After that generate again.

Example
```
$ mono MigrationAssistant.exe
usage:
[-l]             - print all available versions
[version] [path] - apply migration of version [version] to source files located at [path]

$ mono MigrationAssistant.exe -l
========================================
0.18.0
  - Migrates IReactiveSystem GetXyz methods to getters
  - Use on folder, where all systems are located
========================================
0.19.0
  - Migrates IReactiveSystem.Execute to accept List<Entity>
  - Use on folder, where all systems are located
========================================
etc...


// Example from Math-One example project
$ mono MigrationAssistant.exe 0.26.0 /Path/To/Project/Generated/
```

Entitas 0.35.0 upgrade guide
============================

#### Info
`IMatcher.Where()` has been removed. See #194

#### Before you install
- You're fine - nothing to do for you :heart:

#### After you installed
- Fix all the errors where you used `matcher.Where()`


Entitas 0.34.0 upgrade guide
============================

#### Info
`GroupObserver` has been renamed to `EntityCollector`. See #168

#### Before you install
- Rename `GroupObserver` to `EntityCollector`
- Rename `.CreateGroupObserver()` to `.CreateEntityCollector()`
- Rename `IGroupObserverSystem` to `IEntityCollectorSystem`
- Find & Replace `public EntityCollector groupObserver` with `public EntityCollector entityCollector`

#### After you installed
- You're fine - nothing to do for you :heart:


Entitas 0.33.0 upgrade guide
============================

#### Info
`IDeinitializeSystem` has been renamed to `ITearDownSystem`. See #164

#### Before you install
- Manually rename `IDeinitializeSystem` to `ITearDownSystem`

#### After you installed
- You're fine - nothing to do for you :heart:


Entitas 0.32.0 upgrade guide
============================

Use the command line tool `MigrationAssistant.exe` to automatically fix compile errors.
Entitas 0.32.0 introduces a new Pools class. Using the new PoolsGenerator will require
to update your existing project manually. You can still use the old Pools class in your
existing project if you want. If so, please use the OldPoolsGenerator instead of the new one.


Entitas 0.30.0 upgrade guide
============================

Some code generators got renamed. Apply Migration 0.30.0


Entitas 0.29.0 upgrade guide
============================

Marked old PoolMetaData constructor obsolete. If you encounter compile errors
please apply Migration 0.26.0, open C# project and generate again.


Entitas 0.28.0 upgrade guide
============================

If you're using Entitas with Unity, please open the Entitas preferences and make
sure that all your desired code generators are activated.
Due to some code generator renamings the ComponentIndicesGenerators inactive.

The SystemsGenerator has been removed. Please use `pool.CreateSystem<MySystem>()` instead.


Entitas 0.27.0 upgrade guide
============================

If you're using Entitas with Unity, please open the Entitas preferences and make
sure that all your desired code generators are activated.
Due to some code generator renamings the ComponentLookupGenerator and
the ComponentsGenerator are inactive. Activate them (if desired) and generate.


Entitas 0.26.0 upgrade guide
============================

Use the command line tool `MigrationAssistant.exe` to automatically fix compile errors.
After that generate again.

```
$ mono MigrationAssistant.exe
usage:
[-l]             - print all available versions
[version] [path] - apply migration of version [version] to source files located at [path]

$ mono MigrationAssistant.exe -l
0.18.0 - Migrates IReactiveSystem GetXyz methods to getters
0.19.0 - Migrates IReactiveSystem.Execute to accept List<Entity>
0.22.0 - Migrates IReactiveSystem to combine trigger and eventTypes to TriggerOnEvent
0.26.0 - Deactivates code to prevent compile erros

// Example from Math-One example project
$ mono MigrationAssistant.exe 0.26.0 /Path/To/Project/Generated/
```


Entitas 0.24.0 upgrade guide
============================

To fix the compile errors after updating to Entitas 0.24.0, delete in `Pools.cs`

```csharp
#if (UNITY_EDITOR)
var poolObserver = new Entitas.Unity.VisualDebugging.PoolObserver(_pool, ComponentIds.componentNames, ComponentIds.componentTypes, "Pool");
UnityEngine.Object.DontDestroyOnLoad(poolObserver.entitiesContainer);
#endif
```

and generate again.


Entitas 0.23.0 upgrade guide
============================

Entitas 0.23.0 changed and applied naming conventions.
Before updating to this version, follow these steps to prepare your project:

#### Rename

    Pool.Count       -> Pool.count
    Group.Count      -> Group.count
    Properties.count -> Properties.count

#### Find/Replace in generated folder

    ": AllOfMatcher "              -> ""
    ": base(new [] { index }) "    -> ""
    "static AllOfMatcher _matcher" -> "static IMatcher _matcher"
    "public static AllOfMatcher"   -> "public static IMatcher"
    "new Matcher"                  -> "Matcher.AllOf"

#### Delete

In generated ...ComponentIds

    namespace Entitas {
        public partial class XYZMatcher {
            public Matcher(int index) {
            }

            public override string ToString() {
                return ComponentIds.IdToString(indices[0]);
            }
        }
    }

Entitas 0.22.0 upgrade guide
============================

Entitas 0.22.0 changed IReactiveSystem and IMultiReactiveSystem and renamed IStartSystem.Start to IInitializeSystem.Initialize.

Use the command line tool `MigrationAssistant.exe` to automatically migrate IReactiveSystem.

```
$ mono MigrationAssistant.exe
usage:
[-l]             - print all available versions
[version] [path] - apply migration of version [version] to source files located at [path]

$ mono MigrationAssistant.exe -l
0.18.0 - Migrates IReactiveSystem GetXyz methods to getters
0.19.0 - Migrates IReactiveSystem.Execute to accept List<Entity>
0.22.0 - Migrates IReactiveSystem to combine trigger and eventTypes to TriggerOnEvent

// Example from Math-One example project, where all the systems are located in the Features folder
$ mono MigrationAssistant.exe 0.22.0 /Path/To/Project/Assets/Sources/Features
```


Entitas 0.19.0 upgrade guide
============================

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


Entitas 0.18.0 upgrade guide
============================

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


Entitas 0.12.0 upgrade guide
============================

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


Entitas 0.10.0 upgrade guide
============================

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
