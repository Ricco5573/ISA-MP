using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private bool battling;
    private GameObject client, host;
    [SerializeField]
    private FightSceneInit init;

    [ServerRpc (RequireOwnership = false)]
    public void ClientWinServerRpc()
    {
        host.GetComponent<NetworkObject>().Despawn();
        //show text showing who won
    }
    [ServerRpc (RequireOwnership = false)]
    public void HostWinServerRpc()
    {
        client.GetComponent<NetworkObject>().Despawn();
        //show text
        StartCoroutine(ShowWin(true));
    }
    public void SetPlayers(GameObject Client, GameObject Host)
    {
        client = Client;
        host = Host;
    }


    private IEnumerator ShowWin(bool hostWin)
    {
        // show text
        init.HideShowPanelClientRpc(false);
        yield return new WaitForSecondsRealtime(5);
        init.ResetScene();
    }
}
