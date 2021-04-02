using Mirror;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ServerManagerBehaviour : MonoBehaviour
{
  // This variable needs to be hardcoded in your MultiplayerSettings.json for localVM
  // In addition to also being set on your PlayFab server build settings.
  string _playFabPortName = "game_port";

  void Start()
  {
    GameLogger.Info("STARTING UP SERVER LOGIC");

    switch (Configuration.Instance.BuildType)
    {
      case BuildType.SERVER_ON_LOCAL:
        OpenServerOnIpAndPort(Configuration.Instance.LocalIp, Configuration.Instance.LocalPort);
        break;

      case BuildType.SERVER_ON_PLAYFAB:
        PreparePlayFabServer();
        break;
    }
  }

  void PreparePlayFabServer()
  {
    PlayFab.PlayFabMultiplayerAgentAPI.Start();
    PlayFab.PlayFabMultiplayerAgentAPI.OnServerActiveCallback += OnServerActive;
    PlayFab.PlayFabMultiplayerAgentAPI.OnShutDownCallback += OnServerShutDown;
    PlayFab.PlayFabMultiplayerAgentAPI.OnAgentErrorCallback += OnServerError;

    PlayFab.PlayFabMultiplayerAgentAPI.ReadyForPlayers();
  }

  private void OnServerError(string error)
  {
    GameLogger.LogError(error);
  }

  void OnServerActive()
  {
    var connectionInfo = PlayFab.PlayFabMultiplayerAgentAPI.GetGameServerConnectionInfo();
    var gamePortData = connectionInfo.GamePortsConfiguration.Single(x => x.Name == _playFabPortName);
    var playFabInternalIP = "localhost";

    OpenServerOnIpAndPort(playFabInternalIP, (ushort)gamePortData.ServerListeningPort);

    // StartCoroutine(ShutDownServerInMinutes(5));
  }

  void OnServerShutDown()
  {
    GameLogger.Info("Server starting shutdown process");
    BeginShutDown();
  }

  void OpenServerOnIpAndPort(string serverIp, ushort serverPort)
  {
    var activeTransport = Transport.activeTransport as TelepathyTransport;
    activeTransport.port = serverPort;
    NetworkManager.singleton.networkAddress = serverIp;
    
    NetworkManager.singleton.StartServer();

    GameLogger.Info($"Opening Server on: {NetworkManager.singleton.networkAddress}:{activeTransport.port}");
  }


  IEnumerator ShutDownServerInMinutes(int minutes)
  {
    yield return new WaitForSeconds(60 * minutes);

    BeginShutDown();
  }

  void BeginShutDown()
  {
    GameLogger.Info("Closing application");
    Application.Quit();
  }
}
