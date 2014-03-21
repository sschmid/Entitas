#!/bin/sh
xbuild Entitas.sln /verbosity:minimal
mono Tests/Libraries/NSpec/NSpecRunner.exe Tests/bin/Debug/Tests.dll
