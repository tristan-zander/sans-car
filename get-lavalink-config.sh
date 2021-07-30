#!/usr/bin/bash

set -e

# TODO
# This script currently doesn't overwrite $LAVALINK_CONFIG if it knows it's not
# a yml file. I just don't want to add the stdin part where you ask the user to
# overwrite that file. 

function get_lavalink_conf() { 
	curl -o ./lavalink.yml https://raw.githubusercontent.com/freyacodes/Lavalink/master/LavalinkServer/application.yml.example
	$EDITOR ./lavalink.yml
}

LAVALINK_CONFIG=${LAVALINK_CONFIG:="lavalink.yml"}

# Check based on environment variable.
if [[ "${LAVALINK_CONFIG:${#LAVALINK_CONFIG}-4}" == ".yml" ]] && [[ -f "$LAVALINK_CONFIG" ]]; then
	echo "Existing lavalink configuration found."
else
	echo "No lavalink file found."
	get_lavalink_conf 
fi
