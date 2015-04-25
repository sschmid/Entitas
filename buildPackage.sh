#!/bin/sh
./runTests.sh
if [ $? = 0 ]
then
	WD=$(pwd)
	BIN_DIR="bin"
	TMP_DIR="${BIN_DIR}/tmp"

	echo "CLEAN **************************************************************************"
	rm -rfv $BIN_DIR

	echo "CREATE FOLDERS *****************************************************************"
	mkdir -v $BIN_DIR
	mkdir -v $TMP_DIR

	echo "COPY SOURCES *******************************************************************"
	cp -rv Entitas/Entitas $TMP_DIR
	cp -rv Entitas.CodeGenerator/Entitas.CodeGenerator $TMP_DIR
	cp -rv Entitas.Unity/Assets/Entitas.Unity $TMP_DIR
	cp -rv Entitas.Unity.CodeGenerator/Assets/Entitas.Unity.CodeGenerator $TMP_DIR
	cp -rv Entitas.Unity.VisualDebugging/Assets/Entitas.Unity.VisualDebugging $TMP_DIR

	cp RELEASE_NOTES.md ${TMP_DIR}/RELEASE_NOTES.md

	echo "DELETE GARBAGE *****************************************************************"
	cd $TMP_DIR
	find . -name "*.meta" -type f -delete
	find . -name "*.DS_Store" -type f -delete

	echo "CREATE ZIP ARCHIVE *************************************************************"
	cd $TMP_DIR
	zip -r ../Entitas.zip ./
	cd $WD

	echo "DELETE TEMP FOLDER *************************************************************"
	rm -rfv $TMP_DIR

	echo "DONE ***************************************************************************"
else
	echo "ERROR: Tests didn't pass!"
	exit 1
fi
