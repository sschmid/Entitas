#!/bin/bash
PWD=$(pwd)

# Folders
BUILD="Build"
BUILD_SRC="$BUILD/src"
UNITY='/Applications/Unity_5.3.4p6/Unity.app/Contents/MacOS/Unity'

# Tests
TEST_PROJECT="Tests/Tests.csproj"
TEST_RUNNER="Tests/bin/Release/Tests.exe"

# Projects
BUILD_PROJECT="Entitas.sln"

ES="Entitas"
CG="Entitas.CodeGenerator"
MIG="Entitas.Migration"
CG_TR="Entitas.CodeGenerator.TypeReflection"
ESU="Entitas.Unity"

# Project folders
ESU_ASSETS="$ESU/Assets"
ESU_LIBS="$ESU_ASSETS/Libraries"

# Aliases
alias cpe='rsync -arv --exclude-from "Scripts/exclude.txt"'
