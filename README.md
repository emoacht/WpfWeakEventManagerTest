# WPF WeakEventManager Test

Test [PropertyChangedEventManager](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.propertychangedeventmanager) in single and multi thread.

`PropertyChangedEventManager.RemoveListener` method fails when called in thread pool.