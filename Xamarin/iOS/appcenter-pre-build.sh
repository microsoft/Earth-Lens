#!/usr/bin/env bash
# Custom build script for Visual Studio App Center to set environment variables
# For more info visit: 
# https://docs.microsoft.com/en-us/appcenter/build/custom/variables/
# https://docs.microsoft.com/en-us/appcenter/build/custom/scripts/index

echo "APP_CENTER_SECRET_IOS=$APP_CENTER_SECRET_IOS" > Environment.txt
