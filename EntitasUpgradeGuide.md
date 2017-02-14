Entitas Upgrade Guide
=====================

Entitas provides an easy way to make upgrading to new versions a breeze.
Either use the command line tool `MigrationAssistant.exe` or the Migration menu
item in Unity. After that generate again.

In some cases manual steps have to be applied BEFORE installing a new version.
This document contains checklists for every release with breaking changes.


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


$ mono MigrationAssistant.exe 0.26.0 /Path/To/Project/RequestedFolder
```

---

Entitas 0.37.0 upgrade guide
============================

#### Breaking changes
Entitas went type-safe! This was a huge task and I'm happy to finally share this with you guys!
This feature makes Entitas safer and more managable in growing code bases and will eliminate certain kind of bugs.
This change breaks existing projects! It is possible to manually migrate existing projects but there is no special workflow
other than manually use find / replace to fix all compile errors. I use Entitas 0.37.0 in my current project (500+ systems)
and was able to migrate within less than two days. If you have less systems and components you should be able to migrate within one day.

Reminder: If you're updating from versions < 0.36.0 you should update to 0.36.0 first. Be aware that existing Blueprints(Beta) are breaking
because of the renaming from `Pool` to `Context`. Existing Binary Blueprints have to be manually updated.

If you're not sure if you should update you can wait another week. I plan to make a video to show how to upgrade existing projects.
After this you should be able to decide if you want to update or not.

#### Before you install
- Rename `SingleEntityAttribute` to `UniqueAttribute`
- Change namespace of all attributes in CodeGenerator/Attributes to `Entitas.CodeGenerator.Api`
- Find / replace `using Entitas.CodeGenerator` to `using Entitas.CodeGenerator.Api` in all generated context attributes
- Find / replace `using Entitas.CodeGenerator;` to `using Entitas.CodeGenerator.Api;` in all generated components

#### After you installed

After installing Entitas 0.37.0 you most likely end up having lots of compiler errors. The 2 biggest issues are:
- Generated components
- Systems

There migh also be other issues depending how you used Entitas before, but fixing the generated components and the systems
might already do most of the work.

##### Problem 1 (Components):
The old generated components extend Entitas.Entity by using `partial class`.
The new version inherits Entitas.Entity to have a new entity type and to get rid of `partial class` to enable
having Entitas as a precompiled dll.

##### Solution 1 (Components)
The goal is to update the generated components. I see 3 possible workflows to fix them:
1. Delete all components and generated components and use the EntitasLang DSL https://github.com/mzaks/ECS-Lang
2. Temporarily move all the logic (systems) out of your Unity project and delete the generated components.
After this there shouldn't be any compile errors anymore (if so, temporarily move them out if your Unity project).
Now you should be able to re-generate. After that, move all the files back to your Unity project.
3. Manually use find / replace in the generated components folder to migrate the components

##### Problem 2 (Systems)
All reactive systems need to be updated to be type-safe.

##### Solution 2 (Systems)
Manually use find / replace to migrate e.g. method signatures and other issues
Take a look at [Match-One AnimatePositionSystem.cs](https://github.com/sschmid/Match-One/blob/develop/Assets/Sources/Logic/View/Systems/AnimatePositionSystem.cs)
to see how the new reactive systems look like.

##### Other issues
There might be other issues related to the type-safety. Rule of thumb:
- Every occurrences of `Entity` must be typed now, e.g. `GameEntity`
- Every occurrences of `Group` must be typed now, e.g. `IGroup<GameEntity>`
- Every occurrences of `Context` must be typed now, e.g. `IContext<GameEntity>` or `GameContext` if possible
- Every occurrences of `Collector` must be typed now, e.g. `Collector<GameEntity>`
- Every occurrences of `Matcher` must be typed now, e.g. `Matcher<GameEntity>.AllOf(...)`

I recommend using find / replace on ceratin folders to fix those issues efficiently.

---

Entitas 0.36.0 upgrade guide
============================

#### Breaking changes
The term `Pool` has been replaced with `Context`. This affects all classes that
contain the word pool.
`EntityCollector` has been renamed to `Collector`
`GroupEventType` has been renamed to `GroupEvent`


#### Before you install
- Rename `Pools.CreatePool()` to `Pools.CreateContext`
- Rename `Pool` to `Context`
- Rename `Pools` to `Contexts`
- Rename `Pools.SetAllPools()` to `Pools.SetAllContexts()`
- Rename `PoolAttribute` to `ContextAttribute`
- Rename `EntityCollector` to `Collector`
- Rename `GroupEventType` to `GroupEvent`
- Rename `GroupEventType.OnEntityAdded` to `GroupEvent.Added`
- Rename `GroupEventType.OnEntityRemoved` to `GroupEvent.Removed`
- Rename `GroupEventType.OnEntityAddedOrRemoved` to `GroupEvent.AddedOrRemoved`

#### After you installed
- Use the command line tool `MigrationAssistant.exe` and apply Migration 0.36.0-2
- Manually migrate all systems and fix compiler errors
- apply Migration 0.36.0-1
- Ensure all code generator are selected and generate

---

Entitas 0.35.0 upgrade guide
============================

#### Breaking changes
`IMatcher.Where()` has been removed. See #194

#### Before you install
- You're fine - nothing to do for you :heart:

#### After you installed
- Fix all the errors where you used `matcher.Where()`

---

Entitas 0.34.0 upgrade guide
============================

#### Breaking changes
`GroupObserver` has been renamed to `EntityCollector`. See #168

#### Before you install
- Rename `GroupObserver` to `EntityCollector`
- Rename `.CreateGroupObserver()` to `.CreateEntityCollector()`
- Rename `IGroupObserverSystem` to `IEntityCollectorSystem`
- Find & Replace `public EntityCollector groupObserver` with `public EntityCollector entityCollector`

#### After you installed
- You're fine - nothing to do for you :heart:

---

Entitas 0.33.0 upgrade guide
============================

#### Breaking changes
`IDeinitializeSystem` has been renamed to `ITearDownSystem`. See #164

#### Before you install
- Manually rename `IDeinitializeSystem` to `ITearDownSystem`

#### After you installed
- You're fine - nothing to do for you :heart:

---

Entitas 0.32.0 upgrade guide
============================

Use the command line tool `MigrationAssistant.exe` to automatically fix compile errors.
Entitas 0.32.0 introduces a new Pools class. Using the new PoolsGenerator will require
to update your existing project manually. You can still use the old Pools class in your
existing project if you want. If so, please use the OldPoolsGenerator instead of the new one.

---

Entitas 0.30.0 upgrade guide
============================

Some code generators got renamed. Apply Migration 0.30.0

---

Entitas 0.29.0 upgrade guide
============================

Marked old PoolMetaData constructor obsolete. If you encounter compile errors
please apply Migration 0.26.0, open C# project and generate again.

---

Entitas 0.28.0 upgrade guide
============================

If you're using Entitas with Unity, please open the Entitas preferences and make
sure that all your desired code generators are activated.
Due to some code generator renamings the ComponentIndicesGenerators inactive.

The SystemsGenerator has been removed. Please use `pool.CreateSystem<MySystem>()` instead.

---

Entitas 0.27.0 upgrade guide
============================

If you're using Entitas with Unity, please open the Entitas preferences and make
sure that all your desired code generators are activated.
Due to some code generator renamings the ComponentLookupGenerator and
the ComponentsGenerator are inactive. Activate them (if desired) and generate.

---

Entitas 0.26.0 upgrade guide
============================

Use the command line tool `MigrationAssistant.exe` to automatically fix compile errors.
After that generate again.

---

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

---

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

---

Entitas 0.22.0 upgrade guide
============================

Entitas 0.22.0 changed IReactiveSystem and IMultiReactiveSystem and renamed IStartSystem.Start to IInitializeSystem.Initialize.

Use the command line tool `MigrationAssistant.exe` to automatically migrate IReactiveSystem.

---

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

---

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

---

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

---

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
