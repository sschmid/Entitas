Entitas Upgrade Guide
=====================

Entitas provides automated migration tools to help upgrading to new versions.
You can apply automatic migrations in Unity by opening the Entitas Migration Window
`Tools > Entitas > Migrate...`

This document contains checklists for every release with breaking changes.

Entitas 0.47.2 upgrade guide
============================

#### Breaking changes

Apply Migration 0.47.2 to automatically rename the changed keys your properties files.

The following keys changed from:

- CodeGenerator.SearchPaths
- CodeGenerator.Plugins
- CodeGenerator.PreProcessors
- CodeGenerator.DataProviders
- CodeGenerator.CodeGenerators
- CodeGenerator.PostProcessors
- CodeGenerator.CLI.Ignore.UnusedKeys or Ignore.Keys

to:

- Jenny.SearchPaths
- Jenny.Plugins
- Jenny.PreProcessors
- Jenny.DataProviders
- Jenny.CodeGenerators
- Jenny.PostProcessors
- Jenny.Ignore.Keys

---

Entitas 0.46.0 upgrade guide
============================

#### Breaking changes

Removed methods marked obsolete in 0.42.0 from April 2017
- `context.CreateCollector<TEntity>(IMatcher<TEntity> matcher, GroupEvent groupEvent)`
- `new Context(int totalComponents, int startCreationIndex, ContextInfo contextInfo)`
- `context.DestroyEntity(TEnity entity)`

#### After you installed

First, edit the file `Generated/Feature.cs` and comment or delete the lines with compiler errors.

Then, run auto-import to use the new DesperateDevs.CodeGeneration.Plugins and generate.

Entitas.properties can be named differently now. By default it will be called
Preferences.properties. Additionally, you can delete User.properties or rename it
to Xyz.userproperties. If this file doesn't exist, it will automatically be generated for you.
You can have multiple properties and userproperties files now, e.g.
Preferences.properties and Roslyn.properties. In Unity it will automatically find and use
the first file. When using the Code Generator CLI (called Jenny now) you can explicitly
specify files like this

```
// will find and use the first file
$ jenny gen

// specify a file
$ jenny gen Roslyn.properties

// optionally specify an other userproperties
jenny gen Roslyn.properties My.userproperties
```

---

Entitas 0.45.0 upgrade guide
============================

#### Breaking changes

Use the command line tool `MigrationAssistant.exe` and apply Migration 0.45.0 to
automatically rename the changed keys in Entitas.properties

`MigrationAssistant.exe 0.45.0 path/to/project`

The following keys in Entitas.properties changed from:

- Entitas.CodeGeneration.CodeGenerator.SearchPaths
- Entitas.CodeGeneration.CodeGenerator.Plugins
- Entitas.CodeGeneration.CodeGenerator.DataProviders
- Entitas.CodeGeneration.CodeGenerator.CodeGenerators
- Entitas.CodeGeneration.CodeGenerator.PostProcessors
- Entitas.CodeGeneration.CodeGenerator.CLI.Ignore.UnusedKeys

to:

- CodeGenerator.SearchPaths
- CodeGenerator.Plugins
- CodeGenerator.DataProviders
- CodeGenerator.CodeGenerators
- CodeGenerator.PostProcessors
- CodeGenerator.CLI.Ignore.UnusedKeys

The default plugins are now in folder called `Entitas` instead of `Default`. Please update
the searchPaths in Entitas.properties.
`Entitas.exe` is now uppercase with capital E

---

Entitas 0.42.0 upgrade guide
============================

#### Breaking changes
- Removed Entitas.Blueprints.Unity.*
- Changed ReactiveSystem.GetTrigger method signature
- Marked obsolete: `context.DestroyEntity(entity)`. Use `entity.Destroy()` instead
- Marked obsolete: `context.CreateCollector(matcher, event)`, use new `context.CreateCollector(triggerOnEvent)` when you need `.Removed` or `.AddedOrRemoved` (e.g. `GameMatcher.View.Removed()`)

