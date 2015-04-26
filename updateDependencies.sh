#!/bin/sh
./buildPackage.sh
if [ $? = 0 ]
then
	BIN_DIR="bin"
	LIBRARIES_DIR_NAME="Libraries"

	UNITY_CODE_GENERATOR_LIBRARIES_DIR="Entitas.Unity.CodeGenerator/Assets/${LIBRARIES_DIR_NAME}"
	UNITY_VISUAL_DEBUGGING_LIBRARIES_DIR="Entitas.Unity.VisualDebugging/Assets/${LIBRARIES_DIR_NAME}"
	TESTS_LIBRARIES_DIR="Tests/Libraries"

	echo "CLEAN **************************************************************************"
	rm -rfv $UNITY_CODE_GENERATOR_LIBRARIES_DIR
	rm -rfv $UNITY_VISUAL_DEBUGGING_LIBRARIES_DIR
	mkdir $UNITY_CODE_GENERATOR_LIBRARIES_DIR
	mkdir $UNITY_VISUAL_DEBUGGING_LIBRARIES_DIR

	echo "COPY ***************************************************************************"
	cp -rv "${BIN_DIR}/Entitas" $UNITY_CODE_GENERATOR_LIBRARIES_DIR
	cp -rv "${BIN_DIR}/Entitas.CodeGenerator" $UNITY_CODE_GENERATOR_LIBRARIES_DIR
	cp -rv "${BIN_DIR}/Entitas.Unity" $UNITY_CODE_GENERATOR_LIBRARIES_DIR

	cp -rv "${BIN_DIR}/Entitas" $UNITY_VISUAL_DEBUGGING_LIBRARIES_DIR
	cp -rv "${BIN_DIR}/Entitas.CodeGenerator" $UNITY_VISUAL_DEBUGGING_LIBRARIES_DIR
	cp -rv "${BIN_DIR}/Entitas.Unity" $UNITY_VISUAL_DEBUGGING_LIBRARIES_DIR
	cp -rv "${BIN_DIR}/Entitas.Unity.CodeGenerator" $UNITY_VISUAL_DEBUGGING_LIBRARIES_DIR

	cp -rv "${BIN_DIR}/Entitas.Unity" $TESTS_LIBRARIES_DIR
	cp -rv "${BIN_DIR}/Entitas.Unity.CodeGenerator" $TESTS_LIBRARIES_DIR
	cp -rv "${BIN_DIR}/Entitas.Unity.VisualDebugging" $TESTS_LIBRARIES_DIR

echo "DONE ***************************************************************************"
else
	echo "ERROR: Tests didn't pass!"
	exit 1
fi
