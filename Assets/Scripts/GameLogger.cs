using UnityEngine;

class GameLogger
{
  public static void Info(object msg)
  {
    Debug.Log("[SimpleMirrorPlayFabSample] " + msg);
  }

  public static void LogError(string err)
  {
    Debug.LogError("[SimpleMirrorPlayFabSample] " + err);
  }
}
