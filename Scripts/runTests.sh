#!/bin/sh
xbuild Tests/Tests.csproj /property:Configuration=Release /verbosity:minimal
if [ $? = 0 ]
then
	mono Tests/bin/Release/Tests.exe
else
	echo "ERROR: Could not compile!"
	exit 1
fi
