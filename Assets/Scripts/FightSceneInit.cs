using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FightSceneInit : NetworkBehaviour
{
    private bool[] ready = new bool[2];
    [SerializeField]
    private GameObject clientReady, hostReady;
    public void ReadyButton()
    {
        if (IsHost)
        {
            ServerReadyClientRpc();
            Debug.Log("Host ready");
        }
        else if (IsClient)
        {
            ClientReadyServerRpc();
            Debug.Log("Client ready");
        }
        
    }
    [ServerRpc(RequireOwnership = false)]
    public void ClientReadyServerRpc()
    {
        if (!ready[1])
        {   
            ready[1] = true;
            clientReady.transform.localPosition = new Vector3(clientReady.transform.localPosition.x, 0f);
        }
        else
        {
            ready[1] = false;
            clientReady.transform.localPosition = new Vector3(clientReady.transform.localPosition.x, 500f);
        }
    }
    [ClientRpc]
    public void ServerReadyClientRpc()
    {
        if (!ready[0])
        {
            ready[0] = true;
            hostReady.transform.localPosition = new Vector3(hostReady.transform.localPosition.x, 0f);
        }
        else
        {
            ready[0] = false;
            hostReady.transform.localPosition = new Vector3(hostReady.transform.localPosition.x, 500f);
        }
    }
}
