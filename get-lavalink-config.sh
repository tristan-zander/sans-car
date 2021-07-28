#!/usr/bin/bash

set -e

function get_lavalink_conf() { 
	curl -o ./lavalink.yml https://raw.githubusercontent.com/freyacodes/Lavalink/master/LavalinkServer/application.yml.example
	less ./lavalink.yml
}

LAVALINK_CONFIG=${LAVALINK_CONFIG:="lavalink.yml"}

# Check based on environment variable.
if [[ ! -f $LAVAINK_CONFIG ]] && [[ ! ${LAVALINK_CONFIG:${#LAVALINK_CONFIG}-4} == ".yml" ]]; then
	echo "No lavalink file found."
	get_lavalink_conf 
else
	echo "Existing lavalink configuration found."
fi
