#!/usr/bin/env bash

BUILD="Build"
BUILD_SRC="${BUILD}/src"
BUILD_FILES="${BUILD_SRC}/files"
BUILD_DIST="${BUILD}/dist"

DEPS_DIR="Libraries/Dependencies/DesperateDevs"
DEPS=(
  "../DesperateDevs/DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor/bin/Release/"
  "../DesperateDevs/DesperateDevs.CodeGeneration.Plugins/bin/Release/"
  "../DesperateDevs/DesperateDevs.CodeGeneration.Unity.Plugins/bin/Release/"
  "../DesperateDevs/DesperateDevs.Unity.Editor/bin/Release/"
  "../DesperateDevs/DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor/Compile.cs"
)
ENTITAS_PROJECTS=(
  'Entitas'

  'Addons/Entitas.CodeGeneration.Attributes'
  'Addons/Entitas.CodeGeneration.Plugins'

  'Addons/Entitas.Migration'
  'Addons/Entitas.Migration.Unity.Editor'

  'Addons/Entitas.Unity'
  'Addons/Entitas.Unity.Editor'

  'Addons/Entitas.VisualDebugging.Unity'
  'Addons/Entitas.VisualDebugging.Unity.Editor'
  'Addons/Entitas.VisualDebugging.CodeGeneration.Plugins'
)
ENTITAS_EDITOR=(
  'Entitas.Migration.dll'
  'Entitas.Migration.Unity.Editor.dll'
  'Entitas.Unity.Editor.dll'
  'Entitas.VisualDebugging.Unity.Editor.dll'
)
ENTITAS_PLUGINS=(
  'Entitas.CodeGeneration.Plugins.dll'
  'Entitas.VisualDebugging.CodeGeneration.Plugins.dll'
)
BLUEPRINTS_PROJECTS=(
  'Addons/Entitas.Blueprints'
  'Addons/Entitas.Blueprints.CodeGeneration.Plugins'
  'Addons/Entitas.Blueprints.CodeGeneration.Unity.Plugins'
  'Addons/Entitas.Blueprints.Unity'
  'Addons/Entitas.Blueprints.Unity.Editor'
)
BLUEPRINTS_EDITOR=(
  'Entitas.Blueprints.Unity.Editor.dll'
)
BLUEPRINTS_PLUGINS=(
  'Entitas.Blueprints.CodeGeneration.Plugins.dll'
  'Entitas.Blueprints.CodeGeneration.Unity.Plugins.dll'
)
IMAGES=(
  "Addons/Entitas.Unity.Editor/Entitas.Unity.Editor/Images/"
  "Addons/Entitas.VisualDebugging.Unity.Editor/Entitas.VisualDebugging.Unity.Editor/Images/"
)
FILES=(
  'EntitasUpgradeGuide.md'
  'LICENSE.txt'
  'README.md'
  'CHANGELOG.md'
)
DESPERATEDEVS=(
  'Compile.cs'
  'DesperateDevs.Logging.dll'
  'DesperateDevs.Networking.dll'
  'DesperateDevs.Serialization.dll'
  'DesperateDevs.Utils.dll'
)
DESPERATEDEVS_EDITOR=(
  'DesperateDevs.Analytics.dll'
  'DesperateDevs.CodeGeneration.CodeGenerator.dll'
  'DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor.dll'
  'DesperateDevs.CodeGeneration.dll'
  'DesperateDevs.Unity.Editor.dll'
)
DESPERATEDEVS_PLUGINS=(
  'DesperateDevs.CodeGeneration.Plugins.dll'
  'DesperateDevs.CodeGeneration.Unity.Plugins.dll'
)

entitas::update() {
  log_func
  utils::clean_dir "${DEPS_DIR}"
  for d in "${DEPS[@]}"; do
    utils::sync "${d}" "${DEPS_DIR}"
  done
}

