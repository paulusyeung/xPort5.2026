$msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
& $msbuild xPort5.sln /t:Build /p:Configuration=Debug /v:minimal /nologo
