::delete old file
DEL jCute.all.js
::merge file
COPY jCute.js/B+jCute.*.js/B jCuteAll.js /Y
::encoding utf-8
PowerShell -Command "& {get-content jCuteAll.js -encoding utf8 | set-content jCute.all.js -encoding utf8}"
::delete temp file
DEL jCuteAll.js