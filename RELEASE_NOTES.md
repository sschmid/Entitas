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
