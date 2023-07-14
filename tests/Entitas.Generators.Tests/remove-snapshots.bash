#!/usr/bin/env bash
set -uo pipefail
IFS=$'\n\t'

dir=snapshots
files="$(cat snapshots-to-delete.txt)"

for f in ${files}; do
  rm -v "${dir}/${f#*- }"
done
