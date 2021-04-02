using UnityEngine;

class GameLogger
{
  public static void Info(object msg)
  {
    Debug.Log("[SimpleMirrorPlayFabSample] " + msg);
  }

  public static void LogError(object err)
  {
    Debug.LogError("[SimpleMirrorPlayFabSample] " + err);
  }
}
