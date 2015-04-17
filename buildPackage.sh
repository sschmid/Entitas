#!/bin/sh
./runTests.sh
if [ $? = 0 ]
then
	WD=$(pwd)
	BIN_DIR="bin"
	TMP_DIR="${BIN_DIR}/tmp"
	TMP_UNITY_DIR="${TMP_DIR}/EntitasUnity"

	echo "CLEAN **************************************************************************"
	rm -rfv $BIN_DIR

	echo "CREATE FOLDERS *****************************************************************"
	mkdir -v $BIN_DIR
	mkdir -v $TMP_DIR
	mkdir -v $TMP_UNITY_DIR

	echo "COPY SOURCES *******************************************************************"
	cp -rv Entitas/Entitas $TMP_DIR
	cp -rv EntitasCodeGenerator/EntitasCodeGenerator $TMP_DIR
	cp -rv EntitasUnity/CodeGenerator $TMP_UNITY_DIR
	cp -rv EntitasUnity/VisualDebugging/Assets/VisualDebugging $TMP_UNITY_DIR

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
