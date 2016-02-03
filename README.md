<p align="center">
    <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/develop/Readme/Images/Entitas-Header.png" alt="Entitas">
</p>

<p align="center">
    <a href="https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=Y9HGYPFMLG2W4">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/develop/Readme/Images/Donate-PayPal.gif" alt="Thank you!"></a>

    <a href="https://gitter.im/sschmid/Entitas-CSharp?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge">
        <img src="https://img.shields.io/badge/gitter-join%20chat-brightgreen.svg" alt="Join the chat at https://gitter.im/sschmid/Entitas-CSharp"></a>

    <a href="https://twitter.com/intent/follow?original_referer=https%3A%2F%2Fgithub.com%2Fsschmid%2FEntitas-CSharp&screen_name=s_schmid&tw_p=followbutton">
        <img src="https://img.shields.io/badge/twitter-follow%20%40s__schmid-blue.svg" alt="Twitter Follow Me"></a>

    <a href="https://twitter.com/intent/follow?original_referer=https%3A%2F%2Fgithub.com%2Fsschmid%2FEntitas-CSharp&screen_name=entitas_csharp&tw_p=followbutton">
        <img src="https://img.shields.io/badge/twitter-follow%20%40entitas__csharp-blue.svg" alt="Twitter Follow Me"></a>

    <a href="https://travis-ci.org/sschmid/Entitas-CSharp">
        <img src="https://travis-ci.org/sschmid/Entitas-CSharp.svg?branch=master" alt="Build Status"></a>

    <a href="https://github.com/sschmid/Entitas-CSharp/releases">
        <img src="https://img.shields.io/github/release/sschmid/Entitas-CSharp.svg" alt="Latest release"></a>
</p>


The Entity Component System for C# and Unity
============================================

Entitas is a super fast Entity Component System (ECS) specifically made for C# and Unity. Internal caching and blazing fast component access makes it second to none. Several design decisions have been made to work optimal in a garbage collected environment and to go easy on the garbage collector. Entitas comes with an optional code generator which radically reduces the amount of code you have to write and [makes your code read like well written prose.][clean-coders]

<p align="left">
    <a href="https://dev.windows.com">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/develop/Readme/Images/csharp.png" alt="CSharp" height="64"></a>
    <a href="http://unity3d.com">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/develop/Readme/Images/WorksWithUnity.png" alt="Unity3d" height="64"></a>
    <a href="http://unity3d.com/unite/archive/2015">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/develop/Readme/Images/UniteEurope2015.png" alt="Unite Europe 2015" height="64"></a>
    <a href="http://unity3d.com/unite/archive/2015">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/develop/Readme/Images/UniteBoston2015.png" alt="Unite Boston 2015" height="64"></a>
    <a href="https://www.wooga.com">
        <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/develop/Readme/Images/wooga-logo.png" alt="Wooga" height="64"></a>
</p>

---

### **[» Wiki, Overview, Roadmap and example projects][wiki]**
### **[» Community: Games and Examples #madeWithEntitas][wiki-games-and-examples]**


Watch the talk from Unite Europe 2015
=====================================
<a href="http://www.slideshare.net/sschmid/uniteeurope-2015" target="_blank">» Open the slides on SlideShare</a>

[![UniteEurope 2015][unite-europe-2015-video-thumbnail]][unite-europe-2015-video]


First glimpse
=============

The optional [code generator][wiki-code-generator] lets you write code that is super fast, safe and literally screams its intent.

```csharp
public static Entity CreateRedGem(this Pool pool, int x, int y) {
    return pool.CreateEntity()
               .IsGameBoardElement(true)
               .IsMovable(true)
               .AddPosition(x, y)
               .AddResource(Res.redGem)
               .IsInteractive(true);
}
```

```csharp
var entities = pool.GetEntities(Matcher.AllOf(Matcher.Move, Matcher.Position));
foreach (var entity in entities) {
    var move = entity.move;
    var pos = entity.position;
    entity.ReplacePosition(pos.x, pos.y + move.speed);
}
```


Overview
========

Entitas is fast, light and gets rid of unnecessary complexity. There are less than a handful classes you have to know to rocket start your game or application:

- Entity
- Pool
- Group
- Group Observer

[Read more...][wiki-overview]


Code Generator
==============

The Code Generator generates classes and methods for you, so you can focus on getting the job done. It radically reduces the amount of code you have to write and improves readability by a huge magnitude. It makes your code less error-prone while ensuring best performance. I strongly recommend using it!

[Read more...][wiki-code-generator]


Unity integration
=================

The optional Unity module integrates Entitas nicely into Unity and provides powerful editor extensions to inspect and debug pools, groups, entities, components and systems.

[Read more...][wiki-unity-integration]

<p align="center">
    <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/develop/Readme/Images/Entitas.Unity-MenuItems.png" alt="Entitas.Unity MenuItems">
    <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/develop/Readme/Images/Entitas.Unity.VisualDebugging-Entity.png" alt="Entitas.Unity.VisualDebugging Entity">
    <img src="https://raw.githubusercontent.com/sschmid/Entitas-CSharp/develop/Readme/Images/Entitas.Unity.VisualDebugging-DebugSystems.png" alt="Entitas.Unity.VisualDebugging Systems">
