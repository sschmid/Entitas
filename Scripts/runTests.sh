#!/bin/bash
source Scripts/vars.sh

echo `pwd`

echo "Building $TEST_PROJECT"

xbuild $TEST_PROJECT /property:Configuration=Release /verbosity:minimal
if [ $? = 0 ]
then
	echo "Running tests $TEST_RUNNER"
	mono $TEST_RUNNER
else
	echo "ERROR: Could not compile!"
	exit 1
fi
