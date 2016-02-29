#!/bin/sh
BIN_DIR="bin"
SRC_DIR="$BIN_DIR/Sources"

# csharp
ES="Entitas"
CG="Entitas.CodeGenerator"
MIG="Entitas.Migration"
CG_TR="Entitas.CodeGenerator.TypeReflection"
ESU="Entitas.Unity"

ESU_ASSETS="$ESU/Assets"

collect_sources() {
  echo "Collecting sources..."

  rm -rf $BIN_DIR
  mkdir $BIN_DIR $SRC_DIR

  cp -r {"$ES/$ES","$CG/$CG","$MIG/$MIG","$CG_TR/$CG_TR","$ESU_ASSETS/$ESU"} $SRC_DIR
  find "./$SRC_DIR" -name "*.meta" -type f -delete

  header_meta="$ESU/Editor/Entitas-Header.png.meta"
  cp "$ESU_ASSETS/$header_meta" "$SRC_DIR/$header_meta"

  icon_meta="$ESU/VisualDebugging/Editor/EntitasHierarchyIcon.png.meta"
  cp "$ESU_ASSETS/$icon_meta" "$SRC_DIR/$icon_meta"

  migration_header_meta="$ESU/Migration/Editor/Entitas-Migration-Header.png.meta"
  cp "$ESU_ASSETS/$migration_header_meta" "$SRC_DIR/$migration_header_meta"

  echo "Collecting sources done."
}

update_project_dependencies() {
  echo "Updating project dependencies..."

  ESU_LIBS_DIR="$ESU_ASSETS/Libraries"

  rm -rf $ESU_LIBS_DIR
  mkdir $ESU_LIBS_DIR

  cp -r $SRC_DIR/{$ES,$CG,$CG_TR,$MIG} $ESU_LIBS_DIR

  echo "Updating project dependencies done."
}

generateProjectFiles() {
  echo "Generating project files..."
  PWD=$(pwd)

  # Unity bug: https://support.unity3d.com/hc/en-us/requests/36273
  # /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -logfile -projectPath "$PWD/$ESU" -executeMethod Commands.GenerateProjectFiles

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

  mkdir $tmp_editor_dir
  mv "$TMP_DIR/$MIG/"* $tmp_editor_dir
  mv $tmp_editor_dir "$TMP_DIR/$MIG"

  mkdir $tmp_editor_dir
  mv "$TMP_DIR/$CG_TR/"* $tmp_editor_dir
  mv $tmp_editor_dir "$TMP_DIR/$CG_TR"

  pushd $TMP_DIR > /dev/null
    zip -rq ../Entitas-Unity.zip ./
  popd > /dev/null
  rm -rf $TMP_DIR

  echo "Creating zip files done."
}
