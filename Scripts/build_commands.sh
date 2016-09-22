#!/bin/sh
source Scripts/vars.sh

collect_sources() {
  echo "### Collecting sources... ============================================="

  rm -rfv $BUILD
  mkdir $BUILD $BUILD_SRC $BUILD_SRC/$ESU

  cpe {"$ES/$ES","$MIG/$MIG"} $BUILD_SRC
  cpe "$ESU_ASSETS/$ES/Unity/" $BUILD_SRC/$ESU

  echo "### Collecting sources done ==========================================="
}

collect_misc() {
  echo "### Collecting misc files... =========================================="

  # Meta files
  header_meta="Editor/EntitasHeader.png.meta"
  entity_icon_meta="Editor/EntitasEntityHierarchyIcon.png.meta"
  entityError_icon_meta="Editor/EntitasEntityErrorHierarchyIcon.png.meta"
  pool_icon_meta="Editor/EntitasPoolHierarchyIcon.png.meta"
  poolError_icon_meta="Editor/EntitasPoolErrorHierarchyIcon.png.meta"
  systems_icon_meta="Editor/EntitasSystemsHierarchyIcon.png.meta"
  migration_header_meta="Editor/EntitasMigrationHeader.png.meta"

  cp -v "$ESU_ASSETS/$ES/Unity/$header_meta" "$BUILD_SRC/$ESU/$header_meta"
  cp -v "$ESU_ASSETS/$ES/Unity/VisualDebugging/$entity_icon_meta" "$BUILD_SRC/$ESU/VisualDebugging/$entity_icon_meta"
  cp -v "$ESU_ASSETS/$ES/Unity/VisualDebugging/$entityError_icon_meta" "$BUILD_SRC/$ESU/VisualDebugging/$entityError_icon_meta"
  cp -v "$ESU_ASSETS/$ES/Unity/VisualDebugging/$pool_icon_meta" "$BUILD_SRC/$ESU/VisualDebugging/$pool_icon_meta"
  cp -v "$ESU_ASSETS/$ES/Unity/VisualDebugging/$poolError_icon_meta" "$BUILD_SRC/$ESU/VisualDebugging/$poolError_icon_meta"
  cp -v "$ESU_ASSETS/$ES/Unity/VisualDebugging/$systems_icon_meta" "$BUILD_SRC/$ESU/VisualDebugging/$systems_icon_meta"
  cp -v "$ESU_ASSETS/$ES/Unity/Migration/$migration_header_meta" "$BUILD_SRC/$ESU/Migration/$migration_header_meta"

  echo "### Collecting misc files done ========================================"
}

post_build_collect_misc() {
  echo "Collecting misc files... =============================================="

  cp -v "$MIG/bin/Release/Entitas.Migration.exe" "$BUILD_SRC/MigrationAssistant.exe"
  cp -v README.md "$BUILD_SRC/README.md"
  cp -v RELEASE_NOTES.md "$BUILD_SRC/RELEASE_NOTES.md"
  cp -v EntitasUpgradeGuide.md "$BUILD_SRC/EntitasUpgradeGuide.md"
  cp -v LICENSE.txt "$BUILD_SRC/LICENSE.txt"
  rsync -arv Documentation "$BUILD_SRC/"

  echo "Collecting misc files done ============================================"
}

update_project_dependencies() {
  echo "Updating project dependencies... ======================================"

  rm -rf $ESU_LIBS
  mkdir $ESU_LIBS
  rsync -arv $BUILD_SRC/{$ES,$MIG} $ESU_LIBS

  echo "Updating project dependencies done ===================================="
}

generateProjectFiles() {
  echo "Generating project files... ==========================================="

  # Unity bug: https://support.unity3d.com/hc/en-us/requests/36273
  # Fixed in 5.3.4p1
  $UNITY -quit -batchmode -logfile -projectPath "$PWD/$ESU_ASSETS/../" -executeMethod Commands.GenerateProjectFiles

  echo "Generating project files done ========================================="
}

build() {
  echo "Building... ==========================================================="
  xbuild /target:Clean /property:Configuration=Release $BUILD_PROJECT
  xbuild /property:Configuration=Release $BUILD_PROJECT
  echo "Building done ========================================================="
}

runTests() {
  mono $TEST_RUNNER
}

create_zip() {
  echo "Creating zip files... ================================================="

  TMP_DIR="$BUILD/tmp"

  echo "Creating Entitas-CSharp.zip..."
  mkdir $TMP_DIR
  cp -r $BUILD_SRC/$ES $TMP_DIR
  cp -r "$BUILD_SRC/Documentation" $TMP_DIR
  cp "$BUILD_SRC/"* $TMP_DIR || true

  pushd $TMP_DIR > /dev/null
    zip -rq ../Entitas-CSharp.zip ./
  popd > /dev/null
  rm -rf $TMP_DIR

  echo "Creating Entitas-Unity.zip..."
  mkdir $TMP_DIR
  cp -r "$BUILD_SRC/"* $TMP_DIR || true

  tmp_editor_dir="$TMP_DIR/Editor"
  mkdir $tmp_editor_dir
  mv "$TMP_DIR/Entitas/CodeGenerator/"* $tmp_editor_dir
  mv {"$tmp_editor_dir/Attributes",$tmp_editor_dir} "$TMP_DIR/Entitas/CodeGenerator"

  mkdir "$TMP_DIR/$ES/Migration/"
  mv "$TMP_DIR/$MIG/" "$TMP_DIR/$ES/Migration/Editor/"

  mv "$TMP_DIR/$ESU/" "$TMP_DIR/$ES/Unity/"

  pushd $TMP_DIR > /dev/null
    zip -rq ../Entitas-Unity.zip ./
  popd > /dev/null
  rm -rf $TMP_DIR

  echo "Creating zip files done ==============================================="
}

create_tree_overview() {
  echo "Creating tree overview... ============================================="
  tree -I 'bin|obj|Library|Libraries|*Tests|Readme|ProjectSettings|Temp|Examples|*.csproj|*.meta|*.sln|*.userprefs|*.properties' --noreport -d > tree.txt
  tree -I 'bin|obj|Library|Libraries|*Tests|Readme|ProjectSettings|Temp|Examples|*.csproj|*.meta|*.sln|*.userprefs|*.properties' --noreport --dirsfirst >> tree.txt
  cat tree.txt
  echo "Creating tree overview done ==========================================="
}
