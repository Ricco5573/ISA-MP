using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using UnityEngine;
using Unity.Collections;
using System;
using UnityEditor;
using Mono.Cecil.Cil;

public class Host : MonoBehaviour
{
    private MainMenu menu;
    private static string code;
    public struct RelayHostData
    {
        public string JoinCode;
        public string IPv4Address;
        public ushort Port;
        public Guid AllocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] Key;
    }
    private static RelayHostData hostData;
    public static async Task<RelayHostData> HostGame()
    {
        //Initialize the Unity Services engine
        await UnityServices.InitializeAsync();
        //Always autheticate your users beforehand
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            //If not already logged, log the user in
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        //Ask Unity Services to allocate a Relay server
        Allocation allocation = await Unity.Services.Relay.RelayService.Instance.CreateAllocationAsync(1);

        //Populate the hosting data
        RelayHostData data = new RelayHostData
        {
            // WARNING allocation.RelayServer is deprecated
            IPv4Address = allocation.RelayServer.IpV4,
            Port = (ushort)allocation.RelayServer.Port,

            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            Key = allocation.Key,
        };

        //Retrieve the Relay join code for our clients to join our party
        data.JoinCode = await Unity.Services.Relay.RelayService.Instance.GetJoinCodeAsync(data.AllocationID);
        code = data.JoinCode;
        hostData = data;
        return data;
    }
    public string GetJoinCode()
    {
        return code;
    }
    public RelayHostData GetData()
    {
        return hostData;
    }
}
