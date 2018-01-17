Contributing to Entitas
=======================

The project is hosted on [GitHub][github-entitas] where you can [report issues][issues], fork the project and [submit pull requests][pulls].
Entitas is developed with [TDD (Test Driven Development)](https://en.wikipedia.org/wiki/Test-driven_development) and [nspec](http://nspec.org). New features are introduced following the [git-flow](https://github.com/nvie/gitflow) conventions.

Fork the repository and setup git-flow

```
$ git clone https://github.com/<username>/Entitas-CSharp.git
$ cd Entitas-CSharp
$ git branch master origin/master
$ git flow init -d
````

Open `Entitas.sln` and run the Tests project as a console application to ensure everything works as expected. Alternatively run the tests script

```
$ ./Scripts/bee tests
```

To manually tests your changes in a Unity project, run
```
$ ./Scripts/bee build
$ ./Scripts/bee sync
```

This will build Entitas and copy all required assemblies to the Tests/Unity/VisualDebugging project's `Library` folder. Changes to Entitas must be done in the `Entitas.sln` project.

[Create a new ticket][issues-new] to let people know what you're working on and to encourage a discussion. Follow the git-flow conventions and create a new feature branch starting with `#` and the issue number:

```
$ git flow feature start <#123-your-feature>
```

Write and update unit tests and make sure all the existing tests pass. If you have many commits please consider using [git rebase](https://git-scm.com/docs/git-rebase) to cleanup the commits. This can simplify reviewing the pull request.
Once you're happy with your changes open a [pull request][pulls] to your feature branch. The default branch is `develop`. Don't create a [pull request][pulls] from master.

By submitting a pull request, you represent that you have the right to license your contribution to the community, and agree by submitting the patch that your contributions are licensed under the [Entitas license][license].

[github-entitas]: https://github.com/sschmid/Entitas-CSharp "sschmid/Entitas-CSharp"
[issues]: https://github.com/sschmid/Entitas-CSharp/issues "Issues"
[pulls]: https://github.com/sschmid/Entitas-CSharp/pulls "Pull Requests"
[issues-new]: https://github.com/sschmid/Entitas-CSharp/issues/new "New issue"
[license]: https://github.com/sschmid/Entitas-CSharp/blob/develop/LICENSE.txt "License"
