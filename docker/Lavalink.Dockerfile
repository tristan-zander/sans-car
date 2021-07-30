FROM fredboat/lavalink:master
WORKDIR /opt/Lavalink
# TODO get from $LAVALINK_CONFIG
COPY application.yml application.yml
