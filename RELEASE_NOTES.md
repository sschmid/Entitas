# 0.37.0

See and discuss changes in [Milestone 0.37.0](https://github.com/sschmid/Entitas-CSharp/milestone/7)

#### Breaking changes
Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)

The deed is done. Entitas went type-safe! This was a huge task and I'm happy to finally share this with you guys!
This feature makes Entitas safer and more managable in growing code bases and will eliminate certain kind of bugs. Thanks to @mstrchrstphr
for starting the conversation and proposing solutions.

#### Entitas
- Entitas went type-safe! (#257 #266)
- Entity API doesn't return Entity anymore (e.g. e.AddComponent())
- Fixed matchers not recalculating hash when changed
- Added EntityIndex support for multiple keys (#279 #281)
- Removed as many virtual keywords as possible

#### Entitas.CodeGenerator
- Entitas went type-safe! (#257 #266)
- Rewrote code generator architecture (#265 #274 #275)
- ComponentsGenerator doesn't generate `e.IsMoveble(value)`. Only `e.isMoveble = value`
- ComponentsGenerator Entity API doesn't return Entity anymore (e.g. e.AddPosition())
- Added additional ComponentGenerator which respects namespaces (#274)

#### Entitas.Blueprints
- Entitas went type-safe! (#257 #266)

#### Entitas.Migration
- Automatically embedding all migrations to Entitas.Migration.exe

#### Entitas.Unity.Codegenerator
- Added sloc (Source Lines Of Code) and loc (Lines Of Code) info

#### Entitas.Unity.VisualDebugging
- Entitas went type-safe! (#257 #266)
- Added EntityLink (#271)
- Prettier search fields that support multiple search strings

#### Other
- New folder structure with Entitas as the core and everything else as Addons
- Complete reorganization of the project structure (more modular and easier to reason about)


# 0.36.0

See and discuss changes in [Milestone 0.36.0](https://github.com/sschmid/Entitas-CSharp/milestone/6)

#### Breaking changes
Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)

#### Entitas
- Removed pool.CreateSystem() (#233 #237)
- Removed `IEnsureComponents`, `IExcludeComponents`, `ISetPools`, `ISetPool`, `IReactiveSystem`, `IMultiReactiveSystem`, `IEntityCollectorSystem`
- Changed the ReactiveSystem to be an abstract class instead of `IReactiveSystem`. You need to override `GetTrigger`, `Filter` and `Execute`.
This enables filtering entities based on component values (#234)
- Renamed the term Pool to Context (#99 #250)
- Renamed `EntityCollector` to `Collector` (#252 #253)
- Renamed `GroupEventType` to `GroupEvent` and removed the prefix `OnEntity`
- entity.ToString uses component.ToString(). Override ToString() in your components
to get a nice description, e.g. `Health(42)` (#203 #196)

#### Entitas.CodeGenerator
- Removed OldPoolsGenerator
- Fixed code generator line ending for header

#### Entitas.Unity.VisualDebugging
- Improved VisualDebugging performance by reusing StringBuilders
- Added support for `ICleanupSystem` and `ITearDownSystem`
- Changed SystemsMonitor.axisRounding to 1
- Fix error when turning visual debugging on/off in Unity 5.4 or newer (#222)
- Changed default blueprint creation location (#206 #248)

### Other
- Simplified build pipeline


# 0.35.0

See and discuss changes in [Milestone 0.35.0](https://github.com/sschmid/Entitas-CSharp/milestone/5)

#### Breaking changes
Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)

#### Entitas
- Fixed adding disabled entities to groups (#192, #193)
- Removed matcher with filter (#194, #195)

### Other
- Maintenance, cleanup and formatting
- Completely new build system to create new releases


# 0.34.0

See and discuss changes in [Milestone 0.34.0](https://github.com/sschmid/Entitas-CSharp/milestone/4)

#### Breaking changes
Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)

#### Entitas
- Added api to clone entities (#178, #182)
  - `pool.CloneEntity(e);`
  - `entity.CopyTo(target);`

- Added EntityIndex constructor with EqualityComparer (#170, #186)
- Rename GroupObserver to EntityCollector (#168, #188)
- Added filter condition to matchers (#165, #189)
  - `Matcher.Position.Where(e => e.position.x > 10);`

#### Entitas.Serialization.Blueprints
- Added HideInBlueprintInspectorAttribute (#185)

#### Other
- Improved snippets
- Added Visual Studio snippets (#172)
- Added TestRunner to support test debugging (#175, #176)
- Updated build scripts (#173, #177)
- Added tests for code formatting


# 0.33.0

##### Breaking changes
Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)

##### Entitas
- Added pools.CreateSystem()
- Added ObjectPool and ObjectCache and updated EntitasCache to use ObjectCache (#157)
- Added entityIndex.Activate() and removing entity indices from pool (#163)
- Renamed IDeinitializeSystem to ITearDownSystem (#164)

##### Entitas.CodeGenerator
- TypeReflectionProvider sorts pool names and ToUppercaseFirst() (#155)
- CodeGeneratorConfig doesn't add default pool anymore (#156)

##### Other
- Added repository icon
- Added snippets (see Snippets folder)


# 0.32.0

Summer break is over! Entitas development is back on track!
Thanks all of you guys for using and contributing to Entitas.
This release is packed with improvements from all of you, thanks for that!

##### Breaking changes
Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)

##### General
- Lots of maintenance, refactoring, documentation and cleanup. Checked every class and every test ;)
- Removed unused usings (#134 @thematthopkins )
- Added script to generate docset and included it in build script (#141 @mstrchrstphr)
- Updated policy.mdpolicy to support latest Xamarin Studio
- Fixed inconsistent Line endings (#116 @ParagonFable)

##### Entitas
- Added new `Pools` class. There is no static Pools anymore but an instance.
- Added `ISetPools` to inject the shared pools instance
- Removed `pool.CreateSystem<T>()` and `pool.CreateSystem(Type type)` (Apply migration 0.32.0)
- Fixed `pool.CreateSystem()` not creating a ReactiveSystem for IGroupObserverSystem
- Added `EntityIndex` (#154)
- `pool.Reset()` removes all event handlers
- Fixed retain / release didn't update entity toString cache
- Added `EntitasCache` for object pooling of collections to reduce memory allocations
- Updated Entity, Matcher and Pool to use EntitasCache (less garbage :heart:)
- Added `ICleanupSystem`
- Added `IDeinitializeSystem`
- Pushing removed component to component pool after dispatching event

##### Entitas.CodeGenerator
- Fixed ComponentIndicesGenerator with multiple pools (#124)
- CodeGeneratorConfig will add default pool
- Fixed pools order if default pool exists

##### Entitas.Unity.CodeGenerator
- CodeGenerator Preferences is using MaskField instead of Toggles now

##### Entitas.Unity.VisualDebugging
- Less editor repaints for DebugSystemsInspector to improve performance
- Fixed system stats (Log stats) not ignoring Feature class
- Add ITypeDrawer for doubles (#132 @bddckr)
- Added support for enum masks (#132 @bddckr)
- Adjusted foldout spacing in custom inspector (#149 @ByteSheep)

##### Other
- Updated keys for Entitas.properties and moved files from Entitas.Unity to Entitas.Serialization.Configuration
- Moved Properties from Entitas.Unity to Entitas.Serialization


# 0.31.2

##### Entitas.CodeGenerator
- All attributes can now be used for classes, interfaces and structs


# 0.31.1

##### Entitas.CodeGenerator
- Improved component generation for classes and interfaces and added support for default pool [Pool]
- Added support to CustomComponentNameAttribute to generate multiple components with different names for one class or interface

```csharp
// This will automatically generate PositionComponent and VelocityComponent for you
[Pool, CustomComponentName("Position", "Velocity")]
public struct IntVector2 {
  public int x;
  public int y;
}
```

- Added support for generating components for structs
- Not generating obsolete pool attributes for generated classes


# 0.31.0

##### General
- Removed obsolete code

##### Entitas.CodeGenerator
- Generating components for attributed classes and interfaces

```csharp
// will automatically generate SomeClassComponent for you
[Core]
public class SomeClass {
    public string name;

    public SomeClass(string name) {
        this.name = name;
    }
}
```

- Added support to add empty PoolAttribute to assign component to default pool

```csharp
// using [Pool] will also add this component to Pools.pool
[Core, Pool]
public class SomeComponent : IComponent {
}
```

##### Entitas.Unity.VisualDebugging
- Added IComponentDrawer which can draw the whole component
- Added EntitasEntityErrorHierarchyIcon to indicate retained entities in the hierarchy
- Added CharTypeDrawer
- Fixed components not updating in the inspector (#107)
- Improved SystemsMonitor and added average line

![Entitas-SystemsMonitor](https://cloud.githubusercontent.com/assets/233700/15198441/a515d764-17d7-11e6-965c-83c027fa89f7.png)

##### Entitas.Unity.Serialization.Blueprints
- Fixed finding all BinaryBlueprints even when not loaded
- Correctly saving Blueprints when setting all BinaryBlueprints
- Added BlueprintsNotFoundException
- BinaryBlueprintInspector creates new pools instead of using one of Pools.allPools
- Fixed pool not shown when entering play-mode while a blueprint was selected in the project view
- Not caching blueprints when UNITY_EDITOR to enable live edit

# 0.30.3

##### Entitas.CodeGenerator
- Added support for whitespace, '-' and braces in blueprint names

##### Entitas.Unity.Serialization.Blueprints
- Blueprints.FindAllBlueprints orders all blueprints by name
- Fixed pool not shown in hierarchy


# 0.30.2

##### Note
This release introduces Blueprints for Entitas (Beta). Update if you want to
use and play with Blueprints. [Read more...](https://github.com/sschmid/Entitas-CSharp/wiki/Blueprints-(Beta))

##### Entitas.CodeGenerator
- Only creating PoolObserver when Application.isPlaying
- Added BlueprintsGenerator

##### Entitas.Unity.VisualDebugging
- Added more options for sorting systems in the inspector
- Removing event handlers from pool observer when leaving play-mode

##### Entitas.Serialization.Blueprints
- Added Blueprints (and more)

##### Entitas.Unity.Serialization.Blueprints
- Added BlueprintInspector (and more)

##### Other
- Moved build scripts into a folder


# 0.30.1

##### Entitas.Unity.VisualDebugging
- Fixed GameObjectDestroyExtension.DestroyGameObject() compile time error (#91)
- Improved SystemsMonitor.Draw() to use correct available width even with scrollbars
- Tweaked drawing systems list
- Added EntitasPoolErrorHierarchyIcon to visualize when there are erros

##### Other
- Updated build_commands.sh to generate C# project from Unity


# 0.30.0

##### Breaking changes
Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)

##### Entitas
- Added IGroupObserverSystem which allows ReactiveSystems to observe multiple pools
- Added pools.CreateGroupObserver() to simplify creating a GroupObserver for multiple pools

##### Entitas.CodeGenerator
- TypeReflectionProvider ignores abstract IComponents (#88)
- Renamed ComponentsGenerator to ComponentExtensionsGenerator
- Renamed PoolAttributeGenerator to PoolAttributesGenerator

##### Entitas.Unity
- Moved Assets/Entitas.Unity to Assets/Entitas/Unity
- Simplified folder structure in Entitas-Unity.zip

##### Entitas.Unity.CodeGenerator
- Ignoring obsolete code generators
- Generate button changes size depending on generators list height

##### Entitas.Unity.VisualDebugging
- Added Feature class which inherits from Systems or DebugSystems for you, so you don't have to care anymore
- Fixed MissingReferenceException occurring occasionally when stopping game (#71)
- Added support for editing entities in EditorMode (non-playing mode)
- Fixed bug when components are added on entity creation (#87)
- Added clear buttons to search textfields
- Improved DateTimeTypeDrawer
- Added new hierarchy icons for pool and systems

##### Entitas.Migration
- Added M0300
- Moving Entitas.Migration into Entitas/Migration/Editor when creating Entitas-Unity.zip


# 0.29.1

##### Entitas.CodeGenerator
- Added missing support for components with properties
- Updated ComponentsGenerator to use entity.CreateComponent()

##### Entitas.Unity.CodeGenerator
- Added missing support for components with properties


# 0.29.0

##### Obsolete
Marked old PoolMetaData constructor obsolete. If you encounter compile errors please apply Migration 0.26.0, open C# project and generate again.

##### General
- Unified Entitas sub projects into a single project
- Unified all Unity projects into a single project
- Documentation maintenance

##### Entitas
- Removing all event handler for entity.OnEntityReleased after event got dispatched
- Printing entity in EntityIsNotDestroyedException
- Added TypeExtension.ImplementsInterface()
- Added component types to PoolMetaData
- Made all methods in Systems virtual
- Added auto generated header to generated files

```
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGenerator.ComponentsGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
```

##### Entitas.CodeGenerator
- Using pool specific componentIds lookup when generating matchers for components with multiple pools
- TypeReflectionProvider ignores interfaces

##### Entitas.Serialization
- Added Entitas.Serialization
- Added PublicMemberInfo

##### Entitas.Unity.CodeGenerator
- Compile errors won't block code generation anymore
- Printing total generated file count when generating

##### Entitas.Unity.VisualDebugging
- Destroying EntityBahviour when entity got released
- Using entity component pool and providing correct previous and new component
- Added unique color for each component in EntityInspector
- Added component search field in EntityInspector

<img alt="Entitas-Component-Search" src="https://cloud.githubusercontent.com/assets/233700/13554841/98141fac-e3b2-11e5-81ef-4ef39cf2bb4c.gif" width="417" />

- 'Destroy Entity' Buttons are now red
- Simplified EntityInspector and made methods static
- Unfolded components info is now shared between entities within same pool
- Added shortcuts to Entitas Preferences and Generate
- Improved TypeDrawers
- Stepper UI tweaks

![Entitas.Unity.VisualDebugging-Systems](https://cloud.githubusercontent.com/assets/233700/13554882/9c0bd7c0-e3b3-11e5-89ec-65fa888f0a48.png)

- Renamed 'Script Call Optimization' to 'Optimizations'
- Added EntitasEditorLayout


# 0.28.2

##### Entitas
- Added ReactiveSystem destructor to prevent memory leaks
- Added GroupObserver destructor to prevent memory leaks

##### Entitas.Unity.VisualDebugging
- EntityInspector now supports dropping UnityEngine.Object into fields that are null

![Entitas.Unity.VisualDebugging-DefaultInstanceCreator](https://cloud.githubusercontent.com/assets/233700/12884636/ea8c468c-ce5f-11e5-91a9-0fdf83de7252.png)

- UI tweaks


# 0.28.1

##### Entitas.Unity
- Added "Script Call Optimization" to Entitas Preferences Window
- Added priority to IEntitasPreferencesDrawer
- Tweaked UI

![Entitas.Unity-ScriptCallOptimization](https://cloud.githubusercontent.com/assets/233700/12832387/e893b3ec-cb99-11e5-8ccb-d3478ca0c6dc.png)

##### Entitas.Unity.VisualDebugging
- Added toggle to Entitas Preferences to enable or disable Visual Debugging
- Tweaked UI

![Entitas.Unity.VisualDebugging-Toggle](https://cloud.githubusercontent.com/assets/233700/12832391/ec74d2e8-cb99-11e5-87b3-f76e2e9ea58d.png)


# 0.28.0

##### Breaking changes
Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)

##### Entitas
- Added documentation (#55)
- Added an object pool for components (#58)
- Added pool.ClearComponentPool(index) and pool.ClearComponentPools()
- Added ENTITAS_FAST_AND_UNSAFE compiler flag. When set it will speed up e.Retain() and e.Release() (#59)

##### Entitas.CodeGenerator
- Generated component extensions are now reusing components using a component object pool when destroying entities (#58)
- Added tests for testing the logic of generated files
- Decoupling code generation logic by adding Code Generator Intermediate Format (#62)
- Added TypeReflectionProvider
- Supporting components with namespace
- Simplified linq expressions
- Removed generated systems
- The Code Generator is not depending on Entitas anymore

##### Entitas.CodeGenerator.TypeReflection
- Added Entitas.CodeGenerator.TypeReflection project

##### Entitas.Unity
- Added `keys` and `values` getter to Properties

##### Entitas.Unity.VisualDebugging
- Added system search field to DebugSystemsInspector
- UI tweaks and performance optimizations
- Fixed logging wrong system stats
- Added header image and current version label to Entitas Preferences Window

![Entitas.Unity.Visualdebugging-preferences](https://cloud.githubusercontent.com/assets/233700/12795069/a13e5b6e-cab8-11e5-937d-870790e2bfe1.png)

##### Entitas.Unity.Migration
- Added Entitas.Unity.Migration which provides an easy way to migrate source files
- Added header image and current version label to Entitas Migration Window

![Entitas.Unity.Migration](https://cloud.githubusercontent.com/assets/233700/12795026/6acf24b4-cab8-11e5-90e3-98a103676d50.png)

##### Other
- Removed redundant files and gitignored Entitas in all Unity projects (#63)
- Removed Unity projects from Entitas.sln
- Removed warnings


# 0.27.0

##### Note
If you're using Entitas with Unity, please open the Entitas preferences and make sure that all your desired code generators are activated. Due to some code generator renamings the ComponentLookupGenerator and the ComponentsGenerator are inactive. Activate them (if desired) and generate.

##### Entitas
- Added `pool.Reset()` which clears all groups, destroys all entities and resets creationIndex

##### Entitas.CodeGenerator
- Renamed some code generators
- Added `CustomPrefixAttribute` to support custom prefixes for flag components
```
[CustomPrefix("flag")]
public class DestroyComponent : IComponent {
}

// default
entity.isDestroy = true;

// with CustomPrefixAttribute
entity.flagDestroy = true;
```

##### Entitas.Unity
- Added "Feedback" menu item to report bugs, request features, join the chat, read the wiki and donate

##### Entitas.Unity.CodeGenerator
- Removing invalid code generator names from Entitas.properties

##### Entitas.Unity.VisualDebugging
- Lots of UI tweaks
- Added toggle to sort systems by execution duration
- Added toggle to hide empty systems
- ReactiveSystems are highlighted with a white font color
- Added Clear Groups Button
- Added Entity Release Button
- Splitted systems list into initialize and execute systems and visualizing them separately
- Improved stepper UI

##### Entitas.Migration
- All migrations now contain information about on which folder they should be applied

```
0.26.0
  - Deactivates code to prevent compile erros
  - Use on folder, where generated files are located
```

##### Other
- Added Commands.GenerateProjectFiles and using it in build.sh
- Updated build.sh and build_commands.sh to include latest MigrationAssistant.exe


# 0.26.1

##### Breaking changes
Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)


# 0.26.0

##### General
- Updated projects to Unity 5.3
- Improved all error messages and added hints
- Changed and applied policy.mdpolicy to all sources

##### Entitas.Unity
- Moved Entitas Preferences to its own Editor Window

![Entitas.Unity - Entitas Preferences Window](https://cloud.githubusercontent.com/assets/233700/12222689/9492611a-b7c3-11e5-880d-c4cc83c9234e.png)

##### Other
- Added runTests.bat for running test on windows (#49)
- Updated license


# 0.25.0

##### Entitas
- Improved AERC performance
- Added group.RemoveAllEventHandlers()
- Added pool.ClearGroups() to remove all groups and remove all their event handlers
- Added pool.ResetCreationIndex()
- Throwing exception when there are retained entities and pool.DestroyAllEntities() is called
- Renamed entity.refCount to entity.retainCount

##### Entitas.Unity.VisualDebugging
- Fixed creating entities
- Showing warning when there are retained entities

##### Other
- Added UnityTests project with Unity Test Tools to fix a Unity specific HashSet bug


# 0.24.6

##### Entitas
- Changed entity.Retain() to accept an owner object

##### Entitas.Unity.VisualDebugging
- Added VisualDebugging support for displaying owners of entities

![Entitas.Unity.VisualDebugging-RefrenceCount](https://cloud.githubusercontent.com/assets/233700/11320810/0463033a-90a7-11e5-931b-5074b50d7e62.png)


# 0.24.5

##### Entitas
- Fixed dispatching group events after all groups are updated

##### Entitas.CodeGenerator
- Supporting ENTITAS_DISABLE_VISUAL_DEBUGGING compiler flag


# 0.24.4

##### Entitas
- Added entity.componentNames. This field is set by Entitas.Unity.VisualDebugging to provide better error messages
- Added matcher.componentNames. This field is set by Entitas.Unity.CodeGenerator to provide better error messages
- entity.ToString() now removes ComponentSuffix
- Fixed typo

##### Entitas.Unity.CodeGenerator
- ComponentExtensionsGenerator sets matcher.componentNames
- Removed generating unused using in ComponentExtensionsGenerator

##### Other
- Added update_project_dependencies.sh
- Refactored build commands into build_commands.sh


# 0.24.3

##### Entitas
- Added systems.ActivateReactiveSystems() and systems.DeactivateReactiveSystems which should be called when you don't use systems anymore

##### Other
- Merged shell scripts


# 0.24.2

##### General
- Renamed XyzEditor to XyzInspector
- Streamlined naming

##### Entitas.Unity.VisualDebugging
- Simplified adding a component at runtime

##### Other
- buildPackage.sh now creates Entitas-CSharp.zip and Entitas-Unity.zip


# 0.24.1

##### Entitas.Unity.VisualDebugging
- Added support for adding components to multiple entities at once at runtime

![Entitas.Unity.VisualDebugging-Entity](https://cloud.githubusercontent.com/assets/233700/10293066/d4668120-6bb2-11e5-895e-cfdd25cc2e74.png)


# 0.24.0

##### Breaking changes
Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)

##### Entitas.Unity.CodeGenerator
- Throwing exception when attempting to generate while Unity is still compiling or assembly won't compile

##### Entitas.Unity.VisualDebugging
- Added support for creating entities and adding components at runtime

![Entitas.Unity.VisualDebugging-PoolObserver](https://cloud.githubusercontent.com/assets/233700/10291395/d83c3ec4-6ba9-11e5-9c1d-3e18fe2c6370.png)

![Entitas.Unity.VisualDebugging-Entity](https://cloud.githubusercontent.com/assets/233700/10291401/e15d29be-6ba9-11e5-8fc1-87767430342c.png)


# 0.23.0

##### Breaking changes
Before updating, please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)

- Gerneral
  - Updated and applied policy

##### Entitas
- Reimplemented new matcher AnyOf and NoneOf

```csharp
Matcher.AllOf(Matcher.A, Matcher.B)
       .AnyOf(Matcher.C, Matcher.D)
       .NoneOf(Matcher.E, Matcher.F);

```

##### Entitas.CodeGenerator
- Updated generators to work with new matchers
- PoolsGenerator generates Pools.allPools (#39)
- Code Generators convert local newline to unix newline

##### Entitas.Unity.CodeGenerator
- Changed CodeGeneratorConfig.disabledCodeGenerators to CodeGeneratorConfig.enabledCodeGenerators


# 0.22.3

##### Entitas
- Added reactiveSystem.Clear() and systems.ClearReactiveSystems()
- Added IClearReactiveSystem. When implemented, clears reactive system after execute finished


# 0.22.2

##### Fixes
- Entitas
  - GroupObserver retains entities only once

##### Entitas.Unity.VisualDebugging
- PoolObserver now shows retained entities
- Destroying EntityBehaviour e.OnEntityReleased instead of e.OnComponentRemoved

##### Other
- New logo


# 0.22.1

##### Entitas
- Throwing an exception when releasing an entity that is not destroyed yet (#32)

##### Entitas.Unity.VisualDebugging
- Added hierarchy icon
- Renamed DebugSystems related classes

##### Other
- buildPackage.sh includes HierarchyIcon.png.meta


# 0.22.0

##### Breaking changes
Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)

- Entitas
  - Restored previous pool.DestroyEntity() behaviour
  - IReactiveSystem and IMultiReactiveSystem changed and use `TriggerOnEvent`
  - Use the command line tool `MigrationAssistant.exe` to automatically migrate IReactiveSystem
  - Renamed IStartSystem.Start to IInitializeSystem.Initialize (#21)

##### Fixes
- Entitas
  - e.RemoveAllComponents() updates toString cache, even if entity has no components

##### Entitas
- Added AERC (Automatic Entity Reference Counting) (#30, solves #25)
- Reduced gc allocations in e.RemoveAllComponents()
- Reduced gc allocations in pool.CreateEntity() and pool.DestroyEntity()
- pool.DestroyEntity() will clean suscribed event delegates of entities (#27)
- entity.ToString() will always use component type
- Streamlined and refactored tests and sources

##### Entitas.Unity.VisualDebugging
- Improved SystemMonitorEditor graph performance (#14)

#### Entitas.Migration
- Added M0220 (Migrates IReactiveSystem to combine trigger and eventTypes to TriggerOnEvent)
- Updated migration descriptions

##### Other
- Removed project files
- Renamed updateDependencies.sh to updateProjects.sh
- buildPackage.sh includes EntitasUpgradeGuide.md in Entitas.zip


# 0.21.0

##### Fixes
- Entitas.Migration
  - Changed target framework to .NET 3.5 to fix build errors in VisualStudio (#22)

##### Entitas
- Changed pool.DestroyEntity(entity) behaviour
  - won't trigger group.OnEntityRemoved anymore
  - triggers group.OnEntityWillBeDestroyed
  - removes entity from all groupObserver.collectedEntities
    - ReactiveSystem doesn't pass on destroyed entities anymore
- ReactiveSystem doesn't call Execute() when filtered entities.Count == 0

##### Other
- Added project files (#18)


# 0.20.0

##### Breaking changes
- Entitas
  - Removed all matchers except AllOfMatcher

##### Entitas
- Added `IEnsureComponents` to optionally ensure entities passed in via ReactiveSystem have certain components
- Added `IExcludeComponents` to optionally exclude entities passed in via ReactiveSystem
- Added support for multiple PoolAttributes on components

```csharp
[PoolA, PoolB, PoolC]
public class SomeComponent : IComponent {}
```

##### Entitas.Unity.CodeGenerator
- Added `disabledCodeGenerators` to CodeGeneratorConfig
- Added code generator toggles to CodeGeneratorPreferencesDrawer

![Entitas.Unity.Codegenerator.disabledcodegenerators](https://cloud.githubusercontent.com/assets/233700/9046406/b4c6b7c2-3a2a-11e5-8624-a8988f684579.png)

##### Entitas.Unity.VisualDebugging
- Nicer stats


# 0.19.1

##### Entitas
- GroupObserver supports observing multiple groups
- Added support for IMultiReactiveSystem
- Added internal entity._isEnabled to prevent modifying pooled entities
- Replaced internal object pool with Stack<Entity>

##### Entitas.CodeGenerator
- Fixed generated replace method, when replacing non existent component

##### Entitas.Unity.VisualDebugging
- Drastically improved performance and memory usage by caching ToString() and reducing setting gameObject.name


# 0.19.0

##### Breaking changes
Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)

- Entitas
  - Added new e.OnComponentReplaced and removed all *WillBeRemoved events
  - Added component index and changed component to OnEntityAdded and OnEntityRemoved
  - IReactiveSystem.Execute takes List<Entity> instead of Entity[]
    - Entitas now runs without producing garbage!

- Entitas.CodeGenerator
  - Removed support for properties in components

- Entitas.Unity.VisualDebugging
  - Replaced DebugPool with a more flexible PoolObserver

##### Entitas
- Added group.OnEntityUpdated event with previous and new component

##### Entitas.CodeGenerator
- ComponentExtensionsGenerator generates component object pool
- Converting newlines in generated files to Environment.NewLine (Pull request #11, thanks @movrajr)

##### Other
- Added policy.mdpolicy


# 0.18.3

##### Entitas
- Added ReactiveSystem.Activate() and .Deactivate()

##### Entitas.Unity.VisualDebugging
- Displaying nested systems hierarchy for DebugSystems

![Entitas.Unity.VisualDebugging-DebugSystemsHierarchy](https://cloud.githubusercontent.com/assets/233700/8761742/6e26dd22-2d61-11e5-943b-94683b7b02ec.png)
![Entitas.Unity.VisualDebugging-DebugSystemsHierarchyEditor](https://cloud.githubusercontent.com/assets/233700/8761746/9628dbfe-2d61-11e5-9b75-570e5c538c0d.png)
- Unchecking a ReacitveSystem in VisualDebugging deactivates it


# 0.18.2

##### Entitas.CodeGenerator
- Fixed #9


# 0.18.1

##### Entitas.CodeGenerator
- ComponentExtensionsGenerator now supports properties


# 0.18.0

##### Breaking changes
- Use the command line tool `MigrationAssistant.exe` to automatically migrate
    - Changed IReactiveSystem.GetTriggeringMatcher to IReactiveSystem.trigger
    - Changed IReactiveSystem.GetEventType to IReactiveSystem.eventType

Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)

##### Entitas.Unity
- Fixed code generation issues on Windows by converting and normalizing line endings
- Fixed EntitasCheckForUpdates.CheckForUpdates() by temporarily trusting all sources


# 0.17.0

##### Breaking changes
- Added `systemCodeGenerators` to CodeGenerator.Generate()

```csharp
CodeGenerator.Generate(Type[] types, string[] poolNames, string dir,
                            IComponentCodeGenerator[] componentCodeGenerators,
                            ISystemCodeGenerator[] systemCodeGenerators,
                            IPoolCodeGenerator[] poolCodeGenerators)
```

##### Entitas.CodeGenerator
- Added PoolsGenerator which creates a getter for all pools

```csharp
var pool = Pools.pool;
var metaPool = Pools.meta;
```

- Added SystemExtensionsGenerator

```csharp
new Systems()
    .Add(pool.CreateGameBoardSystem())
    .Add(pool.CreateCreateGameBoardCacheSystem())
    .Add(pool.CreateFallSystem())
    .Add(pool.CreateFillSystem())

    .Add(pool.CreateProcessInputSystem())

    .Add(pool.CreateRemoveViewSystem())
    .Add(pool.CreateAddViewSystem())
    .Add(pool.CreateRenderPositionSystem())

    .Add(pool.CreateDestroySystem())
    .Add(pool.CreateScoreSystem());
```
- Added Components, Systems & Pools sub folders to generated folder

##### Entitas.Unity
- Properties split with Environment.NewLine instead of '\n'

##### Entitas.Unity.CodeGenerator
- Entitas preferences appends "/Generated/" to generated folder if necessary

##### Entitas.Unity.VisualDebugging
- Using Queue<float> for SystemsDebugEditor.systemMonitorData


# 0.16.0

##### Breaking changes
- Moved system getters from Systems to DebugSystems

##### Entitas.Unity.CodeGenerator
- Generated ComponentIds use array instead of dictionary for component name lookup

##### Entitas.Unity.VisualDebugging
- Added "Step manually" to DebugSystems
- Added activate / deactivate systems at runtime
- Displaying Systems.totalSystemsCount in SystemsDebugEditor
- Added SystemsMonitor visual graph

![Entitas.Unity.VisualDebugging-DebugSystems](https://cloud.githubusercontent.com/assets/233700/8241713/3bf5e3ce-160b-11e5-8876-497bb09c04b1.png)
- Removed override DebugSystems.DestroyAllEntities()


# 0.15.0

##### Entitas
- Added entitas_version file
- Added CreateSystem(ISystem) to PoolExtensions
- Fixed typo GroupObserver.ClearCollectedEntities()

##### Entitas.Unity
- Added "Check for updates..." menu item

##### Entitas.Unity.VisualDebugging
- Added Stats menu item to log current components, systems and pools


# 0.14.0

##### General
- Upgraded all Unity projects to Unity 5

##### Entitas
- Added Systems class
- Re-combined pool extensions for creating systems to pool.CreateSystem() and removed pool.CreateStartSystem() and pool.CreateExecuteSystem()
- Fixed: Pool won't destroy entities it doesn't contain

##### Entitas.Unity
- Properties now support multiline values and placeholder replacement with ${key}

##### Entitas.Unity.CodeGenerator
- Added fluent api to Entity

```csharp
pool.CreateEntity()
    .IsGameBoardElement(true)
    .IsMovable(true)
    .AddPosition(x, y)
    .AddResource(Res.Piece0)
    .IsInteractive(true);
```
- CodeGenerator takes arrays of IComponentCodeGenerator and IPoolCodeGenerator to generate files so you can easily provide your own custom code generators
- Added dialog for 'Migrate Matcher' menu item

##### Entitas.Unity.VisualDebugging
- Added DebugSystems

![Entitas.Unity.VisualDebugging-Systems](https://cloud.githubusercontent.com/assets/233700/7938066/ebe8b4b6-0943-11e5-9cec-ce694d624aca.png)
- Added HashSetTypeDrawer


# 0.13.0

##### Reminder
- Entitas 0.12.0 generates prefixed matchers based on the PoolAttribute and introduces some API changes. Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)

##### General
- Split into multiple modules and seperate projects. Entitas now consists of
  - Entitas
  - Entitas.CodeGenerator
  - Entitas.Unity
  - Entitas.Unity.CodeGenerator
  - Entitas.Unity.VisualDebugging

##### Entitas.Unity
- Added IEntitasPreferencesDrawer to be able to extend the Entitas preferences panel

##### Entitas.Unity.CodeGenerator
- Entitas preferences internal keys changed. Please check your settings in projectRoot/Entitas.properties and update keys
  - Entitas.CodeGenerator.GeneratedFolderPath -> Entitas.Unity.CodeGenerator.GeneratedFolderPath
  - Entitas.CodeGenerator.Pools               -> Entitas.Unity.CodeGenerator.Pools

##### Entitas.Unity.VisualDebugging
- Added support to set fields to null
- Added support to create a new instance if the value of a field is null
- Added IDefaultInstanceCreator to create default objects for unsupported types
- Added IDefaultInstanceCreator implementations for array, dictionary and string
- Added support to insert and remove elements from lists, arrays and dictionaries

![Entitas.Unity.VisualDebugging-ITypeDrawer](https://cloud.githubusercontent.com/assets/233700/7339538/226d8028-ec72-11e4-8971-53029fb20da8.png)
- Added name property to DebugPool
- Added VisualDebuggingConfig and VisualDebuggingPreferencesDrawer

![Entitas.Unity.VisualDebugging-Preferences](https://cloud.githubusercontent.com/assets/233700/7339599/ef454f34-ec74-11e4-9775-963f477bfb16.png)
- EntityDebugEditor can generate IDefaultInstanceCreator and ITypeDrawer implementations for unsupported types
- Fixed: handling null values
- Renamed ICustomTypeDrawer to ITypeDrawer
- Big refactoring to simplify drawing types

##### Other
- buildPackage.sh keeps uncompressed source files in bin folder
- Added updateDependencies.sh which updates all dependencies of Entitas.Unity.CodeGenerator, Entitas.Unity.VisualDebugging and tests
- Renamed and moved files and folders to be more consistent with the new project structure


# 0.12.0

##### Important
- Entitas 0.12.0 generates prefixed matchers based on the PoolAttribute and introduces some API changes. Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)

##### Entitas
- Added IStartSystem and pool.CreateStartSystem() extension
- Renamed pool.CreateSystem() to pool.CreateExecuteSystem()
- Added pool.CreateStartSystem()
- Added EntitasUpdater to automatically update the introduced matcher API changes

##### Visual Debugging
- Fixed null exceptions
- Added support for multi dimensional and jagged arrays
- Removed Debug.Log

##### Code Generator
- Added Code Generator PreferenceItem
  - set generated folder path
  - define multiple pools

![Entitas.Unity.CodeGenerator-Preferences](https://cloud.githubusercontent.com/assets/233700/7296726/8d74bb5a-e9c2-11e4-8324-10a0db7191ff.png)
- Added PoolAttributeGenerator
- Generated Matcher is now prefixed based on PoolAttribute (e.g. UIMatcher)
- Generating ToString() for matchers to print component name instead of index
- IndicesLookupGenerator generates indices ordered alphabetically
- Added TypeGenerator to streamline string generation from types
- Added support for nested classes

##### Other
- Added Properties and CodeGeneratorConfig to serialize Entitas preferences to file
- Removed warning in AbstractCompoundMatcher
- buildPackage.sh only builds when all tests are passing
- buildPackage.sh deletes meta files before creating zip archive


# 0.11.0

##### Reminder
- Entitas 0.10.0 included lots of renaming. Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md) if you are on < v0.10.0

##### Entitas
- Added AllOfCompoundMatcher
- Added AnyOfMatcher
- Added AnyOfCompoundMatcher
- Added NoneOfMatcher
- Added NoneOfCompoundMatcher
- Updated Entitas to handle any implementation of IMatcher
- Fixed dispatching OnComponentAdded when replacing a non existing component with null
- Optimizations

##### Visual Debugging
- Added support for custom type drawers `ICustomTypeDrawer`
- Added component folding and pooled entities count
- Added groups to PoolDebugEditor

![Entitas.Unity.VisualDebugging-Groups](https://cloud.githubusercontent.com/assets/233700/6547980/e342b3fe-c5e9-11e4-8caa-77662a14679b.png)
- Added support for IList

![Entitas.Unity.VisualDebugging-IList](https://cloud.githubusercontent.com/assets/233700/6547984/eecc3e3e-c5e9-11e4-98bb-700a84047abe.png)
- UI improvements

##### Code Generator
- Fixed typeShortcuts to use type.FullName to support UnityEngine.Object (conflicted with System.Object)
- Added EntitasCodeGeneratorMenuItem

##### Other
- Moved and renamed some folders
- Added buildPackage.sh which creates a bin/Entitas.zip with all necessary source files


# 0.10.0

##### Important
- Entitas 0.10.0 includes lots of renaming. Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)

##### Entitas
- Added empty ISystem and IExecuteSystem for more flexibility
- Added public creationIndex to Entity
- Observer is now on group not on pool
- Removed WillBeRemovedSystem and observer
- Added CreateSystem to PoolExtension
- Added fast entities count call to Pool
- Added creationIndex to entity.ToString()
- pool.CreateEntity() and pool.DestroyEntity() are now virtual

##### Visual Debugging
- Added VisualDebugging

##### Code Generator
- Supports enums nested in components
- Added option to [DontGenerate] to ignore generating index, too
