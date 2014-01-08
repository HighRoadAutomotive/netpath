#Update submodules
git submodule init
git submodule update

#Build System.Utilities
cd System.Utilities
git pull origin net.randombit.botan
/C/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe System.Utilities.sln -nologo -p:Configuration=Release -t:Clean\;Build -p:TrackFileAccess=false
cd ..

#Build the NETPath SDK
/C/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe SDK.sln -nologo -p:Configuration=Release -t:Clean\;Build -p:TrackFileAccess=false