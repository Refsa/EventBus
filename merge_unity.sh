git checkout Unity
git checkout main \
    ./EventBus/GlobalEventBus.cs \
    ./EventBus/MessageBus.cs \
    ./EventBus/MessageHandler.cs \
    ./EventBus/MessageQueue.cs

git add .
git commit -m "Merged main"

git checkout main