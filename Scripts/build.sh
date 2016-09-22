#!/bin/sh
set -e

source Build/build_commands.sh

collect_sources
update_project_dependencies
generateProjectFiles
build
runTests

sh Build/generate_docset.sh

collect_misc_files
create_zip
create_tree_overview
