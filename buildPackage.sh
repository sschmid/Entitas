#!/bin/sh
./runTests.sh
if [ $? = 0 ]
then
	echo "Build package"

	BIN_DIR="bin"
	TMP_DIR=$BIN_DIR"/tmp"

	ES="Entitas"
	CG=$ES".CodeGenerator"
	ESU=$ES".Unity"
	UCG=$ESU".CodeGenerator"
	UVD=$ESU".VisualDebugging"

	echo "  Clean bin"
	rm -rf $BIN_DIR

	echo "  Create folder structure"
	mkdir $BIN_DIR
	mkdir $TMP_DIR

	echo "  Copy sources to temp"
	cp -r {$ES"/"$ES,$CG"/"$CG,$ESU"/Assets/"$ESU,$UCG"/Assets/"$UCG,$UVD"/Assets/"$UVD} $TMP_DIR
	cp RELEASE_NOTES.md ${TMP_DIR}/RELEASE_NOTES.md
	cp README.md ${TMP_DIR}/README.md

	echo "  Remove ignored files"
	find "./"$TMP_DIR -name "*.meta" -type f -delete
	find "./"$TMP_DIR -name "*.DS_Store" -type f -delete

	echo "  Create zip archive"
	cd $TMP_DIR
	zip -rq ../Entitas.zip ./
	cd -

	echo "  Copy temp to bin"
	cp -r $TMP_DIR"/." $BIN_DIR

	echo "  Delete temp"
	rm -rf $TMP_DIR

	echo "Done."
else
	echo "ERROR: Tests didn't pass!"
	exit 1
fi