</p>


Entitas deep dive
=================

[Read the wiki][wiki] or checkout the awesome [example projects][wiki-example-projects] to see Entitas in action. These example projects illustrate how systems, groups, observers and entities all play together seamlessly.


Download Entitas
================

Each release is published with zip files attached containing all source files you need.

[**Entitas-CSharp.zip**][entitas-csharp-zip]
- Entitas
- Entitas.CodeGenerator

[**Entitas-Unity.zip**][entitas-unity-zip]
- Entitas
- Entitas.CodeGenerator
- Entitas.Unity
- Entitas.Unity.CodeGenerator
- Entitas.Unity.VisualDebugging

[Show releases][releases]


Contributing to Entitas
=======================

The project is hosted on [GitHub][github-entitas] where you can [report issues][issues], fork the project and [submit pull requests][pulls].

Entitas.sln contains all projects and tests in one solution. Run build.sh to copy all required Entitas source files to all Unity projects.

To run the tests, navigate to the project root folder and execute runTests.sh.

- Check the [issues][issues] to make sure nobody hasn't already requested it and/or contributed it
- Fork the project
- Checkout the latest develop
- Start a feature/yourFeatureOrBugfix branch based on the latest develop
- Make sure to add/update tests. This is important so nobody will break it in a future version. Please write tests first, followed by the implementation.
- Commit and push until you are happy with your contribution
- Create a [pull request][pulls]


Thanks to
=========

Big shout out to [@mzaks][github-mzaks], [@cloudjubei][github-cloudjubei] and [@devboy][github-devboy] for endless hours of discussion and helping making Entitas awesome!


Maintainer(s)
=============

- [@sschmid][github-sschmid] | [@s_schmid][twitter-sschmid] | [@entitas_csharp][twitter-entitas_csharp]


Different language?
===================

Entitas is also available in
- [Swift](https://github.com/mzaks/Entitas-Swift)
- [Objective-C](https://github.com/wooga/entitas)
- [Go](https://github.com/wooga/go-entitas)
- [Clojure](https://github.com/mhaemmerle/entitas-clj)
- [Haskell](https://github.com/mhaemmerle/entitas-haskell)
- [Erlang](https://github.com/mhaemmerle/entitas_erl)


[clean-coders]: https://cleancoders.com "Clean Coders"
[entitas-csharp-zip]: https://github.com/sschmid/Entitas-CSharp/blob/master/bin/Entitas-CSharp.zip?raw=true "Download Entitas-CSharp.zip"
[entitas-unity-zip]: https://github.com/sschmid/Entitas-CSharp/blob/master/bin/Entitas-Unity.zip?raw=true "Download Entitas-Unity.zip"

[wiki]: https://github.com/sschmid/Entitas-CSharp/wiki "Entitas Wiki"
[wiki-code-generator]: https://github.com/sschmid/Entitas-CSharp/wiki/Code-Generator "Wiki - Code Generator"
[wiki-overview]: https://github.com/sschmid/Entitas-CSharp/wiki/Overview "Wiki - Overview"
[wiki-unity-integration]: https://github.com/sschmid/Entitas-CSharp/wiki/Unity-integration "Wiki - Unity Integration"
[wiki-example-projects]: https://github.com/sschmid/Entitas-CSharp/wiki/Example-projects "Wiki - Example Projects"
[wiki-games-and-examples]: https://github.com/sschmid/Entitas-CSharp/wiki/Games-and-Examples "Wiki - Games and Examples #madeWithEntitas"

[unite-europe-2015-video-thumbnail]: https://raw.githubusercontent.com/sschmid/Entitas-CSharp/develop/Readme/Images/UniteEurope2015-Video.png "Video: Watch the Entitas Talk at Unite Europe 2015"
[unite-europe-2015-video]: http://buff.ly/1KtKlm6 "Video: Watch the Entitas Talk at Unite Europe 2015"

[github-entitas]: https://github.com/sschmid/Entitas-CSharp "sschmid/Entitas-CSharp"
[releases]: https://github.com/sschmid/Entitas-CSharp/releases "Releases"
[issues]: https://github.com/sschmid/Entitas-CSharp/issues "Issues"
[pulls]: https://github.com/sschmid/Entitas-CSharp/pulls "Pull Requests"

[twitter-sschmid]: https://twitter.com/s_schmid "s_schmid on Twitter"
[twitter-entitas_csharp]: https://twitter.com/entitas_csharp "entitas_csharp on Twitter"

[github-sschmid]: https://github.com/sschmid "@sschmid"
[github-mzaks]: https://github.com/mzaks "@mzaks"
[github-cloudjubei]: https://github.com/cloudjubei "@cloudjubei"
[github-devboy]: https://github.com/devboy "@devboy"
