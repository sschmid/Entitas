: "${BUILD:=build}"

BUILD_SRC="${BUILD}/src"
BUILD_DIST="${BUILD}/dist"

entitas::help() {
  cat << 'EOF'
template:

usage:
  new <project-name>              add new src and test project
                                  e.g. bee entitas new Entitas.Xyz
  new_benchmark <project-name>    add benchmark project
                                  e.g. bee entitas new_benchmark Entitas.Xyz
  clean                           delete build directory and all bin and obj directories
  build                           build solution
  publish                         publish solution
  rebuild                         clean and build solution
  test [args]                     run unit tests
  jenny [args]                    build and run jenny
  generate                        generate code for all projects
  nuget                           publish nupkg to nuget.org
  nuget_local                     publish nupkg locally to disk
  pack                            pack Entitas and Jenny
  zip                             create Entitas.zip and Jenny.zip
  restore_unity_visualdebugging   copy source code and samples to all unity projects

EOF
}

entitas::comp() {
  if ((!$# || $# == 1 && COMP_PARTIAL)); then
    bee::comp_plugin entitas
  elif (($# == 1 || $# == 2 && COMP_PARTIAL)); then
    case "$1" in
      new|new_benchmark) echo "Entitas."; return ;;
    esac
  fi
}

entitas::new() {
  local name="$1" path
  path="src/${name}/src"
  dotnet new classlib -n "${name}" -o "${path}"
  cat << 'EOF' > "${path}/${name}.csproj"
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>$(DefaultTargetFramework)</TargetFramework>
    <PackageVersion>0.1.0</PackageVersion>
  </PropertyGroup>

</Project>
EOF
  dotnet sln add -s "${name}" "${path}/${name}.csproj"

  local test_name="${name}.Tests" path="src/${name}/tests"
  dotnet new xunit -n "${test_name}" -o "${path}"
  cat << 'EOF' > "${path}/${test_name}.csproj"
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>$(DefaultTestTargetFramework)</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsPublishable>false</IsPublishable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="FluentAssertions" Version="6.5.1" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.1.0" />
    <PackageReference Include="Microsoft.TestPlatform.ObjectModel" Version="17.1.0" />
    <PackageReference Include="xunit" Version="2.4.1" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.3" />
    <PackageReference Include="coverlet.collector" Version="3.1.2" />
  </ItemGroup>

</Project>
EOF
  dotnet sln add -s "${name}" "${path}/${test_name}.csproj"
}

entitas::new_benchmark() {
  local name="$1"
  local benchmark_name="${name}.Benchmarks" path="src/${name}/benchmarks"
  dotnet new console -n "${benchmark_name}" -o "${path}"
  cat << 'EOF' > "${path}/${benchmark_name}.csproj"
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>$(DefaultNetTargetFramework)</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsPublishable>false</IsPublishable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="BenchmarkDotNet" Version="0.13.1" />
  </ItemGroup>

</Project>
EOF
  dotnet sln add -s "${name}" "${path}/${benchmark_name}.csproj"
}

entitas::clean() {
  find . -type d -name obj -exec rm -rf {} +
  find . -type d -name bin -exec rm -rf {} +
  rm -rf "${BUILD}"
}

entitas::publish() {
  dotnet publish -c Release -p:UseAppHost=false
}

entitas::build() {
  dotnet build -c Release
}

entitas::rebuild() {
  entitas::clean
  dotnet build -c Release
}

entitas::test() {
  dotnet test -c Release "$@"
}

entitas::jenny() {
  dotnet run -c Release --project src/Entitas.CodeGeneration/jenny/Entitas.CodeGeneration.Program.csproj "$@"
}

entitas::generate() {
  local properties=(
    'Readme/Jenny.properties'
    'Tests/TestFixtures/Jenny.properties'
  )
  local dir jenny
  jenny="$(pwd)/src/Entitas.CodeGeneration/jenny/Entitas.CodeGeneration.Program.csproj"
  for p in "${properties[@]}"; do
    dir="$(dirname "${p}")"
    pushd "${dir}" > /dev/null || exit
      bee::log_info "Generating ${p}"
      dotnet run -c Release --project "${jenny}" gen "$(basename "${p}")"
    popd > /dev/null || exit
  done
}

entitas::nuget() {
  entitas::clean
  dotnet pack -c Release
  dotnet nuget push "**/*.nupkg" \
      --api-key "${NUGET_API_KEY}" \
      --skip-duplicate \
      --source https://api.nuget.org/v3/index.json
}

entitas::nuget_local() {
  entitas::clean
  dotnet pack -c Release
  _clean_dir "${ENTITAS_NUGET_LOCAL}"
  find . -type f -name "*.nupkg" -exec nuget add {} -Source "${ENTITAS_NUGET_LOCAL}" \;
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
#    Entitas.Blueprints
#    Entitas.Blueprints.Unity
    Entitas.CodeGeneration.Attributes
    Entitas.Unity
    Entitas.VisualDebugging.Unity
    
    # editor
#    Entitas.Blueprints.Unity.Editor
    Entitas.Migration
    Entitas.Migration.Unity.Editor
    Entitas.Unity.Editor
    Entitas.VisualDebugging.Unity.Editor
    
    # plugins
#    Entitas.Blueprints.CodeGeneration.Plugins
#    Entitas.Blueprints.CodeGeneration.Unity.Plugins
    Entitas.CodeGeneration.Plugins
    Entitas.Roslyn.CodeGeneration.Plugins
    Entitas.VisualDebugging.CodeGeneration.Plugins
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
    src/Entitas.Unity.Editor/src/Images/
    src/Entitas.VisualDebugging.Unity.Editor/src/Images/
  )

  local -a files=(
    EntitasUpgradeGuide.md
    LICENSE.txt
    README.md
    CHANGELOG.md
  )
    
  for p in "${projects[@]}"; do _sync "src/${p}/src/bin/Release/" "${entitas_dir}"; done
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
