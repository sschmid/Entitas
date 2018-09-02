#!/bin/bash

BUILD="${ROOT}/Build"

# utils
UTILS_RSYNC_INCLUDE="${ROOT}/Scripts/rsync_include.txt"
UTILS_RSYNC_EXCLUDE="${ROOT}/Scripts/rsync_exclude.txt"
UTILS_TREE_IGNORE="bin|obj|Library|Libraries|*Tests|Readme|ProjectSettings|Build|docs|Temp|Examples|*.csproj|*.meta|*.sln|*.userprefs|*.properties|tree.txt"
UTILS_TREE_PATH="tree.txt"

# dotnet
DOTNET_PROJECT_NAME="Entitas"
DOTNET_SOLUTION="${ROOT}/${DOTNET_PROJECT_NAME}.sln"
DOTNET_TESTS_PROJECT="${ROOT}/Tests/Tests/Tests.csproj"
DOTNET_TESTS_RUNNER="${ROOT}/Tests/Tests/bin/Release/Tests.exe"

# version
VERSION_PATH="${ROOT}/${DOTNET_PROJECT_NAME}/${DOTNET_PROJECT_NAME}/version.txt"

# unity
UNITY_PROJECT_PATH="${ROOT}"

# docs
DOCS_BUILD="${BUILD}/docs"
DOCS_RES="${ROOT}/Scripts/res"
DOCS_EXPORT="${ROOT}/docs"
declare -a -r DOCS_DOXY_FILES=(
  "${DOCS_RES}/html.doxyfile"
#  "${DOCS_RES}/docset.doxyfile"
)
DOCS_DOCSET="com.desperatedevs.${DOTNET_PROJECT_NAME}.docset"
DOCS_DOCSET_KEY="$(echo ${DOTNET_PROJECT_NAME} | tr "[:upper:]" "[:lower:]")"
declare -a -r DOCS_DOCSET_ICONS=(
  "${DOCS_RES}/icon.png"
  "${DOCS_RES}/icon@2x.png"
)

# changelog
CHANGELOG_PATH="${ROOT}/RELEASE_NOTES.md"
CHANGELOG_CHANGES="${ROOT}/changes.md"

# github
GITHUB_CHANGES="${CHANGELOG_CHANGES}"
GITHUB_RELEASE_PREFIX="${DOTNET_PROJECT_NAME}"
GITHUB_REPO="sschmid/Entitas-CSharp"
declare -a -r GITHUB_ATTACHMENTS_ZIP=(
  "${DOTNET_PROJECT_NAME}.zip"
#  "${DOTNET_PROJECT_NAME}.docset.tgz"
)

# entitas
BUILD_SRC="${BUILD}/src"
BUILD_FILES="${BUILD_SRC}/files"
BUILD_DIST="${BUILD}/dist"

DEPS_DIR="${ROOT}/Libraries/Dependencies/DesperateDevs"
declare -a -r DEPS=(
  "../DesperateDevs/DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor/bin/Release/"
  "../DesperateDevs/DesperateDevs.CodeGeneration.Plugins/bin/Release/"
  "../DesperateDevs/DesperateDevs.CodeGeneration.Unity.Plugins/bin/Release/"
  "../DesperateDevs/DesperateDevs.Unity.Editor/bin/Release/"
  "../DesperateDevs/DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor/Compile.cs"
)

