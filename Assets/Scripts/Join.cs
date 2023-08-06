using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using UnityEngine;
using System;

public class Join : MonoBehaviour
{
    private static RelayJoinData joinData;
    public struct RelayJoinData
    {
        public string IPv4Address;
        public ushort Port;
        public Guid AllocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] HostConnectionData;
        public byte[] Key;
    }

    /// <summary>
    /// Join a Relay server based on the JoinCode received from the Host or Server
    /// </summary>
    /// <param name="joinCode">The join code generated on the host or server</param>
    /// <returns>All the necessary data to connect</returns>
    public static async Task<RelayJoinData> JoinGame(string joinCode)
    {
        //Initialize the Unity Services engine
        await UnityServices.InitializeAsync();
        //Always authenticate your users beforehand
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            //If not already logged, log the user in
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        //Ask Unity Services for allocation data based on a join code
        JoinAllocation allocation = await Unity.Services.Relay.RelayService.Instance.JoinAllocationAsync(joinCode);

        //Populate the joining data
        RelayJoinData data = new RelayJoinData
        {
            // WARNING allocation.RelayServer is deprecated. It's best to read from ServerEndpoints.
            IPv4Address = allocation.RelayServer.IpV4,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            HostConnectionData = allocation.HostConnectionData,
            Key = allocation.Key,
        };
        joinData = data;
        return data;
    }
    public RelayJoinData GetJoinData()
    {
        return joinData;
    }
}
