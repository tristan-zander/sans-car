FROM fredboat/lavalink:master
WORKDIR /opt/Lavalink
# TODO get from $LAVALINK_CONFIG
COPY ../lavalink.yml lavalink.yml
