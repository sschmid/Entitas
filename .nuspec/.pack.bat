@echo off

rmdir .packages /s /q
call msbuild /p:Configuration=Release
for %%f in (*.nuspec) do (
    echo %%~nf
    call nuget pack %%f -OutputDirectory ".packages" -InstallPackageToOutputPath
)