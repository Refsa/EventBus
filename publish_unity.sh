dotnet build --framework net4.7.2 -c Release ./EventBus/EventBus.csproj

rm -rf ./UnityRelease/
mkdir -p ./UnityRelease/Lib/
mkdir -p ./UnityRelease/Source/

cp ./EventBus/bin/Release/net4.7.2/EventBus.dll ./UnityRelease/Lib/EventBus.dll

cp ./EventBus/GlobalEventBus.cs ./UnityRelease/Source/GlobalEventBus.cs
cp ./EventBus/MessageBus.cs ./UnityRelease/Source/MessageBus.cs
cp ./EventBus/MessageHandler.cs ./UnityRelease/Source/MessageHandler.cs
cp ./EventBus/MessageQueue.cs ./UnityRelease/Source/MessageQueue.cs