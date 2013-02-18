rm -rf System.Utilities/

git clone git://github.com/prospectivesoftware/systemutilities.git System.Utilities

cd System.Utilities

/C/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe System.Utilities.sln -nologo -p:Configuration=Release -t:Clean\;Build -p:TrackFileAccess=false

cd ../ProtoBufNet

git svn fetch

cd ..

/C/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe protobufnet-portable.csproj -nologo -p:Configuration=Release -t:Clean\;Build -p:TrackFileAccess=false