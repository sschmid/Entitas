#!/usr/bin/env bash
set -euo pipefail
IFS=$'\n\t'

UNITY_PROJECTS=(samples/Unity)

declare -A PROJECTS=(
  [Entitas]=Assets/Plugins
  [Entitas.Generators.Attributes]=Assets/Plugins
  [Entitas.Unity]=Assets/Plugins
  [Entitas.Unity.Editor]=Assets/Plugins/Editor
)

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
      --include='*/' \
      --include='**/*.cs' \
      --include='**/*.png' \
      --include='**/*.png.meta' \
      --exclude='*' \
      "src/${project}" "${project_path}"
  done

  echo "Restore Dotfiles: ${unity_project_path}"
  mkdir -p "${unity_project_path}/.sln.dotsettings/"
  cp Entitas.sln.DotSettings "${unity_project_path}/$(basename "${unity_project_path}").sln.DotSettings"
  cp .sln.dotsettings/*.DotSettings "${unity_project_path}/.sln.dotsettings/"
  cp .editorconfig "${unity_project_path}"
done