entitas::generate() {
  dotnet::build
  local properties=(
    'Tests/TestFixtures/Preferences.properties'
    'Readme/Prefrences.properties'
  )
  for p in "${properties[@]}"; do
    local dir="$(dirname ${p})"
    pushd "${dir}" > /dev/null
      log_strong "Generating ${p}"
      jenny gen "$(basename ${p})"
    popd > /dev/null
  done
}

entitas::collect_entitas_unity() {
  log_func
  local entitas_dir="${BUILD_SRC}/Unity/Entitas/Assets/Entitas"
  local entitas_editor_dir="${entitas_dir}/Editor"
  local entitas_plugins_dir="${entitas_editor_dir}/Plugins"
  local images_dir="${entitas_editor_dir}/Images"
  local desperatedevs_dir="${BUILD_SRC}/Unity/Entitas/Assets/DesperateDevs"
  local desperatedevs_editor_dir="${desperatedevs_dir}/Editor"
  local desperatedevs_plugins_dir="${desperatedevs_editor_dir}/Plugins"
  utils::clean_dir "${entitas_dir}" "${entitas_editor_dir}" "${entitas_plugins_dir}" "${images_dir}" \
            "${desperatedevs_dir}" "${desperatedevs_editor_dir}" "${desperatedevs_plugins_dir}"

  for p in "${ENTITAS_PROJECTS[@]}"; do utils::sync "${p}/bin/Release/" "${entitas_dir}"; done
  utils::sync "${DEPS_DIR}/" "${entitas_dir}"
  for f in "${ENTITAS_EDITOR[@]}"; do mv "${entitas_dir}/${f}" "${entitas_editor_dir}"; done
  for f in "${ENTITAS_PLUGINS[@]}"; do mv "${entitas_dir}/${f}" "${entitas_plugins_dir}"; done
  for f in "${IMAGES[@]}"; do utils::sync "${f}" "${images_dir}"; done
  for f in "${DESPERATEDEVS[@]}"; do mv "${entitas_dir}/${f}" "${desperatedevs_dir}"; done
  for f in "${DESPERATEDEVS_EDITOR[@]}"; do mv "${entitas_dir}/${f}" "${desperatedevs_editor_dir}"; done
  for f in "${DESPERATEDEVS_PLUGINS[@]}"; do mv "${entitas_dir}/${f}" "${desperatedevs_plugins_dir}"; done
}

entitas::collect_entitas_with_blueprints_unity() {
  log_func
  local entitas_dir="${BUILD_SRC}/Unity/Entitas/Assets/Entitas-Blueprints"
  local entitas_editor_dir="${entitas_dir}/Editor"
  local entitas_plugins_dir="${entitas_editor_dir}/Plugins"
  local images_dir="${entitas_editor_dir}/Images"
  local desperatedevs_dir="${BUILD_SRC}/Unity/Entitas/Assets/DesperateDevs"
  local desperatedevs_editor_dir="${desperatedevs_dir}/Editor"
  local desperatedevs_plugins_dir="${desperatedevs_editor_dir}/Plugins"
  utils::clean_dir "${entitas_dir}" "${entitas_editor_dir}" "${entitas_plugins_dir}" "${images_dir}" \
            "${desperatedevs_dir}" "${desperatedevs_editor_dir}" "${desperatedevs_plugins_dir}"

  for p in "${ENTITAS_PROJECTS[@]}"; do utils::sync "${p}/bin/Release/" "${entitas_dir}"; done
  for p in "${BLUEPRINTS_PROJECTS[@]}"; do utils::sync "${p}/bin/Release/" "${entitas_dir}"; done
  utils::sync "${DEPS_DIR}/" "${entitas_dir}"
  for f in "${ENTITAS_EDITOR[@]}"; do mv "${entitas_dir}/${f}" "${entitas_editor_dir}"; done
  for f in "${BLUEPRINTS_EDITOR[@]}"; do mv "${entitas_dir}/${f}" "${entitas_editor_dir}"; done
  for f in "${ENTITAS_PLUGINS[@]}"; do mv "${entitas_dir}/${f}" "${entitas_plugins_dir}"; done
  for f in "${BLUEPRINTS_PLUGINS[@]}"; do mv "${entitas_dir}/${f}" "${entitas_plugins_dir}"; done
  for d in "${IMAGES[@]}"; do utils::sync "${d}" "${images_dir}"; done
  for f in "${DESPERATEDEVS[@]}"; do mv "${entitas_dir}/${f}" "${desperatedevs_dir}"; done
  for f in "${DESPERATEDEVS_EDITOR[@]}"; do mv "${entitas_dir}/${f}" "${desperatedevs_editor_dir}"; done
  for f in "${DESPERATEDEVS_PLUGINS[@]}"; do mv "${entitas_dir}/${f}" "${desperatedevs_plugins_dir}"; done
}

