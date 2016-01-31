#!/bin/sh
BIN_DIR="bin"
SRC_DIR="$BIN_DIR/Sources"

ES="Entitas"
CG="$ES.CodeGenerator"
ESU="$ES.Unity"
UCG="$ESU.CodeGenerator"
UVD="$ESU.VisualDebugging"
MIG="$ES.Migration"
UMIG="$ESU.Migration"

collect_sources() {
  echo "Collecting sources..."

  rm -rf $BIN_DIR
  mkdir $BIN_DIR $SRC_DIR

  cp -r {"$ES/$ES","$CG/$CG","$MIG/$MIG","$ESU/Assets/$ESU","$UCG/Assets/$UCG","$UVD/Assets/$UVD","$UMIG/Assets/$UMIG"} $SRC_DIR
  find "./$SRC_DIR" -name "*.meta" -type f -delete
  find "./$SRC_DIR" -name "*.DS_Store" -type f -delete

  echo "Collecting sources done."
}

update_project_dependencies() {
  echo "Updating project dependencies..."

  ESU_LIBS_DIR="$ESU/Assets/Libraries"
  UCODEGEN_LIBS_DIR="$UCG/Assets/Libraries"
  UVD_LIBS_DIR="$UVD/Assets/Libraries"
  UMIG_LIBS_DIR="$UMIG/Assets/Libraries"
  UNITY_TESTS_LIBS_DIR="UnityTests/Assets/Libraries"

  find $ESU_LIBS_DIR $UCODEGEN_LIBS_DIR $UVD_LIBS_DIR $UMIG_LIBS_DIR $UNITY_TESTS_LIBS_DIR -type f -name "*.cs" -delete

  cp -r $SRC_DIR/$ES $ESU_LIBS_DIR
  cp -r $SRC_DIR/{$ES,$CG,$ESU} $UCODEGEN_LIBS_DIR
  cp -r $SRC_DIR/{$ES,$CG,$ESU,$UCG} $UVD_LIBS_DIR
  cp -r $SRC_DIR/{$ES,$CG,$MIG,$ESU,$UCG,$UVD} $UMIG_LIBS_DIR
  cp -r $SRC_DIR/{$ES,$CG,$ESU,$UCG,$UVD} $UNITY_TESTS_LIBS_DIR

  echo "Updating project dependencies done."
}

generateProjectFiles() {
  echo "Generating project files..."
  PWD=$(pwd)
  # Unity bug: https://support.unity3d.com/hc/en-us/requests/36273
  # /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -logfile -projectPath "$PWD/$ESU" -executeMethod Commands.GenerateProjectFiles
  /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -logfile -projectPath $PWD/$UCG -executeMethod Commands.GenerateProjectFiles
  /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -logfile -projectPath $PWD/$UVD -executeMethod Commands.GenerateProjectFiles
  /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -logfile -projectPath $PWD/$UMIG -executeMethod Commands.GenerateProjectFiles
  echo "Generating project files done."
}

build() {
  echo "Building..."
  xbuild /target:Clean /property:Configuration=Release Entitas.sln
  xbuild /property:Configuration=Release Entitas.sln
  echo "Building done."
}

runTests() {
  mono Tests/Libraries/NSpec/NSpecRunner.exe Tests/bin/Release/Tests.dll
}

collect_misc_files() {
  echo "Collecting misc files..."

  cp "$MIG/bin/Release/Entitas.Migration.exe" "$SRC_DIR/MigrationAssistant.exe"
  cp README.md "$SRC_DIR/README.md"
  cp RELEASE_NOTES.md "$SRC_DIR/RELEASE_NOTES.md"
  cp EntitasUpgradeGuide.md "$SRC_DIR/EntitasUpgradeGuide.md"
  cp LICENSE.txt "$SRC_DIR/LICENSE.txt"

  icon_meta="$UVD/Editor/EntitasHierarchyIcon.png.meta"
  cp "$UVD/Assets/$icon_meta" "$SRC_DIR/$icon_meta"

  echo "Collecting misc files done."
}

create_zip() {
  echo "Creating zip files..."

  TMP_DIR="$BIN_DIR/tmp"

  echo "Creating Entitas-CSharp.zip..."
  mkdir $TMP_DIR
  cp -r {"$SRC_DIR/$ES","$SRC_DIR/$CG"} $TMP_DIR
  cp "$SRC_DIR/"* $TMP_DIR || true

  pushd $TMP_DIR > /dev/null
    zip -rq ../Entitas-CSharp.zip ./
  popd > /dev/null
  rm -rf $TMP_DIR

  echo "Creating Entitas-Unity.zip..."
  mkdir $TMP_DIR
  cp -r "$SRC_DIR/"* $TMP_DIR || true

  tmp_editor_dir="$TMP_DIR/Editor"
  mkdir $tmp_editor_dir
  mv "$TMP_DIR/$CG/"* $tmp_editor_dir
  mv {"$tmp_editor_dir/Attributes",$tmp_editor_dir} "$TMP_DIR/$CG"

  pushd $TMP_DIR > /dev/null
    zip -rq ../Entitas-Unity.zip ./
  popd > /dev/null
  rm -rf $TMP_DIR

  echo "Creating zip files done."
}
