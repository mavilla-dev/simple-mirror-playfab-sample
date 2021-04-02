using kcp2k;
using Mirror;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ServerManagerBehaviour : MonoBehaviour
{
  [SerializeField] KcpTransport _transport;

  private void OnEnable()
  {
    // We assume GameManager.Instance gets set on Awake
    // Destroy GameObject if running the client
    if (Configuration.Instance.IsClient)
    {
      gameObject.SetActive(false);
      Destroy(gameObject);
    }
  }

  void Start()
  {
    GameLogger.Info("STARTING UP SERVER LOGIC");
    PlayFab.PlayFabMultiplayerAgentAPI.Start();
    PlayFab.PlayFabMultiplayerAgentAPI.OnServerActiveCallback += PF_OnServerActive;
    PlayFab.PlayFabMultiplayerAgentAPI.OnShutDownCallback += PF_OnShutDown;

    StartCoroutine(ReadyForPlayersInSeconds(5));
  }

  private IEnumerator ReadyForPlayersInSeconds(int seconds)
  {
    // Not sure why this would be needed yet...
    yield return new WaitForSeconds(seconds);
    
    PlayFab.PlayFabMultiplayerAgentAPI.ReadyForPlayers();

    StartCoroutine(ShutDownServerInMinutes(5));
  }

  private void PF_OnServerActive()
  {
    var connectionInfo = PlayFab.PlayFabMultiplayerAgentAPI.GetGameServerConnectionInfo();
    var gamePortData = connectionInfo.GamePortsConfiguration.Single(x => x.Name == "game_port");
    _transport.Port = (ushort)gamePortData.ServerListeningPort;
    // NetworkManager.singleton.networkAddress = PlayFab.PlayFabMultiplayerAgentAPI.PublicIpV4AddressKey;
    NetworkManager.singleton.StartServer();
    GameLogger.Info("STARTED SERVER");
  }

  private void PF_OnShutDown()
  {
    BeginShutDown();
  }

  private IEnumerator ShutDownServerInMinutes(int minutes)
  {
    yield return new WaitForSeconds(60 * minutes);
    BeginShutDown();
  }

  private void BeginShutDown()
  {
    Application.Quit();
  }
}
