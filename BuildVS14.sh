#Update submodules
git submodule init
git submodule update

#Build System.Utilities
cd System.Utilities
git pull origin master
"/C/Program Files (x86)/MSBuild/14.0/Bin/MSBuild.exe" System.Utilities.sln -nologo -p:Configuration=Release -t:Clean\;Build -p:TrackFileAccess=false
cd ..

#Build the NETPath SDK
./Tools/Nuget.exe install ./NET45/packages.config -NonInteractive -SolutionDirectory ./
"/C/Program Files (x86)/MSBuild/14.0/Bin/MSBuild.exe" SDK.sln -nologo -p:Configuration=Release -t:Clean\;Build -p:TrackFileAccess=false