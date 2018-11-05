del *.nupkg
nuget pack Simplic.TableMonitor.Data.DB.csproj -properties Configuration=Debug
nuget push *.nupkg -Source http://simplic.biz:10380/nuget