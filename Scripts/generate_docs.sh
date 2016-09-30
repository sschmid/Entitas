#!/bin/bash
set -e

echo "### Generating Documentation... ========================================="

# Ensure Doxygen is installed
command -v doxygen >/dev/null 2>&1 || { echo >&2 "Error: Doxygen not found! Doxygen is required to generate docsets. Try \"brew install doxygen\"."; exit 1; }

# Ensure we are in the directory where the doxyfile is
pushd "$(dirname "$(readlink "$BASH_SOURCE" || echo "$BASH_SOURCE")")"

# Use the current Entitas version to generate the docset name and id
ENTITAS_VERSION=`cat ../Entitas/Entitas/entitas_version`
PROJECT_NAME="Entitas-${ENTITAS_VERSION}"

perl -pi -w -e "s/PROJECT_NAME.*\n/PROJECT_NAME = ${PROJECT_NAME}\n/s" docset.doxyfile
perl -pi -w -e "s/DOCSET_BUNDLE_ID.*\n/DOCSET_BUNDLE_ID = ${PROJECT_NAME}\n/s" docset.doxyfile

perl -pi -w -e "s/PROJECT_NAME.*\n/PROJECT_NAME = ${PROJECT_NAME}\n/s" html.doxyfile

# Generate the docset
doxygen docset.doxyfile
cd ../Build/docs/html
make

# In order for Dash to associate this docset with the "entitas" keyword,
# we have to manually modify the generated plist.
perl -pi -w -e "s/<\/dict>/<key>DocSetPlatformFamily<\/key><string>entitas<\/string><key>DashDocSetFamily<\/key><string>doxy<\/string><\/dict>/s" ${DOCSET_NAME}.docset/Contents/Info.plist

zip -r "../${PROJECT_NAME}.docset.zip" "${PROJECT_NAME}.docset"

# Remove all temp files
rm -rf ../html

# Generate html docs
cd -
doxygen html.doxyfile

rsync -arv ../Build/docs/html/ ../docs

popd

echo "### Generating Documentation done ======================================="
