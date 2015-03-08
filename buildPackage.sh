#!/bin/sh
rm -rfv bin
mkdir -v bin
mkdir -v bin/tmp

cp -rv Entitas/Entitas bin/tmp
cp -rv EntitasCodeGenerator/EntitasCodeGenerator bin/tmp
mkdir -v bin/tmp/EntitasUnity
cp -rv EntitasUnity/CodeGenerator bin/tmp/EntitasUnity/
cp -rv EntitasUnity/VisualDebugging/Assets/VisualDebugging bin/tmp/EntitasUnity/

cd bin/tmp
zip -r ../Entitas.zip ./
cd ..
rm -rfv tmp

find . -name "*.DS_Store" -type f -delete