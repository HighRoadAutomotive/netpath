#Update Submodules
git submodule init
git submodule update

#Build EllipticBit.Controls
cd EllipticBit.Controls
"/C/Program Files (x86)/MSBuild/14.0/Bin/MSBuild.exe" EllipticBit.Controls.sln -nologo -p:Configuration=Debug -p:Platform="Any CPU" -t:Clean\;Build -p:TrackFileAccess=false
cd ..

#Build System.Utilities
cd System.Utilities
"/C/Program Files (x86)/MSBuild/14.0/Bin/MSBuild.exe" ./NET45/NET45.csproj -nologo -p:Configuration=Debug -p:Platform=AnyCPU -t:Clean\;Build -p:TrackFileAccess=false
cd ..

#Build NETPath
"/C/Program Files (x86)/MSBuild/14.0/Bin/MSBuild.exe" ./NETPath.sln -nologo -p:Configuration=Debug -p:Platform="Any CPU" -t:Clean\;Build -p:TrackFileAccess=false