#!/bin/sh
xbuild Entitas.sln /verbosity:minimal
if [ $? = 0 ]
then
	mono Tests/Libraries/nspec/NSpecRunner.exe Tests/bin/Debug/Tests.dll
else
	echo "WARNING: Could not compile!"
fi