entitas::collect_files() {
  log_func
  utils::clean_dir "${BUILD_FILES}"
  for f in "${FILES[@]}"; do
    utils::sync "${f}" "${BUILD_FILES}/${f}"
  done
}

entitas::sync_unity_visualdebugging() {
  log_func
  entitas::collect_entitas_unity
  local unity_libs="Tests/Unity/VisualDebugging/Assets/Libraries"
  utils::clean_dir "${unity_libs}"
  utils::sync "${BUILD_SRC}/Unity/Entitas/Assets/Entitas" "${unity_libs}"
  utils::sync "${BUILD_SRC}/Unity/Entitas/Assets/DesperateDevs" "${unity_libs}"
}

entitas::sync_unity_blueprints() {
  log_func
  entitas::collect_entitas_with_blueprints_unity
  local unity_libs="Tests/Unity/Blueprints/Assets/Libraries"
  utils::clean_dir "${unity_libs}"
  utils::sync "${BUILD_SRC}/Unity/Entitas/Assets/Entitas-Blueprints" "${unity_libs}"
  utils::sync "${BUILD_SRC}/Unity/Entitas/Assets/DesperateDevs" "${unity_libs}"
}

entitas::sync() {
  entitas::sync_unity_visualdebugging
  entitas::sync_unity_blueprints
}

entitas::pack_entitas_unity() {
  log_func
  entitas::collect_entitas_unity
  entitas::collect_files
  local tmp_dir="${BUILD}/tmp"
  utils::clean_dir "${tmp_dir}"

  utils::sync "${BUILD_SRC}/Unity/Entitas/Assets" "${tmp_dir}"
  utils::sync "${BUILD_FILES}/" "${tmp_dir}/Assets/Entitas"

  pushd "${BUILD_DIST}" > /dev/null
    local abs_build_dist="$(pwd)"
  popd > /dev/null

  pushd "${tmp_dir}" > /dev/null
    zip -rq "${abs_build_dist}/${PROJECT}.zip" ./
  popd > /dev/null
  rm -rf "${tmp_dir}"
}

entitas::pack() {
  log_func
  entitas::update
  dotnet::rebuild
  dotnet::tests

  utils::clean_dir "${BUILD_SRC}" "${BUILD_DIST}"

  doxygen::generate
#  create docset tgz
#  pushd "${DOCS_BUILD}/docset" > /dev/null
#    tar --exclude='.DS_Store' -czf "${BUILD_DIST}/${PROJECT_NAME}.docset.tgz" "${PROJECT}.docset"
#  popd > /dev/null

  entitas::pack_entitas_unity
  tree::create
}

entitas::dist() {
  log_func
  changelog::merge
  entitas::pack
  git::commit_release_sync_master
  git::push_all

  log "bzzz... giving GitHub some time to process..."
  sleep 10

  github::create_release
  open "https://github.com/${GITHUB_REPO}/releases"
}

entitas::dist_major() {
  version::bump_major
  entitas::dist
}

entitas::dist_minor() {
  version::bump_minor
  entitas::dist
}

entitas::dist_patch() {
  version::bump_patch
  entitas::dist
}
