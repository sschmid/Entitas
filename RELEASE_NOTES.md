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

![entitas-itypedrawer](https://cloud.githubusercontent.com/assets/233700/7339538/226d8028-ec72-11e4-8971-53029fb20da8.png)
- Added name property to DebugPool
- Added VisualDebuggingConfig and VisualDebuggingPreferencesDrawer

![entitas-preferences](https://cloud.githubusercontent.com/assets/233700/7339599/ef454f34-ec74-11e4-9775-963f477bfb16.png)
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

##### General
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

![entitas-preferences](https://cloud.githubusercontent.com/assets/233700/7296726/8d74bb5a-e9c2-11e4-8324-10a0db7191ff.png)
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

##### General
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

![visualdebugging-groups](https://cloud.githubusercontent.com/assets/233700/6547980/e342b3fe-c5e9-11e4-8caa-77662a14679b.png)
- Added support for IList

![visualdebugging-ilist](https://cloud.githubusercontent.com/assets/233700/6547984/eecc3e3e-c5e9-11e4-98bb-700a84047abe.png)
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

##### General
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
