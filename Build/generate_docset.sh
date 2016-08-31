#!/bin/sh
set -e

# Ensure Doxygen is installed
command -v doxygen >/dev/null 2>&1 || { echo >&2 "Error: Doxygen not found! Doxygen is required to generate docsets. Try \"brew install doxygen\"."; exit 1; }

# Ensure we are in the directory where the doxyfile is
pushd "$(dirname "$(readlink "$BASH_SOURCE" || echo "$BASH_SOURCE")")"

# Use the current Entitas version to generate the docset name and id
ENTITAS_VERSION=`cat ../Entitas/Entitas/entitas_version`
DOCSET_NAME="Entitas-${ENTITAS_VERSION}"
perl -pi -w -e "s/PROJECT_NAME.*\n/PROJECT_NAME = ${DOCSET_NAME}\n/s" docset.doxyfile
perl -pi -w -e "s/DOCSET_BUNDLE_ID.*\n/DOCSET_BUNDLE_ID = ${DOCSET_NAME}\n/s" docset.doxyfile

# Generate the docset
doxygen docset.doxyfile
cd html
make

# In order for Dash to associate this docset with the "ent"
# (short for "Entitas") keyword, we have to manually modify
# the generated plist.
perl -pi -w -e "s/<\/dict>/<key>DocSetPlatformFamily<\/key><string>ent<\/string><key>DashDocSetFamily<\/key><string>doxy<\/string><\/dict>/s" ${DOCSET_NAME}.docset/Contents/Info.plist

# Move the docset to the root Documentation directory
rm -rf "../../Documentation"
mkdir -p "../../Documentation"
zip -r "../../Documentation/${DOCSET_NAME}.docset.zip" "${DOCSET_NAME}.docset"

# Remove all temp files
rm -rf ../html
popd
