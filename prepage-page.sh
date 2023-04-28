#!/bin/sh

has_changes=$(git status --porcelain)
if [ "z$has_changes" != "z" ]; then
    echo "There are changes in the working directory. Please commit or stash them before running this script."
    exit 1
fi
commit=$(git rev-parse HEAD)
echo "Running with commit $commit"


rm -rf bin obj

dotnet publish -c Release -f net8.0

git checkout gh-pages

rm -rf net8

mv bin/Release/net8.0/browser-wasm/AppBundle net8_bundle

rm -rf bin obj

mv net8_bundle/* .

rmdir net8_bundle

git add --all 
git commit -m "Update page with commit $commit"
