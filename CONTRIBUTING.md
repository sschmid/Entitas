Contributing to Entitas
=======================

The project is hosted on [GitHub][repo] where you can [report issues][issues], fork the project and [submit pull requests][pulls].
Entitas is developed with [TDD (Test Driven Development)](https://en.wikipedia.org/wiki/Test-driven_development) and [nspec](http://nspec.org).
New features are introduced following the [git-flow](https://github.com/nvie/gitflow) conventions.

Setup Entitas on your machine
=============================

Fork the repository on [GitHub][repo] and clone your forked repository to your machine

```
$ git clone https://github.com/<username>/Entitas.git
````

If you want to contribute please consider to set up [git-flow](https://github.com/nvie/gitflow).
The default branch of this repository is `master`

````
$ cd Entitas
$ git branch master origin/master
$ git flow init -d
````

Open `Entitas.sln` and run the Tests project as a console application to ensure everything works as expected. Alternatively run the tests script

```
$ ./Scripts/bee tests
```

Make changes
============

[Create a new ticket][issues-new] to let people know what you're working on and to encourage a discussion. Follow the git-flow conventions and create a new feature branch starting with `#` and the issue number:

```
$ git flow feature start <#123-your-feature>
```

Write and update unit tests and make sure all the existing tests pass. To manually test your changes in a Unity project, run

```
$ ./Scripts/bee build
$ ./Scripts/bee sync
```

This will build Entitas with all your changes and copy all required assemblies to the Tests/Unity/VisualDebugging project's `Library` folder. You can open the Tests/Unity/VisualDebugging project in Unity and verify and test your changes manually. All changes to Entitas must be done in the `Entitas.sln` project.


Contribute
==========

If you have many commits please consider using [git rebase](https://git-scm.com/docs/git-rebase) to cleanup the commits. This can simplify reviewing the pull request.
Once you're happy with your changes open a [pull request][pulls] to your feature branch. The default branch is `develop`. Don't create a [pull request][pulls] from master.

By submitting a pull request, you represent that you have the right to license your contribution to the community, and agree by submitting the patch that your contributions are licensed under the [Entitas license][license].

Thanks for your contributions and happy coding :)

Simon

[repo]: https://github.com/sschmid/Entitas "sschmid/Entitas"
[issues]: https://github.com/sschmid/Entitas/issues "Issues"
[pulls]: https://github.com/sschmid/Entitas/pulls "Pull Requests"
[issues-new]: https://github.com/sschmid/Entitas/issues/new "New issue"
[license]: https://github.com/sschmid/Entitas/blob/develop/LICENSE.txt "License"
