using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;

public class PlayerReadyCheck : NetworkBehaviour
{
    private bool bolAlreadyStarted = false;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        bool AllPlayersReady = true;

        if (IsServer)
        {
            if (!(NetworkManager.Singleton.ConnectedClientsList.Count == 2)) { return; }



            foreach (var Networkclient in NetworkManager.Singleton.ConnectedClientsList)
            {
                AllPlayersReady = AllPlayersReady && Networkclient.PlayerObject.GetComponent<PlayerManager>().bolPlayerReady.Value;
                
            }

            if (AllPlayersReady && !bolAlreadyStarted)
            {
                fctClientStartTheGameClientRpc();
                bolAlreadyStarted = true;
            }
        }
    }

    public void fctSetPlayerReady()
    {
        // here we want to save the Object and the Position so that we can later send this Info to the server

        ulong localClientId = NetworkManager.Singleton.LocalClientId;
        // Try to get the local client object
        // Return if unsuccessful

        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient networkClient))
        {
            return;
        }
        // Try to get the TeamPlayer component from the player object
        // Return if unsuccessful
        if (!networkClient.PlayerObject.TryGetComponent<PlayerManager>(out var player))
        {
            return;
        }

        player.fctSetPlayerReadyServerRpc();
    }


    [ClientRpc]
    public void fctClientStartTheGameClientRpc()
    {
        GameObject.Find("RoundLogic").GetComponent<RoundLogic>().fctStart();
    }
}