declare -a -r entitas_projects=(
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

declare -a -r entitas_editor=(
  'Entitas.Migration.dll'
  'Entitas.Migration.Unity.Editor.dll'
  'Entitas.Unity.Editor.dll'
  'Entitas.VisualDebugging.Unity.Editor.dll'
)

declare -a -r entitas_plugins=(
  'Entitas.CodeGeneration.Plugins.dll'
  'Entitas.VisualDebugging.CodeGeneration.Plugins.dll'
)

declare -a -r images=(
  "Addons/Entitas.Unity.Editor/Entitas.Unity.Editor/Images/"
  "Addons/Entitas.VisualDebugging.Unity.Editor/Entitas.VisualDebugging.Unity.Editor/Images/"
)

declare -a -r desperatedevs=(
  'Compile.cs'
  'DesperateDevs.Logging.dll'
  'DesperateDevs.Networking.dll'
  'DesperateDevs.Serialization.dll'
  'DesperateDevs.Utils.dll'
)

declare -a -r desperatedevs_editor=(
  'DesperateDevs.Analytics.dll'
  'DesperateDevs.CodeGeneration.CodeGenerator.dll'
  'DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor.dll'
  'DesperateDevs.CodeGeneration.dll'
  'DesperateDevs.Unity.Editor.dll'
)

declare -a -r desperatedevs_plugins=(
  'DesperateDevs.CodeGeneration.Plugins.dll'
  'DesperateDevs.CodeGeneration.Unity.Plugins.dll'
)

entitas::update() {
  bee::log_func
  utils::clean_dir "${DEPS_DIR}"
  for d in "${DEPS[@]}"; do
    utils::sync_files ${d} "${DEPS_DIR}"
  done
}

entitas::generate() {
  dotnet::build
  declare -a -r properties=(
    'Tests/TestFixtures/Preferences.properties'
    'Readme/Prefrences.properties'
  )
  for p in "${properties[@]}"; do
    local dir=$(dirname ${p})
    pushd "${dir}" > /dev/null
      bee::log_strong "Generating ${p}"
      mono "${ROOT}/../DesperateDevs/DesperateDevs.CodeGeneration.CodeGenerator.CLI/bin/Release/DesperateDevs.CodeGeneration.CodeGenerator.CLI.exe" gen "${ROOT}/${p}"
    popd > /dev/null
  done
}

entitas::collect_entitas_unity() {
  bee::log_func
  local entitas_dir="${BUILD_SRC}/Unity/Entitas/Assets/Entitas"
  local entitas_editor_dir="${entitas_dir}/Editor"
  local entitas_plugins_dir="${entitas_editor_dir}/Plugins"
  local images_dir="${entitas_editor_dir}/Images"
  local desperatedevs_dir="${BUILD_SRC}/Unity/Entitas/Assets/DesperateDevs"
  local desperatedevs_editor_dir="${desperatedevs_dir}/Editor"
  local desperatedevs_plugins_dir="${desperatedevs_editor_dir}/Plugins"
  utils::clean_dir "${entitas_dir}" "${entitas_editor_dir}" "${entitas_plugins_dir}" "${images_dir}" \
            "${desperatedevs_dir}" "${desperatedevs_editor_dir}" "${desperatedevs_plugins_dir}"

  for p in "${entitas_projects[@]}"; do
    utils::sync_files "${ROOT}/${p}/bin/Release/" "${entitas_dir}"
  done

  utils::sync_files "${DEPS_DIR}/" "${entitas_dir}"

  for f in "${entitas_editor[@]}"; do
    mv "${entitas_dir}/${f}" "${entitas_editor_dir}"
  done

  for f in "${entitas_plugins[@]}"; do
    mv "${entitas_dir}/${f}" "${entitas_plugins_dir}"
  done

  for d in "${images[@]}"; do
    utils::sync_files "${ROOT}/${d}" "${images_dir}"
  done

  for f in "${desperatedevs[@]}"; do
    mv "${entitas_dir}/${f}" "${desperatedevs_dir}"
  done

  for f in "${desperatedevs_editor[@]}"; do
    mv "${entitas_dir}/${f}" "${desperatedevs_editor_dir}"
  done

  for f in "${desperatedevs_plugins[@]}"; do
    mv "${entitas_dir}/${f}" "${desperatedevs_plugins_dir}"
  done
}

entitas::collect_entitas_with_blueprints_unity() {
  bee::log_func
  local entitas_dir="${BUILD_SRC}/Unity/Entitas/Assets/Entitas-Blueprints"
  local entitas_editor_dir="${entitas_dir}/Editor"
  local entitas_plugins_dir="${entitas_editor_dir}/Plugins"
  local images_dir="${entitas_editor_dir}/Images"
  local desperatedevs_dir="${BUILD_SRC}/Unity/Entitas/Assets/DesperateDevs"
  local desperatedevs_editor_dir="${desperatedevs_dir}/Editor"
  local desperatedevs_plugins_dir="${desperatedevs_editor_dir}/Plugins"
  utils::clean_dir "${entitas_dir}" "${entitas_editor_dir}" "${entitas_plugins_dir}" "${images_dir}" \
            "${desperatedevs_dir}" "${desperatedevs_editor_dir}" "${desperatedevs_plugins_dir}"

  declare -a -r blueprints_projects=(
    'Addons/Entitas.Blueprints'
    'Addons/Entitas.Blueprints.CodeGeneration.Plugins'
    'Addons/Entitas.Blueprints.CodeGeneration.Unity.Plugins'
    'Addons/Entitas.Blueprints.Unity'
    'Addons/Entitas.Blueprints.Unity.Editor'
  )
  for p in "${entitas_projects[@]}"; do
    utils::sync_files "${ROOT}/${p}/bin/Release/" "${entitas_dir}"
  done
  for p in "${blueprints_projects[@]}"; do
    utils::sync_files "${ROOT}/${p}/bin/Release/" "${entitas_dir}"
  done

  utils::sync_files "${DEPS_DIR}/" "${entitas_dir}"

  declare -a -r blueprints_editor=(
    'Entitas.Blueprints.Unity.Editor.dll'
  )
  for f in "${entitas_editor[@]}"; do
    mv "${entitas_dir}/${f}" "${entitas_editor_dir}"
  done
  for f in "${blueprints_editor[@]}"; do
    mv "${entitas_dir}/${f}" "${entitas_editor_dir}"
  done

  declare -a -r blueprints_plugins=(
    'Entitas.Blueprints.CodeGeneration.Plugins.dll'
    'Entitas.Blueprints.CodeGeneration.Unity.Plugins.dll'
  )
  for f in "${entitas_plugins[@]}"; do
    mv "${entitas_dir}/${f}" "${entitas_plugins_dir}"
  done
  for f in "${blueprints_plugins[@]}"; do
    mv "${entitas_dir}/${f}" "${entitas_plugins_dir}"
  done

  for d in "${images[@]}"; do
    utils::sync_files "${ROOT}/${d}" "${images_dir}"
  done

  for f in "${desperatedevs[@]}"; do
    mv "${entitas_dir}/${f}" "${desperatedevs_dir}"
  done

  for f in "${desperatedevs_editor[@]}"; do
    mv "${entitas_dir}/${f}" "${desperatedevs_editor_dir}"
  done

  for f in "${desperatedevs_plugins[@]}"; do
    mv "${entitas_dir}/${f}" "${desperatedevs_plugins_dir}"
  done
}

entitas::collect_files() {
  bee::log_func
  utils::clean_dir "${BUILD_FILES}"
  declare -a -r files=(
    'EntitasUpgradeGuide.md'
    'LICENSE.txt'
    'README.md'
    'RELEASE_NOTES.md'
  )
  for f in "${files[@]}"; do
    utils::sync_files "${ROOT}/${f}" "${BUILD_FILES}/${f}"
  done
}

entitas::sync_unity_visualdebugging() {
  bee::log_func
  entitas::collect_entitas_unity
  local unity_libs="${ROOT}/Tests/Unity/VisualDebugging/Assets/Libraries"
  utils::clean_dir "${unity_libs}"
  utils::sync_files "${BUILD_SRC}/Unity/Entitas/Assets/Entitas" "${unity_libs}"
  utils::sync_files "${BUILD_SRC}/Unity/Entitas/Assets/DesperateDevs" "${unity_libs}"
}

entitas::sync_unity_blueprints() {
  bee::log_func
  entitas::collect_entitas_with_blueprints_unity
  local unity_libs="${ROOT}/Tests/Unity/Blueprints/Assets/Libraries"
  utils::clean_dir "${unity_libs}"
  utils::sync_files "${BUILD_SRC}/Unity/Entitas/Assets/Entitas-Blueprints" "${unity_libs}"
  utils::sync_files "${BUILD_SRC}/Unity/Entitas/Assets/DesperateDevs" "${unity_libs}"
}

entitas::sync() {
  entitas::sync_unity_visualdebugging
  entitas::sync_unity_blueprints
}

entitas::pack_entitas_unity() {
  bee::log_func
  entitas::collect_entitas_unity
  entitas::collect_files
  local tmp_dir="${BUILD}/tmp"
  utils::clean_dir "${tmp_dir}"

  utils::sync_files "${BUILD_SRC}/Unity/Entitas/Assets" "${tmp_dir}"
  utils::sync_files "${BUILD_FILES}/" "${tmp_dir}/Assets/Entitas"

  pushd "${tmp_dir}" > /dev/null
    zip -rq "$BUILD_DIST/${DOTNET_PROJECT_NAME}.zip" ./
  popd > /dev/null
  rm -rf "${tmp_dir}"
}

entitas::pack() {
  bee::log_func
  entitas::update
  dotnet::clean_build
  dotnet::tests

  utils::clean_dir "${BUILD_SRC}" "${BUILD_DIST}"

   docs::generate
#   create docset tgz
#   pushd "${DOCS_BUILD}/docset" > /dev/null
#     tar --exclude='.DS_Store' -czf "${BUILD_DIST}/${DOTNET_PROJECT_NAME}.docset.tgz" "${DOTNET_PROJECT_NAME}.docset"
#   popd > /dev/null

  entitas::pack_entitas_unity
  utils::tree
}

entitas::dist() {
  bee::log_func
  changelog::merge
  entitas::pack
  git::commit_release
  git::push

  bee::log "bzzz... giving GitHub some time to process..."
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
