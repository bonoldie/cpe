#!/bin/bash
echo "##########################"
echo "# Unity Hub installation #"
echo "##########################"
echo ""
echo ""

apt update
apt-get install -y nano curl screen screenfetch git proxychains \
        alsa alsa-tools libnss3 \
        libcanberra-gtk-module libcanberra-gtk3-module \
        apt-utils fuse desktop-file-utils \
        mesa-utils xdg-utils xvfb zenity 

curl -sSL https://public-cdn.cloud.unity3d.com/hub/prod/UnityHub.AppImage -o UnityHub.AppImage 
chmod +x UnityHub.AppImage 