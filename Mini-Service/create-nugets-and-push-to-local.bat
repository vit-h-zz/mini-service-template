set local_feed=C:\Local_Nuget_Feed

:: Register nuget feed
if not exist %local_feed% dotnet nuget add source %local_feed% -n %local_feed%

:: Create folder for feed
if not exist %local_feed% mkdir %local_feed%

:: Pack contract projects to the local nuget feed
dotnet pack ./Grpc.MiniService/Grpc.MiniService.csproj -o %local_feed%
dotnet pack ./Messaging.MiniService/Messaging.MiniService.csproj -o %local_feed%
dotnet pack ./OpenApi.MiniService/OpenApi.MiniService.csproj -o %local_feed%

::  dotnet pack --output nupkgs
:: Find all nuget packages and push to local nuget
::  for /r %%v in (*.nupkg) do (
	::  del "%local_feed%\%%~nxv"
	::  dotnet nuget push "%%v" -s %local_feed%
::  )

rem To install updated same version you need to clear VS .nuget cache and reinstall package to the solution

pause