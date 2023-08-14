using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightSceneInit : NetworkBehaviour
{
    private bool[] ready = new bool[2];
    [SerializeField]
    private GameObject clientReady, hostReady, readyButton, panel, hostSpawn, clientSpawn, clientPlay, hostPlayer;
    [SerializeField]
    private TextMeshProUGUI countdownTimer;
    private bool countdownStarted = false;
    [SerializeField]
    private Camera uiCam;
    [SerializeField]
    private BattleManager bat;
    public void ReadyButton()
    {
        if (IsHost)
        {
            ServerReadyClientRpc();
            Debug.Log("Host ready");
        }
        else if (!IsHost)
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
        startBattleServerRpc();


    }
    [ClientRpc]
    private void UpdateCountdownClientRpc(int number)
    {
        countdownTimer.text = number.ToString();
    }
    [ClientRpc]
    public void HideShowPanelClientRpc(bool hide)
    {
        if (hide)
        {
            panel.transform.position = new Vector3(5000, 5000);
            uiCam.enabled = false;
        }
        else
        {
            panel.transform.position = new Vector3(0, 0);
            uiCam.enabled = true;
        }
    }

    [ServerRpc(RequireOwnership = false )]
    private void startBattleServerRpc()
    {
        Debug.Log("Starting battle");
        if (IsHost)
        {
            IReadOnlyList<ulong> id = NetworkManager.Singleton.ConnectedClientsIds;
            GameObject Host = Instantiate(hostPlayer, hostSpawn.transform.position, Quaternion.identity);
            Host.GetComponent<NetworkObject>().SpawnAsPlayerObject(id[0]);

            GameObject Client = Instantiate(clientPlay, clientSpawn.transform.position, Quaternion.identity);
            Client.GetComponent<NetworkObject>().SpawnAsPlayerObject(id[1]);
            bat.SetPlayers(Client, Host);

            SetCamerasClientRpc();
        }
    }

    [ClientRpc]
    private void SetCamerasClientRpc()
    {

        if (IsHost)
        {
            Camera hostCam = GameObject.FindGameObjectWithTag("HostCam").GetComponent<Camera>();
            hostCam.enabled = true;
            Camera clientCam = GameObject.FindGameObjectWithTag("ClientCam").GetComponent<Camera>();
            clientCam.enabled = false;
        }
        else if (!IsHost)
        {
            Camera clientCam = GameObject.FindGameObjectWithTag("ClientCam").GetComponent<Camera>();
            clientCam.enabled = true;
            Camera hostCam = GameObject.FindGameObjectWithTag("HostCam").GetComponent<Camera>();
            hostCam.enabled = false;
        }
    }
    public void ResetScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Fight", LoadSceneMode.Single);
    }
}
