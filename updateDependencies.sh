#!/bin/sh
./buildPackage.sh
if [ $? = 0 ]
then
	echo "Update dependencies"

	BIN_DIR="bin"

	ES="Entitas"
	CG=$ES".CodeGenerator"
	ESU=$ES".Unity"
	UCG=$ESU".CodeGenerator"
	UVD=$ESU".VisualDebugging"

	ESU_LIBS_DIR=$ESU"/Assets/Libraries"
	UCODEGEN_LIBS_DIR=$UCG"/Assets/Libraries"
	UVD_LIBS_DIR=$UVD"/Assets/Libraries"
	TESTS_LIBS_DIR="Tests/Libraries"

	echo "  Clean target folders"
	find "./"$ESU_LIBS_DIR -type f -name "*.cs" -delete
	find "./"$UCODEGEN_LIBS_DIR -type f -name "*.cs" -delete
	find "./"$UVD_LIBS_DIR -type f -name "*.cs" -delete
	rm -rf $TESTS_LIBS_DIR"/"{$ESU,$UCG,$UVD}

	echo "  Copy sources"
	cp -r $BIN_DIR"/"$ES $ESU_LIBS_DIR
	cp -r $BIN_DIR"/"{$ES,$CG,$ESU} $UCODEGEN_LIBS_DIR
	cp -r $BIN_DIR"/"{$ES,$CG,$ESU,$UCG} $UVD_LIBS_DIR
	cp -r $BIN_DIR"/"{$ESU,$UCG,$UVD} $TESTS_LIBS_DIR

	echo "Done."
else
	echo "ERROR: Tests didn't pass!"
	exit 1
fi
