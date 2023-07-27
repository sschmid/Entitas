<p align="center">
    <img src="images/Entitas-Header.png" alt="Entitas">
</p>
<p align="center">
    <a href="https://discord.gg/uHrVx5Z"><img src="https://img.shields.io/discord/599321316377624601.svg?logo=discord&logoColor=FFFFFF&label=Discord&labelColor=6A7EC2&color=7389D8" alt="Entitas on Discord"></a>
    <a href="https://github.com/sschmid/Entitas/releases"><img src="https://img.shields.io/github/release/sschmid/Entitas.svg" alt="Latest release"></a>
    <a href="https://twitter.com/intent/follow?original_referer=https%3A%2F%2Fgithub.com%2Fsschmid%2FEntitas&screen_name=entitas_csharp&tw_p=followbutton"><img src="https://img.shields.io/twitter/follow/entitas_csharp" alt="Twitter Follow Me"></a>
    <a href="https://twitter.com/intent/follow?original_referer=https%3A%2F%2Fgithub.com%2Fsschmid%2FEntitas&screen_name=s_schmid&tw_p=followbutton"><img src="https://img.shields.io/twitter/follow/s_schmid" alt="Twitter Follow Me"></a>
</p>
<p align="center">
    <b>Entitas is free, but powered by</b>
    <a href="https://www.paypal.com/donate/?hosted_button_id=BTMLSDQULZ852"><b>your donations</b></a>
</p>
<p align="center">
    <a href="https://www.paypal.com/donate/?hosted_button_id=BTMLSDQULZ852"><img src="https://img.shields.io/static/v1.svg?logo=paypal&label=PayPal&labelColor=3F70B6&&message=Donate&color=gray" alt="Donate"></a>
</p>

# Entitas - The Entity Component System Framework for C# and Unity

