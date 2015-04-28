#!/bin/sh
./runTests.sh
if [ $? = 0 ]
then
	echo "*** BUILDING PACKAGE"

	BIN_DIR="bin"
	TMP_DIR=$BIN_DIR"/tmp"

	ES="Entitas"
	CG=$ES".CodeGenerator"
	ESU=$ES".Unity"
	UCG=$ESU".CodeGenerator"
	UVD=$ESU".VisualDebugging"

	echo "*** CLEAN"
	rm -rf $BIN_DIR

	echo "*** CREATE FOLDERS"
	mkdir $BIN_DIR
	mkdir $TMP_DIR

	echo "*** COPY SOURCES"
	cp -r {$ES"/"$ES,$CG"/"$CG,$ESU"/Assets/"$ESU,$UCG"/Assets/"$UCG,$UVD"/Assets/"$UVD} $TMP_DIR
	cp RELEASE_NOTES.md ${TMP_DIR}/RELEASE_NOTES.md
	cp README.md ${TMP_DIR}/README.md

	echo "*** DELETE GARBAGE"
	find "./"$TMP_DIR -name "*.meta" -type f -delete
	find "./"$TMP_DIR -name "*.DS_Store" -type f -delete

	echo "*** CREATE ZIP ARCHIVE"
	cd $TMP_DIR
	zip -rq ../Entitas.zip ./
	cd -

	echo "*** COPY TEMP TO BIN"
	cp -r $TMP_DIR"/." $BIN_DIR

	echo "*** DELETE TEMP FOLDER"
	rm -rf $TMP_DIR

	echo "*** DONE ***"
else
	echo "ERROR: Tests didn't pass!"
	exit 1
fi
