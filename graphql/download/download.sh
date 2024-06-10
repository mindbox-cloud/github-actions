#!/usr/bin/env bash
set +eo pipefail

directory=$1
start_command=$2
stop_command=$3
schema_url=$4
schema_path=$5

cd $directory || exit 1
echo "start at $(pwd): $start_command"
eval $start_command
echo started
while true; do
    output=$(curl $schema_url)
    if [ ! -z "$output" ]; then
        break
    fi
    sleep 0.1
done
echo "download $schema_url to $schema_path"
curl $schema_url > "$schema_path"
echo "stop with $stop_command"
eval $stop_command
echo stopped
