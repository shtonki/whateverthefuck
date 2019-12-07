@echo off
rm whateverthefuck/bin/ -r
rm whateverthefuck/obj/ -r

rm whateverthefuckserver/bin/ -r
rm whateverthefuckserver/obj/ -r

msbuild /p:Configuration=Release

rm releases/current_release -r

mkdir "releases/current_release"

mv whateverthefuck\bin\Release\ releases\current_release\client
mv whateverthefuckserver\bin\Release\ releases\current_release\server
