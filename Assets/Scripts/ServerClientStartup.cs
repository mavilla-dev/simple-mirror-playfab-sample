using UnityEngine;

public class ServerClientStartup : MonoBehaviour
{
  private void Start()
  {
    if (Configuration.Instance.IsServer)
      gameObject.AddComponent<ServerManagerBehaviour>();

    if (Configuration.Instance.IsClient)
      gameObject.AddComponent<ClientManagerBehaviour>();
  }
}
