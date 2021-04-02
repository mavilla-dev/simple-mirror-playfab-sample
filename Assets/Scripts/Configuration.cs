using PlayFab.MultiplayerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BuildType
{
  SERVER,
  CLIENT_TO_PLAYFAB,
  CLIENT_TO_PLAYFAB_LOCALE,
}

public class Configuration : MonoBehaviour
{
  public static Configuration Instance = null;

  [Tooltip("Decides if this is a host or a client")]
  public BuildType BuildType;

  [Tooltip("CustomID to use as login for PlayFab")]
  public string UserName;

  [Tooltip("Build ID of your PlayFab server")]
  public string BuildID;

  [Tooltip("Region where build exists")]
  public AzureRegion[] AzureRegions;

  private void Awake()
  {
    Instance = this;
    Debug.Log($"[SimpleMirrorPlayFabSample] RUNNING AS {BuildType}");
  }

  public bool IsServer => BuildType == BuildType.SERVER;
  public bool IsClient => BuildType == BuildType.CLIENT_TO_PLAYFAB || BuildType == BuildType.CLIENT_TO_PLAYFAB_LOCALE;

  public List<string> GetAzureRegionList() => AzureRegions.Select(x => x.ToString()).ToList();
}
