@echo off

rmdir ".packages" /s /q
rmdir ".source" /s /q
rmdir "bin" /s /q
rmdir "obj" /s /q
call nuget locals all -Clear
for %%f in (*.nuspec) do (
    echo %%~nf
    call nuget pack %%f -OutputDirectory ".packages" -InstallPackageToOutputPath
)
call nuget init ".packages" ".source"
call nuget restore ".nuspec.csproj"
start /b msbuild ".nuspec.csproj" /p:Configuration=Debug /t:Build
start /b msbuild ".nuspec.csproj" /p:Configuration=Release /t:Build