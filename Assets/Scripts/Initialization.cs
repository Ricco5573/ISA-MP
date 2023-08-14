using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using System;
using Unity.Services.Authentication;
public class Initialization : MonoBehaviour
{
    async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
#if UNITY_EDITOR
        if (ParrelSync.ClonesManager.IsClone())
        {
            // When using a ParrelSync clone, switch to a different authentication profile to force the clone
            // to sign in as a different anonymous user account.
            string customArgument = ParrelSync.ClonesManager.GetArgument();
            AuthenticationService.Instance.SwitchProfile($"Clone_{customArgument}_Profile");
        }
#endif

        Debug.Log(AuthenticationService.Instance.Profile);
    }
}

