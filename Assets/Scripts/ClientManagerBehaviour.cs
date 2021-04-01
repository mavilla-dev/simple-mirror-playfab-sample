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

    RequestServer();
  }

  private void RequestServer()
  {
    RequestMultiplayerServerRequest request = new RequestMultiplayerServerRequest
    {
      BuildId = Configuration.Instance.BuildID,
      SessionId = Guid.NewGuid().ToString(),
      PreferredRegions = Configuration.Instance.GetAzureRegionList()
    };

    PlayFab.PlayFabMultiplayerAPI.RequestMultiplayerServer(request, OnRequestMPServer, OnError);
  }

  private void OnRequestMPServer(RequestMultiplayerServerResponse obj)
  {
    _transport.Port = (ushort)obj.Ports.First().Num;
    NetworkManager.singleton.networkAddress = obj.IPV4Address;

    NetworkManager.singleton.StartClient();
  }
}
