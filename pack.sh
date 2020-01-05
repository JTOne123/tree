PACKDIR="packs"

mkdir -p $PACKDIR

dotnet pack -c Release -o $PACKDIR src/dotnet-pstree/dotnet-pstree.csproj
dotnet pack -c Release -o $PACKDIR src/dotnet-tree/dotnet-tree.csproj
