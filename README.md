# simple-mirror-playfab-sample
Project is a very simple example of how  to host a server on PlayFab and connect to it from Unity.

Project has a single `SampleScene` which will run the server hosting or client connecting logic. The scene has the following GameObjects:
- MainCamera
- DirectionalLight
- Server Manager - Holds `ServerManagerBehaviour.cs` script which houses all the PlayFab hosting logic.
- Client Manager - Holds `ClientManagerBehaviour.cs` script which houses all the PlayFab login and Mirror Connection
- Game Manager - Holds `GameManager` which has a `BuildType` to switch between `CLIENT` or `SERVER` mode.


## PlayFab Resources

[GSDK Location](https://github.com/PlayFab/gsdk/tree/master/UnityGsdk) 
- Needed for Dedicated Server API Calls

[PlayFab SDK Location](https://github.com/PlayFab/UnitySDK/tree/master/Packages) 
- Look for the `PlayFabEditorExtensions.unitypackage` which will install the PlayFab SDK for you

The packages used for this project are within the `PlayFabPackages` Folder at the root.

