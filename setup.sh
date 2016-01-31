#!/bin/sh
set -e

source build_commands.sh

collect_sources
update_project_dependencies
generateProjectFiles
build
collect_misc_files
