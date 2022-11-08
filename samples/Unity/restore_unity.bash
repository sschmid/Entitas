#!/usr/bin/env bash
set -euo pipefail
IFS=$'\n\t'

UNITY_PROJECTS=(samples/Unity)
JENNY="$(pwd)/tests/Entitas.CodeGeneration.Program/Entitas.CodeGeneration.Program.csproj"

declare -A PROJECTS=(
  [Entitas]=Assets/Plugins
  [Entitas.Blueprints]=Assets/Plugins
  [Entitas.Blueprints.Unity]=Assets/Plugins
  [Entitas.Blueprints.Unity.Editor]=Assets/Plugins/Editor
  [Entitas.CodeGeneration.Attributes]=Assets/Plugins
  [Entitas.Migration]=Assets/Plugins/Editor
  [Entitas.Migration.Unity.Editor]=Assets/Plugins/Editor
  [Entitas.Unity]=Assets/Plugins
  [Entitas.Unity.Editor]=Assets/Plugins/Editor
  [Entitas.VisualDebugging.Unity]=Assets/Plugins
  [Entitas.VisualDebugging.Unity.Editor]=Assets/Plugins/Editor
)

RSYNC_EXCLUDE=(
  .DS_Store
  Thumbs.db
  *.pdb
  *.config
  *.json
  *.xml
  Bee.BeeDriver.dll
  BeeBuildProgramCommon.Data.dll
  DesperateDevs*
  ExCSS.Unity.dll
  PlayerBuildProgramLibrary.Data.dll
  ScriptCompilationBuildProgram.Data.dll
  Sherlog*
  Unity.Cecil.dll
  Unity.CompilationPipeline.Common.dll
  Unity.IL2CPP.BeeSettings.dll
  UnityEditor.dll
  UnityEngine.dll
)

RSYNC_INCLUDE=(
  *.deps.json
  *.runtimeconfig.json
)

dotnet publish -c Release

for unity_project_path in "${UNITY_PROJECTS[@]}"; do
  echo "Restore Entitas: ${unity_project_path}"
  for project in "${!PROJECTS[@]}"; do
    echo "Restore ${project}: ${unity_project_path}"
    project_path="${unity_project_path}/${PROJECTS["${project}"]}"
    mkdir -p "${project_path}"
    rsync \
      --archive \
      --recursive \
      --prune-empty-dirs \
      --include-from <(echo "${RSYNC_INCLUDE[*]}") \
      --exclude-from <(echo "${RSYNC_EXCLUDE[*]}") \
      "src/${project}/bin/Release/publish/"*.dll "${project_path}"
  done

  pushd "${unity_project_path}/Assets/Plugins/Editor" > /dev/null || exit 1
    while read -r file; do
      [[ ! -f ".${file}" ]] || rm "${file}"
    done < <(find . -type f -name "*.dll")
  popd > /dev/null || exit 1

  echo "Restore Dotfiles: ${unity_project_path}"
  mkdir -p "${unity_project_path}/.sln.dotsettings/"
  cp Entitas.sln.DotSettings "${unity_project_path}/$(basename "${unity_project_path}").sln.DotSettings"
  cp .sln.dotsettings/*.DotSettings "${unity_project_path}/.sln.dotsettings/"
  cp .editorconfig "${unity_project_path}"

  pushd "${unity_project_path}" > /dev/null || exit 1
    dotnet run -c Release --project "${JENNY}" gen
  popd > /dev/null || exit 1
done
