@echo off

rmdir ".packages" /s /q
rmdir ".source" /s /q
rmdir "bin" /s /q
rmdir "obj" /s /q
start /b nuget locals all -Clear

:loop1
    pslist nuget >nul 2>&1
    if errorlevel 1 (goto continue1) else (goto loop1)

:continue1
    for %%f in (*.nuspec) do (
        start /b nuget pack %%f -OutputDirectory ".packages"
    )

:loop2
    pslist nuget >nul 2>&1
    if errorlevel 1 (goto continue2) else (goto loop2)

:continue2
    call nuget init ".packages" ".source"
    call nuget restore ".nuspec.csproj"
    start /b msbuild ".nuspec.csproj" /p:Configuration=Debug /t:Build
    start /b msbuild ".nuspec.csproj" /p:Configuration=Release /t:Build