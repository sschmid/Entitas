#!/bin/bash
set -e
source Scripts/build_commands.sh

collect_sources
update_project_dependencies
generateProjectFiles

collect_misc

build
runTests

sh Scripts/generate_docset.sh

post_build_collect_misc
create_zip
create_tree_overview
