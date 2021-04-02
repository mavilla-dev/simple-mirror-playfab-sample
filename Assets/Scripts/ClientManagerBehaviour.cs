using kcp2k;
using Mirror;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using System;
using System.Linq;
using UnityEngine;

public class ClientManagerBehaviour : MonoBehaviour
{
  [SerializeField] KcpTransport _transport;

  // PlayFab Login Info
  LoginResult _loginData = null;

  private void OnEnable()
  {
    // We assume GameManager.Instance gets set on Awake
    // Destroy GameObject if running the server
    if (Configuration.Instance.IsServer)
    {
      gameObject.SetActive(false);
      Destroy(gameObject);
    }
  }

  void Start()
  {
    LoginWithCustomIDRequest request = new LoginWithCustomIDRequest();
    request.CustomId = Configuration.Instance.UserName;
    request.CreateAccount = true;

    PlayFab.PlayFabClientAPI.LoginWithCustomID(request, OnLogin, OnError);
  }

  private void OnError(PlayFabError obj)
  {
    GameLogger.LogError(obj.GenerateErrorReport());
    Application.Quit();
  }

  private void OnLogin(LoginResult obj)
  {
    GameLogger.Info("Login was successful");
    _loginData = obj;

    switch (Configuration.Instance.BuildType)
    {
      case BuildType.CLIENT_TO_PLAYFAB:
        RequestServer();
        break;

      case BuildType.CLIENT_TO_PLAYFAB_LOCALE:
        // 56100 is the NodePort on the MultiplayerSettings.json for the local VM
        StartClientConnection("localhost", 56100);
        break;
    }
  }

  private void RequestServer()
  {
    RequestMultiplayerServerRequest request = new RequestMultiplayerServerRequest
    {
      BuildId = Configuration.Instance.BuildID,
      SessionId = Guid.NewGuid().ToString(),
      PreferredRegions = Configuration.Instance.GetAzureRegionList()
    };

    GameLogger.Info($"Requesting Server with SessionID:{request.SessionId}");
    PlayFab.PlayFabMultiplayerAPI.RequestMultiplayerServer(request, OnRequestMPServer, OnError);
  }

  private void OnRequestMPServer(RequestMultiplayerServerResponse obj) =>
    StartClientConnection(obj.IPV4Address, (ushort)obj.Ports.First().Num);

  private void StartClientConnection(string ipAddress, ushort gamePort)
  {
    _transport.Port = gamePort;
    NetworkManager.singleton.networkAddress = ipAddress;

    GameLogger.Info($"Connecting to {NetworkManager.singleton.networkAddress}:{_transport.Port}");

    NetworkManager.singleton.StartClient();
  }
}
