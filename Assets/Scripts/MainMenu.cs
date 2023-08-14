using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Relay;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Unity.Netcode;
public class MainMenu : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI joinCodeHost;
    [SerializeField]
    private GameObject mainMenu, joinMenu, hostMenu, hostOBJ, joinOBJ, fullServer;
    [SerializeField]
    private InputField codeField;
    [SerializeField]
    private NetworkManager nM;
    private Join join;


    public void Back()
    {

        if (joinMenu.activeSelf)
        {
            joinMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
        else if (hostMenu.activeSelf)
        {
            hostMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
    }
    public void SetJoinCode(string code)
    {
        joinCodeHost.text = code;

    }
    public async void JoinMenu()
    {

        Debug.Log(codeField.text.Length);
        await Join.JoinGame(codeField.text);
        UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
        transport.SetRelayServerData(join.GetJoinData().IPv4Address, join.GetJoinData().Port, join.GetJoinData().AllocationIDBytes, join.GetJoinData().Key, join.GetJoinData().ConnectionData);
        nM.StartHost();
        Debug.Log(codeField.text);


    }

    public void startJoin()
    {
        mainMenu.SetActive(false);
        joinMenu.SetActive(true);

        join = Instantiate(joinOBJ, transform.position, Quaternion.identity).GetComponent<Join>();


    }
    public async void startHost()
    {
        mainMenu.SetActive(false);

        Debug.Log("Starting server");
        Host host = Instantiate(hostOBJ, transform.position, Quaternion.identity).GetComponent<Host>();
       await Host.HostGame();
        Debug.Log("Server started");

        UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
       transport.SetRelayServerData(host.GetData().IPv4Address, host.GetData().Port, host.GetData().AllocationIDBytes, host.GetData().Key, host.GetData().ConnectionData);
       SetJoinCode(host.GetJoinCode());
       Debug.Log("Joincode send");
        nM.StartHost();
        hostMenu.SetActive(true);
        fullServer.SetActive(false);

    }

    public void StartGame()
    {

        Debug.Log(nM.SceneManager);
        NetworkManager.Singleton.SceneManager.LoadScene("Fight", LoadSceneMode.Single);
    }

    private void Update()
    {
        if (nM.IsHost)
        {
            if (nM.ConnectedClientsList.Count >= 2)
            {
                fullServer.SetActive(true);


            }
            else
            {
                fullServer.SetActive(false);
            }
        }

        }
    }