#### After you installed
- Removed Entitas.Blueprints.Unity.*
  - Remove all Entitas.Blueprints.Unity.* related code
  - Remove BinaryBlueprints from your project. Consider using extension methods as described here instead https://github.com/sschmid/Entitas-CSharp/issues/390
  - Remove from Entitas.properties:
    - Entitas.Blueprints.CodeGeneration.Plugins
    - Entitas.Blueprints.CodeGeneration.Plugins.BlueprintDataProvider
    - Entitas.Blueprints.CodeGeneration.Plugins.BlueprintsGenerator

- Changed ReactiveSystem.GetTrigger() method signature
  - find and replace `protected override Collector` -> `protected override ICollector`

- Generate

- Marked obsolete: `context.DestroyEntity(entity)`. Use `entity.Destroy()` instead
- Marked obsolete: `context.CreateCollector(matcher, event)`, use new `context.CreateCollector(triggerOnEvent)` when you need `.Removed` or `.AddedOrRemoved` (e.g. `GameMatcher.View.Removed()`)

---

Entitas 0.41.0 upgrade guide
============================

#### Breaking changes
In order to deploy Entitas as Dlls which enables 3rd party Addons and the extendable command line code generator the projects have been restructured. This restructuring has an impact on namespaces.

#### Before you install
- You're fine - nothing to do for you :heart:

#### After you installed
- Apply Migrations 0.41.0-1
- Apply Migrations 0.41.0-2
- Apply Migrations 0.41.0-3

These migrations should update most of the namespaces. Depending on which features of Entitas you have used there might be a chance that not all namespaces have been updated. In this case please fix the remaining namespaces manually.

Entitas.properties keys have been updated to support the latest code generator. Please open Entitas.properties in your project root and make sure the keys are updated. Here's an example from Match One

```
Entitas.CodeGeneration.Project = Assembly-CSharp.csproj
Entitas.CodeGeneration.SearchPaths = Assets/Libraries/Entitas, Assets/Libraries/Entitas/Editor, /Applications/Unity/Unity.app/Contents/Managed
Entitas.CodeGeneration.Assemblies = Library/ScriptAssemblies/Assembly-CSharp.dll
Entitas.CodeGeneration.Plugins = Entitas.CodeGeneration.Plugins, Entitas.CodeGeneration.Unity.Editor, Entitas.VisualDebugging.CodeGeneration.Plugins, Entitas.Blueprints.CodeGeneration.Plugins
Entitas.CodeGeneration.DataProviders = Entitas.Blueprints.CodeGeneration.Plugins.BlueprintDataProvider, Entitas.CodeGeneration.Plugins.ComponentDataProvider, Entitas.CodeGeneration.Plugins.ContextDataProvider, Entitas.CodeGeneration.Plugins.EntityIndexDataProvider
Entitas.CodeGeneration.CodeGenerators = Entitas.Blueprints.CodeGeneration.Plugins.BlueprintsGenerator, Entitas.CodeGeneration.Plugins.ComponentContextGenerator, Entitas.CodeGeneration.Plugins.ComponentEntityGenerator, Entitas.CodeGeneration.Plugins.ComponentGenerator, Entitas.CodeGeneration.Plugins.ComponentsLookupGenerator, Entitas.CodeGeneration.Plugins.ContextAttributeGenerator, Entitas.CodeGeneration.Plugins.ContextGenerator, Entitas.CodeGeneration.Plugins.ContextsGenerator, Entitas.CodeGeneration.Plugins.EntityGenerator, Entitas.CodeGeneration.Plugins.EntityIndexGenerator, Entitas.CodeGeneration.Plugins.MatcherGenerator, Entitas.VisualDebugging.CodeGeneration.Plugins.ContextObserverGenerator, Entitas.VisualDebugging.CodeGeneration.Plugins.FeatureClassGenerator
Entitas.CodeGeneration.PostProcessors = Entitas.CodeGeneration.Plugins.AddFileHeaderPostProcessor, Entitas.CodeGeneration.Plugins.CleanTargetDirectoryPostProcessor, Entitas.CodeGeneration.Plugins.MergeFilesPostProcessor, Entitas.CodeGeneration.Plugins.NewLinePostProcessor, Entitas.CodeGeneration.Plugins.WriteToDiskPostProcessor, Entitas.CodeGeneration.Plugins.ConsoleWriteLinePostProcessor, Entitas.CodeGeneration.Unity.Editor.DebugLogPostProcessor
Entitas.CodeGeneration.TargetDirectory = Assets/Sources/
Entitas.CodeGeneration.Contexts = Game, GameState, Input
Entitas.VisualDebugging.Unity.SystemWarningThreshold = 8
Entitas.VisualDebugging.Unity.DefaultInstanceCreatorFolderPath = Assets/Editor/DefaultInstanceCreator/
Entitas.VisualDebugging.Unity.TypeDrawerFolderPath = Assets/Editor/TypeDrawer/
```

