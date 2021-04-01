using kcp2k;
using Mirror;
using System.Collections;
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
    PlayFab.PlayFabMultiplayerAgentAPI.OnServerActiveCallback += OnServerActive;
    PlayFab.PlayFabMultiplayerAgentAPI.OnShutDownCallback += OnShutDown;

    PlayFab.PlayFabMultiplayerAgentAPI.ReadyForPlayers();
    StartCoroutine(ShutDownServerInMinutes(5));
  }

  private IEnumerator ShutDownServerInMinutes(int minutes)
  {
    yield return new WaitForSeconds(60 * minutes);
    BeginShutDown();
  }

  private void OnServerActive()
  {
    _transport.Port = 7777;
    NetworkManager.singleton.StartServer();
    GameLogger.Info("STARTED SERVER");
  }

  private void OnShutDown()
  {
    BeginShutDown();
  }

  private void BeginShutDown()
  {
    Application.Quit();
  }
}