Entitas is the most popular open-source Entity Component System Framework (ECS)
and is specifically made for C# and Unity. Several design decisions have been
made to work optimal in a garbage collected environment and to go easy on the
garbage collector. Entitas comes with an optional code generator which radically
reduces the amount of code you have to write and
[makes your code read like well written prose.](https://cleancoders.com)

# Why Entitas

- [#1 open-source ECS on GitHub](https://github.com/sschmid/Entitas)
- 100% open-source under the [MIT License](LICENSE.md)
- great and helpful community on [Discord](https://discord.gg/uHrVx5Z)
- easy to learn and easy to use
- works great in pure C# standalone projects without Unity
- comes with great Unity integration called Visual Debugging
- battle-tested at companies like [Popcore](https://popcore.com) (Rollic / Zynga / Take Two), [Gram Games](https://gram.gs), [Wooga](https://www.wooga.com), [Plarium](https://plarium.com), [Storm Chaser](https://www.stormchaser-games.com) and many more

# Video Tutorials and Unity Unite Talks

| Video                                                                                                                                                                                   | Title                                                     | Resources                                                                           |
|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------|-------------------------------------------------------------------------------------|
| <a href="https://www.youtube.com/watch?v=DZpvUnj2dGI"><img src="https://img.youtube.com/vi/DZpvUnj2dGI/0.jpg" width="200" alt="Video: Entitas - Shmup - Part 2"></a>                    | Entitas ECS Unity Tutorial - Git & Unit Tests             |                                                                                     |
| <a href="https://www.youtube.com/watch?v=L-18XRTarOM"><img src="https://img.youtube.com/vi/L-18XRTarOM/0.jpg" width="200" alt="Video: Entitas - Shmup - Part 1"></a>                    | Entitas ECS Unity Tutorial - Setup & Basics               |                                                                                     |
| <a href="https://www.youtube.com/watch?v=Phx7IJ3XUzg"><img src="https://img.youtube.com/vi/Phx7IJ3XUzg/0.jpg" width="200" alt="Video: Watch the Entitas Talk at Unite Europe 2016"></a> | Unite Europe 2016: ECS architecture with Unity by example | [SlideShare: Unite Europe 2016](http://www.slideshare.net/sschmid/uniteeurope-2016) |
| <a href="https://www.youtube.com/watch?v=Re5kGtxTW6E"><img src="https://img.youtube.com/vi/Re5kGtxTW6E/0.jpg" width="200" alt="Video: Watch the Entitas Talk at Unite Europe 2015"></a> | Unite Europe 2015: Entity system architecture with Unity  | [SlideShare: Unite Europe 2015](http://www.slideshare.net/sschmid/uniteeurope-2015) |

# First glimpse

The optional [code generator](https://github.com/sschmid/Entitas/wiki/Code-Generator)
lets you write code that is super fast, safe and literally screams its intent.

```csharp
var entity = context.CreateEntity();
entity.AddPosition(Vector3.zero);
entity.AddVelocity(Vector3.forward);
entity.AddAsset("Player");
```

```csharp
using static GameMatcher;

public sealed class MoveSystem : IExecuteSystem
{
    readonly IGroup<GameEntity> _group;

    public MoveSystem(GameContext context)
    {
        _group = context.GetGroup(AllOf(Position, Velocity));
    }

    public void Execute()
    {
        foreach (var e in _group.GetEntities())
            e.ReplacePosition(e.position.value + e.velocity.value);
    }
}
```

# Overview

Entitas is fast, light and gets rid of unnecessary complexity. There are less
than a handful classes you have to know to rocket start your game or application:

- Context
- Entity
- Component
- Group

```
Entitas ECS

+-----------------+
|     Context     |
|-----------------|
|    e       e    |      +-----------+
|       e      e--|----> |  Entity   |
|  e        e     |      |-----------|
|     e  e     e  |      | Component |
| e          e    |      |           |      +-----------+
|    e     e      |      | Component-|----> | Component |
|  e    e    e    |      |           |      |-----------|
|    e    e     e |      | Component |      |   Data    |
+-----------------+      +-----------+      +-----------+
  |
  |
  |     +-------------+  Groups:
  |     |      e      |  Subsets of entities in the context
  |     |   e     e   |  for blazing fast querying
  +---> |        +------------+
        |     e  |    |       |
        |  e     | e  |  e    |
        +--------|----+    e  |
                 |     e      |
                 |  e     e   |
                 +------------+
 ```

[Read more...](https://github.com/sschmid/Entitas/wiki/Home)

# Code Generator

The Code Generator generates classes and methods for you, so you can focus on
getting the job done. It radically reduces the amount of code you have to write
and improves readability by a huge magnitude. It makes your code less error-prone
while ensuring best performance.

[Read more...](https://github.com/sschmid/Entitas/wiki/Code-Generator)

# Unity integration

The optional Unity module "Visual Debugging" integrates Entitas nicely into Unity and provides powerful
editor extensions to inspect and debug contexts, groups, entities, components and systems.

[Read more...](https://github.com/sschmid/Entitas/wiki/Unity-integration)

<p align="center">
    <img src="images/Entitas.Unity-MenuItems.png" alt="Entitas.Unity MenuItems" height="200"><br />
    <img src="images/Entitas.Unity.VisualDebugging-Entity.png" alt="Entitas.Unity.VisualDebugging Entity" width="400">
    <img src="images/Entitas.Unity.VisualDebugging-DebugSystems.png" alt="Entitas.Unity.VisualDebugging Systems" width="400">
</p>

# Entitas deep dive

[Read the wiki](https://github.com/sschmid/Entitas/wiki) or checkout the [example projects](https://github.com/sschmid/Entitas/wiki/Example-projects) to
see Entitas in action. These example projects illustrate how systems, groups, collectors and entities all play together seamlessly.


### **[» Download and setup](#download-and-setup-entitas)**
### **[» Video Tutorials and Unity Unite Talks](#video-tutorials-and-unity-unite-talks)**
### **[» Wiki and example projects](https://github.com/sschmid/Entitas/wiki)**
### **[» Ask a question](https://github.com/sschmid/Entitas/issues/new)**

---

# Download and setup Entitas

### GitHub releases (recommended)

[Show releases](https://github.com/sschmid/Entitas/releases)

### Unity package manager

> Coming soon

### NuGet

Entitas and all dependencies are available as [NuGet packages](https://www.nuget.org/packages?q=Entitas).
More detailed explanation coming soon.

### Unity Asset Store (deprecated)

[Entitas on the Unity Asset Store](http://u3d.as/NuJ) is deprecated and will not
be updated anymore. The last version available on the Asset Store is 1.12.3 and
is free to download. Please see discussion [Entitas turns 7 - and is FREE now ](https://github.com/sschmid/Entitas/discussions/1009)

# Thanks to

Big shout out to [@mzaks][github-mzaks], [@cloudjubei][github-cloudjubei] and [@devboy][github-devboy]
for endless hours of discussion and helping making Entitas awesome!

[github-mzaks]: https://github.com/mzaks "@mzaks"
[github-cloudjubei]: https://github.com/cloudjubei "@cloudjubei"
[github-devboy]: https://github.com/devboy "@devboy"

# Maintainers

- [@sschmid][github-sschmid] | [@s_schmid][twitter-sschmid] | [@entitas_csharp][twitter-entitas_csharp]

[github-sschmid]: https://github.com/sschmid "@sschmid"
[twitter-sschmid]: https://twitter.com/s_schmid "s_schmid on Twitter"
[twitter-entitas_csharp]: https://twitter.com/entitas_csharp "entitas_csharp on Twitter"

# Different language?

Entitas is available in
- [C#](https://github.com/sschmid/Entitas)
- [C++](https://github.com/JuDelCo/Entitas-Cpp)
- [Clojure](https://github.com/mhaemmerle/entitas-clj)
- [Crystal](https://github.com/spoved/entitas.cr)
- [Erlang](https://github.com/mhaemmerle/entitas_erl)
- [F#](https://github.com/darkoverlordofdata/entitas-fsharp)
- [Go](https://github.com/wooga/go-entitas)
- [Haskell](https://github.com/mhaemmerle/entitas-haskell)
- [Java](https://github.com/Rubentxu/entitas-java)
- [Kotlin](https://github.com/darkoverlordofdata/entitas-kotlin)
- [Objective-C](https://github.com/wooga/entitas)
- [Python](https://github.com/Aenyhm/entitas-python)
- [Scala](https://github.com/darkoverlordofdata/entitas-scala)
- [Swift](https://github.com/mzaks/Entitas-Swift)
- [TypeScript](https://github.com/darkoverlordofdata/entitas-ts)
