using PlayFab.MultiplayerModels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BuildType
{
  SERVER_ON_LOCAL,
  SERVER_ON_PLAYFAB,
  CLIENT_TO_LOCAL,
  CLIENT_TO_PLAYFAB,
  CLIENT_TO_PLAYFAB_LOCALE,
}

public class Configuration : MonoBehaviour
{
  public static Configuration Instance = null;

  [Header("Server Settings")]
  [Tooltip("Decides if this is a host or a client")]
  public BuildType BuildType;
  [Tooltip("Build ID of your PlayFab server")]
  public string BuildID;
  [Tooltip("Region where build exists")]
  public AzureRegion[] AzureRegions;  

  [Header("Client Values")]
  [Tooltip("CustomID to use as login for PlayFab")]
  public string UserName;
  public ushort LocalPort = 3600;
  public string LocalIp = "localhost";
  public string SessionID = "2e562cbe-913e-48af-b1e3-a76a1cf6a5eb";

  private void Awake()
  {
    Instance = this;
    GameLogger.Info($"[SimpleMirrorPlayFabSample] RUNNING AS {BuildType}");
  }

  public bool IsServer => BuildType == BuildType.SERVER_ON_PLAYFAB || BuildType == BuildType.SERVER_ON_LOCAL;
  public bool IsClient => BuildType == BuildType.CLIENT_TO_PLAYFAB
    || BuildType == BuildType.CLIENT_TO_PLAYFAB_LOCALE
    || BuildType == BuildType.CLIENT_TO_LOCAL;

  public List<string> GetAzureRegionList() => AzureRegions.Select(x => x.ToString()).ToList();
}
