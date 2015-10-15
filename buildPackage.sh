#!/bin/sh
clean() {
	echo "  Clean bin"
	rm -rf $BIN_DIR
}

create_folder_structure() {
	echo "  Create folder structure"
	mkdir $BIN_DIR
	mkdir $TMP_DIR
}

copy_files() {
	echo "  Copy sources to temp"
	cp -r {$ES/$ES,$CG/$CG,$ESU/Assets/$ESU,$UCG/Assets/$UCG,$UVD/Assets/$UVD} $TMP_DIR
	cp $MIG/bin/Release/Entitas.Migration.exe $TMP_DIR/MigrationAssistant.exe
	cp README.md $TMP_DIR/README.md
	cp RELEASE_NOTES.md $TMP_DIR/RELEASE_NOTES.md
	cp EntitasUpgradeGuide.md $TMP_DIR/EntitasUpgradeGuide.md
}

remove_ignored_files() {
	echo "  Remove ignored files"
	find ./$TMP_DIR -name "*.meta" -type f -delete
	find ./$TMP_DIR -name "*.DS_Store" -type f -delete
}

copy_required_meta_files() {
	echo "  Copy required meta files"
	icon_meta=$UVD/Editor/EntitasHierarchyIcon.png.meta
	cp $UVD/Assets/$icon_meta $TMP_DIR/$icon_meta
}

package_csharp() {
	echo "  Create Entitas-CSharp.zip"

	package_dir=$BIN_DIR/package
	mkdir $package_dir
	cp -r {$TMP_DIR/$ES,$TMP_DIR/$CG} $package_dir
	cp $TMP_DIR/* $package_dir

	pushd $package_dir > /dev/null
		zip -rq ../Entitas-CSharp.zip ./
	popd > /dev/null
	rm -rf $package_dir
}

package_unity() {
	echo "  Create Entitas-Unity.zip"

	package_dir=$BIN_DIR/package
	mkdir $package_dir
	cp -r $TMP_DIR/* $package_dir

	editor_dir=$BIN_DIR/$CG/Editor
	mkdir -p $editor_dir
	mv $package_dir/$CG/* $editor_dir
	mv $editor_dir/Attributes $BIN_DIR/$CG
	mv $BIN_DIR/$CG $package_dir

	pushd $package_dir > /dev/null
		zip -rq ../Entitas-Unity.zip ./
	popd > /dev/null
	rm -rf $package_dir
}

cleanup() {
	echo "  Delete temp"
	cp -r $TMP_DIR/. $BIN_DIR
	rm -rf $TMP_DIR
}

./runTests.sh
if [ $? = 0 ]
then
	BIN_DIR=bin
	TMP_DIR=$BIN_DIR/tmp

	ES="Entitas"
	CG=$ES".CodeGenerator"
	ESU=$ES".Unity"
	UCG=$ESU".CodeGenerator"
	UVD=$ESU".VisualDebugging"
	MIG=$ES."Migration"

	echo "Build package"

clean
	create_folder_structure
	copy_files
	remove_ignored_files
	copy_required_meta_files
	package_csharp
	package_unity

cleanup

	echo "Done."
else
	echo "ERROR: Tests didn't pass!"
	exit 1
fi
