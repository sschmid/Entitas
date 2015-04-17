#!/bin/sh
./runTests.sh
if [ $? = 0 ]
then
	rm -rfv bin
	mkdir -v bin
	mkdir -v bin/tmp

	cp -rv Entitas/Entitas bin/tmp
	cp -rv EntitasCodeGenerator/EntitasCodeGenerator bin/tmp
	mkdir -v bin/tmp/EntitasUnity
	cp -rv EntitasUnity/CodeGenerator bin/tmp/EntitasUnity/
	cp -rv EntitasUnity/VisualDebugging/Assets/VisualDebugging bin/tmp/EntitasUnity/

	cp RELEASE_NOTES.md bin/tmp/RELEASE_NOTES.md

	cd bin/tmp
	zip -r ../Entitas.zip ./
	cd ..
	rm -rfv tmp

	find . -name "*.DS_Store" -type f -delete
else
	echo "ERROR: Tests didn't pass!"
	exit 1
fi
