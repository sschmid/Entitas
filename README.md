<p align="center">
    <img src="https://raw.githubusercontent.com/sschmid/Entitas/master/Readme/Images/Entitas-Header.png" alt="Entitas">
</p>

<p align="center">
    <a href="https://discord.gg/uHrVx5Z">
        <img src="https://img.shields.io/discord/599321316377624601.svg?logo=discord&logoColor=FFFFFF&label=Discord&labelColor=6A7EC2&color=7389D8" alt="Entitas on Discord"></a>
    <a href="https://github.com/sschmid/Entitas/releases">
        <img src="https://img.shields.io/github/release/sschmid/Entitas.svg" alt="Latest release"></a>
</p>

<p align="center">
    <a href="https://twitter.com/intent/follow?original_referer=https%3A%2F%2Fgithub.com%2Fsschmid%2FEntitas&screen_name=s_schmid&tw_p=followbutton">
        <img src="https://img.shields.io/twitter/follow/s_schmid" alt="Twitter Follow Me"></a>
    <a href="https://twitter.com/intent/follow?original_referer=https%3A%2F%2Fgithub.com%2Fsschmid%2FEntitas&screen_name=entitas_csharp&tw_p=followbutton">
        <img src="https://img.shields.io/twitter/follow/entitas_csharp" alt="Twitter Follow Me"></a>
</p>

<p align="center">
    <b>Entitas is free, but powered by</b>
    <a href="https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BTMLSDQULZ852">
        <b>your donations</b>
    </a>
</p>

<p align="center">
    <a href="https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BTMLSDQULZ852">
        <img src="https://img.shields.io/static/v1.svg?logo=paypal&label=PayPal&labelColor=3F70B6&&message=Donate&color=gray" alt="Join the chat at https://gitter.im/sschmid/Entitas-CSharp"></a>
</p>

Entitas - The Entity Component System Framework for C# and Unity
================================================================

Entitas is a super fast Entity Component System Framework (ECS) specifically made for C# and Unity. Internal caching and blazing fast component access makes it second to none. Several design decisions have been made to work optimal in a garbage collected environment and to go easy on the garbage collector. Entitas comes with an optional code generator which radically reduces the amount of code you have to write and [makes your code read like well written prose.][clean-coders]

<p align="center">
    <a href="https://dev.windows.com">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas/master/Readme/Images/csharp.png" alt="CSharp" height="64"></a>
    <a href="http://unity3d.com">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas/master/Readme/Images/MadeForUnity.png" alt="Unity3d" height="64"></a>
    <a href="http://unity3d.com/unite/archive/2015">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas/master/Readme/Images/UniteEurope2015.png" alt="Unite Europe 2015" height="64"></a>
    <a href="https://unite.unity.com/2016/europe">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas/master/Readme/Images/UniteEurope2016.png" alt="Unite Europe 2016" height="64"></a>
    <a href="https://www.wooga.com">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas/master/Readme/Images/wooga-logo.png" alt="Wooga" height="64"></a>
    <a href="http://gram.gs">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas/master/Readme/Images/GramGames.png" alt="Gram Games.png" height="64"></a>
</p>

---

### **[» Download](#download-entitas)**
### **[» Documentation][documentation]**
### **[» Ask a question][issues-new]**
### **[» Wiki and example projects][wiki]**
### **[» #madeWithEntitas][wiki-games-and-examples]**

---

Video Tutorials & Unity Unite Talks
=================

| Entitas ECS Unity Tutorial        | Entitas ECS Unity Tutorial        | Entity system architecture with Unity                                | ECS architecture with Unity by example                               |
|:---------------------------------:|:---------------------------------:|:--------------------------------------------------------------------:|:--------------------------------------------------------------------:|
| [![Shmup1][shmup1-thumb]][shmup1] | [![Shmup2][shmup2-thumb]][shmup2] | [![Unite 15][unite15-thumb]][unite15]                                | [![Unite 16][unite16-thumb]][unite16]                                |
| Setup & Basics                    | Git & Unit Tests                  | [» Open the slides on SlideShare: Unite Europe 2015][unite15-slides] | [» Open the slides on SlideShare: Unite Europe 2016][unite16-slides] |


First glimpse
=============

The optional [code generator][wiki-code-generator] lets you write code that is super fast, safe and literally screams its intent.

```csharp
public static GameEntity CreateRedGem(this GameContext context, Vector3 position) {
    var entity = context.CreateEntity();
    entity.isGameBoardElement = true;
    entity.isMovable = true;
    entity.AddPosition(position);
    entity.AddAsset("RedGem");
    entity.isInteractive = true;
    return entity;
}
```

```csharp
var entities = context.GetEntities(Matcher<GameEntity>.AllOf(GameMatcher.Position, GameMatcher.Velocity));
foreach(var e in entities) {
    var pos = e.position;
    var vel = e.velocity;
    e.ReplacePosition(pos.value + vel.value);
}
```


Overview
========

Entitas is fast, light and gets rid of unnecessary complexity. There are less than a handful classes you have to know to rocket start your game or application:

- Entity
- Context
- Group
- Entity Collector

[Read more...][wiki-overview]


Code Generator
==============

The Code Generator generates classes and methods for you, so you can focus on getting the job done. It radically reduces the amount of code you have to write and improves readability by a huge magnitude. It makes your code less error-prone while ensuring best performance. I strongly recommend using it!

[Read more...][wiki-code-generator]


Unity integration
=================

The optional Unity module integrates Entitas nicely into Unity and provides powerful editor extensions to inspect and debug contexts, groups, entities, components and systems.

[Read more...][wiki-unity-integration]

