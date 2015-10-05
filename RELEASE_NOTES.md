# 0.24.1
##### Entitas.Unity.VisualDebugging
- Added support for adding components to multiple entities at once at runtime


# 0.24.0

##### Breaking changes
Please follow the [Entitas upgrade guide](https://github.com/sschmid/Entitas-CSharp/blob/master/EntitasUpgradeGuide.md)

##### Entitas.Unity.CodeGenerator
- Throwing exception when attempting to generate while Unity is still compiling or assembly won't compile

##### Entitas.Unity.VisualDebugging
- Added support for creating entities and adding components at runtime

![entitas unity visualdebugging-debugpool](https://cloud.githubusercontent.com/assets/233700/10291395/d83c3ec4-6ba9-11e5-9c1d-3e18fe2c6370.png)

![entitas unity visualdebugging-entity](https://cloud.githubusercontent.com/assets/233700/10291401/e15d29be-6ba9-11e5-8fc1-87767430342c.png)


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
	- Entitas.CodeGenerator.Pools 				-> Entitas.Unity.CodeGenerator.Pools

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
