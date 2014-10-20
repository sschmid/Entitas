# Entitas - The Entity Component System for C# and Unity

Entitas is a super fast Entity Component System specifically made for C# and Unity. Internal caching and blazing fast component access makes it second to none. Several design decisions were made to work optimal in a garbage collected environment and go easy on the garbage collector.

branch  | tests
:------:|------
master  | [![Build Status](https://travis-ci.org/sschmid/Entitas-CSharp.svg?branch=master)](https://travis-ci.org/sschmid/Entitas-CSharp)
develop | [![Build Status](https://travis-ci.org/sschmid/Entitas-CSharp.svg?branch=develop)](https://travis-ci.org/sschmid/Entitas-CSharp)


## Getting started

Entitas is fast, light and gets rid of unnecessary complexity. There are less than a handful classes you have to know to rocket start your application:

- Entity
- Entity Repository
- Entity Collection
- Entity Repository Observer

### Entity
You can imagine an entity as a container holding data to represent certain objects in your application. You can add, replace or remove data from entites in form of `IComponent`. Entities have corresponding events to let you know if components where added or removed.

This example shows how you can interact with an entity. Entitas comes with a code generator that optionally can generate code for you to have a more natural and readable api (see **Code Generator** further down). In this example you can see some generated methods for `PositionComponent`, `HealthComponent`, `MovableComponent`.
```
entity.AddPosition(0, 0, 0);
entity.AddHealth(100);
entity.FlagMovable();

entity.ReplacePosition(10, 100, -30);
entity.ReplaceHealth(entity.health - 1);
entity.UnflagMovable();

entity.RemovePosition();

var hasPos = entity.hasPosition;
var movable = entity.isMovable;
```

### Entity Repository
The Entity Repository is the factory where you create and destroy entities. Use it to filter entities of interest.
```
// Total components is kindly generated for you by the code generator
var repo = new EntityRepository(ComponentIds.TotalComponents);
var entity = repo.CreateEntity();
entity.FlagMovable();

// Returns all entities having MoveComponent. Matcher.Movable is also generated for you.
var entities = repo.GetEntities(Matcher.Movable);
foreach (var e in entities) {
    // do something
}
```

### Entity Collection
Entity Collection enables super quick filtering on all the entities in the repository. They are continuously updated when entities change and can return groups of entities instantly. You have thousands of entities and want only those who have a `PositionComponent`? Just ask the repository for this collection, it already has the result waiting for you in no time.
```
repo.GetCollection(Matcher.Position).GetEntities();
```
Both the collection and getting the entities is cached, so even calling this method multiple times is super fast. Always try to use collections when possible. `repo.GetEntities(Matcher.Movable)` internally uses collections, too.

### Entity Repository Observer
Entity Repository Observer provides an easy way to react to changes made in the repository. Let's say you want to collect and process all the entities where a `PositionComponent` was added or replaced.
```
var observer = repo.AddObserver(
                   EntityCollectionEventType.OnEntityAdded,
                   Matcher.Position
               );

var entities = observer.collectedEntities;
foreach (var e in entities) {
    // do something
}
observer.ClearCollectedEntites();
```
To stop observing, simply deactive the observer.
```
observer.Deactivate();
```


## Processing entities with Systems

Implement `ISystem` to process your entities. I recommend you create systems for each single task or behaviour in your application and execute them in a defined order. This helps to keep your app deterministic. Entitas also provides a special system called `ReactiveEntitySystem`, which is using an Entity Repository Observer under the hood. It holds changed entities of interest at your fingertips. Check the wiki for examples (coming soon).


## Code Generator
The Code Generator can generate classes and methods for you, so you can focus on getting the job done. It radically reduces the amount of code you have to write and improves readability by a huge magnitude. It makes your code less error-prone while ensuring best performance. I strongly recommend using it!

The generated code can be different based on the content of the component. The Code Generator differentiates between four types:
- standard component with public fields (e.g. PositionComponent)
- single standard component that is meant to exist only once in the repository (e.g. UserComponent)
- flag component without any fields (e.g. MovableComponent)
- single flag component that is meant to exist only once in the repository (e.g. AnimatingComponent)

### And here is what you get

#### Standard component (e.g. PositionComponent)
```
public class PositionComponent : IComponent {
    public float x;
    public float y;
    public float z;
}
```
You get
```
var pos = e.position;
var has = e.hasPosition;

e.AddPosition(component);
e.AddPosition(x, y, z);

e.ReplacePosition(component);
e.ReplacePosition(x, y, z);

e.RemovePosition();
```

#### Single standard component (e.g. UserComponent)
```
[SingleEntity]
public class UserComponent : IComponent {
    public string name;
    public int age;
}
```
You get
```
// all from standard component plus methods for the repository

var e = repo.userEntity;
var name = repo.user.name;
var has = repo.hasUser;

repo.SetUser(component);
repo.SetUser("John", "42");

repo.ReplaceUser(component);
repo.ReplaceUser("Max", 24);

repo.RemoveUser();
```

#### Flag component (e.g. MovableComponent)
```
public class MovableComponent : IComponent {}
```
You get
```
var movable = e.isMovable;
e.FlagMovable();
e.UnflagMovable();
```

#### Single flag component (e.g. AnimatingComponent)
```
[SingleEntity]
public class AnimatingComponent : IComponent {}
```
You get
```
// all from flag component plus methods for the repository

var e = repo.animatingEntity;
var isAnimating = repo.isAnimating;
repo.FlagAnimating();
repo.UnflagAnimating();
```

### Unity tip
The following code extends Unity with a menu item that opens a new window that lets you generate code with a single click.

![Unity Entitas Menu](http://sschmid.com/Libs/Entitas-CSharp/Unity-Entitas-Menu.png "Unity Entitas Menu")
![Unity Entitas Window](http://sschmid.com/Libs/Entitas-CSharp/Unity-Entitas-Window.png "Unity Entitas Window")

```
using UnityEngine;
using UnityEditor;
using Entitas.CodeGenerator;

public class EntitasEditorWindow : EditorWindow {
    [MenuItem("Game/Entitas/Entitas Manager")]
    public static void ShowEntitasManagerWindow() {
        var window = (EntitasEditorWindow)EditorWindow.GetWindow(typeof(EntitasEditorWindow));
        window.title = "Entitas Manger";
        window.minSize = new Vector2(135, 45);

        EntitasCodeGenerator.generatedFolder = "Generated/";
    }

    void OnGUI() {
        if (GUILayout.Button("Generate extensions")) {
            EntitasCodeGenerator.Generate();
        }
        if (GUILayout.Button("Clean")) {
            EntitasCodeGenerator.CleanGeneratedFolder();
        }
        AssetDatabase.Refresh();
    }
}

```
