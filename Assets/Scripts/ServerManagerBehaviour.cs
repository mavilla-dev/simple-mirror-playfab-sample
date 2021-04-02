using Mirror;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ServerManagerBehaviour : MonoBehaviour
{
  TelepathyTransport _transport;

  // This variable needs to be hardcoded in your MultiplayerSettings.json for localVM
  // In addition to also being set on your PlayFab server build settings.
  string _playFabPortName = "game_port";

  private void OnEnable()
  {
    // We assume GameManager.Instance gets set on Awake
    // Destroy GameObject if running the client
    if (Configuration.Instance.IsClient)
    {
      enabled = false;
    }
  }

  void Start()
  {
    _transport = FindObjectOfType<TelepathyTransport>();
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
    _transport.port = serverPort;
    NetworkManager.singleton.networkAddress = serverIp;
    NetworkManager.singleton.StartServer();
    GameLogger.Info($"Serving is started on {NetworkManager.singleton.networkAddress}:{_transport.port}");
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
