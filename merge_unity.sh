git checkout Unity
git checkout main \
    ./EventBus/GlobalEventBus.cs \
    ./EventBus/MessageBus.cs \
    ./EventBus/MessageHandler.cs \
    ./EventBus/MessageQueue.cs \
    ./EventBus/MessagePipe.cs

git checkout Unity
git add .
git commit -m "Merged main"
git push

git checkout main