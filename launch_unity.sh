#!/bin/bash

# Build the container
./containers/unity/build.sh

# Setup the X server
xhost +local:

# Run the contianer
docker run -it \
    --privileged \
    --pid host \
    --net host \
    --cap-add SYS_ADMIN \
    --security-opt apparmor:unconfined \
    --device /dev/fuse:rw \
    --env "DISPLAY=$DISPLAY" \
    -v /tmp/.X11-unix:/tmp/.X11-unix:ro \
    -v /opt/unity:/opt/unity \
    -v ./.unityhub:/opt/unityhub \
    -v ./.config/unityhub:/root/.config/unityhub \
    -v ./tests:/home/root/tests \
    -v ./src:/home/root/cpe/src \
    unity-22-04
