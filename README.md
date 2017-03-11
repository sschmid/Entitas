<p align="center">
    <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/master/Readme/Images/Entitas-Header.png" alt="Entitas">
</p>

---

<p align="center">
    <a>🎉 Happy birthday Entitas! 🎉<br />3 years of clean code<br />Please support the development<br/></a>
    <a href="https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BTMLSDQULZ852">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/master/Readme/Images/Donate-PayPal.gif" alt="Thank you!"></a>
</p>

---

<p align="center">
    <a href="https://gitter.im/sschmid/Entitas-CSharp?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge">
        <img src="https://img.shields.io/badge/chat-on%20gitter-brightgreen.svg" alt="Join the chat at https://gitter.im/sschmid/Entitas-CSharp"></a>

    <a href="https://twitter.com/intent/follow?original_referer=https%3A%2F%2Fgithub.com%2Fsschmid%2FEntitas-CSharp&screen_name=s_schmid&tw_p=followbutton">
        <img src="https://img.shields.io/badge/twitter-follow%20%40s__schmid-blue.svg" alt="Twitter Follow Me"></a>

    <a href="https://twitter.com/intent/follow?original_referer=https%3A%2F%2Fgithub.com%2Fsschmid%2FEntitas-CSharp&screen_name=entitas_csharp&tw_p=followbutton">
        <img src="https://img.shields.io/badge/twitter-follow%20%40entitas__csharp-blue.svg" alt="Twitter Follow Me"></a>

    <a href="https://travis-ci.org/sschmid/Entitas-CSharp">
        <img src="https://travis-ci.org/sschmid/Entitas-CSharp.svg?branch=master" alt="Build Status"></a>

    <a href="https://github.com/sschmid/Entitas-CSharp/releases">
        <img src="https://img.shields.io/github/release/sschmid/Entitas-CSharp.svg" alt="Latest release"></a>
</p>


Entitas - The Entity Component System Framework for C# and Unity
================================================================

Entitas is a super fast Entity Component System Framework (ECS) specifically made for C# and Unity. Internal caching and blazing fast component access makes it second to none. Several design decisions have been made to work optimal in a garbage collected environment and to go easy on the garbage collector. Entitas comes with an optional code generator which radically reduces the amount of code you have to write and [makes your code read like well written prose.][clean-coders]

<p align="center">
    <a href="https://dev.windows.com">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/master/Readme/Images/csharp.png" alt="CSharp" height="64"></a>
    <a href="http://unity3d.com">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/master/Readme/Images/MadeForUnity.png" alt="Unity3d" height="64"></a>
    <a href="http://unity3d.com/unite/archive/2015">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/master/Readme/Images/UniteEurope2015.png" alt="Unite Europe 2015" height="64"></a>
    <a href="https://unite.unity.com/2016/europe">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/master/Readme/Images/UniteEurope2016.png" alt="Unite Europe 2016" height="64"></a>
    <a href="https://www.wooga.com">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/master/Readme/Images/wooga-logo.png" alt="Wooga" height="64"></a>
</p>

---

### **[» Download](#download-entitas)**
### **[» Ask a question][issues-new]**
### **[» Wiki and example projects][wiki]**
### **[» #madeWithEntitas][wiki-games-and-examples]**

---

Unity Unite Talks
=================

