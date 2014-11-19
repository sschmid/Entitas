![Entitas Header](http://sschmid.com/Libs/Entitas-CSharp/Header.png)

# Entitas - The Entity Component System for C# and Unity
Entitas is a super fast Entity Component System specifically made for C# and Unity. Internal caching and blazing fast component access makes it second to none. Several design decisions were made to work optimal in a garbage collected environment and to go easy on the garbage collector. Entitas comes with an optional code generator which radically reduces the amount of code you have to write and [makes your code read like well written prose.](https://cleancoders.com)

branch  | tests
:------:|------
master  | [![Build Status](https://travis-ci.org/sschmid/Entitas-CSharp.svg?branch=master)](https://travis-ci.org/sschmid/Entitas-CSharp)
develop | [![Build Status](https://travis-ci.org/sschmid/Entitas-CSharp.svg?branch=develop)](https://travis-ci.org/sschmid/Entitas-CSharp)


## Getting started
Entitas is fast, light and gets rid of unnecessary complexity. There are less than a handful classes you have to know to rocket start your game or application:

- Entity
- Entity Repository
- Entity Collection
- Entity Repository Observer

After you've read the readme, take a look at the [example project](https://github.com/sschmid/Entitas-CSharp-Example.git) to see Entitas in action. The project illustrates how systems, collections, observers and entities all play together seamlessly.


### Entity
You can imagine an entity as a container holding data to represent certain objects in your application. You can add, replace or remove data from entities in form of `IComponent`. Entities have corresponding events to let you know if components were added, replaced or removed.

Here's how you can interact with an entity. To enjoy a more natural and more readable API, simply use the code generator that comes with Entitas (see **Code Generator** further down). In this example you can see some generated methods for `PositionComponent`, `HealthComponent`, `MovableComponent`.
````cs
entity.AddPosition(0, 0, 0);
entity.AddHealth(100);
entity.isMovable = true;

entity.ReplacePosition(10, 100, -30);
entity.ReplaceHealth(entity.health.health - 1);
entity.isMovable = false;

entity.RemovePosition();

var hasPos = entity.hasPosition;
var movable = entity.isMovable;
```

### Entity Repository
The Entity Repository is the factory where you create and destroy entities. Use it to filter entities of interest.
```cs
// Total components is kindly generated for you by the code generator
var repo = new EntityRepository(ComponentIds.TotalComponents);
var entity = repo.CreateEntity();
entity.isMovable = true;

// Returns all entities having MoveComponent and PositionComponent. Matchers are also generated for you.
var entities = repo.GetEntities(Matcher.AllOf(Matcher.Movable, Matcher.Position));
foreach (var e in entities) {
    // do something
}
```

### Entity Collection
Entity Collection enables super quick filtering on all the entities in the repository. They are continuously updated when entities change and can return groups of entities instantly. You have thousands of entities and want only those who have a `PositionComponent`? Just ask the repository for this collection, it already has the result waiting for you in no time.
```cs
repo.GetCollection(Matcher.Position).GetEntities();
```
Both the collection and getting the entities is cached, so even calling this method multiple times is super fast. Always try to use collections when possible. `repo.GetEntities(Matcher.Movable)` internally uses collections, too.

### Entity Repository Observer
Entity Repository Observer provides an easy way to react to changes made in the repository. Let's say you want to collect and process all the entities where a `PositionComponent` was added or replaced.
```cs
var observer = repo.CreateObserver(
                   Matcher.Position,
                   EntityCollectionEventType.OnEntityAdded
               );

var entities = observer.collectedEntities;
foreach (var e in entities) {
    // do something
}
observer.ClearCollectedEntites();
```
To stop observing, simply deactivate the observer.
```cs
observer.Deactivate();
```


## Processing entities with Systems
Implement `ISystem` to process your entities. I recommend you create systems for each single task or behaviour in your application and execute them in a defined order. This helps to keep your app deterministic. Entitas also provides a special system called `ReactiveEntitySystem`, which is using an Entity Repository Observer under the hood. It holds changed entities of interest at your fingertips. Be sure to check out the [example project](https://github.com/sschmid/Entitas-CSharp-Example.git).

## Code Generator
The Code Generator generates classes and methods for you, so you can focus on getting the job done. It radically reduces the amount of code you have to write and improves readability by a huge magnitude. It makes your code less error-prone while ensuring best performance. I strongly recommend using it!

The generated code can be different based on the content of the component. The Code Generator differentiates between four types:
- standard component with public fields (e.g. PositionComponent)
- single standard component that is meant to exist only once in the repository (e.g. UserComponent)
- flag component without any fields (e.g. MovableComponent)
- single flag component that is meant to exist only once in the repository (e.g. AnimatingComponent)

### And here is what you get

#### Standard component (e.g. PositionComponent)
```cs
public class PositionComponent : IComponent {
    public int x;
    public int y;
    public int z;
}
```
You get
```cs
var pos = e.position;
var has = e.hasPosition;

e.AddPosition(x, y, z);
e.AddPosition(component);

e.ReplacePosition(x, y, z);

e.RemovePosition();
```

#### Single standard component (e.g. UserComponent)
```cs
[SingleEntity]
public class UserComponent : IComponent {
    public string name;
    public int age;
}
```
You get
```cs
// all from standard component plus methods for the repository

var e = repo.userEntity;
var name = repo.user.name;
var has = repo.hasUser;

repo.SetUser("John", 42);
repo.SetUser(component);

repo.ReplaceUser("Max", 24);

repo.RemoveUser();
```

#### Flag component (e.g. MovableComponent)
```cs
public class MovableComponent : IComponent {}
```
You get
```cs
var movable = e.isMovable;
e.isMovable = true;
e.isMovable = false;
```

#### Single flag component (e.g. AnimatingComponent)
```cs
[SingleEntity]
public class AnimatingComponent : IComponent {}
```
You get
```cs
// all from flag component plus methods for the repository

var e = repo.animatingEntity;
var isAnimating = repo.isAnimating;
repo.isAnimating = true;
repo.isAnimating = false;
```

### Unity tip
The following code extends Unity with a menu item that opens a new window that lets you generate code with a single click.

![Unity Entitas Menu](http://sschmid.com/Libs/Entitas-CSharp/Unity-Entitas-Menu.png "Unity Entitas Menu")
![Unity Entitas Window](http://sschmid.com/Libs/Entitas-CSharp/Unity-Entitas-Window.png "Unity Entitas Window")

```cs
using UnityEngine;
using UnityEditor;
using Entitas.CodeGenerator;
using System.Reflection;

public class EntitasEditorWindow : EditorWindow {
    [MenuItem("Game/Entitas/Entitas Manager")]
    public static void ShowEntitasManagerWindow() {
        var window = (EntitasEditorWindow)EditorWindow.GetWindow(typeof(EntitasEditorWindow));
        window.title = "Entitas Manger";
        window.minSize = new Vector2(135, 45);
    }

    void OnGUI() {

        EntitasCodeGenerator.generatedFolder = "Assets/Sources/Generated/";

        if (GUILayout.Button("Generate")) {
            var assembly = Assembly.GetAssembly(typeof(EntitasCodeGenerator));
            EntitasCodeGenerator.Generate(assembly);
        }
        if (GUILayout.Button("Clean")) {
            EntitasCodeGenerator.CleanGeneratedFolder();
        }
        AssetDatabase.Refresh();
    }
}
```

# Thanks to
Big shout out to [@mzaks](https://github.com/mzaks), [@cloudjubei](https://github.com/cloudjubei) and [@devboy](https://github.com/devboy) for endless hours of discussion and helping making Entitas awesome!

# Different language?
Entitas is also available in
- [Swift](https://github.com/arne-schroppe/Entitas-Swift)
- [Objective-C](https://github.com/wooga/entitas)
- [Clojure](https://github.com/mhaemmerle/entitas-clj)
