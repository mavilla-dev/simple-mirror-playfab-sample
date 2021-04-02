
# simple-mirror-playfab-sample
Simple example of a dedicated server along with connecting client using Mirror and PlayFab.

## Tech
- Unity 2020.3.0f1
- Mirror 35.1.0
- PlayFabEditorExtension [Last Updated Dec 19, 2020](https://github.com/PlayFab/UnityEditorExtensions/tree/master/Packages) 
	- This installed PlayFabSDK v2.104.210208
	- Upgrade to 2.105 was available however it was never successful
- PlayFab GSDK [Last Updated Nov 3, 2020](https://github.com/PlayFab/gsdk/blob/master/UnityGsdk/PlayFabMultiplayerSDK.unitypackage)
	- This is different from the PlayFab SDK

## Build Setup
It is **VERY** important that your unity project's Build Settings > Architecture is set as `x86x64`. Azure requires 64bit executables and your deploys will never succeed without this.

If starting from scratch, I recommend you first install the PlayFabEditorExtension package (using Assets > Import Package > Custom Package menu item) and then installing the SDK files using the UI. Once you have this fully installed and you logged into your account through this, then install the GSDK. I had issues trying to do this backwards. The Fix was to delete the PlayFabSDK folder that was created and importing the packages again in the order specified.

## Scenes
Project contains a single scene, `SampleScene`. This scene contains the following GameObjects:
- Configuration
	- Some parameters to help run the project as a server or a client
- Network Manager
	- The Mirror NetworkManager component and the Telepathy Transport
	- NOTE: This project uses Telepathy Transport even though KCP Transport is Mirror's new default. PlayFab does not support KCP protocol so we need to use a TCP/UDP transport. Telepathy was the previous default so I went with this.
- World
	- Contains spawn points for players that are connecting. Only 5 points are in the scene but we allow up to 100 connections which should just place cubes on top of each other.
	- MainCamera and Lighting were thrown under here as well for organization purposes

## Scripts
#### ClientManagerBehaviour.cs
Controls the logic for acting as a client to connect to various locations. Code should be mostly self explanatory. Please keep in mind that regardless of build type, when client, we always log into PlayFab using the CustomID method. This most likely isn't needed when connecting to your server locally off of the PlayFab VM

#### Configuration.cs
Contains several `BuildType` options to help the build decide where to connect. In addition, it holds some other options that can be modified based on your test of where you are trying to host your server.
- `BuildType.SERVER_ON_LOCAL` will run the mirror server on localhost and will skip any PlayFab server calls
- `BuildType.SERVER_ON_PLAYFAB` will run the mirror server and necessary PlayFab Server calls to ensure PlayFab hosting works correctly. Please note that this option also works if you are hosting your PlayFab VM locally.
- `BuildType.CLIENT_TO_LOCAL` Logs into PlayFab and connects to localhost as a client
- `BuildType.CLIENT_TO_PLAYFAB` Logs into PlayFab and uses the RequestServerAPI to find a hosted server connection and then attemps to connect to that. `BuildID` needs to be filled out for this to work
- `BuildType.CLIENT_TO_PLAYFAB_LOCALE` Logs into PlayFab and connects to the VM on localhost

#### GameLogger.cs
Just a helper class for logging with a prefix to easily identify what logs are coming from my scripts versus something else.
#### ServerClientStartup.cs
Another helper that checks if we are trying to run as a server or client and instantiates the appropriate script into the scene.


## Resources
TODO - Update with other Repos that helped

[GSDK Location](https://github.com/PlayFab/gsdk/tree/master/UnityGsdk) 
- Needed for Dedicated Server API Calls

[PlayFab SDK Location](https://github.com/PlayFab/UnitySDK/tree/master/Packages) 
- Look for the `PlayFabEditorExtensions.unitypackage` which will install the PlayFab SDK for you
