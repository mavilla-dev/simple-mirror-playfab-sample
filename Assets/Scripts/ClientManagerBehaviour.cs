using Mirror;
using PlayFab.MultiplayerModels;
using System.Linq;
using UnityEngine;

public class ClientManagerBehaviour : MonoBehaviour
{
  // PlayFab Login Info
  PlayFab.ClientModels.LoginResult _loginData = null;

  void Start()
  {
    PlayFab.ClientModels.LoginWithCustomIDRequest request = new PlayFab.ClientModels.LoginWithCustomIDRequest();
    request.CustomId = Configuration.Instance.UserName;
    request.CreateAccount = true;

    PlayFab.PlayFabClientAPI.LoginWithCustomID(request, OnLogin, OnError);
  }

  private void OnError(PlayFab.PlayFabError obj)
  {
    GameLogger.LogError(obj.GenerateErrorReport());
  }

  private void OnLogin(PlayFab.ClientModels.LoginResult obj)
  {
    GameLogger.Info("Login was successful");
    _loginData = obj;

    switch (Configuration.Instance.BuildType)
    {
      case BuildType.CLIENT_TO_LOCAL:
        ConnectToServerWithIpAndPort(Configuration.Instance.LocalIp, Configuration.Instance.LocalPort);
        break;

      case BuildType.CLIENT_TO_PLAYFAB:
        RequestServer();
        break;

      case BuildType.CLIENT_TO_PLAYFAB_LOCALE:
        // 56100 is the NodePort on the MultiplayerSettings.json for the local VM
        ConnectToServerWithIpAndPort("localhost", 56100);
        break;
    }
  }

  private void RequestServer()
  {
    RequestMultiplayerServerRequest request = new RequestMultiplayerServerRequest
    {
      BuildId = Configuration.Instance.BuildID,
      SessionId = Configuration.Instance.SessionID,
      PreferredRegions = Configuration.Instance.GetAzureRegionList()
    };

    PlayFab.PlayFabMultiplayerAPI.RequestMultiplayerServer(request, OnRequestMPServer, OnError);
  }

  private void OnRequestMPServer(RequestMultiplayerServerResponse obj) =>
    ConnectToServerWithIpAndPort(
      obj.IPV4Address, 
      (ushort)obj.Ports.First(x => x.Name == "game_port").Num);

  private void ConnectToServerWithIpAndPort(string ipAddress, ushort gamePort)
  {
    var transport = Transport.activeTransport as TelepathyTransport;
    transport.port = gamePort;
    NetworkManager.singleton.networkAddress = ipAddress;

    GameLogger.Info($"Connecting to {NetworkManager.singleton.networkAddress}:{transport.port}");

    NetworkManager.singleton.StartClient();
  }
}
