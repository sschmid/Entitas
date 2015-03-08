#!/bin/sh
rm -rfv bin
mkdir -v bin

cp -rv Entitas/Entitas bin/
cp -rv EntitasCodeGenerator/EntitasCodeGenerator bin/
cp -rv EntitasCodeGenerator/EntitasCodeGenerator bin/

mkdir -v bin/EntitasUnity
cp -rv EntitasUnity/CodeGenerator bin/EntitasUnity/
cp -rv EntitasUnity/VisualDebugging/Assets/VisualDebugging bin/EntitasUnity/VisualDebugging/

find . -name "*.DS_Store" -type f -delete