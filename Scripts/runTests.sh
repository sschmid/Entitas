#!/bin/sh
source Scripts/vars.sh
xbuild $TEST_PROJECT /property:Configuration=Release /verbosity:minimal
if [ $? = 0 ]
then
	mono $TEST_RUNNER
else
	echo "ERROR: Could not compile!"
	exit 1
fi
