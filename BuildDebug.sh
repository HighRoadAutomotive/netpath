#Update NETPath
git pull origin master

#Update Submodules
git submodule init
git submodule update
cd System.Utilities
git pull origin master

#Build System.Utilities
/C/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe ./WinRT8/WinRT8.csproj -nologo -p:Configuration=Debug -p:Platform=AnyCPU -t:Build
/C/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe ./NET45/NET45.csproj -nologo -p:Configuration=Debug -p:Platform=AnyCPU -t:Build

cd ..

#Build NETPath
/C/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe ./NETPath.sln -nologo -t:Build