Explanation:
- Entitas.CodeGeneration.Project: Relative path to your project.csproj (when using Unity use `Assembly-CSharp.csproj`)
- Entitas.CodeGeneration.SearchPaths: The new code generator can be extended with 3rd party plugins. Specify all folders where plugin dlls can be found. Plugins may depend on UnityEngine or UnityEditor, if so please specify where those dlls can be found (Unity default on Mac: `/Applications/Unity/Unity.app/Contents/Managed`
- Entitas.CodeGeneration.Assemblies: One or more Dlls that contain your components
- Entitas.CodeGeneration.Plugins: One or more Code Generator Plugin Dlls or namespaces

If all set up correctly DataProviders, CodeGenerators and PostProcessors can be set in Unity.

The command line code generator currently doesn't support the following plugins:
- Entitas.Blueprints.CodeGeneration.Plugins (contains Blueprint DataProvider and CodeGenerator)
- Entitas.CodeGeneration.Unity.Editor (contains DebugLogPostProcessor)

because they use Unity specific api. They will work as expected when generating from within Unity but don't work on the command line.

To test the config for potential problems, please unzip Entitas-CodeGenerator.zip in the root folder of your project.

---

### Note for Windows users
- Right-click Entitas-CodeGenerator.zip, open properties
- Check "Unblock"
- Hit Apply
- unzip

---

```
// skip mono on Windows
$ mono ./CodeGenerator/entitas.exe
Entitas Code Generator version 0.41.0
usage: entitas new [-f] - Creates new Entitas.properties config with default values
       entitas edit     - Opens Entitas.properties config
       entitas doctor   - Checks the config for potential problems
       entitas status   - Lists available and unavailable plugins
       entitas fix      - Adds missing or removes unused keys interactively
       entitas scan     - Scans and prints available types found in specified assemblies
       entitas dry      - Simulates generating files without writing to disk
       entitas gen      - Generates files based on Entitas.properties
       [-v]             - verbose output
       [-s]             - silent output (errors only)
```

To check the config for potential problems please run
```
$ mono ./CodeGenerator/entitas.exe doctor
```

The `doctor` command will show you the status and potential problems. Sometime you might get a warning like this:

```
- Could not resolve xyz.dll
```

This is just a warning. If no error is shown after running the `doctor` command, you can ignore those. All code generator plugins must be resolvable in order to be used. Use the `status` command to see available and unavailable plugins. This command helps you manage the plugins. Add or remove DataProviders, CodeGenerators or PostProcessors and check with `status` until you're happy. As usual, you can also use the Entitas Preferences Window in Unity to set up everything.

If there are nor problems use the `gen` command to generate or use the green generate button in Unity as usual.

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