<p align="center">
    <img src="https://raw.githubusercontent.com/sschmid/Entitas/master/Readme/Images/Entitas.Unity-MenuItems.png" alt="Entitas.Unity MenuItems" height="200"><br />
    <img src="https://raw.githubusercontent.com/sschmid/Entitas/master/Readme/Images/Entitas.Unity.VisualDebugging-Entity.png" alt="Entitas.Unity.VisualDebugging Entity" width="400">
    <img src="https://raw.githubusercontent.com/sschmid/Entitas/master/Readme/Images/Entitas.Unity.VisualDebugging-DebugSystems.png" alt="Entitas.Unity.VisualDebugging Systems" width="400">
</p>


Entitas deep dive
=================

[Read the wiki][wiki] or checkout the [example projects][wiki-example-projects] to see Entitas in action. These example projects illustrate how systems, groups, collectors and entities all play together seamlessly.


Download Entitas
================

Each release is published with zip files containing all source files you need.

[Show releases][releases]


Thanks to
=========

Big shout out to [@mzaks][github-mzaks], [@cloudjubei][github-cloudjubei] and [@devboy][github-devboy] for endless hours of discussion and helping making Entitas awesome!


Maintainer(s)
=============

- [@sschmid][github-sschmid] | [@s_schmid][twitter-sschmid] | [@entitas_csharp][twitter-entitas_csharp]


Different language?
===================

Entitas is available in
- [C#](https://github.com/sschmid/Entitas)
- [Swift](https://github.com/mzaks/Entitas-Swift)
- [C++](https://github.com/JuDelCo/Entitas-Cpp)
- [Objective-C](https://github.com/wooga/entitas)
- [Java](https://github.com/Rubentxu/entitas-java)
- [Python](https://github.com/Aenyhm/entitas-python)
- [Scala](https://github.com/darkoverlordofdata/entitas-scala)
- [Go](https://github.com/wooga/go-entitas)
- [F#](https://github.com/darkoverlordofdata/entitas-fsharp)
- [TypeScript](https://github.com/darkoverlordofdata/entitas-ts)
- [Kotlin](https://github.com/darkoverlordofdata/entitas-kotlin)
- [Haskell](https://github.com/mhaemmerle/entitas-haskell)
- [Erlang](https://github.com/mhaemmerle/entitas_erl)
- [Clojure](https://github.com/mhaemmerle/entitas-clj)
- [Crystal](https://github.com/spoved/entitas.cr)

[clean-coders]: https://cleancoders.com "Clean Coders"

[documentation]: http://sschmid.github.io/Entitas/ "Entitas Documentation"
[wiki]: https://github.com/sschmid/Entitas/wiki "Entitas Wiki"
[wiki-code-generator]: https://github.com/sschmid/Entitas/wiki/Code-Generator "Wiki - Code Generator"
[wiki-overview]: https://github.com/sschmid/Entitas/wiki/Overview "Wiki - Overview"
[wiki-unity-integration]: https://github.com/sschmid/Entitas/wiki/Unity-integration "Wiki - Unity Integration"
[wiki-example-projects]: https://github.com/sschmid/Entitas/wiki/Example-projects "Wiki - Example Projects"
[wiki-games-and-examples]: https://github.com/sschmid/Entitas/wiki/Made-With-Entitas "Wiki - #madeWithEntitas"

[shmup1-thumb]: https://raw.githubusercontent.com/sschmid/Entitas/master/Readme/Images/Entitas-Shmup-Part-1.jpg "Video: Entitas - Shmup - Part 1"
[shmup1]: https://www.youtube.com/watch?v=L-18XRTarOM "Video: Entitas - Shmup - Part 1"
[shmup2-thumb]: https://raw.githubusercontent.com/sschmid/Entitas/master/Readme/Images/Entitas-Shmup-Part-2.jpg "Video: Entitas - Shmup - Part 2"
[shmup2]: https://www.youtube.com/watch?v=DZpvUnj2dGI "Video: Entitas - Shmup - Part 2"
[unite15-thumb]: https://raw.githubusercontent.com/sschmid/Entitas/master/Readme/Images/UniteEurope2015-Video.png "Video: Watch the Entitas Talk at Unite Europe 2015"
[unite15]: https://www.youtube.com/watch?v=Re5kGtxTW6E "Video: Watch the Entitas Talk at Unite Europe 2015"
[unite15-slides]: http://www.slideshare.net/sschmid/uniteeurope-2015 "SlideShare: Unite Europe 2015"
[unite16-thumb]: https://raw.githubusercontent.com/sschmid/Entitas/master/Readme/Images/UniteEurope2016-Video.png "Video: Watch the Entitas Talk at Unite Europe 2016"
[unite16]: https://www.youtube.com/watch?v=Phx7IJ3XUzg "Video: Watch the Entitas Talk at Unite Europe 2016"
[unite16-slides]: http://www.slideshare.net/sschmid/uniteeurope-2016 "SlideShare: Unite Europe 2016"

[releases]: https://github.com/sschmid/Entitas/releases "Releases"
[issues-new]: https://github.com/sschmid/Entitas/issues/new "New issue"

[twitter-sschmid]: https://twitter.com/s_schmid "s_schmid on Twitter"
[twitter-entitas_csharp]: https://twitter.com/entitas_csharp "entitas_csharp on Twitter"

[github-sschmid]: https://github.com/sschmid "@sschmid"
[github-mzaks]: https://github.com/mzaks "@mzaks"
[github-cloudjubei]: https://github.com/cloudjubei "@cloudjubei"
[github-devboy]: https://github.com/devboy "@devboy"