| Entity system architecture with Unity | ECS architecture with Unity by example |
|:-------------------------------------:|:--------------------------------------:|
| [![Unite Europe 2015][unite-europe-2015-video-thumbnail]][unite-europe-2015-video] | [![Unite Europe 2016][unite-europe-2016-video-thumbnail]][unite-europe-2016-video] |
| [» Open the slides on SlideShare: Unite Europe 2015](http://www.slideshare.net/sschmid/uniteeurope-2015) | [» Open the slides on SlideShare: Unite Europe 2016](http://www.slideshare.net/sschmid/uniteeurope-2016) |


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
    <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/master/Readme/Images/Entitas.Unity-MenuItems.png" alt="Entitas.Unity MenuItems" height="200"><br />
    <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/master/Readme/Images/Entitas.Unity.VisualDebugging-Entity.png" alt="Entitas.Unity.VisualDebugging Entity" width="400">
    <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/master/Readme/Images/Entitas.Unity.VisualDebugging-DebugSystems.png" alt="Entitas.Unity.VisualDebugging Systems" width="400">
</p>


Entitas deep dive
=================

[Read the wiki][wiki] or checkout the [example projects][wiki-example-projects] to see Entitas in action. These example projects illustrate how systems, groups, collectors and entities all play together seamlessly.


Download Entitas
================

Each release is published with zip files containing all source files you need.

[Show releases][releases]



Contributing to Entitas
=======================

The project is hosted on [GitHub][github-entitas] where you can [report issues][issues], fork the project and [submit pull requests][pulls].
Entitas is developed with [TDD (Test Driven Development)](https://en.wikipedia.org/wiki/Test-driven_development) and [nspec](http://nspec.org). New features are introduced following the [git-flow](https://github.com/nvie/gitflow) conventions.

Fork the repository, then run

```
$ git clone https://github.com/<username>/Entitas-CSharp.git
$ cd Entitas-CSharp
$ git branch master origin/master
$ git flow init -d
````

Open `Entitas.sln` and run the Tests project to ensure everything works as expected. Alternatively run the test script

```
$ ./Scripts/test
```

If you plan to make changes to the Entitas.Unity project, run
```
$ ./Scripts/update
```

This will copy all required Entitas source files to the Entitas.Unity project's `Library` folder. Entitas must be considered as a dependency. Any changes to Entitas source code within the `Library` folder in the Entitas.Unity project won't be committed and will be overwritten when running `update` again. Changes to Entitas must be done in the `Entitas.sln` project.

[Create a new ticket][issues-new] to let people know what you're working on and to encourage a discussion. Follow the git-flow conventions and create a new feature branch starting with the issue number:

```
$ git flow feature start <#issue-your-feature>
```

Write unit tests and make sure all the existing tests pass. If you have many commits please consider using [git rebase](https://git-scm.com/docs/git-rebase) to cleanup the commits. This can simplify reviewing the pull request.
Once you're happy with your changes open a [pull request][pulls] to your feature branch.


Thanks to
=========

Big shout out to [@mzaks][github-mzaks], [@cloudjubei][github-cloudjubei] and [@devboy][github-devboy] for endless hours of discussion and helping making Entitas awesome!


Maintainer(s)
=============

- [@sschmid][github-sschmid] | [@s_schmid][twitter-sschmid] | [@entitas_csharp][twitter-entitas_csharp]


Different language?
===================

Entitas is available in
- [C#](https://github.com/sschmid/Entitas-CSharp)
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


[clean-coders]: https://cleancoders.com "Clean Coders"

[wiki]: https://github.com/sschmid/Entitas-CSharp/wiki "Entitas Wiki"
[wiki-code-generator]: https://github.com/sschmid/Entitas-CSharp/wiki/Code-Generator "Wiki - Code Generator"
[wiki-overview]: https://github.com/sschmid/Entitas-CSharp/wiki/Overview "Wiki - Overview"
[wiki-unity-integration]: https://github.com/sschmid/Entitas-CSharp/wiki/Unity-integration "Wiki - Unity Integration"
[wiki-example-projects]: https://github.com/sschmid/Entitas-CSharp/wiki/Example-projects "Wiki - Example Projects"
[wiki-games-and-examples]: https://github.com/sschmid/Entitas-CSharp/wiki/Games-and-Examples "Wiki - Games and Examples #madeWithEntitas"

[unite-europe-2015-video-thumbnail]: https://raw.githubusercontent.com/sschmid/Entitas-CSharp/master/Readme/Images/UniteEurope2015-Video.png "Video: Watch the Entitas Talk at Unite Europe 2015"
[unite-europe-2015-video]: https://www.youtube.com/watch?v=1wvMXur19M4 "Video: Watch the Entitas Talk at Unite Europe 2015"
[unite-europe-2016-video-thumbnail]: https://raw.githubusercontent.com/sschmid/Entitas-CSharp/master/Readme/Images/UniteEurope2016-Video.png "Video: Watch the Entitas Talk at Unite Europe 2016"
[unite-europe-2016-video]: https://www.youtube.com/watch?v=lNTaC-JWmdI "Video: Watch the Entitas Talk at Unite Europe 2016"

[github-entitas]: https://github.com/sschmid/Entitas-CSharp "sschmid/Entitas-CSharp"
[releases]: https://github.com/sschmid/Entitas-CSharp/releases "Releases"
[issues]: https://github.com/sschmid/Entitas-CSharp/issues "Issues"
[pulls]: https://github.com/sschmid/Entitas-CSharp/pulls "Pull Requests"
[issues-new]: https://github.com/sschmid/Entitas-CSharp/issues/new "New issue"

[twitter-sschmid]: https://twitter.com/s_schmid "s_schmid on Twitter"
[twitter-entitas_csharp]: https://twitter.com/entitas_csharp "entitas_csharp on Twitter"

[github-sschmid]: https://github.com/sschmid "@sschmid"
[github-mzaks]: https://github.com/mzaks "@mzaks"
[github-cloudjubei]: https://github.com/cloudjubei "@cloudjubei"
[github-devboy]: https://github.com/devboy "@devboy"
