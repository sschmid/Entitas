: "${BUILD:=build}"

BUILD_SRC="${BUILD}/src"
BUILD_DIST="${BUILD}/dist"

entitas::help() {
  cat << 'EOF'
template:

usage:
  clean                           delete build directory and all bin and obj directories
  build                           build solution
  rebuild                         clean and build solution
  test [args]                     run unit tests
  generate                        generate code for all projects
  pack                            pack Entitas and Jenny
  zip                             create Entitas.zip and Jenny.zip
  restore_unity_visualdebugging   copy source code and samples to all unity projects

EOF
}

entitas::clean() {
  find . -type d -name obj -exec rm -rf {} +
  find . -type d -name bin -exec rm -rf {} +
  rm -rf "${BUILD}"
}

entitas::build() {
  dotnet build -c Release
}

entitas::rebuild() {
  entitas::clean
  dotnet build -c Release
}

entitas::test() {
  dotnet run --project Tests/Tests/Tests.csproj
}

entitas::generate() {
  local properties=(
    'Tests/TestFixtures/Preferences.properties'
    'Readme/Prefrences.properties'
  )
  local dir current_dir
  current_dir="$(pwd)"
  for p in "${properties[@]}"; do
    dir="$(dirname "${p}")"
    pushd "${dir}" > /dev/null || exit
      bee::log_info "Generating ${p}"
      "${current_dir}/${DESPERATEDEVS_DIR}/Jenny/Jenny/Jenny" gen "$(basename "${p}")"
    popd > /dev/null || exit
  done
}

entitas::pack() {
  entitas::build
  local project_dir="${BUILD_SRC}/Unity/Assets"
  local jenny_dir="${BUILD_SRC}/Unity/Jenny"
  local entitas_dir="${project_dir}/Entitas"
  local entitas_editor_dir="${entitas_dir}/Editor"
  local entitas_jenny_dir="${jenny_dir}/Jenny/Plugins/Entitas"
  local entitas_images_dir="${entitas_editor_dir}/Images"
  _clean_dir "${project_dir}" "${jenny_dir}" "${entitas_dir}" "${entitas_editor_dir}" "${entitas_jenny_dir}" "${entitas_images_dir}" 

  _sync "${DESPERATEDEVS_DIR}/Unity/Assets/" "${project_dir}"
  _sync "${DESPERATEDEVS_DIR}/Jenny/" "${jenny_dir}"

  local -a projects=(
    Entitas
#    Addons/Entitas.Blueprints
#    Addons/Entitas.Blueprints.Unity
    Addons/Entitas.CodeGeneration.Attributes
    Addons/Entitas.Unity
    Addons/Entitas.VisualDebugging.Unity
    
    # editor
#    Addons/Entitas.Blueprints.Unity.Editor
    Addons/Entitas.Migration
    Addons/Entitas.Migration.Unity.Editor
    Addons/Entitas.Unity.Editor
    Addons/Entitas.VisualDebugging.Unity.Editor
    
    # plugins
#    Addons/Entitas.Blueprints.CodeGeneration.Plugins
#    Addons/Entitas.Blueprints.CodeGeneration.Unity.Plugins
    Addons/Entitas.CodeGeneration.Plugins
    Addons/Entitas.Roslyn.CodeGeneration.Plugins
    Addons/Entitas.VisualDebugging.CodeGeneration.Plugins
  )

  local -a to_editor=(
#    Entitas.Blueprints.Unity.Editor
    Entitas.Migration
    Entitas.Migration.Unity.Editor
    Entitas.Unity.Editor
    Entitas.VisualDebugging.Unity.Editor
  )

  local -a to_plugins=(
#    Entitas.Blueprints.CodeGeneration.Plugins
#    Entitas.Blueprints.CodeGeneration.Unity.Plugins
    Entitas.CodeGeneration.Plugins
    Entitas.Roslyn.CodeGeneration.Plugins
    Entitas.VisualDebugging.CodeGeneration.Plugins
  )

  local -a images=(
    Addons/Entitas.Unity.Editor/Entitas.Unity.Editor/Images/
    Addons/Entitas.VisualDebugging.Unity.Editor/Entitas.VisualDebugging.Unity.Editor/Images/
  )

  local -a files=(
    EntitasUpgradeGuide.md
    LICENSE.txt
    README.md
    CHANGELOG.md
  )
    
  for p in "${projects[@]}"; do _sync "${p}/bin/Release/" "${entitas_dir}"; done
  for f in "${to_editor[@]}"; do mv "${entitas_dir}/${f}.dll" "${entitas_editor_dir}"; done
  for f in "${to_plugins[@]}"; do mv "${entitas_dir}/${f}.dll" "${entitas_jenny_dir}"; done
  for f in "${images[@]}"; do _sync "${f}" "${entitas_images_dir}"; done
  for f in "${files[@]}"; do _sync "${f}" "${entitas_dir}"; done
}

entitas::restore_unity_visualdebugging() {
  entitas::pack
  local project_dir="Tests/Unity/VisualDebugging"
  local asset_dir="${project_dir}/Assets/Entitas"
  local jenny_dir="${project_dir}/Jenny"
  _clean_dir "${asset_dir}" "${jenny_dir}"
  _sync "${BUILD_SRC}/Unity/Assets/" "${asset_dir}"
  _sync "${BUILD_SRC}/Unity/Jenny/" "${jenny_dir}"
}

entitas::zip() {
  entitas::pack
  local abs_build_dist
  local project_dir="${BUILD_SRC}/Unity/Assets"
  local jenny_dir="${BUILD_SRC}/Unity/Jenny"
  mkdir -p "${BUILD_DIST}"
  pushd "${BUILD_DIST}" > /dev/null || exit
    abs_build_dist="$(pwd)"
  popd > /dev/null || exit
  pushd "${project_dir}" > /dev/null || exit
    zip -rq "${abs_build_dist}/Entitas.zip" ./
  popd > /dev/null || exit
  pushd "${jenny_dir}" > /dev/null || exit
    zip -rq "${abs_build_dist}/Jenny.zip" ./
  popd > /dev/null || exit
}

_clean_dir() {
  rm -rf "$@"
  mkdir -p "$@"
}

_sync() {
  rsync \
    --archive \
    --recursive \
    --prune-empty-dirs \
    --include-from "${BEE_RESOURCES}/entitas/rsync_include.txt" \
    --exclude-from "${BEE_RESOURCES}/entitas/rsync_exclude.txt" \
    "$@"
}

_sync_unity() {
  rsync \
    --archive \
    --recursive \
    --prune-empty-dirs \
    --exclude-from "${BEE_RESOURCES}/entitas/rsync_exclude_unity.txt" \
    "$@"
}
