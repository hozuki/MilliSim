#!/bin/bash

COLOR_RED='\033[0;31m'
COLOR_RESET='\033[0m'

if [ $TRAVIS != "true" ]
then
    echo "Not running on travis."
    exit 1
fi

echo -e "${COLOR_RED}MonoGame builds is not supported on Travis.${COLOR_RESET}"
exit 0
