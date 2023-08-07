using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class FightSceneInit : NetworkBehaviour
{
    private bool[] ready = new bool[2];
    [SerializeField]
    private GameObject clientReady, hostReady, readyButton, panel;
    [SerializeField]
    private TextMeshProUGUI countdownTimer;
    private bool countdownStarted = false;
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
    private void Update()
    {
        if (ready[0] && ready[1] && IsHost && !countdownStarted)
        {
            readyButton.transform.position = new Vector3(readyButton.transform.position.x, 500f);
            StartCoroutine(Countdown());

        }
    }
    private IEnumerator Countdown()
    {   
        UpdateCountdownClientRpc(5);
        Debug.Log("5");
        countdownStarted = true;
        yield return new WaitForSecondsRealtime(1);
        UpdateCountdownClientRpc(4);
        Debug.Log("4");

        yield return new WaitForSecondsRealtime(1);
        UpdateCountdownClientRpc(3);
        Debug.Log("3");

        yield return new WaitForSecondsRealtime(1);
        UpdateCountdownClientRpc(2);
        Debug.Log("2");

        yield return new WaitForSecondsRealtime(1);
        UpdateCountdownClientRpc(1);
        Debug.Log("1");

        yield return new WaitForSecondsRealtime(1);
        UpdateCountdownClientRpc(0);
        Debug.Log("0");

        yield return new WaitForSecondsRealtime(1);
        HideShowPanelClientRpc(true);
        Debug.Log("Start");


    }
    [ClientRpc]
    private void UpdateCountdownClientRpc(int number)
    {
        countdownTimer.text = number.ToString();
    }
    [ClientRpc]
    private void HideShowPanelClientRpc(bool hide)
    {
        if (hide)
        {
            panel.transform.position = new Vector3(5000, 5000);
        }
        else
        {
            panel.transform.position = new Vector3(0, 0);
        }
    }
}
