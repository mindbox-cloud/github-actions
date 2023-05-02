#!/bin/sh -l

repo_url="https://github.com/$REPO"
git clone --quiet $repo_url &> /dev/null

git config --global --add safe.directory /github/workspace

tag=$(git tag --sort version:refname | tail -n 2 | head -n 1)
if [ "$tag" ]; then
  changelog=$(git log --oneline --no-decorate $tag..HEAD)
  lastcommit=$(git log $tag..HEAD --pretty=format:"%H" --max-count=1)
else
  changelog=$(git log --oneline --no-decorate)
  lastcommit=$(git log --pretty=format:"%H" --max-count=1)
fi

echo "last commit: $lastcommit"
echo $changelog

changelog="${changelog//'%'/'%25'}"
#release problem inspection
echo $changelog

changelog="${changelog//$'\n'/'%0A' - }"
#release problem inspection
echo $changelog

changelog=" - ${changelog//$'\r'/'%0D'}"
#release problem inspection
echo $changelog

lastcommit_url="$repo_url/commits/$lastcommit"
lastcommit_hyperlink="[View latest commits in Github]($lastcommit_url)"

changelog=$lastcommit_hyperlink$'<br/>'$changelog
#release problem inspection
echo $changelog

echo "changelog=$changelog" >> $GITHUB_OUTPUT



